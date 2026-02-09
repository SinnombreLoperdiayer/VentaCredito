using CO.Servidor.CentroServicios.Comun;
using CO.Servidor.CentroServicios.Datos.Mapper;
using CO.Servidor.CentroServicios.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Transactions;

namespace CO.Servidor.CentroServicios.Datos
{
    /// <summary>
    /// Clase para consultar y persistir informacion en la base de datos para los procesos de centro de servicios
    /// </summary>
    public partial class PURepositorio
    {
        private static readonly PURepositorio instancia = new PURepositorio();
        private const string NombreModelo = "ModeloCentroServicios";
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private string conexionString = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string filePath = string.Empty;

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static PURepositorio Instancia
        {
            get { return PURepositorio.instancia; }
        }

        #region CRUD Tipo de comision fija

        /// <summary>
        /// Otiene los tipos de comision fija
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista de tipos de comision fija</returns>
        public IList<PUTipoComisionFija> ObtenerTiposComisionFija(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoComisionFija_COM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(r => new PUTipoComisionFija
                  {
                      IdTipoComFija = r.TCF_IdTipoComisionFija,
                      Descripcion = r.TCF_Descripcion
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un tipo de comision fija
        /// </summary>
        /// <param name="tipoComisionFija">Objeto tipo de comision fija</param>
        public void AdicionarTipoComisionFija(PUTipoComisionFija tipoComisionFija)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoComisionFija_COM tipoCom = new TipoComisionFija_COM()
                {
                    TCF_Descripcion = tipoComisionFija.Descripcion,
                    TCF_IdTipoComisionFija = 0,
                    TCF_CreadoPor = ControllerContext.Current.Usuario,
                    TCF_FechaGrabacion = DateTime.Now
                };
                contexto.TipoComisionFija_COM.Add(tipoCom);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de comision fija
        /// </summary>
        /// <param name="tipoComision">Objeto tipo de comision fija</param>
        public void EditarTipoComisionFija(PUTipoComisionFija tipoComision)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoComisionFija_COM tipoCom = contexto.TipoComisionFija_COM
                  .Where(r => r.TCF_IdTipoComisionFija == tipoComision.IdTipoComFija)
                  .SingleOrDefault();

                if (tipoCom == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                tipoCom.TCF_Descripcion = tipoComision.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de comision fija
        /// </summary>
        /// <param name="tipoComision">Tipo de comision fija</param>
        public void EliminarTipoComisionFija(PUTipoComisionFija tipoComision)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoComisionFija_COM tipoCom = contexto.TipoComisionFija_COM.Where(r => r.TCF_IdTipoComisionFija == tipoComision.IdTipoComFija).SingleOrDefault();
                if (tipoCom != null)
                {
                    contexto.TipoComisionFija_COM.Remove(tipoCom);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion CRUD Tipo de comision fija

        #region CRUD Tipo de descuento

        /// <summary>
        /// Otiene los tipos de descuento
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista de tipos de descuento</returns>
        public IList<PUTiposDescuento> ObtenerTiposDescuento(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoDescuento_COM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PUTiposDescuento
                  {
                      IdTipoDescuento = obj.TDE_IdTipoDescuento,
                      Descripcion = obj.TDE_Descripcion
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un tipo de descuento
        /// </summary>
        /// <param name="tipoDescuento">Objeto tipo de descuento</param>
        public void AdicionarTipoDescuento(PUTiposDescuento tipoDescuento)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoDescuento_COM tipoDes = new TipoDescuento_COM()
                {
                    TDE_Descripcion = tipoDescuento.Descripcion,
                    TDE_IdTipoDescuento = 0,
                    TDE_CreadoPor = ControllerContext.Current.Usuario,
                    TDE_FechaGrabacion = DateTime.Now
                };
                contexto.TipoDescuento_COM.Add(tipoDes);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de descuento
        /// </summary>
        /// <param name="tipoActEconomica">Objeto tipo de descuento</param>
        public void EditarTipoDescuento(PUTiposDescuento tipoDescuento)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoDescuento_COM tipoDes = contexto.TipoDescuento_COM
                  .Where(obj => obj.TDE_IdTipoDescuento == tipoDescuento.IdTipoDescuento)
                  .SingleOrDefault();

                if (tipoDes == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                tipoDes.TDE_Descripcion = tipoDescuento.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de descuento
        /// </summary>
        /// <param name="tipoDescuento">Tipo de descuento</param>
        public void EliminarTipoDescuento(PUTiposDescuento tipoDescuento)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoDescuento_COM tipoDes = contexto.TipoDescuento_COM.Where(obj => obj.TDE_IdTipoDescuento == tipoDescuento.IdTipoDescuento).SingleOrDefault();
                if (tipoDes != null)
                {
                    contexto.TipoDescuento_COM.Remove(tipoDes);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion CRUD Tipo de descuento

        #region CRUD Documentos centro de servicio

        /// <summary>
        /// Otiene los documentos de referencia de los centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de documentos de referencia de los centros de servicio</returns>
        public IList<PUDocuCentroServicio> ObtenerDocuCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsDocumentoCentroServicio_PUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PUDocuCentroServicio
                  {
                      IdDocuCentroServicio = obj.DCS_IdDocumento,
                      Descripcion = obj.DCS_Nombre,
                      Estado = obj.DCS_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                      Tipo = obj.DCS_Tipo
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un documento de referencia de los centros de servicio
        /// </summary>
        /// <param name="docuCentroServ">Objeto documento centro de servicio</param>
        public void AdicionarDocuCentrosServicio(PUDocuCentroServicio docuCentroServ)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DocumentoCentroServicio_PUA docuCentro = new DocumentoCentroServicio_PUA()
                {
                    DCS_Nombre = docuCentroServ.Descripcion,
                    DCS_IdDocumento = 0,
                    DCS_Estado = docuCentroServ.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO,
                    DCS_Tipo = docuCentroServ.Tipo,
                    DCS_CreadoPor = ControllerContext.Current.Usuario,
                    DCS_FechaGrabacion = DateTime.Now
                };
                contexto.DocumentoCentroServicio_PUA.Add(docuCentro);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un documento de referencia de los centros de servicio
        /// </summary>
        /// <param name="docuCentrosServicio">Objeto documento centro de servicio</param>
        public void EditarDocuCentrosServicio(PUDocuCentroServicio docuCentrosServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DocumentoCentroServicio_PUA tipoAct = contexto.DocumentoCentroServicio_PUA
                  .Where(obj => obj.DCS_IdDocumento == docuCentrosServicio.IdDocuCentroServicio)
                  .SingleOrDefault();

                if (tipoAct == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                tipoAct.DCS_Nombre = docuCentrosServicio.Descripcion;
                tipoAct.DCS_Estado = docuCentrosServicio.Estado ? ConstantesFramework.ESTADO_ACTIVO : ConstantesFramework.ESTADO_INACTIVO;
                tipoAct.DCS_Tipo = docuCentrosServicio.Tipo;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inactiva un documento de referencia de los centros de servicio
        /// </summary>
        /// <param name="docuCentroServ">Objeto documento centro de servicio</param>
        public void EliminarDocuCentrosServicio(PUDocuCentroServicio docuCentroServ)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DocumentoCentroServicio_PUA tipoAct = contexto.DocumentoCentroServicio_PUA
                  .Where(obj => obj.DCS_IdDocumento == docuCentroServ.IdDocuCentroServicio)
                  .SingleOrDefault();

                if (tipoAct == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                tipoAct.DCS_Estado = ConstantesFramework.ESTADO_INACTIVO;
                contexto.SaveChanges();
            }
        }

        #endregion CRUD Documentos centro de servicio

        #region CRUD Suministros

        /// <summary>
        /// Otiene los suministros
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de suministros</returns>
        public IList<PUSuministro> ObtenerSuministros(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsSuministro_SUM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PUSuministro
                  {
                      //IdExterno = obj.SUM_IdExterno,
                      Descripcion = obj.SUM_Descripcion,
                      IdSuministro = obj.SUM_IdSuministro,

                      // ManejaInventario = obj.SUM_ManejaInventario,
                      //StockMinimo = obj.SUM_StockMinimoReferencia
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un suministro
        /// </summary>
        /// <param name="suministro">Objeto suministro</param>
        public void AdicionarSuministro(PUSuministro suministro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM sumin = new Suministro_SUM()
                {
                    SUM_Descripcion = suministro.Descripcion,

                    //SUM_IdExterno = suministro.IdExterno,
                    SUM_IdSuministro = suministro.IdSuministro,

                    //SUM_ManejaInventario = suministro.ManejaInventario,
                    //SUM_StockMinimoReferencia = suministro.StockMinimo,
                    SUM_CreadoPor = ControllerContext.Current.Usuario,
                    SUM_FechaGrabacion = DateTime.Now
                };
                contexto.Suministro_SUM.Add(sumin);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un suministro
        /// </summary>
        /// <param name="suministro">Objeto suministro</param>
        public void EditarSuministro(PUSuministro suministro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM sumin = contexto.Suministro_SUM
                  .Where(obj => obj.SUM_IdSuministro == suministro.IdSuministro)
                  .SingleOrDefault();

                if (sumin == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                sumin.SUM_Descripcion = suministro.Descripcion;

                //sumin.SUM_IdExterno = suministro.IdExterno;
                //sumin.SUM_ManejaInventario = suministro.ManejaInventario;
                //sumin.SUM_StockMinimoReferencia = suministro.StockMinimo;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inactiva un suministro
        /// </summary>
        /// <param name="suministro">Objeto suministro</param>
        public void EliminarSuministro(PUSuministro suministro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Suministro_SUM ban = contexto.Suministro_SUM.Where(obj => obj.SUM_IdSuministro == suministro.IdSuministro).SingleOrDefault();
                if (ban != null)
                {
                    contexto.Suministro_SUM.Remove(ban);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion CRUD Suministros

        #region CRUD Servicio descuento Referencia

        /// <summary>
        /// Otiene los descuentos referencia de un servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de descuentos </returns>
        public IList<PUServicioDescuentoRef> ObtenerDescuentoReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (filtro.ContainsKey("SDR_IdTipoDescuento"))
                {
                    if (string.IsNullOrEmpty(filtro["SDR_IdTipoDescuento"]))
                        filtro.Remove("SDR_IdTipoDescuento");
                }

                if (filtro.ContainsKey("SDR_IdServicio"))
                {
                    if (string.IsNullOrEmpty(filtro["SDR_IdServicio"]))
                        filtro.Remove("SDR_IdServicio");
                }

                var d = contexto.ConsultarContainsServicioDescuentoReferen_COM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PUServicioDescuentoRef
                  {
                      IdServicio = obj.SDR_IdServicio,
                      NombreServicio = contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == obj.SDR_IdServicio).Single().SER_Nombre,
                      IdTipoDescuento = obj.SDR_IdTipoDescuento,
                      NombreTipoDescuento = contexto.TipoDescuento_COM.Where(des => des.TDE_IdTipoDescuento == obj.SDR_IdTipoDescuento).Single().TDE_Descripcion,
                      PorcentajeReferencia = obj.SDR_PorcentajeRef,
                      ValorCritPenalizacion = obj.SDR_ValorCriterioPenalizacion,
                      ValorReferencia = obj.SDR_ValorReferencia
                  }).ToList();
                return d;
            }
        }

        /// <summary>
        /// Adiciona un descuento referencia de un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto descuento referencia</param>
        public void AdicionarDescuentoRef(PUServicioDescuentoRef descuentoRef)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioDescuentoReferen_COM descuento = new ServicioDescuentoReferen_COM()
                {
                    SDR_IdServicio = descuentoRef.IdServicio,
                    SDR_IdTipoDescuento = Convert.ToInt16(descuentoRef.IdTipoDescuento),
                    SDR_PorcentajeRef = descuentoRef.PorcentajeReferencia,
                    SDR_ValorCriterioPenalizacion = descuentoRef.ValorCritPenalizacion,
                    SDR_ValorReferencia = descuentoRef.ValorReferencia,
                    SDR_CreadoPor = ControllerContext.Current.Usuario,
                    SDR_FechaGrabacion = DateTime.Now
                };
                contexto.ServicioDescuentoReferen_COM.Add(descuento);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un descuento referencia de un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto descuento referencia de un servicio</param>
        public void EditarDescuentoRef(PUServicioDescuentoRef descuentoRef)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioDescuentoReferen_COM descu = contexto.ServicioDescuentoReferen_COM
                  .Where(obj => obj.SDR_IdServicio == descuentoRef.IdServicio && obj.SDR_IdTipoDescuento == descuentoRef.IdTipoDescuento)
                  .SingleOrDefault();

                if (descu == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                descu.SDR_PorcentajeRef = descuentoRef.PorcentajeReferencia;
                descu.SDR_ValorCriterioPenalizacion = descuentoRef.ValorCritPenalizacion;
                descu.SDR_ValorReferencia = descuentoRef.ValorReferencia;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un descuento referencia de un servicio
        /// </summary>
        /// <param name="suministro">Objeto  descuento referencia de un servicio</param>
        public void EliminarDescuentoRef(PUServicioDescuentoRef descuentoRef)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioDescuentoReferen_COM des = contexto.ServicioDescuentoReferen_COM.Where(obj => obj.SDR_IdServicio == descuentoRef.IdServicio && obj.SDR_IdTipoDescuento == descuentoRef.IdTipoDescuento).SingleOrDefault();
                if (des != null)
                {
                    contexto.ServicioDescuentoReferen_COM.Remove(des);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion CRUD Servicio descuento Referencia

        #region CRUD Servicio comision Referencia

        /// <summary>
        /// Otiene las comisiones de referencia
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consulta</param>
        /// <returns>Lista de comisiones </returns>
        public IList<PUServicioComisionRef> ObtenerComisionReferencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (filtro.ContainsKey("SCR_IdTipoComision"))
                {
                    if (string.IsNullOrEmpty(filtro["SCR_IdTipoComision"]))
                        filtro.Remove("SCR_IdTipoComision");
                }

                if (filtro.ContainsKey("SCR_IdServicio"))
                {
                    if (string.IsNullOrEmpty(filtro["SCR_IdServicio"]))
                        filtro.Remove("SCR_IdServicio");
                }

                var d = contexto.ConsultarContainsServicioComisionReferencia_COM(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Select(obj => new PUServicioComisionRef
                  {
                      IdServicio = obj.SCR_IdServicio,
                      NombreServicio = contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == obj.SCR_IdServicio).Single().SER_Nombre,
                      IdTipoComision = obj.SCR_IdTipoComision,
                      NombreTipoComision = contexto.TipoComision_COM.Where(des => des.TCO_IdTipoComision == obj.SCR_IdTipoComision).Single().TCO_Descripcion,
                      Porcentaje = obj.SCR_Porcentaje,
                      Valor = obj.SCR_Valor
                  }).ToList();
                return d;
            }
        }

        /// <summary>
        /// Adiciona una comision referencia a un servicio
        /// </summary>
        /// <param name="descuentoRef">Objeto comision referencia</param>
        public void AdicionarComisionReferencia(PUServicioComisionRef comisionRef)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioComisionReferencia_COM comision = new ServicioComisionReferencia_COM()
                {
                    SCR_IdServicio = comisionRef.IdServicio,
                    SCR_IdTipoComision = Convert.ToInt16(comisionRef.IdTipoComision),
                    SCR_Porcentaje = comisionRef.Porcentaje,
                    SCR_Valor = comisionRef.Valor,
                    SCR_CreadoPor = ControllerContext.Current.Usuario,
                    SCR_FechaGrabacion = DateTime.Now
                };
                contexto.ServicioComisionReferencia_COM.Add(comision);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Modifica una comision referencia a un servicioun descuento referencia de un servicio
        /// </summary>
        /// <param name="ComisionRef">Objeto comision referencia a un servicio</param>
        public void EditarComisionReferencia(PUServicioComisionRef ComisionRef)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioComisionReferencia_COM comis = contexto.ServicioComisionReferencia_COM
                  .Where(obj => obj.SCR_IdServicio == ComisionRef.IdServicio && obj.SCR_IdTipoComision == ComisionRef.IdTipoComision)
                  .SingleOrDefault();

                if (comis == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                comis.SCR_Porcentaje = ComisionRef.Porcentaje;
                comis.SCR_Valor = ComisionRef.Valor;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una comision referencia
        /// </summary>
        /// <param name="suministro">Objeto  comision referencia</param>
        public void EliminarComisionReferencia(PUServicioComisionRef comisionRef)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioComisionReferencia_COM comis = contexto.ServicioComisionReferencia_COM.Where(obj => obj.SCR_IdServicio == comisionRef.IdServicio && obj.SCR_IdTipoComision == comisionRef.IdTipoComision).SingleOrDefault();
                if (comis != null)
                {
                    contexto.ServicioComisionReferencia_COM.Remove(comis);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion CRUD Servicio comision Referencia

        #region Propietarios (Concesionarios)

        public PUCentroServiciosDC ObtenerCentroServiciosPersonaResponsable(long idCentroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var f = contexto.CentroServicios_PUA.Where(r => r.CES_IdCentroServicios == idCentroServicios).FirstOrDefault();

                Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == f.CES_IdMunicipio).Single();
                string zona = contexto.Zona_PAR.Where(z => z.ZON_IdZona == f.CES_IdZona).Single().ZON_Descripcion;
                Propietario_PUA propietario = contexto.Propietario_PUA.Include("PersonaResponsableLegal_PAR.PersonaExterna_PAR").Where(p => p.CON_IdPropietario == f.CES_IdPropietario).Single();

                string DescripcionRacol = ".";
                string CentroServAdministrador = "";
                string TipoSubtipo = "";
                string tipoAgencia = ".";
                long IdColRacolApoyo = 0;

                Agencia_PUA agencia = contexto.Agencia_PUA.Include("CentroLogistico_PUA").Where(a => a.AGE_IdAgencia == f.CES_IdCentroServicios).SingleOrDefault();
                if (agencia != null)
                {
                    tipoAgencia = agencia.AGE_IdTipoAgencia;
                    TipoSubtipo = f.CES_Tipo + "-" + agencia.AGE_IdTipoAgencia;

                    if (agencia.AGE_IdTipoAgencia == ConstantesFramework.TIPO_CENTRO_SERVICIO_COL)
                        IdColRacolApoyo = agencia.CentroLogistico_PUA.CEL_IdRegionalAdm;
                    else
                        IdColRacolApoyo = agencia.AGE_IdCentroLogistico;
                }

                if (f.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
                {
                    var age = contexto.Agencia_PUA.Include("CentroServicios_PUA").Where(obj => obj.CentroServicios_PUA.CES_IdMunicipio == f.CES_IdMunicipio);
                    if (age != null && age.Count() > 0)
                    {
                        CentroServAdministrador = age.First().CentroServicios_PUA.CES_Nombre;
                        IdColRacolApoyo = age.First().CentroServicios_PUA.CES_IdCentroServicios;
                    }

                    TipoSubtipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO;

                    tipoAgencia = ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA;//se deja este tipo solo con el fin de cargarlo en cliente, pero no significa que se pueda cambiar de punto a agencia
                }

                PersonaExterna_PAR responsable = contexto.PersonaExterna_PAR.Include("PersonaResponsableServicio_PAR").Where(p => p.PEE_IdPersonaExterna == f.CES_IdPersonaResponsable).SingleOrDefault();

                PUCentroServiciosDC centroServicios = new PUCentroServiciosDC
                {
                    Direccion = f.CES_Direccion,
                    Barrio = f.CES_Barrio,
                    Email = f.CES_Email,
                    Fax = f.CES_Fax,
                    CodigoPostal = localidad.LOC_CodigoPostal,
                    IdMunicipio = f.CES_IdMunicipio,
                    IdPropietario = f.CES_IdPropietario,
                    NombrePropietario = propietario.CON_RazonSocial,
                    IdRepresentanteLegalPropietario = propietario.CON_IdRepresentanteLegal,
                    CiudadUbicacion = new PALocalidadDC()
                    {
                        IdLocalidad = localidad.LOC_IdLocalidad,
                        CodigoPostal = localidad.LOC_CodigoPostal
                    },
                    NombreMunicipio = localidad.LOC_Nombre,
                    IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                    NombreDepto = localidad.LOC_NombrePrimero,
                    IdPais = localidad.LOC_IdAncestroSegundoGrado,
                    NombrePais = localidad.LOC_NombreSegundo,
                    DigitoVerificacion = f.CES_DigitoVerificacion,
                    IdTipoPropiedad = f.CES_IdTipoPropiedad,
                    Estado = f.CES_Estado,
                    IdCentroCostos = f.CES_IdCentroCostos,
                    IdCentroServicio = f.CES_IdCentroServicios,
                    IdPersonaResponsable = f.CES_IdPersonaResponsable,
                    IdZona = f.CES_IdZona,
                    NombreZona = zona,
                    Latitud = f.CES_Latitud,
                    Longitud = f.CES_Longitud,
                    Nombre = f.CES_Nombre,
                    Sistematizado = f.CES_Sistematizada,
                    Telefono1 = f.CES_Telefono1,
                    Telefono2 = f.CES_Telefono2,
                    Tipo = f.CES_Tipo,
                    TipoOriginal = f.CES_Tipo,
                    AdmiteFormaPagoAlCobro = f.CES_AdmiteFormaPagoAlCobro,
                    PesoMaximo = f.CES_PesoMaximo,
                    VendePrepago = f.CES_VendePrepago,
                    VolumenMaximo = f.CES_VolumenMaximo,
                    IdTipoAgencia = tipoAgencia,
                    IdColRacolApoyo = IdColRacolApoyo,
                    CentroServiciosAdministrador = CentroServAdministrador,
                    DescripcionRacol = DescripcionRacol,
                    TipoSubtipo = TipoSubtipo,
                    TopeMaximoGiros = f.CES_TopeMaximoPorGiros,
                    PagaGiros = f.CES_PuedePagarGiros,
                    RecibeGiros = f.CES_PuedeRecibirGiros,
                    NombreAMostrar = f.CES_NombreAMostrar,
                    Codigo472 = f.CES_Codigo472,
                    FechaApertura = f.CES_FechaApertura,
                    FechaCierre = f.CES_FechaCierre,
                    PersonaResponsable = new PAPersonaExterna()
                    {
                        Identificacion = responsable.PEE_Identificacion,
                        IdTipoIdentificacion = responsable.PEE_IdTipoIdentificacion,
                        PrimerNombre = responsable.PEE_PrimerNombre,
                        SegundoNombre = responsable.PEE_SegundoNombre,
                        PrimerApellido = responsable.PEE_PrimerApellido,
                        SegundoApellido = responsable.PEE_SegundoApellido,
                        Direccion = responsable.PEE_Direccion,
                        DigitoVerificacion = responsable.PEE_DigitoVerificacion,
                        Telefono = responsable.PEE_Telefono
                    }
                };

                return centroServicios;
            }
        }

        /// <summary>
        /// Retorna los municipios que no permiten forma de pago "Al Cobro"
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<PUMunicipiosSinAlCobro> ObtenerMunicipiosSinFormaPagoAlCobro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsMunicipiosSinAlCobro_VPUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<PUMunicipiosSinAlCobro>(m => new PUMunicipiosSinAlCobro
                  {
                      Municipio = new PALocalidadDC
                      {
                          IdLocalidad = m.MSA_IdLocalidad,
                          Nombre = m.LOC_Nombre
                      }
                  }).ToList();
            }
        }

        /// <summary>
        /// Agrega municipio a la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RegistrarMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio, bool esPorSistema = false)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var mun = contexto.MunicipiosSinAlCobro_PUA.FirstOrDefault(m => m.MSA_IdLocalidad == municipio.IdLocalidad);
                if (mun == null)
                {
                    contexto.MunicipiosSinAlCobro_PUA.Add(new MunicipiosSinAlCobro_PUA
                    {
                        MSA_CreadoPor = ControllerContext.Current.Usuario,
                        MSA_FechaGrabacion = DateTime.Now,
                        MSA_IdLocalidad = municipio.IdLocalidad,
                        MSA_AgregadoPorSistema = esPorSistema
                    });
                }
                AuditarMunicipioSinFormaPagoAlCobro(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Quita municipio de la lista de municipios que no admiten forma de pago "al cobro"
        /// </summary>
        /// <param name="municipio"></param>
        public void RemoverMunicipioSinFormaPagoAlCobro(PALocalidadDC municipio, bool esAutomatico = false)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MunicipiosSinAlCobro_PUA mun;
                if (esAutomatico)
                {
                    mun = contexto.MunicipiosSinAlCobro_PUA.FirstOrDefault(m => m.MSA_IdLocalidad == municipio.IdLocalidad && m.MSA_AgregadoPorSistema);
                }
                else
                {
                    mun = contexto.MunicipiosSinAlCobro_PUA.FirstOrDefault(m => m.MSA_IdLocalidad == municipio.IdLocalidad);
                }

                if (mun != null)
                {
                    contexto.MunicipiosSinAlCobro_PUA.Remove(mun);
                }
                AuditarMunicipioSinFormaPagoAlCobro(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene los propietarios(concesionarios)
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los concesionarios</returns>
        public IList<PUPropietario> ObtenerPropietarios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsPropietariosCentrosServi_VPUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<PUPropietario>(r =>
                  {
                      Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == r.CON_IdMunicipio).Single();

                      PUPropietario propi = new PUPropietario
                      {
                          Direccion = r.CON_Direccion,
                          Barrio = r.CON_Barrio,
                          Email = r.CON_Email,
                          Fax = r.CON_Fax,
                          FechaConstitucion = r.CON_FechaConstitucion,
                          IdActividad = r.CON_IdActividad,
                          IdMunicipio = r.CON_IdMunicipio,
                          CiudadUbicacion = new PALocalidadDC()
                          {
                              IdLocalidad = localidad.LOC_IdLocalidad,
                              Nombre = localidad.NombreCompleto
                          },
                          IdPropietario = r.CON_IdPropietario,
                          IdRegimenContributivo = r.CON_IdRegimenContributivo,
                          IdRepresentanteLegal = r.CON_IdRepresentanteLegal,
                          IdTipoSociedad = r.CON_TipoSociedad,
                          PagaPorInterGiro = r.CON_PagaPorInterGiro,
                          RazonSocial = r.CON_RazonSocial,
                          Telefono = r.CON_Telefono,
                          NombreActividad = r.TAE_Descripcion,
                          NombreMunicipio = localidad.LOC_Nombre,
                          IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                          NombreDepto = localidad.LOC_NombrePrimero,
                          IdPais = localidad.LOC_IdAncestroSegundoGrado,
                          NombrePais = localidad.LOC_NombreSegundo,
                          NombreRegimenContributivo = r.TRC_Descripcion,
                          NombreTipoSociedad = r.TIS_Descripcion,
                          NombreRepresentanteLegal = r.PEE_PrimerNombre + " " + r.PEE_PrimerApellido,
                          DigitoVerificacion = r.CON_DigitoVerificacion,
                          Nit = r.CON_Nit
                      };
                      return propi;
                  }).ToList();
            }
        }

        /// <summary>
        /// Adiciona un propietario
        /// </summary>
        /// <param name="descuentoRef">Objeto comision referencia</param>
        public int AdicionarPropietario(PUPropietario propietario)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Propietario_PUA propiet = new Propietario_PUA()
                {
                    CON_Barrio = propietario.Barrio,
                    CON_CreadoPor = ControllerContext.Current.Usuario,
                    CON_DigitoVerificacion = propietario.DigitoVerificacion,
                    CON_Direccion = propietario.Direccion,
                    CON_Email = propietario.Email,
                    CON_Fax = propietario.Fax,
                    CON_FechaConstitucion = propietario.FechaConstitucion,
                    CON_FechaGrabacion = DateTime.Now,
                    CON_IdActividad = Convert.ToInt16(propietario.IdActividad),
                    CON_IdMunicipio = propietario.IdMunicipio,
                    CON_IdPropietario = propietario.IdPropietario,
                    CON_IdRegimenContributivo = Convert.ToInt16(propietario.IdRegimenContributivo),
                    CON_IdRepresentanteLegal = propietario.IdRepresentanteLegal,
                    CON_Nit = propietario.Nit,
                    CON_PagaPorInterGiro = propietario.PagaPorInterGiro,
                    CON_RazonSocial = propietario.RazonSocial,
                    CON_Telefono = propietario.Telefono,
                    CON_TipoSociedad = Convert.ToInt16(propietario.IdTipoSociedad)
                };
                contexto.Propietario_PUA.Add(propiet);
                contexto.SaveChanges();
                return propiet.CON_IdPropietario;
            }
        }

        /// <summary>
        /// Modifica un propietario
        /// </summary>
        /// <param name="propietario">Objeto propietario</param>
        public void EditarPropietario(PUPropietario propietario)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Propietario_PUA propie = contexto.Propietario_PUA
                  .Where(obj => obj.CON_IdPropietario == propietario.IdPropietario)
                  .SingleOrDefault();

                if (propie == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                propie.CON_Barrio = propietario.Barrio;
                propie.CON_DigitoVerificacion = propietario.DigitoVerificacion;
                propie.CON_Direccion = propietario.Direccion;
                propie.CON_Email = propietario.Email;
                propie.CON_Fax = propietario.Fax;
                propie.CON_FechaConstitucion = propietario.FechaConstitucion;
                propie.CON_IdActividad = Convert.ToInt16(propietario.IdActividad);
                propie.CON_IdMunicipio = propietario.IdMunicipio;
                propie.CON_IdRegimenContributivo = Convert.ToInt16(propietario.IdRegimenContributivo);
                propie.CON_IdRepresentanteLegal = propietario.IdRepresentanteLegal;
                propie.CON_Nit = propietario.Nit;
                propie.CON_PagaPorInterGiro = propietario.PagaPorInterGiro;
                propie.CON_RazonSocial = propietario.RazonSocial;
                propie.CON_Telefono = propietario.Telefono;
                propie.CON_TipoSociedad = Convert.ToInt16(propietario.IdTipoSociedad);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un propietario
        /// </summary>
        /// <param name="propietario">Objeto propietario</param>
        public void EliminarPropietario(PUPropietario propietario)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ///Elimina los archivos asociados al propietario
                List<ArchivosPropietario_PUA> archivos = contexto.ArchivosPropietario_PUA.Where(a => a.APR_IdPropietario == propietario.IdPropietario).ToList();

                for (int i = archivos.Count - 1; i >= 0; i--)
                {
                    contexto.ArchivosPropietario_PUA.Remove(archivos[i]);
                }

                ///Elimina la relacion con los bancos
                List<ConcesionarioBanco_PUA> bancos = contexto.ConcesionarioBanco_PUA.Where(b => b.COB_IdPropietario == propietario.IdPropietario).ToList();
                for (int i = bancos.Count - 1; i >= 0; i--)
                {
                    contexto.ConcesionarioBanco_PUA.Remove(bancos[i]);
                }

                Propietario_PUA propie = contexto.Propietario_PUA.Where(obj => obj.CON_IdPropietario == propietario.IdPropietario).SingleOrDefault();
                if (propie != null)
                {
                    contexto.Propietario_PUA.Remove(propie);
                    contexto.SaveChanges();
                }
            }
        }

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de los propietarios
        /// </summary>
        /// <returns>lista con los archivos de los propietarios</returns>
        public IEnumerable<PUArchivosPropietario> ObtenerArchivosPropietarios(PUPropietario propietario)
        {
            List<PUArchivosPropietario> listaPropietarios = new List<PUArchivosPropietario>();
            string query = @"SELECT dbo.ArchivosPersonas_PUA.*  FROM  dbo.ArchivosPersonas_PUA INNER JOIN " +
                     " ICONTROLLER.dbo.ArchivosPropietario_PUA ON ARP_IdArchivosPersonas = APR_IdArchivosPersonas " +
                     " WHERE APR_IdPropietario = " + propietario.IdPropietario;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        listaPropietarios.Add
                            (
                            new PUArchivosPropietario()
                            {
                                IdPropietario = propietario.IdPropietario,
                                ArchivosPersonas = new PUArchivosPersonas
                                {
                                    IdArchivo = Convert.ToInt64(r["ARP_IdArchivosPersonas"]),
                                    IdDocumento = Convert.ToInt16(r["ARP_IdDocumento"]),
                                    Fecha = Convert.ToDateTime(r["ARP_FechaGrabacion"]),
                                    IdAdjunto = new Guid(r["ARP_IdAdjunto"].ToString()),
                                    NombreAdjunto = r["ARP_NombreAdjunto"].ToString(),
                                    EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                                }
                            }
                            );
                    };
                }
                sqlConn.Close();
            }

            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listadocumentos = contexto.DocumentoCentroServicio_PUA
                .Where(t => t.DCS_Estado != ConstantesFramework.ESTADO_INACTIVO && t.DCS_Tipo == PUConstantesCentroServicios.TipoDocumentosAgenteComercial)
                .OrderByDescending(o => o.DCS_IdDocumento)
                .ToList()
                .ConvertAll<PUArchivosPropietario>(r => new PUArchivosPropietario()
                {
                    ArchivosPersonas = new PUArchivosPersonas()
                    {
                        IdDocumento = r.DCS_IdDocumento,
                        NombreDocumento = r.DCS_Nombre,
                        EstadoDocumento = r.DCS_Estado,
                    },
                    IdPropietario = propietario.IdPropietario
                });

                foreach (PUArchivosPropietario documento in listadocumentos)
                {
                    foreach (PUArchivosPropietario archivo in listaPropietarios)
                    {
                        if (documento.ArchivosPersonas.IdDocumento == archivo.ArchivosPersonas.IdDocumento)
                        {
                            documento.ArchivosPersonas.IdAdjunto = archivo.ArchivosPersonas.IdAdjunto;
                            documento.ArchivosPersonas.IdArchivo = archivo.ArchivosPersonas.IdArchivo;
                            documento.IdPropietario = archivo.IdPropietario;
                            documento.ArchivosPersonas.NombreAdjunto = archivo.ArchivosPersonas.NombreAdjunto;
                            documento.ArchivosPersonas.Fecha = archivo.ArchivosPersonas.Fecha;
                            documento.ArchivosPersonas.EstadoRegistro = archivo.ArchivosPersonas.EstadoRegistro;
                        }
                    }
                }

                return listadocumentos;
            }
        }

        /// <summary>
        /// Adiciona archivo de un propietario
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivoPropietario(PUArchivosPropietario archivo)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.CLIENTES, archivo.ArchivosPersonas.NombreServidor);
            byte[] archivoImagen;
            int id = 0;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = @"INSERT INTO [ArchivosPersonas_PUA] WITH (ROWLOCK)" +
            " ([ARP_Adjunto] ,[ARP_IdDocumento]  ,[ARP_IdAdjunto]  ,[ARP_NombreAdjunto] ,[ARP_FechaGrabacion] ,[ARP_CreadoPor])  " +
           " VALUES(@Adjunto ,@IdDocumento ,@IdAdjunto,@NombreAdjunto ,GETDATE() ,@CreadoPor) ;  SELECT SCOPE_IDENTITY();";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", archivo.ArchivosPersonas.NombreAdjunto));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdDocumento", archivo.ArchivosPersonas.IdDocumento));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                var rowCount = cmd.ExecuteScalar();
                sqlConn.Close();
                if (rowCount != DBNull.Value)
                    id = Convert.ToInt32(rowCount);
            }

            query = @"INSERT INTO ICONTROLLER.dbo.ArchivosPropietario_PUA WITH (ROWLOCK)" +
            " ([APR_IdPropietario] ,[APR_IdArchivosPersonas] ,[APR_FechaGrabacion] ,[APR_CreadoPor])  " +
           " VALUES(@IdPropietario ,@IdArchivosPersonas ,GETDATE() ,@CreadoPor)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@IdPropietario", archivo.IdPropietario));
                cmd.Parameters.Add(new SqlParameter("@IdArchivosPersonas", id));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));
                var rowCount = cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Elimina archivo de un propietario
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void EliminarArchivoPropietario(PUArchivosPropietario archivo)
        {
            string query = "DELETE FROM [ArchivosPersonas_PUA] WITH (ROWLOCK)" +
          "WHERE  ARP_IdArchivosPersonas = " + archivo.ArchivosPersonas.IdArchivo;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un propietario
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoPropietario(PUArchivosPropietario archivo)
        {
            string respuesta;
            string query = "@SELECT  dbo.ArchivosPersonas_PUA.ARP_Adjunto  FROM  dbo.ArchivosPersonas_PUA" +
                     " WHERE ARP_IdArchivosPersonas = " + archivo.ArchivosPersonas.IdArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    respuesta = string.Empty;
                }
                else
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ARP_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }
        }

        #endregion Archivos

        #endregion Propietarios (Concesionarios)

        #region Codeudor

        /// <summary>
        /// Obtiene lista de codeudores de un Centro de servicio
        /// </summary>
        /// <returns>Lista con los codeudores de un  Centro de servicio</returns>
        public IList<PUCodeudor> ObtenerCodeudoresXCentroServicio(long idCentroServicio, IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<CodeudorCentroServicio_VPUA>("CCS_IdCentroServicios", idCentroServicio.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarCodeudorCentroServicio_VPUA(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll(res =>
                  {
                      Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == res.PEE_Municipio).Single();
                      PUCodeudor codeudor = new PUCodeudor()
                      {
                          Email = res.PRL_Email,
                          EmpresaEmpleador = res.PRL_EmpresaEmpleador,
                          Fax = res.PRL_Fax,
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

                      return codeudor;
                  }
                  ).OrderBy(obj => obj.NombreCompuesto).ToList();
            }
        }

        /// <summary>
        /// Adiciona un codeudor
        /// </summary>
        /// <param name="codeudor">Objeto codeudor</param>
        public void AdicionarCodeudor(PUCodeudor codeudor)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fecha = DateTime.Now;

                PersonaExterna_PAR perExt = contexto.PersonaExterna_PAR.Where(p => p.PEE_Identificacion == codeudor.PersonaExterna.Identificacion.Trim() && p.PEE_IdTipoIdentificacion == codeudor.PersonaExterna.IdTipoIdentificacion).FirstOrDefault();
                PersonaResponsableLegal_PAR res = null;
                if (perExt == null)
                {
                    perExt = new PersonaExterna_PAR()
                    {
                        PEE_CreadoPor = ControllerContext.Current.Usuario,
                        PEE_DigitoVerificacion = codeudor.PersonaExterna.DigitoVerificacion,
                        PEE_Direccion = codeudor.PersonaExterna.Direccion,
                        PEE_FechaExpedicionDocumento = codeudor.PersonaExterna.FechaExpedicionDocumento,
                        PEE_FechaGrabacion = fecha,
                        PEE_Identificacion = codeudor.PersonaExterna.Identificacion.Trim(),
                        PEE_IdPersonaExterna = 0,
                        PEE_IdTipoIdentificacion = codeudor.PersonaExterna.IdTipoIdentificacion,
                        PEE_Municipio = codeudor.PersonaExterna.Municipio,
                        PEE_NumeroCelular = codeudor.PersonaExterna.NumeroCelular,
                        PEE_PrimerNombre = codeudor.PersonaExterna.PrimerNombre,
                        PEE_PrimerApellido = codeudor.PersonaExterna.PrimerApellido,
                        PEE_SegundoApellido = codeudor.PersonaExterna.SegundoApellido,
                        PEE_SegundoNombre = codeudor.PersonaExterna.SegundoNombre,
                        PEE_Telefono = codeudor.PersonaExterna.Telefono
                    };
                    contexto.PersonaExterna_PAR.Add(perExt);

                    res = new PersonaResponsableLegal_PAR()
                    {
                        PRL_CreadoPor = ControllerContext.Current.Usuario,
                        PRL_Email = codeudor.Email,
                        PRL_EmpresaEmpleador = codeudor.EmpresaEmpleador,
                        PRL_Fax = codeudor.Fax,
                        PRL_FechaGrabacion = fecha,
                        PRL_IdPersonaExterna = perExt.PEE_IdPersonaExterna,
                        PRL_IngresosEmpleoActual = codeudor.IngresosEmpleoActual,
                        PRL_Ocupacion = codeudor.Ocupacion,
                        PRL_PoseeFincaRaiz = codeudor.PoseeFincaRaiz,
                        PRL_Telefono = codeudor.PersonaExterna.Telefono
                    };
                    contexto.PersonaResponsableLegal_PAR.Add(res);
                }
                else
                {
                    res = contexto.PersonaResponsableLegal_PAR.Where(p => p.PRL_IdPersonaExterna == perExt.PEE_IdPersonaExterna).SingleOrDefault();
                    if (res == null)
                    {
                        res = new PersonaResponsableLegal_PAR()
                        {
                            PRL_CreadoPor = ControllerContext.Current.Usuario,
                            PRL_Email = codeudor.Email,
                            PRL_EmpresaEmpleador = codeudor.EmpresaEmpleador,
                            PRL_Fax = codeudor.Fax,
                            PRL_FechaGrabacion = fecha,
                            PRL_IdPersonaExterna = perExt.PEE_IdPersonaExterna,
                            PRL_IngresosEmpleoActual = codeudor.IngresosEmpleoActual,
                            PRL_Ocupacion = codeudor.Ocupacion,
                            PRL_PoseeFincaRaiz = codeudor.PoseeFincaRaiz,
                            PRL_Telefono = codeudor.PersonaExterna.Telefono
                        };
                        contexto.PersonaResponsableLegal_PAR.Add(res);
                    }
                }

                CodeudorCentroServicios_PUA cod = new CodeudorCentroServicios_PUA()
                {
                    CCS_CreadoPor = ControllerContext.Current.Usuario,
                    CCS_FechaGrabacion = fecha,
                    CCS_IdCentroServicios = codeudor.idCentroServicios,
                    CCS_IdPersonaExterna = perExt.PEE_IdPersonaExterna
                };

                contexto.CodeudorCentroServicios_PUA.Add(cod);
                contexto.SaveChanges();
            }
        }

        #endregion Codeudor

        #region Consultas Basicas

        /// <summary>
        /// Método para obtener el representante legal de un punto
        /// </summary>
        /// <param name="idcentroservicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerRepresentanteLegalPunto(long idcentroservicio)
        {
            DataTable dtRepresentante = new DataTable();

            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRepresentanteLegal", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", idcentroservicio);
                cmd.Parameters.AddWithValue("@CargaMasiva", 1);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dtRepresentante);
                conn.Close();

                return dtRepresentante.AsEnumerable().ToList().ConvertAll<PUCentroServiciosDC>(r =>
                {
                    PUCentroServiciosDC rep = new PUCentroServiciosDC()
                    {
                        IdentificacionPropietario = !r.IsNull("PEE_Identificacion") ? r.Field<string>("PEE_Identificacion") : string.Empty,
                        NombrePropietario = !r.IsNull("Nombres") ? r.Field<string>("Nombres") : string.Empty
                    };
                    return rep;
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtener la agencia a partir de la localidad
        /// </summary>
        ///// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciaLocalidad_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", localidad);

                SqlParameter paramOut = new SqlParameter("@NombreLocalidad", SqlDbType.VarChar, 100);
                paramOut.Direction = ParameterDirection.Output;
                paramOut.IsNullable = true;
                cmd.Parameters.Add(paramOut);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var dr = dt.AsEnumerable().FirstOrDefault();


                switch (dr.Field<string>("Tipo"))
                {
                    case "NOEXISTE":
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), paramOut.Value.ToString()));
                        throw new FaultException<ControllerException>(excepcion);

                        break;

                    case "AGE":

                        return new PUCentroServiciosDC()
                        {
                            IdCentroServicio = dr.Field<long>("AGE_IdAgencia"),
                            Nombre = dr.Field<string>("CES_Nombre"),
                            IdMunicipio = dr.Field<string>("LOC_IdLocalidad"),
                            NombreMunicipio = dr.Field<string>("LOC_Nombre"),
                            Telefono1 = dr.Field<string>("CES_Telefono1"),
                            Direccion = dr.Field<string>("CES_Direccion"),
                            Tipo = dr.Field<string>("CES_Tipo"),
                            TipoSubtipo = dr.Field<string>("AGE_IdTipoAgencia"),
                            Sistematizado = dr.Field<bool>("CES_Sistematizada")
                        };

                        break;

                    case "COL":
                        return new PUCentroServiciosDC()
                        {
                            IdCentroServicio = dr.Field<long>("MCL_IdCentroLogistico"),
                            Nombre = dr.Field<string>("CES_Nombre"),
                            IdMunicipio = dr.Field<string>("MCL_IdLocalidad"),
                            NombreMunicipio = dr.Field<string>("LOC_Nombre"),
                            Telefono1 = dr.Field<string>("CES_Telefono1"),
                            Direccion = dr.Field<string>("CES_Direccion"),
                            Tipo = dr.Field<string>("CES_Tipo"),
                            Sistematizado = dr.Field<bool>("CES_Sistematizada")
                        };

                        break;

                }


                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), "DESCONOCIDO")));



            }
        }


        /// <summary>
        /// Obtener la agencia a partir de la localida
        /// </summary>
        /// <param name="localidad"></param>
        /* public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
         {
             using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
             {
                 Localidad_PAR localidadIn = contexto.Localidad_PAR.Where(loc => loc.LOC_IdLocalidad == localidad).FirstOrDefault();
                 Agencia_VPUA agencia = contexto.Agencia_VPUA.Where(age => age.LOC_IdLocalidad == localidad && age.CES_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();

                 if (agencia == null)
                 {
                     LocalidadesCol_VPUA col = contexto.LocalidadesCol_VPUA.Where(colE => colE.MCL_IdLocalidad == localidad).FirstOrDefault();
                     if (col == null)
                     {
                         ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), localidadIn.LOC_Nombre));
                         throw new FaultException<ControllerException>(excepcion);
                     }
                     else
                     {
                         return new PUCentroServiciosDC()
                         {
                             IdCentroServicio = col.MCL_IdCentroLogistico,
                             Nombre = col.CES_Nombre,
                             IdMunicipio = col.MCL_IdLocalidad,
                             NombreMunicipio = col.LOC_Nombre,
                             Telefono1 = col.CES_Telefono1,
                             Direccion = col.CES_Direccion,
                             Tipo = col.CES_Tipo,
                             Sistematizado = col.CES_Sistematizada
                         };
                     }
                 }
                 return new PUCentroServiciosDC()
                 {
                     IdCentroServicio = agencia.AGE_IdAgencia,
                     Nombre = agencia.CES_Nombre,
                     IdMunicipio = agencia.LOC_IdLocalidad,
                     NombreMunicipio = agencia.LOC_Nombre,
                     Telefono1 = agencia.CES_Telefono1,
                     Direccion = agencia.CES_Direccion,
                     Tipo = agencia.CES_Tipo,
                     TipoSubtipo = agencia.AGE_IdTipoAgencia,
                     Sistematizado = agencia.CES_Sistematizada
                 };
             }
         }
         */
        /// <summary>
        /// Retorna la lista de puntos y agencias dependientes de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasDependientes(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CentroServiciosReporteDinero_VPUA> agenciasDependientes = new List<CentroServiciosReporteDinero_VPUA>();
                ObtenerAgenciasDependientes(idCentroServicio, contexto, agenciasDependientes, idCentroServicio);
                return agenciasDependientes
                  .ConvertAll(cs => new PUCentroServiciosDC
                  {
                      IdCentroServicio = cs.CRD_IdCentroServiciosQueReporta,
                      Nombre = cs.CES_Nombre
                  }); ;
            }
        }

        /// <summary>
        /// Retorna los centros de servicio que reportan al centro de servicio dado como parámetro
        /// </summary>
        /// <param name="idCentroServicio">Centro de servicio de quien se desea conocer quienes reportan</param>
        /// <param name="contexto">Contexto de base de datos</param>
        /// <param name="agenciasDependientes">Listado que va actualizando los centros de servicio dependientes</param>
        /// <param name="idCentroServicioPadre">El id del centro de servicio que inicia la solicitud.</param>
        internal void ObtenerAgenciasDependientes(long idCentroServicio, ModeloCentroServicios contexto, List<CentroServiciosReporteDinero_VPUA> agenciasDependientes, long idCentroServicioPadre)
        {
            IQueryable<CentroServiciosReporteDinero_VPUA> centros = contexto.CentroServiciosReporteDinero_VPUA.Where(cs => cs.CRD_IdCentroServiciosAQuienReporta == idCentroServicio);
            agenciasDependientes.AddRange(centros);

            if (centros.Count() > 0)
            {
                foreach (var a in centros)
                {
                    if (a.CRD_IdCentroServiciosQueReporta != idCentroServicioPadre)
                    {
                        ObtenerAgenciasDependientes(a.CRD_IdCentroServiciosQueReporta, contexto, agenciasDependientes, idCentroServicioPadre);
                    }
                }
            }
        }

        /// <summary>
        /// Obtener el centro logistico en el que se apoya un municipio
        /// </summary>
        /// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerCentroLogisticoApoyaMunicipio(string localidad)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MunicipioCentroLogistico_PUA agencia = contexto.MunicipioCentroLogistico_PUA.Include("CentroLogistico_PUA").Include("CentroServicios_PUA").Where(age => age.MCL_IdLocalidad == localidad).FirstOrDefault();

                if (agencia == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), localidad));
                    throw new FaultException<ControllerException>(excepcion);
                }
                return new PUCentroServiciosDC()
                {
                    IdCentroServicio = agencia.MCL_IdCentroLogistico,
                    Nombre = agencia.CentroLogistico_PUA.CentroServicios_PUA.CES_Nombre,
                    IdMunicipio = agencia.MCL_IdLocalidad,
                };
            }
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsAgencia_VPUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      IdCentroServicio = r.CES_IdCentroServicios,
                      Nombre = r.CES_Nombre,
                      NombreMunicipio = r.LOC_Nombre,
                      Telefono1 = r.CES_Telefono1,
                      Direccion = r.CES_Direccion
                  });
            }
        }


        /// <summary>
        /// Obtiene las Agencias y Bodegas para la Validacion y Archivo - Control de Cuentas
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasBodegas(IDictionary<string, string> filtro)
        {
            List<PUCentroServiciosDC> resultado = null;
            //totalRegistros = 0;
            using (SqlConnection conexion = new SqlConnection(conexionString))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAgenciasBodegas_VPUA", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                string idCentroServicio = null;
                if (filtro.TryGetValue("CES_IdCentroServicios", out idCentroServicio))
                {
                    long idCs = Convert.ToInt64(idCentroServicio);
                    cmd.Parameters.AddWithValue("@IdCentroServicio", idCs);
                }

                var reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var r = new PUCentroServiciosDC
                        {
                            IdCentroServicio = Convert.ToInt64(reader["CES_IdCentroServicios"]),
                            Nombre = reader["CES_Nombre"].ToString(),
                            NombreMunicipio = reader["LOC_Nombre"].ToString(),
                            Telefono1 = reader["CES_Telefono1"].ToString(),
                            Direccion = reader["CES_Direccion"].ToString()
                        };

                        if (resultado == null)
                        {
                            resultado = new List<PUCentroServiciosDC>();
                        }

                        resultado.Add(r);
                    }
                }
            }

            return resultado;
        }


        /// <summary>
        /// Obtiene la informacion de una agencia dependiendo del id
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgencia(long idAgencia)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Agencia_VPUA agencia = contexto.Agencia_VPUA.Where(a => a.AGE_IdAgencia == idAgencia && a.CES_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();
                if (agencia != null)
                {
                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = agencia.CES_IdCentroServicios,
                        Nombre = agencia.CES_Nombre,
                        NombreMunicipio = agencia.LOC_Nombre,
                        Telefono1 = agencia.CES_Telefono1,
                        Direccion = agencia.CES_Direccion,
                        Tipo = agencia.CES_Tipo,
                        TipoSubtipo = agencia.AGE_IdTipoAgencia
                    };
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_AGENCIA_NO_EXISTE_INACTIVA.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_AGENCIA_NO_EXISTE_INACTIVA));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Retorna la lista de centro de servicios que reportan dinero a un centro de servicio dado
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicioAQuienReportan"></param>
        /// <returns></returns>
        public List<PUCentroServicioReporte> ObtenerCentrosServicioReportan(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicioAQuienReportan)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                filtro.Add("CRD_IdCentroServiciosAQuienReporta", idCentroServicioAQuienReportan.ToString());
                List<PUCentroServicioReporte> centrosReportan = contexto
                  .ConsultarContainsCentroServiciosReporteDinero_VPUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new PUCentroServicioReporte()
                  {
                      IdCentroServicio = r.CRD_IdCentroServiciosQueReporta,
                      Nombre = r.CES_Nombre
                  });
                return centrosReportan;
            }
        }

        /// <summary>
        /// Retorna los centro de servicio que reportan a un centro de servicio dado
        /// </summary>
        /// <param name="idCentroServicio">Id del centro de servicio</param>
        /// <returns></returns>
        public List<PUCentroServicioReporte> ObtenerCentrosServicioReportanCentroServicio(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServiciosReporteDinero_VPUA
                  .Where(centro => centro.CRD_IdCentroServiciosAQuienReporta == idCentroServicio)
                  .ToList()
                  .ConvertAll(centro => new PUCentroServicioReporte { IdCentroServicio = centro.CRD_IdCentroServiciosQueReporta, Nombre = centro.CES_Nombre });
            }
        }

        /// <summary>
        /// Adiciona el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void AdicionarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var centroServiciosReporta = contexto.CentroServiciosReporteDinero_PUA.FirstOrDefault(centroServicio => centroServicio.CRD_IdCentroServiciosQueReporta == idCentroServicioReporta);

                if (centroServiciosReporta != null)
                {
                    contexto.CentroServiciosReporteDinero_PUA.Remove(centroServiciosReporta);
                }

                contexto.CentroServiciosReporteDinero_PUA.Add(new CentroServiciosReporteDinero_PUA()
                {
                    CRD_IdCentroServiciosAQuienReporta = idCentroServicioAQuienReporta,
                    CRD_IdCentroServiciosQueReporta = idCentroServicioReporta,
                    CRD_CreadoPor = ControllerContext.Current.Usuario,
                    CRD_FechaGrabacion = DateTime.Now
                });

                AuditarCentroServicioReporta(contexto);
                contexto.SaveChanges();

            }
        }

        /// <summary>
        /// Eliminar el centro de servicios al que se le reporta
        /// </summary>
        /// <param name="idCentroServicioAQuienReporta"></param>
        /// <param name="idCentroServicioReporta"></param>
        public void EliminarCentroServicioReporte(long idCentroServicioAQuienReporta, long idCentroServicioReporta)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServiciosReporteDinero_PUA reg = contexto.CentroServiciosReporteDinero_PUA
                  .FirstOrDefault(cs =>
                    cs.CRD_IdCentroServiciosAQuienReporta == idCentroServicioAQuienReporta &&
                    cs.CRD_IdCentroServiciosQueReporta == idCentroServicioReporta);
                if (reg != null)
                {
                    contexto.CentroServiciosReporteDinero_PUA.Remove(reg);
                    AuditarCentroServicioReporta(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene las agencias de la aplicación sin filtro
        /// </summary>
        public List<PUCentroServiciosDC> ObtenerAgencias()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Agencia_VPUA.ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      IdCentroServicio = r.CES_IdCentroServicios,
                      Nombre = r.CES_Nombre,
                      NombreMunicipio = r.LOC_Nombre,
                      Telefono1 = r.CES_Telefono1,
                      Direccion = r.CES_Direccion,
                      Tipo = r.CES_Tipo
                  });
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de sociedad
        /// </summary>
        /// <returns>Lista con los tipos de sociedad</returns>
        public IList<PUTipoSociedad> ObtenerTipoSociedad()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSociedad_PAR.Select(obj =>
                  new PUTipoSociedad()
                  {
                      Descripcion = obj.TIS_Descripcion,
                      IdTipoSociedad = obj.TIS_IdTipoSociedad
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Otiene todos los tipos de descuento
        /// </summary>
        /// <returns>Lista con los tipos de descuento</returns>
        public IList<PUTiposDescuento> ObtenerTiposDescuento()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoDescuento_COM.Select(obj =>
                  new PUTiposDescuento()
                  {
                      Descripcion = obj.TDE_Descripcion,
                      IdTipoDescuento = obj.TDE_IdTipoDescuento
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los servicios
        /// </summary>
        /// <returns>Lista con los servicios</returns>
        public IList<PUServicio> ObtenerServicios()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Servicio_TAR.Select(obj =>
                  new PUServicio()
                  {
                      IdServicio = obj.SER_IdServicio,
                      NombreServicio = obj.SER_Nombre
                  }
                  ).OrderBy(obj => obj.NombreServicio).ToList();
            }
        }

        /// <summary>
        /// Otiene todos los tipos de comision
        /// </summary>
        /// <returns>Lista con los tipos de comision</returns>
        public IList<PUTiposComision> ObtenerTiposComision()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoComision_COM.Select(obj =>
                  new PUTiposComision()
                  {
                      Descripcion = obj.TCO_Descripcion,
                      IdTipoComision = obj.TCO_IdTipoComision
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de regimen tributario
        /// </summary>
        /// <returns>Lista con los tipos de regimen tributario</returns>
        public IList<PUTipoRegimen> ObtenerTiposRegimen()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoRegimenContributivo_PAR.Select(obj =>
                  new PUTipoRegimen()
                  {
                      Descripcion = obj.TRC_Descripcion,
                      IdRegimen = obj.TRC_IdRegimen
                  }
                  ).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Indica si el centro de servicio pasado como parámetro asociado tiene el servicio de Komprech
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public bool CentroServicioTieneServicioKomprechAsociado(long idCentroServicios, string unidadNegocioKomprech, int idServicioKomprech)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                // Además de validar servicios activos determiina si tiene horario configurado el día y hora en que se realiza la transacción
                IQueryable<CentroServicioServicio_PUA> unidadesNegocio = contexto.CentroServicioServicio_PUA.Include("Servicio_TAR")
                    .Where(
                      servicio =>
                          servicio.CSS_IdCentroServicios == idCentroServicios &&
                          servicio.CSS_FechaInicioVenta <= DateTime.Now &&
                          servicio.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                          servicio.Servicio_TAR.SER_IdUnidadNegocio == unidadNegocioKomprech
                      );
                return unidadesNegocio.FirstOrDefault(u => u.Servicio_TAR.SER_IdServicio == idServicioKomprech) != null;
            }
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de servicios y retorna  la lista de servicios habilitados
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <param name="unidadNegocio">Unidad de negocio a consultar</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<TAServicioDC> ObtenerServiciosPorUnidadNegocio(long idCentroServicios, string unidadNegocio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                string diaActual = ((int)(DateTime.Now.DayOfWeek)).ToString();
                int horaActual = DateTime.Now.Hour;
                int minutoAcutal = DateTime.Now.Minute;

                // Además de validar servicios activos determiina si tiene horario configurado el día y hora en que se realiza la transacción
                List<CentroServicioServicio_VPUA> servicios = contexto.CentroServicioServicio_VPUA
                    .Where(servicio =>
                          servicio.CES_IdCentroServicios == idCentroServicios &&
                          servicio.CES_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                          servicio.CSS_FechaInicioVenta <= DateTime.Now &&
                          servicio.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                          servicio.SER_IdUnidadNegocio == unidadNegocio &&
                          servicio.CSD_IdDia == diaActual &&
                          (servicio.CSD_HoraInicial.Hour < horaActual || servicio.CSD_HoraInicial.Hour == horaActual && servicio.CSD_HoraInicial.Minute <= minutoAcutal) &&
                          (servicio.CSD_HoraFinal.Hour > horaActual || (servicio.CSD_HoraFinal.Hour == horaActual && servicio.CSD_HoraFinal.Minute >= minutoAcutal))
                          )
                    .ToList();

                if (servicios != null && servicios.Count > 0)
                {
                    return servicios.ConvertAll<TAServicioDC>(
                                                                    servicio =>
                                                                        new TAServicioDC
                                                                        {
                                                                            IdServicio = servicio.SER_IdServicio,
                                                                            Nombre = servicio.SER_Nombre,
                                                                            IdConceptoCaja = servicio.SER_IdConceptoCaja,
                                                                            PesoMinimo = servicio.SME_PesoMínimo,
                                                                            PesoMaximo = servicio.SME_PesoMaximo,
                                                                            IdUnidadNegocio = servicio.SER_IdUnidadNegocio
                                                                        });
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de servicios y retorna  la lista de servicios habilitados
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <param name="unidadNegocio">Unidad de negocio a consultar</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<TAServicioDC> ObtenerServiciosPorUnidadesDeNegocio(long idCentroServicios, string unidadNegocioMensajeria, string unidadNegocioCarga, int idListaPrecios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                List<paObtenerServActPorCSLisPrecio_Result> servicios = contexto.paObtenerServActPorCSLisPrecio(idCentroServicios, idListaPrecios, unidadNegocioMensajeria, unidadNegocioCarga).ToList();

                if (servicios != null && servicios.Count > 0)
                {
                    return servicios.ConvertAll<TAServicioDC>(
                                                                    servicio =>
                                                                        new TAServicioDC
                                                                        {
                                                                            IdServicio = servicio.SER_IdServicio,
                                                                            Nombre = servicio.SER_Nombre,
                                                                            IdConceptoCaja = servicio.SER_IdConceptoCaja,
                                                                            PesoMinimo = servicio.SME_PesoMínimo,
                                                                            PesoMaximo = servicio.SME_PesoMaximo,
                                                                            IdUnidadNegocio = servicio.SER_IdUnidadNegocio,

                                                                            //UnidadNegocio = servicio.UNE_Nombre
                                                                        });
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Valida que una agencia pueda realizar venta de servicios y retorna  la lista de servicios habilitados
        /// </summary>
        /// <param name="idCentroServicios">Identificador del centro de servicios</param>
        /// <param name="unidadNegocio">Unidad de negocio a consultar</param>
        /// <returns>Lista de servicios ofrecidos</returns>
        public IEnumerable<TAServicioDC> ObtenerServiciosPorUnidadesDeNegocioSinValidacionHorario(long idCentroServicios, string unidadNegocioMensajeria, string unidadNegocioCarga, int idListaPrecios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                // Además de validar servicios activos determiina si tiene horario configurado el día y hora en que se realiza la transacción
                List<paObtenerServActPorCSLisPrecioSinHora_Result> servicios = contexto.paObtenerServActPorCSLisPrecioSinHora(idCentroServicios, idListaPrecios, unidadNegocioMensajeria, unidadNegocioCarga).ToList();

                if (servicios != null && servicios.Count > 0)
                {
                    return servicios.ConvertAll<TAServicioDC>(
                                                                    servicio =>
                                                                        new TAServicioDC
                                                                        {
                                                                            IdServicio = servicio.SER_IdServicio,
                                                                            Nombre = servicio.SER_Nombre,
                                                                            IdConceptoCaja = servicio.SER_IdConceptoCaja,
                                                                            PesoMinimo = servicio.SME_PesoMínimo,
                                                                            PesoMaximo = servicio.SME_PesoMaximo,
                                                                            IdUnidadNegocio = servicio.SER_IdUnidadNegocio
                                                                        });
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CENTRO_SERVICIO_SIN_SERVICIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var infoAgencia = contexto.paObtInfoAgenciasTrayecto_PUA(localidadOrigen.IdLocalidad, validacion.CodigoPostalDestino).FirstOrDefault();
                if (infoAgencia == null || (infoAgencia != null && !infoAgencia.CES_IdCentroServiciosOri.HasValue))
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                }
                else
                {
                    validacion.PesoMaximoTrayectoOrigen = infoAgencia.CES_PesoMaximoOri.Value;
                    validacion.IdCentroServiciosOrigen = infoAgencia.CES_IdCentroServiciosOri.Value;
                    validacion.NombreCentroServiciosOrigen = infoAgencia.CES_NombreOri;
                    validacion.VolumenMaximoOrigen = infoAgencia.CES_VolumenMaximoOri.Value;
                    validacion.PesoMaximoTrayectoDestino = infoAgencia.CES_PesoMaximoOri.Value;
                    validacion.IdCentroServiciosDestino = infoAgencia.CES_IdCentroServiciosDes.Value;
                    validacion.NombreCentroServiciosDestino = infoAgencia.CES_NombreDes;
                    validacion.VolumenMaximoDestino = infoAgencia.CES_VolumenMaximoDes.Value;
                    validacion.DestinoAdmiteFormaPagoAlCobro = false;
                    validacion.DireccionCentroServiciosDestino = infoAgencia.CES_DireccionDes;
                    validacion.TelefonoCentroServiciosDestino = infoAgencia.CES_Telefono1Des;
                }

                //Agencia_VPUA agenciaOrigen = new Agencia_VPUA();
                //if (localidadOrigen != null)
                //{
                //    agenciaOrigen = contexto.Agencia_VPUA.FirstOrDefault(a => a.LOC_IdLocalidad == localidadOrigen.IdLocalidad);
                //    if (agenciaOrigen == null)
                //    {
                //        // Si no tiene agencia en su municipio / ciudad entonces debe ver si lo administra otro centro de servicio
                //        MunicipioCentroLogistico_PUA agenciaEncargada = contexto.MunicipioCentroLogistico_PUA.FirstOrDefault(m => m.MCL_IdLocalidad == localidadOrigen.IdLocalidad);
                //        if (agenciaEncargada != null)
                //        {
                //            agenciaOrigen = contexto.Agencia_VPUA.FirstOrDefault(agencia => agencia.AGE_IdAgencia == agenciaEncargada.MCL_IdCentroLogistico && agencia.CES_Estado == ConstantesFramework.ESTADO_ACTIVO);
                //        }
                //        else
                //        {
                //            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                //        }
                //    }
                //}

                //validacion.PesoMaximoTrayectoOrigen = agenciaOrigen.CES_PesoMaximo;
                //validacion.PesoMaximoTrayectoDestino = agenciaOrigen.CES_PesoMaximo;
                //validacion.IdCentroServiciosOrigen = agenciaOrigen.CES_IdCentroServicios;
                //validacion.IdCentroServiciosDestino = agenciaOrigen.CES_IdCentroServicios;
                //validacion.NombreCentroServiciosOrigen = agenciaOrigen.CES_Nombre;
                //validacion.NombreCentroServiciosDestino = agenciaOrigen.CES_Nombre;
                //validacion.VolumenMaximoOrigen = agenciaOrigen.CES_VolumenMaximo;
                //validacion.VolumenMaximoDestino = agenciaOrigen.CES_VolumenMaximo;
                //validacion.DestinoAdmiteFormaPagoAlCobro = false;
                //validacion.DireccionCentroServiciosDestino = agenciaOrigen.CES_Direccion;
                //validacion.TelefonoCentroServiciosDestino = agenciaOrigen.CES_Telefono1;
            }
        }

        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Ciudad o municipio de destino del envío</param>
        /// <param name="validacion">Validación del trayecto</param>
        /// <param name="idCentroServicio">Id del centro de servicios de origen de la transacción</param>
        /// <param name="localidadOrigen">si no se tiene el id centro de servicio origen el metodo lo busca a través de la localidad original</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var infoAgencia = contexto.paObtInfoAgenciasTrayecto_PUA(localidadOrigen.IdLocalidad, localidadDestino.IdLocalidad).FirstOrDefault();
                if (infoAgencia == null || (infoAgencia != null && !infoAgencia.CES_IdCentroServiciosOri.HasValue))
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                }
                else
                {
                    validacion.PesoMaximoTrayectoOrigen = infoAgencia.CES_PesoMaximoOri.Value;
                    validacion.IdCentroServiciosOrigen = infoAgencia.CES_IdCentroServiciosOri.Value;
                    validacion.NombreCentroServiciosOrigen = infoAgencia.CES_NombreOri;
                    validacion.VolumenMaximoOrigen = infoAgencia.CES_VolumenMaximoOri.Value;
                }
                if (infoAgencia != null && !infoAgencia.CES_IdCentroServiciosDes.HasValue)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                }
                else
                {
                    validacion.PesoMaximoTrayectoDestino = infoAgencia.CES_PesoMaximoDes.Value;
                    validacion.VolumenMaximoDestino = infoAgencia.CES_VolumenMaximoDes.Value;
                    validacion.IdCentroServiciosDestino = infoAgencia.CES_IdCentroServiciosDes.Value;
                    validacion.DestinoAdmiteFormaPagoAlCobro = infoAgencia.CES_AdmiteFormaPagoAlCobroDes.Value;
                    validacion.NombreCentroServiciosDestino = infoAgencia.CES_NombreDes;
                    validacion.CodigoPostalDestino = infoAgencia.LOC_CodigoPostalDes;
                    validacion.DireccionCentroServiciosDestino = infoAgencia.CES_DireccionDes;
                    validacion.TelefonoCentroServiciosDestino = infoAgencia.CES_Telefono1Des;
                }
            }
        }

        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Ciudad o municipio de destino del envío</param>
        /// <param name="validacion">Validación del trayecto</param>
        /// <param name="idCentroServicio">Id del centro de servicios de origen de la transacción</param>
        /// <param name="localidadOrigen">si no se tiene el id centro de servicio origen el metodo lo busca a través de la localidad original</param>
        public void ObtenerInformacionValidacionTrayectoAdo(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            using (var sqlconn = new SqlConnection(conexionString))
            {
                sqlconn.Open();
                var cmd = new SqlCommand("paObtInfoAgenciasTrayecto_PUA ", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@localidadOrigen", localidadOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@localidadDestino", localidadDestino.IdLocalidad);
                var resul = cmd.ExecuteReader();

                if (resul.HasRows)
                {
                    if (resul.Read())
                    {
                        if (DBNull.Value.Equals(resul["CES_IdCentroServiciosOri"]))

                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                        }
                        else
                        {
                            validacion.PesoMaximoTrayectoOrigen = Convert.ToDecimal(resul["CES_PesoMaximoOri"]);
                            validacion.IdCentroServiciosOrigen = Convert.ToInt64(resul["CES_IdCentroServiciosOri"]);
                            validacion.NombreCentroServiciosOrigen = Convert.ToString(resul["CES_IdCentroServiciosOri"]);
                            validacion.VolumenMaximoOrigen = Convert.ToDecimal(resul["CES_IdCentroServiciosOri"]);
                        }

                        if (DBNull.Value.Equals(resul["CES_IdCentroServiciosDes"]))

                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                        }
                        else
                        {
                            validacion.PesoMaximoTrayectoDestino = Convert.ToDecimal(resul["CES_PesoMaximoDes"]);
                            validacion.VolumenMaximoDestino = Convert.ToDecimal(resul["CES_VolumenMaximoDes"]);
                            validacion.IdCentroServiciosDestino = Convert.ToInt64(resul["CES_IdCentroServiciosDes"]);
                            validacion.DestinoAdmiteFormaPagoAlCobro = Convert.ToBoolean(resul["CES_AdmiteFormaPagoAlCobroDes"]);
                            validacion.NombreCentroServiciosDestino = Convert.ToString(resul["CES_NombreDes"]);
                            validacion.CodigoPostalDestino = Convert.ToString(resul["LOC_CodigoPostalDes"]);
                            validacion.DireccionCentroServiciosDestino = Convert.ToString(resul["CES_DireccionDes"]);
                            validacion.TelefonoCentroServiciosDestino = Convert.ToString(resul["CES_Telefono1Des"]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza la información de validación de un trayecto establecido desde una sucursal de un cliente
        /// </summary>
        /// <param name="localidadDestino">Localidad de destino del envío</param>
        /// <param name="validacion">Contiene información con los reusltados de las validaciones relacionadas con el trayecto</param>
        /// <param name="idCliente">Identificador del cliente que ingresa</param>
        /// <param name="idSucursal">Identificador de la sucursal</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, int idCliente, int idSucursal)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                // Obtener peso agencia origen transacción
                SucursalesAgencia_VCLI agenciaSucursal = contexto.SucursalesAgencia_VCLI.FirstOrDefault(s => s.SUC_IdSucursal == idSucursal && s.SUC_ClienteCredito == idCliente && s.CES_Estado == ConstantesFramework.ESTADO_ACTIVO);
                if (agenciaSucursal != null)
                {
                    validacion.PesoMaximoTrayectoOrigen = agenciaSucursal.CES_PesoMaximo;
                    validacion.IdCentroServiciosOrigen = agenciaSucursal.SUC_AgenciaEncargada;
                    validacion.NombreCentroServiciosOrigen = agenciaSucursal.CES_Nombre;
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                }

                // Obtener información de la agencia destino, el destino debe ser obligatoriamente una agencia, de lo contrario no se puede hacer envío a dicha localidad
                Agencia_VPUA agenciaDestino = contexto.Agencia_VPUA.FirstOrDefault(a => a.LOC_IdLocalidad == localidadDestino.IdLocalidad && a.CES_Estado == ConstantesFramework.ESTADO_ACTIVO);

                if (agenciaDestino == null)
                {
                    // Si no tiene agencia en su municipio / ciudad entonces debe ver si lo administra otro centro de servicio
                    MunicipioCentroLogistico_PUA agenciaEncargada = contexto.MunicipioCentroLogistico_PUA.FirstOrDefault(m => m.MCL_IdLocalidad == localidadDestino.IdLocalidad);
                    if (agenciaEncargada != null)
                    {
                        agenciaDestino = contexto.Agencia_VPUA.FirstOrDefault(a => a.AGE_IdAgencia == agenciaEncargada.MCL_IdCentroLogistico);
                        if (agenciaDestino == null)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                        }
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                    }
                }

                validacion.PesoMaximoTrayectoDestino = agenciaDestino.CES_PesoMaximo;

                //validacion.IdCentroServiciosDestino = agenciaDestino.AGE_IdCentroLogistico;
                // TODO:ID Se ajusta el Id del Centro de Servicio Destino, estaba quedando mal grabado
                validacion.IdCentroServiciosDestino = agenciaDestino.CES_IdCentroServicios;

                validacion.DestinoAdmiteFormaPagoAlCobro = agenciaDestino.CES_AdmiteFormaPagoAlCobro;
                validacion.NombreCentroServiciosDestino = agenciaDestino.CES_Nombre;
                validacion.CodigoPostalDestino = agenciaDestino.LOC_CodigoPostal;
                validacion.TelefonoCentroServiciosDestino = agenciaDestino.CES_Telefono1;
                validacion.DireccionCentroServiciosDestino = agenciaDestino.CES_Direccion;
            }
        }
        /// <summary>
        /// Procedimiento para consulta las direcciones de los puntos disponibles para RECLAME EN OFICINA
        /// </summary>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public List<string> ObtenerDireccionesPuntosSegunUbicacionDestino(int idLocalidadDestinatario)
        {
            List<string> res = new List<string>();
            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                SqlCommand com = new SqlCommand("paConsultarDireccionesLocalidadDestinatario", cn);
                com.Parameters.AddWithValue("@idLocalidadDestinatario", idLocalidadDestinatario);
                com.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cn.Close();
                List<string> lstDirecciones = dt.AsEnumerable().ToList().ConvertAll<string>(r =>
                {
                    //string dir = (r["CES_Tipo"].ToString() + "/" + r["CES_Direccion"].ToString() + "/" + r["CES_Barrio"].ToString());//TODO: Jeisson: agragar tipo y barrio a la lista
                    string dir = (r["CES_Tipo"].ToString() + "/" + r["CES_Direccion"].ToString() + "/");//TODO: Jeisson: agragar tipo y barrio a la lista
                    return dir;
                });

                return lstDirecciones;
            }
        }

        /// <summary>
        /// Procedimiento para consulta las direcciones de los puntos disponibles para RECLAME EN OFICINA
        /// </summary>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public List<PUCentroServicioApoyo> ObtenerPuntosREOSegunUbicacionDestino(int idLocalidadDestino)
        {
            List<PUCentroServicioApoyo> lstRes = new List<PUCentroServicioApoyo>();
            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                SqlCommand com = new SqlCommand("paConsultarDireccionesLocalidadDestinatario", cn);
                com.Parameters.AddWithValue("@idLocalidadDestinatario", idLocalidadDestino);
                com.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cn.Close();

                if (dt.Rows.Count > 0)
                    foreach (DataRow item in dt.Rows)
                    {
                        long idCenSer = long.Parse(item["CES_IdCentroServicios"].ToString());
                        string direccion = item["CES_Nombre"].ToString();
                        string barrio = item["CES_Barrio"].ToString();
                        string direccionCS = item["CES_Direccion"].ToString();

                        PUCentroServicioApoyo newPuntoRO = new PUCentroServicioApoyo();
                        newPuntoRO.IdCentroservicio = idCenSer;
                        newPuntoRO.NombreCentroServicio = direccion + "/ " + barrio;
                        newPuntoRO.DireccionCentroServicio = direccionCS;
                        lstRes.Add(newPuntoRO);
                    }
            }

            return lstRes;

        }


        #endregion Consultas Basicas

        #region Datos Bancarios

        /// <summary>
        /// Adiciona datos bancarios de un agente comercial (propietario)
        /// </summary>
        /// <param name="datosBanca">Objeto datos bancarios</param>
        public void AdicionarInfoBancaria(PUPropietarioBanco datosBanca)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConcesionarioBanco_PUA conBanco = new ConcesionarioBanco_PUA()
                {
                    COB_CreadoPor = ControllerContext.Current.Usuario,
                    COB_FechaGrabacion = DateTime.Now,
                    COB_IdBanco = datosBanca.IdBanco,
                    COB_Identificacion = datosBanca.Identificacion.Trim(),
                    COB_IdPropietario = datosBanca.IdPropietario,
                    COB_IdTipoCuenta = datosBanca.IdTipoCuenta,
                    COB_IdTipoIdentificacion = datosBanca.IdTipoIdentificacion,
                    COB_NumeroCuenta = datosBanca.NumeroCuenta.ToString(),
                    COB_TitularCuenta = datosBanca.TitularCuenta
                };

                contexto.ConcesionarioBanco_PUA.Add(conBanco);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Modifica la informacion bancaria de un agente comercial (propietario)
        /// </summary>
        /// <param name="datosBanca">Objeto datos bancarios</param>
        public void EditarInfoBancaria(PUPropietarioBanco datosBanca)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConcesionarioBanco_PUA concesionarioBanca = contexto.ConcesionarioBanco_PUA
                  .Where(obj => obj.COB_IdPropietario == datosBanca.IdPropietario)
                  .SingleOrDefault();

                if (concesionarioBanca == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                concesionarioBanca.COB_IdBanco = datosBanca.IdBanco;
                concesionarioBanca.COB_Identificacion = datosBanca.Identificacion.Trim();
                concesionarioBanca.COB_IdTipoCuenta = datosBanca.IdTipoCuenta;
                concesionarioBanca.COB_IdTipoIdentificacion = datosBanca.IdTipoIdentificacion;
                concesionarioBanca.COB_NumeroCuenta = datosBanca.NumeroCuenta.ToString();
                concesionarioBanca.COB_TitularCuenta = datosBanca.TitularCuenta;

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene la informacion bancaria de un agente comercial(propietario)
        /// </summary>
        /// <param name="idPropietario"></param>
        /// <returns></returns>
        public PUPropietarioBanco ObtenerDatosBancariosPropietario(int idPropietario)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConcesionarioBanco_PUA concesionarioBanca = contexto.ConcesionarioBanco_PUA.Include("Banco_PAR").Include("TipoCuentaBanco_PAR")
                  .Where(obj => obj.COB_IdPropietario == idPropietario)
                  .SingleOrDefault();

                if (concesionarioBanca == null)
                    return null;
                else
                    return new PUPropietarioBanco()
                    {
                        IdBanco = concesionarioBanca.COB_IdBanco,
                        TitularCuenta = concesionarioBanca.COB_TitularCuenta,
                        Identificacion = concesionarioBanca.COB_Identificacion,
                        NumeroCuenta = concesionarioBanca.COB_NumeroCuenta,//long.Parse(concesionarioBanca.COB_NumeroCuenta),
                        IdTipoIdentificacion = concesionarioBanca.COB_IdTipoIdentificacion,
                        IdTipoCuenta = concesionarioBanca.COB_IdTipoCuenta,
                        IdPropietario = concesionarioBanca.COB_IdPropietario,
                        NombreBanco = concesionarioBanca.Banco_PAR.BAN_Descripcion,
                        TipoCuenta = concesionarioBanca.TipoCuentaBanco_PAR.TCB_Descripcion
                    };
            }
        }

        #endregion Datos Bancarios

        #region Centro de servicios

        /// <summary>
        /// Obtiene los Centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        public IList<PUCentroServiciosDC> ObtenerCentrosServicioAgenciasPuntos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();

                var f = contexto.ConsultarCentrosServicosRacol_VPUA(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<PUCentroServiciosDC>(r =>
                  {
                      PUCentroServiciosDC propi = new PUCentroServiciosDC
                      {
                          Estado = r.CES_Estado,
                          IdCentroServicio = r.CES_IdCentroServicios,
                          Nombre = r.CES_Nombre,
                          Sistematizado = r.CES_Sistematizada,
                          Tipo = r.CES_Tipo,
                          TipoOriginal = r.CES_Tipo,
                          CiudadUbicacion = new PALocalidadDC()
                          {
                              Nombre = r.LOC_Nombre,
                              IdLocalidad = r.LOC_IdLocalidad
                          },
                          Direccion = r.CES_Direccion,
                          Telefono1 = r.CES_Telefono1,
                      };
                      return propi;
                  }).ToList();
                return f;
            }
        }

        /// <summary>
        /// Obtiene los Centros de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        public IList<PUCentroServiciosDC> ObtenerCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int? idPropietario, long? idCentroServicio = null)
        {
            List<PUCentroServiciosDC> centros = new List<PUCentroServiciosDC>();

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentrosServicios_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (idCentroServicio.HasValue && idCentroServicio.Value > 0 && filtro.Where(f => f.Key == "CES_IdCentroServicios").Count() <= 0)
                {
                    cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idCentroServicio.Value);
                }

                filtro.ToList().ForEach(f =>
                {
                    if (!string.IsNullOrWhiteSpace(f.Value))
                    {

                        cmd.Parameters.AddWithValue("@" + f.Key, f.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@" + f.Key, DBNull.Value);
                    }
                });

                if (idPropietario.HasValue && idPropietario.Value > 0)
                {
                    cmd.Parameters.AddWithValue("@CES_IdPropietario", idPropietario.Value);
                }

                cmd.Parameters.AddWithValue("@PageSize", registrosPorPagina);
                cmd.Parameters.AddWithValue("@PageNumber", indicePagina);
                if (!string.IsNullOrWhiteSpace(campoOrdenamiento))
                    cmd.Parameters.AddWithValue("@CampoOrdenamiento", campoOrdenamiento);

                cmd.Parameters.AddWithValue("@OrdenamientoAscendente", ordenamientoAscendente);

                SqlParameter paramOut = new SqlParameter("@totalRegistros", SqlDbType.Int);
                paramOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramOut);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                //dt.Load(cmd.ExecuteReader());
                conn.Close();

                if (dt.Rows.Count > 0)
                {
                    centros = dt.AsEnumerable().
                     ToList()
                     .ConvertAll<PUCentroServiciosDC>(r =>
                     {
                         PUCentroServiciosDC propi = new PUCentroServiciosDC();

                         propi.Fecha = r.Field<DateTime>("CES_FechaGrabacion");
                         propi.BaseInicialCaja = r.Field<decimal>("CES_BaseInicialCaja");
                         propi.Direccion = r.Field<string>("CES_Direccion");
                         propi.Barrio = r.Field<string>("CES_Barrio");
                         propi.Email = r.Field<string>("CES_Email");
                         propi.Fax = r.Field<string>("CES_Fax");
                         propi.IdMunicipio = r.Field<string>("CES_IdMunicipio");
                         propi.IdPropietario = r.Field<int>("CES_IdPropietario");
                         propi.IdentificacionPropietario = r.Field<string>("PEE_Identificacion_Propietario");
                         propi.NombrePropietario = r.Field<string>("CON_RazonSocial");
                         propi.IdRepresentanteLegalPropietario = r.Field<long>("CON_IdRepresentanteLegal");
                         propi.CreadoPor = r.Field<string>("CES_CreadoPor");

                         propi.CiudadUbicacion = new PALocalidadDC()
                         {
                             IdLocalidad = r.Field<string>("LOC_IdLocalidad"),
                             Nombre = r.Field<string>("NombreCompleto")
                         };


                         propi.IdTerritorial = r["REA_IdTerritorial"] == DBNull.Value ? 0 : r.Field<int>("REA_IdTerritorial");
                         propi.NombreMunicipio = r.Field<string>("LOC_Nombre");
                         propi.IdDepto = r.Field<string>("LOC_IdAncestroPrimerGrado");
                         propi.NombreDepto = r.Field<string>("LOC_NombrePrimero");
                         propi.IdPais = r.Field<string>("LOC_IdAncestroSegundoGrado");
                         propi.NombrePais = r.Field<string>("LOC_NombreSegundo");

                         propi.DigitoVerificacion = r.Field<string>("CES_DigitoVerificacion");
                         propi.IdTipoPropiedad = r.Field<short>("CES_IdTipoPropiedad");
                         propi.Estado = r.Field<string>("CES_Estado");
                         propi.EstadoBool = r.Field<string>("CES_Estado") == ConstantesFramework.ESTADO_ACTIVO ? true : false;
                         propi.IdCentroCostos = r.Field<string>("CES_IdCentroCostos");
                         propi.IdCentroServicio = r.Field<long>("CES_IdCentroServicios");
                         propi.IdPersonaResponsable = r.Field<long>("CES_IdPersonaResponsable");
                         propi.NombrePersonaResponsable = r.Field<string>("PEE_PrimerNombre_Responsable") + " " + r.Field<string>("PEE_PrimerApellido_Responsable");
                         propi.IdentificacionPersonaResponsable = r.Field<string>("PEE_Identificacion_Responsable");
                         propi.CelularPersonaResponsable = r.Field<string>("PEE_NumeroCelular_Responsable");
                         propi.IdZona = r.Field<string>("CES_IdZona");
                         propi.NombreZona = r.Field<string>("ZON_Descripcion");
                         propi.Latitud = r.Field<decimal?>("CES_Latitud");
                         propi.Longitud = r.Field<decimal?>("CES_Longitud");
                         propi.Nombre = r.Field<string>("CES_Nombre");
                         propi.Sistematizado = r.Field<bool>("CES_Sistematizada");
                         propi.PuntoPAM = r.Field<bool?>("CES_AplicaPAM");
                         propi.Telefono1 = r.Field<string>("CES_Telefono1");
                         propi.Telefono2 = r.Field<string>("CES_Telefono2");
                         propi.Tipo = r.Field<string>("CES_Tipo");
                         propi.TipoOriginal = r.Field<string>("CES_Tipo");
                         propi.AdmiteFormaPagoAlCobro = r.Field<bool>("CES_AdmiteFormaPagoAlCobro");
                         propi.PesoMaximo = r.Field<decimal>("CES_PesoMaximo");
                         propi.VendePrepago = r.Field<bool>("CES_VendePrepago");
                         propi.VolumenMaximo = r.Field<decimal>("CES_VolumenMaximo");

                         propi.TopeMaximoGiros = r.Field<decimal>("CES_TopeMaximoPorGiros");
                         propi.PagaGiros = r.Field<bool>("CES_PuedePagarGiros");
                         propi.RecibeGiros = r.Field<bool>("CES_PuedeRecibirGiros");
                         propi.IdClasificadorCanalVenta = r.Field<short>("CES_IdClasificadorCanalVenta");
                         propi.CodigoBodega = r.Field<string>("CES_CodigoBodega");

                         propi.CodigoSPN = r.Field<string>("CES_CodigoSPN");
                         propi.CodigoPostal = r.Field<string>("CES_CodigoPostal");

                         propi.NombreAMostrar = r.Field<string>("CES_NombreAMostrar");
                         propi.Codigo472 = r.Field<string>("CES_Codigo472");
                         propi.FechaApertura = r.Field<DateTime?>("CES_FechaApertura");
                         propi.FechaCierre = r.Field<DateTime?>("CES_FechaCierre");
                         propi.Biometrico = r.Field<bool>("CES_Biometrico");
                         propi.Recaudo = r.Field<bool>("CES_Recaudo");



                         int IdCasaMatriz = 0;
                         string DescripcionRacol = ".";
                         string CentroServAdministrador = "";
                         string TipoSubtipo = "";
                         string CodRacolExterno = "";
                         string tipoAgencia = ".";
                         long IdColRacolApoyo = 0;

                         if (r["AGE_IdTipoAgencia_ApoyoAgencia"] != DBNull.Value)
                         {
                             tipoAgencia = r.Field<string>("AGE_IdTipoAgencia_ApoyoAgencia");
                             TipoSubtipo = r.Field<string>("CES_Tipo");
                             if (r.Field<string>("AGE_IdTipoAgencia_ApoyoAgencia") == ConstantesFramework.TIPO_CENTRO_SERVICIO_COL)
                             {
                                 IdColRacolApoyo = r.Field<long>("CEL_IdRegionalAdm_ApoyoAgencia");
                             }
                             else
                             {
                                 IdColRacolApoyo = r.Field<long>("AGE_IdCentroLogistico_ApoyoAgencia");
                             }

                         }


                         if (r.Field<string>("CES_Tipo") == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
                         {

                             CentroServAdministrador = r.Field<string>("CES_Nombre_ApoyoPunto");
                             IdColRacolApoyo = r.Field<long>("CES_IdCentroServicios_ApoyoPunto");


                             TipoSubtipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO;

                             tipoAgencia = ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA;//se deja este tipo solo con el fin de cargarlo en cliente, pero no significa que se pueda cambiar de punto a agencia
                         }
                         else if (r.Field<string>("CES_Tipo") == ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
                         {
                             CentroServAdministrador = "Casa Matriz";

                             DescripcionRacol = r.Field<string>("REA_Descripcion");
                             CodRacolExterno = r.Field<string>("REA_CodigoExternoSucursal");
                             IdCasaMatriz = r.Field<short>("REA_IdCasaMatriz");
                             tipoAgencia = ConstantesFramework.TIPO_CENTRO_SERVICIO_COL;
                             TipoSubtipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL;
                         }
                         else
                         {
                             if (r["CES_IdCentroServicios_Racol"] != DBNull.Value)
                             {
                                 CentroServAdministrador = r.Field<string>("CES_Nombre_Racol");
                                 IdColRacolApoyo = r.Field<long>("CES_IdCentroServicios_Racol");
                             }
                         }

                         bool opera = false;
                         if (r["AGE_Operacional"] != DBNull.Value)
                         {
                             opera = r.Field<bool>("AGE_Operacional");
                         }
                         else
                             opera = false;


                         bool reclama;
                         if (r["PUS_ReclameEnOficina"] != DBNull.Value)
                         {
                             reclama = r.Field<bool>("PUS_ReclameEnOficina");
                         }
                         else
                             reclama = false;


                         if (r["CES_AplicaPAM_Clasificador"] != DBNull.Value)
                         {
                             propi.AplicaPAM = r.Field<bool>("CES_AplicaPAM_Clasificador");
                         }
                         else
                         {
                             propi.AplicaPAM = false;
                         }

                         if (r["CES_Biometrico"] != DBNull.Value)
                         {
                             propi.Biometrico = r.Field<bool>("CES_Biometrico");
                         }
                         else
                         {
                             propi.Biometrico = false;
                         }

                         if (r["CES_Recaudo"] != DBNull.Value)
                         {
                             propi.Recaudo = r.Field<bool>("CES_Recaudo");
                         }
                         else
                         {
                             propi.Recaudo = false;
                         }
                         propi.IdTipoAgencia = tipoAgencia;
                         propi.IdColRacolApoyo = IdColRacolApoyo;
                         propi.CentroServiciosAdministrador = CentroServAdministrador;
                         propi.DescripcionRacol = DescripcionRacol;
                         propi.CodRacolExterno = CodRacolExterno;
                         propi.TipoSubtipo = TipoSubtipo;
                         propi.IdCasaMatriz = IdCasaMatriz;
                         propi.ReclameEnOficina = reclama;
                         propi.Operacional = opera;
                         propi.HorariosCentroServicios = ObtenerHorariosCentroServicios(propi.IdCentroServicio);

                         //EMRL se llena campo para saber en frontal si existe la marca
                         if (propi.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA)
                         {
                             propi.MisPendientes = ConsultarMarcaMisPendietesCentroServicioAgencia(propi.IdCentroServicio);
                         }

                         return propi;

                     });
                }
                totalRegistros = Convert.ToInt32(paramOut.Value);
                return centros;
            }

        }

        /// <summary>
        /// Obtiene los Centros de servicio de acuerdo a una localidad
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <returns>Lista con los centros de servicio</returns>
        public IList<PUCentroServiciosDC> ObtenerCentrosServicioPorLocalidad(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, string IdLocalidad)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                filtro.Add("MRA_IdMunicipio", IdLocalidad.ToString());
                filtro.Add("CES_Estado", ConstantesFramework.ESTADO_ACTIVO);
                var f = contexto.ConsultarContainsAgenciaEncargadaMunicipio_VPUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>
                  (r =>
                  {
                      Localidades_VPAR localidad = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == r.CES_IdMunicipio).Single();
                      PUCentroServiciosDC propi = new PUCentroServiciosDC
                      {
                          Direccion = r.CES_Direccion,
                          Barrio = r.CES_Barrio,
                          Email = r.CES_Email,
                          Fax = r.CES_Fax,
                          IdMunicipio = r.CES_IdMunicipio,
                          IdPropietario = r.CES_IdPropietario,
                          NombreMunicipio = localidad.LOC_Nombre,
                          IdDepto = localidad.LOC_IdAncestroPrimerGrado,
                          NombreDepto = localidad.LOC_NombrePrimero,
                          IdPais = localidad.LOC_IdAncestroSegundoGrado,
                          NombrePais = localidad.LOC_NombreSegundo,
                          IdTipoPropiedad = r.CES_IdTipoPropiedad,
                          Estado = r.CES_Estado,
                          IdCentroCostos = r.CES_IdCentroCostos,
                          IdCentroServicio = r.AGE_IdAgencia,
                          IdPersonaResponsable = r.CES_IdPersonaResponsable,
                          IdZona = r.CES_IdZona,
                          Latitud = r.CES_Latitud,
                          Longitud = r.CES_Longitud,
                          Nombre = r.CES_Nombre,
                          Sistematizado = r.CES_Sistematizada,
                          Telefono1 = r.CES_Telefono1,
                          Telefono2 = r.CES_Telefono2,
                          Tipo = r.CES_Tipo,
                          IdRegionalAdministrativa = r.REA_IdRegionalAdm
                      };
                      return propi;
                  }).ToList();
                return f;
            }
        }

        /// <summary>
        /// Retorna la lista de centros de servicios activos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosActivos()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServiciosCentroSvcResponsable_VPUA
                  .Where(c => c.Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .OrderBy(o => o.Nombre)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>
                  (p =>
                    new PUCentroServiciosDC
                    {
                        IdCentroServicio = p.IdCentroServicios,
                        Nombre = p.Nombre,
                        infoResponsable = new PUAgenciaDeRacolDC
                        {
                            IdCentroServicio = p.IdResponsable,
                            NombreCentroServicio = p.NombreResponsable
                        },
                        Tipo = p.Tipo,
                        BaseInicialCaja = p.BaseInicial,
                        NombreCodigo = p.IdCentroServicios + "  " + p.Nombre
                    });
            }
        }

        /// <summary>
        /// Retorna la lista con todos los centros de servicios del sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicios()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicioServicio_VPUA.OrderBy(o => o.SER_Nombre)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>
                  (p =>
                    new PUCentroServiciosDC
                    {
                        IdCentroServicio = p.CES_IdCentroServicios,
                        Nombre = p.SER_Nombre
                    });
            }
        }

        public List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosXEstado(PAEnumEstados estado)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string est = estado.ToString();
                return contexto.CentroServicios_PUA.Include("Propietario_PUA").Include("Localidad_PAR").Include("Propietario_PUA.PersonaResponsableLegal_PAR").Include("Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR").
                    Where(e => e.CES_Estado == est).OrderBy(o => o.CES_Nombre)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>
                  (p =>
                  {
                      return new PUCentroServiciosDC
                      {
                          NombreCodigo = p.CES_IdCentroServicios.ToString() + "-" + p.CES_Nombre,
                          IdCentroServicio = p.CES_IdCentroServicios,
                          Nombre = p.CES_Nombre,
                          NombreMunicipio = p.Localidad_PAR.LOC_Nombre,
                          Direccion = p.CES_Direccion,
                          Tipo = p.CES_Tipo,
                          Telefono1 = p.CES_Telefono1,
                          NombrePropietario = p.Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR.PEE_PrimerNombre + " " + p.Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR.PEE_PrimerApellido + " " + p.Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR.PEE_SegundoApellido
                      };
                  });
            }
        }

        public List<PUCentroServiciosDC> ObtenerTodosCentrosServiciosNoInactivos()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.CentroServicios_PUA.Include("Propietario_PUA").Include("Localidad_PAR").Include("Propietario_PUA.PersonaResponsableLegal_PAR").Include("Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR").
                    Where(e => e.CES_Estado != "INA").OrderBy(o => o.CES_Nombre)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>
                  (p =>
                  {
                      return new PUCentroServiciosDC
                      {
                          NombreCodigo = p.CES_IdCentroServicios.ToString() + "-" + p.CES_Nombre,
                          IdCentroServicio = p.CES_IdCentroServicios,
                          Nombre = p.CES_Nombre,
                          IdMunicipio = p.CES_IdMunicipio,
                          NombreMunicipio = p.Localidad_PAR.LOC_Nombre,
                          Direccion = p.CES_Direccion,
                          Tipo = p.CES_Tipo,
                          Telefono1 = p.CES_Telefono1,
                          NombrePropietario = p.Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR.PEE_PrimerNombre + " " + p.Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR.PEE_PrimerApellido + " " + p.Propietario_PUA.PersonaResponsableLegal_PAR.PersonaExterna_PAR.PEE_SegundoApellido
                      };
                  });
            }
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// y puntos de atención de los Racoles
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicios(long idRacol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentrosServicosRacol_VPUA.Where(cent => cent.REA_IdRegionalAdm == idRacol)
                  .OrderBy(o => o.CES_Nombre)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>(r => new PUCentroServiciosDC()
                  {
                      IdCentroServicio = r.CES_IdCentroServicios,
                      Nombre = r.CES_Nombre,
                      Tipo = r.CES_Tipo,
                      IdColRacolApoyo = r.REA_IdRegionalAdm,
                      CiudadUbicacion = new PALocalidadDC()
                      {
                          IdLocalidad = r.LOC_IdLocalidad,
                          Nombre = r.LOC_Nombre,
                          NombreCorto = r.LOC_NombreCorto
                      },
                      Telefono1 = r.CES_Telefono1,
                      Direccion = r.CES_Direccion,
                      DescripcionRacol = r.REA_Descripcion,
                      Sistematizado = r.CES_Sistematizada,
                      IdCentroCostos = r.CES_IdCentroCostos,
                      CodigoBodega = r.CES_CodigoBodega,
                      NombreMunicipio = r.LOC_Nombre,
                      TopeMaximoGiros = r.CES_TopeMaximoPorGiros,
                      TopeMaximoPagos = r.CES_TopeMaximoPorPagos,
                      PagaGiros = r.CES_PuedePagarGiros,
                      Estado = r.CES_Estado,
                      RecibeGiros = r.CES_PuedeRecibirGiros,
                      ClasificacionPorIngresos = new PUClasificacionPorIngresosDC()
                      {
                          IdClasificacion = r.CES_ClasGirosPorIngresos
                      }
                  });
            }
        }


        /// <summary>
        /// Obtiene los Centros de Servicios Activos e Inactivos de una Racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosTodos(long idRacol)
        {
            List<PUCentroServiciosDC> centTodos = new List<PUCentroServiciosDC>();

            using (SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCentrosServiciosTodos_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRacol", idRacol);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    PUCentroServiciosDC centServ = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Convert.ToInt64(reader["CES_IdCentroServicios"]),
                        Nombre = reader["CES_Nombre"].ToString(),
                        Tipo = reader["CES_Tipo"].ToString(),
                        IdColRacolApoyo = Convert.ToInt64(reader["REA_IdRegionalAdm"]),
                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = reader["LOC_IdLocalidad"].ToString(),
                            Nombre = reader["LOC_Nombre"].ToString(),
                            NombreCorto = reader["LOC_NombreCorto"].ToString()
                        },
                        Telefono1 = reader["CES_Telefono1"].ToString(),
                        Direccion = reader["CES_Direccion"].ToString(),
                        DescripcionRacol = reader["REA_Descripcion"].ToString(),
                        Sistematizado = Convert.ToBoolean(reader["CES_Sistematizada"]),
                        IdCentroCostos = reader["CES_IdCentroCostos"].ToString(),
                        CodigoBodega = reader["CES_CodigoBodega"].ToString(),
                        NombreMunicipio = reader["LOC_Nombre"].ToString(),
                        TopeMaximoGiros = Convert.ToDecimal(reader["CES_TopeMaximoPorGiros"]),
                        TopeMaximoPagos = Convert.ToDecimal(reader["CES_TopeMaximoPorPagos"]),
                        PagaGiros = Convert.ToBoolean(reader["CES_PuedePagarGiros"]),
                        Estado = reader["CES_Estado"].ToString(),
                        RecibeGiros = Convert.ToBoolean(reader["CES_PuedeRecibirGiros"]),
                        ClasificacionPorIngresos = new PUClasificacionPorIngresosDC()
                        {
                            IdClasificacion = reader["CES_ClasGirosPorIngresos"].ToString()
                        }
                    };
                    centTodos.Add(centServ);
                }
            }
            return centTodos;
        }


        /// <summary>
        /// Método para obtener las agencias de un COL que sean de tipo ARO, mas los puntos de la ciudad del COL
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServiciosAsignacionTulas(long idCol)
        {
            List<PUCentroServiciosDC> centroServicios = new List<PUCentroServiciosDC>();
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var agencias = contexto.Agencia_VPUA.Where(age => age.AGE_IdCentroLogistico == idCol && age.AGE_IdTipoAgencia == "ARO").ToList();
                if (agencias != null && agencias.Any())
                {
                    centroServicios = agencias.ConvertAll<PUCentroServiciosDC>(r => new PUCentroServiciosDC()
                    {
                        IdCentroServicio = r.CES_IdCentroServicios,
                        Nombre = String.Concat(r.AGE_IdAgencia, " - ", r.CES_Nombre),
                        Tipo = r.CES_Tipo,
                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = r.LOC_IdLocalidad,
                            Nombre = r.LOC_Nombre,
                            NombreCorto = r.LOC_NombreCorto
                        },
                        Telefono1 = r.CES_Telefono1,
                        Direccion = r.CES_Direccion,
                        Sistematizado = r.CES_Sistematizada,
                        NombreMunicipio = r.LOC_Nombre,
                    });
                }
                var puntos = contexto.PuntosDeAgencia_VPUA.Where(punt => punt.PUS_IdAgencia == idCol).ToList();
                if (puntos != null && puntos.Any())
                {
                    List<PUCentroServiciosDC> puntosAgencia =
                        puntos.ConvertAll<PUCentroServiciosDC>(r => new PUCentroServiciosDC()
                        {
                            IdCentroServicio = r.CES_IdCentroServicios,
                            Nombre = String.Concat(r.CES_IdCentroServicios, " - ", r.CES_Nombre),
                            Tipo = r.CES_Tipo,
                            CiudadUbicacion = new PALocalidadDC()
                            {
                                IdLocalidad = r.LOC_IdLocalidad,
                                Nombre = r.LOC_Nombre,
                                NombreCorto = r.LOC_NombreCorto
                            },
                            Telefono1 = r.CES_Telefono1,
                            Direccion = r.CES_Direccion,
                            Sistematizado = r.CES_Sistematizada,
                            NombreMunicipio = r.LOC_Nombre,
                        });
                    centroServicios = centroServicios.Concat(puntosAgencia).ToList();
                }
            }
            return centroServicios;
        }

        /// <summary>
        /// Obtiene los centros de servicio que tienen giros pendientes a transmitir
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentroserviciosGirosATransmitir(long idRacol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<PUCentroServiciosDC> data = contexto.paObtenerCentroServGirTran_GIR(idRacol)
                 .OrderByDescending(r => r.Cantidad)
                 .ToList()
                 .ConvertAll<PUCentroServiciosDC>(r => new PUCentroServiciosDC()
                 {
                     IdCentroServicio = r.CES_IdCentroServicios,
                     Nombre = r.CES_Nombre,
                     Tipo = r.CES_Tipo,
                     IdColRacolApoyo = r.REA_IdRegionalAdm,
                     DescripcionRacol = r.REA_Descripcion,
                     Sistematizado = r.CES_Sistematizada,
                     NombreMunicipio = r.LOC_Nombre,
                     Cantidad = r.Cantidad.Value,
                     EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                 });

                return data;
            }
        }



        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {

            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentrosServicios_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idCentroServicio);
                cmd.Parameters.AddWithValue("@totalRegistros", 1);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var centroServicio = dt.AsEnumerable().FirstOrDefault();

                if (centroServicio != null)
                {
                    PUCentroServiciosDC centro = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = centroServicio.Field<long>("CES_IdCentroServicios"),
                        Tipo = centroServicio.Field<string>("CES_Tipo"),
                        Nombre = centroServicio.Field<string>("CES_Nombre"),
                        BaseInicialCaja = centroServicio.Field<decimal>("CES_BaseInicialCaja"),
                        Direccion = centroServicio.Field<string>("CES_Direccion"),
                        IdMunicipio = centroServicio.Field<string>("CES_IdMunicipio"),
                        RecibeGiros = centroServicio.Field<bool>("CES_PuedeRecibirGiros"),
                        VendePrepago = centroServicio.Field<bool>("CES_VendePrepago"),
                        IdTipoPropiedad = Convert.ToInt32(centroServicio["CES_IdTipoPropiedad"]),
                        IdCentroCostos = centroServicio.Field<string>("CES_IdCentroCostos"),
                        CodigoBodega = centroServicio.Field<string>("CES_CodigoBodega"),
                        Telefono1 = centroServicio.Field<string>("CES_Telefono1"),
                        Telefono2 = centroServicio["CES_Telefono2"] != DBNull.Value ? centroServicio.Field<string>("CES_Telefono2") : "",
                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = centroServicio.Field<string>("LOC_IdLocalidad"),
                            Nombre = centroServicio.Field<string>("LOC_Nombre"),
                            //CodigoPostal = centroServicio.Field<long>("LOC_CodigoPostal")
                        }
                    };

                    if (centroServicio["LOC_IdAncestroTercerGrado"] != DBNull.Value)
                    {
                        centro.IdPais = centroServicio.Field<string>("LOC_IdAncestroTercerGrado");
                        centro.NombrePais = centroServicio.Field<string>("LOC_NombreTercero");
                    }
                    else if (centroServicio["LOC_IdAncestroSegundoGrado"] != DBNull.Value)
                    {
                        centro.IdPais = centroServicio.Field<string>("LOC_IdAncestroSegundoGrado");
                        centro.NombrePais = centroServicio.Field<string>("LOC_NombreSegundo");
                    }
                    else if (centroServicio["LOC_IdAncestroPrimerGrado"] != DBNull.Value)
                    {
                        centro.IdPais = centroServicio.Field<string>("LOC_IdAncestroPrimerGrado");
                        centro.NombrePais = centroServicio.Field<string>("LOC_NombrePrimero");
                    }

                    return centro;
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                      EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_EXISTE.ToString(),
                      MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_EXISTE)));
                }
            }
        }


        /// <summary>
        /// Obtiene la Agencia responsable del Punto
        /// </summary>
        /// <param name="idPuntoServicio">el id punto servicio.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerAgenciaResponsable(long idPuntoServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PuntosDeAgencia_VPUA punto = contexto.PuntosDeAgencia_VPUA
                    .FirstOrDefault(pun => pun.CES_IdCentroServicios == idPuntoServicio && pun.CES_Estado == ConstantesFramework.ESTADO_ACTIVO);
                if (punto != null)
                {
                    return new PUAgenciaDeRacolDC()
                    {
                        IdCentroServicio = punto.CES_IdCentroServicios,
                        NombreCentroServicio = punto.CES_Nombre,
                        IdResponsable = punto.PUS_IdAgencia,
                        NombreResponsable = punto.NombreAgencia,
                        NombreCiudadResponsable = punto.NombreCiudadAgencia,
                        TipoCentroServicio = punto.CES_Tipo,
                    };
                }
                else
                {
                    Agencia_VPUA agencia = contexto.Agencia_VPUA
                        .FirstOrDefault(age => age.AGE_IdAgencia == idPuntoServicio);

                    if (agencia != null)
                    {
                        return new PUAgenciaDeRacolDC()
                        {
                            IdCentroServicio = agencia.CES_IdCentroServicios,
                            NombreCentroServicio = agencia.CES_Nombre,
                            IdResponsable = agencia.CES_IdCentroServicios,
                            NombreResponsable = agencia.CES_Nombre,
                            NombreCiudadResponsable = agencia.LOC_Nombre,
                            TipoCentroServicio = agencia.CES_Tipo
                        };
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                         EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE.ToString(),
                         MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE)));
                    }
                }
            };
        }

        /// <summary>
        /// Obtiene el centro logistico de un punto
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgenciaColCentroServicios(long idPuntoServicio)
        {

            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciaColCentroServicios_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idPuntoServicio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var Col = dt.AsEnumerable().FirstOrDefault();
                if (Col != null)
                {
                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Col.Field<long>("CEL_IdCentroLogistico"),
                        IdMunicipio = Col.Field<string>("CES_IdMunicipioCol"),
                        IdColRacolApoyo = Col.Field<long?>("CEL_IdCentroLogistico"),
                        Nombre = Col.Field<string>("CES_NombreCol"),
                        NombreMunicipio = Col.Field<string>("LOC_NombreCol"),
                        Direccion = Col.Field<string>("CES_DireccionCol"),
                        Telefono1 = Col.Field<string>("CES_Telefono1"),
                    };
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                     EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE.ToString(),
                     MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE)));



            }
        }

        /// <summary>
        /// Obtiene el centro logistico de una agencia
        /// </summary>
        /// <param name="idPuntoServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerAgenciaColAgencia(long idAgencia)
        {

            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerColdeAgencia_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idAgencia);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var Col = dt.AsEnumerable().FirstOrDefault();

                if (Col != null)
                {
                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Col.Field<long>("AGE_IdCentroLogistico"),
                        Nombre = Col.Field<string>("CES_Nombre"),
                        IdColRacolApoyo = Col.Field<long?>("AGE_IdCentroLogistico"),
                        IdMunicipio = Col.Field<string>("LOC_IdLocalidad"),
                        infoResponsable = new PUAgenciaDeRacolDC()
                        {
                            IdCentroServicio = Col.Field<long>("CES_IdCentroServicios"),
                            NombreCentroServicio = Col.Field<string>("CES_Nombre"),
                            IdResponsable = Col.Field<long>("AGE_IdCentroLogistico"),
                            NombreResponsable = Col.Field<string>("AGE_NombreCentroLogistico"),
                        }
                    };
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                     EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE.ToString(),
                     MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE)));

            }
        }



        /// <summary>
        /// Método para obtener el propietario de una bodega
        /// </summary>
        /// <param name="idBodega"></param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObteneRacolPropietarioBodega(long idBodega)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPropietarioBodega_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idBodega);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var Propietario = dt.AsEnumerable().FirstOrDefault();

                if (Propietario != null)
                {
                    return new PUAgenciaDeRacolDC()
                    {
                        IdCentroServicio = Propietario.Field<long>("idBodega"),
                        NombreCentroServicio = Propietario.Field<string>("nombreBodega"),
                        IdResponsable = Propietario.Field<long>("CES_IdCentroServicios"),
                        NombreResponsable = Propietario.Field<string>("CES_Nombre"),
                        IdCiudadResponsable = Propietario.Field<string>("municipioBodega")
                    };
                }
                else
                {

                    return null;
                }
            }
        }


        /// <summary>
        /// Método para obtener el propietario de una bodega
        /// </summary>
        /// <param name="idBodega"></param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObteneColPropietarioBodega(long idBodega)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerColPropietarioBodega_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idBodega);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var Propietario = dt.AsEnumerable().FirstOrDefault();

                if (Propietario != null)
                {
                    return new PUAgenciaDeRacolDC()
                    {
                        IdCentroServicio = Propietario.Field<long>("idBodega"),
                        NombreCentroServicio = Propietario.Field<string>("nombreBodega"),
                        IdResponsable = Propietario.Field<long>("CES_IdCentroServicios"),
                        NombreResponsable = Propietario.Field<string>("CES_Nombre"),
                        IdCiudadResponsable = Propietario.Field<string>("municipioBodega")
                    };
                }
                else
                {

                    return null;
                }
            }
        }
        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgencia(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var PuntosDeAgencia = contexto.PuntosDeAgencia_VPUA.Where(l => l.PUS_IdAgencia == idCentroServicio);
                if (PuntosDeAgencia != null)
                {
                    return PuntosDeAgencia.ToList().ConvertAll<PUCentroServiciosDC>(Puntos => new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Puntos.CES_IdCentroServicios,
                        Nombre = Puntos.CES_Nombre,
                        Estado = Puntos.CES_Estado,
                    }).ToList();
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                     EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS.ToString(),
                     MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS)));
                }
            }
        }

        /// <summary>
        /// Obtiene el Racol responsable de la Agencia.
        /// </summary>
        /// <param name="idAgencia">el id agencia.</param>
        /// <returns></returns>
        public PUAgenciaDeRacolDC ObtenerRacolResponsable(long idAgencia)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentrosServicosRacol_VPUA agencia = contexto.CentrosServicosRacol_VPUA.FirstOrDefault(pun => pun.CES_IdCentroServicios == idAgencia);
                if (agencia != null)
                {
                    return new PUAgenciaDeRacolDC()
                    {
                        IdCentroServicio = agencia.CES_IdCentroServicios,
                        NombreCentroServicio = agencia.CES_Nombre,
                        IdResponsable = agencia.REA_IdRegionalAdm,
                        NombreResponsable = agencia.REA_Descripcion,
                        CentroDeCostosCentroServ = agencia.CES_IdCentroCostos,
                        CentroDeCostosResponsable = agencia.CentroCostosRacol,
                        CodResponsableERP = agencia.CodRacolERP,
                        BodegaCentroServ = agencia.CES_CodigoBodega
                    };
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                     EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE.ToString(),
                     MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_RESPONSABLE)));
                }
            };
        }

        /// <summary>
        /// Metodo para consultar las localidades donde existen centros logisticos
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerLocalidadesCol()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.LocalidadesCtroLogistico_VPUA
                .Where(r => r.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                .OrderBy(o => o.CES_Nombre)
                .ToList()
                .ConvertAll<LILocalidadColDC>(r => new LILocalidadColDC()
                {
                    NombreLocalidad = r.LOC_Nombre,
                    IdLocalidad = r.LOC_IdLocalidad,
                    IdCentroServicio = r.CEL_IdCentroLogistico,
                    NombreCentroServicio = r.CES_Nombre,
                    Direccion = r.CES_Direccion,
                    Telefono = r.CES_Telefono1,
                });
            }
        }

        /// <summary>
        /// Metodo para consultar las agencias que dependen de un COL
        /// </summary>
        /// <returns></returns>
        public IList<LILocalidadColDC> ObtenerAgenciasCol(long idCol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IList<LILocalidadColDC> agenciasCol = contexto.AgenciasDeCentroLogistico_VPUA
                .Where(r => r.CES_Estado == ConstantesFramework.ESTADO_ACTIVO && r.AGE_IdCentroLogistico == idCol)
                .OrderBy(o => o.CES_Nombre)
                .ToList()
                .ConvertAll<LILocalidadColDC>(r => new LILocalidadColDC()
                {
                    NombreLocalidad = r.LOC_Nombre,
                    IdLocalidad = r.LOC_IdLocalidad,
                    IdCentroServicio = r.AGE_IdAgencia,
                    NombreCentroServicio = r.CES_Nombre,
                    Direccion = r.CES_Direccion,
                    Telefono = r.CES_Telefono1,
                });

                if (agenciasCol.Count > 0)
                    return agenciasCol;
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                     EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS.ToString(),
                     MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_TIENE_AGENCIAS)));
                }
            }
        }

        /// <summary>
        /// Obtiene los puntos del centro logistico
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosServiciosCol(long idCol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PuntoServiciosCol_VPUA.Where(punto => punto.CEL_IdCentroLogistico == idCol
                  && punto.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      Nombre = r.CES_Nombre,
                      IdCentroServicio = r.CEL_IdCentroLogistico,
                      Telefono1 = r.CES_Telefono1,
                      Direccion = r.CES_Direccion,
                      IdMunicipio = r.CES_IdMunicipio,
                      NombreMunicipio = r.LOC_Nombre
                  });
            }
        }

        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasCol(long idCol)
        {
            List<PUCentroServiciosDC> lstPuntosAgencias = new List<PUCentroServiciosDC>();
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var agenciasCol = contexto.AgenciasDeCentroLogistico_VPUA
                               .Where(r => r.CES_Estado == ConstantesFramework.ESTADO_ACTIVO && r.AGE_IdCentroLogistico == idCol)
                               .OrderBy(o => o.CES_Nombre)
                               .ToList();

                if (agenciasCol != null && agenciasCol.Any())
                {
                    agenciasCol.ForEach(age =>
                    {
                        lstPuntosAgencias.Add(new PUCentroServiciosDC()
                        {
                            IdCentroServicio = age.AGE_IdAgencia,
                            Nombre = age.AGE_IdAgencia.ToString() + '-' + age.CES_Nombre,
                            Telefono1 = age.CES_Telefono1 == null ? string.Empty : age.CES_Telefono1,
                            Direccion = age.CES_Direccion,
                            IdMunicipio = age.LOC_IdLocalidad,
                            NombreMunicipio = age.NombreCompleto
                        });
                    });
                }

                var puntosCol = contexto.PuntoServiciosCol_VPUA.Where(punto => punto.CEL_IdCentroLogistico == idCol
                  && punto.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList();

                if (puntosCol != null && puntosCol.Any())
                {
                    puntosCol.ForEach(punt =>
                    {
                        lstPuntosAgencias.Add(new PUCentroServiciosDC()
                        {
                            IdCentroServicio = punt.PUS_IdPuntoServicio,
                            Nombre = punt.PUS_IdPuntoServicio.ToString() + '-' + punt.CES_Nombre,
                            Telefono1 = punt.CES_Telefono1 == null ? string.Empty : punt.CES_Telefono1,
                            Direccion = punt.CES_Direccion,
                            IdMunicipio = punt.LOC_IdLocalidad,
                            NombreMunicipio = punt.NombreCompleto,
                        });
                    });
                }

                return lstPuntosAgencias;
            }
        }

        /// <summary>
        /// Obtener centros de servicios y racol
        /// </summary>
        /// <returns></returns>
        public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosRacol()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentrosServicosRacol_VPUA.Where(r => r.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new PUAgenciaDeRacolDC()
                  {
                      IdCentroServicio = r.CES_IdCentroServicios,
                      NombreCentroServicio = r.CES_IdCentroServicios + " - " + r.CES_Nombre,
                      IdResponsable = r.REA_IdRegionalAdm,
                      NombreResponsable = r.REA_IdRegionalAdm + " - " + r.REA_Descripcion,
                      TipoCentroServicio = r.CES_Tipo
                  });
            }
        }




        /// <summary>
        /// Obtener todos los racoles activos
        /// </summary>
        /// <returns>Colección con los racoles acitvos</returns>
        public List<PUCentroServiciosDC> ObtenerRacoles()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicios_PUA
                  .Where(r => r.CES_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                         r.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
                  .ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      Nombre = r.CES_Nombre,
                      IdCentroServicio = r.CES_IdCentroServicios,
                      Telefono1 = r.CES_Telefono1,
                      Direccion = r.CES_Direccion,
                      IdMunicipio = r.CES_IdMunicipio,
                      Tipo = r.CES_Tipo,
                      CodigoBodega = r.CES_CodigoBodega,
                      IdCentroCostos = r.CES_IdCentroCostos
                  });
            }
        }

        /// <summary>
        /// Obtener todos los coles activos
        /// </summary>
        /// <returns>Colección con los coles activos</returns>
        public List<PUCentroServiciosDC> ObtenerTodosColes()
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodosColes_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();
                return dt.AsEnumerable().ToList().ConvertAll<PUCentroServiciosDC>(cs =>
                {
                    PUCentroServiciosDC r = new PUCentroServiciosDC()
                    {
                        Nombre = cs.Field<string>("CES_Nombre"),
                        IdCentroServicio = cs.Field<long>("CES_IdCentroServicios"),
                        Telefono1 = cs.Field<string>("CES_Telefono1"),
                        Direccion = cs.Field<string>("CES_Direccion"),
                        IdMunicipio = cs.Field<string>("CES_IdMunicipio"),
                        NombreMunicipio = cs.Field<string>("LOC_Nombre"),
                        NombreDepto = cs.Field<string>("LOC_NombrePrimero"),
                        NombrePais = cs.Field<string>("LOC_NombreSegundo"),
                        Tipo = cs.Field<string>("CES_Tipo"),
                        CodigoBodega = cs.Field<string>("CES_CodigoBodega"),
                        IdCentroCostos = cs.Field<string>("CES_IdCentroCostos"),
                    };
                    return r;
                });
            }
        }

        /// <summary>
        /// Obtiene el horario de la recogida de un centro de Servicio
        /// </summary>
        /// <param name="idCentroSvc">es le id del centro svc</param>
        /// <returns>info de la recogida</returns>
        public List<PUHorarioRecogidaCentroSvcDC> ObtenerHorariosRecogidasCentroSvc(long idCentroSvc)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var horarios = contexto.Dia_PAR.Join(contexto.HorarioRecogidaCentroSvc_PUA.Where
                  (h => h.HRC_IdCentroServicios == idCentroSvc)
                  , d => d.DIA_IdDia, h => h.HRC_Dia, (d, h) =>
                    new PUHorarioRecogidaCentroSvcDC()
                    {
                        Hora = h.HRC_Hora,
                        Dia = h.HRC_Dia,
                        NombreDia = d.DIA_NombreDia,
                        IdCentroServicio = idCentroSvc,
                    }).ToList();
                return horarios;
            }
        }

        /// <summary>
        /// obtiene todos los centros servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicio()
        {
            List<PUCentroServiciosDC> CentrosServicio = new List<PUCentroServiciosDC>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerLocalidades_VPAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    CentrosServicio = MERepositorioMapper.ToListCentrosServicio(resultado);
                }
                return CentrosServicio;

            }
        }

        public List<PUCentroServiciosDC> ObtenerCentrosServicioTipo()
        {
            List<PUCentroServiciosDC> CentrosServicio = new List<PUCentroServiciosDC>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerLocalidadesTipoCiudad_VPAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    CentrosServicio = MERepositorioMapper.ToListCentrosServicioTipo(resultado);
                }
                conn.Close();
                return CentrosServicio;

            }
        }

        /// <summary>
        /// Guarda registro en la tabla de distribucion para mis pendientes de agendas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="usuario"></param>
        public void GuardarCentroServicioDistribucion(long idCentroServicio, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paAdministrarDistribucion_CES", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Operacion", 0); // operación 0 inserta
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Consulta si el centro servicio existe en la tabla de distribucion de centros servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public bool ConsultarMarcaMisPendietesCentroServicioAgencia(long idCentroServicio)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paAdministrarDistribucion_CES", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                cmd.Parameters.AddWithValue("@Operacion", 2); // operación 0 inserta
                conn.Open();
                bool retorno = Convert.ToBoolean(cmd.ExecuteScalar());
                conn.Close();
                return retorno;
            }
        }

        /// <summary>
        /// Elimina registro de la tabla distribucion centro servicio de mis pendientes de agendas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        public void BorrarCentroServicioDistribucion(long idCentroServicio)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paAdministrarDistribucion_CES", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                cmd.Parameters.AddWithValue("@Operacion", 1); // operación 1 elimina
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        #region Insercion

        /// <summary>
        /// Adiciona una agencia
        /// </summary>
        /// <param name="centroServicios">Objeto centro de servicios</param>
        public long AdicionarAgencia(PUCentroServiciosDC centroServicios, ref long idAgenciaIna)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IQueryable<Agencia_PUA> lstAgencias = contexto.Agencia_PUA.Include("CentroServicios_PUA").Where(obj => obj.CentroServicios_PUA.CES_IdMunicipio == centroServicios.IdMunicipio);

                if (lstAgencias.Where(a => a.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO).Count() > 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_EXISTE_AGENCIA_EN_CIUDAD.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_EXISTE_AGENCIA_EN_CIUDAD)));
                }

                CentroServicios_PUA centroServi = new CentroServicios_PUA()
                {
                    CES_BaseInicialCaja = centroServicios.BaseInicialCaja,
                    CES_Barrio = centroServicios.Barrio,
                    CES_CreadoPor = ControllerContext.Current.Usuario,
                    CES_FechaGrabacion = DateTime.Now,
                    CES_IdPersonaResponsable = centroServicios.IdPersonaResponsable,
                    CES_IdPropietario = centroServicios.IdPropietario,
                    CES_IdZona = centroServicios.IdZona,
                    CES_Tipo = centroServicios.Tipo,
                    CES_Telefono2 = centroServicios.Telefono2,
                    CES_Telefono1 = centroServicios.Telefono1,
                    CES_Sistematizada = centroServicios.Sistematizado,
                    CES_AplicaPAM = centroServicios.PuntoPAM,
                    CES_Nombre = centroServicios.Nombre,
                    CES_Longitud = centroServicios.Longitud,
                    CES_Latitud = centroServicios.Latitud,
                    CES_IdMunicipio = centroServicios.IdMunicipio,
                    CES_IdCentroServicios = centroServicios.IdCentroServicio,
                    CES_Fax = centroServicios.Fax,
                    CES_IdCentroCostos = centroServicios.IdCentroCostos,
                    CES_Estado = centroServicios.Estado,
                    CES_IdTipoPropiedad = Convert.ToInt16(centroServicios.IdTipoPropiedad),
                    CES_Email = centroServicios.Email,
                    CES_Direccion = centroServicios.Direccion,
                    CES_DigitoVerificacion = centroServicios.DigitoVerificacion,
                    CES_VolumenMaximo = centroServicios.VolumenMaximo,
                    CES_PesoMaximo = centroServicios.PesoMaximo,
                    CES_VendePrepago = centroServicios.VendePrepago,
                    CES_AdmiteFormaPagoAlCobro = centroServicios.AdmiteFormaPagoAlCobro,
                    CES_CodigoBodega = centroServicios.CodigoBodega,
                    CES_IdClasificadorCanalVenta = (short)centroServicios.IdClasificadorCanalVenta,
                    CES_NombreAMostrar = centroServicios.NombreAMostrar,
                    CES_Codigo472 = centroServicios.Codigo472,
                    CES_FechaApertura = centroServicios.FechaApertura,
                    CES_FechaCierre = centroServicios.FechaCierre,
                    CES_Biometrico = centroServicios.Biometrico,
                    CES_Recaudo = centroServicios.Recaudo,

                    CES_ClasGirosPorIngresos = "COM",
                    CES_PuedePagarGiros = true,
                    CES_PuedeRecibirGiros = true,
                    CES_TopeMaximoPorGiros = 0,
                    CES_CodigoSPN = centroServicios.CodigoSPN == null ? "0" : centroServicios.CodigoSPN
                };

                Agencia_PUA agencia = new Agencia_PUA()
                {
                    AGE_IdAgencia = centroServi.CES_IdCentroServicios,
                    AGE_CreadoPor = ControllerContext.Current.Usuario,
                    AGE_FechaGrabacion = DateTime.Now,
                    AGE_IdCentroLogistico = centroServicios.IdColRacolApoyo.Value,
                    AGE_IdTipoAgencia = centroServicios.IdTipoAgencia,
                    AGE_Operacional = centroServicios.Operacional
                };

                CentroLogistico_PUA colApoyo = contexto.CentroLogistico_PUA.Where(cl => cl.CEL_IdCentroLogistico == centroServicios.IdColRacolApoyo.Value).FirstOrDefault();
                if (colApoyo != null)
                {
                    //  Cuando se define el RACOL de apoyo, se asume que es a quien debe reportarle el centro de servicios, por tanto se deeben aplicar cambios en la tabla "CentroServiciosReporteDinero_PUA"
                    contexto.CentroServiciosReporteDinero_PUA.Add(new CentroServiciosReporteDinero_PUA
                    {
                        CRD_CreadoPor = ControllerContext.Current.Usuario,
                        CRD_FechaGrabacion = DateTime.Now,
                        CRD_IdCentroServiciosAQuienReporta = colApoyo.CEL_IdRegionalAdm,
                        CRD_IdCentroServiciosQueReporta = centroServi.CES_IdCentroServicios
                    });
                }

                contexto.CentroServicios_PUA.Add(centroServi);
                contexto.Agencia_PUA.Add(agencia);

                ///Heredar las sucursales de los clientes a la nueva agencia, desde una agencia inactiva
                if (lstAgencias.Where(a => a.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_INACTIVO).Count() > 0)
                {
                    Agencia_PUA age = lstAgencias.Where(a => a.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_INACTIVO).OrderByDescending(a => a.CentroServicios_PUA.CES_IdCentroServicios).FirstOrDefault();
                    if (age != null)
                        idAgenciaIna = age.CentroServicios_PUA.CES_IdCentroServicios;
                }

                validarMunicipioSinFormaPagoAlCobro(centroServicios.Estado, centroServicios.IdMunicipio);

                this.AuditarAgencia(contexto);
                contexto.SaveChanges();

                if (centroServicios.Estado == "ACT")
                {
                    contexto.paEditarAgenciaDePuntosSinAgenciaActiva_PUA(centroServicios.IdMunicipio, centroServi.CES_IdCentroServicios);
                }

                return centroServi.CES_IdCentroServicios;


            }
        }

        /// <summary>
        /// Adiciona un racol
        /// </summary>
        /// <param name="centroServicios">Objeto centro de servicios</param>
        public long AdicionarRacol(PUCentroServiciosDC centroServicios)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paAdicionarRacol_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter paramOut = new SqlParameter("@IdCSRetorna", SqlDbType.BigInt);
                paramOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paramOut);
                cmd.Parameters.AddWithValue("@Tipo", centroServicios.Tipo);
                cmd.Parameters.AddWithValue("@DigitoVerif", centroServicios.DigitoVerificacion);
                cmd.Parameters.AddWithValue("@IdPropietario", Convert.ToInt16(centroServicios.IdTipoPropiedad));
                cmd.Parameters.AddWithValue("@Nombre", centroServicios.Nombre);
                cmd.Parameters.AddWithValue("@Telefono1", centroServicios.Telefono1);
                cmd.Parameters.AddWithValue("@Telefono2", centroServicios.Telefono2);
                cmd.Parameters.AddWithValue("@Fax", centroServicios.Fax);
                cmd.Parameters.AddWithValue("@Direccion", centroServicios.Direccion);
                cmd.Parameters.AddWithValue("@Barrio", centroServicios.Barrio);
                cmd.Parameters.AddWithValue("@IdZona", centroServicios.IdZona);
                cmd.Parameters.AddWithValue("@IdMunicipio", centroServicios.IdMunicipio);
                cmd.Parameters.AddWithValue("@Estado", centroServicios.Estado);
                cmd.Parameters.AddWithValue("@IdPersonaResponsable", centroServicios.IdPersonaResponsable);
                cmd.Parameters.AddWithValue("@Latitud", centroServicios.Latitud);
                cmd.Parameters.AddWithValue("@Longitud", centroServicios.Longitud);
                cmd.Parameters.AddWithValue("@Email", centroServicios.Email);
                cmd.Parameters.AddWithValue("@Sistematizada", centroServicios.Sistematizado);
                cmd.Parameters.AddWithValue("@IdCentroCostos", centroServicios.IdCentroCostos);
                cmd.Parameters.AddWithValue("@IdTipoPropiedad", Convert.ToInt16(centroServicios.IdTipoPropiedad));
                cmd.Parameters.AddWithValue("@PesoMaximo", centroServicios.PesoMaximo);
                cmd.Parameters.AddWithValue("@VolumenMaximo", centroServicios.VolumenMaximo);
                cmd.Parameters.AddWithValue("@AdmiteFormaPagoAlCobro", centroServicios.AdmiteFormaPagoAlCobro);
                cmd.Parameters.AddWithValue("@VendePrepago", centroServicios.VendePrepago);
                cmd.Parameters.AddWithValue("@ClasGirosPorIngresos", "COM");
                cmd.Parameters.AddWithValue("@PuedePagarGiros", false);
                cmd.Parameters.AddWithValue("@PuedeRecibirGiros", false);
                cmd.Parameters.AddWithValue("@TopeMaximoPorGiros", 0);
                cmd.Parameters.AddWithValue("@BaseInicialCaja", centroServicios.BaseInicialCaja);
                cmd.Parameters.AddWithValue("@CodigoBodega", centroServicios.CodigoBodega);
                cmd.Parameters.AddWithValue("@IdClasificadorCanalVenta", (short)centroServicios.IdClasificadorCanalVenta);
                cmd.Parameters.AddWithValue("@CodigoSPN", centroServicios.CodigoSPN == null ? "0" : centroServicios.CodigoSPN);
                cmd.Parameters.AddWithValue("@NombreAMostrar", centroServicios.NombreAMostrar);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@FechaApertura", centroServicios.FechaApertura);
                cmd.Parameters.AddWithValue("@FechaCierre", centroServicios.FechaCierre);
                cmd.Parameters.AddWithValue("@Descripcion", centroServicios.DescripcionRacol.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@FechaCambio", centroServicios.FechaCierre);
                cmd.Parameters.AddWithValue("@CambiadoPor", centroServicios.FechaCierre);
                cmd.Parameters.AddWithValue("@TipoCambio", centroServicios.FechaCierre);
                conn.Open();
                cmd.ExecuteNonQuery();
                long IdRacol = Convert.ToInt32(paramOut.Value);
                conn.Close();

                return IdRacol;
            }
        }

        /// <summary>
        /// Adiciona un COL
        /// </summary>
        /// <param name="centroServicios">Objeto centro de servicios</param>
        public long AdicionarCOL(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.CentroLogistico_PUA.Include("CentroServicios_PUA").Where(obj => obj.CentroServicios_PUA.CES_IdMunicipio == centroServicios.IdMunicipio && obj.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO).Count() > 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_EXISTE_COL_EN_CIUDAD.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_EXISTE_COL_EN_CIUDAD)));
                }

                CentroServicios_PUA centroServi = new CentroServicios_PUA()
                {
                    CES_BaseInicialCaja = centroServicios.BaseInicialCaja,
                    CES_Barrio = centroServicios.Barrio,
                    CES_CreadoPor = ControllerContext.Current.Usuario,
                    CES_FechaGrabacion = DateTime.Now,
                    CES_IdPersonaResponsable = centroServicios.IdPersonaResponsable,
                    CES_IdPropietario = centroServicios.IdPropietario,
                    CES_IdZona = centroServicios.IdZona,
                    CES_Tipo = ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA,
                    CES_Telefono2 = centroServicios.Telefono2,
                    CES_Telefono1 = centroServicios.Telefono1,
                    CES_Sistematizada = centroServicios.Sistematizado,
                    CES_Nombre = centroServicios.Nombre,
                    CES_Longitud = centroServicios.Longitud,
                    CES_Latitud = centroServicios.Latitud,
                    CES_IdMunicipio = centroServicios.IdMunicipio,
                    CES_IdCentroServicios = centroServicios.IdCentroServicio,
                    CES_Fax = centroServicios.Fax,
                    CES_IdCentroCostos = centroServicios.IdCentroCostos,
                    CES_Estado = centroServicios.Estado,
                    CES_IdTipoPropiedad = Convert.ToInt16(centroServicios.IdTipoPropiedad),
                    CES_Email = centroServicios.Email,
                    CES_Direccion = centroServicios.Direccion,
                    CES_DigitoVerificacion = centroServicios.DigitoVerificacion,
                    CES_VolumenMaximo = centroServicios.VolumenMaximo,
                    CES_PesoMaximo = centroServicios.PesoMaximo,
                    CES_VendePrepago = centroServicios.VendePrepago,
                    CES_AdmiteFormaPagoAlCobro = centroServicios.AdmiteFormaPagoAlCobro,
                    CES_CodigoBodega = centroServicios.CodigoBodega,
                    CES_IdClasificadorCanalVenta = (short)centroServicios.IdClasificadorCanalVenta,
                    CES_NombreAMostrar = centroServicios.NombreAMostrar,
                    CES_Codigo472 = centroServicios.Codigo472,
                    CES_FechaApertura = centroServicios.FechaApertura,
                    CES_FechaCierre = centroServicios.FechaCierre,

                    CES_ClasGirosPorIngresos = "COM",
                    CES_PuedePagarGiros = false,
                    CES_PuedeRecibirGiros = false,
                    CES_TopeMaximoPorGiros = 0,
                    CES_CodigoSPN = centroServicios.CodigoSPN == null ? "0" : centroServicios.CodigoSPN
                };

                CentroLogistico_PUA col = new CentroLogistico_PUA()
                {
                    CEL_CreadoPor = ControllerContext.Current.Usuario,
                    CEL_FechaGrabacion = DateTime.Now,
                    CEL_IdRegionalAdm = centroServicios.IdColRacolApoyo.Value
                };

                Agencia_PUA agencia = new Agencia_PUA()
                {
                    AGE_CreadoPor = ControllerContext.Current.Usuario,
                    AGE_FechaGrabacion = DateTime.Now,
                    AGE_IdTipoAgencia = ConstantesFramework.TIPO_CENTRO_SERVICIO_COL
                };

                //  Cuando se define el RACOL de apoyo, se asume que es a quien debe reportarle el centro de servicios, por tanto se deeben aplicar cambios en la tabla "CentroServiciosReporteDinero_PUA"
                contexto.CentroServiciosReporteDinero_PUA.Add(new CentroServiciosReporteDinero_PUA
                {
                    CRD_CreadoPor = ControllerContext.Current.Usuario,
                    CRD_FechaGrabacion = DateTime.Now,
                    CRD_IdCentroServiciosAQuienReporta = centroServicios.IdColRacolApoyo.Value,
                    CRD_IdCentroServiciosQueReporta = centroServi.CES_IdCentroServicios
                });

                col.Agencia_PUA.Add(agencia);
                contexto.CentroServicios_PUA.Add(centroServi);
                contexto.CentroLogistico_PUA.Add(col);
                AuditarCOL(contexto);
                contexto.SaveChanges();
                return centroServi.CES_IdCentroServicios;
            }
        }

        /// <summary>
        /// Adiciona un punto
        /// </summary>
        /// <param name="centroServicios">Objeto centro de servicios</param>
        public long AdicionarPunto(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long IdAgencia = 0;

                var age = contexto.Agencia_PUA.Include("CentroServicios_PUA").Where(obj => obj.CentroServicios_PUA.CES_IdMunicipio == centroServicios.IdMunicipio && obj.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO);
                if (age != null && age.Count() > 0)
                    IdAgencia = age.First().CentroServicios_PUA.CES_IdCentroServicios;
                else
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_NO_AGENCIA_EN_MUNICIPIO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_NO_AGENCIA_EN_MUNICIPIO)));

                CentroServicios_PUA centroServi = new CentroServicios_PUA()
                {
                    CES_BaseInicialCaja = centroServicios.BaseInicialCaja,
                    CES_Barrio = centroServicios.Barrio,
                    CES_CreadoPor = ControllerContext.Current.Usuario,
                    CES_FechaGrabacion = DateTime.Now,
                    CES_IdPersonaResponsable = centroServicios.IdPersonaResponsable,
                    CES_IdPropietario = centroServicios.IdPropietario,
                    CES_IdZona = centroServicios.IdZona,
                    CES_Tipo = centroServicios.Tipo,
                    CES_Telefono2 = centroServicios.Telefono2,
                    CES_Telefono1 = centroServicios.Telefono1,
                    CES_Sistematizada = centroServicios.Sistematizado,
                    CES_AplicaPAM = centroServicios.PuntoPAM,
                    CES_Nombre = centroServicios.Nombre,
                    CES_Longitud = centroServicios.Longitud,
                    CES_Latitud = centroServicios.Latitud,
                    CES_IdMunicipio = centroServicios.IdMunicipio,
                    CES_IdCentroServicios = centroServicios.IdCentroServicio,
                    CES_Fax = centroServicios.Fax,
                    CES_IdCentroCostos = centroServicios.IdCentroCostos,
                    CES_Estado = centroServicios.Estado,
                    CES_IdTipoPropiedad = Convert.ToInt16(centroServicios.IdTipoPropiedad),
                    CES_Email = centroServicios.Email,
                    CES_Direccion = centroServicios.Direccion,
                    CES_DigitoVerificacion = centroServicios.DigitoVerificacion,
                    CES_VolumenMaximo = centroServicios.VolumenMaximo,
                    CES_PesoMaximo = centroServicios.PesoMaximo,
                    CES_VendePrepago = centroServicios.VendePrepago,
                    CES_AdmiteFormaPagoAlCobro = centroServicios.AdmiteFormaPagoAlCobro,
                    CES_CodigoBodega = centroServicios.CodigoBodega,
                    CES_IdClasificadorCanalVenta = (short)centroServicios.IdClasificadorCanalVenta,
                    CES_NombreAMostrar = centroServicios.NombreAMostrar,
                    CES_Codigo472 = centroServicios.Codigo472,
                    CES_FechaApertura = centroServicios.FechaApertura,
                    CES_FechaCierre = centroServicios.FechaCierre,
                    CES_Biometrico = centroServicios.Biometrico,
                    CES_Recaudo = centroServicios.Recaudo,
                    CES_ClasGirosPorIngresos = "COM",
                    CES_PuedePagarGiros = true,
                    CES_PuedeRecibirGiros = true,
                    CES_TopeMaximoPorGiros = 0,
                    CES_CodigoSPN = centroServicios.CodigoSPN == null ? "0" : centroServicios.CodigoSPN
                };

                PuntoServicio_PUA punto = new PuntoServicio_PUA()
                {
                    PUS_CreadoPor = ControllerContext.Current.Usuario,
                    PUS_FechaGrabacion = DateTime.Now,
                    PUS_IdAgencia = IdAgencia,
                    PUS_IdPuntoServicio = centroServi.CES_IdCentroServicios,
                    PUS_ReclameEnOficina = centroServicios.ReclameEnOficina
                };

                //  Cuando se define el RACOL de apoyo, se asume que es a quien debe reportarle el centro de servicios, por tanto se deeben aplicar cambios en la tabla "CentroServiciosReporteDinero_PUA"
                contexto.CentroServiciosReporteDinero_PUA.Add(new CentroServiciosReporteDinero_PUA
                {
                    CRD_CreadoPor = ControllerContext.Current.Usuario,
                    CRD_FechaGrabacion = DateTime.Now,
                    CRD_IdCentroServiciosAQuienReporta = IdAgencia,
                    CRD_IdCentroServiciosQueReporta = centroServi.CES_IdCentroServicios
                });

                contexto.CentroServicios_PUA.Add(centroServi);
                contexto.PuntoServicio_PUA.Add(punto);
                AuditarPunto(contexto);
                contexto.SaveChanges();
                return centroServi.CES_IdCentroServicios;
            }
        }

        /// <summary>
        /// Inserta los horarios de un centro de sevicio
        /// </summary>
        /// <param name="centroServicios"></param>
        public void AdicionarHorarios(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var horarios = contexto.HorarioCentroServicio_PUA.Where(obj => obj.HCS_IdCentroServicios == centroServicios.IdCentroServicio).ToList();
                for (int i = horarios.Count - 1; i >= 0; i--)
                {
                    contexto.HorarioCentroServicio_PUA.Remove(horarios[i]);
                }

                centroServicios.HorariosCentroServicios.ToList().ForEach(obj =>
                {
                    contexto.HorarioCentroServicio_PUA.Add(new HorarioCentroServicio_PUA()
                    {
                        HCS_CreadoPor = ControllerContext.Current.Usuario,
                        HCS_FechaGrabacion = DateTime.Now,
                        HCS_HoraFin = obj.HoraFin,
                        HCS_HoraInicio = obj.HoraInicio,
                        HCS_IdCentroServicios = centroServicios.IdCentroServicio,
                        HCS_IdDia = obj.IdDia.Trim()
                    });
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona los Horarios de las recogidas
        /// de los centros de Svc
        /// </summary>
        /// <param name="centroServicios">info del Centro de Servicio</param>
        public void AdicionarHorariosRecogidasCentroSvc(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<HorarioRecogidaCentroSvc_PUA> horario = contexto.HorarioRecogidaCentroSvc_PUA.Where(id => id.HRC_IdCentroServicios == centroServicios.IdCentroServicio).ToList();
                if (horario.Count > 0)
                {
                    horario.ForEach(recog =>
                    {
                        contexto.HorarioRecogidaCentroSvc_PUA.Remove(recog);
                    });
                }

                centroServicios.HorariosRecogidasCentroServicio.ToList().ForEach(recog =>
                {
                    contexto.HorarioRecogidaCentroSvc_PUA.Add(new HorarioRecogidaCentroSvc_PUA()
                    {
                        HRC_IdCentroServicios = centroServicios.IdCentroServicio,
                        HRC_Dia = recog.Dia,
                        HRC_Hora = recog.Hora,
                        HRC_FechaGrabacion = DateTime.Now,
                        HRC_CreadoPor = ControllerContext.Current.Usuario,
                    });
                });
                contexto.SaveChanges();
            }
        }

        #endregion Insercion

        #region Edicion

        /// <summary>
        /// Modifica un Centro de servicios
        /// </summary>
        /// <param name="propietario">Objeto centro servicios</param>
        public void EditarCentrosServicio(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicios_PUA centro = contexto.CentroServicios_PUA
                  .Where(obj => obj.CES_IdCentroServicios == centroServicios.IdCentroServicio)
                  .SingleOrDefault();

                if (centro == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                centro.CES_BaseInicialCaja = centroServicios.BaseInicialCaja;
                centro.CES_Barrio = centroServicios.Barrio;
                centro.CES_IdPersonaResponsable = centroServicios.IdPersonaResponsable;
                centro.CES_IdPropietario = centroServicios.IdPropietario;
                centro.CES_IdZona = centroServicios.IdZona;
                centro.CES_Tipo = centroServicios.Tipo;
                centro.CES_Telefono2 = centroServicios.Telefono2;
                centro.CES_Telefono1 = centroServicios.Telefono1;
                centro.CES_Sistematizada = centroServicios.Sistematizado;
                centro.CES_AplicaPAM = centroServicios.PuntoPAM;
                centro.CES_Nombre = centroServicios.Nombre;
                centro.CES_Longitud = centroServicios.Longitud;
                centro.CES_Latitud = centroServicios.Latitud;
                centro.CES_IdMunicipio = centroServicios.IdMunicipio;
                centro.CES_IdCentroServicios = centroServicios.IdCentroServicio;
                centro.CES_Fax = centroServicios.Fax;
                centro.CES_IdCentroCostos = centroServicios.IdCentroCostos;
                centro.CES_Estado = centroServicios.Estado;
                centro.CES_IdTipoPropiedad = Convert.ToInt16(centroServicios.IdTipoPropiedad);
                centro.CES_Email = centroServicios.Email;
                centro.CES_Direccion = centroServicios.Direccion;
                centro.CES_DigitoVerificacion = centroServicios.DigitoVerificacion;
                centro.CES_VolumenMaximo = centroServicios.VolumenMaximo;
                centro.CES_PesoMaximo = centroServicios.PesoMaximo;
                centro.CES_VendePrepago = centroServicios.VendePrepago;
                centro.CES_AdmiteFormaPagoAlCobro = centroServicios.AdmiteFormaPagoAlCobro;
                centro.CES_IdClasificadorCanalVenta = (short)centroServicios.IdClasificadorCanalVenta;
                centro.CES_CodigoBodega = centroServicios.CodigoBodega;
                centro.CES_CodigoSPN = centroServicios.CodigoSPN;
                centro.CES_NombreAMostrar = centroServicios.NombreAMostrar;
                centro.CES_Codigo472 = centroServicios.Codigo472;
                centro.CES_FechaApertura = centroServicios.FechaApertura;
                centro.CES_FechaCierre = centroServicios.FechaCierre;
                centro.CES_CodigoPostal = centroServicios.CodigoPostal;
                centro.CES_Biometrico = centroServicios.Biometrico;
                centro.CES_Recaudo = centroServicios.Recaudo;
                this.AuditarCentroServicio(contexto);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita una agencia
        /// </summary>
        /// <param name="centroServicios"></param>
        public void EditarAgencia(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (centroServicios.Estado == "ACT")
                {
                    IQueryable<Agencia_PUA> lstAgencias = contexto.Agencia_PUA.Include("CentroServicios_PUA").Where(obj => obj.CentroServicios_PUA.CES_IdMunicipio == centroServicios.IdMunicipio);
                    if (lstAgencias.Count() > 1)
                    {
                        if (lstAgencias.Where(a => a.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO && a.AGE_IdAgencia != centroServicios.IdCentroServicio).Count() > 0)
                        {
                            throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_EXISTE_AGENCIA_EN_CIUDAD.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_EXISTE_AGENCIA_EN_CIUDAD)));
                        }
                        else
                        {
                            contexto.paEditarAgenciaDePuntosSinAgenciaActiva_PUA(centroServicios.IdMunicipio, centroServicios.IdCentroServicio);
                        }
                    }

                }


                Agencia_PUA agencia = contexto.Agencia_PUA.Include("CentroServicios_PUA")
                  .Where(obj => obj.AGE_IdAgencia == centroServicios.IdCentroServicio)
                  .SingleOrDefault();

                if (agencia == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                agencia.AGE_IdTipoAgencia = centroServicios.IdTipoAgencia;
                agencia.AGE_IdCentroLogistico = centroServicios.IdColRacolApoyo.Value;

                //  Cuando se define el RACOL de apoyo, se asume que es a quien debe reportarle el centro de servicios, por tanto se deeben aplicar cambios en la tabla "CentroServiciosReporteDinero_PUA"
                CentroServiciosReporteDinero_PUA aQuienReporte = contexto.CentroServiciosReporteDinero_PUA.FirstOrDefault(r => r.CRD_IdCentroServiciosQueReporta == centroServicios.IdCentroServicio);

                if (aQuienReporte != null)
                {
                    contexto.CentroServiciosReporteDinero_PUA.Remove(aQuienReporte);
                }

                CentroLogistico_PUA colApoyo = contexto.CentroLogistico_PUA.Where(cl => cl.CEL_IdCentroLogistico == centroServicios.IdColRacolApoyo.Value).FirstOrDefault();
                if (colApoyo != null)
                {
                    contexto.CentroServiciosReporteDinero_PUA.Add(new CentroServiciosReporteDinero_PUA
                    {
                        CRD_CreadoPor = ControllerContext.Current.Usuario,
                        CRD_FechaGrabacion = DateTime.Now,
                        CRD_IdCentroServiciosAQuienReporta = colApoyo.CEL_IdRegionalAdm,
                        CRD_IdCentroServiciosQueReporta = centroServicios.IdCentroServicio
                    });
                }

                validarMunicipioSinFormaPagoAlCobro(centroServicios.Estado, agencia.CentroServicios_PUA.CES_IdMunicipio);
                AuditarAgencia(contexto);
                agencia.AGE_Operacional = centroServicios.Operacional;
                contexto.SaveChanges();
            }
        }

        private void validarMunicipioSinFormaPagoAlCobro(string estado, string idMunicipio)
        {
            // Si la agencia se está inactivando se debe agregar el municipio a aquellos que no aceptan forma de pago al cobro,
            // si se está activando, se debe verificar si estaba en dicha lista y si fué agregado por este procedimiento, en ese caso, se debe remover de dicha lista.
            if (estado == "INA")
            {
                RegistrarMunicipioSinFormaPagoAlCobro(new PALocalidadDC { IdLocalidad = idMunicipio }, true);
            }
            else if (estado == "ACT")
            {
                RemoverMunicipioSinFormaPagoAlCobro(new PALocalidadDC { IdLocalidad = idMunicipio }, true);
            }
        }

        /// <summary>
        /// Edita un Racol y adiciona el cambio en la tabla de auditoria
        /// </summary>
        /// <param name="centroServicios"></param>
        public void EditarRacol(PUCentroServiciosDC centroServicios)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paEditarRacol_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRegionalAdm", centroServicios.IdCentroServicio);
                cmd.Parameters.AddWithValue("@Descripcion", centroServicios.DescripcionRacol);
                cmd.Parameters.AddWithValue("@FechaGrabacion", centroServicios.Fecha);
                cmd.Parameters.AddWithValue("@CreadoPor", centroServicios.CreadoPor);
                cmd.Parameters.AddWithValue("@CodigoExternoSucursal", centroServicios.CodRacolExterno);
                cmd.Parameters.AddWithValue("@IdCasaMatriz", (short)centroServicios.IdCasaMatriz);
                cmd.Parameters.AddWithValue("@IdTerritorial", centroServicios.IdTerritorial);
                cmd.Parameters.AddWithValue("@FechaCambio", DateTime.Now);
                cmd.Parameters.AddWithValue("@CambiadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@TipoCambio", "Modified");
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Edita un punto de servicio
        /// </summary>
        /// <param name="centroServicios"></param>
        public void EditarPunto(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ///Solo se audita debido a que el punto de servicio no tiene campos para modificar

                AuditarPunto(contexto);

                PuntoServicio_PUA punto = contexto.PuntoServicio_PUA
                            .Where(obj => obj.PUS_IdPuntoServicio == centroServicios.IdCentroServicio)
                            .SingleOrDefault();

                if (punto == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));

                    throw new FaultException<ControllerException>(excepcion);
                }

                punto.PUS_ReclameEnOficina = centroServicios.ReclameEnOficina;


                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un col
        /// </summary>
        /// <param name="centroServicios"></param>
        public void EditarCOL(PUCentroServiciosDC centroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroLogistico_PUA col = contexto.CentroLogistico_PUA
                  .Where(obj => obj.CEL_IdCentroLogistico == centroServicios.IdCentroServicio)
                  .SingleOrDefault();

                if (col == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                col.CEL_IdRegionalAdm = centroServicios.IdColRacolApoyo.Value;
                CentroServiciosReporteDinero_PUA racolReporte = contexto.CentroServiciosReporteDinero_PUA.FirstOrDefault(r => r.CRD_IdCentroServiciosQueReporta == centroServicios.IdCentroServicio);
                if (racolReporte != null)
                {
                    contexto.CentroServiciosReporteDinero_PUA.Remove(racolReporte);
                }

                //  Cuando se define el RACOL de apoyo, se asume que es a quien debe reportarle el centro de servicios, por tanto se deeben aplicar cambios en la tabla "CentroServiciosReporteDinero_PUA"
                contexto.CentroServiciosReporteDinero_PUA.Add(new CentroServiciosReporteDinero_PUA
                {
                    CRD_CreadoPor = ControllerContext.Current.Usuario,
                    CRD_FechaGrabacion = DateTime.Now,
                    CRD_IdCentroServiciosAQuienReporta = centroServicios.IdColRacolApoyo.Value,
                    CRD_IdCentroServiciosQueReporta = centroServicios.IdCentroServicio
                });

                AuditarCOL(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Edicion

        /// <summary>
        /// Elimina un centro de servicio
        /// </summary>
        /// <param name="propietario">Objeto centro servicio</param>
        public void EliminarCentroServicio(PUCentroServiciosDC centroServicios)
        {
            PURepositorio.Instancia.BorrarCentroServicioDistribucion(centroServicios.IdCentroServicio);

            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicios_PUA centro = contexto.CentroServicios_PUA.Where(obj => obj.CES_IdCentroServicios == centroServicios.IdCentroServicio).SingleOrDefault();
                if (centro != null)
                {
                    contexto.CentroServicios_PUA.Remove(centro);

                    this.AuditarCentroServicio(contexto);

                    contexto.SaveChanges();
                }
            }
        }

        #region Auditar

        /// <summary>
        /// Audita los racol
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarRacol(ModeloCentroServicios contexto)
        {
            contexto.Audit<RegionalAdministrativa_PUA, RegionalAdministrativaHist_PUA>((record, action) => new RegionalAdministrativaHist_PUA()
            {
                REA_Descripcion = record.Field<RegionalAdministrativa_PUA, string>(c => c.REA_Descripcion),

                REA_CreadoPor = record.Field<RegionalAdministrativa_PUA, string>(c => c.REA_CreadoPor),
                REA_FechaCambio = DateTime.Now,
                REA_FechaGrabacion = record.Field<RegionalAdministrativa_PUA, DateTime>(c => c.REA_FechaGrabacion),
                REA_IdRegionalAdm = record.Field<RegionalAdministrativa_PUA, long>(c => c.REA_IdRegionalAdm),
                REA_CambiadoPor = ControllerContext.Current.Usuario,
                REA_TipoCambio = action.ToString()
            }, (cs) => contexto.RegionalAdministrativaHist_PUA.Add(cs));
        }

        /// <summary>
        /// Audita los puntos
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarPunto(ModeloCentroServicios contexto)
        {
            contexto.Audit<PuntoServicio_PUA, PuntoServicioHist_PUA>((record, action) => new PuntoServicioHist_PUA()
            {
                PUS_IdAgencia = record.Field<PuntoServicio_PUA, long>(c => c.PUS_IdAgencia),
                PUS_CreadoPor = record.Field<PuntoServicio_PUA, string>(c => c.PUS_CreadoPor),
                PUS_FechaCambio = DateTime.Now,
                PUS_FechaGrabacion = record.Field<PuntoServicio_PUA, DateTime>(c => c.PUS_FechaGrabacion),
                PUS_IdPuntoServicio = record.Field<PuntoServicio_PUA, long>(c => c.PUS_IdPuntoServicio),
                PUS_CambiadoPor = ControllerContext.Current.Usuario,
                PUS_TipoCambio = action.ToString(),
                PUS_ReclameEnOficina = record.Field<PuntoServicio_PUA, bool>(c => c.PUS_ReclameEnOficina)
            }, (cs) => contexto.PuntoServicioHist_PUA.Add(cs));
        }

        /// <summary>
        /// Audita los COL
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarCOL(ModeloCentroServicios contexto)
        {
            contexto.Audit<CentroLogistico_PUA, CentroLogisticoHist_PUA>((record, action) => new CentroLogisticoHist_PUA()
            {
                CEL_IdRegionalAdm = record.Field<CentroLogistico_PUA, long>(c => c.CEL_IdRegionalAdm),

                CEL_CreadoPor = record.Field<CentroLogistico_PUA, string>(c => c.CEL_CreadoPor),
                CEL_FechaCambio = DateTime.Now,
                CEL_FechaGrabacion = record.Field<CentroLogistico_PUA, DateTime>(c => c.CEL_FechaGrabacion),
                CEL_IdCentroLogistico = record.Field<CentroLogistico_PUA, long>(c => c.CEL_IdCentroLogistico),
                CEL_CambiadoPor = ControllerContext.Current.Usuario,
                CEL_TipoCambio = action.ToString()
            }, (cs) => contexto.CentroLogisticoHist_PUA.Add(cs));
        }

        /// <summary>
        /// Audita las agencias
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarAgencia(ModeloCentroServicios contexto)
        {
            contexto.Audit<Agencia_PUA, AgenciaHist_PUA>((record, action) => new AgenciaHist_PUA()
            {
                AGE_IdTipoAgencia = record.Field<Agencia_PUA, string>(c => c.AGE_IdTipoAgencia),
                AGE_IdCentroLogistico = record.Field<Agencia_PUA, long>(c => c.AGE_IdCentroLogistico),
                AGE_CreadoPor = record.Field<Agencia_PUA, string>(c => c.AGE_CreadoPor),
                AGE_FechaCambio = DateTime.Now,
                AGE_FechaGrabacion = record.Field<Agencia_PUA, DateTime>(c => c.AGE_FechaGrabacion),
                AGE_IdAgencia = record.Field<Agencia_PUA, long>(c => c.AGE_IdAgencia),
                AGE_CambiadoPor = ControllerContext.Current.Usuario,
                AGE_TipoCambio = action.ToString()
            }, (cs) => contexto.AgenciaHist_PUA.Add(cs));
        }

        private void AuditarMunicipioSinFormaPagoAlCobro(ModeloCentroServicios contexto)
        {
            contexto.Audit<MunicipiosSinAlCobro_PUA, MunicipiosSinAlCobroHist_PUA>((record, action) => new MunicipiosSinAlCobroHist_PUA
            {
                MSA_CambiadoPor = ControllerContext.Current.Usuario,
                MSA_CreadoPor = record.Field<MunicipiosSinAlCobro_PUA, string>(c => c.MSA_CreadoPor),
                MSA_FechaCambio = DateTime.Now,
                MSA_FechaGrabacion = record.Field<MunicipiosSinAlCobro_PUA, DateTime>(c => c.MSA_FechaGrabacion),
                MSA_IdLocalidad = record.Field<MunicipiosSinAlCobro_PUA, string>(c => c.MSA_IdLocalidad),
                MSA_IdMunSinAlCobro = record.Field<MunicipiosSinAlCobro_PUA, int>(c => c.MSA_IdMunSinAlCobro),
                MSA_TipoCambio = action.ToString(),
                MSA_AgregadoPorSistema = record.Field<MunicipiosSinAlCobro_PUA, bool>(c => c.MSA_AgregadoPorSistema)
            }, cs => contexto.MunicipiosSinAlCobroHist_PUA.Add(cs));
        }

        //Audita los Centros de Servicio Que Reportan Dinero 
        private void AuditarCentroServicioReporta(ModeloCentroServicios contexto)
        {
            contexto.Audit<CentroServiciosReporteDinero_PUA, CentroServiciosReporteDineroHist_PUA>((record, action) => new CentroServiciosReporteDineroHist_PUA()
            {
                CRD_IdCentroServiciosAQuienReporta = record.Field<CentroServiciosReporteDinero_PUA, long>(c => c.CRD_IdCentroServiciosAQuienReporta),
                CRD_IdCentroServiciosQueReporta = record.Field<CentroServiciosReporteDinero_PUA, long>(c => c.CRD_IdCentroServiciosQueReporta),
                CRD_FechaGrabacion = record.Field<CentroServiciosReporteDinero_PUA, DateTime>(c => c.CRD_FechaGrabacion),
                CRD_CreadoPor = record.Field<CentroServiciosReporteDinero_PUA, string>(c => c.CRD_CreadoPor),
                CRD_CambiadoPor = ControllerContext.Current.Usuario,
                CRD_FechaCambio = DateTime.Now,
                CRD_TipoCambio = action.ToString(),
            }, (cs) => contexto.CentroServiciosReporteDineroHist_PUA.Add(cs));
        }

        /// <summary>
        /// Audita los centros de servicio
        /// </summary>
        /// <param name="contexto"></param>
        private void AuditarCentroServicio(ModeloCentroServicios contexto)
        {
            contexto.Audit<CentroServicios_PUA, CentroServiciosHist_PUA>((record, action) => new CentroServiciosHist_PUA()
            {
                CES_BaseInicialCaja = record.Field<CentroServicios_PUA, decimal>(c => c.CES_BaseInicialCaja),
                CES_Barrio = record.Field<CentroServicios_PUA, string>(c => c.CES_Barrio),
                CES_CreadoPor = record.Field<CentroServicios_PUA, string>(c => c.CES_CreadoPor),
                CES_DigitoVerificacion = record.Field<CentroServicios_PUA, string>(c => c.CES_DigitoVerificacion),
                CES_Direccion = record.Field<CentroServicios_PUA, string>(c => c.CES_Direccion),
                CES_Email = record.Field<CentroServicios_PUA, string>(c => c.CES_Email),
                CES_Estado = record.Field<CentroServicios_PUA, string>(c => c.CES_Estado),
                CES_Fax = record.Field<CentroServicios_PUA, string>(c => c.CES_Fax),
                CES_FechaCambio = DateTime.Now,
                CES_FechaGrabacion = record.Field<CentroServicios_PUA, DateTime>(c => c.CES_FechaGrabacion),
                CES_IdCentroCostos = record.Field<CentroServicios_PUA, string>(c => c.CES_IdCentroCostos),
                CES_IdCentroServicios = record.Field<CentroServicios_PUA, long>(c => c.CES_IdCentroServicios),
                CES_IdMunicipio = record.Field<CentroServicios_PUA, string>(c => c.CES_IdMunicipio),
                CES_IdPersonaResponsable = record.Field<CentroServicios_PUA, long>(c => c.CES_IdPersonaResponsable),
                CES_IdPropietario = record.Field<CentroServicios_PUA, int>(c => c.CES_IdPropietario),
                CES_IdZona = record.Field<CentroServicios_PUA, string>(c => c.CES_IdZona),
                CES_Latitud = record.Field<CentroServicios_PUA, decimal?>(c => c.CES_Latitud),
                CES_Longitud = record.Field<CentroServicios_PUA, decimal?>(c => c.CES_Longitud),
                CES_Nombre = record.Field<CentroServicios_PUA, string>(c => c.CES_Nombre),
                CES_Telefono1 = record.Field<CentroServicios_PUA, string>(c => c.CES_Telefono1),
                CES_Telefono2 = record.Field<CentroServicios_PUA, string>(c => c.CES_Telefono2),

                CES_ClasGirosPorIngresos = record.Field<CentroServicios_PUA, string>(c => c.CES_ClasGirosPorIngresos),
                CES_PuedePagarGiros = record.Field<CentroServicios_PUA, bool>(c => c.CES_PuedePagarGiros),
                CES_PuedeRecibirGiros = record.Field<CentroServicios_PUA, bool>(c => c.CES_PuedeRecibirGiros),
                CES_TopeMaximoPorGiros = record.Field<CentroServicios_PUA, decimal>(c => c.CES_TopeMaximoPorGiros),

                CES_TipoCambio = action.ToString(),
                CES_IdTipoPropiedad = record.Field<CentroServicios_PUA, short>(c => c.CES_IdTipoPropiedad),
                CES_Sistematizada = record.Field<CentroServicios_PUA, bool>(c => c.CES_Sistematizada),
                CES_Tipo = record.Field<CentroServicios_PUA, string>(c => c.CES_Tipo),
                CES_CambiadoPor = ControllerContext.Current.Usuario,
                CES_CodigoSPN = record.Field<CentroServicios_PUA, string>(c => c.CES_CodigoSPN),
                CES_AdmiteFormaPagoAlCobro = record.Field<CentroServicios_PUA, bool>(c => c.CES_AdmiteFormaPagoAlCobro),
                CES_PesoMaximo = record.Field<CentroServicios_PUA, decimal>(c => c.CES_PesoMaximo),
                CES_TopeMaximoPorPagos = record.Field<CentroServicios_PUA, decimal>(c => c.CES_TopeMaximoPorPagos),
                CES_VendePrepago = record.Field<CentroServicios_PUA, bool>(c => c.CES_VendePrepago),
                CES_VolumenMaximo = record.Field<CentroServicios_PUA, decimal>(c => c.CES_VolumenMaximo),
                CES_NombreAMostrar = record.Field<CentroServicios_PUA, string>(c => c.CES_NombreAMostrar),
                CES_FechaApertura = record.Field<CentroServicios_PUA, DateTime?>(c => c.CES_FechaApertura),
                CES_FechaCierre = record.Field<CentroServicios_PUA, DateTime?>(c => c.CES_FechaCierre)
            }, (cs) => contexto.CentroServiciosHist_PUA.Add(cs));
        }

        #endregion Auditar

        /// <summary>
        /// Consulta los tipos de agencia
        /// </summary>
        /// <returns>Lista con los tipos de agencia</returns>
        public IList<PUTipoAgencia> ObtenerTiposAgencia()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoAgencia_PUA.Select(obj =>
                  new PUTipoAgencia()
                  {
                      Descripcion = obj.TIA_Descripcion,
                      IdTipo = obj.TIA_IdTipo
                  }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene los tipos de propiedad
        /// </summary>
        /// <returns></returns>
        public IList<PUTipoPropiedad> ObtenerTiposPropiedad()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoPropiedad_PUA.Select(obj =>
                  new PUTipoPropiedad()
                  {
                      Descripcion = obj.TPR_Descripcion,
                      IdTipoPropiedad = obj.TPR_IdTipoPropiedad
                  }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }



        /// <summary>
        /// Obtiene el id y la descripcion de todos los centros logisticos activos y racol activos
        /// </summary>
        /// <returns>lista de centros logisticos y racol </returns>
        public IList<PUCentroServicioApoyo> ObtenerCentrosServicioApoyo()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var COL = contexto.CentroLogistico_PUA.Include("CentroServicios_PUA").Where
                  (obj =>
                    obj.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO).Select(obj =>
                      new PUCentroServicioApoyo()
                      {
                          IdRacol = obj.CEL_IdRegionalAdm,
                          IdCentroservicio = obj.CEL_IdCentroLogistico,
                          NombreCentroServicio = obj.CentroServicios_PUA.CES_Nombre,
                          TipoCentroServicio = ConstantesFramework.TIPO_CENTRO_SERVICIO_COL
                      }).ToList();

                var RACOL = contexto.RegionalAdministrativa_PUA.Include("CentroServicios_PUA").Where
                  (obj =>
                    obj.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO && obj.CentroServicios_PUA.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL).Select(obj =>
                      new PUCentroServicioApoyo()
                      {
                          IdRacol = obj.REA_IdRegionalAdm,
                          IdCentroservicio = obj.REA_IdRegionalAdm,
                          NombreCentroServicio = obj.CentroServicios_PUA.CES_Nombre,
                          TipoCentroServicio = ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL
                      }).ToList();

                return COL.Union(RACOL).OrderBy(obj => obj.NombreCentroServicio).ToList();
            }
        }

        /// <summary>
        /// Obtiene la lista de Territoriales
        /// </summary>
        /// <returns></returns>
        public IList<PUTerritorialDC> ObtenerTerritoriales()
        {
            IList<PUTerritorialDC> lstTerritorial = new List<PUTerritorialDC>();

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                cn.Open();
                SqlCommand conn = new SqlCommand("paObtenerTerritoriales_PUA", cn);
                conn.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = conn.ExecuteReader();
                while (reader.Read())
                {
                    PUTerritorialDC objTerritorial = new PUTerritorialDC();

                    objTerritorial.IdTerritorial = Convert.ToInt32(reader["TER_IdTerritorial"]);
                    objTerritorial.NombreTerritorial = reader["TER_NombreTerritorial"].ToString();

                    lstTerritorial.Add(objTerritorial);
                }
            }
            return lstTerritorial;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<string> ObtenerCenCostoNovasoft()
        {
            List<string> lstCenCosto = new List<string>();

            using (SqlConnection cn = new SqlConnection(conexionString))
            {
                SqlCommand com = new SqlCommand("spObtenerCentrosCostoNovasoft_ADM", cn);
                com.CommandType = CommandType.StoredProcedure;
                cn.Open();
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cn.Close();
                lstCenCosto = dt.AsEnumerable().ToList().ConvertAll<string>(r =>
                {
                    string dir = r["CentroCosto"].ToString();
                    return dir;
                });

            }


            return lstCenCosto;
        }


        /// <summary>
        /// Metodo que consulta el estado
        /// activo del centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public bool ObtenerCentroServicioActivo(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int total = contexto.CentroServicios_PUA.Count(cen => cen.CES_IdCentroServicios == idCentroServicio && cen.CES_Estado == ConstantesFramework.ESTADO_ACTIVO);
                return (total != 0);
            }
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio Activos de una localidad
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerCentrosServicioActivosLocalidad(string idMunicipio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicios_PUA.Where(centro =>
                  centro.CES_Estado == ConstantesFramework.ESTADO_ACTIVO && centro.CES_IdMunicipio == idMunicipio)
                  .ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      Direccion = r.CES_Direccion,
                      Barrio = r.CES_Barrio,
                      Email = r.CES_Email,
                      Fax = r.CES_Fax,
                      IdMunicipio = r.CES_IdMunicipio,
                      IdPropietario = r.CES_IdPropietario,
                      CiudadUbicacion = new PALocalidadDC()
                      {
                          IdLocalidad = r.CES_IdMunicipio,
                      },

                      DigitoVerificacion = r.CES_DigitoVerificacion,
                      IdTipoPropiedad = r.CES_IdTipoPropiedad,
                      Estado = r.CES_Estado,
                      IdCentroCostos = r.CES_IdCentroCostos,
                      IdCentroServicio = r.CES_IdCentroServicios,
                      IdPersonaResponsable = r.CES_IdPersonaResponsable,
                      IdZona = r.CES_IdZona,
                      Latitud = r.CES_Latitud,
                      Longitud = r.CES_Longitud,
                      Nombre = r.CES_Nombre,
                      Sistematizado = r.CES_Sistematizada,
                      Telefono1 = r.CES_Telefono1,
                      Telefono2 = r.CES_Telefono2,
                      Tipo = r.CES_Tipo,
                      TipoOriginal = r.CES_Tipo,
                      AdmiteFormaPagoAlCobro = r.CES_AdmiteFormaPagoAlCobro,
                      PesoMaximo = r.CES_PesoMaximo,
                      VendePrepago = r.CES_VendePrepago,
                      VolumenMaximo = r.CES_VolumenMaximo,
                      TopeMaximoGiros = r.CES_TopeMaximoPorGiros,
                      PagaGiros = r.CES_PuedePagarGiros,
                      RecibeGiros = r.CES_PuedeRecibirGiros,
                      IdClasificadorCanalVenta = r.CES_IdClasificadorCanalVenta,
                      CodigoBodega = r.CES_CodigoBodega,
                      NombreAMostrar = r.CES_NombreAMostrar,
                      FechaApertura = r.CES_FechaApertura,
                      FechaCierre = r.CES_FechaCierre,
                  });
            }
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio de una actividad
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosCentrosServicioPorLocalidad(string idMunicipio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicios_PUA.Where(centro => centro.CES_IdMunicipio == idMunicipio).OrderBy(r => r.CES_Estado)
                  .ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      Direccion = r.CES_Direccion,
                      Barrio = r.CES_Barrio,
                      Email = r.CES_Email,
                      NombreCodigo = r.CES_IdCentroServicios.ToString() + "-" + r.CES_Nombre,
                      Fax = r.CES_Fax,
                      IdMunicipio = r.CES_IdMunicipio,
                      IdPropietario = r.CES_IdPropietario,
                      CiudadUbicacion = new PALocalidadDC()
                      {
                          IdLocalidad = r.CES_IdMunicipio,
                      },

                      DigitoVerificacion = r.CES_DigitoVerificacion,
                      IdTipoPropiedad = r.CES_IdTipoPropiedad,
                      Estado = r.CES_Estado,
                      IdCentroCostos = r.CES_IdCentroCostos,
                      IdCentroServicio = r.CES_IdCentroServicios,
                      IdPersonaResponsable = r.CES_IdPersonaResponsable,
                      IdZona = r.CES_IdZona,
                      Latitud = r.CES_Latitud,
                      Longitud = r.CES_Longitud,
                      Nombre = r.CES_Nombre + " -- " + r.CES_Estado,
                      Sistematizado = r.CES_Sistematizada,
                      Telefono1 = r.CES_Telefono1,
                      Telefono2 = r.CES_Telefono2,
                      Tipo = r.CES_Tipo,
                      TipoOriginal = r.CES_Tipo,
                      AdmiteFormaPagoAlCobro = r.CES_AdmiteFormaPagoAlCobro,
                      PesoMaximo = r.CES_PesoMaximo,
                      VendePrepago = r.CES_VendePrepago,
                      VolumenMaximo = r.CES_VolumenMaximo,
                      TopeMaximoGiros = r.CES_TopeMaximoPorGiros,
                      PagaGiros = r.CES_PuedePagarGiros,
                      RecibeGiros = r.CES_PuedeRecibirGiros,
                      IdClasificadorCanalVenta = r.CES_IdClasificadorCanalVenta,
                      CodigoBodega = r.CES_CodigoBodega,
                      NombreAMostrar = r.CES_NombreAMostrar,
                      FechaApertura = r.CES_FechaApertura,
                      FechaCierre = r.CES_FechaCierre,
                  });
            }
        }


        /// <summary>
        /// obtener todos los tipos de ciudad
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCiudad> ObtenerTiposCiudades()
        {
            List<PUTipoCiudad> tiposCiudades = new List<PUTipoCiudad>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposCiudades_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    tiposCiudades = MERepositorioMapper.ToListTipoCiudad(reader);
                }

            }
            return tiposCiudades;

        }

        /// <summary>
        /// obtener todos los tipos de zona
        /// </summary>
        /// <returns></returns>
        public List<PUTipoZona> ObtenerTiposZona()
        {
            List<PUTipoZona> tiposZona = new List<PUTipoZona>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposZonas_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    tiposZona = MERepositorioMapper.ToListTipoZona(reader);
                }
            }
            return tiposZona;
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio que no son Racol con el servicio Komprech activado para una localidad dada
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasPuntosActivosKomprechPorLocalidad(string idMunicipio, int servicioKomprech)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicioServicio_PUA.Include("CentroServicios_PUA").Where(centro =>
                      centro.CSS_IdServicio == servicioKomprech &&
                      centro.CentroServicios_PUA.CES_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                      centro.CentroServicios_PUA.CES_IdMunicipio == idMunicipio
                      && centro.CentroServicios_PUA.CES_Tipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
                   .ToList()
                   .ConvertAll(r => new PUCentroServiciosDC()
                   {
                       Direccion = r.CentroServicios_PUA.CES_Direccion,
                       IdCentroCostos = r.CentroServicios_PUA.CES_IdCentroCostos,
                       IdCentroServicio = r.CentroServicios_PUA.CES_IdCentroServicios,
                       Nombre = r.CentroServicios_PUA.CES_Nombre,
                       Telefono1 = r.CentroServicios_PUA.CES_Telefono1,
                       Tipo = r.CentroServicios_PUA.CES_Tipo
                   });
            }
        }

        /// <summary>
        /// Método que devuelve una lista con todos los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasPuntosActivosPorLocalidad(string idMunicipio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicios_PUA.Where(centro =>
                  centro.CES_Estado == ConstantesFramework.ESTADO_ACTIVO && centro.CES_IdMunicipio == idMunicipio
                  && centro.CES_Tipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL)
                  .ToList()
                  .ConvertAll(r => new PUCentroServiciosDC()
                  {
                      Direccion = r.CES_Direccion,
                      Barrio = r.CES_Barrio,
                      Email = r.CES_Email,
                      Fax = r.CES_Fax,
                      IdMunicipio = r.CES_IdMunicipio,
                      IdPropietario = r.CES_IdPropietario,
                      CiudadUbicacion = new PALocalidadDC()
                      {
                          IdLocalidad = r.CES_IdMunicipio,
                      },

                      DigitoVerificacion = r.CES_DigitoVerificacion,
                      IdTipoPropiedad = r.CES_IdTipoPropiedad,
                      Estado = r.CES_Estado,
                      IdCentroCostos = r.CES_IdCentroCostos,
                      IdCentroServicio = r.CES_IdCentroServicios,
                      IdZona = r.CES_IdZona,
                      Latitud = r.CES_Latitud,
                      Longitud = r.CES_Longitud,
                      Nombre = r.CES_Nombre,
                      Telefono1 = r.CES_Telefono1,
                      Tipo = r.CES_Tipo
                  });
            }
        }

        /// <summary>
        /// Obtiene los estados para los centros de servicio
        /// </summary>
        /// <returns></returns>
        public IList<PUEstadoDC> ObtenerEstados()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoActivoInactivo_VFRM.Select(obj => new PUEstadoDC()
                {
                    Descripcion = obj.Estado,
                    IdEstado = obj.IdEstado
                }).OrderBy(obj => obj.Descripcion).ToList();
            }
        }

        /// <summary>
        /// Obtiene lista de los horarios de un centro de servicio
        /// </summary>
        /// <returns></returns>
        public IList<PADia> ObtienerHorariosCentroServicios(long idCentroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var horariosCentroServ = contexto.HorarioCentroServicio_PUA.Where(h => h.HCS_IdCentroServicios == idCentroServicios);

                var dias = contexto.Dia_PAR.ToList().ConvertAll<PADia>(obj =>
                {
                    PADia dia = new PADia()
                    {
                        IdDia = obj.DIA_IdDia.Trim(),
                        NombreDia = obj.DIA_NombreDia
                    };

                    return dia;
                });

                return dias;
            }
        }

        #region Archivos

        /// <summary>
        /// Obtiene lista con los archivos de los centros de servicio
        /// </summary>
        /// <returns>objeto de centro de servicio</returns>
        public IEnumerable<PUArchivoCentroServicios> ObtenerArchivosCentroServicios(PUCentroServiciosDC centroServicios)
        {
            List<PUArchivoCentroServicios> listaCentroServicio = new List<PUArchivoCentroServicios>();

            string query = @"SELECT  *  FROM  dbo.ArchivosCentroServicios_PUA " +
                     " WHERE ACS_IdCentroServicios = " + centroServicios.IdCentroServicio;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        listaCentroServicio.Add
                            (
                            new PUArchivoCentroServicios()
                            {
                                IdCentroServicios = Convert.ToInt64(r["ACS_IdCentroServicios"]),
                                IdArchivo = Convert.ToInt64(r["ACS_IdArchivosCentroServicios"]),
                                IdDocumento = Convert.ToInt16(r["ACS_IdDocumento"]),
                                Fecha = Convert.ToDateTime(r["ACS_FechaGrabacion"]),
                                IdAdjunto = new Guid(r["ACS_IdAdjunto"].ToString()),
                                NombreAdjunto = r["ACS_NombreAdjunto"].ToString(),
                                EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                            }
                            );
                    };
                }
                sqlConn.Close();
            }
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var listadocumentos = contexto.DocumentoCentroServicio_PUA
                .Where(t => t.DCS_Estado != ConstantesFramework.ESTADO_INACTIVO && t.DCS_Tipo == PUConstantesCentroServicios.TipoDocumentosCentroServicios)
                .OrderByDescending(o => o.DCS_IdDocumento)
                .ToList()
                .ConvertAll<PUArchivoCentroServicios>(r => new PUArchivoCentroServicios()
                {
                    IdDocumento = r.DCS_IdDocumento,
                    NombreDocumento = r.DCS_Nombre,
                    EstadoDocumento = r.DCS_Estado,
                });

                foreach (PUArchivoCentroServicios documento in listadocumentos)
                {
                    foreach (PUArchivoCentroServicios archivo in listaCentroServicio)
                    {
                        if (documento.IdDocumento == archivo.IdDocumento)
                        {
                            documento.IdAdjunto = archivo.IdAdjunto;
                            documento.IdArchivo = archivo.IdArchivo;
                            documento.IdCentroServicios = archivo.IdCentroServicios;
                            documento.NombreAdjunto = archivo.NombreAdjunto;
                            documento.Fecha = archivo.Fecha;
                            documento.EstadoRegistro = archivo.EstadoRegistro;
                        }
                    }
                }

                return listadocumentos;
            }
        }

        /// <summary>
        /// Adiciona archivo de un centro de servicio
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void AdicionarArchivoCentroServicio(PUArchivoCentroServicios archivo, long idCentroServicios)
        {
            filePath = PAAdministrador.Instancia.ObtenerParametrosAplicacion(PAAdministrador.RutaArchivos);
            string rutaArchivo = Path.Combine(this.filePath, COConstantesModulos.CLIENTES, archivo.NombreServidor);
            byte[] archivoImagen;
            using (FileStream fs = File.OpenRead(rutaArchivo))
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                archivoImagen = bytes;
            }

            string query = @"INSERT INTO [ArchivosCentroServicios_PUA] WITH (ROWLOCK)" +
            " ([ACS_Adjunto],[ACS_IdCentroServicios] ,[ACS_IdDocumento]  ,[ACS_IdAdjunto]  ,[ACS_NombreAdjunto] ,[ACS_FechaGrabacion] ,[ACS_CreadoPor])  " +
           " VALUES(@Adjunto , @IdCentroServicios ,@IdDocumento ,@IdAdjunto,@NombreAdjunto ,GETDATE() ,@CreadoPor)";

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.Parameters.Add(new SqlParameter("@NombreAdjunto", archivo.NombreAdjunto));
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicios", idCentroServicios));
                cmd.Parameters.Add(new SqlParameter("@IdAdjunto", Guid.NewGuid()));
                cmd.Parameters.Add(new SqlParameter("@IdDocumento", archivo.IdDocumento));
                cmd.Parameters.Add(new SqlParameter("@Adjunto", (object)archivoImagen));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", ControllerContext.Current.Usuario));

                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
        }

        /// <summary>
        /// Elimina archivo de un centro de servicio
        /// </summary>
        /// <param name="archivo">objeto de tipo archivo</param>
        public void EliminarArchivoCentroServicio(PUArchivoCentroServicios archivo)
        {
            string query = @"DELETE FROM [ArchivosCentroServicios_PUA] WITH (ROWLOCK)" +
            "WHERE  ACS_IdArchivosCentroServicios = " + archivo.IdArchivo;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                cmd.ExecuteNonQuery();
                sqlConn.Close();
            }
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ArchivosPropietario_PUA archivoBd = contexto.ArchivosPropietario_PUA.FirstOrDefault(ar => ar.APR_IdArchivosPersonas == archivo.IdArchivo);
                if (archivoBd != null)
                {
                    contexto.ArchivosPropietario_PUA.Remove(archivoBd);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        ///Metodo para obtener un archivo asociado a un centro Servicio
        /// </summary>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ObtenerArchivoCentroServicio(PUArchivoCentroServicios archivo)
        {
            string respuesta;
            string query = @"SELECT  dbo.ArchivosCentroServicios_PUA.ACS_Adjunto  FROM  dbo.ArchivosCentroServicios_PUA" +
                     " WHERE ACS_IdArchivosCentroServicios = " + archivo.IdArchivo;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringArchivo))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
                {
                    respuesta = string.Empty;
                }
                else
                    respuesta = Convert.ToBase64String(dt.Rows[0]["ACS_Adjunto"] as byte[]);

                sqlConn.Close();
                return respuesta;
            }
        }

        #endregion Archivos

        #region Asignacion de Suministros

        /// <summary>
        /// Obtiene suministros de un centro de servicio
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="IdCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista suministros del centro de servicio</returns>
        public IList<PUCentroServiciosSuministro> ObtenerSuministrosPorCentrosServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long IdCentroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<CentroServicioSuministro_VPUA>("CSS_IdCentroServicios", IdCentroServicios.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarCentroServicioSuministro_VPUA(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList().ConvertAll<PUCentroServiciosSuministro>(s =>
                    new PUCentroServiciosSuministro()
                    {
                        NombreSuministro = s.SUM_Descripcion,
                        IdSuministro = s.SCS_IdSuministro,
                        IdCentroServicios = s.SCS_IdCentroServicios,
                        CantidadAsignada = s.SCS_CantidadInicialAutorizada,
                        IdCentroServicioSuministro = s.SCS_IdSuministroCentroServicio,
                        StockMinimo = s.SCS_StockMinimo
                    });
            }
        }

        /// <summary>
        /// Adiciona un CentroServiciosSuministro
        /// </summary>
        /// <param name="centroSerSuministro">Objeto CentroServiciosSuministro</param>
        public void AdicionarCentroServiciosSuministro(PUCentroServiciosSuministro centroSerSuministro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosCentroServicios_SUM centroServiSum = new SuministrosCentroServicios_SUM()
                {
                    //CSS_CantidadAsignada = centroSerSuministro.CantidadAsignada,
                    SCS_IdCentroServicios = centroSerSuministro.IdCentroServicios,
                    SCS_IdSuministro = centroSerSuministro.IdSuministro,
                    SCS_StockMinimo = centroSerSuministro.StockMinimo,
                    SCS_CreadoPor = ControllerContext.Current.Usuario,
                    SCS_FechaGrabacion = DateTime.Now
                };
                contexto.SuministrosCentroServicios_SUM.Add(centroServiSum);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un CentroServiciosSuministro
        /// </summary>
        /// <param name="centroServiciosuministro">Objeto CentroServiciosSuministro</param>
        public void EditarCentroServiciosSuministro(PUCentroServiciosSuministro centroServiciosuministro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosCentroServicios_SUM sumin = contexto.SuministrosCentroServicios_SUM
                  .Where(obj => obj.SCS_IdSuministroCentroServicio == centroServiciosuministro.IdCentroServicioSuministro)
                  .SingleOrDefault();

                if (sumin == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                //sumin.CSS_CantidadAsignada = centroServiciosuministro.CantidadAsignada;
                sumin.SCS_StockMinimo = centroServiciosuministro.StockMinimo;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un CentroServiciosSuministro
        /// </summary>
        /// <param name="centroServiciosuministro">Objeto CentroServiciosSuministro</param>
        public void EliminarCentroServiciosSuministro(PUCentroServiciosSuministro centroServiciosuministro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                SuministrosCentroServicios_SUM ban = contexto.SuministrosCentroServicios_SUM.Where(obj => obj.SCS_IdSuministroCentroServicio == centroServiciosuministro.IdCentroServicioSuministro).SingleOrDefault();
                if (ban != null)
                {
                    contexto.SuministrosCentroServicios_SUM.Remove(ban);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene todos los suministros
        /// </summary>
        /// <returns></returns>
        public IList<PUSuministro> ObtenerTodosSuministros()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Suministro_SUM.Select(s => new PUSuministro()
                {
                    Descripcion = s.SUM_Descripcion,
                    IdSuministro = s.SUM_IdSuministro,

                    //StockMinimo = s.SUM_StockMinimoReferencia
                }).OrderBy(s => s.Descripcion).ToList();
            }
        }

        #endregion Asignacion de Suministros

        #region Divulgacion agencia

        /// <summary>
        /// Obtiene los horarios de un centro de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns>Lista con los horarios de un centro de servicios</returns>
        public IList<PUHorariosCentroServicios> ObtenerHorariosCentroServicios(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var horarios = contexto.Dia_PAR.Join(contexto.HorarioCentroServicio_PUA.Where
                  (h => h.HCS_IdCentroServicios == idCentroServicio)
                  , d => d.DIA_IdDia, h => h.HCS_IdDia, (d, h) =>
                    new PUHorariosCentroServicios()
                    {
                        HoraFin = h.HCS_HoraFin,
                        HoraInicio = h.HCS_HoraInicio,
                        IdCentroServicios = idCentroServicio,
                        IdDia = h.HCS_IdDia,
                        IdHorarioCentroServicios = h.HCS_IdHorarioCentroServicio,
                        NombreDia = d.DIA_NombreDia
                    }).ToList();
                return horarios;
            }
        }

        /// <summary>
        /// Obtiene los horarios de un centro de servicios para la app de recogidas
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {

            using (SqlConnection conn = new SqlConnection(this.conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioCentroServicioAppRecogidas_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@HCS_IdCentroServicios", idCentroServicio);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                List<string> horarios = new List<string>();
                while (reader.Read())
                {

                    horarios.Add(reader["Horario"].ToString());
                }
                conn.Close();

                return horarios;
            }
        }

        /// <summary>
        /// Obtiene  los servicios asignados a un centro de servicios
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCentroServicios">Id del centro de servicio</param>
        /// <returns>Lista  de servicios de un centro de servicios</returns>
        public IList<CMServiciosCentroServicios> ObtenerServiciosCentroServicios(long idCentroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var srv = contexto.CentroServiciosSrvUniNeg_VCOM.Where(c => c.CSS_IdCentroServicios == idCentroServicios).ToList();
                var srvUnicos = srv.GroupBy(s => s.CSS_IdServicio).Select(s => s.First()).ToList();
                var servicios = srvUnicos.ConvertAll<CMServiciosCentroServicios>(s =>
                {
                    CMServiciosCentroServicios ser = new CMServiciosCentroServicios()
                    {
                        Estado = s.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                        FechaInicioVenta = s.CSS_FechaInicioVenta,
                        IdCentroServiciosServicio = s.CSS_IdCentroServicioServicio,
                        Servicios = new TAServicioDC()
                        {
                            IdUnidadNegocio = s.SER_IdUnidadNegocio,
                            UnidadNegocio = s.UNE_Descripcion,
                            IdServicio = s.CSS_IdServicio,
                            Nombre = s.SER_Nombre
                        },
                        IdCentroServicios = idCentroServicios,
                    };
                    return ser;
                });

                return servicios;
            }
        }

        #endregion Divulgacion agencia

        /// <summary>
        /// Metodo para Obtener el RACOL
        /// </summary>
        /// <returns></returns>
        public List<PURegionalAdministrativa> ObtenerRegionalAdministrativa()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RegionalAdministrativa_PUA
                  .Include("CentroServicios_PUA")
                  .Where(r => r.CentroServicios_PUA.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .OrderBy(o => o.REA_Descripcion)
                  .ToList()
                  .ConvertAll<PURegionalAdministrativa>(reg => new PURegionalAdministrativa()
                  {
                      IdRegionalAdmin = reg.REA_IdRegionalAdm,
                      Descripcion = reg.REA_Descripcion,
                      IdCentroCosto = reg.CentroServicios_PUA.CES_IdCentroCostos,
                      CentroServicios = new PUCentroServiciosDC()
                      {
                          CiudadUbicacion = new PALocalidadDC()
                          {
                              IdLocalidad = reg.CentroServicios_PUA.CES_IdMunicipio
                          }
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene los centros logisticos
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioApoyo> ObtenerCentroLogistico()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroLogistico_PUA.Include("CentroServicios_PUA").Where
                  (obj =>
                    obj.CentroServicios_PUA.CES_Estado == Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO).Select(obj =>
                      new PUCentroServicioApoyo()
                      {
                          IdCentroservicio = obj.CEL_IdCentroLogistico,
                          NombreCentroServicio = obj.CentroServicios_PUA.CES_Nombre,
                          TipoCentroServicio = ConstantesFramework.TIPO_CENTRO_SERVICIO_COL
                      }).ToList();
            }
        }

        /// <summary>
        /// Obtiene los COL de un Racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>lista de Col de un Racol</returns>
        public List<PUCentroServicioApoyo> ObtenerCentrosLogisticosRacol(long idRacol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroLogisDeRegionalAdmi_VPUA.Where(rac => rac.REA_IdRegionalAdm == idRacol).ToList()
                    .ConvertAll<PUCentroServicioApoyo>(n => new PUCentroServicioApoyo()
                    {
                        IdCentroservicio = n.CEL_IdCentroLogistico,
                        NombreCentroServicio = n.CES_Nombre,
                    });
            }
        }

        /// <summary>
        /// Metodo para Obtener la RACOL de un municipio
        /// </summary>
        /// <returns></returns>
        public PURegionalAdministrativa ObtenerRegionalAdministrativa(string idMunicipio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MunicipioRegionalAdm_PUA racol = contexto.MunicipioRegionalAdm_PUA.Include("RegionalAdministrativa_PUA").Where(reg => reg.MRA_IdMunicipio == idMunicipio).FirstOrDefault();

                if (racol != null)
                {
                    return new PURegionalAdministrativa()
                    {
                        IdRegionalAdmin = racol.RegionalAdministrativa_PUA.REA_IdRegionalAdm,
                        Descripcion = racol.RegionalAdministrativa_PUA.REA_Descripcion
                    };
                }
                else
                {
                    return new PURegionalAdministrativa()
                    {
                        IdRegionalAdmin = 0,
                        Descripcion = ""
                    };
                }
            }
        }

        /// <summary>
        /// Obtiene todas las opciones de clasificador canal ventas
        /// </summary>
        /// <returns></returns>
        public IList<PUClasificadorCanalVenta> ObtenerTodosClasificadorCanalVenta()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ClasificadorCanalVenta_PUA.ToList().ConvertAll(c =>
                  new PUClasificadorCanalVenta()
                  {
                      IdClasificadorCanalVentas = (int)c.CCV_IdClasificadorCanalVenta,
                      ClasificadorCanalVenta = c.CCV_ClasificadorCanalVenta,
                      Nombre = c.CCV_Nombre,
                      IdTipoCentroServicios = c.CCV_IdTipoCentroServicos,
                      IdTipoPropiedad = (int)c.CCV_IdTipoPropiedad
                  }).ToList();
            }
        }

        /// <summary>
        /// Obtiene el centro de servicio responsable de un municipio
        /// </summary>
        /// <param name="centroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCOLResponsableMunicipio(string idMunicipío)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                MunicipioCentroLogistico_VPUA Col =
                    contexto.MunicipioCentroLogistico_VPUA
                    .FirstOrDefault(mun => mun.MCL_IdLocalidad == idMunicipío);
                if (Col != null)
                {
                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Col.MCL_IdCentroLogistico,
                        IdMunicipio = Col.MCL_IdLocalidad,
                        NombreMunicipio = Col.LOC_Nombre,
                        Nombre = Col.CES_Nombre,
                        Direccion = Col.CES_Direccion,
                        Telefono1 = Col.CES_Telefono1
                    };
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                     EnumTipoErrorCentroServicios.EX_NO_RACOL_EN_CIUDAD.ToString(),
                     MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_NO_RACOL_EN_CIUDAD)));
            }
        }

        /// <summary>
        /// Validar si una ciudad se apoya en un col
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <param name="IdCol"></param>
        /// <returns></returns>
        public bool ValidarCiudadSeApoyaCOL(string idLocalidad, long idCol)
        {

            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paValidarSiCiudadSeApoyaCOL_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", idLocalidad);
                cmd.Parameters.AddWithValue("@IdCentroLogistico", idCol);
                conn.Open();
                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }

        public List<string> ConsultarRacoles()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RegionalAdministrativa_VCPO.ToList().ConvertAll(reg => string.Join(";", new string[] { reg.CES_IdCentroServicios.ToString(), reg.REA_Descripcion.Replace(';', ' ').Replace(',', ' ') }));
            }
        }

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciayPuntosActivos()
        {
            List<PUCentroServiciosDC> lstCentrosServicio = new List<PUCentroServiciosDC>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodasAgenciasyPuntosActivos_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader read = cmd.ExecuteReader();
                PUCentroServiciosDC centroSer;

                while (read.Read())
                {
                    centroSer = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Convert.ToInt64(read["CES_IdCentroServicios"]),
                        Tipo = read["CES_Tipo"].ToString(),
                        DigitoVerificacion = read["CES_DigitoVerificacion"].ToString(),
                        IdPropietario = Convert.ToInt32(read["CES_IdPropietario"]),
                        Nombre = read["CES_Nombre"].ToString(),
                        Telefono1 = read["CES_Telefono1"].ToString(),
                        Telefono2 = read["CES_Telefono2"].ToString(),
                        Fax = read["CES_Fax"].ToString(),
                        Direccion = read["CES_Direccion"].ToString(),
                        Barrio = read["CES_Barrio"].ToString(),
                        IdZona = read["CES_IdZona"].ToString(),
                        IdMunicipio = read["CES_IdMunicipio"].ToString(),
                        Estado = read["CES_Estado"].ToString(),
                        IdPersonaResponsable = Convert.ToInt64(read["CES_IdPersonaResponsable"]),
                        Latitud = read["CES_Latitud"] != DBNull.Value ? Convert.ToDecimal(read["CES_Latitud"]) : 0,
                        Longitud = read["CES_Longitud"] != DBNull.Value ? Convert.ToDecimal(read["CES_Longitud"]) : 0,
                        Email = read["CES_Email"].ToString(),
                        Sistematizado = Convert.ToBoolean(read["CES_Sistematizada"]),
                        IdCentroCostos = read["CES_IdCentroCostos"].ToString(),
                        IdTipoPropiedad = Convert.ToInt32(read["CES_IdTipoPropiedad"]),
                        PesoMaximo = Convert.ToDecimal(read["CES_PesoMaximo"]),
                        VolumenMaximo = Convert.ToDecimal(read["CES_VolumenMaximo"]),
                        AdmiteFormaPagoAlCobro = Convert.ToBoolean(read["CES_AdmiteFormaPagoAlCobro"]),
                        VendePrepago = Convert.ToBoolean(read["CES_VendePrepago"]),
                        NombreAMostrar = read["CES_NombreAMostrar"].ToString()
                    };
                    lstCentrosServicio.Insert(0, centroSer);
                }
                conn.Close();
            }

            return lstCentrosServicio;

        }


        /// <summary>
        /// Obtiene todas las agencias, col y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerTodosAgenciaColPuntosActivos()
        {
            List<PUCentroServiciosDC> lstCentrosServicio = new List<PUCentroServiciosDC>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTodasAgenciasColesPuntosActivos_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader read = cmd.ExecuteReader();
                PUCentroServiciosDC centroSer;

                while (read.Read())
                {
                    centroSer = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = Convert.ToInt64(read["CES_IdCentroServicios"]),
                        Tipo = read["CES_Tipo"].ToString(),
                        DigitoVerificacion = read["CES_DigitoVerificacion"].ToString(),
                        IdPropietario = Convert.ToInt32(read["CES_IdPropietario"]),
                        Nombre = read["CES_Nombre"].ToString(),
                        Telefono1 = read["CES_Telefono1"].ToString(),
                        Telefono2 = read["CES_Telefono2"].ToString(),
                        Fax = read["CES_Fax"].ToString(),
                        Direccion = read["CES_Direccion"].ToString(),
                        Barrio = read["CES_Barrio"].ToString(),
                        IdZona = read["CES_IdZona"].ToString(),
                        IdMunicipio = read["CES_IdMunicipio"].ToString(),
                        Estado = read["CES_Estado"].ToString(),
                        IdPersonaResponsable = Convert.ToInt64(read["CES_IdPersonaResponsable"]),
                        Latitud = read["CES_Latitud"] != DBNull.Value ? Convert.ToDecimal(read["CES_Latitud"]) : 0,
                        Longitud = read["CES_Longitud"] != DBNull.Value ? Convert.ToDecimal(read["CES_Longitud"]) : 0,
                        Email = read["CES_Email"].ToString(),
                        Sistematizado = Convert.ToBoolean(read["CES_Sistematizada"]),
                        IdCentroCostos = read["CES_IdCentroCostos"].ToString(),
                        IdTipoPropiedad = Convert.ToInt32(read["CES_IdTipoPropiedad"]),
                        PesoMaximo = Convert.ToDecimal(read["CES_PesoMaximo"]),
                        VolumenMaximo = Convert.ToDecimal(read["CES_VolumenMaximo"]),
                        AdmiteFormaPagoAlCobro = Convert.ToBoolean(read["CES_AdmiteFormaPagoAlCobro"]),
                        VendePrepago = Convert.ToBoolean(read["CES_VendePrepago"]),
                        NombreAMostrar = read["CES_NombreAMostrar"].ToString()
                    };
                    lstCentrosServicio.Insert(0, centroSer);
                }
                conn.Close();
            }

            return lstCentrosServicio;
        }


        #endregion Centro de servicios

        #region Validación Suministros

        /// <summary>
        /// Metodo para validar la provisión de un suministro en un centro de servicio
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="idSuministro"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns> objeto de tipo suministro </returns>
        public SUSuministro ValidarSuministroSerial(long serial, int idSuministro, long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var a = contexto.paValidarSuministroSerial_PUA(serial, idSuministro, idCentroServicio).FirstOrDefault();
                if (a != null)
                {
                    return
                    new SUSuministro()
                    {
                        Id = a.SCS_IdSuministro
                    };
                }
                else
                    return
                  new SUSuministro();
            }
        }

        #endregion Validación Suministros

        #region Parametrizacion Ciudades Centro logístico


        public List<PALocalidadDC> ObtenerMunicipiosXCol(long IdCol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                List<PALocalidadDC> LocalidadesCol = contexto.MunicipioCentroLogistico_PUA.Include("Localidad_PAR").Where(m => m.MCL_IdCentroLogistico == IdCol).ToList()
                  .ConvertAll<PALocalidadDC>(mun =>
                  {
                      return new PALocalidadDC()
                      {
                          IdLocalidad = mun.MCL_IdLocalidad,
                          Nombre = mun.Localidad_PAR.LOC_Nombre
                      };
                  });

                return LocalidadesCol;
            }
        }
        /// <summary>
        /// Consulta los municipios y su respectivo centro logístico asociado
        /// </summary>
        /// <param name="IdDepartamento">Id del departamento por el cual se quiere filtrar</param>
        /// <returns></returns>
        public List<PUMunicipioCentroLogisticoDC> ConsultarMunicipiosCol(string IdDepartamento)
        {
            if (IdDepartamento != null)
            {
                using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
                {
                    List<PUMunicipioCentroLogisticoDC> municipiosCol = contexto.MunicipioCentroLogistico_PUA.Include("Localidad_PAR").Include("CentroLogistico_PUA").Where(loc => loc.Localidad_PAR.LOC_IdAncestroPrimerGrado == IdDepartamento || loc.Localidad_PAR.LOC_IdAncestroSegundoGrado == IdDepartamento).ToList()
                      .ConvertAll<PUMunicipioCentroLogisticoDC>(mun =>
                      {
                          return new PUMunicipioCentroLogisticoDC()
                          {
                              CentroLogistico = new PUCentroServicioApoyo()
                              {
                                  IdRacol = mun.CentroLogistico_PUA.CEL_IdRegionalAdm,
                                  IdCentroservicio = mun.MCL_IdCentroLogistico,
                                  NombreCentroServicio = mun.CentroLogistico_PUA.CentroServicios_PUA.CES_Nombre
                              },
                              Municipio = new PALocalidadDC()
                              {
                                  IdLocalidad = mun.MCL_IdLocalidad,
                                  Nombre = mun.Localidad_PAR.LOC_Nombre
                              },

                              //Data Original Para realizar el cambio en caso de un Update Rafram 31/05/2013
                              CentroLogisticoOriginal = new PUCentroServicioApoyo()
                              {
                                  IdRacol = mun.CentroLogistico_PUA.CEL_IdRegionalAdm,
                                  IdCentroservicio = mun.MCL_IdCentroLogistico,
                                  NombreCentroServicio = mun.CentroLogistico_PUA.CentroServicios_PUA.CES_Nombre
                              },
                              MunicipioOriginal = new PALocalidadDC()
                              {
                                  IdLocalidad = mun.MCL_IdLocalidad,
                                  Nombre = mun.Localidad_PAR.LOC_Nombre
                              }
                          };
                      });

                    municipiosCol.AddRange(contexto.Localidad_PAR.Where(loc => (loc.LOC_IdAncestroPrimerGrado == IdDepartamento || loc.LOC_IdAncestroSegundoGrado == IdDepartamento)
                      && contexto.MunicipioCentroLogistico_PUA.All(sp => sp.MCL_IdLocalidad != loc.LOC_IdLocalidad)).ToList()
                      .ConvertAll<PUMunicipioCentroLogisticoDC>(mun =>
                      {
                          return new PUMunicipioCentroLogisticoDC()
                          {
                              CentroLogistico = new PUCentroServicioApoyo(),
                              Municipio = new PALocalidadDC()
                              {
                                  IdLocalidad = mun.LOC_IdLocalidad,
                                  Nombre = mun.LOC_Nombre
                              },

                              CentroLogisticoOriginal = new PUCentroServicioApoyo(),
                              MunicipioOriginal = new PALocalidadDC()
                              {
                                  IdLocalidad = mun.LOC_IdLocalidad,
                                  Nombre = mun.LOC_Nombre
                              }
                          };
                      }));

                    return municipiosCol;
                }
            }
            else
            {
                return new List<PUMunicipioCentroLogisticoDC>();
            }
        }

        /// <summary>
        /// Guarda en la base de datos el municipio
        /// con su respectivo centro logistico de apoyo
        /// </summary>
        /// <param name="municipioCol"></param>
        public void GuardarMunicipioCol(PUMunicipioCentroLogisticoDC municipioCol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                MunicipioCentroLogistico_PUA munCentroLogistico = contexto.MunicipioCentroLogistico_PUA.Where(m => m.MCL_IdLocalidad == municipioCol.MunicipioOriginal.IdLocalidad).FirstOrDefault();

                if (munCentroLogistico != null)
                {
                    //Se elimina el registro
                    contexto.MunicipioCentroLogistico_PUA.Remove(munCentroLogistico);
                    contexto.SaveChanges();

                    //Se adiciona el nuevo Valor
                    MunicipioCentroLogistico_PUA munCentroLogisNuevo = new MunicipioCentroLogistico_PUA()
                    {
                        MCL_CreadoPor = ControllerContext.Current.Usuario,
                        MCL_FechaGrabacion = DateTime.Now,
                        MCL_IdCentroLogistico = municipioCol.CentroLogistico.IdCentroservicio,
                        MCL_IdLocalidad = municipioCol.Municipio.IdLocalidad
                    };
                    contexto.MunicipioCentroLogistico_PUA.Add(munCentroLogisNuevo);
                    contexto.SaveChanges();
                }
                else
                {
                    munCentroLogistico = new MunicipioCentroLogistico_PUA()
                    {
                        MCL_CreadoPor = ControllerContext.Current.Usuario,
                        MCL_FechaGrabacion = DateTime.Now,
                        MCL_IdCentroLogistico = municipioCol.CentroLogistico.IdCentroservicio,
                        MCL_IdLocalidad = municipioCol.Municipio.IdLocalidad
                    };
                    contexto.MunicipioCentroLogistico_PUA.Add(munCentroLogistico);
                    contexto.SaveChanges();
                }

                MunicipioRegionalAdm_PUA munRegional = contexto.MunicipioRegionalAdm_PUA.Where(m => m.MRA_IdMunicipio == municipioCol.MunicipioOriginal.IdLocalidad).FirstOrDefault();

                if (munRegional != null)
                {
                    ///Elimino el registro anterior
                    contexto.MunicipioRegionalAdm_PUA.Remove(munRegional);
                    contexto.SaveChanges();

                    ///Agrego el nuevo registro
                    MunicipioRegionalAdm_PUA munRegionalNew = new MunicipioRegionalAdm_PUA()
                    {
                        MRA_CreadoPor = ControllerContext.Current.Usuario,
                        MRA_IdLocalidadRegionalAdm = municipioCol.CentroLogistico.IdRacol,
                        MRA_FechaGrabacion = DateTime.Now,
                        MRA_IdMunicipio = municipioCol.Municipio.IdLocalidad
                    };
                    contexto.MunicipioRegionalAdm_PUA.Add(munRegionalNew);
                    contexto.SaveChanges();
                }
                else
                {
                    munRegional = new MunicipioRegionalAdm_PUA()
                    {
                        MRA_CreadoPor = ControllerContext.Current.Usuario,
                        MRA_IdLocalidadRegionalAdm = municipioCol.CentroLogistico.IdRacol,
                        MRA_FechaGrabacion = DateTime.Now,
                        MRA_IdMunicipio = municipioCol.Municipio.IdLocalidad
                    };
                    contexto.MunicipioRegionalAdm_PUA.Add(munRegional);
                }

                contexto.SaveChanges();
            }
        }

        #endregion Parametrizacion Ciudades Centro logístico

        #region Clasificador canal de venta

        /// <summary>
        /// Inserta un nuevo clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void AgregarClasificarCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ClasificadorCanalVenta_PUA clasificador = new ClasificadorCanalVenta_PUA()
                {
                    CCV_ClasificadorCanalVenta = clasificadorCanalVenta.ClasificadorCanalVenta,
                    CCV_CreadoPor = ControllerContext.Current.Usuario,
                    CCV_FechaGrabacion = DateTime.Now,
                    CCV_IdTipoCentroServicos = clasificadorCanalVenta.IdTipoCentroServicios,
                    CCV_IdTipoPropiedad = (short)clasificadorCanalVenta.IdTipoPropiedad,
                    CCV_Nombre = clasificadorCanalVenta.Nombre
                };

                contexto.ClasificadorCanalVenta_PUA.Add(clasificador);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Modifica un clasificador de canal de venta
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void ModificarClasificadorCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ClasificadorCanalVenta_PUA clasificador = contexto.ClasificadorCanalVenta_PUA.Where(c => c.CCV_IdClasificadorCanalVenta == clasificadorCanalVenta.IdClasificadorCanalVentas).FirstOrDefault();
                if (clasificador == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                clasificador.CCV_IdTipoCentroServicos = clasificadorCanalVenta.IdTipoCentroServicios;
                clasificador.CCV_IdTipoPropiedad = (short)clasificadorCanalVenta.IdTipoPropiedad;
                clasificador.CCV_Nombre = clasificadorCanalVenta.Nombre;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Borra un clasificador de canal de ventas
        /// </summary>
        /// <param name="clasificadorCanalVenta"></param>
        public void BorrarClasificadorCanalVenta(PUClasificadorCanalVenta clasificadorCanalVenta)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ClasificadorCanalVenta_PUA clasificador = contexto.ClasificadorCanalVenta_PUA.Where(c => c.CCV_IdClasificadorCanalVenta == clasificadorCanalVenta.IdClasificadorCanalVentas).FirstOrDefault();
                if (clasificador == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                contexto.ClasificadorCanalVenta_PUA.Remove(clasificador);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene los clasificadores del canal de ventas
        /// </summary>
        public List<PUClasificadorCanalVenta> ObtenerClasificadorCanalVenta(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsClasificCanalVenta_VPUA(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new PUClasificadorCanalVenta()
                  {
                      ClasificadorCanalVenta = r.CCV_ClasificadorCanalVenta,
                      IdClasificadorCanalVentas = r.CCV_IdClasificadorCanalVenta,
                      IdTipoCentroServicios = r.TCS_IdTipoCentroServicos,
                      IdTipoPropiedad = r.CCV_IdTipoPropiedad,
                      Nombre = r.CCV_Nombre,
                      NombreTipoCentroServicios = r.TCS_Descripcion,
                      NombreTipoPropiedad = r.TPR_Descripcion,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Selecciona todos los tipos de centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUTipoCentroServicio> ObtenerTodosTipoCentroServicio()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoCentroServicios_PUA.ToList().ConvertAll(t =>
                  new PUTipoCentroServicio()
                  {
                      Descripcion = t.TCS_Descripcion,
                      IdTipo = t.TCS_IdTipoCentroServicos
                  });
            }
        }

        #endregion Clasificador canal de venta

        #region Regionales de Casa Matriz

        /// <summary>
        /// Obtener la información basica de las Regionales Administrativas activas de una Casa Matriz
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la Casa Matriz</param>
        /// <returns>Colección con la información básica de las regionales</returns>
        public IList<PURegionalAdministrativa> ObtenerRegionalesDeCasaMatriz(short idCasaMatriz)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RegionalAdministrativa_PUA
                  .Include("CentroServicios_PUA")
                    .Where(w => w.REA_IdCasaMatriz == idCasaMatriz && w.CentroServicios_PUA.CES_Estado == "ACT")
                    .ToList()
                    .ConvertAll<PURegionalAdministrativa>(i => new PURegionalAdministrativa
                    {
                        Descripcion = i.CentroServicios_PUA.CES_Nombre,
                        IdRegionalAdmin = i.REA_IdRegionalAdm,
                        IdCentroCosto = i.CentroServicios_PUA.CES_IdCentroCostos,
                        IdCasaMatriz = idCasaMatriz
                    });
            }
        }

        #endregion Regionales de Casa Matriz

        #region Auditoria inactivacion de centros de servicio

        //public IList<UsuarioCentroServicio_SEG> ConsultarUsuariosCentrosServicio(long idCentroServicio)
        //{
        //    using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        //    {
        //        try
        //        {
        //            return contexto.UsuarioCentroServicio_SEG.Where(us => us.UCS_IdCentroServicios == idCentroServicio).ToList()
        //                .ConvertAll<UsuarioCentroServicio_SEG>( i => new UsuarioCentroServicio_SEG
        //                    {
        //                       UCS_IdCodigoUsuario = i.UCS_IdCodigoUsuario,

        //                       UCS_IdCentroServicios = i.UCS_IdCentroServicios,

        //                       UCS_NombreCentroServicios = i.UCS_NombreCentroServicios,

        //                       UCS_Caja = i.UCS_Caja,

        //                       UCS_CreadoPor = i.UCS_CreadoPor,

        //                       UCS_FechaGrabacion = i.UCS_FechaGrabacion,

        //                       UCS_ImpresionPOS = i.UCS_ImpresionPOS

        //                    }
        //                );

        //        }

        //        catch
        //        {
        //            return null;
        //        }

        //    }
        //}

        public void InhabilitarUsuariosCentroServicio(long idCentroServicio)
        {
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(conexionString))
                {
                    sqlConn.Open();

                    //se llama al SP que permite el cambio de estado del usuario en la tabla usuario_seg insertando el cambio a su vez en la auditoria

                    SqlCommand cmd2 = new SqlCommand("paCambiarEstadoUsuario_SEG", sqlConn);

                    cmd2.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd2.Parameters.Add(new SqlParameter("@idCD", idCentroServicio));

                    cmd2.Parameters.Add(new SqlParameter("@cambiadoPor", ControllerContext.Current.Usuario));

                    cmd2.Parameters.Add(new SqlParameter("@TipoCambio", "MODIFICADO"));

                    cmd2.Parameters.Add(new SqlParameter("@FechaCambio", DateTime.Now));

                    cmd2.ExecuteNonQuery();

                    //Se llama al SP que permite eliminar a todos los usuarios del centro de servicio inhabilitado

                    SqlCommand cmd = new SqlCommand("paEliminarUsuarioCentroServicio_SEG", sqlConn);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@idCentroServicio", idCentroServicio));

                    cmd.Parameters.Add(new SqlParameter("@cambiadoPor", ControllerContext.Current.Usuario));

                    cmd.Parameters.Add(new SqlParameter("@TipoCambio", "ELIMINADO"));

                    cmd.Parameters.Add(new SqlParameter("@FechaCambio", DateTime.Now));

                    cmd.ExecuteNonQuery();

                    sqlConn.Close();
                }
            }
            catch (Exception error)
            {
                throw new Exception("Error EliminarUsuariosCentroServicio - " + error);
            }
        }

        #endregion Auditoria inactivacion de centros de servicio



        #region Bodegas

        /// <summary>
        /// Adiciona el registro a la tabla movimiento inventario y a la tabla movimiento asignación inventario
        /// </summary>
        /// <param name="movimientoInventario"></param>
        /// <returns></returns>
        public long AdicionarMovimientoInventario(PUMovimientoInventario movimientoInventario)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paAdicionarMovimientoInventario_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@INV_IdCentroServicioOrigen", movimientoInventario.IdCentroServicioOrigen); // Solo aplica para Asignacion... Pero hay que enviar el parametro.
                cmd.Parameters.AddWithValue("@INV_IdCentroServicio", movimientoInventario.Bodega.IdCentroServicio);
                cmd.Parameters.AddWithValue("@INV_IdTipoMovimiento", (short)movimientoInventario.TipoMovimiento);
                cmd.Parameters.AddWithValue("@INV_NumeroGuia", movimientoInventario.NumeroGuia);
                cmd.Parameters.AddWithValue("@INV_FechaEstimadaIngreso", movimientoInventario.FechaEstimadaIngreso);
                cmd.Parameters.AddWithValue("@INV_FechaGrabacion", movimientoInventario.FechaGrabacion);
                cmd.Parameters.AddWithValue("@INV_CreadoPor", ControllerContext.Current.Usuario);

                Int64 newId = (Int64)cmd.ExecuteScalar();
                return (long)newId;
            }
        }

        /// <summary>
        /// <Proceso para obtener el centro de confirmaciones y devoluciones de una localidad>
        /// </summary>
        /// <param name="localidad"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroConfirmacionesDevoluciones(PALocalidadDC localidad)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                PUCentroServiciosDC CentroconfirmacionesDevoluciones = new PUCentroServiciosDC();
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCentroConfirmacionesDevoluciones_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idLocalidad", localidad.IdLocalidad);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {

                    CentroconfirmacionesDevoluciones = new PUCentroServiciosDC
                    {
                        IdCentroServicio = Convert.ToInt64(dt.Rows[0]["CES_IdCentroServicios"]),
                        Nombre = dt.Rows[0]["CES_Nombre"].ToString(),
                        Direccion = dt.Rows[0]["CES_Direccion"].ToString(),
                        Telefono1 = dt.Rows[0]["CES_Telefono1"].ToString(),
                        IdMunicipio = dt.Rows[0]["CES_IdMunicipio"].ToString(),
                    };
                }
                return CentroconfirmacionesDevoluciones;
            }
        }

        /// <summary>
        /// <Proceso para obtenerla bodega de custodia
        /// </summary>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerBodegaCustodia()
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                PUCentroServiciosDC bodegaCustodia = new PUCentroServiciosDC();
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerBodegaCustodia_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {

                    bodegaCustodia = new PUCentroServiciosDC
                    {
                        IdCentroServicio = Convert.ToInt64(dt.Rows[0]["CES_IdCentroServicios"]),
                        Nombre = dt.Rows[0]["CES_Nombre"].ToString(),
                        Direccion = dt.Rows[0]["CES_Direccion"].ToString(),
                        Telefono1 = dt.Rows[0]["CES_Telefono1"].ToString(),
                        IdMunicipio = dt.Rows[0]["CES_IdMunicipio"].ToString(),
                        IdPropietario = Convert.ToInt32(dt.Rows[0]["BDG_IdPropietario"])
                    };
                }
                return bodegaCustodia;
            }
        }

        /// <summary>
        /// Inserta el movimiento inventario solo para el ingreso a CAC desde LOI o Custodia
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <param name="IdTipoMovimiento"></param>
        /// <param name="NumeroGuia"></param>
        /// <param name="FechaGrabacion"></param>
        /// <param name="CreadoPor"></param>
        //public void AdicionarMovimientoInventario_CAC(long IdCentroServicio, int IdTipoMovimiento, long NumeroGuia, DateTime FechaGrabacion, string CreadoPor)
        //{
        //    using (SqlConnection conn = new SqlConnection(conexionString))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("paAdicionarMovimientoInventario_CAC", conn);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@INV_IdCentroServicio", IdCentroServicio);
        //        cmd.Parameters.AddWithValue("@INV_IdTipoMovimiento", IdTipoMovimiento);
        //        cmd.Parameters.AddWithValue("@INV_NumeroGuia", NumeroGuia);
        //        cmd.Parameters.AddWithValue("@INV_FechaGrabacion", FechaGrabacion);
        //        cmd.Parameters.AddWithValue("@INV_CreadoPor", CreadoPor);
        //        cmd.ExecuteNonQuery();
        //    }
        //}


        #endregion Bodegas

        #region Custodia

        public void AdicionarCustodia(PUCustodia Custodia)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paAdicionarCustodia_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CUS_IdCustodia", Custodia.IdCustodia);
                cmd.Parameters.AddWithValue("@CUS_ContenidoGuia", Custodia.ContenidoGuia);
                cmd.Parameters.AddWithValue("@CUS_TipoNovedad", Custodia.TipoNovedad);
                cmd.Parameters.AddWithValue("@CUS_Observacion", Custodia.Observacion);
                cmd.Parameters.AddWithValue("@CUS_Ubicacion", Custodia.Ubicacion);
                cmd.Parameters.AddWithValue("@CUS_TipoUbicacion", Custodia.TipoUbicacion);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.ExecuteNonQuery();
            }


        }


        #endregion

        #region Valida guia asignada a custodia

        /// <summary>
        /// Metodo para validar guia asignada a reclame en oficina
        /// </summary>
        /// <param name="IdCentroServicio"></param>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public PUEnumTipoMovimientoInventario ConsultaGuiaAsignada(PUCustodia custodia)
        {
            PUEnumTipoMovimientoInventario tipoMovimientoActual = new PUEnumTipoMovimientoInventario();
            using (SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarUltimoMovimientoGuia_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@INV_NumeroGuia", custodia.MovimientoInventario.NumeroGuia));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                PUMovimientoInventario movimientoInventario = new PUMovimientoInventario();
                if (dt.Rows.Count > 0)
                {
                    tipoMovimientoActual = (PUEnumTipoMovimientoInventario)Convert.ToInt64(dt.Rows[0]["INV_IdTipoMovimiento"]);
                }
                else
                {
                    tipoMovimientoActual = PUEnumTipoMovimientoInventario.Asignacion;
                }
                sqlConn.Close();
            }
            return tipoMovimientoActual;
        }


        #endregion

        #region AdjuntosMovimientoInventario

        public void AdicionarAdjuntoMovimientoInventario(PUAdjuntoMovimientoInventario AdjuntoMovimientoInventario)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paAdicionarAdjuntoMovimientoInventario_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@AIN_IdMovimientoInventario", AdjuntoMovimientoInventario.MovimientoInventario.IdMovimientoInventario);
                cmd.Parameters.AddWithValue("@AIN_RutaAdjunto", AdjuntoMovimientoInventario.RutaAdjunto);
                cmd.Parameters.AddWithValue("@AIN_FormatoAdjunto", AdjuntoMovimientoInventario.FormatoAdjunto);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region ObtenerGuiasCustodia

        public List<PUCustodia> ObtenerGuiasCustodia(int idTipoMovimiento, Int16 idEstadoGuia, long? numeroGuia, bool muestraReportemuestraTodosreporte)
        {
            //CONSULTAR GUIAS CUSTODIA
            List<PUCustodia> lstInventario = new List<PUCustodia>();
            PUCustodia Custodia;
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("paConsultaGuiasCustodia_PUA", conn);
                cmd.Parameters.Add(new SqlParameter("@IdTipoMovimiento", idTipoMovimiento));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoGuia", idEstadoGuia));
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@MostrarTodos", muestraReportemuestraTodosreporte));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {

                    Custodia = new PUCustodia();
                    Custodia.MovimientoInventario = new PUMovimientoInventario()
                    {
                        NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]),
                        FechaGrabacion = Convert.ToDateTime(read["FechaGrabacion"]),
                        TipoMovimiento = (PUEnumTipoMovimientoInventario)Convert.ToInt16(read["INV_IdTipoMovimiento"])
                    };
                    Custodia.DiasCustodia = (DateTime.Now.Date - Convert.ToDateTime(read["FechaGrabacion"])).Days;
                    Custodia.UbicacionDetalle = read["CUS_TipoUbicacion"] == DBNull.Value ? "" : ((PUEnumTipoUbicacion)Convert.ToInt32(read["CUS_TipoUbicacion"]) == PUEnumTipoUbicacion.Estiba) ? "Estiba " + read["CUS_Ubicacion"] : "Casillero " + read["CUS_Ubicacion"];
                    Custodia.RacolOrigen = read["ADM_NombreCentroServicioOrigen"].ToString();
                    Custodia.NombreDestinatario = read["ADM_NombreDestinatario"].ToString();
                    Custodia.Peso = Convert.ToInt32(read["ADM_Peso"]);
                    Custodia.TipoEnvio = read["ADM_NombreTipoEnvio"].ToString();
                    Custodia.DiceContener = read["ADM_DiceContener"].ToString();
                    Custodia.UsuarioAsignacion = read["ADM_CreadoPor"].ToString();
                    lstInventario.Add(Custodia);
                }

            }
            return lstInventario;
        }


        public List<PUCustodia> ObtenerGuiasCustodiaParaIngreso(int idTipoMovimiento, Int16 idEstadoGuia, bool muestraReportemuestraTodosreporte)
        {
            //CONSULTAR GUIAS CUSTODIA
            List<PUCustodia> lstInventario = new List<PUCustodia>();
            PUCustodia Custodia;
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("paObtenerGuiasPendIngresoCustodia_LOI", conn);
                cmd.Parameters.Add(new SqlParameter("@IdTipoMovimiento", idTipoMovimiento));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoGuia", idEstadoGuia));
                cmd.Parameters.Add(new SqlParameter("@MostrarTodos", muestraReportemuestraTodosreporte));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader read = cmd.ExecuteReader();
                while (read.Read())
                {

                    Custodia = new PUCustodia();
                    Custodia.MovimientoInventario = new PUMovimientoInventario()
                    {
                        NumeroGuia = Convert.ToInt64(read["ADM_NumeroGuia"]),
                        FechaGrabacion = Convert.ToDateTime(read["FechaGrabacion"]),
                        TipoMovimiento = (PUEnumTipoMovimientoInventario)Convert.ToInt16(read["INV_IdTipoMovimiento"])
                    };
                    Custodia.DiasCustodia = (DateTime.Now.Date - Convert.ToDateTime(read["FechaGrabacion"])).Days;
                    Custodia.UbicacionDetalle = read["CUS_TipoUbicacion"] == DBNull.Value ? "" : ((PUEnumTipoUbicacion)Convert.ToInt32(read["CUS_TipoUbicacion"]) == PUEnumTipoUbicacion.Estiba) ? "Estiba " + read["CUS_Ubicacion"] : "Casillero " + read["CUS_Ubicacion"];
                    Custodia.RacolOrigen = read["ADM_NombreCentroServicioOrigen"].ToString();
                    Custodia.RacolDestino = read["ADM_NombreCentroServicioDestino"].ToString();
                    Custodia.NombreDestinatario = read["ADM_NombreDestinatario"].ToString();
                    Custodia.Peso = Convert.ToInt32(read["ADM_Peso"]);
                    Custodia.TipoEnvio = read["ADM_NombreTipoEnvio"].ToString();
                    Custodia.DiceContener = read["ADM_DiceContener"].ToString();
                    Custodia.UsuarioAsignacion = read["ADM_CreadoPor"].ToString();
                    lstInventario.Add(Custodia);
                }

            }
            return lstInventario;
        }


        #endregion

        #region obtener ultimo movimiento inventario de la guia

        /// <summary>
        /// Metodo para obtener ultimo movimiento inventario de la guia
        /// </summary>
        /// <param name="NumeroGuia"></param>
        /// <returns></returns>
        public PUMovimientoInventario ConsultaUltimoMovimientoGuia(long NumeroGuia)
        {
            PUMovimientoInventario movimientoInventario = new PUMovimientoInventario();
            using (SqlConnection sqlConn = new SqlConnection(conexionString))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarUltimoMovimientoGuia_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@INV_NumeroGuia", NumeroGuia));

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    movimientoInventario.IdMovimientoInventario = Convert.ToInt64(dt.Rows[0]["INV_IdMovimientoInventario"]);
                    movimientoInventario.Bodega = new PUCentroServiciosDC() { IdCentroServicio = Convert.ToInt64(dt.Rows[0]["INV_IdCentroServicio"]) };
                    movimientoInventario.TipoMovimiento = (PUEnumTipoMovimientoInventario)Convert.ToInt64(dt.Rows[0]["INV_IdTipoMovimiento"]);
                    movimientoInventario.NumeroGuia = Convert.ToInt64(dt.Rows[0]["INV_NumeroGuia"]);
                    if (dt.Rows[0]["FechaEstimadaIngreso"] != DBNull.Value)
                        movimientoInventario.FechaEstimadaIngreso = Convert.ToDateTime(dt.Rows[0]["FechaEstimadaIngreso"]);
                    movimientoInventario.FechaGrabacion = Convert.ToDateTime(dt.Rows[0]["INV_FechaGrabacion"]);
                    movimientoInventario.CreadoPor = dt.Rows[0]["INV_CreadoPor"].ToString();
                }
                sqlConn.Close();
            }
            return movimientoInventario;
        }

        #endregion

        #region Reclame en oficina

        public List<PUMovimientoInventario> ObtenerGuiasReclameOficina(long idCentroServicio, PUEnumTipoMovimientoInventario tipoMovimiento)
        {
            return new List<PUMovimientoInventario>();
        }


        /// <summary>
        /// Método para obtener los puntos y agencias de un col
        /// </summary>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerPuntosAgenciasColReclamaOficina(long idCol)
        {
            List<PUCentroServiciosDC> lstPuntosAgencias = new List<PUCentroServiciosDC>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPuntosAgenciasColReclamaOficina_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCol", idCol);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PUCentroServiciosDC puntosAgenciasAro = new PUCentroServiciosDC();
                    puntosAgenciasAro.IdCentroServicio = Convert.ToInt64(reader["IdCentroServicio"]);
                    puntosAgenciasAro.Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : string.Empty;
                    puntosAgenciasAro.Telefono1 = reader["Telefono1"] != DBNull.Value ? reader["Telefono1"].ToString() : string.Empty;
                    puntosAgenciasAro.Direccion = reader["Direccion"] != DBNull.Value ? reader["Direccion"].ToString() : string.Empty;
                    puntosAgenciasAro.IdMunicipio = reader["IdMunicipio"] != DBNull.Value ? reader["IdMunicipio"].ToString() : string.Empty;
                    puntosAgenciasAro.NombreMunicipio = reader["NombreMunicipio"] != DBNull.Value ? reader["NombreMunicipio"].ToString() : string.Empty;
                    lstPuntosAgencias.Add(puntosAgenciasAro);
                }
                sqlConn.Close();
            }
            return lstPuntosAgencias;
        }


        #endregion


        public List<PUCentroServiciosDC> ObtenerCentroServicioPASPorRacol(long idRacol)
        {
            List<PUCentroServiciosDC> lstCentrosServicioPAS = new List<PUCentroServiciosDC>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciasPuntosPorRacol_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRegional", idRacol);
                cmd.Parameters.AddWithValue("@tipoAgencia", ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA);
                cmd.Parameters.AddWithValue("@tipoPunto", ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO);
                cmd.Parameters.AddWithValue("@estado", ConstantesFramework.ESTADO_ACTIVO);
                cmd.Parameters.AddWithValue("@aplicaPAM", 1);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PUCentroServiciosDC centroSErvicioPAS = new PUCentroServiciosDC();
                    centroSErvicioPAS.IdCentroServicio = Convert.ToInt64(reader["CES_IdCentroServicios"]);
                    centroSErvicioPAS.Nombre = reader["CES_Nombre"].ToString();
                    lstCentrosServicioPAS.Add(centroSErvicioPAS);
                }
                sqlConn.Close();
            }
            return lstCentrosServicioPAS;
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            var listaCentrosServicios = new List<PUCentroServicioInfoGeneral>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerInfoBasicaCentrosServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                listaCentrosServicios = dt.AsEnumerable().ToList().ConvertAll<PUCentroServicioInfoGeneral>(r =>
                {

                    PUCentroServicioInfoGeneral csGeneral = new PUCentroServicioInfoGeneral()
                    {
                        IdLocalidad = r.Field<string>("LOC_IdLocalidad"),
                        IdCentroServicios = r.Field<long>("CES_IdCentroServicios"),
                        Barrio = r.Field<string>("CES_Barrio"),
                        DireccionCentroServicio = r.Field<string>("CES_Direccion"),
                        Latitud = r["CES_Latitud"] != DBNull.Value ? r.Field<decimal>("CES_Latitud") : 0,
                        Longitud = r["CES_Longitud"] != DBNull.Value ? r.Field<decimal>("CES_Longitud") : 0,
                        LocalidadNombre = r.Field<string>("LOC_Nombre"),
                        LocalidadNombreCompleto = r.Field<string>("NombreCompleto"),
                        NombreCentroServicio = r.Field<string>("CES_Nombre"),
                        Telefono1 = r.Field<string>("CES_Telefono1"),
                        Telefono2 = r.Field<string>("CES_Telefono2"),
                        Tipo = r.Field<string>("CES_Tipo"),
                        EsReclameEnOficina = r.Field<int>("ReclameEnOficina") == 1
                    };

                    //var idHorarioCentroServicio = 0;
                    //var detalleHorariosCentroServicio = dt.AsEnumerable().ToList()
                    //    .Where(c => c.Field<long>("CES_IdCentroServicios") == r.Field<long>("CES_IdCentroServicios"))
                    //    .GroupBy(g => !int.TryParse(g.Field<string>("HCS_IdHorarioCentroServicio"), out idHorarioCentroServicio) ? idHorarioCentroServicio : 0
                    //    ).Select(s => s.First()).ToList();


                    //csGeneral.HorariosCentroServicios = detalleHorariosCentroServicio.ConvertAll<PUHorariosCentroServicios>(h =>
                    //    {
                    //        DateTime horaInicio;
                    //        DateTime horaFin;
                    //        return new PUHorariosCentroServicios()
                    //        {
                    //            //HoraFin = h.Field<DateTime>("HCS_HoraFin"),
                    //            //HoraInicio = h.Field<DateTime>("HCS_HoraInicio"),
                    //            //NombreDia = h.Field<string>("HCS_NombreDia")                                     
                    //        };
                    //    });                         
                    return csGeneral;
                });
            }

            var horariosCentrosServicio = ObtenerHorariosCentrosServicios();

            listaCentrosServicios.ForEach(item =>
            {
                item.HorariosCentroServicios = horariosCentrosServicio.ToList().FindAll(x => x.IdLocalidad == item.IdLocalidad && x.IdCentroServicios == item.IdCentroServicios);
            });

            return listaCentrosServicios;

        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerPosicionesCanalesVenta(DateTime fechaInicial, DateTime fechaFinal, string idMensajero, string idCentroServicio, int idEstado)
        {
            var listaCentrosServicios = new List<PUCentroServicioInfoGeneral>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPosicionesCanalesVenta_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fechaInicial", fechaInicial);
                cmd.Parameters.AddWithValue("@fechaFinal", fechaFinal);
                cmd.Parameters.AddWithValue("@IdEstado", idEstado);
                if (!string.IsNullOrEmpty(idMensajero))
                {
                    cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                }
                if (!string.IsNullOrEmpty(idCentroServicio))
                {
                    cmd.Parameters.AddWithValue("@IdCentroServicios", idCentroServicio);
                }
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                listaCentrosServicios = dt.AsEnumerable().ToList().ConvertAll<PUCentroServicioInfoGeneral>(r =>
                {

                    PUCentroServicioInfoGeneral csGeneral = new PUCentroServicioInfoGeneral()
                    {
                        IdLocalidad = r.Field<string>("LOC_IdLocalidad"),
                        IdCentroServicios = r.Field<long>("CES_IdCentroServicios"),
                        Barrio = r.Field<string>("CES_Barrio"),
                        DireccionCentroServicio = r.Field<string>("CES_Direccion"),
                        Latitud = r["CES_Latitud"] != DBNull.Value ? r.Field<decimal>("CES_Latitud") : 0,
                        Longitud = r["CES_Longitud"] != DBNull.Value ? r.Field<decimal>("CES_Longitud") : 0,
                        LocalidadNombre = r.Field<string>("LOC_Nombre"),
                        LocalidadNombreCompleto = r.Field<string>("NombreCompleto"),
                        NombreCentroServicio = r.Field<string>("CES_Nombre"),
                        Telefono1 = r.Field<string>("CES_Telefono1"),
                        Telefono2 = r.Field<string>("CES_Telefono2"),
                        Tipo = r.Field<string>("CES_Tipo"),
                        EsReclameEnOficina = r.Field<int>("ReclameEnOficina") == 1,
                        EstadoRecogida = r.Field<int>("EstadoRecogida"),
                        FechaHoraRecogida = r.Field<DateTime>("SRE_FechaHoraRecogida")
                    };
                    return csGeneral;
                });
            }

            var horariosCentrosServicio = ObtenerHorariosCentrosServicios();

            listaCentrosServicios.ForEach(item =>
            {
                item.HorariosCentroServicios = horariosCentrosServicio.ToList().FindAll(x => x.IdLocalidad == item.IdLocalidad && x.IdCentroServicios == item.IdCentroServicios);
            });

            return listaCentrosServicios;

        }

        public List<PUHorariosCentroServicios> ObtenerHorariosCentrosServicios()
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorariosCentrosServicio", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<PUHorariosCentroServicios>(r =>
                {

                    PUHorariosCentroServicios csGeneral = new PUHorariosCentroServicios()
                    {
                        IdLocalidad = !r.IsNull("LOC_IdLocalidad") ? r.Field<string>("LOC_IdLocalidad") : string.Empty,
                        IdCentroServicios = !r.IsNull("HCS_IdCentroServicios") ? r.Field<long>("HCS_IdCentroServicios") : 0,
                        IdHorarioCentroServicios = !r.IsNull("HCS_IdHorarioCentroServicio") ? r.Field<int>("HCS_IdHorarioCentroServicio") : 0,
                        IdDia = !r.IsNull("HCS_IdDia") ? r.Field<string>("HCS_IdDia") : string.Empty,
                        NombreDia = !r.IsNull("HCS_NombreDia") ? r.Field<string>("HCS_NombreDia") : string.Empty,
                        HoraInicio = !r.IsNull("HCS_HoraInicio") ? r.Field<DateTime>("HCS_HoraInicio") : new DateTime(),
                        HoraFin = !r.IsNull("HCS_HoraFin") ? r.Field<DateTime>("HCS_HoraFin") : new DateTime(),
                    };
                    return csGeneral;
                });

            }
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerServiciosHorariosCentrosServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicios", idCentroServicio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().GroupBy(r => r.Field<int>("SER_IdServicio"))
                    .Select(s => s.First()).ToList().ConvertAll<PUServicio>(r =>
                    {


                        PUServicio servicio = new PUServicio()
                        {
                            NombreServicio = r.Field<string>("SER_Nombre"),
                            IdServicio = r.Field<int>("SER_IdServicio")
                        };


                        var detalleCentroServicioServicio = dt.AsEnumerable().Where(d => d.Field<int>("SER_IdServicio") == r.Field<int>("SER_IdServicio")).ToList();

                        servicio.HorariosServicios = detalleCentroServicioServicio.ConvertAll<PUHorariosServiciosCentroServicios>(h =>
                        {
                            return new PUHorariosServiciosCentroServicios()
                            {
                                HoraFin = h.Field<DateTime>("HoraFinal"),
                                HoraInicio = h.Field<DateTime>("HoraInicial"),
                                NombreDia = h.Field<string>("DIA_NombreDia"),
                                IdCentroServiciosServicio = h.Field<long>("CSS_IdCentroServicioServicio"),
                                IdDia = h.Field<string>("HCS_IdDia"),
                            };
                        });

                        return servicio;
                    });

            }
        }

        /// <summary>
        /// Obtiene los centros de servicio que contienen un tipo de servicio especificado
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerCentrosServicioPorServicio(int idServicio)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentroServiciosPorTipoServicio", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var listaCentrosServicios = dt.AsEnumerable().ToList().ConvertAll<PUServicio>(r =>
                {
                    PUServicio servicio = new PUServicio()
                    {
                        NombreServicio = r.Field<string>("SER_Nombre"),
                        IdServicio = r.Field<int>("SER_IdServicio"),
                        IdCentroServicio = r.Field<long>("CSS_IdCentroServicios"),
                        IdCentroServicioServicio = r.Field<long>("CSS_IdCentroServicioServicio"),
                        IdLocalidad = r.Field<string>("LOC_IdLocalidad"),
                        NombreLocalidad = r.Field<string>("LOC_Nombre"),
                        NombreCompletoLocalidad = r.Field<string>("NombreCompleto")
                    };
                    return servicio;
                });

                var horariosCentrosServicio = ObtenerHorariosCentrosServicios();

                listaCentrosServicios.ForEach(item =>
                {
                    var horarios = horariosCentrosServicio.ToList().FindAll(x => x.IdLocalidad == item.IdLocalidad && x.IdCentroServicios == item.IdCentroServicio);
                    item.HorariosServicios = horarios.ConvertAll<PUHorariosServiciosCentroServicios>(x =>
                    {
                        var horario = new PUHorariosServiciosCentroServicios
                        {
                            NombreDia = x.NombreDia,
                            IdDia = x.IdDia,
                            HoraInicio = x.HoraInicio,
                            HoraFin = x.HoraFin
                        };
                        return horario;
                    });
                });

                return listaCentrosServicios;
            }
        }

        /// <summary>
        /// Obtiene el centro de acopio de la bodega
        /// </summary>
        /// <param name="idBodega"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroDeAcopioBodega(long idBodega, long idUsuario)
        {
            PUCentroServiciosDC puCentroServicioDC = null;
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentroAcopioBodega_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdBodega", idBodega);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    puCentroServicioDC = new PUCentroServiciosDC()
                    {
                        Direccion = reader["CES_Direccion"].ToString(),
                        Barrio = reader["CES_Barrio"].ToString(),
                        Email = reader["CES_Email"].ToString(),
                        Fax = reader["CES_Fax"].ToString(),
                        IdMunicipio = reader["CES_IdMunicipio"].ToString(),
                        IdPropietario = (int)reader["CES_IdPropietario"],
                        Estado = reader["CES_Estado"].ToString(),
                        IdCentroCostos = reader["CES_IdCentroCostos"].ToString(),
                        IdCentroServicio = (long)reader["CES_IdCentroServicios"],
                        IdPersonaResponsable = (long)reader["CES_IdPersonaResponsable"],
                        IdZona = reader["CES_IdZona"].ToString(),
                        Latitud = (reader["CES_Latitud"] == DBNull.Value) ? 0 : (decimal)reader["CES_Latitud"],
                        Longitud = (reader["CES_Longitud"] == DBNull.Value) ? 0 : (decimal)reader["CES_Longitud"],
                        Nombre = reader["CES_Nombre"].ToString(),
                        Telefono1 = reader["CES_Telefono1"].ToString(),
                        Telefono2 = reader["CES_Telefono2"].ToString(),
                        Tipo = reader["CES_Tipo"].ToString()
                    };
                }
                return puCentroServicioDC;
            }
        }

        public List<PUCentroServiciosDC> ObtenerLocacionesAutorizadas(string usuario)
        {
            List<PUCentroServiciosDC> locaciones = new List<PUCentroServiciosDC>();
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentrosSErviciosAutorizadosPorusuario_SEG", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usuario", usuario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PUCentroServiciosDC locacion = new PUCentroServiciosDC
                    {
                        IdCentroServicio = Convert.ToInt64(reader["UCS_IdCentroServicios"]),
                        Nombre = reader["UCS_NombreCentroServicios"].ToString(),
                        IdMunicipio = reader["CES_IdMunicipio"].ToString(),
                        NombreMunicipio = reader["LOC_Nombre"].ToString()
                    };
                    locaciones.Add(locacion);
                }
                conn.Close();
            }
            return locaciones;
        }


    }
}