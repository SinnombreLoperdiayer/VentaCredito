using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos.Modelo;
using Framework.Servidor.Agenda;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Configuration;
using System.Data;

namespace CO.Servidor.Suministros.Datos
{
    public partial class SURepositorioAdministracion
    {
        private const string NombreModelo = "ModeloSuministros";
        private string conexionString = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Singleton

        private static readonly SURepositorioAdministracion instancia = new SURepositorioAdministracion();

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static SURepositorioAdministracion Instancia
        {
            get { return SURepositorioAdministracion.instancia; }
        }

        #endregion Singleton

        #region Administracion de suministro

        /// <summary>
        /// Valida si el numero del suministro seleccionado ya fue consumido
        /// </summary>
        /// <param name="suministro"></param>
        /// <param name="numeroSuministro"></param>
        /// <returns>True si fue consumido - False si no se ha consumido</returns>
        public bool ValidaConsumoSuministroRango(int suministro, long rangoInicial, long rangoFinal)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConsumoSuministro_SUM consumo =
                    contexto.ConsumoSuministro_SUM
                    .Where(r => r.CSU_IdSuministro == suministro
                        && (r.CSU_NumeroSuministro <= rangoInicial && r.CSU_NumeroSuministro >= rangoFinal))
                    .FirstOrDefault();
                if (consumo == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Valida si el numero del suministro seleccionado ya fue consumido
        /// </summary>
        /// <param name="suministro"></param>
        /// <param name="numeroSuministro"></param>
        /// <returns>True si fue consumido - False si no se ha consumido</returns>
        public bool ValidaConsumoSuministro(int suministro, long numeroSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConsumoSuministro_SUM consumo = contexto.ConsumoSuministro_SUM.Where(r => r.CSU_IdSuministro == suministro && r.CSU_NumeroSuministro == numeroSuministro).FirstOrDefault();
                if (consumo == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Retorna las categorias de los suministros
        /// </summary>
        /// <returns></returns>
        public List<SUCategoriaSuministro> ObtenerCategoriasSuministros()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CategoriaSuministro_SUM.ToList()
                  .ConvertAll(r => new SUCategoriaSuministro()
                  {
                      IdCategoria = r.CAS_IdCategoriaSuministro,
                      Descripcion = r.CAS_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene un suministro por codigo ERP
        /// </summary>
        /// <param name="codigoERP"></param>
        /// <returns></returns>
        public SUSuministro ObtenerSuministroXCodigoERP(string codigoERP)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM suministro = contexto.Suministro_SUM.Where(r => r.SUM_CodigoERP == codigoERP.Trim()).FirstOrDefault();

                if (suministro != null)
                {
                    return new SUSuministro()
                    {
                        Id = suministro.SUM_IdSuministro
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene un suministro por codigo ERP
        /// </summary>
        /// <param name="codigoERP"></param>
        /// <returns></returns>
        public SUSuministro ObtenerSuministroXCodigoAlterno(string codigoAlterno)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM suministro = contexto.Suministro_SUM.Where(r => r.SUM_CodigoAlterno == codigoAlterno).FirstOrDefault();

                if (suministro != null)
                {
                    return new SUSuministro()
                    {
                        Id = suministro.SUM_IdSuministro
                    };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Obtiene un suministro por codigo ERP
        /// </summary>
        /// <param name="codigoERP"></param>
        /// <returns></returns>
        public bool ValidarSuministroXCodigoERP(string codigoERP, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM Suministro = contexto.Suministro_SUM.Where(r => r.SUM_IdSuministro == idSuministro).FirstOrDefault();

                if (Suministro.SUM_CodigoERP == codigoERP)
                    return true;
                else
                    if (contexto.Suministro_SUM.Where(r => r.SUM_CodigoERP == codigoERP).FirstOrDefault() == null)
                        return true;
                    else
                        return false;
            }
        }

        /// <summary>
        /// Obtiene un suministro por codigo ERP
        /// </summary>
        /// <param name="codigoERP"></param>
        /// <returns></returns>
        public bool ValidarSuministroXCodigoAlterno(string codigoAlterno, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM Suministro = contexto.Suministro_SUM.Where(r => r.SUM_IdSuministro == idSuministro).FirstOrDefault();

                if (Suministro.SUM_CodigoAlterno == codigoAlterno)
                    return true;
                else
                    if (contexto.Suministro_SUM.Where(r => r.SUM_CodigoAlterno == codigoAlterno).FirstOrDefault() == null)
                        return true;
                    else
                        return false;
            }
        }

        /// <summary>
        /// Retorna los todos los suministros activos registrados paginados
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerTodosSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (!filtro.ContainsKey("SUM_Estado"))
                {
                    filtro.Add("SUM_Estado", ConstantesFramework.ESTADO_ACTIVO);
                }
                string Estado;
                filtro.TryGetValue("SUM_Estado", out Estado);

                return contexto.ConsultarContainsSuministrosUnidadMedida_VSUM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SUM_IdSuministro,
                      Descripcion = r.SUM_Descripcion,
                      AplicaResolucion = r.SUM_AplicaResolucion,
                      CategoriaSuministro = new SUCategoriaSuministro()
                      {
                          IdCategoria = r.SUM_IdCategoria,
                      },
                      SePreimprime = r.SUM_SePreImprime,
                      CuentaGasto = r.SUM_CuentaGasto,
                      CodigoAlterno = r.SUM_CodigoAlterno,
                      CodigoERP = r.SUM_CodigoERP,
                      Prefijo = r.SUM_Prefijo,
                      UnidadMedida = new PAUnidadMedidaDC()
                      {
                          IdUnidadMedida = r.UNM_IdUnidadMedida,
                          Descripcion = r.UNM_Descripcion
                      },
                      EstaActivo = r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                  });
            }
        }

        /// <summary>
        /// Obtiene todos los suministros
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministros()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Suministro_SUM.Where(r => r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO).OrderBy(or => or.SUM_Descripcion)
                  .ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SUM_IdSuministro,
                      Descripcion = r.SUM_Descripcion,
                      Prefijo = r.SUM_Prefijo
                  });
            }
        }

        public SUDatosResponsableSuministroDC ObtenerResponsableSuministro(long numeroGuia)
        {
            var datosSuministro = new SUDatosResponsableSuministroDC();
            using (SqlConnection sqlConn = new SqlConnection(conexionString))            
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerDatosSuministros_SUM", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader lector = cmd.ExecuteReader();

                while (lector.Read())
                {
                    datosSuministro.IdResponsableGuia = Convert.ToInt64(lector["IdResponsableGuia"]);
                    datosSuministro.NombreResponsable = Convert.ToString(lector["NombreResponsable"]);
                    datosSuministro.PrimerApellido = Convert.ToString(lector["PrimerApellido"]);
                    datosSuministro.SegundoApellido = Convert.ToString(lector["SegundoApellido"]);
                    datosSuministro.Identificacion = Convert.ToString(lector["Identificacion"]);
                    datosSuministro.Telefono = Convert.ToString(lector["Telefono"]);
                    datosSuministro.IdCentroServicios = Convert.ToInt64(lector["IdCentroServicio"]);
                    datosSuministro.NombreCentroServicio = Convert.ToString(lector["NombreCentroServicio"]);
                    datosSuministro.Direccion = Convert.ToString(lector["Direccion"]);
                    datosSuministro.IdLocalidad = Convert.ToString(lector["IdLocalidad"]);
                    datosSuministro.NombreLocalidad = Convert.ToString(lector["NombreLocalidad"]);
                    datosSuministro.CodigoPostal = Convert.ToString(lector["CodigoPostal"]);                    
                }
                sqlConn.Close();
            }

            return datosSuministro;
        }

        /// <summary>
        /// Obtener los suministros activos y que sean para preimprimir
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosPreImpresion()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Suministro_SUM.Where(r => r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO && r.SUM_SePreImprime == true)
                  .OrderBy(or => or.SUM_Descripcion)
                  .ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SUM_IdSuministro,
                      Descripcion = r.SUM_Descripcion,
                      Prefijo = r.SUM_Prefijo
                  });
            }
        }

        /// <summary>
        /// Guarda el sumunistro configurado
        /// </summary>
        /// <param name="suministro"></param>
        public void GuardarSuministro(SUSuministro suministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM suministroEn = new Suministro_SUM()
                {
                    SUM_AplicaResolucion = suministro.AplicaResolucion,
                    SUM_CodigoAlterno = suministro.CodigoAlterno,
                    SUM_CodigoERP = suministro.CodigoERP,
                    SUM_CreadoPor = ControllerContext.Current.Usuario,
                    SUM_CuentaGasto = suministro.CuentaGasto,
                    SUM_Descripcion = suministro.Descripcion,
                    SUM_FechaGrabacion = DateTime.Now,
                    SUM_IdCategoria = (short)suministro.CategoriaSuministro.IdCategoria,
                    SUM_SePreImprime = suministro.SePreimprime,
                    SUM_IdUnidadMedida = suministro.UnidadMedida.IdUnidadMedida,
                    SUM_Estado = suministro.EstaActivo == true ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO,
                    SUM_Prefijo = suministro.Prefijo
                };
                contexto.Suministro_SUM.Add(suministroEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita el suministro seleccionado
        /// </summary>
        /// <param name="suministro"></param>
        public void EditarSuministro(SUSuministro suministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM suministroEn = contexto.Suministro_SUM.Where(r => r.SUM_IdSuministro == suministro.Id).FirstOrDefault();

                suministroEn.SUM_AplicaResolucion = suministro.AplicaResolucion;
                suministroEn.SUM_IdCategoria = (short)suministro.CategoriaSuministro.IdCategoria;
                suministroEn.SUM_CodigoAlterno = suministro.CodigoAlterno;
                suministroEn.SUM_CodigoERP = suministro.CodigoERP;
                suministroEn.SUM_Descripcion = suministro.Descripcion;
                suministroEn.SUM_CuentaGasto = suministro.CuentaGasto;
                suministroEn.SUM_IdUnidadMedida = suministro.UnidadMedida.IdUnidadMedida;
                suministroEn.SUM_SePreImprime = suministro.SePreimprime;
                suministroEn.SUM_Estado = suministro.EstaActivo == true ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
                suministroEn.SUM_Prefijo = suministro.Prefijo;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// trae la info de un suministro por su
        /// id
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns>info suministro</returns>
        public SUSuministro ObtenerSuministro(int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM sumin = contexto.Suministro_SUM.FirstOrDefault(su => su.SUM_IdSuministro == idSuministro);
                SUSuministro infosuministro = new SUSuministro();

                if (sumin != null)
                {
                    infosuministro = new SUSuministro()
                    {
                        Descripcion = sumin.SUM_Descripcion,
                        EstaActivo = sumin.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                        CodigoERP = sumin.SUM_CodigoERP,
                    };
                }
                return infosuministro;
            }
        }

        /// <summary>
        /// Actuliza el estado del Suministro de acuerdo a la
        /// edicion del mismo en la Administracion del suministro
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="estadoSuministro"></param>
        public void ActualizarEstadoSuministroCentroServicio(int idSuministro, string estadoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<SuministrosCentroServicios_SUM> lstSumCtrSvc = contexto.SuministrosCentroServicios_SUM.Where(sumCtr => sumCtr.SCS_IdSuministro == idSuministro).ToList();

                if (lstSumCtrSvc != null && lstSumCtrSvc.Count > 0)
                {
                    lstSumCtrSvc.ForEach(sum =>
                    {
                        sum.SCS_Estado = estadoSuministro;
                    });
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Actuliza el estado del Suministro de acuerdo a la
        /// edicion del mismo en la Administracion del suministro
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="estadoSuministro"></param>
        public void ActualizarEstadoSuministroMensajero(int idSuministro, string estadoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<SuministrosMensajero_SUM> lstSumMensajero = contexto.SuministrosMensajero_SUM.Where(sumMjr => sumMjr.SME_IdSuministro == idSuministro).ToList();

                if (lstSumMensajero != null && lstSumMensajero.Count > 0)
                {
                    lstSumMensajero.ForEach(sum =>
                    {
                        sum.SME_Estado = estadoSuministro;
                    });
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Actuliza el estado del Suministro de acuerdo a la
        /// edicion del mismo en la Administracion del suministro
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="estadoSuministro"></param>
        public void ActualizarEstadoSuministroProceso(int idSuministro, string estadoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<SuministrosProceso_SUM> lstSumPrcso = contexto.SuministrosProceso_SUM.Where(sumPrcs => sumPrcs.SUP_IdSuministro == idSuministro).ToList();

                if (lstSumPrcso != null && lstSumPrcso.Count > 0)
                {
                    lstSumPrcso.ForEach(sum =>
                    {
                        sum.SUP_Estado = estadoSuministro;
                    });
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Actuliza el estado del Suministro de acuerdo a la
        /// edicion del mismo en la Administracion del suministro
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="estadoSuministro"></param>
        public void ActualizarEstadoSuministroSucursal(int idSuministro, string estadoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<SuministrosSucursal_SUM> lstSumSucursal = contexto.SuministrosSucursal_SUM.Where(sumSuc => sumSuc.SUS_IdSuministro == idSuministro).ToList();

                if (lstSumSucursal != null && lstSumSucursal.Count > 0)
                {
                    lstSumSucursal.ForEach(sum =>
                    {
                        sum.SUS_Estado = estadoSuministro;
                    });
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Administracion de suministro

        #region Resoluciones suministros

        /// <summary>
        /// Obtiene las resoluciones de los suministros
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<SUNumeradorPrefijo> ObtenerResolucionesSuministro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<NumeradorAutoSuministros_VSUM>("SUM_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                List<SUNumeradorPrefijo> listNum = new List<SUNumeradorPrefijo>();

                listNum = contexto.ConsultarNumeradorAutoSuministros_VSUM(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll(r => new SUNumeradorPrefijo()
                {
                    Descripcion = r.NUM_Descripcion,
                    IdNumerador = r.NUM_IdNumerador,
                    EstaActivo = r.NUM_EstaActivo,
                    FechaFinal = r.NUM_FechaFinal,
                    FechaInicial = r.NUM_FechaInicial,
                    RangoInicial = r.NUM_Inicio,
                    RangoFinal = r.NUM_Fin,
                    RangoActual = r.NUM_Actual,
                    Prefijo = r.NUM_Prefijo,
                    Resolucion = r.NUM_Resolucion,
                    ValorActual = r.NUM_Actual,
                    Suministro = new SUSuministro()
                    {
                        Id = r.SUM_IdSuministro,
                        Descripcion = r.SUM_Descripcion,
                        Prefijo = r.SUM_Prefijo
                    }
                });

                return listNum;
            }
        }

        /// <summary>
        /// Guarda la resolucion del suministro
        /// </summary>
        /// <param name="resolucion"></param>
        public void GuardarResolucionSuministro(SUNumeradorPrefijo resolucion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NumeradorAutomatico_SUM num = new NumeradorAutomatico_SUM()
                {
                    NUM_IdNumerador = resolucion.IdNumerador,
                    NUM_Descripcion = resolucion.Suministro.Descripcion,
                    NUM_Actual = resolucion.RangoActual == 0 ? resolucion.RangoInicial : resolucion.RangoActual,
                    NUM_FechaInicial = resolucion.FechaInicial,
                    NUM_FechaFinal = resolucion.FechaFinal,
                    NUM_Prefijo = resolucion.Suministro.Prefijo,
                    NUM_Inicio = resolucion.RangoInicial,
                    NUM_Fin = resolucion.RangoFinal,
                    NUM_Resolucion = resolucion.Resolucion,
                    NUM_IdSuministro = resolucion.Suministro.Id,
                    NUM_EstaActivo = resolucion.FechaFinal > DateTime.Now ? true : false,
                    NUM_CreadoPor = ControllerContext.Current.Usuario,
                    NUM_FechaGrabacion = DateTime.Now
                };
                contexto.NumeradorAutomatico_SUM.Add(num);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita la resolucion del suministro
        /// </summary>
        /// <param name="resolucion"></param>
        public void EditarResolucionSuministro(SUNumeradorPrefijo resolucion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NumeradorAutomatico_SUM numEn = contexto.NumeradorAutomatico_SUM.Where(r => r.NUM_IdNumerador == resolucion.IdNumerador).SingleOrDefault();
                if (numEn == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE))));
                else
                {
                    numEn.NUM_IdSuministro = resolucion.Suministro.Id;
                    numEn.NUM_Inicio = resolucion.RangoInicial;
                    numEn.NUM_Fin = resolucion.RangoFinal;
                    numEn.NUM_Actual = resolucion.RangoActual;
                    numEn.NUM_FechaFinal = resolucion.FechaFinal;
                    numEn.NUM_FechaInicial = resolucion.FechaInicial;
                    numEn.NUM_Descripcion = resolucion.Suministro.Descripcion;
                    numEn.NUM_Resolucion = resolucion.Resolucion;
                    numEn.NUM_EstaActivo = resolucion.FechaFinal > DateTime.Now ? true : false;
                    numEn.NUM_Prefijo = resolucion.Prefijo;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene los suministros q aplican resolucion
        /// </summary>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosResolucion()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Suministro_SUM.Where(r => r.SUM_AplicaResolucion == true && r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SUM_IdSuministro,
                      Descripcion = r.SUM_Descripcion,
                      Prefijo = r.SUM_Prefijo,
                      FechaFinalResolucion = DateTime.Now,
                      FechaInicialResolucion = DateTime.Now
                  });
            }
        }

        /// <summary>
        /// Obtiene los numeradores de suministros vigentes
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <returns></returns>
        public List<SUNumeradorPrefijo> ObtenerNumeradorSuministroVigentes(int idSuministro, string descripSuministro)
        {
            //using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using(SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNumeradoresSuministrosVigentes_SUM", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idSuministro", idSuministro);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var listaSuministros = dt.AsEnumerable().ToList();
                /*var listaSuministros =
                  contexto
                  .NumeradorAutomatico_SUM
                  .AsEnumerable()
                  .Where(r => r.NUM_IdSuministro == idSuministro).ToList();*/

                if (listaSuministros != null && listaSuministros.Any())
                {
                    return listaSuministros
                       .ConvertAll(r => new SUNumeradorPrefijo()
                         {
                             RangoFinal = r.Field<long>("NUM_Fin"),
                             RangoInicial = r.Field<long>("NUM_Inicio"),
                             FechaFinal = r.Field<DateTime>("NUM_FechaFinal"),
                             FechaInicial = r.Field<DateTime>("NUM_FechaInicial"),
                             EstaActivo = r.Field<bool>("NUM_EstaActivo"),
                             Suministro = new SUSuministro { Id = r.Field<int>("NUM_IdSuministro") },
                             IdNumerador = r.Field<string>("NUM_IdNumerador")
                         });
                }
                else
                    return new List<SUNumeradorPrefijo>();
            }
        }

        #endregion Resoluciones suministros

        #region Grupo Suministros

        /// <summary>
        /// Obtiene los grupos de suministros
        /// </summary>
        /// <returns></returns>
        public List<SUGrupoSuministrosDC> ObtenerGruposSuministros()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.GrupoSuministro_SUM.ToList()
                  .ConvertAll(r => new SUGrupoSuministrosDC()
                  {
                      IdGrupoSuministro = r.GRS_IdGrupoSuministro,
                      Descripcion = r.GRS_Descripcion,
                  });
            }
        }

        /// <summary>
        /// Obtener grupos suministros paginados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<SUGrupoSuministrosDC> ObtenerGrupoSuministrosConSuminGrupo(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                filtro.Add("SUM_Estado", ConstantesFramework.ESTADO_ACTIVO);

                List<SUGrupoSuministrosDC> listaRetorna = new List<SUGrupoSuministrosDC>();

                List<SuministroGrupoSuministro_VSUM> lista = contexto.ConsultarContainsSuministroGrupoSuministro_VSUM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList();
                if (lista != null && lista.Any())
                {
                    listaRetorna.Add
                    (new SUGrupoSuministrosDC
                    {
                        IdGrupoSuministro = lista.FirstOrDefault().GRS_IdGrupoSuministro,
                        Descripcion = lista.FirstOrDefault().GRS_Descripcion,
                        SuministrosGrupo = lista.ConvertAll(p => new SUSuministro()
                                                   {
                                                       Id = p.GSS_IdSuministro,
                                                       CantidadInicialAutorizada = p.GSS_CantidadInicialAutorizada,
                                                       StockMinimo = p.GSS_StockMinimo,
                                                       Descripcion = string.IsNullOrEmpty(p.SUM_Descripcion) ? string.Empty : p.SUM_Descripcion,
                                                       CodigoERP = string.IsNullOrEmpty(p.SUM_CodigoERP) ? string.Empty : p.SUM_CodigoERP,
                                                       UnidadMedida = new PAUnidadMedidaDC()
                                                       {
                                                           IdUnidadMedida = string.IsNullOrEmpty(p.UNM_IdUnidadMedida) ? string.Empty : p.UNM_IdUnidadMedida,
                                                           Descripcion = string.IsNullOrEmpty(p.UNM_Descripcion) ? string.Empty : p.UNM_Descripcion
                                                       }
                                                   }),
                    });
                }
                return listaRetorna;
            }
        }

        /// <summary>
        /// Obtener grupos suministros paginados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<SUGrupoSuministrosDC> ObtenerGrupoSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsGrupoSuministro_SUM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll(r => new SUGrupoSuministrosDC()
                  {
                      IdGrupoSuministro = r.GRS_IdGrupoSuministro,
                      Descripcion = r.GRS_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene la configuracion del grupo de suministros
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public SUGrupoSuministrosDC ObtieneInformacionGrupoSuministro(SUEnumGrupoSuministroDC grupo)
        {
            //using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using(SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                string idGrupo = grupo.ToString();
                SqlCommand cmd = new SqlCommand("paObtenerGrupoSuministro_SUM", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idGrupo", idGrupo);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var gr = dt.AsEnumerable().SingleOrDefault();
               // GrupoSuministro_SUM gr = contexto.GrupoSuministro_SUM.Where(r => r.GRS_IdGrupoSuministro == idGrupo).SingleOrDefault();
                if (gr != null)
                    return new SUGrupoSuministrosDC()
                    {
                        IdGrupoSuministro = gr.Field<string>("GRS_IdGrupoSuministro"),
                        Descripcion = gr.Field<string>("GRS_Descripcion")
                    };
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_GRUPO_NO_CONFIGURADO.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_GRUPO_NO_CONFIGURADO), grupo)));
            }
        }

        /// <summary>
        /// Guarda los grupos de suministro
        /// </summary>
        /// <param name="grupoSuministro"></param>
        public void GuardarGrupoSuministro(SUGrupoSuministrosDC grupoSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GrupoSuministro_SUM grupo = new GrupoSuministro_SUM()
                {
                    GRS_IdGrupoSuministro = grupoSuministro.IdGrupoSuministro,
                    GRS_Descripcion = grupoSuministro.Descripcion,
                    GRS_FechaGrabacion = DateTime.Now,
                    GRS_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.GrupoSuministro_SUM.Add(grupo);

                grupoSuministro.SuministrosGrupo.ForEach(
                sum =>
                {
                    GrupoSuministroSuministro_SUM gruSum = new GrupoSuministroSuministro_SUM()
                    {
                        GSS_CantidadInicialAutorizada = sum.CantidadInicialAutorizada,
                        GSS_StockMinimo = sum.StockMinimo,
                        GSS_IdGrupoSuministro = grupoSuministro.IdGrupoSuministro,
                        GSS_IdSuministro = sum.Id,
                        GSS_FechaGrabacion = DateTime.Now,
                        GSS_CreadoPor = ControllerContext.Current.Usuario
                    };
                    contexto.GrupoSuministroSuministro_SUM.Add(gruSum);
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona suministro a un grupo
        /// </summary>
        /// <param name="grupo"></param>
        public void AdicionarSuministroGrupo(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GrupoSuministroSuministro_SUM gruSum = new GrupoSuministroSuministro_SUM()
                {
                    GSS_CantidadInicialAutorizada = grupo.SuministroGrupo.CantidadInicialAutorizada,
                    GSS_StockMinimo = grupo.SuministroGrupo.StockMinimo,
                    GSS_IdGrupoSuministro = grupo.IdGrupoSuministro,
                    GSS_IdSuministro = grupo.SuministroGrupo.Id,
                    GSS_FechaGrabacion = DateTime.Now,
                    GSS_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.GrupoSuministroSuministro_SUM.Add(gruSum);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina suministro de un grupo
        /// </summary>
        /// <param name="grupo"></param>
        public void EliminarSuministroGrupo(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GrupoSuministroSuministro_SUM grupoEn = contexto.GrupoSuministroSuministro_SUM.Where(r => r.GSS_IdGrupoSuministro == grupo.IdGrupoSuministro && r.GSS_IdSuministro == grupo.SuministroGrupo.Id).FirstOrDefault();

                if (grupoEn != null)
                {
                    contexto.GrupoSuministroSuministro_SUM.Remove(grupoEn);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene los suministros de un grupo
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosGrupo(string idGrupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.GrupoSuministroSuministro_SUM.Include("Suministro_SUM.UnidadMedida_PAR").Where(r => r.GSS_IdGrupoSuministro == idGrupo &&
                                                                                                                    r.Suministro_SUM.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO)
                                                                                                        .OrderBy(or => or.Suministro_SUM.SUM_Descripcion).ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.GSS_IdSuministro,
                      Descripcion = r.Suministro_SUM.SUM_Descripcion,
                      CantidadInicialAutorizada = r.GSS_CantidadInicialAutorizada,
                      StockMinimo = r.GSS_StockMinimo
                  });
            }
        }

        /// <summary>
        /// Método para adicionar un suministro a todos los centros de servicio
        /// </summary>
        /// <param name="grupo"></param>
        public void AdicionarSuministroCentroServicio(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearSuministroCen_SUM(grupo.SuministroGrupo.Id, Convert.ToInt32(grupo.SuministroGrupo.CantidadInicialAutorizada), Convert.ToInt32(grupo.SuministroGrupo.StockMinimo), ControllerContext.Current.Usuario);
            }
        }


        /// <summary>
        /// Método para adicionar un suministro a todos las sucursales de un cliente
        /// </summary>
        /// <param name="grupo"></param>
        public void AdicionarSuministroSucursal(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearSuministroSuc_SUM(grupo.SuministroGrupo.Id, Convert.ToInt32(grupo.SuministroGrupo.CantidadInicialAutorizada), Convert.ToInt32(grupo.SuministroGrupo.StockMinimo), ControllerContext.Current.Usuario);
            }
        }


        /// <summary>
        /// Método para eliminar un suministro en todos los centros de servicio
        /// </summary>
        /// <param name="grupo"></param>
        public void EliminarSuministroCentroServicio(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarSuministroCen_SUM(grupo.SuministroGrupo.Id);
            }
        }




        /// <summary>
        /// Método para eliminar un suministro en todas las sucursales de un cliente
        /// </summary>
        /// <param name="grupo"></param>
        public void EliminarSuministroSucursal(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarSuministroSuc_SUM(grupo.SuministroGrupo.Id);
            }
        }


        /// <summary>
        /// Obtener suministros de grupo
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosGrupo(string idGrupo, long idMensajero)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerSumAsignadoYNoAsigMen_SUM(idMensajero, idGrupo)
                  .ToList()
                  .ConvertAll(r =>
                    {
                        SUSuministro sum = new SUSuministro()
                          {
                              Id = r.SME_IdSuministro,
                              CantidadInicialAutorizada = r.SME_CantidadInicialAutorizada,
                              StockMinimo = r.SME_StockMinimo,
                              CodigoERP = r.SUM_CodigoERP,
                              Descripcion = r.SUM_Descripcion,
                              CodigoAlterno = r.SUM_CodigoAlterno,
                              SuministroAutorizado = r.SME_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                              UnidadMedida = new PAUnidadMedidaDC()
                              {
                                  Descripcion = r.UNM_Descripcion,
                                  IdUnidadMedida = r.SUM_IdUnidadMedida
                              },
                              EstaActivo = r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                          };
                        return sum;
                    });
            }
        }

        /// <summary>
        /// Obtiene los suministros que no estan incluidos en ningun grupo, ni en en el grupo seleccionado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosNoIncluidosEnGrupo(IDictionary<string, string> filtro, string idGrupo, long idMensajero)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string codigoERP;
                string descripcion;

                filtro.TryGetValue("SUM_CodigoERP", out codigoERP);
                filtro.TryGetValue("SUM_Descripcion", out descripcion);

                return contexto.paObtenerSumMensajeroNoIncluidosEnGrupo_SUM(idMensajero, idGrupo, codigoERP, descripcion).ToList()
                   .ConvertAll(r => new SUSuministro()
                   {
                       Id = r.SUM_IdSuministro,
                       Descripcion = r.SUM_Descripcion,
                       CodigoERP = r.SUM_CodigoERP,
                       CodigoAlterno = r.SUM_CodigoAlterno,
                       UnidadMedida = new PAUnidadMedidaDC()
                       {
                           IdUnidadMedida = r.SUM_IdUnidadMedida,
                           Descripcion = r.UNM_Descripcion
                       },
                       EstaActivo = r.SUM_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false
                   });
            }
        }

        #endregion Grupo Suministros

        #region SuministrosMensajeros

        /// <summary>
        /// Guarda los suministros de un mensajero
        /// </summary>
        /// <param name="grupo"></param>
        public void GuardarSuministrosMensajero(SUGrupoSuministrosDC grupo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosMensajero_SUM suministroBD = contexto.SuministrosMensajero_SUM.FirstOrDefault(sum => sum.SME_IdSuministro == grupo.SuministroGrupo.Id && sum.SME_IdMensajero == grupo.Mensajero.IdMensajero);
                if (suministroBD == null)
                {
                    if (grupo.SuministroGrupo.SuministroAutorizado)
                    {
                        contexto.SuministrosMensajero_SUM.Add(new SuministrosMensajero_SUM()
                      {
                          SME_CantidadInicialAutorizada = grupo.SuministroGrupo.CantidadInicialAutorizada,
                          SME_IdSuministro = grupo.SuministroGrupo.Id,
                          SME_StockMinimo = grupo.SuministroGrupo.StockMinimo,
                          SME_IdMensajero = grupo.Mensajero.IdMensajero,
                          SME_CreadoPor = ControllerContext.Current.Usuario,
                          SME_FechaGrabacion = DateTime.Now,
                          SME_Estado = ConstantesFramework.ESTADO_ACTIVO,
                      });
                    }
                }
                else
                {
                    suministroBD.SME_Estado = grupo.SuministroGrupo.SuministroAutorizado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
                    suministroBD.SME_CantidadInicialAutorizada = grupo.SuministroGrupo.SuministroAutorizado ? grupo.SuministroGrupo.CantidadInicialAutorizada : 0;
                    suministroBD.SME_StockMinimo = grupo.SuministroGrupo.SuministroAutorizado ? grupo.SuministroGrupo.StockMinimo : 0;
                }

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene el id de la asignacion del suministro para el mensajero
        /// </summary>
        /// <returns></returns>
        public int ObtenerInfoAsignacionSuministroMensajero(int idSuministro, long idMensajero)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosMensajero_SUM sum = contexto.SuministrosMensajero_SUM.Where(r => r.SME_IdMensajero == idMensajero && r.SME_IdSuministro == idSuministro).FirstOrDefault();
                if (sum == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO)));
                else
                    return sum.SME_IdSuministroMensajero;
            }
        }

        /// <summary>
        /// Obtiene el id de la asignacion del suministro para el mensajero
        /// </summary>
        /// <returns></returns>
        public int ObtenerInfoAsignacionSuministroProceso(int idSuministro, long idProceso)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosProceso_SUM sum = contexto.SuministrosProceso_SUM.Where(r => r.SUP_CodigoProceso == idProceso && r.SUP_IdSuministro == idSuministro).FirstOrDefault();
                if (sum == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO)));
                else
                    return sum.SUP_IdSuministroProceso;
            }
        }

        /// <summary>
        /// Obtiene el id de la asignacion del suministro para el mensajero
        /// </summary>
        /// <returns></returns>
        public int ObtenerIdAsignacionSuministroSucursal(int idSuministro, long idSucursal)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosSucursal_SUM sum = contexto.SuministrosSucursal_SUM.Where(r => r.SUS_IdSucursal == idSucursal && r.SUS_IdSuministro == idSuministro).FirstOrDefault();
                if (sum == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO)));
                else
                    return sum.SUS_IdSuministroSucursal;
            }
        }

        /// <summary>
        /// Obtiene el id de la asignacion del suministro para el mensajero
        /// </summary>
        /// <returns></returns>
        public int ObtenerIdAsignacionSuministroCanalVenta(int idSuministro, long idCentroServicio)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosCentroServicios_SUM sum = contexto.SuministrosCentroServicios_SUM.Where(r => r.SCS_IdCentroServicios == idCentroServicio && r.SCS_IdSuministro == idSuministro).FirstOrDefault();
                if (sum == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_NO_APROBADO)));
                else
                    return sum.SCS_IdSuministroCentroServicio;
            }
        }

        #endregion SuministrosMensajeros

        #region Aprovisionamiento

        /// <summary>
        /// Obtiene las ultimas remisiones realizadas
        /// </summary>
        public List<SURemisionSuministroDC> ObtenerUltimasRemisiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (string.IsNullOrEmpty(campoOrdenamiento))
                {
                    campoOrdenamiento = "RES_IdRemisionSuministros";
                    ordenamientoAscendente = false;
                }

                List<SURemisionSuministroDC> lstRemisionSuministro = new List<SURemisionSuministroDC>();

                lstRemisionSuministro = contexto.ConsultarContainsRemisionSuministro_VSUM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                .Take(10)
                .OrderByDescending(r => r.RES_IdRemisionSuministros)
                .ToList()
                  .ConvertAll(r =>
                {
                    SURemisionSuministroDC suministro = new SURemisionSuministroDC()
                    {
                        IdRemision = r.RES_IdRemisionSuministros,
                        IdCasaMatriz = r.RES_IdCasaMatrizElabora,
                        FechaRemision = r.RES_FechaGrabacion,
                        NumeroGuiaDespacho = r.RES_NumeroGuiaInternaDespacho,
                        Destinatario = r.RES_NombreDestinatario,
                        CiudadDestino = new PALocalidadDC()
                        {
                            IdLocalidad = r.RES_IdLocalidadDestino,
                            Nombre = r.LOC_Nombre
                        },
                        GrupoSuministros = new SUGrupoSuministrosDC()
                        {
                            IdGrupoSuministro = r.RES_IdGrupoSuministroDestinatario,
                            Descripcion = r.GRS_Descripcion
                        }
                    };

                    return suministro;
                });

                return lstRemisionSuministro;
            }
        }

        /// <summary>
        /// Obtiene los suministros aprabados para realizar la remision al mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosMensajero(long idMensajero)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SuministrosMensajero_SUM.Include("Suministro_SUM.UnidadMedida_PAR")
                  .Where(r => r.SME_IdMensajero == idMensajero && r.SME_Estado == ConstantesFramework.ESTADO_ACTIVO).ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SME_IdSuministro,
                      IdAsignacionSuministro = r.SME_IdSuministroMensajero,
                      Descripcion = r.Suministro_SUM.SUM_Descripcion,
                      UnidadMedida = new PAUnidadMedidaDC()
                      {
                          IdUnidadMedida = r.Suministro_SUM.UnidadMedida_PAR.UNM_IdUnidadMedida,
                          Descripcion = r.Suministro_SUM.UnidadMedida_PAR.UNM_Descripcion,
                      },
                      CantidadInicialAutorizada = r.SME_CantidadInicialAutorizada,
                      AplicaResolucion = r.Suministro_SUM.SUM_AplicaResolucion,
                      CodigoAlterno = r.Suministro_SUM.SUM_CodigoAlterno,
                      CodigoERP = r.Suministro_SUM.SUM_CodigoERP,
                      CategoriaSuministro = new SUCategoriaSuministro()
                      {
                          IdCategoria = r.Suministro_SUM.SUM_IdCategoria
                      },
                      CuentaGasto = r.Suministro_SUM.SUM_CuentaGasto,
                      Rango = new SURango()
                  });
            }
        }

        /// <summary>
        /// Obtiene los suministros asignados al canal de venta
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<SUSuministro> ObtenerSuministrosAsignadosCanalVenta(long idCentroServicios)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<SUSuministro> lstsuministros = new List<SUSuministro>();

                lstsuministros = contexto.SuministrosCentroServicios_SUM.Include("Suministro_SUM.UnidadMedida_PAR").Where(r => r.SCS_IdCentroServicios == idCentroServicios && r.SCS_Estado == ConstantesFramework.ESTADO_ACTIVO)
                .ToList()
                .ConvertAll(r => new SUSuministro()
                {
                    Id = r.SCS_IdSuministro,
                    IdAsignacionSuministro = r.SCS_IdSuministroCentroServicio,
                    Descripcion = r.Suministro_SUM.SUM_Descripcion,
                    UnidadMedida = new PAUnidadMedidaDC()
                    {
                        IdUnidadMedida = r.Suministro_SUM.UnidadMedida_PAR.UNM_IdUnidadMedida,
                        Descripcion = r.Suministro_SUM.UnidadMedida_PAR.UNM_Descripcion,
                    },
                    CantidadInicialAutorizada = r.SCS_CantidadInicialAutorizada,
                    AplicaResolucion = r.Suministro_SUM.SUM_AplicaResolucion,
                    CodigoAlterno = r.Suministro_SUM.SUM_CodigoAlterno,
                    CodigoERP = r.Suministro_SUM.SUM_CodigoERP,
                    CategoriaSuministro = new SUCategoriaSuministro()
                    {
                        IdCategoria = r.Suministro_SUM.SUM_IdCategoria
                    },
                    CuentaGasto = r.Suministro_SUM.SUM_CuentaGasto,
                    Rango = new SURango()
                });

                return lstsuministros;
            }
        }

        /// <summary>
        /// Valida que un rango ingresado no este asignado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="rangoInicial"></param>
        /// <param name="rangoFinal"></param>
        /// <returns></returns>
        public void ValidaRangoSuministrosAsignados(int idSuministro, string descripcionSuministro, long rangoInicial, long rangoFinal)
        {
             using(SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConsultaSuministros_SUM", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RangoDesde",rangoInicial);
                cmd.Parameters.AddWithValue("@RangoHasta",rangoFinal);
                cmd.Parameters.AddWithValue("@IdSuministro", idSuministro);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var suministros = dt.AsEnumerable().FirstOrDefault();
        
                if (suministros != null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_SUMINISTRO_ASIGNADO.ToString(), string.Format(MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_SUMINISTRO_ASIGNADO), descripcionSuministro, suministros.Field<string>("NombrePropietario").ToString())));
            }
        }

        /// <summary>
        /// guarda la provision del suministro del mensajero
        /// </summary>
        /// <param name="remision"></param>
        public long GuardarProvisionSuministrosMensajero(SUSuministro suministro, long idRemision, int idAsignacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumMensajero_SUM sumMensajero = new ProvisionSumMensajero_SUM()
                {
                    PSM_IdRemisionSuministros = idRemision,
                    PSM_IdSuministroMensajero = idAsignacion,
                    PSM_CantidadAsginada = suministro.CantidadAsignada,
                    PSM_FechaGrabacion = DateTime.Now,
                    PSM_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.ProvisionSumMensajero_SUM.Add(sumMensajero);
                contexto.SaveChanges();
                return sumMensajero.PSM_IdProvisionSumMensajero;
            }
        }

        /// <summary>
        /// Guarda la remision del suministro para el canal de venta
        /// </summary>
        public long GuardaRemisionSuministroCanalVenta(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RemisionSuministros_SUM remisionEn = new RemisionSuministros_SUM()
                {
                    RES_NombreDestinatario = remision.CentroServicioAsignacion.Nombre,
                    RES_IdLocalidadDestino = remision.CentroServicioAsignacion.CiudadUbicacion.IdLocalidad,
                    RES_IdAdminisionMensajeriaDespacho = remision.IdGuiaInternaRemision,
                    RES_NumeroGuiaInternaDespacho = remision.NumeroGuiaDespacho,
                    RES_IdGrupoSuministroDestinatario = remision.GrupoSuministroDestino.ToString(),
                    RES_FechaGrabacion = DateTime.Now,
                    RES_CreadoPor = ControllerContext.Current.Usuario,
                    RES_IdCasaMatrizElabora = (short)remision.IdCasaMatriz,
                    RES_Estado = remision.Estado.ToString()
                };

                contexto.RemisionSuministros_SUM.Add(remisionEn);
                contexto.SaveChanges();
                return remisionEn.RES_IdRemisionSuministros;
            }
        }

        /// <summary>
        /// Guarda la remision del suministro para el canal de venta
        /// </summary>
        public long GuardaRemisionSuministroSucursal(SURemisionSuministroDC remision)
        {
            //using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using (SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paInsertarRemisionSuministrosSucursal_SUM", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCasaMatrizElabora", (short)remision.IdCasaMatriz);
                cmd.Parameters.AddWithValue("@IdAdminisionMensajeriaDespacho", remision.IdGuiaInternaRemision);
                cmd.Parameters.AddWithValue("@NumeroGuiaInternaDespacho", remision.NumeroGuiaDespacho);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", remision.CiudadDestino.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdGrupoSuministroDestinatario", SUEnumGrupoSuministroDC.CLI.ToString());
                cmd.Parameters.AddWithValue("@NombreDestinatario", remision.ClienteRemision.RazonSocial);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Estado", remision.Estado.ToString());
                long? idRemision = null;
                sqlConn.Open();
                var idRem = cmd.ExecuteScalar();
                sqlConn.Close();
                if (idRem != null)
                {
                    idRemision = Convert.ToInt64(idRem);
                }
                return idRemision.Value;
                /*RemisionSuministros_SUM remisionEn = new RemisionSuministros_SUM()
                {
                    RES_NombreDestinatario = remision.ClienteRemision.RazonSocial,
                    RES_IdLocalidadDestino = remision.CiudadDestino.IdLocalidad,
                    RES_IdAdminisionMensajeriaDespacho = remision.IdGuiaInternaRemision,
                    RES_NumeroGuiaInternaDespacho = remision.NumeroGuiaDespacho,
                    RES_IdGrupoSuministroDestinatario = SUEnumGrupoSuministroDC.CLI.ToString(),
                    RES_FechaGrabacion = DateTime.Now,
                    RES_CreadoPor = ControllerContext.Current.Usuario,
                    RES_IdCasaMatrizElabora = (short)remision.IdCasaMatriz,
                    RES_Estado = remision.Estado.ToString()
                };

                contexto.RemisionSuministros_SUM.Add(remisionEn);
                contexto.SaveChanges();
                return remisionEn.RES_IdRemisionSuministros;*/
            }
        }

        /// <summary>
        /// Guarda la remision del suministro
        /// </summary>
        public long GuardaRemisionSuministro(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RemisionSuministros_SUM remisionEn = new RemisionSuministros_SUM()
                {
                    RES_NombreDestinatario = remision.MensajeroAsignacion.Nombre,
                    RES_IdLocalidadDestino = remision.MensajeroAsignacion.LocalidadMensajero.IdLocalidad,
                    RES_IdAdminisionMensajeriaDespacho = remision.IdGuiaInternaRemision,
                    RES_NumeroGuiaInternaDespacho = remision.NumeroGuiaDespacho,
                    RES_IdGrupoSuministroDestinatario = SUConstantesSuministros.ID_GRUPO_SUMINISTRO_MENSAJERO,
                    RES_FechaGrabacion = DateTime.Now,
                    RES_CreadoPor = ControllerContext.Current.Usuario,
                    RES_IdCasaMatrizElabora = (short)remision.IdCasaMatriz,
                    RES_Estado = remision.Estado.ToString()
                };

                contexto.RemisionSuministros_SUM.Add(remisionEn);
                contexto.SaveChanges();
                return remisionEn.RES_IdRemisionSuministros;
            }
        }

        /// <summary>
        /// Guarda la provision de suministros del mensajero
        /// </summary>
        /// <param name="suministro"></param>
        /// <param name="idProvision"></param>
        public void GuardarProvisionSuminsitroSerialMensajero(SUSuministro suministro, long idProvision, long rangoInicial, long rangoFinal, bool esModificacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumMensajeroSerial_SUM provisionSerial = new ProvisionSumMensajeroSerial_SUM()
                {
                    MSS_Inicio = rangoInicial,
                    MSS_Fin = rangoFinal,
                    MSS_IdProvisionSumMensajero = idProvision,
                    MSS_FechaGrabacion = DateTime.Now,
                    MSS_CreadoPor = ControllerContext.Current.Usuario,
                    MSS_FechaFinal = suministro.FechaFinalResolucion,
                    MSS_FechaInicial = suministro.FechaInicialResolucion,
                    MSS_Prefijo = string.Empty,
                    MSS_IdNumerador = suministro.IdResolucion,
                    MSS_PorModificacion = esModificacion
                };

                contexto.ProvisionSumMensajeroSerial_SUM.Add(provisionSerial);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Guarda la remision del proceso
        /// </summary>
        public long GuardaRemisionProceso(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RemisionSuministros_SUM remisionEn = new RemisionSuministros_SUM()
                {
                    RES_NombreDestinatario = remision.MensajeroAsignacion.Nombre,
                    RES_IdLocalidadDestino = remision.MensajeroAsignacion.LocalidadMensajero.IdLocalidad,
                    RES_IdAdminisionMensajeriaDespacho = remision.IdGuiaInternaRemision,
                    RES_NumeroGuiaInternaDespacho = remision.NumeroGuiaDespacho,
                    RES_IdGrupoSuministroDestinatario = SUConstantesSuministros.ID_GRUPO_SUMINISTRO_MENSAJERO,
                    RES_FechaGrabacion = DateTime.Now,
                    RES_CreadoPor = ControllerContext.Current.Usuario,
                    RES_IdCasaMatrizElabora = (short)remision.IdCasaMatriz,
                    RES_Estado = remision.Estado.ToString()
                };
                contexto.RemisionSuministros_SUM.Add(remisionEn);
                contexto.SaveChanges();
                return remisionEn.RES_IdRemisionSuministros;
            }
        }

        /// <summary>
        /// Elimina el rango de la remision de los suministros seleccionados
        /// </summary>
        /// <param name="remision"></param>
        public void EliminarRangoProvisionSuministroMenSerial(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumMensajeroSerial_SUM provision = contexto.ProvisionSumMensajeroSerial_SUM.Where(r => r.MSS_IdProvisionSumMensajeroSerial == remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministroSerial).FirstOrDefault();

                if (provision != null)
                {
                    contexto.ProvisionSumMensajeroSerial_SUM.Remove(provision);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina el rango de la remision de los suministros seleccionados
        /// </summary>
        /// <param name="remision"></param>
        public void EliminarRangoProvisionSuministroMen(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumMensajero_SUM provision = contexto.ProvisionSumMensajero_SUM.Where(r => r.PSM_IdProvisionSumMensajero == remision.GrupoSuministros.SuministroGrupo.IdProvisionSuministro).FirstOrDefault();

                if (provision != null)
                {
                    contexto.ProvisionSumMensajero_SUM.Remove(provision);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Aprovisionamiento

        #region Canal de Venta

        public long GuardarProvisionSuministrosCanalVenta(SUSuministro suministro, long idRemision, int idAsignacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumCentroSvc_SUM canalVenta = new ProvisionSumCentroSvc_SUM()
                {
                    PSC_IdRemisionSuministros = idRemision,
                    PSC_CantidadAsginada = suministro.CantidadAsignada,
                    PSC_IdSuministroCentroServicio = idAsignacion,
                    PSC_FechaGrabacion = DateTime.Now,
                    PSC_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ProvisionSumCentroSvc_SUM.Add(canalVenta);
                contexto.SaveChanges();

                return canalVenta.PSC_IdProvisionSumCentroSvc;
            }
        }

        public void GuardarProvisionSuminsitroSerialCanalVenta(SUSuministro suministro, long idProvision, long rangoInicial, long rangoFinal, bool esModificacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumCentroSvcSerial_SUM serialCanalVenta = new ProvisionSumCentroSvcSerial_SUM()
                {
                    PCS_Inicio = rangoInicial,
                    PCS_Fin = rangoFinal,
                    PCS_FechaInicial = suministro.FechaInicialResolucion,
                    PCS_FechaFinal = suministro.FechaFinalResolucion,
                    PCS_IdProvisionSumCentroSvc = idProvision,
                    PCS_Prefijo = string.Empty,
                    PCS_FechaGrabacion = DateTime.Now,
                    PCS_CreadoPor = ControllerContext.Current.Usuario,
                    PCS_IdNumerador = suministro.IdResolucion,
                    PCS_PorModificacion = esModificacion
                };

                contexto.ProvisionSumCentroSvcSerial_SUM.Add(serialCanalVenta);
                contexto.SaveChanges();
            }
        }

        #endregion Canal de Venta

        #region Cliente

        /// <summary>
        /// Obtiene los suministros asignados a la sucursal
        /// </summary>
        /// <param name="idSucursal"></param>
        public List<SUSuministro> ObtenerSuministrosAsignadoSucursal(int idSucursal)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.SuministrosSucursal_SUM.Include("Suministro_SUM.UnidadMedida_PAR").Where(r => r.SUS_IdSucursal == idSucursal &&
                                                                                                                  r.SUS_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new SUSuministro()
                  {
                      Id = r.SUS_IdSuministro,
                      IdAsignacionSuministro = r.SUS_IdSuministroSucursal,
                      Descripcion = r.Suministro_SUM.SUM_Descripcion,
                      UnidadMedida = new PAUnidadMedidaDC()
                      {
                          IdUnidadMedida = r.Suministro_SUM.UnidadMedida_PAR.UNM_IdUnidadMedida,
                          Descripcion = r.Suministro_SUM.UnidadMedida_PAR.UNM_Descripcion,
                      },
                      CantidadInicialAutorizada = r.SUS_CantidadInicialAutorizada,
                      AplicaResolucion = r.Suministro_SUM.SUM_AplicaResolucion,
                      CodigoAlterno = r.Suministro_SUM.SUM_CodigoAlterno,
                      CodigoERP = r.Suministro_SUM.SUM_CodigoERP,
                      CategoriaSuministro = new SUCategoriaSuministro()
                      {
                          IdCategoria = r.Suministro_SUM.SUM_IdCategoria
                      },
                      CuentaGasto = r.Suministro_SUM.SUM_CuentaGasto,
                  });
            }
        }

        public long GuardarProvisionSuministrosSucursal(SUSuministro suministro, long idRemision, int idAsignacion)
        {
            //using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            
            using(SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paInsertarProvisionSuministrosSucursal_SUM", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PSS_IdRemisionSuministros", idRemision);
                cmd.Parameters.AddWithValue("@PSS_IdSuministroSucursal", idAsignacion);
                cmd.Parameters.AddWithValue("@PSS_CantidadAsginada", suministro.CantidadAsignada);
                cmd.Parameters.AddWithValue("@PSS_FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@QPSS_CreadoPor", ControllerContext.Current.Usuario);
                long? idProvision = null;
                sqlConn.Open();
                var idProv = cmd.ExecuteScalar();
                sqlConn.Close();
                if (idProv != null)
                {
                    idProvision = Convert.ToInt64(idProv);
                }
                return idProvision.Value;
               /* ProvisionSumSucursal_SUM sucursal = new ProvisionSumSucursal_SUM()
                {
                    PSS_IdRemisionSuministros = idRemision,
                    PSS_CantidadAsginada = suministro.CantidadAsignada,
                    PSS_IdSuministroSucursal = idAsignacion,
                    PSS_FechaGrabacion = DateTime.Now,
                    PSS_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ProvisionSumSucursal_SUM.Add(sucursal);
                contexto.SaveChanges();

                return sucursal.PSS_IdProvisionSumSucursal;*/

            }
        }

        public void GuardarProvisionSuminsitroSerialSucursal(SUSuministro suministro, long idProvision, long rangoInicial, long rangoFinal, bool esModificacion)
        {
            //using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using(SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paInsertarSuministroSerialSuc_SUM", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProvisionSumSucursal", idProvision);
                cmd.Parameters.AddWithValue("@Prefijo",string.Empty);
                cmd.Parameters.AddWithValue("@Inicio",rangoInicial);
                cmd.Parameters.AddWithValue("@Fin",rangoFinal);
                cmd.Parameters.AddWithValue("@FechaInicial",suministro.FechaInicialResolucion);
                cmd.Parameters.AddWithValue("@FechaFinal",suministro.FechaFinalResolucion);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdNumerador",suministro.IdResolucion);
                cmd.Parameters.AddWithValue("@IdContrato",suministro.IdContrato);
                cmd.Parameters.AddWithValue("@NombreContrato",suministro.NombreContrato);
                cmd.Parameters.AddWithValue("@PorModificacion",esModificacion);

                sqlConn.Open();
                cmd.ExecuteNonQuery();
                sqlConn.Close();
                /*ProvisionSumSucursalSerial_SUM serialSucursal = new ProvisionSumSucursalSerial_SUM()
                {
                    SSS_Inicio = rangoInicial,
                    SSS_Fin = rangoFinal,
                    SSS_FechaInicial = suministro.FechaInicialResolucion,
                    SSS_FechaFinal = suministro.FechaFinalResolucion,
                    SSS_IdProvisionSumSucursal = idProvision,
                    SSS_Prefijo = string.Empty,
                    SSS_FechaGrabacion = DateTime.Now,
                    SSS_CreadoPor = ControllerContext.Current.Usuario,
                    SSS_IdNumerador = suministro.IdResolucion,
                    SSS_IdContrato = suministro.IdContrato,
                    SSS_NombreContrato = suministro.NombreContrato,
                    SSS_PorModificacion = esModificacion
                };

                contexto.ProvisionSumSucursalSerial_SUM.Add(serialSucursal);
                contexto.SaveChanges();*/
            }
        }

        #endregion Cliente

        #region Desasignacion Suministros

        /// <summary>
        /// Retorna los suministros que esten en el rango de fecha seleccionado para un mensajero
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<SURemisionSuministroDC> ObtenerSuministroAsignadoMensajeroXRangoFechas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial;
                DateTime fechaFinal;
                string rangoInicial;
                string rangoFinal;
                string usuario;
                string s_fechaInicial;
                string s_fechaFinal;
                string idMensajero;
                string idSuministro;
                string idRacol;
                string idLocalidadDestino;
                long? _rangoInicial = null;
                long? _rangoFinal = null;
                int? _idSuministro = null;
                long? _idRacol = null;

                filtro.TryGetValue("fechaInicial", out s_fechaInicial);
                filtro.TryGetValue("fechaFinal", out s_fechaFinal);
                filtro.TryGetValue("rangoInicial", out rangoInicial);
                filtro.TryGetValue("rangoFinal", out rangoFinal);
                filtro.TryGetValue("idUsuario", out usuario);
                filtro.TryGetValue("idMensajero", out idMensajero);
                filtro.TryGetValue("idSuministro", out idSuministro);
                filtro.TryGetValue("idRacol", out idRacol);
                filtro.TryGetValue("idLocalidadDestino", out idLocalidadDestino);

                // Con esto garantizo que si no viene fecha o si viene en un formato errado entonces se busque desde la fecha mínima
                // hasta la fecha de hoy.
                if (!string.IsNullOrWhiteSpace(s_fechaFinal) && !string.IsNullOrWhiteSpace(s_fechaInicial))
                {
                    CultureInfo cultura = new CultureInfo("es-CO");
                    if (!DateTime.TryParse(s_fechaInicial, cultura, DateTimeStyles.None, out fechaInicial))
                    {
                        fechaInicial = ConstantesFramework.MinDateTimeController;
                    }
                    if (!DateTime.TryParse(s_fechaFinal, cultura, DateTimeStyles.None, out fechaFinal))
                    {
                        fechaFinal = DateTime.Now.Date.AddDays(1);
                    }
                    fechaInicial.AddDays(1);
                }
                else
                {
                    fechaInicial = ConstantesFramework.MinDateTimeController;
                    fechaFinal = DateTime.Now.Date.AddDays(1);
                }
                if (!string.IsNullOrWhiteSpace(rangoInicial))
                {
                    _rangoInicial = Convert.ToInt64(rangoInicial);
                }
                else
                    _rangoInicial = 0;
                if (!string.IsNullOrWhiteSpace(rangoFinal))
                {
                    _rangoFinal = Convert.ToInt64(rangoFinal);
                }
                else
                    _rangoFinal = 0;
                if (!string.IsNullOrWhiteSpace(idRacol))
                {
                    _idRacol = Convert.ToInt64(idRacol);
                }
                if (!string.IsNullOrWhiteSpace(idSuministro))
                {
                    _idSuministro = Convert.ToInt32(idSuministro);
                }

                return contexto.paObtenerSumProvisionadoMenFecha_SUM(usuario
                  , idMensajero
                  , indicePagina
                  , registrosPorPagina
                  , fechaInicial
                  , fechaFinal
                  , _rangoInicial
                  , _rangoFinal
                  , _idSuministro
                  )
                  .ToList()
                  .ConvertAll(r => new SURemisionSuministroDC()
                  {
                      IdRemision = r.PSM_IdRemisionSuministros,
                      FechaRemision = r.RES_FechaGrabacion,
                      CiudadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.RES_IdLocalidadDestino,
                      },
                      MensajeroAsignacion = new Servicios.ContratoDatos.ParametrosOperacion.POMensajero()
                      {
                          IdMensajero = r.SME_IdMensajero,
                      },
                      GrupoSuministros = new SUGrupoSuministrosDC()
                      {
                          SuministroGrupo = new SUSuministro()
                          {
                              Id = r.SME_IdSuministro,
                              IdAsignacionSuministro = r.SME_IdSuministroMensajero,
                              CantidadAsignada = (int)r.PSM_CantidadAsginada,
                              Descripcion = r.SUM_Descripcion,
                              RangoFinal = r.MSS_Fin,
                              RangoInicial = r.MSS_Inicio,
                              IdProvisionSuministro = r.MSS_IdProvisionSumMensajero,
                              IdProvisionSuministroSerial = r.MSS_IdProvisionSumMensajeroSerial,
                              IdPropietario = r.PEI_Identificacion,
                              NombrePropietario = r.PEI_Nombre.Trim() + " " + r.PEI_PrimerApellido.Trim(),
                              FechaInicialResolucion = r.MSS_FechaInicial,
                              FechaFinalResolucion = r.MSS_FechaFinal,
                              IdResolucion = r.MSS_IdNumerador
                          }
                      }
                  });
            }
        }

        /// <summary>
        /// Retorna los suministros que esten en el rango de fecha seleccionado para una sucursal
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<SURemisionSuministroDC> ObtenerSuministroAsignadoSucursalXRangoFechas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial;
                DateTime fechaFinal;
                string rangoInicial;
                string rangoFinal;
                string s_fechaInicial;
                string s_fechaFinal;
                string idSucursal;
                string idSuministro;
                string usuario;
                long? _rangoInicial = null;
                long? _rangoFinal = null;
                int? _idSuministro = null;
                int? _idSucursal = null;

                filtro.TryGetValue("fechaInicial", out s_fechaInicial);
                filtro.TryGetValue("fechaFinal", out s_fechaFinal);
                filtro.TryGetValue("rangoInicial", out rangoInicial);
                filtro.TryGetValue("rangoFinal", out rangoFinal);
                filtro.TryGetValue("idSucursal", out idSucursal);
                filtro.TryGetValue("idSuministro", out idSuministro);
                filtro.TryGetValue("usuario", out usuario);


                // Con esto garantizo que si no viene fecha o si viene en un formato errado entonces se busque desde la fecha mínima
                // hasta la fecha de hoy.
                if (!string.IsNullOrWhiteSpace(s_fechaFinal) && !string.IsNullOrWhiteSpace(s_fechaInicial))
                {
                    CultureInfo cultura = new CultureInfo("es-CO");
                    if (!DateTime.TryParse(s_fechaInicial, cultura, DateTimeStyles.None, out fechaInicial))
                    {
                        fechaInicial = ConstantesFramework.MinDateTimeController;
                    }
                    if (!DateTime.TryParse(s_fechaFinal, cultura, DateTimeStyles.None, out fechaFinal))
                    {
                        fechaFinal = DateTime.Now.Date.AddDays(1);
                    }
                    fechaInicial.AddDays(1);
                }
                else
                {
                    fechaInicial = ConstantesFramework.MinDateTimeController;
                    fechaFinal = DateTime.Now.Date.AddDays(1);
                }
                if (!string.IsNullOrWhiteSpace(rangoInicial))
                {
                    _rangoInicial = Convert.ToInt64(rangoInicial);
                }
                else
                    _rangoInicial = 0;
                if (!string.IsNullOrWhiteSpace(rangoFinal))
                {
                    _rangoFinal = Convert.ToInt64(rangoFinal);
                }
                else
                    _rangoFinal = 0;
                if (!string.IsNullOrWhiteSpace(idSuministro))
                {
                    _idSuministro = Convert.ToInt32(idSuministro);
                }
                _idSucursal = Convert.ToInt32(idSucursal);

                return contexto.paObtenerSumProvisionadoSucFecha_SUM(usuario
                  , _idSucursal
                  , indicePagina
                  , registrosPorPagina
                  , fechaInicial
                  , fechaFinal
                  , _rangoInicial
                  , _rangoFinal
                  , _idSuministro
                  )
                  .ToList()
                  .ConvertAll(r => new SURemisionSuministroDC()
                  {
                      IdRemision = r.PSS_IdRemisionSuministros,
                      FechaRemision = r.RES_FechaGrabacion,
                      CiudadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.RES_IdLocalidadDestino,
                      },
                      GrupoSuministros = new SUGrupoSuministrosDC()
                      {
                          SuministroGrupo = new SUSuministro()
                          {
                              Id = r.SUS_IdSuministro,
                              CantidadAsignada = (int)r.PSS_CantidadAsginada,
                              Descripcion = r.SUM_Descripcion,

                              //RangoFinalPlano = r.SSS_Fin,
                              IdAsignacionSuministro = r.SUS_IdSuministroSucursal,
                              RangoInicial = r.SSS_Inicio,
                              RangoFinal = r.SSS_Fin,
                              IdProvisionSuministro = r.SSS_IdProvisionSumSucursal,
                              IdProvisionSuministroSerial = r.SSS_IdProvisionSumSucursalSerial,
                              IdPropietario = r.SUC_IdSucursal.ToString(),
                              NombrePropietario = r.SUC_Nombre,
                              FechaFinalResolucion = r.SSS_FechaFinal,
                              FechaInicialResolucion = r.SSS_FechaInicial,
                              IdContrato = r.SSS_IdContrato.Value,
                              NombreContrato = r.SSS_NombreContrato,
                              IdResolucion = r.SSS_IdNumerador
                          }
                      },
                      Sucursal = new Servicios.ContratoDatos.Clientes.CLSucursalDC()
                      {
                          IdSucursal = r.SUC_IdSucursal,
                      },
                      Contrato = new Servicios.ContratoDatos.Clientes.CLContratosDC()
                      {
                          IdContrato = r.SSS_IdContrato.Value,
                          NombreContrato = r.SSS_NombreContrato
                      },
                  });
            }
        }

        /// <summary>
        /// Retorna los suministros que esten en el rango de fecha seleccionado para un centro de servicios
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<SURemisionSuministroDC> ObtenerSuministroAsignadoCentroSvcXRangoFechas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial;
                DateTime fechaFinal;
                string rangoInicial;
                string rangoFinal;
                string s_fechaInicial;
                string s_fechaFinal;
                string idSuministro;
                string idCentroServicio;
                string usuario;
                long? _rangoInicial = null;
                long? _rangoFinal = null;
                int? _idSuministro = null;
                int? _idCentroServicio = null;

                filtro.TryGetValue("fechaInicial", out s_fechaInicial);
                filtro.TryGetValue("fechaFinal", out s_fechaFinal);
                filtro.TryGetValue("rangoInicial", out rangoInicial);
                filtro.TryGetValue("rangoFinal", out rangoFinal);
                filtro.TryGetValue("idCentroServicio", out idCentroServicio);
                filtro.TryGetValue("idSuministro", out idSuministro);
                filtro.TryGetValue("idUsuario", out usuario);

                // Con esto garantizo que si no viene fecha o si viene en un formato errado entonces se busque desde la fecha mínima
                // hasta la fecha de hoy.
                if (!string.IsNullOrWhiteSpace(s_fechaFinal) && !string.IsNullOrWhiteSpace(s_fechaInicial))
                {
                    CultureInfo cultura = new CultureInfo("es-CO");
                    if (!DateTime.TryParse(s_fechaInicial, cultura, DateTimeStyles.None, out fechaInicial))
                    {
                        fechaInicial = ConstantesFramework.MinDateTimeController;
                    }
                    if (!DateTime.TryParse(s_fechaFinal, cultura, DateTimeStyles.None, out fechaFinal))
                    {
                        fechaFinal = DateTime.Now.Date.AddDays(1);
                    }
                    fechaInicial.AddDays(1);
                }
                else
                {
                    fechaInicial = ConstantesFramework.MinDateTimeController;
                    fechaFinal = DateTime.Now.Date.AddDays(1);
                }

                if (!string.IsNullOrWhiteSpace(rangoInicial))
                {
                    _rangoInicial = Convert.ToInt64(rangoInicial);
                }
                else
                    _rangoInicial = 0;
                if (!string.IsNullOrWhiteSpace(rangoFinal))
                {
                    _rangoFinal = Convert.ToInt64(rangoFinal);
                }
                else
                    _rangoFinal = 0;
                if (!string.IsNullOrWhiteSpace(idSuministro))
                {
                    _idSuministro = Convert.ToInt32(idSuministro);
                }
                if (!string.IsNullOrWhiteSpace(idCentroServicio))
                {
                    _idCentroServicio = Convert.ToInt32(idCentroServicio);
                }
                return contexto.paObtenerSumProvisionadoCsvFecha_SUM(usuario
                  , _idCentroServicio
                  , indicePagina
                  , registrosPorPagina
                  , fechaInicial
                  , fechaFinal
                  , _rangoInicial
                  , _rangoFinal
                  , _idSuministro
                  )
                  .ToList()
                  .ConvertAll(r => new SURemisionSuministroDC()
                  {
                      IdRemision = r.PSC_IdRemisionSuministros,
                      FechaRemision = r.RES_FechaGrabacion,
                      CiudadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.RES_IdLocalidadDestino,
                      },
                      GrupoSuministros = new SUGrupoSuministrosDC()
                      {
                          SuministroGrupo = new SUSuministro()
                          {
                              Id = r.SCS_IdSuministro,
                              Descripcion = r.SUM_Descripcion,
                              CantidadAsignada = (int)r.PSC_CantidadAsginada,
                              IdProvisionSuministro = r.PCS_IdProvisionSumCentroSvc,
                              IdAsignacionSuministro = r.SCS_IdSuministroCentroServicio,
                              RangoInicial = r.PCS_Inicio,
                              RangoFinal = r.PCS_Fin,
                              IdProvisionSuministroSerial = r.PCS_IdProvisionSumCentroSvcSerial,
                              IdPropietario = r.SCS_IdCentroServicios.ToString(),
                              NombrePropietario = r.RES_NombreDestinatario,
                              FechaFinalResolucion = r.PCS_FechaFinal,
                              FechaInicialResolucion = r.PCS_FechaInicial,
                              IdResolucion = r.PCS_IdNumerador
                          }
                      },
                      CentroServicioAsignacion = new PUCentroServiciosDC
                      {
                          IdCentroServicio = r.SCS_IdCentroServicios
                      }
                  });
            }
        }

        /// <summary>
        /// Retorna los suministros que esten en el rango de fecha seleccionado para una gestion
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<SURemisionSuministroDC> ObtenerSuministroAsignadoGestionXRangoFechas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial;
                DateTime fechaFinal;
                string rangoInicial;
                string rangoFinal;
                string s_fechaInicial;
                string s_fechaFinal;
                string idSuministro;
                string idProceso;
                string usuario;
                long? _rangoInicial = null;
                long? _rangoFinal = null;
                int? _idSuministro = null;
                long? _idProceso = null;

                filtro.TryGetValue("fechaInicial", out s_fechaInicial);
                filtro.TryGetValue("fechaFinal", out s_fechaFinal);
                filtro.TryGetValue("rangoInicial", out rangoInicial);
                filtro.TryGetValue("rangoFinal", out rangoFinal);
                filtro.TryGetValue("idProceso", out idProceso);
                filtro.TryGetValue("idSuministro", out idSuministro);
                filtro.TryGetValue("idUsuario", out usuario);


                // Con esto garantizo que si no viene fecha o si viene en un formato errado entonces se busque desde la fecha mínima
                // hasta la fecha de hoy.
                if (!string.IsNullOrWhiteSpace(s_fechaFinal) && !string.IsNullOrWhiteSpace(s_fechaInicial))
                {
                    CultureInfo cultura = new CultureInfo("es-CO");
                    if (!DateTime.TryParse(s_fechaInicial, cultura, DateTimeStyles.None, out fechaInicial))
                    {
                        fechaInicial = ConstantesFramework.MinDateTimeController;
                    }
                    if (!DateTime.TryParse(s_fechaFinal, cultura, DateTimeStyles.None, out fechaFinal))
                    {
                        fechaFinal = DateTime.Now.Date.AddDays(1);
                    }
                    fechaInicial.AddDays(1);
                }
                else
                {
                    fechaInicial = ConstantesFramework.MinDateTimeController;
                    fechaFinal = DateTime.Now.Date.AddDays(1);
                }

                if (!string.IsNullOrWhiteSpace(rangoInicial))
                {
                    _rangoInicial = Convert.ToInt64(rangoInicial);
                }
                else
                    _rangoInicial = 0;
                if (!string.IsNullOrWhiteSpace(rangoFinal))
                {
                    _rangoFinal = Convert.ToInt64(rangoFinal);
                }
                else
                    _rangoFinal = 0;

                if (!string.IsNullOrWhiteSpace(idSuministro))
                {
                    _idSuministro = Convert.ToInt32(idSuministro);
                }
                if (!string.IsNullOrWhiteSpace(idProceso))
                {
                    _idProceso = Convert.ToInt64(idProceso);
                }

                return contexto.paObtenerSumProvisionadoProcesoFecha_SUM(usuario
                  , _idProceso
                  , indicePagina
                  , registrosPorPagina
                  , fechaInicial
                  , fechaFinal
                  , _rangoInicial
                  , _rangoFinal
                  , _idSuministro
                  )
                  .ToList()
                  .ConvertAll(r => new SURemisionSuministroDC()
                  {
                      IdRemision = r.PSP_IdRemisionSuministros,
                      FechaRemision = r.RES_FechaGrabacion,
                      CiudadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.RES_IdLocalidadDestino,
                      },
                      GrupoSuministros = new SUGrupoSuministrosDC()
                      {
                          SuministroGrupo = new SUSuministro()
                          {
                              IdAsignacionSuministro = r.SUP_IdSuministroProceso,
                              Id = r.SUM_IdSuministro,
                              Descripcion = r.SUM_Descripcion,
                              CantidadAsignada = (int)r.PSP_CantidadAsginada,
                              RangoFinal = r.PPS_Fin,
                              RangoInicial = r.PPS_Inicio,
                              IdProvisionSuministro = r.PPS_IdProvisionSumProceso,
                              IdProvisionSuministroSerial = r.PPS_IdProvisionSumProcesoSerial,
                              IdPropietario = r.PRO_IdCodigoProceso.ToString(),
                              NombrePropietario = r.PRO_Descripcion,
                              FechaFinalResolucion = r.PPS_FechaFinal,
                              FechaInicialResolucion = r.PPS_FechaInicial,
                              IdResolucion = r.PPS_IdNumerador
                          }
                      },
                  });
            }
        }

        /// <summary>
        /// Desasigna la referencia del suministro para el mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="numeroSuministro"></param>
        /// <param name="idSuministro"></param>
        public void DesasignaSuministroProvisionReferencia(long rangoInicial, long rangoFinal, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarSuministroProvisionReferencia_SUM(rangoInicial, rangoFinal, idSuministro);
            }
        }

        public void ObtenerInformacionProvisionSumMensajero(int idProvisionSerial, long idMensajero, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProvisionSumMensajeroSerial_SUM sumSerial = contexto.ProvisionSumMensajeroSerial_SUM.Where(r => r.MSS_IdProvisionSumMensajeroSerial == idProvisionSerial).FirstOrDefault();
            }
        }

        /// <summary>
        /// Desasigna los rangos de la provision de un suministro a un mensajero
        /// </summary>
        /// <param name="idProvisionSerial"></param>
        /// <returns></returns>
        public void DesasignarRangoSuministrosRemisionMen(long idProvision, long idMensajero, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SUAudit.Instancia.AuditarProvisionSuministrosMensajero(contexto, idProvision, idSuministro, idMensajero, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Desasigna los rangos de la provision de un suministro a un mensajero
        /// </summary>
        /// <param name="idProvision"></param>
        /// <returns></returns>
        public void DesasignarRangoSuministrosRemisionSuc(long idProvision, int idSucursal, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SUAudit.Instancia.AuditarProvisionSuministrosSucursal(contexto, idProvision, idSuministro, idSucursal, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Desasigna los rangos de la provision de un suministro centro de servicios
        /// </summary>
        /// <param name="idProvision"></param>
        /// <returns></returns>
        public void DesasignarRangoSuministrosRemisionCentroSvc(long idProvision, long idCentroServicios, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SUAudit.Instancia.Auditar_ProvisionSuministrosCentroSvc(contexto, idProvision, idSuministro, idCentroServicios, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Desasigna los rangos de la provision de un suministro centro de servicios
        /// </summary>
        /// <param name="idProvision"></param>
        /// <returns></returns>
        public void DesasignarRangoSuministrosRemisionProceso(long idProvision, int idProceso, int idSuministro)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SUAudit.Instancia.AuditarProvisionSuministrosProceso(contexto, idProvision, idSuministro, idProceso, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Método para cambiar el estado de una remision a anulada
        /// </summary>
        /// <param name="remision"></param>
        public void AnularRemision(SURemisionSuministroDC remision)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RemisionSuministros_SUM remisionAct = contexto.RemisionSuministros_SUM.Where(r => r.RES_IdRemisionSuministros == remision.IdRemision).FirstOrDefault();
                if (remisionAct != null)
                {
                    remisionAct.RES_Estado = ConstantesFramework.ESTADO_ANULADO;
                    contexto.SaveChanges();
                }
            };
        }

        #endregion Desasignacion Suministros

        #region Numerador

        /// <summary>
        /// Actualiza el valor actual del numerador del suministro seleccionado
        /// </summary>
        /// <param name="idSuministro"></param>
        /// <param name="cantidadAsignada"></param>
        public SUNumeradorAutomatico ActualizarNumeradorRango(int idSuministro, int cantidadAsignada)
        {
            //using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using(SqlConnection sqlConn = new SqlConnection(conexionString))
            {
               
                SqlCommand cmd = new SqlCommand("paActualizarValorActualNumerador_SUM", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IDSUMINISTROS", idSuministro);
                cmd.Parameters.AddWithValue("@CANTIDADASIGNADA", cantidadAsignada);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var estado = dt.AsEnumerable().FirstOrDefault();
               /* var estado1 = contexto.paActualizarValorActualNumerador_SUM(idSuministro, cantidadAsignada).ToList();

                var estado = estado1.FirstOrDefault();*/

                if (estado == null || estado.Field<bool>("ESTADO") == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_NO_EXISTE)));
                }
                else if (!estado.Field<bool>("ESTADO"))
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_SUMINISTROS, EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO.ToString(), MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_NUM_SUMINISTRO_INVALIDO)));
                }

                SUNumeradorAutomatico numerador = new SUNumeradorAutomatico()
                {
                    FechaFinal = estado.Field<DateTime>("FECHAFINAL"),
                    FechaInicial = estado.Field<DateTime>("FECHAINICIAL"),
                    ValorActual = estado.Field<long>("ACTUAL"),
                    IdNumerador = estado.Field<string>("IDNUMERADOR")
                };

                return numerador;
            }
        }

        #endregion Numerador
    }
}