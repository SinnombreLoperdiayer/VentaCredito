using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUAsignacionDC : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Recogida", Description = "Recogida")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("ATP_IdAsignacionTula")]
        public long IdAsignacion { get; set; }


        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public PUCentroServiciosDC CentroServicioOrigen { get; set; }


        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public PUCentroServiciosDC CentroServicioDestino { get; set; }
      
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoAsignacion", Description = "TipoAsignacion")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public OUTipoAsignacionDC TipoAsignacion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroTula", Description = "NumeroTula")]
        public string NoTula { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroPrecinto", Description = "NumeroPrecinto")]
        public long NoPrecinto { get; set; }

        [DataMember]
        public string  Estado { get; set; }


        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ControlManifiesto", Description = "ControlManifiesto")]
        public long NumContTransDespacho { get; set; }
  
        [DataMember]
        public long? NumContTransRetorno { get; set; }      
       
        /// <summary>
        /// retorna o asigna la fecha del sistema
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaCreacion", Description = "FechaCreacion")]
        public DateTime FechaCreacion { get; set; }


        /// <summary>
        /// retorna o asigna el usuario 
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }


        public event EventHandler OnSeleccionadoChange;

        private bool seleccionar;

        [IgnoreDataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Seleccionar", Description = "Seleccionar")]
        public bool Seleccionar
        {
            get { return seleccionar; }
            set { seleccionar = value;
            if (OnSeleccionadoChange != null)
                OnSeleccionadoChange(this, null);
            }
        }

        /// <summary>
        /// retorna o asigna la fecha del sistema
        /// </summary>
        [DataMember]
        public OUMensajeroDC Mensajero { get; set; }
        /// <summary>
        /// Indica el total de piezas de un envio asociado a una asignacion
        /// </summary>
        [IgnoreDataMember]
        public int? TotalPiezas { get; set; }
        [IgnoreDataMember]
        public long IdAdmisionMensajeria { get; set; }
        [DataMember]
        public string NumeroGuiaRotulo { get; set; }
    }
}
