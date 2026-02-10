using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Facturacion.Comun;
using CO.Servidor.Facturacion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.Entity.Infrastructure;

namespace CO.Servidor.Facturacion.Datos
{
    public class FARepositorioConsFactura
    {
        #region Atributos

        private static readonly FARepositorioConsFactura instancia = new FARepositorioConsFactura();
        private const string NombreModelo = "ModeloFacturacion";
        private const string ObservacionAnulada = "Factura Anulada";
        private const string Creada = "CRE";
        private const string DescripcionCreada = "CREADA";
        private const string Anulada = "ANU";
        private const string DescripcionAnulada = "ANULADA";
        private const string Aprobada = "FACTURA APROBADA";
        private const string NotaCredito = "CRE";
        private const string NotaDebito = "DEB";
        private const string Automatica = "AUT";
        private const string Total = "TOT";
        private const string Reprogramado = "REPROGRAMADO";
        private const string DescripcionAutomatica = "Automatica";
        private const string DescripcionManual = "Manual";

        #endregion Atributos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase FARepositorio
        /// </summary>
        public static FARepositorioConsFactura Instancia
        {
            get { return FARepositorioConsFactura.instancia; }
        }

        #endregion Propiedades

        #region Métodos públicos

        /// <summary>
        /// Consulta la lista de guias asociadas a una factura de cliente crédito
        /// </summary>
        /// <param name="numeroFactura"></param>
        /// <returns></returns>
        public List<FAOperacionFacturadaDC> ConsultarOperacionesFactura(long numeroFactura)
        {
            List<FAOperacionFacturadaDC> operacionesFactura = new List<FAOperacionFacturadaDC>();
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                // Get the ObjectContext related to this DbContext
                var objectContext = (contexto as IObjectContextAdapter).ObjectContext;

                // Sets the command timeout for all the commands
                objectContext.CommandTimeout = 0;

                operacionesFactura = contexto.DetalleXOperFactura_VFAC.Where(f => f.DOF_NumeroFactura == numeroFactura).ToList().ConvertAll < FAOperacionFacturadaDC>(f => 
                {
                    return new FAOperacionFacturadaDC()
                    {
                        IdServicio = f.DOF_IdServicio,
                        NoFactura = numeroFactura,
                        NombreServicio = f.NombreServicio,
                        Fecha = f.DOF_FechaGrabacion,
                        NoOperacion = f.DOF_NumeroOperacion,
                        RazonSocialCliente = f.REF_RazonSocial,
                        Usuario = f.DOF_CreadoPor,
                        Valor = f.DOF_ValorTotal==null?0:(decimal)f.DOF_ValorTotal
                    };
                });
                return operacionesFactura;
            }
        }

