using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    /// <summary>
    /// Contiene la Informacion para cargar
    /// la informacion Inicial de la pantalla
    /// Descargue de Consolidados Urbano-Regional-Nacional
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONDescargueConsolidadosInfoInicialDC
    {
        /// <summary>
        /// es la fuente de la pantalla inicial
        /// </summary>
        [DataMember]
        public List<ONNovedadesEnvioDC> LstNovedadesEnvioGuia { get; set; }

        /// <summary>
        /// Es la fuente de la pantalla inicial
        /// </summary>
        [DataMember]
        public List<ONNovedadesConsolidadoDC> LstNovedadesConsolidados { get; set; }
    }
}