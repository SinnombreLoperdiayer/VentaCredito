using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa
{
    public class PlanillaRequest
    {

        public ADEnumTipoImpreso TipoPlanilla { get; set; }

        public ADEnumTipoCliente TipoCliente { get; set; }

        public string NombreTipoCliente { get; set; }

        public string NitClienteCredito { get; set; }

        public bool EsConsolidado { get; set; }

        public DateTime FechaGrabacion { get; set; }

        public string CreadoPor { get; set; }

        public long idCentroServicio { get; set; }

        public string nombreCentroServicios { get; set; }

    }
}