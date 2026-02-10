using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.LogisticaInversa.Comun
{
    public enum LOIEnumTipoDocumento : short
    {
        /// <summary>
        /// Certificaciones de entrega
        /// </summary>
        CERTIFICACION_ENTREGA = 1,

        /// <summary>
        /// Certificaciones de devolucion
        /// </summary>
        CERTIFICACION_DEVOLUCION = 2,

        /// <summary>
        /// Guias internas de las planillas
        /// </summary>
        GUIA_INTERNA_PLANILLA = 3,

        /// <summary>
        /// Guias internas de las certificaciones
        /// </summary>
        GUIA_INTERNA_CERTIFICACION = 4,

        /// <summary>
        /// Planilla de certificaciones
        /// </summary>
        PLANILLA_CERTIFICACION = 5,

        /// <summary>
        /// Planilla de certificaciones
        /// </summary>
        PLANILLA_CERTIFICACION_GUIA = 6,

        /// <summary>
        /// Prueba Entrega Cliente DIAN
        /// </summary>
        PRUEBA_ENTREGA = 7,

         /// <summary>
        /// Prueba DEVOLUCION Cliente DIAN
        /// </summary>
        PRUEBA_DEVOLUCION = 8
    }
}