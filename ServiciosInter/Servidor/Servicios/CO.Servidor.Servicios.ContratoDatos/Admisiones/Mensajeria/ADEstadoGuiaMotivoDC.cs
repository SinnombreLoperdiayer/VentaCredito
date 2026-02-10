using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADEstadoGuiaMotivoDC
    {
        /// <summary>
        /// retorna o asigna el id de la traza del cambio de estado
        /// </summary>
        [DataMember]
        public long? IdTrazaGuia { get; set; }

        /// <summary>
        /// retorna o asigna el motivo del cambio de estado
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Motivo")]
        public ADMotivoGuiaDC Motivo { get; set; }


        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        [DataMember]
        public ADTrazaGuia EstadoGuia { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        [DataMember]
        public string Observaciones { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        [DataMember]
        public DateTime FechaMotivo { get; set; }

        public int TipoContador { get; set; }
        public string NumeroContador { get; set; }
        public int TipoPredio { get; set; }
        public string DescripcionPredio { get; set; }

    }
}