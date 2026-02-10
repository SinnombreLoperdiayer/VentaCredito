using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaEntregada : IReglaIntegraciones
    {
        private IADFachadaAdmisionesMensajeria fachadaAdmisionMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia datosGuia = null;
            ADEstadoGuia estadoGuia = null;
            int conversion, idMensajero;
            OUDatosMensajeroDC datosMensajero = null;
            RAResponsableDC responsable = null;
            PUCentroServiciosDC centroServicio = null;

            if (parametrosRegla.ContainsKey("guia"))
            {
                datosGuia = (ADGuia)(parametrosRegla["guia"]);
                estadoGuia = fachadaAdmisionMensajeria.ObtenerEstadoGuiaTrazaPorIdEstado(ADEnumEstadoGuia.Entregada, datosGuia.NumeroGuia);

                if (estadoGuia == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_TRAZA_POR_ESTADO)));
                }

                if (int.TryParse(estadoGuia.CreadoPor, out conversion))
                {
                    datosMensajero = fachadaOperacionUrbana.ObtenerDatosMensajeroPorNumeroDeCedula(estadoGuia.CreadoPor);

                    if (datosMensajero == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_DATOS_MENSAJERO)));
                    }

                    responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), datosMensajero.IdCentroServicios, datosMensajero.Identificacion, string.Format("{0} {1}", datosMensajero.NombreMensajero, datosMensajero.PrimerApellido));
                }

                else
                {
                    centroServicio = fachadaCentroServicio.ObtenerTipoYResponsableCentroServicio(datosGuia.IdCentroServicioOrigen);

                    if (centroServicio == null || centroServicio.infoResponsable == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO)));
                    }

                    if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.AGE.ToString())
                    {
                        responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Agencias.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                    }

                    else if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.PTO.ToString())
                    {
                        responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Puntos.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                    }
                }
            }
            return responsable;
        }
    }


}
