
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaBodega : IReglaIntegraciones
    {

        private IADFachadaAdmisionesMensajeria fachadaAdmisionMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();
        private IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            RAResponsableDC responsable = new RAResponsableDC();

            if (parametrosRegla.ContainsKey("guia"))
            {
                ADGuia datosGuia = (ADGuia)(parametrosRegla["guia"]);

                PUCentroServiciosDC centroServicio = fachadaCentroServicios.ObtenerTipoYResponsableCentroServicio(datosGuia.IdCentroServicioOrigen);

                if (centroServicio == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO)));
                }

                responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
            }
            return responsable;
        }
    }
}
