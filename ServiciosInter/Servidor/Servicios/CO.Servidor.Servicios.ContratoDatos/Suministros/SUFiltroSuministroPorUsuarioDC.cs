using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{ 
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUFiltroSuministroPorUsuarioDC
  {
    /// <summary>
    /// Indica si la consulta incluye fecha
    /// </summary>
    [DataMember]
    public bool ConsultaIncluyeFecha { get; set; }

    /// <summary>
    /// fecha inicial de la consulta por defecto deja la actual menos 3 dias
    /// </summary>
    [DataMember]
    public DateTime? FechaInicial { get; set; }

    /// <summary>
    /// fecha final de la consulta por defecto deja la actual
    /// </summary>
    [DataMember]
    public DateTime? FechaFinal { get; set; }
    /// <summary>
    /// Nombre de usuario
    /// </summary>
    [DataMember]
    public string Usuario { get; set; }

    /// <summary>
    /// Id del suministro
    /// </summary>
    [DataMember]
    public int IdSuministro { get; set; }
  }

}
