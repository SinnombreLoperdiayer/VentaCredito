using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract]
    public class RAParametrosPersonalizacionRapsDC
    {
        [DataMember]
        public int IdTipoNovedad { get; set; }

        [DataMember]
        public int IdParametro { get; set; }

        [DataMember]
        public int IdFuncion { get; set; }  

        [DataMember]
        public bool EsAgrupamiento { get; set; }

        [DataMember]
        public string DescripcionParametro { get; set; }

        [DataMember]
        public int TipoDato { get; set; }

        [DataMember]
        public int Longitud { get; set; }

        [DataMember]
        public EnumFuncionesReglasParametrizacionRaps Funcion { get; set; }
        
        [DataMember]
        public string Valor { get; set; }
    }
}
