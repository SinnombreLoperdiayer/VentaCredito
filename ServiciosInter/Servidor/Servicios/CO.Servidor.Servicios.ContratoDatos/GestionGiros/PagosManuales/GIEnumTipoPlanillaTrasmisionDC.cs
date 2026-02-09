using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales
{
  [DataContract(Namespace = "http://contrologis.com")]
  public enum GIEnumTipoPlanillaTrasmisionDC : int
  {
    [EnumMember]
    NO_CONFIGURADO = 0,
    /// <summary>
    /// Planilla para los giros que se trasmiten via telefonica
    /// </summary>
    [EnumMember]
    PLANILLA_TELEFONO = 1,

    /// <summary>
    /// Planilla para los giros que se trasmite via fax
    /// </summary>
    [EnumMember]
    PLANILLA_FAX = 2
  }
}