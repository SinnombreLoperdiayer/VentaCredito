using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Servicios.ContratoDatos.LogisticaInversa
{
    /// <summary>
    /// Clase que contiene la información de archivo giro
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class LIDescargueGuiaDC : DataContractBase
    { 
         [DataMember]
         public OUEnumValidacionDescargue Resultado { get; set; }

         [DataMember]
         public OUGuiaIngresadaDC Guia { get; set; }


         [DataMember]
         public ADEnumEstadoGuia Estado { get; set; }


         [DataMember]
         public string Mensaje { get; set; }

    }
}
