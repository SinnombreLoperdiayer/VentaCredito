using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data.SqlClient;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using System.Data;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes;
using System.Linq;
using System.ServiceModel;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Comun;

namespace ServiciosInterTest
{
    public class Repository
    {
        ConnectionStringSettingsCollection confi = ConfigurationManager.ConnectionStrings;
        ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["ControllerTransaccional"];


        private string CadCnxController = "";// GetConnectionStringByProvider("ControllerTransaccional");// ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        public Repository()
        {
            
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia consultada desde el portal
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public ADTrazaGuiaClienteRespuesta ObtenerTrazaUltimoEstadoPorNumGuiaPorPortal(string numeroGuia)
        {
            ADTrazaGuiaClienteRespuesta traza = new ADTrazaGuiaClienteRespuesta();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUltimoEstadoGuiaxNumeroRastreoGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    traza.IdEstadoGuia = (Convert.ToInt16(reader["EGT_IdEstadoGuia"]));
                    traza.DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString();
                    traza.FechaGrabacion = (Convert.ToDateTime(reader["EGT_FechaGrabacion"]));
                }
            }
            return traza;
        }
        /// <summary>
        /// Obtiene toda la informacion de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ADGuiaClienteRespuesta ObtenerGuiaPorNumeroGuiaPorPortal(string numeroGuia, bool EncriptaAes = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("paObtenerAdmisionMensajeriaRastreoGuias_MEN", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@numGuia", numeroGuia);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstAdm = dt.AsEnumerable().ToList();
                var adm = lstAdm.GroupBy(r => r.Field<long>("ADM_IdAdminisionMensajeria")).Select(s => s.First()).ToList().FirstOrDefault();

