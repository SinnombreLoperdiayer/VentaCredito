using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using CO.Servidor.ParametrosOperacion.Comun;
using CO.Servidor.ParametrosOperacion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using CO.Servidor.ParametrosOperacion.Datos.Mapper;
using System.Transactions;

namespace CO.Servidor.ParametrosOperacion.Datos
{
    public class PORepositorio
    {
        private static readonly PORepositorio instancia = new PORepositorio();
        private const string NombreModelo = "ModeloParametrosOperacion";

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static PORepositorio Instancia
        {
            get { return PORepositorio.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private PORepositorio()
        {
        }

        #region Conductores

        /// <summary>
        /// Consulta todos los tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public IList<POTipoVehiculo> ObtenerTodosTipoVehiculo()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoVehiculo_CPO.ToList().ConvertAll<POTipoVehiculo>(tv =>
                  new POTipoVehiculo()
                  {
                      IdTipoVehiculo = tv.TIV_IdTipoVehiculo,
                      Descripcion = tv.TIV_Descripcion
                  }).OrderBy(o => o.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene  los conductores
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los conductores</returns>
        public IList<POConductores> ObtenerConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsConductorPersonaInterna_VCPO(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .ToList().ConvertAll<POConductores>(c =>
                  {
                      string segundoApellido = c.PEI_SegundoApellido == null ? "" : c.PEI_SegundoApellido;

                      POConductores conductor = new POConductores()
                      {
                          FechaIngreso = c.CON_FechaIngreso,
                          FechaTerminacionContrato = c.CON_FechaTerminacionContrato,
                          IdConductor = c.CON_IdConductor,
                          Telefono2 = c.CON_Telefono2,
                          TipoContrato = new POTipoContrato()
                          {
                              Descripcion = c.CON_TipoContrato,
                          },
                          EsContratista = c.CON_EsContratista,
                          Estado = c.CON_Estado,
                          EstadoInicial = c.CON_Estado,
                          FechaVencimientoPase = c.CON_FechaVencimientoPase,
                          NumeroPase = c.CON_NumeroPase,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          Identificacion = c.PEI_Identificacion.Trim(),
                          NombreCompleto = c.PEI_Nombre + " " + c.PEI_PrimerApellido + " " + segundoApellido,
                          NombreMunicipio = c.LOC_Nombre,
                          NombreMunicipioCompleto = c.LOC_NombreCompleto,
                          Ciudad = new PALocalidadDC()
                          {
                              Nombre = c.LOC_Nombre,
                              IdLocalidad = c.PEI_Municipio
                          },
                          PersonaInterna = new PAPersonaInternaDC()
                          {
                              Direccion = c.PEI_Direccion,
                              IdCargo = c.PEI_IdCargo,
                              Identificacion = c.PEI_Identificacion.Trim(),
                              IdPersonaInterna = c.PEI_IdPersonaInterna,
                              IdRegionalAdministrativa = c.PEI_IdRegionalAdm,
                              Municipio = c.PEI_Municipio,
                              IdTipoIdentificacion = c.PEI_IdTipoIdentificacion.Trim(),
                              Nombre = c.PEI_Nombre,
                              NombreCargo = c.CAR_Descripcion,
                              NombreCompleto = c.PEI_Nombre,
                              PrimerApellido = c.PEI_PrimerApellido,
                              SegundoApellido = c.PEI_SegundoApellido,
                              NombreMunicipio = c.LOC_Nombre,
                              Telefono = c.PEI_Telefono
                          },

                          Racoles = contexto.ConductorRegionalAdmini_VCPO.Where(r => r.CRA_IdConductor == c.CON_IdConductor).ToList().ConvertAll<PURegionalAdministrativa>(
                        r => new PURegionalAdministrativa()
                        {
                            IdRegionalAdmin = r.REA_IdRegionalAdm,
                            CentroServicios = new PUCentroServiciosDC()
                            {
                                Nombre = r.CES_Nombre
                            }
                        })
                      };

                      return conductor;
                  });
            }
        }

        /// <summary>
        /// Obtiene todos los conductores
        /// </summary>
        public IList<POConductores> ObtenerTodosConductores()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConductorPersonaInterna_VCPO.Where(c => c.CON_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList().ConvertAll
                  (
                  c => new POConductores()
                  {
                      FechaIngreso = c.CON_FechaIngreso,
                      FechaTerminacionContrato = c.CON_FechaTerminacionContrato,
                      IdConductor = c.CON_IdConductor,
                      Telefono2 = c.CON_Telefono2,
                      TipoContrato = new POTipoContrato()
                      {
                          Descripcion = c.CON_TipoContrato,
                      },
                      EsContratista = c.CON_EsContratista,
                      Estado = c.CON_Estado,
                      FechaVencimientoPase = c.CON_FechaVencimientoPase,
                      NumeroPase = c.CON_NumeroPase,
                      PersonaInterna = new PAPersonaInternaDC()
                      {
                          Direccion = c.PEI_Direccion,
                          IdCargo = c.PEI_IdCargo,
                          Identificacion = c.PEI_Identificacion.Trim(),
                          IdPersonaInterna = c.PEI_IdPersonaInterna,
                          IdRegionalAdministrativa = c.PEI_IdRegionalAdm,
                          Municipio = c.PEI_Municipio,
                          IdTipoIdentificacion = c.PEI_IdTipoIdentificacion,
                          Nombre = c.PEI_Nombre,
                          NombreCargo = c.CAR_Descripcion,
                          NombreCompleto = c.PEI_Nombre + " " + c.PEI_PrimerApellido,
                          PrimerApellido = c.PEI_PrimerApellido,
                          SegundoApellido = c.PEI_SegundoApellido,
                          NombreMunicipio = c.LOC_Nombre,
                          Telefono = c.PEI_Telefono
                      }
                  }).OrderBy(o => o.PersonaInterna.Nombre).ToList();
            }
        }

        /// <summary>
        /// Adiciona un nuevo conductor
        /// </summary>
        /// <param name="conductor"></param>
        public void AdicionarConductor(POConductores conductor)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInterna_PAR per = contexto.PersonaInterna_PAR.Where(p => p.PEI_Identificacion.Trim() == conductor.PersonaInterna.Identificacion.Trim() && p.PEI_IdTipoIdentificacion == conductor.PersonaInterna.IdTipoIdentificacion).FirstOrDefault();

                if (per == null)
                {
                    per = new PersonaInterna_PAR()
                    {
                        PEI_Comentarios = string.Empty,
                        PEI_CreadoPor = ControllerContext.Current.Usuario,
                        PEI_FechaGrabacion = DateTime.Now,
                        PEI_Direccion = conductor.PersonaInterna.Direccion,
                        PEI_IdCargo = conductor.PersonaInterna.IdCargo,
                        PEI_IdRegionalAdm = conductor.PersonaInterna.IdRegionalAdministrativa,
                        PEI_Identificacion = conductor.PersonaInterna.Identificacion.Trim(),
                        PEI_IdTipoIdentificacion = conductor.PersonaInterna.IdTipoIdentificacion,
                        PEI_Municipio = conductor.Ciudad.IdLocalidad,
                        PEI_Nombre = conductor.PersonaInterna.Nombre,
                        PEI_PrimerApellido = conductor.PersonaInterna.PrimerApellido,
                        PEI_SegundoApellido = conductor.PersonaInterna.SegundoApellido,
                        PEI_Telefono = conductor.PersonaInterna.Telefono
                    };
                    contexto.PersonaInterna_PAR.Add(per);
                    contexto.SaveChanges();

                    Conductor_CPO cond = new Conductor_CPO()
                    {
                        CON_CreadoPor = ControllerContext.Current.Usuario,
                        CON_FechaGrabacion = DateTime.Now,
                        CON_EsContratista = conductor.EsContratista,
                        CON_Estado = conductor.Estado,
                        CON_FechaIngreso = conductor.FechaIngreso,
                        CON_FechaTerminacionContrato = conductor.FechaTerminacionContrato,
                        CON_FechaVencimientoPase = conductor.FechaVencimientoPase,
                        CON_NumeroPase = conductor.NumeroPase,
                        CON_Telefono2 = conductor.Telefono2,
                        CON_TipoContrato = conductor.TipoContrato.Descripcion,
                        CON_IdPersonaInterna = per.PEI_IdPersonaInterna
                    };
                    PORepositorioAudit.MapearAuditPersonaInterna(contexto);
                    contexto.Conductor_CPO.Add(cond);
                    contexto.SaveChanges();

                    //agregar los racol asociados
                    if (conductor.Racoles != null)
                        conductor.Racoles.ForEach(c =>
                        {
                            if (contexto.ConductorRegionalAdmi_CPO.Where(cr => cr.CRA_IdRegionalAdm == c.IdRegionalAdmin && cr.CRA_IdConductor == cond.CON_IdConductor).Count() <= 0)
                            {
                                contexto.ConductorRegionalAdmi_CPO.Add(new ConductorRegionalAdmi_CPO()
                                {
                                    CRA_IdRegionalAdm = c.IdRegionalAdmin,
                                    CRA_IdConductor = cond.CON_IdConductor,
                                    CRA_CreadoPor = ControllerContext.Current.Usuario,
                                    CRA_FechaGrabacion = DateTime.Now
                                });
                                PORepositorioAudit.MapearAuditConductorRegionalAdmin(contexto);
                            }
                        });
                }
                else
                {
                    per.PEI_Comentarios = string.Empty;
                    per.PEI_Direccion = conductor.PersonaInterna.Direccion;
                    per.PEI_IdCargo = conductor.PersonaInterna.IdCargo;
                    per.PEI_IdRegionalAdm = conductor.PersonaInterna.IdRegionalAdministrativa;
                    per.PEI_Identificacion = conductor.PersonaInterna.Identificacion.Trim();
                    per.PEI_IdTipoIdentificacion = conductor.PersonaInterna.IdTipoIdentificacion;
                    per.PEI_Municipio = conductor.Ciudad.IdLocalidad;
                    per.PEI_Nombre = conductor.PersonaInterna.Nombre;
                    per.PEI_PrimerApellido = conductor.PersonaInterna.PrimerApellido;
                    per.PEI_SegundoApellido = conductor.PersonaInterna.SegundoApellido;
                    per.PEI_Telefono = conductor.PersonaInterna.Telefono;

                    Conductor_CPO cond = contexto.Conductor_CPO.FirstOrDefault(c => c.CON_IdPersonaInterna == per.PEI_IdPersonaInterna);

                    if (cond == null)
                    {
                        cond = new Conductor_CPO()
                        {
                            CON_CreadoPor = ControllerContext.Current.Usuario,
                            CON_FechaGrabacion = DateTime.Now,
                            CON_EsContratista = conductor.EsContratista,
                            CON_Estado = conductor.Estado,
                            CON_FechaIngreso = conductor.FechaIngreso,
                            CON_FechaTerminacionContrato = conductor.FechaTerminacionContrato,
                            CON_FechaVencimientoPase = conductor.FechaVencimientoPase,
                            CON_NumeroPase = conductor.NumeroPase,
                            CON_Telefono2 = conductor.Telefono2,
                            CON_TipoContrato = conductor.TipoContrato.Descripcion,
                            CON_IdPersonaInterna = per.PEI_IdPersonaInterna
                        };
                        PORepositorioAudit.MapearAuditPersonaInterna(contexto);
                        contexto.Conductor_CPO.Add(cond);
                        contexto.SaveChanges();
                    }
                    else
                    {
                        cond.CON_EsContratista = conductor.EsContratista;
                        cond.CON_Estado = conductor.Estado;
                        cond.CON_FechaIngreso = conductor.FechaIngreso;
                        cond.CON_FechaTerminacionContrato = conductor.FechaTerminacionContrato;
                        cond.CON_FechaVencimientoPase = conductor.FechaVencimientoPase;
                        cond.CON_NumeroPase = conductor.NumeroPase;
                        cond.CON_Telefono2 = conductor.Telefono2;
                        cond.CON_TipoContrato = conductor.TipoContrato.Descripcion;
                        PORepositorioAudit.MapearAuditConductorVehiculo(contexto);
                        PORepositorioAudit.MapearAuditPersonaInterna(contexto);
                    }

                    //agregar los racol asociados
                    if (conductor.Racoles != null)
                        conductor.Racoles.ForEach(c =>
                        {
                            if (contexto.ConductorRegionalAdmi_CPO.Where(cr => cr.CRA_IdRegionalAdm == c.IdRegionalAdmin && cr.CRA_IdConductor == cond.CON_IdConductor).Count() <= 0)
                            {
                                contexto.ConductorRegionalAdmi_CPO.Add(new ConductorRegionalAdmi_CPO()
                                {
                                    CRA_IdRegionalAdm = c.IdRegionalAdmin,
                                    CRA_IdConductor = cond.CON_IdConductor,
                                    CRA_CreadoPor = ControllerContext.Current.Usuario,
                                    CRA_FechaGrabacion = DateTime.Now
                                });
                                PORepositorioAudit.MapearAuditConductorRegionalAdmin(contexto);
                            }
                        });
                }

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita la informacion de un conductor
        /// </summary>
        /// <param name="conductor"></param>
        public void EditarConductor(POConductores conductor)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Conductor_CPO conducBD;
                PersonaInterna_PAR personaBD;

                conducBD = contexto.Conductor_CPO.Where(c => c.CON_IdConductor == conductor.IdConductor).FirstOrDefault();
                if (conducBD != null)
                {
                    personaBD = contexto.PersonaInterna_PAR.FirstOrDefault(per => per.PEI_IdTipoIdentificacion == conductor.PersonaInterna.IdTipoIdentificacion && per.PEI_Identificacion == conductor.PersonaInterna.Identificacion);
                    if (personaBD == null)
                    {
                        personaBD = new PersonaInterna_PAR
                        {
                            PEI_Comentarios = string.Empty,
                            PEI_Direccion = conductor.PersonaInterna.Direccion,
                            PEI_IdCargo = conductor.PersonaInterna.IdCargo,
                            PEI_IdRegionalAdm = conductor.PersonaInterna.IdRegionalAdministrativa,
                            PEI_Identificacion = conductor.PersonaInterna.Identificacion.Trim(),
                            PEI_IdTipoIdentificacion = conductor.PersonaInterna.IdTipoIdentificacion,
                            PEI_Municipio = conductor.Ciudad.IdLocalidad,
                            PEI_Nombre = conductor.PersonaInterna.Nombre,
                            PEI_PrimerApellido = conductor.PersonaInterna.PrimerApellido,
                            PEI_SegundoApellido = conductor.PersonaInterna.SegundoApellido,
                            PEI_Telefono = conductor.PersonaInterna.Telefono,
                            PEI_CreadoPor = ControllerContext.Current.Usuario,
                            PEI_FechaGrabacion = DateTime.Now,
                            PEI_Email = string.Empty,
                        };
                        contexto.PersonaInterna_PAR.Add(personaBD);
                    }
                    else
                    {
                        personaBD.PEI_Direccion = conductor.PersonaInterna.Direccion;
                        personaBD.PEI_IdCargo = conductor.PersonaInterna.IdCargo;
                        personaBD.PEI_IdRegionalAdm = conductor.PersonaInterna.IdRegionalAdministrativa;
                        personaBD.PEI_Identificacion = conductor.PersonaInterna.Identificacion.Trim();
                        personaBD.PEI_IdTipoIdentificacion = conductor.PersonaInterna.IdTipoIdentificacion;
                        personaBD.PEI_Municipio = conductor.Ciudad.IdLocalidad;
                        personaBD.PEI_Nombre = conductor.PersonaInterna.Nombre;
                        personaBD.PEI_PrimerApellido = conductor.PersonaInterna.PrimerApellido;
                        personaBD.PEI_SegundoApellido = conductor.PersonaInterna.SegundoApellido;
                        personaBD.PEI_Telefono = conductor.PersonaInterna.Telefono;

                        PORepositorioAudit.MapearAuditPersonaInterna(contexto);
                    }

                    conducBD.CON_EsContratista = conductor.EsContratista;
                    conducBD.CON_Estado = conductor.Estado;
                    conducBD.CON_FechaIngreso = conductor.FechaIngreso;
                    conducBD.CON_FechaTerminacionContrato = conductor.FechaTerminacionContrato;
                    conducBD.CON_FechaVencimientoPase = conductor.FechaVencimientoPase;
                    conducBD.CON_NumeroPase = conductor.NumeroPase;
                    conducBD.CON_Telefono2 = conductor.Telefono2;
                    conducBD.CON_TipoContrato = conductor.TipoContrato.Descripcion;
                    conducBD.CON_IdPersonaInterna = personaBD.PEI_IdPersonaInterna;

                    PORepositorioAudit.MapearAuditModificarConductor(contexto);
                }

                if (conductor.Racoles != null && conductor.Racoles.Any())
                {
                    ///Elimina los racol asociados a un condutor
                    List<ConductorRegionalAdmi_CPO> racoles = contexto.ConductorRegionalAdmi_CPO.Where(a => a.CRA_IdConductor == conductor.IdConductor).ToList();

                    for (int i = racoles.Count - 1; i >= 0; i--)
                    {
                        contexto.ConductorRegionalAdmi_CPO.Remove(racoles[i]);
                    }

                    //agregar los racol asociados
                    conductor.Racoles.ForEach(c =>
                    {
                        contexto.ConductorRegionalAdmi_CPO.Add(new ConductorRegionalAdmi_CPO()
                        {
                            CRA_IdRegionalAdm = c.IdRegionalAdmin,
                            CRA_IdConductor = conductor.IdConductor,
                            CRA_CreadoPor = ControllerContext.Current.Usuario,
                            CRA_FechaGrabacion = DateTime.Now
                        });

                        PORepositorioAudit.MapearAuditConductorRegionalAdmin(contexto);
                    });
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<POEstado> ObtenerEstados()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoActivoInactivo_VFRM.Select(obj => new POEstado()
                {
                    Descripcion = obj.Estado,
                    IdEstado = obj.IdEstado
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene la lista de racol
        /// </summary>
        /// <returns></returns>
        public IList<PURegionalAdministrativa> ObtenerRacol()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RegionalAdministrativa_VCPO.Where(r => r.CES_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList().
                  ConvertAll<PURegionalAdministrativa>(r => new PURegionalAdministrativa()
                  {
                      IdRegionalAdmin = r.REA_IdRegionalAdm,
                      CentroServicios = new PUCentroServiciosDC()
                      {
                          Nombre = r.CES_Nombre
                      }
                  });
            }
        }

        public List<POTerritorial> ObtenerTodasTerritoriales()
        {
            List<POTerritorial> result = new List<POTerritorial>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTerritoriales_REC", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    POTerritorial item = new POTerritorial()
                    {
                        IdTerritorial = Convert.ToInt16(reader["TER_IdTerritorial"]),
                        DescripcionTerritorial = reader["TER_NombreTerritorial"].ToString()
                    };

                    result.Add(item);
                }

            }

            return result;
        }


        #endregion Conductores

        #region Vehiculos

