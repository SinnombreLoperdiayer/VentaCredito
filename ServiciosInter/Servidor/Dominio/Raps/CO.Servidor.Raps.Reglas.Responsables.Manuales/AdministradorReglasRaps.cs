using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Excepciones;
using Framework.Servidor.MotorReglas;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.ServiceModel;
//using System.ServiceModel;
using System.Text;
using System.Web;

namespace CO.Servidor.RAPS.Reglas.ResponsablesManuales
{
    public class AdministradorReglasRaps
    {
        private static int tiempoCaducaInstancia = 0;

        /// <summary>
        /// Ejecutar uan regla de negocio.
        /// </summary>
        /// <param name="nombreAssembly">Nombre del assembly que contiene la regla de negocio.</param>
        /// <param name="namespace">Namespace de la regla de negocio.</param>
        /// <param name="nombreClase">Nombre de la clase dentro del namespace que contiene la lógica de la regla.</param>
        /// <param name="parametrosRegla">Parametros de entrada y salida de la regla.</param>
        /// <returns>Respuesta del estado de la ejecución de la regla y referencia a los parámetros de salida de la regla</returns>
        public static RAResponsableDC EjecutarRegla(string nombreAssembly, string @namespace, string nombreClase, IDictionary<string, object> parametrosRegla)
        {
            RAResponsableDC datosResponsable = new RAResponsableDC();

            if (String.IsNullOrEmpty(nombreAssembly) || String.IsNullOrEmpty(@namespace) || String.IsNullOrEmpty(nombreClase))
            {
                ControllerException excepcion = new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.
                                                                        EX_ERROR_PARAMETROS_NULOS_EJECUCION_REGLA.ToString(),
                                                                        MensajesFramework.CargarMensaje(ETipoErrorFramework.
                                                                        EX_ERROR_PARAMETROS_NULOS_EJECUCION_REGLA));

                throw new FaultException<ControllerException>(excepcion);
            }

            if (parametrosRegla == null)
            {
                parametrosRegla = new Dictionary<string, object>();
                parametrosRegla.Add(ClavesReglasFramework.HUBO_ERROR, false);
                parametrosRegla.Add(ClavesReglasFramework.EXCEPCION, null);
            }

            RespuestaEjecutorReglas resultado = new RespuestaEjecutorReglas(parametrosRegla);

            IReglaIntegraciones regla;
            string tipoRegla = @namespace + "." + nombreClase;

            if (Cache.Instancia.ContainsKey(tipoRegla))
            {
                // Cargar la instancia desde el cache del servidor
                regla = Cache.Instancia[tipoRegla] as IReglaIntegraciones;
            }
            else
            {
                tiempoCaducaInstancia = ObtenerTiempoCaducaCache();
                regla = CrearInstancia(nombreAssembly, tipoRegla);
                Cache.Instancia.Add(tipoRegla, regla, tiempoCaducaInstancia);
            }

            // ejecutar la regla
            datosResponsable = regla.ObtenerResponsableNovedadRaps(parametrosRegla);

            //retornar resultado de la ejecución de la regla
            return datosResponsable;
        }

        private static IReglaIntegraciones CrearInstancia(string nombreAssembly, string tipoRegla)
        {
            ObjectHandle instancia;
            IReglaIntegraciones regla;
            instancia = Activator.CreateInstance(nombreAssembly, tipoRegla);
            regla = instancia.Unwrap() as IReglaIntegraciones;

            if (regla == null)
            {
                ControllerException excepcion = new ControllerException(ConstantesFramework.PARAMETROS_FRAMEWORK, ETipoErrorFramework.
                                                                      EX_ERROR_REGLA_NO_IMPLEMENTADA.ToString(),
                                                                      MensajesFramework.CargarMensaje(ETipoErrorFramework.
                                                                      EX_ERROR_REGLA_NO_IMPLEMENTADA));
                throw new FaultException<ControllerException>(excepcion);
            }

            return regla;
        }

        private static int ObtenerTiempoCaducaCache()
        {
            if (tiempoCaducaInstancia == 0)
            {
                tiempoCaducaInstancia = Convert.ToInt32(PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.TIEMPO_CADUCA_REGLA_INSTANCIA_NEGOCIO));
            }

            return tiempoCaducaInstancia;
        }
    }
}
