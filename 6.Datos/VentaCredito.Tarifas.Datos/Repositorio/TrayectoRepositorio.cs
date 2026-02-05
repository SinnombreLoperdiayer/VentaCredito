using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Rangos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using VentaCredito.Tarifas.Comun;

namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class TrayectoRepositorio
    {
        private static TrayectoRepositorio instancia = new TrayectoRepositorio();
        public static TrayectoRepositorio  Instancia { get { return instancia; } }

        private string cadenaTransaccional = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna Validacion si el Servicio-Origen-Destino, debe etiquetarse como AEREO en el campo del casillero de la Guia
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool ValidarServicioTrayectoCasilleroAereo(string municipioOrigen, string municipioDestino, int idServicio)
        {
            bool rta = false;

            using (SqlConnection cnx = new SqlConnection(cadenaTransaccional))
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand("paValidarServicioTrayectoCasilleroAereo_TAR", cnx);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@municipioOrigen", municipioOrigen));
                cmd.Parameters.Add(new SqlParameter("@municipioDestino", municipioDestino));
                cmd.Parameters.Add(new SqlParameter("@idServicio", idServicio));

                Int32 num = (Int32)cmd.ExecuteScalar();
                if (num == 1)
                    rta = true;
            }

            return rta;
        }

        /// <summary>
        /// Obtiene los servicios de rapicarga, Rapi Carga Terrestre y mensajeria por municipio origen y destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public List<int> ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(string municipioOrigen, string municipioDestino)
        {
            List<int> lstIdServicios = new List<int>();
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = new SqlCommand("paObtenerListaServiciosValidacionMensajeriaMayorPeso", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidadOrigen", municipioOrigen);
                cmd.Parameters.AddWithValue("@IdLocalidadDestino", municipioDestino);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lstIdServicios.Add(Convert.ToInt32(reader["STR_IdServicio"] == DBNull.Value ? 0 : reader["STR_IdServicio"]));
                }
                conn.Close();
            }
            return lstIdServicios;
        }

        /// <summary>
        /// Obtener concepto de caja a partir del numero del servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public int ObtenerConceptoCaja(int idServicio)
        {
            int idConcepto = 0;
            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerObtenerConceptoCaja_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        idConcepto = Convert.ToInt32(reader["SER_IdConceptoCaja"]);
                    }
                }
            }

            return idConcepto;
        }

        internal decimal ObtenerPrecioRangoContrapago(int idListaPrecio, decimal valorContraPago, int servicio)
        {
            var listaRangos = new List<TARangoContrapago>();
            using (SqlConnection sqlConn = new SqlConnection(cadenaTransaccional))
            {
                SqlCommand cmd = new SqlCommand("pAConsultarPrecioRangoContrapago_TAR", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdListaPrecio", idListaPrecio);
                cmd.Parameters.AddWithValue("@IdServicio", servicio);
                sqlConn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                decimal valor = 0;

                while (reader.Read())
                {
                    var rango = new TARangoContrapago
                    {
                        IdListaPrecioServicio = Convert.ToInt32(reader["PRA_IdListaPrecioServicio"]),
                        IdPrecioRango = Convert.ToInt32(reader["PRA_IdPrecioRango"]),
                        ValorCobro = Convert.ToDecimal(reader["PRA_Valor"]),
                        ValorInicial = Convert.ToDecimal(reader["PRA_Inicial"]),
                        ValorFinal = Convert.ToDecimal(reader["PRA_Final"]),
                        ValorPorcentaje = Convert.ToDecimal(reader["PRA_Porcentaje"])
                    };
                    listaRangos.Add(rango);
                }

                sqlConn.Close();

                if (listaRangos.Where(r => r.ValorInicial <= valorContraPago && r.ValorFinal >= valorContraPago).Count() > 0)
                {
                    if (listaRangos.Where(r => r.ValorInicial <= valorContraPago && r.ValorFinal >= valorContraPago).FirstOrDefault().ValorCobro == 0)
                        valor = valorContraPago * (listaRangos.Where(r => r.ValorInicial <= valorContraPago && r.ValorFinal >= valorContraPago).FirstOrDefault().ValorPorcentaje / 100);
                    else if (listaRangos.Where(r => r.ValorInicial <= valorContraPago && r.ValorFinal >= valorContraPago).FirstOrDefault().ValorPorcentaje == 0)
                        valor = listaRangos.Where(r => r.ValorInicial <= valorContraPago && r.ValorFinal >= valorContraPago).FirstOrDefault().ValorCobro;
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_CONTRAPAGO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_NO_EXISTE_PRECIO_CONTRAPAGO)));

                return valor;

            }
        }

        public List<TAServicioFormaPagoContrapagoDC> ObtenerServiciosFormaPagoContrapago()
        {
            List<TAServicioFormaPagoContrapagoDC> lst = new List<TAServicioFormaPagoContrapagoDC>();

            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServiciosFormaPagoContrapago_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TAServicioFormaPagoContrapagoDC obj = new TAServicioFormaPagoContrapagoDC()
                        {
                            NombreServicio = reader["Servicio"].ToString(),
                            IdServicio = Convert.ToInt64(reader["IdServicio"]),
                            DescripcionFormapago = reader["Forma Pago"].ToString(),
                            IdFormapago = Convert.ToInt64(reader["IdFormaPago"]),
                            Iva = Convert.ToInt32(reader["Iva"]),
                            FechaGrabacion = Convert.ToDateTime(reader["Fecha Grabacion"]),
                            CreadoPor = reader["Usuario"].ToString()
                        };
                        lst.Add(obj);
                    }
                }
            }
            return lst;
        }

        /// <summary>
        /// Retorna los impuestos asignados a un servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Colección de servicios</returns>
        internal List<TAImpuestosDC> ObtenerValorImpuestosServicio(int idServicio)
        {
            List<TAImpuestosDC> lst = null;

            using (SqlConnection conn = new SqlConnection(cadenaTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerServiciosFormaPagoContrapago_MEN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdServicio", idServicio);
                SqlDataReader reader = cmd.ExecuteReader();

                lst = MapperToTAImpuestosDC(reader);
                
            }
            return lst;
           
        }

        private List<TAImpuestosDC> MapperToTAImpuestosDC(SqlDataReader reader)
        {
            List<TAImpuestosDC> resultado = null;
            if (reader.HasRows)
            {
                resultado = new List<TAImpuestosDC>();

                while (reader.Read())
                {
                    TAImpuestosDC obj = new TAImpuestosDC()
                    {
                        Identificador   = Convert.ToInt16(reader["SEI_IdImpuesto"]),
                        Descripcion     = Convert.ToString(reader["IMP_Descripcion"]),
                        Valor           = Convert.ToDecimal(reader["IMP_Valor"])
                    };
                    resultado.Add(obj);
                }
            }

            return resultado;
        }
    }
}
