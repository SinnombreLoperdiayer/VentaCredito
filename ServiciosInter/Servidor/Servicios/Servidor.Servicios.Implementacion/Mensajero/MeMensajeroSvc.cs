using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.Contratos;
using CO.Servidor.Mensajero;
namespace CO.Servidor.Servicios.Implementacion.Mensajero
{
    public class MeMensajeroSvc : IMeMensajeroSvc
    {
        public MeMensajeroSvc()
        { }

        public MEMensajero ConsultarMensajero(int idDocumento)
        {

            return MeAdministradorMensajero.Instancia.ConsultarMensajero(idDocumento);

        }

        public bool CrearMensajero(MEMensajero mensajero)
        {
            return MeAdministradorMensajero.Instancia.CrearMensajero(mensajero);
        }
        public List<MEMensajero> ObtenerPersonal(string idTipoUsuario)
        {
            return MeAdministradorMensajero.Instancia.ObtenerPersonal(idTipoUsuario);
        }

        public MEMensajero ObtenerDetalleEmpleadoNovasoft(string idDocumento, string compania)
        {
            return MeAdministradorMensajero.Instancia.ObtenerDetalleEmpleadoNovasoft(idDocumento, compania);
        }

        public bool ObtenerPersonaInternaXId(string idDocumento)
        {
            return MeAdministradorMensajero.Instancia.ObtenerPersonaInternaXId(idDocumento);
        }


        public MEMensajero ObtenerEmpleadoNovasoft(string idDocumento)
        {
            return MeAdministradorMensajero.Instancia.ObtenerEmpleadoNovasoft(idDocumento);
        }

    }
}
