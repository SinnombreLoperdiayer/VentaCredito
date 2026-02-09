using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    /// <summary>
    /// Representa una factura de un cliente crédito
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class FAFacturaClienteDC : DataContractBase
    {
        public FAFacturaClienteDC()
        {
            ConceptosFactura = new ObservableCollection<FAConceptoFacturaDC>();
            DeduccionesFactura = new ObservableCollection<FADeduccionFacturaDC>();
            DescuentosFactura = new ObservableCollection<FADescuentoFacturaDC>();
            EstadosFactura = new ObservableCollection<FAEstadoFacturaDC>();
            NotasFactura = new ObservableCollection<FANotaFacturaDC>();
            CiudadRadicacion = new PALocalidadDC();
            ConceptosFactura = new ObservableCollection<FAConceptoFacturaDC>();
            RetencionesFactura = new ObservableCollection<FARetencionFacturaDC>();
            NombreRacol = " ";
            this.Pais = new PALocalidadDC()
            {
                IdLocalidad = "057"
            };
            NumeroFactura = 0;
        }

        /// <summary>
        /// Conceptos asociados a la factura
        /// </summary>
        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAConceptoFacturaDC> ConceptosFactura
        { get; set; }

        /// <summary>
        /// Deduccciones asociadas a la factura
        /// </summary>
        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FADeduccionFacturaDC> DeduccionesFactura
        { get; set; }

        /// <summary>
        /// Desceuentos asociados a la factura
        /// </summary>
        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FADescuentoFacturaDC> DescuentosFactura
        { get; set; }

        /// <summary>
        /// Estados de la factura
        /// </summary>
        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAEstadoFacturaDC> EstadosFactura
        { get; set; }

        /// <summary>
        /// Notas asociadas a la factura
        /// </summary>
        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FANotaFacturaDC> NotasFactura
        { get; set; }

        /// <summary>
        /// Número de factura
        /// </summary>
        [DataMember]
        [Required]
        [CamposOrdenamiento("REF_NumeroFactura")]
        [Filtrable("REF_NumeroFactura", "No. Factura:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 10, FormatoRegex = "^[0-9]+$", MensajeError = "El codigo de la factura debe ser numérico")]
        [Display(Name = "No. Factura")]
        public long NumeroFactura { get; set; }

        /// <summary>
        /// Tipo de Factura (manual, automática)
        /// </summary>
        [DataMember]
        [CamposOrdenamiento("REF_TipoFacturacion")]
        [Display(Name = "Tipo Factura")]
        public string TipoFacturacion { get; set; }

        /// <summary>
        /// Id de la Racol a a cual pertence el cliente dueño de la factura
        /// </summary>
        [DataMember]
        public long? IdRacol { get; set; }

        /// <summary>
        /// Nombre de la Racol a a cual pertence el cliente dueño de la factura
        /// </summary>
        [DataMember]
        [CamposOrdenamiento("REF_NombreRacol")]
        [Filtrable("REF_NombreRacol", "Racol", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 20)]
        [Display(Name = "Racol")]
        public string NombreRacol { get; set; }

        /// <summary>
        /// Identificación del cliente dueño de la factura
        /// </summary>
        [DataMember]
        [Required]
        public int IdCliente { get; set; }

        /// <summary>
        /// Razón social del cliente dueño de la factura
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(100)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "RazonSocial", Description = "TooltipRazonSocial")]
        [CamposOrdenamiento("REF_RazonSocial")]
        [Filtrable("REF_RazonSocial", "Cliente", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 20)]
        public string RazonSocial { get; set; }

        /// <summary>
        /// Id del contrato asociado a la factura
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Contrato", Description = "TooltipContrato")]
        public int IdContrato { get; set; }

        /// <summary>
        /// Nombre del contrao asocido a la factura
        /// </summary>
        [DataMember]
        [CamposOrdenamiento("REF_NombreContrato")]
        [Display(Name = "Contrato")]
        public string NombreContrato { get; set; }

        /// <summary>
        /// Número del contrato asociado a la factura
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Contrato", Description = "Contrato")]
        public string NumeroContrato { get; set; }

        /// <summary>
        /// Forma de pago de la factura
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FormaPago", Description = "TooltipFormaPagoFact")]
        public string FormaPago { get; set; }

        /// <summary>
        /// Forma de pago de la factura
        /// </summary>
        [DataMember]
        [Display(Name = "Forma de Pago")]
        public string DescFormaPago { get; set; }

        /// <summary>
        /// Plazo de pago de la factura
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PlazoPago", Description = "TooltipPlazoPago")]
        public EPlazoPago PlazoPago { get; set; }

        /// <summary>
        /// Pais de la factura
        /// </summary>
        [DataMember]
        public PALocalidadDC Pais { get; set; }

        /// <summary>
        /// Ciudad a donde se debe radicar la factura
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadRadicacion", Description = "ToolTipCiudadRadicacion")]
        public PALocalidadDC CiudadRadicacion { get; set; }

        /// <summary>
        /// Dirección a donde se debe radicar la factura
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(100)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DireccionRadic", Description = "TooltipDireccionRadic")]
        public string DireccionRadicacion { get; set; }

        /// <summary>
        /// Telefono del cliente
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(15)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "Telefono")]
        public string TelefonoRadicacion { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [StringLength(50)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DirigidoA", Description = "TooltipDirigidoA")]
        public string DirigidoA { get; set; }


        private decimal valorNeto;
        /// <summary>
        /// Valor sin impuestos de la factura
        /// </summary>
        [DataMember]
        [Required]
        public decimal ValorNeto
        {
            get { return ConceptosFactura.Sum(con => con.TotalNeto); }
            set { valorNeto = value; }
        }


        private decimal valorImpuestos;
        /// <summary>
        /// Valor de los impuestos de la factura
        /// </summary>
        [DataMember]
        [Required]
        public decimal ValorImpuestos
        {
            get { return ConceptosFactura.Sum(con => con.TotalImpuestos); }
            set { valorImpuestos = value; }
        }

        /// <summary>
        /// Valor de los descuentos de la factura
        /// </summary>
        [DataMember]
        public decimal ValorDescuentos
        {
            get { return DescuentosFactura.Sum(des => des.Valor); }
            set { valorDescuentos = value; }
        }
        private decimal valorDescuentos;



        /// <summary>
        /// Valor total de la factura
        /// </summary>
        [DataMember]
        [Required]
        [CamposOrdenamiento("REF_ValorTotal")]
        [Display(Name = "Valor Total")]
        public decimal ValorTotal
        {
            get { return ValorNeto + ValorImpuestos - ValorDescuentos; }
            set { valorTotal = value; }
        }
        private decimal valorTotal;

        /// <summary>
        /// Último estado de la factura
        /// </summary>
        [DataMember]
        [CamposOrdenamiento("ESF_DescEstadoActual")]
        [Display(Name = "Estado Actual")]
        public string EstadoActual
        {
            get;
            set;
        }

        /// <summary>
        /// FEcha de grabación de la factura
        /// </summary>
        [DataMember]
        [CamposOrdenamiento("REF_FechaGrabacion")]
        [Display(Name = "Fecha Factura")]
        public System.DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// Usuario que creó la factura
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }

        public List<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAOperacionFacturadaDC> DetalleOperaciones
        {
            get;
            set;
        }

        [DataMember]
        public System.Collections.ObjectModel.ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FARetencionFacturaDC> RetencionesFactura
        {
            get;
            set;
        }
    }

    [DataContract(Namespace = "http://contrologis.com")]
    public enum EPlazoPago : short
    {
        [EnumMember]
        Cero_Dias = 0,
        [EnumMember]
        Tres_Dias = 3,
        [EnumMember]
        Treinta_Dias = 30,
        [EnumMember]
        Sesenta_Dias = 60,
        [EnumMember]
        Noventa_Dias = 90
    }
}