using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Raps.Comun
{
    public enum RAEnumTipoDato : int
    {
        /// <summary>
        /// IDENTIFICADOR PARA DATOS ENTEROS
        /// </summary>
        NUMERO = 1,

        /// <summary>
        /// IDENTIFICADOR PARA DATOS DE TIPO TEXTO
        /// </summary>
        CADENA = 2,

        /// <summary>
        /// IDENTIFICADOR PARA DATOS DE TIPO FECHA
        /// </summary>
        FECHA = 3,

        /// <summary>
        /// IDENTIFICADOR PARA DATOS DE TIPO NOVEDAD
        /// </summary>
        TIPONOVEDAD = 4,


        FOTOGRAFIA = 5,

        ADJUNTO = 6
    }
}
