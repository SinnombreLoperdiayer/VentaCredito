using ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADRapiRadicado
    {       
        public short NumeroFolios { get; set; }

        public TATipoDestino TipoDestino { get; set; }
                
        public string CodigoRapiRadicado { get; set; }
                
        public PALocalidadDC PaisDestino { get; set; }
                
        public PALocalidadDC CiudadDestino { get; set; }
                
        public string TipoIdentificacionDestinatario { get; set; }
                
        public string IdDestinatario { get; set; }
                
        public string NombreDestinatario { get; set; }
                
        public string Apellido1Destinatario { get; set; }
                
        public string Apellido2Destinatario { get; set; }
                
        public string TelefonoDestinatario { get; set; }
                
        public string DireccionDestinatario { get; set; }
                
        public string EmailDestinatario { get; set; }

        public long NumeroGuiaInterna { get; set; }

        /// <summary>
        /// Tipos de destino
        /// </summary>
        private ObservableCollection<TATipoDestino> tiposDestino;
                
        public ObservableCollection<TATipoDestino> TiposDestino
        {
            get { return tiposDestino; }
            set { tiposDestino = value; }
        }
                
        public long IdRapiradicado { get; set; }
                
        public ADGuiaInternaDC GuiaInterna { get; set; }
                
        public ADGuia GuiaAdmision { get; set; }
                
        public PUCentroServiciosDC CentroServicioCreacion { get; set; }
                
        public ObservableCollection<ADArchivoRadicadoDC> ListaArchivos { get; set; }
                
        public bool EsRadicado { get; set; }
                
        public bool ExisteRadicado { get; set; }
    }
}
