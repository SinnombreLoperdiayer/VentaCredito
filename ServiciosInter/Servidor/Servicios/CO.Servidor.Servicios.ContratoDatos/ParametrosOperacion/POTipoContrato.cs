using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion
{
    /// <summary>
    /// Clase que contiene la informacion de los tipos de contrato
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class POTipoContrato : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "TipoContrato")]
        public int IdTipoContrato { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "TipoContrato")]
        public string Descripcion { get; set; }
    }
}