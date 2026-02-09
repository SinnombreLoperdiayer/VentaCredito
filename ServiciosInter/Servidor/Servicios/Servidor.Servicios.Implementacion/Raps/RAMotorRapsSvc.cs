using CO.Servidor.Raps;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;

namespace CO.Servidor.Servicios.Implementacion.Raps
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RAMotorRapsSvc
    {


         #region contructor
        
        public RAMotorRapsSvc ()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
        }

        #endregion

         /// <summary>
        /// Realiza el proceso de escalonamiento de las solicitudes vencidas, solo se utiliza en el motorRaps
        /// </summary>
        public void EscalarSolicitudesVencidas()
        {
            RAMotorRaps.Instancia.EscalarSolicitudesVencidas();

        }

        /// <summary>
        /// Realiza el proceso de creacion de las solicitudes automaticas, solo se utiliza en el motorRaps
        /// </summary>
        public void CrearSolicitudesAutomaticasSinSistemaFuente()
        {
            RAMotorRaps.Instancia.CrearSolicitudesAutomaticasSinSistemaFuente();
        }

        /// <summary>
        /// Realiza el proceso de creacion de las solicitudes automaticas, con sistema fuente, solo se utiliza en el motorRaps
        /// </summary>
        public void CrearSolicitudesAutomaticasConSistemaFuente()
        {
            RAMotorRaps.Instancia.CrearSolicitudesAutomaticasConSistemaFuente();
        }

        #region MotorCitas
        /// <summary>
        /// Envia los recordatorios de las citas a los integrantes
        /// </summary>
        public void EnviarRecordatoriosCitasMotor()
        {
            RAMotorRaps.Instancia.EnviarRecordatoriosCitasMotor();
        }

        #endregion


    }
}
