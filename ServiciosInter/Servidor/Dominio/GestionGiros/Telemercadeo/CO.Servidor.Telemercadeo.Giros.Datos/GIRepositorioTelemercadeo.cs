using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;
using CO.Servidor.Telemercadeo.Giros.Datos.Modelo;

//using CO.Servidor.Telemercadeo.Giros.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Telemercadeo.Giros.Datos
{
  public class GIRepositorioTelemercadeo
  {
    #region Campos

    private static readonly GIRepositorioTelemercadeo instancia = new GIRepositorioTelemercadeo();
    private const string NombreModelo = "ModeloTelemercadeoGiros";

    #endregion Campos

    #region Constructor

    /// <summary>
    /// Retorna la instancia de la clase GIRepositorioExploradorGiros
    /// </summary>
    public static GIRepositorioTelemercadeo Instancia
    {
      get { return GIRepositorioTelemercadeo.instancia; }
    }

    #endregion Constructor

    #region Generales

    /// <summary>
    /// Retorna los resultados de la gestion de telemercadeo
    /// </summary>
    /// <returns></returns>
    public List<GIResultadoGestionTelemercadeoDC> ObtenerResultadosGestionTelemercadeo()
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        return contexto.ResultadoTelemercadeoGiro_GIR.Where(r => r.RTG_Estado == ConstantesFramework.ESTADO_ACTIVO)
          .ToList()
          .ConvertAll(r => new GIResultadoGestionTelemercadeoDC()
        {
          IdResultado = r.RTG_IdResultadoTelemercadeoGiro,
          Descripcion = r.RTG_Descripcion,
          Estado = r.RTG_Estado
        });
      }
    }

    /// <summary>
    /// Actualiza la tabla de giros indicando que ya tiene telemercadeo
    /// </summary>
    /// <param name="numeroGiro"></param>
    public void ActualizarTelemercadeoAdmGiro(long numeroGiro)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        contexto.paActualizarTelemercadeoAdmGiro_GIR(numeroGiro);
      }
    }

    public void GuardarTelemercadeoGiro(GITelemercadeoGiroDC telemercadeo)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        GestionGiroTelemercadeo_GIR gestion = new GestionGiroTelemercadeo_GIR()
        {
          GTE_IdAdmisionGiro = telemercadeo.Giro.IdAdminGiro.Value,
          GTE_TipoCliente = telemercadeo.TipoCliente.IdTipoCliente,
          GTE_IdParentesco = telemercadeo.Parentesco.IdPariente,
          GTE_DescripcionParentesco = telemercadeo.Parentesco.NombrePariente,
          GTE_IdResultadoTelemercadeoGiro = telemercadeo.ResultadoTelemercadeo.IdResultado,
          GTE_TelefonoMarcado = telemercadeo.TelefonoMarcado,
          GTE_NumeroGiro = telemercadeo.Giro.IdGiro.Value,
          GTE_PersonaContesta = telemercadeo.PersonaContesta,
          GTE_Observacion = telemercadeo.Observaciones,
          GTE_CreadoPor = ControllerContext.Current.Usuario,
          GTE_EnviarEmailDestinatario = telemercadeo.EnvioCorreoDestinatario,
          GTE_EnviarEmailRemitente = telemercadeo.EnvioCorreoRemitente,
          GTE_FechaGrabacion = DateTime.Now,
        };

        contexto.GestionGiroTelemercadeo_GIR.Add(gestion);
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Retorna la informacion de telemercadeo
    /// </summary>
    /// <param name="idTelemercadeoGiro"></param>
    /// <returns></returns>
    public GITelemercadeoGiroDC ObtenerDetalleTelemercadeoGiro(long idTelemercadeoGiro)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        GestionGiroTelemercadeo_GIR gestion = contexto.GestionGiroTelemercadeo_GIR.Where(r => r.GTE_IdTelemercadeoGiro == idTelemercadeoGiro).SingleOrDefault();
        if (gestion == null)
        {
          ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_GESTION_GIROS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
          throw new FaultException<ControllerException>(excepcion);
        }

        return new GITelemercadeoGiroDC()
        {
          Observaciones = gestion.GTE_Observacion,
          Parentesco = new PAParienteDC()
          {
            IdPariente = gestion.GTE_IdParentesco,
            NombrePariente = gestion.GTE_DescripcionParentesco
          },
          PersonaContesta = gestion.GTE_PersonaContesta,
          TelefonoMarcado = gestion.GTE_TelefonoMarcado,
          ResultadoTelemercadeo = new GIResultadoGestionTelemercadeoDC()
          {
            IdResultado = gestion.GTE_IdResultadoTelemercadeoGiro
          },
          UsuarioRealizaGestion = gestion.GTE_CreadoPor,
          FechaGestion = gestion.GTE_FechaGrabacion,
          EnvioCorreoDestinatario = gestion.GTE_EnviarEmailDestinatario,
          EnvioCorreoRemitente = gestion.GTE_EnviarEmailRemitente,
          TipoCliente = new GITipoClienteDC()
          {
            IdTipoCliente = gestion.GTE_TipoCliente
          },
          Giro = new GIAdmisionGirosDC()
          {
            IdGiro = gestion.GTE_NumeroGiro,
            IdAdminGiro = gestion.GTE_IdAdmisionGiro
          }
        };
      }
    }

    /// <summary>
    /// Retorna el valor del parametro
    /// </summary>
    /// <param name="idParametro"></param>
    /// <returns></returns>
    public string ObtenerParametroGiros(string idParametro)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        ParametrosGiros_GIR parametro = contexto.ParametrosGiros_GIR.Where(r => r.PAG_IdParametro == idParametro).FirstOrDefault();
        if (parametro == null)
        {
          ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO.ToString(),
                                          GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO));
          throw new FaultException<ControllerException>(excepcion);
        }
        return parametro.PAG_ValorParametro;
      }
    }

    #endregion Generales

    #region Telemercadeo

    /// <summary>
    /// Obtiene los giros que cumplen con los tiempos para estar en telemercadeo
    /// </summary>
    /// <returns></returns>
    public List<GIAdmisionGirosDC> ObtenerGirosTelemercadeo(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idRacol, int diasPago)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        DateTime fechaInicial;
        DateTime fechaFinal;
        string idGiro;
        string idCentroServicio;

        filtro.TryGetValue("ADG_IdGiro", out idGiro);
        filtro.TryGetValue("ADG_IdCentroServicioDestino", out idCentroServicio);

        fechaInicial = DateTime.Now.AddDays(-30);
        fechaFinal = DateTime.Now.AddDays(-diasPago);

        return contexto.paObtenerGirosTelemercadeo_GIR
          (Convert.ToInt64(idGiro)
          , fechaInicial
          , fechaFinal
          , indicePagina
          , registrosPorPagina
          , idRacol
          , Convert.ToInt64(idCentroServicio)
          )
          .ToList()
          .ConvertAll(r => new GIAdmisionGirosDC()
          {
            IdGiro = r.ADG_IdGiro,
            IdAdminGiro = r.ADG_IdAdmisionGiro,
            FechaGrabacion = r.ADG_FechaGrabacion,
            NombreDestinatario = r.ADG_NombreDestinatario,
            IdTipoGiro = r.ADG_IdTipoGiro,
            AgenciaDestino = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC()
            {
              IdCentroServicio = r.ADG_IdCentroServicioDestino,
              Nombre = r.ADG_NombreCentroServicioDestino
            },
            AgenciaOrigen = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC()
            {
              IdCentroServicio = r.ADG_IdCentroServicioOrigen,
              Nombre = r.ADG_NombreCentroServicioOrigen
            },
            EstadoGiro = r.UltimoEstadoGiro,
            TieneGestion = r.ADG_TieneTelemercadeo,
            Precio = new Servicios.ContratoDatos.Tarifas.TAPrecioDC()
            {
              ValorGiro = r.ADG_ValorGiro
            },
          });
      }
    }

    /// <summary>
    /// Obtiene el historico de telemercadeo de un giro
    /// </summary>
    /// <param name="idGiro"></param>
    public List<GITelemercadeoGiroDC> ObtenerHistoricoTelemercadeoGiro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idGiro)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        filtro.Add("GTE_IdAdmisionGiro", idGiro.ToString());
        return contexto.ConsultarContainsGestionGiroTelemercadeo_GIR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
          .ConvertAll(r => new GITelemercadeoGiroDC()
          {
            IdTelemercadeo = r.GTE_IdTelemercadeoGiro,
            TelefonoMarcado = r.GTE_TelefonoMarcado,
            ResultadoTelemercadeo = new GIResultadoGestionTelemercadeoDC()
            {
              IdResultado = r.GTE_IdResultadoTelemercadeoGiro,
              Descripcion = contexto.ResultadoTelemercadeoGiro_GIR.Where(p => p.RTG_IdResultadoTelemercadeoGiro == r.GTE_IdResultadoTelemercadeoGiro).FirstOrDefault().RTG_Descripcion
            },
            PersonaContesta = r.GTE_PersonaContesta,
            FechaGestion = r.GTE_FechaGrabacion,
            UsuarioRealizaGestion = r.GTE_CreadoPor,
            Parentesco = new PAParienteDC()
            {
              IdPariente = r.GTE_IdParentesco,
              NombrePariente = r.GTE_DescripcionParentesco
            },
            TipoCliente = new GITipoClienteDC()
            {
              IdTipoCliente = r.GTE_TipoCliente
            }
          });
      }
    }

    /// <summary>
    /// Obtiene la informacion de Telemercadeo de
    /// un giro especifico
    /// </summary>
    /// <param name="idAdmisionGiro"></param>
    /// <returns></returns>
    public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
    {
      using (ModeloTelemercadeoGiros contexto = new ModeloTelemercadeoGiros(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        GITelemercadeoGiroDC telemercadeoGiro = null;
        var infoTelemercadeoGiro = contexto.GestionGiroTelemercadeo_GIR.Include("ResultadoTelemercadeoGiro_GIR").FirstOrDefault(idTel => idTel.GTE_IdAdmisionGiro == idAdmisionGiro);

        if (infoTelemercadeoGiro != null)
        {
          telemercadeoGiro = new GITelemercadeoGiroDC()
          {
            IdTelemercadeo = infoTelemercadeoGiro.GTE_IdTelemercadeoGiro,
            TelefonoMarcado = infoTelemercadeoGiro.GTE_TelefonoMarcado,
            ResultadoTelemercadeo = new GIResultadoGestionTelemercadeoDC()
            {
              IdResultado = infoTelemercadeoGiro.GTE_IdResultadoTelemercadeoGiro,
              Descripcion = infoTelemercadeoGiro.ResultadoTelemercadeoGiro_GIR.RTG_Descripcion
            },
            PersonaContesta = infoTelemercadeoGiro.GTE_PersonaContesta,
            FechaGestion = infoTelemercadeoGiro.GTE_FechaGrabacion,
            UsuarioRealizaGestion = infoTelemercadeoGiro.GTE_CreadoPor,
            Parentesco = new PAParienteDC()
            {
              IdPariente = infoTelemercadeoGiro.GTE_IdParentesco,
              NombrePariente = infoTelemercadeoGiro.GTE_DescripcionParentesco
            },
            TipoCliente = new GITipoClienteDC()
            {
              IdTipoCliente = infoTelemercadeoGiro.GTE_TipoCliente
            }
          };
        }
        return telemercadeoGiro;
      }
    }

    #endregion Telemercadeo
  }
}