using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using Framework.Servidor.Excepciones;
using System.ServiceModel;

namespace CO.Servidor.Integraciones
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IIntegracionSISPostal
    {
        /// <summary>
        /// Permite Crear una Admision desde SisPostal
        /// </summary>
        /// <param name="Credencial"></param>
        /// <param name="admision"></param>
        /// <returns>Identificador de la admision</returns>
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        long AdicionarAdmisionSisPostal(credencialDTO Credencial, ADGuiaSisPostal admision);
    }
}