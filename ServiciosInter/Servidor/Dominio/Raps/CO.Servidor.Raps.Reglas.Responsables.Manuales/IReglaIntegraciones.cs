using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.Comun.Reglas
{
    public interface IReglaIntegraciones
    {
        /// <summary>
        /// Obtiene responsable novedad raps
        /// </summary>
        /// <param name="parametrosRegla"></param>
        /// <returns></returns>
        RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla);


    }
}
