using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ONImpresionManifiestoDC : DataContractBase
    {
        [DataMember]
        public long NumConTransReotorno { get; set; }
        
        [DataMember]
        public long IdManifiesto { get; set; }
        
        [DataMember]
        public List<ONConsolidado> Consolidados { get; set; }

        [DataMember]
        public List<string> GuiasRotulosSueltos { get; set; }
        
        [DataMember]
        public string NombreCentroServiciosOrigen { get; set; }
        [DataMember]
        public string NombreCentroServiciosDestino { get; set; }
        [DataMember]
        public string NombreEmpresaTransportadora { get; set; }
        [DataMember]
        public int TotalConsolidados { get; set; }
        
        /// <summary>
        /// Total de envios sueltos
        /// </summary>
        [DataMember]
        public int TotalEnvios { get; set; }

        [DataMember]
        public string Placa { get; set; }
        [DataMember]
        public string Vehiculo {get;set;}
        [DataMember]
        public string NombreConductor { get; set; }
        [DataMember]
        public string NombreCiudadOrigen { get; set; }

        [DataMember]
        public string NombreCiudadDestino { get; set; }
        [DataMember]
        public string IdLocalidadManifestada { get; set; }



      
    }
}
