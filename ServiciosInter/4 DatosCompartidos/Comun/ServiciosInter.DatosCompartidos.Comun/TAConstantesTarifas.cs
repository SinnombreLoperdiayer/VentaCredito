namespace ServiciosInter.DatosCompartidos.Comun
{
    /// <summary>
    /// Clase que contiene las constantes de Tarifas
    /// </summary>
    public class TAConstantesTarifas
    {
        /// <summary>
        /// Identificador del tipo de valor adicional de retorno del servicio rapi carga
        /// </summary>
        public const string ID_TIPO_VALOR_ADICIONAL_RETORNO = "TSR";

        /// <summary>
        /// Identificador del tipo de valor adicional de retorno del servicio rapi carga contrapago
        /// </summary>
        public const string ID_TIPO_VALOR_ADICIONAL_RETORNO_RAPI_CARGA_CONTRAPAGO = "SRC";

        /// <summary>
        /// Identificador tipo subtrayecto de kilo adicional
        /// </summary>
        public const string ID_TIPO_SUBTRAYECTO_KILO_INICIAL = "SKI";

        /// <summary>
        /// Valor del kilo inicial para la excepción de notificaciones
        /// </summary>
        public const decimal VALOR_KILO_INICIAL_EXCEPCION_NOTIFICACIONES = 1;

        /// <summary>
        /// Identificador del tipo trayecto especial o de díficil acceso
        /// </summary>
        public const string ID_TIPO_TRAYECTO_ESPECIAL = "ESPECIAL";

        /// <summary>
        /// Identificador para el estado activo
        /// </summary>
        public const string ID_ESTADO_ACTIVO = "ACT";

        /// <summary>
        /// Identificador para el valor adicional servicio rapi radicado
        /// </summary>
        public const string ID_TIPO_VALOR_ADICIONAL_SERVICIO_RAPIRADICADO = "SRR";

        /// <summary>
        /// Nombre Parametro Valor Dolar cuan dno hay Sistema
        /// </summary>
        public const string PARAMETRO_CONSULTA_VALOR_DOLAR = "ValorDolar";
    }
}