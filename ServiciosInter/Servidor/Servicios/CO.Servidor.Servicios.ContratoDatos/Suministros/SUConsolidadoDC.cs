using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel.DataAnnotations;

using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    [DataContract(Namespace = "http://interrapidisimo.com")]
    public class SUConsolidadoDC
    {
        [DataMember]
        public long IdCentroServicios { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CentroServicio")]
        public string NombreCentroServicios { get; set; }

        [DataMember]
        public OUTipoConsolidadoDC TipoConsolidado { get; set; }

        [DataMember]
        public OUTipoConsolidadoDetalleDC TipoConsolidadoDetalle { get; set; }

        [DataMember]
        public OUColorTipoConsolidadoDetalleDC Color { get; set; }

        [DataMember]
        public SUTamanoTulaDC Tamano { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Trayecto")]
        public string Trayecto { get; set; }

        // Codigo de la Tula-Contenedor
        [DataMember]
        [Filtrable("INT_Codigo", "Código: ", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 12, MensajeError = "Error en el Campo")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Codigo")]
        public string Codigo { get; set; }

        /// <summary>
        /// Aplica solo cuando es una agencia
        /// </summary>
        [DataMember]
        public string IdLocalidad { get; set; }

        public string NombreLocalidad { get; set; }
    }
}
