using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  /// <summary>
  /// Contiene la información de un cargo dentro de la compañía
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASCargo : DataContractBase
  {
    /// <summary>
    /// Identificador del cargo
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cargo", Description = "TooltipCargoTarea")]
    public int IdCargo { get; set; }

    /// <summary>
    /// Descripción del cargo
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cargo", Description = "TooltipCargo")]
    public string Descripcion { get; set; }
  }
}