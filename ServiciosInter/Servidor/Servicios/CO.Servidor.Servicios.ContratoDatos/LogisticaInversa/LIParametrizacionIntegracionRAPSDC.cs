using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIParametrizacionIntegracionRAPSDC
    {
        [DataMember]
        public string NombreParametro { get; set; }

        [DataMember]
        public int IdParametro { get; set; }

        [DataMember]
        public int Longitud { get; set; }

        [DataMember]
        public int TipoDato { get; set; }

        [DataMember]
        public string DescripcionParametro { get; set; }

        [DataMember]
        public string ClaveTipoFalla { get; set; }

        [DataMember]
        public string NombreObjeto { get; set; }

        [DataMember]
        public string NombrePropiedad { get; set; }

        [DataMember]
        public bool EsArray { get; set; } 

        [DataMember]
        public int PosicionEnArray { get; set; }

        [DataMember]
        public short IdMotivoController { get; set; }

        [DataMember]
        public long IdParametrizacion { get; set; }

        [DataMember]
        public int IdFuncion { get; set; }

        [DataMember]
        public string DescripcionFuncion { get; set; }

        [DataMember]
        public string DescripcionParametrizacion { get; set; }

        [DataMember]
        public int IdTipoNovedad { get; set; }

        [DataMember]
        public bool EsAgrupamiento { get; set; }

        [DataMember]
        public EnumFuncionesReglasParametrizacionRaps Funcion { get; set; }
    }
}
