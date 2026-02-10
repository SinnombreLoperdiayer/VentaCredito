using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.ReversionEstados;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.ReversionEstadosGuia.Datos
{
    public class ADReversionEstadosRepositorio
    {
        private static readonly ADReversionEstadosRepositorio instancia = new ADReversionEstadosRepositorio();

        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;
        private string conexionStringNovasoft = ConfigurationManager.ConnectionStrings["novasoftTransaccional"].ConnectionString;
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringHistorico = ConfigurationManager.ConnectionStrings["ControllerTransaccionalHistorico"].ConnectionString;

        #region ReglasCambioEstado
        public void ActualizacionAdmisionMensajeria(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_ActualizarAdmisionCambioEstado_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarPlanillaAsignacionGuia(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarPlanillaAsignacionGuiaCambioEstado_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarGestionGuiaTelemercadeo(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarGestionGuiaTelemercadeoCambioEstao_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarEvidenciaDevolucion(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarEvidenciaDevolucionCambioEstado_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarEstadoGuiaTraza(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarEstadoGuiaTrazaCambioEstado_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarEstadoGuiaTipoImpreso(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarEstadoGuiaTipoImpresoCambioEstado_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarEstadoGuiaMotivo(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarEstadoGuiaMotivoCambioEstado_MEN", sqlConn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        //public void EliminarCustodia(ReversionEstado reversionEstado)
        //{
        //    using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
        //    {
        //        sqlConn.Open();
        //        SqlCommand cmd = new SqlCommand("pa_EliminarCustodiaCambioEstado_MEN", sqlConn);

        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
        //        cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

        //        var resultReader = cmd.ExecuteNonQuery();
        //    }
        //}

        public void EliminarArchivoEvidencia(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarArchivoEvidencia_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void ReasignarValoresCaja(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paReasignarValoresCajaCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));                

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarAlmacenGuia(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarAlmacenGuiaCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarMovimientoInventario(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_ActualizarMovimientoInventarioCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void EliminarAlmacenArchivoGuia(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_EliminarAlmacenArchivoGuiaCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }

        public void ActualizarPlanillaAsignacionGuia(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("pa_ActualizarPlanillaAsignacionGuiaCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", reversionEstado.IdEstadoSolicitado));

                var resultReader = cmd.ExecuteNonQuery();
            }
        }


        #endregion

        public List<ParametrizacionBorradoTablasPorEstado> ObtenerParametrosReglasCambioEstado(int idEstadoGuiaSolicitado)
        {
            var listaParametros = new List<ParametrizacionBorradoTablasPorEstado>();

            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosReglasCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdEstadoSolicitado", idEstadoGuiaSolicitado));


                var resultReader = cmd.ExecuteReader();

                while (resultReader.Read())
                {
                    var parametro = new ParametrizacionBorradoTablasPorEstado();

                    parametro.IdParametroBorradoTabla = Convert.ToInt32(resultReader["IdParametroBorradoTabla"]);
                    parametro.IdEstadoGuiaDestino = Convert.ToInt32(resultReader["IdEstadoGuiaDestino"]);
                    parametro.IdFuncionRegla = Convert.ToInt32(resultReader["IdFuncionRegla"]);
                    parametro.DescripcionReglaTabla = Convert.ToString(resultReader["DescripcionReglaTabla"]);
                    parametro.IdOrdenEjecucion = Convert.ToInt32(resultReader["IdOrdenEjecucion"]);

                    listaParametros.Add(parametro);
                }

                return listaParametros;
            }
        }

        /// <summary>
        /// Retorna la instancia de la clase ADReversionEstadosRepositorio
        /// </summary>
        public static ADReversionEstadosRepositorio Instancia
        {
            get { return ADReversionEstadosRepositorio.instancia; }
        }

        public List<ADTrazaGuia> ObtenerTrazaGuia(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                List<ADTrazaGuia> listaTraza = new List<ADTrazaGuia>();
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTrazaGuiaEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));

                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    ADTrazaGuia guiaTraza = new ADTrazaGuia
                    {
                        IdEstadoGuiaLog = Convert.ToInt64(lector["EGT_IdEstadoGuiaLog"]),
                        NumeroGuia = Convert.ToInt64(lector["EGT_NumeroGuia"]),
                        IdEstadoGuia = Convert.ToInt16(lector["EGT_IdEstadoGuia"]),
                        DescripcionEstadoGuia = Convert.ToString(lector["EGT_DescripcionEstado"]),
                        Ciudad = Convert.ToString(lector["EGT_NombreLocalidad"]),
                        FechaGrabacion = Convert.ToDateTime(lector["EGT_FechaGrabacion"]),
                        Usuario = Convert.ToString(lector["EGT_CreadoPor"]),
                        NombreCentroServicioEstado = Convert.ToString(lector["EGT_NombreCentroServicio"])
                    };
                    listaTraza.Add(guiaTraza);
                }
                sqlConn.Close();
                return listaTraza;
            }
        }

        public bool VerificarCambioEstadoPermitido(long numeroGuia, int idEstadoOrigen, int idEstadoSolicitado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                bool resultado = false;
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paValidacionReversionEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                cmd.Parameters.Add(new SqlParameter("@EstadoGuiaOrigen", idEstadoOrigen));
                cmd.Parameters.Add(new SqlParameter("@EstadoGuiaDestino", idEstadoSolicitado));

                int resultR = Convert.ToInt16(cmd.ExecuteScalar());
                sqlConn.Close();
                if (resultR == 1)
                {
                    resultado = true;
                }
                else if (resultR == 0)
                {
                    resultado = false;
                }
                return resultado;
            }
        }

        /// <summary>
        /// Lista las acciones
        /// </summary>
        /// <returns></returns>
        public long GrabarAuditoriaEstadoGuia(ReversionEstado reversionEstado)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarAuditoriaCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", reversionEstado.NumeroGuia));
                cmd.Parameters.Add(new SqlParameter("@Ticket", reversionEstado.NumeroTicket));
                cmd.Parameters.Add(new SqlParameter("@FechaSolicitud", DateTime.Now));
                cmd.Parameters.Add(new SqlParameter("@IDEstadoCambio", reversionEstado.IdEstadoSolicitado));
                cmd.Parameters.Add(new SqlParameter("@IDEstadoActual", reversionEstado.IdEstadoOrigen));
                cmd.Parameters.Add(new SqlParameter("@Observaciones", reversionEstado.Observacion));
                cmd.Parameters.Add(new SqlParameter("@TieneRap", reversionEstado.GeneraRAP));
                cmd.Parameters.Add(new SqlParameter("@CreadoPor", reversionEstado.CreadoPor)); 
                cmd.Parameters.Add(new SqlParameter("@SolicitadoPor", reversionEstado.SolicitadoPor));
                var resultReader = cmd.ExecuteScalar();
                return Convert.ToInt64(resultReader);
            }              
            
        }

        public ReversionEstado ObtenerCambioEstadoAnterior(long numeroGuia)
        {
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                ReversionEstado guiaEstado = new ReversionEstado();
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultarGuiaAuditoriaCambioEstado_MEN", sqlConn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));

                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {

                    guiaEstado.NumeroGuia = Convert.ToInt64(lector["ACE_NumeroGuia"]);
                    guiaEstado.NumeroTicket = Convert.ToInt64(lector["ACE_Ticket"]);
                    guiaEstado.IdEstadoOrigen = Convert.ToInt32(lector["ACE_IDEstadoActual"]);
                    guiaEstado.IdEstadoSolicitado = Convert.ToInt32(lector["ACE_IDEstadoCambio"]);
                    guiaEstado.Observacion = Convert.ToString(lector["ACE_Observaciones"]);
                    //guiaEstado.GeneraRAP = Convert.ToBoolean(lector["ACE_TieneRap"]);
                    guiaEstado.CreadoPor = Convert.ToString(lector["ACE_CreadoPor"]);
                }
                sqlConn.Close();
                return guiaEstado;
            }
        }

        public List<DatosPersonaNovasoftDC> ObtenerEmpleadosNovasoft()
        {
            List<DatosPersonaNovasoftDC> lista = new List<DatosPersonaNovasoftDC>();
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadosActivosNovasoft_RE", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader lector = cmd.ExecuteReader();
                while (lector.Read())
                {
                    string nombre = Convert.ToString(lector["nombreCompleto"]);
                    string cargo = Convert.ToString(lector["nom_car"]);
                    DatosPersonaNovasoftDC persona = new DatosPersonaNovasoftDC()
                    {
                        NombreCompleto = nombre + " " + cargo
                    };
                    
                    lista.Add(persona);
                }
                sqlConn.Close();
                return lista;
            }
        }

        public bool InsertarHistoricoGuia(DatosGuiaHistorico datosHistoricoGuia)
        {

            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringHistorico))
            {
                SqlCommand cmd = new SqlCommand("PaInsertarTablasHistorico", sqlConn);
                cmd.CommandTimeout = 300;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAuditoria", datosHistoricoGuia.IdAuditoria);
                cmd.Parameters.AddWithValue("@NumeroGuia", datosHistoricoGuia.NumeroGuia);
                cmd.Parameters.AddWithValue("@IdAdmisionMensajeria", datosHistoricoGuia.IdAdmisioneMensajeria);
                sqlConn.Open();
                var resultReader = cmd.ExecuteNonQuery();
                return resultReader == 0 ? true : false;

            }

        }

        public DatosAfectacionCaja ObtenerComprobanteFormaPago(long numeroGuia)
        {
            var datosAfectacion = new DatosAfectacionCaja();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerComprobanteFormaPago_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        datosAfectacion.FechaGrabacion = Convert.ToDateTime(reader["RTD_FechaGrabacion"]);
                        datosAfectacion.CreadoPor = Convert.ToString(reader["RTD_CreadoPor"]);
                        datosAfectacion.DescripcionFormaPago = Convert.ToString(reader["RVF_DescripcionFormaPago"]);
                        datosAfectacion.NombreConcepto = Convert.ToString(reader["RTD_NombreConcepto"]);
                        datosAfectacion.NumeroComprobante = Convert.ToString(reader["RTD_NumeroComprobante"]);
                    }
                }
            }

            return datosAfectacion;
        }



    }
}
