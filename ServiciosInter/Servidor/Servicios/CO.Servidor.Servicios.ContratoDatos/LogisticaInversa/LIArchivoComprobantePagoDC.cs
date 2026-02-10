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
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIArchivoComprobantePagoDC : DataContractBase
  {
    [DataMember]
    public long IdArchivo { get; set; }

    [DataMember]
    public string Archivo { get; set; }

    /// <summary>
    /// En la opcion de digitalizar comprobantes de pago (GIRO), en esta propiedad se almacena el numero del comprobante de pago
    /// </summary>
    [DataMember]
    [Display(Name = "Número Comprobante")]
    public string ValorDecodificado { get; set; }

    [DataMember]
    public long? IdAdmisionGiro { get; set; }

    [DataMember]
    public long? NumeroGiro { get; set; }

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

    /// <summary>
    /// Indica si el giro fue decoficada  o digitado el codigo de barras de manera manual
    /// </summary>
    [DataMember]
    public bool Decodificada { get; set; }

    /// <summary>
    /// Indica si el giro fue escaneado o ingresada de manera manual
    /// </summary>
    [DataMember]
    public bool Manual { get; set; }
  }
}