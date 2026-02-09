using ServiciosInter.DatosCompartidos.Wrappers.Preenvios;

namespace ServiciosInter.Negocio.Interfaces
{
    interface IPreenviosNegocio
    {
        /// <summary>
        /// Obtiene la información de un preenvio
        /// </summary>
        /// <param name="PreEnvio">número de preenvio</param>
        /// <returns></returns>
        PreenvioAdmisionWrapper obtenerPreenvioPorNumero(long PreEnvio);
    }
}
