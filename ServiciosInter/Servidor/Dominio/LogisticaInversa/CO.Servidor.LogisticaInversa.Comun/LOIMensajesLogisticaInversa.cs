using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
    public class LOIMensajesLogisticaInversa
    {
        /// <summary>
        /// Carga un mensaje de error de logistica inversa desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(LOIEnumTipoErrorLogisticaInversa tipoErrorLogisticaInversa)
        {
            string mensajeError;

            switch (tipoErrorLogisticaInversa)
            {
                case LOIEnumTipoErrorLogisticaInversa.EX_NO_EXISTEN_MANIFIESTOS:
                    mensajeError = LOIMensajes.EX_001;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_SUMINISTRO_SIN_PROVISION:
                    mensajeError = LOIMensajes.EX_002;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_CAMBIO_ESTADO_NO_VALIDO:
                    mensajeError = LOIMensajes.EX_003;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_PRUEBA_NO_VALIDA:
                    mensajeError = LOIMensajes.EX_004;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_DESCARGADA:
                    mensajeError = LOIMensajes.EX_005;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_MAL_DILIGENCIADA:
                    mensajeError = LOIMensajes.EX_006;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_USUARIO_NO_COL:
                    mensajeError = LOIMensajes.EX_007;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_DIGITALIZADA:
                    mensajeError = LOIMensajes.EX_008;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_EXISTE:
                    mensajeError = LOIMensajes.EX_009;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_CAJA_ACTIVA_COL_NO_EXISTE:
                    mensajeError = LOIMensajes.EX_010;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_YA_FUE_ARCHIVADA:
                    mensajeError = LOIMensajes.EX_011;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_MANIFIESTO_SIN_GUIAS:
                    mensajeError = LOIMensajes.EX_012;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CAMBIO_ESTADO:
                    mensajeError = LOIMensajes.EX_013;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DEV:
                    mensajeError = LOIMensajes.EX_014;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CLIENTE_GUIA:
                    mensajeError = LOIMensajes.EX_015;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ALCOBRO_NOPAGO:
                    mensajeError = LOIMensajes.EX_016;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_ESTADO_DIF_CENTROACOPIO:
                    mensajeError = LOIMensajes.EX_017;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_SERVICIO_NO_CONFIGURADO_REEXPEDICION:
                    mensajeError = LOIMensajes.EX_018;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ARCHIVO_NO_ENCONTRADO:
                    mensajeError = LOIMensajes.EX_019;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_INTERNA_MANIFIESTO:
                    mensajeError = LOIMensajes.EX_020;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_PLANILLA_CERTIFICACION:
                    mensajeError = LOIMensajes.EX_021;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_FALTA_DIGITALIZAR_PRUEBA_ENTREGA:
                    mensajeError = LOIMensajes.EX_022;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_FALTA_CAPTURAR_DATOS_ENVIO:
                    mensajeError = LOIMensajes.EX_023;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_PLANILLADA:
                    mensajeError = LOIMensajes.EX_024;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_PLANILLA_NO_EXISE:
                    mensajeError = LOIMensajes.EX_025;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_OBJETO_GESTION_NULL:
                    mensajeError = LOIMensajes.EX_026;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.IN_CONTENIDO_GUIA_INTERNA_CERTIFICACION:
                    mensajeError = LOIMensajes.IN_001;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.IN_CONTENIDO_GUIA_INTERNA_PLANILLA:
                    mensajeError = LOIMensajes.IN_002;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DATOS_RECIBIDO_YA_CAPTURADOS:
                    mensajeError = LOIMensajes.EX_027;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_YA_DEVUELTA:
                    mensajeError = LOIMensajes.EX_028;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_NO_CREDITO:
                    mensajeError = LOIMensajes.EX_029;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_DESTINO:
                    mensajeError = LOIMensajes.EX_030;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NOTIFICACION:
                    mensajeError = LOIMensajes.EX_031;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_SUCURSAL_ACTIVA:
                    mensajeError = LOIMensajes.EX_032;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_DIRECCION_NO_EXISTE:
                    mensajeError = LOIMensajes.EX_033;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CENTROACOPIO_CESDIFERENTE:
                    mensajeError = LOIMensajes.EX_034;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NO_ESTADO_PENDIENTE_INGRESO_CUSTODIA:
                    mensajeError = LOIMensajes.EX_035;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_NO_INGRESADO_CUSTODIA:
                    mensajeError = LOIMensajes.EX_036;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_PESO_GUIA:
                    mensajeError = LOIMensajes.EX_037;
                    break;

                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_CLIENTE_GUIA_DEVOLUCION:
                      mensajeError = LOIMensajes.EX_038;
                    break;
                case LOIEnumTipoErrorLogisticaInversa.EX_GUIA_CREDITO:
                      mensajeError = LOIMensajes.EX_039;
                    break;
                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_FTP:
                    mensajeError = LOIMensajes.EX_040;
                    break;
                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_SIN_RECIBIDO_CAPTURADO:
                    mensajeError = LOIMensajes.EX_041;
                    break;
                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_NO_ES_CONTADO:
                    mensajeError = LOIMensajes.EX_042;
                    break;
                case LOIEnumTipoErrorLogisticaInversa.EX_ERROR_GUIA_NO_ES_NOTIFICACION:
                    mensajeError = LOIMensajes.EX_043;
                    break;
                default:
                    mensajeError = String.Format(LOIMensajes.EX_000, tipoErrorLogisticaInversa.ToString());
                    break;
            }
            return mensajeError;
        }
    }
}