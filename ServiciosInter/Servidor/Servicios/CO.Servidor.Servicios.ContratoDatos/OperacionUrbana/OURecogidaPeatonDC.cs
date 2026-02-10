using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OURecogidaPeatonDC : DataContractBase
    {
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoIdentificacion", Description = "TipoIdentificacion")]
        public PATipoIdentificacion TipoIdentificacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Documento", Description = "Documento")]
        public string DocumentoCliente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "Telefono")]
        public string TelefonoCliente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "Direccion")]
        public string DireccionCliente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCliente", Description = "NombreCliente")]
        public string NombreCliente { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
        public string MunicipioDescripcion { get; set; }

        [DataMember]
        public string IdMunicipio { get; set; }

        /// <summary>
        /// Retorna o asigna los envios solicitados en la recogida
        /// </summary>
        [DataMember]
        public List<OUEnviosRecogidaPeatonDC> EnviosRecogida { get; set; }

        [DataMember]
        public OUEnviosRecogidaPeatonDC EnvioRecogida { get; set; }
        [DataMember]
        public string Celular { get; set; }
        [DataMember]
        public string Email { get; set; }
    }
}