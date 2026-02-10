using ServiciosInter.DatosCompartidos.Wrappers;

namespace ServiciosInter.Negocio.Interfaces
{
    public interface IExploradorGirosNegocio
    {
        /// <summary>
        /// Obtiene la información de giro
        /// </summary>
        /// <param name="informacionGiro">wrapper con información de giro</param>
        /// <returns></returns>
        ExploradorGirosWrapper ObtenerDatosGiros(ExploradorGirosWrapper informacionGiro);
    }
}