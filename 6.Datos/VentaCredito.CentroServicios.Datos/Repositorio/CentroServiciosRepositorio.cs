using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
//using NFramework.Servidor.Servicios.ContratoDatos.Parametros;
using Servicio.Entidades.Admisiones.Mensajeria;
using Servicio.Entidades.CentroServicios;
using VentaCredito.CentroServicios.Comun;

namespace VentaCredito.CentroServicios.Datos.Repositorio
{
    public class CentroServiciosRepositorio
    {

        private static CentroServiciosRepositorio instancia = new CentroServiciosRepositorio();
        private string conexionString = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;


        public static CentroServiciosRepositorio Instancia
        {
            get
            {
                return instancia;
            }
        }
        /// <summary>
        /// Obtiene el centro de servicio.
        /// para el valor de la BaseInicial
        /// </summary>
        /// <param name="idCentroServicio">El idcentroservicio.</param>
        /// <returns>El centro de Servicio con la BaseInicial</returns>
        public PUCentroServiciosDC ObtenerCentroServicio(long idCentroServicio)
        {

            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCentrosServicios_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CES_IdCentroServicios", idCentroServicio);
                cmd.Parameters.AddWithValue("@totalRegistros", 1);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var centroServicio = dt.AsEnumerable().FirstOrDefault();

                if (centroServicio != null)
                {
                    PUCentroServiciosDC centro = new PUCentroServiciosDC()
                    {
                        EstadoBool = centroServicio.Field<string>("CES_Estado") == ConstantesFramework.ESTADO_ACTIVO ? true : false,
                        IdCentroServicio = centroServicio.Field<long>("CES_IdCentroServicios"),
                        Tipo = centroServicio.Field<string>("CES_Tipo"),
                        Nombre = centroServicio.Field<string>("CES_Nombre"),
                        BaseInicialCaja = centroServicio.Field<decimal>("CES_BaseInicialCaja"),
                        Direccion = centroServicio.Field<string>("CES_Direccion"),
                        IdMunicipio = centroServicio.Field<string>("CES_IdMunicipio"),
                        RecibeGiros = centroServicio.Field<bool>("CES_PuedeRecibirGiros"),
                        VendePrepago = centroServicio.Field<bool>("CES_VendePrepago"),
                        IdTipoPropiedad = Convert.ToInt32(centroServicio["CES_IdTipoPropiedad"]),
                        IdResRecogidas = Convert.ToInt32(centroServicio["CES_IdTipoResponsableRecogida"]),
                        IdCentroCostos = centroServicio.Field<string>("CES_IdCentroCostos"),
                        CodigoBodega = centroServicio.Field<string>("CES_CodigoBodega"),
                        Telefono1 = centroServicio.Field<string>("CES_Telefono1"),
                        Telefono2 = centroServicio["CES_Telefono2"] != DBNull.Value ? centroServicio.Field<string>("CES_Telefono2") : "",
                        Biometrico = centroServicio.Field<bool>("CES_Biometrico"),
                        Estado = centroServicio.Field<string>("CES_Estado"),
                        //IdSubtipoCV = centroServicio.Field<int>("IdSubtipoCV"),
                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = centroServicio.Field<string>("LOC_IdLocalidad"),
                            Nombre = centroServicio.Field<string>("LOC_Nombre"),
                            //CodigoPostal = centroServicio.Field<long>("LOC_CodigoPostal")
                        }
                    };


                    if (centroServicio["LOC_IdAncestroTercerGrado"] != DBNull.Value)
                    {
                        centro.IdPais = centroServicio.Field<string>("LOC_IdAncestroTercerGrado");
                        centro.NombrePais = centroServicio.Field<string>("LOC_NombreTercero");
                    }
                    else if (centroServicio["LOC_IdAncestroSegundoGrado"] != DBNull.Value)
                    {
                        centro.IdPais = centroServicio.Field<string>("LOC_IdAncestroSegundoGrado");
                        centro.NombrePais = centroServicio.Field<string>("LOC_NombreSegundo");
                    }
                    else if (centroServicio["LOC_IdAncestroPrimerGrado"] != DBNull.Value)
                    {
                        centro.IdPais = centroServicio.Field<string>("LOC_IdAncestroPrimerGrado");
                        centro.NombrePais = centroServicio.Field<string>("LOC_NombrePrimero");
                    }

                    return centro;
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(PUConstantesCentroServicios.MODULO_CENTRO_SERVICIOS,
                      EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_EXISTE.ToString(),
                      MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_CENTRO_SERVICIO_NO_EXISTE)));
                }
            }
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtInfoAgenciasTrayecto_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@localidadOrigen", localidadOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@localidadDestino", validacion.CodigoPostalDestino);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                conn.Close();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        validacion.PesoMaximoTrayectoOrigen = Convert.ToDecimal(reader["CES_PesoMaximoOri"]);
                        validacion.IdCentroServiciosOrigen = Convert.ToInt64(reader["CES_IdCentroServiciosOri"]);
                        validacion.NombreCentroServiciosOrigen = reader["CES_NombreOri"].ToString();
                        validacion.VolumenMaximoOrigen = Convert.ToDecimal(reader["CES_VolumenMaximoOri"]);
                        validacion.PesoMaximoTrayectoDestino = Convert.ToDecimal(reader["CES_PesoMaximoOri"]);
                        validacion.IdCentroServiciosDestino = Convert.ToInt64(reader["CES_IdCentroServiciosDes"]);
                        validacion.NombreCentroServiciosDestino = reader["CES_NombreDes"].ToString();
                        validacion.VolumenMaximoDestino = Convert.ToDecimal(reader["CES_VolumenMaximoDes"]);
                        validacion.DestinoAdmiteFormaPagoAlCobro = false;
                        validacion.DireccionCentroServiciosDestino = reader["CES_DireccionDes"].ToString();
                        validacion.TelefonoCentroServiciosDestino = reader["CES_Telefono1Des"].ToString();
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                }
            }
        }


        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Ciudad o municipio de destino del envío</param>
        /// <param name="validacion">Validación del trayecto</param>
        /// <param name="idCentroServicio">Id del centro de servicios de origen de la transacción</param>
        /// <param name="localidadOrigen">si no se tiene el id centro de servicio origen el metodo lo busca a través de la localidad original</param>
        public void ObtenerInformacionValidacionTrayectoAdo(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            using (var sqlconn = new SqlConnection(conexionString))
            {
                sqlconn.Open();
                var cmd = new SqlCommand("paObtInfoAgenciasTrayecto_PUA ", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@localidadOrigen", localidadOrigen.IdLocalidad);
                cmd.Parameters.AddWithValue("@localidadDestino", localidadDestino.IdLocalidad);
                var resul = cmd.ExecuteReader();

                if (resul.HasRows)
                {
                    if (resul.Read())
                    {
                        if (DBNull.Value.Equals(resul["CES_IdCentroServiciosOri"]))

                        {
                            throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS)));
                        }
                        else
                        {
                            validacion.PesoMaximoTrayectoOrigen = Convert.ToDecimal(resul["CES_PesoMaximoOri"]);
                            validacion.IdCentroServiciosOrigen = Convert.ToInt64(resul["CES_IdCentroServiciosOri"]);
                            validacion.NombreCentroServiciosOrigen = Convert.ToString(resul["CES_IdCentroServiciosOri"]);
                            validacion.VolumenMaximoOrigen = Convert.ToDecimal(resul["CES_IdCentroServiciosOri"]);
                        }

                        if (DBNull.Value.Equals(resul["CES_IdCentroServiciosDes"]))

                        {
                            throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_TRAYECTO_NO_VALIDO)));
                        }
                        else
                        {
                            validacion.PesoMaximoTrayectoDestino = Convert.ToDecimal(resul["CES_PesoMaximoDes"]);
                            validacion.VolumenMaximoDestino = Convert.ToDecimal(resul["CES_VolumenMaximoDes"]);
                            validacion.IdCentroServiciosDestino = Convert.ToInt64(resul["CES_IdCentroServiciosDes"]);
                            validacion.DestinoAdmiteFormaPagoAlCobro = Convert.ToBoolean(resul["CES_AdmiteFormaPagoAlCobroDes"]);
                            validacion.NombreCentroServiciosDestino = Convert.ToString(resul["CES_NombreDes"]);
                            validacion.CodigoPostalDestino = Convert.ToString(resul["LOC_CodigoPostalDes"]);
                            validacion.DireccionCentroServiciosDestino = Convert.ToString(resul["CES_DireccionDes"]);
                            validacion.TelefonoCentroServiciosDestino = Convert.ToString(resul["CES_Telefono1Des"]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtener la agencia a partir de la localidad
        /// </summary>
        ///// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            using (SqlConnection conn = new SqlConnection(conexionString))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciaLocalidad_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdLocalidad", localidad);

                SqlParameter paramOut = new SqlParameter("@NombreLocalidad", SqlDbType.VarChar, 100);
                paramOut.Direction = ParameterDirection.Output;
                paramOut.IsNullable = true;
                cmd.Parameters.Add(paramOut);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var dr = dt.AsEnumerable().FirstOrDefault();


                switch (dr.Field<string>("Tipo"))
                {
                    case "NOEXISTE":
                        ControllerException excepcion = new ControllerException(Framework.Servidor.Comun.COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), paramOut.Value.ToString()));
                        throw new FaultException<ControllerException>(excepcion);



                    case "AGE":

                        return new PUCentroServiciosDC()
                        {
                            IdCentroServicio = dr.Field<long>("AGE_IdAgencia"),
                            Nombre = dr.Field<string>("CES_Nombre"),
                            IdMunicipio = dr.Field<string>("LOC_IdLocalidad"),
                            NombreMunicipio = dr.Field<string>("LOC_Nombre"),
                            Telefono1 = dr.Field<string>("CES_Telefono1"),
                            Direccion = dr.Field<string>("CES_Direccion"),
                            Tipo = dr.Field<string>("CES_Tipo"),
                            TipoSubtipo = dr.Field<string>("AGE_IdTipoAgencia"),
                            Sistematizado = dr.Field<bool>("CES_Sistematizada")
                        };



                    case "COL":
                        return new PUCentroServiciosDC()
                        {
                            IdCentroServicio = dr.Field<long>("MCL_IdCentroLogistico"),
                            Nombre = dr.Field<string>("CES_Nombre"),
                            IdMunicipio = dr.Field<string>("MCL_IdLocalidad"),
                            NombreMunicipio = dr.Field<string>("LOC_Nombre"),
                            Telefono1 = dr.Field<string>("CES_Telefono1"),
                            Direccion = dr.Field<string>("CES_Direccion"),
                            Tipo = dr.Field<string>("CES_Tipo"),
                            Sistematizado = dr.Field<bool>("CES_Sistematizada")
                        };



                }


                throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS.ToString(), string.Format(MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_LOCALIDAD_SIN_AGENCIAS), "DESCONOCIDO")));



            }
        }

    }
}
