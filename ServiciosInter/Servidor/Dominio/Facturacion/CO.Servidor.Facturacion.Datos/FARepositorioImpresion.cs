using System;
using System.Collections.Generic;
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
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Facturacion.Datos
{
    public class FARepositorioImpresion
    {
        private const string Automatica = "AUT";
        private const string DescripcionAutomatica = "Automatica";
        private const string DescripcionManual = "Manual";
        private const string NombreModelo = "ModeloFacturacion";
        private const string EstadoAnulada = "ANULADA";
        private static readonly FARepositorioImpresion instancia = new FARepositorioImpresion();

        /// <summary>
        /// Retorna la instancia de la clase FARepositorio
        /// </summary>
        public static FARepositorioImpresion Instancia
        {
            get { return FARepositorioImpresion.instancia; }
        }

        #region Campos
        

        #endregion Campos

        #region Métodos

        /// <summary>
        /// Método para obtener facturas para impresión con filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasImpresion(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            //ArmarFiltro(filtro);

            /////////////////////////////////////////////            
            string fechaFacturaciondesde;
            string fechaFacturacionhasta;
            DateTime? fechaInicial;
            DateTime? fechaFinal;
            string cliente;
            string numFacturadesde;
            string numFacturahasta;
            string idRacol;

            filtro.TryGetValue("FechaFacturacionDesde", out fechaFacturaciondesde);
            filtro.TryGetValue("FechaFacturacionHasta", out fechaFacturacionhasta);
            filtro.TryGetValue("idCliente", out cliente);
            filtro.TryGetValue("numFacturaDesde", out numFacturadesde);
            filtro.TryGetValue("numFacturaHasta", out numFacturahasta);
            filtro.TryGetValue("idRacol", out idRacol);            

            if (fechaFacturaciondesde == null && fechaFacturacionhasta==null)
            {
                fechaInicial = ConstantesFramework.MinDateTimeController;
                fechaFinal = DateTime.Now;
            }
            else
            {
                //DateTime fechaFactura = Convert.ToDateTime(fechaFacturacionde, Thread.CurrentThread.CurrentCulture);

                fechaInicial = DateTime.Parse(fechaFacturaciondesde, Thread.CurrentThread.CurrentCulture).Date;
                fechaFinal = DateTime.Parse(fechaFacturacionhasta, Thread.CurrentThread.CurrentCulture).Date.AddDays(1);
            }

            if (string.IsNullOrEmpty(idRacol))
                idRacol = "0";
            if (string.IsNullOrEmpty(cliente))
                cliente = "0";
            /////////////////////////////////////////////

            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var facturas = contexto.paObtenerFacturasImpresion_FAC(fechaInicial, fechaFinal, int.Parse(cliente),long.Parse(numFacturadesde), long.Parse(numFacturahasta), int.Parse(idRacol), indicePagina, registrosPorPagina)
              .ToList();

                if (facturas != null && facturas.Any())
                {
                    var facturasReturn = facturas.ConvertAll<FAFacturaClienteAutDC>(fac =>
                    {
                        FAFacturaClienteAutDC factura = new FAFacturaClienteAutDC()
                        {
                            ConceptosFactura = new System.Collections.ObjectModel.ObservableCollection<FAConceptoFacturaDC>(contexto.DetalleXServicioFactura_FAC.Where(det => det.DSF_NumeroFactura == fac.REF_NumeroFactura).ToList().ConvertAll<FAConceptoFacturaDC>(con =>
                            {
                                return new FAConceptoFacturaDC()
                                {
                                    Cantidad = con.DSF_Cantidad,
                                    DescripcionConcepto = con.DSF_DescripcionConcepto,
                                    IdConcepto = con.DSF_IdDetallexServFactura,
                                    ValorUnitario = con.DSF_TotalNeto / con.DSF_Cantidad,
                                    Manual = con.DSF_ConceptoManual,
                                    ImpuestosConcepto = new System.Collections.ObjectModel.ObservableCollection<FAImpuestoConceptoDC>(contexto.ImpuestosDetalleFactura_FAC.Where(imp => imp.IDF_IdDetalleFactura == con.DSF_IdDetallexServFactura).ToList().ConvertAll<FAImpuestoConceptoDC>(impu =>
                                    {
                                        return new FAImpuestoConceptoDC()
                                        {
                                            BaseCalculo = impu.IDF_BaseCalculo,
                                            ValorPorc = impu.IDF_ValorPorc,
                                            IdImpuesto = impu.IDF_IdImpuesto,
                                            Descripcion = impu.IDF_Descripcion
                                        };
                                    }))
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
                            DireccionRadicacion = fac.REF_DireccionRadicacion,
                            DirigidoA = fac.REF_DirigidoA,
                            FechaGrabacion = fac.fechaEmision,
                            FormaPago = fac.REF_FormaPago,
                            DescFormaPago = fac.REF_FormaPago,
                            IdCliente = fac.REF_IdCliente,
                            IdContrato = fac.REF_IdContrato,
                            IdRacol = fac.REF_IdRacol,
                            NombreRacol = fac.REF_NombreRacol,
                            NombreContrato = fac.REF_NombreContrato,
                            NumeroContrato = fac.REF_NumeroContrato,
                            NumeroFactura = fac.REF_NumeroFactura,
                            RazonSocial = fac.REF_RazonSocial,                            
                            ValorDescuentos = fac.REF_ValorDescuentos,
                            ValorImpuestos = fac.REF_ValorImpuestos,
                            ValorNeto = fac.REF_ValorNeto,
                            ValorTotal = fac.REF_ValorTotal.Value,
                            TelefonoRadicacion = fac.REF_TelefonoRadicacion,
                            CiudadRadicacion = new PALocalidadDC { IdLocalidad = fac.REF_CiudadRadicacion, Nombre = contexto.Localidad_PAR.Where(loc => loc.LOC_IdLocalidad == fac.REF_CiudadRadicacion).FirstOrDefault().LOC_Nombre },
                            TipoFacturacion = fac.REF_TipoFacturacion == Automatica ? DescripcionAutomatica : DescripcionManual,
                            EstadoActual = fac.ESF_DescEstadoActual,
                            GuiaInterna = new ADGuiaInternaDC { NumeroGuia = fac.REF_GuiaInternaAsociada == null ? 0 : fac.REF_GuiaInternaAsociada.Value },
                            SeImprime = fac.ESF_DescEstadoActual == EstadoAnulada ? false : true,
                        };
                        return factura;
                    });

                    return facturasReturn;
                }
                else
                    return new List<FAFacturaClienteAutDC>();
            }
        }

        /// <summary>
        /// Método para obtener facturas para impresión con filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public IEnumerable<FAFacturaClienteAutDC> ConsultaFacturasImpresionCompletas(IDictionary<string, string> filtro)
        {
            //ArmarFiltro(filtro);
            /////////////////////////////////////////////            
            string fechaFacturaciondesde;
            string fechaFacturacionhasta;
            DateTime? fechaInicial;
            DateTime? fechaFinal;
            string cliente;
            string numFacturadesde;
            string numFacturahasta;
            string idRacol;

            filtro.TryGetValue("FechaFacturacionDesde", out fechaFacturaciondesde);
            filtro.TryGetValue("FechaFacturacionHasta", out fechaFacturacionhasta);
            filtro.TryGetValue("idCliente", out cliente);
            filtro.TryGetValue("numFacturaDesde", out numFacturadesde);
            filtro.TryGetValue("numFacturaHasta", out numFacturahasta);
            filtro.TryGetValue("idRacol", out idRacol);

            if (fechaFacturaciondesde == null && fechaFacturacionhasta == null)
            {
                fechaInicial = ConstantesFramework.MinDateTimeController;
                fechaFinal = DateTime.Now;
            }
            else
            {
                //DateTime fechaFactura = Convert.ToDateTime(fechaFacturacionde, Thread.CurrentThread.CurrentCulture);

                fechaInicial = DateTime.Parse(fechaFacturaciondesde, Thread.CurrentThread.CurrentCulture).Date;
                fechaFinal = DateTime.Parse(fechaFacturacionhasta, Thread.CurrentThread.CurrentCulture).Date.AddDays(1);
            }
            if (string.IsNullOrEmpty(idRacol))
                idRacol = "0";

            if (string.IsNullOrEmpty(cliente))
                cliente = "0";
            /////////////////////////////////////////////

            List<FAFacturaClienteAutDC> lista = new List<FAFacturaClienteAutDC>();
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                var facturas = contexto.paObtenerFacturasImpresionFiltro_FAC(fechaInicial, fechaFinal, int.Parse(cliente), long.Parse(numFacturadesde), long.Parse(numFacturahasta), int.Parse(idRacol))
              .ToList();

                if (facturas != null)
                {
                    facturas.ForEach(f =>
                    {
                        if (f.ESF_DescEstadoActual != EstadoAnulada)
                        {
                            lista.Add(new FAFacturaClienteAutDC()
                            {
                                DireccionRadicacion = f.REF_DireccionRadicacion,
                                DirigidoA = f.REF_DirigidoA,
                                FechaGrabacion = f.fechaEmision,
                                FormaPago = f.REF_FormaPago,
                                DescFormaPago = f.REF_FormaPago,
                                IdCliente = f.REF_IdCliente,
                                IdContrato = f.REF_IdContrato,
                                IdRacol = f.REF_IdRacol,
                                NombreRacol = f.REF_NombreRacol,
                                NombreContrato = f.REF_NombreContrato,
                                NumeroContrato = f.REF_NumeroContrato,
                                NumeroFactura = f.REF_NumeroFactura,
                                RazonSocial = f.REF_RazonSocial,
                                ValorTotal = f.REF_ValorTotal.Value,
                                TelefonoRadicacion = f.REF_TelefonoRadicacion,
                                CiudadRadicacion = new PALocalidadDC { IdLocalidad = f.REF_CiudadRadicacion, Nombre = f.LOC_Nombre },
                                TipoFacturacion = f.REF_TipoFacturacion == Automatica ? DescripcionAutomatica : DescripcionManual,
                                EstadoActual = f.ESF_DescEstadoActual,
                                GuiaInterna = new ADGuiaInternaDC { NumeroGuia = f.REF_GuiaInternaAsociada == null ? 0 : f.REF_GuiaInternaAsociada.Value }
                            });
                        }
                    });
                }
                return lista;
            }
        }

        /// <summary>
        /// Método para obtener facturas para impresión con filtro
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<FAFacturaClienteAutDC> ConsultaFacturasListas(List<long> listaFacturas)
        {
            List<FAFacturaClienteAutDC> lista = new List<FAFacturaClienteAutDC>();
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                listaFacturas.ForEach(f =>
                {
                    ResumenFactura_FAC fac = contexto.ResumenFactura_FAC.Where(nro => nro.REF_NumeroFactura == f).FirstOrDefault();
                    if (fac != null && fac.ESF_DescEstadoActual != EstadoAnulada)
                    {
                        lista.Add(new FAFacturaClienteAutDC()
                         {
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
                             RazonSocial = fac.REF_RazonSocial,
                             ValorTotal = fac.REF_ValorTotal.Value,
                             TelefonoRadicacion = fac.REF_TelefonoRadicacion,
                             CiudadRadicacion = new PALocalidadDC { IdLocalidad = fac.REF_CiudadRadicacion, Nombre = fac.Localidad_PAR.LOC_Nombre },
                             TipoFacturacion = fac.REF_TipoFacturacion == Automatica ? DescripcionAutomatica : DescripcionManual,
                             EstadoActual = fac.ESF_DescEstadoActual,
                             GuiaInterna = new ADGuiaInternaDC { NumeroGuia = fac.REF_GuiaInternaAsociada == null ? 0 : fac.REF_GuiaInternaAsociada.Value }
                         });
                    }
                });
                return lista;
            }
        }

        /// <summary>
        /// Método que se encargad de extraer los datos del filtro
        /// </summary>
        /// <param name="filtro"></param>
        private void ArmarFiltro(IDictionary<string, string> filtro)
        {
            
        }

        public void ActualizarGuiaFactura(FAFacturaClienteAutDC factura)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ResumenFactura_FAC fac = contexto.ResumenFactura_FAC.Where(nro => nro.REF_NumeroFactura == factura.NumeroFactura).FirstOrDefault();
                if (fac != null)
                {
                    fac.REF_GuiaInternaAsociada = factura.GuiaInterna.NumeroGuia;
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Métodos
    }
}