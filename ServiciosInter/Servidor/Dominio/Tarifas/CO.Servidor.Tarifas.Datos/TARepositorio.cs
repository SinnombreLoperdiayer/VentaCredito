using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Adicionales;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Servicios;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft;
using System.Data;

namespace CO.Servidor.Tarifas.Datos
{
    /// <summary>
    /// Clase que representa el repositorio para Tarifas
    /// </summary>
    public partial class TARepositorio
    {
        #region Campos

        private static readonly TARepositorio instancia = new TARepositorio();
        private const string NombreModelo = "ModeloTarifas";
        private string cadenaTransaccional = System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        #endregion Campos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static TARepositorio Instancia
        {
            get { return TARepositorio.instancia; }
        }

        #endregion Propiedades

        #region Consultas Generales

        /// <summary>
        /// Obtiene lista de tipos de destino
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATipoDestino> ObtenerTiposDestino()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoDestino_TAR.ToList().ConvertAll(t => new TATipoDestino
                {
                    Id = t.TID_IdTipoDestino,
                    Descripcion = t.TID_TipoDestino
                });
            }
        }

        /// <summary>
        /// Obtiene una lista con los estados del usuario
        /// </summary>
        /// <returns>Objeto lista estados</returns>
        public IEnumerable<EstadoDC> ObtenerEstadoActivoInactivo()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoActivoInactivo_VFRM.OrderBy(o => o.Estado).ToList().ConvertAll<EstadoDC>(r => new EstadoDC()
                {
                    IdEstado = r.IdEstado,
                    EstadoDescripcion = r.Estado
                });
            }
        }

        /// <summary>
        /// Obtiene los Servicios de la DB
        /// </summary>
        /// <returns>Lista con los servicios de la DB</returns>
        public IEnumerable<TAServicioDC> ObtenerServicios()
        {
            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                List<TAServicioDC> servicios = new List<TAServicioDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServicios_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    servicios.Add(new TAServicioDC
                    {
                        IdServicio = int.Parse(lector["SER_IdServicio"].ToString()),
                        Nombre = lector["SER_Nombre"].ToString(),
                        Descripcion = lector["SER_Descripcion"].ToString(),
                        UnidadNegocio = lector["UNE_IdUnidad"].ToString(),
                        IdConceptoCaja = int.Parse(lector["SER_IdConceptoCaja"].ToString()),
                        TiempoEntrega = 0
                    });
                }
                return servicios;
            }
        }

        /// <summary>
        /// Obtiene la lista de servicios por unidad de negocio
        /// </summary>
        /// <param name="IdUnidadNegocio">Id de la unidad de negocio</param>
        /// <returns>Lista de Servicios de la unidad de negocio</returns>
        public IList<TAServicioDC> ObtenerServiciosUnidadNegocio(string IdUnidadNegocio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (IdUnidadNegocio == string.Empty)
                {
                    return contexto.Servicio_TAR.OrderBy(o => o.SER_IdServicio).ToList().ConvertAll(r => new TAServicioDC()
                    {
                        IdServicio = r.SER_IdServicio,
                        Nombre = r.SER_Nombre,
                        Descripcion = r.SER_Descripcion,
                        IdUnidadNegocio = r.SER_IdUnidadNegocio,
                        UnidadNegocio = contexto.UnidadNegocio_TAR.Where(u => u.UNE_IdUnidad == r.SER_IdUnidadNegocio).FirstOrDefault().UNE_IdUnidad,
                    });
                }
                else
                    return contexto.Servicio_TAR.Where(s => s.SER_IdUnidadNegocio == IdUnidadNegocio).OrderBy(o => o.SER_IdServicio).ToList().ConvertAll(r => new TAServicioDC()
                    {
                        IdServicio = r.SER_IdServicio,
                        Nombre = r.SER_Nombre,
                        Descripcion = r.SER_Descripcion,
                        UnidadNegocio = contexto.UnidadNegocio_TAR.Where(u => u.UNE_IdUnidad == r.SER_IdUnidadNegocio).FirstOrDefault().UNE_IdUnidad
                    });
            }
        }

        /// <summary>
        /// Obtiene las cuentas externas
        /// </summary>
        /// <returns>Objeto Cuenta Externa</returns>
        public IEnumerable<TACuentaExternaDC> ObtenerCuentaExterna()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CuentaExterna_PAR.OrderBy(o => o.CEX_Descripcion).ToList().ConvertAll(r => new TACuentaExternaDC()
                {
                    IdCuentaExterna = r.CEX_IdCuentaExterna,
                    Descripcion = r.CEX_Descripcion
                });
            }
        }

        /// <summary>
        /// Obtiene los tipos de entrega de mensajeria
        /// </summary>
        /// <returns>Listado de los tipos de entrega</returns>
        public IEnumerable<TATipoEntrega> ObtenerTipoEntrega()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoEntrega_MEN.OrderBy(o => o.TIE_Descripcion).ToList().ConvertAll(r => new TATipoEntrega()
                {
                    IdTipoEntrega = r.TIE_IdTipoEntrega,
                    Descripcion = r.TIE_Descripcion
                });
            }
        }

        /// <summary>
        /// Obtiene los Operadores Postales de la Aplicación
        /// </summary>
        /// <returns>Colección con los operadores postales</returns>
        public IEnumerable<TAOperadorPostalDC> ObtenerOperadoresPostales()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.OperadorPostal_PAR.OrderBy(o => o.OPO_Nombre).ToList().ConvertAll(r => new TAOperadorPostalDC()
                {
                    IdOperadorPostal = r.OPO_IdOperadorPostal,
                    Nombre = r.OPO_Nombre
                });
            }
        }

        /// <summary>
        /// Obtiene la zonas de la aplicación
        /// </summary>
        /// <returns>Colección con las zonas de la aplicación</returns>
        public IEnumerable<PAZonaDC> ObtenerZonas()
        {
            return Framework.Servidor.ParametrosFW.Datos.PARepositorio.Instancia.ObtenerZonasOperadorPostal();
        }

        /// <summary>
        /// Obtiene los tipos de empaque de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de empaque</returns>
        public IEnumerable<TATipoEmpaque> ObtenerTiposEmpaqueTotal()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoEmpaque_TAR.OrderBy(o => o.TEM_Descripcion).ToList().ConvertAll(r => new TATipoEmpaque()
                {
                    IdTipoEmpaque = r.TEM_IdTipoEmpaque,
                    Descripcion = r.TEM_Descripcion
                });
            }
        }

        /// <summary>
        /// Obtiene los tipos de impuestos de la aplicación
        /// </summary>
        /// <returns>Lista con los impuestos de la aplicación</returns>
        public IList<TAImpuestosDC> ObtenerImpuestos()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Impuesto_TAR.OrderBy(o => o.IMP_Descripcion).ToList().ConvertAll(r => new TAImpuestosDC()
                {
                    Identificador = r.IMP_IdImpuesto,
                    Descripcion = r.IMP_Descripcion,
                    Valor = r.IMP_Valor,
                    CuentaExterna = new TACuentaExternaDC()
                    {
                        IdCuentaExterna = r.IMP_IdCuentaExterna ?? 0
                    }
                });
            }
        }

        /// <summary>
        /// Obtiene los trayectos y subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayectos y subtrayectos</returns>
        public IEnumerable<TATrayectoSubTrayectoDC> ObtenerTrayectosSubtrayectos()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TrayectoSubTrayecto_TAR.Include("TipoTrayecto_TAR")
                  .Where(ts => ts.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ConvertAll(r => new TATrayectoSubTrayectoDC()
                  {
                      IdTipoTrayecto = r.TRS_IdTipoTrayecto,
                      IdTipoSubTrayecto = r.TRS_IdTipoSubTrayecto,
                      DescripcionTrayecto = r.TipoTrayecto_TAR.TTR_Descripcion,
                      DescripcionTipoSubTrayecto = contexto.TipoSubTrayecto_TAR.Where(ts => ts.TST_IdTipoSubTrayecto == r.TRS_IdTipoSubTrayecto).FirstOrDefault().TST_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene los tipos de trayecto de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de trayecto</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayectoGenerales()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoTrayecto_TAR.OrderBy(o => o.TTR_Descripcion)
                  .ToList()
                  .ConvertAll(r => new TATipoTrayecto()
                  {
                      IdTipoTrayecto = r.TTR_IdTipoTrayecto,
                      Descripcion = r.TTR_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene los tipos de subtrayectos de la aplicación
        /// </summary>
        /// <returns>Colección con los tipos de subtrayecto</returns>
        public IEnumerable<TATipoSubTrayecto> ObtenerTiposSubTrayectoGenerales()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoSubTrayecto_TAR.OrderBy(o => o.TST_Descripcion)
                  .Where(ts => ts.TST_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ConvertAll(r => new TATipoSubTrayecto
                  {
                      IdTipoSubTrayecto = r.TST_IdTipoSubTrayecto,
                      Descripcion = r.TST_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene el identificador de ListaPrecioServicio_TAR
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <param name="idListaPrecio">Identificador Lista Precio</param>
        /// <returns>Identificador ListaPrecioServicio_TAR</returns>
        public string ObtenerIdentificadorListaPrecioServicio(int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecioServicio_TAR listaPrecioSvc = contexto.ListaPrecioServicio_TAR
                  .Where(r => r.LPS_IdServicio == idServicio && r.LPS_IdListaPrecios == idListaPrecio)
                  .FirstOrDefault();

                if (listaPrecioSvc == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_EL_SEERVICIO_SOLICTADO_NO_ESTA_EN_LA_LISTA_DE_PRECIOS.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_EL_SEERVICIO_SOLICTADO_NO_ESTA_EN_LA_LISTA_DE_PRECIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return listaPrecioSvc.LPS_IdListaPrecioServicio.ToString();
            }
        }

        /// <summary>
        /// metodo que obtiene las formas de pago posibles
        /// </summary>
        /// <returns>lista con las formas de pago de tipo TAFormaPago </returns>
        public IEnumerable<TAFormaPago> ObtenerFormasPago(bool aplicaFactura)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.FormasPago_TAR.Where(t => t.FOP_AplicaFacturacion == aplicaFactura).OrderBy(o => o.FOP_Descripcion)
                  .ToList()
                  .ConvertAll(r => new TAFormaPago
                  {
                      IdFormaPago = r.FOP_IdFormaPago,
                      Descripcion = r.FOP_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene los tipos de trámite
        /// </summary>
        /// <returns>Colección</returns>
        public IEnumerable<TATipoTramite> ObtenerTiposTramitesGeneral()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoTramite_TAR.OrderBy(o => o.TIT_Descripcion)
                  .ToList()
                  .ConvertAll(r => new TATipoTramite()
                  {
                      IdTipoTramite = r.TIT_IdTipoTramite,
                      Descripcion = r.TIT_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene los tipos de valor adicional
        /// </summary>
        /// <returns>Colección con los valores adicionales</returns>
        public IEnumerable<TAValorAdicionalValorDC> ObtenerTiposValorAdicionalServicio(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoValorAdicional_TAR
                  .Where(w => w.TVA_IdServicio == idServicio)
                  .OrderBy(o => o.TVA_Descripcion)
                  .ToList()
                  .ConvertAll(r => new TAValorAdicionalValorDC()
                  {
                      IdTipoValorAdicional = r.TVA_IdTipoValorAdicional,
                      Descripcion = r.TVA_Descripcion
                  });
            }
        }

        /// <summary>
        /// Retorna un parámetro de configuración de tarifas
        /// </summary>
        /// <param name="nombreParametro"></param>
        /// <returns></returns>
        public string ObtenerParametro(string nombreParametro)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ParametrosTarifas_TAR parametro = contexto.ParametrosTarifas_TAR.FirstOrDefault(p => p.PAT_IdParametro == nombreParametro);
                if (parametro != null)
                {
                    return parametro.PAT_ValorParametro;
                }
                return null;
            }
        }

        /// <summary>
        /// Retorna los servicios paramatrizados para novasoft 
        /// </summary>
        /// <returns></returns>
        public List<CAServiciosFormaPagoDC> ObtenerServiciosFormasPagoNovasoft()
        {
            List<TAFormaPago> lstFormasPago = ObtenerFormasPagoConServicios();
            ObservableCollection<CATipoCuentaDC> lstTipoCuentaNovasoft = ObtenerCuentasNovasoft();

            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<CAServiciosFormaPagoDC> formas = contexto.ContabilizacionServicioFormaPago_VTAR.ToList().ConvertAll(r =>
                {
                    CAServiciosFormaPagoDC servicio = new CAServiciosFormaPagoDC()
                    {
                        IdServicio = r.CSFP_IdContabilidadServicioPago,
                        TipoServicio = new TAServicioDC() { IdServicio = r.SER_IdServicio, Nombre = r.SER_Nombre },
                        FormaPagoNova = new TAFormaPago()
                        {
                            IdFormaPago = r.FOP_IdFormaPago,
                            IdFormaPagoInt = (int)r.FOP_IdFormaPago,
                            Descripcion = r.FOP_Descripcion,
                            ServiciosAsociados = lstFormasPago.Where(s => s.IdFormaPago == r.FOP_IdFormaPago).FirstOrDefault().ServiciosAsociados
                        },
                        TipoCuentaVD = new CATipoCuentaDC() { CodigoCuenta = r.CSFP_VentasDebito.Trim(), CodigoNombreCuenta = r.CSFP_VentasDebito.Trim() + " - " + lstTipoCuentaNovasoft.ToList().Find(e => e.CodigoCuenta == r.CSFP_VentasDebito.Trim()).NombreCuenta.Trim() },
                        TipoCuentaVC = new CATipoCuentaDC() { CodigoCuenta = r.CSFP_VentasCredito.Trim(), CodigoNombreCuenta = r.CSFP_VentasCredito.Trim() + " - " + lstTipoCuentaNovasoft.ToList().Find(e => e.CodigoCuenta == r.CSFP_VentasCredito.Trim()).NombreCuenta.Trim() },
                        TipoCuentaEDD = new CATipoCuentaDC() { CodigoCuenta = r.CSFP_EfectivasTipoDebidoDebito.Trim(), CodigoNombreCuenta = r.CSFP_EfectivasTipoDebidoDebito.Trim() + " - " + lstTipoCuentaNovasoft.ToList().Find(e => e.CodigoCuenta == r.CSFP_EfectivasTipoDebidoDebito.Trim()).NombreCuenta.Trim() },
                        TipoCuentaEDC = new CATipoCuentaDC() { CodigoCuenta = r.CSFP_EfectivasTipoDebitoCredito.Trim(), CodigoNombreCuenta = r.CSFP_EfectivasTipoDebitoCredito.Trim() + " - " + lstTipoCuentaNovasoft.ToList().Find(e => e.CodigoCuenta == r.CSFP_EfectivasTipoDebitoCredito.Trim()).NombreCuenta.Trim() },
                        TipoCuentaECD = new CATipoCuentaDC() { CodigoCuenta = r.CSFP_EfectivasTipoCreditoCredito.Trim(), CodigoNombreCuenta = r.CSFP_EfectivasTipoCreditoCredito.Trim() + " - " + lstTipoCuentaNovasoft.ToList().Find(e => e.CodigoCuenta == r.CSFP_EfectivasTipoCreditoCredito.Trim()).NombreCuenta.Trim() },
                        TipoCuentaECC = new CATipoCuentaDC() { CodigoCuenta = r.CSFP_EfectivasTipoCreditoDebito.Trim(), CodigoNombreCuenta = r.CSFP_EfectivasTipoCreditoDebito.Trim() + " - " + lstTipoCuentaNovasoft.ToList().Find(e => e.CodigoCuenta == r.CSFP_EfectivasTipoCreditoDebito.Trim()).NombreCuenta.Trim() },
                        EstadoRegistro = Framework.Servidor.Comun.EnumEstadoRegistro.SIN_CAMBIOS
                    };

                    servicio.LstTipoCuentaNova = lstTipoCuentaNovasoft;
                    return servicio;
                });
                if (formas.Count != 0)
                {
                    return formas;
                }
                else
                {
                    List<CAServiciosFormaPagoDC> CuentasNovasoft = new List<CAServiciosFormaPagoDC>();
                    CuentasNovasoft.Add(new CAServiciosFormaPagoDC() { LstTipoCuentaNova = ObtenerCuentasNovasoft() });
                    return CuentasNovasoft;
                }
            }
        }

        /// <summary>
        /// Metodo para consultar los tipos de cuentas que se utilizan en la base de datos Novasoft
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<CATipoCuentaDC> ObtenerCuentasNovasoft()
        {
            ObservableCollection<CATipoCuentaDC> cuentas = new ObservableCollection<CATipoCuentaDC>();
            using (SqlConnection con = new SqlConnection("Server=172.20.10.157;initial catalog=ICONTROLLER;user id=usrbdmaster; password=InterS1st3m4s2014*-+"))//TODO: ALEJO Cambiar esta linea.
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM openquery([NOVASOFTPR], 'SELECT cod_cta,nom_cta FROM cnt_puc WHERE tip_cta = 2 AND cod_cta <> ''0''')", con);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader != null && reader.Read())
                    {
                        cuentas.Add(new CATipoCuentaDC()
                        {
                            CodigoCuenta = reader["cod_cta"].ToString().Trim(),
                            NombreCuenta = reader["nom_cta"].ToString(),
                            CodigoNombreCuenta = reader["cod_cta"].ToString().Trim() + " - " + reader["nom_cta"].ToString().Trim()
                        });
                    }
                }
                catch (Exception e)
                {

                    throw;
                }

            }
            return cuentas;
        }

        /// <summary>
        /// Metodo para Adicionar/Eliminar/Actualizar Parametrizaciones de servicios segun forma de pago y cuentas Novasoft
        /// </summary>
        public void ActualizacionRegistrosParametrizacionServicioFormaPagoNova(CAServiciosFormaPagoDC obj)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                switch (obj.EstadoRegistro)
                {
                    case EnumEstadoRegistro.SIN_CAMBIOS:
                        break;
                    case EnumEstadoRegistro.ADICIONADO:
                        ContabilizacionServicioFormaPago_TAR cServiFormaPagoADD = new ContabilizacionServicioFormaPago_TAR()
                        {
                            CSFP_IdTipoServicio = obj.TipoServicio.IdServicio,
                            CSFP_IdFormaPago = obj.LstFormaPago.First(s => s.IdFormaPagoInt == obj.FormaPagoNova.IdFormaPagoInt).IdFormaPago,
                            CSFP_VentasDebito = obj.TipoCuentaVD.CodigoCuenta.ToString(),
                            CSFP_VentasCredito = obj.TipoCuentaVC.CodigoCuenta.ToString(),
                            CSFP_EfectivasTipoDebidoDebito = obj.TipoCuentaEDD.CodigoCuenta.ToString(),
                            CSFP_EfectivasTipoDebitoCredito = obj.TipoCuentaEDC.CodigoCuenta.ToString(),
                            CSFP_EfectivasTipoCreditoDebito = obj.TipoCuentaECD.CodigoCuenta.ToString(),
                            CSFP_EfectivasTipoCreditoCredito = obj.TipoCuentaECC.CodigoCuenta.ToString(),
                            CSFP_CreadoPor = ControllerContext.Current.Usuario,
                            CSFP_FechaCreacion = DateTime.Now
                        };
                        contexto.ContabilizacionServicioFormaPago_TAR.Add(cServiFormaPagoADD);
                        contexto.SaveChanges();
                        break;
                    case EnumEstadoRegistro.MODIFICADO:
                        ContabilizacionServicioFormaPago_TAR cServiFormaPagoUpdate = contexto.ContabilizacionServicioFormaPago_TAR.Where(s => s.CSFP_IdContabilidadServicioPago == obj.IdServicio).FirstOrDefault();
                        if (cServiFormaPagoUpdate == null)
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_SERVICIO_NO_VALIDO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_SERVICIO_NO_VALIDO));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                        else
                        {
                            cServiFormaPagoUpdate.CSFP_IdTipoServicio = obj.TipoServicio.IdServicio;
                            cServiFormaPagoUpdate.CSFP_VentasDebito = obj.TipoCuentaVD.CodigoCuenta;
                            cServiFormaPagoUpdate.CSFP_VentasCredito = obj.TipoCuentaVC.CodigoCuenta;
                            cServiFormaPagoUpdate.CSFP_EfectivasTipoDebidoDebito = obj.TipoCuentaEDD.CodigoCuenta;
                            cServiFormaPagoUpdate.CSFP_EfectivasTipoDebitoCredito = obj.TipoCuentaEDC.CodigoCuenta;
                            cServiFormaPagoUpdate.CSFP_EfectivasTipoCreditoDebito = obj.TipoCuentaECD.CodigoCuenta;
                            cServiFormaPagoUpdate.CSFP_EfectivasTipoCreditoCredito = obj.TipoCuentaECC.CodigoCuenta;
                            cServiFormaPagoUpdate.CSFP_IdFormaPago = Convert.ToSByte(obj.FormaPagoNova.IdFormaPagoInt);
                            contexto.SaveChanges();
                        }

                        break;
                    case EnumEstadoRegistro.BORRADO:
                        ContabilizacionServicioFormaPago_TAR cServiFormaPagoDelete = contexto.ContabilizacionServicioFormaPago_TAR.FirstOrDefault(s => s.CSFP_IdContabilidadServicioPago == obj.IdServicio);
                        if (cServiFormaPagoDelete != null)
                        {
                            contexto.ContabilizacionServicioFormaPago_TAR.Remove(cServiFormaPagoDelete);
                            contexto.SaveChanges();
                        }
                        break;
                }
            }
        }
        #endregion Consultas Generales

        #region Tipos de Envío

        /// <summary>
        /// Obtiene los tipos de envío que están en la base de datos
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de envío</returns>
        public IEnumerable<TATipoEnvio> ObtenerTiposEnvio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoEnvio_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TATipoEnvio
                  {
                      IdTipoEnvio = r.TEN_IdTipoEnvio,
                      Nombre = r.TEN_Nombre,
                      Descripcion = r.TEN_Descripcion,
                      PesoMinimo = r.TEN_PesoMinimo,
                      PesoMaximo = r.TEN_PesoMaximo,
                      CodigoMinisterio = r.TEN_CodigoMinisterio,
                  });
            }
        }

        /// <summary>
        ///  Inserta tipos de envío en la Base de Datos
        /// </summary>
        /// <param name="rol">Objeto tipo envío</param>
        public void AdicionarTiposEnvio(TATipoEnvio tipoEnvio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEnvio_TAR envioEn = new TipoEnvio_TAR()
                {
                    TEN_Nombre = tipoEnvio.Nombre.Trim().ToUpper(),
                    TEN_Descripcion = tipoEnvio.Descripcion.Trim().ToUpper(),
                    TEN_PesoMinimo = tipoEnvio.PesoMinimo,
                    TEN_PesoMaximo = tipoEnvio.PesoMaximo,
                    TEN_CodigoMinisterio = tipoEnvio.CodigoMinisterio,
                    TEN_FechaGrabacion = DateTime.Now,
                    TEN_CreadoPor = ControllerContext.Current.Usuario,
                    TEN_Estado = ConstantesFramework.ESTADO_ACTIVO
                };
                contexto.TipoEnvio_TAR.Add(envioEn);
                contexto.SaveChanges();
            }
        }


        /// <summary>
        /// Obtiene los servicios de rapicarga, Rapi Carga Terrestre y mensajeria por municipio origen y destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public List<int> ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(string municipioOrigen, string municipioDestino)
        {
            List<int> lstIdServicios = new List<int>();
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = new SqlCommand("paObtenerListaServiciosValidacionMensajeriaMayorPeso", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {                    
                    lstIdServicios.Add(Convert.ToInt32(reader["STR_IdServicio"] == DBNull.Value ? 0 : reader["STR_IdServicio"]));
                }
                conn.Close();
            }
            return lstIdServicios;
        }

        /// <summary>
        /// Edita tipos de envío en la base de datos
        /// </summary>
        /// <param name="rol">Objeto tipo envío</param>
        public void EditarTiposEnvio(TATipoEnvio tipoEnvio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEnvio_TAR envioEn = contexto.TipoEnvio_TAR
                  .Where(r => r.TEN_IdTipoEnvio == tipoEnvio.IdTipoEnvio)
                  .FirstOrDefault();

                if (envioEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                envioEn.TEN_Nombre = tipoEnvio.Nombre.Trim().ToUpper();
                envioEn.TEN_Descripcion = tipoEnvio.Descripcion.Trim().ToUpper();
                envioEn.TEN_PesoMinimo = tipoEnvio.PesoMinimo;
                envioEn.TEN_PesoMaximo = tipoEnvio.PesoMaximo;
                envioEn.TEN_CodigoMinisterio = tipoEnvio.CodigoMinisterio;
                TARepositorioAudit.MapearTipoEnvio(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina tipos de envío en la base de datos
        /// </summary>
        /// <param name="rol">Objeto tipo envío</param>
        public void EliminarTiposEnvio(TATipoEnvio tipoEnvio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEnvio_TAR envioEn = contexto.TipoEnvio_TAR.Where(r => r.TEN_IdTipoEnvio == tipoEnvio.IdTipoEnvio).First();
                contexto.TipoEnvio_TAR.Remove(envioEn);
                TARepositorioAudit.MapearTipoEnvio(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Retorna una lista con los tipos de envio
        /// </summary>
        /// <returns></returns>
        public List<TATipoEnvio> ObtenerTipoEnvios()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoEnvio_TAR.Where(r => r.TEN_Estado == TAConstantesTarifas.ID_ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(r => new TATipoEnvio()
                  {
                      IdTipoEnvio = r.TEN_IdTipoEnvio,
                      Nombre = r.TEN_Nombre,
                      Descripcion = r.TEN_Descripcion,
                      PesoMinimo = r.TEN_PesoMinimo,
                      PesoMaximo = r.TEN_PesoMaximo
                  });
            }
        }

        #endregion Tipos de Envío

        #region Tipos de Moneda

        /// <summary>
        /// Obtiene los tipos de moneda
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de moneda</returns>
        public IEnumerable<TAMonedaDC> ObtenerMoneda(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsMoneda_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TAMonedaDC
                  {
                      IdMoneda = r.MON_IdMoneda,
                      Descripcion = r.MON_Descripcion,
                      TasaCambio = r.MON_TasaCambio
                  });
            }
        }

        /// <summary>
        /// Adiciona Moneda
        /// </summary>
        /// <param name="moneda">Objeto Moneda</param>
        public void AdicionarMoneda(TAMonedaDC moneda)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Moneda_TAR monedaEn = new Moneda_TAR()
                {
                    MON_IdMoneda = moneda.IdMoneda.ToUpper(),
                    MON_Descripcion = moneda.Descripcion,
                    MON_TasaCambio = moneda.TasaCambio,
                    MON_FechaGrabacion = DateTime.Now,
                    MON_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.Moneda_TAR.Add(monedaEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita moneda
        /// </summary>
        /// <param name="moneda">Objeto moneda</param>
        public void EditarMoneda(TAMonedaDC moneda)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Moneda_TAR monedaEn = contexto.Moneda_TAR
                  .Where(r => r.MON_IdMoneda == moneda.IdMoneda)
                  .FirstOrDefault();

                if (monedaEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                monedaEn.MON_Descripcion = moneda.Descripcion;
                monedaEn.MON_TasaCambio = moneda.TasaCambio;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina Moneda
        /// </summary>
        /// <param name="moneda">Objeto moneda</param>
        public void EliminarMoneda(TAMonedaDC moneda)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Moneda_TAR monedaEn = contexto.Moneda_TAR.Where(r => r.MON_IdMoneda == moneda.IdMoneda).First();
                contexto.Moneda_TAR.Remove(monedaEn);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Moneda

        #region Tipos de Empaque

        /// <summary>
        /// Obtiene los tipos de empaque
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de empaque</returns>
        public IEnumerable<TATipoEmpaque> ObtenerTiposEmpaque(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoEmpaque_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TATipoEmpaque
                  {
                      IdTipoEmpaque = r.TEM_IdTipoEmpaque,
                      Descripcion = r.TEM_Descripcion
                  });
            }
        }

        /// <summary>
        /// Adicionar un tipo de empaque
        /// </summary>
        /// <param name="tipoEmpaque">Objeto tipo de empaque</param>
        public void AdicionarTiposEmpaque(TATipoEmpaque tipoEmpaque)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEmpaque_TAR tipoEmpaqueEn = new TipoEmpaque_TAR()
                {
                    TEM_IdTipoEmpaque = tipoEmpaque.IdTipoEmpaque,
                    TEM_Descripcion = tipoEmpaque.Descripcion,
                    TEM_FechaGrabacion = DateTime.Now,
                    TEM_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.TipoEmpaque_TAR.Add(tipoEmpaqueEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Editar tipo de empaque
        /// </summary>
        /// <param name="tipoEmpaque">Objeto tipo de empaque</param>
        public void EditarTiposEmpaque(TATipoEmpaque tipoEmpaque)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEmpaque_TAR tipoEn = contexto.TipoEmpaque_TAR
                  .Where(r => r.TEM_IdTipoEmpaque == tipoEmpaque.IdTipoEmpaque)
                  .FirstOrDefault();

                if (tipoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                tipoEn.TEM_Descripcion = tipoEmpaque.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina tipo de empaque
        /// </summary>
        /// <param name="tipoEmpaque"></param>
        public void EliminarTiposEmpaque(TATipoEmpaque tipoEmpaque)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEmpaque_TAR tipoEn = contexto.TipoEmpaque_TAR.Where(r => r.TEM_IdTipoEmpaque == tipoEmpaque.IdTipoEmpaque).First();
                contexto.TipoEmpaque_TAR.Remove(tipoEn);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Empaque

        #region Tipos de Trámites

        /// <summary>
        /// Obtiene los tipos de trámite
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de trámite</returns>
        public IEnumerable<TATipoTramite> ObtenerTiposTramite(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoTramite_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TATipoTramite
                  {
                      IdTipoTramite = r.TIT_IdTipoTramite,
                      Descripcion = r.TIT_Descripcion
                  });
            }
        }

        /// <summary>
        /// Adiciona un tipo de trámite
        /// </summary>
        /// <param name="tipoTramite">Objeto tipo trámite</param>
        public void AdicionarTiposTramite(TATipoTramite tipoTramite)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoTramite_TAR tipoEn = new TipoTramite_TAR()
                {
                    TIT_Descripcion = tipoTramite.Descripcion,
                    TIT_FechaGrabacion = DateTime.Now,
                    TIT_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.TipoTramite_TAR.Add(tipoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de trámite
        /// </summary>
        /// <param name="tipoTramite"></param>
        public void EditarTiposTramite(TATipoTramite tipoTramite)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoTramite_TAR tipoEn = contexto.TipoTramite_TAR
                  .Where(r => r.TIT_IdTipoTramite == tipoTramite.IdTipoTramite)
                  .FirstOrDefault();

                if (tipoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                tipoEn.TIT_Descripcion = tipoTramite.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de trámite
        /// </summary>
        /// <param name="tipoTramite">Objeto tipo trámite</param>
        public void EliminarTiposTramite(TATipoTramite tipoTramite)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoTramite_TAR tipoEn = contexto.TipoTramite_TAR.Where(r => r.TIT_IdTipoTramite == tipoTramite.IdTipoTramite).First();
                contexto.TipoTramite_TAR.Remove(tipoEn);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Trámites

        #region Tipos de Valor Adicional

        /// <summary>
        /// Obtiene los tipos de valor adicional
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de valor adicional</returns>
        public IEnumerable<TAValorAdicional> ObtenerValorAdicional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoValorAdicional_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Where(v => v.TVA_IdTipoValorAdicional != TAConstantesTarifas.ID_TIPO_VALOR_ADICIONAL_RETORNO && v.TVA_IdTipoValorAdicional != TAConstantesTarifas.ID_TIPO_VALOR_ADICIONAL_RETORNO_RAPI_CARGA_CONTRAPAGO && v.TVA_IdTipoValorAdicional != TAConstantesTarifas.ID_TIPO_VALOR_ADICIONAL_SERVICIO_RAPIRADICADO)
                  .ToList()
                  .ConvertAll(r => new TAValorAdicional
                  {
                      IdTipoValorAdicional = r.TVA_IdTipoValorAdicional,
                      Descripcion = r.TVA_Descripcion,
                      IdServicio = r.TVA_IdServicio
                  });
            }
        }

        /// <summary>
        /// Retorna los valores adicionales que son de tipo embalaje
        /// </summary>
        /// <returns></returns>
        public List<TAValorAdicional> ConsultarValoresAdicionalesEmbalaje(long idListaPrecios)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PrecioValorAdicional_VTAR.Where(v => v.TVA_EsEmbalaje == true && v.LPS_IdListaPrecios == idListaPrecios && v.TVA_IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA).ToList().ConvertAll<TAValorAdicional>(v =>
                {
                    return new TAValorAdicional()
                    {
                        Descripcion = v.TVA_Descripcion,
                        IdTipoValorAdicional = v.TVA_IdTipoValorAdicional,
                        PrecioValorAdicional = v.PVA_Valor
                    };
                });
            }
        }

        /// <summary>
        /// Adiciona un valor adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto valor adicional</param>
        public void AdicionarValorAdicional(TAValorAdicional valorAdicional)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoValorAdicional_TAR valorEn = new TipoValorAdicional_TAR()
                {
                    TVA_IdTipoValorAdicional = valorAdicional.IdTipoValorAdicional.ToUpper(),
                    TVA_Descripcion = valorAdicional.Descripcion,
                    TVA_IdServicio = valorAdicional.IdServicio,
                    TVA_FechaGrabacion = DateTime.Now,
                    TVA_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.TipoValorAdicional_TAR.Add(valorEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de valor adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto valor adicional</param>
        public void EditarValorAdicional(TAValorAdicional valorAdicional)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoValorAdicional_TAR valorEn = contexto.TipoValorAdicional_TAR
                  .Where(r => r.TVA_IdTipoValorAdicional == valorAdicional.IdTipoValorAdicional)
                  .FirstOrDefault();

                if (valorEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                valorEn.TVA_Descripcion = valorAdicional.Descripcion;
                valorEn.TVA_IdServicio = valorAdicional.IdServicio;
                TARepositorioAudit.MapearTipoValorAdicional(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de valor adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto de valor adicional</param>
        public void EliminarValorAdicional(TAValorAdicional valorAdicional)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoValorAdicional_TAR valorEn = contexto.TipoValorAdicional_TAR.Where(r => r.TVA_IdTipoValorAdicional == valorAdicional.IdTipoValorAdicional).First();
                contexto.TipoValorAdicional_TAR.Remove(valorEn);
                TARepositorioAudit.MapearTipoValorAdicional(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Valor Adicional

        #region Tipos de Trayectos

        /// <summary>
        /// Obtiene los tipos de trayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de trayecto</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoTrayecto_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TATipoTrayecto
                  {
                      IdTipoTrayecto = r.TTR_IdTipoTrayecto,
                      Descripcion = r.TTR_Descripcion
                  });
            }
        }

        /// <summary>
        /// Adiciona un tipo de trayecto
        /// </summary>
        /// <param name="tipoTrayecto">Objeto tipo de trayecto</param>
        public void AdicionarTiposTrayecto(TATipoTrayecto tipoTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoTrayecto_TAR tipoEn = new TipoTrayecto_TAR()
                {
                    TTR_IdTipoTrayecto = tipoTrayecto.IdTipoTrayecto,
                    TTR_Descripcion = tipoTrayecto.Descripcion,
                    TTR_FechaGrabacion = DateTime.Now,
                    TTR_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.TipoTrayecto_TAR.Add(tipoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de trayecto
        /// </summary>
        /// <param name="tipoTrayecto">Objeto tipo de trayecto</param>
        public void EditarTiposTrayecto(TATipoTrayecto tipoTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoTrayecto_TAR tipoEn = contexto.TipoTrayecto_TAR
                  .Where(r => r.TTR_IdTipoTrayecto == tipoTrayecto.IdTipoTrayecto)
                  .FirstOrDefault();

                if (tipoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                tipoEn.TTR_Descripcion = tipoTrayecto.Descripcion;
                TARepositorioAudit.MapearTipoTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina tipo de trayecto
        /// </summary>
        /// <param name="tipoTrayecto">Objeto tipo de trayecto</param>
        public void EliminarTiposTrayecto(TATipoTrayecto tipoTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoTrayecto_TAR tipoEn = contexto.TipoTrayecto_TAR.Where(r => r.TTR_IdTipoTrayecto == tipoTrayecto.IdTipoTrayecto).First();
                contexto.TipoTrayecto_TAR.Remove(tipoEn);
                TARepositorioAudit.MapearTipoTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Trayectos

        #region Tipos de Subtrayecto

        /// <summary>
        /// Obtiene los tipos de Subtrayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de Subtrayecto</returns>
        public IEnumerable<TATipoSubTrayecto> ObtenerTiposSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoSubTrayecto_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Where(w => w.TST_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ConvertAll(r => new TATipoSubTrayecto
                  {
                      IdTipoSubTrayecto = r.TST_IdTipoSubTrayecto,
                      Descripcion = r.TST_Descripcion
                  });
            }
        }

        /// <summary>
        /// Adiciona tipo de subtrayecto
        /// </summary>
        /// <param name="subTrayecto">Objeto tipo subtrayecto</param>
        public void AdicionarTipoSubTrayecto(TATipoSubTrayecto subTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoSubTrayecto_TAR tipoEn = new TipoSubTrayecto_TAR()
                {
                    TST_IdTipoSubTrayecto = subTrayecto.IdTipoSubTrayecto,
                    TST_Descripcion = subTrayecto.Descripcion,
                    TST_FechaGrabacion = DateTime.Now,
                    TST_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.TipoSubTrayecto_TAR.Add(tipoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita tipo de subtrayecto
        /// </summary>
        /// <param name="subTrayecto">Objeto tipo de subtrayecto</param>
        public void EditarTipoSubTrayecto(TATipoSubTrayecto subTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoSubTrayecto_TAR tipoEn = contexto.TipoSubTrayecto_TAR
                  .Where(r => r.TST_IdTipoSubTrayecto == subTrayecto.IdTipoSubTrayecto)
                  .FirstOrDefault();

                if (tipoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                tipoEn.TST_Descripcion = subTrayecto.Descripcion;
                TARepositorioAudit.MapearTipoSubTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de subtrayecto
        /// </summary>
        /// <param name="subTrayecto"></param>
        public void EliminarTiposSubTrayecto(TATipoSubTrayecto subTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoSubTrayecto_TAR tipoEn = contexto.TipoSubTrayecto_TAR.Where(r => r.TST_IdTipoSubTrayecto == subTrayecto.IdTipoSubTrayecto).First();
                contexto.TipoSubTrayecto_TAR.Remove(tipoEn);
                TARepositorioAudit.MapearTipoSubTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Subtrayecto

        #region Tipos de Impuesto

        /// <summary>
        /// Obtiene los tipos de impuesto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de impuesto</returns>
        public IEnumerable<TAImpuestosDC> ObtenerTiposImpuesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsImpuesto_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TAImpuestosDC
                  {
                      Identificador = r.IMP_IdImpuesto,
                      Descripcion = r.IMP_Descripcion,
                      Valor = r.IMP_Valor,
                      CuentaExterna = new TACuentaExternaDC()
                      {
                          IdCuentaExterna = r.IMP_IdCuentaExterna ?? 0,
                          Descripcion = ObtenerDescripcionCuentaExterna(contexto, r.IMP_IdCuentaExterna ?? 0)
                      }
                  });
            }
        }

        private string ObtenerDescripcionCuentaExterna(EntidadesTarifas contexto, short idCuentaExterna)
        {
            if (idCuentaExterna == 0)
            {
                return String.Empty;
            }

            CuentaExterna_PAR cuenta = contexto.CuentaExterna_PAR.Where(cu => cu.CEX_IdCuentaExterna == idCuentaExterna)
              .FirstOrDefault();

            return cuenta == null ? String.Empty : cuenta.CEX_Descripcion;
        }

        /// <summary>
        /// Adiciona un tipo de impuesto
        /// </summary>
        /// <param name="tipoImpuesto">Objeto tipo de impuesto</param>
        public void AdicionarTipoImpuesto(TAImpuestosDC tipoImpuesto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Impuesto_TAR impuestoEn = new Impuesto_TAR()
                {
                    IMP_Descripcion = tipoImpuesto.Descripcion,
                    IMP_Valor = tipoImpuesto.Valor,
                    IMP_IdCuentaExterna = tipoImpuesto.CuentaExterna.IdCuentaExterna,
                    IMP_FechaGrabacion = DateTime.Now,
                    IMP_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.Impuesto_TAR.Add(impuestoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un tipo de impuesto
        /// </summary>
        /// <param name="tipoImpuesto">Objeto tipo de impuesto</param>
        public void EditarTipoImpuesto(TAImpuestosDC tipoImpuesto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Impuesto_TAR impuestoEn = contexto.Impuesto_TAR
                  .Where(r => r.IMP_IdImpuesto == tipoImpuesto.Identificador)
                  .FirstOrDefault();

                if (impuestoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                impuestoEn.IMP_Descripcion = tipoImpuesto.Descripcion;
                impuestoEn.IMP_Valor = tipoImpuesto.Valor;
                impuestoEn.IMP_IdCuentaExterna = tipoImpuesto.CuentaExterna.IdCuentaExterna;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un tipo de envío
        /// </summary>
        /// <param name="tipoImpuesto">Objeto tipo de envío</param>
        public void EliminarTipoImpuesto(TAImpuestosDC tipoImpuesto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Impuesto_TAR impuestoEn = contexto.Impuesto_TAR.Where(r => r.IMP_IdImpuesto == tipoImpuesto.Identificador).FirstOrDefault();
                contexto.Impuesto_TAR.Remove(impuestoEn);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Impuesto

        #region Tipos  Cuenta Externa

        /// <summary>
        /// Obtiene los tipos de cuenta externa
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de cuenta externa</returns>
        public IEnumerable<TACuentaExternaDC> ObtenerCuentaExternaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsCuentaExterna_PAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TACuentaExternaDC()
                  {
                      IdCuentaExterna = r.CEX_IdCuentaExterna,
                      Descripcion = r.CEX_Descripcion,
                      IdNaturaliza = r.CEX_IdNaturaleza,
                      Codigo = r.CEX_Codigo,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona una cuenta externa
        /// </summary>
        public void AdicionarCuentaExterna(TACuentaExternaDC cuentaExterna)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CuentaExterna_PAR cuentaExt = new CuentaExterna_PAR()
                {
                    CEX_IdCuentaExterna = cuentaExterna.IdCuentaExterna,
                    CEX_Descripcion = cuentaExterna.Descripcion,
                    CEX_IdNaturaleza = cuentaExterna.IdNaturaliza,
                    CEX_Codigo = cuentaExterna.Codigo,
                    CEX_FechaGrabacion = DateTime.Now,
                    CEX_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.CuentaExterna_PAR.Add(cuentaExt);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        ///  Actualiza la cuenta externa
        /// </summary>
        /// <param name="cuentaExterna">clase</param>
        public void EditarCuentaExterna(TACuentaExternaDC cuentaExterna)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CuentaExterna_PAR cuentaExt = contexto.CuentaExterna_PAR
                  .Where(r => r.CEX_IdCuentaExterna == cuentaExterna.IdCuentaExterna)
                  .FirstOrDefault();
                cuentaExt.CEX_Descripcion = cuentaExterna.Descripcion;
                cuentaExt.CEX_IdNaturaleza = cuentaExterna.IdNaturaliza;
                cuentaExt.CEX_Codigo = cuentaExterna.Codigo;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina una cuenta externa
        /// </summary>
        public void EliminarCuentaExterna(TACuentaExternaDC cuentaExterna)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CuentaExterna_PAR cuentaExt = contexto.CuentaExterna_PAR
                  .Where(r => r.CEX_IdCuentaExterna == cuentaExterna.IdCuentaExterna)
                  .FirstOrDefault();

                contexto.CuentaExterna_PAR.Remove(cuentaExt);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos  Cuenta Externa

        #region Tipos de Entrega

        /// <summary>
        /// Obtiene los tipos de estrega
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Obtiene los tipos de estrega</returns>
        public IEnumerable<TATipoEntrega> ObtenerTipoEntregaFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoEntrega_MEN(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .ToList()
                    .ConvertAll(r => new TATipoEntrega()
                    {
                        IdTipoEntrega = r.TIE_IdTipoEntrega,
                        Descripcion = r.TIE_Descripcion,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                    });
            }
        }

        /// <summary>
        /// Adiciona un tipo entrega
        /// </summary>
        ///
        public void AdicionarTipoEntrega(TATipoEntrega tipoEntrega)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEntrega_MEN tipo = new TipoEntrega_MEN
                {
                    TIE_CreadoPor = ControllerContext.Current.Usuario,
                    TIE_Descripcion = tipoEntrega.Descripcion,
                    TIE_FechaGrabacion = DateTime.Now,
                    TIE_IdTipoEntrega = tipoEntrega.IdTipoEntrega
                };

                contexto.TipoEntrega_MEN.Add(tipo);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        ///   Actualizar infomacion tipo entrega
        /// </summary>
        /// <param name="tipoEntrega">clase</param>
        public void EditarTipoEntrega(TATipoEntrega tipoEntrega)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEntrega_MEN tipoEnt = contexto.TipoEntrega_MEN
                    .Where(r => r.TIE_IdTipoEntrega == tipoEntrega.IdTipoEntrega)
                    .FirstOrDefault();
                tipoEnt.TIE_IdTipoEntrega = tipoEntrega.IdTipoEntrega;
                tipoEnt.TIE_Descripcion = tipoEntrega.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Eliminar Tipo entrega MEN
        /// </summary>
        /// <param name="tipoEntrega">clase</param>
        public void EliminarTipoEntrega(TATipoEntrega tipoEntrega)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoEntrega_MEN tipoEnt = contexto.TipoEntrega_MEN
                    .Where(r => r.TIE_IdTipoEntrega == tipoEntrega.IdTipoEntrega)
                    .FirstOrDefault();
                contexto.TipoEntrega_MEN.Remove(tipoEnt);
                contexto.SaveChanges();
            }
        }

        #endregion Tipos de Entrega

        #region Trayectos

        /// <summary>
        /// Obtiene los tipos de impuesto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de impuesto</returns>
        public IEnumerable<TATrayectoDC> ObtenerTrayectos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                campoOrdenamiento = "LOC_Nombre";

                return contexto.ConsultarEqualsTrayectServicio_VTAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .Where(a => a.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ConvertAll(r => new TATrayectoDC()
                  {
                      IdTrayecto = r.STR_IdTrayecto,
                      Trayecto = new TATipoTrayecto()
                      {
                          IdTipoTrayecto = r.TRS_IdTipoTrayecto,
                          Descripcion = r.TTR_Descripcion
                      },
                      SubTrayecto = new TATipoSubTrayecto()
                      {
                          IdTipoSubTrayecto = r.TRS_IdTipoSubTrayecto,
                          Descripcion = r.TST_Descripcion
                      },
                      IdLocalidadOrigen = r.TRA_IdLocalidadOrigen,
                      IdLocalidadDestino = r.TRA_IdLocalidadDestino,
                      NombreLocalidadDestino = r.LOC_Nombre,

                      Servicios = new List<TAServicioDC>() {
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_HOY, TiempoEntrega = r.C1??0,Asignado = (r.C1 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_AM,TiempoEntrega = r.C2??0,Asignado = (r.C2 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_MENSAJERIA,TiempoEntrega = r.C3??0,Asignado = (r.C3 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_MASIVOS,TiempoEntrega = r.C4??0,Asignado = (r.C4 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_PROMOCIONAL,TiempoEntrega = r.C5??0,Asignado = (r.C5 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA,TiempoEntrega = r.C6??0,Asignado = (r.C6 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA_CONTRA_PAGO,TiempoEntrega = r.C7??0,Asignado = (r.C7 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_GIRO,TiempoEntrega = r.C8??0,Asignado = (r.C8 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_INTER_VIAJES,TiempoEntrega = r.C9??0,Asignado = (r.C9 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_TRAMITES,TiempoEntrega = r.C10??0,Asignado = (r.C10 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_INTERNACIONAL,TiempoEntrega = r.C11??0,Asignado = (r.C11 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_CENTRO_CORRESPONDENCIA,TiempoEntrega = r.C12??0,Asignado = (r.C12 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_PERSONALIZADO,TiempoEntrega = r.C13??0,Asignado = (r.C13 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_ENVIOS_CONTRAPAGO,TiempoEntrega = r.C14??0,Asignado = (r.C14 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_NOTIFICACIONES,TiempoEntrega = r.C15??0,Asignado = (r.C15 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPIRADICADO,TiempoEntrega = r.C16??0,Asignado = (r.C16 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_CARGA_EXPRESS,TiempoEntrega = r.C17??0,Asignado = (r.C17 == null) ? false : true},

              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_TULAS,TiempoEntrega = r.C19??0,Asignado = (r.C19 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_VALORES_MENSAJERIA,TiempoEntrega = r.C20??0,Asignado = (r.C20 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_VALORES_CARGA,TiempoEntrega = r.C21??0,Asignado = (r.C21 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_CARGA_CONSOLIDADA,TiempoEntrega = r.C22??0,Asignado = (r.C22 == null) ? false : true},
              new TAServicioDC(){IdServicio = TAConstantesServicios.SERVICIO_RAPI_VALIJAS,TiempoEntrega = r.C23??0,Asignado = (r.C23 == null) ? false : true}
            },

                      IdTrayectoSubTrayecto = r.TRA_IdTrayectoSubTrayecto,
                      CiudadDestinoEditable = false,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona un trayecto
        /// </summary>
        /// <param name="trayecto">Objeto Trayecti</param>
        public void AdicionarTrayecto(TATrayectoDC trayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (ValidarTrayectoSubTrayecto(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == true && trayecto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES)));

                TrayectoSubTrayecto_TAR trayectoSubtrayectoEn = contexto.TrayectoSubTrayecto_TAR
                  .Where(r => r.TRS_IdTipoTrayecto == trayecto.Trayecto.IdTipoTrayecto && r.TRS_IdTipoSubTrayecto == trayecto.SubTrayecto.IdTipoSubTrayecto)
                  .FirstOrDefault();

                if (trayectoSubtrayectoEn == null)
                {
                    TrayectoSubTrayecto_TAR ts = new TrayectoSubTrayecto_TAR()
                    {
                        TRS_IdTipoTrayecto = trayecto.Trayecto.IdTipoTrayecto,
                        TRS_IdTipoSubTrayecto = trayecto.SubTrayecto.IdTipoSubTrayecto,
                        TRS_FechaGrabacion = DateTime.Now,
                        TRS_CreadoPor = ControllerContext.Current.Usuario
                    };
                    contexto.TrayectoSubTrayecto_TAR.Add(ts);
                    trayecto.IdTrayectoSubTrayecto = ts.TRS_IdTrayectoSubTrayecto;
                }
                else if (trayectoSubtrayectoEn != null)
                {
                    trayecto.IdTrayectoSubTrayecto = trayectoSubtrayectoEn.TRS_IdTrayectoSubTrayecto;
                }

                AdicionarServicioTrayecto(trayecto, trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino, contexto);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona Servicio Trayecto
        /// </summary>
        /// <param name="trayecto">Trayecto</param>
        /// <param name="contexto">Contexto</param>
        private static void AdicionarServicioTrayecto(TATrayectoDC trayecto, string origen, string destino, EntidadesTarifas contexto)
        {
            Trayecto_TAR trayectoEn = new Trayecto_TAR()
            {
                TRA_IdLocalidadOrigen = origen,
                TRA_IdLocalidadDestino = destino,
                TRA_IdTrayectoSubTrayecto = trayecto.IdTrayectoSubTrayecto,
                TRA_FechaGrabacion = DateTime.Now,
                TRA_CreadoPor = ControllerContext.Current.Usuario
            };

            contexto.Trayecto_TAR.Add(trayectoEn);

            trayecto.Servicios.ToList().ForEach(s =>
            {
                if (s.Asignado == true)
                {
                    ServicioTrayecto_TAR servicioTrayecto = new ServicioTrayecto_TAR()
                    {
                        STR_IdTrayecto = trayectoEn.TRA_IdTrayecto,
                        STR_IdServicio = s.IdServicio,
                        STR_TiempoEntrega = s.TiempoEntrega ?? 0,
                        STR_FechaGrabacion = DateTime.Now,
                        STR_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.ServicioTrayecto_TAR.Add(servicioTrayecto);
                }
            });
        }

        /// <summary>
        /// Edita un trayecto
        /// </summary>
        /// <param name="trayecto">Objeto Trayecto</param>
        public void EditarTrayecto(TATrayectoDC trayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Trayecto_TAR trayectoEn = contexto.Trayecto_TAR
                  .Where(r => r.TRA_IdTrayecto == trayecto.IdTrayecto)
                  .FirstOrDefault();

                if (ObtenerIdentificadorTrayectoSubtrayecto(trayecto.Trayecto.IdTipoTrayecto, trayecto.SubTrayecto.IdTipoSubTrayecto) != 0)
                    trayectoEn.TRA_IdTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(trayecto.Trayecto.IdTipoTrayecto, trayecto.SubTrayecto.IdTipoSubTrayecto);
                else
                {
                    TrayectoSubTrayecto_TAR trayectoSubTrayecto = new TrayectoSubTrayecto_TAR()
                    {
                        TRS_IdTipoTrayecto = trayecto.Trayecto.IdTipoTrayecto,
                        TRS_IdTipoSubTrayecto = trayecto.SubTrayecto.IdTipoSubTrayecto,
                        TRS_FechaGrabacion = DateTime.Now,
                        TRS_CreadoPor = ControllerContext.Current.Usuario
                    };
                    contexto.TrayectoSubTrayecto_TAR.Add(trayectoSubTrayecto);
                    trayectoEn.TRA_IdTrayectoSubTrayecto = trayectoSubTrayecto.TRS_IdTrayectoSubTrayecto;
                }

                trayecto.Servicios.ToList().ForEach(s =>
                {
                    ServicioTrayecto_TAR servicioTrayecto = contexto.ServicioTrayecto_TAR
                      .Where(st => st.STR_IdTrayecto == trayecto.IdTrayecto && st.STR_IdServicio == s.IdServicio)
                      .FirstOrDefault();

                    if (servicioTrayecto == null && s.Asignado == true)
                    {
                        ServicioTrayecto_TAR servicioTrayectoAdd = new ServicioTrayecto_TAR()
                        {
                            STR_IdTrayecto = trayecto.IdTrayecto,
                            STR_IdServicio = s.IdServicio,
                            STR_TiempoEntrega = s.TiempoEntrega ?? 0,
                            STR_FechaGrabacion = DateTime.Now,
                            STR_CreadoPor = ControllerContext.Current.Usuario
                        };

                        contexto.ServicioTrayecto_TAR.Add(servicioTrayectoAdd);
                    }
                    else if (servicioTrayecto != null && s.Asignado == false)
                    {
                        ServicioTrayecto_TAR servicioTrayectoRemove = contexto.ServicioTrayecto_TAR
                          .Where(str => str.STR_IdTrayecto == trayecto.IdTrayecto && str.STR_IdServicio == s.IdServicio)
                          .FirstOrDefault();

                        contexto.ServicioTrayecto_TAR.Remove(servicioTrayectoRemove);
                    }
                    else if (servicioTrayecto != null && s.Asignado == true)
                    {
                        servicioTrayecto.STR_TiempoEntrega = s.TiempoEntrega ?? 0;
                    }
                });

                TARepositorioAudit.MapearServicioTrayecto(contexto);
                TARepositorioAudit.MapearTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un trayecto
        /// </summary>
        /// <param name="trayecto">Objeto trayecto</param>
        public void EliminarTrayecto(TATrayectoDC trayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ServicioTrayecto_TAR> servicioTrayectoRemove = contexto.ServicioTrayecto_TAR
                  .Where(str => str.STR_IdTrayecto == trayecto.IdTrayecto).ToList();

                servicioTrayectoRemove.ToList().ForEach(f =>
                {
                    contexto.ServicioTrayecto_TAR.Remove(f);
                });

                Trayecto_TAR trayectoRemove = contexto.Trayecto_TAR
                  .Where(t => t.TRA_IdTrayecto == trayecto.IdTrayecto)
                  .FirstOrDefault();

                contexto.Trayecto_TAR.Remove(trayectoRemove);
                TARepositorioAudit.MapearServicioTrayecto(contexto);
                TARepositorioAudit.MapearTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene el identificador del trayecto subtrayecto
        /// </summary>
        /// <param name="idTipoTrayecto">Identificador Tipo Trayecto</param>
        /// <param name="idTipoSubTrayecto">Identificador Tipo Subtrayecto</param>
        /// <returns></returns>
        public int ObtenerIdentificadorTrayectoSubtrayecto(string idTipoTrayecto, string idTipoSubTrayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TrayectoSubTrayecto_TAR trayectoSubTrayectoEn = contexto.TrayectoSubTrayecto_TAR
                  .Where(r => r.TRS_IdTipoTrayecto == idTipoTrayecto && r.TRS_IdTipoSubTrayecto == idTipoSubTrayecto)
                  .FirstOrDefault();

                if (trayectoSubTrayectoEn != null)
                    return trayectoSubTrayectoEn.TRS_IdTrayectoSubTrayecto;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Obtiene el identificador de un trayecto
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="idTrayectoSubTrayecto">Identificador trayecto sub trayecto</param>
        /// <returns>Identificador trayecto</returns>
        public long ObtenerIdentificadorTrayecto(string idLocalidadOrigen, string idLocalidadDestino)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Trayecto_TAR trayecto = contexto.Trayecto_TAR.Include("TrayectoSubTrayecto_TAR")
                  .Where(r => r.TRA_IdLocalidadOrigen == idLocalidadOrigen && r.TRA_IdLocalidadDestino == idLocalidadDestino && r.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .FirstOrDefault();

                if (trayecto != null)
                    return trayecto.TRA_IdTrayecto;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Valida si existe kilo adicional
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <returns>Booleano</returns>
        public bool ValidarTrayectoKiloInicial(string idLocalidadOrigen, string idLocalidadDestino)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Trayecto_TAR consulta = contexto.Trayecto_TAR.Include("TrayectoSubTrayecto_TAR")
                  .Where(r => r.TRA_IdLocalidadOrigen == idLocalidadOrigen && r.TRA_IdLocalidadDestino == idLocalidadDestino && r.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .FirstOrDefault();

                if (consulta == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Valida si existe kilo adicional
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <returns>Booleano</returns>
        public bool ValidarTrayectoSubTrayecto(string idLocalidadOrigen, string idLocalidadDestino)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Trayecto_TAR consulta = contexto.Trayecto_TAR.Include("TrayectoSubTrayecto_TAR")
                  .Where(r => r.TRA_IdLocalidadOrigen == idLocalidadOrigen && r.TRA_IdLocalidadDestino == idLocalidadDestino && r.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                 .FirstOrDefault();

                if (consulta == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Adiciona un trayecto subtrayecto
        /// </summary>
        public void AdicionarTrayectoKiloInicial(TATrayectoDC trayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int idTrayectoSubTrayecto = 0;

                TrayectoSubTrayecto_TAR consulta = contexto.TrayectoSubTrayecto_TAR
                  .Where(r => r.TRS_IdTipoTrayecto == trayecto.Trayecto.IdTipoTrayecto && r.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).FirstOrDefault();

                if (consulta == null)
                {
                    TrayectoSubTrayecto_TAR trayectoSubTrayecto = new TrayectoSubTrayecto_TAR()
                    {
                        TRS_IdTipoTrayecto = trayecto.Trayecto.IdTipoTrayecto,
                        TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL,
                        TRS_FechaGrabacion = DateTime.Now,
                        TRS_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.TrayectoSubTrayecto_TAR.Add(trayectoSubTrayecto);

                    idTrayectoSubTrayecto = trayectoSubTrayecto.TRS_IdTrayectoSubTrayecto;
                }
                else if (consulta != null)
                    idTrayectoSubTrayecto = consulta.TRS_IdTrayectoSubTrayecto;

                Trayecto_TAR trayectoEn = new Trayecto_TAR()
                {
                    TRA_IdLocalidadOrigen = trayecto.IdLocalidadOrigen,
                    TRA_IdLocalidadDestino = trayecto.IdLocalidadDestino,
                    TRA_IdTrayectoSubTrayecto = idTrayectoSubTrayecto,
                    TRA_FechaGrabacion = DateTime.Now,
                    TRA_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.Trayecto_TAR.Add(trayectoEn);

                trayecto.Servicios.ToList().ForEach(s =>
                {
                    //Se coloca la Validacion para que solo agregue los servicios que se
                    //asignaron
                    if (s.Asignado)
                    {
                        ServicioTrayecto_TAR servicioTrayecto = new ServicioTrayecto_TAR()
                        {
                            STR_IdServicio = s.IdServicio,
                            STR_TiempoEntrega = 0,
                            STR_FechaGrabacion = DateTime.Now,
                            STR_CreadoPor = ControllerContext.Current.Usuario
                        };

                        contexto.ServicioTrayecto_TAR.Add(servicioTrayecto);
                    }
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un trayecto subtrayecto kilo inicial
        /// </summary>
        /// <param name="trayecto">Objeto Trayecto</param>
        public void EditarTrayectoKiloInicial(TATrayectoDC trayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Trayecto_TAR trayectoKiloInicial = contexto.Trayecto_TAR
                  .Where(t => t.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL && t.TRA_IdLocalidadOrigen == trayecto.IdLocalidadOrigen && t.TRA_IdLocalidadDestino == trayecto.IdLocalidadDestino)
                  .FirstOrDefault();

                TrayectoSubTrayecto_TAR consulta = contexto.TrayectoSubTrayecto_TAR
                 .Where(r => r.TRS_IdTipoTrayecto == trayecto.Trayecto.IdTipoTrayecto && r.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).FirstOrDefault();

                if (consulta == null)
                {
                    TrayectoSubTrayecto_TAR trayectoSubTrayecto = new TrayectoSubTrayecto_TAR()
                    {
                        TRS_IdTipoTrayecto = trayecto.Trayecto.IdTipoTrayecto,
                        TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL,
                        TRS_FechaGrabacion = DateTime.Now,
                        TRS_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.TrayectoSubTrayecto_TAR.Add(trayectoSubTrayecto);

                    trayectoKiloInicial.TRA_IdTrayectoSubTrayecto = trayectoSubTrayecto.TRS_IdTrayectoSubTrayecto;
                }
                else if (consulta != null)
                    trayectoKiloInicial.TRA_IdTrayectoSubTrayecto = consulta.TRS_IdTrayectoSubTrayecto;

                //Adiciona los servicios asignados al subtrayecto kilo inicial
                trayecto.Servicios.ToList().ForEach(s =>
                {
                    ServicioTrayecto_TAR existeServicioTrayecto = contexto.ServicioTrayecto_TAR.
                      FirstOrDefault(st => st.STR_IdTrayecto == trayectoKiloInicial.TRA_IdTrayecto
                                        && st.STR_IdServicio == s.IdServicio);

                    if (existeServicioTrayecto == null && s.Asignado == true)
                    {
                        ServicioTrayecto_TAR servicioTrayecto = new ServicioTrayecto_TAR()
                        {
                            STR_IdTrayecto = trayectoKiloInicial.TRA_IdTrayecto,
                            STR_IdServicio = s.IdServicio,
                            STR_TiempoEntrega = 0,
                            STR_FechaGrabacion = DateTime.Now,
                            STR_CreadoPor = ControllerContext.Current.Usuario
                        };

                        contexto.ServicioTrayecto_TAR.Add(servicioTrayecto);
                    }
                    else if (existeServicioTrayecto != null && s.Asignado == false)
                    {
                        ServicioTrayecto_TAR servicioTrayectoRemove = contexto.ServicioTrayecto_TAR
                          .FirstOrDefault(str => str.STR_IdTrayecto == trayectoKiloInicial.TRA_IdTrayecto
                                              && str.STR_IdServicio == s.IdServicio);

                        contexto.ServicioTrayecto_TAR.Remove(servicioTrayectoRemove);
                    }
                    else if (existeServicioTrayecto != null && s.Asignado == true)
                    {
                        existeServicioTrayecto.STR_TiempoEntrega = s.TiempoEntrega ?? 0;
                    }
                });

                TARepositorioAudit.MapearTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un trayecto kilo inicial
        /// </summary>
        /// <param name="trayecto">Objeto kilo inicial</param>
        public void EliminarTrayectoKiloInicial(TATrayectoDC trayecto)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Trayecto_TAR trayectoKiloInicial = contexto.Trayecto_TAR
                  .Where(t => t.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL && t.TRA_IdLocalidadOrigen == trayecto.IdLocalidadOrigen && t.TRA_IdLocalidadDestino == trayecto.IdLocalidadDestino)
                  .FirstOrDefault();

                if (trayectoKiloInicial != null)
                {
                    trayecto.IdTrayecto = trayectoKiloInicial.TRA_IdTrayecto;
                    EliminarTrayecto(trayecto);

                    TARepositorioAudit.MapearTrayecto(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Trayectos

        #region Lista de Precios

        /// <summary>
        /// Obtiene las listas de precio de la aplicación
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de listas de precio</returns>
        public IEnumerable<TAListaPrecioDC> ObtenerListaPrecio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, CO.Servidor.Tarifas.Datos.Modelo.OperadorLogico> where = new Dictionary<LambdaExpression, CO.Servidor.Tarifas.Datos.Modelo.OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ListaPrecios_TAR>("LIP_EsListaCliente", "False", CO.Servidor.Tarifas.Datos.Modelo.OperadorComparacion.Equal);
                where.Add(lamda, CO.Servidor.Tarifas.Datos.Modelo.OperadorLogico.And);

                return contexto.ConsultarListaPrecios_TAR(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .ToList()
                  .ConvertAll(r => new TAListaPrecioDC
                  {
                      IdListaPrecio = r.LIP_IdListaPrecios,
                      Nombre = r.LIP_Nombre,
                      Inicio = r.LIP_Inicio,
                      Fin = r.LIP_Fin,
                      Estado = r.LIP_Estado,
                      EstadoDescripcion = contexto.EstadoActivoInactivo_VFRM.Where(e => e.IdEstado == r.LIP_Estado).FirstOrDefault().Estado,
                      TarifaPlena = r.LIP_EsTarifaPlena,
                      IdMoneda = r.LIP_IdMoneda,
                      EsListaCliente = r.LIP_EsListaCliente,
                      IdListaPrecioBase = r.LIP_IdListaPreciosBase,
                      ServiciosAsignados = contexto.ListaPrecioServicio_TAR.Include("Servicio_TAR").OrderBy(o => o.Servicio_TAR.SER_Nombre).Where(l => l.LPS_IdListaPrecios == r.LIP_IdListaPrecios)
                      .ToList()
                        .ConvertAll(l => new TAListaPrecioServicio()
                        {
                            IdServicio = l.LPS_IdServicio,
                            Servicio = contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == l.LPS_IdServicio).FirstOrDefault().SER_Nombre,
                            Estado = l.LPS_Estado,
                            PrimaSeguro = l.LPS_PrimaSeguros,
                            UnidadNegocio = new TAUnidadNegocio()
                            {
                                IdUnidadNegocio = contexto.UnidadNegocio_TAR.Where(uni => uni.UNE_IdUnidad == (contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == l.LPS_IdServicio).FirstOrDefault().SER_IdUnidadNegocio)).FirstOrDefault().UNE_IdUnidad,
                                Nombre = contexto.UnidadNegocio_TAR.Where(uni => uni.UNE_IdUnidad == (contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == l.LPS_IdServicio).FirstOrDefault().SER_IdUnidadNegocio)).FirstOrDefault().UNE_Nombre
                            }
                        })
                  });
            }
        }

        /// <summary>
        /// Obtiene las listas de precio de la aplicación para un cliente
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de listas de precio</returns>
        public IEnumerable<TAListaPrecioDC> ObtenerListaPrecio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                filtro.Add("LIP_IdListaPrecios", idListaPrecio.ToString());
                return contexto.ConsultarEqualsListaPrecios_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .ToList()
                  .ConvertAll(r => new TAListaPrecioDC
                  {
                      IdListaPrecio = r.LIP_IdListaPrecios,
                      Nombre = r.LIP_Nombre,
                      Inicio = r.LIP_Inicio,
                      Fin = r.LIP_Fin,
                      Estado = r.LIP_Estado,
                      EstadoDescripcion = contexto.EstadoActivoInactivo_VFRM.Where(e => e.IdEstado == r.LIP_Estado).FirstOrDefault().Estado,
                      TarifaPlena = r.LIP_EsTarifaPlena,
                      IdMoneda = r.LIP_IdMoneda,
                      EsListaCliente = r.LIP_EsListaCliente,
                      IdListaPrecioBase = r.LIP_IdListaPreciosBase,
                      ServiciosAsignados = contexto.ListaPrecioServicio_TAR.Include("Servicio_TAR").OrderBy(o => o.Servicio_TAR.SER_Nombre).Where(l => l.LPS_IdListaPrecios == r.LIP_IdListaPrecios)
                      .ToList()
                        .ConvertAll(l => new TAListaPrecioServicio()
                        {
                            IdServicio = l.LPS_IdServicio,
                            Servicio = contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == l.LPS_IdServicio).FirstOrDefault().SER_Nombre,
                            PrimaSeguro = l.LPS_PrimaSeguros,
                            Estado = l.LPS_Estado,
                            UnidadNegocio = new TAUnidadNegocio()
                            {
                                IdUnidadNegocio = contexto.UnidadNegocio_TAR.Where(uni => uni.UNE_IdUnidad == (contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == l.LPS_IdServicio).FirstOrDefault().SER_IdUnidadNegocio)).FirstOrDefault().UNE_IdUnidad,
                                Nombre = contexto.UnidadNegocio_TAR.Where(uni => uni.UNE_IdUnidad == (contexto.Servicio_TAR.Where(ser => ser.SER_IdServicio == l.LPS_IdServicio).FirstOrDefault().SER_IdUnidadNegocio)).FirstOrDefault().UNE_Nombre
                            }
                        })
                  });
            }
        }

        /// <summary>
        /// Adiciona una lista de precio
        /// </summary>
        /// <param name="listaPrecio">Objeto lista de precio</param>
        public int AdicionarListaPrecio(TAListaPrecioDC listaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paGuardarListaPrecios_TAR(listaPrecio.Nombre
                  , listaPrecio.Inicio
                  , listaPrecio.Fin
                  , listaPrecio.Estado
                  , listaPrecio.TarifaPlena
                  , listaPrecio.IdMoneda
                  , listaPrecio.EsListaCliente
                  , listaPrecio.IdListaPrecioBase
                  , ControllerContext.Current.Usuario).FirstOrDefault().ListaPrecios.Value;
            }
        }

        /// <summary>
        /// Edita lista de precio
        /// </summary>
        /// <param name="listaPrecio">Objeto lista de precio</param>
        public void EditarListaPrecio(TAListaPrecioDC listaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecios_TAR listaEn = contexto.ListaPrecios_TAR
                  .Where(r => r.LIP_IdListaPrecios == listaPrecio.IdListaPrecio)
                  .FirstOrDefault();

                if (listaEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                listaEn.LIP_Nombre = listaPrecio.Nombre;
                listaEn.LIP_Inicio = listaPrecio.Inicio;
                listaEn.LIP_Fin = listaPrecio.Fin;
                listaEn.LIP_Estado = listaPrecio.Estado;
                listaEn.LIP_EsTarifaPlena = listaPrecio.TarifaPlena;
                listaEn.LIP_IdMoneda = listaPrecio.IdMoneda;
                listaEn.LIP_EsListaCliente = listaPrecio.EsListaCliente;
                listaEn.LIP_IdListaPreciosBase = listaPrecio.IdListaPrecioBase == 0 ? null : listaPrecio.IdListaPrecioBase;

                TARepositorioAudit.MapeoAuditListaPrecio(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Inactiva una lista de precio
        /// </summary>
        /// <param name="listaPrecio">Objeto lista precio</param>
        public void EliminarListaPrecio(TAListaPrecioDC listaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecios_TAR listaEn = contexto.ListaPrecios_TAR
                  .Where(r => r.LIP_IdListaPrecios == listaPrecio.IdListaPrecio)
                  .FirstOrDefault();

                if (listaEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                listaEn.LIP_Estado = ConstantesFramework.ESTADO_INACTIVO;

                TARepositorioAudit.MapeoAuditListaPrecio(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene los tipos de moneda de la aplicación
        /// </summary>
        /// <returns>Objeto Lista Moneda</returns>
        public IEnumerable<TAMonedaDC> ObtenerTiposMoneda()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<TAMonedaDC> consulta = contexto.Moneda_TAR.OrderBy(o => o.MON_Descripcion).ToList().ConvertAll(r => new TAMonedaDC()
                {
                    IdMoneda = r.MON_IdMoneda,
                    Descripcion = r.MON_Descripcion
                });

                if (consulta == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                    return consulta;
            }
        }

        /// <summary>
        /// Retorna las unidades de negocio
        /// </summary>
        /// <returns>Objeto Unidad de negocio</returns>
        public IEnumerable<TAUnidadNegocio> ObtenerUnidadNegocio()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.UnidadNegocio_TAR.OrderBy(o => o.UNE_Nombre).ToList().ConvertAll<TAUnidadNegocio>(r => new TAUnidadNegocio()
                {
                    IdUnidadNegocio = r.UNE_IdUnidad,
                    Nombre = r.UNE_Nombre
                });
            }
        }

        /// <summary>
        /// Valida si una lista de precios puede ser de tarifa plena o no
        /// </summary>
        /// <param name="idListaPrecio">Identificador lista de precios</param>
        /// <returns>Retorna true o false</returns>
        public bool ValidaListaPrecio(int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.ListaPrecios_TAR.Where(ac => ac.LIP_EsTarifaPlena == true && (DateTime.Now >= ac.LIP_Inicio && DateTime.Now <= ac.LIP_Fin) && ac.LIP_Estado == ConstantesFramework.ESTADO_ACTIVO && ac.LIP_IdListaPrecios != idListaPrecio).Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        ///// <summary>
        ///// Adiciona un servicio a una lista de precios
        ///// </summary>
        ///// <param name="listaPrecioServicio">Objetos lista de precio servicio</param>
        public void AdicionarListaPrecioServicio(TAListaPrecioServicio listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecioServicio_TAR listaEn = new ListaPrecioServicio_TAR()
                {
                    LPS_IdServicio = listaPrecioServicio.IdServicio,
                    LPS_IdListaPrecios = listaPrecioServicio.IdListaPrecio,
                    LPS_PrimaSeguros = listaPrecioServicio.PrimaSeguro,
                    LPS_Estado = ConstantesFramework.ESTADO_ACTIVO,
                    LPS_FechaGrabacion = DateTime.Now,
                    LPS_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.ListaPrecioServicio_TAR.Add(listaEn);
                contexto.SaveChanges();

                //EMRL 118281
                if (listaPrecioServicio.IdServicio == TAConstantesServicios.SERVICIO_RAPIRADICADO)
                {                    
                    PrecioTrayecto_TAR precioTrayecto;
                    //Dejar valores y porcentajes en 0, para que el cliente los edite
                    //Se deja solo hasta 27 por q son los trayectos fijos para rapirapicado
                    foreach (TrayectoSubTrayecto_TAR trayecto in contexto.TrayectoSubTrayecto_TAR.Where(t => t.TRS_IdTrayectoSubTrayecto <= 27))
                    {
                        precioTrayecto = new PrecioTrayecto_TAR();
                        precioTrayecto.PTR_FechaGrabacion = DateTime.Now;
                        precioTrayecto.PTR_IdListaPrecioServicio = listaEn.LPS_IdListaPrecioServicio;
                        precioTrayecto.PTR_IdTrayectoSubTrayecto = trayecto.TRS_IdTrayectoSubTrayecto;
                        precioTrayecto.PTR_Porcentaje = 0;
                        precioTrayecto.PTR_ValorFijo = 0;
                        precioTrayecto.PTR_CreadoPor = ControllerContext.Current.Usuario;
                        contexto.PrecioTrayecto_TAR.Add(precioTrayecto);
                    }

                    contexto.SaveChanges();
                }

            }


        }

        /// <summary>
        /// Obtiene la lista precios para el servicio de mensajeria
        /// </summary>
        /// <param name="idListaPrecios"></param>
        /// <returns></returns>
        public TAListaPrecioServicio ObtenerListaPrecioServicioMensajeria(int idListaPrecios)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TAListaPrecioServicio lservicio = null;
                ListaPrecioServicio_TAR lista = contexto.ListaPrecioServicio_TAR.Where(r => r.LPS_IdListaPrecios == idListaPrecios && r.LPS_IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA).FirstOrDefault();

                if (lista != null)
                    lservicio = new TAListaPrecioServicio()
                    {
                        IdListaPrecio = lista.LPS_IdListaPrecios,
                        IdListaPrecioServicio = lista.LPS_IdListaPrecioServicio,
                        PrimaSeguro = lista.LPS_PrimaSeguros
                    };

                return lservicio;
            }
        }

        /// <summary>
        /// Adiciona un servicio a una lista de precios
        /// </summary>
        /// <param name="listaPrecioServicio"></param>
        public void AdicionarListaPrecioServicioBaseMensajeria(TAListaPrecioServicio listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paGuardarListaPreciosServicioUNegMensajeria_TAR(listaPrecioServicio.IdServicio
                  , listaPrecioServicio.IdListaPrecio
                  , listaPrecioServicio.PrimaSeguro
                  , ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Edita un servicio de una lista de precios
        /// </summary>
        /// <param name="listaPrecioServicio">Objetos lista de precio servicio</param>
        public void EditarListaPrecioServicio(TAListaPrecioServicio listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecioServicio_TAR listaEn = contexto.ListaPrecioServicio_TAR
                  .Where(r => r.LPS_IdListaPrecios == listaPrecioServicio.IdListaPrecio && r.LPS_IdServicio == listaPrecioServicio.IdServicio)
                  .FirstOrDefault();

                if (listaEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                listaEn.LPS_PrimaSeguros = listaPrecioServicio.PrimaSeguro;
                listaEn.LPS_Estado = listaPrecioServicio.Estado;
                TARepositorioAudit.MapeoAuditListaPrecioServicio(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un servicio de una lista de precio
        /// </summary>
        /// <param name="listaPrecioServicio">Objetos lista de precio servicio</param>
        public void EliminarListaPrecioServicio(TAListaPrecioServicio listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecioServicio_TAR listaEn = contexto.ListaPrecioServicio_TAR.Where(r => r.LPS_IdListaPrecios == listaPrecioServicio.IdListaPrecio && r.LPS_IdServicio == listaPrecioServicio.IdServicio).First();
                if (listaEn != null)
                {
                    contexto.ListaPrecioServicio_TAR.Remove(listaEn);
                    TARepositorioAudit.MapeoAuditListaPrecioServicio(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Eliminar lista servicio lista precio
        /// </summary>
        /// <param name="listaPrecioServicio"></param>
        public void EliminarListaPrecioServicioMensajeria(TAListaPrecioServicio listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecioServicio_TAR listaEn = contexto.ListaPrecioServicio_TAR.Where(r => r.LPS_IdListaPrecios == listaPrecioServicio.IdListaPrecio && r.LPS_IdServicio == listaPrecioServicio.IdServicio).First();
                if (listaEn != null)
                {
                    contexto.paEliminarServicioListaPrecios_TAR(ControllerContext.Current.Usuario
                                            , listaPrecioServicio.IdListaPrecio
                                            , listaPrecioServicio.IdServicio);
                }
            }
        }

        /// <summary>
        /// Metodo encargado de devolver el id de la lista de precios vigente
        /// </summary>
        /// <returns>int con el id de la lista de precio</returns>
        public int ObtenerIdListaPrecioVigente()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ListaPrecios_TAR> listas = contexto.ListaPrecios_TAR
                  .Where(l => DateTime.Now >= l.LIP_Inicio && DateTime.Now <= l.LIP_Fin && l.LIP_Estado == ConstantesFramework.ESTADO_ACTIVO && l.LIP_EsTarifaPlena)
                  .ToList();

                if (listas.Count == 0)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_LISTA_PRECIOS_PLENA_VIGENTE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_LISTA_PRECIOS_PLENA_VIGENTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                if (listas.Count > 1)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_EXISTE_MAS_DE_UNA_LISTA_PRECIOS_PLENA_VIGENTE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_EXISTE_MAS_DE_UNA_LISTA_PRECIOS_PLENA_VIGENTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return listas[0].LIP_IdListaPrecios;
            }
        }

        /// <summary>
        /// Metodo utilizado para conocer la lista de precios para determinado cliente credito
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        /// <returns></returns>
        public int ObtenerIdListaPrecioClienteCredito(int IdClienteCredito)
        {
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            int idlistaPrecios;
            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                List<TAServicioDC> servicios = new List<TAServicioDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerIdListaPreciosClienteCredito_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idCliente", IdClienteCredito));
                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);

                if (dsRes != null)
                {
                    DataRow dr1 = dsRes.Tables[0].Rows[0];
                    return idlistaPrecios = Convert.ToInt32(dr1["CON_ListaPrecios"]);
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO));
                    throw new FaultException<ControllerException>(excepcion);
                }

            }

        }

        #endregion Lista de Precios

        #region Impuestos

        /// <summary>
        /// Obtiene los impuestos de los servicios de la aplicación
        /// </summary>
        /// <returns></returns>
        public TAServicioImpuestosDC ObtenerServiciosImpuestos(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ServicioImpuestos_VTAR.Where(si => si.SEI_IdServicio == idServicio).ToList().
                   GroupBy(i => i.SEI_IdServicio, (idser, impuestos) =>
                     new TAServicioImpuestosDC()
                     {
                         Servicio = new TAServicioDC()
                         {
                             IdServicio = idser,
                             Nombre = impuestos.FirstOrDefault().SER_Nombre,
                             Descripcion = impuestos.FirstOrDefault().SER_Descripcion
                         },
                         ImpuestosAsignados = impuestos.ToList().ConvertAll<TAImpuestoDelServicio>
                         (imp => new TAImpuestoDelServicio()
                         {
                             DescripcionImpuesto = imp.IMP_Descripcion,
                             IdImpuesto = imp.SEI_IdImpuesto,
                             IdServicio = idser,
                             ValorImpuesto = imp.IMP_Valor
                         })
                     }).FirstOrDefault();
            }
        }

        #endregion Impuestos

        #region Servicios

        /// <summary>
        /// Obtiene los datos básicos del servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Objeto Servicio</returns>
        public TAServicioDC ObtenerDatosServicio(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TAServicioDC servicio = new TAServicioDC();

                Servicio_TAR servicioEn = contexto.Servicio_TAR.Include("UnidadNegocio_TAR")
                  .Where(r => r.SER_IdServicio == idServicio)
                  .FirstOrDefault();

                servicio.Nombre = servicioEn.SER_Nombre;
                servicio.Descripcion = servicioEn.SER_Descripcion;
                servicio.UnidadNegocio = servicioEn.UnidadNegocio_TAR.UNE_Nombre;
                servicio.IdConceptoCaja = servicioEn.SER_IdConceptoCaja;

                return servicio;
            }
        }

        /// <summary>
        /// Metodo para obtener los servicios de una lista de precios
        /// </summary>
        /// <param name="listaPrecios"></param>
        /// <returns> lista de datos tipos servicio</returns>
        public IEnumerable<TAServicioDC> ObtenerServiciosporLista(int listaPrecios)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ListaPrecioServicio_TAR
                  .Where(s => s.LPS_IdListaPrecios == listaPrecios)
                  .ToList()
                  .ConvertAll(r => new TAServicioDC
                  {
                      IdServicio = r.LPS_IdServicio,
                      Nombre = contexto.Servicio_TAR.Where(s => s.SER_IdServicio == r.LPS_IdServicio).FirstOrDefault().SER_Nombre,
                  });
            }
        }

        #endregion Servicios

        #region Servicio Centros de Correspondencia

        /// <summary>
        /// Obtiene los centros de correspondencia de una lista de precio servicio
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <returns>Colección centros de correspondencia</returns>
        public IEnumerable<TAServicioCentroDeCorrespondenciaDC> ObtenerCentrosDeCorrespondencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);

                filtro.Add("POP_IdListaPrecioServicio", idListaPrecioServicio);
                return contexto.ConsultarEqualsServicioCentroCorrespon_VTAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TAServicioCentroDeCorrespondenciaDC()
                  {
                      IdServCentroCorrespondencia = r.SCC_IdServCentroCorrespondencia,
                      Concepto = r.SCC_Concepto,
                      Descripcion = r.SCC_Descripcion,
                      Valor = r.POP_Valor,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona precio de centro de correspondencia
        /// </summary>
        /// <param name="centroDeCorrespondencia">Objeto centro de correspondencia</param>
        public void AdicionarPrecioCentroDeCorrespondencia(TAServicioCentroDeCorrespondenciaDC centroDeCorrespondencia, int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);

                ServicioCentroCorrespondencia_TAR centroEn = new ServicioCentroCorrespondencia_TAR()
                {
                    SCC_IdServicio = idServicio,
                    SCC_Concepto = centroDeCorrespondencia.Concepto,
                    SCC_Descripcion = centroDeCorrespondencia.Descripcion,
                    SCC_FechaGrabacion = DateTime.Now,
                    SCC_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ServicioCentroCorrespondencia_TAR.Add(centroEn);

                PrecioCentroCorrespondencia_TAR precio = new PrecioCentroCorrespondencia_TAR()
                {
                    POP_IdPrecioServiCentroCorresp = centroEn.SCC_IdServCentroCorrespondencia,
                    POP_IdListaPrecioServicio = int.Parse(idListaPrecioServicio),
                    POP_Valor = centroDeCorrespondencia.Valor,
                    POP_FechaGrabacion = DateTime.Now,
                    POP_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.PrecioCentroCorrespondencia_TAR.Add(precio);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita precio de centro de correspondencia
        /// </summary>
        /// <param name="centroDeCorrespondencia">Objeto centro de correspondencia</param>
        public void EditarPrecioCentroDeCorrespondencia(TAServicioCentroDeCorrespondenciaDC centroDeCorrespondencia)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioCentroCorrespondencia_TAR centroEn = contexto.ServicioCentroCorrespondencia_TAR
                  .Where(r => r.SCC_IdServCentroCorrespondencia == centroDeCorrespondencia.IdServCentroCorrespondencia)
                  .FirstOrDefault();

                centroEn.SCC_Concepto = centroDeCorrespondencia.Concepto;
                centroEn.SCC_Descripcion = centroDeCorrespondencia.Descripcion;

                PrecioCentroCorrespondencia_TAR precio = contexto.PrecioCentroCorrespondencia_TAR
                  .Where(p => p.POP_IdPrecioServiCentroCorresp == centroDeCorrespondencia.IdServCentroCorrespondencia)
                  .FirstOrDefault();

                precio.POP_Valor = centroDeCorrespondencia.Valor;
                TARepositorioAudit.MapearPrecioCentroCorrespondencia(contexto);
                TARepositorioAudit.MapearCentrosCorrespondencia(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina precio centro de correspondencia
        /// </summary>
        /// <param name="centroDeCorrespondencia">Objeto centro de correspondencia</param>
        public void EliminarPrecioCentroDeCorrespondencia(TAServicioCentroDeCorrespondenciaDC centroDeCorrespondencia)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioCentroCorrespondencia_TAR precio = contexto.PrecioCentroCorrespondencia_TAR
                 .Where(p => p.POP_IdPrecioServiCentroCorresp == centroDeCorrespondencia.IdServCentroCorrespondencia)
                 .FirstOrDefault();

                contexto.PrecioCentroCorrespondencia_TAR.Remove(precio);

                ServicioCentroCorrespondencia_TAR centroEn = contexto.ServicioCentroCorrespondencia_TAR
                  .Where(r => r.SCC_IdServCentroCorrespondencia == centroDeCorrespondencia.IdServCentroCorrespondencia)
                  .FirstOrDefault();

                contexto.ServicioCentroCorrespondencia_TAR.Remove(centroEn);
                TARepositorioAudit.MapearPrecioCentroCorrespondencia(contexto);
                TARepositorioAudit.MapearCentrosCorrespondencia(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Servicio Centros de Correspondencia

        #region Servicio Rapi Promocional

        /// <summary>
        /// Obtiene rapi promocional
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <returns>Colección rapi promocional</returns>
        public IEnumerable<TAServicioRapiPromocionalDC> ObtenerRapiPromocional(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);

                filtro.Add("PRA_IdListaPrecioServicio", idListaPrecioServicio);
                return contexto.ConsultarEqualsPrecioRango_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TAServicioRapiPromocionalDC()
                  {
                      IdCantidadRango = r.PRA_IdPrecioRango,
                      CantidadInicial = r.PRA_Inicial,
                      CantidadFinal = r.PRA_Final,
                      Valor = r.PRA_Valor,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona Precio Rango
        /// </summary>
        /// <param name="precioRango">Objeto Precio Rango</param>
        /// <param name="idServicio">Identidicador Servicio</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        public void AdicionarRapiPromocional(TAServicioRapiPromocionalDC rapiPromocional, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioRango_TAR cantidadRangoEn = new PrecioRango_TAR()
                {
                    PRA_IdListaPrecioServicio = idListaPrecioServicio,
                    PRA_Inicial = rapiPromocional.CantidadInicial,
                    PRA_Final = rapiPromocional.CantidadFinal,
                    PRA_Valor = rapiPromocional.Valor,
                    PRA_Porcentaje = rapiPromocional.Porcentaje,
                    PRA_FechaGrabacion = DateTime.Now,
                    PRA_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.PrecioRango_TAR.Add(cantidadRangoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita precio rango
        /// </summary>
        /// <param name="precioRango">Objeto Precio Rango</param>
        /// <param name="idServicio">Identidicador Servicio</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        public void EditarRapiPromocional(TAServicioRapiPromocionalDC rapiPromocional)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioRango_TAR cantidadRangoEn = contexto.PrecioRango_TAR
                  .Where(r => r.PRA_IdPrecioRango == rapiPromocional.IdCantidadRango)
                  .FirstOrDefault();

                if (cantidadRangoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                cantidadRangoEn.PRA_Inicial = rapiPromocional.CantidadInicial;
                cantidadRangoEn.PRA_Final = rapiPromocional.CantidadFinal;
                cantidadRangoEn.PRA_Valor = rapiPromocional.Valor;
                cantidadRangoEn.PRA_Porcentaje = rapiPromocional.Porcentaje;
                TARepositorioAudit.MapearPrecioRango(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina precio rango
        /// </summary>
        /// <param name="precioRango">Objeto Precio Rango</param>
        public void EliminarRapiPromocional(TAServicioRapiPromocionalDC rapiPromocional)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioRango_TAR cantidadRangoEn = contexto.PrecioRango_TAR
                  .Where(r => r.PRA_IdPrecioRango == rapiPromocional.IdCantidadRango)
                  .FirstOrDefault();

                contexto.PrecioRango_TAR.Remove(cantidadRangoEn);
                TARepositorioAudit.MapearPrecioRango(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Servicio Rapi Promocional

        #region Servicio Trámites

        /// <summary>
        /// Obtiene los trámites
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <returns>Colección trámites</returns>
        public IEnumerable<TAServicioTramiteDC> ObtenerTramites(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);

                filtro.Add("PRT_IdListaPrecioServicio", idListaPrecioServicio);
                return contexto.ConsultarEqualsServicioTramites_VTAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TAServicioTramiteDC()
                  {
                      IdTramite = r.TRA_IdTramite,
                      TipoTramite = new TATipoTramite()
                      {
                          IdTipoTramite = r.TRA_IdTipoTramite,
                          Descripcion = contexto.TipoTramite_TAR.Where(t => t.TIT_IdTipoTramite == r.TRA_IdTipoTramite).FirstOrDefault().TIT_Descripcion
                      },
                      Nombre = r.TRA_Nombre,
                      Descripcion = r.TRA_Descripcion,
                      Valor = r.PRT_Valor,
                      ValorAdicional = r.PRT_ValorAdicionalLocal,
                      ValorAdicionalDocumento = r.PRT_ValorAdicionalPorDocumento,
                      Duracion = r.TRA_Duracion,
                      Requisitos = new ObservableCollection<TARequisitoTramiteDC>(contexto.RequisitoTramite_TAR.Where(rt => rt.REQ_IdTramite == r.TRA_IdTramite)
                       .ToList()
                       .ConvertAll(rtd => new TARequisitoTramiteDC()
                       {
                           IdRequisitoTramite = rtd.REQ_IdRequisitoServicio,
                           IdTramite = rtd.REQ_IdTramite,
                           Descripcion = rtd.REQ_Descripcion,
                           EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                       })),

                      ImpuestosAsignados = (contexto.TramiteImpuesto_TAR.Include("Impuesto_TAR").Where(i => i.TRI_IdTramite == r.TRA_IdTramite)
                      .ToList()
                      .ConvertAll(il => new TAImpuestosDC
                      {
                          Identificador = il.TRI_IdImpuesto,
                          Descripcion = il.Impuesto_TAR.IMP_Descripcion,
                          Asignado = true,
                          Actual = true,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                      })),

                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona un trámite
        /// </summary>
        /// <param name="tramite">Objeto Trámite</param>
        public void AdicionarTramite(TAServicioTramiteDC tramite, int idServicio, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Tramite_TAR tramiteEn = new Tramite_TAR()
                {
                    TRA_IdServicio = idServicio,
                    TRA_IdTipoTramite = tramite.TipoTramite.IdTipoTramite,
                    TRA_Nombre = tramite.Nombre,
                    TRA_Descripcion = tramite.Descripcion,
                    TRA_Duracion = tramite.Duracion,
                    TRA_FechaGrabacion = DateTime.Now,
                    TRA_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.Tramite_TAR.Add(tramiteEn);

                PrecioTramite_TAR precioTramite = new PrecioTramite_TAR()
                {
                    PRT_IdTramite = tramiteEn.TRA_IdTramite,
                    PRT_IdListaPrecioServicio = idListaPrecioServicio,
                    PRT_Valor = tramite.Valor,
                    PRT_ValorAdicionalLocal = tramite.ValorAdicional,
                    PRT_ValorAdicionalPorDocumento = tramite.ValorAdicionalDocumento,
                    PRT_FechaGrabacion = DateTime.Now,
                    PRT_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.PrecioTramite_TAR.Add(precioTramite);

                ModificarRequisitoTramite(tramite, contexto, tramiteEn);

                tramite.ImpuestosAsignados.ToList().ForEach(imp =>
                {
                    if (imp.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                        AdicionarImpuestoTramite(imp, tramiteEn.TRA_IdTramite, contexto);
                    else if (imp.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                        EliminarImpuestoTramite(imp, tramiteEn.TRA_IdTramite, contexto);
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona un trámite impuesto
        /// </summary>
        /// <param name="impuesto">Objeto impuesto</param>
        /// <param name="idTramite">Identificador trámite</param>
        private void AdicionarImpuestoTramite(TAImpuestosDC impuesto, int idTramite, EntidadesTarifas contexto)
        {
            TramiteImpuesto_TAR tramiteImpuesto = new TramiteImpuesto_TAR()
            {
                TRI_IdTramite = idTramite,
                TRI_IdImpuesto = impuesto.Identificador,
                TRI_FechaGrabacion = DateTime.Now,
                TRI_CreadoPor = ControllerContext.Current.Usuario
            };

            contexto.TramiteImpuesto_TAR.Add(tramiteImpuesto);
        }

        /// <summary>
        /// Elimina trámite impuesto
        /// </summary>
        /// <param name="impuesto">Objeto impuesto</param>
        /// <param name="idTramite">Identificador trámite</param>
        /// <param name="contexto">Contexto</param>
        private void EliminarImpuestoTramite(TAImpuestosDC impuesto, int idTramite, EntidadesTarifas contexto)
        {
            TramiteImpuesto_TAR tramiteImpuesto = contexto.TramiteImpuesto_TAR
              .Where(i => i.TRI_IdTramite == idTramite && i.TRI_IdImpuesto == impuesto.Identificador).FirstOrDefault();

            contexto.TramiteImpuesto_TAR.Remove(tramiteImpuesto);
        }

        /// <summary>
        /// Edita un trámite
        /// </summary>
        /// <param name="tramite">Objeto Trámite</param>
        public void EditarTramite(TAServicioTramiteDC tramite, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Tramite_TAR tramiteEn = contexto.Tramite_TAR.Where(r => r.TRA_IdTramite == tramite.IdTramite).FirstOrDefault();

                tramiteEn.TRA_IdTipoTramite = tramite.TipoTramite.IdTipoTramite;
                tramiteEn.TRA_Nombre = tramite.Nombre;
                tramiteEn.TRA_Descripcion = tramite.Descripcion;
                tramiteEn.TRA_Duracion = tramite.Duracion;

                PrecioTramite_TAR precioTramite = contexto.PrecioTramite_TAR.Where(p => p.PRT_IdTramite == tramite.IdTramite && p.PRT_IdListaPrecioServicio == idListaPrecioServicio).FirstOrDefault();
                precioTramite.PRT_Valor = tramite.Valor;
                precioTramite.PRT_ValorAdicionalLocal = tramite.ValorAdicional;
                precioTramite.PRT_ValorAdicionalPorDocumento = tramite.ValorAdicionalDocumento;

                ModificarRequisitoTramite(tramite, contexto, tramiteEn);

                tramite.ImpuestosAsignados.ToList().ForEach(imp =>
                {
                    if (imp.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                        AdicionarImpuestoTramite(imp, tramiteEn.TRA_IdTramite, contexto);
                    else if (imp.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                        EliminarImpuestoTramite(imp, tramiteEn.TRA_IdTramite, contexto);
                });

                TARepositorioAudit.MapearPrecioTramite(contexto);
                TARepositorioAudit.MapearTramite(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Modifica un trámite
        /// </summary>
        /// <param name="tramite">Objeto trámite</param>
        /// <param name="contexto">Contexto</param>
        /// <param name="tramiteEn">Entidad</param>
        private static void ModificarRequisitoTramite(TAServicioTramiteDC tramite, EntidadesTarifas contexto, Tramite_TAR tramiteEn)
        {
            tramite.Requisitos.ToList().ForEach(f =>
            {
                if (f.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                {
                    RequisitoTramite_TAR requisito = new RequisitoTramite_TAR()
                    {
                        REQ_Descripcion = f.Descripcion,
                        REQ_IdTramite = tramiteEn.TRA_IdTramite,
                        REQ_FechaGrabacion = DateTime.Now,
                        REQ_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.RequisitoTramite_TAR.Add(requisito);
                }
                else if (f.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                {
                    RequisitoTramite_TAR requisito = contexto.RequisitoTramite_TAR
                      .Where(rt => rt.REQ_IdRequisitoServicio == f.IdRequisitoTramite)
                      .FirstOrDefault();

                    requisito.REQ_Descripcion = f.Descripcion;
                }
                else if (f.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                {
                    RequisitoTramite_TAR requisito = contexto.RequisitoTramite_TAR
                      .Where(rt => rt.REQ_IdRequisitoServicio == f.IdRequisitoTramite)
                      .FirstOrDefault();

                    contexto.RequisitoTramite_TAR.Remove(requisito);
                }
            });

            TARepositorioAudit.MapearRequistitoTramite(contexto);
        }

        /// <summary>
        /// Elimina un trámite
        /// </summary>
        /// <param name="tramite">Objeto Trámite</param>
        public void EliminarTramite(TAServicioTramiteDC tramite, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioTramite_TAR precioTramite = contexto.PrecioTramite_TAR
                  .Where(p => p.PRT_IdTramite == tramite.IdTramite && p.PRT_IdListaPrecioServicio == idListaPrecioServicio)
                  .FirstOrDefault();

                contexto.PrecioTramite_TAR.Remove(precioTramite);

                Tramite_TAR tramiteEn = contexto.Tramite_TAR.Where(r => r.TRA_IdTramite == tramite.IdTramite).FirstOrDefault();
                contexto.Tramite_TAR.Remove(tramiteEn);

                tramite.Requisitos.ToList().ForEach(re =>
                {
                    RequisitoTramite_TAR requisito = contexto.RequisitoTramite_TAR
                      .Where(rt => rt.REQ_IdRequisitoServicio == re.IdRequisitoTramite)
                        .FirstOrDefault();

                    if (requisito != null)
                        contexto.RequisitoTramite_TAR.Remove(requisito);
                });

                tramite.ImpuestosAsignados.ToList().ForEach(i =>
                {
                    TramiteImpuesto_TAR impuesto = contexto.TramiteImpuesto_TAR
                      .Where(it => it.TRI_IdTramite == tramite.IdTramite && it.TRI_IdImpuesto == i.Identificador)
                      .FirstOrDefault();

                    if (impuesto != null)
                        contexto.TramiteImpuesto_TAR.Remove(impuesto);
                });

                TARepositorioAudit.MapearPrecioTramite(contexto);
                TARepositorioAudit.MapearRequistitoTramite(contexto);
                TARepositorioAudit.MapearTramite(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Servicio Trámites

        #region Requisito Servicios

        /// <summary>
        /// Retorna los requisitos para un servicio solicitado
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns></returns>
        public IList<TARequisitoServicioDC> ObtenerRequisitosServicio(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.RequisitoServicio_TAR.Where(r => r.RES_IdServicio == idServicio)
                  .ToList()
                  .ConvertAll(r => new TARequisitoServicioDC
                  {
                      Descripcion = r.RES_Descripcion,
                      Identificador = r.RES_IdRequisitoServicio,
                      Servicio = new TAServicioDC() { IdServicio = r.RES_IdServicio }
                  });
            }
        }

        /// <summary>
        /// Obtener los datos basicos de los requisitos del servicio
        /// </summary>
        /// <param name="idIdentficadorInterno"></param>
        /// <returns></returns>
        public IList<TARequisitoServicioDC> ObtenerDatosRequisitoServicios(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsRequisitoServicio_TAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).
                  ToList().ConvertAll(p => new TARequisitoServicioDC()
                  {
                      Descripcion = p.RES_Descripcion,
                      Identificador = p.RES_IdRequisitoServicio,
                      Servicio = new TAServicioDC()
                      {
                          IdServicio = p.RES_IdServicio,
                          Nombre = contexto.Servicio_TAR.Where(se => se.SER_IdServicio == p.RES_IdServicio).FirstOrDefault().SER_Descripcion
                      }
                  });
            }
        }

        /// <summary>
        /// Adiciona un nuevo requisito a un servicio
        /// </summary>
        /// <param name="requisito"></param>
        public void AdicionarRequisitoServicio(TARequisitoServicioDC requisito)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RequisitoServicio_TAR requisitoEn = new RequisitoServicio_TAR()
                {
                    RES_Descripcion = requisito.Descripcion,
                    RES_FechaGrabacion = DateTime.Now,
                    RES_CreadoPor = ControllerContext.Current.Usuario,
                    RES_IdServicio = requisito.Servicio.IdServicio
                };
                contexto.RequisitoServicio_TAR.Add(requisitoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un Requisito a un servicio
        /// </summary>
        /// <param name="tipoImpuesto"></param>
        public void EliminarRequisitoServicio(TARequisitoServicioDC requisito)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RequisitoServicio_TAR requisitoEn = contexto.RequisitoServicio_TAR.Where(r => r.RES_IdRequisitoServicio == requisito.Identificador).FirstOrDefault();
                contexto.RequisitoServicio_TAR.Remove(requisitoEn);
                TARepositorioAudit.MapearRequisitoServicio(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un Requisito de un servicio
        /// </summary>
        /// <param name="tipoImpuesto"></param>
        public void EditarRequisitoServicio(TARequisitoServicioDC requisito)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                RequisitoServicio_TAR requisitoEn = contexto.RequisitoServicio_TAR
                  .Where(r => r.RES_IdRequisitoServicio == requisito.Identificador)
                  .FirstOrDefault();

                if (requisitoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                requisitoEn.RES_Descripcion = requisito.Descripcion;
                requisitoEn.RES_IdServicio = requisito.Servicio.IdServicio;
                TARepositorioAudit.MapearRequisitoServicio(contexto);

                contexto.SaveChanges();
            }
        }

        #endregion Requisito Servicios

        #region Valor Peso Declarado

        /// <summary>
        /// Obtiene el valor peso declarado
        /// </summary>
        /// <returns>Colección con los valores peso declarados</returns>
        public IEnumerable<TAValorPesoDeclaradoDC> ObtenerValorPesoDeclarado(int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ValorPesoDeclarado_TAR
                  .OrderBy(o => o.VMD_PesoInicial)
                  .Where(r => r.VMD_IdListaPrecios == idListaPrecio)
                  .ToList()
                  .ConvertAll(v => new TAValorPesoDeclaradoDC()
                  {
                      IdValorPesoDeclarado = v.VMD_IdValorMinimoDeclarado,
                      IdListaPrecio = v.VMD_IdListaPrecios,
                      PesoInicial = v.VMD_PesoInicial,
                      PesoFinal = v.VMD_PesoFinal,
                      ValorMinimoDeclarado = v.VMD_ValorMinimoDeclarado,
                      ValorMaximoDeclarado = v.VMD_ValorMaximoDeclarado,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });
            }
        }

        /// <summary>
        /// Adiciona un valor peso declarado
        /// </summary>
        /// <param name="valorPeso">Objeto</param>
        public void AdicionarValorPesoDeclarado(TAValorPesoDeclaradoDC valorPeso)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ValorPesoDeclarado_TAR val = new ValorPesoDeclarado_TAR()
                {
                    VMD_IdListaPrecios = valorPeso.IdListaPrecio,
                    VMD_PesoInicial = valorPeso.PesoInicial,
                    VMD_PesoFinal = valorPeso.PesoFinal,
                    VMD_ValorMinimoDeclarado = valorPeso.ValorMinimoDeclarado,
                    VMD_ValorMaximoDeclarado = valorPeso.ValorMaximoDeclarado,
                    VMD_FechaGrabacion = DateTime.Now,
                    VMD_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ValorPesoDeclarado_TAR.Add(val);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un valor peso declarado
        /// </summary>
        /// <param name="valorPeso">Objeto</param>
        public void EditarValorPesoDeclarado(TAValorPesoDeclaradoDC valorPeso)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ValorPesoDeclarado_TAR val = contexto.ValorPesoDeclarado_TAR
                  .Where(r => r.VMD_IdValorMinimoDeclarado == valorPeso.IdValorPesoDeclarado)
                  .FirstOrDefault();

                val.VMD_PesoInicial = valorPeso.PesoInicial;
                val.VMD_PesoFinal = valorPeso.PesoFinal;
                val.VMD_ValorMinimoDeclarado = valorPeso.ValorMinimoDeclarado;
                val.VMD_ValorMaximoDeclarado = valorPeso.ValorMaximoDeclarado;
                TARepositorioAudit.MapearAuditValorPesoDeclarado(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un valor peso declarado
        /// </summary>
        /// <param name="valorPeso">Objeto</param>
        public void EliminarValorPesoDeclarado(TAValorPesoDeclaradoDC valorPeso)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ValorPesoDeclarado_TAR val = contexto.ValorPesoDeclarado_TAR
                  .Where(r => r.VMD_IdValorMinimoDeclarado == valorPeso.IdValorPesoDeclarado)
                  .FirstOrDefault();

                contexto.ValorPesoDeclarado_TAR.Remove(val);
                TARepositorioAudit.MapearAuditValorPesoDeclarado(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion Valor Peso Declarado

        #region Validar Servicio Trayecto Destino

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio de Origen</param>
        /// <param name="municipioDestino">Municipio de Destino</param>
        /// <param name="servicio">Servicio</param>
        /// <returns>Duración en días</returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio)
        {
            TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    paValidarServicioTrayectoDestinoRS_TAR validacionTrayecto = contexto.paValidarServicioTrayectoDestino_TAR(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad, servicio.IdServicio).FirstOrDefault();

            //    if (validacionTrayecto == null)
            //    {
            //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE)));
            //    }

            //    return validacionTrayecto.STR_TiempoEntrega;
            //}
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = new SqlCommand("paValidarServicioTrayectoDestino_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdServicio", servicio.IdServicio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tiempos.numeroDiasArchivo = Convert.ToDouble(reader["STR_TiempoArchivo"]);
                    tiempos.numeroDiasDigitalizacion = Convert.ToDouble(reader["STR_TiempoDigitalizacion"]);
                    tiempos.numeroDiasEntrega = Convert.ToInt32(reader["STR_TiempoEntrega"]);
                }
                conn.Close();
            }
            return tiempos;
        }

        /// <summary>
        /// Retorna los tiempos para la digitalizacion y archivo de una guia despues de entregada
        /// </summary>
        /// <param name="idCiudadOrigen"></param>
        /// <param name="idCiudadDestino"></param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ObtenerTiempoDigitalizacionArchivo(string idCiudadOrigen, string idCiudadDestino)
        {
            string cadenaTiempos = System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTiempoDigitalizacionArchivo"].ConnectionString;
            TATiempoDigitalizacionArchivo tiempos = new TATiempoDigitalizacionArchivo();
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiempoEstimadoDigitalizacionYArchivo_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCiudadOrigen", idCiudadOrigen);
                cmd.Parameters.AddWithValue("@IdCiudadDestino", idCiudadDestino);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tiempos.numeroDiasDigitalizacion = Convert.ToDouble(reader["TIEMPO_ESTIMADO_DE_DIGITALIZACION"].ToString());
                    tiempos.numeroDiasArchivo = Convert.ToDouble(reader["TIEMPO_ESTIMADO_DE_ARCHIVO"].ToString());
                }
                conn.Close();
            }
            return tiempos;
        }

        /// <summary>
        /// Retorna Validacion si el Servicio-Origen-Destino, debe etiquetarse como AEREO en el campo del casillero de la Guia
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool ValidarServicioTrayectoCasilleroAereo(string municipioOrigen, string municipioDestino, int idServicio)
        {
            bool rta = false;

            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paValidarServicioTrayectoCasilleroAereo_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@municipioOrigen", municipioOrigen));
                cmd.Parameters.Add(new SqlParameter("@municipioDestino", municipioDestino));
                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));

                Int32 num = (Int32)cmd.ExecuteScalar();
                if (num == 1)
                    rta = true;
            }

            return rta;
        }


        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                List<TAHorarioRecogidaCsvDC> horario = new List<TAHorarioRecogidaCsvDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerHorarioRecogidasCSV_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idcentroServicios", idCentroServicio));
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    horario.Add(new TAHorarioRecogidaCsvDC
                    {
                        DiaDeLaSemana = int.Parse(lector["HRC_Dia"].ToString()),
                        HoraRecogida = Convert.ToDateTime(lector["HRC_Hora"].ToString())
                    });
                }
                return horario;
            }

        }

        /// <summary>
        /// Retorna la lista de horario de determinada sucursal para cliente credito
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeSucursal(int idSucursal)
        {
            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                List<TAHorarioRecogidaCsvDC> horario = new List<TAHorarioRecogidaCsvDC>();
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paObtenerHorarioRecogidasSucursal_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idSucursal", idSucursal));
                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    horario.Add(new TAHorarioRecogidaCsvDC
                    {
                        DiaDeLaSemana = int.Parse(lector["HRS_Dia"].ToString()),
                        HoraRecogida = Convert.ToDateTime(lector["HRS_Hora"].ToString())
                    });
                }
                return horario;
            }
        }

        /// <summary>
        /// Valida si existe un trayecto para una ciudad de origen y un servicio
        /// </summary>
        public void ValidarServicioTrayectoOrigen(string idLocalidadOrigen, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paValidarServicioTrayectoOrigenRS_TAR validacionTrayecto = contexto.paValidarServicioTrayectoOrigen_TAR(idLocalidadOrigen, idServicio).FirstOrDefault();

                if (validacionTrayecto == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE)));
                }
            }
        }

        /// <summary>
        /// Valida trayecto para la sucursal dada y calcula duración en días del trayecto y el valor de la prima de seguro para clientes crédito
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen del trayecto</param>
        /// <param name="municipioDestino">Municipio de origen del trayecto</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="idListaPrecios">Identificador de la lista de precios</param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idListaPrecios, int idServicio)
        {
            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    ADValidacionServicioTrayectoDestino servicioTrayecto = new ADValidacionServicioTrayectoDestino();

            //    var consultaExcepcion = contexto.paValidarExTrayectoCliente_TAR(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad, idListaPrecios, idServicio)
            //      .Where(r => r.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
            //        .ToList();

            //    if (consultaExcepcion.Count() > 0)
            //    {
            //        servicioTrayecto.ValoresAdicionales = ObtenerValorValoresAdicionalesServicio(idServicio);
            //    }
            //    else if (consultaExcepcion.Count() == 0)
            //    {
            //        var consultaTrayecto = contexto.paValidarTrayectoCliente_TAR(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad, idListaPrecios, idServicio).ToList();

            //        if (consultaTrayecto.Count() > 0)
            //        {
            //            servicioTrayecto.DuracionTrayectoEnHoras = consultaTrayecto.FirstOrDefault().STR_TiempoEntrega;
            //            servicioTrayecto.ValoresAdicionales = ObtenerValorValoresAdicionalesServicio(idServicio);
            //        }
            //    }
            //    else
            //    {
            //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE_CLIENTE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE_CLIENTE)));
            //    }

            //    return servicioTrayecto;
            //}

            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                ADValidacionServicioTrayectoDestino servicioTrayecto = new ADValidacionServicioTrayectoDestino();
                SqlCommand cmd = new SqlCommand("paValidarExTrayectoCliente_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecios);
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                da.Fill(dt);
                conn.Close();

                var consultaExcepcion = dt.AsEnumerable().Where(r => r.Field<string>("TRS_IdTipoSubTrayecto") != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).ToList();

                servicioTrayecto.ValoresAdicionales = new List<TAValorAdicional>();

                if (consultaExcepcion.Count() > 0)
                {
                    servicioTrayecto.ValoresAdicionales = ObtenerValorValoresAdicionalesServicio(idServicio);
                }
                else if (consultaExcepcion.Count() == 0)
                {
                    //var consultaTrayecto = contexto.paValidarTrayectoCliente_TAR(municipioOrigen.IdLocalidad, municipioDestino.IdLocalidad, idListaPrecios, idServicio).ToList();

                    SqlCommand cmd2 = new SqlCommand("paValidarTrayectoCliente_TAR", conn);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen.IdLocalidad);
                    cmd2.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino.IdLocalidad);
                    cmd2.Parameters.AddWithValue("@IdListaPrecio", idListaPrecios);
                    cmd2.Parameters.AddWithValue("@IdServicio", idServicio);
                    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                    DataTable dt2 = new DataTable();

                    conn.Open();
                    da2.Fill(dt2);
                    conn.Close();

                    var consultaTrayecto = dt2.AsEnumerable().ToList().FirstOrDefault();

                    if (consultaTrayecto != null)
                    {
                        servicioTrayecto.DuracionTrayectoEnHoras = consultaTrayecto.Field<Int32>("STR_TiempoEntrega");
                        servicioTrayecto.ValoresAdicionales = ObtenerValorValoresAdicionalesServicio(idServicio);
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE_CLIENTE.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_EXISTE_CLIENTE)));
                }

                return servicioTrayecto;
            }

        }

        #endregion Validar Servicio Trayecto Destino

        #region ValidarServicioRapiRadicado

        /// <summary>
        /// Valida si rapi radicado es un valor adicional de mensajería
        /// </summary>
        /// <returns>Booleano</returns>
        public bool ValidarRapiRadicado()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<TipoValorAdicional_TAR> valorAdicional = contexto.TipoValorAdicional_TAR
                  .Where(r => r.TVA_IdTipoValorAdicional == TAConstantesTarifas.ID_TIPO_VALOR_ADICIONAL_SERVICIO_RAPIRADICADO && r.TVA_IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA)
                  .ToList();

                if (valorAdicional.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        #endregion ValidarServicioRapiRadicado

        #region UserControl Forma de Pago

        /// <summary>
        /// Obtiene formas de pago segun servicios
        /// </summary>
        /// <param name="lstServicios"></param>
        /// <returns></returns>
        public TAFormaPagoServicio ObtenerListaFormasPagoPorServicio(int servicio)
        {
            TAFormaPagoServicio formaPagoServicio = new TAFormaPagoServicio();
            var forma = ObtenerFormasPagoAsignadaAServicio(servicio);
            if (forma != null)
                formaPagoServicio.FormaPago = forma.FormaPago;
            return formaPagoServicio;
        }

        /// <summary>
        /// Obtiene formas de pago asignadas y sin asignar para un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador del servicio</param>
        /// <returns>Objeto con las formas de pago asignadas y dispobibles</returns>
        public TAFormaPagoServicio ObtenerFormaPago(int IdServicio)
        {
            TAFormaPagoServicio formaPago = new TAFormaPagoServicio
            {
                IdServicio = IdServicio,
                FormaPago = new List<TAFormaPago>(),
            };

            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<TAFormaPago> fpAsignadas = contexto.ServicioFormaPago_TAR
                  .Where(r => r.SFP_IdServicio == IdServicio)
                  .ToList()
                  .ConvertAll(fa => new TAFormaPago
                  {
                      IdFormaPago = fa.SFP_IdFormaPago,
                  });

                List<TAFormaPago> fpDisponibles = contexto.FormasPago_TAR
                  .Where(fPag => !fPag.FOP_AplicaFacturacion.Value)
                  .ToList()
                  .ConvertAll(fa => new TAFormaPago
                  {
                      IdFormaPago = fa.FOP_IdFormaPago,
                      Descripcion = fa.FOP_Descripcion,
                      Actual = true,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });

                fpDisponibles.ForEach(fd =>
                {
                    fd.Asignada = fpAsignadas.Exists(fa => fa.IdFormaPago == fd.IdFormaPago);
                });
                formaPago.FormaPago = fpDisponibles;
                return formaPago;
            }
        }

        /// <summary>
        /// Obtiene formas de pago asignadas a un servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public TAFormaPagoServicio ObtenerFormasPagoAsignadaAServicio(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return new TAFormaPagoServicio()
                {
                    FormaPago = contexto.FormasPagoServicio_VTAR
                                  .Where(formaPago => formaPago.SFP_IdServicio == idServicio)
                                    .Select(r => new TAFormaPago() { IdFormaPago = r.SFP_IdFormaPago, Descripcion = r.FOP_Descripcion, AceptaMixto = r.FOP_AceptaMixto })
                                    .ToList(),
                    IdServicio = idServicio
                };
            }
        }

        /// <summary>
        /// Obtiene todas las formas de pago con los servicios que las tienen incluidas
        /// </summary>
        /// <returns></returns>
        public List<TAFormaPago> ObtenerFormasPagoConServicios()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.FormasPagoServicio_VTAR
                  .GroupBy(fp => fp.SFP_IdFormaPago)
                  .ToList()
                  .ConvertAll(fp => new TAFormaPago()
                  {
                      IdFormaPago = fp.First().SFP_IdFormaPago,
                      Descripcion = fp.First().FOP_Descripcion,
                      AceptaMixto = fp.First().FOP_AceptaMixto,
                      ServiciosAsociados = fp.ToList().ConvertAll(ser => new TAServicioDC { IdServicio = ser.SFP_IdServicio, Nombre = ser.SER_Nombre })
                  });
            }
        }

        /// <summary>
        /// Retorna las formas de pago que aplican para un cliente crédito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAFormaPago> ObtenerFormasPagoClienteCredito()
        {
            List<TAFormaPago> formasPago = new List<TAFormaPago>();
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                // Actualmente está definido que
                FormasPago_TAR formaPago = contexto.FormasPago_TAR.FirstOrDefault(f => f.FOP_IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CREDITO);
                if (formaPago != null)
                {
                    formasPago.Add(new TAFormaPago()
                    {
                        IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CREDITO,
                        Descripcion = formaPago.FOP_Descripcion,
                        AceptaMixto = formaPago.FOP_AceptaMixto
                    });
                }
            }
            return formasPago;
        }

        /// <summary>
        /// Adiciona las formas de pago asignadas a un servicio
        /// </summary>
        /// <param name="formaPago"></param>
        public void AdicionarFormaPagoServicio(TAFormaPago fp, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioFormaPago_TAR formaPagoEn = new ServicioFormaPago_TAR()
                {
                    SFP_IdServicio = idServicio,
                    SFP_IdFormaPago = fp.IdFormaPago,
                    SFP_FechaGrabacion = DateTime.Now,
                    SFP_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ServicioFormaPago_TAR.Add(formaPagoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina las formas de pago desasignadas a un servicio
        /// </summary>
        /// <param name="formaPago"></param>
        public void EliminaFormaPagoServicio(TAFormaPago fp, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioFormaPago_TAR formaPagoEn = contexto.ServicioFormaPago_TAR.Where(r => r.SFP_IdServicio == idServicio && r.SFP_IdFormaPago == fp.IdFormaPago).First();
                contexto.ServicioFormaPago_TAR.Remove(formaPagoEn);
                TARepositorioAudit.MapearServicioFormaPago(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion UserControl Forma de Pago

        #region UserControl Impuestos

        /// <summary>
        /// Obtiene los impuestos de un servicio
        /// </summary>
        /// <param name="IdServicio">Identificador servicio</param>
        /// <returns>Colección</returns>
        public TAServicioImpuestosDC ObtenerImpuestosPorServicio(int IdServicio)
        {
            TAServicioImpuestosDC impuesto = new TAServicioImpuestosDC();

            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<Impuesto_TAR> fpAsignadas = contexto.ServicioImpuesto_TAR
                  .Where(r => r.SEI_IdServicio == IdServicio)
                  .Select(r => r.Impuesto_TAR);

                var x = fpAsignadas
                  .ToList()
                  .ConvertAll(fp => new TAImpuestosDC()
                  {
                      Identificador = fp.IMP_IdImpuesto,
                      Descripcion = fp.IMP_Descripcion,
                      Valor = fp.IMP_Valor,
                      Asignado = true,
                      Actual = true,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });

                List<Impuesto_TAR> fpDisponibles = contexto.Impuesto_TAR
                  .Except(fpAsignadas)
                  .ToList();

                var y = fpDisponibles
                  .ToList()
                  .ConvertAll(fp => new TAImpuestosDC()
                  {
                      Identificador = fp.IMP_IdImpuesto,
                      Descripcion = fp.IMP_Descripcion,
                      Valor = fp.IMP_Valor,
                      Asignado = false,
                      Actual = false,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });

                impuesto.Impuestos = x.Union(y).ToList();
                impuesto.Impuestos.OrderBy(o => o.Descripcion);
                impuesto.IdServicio = IdServicio;

                return impuesto;
            }
        }

        /// <summary>
        /// Adiciona un impuesto a un servicio
        /// </summary>
        /// <param name="fp"></param>
        /// <param name="idServicio"></param>
        public void AdicionarImpuestoPorServicio(TAImpuestosDC fp, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioImpuesto_TAR impuestoEn = new ServicioImpuesto_TAR()
                {
                    SEI_IdServicio = idServicio,
                    SEI_IdImpuesto = fp.Identificador,
                    SEI_FechaGrabacion = DateTime.Now,
                    SEI_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ServicioImpuesto_TAR.Add(impuestoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un impuesto por servicio
        /// </summary>
        /// <param name="fp">Impuesto</param>
        /// <param name="idServicio">Identificador servicio</param>
        public void EliminaImpuestoPorServicio(TAImpuestosDC fp, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioImpuesto_TAR impuestoEn = contexto.ServicioImpuesto_TAR.Where(r => r.SEI_IdServicio == idServicio && r.SEI_IdImpuesto == fp.Identificador).First();
                contexto.ServicioImpuesto_TAR.Remove(impuestoEn);
                TARepositorioAudit.MapearServicioImpuesto(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion UserControl Impuestos

        #region UserControl Servicios Peso

        /// <summary>
        /// Obtiene los pesos mínimo y máximo de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <returns>Objeto con los pesos</returns>
        public TAServicioPesoDC ObtenerServicioPeso(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TAServicioPesoDC servicioPeso = new TAServicioPesoDC();

                ServicioMensajeria_TAR servicioMensajeria = contexto.ServicioMensajeria_TAR
                  .Where(r => r.SME_IdServicio == idServicio)
                  .FirstOrDefault();

                if (servicioMensajeria == null)
                {
                    servicioPeso.IdServicio = idServicio;
                    servicioPeso.PesoMinimo = 0;
                    servicioPeso.PesoMaximo = 0;
                    servicioPeso.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                    AdicionarServicioPeso(servicioPeso);
                }
                else
                {
                    servicioPeso.PesoMinimo = servicioMensajeria.SME_PesoMínimo;
                    servicioPeso.PesoMaximo = servicioMensajeria.SME_PesoMaximo;
                    servicioPeso.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                }

                return servicioPeso;
            }
        }

        /// <summary>
        /// Adiciona un Servicio Peso
        /// </summary>
        /// <param name="servicioPeso">Objeto Servicio Peso</param>
        public void AdicionarServicioPeso(TAServicioPesoDC servicioPeso)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioMensajeria_TAR servicioEn = new ServicioMensajeria_TAR()
                {
                    SME_IdServicio = servicioPeso.IdServicio,
                    SME_PesoMínimo = servicioPeso.PesoMinimo,
                    SME_PesoMaximo = servicioPeso.PesoMaximo,
                    SME_FechaGrabacion = DateTime.Now,
                    SME_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.ServicioMensajeria_TAR.Add(servicioEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un servicio peso
        /// </summary>
        /// <param name="servicioPeso">Objeto Servicio Peso</param>
        public void EditarServicioPeso(TAServicioPesoDC servicioPeso)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ServicioMensajeria_TAR servicioEn = contexto.ServicioMensajeria_TAR
                  .Where(r => r.SME_IdServicio == servicioPeso.IdServicio)
                  .FirstOrDefault();

                if (servicioEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                servicioEn.SME_PesoMínimo = servicioPeso.PesoMinimo;
                servicioEn.SME_PesoMaximo = servicioPeso.PesoMaximo;
                TARepositorioAudit.MapearServicioMensajeria(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion UserControl Servicios Peso

        #region UserControl Parámetros Lista de Precios Servicios

        /// <summary>
        /// Retorna los parámetros de lista precio servicio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto listaprecioservicio</param>
        /// <returns>Retorna parámetos listaprecioservicio</returns>
        public TAListaPrecioServicioParametrosDC ObtenerParametrosListaPrecioServicio(TAListaPrecioServicioParametrosDC listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TAListaPrecioServicioParametrosDC parametro = new TAListaPrecioServicioParametrosDC();

                ListaPrecioServicio_TAR listaPrecioServicioEn = contexto.ListaPrecioServicio_TAR
                  .Where(r => r.LPS_IdServicio == listaPrecioServicio.IdServicio && r.LPS_IdListaPrecios == listaPrecioServicio.IdListaPrecio)
                  .FirstOrDefault();

                parametro.PrimaSeguro = listaPrecioServicioEn.LPS_PrimaSeguros;

                return parametro;
            }
        }

        /// <summary>
        /// Edita los parámetros de lista precio servicio
        /// </summary>
        /// <param name="listaPrecioServicio">Objeto con los parámetos de la lista de precios</param>
        public void EditarParametrosListaPrecioServicio(TAListaPrecioServicioParametrosDC listaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ListaPrecioServicio_TAR lpsEn = contexto.ListaPrecioServicio_TAR
                  .Where(r => r.LPS_IdListaPrecios == listaPrecioServicio.IdListaPrecio && r.LPS_IdServicio == listaPrecioServicio.IdServicio)
                  .FirstOrDefault();

                if (lpsEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                lpsEn.LPS_PrimaSeguros = listaPrecioServicio.PrimaSeguro;
                TARepositorioAudit.MapeoAuditListaPrecioServicio(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion UserControl Parámetros Lista de Precios Servicios

        #region UserControl Trayecto  Subtrayecto Rango

        /// <summary>
        /// Obtiene Precio trayecto Rango
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Precio trayecto rango</returns>
        public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTrayectoSubTrayectoRango(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);

                filtro.Add("PTR_IdListaPrecioServicio", idListaPrecioServicio);
                return contexto.ConsultarContainsTrayectSubTrayectValorAdi_VTAR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(r => new TAPrecioTrayectoDC
                  {
                      TipoTrayecto = new TATipoTrayecto()
                      {
                          IdTipoTrayecto = r.TTR_IdTipoTrayecto,
                          Descripcion = r.Trayecto
                      },

                      TipoSubTrayecto = new TATipoSubTrayecto()
                      {
                          IdTipoSubTrayecto = r.TST_IdTipoSubTrayecto,
                          Descripcion = r.SubTrayecto
                      },

                      IdServicio = idServicio,
                      IdPrecioTrayectoSubTrayecto = r.PTV_IdPrecioTrayectoSubTrayect,
                      IdTrayectoSubTrayecto = r.TRS_IdTrayectoSubTrayecto,
                      IdTipoValorAdicional = r.PTV_IdTipoValorAdicional,
                      KiloAdicional = r.KiloAdicional,
                      ServicioRetorno = r.ServicioRetorno,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,

                      Precio = new ObservableCollection<TAPrecioTrayectoRangoDC>(contexto.PrecioTrayectoRango_TAR.Where(p => p.PPR_IdPrecioTrayecto == r.IdPrecioTrayecto)
                        .OrderBy(o => o.PPR_Inicial)
                        .ToList()
                      .ConvertAll(p => new TAPrecioTrayectoRangoDC()
                      {
                          IdPrecioTrayectoRango = p.PRR_IdPrecioTrayectoRango,
                          PesoInicial = p.PPR_Inicial,
                          PesoFinal = p.PPR_Final,
                          Valor = p.PPR_Valor,
                          Porcentaje = p.PPR_Porcentaje,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                      }))
                  });
            }
        }

        /// <summary>
        /// Obtiene los precios de los trayectos por servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idListaPrecio"></param>
        /// <returns></returns>
        public IEnumerable<TAPrecioTrayectoDC> ObtenerPrecioTraySubTrayectoRango(int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerTrayectSubTrayectValorAdi_TAR(idListaPrecio, idServicio)
                  .ToList()
                  .ConvertAll(r => new TAPrecioTrayectoDC
                  {
                      TipoTrayecto = new TATipoTrayecto()
                      {
                          IdTipoTrayecto = r.TTR_IdTipoTrayecto,
                          Descripcion = r.Trayecto
                      },

                      TipoSubTrayecto = new TATipoSubTrayecto()
                      {
                          IdTipoSubTrayecto = r.TST_IdTipoSubTrayecto,
                          Descripcion = r.SubTrayecto
                      },

                      IdServicio = idServicio,
                      IdPrecioTrayectoSubTrayecto = r.IdPrecioTrayecto,
                      IdTrayectoSubTrayecto = r.TRS_IdTrayectoSubTrayecto,
                      IdTipoValorAdicional = r.PTV_IdTipoValorAdicional,
                      KiloAdicional = r.ValorMensajeriaBase,
                      ServicioRetorno = r.ServicioRetorno == null ? 0 : r.ServicioRetorno.Value,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,

                      Precio = new ObservableCollection<TAPrecioTrayectoRangoDC>(contexto.PrecioTrayectoRango_TAR.Where(p => p.PPR_IdPrecioTrayecto == r.IdPrecioTrayecto)
                        .OrderBy(o => o.PPR_Inicial)
                        .ToList()
                      .ConvertAll(p => new TAPrecioTrayectoRangoDC()
                      {
                          IdPrecioTrayectoRango = p.PRR_IdPrecioTrayectoRango,
                          PesoInicial = p.PPR_Inicial,
                          PesoFinal = p.PPR_Final,
                          KiloAdicional = r.ValorMensajeriaBase,
                          Porcentaje = p.PPR_Porcentaje,
                          Valor = p.PPR_Valor,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                      }))
                  });
            }
        }

        /// <summary>
        /// Adiciona trayecto subtrayecto precio rango
        /// </summary>
        /// <param name="precioTrayecto">Objeto</param>
        public void AdicionarPrecioTrayectoSubTrayectoRango(TAPrecioTrayectoDC consolidadoPrecioTrayectoRango, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, consolidadoPrecioTrayectoRango.IdListaPrecio);
                int idTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(consolidadoPrecioTrayectoRango.TipoTrayecto.IdTipoTrayecto, consolidadoPrecioTrayectoRango.TipoSubTrayecto.IdTipoSubTrayecto);

                consolidadoPrecioTrayectoRango.Precio.ToList().ForEach(precio =>
                {
                    PrecioTrayectoRango_TAR precioTrayectoRango = new PrecioTrayectoRango_TAR()
                    {
                        PPR_IdPrecioTrayecto = consolidadoPrecioTrayectoRango.IdPrecioTrayectoSubTrayecto,
                        PPR_Inicial = precio.PesoInicial,
                        PPR_Final = precio.PesoFinal,
                        PPR_Valor = precio.Valor,
                        PPR_Porcentaje = precio.Porcentaje,
                        PPR_FechaGrabacion = DateTime.Now,
                        PPR_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.PrecioTrayectoRango_TAR.Add(precioTrayectoRango);
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita precio trayecto subtrayecto rango
        /// </summary>
        /// <param name="consolidadoPrecioTrayectoRango">Objeto</param>
        public void EditarPrecioTrayectoSubTrayectoRango(TAPrecioTrayectoDC consolidadoPrecioTrayectoRango, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = TARepositorio.Instancia.ObtenerIdentificadorListaPrecioServicio(idServicio, consolidadoPrecioTrayectoRango.IdListaPrecio);
                int idTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(consolidadoPrecioTrayectoRango.TipoTrayecto.IdTipoTrayecto, consolidadoPrecioTrayectoRango.TipoSubTrayecto.IdTipoSubTrayecto);

                consolidadoPrecioTrayectoRango.Precio.ToList().ForEach(precio =>
                {
                    if (precio.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    {
                        PrecioTrayectoRango_TAR borrarPrecio = contexto.PrecioTrayectoRango_TAR
                          .Where(bp => bp.PRR_IdPrecioTrayectoRango == precio.IdPrecioTrayectoRango)
                          .FirstOrDefault();
                        contexto.PrecioTrayectoRango_TAR.Remove(borrarPrecio);
                    }
                    else
                    {
                        PrecioTrayectoRango_TAR precioTrayectoRango = contexto.PrecioTrayectoRango_TAR
                          .Where(pr => pr.PRR_IdPrecioTrayectoRango == precio.IdPrecioTrayectoRango)
                          .FirstOrDefault();

                        if (precioTrayectoRango != null)
                        {
                            precioTrayectoRango.PPR_Inicial = precio.PesoInicial;
                            precioTrayectoRango.PPR_Final = precio.PesoFinal;
                            precioTrayectoRango.PPR_Valor = precio.Valor;
                            precioTrayectoRango.PPR_Porcentaje = precio.Porcentaje;
                        }
                        else
                        {
                            PrecioTrayectoRango_TAR precioTrayectoRangoNuevo = new PrecioTrayectoRango_TAR()
                            {
                                PPR_IdPrecioTrayecto = consolidadoPrecioTrayectoRango.IdPrecioTrayectoSubTrayecto,
                                PPR_Inicial = precio.PesoInicial,
                                PPR_Final = precio.PesoFinal,
                                PPR_Valor = precio.Valor,
                                PPR_Porcentaje = precio.Porcentaje,
                                PPR_FechaGrabacion = DateTime.Now,
                                PPR_CreadoPor = ControllerContext.Current.Usuario
                            };

                            contexto.PrecioTrayectoRango_TAR.Add(precioTrayectoRangoNuevo);
                        }
                    }
                });

                TARepositorioAudit.MapearPrecioTrayecto(contexto);
                TARepositorioAudit.MapearPrecioTraSubTraValAdicional(contexto);
                TARepositorioAudit.MapearPrecioTrayectoRango(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina precio trayecto subtrayecto rango
        /// </summary>
        /// <param name="consolidadoPrecioTrayectoRango">Objeto</param>
        public void EliminarPrecioTrayectoSubTrayectoRango(TAPrecioTrayectoDC consolidadoPrecioTrayectoRango)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioTrayecto_TAR precioTrayecto = contexto.PrecioTrayecto_TAR
                  .Where(pt => pt.PTR_IdPrecioTrayectoSubTrayect == consolidadoPrecioTrayectoRango.IdPrecioTrayectoSubTrayecto)
                  .FirstOrDefault();

                contexto.PrecioTrayecto_TAR.Remove(precioTrayecto);

                PrecioTrayeSubTraValorAdic_TAR valAdicional = contexto.PrecioTrayeSubTraValorAdic_TAR
                  .Where(va => va.PTV_IdPrecioTrayectoSubTrayect == precioTrayecto.PTR_IdPrecioTrayectoSubTrayect)
                  .FirstOrDefault();

                contexto.PrecioTrayeSubTraValorAdic_TAR.Remove(valAdicional);

                consolidadoPrecioTrayectoRango.Precio.ToList().ForEach(precio =>
                {
                    PrecioTrayectoRango_TAR precioTrayectoRango = contexto.PrecioTrayectoRango_TAR
                      .Where(pr => pr.PRR_IdPrecioTrayectoRango == precio.IdPrecioTrayectoRango)
                      .FirstOrDefault();

                    contexto.PrecioTrayectoRango_TAR.Remove(precioTrayectoRango);
                });

                TARepositorioAudit.MapearPrecioTrayecto(contexto);
                TARepositorioAudit.MapearPrecioTraSubTraValAdicional(contexto);
                TARepositorioAudit.MapearPrecioTrayectoRango(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion UserControl Trayecto  Subtrayecto Rango

        #region UserControl Precio Rango

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
                int idLps = int.Parse(idListaPrecioServicio);

                IEnumerable<TAPrecioRangoDC> precioRangos = contexto.PrecioRango_TAR
                  .Where(pr => pr.PRA_IdListaPrecioServicio == idLps)
                  .OrderBy(o => o.PRA_Inicial).ThenBy(o => o.PRA_Final)
                  .ToList()
                  .ConvertAll(r => new TAPrecioRangoDC()
                  {
                      IdPrecioRango = r.PRA_IdPrecioRango,
                      PrecioInicial = r.PRA_Inicial,
                      PrecioFinal = r.PRA_Final,
                      Valor = r.PRA_Valor,
                      Porcentaje = r.PRA_Porcentaje,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                  });

                if (precioRangos.Count() == 0)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_SE_ENCUENTRAN_RANGO_EN_LISTA_DE_PRECIOS.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_SE_ENCUENTRAN_RANGO_EN_LISTA_DE_PRECIOS));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return precioRangos;
            }
        }

        /// <summary>
        /// Obtiene de precio rango que posee un cliente contado a partir de un contrato
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public IEnumerable<TAPrecioRangoDC> ObtenerPrecioRangoClienteContado(int idContrato, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerPrecioRangosClienteContado_CLI(Framework.Servidor.Comun.ConstantesFramework.ESTADO_ACTIVO, idContrato, idServicio).ToList().
                  ConvertAll(r => new TAPrecioRangoDC()
                  {
                      PrecioInicial = r.PRA_Inicial,
                      PrecioFinal = r.PRA_Final,
                      Valor = r.PRA_Valor,
                      Porcentaje = r.PRA_Porcentaje
                  });
            }
        }

        /// <summary>
        /// Adiciona Precio Rango
        /// </summary>
        /// <param name="precioRango">Objeto Precio Rango</param>
        /// <param name="idServicio">Identidicador Servicio</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        public void AdicionarPrecioRango(TAPrecioRangoDC precioRango, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioRango_TAR precioRangoEn = new PrecioRango_TAR()
                {
                    PRA_IdListaPrecioServicio = idListaPrecioServicio,
                    PRA_Inicial = precioRango.PrecioInicial,
                    PRA_Final = precioRango.PrecioFinal,
                    PRA_Valor = precioRango.Valor,
                    PRA_Porcentaje = precioRango.Porcentaje,
                    PRA_FechaGrabacion = DateTime.Now,
                    PRA_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.PrecioRango_TAR.Add(precioRangoEn);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita precio rango
        /// </summary>
        /// <param name="precioRango">Objeto Precio Rango</param>
        /// <param name="idServicio">Identidicador Servicio</param>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        public void EditarPrecioRango(TAPrecioRangoDC precioRango)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioRango_TAR precioRangoEn = contexto.PrecioRango_TAR
                  .Where(r => r.PRA_IdPrecioRango == precioRango.IdPrecioRango)
                  .FirstOrDefault();

                if (precioRangoEn == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                precioRangoEn.PRA_Inicial = precioRango.PrecioInicial;
                precioRangoEn.PRA_Final = precioRango.PrecioFinal;
                precioRangoEn.PRA_Valor = precioRango.Valor;
                precioRangoEn.PRA_Porcentaje = precioRango.Porcentaje;
                TARepositorioAudit.MapearPrecioRango(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina precio rango
        /// </summary>
        /// <param name="precioRango">Objeto Precio Rango</param>
        public void EliminarPrecioRango(TAPrecioRangoDC precioRango)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioRango_TAR precioRangoEn = contexto.PrecioRango_TAR
                  .Where(r => r.PRA_IdPrecioRango == precioRango.IdPrecioRango)
                  .FirstOrDefault();

                contexto.PrecioRango_TAR.Remove(precioRangoEn);
                TARepositorioAudit.MapearPrecioRango(contexto);
                contexto.SaveChanges();
            }
        }

        #endregion UserControl Precio Rango

        #region UserControl Excepción Trayecto

        /// <summary>
        /// Obtiene excepciones de precio trayecto subtrayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>excepción trayecto subtrayecto</returns>
        public IEnumerable<TATrayectoExcepcionDC> ObtenerExcepcionTrayectoSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ExcepcionTrayecto_VTAR>("EXT_IdListaPrecios", idListaPrecio.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarExcepcionTrayecto_VTAR(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
               .Where(w => w.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
               .ConvertAll<TATrayectoExcepcionDC>(r =>
               {
                   TATrayectoExcepcionDC dato = new TATrayectoExcepcionDC
                   {
                       IdListaPrecio = r.EXT_IdListaPrecios,
                       IdLocalidadOrigen = r.EXT_IdLocalidadOrigen,
                       NombreLocalidadOrigen = contexto.Localidad_PAR.Where(f => f.LOC_IdLocalidad == r.EXT_IdLocalidadOrigen).FirstOrDefault().LOC_Nombre,
                       NombreLocalidadDestino = contexto.Localidad_PAR.Where(f => f.LOC_IdLocalidad == r.EXT_IdLocalidadDestino).FirstOrDefault().LOC_Nombre,
                       IdLocalidadDestino = r.EXT_IdLocalidadDestino,
                       IdTrayectoSubTrayectoExcepcion = r.EXT_IdExcepionTrayecto,
                       Editable = false,
                       Pais = new PALocalidadDC(),
                       EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                       IdTrayectosubTrayecto = r.EXT_IdTrayectoSubTrayecto,
                       Trayecto = new TATipoTrayecto() { IdTipoTrayecto = r.TRS_IdTipoTrayecto, Descripcion = r.TTR_Descripcion },
                       Subtrayecto = new TATipoSubTrayecto() { IdTipoSubTrayecto = r.TRS_IdTipoSubTrayecto, Descripcion = r.TST_Descripcion },
                   };
                   return dato;
               }).ToList();
            }
        }

        /// <summary>
        /// Metodo para obtener los servicios por excepcion
        /// </summary>
        /// <param name="excepcion"></param>
        /// <returns></returns>
        public IEnumerable<TATrayectoExcepcionServicioDC> ObtenerServiciosPorExcepcion(TATrayectoExcepcionDC excepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PrecioServicioExcepcionTrayecto_TAR
                  .Where(w => w.SET_IdPrecioServicioExcepionTrayecto == excepcion.IdTrayectoSubTrayectoExcepcion)
                  .ToList()
                  .ConvertAll<TATrayectoExcepcionServicioDC>(r =>
                  {
                      TATrayectoExcepcionServicioDC dato = new TATrayectoExcepcionServicioDC
                      {
                          IdTrayectoSubTrayectoExcepcion = r.SET_IdPrecioServicioExcepionTrayecto,
                          TiempoEntrega = 5,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          Servicio = new TAServicioDC()
                          {
                              IdServicio = r.SET_IdListaPrecioServicio,
                              Nombre = contexto.Servicio_TAR.Where(m => m.SER_IdServicio == r.SET_IdListaPrecioServicio).FirstOrDefault().SER_Nombre
                          },
                      };
                      return dato;
                  }).ToList();
            }
        }

        /// <summary>
        /// Metodo para adicionar una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="excepcion"></param>
        public long AdicionarExcepcionTrayectoSubTrayecto(TATrayectoExcepcionDC excepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TrayectoSubTrayecto_TAR trayectoSubtrayectoEn = contexto.TrayectoSubTrayecto_TAR
                 .Where(r => r.TRS_IdTipoTrayecto == excepcion.Trayecto.IdTipoTrayecto && r.TRS_IdTipoSubTrayecto == excepcion.Subtrayecto.IdTipoSubTrayecto)
                 .FirstOrDefault();

                if (trayectoSubtrayectoEn == null)
                {
                    TrayectoSubTrayecto_TAR ts = new TrayectoSubTrayecto_TAR()
                    {
                        TRS_IdTipoTrayecto = excepcion.Trayecto.IdTipoTrayecto,
                        TRS_IdTipoSubTrayecto = excepcion.Subtrayecto.IdTipoSubTrayecto,
                        TRS_FechaGrabacion = DateTime.Now,
                        TRS_CreadoPor = ControllerContext.Current.Usuario
                    };
                    contexto.TrayectoSubTrayecto_TAR.Add(ts);
                    contexto.SaveChanges();
                    excepcion.IdTrayectosubTrayecto = ts.TRS_IdTrayectoSubTrayecto;
                }
                else if (trayectoSubtrayectoEn != null)
                {
                    excepcion.IdTrayectosubTrayecto = trayectoSubtrayectoEn.TRS_IdTrayectoSubTrayecto;
                }

                //ExcepcionTrayecto_TAR excepcionEn = new ExcepcionTrayecto_TAR()
                //{
                //  EXT_IdLocalidadOrigen = excepcion.IdLocalidadOrigen,
                //  EXT_IdLocalidadDestino = excepcion.IdLocalidadDestino,
                //  EXT_IdListaPrecios = excepcion.IdListaPrecio,
                //  EXT_IdTrayectoSubTrayecto = excepcion.IdTrayectosubTrayecto,
                //  EXT_FechaGrabacion = DateTime.Now,
                //  EXT_CreadoPor = ControllerContext.Current.Usuario
                //};
                //contexto.ExcepcionTrayecto_TAR.Add(excepcionEn);
                contexto.SaveChanges();

                //return excepcionEn.EXT_IdExcepionTrayecto;

                return 3;
            }
        }

        /// <summary>
        /// Metodo para modificar una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="servicio"></param>
        public void AdicionarServicioExcepcion(TATrayectoExcepcionServicioDC servicio)
        {
            //  using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //  {
            //    ServicioExcepcionTrayecto_TAR excepcionEn = new ServicioExcepcionTrayecto_TAR()
            //    {
            //      SET_IdExcepionTrayecto = servicio.IdTrayectoSubTrayectoExcepcion,
            //      SET_IdServicio = servicio.Servicio.IdServicio,
            //      SET_TiempoEntrega = servicio.TiempoEntrega,
            //      SET_FechaGrabacion = DateTime.Now,
            //      SET_CreadoPor = ControllerContext.Current.Usuario
            //    };

            //    contexto.ServicioExcepcionTrayecto_TAR.Add(excepcionEn);
            //    contexto.SaveChanges();
            //  }
        }

        /// <summary>
        /// Metodo para modificar una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="servicio"></param>
        public void ModificarServicioExcepcion(TATrayectoExcepcionServicioDC servicio)
        {
            //  using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //  {
            //    ServicioExcepcionTrayecto_TAR excepcionEn = contexto.ServicioExcepcionTrayecto_TAR
            //    .Where(f => f.SET_IdExcepionTrayecto == servicio.IdTrayectoSubTrayectoExcepcion && f.SET_IdServicio == servicio.Servicio.IdServicio)
            //    .FirstOrDefault();
            //    excepcionEn.SET_TiempoEntrega = servicio.TiempoEntrega;
            //    TARepositorioAudit.MapearAuditExcepcionServicioTrayectoSub(contexto);
            //    contexto.SaveChanges();
            //  }
        }

        /// <summary>
        /// Metodo para modificar una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="servicio"></param>
        public void EliminarServicioExcepcion(TATrayectoExcepcionServicioDC servicio)
        {
            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  ServicioExcepcionTrayecto_TAR excepcionEn = contexto.ServicioExcepcionTrayecto_TAR
            //  .Where(f => f.SET_IdExcepionTrayecto == servicio.IdTrayectoSubTrayectoExcepcion && f.SET_IdServicio == servicio.Servicio.IdServicio)
            //  .FirstOrDefault();
            //  if (excepcionEn == null)
            //  {
            //    ControllerException ex = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
            //    throw new FaultException<ControllerException>(ex);
            //  }
            //  else
            //    TARepositorioAudit.MapearAuditExcepcionServicioTrayectoSub(contexto);
            //  contexto.ServicioExcepcionTrayecto_TAR.Remove(excepcionEn);
            //  contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Metodo para eliminar una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="excepcion"></param>
        public void EliminarExcepcionTrayectoSubTrayecto(TATrayectoExcepcionDC excepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //ExcepcionTrayecto_TAR excepcionEn = contexto.ExcepcionTrayecto_TAR
                //.Where(r => r.EXT_IdExcepionTrayecto == excepcion.IdTrayectoSubTrayectoExcepcion && r.EXT_IdListaPrecios == excepcion.IdListaPrecio && r.EXT_IdTrayectoSubTrayecto == excepcion.IdTrayectosubTrayecto)
                //.FirstOrDefault();
                //if (excepcionEn == null)
                //{
                //  ControllerException ex = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                //  throw new FaultException<ControllerException>(ex);
                //}
                //else
                //  TARepositorioAudit.MapearAuditExcepcionTrayectoSub(contexto);
                //contexto.ExcepcionTrayecto_TAR.Remove(excepcionEn);
                //contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Metodo para modificar una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="servicio"></param>
        public void EliminarServiciosExcepcion(TATrayectoExcepcionDC excepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //IList<ServicioExcepcionTrayecto_TAR> excepcionEn = contexto.ServicioExcepcionTrayecto_TAR
                //.Where(f => f.SET_IdExcepionTrayecto == excepcion.IdTrayectoSubTrayectoExcepcion)
                //.ToList();
                //if (excepcionEn == null)
                //{
                //  ControllerException ex = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                //  throw new FaultException<ControllerException>(ex);
                //}
                //else
                //  excepcionEn.ToList().ForEach(g =>
                //    {
                //      contexto.ServicioExcepcionTrayecto_TAR.Remove(g);
                //      TARepositorioAudit.MapearAuditExcepcionServicioTrayectoSub(contexto);
                //      contexto.SaveChanges();
                //    });
            }
        }

        /// <summary>
        /// Valida si existe kilo adicional
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <returns>Booleano</returns>
        public bool ValidarExcepcionTrayectoSubTrayecto(string idLocalidadOrigen, string idLocalidadDestino, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //ExcepcionTrayecto_TAR consulta = contexto.ExcepcionTrayecto_TAR.Include("TrayectoSubTrayecto_TAR")
                //  .Where(r => r.EXT_IdLocalidadOrigen == idLocalidadOrigen && r.EXT_IdLocalidadDestino == idLocalidadDestino && r.EXT_IdListaPrecios == idListaPrecio && r.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                // .FirstOrDefault();

                //if (consulta == null)
                //  return false;
                //else
                return true;
            }
        }

        /// <summary>
        /// Valida si existe kilo adicional
        /// </summary>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <returns>Booleano</returns>
        public bool ValidarExcepcionTrayectoKiloInicial(string idLocalidadOrigen, string idLocalidadDestino, int idListaPrecio)
        {
            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  ExcepcionTrayecto_TAR consulta = contexto.ExcepcionTrayecto_TAR.Include("TrayectoSubTrayecto_TAR")
            //    .Where(r => r.EXT_IdLocalidadOrigen == idLocalidadOrigen && r.EXT_IdLocalidadDestino == idLocalidadDestino && r.EXT_IdListaPrecios == idListaPrecio && r.TrayectoSubTrayecto_TAR.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
            //    .FirstOrDefault();

            //  if (consulta == null)
            //    return false;
            //  else
            return true;

            //}
        }

        /// <summary>
        /// Adiciona un trayecto subtrayecto
        /// </summary>
        public void AdicionarExcepcionTrayectoKiloInicial(TATrayectoExcepcionDC trayectoExcepcion)
        {
            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  int idTrayectoSubTrayecto = 0;

            //  TrayectoSubTrayecto_TAR consulta = contexto.TrayectoSubTrayecto_TAR
            //    .Where(r => r.TRS_IdTipoTrayecto == trayectoExcepcion.Trayecto.IdTipoTrayecto && r.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).FirstOrDefault();

            //  if (consulta == null)
            //  {
            //    TrayectoSubTrayecto_TAR trayectoSubTrayecto = new TrayectoSubTrayecto_TAR()
            //    {
            //      TRS_IdTipoTrayecto = trayectoExcepcion.Trayecto.IdTipoTrayecto,
            //      TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL,
            //      TRS_FechaGrabacion = DateTime.Now,
            //      TRS_CreadoPor = ControllerContext.Current.Usuario
            //    };

            //    contexto.TrayectoSubTrayecto_TAR.Add(trayectoSubTrayecto);

            //    idTrayectoSubTrayecto = trayectoSubTrayecto.TRS_IdTrayectoSubTrayecto;
            //  }
            //  else if (consulta != null)
            //    idTrayectoSubTrayecto = consulta.TRS_IdTrayectoSubTrayecto;

            //  ExcepcionTrayecto_TAR trayectoEn = new ExcepcionTrayecto_TAR()
            //  {
            //    EXT_IdLocalidadOrigen = trayectoExcepcion.IdLocalidadOrigen,
            //    EXT_IdLocalidadDestino = trayectoExcepcion.IdLocalidadDestino,
            //    EXT_IdTrayectoSubTrayecto = idTrayectoSubTrayecto,
            //    EXT_IdListaPrecios = trayectoExcepcion.IdListaPrecio,
            //    EXT_FechaGrabacion = DateTime.Now,
            //    EXT_CreadoPor = ControllerContext.Current.Usuario
            //  };

            //  contexto.ExcepcionTrayecto_TAR.Add(trayectoEn);

            //  trayectoExcepcion.Servicios.ToList().ForEach(s =>
            //  {
            //    ServicioExcepcionTrayecto_TAR servicioTrayecto = new ServicioExcepcionTrayecto_TAR()
            //    {
            //      SET_IdServicio = s.Servicio.IdServicio,
            //      SET_TiempoEntrega = 0,
            //      SET_FechaGrabacion = DateTime.Now,
            //      SET_CreadoPor = ControllerContext.Current.Usuario
            //    };

            //    contexto.ServicioExcepcionTrayecto_TAR.Add(servicioTrayecto);
            //  });

            //  contexto.SaveChanges();
            //}
        }

        #endregion UserControl Excepción Trayecto

        #region UserControl Precio Trayecto Mensajería

        /// <summary>
        /// Obtiene los datos de precio trayecto subtrayecto de mensajería
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATrayectoMensajeriaDC> ObtenerTiposTrayectoMensajeria(int idListaPrecio, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
                int idLp = int.Parse(idListaPrecioServicio);

                return contexto.TrayectoPrecioMensajeria_VTAR
                  .Where(w => w.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL && w.PTR_IdListaPrecioServicio == idLp)
                  .ToList()
                  .ConvertAll(r => new TATrayectoMensajeriaDC()
                  {
                      IdPrecioTrayectoSubTrayecto = r.PTR_IdPrecioTrayectoSubTrayect,
                      Trayecto = new TATipoTrayecto()
                      {
                          IdTipoTrayecto = r.TRS_IdTipoTrayecto,
                          Descripcion = r.TTR_Descripcion
                      },
                      ValorBaseKiloInicial = r.PrecioBase,
                      PorcentajeIncremento = r.PTR_Porcentaje,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                      ValorFijadoKiloInicial = r.PTR_ValorFijo,

                      PrecioTrayecto = new ObservableCollection<TAPrecioTrayectoMensajeriaDC>(contexto.TrayectoPrecioMensajeria_VTAR
                         .Where(ptm => ptm.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL && ptm.PTR_IdListaPrecioServicio == idLp && ptm.TRS_IdTipoTrayecto == r.TRS_IdTipoTrayecto)
                         .ToList()
                         .ConvertAll(pt => new TAPrecioTrayectoMensajeriaDC()
                         {
                             IdPrecioTrayectoSubTrayecto = pt.PTR_IdPrecioTrayectoSubTrayect,
                             ValorBaseKiloAdicional = pt.PrecioBase,
                             PorcentajeIncremento = pt.PTR_Porcentaje,
                             ValorFijadoKiloAdicional = pt.PTR_ValorFijo,
                             SubTrayecto = new TATipoSubTrayecto()
                             {
                                 IdTipoSubTrayecto = pt.TRS_IdTipoSubTrayecto,
                                 Descripcion = pt.TST_Descripcion
                             },
                             EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                             SubTrayectosDisponibles = contexto.TrayectoSubTrayecto_TAR.Include("TipoSubTrayecto_TAR")
                              .Where(tr => tr.TRS_IdTipoTrayecto == r.TRS_IdTipoTrayecto && tr.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                              .ToList()
                              .ConvertAll(dst => new TATipoSubTrayecto()
                              {
                                  IdTipoSubTrayecto = dst.TRS_IdTipoSubTrayecto,
                                  Descripcion = dst.TipoSubTrayecto_TAR.TST_Descripcion
                              })
                         })),
                  });
            }
        }

        /// <summary>
        /// Obtiene los datos de precio trayecto subtrayecto de mensajería
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TATrayectoMensajeriaDC> ObtenerTiposTrayectoUnidadNegocioMensajeria(int idListaPrecio, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
                int idLp = int.Parse(idListaPrecioServicio);

                return contexto.paObtenerPrecioTraySubTraMensajeria_TAR(idLp, idListaPrecio, null)
                  .ToList()
                  .ConvertAll(r => new TATrayectoMensajeriaDC()
                  {
                      IdPrecioTrayectoSubTrayecto = r.PTR_IdPrecioTrayectoSubTrayect,
                      Trayecto = new TATipoTrayecto()
                      {
                          IdTipoTrayecto = r.TRS_IdTipoTrayecto,
                          Descripcion = r.TTR_Descripcion
                      },
                      ValorBaseKiloInicial = r.PrecioBase,
                      PorcentajeIncremento = r.PTR_Porcentaje,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                      ValorFijadoKiloInicial = r.PTR_ValorFijo,

                      PrecioTrayecto = new ObservableCollection<TAPrecioTrayectoMensajeriaDC>(contexto.paObtenerPrecioTraySubTraMensajeria_TAR(idLp, idListaPrecio, r.TRS_IdTipoTrayecto)
                       .ToList()
                       .ConvertAll(pt => new TAPrecioTrayectoMensajeriaDC()
                       {
                           IdPrecioTrayectoSubTrayecto = pt.PTR_IdPrecioTrayectoSubTrayect,
                           ValorBaseKiloAdicional = pt.PrecioBase,
                           PorcentajeIncremento = pt.PTR_Porcentaje,
                           ValorFijadoKiloAdicional = pt.PTR_ValorFijo,
                           SubTrayecto = new TATipoSubTrayecto()
                           {
                               IdTipoSubTrayecto = pt.TRS_IdTipoSubTrayecto,
                               Descripcion = pt.TST_Descripcion
                           },
                           EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                           SubTrayectosDisponibles = contexto.TrayectoSubTrayecto_TAR.Include("TipoSubTrayecto_TAR")
                            .Where(tr => tr.TRS_IdTipoTrayecto == r.TRS_IdTipoTrayecto && tr.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                            .ToList()
                            .ConvertAll(dst => new TATipoSubTrayecto()
                            {
                                IdTipoSubTrayecto = dst.TRS_IdTipoSubTrayecto,
                                Descripcion = dst.TipoSubTrayecto_TAR.TST_Descripcion
                            })
                       })),
                  });
            }
        }

        /// <summary>
        /// Adiciona un precio trayecto de mensajería
        /// </summary>
        /// <param name="trayectoMensajeria">Objeto precio trayecto</param>
        public void AdicionarTrayectoMensajeria(TATrayectoMensajeriaDC trayectoMensajeria)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int idTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL);
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(trayectoMensajeria.IdServicio, trayectoMensajeria.IdListaPrecio);

                if (idTrayectoSubTrayecto == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_CREADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_CREADO)));
                }

                if (trayectoMensajeria.IdServicio != TAConstantesServicios.SERVICIO_MENSAJERIA)
                {
                    PrecioTrayecto_TAR precioTrayecto = new PrecioTrayecto_TAR()
                    {
                        PTR_IdListaPrecioServicio = int.Parse(idListaPrecioServicio),
                        PTR_IdTrayectoSubTrayecto = idTrayectoSubTrayecto,
                        PTR_ValorFijo = trayectoMensajeria.ValorFijadoKiloInicial,
                        PTR_Porcentaje = trayectoMensajeria.PorcentajeIncremento,
                        PTR_FechaGrabacion = DateTime.Now,
                        PTR_CreadoPor = ControllerContext.Current.Usuario
                    };

                    contexto.PrecioTrayecto_TAR.Add(precioTrayecto);
                }
                else
                {
                    contexto.paAdicionarTraySubTraySvcBaseMen_TAR(trayectoMensajeria.IdListaPrecio, idTrayectoSubTrayecto, trayectoMensajeria.ValorFijadoKiloInicial, ControllerContext.Current.Usuario, trayectoMensajeria.PorcentajeIncremento);
                }

                trayectoMensajeria.PrecioTrayecto.ToList().ForEach(pt =>
                {
                    if (trayectoMensajeria.IdServicio != TAConstantesServicios.SERVICIO_MENSAJERIA)
                    {
                        PrecioTrayecto_TAR precioTrayectokiloAdicional = new PrecioTrayecto_TAR()
                        {
                            PTR_IdListaPrecioServicio = int.Parse(idListaPrecioServicio),
                            PTR_IdTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, pt.SubTrayecto.IdTipoSubTrayecto),
                            PTR_ValorFijo = pt.ValorFijadoKiloAdicional,
                            PTR_Porcentaje = pt.PorcentajeIncremento,
                            PTR_FechaGrabacion = DateTime.Now,
                            PTR_CreadoPor = ControllerContext.Current.Usuario
                        };
                        contexto.PrecioTrayecto_TAR.Add(precioTrayectokiloAdicional);
                    }
                    else
                    {
                        contexto.paAdicionarTraySubTraySvcBaseMen_TAR(trayectoMensajeria.IdListaPrecio
                          , ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, pt.SubTrayecto.IdTipoSubTrayecto)
                          , pt.ValorFijadoKiloAdicional
                          , ControllerContext.Current.Usuario
                          , pt.PorcentajeIncremento);
                    }
                });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un precio trayecto
        /// </summary>
        /// <param name="trayectoMensajeria">Objeto precio trayecto</param>
        public void EditarTrayectoMensajeria(TATrayectoMensajeriaDC trayectoMensajeria)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int idTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL);
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(trayectoMensajeria.IdServicio, trayectoMensajeria.IdListaPrecio);

                if (idTrayectoSubTrayecto == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_CREADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_NO_CREADO)));
                }

                PrecioTrayecto_TAR precioTrayecto = contexto.PrecioTrayecto_TAR
                  .Where(r => r.PTR_IdPrecioTrayectoSubTrayect == trayectoMensajeria.IdPrecioTrayectoSubTrayecto)
                  .FirstOrDefault();

                if (trayectoMensajeria.ValorFijadoKiloInicial <= 0)
                {
                    trayectoMensajeria.ValorFijadoKiloInicial = trayectoMensajeria.ValorPropuestoKiloInicial;
                }

                precioTrayecto.PTR_IdTrayectoSubTrayecto = idTrayectoSubTrayecto;
                precioTrayecto.PTR_ValorFijo = trayectoMensajeria.ValorFijadoKiloInicial;
                precioTrayecto.PTR_Porcentaje = trayectoMensajeria.PorcentajeIncremento;

                if (trayectoMensajeria.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA)
                    contexto.paActualizarPreciosTrayectosBaseMensajeria_TAR(trayectoMensajeria.IdPrecioTrayectoSubTrayecto, trayectoMensajeria.IdListaPrecio, ControllerContext.Current.Usuario);

                trayectoMensajeria.PrecioTrayecto.ToList().ForEach(pt =>
                {
                    if (pt.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                    {
                        if (pt.ValorFijadoKiloAdicional <= 0)
                        {
                            pt.ValorFijadoKiloAdicional = pt.ValorPropuestoKiloAdicional;
                        }

                        if (trayectoMensajeria.IdServicio != TAConstantesServicios.SERVICIO_MENSAJERIA)
                        {
                            PrecioTrayecto_TAR nuevoPrecioTrayecto = new PrecioTrayecto_TAR()
                            {
                                PTR_IdListaPrecioServicio = int.Parse(idListaPrecioServicio),
                                PTR_IdTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, pt.SubTrayecto.IdTipoSubTrayecto),
                                PTR_ValorFijo = pt.ValorFijadoKiloAdicional,
                                PTR_Porcentaje = pt.PorcentajeIncremento,
                                PTR_FechaGrabacion = DateTime.Now,
                                PTR_CreadoPor = ControllerContext.Current.Usuario
                            };
                            contexto.PrecioTrayecto_TAR.Add(nuevoPrecioTrayecto);
                        }
                        else
                        {
                            contexto.paAdicionarTraySubTraySvcBaseMen_TAR(trayectoMensajeria.IdListaPrecio
                              , ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, pt.SubTrayecto.IdTipoSubTrayecto)
                              , pt.ValorFijadoKiloAdicional
                              , ControllerContext.Current.Usuario
                              , pt.PorcentajeIncremento);
                        }
                    }
                    else if (pt.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                    {
                        PrecioTrayecto_TAR cambiaPrecioTrayecto = contexto.PrecioTrayecto_TAR
                           .Where(cpt => cpt.PTR_IdPrecioTrayectoSubTrayect == pt.IdPrecioTrayectoSubTrayecto)
                           .FirstOrDefault();

                        if (pt.ValorFijadoKiloAdicional <= 0)
                            pt.ValorFijadoKiloAdicional = pt.ValorPropuestoKiloAdicional;

                        cambiaPrecioTrayecto.PTR_IdTrayectoSubTrayecto = ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, pt.SubTrayecto.IdTipoSubTrayecto);
                        cambiaPrecioTrayecto.PTR_ValorFijo = pt.ValorFijadoKiloAdicional;
                        cambiaPrecioTrayecto.PTR_Porcentaje = pt.PorcentajeIncremento;

                        if (trayectoMensajeria.IdServicio == TAConstantesServicios.SERVICIO_MENSAJERIA)
                            contexto.paActualizarPreciosTrayectosBaseMensajeria_TAR(pt.IdPrecioTrayectoSubTrayecto, trayectoMensajeria.IdListaPrecio, ControllerContext.Current.Usuario);
                    }
                    else if (pt.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                    {
                        int idTrayectoSubTrayectoBorrado = ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, pt.SubTrayecto.IdTipoSubTrayecto);

                        PrecioTrayecto_TAR eliminaPrecioTrayecto = contexto.PrecioTrayecto_TAR
                           .Where(ept => ept.PTR_IdPrecioTrayectoSubTrayect == pt.IdPrecioTrayectoSubTrayecto)
                           .FirstOrDefault();
                        if (trayectoMensajeria.IdServicio != TAConstantesServicios.SERVICIO_MENSAJERIA)
                            contexto.PrecioTrayecto_TAR.Remove(eliminaPrecioTrayecto);
                        else
                        {
                            if (eliminaPrecioTrayecto != null)
                            {
                                contexto.paEliminarTraySubTraySvcBaseMen_TAR(trayectoMensajeria.IdListaPrecio, idTrayectoSubTrayectoBorrado, ControllerContext.Current.Usuario);
                            }
                        }
                    }
                });

                TARepositorioAudit.MapearPrecioTrayecto(contexto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un precio trayecto
        /// </summary>
        /// <param name="trayectoMensajeria">Obbjeto precio trayecto</param>
        public void EliminarTrayectoMensajeria(TATrayectoMensajeriaDC trayectoMensajeria)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                trayectoMensajeria.PrecioTrayecto.ToList().ForEach(f =>
                {
                    if (trayectoMensajeria.IdServicio != TAConstantesServicios.SERVICIO_MENSAJERIA)
                    {
                        PrecioTrayecto_TAR precioTrayecto = contexto.PrecioTrayecto_TAR
                          .Where(r => r.PTR_IdPrecioTrayectoSubTrayect == f.IdPrecioTrayectoSubTrayecto)
                          .FirstOrDefault();

                        contexto.PrecioTrayecto_TAR.Remove(precioTrayecto);
                        TARepositorioAudit.MapearPrecioTrayecto(contexto);
                        contexto.SaveChanges();
                    }
                    else
                    {
                        contexto.paEliminarTraySubTraySvcBaseMen_TAR(trayectoMensajeria.IdListaPrecio, ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, f.SubTrayecto.IdTipoSubTrayecto), ControllerContext.Current.Usuario);
                    }
                });

                if (trayectoMensajeria.IdServicio != TAConstantesServicios.SERVICIO_MENSAJERIA)
                {
                    PrecioTrayecto_TAR eliminaPrecioTrayecto = contexto.PrecioTrayecto_TAR
                      .Where(p => p.PTR_IdPrecioTrayectoSubTrayect == trayectoMensajeria.IdPrecioTrayectoSubTrayecto)
                      .FirstOrDefault();

                    contexto.PrecioTrayecto_TAR.Remove(eliminaPrecioTrayecto);

                    TARepositorioAudit.MapearPrecioTrayecto(contexto);
                    contexto.SaveChanges();
                }
                else
                {
                    contexto.paEliminarTraySubTraySvcBaseMen_TAR(trayectoMensajeria.IdListaPrecio, ObtenerIdentificadorTrayectoSubtrayecto(trayectoMensajeria.Trayecto.IdTipoTrayecto, TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL), ControllerContext.Current.Usuario);
                }
            }
        }

        /// <summary>
        /// Obtiene los trayectos que tienen asignados el subtrayecto kilo adicional
        /// </summary>
        /// <returns>Colección</returns>
        public IEnumerable<TATipoTrayecto> ObtenerTiposTrayectoGeneralMensajeria()
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TrayectoSubTrayecto_TAR.Include("TipoTrayecto_TAR").Where(r => r.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ConvertAll(t => new TATipoTrayecto()
                  {
                      IdTipoTrayecto = t.TRS_IdTipoTrayecto,
                      Descripcion = t.TipoTrayecto_TAR.TTR_Descripcion
                  });
            }
        }

        #endregion UserControl Precio Trayecto Mensajería

        #region UserControl Servicio Valor Adicional

        /// <summary>
        /// Obtiene el valor de los tipos de valor adicional para una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecio">Identificador Lista de Precio</param>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección con los valores adicionales</returns>
        public IEnumerable<TAValorAdicionalValorDC> ObtenerValoresAdicionalesServicio(int idListaPrecio, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
                int idLp = int.Parse(idListaPrecioServicio);

                return contexto.PrecioValorAdicional_VTAR
                  .Where(w => w.PVA_IdPrecioServicio == idLp)
                  .ToList()
                  .ConvertAll(r => new TAValorAdicionalValorDC()
                  {
                      IdTipoValorAdicional = r.PVA_IdTipoValorAdicional,
                      Descripcion = r.TVA_Descripcion,
                      Valor = r.PVA_Valor,
                      EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                      EsEmbalaje = r.TVA_EsEmbalaje
                  });
            }
        }

        /// <summary>
        /// Adiciona precio valor adicional
        /// </summary>
        /// <param name="valorAdicional">Objeto valor adicional</param>
        public void AdicionarValorAdicionalServicio(TAValorAdicionalValorDC precioValorAdicional, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioValorAdicional_TAR precio = new PrecioValorAdicional_TAR()
                {
                    PVA_IdPrecioServicio = idListaPrecioServicio,
                    PVA_IdTipoValorAdicional = precioValorAdicional.IdTipoValorAdicional,
                    PVA_Valor = precioValorAdicional.Valor,
                    PVA_FechaGrabacion = DateTime.Now,
                    PVA_CreadoPor = ControllerContext.Current.Usuario
                };

                contexto.PrecioValorAdicional_TAR.Add(precio);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Edita un precio valor adicional
        /// </summary>
        public void EditarValorAdicionalServicio(TAValorAdicionalValorDC precioValorAdicional, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioValorAdicional_TAR consulta = contexto.PrecioValorAdicional_TAR
                  .Where(r => r.PVA_IdPrecioServicio == idListaPrecioServicio && r.PVA_IdTipoValorAdicional == precioValorAdicional.IdTipoValorAdicional)
                  .FirstOrDefault();

                if (consulta == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    consulta.PVA_Valor = precioValorAdicional.Valor;
                    TARepositorioAudit.MapearPrecioValorAdicional(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Elimina un precio valor adicional
        /// </summary>
        /// <param name="precioValorAdicional">Objeto precio valor adicional</param>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        public void EliminarValorAdicionalServicio(TAValorAdicionalValorDC precioValorAdicional, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PrecioValorAdicional_TAR consulta = contexto.PrecioValorAdicional_TAR
                  .Where(r => r.PVA_IdPrecioServicio == idListaPrecioServicio && r.PVA_IdTipoValorAdicional == precioValorAdicional.IdTipoValorAdicional)
                  .FirstOrDefault();

                if (consulta == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.TARIFAS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }
                else
                {
                    contexto.PrecioValorAdicional_TAR.Remove(consulta);
                    TARepositorioAudit.MapearPrecioValorAdicional(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion UserControl Servicio Valor Adicional

        #region Precio tipo de entrega

        /// <summary>
        /// Obtiene los precios de tipo por tipo de entrega por lista de precios
        /// </summary>
        /// <returns>lista de precios de tipo entrega por lista de precios</returns>
        public List<TAPrecioTipoEntrega> ObtenerPrecioTipoEntrega(int idServicio, int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PrecioServicioTipoEntrega_VTAR.Where(p => p.PTE_IdListaPrecioServicio == idListaPrecioServicio).ToList().
                    ConvertAll<TAPrecioTipoEntrega>(p =>
                    {
                        TAPrecioTipoEntrega precio = new TAPrecioTipoEntrega()
                        {
                            DescripcionTipoEntrega = p.TIE_Descripcion,
                            IdListaPrecioServicio = p.PTE_IdListaPrecioServicio,
                            IdPrecioTipoEntrega = p.PTE_IdPrecioTipoEntrega,
                            ValorKiloInicial = p.PTE_ValorKiloInicial,
                            ValorKiloAdicional = p.PTE_ValorKiloAdicional,
                            IdListaPrecio = idListaPrecioServicio,
                            IdServicio = idServicio,
                            IdTipoEntrega = p.PTE_IdTipoEntrega.Trim(),
                            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                            TipoEntrega = new ADTipoEntrega()
                            {
                                Descripcion = p.TIE_Descripcion,
                                Id = p.PTE_IdTipoEntrega.Trim()
                            }
                        };
                        if (idServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                        {
                            PrecioTipoEntRango_TAR precioTpRan = contexto.PrecioTipoEntRango_TAR
                         .Where(r => r.PTR_IdPrecioTipoEntrega == precio.IdPrecioTipoEntrega)
                         .FirstOrDefault();
                            if (precioTpRan != null)
                            {
                                precio.PesoFinal = precioTpRan.PTR_Final;
                                precio.PesoInicial = precioTpRan.PTR_Inicial;
                            }
                        }
                        return precio;
                    });
            }
        }

        /// <summary>
        /// Adiciona un precio por tipo de entrega
        /// </summary>
        public void AdicionarPrecioTipoEntrega(TAPrecioTipoEntrega precioTipoEntrega)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(precioTipoEntrega.IdServicio, precioTipoEntrega.IdListaPrecio);
                    int idLp = int.Parse(idListaPrecioServicio);

                    if ((contexto.PrecioTipoEntrega_TAR.Where(r => r.PTE_IdListaPrecioServicio == idLp && r.PTE_IdTipoEntrega == precioTipoEntrega.IdTipoEntrega).FirstOrDefault()) == null)
                    {
                        PrecioTipoEntrega_TAR precioTP = new PrecioTipoEntrega_TAR()
                        {
                            PTE_FechaGrabacion = DateTime.Now,
                            PTE_CreadoPor = ControllerContext.Current.Usuario,
                            PTE_IdListaPrecioServicio = idLp,
                            PTE_IdTipoEntrega = precioTipoEntrega.IdTipoEntrega,
                            PTE_ValorKiloAdicional = precioTipoEntrega.ValorKiloAdicional,
                            PTE_ValorKiloInicial = precioTipoEntrega.ValorKiloInicial
                        };

                        PrecioTipoEntrega_TAR insertado = contexto.PrecioTipoEntrega_TAR.Add(precioTP);

                        if (precioTipoEntrega.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                        {
                            precioTP.PrecioTipoEntRango_TAR = new PrecioTipoEntRango_TAR()
                            {
                                PTR_CreadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaGrabacion = DateTime.Now,
                                PTR_Final = precioTipoEntrega.PesoFinal,
                                PTR_Inicial = precioTipoEntrega.PesoInicial,
                                PTR_IdPrecioTipoEntrega = insertado.PTE_IdPrecioTipoEntrega
                            };
                        }

                        contexto.SaveChanges();
                    }
                    transaccion.Complete();
                }
            }
        }

        /// <summary>
        /// Edita un precio por tipo de entrega
        /// </summary>
        public void EditarPrecioTipoEntrega(TAPrecioTipoEntrega precioTipoEntrega)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    PrecioTipoEntrega_TAR precioTp = contexto.PrecioTipoEntrega_TAR
                      .Where(r => r.PTE_IdPrecioTipoEntrega == precioTipoEntrega.IdPrecioTipoEntrega)
                      .FirstOrDefault();

                    if (precioTp != null)
                    {
                        precioTp.PTE_IdTipoEntrega = precioTipoEntrega.IdTipoEntrega;
                        precioTp.PTE_ValorKiloAdicional = precioTipoEntrega.ValorKiloAdicional;
                        precioTp.PTE_ValorKiloInicial = precioTipoEntrega.ValorKiloInicial;

                        if (precioTipoEntrega.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                        {
                            PrecioTipoEntRango_TAR precioExRan = contexto.PrecioTipoEntRango_TAR
                         .Where(r => r.PTR_IdPrecioTipoEntrega == precioTipoEntrega.IdPrecioTipoEntrega)
                         .FirstOrDefault();

                            if (precioExRan != null)
                            {
                                precioExRan.PTR_Final = precioTipoEntrega.PesoFinal;
                                precioExRan.PTR_Inicial = precioTipoEntrega.PesoInicial;
                            }
                            else
                            {
                                precioTp.PrecioTipoEntRango_TAR = new PrecioTipoEntRango_TAR()
                                {
                                    PTR_CreadoPor = ControllerContext.Current.Usuario,
                                    PTR_FechaGrabacion = DateTime.Now,
                                    PTR_Final = precioTipoEntrega.PesoFinal,
                                    PTR_Inicial = precioTipoEntrega.PesoInicial,
                                    PTR_IdPrecioTipoEntrega = precioTipoEntrega.IdPrecioTipoEntrega
                                };
                            }
                        }

                        contexto.SaveChanges();
                    }
                    transaccion.Complete();
                }
            }
        }

        /// <summary>
        /// Elimina un precio por tipo de entrega
        /// </summary>
        public void EliminarTipoEntrega(TAPrecioTipoEntrega precioTipoEntrega)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (precioTipoEntrega.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                    {
                        PrecioTipoEntRango_TAR precioTpRan = contexto.PrecioTipoEntRango_TAR
                     .Where(r => r.PTR_IdPrecioTipoEntrega == precioTipoEntrega.IdPrecioTipoEntrega)
                     .FirstOrDefault();

                        if (precioTpRan != null)
                        {
                            contexto.PrecioTipoEntRango_TAR.Remove(precioTpRan);
                        }
                    }

                    PrecioTipoEntrega_TAR precioTP = contexto.PrecioTipoEntrega_TAR
                        .Where(r => r.PTE_IdPrecioTipoEntrega == precioTipoEntrega.IdPrecioTipoEntrega)
                        .FirstOrDefault();
                    if (precioTP != null)
                    {
                        contexto.PrecioTipoEntrega_TAR.Remove(precioTP);
                        contexto.SaveChanges();
                    }
                    transaccion.Complete();
                }
            }
        }

        #endregion Precio tipo de entrega

        #region UserControl Excepciones por Ciudad de Origen

        /// <summary>
        /// Obtiene las excepciones por ciudad de origen
        /// </summary>
        public IEnumerable<TAPrecioExcepcionDC> ObtenerExcepcionesPorOrigen(int idServicio, int idListaPrecio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(idServicio, idListaPrecio);
                int idLp = int.Parse(idListaPrecioServicio);

                var f = contexto.PrecioServicioExcepcionTrayecto_VTAR
                  .Where(w => w.SET_IdListaPrecioServicio == idLp)
                  .ToList()
                  .ConvertAll(r =>
                  {
                      TAPrecioExcepcionDC precio = new TAPrecioExcepcionDC()
                      {
                          IdPrecioExcepcionTrayecto = r.SET_IdPrecioServicioExcepionTrayecto,
                          CiudadDestino = new PALocalidadDC() { IdLocalidad = r.SET_IdLocalidadDestino, Nombre = r.NombreLocalidadDestino },
                          CiudadOrigen = new PALocalidadDC() { IdLocalidad = r.SET_IdLocalidadOrigen, Nombre = r.NombreLocalidadOrigen },
                          ValorKiloInicial = r.SET_ValorKiloInicial,
                          ValorKiloAdicional = r.SET_ValorKiloAdicional,
                          EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                          EsDestinoTodoElPais = r.SET_EsDestinoTodoElPais,
                          EsOrigenTodoElPais = r.SET_EsOrigenTodoElPais
                      };

                      if (idServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                      {
                          PrecioSvExcepTrayRango_TAR precioExRan = contexto.PrecioSvExcepTrayRango_TAR
                       .Where(p => p.PTR_IdPrecioSvcExcTrayecto == precio.IdPrecioExcepcionTrayecto)
                       .FirstOrDefault();
                          if (precioExRan != null)
                          {
                              precio.PesoFinal = precioExRan.PTR_Final;
                              precio.PesoInicial = precioExRan.PTR_Inicial;
                          }
                      }

                      return precio;
                  }
                    );

                return f;
            }
        }

        /// <summary>
        /// Adiciona una excepción por ciudad de origen
        /// </summary>
        public void AdicionarExcepcionPorOrigen(TAPrecioExcepcionDC precioExcepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    string idListaPrecioServicio = ObtenerIdentificadorListaPrecioServicio(precioExcepcion.IdServicio, precioExcepcion.IdListaPrecio);
                    int idLp = int.Parse(idListaPrecioServicio);

                    if ((contexto.PrecioServicioExcepcionTrayecto_TAR.Where(r => r.SET_IdLocalidadDestino == precioExcepcion.CiudadDestino.IdLocalidad
                      && r.SET_IdLocalidadOrigen == precioExcepcion.CiudadOrigen.IdLocalidad && r.SET_IdListaPrecioServicio == idLp).FirstOrDefault()) == null)
                    {
                        PrecioServicioExcepcionTrayecto_TAR precioEx = new PrecioServicioExcepcionTrayecto_TAR()
                        {
                            SET_IdListaPrecioServicio = idLp,
                            SET_IdLocalidadOrigen = precioExcepcion.CiudadOrigen.IdLocalidad,
                            SET_IdLocalidadDestino = precioExcepcion.CiudadDestino.IdLocalidad,
                            SET_ValorKiloAdicional = precioExcepcion.ValorKiloAdicional,
                            SET_ValorKiloInicial = precioExcepcion.ValorKiloInicial,
                            SET_FechaGrabacion = DateTime.Now,
                            SET_CreadoPor = ControllerContext.Current.Usuario,
                            SET_EsDestinoTodoElPais = precioExcepcion.EsDestinoTodoElPais,
                            SET_EsOrigenTodoElPais = precioExcepcion.EsOrigenTodoElPais
                        };

                        PrecioServicioExcepcionTrayecto_TAR insertado = contexto.PrecioServicioExcepcionTrayecto_TAR.Add(precioEx);

                        if (precioExcepcion.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                        {
                            precioEx.PrecioSvExcepTrayRango_TAR = new PrecioSvExcepTrayRango_TAR()
                            {
                                PTR_CreadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaGrabacion = DateTime.Now,
                                PTR_Final = precioExcepcion.PesoFinal,
                                PTR_Inicial = precioExcepcion.PesoInicial,
                                PTR_IdPrecioSvcExcTrayecto = insertado.SET_IdPrecioServicioExcepionTrayecto
                            };
                        }

                        contexto.SaveChanges();
                    }
                    transaccion.Complete();
                }
            }
        }

        /// <summary>
        /// Edita una excepción por ciudad de origen
        /// </summary>
        public void EditarExcepcionPorOrigen(TAPrecioExcepcionDC precioExcepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    PrecioServicioExcepcionTrayecto_TAR precioEx = contexto.PrecioServicioExcepcionTrayecto_TAR
                      .Where(r => r.SET_IdPrecioServicioExcepionTrayecto == precioExcepcion.IdPrecioExcepcionTrayecto)
                      .FirstOrDefault();

                    precioEx.SET_IdLocalidadOrigen = precioExcepcion.CiudadOrigen.IdLocalidad;
                    precioEx.SET_IdLocalidadDestino = precioExcepcion.CiudadDestino.IdLocalidad;
                    precioEx.SET_ValorKiloInicial = precioExcepcion.ValorKiloInicial;
                    precioEx.SET_ValorKiloAdicional = precioExcepcion.ValorKiloAdicional;
                    precioEx.SET_EsDestinoTodoElPais = precioExcepcion.EsDestinoTodoElPais;
                    precioEx.SET_EsOrigenTodoElPais = precioExcepcion.EsOrigenTodoElPais;
                    TARepositorioAudit.MapearAuditPrecioExcepcion(contexto);

                    if (precioExcepcion.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                    {
                        PrecioSvExcepTrayRango_TAR precioExRan = contexto.PrecioSvExcepTrayRango_TAR
                     .Where(r => r.PTR_IdPrecioSvcExcTrayecto == precioExcepcion.IdPrecioExcepcionTrayecto)
                     .FirstOrDefault();

                        if (precioExRan != null)
                        {
                            precioExRan.PTR_Final = precioExcepcion.PesoFinal;
                            precioExRan.PTR_Inicial = precioExcepcion.PesoInicial;

                            PrecioSvExcTrayRangoHist_TAR precio = new PrecioSvExcTrayRangoHist_TAR()
                            {
                                PTR_CreadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaGrabacion = DateTime.Now,
                                PTR_Final = precioExcepcion.PesoFinal,
                                PTR_Inicial = precioExcepcion.PesoInicial,
                                PTR_IdPrecioSvcExcTrayecto = precioExcepcion.IdPrecioExcepcionTrayecto,
                                PTR_CambiadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaCambio = DateTime.Now,
                                PTR_IdPrecioExcepcionRango = precioExcepcion.IdPrecioExcepcionTrayecto,
                                PTR_TipoCambio = "Modified"
                            };
                            contexto.PrecioSvExcTrayRangoHist_TAR.Add(precio);
                        }
                        else
                        {
                            precioEx.PrecioSvExcepTrayRango_TAR = new PrecioSvExcepTrayRango_TAR()
                            {
                                PTR_CreadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaGrabacion = DateTime.Now,
                                PTR_Final = precioExcepcion.PesoFinal,
                                PTR_Inicial = precioExcepcion.PesoInicial,
                                PTR_IdPrecioSvcExcTrayecto = precioExcepcion.IdPrecioExcepcionTrayecto
                            };
                        }
                    }
                    contexto.SaveChanges();

                    transaccion.Complete();
                }
            }
        }

        /// <summary>
        /// Elimina una excepción por ciudad de origen
        /// </summary>
        public void EliminarExcepcionPorOrigen(TAPrecioExcepcionDC precioExcepcion)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (precioExcepcion.IdServicio == TAConstantesServicios.SERVICIO_RAPI_CARGA)
                    {
                        PrecioSvExcepTrayRango_TAR precioExRan = contexto.PrecioSvExcepTrayRango_TAR
                     .Where(r => r.PTR_IdPrecioSvcExcTrayecto == precioExcepcion.IdPrecioExcepcionTrayecto)
                     .FirstOrDefault();

                        if (precioExRan != null)
                        {
                            contexto.PrecioSvExcepTrayRango_TAR.Remove(precioExRan);

                            contexto.PrecioSvExcTrayRangoHist_TAR.Add(new PrecioSvExcTrayRangoHist_TAR()
                            {
                                PTR_CreadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaGrabacion = DateTime.Now,
                                PTR_Final = precioExcepcion.PesoFinal,
                                PTR_Inicial = precioExcepcion.PesoInicial,
                                PTR_IdPrecioSvcExcTrayecto = precioExcepcion.IdPrecioExcepcionTrayecto,
                                PTR_CambiadoPor = ControllerContext.Current.Usuario,
                                PTR_FechaCambio = DateTime.Now,
                                PTR_IdPrecioExcepcionRango = precioExcepcion.IdPrecioExcepcionTrayecto,
                                PTR_TipoCambio = "Deleted"
                            });
                        }
                    }

                    PrecioServicioExcepcionTrayecto_TAR precioEx = contexto.PrecioServicioExcepcionTrayecto_TAR
                        .Where(r => r.SET_IdPrecioServicioExcepionTrayecto == precioExcepcion.IdPrecioExcepcionTrayecto)
                        .FirstOrDefault();

                    contexto.PrecioServicioExcepcionTrayecto_TAR.Remove(precioEx);
                    TARepositorioAudit.MapearAuditPrecioExcepcion(contexto);

                    contexto.SaveChanges();
                    transaccion.Complete();
                }
            }
        }

        #endregion UserControl Excepciones por Ciudad de Origen

        #region Precio

        /// <summary>
        /// Retorna los impuestos asignados a un servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Colección de servicios</returns>
        public IEnumerable<TAImpuestosDC> ObtenerValorImpuestosServicio(int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ServicioImpuestos_VTAR
                  .Where(r => r.SEI_IdServicio == idServicio)
                  .ToList()
                  .ConvertAll(i => new TAImpuestosDC()
                  {
                      Identificador = i.SEI_IdImpuesto,
                      Descripcion = i.IMP_Descripcion,
                      Valor = i.IMP_Valor
                  });
            }
        }

        /// <summary>
        /// Obtiene los valores adicionales de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección de valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValorValoresAdicionalesServicio(int idServicio)
        {
            //using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.PrecioValorAdicional_VTAR
            //      .Where(r => r.TVA_IdServicio == idServicio)
            //      .ToList()
            //      .ConvertAll(va => new TAValorAdicional()
            //      {
            //          IdTipoValorAdicional = va.PVA_IdTipoValorAdicional,
            //          Descripcion = va.TVA_Descripcion,
            //          PrecioValorAdicional = va.PVA_Valor
            //      });
            //}
            List<TAValorAdicional> lstValoresAdicionales = new List<TAValorAdicional>();
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = new SqlCommand("paObtenerValorValoresAdicionalesServicio_TAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();
                var valorAdicional = dt.AsEnumerable().ToList();

                if (valorAdicional != null)
                {
                    lstValoresAdicionales = valorAdicional.ConvertAll<TAValorAdicional>(valor =>
                    {
                        TAValorAdicional v = new TAValorAdicional()
                        {
                            IdTipoValorAdicional = valor.Field<string>("PVA_IdTipoValorAdicional"),
                            Descripcion = valor.Field<string>("TVA_Descripcion"),
                            PrecioValorAdicional = valor.Field<decimal>("PVA_Valor")
                        };
                        return v;
                    });
                }
            }
            return lstValoresAdicionales;
        }

        /// <summary>
        /// Obtiene los valores de centros de correspondencia
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <returns></returns>
        public IEnumerable<TAServicioCentroDeCorrespondenciaDC> ObtenerPrecioCentroCorrespondencia(int idListaPrecioServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paPrecioCtrCorrespondencia_TAR(idListaPrecioServicio)
                  .ToList()
                  .ConvertAll(r => new TAServicioCentroDeCorrespondenciaDC()
                  {
                      IdServCentroCorrespondencia = r.SCC_IdServCentroCorrespondencia,
                      Descripcion = r.SCC_Descripcion,
                      Valor = r.POP_Valor
                  });
            }
        }

        /// <summary>
        /// Obtiene el precio del servicio internacional
        /// </summary>
        /// <param name="tipoEmpaque">Identificador tipo empaque</param>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Precio Internacional</returns>
        public decimal ObtenerPrecioInternacional(int tipoEmpaque, int idListaPrecioServicio, string idLocalidadDestino, decimal peso, string idZona)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var consulta = contexto.paPrecioInternacional_TAR(idListaPrecioServicio, tipoEmpaque, idLocalidadDestino, idZona, peso).ToList();

                if (consulta.Count() == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO)));
                }
                else
                    return consulta.FirstOrDefault().PIN_Valor;
            }
        }

        /// <summary>
        /// Obtiene los precios de un trámite con sus impuestos
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precios</param>
        /// <param name="idTramite">Identificador trámite</param>
        /// <returns>Precio trámite y sus impuestos</returns>
        public TAPrecioTramiteDC ObtenerPrecioTramite(int idListaPrecioServicio, int idTramite)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var precio = contexto.paPrecioTramite_TAR(idListaPrecioServicio, idTramite)
                  .GroupBy(g => g.TRA_IdTramite)
                  .Select(s => new TAPrecioTramiteDC()
                  {
                      Valor = s.First().PRT_Valor,
                      ValorAdicionalLocal = s.First().PRT_ValorAdicionalLocal,
                      ValorAdicionalDocumento = s.First().PRT_ValorAdicionalPorDocumento,
                      ImpuestosTramite = s.ToList().ConvertAll(impuesto => new TAImpuestosDC
                      {
                          Identificador = impuesto.TRI_IdImpuesto ?? 0,
                          Descripcion = impuesto.IMP_Descripcion,
                          Valor = impuesto.IMP_Valor ?? 0
                      })
                  });

                if (precio == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO)));
                else
                    return precio.First();
            }
        }

        /// <summary>
        /// Retorna el valor de mensajeria
        /// </summary>
        /// <returns></returns>
        public TAPrecioMensajeriaDC ObtenerPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TAPrecioMensajeriaDC precio = new TAPrecioMensajeriaDC();

                var precioTrayecto = contexto.paPrecioTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio)
                         .ToList();

                ///Obtiene las excepciones del trayecto
                var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();

                if (precioTrayecto.Count() == 0)
                    if (excepciones != null)
                        precioTrayecto.Add(new paPrecioTrayectoRS_TAR() { TRS_IdTipoSubTrayecto = TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL });
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_PARA_TRAYECTO)));

                if ((precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL).Count()) == 0)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_KILO_INICIAL_NO_CONFIGURADO)));

                precio.ValorKiloInicial = precioTrayecto.Where(pt => pt.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .FirstOrDefault()
                  .PTR_ValorFijo;

                precio.ValorKiloAdicional = 0;

                precioTrayecto.Where(r => r.TRS_IdTipoSubTrayecto != TAConstantesTarifas.ID_TIPO_SUBTRAYECTO_KILO_INICIAL)
                  .ToList()
                  .ForEach(f =>
                  {
                      precio.ValorKiloAdicional += f.PTR_ValorFijo;
                  });

                bool aplicaTipoEntrega = false;
                if (idTipoEntrega != "-1")
                {
                    var precioTipoEntrega = contexto.paObtenerPrecioTipoEntregaListaPrecios_TAR(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

                    if (precioTipoEntrega != null)
                    {
                        aplicaTipoEntrega = true;
                        precio.ValorKiloInicial = precioTipoEntrega.PTE_ValorKiloInicial;
                        precio.ValorKiloAdicional = precioTipoEntrega.PTE_ValorKiloAdicional;
                    }
                }

                ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
                ///valor del kilo adicional(valor adicional del trayecto)
                if (!aplicaTipoEntrega && excepciones != null)
                {
                    precio.ValorKiloInicial = excepciones.SET_ValorKiloInicial;
                    precio.ValorKiloAdicional = excepciones.SET_ValorKiloAdicional;
                }

                decimal totalAdicional = (peso - TAConstantesTarifas.VALOR_KILO_INICIAL_EXCEPCION_NOTIFICACIONES) * precio.ValorKiloAdicional;
                precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);

                if (esPrimeraAdmision)
                {
                    precio.Valor = (precio.ValorKiloInicial + totalAdicional);
                }
                else
                {
                    precio.Valor = precio.ValorKiloAdicional + totalAdicional;
                }

                return precio;
            }
        }

        /// <summary>
        /// Obtiene el precio de acuerdo a una cantidad
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <param name="peso">Cantidad</param>
        /// <returns>Precio</returns>
        public decimal ObtenerPrecioRapiPromocional(int idListaPrecioServicio, decimal cantidad)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var consulta = contexto.paPrecioRapiPromo_TAR(idListaPrecioServicio, cantidad);

                if (consulta.Count() == 0)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO)));
                else
                    return consulta.FirstOrDefault().PRA_Valor;
            }
        }

        /// <summary>
        /// Calcula precio rapicarga, el calculo del precio se realiza de acuerdo al peso ingresado y los rangos configurados
        /// Si el peso ingresado esta en un valor intermedio se aplica la siguiente formula
        /// valor=(valorRango * pesoRangoFinal) +(kilosAdicionales * valorRango)
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto precio</returns>
        public TAPrecioCargaDC ObtenerPrecioRapiCarga(int idServicio, int idListaPrecio, int idLp, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision, string idTipoEntrega = "-1")
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                bool esMensajeria = false;
                TAPrecioCargaDC precio = new TAPrecioCargaDC();

                var precioTrayectoRango = contexto.paPrecioTrayectoRango_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idLp).ToList();

                precioTrayectoRango.ForEach(f =>
                {
                    if (f.TRS_IdTipoSubTrayecto == TAConstantesTarifas.ID_TIPO_TRAYECTO_ESPECIAL)
                        esMensajeria = true;
                });

                if (esMensajeria == true)
                {
                    TAPrecioMensajeriaDC precioMensajeria = ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
                    precio.Valor = precioMensajeria.Valor;
                    precio.ValorKiloAdicional = precioMensajeria.ValorKiloAdicional;
                }
                else if (esMensajeria == false)
                {
                    ///Obtiene las excepciones del trayecto
                    var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();
                    if (excepciones != null && precioTrayectoRango.Count() == 0)
                    {
                        precioTrayectoRango.Add(new paPrecioTrayectoRango_TAR_Result());
                    }

                    bool aplicoTipoEntrega = false;
                    if (precioTrayectoRango.Count() > 0)
                    {
                        //Obtiene los precios por tipo de entrega
                        if (idTipoEntrega != "-1")
                        {
                            var precioTipoEntrega = contexto.paObtenerPrecioTipoEntregaListaPrecios_TAR(idListaPrecio, idTipoEntrega, idServicio).FirstOrDefault();

                            if (precioTipoEntrega != null)
                            {
                                aplicoTipoEntrega = true;
                                precioTrayectoRango.ForEach(f =>
                                {
                                    f.PPR_Final = precioTipoEntrega.PTR_Final.Value;
                                    f.PPR_Inicial = precioTipoEntrega.PTR_Inicial.Value;
                                    f.PPR_Valor = precioTipoEntrega.PTE_ValorKiloAdicional;
                                });
                            }
                        }

                        ///Obtiene las excepciones del trayecto
                        //var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecio).FirstOrDefault();

                        ///Si hay excepciones obtiene el valor del kilo inicial(Valor configurado en la excepcion)
                        ///valor del kilo adicional(valor adicional del trayecto)
                        if (!aplicoTipoEntrega && excepciones != null)
                        {
                            precioTrayectoRango.ForEach(f =>
                            {
                                f.PPR_Final = excepciones.PTR_Final.Value;
                                f.PPR_Inicial = excepciones.PTR_Inicial.Value;
                                f.PPR_Valor = excepciones.SET_ValorKiloAdicional;
                            });
                        }

                        precio.ValorPrimaSeguro = ObtenerPrimaSeguro(idListaPrecio, valorDeclarado, idServicio);

                        // Para rapicarga se debe validar una opción que indica si se debe ignorar la prima de seguro sin importar el valor declarado, esto es para ciertos clientes especiales
                        var listaPrecioServicio = contexto.ListaPrecioServicio_TAR.FirstOrDefault(l => l.LPS_IdListaPrecioServicio == idLp);


                        if (precioTrayectoRango.Where(p => p.PPR_Inicial <= peso && p.PPR_Final >= peso).Count() > 0)
                        {
                            var consulta = precioTrayectoRango.Where(p => p.PPR_Inicial <= peso && p.PPR_Final >= peso).FirstOrDefault();
                            long idPrecioTrayectoSubTrayecto = consulta.PTR_IdPrecioTrayectoSubTrayect;
                            var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();
                            precio.Valor = consulta.PPR_Valor * consulta.PPR_Final;
                            precio.ValorKiloAdicional = consulta.PTR_ValorFijo;

                            //precio.ValorServicioRetorno = contexto.paObtenerPreTraSubValAdi_TAR(idPrecioTrayectoSubTrayecto).FirstOrDefault().PTV_Valor;
                        }
                        else if (peso < precioTrayectoRango.OrderBy(o => o.PPR_Inicial).First().PPR_Inicial)
                        {
                            TAPrecioMensajeriaDC precioMensajeria = ObtenerPrecioMensajeria(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision);
                            precio.Valor = precioMensajeria.Valor;
                            precio.ValorKiloAdicional = precioMensajeria.ValorKiloAdicional;
                        }
                        else if (peso > precioTrayectoRango.OrderBy(o => o.PPR_Final).Last().PPR_Final)
                        {
                            var consulta = precioTrayectoRango.OrderBy(o => o.PPR_Final).Last();
                            decimal pesoAdicional = peso - (consulta.PPR_Final);
                            long idPrecioTrayectoSubTrayecto = consulta.PTR_IdPrecioTrayectoSubTrayect;
                            var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();

                            precio.ValorKiloAdicional = consulta.PTR_ValorFijo;
                            precio.Valor = (consulta.PPR_Valor * consulta.PPR_Final) + (pesoAdicional * consulta.PPR_Valor);
                        }
                        else
                        {
                            var rangos = precioTrayectoRango.OrderBy(o => o.PPR_Inicial).ToList();
                            bool calculoTarifa = false;
                            for (int i = 0; i < rangos.Count() - 1; i++)
                            {
                                if (peso > rangos[i].PPR_Final && peso < rangos[i + 1].PPR_Inicial)
                                {
                                    if (!calculoTarifa)
                                    {
                                        decimal pesoAdicional = peso - (rangos[i].PPR_Final);
                                        long idPrecioTrayectoSubTrayecto = rangos[i].PTR_IdPrecioTrayectoSubTrayect;
                                        var valorBase = contexto.PrecioTrayecto_TAR.Where(r => r.PTR_IdPrecioTrayectoSubTrayect == idPrecioTrayectoSubTrayecto).FirstOrDefault();
                                        precio.ValorKiloAdicional = rangos[i].PTR_ValorFijo;
                                        precio.Valor = (rangos[i].PTR_ValorFijo * rangos[i].PPR_Final) + (pesoAdicional * rangos[i].PTR_ValorFijo);
                                        calculoTarifa = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PRECIO_SERVICIO_NO_CONFIGURADO)));
                    }
                }

                return precio;
            }
        }

        /// <summary>
        /// Obtiene el precio contrapago
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <returns>Precio</returns>
        public decimal ObtenerPrecioContrapago(int idListaPrecioServicio, decimal valorContraPago)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var consulta = contexto.paPrecioRango_TAR(idListaPrecioServicio).ToList();
                decimal valor = 0;

                if (consulta.Where(r => r.PRA_Inicial <= valorContraPago && r.PRA_Final >= valorContraPago).Count() > 0)
                {
                    if (consulta.Where(r => r.PRA_Inicial <= valorContraPago && r.PRA_Final >= valorContraPago).FirstOrDefault().PRA_Valor == 0)
                        valor = valorContraPago * (consulta.Where(r => r.PRA_Inicial <= valorContraPago && r.PRA_Final >= valorContraPago).FirstOrDefault().PRA_Porcentaje / 100);
                    else if (consulta.Where(r => r.PRA_Inicial <= valorContraPago && r.PRA_Final >= valorContraPago).FirstOrDefault().PRA_Porcentaje == 0)
                        valor = consulta.Where(r => r.PRA_Inicial <= valorContraPago && r.PRA_Final >= valorContraPago).FirstOrDefault().PRA_Valor;
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_CONTRAPAGO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_CONTRAPAGO)));

                return valor;
            }
        }

        /// <summary>
        /// Indica si un trayecto determinado tiene o no una excepción de precios
        /// </summary>
        /// <param name="idListaPrecios"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool TrayectoTieneExcepcion(int idListaPrecios, string idLocalidadOrigen, string idLocalidadDestino, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ///Obtiene las excepciones del trayecto
                var excepciones = contexto.paObtenerExTrayecto_TAR(idServicio, idLocalidadOrigen, idLocalidadDestino, idListaPrecios).FirstOrDefault();

                if (excepciones != null)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Obtiene la prima de seguro de una lista de precio servicio
        /// </summary>
        /// <param name="idListaPrecioServicio">Identificador lista de precio servicio</param>
        /// <returns>Prima de seguro</returns>
        public decimal ObtenerPrimaSeguro(int idListaPrecio, decimal valorDeclarado, int idServicio)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var consulta = contexto.paObtenerPrimaSeguro_TAR(idListaPrecio, idServicio);

                if (consulta != null)
                {
                    var res = consulta.First();
                    if (res != null)
                    {
                        return (res.LPS_PrimaSeguros / 100) * valorDeclarado;
                    }
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRIMA_SEGURO_PARA_LISTAPRECIO_SERVICIO)));
            }
        }

        /// <summary>
        /// Obtener parámetros para saber si sumar o restar la prima de seguro
        /// </summary>
        /// <param name="idListaPrecio">Identificador lista de precio</param>
        /// <param name="peso">Peso</param>
        /// <param name="valorDeclarado">Valor peso declarado</param>
        /// <returns>True si se suma o false si se resta</returns>
        public bool SumarPrecioPesoValorDeclarado(int idListaPrecio, decimal peso, decimal valorDeclarado, bool esRapiCarga = false)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ValorPesoDeclarado_TAR consulta = contexto.ValorPesoDeclarado_TAR.Where(r => r.VMD_IdListaPrecios == idListaPrecio && r.VMD_PesoInicial <= peso && r.VMD_PesoFinal >= peso).FirstOrDefault();

                if (consulta == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_PESO_FUERA_RANGO)));
                }
                else if (valorDeclarado < consulta.VMD_ValorMinimoDeclarado)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_VALOR_DECLARADO_MENOR_QUE_MINIMO_DECLARADO)));
                }
                else if (valorDeclarado == consulta.VMD_ValorMinimoDeclarado && !esRapiCarga)
                {
                    return false;
                }
                else if (esRapiCarga)
                {
                    return !(valorDeclarado == 300000); // TODO: RON Obtener este valor de un parámetro TopeMinVlrDeclRapiCa
                }
                else
                {
                    return true;
                }
            }
        }

        #endregion Precio

        #region Komprech

        /// <summary>
        /// retorna el numero de Dias de entrega entre bogota y la
        /// localidad de destino para Komprech
        /// </summary>
        /// <param name="idLocalidadDestino">id localidad destino</param>
        /// <returns>numero de horas en entregar</returns>
        public int ObtenerTiempoDiasEntregaKomprech(string idLocalidadDestino)
        {
            using (EntidadesTarifas contexto = new EntidadesTarifas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TiempoEntrega_KOM tiempoEntrega = contexto.TiempoEntrega_KOM.FirstOrDefault(tim => tim.TEK_IdLocalidad == idLocalidadDestino);

                if (tiempoEntrega == null)
                    return 0;
                else
                    return tiempoEntrega.TEK_TiempoEntrega;
            }
        }

        #endregion Komprech


        #region ConsultasAppCliente

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public decimal ConsultarValorComercialPeso(int peso)
        {

            using (SqlConnection sqlConn = new SqlConnection(cadenaTransaccional))
            {

                SqlCommand cmd = new SqlCommand(@"paConsultarValorComercialPorPeso_TAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Peso", peso);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                decimal valor = 0;
                while (reader.Read())
                {
                    valor = Convert.ToDecimal(reader["valorComercial"]);
                }
                sqlConn.Close();
                return valor;
            }


        }


        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos()
        {
            using (SqlConnection sqlConn = new SqlConnection(cadenaTransaccional))
            {

                SqlCommand cmd = new SqlCommand(@"paConsultarServiciosConPesosMinimosMaximos_TAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<TAServicioPesoDC> servicios = new List<TAServicioPesoDC>();



                while (reader.Read())
                {
                    TAServicioPesoDC servicio = new TAServicioPesoDC();

                    servicio.IdServicio = Convert.ToInt32(reader["SER_IdServicio"]);
                    servicio.PesoMaximo = Convert.ToDecimal(reader["SME_PesoMaximo"]);
                    servicio.PesoMinimo = Convert.ToDecimal(reader["SME_PesoMinimo"]);

                    servicios.Insert(0, servicio);
                }
                sqlConn.Close();
                return servicios;
            }
        }


        #endregion

    }
}