using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Suministros.Comun
{
  /// <summary>
  /// Clase que contiene las constantes de suministros
  /// </summary>
  public class SUConstantesSuministros
  {
    /// <summary>
    ///  Almacena los tipos de numerador que tienen relación con un suministro
    /// </summary>
    public const int SUMINISTROS_GIROS = 3;

    /// <summary>
    /// Constante para la falla generación suministros
    /// </summary>
    public const int FALLA_GUIA_SIN_ASIGNAR = 15;

    /// <summary>
    /// COMPROBANTE DE PAGO DE GIRO MANUAL
    /// </summary>
    public const int SUMINISTROS_COMPROBANTE_PAGO_MANUAL = 33;

    /// <summary>
    /// CONSTANTE PARA EL ID DEL GRUPO DE SUMINISTROS DE MENSAJERO
    /// </summary>
    public const string ID_GRUPO_SUMINISTRO_MENSAJERO = "MEN";

    /// <summary>
    /// CONSTANTE PARA EL VALOR POR DEFECTO EN CERO
    /// </summary>
    public const int VALOR_DEFECTO_CERO = 0;

    /// <summary>
    /// Constante para el id de la remision
    /// </summary>
    public const string PARAMETRO_ID_REMISION = "idRemision";

    /// <summary>
    /// descripcion del reporte del canal de venta
    /// </summary>
    public const string REPORTE_REMISION_CANAL_VENTA = "/Suministro/RemisionCanalVenta.aspx";
    /// <summary>
    /// constante con la descripcion del reporte de proceso
    /// </summary>
    public const string REPORTE_REMISION_PROCESO = "/Suministro/RemisionProceso.aspx";

    /// <summary>
    /// reporte de la remision de una sucursal
    /// </summary>
    public const string REPORTE_REMISION_SUCURSAL = "/Suministro/RemisionSucursal.aspx";

    /// <summary>
    /// reporte de la remision de suministros para un mensajero
    /// </summary>
    public const string REPORTE_REMISION_MENSAJERO = "/Suministro/RemisionMensajero.aspx";

    /// <summary>
    /// Tipo de consecutivo para la resolucion de suministros
    /// </summary>
    public const int ID_TIPO_CONSECUTIVO_RESOLUCION = 8;

    /// <summary>
    /// id para el suministro de guia interna
    /// </summary>
    public const int ID_SUMINISTRO_GUIA_INTERNA = 4;

    /// <summary>
    /// id para la gestion en el proceso de traslado
    /// </summary>
    public const int ID_GESTION_TRASLADO = 9999;

    /// <summary>
    ///  suministros a validar propietario
    /// </summary>
    public const string PAR_SUMINISTROS_VALIDAR= "SuministrosValidar";

    /// <summary>
    /// Id del motivo de cambio de contenedor cuando se trata de "Deshabilitar"
    /// </summary>
    public const short ID_MOTIVO_CAMBIO_CONTENEDOR_DESHABILITAR = 1;

    /// <summary>
    /// Id del motivo de cambio de contendor cuando se trata de "Habilitar"
    /// </summary>
    public const short ID_MOTIVO_CAMBIO_CONTENEDOR_HABILITAR = 2;

    /// <summary>
    /// Id del tipo de consolidado "Tula"
    /// </summary>
    public const short ID_TIPO_CONSOLIDADO_TULA = 1;
  }
}