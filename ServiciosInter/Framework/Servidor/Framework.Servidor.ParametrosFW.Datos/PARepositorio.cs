using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW.Datos.Modelo;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Versionamiento;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Transactions;



namespace Framework.Servidor.ParametrosFW.Datos
{
    /// <summary>
    /// Clase encargada de interactuar con la base de datos
    /// </summary>
    public class PARepositorio
    {
        private string NombreModelo = "ModeloParametros";
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        public static readonly PARepositorio Instancia = new PARepositorio();
        private static Dictionary<string, string> ParametrosFrameworkCache = null;
        private static DateTime fechaHoraCacheParametros = DateTime.Now; 
        /// <summary>
        /// Constructor
        /// </summary>
        private PARepositorio()
        {
        }

        #region CONSULTAS

        /// <summary>
        /// Retorna el valor del dólar parametrizado en la tabla ParámetrosFramework
        /// </summary>
        /// <returns></returns>
        public double? ObtenerValorDolarEnPesos()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFramework valorDolar = contexto.ParametrosFramework.FirstOrDefault(p => p.PAR_IdParametro == "ValorDolar");
                double valor = 0;
                if (valorDolar != null && double.TryParse(valorDolar.PAR_ValorParametro, out valor))
                {
                    return valor;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Retorna el valor del parametro para el movimiento de la caja en Controller
        /// </summary>
        /// <returns></returns>
        public bool ObtenerValorMovCajaController()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFramework valorMovCajaController = contexto.ParametrosFramework.FirstOrDefault(p => p.PAR_IdParametro == "MovCajaController");
                bool valor = false;
                if (valorMovCajaController != null && bool.TryParse(valorMovCajaController.PAR_ValorParametro, out valor))
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
        /// Retorna el valor del parametro para el movimiento de la caja en API
        /// </summary>
        /// <returns></returns>
        public bool ObtenerValorMovCajaApi()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFramework valorMovCajaApi = contexto.ParametrosFramework.FirstOrDefault(p => p.PAR_IdParametro == "MovCajaApi");
                bool valor = false;
                if (valorMovCajaApi != null && bool.TryParse(valorMovCajaApi.PAR_ValorParametro, out valor))
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
        /// Obtiene todos los medios de transporte
        /// </summary>
        /// <returns>Lista con los medios de transporte</returns>
        public IList<PAMedioTransporte> ObtenerTodosMediosTrasporte()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MedioTransporte_PAR.ToList().ConvertAll<PAMedioTransporte>(m =>
                  new PAMedioTransporte()
                  {
                      IdMedioTransporte = m.MTR_IdMedioTransporte,
                      NombreMedioTransporte = m.MTR_Descripcion
                  });
            }
        }

        /// <summary>
        /// Valida si una identificacion se encuentra en listas restrictivas
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public bool ValidarListaRestrictiva(string identificacion)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaNegra_PAR lista = contexto.ListaNegra_PAR.FirstOrDefault(l => l.LIN_Identificacion == identificacion && l.LIN_Estado == ConstantesFramework.ESTADO_ACTIVO);
                if (lista != null)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Consulta la informacion de la alerta para el envio de correo electronico
        /// </summary>
        /// <param name="idAlerta"></param>
        /// <returns></returns>
        public InformacionAlerta ConsultarInformacionAlerta(int idAlerta)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var informacion =
                  contexto.TipoAlertaDestinatarios_VPAR
                  .Where(ta => ta.TIA_IdTipoAlerta == idAlerta)
                  .GroupBy(g => g.TIA_IdTipoAlerta, (key, contenido)
                    => new
                    {
                        Asunto = contenido.FirstOrDefault().TIA_Asunto,
                        Mensaje = contenido.FirstOrDefault().TIA_TemplateMensaje,
                        Destinatario = contenido.Select(c => c.PEI_Email)
                    }).FirstOrDefault();

