using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PUAgenciaDeRacolDC : DataContractBase
    {
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdCentroServicio")]
        public long IdCentroServicio { get; set; }

        [DataMember]
        public string CentroDeCostosCentroServ { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CentroServicio")]
        public string NombreCentroServicio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoCentroServicio")]
        public string TipoCentroServicio { get; set; }

        [DataMember]
        public string BodegaCentroServ { get; set; }

        [DataMember]
        public long IdResponsable { get; set; }

        [DataMember]
        public string CentroDeCostosResponsable { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RegionalAdministrativa")]
        public string NombreResponsable { get; set; }


        [DataMember]
        public string IdCiudadResponsable { get; set; }

        [DataMember]
        public string NombreCiudadResponsable { get; set; }

        [DataMember]
        public string CodResponsableERP { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAcumuladoCaja")]
        public decimal SaldoAcumuladoCaja { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "SaldoAcumuladoCaja")]
        public DateTime? FechaUltimaLiquidacion { get; set; }
    }
}