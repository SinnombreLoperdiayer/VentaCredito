using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Runtime.Remoting.Messaging;

namespace Framework.Servidor.Excepciones
{
    /// <summary>
    /// Descripcion: Clase manejador de excepciones
    /// Autor: Cristian Velandia
    /// Fecha: 19/09/2011
    /// Version: 1.0
    /// Modificado por:
    /// Fecha Modificación:
    /// </summary>
    public class ControlExcepcionesHandler
    {
        /// <summary>
        /// Metodo que sirve como extension de otro metodo, para el manejo de las excepciones
        /// </summary>
        /// <param name="Accion"></param>
        public static void EjecutarAccion(Action Accion, string Modulo, IMessage msg, string usuario)
        {

            try
            {
                Accion();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEVal)
            {
                StringBuilder mensajes = new StringBuilder();
                dbEVal.EntityValidationErrors.ToList().ForEach(ev =>
                {
                    ev.ValidationErrors.ToList().ForEach(m =>
              {
                        mensajes.AppendLine(m.ErrorMessage + "  ==>" + m.PropertyName);
                    });
                });
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, mensajes.ToString(), Modulo, null, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_ENTITY_VALITATION.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_ENTITY_VALITATION)));
            }

            catch (System.Data.EntityCommandExecutionException dbcomEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, dbcomEx.Message, Modulo, dbcomEx, usuario, msg);

                if (dbcomEx.InnerException != null && dbcomEx.InnerException is System.Data.SqlClient.SqlException)
                {
                    if (dbcomEx.InnerException.InnerException != null && dbcomEx.InnerException.InnerException is SqlException)
                    {
                        if ((dbcomEx.InnerException.InnerException as SqlException).Number == 50000)
                            throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, "Error customizado SP", (dbcomEx.InnerException.InnerException as SqlException).Message));
                        ValidarCodigoSqlException((dbcomEx.InnerException.InnerException as SqlException).Number);
                    }
                    else if (dbcomEx.InnerException.InnerException == null && dbcomEx.InnerException is SqlException)
                    {
                        if ((dbcomEx.InnerException as SqlException).Number == 50000)
                            throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, "Error customizado SP", (dbcomEx.InnerException as SqlException).Message));
                        ValidarCodigoSqlException((dbcomEx.InnerException as SqlException).Number);
                    }
                    else
                    {
                        if (dbcomEx.InnerException.InnerException != null)
                        {
                            if (dbcomEx.InnerException.InnerException.Message.StartsWith("EX_", StringComparison.OrdinalIgnoreCase))
                            {
                                string codigoError = dbcomEx.InnerException.InnerException.Message;
                                codigoError = codigoError.Substring(2, codigoError.Length - 3);
                                ValidarCodigoSqlException(Convert.ToInt32(codigoError));
                            }
                            else
                            {
                                if (dbcomEx.InnerException.GetType().Equals(typeof(SqlException)))
                                    ValidarCodigoSqlException((dbcomEx.InnerException as SqlException).Number);
                                else
                                    ValidarCodigoSqlException((dbcomEx.InnerException.InnerException as SqlException).Number);
                            }
                        }
                        else
                        {
                            if (dbcomEx.InnerException != null && dbcomEx.InnerException.Message.StartsWith("EX_", StringComparison.OrdinalIgnoreCase))
                            {
                                string codigoError = dbcomEx.InnerException.InnerException.Message;
                                codigoError = codigoError.Substring(2, codigoError.Length - 3);
                                ValidarCodigoSqlException(Convert.ToInt32(codigoError));
                            }
                            else
                            {
                                ValidarCodigoSqlException((dbcomEx.InnerException.InnerException as SqlException).Number);
                            }
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(dbcomEx.Source) && dbcomEx.Source.Equals("System.Data.Entity"))
                {
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                      ETipoErrorFramework.EX_ERROR_ENTITY_DETALLE.ToString(),
                      MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_ENTITY_DETALLE)));
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK,
                      ETipoErrorFramework.EX_ERROR_ENTITY_DETALLE.ToString(),
                      MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_ENTITY_DETALLE)));
                }
            }

            catch (System.Data.ConstraintException dbConstraintEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, dbConstraintEx.Message, Modulo, dbConstraintEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_EN_CONSTRAINT.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_EN_CONSTRAINT)));
            }

            catch (System.Data.Entity.Infrastructure.DbUpdateException dbUPEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, dbUPEx.Message, Modulo, dbUPEx, usuario, msg);
                if (dbUPEx.InnerException != null && dbUPEx.InnerException is UpdateException)
                    if (dbUPEx.InnerException.InnerException != null && dbUPEx.InnerException.InnerException is SqlException)
                        ValidarCodigoSqlException((dbUPEx.InnerException.InnerException as SqlException).Number);
            }

            catch (UpdateException UpEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, UpEx.Message, Modulo, UpEx, usuario, msg);
                if (UpEx.InnerException != null && UpEx.InnerException is SqlException)
                    ValidarCodigoSqlException((UpEx.InnerException as SqlException).Number);
            }

            catch (SqlException SQLEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, SQLEx.Message, Modulo, SQLEx, usuario, msg);
                ValidarCodigoSqlException(SQLEx.Number);
            }

            catch (FaultException<ControllerException> ContException)
            {
                //AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ContException.Message, Modulo, ContException);
                //throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_FORMATO_INFOR.ToString(), "mensjae de prueba"));
                throw ContException;
            }
            catch (FormatException FormatEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, FormatEx.Message, Modulo, FormatEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_FORMATO_INFOR.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_FORMATO_INFOR)));
            }
            catch (DivideByZeroException divEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, divEx.Message, Modulo, divEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_DIVISION_CERO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_DIVISION_CERO)));
            }
            catch (System.OverflowException ovEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, ovEx.Message, Modulo, ovEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_OPERACION_ARITMETICA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_OPERACION_ARITMETICA)));
            }
            catch (System.InvalidCastException icEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, icEx.Message, Modulo, icEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_CONVERSION_DATOS.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_CONVERSION_DATOS)));
            }
            catch (System.InvalidOperationException inoEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, inoEx.Message, Modulo, inoEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_OPERACION_INVALIDA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_OPERACION_INVALIDA)));
            }
            catch (System.IO.DirectoryNotFoundException dirEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, dirEx.Message, Modulo, dirEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_DIRECTORIO_NO_ENCONTRADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_DIRECTORIO_NO_ENCONTRADO)));
            }
            catch (System.IO.FileLoadException flEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, flEx.Message, Modulo, flEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_CARGANDO_ARCHIVO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_CARGANDO_ARCHIVO)));
            }
            catch (System.IO.FileNotFoundException fnEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, fnEx.Message, Modulo, fnEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_ARCHIVO_NO_ENCONTRADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_ARCHIVO_NO_ENCONTRADO)));
            }
            catch (System.OutOfMemoryException omEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, omEx.Message, Modulo, omEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_MEMORIA_EXCEDIDA.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_MEMORIA_EXCEDIDA)));
            }
            catch (System.Threading.ThreadAbortException thaEx)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, thaEx.Message, Modulo, thaEx, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_PROCESO_ABORTADO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_PROCESO_ABORTADO)));
            }
            catch (FaultException thaEx)
            {
                throw (thaEx);
            }
            catch (Exception Ex)
            {
                AuditoriaTrace.EscribirAuditoriaParametros(ETipoAuditoria.Error, Ex.Message, Modulo, Ex, usuario, msg);
                throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_DESCONOCIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_DESCONOCIDO)));
            }
        }

        private static void ValidarCodigoSqlException(int condigo)
        {
            switch (condigo)
            {
                case 64:

                    //falla en la obtención de un numerador
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_OBTENER_NUMERADOR.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_OBTENER_NUMERADOR)));
                case 547:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_VIOLACION_INEGRIDAD_REFERENCIAL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_VIOLACION_INEGRIDAD_REFERENCIAL)));
                case 2627:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_REGISTRO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_REGISTRO_YA_EXISTE)));
                case 2601:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_REGISTRO_YA_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_REGISTRO_YA_EXISTE)));
                case 207:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_CAMPO_NO_EXISTE.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CAMPO_NO_EXISTE)));
                default:
                    throw new FaultException<ControllerException>(new ControllerException(ConstantesFramework.MODULO_FRAMEWORK, ETipoErrorFramework.EX_ERROR_DESCONOCIDO.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_ERROR_DESCONOCIDO)));
            }
        }
    }
}