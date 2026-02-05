using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Transversal.Entidades.Planilla;

namespace VentaCredito.Planilla.Datos.Interfaces
{
    public interface IPlanillaDatos
    {
        int InsertarMovimientoConsolidado(CAMovimientoConsolidadoDCIntegra movimientoConsolidado);

        bool ValidarInsertarMovimientoConsolidadoUrbano(CAMovimientoConsolidadoDCIntegra movimientoConsolidado, int idEstado);

        bool ValidarInsertarMovimientoPrecinto(LogMovimientoPrecintoPUAIntegra logMovimiento, int idEstado, long idCentroServicio);

        string InsertarLogMovimientoPrecinto(LogMovimientoPrecintoPUAIntegra logMovimiento);
        Persona_Huella_AUT ObtenerHuellaMensajero(string numeroDocumento);

        MensajeroPlanilla ObtenerDatosMensajeroXDocumento(string numeroDocumento);

        CentroServicioPlanilla ObtenerInfoCentroServicioXId(long IdCentroServicio);

        DateTime ObtenerFechaPlanilla(long idPlanilla);
        int ObtenerCantidadEnviosSinPlanillarXIdCentroSrvicio(long IdCentroservicio);




    }
}
