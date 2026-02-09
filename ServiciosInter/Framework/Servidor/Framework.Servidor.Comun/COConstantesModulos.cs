using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Framework.Servidor.Comun
{
    /// <summary>
    /// Constantes con los id de los modulos
    /// </summary>
    public class COConstantesModulos
    {
        #region Constantes globales para la aplicacion

        /// <summary>
        /// Identifica a un usuario de controler
        /// </summary>
        public const string USUARIO_APLICACION = "APLI";

        /// <summary>
        /// Identifica a un usuario del directorio activo
        /// </summary>
        public const string USUARIO_LDAP = "LDAP";

        #endregion Constantes globales para la aplicacion

        private static readonly COConstantesModulos instancia = new COConstantesModulos();

        /// <summary>
        /// Retorna instancia de la clase de constantes de los móudulos de controller
        /// </summary>
        public static COConstantesModulos Instancia
        {
            get { return COConstantesModulos.instancia; }
        }

        #region MacroProceso Admision

        public const string MENSAJERIA = "MEN";
        public const string GIROS = "GIA";
        public const string TRAMITES = "TRA";
        public const string INTERVIAJES = "INT";
        public const string CAJA = "CAJ";
        public const string RECOGIDAS = "REC";

        #endregion MacroProceso Admision

        #region Macroproceso Configuracion

        public const string PARAMETROS_GENERALES = "PAR";
        public const string SEGURIDAD = "SEG";
        public const string PERSONAL = "PER";

        /// <summary>
        ///  Módulo de áreas: ARE
        /// </summary>
        public const string MODULO_AREAS = "ARE";

        public const string REPORTES = "REP";

        #endregion Macroproceso Configuracion

        #region Macroproceso Comercial

        /// <summary>
        /// Modulo de agencias y puntos de atención (PUA)
        /// </summary>
        public const string CENTRO_SERVICIOS = "PUA";

        /// <summary>
        /// Modulo de comisiones (COM)
        /// </summary>
        public const string COMISIONES = "COM";

        /// <summary>
        /// Módulo de clientes (CLI)
        /// </summary>
        public const string CLIENTES = "CLI";

        /// <summary>
        /// Constante para el módulo de tarifas (TAR)
        /// </summary>
        public const string TARIFAS = "TAR";

        /// <summary>
        /// Constante para el modulo de Logistica Inversa
        /// </summary>
        public const string LOGISTICA_INVERSA = "LOI";

        /// <summary>
        /// Constante para el modulo de Logistica Inversa
        /// </summary>
        public const string CENTROACOPIO = "CAC";

        #endregion Macroproceso Comercial

        #region Macroproceso Operacion

        public const string MODULO_CENTRO_ACOPIO = "CAC";
        public const string MODULO_OPERACION_URBANA = "OPU";
        public const string MODULO_OPERACION_NACIONAL = "OPN";
        public const string MODULO_RUTAS = "RUT";
        public const string MODULO_GESTION_NOIFICACIONES = "GNO";
        public const string MODULO_GESION_INTERVIAJES = "GIN";
        public const string MODULO_TRAMITES = "GTR";
        public const string MODULO_GESTION_GIROS = "GGI";
        public const string MODULO_RECOGIDAS = "REC";

        public const String MODULO_RAPS = "RAP";
        public const String MODULO_MASIVOS = "MAS";

        #endregion Macroproceso Operacion

        #region Macroproceso logistifca inversa

        public const string PRUEBAS_DE_ENTREGA = "PUE";
        public const string TELEMERCADEO = "TEL";
        public const string DIGITALIZACION_Y_ARCHIVO = "DIA";
        public const string NOTIFICACIONES = "GNO";

        #endregion Macroproceso logistifca inversa

        #region Macroproceso Administracion

        public const string MODULO_GESTION_CAJAS = "GCA";
        public const string MODULO_CONTROL_CUENTAS = "CCU";
        public const string MODULO_SUMINISTROS = "SUM";
        public const string MODULO_PRODUCCION = "PRO";
        public const string MODULO_FACTURACION = "FAC";

        #endregion Macroproceso Administracion

        #region Macroproceso Sac

        public const string BUZON = "MEI";
        public const string CONSULTA_OPERACIONES = "COO";

        #endregion Macroproceso Sac

        #region Macroproceso Agenda

        public const string AGENDA = "AGE";

        #endregion Macroproceso Agenda

        #region Macroproceso Indicadores

        //Agregar constantes de Indicadores

        #endregion Macroproceso Indicadores

        #region Macroproceso Configuración

        /// <summary>
        /// Constante para el módulo de configuración de parámetros operativos
        /// </summary>
        public const string PARAMETROS_OPERATIVOS = "CPO";

        #endregion Macroproceso Configuración

        #region Macroproceso Servicio al Cliente

        /// <summary>
        /// Constante para el módulo de configuración de parámetros operativos
        /// </summary>
        public const string SERVICIO_AL_CLIENTE = "SAC";

        #endregion Macroproceso Servicio al Cliente

        #region Listas donde se agrupan los modulos dentro de sus respectivos macroprocesos

        /// <summary>
        /// Lista con los modulos del macroproceso Admision
        /// </summary>
        public List<string> MacroprocesoAdmision { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Consfiguracion
        /// </summary>
        public List<string> MacroprocesoConfiguracion { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Comercial
        /// </summary>
        public List<string> MacroprocesoComercial { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Operacion
        /// </summary>
        public List<string> MacroprocesoOperacion { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Logistica Inversa
        /// </summary>
        public List<string> MacroprocesoLogInversa { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Administracion
        /// </summary>
        public List<string> MacroprocesoAdministracion { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Servicio al Cliente
        /// </summary>
        public List<string> MacroprocesoSac { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Agenda
        /// </summary>
        public List<string> MacroprocesoAgenda { get; private set; }

        /// <summary>
        /// Lista con los modulos del macroproceso Indicadores
        /// </summary>
        public List<string> MacroprocesoIndicadores { get; private set; }

        #endregion Listas donde se agrupan los modulos dentro de sus respectivos macroprocesos

        #region Macroproceso mensajero

        public const string MENSAJERO = "MEN";

        #endregion

        //, PROVEEDORES
        private COConstantesModulos()
        {
            MacroprocesoAdmision = new List<string>(new string[] { MENSAJERIA, GIROS, TRAMITES, INTERVIAJES, CAJA });
            MacroprocesoConfiguracion = new List<string>(new string[] { PARAMETROS_GENERALES, SEGURIDAD, PERSONAL });
            MacroprocesoComercial = new List<string>(new string[] { CENTRO_SERVICIOS, CLIENTES, TARIFAS });
            MacroprocesoOperacion = new List<string>(new string[] { MODULO_OPERACION_URBANA, MODULO_OPERACION_NACIONAL, MODULO_RUTAS, MODULO_GESTION_NOIFICACIONES, MODULO_GESION_INTERVIAJES, MODULO_TRAMITES, MODULO_GESTION_GIROS, MODULO_CENTRO_ACOPIO });
            MacroprocesoLogInversa = new List<string>(new string[] { PRUEBAS_DE_ENTREGA, TELEMERCADEO, DIGITALIZACION_Y_ARCHIVO });
            MacroprocesoAdministracion = new List<string>(new string[] { MODULO_GESTION_CAJAS, MODULO_CONTROL_CUENTAS, MODULO_SUMINISTROS, MODULO_PRODUCCION });
            MacroprocesoSac = new List<string>(new string[] { BUZON, CONSULTA_OPERACIONES });
            MacroprocesoAgenda = new List<string>(new string[] { AGENDA });
            MacroprocesoIndicadores = new List<string>(new string[] { });//agregar constantes de los modulos de indicadores
        }
        #region Seguridad
        public const string SEG_SEGURIDAD = "SEGURIDAD";
        #endregion
        #region OtrosServicios
        public const string OTROSSERVICIOS = "OTROSSERVICIOS";
        #endregion
        #region 
        public const string INT_YANBAL = "YAN";

        #endregion
    }
}