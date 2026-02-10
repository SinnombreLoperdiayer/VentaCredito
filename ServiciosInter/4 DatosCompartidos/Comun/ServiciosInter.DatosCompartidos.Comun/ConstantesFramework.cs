using System;

namespace ServiciosInter.DatosCompartidos.Comun
{
    /// <summary>
    /// Clase para manejo de constantes propias del framework
    /// </summary>
    public class ConstantesFramework
    {
        private static DateTime maxDateParaControlDatePicker = DateTime.MaxValue.AddYears(-1).AddDays(-1);

        /// <summary>
        /// Retorna la fecha mínima que puede manejar el sistema
        /// </summary>
        /// <remarks>Compatible con los mínimos tanto de SqlServer como de Oracle en la actualidad.  Esta fecha es útil para fechas de referencia mínimas y para almacenar en tipos DateTime en base de datos parte de la hora</remarks>
        public static DateTime MinDateTimeController
        {
            get { return new DateTime(1753, 01, 01, 0, 0, 0); }
        }

        /// <summary>
        /// Retorna la fecha máxima permitida para un control de DatePicker
        /// </summary>
        public static DateTime MaxDateParaControlDatePicker
        {
            get { return maxDateParaControlDatePicker; }
        }

        /// <summary>
        /// Nombre de la cultura preferida:  "es-co"
        /// </summary>
        public const string NOMBRE_CULTURA_PREFERIDA = "es-co";

        /// <summary>
        /// Constante para identificar al framerwork (valor = FW)
        /// </summary>
        public const string MODULO_FRAMEWORK = "FW";

        /// <summary>
        /// Constante para identificar al módulo de seguridad del framerwork (valor = FWSEG)
        /// </summary>
        public const string MODULO_FW_SEGURIDAD = "FWSEG";

        /// <summary>
        /// Constante para identificar al módulo de reportes del framerwork (valor = FWREP)
        /// </summary>
        public const string MODULO_FW_REPORTES = "FWREP";

        /// <summary>
        /// Constante para identificar al framework de versionamiento (valor = FWVER)
        /// </summary>
        public const string MODULO_FW_VERSIONAMIENTO = "FWVER";

        /// <summary>
        /// Constante para identificar al framework de Agenda (valor = FWAGE)
        /// </summary>
        public const string MODULO_FW_AGENDA = "FWAGE";

        /// <summary>
        /// Constante para identificar al framework de Agenda (valor = FWAGE)
        /// </summary>
        public const string MODULO_FW_PLANTILLAS = "FWPLA";

        /// <summary>
        /// Constante para identificar al framework de Agenda (valor = FWAGE)
        /// </summary>
        public const string MODULO_FW_MENSAJERIA = "FWMEN";

        /// <summary>
        /// Constante para identificar a los parametros del framework
        /// </summary>
        public const string PARAMETROS_FRAMEWORK = "PARFW";

        /// <summary>
        /// Estado activo (valor = ACT)
        /// </summary>
        public const string ESTADO_ACTIVO = "ACT";

        /// <summary>
        ///  Estado inactivo (valor = INA)
        /// </summary>
        public const string ESTADO_INACTIVO = "INA";

        /// <summary>
        ///  Estado Anulado (valor = ANU)
        /// </summary>
        public const string ESTADO_ANULADO = "ANU";

        /// <summary>
        /// Valor del tipo de autenticación operativo
        /// </summary>
        public const string TIPO_AUTENTICACION_OPERATIVO = "OPE";

        /// <summary>
        /// Valor del tipo de autenticación administrativo
        /// </summary>
        public const string TIPO_AUTENTICACION_ADMINISTRATIVO = "ADM";

        /// <summary>
        /// Tipo de documento NIT
        /// </summary>
        public const string TIPO_DOCUMENTO_NIT = "NI";

        /// <summary>
        /// Tipo de documento Cédula de Ciudadania
        /// </summary>
        public const string TIPO_DOCUMENTO_CC = "CC";

        /// <summary>
        /// Tipo de entrega "Domicilio", este debe ser el valor por defecto en el tipo de entrega
        /// </summary>
        public const string TIPO_ENTREGA_DOMICILIO = "DOM";

        /// <summary>
        /// Tipo de entrega "Entrega en dirección"
        /// </summary>
        public const string TIPO_ENTREGA_EN_DIRECCION = "1";

        /// <summary>
        /// Tipo de entrega "Reclame en Oficina"
        /// </summary>
        public const string TIPO_ENTREGA_RECLAME_EN_OFICINA = "2";

