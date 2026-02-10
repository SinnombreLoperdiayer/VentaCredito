using CO.Servidor.Mensajero.Datos;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CO.Servidor.Mensajero
{
    public class MeAdministradorMensajero
    {
        private static readonly MeAdministradorMensajero instancia = new MeAdministradorMensajero();

        public static MeAdministradorMensajero Instancia
        {
            get { return MeAdministradorMensajero.instancia; }
        }

        private MeAdministradorMensajero(){ }

        public MEMensajero ConsultarMensajero(int idDocumento)
        {
            return MeRepositorio.Instancia.ConsultarMensajero(idDocumento);
        }

        public bool CrearMensajero(MEMensajero mensajero)
        {
            int idPersonaInterna = 0;
            int idMensajero = 0;
            bool res=true;

            using (TransactionScope transaccion = new TransactionScope())
            {

                idPersonaInterna = MeRepositorio.Instancia.InsertarPersonaInterna(mensajero);

                idMensajero= MeRepositorio.Instancia.CrearMensajero(mensajero, idPersonaInterna);

                if (mensajero.TipoMensajero == "Auxiliar")
                {
                    res = MeRepositorio.Instancia.InsertarMensajeroVehiculo(mensajero.Vehiculo, idMensajero);
                }
                transaccion.Complete();
                return res;
            }

           

        }

        public List<MEMensajero> ObtenerPersonal(string idTipoUsuario)
        {
            return MeRepositorio.Instancia.ObtenerPersonal(idTipoUsuario);
        }

        public MEMensajero ObtenerDetalleEmpleadoNovasoft(string idDocumento, string compania)
        {
            return MeRepositorio.Instancia.ObtenerDetalleEmpleadoNovasoft(idDocumento,compania);
        }

        public bool ObtenerPersonaInternaXId(string idDocumento)
        {
            return MeRepositorio.Instancia.ObtenerPersonaInternaXId(idDocumento);
        }


        public MEMensajero ObtenerEmpleadoNovasoft(string idDocumento)
        {
            return MeRepositorio.Instancia.ObtenerEmpleadoNovasoft(idDocumento);
        }
    }
}
