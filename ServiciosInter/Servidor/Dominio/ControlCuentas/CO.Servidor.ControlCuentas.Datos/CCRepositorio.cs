using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using CO.Servidor.ControlCuentas.Comun;
using CO.Servidor.ControlCuentas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.ControlCuentas.Datos
{
  public partial class CCRepositorio
  {
    #region Campos

    private static readonly CCRepositorio instancia = new CCRepositorio();
    private const string NombreModelo = "ModeloControlCuentas";

    #endregion Campos

    #region Propiedades

    /// <summary>
    /// Retorna la instancia de la clase TARepositorio
    /// </summary>
    public static CCRepositorio Instancia
    {
      get { return CCRepositorio.instancia; }
    }

    #endregion Propiedades

    /// <summary>
    /// Adiciona un registro al almacen de control de cuentas
    /// </summary>
    /// <param name="almacen">Objeto almacen</param>
    public void AdicionarAlmacenControlCuentas(CCAlmacenDC almacen)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {          
        AlmacenControlCuentas_CCU nuevoAlmacen = new AlmacenControlCuentas_CCU()
        {
          AAC_IdOperacion = almacen.IdOperacion,
          AAC_IdTipoOperacion = almacen.IdTipoOperacion,
          AAC_IdRacol=almacen.IdRacol,
          AAC_Caja = almacen.Caja,
          AAC_Lote = almacen.Lote,
          AAC_Posicion = almacen.Posicion,
          AAC_FechaGrabacion = DateTime.Now,
          AAC_CreadoPor = ControllerContext.Current.Usuario
        };

        contexto.AlmacenControlCuentas_CCU.Add(nuevoAlmacen);
        contexto.SaveChanges();
      }
    }

    /// <summary>
    /// Obtiene los datos de caja, lote y posición
    /// </summary>
    /// <param name="almacen">Objeto almacen</param>
    /// <returns>Almacen</returns>
    public CCAlmacenDC ObtenerDatosAlmacen(CCAlmacenDC almacen)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        AlmacenControlCuentas_CCU consulta = contexto.AlmacenControlCuentas_CCU
          .Where(r => r.AAC_CreadoPor == ControllerContext.Current.Usuario && r.AAC_IdRacol==almacen.IdRacol && r.AAC_IdTipoOperacion==almacen.IdTipoOperacion)
          .OrderByDescending(o => o.AAC_Caja)
          .ThenByDescending(ol => ol.AAC_Lote)
          .ThenByDescending(ot => ot.AAC_Posicion)
          .FirstOrDefault();

        if (consulta != null && consulta.AAC_Posicion < CCConstantesControlCuentas.NUMERO_MAXIMO_POSICIONES_LOTE)
        {
          almacen.Caja = consulta.AAC_Caja;
          almacen.Lote = consulta.AAC_Lote;
          almacen.Posicion = consulta.AAC_Posicion + 1;
        }
        else
        {
            lock (this)
            {
                AlmacenControlCuentas_CCU loteDisponible = contexto.AlmacenControlCuentas_CCU
                    .Where(r => r.AAC_IdRacol == almacen.IdRacol && r.AAC_IdTipoOperacion == almacen.IdTipoOperacion)
                    //.Where(ld => ld.AAC_Caja == almacen.CajaActual)
                  .OrderByDescending(c => c.AAC_Caja)
                  .ThenByDescending(old => old.AAC_Lote)
                  .FirstOrDefault();

                if (loteDisponible != null)
                {
                    if (loteDisponible.AAC_Lote < CCConstantesControlCuentas.NUMERO_MAXIMO_LOTES_CAJA)
                    {
                        almacen.Caja = loteDisponible.AAC_Caja;
                        almacen.Lote = loteDisponible.AAC_Lote + 1;
                        almacen.Posicion = 1;
                    }
                    else
                    {
                        almacen.Caja = loteDisponible.AAC_Caja + 1;
                        almacen.Lote = 1;
                        almacen.Posicion = 1;
                        almacen.CajaLlena = true;
                    }
                }
                else
                {
                    almacen.Caja = 1;
                    almacen.Lote = 1;
                    almacen.Posicion = 1;
                    almacen.CajaLlena = true;
                }
            }
        }

        return almacen;
      }
    }

    /// <summary>
    /// Obtiene los giros de una agencia
    /// </summary>
    /// <returns>Colección giros</returns>
    public List<GIAdmisionGirosDC> ObtenerGirosAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        List<paObtenerGiroAlmacen> consulta = contexto.paObtenerGiroAlmacen_CCU(agencia.IdCentroServicio, fechaInicial, fechaFinal).ToList();

        if (consulta != null)
        {
          return consulta.ConvertAll(r => new GIAdmisionGirosDC()
          {
            IdGiro = r.ADG_IdGiro,
            FechaGrabacion = r.ADG_FechaGrabacion,
            AgenciaDestino = new PUCentroServiciosDC()
            {
              IdCentroServicio = r.ADG_IdCentroServicioDestino,
              Nombre = r.ADG_NombreCentroServicioDestino
            },
            GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
            {
              ClienteRemitente = new CLClienteContadoDC()
              {
                Identificacion = r.ADG_IdRemitente,
                Nombre = r.ADG_NombreRemitente
              },
              ClienteDestinatario = new CLClienteContadoDC()
              {
                Identificacion = r.ADG_IdDestinatario,
                Nombre = r.ADG_NombreDestinatario
              }
            },
            Precio = new TAPrecioDC()
            {
              ValorServicio = r.ADG_ValorPorte,
              ValorGiro = r.ADG_ValorGiro,
              ValorAdicionales = r.ADG_ValorAdicionales,
              ValorTotal = r.ADG_ValorTotal
            },
            Caja = r.AAC_Caja ?? 0,
            Lote = r.AAC_Lote ?? 0,
            Posicion = r.AAC_Posicion ?? 0,
            Aprobada = r.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = r.AAC_IdAlmacenControCuentas == null ? true : false,
            EstadoGiro = r.ESG_Estado
          });
        }
        else
          return new List<GIAdmisionGirosDC>();
      }
    }

    /// <summary>
    /// Obtiene las admisiones y pagos de una agencia
    /// </summary>
    /// <param name="agencia">Objeto agencia</param>
    /// <returns>Colección admisiones pagos</returns>
    public List<PGPagoAdmisionGiroDC> ObtenerAdmisionPagoGiroAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        List<paObtenerPagoGiroAlmacen> consulta = contexto.paObtenerPagoGiroAlmacen_CCU(agencia.IdCentroServicio, fechaInicial, fechaFinal).ToList();

        if (consulta != null)
        {
          return consulta.ConvertAll(r => new PGPagoAdmisionGiroDC()
          {
            Pago = new PGPagosGirosDC()
            {
              IdComprobantePago = r.PAG_IdComprobantePago,
              FechaHoraPago = r.PAG_FechaGrabacion,
              ValorPagado = r.PAG_ValorPagado,
              ClienteCobrador = new CLClienteContadoDC() { Nombre = r.PAG_NombreCobrador + r.PAG_Apellido1Cobrador + r.PAG_Apellido2Cobrador }
            },
            Admision = new GIAdmisionGirosDC()
            {
              IdGiro = r.ADG_IdGiro,
              AgenciaOrigen = new PUCentroServiciosDC() { Nombre = r.ADG_NombreCentroServicioOrigen },
              GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
              {
                ClienteRemitente = new CLClienteContadoDC() { Nombre = r.ADG_NombreRemitente }
              },
              Caja = r.AAC_Caja ?? 0,
              Lote = r.AAC_Lote ?? 0,
              Posicion = r.AAC_Posicion ?? 0
            },
            Aprobada = r.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = r.AAC_IdAlmacenControCuentas == null ? true : false
          });
        }
        else
          return new List<PGPagoAdmisionGiroDC>();
      }
    }

    /// <summary>
    /// Obtiene los gastos de caja
    /// </summary>
    /// <param name="agencia">Objeto agencia</param>
    /// <returns>Colección de gastos</returns>
    public List<CAMovimientoCajaDC> ObtenerGastosCaja(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        List<paObtenerMovCajaAlmacen> consulta = contexto.paObtenerMovCajaAlmacen_CCU(agencia.IdCentroServicio, fechaInicial, fechaFinal, (int)CAEnumConceptosCaja.GASTOS_GENERALES).ToList();

        if (consulta != null)
        {
          return consulta.ConvertAll(r => new CAMovimientoCajaDC()
          {
            IdOperacion = r.AAC_IdOperacion ?? r.RTD_IdRegistroTransDetalleCaja,
            Concepto = r.COC_Nombre,
            Fecha = r.RVF_FechaGrabacion,
            FormaPago = r.RVF_DescripcionFormaPago,
            Valor = r.RVF_Valor,
            Caja = Convert.ToInt32(r.AAC_Caja),
            Lote = Convert.ToInt32(r.AAC_Lote),
            Posicion = Convert.ToInt32(r.AAC_Posicion),
            Aprobada = r.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = r.AAC_IdAlmacenControCuentas == null ? true : false
          });
        }
        else
          return new List<CAMovimientoCajaDC>();
      }
    }

    /// <summary>
    /// Obtiene los movimientos de caja para un centro de servicio dado en un rago de fechas que difieren de ventas de mensajería, 
    /// pago de al cobros, venta de giros, pago de giros y ventas de pines prepago.
    /// </summary>
    ///<param name="agencia"></param>
    ///<param name="fechaFinal"></param>
    ///<param name="fechaInicial"></param>
    public List<CAMovimientoCajaDC> ObtenerOtrosMovimientosCaja(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        List<paObtenerOtrosMovCaja_CCU_Result> consulta = contexto.paObtenerOtrosMovCaja_CCU(agencia.IdCentroServicio, fechaInicial, fechaFinal).ToList();

        if (consulta != null)
        {
          return consulta.ConvertAll(r => new CAMovimientoCajaDC()
          {
            IdOperacion = r.AAC_IdOperacion ?? r.RTD_IdRegistroTransDetalleCaja,
            Concepto = r.COC_Nombre,
            Fecha = r.RVF_FechaGrabacion,
            FormaPago = r.RVF_DescripcionFormaPago,
            Valor = r.RVF_Valor,
            Caja = Convert.ToInt32(r.AAC_Caja),
            Lote = Convert.ToInt32(r.AAC_Lote),
            Posicion = Convert.ToInt32(r.AAC_Posicion),
            Aprobada = r.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = r.AAC_IdAlmacenControCuentas == null ? true : false
          });
        }
        else
          return new List<CAMovimientoCajaDC>();
      }
    }

    /// <summary>
    /// Retorna las ventas de pin prepago realizads por la gencia en el rango de fechas dado
    /// </summary>
    /// <param name="agencia"></param>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <returns></returns>
    public List<CAMovimientoCajaDC> ObtenerVentasPinPrepago(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        List<paObtenerMovCajaAlmacen> consulta = contexto.paObtenerMovCajaAlmacen_CCU(agencia.IdCentroServicio, fechaInicial, fechaFinal, (int)CAEnumConceptosCaja.VENTA_PIN_PREPAGO).ToList();

        if (consulta != null)
        {
          return consulta.ConvertAll(r => new CAMovimientoCajaDC()
          {
            IdOperacion = r.AAC_IdOperacion ?? r.RTD_IdRegistroTransDetalleCaja,
            Concepto = r.COC_Nombre,
            Fecha = r.RVF_FechaGrabacion,
            FormaPago = r.RVF_DescripcionFormaPago,
            Valor = r.RVF_Valor,
            Caja = Convert.ToInt32(r.AAC_Caja),
            Lote = Convert.ToInt32(r.AAC_Lote),
            Posicion = Convert.ToInt32(r.AAC_Posicion),
            Aprobada = r.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = r.AAC_IdAlmacenControCuentas == null ? true : false
          });
        }
        else
          return new List<CAMovimientoCajaDC>();
      }
    }

    /// <summary>
    /// Retorna las operaciones de caja que tengan el concepto de pago "Al Cobro" realizadas por la agencia en el rango de fechas dado
    /// </summary>
    /// <param name="agencia"></param>
    /// <param name="fechaInicial"></param>
    /// <param name="fechaFinal"></param>
    /// <returns></returns>
    public List<CAMovimientoCajaDC> ObtenerRecaudosAlCobro(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        List<paObtenerMovCajaAlmacen> consulta = contexto.paObtenerMovCajaAlmacen_CCU(agencia.IdCentroServicio, fechaInicial, fechaFinal, (int)CAEnumConceptosCaja.PAGO_DE_ENVIO_AL_COBRO).ToList();

        if (consulta != null)
        {
          return consulta.ConvertAll(r => new CAMovimientoCajaDC()
          {
            IdOperacion = r.AAC_IdOperacion ?? r.RTD_IdRegistroTransDetalleCaja,
            Concepto = r.COC_Nombre,
            Fecha = r.RVF_FechaGrabacion,
            FormaPago = r.RVF_DescripcionFormaPago,
            Valor = r.RVF_Valor,
            Caja = Convert.ToInt32(r.AAC_Caja),
            Lote = Convert.ToInt32(r.AAC_Lote),
            Posicion = Convert.ToInt32(r.AAC_Posicion),
            Aprobada = r.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = r.AAC_IdAlmacenControCuentas == null ? true : false
          });
        }
        else
          return new List<CAMovimientoCajaDC>();
      }
    }

    /// <summary>
    /// Obtiene las guías de mensajería a partir de una agencia
    /// </summary>
    /// <param name="agencia">Objeto Agencia</param>
    /// <returns>Colección de guías</returns>
    public List<ADGuia> ObtenerGuiasAgencia(PUCentroServiciosDC agencia, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        //buscar en la tabla admisión donde agencia origen, mensajero = 0 y fecha
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        var consulta = contexto.paObtenerGuiaMenAlmacen_CCU(agencia.IdCentroServicio, 0, fechaInicial, fechaFinal, false).ToList();

        if (consulta != null)
        {
          List<ADGuia> guias = consulta.Select(s => new ADGuia()
            {
              NumeroGuia = s.ADM_NumeroGuia,
              FechaAdmision = s.ADM_FechaAdmision,
              NombreCiudadDestino=s.CiudadDestino,
              NombreCentroServicioDestino = s.ADM_NombreCentroServicioDestino,
              Remitente = new CLClienteContadoDC() { Nombre = s.ADM_NombreRemitente },
              Destinatario = new CLClienteContadoDC() { Nombre = s.ADM_NombreDestinatario },
              ValorDeclarado = s.ADM_ValorDeclarado,
              ValorAdmision = s.ADM_ValorAdmision,
              ValorPrimaSeguro = s.ADM_ValorPrimaSeguro,
              ValorEmpaque = s.ADM_ValorEmpaque,
              ValorAdicionales = s.ADM_ValorAdicionales,
              ValorTotal = s.ADM_ValorTotal,
              Aprobada = s.AAC_IdAlmacenControCuentas == null ? false : true,
              NoAprobada = s.AAC_IdAlmacenControCuentas == null ? true : false,
              NombreServicio=s.ADM_NombreServicio,
              Caja = s.AAC_Caja ?? 0,
              Lote = s.AAC_Lote ?? 0,
              Posicion = s.AAC_Posicion ?? 0,
              Peso = s.ADM_Peso,
              FormasPagoDescripcion = s.FormasPago,
              FormasPago = s.IdsFormasPago.Split(',').ToList().ConvertAll(forma =>
              {
                if (forma.Trim() != string.Empty)
                {
                  return new ADGuiaFormaPago { IdFormaPago = short.Parse(forma.Trim()) };
                }
                else
                {
                  return new ADGuiaFormaPago { IdFormaPago = 0 };
                }
              })
            }).ToList();

          return guias;
        }
        else
          return new List<ADGuia>();
      }
    }

    /// <summary>
    /// Obtiene las guías de mensajería a partir de una agencia y un cliente
    /// </summary>
    /// <param name="agencia">Objeto Agencia</param>
    /// <param name="cliente">Objeto Cliente</param>
    /// <returns>Colección guías de mensajería</returns>
    public List<ADGuia> ObtenerGuiasClienteCredito(PUCentroServiciosDC agencia, CLClientesDC cliente, DateTime fechaInicial, DateTime fechaFinal, int idSucursal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        //Buscar en la tabla admision donde idmensajero=,  para esa agencia origen y fecha
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        var consulta = contexto.paObtenerGuiaMenAlmacenCliente_CCU(fechaInicial, fechaFinal, cliente.IdCliente, idSucursal).ToList();

        if (consulta != null)
        {
          List<ADGuia> guias = consulta.Select(s => new ADGuia()
          {
            NumeroGuia = s.ADM_NumeroGuia,
            FechaAdmision = s.ADM_FechaAdmision,
            NombreCiudadDestino = s.CiudadDestino,
            NombreCentroServicioDestino = s.ADM_NombreCentroServicioDestino,
            Remitente = new CLClienteContadoDC() { Nombre = s.ADM_NombreRemitente },
            Destinatario = new CLClienteContadoDC() { Nombre = s.ADM_NombreDestinatario },
            ValorDeclarado = s.ADM_ValorDeclarado,
            ValorAdmision = s.ADM_ValorAdmision,
            ValorPrimaSeguro = s.ADM_ValorPrimaSeguro,
            ValorEmpaque = s.ADM_ValorEmpaque,
            ValorAdicionales = s.ADM_ValorAdicionales,
            ValorTotal = s.ADM_ValorTotal,
            Aprobada = s.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = s.AAC_IdAlmacenControCuentas == null ? true : false,
            Caja = s.AAC_Caja ?? 0,
            Lote = s.AAC_Lote ?? 0,
            Posicion = s.AAC_Posicion ?? 0,
            Peso = s.ADM_Peso,
            NombreServicio=s.ADM_NombreServicio,
            FormasPagoDescripcion = s.FormasPago,
            FormasPago = s.IdsFormasPago.Split(',').ToList().ConvertAll(forma =>
            {
              if (forma.Trim() != string.Empty)
              {
                return new ADGuiaFormaPago { IdFormaPago = short.Parse(forma.Trim()) };
              }
              else
              {
                return new ADGuiaFormaPago { IdFormaPago = 0 };
              }
            })
          }).ToList();

          return guias;
        }
        else
          return new List<ADGuia>();

      }
    }

    /// <summary>
    /// Obtiene las guías de mensajería a partir de una agencia y un mensajero
    /// </summary>
    /// <param name="agencia">Objeto Agencia</param>
    /// <param name="cliente">Objeto Cliente</param>
    /// <returns>Colección guías de mensajería</returns>
    public List<ADGuia> ObtenerGuiasMensajero(PUCentroServiciosDC agencia, OUNombresMensajeroDC mensajero, DateTime fechaInicial, DateTime fechaFinal)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        //Buscar en la tabla admision donde idmensajero=,  para esa agencia origen y fecha
        fechaInicial = (Convert.ToDateTime(fechaInicial, Thread.CurrentThread.CurrentCulture)).AddHours(00).AddMinutes(00).AddSeconds(00);
        fechaFinal = Convert.ToDateTime(fechaFinal, Thread.CurrentThread.CurrentCulture).AddHours(23).AddMinutes(59).AddSeconds(59);

        var consulta = contexto.paObtenerGuiaMenAlmacen_CCU(agencia.IdCentroServicio, mensajero.IdPersonaInterna, fechaInicial, fechaFinal, true).ToList();

        if (consulta != null)
        {
          List<ADGuia> guias = consulta.Select(s => new ADGuia()
          {
            NombreCiudadDestino = s.CiudadDestino,
            NumeroGuia = s.ADM_NumeroGuia,
            FechaAdmision = s.ADM_FechaAdmision,
            NombreCentroServicioDestino = s.ADM_NombreCentroServicioDestino,
            Remitente = new CLClienteContadoDC() { Nombre = s.ADM_NombreRemitente },
            Destinatario = new CLClienteContadoDC() { Nombre = s.ADM_NombreDestinatario },
            ValorDeclarado = s.ADM_ValorDeclarado,
            ValorAdmision = s.ADM_ValorAdmision,
            ValorPrimaSeguro = s.ADM_ValorPrimaSeguro,
            ValorEmpaque = s.ADM_ValorEmpaque,
            ValorAdicionales = s.ADM_ValorAdicionales,
            ValorTotal = s.ADM_ValorTotal,
            Aprobada = s.AAC_IdAlmacenControCuentas == null ? false : true,
            NoAprobada = s.AAC_IdAlmacenControCuentas == null ? true : false,
            Caja = s.AAC_Caja ?? 0,
            Lote = s.AAC_Lote ?? 0,
            Posicion = s.AAC_Posicion ?? 0,
            Peso = s.ADM_Peso,
            NombreServicio=s.ADM_NombreServicio,
            FormasPagoDescripcion = s.FormasPago, 
            FormasPago = s.IdsFormasPago.Split(',').ToList().ConvertAll(forma => 
              {
                if (forma.Trim() != string.Empty)
                {
                  return new ADGuiaFormaPago { IdFormaPago = short.Parse(forma.Trim()) };
                }
                else
                {
                  return new ADGuiaFormaPago { IdFormaPago = 0 };
                }
              })
          }).ToList();

          return guias;
        }
        else
          return new List<ADGuia>();
      }
    }

    /// <summary>
    /// Obtiene la informacion del almacen
    /// </summary>
    /// <param name="idOperacion">Numero del giro</param>
    /// <returns></returns>
    public List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        List<CCAlmacenDC> almacenControl = contexto.AlmacenControlCuentas_CCU.Include("TipoOperacion_CCU").Where(almacen => almacen.AAC_IdOperacion == idOperacion).ToList()
          .ConvertAll(conv => new CCAlmacenDC()
                        {
                          IdOperacion = conv.AAC_IdOperacion,
                          IdTipoOperacion = conv.AAC_IdTipoOperacion,
                          DescripcionTipoOperacion = conv.TipoOperacion_CCU.TOP_Descripcion,
                          Caja = conv.AAC_Caja,
                          Lote = conv.AAC_Lote,
                          Posicion = conv.AAC_Posicion,
                          Fecha = conv.AAC_FechaGrabacion,
                          Usuario = conv.AAC_CreadoPor
                        });

        return almacenControl;
      }
    }

    /// <summary>
    /// Obtiene el almacen CtrolCuentas Giros
    /// </summary>
    /// <param name="idOperacion"></param>
    /// <returns>info del Archivo Control Ctas</returns>
    public CCAlmacenDC ObtenerAlmacenControlCuentasGiros(long idOperacion)
    {
      using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
      {
        AlmacenControlCuentas_CCU almacenControl = contexto.AlmacenControlCuentas_CCU.Include("TipoOperacion_CCU").FirstOrDefault(almacen => almacen.AAC_IdOperacion == idOperacion);
        CCAlmacenDC infoAlmacen = null;

        if (almacenControl != null)
        {
          infoAlmacen = new CCAlmacenDC()
          {
            IdOperacion = almacenControl.AAC_IdOperacion,
            IdTipoOperacion = almacenControl.AAC_IdTipoOperacion,
            DescripcionTipoOperacion = almacenControl.TipoOperacion_CCU.TOP_Descripcion,
            Caja = almacenControl.AAC_Caja,
            Lote = almacenControl.AAC_Lote,
            Posicion = almacenControl.AAC_Posicion,
            Fecha = almacenControl.AAC_FechaGrabacion,
            Usuario = almacenControl.AAC_CreadoPor
          };
        }

        return infoAlmacen;
      }
    }

    /// <summary>
    /// Ejecuta toda la lógica para cambiar una factura de forma de pago al cobro a crédito
    /// </summary>
    /// <param name="cambioFPAlCobroCredito">Datos del cambio</param>
    public void CambiarFPAlCobroACredito(CCNovedadFPAlCobroCreditoDC cambioFPAlCobroCredito)
    {
        using (DesModeloControlCuentas contexto = new DesModeloControlCuentas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
        {
            contexto.paCambioFPAlCobroCredito_CCU(cambioFPAlCobroCredito.Guia.NumeroGuia, cambioFPAlCobroCredito.IdClienteConvenio, cambioFPAlCobroCredito.IdContrato, cambioFPAlCobroCredito.IdSucursal,cambioFPAlCobroCredito.QuienSolicita,cambioFPAlCobroCredito.Observaciones, ControllerContext.Current.Usuario);
            contexto.SaveChanges();            
        }
    }

    public decimal RegistrarEncabezadoCargueArchivoAjuste(CCEncabezadoArchivoAjusteGuiaDC encabezadoArchivo)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paCrearEncabezadoArchivoCargueAjustes_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("EAC_NombreArchivo", encabezadoArchivo.NombreArchivo));
          cmd.Parameters.Add(new SqlParameter("EAC_TotalRegistros", encabezadoArchivo.TotalRegistros));
          cmd.Parameters.Add(new SqlParameter("EAC_Guid", encabezadoArchivo.Guid));
          cmd.Parameters.Add(new SqlParameter("EAC_CreadoPor", encabezadoArchivo.Usuario));
          var paramId = new SqlParameter();
          paramId.ParameterName = "EAC_Id";
          paramId.Direction = System.Data.ParameterDirection.Output;
          paramId.DbType = System.Data.DbType.Decimal;
          cmd.Parameters.Add(paramId);

          cmd.ExecuteNonQuery();
          cnx.Close();
          return (decimal)paramId.Value;

       }      
       
    }

    public void ActualizarEncabezadoCargueArchivoAjuste(long IdArchivo, short idEstado)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paActualizarEncabezadoArchivoCargueAjustes_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("EAC_Id", IdArchivo));
          cmd.Parameters.Add(new SqlParameter("EAC_Estado", idEstado));
          cmd.ExecuteNonQuery();
          cnx.Close();
       }

    }

    public void InsertarLogErrorCalculoPrecios(long numeroGuia, string mensaje, int? idListaPrecios, decimal? valorPrimaCalculado, decimal? valorAdmisionCalculado, decimal? valorTotalCalculado, string usuario, short fuenteInformacion)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paInsertaLogErrorCalculo_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("LEC_NumeroGuia", numeroGuia));
          cmd.Parameters.Add(new SqlParameter("LEC_Mensaje", mensaje));
          if (idListaPrecios.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("LEC_IdListaPrecios", idListaPrecios));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("LEC_IdListaPrecios", DBNull.Value));
          }

          if (valorAdmisionCalculado.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("LEC_ValorTransCalculado", valorAdmisionCalculado.Value));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("LEC_ValorTransCalculado", DBNull.Value));
          }

          if (valorPrimaCalculado.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("LEC_ValorPrimaCalculado", valorPrimaCalculado.Value));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("LEC_ValorPrimaCalculado", DBNull.Value));
          }

          if (valorTotalCalculado.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("LEC_ValorTotalCalculado", valorTotalCalculado.Value));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("LEC_ValorTotalCalculado", DBNull.Value));
          }

          cmd.Parameters.Add(new SqlParameter("LEC_CreadoPor", usuario));
          cmd.Parameters.Add(new SqlParameter("LEC_FuenteInformacion", fuenteInformacion));
          cmd.ExecuteNonQuery();
          cnx.Close();
       }
    }

    public List<CCEncabezadoArchivoAjusteGuiaDC> ConsultarUltimosRegistrosCargueArchivoUsuario(string usuario)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          List<CCEncabezadoArchivoAjusteGuiaDC> listaUltimosCargues = new List<CCEncabezadoArchivoAjusteGuiaDC>();
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paConsultaUltimosCargue_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("DAC_CreadoPor", usuario));
          var reader = cmd.ExecuteReader();
          while (reader.Read())
          {
             listaUltimosCargues.Add(new CCEncabezadoArchivoAjusteGuiaDC
             {
                Id = long.Parse(reader["EAC_Id"].ToString()),
                NombreArchivo = reader["EAC_NombreArchivo"].ToString(),
                TotalRegistros = int.Parse(reader["EAC_TotalRegistros"].ToString()),
                Estado = short.Parse(reader["EAC_Estado"].ToString()),
                DescripcionEstado = (short.Parse(reader["EAC_Estado"].ToString()) == 1 ? "CARGUE INICIADO" : short.Parse(reader["EAC_Estado"].ToString()) == 2 ? "ARCHIVO CARGADO" : short.Parse(reader["EAC_Estado"].ToString()) == 3 ? "PROCESAMIENTO INICIADO" : short.Parse(reader["EAC_Estado"].ToString()) == 4 ? "FINALIZADO PROCESAMIENTO" : "ESTADO INVALIDO"),
                Fecha = (DateTime)(reader["EAC_FechaGrabacion"])
             });
          }
          cnx.Close();
          return listaUltimosCargues;
       }
    }

    public List<long> ConsultarIdsArchivoNoProcesados(long idArchivo)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          List<long> lista = new List<long>();
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paConsultaDetalladoArchivoSinProcesar_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("EAC_Id", idArchivo));
          var reader = cmd.ExecuteReader();
          while (reader.Read())
          {
             lista.Add((long)reader["DAC_Id"]);
          }
          cnx.Close();
          return lista;
       }
    }

    public void ProcesarRegistroArchivo(long idRegistro)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paProcesarDetallado_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("DAC_Id", idRegistro));
          cmd.ExecuteNonQuery();
          cnx.Close();
       }
    }

    public CO.Servidor.Servicios.ContratoDatos.ControlCuentas.CCGuiaValidacion ObtenerInformacionBasicaGuiaRegistroModificada(long idRegistro)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paConsultaInfoGuiaForzada_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("DAC_Id", idRegistro));
          SqlDataReader reader = cmd.ExecuteReader();
          CCGuiaValidacion guia = null;
          while (reader.Read())
          {
             guia = new CCGuiaValidacion
             {
                NumeroGuia = (long)reader["DAC_NumeroGuia"],                
                IdTipoEntrega = reader["ADM_IdTipoEntrega"].ToString(),
                IdCiudadDestino = reader["ADM_IdCiudadDestino"].ToString(),
                IdCiudadOrigen = reader["ADM_IdCiudadOrigen"].ToString(),
                IdServicio = (int)reader["ADM_IdServicio"],
                Peso = (decimal)reader["ADM_Peso"],
                ValorDeclarado = (decimal)reader["ADM_ValorDeclarado"]
                
             };
             if (reader["IdListaPreciosMCC"] != DBNull.Value)
             {
               guia.IdListaPreciosConvenioConvenio = (int)reader["IdListaPreciosMCC"];
             }
             if (reader["IdListaPreciosMCP"] != DBNull.Value)
                guia.IdListaPreciosConvenioPeaton = (int?)reader["IdListaPreciosMCP"];
             if (reader["IdListaPreciosMPC"] != DBNull.Value)
                guia.IdListaPreciosPeatonConvenio = (int?)reader["IdListaPreciosMPC"];
             if (reader["DAC_ValorTransporte"] != DBNull.Value)
               guia.ValorTransporte = (decimal?)reader["DAC_ValorTransporte"];
             if (reader["DAC_ValorTotal"] != DBNull.Value)
                guia.ValorTotal = (decimal?)reader["DAC_ValorTotal"];
             if (reader["DAC_ValorPrima"] != DBNull.Value)
                guia.ValorPrima = (decimal?)reader["DAC_ValorPrima"];
             
          }
          cnx.Close();
          return guia;
       }
    }

    public void RegistarDetalleCargueArchivoAjuste(CCDetalladoArchivoAjusteGuiaDC detalladoArchivo)
    {
       using (SqlConnection cnx = new SqlConnection(conexionStringController))
       {
          cnx.Open();
          SqlCommand cmd = new SqlCommand("paCrearDetalleArchivoCargueAjustes_CCU", cnx);
          cmd.CommandType = System.Data.CommandType.StoredProcedure;
          cmd.Parameters.Add(new SqlParameter("DAC_IdEncabezado", detalladoArchivo.IdArchivo));
          cmd.Parameters.Add(new SqlParameter("DAC_NumeroGuia", detalladoArchivo.NumeroGuia));
          if (detalladoArchivo.ValorComercial.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorComercial", detalladoArchivo.ValorComercial));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorComercial", DBNull.Value));
          }
          if (detalladoArchivo.ValorTransporte.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorTransporte", detalladoArchivo.ValorTransporte));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorTransporte", DBNull.Value));
          }
          if (detalladoArchivo.ValorPrima.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorPrima", detalladoArchivo.ValorPrima));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorPrima", DBNull.Value));
          }
          if (detalladoArchivo.ValorTotal.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorTotal", detalladoArchivo.ValorTotal));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_ValorTotal", DBNull.Value));
          }
          if (detalladoArchivo.Peso.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_Peso", detalladoArchivo.Peso));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_Peso", DBNull.Value));
          }
          if (detalladoArchivo.Servicio.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_Servicio", detalladoArchivo.Servicio));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_Servicio", DBNull.Value));
          }
          if (detalladoArchivo.FormaPago.HasValue)
          {
             cmd.Parameters.Add(new SqlParameter("DAC_FormaPago", detalladoArchivo.FormaPago.Value));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_FormaPago", DBNull.Value));
          }
          if (string.IsNullOrWhiteSpace(detalladoArchivo.TipoEntrega))
          {
             cmd.Parameters.Add(new SqlParameter("DAC_TipoEntrega", DBNull.Value));
          }
          else
          {
             cmd.Parameters.Add(new SqlParameter("DAC_TipoEntrega", detalladoArchivo.TipoEntrega));
          }
          cmd.Parameters.Add(new SqlParameter("DAC_CreadoPor", detalladoArchivo.Usuario));
          
          cmd.ExecuteNonQuery();
          cnx.Close();
       }
    }
    private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
  }
}