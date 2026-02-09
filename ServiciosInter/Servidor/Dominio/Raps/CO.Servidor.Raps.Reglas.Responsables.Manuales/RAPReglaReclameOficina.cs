
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using System.Collections.Generic;
using System.ServiceModel;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAPReglaReclameOficina : IReglaIntegraciones
    {
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            ADGuia datosGuia = null;
            RAResponsableDC responsable = null;
            PUCentroServiciosDC centroServicio = null;

            if (parametrosRegla.ContainsKey("guia"))
            {
                datosGuia = (ADGuia)(parametrosRegla["guia"]);
                centroServicio = fachadaCentroServicio.ObtenerTipoYResponsableCentroServicio(datosGuia.IdCentroServicioDestino);



                if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.AGE.ToString())
                {
                    responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Agencias.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                }

                else if (centroServicio.Tipo == PUEnumTipoCentroServicioDC.PTO.ToString())
                {
                    responsable = RAMapeoResponsable.MapeoResponsable((int)RAEnumOrigenRaps.Puntos.GetHashCode(), centroServicio.IdCentroServicio, centroServicio.infoResponsable.IdResponsable.ToString(), centroServicio.Nombre);
                }
            }

            return responsable;
        }
    }
}
