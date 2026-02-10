using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Servidor.Servicios.ContratoDatos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class VEDatosCentroServicio : DataContractBase
    {
        /// <summary>
        /// Identificador agencia o centro de servicio
        /// </summary>
        [DataMember]
        public long IdCentroServicio { get; set; }

        /// <summary>
        /// Nombre centro de servicio
        /// </summary>
        [DataMember]
        public string NombreCentroServicio { get; set; }

        /// <summary>
        /// Centro de Costos del centro de servicio
        /// </summary>
        [DataMember]
        public string CentroCostos { get; set; }

        /// <summary>
        /// Direccion del Centro de Servicio
        /// </summary>
        [DataMember]
        public string DireccionCentroServicio { get; set; }

        /// <summary>
        /// Telefono CentroServicio
        /// </summary>
        [DataMember]
        public string TelefonoCentroServicio { get; set; }

        /// <summary>
        /// Identificador col al que pertenece el usuario
        /// </summary>
        [DataMember]
        public long IdCol { get; set; }

        /// <summary>
        /// Nombre Col al que pertenece el usuario
        /// </summary>
        [DataMember]
        public string NombreCol { get; set; }

        /// <summary>
        /// Direccion del Col
        /// </summary>
        [DataMember]
        public string DireccionCol { get; set; }

        /// <summary>
        /// Telefono Col
        /// </summary>
        [DataMember]
        public string TelefonoCol { get; set; }

        /// <summary>
        /// Identificador racol al que pertenece el usuario
        /// </summary>
        [DataMember]
        public long IdRacol { get; set; }

        /// <summary>
        /// Identificador de la casa matriz a la cual pertenece el centro de servicio
        /// </summary>
        [DataMember]
        public int IdCasaMAtriz { get; set; }

        /// <summary>
        /// Retorno a asigna el tipo de tipo centro servicios.
        /// </summary>
        /// <value>
        /// Tipo de centro servicios.
        /// </value>
        [DataMember]
        public string TipoCentroServicios { get; set; }

        /// <summary>
        /// Nombre raCol al que pertenece el usuario
        /// </summary>
        [DataMember]
        public string NombreRaCol { get; set; }

        /// <summary>
        /// Direccion del RACOL
        /// </summary>
        [DataMember]
        public string DireccionRaCol { get; set; }

        /// <summary>
        /// Telefono RACOL
        /// </summary>
        [DataMember]
        public string TelefonoRaCol { get; set; }

        /// <summary>
        /// Caja a la que corresponde el centro de servicio
        /// </summary>
        [DataMember]
        public int Caja { get; set; }

        [DataMember]
        public string IdCiudadCentroServicio { get; set; }

        [DataMember]
        public string DescCiudadCentroServicio { get; set; }

        [DataMember]
        public string DescCiudadCentroServicioCompleta { get; set; }

        [DataMember]
        public string CodigoPostal { get; set; }

        [DataMember]
        public string IdPaisCentroServicio { get; set; }

        [DataMember]
        public string DescPaisCentroServicio { get; set; }

        [DataMember]
        public decimal BaseInicialCaja { get; set; }

        [DataMember]
        public bool Biometrico { get; set; }

    }
}