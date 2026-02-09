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
    public class LIArchivoGuiaMensajeriaFachadaDC : DataContractBase
    {
        [DataMember]
        public long IdMensajero { get; set; }

        [DataMember]
        public long IdentificacionReciboPor { get; set; }

        [DataMember]
        public string RutaServidor { get; set; }

        [DataMember]
        public long IdCiudad { get; set; }

        [DataMember]
        public string Ciudad { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string ValorDecodificado { get; set; }

        [DataMember]
        public decimal Latitud { get; set; }

        [DataMember]
        public decimal Longitud { get; set; }

        [DataMember]
        public long Archivo { get; set; }

        [DataMember]
        public string Imagen { get; set; }

        [DataMember]
        public string DescripcionEvidencia { get; set; }
    }
}
