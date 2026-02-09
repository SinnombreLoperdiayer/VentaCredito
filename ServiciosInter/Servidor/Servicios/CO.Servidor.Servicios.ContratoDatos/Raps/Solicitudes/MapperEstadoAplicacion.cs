using System.Collections.Generic;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    public static class MapperEstadoAplicacion
    {
        public static Dictionary<int, EnumEstadosAplicacion> Acciones
        {
            get
            {
                return new Dictionary<int, EnumEstadosAplicacion>
                    {
                        {0,EnumEstadosAplicacion.None      },
                        {1,EnumEstadosAplicacion.PorResolver},
                        {2,EnumEstadosAplicacion.Rechazadas},
                        {3,EnumEstadosAplicacion.Resueltas},
                        {4,EnumEstadosAplicacion.VencidasSinRevision},
                        {5,EnumEstadosAplicacion.Vencidas },
                        {6,EnumEstadosAplicacion.Vigentes },
                        {7,EnumEstadosAplicacion.Canceladas },
                        {6,EnumEstadosAplicacion.CerradasExitosas },
                        {6,EnumEstadosAplicacion.CerradasVencidas },
                    };
            }
        }

        public static EnumEstadosAplicacion ToEnumEstadosAplicacion(int p)
        {
            var acciones = MapperEstadoAplicacion.Acciones;

            EnumEstadosAplicacion resultado = EnumEstadosAplicacion.None;

            if (acciones.ContainsKey(p))
            {
                resultado = acciones[p];
            }

            return resultado;
        }
    }
}
