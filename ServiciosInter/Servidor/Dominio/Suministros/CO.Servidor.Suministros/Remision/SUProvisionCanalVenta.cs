using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.CentroServicios;

namespace CO.Servidor.Suministros.Remision
{
  public class SUProvisionCanalVenta
  {
      IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
                      
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
    /// Obtiene los suministros asignados al canal de venta
    /// </summary>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<SUSuministro> ObtenerSuministrosAsignadosCanalVenta(long idCentroServicios)
    {
      return SURepositorioAdministracion.Instancia.ObtenerSuministrosAsignadosCanalVenta(idCentroServicios);
    }

    /// <summary>
    /// Genera la remision de suministros para el mensajero
    /// </summary>
    /// <param name="remision"></param>
    public long AdminRemisionCanalVenta(SURemisionSuministroDC remision)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        SUTrasladoSuministroDC traslado;
        SUConsumoSuministroDC consumo;
        long idProvisionSum;
        remision.IdGuiaInternaRemision = null; //guia.IdAdmisionGuia;
        remision.NumeroGuiaDespacho = null;//guia.NumeroGuia;

        switch (remision.CentroServicioAsignacion.Tipo)
        {
          case "PTO":
            remision.GrupoSuministroDestino = SUEnumGrupoSuministroDC.PTO;
            break;

          case "AGE":
            remision.GrupoSuministroDestino = SUEnumGrupoSuministroDC.AGE;
            break;

          case "RAC":
            remision.GrupoSuministroDestino = SUEnumGrupoSuministroDC.RAC;
            break;
        }

        ///Guarda la remision de suministros para el canal de venta
        remision.IdRemision = SURepositorioAdministracion.Instancia.GuardaRemisionSuministroCanalVenta(remision);

        ///crea y guarda la guia interna para la remision de los suministros
        if (remision.GeneraGuiaInterna)
        {
            ADGuiaInternaDC guia = GeneraGuiaInternaRemisionSuministros(remision);
            //Actualizar número de guía de la remisión
            SURepositorio.Instancia.ActualizarNumeroGuiaRemision(guia.IdAdmisionGuia, guia.NumeroGuia, remision.IdRemision);
        }