        /// <summary>
        /// Obtiene  los vehiculos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los vehiculos</returns>
        public IList<POVehiculo> ObtenerVehiculos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ReturnVehiculo = contexto.ConsultarContainsVehiculos_VCPO(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().

                 ConvertAll<POVehiculo>(c =>
               {
                   POVehiculo vehiculo = new POVehiculo()
                   {
                       Capacidad = c.VEH_Capacidad,
                       Cilindraje = c.VEH_Cilindraje.Value,
                       Estado = c.VEH_Estado,
                       EstadoBool = c.VEH_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                       IdColorVehiculo = c.VEH_IdColorVehiculo,
                       CiudadExpedicionPlaca = new PALocalidadDC() { IdLocalidad = c.VEH_IdLocalidadExpedicionPlaca, Nombre = c.LOC_NombreLocalidadExpedicionPlaca },
                       CiudadUbicacion = new PALocalidadDC() { IdLocalidad = c.VEH_IdLocalidadUbicacion, Nombre = c.LOC_NombreLocalidadUbicacion },
                       IdMarcaVehiculo = c.VEH_IdMarcaVehiculo.Value,
                       NombreMarcaVehiculo = c.MVH_Descripcion,
                       IdVehiculo = c.VEH_IdVehiculo,
                       Modelo = c.VEH_Modelo,
                       NumeroMotor = c.VEH_NumeroMotor,
                       Placa = c.VEH_Placa,
                       NumeroSerie = c.VEH_Serie,
                       IdTipoContrato = c.VEH_IdTipoContrato,
                       NombreLocalidadUbicacion = c.LOC_NombreLocalidadUbicacion,
                       IdentificacionPropietario = c.PEE_Identificacion,
                       NombrePropietario = c.PEE_PrimerNombre + " " + c.PEE_PrimerApellido,
                       IdTipoVehiculo = c.VEH_IdTipoVehiculo,
                       ReportarSatrack = c.VEH_ReportarSatrack,

                       PropietarioVehiculo = new POPropietarioVehiculo()
                       {
                           IdPropietarioVehiculo = c.PRV_IdPropietarioVehiculo,
                           IdTipoContrato = c.PRV_IdTipoContrato,
                           CiudadPropietario = new PALocalidadDC()
                           {
                               IdLocalidad = c.PEE_Municipio,
                               Nombre = c.LOC_NombreCompleto_PE
                           },
                           PersonaExterna = new PAPersonaExterna()
                           {
                               NombreCompleto = c.PEE_PrimerNombre + " " + c.PEE_PrimerApellido,
                               Identificacion = c.PEE_Identificacion,
                               DigitoVerificacion = c.PEE_DigitoVerificacion,
                               Direccion = c.PEE_Direccion,
                               EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                               FechaExpedicionDocumento = c.PEE_FechaExpedicionDocumento,
                               IdPersonaExterna = c.PRV_IdPropietarioVehiculo,
                               IdTipoIdentificacion = c.PEE_IdTipoIdentificacion,
                               Municipio = c.PEE_Municipio,
                               NombreMunicipio = c.LOC_NombreCompleto_PE,
                               NumeroCelular = c.PEE_NumeroCelular,
                               PrimerApellido = c.PEE_PrimerApellido,
                               PrimerNombre = c.PEE_PrimerNombre,
                               SegundoApellido = c.PEE_SegundoApellido,
                               SegundoNombre = c.PEE_SegundoNombre,
                               Telefono = c.PEE_Telefono
                           }
                       },
                       EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                   };

                   ///datos extra cuando el tipo de vehiculo es carro
                   Carro_CPO carro = contexto.Carro_CPO.Where(ca => ca.CAR_IdVehiculo == vehiculo.IdVehiculo).FirstOrDefault();
                   if (carro != null)
                   {
                       vehiculo.IdLineaVehiculo = carro.CAR_IdLineaVehiculo;
                       vehiculo.ModeloRepotenciado = carro.CAR_ModeloRepontenciado;
                       vehiculo.IdTipoCarroceria = carro.CAR_IdTipoCarroceria;
                       vehiculo.PesoBrutoTaraVacio = carro.CAR_PesoBrutoTaraVacio;
                       vehiculo.RegistroNacionalCarga = carro.CAR_RegistroNacionalCarga;
                       vehiculo.IdTablaConfiguracionCarro = carro.CAR_IdTablaConfiguracionCarro;
                       vehiculo.IdTipoCombustible = carro.CAR_IdTipoCombustible;
                       TablaConfiguracionCarro_CPO config = contexto.TablaConfiguracionCarro_CPO.Where(t => t.TCC_IdTablaConfiguracionCarro == carro.CAR_IdTablaConfiguracionCarro).SingleOrDefault();
                       if (config != null)
                           vehiculo.DescripcionTablaConfiguracionCarro = config.TCC_Descripcion;
                   }

                   #region Datos tenedor vehiculo

                   ///Datos del tenedor del vehiculo
                   TenedorVehiculos_VCPO tenedor = contexto.TenedorVehiculos_VCPO.Where(t => t.TVV_IdVehiculo == c.VEH_IdVehiculo).FirstOrDefault();
                   if (tenedor != null)
                   {
                       vehiculo.TenedorVehiculo = new POTenedorVehiculo()
                       {
                           IdCategoriaLicencia = tenedor.TEV_IdCategoriaLicencia,
                           NumeroCelular2 = tenedor.TEV_NumeroCelular2,
                           NumeroLicencia = tenedor.TEV_NumeroLicencia,
                           CiudadTenedor = new PALocalidadDC()
                           {
                               IdLocalidad = tenedor.PEE_Municipio,
                               Nombre = tenedor.LOC_NombreCompleto_PE
                           },
                           IdTipoContrato = tenedor.VEH_IdTipoContrato,
                           IdTenedorVehiculo = tenedor.TEV_IdTenedorVehiculo,

                           PersonaExterna = new PAPersonaExterna()
                           {
                               NombreCompleto = tenedor.PEE_PrimerNombre + " " + tenedor.PEE_PrimerApellido,
                               Identificacion = tenedor.PEE_Identificacion,
                               DigitoVerificacion = tenedor.PEE_DigitoVerificacion,
                               Direccion = tenedor.PEE_Direccion,
                               EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                               FechaExpedicionDocumento = tenedor.PEE_FechaExpedicionDocumento,
                               IdPersonaExterna = tenedor.PEE_IdPersonaExterna,
                               IdTipoIdentificacion = tenedor.PEE_IdTipoIdentificacion,
                               Municipio = tenedor.PEE_Municipio,
                               NombreMunicipio = tenedor.LOC_NombreCompleto_PE,
                               NumeroCelular = tenedor.PEE_NumeroCelular,
                               PrimerApellido = tenedor.PEE_PrimerApellido,
                               PrimerNombre = tenedor.PEE_PrimerNombre,
                               SegundoApellido = tenedor.PEE_SegundoApellido,
                               SegundoNombre = tenedor.PEE_SegundoNombre,
                               Telefono = tenedor.PEE_Telefono
                           },
                           EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                       };
                   }
                   else
                   {
                       vehiculo.TenedorVehiculo = new POTenedorVehiculo()
                       {
                           PersonaExterna = new PAPersonaExterna()
                       };
                   }

                   #endregion Datos tenedor vehiculo

                   #region datos seguros

                   ///datos de la poliza de seguro SOAT
                   var polizaSoat = contexto.PolizaSeguroVehiculo_CPO.Where(p => p.PSV_IdVehiculo == c.VEH_IdVehiculo).Join(
                   contexto.PolizaSeguro_CPO.Where(p => p.POS_IdTipoPolizaSerguro == POConstantesParametrosOperacion.TipoPolizaSeguro_SOAT),
                   pv => pv.PSV_IdPolizaSeguro, p => p.POS_IdPolizaSeguro, (pv, p) =>
                     new
                     {
                         pv,
                         p
                     }).FirstOrDefault();

                   if (polizaSoat != null)
                   {
                       vehiculo.PolizaSeguroSoat = new POPolizaSeguroVehiculo()
                       {
                           EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                           FechaExpedicion = polizaSoat.p.POS_FechaExpedicion,
                           FechaVencimiento = polizaSoat.p.POS_FechaVencimiento,
                           IdAseguradora = polizaSoat.p.POS_IdAseguradora,
                           IdPoliza = polizaSoat.p.POS_IdPolizaSeguro,
                           IdTipoPoliza = polizaSoat.p.POS_IdTipoPolizaSerguro,
                           IdVehiculo = polizaSoat.pv.PSV_IdVehiculo,
                           NumeroPoliza = polizaSoat.p.POS_NumeroPoliza,
                           Cobertura = polizaSoat.p.POS_Cobertura
                       };

                       Aseguradora_CPO Aseguradora = contexto.Aseguradora_CPO.Where(aseg => aseg.ASE_IdAseguradora == polizaSoat.p.POS_IdAseguradora).SingleOrDefault();
                       if (Aseguradora != null)
                       {
                           vehiculo.PolizaSeguroSoat.Aseguradora = new POAseguradora()
                           {
                               Descripcion = Aseguradora.ASE_Nombre,
                               IdAseguradora = Aseguradora.ASE_IdAseguradora,
                               Identificacion = Aseguradora.ASE_Identificacion
                           };
                       }
                   }
                   else
                   {
                       vehiculo.PolizaSeguroSoat = new POPolizaSeguroVehiculo();
                   }

                   ///datos de la poliza de seguro Todo Riesgo

                   var polizaTodoRiesgo = contexto.PolizaSeguroVehiculo_CPO.Where(p => p.PSV_IdVehiculo == c.VEH_IdVehiculo).Join(
                  contexto.PolizaSeguro_CPO.Where(p => p.POS_IdTipoPolizaSerguro == POConstantesParametrosOperacion.TipoPolizaSeguro_TodoRiesgo), pv => pv.PSV_IdPolizaSeguro, p => p.POS_IdPolizaSeguro, (pv, p) =>
                    new
                    {
                        pv,
                        p
                    }).FirstOrDefault();

                   if (polizaTodoRiesgo != null)
                   {
                       vehiculo.PolizaSeguroTodoRiesgo = new POPolizaSeguroVehiculo()
                       {
                           EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                           FechaExpedicion = polizaTodoRiesgo.p.POS_FechaExpedicion,
                           FechaVencimiento = polizaTodoRiesgo.p.POS_FechaVencimiento,
                           IdAseguradora = polizaTodoRiesgo.p.POS_IdAseguradora,
                           IdPoliza = polizaTodoRiesgo.p.POS_IdPolizaSeguro,
                           IdTipoPoliza = polizaTodoRiesgo.p.POS_IdTipoPolizaSerguro,
                           IdVehiculo = polizaTodoRiesgo.pv.PSV_IdVehiculo,
                           NumeroPoliza = polizaTodoRiesgo.p.POS_NumeroPoliza,
                           Cobertura = polizaTodoRiesgo.p.POS_Cobertura
                       };

                       Aseguradora_CPO Aseguradora = contexto.Aseguradora_CPO.Where(aseg => aseg.ASE_IdAseguradora == polizaTodoRiesgo.p.POS_IdAseguradora).SingleOrDefault();
                       if (Aseguradora != null)
                       {
                           vehiculo.PolizaSeguroTodoRiesgo.Aseguradora = new POAseguradora()
                           {
                               Descripcion = Aseguradora.ASE_Nombre,
                               IdAseguradora = Aseguradora.ASE_IdAseguradora,
                               Identificacion = Aseguradora.ASE_Identificacion.Trim()
                           };
                       }
                   }
                   else
                   {
                       vehiculo.PolizaSeguroTodoRiesgo = new POPolizaSeguroVehiculo()
                       {
                           FechaExpedicion = DateTime.Now,
                           FechaVencimiento = DateTime.Now
                       };
                   }

                   #endregion datos seguros

                   #region datos revision tecnicomencanica

                   ///Datos revision tecnicomecanica
                   RevisionMecanica_CPO revision = contexto.RevisionMecanica_CPO.Where(r => r.REM_IdVehiculo == c.VEH_IdVehiculo).SingleOrDefault();
                   if (revision != null)
                   {
                       vehiculo.RevisionMecanica = new PORevisionMecanica()
                       {
                           EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                           FechaExpedicion = revision.REM_FechaExpedicion,
                           FechaVencimiento = revision.REM_FechaVencimiento,
                           IdRevisionMecanica = revision.REM_IdRevisionMecanica,
                           Idvehiculo = revision.REM_IdVehiculo
                       };
                   }
                   else
                   {
                       vehiculo.RevisionMecanica = new PORevisionMecanica();
                   }

                   #endregion datos revision tecnicomencanica

                   #region datos operacion Mensajeros

                   if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Moto)
                   {
                       List<POMensajero> mensajeros = contexto.MensajeroVehiculo_OPU.Where(mv => mv.MEV_IdVehiculo == c.VEH_IdVehiculo).Join(contexto.Mensajero_CPO, mv => mv.MEV_IdMensajero, men => men.MEN_IdMensajero, (mv, men) =>
                         new
                         {
                             mv,
                             men
                         }).Join(contexto.PersonaInterna_PAR, men => men.men.MEN_IdPersonaInterna, perIn => perIn.PEI_IdPersonaInterna, (men, perIn) =>
                           new
                           {
                               men,
                               perIn
                           }).Join(contexto.Cargo_SEG, men => men.perIn.PEI_IdCargo, car => car.CAR_IdCargo, (men, car) =>
                              new
                              {
                                  men,
                                  car
                              }).ToList().ConvertAll<POMensajero>(m =>
                             {
                                 POMensajero mensajero = new POMensajero()
                                 {
                                     EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                                     IdMensajero = m.men.men.men.MEN_IdMensajero,
                                     IdTipoMensajero = m.men.men.men.MEN_IdTipoMensajero,
                                     PersonaInterna = new PAPersonaInternaDC()
                                     {
                                         Identificacion = m.men.perIn.PEI_Identificacion,
                                         IdTipoIdentificacion = m.men.perIn.PEI_IdTipoIdentificacion,
                                         NombreCompleto = m.men.perIn.PEI_Nombre + " " + m.men.perIn.PEI_PrimerApellido,
                                         NombreCargo = m.car.CAR_Descripcion
                                     }
                                 };
                                 return mensajero;
                             });

                       vehiculo.Mensajeros = mensajeros;
                   }
                   else
                       if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                   {
                       List<POMensajero> conductores = contexto.ConductorVehiculo_CPO.Where(mv => mv.COV_IdVehiculo == c.VEH_IdVehiculo).Join(contexto.Conductor_CPO, mv => mv.COV_IdConductor, men => men.CON_IdConductor, (mv, men) =>
                     new
                     {
                         mv,
                         men
                     }).Join(contexto.PersonaInterna_PAR, men => men.men.CON_IdPersonaInterna, perIn => perIn.PEI_IdPersonaInterna, (men, perIn) =>
                       new
                       {
                           men,
                           perIn
                       }).Join(contexto.Cargo_SEG, men => men.perIn.PEI_IdCargo, car => car.CAR_IdCargo, (men, car) =>
                          new
                          {
                              men,
                              car
                          }).ToList().ConvertAll<POMensajero>(m =>
                          {
                              POMensajero mensajero = new POMensajero()
                              {
                                  EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                                  IdMensajero = m.men.men.men.CON_IdConductor,
                                  IdTipoMensajero = -1,
                                  PersonaInterna = new PAPersonaInternaDC()
                                  {
                                      Identificacion = m.men.perIn.PEI_Identificacion,
                                      IdTipoIdentificacion = m.men.perIn.PEI_IdTipoIdentificacion,
                                      NombreCompleto = m.men.perIn.PEI_Nombre + " " + m.men.perIn.PEI_PrimerApellido,
                                      NombreCargo = m.car.CAR_Descripcion
                                  }
                              };
                              return mensajero;
                          });

                       List<POMensajero> auxiliares = contexto.MensajeroVehiculo_OPU.Where(mv => mv.MEV_IdVehiculo == c.VEH_IdVehiculo).Join(contexto.Mensajero_CPO, mv => mv.MEV_IdMensajero, men => men.MEN_IdMensajero, (mv, men) =>
                   new
                   {
                       mv,
                       men
                   }).Join(contexto.PersonaInterna_PAR, men => men.men.MEN_IdPersonaInterna, perIn => perIn.PEI_IdPersonaInterna, (men, perIn) =>
                     new
                     {
                         men,
                         perIn
                     }).Join(contexto.Cargo_SEG, men => men.perIn.PEI_IdCargo, car => car.CAR_IdCargo, (men, car) =>
                        new
                        {
                            men,
                            car
                        }).ToList().ConvertAll<POMensajero>(m =>
                        {
                            POMensajero mensajero = new POMensajero()
                            {
                                EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                                IdMensajero = m.men.men.men.MEN_IdMensajero,
                                IdTipoMensajero = m.men.men.men.MEN_IdTipoMensajero,
                                PersonaInterna = new PAPersonaInternaDC()
                                {
                                    Identificacion = m.men.perIn.PEI_Identificacion,
                                    IdTipoIdentificacion = m.men.perIn.PEI_IdTipoIdentificacion,
                                    NombreCompleto = m.men.perIn.PEI_Nombre + " " + m.men.perIn.PEI_PrimerApellido,
                                    NombreCargo = m.car.CAR_Descripcion
                                }
                            };
                            return mensajero;
                        });

                       vehiculo.Mensajeros = conductores.Union(auxiliares).ToList();
                   }

                   #endregion datos operacion Mensajeros

                   #region Datos Operacion Racol

                   List<PURegionalAdministrativa> racol = contexto.RegionalOperacionVehiculo_CPO.Where(rv => rv.ROV_IdVehiculo == c.VEH_IdVehiculo).Join(contexto.RegionalAdministrativa_VCPO.Where(ra => ra.CES_Estado == ConstantesFramework.ESTADO_ACTIVO), rv => rv.ROV_IdRegionalAdm, ra => ra.REA_IdRegionalAdm, (rv, ra) =>
                     new
                     {
                         ra,
                         rv
                     }).ToList().ConvertAll<PURegionalAdministrativa>(r =>
                       {
                           PURegionalAdministrativa rac = new PURegionalAdministrativa()
                           {
                               IdRegionalAdmin = r.ra.REA_IdRegionalAdm,
                               CentroServicios = new PUCentroServiciosDC()
                               {
                                   Nombre = r.ra.CES_Nombre
                               }
                           };
                           return rac;
                       });

                   vehiculo.Racol = racol;

                   #endregion Datos Operacion Racol

                   return vehiculo;
               });

