using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa
{
    public class PlanillaDetalleRequest
    {

        public long IdPlanillaGuia { get; set; }

        public long NumeroPlanilla { get; set; }

        public long AdmisionNumeroGuia { get; set; }

        public long GuiaInternaIdCentroServicioOrigen { get; set; }

        public string GuiaInternaNombreCentroServicioOrigen { get; set; }

        public string GuiaInternaTelefonoRemitente { get; set; }

        public string GuiaInternaDireccionRemitente { get; set; }

        public bool GuiaInternaEsOrigenGestion { get; set; }

        public string PaisDefaultIdLocalidad { get; set; }

        public string PaisDefaultNombre { get; set; }

        public string LocalidadOrigenIdLocalidad { get; set; }

        public string LocalidadOrigenNombre { get; set; }

        public long GestionOrigenIdGestion { get; set; }

        public string GestionOrigenDescripcion { get; set; }

        public int caja { get; set; }

        public DateTime FechaGrabacion { get; set; }

        public string CreadoPor { get; set; }

        public string tipoIdNuevoDestinatario { get; set; }

        public string numeroIdNuevodestinatario { get; set; }

        public string nombreNuevoDestinatario { get; set; }

        public string direccionNuevoDestinatario { get; set; }

        public string idCiudadNuevoDestinatario { get; set; }

        public string nombreCiudadNuevoDestinatario { get; set; }

        public string telefonoNuevoDestinatario { get; set; }

        public string emailNuevoDestinatario { get; set; }

        public int valorTotal { get; set; }

        public int modificoDestinatario { get; set; }
    }
}