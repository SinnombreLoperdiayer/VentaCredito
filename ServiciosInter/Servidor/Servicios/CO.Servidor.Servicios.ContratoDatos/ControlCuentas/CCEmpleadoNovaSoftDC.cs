using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CCEmpleadoNovaSoftDC : DataContractBase
  {
    [DataMember]
    public string Identificacion { get; set; }
    [DataMember]
    public string Correo { get; set; }
    [DataMember]
    public string Cargo { get; set; }

    [DataMember]
    public string Nombre { get; set; }

    [DataMember]
    public string PrimerApellido { get; set; }

    [DataMember]
    public string SegundoApellido { get; set; }

    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NombreCompleto", Description = "ToolTipNombreCompleto")]
    public string NombreApellidos
    {
      get
      {
        return Nombre + " " + PrimerApellido + " " + SegundoApellido;
      }
    }
  }
}