using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Threading;
using System.Transactions;


namespace CO.Servidor.OperacionUrbana.Datos
{
    public partial class OURepositorio : ControllerBase
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Campos

        /// <summary>
        /// Se intercepta clase de repositorio porque ya estaba avanzado el desarrollo y no estaban interceptadas la clases. Esto se acuerda con Diego para evitar inconvenientes. 22/02/2013 Ronald Ramírez
        /// </summary>
        private static readonly OURepositorio instancia = (OURepositorio)FabricaInterceptores.GetProxy(new OURepositorio(), COConstantesModulos.MODULO_OPERACION_URBANA);

        private const string NombreModelo = "ModeloOperacionUrbana";

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringControllerAud = ConfigurationManager.ConnectionStrings["ControllerExcepciones"].ConnectionString;
        //ControllerExcepciones
        #endregion Campos

        #region Propiedades

        public static OURepositorio Instancia
        {
            get { return OURepositorio.instancia; }
        }

        #endregion Propiedades

        #region mensajero

        public string EstadosMensajeros(string estadoMensajero)
        {
            string mensajeEstado = String.Empty;
            if (OUEnumEstadosMensajero.ACT.ToString().CompareTo(estadoMensajero) == 0)
                mensajeEstado = OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.IN_ACTIVO);
            else if (OUEnumEstadosMensajero.INA.ToString().CompareTo(estadoMensajero) == 0)
                mensajeEstado = OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.IN_INACTIVO);
            else if (OUEnumEstadosMensajero.SUS.ToString().CompareTo(estadoMensajero) == 0)
                mensajeEstado = OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.IN_SUSPENDIDO);

            return mensajeEstado;
        }

        /// <summary>
        /// Método para obtener los mensajeros por localidad
        /// </summary>
        /// <param name="Localidad"></param>
        /// <returns></returns>
        public IEnumerable<POMensajero> ObtenerMensajerosLocalidad(string Localidad)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var mensajeros = contexto.paObtenerMensajerosLocalidad_OPU(Localidad).ToList();
                if (mensajeros != null && mensajeros.Any())
                {
                    return mensajeros.ConvertAll(r => new POMensajero()
                    {
                        Nombre = r.PEI_Nombre + "" + r.PEI_PrimerApellido,
                        IdMensajero = r.MEN_IdMensajero,
                        IdTipoMensajero = r.MEN_IdTipoMensajero,
                        PersonaInterna = new PAPersonaInternaDC()
                        {
                            Identificacion = r.PEI_Identificacion,
                        },
                        IdAgencia = r.MEN_IdAgencia,
                        Estado = r.MEN_Estado
                    });
                }
                else
                    return null;
            }
        }

        #endregion mensajero

        #region tipo mensajero

        /// <summary>
        /// Obtiene los Tipos de mensajero de la DB
        /// </summary>
        /// <returns>Lista con los tipos de mensajero BD</returns>
        public IEnumerable<OUTipoMensajeroDC> ObtenerTiposMensajero()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoMensajero_CPO.OrderBy(o => o.TIM_Descripcion).ToList().ConvertAll(r => new OUTipoMensajeroDC()
                {
                    Descripcion = r.TIM_Descripcion,
                    IdTipoMensajero = r.TIM_IdTipoMensajero
                });
            }
        }

        /// <summary>
        /// retorna el nombre de los mensajeros que pertenecen al centro logistico
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <returns></returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajerosCOL(long idCentroLogistico)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MensajerosAgenciaCol_VOPU.Where(r => r.AGE_IdCentroLogistico == idCentroLogistico).
                  ToList().
                  ConvertAll(r => new OUNombresMensajeroDC()
                  {
                      NombreApellido = r.NombreCompleto,
                      IdPersonaInterna = r.PEI_IdPersonaInterna,
                      Identificacion = r.PEI_Identificacion
                  });
            }
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgencia(long idAgencia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MensajerosAgenciaCol_VOPU
                    .Where(Agencia => Agencia.AGE_IdAgencia == idAgencia &&
                           Agencia.MEN_Estado == "ACT")
                  .OrderBy(m => m.NombreCompleto).ToList()
                  .ConvertAll(r => new OUNombresMensajeroDC()
                  {
                      NombreApellido = r.NombreCompleto,
                      IdPersonaInterna = r.PEI_IdPersonaInterna,
                      Identificacion = r.PEI_Identificacion,
                      IdCentroLogistico = r.AGE_IdCentroLogistico,
                      IdTipoMensajero = r.MEN_IdTipoMensajero,
                      DescripcionTipomensajero = r.TIM_Descripcion,
                      CargoMensajero = r.CAR_Descripcion,
                      EsMensajeroUrbano = r.MEN_EsMensajeroUrbano,
                      IdMensajero = r.MEN_IdMensajero,
                  });
            }
        }

        /// <summary>
        /// Método que devuelve la información del mensajero a partir del Id
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public OUNombresMensajeroDC ObtenerMensajeroporId(long idMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MensajerosAgenciaCol_VOPU men = contexto.MensajerosAgenciaCol_VOPU.Where(m => m.PEI_IdPersonaInterna == idMensajero).FirstOrDefault();
                if (men == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_MENSAJERO_NO_EXISTE.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_MENSAJERO_NO_EXISTE)));
                else
                {
                    return new OUNombresMensajeroDC()
                    {
                        NombreApellido = men.NombreCompleto,
                        IdPersonaInterna = men.PEI_IdPersonaInterna,
                        Identificacion = men.PEI_Identificacion,
                        IdCentroLogistico = men.AGE_IdCentroLogistico,
                        IdTipoMensajero = men.MEN_IdTipoMensajero,
                        DescripcionTipomensajero = men.TIM_Descripcion
                    };
                }
            }
        }

        /// <summary>
        /// Obtene los datos del mensajero de la agencia.
        /// </summary>
        /// <param name="idAgencia">Es el id agencia.</param>
        /// <returns>la lista de mensajeros de una agencia</returns>
        public IEnumerable<OUNombresMensajeroDC> ObtenerNombreMensajeroAgenciaPag(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                              int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idAgencia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<MensajerosAgenciaCol_VOPU>("AGE_IdAgencia", idAgencia.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarMensajerosAgenciaCol_VOPU(filtro, where, "PEI_Identificacion", out totalRegistros,
                                                      indicePagina, registrosPorPagina, false)
                                                      .ToList()
                                                      .ConvertAll(r => new OUNombresMensajeroDC()
                                                      {
                                                          NombreApellido = r.NombreCompleto,
                                                          IdPersonaInterna = r.PEI_IdPersonaInterna,
                                                          Identificacion = r.PEI_Identificacion,
                                                          IdCentroLogistico = r.AGE_IdCentroLogistico,
                                                          IdTipoMensajero = r.MEN_IdTipoMensajero,
                                                          DescripcionTipomensajero = r.TIM_Descripcion,
                                                          IdMensajero = r.MEN_IdMensajero
                                                      });
            }
        }

        #endregion tipo mensajero

        #region Persona Interna

        //////public void AdicionarMensajero(OUMensajeroDC mensajero)
        //////{
        //////  long id;
        //////  using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        //////  {
        //////    PersonaInterna_PAR persona = null;
        //////    PersonaInterna_PAR personaEn = contexto.PersonaInterna_PAR
        //////      .Where(r => r.PEI_Identificacion == mensajero.PersonaInterna.Identificacion)
        //////      .SingleOrDefault();

        //////    if (personaEn == null)
        //////    {
        //////      persona = new PersonaInterna_PAR()
        //////      {
        //////        PEI_Identificacion = mensajero.PersonaInterna.Identificacion,
        //////        PEI_IdTipoIdentificacion = mensajero.PersonaInterna.IdTipoIdentificacion,
        //////        PEI_Nombre = mensajero.PersonaInterna.Nombre,
        //////        PEI_Direccion = mensajero.PersonaInterna.Direccion,
        //////        PEI_Email = mensajero.PersonaInterna.Email,
        //////        PEI_Municipio = mensajero.PersonaInterna.IdMunicipio,
        //////        PEI_PrimerApellido = mensajero.PersonaInterna.PrimerApellido,
        //////        PEI_SegundoApellido = mensajero.PersonaInterna.SegundoApellido,
        //////        PEI_Telefono = mensajero.PersonaInterna.Telefono,
        //////        PEI_IdCargo = mensajero.PersonaInterna.IdCargo,
        //////        PEI_IdRegionalAdm = mensajero.PersonaInterna.Regional,
        //////        PEI_Comentarios = mensajero.PersonaInterna.Comentarios,
        //////        PEI_CreadoPor = ControllerContext.Current.Usuario,
        //////        PEI_FechaGrabacion = DateTime.Now
        //////      };
        //////      contexto.PersonaInterna_PAR.Add(persona);
        //////      id = persona.PEI_IdPersonaInterna;
        //////    }
        //////    else
        //////    {
        //////      id = personaEn.PEI_IdPersonaInterna;
        //////    }

        //////    Mensajero_OPU mensajeroEn = new Mensajero_OPU()
        //////     {
        //////       MEN_EsContratista = mensajero.EsContratista,
        //////       MEN_Estado = mensajero.Estado.IdEstado,
        //////       MEN_FechaIngreso = mensajero.PersonaInterna.FechaInicioContrato,
        //////       MEN_FechaTerminacionContrato = mensajero.PersonaInterna.FechaTerminacionContrato,
        //////       MEN_FechaVencimientoPase = mensajero.FechaVencimientoPase,
        //////       MEN_IdTipoMensajero = mensajero.TipMensajeros.IdTipoMensajero,
        //////       MEN_NumeroPase = mensajero.NumeroPase,
        //////       MEN_Telefono2 = mensajero.Telefono2,
        //////       MEN_IdMensajero = persona.PEI_IdPersonaInterna,
        //////       MEN_CreadoPor = ControllerContext.Current.Usuario,
        //////       MEN_FechaGrabacion = DateTime.Now,
        //////       MEN_TipoContrato = mensajero.PersonaInterna.TipoContrato,
        //////       MEN_IdAgencia = mensajero.Agencia
        //////     };

        //////    contexto.Mensajero_OPU.Add(mensajeroEn);

        //////    contexto.SaveChanges();
        //////  }
        //////}

        #endregion Persona Interna

        #region Centro de Acopio

        /// <summary>
        /// Obtiene la informacion del mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerInfoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long centroLogistico)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MensajeroCentroLogistico_VOPU mensajeroInfo = contexto.ConsultarEqualsMensajeroCentroLogistico_VOPU(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .SingleOrDefault();

                if (mensajeroInfo == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_MENSAJERO_NO_EXISTE.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_MENSAJERO_NO_EXISTE)));

                OUGuiaIngresadaDC infoMensajero = new OUGuiaIngresadaDC()
                {
                    FechaActual = DateTime.Now,
                    IdMensajero = mensajeroInfo.MEN_IdMensajero,
                    NombreCompleto = mensajeroInfo.NombreCompleto,
                    TipoImpreso = ADEnumTipoImpreso.Planilla,
                    Identificacion = mensajeroInfo.PEI_Identificacion
                };
                return infoMensajero;
            }
        }

        /// <summary>
        /// Obtiene los mensajeros del centro logistico para el ingreso de una guia paginados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long centroLogistico)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("AGE_IdCentroLogistico", centroLogistico.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_Estado", OUConstantesOperacionUrbana.ESTADO_ACTIVO, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarMensajeroCentroLogistico_VOPU(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).
                  ToList().ConvertAll(r => new OUMensajeroDC()
                  {
                      NombreCompleto = r.NombreCompleto,
                      FechaActual = DateTime.Now,
                      IdMensajero = r.MEN_IdMensajero,
                      IdTipoMensajero = r.MEN_IdTipoMensajero,
                      TipMensajeros = new OUTipoMensajeroDC()
                      {
                          IdTipoMensajero = r.MEN_IdTipoMensajero,
                          // Descripcion = r.TIM_Descripcion

                      },
                      EsVehicular = r.TIM_EsVehicular,
                      PersonaInterna = new OUPersonaInternaDC()
                      {
                          Identificacion = r.PEI_Identificacion,
                      },
                      Agencia = r.MEN_IdAgencia,
                      Estado = new OUEstadosMensajeroDC()
                      {
                          IdEstado = r.MEN_Estado,
                          Descripcion = EstadosMensajeros(r.MEN_Estado)
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene todos los mensajeros de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensajerosCol(long idCol)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerMensajerosCol_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCol", idCol);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<OUMensajeroDC>(r =>
                    {
                        OUMensajeroDC mensajero = new OUMensajeroDC()
                        {
                            NombreCompleto = r.Field<string>("NombreCompleto"),
                            FechaActual = DateTime.Now,
                            IdMensajero = r.Field<long>("MEN_IdMensajero"),
                            IdTipoMensajero = r.Field<short>("MEN_IdTipoMensajero"),
                            TipMensajeros = new OUTipoMensajeroDC()
                            {
                                IdTipoMensajero = r.Field<short>("MEN_IdTipoMensajero"),
                                Descripcion = r.Field<string>("TIM_Descripcion")

                            },
                            EsVehicular = r.Field<bool>("TIM_EsVehicular"),
                            CargoMensajero = new Framework.Servidor.Servicios.ContratoDatos.Seguridad.SECargo()
                            {
                                IdCargo = r.Field<int>("PEI_IdCargo")
                            },
                            PersonaInterna = new OUPersonaInternaDC()
                            {
                                Identificacion = r.Field<string>("PEI_Identificacion"),
                            },
                            Agencia = r.Field<long>("MEN_IdAgencia")
                        };

                        return mensajero;

                    });

            }

        }


        /// <summary>
        /// Obtiene los mensajeros del centro logistico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroPorAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCol)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_IdAgencia", idCol.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_Estado", OUConstantesOperacionUrbana.ESTADO_ACTIVO, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                var resultado = contexto.ConsultarMensajeroCentroLogistico_VOPU(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList();

                return resultado
                  .ConvertAll(r => new OUMensajeroDC()
                  {
                      NombreCompleto = r.NombreCompleto,
                      FechaActual = DateTime.Now,
                      IdMensajero = r.MEN_IdMensajero,
                      IdTipoMensajero = r.MEN_IdTipoMensajero,
                      PersonaInterna = new OUPersonaInternaDC()
                      {
                          Identificacion = r.PEI_Identificacion,
                          IdMunicipio = r.PEI_Municipio,
                          Municipio = r.LOC_Nombre
                      },
                      Agencia = r.MEN_IdAgencia,
                      Estado = new OUEstadosMensajeroDC()
                      {
                          IdEstado = r.MEN_Estado,
                          Descripcion = EstadosMensajeros(r.MEN_Estado)
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene los mensajeros del centro logistico
        /// </summary>
        /// <param name="centroLogistico"></param>
        /// <returns></returns>
        public IList<OUMensajeroDC> ObtenerMensajeroPorRegional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idRacol)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("CEL_IdRegionalAdm", idRacol.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_Estado", OUConstantesOperacionUrbana.ESTADO_ACTIVO, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarContainsMensajeroCentroLogistico_VOPU(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).
                    ToList().ConvertAll(r => new OUMensajeroDC()
                    {
                        NombreCompleto = r.NombreCompleto,
                        FechaActual = DateTime.Now,
                        IdMensajero = r.MEN_IdMensajero,
                        IdTipoMensajero = r.MEN_IdTipoMensajero,
                        PersonaInterna = new OUPersonaInternaDC()
                        {
                            Identificacion = r.PEI_Identificacion,
                            IdMunicipio = r.PEI_Municipio,
                            Municipio = r.LOC_Nombre
                        },
                        Agencia = r.MEN_IdAgencia,
                        Estado = new OUEstadosMensajeroDC()
                        {
                            IdEstado = r.MEN_Estado,
                            Descripcion = EstadosMensajeros(r.MEN_Estado)
                        }
                    });
            }
        }

        /// <summary>
        /// Retorna la planilla de recogidas de una solicitud de recogida
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public long ObtenerPlanillaRecogidaSolicitud(long idSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long idPlanilla;
                var planilla = contexto.SolicitudProgramadaPlanill_OPU.Where(r => r.SPP_IdSolicitudProgramadaPlanill == idSolicitud).FirstOrDefault();

                if (planilla == null)
                    idPlanilla = 0;
                else
                    idPlanilla = planilla.SPP_IdPlanillaSolicitudRecogid;

                return idPlanilla;
            }
        }

        /// <summary>
        /// consulta la guia por el número
        /// </summary>
        /// <param name="guiaIngresada"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaGuia(OUGuiaIngresadaDC guiaIngresada)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaSistema = DateTime.Now;
                int horaActual = fechaSistema.Hour;

                // fecha inicial para la consulta
                DateTime inicial = fechaSistema.Date.AddDays(-1);

                PlanillaVentasGuia_OPU planilla = new PlanillaVentasGuia_OPU();

                //consulta las planillas del dia anterior
                if (horaActual > 0 && horaActual < 7)
                {
                    inicial = fechaSistema.Date.AddDays(-2);
                    planilla = contexto.PlanillaVentasGuia_OPU
                      .Where(r => r.PVG_NumeroGuia == guiaIngresada.NumeroGuia
                        && r.PVG_FechaGrabacion < fechaSistema
                        && r.PVG_FechaGrabacion > inicial)
                      .FirstOrDefault();
                }//consulta las planilla del dia
                else
                {
                    planilla = contexto.PlanillaVentasGuia_OPU
                      .Where(r => r.PVG_NumeroGuia == guiaIngresada.NumeroGuia
                        && r.PVG_FechaGrabacion < fechaSistema
                        && r.PVG_FechaGrabacion > inicial)
                      .FirstOrDefault();
                }

                //si la guia no se encuentra planillada
                if (planilla == null)
                {
                    guiaIngresada.IngresoPorPlanilla = false;
                    guiaIngresada.Planilla = 0;
                }
                else
                {
                    guiaIngresada.IngresoPorPlanilla = true;
                    guiaIngresada.Planilla = planilla.PVG_IdPlanilla;
                }

                //valida si existe la guia en sistema
                var guiaRegistrada = contexto.paObtenerAdmisionMensajeri_MEN(guiaIngresada.NumeroGuia).FirstOrDefault();
                if (guiaRegistrada == null)
                {
                    guiaIngresada.IdAdmision = 0;
                    guiaIngresada.GuiaRegistrada = false;
                    guiaIngresada.DetalleGuia = null;
                }

                //la guia existe en sistema
                else
                {
                    guiaIngresada.GuiaRegistrada = true;
                    guiaIngresada.GuiaAutomatica = guiaRegistrada.ADM_EsAutomatico;
                    guiaIngresada.TipoCliente = guiaRegistrada.ADM_TipoCliente;
                    guiaIngresada.PesoSistema = guiaRegistrada.ADM_Peso;
                    guiaIngresada.NumeroGuia = guiaIngresada.NumeroGuia;
                    guiaIngresada.IdAdmision = guiaRegistrada.ADM_IdAdminisionMensajeria;
                    guiaIngresada.DetalleGuia = new OUDetalleGuiaDC()
                    {
                        CiudadDestino = guiaRegistrada.ADM_NombreCiudadDestino,
                        IdCiudadDestino = guiaRegistrada.ADM_IdCiudadDestino,
                        CiudadOrigen = guiaRegistrada.ADM_NombreCiudadOrigen,
                        IdCiudadOrigen = guiaRegistrada.ADM_IdCiudadOrigen,
                        IdTipoEnvio = guiaRegistrada.ADM_IdTipoEnvio
                    };
                    guiaIngresada.DetalleGuia.TipoEnvio = guiaRegistrada.ADM_NombreTipoEnvio;
                    guiaIngresada.ValorTotal = guiaRegistrada.ADM_ValorTotal;
                    guiaIngresada.TotalPiezas = guiaRegistrada.ADM_TotalPiezas;
                    guiaIngresada.IdCentroServicioDestino = guiaRegistrada.ADM_IdCentroServicioDestino;
                    guiaIngresada.NombreCentroServicioDestino = guiaRegistrada.ADM_NombreCentroServicioDestino;
                    guiaIngresada.IdCentroServicioOrigen = guiaRegistrada.ADM_IdCentroServicioOrigen;
                    guiaIngresada.NombreCentroServicioOrigen = guiaRegistrada.ADM_NombreCentroServicioOrigen;
                    guiaIngresada.EsAlCobro = guiaRegistrada.ADM_EsAlCobro;
                    guiaIngresada.EstaPagada = guiaRegistrada.ADM_EstaPagada;
                    guiaIngresada.CantidadReintentosEntrega = (short)guiaRegistrada.ADM_CantidadReintentosEntrega;
                }
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }
        }

        /// <summary>
        /// Si la guia no esta registrada en el sistema ingresa la guia al sistema
        /// </summary>
        /// <param name="guiaIngresada">Guía ingresada por el usuario</param>
        public OUGuiaIngresadaDC GuardaGuiaNoRegistrada(OUGuiaIngresadaDC guiaIngresada)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var validaGuia = contexto.paObtenerIngrGuiaNoReAgencia_OPU(guiaIngresada.NumeroGuia).FirstOrDefault();
                if (validaGuia != null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA)));

                short idEstadoEmapaque;
                if (guiaIngresada.MayorUnKg)
                {
                    idEstadoEmapaque = guiaIngresada.EstadoEmpaqueMayorUnKG.IdEstadoEmpaque;
                }
                else
                {
                    idEstadoEmapaque = guiaIngresada.EstadoEmpaqueMenorUnKG.IdEstadoEmpaque;
                }

                IngresoGuiaNoRegAgencia_OPU guiaEn = new IngresoGuiaNoRegAgencia_OPU()
                {
                    //IGN_PesoEnIngreso = guiaIngresada.Peso.Value,
                    //IGN_NumeroGuia = guiaIngresada.NumeroGuia.Value,
                    //IGN_IdAgencia = guiaIngresada.IdCentroLogistico,
                    //IGN_IdMensajero = guiaIngresada.IdMensajero,
                    //IGN_IdEstadoEmpaque = idEstadoEmapaque,
                    //IGN_Estado = OUConstantesOperacionUrbana.SIN_GUIA_CREADA,
                    //IGN_FechaCaptura = ConstantesFramework.MinDateTimeController,
                    //IGN_CreadoPor = ControllerContext.Current.Usuario,
                    //IGN_FechaGrabacion = DateTime.Now
                };

                contexto.IngresoGuiaNoRegAgencia_OPU.Add(guiaEn);
                contexto.SaveChanges();
                guiaIngresada.TotalEnvios = guiaIngresada.TotalEnvios + 1;
                guiaIngresada.IdAdmision = 0;
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }
        }

        /// <summary>
        /// Valida si la guia que se encuentra registrada ya fue descargada
        /// </summary>
        /// <param name="numGuia"></param>
        /// <returns>True "Guía Registrada" False="Guía sin registrar"</returns>
        public void ValidaGuiaRegistrada(long numGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var validaExisteGuia = contexto.paObtenerIngrGuiaAgencia_OPU(numGuia).FirstOrDefault();

                if (validaExisteGuia != null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA)));
            }
        }

        /// <summary>
        /// Valida si la guía que no se encuentra registrada ya fue descarga
        /// </summary>
        /// <param name="numGuia"></param>
        /// <returns></returns>
        public void ValidaGuiaNoRegistrada(long numGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var validaExisteGuia = contexto.paObtenerIngrGuiaNoReAgencia_OPU(numGuia).FirstOrDefault();
                if (validaExisteGuia != null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_YA_SE_ENCUENTRA_REGISTRADA)));
            }
        }

        /// <summary>
        /// Consulta el cliente convenio peaton
        /// </summary>
        /// <param name="idAdmision">Id admision Mensajeria</param>
        /// <returns>Guía con el id de la sucursal y el id del contrato</returns>
        public OUGuiaIngresadaDC ConsultaClienteConvenioPeaton(OUGuiaIngresadaDC guiaIngresada)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var convenioPeaton = contexto.paObtenerAdmisioConvPeaton_MEN(guiaIngresada.IdAdmision).FirstOrDefault();
                if (convenioPeaton == null)
                    guiaIngresada.IdSucursal = 0;
                else
                {
                    guiaIngresada.IdSucursal = convenioPeaton.MCP_IdSucursalRecogida;
                    guiaIngresada.IdContratoRemitente = convenioPeaton.MCP_IdContratoConvenioRemite;
                }
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }
        }

        /// <summary>
        /// consulta la sucursal y el id del contrato del cliente
        /// </summary>
        /// <param name="guiaIngresada"></param>
        /// <returns>Guía con el id de la sucursal y el id del contrato</returns>
        public OUGuiaIngresadaDC ConsultaClienteConvenioConvenio(OUGuiaIngresadaDC guiaIngresada)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var convenioConvenio = contexto.paObtenerAdmisionConvConv_MEN(guiaIngresada.IdAdmision).FirstOrDefault();

                if (convenioConvenio == null)
                    guiaIngresada.IdSucursal = 0;
                else
                {
                    guiaIngresada.IdSucursal = convenioConvenio.MCC_IdSucursalRecogida == null ? 0 : convenioConvenio.MCC_IdSucursalRecogida.Value;
                    guiaIngresada.IdContratoRemitente = convenioConvenio.MCC_IdContratoConvenioRemite;
                }
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }
        }

        /// <summary>
        /// Guarda los envios registrados en sistema
        /// </summary>
        public OUGuiaIngresadaDC GuardarGuiaIngresada(OUGuiaIngresadaDC guiaIngresada)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string estadoEmpaque;
                short idEstadoEmpaque;
                if (guiaIngresada.MayorUnKg)
                {
                    estadoEmpaque = guiaIngresada.EstadoEmpaqueMayorUnKG.DescripcionEstado;
                    idEstadoEmpaque = guiaIngresada.EstadoEmpaqueMayorUnKG.IdEstadoEmpaque;
                }
                else
                {
                    estadoEmpaque = guiaIngresada.EstadoEmpaqueMenorUnKG.DescripcionEstado;
                    idEstadoEmpaque = guiaIngresada.EstadoEmpaqueMenorUnKG.IdEstadoEmpaque;
                }

                IngresoGuiaAgencia_OPU guia = new IngresoGuiaAgencia_OPU()
                {
                    IGA_NumeroGuia = guiaIngresada.NumeroGuia.Value,
                    IGA_IdAdminisionMensajeria = guiaIngresada.IdAdmision,
                    IGA_IdAgencia = guiaIngresada.IdCentroLogistico,
                    //   IGA_IdCiudadDestino = guiaIngresada.DetalleGuia.IdCiudadDestino,
                    // //  IGA_PesoEnIngreso = guiaIngresada.Peso.Value,
                    //   IGA_IdMensajero = guiaIngresada.IdMensajero,
                    //   IGA_IdCiudadOrigen = guiaIngresada.DetalleGuia.IdCiudadOrigen,
                    //   IGA_IdTipoEnvio = guiaIngresada.DetalleGuia.IdTipoEnvio,
                    //  // IGA_NombreRutaDestino = guiaIngresada.DetalleGuia.RutaDestino,
                    ////   IGA_IdRutaDestino = guiaIngresada.DetalleGuia.IdRutaDestino,
                    //   IGA_NumeroPlanilla = guiaIngresada.Planilla,
                    //   IGA_IngresoPorPlanilla = guiaIngresada.IngresoPorPlanilla,
                    //   IGA_NombreCiudadDestino = guiaIngresada.DetalleGuia.CiudadDestino,
                    //   IGA_NombreCiudadOrigen = guiaIngresada.DetalleGuia.CiudadOrigen,
                    //   IGA_NombreTipoEnvio = guiaIngresada.DetalleGuia.TipoEnvio,
                    //   IGA_CreadoPor = ControllerContext.Current.Usuario,
                    //   IGA_FechaGrabacion = DateTime.Now,
                    //   IGA_DescripcionEstadoEmpaque = estadoEmpaque,
                    //   IGA_IdEstadoEmpaque = idEstadoEmpaque,
                    IGA_TotalPiezas = guiaIngresada.TotalPiezas.Value,
                    IGA_NumeroPieza = guiaIngresada.NumeroPiezas.Value
                };
                contexto.IngresoGuiaAgencia_OPU.Add(guia);
                contexto.SaveChanges();
                guiaIngresada.TotalEnvios = guiaIngresada.TotalEnvios + 1;
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }
        }

        /// <summary>
        /// Retorna el total de los envios planillados pendientes por descargar
        /// </summary>
        /// <returns>Total envios pendientes por planillar</returns>
        public int TotalEnviosPlanillados(long idMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaActual = DateTime.Now;
                DateTime fechaInicial;
                int horaActual = fechaActual.Hour;

                if (horaActual > 0 && horaActual < 7)
                    fechaInicial = fechaActual.Date.AddDays(-2);
                else
                    fechaInicial = fechaActual.Date.AddDays(-1);

                return contexto.PlanillaVentas_OPU.Where(r => r.PLA_IdMensajero == idMensajero
                   && r.PLA_FechaGrabacion < fechaActual
                   && r.PLA_FechaGrabacion > fechaInicial)
                  .ToList().Sum(r => r.PLA_TotalEnvios);
            }
        }

        #endregion Centro de Acopio

        #region Estados Empaque

        /// <summary>
        /// Obtiene los Estados del empaque
        /// </summary>
        /// <returns>Lista con los estados de los empaques</returns>
        public List<OUEstadosEmpaqueDC> ObtenerEstadosEmpaque()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoEmpaque_PAR.Where(r => r.TEE_PesoInicial >= 0).ToList().ConvertAll(r => new OUEstadosEmpaqueDC()
                {
                    DescripcionEstado = r.TEE_Descripcion,
                    IdEstadoEmpaque = r.TEE_IdEstadoEmpaque,
                    PesoFinal = r.TEE_PesoFinal,
                    PesoInicial = r.TEE_PesoInicial
                });
            }
        }

        //////////public List<OUEstadosEmpaqueDC> ObtenerEstadosEmpaque(decimal peso)
        //////////{
        //////////  using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        //////////  {
        //////////    return contexto.EstadoEmpaque_PAR.Where(r => r.TEE_PesoInicial < peso && r.TEE_PesoFinal >= peso).ToList().ConvertAll(r => new OUEstadosEmpaqueDC()
        //////////    {
        //////////      DescripcionEstado = r.TEE_Descripcion,
        //////////      IdEstadoEmpaque = r.TEE_IdEstadoEmpaque
        //////////    });
        //////////  }
        //////////}

        #endregion Estados Empaque

        #region Envios Pendientes

        /// <summary>
        /// Obtiene el total de los envios pendientes asignados por planilla de venta al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosPendientes(long idMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                // fecha inicial para la consulta
                DateTime inicial = DateTime.Now.AddDays(-7);

                return contexto.GuiasNoIngresadasPorPlani_VOPU.Where(r => r.PLA_IdMensajero == idMensajero && r.PLA_FechaGrabacion >= inicial && r.PLA_FechaGrabacion <= DateTime.Now).ToList().Count;
            }
        }

        public List<OUGuiasPendientesDC> ObtenerEnviosPendientes(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaSistema = DateTime.Now;
                int horaActual = fechaSistema.Hour;

                // fecha inicial para la consulta
                DateTime inicial = fechaSistema.Date.AddDays(-7);

                //consulta las planillas del dia anterior
                if (horaActual > 0 && horaActual < 7)
                    inicial = fechaSistema.Date.AddDays(-7);

                //consulta las planilla del dia
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<GuiasNoIngresadasPorPlani_VOPU>("PLA_IdMensajero", idMensajero.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);
                lambda = contexto.CrearExpresionLambda<GuiasNoIngresadasPorPlani_VOPU>("PLA_FechaGrabacion", inicial.ToString(), OperadorComparacion.Between, DateTime.Now.ToString());
                where.Add(lambda, OperadorLogico.And);
                return contexto.ConsultarGuiasNoIngresadasPorPlani_VOPU(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                 .ToList().ConvertAll(r => new OUGuiasPendientesDC()
                 {
                     NumeroGuia = r.PVG_NumeroGuia,
                     NombrePuntoServicio = r.PLA_NombrePuntoServicio,
                     IdPuntoServicio = r.PLA_IdPuntoServicio,
                     IdPlanillaVenta = r.PVG_IdPlanilla,
                     FechaPlanillaVenta = r.PLA_FechaGrabacion
                 });
            }
        }

        #endregion Envios Pendientes

        #region Planilla Ventas

        #region Consultas

        /// <summary>
        /// Consulta la planilla de ventas por id de planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public OUPlanillaVentaDC ObtenerPlanillaVentasPorIdPlanilla(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PlanillaVentasMensajero_VOPU.Where(p => p.PLA_IdPlanilla == idPlanilla).ToList().ConvertAll(r => new OUPlanillaVentaDC()
                {
                    TipoMensajero = r.TIM_Descripcion,
                    NombreCompleto = r.NombreCompleto,
                    IdTipoMensajero = r.MEN_IdTipoMensajero,
                    IdMensajero = r.MEN_IdMensajero,
                    IdPuntoServicio = r.PLA_IdPuntoServicio,
                    NumeroPlanilla = r.PLA_IdPlanilla,
                    FechaPlanilla = r.PLA_FechaGrabacion.ToShortDateString(),
                    IdVehiculo = r.PLA_IdVehiculo,
                    Placa = r.VEH_Placa
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene el total de envios de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public int ObtenerCantidadEnviosPorPlanilla(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? totalEnvios = contexto.paObtenerTotalPiezasPlanillaVentas_OPU(idPlanilla).FirstOrDefault().TotalEnvios;
                return totalEnvios != null ? totalEnvios.Value : 0;
            }
        }

        /// <summary>
        /// Obtiene el total de envios sueltos de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public int ObtenerCantidadEnviosSueltosPorPlanilla(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? totalEnvios = contexto.paObtenerTotalEnviosSueltosPlanillaVentas_OPU(idPlanilla).FirstOrDefault();
                return totalEnvios != null ? totalEnvios.Value : 0;
            }
        }

        /// <summary>
        /// Obtiene el total de envios de la planilla y NumConTransRetorno
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public int ObtenerCantidadEnviosPorPlanillaNumConTransRetorno(long idPlanilla, long conTransretorno)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? totalEnvios = contexto.paObtenerTotalPiezasPlanillaVentasNumConTransRetorno_OPU(idPlanilla, conTransretorno).FirstOrDefault();
                return totalEnvios != null ? totalEnvios.Value : 0;
            }
        }

        /// <summary>
        /// Obtiene el total de envios sueltos de la planilla y NumConTransRetorno
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public int ObtenerCantidadEnviosSueltosPorPlanillaNumConTransRetorno(long idPlanilla, long conTransretorno)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? totalEnvios = contexto.paObtenerTotalEnviosSueltosPlanillaVentasNumConTransRetorno_OPU(idPlanilla, conTransretorno).FirstOrDefault();
                return totalEnvios != null ? totalEnvios.Value : 0;
            }
        }

        /// <summary>
        /// Obtiene el total de envios dentro de un consolidado de la planilla y NumConTransRetorno
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public int ObtenerCantidadEnviosEnConsolidadoPorPlanillaNumConTransRetorno(long idPlanilla, long conTransretorno, long idAsignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? totalEnvios = contexto.paObtenerTotalPiezasConsolidadoPlanillaVentas_OPU(idPlanilla, conTransretorno, idAsignacion).FirstOrDefault();
                return totalEnvios != null ? totalEnvios.Value : 0;
            }
        }

        /// <summary>
        /// Actualza el numero total  de envios en la planilla de ventas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public void ActualizarTotalEnviosPlanilladosPorPlanillaVentas(long idPlanilla, short numeroTotalEnvios)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarTotalEnviosPlanillaVentas_OPU(idPlanilla, numeroTotalEnvios);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene las planillas creadas por centro de servicios
        /// </summary>
        /// <param name="idCentroServicios">id del centro de servicios</param>
        /// <returns>Planillas por centro de servicios</returns>
        public List<OUPlanillaVentaDC> ObtenerPlanillasPorCentroServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicios)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string fechaInicial;
                string fechaFinal;
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();

                if (filtro != null && filtro.Any())
                {
                    filtro.TryGetValue("fechaInicial", out fechaInicial);
                    filtro.TryGetValue("fechaFinal", out fechaFinal);
                    if (fechaInicial != null && fechaFinal != null)
                    {
                        fechaFinal = Convert.ToDateTime(fechaFinal).AddHours(23).AddMinutes(59).AddSeconds(59).ToString();
                        LambdaExpression lambda = contexto.CrearExpresionLambda<PlanillaVentasMensajero_VOPU>("PLA_FechaGrabacion", fechaInicial.ToString(), OperadorComparacion.Between, fechaFinal);
                        where.Add(lambda, OperadorLogico.And);
                    }
                    if (filtro.ContainsKey("fechaInicial"))
                        filtro.Remove("fechaInicial");

                    if (filtro.ContainsKey("fechaFinal"))
                        filtro.Remove("fechaFinal");
                }
                else
                {
                    if (filtro.ContainsKey("fechaInicial"))
                        filtro.Remove("fechaInicial");

                    if (filtro.ContainsKey("fechaFinal"))
                        filtro.Remove("fechaFinal");

                    if (!filtro.ContainsKey("PLA_EstaCerrada"))
                        filtro.Add("PLA_EstaCerrada", "false");
                }

                filtro.Add("PLA_IdPuntoServicio", idCentroServicios.ToString());

                return contexto.ConsultarPlanillaVentasMensajero_VOPU(
                    filtro,
                    where,
                    campoOrdenamiento,
                    out totalRegistros,
                    indicePagina,
                    registrosPorPagina,
                    ordenamientoAscendente)
                  .OrderByDescending(orden => orden.PLA_FechaGrabacion)
                  .ToList().ConvertAll(r =>
                      {
                          OUPlanillaVentaDC planilla = new OUPlanillaVentaDC();

                          planilla.TipoMensajero = r.TIM_Descripcion;
                          planilla.NombreCompleto = r.NombreCompleto;
                          planilla.IdTipoMensajero = r.MEN_IdTipoMensajero;
                          planilla.IdMensajero = r.MEN_IdMensajero;
                          planilla.IdPuntoServicio = r.PLA_IdPuntoServicio;
                          planilla.NumeroPlanilla = r.PLA_IdPlanilla;
                          planilla.FechaPlanilla = r.PLA_FechaGrabacion.ToShortDateString();
                          planilla.IdVehiculo = r.PLA_IdVehiculo;
                          planilla.Placa = r.VEH_Placa;
                          planilla.EstadoPlanillaDescripcion = r.PLA_EstaCerrada ? OUConstantesOperacionUrbana.ESTADO_CERRADO_DESCRIPCION : OUConstantesOperacionUrbana.ESTADO_ABIERTA_DESCRIPCION;
                          planilla.EstaCerrada = r.PLA_EstaCerrada;

                          // TODO:ID Se adiciona los Datos de la Asignacion-Tula a la Cabecera de la Planilla
                          planilla.AsignacionTula = new OUAsignacionDC
                          {
                              IdAsignacion = r.ATP_IdAsignacionTula,
                              NoTula = r.ATP_NoTula,
                              NoPrecinto = r.ATP_NoPrecinto
                          };

                          return planilla;
                      });
            }
        }

        /// <summary>
        /// Obtiene las guias del punto de servicios que no estan planilladas
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<OUPlanillaVentaGuiasDC> ObtenerGuiasPorPuntoDeServicios(long idCentroServicios)
        {

            List<OUPlanillaVentaGuiasDC> arr = new List<OUPlanillaVentaGuiasDC>();
            OUPlanillaVentaGuiasDC objNew;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand comm = new SqlCommand("paObtenerAdmiMenCtrSvc_MEN", conn);
                comm.CommandType = CommandType.StoredProcedure;
                comm.Parameters.Add(new SqlParameter("@idCentroServicios", idCentroServicios));

                SqlDataReader reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    objNew = new OUPlanillaVentaGuiasDC();

                    objNew.NumeroGuia = Convert.ToInt64(reader["ADM_NumeroGuia"]);

                    objNew.IdCiudadOrigen = reader["ADM_IdCiudadOrigen"].ToString();
                    objNew.NombreCiudadOrigen = reader["ADM_NombreCiudadOrigen"].ToString();
                    objNew.IdCiudadDestino = reader["ADM_IdCiudadDestino"].ToString();
                    objNew.NombreCiudadDestino = reader["ADM_NombreCiudadDestino"].ToString();
                    objNew.TipoEnvio = reader["ADM_NombreTipoEnvio"].ToString();
                    objNew.TipoEnvioPlanilla = (OUEnumTipoEnvioPlanilla)Enum.Parse(typeof(OUEnumTipoEnvioPlanilla), reader["TipoEnvioPlanilla"].ToString());
                    objNew.EsEntregada = Convert.ToBoolean(reader["ADM_EstaEntregada"]);
                    objNew.CreadoPor = reader["ADM_CreadoPor"].ToString();
                    arr.Add(objNew);
                }
            }

            return arr;

            //CODIGO ANTERIOR
            //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerAdmiMenCtrSvc_MEN(idCentroServicios).ToList().ConvertAll(r => new OUPlanillaVentaGuiasDC()
            //    {
            //        NumeroGuia = r.ADM_NumeroGuia,
            //        IdAdmision = r.ADM_IdAdminisionMensajeria,
            //        Peso = r.ADM_Peso,
            //        EsRecomendado = r.ADM_EsRecomendado,
            //        IdUnidadNegocio = r.ADM_IdUnidadNegocio,
            //        NombreCiudadOrigen = r.ADM_NombreCiudadOrigen,
            //        IdCiudadOrigen = r.ADM_IdCiudadOrigen,
            //        NombreCiudadDestino = r.ADM_NombreCiudadDestino,
            //        IdCiudadDestino = r.ADM_IdCiudadDestino,
            //        IdServicio = r.ADM_IdServicio,
            //        NombreServicio = r.ADM_NombreServicio,
            //        DiceContener = r.ADM_DiceContener
            //    });
            //}
        }

        /// <summary>
        /// Elimina una guia de un consolidado en la planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public void EliminarGuiaConsolidadoPlanillaVentas(long idAdmisionMensajeria, long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarGuiaDeConsolidadoPlanillaVentas_OPU(idAdmisionMensajeria, idPlanilla);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una guia de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public void EliminarGuiaPlanillaVentas(long idAdmisionMensajeria, long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarGuiaDePlanillaVentas_OPU(idAdmisionMensajeria, idPlanilla);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Valida si existen rotulos de una guia asignados a una planilla
        /// </summary>
        /// <param name="idPlanillaVentas"></param>
        /// <param name="idAdmisionMensajeria"></param>
        /// <returns></returns>
        public bool ValidarExistenciaRotulosEnPlanillaVentas(long idPlanillaVentas, long idAdmisionMensajeria)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.paObtenerRotulosDePlanillaVentas_OPU(idAdmisionMensajeria, idPlanillaVentas).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Elimina una rotulo de consolidado de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public void EliminarRotuloDeConsolidadoPlanillaVentas(long idAdmisionMensajeria, long idPlanilla, short numeroPieza)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarRotuloDeConsolidadoPlanillaVentas_OPU(idAdmisionMensajeria, idPlanilla, numeroPieza);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene todas las asignaciones para imprimir por planilla de ventas
        /// </summary>
        /// <param name="idPlanillaVentas"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerAsignacionesPlanillaImprimir(long idPlanillaVentas, string estado)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasPlanVentaAsigTulaImpresion_OPU(idPlanillaVentas, estado).ToList().ConvertAll<OUAsignacionDC>(a =>
                new OUAsignacionDC()
                {
                    CentroServicioDestino = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC()
                    {
                        IdCentroServicio = a.ATP_IdCentroServicioDestino,
                        Nombre = a.LOC_NombreCompletoDestino,
                    },
                    CentroServicioOrigen = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC()
                    {
                        IdCentroServicio = a.ATP_IdCentroServicioOrigen,
                        Nombre = a.NombreCompletoOrigen,
                    },
                    NumContTransDespacho = a.ATP_NumContTransDespacho,
                    Estado = a.ATP_Estado,
                    IdAsignacion = a.ATP_IdAsignacionTula,
                    NoPrecinto = a.ATP_NoPrecinto,
                    NoTula = a.ATP_NoTula,
                    NumContTransRetorno = a.ATP_NumContTransRetorno,
                    TotalPiezas = a.PVR_TotalPiezas,
                    IdAdmisionMensajeria = a.PVG_IdAdminisionMensajeria,
                    TipoAsignacion = new OUTipoAsignacionDC()
                    {
                        DescripcionTipoAsignacion = a.TAS_Descripcion,
                        IdTipoAsignacion = a.ATP_IdTipoAsignacion
                    }
                });
            }
        }

        /// <summary>
        /// Selecciona las asignaciones utilizadas en una planilla
        /// </summary>
        /// <param name="idPlanillaVentas"></param>
        /// <returns></returns>
        public List<long> ObtenerIdAsignacionesUtilizadasPlanillaVentas(long idPlanillaVentas)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerAsignacionesUtilizadasPlanillaVentas_OPU(idPlanillaVentas).ToList().ConvertAll<long>(c => c.Value);
            }
        }

        /// <summary>
        /// Selecciona la guias sueltas asociadas a una planilla
        /// </summary>
        /// <param name="idPlanillaVentas"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerGuiasSueltasAsociadasPlanillaVentas(long idPlanillaVentas)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasSueltasPlanillaVentas_OPU(idPlanillaVentas).ToList().ConvertAll<OUAsignacionDC>(g =>
                    new OUAsignacionDC()
                    {
                        NumContTransRetorno = g.PVG_NumContTransRetorno,
                        NumeroGuiaRotulo = g.PVG_NumeroGuia
                    });
            }
        }

        /// <summary>
        /// Selecciona la guias asociadas a una asignacion dentro de una planilla
        /// </summary>
        /// <param name="idPlanillaVentas"></param>
        /// <returns></returns>
        public List<OUAsignacionDC> ObtenerGuiasAsociadasAsignacionPlanillaVentas(long idPlanillaVentas, long idAsignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasAsignacionPlanillaVentas_OPU(idPlanillaVentas, idAsignacion).ToList().ConvertAll<OUAsignacionDC>(g =>
                    new OUAsignacionDC()
                    {
                        NoPrecinto = g.ATP_NoPrecinto,
                        NoTula = g.ATP_NoTula,
                        NumContTransRetorno = g.PVG_NumContTransRetorno,
                        NumeroGuiaRotulo = g.PVG_NumeroGuia
                    });
            }
        }

        /// <summary>
        /// Elimina un rotulo de una planilla de ventas
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        /// <param name="idPlanilla"></param>
        public void EliminarRotuloDePlanillaVentas(long idAdmisionMensajeria, long idPlanilla, short numeroPieza)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarRotuloDePlanillaVentas_OPU(idAdmisionMensajeria, idPlanilla, numeroPieza);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene las guias asignadas a una planilla y a una asignacion tula
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="idAsignacionTula">Id de la asignacion tula</param>
        /// <returns>Lista con las guias asignadas</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorPlanillaAsignacionTula(long idPlanillaVentas, long idAsignacionTula)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiasPlanVentaAsigTula_OPU(idAsignacionTula, idPlanillaVentas).ToList().ConvertAll<OUGuiaIngresadaDC>(
                    p =>
                    {
                        OUGuiaIngresadaDC plan = new OUGuiaIngresadaDC();

                        plan.IdAdmision = p.PVG_IdAdminisionMensajeria;
                        plan.NumeroGuia = p.PVG_NumeroGuia;
                        plan.NumeroGuiaRotulo = p.PVG_NumeroGuia.ToString();
                        plan.TipoDespacho = OUConstantesOperacionUrbana.TIPO_DESPACHO_CONSOLIDADO;
                        if (p.PVR_TotalPiezas != -1)
                        {
                            plan.NumeroGuiaRotulo = plan.NumeroGuiaRotulo + "-" + p.PVR_NumeroPieza.ToString() + "/" + p.PVR_TotalPiezas.ToString();
                        }
                        return plan;
                    });
            }
        }

        /// <summary>
        /// Obtiene todas las guias sueltas planilladas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasSueltasPlanilladas(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviosSueltosPlanillaVentas_OPU(idPlanilla).ToList().ConvertAll<OUGuiaIngresadaDC>(g =>
                {
                    OUGuiaIngresadaDC plan = new OUGuiaIngresadaDC();

                    plan.IdAdmision = g.PVG_IdAdminisionMensajeria;
                    plan.NumeroGuia = g.PVG_NumeroGuia;
                    plan.NumeroGuiaRotulo = g.PVG_NumeroGuia.ToString();
                    plan.TipoDespacho = OUConstantesOperacionUrbana.TIPO_DESPACHO_SUELTO;
                    plan.Ciudad = g.LOC_Nombre;
                    plan.IdCiudad = g.PVG_IdCiudadOrigenGuia;
                    if (g.PVR_TotalPiezas != -1)
                    {
                        plan.NumeroGuiaRotulo = plan.NumeroGuiaRotulo + "-" + g.PVR_NumeroPieza.ToString() + "/" + g.PVR_TotalPiezas.ToString();
                    }
                    return plan;
                });
            }
        }

        /// <summary>
        /// Valida si una guia fue ingresada a una planilla de ventas
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="numeroGuia">Numero de la guia</param>
        /// <returns>Lista con las guias asignadas</returns>
        public bool ValidarGuiaIngresadaPlanillaAsignacionTula(long idPlanillaVentas, long numeroGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paValidaGuiaIngresadaPlanillaVentas_OPU(idPlanillaVentas, numeroGuia).ToList().Count() > 0 ? true : false;
            }
        }

        /// <summary>
        /// Valida si una guia fue ingresada a una planilla de ventas
        /// </summary>
        /// <param name="idPlanillaVentas"> id de la planilla de ventas</param>
        /// <param name="idAdmisionMensajeria">Identificador de la adminision mensajeria</param>
        /// <returns></returns>
        public bool ValidarRotuloIngresadoPlanilla(long idPlanillaVentas, long idAdmisionMensajeria, short numeroPieza)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paValidarRotuloIngresadoPlanillaVentas_OPU(idPlanillaVentas, idAdmisionMensajeria, numeroPieza).ToList().Count() > 0 ? true : false;
            }
        }

        #endregion Consultas

        #region Inserción

        public void ActualizarConTransRetornoPlanillaVentas(int? totalPiezas, long idPlanilla, long idAdmisionMensajeria, long numConstTransRetorno, long idAsignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //Actualiza el conTransRetorno en la tabla planillaventasGuia
                contexto.paActualizarConsTransRetornoPlanillaVentasGuia_OPU(idPlanilla, idAdmisionMensajeria, numConstTransRetorno);

                if (totalPiezas.HasValue && totalPiezas.Value > 0)
                {
                    //Actualiza el conTransRetorno en la tabla planillaventasRotuloGuia
                    contexto.paActualizarConsTransRetornoPlanillaVentasRotuloGuia_OPU(idPlanilla, idAdmisionMensajeria, numConstTransRetorno);
                }

                contexto.paActualizarConsTransRetornoPlanillaVentasAsignacionTula_OPU(idAsignacion, numConstTransRetorno);

                contexto.SaveChanges();
            }
        }

        public void ActualizarConTransRetornoPlanillaVentasSueltos(long idPlanilla, long idAdmisionMensajeria, long numConstTransRetorno)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //Actualiza el conTransRetorno en la tabla planillaventasGuia
                contexto.paActualizarConsTransRetornoPlanillaVentasGuia_OPU(idPlanilla, idAdmisionMensajeria, numConstTransRetorno);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Guarda una planilla de venta y retorna el numero de planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <returns></returns>
        public long GuardarPlanillaVenta(OUPlanillaVentaDC planilla)
        {
            long newID = 0;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paCrearPlanillaVenta_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramOut = new SqlParameter("@IdPlanilla", SqlDbType.BigInt);
                paramOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramOut);

                cmd.Parameters.Add(new SqlParameter("@idPuntoServicio", planilla.IdPuntoServicio));
                cmd.Parameters.Add(new SqlParameter("@nombrePuntoServicio", planilla.NombreCentroServicios));
                cmd.Parameters.Add(new SqlParameter("@direccionPuntoServicio", planilla.DireccionPuntoServicio));
                cmd.Parameters.Add(new SqlParameter("@idMensajero", planilla.IdMensajero));
                cmd.Parameters.Add(new SqlParameter("@totalEnvios", planilla.TotalEnvios));
                cmd.Parameters.Add(new SqlParameter("@PLA_EstaCerrada", planilla.EstaCerrada));
                cmd.Parameters.Add(new SqlParameter("@PLA_FechaCierrePlanilla", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@fechaGrabacion", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@creadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@PLA_NombreMensajero", planilla.NombreCompleto));
                cmd.Parameters.Add(new SqlParameter("@PLA_IdVehiculo", planilla.IdVehiculo));

                cmd.ExecuteNonQuery();

                newID = Convert.ToInt64(paramOut.Value);
            }
            return newID;


            //CODIGO ANTERIOR
            //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    decimal? idPlanilla = contexto.paCrearPlanillaVenta_OPU(planilla.IdPuntoServicio
            //      , planilla.NombreCentroServicios
            //      , planilla.DireccionPuntoServicio
            //      , planilla.IdMensajero
            //      , planilla.TotalEnvios
            //      , planilla.EstaCerrada
            //      , planilla.FechaCierrePlanilla
            //      , DateTime.Now
            //      , ControllerContext.Current.Usuario, planilla.NombreCompleto, planilla.IdVehiculo
            //      , planilla.AsignacionTula.IdAsignacion).FirstOrDefault();
            //    contexto.SaveChanges();
            //    return (Convert.ToInt64(idPlanilla.Value));
            //}
        }

        /// <summary>
        /// Guarda una guia de una planilla
        /// </summary>
        /// <param name="guiaPlanilla"></param>
        public void GuardarPlanillaVentaGuia(OUPlanillaVentaGuiasDC guiaPlanilla, long? idAsignacionTula)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paCrearPlanillaVentaGuia_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@idPlanilla", guiaPlanilla.IdPlanilla));
                cmd.Parameters.Add(new SqlParameter("@idAdmisionMensajeria", guiaPlanilla.IdAdmision));
                cmd.Parameters.Add(new SqlParameter("@idPuntoServicio", guiaPlanilla.IdPuntoServicio));
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", guiaPlanilla.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@idUnidadNegocio", guiaPlanilla.IdUnidadNegocio));
                cmd.Parameters.Add(new SqlParameter("@idServicio", guiaPlanilla.IdServicio));
                cmd.Parameters.Add(new SqlParameter("@nombreServicio", guiaPlanilla.NombreServicio));
                cmd.Parameters.Add(new SqlParameter("@diceContener", guiaPlanilla.DiceContener));
                cmd.Parameters.Add(new SqlParameter("@peso", guiaPlanilla.Peso));
                cmd.Parameters.Add(new SqlParameter("@esRecomendado", guiaPlanilla.EsRecomendado));
                cmd.Parameters.Add(new SqlParameter("@PVG_IdAsignacionTula", idAsignacionTula));
                cmd.Parameters.Add(new SqlParameter("@PVG_NumContTransRetorno", guiaPlanilla.NumContTransRetorno));
                cmd.Parameters.Add(new SqlParameter("@fechaGrabacion", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@creadoPor", ControllerContext.Current.Usuario));
                cmd.Parameters.Add(new SqlParameter("@PVG_IdCiudadOrigenGuia", guiaPlanilla.IdCiudadOrigenGuia));
                cmd.Parameters.Add(new SqlParameter("@PVG_TipoEnvioPlanilla", (int)guiaPlanilla.TipoEnvioPlanilla));

                cmd.ExecuteNonQuery();
            }


            //CODIGO ANTERIOR
            //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearPlanillaVentaGuia_OPU(guiaPlanilla.IdPlanilla
            //                     , guiaPlanilla.IdAdmision
            //                     , guiaPlanilla.IdPuntoServicio
            //                     , guiaPlanilla.NumeroGuia
            //                     , guiaPlanilla.IdUnidadNegocio
            //                     , guiaPlanilla.IdServicio
            //                     , guiaPlanilla.NombreServicio
            //                     , guiaPlanilla.DiceContener
            //                     , guiaPlanilla.Peso
            //                     , guiaPlanilla.EsRecomendado
            //                     , idAsignacionTula
            //                     , guiaPlanilla.NumContTransRetorno
            //                     , DateTime.Now
            //                     , ControllerContext.Current.Usuario,
            //                     guiaPlanilla.IdCiudadOrigenGuia);
            //    contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Guarda los rotulos asociados a una planilla de ventas
        /// </summary>
        /// <param name="guiaPlanilla"></param>
        public void GuardarRotuloGuiaPlanillaVentas(OUPlanillaVentaGuiasDC guiaPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long? idAsignacionTula = null;
                if (guiaPlanilla.IdAsignacionTula != -1)
                {
                    idAsignacionTula = guiaPlanilla.IdAsignacionTula;
                }

                contexto.paGuardarRotuloGuiaPlanillaVentas_OPU(guiaPlanilla.IdPlanilla, guiaPlanilla.IdAdmision, (short)guiaPlanilla.PiezaActualRotulo, (short)guiaPlanilla.TotalPiezasRotulo, idAsignacionTula, guiaPlanilla.NumContTransRetorno, DateTime.Now, ControllerContext.Current.Usuario);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Cambia el estado de una asignacion de una tula a un punto
        /// </summary>
        /// <param name="guiaPlanilla"></param>
        public void CambiarEstadoAsignacionTulaPunto(long idAsignacion, string estado)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCambiarEstadoAsigTulaPunto_OPU(idAsignacion, estado);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona una guia a una planilla, si la planilla no existe crea una nueva y retorna el numero de la planilla
        /// </summary>
        /// <param name="planilla"></param>
        /// <param name="guiaPlanilla"></param>
        /// <returns></returns>
        public long GuardarPlanillaVenta(OUPlanillaVentaDC planilla, List<OUPlanillaVentaGuiasDC> guiasPlanilla)
        {
            long id;
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    decimal? idPlanilla = contexto.paCrearPlanillaVenta_OPU(
                        planilla.IdPuntoServicio
                      , planilla.NombreCentroServicios
                      , planilla.DireccionPuntoServicio
                      , planilla.IdMensajero
                      , planilla.TotalEnvios
                      , planilla.EstaCerrada
                      , planilla.FechaCierrePlanilla
                      , DateTime.Now
                      , ControllerContext.Current.Usuario, planilla.NombreCompleto, planilla.IdVehiculo, null).FirstOrDefault();

                    if (long.TryParse(idPlanilla.ToString(), out id))
                    {
                        foreach (OUPlanillaVentaGuiasDC guiaPlanilla in guiasPlanilla)
                        {
                            if (guiaPlanilla.EsSeleccionado)
                            {
                                contexto.paCrearPlanillaVentaGuia_OPU(id
                                  , guiaPlanilla.IdAdmision
                                  , planilla.IdPuntoServicio
                                  , guiaPlanilla.NumeroGuia
                                  , guiaPlanilla.IdUnidadNegocio
                                  , guiaPlanilla.IdServicio
                                  , guiaPlanilla.NombreServicio
                                  , guiaPlanilla.DiceContener
                                  , guiaPlanilla.Peso
                                  , guiaPlanilla.EsRecomendado
                                  , guiaPlanilla.IdAsignacionTula
                                  , guiaPlanilla.NumContTransRetorno
                                  , DateTime.Now
                                  , ControllerContext.Current.Usuario, planilla.NombreCompleto);
                            }
                        }
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, ETipoErrorFramework.EX_ERROR_PLANILLA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_PLANILLA)));
                    }
                    scope.Complete();
                    return id;
                }
            }
        }

        #endregion Inserción

        #endregion Planilla Ventas

        #region Planilla Asignacion

        #region Consultas

        public string ObtenerParametroOperacionUrbana(string llave)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosOperacionUrbana_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@POU_IdParametro", llave);
                conn.Open();
                var rta = cmd.ExecuteScalar();
                conn.Close();
                if (rta == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_PARAMETRO_NO_EXISTE.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_PARAMETRO_NO_EXISTE), llave)));

                return rta.ToString();


            }

        }

        /// <summary>
        /// Retorna el numero de veces que se a asignado una guia para reparto
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns>Retorna el numero de veces que se a asignado una guia para reparto</returns>

        public int ObtenerNumeroDeAsignacionesParaUnaGuia(long NumeroGuia)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCantidadAsignacionesGuiaEnPlanillaMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PAG_NumeroGuia", NumeroGuia);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }

            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var cantidadAsignaciones = contexto.PlanillaAsignacionGuia_OPU.Where(p => p.PAG_NumeroGuia == NumeroGuia).Count();
                return cantidadAsignaciones;
            }*/
        }

        public List<OUPlanillaAsignacionDC> ObtenerPlanillasAsignacionCentroLogistico(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCol, bool incluyeFecha)
        {


            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                DateTime fechaAsignacion;
                DateTime fechaAsignacionFinal;
                string idZona;
                string idTipoMensajero;
                string idPlanilla;
                string idMensajero;
                string idEstadoPlanilla;
                string fecha;
                string idCargo;

                filtro.TryGetValue("PAE_IdZona", out idZona);
                filtro.TryGetValue("PAE_IdTipoMensajero", out idTipoMensajero);
                filtro.TryGetValue("PAE_IdEstadoPlanillaEnvios", out idEstadoPlanilla);
                filtro.TryGetValue("PAM_IdMensajero", out idMensajero);
                filtro.TryGetValue("PAE_IdPlanillaAsignacionEnvio", out idPlanilla);
                filtro.TryGetValue("PAE_FechaGrabacion", out fecha);
                filtro.TryGetValue("PEI_IdCargo", out idCargo);
                fechaAsignacion = Convert.ToDateTime(fecha, Thread.CurrentThread.CurrentCulture);
                fechaAsignacionFinal = fechaAsignacion.AddDays(1);

                if (indicePagina == 0)
                    indicePagina = 1;

                if (Convert.ToInt64(idMensajero) > 0)
                {

                    SqlCommand cmd = new SqlCommand("paPlanillaAsignacionEnvios_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCentroLogistico", idCol);


                    if (!string.IsNullOrEmpty(idPlanilla))
                        cmd.Parameters.AddWithValue("@idPlanilla", Convert.ToInt64(idPlanilla));
                    else
                        cmd.Parameters.AddWithValue("@idPlanilla", DBNull.Value);
                                        

                    if (string.IsNullOrEmpty(idZona))
                        cmd.Parameters.AddWithValue("@idZona", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@idZona", idZona);


                    if (!string.IsNullOrEmpty(idTipoMensajero))
                        cmd.Parameters.AddWithValue("@idTipoMensajero", Convert.ToInt16(idTipoMensajero));
                    else
                        cmd.Parameters.AddWithValue("@idTipoMensajero", DBNull.Value);
                                        

                    if (string.IsNullOrEmpty(idEstadoPlanilla))
                        cmd.Parameters.AddWithValue("@idEstadoPlanilla", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@idEstadoPlanilla", idEstadoPlanilla);

                    if (string.IsNullOrEmpty(idMensajero))
                        cmd.Parameters.AddWithValue("@idMensajero", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@idMensajero", idMensajero);
                    //Adición Id Cargo Mensajero
                    if (string.IsNullOrEmpty(idCargo))
                        cmd.Parameters.AddWithValue("@cargoMensajero", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@cargoMensajero", idCargo);

                    cmd.Parameters.AddWithValue("@fechaAsignacionInicial", fechaAsignacion.Year == 1 ? DateTime.Today : fechaAsignacion);
                    cmd.Parameters.AddWithValue("@fechaAsignacionFinal", fechaAsignacionFinal.Year == 1 ? DateTime.Today : fechaAsignacionFinal);
                    cmd.Parameters.AddWithValue("@PageIndex", indicePagina);
                    cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);
                    cmd.Parameters.AddWithValue("@IncluyeFiltroFecha", incluyeFecha);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    conn.Close();

                    return dt.AsEnumerable().ToList().ConvertAll(r => new OUPlanillaAsignacionDC()
                    {
                        EstadoPlanilla = r.Field<string>("EPE_Descripcion"),
                        IdEstadoPlanilla = r.Field<string>("PAE_IdEstadoPlanillaEnvios"),
                        IdMunicipioAsignacion = r.Field<string>("PAE_IdMunicipioAsignacion"),
                        IdPlanillaAsignacion = r.Field<long>("PAE_IdPlanillaAsignacionEnvio"),
                        FechaAsignacion = r.Field<DateTime>("PAE_FechaGrabacion"),
                        RequiereAuditoria = r.Field<bool>("PAE_RequiereAuditoria"),
                        Zona = new PAZonaDC()
                        {
                            IdZona = r.Field<string>("PAE_IdZona"),
                            Descripcion = r.Field<string>("PAE_DescripcionZona")
                        },
                        TotalGuias = r.Field<int>("PAE_TotalGuiasAsignadas"),
                        Mensajero = new OUMensajeroDC()
                        {
                            IdMensajero = r["PAM_IdMensajero"] == DBNull.Value ? 0 : r.Field<long>("PAM_IdMensajero"),
                            NombreCompleto = r["NombreCompleto"] == DBNull.Value ? "Sin asignar" : r.Field<string>("NombreCompleto"),
                            TipMensajeros = new OUTipoMensajeroDC()
                            {
                                IdTipoMensajero = r.Field<Int16>("PAE_IdTipoMensajero"),
                                Descripcion = r.Field<string>("PAE_DescripcionTipoMensajero")
                            }
                        },
                        //Ciudad = r.Field<string>("NombreCiudadDestino"),
                        CreadoPor = r.Field<string>("PAE_CreadoPor"),
                        FechaCreacion = r.Field<DateTime>("PAE_FechaGrabacion")


                    });
                }
                else
                {

                    SqlCommand cmd = new SqlCommand("paObtenerPlaAsiEnSinFilMen_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idCentroLogistico", idCol);

                    if (!string.IsNullOrEmpty(idPlanilla))
                        cmd.Parameters.AddWithValue("@idPlanilla", Convert.ToInt64(idPlanilla));
                    else
                        cmd.Parameters.AddWithValue("@idPlanilla", DBNull.Value);
                    
                    if (!string.IsNullOrEmpty(idZona))
                        cmd.Parameters.AddWithValue("@idZona", idZona);
                    else
                        cmd.Parameters.AddWithValue("@idZona", DBNull.Value);

                    if (!string.IsNullOrEmpty(idTipoMensajero))                    
                        cmd.Parameters.AddWithValue("@idTipoMensajero", Convert.ToInt16(idTipoMensajero));                    
                    else
                        cmd.Parameters.AddWithValue("@idTipoMensajero", DBNull.Value);

                    if (string.IsNullOrEmpty(idEstadoPlanilla))
                        cmd.Parameters.AddWithValue("@idEstadoPlanilla", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@idEstadoPlanilla", idEstadoPlanilla);

                    if (string.IsNullOrEmpty(idCargo))
                        cmd.Parameters.AddWithValue("@cargoMensajero", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@cargoMensajero", idCargo);

                    cmd.Parameters.AddWithValue("@fechaAsignacionInicial", fechaAsignacion.Year == 1 ? DateTime.Today : fechaAsignacion);
                    cmd.Parameters.AddWithValue("@fechaAsignacionFinal", fechaAsignacionFinal.Year == 1 ? DateTime.Today : fechaAsignacionFinal);
                    cmd.Parameters.AddWithValue("@PageIndex", indicePagina);
                    cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);
                    cmd.Parameters.AddWithValue("@IncluyeFiltroFecha", incluyeFecha);
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    conn.Close();


                    return dt.AsEnumerable().ToList()
                       .ConvertAll(r => new OUPlanillaAsignacionDC()
                       {
                           EstadoPlanilla = r.Field<string>("EPE_Descripcion"),
                           IdEstadoPlanilla = r.Field<string>("PAE_IdEstadoPlanillaEnvios"),
                           IdMunicipioAsignacion = r.Field<string>("PAE_IdMunicipioAsignacion"),
                           IdPlanillaAsignacion = r.Field<long>("PAE_IdPlanillaAsignacionEnvio"),
                           FechaAsignacion = r.Field<DateTime>("PAE_FechaGrabacion"),
                           RequiereAuditoria = r.Field<bool>("PAE_RequiereAuditoria"),
                           Zona = new PAZonaDC()
                           {
                               IdZona = r.Field<string>("PAE_IdZona"),
                               Descripcion = r.Field<string>("PAE_DescripcionZona")
                           },
                           TotalGuias = r.Field<int>("PAE_TotalGuiasAsignadas"),
                           Mensajero = new OUMensajeroDC()
                           {
                               IdMensajero = r["PAM_IdMensajero"] == DBNull.Value ? 0 : r.Field<long>("PAM_IdMensajero"),
                               NombreCompleto = r["NombreCompleto"] == DBNull.Value ? "Sin asignar" : r.Field<string>("NombreCompleto"),
                               TipMensajeros = new OUTipoMensajeroDC()
                               {
                                   IdTipoMensajero = r.Field<Int16>("PAE_IdTipoMensajero"),
                                   Descripcion = r.Field<string>("PAE_DescripcionTipoMensajero")
                               }
                           },
                           //Ciudad = r.Field<string>("NombreCiudadDestino"),
                           CreadoPor = r.Field<string>("PAE_CreadoPor"),
                           FechaCreacion = r.Field<DateTime>("PAE_FechaGrabacion")
                       });
                }
            }


        }

        /// <summary>
        /// Retorna los mensajeros filtrados por centro logistico y por tipo de mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="centroLogistico"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        public List<OUMensajeroDC> ObtenerMensejorPorColYTipoMensajero(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long centroLogistico, int idTipoMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_IdAgencia", centroLogistico.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                if (idTipoMensajero > 0)
                {
                    lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_IdTipoMensajero", idTipoMensajero.ToString(), OperadorComparacion.Equal);
                    where.Add(lambda, OperadorLogico.And);
                }

                lambda = contexto.CrearExpresionLambda<MensajeroCentroLogistico_VOPU>("MEN_Estado", OUConstantesOperacionUrbana.ESTADO_ACTIVO, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarMensajeroCentroLogistico_VOPU(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).
                    ToList().ConvertAll(r => new OUMensajeroDC()
                    {
                        NombreCompleto = r.NombreCompleto,
                        FechaActual = DateTime.Now,
                        IdMensajero = r.MEN_IdMensajero,
                        IdTipoMensajero = r.MEN_IdTipoMensajero,
                        EsVehicular = r.TIM_EsVehicular,
                        FechaVencimientoPase = r.MEN_FechaVencimientoPase,
                        PersonaInterna = new OUPersonaInternaDC()
                        {
                            Identificacion = r.PEI_Identificacion,
                            Municipio = r.LOC_Nombre,
                            IdMunicipio = r.PEI_Municipio
                        },
                        Agencia = r.MEN_IdAgencia,
                        Estado = new OUEstadosMensajeroDC()
                        {
                            IdEstado = r.MEN_Estado,
                            Descripcion = EstadosMensajeros(r.MEN_Estado)
                        },
                        TipMensajeros = new OUTipoMensajeroDC()
                        {
                            IdTipoMensajero = r.MEN_IdTipoMensajero
                        }
                    });
            }
        }

        /// <summary>
        /// Obtiene los envios por planilla de asignacion
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerEnviosPlanillaAsignacion(long idPlanilla)
        {
            List<OUGuiaIngresadaDC> guiasPlanilla = new List<OUGuiaIngresadaDC>();
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerEnvioPlanillaAsig_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idPlanilla", idPlanilla);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        guiasPlanilla.Add(new OUGuiaIngresadaDC()
                        {
                            NumeroGuia = (long)reader["PAG_NumeroGuia"],
                            IdAdmision = (long)reader["PAG_IdAdminisionMensajeria"],
                            Peso = (decimal)reader["PAG_Peso"],
                            Consecutivo = (short)reader["PAG_Consecutivo"],
                            EstaVerificada = (bool)reader["PAG_EstaVerificada"],
                            CreadoPor = reader["PAG_CreadoPor"].ToString(),
                            DireccionDestinatario = reader["PAG_DireccionDestinatario"].ToString(),
                            TelefonoDestinatario = reader["PAG_TelefonoDestinatario"].ToString(),

                            DetalleGuia = new OUDetalleGuiaDC()
                            {
                                IdServicio = (int)reader["PAG_IdServicio"],
                                NombreServicio = reader["PAG_NombreServicio"].ToString(),
                                IdTipoEnvio = (short)reader["PAG_IdTipoEnvio"],
                                TipoEnvio = reader["PAG_NombreTipoEnvio"].ToString(),
                                CiudadDestino = reader["PAG_NombreCiudadDestino"].ToString()
                            },
                            TipoImpreso = ADEnumTipoImpreso.Planilla,
                            EsReclameEnOficina = (Convert.ToInt16(reader["ADM_IdTipoEntrega"]) == 2 ? true : false),
                            Ciudad = reader["PAG_NombreCiudadDestino"].ToString(),
                            NombreTipoEnvio = reader["ADM_NombreTipoEnvio"].ToString(),
                            DiceContener = reader["ADM_DiceContener"].ToString(),
                            FechaAsignacion = Convert.ToDateTime(reader["PAG_FechaGrabacion"].ToString())
                        });
                    }
                    cmd.Dispose();
                    conn.Close();
                    return guiasPlanilla;
                }
            }
        }

        /// <summary>
        /// Obtiene las guias de determinado mensajero en determinado dia
        /// </summary>
        /// <param name="idmensajero"></param>
        /// <param name="fechaPlanilla"></param>
        /// <returns>las guias de determinado mensajero con determinada fecha</returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPlanilladasPorDiaYMensajero(long idmensajero, DateTime fechaPlanilla)
        {

            List<OUGuiaIngresadaDC> lista = new List<OUGuiaIngresadaDC>();

            using (SqlConnection cnx = new SqlConnection(conexionStringController))
            {
                cnx.Open();

                SqlCommand cmd = new SqlCommand("paObtenerGuiasPlanilladasPorDia_OPU", cnx);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@idMensajero", idmensajero));

                cmd.Parameters.Add(new SqlParameter("@fechaPlanilla", fechaPlanilla));

                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {

                    lista.Add(new OUGuiaIngresadaDC
                    {
                        NumeroGuia = Convert.ToInt64(lector["PAG_NUMEROGUIA"]),
                        CreadoPor = lector["PAG_CREADOPOR"].ToString(),
                        Planilla = Convert.ToInt64(lector["PAG_IDPLANILLAASIGNACIONENVIO"]),
                        Peso = Convert.ToDecimal(lector["PAG_Peso"]),
                        DireccionDestinatario = lector["PAG_DireccionDestinatario"].ToString(),
                        NombreTipoEnvio = lector["PAG_NOMBRETIPOENVIO"].ToString(),
                        EstaVerificada = Convert.ToBoolean(lector["PAG_EstaVerificada"]),
                        DiceContener = lector["ADM_DiceContener"].ToString(),
                        FechaAsignacion = Convert.ToDateTime(lector["PAG_FechaGrabacion"].ToString())
                    });

                }
                cnx.Close();
                cnx.Dispose();
            }

            return lista;
        }


        /// <summary>
        /// Obtiene los envios admitidos no planillados de un punto de servicio
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public List<OUPlanillaVentaGuiasDC> ObtenerEnviosEstadoAdmitidoNoPlanilladosPuntoServicio(long idPuntoServicio)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviosEstadoAdmitidoNoPlanilladosPuntoServicio_OPU(idPuntoServicio).ToList().ConvertAll(r => new OUPlanillaVentaGuiasDC()
                {
                    NumeroGuia = r.ADM_NumeroGuia,
                    IdAdmision = r.ADM_IdAdminisionMensajeria,
                    Peso = r.ADM_Peso,
                    IdPuntoServicio = r.ADM_IdCentroServicioOrigen,
                    IdUnidadNegocio = r.ADM_IdUnidadNegocio,
                    IdServicio = r.ADM_IdServicio,
                    NombreServicio = r.ADM_NombreServicio,
                    DiceContener = r.ADM_DiceContener,
                    EsRecomendado = r.ADM_EsRecomendado,
                    TotalPiezasRotulo = r.ADM_TotalPiezas,
                    PiezaActualRotulo = r.ADM_NumeroPieza,
                    IdCiudadOrigenGuia = r.ADM_IdCiudadOrigen,
                    IdCiudadOrigen = r.ADM_IdCiudadOrigen,
                    NombreCiudadOrigen = r.ADM_NombreCiudadOrigen
                });
            }
        }

        /// <summary>
        /// consulta la planilla para el ingreso
        /// </summary>
        /// <param name="guiaIngresada"></param>
        public OUGuiaIngresadaDC ConsultaGuiaParaPlanilla(OUGuiaIngresadaDC guiaIngresada)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAdmisionMensajeri_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numGuia", guiaIngresada.NumeroGuia.Value);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var guiaRegistrada = dt.AsEnumerable().ToList().FirstOrDefault();
                if (guiaRegistrada == null)
                {
                    guiaIngresada.IdAdmision = 0;
                    guiaIngresada.GuiaRegistrada = false;
                    guiaIngresada.DetalleGuia = null;
                }
                else
                {
                    guiaIngresada.IdAdmision = guiaRegistrada.Field<long>("ADM_IdAdminisionMensajeria");
                    guiaIngresada.EsAlCobro = guiaRegistrada.Field<bool>("ADM_EsAlCobro");
                    guiaIngresada.GuiaRegistrada = true;
                    guiaIngresada.GuiaAutomatica = guiaRegistrada.Field<bool>("ADM_EsAutomatico");
                    guiaIngresada.TipoCliente = guiaRegistrada.Field<string>("ADM_TipoCliente");
                    guiaIngresada.PesoSistema = guiaRegistrada.Field<decimal>("ADM_Peso");
                    guiaIngresada.Peso = guiaRegistrada.Field<decimal?>("ADM_Peso");                    
                    guiaIngresada.TelefonoDestinatario = guiaRegistrada.Field<string>("ADM_TelefonoDestinatario");
                    guiaIngresada.DireccionDestinatario = guiaRegistrada.Field<string>("ADM_DireccionDestinatario");
                    guiaIngresada.IdAdmision = guiaRegistrada.Field<long>("ADM_IdAdminisionMensajeria");
                    guiaIngresada.DetalleGuia = new OUDetalleGuiaDC()
                    {
                        CiudadDestino = guiaRegistrada.Field<string>("ADM_NombreCiudadDestino"),
                        IdCiudadDestino = guiaRegistrada.Field<string>("ADM_IdCiudadDestino"),
                        CiudadOrigen = guiaRegistrada.Field<string>("ADM_NombreCiudadOrigen"),
                        IdCiudadOrigen = guiaRegistrada.Field<string>("ADM_IdCiudadOrigen"),
                        IdTipoEnvio = guiaRegistrada.Field<short>("ADM_IdTipoEnvio"),
                        IdServicio = guiaRegistrada.Field<int>("ADM_IdServicio"),
                        NombreServicio = guiaRegistrada.Field<string>("ADM_NombreServicio"),
                        TipoEnvio = guiaRegistrada.Field<string>("ADM_NombreTipoEnvio")
                    };
                    guiaIngresada.ValorTotal = guiaRegistrada.Field<decimal>("ADM_ValorTotal");
                    guiaIngresada.TotalPiezas = guiaRegistrada.Field<short?>("ADM_TotalPiezas");
                    guiaIngresada.RecogeDinero = guiaRegistrada.Field<bool>("ADM_EsAlCobro");
                    guiaIngresada.IdCentroServicioDestino = guiaRegistrada.Field<long>("ADM_IdCentroServicioDestino");
                    guiaIngresada.NumeroPedido = guiaRegistrada.Field<string>("ADM_NoPedido");
                    guiaIngresada.IdCliente = guiaRegistrada.Field<int>("CLI_IdCliente");
                }
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }


            /*using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guiaRegistrada = contexto.paObtenerAdmisionMensajeri_MEN(guiaIngresada.NumeroGuia).FirstOrDefault();
                if (guiaRegistrada == null)
                {
                    guiaIngresada.IdAdmision = 0;
                    guiaIngresada.GuiaRegistrada = false;
                    guiaIngresada.DetalleGuia = null;
                }
                else
                {
                    guiaIngresada.EsAlCobro = guiaRegistrada.ADM_EsAlCobro;
                    guiaIngresada.GuiaRegistrada = true;
                    guiaIngresada.GuiaAutomatica = guiaRegistrada.ADM_EsAutomatico;
                    guiaIngresada.TipoCliente = guiaRegistrada.ADM_TipoCliente;
                    guiaIngresada.PesoSistema = guiaRegistrada.ADM_Peso;
                    guiaIngresada.Peso = guiaRegistrada.ADM_Peso;
                    guiaIngresada.NumeroGuia = guiaIngresada.NumeroGuia;
                    guiaIngresada.TelefonoDestinatario = guiaRegistrada.ADM_TelefonoDestinatario;
                    guiaIngresada.DireccionDestinatario = guiaRegistrada.ADM_DireccionDestinatario;
                    guiaIngresada.IdAdmision = guiaRegistrada.ADM_IdAdminisionMensajeria;
                    guiaIngresada.DetalleGuia = new OUDetalleGuiaDC()
                    {
                        CiudadDestino = guiaRegistrada.ADM_NombreCiudadDestino,
                        IdCiudadDestino = guiaRegistrada.ADM_IdCiudadDestino,
                        CiudadOrigen = guiaRegistrada.ADM_NombreCiudadOrigen,
                        IdCiudadOrigen = guiaRegistrada.ADM_IdCiudadOrigen,
                        IdTipoEnvio = guiaRegistrada.ADM_IdTipoEnvio,
                        IdServicio = guiaRegistrada.ADM_IdServicio,
                        NombreServicio = guiaRegistrada.ADM_NombreServicio,
                        TipoEnvio = guiaRegistrada.ADM_NombreTipoEnvio
                    };
                    guiaIngresada.ValorTotal = guiaRegistrada.ADM_ValorTotal;
                    guiaIngresada.TotalPiezas = guiaRegistrada.ADM_TotalPiezas;
                    guiaIngresada.RecogeDinero = guiaRegistrada.ADM_EsAlCobro;
                    guiaIngresada.IdCentroServicioDestino = guiaRegistrada.ADM_IdCentroServicioDestino;
                }
                guiaIngresada.TipoImpreso = ADEnumTipoImpreso.Planilla;
                return guiaIngresada;
            }*/
        }

        /// <summary>
        /// Obtiene la cantidad de folios del envio, para los envios de rapiRadicado
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns>Total de Folio</returns>
        public short ObtenerInfoEnvioRapiRadicado(long idAdmision)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAdmiRapiRadicado_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", idAdmision);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().First().Field<short>("ARR_NumeroFolios");

            }
            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var envio = contexto.paObtenerAdmiRapiRadicado_MEN(idAdmision).FirstOrDefault();
                return envio.ARR_NumeroFolios;
            }*/
        }

        /// <summary>
        /// Valida si la guia ya esta planillada y ha sido descargada
        /// </summary>
        /// <param name="idAdmision"></param>
        public void ObtenerEnviosPlanillaAsignacionGuia(long idAdmision, long idPlanillaActual)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEnvioPlaniAsiGuia_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmision", idAdmision);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var envio = dt.AsEnumerable().FirstOrDefault();

                if (envio != null)
                {
                    if (envio.Field<long>("PAG_IdPlanillaAsignacionEnvio") == idPlanillaActual)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, "0", "La guía que intenta asignar a la planilla ya existe en la misma planilla"));
                    }
                }
                if (envio != null && !envio.Field<bool>("PAG_EstaDescargada") && envio.Field<string>("PAG_EstadoEnPlanilla") == OUConstantesOperacionUrbana.ESTADO_PLANILLADA)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_ESTA_PLANILLADA_ASIGNACION_MESAJERO.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_ESTA_PLANILLADA_ASIGNACION_MESAJERO), envio.Field<long>("PAG_IdPlanillaAsignacionEnvio"))));

            }


           /* using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var envio = contexto.paObtenerEnvioPlaniAsiGuia_OPU(idAdmision).FirstOrDefault();
                if (envio != null)
                {
                    if (envio.PAG_IdPlanillaAsignacionEnvio == idPlanillaActual)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, "0", "La guía que intenta asignar a la planilla ya existe en la misma planilla"));
                    }
                }
                if (envio != null && !envio.PAG_EstaDescargada && envio.PAG_EstadoEnPlanilla == OUConstantesOperacionUrbana.ESTADO_PLANILLADA)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_GUIA_ESTA_PLANILLADA_ASIGNACION_MESAJERO.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_GUIA_ESTA_PLANILLADA_ASIGNACION_MESAJERO), envio.PAG_IdPlanillaAsignacionEnvio)));
            }*/
        }

        /// <summary>
        /// Obtiene la planilla donde esta asignado un envio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ObtenerPlanillaGuia(long idAdmision)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var envio = contexto.paObtenerEnvioPlaniAsiGuia_OPU(idAdmision).FirstOrDefault();
                if (envio != null)
                {
                    return new OUGuiaIngresadaDC()
                    {
                        IdMensajero = envio.PAM_IdMensajero.Value,
                        Planilla = envio.PAG_IdPlanillaAsignacionEnvio,
                        IdAdmision = envio.PAG_IdAdminisionMensajeria
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene las planillas donde esta asignado un envio
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerPlanillasGuia(long idAdmision)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvioPlaniAsiGuia_OPU(idAdmision).ToList().ConvertAll<OUGuiaIngresadaDC>(g =>
                    new OUGuiaIngresadaDC()
                    {
                        IdMensajero = g.PAM_IdMensajero.HasValue ? g.PAM_IdMensajero.Value : 0,
                        NombreCompleto = g.NombreMensajero ?? string.Empty,
                        Planilla = g.PAG_IdPlanillaAsignacionEnvio,
                        IdAdmision = g.PAG_IdAdminisionMensajeria,
                        EstaPagada = g.PAG_ReportadoACaja == 0 ? false : true,
                        IdCentroLogistico = g.PAE_IdAgencia.HasValue ? g.PAE_IdAgencia.Value : 0,
                        NombreCentroLogistico = g.NombreAgencia,
                        EstadoGuiaPlanilla = g.PAG_EstadoEnPlanilla,
                        FechaPlanilla = g.PAG_FechaGrabacion
                    });
            }
        }

        /// <summary>
        /// valida si el envio esta registrado en las planilla de asignacion
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public OUGuiaIngresadaDC ConsultaEnvioPlanillaAsignacionGuia(long idGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var envio = contexto.paObtenerGuiaPlanillaAsig_OPU(idGuia).LastOrDefault();
                if (envio != null)
                    return new OUGuiaIngresadaDC()
                    {
                        IdAdmision = envio.PAG_IdAdminisionMensajeria,
                        Planilla = envio.PAG_IdPlanillaAsignacionEnvio,
                        CreadoPor = envio.PAG_CreadoPor,
                        EstaVerificada = envio.PAG_EstaVerificada,
                        TipoImpreso = ADEnumTipoImpreso.Planilla,
                    };
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_EL_ENVIO_NO_ESTA_ASIGNADO)));
            }
        }

        /// <summary>
        /// Consulta el número de planilla de una guía específica
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public long ConsultarPlanillaGuia(long idGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var envio = contexto.paObtenerGuiaPlanillaAsig_OPU(idGuia).FirstOrDefault();
                if (envio != null)
                    return envio.PAG_IdPlanillaAsignacionEnvio;
                else
                    return 0;
            }
        }


        /// <summary>
        /// Obtiene el total de los envios pendientes del mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public int ObtenerEnviosPendientesMensajero(long idMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int totalPendientes;
                var pendientes = contexto.paObtenerEnviosPendientes_OPU(idMensajero).FirstOrDefault();
                int.TryParse(pendientes.ToString(), out totalPendientes);

                return totalPendientes;
            }
        }

        /// <summary>
        /// Obtiene las entregas al cobro del mensajero que no ha reportado a caja.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <returns>Lista de Guias entregadas de alcobro  por mensajero</returns>
        public List<OUEnviosPendMensajerosDC> ObtenerEnviosPendMensajero(long idMensajero, long idComprobante)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var Mensaj = contexto.paObtenerEnviMenjAlCobro_CAJ(idMensajero, idComprobante).ToList();

                if (Mensaj.Count > 0)
                {
                    List<OUEnviosPendMensajerosDC> PendMensajero = Mensaj
                                  .ConvertAll<OUEnviosPendMensajerosDC>(envio => new OUEnviosPendMensajerosDC()
                                  {
                                      NumeroGuia = envio.ADM_NumeroGuia,
                                      NumeroPlanilla = envio.PAG_IdPlanillaAsignacionEnvio.Value,
                                      ValorTotalGuia = envio.PAG_ValorTotalGuia.Value,
                                      Descargado = false,
                                      FechaPlanilla = envio.PAM_FechaGrabacion,
                                      NoComprobantePago = envio.RTD_NumeroComprobante,
                                      AfectadoEnCaja = envio.RTD_IdRegistroTransDetalleCaja == null ? false : true
                                  });
                    return PendMensajero;
                }
                else
                {
                    //Error de no tiene pendientes mesanjero
                    List<OUEnviosPendMensajerosDC> PendMensajero = new List<OUEnviosPendMensajerosDC>();
                    return PendMensajero;
                }
            }
        }

        /// <summary>
        /// Actualiza las guias al cobro planilladas asociadas a un mensajero como ya reportadas en caja
        /// </summary>
        /// <param name="idMensajero"></param>
        public void ActualizarAlCobrosEntregaMensajero(long idMensajero, long idComprobante, List<OUEnviosPendMensajerosDC> alcobrosDescargados)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                alcobrosDescargados.ForEach(a =>
                    {
                        contexto.paActualizarEnviMenjAlCobro_CAJ(idMensajero, idComprobante, a.NumeroGuia);
                    }
                );

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Retorna los estados de la planilla
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUEstadosPlanillaAsignacionDC> ObtenerEstadosPlanillaAsignacion()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadosPlanillaEnvios_OPU.OrderBy(o => o.EPE_IdEstadoPlanillaEnvios).ToList().ConvertAll(r => new OUEstadosPlanillaAsignacionDC()
                {
                    Descripcion = r.EPE_Descripcion,
                    IdEstado = r.EPE_IdEstadoPlanillaEnvios
                });
            }
        }

        /// <summary>
        /// Consulta el mensajero que está asignado a una planilla.
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <returns></returns>
        public OUMensajeroDC ConsultarMensajeroPlanilla(long idPlanilla)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajeroPlanilla_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PAM_IdPlanillaAsignacionEnvio", idPlanilla);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var datosPlanilla = dt.AsEnumerable().FirstOrDefault();
                if (datosPlanilla != null)
                {
                    return new OUMensajeroDC()
                    {
                        IdMensajero = datosPlanilla.Field<long>("MEN_IdMensajero"),
                        NombreCompleto = datosPlanilla.Field<string>("NombreCompletoMensajero")
                    };
                }
                return null;


            }
            /*

            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaAsignaEnvioMensaj_OPU datosPlanilla = contexto.PlanillaAsignaEnvioMensaj_OPU.Include("Mensajero_CPO").Include("Mensajero_CPO.PersonaInterna_PAR").Where(m => m.PAM_IdPlanillaAsignacionEnvio == idPlanilla).FirstOrDefault();
                if (datosPlanilla != null)
                {
                    return new OUMensajeroDC()
                    {
                        IdMensajero = datosPlanilla.Mensajero_CPO.MEN_IdMensajero,
                        NombreCompleto = datosPlanilla.Mensajero_CPO.PersonaInterna_PAR.PEI_Nombre + datosPlanilla.Mensajero_CPO.PersonaInterna_PAR.PEI_PrimerApellido + datosPlanilla.Mensajero_CPO.PersonaInterna_PAR.PEI_SegundoApellido
                    };
                }
                return null;
            }*/
        }

        #endregion Consultas

        #region Inserción

        /// <summary>
        /// Guarda en la cuenta del mensajero los al cobros de una planilla ya sea por asignación, o desasignación
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idMensajero"></param>
        public void AfectarCuentaMensajeroPorAsignacion(long idPlanilla, long idMensajero, string nombreMensajero, CAEnumConceptosCaja conceptoCaja, string observaciones)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {                
                SqlCommand cmd = new SqlCommand("paInsAlCobrosCuentaMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@NombreMensajero", nombreMensajero);
                cmd.Parameters.AddWithValue("@ConceptoCaja", (int)conceptoCaja);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Observacion", observaciones);
                
                conn.Open();
                cmd.ExecuteNonQuery();

            }

            /*

            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paInsAlCobrosCuentaMensajero_OPU(idPlanilla, idMensajero, nombreMensajero, (int)conceptoCaja, ControllerContext.Current.Usuario, observaciones);
                contexto.SaveChanges();
            }*/
        }

        /// <summary>
        /// Guarda la planilla de asignacion de envios
        /// </summary>
        /// <param name="planilla"></param>
        public long GuardarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                long id;                           
                SqlCommand cmd = new SqlCommand("paCrearPlanillaAsignaEnvio_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAgencia", planilla.IdAgencia);
                cmd.Parameters.AddWithValue("@idCentroLogistico", planilla.IdCentroLogistico);
                cmd.Parameters.AddWithValue("@idZona", planilla.Zona.IdZona);
                cmd.Parameters.AddWithValue("@descripcionZona", planilla.Zona.Descripcion);
                cmd.Parameters.AddWithValue("@idMunicipio", planilla.IdCiudad);
                cmd.Parameters.AddWithValue("@idTipoMensajero", planilla.Mensajero.TipMensajeros.IdTipoMensajero);
                cmd.Parameters.AddWithValue("@descripcionTipoMen", planilla.Mensajero.TipMensajeros.Descripcion);
                cmd.Parameters.AddWithValue("@idEstadoPlanilla", planilla.IdEstadoPlanilla);
                cmd.Parameters.AddWithValue("@totalGuiasAsignadas", planilla.TotalGuias);
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@requiereAuditoria", planilla.RequiereAuditoria);
                conn.Open();
                return Convert.ToInt64(cmd.ExecuteScalar());

            }

           /* using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long id;

                decimal? idPlanilla = contexto.paCrearPlanillaAsignaEnvio_OPU(planilla.IdAgencia
                  , planilla.IdCentroLogistico
                  , planilla.Zona.IdZona
                  , planilla.Zona.Descripcion
                  , planilla.IdCiudad
                  , planilla.Mensajero.TipMensajeros.IdTipoMensajero
                  , planilla.Mensajero.TipMensajeros.Descripcion
                  , planilla.IdEstadoPlanilla
                  , planilla.TotalGuias
                  , DateTime.Now
                  , ControllerContext.Current.Usuario
                  , planilla.RequiereAuditoria
                  ).FirstOrDefault();

                if (long.TryParse(idPlanilla.ToString(), out id))
                    return id;
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, ETipoErrorFramework.EX_ERROR_PLANILLA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_PLANILLA)));
            }*/
        }

        /// <summary>
        /// Modifica una planilla de asignacion de envios
        /// </summary>
        /// <param name="planilla"></param>
        public void ModificarPlanillaAsignacionEnvio(OUPlanillaAsignacionDC planilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PlanillaAsignacionEnvios_OPU planti = contexto.PlanillaAsignacionEnvios_OPU.Where(p => p.PAE_IdPlanillaAsignacionEnvio == planilla.IdPlanillaAsignacion).FirstOrDefault();

                if (planti != null)
                {
                    if (planti.PAE_IdTipoMensajero != planilla.Mensajero.TipMensajeros.IdTipoMensajero || planti.PAE_IdZona != planilla.Zona.IdZona || planti.PAE_RequiereAuditoria != planilla.RequiereAuditoria)
                    {
                        contexto.paModificarPlanillaAsignaEnvio_OPU(planilla.IdPlanillaAsignacion, planilla.Zona.IdZona, planilla.Zona.Descripcion, planilla.Mensajero.TipMensajeros.IdTipoMensajero, planilla.Mensajero.TipMensajeros.Descripcion, planilla.RequiereAuditoria);
                        contexto.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Guarda las guias de la planilla de asignacion
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarGuiaPlanillaAsignacion(OUPlanillaAsignacionDC planilla)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCrearPlanillaAsignaGuia_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;     
                cmd.Parameters.AddWithValue("@idPlanilla", planilla.IdPlanillaAsignacion);
                cmd.Parameters.AddWithValue("@idAdmisionMensajeria", planilla.Guias.IdAdmision);
                cmd.Parameters.AddWithValue("@numeroGuia", planilla.Guias.NumeroGuia);
                cmd.Parameters.AddWithValue("@valorTotalGuia", planilla.Guias.ValorTotal);
                cmd.Parameters.AddWithValue("@idServicio", planilla.Guias.DetalleGuia.IdServicio);
                cmd.Parameters.AddWithValue("@nombreServicio", planilla.Guias.DetalleGuia.NombreServicio);
                cmd.Parameters.AddWithValue("@consecutivo", planilla.Guias.Consecutivo);
                cmd.Parameters.AddWithValue("@estaDescargada", planilla.Guias.EstaDescargada);
                cmd.Parameters.AddWithValue("@recogeDinero", planilla.Guias.RecogeDinero);
                cmd.Parameters.AddWithValue("@cantidadRadicados", planilla.Guias.CantidadRadicados);
                cmd.Parameters.AddWithValue("@idCiudadOrigen", planilla.Guias.DetalleGuia.IdCiudadOrigen);
                cmd.Parameters.AddWithValue("@nombreCiudadOrigen", planilla.Guias.DetalleGuia.CiudadOrigen);
                cmd.Parameters.AddWithValue("@totalPiezas", planilla.Guias.TotalPiezas);
                cmd.Parameters.AddWithValue("@peso", planilla.Guias.PesoSistema);
                cmd.Parameters.AddWithValue("@telefonoDestinatario", planilla.Guias.TelefonoDestinatario);
                cmd.Parameters.AddWithValue("@direccionDestinatario", planilla.Guias.DireccionDestinatario);
                cmd.Parameters.AddWithValue("@idCiudadDestino", planilla.Guias.DetalleGuia.IdCiudadDestino);
                cmd.Parameters.AddWithValue("@nombreCiudadDestino", planilla.Guias.DetalleGuia.CiudadDestino);
                cmd.Parameters.AddWithValue("@idTipoEnvio", planilla.Guias.DetalleGuia.IdTipoEnvio);
                cmd.Parameters.AddWithValue("@descripcionTipoEnvio", planilla.Guias.DetalleGuia.TipoEnvio);
                cmd.Parameters.AddWithValue("@fechaDescargue", planilla.Guias.FechaDescarga);
                cmd.Parameters.AddWithValue("@descargadaPor", planilla.Guias.CreadoPor);
                cmd.Parameters.AddWithValue("@estadoEnPlanilla", planilla.Guias.EstadoGuiaPlanilla);
                cmd.Parameters.AddWithValue("@fuePlanillada", planilla.Guias.EstaPlanillada);
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", planilla.Guias.CreadoPor);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }

            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearPlanillaAsignaGuia_OPU(planilla.IdPlanillaAsignacion
                 , planilla.Guias.IdAdmision
                 , planilla.Guias.NumeroGuia
                 , planilla.Guias.ValorTotal
                 , planilla.Guias.DetalleGuia.IdServicio
                 , planilla.Guias.DetalleGuia.NombreServicio
                 , planilla.Guias.Consecutivo
                 , planilla.Guias.EstaDescargada
                 , planilla.Guias.RecogeDinero
                 , planilla.Guias.CantidadRadicados
                 , planilla.Guias.DetalleGuia.IdCiudadOrigen
                 , planilla.Guias.DetalleGuia.CiudadOrigen
                 , planilla.Guias.TotalPiezas
                 , planilla.Guias.PesoSistema
                 , planilla.Guias.TelefonoDestinatario
                 , planilla.Guias.DireccionDestinatario
                 , planilla.Guias.DetalleGuia.IdCiudadDestino
                 , planilla.Guias.DetalleGuia.CiudadDestino
                 , planilla.Guias.DetalleGuia.IdTipoEnvio
                 , planilla.Guias.DetalleGuia.TipoEnvio
                 , planilla.Guias.FechaDescarga
                 , planilla.Guias.CreadoPor
                 , planilla.Guias.EstadoGuiaPlanilla
                 , planilla.Guias.EstaPlanillada
                 , DateTime.Now
                 , planilla.Guias.CreadoPor);
            }*/
        }

        /// <summary>
        /// Asigna a la planilla un mensajero
        /// </summary>
        /// <param name="planilla"></param>
        public void GuardarMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paAsignacionPlanillaMensaj_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idplanilla", planilla.IdPlanillaAsignacion);
                cmd.Parameters.AddWithValue("@idMensajero", planilla.Mensajero.IdMensajero);
                cmd.Parameters.AddWithValue("@AutorizadoPor", planilla.Mensajero.CreadoPor);
                cmd.Parameters.AddWithValue("@fechaAutorizacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }
            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paAsignacionPlanillaMensaj_OPU(planilla.IdPlanillaAsignacion
                  , planilla.Mensajero.IdMensajero
                  , planilla.Mensajero.CreadoPor
                  , DateTime.Now
                  , DateTime.Now
                  , ControllerContext.Current.Usuario);
            }*/
        }

        #endregion Inserción

        #region Eliminar

        /// <summary>
        /// Elimina el envio de la planilla de asignación
        /// </summary>
        /// <param name="idAdmisionMensajeria"></param>
        public void EliminarEnvioPlanillaAsignacion(long idAdmisionMensajeria, long idPlanillaAsignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                bool? estado = contexto.paBorrarEnvioPlanillaAsig_OPU(idAdmisionMensajeria, idPlanillaAsignacion, ControllerContext.Current.Usuario).SingleOrDefault();

                if (!estado.Value)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_PLANILLA_CERRADA_O_NO_EXISTE.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_PLANILLA_CERRADA_O_NO_EXISTE)));
                }
            }
        }

        #endregion Eliminar

        #region Actualizacion

        /// <summary>
        /// Actualiza el total de los envios en la planilla de asignacion de envios
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla, int totalGuias)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizaPlanillaAsig_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPlanilla", idPlanilla);
                cmd.Parameters.AddWithValue("@totalGuias", totalGuias);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizaPlanillaAsig_OPU(idPlanilla, totalGuias);
            }*/
        }

        /// <summary>
        /// Actualiza el total de los envios planillados
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="totalGuias"></param>
        public void ActualizaTotalEnviosPlanillados(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarTotalPlaniMens_OPU(idPlanilla);
            }
        }

        /// <summary>
        /// verifica el envio seleccionado
        /// </summary>
        /// <param name="guia"></param>
        public void ActualizaEnvioVerificadoPlanillaAsignacion(OUGuiaIngresadaDC guia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualiEnvioPlanillaAsig_OPU(guia.EstaVerificada
                  , ControllerContext.Current.Usuario
                  , DateTime.Now
                  , guia.IdAdmision);
            }
        }

        /// <summary>
        /// Reasigna el envio seleccionado
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idAdmisionMensajeria"></param>
        public void ReasignaEnvioPlanillaAsignacion(long idPlanilla, long idPlanillaNueva, long idAdmisionMensajeria, long idAgencia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? resultado = contexto.paReasignaEnvioPlanAsig_OPU(idPlanillaNueva, idPlanilla, idAdmisionMensajeria, idAgencia, ControllerContext.Current.Usuario).FirstOrDefault();

                if (resultado.HasValue)
                    if (resultado == 0)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_PLANILLA_CERRADA_O_NO_EXISTE.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_PLANILLA_CERRADA_O_NO_EXISTE)));
            }
        }

        /// <summary>
        /// Cierra la planilla de asignacion el procedimiento valida q la planilla no tenga envios sin
        /// verificar, el procedimiento retorna la cantidad de envios sin verificar
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void CerrarPlanillaAsignacion(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int enviosSinVerificar;
                var resultado = contexto.paCerrarPlanillaAsignacion_OPU(idPlanilla, DateTime.Now, ControllerContext.Current.Usuario).FirstOrDefault();

                if ((int.TryParse(resultado.ToString(), out enviosSinVerificar)))
                {
                    if (enviosSinVerificar > 0)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_ENVIOS_SIN_VERIFICAR.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_ENVIOS_SIN_VERIFICAR)));
                }
            }
        }

        /// <summary>
        ///Cierra la planilla de ventas
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void CerrarPlanillaVentas(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCierraPlanillaVentas_OPU(idPlanilla);
            }
        }

        /// <summary>
        /// Abre la planilla de asignación, el procedimiento almacenado hace el insert en la tabla de log
        /// </summary>
        /// <param name="idPlanilla"></param>
        public void AbrirPlanillaAsignacion(long idPlanilla)
        {
            SqlParameter outputIdParam;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paAbrirPlanillaAsignacion_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idPlanilla", idPlanilla));
                cmd.Parameters.Add(new SqlParameter("@creadoPor", ControllerContext.Current.Usuario));
                outputIdParam = new SqlParameter("@respuesta", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }

            if ((int)outputIdParam.Value == 0)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_NO_SE_PUEDE_ABRIR_LA_PLANILLA_ASIGNACION.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_NO_SE_PUEDE_ABRIR_LA_PLANILLA_ASIGNACION)));
        }

        /// <summary>
        /// Actualiza la asignacion del mensajero a la planilla
        /// </summary>
        public void ActualizaMensajeroPlanilla(OUPlanillaAsignacionDC planilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paReasignaPlanillaMensajer_OPU(planilla.IdPlanillaAsignacion
                  , planilla.Mensajero.IdMensajero
                  , planilla.Mensajero.CreadoPor
                  , DateTime.Now);
            }
        }

        /// <summary>
        /// Nivela a cero todas las cuentas de mensajeros asociadas a un número de planilla específico
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <param name="observaciones"></param>
        /// <param name="usuario"></param>
        public void NivelarCuentasMensajerosACero(long idPlanilla, string observaciones)
        {
          
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paNivelarCuentaMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPlanilla", idPlanilla);
                cmd.Parameters.AddWithValue("@Observaciones", observaciones);
                cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);                
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

          
            }
            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paNivelarCuentaMensajero_OPU(idPlanilla, observaciones, ControllerContext.Current.Usuario);
            }*/
        }

        /// <summary>
        /// Nivela a cero todas las cuentas de mensajeros asociadas a un número de factura específico
        /// </summary>
        /// <param name="numeroDocumento"></param>
        /// <param name="observaciones"></param>
        /// <param name="usuario"></param>
        public void NivelarCuentasMensajerosACeroXFactura(long noFactura, string observaciones, int idConcepto)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paNivelarCuentaMensajeroXFac_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;                 
                cmd.Parameters.AddWithValue("@NoFactura", noFactura);
                cmd.Parameters.AddWithValue("@Observaciones", observaciones);
                cmd.Parameters.AddWithValue("@Usuario", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdConcepto", idConcepto);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

            }

            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paNivelarCuentaMensajeroXFac_OPU(noFactura, observaciones, ControllerContext.Current.Usuario, idConcepto);
            }*/
        }

        public string ObtenerParametroDesfasePeso()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var parametro = contexto.ParametrosOperacionUrbana_OPU.Where(r => r.POU_IdParametro == OUConstantesOperacionUrbana.PARAMETRO_DESFASE_PESO).SingleOrDefault();

                return parametro.POU_ValorParametro;
            }
        }

        /// <summary>
        /// Retorna el valor del parametro del id indicado
        /// </summary>
        /// <returns></returns>
        public string ObtenerValorParametro(string idParametro)
        {

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametroOperacionUrbana_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@POU_IdParametro", idParametro);
                conn.Open();
                return Convert.ToString(cmd.ExecuteScalar());

            }
            /*
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var parametro = contexto.ParametrosOperacionUrbana_OPU.Where(r => r.POU_IdParametro == idParametro).SingleOrDefault();

                return parametro.POU_ValorParametro;
            }*/
        }

        #endregion Actualizacion

        #endregion Planilla Asignacion

        #region Solicitud Recogidas

        #region Generales

        /// <summary>
        /// Retorna los estados de la solicitud de recogidas
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosSolicitudRecogidaDC> ObtenerEstadosRecogida()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoSolicitudRecogida_OPU.OrderBy(o => o.EPS_Descripcion).ToList().ConvertAll(r => new OUEstadosSolicitudRecogidaDC()
                {
                    Descripcion = r.EPS_Descripcion,
                    IdEstado = r.EPS_IdEstadoSolicitudRecogida
                });
            }
        }

        #endregion Generales

        #region Consultas

        /// <summary>
        /// Consulta la solicitud de recogida para un cliente peaton especifico
        /// </summary>x
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionClientePeaton(OURecogidasDC infoRecogida)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SolicitudRecogidas_VOPU recogida = null;

                recogida = contexto.SolicitudRecogidas_VOPU.ToList().Where(r => r.SRP_Identificacion == infoRecogida.RecogidaPeaton.DocumentoCliente
                 && r.SRP_IdTipoIdentificacion == infoRecogida.RecogidaPeaton.TipoIdentificacion.IdTipoIdentificacion)
                 .FirstOrDefault();

                if (recogida != null)
                {
                    infoRecogida.RecogidaPeaton.DocumentoCliente = recogida.SRP_Identificacion;
                    infoRecogida.RecogidaPeaton.DireccionCliente = recogida.SOR_Direccion;
                    infoRecogida.RecogidaPeaton.TelefonoCliente = recogida.SOR_Telefono;
                    infoRecogida.RecogidaPeaton.NombreCliente = recogida.SRP_Nombre;
                    infoRecogida.LocalidadRecogida = new PALocalidadDC() { IdLocalidad = recogida.SOR_IdMunicipio, Nombre = recogida.LOC_Nombre };
                }

                return infoRecogida;
            }
        }

        /// <summary>
        /// Obtiene las recogidas asociadas al Col del usuario registrado en la aplicación
        /// </summary>
        public List<OURecogidasDC> ObtenerRecogidas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idAgencia, bool incluyeFechaAsignacion, bool incluyeFechaRecogida)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaSolicitud;
                DateTime fechaSolicitudFinal;
                DateTime fechaRecogida;
                DateTime fechaRecogidaFinal;
                string idRecogida;
                string idMensajero;
                string direccion;
                string contacto;
                string recepciono;
                string idEstado;
                string fechaSolicitudRecogida;
                string fechaRecogidaFiltro;

                filtro.TryGetValue("SOR_IdSolicitudRecogida", out idRecogida);
                filtro.TryGetValue("SOR_Direccion", out direccion);
                filtro.TryGetValue("SOR_PersonaContacto", out contacto);
                filtro.TryGetValue("SPP_IdMensajero", out idMensajero);
                filtro.TryGetValue("SOR_NombrePersonaCreaSolicitud", out recepciono);
                filtro.TryGetValue("SOR_IdEstadoSolicitudRecogida", out idEstado);
                filtro.TryGetValue("SOR_FechaGrabacion", out fechaSolicitudRecogida);
                filtro.TryGetValue("SOR_FechaRecogida", out fechaRecogidaFiltro);

                if (!incluyeFechaAsignacion)
                    fechaSolicitudRecogida = DateTime.Now.ToString();
                fechaSolicitud = Convert.ToDateTime(fechaSolicitudRecogida, Thread.CurrentThread.CurrentCulture);
                fechaSolicitudFinal = fechaSolicitud.AddDays(1);

                if (!incluyeFechaRecogida)
                    fechaRecogidaFiltro = DateTime.Now.ToString();
                fechaRecogida = Convert.ToDateTime(fechaRecogidaFiltro, Thread.CurrentThread.CurrentCulture);
                fechaRecogidaFinal = fechaRecogida.AddDays(1);

                if (indicePagina == 0)
                    indicePagina = 1;

                if (Convert.ToInt64(idMensajero) > 0)
                    return contexto.paObtenerSolicitudRecogida_OPU(idAgencia
                          , Convert.ToInt64(idMensajero)
                          , Convert.ToInt32(idEstado)
                          , Convert.ToInt64(idRecogida)
                          , indicePagina
                          , registrosPorPagina
                          , incluyeFechaAsignacion
                          , incluyeFechaRecogida
                          , fechaSolicitud
                          , fechaSolicitudFinal
                          , fechaRecogida
                          , fechaRecogidaFinal
                          , direccion
                          , contacto
                          , recepciono)
                    .ToList()
                    .ConvertAll(r => new OURecogidasDC()
                    {
                        IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog != null ? r.PSR_IdProgramacionSolicitudRecog.Value : 0,
                        Contacto = r.SOR_PersonaContacto,
                        Direccion = r.SOR_Direccion,
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                        {
                            IdEstado = r.SOR_IdEstadoSolicitudRecogida,
                            Descripcion = r.EPS_Descripcion
                        },
                        FechaRecogida = r.SOR_FechaRecogida,
                        FechaSolicitud = r.SOR_FechaGrabacion,
                        IdRecogida = r.SOR_IdSolicitudRecogida,
                        MensajeroPlanilla = new OUNombresMensajeroDC()
                        {
                            IdPersonaInterna = r.SPP_IdMensajero.Value,
                            NombreApellido = r.SPP_NombreMensajero == null ? "Sin asignar" : r.SPP_NombreMensajero
                        },

                        NombreCliente = r.SOR_NombreCliente,
                        PersonaRecepcionoRecogida = r.SOR_NombrePersonaCreaSolicitud,
                        Zona = new PAZonaDC()
                        {
                            IdZona = r.PSR_IdZona == null ? "Sin dato" : r.PSR_IdZona,
                            Descripcion = r.PSR_DescripcionZona == null ? "Sin dato" : r.PSR_DescripcionZona
                        },
                        EstaReportada = r.SPP_EstaReportadaMensajero.Value,
                        IdTipoRecogida = r.SOR_TipoRecogida,
                        TipoCliente = (OUEnumTipoClienteRecogidaDC)Enum.Parse(typeof(OUEnumTipoClienteRecogidaDC), r.SOR_TipoClienteRecogida, true),
                    });
                else
                    return contexto.paObtenerSolRecoSinFiltMen_OPU(idAgencia
                          , Convert.ToInt32(idEstado)
                          , Convert.ToInt64(idRecogida)
                          , indicePagina
                          , registrosPorPagina
                          , incluyeFechaAsignacion
                          , incluyeFechaRecogida
                          , fechaSolicitud
                          , fechaSolicitudFinal
                          , fechaRecogida
                          , fechaRecogidaFinal
                          , direccion
                          , contacto
                          , recepciono)
                          .ToList()
                          .ConvertAll(r => new OURecogidasDC()
                          {
                              IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog != null ? r.PSR_IdProgramacionSolicitudRecog.Value : 0,
                              Contacto = r.SOR_PersonaContacto,
                              Direccion = r.SOR_Direccion,
                              EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                              {
                                  IdEstado = r.SOR_IdEstadoSolicitudRecogida,
                                  Descripcion = r.EPS_Descripcion
                              },
                              FechaRecogida = r.SOR_FechaRecogida,
                              FechaSolicitud = r.SOR_FechaGrabacion,
                              IdRecogida = r.SOR_IdSolicitudRecogida,
                              MensajeroPlanilla = new OUNombresMensajeroDC()
                              {
                                  IdPersonaInterna = r.SPP_IdMensajero.HasValue == false ? 0 : r.SPP_IdMensajero.Value,
                                  NombreApellido = r.SPP_NombreMensajero == null ? "Sin asignar" : r.SPP_NombreMensajero
                              },
                              NombreCliente = r.SOR_NombreCliente,
                              PersonaRecepcionoRecogida = r.SOR_NombrePersonaCreaSolicitud,
                              Zona = new PAZonaDC()
                              {
                                  IdZona = r.PSR_IdZona == null ? "Sin dato" : r.PSR_IdZona,
                                  Descripcion = r.PSR_DescripcionZona == null ? "Sin dato" : r.PSR_DescripcionZona
                              },
                              EstaReportada = r.SPP_EstaReportadaMensajero.HasValue == false ? false : r.SPP_EstaReportadaMensajero.Value,
                              IdTipoRecogida = r.SOR_TipoRecogida,
                              TipoCliente = (OUEnumTipoClienteRecogidaDC)Enum.Parse(typeof(OUEnumTipoClienteRecogidaDC), r.SOR_TipoClienteRecogida, true)
                          });
            }
        }

        #endregion Consultas

        #region Insercion

        /// <summary>
        /// Guarda la solicitud de recogida del punto de servicios
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardaSolicitudRecogidaPuntoSvc(OURecogidasDC recogida)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long id;
                using (TransactionScope scope = new TransactionScope())
                {
                    decimal? idSolicitud = contexto.paCrearSolicitudRecogida_OPU(recogida.IdAgenciaResponsable
                      , OUEnumTipoClienteRecogida.PSE.ToString()
                      , OUConstantesOperacionUrbana.TIPO_RECOGIDA_ESPORADICA
                      , OUConstantesOperacionUrbana.ID_ESTADO_PENDIENTE_POR_PROGRAMAR
                      , recogida.FechaRecogida
                      , recogida.PersonaSolicita
                      , recogida.Contacto
                      , recogida.CantidadEnvios != null ? recogida.CantidadEnvios.Value : (short)0
                      , recogida.PesoAproximado != null ? recogida.PesoAproximado.Value : 0
                      , recogida.Observaciones
                      , recogida.PuntoServicio.IdMunicipio
                      , recogida.PuntoServicio.Direccion
                      , recogida.PuntoServicio.Telefono1
                      , recogida.PersonaRecepcionoRecogida
                      , DateTime.Now
                      , ControllerContext.Current.Usuario
                      , recogida.NombreCliente).FirstOrDefault();

                    if (long.TryParse(idSolicitud.ToString(), out id))
                    {
                        contexto.paCrearSolRecoPuntoSvc_OPU(id
                          , recogida.PuntoServicio.IdCentroServicio
                          , DateTime.Now
                          , ControllerContext.Current.Usuario);
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_ERROR_RECOGIDA.ToString(), OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_ERROR_RECOGIDA)));
                    }
                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// Guarda la solicitud de la recogida por cliente convenio
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarSolicitudClienteConvenio(OURecogidasDC recogida)
        {

            long idSolicitud = 0;
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                try
                {
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();
                    SqlCommand cmd = new SqlCommand("paCrearSolicitudRecogida_OPU", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // cmd.Transaction = objtran;
                    cmd.Parameters.AddWithValue("@idAgencia", recogida.IdAgenciaResponsable);
                    cmd.Parameters.AddWithValue("@tipoCliente", OUEnumTipoClienteRecogida.CON.ToString());
                    cmd.Parameters.AddWithValue("@tipoRecogida", OUConstantesOperacionUrbana.TIPO_RECOGIDA_ESPORADICA);
                    cmd.Parameters.AddWithValue("@idEstadoSolicitudRecogida", OUConstantesOperacionUrbana.ID_ESTADO_PENDIENTE_POR_PROGRAMAR);
                    cmd.Parameters.AddWithValue("@fechaRecogida", recogida.FechaRecogida);
                    cmd.Parameters.AddWithValue("@personaQueSolicita", recogida.PersonaSolicita);
                    cmd.Parameters.AddWithValue("@personaContacto", recogida.Contacto);
                    cmd.Parameters.AddWithValue("@cantidadEnvios", recogida.CantidadEnvios != null ? recogida.CantidadEnvios.Value : (short)0);
                    cmd.Parameters.AddWithValue("@pesoAproximado", recogida.PesoAproximado != null ? recogida.PesoAproximado : 0);
                    cmd.Parameters.AddWithValue("@observaciones", recogida.Observaciones);
                    cmd.Parameters.AddWithValue("@idMunicipio", recogida.Sucursal.Localidad);
                    cmd.Parameters.AddWithValue("@direccion", recogida.Sucursal.Direccion);
                    cmd.Parameters.AddWithValue("@telefono", recogida.Sucursal.Telefono);
                    cmd.Parameters.AddWithValue("@nombreCreadoPor", recogida.PersonaRecepcionoRecogida);
                    cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@cliente", recogida.NombreCliente);
                    cmd.Parameters.AddWithValue("@SOR_Longitud", null);
                    cmd.Parameters.AddWithValue("@SOR_Latitud", null);
                    cmd.Parameters.AddWithValue("@SOR_TipoOrigenRecogida", recogida.TipoOrigenRecogida.ToString());
                    cmd.Parameters.AddWithValue("@SOR_Celular", null);
                    cmd.Parameters.AddWithValue("@SOR_Email", null);
                    cmd.Parameters.AddWithValue("@SOR_ComplementoDireccion", null);
                    idSolicitud = Convert.ToInt64(cmd.ExecuteScalar());
                    SqlCommand cmd2 = new SqlCommand("paCrearSolRecoCliConvenio_OPU", cnn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    // cmd2.Transaction = objtran;
                    cmd2.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                    cmd2.Parameters.AddWithValue("@idCliente", recogida.Cliente.IdCliente);
                    cmd2.Parameters.AddWithValue("@nit", recogida.Cliente.Nit);
                    cmd2.Parameters.AddWithValue("@razonSocial", recogida.Cliente.RazonSocial);
                    cmd2.Parameters.AddWithValue("@idSucursal", recogida.Sucursal.IdSucursal);
                    cmd2.Parameters.AddWithValue("@nombreSucursal", recogida.Sucursal.Nombre);
                    cmd2.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                    cmd2.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                    cmd2.ExecuteNonQuery();
                }
                catch (Exception x)
                {
                    throw x;
                }
                finally
                {
                    if (cnn.State != ConnectionState.Closed)
                        cnn.Close();
                    cnn.Dispose();
                }
            }
        }

        public long GuardaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            long idSolicitud = 0;
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                SqlTransaction objtran = null;

                try
                {
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();

                    objtran = cnn.BeginTransaction();

                    SqlCommand cmd = new SqlCommand("paCrearSolicitudRecogida_OPU", cnn, objtran);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idAgencia", recogida.IdAgenciaResponsable);
                    cmd.Parameters.AddWithValue("@tipoCliente", ADEnumTipoCliente.PEA.ToString());
                    cmd.Parameters.AddWithValue("@tipoRecogida", OUConstantesOperacionUrbana.TIPO_RECOGIDA_ESPORADICA);
                    //cmd.Parameters.AddWithValue("@idEstadoSolicitudRecogida", OUConstantesOperacionUrbana.ID_ESTADO_PENDIENTE_POR_PROGRAMAR);
                    cmd.Parameters.AddWithValue("@idEstadoSolicitudRecogida", recogida.EstadoRecogida.IdEstado);
                    cmd.Parameters.AddWithValue("@fechaRecogida", recogida.FechaRecogida);
                    cmd.Parameters.AddWithValue("@personaQueSolicita", recogida.PersonaSolicita);
                    cmd.Parameters.AddWithValue("@personaContacto", recogida.Contacto);
                    cmd.Parameters.AddWithValue("@cantidadEnvios", recogida.CantidadEnvios != null ? recogida.CantidadEnvios.Value : (short)0);
                    cmd.Parameters.AddWithValue("@pesoAproximado", recogida.PesoAproximado != null ? recogida.PesoAproximado : 0);
                    cmd.Parameters.AddWithValue("@observaciones", recogida.Observaciones);
                    cmd.Parameters.AddWithValue("@idMunicipio", recogida.LocalidadRecogida.IdLocalidad);
                    cmd.Parameters.AddWithValue("@direccion", recogida.RecogidaPeaton.DireccionCliente);
                    cmd.Parameters.AddWithValue("@telefono", recogida.RecogidaPeaton.TelefonoCliente);
                    cmd.Parameters.AddWithValue("@nombreCreadoPor", recogida.PersonaRecepcionoRecogida);
                    cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@cliente", recogida.NombreCliente);
                    cmd.Parameters.AddWithValue("@SOR_Longitud", recogida.LongitudRecogida);
                    cmd.Parameters.AddWithValue("@SOR_Latitud", recogida.LatitudRecogida);
                    cmd.Parameters.AddWithValue("@SOR_TipoOrigenRecogida", recogida.TipoOrigenRecogida.ToString());
                    cmd.Parameters.AddWithValue("@SOR_Celular", recogida.RecogidaPeaton.Celular);
                    cmd.Parameters.AddWithValue("@SOR_Email", recogida.RecogidaPeaton.Email);
                    cmd.Parameters.AddWithValue("@SOR_ComplementoDireccion", recogida.ComplementoDireccion);
                    idSolicitud = Convert.ToInt64(cmd.ExecuteScalar());

                    SqlCommand cmd2 = new SqlCommand("paCrearSolRecogidaPeaton_OPU", cnn, objtran);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                    cmd2.Parameters.AddWithValue("@idTipoIdentificacion", recogida.RecogidaPeaton.TipoIdentificacion.IdTipoIdentificacion);
                    cmd2.Parameters.AddWithValue("@identificacion", recogida.RecogidaPeaton.DocumentoCliente);
                    cmd2.Parameters.AddWithValue("@nombre", recogida.RecogidaPeaton.NombreCliente);
                    cmd2.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                    cmd2.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                    cmd2.ExecuteNonQuery();

                    cmd2 = new SqlCommand("paInsertarEstadoRecogidaTraza_OPU", cnn, objtran);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@ERT_IdSolicitudRecogida", idSolicitud);
                    cmd2.Parameters.AddWithValue("@ERT_IdEstadoSolicitud", recogida.EstadoRecogida.IdEstado);
                    cmd2.Parameters.AddWithValue("@ERT_CreadoPor", ControllerContext.Current.Usuario);
                    cmd2.ExecuteNonQuery();



                    foreach (OUEnviosRecogidaPeatonDC envio in recogida.RecogidaPeaton.EnviosRecogida)
                    {
                        SqlCommand cmd3 = new SqlCommand("paCrearTipEnvSolRecoPeaton_OPU", cnn, objtran);
                        cmd3.CommandType = CommandType.StoredProcedure;
                        cmd3.Parameters.AddWithValue("@idMunicipio", envio.MunicipioDestino.IdLocalidad);
                        cmd3.Parameters.AddWithValue("@idTipoEnvio", envio.TipoEnvio.IdTipoEnvio);
                        cmd3.Parameters.AddWithValue("@nombreEnvio", envio.TipoEnvio.Descripcion);
                        cmd3.Parameters.AddWithValue("@cantidad", envio.CantidadEnvios != null ? envio.CantidadEnvios.Value : (short)0);
                        cmd3.Parameters.AddWithValue("@pesoAproximado", envio.PesoAproximado != null ? envio.PesoAproximado.Value : 0);
                        cmd3.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                        cmd3.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                        cmd3.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                        cmd3.ExecuteNonQuery();
                    }

                    objtran.Commit();
                }
                catch (Exception x)
                {
                    objtran.Rollback();
                    throw x;
                }
                finally
                {
                    if (cnn.State != ConnectionState.Closed)
                        cnn.Close();
                    cnn.Dispose();
                }
                return idSolicitud;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagenes"></param>
        /// <param name="idSolicitudRecogida"></param>
        public void GuardarFotografiasSolicitudRecogidaPeaton(List<string> imagenes, long idSolicitudRecogida)
        {
            string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenRecogida");
            string carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);


            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                int file = 0;
                imagenes = imagenes.Where(i => !string.IsNullOrEmpty(i)).ToList();
                foreach (string imagen in imagenes)
                {

                    file++;
                    byte[] bytebuffer = Convert.FromBase64String(imagen);
                    MemoryStream memoryStream = new MemoryStream(bytebuffer);
                    var image = Image.FromStream(memoryStream);
                    ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
                    string ruta = carpetaDestino + "\\" + idSolicitudRecogida + "-" + file + ".jpg";

                    Encoder myEncoder = Encoder.Quality;
                    EncoderParameters myEncoderParameters = new EncoderParameters(1);
                    EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    image.Save(ruta, jpgEncoder, myEncoderParameters);

                    SqlCommand cmd1 = new SqlCommand("paGuardarImagenesSolicitudRecogida_OPU", cnn);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@IdSolicitudRecogida", idSolicitudRecogida);
                    cmd1.Parameters.AddWithValue("@RutaImagen", ruta);
                    cmd1.Parameters.AddWithValue("@CreadoPor", "APP");
                    cnn.Open();
                    cmd1.ExecuteNonQuery();
                    cnn.Close();

                }
            }
        }




        #region ActualizaSolicitudClientePeaton

        public void ActualizaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                try
                {
                    if (cnn.State != ConnectionState.Open)
                        cnn.Open();

                    SqlCommand cmd = new SqlCommand("paModificarSolicitudRecogida_SOR", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@SOR_IdSolicitud", recogida.IdRecogida));
                    cmd.Parameters.Add(new SqlParameter("@SOR_FechaRecogida", recogida.FechaRecogida));
                    cmd.Parameters.Add(new SqlParameter("@SOR_CantidadEnvios", recogida.CantidadEnvios));
                    cmd.Parameters.Add(new SqlParameter("@SOR_PesoAproximado", recogida.PesoAproximado));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Direccion", recogida.Direccion));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Complemento", recogida.ComplementoDireccion));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Longitud", recogida.LongitudRecogida));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Latitud", recogida.LatitudRecogida));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Email", recogida.Email));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Documento", recogida.Cliente.Nit));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Nombre", recogida.Cliente.NombreRepresentanteLegal));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Telefono", recogida.Cliente.Telefono));
                    cmd.Parameters.Add(new SqlParameter("@SOR_Celular", recogida.Contacto));

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    if (cnn.State != ConnectionState.Closed)
                        cnn.Close();
                    cnn.Dispose();
                }
            }
        }

        #endregion

        #endregion Insercion

        #region ObtenerDireccionesPeaton

        public List<OURecogidasDC> ObtenerDireccionesPeaton(OURecogidaPeatonDC Peaton)
        {
            List<OURecogidasDC> recogidas = new List<OURecogidasDC>();
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {

                con.Open();

                SqlCommand cmd = new SqlCommand("paObtenerDireccionesPeaton_OPU", con);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@SRP_IdTipoIdentificacion", Peaton.TipoIdentificacion.IdTipoIdentificacion));
                cmd.Parameters.Add(new SqlParameter("@SRP_Identificacion", Peaton.DocumentoCliente));

                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    recogidas.Add(new OURecogidasDC
                    {
                        LatitudRecogida = lector["SOR_Latitud"].ToString(),
                        LongitudRecogida = lector["SOR_Longitud"].ToString(),
                        Direccion = lector["SOR_Direccion"].ToString(),
                        ComplementoDireccion = lector["SOR_ComplementoDireccion"].ToString()
                    });
                }

                con.Close();

                con.Dispose();

            }
            return recogidas;
        }

        /// <summary>
        /// Metodo para obtener la ultima informacion registrada por el usuario externo
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionRecogidaUsuarioExterno(string nomUsuario)
        {
            OURecogidasDC recogida;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerInformacionRecogidaUsuarioExterno_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Identificacion", nomUsuario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return recogida = new OURecogidasDC()
                    {
                        NombreCliente = reader["SRP_Nombre"].ToString(),
                        Contacto = reader["SOR_PersonaContacto"].ToString(),
                        LocalidadRecogida = new PALocalidadDC()
                        {
                            IdLocalidad = reader["SOR_IdMunicipio"].ToString(),
                            Nombre = reader["LOC_Nombre"].ToString()
                        },
                        ComplementoDireccion = reader["SOR_ComplementoDireccion"].ToString(),
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            Email = reader["SOR_Email"].ToString(),
                            Celular = reader["SOR_Celular"].ToString(),
                            DireccionCliente = reader["SOR_Direccion"].ToString(),
                            DocumentoCliente = reader["SRP_Identificacion"].ToString()
                        },
                        LatitudRecogida = reader["SOR_Latitud"].ToString(),
                        LongitudRecogida = reader["SOR_Longitud"].ToString()
                    };
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion ObtenerDireccionesPeaton

        #region movil
        /// <summary>
        /// Inserta la relacion entre un dispositivo movil y una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        /// <param name="idDispositivoMovil"></param>
        public void RegistrarSolicitudRecogidaMovil(long idRecogida, long idDispositivoMovil)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarSolicitudRecogidaDispositivoMovil_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SRM_IdSolicitudRecogida", idRecogida);
                cmd.Parameters.AddWithValue("@SRM_IdDispositivoMovil", idDispositivoMovil);
                cmd.Parameters.AddWithValue("@SRM_CreadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        /// <summary>
        /// retorna el token del dispositivo movil con el que se hizo una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        public PADispositivoMovil ObtenerdispositivoMovilClienteRecogida(long idRecogida)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTokenDispositivoMovilSolicitudRecogida_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SRM_IdSolicitudRecogida", idRecogida);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                var reg = dt.AsEnumerable().ToList().FirstOrDefault();

                if (reg == null)
                {
                    return new PADispositivoMovil();
                }
                PADispositivoMovil dispositivo = new PADispositivoMovil()
                {
                    IdDispositivo = reg.Field<long>("DIM_IdDispositivo"),
                    SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), reg.Field<string>("DIM_SistemaOperativo")),
                    TokenDispositivo = reg.Field<string>("DIM_TokenDispositivo"),
                    TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), reg.Field<string>("DIM_TipoDispositivo"))
                };


                return dispositivo;
            }
        }


        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida
        /// y que la ultima notificacion haya sido lanzada hace mas de 10 minutos
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesDia()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSolicitudRecogidaPeatonPorProgramarNotificadas_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<OURecogidasDC>(d =>
                    {
                        OURecogidasDC r = new OURecogidasDC()
                        {
                            IdRecogida = d.Field<long>("SOR_IdSolicitudRecogida"),
                            CantidadEnvios = d.Field<short>("SOR_CantidadEnvios"),
                            LocalidadRecogida = new PALocalidadDC() { IdLocalidad = d.Field<string>("SOR_IdMunicipio") },
                            FechaRecogida = d.Field<DateTime>("SOR_FechaRecogida"),
                            Direccion = d.Field<string>("SOR_Direccion"),
                            EstadoRecogida = new OUEstadosSolicitudRecogidaDC() { IdEstado = d.Field<short>("SOR_IdEstadoSolicitudRecogida") },
                            RecogidaPeaton = new OURecogidaPeatonDC()
                            {
                                NombreCliente = d.Field<string>("SRP_Nombre"),
                                TipoIdentificacion = new PATipoIdentificacion() { IdTipoIdentificacion = d.Field<string>("SRP_IdTipoIdentificacion") },
                                DocumentoCliente = d.Field<string>("SRP_Identificacion")
                            },
                            PersonaSolicita = d.Field<string>("SOR_PersonaQueSolicita"),
                            Contacto = d.Field<string>("SOR_PersonaContacto"),
                            PesoAproximado = d.Field<decimal>("SOR_PesoAproximado"),
                            NombreCliente = d.Field<string>("SOR_NombreCliente"),
                            LongitudRecogida = d.Field<string>("SOR_Longitud"),
                            LatitudRecogida = d.Field<string>("SOR_Latitud"),
                            VecesNotificadaPush = d["SRN_VecesNotificada"] != DBNull.Value ? d.Field<int>("SRN_VecesNotificada") : 0
                        };

                        if (d["DIM_IdDispositivo"] != DBNull.Value)
                        {
                            r.DispositivoMovil = new PADispositivoMovil()
                            {
                                IdDispositivo = d.Field<long>("DIM_IdDispositivo"),
                                IdCiudad = d.Field<string>("SOR_IdMunicipio"),
                                SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), d.Field<string>("DIM_SistemaOperativo")),
                                TokenDispositivo = d.Field<string>("DIM_TokenDispositivo"),
                                TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), d.Field<string>("DIM_TipoDispositivo"))
                            };
                        }

                        return r;
                    });
            }

        }


        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y ciudad Recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesPorProgramarDia(string idLocalidad)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSolicitudRecogidaPeatonPorProgramar_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (!string.IsNullOrWhiteSpace(idLocalidad))
                {
                    cmd.Parameters.AddWithValue("@IdLocalidadRecogida", idLocalidad);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<OURecogidasDC>(d =>
                {
                    OURecogidasDC r = new OURecogidasDC()
                    {
                        IdRecogida = d.Field<long>("SOR_IdSolicitudRecogida"),
                        CantidadEnvios = d.Field<short>("SOR_CantidadEnvios"),
                        LocalidadRecogida = new PALocalidadDC() { IdLocalidad = d.Field<string>("SOR_IdMunicipio") },
                        FechaRecogida = d.Field<DateTime>("SOR_FechaRecogida"),
                        Direccion = d.Field<string>("SOR_Direccion"),
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC() { IdEstado = d.Field<short>("SOR_IdEstadoSolicitudRecogida") },
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            NombreCliente = d.Field<string>("SRP_Nombre"),
                            TipoIdentificacion = new PATipoIdentificacion() { IdTipoIdentificacion = d.Field<string>("SRP_IdTipoIdentificacion") },
                            DocumentoCliente = d.Field<string>("SRP_Identificacion"),
                            TelefonoCliente = d.Field<string>("SOR_Telefono"),
                            Celular = d.Field<string>("SOR_Celular")

                        },
                        PersonaSolicita = d.Field<string>("SOR_PersonaQueSolicita"),
                        Contacto = d.Field<string>("SOR_PersonaContacto"),
                        PesoAproximado = d.Field<decimal>("SOR_PesoAproximado"),
                        NombreCliente = d.Field<string>("SOR_NombreCliente"),
                        LongitudRecogida = d.Field<string>("SOR_Longitud"),
                        LatitudRecogida = d.Field<string>("SOR_Latitud"),

                    };

                    if (d["DIM_IdDispositivo"] != DBNull.Value)
                    {
                        r.DispositivoMovil = new PADispositivoMovil()
                        {
                            IdDispositivo = d.Field<long>("DIM_IdDispositivo"),
                            IdCiudad = d.Field<string>("SOR_IdMunicipio"),
                            SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), d.Field<string>("DIM_SistemaOperativo")),
                            TokenDispositivo = d.Field<string>("DIM_TokenDispositivo"),
                            TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), d.Field<string>("DIM_TipoDispositivo"))
                        };
                    }

                    return r;
                });
            }

        }

        /// <summary>
        /// Obtiene todas las solicitudes de recogida DISPONIBLES por localidad.
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasDisponiblesPeatonDia(string idLocalidad)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRecogidasDisponiblesPeatonDia_OPU", conn);
                if (!string.IsNullOrWhiteSpace(idLocalidad))
                {
                    cmd.Parameters.AddWithValue("@idLocalidad", idLocalidad);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();


                return dt.AsEnumerable().ToList().ConvertAll<OURecogidasDC>(d =>
                {
                    OURecogidasDC a = new OURecogidasDC()
                    {
                        IdRecogida = d.Field<long>("SOR_IdSolicitudRecogida"),
                        CantidadEnvios = d.Field<short>("SOR_CantidadEnvios"),
                        LocalidadRecogida = new PALocalidadDC()
                        {
                            IdLocalidad = d.Field<string>("SOR_IdMunicipio"),
                            Nombre = d.Field<string>("LOC_Nombre")
                        },
                        FechaRecogida = d.Field<DateTime>("SOR_FechaRecogida"),
                        Direccion = d.Field<string>("SOR_Direccion"),
                        ComplementoDireccion = d.Field<string>("SOR_ComplementoDireccion"),
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC() { IdEstado = d.Field<short>("SOR_IdEstadoSolicitudRecogida") },
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            NombreCliente = d.Field<string>("SRP_Nombre"),
                            TipoIdentificacion = new PATipoIdentificacion() { IdTipoIdentificacion = d.Field<string>("SRP_IdTipoIdentificacion") },
                            DocumentoCliente = d.Field<string>("SRP_Identificacion"),
                            TelefonoCliente = d.Field<string>("SOR_Telefono"),
                            Celular = d.Field<string>("SOR_Celular"),
                            Email = d.Field<string>("SOR_Email")
                        },
                        PersonaSolicita = d.Field<string>("SOR_PersonaQueSolicita"),
                        Contacto = d.Field<string>("SOR_PersonaContacto"),
                        PesoAproximado = d.Field<decimal>("SOR_PesoAproximado"),
                        NombreCliente = d.Field<string>("SOR_NombreCliente"),
                        LongitudRecogida = d.Field<string>("SOR_Longitud"),
                        LatitudRecogida = d.Field<string>("SOR_Latitud"),
                        Observaciones = d.Field<string>("SOR_Observaciones")
                    };

                    if (d["DIM_IdDispositivo"] != DBNull.Value)
                    {
                        a.DispositivoMovil = new PADispositivoMovil()
                        {
                            IdDispositivo = d.Field<long>("DIM_IdDispositivo"),
                            IdCiudad = d.Field<string>("SOR_IdMunicipio"),
                            SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), d.Field<string>("DIM_SistemaOperativo")),
                            TokenDispositivo = d.Field<string>("DIM_TokenDispositivo"),
                            TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), d.Field<string>("DIM_TipoDispositivo"))
                        };
                    }



                    return a;
                });
            }
        }

        /// <summary>
        /// Obtiene todas las recogidas de peaton pendientes por programas
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerTodasRecogidasPeatonPendientesPorProgramar()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTodasSolicitudRecogidasPeatonPorProgramar_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                var lst = dt.AsEnumerable().ToList();

                return lst.GroupBy(r => r.Field<long>("SOR_IdSolicitudRecogida")).Select(s => s.First()).ToList().ConvertAll<OURecogidasDC>(d =>
                {
                    OURecogidasDC r = new OURecogidasDC()
                    {
                        IdRecogida = d.Field<long>("SOR_IdSolicitudRecogida"),
                        CantidadEnvios = d.Field<short>("SOR_CantidadEnvios"),
                        LocalidadRecogida = new PALocalidadDC() { IdLocalidad = d.Field<string>("SOR_IdMunicipio"), Nombre = d.Field<string>("LOC_Nombre") },
                        FechaRecogida = d.Field<DateTime>("SOR_FechaRecogida"),
                        Direccion = d.Field<string>("SOR_Direccion"),
                        ComplementoDireccion = d.Field<string>("SOR_ComplementoDireccion"),
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC() { IdEstado = d.Field<short>("SOR_IdEstadoSolicitudRecogida") },
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            NombreCliente = d.Field<string>("SRP_Nombre"),
                            TipoIdentificacion = new PATipoIdentificacion() { IdTipoIdentificacion = d.Field<string>("SRP_IdTipoIdentificacion") },
                            DocumentoCliente = d.Field<string>("SRP_Identificacion"),
                            TelefonoCliente = d.Field<string>("SOR_Telefono"),
                            Celular = d.Field<string>("SOR_Celular"),
                            Email = d.Field<string>("SOR_Email")
                        },
                        PersonaSolicita = d.Field<string>("SOR_PersonaQueSolicita"),
                        Contacto = d.Field<string>("SOR_PersonaContacto"),
                        PesoAproximado = d.Field<decimal>("SOR_PesoAproximado"),
                        NombreCliente = d.Field<string>("SOR_NombreCliente"),
                        LongitudRecogida = d.Field<string>("SOR_Longitud"),
                        LatitudRecogida = d.Field<string>("SOR_Latitud"),
                        MinutosTranscurridos = d.Field<int>("MinutosTranscurridos")

                    };

                    if (d["SOR_TipoOrigenRecogida"] != DBNull.Value)
                    {
                        r.TipoOrigenRecogida = (OUEnumTipoOrigenRecogida)Enum.Parse(typeof(OUEnumTipoOrigenRecogida), d.Field<string>("SOR_TipoOrigenRecogida"));
                    }
                    else
                    {
                        r.TipoOrigenRecogida = OUEnumTipoOrigenRecogida.WEB;
                    }

                    if (d["DIM_IdDispositivo"] != DBNull.Value)
                    {
                        r.DispositivoMovil = new PADispositivoMovil()
                        {
                            IdDispositivo = d.Field<long>("DIM_IdDispositivo"),
                            IdCiudad = d.Field<string>("SOR_IdMunicipio"),
                            SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), d.Field<string>("DIM_SistemaOperativo")),
                            TokenDispositivo = d.Field<string>("DIM_TokenDispositivo"),
                            TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), d.Field<string>("DIM_TipoDispositivo"))
                        };
                    }

                    if (d["PEI_Identificacion"] != DBNull.Value)
                    {
                        r.AsignacionMensajero = new OUAsignacionRecogidaMensajeroDC()
                        {
                            IdAsignacion = d.Field<long>("SAM_IdAsignacion"),
                            IdentificacionMensajero = d.Field<string>("PEI_Identificacion"),
                            NombreApellidoMensajero = d.Field<string>("PEI_Nombre") + " " + d.Field<string>("PEI_PrimerApellido"),
                            Telefono = d.Field<string>("PEI_Telefono"),
                            Usuario = d.Field<string>("USU_IdUsuario"),
                            PlacaVehiculo = d.Field<string>("VEH_Placa"),
                            TipoVehiculo = d.Field<string>("TIV_Descripcion")
                        };
                    }


                    r.Fotografias = new List<string>();
                    lst.Where(l => l.Field<long>("SOR_IdSolicitudRecogida") == d.Field<long>("SOR_IdSolicitudRecogida")).ToList()
                        .ForEach(l =>
                        {
                            if (d["IMR_RutaImagen"] != DBNull.Value && File.Exists(l.Field<string>("IMR_RutaImagen")))
                            {
                                using (FileStream fs = new FileStream(l.Field<string>("IMR_RutaImagen"), FileMode.Open, FileAccess.Read))
                                {
                                    byte[] filebytes = new byte[fs.Length];
                                    fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                                    string encodedData = Convert.ToBase64String(filebytes, Base64FormattingOptions.None);
                                    r.Fotografias.Add(encodedData);
                                }

                            }
                        });


                    return r;
                });
            }

        }



        /// <summary>
        /// Guarda las notificaciones enviadas por cada recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        public void GuardarNotificacionRecogida(long idSolicitudRecogida)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarRecogidaVecesNotificada_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SRN_IdSolicitudRecogida", idSolicitudRecogida);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }


        #endregion

        #region ObtenerDatosPeaton

        /// <summary>
        /// Obtiene los datos del usuario que está solicitando 
        /// la recogida si ya se ha registrado antes.
        /// </summary>
        /// <param name="tipoid"></param>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public PAPersonaInternaDC ObtenerInfoUsuarioRecogida(string tipoid, string identificacion)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("paObtenerInfoUsuarioSolicitaRecogida_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("TIPOID", tipoid));
                    cmd.Parameters.Add(new SqlParameter("IDENTIFICACION", identificacion));
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    conn.Close();

                    var objPerson = dt.AsEnumerable().FirstOrDefault();

                    if (objPerson != null)
                    {
                        PAPersonaInternaDC pr = new PAPersonaInternaDC()
                        {
                            NombreCompleto = objPerson.Field<string>("Nombre"),
                            Email = objPerson.Field<string>("Email"),
                            Telefono = objPerson.Field<string>("Telefono"),
                            NumeroCelular = objPerson.Field<string>("Celular"),
                            Direccion = objPerson.Field<string>("Direccion"),
                            NombreRegional = objPerson.Field<string>("Ciudad")
                        };

                        return pr;
                    }

                    return new PAPersonaInternaDC();
                }
            }
            catch (SqlException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        #endregion

        #endregion Solicitud Recogidas

        #region Programación Recogidas

        #region Consultas

        /// <summary>
        /// Retorna los motivos de la reprogramacion de la solicitud
        /// </summary>
        /// <returns></returns>
        public List<OUMotivosReprogramacionDC> ObtenerMotivosReprogramacion()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoReProgramacionRecogi_OPU
                  .ToList()
                  .ConvertAll(r => new OUMotivosReprogramacionDC()
                  {
                      IdMotivo = r.MDR_IdMotivoReProgramacionReco,
                      DescripcionMotivo = r.MDR_Descripcion
                  });
            }
        }

        /// <summary>
        /// Retorna la recogida por punto de servicio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPuntoServicio(long idSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OURecogidasDC recogidaPunto = new OURecogidasDC();
                var recogida = contexto.paObtenerSolRecogPuntoSvc_OPU(idSolicitud).FirstOrDefault();

                if (recogida != null)
                {
                    recogidaPunto.IdRecogida = recogida.SOR_IdSolicitudRecogida;
                    recogidaPunto.PuntoServicio = new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC()
                    {
                        IdCentroServicio = recogida.SRP_IdPuntoServicio,
                        Direccion = recogida.SOR_Direccion,
                        Telefono1 = recogida.SOR_Telefono,
                        NombreMunicipio = recogida.LOC_Nombre,
                        Nombre = recogida.CES_Nombre
                    };
                    recogidaPunto.FechaRecogida = recogida.SOR_FechaRecogida;
                    recogidaPunto.FechaSolicitud = recogida.SOR_FechaGrabacion;
                    recogidaPunto.PersonaSolicita = recogida.SOR_PersonaQueSolicita;
                    recogidaPunto.Contacto = recogida.SOR_PersonaContacto;

                    recogidaPunto.CantidadEnvios = recogida.SOR_CantidadEnvios;

                    recogidaPunto.PesoAproximado = recogida.SOR_PesoAproximado;
                    recogidaPunto.Observaciones = recogida.SOR_Observaciones;
                }

                return recogidaPunto;
            }
        }

        /// <summary>
        /// Retorna la recogida del cliente convenio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaConvenio(long idSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OURecogidasDC recogidaConvenio = new OURecogidasDC();

                var recogida = contexto.paObtenerSolRecogConvenio_OPU(idSolicitud).FirstOrDefault();
                if (recogida != null)
                {
                    recogidaConvenio.IdRecogida = recogida.SOR_IdSolicitudRecogida;
                    recogidaConvenio.Cliente = new Servicios.ContratoDatos.Clientes.CLClientesDC()
                    {
                        Nit = recogida.PCL_Nit,
                        RazonSocial = recogida.PCL_RazonSocial,
                    };
                    recogidaConvenio.Sucursal = new Servicios.ContratoDatos.Clientes.CLSucursalDC()
                    {
                        IdSucursal = recogida.PCL_IdSucursal,
                        Nombre = recogida.PCL_NombreSucursal,
                        Telefono = recogida.SOR_Telefono,
                        Direccion = recogida.SOR_Direccion,
                        Localidad = recogida.LOC_Nombre
                    };
                    recogidaConvenio.FechaSolicitud = recogida.SOR_FechaGrabacion;
                    recogidaConvenio.FechaRecogida = recogida.SOR_FechaRecogida;
                    recogidaConvenio.PersonaSolicita = recogida.SOR_PersonaQueSolicita;
                    recogidaConvenio.Contacto = recogida.SOR_PersonaContacto;
                    recogidaConvenio.CantidadEnvios = recogida.SOR_CantidadEnvios;
                    recogidaConvenio.PesoAproximado = recogida.SOR_PesoAproximado;
                    recogidaConvenio.Observaciones = recogida.SOR_Observaciones;
                }

                return recogidaConvenio;
            }
        }

        /// <summary>
        /// Retorna la recogida peaton
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPeaton(long idSolicitud)
        {

            OURecogidasDC peaton = new OURecogidasDC();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerSolRecogPeaton_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var recogida = dt.AsEnumerable().ToList().FirstOrDefault();

                if (recogida == null)
                    return peaton;
                else
                {
                    peaton.IdRecogida = recogida.Field<long>("SOR_IdSolicitudRecogida");
                    peaton.RecogidaPeaton = new OURecogidaPeatonDC();
                    peaton.IdAgenciaResponsable = recogida.Field<long>("SOR_IdAgenciaResponsable");
                    peaton.RecogidaPeaton.TipoIdentificacion = new PATipoIdentificacion();

                    peaton.RecogidaPeaton.TipoIdentificacion.IdTipoIdentificacion = recogida.Field<string>("SRP_IdTipoIdentificacion");
                    peaton.RecogidaPeaton.NombreCliente = recogida.Field<string>("SRP_Nombre");
                    peaton.RecogidaPeaton.DireccionCliente = recogida.Field<string>("SOR_Direccion");
                    peaton.RecogidaPeaton.TelefonoCliente = recogida.Field<string>("SOR_Telefono");
                    peaton.RecogidaPeaton.DocumentoCliente = recogida.Field<string>("SRP_Identificacion");

                    peaton.PersonaSolicita = recogida.Field<string>("SOR_PersonaQueSolicita");
                    peaton.Contacto = recogida.Field<string>("SOR_PersonaContacto");
                    peaton.CantidadEnvios = recogida.Field<short>("SOR_CantidadEnvios");
                    peaton.PesoAproximado = recogida.Field<decimal>("SOR_PesoAproximado");
                    peaton.FechaRecogida = recogida.Field<DateTime>("SOR_FechaRecogida");
                    peaton.FechaSolicitud = recogida.Field<DateTime>("SOR_FechaGrabacion");



                    peaton.RecogidaPeaton.EnviosRecogida = new List<OUEnviosRecogidaPeatonDC>();
                    peaton.RecogidaPeaton.EnviosRecogida =
                      dt.AsEnumerable().ToList().ConvertAll(r => new OUEnviosRecogidaPeatonDC()
                      {
                          CantidadEnvios = r.Field<short>("SRT_Cantidad"),
                          PesoAproximado = r.Field<decimal>("SRT_PesoAproximado"),
                          MunicipioDestino = new PALocalidadDC()
                          {
                              Nombre = r.Field<string>("LocalidadEnvios"),
                              IdLocalidad = r.Field<string>("SRT_IdMunicipioDestino")
                          },
                          TipoEnvio = new Servicios.ContratoDatos.Tarifas.TATipoEnvio()
                          {
                              IdTipoEnvio = r.Field<short>("SRT_IdTipoEnvio"),
                              Nombre = r.Field<string>("SRT_NombreTipoEnvio")
                          }
                      });
                    peaton.Direccion = recogida.Field<string>("SOR_Direccion");
                    peaton.LocalidadRecogida = new PALocalidadDC()
                    {
                        IdLocalidad = recogida.Field<string>("SOR_IdMunicipio")
                    };
                    peaton.Observaciones = recogida.Field<string>("SOR_Observaciones");
                    peaton.EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                    {
                        IdEstado = recogida.Field<short>("SOR_IdEstadoSolicitudRecogida")
                    };


                    /*if (recogida["PSR_IdProgramacionSolicitudRecog"] != null && recogida["PSR_IdProgramacionSolicitudRecog"] != DBNull.Value)
                    {
                        peaton.IdProgramacionSolicitudRecogida = recogida.Field<long>("PSR_IdProgramacionSolicitudRecog");

                    }*/


                }

            }

            return peaton;

        }

        public OUProgramacionSolicitudRecogidaDC ObtenerProgramacionSolicitudRecogida(long idProgramacionSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProgramacionSolicitudRecog_OPU prog = contexto.ProgramacionSolicitudRecog_OPU.Where(p => p.PSR_IdProgramacionSolicitudRecog == idProgramacionSolicitud).FirstOrDefault();
                if (prog == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL)));
                }

                return new OUProgramacionSolicitudRecogidaDC()
                {
                    FechaProgramacion = prog.PSR_UltimaFechaReprogramacion
                };
            }
        }

        /// <summary>
        /// Retorna el historico de la programacion de la recogida
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerProgramacionRecogidas(long idSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerPrograSoliciRecog_OPU(idSolicitud)
                  .ToList()
                  .ConvertAll(r => new OUProgramacionSolicitudRecogidaDC()
                  {
                      IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog,
                      FechaProgramacion = r.fechaProgramacion,
                      FechaReporteMensajero = r.SPP_FechaReporteMensajero == null ? ConstantesFramework.MinDateTimeController : r.SPP_FechaReporteMensajero.Value,
                      MotivoReprogramacion = new OUMotivosReprogramacionDC()
                      {
                          IdMotivo = r.PSR_IdMotivoReProgramacionReco == null ? (short)0 : r.PSR_IdMotivoReProgramacionReco.Value,
                          DescripcionMotivo = r.PSR_DescripcionMotivo
                      },
                      ReportadoMensajero = r.SPP_EstaReportadaMensajero == null ? false : r.SPP_EstaReportadaMensajero.Value,
                      Recogida = new OURecogidasDC()
                      {
                          IdRecogida = r.SOR_IdSolicitudRecogida,
                          FechaRecogida = r.SOR_FechaRecogida,
                          PersonaRecepcionoRecogida = r.SOR_NombrePersonaCreaSolicitud,
                          IdAgenciaResponsable = r.SOR_IdAgenciaResponsable,
                          EstaReportada = r.SPP_EstaReportadaMensajero == null ? false : r.SPP_EstaReportadaMensajero.Value
                      },
                      MensajeroPlanilla = new OUNombresMensajeroDC()
                      {
                          Identificacion = r.SPP_IdentificacionMensajero == null ? "Sin Planillar" : r.SPP_IdentificacionMensajero,
                          NombreApellido = r.SPP_NombreMensajero == null ? "Sin Planillar" : r.SPP_NombreMensajero,
                          IdPersonaInterna = r.SPP_IdMensajero == null ? 0 : r.SPP_IdMensajero.Value
                      },
                      TipoMensajero = new OUTipoMensajeroDC()
                      {
                          IdTipoMensajero = r.PSR_IdTipoMensajero,
                          Descripcion = r.PSR_DescripcionTipoMensajero
                      },
                      Estado = r.PSR_Estado,
                      FechaCreacion = r.fechaProgramacion,
                      CreadoPor = r.SPP_CreadoPor,
                      IdPlanillaRecogida = r.SPP_IdPlanillaSolicitudRecogid == null ? 0 : r.SPP_IdPlanillaSolicitudRecogid.Value
                  });
            }
        }

        /// <summary>
        /// Obtiene las planillas de recogidas creadas para el tipo de mensajero, zona y fecha de recogidas
        /// </summary>
        /// <param name="idZona">id de la zona</param>
        /// <param name="idTipoMensajero">id del tipo de mensajero</param>
        /// <param name="fechaRecogida">fecha de recogida</param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidaZonaTipoMenFecha(string idZona, int idTipoMensajero, DateTime fechaRecogida, long idCol)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerPlanillaSoliReco_OPU(idTipoMensajero, idZona, fechaRecogida.Date, idCol)
                  .ToList()
                  .ConvertAll(r => new OUProgramacionSolicitudRecogidaDC()
                  {
                      IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog,
                      IdPlanillaRecogida = r.SPP_IdPlanillaSolicitudRecogid,
                      MensajeroPlanilla = new OUNombresMensajeroDC()
                      {
                          Identificacion = r.SPP_IdentificacionMensajero,
                          NombreApellido = r.SPP_NombreMensajero,
                          IdPersonaInterna = r.SPP_IdMensajero
                      },
                      TipoMensajero = new OUTipoMensajeroDC()
                      {
                          IdTipoMensajero = r.PSR_IdTipoMensajero,
                          Descripcion = r.PSR_DescripcionTipoMensajero
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene los mensajero por centro logistico y tipo
        /// </summary>
        /// <param name="idCentroLogistico"></param>
        /// <param name="idTipoMensajero"></param>
        /// <returns></returns>
        public List<OUNombresMensajeroDC> ObtenerMensajerosPorTipo(long idCentroLogistico, int idTipoMensajero)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MensajerosAgenciaCol_VOPU.Where(r => r.AGE_IdCentroLogistico == idCentroLogistico && r.MEN_IdTipoMensajero == idTipoMensajero).
                  ToList().
                  ConvertAll(r => new OUNombresMensajeroDC()
                  {
                      NombreApellido = r.NombreCompleto,
                      IdPersonaInterna = r.PEI_IdPersonaInterna,
                      Identificacion = r.PEI_Identificacion,
                      IdMensajero = r.MEN_IdMensajero
                  });
            }
        }

        #endregion Consultas

        #region insert

        /// <summary>
        /// Guarda la programacion de la solicitud de la recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void GuardaProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCrearProgramaSoliciReco_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idMotivoReprogramacion", programacion.MotivoReprogramacion.IdMotivo);
                cmd.Parameters.AddWithValue("@descripcionMotivo", programacion.MotivoReprogramacion.DescripcionMotivo);
                cmd.Parameters.AddWithValue("@idZona", programacion.Zona.IdZona);
                cmd.Parameters.AddWithValue("@descripcionZona", programacion.Zona.Descripcion);
                cmd.Parameters.AddWithValue("@idTipoMensajero", programacion.TipoMensajero.IdTipoMensajero);
                cmd.Parameters.AddWithValue("@idDescripcionTipo", programacion.TipoMensajero.Descripcion);
                cmd.Parameters.AddWithValue("@estado", programacion.Estado);
                cmd.Parameters.AddWithValue("@fechaProgramacion", programacion.FechaProgramacion.Date);
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@tipoCambio", EnumEstadoRegistro.ADICIONADO.ToString());
                cmd.Parameters.AddWithValue("@EsProgramacion", true);
                cmd.Parameters.AddWithValue("@IdSolicitudRecogida", programacion.Recogida.IdRecogida);
                cmd.Parameters.AddWithValue("@FechaDescarga", DateTime.Now);
                cmd.Parameters.AddWithValue("@EstaPlanillada", false);
                cmd.Parameters.AddWithValue("@EstaDescargada", false);
                conn.Open();
                programacion.IdProgramacionSolicitudRecogida = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
            }
        }

        public void GuardarHistoricoProgramacionRecogidas(OUProgramacionSolicitudRecogidaDC programacion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paCrearHistoricoProgramaSoliciReco_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSolicitud", programacion.Recogida.IdRecogida);
                cmd.Parameters.AddWithValue("@idMotivoReprogramacion", programacion.MotivoReprogramacion.IdMotivo);
                cmd.Parameters.AddWithValue("@descripcionMotivo", programacion.MotivoReprogramacion.DescripcionMotivo);
                cmd.Parameters.AddWithValue("@idZona", programacion.Zona.IdZona);
                cmd.Parameters.AddWithValue("@descripcionZona", programacion.Zona.Descripcion);
                cmd.Parameters.AddWithValue("@idTipoMensajero", programacion.TipoMensajero.IdTipoMensajero);
                cmd.Parameters.AddWithValue("@idDescripcionTipo", programacion.TipoMensajero.Descripcion);
                cmd.Parameters.AddWithValue("@estado", programacion.Estado);
                cmd.Parameters.AddWithValue("@fechaProgramacion", programacion.FechaProgramacion.Date);
                cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@tipoCambio", EnumEstadoRegistro.ADICIONADO.ToString());
                cmd.Parameters.AddWithValue("@EsProgramacion", false);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }



            /*  using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
              {
                  contexto.paCrearHistoricoProgramaSoliciReco_OPU(programacion.Recogida.IdRecogida
                    , programacion.MotivoReprogramacion.IdMotivo
                    , programacion.MotivoReprogramacion.DescripcionMotivo
                    , programacion.Zona.IdZona
                    , programacion.Zona.Descripcion
                    , programacion.TipoMensajero.IdTipoMensajero
                    , programacion.TipoMensajero.Descripcion
                    , programacion.Estado
                    , programacion.FechaProgramacion.Date
                    , DateTime.Now
                    , ControllerContext.Current.Usuario
                    , EnumEstadoRegistro.ADICIONADO.ToString(), false);

                  contexto.SaveChanges();
              }*/
        }

        /// <summary>
        /// Guarda la planilla de la solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void GuardarSolicitudPlanillaRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {


            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerSolicitudProgramadaPlanilla_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SPP_IdSolicitudProgramadaPlanill", programacion.IdProgramacionSolicitudRecogida);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();
                if (dt.AsEnumerable().ToList().Count() <= 0)
                {



                    cmd = new SqlCommand("paCrearPlanillaSolicRecogi_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idProgramacionSolicitud", programacion.IdProgramacionSolicitudRecogida);
                    cmd.Parameters.AddWithValue("@idPlanillaSolicitud", programacion.IdPlanillaRecogida);
                    cmd.Parameters.AddWithValue("@estaReportada", programacion.Recogida.EstaReportada);
                    cmd.Parameters.AddWithValue("@fechaReporte", ConstantesFramework.MinDateTimeController);
                    cmd.Parameters.AddWithValue("@idAgenciaResponsable", programacion.Recogida.IdAgenciaResponsable);
                    cmd.Parameters.AddWithValue("@idMensajero", programacion.MensajeroPlanilla.IdMensajero);
                    cmd.Parameters.AddWithValue("@nombreMensajero", programacion.MensajeroPlanilla.NombreApellido);
                    cmd.Parameters.AddWithValue("@identificacion", programacion.MensajeroPlanilla.Identificacion);
                    cmd.Parameters.AddWithValue("@fechaGrabacion", DateTime.Now);
                    cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@cambiadoPor", ControllerContext.Current.Usuario);
                    cmd.Parameters.AddWithValue("@tipoCambio", EnumEstadoRegistro.ADICIONADO.ToString());

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                else
                {

                    cmd = new SqlCommand("paActualizarMesajeroSolicitudProgramadaPlanilla_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SPP_IdSolicitudProgramadaPlanill", programacion.IdProgramacionSolicitudRecogida);
                    cmd.Parameters.AddWithValue("@SPP_IdMensajero", programacion.MensajeroPlanilla.IdMensajero);
                    cmd.Parameters.AddWithValue("@SPP_NombreMensajero", programacion.MensajeroPlanilla.NombreApellido);
                    cmd.Parameters.AddWithValue("@SPP_IdentificacionMensajero", programacion.MensajeroPlanilla.Identificacion);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                }
            }



            /*

            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    SolicitudProgramadaPlanill_OPU soli = contexto.SolicitudProgramadaPlanill_OPU.Where(s => s.SPP_IdSolicitudProgramadaPlanill == programacion.IdProgramacionSolicitudRecogida).FirstOrDefault();

                    if (soli == null)
                    {
                        contexto.paCrearPlanillaSolicRecogi_OPU(programacion.IdProgramacionSolicitudRecogida
                          , programacion.IdPlanillaRecogida
                          , programacion.Recogida.EstaReportada
                          , ConstantesFramework.MinDateTimeController
                          , programacion.Recogida.IdAgenciaResponsable
                          , programacion.MensajeroPlanilla.IdPersonaInterna
                          , programacion.MensajeroPlanilla.NombreApellido
                          , programacion.MensajeroPlanilla.Identificacion
                          , DateTime.Now
                          , ControllerContext.Current.Usuario
                          , ControllerContext.Current.Usuario
                          , EnumEstadoRegistro.ADICIONADO.ToString());
                    }
                    else
                    {
                        soli.SPP_IdentificacionMensajero = programacion.MensajeroPlanilla.Identificacion;
                        soli.SPP_NombreMensajero = programacion.MensajeroPlanilla.NombreApellido;
                        soli.SPP_IdMensajero = programacion.MensajeroPlanilla.IdPersonaInterna;
                        contexto.SaveChanges();
                    }

                    scope.Complete();
                }
            }*/
        }

        /// <summary>
        /// Actualiza la programacion de la solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    contexto.paActualizarPrograRecogi_OPU(programacion.IdProgramacionSolicitudRecogida
                      , programacion.MotivoReprogramacion.IdMotivo
                      , programacion.MotivoReprogramacion.DescripcionMotivo
                      , programacion.Zona.IdZona
                      , programacion.Zona.Descripcion
                      , programacion.TipoMensajero.IdTipoMensajero
                      , programacion.TipoMensajero.Descripcion
                      , programacion.Estado
                      , programacion.FechaProgramacion
                      , ControllerContext.Current.Usuario
                      , EnumEstadoRegistro.MODIFICADO.ToString(),
                      programacion.Recogida.IdRecogida,
                      DateTime.Now,
                      programacion.EstaPlanillada,
                      programacion.EstaDescargada);

                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// Agrega una programacion a una solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        public long AgregarProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarProgramacionSolicitudRecogida_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PSR_IdZona", programacion.Zona.IdZona);
                cmd.Parameters.AddWithValue("@PSR_DescripcionZona", programacion.Zona.Descripcion);
                cmd.Parameters.AddWithValue("@PSR_IdTipoMensajero", programacion.TipoMensajero.IdTipoMensajero);
                cmd.Parameters.AddWithValue("@PSR_DescripcionTipoMensajero", programacion.TipoMensajero.Descripcion);
                cmd.Parameters.AddWithValue("@PSR_Estado", "PLA");
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@PSR_IdSolicitudRecogida", programacion.Recogida.IdRecogida);
                cmd.Parameters.AddWithValue("@IdPlanillaSolicitudRecogid", programacion.IdPlanillaRecogida);
                cmd.Parameters.AddWithValue("@SPP_IdAgenciaResponsable", programacion.Recogida.IdAgenciaResponsable);
                cmd.Parameters.AddWithValue("@SPP_IdMensajero", programacion.MensajeroPlanilla.IdMensajero);
                cmd.Parameters.AddWithValue("@SPP_NombreMensajero", programacion.MensajeroPlanilla.NombreApellido);
                cmd.Parameters.AddWithValue("@SPP_IdentificacionMensajero", programacion.MensajeroPlanilla.Identificacion);
                conn.Open();
                long idProgramacion = Convert.ToInt64(cmd.ExecuteScalar());
                conn.Close();
                return idProgramacion;
            }
        }


        /// <summary>
        /// Asigna una solicitud de recogida a un mensajero
        /// </summary>
        /// <returns></returns>
        public long AsignarRecogidaMensajero(OUAsignacionRecogidaMensajeroDC asignacion)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                using (SqlConnection conn = new SqlConnection(conexionStringController))
                {
                    SqlCommand cmd = new SqlCommand("paInsertarRecogidaAsignacionMensajero_OPU", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SAM_IdMensajero", asignacion.IdMensajero);
                    cmd.Parameters.AddWithValue("@SAM_IdSolicitudRecogida", asignacion.IdSolicitudRecogida);
                    cmd.Parameters.AddWithValue("@SAM_GrabadoPor", ControllerContext.Current.Usuario);
                    conn.Open();
                    long idAsignacion = Convert.ToInt64(cmd.ExecuteScalar());
                    conn.Close();
                    ActualizarEstadoSolicitudRecogida(asignacion.IdSolicitudRecogida, OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA);
                    trans.Complete();
                    return idAsignacion;
                }
            }
        }

        /// <summary>
        /// Actualiza el estado de la solicitud de uan recogida e inserta el estado traza
        /// </summary>
        public void ActualizarEstadoSolicitudRecogida(long idRecogida, OUEnumEstadoSolicitudRecogidas estado)
        {
            ///actualiza el estado de la solicitud de recogida e inserta en la tabla de estado recogida traza 
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarEstadoSolicitudRecogida_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SOR_IdEstadoSolicitudRecogida", (int)estado);
                cmd.Parameters.AddWithValue("@SOR_IdSolicitudRecogida", idRecogida);
                cmd.Parameters.AddWithValue("@ERT_CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }



        /// <summary>
        /// Actualiza la georeferenciacion de una recogida
        /// </summary>
        /// <param name="longitud"></param>
        /// <param name="latitud"></param>
        /// <param name="idRecogida"></param>
        public void ActualizarGeoreferenciacionRecogida(string longitud, string latitud, long idRecogida)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizarGeoreferenciacionRecogida_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SOR_Longitud", longitud);
                cmd.Parameters.AddWithValue("@SOR_Latitud", latitud);
                cmd.Parameters.AddWithValue("@SOR_IdSolicitudRecogida", idRecogida);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }


        /// <summary>
        /// Actualiza la planilla de la solicitud de la recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizarSolicitudPlanillaRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    contexto.paActualizarPlanSolicRecog_OPU(programacion.Recogida.IdRecogida
                      , programacion.IdPlanillaRecogida
                      , programacion.Recogida.EstaReportada

                      ///Como es una reprogramacion no ha sido reportada a mensajero
                      , ConstantesFramework.MinDateTimeController
                      , programacion.Recogida.IdAgenciaResponsable
                      , programacion.MensajeroPlanilla.IdPersonaInterna
                      , programacion.MensajeroPlanilla.NombreApellido
                      , programacion.MensajeroPlanilla.Identificacion
                      , ControllerContext.Current.Usuario
                      , EnumEstadoRegistro.MODIFICADO.ToString());

                    scope.Complete();
                }
            }
        }

        /// <summary>
        /// Actualiza la solicitud de la recogida
        /// </summary>
        /// <param name="estaPlanillada"></param>
        /// <param name="estado"></param>
        /// <param name="idSolicitud"></param>
        public void ActualizaEstadoSolicitudRecogida(short estadoSolicitud, long idSolicitud)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizaEstadoSoliRecog_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idEstadoSolicitud", estadoSolicitud);
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }


            //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paActualizaEstadoSoliRecog_OPU(estadoSolicitud, idSolicitud);
            //}
        }

        /// <summary>
        /// Actualiza la solicitud de la recogida
        /// </summary>
        /// <param name="estaPlanillada"></param>
        /// <param name="estado"></param>
        /// <param name="idSolicitud"></param>
        public void ActualizaEstadoProgramacionSolicitudRecogida(bool estaPlanillada, string estadoProgramacion, long idProgramacionSolicitud)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paActualizaEstadoProgramacionSoliRecog_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@estadoProgramacion", estadoProgramacion);
                cmd.Parameters.AddWithValue("@estaPlanillada", estaPlanillada);
                cmd.Parameters.AddWithValue("@IdProgramacionSolicitud", idProgramacionSolicitud);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }


            //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paActualizaEstadoProgramacionSoliRecog_OPU(estadoProgramacion
            //      , estaPlanillada, idProgramacionSolicitud);
            //}
        }

        /// <summary>
        /// Actualiza el reporte de la recogida al mensajero
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaReporteMensajero(OUProgramacionSolicitudRecogidaDC programacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarProgRecoRepor_OPU(programacion.IdProgramacionSolicitudRecogida
                  , programacion.IdPlanillaRecogida
                  , programacion.Recogida.EstaReportada
                  , DateTime.Now
                  , programacion.Recogida.IdAgenciaResponsable
                  , programacion.MensajeroPlanilla.IdPersonaInterna
                  , programacion.MensajeroPlanilla.NombreApellido
                  , programacion.MensajeroPlanilla.Identificacion
                  , programacion.FechaCreacion
                  , programacion.CreadoPor
                  , ControllerContext.Current.Usuario
                  , EnumEstadoRegistro.MODIFICADO.ToString());
            }
        }

        #endregion insert

        #region Eliminacion

        public void EliminarSolicitudPlanillaRecogida(long idSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    contexto.paEliminarSoliPlaniRecogi_OPU(ControllerContext.Current.Usuario
                      , EnumEstadoRegistro.BORRADO.ToString()
                      , idSolicitud);

                    scope.Complete();
                }
            }
        }

        #endregion Eliminacion

        #region Planilla de recogidas

        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasMensajerosDia(long idMensajero)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRecogidasMensajerosDia_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SPP_IdMensajero", idMensajero);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<OURecogidasDC>(r =>
                {
                    OURecogidasDC recogida = new OURecogidasDC()
                    {
                        Direccion = r.Field<string>("SOR_Direccion"),
                        ComplementoDireccion = r.Field<string>("SOR_ComplementoDireccion"),
                        Observaciones = r.Field<string>("SOR_Observaciones"),
                        PersonaSolicita = r.Field<string>("SOR_PersonaQueSolicita"),
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                        {
                            IdEstado = r.Field<short>("SOR_IdEstadoSolicitudRecogida"),
                            Descripcion = r.Field<string>("EPS_Descripcion")
                        },
                        IdRecogida = r.Field<long>("SOR_IdSolicitudRecogida"),
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            NombreCliente = r.Field<string>("SRP_Nombre"),
                            DireccionCliente = r.Field<string>("SOR_Direccion"),
                            TelefonoCliente = r.Field<string>("SOR_Telefono"),
                            Celular = r.Field<string>("SOR_Celular"),
                            DocumentoCliente = r.Field<string>("SRP_Identificacion")

                        },
                        FechaRecogida = r.Field<DateTime>("SOR_FechaRecogida"),
                        LongitudRecogida = r.Field<string>("SOR_Longitud"),
                        LatitudRecogida = r.Field<string>("SOR_Latitud"),
                        NombreTipoEnvio = r.Field<string>("SRT_NombreTipoEnvio")

                    };


                    if (r["SOR_Celular"] != null && r["SOR_Celular"] != DBNull.Value)
                    {
                        recogida.RecogidaPeaton.Celular = r.Field<string>("SOR_Celular");
                    }

                    if (r["SAM_IdAsignacion"] != null && r["SAM_IdAsignacion"] != DBNull.Value)
                    {
                        recogida.AsignacionMensajero = new OUAsignacionRecogidaMensajeroDC()
                        {
                            IdAsignacion = r.Field<long>("SAM_IdAsignacion")
                        };
                    }

                    if (r["SRD_IdMotivoDescargueSolicitud"] != null && r["SRD_IdMotivoDescargueSolicitud"] != DBNull.Value)
                    {
                        if (r.Field<short>("SRD_IdMotivoDescargueSolicitud") > 0)
                        {
                            recogida.MotivoDescargue = new OUMotivoDescargueRecogidasDC()
                            {
                                DescripcionMotivo = r.Field<string>("MOD_Descripcion"),
                                IdMotivo = r.Field<short>("SRD_IdMotivoDescargueSolicitud"),
                                VisibleMensajero = Convert.ToBoolean(r.Field<int>("MOD_VisibleMensajero"))
                            };
                        }

                    }

                    return recogida;
                });
            }
        }

        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasClienteMovilDia(string tokenDispositivo)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRecogidasClientesMovilesDia_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DIM_TokenDispositivo", tokenDispositivo);

                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<OURecogidasDC>(r =>
                {
                    OURecogidasDC recogida = new OURecogidasDC()
                    {
                        Direccion = r.Field<string>("SOR_Direccion"),
                        PersonaSolicita = r.Field<string>("SOR_PersonaQueSolicita"),
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                        {
                            IdEstado = r.Field<short>("SOR_IdEstadoSolicitudRecogida"),
                            Descripcion = r.Field<string>("EPS_Descripcion"),
                            DescripcionClienteMovil = r.Field<string>("DescripcionEstadoCliente")
                        },
                        IdRecogida = r.Field<long>("SOR_IdSolicitudRecogida"),
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            NombreCliente = r.Field<string>("SRP_Nombre"),
                            DireccionCliente = r.Field<string>("SOR_Direccion"),
                            TelefonoCliente = r.Field<string>("SOR_Telefono"),
                        },
                        FechaRecogida = r.Field<DateTime>("SOR_FechaRecogida")
                    };

                    if (r["SAM_IdAsignacion"] != null && r["SAM_IdAsignacion"] != DBNull.Value)
                    {
                        if (r.Field<long>("SAM_IdAsignacion") > 0)
                        {

                            recogida.AsignacionMensajero = new OUAsignacionRecogidaMensajeroDC()
                            {
                                IdAsignacion = r.Field<long>("SAM_IdAsignacion"),
                                IdMensajero = r.Field<long>("SAM_IdMensajero"),
                                NombreApellidoMensajero = r.Field<string>("SPP_NombreMensajero"),
                                IdentificacionMensajero = r.Field<string>("SPP_IdentificacionMensajero")

                            };
                        }
                    }

                    if (r["SRD_IdMotivoDescargueSolicitud"] != null && r["SRD_IdMotivoDescargueSolicitud"] != DBNull.Value)
                    {
                        if (r.Field<int>("SRD_IdMotivoDescargueSolicitud") > 0)
                        {
                            recogida.MotivoDescargue = new OUMotivoDescargueRecogidasDC()
                            {
                                DescripcionMotivo = r.Field<string>("MOD_Descripcion"),
                                IdMotivo = r.Field<int>("SRD_IdMotivoDescargueSolicitud"),
                                VisibleMensajero = Convert.ToBoolean(r.Field<int>("MOD_VisibleMensajero")),
                                PermiteReprogramar = Convert.ToBoolean(r.Field<int>("MOD_PermiteReprogramar"))
                            };
                        }

                    }

                    return recogida;
                });
            }
        }



        /// <summary>
        /// Selecciona todas las recogidas vencidas que fueron asignadas a los usuarioMensajero (usuarios PAM ) en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasVencidasMensajerosPAMDia()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerSolRecogPeatonPlanilladasVencidas_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<OURecogidasDC>(r =>
                {
                    OURecogidasDC recogida = new OURecogidasDC()
                    {
                        IdRecogida = r.Field<long>("SOR_IdSolicitudRecogida"),
                        RecogidaPeaton = new OURecogidaPeatonDC()
                        {
                            NombreCliente = r.Field<string>("SRP_Nombre"),
                            DireccionCliente = r.Field<string>("SOR_Direccion"),
                            TelefonoCliente = r.Field<string>("SOR_Telefono"),
                        },
                        FechaRecogida = r.Field<DateTime>("SOR_FechaRecogida")
                    };
                    recogida.LocalidadRecogida = new PALocalidadDC()
                    {
                        IdLocalidad = r.Field<string>("SOR_IdMunicipio")
                    };

                    recogida.AsignacionMensajero = new OUAsignacionRecogidaMensajeroDC()
                    {
                        IdAsignacion = r.Field<long>("SAM_IdAsignacion")
                    };
                    recogida.MensajeroPlanilla = new OUNombresMensajeroDC()
                    {
                        IdMensajero = r.Field<long>("SAM_IdMensajero"),
                        NombreApellido = r.Field<string>("USU_IdUsuario")
                    };
                    return recogida;
                });
            }
        }

        /// <summary>
        /// Obtiene las planillas de recogidas por centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCol"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidas(Dictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCol, bool incluyeFecha)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaPlanilla;
                string idPlanilla;
                string cedula;
                string idMensajero;
                string idTipoMensajero;
                string idZona;
                string fecha;

                filtro.TryGetValue("PEI_Identificacion", out cedula);
                filtro.TryGetValue("SPP_IdPlanillaSolicitudRecogid", out idPlanilla);
                filtro.TryGetValue("SPP_IdMensajero", out idMensajero);
                filtro.TryGetValue("PSR_IdZona", out idZona);
                filtro.TryGetValue("PSR_IdTipoMensajero", out idTipoMensajero);
                filtro.TryGetValue("PSR_UltimaFechaReprogramacion", out fecha);

                fechaPlanilla = Convert.ToDateTime(fecha, new CultureInfo("es-CO"));
                if (indicePagina == 0)
                    indicePagina = 1;

                return contexto.paObtenerPlaniSoliRecoCOL_OPU(idCol
                         , Convert.ToInt64(idMensajero)
                         , Convert.ToInt64(idPlanilla)
                         , idZona
                         , Convert.ToInt16(idTipoMensajero)
                         , indicePagina
                         , registrosPorPagina
                         , incluyeFecha
                         , fechaPlanilla
                         , cedula).OrderByDescending(r => r.SPP_IdPlanillaSolicitudRecogid)
                   .ToList()
                   .ConvertAll(r => new OUProgramacionSolicitudRecogidaDC()
                   {
                       EstaDescargada = r.PSR_EstaDescargada,
                       EstaPlanillada = r.PSR_EstaPlanillada.Value,
                       FechaDescarga = r.PSR_FechaDescarga,
                       IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog,
                       IdPlanillaRecogida = r.SPP_IdPlanillaSolicitudRecogid,
                       MensajeroPlanilla = new OUNombresMensajeroDC()
                       {
                           Identificacion = r.SPP_IdentificacionMensajero,
                           IdPersonaInterna = r.SPP_IdMensajero,
                           NombreApellido = r.SPP_NombreMensajero
                       },
                       TipoMensajero = new OUTipoMensajeroDC()
                       {
                           IdTipoMensajero = r.PSR_IdTipoMensajero,
                           Descripcion = r.PSR_DescripcionTipoMensajero
                       },
                       Zona = new PAZonaDC()
                       {
                           IdZona = r.PSR_IdZona,
                           Descripcion = r.PSR_DescripcionZona
                       },
                       FechaProgramacion = r.fecha.Value
                   });
            }
        }

        /// <summary>
        /// Obtiene la programacion de la recogida esporadica sin planillar
        /// </summary>
        /// <param name="idZona"></param>
        /// <param name="idTipoMensajero"></param>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerProgramacionRecogidasSinPlanillar(string idZona, short idTipoMensajero, long idCol, DateTime fechaRecogidas)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerPrograSolicSinPla_OPU(idZona, idTipoMensajero, idCol, fechaRecogidas.Date)
                  .ToList()
                  .ConvertAll(r => new OURecogidasDC()
                  {
                      IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog,
                      IdRecogida = r.SOR_IdSolicitudRecogida,
                      IdTipoRecogida = r.SOR_TipoRecogida,
                      EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                      {
                          IdEstado = r.SOR_IdEstadoSolicitudRecogida,
                      },
                      FechaSolicitud = r.SOR_FechaGrabacion,
                      Direccion = r.SOR_Direccion,
                      Zona = new PAZonaDC()
                      {
                          IdZona = r.PSR_IdZona,
                          Descripcion = r.PSR_DescripcionZona
                      },
                      FechaRecogida = r.PSR_UltimaFechaReprogramacion,
                      CodigoPuntoCliente = r.idPuntoOCliente.ToString(),
                      IdAgenciaResponsable = r.SOR_IdAgenciaResponsable
                  });
            }
        }

        /// <summary>
        /// Obtiene la planilla de programacion de recogidas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public OUProgramacionSolicitudRecogidaDC ObtenerPlanillaRecogida(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OUProgramacionSolicitudRecogidaDC planillaInfo = new OUProgramacionSolicitudRecogidaDC();
                var planilla = contexto.paObtenerPlanilSoliciRecog_OPU(idPlanilla).ToList();

                if (planilla != null && planilla.Count > 0)
                {
                    planillaInfo.IdProgramacionSolicitudRecogida = planilla.First().PSR_IdProgramacionSolicitudRecog;
                    planillaInfo.IdPlanillaRecogida = planilla.First().SPP_IdPlanillaSolicitudRecogid;
                    planillaInfo.FechaProgramacion = planilla.FirstOrDefault().PSR_UltimaFechaReprogramacion;
                    planillaInfo.TipoMensajero = new OUTipoMensajeroDC()
                    {
                        IdTipoMensajero = planilla.First().PSR_IdTipoMensajero,
                        Descripcion = planilla.First().PSR_DescripcionTipoMensajero
                    };
                    planillaInfo.Zona = new PAZonaDC()
                    {
                        IdZona = planilla.First().PSR_IdZona,
                        Descripcion = planilla.First().PSR_DescripcionZona
                    };
                    planillaInfo.MensajeroPlanilla = new OUNombresMensajeroDC()
                    {
                        Identificacion = planilla.First().SPP_IdentificacionMensajero,
                        NombreApellido = planilla.First().SPP_NombreMensajero,
                        IdPersonaInterna = planilla.First().SPP_IdMensajero
                    };

                    planillaInfo.RecogidasPlanilla = planilla.ToList().ConvertAll(r => new OURecogidasDC()
                    {
                        IdRecogida = r.SOR_IdSolicitudRecogida,
                        Direccion = r.SOR_Direccion,
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                        {
                            IdEstado = r.SOR_IdEstadoSolicitudRecogida,
                            Descripcion = r.EPS_Descripcion
                        },
                        FechaSolicitud = r.SOR_FechaGrabacion,
                        CodigoPuntoCliente = r.IdCliente.ToString(),
                        IdAgenciaResponsable = r.SOR_IdAgenciaResponsable,
                        IdTipoRecogida = r.SOR_TipoRecogida,
                        Zona = new PAZonaDC()
                        {
                            IdZona = r.PSR_IdZona,
                            Descripcion = r.PSR_DescripcionZona
                        }
                    });
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_NO_EXISTE_PLANILLA.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_NO_EXISTE_PLANILLA), idPlanilla)));

                return planillaInfo;
            }
        }

        #endregion Planilla de recogidas

        #endregion Programación Recogidas

        #region Recogida Fija

        public void GuardarRecogidaFija()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearSolicitRecogidaFija_OPU();
            }
        }

        #endregion Recogida Fija

        #region Descargue de recogidas

        #region Consulta

        /// <summary>
        /// Retorna los motivos de descargue de recogidas
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoDescargueRecogidasDC> ObtenerMotivosDescargueRecogidas()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoDescargueSolicitud_OPU.Where(r => r.MOD_Estado == OUConstantesOperacionUrbana.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new OUMotivoDescargueRecogidasDC()
                  {
                      DescripcionMotivo = r.MOD_Descripcion,
                      IdMotivo = r.MOD_IdMotivoDescargue,
                      PermiteReprogramar = r.MOD_PermiteReprogramar
                  });
            }
        }


        /// <summary>
        /// Retorna los motivos de descargue de recogidas filtrado por id motivo
        /// </summary>
        /// <returns></returns>
        public OUMotivoDescargueRecogidasDC ObtenerMotivosDescargueRecogidasIdMotivo(int idMotivo)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                var motivo = contexto.MotivoDescargueSolicitud_OPU.Where(r => r.MOD_IdMotivoDescargue == idMotivo && r.MOD_Estado == OUConstantesOperacionUrbana.ESTADO_ACTIVO)
                  .ToList().FirstOrDefault();

                if (motivo == null)
                    return null;

                return new OUMotivoDescargueRecogidasDC()
                {
                    DescripcionMotivo = motivo.MOD_Descripcion,
                    IdMotivo = motivo.MOD_IdMotivoDescargue,
                    PermiteReprogramar = motivo.MOD_PermiteReprogramar
                };
            }
        }

        /// <summary>
        /// Obtiene las recogidas de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPlanilla(long idPlanilla)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<OURecogidasDC> recogidas = new List<OURecogidasDC>();
                var planilla = contexto.paObtenerPlanilSoliciRecog_OPU(idPlanilla);

                if (planilla != null)
                {
                    recogidas = planilla.ToList().ConvertAll(r => new OURecogidasDC()
                    {
                        IdProgramacionSolicitudRecogida = r.PSR_IdProgramacionSolicitudRecog,
                        IdRecogida = r.SOR_IdSolicitudRecogida,
                        Direccion = r.SOR_Direccion,
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                        {
                            IdEstado = r.SOR_IdEstadoSolicitudRecogida,
                            Descripcion = r.EPS_Descripcion
                        },
                        FechaSolicitud = r.SOR_FechaGrabacion,
                        CodigoPuntoCliente = r.IdCliente.ToString(),
                        IdAgenciaResponsable = r.SOR_IdAgenciaResponsable,
                        IdTipoRecogida = r.SOR_TipoRecogida,
                        Zona = new PAZonaDC()
                        {
                            IdZona = r.PSR_IdZona,
                            Descripcion = r.PSR_DescripcionZona
                        },
                        FechaRecogida = r.PSR_UltimaFechaReprogramacion,
                        TipoMensajero = new OUTipoMensajeroDC()
                        {
                            IdTipoMensajero = r.PSR_IdTipoMensajero,
                            Descripcion = r.PSR_DescripcionTipoMensajero
                        },
                        MensajeroPlanilla = new OUNombresMensajeroDC()
                        {
                            NombreApellido = r.SPP_NombreMensajero,
                            Identificacion = r.SPP_IdentificacionMensajero,
                            IdPersonaInterna = r.SPP_IdMensajero
                        }
                    });
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_NO_EXISTE_PLANILLA.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_NO_EXISTE_PLANILLA), idPlanilla)));

                return recogidas;
            }
        }

        /// <summary>
        /// Consulta la solicitud de recogida por id de solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerDescargueRecogidaPorSolicitud(long idSolicitud)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OURecogidasDC recogida = new OURecogidasDC();
                var recogidaDescargada = contexto.paObtenerDescargaSolicReco_OPU(idSolicitud)
                 .FirstOrDefault();

                if (recogidaDescargada != null)
                {
                    recogida.IdRecogida = recogidaDescargada.SOR_IdSolicitudRecogida;
                    recogida.IdTipoRecogida = recogidaDescargada.SOR_TipoRecogida;
                    recogida.IdAgenciaResponsable = recogidaDescargada.SOR_IdAgenciaResponsable;
                    recogida.EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                    {
                        IdEstado = recogidaDescargada.SOR_IdEstadoSolicitudRecogida,
                        Descripcion = recogidaDescargada.SPP_DescripcionMotivo
                    };
                }

                return recogida;
            }
        }

        #endregion Consulta

        #region Guardar

        /// <summary>
        /// Guarda el descargue de la recogida y el historico
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarDescargueRecogida(OURecogidasDC recogida)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    contexto.paCrearDescargueRecogida_OPU(recogida.IdProgramacionSolicitudRecogida
                      , recogida.MotivoDescargue.IdMotivo
                      , recogida.MotivoDescargue.DescripcionMotivo
                      , recogida.Observaciones == null ? "" : recogida.Observaciones
                      , ControllerContext.Current.Usuario
                      , EnumEstadoRegistro.ADICIONADO.ToString());

                    scope.Complete();
                }
            }
        }


        /// <summary>
        /// Guarda el descargue de la recogida peaton
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarDescargueRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paGuardarDescargueSolicitudRecogidaPeaton_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SRD_IdAsignacion", descargue.IdAsignacion);
                cmd.Parameters.AddWithValue("@SRD_IdMotivoDescargueSolicitud", descargue.MotivoDescargue.IdMotivo);
                cmd.Parameters.AddWithValue("@SRD_Novedad", descargue.Novedad);
                cmd.Parameters.AddWithValue("@SRD_GrabadoPor", ControllerContext.Current.Usuario);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

        }

        /// <summary>
        /// Actualiza la solicitud de recogidas
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <param name="idEstado"></param>
        /// <param name="estaDescargada"></param>
        public void ActualizarDescargueRecogida(long idSolicitud, long idProgramacionSolicitud, int idEstado, bool estaDescargada, bool estaPlanillada)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarDescargaRecogi_OPU(idSolicitud, idProgramacionSolicitud, idEstado, estaDescargada, estaPlanillada);
            }
        }

        #endregion Guardar

        #region Apertura Recogidas

        /// <summary>
        /// Obtiene los motivos de apertura de las recogidas
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoAperturaDC> ObtenerMotivosAperturaRecogida()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoAperturaRecogida_OPU.Where(r => r.MAR_Estado == OUConstantesOperacionUrbana.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new OUMotivoAperturaDC()
                  {
                      IdMotivo = r.MAR_IdMotivoAperturaRecogida,
                      DescripcionMotivo = r.MAR_Descripcion
                  });
            }
        }

        public void AbrirRecogida(long idSolicitud, string observaciones, int idMotivoApertura)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCreaReaperturaRecogida_OPU(ControllerContext.Current.Usuario,
                                                EnumEstadoRegistro.BORRADO.ToString(),
                                                idSolicitud,
                                                observaciones,
                                                idMotivoApertura);
            }
        }

        #endregion Apertura Recogidas

        #endregion Descargue de recogidas

        //#region Estado Guia

        ///// <summary>
        ///// Guarda la traza de la guia ingresada
        ///// </summary>
        ///// <param name="guia"></param>
        ///// <param name="estadoGuia"></param>
        //public void GuardaTrazaGuia(OUGuiaIngresadaDC guia)
        //{
        //    //using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        //    using(SqlConnection sqlConn = new SqlConnection(CadCnxController))
        //    {
        //        SqlCommand cmd = new SqlCommand("PaCrearEstadoGuiaTraza_MEN", sqlConn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
        //        cmd.Parameters.AddWithValue("@@numeroGuia", guia.IdAdmision);
        //        cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
        //        cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
        //        cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
        //        cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
        //        cmd.Parameters.AddWithValue("@idAdmisionMensajeria", guia.IdAdmision);
        //        contexto.PaCrearEstadoGuiaTraza_MEN(guia.IdAdmision
        //          , guia.NumeroGuia
        //          , guia.IdEstadoGuia
        //          , guia.Observaciones
        //          , guia.IdCiudad
        //          , guia.Ciudad
        //          , COConstantesModulos.MODULO_OPERACION_URBANA
        //          , DateTime.Now
        //          , ControllerContext.Current.Usuario
        //          );

        //        /*
        //                 BIGINT  

        //, BIGINT  

        //,@idEstadoGuia SMALLINT  

        //,@observacion VARCHAR(250)  

        //,@idLocalidad VARCHAR(8)  

        //,@nombreLocalidad VARCHAR(100)  

        //,@idModulo NCHAR(10)  

        //,@fechaGrabacion DATETIME  

        //,@creadoPor VARCHAR(20)  

        //,@idCentroServicio BIGINT 

        //,@NombreCentroServicio VARCHAR(250)  


        //         */
        //    }
        //}

        //#endregion Estado Guia

        #region Generales

        /// <summary>
        /// Retorna el último mensajero que tuvo asignada una guía dada
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns></returns>
        public OUNombresMensajeroDC ConsultarUltimoMensajeroGuia(long idGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GuiaMensajerosAsignados_OPU guia = contexto.GuiaMensajerosAsignados_OPU.OrderByDescending(g => g.PAM_FechaAutorizacion).FirstOrDefault(g => g.PAG_IdAdminisionMensajeria == idGuia);
                if (guia != null)
                {
                    return new OUNombresMensajeroDC
                    {
                        Identificacion = guia.PEI_Identificacion,
                        IdMensajero = guia.MEN_IdMensajero,
                        NombreApellido =
                          string.Join(" ", guia.PEI_Nombre.Trim().ToUpper(),
                                  guia.PEI_PrimerApellido.Trim().ToUpper(),
                                  guia.PEI_SegundoApellido != null ? guia.PEI_SegundoApellido.Trim().ToUpper() : string.Empty)
                    };
                }
                return null;
            }
        }

        #endregion Generales

        #region Parametros

        /// <summary>
        /// Retorna la lista de parametros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<OUParametrosDC> ObtenerParametrosOperacionUrbana(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsParametrosOperacionUrbana_OPU(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new OUParametrosDC()
                  {
                      Descripcion = r.POU_Descripcion,
                      IdParametro = r.POU_IdParametro,
                      Valor = r.POU_ValorParametro
                  });
            }
        }

        public void EditarParametroOperacionUrbana(OUParametrosDC parametro)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosOperacionUrbana_OPU parametroEn = contexto.ParametrosOperacionUrbana_OPU
                  .Where(r => r.POU_IdParametro == parametro.IdParametro)
                  .FirstOrDefault();

                if (parametroEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                parametroEn.POU_ValorParametro = parametro.Valor;
                parametroEn.POU_Descripcion = parametro.Descripcion;

                contexto.SaveChanges();
            }
        }

        #endregion Parametros

        /// <summary>
        /// Retorna el número de la última planilla y el mensajero asociado dado el número de guía
        /// </summary>
        public OUPlanillaAsignacionMensajero ObtenerUltimaPlanillaMensajeroGuia(long numeroGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var planilla = contexto.paObtenerPlanMenPorNumGuia_OPU(numeroGuia).FirstOrDefault();
                if (planilla != null)
                {
                    return new OUPlanillaAsignacionMensajero
                    {
                        IdPlanillaAsignacionEnvio = planilla.PAG_IdPlanillaAsignacionEnvio,
                        NumeroGuia = planilla.PAG_NumeroGuia,
                        Mensajero = new OUMensajeroDC
                        {
                            NombreCompleto = string.Join(" ", planilla.PEI_Nombre, planilla.PEI_PrimerApellido, planilla.PEI_SegundoApellido),
                            IdMensajero = planilla.PAM_IdMensajero
                        },
                        EstadoEnPlanilla = planilla.PAG_EstadoEnPlanilla
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// Valida si el al cobro ya fué reportado en caja por algun mensajero
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns>Numero de Comprobante con que se reporto en caja</returns>
        public long AlCobroReportadoEnCaja(long numeroGuia)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var planillaGuia = contexto.PlanillaAsignacionGuia_OPU.
                    Where(p => p.PAG_NumeroGuia == numeroGuia && p.PAG_ReportadoACaja > 0).FirstOrDefault();

                if (planillaGuia != null)
                {
                    return planillaGuia.PAG_ReportadoACaja;
                }

                return 0;
            }
        }

        #region Asignacion de tulas y precintos

        /// <summary>
        /// Método para obtener los tipos de asignación posibles
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OUTipoAsignacionDC> ObtenerTiposAsignacion()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoAsignacionTula_OPU.OrderBy(o => o.TAS_Descripcion).ToList().ConvertAll(r => new OUTipoAsignacionDC()
                {
                    DescripcionTipoAsignacion = r.TAS_Descripcion,
                    IdTipoAsignacion = r.TAS_IdTipoAsignacion
                });
            };
        }

        /// <summary>
        /// Método para obtener las tulas y precintos sin utilizar generadas desde una racol
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<OUAsignacionDC> ObtenerAsignacionCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                filtro.Add("ATP_Estado", OUConstantesOperacionUrbana.ESTADO_CREADA);
                return contexto.ConsultarContainsAsignacionTulaPuntoServicio_VOPU(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new OUAsignacionDC()
                  {
                      CentroServicioOrigen =
                      new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC
                      {
                          IdCentroServicio = r.ATP_IdCentroServicioOrigen,
                          Nombre = String.Concat(r.ATP_IdCentroServicioOrigen, " - ", r.Ces_Nombre_Origen),
                          Telefono1 = r.tel_origen,
                          Direccion = r.dir_origen,
                          CiudadUbicacion = new PALocalidadDC { IdLocalidad = r.id_loc_origen, Nombre = r.nom_loc_prigen }
                      },

                      CentroServicioDestino =
                      new Servicios.ContratoDatos.CentroServicios.PUCentroServiciosDC
                      {
                          IdCentroServicio = r.ATP_IdCentroServicioDestino,
                          Nombre = String.Concat(r.ATP_IdCentroServicioDestino, " - ", r.Ces_Nombre_Destino),
                          Telefono1 = r.tel_destino,
                          Direccion = r.dir_destino,
                          CiudadUbicacion = new PALocalidadDC { IdLocalidad = r.id_loc_detino, Nombre = r.nom_loc_destino }
                      },
                      CreadoPor = r.ATP_CreadoPor,
                      Estado = r.ATP_Estado,
                      FechaCreacion = r.ATP_FechaGrabacion,
                      IdAsignacion = r.ATP_IdAsignacionTula,
                      NoPrecinto = r.ATP_NoPrecinto,
                      NoTula = r.ATP_NoTula,
                      NumContTransDespacho = r.ATP_NumContTransDespacho,
                      NumContTransRetorno = r.ATP_NumContTransRetorno,
                      TipoAsignacion = new OUTipoAsignacionDC { IdTipoAsignacion = r.ATP_IdTipoAsignacion, DescripcionTipoAsignacion = r.TAS_Descripcion },
                  });
            }
        }

        /// <summary>
        /// Método para asignar una tula y un precinto a un centro de servicio
        /// </summary>
        /// <param name="asignacionTula"></param>
        /// <returns></returns>
        public OUAsignacionDC AdicionarAsignacionCentroServicio(OUAsignacionDC asignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AsignacionTulaPuntoServicio_OPU asignacionBD = new AsignacionTulaPuntoServicio_OPU
                {
                    ATP_Estado = asignacion.Estado.ToString(),
                    ATP_IdCentroServicioDestino = asignacion.CentroServicioDestino.IdCentroServicio,
                    ATP_IdCentroServicioOrigen = asignacion.CentroServicioOrigen.IdCentroServicio,
                    ATP_IdTipoAsignacion = asignacion.TipoAsignacion.IdTipoAsignacion,
                    ATP_NoPrecinto = asignacion.NoPrecinto,
                    ATP_NoTula = asignacion.NoTula,
                    ATP_NumContTransDespacho = asignacion.NumContTransDespacho,
                    ATP_NumContTransRetorno = asignacion.NumContTransRetorno,
                    ATP_CreadoPor = ControllerContext.Current.Usuario,
                    ATP_FechaGrabacion = DateTime.Now,
                };
                contexto.AsignacionTulaPuntoServicio_OPU.Add(asignacionBD);
                contexto.SaveChanges();
                asignacion.IdAsignacion = asignacionBD.ATP_IdAsignacionTula;
                asignacion.FechaCreacion = DateTime.Now;
                asignacion.CreadoPor = ControllerContext.Current.Usuario;
                return asignacion;
            }
        }

        /// <summary>
        /// Método para cambiar el estado de una asignación
        /// </summary>
        /// <param name="asignacionTula"></param>
        /// <returns></returns>
        public void CambiarEstadoAdicionarAsignacion(long idAsignacion, string Estado)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AsignacionTulaPuntoServicio_OPU asignacionBD = contexto.AsignacionTulaPuntoServicio_OPU.Where(asg => asg.ATP_IdAsignacionTula == idAsignacion).FirstOrDefault();
                if (asignacionBD != null)
                {
                    asignacionBD.ATP_Estado = Estado;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para validar que una tula/contenedor se encuentre libre para ser asignado
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public void ValidarTulaContenedor(OUAsignacionDC asignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.AsignacionTulaPuntoServicio_OPU.Where(tul => tul.ATP_NoTula == asignacion.NoTula && tul.ATP_Estado != OUConstantesOperacionUrbana.ESTADO_DESCARGADO).FirstOrDefault() != null)
                    throw new FaultException<ControllerException>
                        (new ControllerException
                            (COConstantesModulos.MODULO_OPERACION_URBANA,
                            OUEnumTipoErrorOU.EX_ERROR_ASIGNACION.ToString(),
                            OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_ERROR_ASIGNACION)));
            }
        }

        /// <summary>
        /// Método para eliminar una asignacion de tulas o contenedores
        /// </summary>
        /// <param name="asignacion"></param>
        public void EliminarAsignacionTulaContenedor(OUAsignacionDC asignacion)
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AsignacionTulaPuntoServicio_OPU asignacionBD = contexto.AsignacionTulaPuntoServicio_OPU.Where(asi => asi.ATP_IdAsignacionTula == asignacion.IdAsignacion).FirstOrDefault();
                if (asignacionBD != null)
                {
                    contexto.AsignacionTulaPuntoServicio_OPU.Remove(asignacionBD);
                    //    OURepositorioAudit.MapeoAuditAsignacionTulas(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Asignacion de tulas y precintos

        #region Novedades de ingreso

        /// <summary>
        /// Método para obtener las novedades de ingreso
        /// </summary>
        /// <returns></returns>
        public List<OUNovedadIngresoDC> ObtenerNovedadesIngreso()
        {
            using (ModeloOperacionUrbana contexto = new ModeloOperacionUrbana(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.NovedadesEnviosSueltos_OPU.OrderBy(o => o.NES_Descripcion)
                    .ToList().ConvertAll(r => new OUNovedadIngresoDC()
                    {
                        DescripcionNovedad = r.NES_Descripcion,
                        IdNovedad = r.NES_IdNovedad
                    });
            }
        }

        #endregion Novedades de ingreso

        /// <summary>
        /// Aprovisiona guía catalogada como "fantasma", es decir una numeración que debería ser automática
        /// </summary>
        /// <param name="numGUia"></param>
        /// <param name="idCs"></param>
        public bool AprovisionGuiaFantasma(long numGuia, long idCs)
        {
            int iResultado = 0;
            using (SqlConnection cnx = new SqlConnection(conexionStringController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paValidarIngresoFantasma_OPU", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numGuia", numGuia));
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicios", idCs));
                cmd.Parameters.Add(new SqlParameter("@usuario", ControllerContext.Current.Usuario));
                var reader = cmd.ExecuteReader();
                reader.Read();
                var resultado = reader[0];
                int.TryParse(resultado.ToString(), out iResultado);

                cnx.Close();
            }
            return iResultado > 0;
        }

        /// <summary>
        /// crea nueva auditoria
        /// </summary>
        /// <param name="idCs"></param>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public long CrearAuditoriaAsignacionMensajero(long idCs, long idMensajero)
        {
            long iResultado = 0;
            using (SqlConnection cnx = new SqlConnection(conexionStringController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paInsertarAuditoriaAsignacionMensajero_OPU", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCentroServicios", idCs));
                cmd.Parameters.Add(new SqlParameter("@idMensajero", idMensajero));
                cmd.Parameters.Add(new SqlParameter("@usuario", ControllerContext.Current.Usuario));
                var reader = cmd.ExecuteReader();
                reader.Read();
                var resultado = reader[0];
                long.TryParse(resultado.ToString(), out iResultado);
                cnx.Close();
                cnx.Dispose();
            }
            return iResultado;
        }

        /// <summary>
        /// inserta las guias en la tabla planillaguiaaudit
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="idMensajero"></param>
        /// <param name="fecha"></param>
        public void CrearAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long idMensajero, DateTime fecha)
        {
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paInsertarAuditoriaAsignacionMensajeroGuia_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idAuditoria", idAuditoria));
                cmd.Parameters.Add(new SqlParameter("@esSobrante", esSobrante));
                cmd.Parameters.Add(new SqlParameter("@idMensajero", idMensajero));
                cmd.Parameters.Add(new SqlParameter("@fecha", fecha));
                cmd.Parameters.Add(new SqlParameter("@usuario", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// actualiza el estado en la tabla planilaauditoriaguia
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="numeroGuia"></param>
        public void ActualizarAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long numeroGuia)
        {
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paActualizarAuditoriaAsignacionMensajeroGuia_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idAuditoria", idAuditoria));
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@esSobrante", esSobrante));
                cmd.Parameters.Add(new SqlParameter("@usuario", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// ConsultaSobrantes
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="numeroAuditoria"></param>
        /// <returns></returns>
        public string ConsultarNumeroGuiaSobrante(long numeroGuia, long numeroAuditoria)
        {
            string guia = null;
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paConsultarGuiaSobrante_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@numeroAuditoria", numeroAuditoria));
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    guia = reader["PAG_NumeroGuia"].ToString();
                }
                con.Close();
                con.Dispose();
            }

            return guia;

        }


        /// <summary>
        /// Inserta el umero digitado como un sobrante
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="numeroGuia"></param>
        public void InsertarGuiaSobrante(long idAuditoria, int esSobrante, long numeroGuia)
        {
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paInsertarGuiaSobrante_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idAuditoria", idAuditoria));
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@esSobrante", esSobrante));
                cmd.Parameters.Add(new SqlParameter("@usuario", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
            }
        }

        /// <summary>
        /// Obtiene todas las auditorias realizadas a mensajero en determinado rango de fecha
        /// </summary>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerAuditoriasPorMensajero(long idMensajero, DateTime fechaIni, DateTime fechaFin)
        {
            List<OUGuiaIngresadaDC> lista = new List<OUGuiaIngresadaDC>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAuditoriasPorMensajero_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdMensajero", idMensajero));
                cmd.Parameters.Add(new SqlParameter("@FechaIni", fechaIni));
                cmd.Parameters.Add(new SqlParameter("@FechaFin", fechaFin));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    lista.Add(new OUGuiaIngresadaDC
                    {
                        NumeroAuditoria = Convert.ToInt64(lector["PAA_IdAuditoria"]),
                        FechaAuditoria = Convert.ToDateTime(lector["PAA_Fecha"].ToString())
                    });
                }

                conn.Close();
                conn.Dispose();
            }

            return lista;
        }

        /// <summary>
        /// Obtiene todas las guias que poertenezcan a cierto numero de auditoria
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorAuditoria(long idAuditoria)
        {
            List<OUGuiaIngresadaDC> lista = new List<OUGuiaIngresadaDC>();

            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasDeAuditoria_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idAuditoria", idAuditoria));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    lista.Add(new OUGuiaIngresadaDC
                    {
                        NumeroAuditoria = Convert.ToInt64(lector["PAG_IdAuditoria"]),
                        NumeroGuia = Convert.ToInt64(lector["PAG_NumeroGuia"]),
                        EsSobrante = Convert.ToInt32(lector["PAG_EsSobrante"])
                    });
                }

                con.Close();
                con.Dispose();
            }
            return lista;
        }

        /// <summary>
        /// Obtiene el total de guias de la auditoria con todo el detalle para la grilla principal
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerDetalleGuiasAuditadas(long idAuditoria)
        {
            List<OUGuiaIngresadaDC> lista = new List<OUGuiaIngresadaDC>();

            using (SqlConnection con = new SqlConnection(conexionStringController))
            {

                con.Open();

                SqlCommand cmd = new SqlCommand("paObtenerTotalGuiasAuditadasMensajero_OPU", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@idAuditoria", idAuditoria));

                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    lista.Add(new OUGuiaIngresadaDC
                    {
                        NumeroGuia = Convert.ToInt64(lector["PAG_NUMEROGUIA"]),
                        CreadoPor = lector["PAG_CREADOPOR"].ToString(),
                        Planilla = Convert.ToInt64(lector["PAG_IDPLANILLAASIGNACIONENVIO"]),
                        Peso = Convert.ToDecimal(lector["PAG_Peso"]),
                        DireccionDestinatario = lector["PAG_DireccionDestinatario"].ToString(),
                        NombreTipoEnvio = lector["PAG_NOMBRETIPOENVIO"].ToString(),
                        EstaVerificada = Convert.ToBoolean(lector["PAG_EstaVerificada"]),
                        DiceContener = lector["ADM_DiceContener"].ToString(),
                        FechaAsignacion = Convert.ToDateTime(lector["PAG_FechaGrabacion"].ToString()),
                        NumeroAuditoria = Convert.ToInt64(lector["PAG_IdAuditoria"]),
                        UsuarioAuditor = lector["PAG_CreadoPor"].ToString(),
                        FechaAuditoria = Convert.ToDateTime(lector["PAG_Fecha"].ToString())
                    });
                }

                con.Close();

                con.Dispose();

            }

            return lista;
        }

        /// <summary>
        /// Obtiene las recogidas realizadas de cliente peaton segun el token del dispositivo.
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerMisRecogidasClientePeaton(string tokenDispositivo)
        {
            List<OURecogidasDC> misRecogidas = new List<OURecogidasDC>();
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {

                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMisRecogidasClientePeaton_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@tokenDispositivo", tokenDispositivo));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    misRecogidas.Add(new OURecogidasDC
                    {
                        IdRecogida = (long)lector["SOR_IdSolicitudRecogida"],
                        FechaSolicitud = Convert.ToDateTime(lector["SOR_FechaGrabacion"].ToString()),
                        FechaRecogida = Convert.ToDateTime(lector["SOR_FechaRecogida"].ToString()),
                        Direccion = lector["SOR_Direccion"].ToString(),
                        ComplementoDireccion = lector["SOR_ComplementoDireccion"].ToString(),
                        EstadoRecogida = new OUEstadosSolicitudRecogidaDC()
                        {
                            IdEstado = (short)lector["SOR_IdEstadoSolicitudRecogida"],
                            Descripcion = lector["EPS_Descripcion"].ToString()
                        },
                        AsignacionMensajero = new OUAsignacionRecogidaMensajeroDC()
                        {
                            IdAsignacion = (lector["SAM_IdAsignacion"] == DBNull.Value) ? 0 : (long)lector["SAM_IdAsignacion"]
                        },
                        MotivoDescargue = new OUMotivoDescargueRecogidasDC()
                        {
                            IdMotivo = (lector["MOD_IdMotivoDescargue"] == DBNull.Value) ? 0 : (short)lector["MOD_IdMotivoDescargue"],
                            DescripcionMotivo = (lector["MOD_Descripcion"] == DBNull.Value) ? "" : lector["MOD_Descripcion"].ToString()
                        }
                    });
                }
                con.Close();
                con.Dispose();
            }

            return misRecogidas;

        }

        /// <summary>
        /// Metodo que obtiene las imagenes de la solicitud recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesSolicitudRecogida(long idSolicitudRecogida)
        {
            List<string> misFotografias = new List<string>();
            using (SqlConnection con = new SqlConnection(conexionStringController))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutaImagenesSolicitudRecogidaPeaton_OPU", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idSolicitudRecogida", idSolicitudRecogida));
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    byte[] imagen = File.ReadAllBytes(lector["IMR_RutaImagen"].ToString());
                    misFotografias.Add(Convert.ToBase64String(imagen));
                }
            }

            return misFotografias;

        }

        /// <summary>
        /// Metodo para calificar la solicitud recogida 
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="calificacion"></param>
        /// <param name="observaciones"></param>
        public void CalificarSolicitudRecogida(long idSolicitudRecogida, int calificacion, string observaciones)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paIngresarCalificacionSolicitudRecogidaPeaton_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idSolicitudRecogida", idSolicitudRecogida));
                cmd.Parameters.Add(new SqlParameter("@calificacion", calificacion));
                cmd.Parameters.Add(new SqlParameter("@observaciones", observaciones));
                cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }
        }

        #region Obtener mensajeros pam por centro de servicio
        /// <summary>
        /// Obtiene los mensajero pam de un centro de servicio
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <returns></returns>
        public List<OUMensajeroPamDC> ObtenerMensajerosPamPorCentroServicio(long IdCentroServicio)
        {
            List<OUMensajeroPamDC> lista = new List<OUMensajeroPamDC>();
            OUMensajeroPamDC MensajeroPam;
            using (SqlConnection cnx = new SqlConnection(conexionStringController))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMensajerosPamCentroServicio_OPU", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicio", IdCentroServicio));
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    MensajeroPam = new OUMensajeroPamDC()
                    {
                        PersonaInterna = new OUPersonaInternaDC()
                        {
                            IdPersonaInterna = Convert.ToInt64(lector["PEI_IdPersonaInterna"]),
                            IdTipoIdentificacion = lector["PEI_IdTipoIdentificacion"].ToString(),
                            Identificacion = lector["PEI_Identificacion"].ToString(),
                            IdCargo = Convert.ToInt32(lector["PEI_IdCargo"]),
                            Nombre = lector["PEI_Nombre"].ToString(),
                            PrimerApellido = lector["PEI_PrimerApellido"].ToString(),
                            SegundoApellido = lector["PEI_SegundoApellido"].ToString(),
                            Telefono = lector["PEI_Telefono"].ToString(),
                            Email = lector["PEI_Email"].ToString()
                        },
                        Usuario = new ASUsuario()
                        {
                            IdUsuario = lector["USU_IdUsuario"].ToString()
                        },
                        NombreCompleto = lector["PEI_Nombre"].ToString() + " " + lector["PEI_PrimerApellido"].ToString() + " " + lector["PEI_SegundoApellido"].ToString()
                    };
                    lista.Add(MensajeroPam);
                }
                cnx.Close();
                cnx.Dispose();
            }
            return lista;
        }
        #endregion


        #region Obtener lista guias mensajeros pam por centro de servicio

        public List<LIReclameEnOficinaDC> ObtenerGuiasDelPamPorCentroServicio(long idCentroServicio, string usuario)
        {
            List<LIReclameEnOficinaDC> lista = new List<LIReclameEnOficinaDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasMensajeroPam_OPU", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@INV_IdCentroServicio", idCentroServicio));
                cmd.Parameters.Add(new SqlParameter("@INV_CreadoPor", null));  // Pendiente Cambiar para Filtrar por MensajeroPAM
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {
                    LIReclameEnOficinaDC ReclameEnOficina = new LIReclameEnOficinaDC
                    {
                        DocumentoDestinatario = read["ADM_IdDestinatario"].ToString(),
                        NombreDestinatario = read["ADM_NombreDestinatario"].ToString(),
                        DocumentoRemitente = read["ADM_IdRemitente"].ToString(),
                        NombreRemitente = read["ADM_NombreRemitente"].ToString(),
                        DireccionRemitente = read["ADM_DireccionRemitente"].ToString(),
                        EsAlCobro = Convert.ToBoolean(read["ADM_EsAlCobro"]),
                        ValorTotal = Convert.ToDecimal(read["ADM_ValorTotal"]),
                        Peso = Convert.ToInt64(read["ADM_Peso"]),
                        DiceContener = read["ADM_DiceContener"].ToString(),
                        MovimientoInventario = new PUMovimientoInventario()
                        {
                            NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]),
                            FechaGrabacion = Convert.ToDateTime(read["INV_FechaGrabacion"]),
                            IdMovimientoInventario = Convert.ToInt64(read["INV_IdMovimientoInventario"]),
                            TipoMovimiento = (PUEnumTipoMovimientoInventario)Convert.ToInt16(read["INV_IdTipoMovimiento"]),
                        },
                        FormaPago = new ADGuiaFormaPago()
                        {
                            IdFormaPago = Convert.ToInt16(read["FOP_IdFormaPago"]),
                            Descripcion = read["FOP_Descripcion"].ToString()
                        },
                        TipoUbicacion = PUEnumTipoUbicacion.Casillero,
                        Ubicacion = 0,
                        DiasTranscurridos = (DateTime.Now.Date - Convert.ToDateTime(read["INV_FechaGrabacion"])).Days,
                        Respuesta = OUEnumValidacionDescargue.Exitosa,
                    };
                    ReclameEnOficina.MovimientoInventario.Bodega = new PUCentroServiciosDC();
                    ReclameEnOficina.MovimientoInventario.Bodega.CiudadUbicacion = new PALocalidadDC();
                    if (ReclameEnOficina.FormaPago.IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO)
                    {
                        ReclameEnOficina.EsContado = true;
                    }
                    else
                    {
                        ReclameEnOficina.EsContado = false;
                    }

                    lista.Add(ReclameEnOficina);
                }
            }
            return lista;

        }
        #endregion

        /// <summary>
        /// Método que elimina una guia de la planilla y actualiza el total de envios de la planilla
        /// </summary>
        /// <param name="numeroGuia"></param>
        public void EliminarEnvioPlanillaAsignacionCac(long numeroGuia)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paActualizaPlanillaAsigCac_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@numeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@Usuario", ControllerContext.Current.Usuario));
                cmd.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
            }

        }

        /// <summary>
        /// Metodo para obtener la informacion del usuario (mensajero/auditor)
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public OUMensajeroDC ObtenerInformacionUsuarioControllerApp(string numIdentificacion)
        {
            OUMensajeroDC respuesta = new OUMensajeroDC();
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand(@"paObtenerInformacionMensajeroAuditorControllerApp_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numIdentificacion", numIdentificacion);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    respuesta.IdMensajero = reader["MEN_IdMensajero"] == DBNull.Value ? 0 : Convert.ToInt64(reader["MEN_IdMensajero"]);
                    respuesta.NombreCompleto = (reader["PEI_Nombre"] == DBNull.Value ? string.Empty : reader["PEI_Nombre"].ToString()) + " " + (reader["PEI_PrimerApellido"] == DBNull.Value ? string.Empty : reader["PEI_PrimerApellido"].ToString()) + " " + (reader["PEI_SegundoApellido"] == DBNull.Value ? string.Empty : reader["PEI_SegundoApellido"].ToString());
                }
                conn.Close();
                cmd.Dispose();
            }
            return respuesta;
        }


        #region Manifiestos

        public OUMensajeroDC ObtenerResponsableGuiaManifiestoUrbPorNGuia(long numeroGuia)
        {
            OUMensajeroDC mensajero = null;
            using (SqlConnection conn = new SqlConnection(CadCnxController)) {

                SqlCommand cmd = new SqlCommand("paConsultarManifiestoUrbPorNumeroGuia_OPN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    mensajero = new OUMensajeroDC();
                    if (reader.Read())
                    {
                        mensajero.PersonaInterna.Identificacion = reader["PEI_IDENTIFICACION"].ToString();
                        mensajero.PersonaInterna.Nombre = reader["NOMBRE"].ToString();
                        mensajero.IdCentroServicio = Convert.ToInt64(reader["MEN_IDAGENCIA"]);
                    }

                }
                conn.Close();
                return mensajero;
            }
        }

        public OUMensajeroDC ObtenerAsignacionMensajeroPorNumeroGuia(long numeroGuia)
        {
            OUMensajeroDC mensajero = null;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerAsignacionMensajeroPorNumeroGuia_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    mensajero = new OUMensajeroDC();
                    if (reader.Read())
                    {
                        mensajero.PersonaInterna.Identificacion = reader["PEI_IDENTIFICACION"].ToString();
                        mensajero.PersonaInterna.Nombre = reader["NOMBRE"].ToString();
                        mensajero.IdCentroServicio = Convert.ToInt64(reader["MEN_IDAGENCIA"]);
                    }

                }
                conn.Close();
                return mensajero;
            }
        }


        /// <summary>
        /// Obtiene los datos de determinado mensajero por su numero de cedula
        /// </summary>
        /// <returns></returns>
        public OUDatosMensajeroDC ObtenerDatosMensajeroPorNumeroDeCedula(string identificacionMensajero)
        {
            OUDatosMensajeroDC datosMensajero = null;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDatosMensajeroPorCedula_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Identificacion", identificacionMensajero);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    datosMensajero = new OUDatosMensajeroDC
                    {
                        IdMensajero = Convert.ToInt64(reader["MEN_IdMensajero"]),
                        IdTipoMensajero = Convert.ToInt32(reader["MEN_IdTipoMensajero"]),
                        IdAgencia = Convert.ToInt64(reader["MEN_IdAgencia"]),
                        Telefono2 = Convert.ToString(reader["MEN_Telefono2"]),
                        FechaIngreso = Convert.ToDateTime(reader["MEN_FechaIngreso"]),
                        FechaTerminacionContrato = Convert.ToDateTime(reader["MEN_FechaTerminacionContrato"]),
                        NumeroPase = Convert.ToString(reader["MEN_NumeroPase"]),
                        FechaVencimientoPase = Convert.ToDateTime(reader["MEN_FechaVencimientoPase"]),
                        Estado = Convert.ToString(reader["MEN_Estado"]),
                        EsContratista = Convert.ToBoolean(reader["MEN_EsContratista"]),
                        Descripcion = Convert.ToString(reader["TIM_Descripcion"]),
                        EsVehicular = Convert.ToBoolean(reader["TIM_EsVehicular"]),
                        IdPersonaInterna = Convert.ToInt64(reader["PEI_IdPersonaInterna"]),
                        IdTipoIdentificacion = Convert.ToString(reader["PEI_IdTipoIdentificacion"]),
                        Identificacion = Convert.ToString(reader["PEI_Identificacion"]),
                        IdCargo = Convert.ToInt32(reader["PEI_IdCargo"]),
                        NombreMensajero = Convert.ToString(reader["NombreMensajero"]),
                        PrimerApellido = Convert.ToString(reader["PEI_PrimerApellido"]),
                        SegundoApellido = Convert.ToString(reader["PEI_SegundoApellido"]),
                        DireccionMensajero = Convert.ToString(reader["DireccionMensajero"]),
                        Municipio = Convert.ToString(reader["PEI_Municipio"]),
                        Telefono = Convert.ToString(reader["PEI_Telefono"]),
                        EmailMensajero = Convert.ToString(reader["EmailMensajero"]),
                        IdRegionalAdm = Convert.ToInt64(reader["PEI_IdRegionalAdm"]),
                        Comentarios = Convert.ToString(reader["PEI_Comentarios"]),
                        IdCentroServicios = Convert.ToInt64(reader["CES_IdCentroServicios"]),
                        NombreCentroServicio = Convert.ToString(reader["NombreCentroServicio"]),
                        Telefono1 = Convert.ToString(reader["CES_Telefono1"]),
                        DireccionCentroServicio = Convert.ToString(reader["DireccionCentroServicio"]),
                        IdMunicipio = Convert.ToString(reader["CES_IdMunicipio"]),
                        NombreLocalidad = Convert.ToString(reader["NombreLocalidad"]),
                        TipoContrato = Convert.ToInt32(reader["MEN_TipoContrato"])
                    };
                }
                conn.Close();
            }
            return datosMensajero;
        }

        #endregion
    }
}