        /// <summary>
        /// Descripcion de usuario sin centro de servicio
        /// </summary>
        public const string SIN_CENTRO_SERVICIO = "NINGUNO";

        /// <summary>
        /// Descripcion de usuario Sistema
        /// </summary>
        public const string USUARIO_SISTEMA = "Sistema";

        /// <summary>
        /// Es el id del usuario para realizar el proceso del cierre automatico
        /// de cajas y puntos
        /// </summary>
        public const long ID_USUARIO_SISTEMA = -1;

        /// <summary>
        /// Descripcion de usuario Gestion (USUARIO DE DIRECCCION GENERAL)
        /// </summary>
        public const string USUARIO_GESTION = "UsuarioGestion";

        /// <summary>
        /// Documento del usuario Sistema
        /// </summary>
        public const string USUARIO_SISTEMA_DOCUMENTO = "99999999";

        public const string NOMBRE_EMPRESA = "INTERRAPIDISMO";

        /// <summary>
        /// Es el tipo de Zona internacional
        /// </summary>
        public const int TIPO_ZONA_INTERNACIONAL = 1;

        /// <summary>
        /// Mensaje de mostrar toda la Info del Filtro
        /// en un combo
        /// </summary>
        public const string MOSTRAR_TODOS = "Mostrar Todos...";

