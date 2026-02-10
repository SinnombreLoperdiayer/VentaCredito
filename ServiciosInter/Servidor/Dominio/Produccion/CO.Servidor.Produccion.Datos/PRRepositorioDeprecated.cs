using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using CO.Servidor.Produccion.Comun;
using CO.Servidor.Produccion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Comisiones;
using CO.Servidor.Servicios.ContratoDatos.Produccion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Produccion.Datos
{
    public class PRRepositorioDeprecated
    {
        #region Instancia singleton de la clase

        private static readonly PRRepositorioDeprecated instancia = new PRRepositorioDeprecated();

        public static PRRepositorioDeprecated Instancia
        {
            get
            {
                return instancia;
            }
        }

        #endregion Instancia singleton de la clase

        #region Atributos

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private const string NombreModelo = "ModeloProduccion";

        #endregion Atributos

        #region Metodos

        /// <summary>
        /// Adicionar novedad de Forma de Pago al Centro Servicio
        /// </summary>
        /// <param name="novedad"></param>
        public void AdicionarNovedadCentroServicioFormaPago(PANovedadCentroServicioDCDeprecated novedad, string idMotivoNovedad)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  NovedadCentroServicio_PRO novedadCentro = new NovedadCentroServicio_PRO()
            //  {
            //    NCS_FechaAplicacionPr = novedad.FechaAplicacionPr,
            //    NCS_IdCentroServicios = novedad.IdCentroServicios,
            //    NCS_NombreCentroServicios = novedad.NombreCentroServicios,
            //    NCS_IdProduccion = novedad.IdProduccion,
            //    NCS_Observacion = novedad.Observaciones,
            //    NCS_Valor = novedad.Valor,
            //    NCS_IdMotivoNovedad = short.Parse(idMotivoNovedad),
            //    NCS_CreadoPor = ControllerContext.Current.Usuario,
            //    NCS_FechaGrabacion = DateTime.Now
            //  };

            //  contexto.NovedadCentroServicio_PRO.Add(novedadCentro);
            //  contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Obtiene el motivo de la novedad
        /// </summary>
        /// <param name="idMotivoNovedad"></param>
        /// <returns></returns>
        ////public PRMotivoNovedadDC ObtenerMotivoNovedad(short idMotivoNovedad)
        ////{
        ////  using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
        ////  {
        ////    PRMotivoNovedadDC motivoNovedad = new PRMotivoNovedadDC();
        ////    MotivoNovedad_PRO motivo = contexto.MotivoNovedad_PRO.Where(r => r.MNO_IdMotivoNovedad == idMotivoNovedad).SingleOrDefault();
        ////    if (motivo != null)
        ////    {
        ////      motivoNovedad.DescripcionMotivoNovedad = motivo.MNO_Descripcion;
        ////      motivoNovedad.IdMotivoNovedad = motivo.MNO_IdMotivoNovedad;
        ////      motivoNovedad.IdRetencion = motivo.MNO_IdRetencion;
        ////    }

        ////    return motivoNovedad;
        ////  }
        ////}

        /// <summary>
        /// Adicionar novedad de Cambio de destiono al Centro Servicio
        /// </summary>
        /// <param name="novedad"></param>
        public void AdicionarNovedadCentroServicioCambioDestino(PANovedadCentroServicioDCDeprecated novedad)
        {
            //string idMotivoNovedad = ObtenerParametrosProduccion(PRConstantesProduccion.ID_NOVEDAD_CAMBIO_DESTINO_PRODUCCION);
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  NovedadCentroServicio_PRO novedadCentro = new NovedadCentroServicio_PRO()
            //  {
            //    NCS_FechaAplicacionPr = novedad.FechaAplicacionPr,
            //    NCS_IdCentroServicios = novedad.IdCentroServicios,
            //    NCS_NombreCentroServicios = novedad.NombreCentroServicios,
            //    NCS_IdProduccion = novedad.IdProduccion,
            //    NCS_Observacion = novedad.Observaciones,
            //    NCS_Valor = novedad.Valor,
            //    NCS_IdMotivoNovedad = short.Parse(idMotivoNovedad),
            //    NCS_CreadoPor = ControllerContext.Current.Usuario,
            //    NCS_FechaGrabacion = DateTime.Now
            //  };

            //  contexto.NovedadCentroServicio_PRO.Add(novedadCentro);
            //  contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Adicionar novedad de Cambio de destiono al Centro Servicio
        /// </summary>
        /// <param name="novedad"></param>
        public void GuardarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  NovedadCentroServicio_PRO novedadCentro = new NovedadCentroServicio_PRO()
            //  {
            //    NCS_FechaAplicacionPr = ConstantesFramework.MinDateTimeController,
            //    NCS_IdCentroServicios = novedad.CentroServicios.IdCentroServicio,
            //    NCS_NombreCentroServicios = novedad.CentroServicios.Nombre,
            //    NCS_IdProduccion = 0,
            //    NCS_Observacion = novedad.Observaciones,
            //    NCS_Valor = novedad.Valor,
            //    NCS_IdMotivoNovedad = short.Parse(novedad.MotivoNovedad.IdMotivoNovedad.ToString()),
            //    NCS_CreadoPor = ControllerContext.Current.Usuario,
            //    NCS_FechaGrabacion = DateTime.Now,
            //    NCS_EstaAprobada = false
            //  };

            //  contexto.NovedadCentroServicio_PRO.Add(novedadCentro);
            //  contexto.SaveChanges();
            //}
        }

        /// <summary>
        /// Adiciona la Novedad del Centro de Servicio
        /// </summary>
        /// <param name="novedad">Data de la Novedad</param>
        public void AdicionarNovedadCentroServicio(PANovedadCentroServicioDCDeprecated novedad)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //  NovedadCentroServicio_PRO novedadCentro = new NovedadCentroServicio_PRO()
            //  {
            //    NCS_IdMotivoNovedad = (short)novedad.MotivoNovedad.IdMotivoNovedad,
            //    NCS_IdCentroServicios = novedad.IdCentroServicios,
            //    NCS_NombreCentroServicios = novedad.NombreCentroServicios,
            //    NCS_Valor = novedad.Valor,
            //    NCS_IdProduccion = novedad.IdProduccion,
            //    NCS_FechaAplicacionPr = novedad.FechaAplicacionPr,
            //    NCS_EstaAprobada = novedad.EstaAprobada,
            //    NCS_Observacion = novedad.Observaciones,
            //    NCS_FechaGrabacion = novedad.FechaGrabacion,
            //    NCS_CreadoPor = ControllerContext.Current.Usuario
            //  };
            //  contexto.NovedadCentroServicio_PRO.Add(novedadCentro);
            //  contexto.SaveChanges();
            //}
        }

        #region Admin Novedades

        /// <summary>
        /// Retona las novedades de los centros de servicios
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PANovedadCentroServicioDCDeprecated> ObtenerNovedadesCentroSvc(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idCentroServicio)
        {
            totalRegistros = 0;
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();

            //    //LambdaExpression lamda = contexto.CrearExpresionLambda<CentroServicios_PUA>("CES_Estado", ConstantesFramework.ESTADO_ACTIVO, OperadorComparacion.Equal);
            //    //where.Add(lamda, OperadorLogico.And);

            //    LambdaExpression lamda = contexto.CrearExpresionLambda<NovedadesCentroServicios_VPRO>("NCS_IdCentroServicios", idCentroServicio.ToString(), OperadorComparacion.Equal);
            //    where.Add(lamda, OperadorLogico.And);

            //    return contexto.ConsultarNovedadesCentroServicios_VPRO(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
            //      .OrderByDescending(r => r.NCS_FechaGrabacion)
            //      .ToList()
            //      .ConvertAll(r => new PANovedadCentroServicioDCDeprecated()
            //      {
            //          Valor = r.NCS_Valor,
            //          IdCentroServicios = r.NCS_IdCentroServicios,
            //          NombreCentroServicios = r.NCS_NombreCentroServicios,
            //          TipoNovedad = new PRTipoNovedadDCDeprecated()
            //          {
            //              DescripcionTipoNovedad = r.TNP_Descripcion,
            //          },
            //          AplicadaEnPR = r.NCS_EstaAprobada,
            //          FechaAplicacionPR = r.NCS_FechaAplicacionPr,
            //          FechaNovedad = r.NCS_FechaGrabacion,
            //          Usuario = r.NCS_CreadoPor,
            //          IdProduccion = r.NCS_IdProduccion,
            //          Observaciones = r.NCS_Observacion,
            //          IdNovedad = r.NCS_IdNovedadCentroServicios
            //      });
            //}
        }

        /// <summary>
        /// Retorna los tipos de novedad
        /// </summary>
        /// <returns></returns>
        public List<PRTipoNovedadDCDeprecated> ObtenerTiposNovedad()
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.TipoNovedad_PRO.ToList()
            //      .ConvertAll(r => new PRTipoNovedadDCDeprecated()
            //      {
            //          IdTipoNovedad = r.TNP_IdTipoNovedad,
            //          DescripcionTipoNovedad = r.TNP_Descripcion
            //      });
            //}
        }

        /// <summary>
        /// Retorna los tipos de novedad
        /// </summary>
        /// <returns></returns>
        public List<PRMotivoNovedadDCDeprecated> ObtenerMotivoNovedadTipo(int idTipoNovedad)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    var retencionesMotivo = contexto.MotivoNovedad_PRO.Include("RetencionMotivoNovedad_PRO").Where(r => r.MNO_IdTipoNovedad == idTipoNovedad)
            //      .ToList()
            //      .ConvertAll<PRMotivoNovedadDCDeprecated>
            //      (r =>
            //      {
            //          var retencion = contexto.RetencionMotivoNovedad_PRO.Where(t => t.RMN_IdMotivoNovedad == r.MNO_IdMotivoNovedad);

            //          PRMotivoNovedadDCDeprecated motivoNov = new PRMotivoNovedadDCDeprecated()
            //          {
            //              DescripcionMotivoNovedad = r.MNO_Descripcion,
            //              IdMotivoNovedad = r.MNO_IdMotivoNovedad,
            //              Retenciones = retencion.ToList().ConvertAll(ret => new PRRetencionMotivoNovedadDCDeprecated()
            //              {
            //                  IdRetencion = ret.RMN_IdRetencion,
            //                  TarifaPorcentual = ret.RMN_TarifaPorcentual,
            //                  ValorBase = ret.RMN_Base,
            //                  ValorFijo = ret.RMN_ValorFijo,
            //              }),
            //          };
            //          return motivoNov;
            //      }).ToList();

            //    return retencionesMotivo;
            //}
        }

        #endregion Admin Novedades

        /// <summary>
        /// Obtener el valor de un parametro de produccion
        /// </summary>
        /// <param name="idParametro"></param>
        public string ObtenerParametrosProduccion(string idParametro)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    ParametrosProduccion_PRO parametro = contexto.ParametrosProduccion_PRO.Where(par => par.PAP_IdParametro == idParametro.Trim()).FirstOrDefault();
            //    if (parametro != null)
            //    {
            //        return parametro.PAP_ValorParametro;
            //    }
            //    else
            //    {
            //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_PRODUCCION, PREnumTipoError.EX_NO_CONFIGURADO_PARAMETRO.ToString(), String.Format(PRMensajesProduccion.CargarMensaje(PREnumTipoError.EX_NO_CONFIGURADO_PARAMETRO), idParametro)));
            //    }
            //}
        }

        #region Liquidacion Manual

        #region Consultas

        /// <summary>
        /// Obtiene la ultima liquidacion aprobada de la agencia
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PRLiquidacionManualDCDeprecated ObtenerUltimaLiquidacionAprobada(long idCentroServicios)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    PRLiquidacionManualDCDeprecated liquidacionManual = new PRLiquidacionManualDCDeprecated();
            //    var liquidacion = contexto.LiquidacionProduccion_PRO.Where(r => r.LPR_IdCentroServicios == idCentroServicios && r.LPR_EstaAprobada == true)
            //      .OrderByDescending(r => r.LPR_FechaAprobacion)
            //      .FirstOrDefault();
            //    if (liquidacion != null)
            //    {
            //        if (liquidacion.LPR_EstaAprobada)
            //        {
            //            liquidacionManual.IdLiquidacion = liquidacion.LPR_IdLiquidacionProduccion;
            //            liquidacionManual.FechaLiquidacion = liquidacion.LPR_FechaAprobacion;
            //        }
            //        else
            //            liquidacionManual.IdLiquidacion = 0;
            //    }
            //    else
            //    {
            //        liquidacionManual.IdLiquidacion = 0;
            //    }

            //    return liquidacionManual;
            //}
        }

        /// <summary>
        /// Obtiene las comisiones por concepto de un centro de servicios
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="fechaCorte"></param>
        /// <returns></returns>
        public List<PRComisionConceptoDCDeprecated> ObtenerComisionXConcepto(long idCentroServicios, DateTime fechaCorte)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerComisionConcepto_PRO(idCentroServicios, fechaCorte)
            //      .ToList()
            //      .ConvertAll(r => new PRComisionConceptoDCDeprecated()
            //      {
            //          ValorComision = r.ValorComisionVenta.Value,
            //          ValorBaseComision = r.ValorBase.Value,
            //          CantidadComisiones = r.Cantidad.Value,
            //          PorcentajeComision = r.LIC_PorcentajeComisionVenta,
            //          IdTipoComision = r.LIC_IdTipoComision,
            //          DescripcionTipoComision = r.TCO_Descripcion,
            //          IdUnidadNegocio = r.SER_IdUnidadNegocio,
            //          DescripcionUnidadNegocio = r.UNE_Nombre,
            //          DescripcionConceptoLiquidacion = r.UNE_Nombre + " - " + r.TCO_Descripcion
            //      });
            //}
        }

        /// <summary>
        /// Obtiene la comision de venta por los punto de servicios a cargo
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="fechaCorte"></param>
        /// <returns></returns>
        public List<PRComisionPuntosDCDeprecated> ObtenerComisionXPuntosACargo(long idCentroServicios, DateTime fechaCorte)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerComisionPuntosAcargo_PRO(idCentroServicios, fechaCorte)
            //      .ToList()
            //      .ConvertAll(r => new PRComisionPuntosDCDeprecated()
            //    {
            //        IdPuntosServicio = r.LIC_IdCentroServiciosVenta,
            //        NombrePuntoServicio = r.LIC_NombreCentroServiciosVenta,
            //        ValorBase = r.ValorBase.Value,
            //        ValorComision = r.ValorComision.Value
            //    });
            //}
        }

        /// <summary>
        /// Obtiene el detalle de la novedad por tipo
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="fechaCorte"></param>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public List<PRDetalleNovedadDCDeprecated> ObtenerDetalleNovedad(long idCentroServicios, DateTime fechaCorte, int idTipoNovedad)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerNovedadTipoNoved_PRO(idCentroServicios, fechaCorte, idTipoNovedad)
            //      .ToList()
            //      .ConvertAll(r => new PRDetalleNovedadDCDeprecated()
            //      {
            //          IdMotivo = r.MNO_IdMotivoNovedad,
            //          Motivo = r.MNO_Descripcion,
            //          Valor = r.ValorNovedad.Value
            //      });
            //}
        }

        /// <summary>
        /// Obteniene las retenciones por conceptos
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="fechaCorte"></param>
        /// <returns></returns>
        public List<PRDetalleRetencionesDCDeprecated> ObtenerRetencionConcepto(long idCentroServicios, DateTime fechaCorte)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerRetencionServicio_PRO(idCentroServicios, fechaCorte)
            //      .ToList()
            //      .ConvertAll(r => new PRDetalleRetencionesDCDeprecated()
            //      {
            //          ValorBase = r.ValorBase.Value,
            //          PorcentajeRetencion = r.CCR_TarifaPorcentual,
            //          ValorFijo = r.CCR_ValorFijo,
            //          IdRetencion = r.CCR_IdRetencion,
            //          Retencion = r.RET_Descripcion,
            //          IdServicio = r.LIC_IdServicio,
            //          DescripcionServicio = r.SER_Nombre,
            //          IdTipoComision = r.LIC_IdTipoComision,
            //          DescripcionTipoComision = r.TCO_Descripcion,
            //          ValorBaseRetencion = r.CCR_Base,
            //          DescripcionConceptoRetencion = r.SER_Nombre == null ? string.Empty : r.SER_Nombre + " - " + r.TCO_Descripcion,
            //          ValorRetencion = r.RetencionLiquidada.Value
            //      });
            //}
        }

        /// <summary>
        /// Obtiene las retenciones por novedad de reintegro
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <param name="fechaCorte"></param>
        /// <returns></returns>
        public List<PRDetalleRetencionesDCDeprecated> ObtenerRetencionNovedadReintegro(long idCentroServicios, DateTime fechaCorte)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerRetencionNovedad_PRO(idCentroServicios, fechaCorte, PRConstantesProduccion.ID_TIPO_NOVEDAD_REINTEGRO)
            //      .ToList()
            //      .ConvertAll(r => new PRDetalleRetencionesDCDeprecated()
            //      {
            //          Novedad = new PRMotivoNovedadDCDeprecated()
            //          {
            //              IdMotivoNovedad = r.NCS_IdMotivoNovedad,
            //              DescripcionMotivoNovedad = r.MNO_Descripcion
            //          },
            //          IdRetencion = (int)r.RNC_IdRetencion,
            //          Retencion = r.RNC_Descripcion,
            //          ValorFijo = r.RNC_ValorFijo,
            //          PorcentajeRetencion = r.RNC_TarifaPorcentual,
            //          ValorBase = r.ValorBase.Value,
            //          ValorRetencion = r.ValorRetencion.Value
            //      });
            //}
        }

        /// <summary>
        /// retorna el id de la ultima liquidacion si esta no ha sido aprobada
        /// si ya fue aprobada retorna 0
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public long ObtenerUltimaLiquidacionCentroSvc(long idCentroServicio)
        {
            return 0;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    long idLiquidacion;
            //    var ultimaLiquidacion = contexto.paObtenerUltLiquidacionCentroSvc_PRO(idCentroServicio).FirstOrDefault();
            //    if (ultimaLiquidacion != null)
            //    {
            //        if (!ultimaLiquidacion.LPR_EstaAprobada)
            //            idLiquidacion = ultimaLiquidacion.LPR_IdLiquidacionProduccion;
            //        else
            //            idLiquidacion = 0;
            //    }
            //    else
            //        idLiquidacion = 0;

            //    return idLiquidacion;
            //}
        }

        #endregion Consultas

        #region Insert

        /// <summary>
        /// Guarda la liquidacion manual de la agencia
        /// </summary>
        /// <param name="liquidacion"></param>
        /// <returns></returns>
        public long GuardarLiquidacionManual(PRLiquidacionManualDCDeprecated liquidacion)
        {
            return 0;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    long liquidacionProduccion;
            //    var idLiquidacion = contexto.paCrearLiquidacionProducci_PRO(liquidacion.TotalComisiones
            //      , liquidacion.IdCentroServicios
            //      , liquidacion.TotalComisionesPuntos
            //      , liquidacion.TotalComisionesFijas
            //      , liquidacion.TotalNovedadReintegro
            //      , liquidacion.TotalNovedadDescuento
            //      , liquidacion.TotalRetencionesConcepto
            //      , liquidacion.TotalRetencionesNovedad
            //      , liquidacion.FechaCorte
            //      , liquidacion.FechaCorte
            //      , liquidacion.SaldoCaja
            //      , liquidacion.TotalPagos
            //      , liquidacion.TotalDeducciones
            //      , DateTime.Now
            //      , ConstantesFramework.MinDateTimeController
            //      , ControllerContext.Current.Usuario
            //      , string.Empty).FirstOrDefault();

            //    long.TryParse(idLiquidacion.ToString(), out liquidacionProduccion);

            //    return liquidacionProduccion;
            //}
        }

        /// <summary>
        /// Guarda la liquidacion agrupada para la agencia
        /// </summary>
        /// <param name="liquidacion"></param>
        public void GuardaLiquidacionAgrupada(PRLiquidacionManualDCDeprecated liquidacion)
        {        
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearLiquidacionesAgrupadas_PRO(liquidacion.IdCentroServicios
            //      , liquidacion.FechaCorte
            //      , liquidacion.IdLiquidacion
            //      , ControllerContext.Current.Usuario);
            //}
        }

        /// <summary>
        /// Guarda los ingresos fijos de la liquidacion para la agencia
        /// </summary>
        /// <param name="liquidacion"></param>
        public void GuardarIngresosFijos(PRLiquidacionManualDCDeprecated liquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paCrearLiquidaIngresosFijos_PRO(liquidacion.IdLiquidacion
            //      , liquidacion.IdCentroServicios
            //      , (short)liquidacion.ComisionFija.IdTipoComisionFija
            //      , liquidacion.ComisionFija.Descripcion
            //      , liquidacion.ComisionFija.Valor
            //      , liquidacion.ComisionFija.ValorComision
            //      , liquidacion.ComisionFija.IdContrato
            //      , liquidacion.ComisionFija.DiasHabiles
            //      , DateTime.Now
            //      , ControllerContext.Current.Usuario
            //      );
            //}
        }

        #endregion Insert

        #region Update

        public void ActualizarEstadoLiquidacion(long idLiquidacion, string estadoLiquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paActualizaEstLiquidacion_PRO(idLiquidacion, estadoLiquidacion);
            //}
        }

        #endregion Update

        #endregion Liquidacion Manual

        #region Liquidaciones

        #region Consultas

        /// <summary>
        /// Obtiene las retenciones de las novedades de reintegro de una liquidacion
        /// </summary>
        /// <param name="idLiquidacion"></param>
        /// <returns></returns>
        public List<PRDetalleRetencionesDCDeprecated> ObtenerRetencionNovedadReintegroLiquidacion(long idLiquidacion, int idTipoNovedad)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerLiquidaRetenNovedad_PRO(idLiquidacion, idTipoNovedad)
            //      .ToList()
            //      .ConvertAll(r => new PRDetalleRetencionesDCDeprecated()
            //      {
            //          Novedad = new PRMotivoNovedadDCDeprecated()
            //          {
            //              IdMotivoNovedad = r.NCS_IdMotivoNovedad,
            //              DescripcionMotivoNovedad = r.MNO_Descripcion
            //          },
            //          IdRetencion = (int)r.RNC_IdRetencion,
            //          Retencion = r.RNC_Descripcion,
            //          PorcentajeRetencion = r.RNC_TarifaPorcentual,
            //          ValorFijo = r.RNC_ValorFijo,
            //          ValorBase = r.ValorBase.Value,
            //          ValorRetencion = r.ValorRetencion.Value
            //      }); ;
            //}
        }

        /// <summary>
        /// Obtiene las liquidaciones de las novedades de una liquidacion de produccion
        /// </summary>
        /// <param name="idLiquidacion"></param>
        /// <param name="idTipoNovedad"></param>
        public List<PRDetalleNovedadDCDeprecated> ObtenerLiquidacionNovedad(long idLiquidacion, int idTipoNovedad)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerLiquidacionNovedad_PRO(idLiquidacion, idTipoNovedad)
            //      .ToList()
            //      .ConvertAll(r => new PRDetalleNovedadDCDeprecated()
            //      {
            //          IdMotivo = r.MNO_IdMotivoNovedad,
            //          Motivo = r.MNO_Descripcion,
            //          Valor = r.ValorNovedad.Value
            //      });
            //}
        }

        /// <summary>
        /// Obtiene las liquidaciones de las agencias/puntos
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<PRLiquidacionManualDCDeprecated> ObtenerLiquidaciones(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            return null;
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    string idRacol;
            //    string idLiquidacion;
            //    string idAgenciaPunto;
            //    string fechaLiquidacion;
            //    DateTime fLiquidacion;
            //    bool IncluyeFecha;

            //    filtro.TryGetValue("IdRacol", out idRacol);
            //    filtro.TryGetValue("IdLiquidacion", out idLiquidacion);
            //    filtro.TryGetValue("IdCentroServicio", out idAgenciaPunto);
            //    filtro.TryGetValue("FechaLiquidacion", out fechaLiquidacion);

            //    if (string.IsNullOrEmpty(fechaLiquidacion))
            //        IncluyeFecha = false;
            //    else
            //        IncluyeFecha = true;

            //    fLiquidacion = Convert.ToDateTime(fechaLiquidacion, Thread.CurrentThread.CurrentCulture);

            //    if (indicePagina == 0)
            //        indicePagina = 1;

            //    return contexto.paObtenerLiquidaProduccion_PRO(Convert.ToInt64(idRacol)
            //      , Convert.ToInt64(idAgenciaPunto)
            //      , Convert.ToInt64(idLiquidacion)
            //      , fLiquidacion
            //      , IncluyeFecha
            //      , indicePagina
            //      , registrosPorPagina
            //      )
            //      .ToList()
            //      .ConvertAll(r => new PRLiquidacionManualDCDeprecated()
            //      {
            //          IdLiquidacion = r.LPR_IdLiquidacionProduccion,
            //          IdCentroServicios = r.LPR_IdCentroServicios,
            //          EstaAprobada = r.LPR_EstaAprobada,
            //          TotalPagos = r.LPR_TotalPagos,
            //          TotalDeducciones = r.LPR_TotalDeducciones,
            //          SaldoCaja = r.LPR_SaldoFinalCaja,
            //          FechaLiquidacion = r.LPR_FechaGrabacion,
            //          NombreCentroServicios = r.CES_Nombre,
            //          FechaAprobacion = r.LPR_FechaAprobacion,
            //          UsuarioAprobacion = r.LPR_UsuarioAprueba,
            //          NumeroGiro = r.LPR_NumeroGiro,
            //          NumeroGuiaInterna = r.LPR_NumeroGuiaInterna,
            //          FechaCorte = r.LPR_FechaFinCorte
            //      });
            //}
        }

        /// <summary>
        /// Obtiene las liquidaciones de las agencias/puntos
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<PRLiquidacionManualDCDeprecated> ObtenerLiquidacionesAprobadas(IDictionary<string, string> filtro)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    string idCentroLogistico;
            //    string idLiquidacion;
            //    string idTipoCentroSvc;
            //    string idProgramacion;

            //    filtro.TryGetValue("IdCentroLogistico", out idCentroLogistico);
            //    filtro.TryGetValue("IdLiquidacion", out idLiquidacion);
            //    filtro.TryGetValue("IdTipoCentroSvc", out idTipoCentroSvc);
            //    filtro.TryGetValue("IdProgramacion ", out idProgramacion);

            //    return contexto.paObtenerLiquidaProduAprobadas_PRO(Convert.ToInt64(idCentroLogistico)
            //      , Convert.ToInt64(idLiquidacion)
            //      , idTipoCentroSvc
            //      , Convert.ToInt64(idProgramacion)
            //         )
            //      .ToList()
            //      .ConvertAll(r => new PRLiquidacionManualDCDeprecated()
            //      {
            //          IdLiquidacion = r.LPR_IdLiquidacionProduccion,
            //          IdCentroServicios = r.LPR_IdCentroServicios,
            //          EstaAprobada = r.LPR_EstaAprobada,
            //          TotalPagos = r.LPR_TotalPagos,
            //          TotalDeducciones = r.LPR_TotalDeducciones,
            //          SaldoCaja = r.LPR_SaldoFinalCaja,
            //          FechaLiquidacion = r.LPR_FechaGrabacion,
            //          NombreCentroServicios = r.CES_Nombre,
            //          FechaAprobacion = r.LPR_FechaAprobacion,
            //          UsuarioAprobacion = r.LPR_UsuarioAprueba,
            //          NumeroGiro = r.LPR_NumeroGiro,
            //          NumeroGuiaInterna = r.LPR_NumeroGuiaInterna,
            //          FechaCorte = r.LPR_FechaFinCorte,
            //          IdProgramacion = r.PRC_IdProgramacion == null ? 0 : r.PRC_IdProgramacion.Value
            //      });
            //}
            return null;
        }

        /// <summary>
        /// Obtiene las liquidaciones de las comisiones por concepto para una liquidacion
        /// </summary>
        /// <param name="idLiquidacion"></param>
        /// <returns></returns>
        public List<PRComisionConceptoDCDeprecated> ObtenerComisionesLiquidacionProduccion(long idLiquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerLiquidaComiConcepto_PRO(idLiquidacion)
            //      .ToList()
            //   .ConvertAll(r => new PRComisionConceptoDCDeprecated()
            //    {
            //        ValorComision = r.ValorComisionVenta.Value,
            //        ValorBaseComision = r.ValorBase.Value,
            //        CantidadComisiones = r.Cantidad.Value,
            //        IdUnidadNegocio = r.LPG_IdUnidadNegocio,
            //        DescripcionUnidadNegocio = r.LPG_NombreUnidadNegocio,
            //        PorcentajeComision = r.LPG_PorcentajeComision,
            //        IdTipoComision = r.LPG_IdTipoComision,
            //        DescripcionTipoComision = r.LPG_DescripcionTipoComision,
            //        DescripcionConceptoLiquidacion = r.LPG_NombreUnidadNegocio + " - " + r.LPG_DescripcionTipoComision
            //    });
            //}
            return null;
        }

        /// <summary>
        /// Obtiene la liquidacion de los puntos a cargo de una produccion
        /// </summary>
        /// <param name="idLiquidacion"></param>
        /// <returns></returns>
        public List<PRComisionPuntosDCDeprecated> ObtenerLiquidacionPuntosaCargo(long idLiquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerLiquidaComiPtoAcargo_PRO(idLiquidacion)
            //      .ToList()
            //       .ConvertAll(r => new PRComisionPuntosDCDeprecated()
            //       {
            //           IdPuntosServicio = r.LIC_IdCentroServiciosVenta,
            //           NombrePuntoServicio = r.LIC_NombreCentroServiciosVenta,
            //           ValorBase = r.ValorBase.Value,
            //           ValorComision = r.ValorComision.Value
            //       });
            //}
            return null;
        }

        /// <summary>
        /// Obtiene la liquidacion de los ingresos fijos de la produccion
        /// </summary>
        /// <param name="idLiquidacion"></param>
        /// <returns></returns>
        public List<CMComisionesConceptosAdicionales> ObtenerLiquidacionIngresosFijos(long idLiquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerLiquidaIngresosFijos_PRO(idLiquidacion)
            //      .ToList()
            //      .ConvertAll(r => new CMComisionesConceptosAdicionales()
            //      {
            //          DiasHabiles = r.LIF_DiasHabilesLiquidados,
            //          ValorComision = r.LIF_ValorLiquidado,
            //          Descripcion = r.LIF_DescripcionTipoComi,
            //          Valor = r.ValorBase
            //      });
            //}
            return null;
        }

        /// <summary>
        /// Obtener la liquidacion de las retenciones por concepto de una produccion
        /// </summary>
        /// <param name="idLiquidacion"></param>
        /// <returns></returns>
        public List<PRDetalleRetencionesDCDeprecated> ObtenerLiquidacionRetencionConcepto(long idLiquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerLiquidaRetenConce_PRO(idLiquidacion)
            //      .ToList()
            //      .ConvertAll(r => new PRDetalleRetencionesDCDeprecated()
            //      {
            //          ValorBase = r.LPR_BaseRetencion,
            //          IdServicio = r.LPR_IdServicio,
            //          DescripcionServicio = r.LPR_DescripcionServicio,
            //          PorcentajeRetencion = r.LPR_TarifaPorcentualRetencion,
            //          ValorFijo = r.LPR_ValorFijoRetencion,
            //          IdRetencion = r.LPR_IdRetencion,
            //          Retencion = r.LPR_DescripcionRetencion,
            //          ValorRetencion = r.LPR_ValorRetencionLiquidado
            //      });
            //}
            return null;
        }

        #endregion Consultas

        #region Update

        public void ActualizaLiquidacionProduccionAprobada(PRLiquidacionManualDCDeprecated liquidacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    contexto.paActualizaLiquidaAprobada_PRO(liquidacion.IdLiquidacion
            //      , liquidacion.NumeroGuiaInterna
            //      , (short)liquidacion.NumeroGiro
            //      , DateTime.Now
            //      , ControllerContext.Current.Usuario);
            //}
        }

        #endregion Update

        #endregion Liquidaciones

        #region Programacion Liquidaciones

        /// <summary>
        /// Obtiene la programacion de las liquidaciones
        /// </summary>
        public List<PRProgramacionLiquidacionDCDeprecated> ObtenerProgramacionLiquidaciones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    List<PRProgramacionLiquidacionDCDeprecated> programacion = contexto.ConsultarContainsProgramacionLiquidacion_PRO(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
            //      .ConvertAll(r => new PRProgramacionLiquidacionDCDeprecated()
            //      {
            //          IdProgramacion = r.PRL_IdProgramacion,
            //          FechaCorte = r.PRI_FechaCorte,
            //          FechaProgramacion = r.PRI_FechaProgramacion,
            //          FechaGrabacion = r.PRI_FechaGrabacion,
            //          FechaEjecucion = r.PRI_FechaEjecucion,
            //          Usuario = r.PRI_CreadoPor,
            //          DescripcionEstado = r.PRI_FueEjecutada == true ? PRMensajesProduccion.CargarMensaje(PREnumTipoError.IN_EJECUTADO) : PRMensajesProduccion.CargarMensaje(PREnumTipoError.IN_NO_EJECUTADO),
            //          EstaEjecutada = r.PRI_FueEjecutada,
            //          HoraProgramada = r.PRI_FechaProgramacion
            //      });

            //    programacion.ForEach(liquida =>
            //    {
            //        liquida.AgenciasIncluidas = contexto.CentroSvcProgramaLiquida_VPRO
            //          .Where(r => r.PRC_IdProgramacion == liquida.IdProgramacion)
            //          .ToList()
            //          .ConvertAll(r => new PUAgenciaDeRacolDC()
            //          {
            //              IdCentroServicio = r.PRC_IdCentroServicios,
            //              NombreCentroServicio = r.CES_Nombre
            //          });
            //        liquida.CantidadAgencias = liquida.AgenciasIncluidas.Count;
            //    });

            //    return programacion;
            //}
            totalRegistros = 0;
            return null;
        }

        /// <summary>
        /// Obtiene los centros de servicios sin programar
        /// </summary>
        /// <param name="idRacol"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<PUAgenciaDeRacolDC> ObtenerCentroServiciosSinPromagramar(long? idRacol, long? idCentroServicios)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    return contexto.paObtenerCenSvcSinPrograLiqui_PRO(idCentroServicios.Value, idRacol.Value)
            //      .ToList()
            //      .ConvertAll(r => new PUAgenciaDeRacolDC()
            //      {
            //          IdCentroServicio = r.CES_IdCentroServicios,
            //          NombreCentroServicio = r.CES_Nombre,
            //          IdResponsable = r.CEL_IdRegionalAdm,
            //          NombreResponsable = r.NombreRacol,
            //          TipoCentroServicio = r.CES_Tipo,
            //          SaldoAcumuladoCaja = r.saldo == null ? 0 : r.saldo.Value,
            //          FechaUltimaLiquidacion = r.FechaUltimaLiquidacion == null ? DateTime.MinValue : r.FechaUltimaLiquidacion.Value
            //      });
            //}
            return null;
        }

        #region Insert

        /// <summary>
        /// Guarda la programacion de la liquidacion
        /// </summary>
        /// <param name="programacion"></param>
        public void GuardarProgramacionLiquidacion(PRProgramacionLiquidacionDCDeprecated programacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    long id;
            //    ProgramacionLiquidacion_PRO programacionLiquidacion = new ProgramacionLiquidacion_PRO()
            //    {
            //        PRI_FechaEjecucion = ConstantesFramework.MinDateTimeController,
            //        PRI_FechaProgramacion = programacion.FechaProgramacion,
            //        PRI_FechaCorte = programacion.FechaCorte,
            //        PRI_FueEjecutada = false,
            //        PRI_IdEstadoProgramacionLiquidacion = 1,
            //        PRI_FechaGrabacion = DateTime.Now,
            //        PRI_CreadoPor = ControllerContext.Current.Usuario
            //    };

            //    contexto.ProgramacionLiquidacion_PRO.Add(programacionLiquidacion);
            //    contexto.SaveChanges();

            //    id = programacionLiquidacion.PRL_IdProgramacion;

            //    programacion.AgenciasIncluidas.ForEach(agencias =>
            //    {
            //        ProgramacionLiquidacionCentroSvc_PRO programacionCentroSvc = new ProgramacionLiquidacionCentroSvc_PRO()
            //        {
            //            PRC_IdProgramacion = id,
            //            PRC_IdCentroServicios = agencias.IdCentroServicio,
            //            PRC_FechaGrabacion = DateTime.Now,
            //            PRC_CreadoPor = ControllerContext.Current.Usuario
            //        };
            //        contexto.ProgramacionLiquidacionCentroSvc_PRO.Add(programacionCentroSvc);
            //        contexto.SaveChanges();
            //    });
            //}
        }

        /// <summary>
        /// Edita la programacion de la liquidacion
        /// </summary>
        /// <param name="programacion"></param>
        public void EditarProgramacionLiquidacion(PRProgramacionLiquidacionDCDeprecated programacion)
        {
            //using (ModeloProduccion contexto = new ModeloProduccion(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            //{
            //    ProgramacionLiquidacion_PRO pro = contexto.ProgramacionLiquidacion_PRO
            //      .Where(r => r.PRL_IdProgramacion == programacion.IdProgramacion)
            //      .SingleOrDefault();

            //    if (pro == null)
            //    { }

            //    pro.PRI_FechaCorte = programacion.FechaCorte;
            //    pro.PRI_FechaProgramacion = programacion.FechaProgramacion;
            //    contexto.SaveChanges();

            //    List<ProgramacionLiquidacionCentroSvc_PRO> agenciasIncluidas = contexto.ProgramacionLiquidacionCentroSvc_PRO.Where(r => r.PRC_IdProgramacion == programacion.IdProgramacion).ToList();

            //    ///Adiciona las agencias a la programacion
            //    programacion.AgenciasIncluidas.ForEach(agencias =>
            //    {
            //        ///Valida que la agencia este en el listado de las agencias incluidas
            //        var estabaProgramada = agenciasIncluidas.Where(r => r.PRC_IdCentroServicios == agencias.IdCentroServicio).FirstOrDefault();
            //        if (estabaProgramada == null)
            //        {
            //            ProgramacionLiquidacionCentroSvc_PRO programacionCentroSvc = new ProgramacionLiquidacionCentroSvc_PRO()
            //            {
            //                PRC_IdProgramacion = programacion.IdProgramacion,
            //                PRC_IdCentroServicios = agencias.IdCentroServicio,
            //                PRC_FechaGrabacion = DateTime.Now,
            //                PRC_CreadoPor = ControllerContext.Current.Usuario
            //            };
            //            contexto.ProgramacionLiquidacionCentroSvc_PRO.Add(programacionCentroSvc);
            //            contexto.SaveChanges();
            //        }
            //    });

            //    ///Elimina las agencias seleccionadas de la programacion
            //    agenciasIncluidas.ForEach(agencias =>
            //    {
            //        var EliminarAgencia = programacion.AgenciasIncluidas.Where(r => r.IdCentroServicio == agencias.PRC_IdCentroServicios).FirstOrDefault();
            //        if (EliminarAgencia == null)
            //        {
            //            contexto.ProgramacionLiquidacionCentroSvc_PRO.Remove(agencias);
            //            contexto.SaveChanges();
            //        }
            //    });
            //}
        }

        #endregion Insert

        #endregion Programacion Liquidaciones

        #endregion Metodos
    }
}