        remision.GrupoSuministros.SuministrosGrupo.ForEach(sum =>
        {
          ///Valida el suministro, si hay existencias y si los rangos no han sido asignadoss
          sum = Remision.ValidaSuministroAsignacion(sum);

          ///Guarda la provision de suministros
          idProvisionSum = SURepositorioAdministracion.Instancia.GuardarProvisionSuministrosCanalVenta(sum, remision.IdRemision, (int)sum.IdAsignacionSuministro);

          SUNumeradorAutomatico numeradorActual = null;

          ///si el suministro es guia interna realiza la actualizacion del numerador
          if (sum.Id == Convert.ToInt16(SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA) || sum.Id == Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE))
          {
            numeradorActual = SURepositorioAdministracion.Instancia.ActualizarNumeradorRango(sum.Id, sum.CantidadAsignada);
            sum.RangoInicial = numeradorActual.ValorActual - sum.CantidadAsignada + 1;
            sum.RangoFinal = numeradorActual.ValorActual;

            sum.FechaInicialResolucion = numeradorActual.FechaInicial;
            sum.FechaFinalResolucion = numeradorActual.FechaFinal;
            sum.IdResolucion = numeradorActual.IdNumerador;

            SURepositorioAdministracion.Instancia.GuardarProvisionSuminsitroSerialCanalVenta(sum, idProvisionSum, sum.RangoInicial, sum.RangoFinal, false);
            remision.GrupoSuministros.SuministroGrupo = sum;

            traslado = new SUTrasladoSuministroDC()
            {
              Cantidad = (int)remision.GrupoSuministros.SuministroGrupo.CantidadAsignada,
              GrupoSuministroDestino = remision.GrupoSuministroDestino,
              GrupoSuministroOrigen = SUEnumGrupoSuministroDC.PRO,
              IdentificacionDestino = remision.CentroServicioAsignacion.IdCentroServicio,
              IdentificacionOrigen = SUConstantesSuministros.ID_GESTION_TRASLADO,
              Suministro = (SUEnumSuministro)sum.Id
            };
            SUGrupoSuministrosDC descGrupo = SURepositorioAdministracion.Instancia.ObtieneInformacionGrupoSuministro(SUEnumGrupoSuministroDC.PRO);
            remision.GrupoSuministros.Descripcion = descGrupo.Descripcion;
            SURepositorio.Instancia.GuardaSuministroProvisionReferenciaCanalVenta(remision, traslado);
          }

          ///Si el suministro aplica resolucion guarda la provision de suministros serial
          if (sum.AplicaResolucion)
          {
            sum.RangoFinal = sum.RangoInicial + sum.CantidadAsignada - 1;
            SURepositorioAdministracion.Instancia.GuardarProvisionSuminsitroSerialCanalVenta(sum, idProvisionSum, sum.RangoInicial, sum.RangoFinal, false);
            remision.GrupoSuministros.SuministroGrupo = sum;

            if (sum.Id != Convert.ToInt16(SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA) && sum.Id != Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE))
            {
              traslado = new SUTrasladoSuministroDC()
              {
                Cantidad = (int)remision.GrupoSuministros.SuministroGrupo.CantidadAsignada,
                GrupoSuministroDestino = remision.GrupoSuministroDestino,
                GrupoSuministroOrigen = SUEnumGrupoSuministroDC.PRO,
                IdentificacionDestino = remision.CentroServicioAsignacion.IdCentroServicio,
                IdentificacionOrigen = SUConstantesSuministros.ID_GESTION_TRASLADO,
                Suministro = (SUEnumSuministro)sum.Id
              };

              SUGrupoSuministrosDC descGrupo = SURepositorioAdministracion.Instancia.ObtieneInformacionGrupoSuministro(remision.GrupoSuministroDestino);
              remision.GrupoSuministros.Descripcion = descGrupo.Descripcion;
              SURepositorio.Instancia.GuardaSuministroProvisionReferenciaCanalVenta(remision, traslado);
            }
          }
          else if (sum.Id != Convert.ToInt16(SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA) && sum.Id != Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE))
          {
            consumo = new SUConsumoSuministroDC()
            {
              Cantidad = (int)sum.CantidadAsignada,
              EstadoConsumo = SUEnumEstadoConsumo.CON,
              GrupoSuministro = remision.GrupoSuministroDestino,
              Suministro = (SUEnumSuministro)Enum.Parse(typeof(SUEnumSuministro), sum.Id.ToString()),
              IdServicioAsociado = SUConstantesSuministros.VALOR_DEFECTO_CERO,
              NumeroSuministro = SUConstantesSuministros.VALOR_DEFECTO_CERO,
              IdDuenoSuministro = remision.CentroServicioAsignacion.IdCentroServicio
            };
            var a = (int)consumo.Suministro;

            SURepositorio.Instancia.GuardarConsumoSuministro(consumo);
          }
        });

        scope.Complete();
        return remision.IdRemision;
      }
    }

    /// <summary>
    /// Genera la guia interna para la remision de los suministros del canal de ventas
    /// </summary>
    /// <param name="remision"></param>
    /// <returns></returns>
    public ADGuiaInternaDC GeneraGuiaInternaRemisionSuministros(SURemisionSuministroDC remision)
    {
      ADGuiaInternaDC guiaInterna = new ADGuiaInternaDC()
      {
        DiceContener = "Remisión de Despacho No.:" + remision.IdRemision,
        DireccionDestinatario = remision.CentroServicioAsignacion.Direccion,
        IdCentroServicioDestino = remision.CentroServicioAsignacion.IdCentroServicio,
        NombreCentroServicioDestino = remision.CentroServicioAsignacion.Nombre,
        LocalidadDestino = new PALocalidadDC()
        {
          IdLocalidad = remision.CiudadRemisionSuministro.IdLocalidad,
          Nombre = remision.CiudadRemisionSuministro.Nombre
        },
        PaisDefault = new PALocalidadDC()
        {
          IdLocalidad = remision.CasaMatrizGeneraRemision.IdPais,
          Nombre = remision.CasaMatrizGeneraRemision.NombrePais
        },
        EsManual = false,
        EsOrigenGestion = true,
        TelefonoDestinatario = remision.CentroServicioAsignacion.Telefono1,
        NombreDestinatario = remision.CentroServicioAsignacion.Nombre,
        NombreRemitente = remision.CasaMatrizGeneraRemision.DescripcionGestion,
        GestionDestino = new ARGestionDC()
        {
          IdGestion = remision.CentroServicioAsignacion.IdCentroServicio,
          Descripcion = remision.CentroServicioAsignacion.Nombre,
        },
        GestionOrigen = new ARGestionDC()
        {
          IdGestion = Convert.ToInt64(remision.CasaMatrizGeneraRemision.CodigoGestion),
          Descripcion = remision.CasaMatrizGeneraRemision.DescripcionGestion,
          IdCasaMatriz = remision.CasaMatrizGeneraRemision.IdCasaMatriz
        }
      };

      return Remision.CrearGuiaInternaRemision(guiaInterna);
    }

    /// <summary>
    /// Guarda el traslado del suministro del mensajero
    /// </summary>
    /// <param name="remisionDestino"></param>
    public long GuardarModificacionSuministroCanalVenta(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen, long rangoInicial, long rangoFinal)
    {
      long idProvisionSum;
      int idAsignacion = 0;
      ///crea y guarda la guia interna para la remision de los suministros del canal de venta
      //ADGuiaInternaDC guia = GeneraGuiaInternaRemisionSuministros(remisionDestino);
      remisionDestino.IdGuiaInternaRemision = null;
      remisionDestino.NumeroGuiaDespacho = null;

      ///Guarda la remision de suministros para el canal de venta
      remisionDestino.IdRemision = SURepositorioAdministracion.Instancia.GuardaRemisionSuministroCanalVenta(remisionDestino);

      idAsignacion = SURepositorioAdministracion.Instancia.ObtenerIdAsignacionSuministroCanalVenta(remisionDestino.GrupoSuministros.SuministroGrupo.Id, remisionDestino.CentroServicioAsignacion.IdCentroServicio);
      remisionDestino.GrupoSuministros.SuministroGrupo.CantidadAsignada = (int)(rangoFinal - rangoInicial) + 1;

      ///Guarda la provision de suministros
      idProvisionSum = SURepositorioAdministracion.Instancia.GuardarProvisionSuministrosCanalVenta(remisionDestino.GrupoSuministros.SuministroGrupo, remisionDestino.IdRemision, idAsignacion);

      ///Guarda el serial de la provision para los suministros del canal de venta
      SURepositorioAdministracion.Instancia.GuardarProvisionSuminsitroSerialCanalVenta(remisionDestino.GrupoSuministros.SuministroGrupo, idProvisionSum, rangoInicial, rangoFinal, true);
      return remisionDestino.IdRemision;
    }

    public long ModificarProvisionSuministroCanalVenta(SURemisionSuministroDC remisionDestino, SURemisionSuministroDC remisionOrigen)
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
        idRemision = GuardarModificacionSuministroCanalVenta(remisionDestino, remisionOrigen, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal);
        SURepositorio.Instancia.ActualizarSuministroProvisionReferencia(remisionDestino, remisionDestino.CentroServicioAsignacion.IdCentroServicio);

        if (remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial != remisionOrigen.GrupoSuministros.SuministroGrupo.RangoInicial)
          Remision.GuardarProvisionRestanteModificacionSuministro(remisionOrigen, remisionOrigen.GrupoSuministros.SuministroGrupo.RangoInicial, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoInicial);

        if (remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal != rangoFinal)
          Remision.GuardarProvisionRestanteModificacionSuministro(remisionOrigen, remisionOrigen.GrupoSuministros.SuministroGrupo.NuevoRangoFinal + 1, rangoFinal + 1);

        scope.Complete();
      }

      return idRemision;
    }

      /// <summary>
      /// Método ´para generar guias manuales offline desde la aplicacion POS
      /// </summary>
      /// <param name="idCentroServicio"></param>
      /// <param name="Cantidad"></param>
      /// <returns></returns>
    public SURemisionSuministroDC GenerarRangoGuiaManualOffline(long idCentroServicio, int Cantidad)
    {
       List<SUSuministro> listaSuministros = ObtenerSuministrosAsignadosCanalVenta(idCentroServicio);

       SUSuministro sum = listaSuministros.FirstOrDefault(su => su.Id == Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE));

         if (sum == null)
         {
             SUSuministroCentroServicioDC suministroCentroServicio = new SUSuministroCentroServicioDC()
             {
                  IdCentroServicio = idCentroServicio,
                   Suministro = new  List<SUSuministro>()
             };

             sum = new SUSuministro() { 
                 Id = Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE),
                 StockMinimo = 30, 
                 CantidadInicialAutorizada = Cantidad,
                 SuministroAutorizado = true
                };
             suministroCentroServicio.Suministro.Add(sum);             
             SURepositorioAdministracion.Instancia.GuardarSuministroCentroServicio(suministroCentroServicio);
             
             listaSuministros = ObtenerSuministrosAsignadosCanalVenta(idCentroServicio);
             sum = listaSuministros.FirstOrDefault(su => su.Id == Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE));

            // throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_ERROR_FALTA_ASIGNAR_SUMINISTRO_MANUAL_OFFLINE.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_FALTA_ASIGNAR_SUMINISTRO_MANUAL_OFFLINE))));
         }

        SUSuministro suministro = new SUSuministro()
                {
                    Id = Convert.ToInt16(SUEnumSuministro.GUIA_TRANSPORTE_MANUAL_OFFLINE),
                    CantidadAsignada = Cantidad,
                    AplicaResolucion = false,
                    IdAsignacionSuministro =sum.IdAsignacionSuministro
                };
        PUCentroServiciosDC centroServ = fachadaCentroServicio.ObtenerCentroServicio(idCentroServicio);

      
        SURemisionSuministroDC remision = new SURemisionSuministroDC()
        {
            CasaMatrizGeneraRemision = new ARCasaMatrizDC
            {
                IdCasaMatriz =  1,
                Nombre = string.Empty,
                IdPais = ConstantesFramework.ID_LOCALIDAD_COLOMBIA,
                NombrePais = ConstantesFramework.DESC_LOCALIDAD_COLOMBIA
            },
            CiudadRemisionSuministro = centroServ.CiudadUbicacion,
            FechaRemision = DateTime.Now,
            RangoFechaFinal = DateTime.Now.AddYears(1),
            RangoFechaInicial = DateTime.Now,
            CentroServicioAsignacion = centroServ,
            IdCasaMatriz = 1,
            IdGuiaInternaRemision = 0,
            NumeroGuiaDespacho = 0,
            Estado = PAEnumEstados.ACT,
            GrupoSuministros = new SUGrupoSuministrosDC
            {
                SuministrosGrupo = new List<SUSuministro>() { suministro }
            }
        };
        switch (centroServ.Tipo)
        {
            case "PTO":
                remision.GrupoSuministroDestino = SUEnumGrupoSuministroDC.PTO;
                break;

            case "AGE":
                remision.GrupoSuministroDestino = SUEnumGrupoSuministroDC.AGE;
                break;

            case "RAC":
                remision.GrupoSuministroDestino = SUEnumGrupoSuministroDC.RAC;
                break;
        }

        AdminRemisionCanalVenta(remision);
        return remision;
    }


  }
}