                if (adm == null)
                {
                    throw new FaultException("El número de guía no existe");
                }
                List<CLClienteContadoClienteRespuesta> remitenteDestinatario = ObtenerDatosRemitenteDestinatarioProtegidos(numeroGuia, EncriptaAes);
                ADGuiaClienteRespuesta guia = new ADGuiaClienteRespuesta()
                {
                    Remitente = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = null,
                        Telefono = remitenteDestinatario[0].Telefono,
                        Identificacion = remitenteDestinatario[0].Identificacion,
                        Direccion = null,
                        TipoId = null
                    },
                    Destinatario = new CLClienteContadoClienteRespuesta()
                    {
                        Nombre = null,
                        Telefono = remitenteDestinatario[1].Telefono,
                        Identificacion = remitenteDestinatario[1].Identificacion,
                        Direccion = null,
                        TipoId = null
                    },
                    NumeroGuia = Convert.ToInt64(adm["ADM_NumeroGuia"]),
                    NombreCiudadOrigen = Convert.ToString(adm["ADM_NombreCiudadOrigen"]),
                    NombreCiudadDestino = Convert.ToString(adm["ADM_NombreCiudadDestino"]),
                    FechaEstimadaEntregaNew = adm["ADM_FechaEstimadaEntregaNew"] == DBNull.Value ? Convert.ToDateTime(adm["ADM_FechaEntrega"]) : Convert.ToDateTime(adm["ADM_FechaEstimadaEntregaNew"]),
                    FechaAdmision = Convert.ToDateTime(adm["ADM_FechaAdmision"]),
                    NombreTipoEnvio = Convert.ToString(adm["ADM_NombreTipoEnvio"]),
                    TotalPiezas = Convert.ToInt16(adm["ADM_TotalPiezas"]),
                    Peso = Convert.ToDecimal(adm["ADM_Peso"]),
                    PesoLiqVolumetrico = Convert.ToDecimal(adm["ADM_PesoLiqVolumetrico"]),
                    NumeroBolsaSeguridad = Convert.ToString(adm["ADM_NumeroBolsaSeguridad"]),
                    DiceContener = Convert.ToString(adm["ADM_DiceContener"]),
                    Observaciones = Convert.ToString(adm["ADM_Observaciones"]),
                    IdServicio = Convert.ToInt32(adm["ADM_IdServicio"]),
                    NombreServicio = Convert.ToString(adm["ADM_NombreServicio"]),
                    NumeroPieza = Convert.ToInt16(adm["ADM_NumeroPieza"])
                };

                long IdAdmision = Convert.ToInt64(adm["ADM_IdAdminisionMensajeria"]);

                guia.FormasPago = lstAdm.Where(a => a.Field<long>("ADM_IdAdminisionMensajeria") == IdAdmision).ToList().ConvertAll<ADGuiaFormaPagoClienteRespuesta>(f =>
                new ADGuiaFormaPagoClienteRespuesta
                {
                    IdFormaPago = Convert.ToInt16(f["FOP_IdFormaPago"]),
                    Descripcion = Convert.ToString(f["FOP_Descripcion"]),
                });

                if (guia.FormasPago == null)
                {
                    guia.FormasPago = new List<ADGuiaFormaPagoClienteRespuesta>
                    {
                        new ADGuiaFormaPagoClienteRespuesta { }
                    };
                }

                return guia;
            }
        }
        

        /// <summary>
        /// Obtiene la informacion protegida de una guia a partir del numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<CLClienteContadoClienteRespuesta> ObtenerDatosRemitenteDestinatarioProtegidos(string numeroGuia, bool EncriptaAes = false)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand comm = new SqlCommand("usp_ConsultarDatosProtegidosAdmisionMensajeria", conn);
                comm.CommandType = System.Data.CommandType.StoredProcedure;
                comm.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataAdapter da = new SqlDataAdapter(comm);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstAdm = dt.AsEnumerable().ToList();
                var adm = lstAdm.GroupBy(r => r.Field<string>("ADM_IdRemitente")).Select(s => s.First()).ToList().FirstOrDefault();

                if (adm == null)
                {
                    throw new FaultException("El número de guía no existe");
                }
                if (!EncriptaAes)
                {
                    List<CLClienteContadoClienteRespuesta> lstGuia = new List<CLClienteContadoClienteRespuesta>();
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.Encrypt(adm["ADM_TelefonoRemitente"].ToString()),
                        Identificacion = Cifrado.Encrypt(adm["ADM_IdRemitente"].ToString())
                    });
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.Encrypt(adm["ADM_TelefonoDestinatario"].ToString()),
                        Identificacion = Cifrado.Encrypt(adm["ADM_IdDestinatario"].ToString()),
                    });
                    return lstGuia;
                }
                else
                {
                    List<CLClienteContadoClienteRespuesta> lstGuia = new List<CLClienteContadoClienteRespuesta>();
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.EncriptarTexto(adm["ADM_TelefonoRemitente"].ToString()),
                        Identificacion = Cifrado.EncriptarTexto(adm["ADM_IdRemitente"].ToString())
                    });
                    lstGuia.Add(new CLClienteContadoClienteRespuesta()
                    {
                        Telefono = Cifrado.EncriptarTexto(adm["ADM_TelefonoDestinatario"].ToString()),
                        Identificacion = Cifrado.EncriptarTexto(adm["ADM_IdDestinatario"].ToString()),
                    });
                    return lstGuia;
                }


            }
        }
        /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada consultada por el portal.
        /// </summary>
        /// <returns></returns>
        public List<ADEstadoGuiaMotivoClienteRespuesta> ObtenerEstadosMotivosGuiaPorPortal(string numeroGuia)
        {
            List<ADEstadoGuiaMotivoClienteRespuesta> listaestadosmotivos = new List<ADEstadoGuiaMotivoClienteRespuesta>();
            CadCnxController = "Server=MUNDO\\DESCONTSQL;initial catalog=Icontroller;Integrated Security=True";//GetConnectionStringByProvider();
            using (SqlConnection cnn = new SqlConnection(CadCnxController))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosMotivosGuiasRastreoGuias_MEN", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ADEstadoGuiaMotivoClienteRespuesta estado = new ADEstadoGuiaMotivoClienteRespuesta
                    {
                        EstadoGuia = new ADTrazaGuiaEstadoGuia
                        {
                            IdEstadoGuia = Convert.ToInt16(reader["EGT_IdEstadoGuia"]),
                            DescripcionEstadoGuia = reader["EGT_DescripcionEstado"].ToString(),
                            Ciudad = reader["EGT_NombreLocalidad"].ToString(),
                            FechaGrabacion = Convert.ToDateTime(reader["EGT_FechaGrabacion"]),
                        }
                    };

                    listaestadosmotivos.Add(estado);
                }
            }
            return listaestadosmotivos;
        }
    }
}
