using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;

namespace CO.Servidor.Servicios.ContratoDatos.Facturacion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class FAProgramacionFacturaDC
    {
        //[DataMember]
        //public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAExclusionProgramacionDC> ExclusionesProgramacion
        //{ get; set; }

        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAExclusionProgramacionDC> ExclusionesProgramacion
        {
            get;
            set;
        }

        [DataMember]
        [Required]
        [CamposOrdenamiento("PRF_IdProgramacion")]
        [Filtrable("PRF_IdProgramacion", "Id Programación:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 10, FormatoRegex = "^[0-9]+$", MensajeError = "El codigo de la programación debe ser numérico")]
        [Display(Name = "Id Programación")]
        public long IdProgramacion { get; set; }

        [DataMember]
        [Filtrable("PRF_IdRacol", "Id Racol:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 10, FormatoRegex = "^[0-9]+$", MensajeError = "El codigo del Racol debe ser numérico")]
        [Display(Name = "Id Racol")]
        public long IdRacol { get; set; }

        [DataMember]
        [Filtrable("PRF_NombreRacol", "Nombre Racol:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 12)]
        [CamposOrdenamiento("PRF_NombreRacol")]
        [Display(Name = "Racol")]
        public string NombreRacol { get; set; }

        [DataMember]
        [Filtrable("PRF_IdCliente", "Id Cliente:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 12, FormatoRegex = "^[0-9]+$", MensajeError = "El codigo del Cliente debe ser numérico")]
        [Display(Name = "Id Cliente")]
        public int IdCliente { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRF_RazonSocialCliente")]
        [Filtrable("PRF_RazonSocialCliente", "Nombre Cliente:", COEnumTipoControlFiltro.TextBox, MaximaLongitud = 30)]
        [Display(Name = "Cliente")]
        [Required]
        public string RazonSocialCliente { get; set; }

        [DataMember]
        public int IdContrato { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRF_DescContrato")]
        [Display(Name = "Contrato")]
        [Required]
        public string DescContrato { get; set; }

        [DataMember]
        public int IdAgrupamiento { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRF_DescAgrupamiento")]
        [Display(Name = "Agrupamiento")]
        [Required]
        public string DescAgrupamiento { get; set; }

        [DataMember]
        [Required]
        [CamposOrdenamiento("PRF_DiaCorte")]
        [Display(Name = "Dia de Corte")]
        public int DiaCorte { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRF_FechaProgramacion")]
        [Display(Name = "Fecha Programada")]
        [Required]
        public System.DateTime FechaProgramacion { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRF_Ejecutado")]
        [Display(Name = "Ejecutado")]
        public bool Ejecutado { get; set; }

        [DataMember]
        [CamposOrdenamiento("PRF_Tipo")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [DataMember]
        public System.DateTime FechaEjecucion { get; set; }

        [DataMember]
        public System.DateTime FechaGrabacion { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }

        [DataMember]
        public ObservableCollection<CO.Servidor.Servicios.ContratoDatos.Facturacion.FAConceptoProgramadoDC> ConceptosProgramados
        {
            get;
            set;
        }

        [DataMember]
        public bool IncluirEnFacturacion
        {
            get;
            set;
        }

        [DataMember]
        public string ResultadoEjecucion
        {
            get;
            set;
        }

        
    }
}