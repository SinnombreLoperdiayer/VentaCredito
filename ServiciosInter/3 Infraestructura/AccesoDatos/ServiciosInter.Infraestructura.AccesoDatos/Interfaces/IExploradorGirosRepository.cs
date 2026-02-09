using System.Collections.Generic;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros;
using ServiciosInter.DatosCompartidos.Wrappers;

namespace ServiciosInter.Infraestructura.AccesoDatos.Interfaces
{
    public interface IExploradorGirosRepository
    {
        /// <summary>
        /// Obtiene los estados de un giro a traves del tiempo
        /// </summary>
        /// <param name="idAdminGiro">Es el idAdmin del Giro</param>
        /// <returns>Lista de estados y Fecha</returns>
        IList<EstadosGiro_GIR> ObtenerEstadosGiro(long idAdminGiro);

        /// <summary>
        /// Obtiena la información del giro
        /// </summary>
        /// <param name="informacionGiro"></param>
        /// <returns>Mapping de registro en tabla AdmisionGiros_GIR</returns>
        ExploradorGirosWrapper ObtenerDatosGiros(ExploradorGirosWrapper informacionGiro);
    }
}