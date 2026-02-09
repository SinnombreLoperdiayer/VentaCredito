using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria.AdmMasiva
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADDatosValidadosDC
  {
    [DataMember]
    public int NoFila { get; set; }

    [DataMember]
    public ADValidacionServicioTrayectoDestino ValidacionTrayectoDestino { get; set; }

    [DataMember]
    public TAPrecioMensajeriaDC PrecioMensajeria { get; set; }

    [DataMember]
    public string Error { get; set; }
  }
}