using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class SUFiltroSuministroPorRemisionDC
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
    /// Usuario que realizo la remision
    /// </summary>
    [DataMember]
    public string Usuario { get; set; }

    /// <summary>
    /// numero de la remision inicial
    /// </summary>
    [DataMember]
    public long? RemisionInicial { get; set; }

    /// <summary>
    /// numero de la remision final
    /// </summary>
    [DataMember]
    public long? RemisionFinal { get; set; }

    /// <summary>
    /// id del suministro a filtrar
    /// </summary>
    [DataMember]
    public int IdSuministro { get; set; }

    /// <summary>
    /// Ciudad de destino de la localidad
    /// </summary>
    [DataMember]
    public string IdCiudad { get; set; }
  }
}