        /// <summary>
        /// Asocia un movimento (guía crédito) a una factura específica de un cliente crédito
        /// </summary>
        /// <param name="operacionFac"></param>
        public void AsociarOperacionAFactura(FAAsociacionGuiasFacturaDC datosAsociacion)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paAsociarGuiasCreditoAFactura(datosAsociacion.FacturaDesde, datosAsociacion.FacturaHasta, datosAsociacion.IdCliente, datosAsociacion.NumeroFactura, ControllerContext.Current.Usuario);                
            }
        }

        /// <summary>
        /// Deasociar un movimiento (guía crédito) de una factura de un cliente crédito
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <param name="numeroFactura"></param>
        public void DesasociarOperacionAFactura(long numeroOperacion, long numeroFactura)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paDesasociarGuiaDeFactura(numeroOperacion, numeroFactura);
                //DetalleXOperFactura_FAC detalleOper = contexto.DetalleXOperFactura_FAC.Where(d => d.DOF_NumeroOperacion == numeroOperacion && d.DOF_NumeroFactura == numeroFactura).FirstOrDefault();

                //if (detalleOper != null)
                //{
                //    contexto.DetalleXOperFactura_FAC.Remove(detalleOper);
                //    contexto.SaveChanges();
                //}
            }
        }

        /// <summary>
        /// Metodo que consulta las facturas existentes en el sistema utilizando unos parámetros de búsqueda
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string fechaInicial;
                string fechaFinal;

                filtro.TryGetValue("Fecha_Inicial", out fechaInicial);
                filtro.TryGetValue("Fecha_Final", out fechaFinal);

                filtro.Remove("Fecha_Inicial");
                filtro.Remove("Fecha_Final");

                if (fechaInicial == null)
                    fechaInicial = DateTime.Now.AddMonths(-1).ToString();
                if (fechaFinal == null)
                    fechaFinal = DateTime.Now.ToString();

                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ResumenFactura_FAC>("REF_FechaGrabacion", fechaInicial, OperadorComparacion.Between, fechaFinal);
                where.Add(lamda, OperadorLogico.And);

                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();

                IEnumerable<FAFacturaClienteAutDC> facturas = contexto.ConsultarResumenFactura_FAC(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
              .ToList().ConvertAll<FAFacturaClienteAutDC>(fac =>
              {
                  Localidad_PAR localidad = contexto.Localidad_PAR.Where(loc => loc.LOC_IdLocalidad == fac.REF_CiudadRadicacion).FirstOrDefault();
                  EstadosFactura_FAC estadoAct = contexto.EstadosFactura_FAC.Where(esta => esta.ESF_NumeroFactura == fac.REF_NumeroFactura).OrderByDescending(esta => esta.ESF_FechaGrabacion).FirstOrDefault();
                  if (estadoAct == null)
                  {
                      estadoAct = new EstadosFactura_FAC() { ESF_Estado = Creada, ESF_DescripcionEstado = DescripcionCreada, ESF_FechaGrabacion = fac.REF_FechaGrabacion };
                  }
                  if (fac.REF_PlazoPago <= 30)
                      fac.REF_PlazoPago = 30;
                  else if (fac.REF_PlazoPago <= 60 && fac.REF_PlazoPago > 30)
                      fac.REF_PlazoPago = 60;
                  else if (fac.REF_PlazoPago > 60)
                      fac.REF_PlazoPago = 90;

                  FAFacturaClienteAutDC factura = new FAFacturaClienteAutDC()
                  {
                      CiudadRadicacion = new PALocalidadDC() { IdLocalidad = localidad.LOC_IdLocalidad, Nombre = localidad.LOC_Nombre },
                      CreadoPor = fac.REF_CreadoPor,
                      DireccionRadicacion = fac.REF_DireccionRadicacion,
                      DirigidoA = fac.REF_DirigidoA,
                      FechaGrabacion = fac.REF_FechaGrabacion,
                      FormaPago = fac.REF_FormaPago,
                      DescFormaPago = fac.REF_FormaPago,
                      IdCliente = fac.REF_IdCliente,
                      IdContrato = fac.REF_IdContrato,
                      IdRacol = fac.REF_IdRacol,
                      ValorDescuentos = fac.REF_ValorDescuentos,
                      ValorImpuestos = fac.REF_ValorImpuestos,
                      ValorNeto = fac.REF_ValorNeto,
                      NombreRacol = fac.REF_NombreRacol,
                      NombreContrato = fac.REF_NombreContrato,
                      NumeroContrato = fac.REF_NumeroContrato,
                      NumeroFactura = fac.REF_NumeroFactura,
                      PlazoPago = (EPlazoPago)fac.REF_PlazoPago,
                      RazonSocial = fac.REF_RazonSocial,
                      TelefonoRadicacion = fac.REF_TelefonoRadicacion,
                      TipoFacturacion = fac.REF_TipoFacturacion == Automatica ? DescripcionAutomatica : DescripcionManual,
                      EstadoActual = estadoAct.ESF_DescripcionEstado,
                      ConceptosFactura = new System.Collections.ObjectModel.ObservableCollection<FAConceptoFacturaDC>(contexto.DetalleXServicioFactura_FAC.Where(det => det.DSF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FAConceptoFacturaDC>(con =>
                        {
                            return new FAConceptoFacturaDC()
                            {
                                Cantidad = con.DSF_Cantidad,
                                DescripcionConcepto = con.DSF_DescripcionConcepto,
                                IdConcepto = con.DSF_IdDetallexServFactura,
                                ValorUnitario = con.DSF_TotalNeto / con.DSF_Cantidad,
                                Manual = con.DSF_ConceptoManual,
                                Sucursal = new Servicios.ContratoDatos.Clientes.CLSucursalContratoDC()
                                {
                                    IdSucursal = con.DSF_IdSucursal,
                                    Nombre = con.DSF_DescripcionSucursal,
                                },
                                ImpuestosConcepto = new System.Collections.ObjectModel.ObservableCollection<FAImpuestoConceptoDC>(contexto.ImpuestosDetalleFactura_FAC.Where(imp => imp.IDF_IdDetalleFactura == con.DSF_IdDetallexServFactura).ToList().ConvertAll<FAImpuestoConceptoDC>(impu =>
                                  {
                                      return new FAImpuestoConceptoDC()
                                      {
                                          BaseCalculo = impu.IDF_BaseCalculo,
                                          ValorPorc = impu.IDF_ValorPorc,
                                          IdImpuesto = impu.IDF_IdImpuesto,
                                          Descripcion = impu.IDF_Descripcion
                                      };
                                  })),
                                LstNotasAsociadasAlConcepto = new ObservableCollection<FANotaFacturaDC>(contexto.NotasFactura_FAC
                                                       .Where(not => not.NOF_IdDetallexServFactura == con.DSF_IdDetallexServFactura)
                                                       .ToList()
                                                       .ConvertAll<FANotaFacturaDC>(nott =>
                                                       {
                                                           return new FANotaFacturaDC()
                                                           {
                                                               idDetalleServicioFact = nott.NOF_IdDetallexServFactura,
                                                               TipoNota = new FATipoNotaDC()
                                                               {
                                                                   IdTipoNota = nott.NOF_TipoNota
                                                               },
                                                               ValorNota = nott.NOF_ValorNota,
                                                               EstadoNota = new FAEstadoNotaDC()
                                                               {
                                                                   Id = nott.NOF_EstadoNota
                                                               },
                                                               Descripcion = new FADescripcionNotaDC()
                                                               {
                                                                   IdDescripcion = nott.NOF_IdDescripcion
                                                               },
                                                               Responsable = new FAResponsableDC()
                                                               {
                                                                   IdResponsable = nott.NOF_IdResponsable
                                                               }
                                                           };
                                                       })),
                            };
                        })),
                      DeduccionesFactura = new System.Collections.ObjectModel.ObservableCollection<FADeduccionFacturaDC>(contexto.DeduccionesFactura_FAC.Where(de => de.DEF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FADeduccionFacturaDC>(ded =>
                        {
                            return new FADeduccionFacturaDC()
                            {
                                BaseCalculo = ded.DEF_BaseCalculo,
                                Descripcion = ded.DEF_Descripcion,
                                IdDeduccion = ded.DEF_IdDeduccion,
                                TarifaPorcentual = ded.DEF_TarifaPorcentual,
                                ValorFijo = ded.DEF_ValorFijo,
                                TotalCalculado = ded.DEF_TotalCalculado
                            };
                        })),
                      DescuentosFactura = new System.Collections.ObjectModel.ObservableCollection<FADescuentoFacturaDC>(contexto.DescuentosFactura_FAC.Where(des => des.DEF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FADescuentoFacturaDC>(desc =>
                        {
                            return new FADescuentoFacturaDC()
                            {
                                IdDescuento = desc.DEF_IdDescuento,
                                Motivo = desc.DEF_Motivo,
                                Valor = desc.DEF_Valor
                            };
                        })),
                      EstadosFactura = new System.Collections.ObjectModel.ObservableCollection<FAEstadoFacturaDC>(contexto.EstadosFactura_FAC.Where(est => est.ESF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FAEstadoFacturaDC>(est =>
                        {
                            return new FAEstadoFacturaDC()
                            {
                                Estado = est.ESF_Estado,
                                DescripcionEstado = est.ESF_DescripcionEstado,
                                FechaGrabacion = est.ESF_FechaGrabacion
                            };
                        })),
                      RetencionesFactura = new System.Collections.ObjectModel.ObservableCollection<FARetencionFacturaDC>(contexto.RetencionesFactura_FAC.Where(ret => ret.REF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FARetencionFacturaDC>(ret =>
                        {
                            return new FARetencionFacturaDC()
                            {
                                BaseCalculo = ret.REF_BaseCalculo,
                                BaseMinima = ret.REF_BaseMinima,
                                Descripcion = ret.REF_Descripcion,
                                IdRetencion = ret.REF_IdRetencion,
                                NumeroFactura = ret.REF_NumeroFactura,
                                Total = ret.REF_ValorLiquidado,
                                ValorFijo = ret.REF_ValorFijo,
                                ValorPorcenctual = ret.REF_TarifaPorcentual,
                                Valorx100 = ret.REF_ValorPorMonto
                            };
                        })),

                      NotasFactura = new System.Collections.ObjectModel.ObservableCollection<FANotaFacturaDC>(contexto.NotasFactura_FAC.Where(not => not.NOF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FANotaFacturaDC>(not =>
                        {
                            return new FANotaFacturaDC()
                            {
                                IdNota = not.NOF_IdNota,
                                TipoNota = new FATipoNotaDC { IdTipoNota = not.NOF_TipoNota, DescripcionTipoNota = not.DescripcionNotasFactura_FAC.DNF_Descripcion },
                                Observaciones = not.NOF_Observaciones,
                                ValorNota = not.NOF_ValorNota,
                                NumeroFactura = not.NOF_NumeroFactura,
                                FechaGrabacion = not.NOF_FechaGrabacion,
                                CreadoPor = not.NOF_CreadoPor
                            };
                        })),
                  };

                  //lleno los Totales de notas creditos y debitos y el total por concepto
                  if (factura.ConceptosFactura != null && factura.ConceptosFactura.Count > 0)
                  {
                      factura.ConceptosFactura.ToList().ForEach(concep =>
                      {
                          if (concep != null)
                          {
                              decimal totalNotasConceptoCredito = 0;
                              decimal totalNotasConceptoDebito = 0;

                              if (concep.LstNotasAsociadasAlConcepto != null && concep.LstNotasAsociadasAlConcepto.Count > 0)
                              {
                                  concep.LstNotasAsociadasAlConcepto.ToList().ForEach(not =>
                                  {
                                      if (not.TipoNota.IdTipoNota == NotaCredito)
                                      {
                                          factura.TotalNotasCredito = factura.TotalNotasCredito + not.ValorNota;
                                          totalNotasConceptoCredito = totalNotasConceptoCredito + not.ValorNota;
                                      }
                                      else
                                      {
                                          factura.TotalNotasDebito = factura.TotalNotasDebito + not.ValorNota;
                                          totalNotasConceptoDebito = totalNotasConceptoDebito + not.ValorNota;
                                      }
                                  });
                              }
                              concep.ValorTotalNotaAsociadaConcepto = totalNotasConceptoCredito - totalNotasConceptoDebito < 0 ?
                                                                            (totalNotasConceptoCredito - totalNotasConceptoDebito) * -1 :
                                                                            totalNotasConceptoCredito - totalNotasConceptoDebito;
                          }
                      });
                  }

                  if (fac.REF_TipoFacturacion == Automatica)
                  {
                      ResumenFacturaAut_FAC facturaAut = contexto.ResumenFacturaAut_FAC.Where(res => res.RFA_NumeroFactura == fac.REF_NumeroFactura).FirstOrDefault();
                      if (facturaAut != null)
                      {
                          factura.IdProgramacion = facturaAut.RFA_IdProgramacion;
                          factura.IdAgrupamientoFactura = facturaAut.RFA_IdAgrupamientoFactura;
                          factura.FechaCorte = facturaAut.RFA_FechaCorte;
                          factura.NombreAgrupamientoFactura = facturaAut.RFA_NombreAgrupamientoFactura;
                          factura.DiaFacturacion = facturaAut.RFA_DiaFacturacion;
                          factura.DescDiaFacturacion = fachadaClientes.TraducirNotacionDiaInter(facturaAut.RFA_DiaFacturacion);
                          factura.DiaPago = facturaAut.RFA_DiaPago;
                          factura.DescDiaPago = fachadaClientes.TraducirNotacionDiaInter(facturaAut.RFA_DiaPago);
                          factura.DiaRadicacion = facturaAut.RFA_DiaRadicacion;
                          factura.DescDiaRadicacion = fachadaClientes.TraducirNotacionDiaInter(facturaAut.RFA_DiaRadicacion);
                          factura.Sucursales = fachadaClientes.ConsultarSucursalesAgrupamientoFactura(facturaAut.RFA_IdAgrupamientoFactura);
                          factura.Servicios = fachadaClientes.ConsultarServiciosAgrupamientoFactura(facturaAut.RFA_IdAgrupamientoFactura);
                          factura.RequisitosFactura = fachadaClientes.ConsultarRequisitosAgrupamientoFactura(facturaAut.RFA_IdAgrupamientoFactura);
                      }
                  }
                  return factura;
              });

                return facturas;
            }
        }

        /// <summary>
        /// Método para consultar la información de una factura
        /// </summary>
        /// <param name="numeroFactura"></param>
        /// <returns></returns>
        public FAFacturaClienteAutDC ConsultarFactura(long numeroFactura)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ICLFachadaClientes fachadaClientes = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>();
                ResumenFactura_FAC fac = contexto.ResumenFactura_FAC.Where(fact => fact.REF_NumeroFactura == numeroFactura).FirstOrDefault();
                Localidad_PAR localidad = contexto.Localidad_PAR.Where(loc => loc.LOC_IdLocalidad == fac.REF_CiudadRadicacion).FirstOrDefault();

                if (fac != null)
                {
                    FAFacturaClienteAutDC factura = new FAFacturaClienteAutDC()
                 {
                     CiudadRadicacion = new PALocalidadDC() { IdLocalidad = localidad.LOC_IdLocalidad, Nombre = localidad.LOC_Nombre },
                     CreadoPor = fac.REF_CreadoPor,
                     DireccionRadicacion = fac.REF_DireccionRadicacion,
                     DirigidoA = fac.REF_DirigidoA,
                     FechaGrabacion = fac.REF_FechaGrabacion,
                     FormaPago = fac.REF_FormaPago,
                     DescFormaPago = fac.REF_FormaPago,
                     IdCliente = fac.REF_IdCliente,
                     IdContrato = fac.REF_IdContrato,
                     IdRacol = fac.REF_IdRacol,
                     NombreRacol = fac.REF_NombreRacol,
                     NombreContrato = fac.REF_NombreContrato,
                     NumeroContrato = fac.REF_NumeroContrato,
                     NumeroFactura = fac.REF_NumeroFactura,
                     PlazoPago = (EPlazoPago)fac.REF_PlazoPago,
                     RazonSocial = fac.REF_RazonSocial,
                     TelefonoRadicacion = fac.REF_TelefonoRadicacion,
                     TipoFacturacion = fac.REF_TipoFacturacion,
                     ValorDescuentos = fac.REF_ValorDescuentos,
                     ValorImpuestos = fac.REF_ValorImpuestos,
                     ValorNeto = fac.REF_ValorNeto,
                     ValorTotalFactura = fac.REF_ValorTotal.Value,
                     ConceptosFactura = new System.Collections.ObjectModel.ObservableCollection<FAConceptoFacturaDC>(contexto.DetalleXServicioFactura_FAC.Where(det => det.DSF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FAConceptoFacturaDC>(con =>
                       {
                           return new FAConceptoFacturaDC()
                           {
                               Cantidad = con.DSF_Cantidad,
                               DescripcionConcepto = con.DSF_DescripcionConcepto,
                               IdConcepto = con.DSF_IdDetallexServFactura,
                               ValorUnitario = con.DSF_TotalNeto / con.DSF_Cantidad,
                               Manual = con.DSF_ConceptoManual,
                               Sucursal = new Servicios.ContratoDatos.Clientes.CLSucursalContratoDC()
                               {
                                   IdSucursal = con.DSF_IdSucursal,
                                   Nombre = con.DSF_DescripcionSucursal,
                               },
                               ImpuestosConcepto = new System.Collections.ObjectModel.ObservableCollection<FAImpuestoConceptoDC>(contexto.ImpuestosDetalleFactura_FAC.Where(imp => imp.IDF_IdDetalleFactura == con.DSF_IdDetallexServFactura).ToList().ConvertAll<FAImpuestoConceptoDC>(impu =>
                                 {
                                     return new FAImpuestoConceptoDC()
                                     {
                                         BaseCalculo = impu.IDF_BaseCalculo,
                                         ValorPorc = impu.IDF_ValorPorc,
                                         IdImpuesto = impu.IDF_IdImpuesto,
                                         Descripcion = impu.IDF_Descripcion
                                     };
                                 })),
                               LstNotasAsociadasAlConcepto = new ObservableCollection<FANotaFacturaDC>(contexto.NotasFactura_FAC
                                                       .Where(not => not.NOF_IdDetallexServFactura == con.DSF_IdDetallexServFactura)
                                                       .ToList()
                                                       .ConvertAll<FANotaFacturaDC>(nott =>
                                                           {
                                                               return new FANotaFacturaDC()
                                                               {
                                                                   idDetalleServicioFact = nott.NOF_IdDetallexServFactura,
                                                                   TipoNota = new FATipoNotaDC()
                                                                   {
                                                                       IdTipoNota = nott.NOF_TipoNota
                                                                   },
                                                                   ValorNota = nott.NOF_ValorNota,
                                                                   EstadoNota = new FAEstadoNotaDC()
                                                                   {
                                                                       Id = nott.NOF_EstadoNota
                                                                   },
                                                                   Descripcion = new FADescripcionNotaDC()
                                                                   {
                                                                       IdDescripcion = nott.NOF_IdDescripcion
                                                                   },
                                                                   Responsable = new FAResponsableDC()
                                                                   {
                                                                       IdResponsable = nott.NOF_IdResponsable
                                                                   }
                                                               };
                                                           })),
                           };
                       })),
                     DeduccionesFactura = new System.Collections.ObjectModel.ObservableCollection<FADeduccionFacturaDC>(contexto.DeduccionesFactura_FAC.Where(de => de.DEF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FADeduccionFacturaDC>(ded =>
                       {
                           return new FADeduccionFacturaDC()
                           {
                               BaseCalculo = ded.DEF_BaseCalculo,
                               Descripcion = ded.DEF_Descripcion,
                               IdDeduccion = ded.DEF_IdDeduccion,
                               TarifaPorcentual = ded.DEF_TarifaPorcentual,
                               ValorFijo = ded.DEF_ValorFijo,
                               TotalCalculado = ded.DEF_TotalCalculado
                           };
                       })),
                     DescuentosFactura = new System.Collections.ObjectModel.ObservableCollection<FADescuentoFacturaDC>(contexto.DescuentosFactura_FAC.Where(des => des.DEF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FADescuentoFacturaDC>(desc =>
                       {
                           return new FADescuentoFacturaDC()
                           {
                               IdDescuento = desc.DEF_IdDescuento,
                               Motivo = desc.DEF_Motivo,
                               Valor = desc.DEF_Valor
                           };
                       })),
                     EstadosFactura = new System.Collections.ObjectModel.ObservableCollection<FAEstadoFacturaDC>(contexto.EstadosFactura_FAC.Where(est => est.ESF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FAEstadoFacturaDC>(est =>
                       {
                           return new FAEstadoFacturaDC()
                           {
                               Estado = est.ESF_Estado,
                               DescripcionEstado = est.ESF_DescripcionEstado,
                               FechaGrabacion = est.ESF_FechaGrabacion
                           };
                       })),
                     RetencionesFactura = new System.Collections.ObjectModel.ObservableCollection<FARetencionFacturaDC>(contexto.RetencionesFactura_FAC.Where(ret => ret.REF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FARetencionFacturaDC>(ret =>
                       {
                           return new FARetencionFacturaDC()
                           {
                               BaseCalculo = ret.REF_BaseCalculo,
                               BaseMinima = ret.REF_BaseMinima,
                               Descripcion = ret.REF_Descripcion,
                               IdRetencion = ret.REF_IdRetencion,
                               NumeroFactura = ret.REF_NumeroFactura,
                               Total = ret.REF_ValorLiquidado,
                               ValorFijo = ret.REF_ValorFijo,
                               ValorPorcenctual = ret.REF_TarifaPorcentual,
                               Valorx100 = ret.REF_ValorPorMonto
                           };
                       })),

                     NotasFactura = new System.Collections.ObjectModel.ObservableCollection<FANotaFacturaDC>(contexto.NotasFactura_FAC.Where(not => not.NOF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FANotaFacturaDC>(not =>
                       {
                           return new FANotaFacturaDC()
                           {
                               IdNota = not.NOF_IdNota,
                               TipoNota = new FATipoNotaDC { IdTipoNota = not.NOF_TipoNota, DescripcionTipoNota = not.DescripcionNotasFactura_FAC.DNF_Descripcion },
                               Observaciones = not.NOF_Observaciones,
                               ValorNota = not.NOF_ValorNota,
                               NumeroFactura = not.NOF_NumeroFactura,
                               FechaGrabacion = not.NOF_FechaGrabacion,
                               CreadoPor = not.NOF_CreadoPor
                           };
                       })),
                 };

                    //lleno los Totales de notas creditos y debitos y el total por concepto
                    if (factura.ConceptosFactura != null && factura.ConceptosFactura.Count > 0)
                    {
                        factura.ConceptosFactura.ToList().ForEach(concep =>
                        {
                            if (concep != null)
                            {
                                decimal totalNotasConceptoCredito = 0;
                                decimal totalNotasConceptoDebito = 0;

                                if (concep.LstNotasAsociadasAlConcepto != null && concep.LstNotasAsociadasAlConcepto.Count > 0)
                                {
                                    concep.LstNotasAsociadasAlConcepto.ToList().ForEach(not =>
                                    {
                                        if (not.TipoNota.IdTipoNota == NotaCredito)
                                        {
                                            factura.TotalNotasCredito = factura.TotalNotasCredito + not.ValorNota;
                                            totalNotasConceptoCredito = totalNotasConceptoCredito + not.ValorNota;
                                        }
                                        else
                                        {
                                            factura.TotalNotasDebito = factura.TotalNotasDebito + not.ValorNota;
                                            totalNotasConceptoDebito = totalNotasConceptoDebito + not.ValorNota;
                                        }
                                    });
                                }
                                concep.ValorTotalNotaAsociadaConcepto = totalNotasConceptoCredito - totalNotasConceptoDebito < 0 ?
                                                                            (totalNotasConceptoCredito - totalNotasConceptoDebito) * -1 :
                                                                            totalNotasConceptoCredito - totalNotasConceptoDebito;
                            }
                        });
                    }

                    return factura;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Consulta la informacion basica de una factura por el numero de Operacion( Numero de giro)
        /// </summary>
        /// <param name="numeroOperacion"></param>
        /// <returns></returns>
        public FAOperacionFacturadaDC ConsultarFacturaPorNumeroOperacion(long numeroOperacion)
        {
            FAOperacionFacturadaDC factura = null;
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DetalleXOperFactura_FAC detalleOperacion = contexto.DetalleXOperFactura_FAC.Include("ResumenFactura_FAC").FirstOrDefault(oper => oper.DOF_NumeroOperacion == numeroOperacion);

                if (detalleOperacion != null)
                {
                    factura = new FAOperacionFacturadaDC()
                    {
                        NoFactura = detalleOperacion.DOF_NumeroFactura,
                        NoOperacion = detalleOperacion.DOF_NumeroOperacion,
                        IdCliente = detalleOperacion.ResumenFactura_FAC.REF_IdCliente,
                        RazonSocialCliente = detalleOperacion.ResumenFactura_FAC.REF_RazonSocial,
                        Valor = detalleOperacion.DOF_ValorTotal.HasValue == true ? detalleOperacion.DOF_ValorTotal.Value : 0,
                        Fecha = detalleOperacion.DOF_FechaOperacion,
                        Usuario = detalleOperacion.DOF_CreadoPor
                    };
                }

                return factura;
            }
        }

        /// <summary>
        /// Anular la factura indicada
        /// </summary>
        /// <param name="numeroFactura"></param>
        public void AnularFactura(FAEstadoFacturaDC estadoFactura)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstadosFactura_FAC estadoAnulado = contexto.EstadosFactura_FAC.Where(est => est.ESF_NumeroFactura == estadoFactura.NumeroFactura && est.ESF_Estado == estadoFactura.Estado).FirstOrDefault();
                FAFacturaClienteAutDC Factura = ConsultarFactura(estadoFactura.NumeroFactura);

                if (estadoAnulado == null)
                {
                    estadoAnulado = new EstadosFactura_FAC()
                  {
                      ESF_CreadoPor = ControllerContext.Current.Usuario,
                      ESF_DescripcionEstado = DescripcionAnulada,
                      ESF_Estado = Anulada,
                      ESF_FechaGrabacion = DateTime.Now,
                      ESF_NumeroFactura = estadoFactura.NumeroFactura,
                      ESF_Observaciones = estadoFactura.Observaciones == null ? string.Empty : estadoFactura.Observaciones
                  };
                    contexto.EstadosFactura_FAC.Add(estadoAnulado);
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_FACTURACION, FAEnumTipoErrorFacturacion.EX_FACTURA_ANULADA.ToString(), FAMensajesFacturacion.CargarMensaje(FAEnumTipoErrorFacturacion.EX_FACTURA_ANULADA)));
                }

                //Genero las Notas Creditos Correspondientes a detalle de la Factura Generado
                if (Factura != null && Factura.ConceptosFactura != null && Factura.ConceptosFactura.Count > 0)
                {
                    Factura.ConceptosFactura.ToList().ForEach(conc =>
                    {
                        //Valido si existen notas anteriores asociadas al concepto
                        if (conc.LstNotasAsociadasAlConcepto != null && conc.LstNotasAsociadasAlConcepto.Count > 0)
                        {
                            //Si existe valido el tipo de nota y el valor para hacer el ajuste
                            //correspondiente
                            decimal totalNotasCreditoXConcepto = 0;
                            decimal totalNotasDebitoXConcepto = 0;
                            decimal valorAjustar;
                            conc.LstNotasAsociadasAlConcepto.ToList().ForEach(nota =>
                            {
                                if (nota.TipoNota.IdTipoNota == NotaCredito)
                                    totalNotasCreditoXConcepto = totalNotasCreditoXConcepto + nota.ValorNota;

                                else
                                    totalNotasDebitoXConcepto = totalNotasDebitoXConcepto + nota.ValorNota;
                            });

                            //sumo las notas debito y resto la credito para el total
                            conc.ValorTotalNotaAsociadaConcepto = totalNotasDebitoXConcepto - totalNotasCreditoXConcepto;

                            //Valido el valor de las notas para hacer un ajuste en caso de ser necesario
                            if (conc.Total > conc.ValorTotalNotaAsociadaConcepto)
                            {
                                //si son mas la notas credito pero inferiores al total(eso se controla en cliente "que la nota credito no sea mayor que el valor
                                //del detalle total")
                                if (conc.ValorTotalNotaAsociadaConcepto < 0)
                                    conc.ValorTotalNotaAsociadaConcepto = conc.ValorTotalNotaAsociadaConcepto * -1;

                                valorAjustar = conc.Total - conc.ValorTotalNotaAsociadaConcepto;

                                //creo la Nota credito ajustando el valor pendiente por concepto
                                AdicionarNotaCreditoPorAnulacion(estadoFactura, conc, valorAjustar);
                            }
                            else
                            {
                                //si son mas la notas debito
                                if (conc.Total < conc.ValorTotalNotaAsociadaConcepto)
                                {
                                    //Sumo las notas debito y el total del detalle para hacer una sola nota
                                    //credito que anule las 2 las debito y el total
                                    valorAjustar = conc.ValorTotalNotaAsociadaConcepto + conc.Total;

                                    //creo la Nota credito correspondiente a cada detalle en tabla
                                    AdicionarNotaCreditoPorAnulacion(estadoFactura, conc, valorAjustar);
                                }
                            }
                        }
                        else
                        {
                            //creo la Nota credito correspondiente a cada detalle en tabla
                            AdicionarNotaCreditoPorAnulacion(estadoFactura, conc, conc.Total);
                        }
                    });
                }

                EstadoFacturaAnulada_FAC estadoAnulada = new EstadoFacturaAnulada_FAC()
                {
                    EFA_Estado = estadoAnulado.ESF_Estado,
                    EFA_IdMotivo = estadoFactura.Motivo.IdMotivo,
                    EFA_IdResponsable = estadoFactura.NotaFactura.Responsable.IdResponsable,
                    EFA_NumeroFactura = estadoFactura.NumeroFactura,
                    EFA_CreadoPor = ControllerContext.Current.Usuario,
                    EFA_FechaGrabacion = DateTime.Now,
                };

                contexto.EstadoFacturaAnulada_FAC.Add(estadoAnulada);

                contexto.paDesmarcarOperaciones_FAC(estadoFactura.NumeroFactura);

                ResumenFactura_FAC resumenFac = contexto.ResumenFactura_FAC.Where(fac => fac.REF_NumeroFactura == estadoFactura.NumeroFactura).FirstOrDefault();
                if (resumenFac != null)
                {
                    resumenFac.ESF_DescEstadoActual = DescripcionAnulada;
                    resumenFac.REF_ValorNeto = 0;
                    resumenFac.REF_ValorTotal = 0;
                    resumenFac.REF_ValorImpuestos = 0;
                    resumenFac.REF_ValorDescuentos = 0;
                    contexto.SaveChanges();
                }

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona una nota credito por cada detalle
        /// cuando se realiza una anulacion de una factura
        /// </summary>
        /// <param name="estadoFactura">Es el estado de la FActira</param>
        /// <param name="conc">concepto detalle de la factura</param>
        private void AdicionarNotaCreditoPorAnulacion(FAEstadoFacturaDC estadoFactura, FAConceptoFacturaDC conc, decimal valorNota)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NotasFactura_FAC notaCredito = new NotasFactura_FAC()
                {
                    NOF_CreadoPor = ControllerContext.Current.Usuario,
                    NOF_FechaGrabacion = DateTime.Now,
                    NOF_NumeroFactura = estadoFactura.NumeroFactura,
                    NOF_Observaciones = ObservacionAnulada,
                    NOF_IdResponsable = estadoFactura.NotaFactura.Responsable.IdResponsable,
                    NOF_IdDescripcion = FAConstantesFacturacion.DESCRIPCION_ANULACION_FACTURA,
                    NOF_TipoNota = NotaCredito,
                    NOF_ValorNota = valorNota,
                    NOF_IdDetallexServFactura = conc.IdConcepto,
                    NOF_EstadoNota = Total,
                };
                contexto.NotasFactura_FAC.Add(notaCredito);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Permite reprogramar una factura anulada para que nuevamente sea generada
        /// </summary>
        /// <param name="FechaNueva"></param>
        public void ReprogramarFactura(DateTime FechaNueva, FAFacturaClienteAutDC factura)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProgramacionFactura_FAC programacion = contexto.ProgramacionFactura_FAC.Where(pro => pro.PRF_IdProgramacion == factura.IdProgramacion).FirstOrDefault();
                if (programacion != null)
                {
                    programacion.PRF_Ejecutado = false;
                    programacion.PRF_FechaProgramacion = FechaNueva;
                    programacion.PRF_UsuarioReprograma = ControllerContext.Current.Usuario;
                    programacion.PRF_Tipo = Reprogramado;
                    programacion.PRF_FechaReprogramacion = DateTime.Now;

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// permite aprobar una factura
        /// </summary>
        /// <param name="estadoFactura"></param>
        public void AprobarFactura(FAEstadoFacturaDC estadoFactura)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                EstadosFactura_FAC estadoAprobado = contexto.EstadosFactura_FAC.Where(est => est.ESF_NumeroFactura == estadoFactura.NumeroFactura && est.ESF_Estado == estadoFactura.Estado).FirstOrDefault();

                if (estadoAprobado == null)
                {
                    estadoAprobado = new EstadosFactura_FAC()
                    {
                        ESF_CreadoPor = ControllerContext.Current.Usuario,
                        ESF_DescripcionEstado = estadoFactura.DescripcionEstado,
                        ESF_Estado = estadoFactura.Estado,
                        ESF_FechaGrabacion = DateTime.Now,
                        ESF_NumeroFactura = estadoFactura.NumeroFactura,
                        ESF_Observaciones = Aprobada
                    };
                    contexto.EstadosFactura_FAC.Add(estadoAprobado);
                    contexto.SaveChanges();
                };
            }
        }

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        public FANotaFacturaDC AgregarNotaFactura(FANotaFacturaDC nota)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NotasFactura_FAC NuevaNota = new NotasFactura_FAC()
                {
                    NOF_CreadoPor = ControllerContext.Current.Usuario,
                    NOF_FechaGrabacion = DateTime.Now,
                    NOF_NumeroFactura = nota.NumeroFactura,
                    NOF_Observaciones = nota.Observaciones == null ? string.Empty : nota.Observaciones,
                    NOF_TipoNota = nota.TipoNota.IdTipoNota,
                    NOF_IdResponsable = nota.Responsable.IdResponsable,
                    NOF_IdDescripcion = nota.Descripcion.IdDescripcion,
                    NOF_ValorNota = nota.ValorNota,
                    NOF_EstadoNota = nota.EstadoNota.Id,
                    NOF_IdDetallexServFactura = nota.idDetalleServicioFact
                };

                contexto.NotasFactura_FAC.Add(NuevaNota);
                contexto.SaveChanges();

                nota.FechaGrabacion = NuevaNota.NOF_FechaGrabacion;
                nota.IdNota = NuevaNota.NOF_IdNota;
                nota.CreadoPor = NuevaNota.NOF_CreadoPor;
            }
            return nota;
        }

        /// <summary>
        /// Agrega una nueva nota crédito o débito a una factura
        /// </summary>
        /// <param name="nota"></param>
        public void EliminarNotaFactura(FANotaFacturaDC nota)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                NotasFactura_FAC notaFactura = contexto.NotasFactura_FAC.Where(no => no.NOF_IdNota == nota.IdNota).FirstOrDefault();
                if (notaFactura != null)
                {
                    contexto.NotasFactura_FAC.Remove(notaFactura);
                    FAFacturacionAudit.MapearAuditBorrarNota(contexto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para obtener los tipos de nota
        /// </summary>
        /// <returns></returns>
        public IList<FATipoNotaDC> ObtenerTiposNota()
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoNota_VFAC
                     .OrderBy(o => o.TIN_NombreTipoNota)
         .ToList()
         .ConvertAll<FATipoNotaDC>
         (r => new FATipoNotaDC()
         {
             IdTipoNota = r.TIN_IdTipoNota,
             DescripcionTipoNota = r.TIN_NombreTipoNota,
         });
            }
        }

        #endregion Métodos públicos

        #region Métodos de Consulta

        /// <summary>
        /// Consultar los motivos causa de una anualción de una factura
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAMotivoAnulacionDC> ConsultarMotivosAnulacion()
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoAnulacionFactura_FAC.ToList().ConvertAll<FAMotivoAnulacionDC>(mo =>
                {
                    return new FAMotivoAnulacionDC()
                    {
                        IdMotivo = mo.MAF_IdMotivo,
                        DescripcionMotivo = mo.MAF_DescripcionMotivo
                    };
                });
            }
        }

        /// <summary>
        /// Método para consultar responsables de facturacion
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAResponsableDC> ConsultarResponsables()
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ResponsableFacturacion_FAC.ToList().ConvertAll<FAResponsableDC>(mo =>
                {
                    return new FAResponsableDC()
                    {
                        IdResponsable = mo.RFA_IdResponsable,
                        DescripcionResponsable = mo.RFA_DescripcionResponsable
                    };
                });
            }
        }

        /// <summary>
        /// Método de consulta de descripciones de nota credito
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FADescripcionNotaDC> ConsultarDescripcionesNota()
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.DescripcionNotasFactura_FAC.ToList().ConvertAll<FADescripcionNotaDC>(mo =>
                {
                    return new FADescripcionNotaDC()
                    {
                        IdDescripcion = mo.DNF_IdDescripcion,
                        Descripcion = mo.DNF_Descripcion
                    };
                });
            }
        }

        /// <summary>
        /// Método de consulta de estados de nota
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FAEstadoNotaDC> ConsultarEstadosNota()
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.EstadoNota_VFAC.ToList().ConvertAll<FAEstadoNotaDC>(mo =>
                {
                    return new FAEstadoNotaDC()
                    {
                        Id = mo.ENF_IdEstado,
                        Descripcion = mo.ENF_Descripcion
                    };
                });
            }
        }

        #endregion Métodos de Consulta
    }
}