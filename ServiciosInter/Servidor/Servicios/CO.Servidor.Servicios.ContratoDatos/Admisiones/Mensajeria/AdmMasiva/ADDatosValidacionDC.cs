using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADDatosValidacionDC
    {
        [DataMember]
        public int NoFila { get; set; }

        [DataMember]
        public TAServicioDC TipoServicio { get; set; }

        [DataMember]
        public int IdListaPrecios { get; set; }

        [DataMember]
        public PALocalidadDC MunicipioOrigen { get; set; }

        [DataMember]
        public PALocalidadDC MunicipioDestino { get; set; }

        [DataMember]
        public long IdcentroServiciosOrigen { get; set; }

        [DataMember]
        public long IdSucursalClienteOrigen { get; set; }

        [DataMember]
        public decimal peso { get; set; }

        [DataMember]
        public decimal valorDeclarado { get; set; }

        [DataMember]
        public long Numeroguia { get; set; }

        [DataMember]
        public bool Credito { get; set; }

        [DataMember]
        public bool EsGuiaInterna { get; set; }

        [DataMember]
        public string IdTipoEntrega { get; set; }
    }
}