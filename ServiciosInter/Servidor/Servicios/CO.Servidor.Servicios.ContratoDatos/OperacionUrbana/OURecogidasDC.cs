using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OURecogidasDC : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Recogida", Description = "Recogida")]
        [CamposOrdenamiento("SOR_IdSolicitudRecogida")]
        public long? IdRecogida { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ProgramacionSolRecogida", Description = "ToolTipProgramacionSolRecogida")]
        [CamposOrdenamiento("PSR_IdProgramacionSolicitudRecog")]
        public long IdProgramacionSolicitudRecogida { get; set; }

        [DataMember]
        public OUEstadosSolicitudRecogidaDC EstadoRecogida { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaSolicitud", Description = "FechaSolicitud")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("SOR_FechaGrabacion")]
        public DateTime FechaSolicitud { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaRecogida", Description = "FechaRecogida")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("SOR_FechaRecogida")]
        public DateTime FechaRecogida { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "Direccion")]
        [CamposOrdenamiento("SOR_Direccion")]
        public string Direccion { get; set; }


        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ComplementoDireccion", Description = "ComplementoDireccion")]
        [CamposOrdenamiento("SOR_ComplementoDireccion")]
        public string ComplementoDireccion { get; set; }


        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Contacto", Description = "Contacto")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [CamposOrdenamiento("SOR_PersonaContacto")]
        public string Contacto { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PersonaRecepciono", Description = "PersonaRecepciono")]
        public string PersonaRecepcionoRecogida { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PersonaSolicita", Description = "PersonaSolicita")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string PersonaSolicita { get; set; }

        [DataMember]
        public PAZonaDC Zona { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Reportada", Description = "Reportada")]
        public bool EstaReportada { get; set; }

        [DataMember]
        public PUCentroServiciosDC PuntoServicio { get; set; }

        [DataMember]
        public CLSucursalDC Sucursal { get; set; }

        [DataMember]
        public CLClientesDC Cliente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CantidadEnvios", Description = "CantidadEnvios")]
        public short? CantidadEnvios { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoAproximado", Description = "PesoAproximado")]
        public decimal? PesoAproximado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "Observaciones")]
        public string Observaciones { get; set; }

        [DataMember]
        public long IdAgenciaResponsable { get; set; }

        [DataMember]
        public OURecogidaPeatonDC RecogidaPeaton { get; set; }

        [DataMember]
        public OUEnumTipoClienteRecogidaDC TipoCliente { get; set; }

        [DataMember]
        public bool EstaProgramada { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CodigoPuntoCliente", Description = "CodigoPuntoCliente")]
        public string CodigoPuntoCliente { get; set; }

        [DataMember]
        public bool EstaSeleccionada { get; set; }

        [DataMember]
        public string IdTipoRecogida { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoRecogida", Description = "TipoRecogida")]
        public string DescripcionTipoRecogida { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoRecogida", Description = "TipoRecogida")]
        public bool RegistroHabilitado { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "MotivoDescargue", Description = "MotivoDescargue")]
        public OUMotivoDescargueRecogidasDC MotivoDescargue { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaDescargue", Description = "FechaDescargue")]
        public DateTime FechaDescargue { get; set; }

        [DataMember]
        public OUNombresMensajeroDC MensajeroPlanilla { get; set; }

        [DataMember]
        public OUTipoMensajeroDC TipoMensajero { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Reportada", Description = "Reportada")]
        public string DescripcionEstaReportada { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoSolicitud")]
        public string DescripcionTipoSolicitante { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreCliente")]
        public string NombreCliente { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "ToolTipCiudadRecogida")]
        public PALocalidadDC LocalidadRecogida { get; set; }

        [DataMember]
        public string LongitudRecogida { get; set; }

        [DataMember]
        public string LatitudRecogida { get; set; }

        [DataMember]
        public long IdPlanillaRecogida { get; set; }

        [DataMember]
        public CO.Servidor.Servicios.ContratoDatos.OperacionUrbana.OUEnumTipoOrigenRecogida TipoOrigenRecogida { get; set; }

        [IgnoreDataMember]
        public Framework.Servidor.Servicios.ContratoDatos.Parametros.PADispositivoMovil DispositivoMovil { get; set; }

        [DataMember]
        public long MinutosTranscurridos { get; set; }

        [DataMember]
        public string NombreTipoEnvio { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public List<string> Fotografias { get; set; }

        [DataMember]
        public OUAsignacionRecogidaMensajeroDC AsignacionMensajero { get; set; }

        [DataMember]
        public int VecesNotificadaPush { get; set; }


    }
}