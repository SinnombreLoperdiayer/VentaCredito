using System;

namespace Framework.Servidor.Excepciones
{
    /// <summary>
    /// Clase para manejar los mensajes del framework
    /// </summary>
    public class MensajesFramework
    {
        /// <summary>
        /// Carga un mensaje de error del framework desde el recurso del lenguaje
        /// </summary>
        /// <param name="tipoErrorFramework">Enumeración con el tipo del error</param>
        /// <returns>Mensaje de error</returns>
        public static string CargarMensaje(ETipoErrorFramework tipoErrorFramework)
        {
            string mensajeError;

            switch (tipoErrorFramework)
            {
                #region Mensajes de Excepción

                case ETipoErrorFramework.EX_ID_NO_IDENTIFICADO:
                    mensajeError = Mensajes.EX_001;
                    break;

                case ETipoErrorFramework.EX_USUARIO_BLOQUEADO_INTENTOS_AUTENTICA:
                    mensajeError = Mensajes.EX_002;
                    break;

                case ETipoErrorFramework.EX_PASSWORD_DIFERENTE:
                    mensajeError = Mensajes.EX_003;
                    break;

                case ETipoErrorFramework.EX_USUARIO_CLAVE_NO_VALIDO:
                    mensajeError = Mensajes.EX_004;
                    break;

                case ETipoErrorFramework.EX_USUARIO_BLOQUEADO:
                    mensajeError = Mensajes.EX_005;
                    break;

                case ETipoErrorFramework.EX_DEBECAMBIAR_CLAVE:
                    mensajeError = Mensajes.EX_006;
                    break;

                case ETipoErrorFramework.EX_CLAVE_INVALIDA:
                    mensajeError = Mensajes.EX_007;
                    break;

                case ETipoErrorFramework.EX_TIEMPO_VIDA_NO_CONFIGURADO:
                    mensajeError = Mensajes.EX_008;
                    break;

                case ETipoErrorFramework.EX_IDMAQUINA_NOVALIDO:
                    mensajeError = Mensajes.EX_009;
                    break;

                case ETipoErrorFramework.EX_CAMPO_NO_EXISTE:
                    mensajeError = Mensajes.EX_010;
                    break;

                case ETipoErrorFramework.EX_ERROR_DESCONOCIDO:
                    mensajeError = Mensajes.EX_011;
                    break;

                case ETipoErrorFramework.EX_ERROR_FORMATO_INFOR:
                    mensajeError = Mensajes.EX_012;
                    break;

                case ETipoErrorFramework.EX_ERROR_DIVISION_CERO:
                    mensajeError = Mensajes.EX_013;
                    break;

                case ETipoErrorFramework.EX_ERROR_OPERACION_ARITMETICA:
                    mensajeError = Mensajes.EX_014;
                    break;

                case ETipoErrorFramework.EX_ERROR_CONVERSION_DATOS:
                    mensajeError = Mensajes.EX_015;
                    break;

                case ETipoErrorFramework.EX_ERROR_OPERACION_INVALIDA:
                    mensajeError = Mensajes.EX_016;
                    break;

                case ETipoErrorFramework.EX_ERROR_DIRECTORIO_NO_ENCONTRADO:
                    mensajeError = Mensajes.EX_017;
                    break;

                case ETipoErrorFramework.EX_ERROR_CARGANDO_ARCHIVO:
                    mensajeError = Mensajes.EX_018;
                    break;

                case ETipoErrorFramework.EX_ERROR_ARCHIVO_NO_ENCONTRADO:
                    mensajeError = Mensajes.EX_019;
                    break;

                case ETipoErrorFramework.EX_ERROR_MEMORIA_EXCEDIDA:
                    mensajeError = Mensajes.EX_020;
                    break;

                case ETipoErrorFramework.EX_ERROR_PROCESO_ABORTADO:
                    mensajeError = Mensajes.EX_021;
                    break;

                case ETipoErrorFramework.EX_ERROR_EN_INTERCEPTOR:
                    mensajeError = Mensajes.EX_022;
                    break;

                case ETipoErrorFramework.EX_ERROR_FALTA_CREDENCIALES_WCF:
                    mensajeError = Mensajes.EX_023;
                    break;

                case ETipoErrorFramework.EX_ERROR_USER_PASS_WCF_INCORRECTOS:
                    mensajeError = Mensajes.EX_024;
                    break;

                case ETipoErrorFramework.EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO:
                    mensajeError = Mensajes.EX_025;
                    break;

                case ETipoErrorFramework.EX_ERROR_TAREA_MAL_CONFIGURADA:
                    mensajeError = Mensajes.EX_026;
                    break;

                case ETipoErrorFramework.EX_PARAMETROS_ARCHIVOS_ADJUNTOS_NO_CONFIGURADO:
                    mensajeError = Mensajes.EX_027;
                    break;

                case ETipoErrorFramework.EX_VERSION_NO_PARAMETRIZADA:
                    mensajeError = Mensajes.EX_028;
                    break;

                case ETipoErrorFramework.EX_FALLA_MAL_CONFIGURADA:
                    mensajeError = Mensajes.EX_029;
                    break;

                case ETipoErrorFramework.EX_CONSULTA_DB_NULL:
                    mensajeError = Mensajes.EX_035;
                    break;

                case ETipoErrorFramework.EX_USUARIO_LDAP_NULO:
                    mensajeError = Mensajes.EX_031;
                    break;

                case ETipoErrorFramework.EX_REGISTRO_YA_EXISTE:
                    mensajeError = Mensajes.EX_037;
                    break;

                case ETipoErrorFramework.EX_TAREA_ESCALADA:
                    mensajeError = Mensajes.EX_038;
                    break;

                case ETipoErrorFramework.EX_NO_USUARIOS_ACTIVOS_ESCALAMIENTO:
                    mensajeError = Mensajes.EX_039;
                    break;

                case ETipoErrorFramework.EX_NO_CARGOS_SUPERIORES:
                    mensajeError = Mensajes.EX_040;
                    break;

                case ETipoErrorFramework.EX_USUARIO_ASIGNADO_NO_EXISTE:
                    mensajeError = Mensajes.EX_041;
                    break;

                case ETipoErrorFramework.EX_CARGO_NO_EXISTE:
                    mensajeError = Mensajes.EX_042;
                    break;

                case ETipoErrorFramework.EX_ERROR_DESCONOCIDO_LDAP:
                    mensajeError = Mensajes.EX_043;
                    break;

                case ETipoErrorFramework.EX_ERROR_BORRANDO_FK:
                    mensajeError = Mensajes.EX_044;
                    break;

                case ETipoErrorFramework.EX_USUARIO_NO_ACTIVO_O_NO_VALIDO_ATENDER_TAREA:
                    mensajeError = Mensajes.EX_045;
                    break;

                case ETipoErrorFramework.EX_ERROR_CAMBIANDO_PASSWORD:
                    mensajeError = Mensajes.EX_046;
                    break;

                case ETipoErrorFramework.EX_ALERTA_LISTAS_RESTRICTIVAS:
                    mensajeError = Mensajes.EX_051;
                    break;

                case ETipoErrorFramework.EX_FALTA_ZONA_GENERAL:
                    mensajeError = Mensajes.EX_052;
                    break;

                case ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS:
                    mensajeError = Mensajes.EX_053;
                    break;

                case ETipoErrorFramework.EX_USUARIO_NO_PERTENECE_REGIONAL_MAQUINA:
                    mensajeError = Mensajes.EX_054;
                    break;

                case ETipoErrorFramework.EX_CENTRO_SERVICIO_NO_VALIDO:
                    mensajeError = Mensajes.EX_055;
                    break;

                case ETipoErrorFramework.EX_SUCURSAL_NO_VALIDA:
                    mensajeError = Mensajes.EX_056;
                    break;

                case ETipoErrorFramework.EX_VERSION_CLIENTE_NO_CONFIGURADA:
                    mensajeError = Mensajes.EX_057;
                    break;

                case ETipoErrorFramework.EX_CONFIGURACION_CLIENTE_INVALIDA:
                    mensajeError = Mensajes.EX_058;
                    break;

                case ETipoErrorFramework.EX_SUCURSAL_SIN_SERVICIOS:
                    mensajeError = Mensajes.EX_059;
                    break;

                case ETipoErrorFramework.EX_NO_SE_PUEDE_ASIGNAR_CONSECUTIVO:
                    mensajeError = Mensajes.EX_060;
                    break;

                case ETipoErrorFramework.EX_FALLO_ADJUNTAR_ARCHIVO:
                    mensajeError = Mensajes.EX_061;
                    break;

                case ETipoErrorFramework.EX_ALERTA_NO_CONFIGURADA:
                    mensajeError = Mensajes.EX_062;
                    break;

                case ETipoErrorFramework.EX_ERROR_PLANILLA:
                    mensajeError = Mensajes.EX_063;
                    break;

                case ETipoErrorFramework.EX_ERROR_ENTITY_VALITATION:
                    mensajeError = Mensajes.EX_064;
                    break;

                case ETipoErrorFramework.EX_ERROR_OBTENER_NUMERADOR:
                    mensajeError = Mensajes.EX_065;
                    break;

                case ETipoErrorFramework.EX_ERROR_EN_CONSTRAINT:
                    mensajeError = Mensajes.EX_066;
                    break;

                case ETipoErrorFramework.EX_ERROR_NO_TIENE_COMISION_ASIGNADA:
                    mensajeError = Mensajes.EX_067;
                    break;

                case ETipoErrorFramework.EX_ERROR_VALOR_BASE_COMISION_ES_CERO:
                    mensajeError = Mensajes.EX_068;
                    break;

                case ETipoErrorFramework.EX_ERROR_PORCENTAJE_COMISION_RESPONSABLE:
                    mensajeError = Mensajes.EX_069;
                    break;

                case ETipoErrorFramework.EX_ERROR_VALOR_COMISION_RESPONSABLE:
                    mensajeError = Mensajes.EX_070;
                    break;

                case ETipoErrorFramework.EX_ERROR_PORCENTAJE_COMISION_CENTROSERVICIO:
                    mensajeError = Mensajes.EX_071;
                    break;

                case ETipoErrorFramework.EX_ERROR_VALOR_COMISION_CENTROSERVICIO:
                    mensajeError = Mensajes.EX_072;
                    break;

                case ETipoErrorFramework.EX_ERROR_VIOLACION_INEGRIDAD_REFERENCIAL:
                    mensajeError = Mensajes.EX_073;
                    break;

                case ETipoErrorFramework.EX_ERROR_AREA_INTERNA_DEFAULT:
                    mensajeError = Mensajes.EX_074;
                    break;

                case ETipoErrorFramework.EX_ERROR_NO_SE_PUDO_CONECTAR_WS_DOLAR:
                    mensajeError = Mensajes.EX_075;
                    break;

                case ETipoErrorFramework.EX_ERROR_NO_SE_PUDO_EDITAR_CAJA_CONSECUTIVO:
                    mensajeError = Mensajes.EX_076;
                    break;

                case ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO:
                    mensajeError = Mensajes.EX_077;
                    break;

                case ETipoErrorFramework.EX_ERROR_PARAMETROS_NULOS_EJECUCION_REGLA:
                    mensajeError = Mensajes.EX_078;
                    break;

                case ETipoErrorFramework.EX_ERROR_REGLA_NO_IMPLEMENTADA:
                    mensajeError = Mensajes.EX_079;
                    break;

                case ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO:
                    mensajeError = Mensajes.EX_081;
                    break;

                case ETipoErrorFramework.EX_NO_EXISTE_VERSION_RACOL:
                    mensajeError = Mensajes.EX_082;
                    break;

                case ETipoErrorFramework.EX_NO_EXISTE_VERSION_GESTION:
                    mensajeError = Mensajes.EX_083;
                    break;

                case ETipoErrorFramework.EX_NO_EXISTE_PORCENTAJE_RECARGO_COMBUSTIBLE_OP:
                    mensajeError = Mensajes.EX_084;
                    break;

                case ETipoErrorFramework.EX_ERROR_ENTITY_DETALLE:
                    mensajeError = Mensajes.EX_085;
                    break;

                case ETipoErrorFramework.EX_NO_EXISTEN_DESTINATARIOS_CONFIGURADOS:
                    mensajeError = Mensajes.EX_087;
                    break;

                case ETipoErrorFramework.EX_USUARIO_YA_EXISTE:
                    mensajeError = Mensajes.EX_089;
                    break;

                case ETipoErrorFramework.EX_PERSONA_INTERNA_ASOCIADA:
                    mensajeError = Mensajes.EX_090;
                    break;

                case ETipoErrorFramework.EX_USUARIO_ACTIVO_DE_PERSONA_INTERNA:
                    mensajeError = Mensajes.EX_091;
                    break;

                case ETipoErrorFramework.EX_USUARIO_ACTIVO_PARA_UNA_PERSONA_INTERNA:
                    mensajeError = Mensajes.EX_092;
                    break;

                case ETipoErrorFramework.EX_DOCUMENTO_PERSONA_ACTIVO_USUARIO_ACTIVO:
                    mensajeError = Mensajes.EX_093;
                    break;

                case ETipoErrorFramework.EX_USUARIO_ACTIVO_SIN_ASIGNAR:
                    mensajeError = Mensajes.EX_094;
                    break;

                case ETipoErrorFramework.EX_NO_ENCONTRO_CODIGO_INVENTARIO:
                    mensajeError = Mensajes.EX_095;
										break;
								
								case ETipoErrorFramework.EX_DIFERENTE_CENTRO_SERVICIO:
										mensajeError = Mensajes.EX_096;
										break;

                #endregion Mensajes de Excepción

                #region Mensajes Informativos

                case ETipoErrorFramework.IN_MAQUINA_REGISTRADA:
                    mensajeError = Mensajes.IN_001;
                    break;

                #endregion Mensajes Informativos

                default:
                    mensajeError = String.Format(Mensajes.EX_000, tipoErrorFramework.ToString());
                    break;
            }

            return mensajeError;
        }

        /// <summary>
        /// No se encuentra MAC asociada
        /// </summary>
        //public static readonly string EX_ID_NO_IDENTIFICADO = Mensajes.EX_001;
    }
}