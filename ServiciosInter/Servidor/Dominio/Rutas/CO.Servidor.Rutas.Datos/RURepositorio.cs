using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Rutas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace CO.Servidor.Rutas.Datos
{
    /// <summary>
    /// Clase para consultar y persistir informacion en la base de datos para los procesos de rutas
    /// </summary>
    public class RURepositorio
    {
        private static readonly RURepositorio instancia = new RURepositorio();
        private const string NombreModelo = "ModeloRutas";
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static RURepositorio Instancia
        {
            get { return RURepositorio.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private RURepositorio() { }

        #region Nacional

        /// <summary>
        /// Obtiene  las rutas
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista  con las rutas</returns>
        public IList<RURutaDC> ObtenerRutas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsRutas_VRUT(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
                  .ConvertAll<RURutaDC>(
                  r =>
                  {
                      RURutaDC ruta = new RURutaDC()
                      {
                          CostoMensualRuta = r.RUT_CostoMensualTotal,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          GeneraManifiesto = r.RUT_GeneraManifiestoMinisterio,

                          IdLocalidadDestino = r.RUT_IdLocalidadDestino,
                          IdLocalidadOrigen = r.RUT_IdLocalidadOrigen,
                          CiudadDestino = new PALocalidadDC()
                          {
                              IdLocalidad = r.RUT_IdLocalidadDestino,
                              Nombre = r.RUT_NombreLocalidadDestino,
                          },
                          CiudadOrigen = new PALocalidadDC()
                          {
                              Nombre = r.RUT_NombreLocalidadOrigen,
                              IdLocalidad = r.RUT_IdLocalidadOrigen
                          },
                          IdRuta = r.RUT_IdRuta,
                          IdTipoRuta = r.RUT_IdTipoRuta,
                          NombreRuta = r.RUT_Nombre,

                          NombreLocalidadDestino = r.RUT_NombreLocalidadDestino,
                          NombreLocalidadOrigen = r.RUT_NombreLocalidadOrigen,
                          NombreMedioTransporte = r.MTR_Descripcion,
                          NombreTipoRuta = r.TRU_Descripcion,

                          IdMediotransporte = r.MTR_IdMedioTransporte,
                          Estado = r.RUT_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,

                          IdTipoVehiculo = r.RUT_IdTipoVehiculo,

                          FrecuenciaRuta = contexto.FrecuenciaRuta_RUT.Where(f => f.FRR_IdRuta == r.RUT_IdRuta).ToList().
                          ConvertAll<RUFrecuenciaRuta>(f =>
                            new RUFrecuenciaRuta()
                            {
                                EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                                IdFrecuenciaRuta = f.FRR_IdFrecuenciaRuta,
                                HoraLlegada = f.FRR_HoraLlegada,
                                HoraSalida = f.FRR_HoraSalida,
                                IdDia = f.FRR_IdDia,
                                IdRuta = f.FRR_IdRuta
                            }),
                          EmpresasTransportadoras = contexto.RutaEmpresaTrans_VRUT.Where(em => em.RET_IdRuta == r.RUT_IdRuta).ToList().
                          ConvertAll<RUEmpresaTransportadora>(em => new RUEmpresaTransportadora()
                          {
                              IdEmpresaTransportadora = em.RET_IdEmpresaTransportadora,
                              Nombre = em.ETR_Nombre,
                              IdMedioTransporte = em.RUT_IdMedioTransporte,
                              IdTipoTransporte = em.TIT_IdTipoTransporte,
                              EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                          })
                      };

                      RUEmpresaTransportadora empresa = ruta.EmpresasTransportadoras.FirstOrDefault();

                      if (empresa != null)
                      {
                          ruta.IdTipoTransporte = empresa.IdTipoTransporte;

                          TipoTransporte_RUT tipotrans = contexto.TipoTransporte_RUT.Where(t => t.TIT_IdTipoTransporte == empresa.IdTipoTransporte).FirstOrDefault();
                          if (tipotrans != null)
                              ruta.NombreTipoTransporte = tipotrans.TIT_Descripcion;
                      }
                      return ruta;
                  });
            }
        }

        /// <summary>
        /// Obtiene la informacion de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public RURutaDC ObtenerRutaIdRuta(int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var r = contexto.Rutas_VRUT.Where(o => o.RUT_IdRuta == idRuta).FirstOrDefault();
                if (r != null)
                {
                    RURutaDC ruta = new RURutaDC()
                     {
                         CostoMensualRuta = r.RUT_CostoMensualTotal,
                         EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                         GeneraManifiesto = r.RUT_GeneraManifiestoMinisterio,

                         IdLocalidadDestino = r.RUT_IdLocalidadDestino,
                         IdLocalidadOrigen = r.RUT_IdLocalidadOrigen,
                         CiudadDestino = new PALocalidadDC()
                         {
                             IdLocalidad = r.RUT_IdLocalidadDestino,
                             Nombre = r.RUT_NombreLocalidadDestino,
                         },
                         CiudadOrigen = new PALocalidadDC()
                         {
                             Nombre = r.RUT_NombreLocalidadOrigen,
                             IdLocalidad = r.RUT_IdLocalidadOrigen
                         },
                         IdRuta = r.RUT_IdRuta,
                         IdTipoRuta = r.RUT_IdTipoRuta,
                         NombreRuta = r.RUT_Nombre,

                         NombreLocalidadDestino = r.RUT_NombreLocalidadDestino,
                         NombreLocalidadOrigen = r.RUT_NombreLocalidadOrigen,
                         NombreMedioTransporte = r.MTR_Descripcion,
                         NombreTipoRuta = r.TRU_Descripcion,

                         IdMediotransporte = r.MTR_IdMedioTransporte,
                         Estado = r.RUT_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,

                         IdTipoVehiculo = r.RUT_IdTipoVehiculo
                     };
                    return ruta;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_RUTAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtiene  las ciudades estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idRuta">Id de la ruta por la cual se filtraran las estaciones</param>
        /// <returns>Lista  con las rutas</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<EstacionRuta_RUT>("ESR_IdRuta", idRuta.ToString(), OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);
                if (string.IsNullOrWhiteSpace(campoOrdenamiento))
                    campoOrdenamiento = "ESR_OrdenEnRuta";

                return contexto.ConsultarEstacionRuta_RUT(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().
                  ConvertAll<RUEstacionRuta>(es =>
                  {
                      RUEstacionRuta estacion = new RUEstacionRuta()
                      {
                          IdEstacionRuta = es.ESR_IdEstacionRuta,
                          IdLocalidadEstacion = es.ESR_IdLocalidadEstacion,
                          IdRuta = es.ESR_IdRuta,
                          NombreLocalidadEstacion = es.ESR_NombreLocalidadEstacion,
                          OrdenEnCasillero = es.ESR_OrdenEnCasillero,
                          OrdenEnRuta = es.ESR_OrdenEnRuta,
                          PermiteEnganche = es.ESR_PermiteEnganche,
                          RequierePrecintoRetorno = es.ESR_RequierePrecintoRetorno,
                          TiempoParada = es.ESR_TiempoParada,
                          TiempoViajeDesdeAnterior = es.ESR_TiempoDeViajeDesdeAnterior,
                          CiudadEstacion = new PALocalidadDC() { Nombre = es.ESR_NombreLocalidadEstacion, IdLocalidad = es.ESR_IdLocalidadEstacion },
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                      };

                      estacion.FrecuenciaLlegada = contexto.FrecuenciaEstaciones_VRUT.Where(f => f.FRE_IdEstacionRuta == es.ESR_IdEstacionRuta).ToList().
                        ConvertAll<RUFrecuenciaParadaEstacion>(f =>
                          new RUFrecuenciaParadaEstacion()
                          {
                              IdDia = f.FRE_IdDia,
                              HoraLlegada = f.FRE_HoraLlegada,
                              IdEstacionRuta = f.ESR_IdEstacionRuta,
                              IdFrecuenciaParadaEstacion = f.FRE_IdFrecuenciaParadaEstacion,
                              EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                          });

                      return estacion;
                  }
              );
            }
        }

        /// <summary>
        /// Obtiene  las ciudades hijas (cobertura) de una estacion de una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCiudadEstacion">Id de ciudad estacion para la cual se retornara la cobertura</param>
        /// <returns>Lista  con las ciudades hijas de la ciudad estacion (cobertura de la ciudad estacion)</returns>
        public IList<RUCoberturaEstacion> ObtenerCiudadesHijasEstacionesRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCiudadEstacion)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<CoberturaEstacionRuta_RUT>("CER_IdEstacionRuta", idCiudadEstacion.ToString(), OperadorComparacion.Equal);

                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarCoberturaEstacionRuta_RUT(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().
                   ConvertAll<RUCoberturaEstacion>(o =>
                     new RUCoberturaEstacion()
                     {
                         EstacionHija = new PALocalidadDC()
                         {
                             Nombre = o.CER_NombreLocalidadEstacion,
                             IdLocalidad = o.CER_IdLocalidadCubierta,
                             EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                         },
                         IdCoberturaEstacionRuta = o.CER_IdCoberturaEstacionRuta,
                         EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                     });
            }
        }

        /// <summary>
        /// Obtiene una lista con todos los tipos de ruta, para llenar un comboBox
        /// </summary>
        /// <returns>Lista con los tipos de ruta</returns>
        public IList<RUTipoRuta> ObtenerTodosTipoRuta()
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoRuta_RUT.ToList().ConvertAll<RUTipoRuta>(tr =>
                  new RUTipoRuta()
                  {
                      IdTipoRuta = tr.TRU_IdTipoRuta,
                      NombreTipoRuta = tr.TRU_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene las empresas trasportadoras filtradas por el tipo de transporte
        /// </summary>
        /// <param name="idTipoTransporte"></param>
        /// <returns>Lista de empresas transportadoras filtradas por el tipo de transporte</returns>
        public IList<RUEmpresaTransportadora> ObtieneEmpresaTransportadoraTipoTransporte(int idTipoTransporte)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EmpresaTransportadora_RUT.Where(e => e.ETR_IdTipoTransporte == idTipoTransporte).ToList().ConvertAll<RUEmpresaTransportadora>(em =>
                  new RUEmpresaTransportadora()
                  {
                      IdEmpresaTransportadora = em.ETR_IdEmpresaTransportadora,
                      Nombre = em.ETR_Nombre
                  }).OrderBy(e => e.Nombre).ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las empresas transportadoras dependiendo de un medio de transporte
        /// </summary>
        /// <param name="idMedioTransporte">Identificador del medio de transporte</param>
        /// <returns>Lista de empresas transportadoras</returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresaTransportadoraXMedioTransporte(int idMedioTransporte)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MedioTransEmpresaTrans_VRUT.Where(m => m.ETM_IdMedioTransporte == idMedioTransporte).ToList()
                  .ConvertAll<RUEmpresaTransportadora>(m => new RUEmpresaTransportadora()
                  {
                      IdEmpresaTransportadora = m.ETR_IdEmpresaTransportadora,
                      Nombre = m.ETR_Nombre,
                      IdMedioTransporte = idMedioTransporte
                  }).OrderBy(e => e.Nombre).ToList();
            }
        }

        /// <summary>
        /// Obtiene todas las empresas transportadoras dependiendo de un medio de transporte y el tipo de transporte
        /// </summary>
        /// <param name="idMedioTransporte">Identificador del medio de transporte</param>
        /// <returns>Lista de empresas transportadoras</returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresaTransportadoraXMedioTransporteXTipoTransporte(int idMedioTransporte, int tipoTransporte)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MedioTransEmpresaTrans_VRUT.Where(m => m.ETM_IdMedioTransporte == idMedioTransporte && m.ETR_IdTipoTransporte == tipoTransporte).ToList()
                  .ConvertAll<RUEmpresaTransportadora>(m => new RUEmpresaTransportadora()
                  {
                      IdEmpresaTransportadora = m.ETR_IdEmpresaTransportadora,
                      Nombre = m.ETR_Nombre,
                      IdMedioTransporte = m.ETM_IdMedioTransporte,
                      IdTipoTransporte = tipoTransporte
                  }).OrderBy(e => e.Nombre).ToList();
            }
        }

        /// <summary>
        /// Obtiene lista de tipos de transporte
        /// </summary>
        /// <returns>lista con los tipos de transporte</returns>
        public IList<RUTipoTransporte> ObtieneTodosTipoTransporte()
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoTransporte_RUT.ToList().ConvertAll<RUTipoTransporte>(t =>
                  new RUTipoTransporte()
                  {
                      IdTipoTransporte = t.TIT_IdTipoTransporte,
                      NombreTipoTransporte = t.TIT_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene  las ciudades que se manifiestan por una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idRuta">Id de la ruta por la cual se manifiestan las ciudades</param>
        /// <returns>Lista  con las ciudades que se manifiestan por una ruta</returns>
        public IList<RUCiudadManifestadaEnRuta> ObtenerCiudadesManifiestanEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<LocalidadAdicionalEnRuta_RUT>("LMR_IdRuta", idRuta.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarLocalidadAdicionalEnRuta_RUT(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().
                   ConvertAll<RUCiudadManifestadaEnRuta>(o =>
                     new RUCiudadManifestadaEnRuta()
                     {
                         CiudadManifiestaRuta = new PALocalidadDC()
                         {
                             Nombre = o.LMR_NombreLocalidadManifiesta,
                             IdLocalidad = o.LMR_IdLocalidadManifiesta,
                             EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                         },
                         IdCiudadManifiestaEnRuta = o.LMR_IdLocalidadManifiestEnRuta,
                         IdRuta = o.LMR_IdRuta,
                         GeneraConsolidado = o.LMR_GeneraConsolidado,
                         EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                     });
            }
        }

        /// <summary>
        /// Guarda una ciudad que se manifiesta en una ruta
        /// </summary>
        /// <param name="ciudad">Objeto con la informacion de la ciudad que se manifiesta en la ruta</param>
        public void AdicionarCiudadQueManifiestaenRuta(RUCiudadManifestadaEnRuta ciudad)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.LocalidadAdicionalEnRuta_RUT.Add(new LocalidadAdicionalEnRuta_RUT()
                {
                    LMR_CreadoPor = ControllerContext.Current.Usuario,
                    LMR_NombreLocalidadManifiesta = ciudad.CiudadManifiestaRuta.Nombre,
                    LMR_FechaGrabacion = DateTime.Now,
                    LMR_IdLocalidadManifiesta = ciudad.CiudadManifiestaRuta.IdLocalidad,
                    LMR_IdRuta = ciudad.IdRuta,
                    LMR_GeneraConsolidado = ciudad.GeneraConsolidado
                });
                AuditarLocalidadAdicionalEnRuta(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Borra una ciudad que manifiesta por una ruta
        /// </summary>
        /// <param name="ciudad"></param>
        public void EliminarCiudadQueManifiestaenRuta(RUCiudadManifestadaEnRuta ciudad)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LocalidadAdicionalEnRuta_RUT localidad = contexto.LocalidadAdicionalEnRuta_RUT.Where(obj => obj.LMR_IdLocalidadManifiestEnRuta == ciudad.IdCiudadManifiestaEnRuta).SingleOrDefault();
                if (localidad != null)
                {
                    contexto.LocalidadAdicionalEnRuta_RUT.Remove(localidad);

                    this.AuditarLocalidadAdicionalEnRuta(contexto);

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Audita las localidades adicionales de una ruta
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarLocalidadAdicionalEnRuta(ModeloRutas contexto)
        {
            contexto.Audit<LocalidadAdicionalEnRuta_RUT, LocalidadAdicionEnRutaHist_RUT>((record, action) => new LocalidadAdicionEnRutaHist_RUT()
            {
                LMR_CreadoPor = record.Field<LocalidadAdicionalEnRuta_RUT, string>(c => c.LMR_CreadoPor),
                LMR_FechaCambio = DateTime.Now,
                LMR_FechaGrabacion = record.Field<LocalidadAdicionalEnRuta_RUT, DateTime>(c => c.LMR_FechaGrabacion),
                LMR_CambiadoPor = ControllerContext.Current.Usuario,
                LMR_TipoCambio = action.ToString(),
                LMR_IdLocalidadManifiesta = record.Field<LocalidadAdicionalEnRuta_RUT, string>(c => c.LMR_IdLocalidadManifiesta),
                LMR_IdRuta = record.Field<LocalidadAdicionalEnRuta_RUT, int>(c => c.LMR_IdRuta)
            }, (cs) => contexto.LocalidadAdicionEnRutaHist_RUT.Add(cs));
        }

        /// <summary>
        /// Obtiene  la cobertura de una ciudad que se manifiestan por una ruta
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCiudadManifiestaRuta">Id de la localidadManifiestaenRuta</param>
        /// <returns>cobertura de una ciudad que se manifiesta en una ruta</returns>
        public IList<RUCoberturaCiudadManifiestaPorRuta> ObtenerCoberturaCiudadManifiestaEnRuta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idCiudadManifiestaRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<CoberturaManifiestaEnRuta_RUT>("CMR_IdLocalidadManifiestEnRuta", idCiudadManifiestaRuta.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarCoberturaManifiestaEnRuta_RUT(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().
                   ConvertAll<RUCoberturaCiudadManifiestaPorRuta>(o =>
                     new RUCoberturaCiudadManifiestaPorRuta()
                     {
                         CiudadHija = new PALocalidadDC()
                         {
                             Nombre = o.CMR_NombreLocalidadCubierta,
                             IdLocalidad = o.CMR_IdLocalidadCubierta,
                             EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                         },
                         IdCiudadManifiestaEnRuta = o.CMR_IdLocalidadManifiestEnRuta,
                         IdCoberturaManifiestaEnRuta = o.CMR_IdCoberturaManifiestaEnRut,
                         EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                     });
            }
        }

        /// <summary>
        /// adiciona la cobertura de una ciudad que manifiesta en ruta
        /// </summary>
        /// <param name="cobertura"></param>
        public void AdicionarCoberturaCiudadManifiestaEnRuta(RUCoberturaCiudadManifiestaPorRuta cobertura)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.CoberturaManifiestaEnRuta_RUT.Add(new CoberturaManifiestaEnRuta_RUT()
                {
                    CMR_CreadoPor = ControllerContext.Current.Usuario,
                    CMR_FechaGrabacion = DateTime.Now,
                    CMR_IdLocalidadCubierta = cobertura.CiudadHija.IdLocalidad,
                    CMR_IdLocalidadManifiestEnRuta = cobertura.IdCiudadManifiestaEnRuta,
                    CMR_NombreLocalidadCubierta = cobertura.CiudadHija.Nombre
                });
                AuditarCoberturaCiudadManifiestaEnRutaEnRuta(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Borra una ciudad cubierta por una ciudad que manifiesta por una ruta
        /// </summary>
        /// <param name="ciudad"></param>
        public void EliminarCoberturaCiudadManifiestaEnRuta(RUCoberturaCiudadManifiestaPorRuta ciudad)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CoberturaManifiestaEnRuta_RUT localidad = contexto.CoberturaManifiestaEnRuta_RUT.Where(obj => obj.CMR_IdCoberturaManifiestaEnRut == ciudad.IdCoberturaManifiestaEnRuta).SingleOrDefault();
                if (localidad != null)
                {
                    contexto.CoberturaManifiestaEnRuta_RUT.Remove(localidad);

                    this.AuditarLocalidadAdicionalEnRuta(contexto);

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Audita la cobertura de una ciudad manifestada en una ruta
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarCoberturaCiudadManifiestaEnRutaEnRuta(ModeloRutas contexto)
        {
            contexto.Audit<CoberturaManifiestaEnRuta_RUT, CoberturaManifieEnRutaHist_RUT>((record, action) => new CoberturaManifieEnRutaHist_RUT()
            {
                CMR_CreadoPor = record.Field<CoberturaManifiestaEnRuta_RUT, string>(c => c.CMR_CreadoPor),
                CMR_FechaCambio = DateTime.Now,
                CMR_FechaGrabacion = record.Field<CoberturaManifiestaEnRuta_RUT, DateTime>(c => c.CMR_FechaGrabacion),
                CMR_CambiadoPor = ControllerContext.Current.Usuario,
                CMr_TipoCambio = action.ToString(),
                CMR_IdCoberturaManifiestaEnRut = record.Field<CoberturaManifiestaEnRuta_RUT, int>(c => c.CMR_IdCoberturaManifiestaEnRut),
                CMR_IdLocalidadCubierta = record.Field<CoberturaManifiestaEnRuta_RUT, string>(c => c.CMR_IdLocalidadCubierta),
                CMR_IdLocalidadManifiestEnRuta = record.Field<CoberturaManifiestaEnRuta_RUT, int>(c => c.CMR_IdLocalidadManifiestEnRuta)
            }, (cs) => contexto.CoberturaManifieEnRutaHist_RUT.Add(cs));
        }

        /// <summary>
        /// Inserta una estacion de una ruta ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        public void AdicionarEstacionRuta(RUEstacionRuta estacionRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstacionRuta_RUT estacion = new EstacionRuta_RUT()
                {
                    ESR_CreadoPor = ControllerContext.Current.Usuario,
                    ESR_FechaGrabacion = DateTime.Now,
                    ESR_IdLocalidadEstacion = estacionRuta.CiudadEstacion.IdLocalidad,
                    ESR_IdRuta = estacionRuta.IdRuta,
                    ESR_NombreLocalidadEstacion = estacionRuta.CiudadEstacion.Nombre,
                    ESR_OrdenEnCasillero = (short)estacionRuta.OrdenEnCasillero,
                    ESR_OrdenEnRuta = (short)estacionRuta.OrdenEnRuta,
                    ESR_PermiteEnganche = estacionRuta.PermiteEnganche,
                    ESR_RequierePrecintoRetorno = estacionRuta.RequierePrecintoRetorno,
                    ESR_TiempoDeViajeDesdeAnterior = estacionRuta.TiempoViajeDesdeAnterior,
                    ESR_TiempoParada = estacionRuta.TiempoParada
                };

                contexto.EstacionRuta_RUT.Add(estacion);

                this.AdicionarFrecuenciaLlegadaEstacionRuta(estacionRuta, contexto);

                this.AdicionarCoberturaEstacionRuta(estacionRuta, contexto, estacion.ESR_IdEstacionRuta);

                this.AuditarEstacionRuta(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inserta la cobertura de la estacion ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        /// <param name="contexto"></param>
        /// <param name="estacion"></param>
        private void AdicionarCoberturaEstacionRuta(RUEstacionRuta estacionRuta, ModeloRutas contexto, int idEstacionRuta)
        {
            ///Inserta la frecuencia de parada en la estacion
            if (estacionRuta.CiudadesHijas != null)
            {
                estacionRuta.CiudadesHijas.ForEach(es =>
                {
                    contexto.CoberturaEstacionRuta_RUT.Add(new CoberturaEstacionRuta_RUT()
                    {
                        CER_CreadoPor = ControllerContext.Current.Usuario,
                        CER_FechaGrabacion = DateTime.Now,
                        CER_IdEstacionRuta = idEstacionRuta,
                        CER_IdLocalidadCubierta = es.EstacionHija.IdLocalidad,
                        CER_NombreLocalidadEstacion = es.EstacionHija.Nombre
                    });
                    this.AuditarCoberturaEstacionRuta(contexto);
                });
            }
        }

        /// <summary>
        /// Inserta la frecuencia de llegada a la estacion de ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        /// <param name="contexto"></param>
        /// <param name="estacion"></param>
        private void AdicionarFrecuenciaLlegadaEstacionRuta(RUEstacionRuta estacionRuta, ModeloRutas contexto)
        {
            ///Inserta la cobertura de la estacion
            if (estacionRuta.FrecuenciaLlegada != null)
            {
                estacionRuta.FrecuenciaLlegada.ForEach(es =>
                {
                    contexto.FrecuenciaParadaEstacion_RUT.Add(new FrecuenciaParadaEstacion_RUT()
                    {
                        FRE_CreadoPor = ControllerContext.Current.Usuario,
                        FRE_FechaGrabacion = DateTime.Now,
                        FRE_HoraLlegada = es.HoraLlegada,
                        FRE_IdDia = es.IdDia,
                        FRE_IdEstacionRuta = estacionRuta.IdEstacionRuta
                    });
                    this.AuditarFrecuenciaParadaEstacionRuta(contexto);
                });
            }
        }

        /// <summary>
        /// Elimina una estacion de una ruta, junto con su cobertura y su frecuencia
        /// </summary>
        /// <param name="estacionRuta"></param>
        public void EliminarEstacionRuta(RUEstacionRuta estacionRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstacionRuta_RUT estacion = contexto.EstacionRuta_RUT.Where(e => e.ESR_IdEstacionRuta == estacionRuta.IdEstacionRuta).SingleOrDefault();
                if (estacion == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_RUTAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    //Elimina la frecuencia
                    var frecuencia = contexto.FrecuenciaParadaEstacion_RUT.Where(obj => obj.FRE_IdEstacionRuta == estacionRuta.IdEstacionRuta).ToList();
                    for (int i = frecuencia.Count - 1; i >= 0; i--)
                    {
                        contexto.FrecuenciaParadaEstacion_RUT.Remove(frecuencia[i]);
                        this.AuditarFrecuenciaParadaEstacionRuta(contexto);
                    }

                    //Elimina la cobertura
                    var cobertura = contexto.CoberturaEstacionRuta_RUT.Where(e => e.CER_IdEstacionRuta == estacionRuta.IdEstacionRuta).ToList();
                    for (int i = cobertura.Count - 1; i >= 0; i--)
                    {
                        contexto.CoberturaEstacionRuta_RUT.Remove(cobertura[i]);
                        this.AuditarCoberturaEstacionRuta(contexto);
                    }

                    contexto.EstacionRuta_RUT.Remove(estacion);
                    this.AuditarEstacionRuta(contexto);

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Modifica una estacion ruta
        /// </summary>
        /// <param name="estacionRuta"></param>
        public void EditarEstacionRuta(RUEstacionRuta estacionRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstacionRuta_RUT estacion = contexto.EstacionRuta_RUT.Where(e => e.ESR_IdEstacionRuta == estacionRuta.IdEstacionRuta).SingleOrDefault();
                if (estacion == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_RUTAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    estacion.ESR_IdLocalidadEstacion = estacionRuta.CiudadEstacion.IdLocalidad;
                    estacion.ESR_IdRuta = estacionRuta.IdRuta;
                    estacion.ESR_NombreLocalidadEstacion = estacionRuta.CiudadEstacion.Nombre;
                    estacion.ESR_OrdenEnCasillero = (short)estacionRuta.OrdenEnCasillero;
                    estacion.ESR_OrdenEnRuta = (short)estacionRuta.OrdenEnRuta;
                    estacion.ESR_PermiteEnganche = estacionRuta.PermiteEnganche;
                    estacion.ESR_RequierePrecintoRetorno = estacionRuta.RequierePrecintoRetorno;
                    estacion.ESR_TiempoDeViajeDesdeAnterior = estacionRuta.TiempoViajeDesdeAnterior;
                    estacion.ESR_TiempoParada = estacionRuta.TiempoParada;

                    if (estacionRuta.FrecuenciaLlegada != null)
                    {
                        var horarios = contexto.FrecuenciaParadaEstacion_RUT.Where(obj => obj.FRE_IdEstacionRuta == estacionRuta.IdEstacionRuta).ToList();
                        for (int i = horarios.Count - 1; i >= 0; i--)
                        {
                            contexto.FrecuenciaParadaEstacion_RUT.Remove(horarios[i]);
                        }

                        this.AdicionarFrecuenciaLlegadaEstacionRuta(estacionRuta, contexto);
                    }

                    if (estacionRuta.CiudadesHijas != null)
                    {
                        estacionRuta.CiudadesHijas.ForEach(es =>
                          {
                              if (es.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                              {
                                  contexto.CoberturaEstacionRuta_RUT.Add(new CoberturaEstacionRuta_RUT()
                                  {
                                      CER_CreadoPor = ControllerContext.Current.Usuario,
                                      CER_FechaGrabacion = DateTime.Now,
                                      CER_IdEstacionRuta = estacionRuta.IdEstacionRuta,
                                      CER_IdLocalidadCubierta = es.EstacionHija.IdLocalidad,
                                      CER_NombreLocalidadEstacion = es.EstacionHija.Nombre
                                  });
                                  this.AuditarCoberturaEstacionRuta(contexto);
                              }
                              if (es.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                              {
                                  var cobertura = contexto.CoberturaEstacionRuta_RUT.Where(e => e.CER_IdCoberturaEstacionRuta == es.IdCoberturaEstacionRuta).SingleOrDefault();
                                  if (cobertura != null)
                                  {
                                      contexto.CoberturaEstacionRuta_RUT.Remove(cobertura);
                                      this.AuditarCoberturaEstacionRuta(contexto);
                                  }
                              }
                          });
                    }
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Audita la frecuencia de parada de la estacion de una ruta
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarFrecuenciaParadaEstacionRuta(ModeloRutas contexto)
        {
            contexto.Audit<FrecuenciaParadaEstacion_RUT, FrecuenciaParadaEstaciHist_RUT>((record, action) => new FrecuenciaParadaEstaciHist_RUT()
            {
                FRE_CreadoPor = record.Field<FrecuenciaParadaEstacion_RUT, string>(c => c.FRE_CreadoPor),
                FRR_FechaCambio = DateTime.Now,
                FRE_FechaGrabacion = record.Field<FrecuenciaParadaEstacion_RUT, DateTime>(c => c.FRE_FechaGrabacion),
                FRR_CambiadoPor = ControllerContext.Current.Usuario,
                FRR_TipoCambio = action.ToString(),
                FRE_HoraLlegada = record.Field<FrecuenciaParadaEstacion_RUT, DateTime>(c => c.FRE_HoraLlegada),
                FRE_IdDia = record.Field<FrecuenciaParadaEstacion_RUT, string>(c => c.FRE_IdDia),
                FRE_IdEstacionRuta = record.Field<FrecuenciaParadaEstacion_RUT, int>(c => c.FRE_IdEstacionRuta),
                FRE_IdFrecuenciaParadaEstacion = record.Field<FrecuenciaParadaEstacion_RUT, int>(c => c.FRE_IdFrecuenciaParadaEstacion)
            }, (cs) => contexto.FrecuenciaParadaEstaciHist_RUT.Add(cs));
        }

        /// <summary>
        /// Audita la cobertura de la estacion de una ruta
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarCoberturaEstacionRuta(ModeloRutas contexto)
        {
            contexto.Audit<CoberturaEstacionRuta_RUT, CoberturaEstacionRutaHist_RUT>((record, action) => new CoberturaEstacionRutaHist_RUT()
            {
                CER_CreadoPor = record.Field<CoberturaEstacionRuta_RUT, string>(c => c.CER_CreadoPor),
                CER_FechaCambio = DateTime.Now,
                CER_FechaGrabacion = record.Field<CoberturaEstacionRuta_RUT, DateTime>(c => c.CER_FechaGrabacion),
                CER_CambiadoPor = ControllerContext.Current.Usuario,
                CER_TipoCambio = action.ToString(),
                CER_IdCoberturaEstacionRuta = record.Field<CoberturaEstacionRuta_RUT, int>(c => c.CER_IdCoberturaEstacionRuta),
                CER_IdEstacionRuta = record.Field<CoberturaEstacionRuta_RUT, int>(c => c.CER_IdEstacionRuta),
                CER_IdLocalidadCubierta = record.Field<CoberturaEstacionRuta_RUT, string>(c => c.CER_IdLocalidadCubierta)
            }, (cs) => contexto.CoberturaEstacionRutaHist_RUT.Add(cs));
        }

        /// <summary>
        /// Audita estaciones de una ruta
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarEstacionRuta(ModeloRutas contexto)
        {
            contexto.Audit<EstacionRuta_RUT, EstacionRutaHist_RUT>((record, action) => new EstacionRutaHist_RUT()
            {
                ESR_CreadoPor = record.Field<EstacionRuta_RUT, string>(c => c.ESR_CreadoPor),
                ESR_FechaCambio = DateTime.Now,
                ESR_FechaGrabacion = record.Field<EstacionRuta_RUT, DateTime>(c => c.ESR_FechaGrabacion),
                ESR_CambiadoPor = ControllerContext.Current.Usuario,
                ESR_TipoCambio = action.ToString(),
                ESR_IdEstacionRuta = record.Field<EstacionRuta_RUT, int>(c => c.ESR_IdEstacionRuta),
                ESR_IdLocalidadEstacion = record.Field<EstacionRuta_RUT, string>(c => c.ESR_IdLocalidadEstacion),
                ESR_IdRuta = record.Field<EstacionRuta_RUT, int>(c => c.ESR_IdRuta),
                ESR_NombreLocalidadEstacion = record.Field<EstacionRuta_RUT, string>(c => c.ESR_NombreLocalidadEstacion),
                ESR_OrdenEnCasillero = record.Field<EstacionRuta_RUT, short>(c => c.ESR_OrdenEnCasillero),
                ESR_OrdenEnRuta = record.Field<EstacionRuta_RUT, short>(c => c.ESR_OrdenEnRuta),
                ESR_PermiteEnganche = record.Field<EstacionRuta_RUT, bool>(c => c.ESR_PermiteEnganche),
                ESR_RequierePrecintoRetorno = record.Field<EstacionRuta_RUT, bool>(c => c.ESR_RequierePrecintoRetorno),
                ESR_TiempoDeViajeDesdeAnterior = record.Field<EstacionRuta_RUT, decimal>(c => c.ESR_TiempoDeViajeDesdeAnterior),
                ESR_TiempoParada = record.Field<EstacionRuta_RUT, decimal>(c => c.ESR_TiempoParada)
            }, (cs) => contexto.EstacionRutaHist_RUT.Add(cs));
        }

        /// <summary>
        /// Insera una nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        public int AdicionarRuta(RURutaDC ruta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Ruta_RUT rut = new Ruta_RUT()
                {
                    RUT_CreadoPor = ControllerContext.Current.Usuario,
                    RUT_FechaGrabacion = DateTime.Now,
                    RUT_GeneraManifiestoMinisterio = ruta.GeneraManifiesto,
                    RUT_IdLocalidadDestino = ruta.CiudadDestino.IdLocalidad,
                    RUT_IdLocalidadOrigen = ruta.CiudadOrigen.IdLocalidad,
                    RUT_IdMedioTransporte = (short)ruta.IdMediotransporte,
                    RUT_IdTipoRuta = (short)ruta.IdTipoRuta,
                    RUT_Nombre = ruta.NombreRuta,
                    RUT_NombreLocalidadDestino = ruta.CiudadDestino.Nombre,
                    RUT_NombreLocalidadOrigen = ruta.CiudadOrigen.Nombre,
                    RUT_CostoMensualTotal = ruta.CostoMensualRuta,
                    RUT_IdTipoVehiculo = (short)ruta.IdTipoVehiculo,
                    RUT_Estado = ruta.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO
                };
                contexto.Ruta_RUT.Add(rut);

                if (ruta.FrecuenciaRuta != null)
                {
                    ruta.FrecuenciaRuta.ForEach(f =>
                      {
                          FrecuenciaRuta_RUT frecuencia = new FrecuenciaRuta_RUT()
                          {
                              FRR_CreadoPor = ControllerContext.Current.Usuario,
                              FRR_FechaGrabacion = DateTime.Now,
                              FRR_HoraLlegada = f.HoraLlegada,
                              FRR_HoraSalida = f.HoraSalida,
                              FRR_IdDia = f.IdDia,
                              FRR_IdRuta = rut.RUT_IdRuta
                          };

                          if (frecuencia.FRR_HoraLlegada <= frecuencia.FRR_HoraSalida)
                              frecuencia.FRR_HoraLlegada.AddDays(1);

                          contexto.FrecuenciaRuta_RUT.Add(frecuencia);
                      });
                }

                //agregar las empresas transportadoras
                if (ruta.EmpresasTransportadoras != null)
                    ruta.EmpresasTransportadoras.ForEach(c =>
                    {
                        contexto.RutaEmpresaTransportadora_RUT.Add(new RutaEmpresaTransportadora_RUT()
                        {
                            RET_IdEmpresaTransportadora = (short)c.IdEmpresaTransportadora,
                            RET_IdRuta = rut.RUT_IdRuta,
                            RET_CreadoPor = ControllerContext.Current.Usuario,
                            RET_FechaGrabacion = DateTime.Now
                        });
                    });

                this.AuditarRuta(contexto);

                contexto.SaveChanges();
                return rut.RUT_IdRuta;
            }
        }

        /// <summary>
        /// Modifica una ruta
        /// </summary>
        /// <param name="ruta"></param>
        public void EditarRuta(RURutaDC ruta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Ruta_RUT rut = contexto.Ruta_RUT.Where(e => e.RUT_IdRuta == ruta.IdRuta).SingleOrDefault();
                if (rut == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_RUTAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    rut.RUT_GeneraManifiestoMinisterio = ruta.GeneraManifiesto;
                    rut.RUT_IdLocalidadDestino = ruta.CiudadDestino.IdLocalidad;
                    rut.RUT_IdLocalidadOrigen = ruta.CiudadOrigen.IdLocalidad;
                    rut.RUT_IdTipoRuta = (short)ruta.IdTipoRuta;
                    rut.RUT_Nombre = ruta.NombreRuta;
                    rut.RUT_NombreLocalidadDestino = ruta.CiudadDestino.Nombre;
                    rut.RUT_NombreLocalidadOrigen = ruta.CiudadOrigen.Nombre;
                    rut.RUT_CostoMensualTotal = ruta.CostoMensualRuta;
                    rut.RUT_Estado = ruta.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;

                    if (ruta.FrecuenciaRuta != null && ruta.FrecuenciaRuta.Count > 0)
                    {
                        var frecuencia = contexto.FrecuenciaRuta_RUT.Where(obj => obj.FRR_IdRuta == ruta.IdRuta).ToList();
                        for (int i = frecuencia.Count - 1; i >= 0; i--)
                        {
                            contexto.FrecuenciaRuta_RUT.Remove(frecuencia[i]);
                        }
                        ruta.FrecuenciaRuta.ForEach(f =>
                        {
                            FrecuenciaRuta_RUT frecu = new FrecuenciaRuta_RUT()
                            {
                                FRR_CreadoPor = ControllerContext.Current.Usuario,
                                FRR_FechaGrabacion = DateTime.Now,
                                FRR_HoraLlegada = f.HoraLlegada,
                                FRR_HoraSalida = f.HoraSalida,
                                FRR_IdDia = f.IdDia,
                                FRR_IdRuta = rut.RUT_IdRuta
                            };

                            if (frecu.FRR_HoraLlegada <= frecu.FRR_HoraSalida)
                                frecu.FRR_HoraLlegada.AddDays(1);

                            contexto.FrecuenciaRuta_RUT.Add(frecu);
                        });
                    }

                    if (ruta.EstacionesHijas != null)
                    {
                        ruta.EstacionesHijas.ForEach(es =>
                          {
                              EstacionRuta_RUT estacion = contexto.EstacionRuta_RUT.Where(e => e.ESR_IdEstacionRuta == es.IdEstacionRuta).SingleOrDefault();
                              if (estacion != null)
                              {
                                  estacion.ESR_OrdenEnRuta = (short)es.OrdenEnRuta;
                              }
                          });
                    }

                    if (ruta.EmpresasTransportadoras != null)
                    {
                        ///Elimina los empresas transportadoras asociadas a la ruta
                        List<RutaEmpresaTransportadora_RUT> empresas = contexto.RutaEmpresaTransportadora_RUT.Where(a => a.RET_IdRuta == ruta.IdRuta).ToList();

                        for (int i = empresas.Count - 1; i >= 0; i--)
                        {
                            contexto.RutaEmpresaTransportadora_RUT.Remove(empresas[i]);
                        }

                        //agregar las empresas transportadoras
                        ruta.EmpresasTransportadoras.ForEach(c =>
                        {
                            contexto.RutaEmpresaTransportadora_RUT.Add(new RutaEmpresaTransportadora_RUT()
                            {
                                RET_IdEmpresaTransportadora = (short)c.IdEmpresaTransportadora,
                                RET_IdRuta = rut.RUT_IdRuta,
                                RET_CreadoPor = ControllerContext.Current.Usuario,
                                RET_FechaGrabacion = DateTime.Now
                            });
                        });
                    }

                    this.AuditarRuta(contexto);

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Audita las rutas
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarRuta(ModeloRutas contexto)
        {
            contexto.Audit<Ruta_RUT, RutaHist_RUT>((record, action) => new RutaHist_RUT()
            {
                RUT_CreadoPor = record.Field<Ruta_RUT, string>(c => c.RUT_CreadoPor),
                RUT_FechaCambio = DateTime.Now,
                RUT_FechaGrabacion = record.Field<Ruta_RUT, DateTime>(c => c.RUT_FechaGrabacion),
                RUT_CambiadoPor = ControllerContext.Current.Usuario,
                RUT_TipoCambio = action.ToString(),

                RUT_CostoMensualTotal = record.Field<Ruta_RUT, decimal>(c => c.RUT_CostoMensualTotal),
                RUT_GeneraManifiestoMinisterio = record.Field<Ruta_RUT, bool>(c => c.RUT_GeneraManifiestoMinisterio),
                RUT_IdLocalidadDestino = record.Field<Ruta_RUT, string>(c => c.RUT_IdLocalidadDestino),
                RUT_IdLocalidadOrigen = record.Field<Ruta_RUT, string>(c => c.RUT_IdLocalidadOrigen),

                //RUT_IdMedioTranEmpresa = record.Field<Ruta_RUT, short>(c => c.RUT_IdMedioTranEmpresa), !OJO!
                RUT_IdRuta = record.Field<Ruta_RUT, int>(c => c.RUT_IdRuta),
                RUT_IdTipoRuta = record.Field<Ruta_RUT, short>(c => c.RUT_IdTipoRuta),
                RUT_Nombre = record.Field<Ruta_RUT, string>(c => c.RUT_Nombre),
                RUT_NombreLocalidadDestino = record.Field<Ruta_RUT, string>(c => c.RUT_NombreLocalidadDestino),
                RUT_NombreLocalidadOrigen = record.Field<Ruta_RUT, string>(c => c.RUT_NombreLocalidadOrigen)
            }, (cs) => contexto.RutaHist_RUT.Add(cs));
        }

        /// <summary>
        /// Obtiene todas las rutas filtradas a partir de una localidad de orgen
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public IList<RURutaDC> ObtenerRutasXLocalidadOrigen(string idLocalidad)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Rutas_VRUT.Where(r => r.RUT_IdLocalidadOrigen == idLocalidad && r.RUT_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList().ConvertAll<RURutaDC>(r =>
                  new RURutaDC()
                  {
                      IdRuta = r.RUT_IdRuta,
                      NombreRuta = r.RUT_Nombre,
                      IdMediotransporte = r.RUT_IdMedioTransporte,
                      NombreMedioTransporte = r.MTR_Descripcion,
                      GeneraManifiesto = r.RUT_GeneraManifiestoMinisterio,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                      IdTipoRuta = r.TRU_IdTipoRuta
                  });
            }
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una ruta
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRuta(int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RutaEmpresaTrans_VRUT.Where(em => em.RET_IdRuta == idRuta).ToList().
                      ConvertAll<RUEmpresaTransportadora>(em => new RUEmpresaTransportadora()
                      {
                          IdEmpresaTransportadora = em.RET_IdEmpresaTransportadora,
                          Nombre = em.ETR_Nombre,
                          IdMedioTransporte = em.RUT_IdMedioTransporte,
                          IdTipoTransporte = em.TIT_IdTipoTransporte,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                      });
            }
        }

        /// <summary>
        /// Obtiene las empresas transportadoras asociadas a una racol
        /// </summary>
        /// <param name="idRuta">id de la ruta</param>
        /// <returns></returns>
        public IList<RUEmpresaTransportadora> ObtenerEmpresasTransportadorasXRacol(int idRacol)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EmpresaTransRacol_RUT.Include("EmpresaTransportadora_RUT").Include("EmpresaTransportadora_RUT.EmpresaTransporMedioTransp_RUT").Where(em => em.ETR_IdRegionalAdm == idRacol && em.EmpresaTransportadora_RUT.ETR_IdTipoTransporte == 2).OrderBy(o => o.EmpresaTransportadora_RUT.ETR_Nombre).
                    ToList().
                      ConvertAll<RUEmpresaTransportadora>(em => new RUEmpresaTransportadora()
                      {
                          IdEmpresaTransportadora = em.ETR_IdEmpresaTrans,
                          Nombre = em.EmpresaTransportadora_RUT.ETR_Nombre,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                      });
            }
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesYLocalidadesAdicionalesRuta(int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //1 = true. para la siguiente consulta se utiliza con numero porque es lo que espera el procedimiento
                return contexto.EstacionesYCiudAdiciRuta_VRUT.Where(e => e.IdRuta == idRuta && e.GeneraConsolidado == 1).OrdenarPor("orden").ToList().
                  ConvertAll<RUEstacionRuta>(e =>
                    {
                        RUEstacionRuta estacion = new RUEstacionRuta()
                        {
                            CiudadEstacion = new PALocalidadDC()
                            {
                                Nombre = e.NombreEstacion,
                                IdLocalidad = e.IdCiudadEstacion
                            },
                            IdRuta = e.IdRuta,
                            OrdenEnRuta = e.orden,
                            IdEstacionRuta = e.IdEstacion,
                        };
                        if (e.RequierePrecintoRetorno == 1)
                            estacion.RequierePrecintoRetorno = true;
                        else
                            estacion.RequierePrecintoRetorno = false;
                        if (e.GeneraConsolidado == 1)
                            estacion.GeneraConsolidado = true;
                        else
                            estacion.GeneraConsolidado = false;

                        return estacion;
                    }

                    );
            }
        }


        // TODO:ID Lista de las Estaciones-Ruta de un Manifiesto
        /// <summary>
        /// Obtiene las estaciones de la ruta de un Manifiesto
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRutaDeManifiesto(long IdManifiesto)
        {
            List<RUEstacionRuta> lista = new List<RUEstacionRuta>();

            DataSet dsRes = new DataSet();
            SqlDataAdapter da;

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstacionesRuta_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdManifiesto", IdManifiesto));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }

            DataRow[] dtFiltroNovedades; // Novedades ya asignadas
            RUEstacionRuta estacion;
            foreach (DataRow ite in dsRes.Tables[0].Rows)
            {
                estacion = new RUEstacionRuta()
                {
                    NomTabla = ite["Tabla"].ToString(),
                    IdEstacionRuta = Convert.ToInt32(ite["IdEstacion"]),

                    IdLocalidadEstacion = ite["IdCiudadEstacion"].ToString(),
                    NombreLocalidadEstacion = ite["NombreEstacion"].ToString(),
                    CantConsolidados = Convert.ToInt32(ite["CntConso"]),
                    CantSueltos = Convert.ToInt32(ite["CntSueltos"]),
                    Seleccionada = false,

                    TiempoParada = 1,
                    TiempoViajeDesdeAnterior = 1
                };

                dtFiltroNovedades = dsRes.Tables[1].Select("IdCiudadEstacion = " + estacion.IdLocalidadEstacion);

                string CadenaNovedades = string.Empty;
                foreach (DataRow iteRow in dtFiltroNovedades)
                {
                    CadenaNovedades += iteRow["NombreNovedad"].ToString() + "- " + Convert.ToDateTime(iteRow["NER_FechaNovedad"]).ToShortDateString() + "- " + iteRow["Tiempo"].ToString() + "; ";
                }

                estacion.StrNovedades = CadenaNovedades;

                lista.Add(estacion);
            }

            return lista;
        }




        /// <summary>
        /// Obtiene las ciudades que oertenecen a una ruta, incluye estaciones de la ruta ciudades hijas de la estacion, ciudades adicionales ciudades hijas de las adicionales y la ciudad destino de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las ciudades de una ruta</returns>
        public IList<PALocalidadDC> ObtenerTodasCiudadesEnRuta(int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //1 = true. para la siguiente consulta se utiliza con numero porque es lo que espera el procedimiento
                List<CiudadesRutas_VRUT> ciudades = contexto.CiudadesRutas_VRUT.Where(e => e.IdRuta == idRuta).ToList();
                List<PALocalidadDC> localidades = new List<PALocalidadDC>();

                localidades.AddRange(ciudades.Where(c => !string.IsNullOrEmpty(c.IdCiudadHija)).ToList().ConvertAll<PALocalidadDC>(l =>
                  new PALocalidadDC()
                  {
                      IdLocalidad = l.IdCiudadHija
                  }));

                localidades.AddRange(ciudades.Select(c => c.IdCiudadEstacion).Distinct().ToList().ConvertAll<PALocalidadDC>(l =>
                  new PALocalidadDC()
                  {
                      IdLocalidad = l.ToString()
                  }));

                return localidades;
            }
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son conolidado, incluye la localidad de origen de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesRuta(int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<RUEstacionRuta> origen = contexto.Ruta_RUT.Where(ruta => ruta.RUT_IdRuta == idRuta).ToList().
                  ConvertAll<RUEstacionRuta>(ee =>
                    new RUEstacionRuta()
                    {
                        CiudadEstacion = new PALocalidadDC()
                        {
                            Nombre = ee.RUT_NombreLocalidadOrigen,
                            IdLocalidad = ee.RUT_IdLocalidadOrigen
                        },
                        IdRuta = ee.RUT_IdRuta
                    });

                List<RUEstacionRuta> estaciones = contexto.EstacionesYCiudAdiciRuta_VRUT.Where(e => e.IdRuta == idRuta).OrdenarPor("orden").ToList().
                  ConvertAll<RUEstacionRuta>(e =>
                  {
                      RUEstacionRuta estacion = new RUEstacionRuta()
                      {
                          CiudadEstacion = new PALocalidadDC()
                          {
                              Nombre = e.NombreEstacion,
                              IdLocalidad = e.IdCiudadEstacion
                          },
                          IdRuta = e.IdRuta,
                          OrdenEnRuta = e.orden,
                          IdEstacionRuta = e.IdEstacion,
                      };
                      if (e.RequierePrecintoRetorno == 1)
                          estacion.RequierePrecintoRetorno = true;
                      else
                          estacion.RequierePrecintoRetorno = false;
                      if (e.GeneraConsolidado == 1)
                          estacion.GeneraConsolidado = true;
                      else
                          estacion.GeneraConsolidado = false;

                      return estacion;
                  });

                origen.AddRange(estaciones);

                return origen;
            }
        }

        /// <summary>
        /// Obtiene las estaciones y localidades adicionales de una ruta sin importar si son consolidado , ordenadas por el campo orden de la ruta
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns>Lista con las estaciones de la ruta</returns>
        public IList<RUEstacionRuta> ObtenerEstacionesCiudAdicionalesRuta(int idRuta)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<RUEstacionRuta> estaciones = contexto.EstacionesYCiudAdiciRuta_VRUT.Where(e => e.IdRuta == idRuta).OrdenarPor("orden").ToList().
                 ConvertAll<RUEstacionRuta>(e =>
                 {
                     RUEstacionRuta estacion = new RUEstacionRuta()
                     {
                         CiudadEstacion = new PALocalidadDC()
                         {
                             Nombre = e.NombreEstacion,
                             IdLocalidad = e.IdCiudadEstacion
                         },
                         IdRuta = e.IdRuta,
                         OrdenEnRuta = e.orden,
                         IdEstacionRuta = e.IdEstacion,
                         IdLocalidadEstacion = e.IdCiudadEstacion
                     };
                     if (e.RequierePrecintoRetorno == 1)
                         estacion.RequierePrecintoRetorno = true;
                     else
                         estacion.RequierePrecintoRetorno = false;
                     if (e.GeneraConsolidado == 1)
                         estacion.GeneraConsolidado = true;
                     else
                         estacion.GeneraConsolidado = false;

                     return estacion;
                 });

                return estaciones;
            }
        }

        #endregion Nacional

        #region Optimizacion


        private static List<Vertice> verticesTodos;

        /// <summary>
        /// Consulta todas las aristas que compone la red de rutas
        /// </summary>
        /// <param name="verticesRed">Listado de todos los vértices de la red</param>
        /// <returns>Listado de todas las aristas de la red según hayan sido configuradas</returns>
        public List<Arista> ConsultarAristasRutas(out List<Vertice> verticesRed)
        {
            if (verticesTodos == null)
            {
                using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    verticesTodos = contexto.Localidad_PAR.Select(loc => new Vertice() { IdVertice = loc.LOC_IdLocalidad, Descripcion = loc.LOC_Nombre }).ToList();
                }
            }

            List<Arista> aristas = new List<Arista>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAristasRutasGrafoCiudadesAdicionales_RUT", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();


                List<DataRow> lstTodasRutas = dt.AsEnumerable().ToList();
                List<DataRow> rutas = lstTodasRutas.GroupBy(r => r.Field<int>("RUT_IdRuta"))
                                .Select(r => r.First()).ToList();                

                verticesRed = new List<Vertice>(verticesTodos);

                foreach (DataRow ruta in rutas)
                {

                            int idRuta = ruta.Field<int>("RUT_IdRuta");

                    if(idRuta == 267 )
                    {


                    }

                            Ruta rut = new Ruta()
                            {
                                IdRuta = idRuta,
                                Descripcion = ruta.Field<string>("RUT_Nombre"),
                                MedioTransporte = ruta.Field<string>("MTR_Descripcion"),
                                TipoVehiculo = ruta.Field<string>("TIV_Descripcion")
                            };

                            List<DataRow> locAdicionalesRuta = lstTodasRutas.Where(r => r.Field<int>("RUT_IdRuta") == idRuta).ToList();

                            foreach (DataRow locAdicional in locAdicionalesRuta)
                            {

                                Arista aristaAdicional = new Arista()
                                {
                                    RutaArista = rut,
                                    VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == ruta.Field<string>("RUT_IdLocalidadDestino")),
                                    VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == locAdicional.Field<string>("LMR_IdLocalidadManifiesta")),
                                    TiempoParadaEstacionorigen = 0,
                                };

                                aristaAdicional.Frecuencias = new List<Frecuencia>();
                                for (int j = 1; j <= 7; j++)
                                {
                                    aristaAdicional.Frecuencias.Add(new Frecuencia()
                                    {
                                        Dia = j,
                                        HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                        HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                    });
                                }

                                if (aristaAdicional.VerticeOrigen != null && aristaAdicional.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaAdicional.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaAdicional.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                    aristas.Add(aristaAdicional);

                              /*  aristaAdicional = new Arista()
                                {
                                    RutaArista = rut,
                                    VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == locAdicional.Field<string>("LMR_IdLocalidadManifiesta")),
                                    VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == ruta.Field<string>("RUT_IdLocalidadDestino")),
                                    TiempoParadaEstacionorigen = 0,
                                };

                                aristaAdicional.Frecuencias = new List<Frecuencia>();
                                for (int j = 1; j <= 7; j++)
                                {
                                    aristaAdicional.Frecuencias.Add(new Frecuencia()
                                    {
                                        Dia = j,
                                        HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                        HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                    });
                                }

                                if (aristaAdicional.VerticeOrigen != null && aristaAdicional.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaAdicional.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaAdicional.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                    aristas.Add(aristaAdicional);
                                    */

                                List<DataRow> coberturaenRuta = lstTodasRutas.Where(r => r.Field<int>("RUT_IdRuta") == idRuta && locAdicional["LMR_IdLocalidadManifiestEnRuta"] != DBNull.Value && r["CMR_IdLocalidadManifiestEnRuta"] != DBNull.Value && r["CMR_IdLocalidadManifiestEnRuta"].ToString() == locAdicional["LMR_IdLocalidadManifiestEnRuta"].ToString()).ToList();

                               /* foreach (DataRow coberturaLocAdicional in coberturaenRuta)
                                {
                             Arista aristaCoberturaAdicional = new Arista()
                             {
                                 RutaArista = rut,
                                 VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == locAdicional.Field<string>("LMR_IdLocalidadManifiesta")),
                                 VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == coberturaLocAdicional.Field<string>("CMR_IdLocalidadCubierta")),
                                 TiempoParadaEstacionorigen = 0,
                             };
                             aristaCoberturaAdicional.Frecuencias = new List<Frecuencia>();
                             for (int j = 1; j <= 7; j++)
                             {
                                 aristaCoberturaAdicional.Frecuencias.Add(new Frecuencia()
                                 {
                                     Dia = j,
                                     HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                     HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                 });
                             }
                             if (aristaCoberturaAdicional.VerticeOrigen != null && aristaCoberturaAdicional.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaCoberturaAdicional.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaCoberturaAdicional.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                 aristas.Add(aristaCoberturaAdicional);
                             
                             aristaCoberturaAdicional = new Arista()
                                    {
                                        RutaArista = rut,
                                        VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == coberturaLocAdicional.Field<string>("CMR_IdLocalidadCubierta")),
                                        VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == locAdicional.Field<string>("LMR_IdLocalidadManifiesta")),
                                        TiempoParadaEstacionorigen = 0,
                                    };
                                    aristaCoberturaAdicional.Frecuencias = new List<Frecuencia>();
                                    for (int j = 1; j <= 7; j++)
                                    {
                                        aristaCoberturaAdicional.Frecuencias.Add(new Frecuencia()
                                        {
                                            Dia = j,
                                            HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                            HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                        });
                                    }
                                    if (aristaCoberturaAdicional.VerticeOrigen != null && aristaCoberturaAdicional.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaCoberturaAdicional.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaCoberturaAdicional.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                        aristas.Add(aristaCoberturaAdicional);
                                }*/
                            }

                            cmd = new SqlCommand("paObtenerAristasRutasGrafoEstacionesRuta_RUT", conn);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@IdRuta", idRuta);
                            da = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            conn.Open();
                            da.Fill(dt);
                            conn.Close();



                            List<DataRow> lstTodasEstacionesRuta = dt.AsEnumerable().ToList();
                            List<DataRow> estacionesRuta = lstTodasEstacionesRuta.GroupBy(r => r.Field<int>("ESR_IdEstacionRuta"))
                                            .Select(r => r.First()).ToList();


                            if (estacionesRuta.Count > 0)
                            {
                                for (int i = 0; i < estacionesRuta.Count; i++)
                                {
                                    string origen;
                                    string destino;
                                    origen = (i == 0) ? ruta.Field<string>("RUT_IdLocalidadOrigen") : estacionesRuta[i - 1].Field<string>("ESR_IdLocalidadEstacion");
                                    destino = estacionesRuta[i].Field<string>("ESR_IdLocalidadEstacion");

                                    Arista arista = new Arista()
                                    {
                                        RutaArista = rut,
                                        VerticeOrigen = (i == 0) ?
                                           verticesRed.FirstOrDefault(ver => ver.IdVertice == ruta.Field<string>("RUT_IdLocalidadOrigen"))
                                           :
                                          verticesRed.FirstOrDefault(ver => ver.IdVertice == estacionesRuta[i - 1].Field<string>("ESR_IdLocalidadEstacion")),

                                        VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == estacionesRuta[i].Field<string>("ESR_IdLocalidadEstacion")),

                                        TiempoParadaEstacionorigen = (i == 0) ? 0 : estacionesRuta[i - 1].Field<decimal>("ESR_TiempoParada") / 60
                                    };

                                    var frecuencias = lstTodasEstacionesRuta.Where(es => es["FRE_IdEstacionRuta"] != DBNull.Value && es["CER_IdCoberturaEstacionRuta"] != DBNull.Value && es.Field<int>("FRE_IdEstacionRuta") == estacionesRuta[i].Field<int>("ESR_IdEstacionRuta") && es.Field<int>("CER_IdCoberturaEstacionRuta") == estacionesRuta[i].Field<int>("CER_IdCoberturaEstacionRuta")).ToList();
                                    if (frecuencias != null)
                                    {

                                        arista.Frecuencias = frecuencias.Where(frec => frec["FRE_IdDia"] != DBNull.Value).Select(frec =>
                                        {
                                            DateTime horaLlegada = frec.Field<DateTime>("FRE_HoraLlegada");
                                            Frecuencia frecuencia = new Frecuencia()
                                            {
                                                Dia = int.Parse(frec.Field<string>("FRE_IdDia")),
                                                HoraLlegadaDestino = (horaLlegada.Year < DateTime.Now.Year) ? horaLlegada.AddYears(DateTime.Now.Year - horaLlegada.Year) : horaLlegada,
                                                HoraSalidaOrigen = (horaLlegada.Year < DateTime.Now.Year) ?
                                                     horaLlegada.AddYears(DateTime.Now.Year - horaLlegada.Year).AddHours(-decimal.ToDouble(estacionesRuta[i].Field<decimal>("ESR_TiempoDeViajeDesdeAnterior") / 60)) :
                                                     horaLlegada.AddHours(-decimal.ToDouble(estacionesRuta[i].Field<decimal>("ESR_TiempoDeViajeDesdeAnterior") / 60))
                                            };
                                            return frecuencia;

                                        }
                                    ).ToList();
                                    }


                                    if (arista.VerticeOrigen != null && arista.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == arista.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == arista.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                        aristas.Add(arista);


                                    List<DataRow> coberturas = lstTodasEstacionesRuta.Where(es => es["CER_IdEstacionRuta"] != DBNull.Value && es.Field<int>("CER_IdEstacionRuta") == estacionesRuta[i].Field<int>("ESR_IdEstacionRuta")).ToList();

                                    foreach (DataRow coberturaEstacion in coberturas)
                                    {
                                        Arista aristaEstacion = new Arista()
                                        {
                                            RutaArista = rut,
                                            VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == estacionesRuta[i].Field<string>("ESR_IdLocalidadEstacion")),
                                            VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == coberturaEstacion.Field<string>("CER_IdLocalidadCubierta")),
                                            TiempoParadaEstacionorigen = 0
                                        };
                                        aristaEstacion.Frecuencias = new List<Frecuencia>();
                                        for (int j = 1; j <= 7; j++)
                                        {
                                            aristaEstacion.Frecuencias.Add(new Frecuencia()
                                            {
                                                Dia = j,
                                                HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                                HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                            });
                                        }
                                        if (aristaEstacion.VerticeOrigen != null && aristaEstacion.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaEstacion.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaEstacion.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                            aristas.Add(aristaEstacion);

                                        aristaEstacion = new Arista()
                                        {
                                            RutaArista = rut,
                                            VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == coberturaEstacion.Field<string>("CER_IdLocalidadCubierta")),
                                            VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == estacionesRuta[i].Field<string>("ESR_IdLocalidadEstacion")),
                                            TiempoParadaEstacionorigen = 0
                                        };
                                        aristaEstacion.Frecuencias = new List<Frecuencia>();
                                        for (int j = 1; j <= 7; j++)
                                        {
                                            aristaEstacion.Frecuencias.Add(new Frecuencia()
                                            {
                                                Dia = j,
                                                HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                                HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                            });
                                        }

                                        if (aristaEstacion.VerticeOrigen != null && aristaEstacion.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaEstacion.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaEstacion.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                            aristas.Add(aristaEstacion);

                                    }

                                }

                                Arista aristaFin = new Arista()
                                {
                                    RutaArista = rut,
                                    VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == estacionesRuta[estacionesRuta.Count - 1].Field<string>("ESR_IdLocalidadEstacion")),
                                    VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == ruta.Field<string>("RUT_IdLocalidadDestino")),
                                    TiempoParadaEstacionorigen = 0
                                };

                                aristaFin.Frecuencias = new List<Frecuencia>();
                                for (int j = 1; j <= 7; j++)
                                {
                                    aristaFin.Frecuencias.Add(new Frecuencia()
                                    {
                                        Dia = j,
                                        HoraLlegadaDestino = DateTime.Now.Date.AddHours(9),
                                        HoraSalidaOrigen = DateTime.Now.Date.AddHours(8)
                                    });
                                }
                                if (aristaFin.VerticeOrigen != null && aristaFin.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == aristaFin.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == aristaFin.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                    aristas.Add(aristaFin);

                            }

                            else
                            {
                                Arista arista = new Arista()
                                {
                                    RutaArista = rut,
                                    VerticeOrigen = verticesRed.FirstOrDefault(ver => ver.IdVertice == ruta.Field<string>("RUT_IdLocalidadOrigen")),
                                    VerticeDestino = verticesRed.FirstOrDefault(ver => ver.IdVertice == ruta.Field<string>("RUT_IdLocalidadDestino")),
                                    TiempoParadaEstacionorigen = 0
                                };

                                var frecuencias = lstTodasRutas.Where(rutFrec => rutFrec.Field<int>("RUT_IdRuta") == idRuta && rutFrec["FRR_IdDia"] != DBNull.Value && rutFrec.Field<int>("FRR_IdRuta") == idRuta).ToList();

                                arista.Frecuencias = frecuencias.Select(frec =>
                                     new Frecuencia()
                                     {
                                         Dia = int.Parse(frec.Field<string>("FRR_IdDia")),
                                         HoraLlegadaDestino = frec.Field<DateTime>("FRR_HoraLlegada"),
                                         HoraSalidaOrigen = frec.Field<DateTime>("FRR_HoraSalida")
                                     }
                                 ).ToList();


                                if (arista.VerticeOrigen != null && arista.VerticeDestino != null && aristas.Where(ar => ar.VerticeOrigen.IdVertice == arista.VerticeOrigen.IdVertice && ar.VerticeDestino.IdVertice == arista.VerticeDestino.IdVertice).FirstOrDefault() == null)
                                    aristas.Add(arista);
                            }



                     
                }


              

            }
            return aristas;

        }

        #endregion Optimizacion

        #region Rutas y estaciones de rutas

        /// <summary>
        /// Obtiene las rutas filtradas por la ciudad de origen o destino
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idLocalidadAgencia"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasXCiudadOrigenDestino(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string idLocalidadAgencia)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idRuta;
                string nombreRuta;
                string localidadOrigenRuta;
                string localidadDestinoRuta;

                filtro.TryGetValue("RUT_IdRuta", out idRuta);
                filtro.TryGetValue("RUT_Nombre", out nombreRuta);
                filtro.TryGetValue("RUT_NombreLocalidadOrigen", out localidadOrigenRuta);
                filtro.TryGetValue("RUT_NombreLocalidadDestino", out localidadDestinoRuta);

                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();

                LambdaExpression lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_IdLocalidadDestino", idLocalidadAgencia, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.Or);

                lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_IdLocalidadOrigen", idLocalidadAgencia, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.Or);
                lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                if (!string.IsNullOrEmpty(idRuta))
                {
                    lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_IdRuta", idRuta, OperadorComparacion.Equal);
                    where.Add(lambda, OperadorLogico.And);
                }

                if (!string.IsNullOrEmpty(nombreRuta))
                {
                    lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_Nombre", nombreRuta, OperadorComparacion.Contains);
                    where.Add(lambda, OperadorLogico.And);
                }

                if (!string.IsNullOrEmpty(localidadOrigenRuta))
                {
                    lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_NombreLocalidadOrigen", localidadOrigenRuta, OperadorComparacion.Contains);
                    where.Add(lambda, OperadorLogico.And);
                }

                if (!string.IsNullOrEmpty(localidadDestinoRuta))
                {
                    lambda = contexto.CrearExpresionLambda<Rutas_VRUT>("RUT_NombreLocalidadDestino", localidadDestinoRuta, OperadorComparacion.Contains);
                    where.Add(lambda, OperadorLogico.And);
                }

                filtro.Clear();

                return contexto.ConsultarRutas_VRUT(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)

                  //  .Where(r => r.RUT_IdLocalidadDestino == idLocalidadAgencia || r.RUT_IdLocalidadOrigen == idLocalidadAgencia)
                  .ToList()
                .ConvertAll(r => new RURutaDC()
                {
                    IdLocalidadDestino = r.RUT_IdLocalidadDestino,
                    NombreLocalidadDestino = r.RUT_NombreLocalidadDestino,
                    IdLocalidadOrigen = r.RUT_IdLocalidadOrigen,
                    NombreLocalidadOrigen = r.RUT_NombreLocalidadOrigen,
                    IdRuta = r.RUT_IdRuta,
                    NombreRuta = r.RUT_Nombre
                });
            }
        }

        #endregion Rutas y estaciones de rutas

        /// <summary>
        /// Obtiene las rutas de una localidad si el idlocalida es null  trae todas las rutas
        /// </summary>
        /// <param name="idLocalidad"></param>
        public List<RURutaDC> ObtenerRutasPorLocalidad(string idLocalidad)
        {
            SqlDataReader rdr = null;
            List<RURutaDC> listaRutas = new List<RURutaDC>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasPorLocalidad_CAC", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idLocalidad", idLocalidad));
                rdr = cmd.ExecuteReader();

                while(rdr.Read())
                {
                    RURutaDC rut = new RURutaDC()
                    {
                        IdLocalidadDestino = Convert.ToString(rdr["RUT_IdLocalidadDestino"]),
                        NombreLocalidadDestino = Convert.ToString(rdr["RUT_NombreLocalidadDestino"]),
                        IdLocalidadOrigen = Convert.ToString(rdr["RUT_IdLocalidadOrigen"]),
                        NombreLocalidadOrigen = Convert.ToString(rdr["RUT_NombreLocalidadOrigen"]),
                        IdRuta = (int)rdr["RUT_IdRuta"],
                        NombreRuta = Convert.ToString(rdr["RUT_Nombre"])
                    };
                    listaRutas.Add(rut);
                }
                sqlConn.Close();
            }

            return listaRutas;

            //using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    var retorno = contexto.RutasEstacionRuta_VRUT.Where(ruta =>
            //      (ruta.RUT_IdLocalidadOrigen == (idLocalidad != null ? idLocalidad : ruta.RUT_IdLocalidadOrigen) ||
            //      ruta.RUT_IdLocalidadDestino == (idLocalidad != null ? idLocalidad : ruta.RUT_IdLocalidadDestino))
            //      && ruta.RUT_Estado == ConstantesFramework.ESTADO_ACTIVO);

            //    if (retorno != null)
            //    {
            //        return retorno.ToList().ConvertAll(r => new RURutaDC()
            //        {
            //            IdLocalidadDestino = r.RUT_IdLocalidadDestino,
            //            NombreLocalidadDestino = r.RUT_NombreLocalidadDestino,
            //            IdLocalidadOrigen = r.RUT_IdLocalidadOrigen,
            //            NombreLocalidadOrigen = r.RUT_NombreLocalidadOrigen,
            //            IdRuta = r.RUT_IdRuta,
            //            NombreRuta = r.RUT_Nombre
            //        });
            //    }
            //    else
            //    {
            //       return new List<RURutaDC>();
            //    }
            //}
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacion(string idLocalidadEstacion)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int dia = (int)DateTime.Now.DayOfWeek;
                if (dia == 0)
                    dia = 7;

                return contexto.paObtenerRutasEstacionRuta_RUT(idLocalidadEstacion, dia.ToString()).ToList().ConvertAll<RURutaDC>(r =>
                new RURutaDC()
                {
                    IdLocalidadDestino = r.RUT_IdLocalidadDestino,
                    NombreLocalidadDestino = r.RUT_NombreLocalidadDestino,
                    IdLocalidadOrigen = r.RUT_IdLocalidadOrigen,
                    NombreLocalidadOrigen = r.RUT_NombreLocalidadOrigen,
                    IdRuta = r.RUT_IdRuta,
                    NombreRuta = r.RUT_Nombre,
                    IdMediotransporte = r.RUT_IdMedioTransporte,
                    NombreMedioTransporte = r.MTR_Descripcion,
                    GeneraManifiesto = r.RUT_GeneraManifiestoMinisterio,
                    EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                    IdTipoRuta = r.TRU_IdTipoRuta,
                    EsRutaMasivos = r.RUT_RutaMasivos
                });
            }
        }

        /// <summary>
        /// Obtiene las rutas a las cuales pertenece una estacion, incluye las rutas en las que la estacion es origen y destino
        /// </summary>
        /// <param name="idLocalidadEstacion"></param>
        /// <returns></returns>
        public List<RURutaDC> ObtenerRutasPerteneceEstacionOrigDest(string idLocalidadEstacion)
        {
            List<RURutaDC> listaRu = new List<RURutaDC>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerRutasEstacionRutaOriDest_RUT", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdLocalidadEstacion", idLocalidadEstacion));
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    RURutaDC rut = new RURutaDC()
                    {
                        IdLocalidadDestino = Convert.ToString(rdr["RUT_IdLocalidadDestino"]),
                        NombreLocalidadDestino = Convert.ToString(rdr["RUT_NombreLocalidadDestino"]),
                        IdLocalidadOrigen = Convert.ToString(rdr["RUT_IdLocalidadOrigen"]),
                        NombreLocalidadOrigen = Convert.ToString(rdr["RUT_NombreLocalidadOrigen"]),
                        IdRuta = Convert.ToInt16( rdr["RUT_IdRuta"]),
                        NombreRuta = Convert.ToString(rdr["RUT_Nombre"]) + " " + Convert.ToString(rdr["RUT_IdRuta"]),
                        IdMediotransporte = Convert.ToInt16(rdr["RUT_IdMedioTransporte"]),
                        NombreMedioTransporte = Convert.ToString(rdr["MTR_Descripcion"]),
                        GeneraManifiesto = (bool)rdr["RUT_GeneraManifiestoMinisterio"],
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        IdTipoRuta = Convert.ToInt16(rdr["TRU_IdTipoRuta"])
                    };
                    listaRu.Add(rut);
                }
                sqlConn.Close();
            }
            return listaRu;
        }

        /// <summary>
        /// Obtener una ruta
        /// </summary>
        /// <param name="idLocalidadOrigen">Ciudad origen</param>
        /// <param name="idLocalidadDestino">Ciudad Destino</param>
        /// <returns>La ruta encontrada</returns>
        public RURutaDC ObtenerRuta(string idLocalidadOrigen, string idLocalidadDestino)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RURutaDC infoRuta = null;
                Ruta_RUT ruta = contexto.Ruta_RUT.FirstOrDefault(rut => rut.RUT_IdLocalidadOrigen == idLocalidadOrigen
                                                                    && rut.RUT_IdLocalidadDestino == idLocalidadDestino);
                if (ruta != null)
                {
                    infoRuta = new RURutaDC()
                    {
                        IdLocalidadDestino = ruta.RUT_IdLocalidadDestino,
                        NombreLocalidadDestino = ruta.RUT_NombreLocalidadDestino,
                        IdLocalidadOrigen = ruta.RUT_IdLocalidadOrigen,
                        NombreLocalidadOrigen = ruta.RUT_NombreLocalidadOrigen,
                        IdRuta = ruta.RUT_IdRuta,
                        NombreRuta = ruta.RUT_Nombre,
                        IdMediotransporte = ruta.RUT_IdMedioTransporte,
                        GeneraManifiesto = ruta.RUT_GeneraManifiestoMinisterio,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                    };
                }
                return infoRuta;
            }
        }

        #region Empresa Transportadora

        #region Consulta

        /// <summary>
        /// Metodo que consulta las empresas
        /// transportadoras
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns>lista de empresas transportadoras</returns>
        public List<RUEmpresaTransportadora> ObtenerEmpresaTransportadora(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda;

                string estado;
                filtro.TryGetValue("ERT_Estado", out estado);
                if (!String.IsNullOrWhiteSpace(estado))
                {
                    //if (estado == ConstantesFramework.ESTADO_INACTIVO)
                    //    estado = ConstantesFramework.ESTADO_ACTIVO;

                    lamda = contexto.CrearExpresionLambda<EmpresaTransportadora_RUT>("ERT_Estado", estado, OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }
                else
                {
                    lamda = contexto.CrearExpresionLambda<EmpresaTransportadora_RUT>("ERT_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }

                return contexto.ConsultarEmpresaTransportadora_RUT(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                        .ToList()
                        .ConvertAll(emp => new RUEmpresaTransportadora
                        {
                            IdEmpresaTransportadora = emp.ETR_IdEmpresaTransportadora,
                            Nombre = emp.ETR_Nombre,
                            TipoTransporte = new RUTipoTransporte()
                            {
                                IdTipoTransporte = emp.ETR_IdTipoTransporte,
                                NombreTipoTransporte = ConvertirTipoTransporte(emp.ETR_IdTipoTransporte).ToString()
                            },
                            EstadoEmpresa = emp.ERT_Estado,
                            FechaGrabacion = emp.ETR_FechaGrabacion,
                            CreadoPor = emp.ETR_CreadoPor,
                            LstMediosTransporte = contexto.EmpresasTransportadorasMedioTransporte_VRUT
                                                .Where(med => med.ETM_IdEmpresaTransportadora == emp.ETR_IdEmpresaTransportadora)
                                                .ToList().ConvertAll<PAMedioTransporte>(op => new PAMedioTransporte()
                                                {
                                                    IdMedioTransporte = op.MTR_IdMedioTransporte,
                                                    NombreMedioTransporte = op.MTR_Descripcion,
                                                    Asignado = true
                                                }),
                            LstRacolAsociados = contexto.EmpresasTransportadorasRacolAsociado_VRUT.Where(rac => rac.ETR_IdEmpresaTrans == emp.ETR_IdEmpresaTransportadora)
                                                .ToList().ConvertAll<PURegionalAdministrativa>(cen => new PURegionalAdministrativa()
                                                {
                                                    IdRegionalAdmin = cen.ETR_IdRegionalAdm,
                                                    Descripcion = cen.REA_Descripcion
                                                }),
                            LstRacolDisponibles = contexto.paObtenerRacolsAsignarAEmpresaTransp_CPO(emp.ETR_IdEmpresaTransportadora)
                                                .ToList()
                                                .ConvertAll<PURegionalAdministrativa>(cen => new PURegionalAdministrativa()
                                                {
                                                    IdRegionalAdmin = cen.REA_IdRegionalAdm,
                                                    Descripcion = cen.REA_Descripcion
                                                })
                        });
            }
        }

        #endregion Consulta

        #region Adicionar

        /// <summary>
        /// Adiciona una Nueva empresa Tranportadora
        /// </summary>
        /// <param name="adicionarEmpresaTrans">Info de la Nva Empresa</param>
        public void AdicionarEmpresaTransportadora(RUEmpresaTransportadora adicionarEmpresaTrans)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (adicionarEmpresaTrans != null)
                {
                    EmpresaTransportadora_RUT nvaEmpresa = new EmpresaTransportadora_RUT()
                    {
                        ETR_Nombre = adicionarEmpresaTrans.Nombre,
                        ETR_IdTipoTransporte = (short)adicionarEmpresaTrans.TipoTransporte.IdTipoTransporte,
                        ERT_Estado = adicionarEmpresaTrans.EstadoEmpresa,
                        ETR_FechaGrabacion = DateTime.Now,
                        ETR_CreadoPor = ControllerContext.Current.Usuario,
                    };
                    contexto.EmpresaTransportadora_RUT.Add(nvaEmpresa);
                    contexto.SaveChanges();

                    //Adiciono Racoles Asociados si existen
                    if (adicionarEmpresaTrans.LstRacolAsociados != null && adicionarEmpresaTrans.LstRacolAsociados.Count > 0)
                    {
                        adicionarEmpresaTrans.LstRacolAsociados.ForEach(emp =>
                        {
                            if (emp.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                            {
                                EmpresaTransRacol_RUT nvaRacolAsociada = new EmpresaTransRacol_RUT()
                                {
                                    ETR_IdEmpresaTrans = nvaEmpresa.ETR_IdEmpresaTransportadora,
                                    ETR_IdRegionalAdm = emp.IdRegionalAdmin,
                                    ETR_FechaGrabacion = DateTime.Now,
                                    ETR_CreadoPor = ControllerContext.Current.Usuario
                                };
                                contexto.EmpresaTransRacol_RUT.Add(nvaRacolAsociada);
                                contexto.SaveChanges();
                            }
                        });
                    }

                    //Adiciono los Medios de Transporte
                    if (adicionarEmpresaTrans.LstMediosTransporte != null && adicionarEmpresaTrans.LstMediosTransporte.Count > 0)
                    {
                        adicionarEmpresaTrans.LstMediosTransporte.ForEach(med =>
                        {
                            EmpresaTransporMedioTransp_RUT nvaEmpreMedioTrans = new EmpresaTransporMedioTransp_RUT()
                            {
                                ETM_IdEmpresaTransportadora = Convert.ToInt16(nvaEmpresa.ETR_IdEmpresaTransportadora),
                                ETM_IdMedioTransporte = Convert.ToInt16(med.IdMedioTransporte),
                                ETM_FechaGrabacion = DateTime.Now,
                                ETM_CreadoPor = ControllerContext.Current.Usuario
                            };
                            contexto.EmpresaTransporMedioTransp_RUT.Add(nvaEmpreMedioTrans);
                            contexto.SaveChanges();
                        });
                    }
                }
            }
        }

        #endregion Adicionar

        #region Actualizar

        /// <summary>
        /// Actualiza la Empresa Transportadora
        /// </summary>
        /// <param name="actualizarEmpresaTrans"></param>
        public void ActualizarEmpresaTransportadora(RUEmpresaTransportadora actualizarEmpresaTrans)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (actualizarEmpresaTrans != null && actualizarEmpresaTrans.IdEmpresaTransportadora != 0)
                {
                    EmpresaTransportadora_RUT updateEmpresa = contexto.EmpresaTransportadora_RUT.FirstOrDefault(id => id.ETR_IdEmpresaTransportadora == actualizarEmpresaTrans.IdEmpresaTransportadora);
                    if (updateEmpresa != null)
                    {
                        updateEmpresa.ETR_Nombre = actualizarEmpresaTrans.Nombre;
                        updateEmpresa.ETR_IdTipoTransporte = (short)actualizarEmpresaTrans.TipoTransporte.IdTipoTransporte;
                        updateEmpresa.ERT_Estado = actualizarEmpresaTrans.EstadoEmpresa;
                    }
                    RURepositorioAuditoria.MapeoAuditEmpresaTransportadora(contexto);
                    contexto.SaveChanges();

                    #region Actualizar Racols Asociados

                    //Borro los Racoles Asociados a la Empresa
                    List<EmpresaTransRacol_RUT> borradoRacolsEmpresa = contexto.EmpresaTransRacol_RUT.Where(emp => emp.ETR_IdEmpresaTrans == actualizarEmpresaTrans.IdEmpresaTransportadora).ToList();
                    if (borradoRacolsEmpresa != null && borradoRacolsEmpresa.Count > 0)
                    {
                        borradoRacolsEmpresa.ForEach(bor =>
                        {
                            contexto.EmpresaTransRacol_RUT.Remove(bor);
                            contexto.SaveChanges();
                        });
                    }

                    //Agrego Racoles Asociados si existen
                    if (actualizarEmpresaTrans.LstRacolAsociados.Count > 0)
                    {
                        actualizarEmpresaTrans.LstRacolAsociados.ForEach(emp =>
                        {
                            EmpresaTransRacol_RUT nvaRacolAsociada = new EmpresaTransRacol_RUT()
                            {
                                ETR_IdEmpresaTrans = Convert.ToInt16(actualizarEmpresaTrans.IdEmpresaTransportadora),
                                ETR_IdRegionalAdm = emp.IdRegionalAdmin,
                                ETR_FechaGrabacion = DateTime.Now,
                                ETR_CreadoPor = ControllerContext.Current.Usuario
                            };
                            contexto.EmpresaTransRacol_RUT.Add(nvaRacolAsociada);
                            contexto.SaveChanges();
                        });
                    }

                    #endregion Actualizar Racols Asociados

                    #region Actualizar Medios Transporte

                    ///Consulto los medios de Transporte Asociados a la empresa
                    List<EmpresaTransporMedioTransp_RUT> actMedioTransporte = contexto.EmpresaTransporMedioTransp_RUT
                                                                .Where(m => m.ETM_IdEmpresaTransportadora == actualizarEmpresaTrans.IdEmpresaTransportadora).ToList();

                    if (actMedioTransporte != null && actMedioTransporte.Count > 0)
                    {
                        actMedioTransporte.ForEach(medioBorrar =>
                        {
                            //borro cada uno
                            contexto.EmpresaTransporMedioTransp_RUT.Remove(medioBorrar);
                            contexto.SaveChanges();
                        });
                    }

                    //Adiciono los nuevos Medios de Transporte
                    if (actualizarEmpresaTrans.LstMediosTransporte != null && actualizarEmpresaTrans.LstMediosTransporte.Count > 0)
                    {
                        actualizarEmpresaTrans.LstMediosTransporte.ForEach(nvMedio =>
                        {
                            EmpresaTransporMedioTransp_RUT nvaEmpreMedioTrans = new EmpresaTransporMedioTransp_RUT()
                            {
                                ETM_IdEmpresaTransportadora = Convert.ToInt16(actualizarEmpresaTrans.IdEmpresaTransportadora),
                                ETM_IdMedioTransporte = Convert.ToInt16(nvMedio.IdMedioTransporte),
                                ETM_FechaGrabacion = DateTime.Now,
                                ETM_CreadoPor = ControllerContext.Current.Usuario
                            };
                            contexto.EmpresaTransporMedioTransp_RUT.Add(nvaEmpreMedioTrans);
                            contexto.SaveChanges();
                        });
                    }

                    #endregion Actualizar Medios Transporte
                }
            }
        }

        #endregion Actualizar

        #endregion Empresa Transportadora

        /// <summary>
        /// Consulta un parámetro de asociado al módulo de rutas
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ConsultarParametrosRutas(string key)
        {
            using (ModeloRutas contexto = new ModeloRutas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ParametrosRutas_RUT.Where(p => p.PRU_IdParametro == key).FirstOrDefault().PRU_ValorParametro;
            }
        }

        #region Enumerables

        /// <summary>
        /// Convierte el estado del giro.
        /// </summary>
        /// <param name="estado">valor actual del estado.</param>
        /// <returns>la palabra del Estado</returns>
        internal RUEnumTipoTransporte ConvertirTipoTransporte(int transporte)
        {
            switch (transporte)
            {
                case 1:
                    return RUEnumTipoTransporte.Propio;

                default:
                    return RUEnumTipoTransporte.Aforado;
            }
        }

        #endregion Enumerables
    }
}