
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaReparto : IReglaIntegraciones
    {
        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia datosGuia = null;
            OUMensajeroDC datosMensajero = null;
            RAResponsableDC responsable = null;

            if (parametrosRegla.ContainsKey("guia"))
            {
                datosGuia = (ADGuia)(parametrosRegla["guia"]);
                datosMensajero = fachadaOperacionUrbana.ObtenerAsignacionMensajeroPorNumeroGuia(datosGuia.NumeroGuia);

                if (datosMensajero == null || datosMensajero.PersonaInterna == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_DATOS_MENSAJERO)));
                }

                responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), datosMensajero.IdCentroServicio, datosMensajero.PersonaInterna.Identificacion, datosMensajero.PersonaInterna.Nombre);
            }

            return responsable;
        }
    }
}
