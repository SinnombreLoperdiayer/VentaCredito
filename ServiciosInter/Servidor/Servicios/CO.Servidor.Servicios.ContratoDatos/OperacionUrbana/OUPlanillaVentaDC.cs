using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUPlanillaVentaDC : DataContractBase
    {
        /// <summary>
        /// retorna o asigna nombre y apellido
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Mensajero", Description = "Mensajero")]
        [Filtrable("NombreCompleto", "Nombre: ", COEnumTipoControlFiltro.TextBox)]
        [CamposOrdenamiento("NombreCompleto")]
        public string NombreCompleto { get; set; }

        /// <summary>
        /// retorna o asigna el tipo de mensajero
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoMensajero", Description = "TipoMensajero")]
        [CamposOrdenamiento("TIM_Descripcion")]
        public string TipoMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el número de la planilla venta
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Planilla", Description = "Planilla")]
        [CamposOrdenamiento("PLA_IdPlanilla")]
        [Filtrable("PLA_IdPlanilla", "Planilla: ", COEnumTipoControlFiltro.TextBox)]
        public long NumeroPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna la fecha de grabación de la planilla
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha", Description = "Fecha")]
        [CamposOrdenamiento("PLA_FechaGrabacion")]
        public string FechaPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna el id del tipo de mensajero
        /// </summary>
        [DataMember]
        public int IdTipoMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el id del mensajero
        /// </summary>
        [DataMember]
        public long IdMensajero { get; set; }

        /// <summary>
        /// retorna o asigna el id del punto de servicio
        /// </summary>
        [DataMember]
        public long IdPuntoServicio { get; set; }

        [DataMember]
        public string DireccionPuntoServicio { get; set; }

        /// <summary>
        /// Retorna o asigna el numero de identificacion del mensajero
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("PEI_Identificacion")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "Identificacion")]
        public string IdentificacionMensajero { get; set; }

        [DataMember]
        public short TotalEnviosCarga { get; set; }

        [DataMember]
        public short TotalEnviosMensajeria { get; set; }

        [DataMember]
        public short TotalEnviosRecomendados { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public short TotalEnviosPlanillados { get; set; }
        [DataMember]
        public int TotalEnviosSueltosPlanillados { get; set; }
        [DataMember]
        public int TotalConsolidadosPlanillados { get; set; }
        [DataMember]
        public string NombreCentroServicios { get; set; }

        [DataMember]
        public string Precinto { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BolsaSeguridad", Description = "BolsaSeguridad")]
        public string BolsaSeguridad { get; set; }
        [DataMember]
        public short TotalEnvios { get; set; }
        [DataMember]
        public bool EstaCerrada { get; set; }
        [DataMember]
        public DateTime? FechaCierrePlanilla { get; set; }
        [DataMember]
        public int IdVehiculo { get; set; }
        [DataMember]
        public string Placa { get; set; }
        [DataMember]
        public string EstadoPlanillaDescripcion { get; set; }
        [DataMember]
        public string TipoPlanillaDescripcion { get; set; }

        [DataMember]
        public OUAsignacionDC AsignacionTula;

    }
}