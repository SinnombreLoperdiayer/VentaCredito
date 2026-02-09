using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Servidor.OperacionNacional.Comun
{
    /// <summary>
    /// Clase con las constantes necesarias para la operacion nacional
    /// </summary>
    public class OPConstantesOperacionNacional
    {
        /// <summary>
        /// Constante para identificar el tipo de transporte propio
        /// </summary>
        public const int ID_TIPO_TRANSPORTE_PROPIO = 1;

        /// <summary>
        /// Constante para identificar el tipo de transporte aforado
        /// </summary>
        public const int ID_TIPO_TRANSPORTE_AFORADO = 2;

        /// <summary>
        /// Constante para el parametro de peso maximo de los envios consolidados
        /// </summary>
        public const string ID_PARAMETRO_PESO_MAX_ENVIOS_CONSOLIDADOS = "PesoMaxEnviosConso";

        /// <summary>
        /// Constante para el parametro de peso maximo de los envios consolidados
        /// </summary>
        public const string ID_PARAMETRO_ESTADO_EMPAQUE_ENVIO_TRANSITO = "IdEstadoEmpaTrans";

        /// <summary>
        /// COntante para el parametro de peso maximo de los envios sueltos del manifiesto
        /// </summary>
        public const string ID_PARAMETRO_PESO_MAX_ENVIOS_SUELTOS = "PesoMaxEnviosSueltos";

        /// <summary>
        /// COntante para el parametro de peso maximo de los envios sueltos del manifiesto
        /// </summary>
        public const string ID_PARAMETRO_DESFASE_PESO = "DesfasePesoIngreso";

        /// <summary>
        /// COntante para el parametro de usuario
        /// </summary>
        public const string ID_PARAMETRO_USUARIO = "Usuario";

        // <summary>
        /// COntante para el parametro para el id del manifiesto
        /// </summary>
        public const string ID_PARAMETRO_ID_MANIFIESTO = "IdManifiestoOperacionNal";

        /// <summary>
        /// Constante para el id del tipo de operativo por ruta
        /// </summary>
        public const short ID_TIPO_OPERATIVO_RUTA = 1;

        /// <summary>
        /// Constante para el id del tipo de operativo por ruta
        /// </summary>
        public const short ID_TIPO_OPERATIVO_CIUDAD = 2;

        /// <summary>
        /// Constante para el id del tipo de operativo por Manifiesto
        /// </summary>
        public const short ID_TIPO_OPERATIVO_MANIFIESTO = 3;

        /// <summary>
        /// Constante para el id de la falla en la novedad del consolidado
        /// </summary>
        public const int ID_FALLA_NOVEDAD_CONSOLIDADO = 10;

        /// <summary>
        /// Constante para el id de la falla en la novedad del consolidado
        /// </summary>
        public const int ID_FALLA_ESTADO_EMPAQUE = 12;

        /// <summary>
        /// Constante para el estado del empaque sin bolsa de seguridad
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_SIN_BOLSA_SEGURIDAD = 2;

        /// <summary>
        /// Constante para el estado del empaque sin bolsa de seguridad
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_CON_BOLSA_SEGURIDAD = 1;

        /// <summary>
        /// Constante para el estado del empaque sin bolsa de seguridad
        /// </summary>
        public const int ID_FALLA_DIFERENCIA_PESO = 13;

        /// <summary>
        /// Constante para el estado del empaque mal embalado
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_MAL_EMBALADO = 4;

        /// <summary>
        /// Constante para el estado del empaque mal embalado
        /// </summary>
        public const int ID_ESTADO_EMPAQUE_BIEN_EMBALADO = 3;

        /// <summary>
        /// compañia de seguros, para poliza general de InterRapidisimo
        /// </summary>
        public const string NOMBRE_ASEGURADORA_GENERAL = "SegGenEmpNomAsegura";

        /// <summary>
        /// numero de la poliza de seguros general de interrapidisimo
        /// </summary>
        public const string NUMERO_POLIZA_ASEGURADORA_GENERAL = "SeguGenEmpNumPoliza";

        /// <summary>
        /// Constante del nombre de la planilla del Reporte del control de paradas de las estaciones
        /// </summary>
        public const string REPORTE_CONTROL_ESTACIONES = "/OperacionNacional/ReporteManifiestoCargaParadas.aspx";

        /// <summary>
        /// Constante del nombre del reporte de manifiesto detallado por estacion
        /// </summary>
        public const string REPORTE_MANIFIESTO_DETALLADO_ESTACION = "/OperacionNacional/ReporteManifiestoDetalladoEstacion.aspx";

        /// <summary>
        /// Constante del nombre del reporte de manifiesto completo por estacion
        /// </summary>
        public const string REPORTE_MANIFIESTO_COMPLETO_ESTACION = "/OperacionNacional/ReporteManifiestoCompleto.aspx";

        /// <summary>
        /// Constante con el nombre del parametro de IdRuta
        /// </summary>
        public const string PARAMETRO_IDRUTA = "IdRuta";

        /// <summary>
        /// Constante con el nombre del parametro usuario
        /// </summary>
        public const string PARAMETRO_NUM_MANIFIESTO_CARGA = "NumManCarga";

        /// <summary>
        /// Constante con el mombre del parametro IdManifiestoOperacionNacional
        /// </summary>
        public const string PARAMETROS_NUMERO_INTERNO_MANIFIESTO = "NumInternoMan";

        /// <summary>
        /// Constante con el mombre del parametro IdManifiestoOperacionNacional
        /// </summary>
        public const string TIPO_CLIENTE_INTERNO = "INT";

        /// <summary>
        /// Constante con el mombre del parametro IdManifiestoOperacionNacional
        /// </summary>
        public const string DETALLE_SI = "Si";

        /// <summary>
        /// Constante con el mombre del parametro IdManifiestoOperacionNacional
        /// </summary>
        public const string DETALLE_NO = "No";

        /// <summary>
        /// Constante con el tipo de consecutivo del manifiesto del ministerio
        /// </summary>
        public const int TIPO_CONSECUTIVO_MANIFIESTO_MINISTERIO = 2;

        public const string ID_PARAMETRO_INGRESO_OPERATIVO = "IdIngresoOperativo";

        /// <summary>
        /// Contiene la url del reporte de faltantes de sobrantes de descargue
        /// </summary>
        public const string REPORTE_FALTANTES_SOBRANTES_DESCARGUE = "/OperacionNacional/InconsistenciasIgresoOPN.aspx";

        /// <summary>
        /// Es el id de la Novedad sin novedad del desacrgue de Consolidados
        /// </summary>
        public const int ID_SIN_NOVEDAD_CONSOLIDADO = 5;

        /// <summary>
        /// Es el id de la Novedad sin novedad del desacrgue de las guias
        /// </summary>
        public const int ID_SIN_NOVEDAD_ENVIOS_SUELTOS = 8;

        public const int ID_TIPO_RUTA_REGIONAL = 2;

        public const int ID_TIPO_RUTA_NACIONAL = 1;
    }
}