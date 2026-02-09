using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    /// <summary>
    /// Enumeración para clasificar el propietario de un suminitro en terminos de su grupo
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public enum SUEnumGrupoSuministroDC
  {
        /// <summary>
        /// Grupo Procesos
        /// </summary>
        [EnumMember]
        PRO,

        /// <summary>
        /// Grupo Sucursales
        /// </summary>
        [EnumMember]
        CLI,

        /// <summary>
        /// Grupo Mensajeros
        /// </summary>
        [EnumMember]
        MEN,

        /// <summary>
        ///  Grupo de agencias
        /// </summary>
        [EnumMember]
        AGE,

        /// <summary>
        ///  Grupo de RACOL
        /// </summary>
        [EnumMember]
        RAC,

        /// <summary>
        ///  Grupo de Puntos
        /// </summary>
        [EnumMember]
        PTO,
        
        //TODO PARRA: VERIFICAR ESTO
        [EnumMember]
        BDG
  }
}