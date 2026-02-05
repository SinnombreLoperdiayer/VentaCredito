using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.CentroServicios.Comun
{
    /// <summary>
    /// Clase para manejar los mensajes de Centro de servicios
    /// </summary>
    public class MensajesCentroServicios
    {
        private static Dictionary<EnumTipoErrorCentroServicios, string> DiccionarioMensajes
            = new Dictionary<EnumTipoErrorCentroServicios, string>
      {
        #region Mensajes de Excepción
            {EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_ACTIVO, Mensajes.EX_001},
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_VENDER_GIROS, Mensajes.EX_002 },
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS, Mensajes.EX_003 },
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_EXISTE, Mensajes.EX_004 },
            { EnumTipoErrorCentroServicios.EX_ALERTA_LISTAS_RESTRICTIVAS, Mensajes.EX_005 },
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_PAGAR_GIROS, Mensajes.EX_006 },
            { EnumTipoErrorCentroServicios.EX_NO_AGENCIA_EN_MUNICIPIO, Mensajes.EX_007 },
            { EnumTipoErrorCentroServicios.EX_NO_RACOL_EN_CIUDAD, Mensajes.EX_008 },
            { EnumTipoErrorCentroServicios.EX_EXISTE_COL_EN_CIUDAD, Mensajes.EX_009 },
            { EnumTipoErrorCentroServicios.EX_EXISTE_AGENCIA_EN_CIUDAD, Mensajes.EX_010 },
            { EnumTipoErrorCentroServicios.EX_EXISTE_RACOL_EN_CIUDAD, Mensajes.EX_011 },
            { EnumTipoErrorCentroServicios.EX_AGENCIA_SUPERO_MAX_VENTA_GIROS, Mensajes.EX_012 },
            { EnumTipoErrorCentroServicios.EX_EXISTE_PERSONA_CONTRATO, Mensajes.EX_013 },
            { EnumTipoErrorCentroServicios.EX_FALLO_ADJUNTAR_ARCHIVO, Mensajes.EX_014 },
            { EnumTipoErrorCentroServicios.EX_FALLO_ELIMINAR_ARCHIVO, Mensajes.EX_015 },
            { EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS, Mensajes.EX_021 },
            { EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO, Mensajes.EX_022 },
            { EnumTipoErrorCentroServicios.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA, Mensajes.EX_023 },
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_EXISTE, Mensajes.EX_026 },
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE, Mensajes.EX_027 },
            { EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS, Mensajes.EX_028 },
            { EnumTipoErrorCentroServicios.EX_AGENCIA_NO_EXISTE_INACTIVA, Mensajes.EX_028 },
        #endregion Mensajes de Excepción
        #region Mensajes de Informacion
            { EnumTipoErrorCentroServicios.IN_ESTADO_LIQUIDACION, Mensajes.IN_001 },
            { EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_RACOL, Mensajes.IN_002 },
            { EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_COL, Mensajes.IN_003 },
            { EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_AGENCIA, Mensajes.IN_004 },
            { EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_PUNTO, Mensajes.IN_005 },
            { EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_AGENTE_COMERCIAL, Mensajes.IN_006 },
            { EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_CENTRO_SERVICIOS, Mensajes.IN_007 },
        #endregion Mensajes de Informacion
      };

        /// <summary>
        /// Carga un mensaje de error de Centro de servicios desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(EnumTipoErrorCentroServicios tipoErrorCentroServicios)
        {
            string mensajeError;

            if (DiccionarioMensajes.ContainsKey(tipoErrorCentroServicios))
            {
                DiccionarioMensajes.TryGetValue(tipoErrorCentroServicios, out mensajeError);
            }
            else
            {
                mensajeError = String.Format(Mensajes.EX_000, tipoErrorCentroServicios.ToString());
            }

            return mensajeError;
        }

        /// <summary>
        /// Carga un mensaje de error de Centro de servicios desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensajeOld(EnumTipoErrorCentroServicios tipoErrorCentroServicios)
        {
            string mensajeError;

            switch (tipoErrorCentroServicios)
            {
                #region Mensajes de Excepción

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_ACTIVO:
                    mensajeError = Mensajes.EX_001;
                    break;

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_VENDER_GIROS:
                    mensajeError = Mensajes.EX_002;
                    break;

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_TIENE_SERVICIO_GIROS:
                    mensajeError = Mensajes.EX_003;
                    break;

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_EXISTE:
                    mensajeError = Mensajes.EX_004;
                    break;
                case EnumTipoErrorCentroServicios.EX_ALERTA_LISTAS_RESTRICTIVAS:
                    mensajeError = Mensajes.EX_005;
                    break;
                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIOS_NO_PUEDE_PAGAR_GIROS:
                    mensajeError = Mensajes.EX_006;
                    break;
                case EnumTipoErrorCentroServicios.EX_NO_AGENCIA_EN_MUNICIPIO:
                    mensajeError = Mensajes.EX_007;
                    break;
                case EnumTipoErrorCentroServicios.EX_NO_RACOL_EN_CIUDAD:
                    mensajeError = Mensajes.EX_008;
                    break;
                case EnumTipoErrorCentroServicios.EX_EXISTE_COL_EN_CIUDAD:
                    mensajeError = Mensajes.EX_009;
                    break;
                case EnumTipoErrorCentroServicios.EX_EXISTE_AGENCIA_EN_CIUDAD:
                    mensajeError = Mensajes.EX_010;
                    break;
                case EnumTipoErrorCentroServicios.EX_EXISTE_RACOL_EN_CIUDAD:
                    mensajeError = Mensajes.EX_011;
                    break;
                case EnumTipoErrorCentroServicios.EX_AGENCIA_SUPERO_MAX_VENTA_GIROS:
                    mensajeError = Mensajes.EX_012;
                    break;
                case EnumTipoErrorCentroServicios.EX_EXISTE_PERSONA_CONTRATO:
                    mensajeError = Mensajes.EX_013;
                    break;

                case EnumTipoErrorCentroServicios.EX_FALLO_ADJUNTAR_ARCHIVO:
                    mensajeError = Mensajes.EX_014;
                    break;

                case EnumTipoErrorCentroServicios.EX_FALLO_ELIMINAR_ARCHIVO:
                    mensajeError = Mensajes.EX_015;
                    break;

                case EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS:
                    mensajeError = Mensajes.EX_021;
                    break;

                case EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO:
                    mensajeError = Mensajes.EX_022;
                    break;

                case EnumTipoErrorCentroServicios.EX_PLANTILLA_DIVULGACION_NO_CONFIGURADA:
                    mensajeError = Mensajes.EX_023;
                    break;

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_EXISTE:
                    mensajeError = Mensajes.EX_026;
                    break;

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE:
                    mensajeError = Mensajes.EX_027;
                    break;

                case EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS:
                    mensajeError = Mensajes.EX_028;
                    break;

                case EnumTipoErrorCentroServicios.EX_AGENCIA_NO_EXISTE_INACTIVA:
                    mensajeError = Mensajes.EX_028;
                    break;

                #endregion Mensajes de Excepción

                #region Mensajes de Informacion

                case EnumTipoErrorCentroServicios.IN_ESTADO_LIQUIDACION:
                    mensajeError = Mensajes.IN_001;
                    break;
                case EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_RACOL:
                    mensajeError = Mensajes.IN_002;
                    break;
                case EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_COL:
                    mensajeError = Mensajes.IN_003;
                    break;
                case EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_AGENCIA:
                    mensajeError = Mensajes.IN_004;
                    break;
                case EnumTipoErrorCentroServicios.IN_TIPO_CENTRO_SERVICIO_PUNTO:
                    mensajeError = Mensajes.IN_005;
                    break;
                case EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_AGENTE_COMERCIAL:
                    mensajeError = Mensajes.IN_006;
                    break;
                case EnumTipoErrorCentroServicios.IN_TIPO_DOCUMENTO_CENTRO_SERVICIOS:
                    mensajeError = Mensajes.IN_007;
                    break;

                #endregion Mensajes de Informacion

                default:
                    mensajeError = String.Format(Mensajes.EX_000, tipoErrorCentroServicios.ToString());
                    break;
            }

            return mensajeError;
        }

    }
}
