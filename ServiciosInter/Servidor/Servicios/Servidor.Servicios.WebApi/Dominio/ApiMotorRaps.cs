using CO.Servidor.Servicios.ContratoDatos.Integraciones;
using CO.Servidor.Servicios.WebApi.Comun;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiMotorRaps : ApiDominioBase
    {
        private static readonly ApiMotorRaps instancia = (ApiMotorRaps)FabricaInterceptorApi.GetProxy(new ApiMotorRaps(), COConstantesModulos.MODULO_RAPS);

        public static ApiMotorRaps Instancia
        {
            get { return ApiMotorRaps.instancia; }
        }

        private ApiMotorRaps()
        {
        }


       
        /// <summary>
        /// Realiza el proceso de escalonamiento de las solicitudes vencidas, solo se utiliza en el motorRaps
        /// </summary>
        public void EscalarSolicitudesVencidas()
        {
            try
            { 
             FabricaServicios.ServicioMotorRaps.EscalarSolicitudesVencidas();
            }
            catch (Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }
        }


        /// <summary>
        /// Realiza el proceso de creacion de las solicitudes automaticas, solo se utiliza en el motorRaps
        /// </summary>
        public void CrearSolicitudesAutomaticasSinSistemaFuente()
        {
            try
            {
                FabricaServicios.ServicioMotorRaps.CrearSolicitudesAutomaticasSinSistemaFuente();
            }
             
            catch(Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }
}

        /// <summary>
        /// Realiza el proceso de creacion de las solicitudes automaticas, con sistema fuente, solo se utiliza en el motorRaps
        /// </summary>
        public void CrearSolicitudesAutomaticasConSistemaFuente()
        {
            try
            { 
            FabricaServicios.ServicioMotorRaps.CrearSolicitudesAutomaticasConSistemaFuente();
            }
            catch (Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }
        }

        #region MotorCitas
        /// <summary>
        /// Envia los recordatorios de las citas a los integrantes
        /// </summary>
        public void EnviarRecordatoriosCitasMotor()
        {
            try
            {
                FabricaServicios.ServicioMotorRaps.EnviarRecordatoriosCitasMotor();
            }
            catch (Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }
            
        }

        public void ConsultarEntregasClaro_SISP()
        {
            try
            {
                FabricaServicios.ServicioMotorSispostal.ConsultarEntregasClaro();
            }
            catch (Exception ex)
            {
                AuditarError(ex);
                throw new FaultException<Exception>(ex);
            }
            
        }

        public List<INEstadosSWSispostal> ObtenerEstadosGuiaSispostal(long NGuia)
        {
            return FabricaServicios.ServicioMotorSispostal.ObtenerEstadosGuiaSispostal(NGuia);
        }


        #endregion

        /// <summary>
        /// Audita errores
        /// </summary>
        /// <param name="ex"></param>
        private void AuditarError(Exception ex)
        {
            try
            {
                string archivo = @"c:\logServiciosAutomaticos\logMotorRAPS.txt";
                FileInfo f = new FileInfo(archivo);
                StreamWriter writer;
                if (!f.Exists)
                {
                    writer = f.CreateText();
                    writer.Close();
                }
                writer = f.AppendText();

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(ex.Message + "-" + ex.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                if (ex.InnerException != null)
                {
                    sb.AppendLine("    " + ex.InnerException.Message + "-" + ex.InnerException.StackTrace);
                    if (ex.InnerException.InnerException != null)
                    {
                        sb.AppendLine("          " + ex.InnerException.InnerException.Message + "-" + ex.InnerException.InnerException.StackTrace);
                    }
                }
                writer.WriteLine(ex.Message + "-" + ex.StackTrace + "_" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                writer.Close();
            }
            catch
            { }
        }


    }
}