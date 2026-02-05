using VentaCredito.Transversal.Entidades.Planilla;

namespace VentaCredito.Planilla.Interfaces
{
    public interface IPlanillaNegocio
    {
        string ReservarConsolidadoPrecinto(CAReservaPrecintoConsolidado reserva);

        MensajeroRecogida ObtenerDatosMensajero(ConsultaMensajero consultaMesajero);
        ResponsePlanilla CrearPlanillaCentroServicio(RequestPlanilla requestPlanilla);
        PlanillaRecoleccionPreenviosResponse GenerarPlanillaRecoleccionPreenvios(PlanillaRecoleccionPreenviosRequest planillaRecoleccion);
    }
}
