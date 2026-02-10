using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAMovEmpresaBancoDC : DataContractBase
  {
    /// <summary>
    /// Es el id de la Caja de la Empresa
    /// </summary>
    /// <value>
    /// The id caja empresa.
    /// </value>
    [DataMember]
    public long IdCajaEmpresa { get; set; }

    /// <summary>
    /// Es el Id de la caja del Banco
    /// </summary>
    /// <value>
    /// The id caja banco.
    /// </value>
    [DataMember]
    public long IdCajaBanco { get; set; }

    /// <summary>
    /// Fecha en la que se inserta el registro
    /// </summary>
    /// <value>
    /// The fecha grabacion.
    /// </value>
    [DataMember]
    public DateTime FechaGrabacion { get; set; }

    /// <summary>
    /// Usuario que realiza el regisro
    /// </summary>
    /// <value>
    /// The creado por.
    /// </value>
    [DataMember]
    public string CreadoPor { get; set; }
  }
}