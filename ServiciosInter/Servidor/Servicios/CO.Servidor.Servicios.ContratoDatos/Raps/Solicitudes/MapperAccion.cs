
using System.Collections.Generic;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    public static class MapperAccion
    {
        public static Dictionary<int, RAEnumAccion> Acciones {
            get
            {
                return new Dictionary<int, RAEnumAccion>
                    {
                        {0,RAEnumAccion.None},
                        {1,RAEnumAccion.Crear    },
                        {2,RAEnumAccion.Gestionar},
                        {3,RAEnumAccion.Revisar  },
                        {4,RAEnumAccion.Escalar  },
                        {5,RAEnumAccion.Asignar  },
                        {6,RAEnumAccion.Cerrar   },
                        {7,RAEnumAccion.Vencer   },
                    };
            }
            }

        public static RAEnumAccion ToEnumAccion(byte p)
        {
            var acciones = MapperAccion.Acciones;

            RAEnumAccion resultado = RAEnumAccion.None;

            if (acciones.ContainsKey(p))
            {
                resultado = acciones[p];
            }

            return resultado;
        }
    }
}
