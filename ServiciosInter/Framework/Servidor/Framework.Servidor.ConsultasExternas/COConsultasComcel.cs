using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.ConsultasExternas.Datos;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.ConsultasExternas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Servidor.ConsultasExternas
{
    public class COConsultasComcel: ControllerBase
    {
        private static COConsultasComcel instancia = (COConsultasComcel)FabricaInterceptores.GetProxy(new COConsultasComcel(), ConstantesFramework.MODULO_FRAMEWORK);

        public static COConsultasComcel Instancia
        {
            get { return instancia; }
        }

        private COConsultasComcel()
        {

        }


        public List<COConsultaComcelResponse> ConsultarGuiaCliente(COConsultaComcelRequest numeroCuenta)
        {

            try
            {

                string usuario = PAAdministrador.Instancia.ConsultarParametrosFramework("ConUserComcel");
                string password = PAAdministrador.Instancia.ConsultarParametrosFramework("ConPassComcel");

                if (usuario == numeroCuenta.NomUsuario && password == numeroCuenta.Password)
                {

                    //TODO: auditar la consulta, autenticar el usuario, manejar excepciones
                    List<COConsultaComcelResponse> respuesta = COConsultasComcelRepositorio.Instancia.ConsultarGuiaCliente(numeroCuenta);

                    string peticionSerializada = Serializacion.Serialize<COConsultaComcelRequest>(numeroCuenta);

                    string respuestaSerializada = Serializacion.Serialize<List<COConsultaComcelResponse>>(respuesta);




                    COConsultasComcelRepositorio.Instancia.AuditarIntegracion("ConsultaComcel", peticionSerializada, respuestaSerializada);

                    return respuesta;
                }
                else
                {
                    List<COConsultaComcelResponse> lst = new List<COConsultaComcelResponse>();
                    lst.Add(new COConsultaComcelResponse() { Mensaje = "Usuario o contraseña no validos." });

                    string peticionSerializada = Serializacion.Serialize<COConsultaComcelRequest>(numeroCuenta);
                    COConsultasComcelRepositorio.Instancia.AuditarIntegracion("ConsultaComcel", peticionSerializada, "Usuario o contraseña no validos.");

                    return lst;
                }
            }
            catch (Exception ex)
            {
                List<COConsultaComcelResponse> lst = new List<COConsultaComcelResponse>();
                lst.Add(new COConsultaComcelResponse (){ Mensaje = "Error en la consulta de la información."});
                string mensaje = ExtraerInformacionExcepcion(ex);
                string peticionSerializada = Serializacion.Serialize<COConsultaComcelRequest>(numeroCuenta);
                COConsultasComcelRepositorio.Instancia.AuditarIntegracion("ConsultaComcel", peticionSerializada, mensaje);
                return lst;
            }
        }

        /// <summary>
        /// Extrae la traza completa del error
        /// </summary>
        /// <param name="excepcion">Excepción</param>
        /// <returns>Traza del error</returns>
        public string ExtraerInformacionExcepcion(Exception excepcion)
        {
            //traza completa del error
            StringBuilder detalleError = new StringBuilder();
            Exception excep = excepcion;
            detalleError.AppendLine(excep.Message);
            detalleError.AppendLine("----------------------------------");
            detalleError.AppendLine("Trace Exception :" + excep.StackTrace);
            detalleError.AppendLine("----------------------------------");
            int i = 0;
            while (excep.InnerException != null)
            {
                i += 1;
                excep = excep.InnerException;
                detalleError.AppendLine("----------------------------------");
                detalleError.AppendLine("Mensaje InnerException " + i + ":");
                detalleError.AppendLine(excep.Message);
                detalleError.AppendLine("----------------------------------");
                detalleError.AppendLine("Trace InnerException " + i + " :");
                detalleError.AppendLine(excep.StackTrace);
            }
            return detalleError.ToString();
        }
    }
}
