using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class LIReexpedicionEnvioDC : DataContractBase
  {
    /// <summary>
    /// Guia inicial antes de realizar la reexpedicion
    /// </summary>
    [DataMember]
    public ADGuia GuiaEnvioInicial { get; set; }

    /// <summary>
    /// Guia de la reexpedicion del envio
    /// </summary>
    [DataMember]
    public ADGuia GuiaReexpedicion { get; set; }

    /// <summary>
    /// informacion del estado de la guia que se va a reexpedir
    /// </summary>
    [DataMember]
    public ADTrazaGuia EstadoGuiaEnvio { get; set; }

    /// <summary>
    /// Informacion del tipo de cliente del envio original
    /// </summary>
    [DataMember]
    public ADMensajeriaTipoCliente TipoClienteEnvioInicial { get; set; }

    /// <summary>
    /// Nueva ciudad destino del envio
    /// </summary>
    [DataMember]
    public PALocalidadDC CiudadDestino { get; set; }
  }
}