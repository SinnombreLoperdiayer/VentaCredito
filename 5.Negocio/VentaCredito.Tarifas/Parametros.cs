using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Excepciones;
using VentaCredito.Transversal;

namespace VentaCredito.Tarifas
{
    public class Parametros
    {

        private static Parametros instancia = new Parametros();

        public static Parametros Instancia
        {
            get
            {
                return instancia;
            }
        }

        public IEnumerable<TAServicioDC> ObtenerServicios()
        {
            return Datos.Repositorio.Parametros.Instancia.ObtenerServicios();
        }

        public string ObtenerNombreTipoEnvio(int idTipoEnvio, int idServicio)
        {
            string NombreEnvio = "";
               var servicioTipoEnvio =  Datos.Repositorio.Parametros.Instancia.ObtenerServicioTipoEnvio(idTipoEnvio, idServicio);

            if (servicioTipoEnvio == null) {
                var msg = string.Format("VALIDAR INFORMACION DE ADMISION /n/r" + Properties.Resources.MensajeSinTipoEnvioxServicio, idServicio, idTipoEnvio);
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Info,
                    msg
                    , "VCR", new Exception(msg,null), ContextoSitio.Current.Usuario);
                
                    }
            else {
                NombreEnvio = servicioTipoEnvio.FirstOrDefault().Nombre;
            }
            return NombreEnvio;
        }
    }
}
