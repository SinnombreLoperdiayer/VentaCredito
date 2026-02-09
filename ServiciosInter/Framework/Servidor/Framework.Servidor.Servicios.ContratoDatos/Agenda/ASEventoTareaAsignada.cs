using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
    /// <summary>
    /// Contiene información de un evento de seguimiento de una tarea asignada, una tarea asignada tiene uno o muchos eventos de seguimiento asignados
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ASEventoTareaAsignada : DataContractBase
    {
        /// <summary>
        /// Fecha de la asignación de la tarea
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Evento de asignación
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EventoAsignacion")]
        public ASEEventoAsignacion EventoAsignacion { get; set; }

        /// <summary>
        /// Comentarios de la tarea
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Comentarios")]
        public string Comentarios { get; set; }
    }
}