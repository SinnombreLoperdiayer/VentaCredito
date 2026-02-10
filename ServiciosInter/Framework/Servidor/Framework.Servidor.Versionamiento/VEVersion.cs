using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;
using Framework.Servidor.Versionamiento.Modelo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;

namespace Framework.Servidor.Versionamiento
{
    /// <summary>
    /// Clase para manejo de versionamiento
    /// </summary>
    public class VEVersion : ControllerBase
    {
        #region Instancia Singleton de la Clase

        private const string nombreModelo = "ModeloVersion";
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Instancia de VEVersion
        /// </summary>
        private static readonly VEVersion instancia = (VEVersion)FabricaInterceptores.GetProxy(new VEVersion(), ConstantesFramework.MODULO_FW_VERSIONAMIENTO);

        /// <summary>
        /// Instancia de la clase
        /// </summary>
        public static VEVersion Instancia
        {
            get { return VEVersion.instancia; }
        }

        #endregion Instancia Singleton de la Clase

        #region Version

        /// <summary>
        ///  Calcula la versión asociadaa un id de máquina
        /// </summary>
        /// <param name="idMaquina">identificador de la máquina que se está autenticando</param>
        /// <returns></returns>
        public VEVersionInfo CalcularVersionxIdMaquina(string idMaquina, string token, VEDatosIngresoUsuario datosIngreso)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                // TODO: ID, se Ajusta la idendificacion de MAQUINAS, Se Validara Usuario y MaquinaId
                Modelo.MaqVersion_VER maquina = entidades.MaqVersion_VER.Include("Version_VER").Include("Version_VER.VersionServicios_VER").Include("Version_VER.VersionServicios_VER.Modulo_VER").FirstOrDefault(v => v.MAV_MaquinaId == idMaquina && v.MAV_CreadoPor == datosIngreso.Usuario);
                if (maquina != null)
                {
                    if (maquina.MAV_Estado == ConstantesFramework.ESTADO_ACTIVO)
                    {
                        // Se obtiene la información del usuario que intenta ingresar a la aplicación
                        Framework.Servidor.Versionamiento.Modelo.UsuarioPersonaInterna_VSEG _usuario = entidades.UsuarioPersonaInterna_VSEG.First(u => u.USU_IdUsuario == datosIngreso.Usuario && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO);

                        VEVersionInfo infoVersion = new VEVersionInfo()
                        {
                            VersionComercial = maquina.MAV_IdVersion,
                            Binding = maquina.Version_VER.VER_BindingServicios,
                            Modulos = maquina.Version_VER.VersionServicios_VER.ToList()
                                                 .ConvertAll<VEModulo>(
                                                     version => new VEModulo()
                                                     {
                                                         Descripcion = version.Modulo_VER.MOD_Descripcion,
                                                         IdModulo = version.VES_IdModulo,
                                                         UrlAplicacion = version.VES_UriAplicacion,
                                                         UrlServicio = version.VES_UrlServicio,
                                                         TipoProxy = version.VES_TipoProxy
                                                     }),
                            MenusCapacitacion = entidades.MenuCapacitacion_VER.ToList()
                            .ConvertAll<VEMenuCapacitacion>(
                                                     menuCapacitacion => new VEMenuCapacitacion()
                                                     {
                                                         Id = menuCapacitacion.MEC_IdMenu,
                                                         Descripcion = menuCapacitacion.MEC_Descripcion,
                                                         Activo = menuCapacitacion.MEC_Activo,
                                                         IdProceso = (VEEnumProcesos)(Enum.Parse(typeof(VEEnumProcesos), menuCapacitacion.MEC_IdProceso.ToString())),
                                                         Target = menuCapacitacion.MEC_Target,
                                                         IdAncestro = menuCapacitacion.MEC_IdAncestro,
                                                         URL = menuCapacitacion.MEC_URL
                                                     }),
                            Cultura = entidades.ParametrosFramework.Where(p => p.PAR_IdParametro == "Cultura").First().PAR_ValorParametro,
                        };

                        infoVersion.DatosInicioSesion = new VEDatosInicioSesion();
                        infoVersion.DatosInicioSesion.EsCajeroPpal = _usuario.USU_EsCajeroPpal;
                        infoVersion.DatosInicioSesion.Cargo = new Servicios.ContratoDatos.Agenda.ASCargo()
                        {
                            IdCargo = _usuario.PEI_IdCargo,
                            Descripcion = _usuario.CAR_Descripcion
                        };

                        if (datosIngreso.LocacionIngreso.TipoLocacion == TipoLocacionAutorizada.CentroServicios)
                        {
                            VEDatosCentroServicio datosCentroServicio = ConsultarDatosCentroServicio(long.Parse(datosIngreso.LocacionIngreso.IdLocacion));

                            infoVersion.DatosInicioSesion.DatosCentroServicio = datosCentroServicio;
                            infoVersion.DatosInicioSesion.DatosCentroServicio.Caja = datosIngreso.LocacionIngreso.IdCaja;
                            infoVersion.DatosInicioSesion.IdCasaMatriz = datosCentroServicio.IdCasaMAtriz;
                        }
                        else if (datosIngreso.LocacionIngreso.TipoLocacion == TipoLocacionAutorizada.Sucursal)
                        {
                            int idSucursal = int.Parse(datosIngreso.LocacionIngreso.IdLocacion);
                            infoVersion.DatosInicioSesion.DatosClienteCredito = ConsultarDatosSucursalCliente(idSucursal);
                        }
                        else if (datosIngreso.LocacionIngreso.TipoLocacion == TipoLocacionAutorizada.Gestion)
                        {
                            long idGestion = long.Parse(datosIngreso.LocacionIngreso.IdLocacion);
                            Gestion_ARE gestion = entidades.Gestion_ARE.Include("MacroProceso_ARE").Where(g => g.GES_CodigoGestion == idGestion).FirstOrDefault();
                            if (gestion != null)
                            {
                                infoVersion.DatosInicioSesion.CodigoGestion = gestion.GES_CodigoGestion;
                                infoVersion.DatosInicioSesion.NombreGestion = gestion.MacroProceso_ARE.MAP_Descripcion + " - " + gestion.GES_Descripcion;
                                infoVersion.DatosInicioSesion.IdCasaMatriz = gestion.MacroProceso_ARE.MAP_IdCasaMatriz;
                            }
                        };

                        infoVersion.DatosInicioSesion.DocumentoUsuario = _usuario.PEI_Identificacion;
                        infoVersion.DatosInicioSesion.IdPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "PaisPredeterminado").PAR_ValorParametro;
                        infoVersion.DatosInicioSesion.DescPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "DesPaisPredeterminad").PAR_ValorParametro;
                        infoVersion.DatosInicioSesion.NombresUsuario = _usuario.PEI_Nombre;
                        infoVersion.DatosInicioSesion.IdUsuario = _usuario.USU_IdCodigoUsuario;
                        infoVersion.DatosInicioSesion.ApellidosUsuario = _usuario.PEI_PrimerApellido + " " + _usuario.PEI_SegundoApellido;
                        infoVersion.DatosInicioSesion.FechaServidor = System.DateTime.Now;
                        infoVersion.DatosInicioSesion.URLKompadreStereo = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "URLKompadreStereo").PAR_ValorParametro;

