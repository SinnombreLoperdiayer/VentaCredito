using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  /// <summary>
  /// Clase que contiene la información de archivo giro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIArchivoGiroDC : DataContractBase
  {
    [DataMember]
    public long IdArchivo { get; set; }

    [DataMember]
    public string Archivo { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(CO.Cliente.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGiro")]
    public string ValorDecodificado { get; set; }

    [DataMember]
    public long? IdAdmisionGiro { get; set; }

    [DataMember]
    public bool ExisteGiro { get; set; }

    [DataMember]
    public bool ExisteArchivo { get; set; }

    [DataMember]
    public string Ruta { get; set; }

    [DataMember]
    public string RutaServidor { get; set; }

    [DataMember]
    public ADEnumResultadoEscaner ResultadoEscaner { get; set; }

    /// <summary>
    /// Indica si la imagen ya fué cargada en el servidor
    /// </summary>
    public bool Cargada
    {
      get;
      set;
    }

  }
}