                if (informacion != null)
                {
                    return new InformacionAlerta()
                    {
                        Asunto = informacion.Asunto,
                        Mensaje = informacion.Mensaje,
                        Destinatario = string.Join(", ", informacion.Destinatario)
                    };
                }
                else
                {
                    return
                      contexto.TipoAlerta_PAR
                      .Where(t => t.TIA_IdTipoAlerta == idAlerta).ToList()
                      .ConvertAll<InformacionAlerta>(t =>
                      new InformacionAlerta()
                      {
                          Asunto = t.TIA_Asunto,
                          Mensaje = t.TIA_TemplateMensaje
                      }).FirstOrDefault();
                }
            }
        }

        public InformacionAlerta ConsultarInformacionEnvioCorreo(int idAlerta)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoAlerta_PAR.Where(tip => tip.TIA_IdTipoAlerta == idAlerta)
                  .ToList()
                  .ConvertAll<InformacionAlerta>(s => new InformacionAlerta
                  {
                      Asunto = s.TIA_Asunto,
                      Mensaje = s.TIA_TemplateMensaje
                  }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosFramework(string llave)
        {
            PrepararParametrosFrameworkCache();

            if (PARepositorio.ParametrosFrameworkCache != null)
            {
               if(PARepositorio.ParametrosFrameworkCache.ContainsKey(llave))
                {
                    return PARepositorio.ParametrosFrameworkCache[llave];
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                           ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO.ToString(),
                                            MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(CadCnxController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerParametrosFramework_PAR", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PAR_IdParametro", llave);
                    conn.Open();
                    var parametro = cmd.ExecuteScalar();
                    conn.Close();

                    if (parametro != null)
                    {
                        return Convert.ToString(parametro);
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                            ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO.ToString(),
                                             MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO));
                        throw new FaultException<ControllerException>(excepcion);
                    }

                }
            }

        }
        /// <summary>
        /// Levanta diccionario a memoria con todos los parametrosFramework
        /// </summary>
        private void PrepararParametrosFrameworkCache()
        {
            ///Contrala que cada 10 minutos se refresque la cache
            if (Math.Abs((PARepositorio.fechaHoraCacheParametros - DateTime.Now).TotalMinutes) > 10)
            {
                lock(this)
                {
                    if(PARepositorio.ParametrosFrameworkCache != null)
                    {
                        PARepositorio.ParametrosFrameworkCache = null;
                    }
                }
                
            }


            if (PARepositorio.ParametrosFrameworkCache == null)
            {
                lock (this)
                {
                    if (PARepositorio.ParametrosFrameworkCache == null)
                    {
                        try
                        {
                            PARepositorio.ParametrosFrameworkCache = new Dictionary<string, string>();
                            PARepositorio.fechaHoraCacheParametros = DateTime.Now;
                            using (SqlConnection conn = new SqlConnection(CadCnxController))
                            {
                                SqlCommand cmd = new SqlCommand("paObtenerTodosParametrosFramework_PAR", conn);
                                cmd.CommandType = CommandType.StoredProcedure;
                                conn.Open();
                                SqlDataReader reader = cmd.ExecuteReader();

                                while (reader.Read())
                                {
                                    PARepositorio.ParametrosFrameworkCache.Add(reader["PAR_IdParametro"].ToString().Trim(), reader["PAR_ValorParametro"].ToString().Trim());
                                }
                                conn.Close();
                            }
                        }
                        catch
                        {
                            PARepositorio.ParametrosFrameworkCache = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>lista de parametros</returns>
        public Dictionary<string, string> ConsultarListaParametrosFramework()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ParametrosFramework.ToDictionary(Field => Field.PAR_IdParametro, mc => mc.PAR_ValorParametro);
            }
        }

        /// <summary>
        /// Consulta los tipos de identificacion
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoIdentificacion_PAR.Select(obj =>
                  new PATipoIdentificacion()
                  {
                      DescripcionIdentificacion = obj.TII_Descripcion,
                      IdTipoIdentificacion = obj.TII_IdTipoIdentificacion
                  }).ToList();
            }
        }

        /// <summary>
        /// Consultar las ocupaciones
        /// </summary>
        /// <returns>Lista de ocupaciones</returns>
        public IList<PAOcupacionDC> ConsultarOcupacion()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Ocupacion_PAR.Select(obj =>
                  new PAOcupacionDC()
                  {
                      IdOcupacion = obj.OCU_IdOcupacion,
                      DescripcionOcupacion = obj.OCU_Descripcion
                  }).OrderBy(ord => ord.DescripcionOcupacion).ToList();
            }
        }

        /// <summary>
        /// Consulta los tipos de zona
        /// </summary>
        /// <returns>Lista con los tipos de zona</returns>
        public List<PATipoZona> ConsultarTipoZona()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoZona_PAR.OrderBy(loc => loc.TZO_Descripcion).Select(tZona => new PATipoZona() { IdTipoZona = tZona.TZO_IdTipoZona, Descripcion = tZona.TZO_Descripcion }).ToList();
            }
        }

        /// <summary>
        /// Consulta los tipos de localidad
        /// </summary>
        /// <returns>Lista con los tipos de localidad</returns>
        public List<PATipoLocalidad> ConsultarTipoLocalidad()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoLocalidad_PAR.OrderBy(loc => loc.TLO_Descripcion).Select(tLoc => new PATipoLocalidad() { IdTipoLocalidad = tLoc.TLO_IdTipo, Descripcion = tLoc.TLO_Descripcion }).ToList();
            }
        }

        /// <summary>
        /// Consulta las zonas
        /// </summary>
        /// <returns>Lista de zonas</returns>
        public List<PAZonaDC> ConsultarZonas(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            totalRegistros = 0;
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsZona_PAR(filtro,
                                        campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, esAscendente).OrderBy(loc => loc.ZON_Descripcion).Select(zona =>
                  new PAZonaDC()
                  {
                      IdZona = zona.ZON_IdZona,
                      Descripcion = zona.ZON_Descripcion,
                      IdTipoZona = zona.ZON_IdTipoZona
                  }).ToList();
            }
        }

        /// <summary>
        /// Consulta las zonas de localidad por el id de la localidad
        /// </summary>
        /// <param name="IdLocalidad"></param>
        /// <returns></returns>
        public List<PAZonaDC> ConsultarZonaDeLocalidadXLocalidad(string idLocalidad, IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;

                var l = contexto.ConsultarContainsZona_PAR(filtro,
                                       campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, esAscendente).OrderBy(loc => loc.ZON_Descripcion).Select(zon =>
                                       {
                                           var zonas = contexto.ZonaDeLocalidad_PAR.Where(obj => obj.ZLO_IdLocalidad == idLocalidad && obj.ZLO_IdZona == zon.ZON_IdZona).ToList();
                                           var nombreTipoZona = contexto.TipoZona_PAR.Where(obj => obj.TZO_IdTipoZona == zon.ZON_IdTipoZona).Single().TZO_Descripcion;

                                           bool zonAsignada = false;
                                           if (zonas.Count > 0)
                                               zonAsignada = true;

                                           return new PAZonaDC()
                                           {
                                               Descripcion = zon.ZON_Descripcion,
                                               IdTipoZona = zon.ZON_IdTipoZona,
                                               IdZona = zon.ZON_IdZona,
                                               AsignadoALocalidad = zonAsignada,
                                               AsignadoALocalidadOrig = zonAsignada,
                                               NombreTipoZona = nombreTipoZona
                                           };
                                       });
                return l.ToList();
            }
        }

        /// <summary>
        /// Consulta las localidades en zona por el id de zona
        /// </summary>
        /// <param name="IdZona"></param>
        /// <returns>Lista con las localidades asignadas y no asignadas, con una bandera(AsignadoEnZona) indicando si la localidad esta asignada en la zona</returns>
        public List<PALocalidadDC> ConsultarLocalidadEnZonaXZona(string idZona, IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;

                var l = contexto.ConsultarContainsLocalidades_VPAR(filtro,
                                       campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, esAscendente).OrderBy(loc => loc.LOC_Nombre).Select(loc =>
                                       {
                                           var localidades = contexto.LocalidadEnZona_PAR.Include("Zona_PAR").Where(obj => obj.LZO_IdLocalidad == loc.LOC_IdLocalidad).ToList();

                                           string NombreZona = string.Empty;
                                           bool Locdisponible = true;
                                           if (localidades.Count > 0)
                                           {
                                               Locdisponible = false;
                                               NombreZona = localidades.Where(obj => obj.LZO_IdLocalidad == loc.LOC_IdLocalidad).Select(sel => sel.Zona_PAR.ZON_Descripcion).SingleOrDefault();
                                           }

                                           var localidad = localidades.Where(obj => obj.LZO_IdZona == idZona).SingleOrDefault();

                                           bool asignado = false;

                                           if (localidad != null)
                                           {
                                               asignado = true;
                                           }
                                           return new PALocalidadDC()
                                           {
                                               IdLocalidad = loc.LOC_IdLocalidad,
                                               Nombre = loc.LOC_Nombre,
                                               NombreCorto = loc.LOC_NombreCorto,
                                               AsignadoEnZona = asignado,
                                               AsignadoEnZonaOrig = asignado,
                                               DispoLocalidad = Locdisponible,
                                               NombreTipoLocalidad = loc.TLO_Descripcion,
                                               NombreZona = NombreZona,
                                               CodigoPostal = loc.LOC_CodigoPostal
                                           };
                                       });
                return l.ToList();
            }
        }

        /// <summary>
        /// Consulta las localidades por tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidadesXTipoLocalidad(string idTipo)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Localidades_VPAR.Where(loc => loc.LOC_IdTipo == idTipo).OrderBy(loc => loc.LOC_Nombre).Select(loc =>
                  new PALocalidadDC()
                  {
                      IdLocalidad = loc.LOC_IdLocalidad,
                      Nombre = loc.LOC_Nombre,
                      NombreCorto = loc.LOC_NombreCorto,
                      IdAncestroPGrado = loc.LOC_IdAncestroPrimerGrado,
                      NombreAncestroPGrado = loc.LOC_NombrePrimero,
                      NombreAncestroSGrado = loc.LOC_NombreSegundo,
                      NombreAncestroTGrado = loc.LOC_NombreTercero,
                      IdAncestroSGrado = loc.LOC_IdAncestroSegundoGrado,
                      IdAncestroTGrado = loc.LOC_IdAncestroTercerGrado,
                      IdTipoLocalidad = loc.LOC_IdTipo,
                      NombreCompleto = loc.NombreCompleto,
                      CodigoPostal = loc.LOC_CodigoPostal
                  }).ToList();
            }
        }

        /// <summary>
        /// Consulta las localidades por el padre y por el tipo de localidad
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidadesXidPadreXidTipo(string idPadre, string idTipo)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Localidades_VPAR.Where(loc => loc.LOC_IdAncestroPrimerGrado == idPadre && loc.LOC_IdTipo == idTipo).OrderBy(loc => loc.LOC_Nombre).Select(loc =>
                  new PALocalidadDC()
                  {
                      IdLocalidad = loc.LOC_IdLocalidad,
                      Nombre = loc.LOC_Nombre,
                      NombreCorto = loc.LOC_NombreCorto,
                      IdAncestroPGrado = loc.LOC_IdAncestroPrimerGrado,
                      IdAncestroSGrado = loc.LOC_IdAncestroSegundoGrado,
                      IdAncestroTGrado = loc.LOC_IdAncestroTercerGrado,
                      IdTipoLocalidad = loc.LOC_IdTipo,
                      NombreCompleto = loc.NombreCompleto,
                      CodigoPostal = loc.LOC_CodigoPostal
                  }).ToList();
            }
        }

        /// <summary>
        /// Consulta las ciudades por departamento
        /// </summary>
        /// <param name="idDepto">Id del departamento</param>
        /// /// <param name="SoloMunicipios">Indica si solo se selecciona municipios o municipios + corregimientos inspecciones  caserios...</param>
        /// <returns>Lista de localidades</returns>
        public List<PALocalidadDC> ConsultarLocalidadesXDepartamento(string idDepto, bool SoloMunicipios)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IQueryable<Localidades_VPAR> localidades;
                if (SoloMunicipios)
                    localidades = contexto.Localidades_VPAR.Where(loc => loc.LOC_IdAncestroPrimerGrado == idDepto).OrderBy(loc => loc.LOC_Nombre);
                else
                    localidades = contexto.Localidades_VPAR.Where(loc => loc.LOC_IdAncestroPrimerGrado == idDepto || loc.LOC_IdAncestroSegundoGrado == idDepto).OrderBy(loc => loc.LOC_Nombre);

                return localidades.Select(loc =>
                 new PALocalidadDC()
                 {
                     IdLocalidad = loc.LOC_IdLocalidad,
                     Nombre = loc.LOC_Nombre,
                     NombreCorto = loc.LOC_NombreCorto,
                     IdAncestroPGrado = loc.LOC_IdAncestroPrimerGrado,
                     NombreAncestroPGrado = loc.LOC_NombrePrimero,
                     NombreAncestroSGrado = loc.LOC_NombreSegundo,
                     NombreAncestroTGrado = loc.LOC_NombreTercero,
                     IdAncestroSGrado = loc.LOC_IdAncestroSegundoGrado,
                     IdAncestroTGrado = loc.LOC_IdAncestroTercerGrado,
                     IdTipoLocalidad = loc.LOC_IdTipo,
                     NombreCompleto = loc.NombreCompleto,
                     CodigoPostal = loc.LOC_CodigoPostal
                 }).ToList();
            }
        }

        /// <summary>
        /// Consulta las ciudades por departamento
        /// </summary>
        /// <param name="idDepto">Id del departamento</param>
        /// /// <param name="SoloMunicipios">Indica si solo se selecciona municipios o municipios + corregimientos inspecciones  caserios...</param>
        /// <returns>Lista de localidades</returns>
        public PALocalidadDC ObtenerLocalidadPorId(string idLocalidad)
        {
            PALocalidadDC resultado = null;
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Localidades_VPAR loc = contexto.Localidades_VPAR
                  .Where(e => e.LOC_IdLocalidad == idLocalidad)
                  .FirstOrDefault();

                if (loc != null)
                {
                    resultado = new PALocalidadDC
                    {
                        IdLocalidad = loc.LOC_IdLocalidad,
                        Nombre = loc.LOC_Nombre,
                        NombreCorto = loc.LOC_NombreCorto,
                        IdAncestroPGrado = loc.LOC_IdAncestroPrimerGrado,
                        NombreAncestroPGrado = loc.LOC_NombrePrimero,
                        NombreAncestroSGrado = loc.LOC_NombreSegundo,
                        NombreAncestroTGrado = loc.LOC_NombreTercero,
                        IdAncestroSGrado = loc.LOC_IdAncestroSegundoGrado,
                        IdAncestroTGrado = loc.LOC_IdAncestroTercerGrado,
                        IdTipoLocalidad = loc.LOC_IdTipo,
                        NombreCompleto = loc.NombreCompleto,
                        CodigoPostal = loc.LOC_CodigoPostal
                    };
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el radio de busqueda de las reqcogidas en una localidad
        /// </summary>
        /// <param name="Idlocalidad"></param>
        /// <returns></returns>
        public int ObtenerRadioBusquedaRecogidaLocalidad(string Idlocalidad)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRadioBusquedaRecogidaLocalidad_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LOC_IdLocalidad", Idlocalidad);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();

            }

        }


        /// <summary>
        /// Verifica la disponibilidad de las localidades en zona
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public bool VerificarDisponibilidadLocalidadEnZona(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.LocalidadEnZona_PAR.Where(loc => loc.LZO_IdLocalidad == idLocalidad).Count() > 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Consulta la zona a la que está asociada una localidad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAZonaDC ConsultarZonaDeLocalidad(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LocalidadEnZona_PAR locEnZona = contexto.LocalidadEnZona_PAR.Include("Zona_PAR").Where(z => z.LZO_IdLocalidad == idLocalidad).FirstOrDefault();

                if (locEnZona != null)
                {
                    return new PAZonaDC()
                    {
                        Descripcion = locEnZona.Zona_PAR.ZON_Descripcion,
                        IdZona = locEnZona.Zona_PAR.ZON_IdZona
                    };
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene la Zona y el tipo de Zona
        /// correspondiente
        /// </summary>
        /// <returns>lista de zonas y su tipo</returns>
        public List<PAZonaDC> ObtenerListadoZonas()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Zona_PAR.Include("TipoZona_PAR").ToList().ConvertAll<PAZonaDC>(zo => new PAZonaDC()
                {
                    IdZona = zo.ZON_IdZona,
                    Descripcion = zo.ZON_Descripcion,
                    IdTipoZona = zo.ZON_IdTipoZona,
                    NombreTipoZona = zo.TipoZona_PAR.TZO_Descripcion
                });
            }
        }

        /// <summary>
        /// Verifica la disponibilidad de las localidades
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public bool VerificarDisponibilidadLocalidad(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.Localidades_VPAR.Where(loc => loc.LOC_IdLocalidad == idLocalidad).Count() > 0)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Consulta las localidades aplicando el filtro y la paginacion
        /// </summary>
        /// <param name="IdTipoLocalidad"></param>
        /// <returns></returns>
        public List<PALocalidadDC> ConsultarLocalidades(IDictionary<string, string> filtro, out int totalRegistros, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool esAscendente)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;

                List<PALocalidadDC> localidades = contexto.ConsultarContainsLocalidades_VPAR(filtro,
                                        campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, esAscendente).OrderBy(loc => loc.LOC_Nombre).Select(loc =>
                                        {
                                            return new PALocalidadDC()
                                            {
                                                IdLocalidad = loc.LOC_IdLocalidad,
                                                Nombre = loc.LOC_Nombre,
                                                NombreCorto = loc.LOC_NombreCorto,
                                                IdAncestroPGrado = loc.LOC_IdAncestroPrimerGrado,
                                                IdTipoLocalidad = loc.LOC_IdTipo,
                                                IdAncestroSGrado = loc.LOC_IdAncestroSegundoGrado,
                                                IdAncestroTGrado = loc.LOC_IdAncestroTercerGrado,
                                                NombreAncestroSGrado = loc.LOC_NombreSegundo,
                                                NombreAncestroTGrado = loc.LOC_NombreTercero,
                                                NombreAncestroPGrado = loc.LOC_NombrePrimero,
                                                NombreTipoLocalidad = loc.TLO_Descripcion,
                                                CodigoPostal = loc.LOC_CodigoPostal
                                            };
                                        }).ToList();

                return localidades;
            }
        }

        public PAConsecutivoDC CrearRangoConsecutivoIntentoEntrega(PAEnumConsecutivos cajas_PruebasEntrega, long idCentroLogistico, string creadoPor)
        {
            PAConsecutivoDC consecutivo = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_CreacionConsecutivoIntentoEntrega_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CIE_IdTipoConsecutivo", (int)cajas_PruebasEntrega);
                cmd.Parameters.AddWithValue("@CIE_IdCentroLogistico", idCentroLogistico);
                cmd.Parameters.AddWithValue("@CIE_CreadoPor", creadoPor);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    consecutivo = new PAConsecutivoDC
                    {
                        Actual =Convert.ToInt32(reader["CIE_Actual"]),
                        IdTipoConsecutivo = Convert.ToInt16(reader["CIE_IdTipoConsecutivo"]),
                        EstadoActivo = Convert.ToBoolean(reader["CIE_EstaActivo"])                        
                    };                    
                }
            }

            return consecutivo;
        }

        /// <summary>
        /// Obtiene la zonas de la aplicación
        /// </summary>
        /// <returns>Colección con las zonas de la aplicación</returns>
        public IEnumerable<PAZonaDC> ObtenerZonasOperadorPostal()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //return contexto.Zona_PAR.OrderBy(o => o.ZON_Descripcion).ToList().ConvertAll(r => new PAZonaDC()
                return contexto.OperadorPostalZona_PAR.Include("Zona_PAR")
                  .OrderBy(o => o.Zona_PAR.ZON_Descripcion)
                  .ToList()
                  .ConvertAll(r => new PAZonaDC()
                  {
                      IdZona = r.Zona_PAR.ZON_IdZona,
                      Descripcion = r.Zona_PAR.ZON_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene una lista con las actividades economicas
        /// </summary>
        /// <returns>objeto lista de actividades economicas</returns>
        public IEnumerable<PATipoActEconomica> ObtenerActividadesEconomicas()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoActividadEconomica_PAR.OrderBy(o => o.TAE_IdActividad).ToList().ConvertAll<PATipoActEconomica>(r => new PATipoActEconomica()
                {
                    IdTipoActEconomica = r.TAE_IdActividad,
                    Descripcion = r.TAE_Descripcion
                });
            }
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <param name="idTipoPais"></param>
        /// <param name="idTipoDepartamento"></param>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamento(int idTipoPais, int idTipoDepartamento)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string tipoPais = idTipoPais.ToString();
                string tipoDepartamento = idTipoDepartamento.ToString();
                return contexto.Localidades_VPAR
                           .Where(l => l.LOC_IdTipo != tipoPais && l.LOC_IdTipo != tipoDepartamento)
                           .ToList()
                           .ConvertAll<PALocalidadDC>(localidad => new PALocalidadDC
                           {
                               Nombre = localidad.NombreCompleto,
                               IdLocalidad = localidad.LOC_IdLocalidad,
                               IdTipoLocalidad = localidad.LOC_IdTipo,
                               IdAncestroPGrado = localidad.LOC_IdAncestroPrimerGrado,
                               IdAncestroSGrado = localidad.LOC_IdAncestroSegundoGrado,
                               IdAncestroTGrado = localidad.LOC_IdAncestroTercerGrado,
                               NombreCompleto = localidad.NombreCompleto
                           }).OrderBy(x => x.Nombre);
            }
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <param name="idTipoPais"></param>
        /// <param name="idTipoDepartamento"></param>
        /// <param name="idLocalidPais"></param>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerMunicipiosCorregimientoInspeccionCaserioPais(int idTipoPais, int idTipoDepartamento, string idPais)
        {

            string tipoPais = idTipoPais.ToString();
            string tipoDepartamento = idTipoDepartamento.ToString();
            string _idTipoPais = ((int)PAEnumTipoLocalidad.PAIS).ToString();
            string _idTipoDepartamento = ((int)PAEnumTipoLocalidad.DEPARTAMENTO).ToString();
            string _idTipoMunicipio = ((int)PAEnumTipoLocalidad.MUNICIPIO).ToString();
            string _idTipoCorregimiento = ((int)PAEnumTipoLocalidad.CORREGIMIENTO).ToString();
            string _idTipoCaserio = ((int)PAEnumTipoLocalidad.CASERIO).ToString();
            string _idTipoInspeccion = ((int)PAEnumTipoLocalidad.INSPECCION).ToString();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodasLocalidadesAncestros_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().Where(r => r.Field<string>("LOC_IdTipo") != tipoPais && r.Field<string>("LOC_IdTipo") != tipoDepartamento &&
                      ((r.Field<string>("LOC_IdTipo") == _idTipoDepartamento && r.Field<string>("LOC_IdAncestroPrimerGrado") == idPais) ||

                                r.Field<string>("LOC_IdTipo") == _idTipoMunicipio && r.Field<string>("LOC_IdAncestroSegundoGrado") == idPais ||

                               (r.Field<string>("LOC_IdTipo") == _idTipoCorregimiento || r.Field<string>("LOC_IdTipo") == _idTipoInspeccion
                                 || r.Field<string>("LOC_IdTipo") == _idTipoCaserio && r.Field<string>("LOC_IdAncestroTercerGrado") == idPais)))
                            .ToList().ConvertAll<PALocalidadDC>(localidad => new PALocalidadDC
                            {
                                Nombre = localidad.Field<string>("NombreCompleto"),
                                IdLocalidad = localidad.Field<string>("LOC_IdLocalidad"),
                                IdTipoLocalidad = localidad.Field<string>("LOC_IdTipo"),
                                IdAncestroPGrado = localidad.Field<string>("LOC_IdAncestroPrimerGrado"),
                                IdAncestroSGrado = localidad.Field<string>("LOC_IdAncestroSegundoGrado"),
                                IdAncestroTGrado = localidad.Field<string>("LOC_IdAncestroTercerGrado"),
                                NombreAncestroPGrado = localidad.Field<string>("LOC_NombrePrimero"),
                                NombreCompleto = localidad.Field<string>("NombreCompleto"),
                                Indicativo = localidad.Field<string>("LOC_Indicativo"),
                                IdCentroServicio = localidad.Field<long>("MCL_IdCentroLogistico")
                            });
            }

        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos.
        /// </summary>
        /// <param name="idTipoPais"></param>
        /// <param name="idTipoDepartamento"></param>
        /// <param name="idLocalidPais"></param>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerMunicipiosPorPais(int idTipoPais, int idTipoDepartamento, string idPais)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string tipoPais = idTipoPais.ToString();
                string tipoDepartamento = idTipoDepartamento.ToString();
                string _idTipoPais = ((int)PAEnumTipoLocalidad.PAIS).ToString();
                string _idTipoDepartamento = ((int)PAEnumTipoLocalidad.DEPARTAMENTO).ToString();
                string _idTipoMunicipio = ((int)PAEnumTipoLocalidad.MUNICIPIO).ToString();
                string _idTipoCorregimiento = ((int)PAEnumTipoLocalidad.CORREGIMIENTO).ToString();
                string _idTipoCaserio = ((int)PAEnumTipoLocalidad.CASERIO).ToString();
                string _idTipoInspeccion = ((int)PAEnumTipoLocalidad.INSPECCION).ToString();
                return contexto.Localidades_VPAR
                           .Where(l => l.LOC_IdTipo != tipoPais && l.LOC_IdTipo != tipoDepartamento &&
                              (l.LOC_IdAncestroPrimerGrado == idPais ||
                              l.LOC_IdAncestroSegundoGrado == idPais ||
                              l.LOC_IdAncestroTercerGrado == idPais))
                           .ToList()
                           .ConvertAll<PALocalidadDC>(localidad => new PALocalidadDC
                           {
                               Nombre = localidad.NombreCompleto,
                               IdLocalidad = localidad.LOC_IdLocalidad,
                               IdTipoLocalidad = localidad.LOC_IdTipo,
                               IdAncestroPGrado = localidad.LOC_IdAncestroPrimerGrado,
                               IdAncestroSGrado = localidad.LOC_IdAncestroSegundoGrado,
                               IdAncestroTGrado = localidad.LOC_IdAncestroTercerGrado,
                               NombreCompleto = localidad.NombreCompleto,
                               CodigoPostal = localidad.LOC_CodigoPostal
                           });
            }
        }

        /// <summary>
        /// Retorna la informacion de una localidad por el id de ella misma
        /// </summary>
        /// <returns></returns>
        public PALocalidadDC ObtenerInformacionLocalidad(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PALocalidadDC localidad = null;
                Localidades_VPAR localidadVPAR = contexto.Localidades_VPAR.Where(l => l.LOC_IdLocalidad == idLocalidad).FirstOrDefault();

                if (localidadVPAR != null)
                {
                    localidad = new PALocalidadDC
                         {
                             Nombre = localidadVPAR.LOC_Nombre,
                             IdLocalidad = localidadVPAR.LOC_IdLocalidad,
                             IdTipoLocalidad = localidadVPAR.LOC_IdTipo,
                             IdAncestroPGrado = localidadVPAR.LOC_IdAncestroPrimerGrado,
                             IdAncestroSGrado = localidadVPAR.LOC_IdAncestroSegundoGrado,
                             IdAncestroTGrado = localidadVPAR.LOC_IdAncestroTercerGrado,
                             NombreCompleto = localidadVPAR.NombreCompleto,
                             CodigoPostal = localidadVPAR.LOC_CodigoPostal
                         };
                }
                else
                {
                    localidad = new PALocalidadDC
                    {
                        Nombre = string.Empty,
                        IdLocalidad = string.Empty,
                        IdTipoLocalidad = string.Empty,
                        IdAncestroPGrado = string.Empty,
                        IdAncestroSGrado = string.Empty,
                        IdAncestroTGrado = string.Empty,
                        NombreCompleto = string.Empty
                    };
                }

                return localidad;
            }
        }

        /// <summary>
        /// Retorna la lista de países
        /// </summary>
        /// <param name="idTipoPais"></param>
        /// <returns></returns>
        public IEnumerable<PALocalidadDC> ObtenerPaises(int idTipoPais)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string tipoPais = idTipoPais.ToString();
                IEnumerable<PALocalidadDC> lista = contexto.Localidades_VPAR.Where(localidad => localidad.LOC_IdTipo == tipoPais)
                  .ToList().ConvertAll<PALocalidadDC>(localidad => new PALocalidadDC
                  {
                      IdLocalidad = localidad.LOC_IdLocalidad.Trim(),
                      Nombre = localidad.NombreCompleto.Trim()
                  });
                return lista;
            }
        }

        /// <summary>
        /// Obtiene una lista con los regimenes contributivos
        /// </summary>
        /// <returns>objeto lista de regimenes contributivos</returns>
        public IList<PATipoRegimenDC> ObtenerRegimenContributivo()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoRegimenContributivo_PAR.OrderBy(o => o.TRC_IdRegimen).ToList().ConvertAll<PATipoRegimenDC>(r => new PATipoRegimenDC()
                {
                    IdRegimen = r.TRC_IdRegimen,
                    Descripcion = r.TRC_Descripcion
                });
            }
        }

        /// <summary>
        /// Obtiene una lista con los segmentos de mercado
        /// </summary>
        /// <returns>objeto lista de los segmentos de mercado</returns>
        public IList<PATipoSegmentoDC> ObtenerSegmentoMercado()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSegmentoMercado_PAR.OrderBy(o => o.TSM_IdSegmento).ToList().ConvertAll<PATipoSegmentoDC>(r => new PATipoSegmentoDC()
                {
                    IdSegmento = r.TSM_IdSegmento,
                    Descripcion = r.TSM_Descripcion
                });
            }
        }

        /// <summary>
        /// Obtiene una lista con los tipos de sociedad
        /// </summary>
        /// <returns>objeto lista de los tipos de sociedad</returns>
        public IList<PATipoSociedadDC> ObtenerTipoSociedad()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSociedad_PAR.OrderBy(o => o.TIS_IdTipoSociedad).ToList().ConvertAll<PATipoSociedadDC>(r => new PATipoSociedadDC()
                {
                    IdTipoSociedad = r.TIS_IdTipoSociedad,
                    Descripcion = r.TIS_Descripcion
                });
            }
        }

        /// <summary>
        /// Retorna el id del operador postal de la localidad dada
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PAOperadorPostal ObtenerOperadorPostalLocalidad(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LocalidadEnZona_VPAR localidadEnZona = contexto.LocalidadEnZona_VPAR.FirstOrDefault(localidad => localidad.LOC_IdLocalidad == idLocalidad);

                if (localidadEnZona != null)
                {
                    return new PAOperadorPostal { Id = localidadEnZona.OPZ_IdOperadorPostal, IdZona = localidadEnZona.ZON_IdZona, TiempoEntrega = localidadEnZona.OPZ_TiempoEntrega };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Consulta Obtener condicion Operador filtro y paginacion Rafael Ramirez 28-12-2011
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;

                return contexto.ConsultarContainsCondicionOperadorPostal_VPAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll(s => new PACondicionOperadorPostalDC
                  {
                      IdConOperadorPostal = s.COP_IdCondicionOperadorPostal,
                      Descripcion = s.COP_Descripcion,
                      OperadorPostalNombre = s.OPO_Nombre,
                      OperadorPostal = new PAOperadorPostal
                      {
                          Id = s.OPO_IdOperadorPostal,
                          Nombre = s.OPO_Nombre
                      }
                  });
            }
        }

        /// <summary>
        /// Retorna la lista de condiciones dado el operador postal
        /// </summary>
        /// <param name="idOperadorPostal">El operador postal viene dado por el destino del trayecto</param>
        /// <returns></returns>
        public IEnumerable<PACondicionOperadorPostalDC> ObtenerCondicionesPorOperadorPostal(int idOperadorPostal)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CondicionOperadorPostal_PAR> operadores = contexto.CondicionOperadorPostal_PAR.Where(c => c.COP_IdOperadorPostal == idOperadorPostal).ToList();
                if (operadores != null && operadores.Count > 0)
                {
                    return operadores.ConvertAll(o => new PACondicionOperadorPostalDC { Descripcion = o.COP_Descripcion });
                }
                else
                {
                    return new List<PACondicionOperadorPostalDC>();
                }
            }
        }

        /// <summary>
        /// Consulta los Operadores Postales Rafael Ramirez 28-12-2011
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PAOperadorPostal> ObtenerOperadorPostal()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.OperadorPostal_PAR.OrderBy(o => o.OPO_Descripcion).ToList().ConvertAll<PAOperadorPostal>(r => new PAOperadorPostal()
                {
                    Id = r.OPO_IdOperadorPostal,
                    Nombre = r.OPO_Nombre
                });
            }
        }

        /// <summary>
        /// Consulta las zonas de una localidad incluye la zona general
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns>Lista de zonas</returns>
        public IList<PAZonaDC> ConsultarZonasDeLocalidadXLocalidad(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var zonas = contexto.ZonaDeLocalidad_PAR.Include("Zona_PAR").Where(obj => obj.ZLO_IdLocalidad == idLocalidad).Select(obj =>
                  new PAZonaDC()
                  {
                      IdZona = obj.ZLO_IdZona,
                      Descripcion = obj.Zona_PAR.ZON_Descripcion
                  }).ToList();

                if (zonas.Count <= 0)//si la localidad no tiene zonas asignadas se agrega la zona general
                {
                    var zonaGeneral = contexto.Zona_PAR.Where(obj => obj.ZON_IdZona == "-1").SingleOrDefault();
                    if (zonaGeneral == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, ETipoErrorFramework.EX_FALTA_ZONA_GENERAL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALTA_ZONA_GENERAL)));
                    }
                    zonas.Add(new PAZonaDC() { Descripcion = zonaGeneral.ZON_Descripcion, IdZona = zonaGeneral.ZON_IdZona });
                }
                return zonas;
            }
        }

        /// <summary>
        /// Metodo Para obtener un Consecutivo global
        /// enviando el tipo de consecutivo
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivo(PAEnumConsecutivos idConsecutivo)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PApaConsecutivoGlobal pacon = contexto.paObtenerConsecutivoGlobal_PAR((int)idConsecutivo).FirstOrDefault();
                if (pacon != null)
                {
                    return pacon.ValorActual.Value;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                                      ETipoErrorFramework.EX_NO_SE_PUEDE_ASIGNAR_CONSECUTIVO.ToString(),
                                                       MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_SE_PUEDE_ASIGNAR_CONSECUTIVO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Metodo Para obtener un Consecutivo Por Col
        /// enviando el tipo de consecutivo
        /// </summary>
        /// <param name="tipoConsecutivo"></param>
        /// <returns></returns>
        public long ObtenerConsecutivoPorCol(PAEnumConsecutivos idConsecutivo, long idCol)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PApaConsecutivoPorCol pacon = contexto.paObtenerConsecutivoPorCol_PAR((int)idConsecutivo, idCol).FirstOrDefault();
                if (pacon != null)
                {
                    return pacon.ValorActual.Value;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                                      ETipoErrorFramework.EX_NO_SE_PUEDE_ASIGNAR_CONSECUTIVO.ToString(),
                                                       MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_SE_PUEDE_ASIGNAR_CONSECUTIVO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtiene el consecutivo para el archivo de intento de entrega
        /// </summary>
        /// <param name="idConsecutivo">identificador consecutivo</param>
        /// <param name="idCol">identificador del col</param>
        /// <returns>consecutivo</returns>
        public long ObtenerConsecutivoIntentoEntregaPorCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {
            long consecutivo = 0;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConsecutivoIntentoEntregaPorCol_PAR", conn);
                cmd.Parameters.AddWithValue("@IdTipoConsecutivo", (int)idConsecutivo);
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCentroLogistico);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    consecutivo = Convert.ToInt32(reader["CIE_Actual"]);                    
                }
            }

            return consecutivo;
        }

        /// <summary>
        /// Obtiene los estados de la aplicación
        /// </summary>
        /// <returns>Colección con los estados</returns>
        public IEnumerable<PAEstadoActivoInactivoDC> ObtenerEstadosAplicacion()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoActivoInactivo_VFRM
                  .ToList()
                  .ConvertAll(r => new PAEstadoActivoInactivoDC()
                  {
                      IdEstado = r.IdEstado,
                      Estado = r.Estado
                  });
            }
        }

        /// <summary>
        /// Obtiene el operador postal
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns>el operador postal</returns>
        private PAOperadorPostal ObtenerOperadorPostal(int idOperadorPostal)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostal_PAR operador = contexto.OperadorPostal_PAR.FirstOrDefault(op => op.OPO_IdOperadorPostal == idOperadorPostal);

                PAOperadorPostal operadorPostal = null;

                if (operador != null)
                {
                    operadorPostal = new PAOperadorPostal()
                    {
                        Id = operador.OPO_IdOperadorPostal,
                        Nombre = operador.OPO_Nombre
                    };
                }

                return operadorPostal;
            }
        }

        /// <summary>
        /// Retorna el porcentaje del recargo de combustible para el operador postal
        /// </summary>
        /// <param name="idZona"></param>
        /// <returns></returns>
        public decimal ObtenerPorcentajeRecargoCombustibleOPxZona(string idZona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostalZona_PAR operador = contexto.OperadorPostalZona_PAR.Include("OperadorPostal_PAR").Where(r => r.OPZ_IdZona == idZona).FirstOrDefault();
                if (operador == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES,
                                                      ETipoErrorFramework.EX_NO_EXISTE_PORCENTAJE_RECARGO_COMBUSTIBLE_OP.ToString(),
                                                       MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_PORCENTAJE_RECARGO_COMBUSTIBLE_OP));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return operador.OperadorPostal_PAR.OPO_PorcentajeCombustible;
            }
        }

        /// <summary>
        /// Obtiene la zona con el tipo de Zona
        /// </summary>
        /// <param name="idOperadorPostal"></param>
        /// <returns>Zona con su tipo respectivo</returns>
        private PAZonaDC ObtenerZona(string idZona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Zona_PAR zona = contexto.Zona_PAR.Include("TipoZona_PAR").FirstOrDefault(zon => zon.ZON_IdZona == idZona);

                PAZonaDC zonaRetorno = null;

                if (zona != null)
                {
                    zonaRetorno = new PAZonaDC()
                    {
                        IdZona = zona.ZON_IdZona,
                        Descripcion = zona.ZON_Descripcion,
                        IdTipoZona = zona.ZON_IdTipoZona,
                        NombreTipoZona = zona.TipoZona_PAR.TZO_Descripcion
                    };
                }

                return zonaRetorno;
            }
        }

        /// <summary>
        ///Obtiene los Operadores Postales de la Zona
        /// </summary>
        /// <returns>lista de operadores Postales de zona con tiempos de entrega</returns>
        public List<PAOperadorPostalZonaDC> ObtenerOperadorPostalZona(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;

                string opePostal;
                filtro.TryGetValue("OPZ_IdOperadorPostal", out opePostal);

                if (opePostal == "0")
                {
                    filtro.Remove("OPZ_IdOperadorPostal");
                }

                List<PAOperadorPostalZonaDC> operadorPostalZona = contexto.ConsultarContainsOperadorPostalZona_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll(s => new PAOperadorPostalZonaDC
                  {
                      Zona = new PAZonaDC()
                      {
                          IdZona = s.OPZ_IdZona,
                          Descripcion = ObtenerZona(s.OPZ_IdZona).Descripcion,
                      },

                      OperadorPostal = new PAOperadorPostal()
                      {
                          Id = s.OPZ_IdOperadorPostal,
                          Nombre = ObtenerOperadorPostal(s.OPZ_IdOperadorPostal).Nombre,
                      },

                      TiempoEntrega = s.OPZ_TiempoEntrega
                  });

                return operadorPostalZona;
            }
        }

        #endregion CONSULTAS

        #region INSERCIONES

        /// <summary>
        /// Inserta una nueva zona
        /// </summary>
        /// <param name="zona"></param>
        public void InsertarZona(PAZonaDC zona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;
                contexto.Zona_PAR.Add(new Zona_PAR()
                {
                    ZON_IdZona = zona.IdZona,
                    ZON_Descripcion = zona.Descripcion,
                    ZON_IdTipoZona = zona.IdTipoZona.Value,
                    ZON_FechaGrabacion = fecha,
                    ZON_CreadoPor = ControllerContext.Current.Usuario
                });
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inserta una nueva localidad
        /// </summary>
        /// <param name="localidad">Objeto con la informacion de la localidad</param>
        public void InsertarLocalidad(PALocalidadDC localidad)
        {
            //using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;
                contexto.Localidad_PAR.Add(new Localidad_PAR()
                {
                    LOC_IdLocalidad = localidad.IdLocalidad,
                    LOC_IdTipo = localidad.IdTipoLocalidad,
                    LOC_IdAncestroPrimerGrado = localidad.IdAncestroPGrado,
                    LOC_IdAncestroSegundoGrado = localidad.IdAncestroSGrado,
                    LOC_IdAncestroTercerGrado = localidad.IdAncestroTGrado,
                    LOC_Nombre = localidad.Nombre,
                    LOC_NombreCorto = localidad.NombreCorto,
                    LOC_CodigoPostal = localidad.CodigoPostal,
                    LOC_FechaGrabacion = fecha,
                    LOC_CreadoPor = ControllerContext.Current.Usuario
                });

                //Guardar zona de localidad
                contexto.ZonaDeLocalidad_PAR.Add(new ZonaDeLocalidad_PAR
                {
                    ZLO_IdZona = "-1",
                    ZLO_IdLocalidad = localidad.IdLocalidad,
                    ZLO_FechaGrabacion = DateTime.Now,
                    ZLO_CreadoPor = ControllerContext.Current.Usuario
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inserta una nueva zona perteneciente a una localidad
        /// </summary>
        /// <param name="zonaDeLocalidad"></param>
        public void InsertaZonaDeLocalidad(PAZonaLocalidad zonaDeLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;
                contexto.ZonaDeLocalidad_PAR.Add(new ZonaDeLocalidad_PAR()
                {
                    ZLO_IdZona = zonaDeLocalidad.Zona.IdZona,
                    ZLO_IdLocalidad = zonaDeLocalidad.Localidad.IdLocalidad,
                    ZLO_FechaGrabacion = fecha,
                    ZLO_CreadoPor = ControllerContext.Current.Usuario
                });
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inserta una nueva localidad perteneciente a una zona
        /// </summary>
        /// <param name="localidadEnZona"></param>
        public void InsertaLocalidadEnZona(PAZonaLocalidad localidadEnZona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.LocalidadEnZona_PAR.Add(new LocalidadEnZona_PAR()
                {
                    LZO_IdLocalidad = localidadEnZona.Localidad.IdLocalidad,
                    LZO_IdZona = localidadEnZona.Zona.IdZona,
                    LZO_CreadoPor = ControllerContext.Current.Usuario,
                    LZO_FechaGrabacion = DateTime.Now
                });
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Insercion de Condiciones del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="conOperadorPostal"></param>
        public void InsertaCondicionOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.CondicionOperadorPostal_PAR.Add(new CondicionOperadorPostal_PAR()
                {
                    COP_IdOperadorPostal = conOperadorPostal.OperadorPostal.Id,
                    COP_Descripcion = conOperadorPostal.Descripcion,
                    COP_FechaGrabacion = DateTime.Now,
                    COP_CreadoPor = ControllerContext.Current.Usuario
                });
                PARepositorioAudit.MapeoAuditCondOperadorPostal(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// inserta la info del Operador postal y zona
        /// con el tiempo estimado de entrega
        /// </summary>
        /// <param name="operadorPostalZona"></param>
        public void InsertarOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostalZona_PAR nvoOperadorZona = new OperadorPostalZona_PAR()
                {
                    OPZ_IdZona = operadorPostalZona.Zona.IdZona,
                    OPZ_IdOperadorPostal = operadorPostalZona.OperadorPostal.Id,
                    OPZ_TiempoEntrega = operadorPostalZona.TiempoEntrega,
                    OPZ_FechaGrabacion = DateTime.Now,
                    OPZ_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.OperadorPostalZona_PAR.Add(nvoOperadorZona);
                PARepositorioAudit.MapeoAuditOperadorPostalZona(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion INSERCIONES

        #region MODIFICACIONES

        /// <summary>
        /// Modifica una zona
        /// </summary>
        /// <param name="zona"></param>
        public void ModificarZona(PAZonaDC zona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Zona_PAR zonaDB = contexto.Zona_PAR.Where(zon => zon.ZON_IdZona == zona.IdZona).SingleOrDefault();
                if (zonaDB != null)
                {
                    zonaDB.ZON_Descripcion = zona.Descripcion;
                    zonaDB.ZON_IdTipoZona = zona.IdTipoZona.Value;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Modifica una localidad
        /// </summary>
        /// <param name="localidad"></param>
        public void ModificarLocalidad(PALocalidadDC localidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Localidad_PAR localidadDB = contexto.Localidad_PAR.Where(loc => loc.LOC_IdLocalidad == localidad.IdLocalidad).SingleOrDefault();
                if (localidadDB != null)
                {
                    localidadDB.LOC_IdAncestroPrimerGrado = localidad.IdAncestroPGrado;
                    localidadDB.LOC_IdAncestroSegundoGrado = localidad.IdAncestroSGrado;
                    localidadDB.LOC_IdAncestroTercerGrado = localidad.IdAncestroTGrado;
                    localidadDB.LOC_IdTipo = localidad.IdTipoLocalidad;
                    localidadDB.LOC_Nombre = localidad.Nombre;
                    localidadDB.LOC_NombreCorto = localidad.NombreCorto;
                    localidadDB.LOC_CodigoPostal = localidad.CodigoPostal;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Modificacion de las condiciones del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="conOperadorPostal"></param>
        public void ModificarCondicionOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CondicionOperadorPostal_PAR conOperacionPostalDB = contexto.CondicionOperadorPostal_PAR.Where(r => r.COP_IdCondicionOperadorPostal == conOperadorPostal.IdConOperadorPostal).SingleOrDefault();
                if (conOperacionPostalDB != null)
                {
                    conOperacionPostalDB.COP_IdOperadorPostal = conOperadorPostal.OperadorPostal.Id;
                    conOperacionPostalDB.COP_Descripcion = conOperadorPostal.Descripcion;
                    conOperacionPostalDB.COP_FechaGrabacion = DateTime.Now;
                    conOperacionPostalDB.COP_CreadoPor = ControllerContext.Current.Usuario;
                    PARepositorioAudit.MapeoAuditCondOperadorPostal(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Modificar el tiempo del operadorpostal de Zona
        /// </summary>
        /// <param name="operadorPostalZona">info del operador postal</param>
        public void ModificarOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostalZona_PAR modOperadorPostalZona = contexto.OperadorPostalZona_PAR.FirstOrDefault(oz => oz.OPZ_IdZona == operadorPostalZona.Zona.IdZona);

                modOperadorPostalZona.OPZ_IdOperadorPostal = operadorPostalZona.OperadorPostal.Id;
                modOperadorPostalZona.OPZ_TiempoEntrega = operadorPostalZona.TiempoEntrega;

                PARepositorioAudit.MapeoAuditOperadorPostalZona(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion MODIFICACIONES

        #region ELIMINACION

        /// <summary>
        /// Elimina la informacion de una zona y sus relaciones
        /// </summary>
        /// <param name="IdZona"></param>
        public void EliminarZona(string idZona)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    Zona_PAR zona = contexto.Zona_PAR.Include("ZonaDeLocalidad_PAR").Include("OperadorPostalZona_PAR").Where(zon => zon.ZON_IdZona == idZona).SingleOrDefault();

                    if (zona != null)
                    {
                        //Elimina relacion con Operador postal
                        if (zona.OperadorPostalZona_PAR != null)
                            contexto.OperadorPostalZona_PAR.Remove(zona.OperadorPostalZona_PAR);

                        //Elimina relacion con zona de localidad
                        if (zona.ZonaDeLocalidad_PAR != null)
                            zona.ZonaDeLocalidad_PAR.Where(zon => zon.ZLO_IdZona == idZona).ToList().ForEach(zon =>
                            contexto.ZonaDeLocalidad_PAR.Remove(zon));

                        //Elimina relacion con localidad en zona
                        LocalidadEnZona_PAR loc = contexto.LocalidadEnZona_PAR.Where(zon => zon.LZO_IdZona == idZona).SingleOrDefault();
                        if (loc != null)
                            contexto.LocalidadEnZona_PAR.Remove(loc);

                        //Elimina zona
                        contexto.Zona_PAR.Remove(zona);

                        contexto.SaveChanges();
                    }
                }
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Elimina una localidad y sus relaciones
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <param name="contexto"></param>
        private void EliminarLocalidad(string idLocalidad, ModeloParametrosConn contexto)
        {
            Localidad_PAR localidad = contexto.Localidad_PAR.Include("LocalidadEnZona_PAR").Include("ZonaDeLocalidad_PAR").Include("MunicipioRegionalAdm_PUA").Where(loc => loc.LOC_IdLocalidad == idLocalidad).SingleOrDefault();
            if (localidad != null)
            {
                //Elimina hijos en la tabla localidad
                contexto.Localidad_PAR.Where(loc => loc.LOC_IdAncestroPrimerGrado == idLocalidad).ToList().
                  ForEach(loc => EliminarLocalidad(loc.LOC_IdLocalidad, contexto));

                //Elimina la localidad en zona
                if (localidad.LocalidadEnZona_PAR != null)
                    contexto.LocalidadEnZona_PAR.Remove(localidad.LocalidadEnZona_PAR);

                //Elimina la zona de localidad
                if (localidad.ZonaDeLocalidad_PAR != null)
                    localidad.ZonaDeLocalidad_PAR.ToList().
                     ForEach(zonloc => contexto.ZonaDeLocalidad_PAR.Remove(zonloc));

                //Elimina relacion municipio regional administrativa
                if (localidad.MunicipioRegionalAdm_PUA != null)
                    contexto.MunicipioRegionalAdm_PUA.Remove(localidad.MunicipioRegionalAdm_PUA);

                //Elimina localidad
                contexto.Localidad_PAR.Remove(localidad);
            }
        }

        /// <summary>
        /// Elimina la informacion de una localidad y sus relaciones
        /// </summary>
        /// <param name="IdLocalidad"></param>
        public void EliminarLocalidad(string idLocalidad)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    EliminarLocalidad(idLocalidad, contexto);
                    contexto.SaveChanges();
                }
                transaccion.Complete();
            }
        }

        /// <summary>
        /// Elimina Zona de localidad
        /// </summary>
        /// <param name="IdZona"></param>
        public void EliminarZonaDeLocalidad(string idZona, string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ZonaDeLocalidad_PAR zonaLocalidad = contexto.ZonaDeLocalidad_PAR.Where(zonl => zonl.ZLO_IdZona == idZona && zonl.ZLO_IdLocalidad == idLocalidad).SingleOrDefault();

                if (zonaLocalidad != null)
                {
                    contexto.ZonaDeLocalidad_PAR.Remove(zonaLocalidad);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina localidad en zona
        /// </summary>
        /// <param name="IdLocalidad"></param>
        public void EliminarLocalidadEnZona(string idLocalidad)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                LocalidadEnZona_PAR localidadZona = contexto.LocalidadEnZona_PAR.Where(locZ => locZ.LZO_IdLocalidad == idLocalidad).SingleOrDefault();

                if (localidadZona != null)
                {
                    contexto.LocalidadEnZona_PAR.Remove(localidadZona);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina la Condicion del Operador Postal Rafael Ramirez 28-12-2011
        /// </summary>
        /// <param name="conOperadorPostal"></param>
        public void EliminarConOperadorPostal(PACondicionOperadorPostalDC conOperadorPostal)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CondicionOperadorPostal_PAR conOpePostal = contexto.CondicionOperadorPostal_PAR.Where(r => r.COP_IdCondicionOperadorPostal == conOperadorPostal.IdConOperadorPostal).SingleOrDefault();

                if (conOpePostal != null)
                {
                    contexto.CondicionOperadorPostal_PAR.Remove(conOpePostal);
                    PARepositorioAudit.MapeoAuditCondOperadorPostal(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina el Operador postal de l
        /// </summary>
        /// <param name="operadorPostalZona"></param>
        public void EliminarOperadorPostalZona(PAOperadorPostalZonaDC operadorPostalZona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostalZona_PAR eliOperadorPostalZona = contexto.OperadorPostalZona_PAR.FirstOrDefault(oz => oz.OPZ_IdZona == operadorPostalZona.Zona.IdZona
                                                                                                              && oz.OPZ_IdOperadorPostal == operadorPostalZona.OperadorPostal.Id);
                contexto.OperadorPostalZona_PAR.Remove(eliOperadorPostalZona);
                PARepositorioAudit.MapeoAuditOperadorPostalZona(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion ELIMINACION

        #region Representante Legal

        /// <summary>
        /// Obtiene lista de responsable legal
        /// </summary>
        /// <returns>Lista con los responsables legales</returns>
        public IList<PAResponsableLegal> ObtenerResponsableLegal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var d = contexto.ConsultarContainsResponsableLegal_VPAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList().ConvertAll(obj =>
                {
                    Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == obj.PEE_Municipio).Single();
                    PAResponsableLegal resp = new PAResponsableLegal()
                    {
                        Email = obj.PRL_Email,
                        EmpresaEmpleador = obj.PRL_EmpresaEmpleador,
                        Fax = obj.PRL_Fax,
                        IdResponsable = obj.PRL_IdPersonaExterna,
                        IngresosEmpleoActual = obj.PRL_IngresosEmpleoActual,
                        Ocupacion = obj.PRL_Ocupacion,
                        PoseeFincaRaiz = obj.PRL_PoseeFincaRaiz,
                        Telefono = obj.PRL_Telefono,
                        NombreCompuesto = obj.PEE_PrimerNombre + " " + obj.PEE_PrimerApellido,
                        PersonaExterna = new PAPersonaExterna()
                        {
                            DigitoVerificacion = obj.PEE_DigitoVerificacion,
                            Direccion = obj.PEE_Direccion,
                            FechaExpedicionDocumento = obj.PEE_FechaExpedicionDocumento,
                            Identificacion = obj.PEE_Identificacion,
                            IdPersonaExterna = obj.PEE_IdPersonaExterna,
                            IdTipoIdentificacion = obj.PEE_IdTipoIdentificacion,
                            Municipio = obj.PEE_Municipio,
                            NumeroCelular = obj.PEE_NumeroCelular,
                            PrimerApellido = obj.PEE_PrimerApellido,
                            PrimerNombre = obj.PEE_PrimerNombre,
                            SegundoApellido = obj.PEE_SegundoApellido,
                            SegundoNombre = obj.PEE_SegundoNombre,
                            Telefono = obj.PEE_Telefono,
                            NombreMunicipio = localidad.LOC_Nombre,
                            IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                            NombreDepto = localidad.LOC_NombrePrimero,
                            IdPais = localidad.LOC_IdAncestroSegundoGrado,
                            NombrePais = localidad.LOC_NombreSegundo,
                        }
                    };
                    return resp;
                }
                  ).OrderBy(obj => obj.NombreCompuesto).ToList();

                return d;
            }
        }

        /// <summary>
        /// Consulta un responsable legal dependiendo de su id
        /// </summary>
        /// <param name="idResponsable">Id del responsable legal</param>
        /// <returns>Responsable legal</returns>
        public PAResponsableLegal ObtenerResponsableLegal(long idResponsable)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ResponsableLegal_VPAR.Where(res =>
                  res.PRL_IdPersonaExterna == idResponsable
                ).ToList().ConvertAll<PAResponsableLegal>(res =>
                {
                    Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == res.PEE_Municipio).Single();

                    PAResponsableLegal respon = new PAResponsableLegal
                    {
                        Email = res.PRL_Email,
                        EmpresaEmpleador = res.PRL_EmpresaEmpleador,
                        Fax = res.PRL_Fax,
                        IdResponsable = res.PRL_IdPersonaExterna,
                        IngresosEmpleoActual = res.PRL_IngresosEmpleoActual,
                        Ocupacion = res.PRL_Ocupacion,
                        PoseeFincaRaiz = res.PRL_PoseeFincaRaiz,
                        Telefono = res.PRL_Telefono,
                        NombreCompuesto = res.PEE_PrimerNombre + " " + res.PEE_PrimerApellido,
                        PersonaExterna = new PAPersonaExterna()
                        {
                            DigitoVerificacion = res.PEE_DigitoVerificacion,
                            Direccion = res.PEE_Direccion,
                            FechaExpedicionDocumento = res.PEE_FechaExpedicionDocumento,
                            Identificacion = res.PEE_Identificacion,
                            IdPersonaExterna = res.PEE_IdPersonaExterna,
                            IdTipoIdentificacion = res.PEE_IdTipoIdentificacion,
                            Municipio = res.PEE_Municipio,
                            NumeroCelular = res.PEE_NumeroCelular,
                            PrimerApellido = res.PEE_PrimerApellido,
                            PrimerNombre = res.PEE_PrimerNombre,
                            SegundoApellido = res.PEE_SegundoApellido,
                            SegundoNombre = res.PEE_SegundoNombre,
                            Telefono = res.PEE_Telefono,
                            NombreMunicipio = localidad.LOC_Nombre,
                            IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                            NombreDepto = localidad.LOC_NombrePrimero,
                            IdPais = localidad.LOC_IdAncestroSegundoGrado,
                            NombrePais = localidad.LOC_NombreSegundo,
                        }
                    };

                    return respon;
                }).ToList().SingleOrDefault();
            }
        }

        /// <summary>
        /// Adiciona un responsable legal
        /// </summary>
        /// <param name="responsable">Objeto responsable legal</param>
        public long AdicionarResponsableLegal(PAResponsableLegal responsable)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                PersonaExterna_PAR perExt = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion == responsable.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == responsable.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();

                if (perExt == null)
                {
                    perExt = new PersonaExterna_PAR()
                    {
                        PEE_CreadoPor = ControllerContext.Current.Usuario,
                        PEE_DigitoVerificacion = responsable.PersonaExterna.DigitoVerificacion,
                        PEE_Direccion = responsable.PersonaExterna.Direccion,
                        PEE_FechaExpedicionDocumento = responsable.PersonaExterna.FechaExpedicionDocumento,
                        PEE_FechaGrabacion = fecha,
                        PEE_Identificacion = responsable.PersonaExterna.Identificacion.Trim(),
                        PEE_IdPersonaExterna = 0,
                        PEE_IdTipoIdentificacion = responsable.PersonaExterna.IdTipoIdentificacion,
                        PEE_Municipio = responsable.PersonaExterna.Municipio,
                        PEE_NumeroCelular = responsable.PersonaExterna.NumeroCelular,
                        PEE_PrimerNombre = responsable.PersonaExterna.PrimerNombre,
                        PEE_PrimerApellido = responsable.PersonaExterna.PrimerApellido,
                        PEE_SegundoApellido = responsable.PersonaExterna.SegundoApellido,
                        PEE_SegundoNombre = responsable.PersonaExterna.SegundoNombre,
                        PEE_Telefono = responsable.PersonaExterna.Telefono
                    };
                    contexto.PersonaExterna_PAR.Add(perExt);
                }

                PersonaResponsableLegal_PAR res = new PersonaResponsableLegal_PAR()
                {
                    PRL_CreadoPor = ControllerContext.Current.Usuario,
                    PRL_Email = responsable.Email,
                    PRL_EmpresaEmpleador = responsable.EmpresaEmpleador,
                    PRL_Fax = responsable.Fax,
                    PRL_FechaGrabacion = fecha,
                    PRL_IdPersonaExterna = perExt.PEE_IdPersonaExterna,
                    PRL_IngresosEmpleoActual = responsable.IngresosEmpleoActual,
                    PRL_Ocupacion = responsable.Ocupacion,
                    PRL_PoseeFincaRaiz = responsable.PoseeFincaRaiz,
                    PRL_Telefono = responsable.Telefono
                };

                contexto.PersonaResponsableLegal_PAR.Add(res);
                contexto.SaveChanges();
                return perExt.PEE_IdPersonaExterna;
            }
        }

        /// <summary>
        /// Obtiene la informacion de una persona externa
        /// </summary>
        /// <param name="idPersona"></param>
        /// <returns></returns>
        public PAPersonaExterna ObtenerPersonaExterna(long idPersona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaExterna_PAR persona = contexto.PersonaExterna_PAR.Where(p => p.PEE_IdPersonaExterna == idPersona).FirstOrDefault();
                if (persona != null)
                {
                    return new PAPersonaExterna()
                    {
                        DigitoVerificacion = persona.PEE_DigitoVerificacion,
                        Direccion = persona.PEE_Direccion,
                        FechaExpedicionDocumento = persona.PEE_FechaExpedicionDocumento,
                        Identificacion = persona.PEE_Identificacion,
                        IdPersonaExterna = persona.PEE_IdPersonaExterna,
                        IdTipoIdentificacion = persona.PEE_IdTipoIdentificacion,
                        Municipio = persona.PEE_Municipio,
                        NumeroCelular = persona.PEE_NumeroCelular,
                        PrimerApellido = persona.PEE_PrimerApellido,
                        PrimerNombre = persona.PEE_PrimerNombre,
                        SegundoApellido = persona.PEE_SegundoApellido,
                        SegundoNombre = persona.PEE_SegundoNombre,
                        Telefono = persona.PEE_Telefono,
                        NombreCompleto = persona.PEE_PrimerNombre + " " + persona.PEE_PrimerApellido
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene la informacion de una persona interna
        /// </summary>
        /// <param name="idPersona"></param>
        /// <returns></returns>
        public PAPersonaInternaDC ObtenerPersonaInterna(long idPersona)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaInterna_PAR persona = contexto.PersonaInterna_PAR.Where(p => p.PEI_IdPersonaInterna == idPersona).FirstOrDefault();
                if (persona != null)
                {
                    return new PAPersonaInternaDC()
                    {
                        IdPersonaInterna = persona.PEI_IdPersonaInterna,
                        NombreCompleto = persona.PEI_Nombre + " " + persona.PEI_PrimerApellido,
                        Identificacion = persona.PEI_Identificacion,
                        IdCargo = persona.PEI_IdCargo,
                        Telefono = persona.PEI_Telefono,
                        Email = persona.PEI_Email
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Modifica un ResponsableLega
        /// </summary>
        /// <param name="responsable">Objeto ResponsableLega</param>
        public void EditarResponsableLegal(PAResponsableLegal responsable)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaResponsableLegal_PAR respon = contexto.PersonaResponsableLegal_PAR.Include("PersonaExterna_PAR")
                  .Where(obj => obj.PRL_IdPersonaExterna == responsable.IdResponsable)
                  .SingleOrDefault();

                if (respon == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                respon.PersonaExterna_PAR.PEE_DigitoVerificacion = responsable.PersonaExterna.DigitoVerificacion;
                respon.PersonaExterna_PAR.PEE_Direccion = responsable.PersonaExterna.Direccion;
                respon.PersonaExterna_PAR.PEE_FechaExpedicionDocumento = responsable.PersonaExterna.FechaExpedicionDocumento;

                respon.PersonaExterna_PAR.PEE_Identificacion = responsable.PersonaExterna.Identificacion.Trim();

                respon.PersonaExterna_PAR.PEE_IdTipoIdentificacion = responsable.PersonaExterna.IdTipoIdentificacion;
                respon.PersonaExterna_PAR.PEE_Municipio = responsable.PersonaExterna.Municipio;
                respon.PersonaExterna_PAR.PEE_NumeroCelular = responsable.PersonaExterna.NumeroCelular;
                respon.PersonaExterna_PAR.PEE_PrimerNombre = responsable.PersonaExterna.PrimerNombre;
                respon.PersonaExterna_PAR.PEE_PrimerApellido = responsable.PersonaExterna.PrimerApellido;
                respon.PersonaExterna_PAR.PEE_SegundoApellido = responsable.PersonaExterna.SegundoApellido;
                respon.PersonaExterna_PAR.PEE_SegundoNombre = responsable.PersonaExterna.SegundoNombre;
                respon.PersonaExterna_PAR.PEE_Telefono = responsable.PersonaExterna.Telefono;
                respon.PRL_Email = responsable.Email;
                respon.PRL_EmpresaEmpleador = responsable.EmpresaEmpleador;
                respon.PRL_Fax = responsable.Fax;
                respon.PRL_IngresosEmpleoActual = responsable.IngresosEmpleoActual;
                respon.PRL_Ocupacion = responsable.Ocupacion;
                respon.PRL_PoseeFincaRaiz = responsable.PoseeFincaRaiz;
                respon.PRL_Telefono = responsable.Telefono;

                contexto.SaveChanges();
            }
        }

        #endregion Representante Legal

        #region CRUD Tipo de actividad economica

        /// <summary>
        /// Otiene los tipos de actividad
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de actividad economica</returns>
        public IList<PATipoActEconomica> ObtenerTiposActividadEconomica(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoActividadEconomica_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PATipoActEconomica
                  {
                      IdTipoActEconomica = obj.TAE_IdActividad,
                      Descripcion = obj.TAE_Descripcion
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un tipo de actividad Economica
        /// </summary>
        /// <param name="docuCentroServ">Objeto tipo de actividad economica</param>
        public void AdicionarTipoActividadEconomica(PATipoActEconomica tipoActEconomica)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoActividadEconomica_PAR tipoAct = new TipoActividadEconomica_PAR()
                {
                    TAE_Descripcion = tipoActEconomica.Descripcion,
                    /// TAE_IdActividad = 0,
                    TAE_CreadoPor = ControllerContext.Current.Usuario,
                    TAE_FechaGrabacion = DateTime.Now
                };
                contexto.TipoActividadEconomica_PAR.Add(tipoAct);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de actividad Economica
        /// </summary>
        /// <param name="banco">Objeto tipo de actividad economica</param>
        public void EditarTipoActividadEconomica(PATipoActEconomica tipoActEconomica)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoActividadEconomica_PAR tipoAct = contexto.TipoActividadEconomica_PAR
                  .Where(obj => obj.TAE_IdActividad == tipoActEconomica.IdTipoActEconomica)
                  .SingleOrDefault();

                if (tipoAct == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                tipoAct.TAE_Descripcion = tipoActEconomica.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de actividad Economica
        /// </summary>
        /// <param name="banco">Tipo de actividad economica</param>
        public void EliminarTipoActividadEconomica(PATipoActEconomica tipoActEconomica)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoActividadEconomica_PAR tipoAct = contexto.TipoActividadEconomica_PAR.Where(obj => obj.TAE_IdActividad == tipoActEconomica.IdTipoActEconomica).SingleOrDefault();
                if (tipoAct != null)
                {
                    contexto.TipoActividadEconomica_PAR.Remove(tipoAct);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de actividad economica
        /// </summary>
        /// <returns>Lista con los tipos de actividad economica</returns>
        public IList<PATipoActEconomica> ObtenerTiposActividadEconomica()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoActividadEconomica_PAR.Select(obj =>
                  new PATipoActEconomica()
                  {
                      Descripcion = obj.TAE_Descripcion,
                      IdTipoActEconomica = obj.TAE_IdActividad
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        #endregion CRUD Tipo de actividad economica

        #region Bancos

        /// <summary>
        /// Obtiene los bancos
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de tipos de Bancos</returns>
        public IList<PABanco> ObtenerBancos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsBanco_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PABanco
                  {
                      IdBanco = obj.BAN_IdBanco,
                      Descripcion = obj.BAN_Descripcion
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un banco
        /// </summary>
        /// <param name="banco">Objeto banco</param>
        public void AdicionarBanco(PABanco banco)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Banco_PAR ban = new Banco_PAR()
                {
                    BAN_Descripcion = banco.Descripcion,
                    BAN_IdBanco = banco.IdBanco,
                    BAN_CreadoPor = ControllerContext.Current.Usuario,
                    BAN_FechaGrabacion = DateTime.Now
                };
                contexto.Banco_PAR.Add(ban);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un Banco
        /// </summary>
        /// <param name="banco">Objeto banco</param>
        public void EditarBanco(PABanco banco)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Banco_PAR ban = contexto.Banco_PAR
                  .Where(obj => obj.BAN_IdBanco == banco.IdBanco)
                  .SingleOrDefault();

                if (ban == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                ban.BAN_Descripcion = banco.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un banco
        /// </summary>
        /// <param name="banco">Objeto con la informacion de un banco</param>
        public void EliminarBanco(PABanco banco)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Banco_PAR ban = contexto.Banco_PAR.Where(obj => obj.BAN_IdBanco == banco.IdBanco).SingleOrDefault();
                if (ban != null)
                {
                    contexto.Banco_PAR.Remove(ban);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene todos los bancos
        /// </summary>
        /// <returns>Lista con los bancos</returns>
        public IList<PABanco> ObtenerTodosBancos()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Banco_PAR.Select(obj =>
                  new PABanco()
                  {
                      Descripcion = obj.BAN_Descripcion,
                      IdBanco = obj.BAN_IdBanco
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de cuenta
        /// </summary>
        /// <returns>Lista con los tipos de cuenta</returns>
        public IList<PATipoCuenta> ObtenerTiposCuentaBanco()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoCuentaBanco_PAR.Select(obj =>
                  new PATipoCuenta()
                  {
                      Descripcion = obj.TCB_Descripcion,
                      IdTipoCuenta = obj.TCB_IdTipoCuenta
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene los Tipos de Documentos de Banco
        /// </summary>
        /// <returns>lista de los Tipos de Doc Banco</returns>
        public IList<PATipoDocumBancoDC> ObtenerTiposDocumentosBanco()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoDocumentoBanco_PAR.Select(tip => new PATipoDocumBancoDC()
                {
                    IdTipoDocumento = tip.TDB_IdTipoDocumento,
                    Descripcion = tip.TDB_Descripcion,
                    Estado = tip.TDB_Estado
                }).ToList();
            }
        }

        #endregion Bancos

        #region Responsable del Servicio

        /// <summary>
        /// Obtiene los Responsables de los servicios
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista con los responsables de los servicios</returns>
        public IList<PAResponsableServicio> ObtenerResponsableServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsResponsableServicio_VPAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
                   .ConvertAll<PAResponsableServicio>(obj =>
                     {
                         Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == obj.PEE_Municipio).Single();

                         PAResponsableServicio responsable = new PAResponsableServicio()
                        {
                            Email = obj.PRS_Email,
                            Fax = obj.PRS_Fax,
                            PersonaExterna = new PAPersonaExterna()
                            {
                                DigitoVerificacion = obj.PEE_DigitoVerificacion,
                                Direccion = obj.PEE_Direccion,
                                FechaExpedicionDocumento = obj.PEE_FechaExpedicionDocumento,
                                Identificacion = obj.PEE_Identificacion,
                                IdPersonaExterna = obj.PEE_IdPersonaExterna,
                                IdTipoIdentificacion = obj.PEE_IdTipoIdentificacion,
                                Municipio = obj.PEE_Municipio,
                                NombreMunicipio = localidad.LOC_Nombre,
                                IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                                NombreDepto = localidad.LOC_NombrePrimero,
                                IdPais = localidad.LOC_IdAncestroSegundoGrado,
                                NombrePais = localidad.LOC_NombreSegundo,
                                NumeroCelular = obj.PEE_NumeroCelular,
                                PrimerApellido = obj.PEE_PrimerApellido,
                                PrimerNombre = obj.PEE_PrimerNombre,
                                SegundoApellido = obj.PEE_SegundoApellido,
                                SegundoNombre = obj.PEE_SegundoApellido,
                                Telefono = obj.PEE_Telefono
                            }
                        };
                         return responsable;
                     }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un responsable del servicio
        /// </summary>
        /// <param name="responsable">Objeto Responsable de servicio</param>
        public long AdicionarResponsableServicio(PAResponsableServicio responsable)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                PersonaExterna_PAR perExt = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion == responsable.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == responsable.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();

                if (perExt == null)
                {
                    perExt = new PersonaExterna_PAR()
                    {
                        PEE_CreadoPor = ControllerContext.Current.Usuario,
                        PEE_DigitoVerificacion = responsable.PersonaExterna.DigitoVerificacion,
                        PEE_Direccion = responsable.PersonaExterna.Direccion,
                        PEE_FechaExpedicionDocumento = responsable.PersonaExterna.FechaExpedicionDocumento,
                        PEE_Identificacion = responsable.PersonaExterna.Identificacion.Trim(),
                        PEE_IdPersonaExterna = 0,
                        PEE_IdTipoIdentificacion = responsable.PersonaExterna.IdTipoIdentificacion,
                        PEE_Municipio = responsable.PersonaExterna.Municipio,
                        PEE_NumeroCelular = responsable.PersonaExterna.NumeroCelular,
                        PEE_PrimerApellido = responsable.PersonaExterna.PrimerApellido,
                        PEE_PrimerNombre = responsable.PersonaExterna.PrimerNombre,
                        PEE_SegundoApellido = responsable.PersonaExterna.SegundoApellido,
                        PEE_SegundoNombre = responsable.PersonaExterna.SegundoNombre,
                        PEE_Telefono = responsable.PersonaExterna.Telefono,
                        PEE_FechaGrabacion = fecha
                    };
                    contexto.PersonaExterna_PAR.Add(perExt);
                }

                PersonaResponsableServicio_PAR respon = new PersonaResponsableServicio_PAR()
                {
                    PRS_Email = responsable.Email,
                    PRS_Fax = responsable.Fax,
                    PRS_IdPersonaExterna = perExt.PEE_IdPersonaExterna,
                    PRS_CreadoPor = ControllerContext.Current.Usuario,
                    PRS_FechaGrabacion = fecha
                };

                contexto.PersonaResponsableServicio_PAR.Add(respon);
                contexto.SaveChanges();

                return respon.PRS_IdPersonaExterna;
            }
        }

        /// <summary>
        /// Edita un responsable del servicio
        /// </summary>
        /// <param name="responsable">Objeto responsable del servicio</param>
        public void EditarResponsableServicio(PAResponsableServicio responsable)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaResponsableServicio_PAR resp = contexto.PersonaResponsableServicio_PAR.Include("PersonaExterna_PAR")
                  .Where(obj => obj.PRS_IdPersonaExterna == responsable.PersonaExterna.IdPersonaExterna)
                  .SingleOrDefault();

                if (resp == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                resp.PRS_Email = responsable.Email;
                resp.PRS_Fax = responsable.Fax;

                resp.PersonaExterna_PAR.PEE_DigitoVerificacion = responsable.PersonaExterna.DigitoVerificacion;
                resp.PersonaExterna_PAR.PEE_Direccion = responsable.PersonaExterna.Direccion;
                resp.PersonaExterna_PAR.PEE_FechaExpedicionDocumento = responsable.PersonaExterna.FechaExpedicionDocumento;
                resp.PersonaExterna_PAR.PEE_Identificacion = responsable.PersonaExterna.Identificacion.Trim();
                resp.PersonaExterna_PAR.PEE_IdTipoIdentificacion = responsable.PersonaExterna.IdTipoIdentificacion;
                resp.PersonaExterna_PAR.PEE_Municipio = responsable.PersonaExterna.Municipio;
                resp.PersonaExterna_PAR.PEE_NumeroCelular = responsable.PersonaExterna.NumeroCelular;
                resp.PersonaExterna_PAR.PEE_PrimerApellido = responsable.PersonaExterna.PrimerApellido;
                resp.PersonaExterna_PAR.PEE_PrimerNombre = responsable.PersonaExterna.PrimerNombre;
                resp.PersonaExterna_PAR.PEE_SegundoApellido = responsable.PersonaExterna.SegundoApellido;
                resp.PersonaExterna_PAR.PEE_SegundoNombre = responsable.PersonaExterna.SegundoNombre;
                resp.PersonaExterna_PAR.PEE_Telefono = responsable.PersonaExterna.Telefono;

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un responable de servicio
        /// </summary>
        /// <param name="responsable">Objeto con la informacion de un responable de servicio</param>
        public void EliminarResponsableServicio(PAResponsableServicio responsable)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PersonaResponsableServicio_PAR resp = contexto.PersonaResponsableServicio_PAR.Where(obj => obj.PRS_IdPersonaExterna == responsable.PersonaExterna.IdPersonaExterna).SingleOrDefault();
                if (resp != null)
                {
                    try
                    {
                        contexto.PersonaResponsableServicio_PAR.Remove(resp);
                        contexto.PersonaExterna_PAR.Remove(resp.PersonaExterna_PAR);
                        contexto.SaveChanges();
                    }
                    catch (UpdateException UpEx)
                    {
                        if ((UpEx.InnerException as SqlException).Number == 547)
                        {
                            contexto.PersonaResponsableServicio_PAR.Remove(resp);
                            contexto.SaveChanges();
                        }
                        else
                            throw UpEx;
                    }
                }
            }
        }

        #endregion Responsable del Servicio

        #region Persona Interna

        public IEnumerable<PAPersonaInternaDC> ObtenerPersonasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            try
            {
                using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    var personas = contexto.ConsultarContainsPersonaInternaCargoRegional_VPAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .ToList().ConvertAll<PAPersonaInternaDC>(r =>
                    {
                        PAPersonaInternaDC persona = new PAPersonaInternaDC
                        {
                            IdPersonaInterna = r.PEI_IdPersonaInterna,
                            NombreCompleto = r.PEI_Nombre + " " + r.PEI_PrimerApellido,
                            Identificacion = r.PEI_Identificacion,
                            IdCargo = r.PEI_IdCargo,
                            IdRegionalAdministrativa = r.PEI_IdRegionalAdm,
                            NombreCargo = r.CAR_Descripcion,
                            NombreRegional = r.REA_Descripcion,
                        };

                        return persona;
                    }).ToList();
                    return personas;
                }
            }
            catch
            {
                totalRegistros = 0;
                return null;
            }
        }

        #endregion Persona Interna

        #region Dias

        /// <summary>
        /// Obtiene todos los dias
        /// </summary>
        /// <returns>Lista con los dias</returns>
        public IList<PADia> ObtenerTodosDias()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Dia_PAR.Select(obj =>
                  new PADia()
                  {
                      NombreDia = obj.DIA_NombreDia,
                      IdDia = obj.DIA_IdDia.Trim()
                  }
                  ).OrderBy(obj => obj.IdDia).ToList();
            }
        }

        #endregion Dias

        #region Semanas

        /// <summary>
        /// Obtiene todos los dias
        /// </summary>
        /// <returns>Lista con los dias</returns>
        public IList<PASemanaDC> ObtenerTodasSemanas()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Semana_PAR.Select(obj =>
                  new PASemanaDC()
                  {
                      Descripcion = obj.SEM_NombreSemana,
                      IdSemana = obj.SEM_IdSemana
                  }
                  ).OrderBy(obj => obj.IdSemana).ToList();
            }
        }

        #endregion Semanas

        #region Meses

        /// <summary>
        /// Obtiene todos los meses
        /// </summary>
        /// <returns>Lista con los meses</returns>
        public IList<PAMesDC> ObtenerTodosMeses()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Mes_PAR.Select(obj =>
                  new PAMesDC()
                  {
                      Descripcion = obj.MES_NombreMes,
                      IdMes = obj.MES_IdMes
                  }
                  ).OrderBy(obj => obj.IdMes).ToList();
            }
        }

        #endregion Meses

        #region Zonas

        /// <summary>
        /// Obtener las zonas de localidades
        /// </summary>
        /// <returns>Colección con las zonas de localidades</returns>
        public IEnumerable<PAZonaDC> ObtenerZonasDeLocalidad()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ZonaDeLocalidadLocalidad_VPAR
                  .ToList()
                  .ConvertAll<PAZonaDC>(zonaVista => new PAZonaDC
                              {
                                  IdZona = zonaVista.ZLO_IdZona,
                                  Descripcion = zonaVista.ZON_Descripcion,
                                  IdTipoZona = zonaVista.ZON_IdTipoZona,
                                  NombreTipoZona = zonaVista.TZO_Descripcion,
                                  EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                              });
            }
        }

        #endregion Zonas

        #region Calendario Festivos x Pais

        /// <summary>
        /// Obtiene el número de días hábiles que hay entre una fecha y otra teniendo en cuenta el sábado como día hábil
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public int ObtenerDiasHabiles(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            List<DateTime> Festivos = new List<DateTime>();
            Festivos = ObtenerFestivos(fechadesde, fechahasta, idPais);
            int NumDomingos = ObtenerNumeroDeDomingos(fechadesde, fechahasta);
            return fechahasta.Subtract(fechadesde).Days - Festivos.Count - NumDomingos;
        }

        /// <summary>
        /// Obtiene el listado de días festivos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivos(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            List<DateTime> Festivos = new List<DateTime>();
            if (Cache.Instancia.ContainsFestivos())
            {
                Festivos = Cache.Instancia.GetFestivos();
            }
            else
            {               
                using (SqlConnection cnn = new SqlConnection(CadCnxController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerCalendarioFestivos", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CAL_IdPais", idPais);
                    cmd.Parameters.AddWithValue("@fechadesde",fechadesde);
                    cmd.Parameters.AddWithValue("@fechahasta",fechahasta);
                    cnn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Festivos.Add(Convert.ToDateTime(reader["CAL_Fecha"]).Date);
                    }

                    cnn.Close();

                    Cache.Instancia.AddFestivos(Festivos);
                }
            }

            return Festivos;
        }

        /// <summary>
        /// Obtiene los festivos entre dos fechas pero no los agrega a la cache, con el fin de consultar fechas por meses
        /// </summary>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivosSinCache(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            List<DateTime> Festivos = new List<DateTime>();

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCalendarioFestivos", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CAL_IdPais", idPais);
                cmd.Parameters.AddWithValue("@fechadesde", fechadesde);
                cmd.Parameters.AddWithValue("@fechahasta", fechahasta);
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Festivos.Add(Convert.ToDateTime(reader["CAL_Fecha"]).Date);
                }
                cnn.Close();
            }
            return Festivos;
        }


        /// <summary>
        /// Obtiene el listado de días festivos a partir de una fecha
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <returns></returns>
        /// 

        public List<DateTime> ObtenerFestivosAdo(DateTime fechadesde)
        {
            string paispredeterminado = ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT);
            List<DateTime> Festivos = new List<DateTime>();
            if (Cache.Instancia.ContainsFestivos())
            {
                Festivos = Cache.Instancia.GetFestivos();
            }
            else
            {
                using (var sqlconn = new SqlConnection(CadCnxController))
                {
                    var cmd = new SqlCommand("paObtenerFestivosDesde_PAR", sqlconn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idPais", paispredeterminado);
                    cmd.Parameters.AddWithValue("@fechaDesde", DateTime.Now.Date);
                    sqlconn.Open();
                    var resultado = cmd.ExecuteReader();
                    Festivos = MapperFestivos(resultado);
                }
            }

            return Festivos.Where
                  (cf =>
                  cf > fechadesde).ToList();
        }

        private List<DateTime> MapperFestivos(SqlDataReader reader)
        {
            List<DateTime> resultado = null;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var r = Convert.ToDateTime(reader["CAL_Fecha"]);

                    if (resultado == null)
                    {
                        resultado = new List<DateTime>();
                    }

                    resultado.Add(r);
                }
            }
            return resultado;

        }
        public List<DateTime> ObtenerFestivos(DateTime fechadesde)
        {
            string paispredeterminado = ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT);
            List<DateTime> Festivos = new List<DateTime>();
            if (Cache.Instancia.ContainsFestivos())
            {
                Festivos = Cache.Instancia.GetFestivos();
            }
            else
            {
                using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {
                    Festivos = contexto.CalendarioFestivos_PAR
                         .Where(cd => cd.CAL_IdPais == paispredeterminado)
                         .ToList()
                         .ConvertAll<DateTime>(fecha => fecha.CAL_Fecha.Date);
                    Cache.Instancia.AddFestivos(Festivos);
                }
            }

            return Festivos.Where
                  (cf =>
                  cf > fechadesde).ToList();

            //string paispredeterminado = ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT);
            //List<DateTime> Festivos = new List<DateTime>();
            //using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    Festivos = contexto.CalendarioFestivos_PAR.Where(cf =>
            //      cf.CAL_Fecha > fechadesde &&
            //      cf.CAL_IdPais == paispredeterminado).ToList().ConvertAll<DateTime>(
            //    fecha => fecha.CAL_Fecha);
            //}
            //return Festivos;
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabil(DateTime fechadesde, double numerodias, string idPais, out double numeroDiasHabiles)
        {
            numeroDiasHabiles = 0;
            DateTime fechaFinalHabil = DateTime.Now;
            DateTime fechaParcial = DateTime.Now;
            List<DateTime> Festivos = new List<DateTime>();
            List<DateTime> otrosFestivos = new List<DateTime>();
            DateTime fechaHasta = fechadesde.AddDays(numerodias);
            Festivos = ObtenerFestivos(fechadesde, fechaHasta, idPais);
            fechaFinalHabil = fechadesde.AddDays(numerodias + Festivos.Count);
            fechaParcial = fechaFinalHabil;
            int NumDomingos = 0;
            if (Convert.ToInt32(DateTime.Now.DayOfWeek) != 6)
            {
                NumDomingos = ObtenerNumeroDeDomingos(fechadesde, fechaFinalHabil);
                fechaFinalHabil = fechaFinalHabil.AddDays(NumDomingos);
            }

            if (NumDomingos > 0 || Convert.ToInt32(DateTime.Now.DayOfWeek) == 6)
            {
                otrosFestivos = ObtenerFestivos(fechaParcial, fechaFinalHabil, idPais);
                fechaFinalHabil = fechaFinalHabil.AddDays(otrosFestivos.Count);
            }
            double numeroHorasHabiles = fechaFinalHabil.Subtract(fechadesde).Hours;
            numeroDiasHabiles = numeroHorasHabiles / 24 + fechaFinalHabil.Subtract(fechadesde).Days;

            return fechaFinalHabil;
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos NO teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabilSinSabados(DateTime fechadesde, double numerodias, string idPais, out double numeroDiasHabiles)
        {
            numeroDiasHabiles = 0;
            DateTime fechaFinalHabil = DateTime.Now;
            DateTime fechaParcial = DateTime.Now;
            List<DateTime> Festivos = new List<DateTime>();
            List<DateTime> otrosFestivos = new List<DateTime>();
            DateTime fechaHasta = fechadesde.AddDays(numerodias);
            Festivos = ObtenerFestivos(fechadesde, fechaHasta, idPais);
            fechaFinalHabil = fechadesde.AddDays(numerodias + Festivos.Count);
            fechaParcial = fechaFinalHabil;
            int NumDomingos = 0;
            if (Convert.ToInt32(DateTime.Now.DayOfWeek) != 6)
            {
                NumDomingos = ObtenerNumeroDeDomingos(fechadesde, fechaFinalHabil);
                fechaFinalHabil = fechaFinalHabil.AddDays(NumDomingos);
            }

            if (NumDomingos > 0 || Convert.ToInt32(DateTime.Now.DayOfWeek) == 6)
            {
                otrosFestivos = ObtenerFestivos(fechaParcial, fechaFinalHabil, idPais);
                fechaFinalHabil = fechaFinalHabil.AddDays(otrosFestivos.Count);
            }

            double numeroHorasHabiles = fechaFinalHabil.Subtract(fechadesde).Hours;
            numeroDiasHabiles = numeroHorasHabiles / 24 + fechaFinalHabil.Subtract(fechadesde).Days;

            if (fechaFinalHabil.DayOfWeek == DayOfWeek.Saturday)
            {
                double numeroDiasHabilesSabado = 0;
                ObtenerFechaFinalHabilSinSabados(fechaFinalHabil, 1, idPais, out numeroDiasHabilesSabado);
                numeroDiasHabiles = numeroDiasHabiles + numeroDiasHabilesSabado;
                fechaFinalHabil = fechaFinalHabil.AddDays(numeroDiasHabilesSabado);
            }

            return fechaFinalHabil;
        }



        /// <summary>
        /// Retorn la cantidad de domingos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public int ObtenerNumeroDeDomingos(DateTime fechadesde, DateTime fechahasta)
        {
            int cuentaDomingos = 0;
            fechadesde = new DateTime(fechadesde.Year, fechadesde.Month, fechadesde.Day);
            fechahasta = new DateTime(fechahasta.Year, fechahasta.Month, fechahasta.Day);
            while (fechadesde <= fechahasta)
            {
                if (fechadesde.DayOfWeek == 0)
                {
                    cuentaDomingos++;
                }
                fechadesde = fechadesde.AddDays(1);

            }
            return cuentaDomingos;
            //TimeSpan ts = fechahasta - fechadesde;
            //int diashabiles = ts.Days - (ts.Days / 7);
            //return ts.Days - diashabiles;
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales a una fecha predeterminada
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public DateTime AgregarDiasLaborales(DateTime fechaOriginal, int diasLaborables)
        {
            DateTime fechaTemporal = fechaOriginal.Date;
            List<DateTime> festivos = ObtenerFestivos(fechaOriginal.Date);
            while (diasLaborables > 0)
            {
                fechaTemporal = fechaTemporal.AddDays(1);
                if (!(fechaTemporal.DayOfWeek == DayOfWeek.Saturday || fechaTemporal.DayOfWeek == DayOfWeek.Sunday || festivos.Contains(fechaTemporal)))
                {
                    diasLaborables--;
                }
            }
            return fechaTemporal;
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales entre dos fechas
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public int ConsultarDiasLaborales(DateTime fechaInicial, DateTime fechaFinal)
        {
            int diasRetorna;
            int diasLaborables = fechaFinal.Subtract(fechaInicial).Days;
            List<DateTime> festivos = ObtenerFestivos(fechaInicial.Date);
            if (fechaInicial < fechaFinal)
            {
                diasRetorna = diasLaborables;
                DateTime fechaTemporal = fechaInicial.Date;
                while (diasLaborables > 0)
                {
                    fechaTemporal = fechaTemporal.AddDays(1);
                    if (fechaTemporal.DayOfWeek == DayOfWeek.Saturday || fechaTemporal.DayOfWeek == DayOfWeek.Sunday || festivos.Contains(fechaTemporal))
                    {
                        diasRetorna--;
                    }
                    diasLaborables--;
                }
                return diasRetorna;
            }
            else
            {
                diasRetorna = diasLaborables;
                DateTime fechaTemporal = fechaInicial.Date;
                while (diasLaborables < 0)
                {
                    fechaTemporal = fechaTemporal.AddDays(1);
                    if (fechaTemporal.DayOfWeek == DayOfWeek.Saturday || fechaTemporal.DayOfWeek == DayOfWeek.Sunday || festivos.Contains(fechaTemporal))
                    {
                        diasRetorna++;
                    }
                    diasLaborables++;
                }
                return diasRetorna;
            }
        }

        #endregion Calendario Festivos x Pais

        #region Archivos framework

        /// <summary>
        /// Adiciona los archivos del framework
        /// </summary>
        /// <param name="archivo">objeto con la informacion del archivo a guardar</param>
        /// <param name="filePath">Ruta de donde se cargaran los archivos</param>
        public void AdicionarArchivosFramework(PAArchivosFramework archivo, string filePath)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (archivo != null)
                {
                    string rutaArchivo = Path.Combine(filePath, archivo.NombreServidor);
                    ArchivosFramework arch = new ArchivosFramework()
                    {
                        //AFW_Adjunto = System.IO.File.ReadAllBytes(rutaArchivo),
                        AFW_Descripcion = archivo.Descripcion,
                        AFW_FechaCargaArchivo = archivo.FechaCargaArchivo,
                        AFW_IdAdjunto = Guid.NewGuid(),
                        AFW_NombreAdjunto = archivo.NombreAdjunto,
                        AFW_Usuario = ControllerContext.Current.Usuario
                    };

                    using (FileStream fs = File.OpenRead(rutaArchivo))
                    {
                        byte[] bytes = new byte[fs.Length];
                        fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                        fs.Close();
                        arch.AFW_Adjunto = bytes;
                    }

                    contexto.ArchivosFrameworks.Add(arch);
                    contexto.SaveChanges();
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException
                         (
                         COConstantesModulos.CLIENTES,
                         ETipoErrorFramework.EX_FALLO_ADJUNTAR_ARCHIVO.ToString(),
                         MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_FALLO_ADJUNTAR_ARCHIVO)
                         ));
                }
            }
        }

        /// <summary>
        ///Metodo para obtener una plantilla del framework
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns>Archivo</returns>
        public byte[] ObtenerPlantillaFramework(long idPlantilla)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ArchivosFramework archivo = contexto.Plantilla_PAR.Where(p => p.PLA_IdPlantilla == idPlantilla).Join
                 (contexto.ArchivosFrameworks, p => p.PLA_IdArchivo, a => a.AFW_IdArchivo, (p, a) => a).SingleOrDefault();

                if (archivo != null)
                    return archivo.AFW_Adjunto;
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene los grupos para realizar la divulgacion
        /// </summary>
        /// <returns></returns>
        public PADivulgacion ObtenerGruposDivulgacion(int idAlerta)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var alertas = contexto.TipoAlertaDestinatarios_VPAR.Where(t => t.DEA_IdAlerta == idAlerta).ToList();

                //&& grupos.Contains(t.DEA_IdGrupoAlerta)

                if (alertas.Count == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.EX_ALERTA_NO_CONFIGURADA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ALERTA_NO_CONFIGURADA)));
                }

                PADivulgacion divulgacion = new PADivulgacion()
                {
                    Asunto = alertas.First().TIA_Asunto,
                    TemplateMensaje = alertas.First().TIA_TemplateMensaje,
                    Grupos = new System.Collections.ObjectModel.ObservableCollection<PAGrupoAlerta>()
                };

                var grupos = alertas.GroupBy(ag => ag.GRA_IdGrupoAlerta).Select(ag => ag.First()).ToList();

                grupos.ForEach(g =>
                  {
                      string destinatarios = string.Empty;
                      alertas.Where(a => a.GRA_IdGrupoAlerta == g.GRA_IdGrupoAlerta).ToList().ForEach(a =>
                        {
                            destinatarios = destinatarios != string.Empty ? string.Join(",", destinatarios, a.PEI_Email) : a.PEI_Email;
                        });

                      divulgacion.Grupos.Add(new PAGrupoAlerta()
                      {
                          CorrerosDestinatarios = destinatarios,
                          Descripcion = g.GRA_Descripcion,
                          IdGrupo = g.DEA_IdGrupoAlerta,
                          Seleccionado = false
                      });
                  });
                return divulgacion;
            }
        }

        #endregion Archivos framework

        #region Lista Restrictiva

        /// <summary>
        /// Obtiene los datos de la lista restrictiva
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Colección lista restrictiva</returns>
        public IEnumerable<PAListaRestrictivaDC> ObtenerListaRestrictiva(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsListaNegra_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new PAListaRestrictivaDC()
                  {
                      IdListaRestrictiva = r.LIN_IdRegistroListaNegra,
                      Identificacion = r.LIN_Identificacion,
                      Nombre = r.LIN_Nombre,
                      Concepto = r.LIN_Concepto,
                      Estado = r.LIN_Estado,
                      TipoListaRestrictiva = new PATipoListaRestrictivaDC()
                      {
                          IdTipoListaRestrictiva = r.TLN_IdTipoListaNegra,
                          Descripcion = contexto.TipoListaNegra_PAR.Where(w => w.TLN_IdTipoListaNegra == r.TLN_IdTipoListaNegra).SingleOrDefault().TLN_Descripcion
                      },
                      ColeccionEstados = ObtenerEstadosAplicacion(),
                      ColeccionTiposListasRestricitvas = ObtenerTiposListaRestrictiva(),
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona una lista restrictiva
        /// </summary>
        /// <param name="listaRestricitva">Objeto lista restricitva</param>
        public void AdicionarListaRestrictiva(PAListaRestrictivaDC listaRestricitva)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaNegra_PAR listaEn = new ListaNegra_PAR()
                {
                    LIN_Identificacion = listaRestricitva.Identificacion.Trim(),
                    TLN_IdTipoListaNegra = listaRestricitva.TipoListaRestrictiva.IdTipoListaRestrictiva,
                    LIN_Nombre = listaRestricitva.Nombre,
                    LIN_Concepto = listaRestricitva.Concepto,
                    LIN_Estado = listaRestricitva.Estado,
                    LIN_FechaActualizacion = DateTime.Now,
                    LIN_FechaGrabacion = DateTime.Now,
                    LIN_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ListaNegra_PAR.Add(listaEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita una lista restrictiva
        /// </summary>
        /// <param name="listaRestricitva">Objeto lista restrictiva</param>
        public void EditarListaRestrictiva(PAListaRestrictivaDC listaRestricitva)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaNegra_PAR listaEn = contexto.ListaNegra_PAR
                  .Where(w => w.LIN_IdRegistroListaNegra == listaRestricitva.IdListaRestrictiva)
                  .SingleOrDefault();

                if (listaEn == null)
                {
                    ControllerException excepcion = new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    listaEn.LIN_Identificacion = listaRestricitva.Identificacion.Trim();
                    listaEn.TLN_IdTipoListaNegra = listaRestricitva.TipoListaRestrictiva.IdTipoListaRestrictiva;
                    listaEn.LIN_Nombre = listaRestricitva.Nombre;
                    listaEn.LIN_Concepto = listaRestricitva.Concepto;
                    listaEn.LIN_Estado = listaRestricitva.Estado;
                    listaEn.LIN_FechaActualizacion = DateTime.Now;

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina una lista restrictiva
        /// </summary>
        /// <param name="listaRestricitva"></param>
        public void EliminarListaRestrictiva(PAListaRestrictivaDC listaRestricitva)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaNegra_PAR listaEn = contexto.ListaNegra_PAR
                  .Where(w => w.LIN_IdRegistroListaNegra == listaRestricitva.IdListaRestrictiva)
                  .SingleOrDefault();

                if (listaEn == null)
                {
                    ControllerException excepcion = new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    contexto.ListaNegra_PAR.Remove(listaEn);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene los tipos de lista restrictiva
        /// </summary>
        /// <returns>Colección tipo de lista restrictiva</returns>
        public IEnumerable<PATipoListaRestrictivaDC> ObtenerTiposListaRestrictiva()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoListaNegra_PAR
                  .ToList()
                  .ConvertAll(r => new PATipoListaRestrictivaDC()
                  {
                      IdTipoListaRestrictiva = r.TLN_IdTipoListaNegra,
                      Descripcion = r.TLN_Descripcion
                  });
            }
        }

        #endregion Lista Restrictiva

        #region Consecutivo

        /// <summary>
        /// Obtiene el consecutivo de acuerdo al id del tipo
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivo(PAEnumConsecutivos idConsecutivo)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConsecutivosGlobales_PAR consecutivo = contexto.ConsecutivosGlobales_PAR
                  .Where(r => r.CON_IdTipoConsecutivo == (int)idConsecutivo && r.CON_EstaActivo == true)
                  .FirstOrDefault();

                if (consecutivo != null)
                {
                    PAConsecutivoDC consecutivoActual = new PAConsecutivoDC();
                    consecutivoActual.Actual = consecutivo.CON_Actual;
                    return consecutivoActual;
                }
                else
                {
                    string mensaje = string.Format(ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO.ToString(), "0");
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, string.Format(ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO.ToString(), idConsecutivo.ToString()), string.Format(MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO), idConsecutivo.ToString()));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtiene la caja actual
        /// </summary>
        /// <param name="idTipoConsecutivo">Tipo de consecutivo</param>
        /// <returns>Objeto Consecutivo</returns>
        public PAConsecutivoDC ObtenerDatosConsecutivoxCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConsecutivoPorCol_PAR consecutivo = contexto.ConsecutivoPorCol_PAR
                  .Where(r => r.CCC_IdTipoConsecutivo == (int)idConsecutivo && r.CCC_EstaActivo == true && r.CCC_IdCentroLogistico == idCentroLogistico)
                  .FirstOrDefault();

                if (consecutivo != null)
                {
                    PAConsecutivoDC consecutivoActual = new PAConsecutivoDC();
                    consecutivoActual.Actual = consecutivo.CCC_Actual;
                    return consecutivoActual;
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        public PAConsecutivoDC ObtenerDatosConsecutivoIntentoEntregaxCol(PAEnumConsecutivos idConsecutivo, long idCentroLogistico)
        {

            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("pa_ConsultarConsecutivoColIntentoEntrega_LOI", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCentroLogistico);
                cmd.Parameters.AddWithValue("@IdTipoConsecutivo", (int)idConsecutivo);
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                                
                if(reader.Read())
                {
                    PAConsecutivoDC consecutivoActual = new PAConsecutivoDC();
                    consecutivoActual.Actual =  Convert.ToInt32(reader["CIE_Actual"]);
                    return consecutivoActual;
                }
                else
                {
                    return null;
                }                
            }

            //using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    ConsecutivoPorCol_PAR consecutivo = contexto.ConsecutivoPorCol_PAR
            //      .Where(r => r.CCC_IdTipoConsecutivo == (int)idConsecutivo && r.CCC_EstaActivo == true && r.CCC_IdCentroLogistico == idCentroLogistico)
            //      .FirstOrDefault();

            //    if (consecutivo != null)
            //    {
            //        PAConsecutivoDC consecutivoActual = new PAConsecutivoDC();
            //        consecutivoActual.Actual = consecutivo.CCC_Actual;
            //        return consecutivoActual;
            //    }
            //    else
            //    {
            //        ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO));
            //        throw new FaultException<ControllerException>(excepcion);
            //    }
            //}
        }

        #endregion Consecutivo

        #region Parientes

        /// <summary>
        /// Método para obtener los parentezcos configurados
        /// </summary>
        /// <returns></returns>
        public IList<PAParienteDC> ObtenerParientes()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Parentesto_PAR.Select(j =>
                  new PAParienteDC()
                  {
                      IdPariente = j.PAR_IdParentesto,
                      NombrePariente = j.PAR_Descripcion,
                  }
                  ).ToList();
            }
        }

        #endregion Parientes

        #region Estado empaque

        /// <summary>
        /// Obtiene los estados del empaque para un peso dado
        /// </summary>
        /// <param name="peso"></param>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerEstadosEmpaque(decimal peso)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoEmpaque_PAR.Where(r => r.TEE_PesoInicial < peso && r.TEE_PesoFinal >= peso).ToList().ConvertAll(r => new PAEstadoEmpaqueDC()
                {
                    DescripcionEstado = r.TEE_Descripcion,
                    IdEstadoEmpaque = r.TEE_IdEstadoEmpaque
                });
            }
        }

        /// <summary>
        /// Obtiene los estados del empaque
        /// </summary>
        /// <returns></returns>
        public List<PAEstadoEmpaqueDC> ObtenerTodosEstadosEmpaque()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoEmpaque_PAR.ToList().ConvertAll(r => new PAEstadoEmpaqueDC()
                {
                    DescripcionEstado = r.TEE_Descripcion,
                    IdEstadoEmpaque = r.TEE_IdEstadoEmpaque,
                    PesoFinal = r.TEE_PesoFinal,
                    PesoInicial = r.TEE_PesoInicial
                });
            }
        }

        #endregion Estado empaque

        #region Unidades de Medida

        /// <summary>
        /// Retorna las unidades de medida
        /// </summary>
        /// <returns></returns>
        public List<PAUnidadMedidaDC> ObtenerUnidadesMedida()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.UnidadMedida_PAR.ToList().ConvertAll(r => new PAUnidadMedidaDC()
                {
                    IdUnidadMedida = r.UNM_IdUnidadMedida,
                    Descripcion = r.UNM_Descripcion
                });
            }
        }

        #endregion Unidades de Medida

        #region Tipo Sector Cliente

        /// <summary>
        /// Obtiene todos los tipos de sector de cliente
        /// </summary>
        /// <returns></returns>
        public IList<PATipoSectorCliente> ObtenerTodosTipoSectorCliente()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSectorCliente_PAR.OrderBy(t => t.TSC_Descripcion).ToList().
                   ConvertAll(t =>
                   new PATipoSectorCliente()
                   {
                       EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                       IdTipoSectorCliente = t.TSC_IdTipoSectorCliente,
                       NombreTipo = t.TSC_Descripcion
                   });
            }
        }

        /// <summary>
        /// Obtener tipos de sector de cliente
        /// </summary>
        /// <typeparam name="?"></typeparam>
        /// <typeparam name="?"></typeparam>
        /// <param name="?"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IList<PATipoSectorCliente> ObtenerTipoSectorCliente(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoSectorCliente_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                   .ToList().ConvertAll(t =>
                     new PATipoSectorCliente()
                     {
                         EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                         IdTipoSectorCliente = t.TSC_IdTipoSectorCliente,
                         NombreTipo = t.TSC_Descripcion
                     });
            }
        }

        /// <summary>
        /// inserta un nuevo tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        public void AdicionarTipoSectorCliente(PATipoSectorCliente tipoSectorCliente)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoSectorCliente_PAR tipo = new TipoSectorCliente_PAR()
                {
                    TSC_CreadoPor = ControllerContext.Current.Usuario,
                    TSC_Descripcion = tipoSectorCliente.NombreTipo,
                    TSC_FechaGrabacion = DateTime.Now,
                    TSC_IdTipoSectorCliente = (short)tipoSectorCliente.IdTipoSectorCliente
                };

                contexto.TipoSectorCliente_PAR.Add(tipo);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// inserta un nuevo tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        public void EditarTipoSectorCliente(PATipoSectorCliente tipoSectorCliente)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoSectorCliente_PAR tipo = contexto.TipoSectorCliente_PAR.Where(t => t.TSC_IdTipoSectorCliente == tipoSectorCliente.IdTipoSectorCliente).FirstOrDefault();

                if (tipo == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                tipo.TSC_Descripcion = tipoSectorCliente.NombreTipo;

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// inserta un nuevo tipo de sector de cliente
        /// </summary>
        /// <param name="tipoSectorCliente"></param>
        public void EliminarTipoSectorCliente(PATipoSectorCliente tipoSectorCliente)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoSectorCliente_PAR tipo = contexto.TipoSectorCliente_PAR.Where(t => t.TSC_IdTipoSectorCliente == tipoSectorCliente.IdTipoSectorCliente).FirstOrDefault();

                if (tipo == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                contexto.TipoSectorCliente_PAR.Remove(tipo);

                contexto.SaveChanges();
            }
        }

        #endregion Tipo Sector Cliente

        #region OperadorPostal

        /// <summary>
        /// Obtiene el operador postal
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<PAOperadorPostal> ObtenerOperadorPostal(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsOperadorPostal_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll(r => new PAOperadorPostal()
                {
                    Id = r.OPO_IdOperadorPostal,
                    Nombre = r.OPO_Nombre,
                    PorcentajeCombustible = r.OPO_PorcentajeCombustible,
                    Descripcion = r.OPO_Descripcion,
                    FormulaPesoVolumetrico = r.OPO_FormulaPesoVolumetrico
                });
            }
        }

        /// <summary>
        /// Edita un Operador postal
        /// </summary>
        /// <param name="operador"></param>
        public void EditarOperadorPostal(PAOperadorPostal operador)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostal_PAR op = contexto.OperadorPostal_PAR.Where(r => r.OPO_IdOperadorPostal == operador.Id).SingleOrDefault();

                if (op == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                op.OPO_Descripcion = operador.Descripcion;
                op.OPO_Nombre = operador.Nombre;
                op.OPO_PorcentajeCombustible = operador.PorcentajeCombustible;
                op.OPO_FormulaPesoVolumetrico = operador.FormulaPesoVolumetrico;

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina el operador postal
        /// </summary>
        /// <param name="operador"></param>
        public void EliminarOperadorPostal(PAOperadorPostal operador)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostal_PAR op = contexto.OperadorPostal_PAR.Where(r => r.OPO_IdOperadorPostal == operador.Id).SingleOrDefault();

                if (op == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_GENERALES, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                contexto.OperadorPostal_PAR.Remove(op);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona un tipo de actividad Economica
        /// </summary>
        /// <param name="docuCentroServ">Objeto tipo de actividad economica</param>
        public void AdicionarOperadorPostal(PAOperadorPostal operador)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                OperadorPostal_PAR opePostal = new OperadorPostal_PAR()
                {
                    OPO_Nombre = operador.Nombre,
                    OPO_Descripcion = operador.Descripcion,
                    OPO_PorcentajeCombustible = operador.PorcentajeCombustible,
                    OPO_FormulaPesoVolumetrico = operador.FormulaPesoVolumetrico,
                    OPO_CreadoPor = ControllerContext.Current.Usuario,
                    OPO_FechaGrabacion = DateTime.Now
                };
                contexto.OperadorPostal_PAR.Add(opePostal);
                contexto.SaveChanges();
            }
        }

        #endregion OperadorPostal

        /// <summary>
        /// Consulta la url en la cual se encuentra ubicada la app de carga masiva de guias o  facturas
        /// </summary>
        /// <returns></returns>
        public string ConsultarURLAppCargaMasiva()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosFramework parFr = contexto.ParametrosFramework.Where(p => p.PAR_IdParametro == "UrlCargueMasivo").FirstOrDefault();
                if (parFr != null)
                    return parFr.PAR_ValorParametro;
                return null;
            }
        }

        /// <summary>
        /// Obtiene los numeros de guia que no tienen imagen en el servidor
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<string> ConsultarArchivosPendientesSincronizar(long idCol)
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmdTablasSync = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                cmdTablasSync.CommandText = @"paObtenerGuiasSinImagenFS_LOI";
                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                cmdTablasSync.Parameters.Add(new SqlParameter("@ARG_IdCentroLogistico", Convert.ToInt64(idCol)));

                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = cmdTablasSync;
                daTablasSync.Fill(ds);

                List<string> lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                      t.Field<long>("ARG_NumeroGuia", DataRowVersion.Current).ToString()
                     );
                return lst;
            }
        }

        /// <summary>
        /// Obtiene los centros de servicios que deberian estar en linea
        /// </summary>
        /// <returns></returns>
        public List<Framework.Servidor.Servicios.ContratoDatos.Notificador.CentrosServiciosLinea> ConsultarCentrosServiciosDeberianEstarLinea()
        {
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmdTablasSync = new SqlCommand();
                SqlDataAdapter daTablasSync = new SqlDataAdapter();
                cmdTablasSync.CommandText = @"paObtenerCentrosServiciosNotificador_FRM";
                cmdTablasSync.Connection = cnn;
                cmdTablasSync.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                daTablasSync.SelectCommand = cmdTablasSync;
                daTablasSync.Fill(ds);

                List<Framework.Servidor.Servicios.ContratoDatos.Notificador.CentrosServiciosLinea> lst = ds.Tables[0].AsEnumerable().ToList().ConvertAll(t =>
                    new Framework.Servidor.Servicios.ContratoDatos.Notificador.CentrosServiciosLinea()
                    {
                        IdNotificador = t.Field<string>("IdNotificador"),
                        NombreCentroServicios = t.Field<string>("UCS_NombreCentroServicios"),
                        IdCentroServicios = t.Field<long>("UCS_IdCentroServicios"),
                        IdCaja = t.Field<int>("UCS_Caja"),
                        IdCodUsuario = t.Field<int>("UCS_IdCodigoUsuario"),
                        NombrePersona = t.Field<string>("nombreCompletoPersona"),
                        IdentificacionPersona = t.Field<string>("PEI_Identificacion"),
                        NombreUsuario = t.Field<string>("USU_IdUsuario"),
                        TipoCentoServicios = t.Field<string>("CES_Tipo"),
                        NombreCol = t.Field<string>("NombreCol")


                    }).ToList();
                return lst;
            }
        }

        /// <summary>
        /// Retorna la imagen de publicidad asociada al login de la aplicación, si no retorna nada, no requiere visualizar publicidad
        /// </summary>
        /// <returns></returns>
        public ParamPublicidad ConsultarImagenPublicidadLogin()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParamPublicidad pub = new ParamPublicidad();
                ParametrosFramework parFr = contexto.ParametrosFramework.Where(p => p.PAR_IdParametro == "urlImgPubLogin").FirstOrDefault();

                if (parFr != null)
                {
                    // Armar la imagen
                    if (!string.IsNullOrWhiteSpace(parFr.PAR_ValorParametro))
                    {
                        try
                        {
                            pub.Imagen = Convert.ToBase64String(System.IO.File.ReadAllBytes(parFr.PAR_ValorParametro));
                        }
                        catch (FileNotFoundException )
                        {
                            pub.Imagen = null;
                        }
                    }
                    else pub.Imagen = null;
                }
                else pub.Imagen = null;

                ParametrosFramework parTiempo = contexto.ParametrosFramework.Where(p => p.PAR_IdParametro == "TiempoPublicidad").FirstOrDefault();
                int tiempo = 0;
                if (parTiempo != null && int.TryParse(parTiempo.PAR_ValorParametro, out tiempo))
                {
                    pub.TiempoPublicidad = tiempo;
                }

                return pub;
            }
        }


        /// <summary>
        /// Consulta las variaciones y abreviaciones del campo Direccion
        /// </summary>
        /// <returns></returns>
        public List<PAEstandarDireccionDC> ConsultarAbreviacionesVariacionesDireccion()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var result = from variaciones in contexto.VariacionesDireccion
                             join abreviaciones in contexto.AbreviaturasDireccion
                             on variaciones.VD_IdAbreviaturaDireccion equals abreviaciones.AD_id
                             select new PAEstandarDireccionDC { IdAbreviaturaDireccion = variaciones.VD_IdAbreviaturaDireccion, VariacionDireccion = variaciones.VD_VariacionDireccion, AbreviacionDireccion = abreviaciones.AD_abreviatura };
                return result.ToList();
            }
        }

        /// <summary>
        /// Consulta la informacion del mensajero
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="rol"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesMensajeroAplicacion(string codigo)
        {
            PAPropiedadesAplicacionDC response = null;
            using (SqlConnection conn = new SqlConnection())
            {
                SqlCommand cmd = new SqlCommand(@"paConsultarInformacionMensajero_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Codigo", codigo);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                conn.Close();

                if (reader.Read())
                {
                    response = new PAPropiedadesAplicacionDC() { 
                    
                        
                    };
                }
            }
            return response;
        }

        /// <summary>
        /// Consulta de infomracion del Auditor
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesAuditorAplicacion(string codigo)
        {
            return null;
        }

        /// <summary>
        /// Consultar infomracion del mensajero PAM
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesMensajeroPAMAplicacion(string codigo)
        {
            return null;
        }

        /// <summary>
        /// Consultar propiedades de agencia 
        /// </summary>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public PAPropiedadesAplicacionDC ConsultarPropiedadesAgenciaAplicacion(string codigo)
        {
            return null;
        }


        #region Menus de capacitación

        /// <summary>
        /// Obtiene los menus de capacitacion
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de Menus</returns>
        public IList<VEMenuCapacitacion> ObtenerMenus(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto
                    .ConsultarContainsMenuCapacitacion_VER(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .Select(obj => new VEMenuCapacitacion
                {
                    Activo = obj.MEC_Activo,
                    AplicaUsuario = obj.MEC_AplicaUsuario,
                    Descripcion = obj.MEC_Descripcion,
                    Id = obj.MEC_IdMenu,
                    IdAncestro = obj.MEC_IdAncestro,
                    IdProceso = (VEEnumProcesos)obj.MEC_IdProceso,
                    Target = obj.MEC_Target,
                    URL = obj.MEC_URL
                }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un Menu
        /// </summary>
        /// <param name="menu">Objeto banco</param>
        public void AdicionarMenu(VEMenuCapacitacion menu)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MenuCapacitacion_VER Men = new MenuCapacitacion_VER()
                {
                    MEC_Activo = menu.Activo,
                    MEC_AplicaUsuario = menu.AplicaUsuario,
                    MEC_CreadoPor = ControllerContext.Current.Usuario,
                    MEC_Descripcion = menu.Descripcion,
                    MEC_FechaGrabacion = DateTime.Now,
                    MEC_IdAncestro = menu.IdAncestro,
                    MEC_IdProceso = (int)menu.IdProceso,
                    MEC_Target = menu.Target,
                    MEC_URL = menu.URL
                };
                contexto.MenuCapacitacion_VER.Add(Men);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un Menu
        /// </summary>
        /// <param name="menu">Objeto Menu</param>
        public void EditarMenu(VEMenuCapacitacion menu)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MenuCapacitacion_VER Men = contexto.MenuCapacitacion_VER
                 .Where(obj => obj.MEC_IdMenu == menu.Id)
                 .SingleOrDefault();

                if (Men == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                Men.MEC_IdMenu = menu.Id;
                Men.MEC_IdAncestro = menu.IdAncestro;
                Men.MEC_URL = menu.URL;
                Men.MEC_Descripcion = menu.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un Menu
        /// </summary>
        /// <param name="menu">Objeto con la informacion de un Menu</param>
        public void EliminarMenu(VEMenuCapacitacion menu)
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MenuCapacitacion_VER Men = contexto
                    .MenuCapacitacion_VER
                    .Where(obj => obj.MEC_IdMenu == menu.Id).SingleOrDefault();

                if (Men != null)
                {
                    contexto.MenuCapacitacion_VER.Remove(Men);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Menus de capacitación

        #region Imagen Publicidad guia

        public void GuardarImagenPub(string rutaImagen, string imagenPublicidad)
        {
            try
            {

                byte[] imageBytes = Convert.FromBase64String(imagenPublicidad);

                MemoryStream ms = new MemoryStream(imageBytes);

                Image resizableImage = Image.FromStream(ms);

                resizableImage = resizeImage(resizableImage, new Size(628, 268));

                MemoryStream mst = new MemoryStream();

                resizableImage.Save(mst, System.Drawing.Imaging.ImageFormat.Png);

                imageBytes = mst.ToArray();

                File.WriteAllBytes(rutaImagen, imageBytes);

                System.Threading.Tasks.Task.Factory.StartNew(() => RegistrarAuditoriaImagenPub());

            }
            catch (Exception error)
            {
                throw new Exception("Error GuardarImagenPub - " + error);
            }
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }


        private void RegistrarAuditoriaImagenPub()
        {
            using (ModeloParametrosConn contexto = new ModeloParametrosConn(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                try
                {
                    ParametrosAdmisiones_MEN parametroConsulta = contexto.ParametrosAdmisiones_MEN.FirstOrDefault(par => par.PAM_IdParametro == "ImagenPublicidadGuia");

                    ParametrosAdmisionesHist_MEN parametroAuditoria = new ParametrosAdmisionesHist_MEN()
                    {
                        PAM_IdParametro = parametroConsulta.PAM_IdParametro,

                        PAM_ValorParametro = parametroConsulta.PAM_ValorParametro,

                        PAM_Descripcion = parametroConsulta.PAM_Descripcion,

                        PAM_FechaGrabacion = parametroConsulta.PAM_FechaGrabacion,

                        PAM_CreadoPor = parametroConsulta.PAM_CreadoPor,

                        PAM_FechaCambio = DateTime.Now,

                        PAM_CambiadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.ParametrosAdmisionesHist_MEN.Add(parametroAuditoria);

                    contexto.SaveChanges();
                }

                catch (Exception error)
                {
                    throw new Exception("Error RegistrarAuditoriaImagenPub - " + error);
                }
            }
        }


        #endregion

        #region Notificaciones Moviles

        /// <summary>
        /// Registra un nuevo dispositivo movil dentro de la plataforma
        /// </summary>
        /// <param name="dispositivo"></param>
        public long RegistrarDispositivoMovil(PADispositivoMovil dispositivo)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("PARegistrarDispositivoMovil_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@DIM_SistemaOperativo", dispositivo.SistemaOperativo.ToString());
                cmd.Parameters.AddWithValue("@DIM_TokenDispositivo", dispositivo.TokenDispositivo);
                cmd.Parameters.AddWithValue("@DIM_TipoDispositivo", dispositivo.TipoDispositivo.ToString());
                cmd.Parameters.AddWithValue("@DIM_CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@DIM_IdLocalidad", dispositivo.IdCiudad);
                cmd.Parameters.AddWithValue("@DIM_NumeroImei", dispositivo.NumeroImei);
                if (ControllerContext.Current.IdAplicativoOrigen > 0)
                {
                    cmd.Parameters.AddWithValue("@DIM_IdOrigenAplicacion", ControllerContext.Current.IdAplicativoOrigen);
                }
                if (ControllerContext.Current.Identificacion > 0)
                {
                    cmd.Parameters.AddWithValue("@DIM_NumeroIdentificacion", ControllerContext.Current.Identificacion);
                }


                object idDispositivo = cmd.ExecuteScalar();
                conn.Close();

                return Convert.ToInt64(idDispositivo);

            }
        }

        /// <summary>
        /// Inactiva un dispositivo movil registrado
        /// </summary>
        /// <param name="dispositivo"></param>
        public void InactivarDispositivoMovil(PADispositivoMovil dispositivo)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paInactivarDispositivoMovil_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DIM_TokenDispositivo", dispositivo.TokenDispositivo);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Obtiene todos los dispositivos de registrados de los peatones o los empleados
        /// </summary>
        /// <param name="tipoDispositivo">Indica si se filtra por los dispositivos de los empleados o los peatones</param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerTodosDispositivosPeatonEmpleados(PAEnumTiposDispositivos tipoDispositivo)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDispositivosMovilesPeatonesEmpleados_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DIM_TipoDispositivo", tipoDispositivo.ToString());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                List<PADispositivoMovil> dispositivos = new List<PADispositivoMovil>();

                dispositivos = dt.AsEnumerable().ToList().ConvertAll<PADispositivoMovil>(t =>
                    new PADispositivoMovil()
                    {
                        IdDispositivo = t.Field<long>("DIM_IdDispositivo"),
                        SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), t.Field<string>("DIM_SistemaOperativo")),
                        TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), t.Field<string>("DIM_TipoDispositivo")),
                        TokenDispositivo = t.Field<string>("DIM_TokenDispositivo")
                    });

                conn.Close();

                return dispositivos;
            }

        }

        /// <summary>
        /// Obtiene los dispositivos moviles de los empleados en una ciudad
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerDispositivosMovilesEmpleadosCiudad(string idLocalidad, bool esControllerApp = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDispositivosMovilesEmpleadosCiudad_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DIM_TipoDispositivo", PAEnumTiposDispositivos.DEM.ToString());
                cmd.Parameters.AddWithValue("@DIM_IdLocalidad", idLocalidad);
                cmd.Parameters.AddWithValue("@esControllerApp", esControllerApp);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                List<PADispositivoMovil> dispositivos = new List<PADispositivoMovil>();

                dispositivos = dt.AsEnumerable().ToList().ConvertAll<PADispositivoMovil>(t =>
                    new PADispositivoMovil()
                    {
                        IdDispositivo = t.Field<long>("DIM_IdDispositivo"),
                        SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), t.Field<string>("DIM_SistemaOperativo")),
                        TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), t.Field<string>("DIM_TipoDispositivo")),
                        TokenDispositivo = t.Field<string>("DIM_TokenDispositivo"),
                        IdCiudad = t.Field<string>("DIM_IdLocalidad")

                    });
                conn.Close();
                return dispositivos;
            }

        }


        /// <summary>
        ///  Obtiene los dispositivos el dispositivo movil asociado a la identificacion de un empleado
        /// </summary>
        /// <param name="numeroIdentificacion"></param>
        /// <returns></returns>
        public List<PADispositivoMovil> ObtenerDispositivosMovilesIdentificacionEmpleado(long numeroIdentificacion)
        {
            List<PADispositivoMovil> resultado = null;
            using(SqlConnection conn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerDispositivosMovilesIdentificacionEmpleado_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroIdentificacion", numeroIdentificacion);
                conn.Open();
                var reader = cmd.ExecuteReader();
                resultado = MapperRepositorio.ToDispositivoMovil(reader);                
                return resultado;
            }
        }

        /// <summary>
        /// Obtiene los dispositivos el dispositivo movil asociado a una empleado
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public PADispositivoMovil ObtenerDispositivoMovilEmpleado(string nombUsuario)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerDispositivoMovilEmpleado_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DIM_TipoDispositivo", PAEnumTiposDispositivos.DEM.ToString());
                cmd.Parameters.AddWithValue("@NombreUsuario", nombUsuario);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var disp = dt.AsEnumerable().ToList().FirstOrDefault();

                if (disp == null)
                    return null;



                PADispositivoMovil dispositivo = new PADispositivoMovil()
                    {
                        IdDispositivo = disp.Field<long>("DIM_IdDispositivo"),
                        SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), disp.Field<string>("DIM_SistemaOperativo")),
                        TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), disp.Field<string>("DIM_TipoDispositivo")),
                        TokenDispositivo = disp.Field<string>("DIM_TokenDispositivo"),
                        IdCiudad = disp.Field<string>("DIM_IdLocalidad")

                    };

                return dispositivo;
            }

        }



        /// <summary>
        /// Obtiene un dispositivo movil a partir del token y del sistema operativo
        /// </summary>
        /// <param name="tokenMovil"></param>
        /// <returns></returns>
        public PADispositivoMovil ObtenerDispositivoMovilTokenOs(string tokenMovil, PAEnumOsDispositivo sistemaOperativo)
        {

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDispositivoMovilPorTokenOs_OPU", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DIM_TokenDispositivo", tokenMovil);
                cmd.Parameters.AddWithValue("@DIM_SistemaOperativo", sistemaOperativo.ToString());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                conn.Close();

                var dis = dt.AsEnumerable().ToList().FirstOrDefault();
                if (dis == null)
                {
                    return null;
                }

                PADispositivoMovil dispositivo = new PADispositivoMovil()
                {
                    IdDispositivo = dis.Field<long>("DIM_IdDispositivo"),
                    SistemaOperativo = (PAEnumOsDispositivo)Enum.Parse(typeof(PAEnumOsDispositivo), dis.Field<string>("DIM_SistemaOperativo")),
                    TipoDispositivo = (PAEnumTiposDispositivos)Enum.Parse(typeof(PAEnumTiposDispositivos), dis.Field<string>("DIM_TipoDispositivo")),
                    TokenDispositivo = dis.Field<string>("DIM_TokenDispositivo")

                };

                return dispositivo;
            }


        }

        #endregion


     
        public List<DateTime> ObtenerDiasNoHabiles()
        {

            List<DateTime> lstDias = new List<DateTime>();
            DateTime fechaInicial = Convert.ToDateTime("01/01/" + DateTime.Now.Year);
            DateTime fechaFinal = Convert.ToDateTime("31/12/" + DateTime.Now.Year);
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerDiasNoHabiles_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicial);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFinal);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime diaNoHabil = new DateTime();
                    diaNoHabil = Convert.ToDateTime(reader["DNH_Fecha"]);
                    lstDias.Add(diaNoHabil);
                }
                conn.Close();
            }
            return lstDias;
        }

        /// <summary>
        /// Verifica si la base de  datos está disponible
        /// </summary>
        /// <returns></returns>
        public bool VerificarConexionBD()
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("paVerificarDisponibilidadBD_PAR", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return true;
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Obtiene la lista de programas Congeladores de Disco duro
        /// </summary>
        /// <returns></returns>
        public List<PAFreezersDiscoDC> ObtenerListaFreezersDisco()
        {
            List<PAFreezersDiscoDC> lstCon = new List<PAFreezersDiscoDC>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerListaFreezersDisco_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PAFreezersDiscoDC objCongeladores = new PAFreezersDiscoDC
                    {
                        NombrePrograma = reader["LFD_NombrePrograma"].ToString(),
                        NombreProceso = reader["LFD_NombreProceso"].ToString(),
                        NombreServicio = reader["LFD_NombreServicio"].ToString()
                    };
                    lstCon.Add(objCongeladores);
                }
            }
            return lstCon;
        }

    }
}