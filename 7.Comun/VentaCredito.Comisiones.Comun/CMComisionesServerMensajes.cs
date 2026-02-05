using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Comisiones.Comun
{
    public class CMComisionesServerMensajes
    {
        public static string CargarMensaje(CMEnumTipoErrorComisiones tipoErrorComisiones)
        {
            string mensajeError = string.Empty;
            switch (tipoErrorComisiones)
            {
                case CMEnumTipoErrorComisiones.EX_ERROR_PORCENTAJE_COMISION_CENTROSERVICIO:
                    mensajeError = CMComisionesServidorMensajes.EX_001;
                    break;

                case CMEnumTipoErrorComisiones.EX_ERROR_PORCENTAJE_COMISION_RESPONSABLE:
                    mensajeError = CMComisionesServidorMensajes.EX_002;
                    break;

                case CMEnumTipoErrorComisiones.EX_ERROR_NO_TIENE_COMISION_ASIGNADA:
                    mensajeError = CMComisionesServidorMensajes.EX_003;
                    break;

                case CMEnumTipoErrorComisiones.EX_ERROR_VALOR_BASE_COMISION_ES_CERO:
                    mensajeError = CMComisionesServidorMensajes.EX_004;
                    break;

                case CMEnumTipoErrorComisiones.EX_ERROR_VALOR_COMISION_RESPONSABLE:
                    mensajeError = CMComisionesServidorMensajes.EX_005;
                    break;

                case CMEnumTipoErrorComisiones.EX_COMISION_NO_CONFIGURADA:
                    mensajeError = CMComisionesServidorMensajes.EX_006;
                    break;

                default:
                    mensajeError = String.Format(CMComisionesServidorMensajes.EX_000, tipoErrorComisiones.ToString());
                    break;
            }
            return mensajeError;
        }
    }
}
