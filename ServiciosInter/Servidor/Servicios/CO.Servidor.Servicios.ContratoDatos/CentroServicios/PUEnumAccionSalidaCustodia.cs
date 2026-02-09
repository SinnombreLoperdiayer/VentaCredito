using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.CentroServicios
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum PUEnumAccionSalidaCustodia : short
    {
        [EnumMember]
        DESTRUCCIÓN = 52,
        [EnumMember]
        DONACIÓN = 53,
        [EnumMember]
        VENTAINTERNA = 54,
    }
}

