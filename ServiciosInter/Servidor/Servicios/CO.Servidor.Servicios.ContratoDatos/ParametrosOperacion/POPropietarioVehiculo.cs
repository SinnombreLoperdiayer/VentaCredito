using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Comun;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
  /// <summary>
  /// Clase que contiene la informacion los propiestarios de los vehiculos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class POPropietarioVehiculo : DataContractBase
  {
    public event EventHandler OnTipoContratoCambio;

    [DataMember]
    public long IdPropietarioVehiculo { get; set; }

    private int idTipoContrato;

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "ToolTipTipoContratoPropietario")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public int IdTipoContrato
    {
      get { return idTipoContrato; }
      set
      {
        idTipoContrato = value;
        if (OnTipoContratoCambio != null)
          OnTipoContratoCambio(null, null);
      }
    }

    [DataMember]
    public PALocalidadDC PaisCiudad { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudadPropietario")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public PALocalidadDC CiudadPropietario { get; set; }

    [DataMember]
    public PAPersonaExterna PersonaExterna { get; set; }


    /// <summary>
    /// Enumeración que indica el estado del objeto dentro de una lista
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}