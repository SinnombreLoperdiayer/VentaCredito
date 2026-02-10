using System;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    public class RAPersonaDC
    {
        public string IdCargo { get; set; }

        public string IdCargoNovasoft { get; set; }

        public string Descripcion { get; set; }

        public bool Estado { get; set; }

        public int IdProcedimiento { get; set; }

        public string NombreCompleto { get; set; }

        public long NumeroDocumento { get; set; }

        public string TipoIdentificacion { get; set; }

        public string Email { get; set; }

        public string Proceso { get; set; }

        public string Telefono { get; set; }

        public string DescripcionTarea { get; set; }

        public DateTime FechaEjecucionTarea { get; set; }
    }
}
