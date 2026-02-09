using CO.Servidor.Recogidas;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.Implementacion.Recogidas
{
    public class RGRecogidasMotorSvc
    {

        /// <summary>
        /// Vence las solicitudes asignadas y retorna la lista de dispositivos para notificar el vencimiento
        /// </summary>
        /// <returns></returns>
        public List<PADispositivoMovil> VencerSolicitudesRecogidas()
        {
            return RGRecogidasMotor.Instancia.VencerSolicitudesRecogidas();
        }

        /// <summary>
        /// Obtiene las recogidas nuevas para notificar a los mensajeros y obtiene las recogidas para cambiar de estado a ParaForzar
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidaMotorDC> ObtenerRecogidasNuevasNotificarForzar()
        {
            return RGRecogidasMotor.Instancia.ObtenerRecogidasNuevasNotificarForzar();
        }

        /// <summary>
        /// aumenta el contador de las recogidas nuevas o canceladas por mensajero
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="esNueva"></param>
        public void AumentarContadorNotificacionRecogidas(long idSolicitudRecogida, bool esNueva)
        {
            RGRecogidasMotor.Instancia.AumentarContadorNotificacionRecogidas(idSolicitudRecogida, esNueva);
        }
    }
}
