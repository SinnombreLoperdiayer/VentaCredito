using CO.Cliente.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
    public class ONNovedadEstacionRutaxGuiaDC
    {
        /// <summary>
        /// Cantidad Horas de la Novedad
        /// </summary>
        [DataMember]
        public ONNovedadEstacionRutaDC NovedadRutaEstacion { get; set; }
        /// <summary>
        /// Cantidad Minutos de la Novedad
        /// </summary>
        [DataMember]
        public List<long> ListaGuias { get; set; }

    }
}