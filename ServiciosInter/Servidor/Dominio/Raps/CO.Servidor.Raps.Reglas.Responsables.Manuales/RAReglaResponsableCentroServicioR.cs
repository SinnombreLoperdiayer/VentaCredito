using Framework.Servidor.Comun.Reglas;
using System;
using System.Collections.Generic;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Dominio.Comun.Raps;
using CO.Servidor.Dominio.Comun.Middleware;
//using CO.Servidor.Raps.Datos;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class RAReglaResponsableCentroServicioR : IReglaIntegraciones
    {
        private IRAFachadaRaps fachadaRaps = COFabricaDominio.Instancia.CrearInstancia<IRAFachadaRaps>();

        public RAResponsableDC ObtenerResponsableNovedadRaps(IDictionary<string, object> parametrosRegla)
        {
            RAResponsableDC responsable = new RAResponsableDC();
            RACargoEscalarDC datosEmpleadoParaAsignarFalla = new RACargoEscalarDC();
            if (parametrosRegla.ContainsKey("responsableFalla"))
            {
                string identificacionResponsableFalla = parametrosRegla["responsableFalla"].ToString();
                datosEmpleadoParaAsignarFalla = fachadaRaps.ObtenerResponsableCentroServicioParaAsignarRaps(identificacionResponsableFalla);
                datosEmpleadoParaAsignarFalla.HorarioEmpleado = fachadaRaps.ObtenerHorariosEmpleadoPorIdentificacion(String.IsNullOrEmpty(datosEmpleadoParaAsignarFalla.DocumentoEmpleado) ? "" : datosEmpleadoParaAsignarFalla.DocumentoEmpleado);
                responsable.CargoEscalona = datosEmpleadoParaAsignarFalla;
            }
            return responsable;
        }
    }
}
