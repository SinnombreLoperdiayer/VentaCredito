using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUPlanillaAsignacionDC : DataContractBase
    {
        /// <summary>
        /// retorna o asigna la informacion del mensajero para la planilla
        /// </summary>
        [DataMember]
        public OUMensajeroDC Mensajero { get; set; }

        /// <summary>
        /// Retorna o asigna el id de la agencia tipo COL
        /// </summary>
        [DataMember]
        public long IdAgencia { get; set; }

        /// <summary>
        /// Retorna o asigna el id del COL
        /// </summary>
        [DataMember]
        public long IdCentroLogistico { get; set; }

        /// <summary>
        /// Usuario que creo la planilla
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }

        /// <summary>
        /// Fecha de creación de la planilla
        /// </summary>
        [DataMember]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Retorna o asigna el id de la planilla de asignación
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
        [Filtrable("PAE_IdPlanillaAsignacionEnvio", "Planilla: ", COEnumTipoControlFiltro.TextBox)]
        [CamposOrdenamiento("PAE_IdPlanillaAsignacionEnvio")]
        public long IdPlanillaAsignacion { get; set; }

        /// <summary>
        /// Retorna o asigna el total de guias de la planilla
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalEnviosPlanillados", Description = "TotalEnviosPlanillados")]
        public int TotalGuias { get; set; }

        /// <summary>
        /// retorna o asigna el id del municipio de la asignacion del envio
        /// </summary>
        [DataMember]
        public string IdMunicipioAsignacion { get; set; }

        /// <summary>
        /// retorna o asigna el id del estado de la planilla de asignacion
        /// </summary>
        [DataMember]
        public string IdEstadoPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la planilla
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado", Description = "Estado")]
        [CamposOrdenamiento("PAE_IdPlanillaAsignacionEnvio")]
        public string EstadoPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna el id del estado de la planilla de asignacion
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
        public string Ciudad { get; set; }

        /// <summary>
        /// retorna o asigna el id del estado de la planilla de asignacion
        /// </summary>
        [DataMember]
        public string IdCiudad { get; set; }

        /// <summary>
        /// retorna o asigna la fecha de asignacion de la planilla
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha", Description = "Fecha")]
        [CamposOrdenamiento("PAE_FechaGrabacion")]
        public DateTime FechaAsignacion { get; set; }

        [DataMember]
        public OUGuiaIngresadaDC Guias { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public PAZonaDC Zona { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RequiereAuditoria", Description = "RequiereAuditoria")]
        public bool RequiereAuditoria { get; set; }

        /// <summary>
        /// identifica si la agencia donde se esta haciendo la asignacion es col o agencia
        /// </summary>
        [DataMember]
        public bool EsCol { get; set; }

        /// <summary>
        /// retorna un mensaje en dado caso que el numerod e asignaciones en paramtros sea superado
        /// </summary>
        [DataMember]
        public string MensajeAdvertenciaCantidadAsignacion { get; set; }

        /// <summary>
        /// Propiedad para validar si la planilla es para auditores
        /// </summary>
        private bool esAuditoria = false;

        [DataMember]
        public bool EsAuditoria
        {
            get { return esAuditoria; }
            set { esAuditoria = value; }
        }
    }
}