using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Recogidas
{
    public class RGRecogidasDC
    {
        public long? Id { get; set; }
        public string NumeroDocumento { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string NombreCiudad { get; set; }
        public int? idCliente { get; set; }
        public int? IdCentroServicio { get; set; }
        public int? IdSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string NombreCentroServicio { get; set; }
        public string PreguntarPor { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaRecogida { get; set; }
        public RGEnumTipoRecogidaDC TipoRecogida { get; set; }
        public string CodigoVerificacion { get; set; }
        public long IdAsignacion { get; set; }
        public long IdProgramacion { get; set; }
        public string Correo { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroTelefono { get; set; }
        public string DescripcionEnvios { get; set; }
        public int TotalPiezas { get; set; }
        public int PesoAproximado { get; set; }
        public bool EstaForzada { get; set; }
        public bool EsEsporadicaCliente { get; set; }
        public decimal Cantidad { get; set; }
        public long IdHorarioCentroServicio { get; set; }
        public long IdHorarioSucursal { get; set; }
        public string DescripcionHorario { get; set; }
        public DateTime Hora { get; set; }
        public int DiaSemana { get; set; }
        public long Paginas { get; set; }
    }
}