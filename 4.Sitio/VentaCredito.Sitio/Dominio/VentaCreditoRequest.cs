using Servicio.Entidades.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using VentaCredito.Sitio.Comun;

namespace VentaCredito.Sitio.Dominio
{
    [DataContract(Namespace = "http://interrapidisimo.com")]
    public class VentaCreditoRequest
    {
   
        [DataMember]
        public ADGuia Guia { get; set; }

        [DataMember]
        public int IdCaja { get; set; }

        [DataMember]
        public ADMensajeriaTipoCliente RemitenteDestinatario { get; set; }
        public bool ImprimeGuia { get; set; }
    }
}