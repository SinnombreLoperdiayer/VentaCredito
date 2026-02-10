using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
    /// <summary>
    /// Clase que contiene la informacion de los tipos de transporte
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class RUTipoTransporte : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoTransporte", Description = "ToolTipTipoTransporte")]
        public int IdTipoTransporte { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoTransporte", Description = "ToolTipTipoTransporte")]
        public string NombreTipoTransporte { get; set; }
    }
}