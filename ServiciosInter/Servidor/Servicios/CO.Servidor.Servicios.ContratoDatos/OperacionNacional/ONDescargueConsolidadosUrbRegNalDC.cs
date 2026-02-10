using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// contiene la Informacion para la pantalla
    /// de Descargue de Consolidados Urbano-Regional-Nacional
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONDescargueConsolidadosUrbRegNalDC
    {
        /// <summary>
        /// Es el numero de control del Manifiesto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CtrolTranMan")]
        public long CtrolManifiesto { get; set; }

        /// <summary>
        /// Es el numero del Precinto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPrecinto")]
        public long NumPrecinto { get; set; }

        /// <summary>
        /// Es el numero de la tula
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroTula")]
        public string NumTula { get; set; }

        /// <summary>
        /// es verdadero cuando el Manifiesto es Manual
        /// </summary>
        [DataMember]
        public bool EsmanifiestoManual { get; set; }

        /// <summary>
        /// Es el id de centro de servicio que realiza el descargue
        /// </summary>
        [DataMember]
        public long IdCentroServicio { get; set; }

        /// <summary>
        /// List Novedades de Este Consolidado
        /// </summary>
        [DataMember]
        public List<ONNovedadesConsolidadoDC> ListNovedadConsolidado { get; set; }

        /// <summary>
        /// Numero de Guia que se asocia a un manifiesto o planilla
        /// </summary>
        [DataMember]
        public OnDescargueEnvioSueltoDC IngresarEnvioSuelto { get; set; }

        /// <summary>
        /// es la lista de las guias ingresadas asociadas
        /// a un manifiesto ó planilla de despacho
        /// </summary>
        [DataMember]
        public List<OnDescargueEnvioSueltoDC> ListGuiasIngresadas { get; set; }

        [DataMember]
        public string EstadoAsignacion { get; set; }

        [DataMember]
        public bool EstaDescargadoManifiesto { get; set; }
    }
}