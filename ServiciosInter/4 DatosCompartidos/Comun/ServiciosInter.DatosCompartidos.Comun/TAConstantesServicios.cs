using System.Collections.Generic;

namespace ServiciosInter.DatosCompartidos.Comun
{
    public class TAConstantesServicios
    {
        /// <summary>
        /// Unidad de negocio de mensajería
        /// </summary>
        public const string UNIDAD_MENSAJERIA = "MEN";

        /// <summary>
        /// Unidad de negocio de giros
        /// </summary>
        public const string UNIDAD_GIROS = "GIR";

        /// <summary>
        /// Unidad de negocio de carga
        /// </summary>
        public const string UNIDAD_CARGA = "CAR";

        /// <summary>
        /// Unidad de negocio de Interviajes
        /// </summary>
        public const string UNIDAD_INTERVIAJES = "INV";

        /// <summary>
        /// Unidad de negocio Masivos
        /// </summary>
        public const string UNIDAD_MASIVOS = "MAS";

        public const string UNIDAD_KOMPRECH = "KOM";

        #region Servicios

        /// <summary>
        /// Id Servicio de Giros
        /// </summary>
        public const int SERVICIO_GIRO = 8;

        /// <summary>
        /// Id Servicio Rapi Hoy
        /// Cambio a Servicios Agiles = Noche Hoy
        /// </summary>
        public const int SERVICIO_RAPI_HOY = 1;

        /// <summary>
        /// Id Servicio Rapi AM
        /// Cambio a Servicios Agiles = Manana
        /// </summary>
        public const int SERVICIO_RAPI_AM = 2;

        /// <summary>
        /// Id Servicio Mensajeria
        /// </summary>
        public const int SERVICIO_MENSAJERIA = 3;

        /// <summary>
        /// Id Servicio Rapi Masivos
        /// </summary>
        public const int SERVICIO_RAPI_MASIVOS = 4;

        /// <summary>
        /// Id Servicio Rapi Promocional
        /// </summary>
        public const int SERVICIO_RAPI_PROMOCIONAL = 5;

        /// <summary>
        /// Id Servicio Rapi Carga
        /// </summary>
        public const int SERVICIO_RAPI_CARGA = 6;

        /// <summary>
        /// Id Servicio Rapi Carga Contra Pago
        /// </summary>
        public const int SERVICIO_RAPI_CARGA_CONTRA_PAGO = 7;

        /// <summary>
        /// Id Servicio Interviajes
        /// </summary>
        public const int SERVICIO_INTER_VIAJES = 9;

        /// <summary>
        /// Id Servicio Trámites
        /// </summary>
        public const int SERVICIO_TRAMITES = 10;

        /// <summary>
        /// Id Servicio Internacional
        /// </summary>
        public const int SERVICIO_INTERNACIONAL = 11;

        /// <summary>
        /// Id Servicio Centro de Correspondencia
        /// </summary>
        public const int SERVICIO_CENTRO_CORRESPONDENCIA = 12;

        /// <summary>
        /// Id Servicio Rapi Personalizado
        /// Cambio a Servicios Agiles =  Noche
        /// </summary>
        public const int SERVICIO_RAPI_PERSONALIZADO = 13;

        /// <summary>
        /// Id Servicio Rapi envíos contrapago
        /// </summary>
        public const int SERVICIO_RAPI_ENVIOS_CONTRAPAGO = 14;

        /// <summary>
        /// Id Servicio Notificaciones
        /// </summary>
        public const int SERVICIO_NOTIFICACIONES = 15;

        /// <summary>
        /// Id Servicio Rapi Radicado
        /// </summary>
        public const int SERVICIO_RAPIRADICADO = 16;

        /// <summary>
        /// Id Servicio Carga Express
        /// </summary>
        public const int SERVICIO_CARGA_EXPRESS = 17;

        public const int SERVICIO_KOMPRECH = 18;

        /// <summary>
        /// Cambio a Servicios Agiles = Tarde
        /// </summary>

        public const int SERVICIO_RAPI_TULAS = 19;

