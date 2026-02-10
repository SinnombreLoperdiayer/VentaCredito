namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Recogidas
{
    public enum EnumEstadoSolicitudRecogida : int
    {
        None = 0,
        Creado = 1,
        Reservado = 2,
        ParaForzar = 3,
        CanceladoPorElCliente = 4,
        Realizada = 5,
        Cancelada = 6,
        Telemercadeo = 8,
        Forzada = 9,
    }
}