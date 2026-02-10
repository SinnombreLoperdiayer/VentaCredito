using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Integraciones.Novasoft.Proxies;

namespace CO.Servidor.Suministros.Remision
{
  internal class SUProvisionProceso
  {
    /// <summary>
    /// Retorna instancia del configurador de suministros
    /// </summary>
    internal CO.Servidor.Suministros.Remision.SURemision Remision
    {
      get
      {
        return new SURemision();
      }
    }

    /// <summary>
    /// Genera la remision de suministros para un proceso
    /// </summary>
    /// <param name="remision"></param>
    public long AdminRemisionProceso(SURemisionSuministroDC remision)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        SUTrasladoSuministroDC traslado;
        SUConsumoSuministroDC consumo;
        long idProvisionSum;
        remision.CentroServicioAsignacion.IdCentroCostos = remision.ProcesoAsignacion.CentroCostos;
        remision.CentroServicioAsignacion.CodigoBodega = remision.ProcesoAsignacion.CodBodegaErp;

        remision.CasaMatrizDestinoRemision = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerCasaMatrizProceso(remision.ProcesoAsignacion.IdProceso);

        remision.IdGuiaInternaRemision = null;// guia.IdAdmisionGuia;
        remision.NumeroGuiaDespacho = null;// guia.NumeroGuia;

        ///Guarda la remision de suministros para el proceso
        remision.IdRemision = SURepositorio.Instancia.GuardaRemisionSuministroProceso(remision);

        if (remision.GeneraGuiaInterna)
        {
            ///crea y guarda la guia interna para la remision de los suministros del mensajero
            ADGuiaInternaDC guia = GeneraGuiaInternaRemisionSuministros(remision);
            //Actualizar número de guía de la remisión
            SURepositorio.Instancia.ActualizarNumeroGuiaRemision(guia.IdAdmisionGuia, guia.NumeroGuia, remision.IdRemision);
        }

        ///Valida los rangos de los suminoistros
        remision.GrupoSuministros.SuministrosGrupo.ForEach(sum =>
        {
          sum = Remision.ValidaSuministroAsignacion(sum);

          SUNumeradorAutomatico numeradorActual = null;

          ///Guarda la provision de suministros
          idProvisionSum = SURepositorio.Instancia.GuardarProvisionSuministrosProceso(sum, remision.IdRemision, (int)sum.IdAsignacionSuministro);

          ///si el suministro es guia interna realiza la actualizacion del numerador
          if (sum.Id == SUConstantesSuministros.ID_SUMINISTRO_GUIA_INTERNA)
          {
            numeradorActual = SURepositorioAdministracion.Instancia.ActualizarNumeradorRango(sum.Id, sum.CantidadAsignada);

            numeradorActual = SURepositorioAdministracion.Instancia.ActualizarNumeradorRango(sum.Id, sum.CantidadAsignada);
            sum.RangoInicial = numeradorActual.ValorActual - sum.CantidadAsignada + 1;
            sum.RangoFinal = numeradorActual.ValorActual;

            sum.FechaInicialResolucion = numeradorActual.FechaInicial;
            sum.FechaFinalResolucion = numeradorActual.FechaFinal;
            sum.IdResolucion = numeradorActual.IdNumerador;

            SURepositorio.Instancia.GuardarProvisionSuministroSerialProceso(sum, idProvisionSum, sum.RangoInicial, sum.RangoFinal, false);
            remision.GrupoSuministros.SuministroGrupo = sum;

            traslado = new SUTrasladoSuministroDC()
            {
              Cantidad = (int)remision.GrupoSuministros.SuministroGrupo.CantidadAsignada,
              GrupoSuministroDestino = SUEnumGrupoSuministroDC.PRO,
              GrupoSuministroOrigen = SUEnumGrupoSuministroDC.PRO,
              IdentificacionDestino = remision.ProcesoAsignacion.IdProceso,
              IdentificacionOrigen = SUConstantesSuministros.ID_GESTION_TRASLADO,
              Suministro = (SUEnumSuministro)sum.Id
            };
            SUGrupoSuministrosDC descGrupo = SURepositorioAdministracion.Instancia.ObtieneInformacionGrupoSuministro(SUEnumGrupoSuministroDC.PRO);
            remision.GrupoSuministros.Descripcion = descGrupo.Descripcion;
            SUReferenciaSuministro.Instancia.GuardaSuministroProvisionReferenciaProceso(remision, traslado);
          }

          ///Si el suministro aplica resolucion guarda la provision de suministros serial
          if (sum.AplicaResolucion)
          {
            SURepositorio.Instancia.GuardarProvisionSuministroSerialProceso(sum, idProvisionSum, sum.RangoInicial, sum.RangoFinal, false);
            remision.GrupoSuministros.SuministroGrupo = sum;

            traslado = new SUTrasladoSuministroDC()
            {
              Cantidad = (int)remision.GrupoSuministros.SuministroGrupo.CantidadAsignada,
              GrupoSuministroDestino = SUEnumGrupoSuministroDC.PRO,
              GrupoSuministroOrigen = SUEnumGrupoSuministroDC.PRO,
              IdentificacionDestino = remision.ProcesoAsignacion.IdProceso,
              IdentificacionOrigen = SUConstantesSuministros.ID_GESTION_TRASLADO,
              Suministro = (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), sum.Id)
            };
            SUGrupoSuministrosDC descGrupo = SURepositorioAdministracion.Instancia.ObtieneInformacionGrupoSuministro(SUEnumGrupoSuministroDC.PRO);
            remision.GrupoSuministros.Descripcion = descGrupo.Descripcion;
            SUReferenciaSuministro.Instancia.GuardaSuministroProvisionReferenciaProceso(remision, traslado);
          }
          else if (sum.Id != SUConstantesSuministros.ID_SUMINISTRO_GUIA_INTERNA)
          {
            consumo = new SUConsumoSuministroDC()
            {
              Cantidad = (int)sum.CantidadAsignada,
              EstadoConsumo = SUEnumEstadoConsumo.CON,
              GrupoSuministro = SUEnumGrupoSuministroDC.PRO,
              Suministro = (SUEnumSuministro)Enum.ToObject(typeof(SUEnumSuministro), sum.Id),
              IdServicioAsociado = SUConstantesSuministros.VALOR_DEFECTO_CERO,
              NumeroSuministro = SUConstantesSuministros.VALOR_DEFECTO_CERO,
              IdDuenoSuministro = remision.ProcesoAsignacion.IdProceso
            };

            SURepositorio.Instancia.GuardarConsumoSuministro(consumo);
          }
        });

