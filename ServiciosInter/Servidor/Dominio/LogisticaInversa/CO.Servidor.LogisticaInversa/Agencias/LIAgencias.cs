using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.LogisticaInversa.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using System.Data.SqlClient;
using CO.Servidor.LogisticaInversa.Datos.Modelo;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Threading.Tasks;

namespace CO.Servidor.LogisticaInversa.Agencias 
{
    public class LIAgencia : ControllerBase
    {
        private static readonly LIAgencia instancia = (LIAgencia)FabricaInterceptores.GetProxy(new LIAgencia(), COConstantesModulos.DIGITALIZACION_Y_ARCHIVO);

        private IADFachadaAdmisionesMensajeria fachadaMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();

        private IOUFachadaOperacionUrbana fachadaOperacionUrbana = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>();



        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];

        public static LIAgencia Instancia
        {
            get { return LIAgencia.instancia; }
        }



        #region Metodos

        /// <summary>
        /// Método para obtener los pendientes de la agencia
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <returns></returns>
        public List<LIDescargueGuiaAgenciaDC> ObtenerPendientesAgencia(long idAgencia)
        {
            return LIRepositorioPruebasEntrega.Instancia.ObtenerPendientesAgencia(idAgencia);
        }



        #endregion
    }
}
