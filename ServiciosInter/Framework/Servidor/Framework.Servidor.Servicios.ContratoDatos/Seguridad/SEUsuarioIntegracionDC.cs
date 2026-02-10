using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos.Seguridad
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SEUsuarioIntegracionDC : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdUsuarioIntegracion")]
        public int IdUsuario { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioIntegracion")]
        public string Usuario { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ContrasenaIntegracion")]
        public string Contrasena { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdServidorIntegracion")]
        public string IdServidor { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ServidorIntegracion")]
        public string Servidor { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CreadoPorIntegracion")]
        public string CreadoPor { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaGrabacionIntegracion")]
        public DateTime FechaGrabacion { get; set; }
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EstadoIntegracion")]
        public bool Estado { get; set; }
        [DataMember]
        public int idServidorInt { get; set; }

        [DataMember]
        public int idClienteInt { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdClienteIntegracion")]
        public string IdCliente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ClienteIntegracion")]
        public string Cliente { get; set; }

    }

    [DataContract(Namespace = "http://contrologis.com")]
    public class SERespuestaProceso : DataContractBase
    {
        [DataMember]
        public int valor { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
    }

}
