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
    public class RAReglaTransitoUrbano : IReglaIntegraciones
    {
        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia DatosGuia = null;
            OUMensajeroDC DatosMensajero = null;
            RAResponsableDC Responsable = null;

            if (parametrosRegla.ContainsKey("guia"))
            {
                DatosGuia = (ADGuia)(parametrosRegla["guia"]);
                DatosMensajero = fachadaOperacionUrbana.ObtenerResponsableGuiaManifiestoUrbPorNGuia(DatosGuia.NumeroGuia);

                if (DatosMensajero == null || DatosMensajero.PersonaInterna == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_DATOS_MENSAJERO)));
                }

                Responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Mensajero.GetHashCode(), DatosMensajero.IdCentroServicio, DatosMensajero.PersonaInterna.Identificacion, DatosMensajero.PersonaInterna.Nombre);
            }
            return Responsable;
        }
    }
}
