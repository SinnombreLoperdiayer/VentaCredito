using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System.Collections.Generic;
using System.Linq;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.Raps
{
    public static class MapperParametrizacionRaps
    {
        public static List<NombresParametrizacionRaps> ToNombresParametrizacionRaps(List<RAParametrizacionRapsDC> idParametrizacionRap)
        {
            List<NombresParametrizacionRaps> resultado = null;

            if (idParametrizacionRap == null)
            {
                return null;
            }

            resultado = (from par in idParametrizacionRap
                         select new NombresParametrizacionRaps
                                 {
                                     IdParametrizacionRap = par.IdParametrizacionRap,
                                     Nombre = par.Nombre,
                                     Estado = par.Estado,
                                     IdParametrizacionPadre = par.IdParametrizacionPadre
                                 }).ToList();

            return resultado;
        }

        public static List<NombreListarParametroTipoRap> ToNombreListarParametroTipoRap(IEnumerable<RAParametrizacionRapsDC> parametros)
        {

            return parametros == null 
                ? null 
                : (from p in parametros
                        select new NombreListarParametroTipoRap
                                {
                                    IdParametrizacionRap = p.IdParametrizacionRap,
                                    Nombre = p.Nombre
                                }).ToList();
        }

        //internal static List<ContratoDatos.Raps.Consultas.RAObtenerListaSolicitudesRaps> ObtenerListaSolicitudesRaps(List<ContratoDatos.Raps.Consultas.RAObtenerListaSolicitudesRaps> DocumentoSolicita, IdEstado)
        //{
        //    List<ContratoDatos.Raps.Consultas.RAObtenerListaSolicitudesRaps> resultado = null;
        //}
    }
}