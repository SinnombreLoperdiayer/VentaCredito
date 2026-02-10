using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Servidor.ParametrosFW
{
  public static class PAConstantesParametros
  {
    /// <summary>
    /// Nombre como quedaran las imagenes despues de ser scanneadas
    /// </summary>
    public static int ALERTA_LISTA_RESTRICTIVA = 1;

    /// <summary>
    /// Constante para indicar la alerta de los centros de servicio
    /// </summary>
    public static int ALERTA_DIVULGACION_CENTRO_SERVICIOS = 3;

    /// <summary>
    /// Constante para indicar la alerta de los clientes
    /// </summary>
    public static int ALERTA_DIVULGACION_CLIENTE = 4;

    /// <summary>
    /// Constante para indica que la agencia supero los topes maximos
    /// </summary>
    public static int ALERTA_AGENCIA_TOPES_MAX = 5;

    /// <summary>
    /// Id tipo consecutivo cajas pruebas de entrega
    /// </summary>
    public const short ID_TIPO_CONSECUTIVO_CAJAS_PRUEBAS_ENTREGA = 5;

    /// <summary>
    /// Id tipo consecutivo cajas de contabilidad
    /// </summary>
    public const short ID_TIPO_CONSECUTIVO_CAJAS_CONTABILIDAD = 6;

    /// <summary>
    /// Es el id de la zona General
    /// </summary>
    public const string ZONA_GENERAL = "-1";

    /// <summary>
    /// Constante para indicar la alerta de telemercadeo para giros remitente
    /// </summary>
    public static int ALERTA_TELEMERCADEO_GIRO_REMITENTE = 6;

    /// <summary>
    /// Constante para indicar la alerta de telemercadeo para giros destinatario
    /// </summary>
    public static int ALERTA_TELEMERCADEO_GIRO_DESTINATARIO = 7;

    /// <summary>
    /// Constante para indicar la alerta de telemercadeo para envios remitente que pasan a custodia
    /// </summary>
    public static int ALERTA_TELEMERCADEO_ENVIO_REMITENTE = 8;

    /// <summary>
    /// Constante para indicar la alerta de telemercadeo para envios remitente que pasan a custodia
    /// </summary>
    public static int ALERTA_DESCARGUE_DEVOLUCION = 9;

    /// <summary>
    /// Constante para indicar la alerta de telemercadeo para envios remitente que pasan a custodia
    /// </summary>
    public static int ALERTA_ENTREGA_EXITOSA = 10;

    /// <summary>
    /// parametro con la direccion del correo electronico
    /// </summary>
    public const string PARAMETRO_CORREO_ELECTRONICO = "CorreoElectronico";

    /// <summary>
    /// parametro con la direccion del correo electronico
    /// </summary>
    public const string PARAMETRO_NOMBRE_DESTINATARIO = "NombreDestinatario";


    /// <summary>
    /// Constante para indicar la alerta de  pago de un giro
    /// </summary>
    public static int ALERTA_PAGO_GIRO = 11;
  }
}