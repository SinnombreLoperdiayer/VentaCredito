using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Collections.ObjectModel;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los conceptos de
    /// la Caja
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAConceptoCajaDC : DataContractBase
    {
        /// <summary>
        /// Gets or sets the id concepto caja.
        /// </summary>
        /// <value>
        /// Es el Id del Concepto caja.
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdConcepto", Description = "TooltipIdConcepto")]
        public int IdConceptoCaja { get; set; }

        /// <summary>
        /// Gets or sets the nombre.
        /// </summary>
        /// <value>
        /// Es el nombre del Concepto de Caja
        /// </value>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Concepto", Description = "TooltipConcepto")]
        [Filtrable("COC_Nombre", "Nombre Concepto:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 50)]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Nombre { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "DescripcionConcepto", Description = "TooltipDescripcionConcepto")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Retorna o asigna un valor indicado si la operación es de ingreso
        /// </summary>
        /// <value>
        /// <c>true</c> Si es un ingreso, si es un Egreso <c>false</c>.
        /// </value>
        [DataMember]
        public bool EsIngreso { get; set; }

        [DataMember]
        public bool EsEgreso
        {
            get
            {
                return !EsIngreso;
            }
            set
            {
                EsIngreso = !value;
            }
        }

        /// <summary>
        /// Es la Dupla Correpondiente al Concepto
        /// de Caja.
        /// </summary>
        /// <value>
        /// The id dupla concepto caja.
        /// </value>
        [DataMember]
        public int IdDuplaConceptoCaja { get; set; }

        /// <summary>
        /// Identificador de la cuenta externa
        /// </summary>
        /// <value>
        /// The id cuenta externa.
        /// </value>
        [DataMember]
        public short? IdCuentaExterna { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CuentaExterna", Description = "TooltipCuentaExterna")]
        public CACuentaExterna CuentaExterna { get; set; }

        /// <summary>
        /// Es el grupo en el que esta el
        /// concepto de caja
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CategoriaConcepto", Description = "ToolTipCategoriaConcepto")]
        public ObservableCollection<CAConceptoCajaCategoriaDC> GruposCategorias { get; set; }

        /// <summary>
        /// En caso de Un Update se toma este registro como el
        /// id de Categoria anterior
        /// </summary>
        [DataMember]
        public int IdCategoriaAnterior { get; set; }

        [DataMember]
        public bool EsServicio { get; set; }

        [DataMember]
        public bool ContraPartidaCasaMatriz { get; set; }

        [DataMember]
        public bool ContraPartidaCS { get; set; }

        [DataMember]
        public bool RequiereNoDocumento { get; set; }

        public string NombreCompleto
        {
            get
            {
                if (EsIngreso)
                    return Nombre + " (Ingreso)";
                else
                    return Nombre + " (Egreso)";
            }
        }

    }
}