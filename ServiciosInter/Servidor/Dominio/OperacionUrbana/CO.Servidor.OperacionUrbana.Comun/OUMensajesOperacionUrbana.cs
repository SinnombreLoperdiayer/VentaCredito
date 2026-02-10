using System;

namespace CO.Servidor.OperacionUrbana.Comun
{
    public class OUMensajesOperacionUrbana
    {
        /// <summary>
        /// Carga un mensaje de error de tarifas desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(OUEnumTipoErrorOU tipoErrorOU)
        {
            string mensajeError;

            switch (tipoErrorOU)
            {
                case OUEnumTipoErrorOU.IN_ACTIVO:
                    mensajeError = OUMensajes.IN_001;
                    break;

                case OUEnumTipoErrorOU.IN_INACTIVO:
                    mensajeError = OUMensajes.IN_002;
                    break;

                case OUEnumTipoErrorOU.IN_SUSPENDIDO:
                    mensajeError = OUMensajes.IN_003;
                    break;

                case OUEnumTipoErrorOU.EX_NO_CUMPLE_TIPO_VINCULACION:
                    mensajeError = OUMensajes.EX_001;
                    break;

                case OUEnumTipoErrorOU.EX_NO_EXISTE_PERSONA:
                    mensajeError = OUMensajes.EX_002;
                    break;

                case OUEnumTipoErrorOU.EX_MENSAJERO_NO_EXISTE:
                    mensajeError = OUMensajes.EX_003;
                    break;

                case OUEnumTipoErrorOU.EX_CONTRATO_NO_ESTA_VIGENTE:
                    mensajeError = OUMensajes.EX_004;
                    break;

                case OUEnumTipoErrorOU.EX_FALTA_ESTADO_EMPAQUE:
                    mensajeError = OUMensajes.EX_005;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA:
                    mensajeError = OUMensajes.EX_006;
                    break;

                case OUEnumTipoErrorOU.EX_DIFERENCIAS_PESO:
                    mensajeError = OUMensajes.EX_007;
                    break;

                case OUEnumTipoErrorOU.EX_CLIENTE_CON_PRESUPUESO_VENCIDO:
                    mensajeError = OUMensajes.EX_008;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_NO_PROVISIONADA:
                    mensajeError = OUMensajes.EX_009;
                    break;

                case OUEnumTipoErrorOU.EX_SIN_BOLSA_DE_SEGURIDAD:
                    mensajeError = OUMensajes.EX_010;
                    break;

                case OUEnumTipoErrorOU.EX_GUIAS_SIN_PLANILLAR:
                    mensajeError = OUMensajes.EX_011;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_NO_ESTA_REGISTRADA:
                    mensajeError = OUMensajes.EX_012;
                    break;

                case OUEnumTipoErrorOU.EX_CIUDAD_ASIGNACION_DIFERENTE_CIUDAD_DESTINO_GUIA:
                    mensajeError = OUMensajes.EX_013;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_ESTA_EN_PLANILLA_ASIGNACION:
                    mensajeError = OUMensajes.EX_014;
                    break;

                case OUEnumTipoErrorOU.EX_EL_ENVIO_SE_ENCUENTRA_EN_UN_ESTADO_NO_VALIDO:
                    mensajeError = OUMensajes.EX_015;
                    break;

                case OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO:
                    mensajeError = OUMensajes.EX_016;
                    break;

                case OUEnumTipoErrorOU.EX_ENVIOS_SIN_VERIFICAR:
                    mensajeError = OUMensajes.EX_017;
                    break;

                case OUEnumTipoErrorOU.EX_ERROR_USUARIO_VERIFICA:
                    mensajeError = OUMensajes.EX_018;
                    break;

                case OUEnumTipoErrorOU.EX_NO_SE_PUEDE_ABRIR_LA_PLANILLA_ASIGNACION:
                    mensajeError = OUMensajes.EX_019;
                    break;

                case OUEnumTipoErrorOU.EX_LA_GUIA_ESTA_VERIFICADA:
                    mensajeError = OUMensajes.EX_020;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_INGRESADA_PLANILLA_AGENCIA:
                    mensajeError = OUMensajes.EX_021;
                    break;

                case OUEnumTipoErrorOU.EX_NO_SE_PUEDE_INGRESAR_GUIA_POR_ESTADOS:
                    mensajeError = OUMensajes.EX_022;
                    break;

                case OUEnumTipoErrorOU.EX_ERROR_RECOGIDA:
                    mensajeError = OUMensajes.EX_023;
                    break;

                case OUEnumTipoErrorOU.EX_ERROR_CONSECUTIVO_PLANILLA_RECOGIDA:
                    mensajeError = OUMensajes.EX_024;
                    break;

                case OUEnumTipoErrorOU.EX_NO_EXISTE_PLANILLA:
                    mensajeError = OUMensajes.EX_025;
                    break;

                case OUEnumTipoErrorOU.EX_RECOGIDA_DESCARGADA:
                    mensajeError = OUMensajes.EX_026;
                    break;

                case OUEnumTipoErrorOU.EX_CAMBIO_ESTADO_NO_VALIDO:
                    mensajeError = OUMensajes.EX_028;
                    break;

                case OUEnumTipoErrorOU.EX_DESHACER_DESCARGUE_INVALIDO:
                    mensajeError = OUMensajes.EX_029;
                    break;

                case OUEnumTipoErrorOU.EX_DESTINO_ENVIO_NO_SE_APOYA_COL_ASIGNACION:
                    mensajeError = OUMensajes.EX_030;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_NO_SUPERVISADA:
                    mensajeError = OUMensajes.EX_031;
                    break;

                case OUEnumTipoErrorOU.EX_INGRESE_ENVIO_EN_SECCION_CARGA:
                    mensajeError = OUMensajes.EX_032;
                    break;

                case OUEnumTipoErrorOU.EX_INGRESE_ENVIO_EN_SECCION_MENSAJERIA:
                    mensajeError = OUMensajes.EX_033;
                    break;

                case OUEnumTipoErrorOU.EX_PLANILLA_CERRADA_O_NO_EXISTE:
                    mensajeError = OUMensajes.EX_034;
                    break;

                case OUEnumTipoErrorOU.EX_ENVIO_ALCOBRO_NO_PAGO:
                    mensajeError = OUMensajes.EX_035;
                    break;
                case OUEnumTipoErrorOU.EX_LA_GUIA_YA_ESTA_DESCARGADA:
                    mensajeError = OUMensajes.EX_037;
                    break;
                case OUEnumTipoErrorOU.EX_PARAMETRO_NO_EXISTE:
                    mensajeError = OUMensajes.EX_038;
                    break;
                case OUEnumTipoErrorOU.EX_ERROR_ASIGNACION:
                    mensajeError = OUMensajes.EX_039;
                    break;
                case OUEnumTipoErrorOU.EX_GUIA_NO_PERTENECE_PUNTO:
                    mensajeError = OUMensajes.EX_040;
                    break;
                case OUEnumTipoErrorOU.EX_GUIA_YA_PLANILLADA_VENTAS:
                    mensajeError = OUMensajes.EX_041;
                    break;

                case OUEnumTipoErrorOU.EX_PIEZA_ROTULO_YA_PLANILLADA:
                    mensajeError = OUMensajes.EX_042;
                    break;

                case OUEnumTipoErrorOU.EX_ERROR_ESTADO_CONSOLIDADO:
                    mensajeError = OUMensajes.EX_042;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_YA_INGRESADA_CENTRO_ACOPIO:
                    mensajeError = OUMensajes.EX_044;
                    break;

                case OUEnumTipoErrorOU.EX_PLANILLA_YA_ASIGNADA:
                    mensajeError = OUMensajes.EX_045;
                    break;

                case OUEnumTipoErrorOU.EX_SOBRANTE_YA_AUDITADO:
                    mensajeError = OUMensajes.EX_046;
                    break;


                case OUEnumTipoErrorOU.EX_GUIA_RECLAME_OFICINA:
                    mensajeError = OUMensajes.EX_047;
                    break;

                case OUEnumTipoErrorOU.EX_ENVIO_NO_SE_ENCUENTRA_CENTRO_ACOPIO_ASIG_MENSAJERO:
                    mensajeError = OUMensajes.EX_048;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_ESTA_PLANILLADA_ASIGNACION_MESAJERO:
                    mensajeError = OUMensajes.EX_049;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_EN_AUDITORIA_ASIGNACION_MENSAJERO:
                    mensajeError = OUMensajes.EX_050;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_ASIGNADA_A_OTRO_INVENTARIO:
                    mensajeError = OUMensajes.EX_051;
                    break;

                case OUEnumTipoErrorOU.EX_GUIA_NO_SE_ENCUENTRA_EN_AUDITORIA:
                    mensajeError = OUMensajes.EX_052;
                    break;

                default:
                    mensajeError = String.Format(OUMensajes.EX_000, tipoErrorOU.ToString());
                    break;
            }

            return mensajeError;
        }
    }
}