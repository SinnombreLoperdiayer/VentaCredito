using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ServicioalCliente.Comun
{
    public class SCMensajesCliente
    {
        public static string CargarMensaje(SCEnumTipoError tipoErrorSC)
        {
            string mensajeError;
            switch (tipoErrorSC)
            {
                case SCEnumTipoError.EX_ERROR_GUIA_NO_RECLAME_EN_OFICINA:
                    mensajeError = SCMensajes.EX_000;
                    break;

                default:
                    mensajeError = String.Format(SCMensajes.EX_000, tipoErrorSC.ToString());
                    break;
            }
            return mensajeError;
        }
    }
}
