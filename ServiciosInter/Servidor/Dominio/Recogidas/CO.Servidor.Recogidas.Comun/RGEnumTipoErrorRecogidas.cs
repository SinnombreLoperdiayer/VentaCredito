using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Recogidas.Comun
{
    public enum RGEnumTipoErrorRecogidas
    {
        #region Mensajes de Excepción

        EX_ERROR_NO_IDENTIFICADO = 0,
        EX_PARAMETRIZACION_INVALIDA = 1,
        EX_RECOGIDA_NO_EXISTE = 2,        

        /// <summary>
        /// El número del sOLICITUD RECOGIDA no existe
        /// </summary>
        EX_NUMERO_SOL_RECOGIDA_NO_EXISTE = 3,

        /// <summary>
        /// La guía ya fue entregada
        /// </summary>
        EX_RECOGIDA_EJECUTADA  = 4,

        /// <summary>
        /// No se encontró ningún cliente convenio con  ese número de guía
        /// </summary>
        EX_RECOGIDA_NO_ES_ESPORADICA = 5,

        EX_SOLICITUD_RECOGIDA_NO_ESPECIFICADO,


        #endregion Mensajes de Excepción
    }
}