        /// <summary>
        /// Cambio a Servicios Agiles = Madrugada
        /// </summary>
        public const int SERVICIO_RAPI_VALORES_MENSAJERIA = 20;

        /// <summary>
        /// Cambio a Servicios Agiles = Tarde Noche
        /// </summary>

        public const int SERVICIO_RAPI_VALORES_CARGA = 21;

        /// <summary>
        /// Cambio a Servicios Agiles = Domingo Manana
        /// </summary>

        public const int SERVICIO_RAPI_CARGA_CONSOLIDADA = 22;

        /// <summary>
        /// Cambio a Servicios Agiles = Domingo tarde
        /// </summary>

        public const int SERVICIO_RAPI_VALIJAS = 23;

        public const int SERVICIO_CARGA_AEREA = 24;

        #endregion Servicios

        #region Formas de pago

        public const short ID_FORMA_PAGO_MIXTO = -1;

        /// <summary>
        /// Descripción del tipo de pago "Mixto". Este tipo de dato no se graba en la base de datos, me permite es desplegar una ventana donde se puede seleccionar
        /// más de una forma de pago.
        /// </summary>
        public const string DESCRIPCION_FORMA_PAGO_MIXTO = "Mixto";

        /// <summary>
        /// ID DE LA FORMA DE PAGO CONTADO
        /// </summary>
        public const short ID_FORMA_PAGO_CONTADO = 1;

        /// <summary>
        /// Descripcion de Forma de Pago Contado
        /// </summary>
        public const string DESCRIPCION_FORMA_PAGO_CONTADO = "Contado";

        /// <summary>
        /// Id de la forma de pago crédito, en admisiones solo para clientes crédito solo se puede usar esta forma de pago
        /// </summary>
        public const short ID_FORMA_PAGO_CREDITO = 2;

        /// <summary>
        /// Id de la forma de pago al cobro,
        /// </summary>
        public const short ID_FORMA_PAGO_AL_COBRO = 3;

        /// <summary>
        /// Descripcion de Forma de Pago Contado
        /// </summary>
        public const string DESCRIPCION_FORMA_PAGO_AL_COBRO = "AlCobro";

        /// <summary>
        /// Id de la forma de pago prepago,
        /// </summary>
        public const short ID_FORMA_PAGO_PREPAGO = 4;

        /// <summary>
        /// Descripcion de Forma de Pago Contado
        /// </summary>
        public const string DESCRIPCION_FORMA_PAGO_PREPAGO = "Prepago";

        /// <summary>
        /// Descripcion de Forma de Pago Credito
        /// </summary>
        public const string DESCRIPCION_FORMA_PAGO_CREDITO = "Crédito";

        /// <summary>
        /// Id de la forma de pago no Existe,
        /// </summary>
        public const short ID_FORMA_PAGO_DIF_DEMAS = 0;

        #endregion Formas de pago

        #region Tipo Servicio Nombres
        public static string ObtenerDescripcionTipoServicio(int valor)
        {
            Dictionary<int, string> Datos = new Dictionary<int, string>
        {
        { 1, "Rapidísimo Hoy" },
        { 2, "Rapidísimo AM" },
        { 3, "Mensajería" },
        { 4, "Rapi Masivos" },
        { 5, "Rapi Promocional" },
        { 6, "Carga Terrestre" },
        { 7, "Rapi Carga Contrapago" },
        { 8, "Giros" },
        { 9, "Inter Viajes" },
        { 10, "Trámites" },
        { 11, "Internacional" },
        { 12, "Centros de Correspon." },
        { 13, "Rapi Personalizado" },
        { 14, "Rapi Envíos Contrapago" },
        { 15, "Notificaciones" },
        { 16, "Radicado" },
        { 17, "Rapi Carga" },
        { 18, "Komprech" },
        { 19, "Rapi Tula" },
        { 20, "Rapi Valores Msj" },
        { 21, "Canje" },
        { 22, "Rapi Carga Consolidada" },
        { 23, "Rapi Valijas" },
        { 24, "Otros Ingresos" }
        };

            if (Datos.ContainsKey(valor))
            {
                return Datos[valor];
            }

            return default;

        }
        #endregion
    }


}