using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Agenda
{
    /// <summary>
    /// Enumeración asociada el evento de asignación de tareas
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public enum ASEEventoAsignacion : int
    {
        /// <summary>
        /// Creación de la tarea
        /// </summary>
        [EnumMember]
        Creacion = 0,

        /// <summary>
        /// Cierre de la tarea
        /// </summary>
        [EnumMember]
        Cierre = 1,

        /// <summary>
        /// En proceso de seguimiento de la tarea
        /// </summary>
        [EnumMember]
        Seguimiento = 2,

        /// <summary>
        /// La tarea ha sido asignada por escalamiento de otra tarea
        /// </summary>
        [EnumMember]
        Escalamiento = 3
    }
}