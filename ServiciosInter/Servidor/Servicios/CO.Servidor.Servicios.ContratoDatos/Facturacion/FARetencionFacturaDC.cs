using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class FARetencionFacturaDC : DataContractBase
    {
        [DataMember]
        public int IdRetencion
        {
            get;
            set;
        }

        [DataMember]
        public long NumeroFactura
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Descripcion")]
        public string Descripcion
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Valor Fijo")]
        public decimal ValorFijo
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Porcentaje")]
        public decimal ValorPorcenctual
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Valor x 1000")]
        public decimal Valorx100
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Base Minima")]
        public decimal BaseMinima
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Base Cálculo")]
        public decimal BaseCalculo
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Total")]
        public decimal? Total
        {
            get;
            set;
        }
    }
}