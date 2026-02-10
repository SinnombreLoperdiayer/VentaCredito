using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CO.Servidor.Servicios.Contratos
{
    [ServiceContract(Namespace = "http://contrologis.com")]
    public interface IADReversionEstadosSvc
    {
        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ADGuia ObtenerEstadoGuia(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<ADTrazaGuia> ObtenerTrazaGuia(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        void GrabarHistoricoEstadoGuia(ReversionEstado reversionEstado);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificarCambioEstadoPermitido(long numeroGuia, int idEstadoOrigen, int idEstadoSolicitado);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificarExistenciaGuia(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool VerificarGuiaPQRS(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        ReversionEstado ObtenerCambioEstadoAnterior(long numeroGuia);

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        List<DatosPersonaNovasoftDC> ObtenerEmpleadosNovasoft();

        [OperationContract]
        [FaultContract(typeof(ControllerException))]
        bool GrabarCambioDeEstadoGuia(ReversionEstado reversionEstado);

    }
}
