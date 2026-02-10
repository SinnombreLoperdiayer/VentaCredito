using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Tarifas.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Tarifas.Datos
{
  /// <summary>
  /// Clase para guardar la Auditoria del Módulo de Tarifas
  /// </summary>
  internal class TARepositorioAudit
  {
    /// <summary>
    /// Guardar Auditoria lista de precio
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapeoAuditListaPrecio(EntidadesTarifas contexto)
    {
      contexto.Audit<ListaPrecios_TAR, ListaPreciosHist_TAR>((record, action) => new ListaPreciosHist_TAR()
      {
        LIP_IdListaPrecios = record.Field<ListaPrecios_TAR, int>(f => f.LIP_IdListaPrecios),
        LIP_Nombre = record.Field<ListaPrecios_TAR, string>(f => f.LIP_Nombre),
        LIP_Inicio = record.Field<ListaPrecios_TAR, DateTime>(f => f.LIP_Inicio),
        LIP_Fin = record.Field<ListaPrecios_TAR, DateTime>(f => f.LIP_Fin),
        LIP_Estado = record.Field<ListaPrecios_TAR, string>(f => f.LIP_Estado),
        LIP_EsTarifaPlena = record.Field<ListaPrecios_TAR, Boolean>(f => f.LIP_EsTarifaPlena),
        LIP_IdMoneda = record.Field<ListaPrecios_TAR, string>(f => f.LIP_IdMoneda),
        LIP_EsListaCliente = record.Field<ListaPrecios_TAR, bool>(f => f.LIP_EsListaCliente),
        LIP_FechaGrabacion = record.Field<ListaPrecios_TAR, DateTime>(f => f.LIP_FechaGrabacion),
        LIP_CreadoPor = record.Field<ListaPrecios_TAR, string>(f => f.LIP_CreadoPor),
        LIP_FechaCambio = DateTime.Now,
        LIP_CambiadoPor = ControllerContext.Current.Usuario,
        LIP_TipoCambio = action.ToString(),
      }, (ph) => contexto.ListaPreciosHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda auditoria de lista de precio servicio
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapeoAuditListaPrecioServicio(EntidadesTarifas contexto)
    {
      contexto.Audit<ListaPrecioServicio_TAR, ListaPrecioServicioHist_TAR>((record, action) => new ListaPrecioServicioHist_TAR()
      {
        LPS_IdListaPrecioServicio = record.Field<ListaPrecioServicio_TAR, int>(f => f.LPS_IdListaPrecioServicio),
        LPS_IdServicio = record.Field<ListaPrecioServicio_TAR, int>(f => f.LPS_IdServicio),
        LPS_IdListaPrecios = record.Field<ListaPrecioServicio_TAR, int>(f => f.LPS_IdListaPrecios),
        LPS_PrimaSeguros = record.Field<ListaPrecioServicio_TAR, decimal>(f => f.LPS_PrimaSeguros),
        LPS_FechaGrabacion = record.Field<ListaPrecioServicio_TAR, DateTime>(f => f.LPS_FechaGrabacion),
        LPS_CreadoPor = record.Field<ListaPrecioServicio_TAR, string>(f => f.LPS_CreadoPor),
        LIS_FechaCambio = DateTime.Now,
        LIS_CambiadoPor = ControllerContext.Current.Usuario,
        LIS_TipoCambio = action.ToString(),
      }, (ph) => contexto.ListaPrecioServicioHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda Auditoria de Precio internacional
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapeoAuditPrecioInternacional(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioInternacional_TAR, PrecioInternacionalHist_TAR>((record, action) => new PrecioInternacionalHist_TAR()
      {
        PIN_IdPrecioInternalcional = record.Field<PrecioInternacional_TAR, int>(f => f.PIN_IdPrecioInternalcional),
        PIN_IdListaPrecioServicio = record.Field<PrecioInternacional_TAR, int>(f => f.PIN_IdListaPrecioServicio),
        PIN_IdZona = record.Field<PrecioInternacional_TAR, string>(f => f.PIN_IdZona),
        PIN_IdTipoEmpaque = record.Field<PrecioInternacional_TAR, int>(f => f.PIN_IdTipoEmpaque),
        PIN_Peso = record.Field<PrecioInternacional_TAR, decimal>(f => f.PIN_Peso),
        PIN_Valor = record.Field<PrecioInternacional_TAR, decimal>(f => f.PIN_Valor),
        PIN_FechaGrabacion = record.Field<PrecioInternacional_TAR, DateTime>(f => f.PIN_FechaGrabacion),
        PIN_CreadoPor = record.Field<PrecioInternacional_TAR, string>(f => f.PIN_CreadoPor),
        PIN_FechaCambio = DateTime.Now,
        PIN_CambiadoPor = ControllerContext.Current.Usuario,
        PIN_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioInternacionalHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio excepción
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearAuditPrecioExcepcion(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioServicioExcepcionTrayecto_TAR, PrecioServicioExcepcionTrayectoHist_TAR>((record, action) => new PrecioServicioExcepcionTrayectoHist_TAR()
        {
          SET_IdPrecioServicioExcepionTrayecto = record.Field<PrecioServicioExcepcionTrayecto_TAR, long>(f => f.SET_IdPrecioServicioExcepionTrayecto),
          SET_IdListaPrecioServicio = record.Field<PrecioServicioExcepcionTrayecto_TAR, int>(f => f.SET_IdListaPrecioServicio),
          SET_IdLocalidadOrigen = record.Field<PrecioServicioExcepcionTrayecto_TAR, string>(f => f.SET_IdLocalidadOrigen),
          SET_IdLocalidadDestino = record.Field<PrecioServicioExcepcionTrayecto_TAR, string>(f => f.SET_IdLocalidadDestino),
          SET_ValorKiloAdicional = record.Field<PrecioServicioExcepcionTrayecto_TAR, decimal>(f => f.SET_ValorKiloAdicional),
          SET_ValorKiloInicial = record.Field<PrecioServicioExcepcionTrayecto_TAR, decimal>(f => f.SET_ValorKiloInicial),
          SET_EsDestinoTodoElPais = record.Field<PrecioServicioExcepcionTrayecto_TAR, bool?>(f => f.SET_EsDestinoTodoElPais),
          SET_EsOrigenTodoElPais = record.Field<PrecioServicioExcepcionTrayecto_TAR, bool?>(f => f.SET_EsOrigenTodoElPais),
          SET_FechaGrabacion = record.Field<PrecioServicioExcepcionTrayecto_TAR, DateTime>(f => f.SET_FechaGrabacion),
          SET_CreadoPor = record.Field<PrecioServicioExcepcionTrayecto_TAR, string>(f => f.SET_CreadoPor),
          SET_FechaCambio = DateTime.Now,
          SET_CambiadoPor = ControllerContext.Current.Usuario,
          SET_TipoCambio = action.ToString(),
        }, (ph) => contexto.PrecioServicioExcepcionTrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de valor peso declarado
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapearAuditValorPesoDeclarado(EntidadesTarifas contexto)
    {
      contexto.Audit<ValorPesoDeclarado_TAR, ValorPesoDeclaradoHist_TAR>((record, action) => new ValorPesoDeclaradoHist_TAR()
        {
          VMD_IdValorMinimoDeclarado = record.Field<ValorPesoDeclarado_TAR, int>(f => f.VMD_IdValorMinimoDeclarado),
          VMD_IdListaPrecios = record.Field<ValorPesoDeclarado_TAR, int>(f => f.VMD_IdListaPrecios),
          VMD_PesoInicial = record.Field<ValorPesoDeclarado_TAR, decimal>(f => f.VMD_PesoInicial),
          VMD_PesoFinal = record.Field<ValorPesoDeclarado_TAR, decimal>(f => f.VMD_PesoFinal),
          VMD_ValorMinimoDeclarado = record.Field<ValorPesoDeclarado_TAR, decimal>(f => f.VMD_ValorMinimoDeclarado),
          VMD_ValorMaximoDeclarado = record.Field<ValorPesoDeclarado_TAR, decimal>(f => f.VMD_ValorMaximoDeclarado),
          VMD_FechaGrabacion = record.Field<ValorPesoDeclarado_TAR, DateTime>(f => f.VMD_FechaGrabacion),
          VMD_CreadoPor = record.Field<ValorPesoDeclarado_TAR, string>(f => f.VMD_CreadoPor),
          VMD_FechaCambio = DateTime.Now,
          VMD_CambiadoPor = ControllerContext.Current.Usuario,
          VMD_TipoCambio = action.ToString(),
        }, (ph) => contexto.ValorPesoDeclaradoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de excepciones de trayecto subtrayecto
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapearAuditExcepcionTrayectoSub(EntidadesTarifas contexto)
    {
      //contexto.Audit<ExcepcionTrayecto_TAR, ExcepcionTrayectoHist_TAR>((record, action) => new ExcepcionTrayectoHist_TAR()
      //{
      //  EXT_IdExcepionTrayecto = record.Field<ExcepcionTrayecto_TAR, long>(f => f.EXT_IdExcepionTrayecto),
      //  EXT_IdListaPrecios = record.Field<ExcepcionTrayecto_TAR, int>(f => f.EXT_IdListaPrecios),
      //  EXT_IdLocalidadDestino = record.Field<ExcepcionTrayecto_TAR, string>(f => f.EXT_IdLocalidadDestino),
      //  EXT_IdLocalidadOrigen = record.Field<ExcepcionTrayecto_TAR, string>(f => f.EXT_IdLocalidadOrigen),
      //  EXT_IdTrayectoSubTrayecto = record.Field<ExcepcionTrayecto_TAR, int>(f => f.EXT_IdTrayectoSubTrayecto),
      //  EXT_FechaGrabacion = record.Field<ExcepcionTrayecto_TAR, DateTime>(f => f.EXT_FechaGrabacion),
      //  EXT_CreadoPor = record.Field<ExcepcionTrayecto_TAR, string>(f => f.EXT_CreadoPor),
      //  EXT_FechaCambio = DateTime.Now,
      //  EXT_CambiadoPor = ControllerContext.Current.Usuario,
      //  EXT_TipoCambio = action.ToString(),
      //}, (ph) => contexto.ExcepcionTrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de excepciones de servicios de un trayecto subtrayecto
    /// </summary>
    /// <param name="contexto"></param>
    internal static void MapearAuditExcepcionServicioTrayectoSub(EntidadesTarifas contexto)
    {
      //contexto.Audit<ServicioExcepcionTrayecto_TAR, ServicioExcepcionTrayeHist_TAR>((record, action) => new ServicioExcepcionTrayeHist_TAR()
      //{
      //  SET_IdExcepionTrayecto = record.Field<ServicioExcepcionTrayecto_TAR, long>(f => f.SET_IdExcepionTrayecto),
      //  SET_IdServicio = record.Field<ServicioExcepcionTrayecto_TAR, int>(f => f.SET_IdServicio),
      //  SET_TiempoEntrega = record.Field<ServicioExcepcionTrayecto_TAR, int>(f => f.SET_TiempoEntrega),
      //  SET_FechaGrabacion = record.Field<ServicioExcepcionTrayecto_TAR, DateTime>(f => f.SET_FechaGrabacion),
      //  SET_CreadoPor = record.Field<ServicioExcepcionTrayecto_TAR, string>(f => f.SET_CreadoPor),
      //  SET_FechaCambio = DateTime.Now,
      //  SET_CambiadoPor = ControllerContext.Current.Usuario,
      //  SET_TipoCambio = action.ToString(),
      //}, (ph) => contexto.ServicioExcepcionTrayeHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la autitoria de requisito servicio
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearRequisitoServicio(EntidadesTarifas contexto)
    {
      contexto.Audit<RequisitoServicio_TAR, RequisitoServicioHist_TAR>((record, action) => new RequisitoServicioHist_TAR()
      {
        RES_IdRequisitoServicio = record.Field<RequisitoServicio_TAR, int>(f => f.RES_IdRequisitoServicio),
        RES_IdServicio = record.Field<RequisitoServicio_TAR, int>(f => f.RES_IdServicio),
        RES_Descripcion = record.Field<RequisitoServicio_TAR, string>(f => f.RES_Descripcion),
        RES_FechaGrabacion = record.Field<RequisitoServicio_TAR, DateTime>(f => f.RES_FechaGrabacion),
        RES_CreadoPor = record.Field<RequisitoServicio_TAR, string>(f => f.RES_CreadoPor),
        RES_FechaCambio = DateTime.Now,
        RES_CambiadoPor = ControllerContext.Current.Usuario,
        RES_TipoCambio = action.ToString(),
      }, (ph) => contexto.RequisitoServicioHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioTrayecto(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioTrayecto_TAR, PrecioTrayectoHist_TAR>((record, action) => new PrecioTrayectoHist_TAR()
      {
        PTR_IdPrecioTrayectoSubTrayect = record.Field<PrecioTrayecto_TAR, long>(f => f.PTR_IdPrecioTrayectoSubTrayect),
        PTR_IdListaPrecioServicio = record.Field<PrecioTrayecto_TAR, int>(f => f.PTR_IdListaPrecioServicio),
        PTR_IdTrayectoSubTrayecto = record.Field<PrecioTrayecto_TAR, int>(f => f.PTR_IdTrayectoSubTrayecto),
        PTR_ValorFijo = record.Field<PrecioTrayecto_TAR, decimal>(f => f.PTR_ValorFijo),
        PTR_FechaGrabacion = record.Field<PrecioTrayecto_TAR, DateTime>(f => f.PTR_FechaGrabacion),
        PTR_CreadoPor = record.Field<PrecioTrayecto_TAR, string>(f => f.PTR_CreadoPor),
        PTR_Porcentaje = record.Field<PrecioTrayecto_TAR, decimal>(f => f.PTR_Porcentaje),
        PTR_FechaCambio = DateTime.Now,
        PTR_CambiadoPor = ControllerContext.Current.Usuario,
        PTR_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioTrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio rango
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioRango(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioRango_TAR, PrecioRangoHist_TAR>((record, action) => new PrecioRangoHist_TAR()
      {
        PRA_IdPrecioRango = record.Field<PrecioRango_TAR, long>(f => f.PRA_IdPrecioRango),
        PRA_IdListaPrecioServicio = record.Field<PrecioRango_TAR, int>(f => f.PRA_IdListaPrecioServicio),
        PRA_Inicial = record.Field<PrecioRango_TAR, decimal>(f => f.PRA_Inicial),
        PRA_Final = record.Field<PrecioRango_TAR, decimal>(f => f.PRA_Final),
        PRA_Valor = record.Field<PrecioRango_TAR, decimal>(f => f.PRA_Valor),
        PRA_Porcentaje = record.Field<PrecioRango_TAR, decimal>(f => f.PRA_Porcentaje),
        PRA_FechaGrabacion = record.Field<PrecioRango_TAR, DateTime>(f => f.PRA_FechaGrabacion),
        PRA_CreadoPor = record.Field<PrecioRango_TAR, string>(f => f.PRA_CreadoPor),
        PRA_FechaCambio = DateTime.Now,
        PRA_CambiadoPor = ControllerContext.Current.Usuario,
        PRA_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioRangoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de servicio impuesto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearServicioImpuesto(EntidadesTarifas contexto)
    {
      contexto.Audit<ServicioImpuesto_TAR, ServicioImpuestoHist_TAR>((record, action) => new ServicioImpuestoHist_TAR()
      {
        SEI_IdServicio = record.Field<ServicioImpuesto_TAR, int>(f => f.SEI_IdServicio),
        SEI_IdImpuesto = record.Field<ServicioImpuesto_TAR, short>(f => f.SEI_IdImpuesto),
        SEI_FechaGrabacion = record.Field<ServicioImpuesto_TAR, DateTime>(f => f.SEI_FechaGrabacion),
        SEI_CreadoPor = record.Field<ServicioImpuesto_TAR, string>(f => f.SEI_CreadoPor),
        SEI_FechaCambio = DateTime.Now,
        SEI_CambiadoPor = ControllerContext.Current.Usuario,
        SEI_TipoCambio = action.ToString(),
      }, (ph) => contexto.ServicioImpuestoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de centro de correspondencia
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioCentroCorrespondencia(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioCentroCorrespondencia_TAR, PrecioCentroCorresponHist_TAR>((record, action) => new PrecioCentroCorresponHist_TAR()
      {
        POP_IdPrecioServiCentroCorresp = record.Field<PrecioCentroCorrespondencia_TAR, int>(f => f.POP_IdPrecioServiCentroCorresp),
        POP_IdListaPrecioServicio = record.Field<PrecioCentroCorrespondencia_TAR, int>(f => f.POP_IdListaPrecioServicio),
        POP_Valor = record.Field<PrecioCentroCorrespondencia_TAR, decimal>(f => f.POP_Valor),
        POP_FechaGrabacion = record.Field<PrecioCentroCorrespondencia_TAR, DateTime>(f => f.POP_FechaGrabacion),
        POP_CreadoPor = record.Field<PrecioCentroCorrespondencia_TAR, string>(f => f.POP_CreadoPor),
        POP_FechaCambio = DateTime.Now,
        POP_CambiadoPor = ControllerContext.Current.Usuario,
        POP_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioCentroCorresponHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio valor adicional
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioValorAdicional(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioValorAdicional_TAR, PrecioValorAdicionalHist_TAR>((record, action) => new PrecioValorAdicionalHist_TAR()
      {
        PVA_IdPrecioServicio = record.Field<PrecioValorAdicional_TAR, int>(f => f.PVA_IdPrecioServicio),
        PVA_IdTipoValorAdicional = record.Field<PrecioValorAdicional_TAR, string>(f => f.PVA_IdTipoValorAdicional),
        PVA_Valor = record.Field<PrecioValorAdicional_TAR, decimal>(f => f.PVA_Valor),
        PVA_FechaGrabacion = record.Field<PrecioValorAdicional_TAR, DateTime>(f => f.PVA_FechaGrabacion),
        PVA_CreadoPor = record.Field<PrecioValorAdicional_TAR, string>(f => f.PVA_CreadoPor),
        PVA_FechaCambio = DateTime.Now,
        PVA_CambiadoPor = ControllerContext.Current.Usuario,
        PVA_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioValorAdicionalHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio trayecto subtrayecto valor adicional
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioTraSubTraValAdicional(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioTrayeSubTraValorAdic_TAR, PrecioTraSubTraVlrAdiHist_TAR>((record, action) => new PrecioTraSubTraVlrAdiHist_TAR()
      {
        PTV_IdPrecioTrayectoSubTrayect = record.Field<PrecioTrayeSubTraValorAdic_TAR, long>(f => f.PTV_IdPrecioTrayectoSubTrayect),
        PTV_IdTipoValorAdicional = record.Field<PrecioTrayeSubTraValorAdic_TAR, string>(f => f.PTV_IdTipoValorAdicional),
        PTV_Valor = record.Field<PrecioTrayeSubTraValorAdic_TAR, decimal>(f => f.PTV_Valor),
        PTV_FechaGrabacion = record.Field<PrecioTrayeSubTraValorAdic_TAR, DateTime>(f => f.PTV_FechaGrabacion),
        PTV_CreadoPor = record.Field<PrecioTrayeSubTraValorAdic_TAR, string>(f => f.PTV_CreadoPor),
        PTV_FechaCambio = DateTime.Now,
        PTV_CambiadoPor = ControllerContext.Current.Usuario,
        PTV_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioTraSubTraVlrAdiHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio trámite
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioTramite(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioTramite_TAR, PrecioTramiteHist_TAR>((record, action) => new PrecioTramiteHist_TAR()
      {
        PRT_IdPrecioTramite = record.Field<PrecioTramite_TAR, int>(f => f.PRT_IdPrecioTramite),
        PRT_IdTramite = record.Field<PrecioTramite_TAR, int>(f => f.PRT_IdTramite),
        PRT_IdListaPrecioServicio = record.Field<PrecioTramite_TAR, int>(f => f.PRT_IdListaPrecioServicio),
        PRT_Valor = record.Field<PrecioTramite_TAR, decimal>(f => f.PRT_Valor),
        PRT_ValorAdicionalLocal = record.Field<PrecioTramite_TAR, decimal>(f => f.PRT_ValorAdicionalLocal),
        PRT_ValorAdicionalPorDocumento = record.Field<PrecioTramite_TAR, decimal>(f => f.PRT_ValorAdicionalPorDocumento),
        PRT_FechaGrabacion = record.Field<PrecioTramite_TAR, DateTime>(f => f.PRT_FechaGrabacion),
        PRT_CreadoPor = record.Field<PrecioTramite_TAR, string>(f => f.PRT_CreadoPor),
        PRT_FechaCambio = DateTime.Now,
        PRT_CambiadoPor = ControllerContext.Current.Usuario,
        PRT_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioTramiteHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de precio trayecto rango
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearPrecioTrayectoRango(EntidadesTarifas contexto)
    {
      contexto.Audit<PrecioTrayectoRango_TAR, PrecioTrayectoRangoHist_TAR>((record, action) => new PrecioTrayectoRangoHist_TAR()
      {
        PRR_IdPrecioTrayectoRango = record.Field<PrecioTrayectoRango_TAR, long>(f => f.PRR_IdPrecioTrayectoRango),
        PPR_IdPrecioTrayecto = record.Field<PrecioTrayectoRango_TAR, long>(f => f.PPR_IdPrecioTrayecto),
        PPR_Inicial = record.Field<PrecioTrayectoRango_TAR, decimal>(f => f.PPR_Inicial),
        PPR_Final = record.Field<PrecioTrayectoRango_TAR, decimal>(f => f.PPR_Final),
        PPR_Valor = record.Field<PrecioTrayectoRango_TAR, decimal>(f => f.PPR_Valor),
        PPR_Porcentaje = record.Field<PrecioTrayectoRango_TAR, decimal>(f => f.PPR_Porcentaje),
        PPR_FechaGrabacion = record.Field<PrecioTrayectoRango_TAR, DateTime>(f => f.PPR_FechaGrabacion),
        PPR_CreadoPor = record.Field<PrecioTrayectoRango_TAR, string>(f => f.PPR_CreadoPor),
        PPR_FechaCambio = DateTime.Now,
        PPR_CambiadoPor = ControllerContext.Current.Usuario,
        PPR_TipoCambio = action.ToString(),
      }, (ph) => contexto.PrecioTrayectoRangoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de servicio forma pago
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearServicioFormaPago(EntidadesTarifas contexto)
    {
      contexto.Audit<ServicioFormaPago_TAR, ServicioFormaPagoHist_TAR>((record, action) => new ServicioFormaPagoHist_TAR()
      {
        SFP_IdServicio = record.Field<ServicioFormaPago_TAR, int>(f => f.SFP_IdServicio),
        SFP_IdFormaPago = record.Field<ServicioFormaPago_TAR, short>(f => f.SFP_IdFormaPago),
        SFP_FechaGrabacion = record.Field<ServicioFormaPago_TAR, DateTime>(f => f.SFP_FechaGrabacion),
        SFP_CreadoPor = record.Field<ServicioFormaPago_TAR, string>(f => f.SFP_CreadoPor),
        SFP_FechaCambio = DateTime.Now,
        SFP_CambiadoPor = ControllerContext.Current.Usuario,
        SFP_TipoCambio = action.ToString(),
      }, (ph) => contexto.ServicioFormaPagoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de requisito trámite
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearRequistitoTramite(EntidadesTarifas contexto)
    {
      contexto.Audit<RequisitoTramite_TAR, RequisitoTramiteHist_TAR>((record, action) => new RequisitoTramiteHist_TAR()
      {
        REQ_IdRequisitoServicio = record.Field<RequisitoTramite_TAR, short>(f => f.REQ_IdRequisitoServicio),
        REQ_Descripcion = record.Field<RequisitoTramite_TAR, string>(f => f.REQ_Descripcion),
        REQ_IdTramite = record.Field<RequisitoTramite_TAR, int>(f => f.REQ_IdTramite),
        REQ_FechaGrabacion = record.Field<RequisitoTramite_TAR, DateTime>(f => f.REQ_FechaGrabacion),
        REQ_CreadoPor = record.Field<RequisitoTramite_TAR, string>(f => f.REQ_CreadoPor),
        REQ_FechaCambio = DateTime.Now,
        REQ_CambiadoPor = ControllerContext.Current.Usuario,
        REQ_TipoCambio = action.ToString(),
      }, (ph) => contexto.RequisitoTramiteHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de centros de correspondencia
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearCentrosCorrespondencia(EntidadesTarifas contexto)
    {
      contexto.Audit<ServicioCentroCorrespondencia_TAR, ServicioCtroCorrespondHist_TAR>((record, action) => new ServicioCtroCorrespondHist_TAR()
      {
        SCC_IdServCentroCorrespondencia = record.Field<ServicioCentroCorrespondencia_TAR, int>(f => f.SCC_IdServCentroCorrespondencia),
        SCC_IdServicio = record.Field<ServicioCentroCorrespondencia_TAR, int>(f => f.SCC_IdServicio),
        SCC_Concepto = record.Field<ServicioCentroCorrespondencia_TAR, string>(f => f.SCC_Concepto),
        SCC_Descripcion = record.Field<ServicioCentroCorrespondencia_TAR, string>(f => f.SCC_Descripcion),
        SCC_FechaGrabacion = record.Field<ServicioCentroCorrespondencia_TAR, DateTime>(f => f.SCC_FechaGrabacion),
        SCC_CreadoPor = record.Field<ServicioCentroCorrespondencia_TAR, string>(f => f.SCC_CreadoPor),
        SCC_FechaCambio = DateTime.Now,
        SCC_CambiadoPor = ControllerContext.Current.Usuario,
        SCC_TipoCambio = action.ToString(),
      }, (ph) => contexto.ServicioCtroCorrespondHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de servicio mensajeria
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearServicioMensajeria(EntidadesTarifas contexto)
    {
      contexto.Audit<ServicioMensajeria_TAR, ServicioMensajeriaHist_TAR>((record, action) => new ServicioMensajeriaHist_TAR()
      {
        SME_IdServicio = record.Field<ServicioMensajeria_TAR, int>(f => f.SME_IdServicio),
        SME_PesoMínimo = record.Field<ServicioMensajeria_TAR, decimal>(f => f.SME_PesoMínimo),
        SME_PesoMaximo = record.Field<ServicioMensajeria_TAR, decimal>(f => f.SME_PesoMaximo),
        SME_FechaGrabacion = record.Field<ServicioMensajeria_TAR, DateTime>(f => f.SME_FechaGrabacion),
        SME_CreadoPor = record.Field<ServicioMensajeria_TAR, string>(f => f.SME_CreadoPor),
        SME_FechaCambio = DateTime.Now,
        SME_CambiadoPor = ControllerContext.Current.Usuario,
        SME_TipoCambio = action.ToString(),
      }, (ph) => contexto.ServicioMensajeriaHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de trámite
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearTramite(EntidadesTarifas contexto)
    {
      contexto.Audit<Tramite_TAR, TramiteHist_TAR>((record, action) => new TramiteHist_TAR()
      {
        TRA_IdTramite = record.Field<Tramite_TAR, int>(f => f.TRA_IdTramite),
        TRA_IdServicio = record.Field<Tramite_TAR, int>(f => f.TRA_IdServicio),
        TRA_IdTipoTramite = record.Field<Tramite_TAR, short>(f => f.TRA_IdTipoTramite),
        TRA_Nombre = record.Field<Tramite_TAR, string>(f => f.TRA_Nombre),
        TRA_Descripcion = record.Field<Tramite_TAR, string>(f => f.TRA_Descripcion),
        TRA_Duracion = record.Field<Tramite_TAR, decimal>(f => f.TRA_Duracion),
        TRA_FechaGrabacion = record.Field<Tramite_TAR, DateTime>(f => f.TRA_FechaGrabacion),
        TRA_CreadoPor = record.Field<Tramite_TAR, string>(f => f.TRA_CreadoPor),
        TRA_FechaCambio = DateTime.Now,
        TRA_CambiadoPor = ControllerContext.Current.Usuario,
        TRA_TipoCambio = action.ToString(),
      }, (ph) => contexto.TramiteHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de servicio trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearServicioTrayecto(EntidadesTarifas contexto)
    {
      contexto.Audit<ServicioTrayecto_TAR, ServicioTrayectoHist_TAR>((record, action) => new ServicioTrayectoHist_TAR()
      {
        STR_IdTrayecto = record.Field<ServicioTrayecto_TAR, long>(f => f.STR_IdTrayecto),
        STR_IdServicio = record.Field<ServicioTrayecto_TAR, int>(f => f.STR_IdServicio),
        STR_TiempoEntrega = record.Field<ServicioTrayecto_TAR, int>(f => f.STR_TiempoEntrega),
        STR_FechaGrabacion = record.Field<ServicioTrayecto_TAR, DateTime>(f => f.STR_FechaGrabacion),
        STR_CreadoPor = record.Field<ServicioTrayecto_TAR, string>(f => f.STR_CreadoPor),
        STR_FechaCambio = DateTime.Now,
        STR_CambiadoPor = ControllerContext.Current.Usuario,
        STR_TipoCambio = action.ToString(),
      }, (ph) => contexto.ServicioTrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de tipo trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearTipoTrayecto(EntidadesTarifas contexto)
    {
      contexto.Audit<TipoTrayecto_TAR, TipoTrayectoHist_TAR>((record, action) => new TipoTrayectoHist_TAR()
      {
        TTR_IdTipoTrayecto = record.Field<TipoTrayecto_TAR, string>(f => f.TTR_IdTipoTrayecto),
        TTR_Descripcion = record.Field<TipoTrayecto_TAR, string>(f => f.TTR_Descripcion),
        TTR_FechaGrabacion = record.Field<TipoTrayecto_TAR, DateTime>(f => f.TTR_FechaGrabacion),
        TTR_CreadoPor = record.Field<TipoTrayecto_TAR, string>(f => f.TTR_CreadoPor),
        TTR_FechaCambio = DateTime.Now,
        TTR_CambiadoPor = ControllerContext.Current.Usuario,
        TTR_TipoCambio = action.ToString(),
      }, (ph) => contexto.TipoTrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de tipo trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearTipoSubTrayecto(EntidadesTarifas contexto)
    {
      contexto.Audit<TipoSubTrayecto_TAR, TipoSubTrayectoHist_TAR>((record, action) => new TipoSubTrayectoHist_TAR()
      {
        TST_IdTipoSubTrayecto = record.Field<TipoSubTrayecto_TAR, string>(f => f.TST_IdTipoSubTrayecto),
        TST_Descripcion = record.Field<TipoSubTrayecto_TAR, string>(f => f.TST_Descripcion),
        TST_FechaGrabacion = record.Field<TipoSubTrayecto_TAR, DateTime>(f => f.TST_FechaGrabacion),
        TST_CreadoPor = record.Field<TipoSubTrayecto_TAR, string>(f => f.TST_CreadoPor),
        TST_FechaCambio = DateTime.Now,
        TST_CambiadoPor = ControllerContext.Current.Usuario,
        TST_TipoCambio = action.ToString(),
      }, (ph) => contexto.TipoSubTrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria de tipo trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearTipoValorAdicional(EntidadesTarifas contexto)
    {
      contexto.Audit<TipoValorAdicional_TAR, TipoValorAdicionalHist_TAR>((record, action) => new TipoValorAdicionalHist_TAR()
      {
        TVA_IdTipoValorAdicional = record.Field<TipoValorAdicional_TAR, string>(f => f.TVA_IdTipoValorAdicional),
        TVA_Descripcion = record.Field<TipoValorAdicional_TAR, string>(f => f.TVA_Descripcion),
        TVA_IdServicio = record.Field<TipoValorAdicional_TAR, int>(f => f.TVA_IdServicio),
        TVA_FechaGrabacion = record.Field<TipoValorAdicional_TAR, DateTime>(f => f.TVA_FechaGrabacion),
        TVA_CreadoPor = record.Field<TipoValorAdicional_TAR, string>(f => f.TVA_CreadoPor),
        TVA_FechaCambio = DateTime.Now,
        TVA_CambiadoPor = ControllerContext.Current.Usuario,
        TVA_TipoCambio = action.ToString(),
      }, (ph) => contexto.TipoValorAdicionalHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearTrayecto(EntidadesTarifas contexto)
    {
      contexto.Audit<Trayecto_TAR, TrayectoHist_TAR>((record, action) => new TrayectoHist_TAR()
      {
        TRA_IdTrayecto = record.Field<Trayecto_TAR, long>(f => f.TRA_IdTrayecto),
        TRA_IdLocalidadOrigen = record.Field<Trayecto_TAR, string>(f => f.TRA_IdLocalidadOrigen),
        TRA_IdLocalidadDestino = record.Field<Trayecto_TAR, string>(f => f.TRA_IdLocalidadDestino),
        TRA_IdTrayectoSubTrayecto = record.Field<Trayecto_TAR, int>(f => f.TRA_IdTrayectoSubTrayecto),
        TRA_FechaGrabacion = record.Field<Trayecto_TAR, DateTime>(f => f.TRA_FechaGrabacion),
        TRA_CreadoPor = record.Field<Trayecto_TAR, string>(f => f.TRA_CreadoPor),
        TRA_FechaCambio = DateTime.Now,
        TRA_CambiadoPor = ControllerContext.Current.Usuario,
        TRA_TipoCambio = action.ToString(),
      }, (ph) => contexto.TrayectoHist_TAR.Add(ph));
    }

    /// <summary>
    /// Guarda la auditoria trayecto
    /// </summary>
    /// <param name="contexto">Contexto</param>
    internal static void MapearTipoEnvio(EntidadesTarifas contexto)
    {
      contexto.Audit<TipoEnvio_TAR, TipoEnvioHist_TAR>((record, action) => new TipoEnvioHist_TAR()
      {
        TEN_IdTipoEnvio = record.Field<TipoEnvio_TAR, short>(f => f.TEN_IdTipoEnvio),
        TEN_Nombre = record.Field<TipoEnvio_TAR, string>(f => f.TEN_Nombre),
        TEN_Descripcion = record.Field<TipoEnvio_TAR, string>(f => f.TEN_Descripcion),
        TEN_PesoMinimo = record.Field<TipoEnvio_TAR, decimal>(f => f.TEN_PesoMinimo),
        TEN_PesoMaximo = record.Field<TipoEnvio_TAR, decimal>(f => f.TEN_PesoMaximo),
        TEN_CodigoMinisterio = record.Field<TipoEnvio_TAR, decimal>(f => f.TEN_CodigoMinisterio),
        TEN_FechaGrabacion = record.Field<TipoEnvio_TAR, DateTime>(f => f.TEN_FechaGrabacion),
        TEN_CreadoPor = record.Field<TipoEnvio_TAR, string>(f => f.TEN_CreadoPor),
        TEN_FechaCambio = DateTime.Now,
        TEN_CambiadoPor = ControllerContext.Current.Usuario,
        TEN_TipoCambio = action.ToString()
      }, (ph) => contexto.TipoEnvioHist_TAR.Add(ph));
    }
  }
}