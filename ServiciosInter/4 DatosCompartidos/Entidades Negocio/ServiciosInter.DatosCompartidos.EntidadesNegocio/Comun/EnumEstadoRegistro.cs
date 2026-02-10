using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun
{
    public enum EnumEstadoRegistro
    {
        /// <summary>
        /// Sin cambios en el registro
        /// </summary>        
        SIN_CAMBIOS = 0x1,

        /// <summary>
        /// Registro adicionado
        /// </summary>        
        ADICIONADO = 0x2,

        /// <summary>
        /// Registro modificado
        /// </summary>        
        MODIFICADO = 0x4,

        /// <summary>
        /// Registro borrado
        /// </summary>        
        BORRADO = 0x8
    }
}
