using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RASolicitudDC
    {
        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public long IdParametrizacionRapPadre { get; set; }

        [DataMember]
        public string IdCargoSolicita { get; set; }

        [DataMember]
        public string IdCargoResponsable { get; set; }

        [DataMember]
        public string IdResponsable { get; set; }

        [DataMember]
        public DateTime FechaCreacion { get; set; }

        [DataMember]
        public DateTime FechaInicio { get; set; }

        [DataMember]
        public DateTime FechaVencimiento { get; set; }

        [DataMember]
        public RAEnumEstados IdEstado { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public long IdSolicitudPadre { get; set; }

        [DataMember]
        public string DocumentoSolicita { get; set; }

        [DataMember]
        public string DocumentoResponsable { get; set; }

        [DataMember]
        public List<RAEscalonamientoDC> Escalonamiento { get; set; }

        [DataMember]
        public RACargoDC Cargo { get; set; }

        [DataMember]
        public List<RAParametrosParametrizacionDC> ParametrosSolicitud { get; set; }

        [DataMember]
        public string idSucursal { get; set; }

        [DataMember]
        public string NombreParametrizacionRapPadre { get; set; }
        [DataMember]
        public string IdCiudad { get; set; }

        private bool esAcumulativa = false;
        [DataMember]
        public bool EsAcumulativa
        {
            get
            {
                return esAcumulativa;
            }

            set
            {
                esAcumulativa = value;
            }
        }


        public String Anchor { get; set; }

        [DataMember]
        public int IdTipoEscalonamiento { get; set; }
    }

}
