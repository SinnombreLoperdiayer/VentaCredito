using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Produccion.Comun;
using CO.Servidor.Produccion.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Produccion
{
  public class PRProduccionDeprecated : ControllerBase
  {
    #region creacion Instancias

    private static readonly PRProduccionDeprecated instancia = (PRProduccionDeprecated)FabricaInterceptores.GetProxy(new PRProduccionDeprecated(), COConstantesModulos.MODULO_PRODUCCION);

    /// <summary>
    /// Retorna una instancia de administracion de produccion
    /// /// </summary>
    public static PRProduccionDeprecated Instancia
    {
      get { return PRProduccionDeprecated.instancia; }
    }

    #endregion creacion Instancias

    #region Metodos

    /// <summary>
    /// Adicionar novedad de Forma de Pago al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void AdicionarNovedadCentroServicioFormaPago(PANovedadCentroServicioDCDeprecated novedad)
    {
      string idMotivoNovedad = PRRepositorioDeprecated.Instancia.ObtenerParametrosProduccion(PRConstantesProduccion.ID_NOVEDAD_CAMBIO_FORMA_PAGO_PRODUCCION);
      PRRepositorioDeprecated.Instancia.AdicionarNovedadCentroServicioFormaPago(novedad, idMotivoNovedad);
    }

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void AdicionarNovedadCentroServicioCambioDestino(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRRepositorioDeprecated.Instancia.AdicionarNovedadCentroServicioCambioDestino(novedad);
    }

    /// <summary>
    /// Realiza la aprobacion de la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void AprobarLiquidacionProduccion(PRLiquidacionManualDCDeprecated liquidacion)
    {
      GIAdmisionGirosDC giro = new GIAdmisionGirosDC();

      IPUFachadaCentroServicios fachadaCentroSvc = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
      IADFachadaAdmisionesGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();

      ///consulta la informacion del centro de servicios al que se le va a hacer la liquidacion
      PUCentroServiciosDC centroServicios = fachadaCentroSvc.ObtenerCentroServiciosPersonaResponsable(liquidacion.IdCentroServicios);

      ///Consulta la informacion del racol del usuario que esta haciendo la liquidacion
      PUCentroServiciosDC centroServiciosRacol = fachadaCentroSvc.ObtenerCentroServiciosPersonaResponsable(liquidacion.IdRacolLiquidacion);

      AREmpresaDC empresaRacol = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerDatosEmpresa();

      if (liquidacion.TotalFavorAgencia > 0)
      {
        ///Validar que la agencia pueda pagar giros
        if (COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServiciosPuedePagarGiros(liquidacion.IdCentroServicios))
        {
          giro.AgenciaDestino = centroServicios;
          giro.AgenciaOrigen = centroServiciosRacol;

          giro.AgenciaOrigen.PersonaResponsable.IdTipoIdentificacion = "NI";

          giro.AgenciaOrigen.PersonaResponsable.Identificacion = empresaRacol.Nit;
          giro.AgenciaOrigen.PersonaResponsable.PrimerNombre = empresaRacol.NombreEmpresa;
          giro.AgenciaOrigen.PersonaResponsable.PrimerApellido = string.Empty;
          giro.AgenciaOrigen.PersonaResponsable.SegundoApellido = string.Empty;

          giro.Precio = new TAPrecioDC()
          {
            ValorGiro = liquidacion.TotalFavorAgencia,
            ValorTotal = liquidacion.TotalFavorAgencia,
            TarifaPorcPorte = 0,
            TarifaFijaPorte = 0
          };
        }
      }
      else if (liquidacion.TotalFavorInter > 0)
      {
        giro.AgenciaDestino = centroServiciosRacol;
        giro.AgenciaOrigen = centroServicios;

        giro.AgenciaDestino.PersonaResponsable.IdTipoIdentificacion = "NI";

        giro.AgenciaDestino.PersonaResponsable.Identificacion = empresaRacol.Nit;
        giro.AgenciaDestino.PersonaResponsable.PrimerNombre = empresaRacol.NombreEmpresa;
        giro.AgenciaDestino.PersonaResponsable.PrimerApellido = string.Empty;
        giro.AgenciaDestino.PersonaResponsable.SegundoApellido = string.Empty;

        giro.Precio = new TAPrecioDC()
        {
          ValorGiro = liquidacion.TotalFavorInter,
          ValorTotal = liquidacion.TotalFavorInter,
          TarifaPorcPorte = 0,
          TarifaFijaPorte = 0
        };
      }

      giro.GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
      {
        ClienteDestinatario = new CLClienteContadoDC()
        {
          TipoId = giro.AgenciaDestino.PersonaResponsable.IdTipoIdentificacion,
          Identificacion = giro.AgenciaDestino.PersonaResponsable.Identificacion,
          Nombre = giro.AgenciaDestino.PersonaResponsable.PrimerNombre,
          Apellido1 = giro.AgenciaDestino.PersonaResponsable.PrimerApellido,
          Apellido2 = giro.AgenciaDestino.PersonaResponsable.SegundoApellido,
          Telefono = giro.AgenciaDestino.Telefono1,
          Direccion = giro.AgenciaDestino.Direccion,
          TipoIdentificacionReclamoGiro = giro.AgenciaDestino.PersonaResponsable.IdTipoIdentificacion,
          Ocupacion = new PAOcupacionDC()
          {
            IdOcupacion = 0,
            DescripcionOcupacion = string.Empty
          }
        },
        ClienteRemitente = new CLClienteContadoDC()
        {
          TipoId = giro.AgenciaOrigen.PersonaResponsable.IdTipoIdentificacion,
          Identificacion = giro.AgenciaOrigen.PersonaResponsable.Identificacion,
          Nombre = giro.AgenciaOrigen.PersonaResponsable.PrimerNombre,
          Apellido1 = giro.AgenciaOrigen.PersonaResponsable.PrimerApellido,
          Apellido2 = giro.AgenciaOrigen.PersonaResponsable.SegundoApellido,
          Telefono = giro.AgenciaOrigen.Telefono1,
          Direccion = giro.AgenciaOrigen.Direccion,
          Ocupacion = new PAOcupacionDC()
          {
            IdOcupacion = 0,
            DescripcionOcupacion = string.Empty
          }
        }
      };

      giro.GuidDeChequeo = System.Guid.NewGuid().ToString();

      using (TransactionScope scope = new TransactionScope())
      {
        GINumeroGiro giroProduccion = fachadaGiros.CrearGiroProduccion(giro);
        liquidacion.NumeroGiro = giroProduccion.IdGiro.Value;
        liquidacion.PrefijoGiro = giroProduccion.PrefijoIdGiro;

        ADGuiaInternaDC guiaInterna = new ADGuiaInternaDC()
        {
          GestionOrigen = new ARGestionDC(),
          GestionDestino = new ARGestionDC(),
          DiceContener = string.Empty,
          DireccionDestinatario = giro.AgenciaDestino.Direccion,
          IdCentroServicioDestino = giro.AgenciaDestino.IdCentroServicio,
          NombreCentroServicioDestino = giro.AgenciaDestino.Nombre,
          IdCentroServicioOrigen = giro.AgenciaOrigen.IdCentroServicio,
          NombreCentroServicioOrigen = giro.AgenciaOrigen.Nombre,
          LocalidadDestino = new PALocalidadDC()
          {
            IdLocalidad = giro.AgenciaDestino.IdMunicipio,
            Nombre = giro.AgenciaDestino.NombreMunicipio,
          },
          LocalidadOrigen = new PALocalidadDC()
          {
            IdLocalidad = giro.AgenciaOrigen.IdMunicipio,
            Nombre = giro.AgenciaOrigen.NombreMunicipio
          },
          PaisDefault = new PALocalidadDC()
          {
            IdLocalidad = liquidacion.IdPaisDefecto,
            Nombre = liquidacion.NombrePaisDefecto
          },
          EsManual = false,
          TelefonoDestinatario = giro.AgenciaDestino.Telefono1,
          NombreDestinatario = giro.AgenciaDestino.PersonaResponsable.PrimerNombre + " " + giro.AgenciaDestino.PersonaResponsable.PrimerApellido + " " + giro.AgenciaDestino.PersonaResponsable.SegundoApellido,
          NombreRemitente = giro.AgenciaOrigen.PersonaResponsable.PrimerNombre + " " + giro.AgenciaDestino.PersonaResponsable.PrimerApellido + " " + giro.AgenciaDestino.PersonaResponsable.SegundoApellido,
          DireccionRemitente = giro.AgenciaOrigen.PersonaResponsable.Direccion,
          TelefonoRemitente = giro.AgenciaOrigen.PersonaResponsable.Telefono
        };

        liquidacion.NumeroGuiaInterna = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().AdicionarGuiaInterna(guiaInterna).NumeroGuia;
        PRRepositorioDeprecated.Instancia.ActualizaLiquidacionProduccionAprobada(liquidacion);

        scope.Complete();
      }
    }

    /// <summary>
    /// Anula la liquidacion de produccion
    /// </summary>
    /// <param name="liquidacion"></param>
    public void AnularLiquidacionProduccion(long liquidacion)
    {
      PRRepositorioDeprecated.Instancia.ActualizarEstadoLiquidacion(liquidacion, PRConstantesProduccion.ESTADO_LIQUIDACION_ANULADA);
    }

    #region Programacion

    /// <summary>
    /// Obtiene la programacion de las liquidaciones
    /// </summary>
    public List<PRProgramacionLiquidacionDCDeprecated> ObtenerProgramacionLiquidaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
    {
      return PRRepositorioDeprecated.Instancia.ObtenerProgramacionLiquidaciones(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
    }

    /// <summary>
    /// Obtener centros de servicios y racol
    /// </summary>
    /// <returns></returns>
    public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol()
    {
      return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>()
                  .ObtenerCentroServiciosRacol();
    }

    /// <summary>
    /// Obtener centros logisticos activos
    /// </summary>
    /// <returns></returns>
    public List<PUCentroServicioApoyo> ObtenerCentroLogistico()
    {
      return COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>()
                  .ObtenerCentroLogistico();
    }

    /// <summary>
    /// Obtiene los centros de servicios sin programar
    /// </summary>
    /// <param name="idRacol"></param>
    /// <param name="idCentroServicios"></param>
    /// <returns></returns>
    public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosSinPromagramar(long? idRacol, long? idCentroServicios)
    {
      return PRRepositorioDeprecated.Instancia.ObtenerCentroServiciosSinPromagramar(idRacol.Value, idCentroServicios.Value);
    }

    /// <summary>
    /// Guarda la programacion de la liquidacion
    /// </summary>
    /// <param name="programacion"></param>
    public void GuardarProgramacionLiquidacion(PRProgramacionLiquidacionDCDeprecated programacion)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        PRRepositorioDeprecated.Instancia.GuardarProgramacionLiquidacion(programacion);
        scope.Complete();
      }
    }

    /// <summary>
    /// Modifica la programacion
    /// </summary>
    /// <param name="programacion"></param>
    public void ModificarProgramacion(PRProgramacionLiquidacionDCDeprecated programacion)
    {
      using (TransactionScope scope = new TransactionScope())
      {
        PRRepositorioDeprecated.Instancia.EditarProgramacionLiquidacion(programacion);
        scope.Complete();
      }
    }

    #endregion Programacion

    #region Impresion

    /// <summary>
    /// Obtiene las liquidaciones de las agencias/puntos
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <returns></returns>
    public List<PRLiquidacionManualDCDeprecated> ObtenerLiquidacionesAprobadas(IDictionary<string, string> filtro)
    {
      return PRRepositorioDeprecated.Instancia.ObtenerLiquidacionesAprobadas(filtro);
    }

    #endregion Impresion

    #region Novedades

    /// <summary>
    /// Retona las novedades de los centros de servicios
    /// </summary>
    /// <param name="filtro"></param>
    /// <param name="campoOrdenamiento"></param>
    /// <param name="indicePagina"></param>
    /// <param name="registrosPorPagina"></param>
    /// <param name="ordenamientoAscendente"></param>
    /// <param name="totalRegistros"></param>
    /// <param name="idCentroServicio"></param>
    /// <returns></returns>
    public List<PANovedadCentroServicioDCDeprecated> ObtenerNovedadesCentroSvc(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicio)
    {
      return PRRepositorioDeprecated.Instancia.ObtenerNovedadesCentroSvc(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idCentroServicio);
    }

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    public List<PRTipoNovedadDCDeprecated> ObtenerTiposNovedad()
    {
      return PRRepositorioDeprecated.Instancia.ObtenerTiposNovedad();
    }

    /// <summary>
    /// Retorna los tipos de novedad
    /// </summary>
    /// <returns></returns>
    public List<PRMotivoNovedadDCDeprecated> ObtenerMotivoNovedadTipo(int idTipoNovedad)
    {
      return PRRepositorioDeprecated.Instancia.ObtenerMotivoNovedadTipo(idTipoNovedad);
    }

    /// <summary>
    /// Adicionar novedad de Cambio de destiono al Centro Servicio
    /// </summary>
    /// <param name="novedad"></param>
    public void GuardarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRRepositorioDeprecated.Instancia.GuardarNovedadCentroServicio(novedad);
    }

    /// <summary>
    /// Adiciona la Novedad del Centro de Servicio
    /// </summary>
    /// <param name="novedad">Data de la Novedad</param>
    public void AdicionarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
    {
      PRRepositorioDeprecated.Instancia.AdicionarNovedadCentroServicio(novedad);
    }

    #endregion Novedades

    #endregion Metodos
  }
}