using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class ADArchivoAlmacenGuia
  {
    [DataMember]
    public ADGuia Guia { get; set; }

    [DataMember]
    public string Archivo { get; set; }

    [DataMember]
    public long IdMensajero { get; set; }

    [DataMember]
    public string NombreMensajero { get; set; }
  }
}
