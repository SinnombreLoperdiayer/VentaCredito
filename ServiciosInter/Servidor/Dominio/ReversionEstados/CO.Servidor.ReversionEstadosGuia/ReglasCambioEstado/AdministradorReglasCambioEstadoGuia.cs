using CO.Servidor.ReversionEstadosGuia.Datos;
using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ReversionEstadosGuia.ReglasCambioEstado
{
    public class AdministradorReglasCambioEstadoGuia
    {
        private List<ParametrizacionBorradoTablasPorEstado> listaParametrizacionReglasEstado;
        private int idEstadoGuiaSolicitado;
        private Dictionary<int, IReglasCambioEstado> listaEjecucionFunciones;
        private ReversionEstado reversionEstadoLocal;

        public AdministradorReglasCambioEstadoGuia(ReversionEstado reversionEstado)
        {
            idEstadoGuiaSolicitado = reversionEstado.IdEstadoSolicitado;
            reversionEstadoLocal = reversionEstado;
            listaEjecucionFunciones = new Dictionary<int, IReglasCambioEstado>();

            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.ACTUALIZAR_ADMISION.GetHashCode(), new ReglaActualizacionAdmisionMensajeria());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_ARCHIVO_GUIA.GetHashCode(), new ReglaBorrarAlmacenArchivoGuia());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_PLANILLA_ASIGNACION_GUIA.GetHashCode(), new ReglaBorrarPlanillaAsignacionGuia());            
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_ESTADO_GUIA_TIPO_IMPRESO.GetHashCode(), new ReglaBorrarEstadoGuiaTipoImpreso());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_ESTADO_GUIA_TRAZA.GetHashCode(), new ReglaBorrarEstadoGuiaTraza());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_TELEMERCADEO.GetHashCode(), new ReglaBorrarGestionGuiaTelemercadeo());
            //listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_PLANILLA_ASIGNACION_GUIA.GetHashCode(), new ReglaBorrarPlanillaAsignacionGuia());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.ACTUALIZAR_PLANILLA_ASIGNACION_GUIA.GetHashCode(), new ReglaActualizarPlanillaAsignacionGuia());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_MOTIVO_GUIA.GetHashCode(), new ReglaBorrarEstadoGuiaMotivo());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_ALMACEN_GUIA.GetHashCode(), new ReglaBorrarAlmacenGuia());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.ACTUALIZAR_MOVIMIENTO_INVENTARIO.GetHashCode(), new ReglaActualizarMovimientoInventario());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_EVIDENCIA_DEVOLUCION.GetHashCode(), new ReglaBorrarEvidenciaDevolucion());
            listaEjecucionFunciones.Add(EnumFuncionesReglasCambioEstado.BORRAR_ARCHIVO_EVIDENCIA.GetHashCode(), new ReglaBorrarArchivoEvidencia());            
        }

        private void ConsultarParametrizacion(int idEstadoDestino)
        {

            listaParametrizacionReglasEstado = ADReversionEstadosRepositorio.Instancia.ObtenerParametrosReglasCambioEstado(idEstadoGuiaSolicitado);
        }

        public void EjecucionReglasCambioEstado()
        {           
            if (listaEjecucionFunciones == null)
            {
                throw new Exception("No existen parametros para el cambio de estado solicitado");
            }

            if(reversionEstadoLocal == null)
            {
                throw new Exception("La reversion no puede ser ejecutada por falta de información");
            }

            ConsultarParametrizacion(idEstadoGuiaSolicitado);

            foreach (var item in listaParametrizacionReglasEstado)
            {
                listaEjecucionFunciones[item.IdFuncionRegla].EjecucionRegla(reversionEstadoLocal);
            }
        }


    }
}
