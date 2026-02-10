using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Clase con el DataContract de los archivos de logistica inversa
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIEvidenciaDevolucionDC : DataContractBase
    {
        [DataMember]
        public long? IdEvidenciaDevolucion { get; set; }

        [DataMember]
        public long IdEstadoGuialog { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Tipo", Description = "ToolTipTipoEvidencia")]
        public LITipoEvidenciaDevolucionDC TipoEvidencia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, long.MaxValue, ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Numero", Description = "TooTipNumeroEvidencia")]
        public long NumeroEvidencia { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public bool EstaDigitalizado { get; set; }

        [DataMember]
        public LIArchivosDC Archivo { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        [DataMember]
        public string NombreArchivo { get; set; }

        [DataMember]
        public long IdArchivo { get; set; }

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
        /// Es el mismo Numero de Volante
        /// </summary>
        [DataMember]
        public string ValorDecodificado { get; set; }
        [DataMember]
        public ADEnumResultadoEscaner ResultadoEscaner { get; set; }
        [DataMember]
        public bool Decodificada { get; set; }
        [DataMember]
        public string DirectorioImagen { get; set; }
        [DataMember]
        public long? NumeroGuia { get; set; }
        [DataMember]
        public DateTime? FechaMotivo { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
    }
}