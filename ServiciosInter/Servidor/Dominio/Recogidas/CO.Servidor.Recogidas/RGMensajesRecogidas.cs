using System;
using System.Collections.Generic;
using CO.Servidor.Recogidas.Comun;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;

namespace CO.Servidor.Recogidas
{
    public class RGMensajesRecogidas
    {


        private Dictionary<RGEnumTipoErrorRecogidas, string> TipoErrorMensaje;
        private static RGMensajesRecogidas instance;

        public static RGMensajesRecogidas Instance {
            get {
                if (instance == null)
                {
                    instance = new RGMensajesRecogidas();
                }
                return instance;
            }
        }

        public RGMensajesRecogidas()
        {
            CargaTipoErrorMensaje();
        }
        /// <summary>
        /// Carga los tipos de mensaje en el dicionario
        /// </summary>
        private void CargaTipoErrorMensaje()
        {
            this.TipoErrorMensaje = new Dictionary<RGEnumTipoErrorRecogidas, string>();
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_ERROR_NO_IDENTIFICADO, RGMensajes.EX_ERROR_NO_IDENTIFICADO);
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_NUMERO_SOL_RECOGIDA_NO_EXISTE, RGMensajes.EX_NUMERO_SOL_RECOGIDA_NO_EXISTE);
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_PARAMETRIZACION_INVALIDA, RGMensajes.EX_PARAMETRIZACION_INVALIDA);
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_RECOGIDA_EJECUTADA, RGMensajes.EX_RECOGIDA_EJECUTADA);
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_RECOGIDA_NO_ES_ESPORADICA, RGMensajes.EX_RECOGIDA_NO_ES_ESPORADICA);
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_RECOGIDA_NO_EXISTE, RGMensajes.EX_RECOGIDA_NO_EXISTE);
            TipoErrorMensaje.Add(RGEnumTipoErrorRecogidas.EX_SOLICITUD_RECOGIDA_NO_ESPECIFICADO, RGMensajes.EX_SOLICITUD_RECOGIDA_NO_ESPECIFICADO);
        }

        /// <summary>
        /// Carga un mensaje de error de solicitudes de recogida desde el recurso del lenguaje
        /// </summary>
        /// <param name="eX_SOLICITUD_RECOGIDA_NO_ESPECIFICADO"></param>
        /// <returns></returns>
        public string CargarMensaje(RGEnumTipoErrorRecogidas tipoErrorRecogidas)
        {
            return IdentificaTipoErroMensaje(tipoErrorRecogidas);
        }

        private string IdentificaTipoErroMensaje(RGEnumTipoErrorRecogidas tipoErrorRecogidas)
        {
            if (TipoErrorMensaje.ContainsKey(tipoErrorRecogidas))
            {
                return Convert.ToString(TipoErrorMensaje[tipoErrorRecogidas]);
            }

            return Convert.ToString(TipoErrorMensaje[RGEnumTipoErrorRecogidas.EX_ERROR_NO_IDENTIFICADO]);
        }
    }
}