using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana
{
    public class ActualizarRecogidaRequest
    {
        public long idRecogida { get; set; }
        public DateTime FechaRecogida { get; set; }
        public short CantidadEnvios { get; set; }
        public decimal PesoAproximado { get; set; }
        public string Documento { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Complemento { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public short IdEstado { get; set; }
    }
}