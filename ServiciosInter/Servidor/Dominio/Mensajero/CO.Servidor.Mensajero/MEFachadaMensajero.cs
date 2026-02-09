using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Dominio.Comun.Mensajero;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;

namespace CO.Servidor.Mensajero
{
    internal class MEFachadaMensajero : IMEFachadaMensajero
    {
        private static readonly MEFachadaMensajero instancia = new MEFachadaMensajero();

        #region  Propiedades
        public static MEFachadaMensajero Instancia
        {
            get { return MEFachadaMensajero.instancia; }
        }
        #endregion


        public MEMensajero ConsultarMensajero(int idDocumento)
        {
            return MeAdministradorMensajero.Instancia.ConsultarMensajero(idDocumento);
        }

    }
}
