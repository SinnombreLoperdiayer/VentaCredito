using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// es la clase que muestra la informacion de los
    /// alcobros sin cancelar que estan en transito y que estan
    /// en el col en reparto de acuerdo al tiempo esperado de entrega
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADAlCobrosSinCancelarDC
    {
        /// <summary>
        /// Es al Cantidad de alcobros que estan sin Cancelar
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
        public int CantAlCobrosSinCancelar { get; set; }

        /// <summary>
        /// Es la Sumatoria de los Alcobros sin Cancelar
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
        public decimal ValorAlCobrosSinCancelar { get; set; }

        /// <summary>
        /// Es al Cantidad de alcobros que estan en Transito
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad")]
        public int CantAlCobrosEnTransito { get; set; }

        /// <summary>
        /// Es la Sumatoria de los Alcobros que estan en Transito
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Valor")]
        public decimal ValorAlCobrosEnTransito { get; set; }
    }
}