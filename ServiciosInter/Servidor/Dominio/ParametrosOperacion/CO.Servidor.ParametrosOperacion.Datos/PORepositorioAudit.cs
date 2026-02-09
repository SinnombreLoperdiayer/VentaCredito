using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.ParametrosOperacion.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.ParametrosOperacion.Datos
{
    internal class PORepositorioAudit
    {
        /// <summary>
        /// Metodo de auditoria de propietario vehiculo
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarPropietarioVehiculo(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<PropietarioVehiculo_CPO, PropietarioVehiculoHist_CPO>((record, action) => new PropietarioVehiculoHist_CPO()
            {
                PRV_IdPropietarioVehiculo = record.Field<PropietarioVehiculo_CPO, long>(f => f.PRV_IdPropietarioVehiculo),
                PRV_IdTipoContrato = record.Field<PropietarioVehiculo_CPO, short>(f => f.PRV_IdTipoContrato),
                PRV_FechaGrabacion = record.Field<PropietarioVehiculo_CPO, DateTime>(f => f.PRV_FechaGrabacion),
                PRV_CreadoPor = record.Field<PropietarioVehiculo_CPO, string>(f => f.PRV_CreadoPor),
                PRV_FechaCambio = DateTime.Now,
                PRV_CambiadoPor = ControllerContext.Current.Usuario,
                PRV_TipoCambio = action.ToString(),
            }, (ph) => contexto.PropietarioVehiculoHist_CPO.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de propietario vehiculo vehiculo
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarPropietarioVehiculoVehiculo(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<PropietarioVehiculoVehiculo_CPO, PropietarioVehiVehiHist_CPO>((record, action) => new PropietarioVehiVehiHist_CPO()
            {
                PVV_IdPropietarioVehiculo = record.Field<PropietarioVehiculoVehiculo_CPO, long>(f => f.PVV_IdPropietarioVehiculo),
                PVV_IdVehiculo = record.Field<PropietarioVehiculoVehiculo_CPO, int>(f => f.PVV_IdVehiculo),
                PVV_FechaGrabacion = record.Field<PropietarioVehiculoVehiculo_CPO, DateTime>(f => f.PVV_FechaGrabacion),
                PVV_CreadoPor = record.Field<PropietarioVehiculoVehiculo_CPO, string>(f => f.PVV_CreadoPor),
                PVV_FechaCambio = DateTime.Now,
                PVV_CambiadoPor = ControllerContext.Current.Usuario,
                PVV_TipoCambio = action.ToString(),
            }, (ph) => contexto.PropietarioVehiVehiHist_CPO.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de persona externa
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarPersonaExterna(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<PersonaExterna_PAR, PersonaExternaHist_PAR>((record, action) => new PersonaExternaHist_PAR()
            {
                PEE_DigitoVerificacion = record.Field<PersonaExterna_PAR, string>(f => f.PEE_DigitoVerificacion),
                PEE_Direccion = record.Field<PersonaExterna_PAR, string>(f => f.PEE_Direccion),
                PEE_FechaExpedicionDocumento = record.Field<PersonaExterna_PAR, DateTime>(f => f.PEE_FechaExpedicionDocumento),
                PEE_Identificacion = record.Field<PersonaExterna_PAR, string>(f => f.PEE_Identificacion),
                PEE_IdPersonaExterna = record.Field<PersonaExterna_PAR, long>(f => f.PEE_IdPersonaExterna),
                PEE_IdTipoIdentificacion = record.Field<PersonaExterna_PAR, string>(f => f.PEE_IdTipoIdentificacion),
                PEE_Municipio = record.Field<PersonaExterna_PAR, string>(f => f.PEE_Municipio),
                PEE_NumeroCelular = record.Field<PersonaExterna_PAR, string>(f => f.PEE_NumeroCelular),
                PEE_PrimerApellido = record.Field<PersonaExterna_PAR, string>(f => f.PEE_PrimerApellido),
                PEE_PrimerNombre = record.Field<PersonaExterna_PAR, string>(f => f.PEE_PrimerNombre),
                PEE_SegundoApellido = record.Field<PersonaExterna_PAR, string>(f => f.PEE_SegundoApellido),
                PEE_SegundoNombre = record.Field<PersonaExterna_PAR, string>(f => f.PEE_SegundoNombre),
                PEE_Telefono = record.Field<PersonaExterna_PAR, string>(f => f.PEE_Telefono),
                PEE_FechaGrabacion = record.Field<PersonaExterna_PAR, DateTime>(f => f.PEE_FechaGrabacion),
                PEE_CreadoPor = record.Field<PersonaExterna_PAR, string>(f => f.PEE_CreadoPor),
                PEE_FechaCambio = DateTime.Now,
                PEE_CambiadoPor = ControllerContext.Current.Usuario,
                PEE_TipoCambio = action.ToString(),
            }, (ph) => contexto.PersonaExternaHist_PAR.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de tenedor vehiculo vehiculo
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarTenedorVehiculoVehiculo(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<TenedorVehiculoVehiculo_CPO, TenedorVehiculoVehiculHist_CPO>((record, action) => new TenedorVehiculoVehiculHist_CPO()
            {
                TVV_IdTenedorVehiculo = record.Field<TenedorVehiculoVehiculo_CPO, long>(f => f.TVV_IdTenedorVehiculo),
                TVV_IdVehiculo = record.Field<TenedorVehiculoVehiculo_CPO, int>(f => f.TVV_IdVehiculo),
                TVV_FechaGrabacion = record.Field<TenedorVehiculoVehiculo_CPO, DateTime>(f => f.TVV_FechaGrabacion),
                TVV_CreadoPor = record.Field<TenedorVehiculoVehiculo_CPO, string>(f => f.TVV_CreadoPor),
                TVV_FechaCambio = DateTime.Now,
                TVV_CambiadoPor = ControllerContext.Current.Usuario,
                TVV_TipoCambio = action.ToString(),
            }, (ph) => contexto.TenedorVehiculoVehiculHist_CPO.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de tenedor vehiculo
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarTenedorVehiculo(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<TenedorVehiculo_CPO, TenedorVehiculoHist_CPO>((record, action) => new TenedorVehiculoHist_CPO()
            {
                TEV_IdCategoriaLicencia = record.Field<TenedorVehiculo_CPO, short>(f => f.TEV_IdCategoriaLicencia),
                TEV_NumeroLicencia = record.Field<TenedorVehiculo_CPO, string>(f => f.TEV_NumeroLicencia),
                TEV_NumeroCelular2 = record.Field<TenedorVehiculo_CPO, string>(f => f.TEV_NumeroCelular2),
                TEV_FechaGrabacion = record.Field<TenedorVehiculo_CPO, DateTime>(f => f.TEV_FechaGrabacion),
                TEV_CreadoPor = record.Field<TenedorVehiculo_CPO, string>(f => f.TEV_CreadoPor),
                TEV_FechaCambio = DateTime.Now,
                TEV_CambiadoPor = ControllerContext.Current.Usuario,
                TEV_TipoCambio = action.ToString(),
            }, (ph) => contexto.TenedorVehiculoHist_CPO.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de un mensajero
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarMensajero(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<Mensajero_CPO, MensajeroHist_CPO>((record, action) => new MensajeroHist_CPO()
            {
                MEN_EsContratista = record.Field<Mensajero_CPO, bool>(f => f.MEN_EsContratista),
                MEN_Estado = record.Field<Mensajero_CPO, string>(f => f.MEN_Estado),
                MEN_FechaIngreso = record.Field<Mensajero_CPO, DateTime>(f => f.MEN_FechaIngreso),
                MEN_FechaTerminacionContrato = record.Field<Mensajero_CPO, DateTime>(f => f.MEN_FechaTerminacionContrato),
                MEN_FechaVencimientoPase = record.Field<Mensajero_CPO, DateTime>(f => f.MEN_FechaVencimientoPase),
                MEN_IdAgencia = record.Field<Mensajero_CPO, long>(f => f.MEN_IdAgencia),
                MEN_IdMensajero = record.Field<Mensajero_CPO, long>(f => f.MEN_IdMensajero),
                MEN_IdPersonaInterna = record.Field<Mensajero_CPO, long>(f => f.MEN_IdPersonaInterna),
                MEN_IdTipoMensajero = record.Field<Mensajero_CPO, short>(f => f.MEN_IdTipoMensajero),
                MEN_NumeroPase = record.Field<Mensajero_CPO, string>(f => f.MEN_NumeroPase),
                MEN_Telefono2 = record.Field<Mensajero_CPO, string>(f => f.MEN_Telefono2),
                MEN_TipoContrato = record.Field<Mensajero_CPO, short>(f => f.MEN_TipoContrato),
                MEN_FechaGrabacion = record.Field<Mensajero_CPO, DateTime>(f => f.MEN_FechaGrabacion),
                MEN_CreadoPor = record.Field<Mensajero_CPO, string>(f => f.MEN_CreadoPor),
                MEN_FechaCambio = DateTime.Now,
                MEN_CambiadoPor = ControllerContext.Current.Usuario,
                MEN_TipoCambio = action.ToString(),
                MEN_EsMensajeroUrbano = record.Field<Mensajero_CPO, bool>(f => f.MEN_EsMensajeroUrbano),
            }, (ph) => contexto.MensajeroHist_CPO.Add(ph));
        }

        /// <summary>
        /// Metodo de auditoria de un conductor
        /// </summary>
        /// <param name="cliente"></param>
        internal static void MapearAuditModificarConductor(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<Conductor_CPO, ConductorHist_CPO>((record, action) => new ConductorHist_CPO()
            {
                CON_EsContratista = record.Field<Conductor_CPO, bool>(f => f.CON_EsContratista),
                CON_Estado = record.Field<Conductor_CPO, string>(f => f.CON_Estado),
                CON_FechaIngreso = record.Field<Conductor_CPO, DateTime>(f => f.CON_FechaIngreso),
                CON_FechaTerminacionContrato = record.Field<Conductor_CPO, DateTime>(f => f.CON_FechaTerminacionContrato),
                CON_FechaVencimientoPase = record.Field<Conductor_CPO, DateTime>(f => f.CON_FechaVencimientoPase),
                CON_IdConductor = record.Field<Conductor_CPO, long>(f => f.CON_IdConductor),
                CON_IdPersonaInterna = record.Field<Conductor_CPO, long>(f => f.CON_IdPersonaInterna),
                CON_Telefono2 = record.Field<Conductor_CPO, string>(f => f.CON_Telefono2),
                CON_TipoContrato = record.Field<Conductor_CPO, string>(f => f.CON_TipoContrato),
                CON_NumeroPase = record.Field<Conductor_CPO, string>(f => f.CON_NumeroPase),
                CON_FechaGrabacion = record.Field<Conductor_CPO, DateTime>(f => f.CON_FechaGrabacion),
                CON_CreadoPor = record.Field<Conductor_CPO, string>(f => f.CON_CreadoPor),
                CON_FechaCambio = DateTime.Now,
                CON_CambiadoPor = ControllerContext.Current.Usuario,
                CON_TipoCambio = action.ToString(),
            }, (ph) => contexto.ConductorHist_CPO.Add(ph));
        }

        /// <summary>
        /// Audita conductor RegionalAdministrativa
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditConductorRegionalAdmin(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<ConductorRegionalAdmi_CPO, ConductorRegionalAdmiHist_CPO>((record, action) => new ConductorRegionalAdmiHist_CPO()
            {
                CRA_CreadoPor = record.Field<ConductorRegionalAdmi_CPO, string>(c => c.CRA_CreadoPor),
                CRA_FechaCambio = DateTime.Now,
                CRA_FechaGrabacion = record.Field<ConductorRegionalAdmi_CPO, DateTime>(c => c.CRA_FechaGrabacion),
                CRA_CambiadoPor = ControllerContext.Current.Usuario,
                CRA_TipoCambio = action.ToString(),
                CRA_IdConductor = record.Field<ConductorRegionalAdmi_CPO, long>(c => c.CRA_IdConductor),
                CRA_IdConductorRegionalAdmi = record.Field<ConductorRegionalAdmi_CPO, int>(c => c.CRA_IdConductorRegionalAdmi),
                CRA_IdRegionalAdm = record.Field<ConductorRegionalAdmi_CPO, long>(c => c.CRA_IdRegionalAdm)
            }, (cs) => contexto.ConductorRegionalAdmiHist_CPO.Add(cs));
        }

        /// <summary>
        /// Audita persona interna
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditPersonaInterna(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<PersonaInterna_PAR, PersonaInternaHistorico_PAR>((record, action) => new PersonaInternaHistorico_PAR()
            {
                PEI_CreadoPor = record.Field<PersonaInterna_PAR, string>(c => c.PEI_CreadoPor),
                PEI_FechaCambio = DateTime.Now,
                PEI_FechaGrabacion = record.Field<PersonaInterna_PAR, DateTime>(c => c.PEI_FechaGrabacion),
                PEI_CambiadoPor = ControllerContext.Current.Usuario,
                PEI_TipoCambio = action.ToString(),
                PEI_Comentarios = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Comentarios),
                PEI_Direccion = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Direccion),
                PEI_Email = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Email),
                PEI_IdCargo = record.Field<PersonaInterna_PAR, int>(c => c.PEI_IdCargo),
                PEI_Identificacion = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Identificacion),
                PEI_IdPersonaInterna = record.Field<PersonaInterna_PAR, long>(c => c.PEI_IdPersonaInterna),
                PEI_IdRegionalAdm = record.Field<PersonaInterna_PAR, long?>(c => c.PEI_IdRegionalAdm),
                PEI_IdTipoIdentificacion = record.Field<PersonaInterna_PAR, string>(c => c.PEI_IdTipoIdentificacion),
                PEI_Municipio = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Municipio),
                PEI_Nombre = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Nombre),
                PEI_PrimerApellido = record.Field<PersonaInterna_PAR, string>(c => c.PEI_PrimerApellido),
                PEI_SegundoApellido = record.Field<PersonaInterna_PAR, string>(c => c.PEI_SegundoApellido),
                PEI_Telefono = record.Field<PersonaInterna_PAR, string>(c => c.PEI_Telefono),
            }, (cs) => contexto.PersonaInternaHistorico_PAR.Add(cs));
        }

        /// <summary>
        /// Audita seguro (soat o todoriesgo )de un vehiculo
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditSeguro(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<PolizaSeguro_CPO, PolizaSeguroHist_CPO>((record, action) => new PolizaSeguroHist_CPO()
            {
                POS_CreadoPor = record.Field<PolizaSeguro_CPO, string>(c => c.POS_CreadoPor),
                POS_FechaCambio = DateTime.Now,
                POS_FechaGrabacion = record.Field<PolizaSeguro_CPO, DateTime>(c => c.POS_FechaGrabacion),
                POS_CambiadoPor = ControllerContext.Current.Usuario,
                POS_TipoCambio = action.ToString(),
                POS_Cobertura = record.Field<PolizaSeguro_CPO, decimal>(c => c.POS_Cobertura),
                POS_FechaExpedicion = record.Field<PolizaSeguro_CPO, DateTime>(c => c.POS_FechaExpedicion),
                POS_FechaVencimiento = record.Field<PolizaSeguro_CPO, DateTime>(c => c.POS_FechaVencimiento),
                POS_IdAseguradora = record.Field<PolizaSeguro_CPO, short>(c => c.POS_IdAseguradora),
                POS_IdPolizaSeguro = record.Field<PolizaSeguro_CPO, short>(c => c.POS_IdPolizaSeguro),
                POS_IdTipoPolizaSerguro = record.Field<PolizaSeguro_CPO, string>(c => c.POS_IdTipoPolizaSerguro),
                POS_NumeroPoliza = record.Field<PolizaSeguro_CPO, string>(c => c.POS_NumeroPoliza)
            }, (cs) => contexto.PolizaSeguroHist_CPO.Add(cs));
        }

        /// <summary>
        /// Audita la revision mecanica de un vehiculo
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditRevisionMecanica(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<RevisionMecanica_CPO, RevisionMecanicaHist_CPO>((record, action) => new RevisionMecanicaHist_CPO()
            {
                REM_CreadoPor = record.Field<RevisionMecanica_CPO, string>(c => c.REM_CreadoPor),
                REM_FechaCambio = DateTime.Now,
                REM_FechaGrabacion = record.Field<RevisionMecanica_CPO, DateTime>(c => c.REM_FechaGrabacion),
                REM_CambiadoPor = ControllerContext.Current.Usuario,
                REM_TipoCambio = action.ToString(),
                REM_FechaExpedicion = record.Field<RevisionMecanica_CPO, DateTime>(c => c.REM_FechaExpedicion),
                REM_FechaVencimiento = record.Field<RevisionMecanica_CPO, DateTime>(c => c.REM_FechaVencimiento),
                REM_IdRevisionMecanica = record.Field<RevisionMecanica_CPO, short>(c => c.REM_IdRevisionMecanica),
                REM_IdVehiculo = record.Field<RevisionMecanica_CPO, int>(c => c.REM_IdVehiculo)
            }, (cs) => contexto.RevisionMecanicaHist_CPO.Add(cs));
        }

        /// <summary>
        /// Audita los conductores asociados a un vehiculo
        /// </summary>
        /// <param name="contexto"></param>
        internal static void MapearAuditConductorVehiculo(ModeloParametrosOperacion contexto)
        {
            contexto.Audit<ConductorVehiculo_CPO, ConductorVehiculoHist_CPO>((record, action) => new ConductorVehiculoHist_CPO()
            {
                COV_CreadoPor = record.Field<ConductorVehiculo_CPO, string>(c => c.COV_CreadoPor),
                COV_FechaCambio = DateTime.Now,
                COV_FechaGrabacion = record.Field<ConductorVehiculo_CPO, DateTime>(c => c.COV_FechaGrabacion),
                COV_CambiadoPor = ControllerContext.Current.Usuario,
                COV_TipoCambio = action.ToString(),
                COV_IdConductor = record.Field<ConductorVehiculo_CPO, long>(c => c.COV_IdConductor),
                COV_IdVehiculo = record.Field<ConductorVehiculo_CPO, int>(c => c.COV_IdVehiculo)
            }, (cs) => contexto.ConductorVehiculoHist_CPO.Add(cs));
        }
    }
}