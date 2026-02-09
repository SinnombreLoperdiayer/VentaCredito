using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Rutas
{
    /// <summary>
    /// Clase que contiene la informacion de las empresas transportadoras
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class RUEmpresaTransportadora : DataContractBase
    {
        /// <summary>
        /// Identificador de la empresa de transportadora
        /// </summary>
        [DataMember]
        public int IdEmpresaTransportadora { get; set; }

        /// <summary>
        /// Nombre de la empresa transportadora
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "EmpresaTransportadora", Description = "ToolTipEmpresaTransportadora")]
        [Filtrable("ETR_Nombre", "Nombre Empresa", COEnumTipoControlFiltro.TextBox)]
        public string Nombre { get; set; }

        [DataMember]
        public int IdMedioTransporte { get; set; }

        [DataMember]
        public int IdTipoTransporte { get; set; }

        /// <summary>
        /// Identificador del tipo de transporte
        /// </summary>
        [DataMember]
        public RUTipoTransporte TipoTransporte { get; set; }

        /// <summary>
        /// Identificador del tipo de transporte
        /// </summary>
        [DataMember]
        public IList<RUTipoTransporte> TiposTransportes { get; set; }

        /// <summary>
        /// Estado de la empresa transportadora: Activa: ACT, Inactiva: INA
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]

        //[Filtrable("ERT_Estado", "Estado", COEnumTipoControlFiltro.ComboBox)]
        public string EstadoEmpresa { get; set; }

        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Fecha en la que se graba el Registro
        /// </summary>
        [DataMember]
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// usuario que creo el registro
        /// </summary>
        [DataMember]
        public string CreadoPor { get; set; }

        /// <summary>
        /// lista de Medios de Transporte Asociados
        /// </summary>
        [DataMember]
        public List<PAMedioTransporte> LstMediosTransporte { get; set; }

        /// <summary>
        /// lista de Racoles Asociados
        /// </summary>
        [DataMember]
        public List<PURegionalAdministrativa> LstRacolAsociados { get; set; }

        /// <summary>
        /// Lista de Racoles Diponibles
        /// </summary>
        [DataMember]
        public List<PURegionalAdministrativa> LstRacolDisponibles { get; set; }
    }
}