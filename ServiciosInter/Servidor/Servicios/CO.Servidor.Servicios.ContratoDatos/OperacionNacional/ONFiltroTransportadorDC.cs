using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Rutas;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Contiene la informacion para el filtro del  ingreso y salida del transportador
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONFiltroTransportadorDC
  {
    [DataMember]
    public string Placa { get; set; }

    [DataMember]
    public RURutaDC Ruta { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaInicio", Description = "TooltipFechaInicio")]
    public DateTime FechaInicio { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaFin", Description = "FechaFin")]
    public DateTime FechaFin { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "TooltipIdentificacion")]
    public string IdentificadorConductor { get; set; }

    [DataMember]
    public bool IncluyeFecha { get; set; }
    
    [DataMember]
    public long IdAgenciaIngreso { get; set; }
  }
}