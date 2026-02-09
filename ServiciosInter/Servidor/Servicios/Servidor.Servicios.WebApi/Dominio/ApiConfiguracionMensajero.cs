using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiConfiguracionMensajero : ApiDominioBase
    {
        private static readonly ApiConfiguracionMensajero instancia = (ApiConfiguracionMensajero)FabricaInterceptorApi.
            GetProxy(new ApiConfiguracionMensajero(), COConstantesModulos.MENSAJERO);

        public static ApiConfiguracionMensajero Instancia
        {
            get { return ApiConfiguracionMensajero.instancia; }
        }

        public List<MECargo> ObtenerCargos()
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerCargos();
        }

        public List<METipoContrato> ObtenerTiposContrato()
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerTiposContrato();
        }

        public List<METipoMensajero> ObtenerTipoMensajero()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTipoMensajero();
        }

        public List<MEEstadoMensajero> ObtenerEstadosMensajero()
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerEstadosMensajero();
        }

        public List<MEGruposLiquidacion> ObtenerGruposliquidacion(int pagina, int nRegistros, bool estado)
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerGruposliquidacion(pagina, nRegistros, estado);
        }

        public List<MEGruposLiquidacion> ObtenerGruposliquidacionUnico()
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerGruposliquidacionUnico();
        }

        public List<MEGrupoBasico> ObtenerGruposBasicos(int pagina, int nRegistros, bool estado)
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerGruposBasicos(pagina, nRegistros, estado);
        }

        public List<MEGrupoRodamiento> ObtenerGruposRodamiento(string idCiudad, int pagina, int nRegistros, bool estado)
        {
            return FabricaServicios.ServicioConfiguracionMensajero.ObtenerGruposRodamiento(idCiudad, pagina, nRegistros, estado);
        }
        public List<MEBasicoLiquidacion> ObtenerLiquidacionBasico(int idBasico)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerLiquidacionBasico(idBasico);
        }

        public List<METipoTransporte> ObtenerTiposTransporte()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposTransporte();
        }

        public bool InsertarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarGrupoRodamiento(GrupoRodamiento);
        }

        public bool ModificarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ModificarGrupoRodamiento(GrupoRodamiento);
        }

        public List<MEBasicoLiquidacion> InsertarBasico(MEGrupoBasico GrupoBasico)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarBasico(GrupoBasico);
        }
        public List<MEBasicoLiquidacion> ModificarBasico(MEGrupoBasico GrupoBasico)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ModificarBasico(GrupoBasico);
        }

        public List<METipoAccion> ObtenerTiposAccion()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposAccion();
        }

        public List<METipoPenalidad> ObtenerTiposPenalidad(int pagina, int nRegistros, bool estado)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposPenalidad(pagina, nRegistros, estado);
        }

        public List<METipoPenalidad> ObtenerTiposPenalidadesRaps()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposPenalidadesRaps();
        }

        public List<METipoUsuario> ObtenerTiposUsuarios()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposUsuarios();
        }

        public List<METipoRodamiento> ObtenerTiposRodamientoXGrupo(int idGrupoRodamiento)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposRodamientoXGrupo(idGrupoRodamiento);
        }

        public List<METiposLiquidacion> ObtenerTiposLiquidacion(int idGrupoLiquidacion, int pagina, int nRegistros)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTiposLiquidacion(idGrupoLiquidacion, pagina, nRegistros);
        }

        public bool InsertarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarGrupoLiquidacion(GrupoLiquidacion);
        }

        public bool InsertarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarTipoLiquidacion(TipoLiquidacion);
        }

        public bool ModificarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ModificarGrupoLiquidacion(GrupoLiquidacion);
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocioFormaPago(string idTipoAccion, int pagina, int nRegistros)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerUnidadesNegocioFormaPago(idTipoAccion, pagina, nRegistros);
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocio()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerUnidadesNegocio();
        }
        public List<MEUnidadNegocioFormaPago> ObtenerFormaPago(int IdTipoAccion)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerFormaPago(IdTipoAccion);
        }
        public bool ModificarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ModificarTipoLiquidacion(TipoLiquidacion);
        }

        public List<METipoCuenta> ObtenerTipoCuenta()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTipoCuenta();
        }

        public bool InsertarPenalidad(METipoPenalidad TipoPenalidad)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarPenalidad(TipoPenalidad);
        }

        public bool ModificarPenalidad(METipoPenalidad TipoPenalidad)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ModificarPenalidad(TipoPenalidad);
        }



        public List<POVehiculo> ConsultaVehiculosNovasoft()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ConsultaVehiculosNovasoft();
        }

        public List<POVehiculo> ConsultaVehiculosNovasoftXDocumento(string idDocumento)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ConsultaVehiculosNovasoftXDocumento(idDocumento);
        }

        public List<METipoPAM> ObtenerTipoPAM()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerTipoPAM();
        }
        public bool InsertarModificarLiquidacionPersonaInterna(long idPersona, string valor, string tipo)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarModificarLiquidacionPersonaInterna(idPersona, valor, tipo);
        }

        public bool InsertarModificarLiquidacionPersonaExterna(long idPersona, string valor, string tipo)
        {
            return FabricaServicios.servicioConfiguracionMensajero.InsertarModificarLiquidacionPersonaExterna(idPersona, valor, tipo);
        }

        public double ConsultarIPC(DateTime fecha)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ConsultarIPC(fecha);
        }

        public double ConsultarActivo(string tabla, string campo, DateTime fechaInicio)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ConsultarActivo(tabla, campo, fechaInicio);
        }

        public MEConfigComisiones ConsultarConfiguracionComisionesEmpleado(int idPersona, string tabla, string campo)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ConsultarConfiguracionComisionesEmpleado(idPersona, tabla, campo);
        }

        public bool ModificarComisionesIPC(double IPC)
        {
            return FabricaServicios.servicioConfiguracionMensajero.ModificarComisionesIPC(IPC);
        }

        public List<MEMensajero> ObtenerPersonasConfig()
        {
            return FabricaServicios.servicioConfiguracionMensajero.ObtenerPersonasConfig();
        }
    }
}
