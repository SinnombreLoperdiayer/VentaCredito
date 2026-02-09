using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Area;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase que contiene la informacion para aplicar una operación sobre la Caja Banco
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CABaseGestionCajasDC : DataContractBase
  {
    /// <summary>
    /// Es el id de la Casa Matriz
    /// </summary>
    [DataMember]
    public ARCasaMatrizDC CasaMatriz { get; set; }

    /// <summary>
    /// Es la Base inicial de la Caja de la Casa Matriz
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ValorBaseCajaCasaMatriz", Description = "ToolTipValorBaseCajaCasaMatriz")]
    public decimal BaseCasaMatriz { get; set; }

    /// <summary>
    /// Es la Base inicial de la Caja de
    /// Operacion Nacional
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ValorBaseCajaOperacionNal", Description = "ToolTipValorBaseCajaOperacionNal")]
    public decimal BaseOperacionNacional { get; set; }

    /// <summary>
    /// Estado del Registro
    /// </summary>
    [DataMember]
    public EnumEstadoRegistro EstadoRegistro { get; set; }
  }
}