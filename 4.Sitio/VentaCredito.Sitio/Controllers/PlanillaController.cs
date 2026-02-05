using CustomException;
using System.Web.Http;
using VentaCredito.Planilla;
using VentaCredito.Planilla.Interfaces;
using VentaCredito.Sitio.Seguridad;
using VentaCredito.Transversal.Entidades.Planilla;

namespace VentaCredito.Sitio.Controllers
{
    [RoutePrefix("api/Planilla")]
    public class PlanillaController : ApiController
    {
        //Crear instancia de negocio
        private readonly IPlanillaNegocio negocio = PlanillaNegocio.Instancia;

        /// <summary>
        /// Realiza validaciones del consolidado y cambia el estado
        /// </summary>
        /// <param name="reserva"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize]
        //[AdministradorSeguridad]
        [Route("ReservarConsolidadoPrecinto")]
        [LogExceptionFilter]
        public IHttpActionResult ReservarConsolidadoPrecinto(CAReservaPrecintoConsolidado reserva)
        {
            return Ok(negocio.ReservarConsolidadoPrecinto(reserva));
        }

        /// <summary>
        /// Obtiene la información del mensajero para la recogida del centro servicio
        /// </summary>
        /// <param name="consultaMesajero"></param>
        /// <returns></returns>
        [HttpPost]
        //[AdministradorSeguridad]
        [Route("ObtenerDatosMensajero")]
        [LogExceptionFilter]
        public IHttpActionResult ObtenerDatosMensajero(ConsultaMensajero consultaMesajero)
        {
            return Ok(negocio.ObtenerDatosMensajero(consultaMesajero));
        }

        /// <summary>
        /// Crea planilla de recolección por centro de servicio
        /// </summary>
        /// <param name="requestPlanilla"></param>
        /// <returns></returns>
        [HttpPost]
        //[AdministradorSeguridad]
        [Route("CrearPlanillaCentroServicio")]
        [LogExceptionFilter]
        public IHttpActionResult CrearPlanillaCentroServicio(RequestPlanilla requestPlanilla)
        {
            return Ok(negocio.CrearPlanillaCentroServicio(requestPlanilla));
        }
        /// <summary>
        /// Genera la planilla de recolección de preenvios.
        /// Alejandro Cardenas 16/07/2021
        /// </summary>
        /// <param name="planillaRecoleccion"></param>
        /// <returns></returns>
        [HttpPost]
        [AdministradorSeguridad]
        [Route("GenerarPlanillaRecoleccionPreenvios")]
        [LogExceptionFilter]
        public PlanillaRecoleccionPreenviosResponse GenerarPlanillaRecoleccionPreenvios([FromBody] PlanillaRecoleccionPreenviosRequest planillaRecoleccion)
        {
            return negocio.GenerarPlanillaRecoleccionPreenvios(planillaRecoleccion);
        }
    }
}
