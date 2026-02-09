using CO.Servidor.Mensajero;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.Contratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.Implementacion.Mensajero
{
    public class MeConfiguracionMensajeroSvc
    //: IMEConfiguracionMensajeroSvc
    {
        public MeConfiguracionMensajeroSvc()
        { }

        public List<MECargo> ObtenerCargos()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerCargos();
        }

        public List<METipoContrato> ObtenerTiposContrato()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposContrato();
        }

        public List<METipoMensajero> ObtenerTipoMensajero()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTipoMensajero();
        }

        public List<MEEstadoMensajero> ObtenerEstadosMensajero()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerEstadosMensajero();
        }

        public List<MEGruposLiquidacion> ObtenerGruposliquidacion(int pagina, int nRegistros, bool estado)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerGruposliquidacion(pagina, nRegistros, estado);
        }

        public List<MEGruposLiquidacion> ObtenerGruposliquidacionUnico()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerGruposliquidacionUnico();
        }


        public List<MEGrupoBasico> ObtenerGruposBasicos(int pagina, int nRegistros, bool estado)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerGruposBasicos(pagina, nRegistros, estado);
        }

        public List<MEGrupoRodamiento> ObtenerGruposRodamiento(string idCiudad, int pagina, int nRegistros, bool estado)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerGruposRodamiento(idCiudad, pagina, nRegistros, estado);
        }
        public List<MEBasicoLiquidacion> ObtenerLiquidacionBasico(int idBasico)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerLiquidacionBasico(idBasico);
        }

        public List<METipoTransporte> ObtenerTiposTransporte()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposTransporte();
        }

        public bool InsertarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            return MeConfiguracionMensajero.Instancia.InsertarGrupoRodamiento(GrupoRodamiento);
        }

        public bool ModificarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            return MeConfiguracionMensajero.Instancia.ModificarGrupoRodamiento(GrupoRodamiento);
        }

        public List<MEBasicoLiquidacion> InsertarBasico(MEGrupoBasico GrupoBasico)
        {
            return MeConfiguracionMensajero.Instancia.InsertarBasico(GrupoBasico);
        }
        public List<MEBasicoLiquidacion> ModificarBasico(MEGrupoBasico GrupoBasico)
        {
            return MeConfiguracionMensajero.Instancia.ModificarBasico(GrupoBasico);
        }

        public List<METipoAccion> ObtenerTiposAccion()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposAccion();
        }

        public List<METipoPenalidad> ObtenerTiposPenalidad(int pagina, int nRegistros, bool estado)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposPenalidad(pagina, nRegistros, estado);
        }

        public List<METipoPenalidad> ObtenerTiposPenalidadesRaps()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposPenalidadesRaps();
        }

        public List<METipoUsuario> ObtenerTiposUsuarios()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposUsuarios();
        }

        public List<METipoRodamiento> ObtenerTiposRodamientoXGrupo(int idGrupoRodamiento)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposRodamientoXGrupo(idGrupoRodamiento);
        }
        public List<METiposLiquidacion> ObtenerTiposLiquidacion(int idGrupoLiquidacion, int pagina, int nRegistros)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTiposLiquidacion(idGrupoLiquidacion, pagina, nRegistros);
        }

        public bool InsertarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {
            return MeConfiguracionMensajero.Instancia.InsertarGrupoLiquidacion(GrupoLiquidacion);
        }

        public bool InsertarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            return MeConfiguracionMensajero.Instancia.InsertarTipoLiquidacion(TipoLiquidacion);
        }


        public bool ModificarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {
            return MeConfiguracionMensajero.Instancia.ModificarGrupoLiquidacion(GrupoLiquidacion);
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocioFormaPago(string idTipoAccion, int pagina, int nRegistros)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerUnidadesNegocioFormaPago(idTipoAccion, pagina, nRegistros);
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocio()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerUnidadesNegocio();
        }
        public List<MEUnidadNegocioFormaPago> ObtenerFormaPago(int IdTipoAccion)
        {
            return MeConfiguracionMensajero.Instancia.ObtenerFormaPago(IdTipoAccion);
        }

        public bool ModificarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            return MeConfiguracionMensajero.Instancia.ModificarTipoLiquidacion(TipoLiquidacion);
        }

        public List<METipoCuenta> ObtenerTipoCuenta()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTipoCuenta();
        }

        public bool InsertarPenalidad(METipoPenalidad TipoPenalidad)
        {
            return MeConfiguracionMensajero.Instancia.InsertarPenalidad(TipoPenalidad);
        }

        public bool ModificarPenalidad(METipoPenalidad TipoPenalidad)
        {
            return MeConfiguracionMensajero.Instancia.ModificarPenalidad(TipoPenalidad);
        }

        public List<POVehiculo> ConsultaVehiculosNovasoft()
        {
            return MeConfiguracionMensajero.Instancia.ConsultaVehiculosNovasoft();
        }

        public List<POVehiculo> ConsultaVehiculosNovasoftXDocumento(string idDocumento)
        {
            return MeConfiguracionMensajero.Instancia.ConsultaVehiculosNovasoftXDocumento(idDocumento);
        }

        public List<METipoPAM> ObtenerTipoPAM()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerTipoPAM();
        }

        public bool InsertarModificarLiquidacionPersonaInterna(long idPersona, string valor, string tipo)
        {
            return MeConfiguracionMensajero.Instancia.InsertarModificarLiquidacionPersonaInterna(idPersona, valor, tipo);
        }

        public bool InsertarModificarLiquidacionPersonaExterna(long idPersona, string valor, string tipo)
        {
            return MeConfiguracionMensajero.Instancia.InsertarModificarLiquidacionPersonaExterna(idPersona, valor, tipo);
        }

        public double ConsultarIPC(DateTime fecha)
        {
            return MeConfiguracionMensajero.Instancia.ConsultarIPC(fecha);
        }

        public double ConsultarActivo(string tabla, string campo, DateTime fechaInicio)
        {
            return MeConfiguracionMensajero.Instancia.ConsultarActivo(tabla, campo, fechaInicio);
        }

        public MEConfigComisiones ConsultarConfiguracionComisionesEmpleado(int idPersona, string tabla, string campo)
        {
            return MeConfiguracionMensajero.Instancia.ConsultarConfiguracionComisionesEmpleado(idPersona, tabla, campo);
        }
        public bool ModificarComisionesIPC(double IPC)
        {
            return MeConfiguracionMensajero.Instancia.ModificarComisionesIPC(IPC);
        }

        public List<MEMensajero> ObtenerPersonasConfig()
        {
            return MeConfiguracionMensajero.Instancia.ObtenerPersonasConfig();
        }
    }
}