        scope.Complete();

        return remision.IdRemision;
      }
    }

    /// <summary>
    /// Genera la guia interna para la remision
    /// </summary>
    public ADGuiaInternaDC GeneraGuiaInternaRemisionSuministros(SURemisionSuministroDC remision)
    {
      ARCasaMatrizDC casaMatrizOrigen = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerCasaMatriz(remision.CasaMatrizGeneraRemision.IdCasaMatriz);

      ADGuiaInternaDC guiaInterna = new ADGuiaInternaDC();

      if (remision.CasaMatrizDestinoRemision == null)
        remision.CasaMatrizDestinoRemision = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerCasaMatrizProceso(remision.ProcesoAsignacion.IdProceso);

      guiaInterna.DiceContener = "Remisión de Despacho No.:" + remision.IdRemision;

      guiaInterna.PaisDefault = new PALocalidadDC()
      {
        IdLocalidad = remision.CasaMatrizGeneraRemision.IdPais,
        Nombre = remision.CasaMatrizGeneraRemision.NombrePais
      };
      guiaInterna.GestionDestino = new ARGestionDC()
      {
        IdGestion = remision.CasaMatrizDestinoRemision.CodigoGestion,
        Descripcion = remision.CasaMatrizDestinoRemision.DescripcionGestion,
        IdCasaMatriz = remision.CasaMatrizDestinoRemision.IdCasaMatriz
      };
      guiaInterna.LocalidadDestino = new PALocalidadDC()
      {
        IdLocalidad = remision.CasaMatrizDestinoRemision.IdLocalidad,
        Nombre = remision.CasaMatrizDestinoRemision.NombreLocalidad
      };
      guiaInterna.LocalidadOrigen = new PALocalidadDC()
      {
        IdLocalidad = remision.CasaMatrizGeneraRemision.IdLocalidad,
        Nombre = remision.CasaMatrizGeneraRemision.NombreLocalidad
      };
      guiaInterna.EsDestinoGestion = true;
      guiaInterna.EsOrigenGestion = true;
      guiaInterna.EsManual = false;
      guiaInterna.NombreRemitente = casaMatrizOrigen.Nombre;
      guiaInterna.TelefonoRemitente = casaMatrizOrigen.Telefono;
      guiaInterna.DireccionRemitente = casaMatrizOrigen.Direccion;
      guiaInterna.TelefonoDestinatario = remision.CasaMatrizDestinoRemision.Telefono;
      guiaInterna.NombreDestinatario = remision.CasaMatrizDestinoRemision.Nombre;
      guiaInterna.DireccionDestinatario = remision.CasaMatrizDestinoRemision.Direccion;

      guiaInterna.GestionOrigen = new ARGestionDC()
      {
        IdGestion = remision.CasaMatrizGeneraRemision.CodigoGestion,
        Descripcion = remision.CasaMatrizGeneraRemision.DescripcionGestion,
        IdCasaMatriz = remision.CasaMatrizGeneraRemision.IdCasaMatriz
      };

      return Remision.CrearGuiaInternaRemision(guiaInterna);
    }

    /// <summary>
    /// Guarda el traslado del suministro del proceso
    /// </summary>
    /// <param name="remisionDestino"></param>
    public long GuardarModificacionSuministroProceso(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen, long rangoInicial, long rangoFinal)
    {
      long idProvisionSum;
      int idAsignacion = 0;
      ///crea y guarda la guia interna para la remision de los suministros del mensajero
      //ADGuiaInternaDC guia = GeneraGuiaInternaRemisionSuministros(remisionDestino);
      remisionDestino.IdGuiaInternaRemision = null;
      remisionDestino.NumeroGuiaDespacho = null;

      ///Guarda la remision de suministros para el mensajero
      remisionDestino.IdRemision = SURepositorio.Instancia.GuardaRemisionSuministroProceso(remisionDestino);

      ///Obtiene el id de la asignacion de los suministros
      idAsignacion = SURepositorioAdministracion.Instancia.ObtenerInfoAsignacionSuministroProceso(remisionOrigen.GrupoSuministros.SuministroGrupo.Id, remisionDestino.ProcesoAsignacion.IdProceso);
      remisionOrigen.GrupoSuministros.SuministroGrupo.CantidadAsignada = (int)(rangoFinal - rangoInicial) + 1;

      ///Guarda la provision de suministros
      idProvisionSum = SURepositorio.Instancia.GuardarProvisionSuministrosProceso(remisionOrigen.GrupoSuministros.SuministroGrupo, remisionDestino.IdRemision, idAsignacion);

      ///Guarda el serial de la provision para los suministros del mensajero
      SURepositorio.Instancia.GuardarProvisionSuministroSerialProceso(remisionOrigen.GrupoSuministros.SuministroGrupo, idProvisionSum, rangoInicial, rangoFinal, true);
      return remisionDestino.IdRemision;
    }

    public long ModificarProvisionSuministroProceso(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen)
    {
      long idRemision = 0;
      ///Valida que los rangos de la nueva remision no esten consumidos
      for (long i = remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial; i <= remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal; i++)
      {
        ///Valida que el suministro no haya sido consumido y realiza la desasignacion
        if (SURepositorioAdministracion.Instancia.ValidaConsumoSuministro(remisionOrigen.GrupoSuministros.SuministroGrupo.Id, i))
        {
          throw new FaultException<ControllerException>
          (new ControllerException(COConstantesModulos.MODULO_SUMINISTROS,
           EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO.ToString(),
           string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_DESASIGNAR_SUMINISTRO), remisionOrigen.GrupoSuministros.Descripcion, i)));
        }
      }
      long rangoFinal = remisionOrigen.GrupoSuministros.SuministroGrupo.RangoFinal;
      long rangoInicial = remisionOrigen.GrupoSuministros.SuministroGrupo.RangoInicial;
      long nuevoRangoFinal = remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal;
      long nuevoRangoInicial = remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial;

      using (TransactionScope scope = new TransactionScope())
      {
        ///Envia true para q se realice solo el proceso de desasignacion correspondiente
        Remision.DesasignarSuministrosRemision(remisionOrigen, true);
        ///Si los nuevos rangos son iguales a los rangos provisionados, realiza el proceso de modificacion GuardarModificacionSuministroMensajero
        idRemision = GuardarModificacionSuministroProceso(remisionDestino, remisionOrigen, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal);
        SURepositorio.Instancia.ActualizarSuministroProvisionReferencia(remisionDestino, remisionDestino.ProcesoAsignacion.IdProceso);

        if (remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial != remisionOrigen.GrupoSuministros.SuministroGrupo.RangoInicial)
          Remision.GuardarProvisionRestanteModificacionSuministro(remisionOrigen, remisionOrigen.GrupoSuministros.SuministroGrupo.RangoInicial, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial);

        if (remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal != rangoFinal)
          Remision.GuardarProvisionRestanteModificacionSuministro(remisionOrigen, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal + 1, rangoFinal + 1);

        scope.Complete();
      }

      return idRemision;
    }
  }
}