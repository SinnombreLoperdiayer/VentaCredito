using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ASTareaPorAgenda : DataContractBase
  {
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Comentarios", Description = "TooltipComentariosAsignacionTareaManual")]
    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [DataMember]
    public string Comentario { get; set; }

    [Required(ErrorMessageResourceType = typeof(Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TiempoRespuesta", Description = "TooltipTiempoRespuesta")]
    public int TiempoRespuesta { get; set; }

    [DataMember]
    public ASEEventoAsignacion EventoAsignacion { get; set; }

    [DataMember]
    public List<string> Archivos { get; set; }

    [DataMember]
    public string UsuarioResponsable { get; set; }
  }
}