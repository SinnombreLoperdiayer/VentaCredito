using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
    public enum EnumTipoNovedadRaps
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

        //AGENCIA
        SAQUEO_AGE= 1068,
        PERDIDA_AGE=1067,
        AVERIA_AGE=1069,

        //PUNTO
        SAQUEO_PTO= 2140,
        HURTO_PTO=2143,

    }



}
