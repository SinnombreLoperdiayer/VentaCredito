using System;

namespace CO.Servidor.Raps.Comun
{
    public class RAMensajesRaps
    {
        public static string CargarMensaje(RAEnumTipoErrorClientes tipoErrorRaps)
        {
            string mensajeError;

            switch (tipoErrorRaps)
            {
                #region excepciones
                case RAEnumTipoErrorClientes.EX_INSERTAR_PARAMETRIZACION:
                    mensajeError = RAMensajes.EX_001;
                    break;
                case RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD:
                    mensajeError = RAMensajes.EX_002;
                    break;
                case RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO:
                    mensajeError = RAMensajes.EX_003;
                    break;
                case RAEnumTipoErrorClientes.EX_NO_ES_POSIBLE_CERRAR_LA_SOLICITUD:
                    mensajeError = RAMensajes.EX_004;
                    break;
                case RAEnumTipoErrorClientes.EX_SOL_CAMBIO_DE_ESTADO:
                    mensajeError = RAMensajes.EX_005;
                    break;
                case RAEnumTipoErrorClientes.EX_CONSULTA_TRAZA_POR_ESTADO:
                    mensajeError = RAMensajes.EX_006;
                    break;
                case RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_CENTRO_SERVICIO:
                    mensajeError = RAMensajes.EX_007;
                    break;
                case RAEnumTipoErrorClientes.EX_CONSULTA_DATOS_MENSAJERO:
                    mensajeError = RAMensajes.EX_008;
                    break;
                case RAEnumTipoErrorClientes.EX_CONSULTA_RESPONSABLE_SUMINISTRO:
                    mensajeError = RAMensajes.EX_009;
                    break;
                case RAEnumTipoErrorClientes.EX_CONSULTA_GUIA_NO_EXISTE:
                    mensajeError = RAMensajes.EX_010;
                    break;
                case RAEnumTipoErrorClientes.EX_CONSULTA_NOVEDAD_HIJA:
                    mensajeError = RAMensajes.EX_011;
                    break;
                case RAEnumTipoErrorClientes.EX_FALLA_YA_REGISTRADA_MISMO_RESPONSABLE:
                    mensajeError = RAMensajes.EX_012;
                    break;
                case RAEnumTipoErrorClientes.EX_NO_EXISTE_REGLA_PARA_ESTADO:
                    mensajeError = RAMensajes.EX_013;
                    break;
                case RAEnumTipoErrorClientes.EX_NO_ASIGNO_TIPONOVEDAD:
                    mensajeError = RAMensajes.EX_014;
                    break;
                case RAEnumTipoErrorClientes.EX_NO_RESPONSABLE_MANIFIESTO:
                    mensajeError = RAMensajes.EX_015;
                    break;
                case RAEnumTipoErrorClientes.EX_RESPONSABLE_DIFERENTE_AGE_PTO:
                    mensajeError = RAMensajes.EX_016;
                    break;
                case RAEnumTipoErrorClientes.EX_RESPONSABLE_DIFERENTE_RACOL:
                    mensajeError = RAMensajes.EX_017;
                    break;
                case RAEnumTipoErrorClientes.EX_NO_CONTIENE_ID_CENTRO_SERVICIO_ESTADO:
                    mensajeError = RAMensajes.EX_018;
                    break;


                #endregion
                default:
                    mensajeError = String.Format(RAMensajes.EX_000, tipoErrorRaps.ToString());
                    break;
            }
            return mensajeError;
        }
    }
}
