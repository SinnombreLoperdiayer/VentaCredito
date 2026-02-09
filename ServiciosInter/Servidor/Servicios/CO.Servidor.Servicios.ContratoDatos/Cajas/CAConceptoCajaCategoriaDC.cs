using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
    /// <summary>
    /// Clase que contiene los conceptos de
    /// la Caja
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAConceptoCajaCategoriaDC
    {
        /// <summary>
        /// Identificador de la categoría del concepto de caja
        /// </summary>
        [DataMember]
        public int IdCategoria { get; set; }

        /// <summary>
        /// Identificador del concepto de caja
        /// </summary>
        [DataMember]
        public string Descripcion { get; set; }
    }
}