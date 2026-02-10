using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos
{
  /// <summary>
  /// Enumeración con los consecutivos configurados en el sistema
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public enum PAEnumConsecutivos : int
  {
    [EnumMember]
    No_Aplica = 0,

    [EnumMember]
    Solicitudes_Giros = 1,

    [EnumMember]
    Manifiesto_Ministerio = 2,

    [EnumMember]
    Planilla_prog_de_recogida = 3,

    [EnumMember]
    Cajas_Contabilidad_Guias = 4,

    [EnumMember]
    Cajas_PruebasEntrega = 5,

    [EnumMember]
    Comprobante_Egreso = 6,

    [EnumMember]
    Comprobante_Ingreso = 7,

    [EnumMember]
    Resolucion_de_suministros = 8,

    [EnumMember]
    Documento_caja_GRI_R10 = 9,

    [EnumMember]
    Cajas_Contabilidad_Giros = 10,

    [EnumMember]
    Cajas_Contabilidad_Movimiento_Cajas = 11,

    [EnumMember]
    Cajas_Contabilidad_Pago_Giros = 13,

    [EnumMember]
    Planilla_transmision_giros = 14,

    [EnumMember]
    Acta_disposicion_final = 15,

    [EnumMember]
    Control_Transporte_Manifiesto_Urbano_Despacho = 16,

    [EnumMember]
     Control_Transporte_Manifiesto_Urbano_Retorno = 17,

   [EnumMember]
   Manifiesto_Operacion_Nacional = 18,

   [EnumMember]
   Control_Transporte_Manifiesto_Nacional_Despacho =19,
   
   [EnumMember]
   Control_Transporte_Manifiesto_Nacional_Retorno=20


  }
}