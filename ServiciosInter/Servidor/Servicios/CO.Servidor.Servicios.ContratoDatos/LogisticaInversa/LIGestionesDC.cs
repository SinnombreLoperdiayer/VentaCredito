using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{

    [DataContract(Namespace = "http://contrologis.com")]
    public class LIGestionesDC : DataContractBase
    {
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NoGestion", Description = "ToolTipNoGestion")]
        public long IdGestion { get; set; }

        [DataMember]
        public long idTrazaGuia { get; set; }

        [DataMember]
        public long idAdmisionGuia { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaGestion", Description = "ToolTipFechaGestion")]
        public DateTime FechaGestion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ResultadoGestion", Description = "ToolTipResultadoGestion")]
        public LIResultadoTelemercadeoDC Resultado { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "TelefonoMarcado", Description = "TooltipTelefono")]
        [StringLength(25, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Telefono { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "TelefonoAdiciona", Description = "TooltipTelefono")]
        [StringLength(25, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NuevoTelefono { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Usuario", Description = "ToolTipUsuarioGestion")]
        [StringLength(50, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Usuario { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Contacto", Description = "ToolTipContacto")]
        [StringLength(25, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NuevoContacto { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "TooltipDireccion")]
        [StringLength(250, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string NuevaDireccion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Contesta", Description = "ToolTipContesta")]
        [StringLength(50, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string PersonaContesta { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Parentesco", Description = "ToolTipParentesco")]
        public PAParienteDC Pariente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
        [StringLength(250, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoLongitud")]
        public string Observaciones { get; set; }

        [DataMember]
        public LIGestionMotivoBorradoDC MotivoBorrado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "AsignarASupervisor", Description = "TooltipAsignarASupervisor")]
        public bool AsignarASupervisor { get; set; }

        //[DataMember]
        //private LIEnumTipoGestionTelemercadeo tipoGestion = LIEnumTipoGestionTelemercadeo.None;
        //[DataMember]
        //public LIEnumTipoGestionTelemercadeo TipoGestion
        //{
        //    get { return tipoGestion; }
        //    set { tipoGestion = value; }
        //}

        [DataMember]
        public LIEnumTipoGestionTelemercadeo TipoGestion { get; set; }

        [DataMember]
        public long IdCentroServicio { get; set; }

        [DataMember]
        public string Idmensajero { get; set; }

        [DataMember]
        public string NombreMensajero { get; set; }

        [DataMember]
        public long IdPlanilla { get; set; }
    }
}