using CO.Servidor.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.ReversionEstadosGuia;
using System.ServiceModel.Activation;
using System.ServiceModel;
using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using System.Transactions;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.Servicios.Implementacion.ReversionEstados
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ADReversionEstadosSvc : IADReversionEstadosSvc
    {
        public void GrabarHistoricoEstadoGuia(ReversionEstado reversionEstado)
        {           
            
        }

        public ADGuia ObtenerEstadoGuia(long numeroGuia)
        {
            return ADAdministradorReversionEstadosGuia.Instancia.ObtenerEstadoGuia(numeroGuia);
        }

        public List<ADTrazaGuia> ObtenerTrazaGuia(long numeroGuia)
        {
            return ADAdministradorReversionEstadosGuia.Instancia.ObtenerTrazaGuia(numeroGuia);
        }

        public bool VerificarCambioEstadoPermitido(long numeroGuia, int idEstadoOrigen, int idEstadoSolicitado)
        {
            //Se debe implementar la consulta del estado en la traza actual de la guia
            //Verificar que la guia se encuentre en el estado origen del ticket
            //verificar que el estado solicitado este permitido en la matriz
            throw new NotImplementedException();
        }

        public bool VerificarExistenciaGuia(long numeroGuia)
        {
            return ADAdministradorReversionEstadosGuia.Instancia.VerificarExistenciaGuia(numeroGuia);
        }

        public bool VerificarGuiaPQRS(long numeroGuia)
        {
            return ADAdministradorReversionEstadosGuia.Instancia.VerificarGuiaPQRS(numeroGuia);
        }

        public List<DatosPersonaNovasoftDC> ObtenerEmpleadosNovasoft()
        {
            return ADAdministradorReversionEstadosGuia.Instancia.ObtenerEmpleadosNovasoft();
        }

        public ReversionEstado ObtenerCambioEstadoAnterior(long numeroGuia)
        {
            return ADAdministradorReversionEstadosGuia.Instancia.ObtenerCambioEstadoAnterior(numeroGuia);
        }

        public bool GrabarCambioDeEstadoGuia(ReversionEstado reversionEstado)
        {
            return ADAdministradorReversionEstadosGuia.Instancia.GrabarCambioDeEstadoGuia(reversionEstado);
        }
    }
}
