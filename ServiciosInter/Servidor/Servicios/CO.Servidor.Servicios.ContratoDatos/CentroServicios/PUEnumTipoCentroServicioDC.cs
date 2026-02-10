
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    /// <summary>
    /// Clasificacion del tipo de centro de servicio
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public enum PUEnumTipoCentroServicioDC
    {
        /// <summary>
        /// Agencias
        /// </summary>    
        [EnumMember]
        AGE,

        /// <summary>
        /// RACOL
        /// </summary>
        [EnumMember]
        RAC,

        /// <summary>
        /// PUNTO SERVICIO
        /// </summary>
        [EnumMember]
        PTO,

        /// <summary>
        /// PAS
        /// </summary>
        [EnumMember]
        PAS,

        /// <summary>
        /// TODOS
        /// </summary>
        [EnumMember]
        ALL

    }
}
