
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionNacional;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaTransitoNacional : IReglaIntegraciones
    {
        private IONFachadaOperacionNacional fachadaOperacionNacional = COFabricaDominio.Instancia.CrearInstancia<IONFachadaOperacionNacional>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia datosGuia = null;
            RAResponsableDC responsable = null;
            ONManifiestoOperacionNacional ManifiestoResponsable = null;

            if (parametrosRegla.ContainsKey("guia"))
            {
                datosGuia = (ADGuia)(parametrosRegla["guia"]);
                ManifiestoResponsable = fachadaOperacionNacional.ObtenerResponsableGuiaManifiestoPorNGuia(datosGuia.NumeroGuia);

                if (ManifiestoResponsable == null || ManifiestoResponsable.ManifiestoTerrestre == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_DATOS_MENSAJERO)));
                }

                responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), ManifiestoResponsable.IdRacolDespacho, ManifiestoResponsable.ManifiestoTerrestre.CedulaConductor, ManifiestoResponsable.ManifiestoTerrestre.NombreConductor);
            }
            return responsable;
        }
    }
}
