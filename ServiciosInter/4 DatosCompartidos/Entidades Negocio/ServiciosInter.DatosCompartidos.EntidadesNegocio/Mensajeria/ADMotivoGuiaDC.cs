namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADMotivoGuiaDC
    {
        public short IdMotivoGuia { get; set; }

        public string Descripcion { get; set; }

        //public ADEnumTipoMotivoDC Tipo { get; set; }

        public bool EsVisible { get; set; }

        public bool EsEscaneo { get; set; }

        public int MotivoCRC { get; set; }

        public bool SeReporta { get; set; }

        public bool CausaSupervision { get; set; }

        public string nombreAssembly { get; set; }

        public string @namespace { get; set; }

        public string nombreClase { get; set; }

        public int TiempoAfectacion { get; set; }

        public bool IntentoEntrega { get; set; }

        public long? IdTercero { get; set; }

        public string ObservacionMotivo { get; set; }
    }
}