using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.ModelosRequest.LogisticaInversa
{
    public class MotivoDescargueRequest
    {
        public short IdMotivoGuia { get; set; }
        public string Descripcion { get; set; }
        public int TiempoAfectacion { get; set; }
        public bool IntentoEntrega { get; set; }

        public bool EsEscaneo { get; set; }
    }
}