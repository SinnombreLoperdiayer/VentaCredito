using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Cliente.Servicios.ContratoDatos;

namespace Framework.Servidor.Servicios.ContratoDatos.Parametros
{
  /// <summary>
  /// Clase que contiene los elementos de la Tbla
  /// CuentaBancoEmpresa
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PACuentaBancoEmpresaDC : DataContractBase
  {
    [DataMember]
    public string IdBanco { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Etiquetas), Name = "NumeroCuenta", Description = "ToolTipNumeroCuenta")]
    public string NumeroCuenta { get; set; }

    [DataMember]
    public string TipoCta { get; set; }
  }
}