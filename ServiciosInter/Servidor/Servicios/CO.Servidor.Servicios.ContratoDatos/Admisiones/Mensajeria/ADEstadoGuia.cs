using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADEstadoGuia : DataContractBase
    {
        [DataMember]
        public short Id { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        public string Descripcion { get; set; }

        [DataMember]
        public bool EsVisible { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public long IdCentroServicio { get; set; }

        [DataMember]
        public string NombreCentroServicio { get; set; }
    }
}