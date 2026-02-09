using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    /// <summary>
    /// Representa un concepto asociado a una factura
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class FAConceptoFacturaDC
    {
        public FAConceptoFacturaDC()
        {
            ImpuestosConcepto = new ObservableCollection<FAImpuestoConceptoDC>();
            TotalNeto = 0;
        }

        /// <summary>
        /// Impuestos asociados al concepto
        /// </summary>
        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAImpuestoConceptoDC> ImpuestosConcepto
        { get; set; }

        /// <summary>
        /// Identificador único del concepto
        /// </summary>
        [DataMember]
        public long IdConcepto { get; set; }

        /// <summary>
        /// Número de la factura a la que está asociado el concepto
        /// </summary>
        [DataMember]
        public long NumeroFactura { get; set; }

        /// <summary>
        /// Servicio al que esta asociao el concepto
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Servicio", Description = "ToolTipServicioFac")]
        public int IdServicio { get; set; }

        /// <summary>
        /// Sucursal del cliente a la cual está asociado el servicio
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Sucursal", Description = "ToolTipSucursalFac")]
        public CLSucursalContratoDC Sucursal { get; set; }

        /// <summary>
        /// Cantidad de operaciones asociadas al concepto
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, 9999999999, ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cantidad", Description = "Cantidad")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Descripción del concepto
        /// </summary>
        [DataMember]
        [StringLength(800)]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto", Description = "Concepto")]
        public string DescripcionConcepto { get; set; }

        /// <summary>
        /// Valor unitario del concepto
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, 9999999999)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorUnitario", Description = "ValorUnitario")]
        public decimal ValorUnitario { get; set; }

        /// <summary>
        /// Total neto del concpto (valor unitario * cantidad)
        /// </summary>
        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Range(1, 99999999999)]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalNeto", Description = "TotalNeto")]
        public decimal TotalNeto
        {
            get
            {
                return Cantidad * ValorUnitario;
            }
            set { }
        }

        /// <summary>
        /// Total de los impuestos asociados al concepto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalImpuestos", Description = "TotalImpuestos")]
        public decimal TotalImpuestos
        {
            get
            {
                if (ImpuestosConcepto != null)
                {
                    return ImpuestosConcepto.Sum(imp => imp.Total);
                }
                return 0;
            }
            set
            {
            }
        }

        /// <summary>
        /// Total con impuestos del concepto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorTotal", Description = "ValorTotal")]
        public decimal Total
        {
            get
            {
                return TotalNeto + TotalImpuestos;
            }
            set
            {
            }
        }

        /// <summary>
        /// Indica si el concepto fué creado manualmente o automáticamente por el sistema
        /// </summary>
        [DataMember]
        public bool Manual { get; set; }

        /// <summary>
        /// Fecha de grabación del concepto
        /// </summary>
        [DataMember]
        public System.DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// Usuario que creó el concepto
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Es la Regional Administrativa asociada
        /// </summary>
        [DataMember]
        public PURegionalAdministrativa RacolAsociado { get; set; }

        /// <summary>
        /// si el detalle tiene un valor de nota
        /// es el valor que se registra aqui
        /// </summary>
        [DataMember]
        public ObservableCollection<FANotaFacturaDC> LstNotasAsociadasAlConcepto { get; set; }

        /// <summary>
        /// Es la Sumatorias de las notas creditos y debitos asociadas
        /// al concepto
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorNota", Description = "TooltipValorNota")]
        public decimal ValorTotalNotaAsociadaConcepto { get; set; }
    }
}