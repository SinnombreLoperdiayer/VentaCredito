using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Facturacion.Comun
{
  public class FAConstantesFacturacion
  {
      /// <summary>
      ///  Tipo de descripcion de anulacion de factura (valor = 1)
      /// </summary>
      public const int DESCRIPCION_ANULACION_FACTURA = 1;

      /// <summary>
      ///  Tipo de descripcion de factura
      /// </summary>
      public const string DESCRIPCION_FACTURA = "Factura número ";

      /// <summary>
      ///  Remitente guía interna
      /// </summary>
      public const string REMITENTE_GUIAINTERNA = "Facturación/Interrapidisimo";


      /// <summary>
      ///  Direccion guía interna
      /// </summary>
      public const string DIRECCION_GUIAINTERNA = "Carrera 30 no 7 - 45";

      /// <summary>
      ///  Telefono guía interna
      /// </summary>
      public const string TELEFONO_GUIAINTERNA = "7456000";

  }
}
