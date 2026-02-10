using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Raps.Comun.Integraciones.Datos;
using CO.Servidor.Raps.Datos;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun.Util;
using RestSharp;
using CO.Servidor.Servicios.ContratoDatos.Raps;
using Framework.Servidor.Comun;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.MotorReglas;

namespace CO.Servidor.Raps.Comun.Integraciones
{
    public class RAIntegracionesRaps
    {
        #region singleton
        private static readonly RAIntegracionesRaps instancia = new RAIntegracionesRaps();

        public static RAIntegracionesRaps Instancia
        {
            get
            {
                return instancia;
            }
        }
        #endregion
        #region Fachadas
        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        #endregion        

        private RAIntegracionesRaps()
        {

        }

        /// <summary>
        /// Crea una solicitud acumulativa
        /// </summary>
        public void CrearSolicitudAcumulativaRaps(CoEnumTipoNovedadRaps tipoNovedad, Dictionary<string, object> parametrosParametrizacion, string idLocalidad, string usuario, int idSistema = 1)
        {

            Task.Factory.StartNew(() =>
            {
                try
                {
                    EnviarSolicitudAcumulativaRaps(tipoNovedad, parametrosParametrizacion, idLocalidad, usuario, idSistema);
                }
                catch (Exception ex)
                {

                    string archivo = @"c:\logExcepciones\logExcepciones.txt";
                    FileInfo f = new FileInfo(archivo);
                    StreamWriter writer;
                    if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
                    {
                        writer = f.CreateText();
                        writer.Close();
                    }

                    writer = f.AppendText();
                    writer.WriteLine("********ERROR INTEGRANDO ***** " + ex.Message);
                    writer.Close();

                    UtilidadesFW.AuditarExcepcion(ex, true);
                }
            });
        }



        /// <summary>
        /// Crea una solicitud acumulativa
        /// </summary>
        private void EnviarSolicitudAcumulativaRaps(CoEnumTipoNovedadRaps tipoNovedad, Dictionary<string, object> parametrosParametrizacion, string idLocalidad, string usuario, int idSistema)
        {
            string urlApi = RAIntegracionRaps.Instancia.ObtenerUrlApi();

            //idSistema = 1 => controller
            var objSignal = new { parametrosParametrizacion = parametrosParametrizacion, idCiudad = idLocalidad, idSistema, idTipoNovedad = tipoNovedad };
            var restClient = new RestClient(urlApi);
            var restRequest = new RestRequest("api/SolicitudRaps/CrearSolicitudAcumulativa", Method.POST);
            restRequest.AddJsonBody(objSignal);
            restRequest.AddHeader("usuario", usuario);
            restClient.Execute(restRequest);

        }

    
        /// <summary>
        /// Obtiene los parametros por tipo de integracion
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosPorIntegracion(int tipoMotivo, RAEnumOrigenRaps origenRaps)
        {
            return RARepositorioIntegraciones.Instancia.ObtenerParametrosPorIntegracion(tipoMotivo, origenRaps.GetHashCode());
        }

     }
}