                        // TODO ID: Se quita este Registro que se graba en BD, con el siguiente metodo es suficiente para tener el Historico de Ingreso de los usuarios
                        //RegistrarToken(token, idMaquina, datosIngreso.Usuario);

                        RegistrarIngresoDeUsuario(datosIngreso);
                        if (infoVersion.DatosInicioSesion.DatosCentroServicio != null)
                        {
                            ControllerContext.Current.IdCaja = infoVersion.DatosInicioSesion.DatosCentroServicio.Caja;
                            ControllerContext.Current.IdCentroServicio = infoVersion.DatosInicioSesion.DatosCentroServicio.IdCentroServicio;
                        }
                        else
                        {
                            //todo w  que centro servicio tiene cuando entra como gestion se po ne default racol bogota
                            ControllerContext.Current.IdCaja = 0;
                            ControllerContext.Current.IdCentroServicio = 1;
                        }
                        Framework.Servidor.Excepciones.Session.SESesionUsuario.Instancia.ActualizaSesion();

                        return infoVersion;
                    }
                    else
                    {
                        ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                                             ETipoErrorFramework.EX_IDMAQUINA_NOVALIDO.ToString(),
                                             MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_IDMAQUINA_NOVALIDO));
                        throw new FaultException<ControllerException>(exc);
                    }
                }
                else
                {
                    SolicitarAltaMaquina(idMaquina, datosIngreso.Usuario);
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                                           ETipoErrorFramework.IN_MAQUINA_REGISTRADA.ToString(),
                                           MensajesFramework.CargarMensaje(ETipoErrorFramework.IN_MAQUINA_REGISTRADA));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        /// <summary>
        /// Calcula la versión de un Racol
        /// </summary>
        /// <param name="idRacol">Id del Racol</param>
        public VEVersionInfo CalcularVersionxCentroServicios(long idCentroServicios, VEDatosIngresoUsuario datosIngreso)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                VEDatosCentroServicio datosCentroServicio = ConsultarDatosCentroServicio(idCentroServicios);

                Modelo.VersionRegionalAdmin_VER regional = entidades.VersionRegionalAdmin_VER.Include("Version_VER.VersionServicios_VER").Include("Version_VER.VersionServicios_VER.Modulo_VER").FirstOrDefault(v => v.VRA_IdRegionalAdm == datosCentroServicio.IdRacol);

                if (regional != null)
                {
                    // Se obtiene la información del usuario que intenta ingresar a la aplicación
                    Framework.Servidor.Versionamiento.Modelo.UsuarioPersonaInterna_VSEG _usuario = entidades.UsuarioPersonaInterna_VSEG.First(u => u.USU_IdUsuario == datosIngreso.Usuario && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO);

                    VEVersionInfo infoVersion = new VEVersionInfo()
                    {
                        VersionComercial = regional.VRA_IdVersion,
                        Binding = regional.Version_VER.VER_BindingServicios,
                        Modulos = regional.Version_VER.VersionServicios_VER.ToList()
                                             .ConvertAll<VEModulo>(
                                                 version => new VEModulo()
                                                 {
                                                     Descripcion = version.Modulo_VER.MOD_Descripcion,
                                                     IdModulo = version.VES_IdModulo,
                                                     UrlAplicacion = version.VES_UriAplicacion,
                                                     UrlServicio = version.VES_UrlServicio,
                                                     TipoProxy = version.VES_TipoProxy
                                                 }),
                        MenusCapacitacion = entidades.MenuCapacitacion_VER.ToList()
                          .ConvertAll<VEMenuCapacitacion>(
                          menuCapacitacion => new VEMenuCapacitacion()
                          {
                              Id = menuCapacitacion.MEC_IdMenu,
                              Descripcion = menuCapacitacion.MEC_Descripcion,
                              Activo = menuCapacitacion.MEC_Activo,
                              IdProceso = (VEEnumProcesos)(Enum.Parse(typeof(VEEnumProcesos), menuCapacitacion.MEC_IdProceso.ToString())),
                              Target = menuCapacitacion.MEC_Target,
                              URL = menuCapacitacion.MEC_URL,
                              IdAncestro = menuCapacitacion.MEC_IdAncestro,
                              AplicaUsuario = menuCapacitacion.MEC_AplicaUsuario,
                              EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          }),

                        Cultura = entidades.ParametrosFramework.Where(p => p.PAR_IdParametro == "Cultura").First().PAR_ValorParametro
                    };

                    infoVersion.DatosInicioSesion = new VEDatosInicioSesion();

                    infoVersion.DatosInicioSesion.Cargo = new Servicios.ContratoDatos.Agenda.ASCargo()
                    {
                        IdCargo = _usuario.PEI_IdCargo,
                        Descripcion = _usuario.CAR_Descripcion
                    };

                    infoVersion.DatosInicioSesion.DatosCentroServicio = datosCentroServicio;                    
                    infoVersion.DatosInicioSesion.DatosCentroServicio.Caja = datosIngreso.LocacionIngreso.IdCaja;                    
                    infoVersion.DatosInicioSesion.DocumentoUsuario = _usuario.PEI_Identificacion;
                    infoVersion.DatosInicioSesion.IdPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "PaisPredeterminado").PAR_ValorParametro;
                    infoVersion.DatosInicioSesion.DescPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "DesPaisPredeterminad").PAR_ValorParametro;
                    infoVersion.DatosInicioSesion.NombresUsuario = _usuario.PEI_Nombre;
                    infoVersion.DatosInicioSesion.ApellidosUsuario = _usuario.PEI_PrimerApellido + " " + _usuario.PEI_SegundoApellido;
                    infoVersion.DatosInicioSesion.FechaServidor = System.DateTime.Now;
                    infoVersion.DatosInicioSesion.EsCajeroPpal = _usuario.USU_EsCajeroPpal;
                    infoVersion.DatosInicioSesion.IdUsuario = _usuario.USU_IdCodigoUsuario;
                    infoVersion.DatosInicioSesion.IdCasaMatriz = datosCentroServicio.IdCasaMAtriz;
                    infoVersion.DatosInicioSesion.URLKompadreStereo = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "URLKompadreStereo").PAR_ValorParametro;

                    RegistrarIngresoDeUsuario(datosIngreso);

                    ControllerContext.Current.IdCaja = datosIngreso.LocacionIngreso.IdCaja;
                    ControllerContext.Current.IdCentroServicio = idCentroServicios;
                    Framework.Servidor.Excepciones.Session.SESesionUsuario.Instancia.ActualizaSesion();

                    return infoVersion;
                }
                else
                {
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                                           ETipoErrorFramework.EX_NO_EXISTE_VERSION_RACOL.ToString(),
                                           MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_VERSION_RACOL));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        /// <summary>
        /// Calcula la versión asociada a una gestión de la empresa
        /// </summary>
        /// <param name="idGestion">Identificador de la gestión</param>
        public VEVersionInfo CalcularVersionxGestion(string idGestion, VEDatosIngresoUsuario datosIngreso)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                long codigoGestion = long.Parse(idGestion);
                Modelo.VersionGestion_VER gestion = entidades.VersionGestion_VER.Include("Version_VER").Include("Gestion_ARE").Include("Gestion_ARE.MacroProceso_ARE").Include("Version_VER.VersionServicios_VER").Include("Version_VER.VersionServicios_VER.Modulo_VER").FirstOrDefault(v => v.VEG_CodigoGestion == codigoGestion);
                if (gestion != null)
                {
                    // Se obtiene la información del usuario que intenta ingresar a la aplicación
                    Framework.Servidor.Versionamiento.Modelo.UsuarioPersonaInterna_VSEG _usuario = entidades.UsuarioPersonaInterna_VSEG.First(u => u.USU_IdUsuario == datosIngreso.Usuario && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO);

                    VEVersionInfo infoVersion = new VEVersionInfo()
                    {
                        VersionComercial = gestion.VEG_IdVersion,
                        Binding = gestion.Version_VER.VER_BindingServicios,
                        Modulos = gestion.Version_VER.VersionServicios_VER.ToList()
                                             .ConvertAll<VEModulo>(
                                                 version => new VEModulo()
                                                 {
                                                     Descripcion = version.Modulo_VER.MOD_Descripcion,
                                                     IdModulo = version.VES_IdModulo,
                                                     UrlAplicacion = version.VES_UriAplicacion,
                                                     UrlServicio = version.VES_UrlServicio,
                                                     TipoProxy = version.VES_TipoProxy
                                                 }),
                        MenusCapacitacion = entidades.MenuCapacitacion_VER.ToList()
                          .ConvertAll<VEMenuCapacitacion>(
                          menuCapacitacion => new VEMenuCapacitacion()
                          {
                              Id = menuCapacitacion.MEC_IdMenu,
                              Descripcion = menuCapacitacion.MEC_Descripcion,
                              Activo = menuCapacitacion.MEC_Activo,
                              IdProceso = (VEEnumProcesos)(Enum.Parse(typeof(VEEnumProcesos), menuCapacitacion.MEC_IdProceso.ToString())),
                              Target = menuCapacitacion.MEC_Target,
                              URL = menuCapacitacion.MEC_URL,
                              IdAncestro = menuCapacitacion.MEC_IdAncestro,
                              AplicaUsuario = menuCapacitacion.MEC_AplicaUsuario,
                              EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          }),

                        Cultura = entidades.ParametrosFramework.Where(p => p.PAR_IdParametro == "Cultura").First().PAR_ValorParametro
                    };

                    infoVersion.DatosInicioSesion = new VEDatosInicioSesion();

                    infoVersion.DatosInicioSesion.Cargo = new Servicios.ContratoDatos.Agenda.ASCargo()
                    {
                        IdCargo = _usuario.PEI_IdCargo,
                        Descripcion = _usuario.CAR_Descripcion
                    };

                    infoVersion.DatosInicioSesion.CodigoGestion = codigoGestion;
                    infoVersion.DatosInicioSesion.NombreGestion = gestion.Gestion_ARE.MacroProceso_ARE.MAP_Descripcion + " - " + gestion.Gestion_ARE.GES_Descripcion;
                    infoVersion.DatosInicioSesion.IdCasaMatriz = gestion.Gestion_ARE.MacroProceso_ARE.MAP_IdCasaMatriz;

                    infoVersion.DatosInicioSesion.DocumentoUsuario = _usuario.PEI_Identificacion;
                    infoVersion.DatosInicioSesion.IdPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "PaisPredeterminado").PAR_ValorParametro;
                    infoVersion.DatosInicioSesion.DescPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "DesPaisPredeterminad").PAR_ValorParametro;
                    infoVersion.DatosInicioSesion.NombresUsuario = _usuario.PEI_Nombre;
                    infoVersion.DatosInicioSesion.ApellidosUsuario = _usuario.PEI_PrimerApellido + " " + _usuario.PEI_SegundoApellido;
                    infoVersion.DatosInicioSesion.FechaServidor = System.DateTime.Now;
                    infoVersion.DatosInicioSesion.EsCajeroPpal = _usuario.USU_EsCajeroPpal;
                    infoVersion.DatosInicioSesion.IdUsuario = _usuario.USU_IdCodigoUsuario;
                    infoVersion.DatosInicioSesion.URLKompadreStereo = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "URLKompadreStereo").PAR_ValorParametro;

                    RegistrarIngresoDeUsuario(datosIngreso);

                    return infoVersion;
                }
                else
                {
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                                           ETipoErrorFramework.EX_NO_EXISTE_VERSION_GESTION.ToString(),
                                           MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_VERSION_GESTION));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        /// <summary>
        /// Calcula la versión del cliente crédito que no requiere id de máquina
        /// </summary>
        /// <param name="idCliente"></param>
        public VEVersionInfo CalcularVersionxSucursal(int idSucursal, VEDatosIngresoUsuario datosIngreso)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                int idCliente = 0;
                Sucursal_CLI sucursal = entidades.Sucursal_CLI.Include("Localidad_PAR").Include("ClienteCredito_CLI").Where(su => su.SUC_IdSucursal == idSucursal).FirstOrDefault();

                if (sucursal != null)
                    idCliente = sucursal.ClienteCredito_CLI.CLI_IdCliente;

                Modelo.VersionCliente_VER vcliente = entidades.VersionCliente_VER.Include("Version_VER").Include("Version_VER.VersionServicios_VER").Include("Version_VER.VersionServicios_VER.Modulo_VER").FirstOrDefault(v => v.VEC_IdCliente == idCliente);
                if (vcliente != null)
                {
                    // Se obtiene la información del usuario que intenta ingresar a la aplicación
                    Framework.Servidor.Versionamiento.Modelo.UsuarioClienteCredito_VSEG _usuario = entidades.UsuarioClienteCredito_VSEG.First(u => u.USU_IdUsuario == datosIngreso.Usuario && u.USU_Estado == ConstantesFramework.ESTADO_ACTIVO);

                    VEVersionInfo infoVersion = new VEVersionInfo()
                    {
                        VersionComercial = vcliente.VEC_IdVersion,
                        Binding = vcliente.Version_VER.VER_BindingServicios,
                        Modulos = vcliente.Version_VER.VersionServicios_VER.ToList()
                                             .ConvertAll<VEModulo>(
                                                 version => new VEModulo()
                                                 {
                                                     Descripcion = version.Modulo_VER.MOD_Descripcion,
                                                     IdModulo = version.VES_IdModulo,
                                                     UrlAplicacion = version.VES_UriAplicacion,
                                                     UrlServicio = version.VES_UrlServicio,
                                                     TipoProxy = version.VES_TipoProxy
                                                 }),
                        MenusCapacitacion = entidades.MenuCapacitacion_VER.ToList()
 .ConvertAll<VEMenuCapacitacion>(
                          menuCapacitacion => new VEMenuCapacitacion()
                          {
                              Id = menuCapacitacion.MEC_IdMenu,
                              Descripcion = menuCapacitacion.MEC_Descripcion,
                              Activo = menuCapacitacion.MEC_Activo,
                              IdProceso = (VEEnumProcesos)(Enum.Parse(typeof(VEEnumProcesos), menuCapacitacion.MEC_IdProceso.ToString())),
                              Target = menuCapacitacion.MEC_Target,
                              URL = menuCapacitacion.MEC_URL,
                              IdAncestro = menuCapacitacion.MEC_IdAncestro,
                              AplicaUsuario = menuCapacitacion.MEC_AplicaUsuario,
                              EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          }),

                        Cultura = entidades.ParametrosFramework.Where(p => p.PAR_IdParametro == "Cultura").First().PAR_ValorParametro
                    };

                    infoVersion.DatosInicioSesion = new VEDatosInicioSesion();
                    infoVersion.DatosInicioSesion.EsCajeroPpal = true;
                    infoVersion.DatosInicioSesion.Cargo = new Servicios.ContratoDatos.Agenda.ASCargo()
                    {
                        IdCargo = 0,
                        Descripcion = "Cliente crédito"
                    };

                    infoVersion.DatosInicioSesion.DatosClienteCredito = ConsultarDatosSucursalCliente(idSucursal);

                    infoVersion.DatosInicioSesion.DocumentoUsuario = _usuario.CLI_Nit;
                    infoVersion.DatosInicioSesion.IdPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "PaisPredeterminado").PAR_ValorParametro;
                    infoVersion.DatosInicioSesion.DescPaisPorDefecto = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "DesPaisPredeterminad").PAR_ValorParametro;
                    infoVersion.DatosInicioSesion.NombresUsuario = _usuario.CLI_RazonSocial;
                    infoVersion.DatosInicioSesion.ApellidosUsuario = "";
                    infoVersion.DatosInicioSesion.FechaServidor = System.DateTime.Now;
                    infoVersion.DatosInicioSesion.IdUsuario = _usuario.USU_IdCodigoUsuario;
                    infoVersion.DatosInicioSesion.URLKompadreStereo = entidades.ParametrosFramework.First(p => p.PAR_IdParametro == "URLKompadreStereo").PAR_ValorParametro;

                    RegistrarIngresoDeUsuario(datosIngreso);

                    return infoVersion;
                }
                else
                {
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                                           ETipoErrorFramework.EX_VERSION_CLIENTE_NO_CONFIGURADA.ToString(),
                                           MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_VERSION_CLIENTE_NO_CONFIGURADA));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        /// <summary>
        /// Consulta la información propia de una sucursal de un cliente crédito para la autenticación de la misma
        /// </summary>
        /// <returns></returns>
        private VEDatosClienteCredito ConsultarDatosSucursalCliente(int idSucursal)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Sucursal_CLI sucursal = entidades.Sucursal_CLI.Include("Localidad_PAR").Include("ClienteCredito_CLI").Where(su => su.SUC_IdSucursal == idSucursal).FirstOrDefault();

                VEDatosClienteCredito DatosClienteCredito = new VEDatosClienteCredito()
                {
                    ClienteId = sucursal.ClienteCredito_CLI.CLI_IdCliente,
                    ClienteNombre = sucursal.ClienteCredito_CLI.CLI_RazonSocial,
                    DigitoVerificacionCliente = sucursal.ClienteCredito_CLI.CLI_DigitoVerificacion,
                    DireccionCliente = sucursal.ClienteCredito_CLI.CLI_Direccion,
                    NitCliente = sucursal.ClienteCredito_CLI.CLI_Nit,
                    SucursalId = sucursal.SUC_IdSucursal,
                    SucursalNombre = sucursal.SUC_Nombre,
                    TelefonoCliente = sucursal.ClienteCredito_CLI.CLI_Telefono,
                    IdCiudad = sucursal.Localidad_PAR.LOC_IdLocalidad,
                    DescripcionCiudad = sucursal.Localidad_PAR.LOC_Nombre,
                    CodigoPostal = sucursal.Localidad_PAR.LOC_CodigoPostal
                };
                // Determinar país
                if (sucursal.Localidad_PAR.LOC_IdAncestroTercerGrado != null && sucursal.Localidad_PAR.LOC_IdAncestroTercerGrado != string.Empty)
                {
                    Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == sucursal.Localidad_PAR.LOC_IdAncestroTercerGrado);
                    DatosClienteCredito.IdPais = pais.LOC_IdLocalidad;
                    DatosClienteCredito.DescripcionPais = pais.LOC_Nombre;
                }
                else
                {
                    if (sucursal.Localidad_PAR.LOC_IdAncestroSegundoGrado != null && sucursal.Localidad_PAR.LOC_IdAncestroSegundoGrado != string.Empty)
                    {
                        Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == sucursal.Localidad_PAR.LOC_IdAncestroSegundoGrado);
                        DatosClienteCredito.IdPais = pais.LOC_IdLocalidad;
                        DatosClienteCredito.DescripcionPais = pais.LOC_Nombre;
                    }
                    else
                    {
                        Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == sucursal.Localidad_PAR.LOC_IdAncestroPrimerGrado);
                        DatosClienteCredito.IdPais = pais.LOC_IdLocalidad;
                        DatosClienteCredito.DescripcionPais = pais.LOC_Nombre;
                    }
                }
                return DatosClienteCredito;
            }
        }

        /// <summary>
        /// Consulta la información básica asociada a un centro de servicios.
        /// </summary>
        /// <param name="idCentroServicios">Código del centro de servicios</param>
        /// <returns></returns>
        private VEDatosCentroServicio ConsultarDatosCentroServicio(long idCentroServicios)
        {
            VEDatosCentroServicio datosCentroServicio = new VEDatosCentroServicio();
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                var centroServicios =
                  entidades.CentroServicios_PUA
                  .Where(cs => cs.CES_IdCentroServicios == idCentroServicios)
                  .Join(entidades.Localidades_VPAR, locCen => locCen.CES_IdMunicipio, locLoc => locLoc.LOC_IdLocalidad, (locCen, locLoc) =>
                    new
                    {
                        f = locCen,
                        g = locLoc
                    }
                    )
                  .FirstOrDefault();

                datosCentroServicio.CentroCostos = centroServicios.f.CES_IdCentroCostos;
                datosCentroServicio.BaseInicialCaja = centroServicios.f.CES_BaseInicialCaja;
                datosCentroServicio.DescCiudadCentroServicio = centroServicios.g.LOC_Nombre;
                datosCentroServicio.DescCiudadCentroServicioCompleta = centroServicios.g.NombreCompleto;
                datosCentroServicio.DireccionCentroServicio = centroServicios.f.CES_Direccion;
                datosCentroServicio.IdCentroServicio = idCentroServicios;
                datosCentroServicio.IdCiudadCentroServicio = centroServicios.g.LOC_IdLocalidad;
                datosCentroServicio.NombreCentroServicio = centroServicios.f.CES_Nombre;
                datosCentroServicio.TelefonoCentroServicio = centroServicios.f.CES_Telefono1;
                datosCentroServicio.TipoCentroServicios = centroServicios.f.CES_Tipo;
                datosCentroServicio.CodigoPostal = centroServicios.g.LOC_CodigoPostal;

                // Determinar país
                if (centroServicios.g.LOC_IdAncestroTercerGrado != null && centroServicios.g.LOC_IdAncestroTercerGrado != string.Empty)
                {
                    Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == centroServicios.g.LOC_IdAncestroTercerGrado);
                    datosCentroServicio.IdPaisCentroServicio = pais.LOC_IdLocalidad;
                    datosCentroServicio.DescPaisCentroServicio = pais.LOC_Nombre;
                }
                else
                {
                    if (centroServicios.g.LOC_IdAncestroSegundoGrado != null && centroServicios.g.LOC_IdAncestroSegundoGrado != string.Empty)
                    {
                        Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == centroServicios.g.LOC_IdAncestroSegundoGrado);
                        datosCentroServicio.IdPaisCentroServicio = pais.LOC_IdLocalidad;
                        datosCentroServicio.DescPaisCentroServicio = pais.LOC_Nombre;
                    }
                    else
                    {
                        Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == centroServicios.g.LOC_IdAncestroPrimerGrado);
                        datosCentroServicio.IdPaisCentroServicio = pais.LOC_IdLocalidad;
                        datosCentroServicio.DescPaisCentroServicio = pais.LOC_Nombre;
                    }
                }

                //Determinar Col y Racol
                if (centroServicios.f.CES_Tipo == "PTO")
                {
                    PuntoServicio_PUA pun = entidades.PuntoServicio_PUA.Include("Agencia_PUA").Include("Agencia_PUA.CentroLogistico_PUA").Include("Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA")
                      .Include("Agencia_PUA.CentroLogistico_PUA.CentroServicios_PUA").Include("Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA").Where(pu => pu.PUS_IdPuntoServicio == idCentroServicios).FirstOrDefault();
                    datosCentroServicio.NombreCol = pun.Agencia_PUA.CentroLogistico_PUA.CentroServicios_PUA.CES_Nombre;
                    datosCentroServicio.IdCol = pun.Agencia_PUA.CentroLogistico_PUA.CEL_IdCentroLogistico;
                    datosCentroServicio.DireccionCol = pun.Agencia_PUA.CentroLogistico_PUA.CentroServicios_PUA.CES_Direccion;
                    datosCentroServicio.TelefonoCol = pun.Agencia_PUA.CentroLogistico_PUA.CentroServicios_PUA.CES_Telefono1;

                    datosCentroServicio.IdRacol = pun.Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA.REA_IdRegionalAdm;
                    datosCentroServicio.NombreRaCol = pun.Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA.REA_Descripcion;
                    datosCentroServicio.DireccionRaCol = pun.Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA.CES_Direccion;
                    datosCentroServicio.TelefonoRaCol = pun.Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA.CES_Telefono1;
                    datosCentroServicio.IdCasaMAtriz = pun.Agencia_PUA.CentroLogistico_PUA.RegionalAdministrativa_PUA.REA_IdCasaMatriz;
                }
                else if (centroServicios.f.CES_Tipo == "AGE")
                {
                    Agencia_PUA age = entidades.Agencia_PUA.Include("CentroLogistico_PUA").Include("CentroLogistico_PUA.RegionalAdministrativa_PUA")
                      .Include("CentroLogistico_PUA.CentroServicios_PUA").Include("CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA").Where(ag => ag.AGE_IdAgencia == idCentroServicios).FirstOrDefault();
                    datosCentroServicio.NombreCol = age.CentroLogistico_PUA.CentroServicios_PUA.CES_Nombre;
                    datosCentroServicio.IdCol = age.CentroLogistico_PUA.CEL_IdCentroLogistico;
                    datosCentroServicio.DireccionCol = age.CentroLogistico_PUA.CentroServicios_PUA.CES_Direccion;
                    datosCentroServicio.TelefonoCol = age.CentroLogistico_PUA.CentroServicios_PUA.CES_Telefono1;

                    datosCentroServicio.IdRacol = age.CentroLogistico_PUA.CEL_IdRegionalAdm;
                    datosCentroServicio.NombreRaCol = age.CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA.CES_Nombre;
                    datosCentroServicio.DireccionRaCol = age.CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA.CES_Direccion;
                    datosCentroServicio.TelefonoRaCol = age.CentroLogistico_PUA.RegionalAdministrativa_PUA.CentroServicios_PUA.CES_Telefono1;
                    datosCentroServicio.IdCasaMAtriz = age.CentroLogistico_PUA.RegionalAdministrativa_PUA.REA_IdCasaMatriz;
                }
                else if (centroServicios.f.CES_Tipo == "RAC")
                {
                    RegionalAdministrativa_PUA regional = entidades.RegionalAdministrativa_PUA.Include("CentroServicios_PUA").Where(r => r.REA_IdRegionalAdm == idCentroServicios).FirstOrDefault();
                    datosCentroServicio.IdRacol = regional.REA_IdRegionalAdm;
                    datosCentroServicio.NombreRaCol = regional.CentroServicios_PUA.CES_Nombre;
                    datosCentroServicio.DireccionRaCol = regional.CentroServicios_PUA.CES_Direccion;
                    datosCentroServicio.TelefonoRaCol = regional.CentroServicios_PUA.CES_Telefono1;
                    datosCentroServicio.IdCasaMAtriz = regional.REA_IdCasaMatriz;
                }
            }

            datosCentroServicio.Biometrico = ObtenerBiometricoCentroServicio(idCentroServicios);

            return datosCentroServicio;
        }
        /// <summary>
        /// retorna si un centro de servicios tiene un biometrico habilitado
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        private bool ObtenerBiometricoCentroServicio(long idCentroServicios)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerBiometricoCentroServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCentroServicio", idCentroServicios);
                conn.Open();
                bool rst = Convert.ToBoolean(cmd.ExecuteScalar());
                conn.Close();
                return rst;
            }

        }

        /// <summary>
        /// Se inicia la solicitud de alta del punto o agencia
        /// </summary>
        /// <param name="idPuntoAtencion">Identificador del punto o agencia a solicitar alta</param>
        /// <param name="caja">Número de caja a registrar</param>
        /// <param name="idMaquina">Identificador de la máquina</param>
        /// <param name="usuario">Usuario que hace la solicitud</param>
        public void SolicitarAltaMaquina(string idMaquina, string usuario)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                MaqVersion_VER maquversion = entidades.MaqVersion_VER.Where(m => m.MAV_MaquinaId == idMaquina && m.MAV_CreadoPor == usuario).FirstOrDefault();

                if (maquversion == null)
                {
                    maquversion = new MaqVersion_VER()
                    {
                        MAV_CreadoPor = usuario,
                        MAV_Estado = "PAC",
                        MAV_FechaGrabacion = DateTime.Now,
                        MAV_IdVersion = entidades.Version_VER.OrderByDescending(OR => OR.VER_FechaGrabacion).FirstOrDefault().VER_IdVersion,
                        MAV_MaquinaId = idMaquina,
                        MAV_Observaciones = "Solicitud de registro de máquina"
                    };

                    entidades.MaqVersion_VER.Add(maquversion);
                    entidades.SaveChanges();
                }
            }
        }

        #endregion Version

        #region Token

        /// <summary>
        /// Recicla las sesiones de los clientes cuanto cumplen un tiempo de haber sido iniciadas, este tiempo es configurable por base de datos.
        /// </summary>
        public void ReciclarSesionClientess()
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Modelo.ParametrosFramework tiempoVidaToken = entidades.ParametrosFramework.SingleOrDefault(par => par.PAR_IdParametro == "TiempoVidaToken");
                if (tiempoVidaToken != null)
                {
                    int tiempoEnMinutos = 0;
                    if (int.TryParse(tiempoVidaToken.PAR_ValorParametro, out tiempoEnMinutos))
                    {
                        DateTime fechaAComparar = DateTime.Now.AddMinutes(-1 * tiempoEnMinutos);
                        List<Modelo.ValidacionMaq_VER> tokens = entidades.ValidacionMaq_VER.Where(maq => maq.VAM_FechaGrabacion.CompareTo(fechaAComparar) < 0).ToList();
                        tokens.ForEach(token => this.EliminarToken(token.VAM_TokenIngreso, token.VAM_MaquinaVersionId));
                    }
                    else
                    {
                        ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_TIEMPO_VIDA_NO_CONFIGURADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_TIEMPO_VIDA_NO_CONFIGURADO));
                        throw new FaultException<ControllerException>(exc);
                    }
                }
                else
                {
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_TIEMPO_VIDA_NO_CONFIGURADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_TIEMPO_VIDA_NO_CONFIGURADO));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        /// <summary>
        /// Valida el Token de la sesión
        /// </summary>
        /// <param name="token">Identificador de la sesión de usuario</param>
        /// <param name="idMaquina">Identificador de la máquina</param>
        /// <returns>Se retorna si el token fué validado o no</returns>
        public bool ValidarToken(string token, string idMaquina)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Modelo.MaqVersion_VER maquina = (entidades.MaqVersion_VER.SingleOrDefault(v => v.MAV_MaquinaId == idMaquina));
                if (maquina != null)
                {
                    int idVersionMaquina = maquina.MAV_MaquinaVersionId;
                    return entidades.ValidacionMaq_VER.Count(e => e.VAM_TokenIngreso == token &&
                       e.VAM_MaquinaVersionId == idVersionMaquina) > 0;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Se elimina un token de la tabla
        /// </summary>
        /// <param name="token">Identificador de la sesión de usuario</param>
        /// <param name="idMaquina">Identificador de la máquina</param>
        public void EliminarToken(string token, int idVersionMaquina)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Modelo.ValidacionMaq_VER sesion = entidades.ValidacionMaq_VER.FirstOrDefault(validacion => validacion.VAM_TokenIngreso == token && validacion.VAM_MaquinaVersionId == idVersionMaquina);
                if (sesion != null)
                {
                    entidades.ValidacionMaq_VER.Remove(sesion);
                    entidades.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Registra el token de sesión
        /// </summary>
        /// <param name="token">Identificador de la sesión de usuario</param>
        /// <param name="idMaquina">Identificador de la máquina</param>
        public void RegistrarToken(string token, string idMaquina, string usuario)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Modelo.MaqVersion_VER maquina = (entidades.MaqVersion_VER.SingleOrDefault(v => v.MAV_MaquinaId == idMaquina && v.MAV_CreadoPor == usuario));
                if (maquina != null)
                {
                    int idVersionMaquina = maquina.MAV_MaquinaVersionId;

                    Modelo.ValidacionMaq_VER sesion = new Modelo.ValidacionMaq_VER()
                    {
                        VAM_FechaGrabacion = DateTime.Now,
                        VAM_MaquinaVersionId = idVersionMaquina,
                        VAM_TokenIngreso = token,
                        VAM_CreadoPor = usuario
                    };

                    entidades.ValidacionMaq_VER.Add(sesion);
                    entidades.SaveChanges();
                }
                else
                {
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ID_NO_IDENTIFICADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ID_NO_IDENTIFICADO));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        #endregion Token

        /// <summary>
        /// Obtiene la lista de clientes crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Servicios.ContratoDatos.Versionamiento.VECliente> ObtenerClientes()
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                return entidades.ClienteCredito_CLI.ToList().ConvertAll<Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VECliente>(c =>
                    new Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VECliente()
                    {
                        Id = c.CLI_IdCliente,
                        Descripcion = c.CLI_RazonSocial
                    }
                  );
            }
        }

        /// <summary>
        /// Retorna lista de sucursales
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public IEnumerable<Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VESucursal> ObtenerSucursales(int idCliente)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                return entidades.Sucursal_CLI.Where(s => s.SUC_ClienteCredito == idCliente).ToList().ConvertAll(s =>
                    new Framework.Servidor.Servicios.ContratoDatos.Versionamiento.VESucursal
                    {
                        Id = s.SUC_IdSucursal,
                        Descripcion = s.SUC_Nombre
                    }
                  );
            }
        }

        /// <summary>
        /// Retorna información de una sucursal de un cliente
        /// </summary>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        /// <returns></returns>
        public VESucursal ObtenerInfoSucursal(int idSucursal)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                SucursalZonaLocalidad_VCLI sucursal = entidades.SucursalZonaLocalidad_VCLI.FirstOrDefault(s => s.SUC_IdSucursal == idSucursal);
                if (sucursal != null)
                {
                    VESucursal suc = new VESucursal
                    {
                        Id = sucursal.SUC_IdSucursal,
                        Descripcion = sucursal.SUC_Nombre,
                        Ciudad = new Servicios.ContratoDatos.Parametros.PALocalidadDC()
                        {
                            IdLocalidad = sucursal.LOC_IdLocalidad,
                            Nombre = sucursal.LOC_Nombre
                        },
                        CodigoPostal = sucursal.LOC_CodigoPostal
                    };

                    // Determinar país
                    if (sucursal.LOC_IdAncestroTercerGrado != null && sucursal.LOC_IdAncestroTercerGrado != string.Empty)
                    {
                        Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == sucursal.LOC_IdAncestroTercerGrado);
                        suc.Pais = new Servicios.ContratoDatos.Parametros.PALocalidadDC() { IdLocalidad = pais.LOC_IdLocalidad, Nombre = pais.LOC_Nombre };
                    }
                    else
                    {
                        if (sucursal.LOC_IdAncestroSegundoGrado != null && sucursal.LOC_IdAncestroSegundoGrado != string.Empty)
                        {
                            Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == sucursal.LOC_IdAncestroSegundoGrado);
                            suc.Pais = new Servicios.ContratoDatos.Parametros.PALocalidadDC { IdLocalidad = pais.LOC_IdLocalidad, Nombre = pais.LOC_Nombre };
                        }
                        else
                        {
                            Modelo.Localidad_PAR pais = entidades.Localidad_PAR.SingleOrDefault(localidad => localidad.LOC_IdLocalidad == sucursal.LOC_IdAncestroPrimerGrado);
                            suc.Pais = new Servicios.ContratoDatos.Parametros.PALocalidadDC() { IdLocalidad = pais.LOC_IdLocalidad, Nombre = pais.LOC_Nombre };
                        }
                    }
                    return suc;
                }
                else
                {
                    ControllerException exc = new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_SUCURSAL_NO_VALIDA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_SUCURSAL_NO_VALIDA));
                    throw new FaultException<ControllerException>(exc);
                }
            }
        }

        /// <summary>
        /// Calucula el cargo del usuario autenticado
        /// </summary>
        /// <param name="idUsuario">Id del usuario autenticado</param>
        /// <returns>Id del cargo</returns>
        public int CalcularCargoUsuarioAutenticado(string idUsuario)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Framework.Servidor.Versionamiento.Modelo.UsuarioPersonaInterna_VSEG usuario = entidades.UsuarioPersonaInterna_VSEG.Where(r => r.USU_IdUsuario == idUsuario && r.USU_Estado == ConstantesFramework.ESTADO_ACTIVO).SingleOrDefault();
                return usuario.PEI_IdCargo;
            }
        }

        public Dictionary<string, string> ObtenerUrlsServicios()
        {
            using (Modelo.VersionEntidades contexto = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                Dictionary<string, string> urlsServicios = new Dictionary<string, string>();

                contexto.VersionServicios_VER.ToList().ForEach((u) =>
                {
                    string tipoProxy = u.VES_TipoProxy.Split('.')[u.VES_TipoProxy.Split('.').Length - 1];
                    urlsServicios.Add(tipoProxy, u.VES_UrlServicio);
                });

                return urlsServicios;
            }
        }

        /// <summary>
        /// Obtiene la lista de menus capacitación
        /// </summary>
        /// <param name="idRacol">Id del Racol</param>
        public List<VEMenuCapacitacion> ObtenerMenusCapacitacion()
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(@"
                                                SELECT [MEC_IdMenu]
                                                      ,[MEC_IdAncestro]
                                                      ,[MEC_IdProceso]
                                                      ,[MEC_Descripcion]
                                                      ,[MEC_Target]
                                                      ,[MEC_URL]
                                                      ,[MEC_AplicaUsuario]
                                                      ,[MEC_Activo]
                                                  FROM [MenuCapacitacion_VER]
                                                  WHERE MEC_Activo = 1", sqlConn);

                DataTable dt = new DataTable();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(dt);
                sqlConn.Close();

                return dt.AsEnumerable().ToList()
                            .ConvertAll<VEMenuCapacitacion>(
                            m => new VEMenuCapacitacion()
                            {
                                Id = m.Field<int>("MEC_IdMenu"),
                                Descripcion = m.Field<string>("MEC_Descripcion"),
                                Activo = m.Field<bool>("MEC_Activo"),
                                IdProceso = (VEEnumProcesos)(Enum.Parse(typeof(VEEnumProcesos), m.Field<int>("MEC_IdProceso").ToString())),
                                Target = m.Field<string>("MEC_Target"),
                                URL = m.Field<string>("MEC_URL"),
                                IdAncestro = m.Field<int>("MEC_IdAncestro"),
                                AplicaUsuario = m.Field<bool>("MEC_AplicaUsuario")
                            });
            }
        }

        #region Metodos Privados

        /// <summary>
        /// Registra en la base de datos el ingreso al sistema de un usuario
        /// </summary>
        /// <param name="datosIngreso">Datos asociados al ingreso del usuario</param>
        private void RegistrarIngresoDeUsuario(VEDatosIngresoUsuario datosIngreso)
        {
            using (Modelo.VersionEntidades entidades = new Modelo.VersionEntidades(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(nombreModelo)))
            {
                HistoricoIngresosUsuario_SEG histIngreso = new HistoricoIngresosUsuario_SEG()
                {
                    HIU_DescripcionLocacion = datosIngreso.LocacionIngreso.DescripcionLocacion,
                    HIU_FechaGrabacion = DateTime.Now,
                    HIU_Usuario = datosIngreso.Usuario,
                    HIU_IdLocacion = datosIngreso.LocacionIngreso.IdLocacion,
                    HIU_TipoLocacion = datosIngreso.LocacionIngreso.TipoLocacion.ToString()
                };
                entidades.HistoricoIngresosUsuario_SEG.Add(histIngreso);
                entidades.SaveChanges();
            }
        }

        #endregion Metodos Privados
    }
}