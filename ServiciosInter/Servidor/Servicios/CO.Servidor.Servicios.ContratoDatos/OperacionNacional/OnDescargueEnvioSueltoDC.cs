using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Contiene la Informacion de los Envios Sueltos que se asocian
    /// a un Manifiesto o Planilla de despacho
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class OnDescargueEnvioSueltoDC
    {
        /// <summary>
        /// Es el id de ingreso de la guia
        /// </summary>
        [DataMember]
        public long IdIngresoGuia { get; set; }

        /// <summary>
        /// es el numero de Guia que se asocia aun manifiesto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas),
            Name = "IngresarEnvíoSuelto", Description = "IngresarEnvíoSueltoToolTip")]
        public string NumeroGuia { get; set; }

        /// <summary>
        /// Es el numero de Piezas de la Guia
        /// </summary>
        [DataMember]
        public int NumeroPiezas { get; set; }

        /// <summary>
        /// Es el total de Piezas de la Guia
        /// </summary>
        [DataMember]
        public int TotalPiezas { get; set; }

        /// <summary>
        /// List de las Novedades del Envio
        /// </summary>
        [DataMember]
        public List<ONNovedadesEnvioDC> LstNovedadesGuias { get; set; }

        /// <summary>
        /// Ingreso con Numero de Guia
        /// </summary>
        [DataMember]
        public bool IngresoConGuia { get; set; }

        /// <summary>
        /// Es el numero de laAdmision Mensajeria
        /// </summary>
        [DataMember]
        public long IdAdminMensajeria { get; set; }

        /// <summary>
        /// Es le numero del descargue
        /// </summary>
        [DataMember]
        public long NumCtrolTransMan { get; set; }
    }
}