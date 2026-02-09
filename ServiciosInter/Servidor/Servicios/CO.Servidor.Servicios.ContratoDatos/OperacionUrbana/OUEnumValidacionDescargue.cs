using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public enum OUEnumValidacionDescargue : short
    {
        [EnumMember]
        SinAsignar = 0,
        [EnumMember]
        Exitosa = 1,
        [EnumMember]
        Notificacion = 2,
        [EnumMember]
        Rapiradicado = 3,
        [EnumMember]
        ErrorEstado = 4,
        [EnumMember]
        Error = 5,
        [EnumMember]
        ErrorSolicitudRaps = 6
    }
}