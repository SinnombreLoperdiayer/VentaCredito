using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIArchivoGuiaMensajeriaDC : DataContractBase
    {
        [DataMember]
        public long IdArchivo { get; set; }

        [DataMember]
        public string Archivo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string ValorDecodificado { get; set; }

        [DataMember]
        public long IdAdmisionMensajeria { get; set; }


        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public long IdCentroLogistico { get; set; }


        private long caja;

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Caja")]
        public long Caja
        {
            get { return caja; }
            set { caja = value; }
        }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Lote")]
        public int Lote { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Posicion")]
        public int Posicion { get; set; }

        [DataMember]
        public bool CajaLlena { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "DatosEdicion", Description = "ToolTipDatosEdicion")]
        public LIEstadoDatosGuiaDC EstadoDatosEdicion { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "DatosEntrega", Description = "ToolTipDatosEntrega")]
        public LIEstadoDatosGuiaDC EstadoDatosEntrega { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "EstadoGuia", Description = "ToolTipEstadoGuia")]
        public LIEstadoFisicoGuiaDC EstadoFisicoGuia { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaEntrega", Description = "ToolTipFechaEntrega")]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public DateTime FechaEntrega { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "HoraEntrega", Description = "ToolTipHoraEntrega")]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public DateTime Hora { get; set; }

        [DataMember]
        public bool EntregaExitosa { get; set; }

        [DataMember]
        public bool ExisteGuia { get; set; }

        [DataMember]
        public bool ExisteArchivo { get; set; }

        [DataMember]
        public bool CambioEstado { get; set; }

        [DataMember]
        public string DirectorioImagen { get; set; }

        [DataMember]
        public long IdCol { get; set; }

        [DataMember]
        public ADTrazaGuia TrazaGuia { get; set; }

        [DataMember]
        public DateTime FechaEstimadaEntrega { get; set; }

        [DataMember]
        public DateTime FechaAdmision { get; set; }

        [DataMember]
        public string NombreRemitente { get; set; }

        [DataMember]
        public string CiudadOrigen { get; set; }

        [DataMember]
        public string TelefonoRemitente { get; set; }

        [DataMember]
        public string DireccionRemitente { get; set; }

        [DataMember]
        public string Peso { get; set; }

        [DataMember]
        public string ValorAdmision { get; set; }

        [DataMember]
        public string ValorPrimaSeguro { get; set; }

        [DataMember]
        public string ValorTotal { get; set; }



        [DataMember]
        public string NombreDestinatario { get; set; }

        [DataMember]
        public string DireccionDestinatario { get; set; }

        [DataMember]
        public string TelefonoDestinatario { get; set; }

        [DataMember]
        public string CiudadDestinatario { get; set; }

        [DataMember]
        public string DiceContener { get; set; }

        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public string DescripcionEstado { get; set; }

        [DataMember]
        public string RutaServidor { get; set; }

        [DataMember]
        public DateTime FechaArchivo { get; set; }

        [DataMember]
        public string UsuarioArchivo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "EstadoGuia")]
        public string EstadoGuia { get; set; }

        [DataMember]
        public ADEnumResultadoEscaner ResultadoEscaner { get; set; }

        [DataMember]
        public string NombreCiudad { get; set; }

        [DataMember]
        public string IdCiudad { get; set; }

        /// <summary>
        /// Indica si la imagen ya fué cargada en el servidor
        /// </summary>
        [DataMember]
        public bool Cargada
        {
            get;
            set;
        }
        /// <summary>
        /// Indica si la guia fue decoficada o digitado el codigo de barras de manera manual
        /// </summary>
        [DataMember]
        public bool Decodificada { get; set; }

        /// <summary>
        /// Indica si la guia fue escaneado o ingresada de manera manual
        /// </summary>
        [DataMember]
        public bool Manual { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public string NombreAdjunto { get; set; }



        [DataMember]
        public bool Sincronizada { get; set; }

        [DataMember]
        public bool EsCarpeta { get; set; }

        [DataMember]
        //System.Drawing.Point
        public List<string> PuntosUbicacionCodigoBarras { get; set; }


        [DataMember]
        public int IdServicio { get; set; }

        
        private EnumTipoArchivo tipoArchivoPruebaEntrega = EnumTipoArchivo.ENUM_SIN_VALOR;

        [DataMember]
        public EnumTipoArchivo TipoArchivoPruebaEntrega
        {
            get
            {
                return tipoArchivoPruebaEntrega;
            }

            set
            {
                tipoArchivoPruebaEntrega = value;
            }
        }


        [DataMember]
        public long NumeroEvidencia { get; set; }

        
    }
}