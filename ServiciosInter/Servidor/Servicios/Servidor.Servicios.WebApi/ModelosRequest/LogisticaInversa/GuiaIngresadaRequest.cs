using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa
{
    public class GuiaIngresadaRequest
    {
        public short CantidadReintentosEntrega { get; set; }
        public string Ciudad { get; set; }
        public LIEvidenciaDevolucionDC EvidenciaDevolucion { get; set; }
        public DateTime FechaMotivoDevolucion { get; set; }
        public long IdAdmision { get; set; }
        public string IdCiudad { get; set; }
        public ADMotivoGuiaDC Motivo { get; set; }
        public COTipoNovedadGuiaDC Novedad { get; set; }
        public long? NumeroGuia { get; set; }
        public string Observaciones { get; set; }
        public long Planilla { get; set; }
        public ADEnumTipoImpreso TipoImpreso { get; set; }
    }
}