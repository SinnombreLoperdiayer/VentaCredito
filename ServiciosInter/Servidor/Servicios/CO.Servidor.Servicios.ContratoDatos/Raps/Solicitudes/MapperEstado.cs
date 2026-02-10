
using System.Collections.Generic;
namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    public static class MapperEstado
    {
        public static Dictionary<int, RAEnumEstados> Acciones
        {
            get
            {
                return new Dictionary<int, RAEnumEstados>
                    {
                        {0,RAEnumEstados.None      },
                        {1,RAEnumEstados.Creada    },
                        {2,RAEnumEstados.Respuesta },
                        {3,RAEnumEstados.Revisado  },
                        {4,RAEnumEstados.Escalado  },
                        {5,RAEnumEstados.Asignado  },
                        {6,RAEnumEstados.Cerrado   },
                        {7,RAEnumEstados.Rechazado },
                        {8,RAEnumEstados.Cancelado },
                        {9,RAEnumEstados.Vencido },
                        {10,RAEnumEstados.Reasignado },
                    };
            }
        }
        public static RAEnumEstados ToEnumEstados(int p)
        {            
            var acciones = MapperEstado.Acciones;

            RAEnumEstados resultado = RAEnumEstados.None;

            if (acciones.ContainsKey(p))
            {
                resultado = acciones[p];
            }

            return resultado;
        }
    }
}
