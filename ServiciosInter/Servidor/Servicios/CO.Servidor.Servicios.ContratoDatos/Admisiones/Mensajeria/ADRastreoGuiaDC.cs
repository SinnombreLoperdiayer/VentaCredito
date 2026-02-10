using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    public class ADRastreoGuiaDC
    {
        /// <summary>
        /// Fecha y Ultimo estado de la guia 
        /// </summary>
        public ADTrazaGuia TrazaGuia { get; set; }
        /// <summary>
        /// Estados de la guía
        /// </summary>
        public List<ADEstadoGuiaMotivoDC> EstadosGuia { get; set; }
        /// <summary>
        /// Remitente, destinatario
        /// </summary>
        public ADGuia Guia { get; set; }
        /// <summary>
        /// Gestión telemercadeo
        /// </summary>
        public List<LIGestionesDC> Telemercadeo { get; set; }
        /// <summary>
        /// Volantes
        /// </summary>
        public List<string> Volantes { get; set; }
        /// <summary>
        /// Novedades de transporte
        /// </summary>
        public List<ONNovedadesTransporteDC> NovedadesTransporte { get; set; }
        /// <summary>
        /// Imagen de la guía 
        /// </summary>
        public string ImagenGuia { get; set; }

        /// <summary>
        /// Es Sispostal
        /// </summary>
        public bool EsSispostal { get; set; }
    }
}
