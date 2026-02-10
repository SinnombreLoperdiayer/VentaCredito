using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun
{
    public enum CoEnumTipoNovedadRaps
    {

        #region Controller
        Pordefecto = 0,
        NoAlcanzo = 1,
        NoDescargo = 2,
        FueraDeZona = 3,
        MotivoDevolucionFalsa = 4,
        FirmaFalsificada = 5,
        Hurto_CONTROLLER = 23,
        Saqueo_CONTROLLER = 24,
        Averia_CONTROLLER = 25,
        No_diligencio_Intento_Fallido_CONTROLLER = 26,
        No_Reporto_Ventas_CONTROLLER = 27,
        No_Reporto_Dinero_Alcobro_CONTROLLER = 28,
        No_descargoAPP_CONTROLLER = 29,
        Resagado_En_Origen_CONTROLLER = 31,
        No_remitio_copia_de_contabilidad_CONTROLLER = 32,
        La_Agencia_no_realiza_compensacion_CONTROLLER = 54,
        Envios_Rezagados_Destino_CONTROLLER = 55,
        Envios_Rezagados_Origen_CONTROLLER = 56,


        #endregion

        #region ControllerWeb
        NoCumplioConHorario_ControllerWeb = 30,
        #endregion

        //TIPO Novedad APP
        MAL_VESTIDO_InterLogisAPP = 33,
        NO_CONTESTA_CELULAR_InterLogisAPP = 47,
        AL_COBRO_NO_PERMITIDO_InterLogisAPP = 49,
        PROHIBIDA_CIRCULACION_InterLogisAPP = 39,
        ENTREGA_OTRAS_EMPRESAS_InterLogisAPP = 41,
        GUIA_MAL_EDITADA_InterLogisAPP = 43,
        FIRMA_CLASIFICADA_InterLogisAPP = 45,
        BILLETE_FALSO_InterLogisAPP = 52,

        //TIPO Novedad WEB
        MAL_VESTIDO_WEB = 34,
        NO_CONTESTA_CELULAR_WEB = 48,
        AL_COBRO_NO_PERMITIDO_WEB = 51,
        PROHIBIDA_CIRCULACION_WEB = 40,
        ENTREGA_OTRAS_EMPRESAS_WEB = 42,
        GUIA_MAL_EDITADA_WEB = 44,
        FIRMA_CLASIFICADA_WEB = 46,
        BILLETE_FALSO_WEB = 53,

        // INTEGRACIONES AUTOMATICAS                    

        SAQUEO_AGENCIA_AUTO = 85,
        AVERIA_AGENCIA_AUTO = 87,
        MALA_PRESENTACIÓN_PERSONAL_AUTO = 102,
        NO_REALIZÓ_RECOGIDA_ESPORADICA_PUNTO_AUTO = 169,
        NO_REALIZÓ_RECOGIDA_FIJA_MENSAJERO_AUTO = 170,
        NO_REALIZÓ_RECOGIDA_ESPORADICA_MENSAJERO_AUTO = 171,
        GUIA_MAL_EDITADA_DATOS_AGE_AUTO = 178,
        GUIA_MAL_EDITADA_DATOS_PTO_AUTO = 179,
        ASOCIACIÓN_GUIAS_NN_AGE_AUTO = 180,
        ASOCIACIÓN_GUIAS_NN_PTO_AUTO = 181,
        GUIA_MAL_DILIGENCIADA_PESO_PTO_AUTO = 173,
        GUIA_MAL_DILIGENCIADA_TIPO_SERVICIO_PTO_AUTO = 175,
        GUIA_MAL_DILIGENCIADA_PESO_AGE_AUTO = 172,
        GUIA_MAL_DILIGENCIADA_TIPO_SERVICIO_AGE_AUTO = 174,

        // INTEGRACIONES MANUALES 

        NO_TIENE_QR_PUNTO = 83,
        NO_TIENE_QR_CLIENTE_CRE = 84,
        PROHIBIDA_CIRCULACION_MEN_MAN = 119,
        PROHIBIDA_CIRCULACION_AGE_MAN = 120,
        PROHIBIDA_CIRCULACION_PTO_MAN = 121,
        MALA_ATENCIÓN_MENSAJERO_MAN = 122,
        MALA_ATENCIÓN_AGENCIA_MAN = 123,
        MALA_ATENCIÓN_PUNTO_MAN = 124,
        MALA_ATENCIÓN_NO_CONOCE_MENSAJERO_MAN = 125,
        MALA_DILIGENCIADA_MENSAJERO_MAN = 127,
        MALA_DILIGENCIADA_AGENCIA_MAN = 128,
        MALA_DILIGENCIADA_PUNTO_MAN = 129,
        GUIA_MAL_LIQUIDADA_PESO_MENSAJERO_MAN = 131,
        GUIA_MAL_LIQUIDADA_PESO_AGENCIA_MAN = 132,
        GUIA_MAL_LIQUIDADA_PESO_PUNTO_MAN = 133,
        GUIA_MAL_LIQUIDADA_SERVICIO_MENSAJERO_MAN = 134,
        GUIA_MAL_LIQUIDADA_SERVICIO_AGENCIA_MAN = 135,
        GUIA_MAL_LIQUIDADA_SERVICIO_PUNTO_MAN = 136,
        GUIA_MAL_LIQUIDADA_POR_VALOR_COMERCIAL_MENSAJERO_MAN = 137,
        GUIA_MAL_LIQUIDADA_POR_VALOR_COMERCIAL_AGENCIA_MAN = 138,
        GUIA_MAL_LIQUIDADA_POR_VALOR_COMERCIAL_PUNTO_MAN = 139,
        SAQUEO_MEN_MAN = 151,
        SAQUEO_AGE_MAN = 152,
        SAQUEO_PTO_MAN = 153,
        PERDIDAHURTO_MEN_MAN = 154,
        PERDIDAHURTO_AGE_MAN = 156,
        PERDIDAHURTO_CENTRO_ACOPIO_MAN = 157,
        FIRMA_FALSIFICADA_AGE_MAN = 158,
        AVERÍA_MEN_MAN = 159,
        AVERÍA_AGE_MAN = 160,
        AVERÍA_PTO_MAN = 161,
        AVERÍA_CENTRO_ACOPIO_MAN = 162,
        ADMISIÓN_ENVÍO_ALCOBRO_NO_PERMITIDO_MEN_MAN = 163,
        ADMISIÓN_ENVÍO_ALCOBRO_NO_PERMITIDO_AGE_MAN = 164,
        ADMISIÓN_ENVÍO_ALCOBRO_NO_PERMITIDO_PTO_MAN = 165,
        PERDIDAHURTO_PTO_MAN = 166,
        GUÍA_MAL_EDITADA_PTO_MAN = 167,


    }


}