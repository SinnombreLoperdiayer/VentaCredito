using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;
using CO.Servidor.Telemercadeo.Giros.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;

namespace CO.Servidor.Telemercadeo.Giros
{
  public class GITelemercadeoGiros : ControllerBase
  {
    private static readonly GITelemercadeoGiros instancia = (GITelemercadeoGiros)FabricaInterceptores.GetProxy(new GITelemercadeoGiros(), COConstantesModulos.GIROS);

    public static GITelemercadeoGiros Instancia
    {
      get { return GITelemercadeoGiros.instancia; }
    }

    #region Generales

    /// <summary>
    /// Retorna los resultados de la gestion de telemercadeo
    /// </summary>
    /// <returns></returns>
    public List<GIResultadoGestionTelemercadeoDC> ObtenerResultadoGestionTelemercadeo()
    {
      return GIRepositorioTelemercadeo.Instancia.ObtenerResultadosGestionTelemercadeo();
    }

    #endregion Generales

    #region Telemercadeo

    /// <summary>
    /// Obtiene los giros que cumplen con los tiempos para estar en telemercadeo
    /// </summary>
    /// <returns></returns>
    public List<GIAdmisionGirosDC> ObtenerGirosTelemercadeo(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idRacol)
    {
      ///Obtiene el valor del parametro para la cantidad de dias disponibles para realizar el pago
      string valorDiasPago = GIRepositorioTelemercadeo.Instancia.ObtenerParametroGiros(GIConstantesTelemercadeo.PARAMETRO_DIAS_DISP_PAGO_GIR);

      int dias;
      if (Int32.TryParse(valorDiasPago, out dias))
        return GIRepositorioTelemercadeo.Instancia.ObtenerGirosTelemercadeo(filtro, indicePagina, registrosPorPagina, idRacol, Convert.ToInt32(valorDiasPago));
      else
      {
        ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO.ToString(),
                                         GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO));
        throw new FaultException<ControllerException>(excepcion);
      }
    }

    /// <summary>
    /// Guardar telemercadeo giro
    /// </summary>
    /// <param name="telemercadeo"></param>
    public void GuardarTelemercadeoGiro(GITelemercadeoGiroDC telemercadeo)
    {
      if (telemercadeo.EnvioCorreoRemitente)
      {
        if (!string.IsNullOrEmpty(telemercadeo.Giro.EmailRemitente))
        {
          Task.Factory.StartNew(() =>
          {
            try
            {
              PAAdministrador.Instancia.EnviarCorreoTelemercadeoGiroRemitente(telemercadeo.Giro.AgenciaOrigen.Nombre, telemercadeo.Giro.AgenciaOrigen.CiudadUbicacion.Nombre, telemercadeo.Giro.AgenciaOrigen.Direccion, telemercadeo.Giro.EmailRemitente);
            }
            catch (Exception ex)
            {
              AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.GIROS);
            }
          }, TaskCreationOptions.PreferFairness);
        }
      }

      if (telemercadeo.EnvioCorreoDestinatario)
      {
        if (!string.IsNullOrEmpty(telemercadeo.Giro.EmailDestinatario))
        {
          Task.Factory.StartNew(() =>
          {
            try
            {
              PAAdministrador.Instancia.EnviarCorreoTelemercadeoGiroDetinatario(telemercadeo.Giro.NombreRemitente, telemercadeo.Giro.AgenciaDestino.CiudadUbicacion.Nombre, telemercadeo.Giro.AgenciaDestino.Direccion, telemercadeo.Giro.FechaGrabacion.AddDays(30), telemercadeo.Giro.EmailDestinatario);
            }
            catch (Exception ex)
            {
              AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.GIROS);
            }
          }, TaskCreationOptions.PreferFairness);
        }
      }

      using (TransactionScope scope = new TransactionScope())
      {
        ///Guarda el telemercadeo del giro
        GIRepositorioTelemercadeo.Instancia.GuardarTelemercadeoGiro(telemercadeo);
        ///Actualiza la tabla de admision giros indicando que el giro ya tiene telemercadeo
        GIRepositorioTelemercadeo.Instancia.ActualizarTelemercadeoAdmGiro(telemercadeo.Giro.IdGiro.Value);

        scope.Complete();
      }
    }

    /// <summary>
    /// Obtiene la informacion del giro
    /// </summary>
    /// <param name="giro"></param>
    public GIAdmisionGirosDC ObtenerInformacionGiroTelemercadeo(GIAdmisionGirosDC giro)
    {
      IADFachadaAdmisionesGiros fachadaGiros = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesGiros>();
      switch (giro.IdTipoGiro)
      {
        case "PP":
          giro.GirosPeatonPeaton = fachadaGiros.ConsultarInformacionPeatonPeaton(giro.IdAdminGiro.Value).GirosPeatonPeaton;
          ///Destinatario
          giro.NombreDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Nombre + ' ' + giro.GirosPeatonPeaton.ClienteDestinatario.Apellido1 + ' ' + giro.GirosPeatonPeaton.ClienteDestinatario.Apellido2;
          giro.TelefonoDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Telefono;
          giro.EmailDestinatario = giro.GirosPeatonPeaton.ClienteDestinatario.Email;
          ///Remitente
          giro.NombreRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Nombre + ' ' + giro.GirosPeatonPeaton.ClienteRemitente.Apellido1 + ' ' + giro.GirosPeatonPeaton.ClienteRemitente.Apellido2;
          giro.TelefonoRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Telefono;
          giro.EmailRemitente = giro.GirosPeatonPeaton.ClienteRemitente.Email;

          break;

        case "PC":
          giro.GirosPeatonConvenio = fachadaGiros.ConsultarInformacionPeatonConvenio(giro.IdAdminGiro.Value).GirosPeatonConvenio;
          ///Destinatario
          giro.NombreDestinatario = giro.GirosPeatonConvenio.ClienteConvenio.RazonSocial;
          giro.TelefonoDestinatario = giro.GirosPeatonConvenio.ClienteConvenio.Telefono;
          //giro.EmailDestinatario = giro.GirosPeatonConvenio.ClienteConvenio.
          ///Remitente
          giro.NombreRemitente = giro.GirosPeatonConvenio.ClienteContado.Nombre + ' ' + giro.GirosPeatonPeaton.ClienteRemitente.Apellido1 + ' ' + giro.GirosPeatonPeaton.ClienteRemitente.Apellido2 == null ? string.Empty : giro.GirosPeatonPeaton.ClienteRemitente.Apellido2;
          giro.TelefonoRemitente = giro.GirosPeatonConvenio.ClienteContado.Telefono;
          giro.EmailRemitente = giro.GirosPeatonConvenio.ClienteContado.Email;
          break;

        default:
          ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_TIPO_GIRO_NO_CONFIGURADO.ToString(),
                                         GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_TIPO_GIRO_NO_CONFIGURADO));
          throw new FaultException<ControllerException>(excepcion);
      }
      ///Obtiene el valor del parametro para la cantidad de dias disponibles para realizar el pago
      string valorDiasPago = GIRepositorioTelemercadeo.Instancia.ObtenerParametroGiros(GIConstantesTelemercadeo.PARAMETRO_DIAS_LIMITE_PAGO_GIRO);

      int dias;
      if (Int32.TryParse(valorDiasPago, out dias))
        giro.FechaLimitePagoGiro = giro.FechaGrabacion.AddDays(dias);
      else
      {
        ControllerException excepcion = new ControllerException(COConstantesModulos.GIROS, GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO.ToString(),
                                         GISolicitudesServidorMensajes.CargarMensaje(GIEnumTipoErrorSolicitud.ERROR_PARAMETRO_NO_CONFIGURADO));
        throw new FaultException<ControllerException>(excepcion);
      }

      giro.AgenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerInformacionCentroServicioPorId(giro.AgenciaDestino.IdCentroServicio);
      giro.AgenciaOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerInformacionCentroServicioPorId(giro.AgenciaOrigen.IdCentroServicio);

      return giro;
    }

    /// <summary>
    /// Retorna la informacion de telemercadeo
    /// </summary>
    /// <param name="idTelemercadeoGiro"></param>
    /// <returns></returns>

    public GITelemercadeoGiroDC ObtenerDetalleTelemercadeoGiro(long idTelemercadeoGiro)
    {
      return GIRepositorioTelemercadeo.Instancia.ObtenerDetalleTelemercadeoGiro(idTelemercadeoGiro);
    }

    /// <summary>
    /// Obtiene el historico de telemercadeo de un giro
    /// </summary>
    /// <param name="idGiro"></param>
    public List<GITelemercadeoGiroDC> ObtenerHistoricoTelemercadeoGiro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idGiro)
    {
      return GIRepositorioTelemercadeo.Instancia.ObtenerHistoricoTelemercadeoGiro(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idGiro);
    }

    /// <summary>
    /// Obtiene la informacion de Telemercadeo de
    /// un giro especifico
    /// </summary>
    /// <param name="idAdmisionGiro"></param>
    /// <returns>la info del telemercadeo de un giro</returns>
    public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
    {
      return GIRepositorioTelemercadeo.Instancia.ObtenerTelemercadeoDeGiro(idAdmisionGiro);
    }

    #endregion Telemercadeo
  }
}