                return ReturnVehiculo;
            }
        }


        /// <summary>
        /// Valida si ya existe un vehiculo creado a partir de la placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public bool ValidarExisteVehiculoPlaca(string placa)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.Vehiculo_CPO.Where(v => v.VEH_Placa == placa.Trim()).Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Insertar un nuevo vehiculo
        /// </summary>
        /// <param name="vehiculo"></param>
        public void AdicionarVehiculo(POVehiculo vehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaActual = DateTime.Now;

                Vehiculo_CPO vehi = new Vehiculo_CPO()
                {
                    VEH_Capacidad = vehiculo.Capacidad,
                    VEH_Cilindraje = (short)vehiculo.Cilindraje,
                    VEH_CreadoPor = ControllerContext.Current.Usuario,
                    VEH_FechaGrabacion = fechaActual,
                    VEH_Estado = vehiculo.Estado,
                    VEH_IdColorVehiculo = (short)vehiculo.IdColorVehiculo,
                    VEH_IdLocalidadExpedicionPlaca = vehiculo.CiudadExpedicionPlaca.IdLocalidad,
                    VEH_IdLocalidadUbicacion = vehiculo.CiudadUbicacion.IdLocalidad,
                    VEH_IdMarcaVehiculo = (short)vehiculo.IdMarcaVehiculo,
                    VEH_IdTipoContrato = (short)vehiculo.IdTipoContrato,
                    VEH_IdTipoVehiculo = (short)vehiculo.IdTipoVehiculo,
                    VEH_Modelo = vehiculo.Modelo,
                    VEH_NumeroMotor = vehiculo.NumeroMotor,
                    VEH_Serie = vehiculo.NumeroSerie,
                    VEH_ReportarSatrack = vehiculo.ReportarSatrack,
                };
                vehi.VEH_Placa = vehiculo.Placa.Trim().Replace(" ", "").ToUpper();

                contexto.Vehiculo_CPO.Add(vehi);
                contexto.SaveChanges();

                vehiculo.IdVehiculo = vehi.VEH_IdVehiculo;

                PersonaExterna_PAR perExtPropietario = null;

                ///Inserta info de del propietario del vehiculo
                if (vehiculo.PropietarioVehiculo != null)
                {
                    PropietarioVehiculo_CPO propietario = new PropietarioVehiculo_CPO()
                    {
                        PRV_CreadoPor = ControllerContext.Current.Usuario,
                        PRV_FechaGrabacion = fechaActual,
                        PRV_IdTipoContrato = (short)vehiculo.PropietarioVehiculo.IdTipoContrato
                    };

                    //if (vehiculo.PropietarioVehiculo.IdTipoContrato == POConstantesParametrosOperacion.TipoContrato_Contratista)
                    //{
                    //Valido la creacion de Persona Externa
                    perExtPropietario = AgregarPersonaExternaVehiculo(vehiculo, contexto, fechaActual, perExtPropietario, propietario);

                    //}
                    //else
                    //{
                    //    propietario.PRV_IdPropietarioVehiculo = vehiculo.PropietarioVehiculo.PersonaExterna.IdPersonaExterna;
                    //    var prop = contexto.PropietarioVehiculo_CPO.Where(p => p.PRV_IdPropietarioVehiculo == vehiculo.PropietarioVehiculo.PersonaExterna.IdPersonaExterna).FirstOrDefault();

                    //    if (prop == null)
                    //    {
                    //        //Valido la creacion de Persona Externa
                    //        AgregarPersonaExternaVehiculo(vehiculo, contexto, fechaActual, perExtPropietario, propietario);
                    //    }
                    //    else
                    //    {
                    //        prop.PRV_IdTipoContrato = propietario.PRV_IdTipoContrato;
                    //    }
                    //}

                    if (contexto.PropietarioVehiculoVehiculo_CPO.Where(p => p.PVV_IdPropietarioVehiculo == propietario.PRV_IdPropietarioVehiculo && p.PVV_IdVehiculo == vehi.VEH_IdVehiculo).Count() <= 0)
                    {
                        PropietarioVehiculoVehiculo_CPO propietarioVV = new PropietarioVehiculoVehiculo_CPO()
                        {
                            PVV_CreadoPor = ControllerContext.Current.Usuario,
                            PVV_FechaGrabacion = fechaActual,
                            PVV_IdPropietarioVehiculo = propietario.PRV_IdPropietarioVehiculo,
                            PVV_IdVehiculo = vehi.VEH_IdVehiculo
                        };
                        contexto.PropietarioVehiculoVehiculo_CPO.Add(propietarioVV);
                        contexto.SaveChanges();
                    }
                }

                //Inserta info del tenedor del vehiculo
                if (vehiculo.TenedorVehiculo != null)
                {
                    TenedorVehiculo_CPO tenedor = new TenedorVehiculo_CPO()
                    {
                        TEV_CreadoPor = ControllerContext.Current.Usuario,
                        TEV_FechaGrabacion = fechaActual,
                        TEV_IdCategoriaLicencia = (short)vehiculo.TenedorVehiculo.IdCategoriaLicencia,
                        TEV_NumeroCelular2 = vehiculo.TenedorVehiculo.NumeroCelular2,
                        TEV_NumeroLicencia = vehiculo.TenedorVehiculo.NumeroLicencia
                    };
                    PersonaExterna_PAR perExt = null;
                    if (vehiculo.PropietarioVehiculo.PersonaExterna.Identificacion.Trim() == vehiculo.TenedorVehiculo.PersonaExterna.Identificacion.Trim())
                        perExt = perExtPropietario;
                    else
                        perExt = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == vehiculo.TenedorVehiculo.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == vehiculo.TenedorVehiculo.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();

                    if (perExt == null)
                    {
                        perExt = new PersonaExterna_PAR()
                        {
                            PEE_CreadoPor = ControllerContext.Current.Usuario,
                            PEE_FechaGrabacion = fechaActual,
                            PEE_DigitoVerificacion = vehiculo.TenedorVehiculo.PersonaExterna.DigitoVerificacion,
                            PEE_Direccion = vehiculo.TenedorVehiculo.PersonaExterna.Direccion,
                            PEE_FechaExpedicionDocumento = vehiculo.TenedorVehiculo.PersonaExterna.FechaExpedicionDocumento,
                            PEE_Identificacion = vehiculo.TenedorVehiculo.PersonaExterna.Identificacion.Trim(),
                            PEE_IdTipoIdentificacion = vehiculo.TenedorVehiculo.PersonaExterna.IdTipoIdentificacion,
                            PEE_Municipio = vehiculo.TenedorVehiculo.CiudadTenedor.IdLocalidad,
                            PEE_NumeroCelular = vehiculo.TenedorVehiculo.PersonaExterna.NumeroCelular,
                            PEE_PrimerApellido = vehiculo.TenedorVehiculo.PersonaExterna.PrimerApellido,
                            PEE_PrimerNombre = vehiculo.TenedorVehiculo.PersonaExterna.PrimerNombre,
                            PEE_SegundoApellido = vehiculo.TenedorVehiculo.PersonaExterna.SegundoApellido,
                            PEE_SegundoNombre = vehiculo.TenedorVehiculo.PersonaExterna.SegundoNombre,
                            PEE_Telefono = vehiculo.TenedorVehiculo.PersonaExterna.Telefono
                        };

                        //contexto.PersonaExterna_PAR.Add(perExt);
                        //tenedor.TEV_IdTenedorVehiculo = perExt.PEE_IdPersonaExterna;
                        tenedor.PersonaExterna_PAR = perExt;
                        contexto.TenedorVehiculo_CPO.Add(tenedor);
                        contexto.SaveChanges();
                    }
                    else
                    {
                        tenedor.TEV_IdTenedorVehiculo = perExt.PEE_IdPersonaExterna;
                        var ten = contexto.TenedorVehiculo_CPO.Where(t => t.TEV_IdTenedorVehiculo == perExt.PEE_IdPersonaExterna).FirstOrDefault();
                        if (ten == null)
                        {
                            contexto.TenedorVehiculo_CPO.Add(tenedor);
                            contexto.SaveChanges();
                        }
                        else
                        {
                            ten.TEV_IdCategoriaLicencia = (short)vehiculo.TenedorVehiculo.IdCategoriaLicencia;
                            ten.TEV_NumeroCelular2 = vehiculo.TenedorVehiculo.NumeroCelular2;
                            ten.TEV_NumeroLicencia = vehiculo.TenedorVehiculo.NumeroLicencia;
                            contexto.SaveChanges();
                        }
                    }

                    if (contexto.TenedorVehiculoVehiculo_CPO.Where(t => t.TVV_IdTenedorVehiculo == perExt.PEE_IdPersonaExterna && t.TVV_IdVehiculo == vehi.VEH_IdVehiculo).Count() <= 0)
                    {
                        contexto.TenedorVehiculoVehiculo_CPO.Add(new TenedorVehiculoVehiculo_CPO()
                        {
                            TVV_CreadoPor = ControllerContext.Current.Usuario,
                            TVV_FechaGrabacion = fechaActual,
                            TVV_IdTenedorVehiculo = perExt.PEE_IdPersonaExterna,
                            TVV_IdVehiculo = vehi.VEH_IdVehiculo
                        });
                        contexto.SaveChanges();
                    }
                }

                if (vehiculo.PolizaSeguroSoat != null)
                {
                    PolizaSeguro_CPO poliza = new PolizaSeguro_CPO()
                    {
                        POS_CreadoPor = ControllerContext.Current.Usuario,
                        POS_FechaGrabacion = DateTime.Now,
                        POS_FechaExpedicion = vehiculo.PolizaSeguroSoat.FechaExpedicion,
                        POS_IdAseguradora = (short)vehiculo.PolizaSeguroSoat.IdAseguradora,
                        POS_FechaVencimiento = vehiculo.PolizaSeguroSoat.FechaVencimiento,
                        POS_IdTipoPolizaSerguro = POConstantesParametrosOperacion.TipoPolizaSeguro_SOAT,
                        POS_NumeroPoliza = vehiculo.PolizaSeguroSoat.NumeroPoliza,
                        POS_Cobertura = vehiculo.PolizaSeguroSoat.Cobertura,
                        PolizaSeguroVehiculo_CPO = new TrackableCollection<PolizaSeguroVehiculo_CPO>()
                    };
                    contexto.PolizaSeguro_CPO.Add(poliza);
                    contexto.SaveChanges();

                    PolizaSeguroVehiculo_CPO seguro = new PolizaSeguroVehiculo_CPO()
                    {
                        PSV_CreadoPor = ControllerContext.Current.Usuario,
                        PSV_FechaGrabacion = DateTime.Now,
                        PSV_IdVehiculo = vehi.VEH_IdVehiculo,
                        PSV_IdPolizaSeguro = poliza.POS_IdPolizaSeguro
                    };

                    poliza.PolizaSeguroVehiculo_CPO.Add(seguro);
                    contexto.SaveChanges();
                }

                if (vehiculo.PolizaSeguroTodoRiesgo != null && vehiculo.PolizaSeguroTodoRiesgo.IdAseguradora > 0 && !string.IsNullOrWhiteSpace(vehiculo.PolizaSeguroTodoRiesgo.NumeroPoliza))
                {
                    PolizaSeguro_CPO polizaTR = new PolizaSeguro_CPO()
                    {
                        POS_CreadoPor = ControllerContext.Current.Usuario,
                        POS_FechaGrabacion = fechaActual,
                        POS_FechaExpedicion = vehiculo.PolizaSeguroTodoRiesgo.FechaExpedicion,
                        POS_IdAseguradora = (short)vehiculo.PolizaSeguroTodoRiesgo.IdAseguradora,
                        POS_FechaVencimiento = vehiculo.PolizaSeguroTodoRiesgo.FechaVencimiento,
                        POS_IdTipoPolizaSerguro = POConstantesParametrosOperacion.TipoPolizaSeguro_TodoRiesgo,
                        POS_NumeroPoliza = vehiculo.PolizaSeguroTodoRiesgo.NumeroPoliza,
                        POS_Cobertura = vehiculo.PolizaSeguroTodoRiesgo.Cobertura,
                        PolizaSeguroVehiculo_CPO = new TrackableCollection<PolizaSeguroVehiculo_CPO>()
                    };
                    contexto.PolizaSeguro_CPO.Add(polizaTR);
                    contexto.SaveChanges();

                    PolizaSeguroVehiculo_CPO seguroTR = new PolizaSeguroVehiculo_CPO()
                    {
                        PSV_CreadoPor = ControllerContext.Current.Usuario,
                        PSV_FechaGrabacion = fechaActual,
                        PSV_IdVehiculo = vehi.VEH_IdVehiculo,
                        PSV_IdPolizaSeguro = polizaTR.POS_IdPolizaSeguro
                    };
                    polizaTR.PolizaSeguroVehiculo_CPO.Add(seguroTR);
                    contexto.SaveChanges();
                }

                if (vehiculo.RevisionMecanica != null)
                {
                    RevisionMecanica_CPO revision = new RevisionMecanica_CPO()
                    {
                        REM_CreadoPor = ControllerContext.Current.Usuario,
                        REM_FechaGrabacion = fechaActual,
                        REM_FechaExpedicion = vehiculo.RevisionMecanica.FechaExpedicion,
                        REM_FechaVencimiento = vehiculo.RevisionMecanica.FechaVencimiento,
                        REM_IdVehiculo = vehi.VEH_IdVehiculo
                    };
                    contexto.RevisionMecanica_CPO.Add(revision);
                    contexto.SaveChanges();
                }

                //agregar los racol asociados
                if (vehiculo.Racol != null)
                {
                    vehiculo.Racol.ForEach(c =>
                    {
                        contexto.RegionalOperacionVehiculo_CPO.Add(new RegionalOperacionVehiculo_CPO()
                        {
                            ROV_IdRegionalAdm = c.IdRegionalAdmin,
                            ROV_IdVehiculo = vehi.VEH_IdVehiculo,
                            ROV_CreadoPor = ControllerContext.Current.Usuario,
                            ROV_FechaGrabacion = DateTime.Now
                        });
                        contexto.SaveChanges();
                    });
                }

                //Agregar Mensajeros asociados
                if (vehiculo.Mensajeros != null)
                {
                    vehiculo.Mensajeros.ForEach(m =>
                    {
                        switch (m.EstadoRegistro)
                        {
                            case EnumEstadoRegistro.ADICIONADO:

                                if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Moto)
                                {
                                    if (contexto.MensajeroVehiculo_OPU.Where(t => t.MEV_IdMensajero == m.IdMensajero && t.MEV_IdVehiculo == vehiculo.IdVehiculo).Count() <= 0)
                                    {
                                        contexto.MensajeroVehiculo_OPU.Add(new MensajeroVehiculo_OPU()
                                        {
                                            MEV_IdMensajero = m.IdMensajero,
                                            MEV_IdVehiculo = vehiculo.IdVehiculo,
                                            MEV_CreadoPor = ControllerContext.Current.Usuario,
                                            MEV_FechaGrabacion = DateTime.Now
                                        });
                                        contexto.SaveChanges();
                                    }
                                }
                                else if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                                {
                                    if (m.IdTipoMensajero == POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR)
                                    {
                                        if (contexto.MensajeroVehiculo_OPU.Where(t => t.MEV_IdMensajero == m.IdMensajero && t.MEV_IdVehiculo == vehiculo.IdVehiculo).Count() <= 0)
                                        {
                                            contexto.MensajeroVehiculo_OPU.Add(new MensajeroVehiculo_OPU()
                                            {
                                                MEV_CreadoPor = ControllerContext.Current.Usuario,
                                                MEV_FechaGrabacion = DateTime.Now,
                                                MEV_IdMensajero = m.IdMensajero,
                                                MEV_IdVehiculo = vehiculo.IdVehiculo
                                            });
                                            contexto.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        if (contexto.ConductorVehiculo_CPO.Where(t => t.COV_IdConductor == m.IdMensajero && t.COV_IdVehiculo == vehiculo.IdVehiculo).Count() <= 0)
                                        {
                                            contexto.ConductorVehiculo_CPO.Add(new ConductorVehiculo_CPO()
                                            {
                                                COV_CreadoPor = ControllerContext.Current.Usuario,
                                                COV_FechaGrabacion = DateTime.Now,
                                                COV_IdConductor = m.IdMensajero,
                                                COV_IdVehiculo = vehiculo.IdVehiculo
                                            });
                                            contexto.SaveChanges();
                                        }
                                    }
                                }
                                break;
                        }
                    });
                }

                if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                {
                    Carro_CPO carro = new Carro_CPO()
                    {
                        CAR_CreadoPor = ControllerContext.Current.Usuario,
                        CAR_FechaGrabacion = fechaActual,
                        CAR_IdLineaVehiculo = (short)vehiculo.IdLineaVehiculo,
                        CAR_IdTablaConfiguracionCarro = (short)vehiculo.IdTablaConfiguracionCarro,
                        CAR_RegistroNacionalCarga = (short)vehiculo.RegistroNacionalCarga,
                        CAR_PesoBrutoTaraVacio = (short)vehiculo.PesoBrutoTaraVacio,
                        CAR_IdTipoCarroceria = (short)vehiculo.IdTipoCarroceria,
                        CAR_ModeloRepontenciado = vehiculo.ModeloRepotenciado,
                        CAR_IdVehiculo = vehi.VEH_IdVehiculo,
                        CAR_IdTipoCombustible = vehiculo.IdTipoCombustible
                    };
                    contexto.Carro_CPO.Add(carro);
                }

                contexto.SaveChanges();
            }
        }

        private PersonaExterna_PAR AgregarPersonaExternaVehiculo(POVehiculo vehiculo, ModeloParametrosOperacion contexto, DateTime fechaActual, PersonaExterna_PAR perExtPropietario, PropietarioVehiculo_CPO propietario)
        {
            perExtPropietario = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == vehiculo.PropietarioVehiculo.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == vehiculo.PropietarioVehiculo.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();

            if (perExtPropietario == null)
            {
                perExtPropietario = new PersonaExterna_PAR()
                {
                    PEE_CreadoPor = ControllerContext.Current.Usuario,
                    PEE_FechaGrabacion = fechaActual,
                    PEE_DigitoVerificacion = vehiculo.PropietarioVehiculo.PersonaExterna.DigitoVerificacion,
                    PEE_Direccion = vehiculo.PropietarioVehiculo.PersonaExterna.Direccion,
                    PEE_FechaExpedicionDocumento = vehiculo.PropietarioVehiculo.PersonaExterna.FechaExpedicionDocumento,
                    PEE_Identificacion = vehiculo.PropietarioVehiculo.PersonaExterna.Identificacion.Trim(),
                    PEE_IdTipoIdentificacion = vehiculo.PropietarioVehiculo.PersonaExterna.IdTipoIdentificacion,
                    PEE_Municipio = vehiculo.PropietarioVehiculo.CiudadPropietario.IdLocalidad,
                    PEE_NumeroCelular = vehiculo.PropietarioVehiculo.PersonaExterna.NumeroCelular,
                    PEE_PrimerApellido = vehiculo.PropietarioVehiculo.PersonaExterna.PrimerApellido,
                    PEE_PrimerNombre = vehiculo.PropietarioVehiculo.PersonaExterna.PrimerNombre,
                    PEE_SegundoApellido = vehiculo.PropietarioVehiculo.PersonaExterna.SegundoApellido,
                    PEE_SegundoNombre = vehiculo.PropietarioVehiculo.PersonaExterna.SegundoNombre,
                    PEE_Telefono = vehiculo.PropietarioVehiculo.PersonaExterna.Telefono,
                };

                propietario.PersonaExterna_PAR = perExtPropietario;
                contexto.PropietarioVehiculo_CPO.Add(propietario);
                contexto.SaveChanges();
            }
            else
            {
                perExtPropietario.PEE_DigitoVerificacion = vehiculo.PropietarioVehiculo.PersonaExterna.DigitoVerificacion;
                perExtPropietario.PEE_Direccion = vehiculo.PropietarioVehiculo.PersonaExterna.Direccion;
                perExtPropietario.PEE_FechaExpedicionDocumento = vehiculo.PropietarioVehiculo.PersonaExterna.FechaExpedicionDocumento;
                perExtPropietario.PEE_Identificacion = vehiculo.PropietarioVehiculo.PersonaExterna.Identificacion.Trim();
                perExtPropietario.PEE_IdTipoIdentificacion = vehiculo.PropietarioVehiculo.PersonaExterna.IdTipoIdentificacion;
                perExtPropietario.PEE_Municipio = vehiculo.PropietarioVehiculo.CiudadPropietario.IdLocalidad;
                perExtPropietario.PEE_NumeroCelular = vehiculo.PropietarioVehiculo.PersonaExterna.NumeroCelular;
                perExtPropietario.PEE_PrimerApellido = vehiculo.PropietarioVehiculo.PersonaExterna.PrimerApellido;
                perExtPropietario.PEE_PrimerNombre = vehiculo.PropietarioVehiculo.PersonaExterna.PrimerNombre;
                perExtPropietario.PEE_SegundoApellido = vehiculo.PropietarioVehiculo.PersonaExterna.SegundoApellido;
                perExtPropietario.PEE_SegundoNombre = vehiculo.PropietarioVehiculo.PersonaExterna.SegundoNombre;
                perExtPropietario.PEE_Telefono = vehiculo.PropietarioVehiculo.PersonaExterna.Telefono;

                propietario.PRV_IdPropietarioVehiculo = perExtPropietario.PEE_IdPersonaExterna;
                if (contexto.PropietarioVehiculo_CPO.Where(p => p.PRV_IdPropietarioVehiculo == perExtPropietario.PEE_IdPersonaExterna).Count() <= 0)
                {
                    contexto.PropietarioVehiculo_CPO.Add(propietario);
                    contexto.SaveChanges();
                }
            }
            return perExtPropietario;
        }

        /// <summary>
        /// Edita la informacion un vehiculo y sus datos asociados
        /// </summary>
        /// <param name="vehiculo"></param>
        public void EditarVehiculo(POVehiculo vehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Vehiculo_CPO vehi = contexto.Vehiculo_CPO.Where(v => v.VEH_IdVehiculo == vehiculo.IdVehiculo).SingleOrDefault();

                if (vehi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_OPERATIVOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                vehi.VEH_Capacidad = vehiculo.Capacidad;
                vehi.VEH_Cilindraje = (short)vehiculo.Cilindraje;
                vehi.VEH_Estado = vehiculo.Estado;
                vehi.VEH_IdColorVehiculo = (short)vehiculo.IdColorVehiculo;
                vehi.VEH_IdLocalidadExpedicionPlaca = vehiculo.CiudadExpedicionPlaca != null ? vehiculo.CiudadExpedicionPlaca.IdLocalidad : vehi.VEH_IdLocalidadExpedicionPlaca;
                vehi.VEH_IdLocalidadUbicacion = vehiculo.CiudadUbicacion != null ? vehiculo.CiudadUbicacion.IdLocalidad : vehi.VEH_IdLocalidadUbicacion;
                vehi.VEH_IdMarcaVehiculo = (short)vehiculo.IdMarcaVehiculo;
                vehi.VEH_IdTipoContrato = (short)vehiculo.IdTipoContrato;
                vehi.VEH_IdTipoVehiculo = (short)vehiculo.IdTipoVehiculo;
                vehi.VEH_Modelo = vehiculo.Modelo;
                vehi.VEH_NumeroMotor = vehiculo.NumeroMotor;
                vehi.VEH_Placa = vehiculo.Placa.Trim().Replace(" ", "").ToUpper();
                vehi.VEH_Serie = vehiculo.NumeroSerie;
                vehi.VEH_ReportarSatrack = vehiculo.ReportarSatrack;

                DateTime fechaActual = DateTime.Now;

                if (vehiculo.PolizaSeguroSoat != null)
                {
                    PolizaSeguro_CPO poliza = contexto.PolizaSeguro_CPO.Where(p => p.POS_IdPolizaSeguro == vehiculo.PolizaSeguroSoat.IdPoliza).SingleOrDefault();
                    if (poliza == null)
                    {
                        poliza = new PolizaSeguro_CPO()
                        {
                            POS_CreadoPor = ControllerContext.Current.Usuario,
                            POS_FechaGrabacion = DateTime.Now,
                            POS_FechaExpedicion = vehiculo.PolizaSeguroSoat.FechaExpedicion,
                            POS_IdAseguradora = (short)vehiculo.PolizaSeguroSoat.IdAseguradora,
                            POS_FechaVencimiento = vehiculo.PolizaSeguroSoat.FechaVencimiento,
                            POS_IdTipoPolizaSerguro = POConstantesParametrosOperacion.TipoPolizaSeguro_SOAT,
                            POS_NumeroPoliza = vehiculo.PolizaSeguroSoat.NumeroPoliza,
                            POS_Cobertura = vehiculo.PolizaSeguroSoat.Cobertura,
                            PolizaSeguroVehiculo_CPO = new TrackableCollection<PolizaSeguroVehiculo_CPO>()
                        };
                        contexto.PolizaSeguro_CPO.Add(poliza);
                        PolizaSeguroVehiculo_CPO seguro = new PolizaSeguroVehiculo_CPO()
                        {
                            PSV_CreadoPor = ControllerContext.Current.Usuario,
                            PSV_FechaGrabacion = DateTime.Now,
                            PSV_IdVehiculo = vehi.VEH_IdVehiculo,
                            PSV_IdPolizaSeguro = poliza.POS_IdPolizaSeguro
                        };

                        poliza.PolizaSeguroVehiculo_CPO.Add(seguro);
                    }
                    else
                    {
                        poliza.POS_FechaExpedicion = vehiculo.PolizaSeguroSoat.FechaExpedicion;
                        poliza.POS_IdAseguradora = (short)vehiculo.PolizaSeguroSoat.IdAseguradora;
                        poliza.POS_FechaVencimiento = vehiculo.PolizaSeguroSoat.FechaVencimiento;
                        poliza.POS_IdTipoPolizaSerguro = POConstantesParametrosOperacion.TipoPolizaSeguro_SOAT;
                        poliza.POS_NumeroPoliza = vehiculo.PolizaSeguroSoat.NumeroPoliza;
                        poliza.POS_Cobertura = vehiculo.PolizaSeguroSoat.Cobertura;

                        PORepositorioAudit.MapearAuditSeguro(contexto);
                    }
                }

                if (vehiculo.PolizaSeguroTodoRiesgo != null && vehiculo.PolizaSeguroTodoRiesgo.IdAseguradora > 0 && !string.IsNullOrWhiteSpace(vehiculo.PolizaSeguroTodoRiesgo.NumeroPoliza))
                {
                    PolizaSeguro_CPO poliza = contexto.PolizaSeguro_CPO.Where(p => p.POS_IdPolizaSeguro == vehiculo.PolizaSeguroTodoRiesgo.IdPoliza).SingleOrDefault();
                    if (poliza == null)
                    {
                        PolizaSeguro_CPO polizaTR = new PolizaSeguro_CPO()
                        {
                            POS_CreadoPor = ControllerContext.Current.Usuario,
                            POS_FechaGrabacion = DateTime.Now,
                            POS_FechaExpedicion = vehiculo.PolizaSeguroTodoRiesgo.FechaExpedicion,
                            POS_IdAseguradora = (short)vehiculo.PolizaSeguroTodoRiesgo.IdAseguradora,
                            POS_FechaVencimiento = vehiculo.PolizaSeguroTodoRiesgo.FechaVencimiento,
                            POS_IdTipoPolizaSerguro = POConstantesParametrosOperacion.TipoPolizaSeguro_TodoRiesgo,
                            POS_NumeroPoliza = vehiculo.PolizaSeguroTodoRiesgo.NumeroPoliza,
                            POS_Cobertura = vehiculo.PolizaSeguroTodoRiesgo.Cobertura,
                            PolizaSeguroVehiculo_CPO = new TrackableCollection<PolizaSeguroVehiculo_CPO>()
                        };
                        contexto.PolizaSeguro_CPO.Add(polizaTR);
                        PolizaSeguroVehiculo_CPO seguroTR = new PolizaSeguroVehiculo_CPO()
                        {
                            PSV_CreadoPor = ControllerContext.Current.Usuario,
                            PSV_FechaGrabacion = DateTime.Now,
                            PSV_IdVehiculo = vehi.VEH_IdVehiculo,
                            PSV_IdPolizaSeguro = polizaTR.POS_IdPolizaSeguro
                        };
                        polizaTR.PolizaSeguroVehiculo_CPO.Add(seguroTR);
                    }
                    else
                    {
                        poliza.POS_FechaExpedicion = vehiculo.PolizaSeguroTodoRiesgo.FechaExpedicion;
                        poliza.POS_IdAseguradora = (short)vehiculo.PolizaSeguroTodoRiesgo.IdAseguradora;
                        poliza.POS_FechaVencimiento = vehiculo.PolizaSeguroTodoRiesgo.FechaVencimiento;
                        poliza.POS_IdTipoPolizaSerguro = POConstantesParametrosOperacion.TipoPolizaSeguro_TodoRiesgo;
                        poliza.POS_NumeroPoliza = vehiculo.PolizaSeguroTodoRiesgo.NumeroPoliza;
                        poliza.POS_Cobertura = vehiculo.PolizaSeguroTodoRiesgo.Cobertura;
                        PORepositorioAudit.MapearAuditSeguro(contexto);
                    }
                }

                if (vehiculo.RevisionMecanica != null)
                {
                    RevisionMecanica_CPO revision = contexto.RevisionMecanica_CPO.Where(r => r.REM_IdRevisionMecanica == vehiculo.RevisionMecanica.IdRevisionMecanica).SingleOrDefault();
                    if (revision == null)
                    {
                        revision = new RevisionMecanica_CPO()
                        {
                            REM_CreadoPor = ControllerContext.Current.Usuario,
                            REM_FechaGrabacion = DateTime.Now,
                            REM_FechaExpedicion = vehiculo.RevisionMecanica.FechaExpedicion,
                            REM_FechaVencimiento = vehiculo.RevisionMecanica.FechaVencimiento,
                            REM_IdVehiculo = vehi.VEH_IdVehiculo
                        };
                        contexto.RevisionMecanica_CPO.Add(revision);
                    }
                    else
                    {
                        revision.REM_FechaExpedicion = vehiculo.RevisionMecanica.FechaExpedicion;
                        revision.REM_FechaVencimiento = vehiculo.RevisionMecanica.FechaVencimiento;
                        revision.REM_IdVehiculo = vehiculo.IdVehiculo;
                        PORepositorioAudit.MapearAuditRevisionMecanica(contexto);
                    }
                }

                if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                {
                    Carro_CPO carro = contexto.Carro_CPO.Where(c => c.CAR_IdVehiculo == vehiculo.IdVehiculo).SingleOrDefault();
                    if (carro == null)
                    {
                        carro = new Carro_CPO()
                        {
                            CAR_CreadoPor = ControllerContext.Current.Usuario,
                            CAR_FechaGrabacion = DateTime.Now,
                            CAR_IdLineaVehiculo = (short)vehiculo.IdLineaVehiculo,
                            CAR_IdTablaConfiguracionCarro = (short)vehiculo.IdTablaConfiguracionCarro,
                            CAR_RegistroNacionalCarga = (short)vehiculo.RegistroNacionalCarga,
                            CAR_PesoBrutoTaraVacio = (short)vehiculo.PesoBrutoTaraVacio,
                            CAR_IdTipoCarroceria = (short)vehiculo.IdTipoCarroceria,
                            CAR_ModeloRepontenciado = vehiculo.ModeloRepotenciado,
                            CAR_IdVehiculo = vehi.VEH_IdVehiculo,
                            CAR_IdTipoCombustible = vehiculo.IdTipoCombustible
                        };
                        contexto.Carro_CPO.Add(carro);
                    }
                    else
                    {
                        carro.CAR_IdLineaVehiculo = (short)vehiculo.IdLineaVehiculo;
                        carro.CAR_IdTablaConfiguracionCarro = (short)vehiculo.IdTablaConfiguracionCarro;
                        carro.CAR_RegistroNacionalCarga = (short)vehiculo.RegistroNacionalCarga;
                        carro.CAR_PesoBrutoTaraVacio = (short)vehiculo.PesoBrutoTaraVacio;
                        carro.CAR_IdTipoCarroceria = (short)vehiculo.IdTipoCarroceria;
                        carro.CAR_ModeloRepontenciado = vehiculo.ModeloRepotenciado;
                        carro.CAR_IdVehiculo = vehiculo.IdVehiculo;
                        carro.CAR_IdTipoCombustible = vehiculo.IdTipoCombustible;
                    }
                }
                else
                {
                    Carro_CPO carro = contexto.Carro_CPO.Where(c => c.CAR_IdVehiculo == vehiculo.IdVehiculo).SingleOrDefault();
                    if (carro != null)
                    {
                        contexto.Carro_CPO.Remove(carro);
                    }
                }

                ///Editar racoles asociados
                if (vehiculo.Racol != null && vehiculo.Racol.Count > 0)
                {
                    ///Elimina los racol asociados a un vahiculo
                    List<RegionalOperacionVehiculo_CPO> racoles = contexto.RegionalOperacionVehiculo_CPO.Where(a => a.ROV_IdVehiculo == vehiculo.IdVehiculo).ToList();

                    for (int i = racoles.Count - 1; i >= 0; i--)
                    {
                        contexto.RegionalOperacionVehiculo_CPO.Remove(racoles[i]);
                    }

                    //agregar los racol asociados
                    vehiculo.Racol.ForEach(c =>
                    {
                        contexto.RegionalOperacionVehiculo_CPO.Add(new RegionalOperacionVehiculo_CPO()
                        {
                            ROV_IdRegionalAdm = c.IdRegionalAdmin,
                            ROV_IdVehiculo = vehiculo.IdVehiculo,
                            ROV_CreadoPor = ControllerContext.Current.Usuario,
                            ROV_FechaGrabacion = DateTime.Now
                        });
                    });
                }

                //Editar Mensajeros asociados
                if (vehiculo.Mensajeros != null && vehiculo.Mensajeros.Any())
                {
                    vehiculo.Mensajeros.ForEach(m =>
                      {
                          switch (m.EstadoRegistro)
                          {
                              case EnumEstadoRegistro.ADICIONADO:

                                  if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Moto)
                                  {
                                      if (contexto.MensajeroVehiculo_OPU.Where(t => t.MEV_IdMensajero == m.IdMensajero && t.MEV_IdVehiculo == vehiculo.IdVehiculo).Count() <= 0)
                                      {
                                          contexto.MensajeroVehiculo_OPU.Add(new MensajeroVehiculo_OPU()
                                          {
                                              MEV_IdMensajero = m.IdMensajero,
                                              MEV_IdVehiculo = vehiculo.IdVehiculo,
                                              MEV_CreadoPor = ControllerContext.Current.Usuario,
                                              MEV_FechaGrabacion = DateTime.Now
                                          });
                                      }
                                  }
                                  else if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                                  {
                                      if (m.IdTipoMensajero == POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR)
                                      {
                                          if (!contexto.MensajeroVehiculo_OPU.Where(t => t.MEV_IdMensajero == m.IdMensajero && t.MEV_IdVehiculo == vehiculo.IdVehiculo).Any())
                                          {
                                              contexto.MensajeroVehiculo_OPU.Add(new MensajeroVehiculo_OPU()
                                              {
                                                  MEV_CreadoPor = ControllerContext.Current.Usuario,
                                                  MEV_FechaGrabacion = DateTime.Now,
                                                  MEV_IdMensajero = m.IdMensajero,
                                                  MEV_IdVehiculo = vehiculo.IdVehiculo
                                              });
                                          }
                                      }
                                      else
                                      {
                                          if (!contexto.ConductorVehiculo_CPO.Where(t => t.COV_IdConductor == m.IdMensajero && t.COV_IdVehiculo == vehiculo.IdVehiculo).Any())
                                          {
                                              contexto.ConductorVehiculo_CPO.Add(new ConductorVehiculo_CPO()
                                              {
                                                  COV_CreadoPor = ControllerContext.Current.Usuario,
                                                  COV_FechaGrabacion = DateTime.Now,
                                                  COV_IdConductor = m.IdMensajero,
                                                  COV_IdVehiculo = vehiculo.IdVehiculo
                                              });
                                          }
                                      }
                                  }
                                  break;

                              case EnumEstadoRegistro.BORRADO:
                                  if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Moto)
                                  {
                                      MensajeroVehiculo_OPU mensajeroVehiculo = contexto.MensajeroVehiculo_OPU.Where(mv => mv.MEV_IdMensajero == m.IdMensajero && mv.MEV_IdVehiculo == vehiculo.IdVehiculo).SingleOrDefault();
                                      if (mensajeroVehiculo != null)
                                      {
                                          contexto.MensajeroVehiculo_OPU.Remove(mensajeroVehiculo);
                                      }
                                  }
                                  else if (vehiculo.IdTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Carro)
                                  {
                                      if (m.IdTipoMensajero == POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR)
                                      {
                                          MensajeroVehiculo_OPU auxiliarVehiculo = contexto.MensajeroVehiculo_OPU.Where(av => av.MEV_IdMensajero == m.IdMensajero && av.MEV_IdVehiculo == vehiculo.IdVehiculo).SingleOrDefault();

                                          if (auxiliarVehiculo != null)
                                          {
                                              contexto.MensajeroVehiculo_OPU.Remove(auxiliarVehiculo);
                                          }
                                      }
                                      else
                                      {
                                          ConductorVehiculo_CPO conductorVehiculo = contexto.ConductorVehiculo_CPO.Where(cv => cv.COV_IdConductor == m.IdMensajero && cv.COV_IdVehiculo == vehiculo.IdVehiculo).SingleOrDefault();
                                          if (conductorVehiculo != null)
                                          {
                                              contexto.ConductorVehiculo_CPO.Remove(conductorVehiculo);
                                              PORepositorioAudit.MapearAuditConductorVehiculo(contexto);
                                          }
                                      }
                                  }
                                  break;
                          }
                      });
                }

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene todas las marcas de los vehiculos
        /// </summary>
        /// <returns>Lista con las marcas de los vehiculos</returns>
        public IList<POMarca> ObtenerTodosMarcaVehiculo()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MarcaVehiculo_CPO.Select(obj => new POMarca()
                {
                    Descripcion = obj.MVH_Descripcion,
                    IdMarca = obj.MVH_IdMarcaVehiculo
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los colores de los vehiculos
        /// </summary>
        /// <returns></returns>
        public IList<POColor> ObtenerTodosColor()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ColorVehiculo_CPO.Select(obj => new POColor()
                {
                    Descripcion = obj.CVH_Descripcion,
                    IdColor = obj.CVH_IdColorVehiculo
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene las lineas de los vehiculos
        /// </summary>
        /// <returns></returns>
        public IList<POLinea> ObtenerTodosLinea()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.LineaVehiculo_CPO.Select(obj => new POLinea()
                {
                    Descripcion = obj.LVH_Descripcion,
                    IdLineaVehiculo = obj.LVH_IdLineaVehiculo
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las configuraciones de los vehivulos
        /// </summary>
        /// <returns></returns>
        public IList<POConfiguracionVehiculo> ObtenerTodosConfiguracionVehiculo()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TablaConfiguracionCarro_CPO.Select(obj => new POConfiguracionVehiculo()
                {
                    Descripcion = obj.TCC_Descripcion,
                    IdConfiguracionVehiculo = obj.TCC_IdTablaConfiguracionCarro
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de carroceria
        /// </summary>
        /// <returns></returns>
        public IList<POTipoCarroceria> ObtenerTodosTipoCarroceria()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoCarroceria_CPO.Select(obj => new POTipoCarroceria()
                {
                    Descripcion = obj.TIC_Descripcion,
                    IdTipoCarroceria = obj.TIC_IdTipoCarroceria
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }


        /// <summary>
        /// Obtiene todos los tipos de combustible para carros
        /// </summary>
        /// <returns></returns>
        public IList<POTipoCombustibleDC> ObtenerTodosTiposCombustible()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoCombustible_CPO.Select(obj => new POTipoCombustibleDC()
                {
                    Descripcion = obj.TCO_Descripcion,
                    IdTipoCombustible = obj.TCO_IdTipoCombustible
                }).OrderBy(obj => obj.IdTipoCombustible).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de contrato
        /// </summary>
        /// <returns></returns>
        public IList<POTipoContrato> ObtenerTodosTipoContrato()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoContrato_CPO.Select(obj => new POTipoContrato()
                {
                    Descripcion = obj.TIC_Descripcion.ToUpper(),
                    IdTipoContrato = obj.TIC_IdTipoContrato
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las categorias de licencia de conduccion
        /// </summary>
        /// <returns></returns>
        public IList<POCategoriaLicencia> ObtenerTodosCategoriaLicencia()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CategoriaLicencia_CPO.Select(obj => new POCategoriaLicencia()
                {
                    Descripcion = obj.CAL_Descripcion,
                    IdCategoriaLicencia = obj.CAL_IdCategoriaLicencia
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de poliza de seguro
        /// </summary>
        /// <returns></returns>
        public IList<POTipoPolizaSeguro> ObtenerTodosTipoPolizaSeguro()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoPolizaSeguro_CPO.Select(obj => new POTipoPolizaSeguro()
                {
                    Descripcion = obj.TPS_Descripcion,
                    IdTipoPoliza = obj.TPS_IdTipoPolizaSerguro
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las aseguradoras
        /// </summary>
        /// <returns></returns>
        public IList<POAseguradora> ObtenerTodosAseguradora()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Aseguradora_CPO.Select(obj => new POAseguradora()
                {
                    Descripcion = obj.ASE_Nombre,
                    IdAseguradora = obj.ASE_IdAseguradora,
                    Identificacion = obj.ASE_Identificacion
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        #endregion Vehiculos

        /// <summary>
        /// Verifica que los vehiculos asociados al mensajero tenga vencido el soat y la revision tecnomecanica
        /// </summary>
        /// <param name="idMensajero">Id del mensajero</param>
        /// <returns></returns>
        public bool VerificaMensajeroSoatTecnoMecanica(long idMensajero)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var revision = contexto.paVerificaMensajeroTecSoat_CPO(idMensajero).FirstOrDefault();

                if (revision.Soat == idMensajero || revision.TecnoMecanica == idMensajero)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Obtiene información básica del mensajero dado su identificador
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public POMensajero ObtenerMensajero(long idMensajero)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Mensajeros_VCPO mensajero = contexto.Mensajeros_VCPO.FirstOrDefault(men => men.MEN_IdMensajero == idMensajero);
                if (mensajero != null)
                {
                    return new POMensajero()
                    {
                        IdMensajero = mensajero.MEN_IdMensajero,
                        Nombre = string.Join(" ", mensajero.PEI_Nombre.Trim(), mensajero.PEI_PrimerApellido.Trim(), string.IsNullOrEmpty(mensajero.PEI_SegundoApellido) ? string.Empty : mensajero.PEI_SegundoApellido.Trim()),
                        IdAgencia = mensajero.MEN_IdAgencia,
                        PersonaInterna = new PAPersonaInternaDC()
                        {
                            Direccion = mensajero.PEI_Direccion,
                            Telefono = mensajero.PEI_Telefono,
                            Identificacion = mensajero.PEI_Identificacion
                        }
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<POMensajero> ObtenerMensajerosAgencia(long idAgencia)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Mensajeros_VCPO.Where(Agencia => Agencia.MEN_IdAgencia == idAgencia && Agencia.MEN_Estado == ConstantesFramework.ESTADO_ACTIVO).
                  ToList().
                  ConvertAll(mensajero => new POMensajero()
                  {
                      Nombre = string.Join(" ", mensajero.PEI_Nombre, mensajero.PEI_PrimerApellido, mensajero.PEI_SegundoApellido),
                      IdMensajero = mensajero.MEN_IdMensajero,
                      Identificacion = mensajero.PEI_Identificacion,
                      IdAgencia = mensajero.MEN_IdAgencia,
                      NombreAgencia = mensajero.CES_Nombre,
                      IdTipoMensajero = mensajero.MEN_IdTipoMensajero,
                      LocalidadMensajero = new PALocalidadDC()
                      {
                          IdLocalidad = mensajero.CES_IdMunicipio,
                          Nombre = mensajero.LOC_Nombre
                      },
                      PersonaInterna = new PAPersonaInternaDC()
                      {
                          Direccion = mensajero.PEI_Direccion,
                          Telefono = mensajero.PEI_Telefono,
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene los tipos de mensajero vehicular
        /// </summary>
        /// <returns></returns>
        public IList<POTipoMensajero> ObtenerTiposMensajeroVehicular(int idTipoVehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (idTipoVehiculo == POConstantesParametrosOperacion.TipoVehiculo_Moto)
                {
                    return contexto.TipoMensajero_CPO
                      .Where(t => t.TIM_EsVehicular && t.TIM_IdTipoMensajero != POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR)
                      .OrderBy(o => o.TIM_Descripcion).ToList().ConvertAll<POTipoMensajero>
                      (t => new POTipoMensajero()
                      {
                          Descripcion = t.TIM_Descripcion,
                          EsVehicular = t.TIM_EsVehicular,
                          IdTipoMensajero = t.TIM_IdTipoMensajero
                      });
                }
                else
                {
                    return contexto.TipoMensajero_CPO
                      .Where(t => t.TIM_EsVehicular && t.TIM_IdTipoMensajero == POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR)
                      .OrderBy(o => o.TIM_Descripcion).ToList().ConvertAll<POTipoMensajero>
                     (t => new POTipoMensajero()
                     {
                         Descripcion = t.TIM_Descripcion,
                         EsVehicular = t.TIM_EsVehicular,
                         IdTipoMensajero = t.TIM_IdTipoMensajero
                     });
                }
            }
        }

        /// <summary>
        /// Obtiene  los mensajeros tipo vehicular
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los mensajeros</returns>
        public IList<POMensajero> ObtenerMensajerosVehicular(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<Mensajeros_VCPO>("TIM_EsVehicular", true.ToString(), OperadorComparacion.Equal);
                LambdaExpression lamda2 = contexto.CrearExpresionLambda<Mensajeros_VCPO>("MEN_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);

                LambdaExpression lamda3 = contexto.CrearExpresionLambda<Mensajeros_VCPO>("MEN_IdTipoMensajero", POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR.ToString(), OperadorComparacion.NotEqual);

                where.Add(lamda, OperadorLogico.And);
                where.Add(lamda2, OperadorLogico.And);
                where.Add(lamda3, OperadorLogico.And);

                for (int i = filtro.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(filtro.ToList()[i].Value))
                    {
                        filtro.Remove(filtro.ToList()[i].Key);
                    }
                }

                return contexto.ConsultarMensajeros_VCPO(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().
                  ConvertAll<POMensajero>(c =>
                    new POMensajero()
                    {
                        IdMensajero = c.MEN_IdMensajero,
                        TipoContrato = c.TIC_Descripcion.ToUpper(),
                        EsContratista = c.MEN_EsContratista,
                        Estado = c.MEN_Estado,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        FechaIngreso = c.MEN_FechaIngreso,
                        FechaTerminacionContrato = c.MEN_FechaTerminacionContrato,
                        FechaVencimientoPase = c.MEN_FechaVencimientoPase,
                        Identificacion = c.PEI_Identificacion,
                        IdTipoMensajero = c.MEN_IdTipoMensajero,
                        IdAgencia = c.MEN_IdAgencia,
                        Nombre = c.PEI_Nombre,

                        NumeroPase = c.MEN_NumeroPase,
                        Telefono2 = c.MEN_Telefono2,
                        LocalidadMensajero = new PALocalidadDC()
                        {
                            IdLocalidad = c.PEI_Municipio
                        },
                        PersonaInterna = new PAPersonaInternaDC()
                        {
                            Direccion = c.PEI_Direccion,
                            IdCargo = c.PEI_IdCargo,
                            Identificacion = c.PEI_Identificacion.Trim(),
                            IdPersonaInterna = c.PEI_IdPersonaInterna,
                            IdRegionalAdministrativa = c.PEI_IdRegionalAdm.Value,
                            Municipio = c.PEI_Municipio,
                            IdTipoIdentificacion = c.PEI_IdTipoIdentificacion,
                            Nombre = c.PEI_Nombre,
                            NombreCargo = c.CAR_Descripcion,
                            NombreCompleto = c.PEI_Nombre + " " + c.PEI_PrimerApellido,
                            PrimerApellido = c.PEI_PrimerApellido,
                            SegundoApellido = c.PEI_SegundoApellido,
                            Telefono = c.PEI_Telefono,
                            Email = c.PEI_Email
                        }
                    });
            }
        }

        /// <summary>
        /// Obtiene  los conductores y auxiliares de camion (mensajeros tipo auxiliar) activos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idTipoMensajero">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con los conductores y auxiliares en un objeto tipo mensajero</returns>
        public IList<POMensajero> ObtenerConductoresAuxiliares(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                //LambdaExpression lamda = contexto.CrearExpresionLambda<ConductoresMensajerosVCPO>("EsVehicular", "1", OperadorComparacion.Equal);
                //LambdaExpression lamda2 = contexto.CrearExpresionLambda<ConductoresMensajerosVCPO>("Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);
                //LambdaExpression lamda3 = contexto.CrearExpresionLambda<ConductoresMensajerosVCPO>("IdTipo", "-1", OperadorComparacion.Equal);
                //LambdaExpression lamda4 = contexto.CrearExpresionLambda<ConductoresMensajerosVCPO>("IdTipo", POConstantesParametrosOperacion.ID_TIPO_MENSAJERO_AUXILIAR.ToString(), OperadorComparacion.Equal);


                //where.Add(lamda3, OperadorLogico.And);
                //where.Add(lamda4, OperadorLogico.Or);     
                //where.Add(lamda, OperadorLogico.And);
                //where.Add(lamda2, OperadorLogico.And);



                if (filtro.ContainsKey("MEN_IdTipoMensajero"))
                {
                    //if (filtro["MEN_IdTipoMensajero"] == "-1")
                    //{
                    //    where.Remove(lamda4);
                    //}

                    filtro.Add("IdTipo", filtro["MEN_IdTipoMensajero"]);
                    filtro.Remove("MEN_IdTipoMensajero");
                }

                if (filtro.ContainsKey("EsVehicular"))
                    filtro.Remove("EsVehicular");

                if (filtro.ContainsKey("Estado"))
                    filtro.Remove("Estado");

                filtro.Add("EsVehicular", "1");
                filtro.Add("Estado", ConstantesFramework.ESTADO_ACTIVO);


                for (int i = filtro.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrWhiteSpace(filtro.ToList()[i].Value))
                    {
                        filtro.Remove(filtro.ToList()[i].Key);
                    }
                }
                var conductores = contexto.ConsultarContainsConductoresMensajerosVCPOes(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).Where(l => l.IdTipo == -1 || l.IdTipo == 4).ToList();

                return conductores.
                  ConvertAll<POMensajero>(m =>
                    new POMensajero()
                    {
                        IdMensajero = m.Identificador,
                        IdTipoMensajero = m.IdTipo,
                        PersonaInterna = new PAPersonaInternaDC()
                        {
                            Nombre = m.NombreCompleto,
                            NombreCargo = m.CAR_Descripcion,
                            Identificacion = m.PEI_Identificacion,
                            NombreCompleto = m.NombreCompleto
                        }
                    }).OrderBy(m => m.Nombre).ToList();
            }
        }

        /// <summary>
        /// Obtiene el propietario del vehiculo propio ( interrapidisimo)
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion del propietario</returns>
        public PAPersonaExterna ObtenerPropietarioVehiculo_Propio()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR persona = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion == POConstantesParametrosOperacion.NitInterrapidisimo && p.PEE_DigitoVerificacion == POConstantesParametrosOperacion.DigVerificacionInterRapidisimo
                  && p.PEE_IdTipoIdentificacion == POConstantesParametrosOperacion.TipoDocumento_Nit).FirstOrDefault();
                if (persona != null)
                {
                    return new PAPersonaExterna()
                    {
                        DigitoVerificacion = persona.PEE_DigitoVerificacion,
                        Direccion = persona.PEE_Direccion,
                        EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                        FechaExpedicionDocumento = persona.PEE_FechaExpedicionDocumento,
                        Identificacion = persona.PEE_Identificacion,
                        IdTipoIdentificacion = persona.PEE_IdTipoIdentificacion,
                        Municipio = persona.PEE_Municipio,
                        NombreCompleto = persona.PEE_PrimerNombre + " " + persona.PEE_PrimerApellido,
                        NumeroCelular = persona.PEE_NumeroCelular,
                        PrimerApellido = persona.PEE_PrimerApellido,
                        PrimerNombre = persona.PEE_PrimerNombre,
                        SegundoApellido = persona.PEE_SegundoApellido,
                        SegundoNombre = persona.PEE_SegundoNombre,
                        Telefono = persona.PEE_Telefono,
                        IdPersonaExterna = persona.PEE_IdPersonaExterna
                    };
                }
                return new PAPersonaExterna();
            }
        }

        /// <summary>
        /// Obtiene una persona externa a partir de la cedula
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion de la persona externa</returns>
        public PAPersonaExterna ObtenerPersonaExternaCedula(string cedula)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR persona = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == cedula.Trim()).FirstOrDefault();
                if (persona != null)
                {
                    return new PAPersonaExterna()
                    {
                        DigitoVerificacion = persona.PEE_DigitoVerificacion,
                        Direccion = persona.PEE_Direccion,
                        EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                        FechaExpedicionDocumento = persona.PEE_FechaExpedicionDocumento,
                        Identificacion = persona.PEE_Identificacion,
                        IdTipoIdentificacion = persona.PEE_IdTipoIdentificacion,
                        Municipio = persona.PEE_Municipio,
                        NombreCompleto = persona.PEE_PrimerNombre + " " + persona.PEE_PrimerApellido,
                        NumeroCelular = persona.PEE_NumeroCelular,
                        PrimerApellido = persona.PEE_PrimerApellido,
                        PrimerNombre = persona.PEE_PrimerNombre,
                        SegundoApellido = persona.PEE_SegundoApellido,
                        SegundoNombre = persona.PEE_SegundoNombre,
                        Telefono = persona.PEE_Telefono,
                        IdPersonaExterna = persona.PEE_IdPersonaExterna
                    };
                }
                return new PAPersonaExterna();
            }
        }

        /// <summary>
        /// Obtiene una persona interna a partir de la cedula
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion de la persona interna</returns>
        public PAPersonaInternaDC ObtenerPersonaInternaCedula(string cedula)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInternaCargoRegional_VPAR persona = contexto.PersonaInternaCargoRegional_VPAR.Where(p => p.PEI_Identificacion == cedula.Trim()).FirstOrDefault();

                if (persona != null)
                {
                    string NomLoc = "";
                    Localidad_PAR loc = contexto.Localidad_PAR.Where(l => l.LOC_IdLocalidad == persona.PEI_Municipio).FirstOrDefault();
                    if (loc != null)
                    {
                        NomLoc = loc.LOC_Nombre;
                    }
                    return new PAPersonaInternaDC()
                    {
                        Direccion = persona.PEI_Direccion,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        IdCargo = persona.PEI_IdCargo,
                        Identificacion = persona.PEI_Identificacion,
                        IdTipoIdentificacion = persona.PEI_IdTipoIdentificacion,
                        NombreMunicipio = NomLoc,
                        IdRegionalAdministrativa = persona.PEI_IdRegionalAdm,
                        NombreRegional = persona.REA_Descripcion,
                        Nombre = persona.PEI_Nombre,
                        IdPersonaInterna = persona.PEI_IdPersonaInterna,
                        PrimerApellido = persona.PEI_PrimerApellido,
                        NombreCargo = persona.CAR_Descripcion,
                        Telefono = persona.PEI_Telefono,
                        Municipio = persona.PEI_Municipio
                    };
                }
                return new PAPersonaInternaDC();
            }
        }

        /// <summary>
        /// Obtiene una persona interna a partir de la cedula
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion de la persona interna</returns>
        public POConductores ObtenerPersonaInternaConductor(string cedula)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInternaCargoRegional_VPAR persona = contexto.PersonaInternaCargoRegional_VPAR.Where(p => p.PEI_Identificacion == cedula.Trim()).FirstOrDefault();

                if (persona != null)
                {
                    string NomLoc = "";
                    string idLoc = "";
                    Localidad_PAR loc = contexto.Localidad_PAR.Where(l => l.LOC_IdLocalidad == persona.PEI_Municipio).FirstOrDefault();
                    if (loc != null)
                    {
                        NomLoc = loc.LOC_Nombre;
                        idLoc = loc.LOC_IdLocalidad;
                    }
                    return new POConductores()
                    {
                        PersonaInterna = new PAPersonaInternaDC()
                        {
                            IdCargo = persona.PEI_IdCargo,
                            Direccion = persona.PEI_Direccion,
                            IdTipoIdentificacion = persona.PEI_IdTipoIdentificacion,
                            Identificacion = persona.PEI_Identificacion,
                            IdRegionalAdministrativa = persona.PEI_IdRegionalAdm,
                            NombreRegional = persona.REA_Descripcion,
                            Nombre = persona.PEI_Nombre,
                            IdPersonaInterna = persona.PEI_IdPersonaInterna,
                            PrimerApellido = persona.PEI_PrimerApellido,
                            NombreCargo = persona.CAR_Descripcion,
                            Telefono = persona.PEI_Telefono,
                            Municipio = persona.PEI_Municipio,
                            NombreMunicipio = NomLoc,
                        },
                        Ciudad = new PALocalidadDC()
                        {
                            Nombre = NomLoc,
                            IdLocalidad = idLoc
                        },
                        EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                        Identificacion = persona.PEI_Identificacion,
                        NombreMunicipio = NomLoc,
                        FechaVencimientoPase = DateTime.Now,
                        FechaIngreso = DateTime.Now,
                        FechaTerminacionContrato = DateTime.Now
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene los conductores activos de un vehiculo por placa
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public IList<POConductores> ObtenerConductoresActivosVehiculos(string placa)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerConductoresVehicu_CPO(placa, ConstantesFramework.ESTADO_ACTIVO, ConstantesFramework.ESTADO_ACTIVO).ToList().ConvertAll<POConductores>(c =>
                    new POConductores()
                    {
                        NombreCompleto = c.PEI_Nombre + " " + c.PEI_PrimerApellido,
                        IdConductor = c.CON_IdConductor,
                        Identificacion = c.PEI_Identificacion
                    });
            }
        }

        /// <summary>
        /// Obtiene los conductores activos de una moto por placa
        /// </summary>
        /// <param name="placa">placa del vehiculo a consultar</param>
        public IList<POConductores> ObtenerConductoresActivosMoto(string placa)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerMensajerosMoto_CPO(placa, ConstantesFramework.ESTADO_ACTIVO, ConstantesFramework.ESTADO_ACTIVO).ToList()
                  .ConvertAll(cond => new POConductores()
                  {
                      IdConductor = cond.MEN_IdMensajero,
                      Identificacion = cond.PEI_Identificacion,
                      NombreCompleto = cond.PEI_Nombre + " " + cond.PEI_PrimerApellido + " " + cond.PEI_SegundoApellido
                  }
                  );
            }
        }

        /// <summary>
        /// Obteners el primer conductor de un vehiculo por placa
        /// </summary>
        /// <param name="placa">placa del vehiculo a consultar</param>
        public ONRutaConductorDC ObtenerConductoresPorVehiculo(string placa)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerConductoresVehicu_CPO(placa, ConstantesFramework.ESTADO_ACTIVO, ConstantesFramework.ESTADO_ACTIVO).ToList()
                  .ConvertAll(cond => new ONRutaConductorDC()
                  {
                      Conductor = new POConductores()
                      {
                          IdConductor = cond.CON_IdConductor,
                          Identificacion = cond.PEI_Identificacion,
                          NombreCompleto = cond.PEI_Nombre + " " + cond.PEI_PrimerApellido + " " + cond.PEI_SegundoApellido
                      },
                      IdVehiculo = cond.VEH_IdVehiculo,
                      IdTipoVehiculo = cond.VEH_IdTipoVehiculo
                  }
                  ).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obteners el primer conductor de una moto por placa
        /// </summary>
        /// <param name="placa">placa del vehiculo a consultar</param>
        public ONRutaConductorDC ObtenerConductoresPorMoto(string placa)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerMensajerosMoto_CPO(placa, ConstantesFramework.ESTADO_ACTIVO, ConstantesFramework.ESTADO_ACTIVO).ToList()
                  .ConvertAll(cond => new ONRutaConductorDC()
                  {
                      Conductor = new POConductores()
                      {
                          IdConductor = cond.MEN_IdMensajero,
                          Identificacion = cond.PEI_Identificacion,
                          NombreCompleto = cond.PEI_Nombre + " " + cond.PEI_PrimerApellido + " " + cond.PEI_SegundoApellido
                      },
                      IdVehiculo = cond.VEH_IdVehiculo,
                      IdTipoVehiculo = cond.VEH_IdTipoVehiculo
                  }
                  ).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene un vehiculo a partir de una placa
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public POVehiculo ObtenerVehiculoPlaca(string placa)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Vehiculo_CPO vehi = contexto.Vehiculo_CPO.Where(v => v.VEH_Placa == placa && v.VEH_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();
                if (vehi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_OPERATIVOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                return new POVehiculo()
                {
                    Placa = vehi.VEH_Placa,
                    IdTipoVehiculo = vehi.VEH_IdTipoVehiculo
                };
            }
        }

        /// <summary>
        /// Obtiene el vehiculo asociado a un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public POVehiculo ObtenerVehiculoMensajero(long idMensajero)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerVehiculoMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MEN_IdMensajero", idMensajero);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();


                var vehi = dt.AsEnumerable().FirstOrDefault();
                if (vehi != null)
                {
                    POVehiculo vehiculo = new POVehiculo()
                    {
                        IdVehiculo = vehi.Field<int>("VEH_IdVehiculo"),
                        Placa = vehi.Field<string>("VEH_Placa")
                    };
                    return vehiculo;
                }
                else
                    return null;

            }

        }

        /// <summary>
        /// Obtiene un vehiculo a partir del id del vehiculo
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public POVehiculo ObtenerVehiculoIdVehiculo(int idVehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Vehiculo_CPO vehi = contexto.Vehiculo_CPO.Where(v => v.VEH_IdVehiculo == idVehiculo && v.VEH_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();
                if (vehi == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_OPERATIVOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                return new POVehiculo()
                {
                    Placa = vehi.VEH_Placa,
                    IdTipoVehiculo = vehi.VEH_IdTipoVehiculo
                };
            }
        }

        /// <summary>
        /// Obtiene una persona interna a partir de la cedula
        /// </summary>
        /// <param name="cedula">Numero de cedula del propietario</param>
        /// <returns>Objeto con la informacion de la persona interna</returns>
        public OUPersonaInternaDC ObtenerMensajeroPersonaInterna(string cedula)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInternaCargoRegional_VPAR persona = contexto.PersonaInternaCargoRegional_VPAR.Where(p => p.PEI_Identificacion == cedula.Trim()).FirstOrDefault();

                if (persona != null)
                {
                    string NomLoc = "";
                    Localidad_PAR loc = contexto.Localidad_PAR.Where(l => l.LOC_IdLocalidad == persona.PEI_Municipio).FirstOrDefault();
                    if (loc != null)
                    {
                        NomLoc = loc.LOC_Nombre;
                    }
                    return new OUPersonaInternaDC()
                    {
                        Direccion = persona.PEI_Direccion,
                        IdCargo = persona.PEI_IdCargo,
                        Identificacion = persona.PEI_Identificacion,
                        IdTipoIdentificacion = persona.PEI_IdTipoIdentificacion,
                        Nombre = persona.PEI_Nombre,
                        IdPersonaInterna = persona.PEI_IdPersonaInterna,
                        PrimerApellido = persona.PEI_PrimerApellido,
                        Telefono = persona.PEI_Telefono,
                        Municipio = persona.PEI_Municipio
                    };
                }
                return new OUPersonaInternaDC();
            }
        }

        /// <summary>
        /// Obtiene las lineas de los vehiculos por la marca
        /// </summary>
        /// <returns></returns>
        public IList<POLinea> ObtenerLineaVehiculo(int idMarcaVehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.LineaMarcarVehiculo_CPO.Where(l => l.LMV_IdMarcaVehiculo == idMarcaVehiculo).Join(contexto.LineaVehiculo_CPO, linMar => linMar.LMV_IdLineaVehiculo, lin => lin.LVH_IdLineaVehiculo, (linMar, lin) =>
                  new
                  {
                      lin,
                      linMar
                  }).Select(obj => new POLinea()
                  {
                      Descripcion = obj.lin.LVH_Descripcion,
                      IdLineaVehiculo = obj.lin.LVH_IdLineaVehiculo
                  }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }
        #region Mensajeros

        public IEnumerable<POMensajero> ObtenerMensajerosConductores(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                //LambdaExpression lambda = contexto.CrearExpresionLambda<MensajerosAgenciaCol_VOPU>("AGE_IdAgencia", idAgencia.ToString(), OperadorComparacion.Equal);
                //where.Add(lambda, OperadorLogico.And);

                filtro.Add("Estado", ConstantesFramework.ESTADO_ACTIVO);

                return contexto.ConsultarContainsConductoresMensajerosVCPOes(filtro, campoOrdenamiento, out totalRegistros,
                                                      indicePagina, registrosPorPagina, ordenamientoAscendente)
                                                      .ToList()
                                                      .ConvertAll(r => new POMensajero()
                                                      {
                                                          PersonaInterna = new PAPersonaInternaDC()
                                                          {
                                                              NombreCompleto = r.NombreCompleto,
                                                              Nombre = r.PEI_Nombre,
                                                              PrimerApellido = r.PEI_PrimerApellido,
                                                              SegundoApellido = r.PEI_SegundoApellido,
                                                              Identificacion = r.PEI_Identificacion,
                                                              Direccion = r.PEI_Direccion,
                                                              Telefono = r.PEI_Telefono,
                                                              NombreCargo = r.CAR_Descripcion,
                                                              IdCargo = r.CAR_IdCargo
                                                          },
                                                          IdAgencia = r.MEN_IdAgencia,
                                                          NombreAgencia = r.CES_Nombre,
                                                          LocalidadMensajero = new PALocalidadDC()
                                                          {
                                                              IdLocalidad = r.PEI_Municipio,
                                                              Nombre = r.LOC_Nombre
                                                          },
                                                          IdMensajero = r.PEI_IdPersonaInterna
                                                      });
            }
        }

        #endregion Mensajeros

        #region Creación de mensajero

        #region Obtener

        /// <summary>
        /// Obtiene los mensajeros creados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            IList<OUMensajeroDC> resultado = null;
            totalRegistros = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMensajero_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrdenamientoAscendente", ordenamientoAscendente);
                cmd.Parameters.AddWithValue("@Pagina", indicePagina);
                cmd.Parameters.AddWithValue("@RegistrosxPagina", registrosPorPagina);
                cmd.Parameters.AddWithValue("@CampoOrdenamiento", campoOrdenamiento);


                string idTipoMensajero;
                if (filtro.TryGetValue("MEN_IdTipoMensajero", out idTipoMensajero))
                {
                    cmd.Parameters.AddWithValue("@fldIdTipoMensajero", idTipoMensajero);
                }

                string estadoMensajero;
                if (filtro.TryGetValue("MEN_Estado", out estadoMensajero))
                {
                    cmd.Parameters.AddWithValue("@fldEstadoMensajero", estadoMensajero);
                }

                string nombreCompleto;
                if (filtro.TryGetValue("NombreCompleto", out nombreCompleto))
                {
                    cmd.Parameters.AddWithValue("@flNombreMensajero", nombreCompleto);
                }

                string identificacion;
                if (filtro.TryGetValue("PEI_Identificacion", out identificacion))
                {
                    cmd.Parameters.AddWithValue("@flIdentificacion", identificacion);
                }

                string idCIudad;
                if (filtro.TryGetValue("LOC_Nombre", out idCIudad))
                {
                    cmd.Parameters.AddWithValue("@flIdCIudad", idCIudad);
                }

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        if (resultado == null)
                        {
                            resultado = new List<OUMensajeroDC>();
                            totalRegistros = Convert.ToInt32(reader["TotalRegistros"]);
                        }

                        var r = new OUMensajeroDC()
                        {

                            Estado = new OUEstadosMensajeroDC()
                            {
                                IdEstado = reader["MEN_Estado"].ToString(),
                                Descripcion = EstadosMensajeros(reader["MEN_Estado"].ToString()),
                            },

                            PersonaInterna = new OUPersonaInternaDC()
                            {
                                Direccion = reader["PEI_Direccion"].ToString(),
                                Nombre = reader["PEI_Nombre"].ToString(),
                                PrimerApellido = reader["PEI_PrimerApellido"].ToString(),
                                SegundoApellido = reader["PEI_SegundoApellido"].ToString(),
                                Telefono = reader["PEI_Telefono"].ToString(),
                                Email = reader["PEI_Email"].ToString(),
                                IdCargo = Convert.ToInt32(reader["PEI_IdCargo"]),
                                Identificacion = reader["PEI_Identificacion"].ToString(),
                                IdTipoIdentificacion = reader["PEI_IdTipoIdentificacion"].ToString(),
                                Municipio = reader["LOC_Nombre"].ToString(),
                                IdMunicipio = reader["PEI_Municipio"].ToString(),
                                Regional = Convert.ToInt64(reader["PEI_IdRegionalAdm"]),
                                TipoContrato = reader["DescripTipoContrato"].ToString().ToUpper(),
                                Cargo = reader["CAR_Descripcion"].ToString(),
                                FechaInicioContrato = Convert.ToDateTime(reader["MEN_FechaIngreso"]),
                                FechaTerminacionContrato = Convert.ToDateTime(reader["MEN_FechaTerminacionContrato"])
                            },

                            IdMensajero = Convert.ToInt64(reader["MEN_IdMensajero"]),
                            TipoMensajero = reader["TIM_Descripcion"].ToString(),
                            NombreCompleto = reader["NombreCompleto"].ToString(),
                            NumeroPase = reader["MEN_NumeroPase"].ToString(),
                            IdTipoMensajero = Convert.ToInt16(reader["MEN_IdTipoMensajero"]),
                            EsContratista = Convert.ToBoolean(reader["MEN_EsContratista"]),
                            FechaVencimientoPase = Convert.ToDateTime(reader["MEN_FechaVencimientoPase"]),
                            Apellidos = reader["PEI_PrimerApellido"].ToString() + " " + reader["PEI_SegundoApellido"].ToString(),
                            Telefono2 = reader["MEN_Telefono2"].ToString(),
                            Agencia = Convert.ToInt64(reader["AGE_IdAgencia"]),
                            NombreAgencia = reader["CES_Nombre"].ToString(),
                            EsMensajeroUrbano = Convert.ToBoolean(reader["MEN_EsMensajeroUrbano"]),
                            EsMensajeroPAM = Convert.ToBoolean(reader["MEN_AplicaPAM"]),
                            CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                            {
                                IdCargo = Convert.ToInt32(reader["PEI_IdCargo"]),
                            },
                            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                            FechaIngreso = Convert.ToDateTime(reader["MEN_FechaIngreso"]),
                            FechaTerminacionContrato = Convert.ToDateTime(reader["MEN_FechaTerminacionContrato"]),
                            LocalidadMensajero = new PALocalidadDC()
                            {
                                IdLocalidad = reader["PEI_Municipio"].ToString(),
                                Nombre = reader["LOC_Nombre"].ToString()
                            },
                            TipoContrato = new POTipoContrato()
                            {
                                IdTipoContrato = Convert.ToInt32(reader["MEN_TipoContrato"]),
                                Descripcion = reader["DescripTipoContrato"].ToString().ToUpper()
                            },

                            TipMensajeros = new OUTipoMensajeroDC()
                            {
                                IdTipoMensajero = Convert.ToInt16(reader["MEN_IdTipoMensajero"]),
                                Descripcion = reader["TIM_Descripcion"].ToString()
                            }
                        };


                        resultado.Add(r);
                    }
                }

            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del nombre de usuario, esto aplica
        /// para los usuario que tambien son mensajeros en el caso de los puntos moviles
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public long ObtenerIdMensajeroNomUsuario(string usuario)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajeroPorUsuario_OPU", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USU_IdUsuario", usuario);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var usr = dt.AsEnumerable().ToList().FirstOrDefault();

                if (usr == null)
                    return 0;

                return usr.Field<long>("MEN_IdMensajero");

            }
        }

        /// <summary>
        /// Metodo para obtener informacion respectiva al mensajero segun el usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionMensajeroNomUsuarioPAM(string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerInformacionMensajeroPorUsuarioPAM_OPU", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@USU_IdUsuario", usuario);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var usr = dt.AsEnumerable().ToList().FirstOrDefault();

                if (usr == null)
                    return null;

                return new OUMensajeroDC()
                {
                    IdMensajero = usr.Field<long>("MEN_IdMensajero"),
                    NombreCompleto = usr.Field<string>("PEI_PrimerApellido") + " " + usr.Field<string>("PEI_SegundoApellido")
                };
            }
        }

        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajero(long idMensajero)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajeroPersonaInternaIdMensajero_CPO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MEN_IdMensajero", idMensajero);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);



                return dt.AsEnumerable().ToList().ConvertAll<OUMensajeroDC>(r =>
                   {
                       OUMensajeroDC men = new OUMensajeroDC();

                       men.Estado = new OUEstadosMensajeroDC()
                       {
                           IdEstado = r.Field<string>("MEN_Estado"),
                           Descripcion = EstadosMensajeros(r.Field<string>("MEN_Estado"))
                       };
                       men.PersonaInterna = new OUPersonaInternaDC();

                       men.PersonaInterna.Direccion = r.Field<string>("PEI_Direccion");
                       men.PersonaInterna.Nombre = r.Field<string>("PEI_Nombre");
                       men.PersonaInterna.PrimerApellido = r.Field<string>("PEI_PrimerApellido");
                       men.PersonaInterna.SegundoApellido = r.Field<string>("PEI_SegundoApellido");
                       men.PersonaInterna.Telefono = r.Field<string>("PEI_Telefono");
                       men.PersonaInterna.Email = r.Field<string>("PEI_Email");
                       men.PersonaInterna.IdCargo = r.Field<int>("PEI_IdCargo");
                       men.PersonaInterna.Identificacion = r.Field<string>("PEI_Identificacion");
                       men.PersonaInterna.IdTipoIdentificacion = r.Field<string>("PEI_IdTipoIdentificacion");
                       men.PersonaInterna.Municipio = r.Field<string>("LOC_Nombre");
                       men.PersonaInterna.IdMunicipio = r.Field<string>("PEI_Municipio");
                       men.PersonaInterna.Regional = r.Field<long?>("PEI_IdRegionalAdm").Value;
                       men.PersonaInterna.TipoContrato = r.Field<string>("DescripTipoContrato").ToUpper();
                       men.PersonaInterna.Cargo = r.Field<string>("CAR_Descripcion");
                       men.PersonaInterna.FechaInicioContrato = r.Field<DateTime>("MEN_FechaIngreso");
                       men.PersonaInterna.FechaTerminacionContrato = r.Field<DateTime>("MEN_FechaTerminacionContrato");
                       men.PersonaInterna.IdPersonaInterna = r.Field<long>("PEI_IdPersonaInterna");



                       men.IdMensajero = r.Field<long>("MEN_IdMensajero");
                       men.TipoMensajero = r.Field<string>("TIM_Descripcion");
                       men.NombreCompleto = r.Field<string>("NombreCompleto");
                       men.NumeroPase = r.Field<string>("MEN_NumeroPase");
                       men.IdTipoMensajero = r.Field<short>("MEN_IdTipoMensajero");
                       men.EsContratista = r.Field<bool>("MEN_EsContratista");
                       men.FechaVencimientoPase = r.Field<DateTime>("MEN_FechaVencimientoPase");
                       men.Apellidos = r.Field<string>("PEI_PrimerApellido") + " " + r.Field<string>("PEI_SegundoApellido");
                       men.Telefono2 = r.Field<string>("MEN_Telefono2");
                       men.Agencia = r.Field<long>("AGE_IdAgencia");
                       men.NombreAgencia = r.Field<string>("CES_Nombre");
                       men.EsMensajeroUrbano = r.Field<bool>("MEN_EsMensajeroUrbano");
                       men.CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                       {
                           IdCargo = r.Field<int>("PEI_IdCargo"),
                       };
                       men.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                       men.FechaIngreso = r.Field<DateTime>("MEN_FechaIngreso");
                       men.FechaTerminacionContrato = r.Field<DateTime>("MEN_FechaTerminacionContrato");
                       men.LocalidadMensajero = new PALocalidadDC()
                       {
                           IdLocalidad = r.Field<string>("PEI_Municipio"),
                           Nombre = r.Field<string>("LOC_Nombre")
                       };
                       men.TipoContrato = new POTipoContrato()
                       {
                           IdTipoContrato = r.Field<short>("MEN_TipoContrato"),
                           Descripcion = r.Field<string>("DescripTipoContrato").ToUpper()
                       };

                       men.TipMensajeros = new OUTipoMensajeroDC()
                       {
                           IdTipoMensajero = r.Field<short>("MEN_IdTipoMensajero"),
                           Descripcion = r.Field<string>("TIM_Descripcion")
                       };


                       return men;
                   }).FirstOrDefault();

            }

        }



        /// <summary>
        /// Obtiene la informacion de un mensajero a partir del id del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerMensajeroIdMensajeroPAM(long idMensajero)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajeroPersonaInternaPAM_CPO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MEN_IdMensajero", idMensajero);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);



                return dt.AsEnumerable().ToList().ConvertAll<OUMensajeroDC>(r =>
                {
                    OUMensajeroDC men = new OUMensajeroDC();

                    men.Estado = new OUEstadosMensajeroDC()
                    {
                        IdEstado = r.Field<string>("MEN_Estado"),
                        Descripcion = EstadosMensajeros(r.Field<string>("MEN_Estado"))
                    };
                    men.PersonaInterna = new OUPersonaInternaDC();

                    men.PersonaInterna.Direccion = r.Field<string>("PEI_Direccion");
                    men.PersonaInterna.Nombre = r.Field<string>("PEI_Nombre");
                    men.PersonaInterna.PrimerApellido = r.Field<string>("PEI_PrimerApellido");
                    men.PersonaInterna.SegundoApellido = r.Field<string>("PEI_SegundoApellido");
                    men.PersonaInterna.Telefono = r.Field<string>("PEI_Telefono");
                    men.PersonaInterna.Email = r.Field<string>("PEI_Email");
                    men.PersonaInterna.IdCargo = r.Field<int>("PEI_IdCargo");
                    men.PersonaInterna.Identificacion = r.Field<string>("PEI_Identificacion");
                    men.PersonaInterna.IdTipoIdentificacion = r.Field<string>("PEI_IdTipoIdentificacion");
                    men.PersonaInterna.Municipio = r.Field<string>("LOC_Nombre");
                    men.PersonaInterna.IdMunicipio = r.Field<string>("PEI_Municipio");
                    men.PersonaInterna.Regional = r.Field<long?>("PEI_IdRegionalAdm").Value;
                    men.PersonaInterna.TipoContrato = r.Field<string>("DescripTipoContrato").ToUpper();
                    men.PersonaInterna.Cargo = r.Field<string>("CAR_Descripcion");
                    men.PersonaInterna.FechaInicioContrato = r.Field<DateTime>("MEN_FechaIngreso");
                    men.PersonaInterna.FechaTerminacionContrato = r.Field<DateTime>("MEN_FechaTerminacionContrato");
                    men.PersonaInterna.IdPersonaInterna = r.Field<long>("PEI_IdPersonaInterna");



                    men.IdMensajero = r.Field<long>("MEN_IdMensajero");
                    men.TipoMensajero = r.Field<string>("TIM_Descripcion");
                    men.NombreCompleto = r.Field<string>("NombreCompleto");
                    men.NumeroPase = r.Field<string>("MEN_NumeroPase");
                    men.IdTipoMensajero = r.Field<short>("MEN_IdTipoMensajero");
                    men.EsContratista = r.Field<bool>("MEN_EsContratista");
                    men.FechaVencimientoPase = r.Field<DateTime>("MEN_FechaVencimientoPase");
                    men.Apellidos = r.Field<string>("PEI_PrimerApellido") + " " + r.Field<string>("PEI_SegundoApellido");
                    men.Telefono2 = r.Field<string>("MEN_Telefono2");
                    men.Agencia = r.Field<long>("CES_IdCentroServicios");
                    men.NombreAgencia = r.Field<string>("CES_Nombre");
                    men.EsMensajeroUrbano = r.Field<bool>("MEN_EsMensajeroUrbano");
                    men.CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                    {
                        IdCargo = r.Field<int>("PEI_IdCargo"),
                    };
                    men.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                    men.FechaIngreso = r.Field<DateTime>("MEN_FechaIngreso");
                    men.FechaTerminacionContrato = r.Field<DateTime>("MEN_FechaTerminacionContrato");
                    men.LocalidadMensajero = new PALocalidadDC()
                    {
                        IdLocalidad = r.Field<string>("PEI_Municipio"),
                        Nombre = r.Field<string>("LOC_Nombre")
                    };
                    men.TipoContrato = new POTipoContrato()
                    {
                        IdTipoContrato = r.Field<short>("MEN_TipoContrato"),
                        Descripcion = r.Field<string>("DescripTipoContrato").ToUpper()
                    };

                    men.TipMensajeros = new OUTipoMensajeroDC()
                    {
                        IdTipoMensajero = r.Field<short>("MEN_IdTipoMensajero"),
                        Descripcion = r.Field<string>("TIM_Descripcion")
                    };


                    return men;
                }).FirstOrDefault();

            }

        }

        /// <summary>
        /// COnsulta los mensajeros activso del sistema activos y pertenecientes a agencias activas
        /// </summary>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosActivos()
        {
            List<OUMensajeroDC> retorno = null;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("sp_ConsultaMensajerosGeneral", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Estado", ConstantesFramework.ESTADO_ACTIVO);
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    retorno = new List<OUMensajeroDC>();
                    while (reader.Read())
                    {
                        OUMensajeroDC men = new OUMensajeroDC();
                        men.Estado = new OUEstadosMensajeroDC()
                        {
                            IdEstado = reader["MEN_Estado"].ToString(),
                            Descripcion = EstadosMensajeros(reader["MEN_Estado"].ToString())
                        };
                        men.PersonaInterna = new OUPersonaInternaDC();
                        men.PersonaInterna.Direccion = reader["PEI_Direccion"].ToString();
                        men.PersonaInterna.Nombre = reader["PEI_Nombre"].ToString();
                        men.PersonaInterna.PrimerApellido = reader["PEI_PrimerApellido"] is DBNull ? string.Empty : reader["PEI_PrimerApellido"].ToString();
                        men.PersonaInterna.SegundoApellido = reader["PEI_SegundoApellido"] is DBNull ? string.Empty : reader["PEI_SegundoApellido"].ToString();
                        men.PersonaInterna.Telefono = reader["PEI_Telefono"].ToString();
                        men.PersonaInterna.Email = reader["PEI_Email"].ToString();
                        men.PersonaInterna.IdCargo = Convert.ToInt32(reader["PEI_IdCargo"]);
                        men.PersonaInterna.Identificacion = reader["PEI_Identificacion"].ToString();
                        men.PersonaInterna.IdTipoIdentificacion = reader["PEI_IdTipoIdentificacion"].ToString();
                        men.PersonaInterna.Municipio = reader["LOC_Nombre"].ToString();
                        men.PersonaInterna.IdMunicipio = reader["PEI_Municipio"].ToString();
                        men.PersonaInterna.Regional = reader["PEI_IdRegionalAdm"] is DBNull ? 0 : Convert.ToInt64(reader["PEI_IdRegionalAdm"]);
                        men.PersonaInterna.TipoContrato = reader["DescripTipoContrato"].ToString().ToUpper();
                        men.PersonaInterna.Cargo = reader["CAR_Descripcion"].ToString();
                        men.PersonaInterna.FechaInicioContrato = Convert.ToDateTime(reader["MEN_FechaIngreso"]);
                        men.PersonaInterna.FechaTerminacionContrato = Convert.ToDateTime(reader["MEN_FechaTerminacionContrato"]);
                        men.PersonaInterna.IdPersonaInterna = Convert.ToInt32(reader["PEI_IdPersonaInterna"]);
                        men.IdMensajero = Convert.ToInt32(reader["MEN_IdMensajero"]);
                        men.TipoMensajero = reader["TIM_Descripcion"].ToString();
                        men.NombreCompleto = reader["NombreCompleto"].ToString();
                        men.NumeroPase = reader["MEN_NumeroPase"].ToString();
                        men.IdTipoMensajero = Convert.ToInt16(reader["MEN_IdTipoMensajero"]);
                        men.IdCentroServicio = Convert.ToInt64(reader["CES_IdCentroServicios"]);
                        men.EsContratista = Convert.ToBoolean(reader["MEN_EsContratista"]);
                        men.FechaVencimientoPase = Convert.ToDateTime(reader["MEN_FechaVencimientoPase"]);
                        var apellido2 = reader["PEI_SegundoApellido"] is DBNull ? string.Empty : reader["PEI_SegundoApellido"].ToString();
                        men.Apellidos = reader["PEI_PrimerApellido"].ToString() + " " + apellido2;
                        men.Telefono2 = reader["MEN_Telefono2"].ToString();
                        men.Agencia = Convert.ToInt64(reader["CES_IdCentroServicios"]);
                        men.NombreAgencia = reader["CES_Nombre"].ToString();
                        men.EsMensajeroUrbano = Convert.ToBoolean(reader["MEN_EsMensajeroUrbano"]);
                        men.CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                        {
                            IdCargo = Convert.ToInt32(reader["PEI_IdCargo"])
                        };
                        men.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                        men.FechaIngreso = Convert.ToDateTime(reader["MEN_FechaIngreso"]);
                        men.FechaTerminacionContrato = Convert.ToDateTime(reader["MEN_FechaTerminacionContrato"]);
                        men.LocalidadMensajero = new PALocalidadDC()
                        {
                            IdLocalidad = reader["PEI_Municipio"].ToString(),
                            Nombre = reader["LOC_Nombre"].ToString()
                        };
                        men.TipoContrato = new POTipoContrato()
                        {
                            IdTipoContrato = Convert.ToInt32(reader["MEN_TipoContrato"]),
                            Descripcion = reader["DescripTipoContrato"].ToString().ToUpper()
                        };
                        men.TipMensajeros = new OUTipoMensajeroDC()
                        {
                            IdTipoMensajero = Convert.ToInt16(reader["MEN_IdTipoMensajero"]),
                            Descripcion = reader["TIM_Descripcion"].ToString()
                        };

                        retorno.Add(men);
                    }
                }
                return retorno;
            }
        }

        /// <summary>
        /// Estado de los mensajeros
        /// </summary>
        /// <param name="estadoMensajero"></param>
        /// <returns></returns>
        public string EstadosMensajeros(string estadoMensajero)
        {
            string mensajeEstado = String.Empty;
            if (POConstantesParametrosOperacion.Estado_Activo.ToString().CompareTo(estadoMensajero) == 0)
                mensajeEstado = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_ACTIVO);

            else if (POConstantesParametrosOperacion.Estado_Inactivo.ToString().CompareTo(estadoMensajero) == 0)
                mensajeEstado = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_INACTIVO);

            else if (POConstantesParametrosOperacion.EstadoSuspendido.ToString().CompareTo(estadoMensajero) == 0)
                mensajeEstado = MensajesParametrosOperacion.CargarMensaje(EnumTipoErrorParametrosOperacion.IN_ESTADO_SUSPENDIDO);

            return mensajeEstado;
        }

        /// <summary>
        /// Consulta si el mensajero existe
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ConsultaExisteMensajero(string identificacion)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MensajeroLocalidad_VOPU mensajeroEn = contexto.MensajeroLocalidad_VOPU
                   .Where(r => r.PEI_Identificacion == identificacion.Trim())
                   .FirstOrDefault();

                if (mensajeroEn != null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.PARAMETROS_OPERATIVOS, ETipoErrorFramework.EX_REGISTRO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_REGISTRO_YA_EXISTE)));
                }

                return true;
            }
        }

        /// <summary>
        /// Obtiene los tipos de mensajeros
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoMensajero_CPO.OrderBy(o => o.TIM_Descripcion).ToList().ConvertAll(r => new OUTipoMensajeroDC()
                {
                    Descripcion = r.TIM_Descripcion,
                    IdTipoMensajero = r.TIM_IdTipoMensajero
                });
            }
        }

        #endregion Obtener

        #region Edicion

        /// <summary>
        /// Edita los datos del mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void EditarMensajero(OUMensajeroDC mensajero)
        {
            OUMensajeroDC ouMensajeroDC = ObtenerMensajeroIdMensajero(mensajero.IdMensajero);

            if (ouMensajeroDC != null)
            {
                PAPersonaInternaDC paPersonaInternaDC = ObtenerPersonaInternaCedula(mensajero.PersonaInterna.Identificacion);
                if (paPersonaInternaDC == null)
                {
                    int idPersonaInterna = 0;
                    using (SqlConnection conn = new SqlConnection(conexionStringController))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("paInsertarPersonaInterna_PAR", conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Comentarios", string.Empty);
                        cmd.Parameters.AddWithValue("@Direccion", mensajero.PersonaInterna.Direccion);
                        cmd.Parameters.AddWithValue("@IdCargo", mensajero.CargoMensajero.IdCargo);
                        cmd.Parameters.AddWithValue("@IdRegionalAdm", mensajero.PersonaInterna.Regional);
                        cmd.Parameters.AddWithValue("@Identificacion", mensajero.PersonaInterna.Identificacion.Trim());
                        cmd.Parameters.AddWithValue("@IdTipoIdentificacion", ConstantesFramework.TIPO_DOCUMENTO_CC);
                        cmd.Parameters.AddWithValue("@Municipio", mensajero.LocalidadMensajero.IdLocalidad);
                        cmd.Parameters.AddWithValue("@Nombre", mensajero.PersonaInterna.Nombre);
                        cmd.Parameters.AddWithValue("@PrimerApellido", mensajero.PersonaInterna.PrimerApellido);
                        cmd.Parameters.AddWithValue("@SegundoApellido", mensajero.PersonaInterna.SegundoApellido);
                        cmd.Parameters.AddWithValue("@Telefono", mensajero.PersonaInterna.Telefono);
                        cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                        cmd.Parameters.AddWithValue("@Email", string.Empty);
                        idPersonaInterna = Convert.ToInt32(cmd.ExecuteScalar());
                        conn.Close();
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(conexionStringController))
                    {
                        DateTime fechaVencimiento = mensajero.FechaVencimientoPase.Year < 2000 ? DateTime.Now : mensajero.FechaVencimientoPase;
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("paActualizarPersonaInterna_PAR", conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Comentarios", string.IsNullOrWhiteSpace(mensajero.PersonaInterna.Comentarios) ? "." : mensajero.PersonaInterna.Comentarios);
                        cmd.Parameters.AddWithValue("@Direccion", mensajero.PersonaInterna.Direccion);
                        cmd.Parameters.AddWithValue("@Email", mensajero.PersonaInterna.Email);
                        cmd.Parameters.AddWithValue("@IdCargo", mensajero.CargoMensajero.IdCargo);
                        cmd.Parameters.AddWithValue("@Identificacion", mensajero.PersonaInterna.Identificacion);
                        cmd.Parameters.AddWithValue("@IdRegionalAdm", mensajero.PersonaInterna.Regional);
                        cmd.Parameters.AddWithValue("@IdTipoIdentificacion", mensajero.PersonaInterna.IdTipoIdentificacion);
                        cmd.Parameters.AddWithValue("@Municipio", mensajero.LocalidadMensajero.IdLocalidad);
                        cmd.Parameters.AddWithValue("@Nombre", mensajero.PersonaInterna.Nombre);
                        cmd.Parameters.AddWithValue("@PrimerApellido", mensajero.PersonaInterna.PrimerApellido);
                        cmd.Parameters.AddWithValue("@SegundoApellido", mensajero.PersonaInterna.SegundoApellido);
                        cmd.Parameters.AddWithValue("@Telefono", mensajero.PersonaInterna.Telefono);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }

                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    DateTime fechaVencimiento = mensajero.FechaVencimientoPase.Year < 2000 ? DateTime.Now : mensajero.FechaVencimientoPase;
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("paActualizarMensajero_CPO", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdMensajero", mensajero.IdMensajero);
                    cmd.Parameters.AddWithValue("@IdTipoMensajero", mensajero.TipMensajeros.IdTipoMensajero);
                    cmd.Parameters.AddWithValue("@IdAgencia", mensajero.Agencia);
                    cmd.Parameters.AddWithValue("@Telefono2", mensajero.Telefono2);
                    cmd.Parameters.AddWithValue("@FechaIngreso", mensajero.FechaIngreso);
                    cmd.Parameters.AddWithValue("@FechaTerminacionContrato", mensajero.PersonaInterna.FechaTerminacionContrato);
                    cmd.Parameters.AddWithValue("@Numeropase", mensajero.NumeroPase);
                    cmd.Parameters.AddWithValue("@FechaVencimientoPase", fechaVencimiento);
                    cmd.Parameters.AddWithValue("@Estado", mensajero.Estado.IdEstado);
                    cmd.Parameters.AddWithValue("@EsContratista", mensajero.EsContratista);
                    cmd.Parameters.AddWithValue("@TipoContrato", mensajero.TipoContrato.IdTipoContrato);
                    cmd.Parameters.AddWithValue("@EsMensajeroUrbano", mensajero.EsMensajeroUrbano);
                    cmd.Parameters.AddWithValue("@ESMensajeroPAM", mensajero.EsMensajeroPAM);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        #endregion Edicion

        #region Insercion

        /// <summary>
        /// Crea un nuevo mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void AdicionarMensajero(OUMensajeroDC mensajero)
        {

            using (TransactionScope transac = new TransactionScope())
            {
                long id;
                id = GuardarPersonaInterna(mensajero);

                InsertarMensajero(mensajero, id);

                transac.Complete();
            }
        }

        /// <summary>
        /// Crea la persona interna
        /// </summary>
        /// <param name="mensajero"></param>
        /// <returns></returns>
        private long GuardarPersonaInterna(OUMensajeroDC mensajero)
        {
            PAPersonaInternaDC perInterna = ObtenerPersonaInternaCedula(mensajero.PersonaInterna.Identificacion);
            long id = 0;
            if (perInterna.Identificacion == null)
            {
                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("paInsertarPersonaInterna_PAR", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Comentarios", string.Empty);
                    cmd.Parameters.AddWithValue("@Direccion", mensajero.PersonaInterna.Direccion);
                    cmd.Parameters.AddWithValue("@IdCargo", mensajero.CargoMensajero.IdCargo);
                    cmd.Parameters.AddWithValue("@IdRegionalAdm", mensajero.PersonaInterna.Regional);
                    cmd.Parameters.AddWithValue("@Identificacion", mensajero.PersonaInterna.Identificacion.Trim());
                    cmd.Parameters.AddWithValue("@IdTipoIdentificacion", ConstantesFramework.TIPO_DOCUMENTO_CC);
                    cmd.Parameters.AddWithValue("@Municipio", mensajero.LocalidadMensajero.IdLocalidad);
                    cmd.Parameters.AddWithValue("@Nombre", mensajero.PersonaInterna.Nombre);
                    cmd.Parameters.AddWithValue("@PrimerApellido", mensajero.PersonaInterna.PrimerApellido);
                    cmd.Parameters.AddWithValue("@SegundoApellido", mensajero.PersonaInterna.SegundoApellido == null ? string.Empty : mensajero.PersonaInterna.SegundoApellido);
                    cmd.Parameters.AddWithValue("@Telefono", mensajero.PersonaInterna.Telefono);
                    cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@Email", string.Empty);
                    id = Convert.ToInt64(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            else
            {
                id = perInterna.IdPersonaInterna;
            }

            return id;
        }

        /// <summary>
        /// Inserta nuevo mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        /// <param name="id"></param>
        public void InsertarMensajero(OUMensajeroDC mensajero, long id)
        {
            int idMensajero;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarMensajero_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoMensajero", mensajero.TipMensajeros.IdTipoMensajero);
                cmd.Parameters.AddWithValue("@IdAgencia", mensajero.Agencia);
                cmd.Parameters.AddWithValue("@Telefono2", mensajero.Telefono2);
                cmd.Parameters.AddWithValue("@FechaIngreso", mensajero.PersonaInterna.FechaInicioContrato);
                cmd.Parameters.AddWithValue("@FechaTerminacionContrato", mensajero.PersonaInterna.FechaTerminacionContrato);
                cmd.Parameters.AddWithValue("@NumeroPase", mensajero.NumeroPase);
                cmd.Parameters.AddWithValue("@FechaVencimientoPase", mensajero.FechaVencimientoPase);
                cmd.Parameters.AddWithValue("@Estado", mensajero.Estado.IdEstado);
                cmd.Parameters.AddWithValue("@EsContratista", mensajero.EsContratista);
                cmd.Parameters.AddWithValue("@TipoContrato", mensajero.TipoContrato.IdTipoContrato);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdPersonaInterna", id);
                cmd.Parameters.AddWithValue("@EsMensajeroUrbano", mensajero.EsMensajeroUrbano);
                cmd.Parameters.AddWithValue("@EsMensajeroPAM", mensajero.EsMensajeroPAM);
                idMensajero = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }
        }

        /// <summary>
        /// Guarda un mensajero
        /// </summary>
        /// <param name="mensajero"></param>
        public void GuardarMensajero(POMensajero mensajero)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Mensajero_CPO mensajeroEn = new Mensajero_CPO()
                {
                    MEN_EsContratista = mensajero.EsContratista,
                    MEN_Estado = mensajero.EstadoMensajero.IdEstado,
                    MEN_FechaIngreso = mensajero.FechaIngreso,
                    MEN_FechaTerminacionContrato = mensajero.FechaTerminacionContrato,
                    MEN_FechaVencimientoPase = mensajero.FechaVencimientoPase,
                    MEN_IdTipoMensajero = mensajero.TipoMensajero.IdTipoMensajero,
                    MEN_NumeroPase = mensajero.NumeroPase == null ? " " : mensajero.NumeroPase,
                    MEN_Telefono2 = mensajero.Telefono2,
                    MEN_IdMensajero = mensajero.IdMensajero,
                    MEN_CreadoPor = ControllerContext.Current.Usuario,
                    MEN_FechaGrabacion = DateTime.Now,
                    MEN_TipoContrato = Convert.ToInt16(mensajero.TipoContratoMensajero.IdTipoContrato),
                    MEN_IdAgencia = mensajero.IdAgencia,
                    MEN_EsMensajeroUrbano = mensajero.EsMensajeroUrbano
                };

                contexto.Mensajero_CPO.Add(mensajeroEn);

                contexto.SaveChanges();
            }
        }

        #endregion Insercion

        #endregion Creación de mensajero

        #region Vehiculos

        /// <summary>
        /// Obtiene los vehiculos del racol del usuario
        /// </summary>
        /// <param name="idRacol">id del racol</param>
        /// <returns>Lista de los vehiculo del racol</returns>
        public List<POVehiculo> ObtenerVehiculosRacol(long idRacol)
        {
            //using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerVehiculosActRacol_CPO(idRacol)
            //      .ToList()
            //      .ConvertAll(r => new POVehiculo()
            //      {
            //          IdVehiculo = r.VEH_IdVehiculo,
            //          Placa = r.VEH_Placa,
            //          NumeroSerie = r.VEH_Serie,
            //          IdTipoVehiculo = r.VEH_IdTipoVehiculo,
            //          Modelo = r.VEH_Modelo,
            //          NombreMarcaVehiculo = r.Marca,
            //          TipoVehiculo = r.TipoVehiculo
            //      }).OrderBy(v => v.Placa).ToList();
            //}

            List<POVehiculo> vehiculos = new List<POVehiculo>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerVehiculosActRacol_CPO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRacol", idRacol);
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    vehiculos = PORepositorioMapper.ToListVehiculos(resultado);
                }
            }
            return vehiculos;
        }

        #endregion Vehiculos

        /// <summary>
        /// Retorna un usuario interno dado su número de cédula
        /// </summary>
        /// <param name="idcedula"></param>
        /// <returns></returns>
        public SEAdminUsuario ObtenerUsuarioInternoPorCedula(string idcedula)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                UsuarioInterno_VSEG usuario = contexto.UsuarioInterno_VSEG.FirstOrDefault(u => u.PEI_Identificacion == idcedula && u.PEI_IdTipoIdentificacion == ConstantesFramework.TIPO_DOCUMENTO_CC);
                if (usuario != null)
                {
                    return new SEAdminUsuario
                    {
                        Apellido1 = usuario.PEI_PrimerApellido,
                        Apellido2 = usuario.PEI_SegundoApellido,
                        Cargo = usuario.CAR_Descripcion,
                        Nombre = usuario.PEI_Nombre
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public void ModificarPropietario(POPropietarioVehiculo propietario, int idVehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PropietarioVehiculo_CPO propietarioVehiculoBD;

                PropietarioVehiculoVehiculo_CPO propietarioVeVe = contexto.PropietarioVehiculoVehiculo_CPO.Where(prV => prV.PVV_IdVehiculo == idVehiculo).FirstOrDefault();

                if (propietarioVeVe != null)
                {
                    contexto.PropietarioVehiculoVehiculo_CPO.Remove(propietarioVeVe);
                    PORepositorioAudit.MapearAuditModificarPropietarioVehiculoVehiculo(contexto);
                }

                PersonaExterna_PAR perExtPropietario = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == propietario.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == propietario.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();
                if (perExtPropietario == null)
                {
                    perExtPropietario = new PersonaExterna_PAR()
                    {
                        PEE_CreadoPor = ControllerContext.Current.Usuario,
                        PEE_FechaGrabacion = DateTime.Now,
                        PEE_DigitoVerificacion = propietario.PersonaExterna.DigitoVerificacion,
                        PEE_Direccion = propietario.PersonaExterna.Direccion,
                        PEE_FechaExpedicionDocumento = propietario.PersonaExterna.FechaExpedicionDocumento,
                        PEE_Identificacion = propietario.PersonaExterna.Identificacion.Trim(),
                        PEE_IdTipoIdentificacion = propietario.PersonaExterna.IdTipoIdentificacion,
                        PEE_Municipio = propietario.CiudadPropietario.IdLocalidad,
                        PEE_NumeroCelular = propietario.PersonaExterna.NumeroCelular,
                        PEE_PrimerApellido = propietario.PersonaExterna.PrimerApellido,
                        PEE_PrimerNombre = propietario.PersonaExterna.PrimerNombre,
                        PEE_SegundoApellido = propietario.PersonaExterna.SegundoApellido,
                        PEE_SegundoNombre = propietario.PersonaExterna.SegundoNombre,
                        PEE_Telefono = propietario.PersonaExterna.Telefono,
                    };

                    propietarioVehiculoBD = new PropietarioVehiculo_CPO()
                    {
                        PRV_IdPropietarioVehiculo = perExtPropietario.PEE_IdPersonaExterna,
                        PRV_CreadoPor = ControllerContext.Current.Usuario,
                        PRV_FechaGrabacion = DateTime.Now,
                        PRV_IdTipoContrato = (short)propietario.IdTipoContrato
                    };
                    contexto.PersonaExterna_PAR.Add(perExtPropietario);
                    contexto.PropietarioVehiculo_CPO.Add(propietarioVehiculoBD);
                }
                else
                {
                    perExtPropietario.PEE_DigitoVerificacion = propietario.PersonaExterna.DigitoVerificacion;
                    perExtPropietario.PEE_Direccion = propietario.PersonaExterna.Direccion;
                    perExtPropietario.PEE_FechaExpedicionDocumento = propietario.PersonaExterna.FechaExpedicionDocumento;
                    perExtPropietario.PEE_Identificacion = propietario.PersonaExterna.Identificacion.Trim();
                    perExtPropietario.PEE_IdTipoIdentificacion = propietario.PersonaExterna.IdTipoIdentificacion;
                    perExtPropietario.PEE_Municipio = propietario.CiudadPropietario.IdLocalidad;
                    perExtPropietario.PEE_NumeroCelular = propietario.PersonaExterna.NumeroCelular;
                    perExtPropietario.PEE_PrimerApellido = propietario.PersonaExterna.PrimerApellido;
                    perExtPropietario.PEE_PrimerNombre = propietario.PersonaExterna.PrimerNombre;
                    perExtPropietario.PEE_SegundoApellido = propietario.PersonaExterna.SegundoApellido;
                    perExtPropietario.PEE_SegundoNombre = propietario.PersonaExterna.SegundoNombre;
                    perExtPropietario.PEE_Telefono = propietario.PersonaExterna.Telefono;
                    PORepositorioAudit.MapearAuditModificarPersonaExterna(contexto);

                    propietarioVehiculoBD = contexto.PropietarioVehiculo_CPO
                        .Where(prV => prV.PRV_IdPropietarioVehiculo == perExtPropietario.PEE_IdPersonaExterna)
                        .FirstOrDefault();

                    if (propietarioVehiculoBD != null)
                    {
                        propietarioVehiculoBD.PRV_IdTipoContrato = (short)propietario.IdTipoContrato;
                        PORepositorioAudit.MapearAuditModificarPropietarioVehiculo(contexto);
                    }
                    else
                    {
                        propietarioVehiculoBD = new PropietarioVehiculo_CPO()
                        {
                            PRV_IdPropietarioVehiculo = perExtPropietario.PEE_IdPersonaExterna,
                            PRV_CreadoPor = ControllerContext.Current.Usuario,
                            PRV_FechaGrabacion = DateTime.Now,
                            PRV_IdTipoContrato = (short)propietario.IdTipoContrato
                        };
                        contexto.PropietarioVehiculo_CPO.Add(propietarioVehiculoBD);
                    }
                }
                PropietarioVehiculoVehiculo_CPO propietarioVeVeBD = new PropietarioVehiculoVehiculo_CPO()
                {
                    PVV_IdPropietarioVehiculo = perExtPropietario.PEE_IdPersonaExterna,
                    PVV_CreadoPor = ControllerContext.Current.Usuario,
                    PVV_FechaGrabacion = DateTime.Now,
                    PVV_IdVehiculo = idVehiculo,
                };
                contexto.PropietarioVehiculoVehiculo_CPO.Add(propietarioVeVeBD);
                contexto.SaveChanges();
            }
        }

        public void EditarPropietario(POPropietarioVehiculo propietario)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR perExtPropietario = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == propietario.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == propietario.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();
                if (perExtPropietario != null)
                {
                    perExtPropietario.PEE_DigitoVerificacion = propietario.PersonaExterna.DigitoVerificacion;
                    perExtPropietario.PEE_Direccion = propietario.PersonaExterna.Direccion;
                    perExtPropietario.PEE_FechaExpedicionDocumento = propietario.PersonaExterna.FechaExpedicionDocumento;
                    perExtPropietario.PEE_Identificacion = propietario.PersonaExterna.Identificacion.Trim();
                    perExtPropietario.PEE_IdTipoIdentificacion = propietario.PersonaExterna.IdTipoIdentificacion;
                    perExtPropietario.PEE_Municipio = propietario.CiudadPropietario.IdLocalidad;
                    perExtPropietario.PEE_NumeroCelular = propietario.PersonaExterna.NumeroCelular;
                    perExtPropietario.PEE_PrimerApellido = propietario.PersonaExterna.PrimerApellido;
                    perExtPropietario.PEE_PrimerNombre = propietario.PersonaExterna.PrimerNombre;
                    perExtPropietario.PEE_SegundoApellido = propietario.PersonaExterna.SegundoApellido;
                    perExtPropietario.PEE_SegundoNombre = propietario.PersonaExterna.SegundoNombre;
                    perExtPropietario.PEE_Telefono = propietario.PersonaExterna.Telefono;

                    PORepositorioAudit.MapearAuditModificarPersonaExterna(contexto);

                    PropietarioVehiculo_CPO propietarioVehiculoBD = contexto.PropietarioVehiculo_CPO
                        .Where(prV => prV.PRV_IdPropietarioVehiculo == perExtPropietario.PEE_IdPersonaExterna)
                        .FirstOrDefault();

                    if (propietarioVehiculoBD != null)
                    {
                        propietarioVehiculoBD.PRV_IdTipoContrato = (short)propietario.IdTipoContrato;
                        PORepositorioAudit.MapearAuditModificarPropietarioVehiculo(contexto);
                    }
                    contexto.SaveChanges();
                }
            }
        }

        public void EditarTenedor(POTenedorVehiculo tenedor)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR perExtTenedor = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == tenedor.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == tenedor.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();
                if (perExtTenedor != null)
                {
                    perExtTenedor.PEE_DigitoVerificacion = tenedor.PersonaExterna.DigitoVerificacion;
                    perExtTenedor.PEE_Direccion = tenedor.PersonaExterna.Direccion;
                    perExtTenedor.PEE_FechaExpedicionDocumento = tenedor.PersonaExterna.FechaExpedicionDocumento;
                    perExtTenedor.PEE_Identificacion = tenedor.PersonaExterna.Identificacion.Trim();
                    perExtTenedor.PEE_IdTipoIdentificacion = tenedor.PersonaExterna.IdTipoIdentificacion;
                    perExtTenedor.PEE_Municipio = tenedor.CiudadTenedor.IdLocalidad;
                    perExtTenedor.PEE_NumeroCelular = tenedor.PersonaExterna.NumeroCelular;
                    perExtTenedor.PEE_PrimerApellido = tenedor.PersonaExterna.PrimerApellido;
                    perExtTenedor.PEE_PrimerNombre = tenedor.PersonaExterna.PrimerNombre;
                    perExtTenedor.PEE_SegundoApellido = tenedor.PersonaExterna.SegundoApellido;
                    perExtTenedor.PEE_SegundoNombre = tenedor.PersonaExterna.SegundoNombre;
                    perExtTenedor.PEE_Telefono = tenedor.PersonaExterna.Telefono;
                    PORepositorioAudit.MapearAuditModificarPersonaExterna(contexto);

                    TenedorVehiculo_CPO tenedorVehiculoBD = contexto.TenedorVehiculo_CPO
                    .Where(prV => prV.TEV_IdTenedorVehiculo == perExtTenedor.PEE_IdPersonaExterna)
                    .FirstOrDefault();

                    if (tenedorVehiculoBD != null)
                    {
                        tenedorVehiculoBD.TEV_IdCategoriaLicencia = (short)tenedor.IdCategoriaLicencia;
                        tenedorVehiculoBD.TEV_NumeroCelular2 = tenedor.NumeroCelular2;
                        tenedorVehiculoBD.TEV_NumeroLicencia = tenedor.NumeroLicencia;
                        PORepositorioAudit.MapearAuditModificarTenedorVehiculo(contexto);
                    }
                    contexto.SaveChanges();
                }
            }
        }


        public void ModificarTenedor(POTenedorVehiculo tenedor, int idVehiculo)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TenedorVehiculo_CPO tenedorVehiculoBD;
                TenedorVehiculoVehiculo_CPO tenedorVeVe = contexto.TenedorVehiculoVehiculo_CPO.Where(teV => teV.TVV_IdVehiculo == idVehiculo).FirstOrDefault();

                if (tenedorVeVe != null)
                {
                    contexto.TenedorVehiculoVehiculo_CPO.Remove(tenedorVeVe);
                    PORepositorioAudit.MapearAuditModificarTenedorVehiculoVehiculo(contexto);
                }

                PersonaExterna_PAR perExtTenedor = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion.Trim() == tenedor.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == tenedor.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();
                if (perExtTenedor == null)
                {
                    perExtTenedor = new PersonaExterna_PAR()
                    {
                        PEE_CreadoPor = ControllerContext.Current.Usuario,
                        PEE_FechaGrabacion = DateTime.Now,
                        PEE_DigitoVerificacion = tenedor.PersonaExterna.DigitoVerificacion,
                        PEE_Direccion = tenedor.PersonaExterna.Direccion,
                        PEE_FechaExpedicionDocumento = tenedor.PersonaExterna.FechaExpedicionDocumento,
                        PEE_Identificacion = tenedor.PersonaExterna.Identificacion.Trim(),
                        PEE_IdTipoIdentificacion = tenedor.PersonaExterna.IdTipoIdentificacion,
                        PEE_Municipio = tenedor.CiudadTenedor.IdLocalidad,
                        PEE_NumeroCelular = tenedor.PersonaExterna.NumeroCelular,
                        PEE_PrimerApellido = tenedor.PersonaExterna.PrimerApellido,
                        PEE_PrimerNombre = tenedor.PersonaExterna.PrimerNombre,
                        PEE_SegundoApellido = tenedor.PersonaExterna.SegundoApellido,
                        PEE_SegundoNombre = tenedor.PersonaExterna.SegundoNombre,
                        PEE_Telefono = tenedor.PersonaExterna.Telefono,
                    };

                    tenedorVehiculoBD = new TenedorVehiculo_CPO()
                    {
                        TEV_IdTenedorVehiculo = perExtTenedor.PEE_IdPersonaExterna,
                        TEV_CreadoPor = ControllerContext.Current.Usuario,
                        TEV_FechaGrabacion = DateTime.Now,
                        TEV_IdCategoriaLicencia = (short)tenedor.IdCategoriaLicencia,
                        TEV_NumeroCelular2 = tenedor.NumeroCelular2,
                        TEV_NumeroLicencia = tenedor.NumeroLicencia
                    };

                    contexto.PersonaExterna_PAR.Add(perExtTenedor);
                    contexto.TenedorVehiculo_CPO.Add(tenedorVehiculoBD);
                }
                else
                {
                    perExtTenedor.PEE_DigitoVerificacion = tenedor.PersonaExterna.DigitoVerificacion;
                    perExtTenedor.PEE_Direccion = tenedor.PersonaExterna.Direccion;
                    perExtTenedor.PEE_FechaExpedicionDocumento = tenedor.PersonaExterna.FechaExpedicionDocumento;
                    perExtTenedor.PEE_Identificacion = tenedor.PersonaExterna.Identificacion.Trim();
                    perExtTenedor.PEE_IdTipoIdentificacion = tenedor.PersonaExterna.IdTipoIdentificacion;
                    perExtTenedor.PEE_Municipio = tenedor.CiudadTenedor.IdLocalidad;
                    perExtTenedor.PEE_NumeroCelular = tenedor.PersonaExterna.NumeroCelular;
                    perExtTenedor.PEE_PrimerApellido = tenedor.PersonaExterna.PrimerApellido;
                    perExtTenedor.PEE_PrimerNombre = tenedor.PersonaExterna.PrimerNombre;
                    perExtTenedor.PEE_SegundoApellido = tenedor.PersonaExterna.SegundoApellido;
                    perExtTenedor.PEE_SegundoNombre = tenedor.PersonaExterna.SegundoNombre;
                    perExtTenedor.PEE_Telefono = tenedor.PersonaExterna.Telefono;
                    PORepositorioAudit.MapearAuditModificarPersonaExterna(contexto);

                    tenedorVehiculoBD = contexto.TenedorVehiculo_CPO
                        .Where(prV => prV.TEV_IdTenedorVehiculo == perExtTenedor.PEE_IdPersonaExterna)
                        .FirstOrDefault();

                    if (tenedorVehiculoBD != null)
                    {
                        tenedorVehiculoBD.TEV_IdCategoriaLicencia = (short)tenedor.IdCategoriaLicencia;
                        tenedorVehiculoBD.TEV_NumeroCelular2 = tenedor.NumeroCelular2;
                        tenedorVehiculoBD.TEV_NumeroLicencia = tenedor.NumeroLicencia;
                        PORepositorioAudit.MapearAuditModificarTenedorVehiculo(contexto);
                    }
                    else
                    {
                        tenedorVehiculoBD = new TenedorVehiculo_CPO()
                        {
                            TEV_IdTenedorVehiculo = perExtTenedor.PEE_IdPersonaExterna,
                            TEV_CreadoPor = ControllerContext.Current.Usuario,
                            TEV_FechaGrabacion = DateTime.Now,
                            TEV_IdCategoriaLicencia = (short)tenedor.IdCategoriaLicencia,
                            TEV_NumeroCelular2 = tenedor.NumeroCelular2,
                            TEV_NumeroLicencia = tenedor.NumeroLicencia
                        };
                        contexto.TenedorVehiculo_CPO.Add(tenedorVehiculoBD);
                    }
                }

                TenedorVehiculoVehiculo_CPO tenedorVeVeBD = new TenedorVehiculoVehiculo_CPO()
                {
                    TVV_IdTenedorVehiculo = perExtTenedor.PEE_IdPersonaExterna,
                    TVV_CreadoPor = ControllerContext.Current.Usuario,
                    TVV_FechaGrabacion = DateTime.Now,
                    TVV_IdVehiculo = idVehiculo,
                };
                contexto.TenedorVehiculoVehiculo_CPO.Add(tenedorVeVeBD);
                contexto.SaveChanges();
            }
        }


        /// <summary>
        /// Agregar una nueva posicion (longitud laitud) de un mensajero
        /// </summary>
        /// <param name="posicionMensajero"></param>        
        public void AgregarPosicionMensajero(POUbicacionMensajero posicionMensajero)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paAgregarPosicionMensajero_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PME_IdMensajero", posicionMensajero.IdMensajero);
                cmd.Parameters.AddWithValue("@PME_IdDispositivo", posicionMensajero.IdDispositivo);
                cmd.Parameters.AddWithValue("@PME_Longitud", posicionMensajero.Longitud);
                cmd.Parameters.AddWithValue("@PME_Latitud", posicionMensajero.Latitud);
                cmd.Parameters.AddWithValue("@PME_IdLocalidad", posicionMensajero.IdLocalidad);
                cmd.Parameters.AddWithValue("@PME_CreadoPor", ControllerContext.Current.Usuario);
                /******************************** Id Aplicacion Origen ***********************************************/
                if (ControllerContext.Current.IdAplicativoOrigen > 0)
                {
                    cmd.Parameters.AddWithValue("@PME_IdOrigenAplicacion", ControllerContext.Current.IdAplicativoOrigen);
                }
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }

        /// <summary>
        /// Obtiene las ubicaciones de un mensajero en un rango de fechas determinado
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="fechaInicial"></param>
        /// <param name="fechaFinal"></param>
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUbicacionesMensajero(long idMensajero, DateTime fechaInicial, DateTime fechaFinal)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPosicionesMensajero_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PME_IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@FechaInicial", fechaInicial);
                cmd.Parameters.AddWithValue("@FechaFinal", fechaFinal);
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                List<POUbicacionMensajero> retorno = new List<POUbicacionMensajero>();
                while (reader.Read())
                {
                    POUbicacionMensajero ubicacion = new POUbicacionMensajero()
                    {
                        IdDispositivo = Convert.ToInt64(reader["PME_IdDispositivo"]),
                        IdMensajero = Convert.ToInt64(reader["PME_IdMensajero"]),
                        Latitud = decimal.Parse(reader["PME_Latitud"].ToString(), System.Globalization.NumberStyles.Float),
                        Longitud = decimal.Parse(reader["PME_Longitud"].ToString(), System.Globalization.NumberStyles.Float),
                        Mensajero = new POMensajero()
                        {
                            Nombre = reader["PEI_Nombre"].ToString(),
                            PersonaInterna = new PAPersonaInternaDC()
                            {
                                Nombre = reader["PEI_Nombre"].ToString(),
                                PrimerApellido = reader["PEI_PrimerApellido"].ToString(),
                                Identificacion = reader["PEI_Identificacion"].ToString(),
                                IdTipoIdentificacion = reader["PEI_IdTipoIdentificacion"].ToString(),
                                NombreCompleto = reader["PEI_Nombre"].ToString() + " " + reader["PEI_PrimerApellido"].ToString()
                            }
                        }

                    };
                    retorno.Add(ubicacion);

                }

                conn.Close();

                return retorno;
            }
        }




        /// <summary>
        /// Obtiene la ultima posicion registrada de un mensajero en el dia actual
        /// </summary>
        /// <param name="idMensajero"></param>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaUbicacionMensajeroDiaActual(long idMensajero)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUltimaPosicionRegistradaMensajeroDiaActual_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PME_IdMensajero", idMensajero);
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                List<POUbicacionMensajero> retorno = new List<POUbicacionMensajero>();
                while (reader.Read())
                {
                    POUbicacionMensajero ubicacion = new POUbicacionMensajero()
                    {
                        IdDispositivo = Convert.ToInt64(reader["PME_IdDispositivo"]),
                        IdMensajero = Convert.ToInt64(reader["PME_IdMensajero"]),
                        Latitud = decimal.Parse(reader["PME_Latitud"].ToString(), System.Globalization.NumberStyles.Float),
                        Longitud = decimal.Parse(reader["PME_Longitud"].ToString(), System.Globalization.NumberStyles.Float),
                        Mensajero = new POMensajero()
                        {
                            Nombre = reader["PEI_Nombre"].ToString(),
                            PersonaInterna = new PAPersonaInternaDC()
                            {
                                Nombre = reader["PEI_Nombre"].ToString(),
                                PrimerApellido = reader["PEI_PrimerApellido"].ToString(),
                                Identificacion = reader["PEI_Identificacion"].ToString(),
                                IdTipoIdentificacion = reader["PEI_IdTipoIdentificacion"].ToString(),
                                NombreCompleto = reader["PEI_Nombre"].ToString() + " " + reader["PEI_PrimerApellido"].ToString()
                            }
                        }

                    };
                    retorno.Add(ubicacion);

                }

                conn.Close();

                return retorno;
            }
        }

        /// <summary>
        /// Obtiene la ultima posicion (del dia actual) de todos los mensajeros
        /// </summary>        
        /// <returns></returns>
        public List<POUbicacionMensajero> ObtenerUltimaPosicionTodosMensajeros()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUltimaPosicionTodosMensajeros_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                List<POUbicacionMensajero> retorno = new List<POUbicacionMensajero>();
                while (reader.Read())
                {
                    POUbicacionMensajero ubicacion = new POUbicacionMensajero()
                    {
                        IdDispositivo = Convert.ToInt64(reader["PME_IdDispositivo"]),
                        IdMensajero = Convert.ToInt64(reader["PME_IdMensajero"]),
                        Latitud = decimal.Parse(reader["PME_Latitud"].ToString(), System.Globalization.NumberStyles.Float),
                        Longitud = decimal.Parse(reader["PME_Longitud"].ToString(), System.Globalization.NumberStyles.Float),
                        IdLocalidad = reader["PME_IdLocalidad"].ToString(),
                        Mensajero = new POMensajero()
                        {
                            Nombre = reader["PEI_Nombre"].ToString(),
                            PersonaInterna = new PAPersonaInternaDC()
                            {
                                Nombre = reader["PEI_Nombre"].ToString(),
                                PrimerApellido = reader["PEI_PrimerApellido"].ToString(),
                                Identificacion = reader["PEI_Identificacion"].ToString(),
                                IdTipoIdentificacion = reader["PEI_IdTipoIdentificacion"].ToString(),
                                NombreCompleto = reader["PEI_Nombre"].ToString() + " " + reader["PEI_PrimerApellido"].ToString()
                            }
                        }

                    };
                    retorno.Add(ubicacion);

                }

                conn.Close();

                return retorno;
            }
        }

        public List<POPersonaExterna> ObtenerPersonasExternas()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodasPersonasExternas_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                List<POPersonaExterna> retorno = new List<POPersonaExterna>();
                while (reader.Read())
                {
                    POPersonaExterna persona = new POPersonaExterna()
                    {
                        IdPersona = Convert.ToInt64(reader["PEE_IdPersonaExterna"])
                        ,
                        IdTipoIdentificacion = reader["PEE_IdTipoIdentificacion"].ToString()
                        ,
                        Identificacion = reader["PEE_Identificacion"].ToString()
                        ,
                        DigitoVerificacion = reader["PEE_DigitoVerificacion"].ToString()
                        ,
                        FechaExpedicionDocumento = Convert.ToDateTime(reader["PEE_FechaExpedicionDocumento"])
                        ,
                        PrimerNombre = reader["PEE_PrimerNombre"].ToString()
                        ,
                        SegundoNombre = reader["PEE_SegundoNombre"].ToString()
                        ,
                        PrimerApellido = reader["PEE_PrimerApellido"].ToString()
                        ,
                        SegundoApellido = reader["PEE_SegundoApellido"].ToString()
                        ,
                        Direccion = reader["PEE_Direccion"].ToString()
                        ,
                        Municipio = reader["PEE_Municipio"].ToString()
                        ,
                        Telefono = reader["PEE_Telefono"].ToString()
                        ,
                        NumeroCelular = reader["PEE_NumeroCelular"].ToString()
                        ,
                        FechaGrabacion = Convert.ToDateTime(reader["PEE_FechaGrabacion"])
                        ,
                        CreadoPor = reader["PEE_CreadoPor"].ToString()
                        ,
                        Email = reader["PEE_Email"].ToString()
                    };
                    retorno.Add(persona);

                }
                conn.Close();

                return retorno;
            }
        }

        public void ActualizarPersonaExterna(POPersonaExterna persona)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR personaExterna = contexto.PersonaExterna_PAR.Where(p => p.PEE_IdPersonaExterna == persona.IdPersona).FirstOrDefault();

                personaExterna.PEE_Direccion = string.IsNullOrEmpty(persona.Direccion) ? personaExterna.PEE_Direccion : persona.Direccion;
                personaExterna.PEE_PrimerNombre = string.IsNullOrEmpty(persona.PrimerNombre) ? personaExterna.PEE_PrimerNombre : persona.PrimerNombre;
                personaExterna.PEE_PrimerApellido = string.IsNullOrEmpty(persona.PrimerApellido) ? personaExterna.PEE_PrimerApellido : persona.PrimerApellido;
                personaExterna.PEE_Telefono = string.IsNullOrEmpty(persona.Telefono) ? personaExterna.PEE_Telefono : persona.Telefono;
                personaExterna.PEE_NumeroCelular = string.IsNullOrEmpty(persona.NumeroCelular) ? personaExterna.PEE_NumeroCelular : persona.NumeroCelular;
                personaExterna.PEE_Email = string.IsNullOrEmpty(persona.Email) ? personaExterna.PEE_Email : persona.Email;

                contexto.SaveChanges();
            }
        }

        public void AdicionarPersonaExterna(POPersonaExterna persona)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCrearPersonaExterna_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PEE_IdTipoIdentificacion", persona.IdTipoIdentificacion);
                cmd.Parameters.AddWithValue("@PEE_Identificacion", persona.Identificacion);
                cmd.Parameters.AddWithValue("@PEE_DigitoVerificacion", persona.DigitoVerificacion);
                cmd.Parameters.AddWithValue("@PEE_FechaExpedicionDocumento", DateTime.Now);
                cmd.Parameters.AddWithValue("@PEE_PrimerNombre", persona.PrimerNombre);
                cmd.Parameters.AddWithValue("@PEE_SegundoNombre", persona.SegundoNombre);
                cmd.Parameters.AddWithValue("@PEE_PrimerApellido", persona.PrimerApellido);
                cmd.Parameters.AddWithValue("@PEE_SegundoApellido", persona.SegundoApellido);
                cmd.Parameters.AddWithValue("@PEE_Direccion", persona.Direccion);
                cmd.Parameters.AddWithValue("@PEE_Municipio", persona.Municipio);
                cmd.Parameters.AddWithValue("@PEE_Telefono", persona.Telefono);
                cmd.Parameters.AddWithValue("@PEE_NumeroCelular", persona.NumeroCelular);
                cmd.Parameters.AddWithValue("@PEE_FechaGrabacion", Convert.ToDateTime(persona.FechaGrabacion));
                cmd.Parameters.AddWithValue("@PEE_CreadoPor", persona.CreadoPor);
                cmd.Parameters.AddWithValue("@PEE_Email", persona.Email);
                conn.Open();
                cmd.ExecuteScalar();
                conn.Close();
            }
        }

        public void EliminarPersonaExterna(long idPersona)
        {
            using (ModeloParametrosOperacion contexto = new ModeloParametrosOperacion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR personaExterna = contexto.PersonaExterna_PAR.Where(p => p.PEE_IdPersonaExterna == idPersona).FirstOrDefault();
                contexto.PersonaExterna_PAR.Remove(personaExterna);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para obtener la ultima posicion mensajero por numero de guia 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public POUbicacionMensajero ObtenerUltimaPosicionMensajeroPorNumeroGuia(long numeroGuia)
        {
            POUbicacionMensajero poUbicacionMensajero = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUltimaPosicionMensajeroPorNumeroGuia_CPO", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    poUbicacionMensajero = new POUbicacionMensajero();
                    poUbicacionMensajero.Latitud = reader["UPM_Latitud"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["UPM_Latitud"]);
                    poUbicacionMensajero.Longitud = reader["UPM_Longitud"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["UPM_Longitud"]);
                }
            }
            return poUbicacionMensajero;
        }
    }
}