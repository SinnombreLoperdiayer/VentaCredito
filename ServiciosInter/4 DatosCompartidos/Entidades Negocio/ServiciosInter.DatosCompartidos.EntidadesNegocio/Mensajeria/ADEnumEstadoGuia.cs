namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public enum ADEnumEstadoGuia : short
    {
        SinEstado = 0,
        Admitida = 1,
        CentroAcopio = 2,
        TransitoNacional = 3,
        TransitoRegional = 4,
        ReclameEnOficina = 5,
        EnReparto = 6,
        IntentoEntrega = 7,
        Telemercadeo = 8,
        Custodia = 9,
        DevolucionRatificada = 10,
        Entregada = 11,
        Reenvio = 12,
        Digitalizada = 13,
        Indemnizacion = 14,
        Anulada = 15,
        Archivada = 16,
        DisposicionFinal = 17,
        TransitoUrbano = 18,
        Incautado = 21,
        PendienteIngresoaCustodia = 22,
        FisicoFaltante = 23,
        CasoFortuito = 24,
        NotaCredito = 26,
        IngresoABodega = 27,
        SalidadeBodega = 28,
        Auditoria = 29,
        DevolucionEsperaConfirmacionCliente = 30,
        Distribucion = 31,
        DevolucionRegional = 32,
        DevolverALaRacol = 33
    }

    /// <summary>
    /// Enum para definir valores de los estados de evaluación del envío
    /// </summary>
    public enum ADEstadoEvaluacionEnvio
    {
        /// <summary>
        /// Define VENCIDO cuando la fecha FechaEstimadaEntregaNew supera a la fecha actual
        /// </summary>
        Vencido = 0,
        /// <summary>
        /// Define A_TIEMPO cuando la fecha FechaEstimadaEntregaNew es menor a la fecha actual 
        /// </summary>
        A_Tiempo = 1
    }
}