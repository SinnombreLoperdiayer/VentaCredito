using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class FAFacturaClienteAutDC : FAFacturaClienteDC
    {
        [DataMember]
        public int IdAgrupamientoFactura { get; set; }

        [DataMember]
        [Display(Name = "Agrupamiento")]
        public string NombreAgrupamientoFactura { get; set; }

        [DataMember]
        public System.DateTime FechaCorte { get; set; }

        [DataMember]
        public short DiaPago { get; set; }

        [DataMember]
        [Display(Name = "Dia Pago")]
        public string DescDiaPago { get; set; }

        [DataMember]
        public short DiaFacturacion { get; set; }

        [DataMember]
        [Display(Name = "Dia Facturacion")]
        public string DescDiaFacturacion { get; set; }

        [DataMember]
        public short DiaRadicacion { get; set; }

        [DataMember]
        [Display(Name = "Dia Radicacion")]
        public string DescDiaRadicacion { get; set; }

        [DataMember]
        public long? IdProgramacion { get; set; }

        [DataMember]
        public IEnumerable<CO.Servidor.Servicios.ContratoDatos.Clientes.CLSucursalDC> Sucursales
        {
            get;
            set;
        }

        [DataMember]
        public IEnumerable<CO.Servidor.Servicios.ContratoDatos.Tarifas.TAServicioDC> Servicios
        {
            get;
            set;
        }

        [DataMember]
        [Display(Name = "Requisitos Factura")]
        public IEnumerable<string> RequisitosFactura
        {
            get;
            set;
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NotasCredito", Description = "NotasCredito")]
        public decimal TotalNotasCredito
        {
            get;
            set;
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NotasDebito", Description = "NotasDebito")]
        public decimal TotalNotasDebito
        {
            get;
            set;
        }

        [DataMember]
        public decimal ValorTotalFactura { get; set; }

        public bool Seleccionado
        {
            get;
            set;
        }

        [DataMember]
        public bool SeImprime
        {
            get;
            set;
        }

        [DataMember]
        public ADGuiaInternaDC GuiaInterna
        {
            get;
            set;
        }
    }
}