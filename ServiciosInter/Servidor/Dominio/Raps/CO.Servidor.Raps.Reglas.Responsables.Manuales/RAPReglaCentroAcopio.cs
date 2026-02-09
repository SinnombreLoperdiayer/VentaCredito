using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaCentroAcopio : IReglaIntegraciones
    {
        private IADFachadaAdmisionesMensajeria fachadaAdmisionMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia datosGuia = null;
            RAResponsableDC responsable = null;
            RegistroSolicitudAppDC datosResponsable = null;
            PUCentroServiciosDC centroServicio = null;

            if (parametrosRegla.ContainsKey("guia"))
            {
                datosGuia = (ADGuia)(parametrosRegla["guia"]);
                datosResponsable = new RegistroSolicitudAppDC();

                if (datosGuia.IdCentroServicioEstado == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_NO_CONTIENE_ID_CENTRO_SERVICIO_ESTADO)));
                }

                centroServicio = fachadaCentroServicio.ObtenerTipoYResponsableCentroServicio(datosGuia.IdCentroServicioEstado);

                if (centroServicio == null || centroServicio.infoResponsable == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO)));
                }
                if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.RAC.ToString())
                {
                    responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Centro_de_Acopio.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_RESPONSABLE_DIFERENTE_RACOL)));
                }



            }
            return responsable;
        }
    }
}
