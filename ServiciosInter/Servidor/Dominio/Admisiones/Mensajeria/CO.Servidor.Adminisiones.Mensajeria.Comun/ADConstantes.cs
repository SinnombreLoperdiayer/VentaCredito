using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Adminisiones.Mensajeria.Comun
{
  /// <summary>
  /// Contiene información de las constantes del módulo de admisiones mensajería
  /// </summary>
  public class ADConstantes
  {
    public const string TIPO_ENVIO_ENVIAR_A_REMITENTE = "EDR";

    public const string TIPO_ENVIO_RECLAMA_EN_PUNTO = "REP";

    public const string TIPO_ENVIO_ENVIAR_A_NUEVA_DIRECCION = "ENV";

    public const short ID_TIPO_ENVIO_NOTIFICACION = 17;

    public const string TIPO_VALOR_ADICIONAL_EMPAQUE = "BSP";

    public const string NOVEDAD_CAMBIO_DESTINO = "MODIFICACIÓN DESTINO";

    public const string NOVEDAD_CAMBIO_FORMA_PAGO = "MODIFICACIÓN FORMA DE PAGO";

    /// <summary>
    /// Constante de Observacion por el recaudo de
    /// dinero de una Guia al cobro
    /// </summary>
    public const string OBSERVACION_RECAUDO_DINERO_GUIA_ALCOBRO = "Recaudo de Dinero por guia al cobro";

    public const string CAMBIO_ELIMINA = "ELIMINAR";

    public const int ID_FALLA_DIFERENCIA_VALOR_COBRADO = 14;

    public const string ID_UNIDAD_NEGOCIO_MENSAJERIA = "MEN";

    public const int ID_SERVICIO_MENSAJERIA_EXPRESA = 3;

    public const string NOMBRE_SERVICIO_MENSAJERIA_EXPRESA = "Mensajería";

    public const string ID_TIPO_ENTREGA = "1";

    public const string NOMBRE_TIPO_ENTREGA = "";

    public const string REPORTE_EXPLORADOR = "/ExploradorEnvios/ExploradorEnvios.aspx";
  }

}