        /// <summary>
        /// Constante con la expresion regular para  validar el telefono
        /// </summary>
        public const string REGEX_VALIDACION_TELEFONO = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([0-9]1[0-9]|[0-9][0-8]1|[0-9][0-8][0-9])\s*\)|([0-9]1[0-9]|[0-9][0-8]1|[0-9][0-8][0-9]))\s*(?:[.-]\s*)?)?([0-9]1[0-9]|[0-9][0-9]1|[0-9][0-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension\.?|EXT)\s*(\d+))?$";

        /// <summary>
        /// Constante con la expresion regular para  validar el correo electronico
        /// </summary>
        public const string REGEX_VALIDACION_EMAIL = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

        public const string TIPO_LOCALIDAD_PAIS = "1";

        public const string TIPO_LOCALIDAD_DEPTO = "2";

        public const string TIPO_LOCALIDAD_MUNICIPIO = "3";

        public const string TIPO_LOCALIDAD_CORREGIMIENTO = "4";

        public const string TIPO_LOCALIDAD_INSPECCION = "5";

        public const string TIPO_LOCALIDAD_CASERIO = "6";

        public const string ID_LOCALIDAD_COLOMBIA = "057";

        public const string DESC_LOCALIDAD_COLOMBIA = "Colombia";

        public const string ID_LOCALIDAD_BOGOTA = "11001000";

        public const string DESC_LOCALIDAD_BOGOTA = "Bogota";

        public const string SELECCIONE_COMBO = "-Seleccione-";

        #region Centro de Servicios

        /// <summary>
        /// Tipo de Centro de Servicios RACOL: RAC
        /// </summary>
        public const string TIPO_CENTRO_SERVICIO_RACOL = "RAC";

        /// <summary>
        /// Tipo de Centro de Servicios Punto: PTO
        /// </summary>
        public const string TIPO_CENTRO_SERVICIO_PUNTO = "PTO";

        /// <summary>
        /// Tipo de Centro de Servicios Agencia: AGE
        /// </summary>
        public const string TIPO_CENTRO_SERVICIO_AGENCIA = "AGE";

        /// <summary>
        /// Tipo de Centro de Servicios COL: COL
        /// </summary>
        public const string TIPO_CENTRO_SERVICIO_COL = "COL";

        /// <summary>
        /// Nombre del tipo de centro de servicios para RACOL: RACOL
        /// </summary>
        public const string TIPO_CENTRO_SERVICIOS_RACOL_NOMBRE = "RACOL";

        #endregion Centro de Servicios

        #region Digitalización

        /// <summary>
        /// Esta es la Ruta Temporal
        /// donde se guardan las imagenes
        /// digitalizadas por el Scanner
        /// </summary>
        public static string RUTA_TEMP_GUARDADO_IMAGENES = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Imagenesescaner";

        #endregion Digitalización

        #region Cache

        /// <summary>
        /// Nombre de la llave de ciudades para establecer y / o consultar
        /// </summary>
        public const string CACHE_CIUDADES = "Ciudades";

        /// <summary>
        /// Nombre de la llave de centros de servicio activos para establecer y / o consultar
        /// </summary>
        public const string CACHE_CENTROS_SERVICIO_ACTIVOS = "CentrosServicio";

        /// <summary>
        /// Nombre de la llave de todos los centros de servicio del sistema
        /// </summary>
        public const string CACHE_TODOS_CENTROS_SERVICIO_SISTEMA = "TodosCentrosServicio";

        /// <summary>
        /// Nombre de la llave de los Cols para establecer y / o consultar
        /// </summary>
        public const string CACHE_CENTROS_COLS = "Cols";

        /// <summary>
        /// Nombre de la llave de clientes crédito activos
        /// </summary>
        public const string CACHE_CLIENTES_CREDITO_ACTIVOS = "ClientesCreditoActivos";

        /// <summary>
        /// Nombre de la llave para consultar y o establecer todos los mensajeros
        /// </summary>
        public const string CACHE_MENSAJEROS = "Mensajeros";

        /// <summary>
        /// Nombre de la llave de paises para establecer y / o consultar
        /// </summary>
        public const string CACHE_PAISES = "Paises";

        /// <summary>
        /// Nombre de las sucursales activas para establecer y / o consultar
        /// </summary>
        public const string CACHE_SUCURSALES_ACTIVAS = "SucursalesActivas";

        /// <summary>
        /// Nombre de la llave de los servicios ofrecidos por la compañía
        /// </summary>
        public const string CACHE_SERVICIOS = "Servicios";

        /// <summary>
        /// Nombre de la llave que indica si se debe hacer integración con el sistema "Mensajero"
        /// </summary>
        public const string CACHE_INTEGRA_CON_MENSAJERO = "IntegraConMensajero";

        /// <summary>
        /// Nombre de la llave de las agencias que pueden pagar giros
        /// </summary>
        public const string CACHE_AGENCIA_PAGAN_GIRO = "AgenciasPaganGiros";

        /// <summary>
        /// Nombre de la llave de los centros de servicios asociados a un RACOL
        /// </summary>
        public const string CACHE_AGENCIAS_POR_RACOL = "AgenciasPorRACOL";

        /// <summary>
        /// Nombre de la llave de los centros de servicios Nacional
        /// </summary>
        public const string CACHE_TODAS_LAS_AGENCIAS = "TodasAgenciasNacional";

        /// <summary>
        /// Nombre de la llave de Los rangos de precios del servicio de Giros
        /// </summary>
        public const string CACHE_RANGOS_PRECIOS_GIROS = "RangosPrecioGiros";

        /// <summary>
        /// Nombre de la llave que contiene el valor del dólar en pesos colombianos
        /// </summary>
        public const string CACHE_DOLAR_EN_PESOS = "CacheDolarEnPesos";

        /// <summary>
        /// Nombre de la llace que contiene la lista con las lineas de los vehiculos
        /// </summary>
        public const string CACHE_LINEA_VEHICULOS = "CacheLineaVehiculos";

        /// <summary>
        /// Número maximo de registros a devolver en una consulta
        /// </summary>
        public const string CACHE_MAXIMO_REGISTROS = "CacheMaximoRegistros";

        /// <summary>
        /// Cache del cliente contado
        /// </summary>
        public const string CACHE_CLIENTE_CONTADO = "CacheClienteContado";

        /// <summary>
        /// Cache de Contratos de Sucursales Activas
        /// </summary>
        public const string CACHE_CONTRATOS_SUCURSALES_ACTIVAS = "CacheContratosSucursales";

        #endregion Cache

        #region Parámetros

        /// <summary>
        /// Constante para el parámetro del pais predeterminado
        /// </summary>
        public const string PARA_PAIS_DEFAULT = "PaisPredeterminado";

        /// <summary>
        /// Constante para el parámetro de la cultura del servidor de base de datos
        /// </summary>
        public const string PARA_CULTURA_SERVER = "Cultura";

        /// <summary>
        /// Constante para el parámetro de área interna default
        /// </summary>
        public const string AREA_DEFAULT = "AreaInternaDefault";

        /// <summary>
        /// Constante para el parámetro de tiempo de caducacion
        /// del cache de la regla de negocio.
        /// </summary>
        public const string TIEMPO_CADUCA_REGLA_INSTANCIA_NEGOCIO = "IdCaducaInstRegla";

        /// <summary>
        /// Constante con la identificación de la casa matriz que tiene la información de Interrapidisimo
        /// </summary>
        public const string ID_CASA_MATRIZ_EMPRESA = "IdCasaMatrizEmpresa";

        #endregion Parámetros
    }
}