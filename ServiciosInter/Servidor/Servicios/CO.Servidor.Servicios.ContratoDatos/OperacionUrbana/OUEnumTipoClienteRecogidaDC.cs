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
    public enum OUEnumTipoClienteRecogidaDC
  {
    /// <summary>
    /// Cliente tipo Convenio
    /// </summary>
    [EnumMember]
    CON,

    /// <summary>
    /// Cliente tipo Peaton
    /// </summary>
    [EnumMember]
    PEA,

    /// <summary>
    /// Cliente tipo Punto de servicio
    /// </summary>
    [EnumMember]
    PSE
  }
}