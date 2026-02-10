using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene la informacion utilizada para la divulgacion
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PADivulgacion : DataContractBase
  {
    public event EventHandler OnEmailCambio;

    [DataMember]
    public ObservableCollection<PAGrupoAlerta> Grupos { get; set; }

    [DataMember]
    public ObservableCollection<string> DestinatariosAdicionales { get; set; }

    [DataMember]
    public string Asunto { get; set; }

    [DataMember]
    public string TemplateMensaje { get; set; }

    private string email;

    [IgnoreDataMember]
    [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}", ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "EmailNoValido")]
    [Display(ResourceType = typeof(Etiquetas), Name = "Email", Description = "Tooltipemail")]
    [StringLength(100, ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Email
    {
      get { return email; }
      set
      {
        email = value;
        if (OnEmailCambio != null)
          OnEmailCambio(null, null);
      }
    }
    [DataMember]
    public string ObservacionesGenerales { get; set; }
  }
}