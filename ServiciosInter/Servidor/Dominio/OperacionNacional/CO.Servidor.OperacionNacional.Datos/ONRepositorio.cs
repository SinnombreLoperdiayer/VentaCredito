using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Transactions;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos.Modelo;
using CO.Servidor.ParametrosOperacion.Comun;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data;
using System.Configuration;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Controller.Servidor.Integraciones;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.IO;
using Framework.Servidor.Comun.Util;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;

namespace CO.Servidor.OperacionNacional.Datos
{
    public partial class ONRepositorio
    {
        private static readonly ONRepositorio instancia = new ONRepositorio();
        private const string NombreModelo = "ModeloOperacionNacional";
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;


        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static ONRepositorio Instancia
        {
            get { return ONRepositorio.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private ONRepositorio() { }

        /// <summary>
        /// Obtiene  los manifiestos de la operacion nacional
        /// </summary>
        /// <param name="filtro">Diccionario con los parametros del filtro</param>
        /// /// <param name="campoOrdenamiento">Campo por el que se realiza la ordenacion</param>
        /// <param name="indicePagina">Indice de la pagina</param>
        /// <param name="registrosPorPagina">Cantidad de registros por pagina</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de registros de la consult</param>
        /// <param name="idCentroServiciosManifiesta">Centro de servicios que manifiesta</param>
        /// <returns>Lista con los vehiculos</returns>
        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOperacionNacional(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta, bool incluyeFecha)
        {
            string IdManifiestoOperacionNacional;
            string IdRuta;
            string Placa;
            string IdEmpresaTransportadora;
            string FechaCreacionI;

            DateTime? FechaCreacionInicial = null;
            DateTime? FechaCreacionFinal = null;

            filtro.TryGetValue("MON_IdManifiestoOperacionNacio", out IdManifiestoOperacionNacional);
            filtro.TryGetValue("IdRutaFiltro", out IdRuta);
            filtro.TryGetValue("Placa", out Placa);
            filtro.TryGetValue("IdEmpresaTransportadora", out IdEmpresaTransportadora);
            filtro.TryGetValue("FechaManifiestoFiltro", out FechaCreacionI);

            long? IdManifiestoOperacionNacional_P = null;
            if (!string.IsNullOrWhiteSpace(IdManifiestoOperacionNacional))
                IdManifiestoOperacionNacional_P = long.Parse(IdManifiestoOperacionNacional);

            int? IdRuta_p = null;
            if (!string.IsNullOrWhiteSpace(IdRuta))
                IdRuta_p = int.Parse(IdRuta);

            short? IdEmpresaTransportadora_P = null;
            if (!string.IsNullOrWhiteSpace(IdEmpresaTransportadora))
                IdEmpresaTransportadora_P = short.Parse(IdEmpresaTransportadora);

            long? idCentroServiciosManifiesta_P = idCentroServiciosManifiesta;

            if (!string.IsNullOrWhiteSpace(FechaCreacionI))
            {
                //FechaCreacionInicial =  DateTime.ParseExact(FechaCreacionI, "dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("es-CO"));
                FechaCreacionInicial = Convert.ToDateTime(FechaCreacionI);
                FechaCreacionFinal = FechaCreacionInicial.Value.AddDays(1);
            }

            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (!string.IsNullOrWhiteSpace(Placa))
                {
                    var manifiestos = contexto.paObtenerManOpnNacConPlaca_OPN1(indicePagina, registrosPorPagina, IdManifiestoOperacionNacional_P, IdRuta_p, Placa, IdEmpresaTransportadora_P, FechaCreacionInicial, FechaCreacionFinal, incluyeFecha, idCentroServiciosManifiesta_P).ToList();

                    return manifiestos.ConvertAll<ONManifiestoOperacionNacional>(m =>
                    {
                        ONManifiestoOperacionNacional mon = new ONManifiestoOperacionNacional()
                        {
                            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                            EstaManifiestoCerrado = m.MON_EstaManifiestoCerrado,
                            FechaCierre = m.MON_FechaCierre,
                            FechaCreacion = m.MON_FechaGrabacion,
                            IdCentroServiciosManifiesta = m.MON_IdCentroServiciosManifiesta,
                            IdEmpresaTransportadora = m.ETR_IdEmpresaTransportadora,
                            NombreEmpresaTransportadora = m.ETR_Nombre,
                            IdManifiestoOperacionNacional = m.MON_IdManifiestoOperacionNacio,
                            IdRutaDespacho = m.MON_IdRutaDespacho,
                            NombreRuta = m.RUT_Nombre,
                            IdTipoVehiculoManifiesto = m.VEH_IdTipoVehiculo,
                            IdMedioTransporte = m.MON_IdMedioTransporte,
                            IdTipoTransporte = m.ETR_IdTipoTransporte,
                            DescripcionMedioTransporte = m.MTR_DescripcionMedioTrans,
                            NombreTipoTransporte = m.TIT_DescripcionTipoTrans,
                            RutaGeneraManifiestoMinisterio = m.RUT_GeneraManifiestoMinisterio,
                            LocalidadDespacho = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                            {
                                IdLocalidad = m.MON_IdLocalidadDespacho,
                                Nombre = m.MON_NombreLocalidadDespacho
                            },
                            LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                            {
                                IdLocalidad = m.RUT_IdLocalidadDestino,
                                Nombre = m.RUT_NombreLocalidadDestino
                            },
                            NumeroManifiestoCarga = m.MON_NumeroManifiestoCarga,
                            FechaSalidaSatrack = m.MON_FechaSalidaSatrack
                        };

                        if (m.MOT_IdVehiculo != null)
                        {
                            mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre()
                            {
                                PlacaVehiculo = m.MOT_Placa,
                                IdVehiculo = m.MOT_IdVehiculo.Value,
                                CedulaConductor = m.MOT_IdentificacionConductor,
                                NombreConductor = m.MOT_NombreConductor,
                                IdConductor = m.MOT_IdConductor.Value
                            };
                        }
                        else
                            mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre();

                        return mon;
                    });
                }
                else
                {
                    var manifiestos = contexto.paObtenerManOpnNacSinPlaca_OPN1(indicePagina, registrosPorPagina, IdManifiestoOperacionNacional_P, IdRuta_p, IdEmpresaTransportadora_P, FechaCreacionInicial, FechaCreacionFinal, incluyeFecha, idCentroServiciosManifiesta_P).ToList();

                    return manifiestos.ConvertAll<ONManifiestoOperacionNacional>(m =>
                    {
                        ONManifiestoOperacionNacional mon = new ONManifiestoOperacionNacional()
                        {
                            EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                            EstaManifiestoCerrado = m.MON_EstaManifiestoCerrado,
                            FechaCierre = m.MON_FechaCierre,
                            FechaCreacion = m.MON_FechaGrabacion,
                            IdCentroServiciosManifiesta = m.MON_IdCentroServiciosManifiesta,
                            IdEmpresaTransportadora = m.ETR_IdEmpresaTransportadora,
                            NombreEmpresaTransportadora = m.ETR_Nombre,
                            IdManifiestoOperacionNacional = m.MON_IdManifiestoOperacionNacio,
                            IdRutaDespacho = m.MON_IdRutaDespacho,
                            NombreRuta = m.RUT_Nombre,
                            IdMedioTransporte = m.MON_IdMedioTransporte,
                            IdTipoTransporte = m.ETR_IdTipoTransporte,
                            DescripcionMedioTransporte = m.MTR_DescripcionMedioTrans,
                            NombreTipoTransporte = m.TIT_DescripcionTipoTrans,
                            RutaGeneraManifiestoMinisterio = m.RUT_GeneraManifiestoMinisterio,
                            IdTipoVehiculoManifiesto = m.VEH_IdTipoVehiculo,
                            LocalidadDespacho = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                            {
                                IdLocalidad = m.MON_IdLocalidadDespacho,
                                Nombre = m.MON_NombreLocalidadDespacho
                            },
                            LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                            {
                                IdLocalidad = m.RUT_IdLocalidadDestino,
                                Nombre = m.RUT_NombreLocalidadDestino
                            },
                            NumeroManifiestoCarga = m.MON_NumeroManifiestoCarga,
                            FechaSalidaSatrack = m.MON_FechaSalidaSatrack
                        };
                        if (m.MOT_IdVehiculo != null)
                        {
                            mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre()
                            {
                                PlacaVehiculo = m.MOT_Placa,
                                IdVehiculo = m.MOT_IdVehiculo.Value,
                                CedulaConductor = m.MOT_IdentificacionConductor,
                                NombreConductor = m.MOT_NombreConductor,
                                IdConductor = m.MOT_IdConductor.Value
                            };
                        }
                        else
                            mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre();

                        return mon;
                    });
                }
            }
        }


        // TODO:ID Grabar Novedades Estacion Ruta
        public long AdicionarNovedadEstacionRutaCargueDatos(ONNovedadEstacionRutaxGuiaDC Novedad_EstRuta)
        {
            long newIdNovedadEstacionRuta = 0;
            //Se hace paso a ADO de como se graba la novedad
            // 1. Graba en la Tabla de Novedades (NovedadEstacionRuta_OPN)
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarNovedadEstacionRuta_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdManifiestoOperacionNacion", Novedad_EstRuta.NovedadRutaEstacion.IdManifiestoOpeNal);
                cmd.Parameters.AddWithValue("@IdLocalidad", Novedad_EstRuta.NovedadRutaEstacion.IdLocalidadEstacion);
                cmd.Parameters.AddWithValue("@IdTipoNovedadRuta", Novedad_EstRuta.NovedadRutaEstacion.IdTipoNovedad);
                cmd.Parameters.AddWithValue("@FechaNovedad", Novedad_EstRuta.NovedadRutaEstacion.FechaNovedad);
                cmd.Parameters.AddWithValue("@HorasNov", Novedad_EstRuta.NovedadRutaEstacion.HorasNovedad);
                cmd.Parameters.AddWithValue("@MinutosNov", Novedad_EstRuta.NovedadRutaEstacion.MinutosNovedad);
                cmd.Parameters.AddWithValue("@Observaciones", Novedad_EstRuta.NovedadRutaEstacion.Observaciones);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", Novedad_EstRuta.NovedadRutaEstacion.CreadoPor);
                cmd.Parameters.AddWithValue("@ClaseNovedad", Novedad_EstRuta.NovedadRutaEstacion.ClaseNovedad);
                cmd.Parameters.AddWithValue("@TipoAfectacion", Convert.ToInt32(Novedad_EstRuta.NovedadRutaEstacion.TipoAfectacion));
                cmd.Parameters.AddWithValue("@LugarIncidente", Novedad_EstRuta.NovedadRutaEstacion.LugarIncidente);
                cmd.Parameters.AddWithValue("@Retraso", Novedad_EstRuta.NovedadRutaEstacion.Retraso);
                if (Novedad_EstRuta.NovedadRutaEstacion.IdCentroServiciosCOL == 0)
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosCOL", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosCOL", Novedad_EstRuta.NovedadRutaEstacion.IdCentroServiciosCOL);
                }
                //1 Ciudad
                if (string.IsNullOrWhiteSpace(Novedad_EstRuta.NovedadRutaEstacion.IdCiudad))
                {
                    cmd.Parameters.AddWithValue("@IdCiudadAfectada", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCiudadAfectada", Novedad_EstRuta.NovedadRutaEstacion.IdCiudad);
                }
                if (Novedad_EstRuta.NovedadRutaEstacion.IdCentroServiciosPunto == 0)
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosPTO", null);

                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosPTO", Novedad_EstRuta.NovedadRutaEstacion.IdCentroServiciosPunto);
                }
                if (Novedad_EstRuta.NovedadRutaEstacion.IdMensajero == 0)
                {
                    cmd.Parameters.AddWithValue("@IdMensajero", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdMensajero", Novedad_EstRuta.NovedadRutaEstacion.IdMensajero);
                }
                if (Novedad_EstRuta.NovedadRutaEstacion.IdClienteCre == 0)
                {
                    cmd.Parameters.AddWithValue("@IdCliente", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCliente", Novedad_EstRuta.NovedadRutaEstacion.IdClienteCre);
                }
                if (Novedad_EstRuta.NovedadRutaEstacion.IdContrato == 0)
                {
                    cmd.Parameters.AddWithValue("@IdContratoCliCre", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdContratoCliCre", Novedad_EstRuta.NovedadRutaEstacion.IdContrato);
                }
                if (Novedad_EstRuta.NovedadRutaEstacion.IdSucursalContrato == 0)
                {
                    cmd.Parameters.AddWithValue("@IdSucursalContrato", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdSucursalContrato", Novedad_EstRuta.NovedadRutaEstacion.IdSucursalContrato);
                }
                newIdNovedadEstacionRuta = Convert.ToInt64(cmd.ExecuteScalar());
                sqlConn.Close();
            }

            foreach (long guia in Novedad_EstRuta.ListaGuias)
            {
                using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
                {
                    sqlConn.Open();
                    SqlCommand cmd = new SqlCommand("paAfectarGuiasNovedadCargueArchivo_OPN", sqlConn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@newIdNovedadEstacionRuta", newIdNovedadEstacionRuta);
                    cmd.Parameters.AddWithValue("@NumeroGuia", guia);

                    if (Novedad_EstRuta.NovedadRutaEstacion.Retraso > 0)
                    {

                        EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                        {
                            Guia = new ADGuia { NumeroGuia = Convert.ToInt64(guia) },
                            TiempoAfectacion = Novedad_EstRuta.NovedadRutaEstacion.Retraso * 24
                        });
                    }
                }
            }
            return newIdNovedadEstacionRuta;
        }


        public ONManifiestoOperacionNacional ObtenerResponsableGuiaManifiestoPorNGuia(long numeroGuia)
        {
            ONManifiestoOperacionNacional manifiesto = null;

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarManifiestoNacPorNumeroGuia_OPN", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    manifiesto = new ONManifiestoOperacionNacional();

                    if (reader.Read())
                    {
                        manifiesto.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre
                        {
                            CedulaConductor = reader["MOT_IDENTIFICACIONCONDUCTOR"].ToString(),
                            NombreConductor = reader["MOT_NOMBRECONDUCTOR"].ToString()
                        };
                        manifiesto.IdRacolDespacho = Convert.ToInt64(reader["MEN_IDAGENCIA"]);
                    }
                }
                return manifiesto;
            }
        }

        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_ConNovRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta)
        {
            string IdManifiestoOperacionNacional;
            string IdRuta;
            filtro.TryGetValue("MON_IdManifiestoOperacionNacio", out IdManifiestoOperacionNacional);
            filtro.TryGetValue("IdRutaFiltro", out IdRuta);

            long? IdManifiestoOperacionNacional_P = null;
            if (!string.IsNullOrWhiteSpace(IdManifiestoOperacionNacional))
                IdManifiestoOperacionNacional_P = long.Parse(IdManifiestoOperacionNacional);
            IdManifiestoOperacionNacional_P = IdManifiestoOperacionNacional_P == null ? 0 : IdManifiestoOperacionNacional_P;

            int? IdRuta_p = null;
            if (!string.IsNullOrWhiteSpace(IdRuta))
                IdRuta_p = int.Parse(IdRuta);
            IdRuta_p = IdRuta_p == null ? 0 : IdRuta_p;


            long? idCentroServiciosManifiesta_P = idCentroServiciosManifiesta;


            // TODO: ID, Se evita el uso de Entityframework
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerManOpnConNovRuta_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@PageIndex", indicePagina));
                cmd.Parameters.Add(new SqlParameter("@PageSize", registrosPorPagina));
                cmd.Parameters.Add(new SqlParameter("@IdManifiestoOperacionNaci", IdManifiestoOperacionNacional_P));
                cmd.Parameters.Add(new SqlParameter("@IdRuta", IdRuta_p));
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicioManifiesta", idCentroServiciosManifiesta_P));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }


            List<ONManifiestoOperacionNacional> listaMon = new List<ONManifiestoOperacionNacional>();
            ONManifiestoOperacionNacional mon;
            foreach (DataRow iteRow in dsRes.Tables[0].Rows)
            {
                mon = new ONManifiestoOperacionNacional();

                mon.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                mon.EstaManifiestoCerrado = Convert.ToBoolean(iteRow["MON_EstaManifiestoCerrado"]);
                mon.FechaCierre = Convert.ToDateTime(iteRow["MON_FechaCierre"]);
                mon.FechaCreacion = Convert.ToDateTime(iteRow["MON_FechaGrabacion"]);
                mon.IdCentroServiciosManifiesta = Convert.ToInt64(iteRow["MON_IdCentroServiciosManifiesta"]);
                mon.IdEmpresaTransportadora = Convert.ToInt32(iteRow["ETR_IdEmpresaTransportadora"]);
                mon.NombreEmpresaTransportadora = iteRow["ETR_Nombre"].ToString();
                mon.IdManifiestoOperacionNacional = Convert.ToInt64(iteRow["MON_IdManifiestoOperacionNacio"]);
                mon.IdRutaDespacho = Convert.ToInt32(iteRow["MON_IdRutaDespacho"]);
                mon.NombreRuta = iteRow["RUT_Nombre"].ToString();
                mon.IdMedioTransporte = Convert.ToInt32(iteRow["MON_IdMedioTransporte"]);
                mon.IdTipoTransporte = Convert.ToInt32(iteRow["ETR_IdTipoTransporte"]);
                mon.DescripcionMedioTransporte = iteRow["MTR_DescripcionMedioTrans"].ToString();
                mon.NombreTipoTransporte = iteRow["TIT_DescripcionTipoTrans"].ToString();
                mon.RutaGeneraManifiestoMinisterio = Convert.ToBoolean(iteRow["RUT_GeneraManifiestoMinisterio"]);
                if (iteRow["VEH_IdTipoVehiculo"] != DBNull.Value)
                    mon.IdTipoVehiculoManifiesto = Convert.ToInt16(iteRow["VEH_IdTipoVehiculo"]);

                mon.LocalidadDespacho = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                {
                    IdLocalidad = iteRow["MON_IdLocalidadDespacho"].ToString(),
                    Nombre = iteRow["MON_NombreLocalidadDespacho"].ToString()
                };
                mon.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                {
                    IdLocalidad = iteRow["RUT_IdLocalidadDestino"].ToString(),
                    Nombre = iteRow["RUT_NombreLocalidadDestino"].ToString()
                };
                mon.NumeroManifiestoCarga = Convert.ToInt64(iteRow["MON_NumeroManifiestoCarga"]);

                mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre();
                if (iteRow["MOT_IdVehiculo"] != DBNull.Value)
                {
                    mon.ManifiestoTerrestre.PlacaVehiculo = iteRow["MOT_Placa"].ToString();
                    mon.ManifiestoTerrestre.IdVehiculo = Convert.ToInt32(iteRow["MOT_IdVehiculo"]);
                    mon.ManifiestoTerrestre.CedulaConductor = iteRow["MOT_IdentificacionConductor"].ToString();
                    mon.ManifiestoTerrestre.NombreConductor = iteRow["MOT_NombreConductor"].ToString();
                    mon.ManifiestoTerrestre.IdConductor = Convert.ToInt64(iteRow["MOT_IdConductor"]);
                }

                listaMon.Add(mon);
            }

            return listaMon;

        }


        public IList<ONNovedadEstacionRutaDC> Obtener_NovedadesEstacionRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta)
        {
            string IdManifiestoOperacionNacional;
            string IdRuta;
            filtro.TryGetValue("MON_IdManifiestoOperacionNacio", out IdManifiestoOperacionNacional);
            filtro.TryGetValue("IdRutaFiltro", out IdRuta);

            long? IdManifiestoOperacionNacional_P = null;
            if (!string.IsNullOrWhiteSpace(IdManifiestoOperacionNacional))
                IdManifiestoOperacionNacional_P = long.Parse(IdManifiestoOperacionNacional);
            IdManifiestoOperacionNacional_P = IdManifiestoOperacionNacional_P == null ? 0 : IdManifiestoOperacionNacional_P;

            int? IdRuta_p = null;
            if (!string.IsNullOrWhiteSpace(IdRuta))
                IdRuta_p = int.Parse(IdRuta);
            IdRuta_p = IdRuta_p == null ? 0 : IdRuta_p;


            long? idCentroServiciosManifiesta_P = idCentroServiciosManifiesta;


            // TODO: ID, Se evita el uso de Entityframework
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerNovedadesEstacionRuta_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@PageIndex", indicePagina));
                cmd.Parameters.Add(new SqlParameter("@PageSize", registrosPorPagina));
                cmd.Parameters.Add(new SqlParameter("@IdManifiestoOperacionNaci", IdManifiestoOperacionNacional_P));
                cmd.Parameters.Add(new SqlParameter("@IdRuta", IdRuta_p));
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicioManifiesta", idCentroServiciosManifiesta_P));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }


            List<ONNovedadEstacionRutaDC> lista = new List<ONNovedadEstacionRutaDC>();
            ONNovedadEstacionRutaDC nov;
            foreach (DataRow iteRow in dsRes.Tables[0].Rows)
            {
                nov = new ONNovedadEstacionRutaDC();
                nov.IdNovedadEstacionRuta = Convert.ToInt64(iteRow["NER_IdNovedadEstacionRuta"]);
                nov.ClaseNovedad = iteRow["NER_ClaseNovedad"].ToString();

                nov.TipoAfectacion = (ONEnumTipoAfectaNovGuia)Enum.Parse(typeof(ONEnumTipoAfectaNovGuia), iteRow["NER_TipoAfectacion"].ToString());

                nov.FechaNovedad = Convert.ToDateTime(iteRow["NER_FechaNovedad"]);
                nov.NombreLocalidad = iteRow["NombreLocalidad"].ToString();

                nov.IdManifiestoOpeNal = Convert.ToInt64(iteRow["NER_IdManifiestoOperacionNacio"]);

                nov.IdCentroServiciosCOL = Convert.ToInt64(iteRow["NER_IdCentroServiciosCol"]);
                nov.IdCentroServiciosPunto = Convert.ToInt64(iteRow["NER_IdCentroServiciosPTO"]);
                nov.IdMensajero = Convert.ToInt64(iteRow["NER_IdMensajero"]);

                nov.NombreNovedad = iteRow["NombreNovedad"].ToString();

                nov.NombreCiudad = iteRow["NombreCiudad"].ToString();

                nov.NombreCOL = iteRow["NombreCol"].ToString();
                nov.NombrePunto = iteRow["NombrePto"].ToString();
                nov.NombreMensajero = iteRow["NombreMensajero"].ToString();

                nov.NombreClienteCre = iteRow["NombreClienteCre"].ToString();
                nov.NombreContrato = iteRow["NombreContrato"].ToString();
                nov.NombreSucursalCon = iteRow["NombreSucursalCon"].ToString();


                nov.HorasNovedad = Convert.ToInt32(iteRow["NER_HorasNov"]);
                nov.MinutosNovedad = Convert.ToInt32(iteRow["NER_MinutosNov"]);
                nov.LugarIncidente = iteRow["NER_LugarIncidente"].ToString();
                nov.Observaciones = iteRow["NER_Observaciones"].ToString();
                nov.Retraso = Convert.ToInt32(iteRow["NER_Retraso"]);


                lista.Add(nov);
            }

            return lista;

        }

        public ONManifiestoOperacionNacional Obtener_ManifiestoxId(long IdManifiesto)
        {
            ONManifiestoOperacionNacional mon = new ONManifiestoOperacionNacional();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerManOpnPorId_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdManifiesto", IdManifiesto));

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    mon.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                    mon.EstaManifiestoCerrado = Convert.ToBoolean(reader["MON_EstaManifiestoCerrado"]);
                    mon.FechaCierre = Convert.ToDateTime(reader["MON_FechaCierre"]);
                    mon.FechaCreacion = Convert.ToDateTime(reader["MON_FechaGrabacion"]);
                    mon.IdCentroServiciosManifiesta = Convert.ToInt64(reader["MON_IdCentroServiciosManifiesta"]);
                    mon.IdEmpresaTransportadora = Convert.ToInt32(reader["ETR_IdEmpresaTransportadora"]);
                    mon.NombreEmpresaTransportadora = reader["ETR_Nombre"].ToString();
                    mon.IdManifiestoOperacionNacional = Convert.ToInt64(reader["MON_IdManifiestoOperacionNacio"]);
                    mon.IdRutaDespacho = Convert.ToInt32(reader["MON_IdRutaDespacho"]);
                    mon.NombreRuta = reader["RUT_Nombre"].ToString();
                    mon.IdMedioTransporte = Convert.ToInt32(reader["MON_IdMedioTransporte"]);
                    mon.IdTipoTransporte = Convert.ToInt32(reader["ETR_IdTipoTransporte"]);
                    mon.DescripcionMedioTransporte = reader["MTR_DescripcionMedioTrans"].ToString();
                    mon.NombreTipoTransporte = reader["TIT_DescripcionTipoTrans"].ToString();
                    mon.RutaGeneraManifiestoMinisterio = Convert.ToBoolean(reader["RUT_GeneraManifiestoMinisterio"]);
                    if (reader["VEH_IdTipoVehiculo"] != DBNull.Value)
                        mon.IdTipoVehiculoManifiesto = Convert.ToInt16(reader["VEH_IdTipoVehiculo"]);

                    mon.LocalidadDespacho = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                    {
                        IdLocalidad = reader["MON_IdLocalidadDespacho"].ToString(),
                        Nombre = reader["MON_NombreLocalidadDespacho"].ToString()
                    };
                    mon.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                    {
                        IdLocalidad = reader["RUT_IdLocalidadDestino"].ToString(),
                        Nombre = reader["RUT_NombreLocalidadDestino"].ToString()
                    };
                    mon.NumeroManifiestoCarga = Convert.ToInt64(reader["MON_NumeroManifiestoCarga"]);

                    mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre();
                    if (reader["MOT_IdVehiculo"] != DBNull.Value)
                    {
                        mon.ManifiestoTerrestre.PlacaVehiculo = reader["MOT_Placa"].ToString();
                        mon.ManifiestoTerrestre.IdVehiculo = Convert.ToInt32(reader["MOT_IdVehiculo"]);
                        mon.ManifiestoTerrestre.CedulaConductor = reader["MOT_IdentificacionConductor"].ToString();
                        mon.ManifiestoTerrestre.NombreConductor = reader["MOT_NombreConductor"].ToString();
                        mon.ManifiestoTerrestre.IdConductor = Convert.ToInt64(reader["MOT_IdConductor"]);
                    }

                }
            }
            return mon;
        }


        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_Disponibles(long idCentroServicios, int idRuta, DateTime fechaInicial, DateTime fechaFinal)
        {

            //long? idCentroServiciosManifiesta_P = idCentroServicios;
            // TODO: ID, Se evita el uso de Entityframework,
            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerManOpnHabilitados_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCentroServicioManifiesta", idCentroServicios));
                cmd.Parameters.Add(new SqlParameter("@IdRuta", idRuta));
                cmd.Parameters.Add(new SqlParameter("@FechaInicial", fechaInicial));
                cmd.Parameters.Add(new SqlParameter("@FechaFinal", fechaFinal));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }


            List<ONManifiestoOperacionNacional> listaMon = new List<ONManifiestoOperacionNacional>();
            ONManifiestoOperacionNacional mon;
            foreach (DataRow iteRow in dsRes.Tables[0].Rows)
            {
                mon = new ONManifiestoOperacionNacional();

                mon.EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS;
                mon.EstaManifiestoCerrado = Convert.ToBoolean(iteRow["MON_EstaManifiestoCerrado"]);
                mon.FechaCierre = Convert.ToDateTime(iteRow["MON_FechaCierre"]);
                mon.FechaCreacion = Convert.ToDateTime(iteRow["MON_FechaGrabacion"]);
                mon.IdCentroServiciosManifiesta = Convert.ToInt64(iteRow["MON_IdCentroServiciosManifiesta"]);
                mon.IdEmpresaTransportadora = Convert.ToInt32(iteRow["ETR_IdEmpresaTransportadora"]);
                mon.NombreEmpresaTransportadora = iteRow["ETR_Nombre"].ToString();
                mon.IdManifiestoOperacionNacional = Convert.ToInt64(iteRow["MON_IdManifiestoOperacionNacio"]);
                mon.IdRutaDespacho = Convert.ToInt32(iteRow["MON_IdRutaDespacho"]);
                mon.NombreRuta = iteRow["RUT_Nombre"].ToString();
                mon.IdMedioTransporte = Convert.ToInt32(iteRow["MON_IdMedioTransporte"]);
                mon.IdTipoTransporte = Convert.ToInt32(iteRow["ETR_IdTipoTransporte"]);
                mon.DescripcionMedioTransporte = iteRow["MTR_DescripcionMedioTrans"].ToString();
                mon.NombreTipoTransporte = iteRow["TIT_DescripcionTipoTrans"].ToString();
                mon.RutaGeneraManifiestoMinisterio = Convert.ToBoolean(iteRow["RUT_GeneraManifiestoMinisterio"]);
                if (iteRow["VEH_IdTipoVehiculo"] != DBNull.Value)
                    mon.IdTipoVehiculoManifiesto = Convert.ToInt16(iteRow["VEH_IdTipoVehiculo"]);

                mon.LocalidadDespacho = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                {
                    IdLocalidad = iteRow["MON_IdLocalidadDespacho"].ToString(),
                    Nombre = iteRow["MON_NombreLocalidadDespacho"].ToString()
                };
                mon.LocalidadDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                {
                    IdLocalidad = iteRow["RUT_IdLocalidadDestino"].ToString(),
                    Nombre = iteRow["RUT_NombreLocalidadDestino"].ToString()
                };
                mon.NumeroManifiestoCarga = Convert.ToInt64(iteRow["MON_NumeroManifiestoCarga"]);

                mon.ManifiestoTerrestre = new ONManifiestoOperacionNalTerrestre();
                if (iteRow["MOT_IdVehiculo"] != DBNull.Value)
                {
                    mon.ManifiestoTerrestre.PlacaVehiculo = iteRow["MOT_Placa"].ToString();
                    mon.ManifiestoTerrestre.IdVehiculo = Convert.ToInt32(iteRow["MOT_IdVehiculo"]);
                    mon.ManifiestoTerrestre.CedulaConductor = iteRow["MOT_IdentificacionConductor"].ToString();
                    mon.ManifiestoTerrestre.NombreConductor = iteRow["MOT_NombreConductor"].ToString();
                    mon.ManifiestoTerrestre.IdConductor = Convert.ToInt64(iteRow["MOT_IdConductor"]);
                }

                listaMon.Add(mon);
            }

            return listaMon;




        }


        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoOperaAgencGuiaNoReg_OPN guia = contexto.IngresoOperaAgencGuiaNoReg_OPN.FirstOrDefault(g => g.IGN_NumeroGuia == numeroGuia);// && g.IGN_IdCentroServicio == idAgencia);
                return guia != null;
            }
        }

        /// <summary>
        /// Obtiene todos los manifiestos abiertos con origen en una ciudad a donde un vehiculo esta asignado
        /// </summary>
        /// <param name="idLocDespachoMan"></param>
        /// <param name="placa"></param>
        /// <returns></returns>
        public List<long> ObtenerManiAbiCiudadOrigVehiculo(string idLocDespachoMan, string placa)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerManifAbiertosCiudadOrigVehiculo_OPN(idLocDespachoMan, placa).ToList().ConvertAll<long>(m =>
                {
                    return m.MON_IdManifiestoOperacionNacio;
                });
            }
        }

        /// <summary>
        /// Valida si una guia esta dentro del centro de acopio de un centro se servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="numeroGuia"></param>
        /// <returns>True: si esta ingresada la guia,  False: no esta en el centro de acopio</returns>
        public bool ValidarGuiaCentroAcopio(long numeroGuia, long idAgencia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int? validacion = contexto.paValidarGuiaCentroAcopio_OPN(idAgencia, numeroGuia).First();
                return validacion != null && validacion.Value > 0 ? true : false;
            }
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresasda a centro de copio antes de haberla creado en el sistema.
        /// </summary>
        /// <param name="numeroGuia">Retorna el id del centro de servicio que ingresó a centro de acopio la guía</param>
        /// <returns></returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroServicio(long numeroGuia)
        {
            //using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paValidarIngresoCentroAcopioSinCrear_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@numeroGuia", numeroGuia);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var guia = dt.AsEnumerable().FirstOrDefault();
                //IngresoOperaAgencGuiaNoReg_OPN guia = contexto.IngresoOperaAgencGuiaNoReg_OPN.OrderByDescending(g => g.IGN_FechaGrabacion).FirstOrDefault(g => g.IGN_NumeroGuia == numeroGuia);
                if (guia != null)
                    return guia.Field<long>("IGN_IdCentroServicio");
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el peso de los envios ingresados a un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public decimal ObtenerPesoEnviosManifiesto(long idManifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var f = contexto.paPesoTotalEnviosManifiesto_OPU(idManifiesto).FirstOrDefault();
                if (f != null)
                {
                    decimal? peso = f.PesoEnvios;
                    if (peso != null)
                        return peso.Value;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// Obtiene una lista de los vehiculos activos que pertenecen a un racol, que tienen ingreso a un col, activos, y con fechavencimiento de soat y de tecnomecanica vigente
        /// </summary>
        /// <param name="idRacol">Id del racol al que pertenece el vehiculo</param>
        /// <param name="idCentroServicios">Id del centro de servicios al que ingreso el vehiculo</param>
        /// <returns></returns>
        public IList<POVehiculo> ObtenerVehiculosIngresoCentroServicioXRacol(long idRacol, long idCentroServicios, DateTime? fechaActual)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var vehiculos = contexto.IngresoVehiculoRacol_VOPN.Where(v => v.ROV_IdRegionalAdm == idRacol && v.IST_IdAgenciaIngresoSalida == idCentroServicios
                 && v.VEH_Estado == ConstantesFramework.ESTADO_ACTIVO && v.REM_FechaVencimiento >= fechaActual && v.POS_IdTipoPolizaSerguro == POConstantesParametrosOperacion.TipoPolizaSeguro_SOAT
                 && v.POS_FechaVencimiento >= fechaActual).ToList().
                 ConvertAll<POVehiculo>(v => new POVehiculo()
                 {
                     Placa = v.IST_Placa,
                     IdVehiculo = v.IST_IdVehiculo,
                     ReportarSatrack = v.VEH_ReportarSatrack
                 }).OrderBy(v => v.Placa).ToList(); ;

                return vehiculos;
            }
        }

        /// <summary>
        /// Obtiene una lista de los vehiculos activos que pertenecen a un racol, sin tener en cuenta el ingreso a un col o agencia, activos, y con fechavencimiento de soat y de tecnomecanica vigente
        /// </summary>
        /// <param name="idRacol">Id del racol al que pertenece el vehiculo</param>
        /// <param name="idCentroServicios">Id del centro de servicios al que ingreso el vehiculo</param>
        /// <returns></returns>
        public IList<POVehiculo> ObtenerVehiculosManifestarSinIngresoCentroServicioXRacol(long idRacol, DateTime? fechaActual)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var vehiculos = contexto.VehiculosHabilitadosManifiestoSinIngresoAgenciaPorRacol_VOPN.Where(v => v.ROV_IdRegionalAdm == idRacol
                 && v.VEH_Estado == ConstantesFramework.ESTADO_ACTIVO && v.REM_FechaVencimiento >= fechaActual && v.POS_IdTipoPolizaSerguro == POConstantesParametrosOperacion.TipoPolizaSeguro_SOAT
                 && v.POS_FechaVencimiento >= fechaActual);
                List<POVehiculo> lstVehiculos = vehiculos.ToList().ConvertAll<POVehiculo>(v => new POVehiculo()
                {
                    Placa = v.VEH_Placa,
                    IdVehiculo = v.VEH_IdVehiculo,
                    ReportarSatrack = v.VEH_ReportarSatrack
                }).OrderBy(v => v.Placa).ToList();

                return lstVehiculos;
            }
        }

        /// <summary>
        /// Inserta un manifiesto nacional
        /// </summary>
        /// <param name="manifiesto"></param>
        public void AgregarManifiestoOperacionNacional(ONManifiestoOperacionNacional manifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //En la creacion del manifiesto,  La fecha de creacion y la de cierre deben ser iguales, para poder verificar si el
                //manifiesto ha sido cerrado o reabierto
                int? idVehiculo = null;
                if (manifiesto.IdVehiculoDespacho > 0)
                    idVehiculo = manifiesto.IdVehiculoDespacho;

                var Manifiesto = contexto.paCrearManifiestoOpnNac_OPN(
                  manifiesto.IdManifiestoOperacionNacional,
                  manifiesto.LocalidadDespacho.IdLocalidad,
                  manifiesto.LocalidadDespacho.Nombre,
                  manifiesto.IdRutaDespacho,
                  (short)manifiesto.IdEmpresaTransportadora,
                  manifiesto.NumeroManifiestoCarga,
                  manifiesto.IdCentroServiciosManifiesta,
                  DateTime.Now,
                  false,
                  (short)manifiesto.IdMedioTransporte,
                  DateTime.Now,
                  false,
                  idVehiculo,
                  ControllerContext.Current.Usuario, manifiesto.FechaSalidaSatrack).FirstOrDefault();
            }
        }

        /// <summary>
        /// Inserta un manifiesto nacional
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EditarManifiestoOperacionNacional(ONManifiestoOperacionNacional manifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                if (manifiesto.IdTipoTransporte == OPConstantesOperacionNacional.ID_TIPO_TRANSPORTE_PROPIO)
                {
                    contexto.paModificarManifiestoOpnNac_OPN(
                      manifiesto.IdManifiestoOperacionNacional,
                      (short)manifiesto.IdEmpresaTransportadora,
                      manifiesto.IdVehiculoDespacho);
                }
                else
                {
                    contexto.paModificarManifiestoOpnNac_OPN(
                      manifiesto.IdManifiestoOperacionNacional,
                      (short)manifiesto.IdEmpresaTransportadora,
                      null);
                }

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// inserta manifiesto de operacion nacional terrestre
        /// </summary>
        /// <param name="manifiesto"></param>
        public void AgregarManifiestoOperacionNacionalTerrestre(ONManifiestoOperacionNalTerrestre manifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearManiOpnNacTerrestre_OPN(manifiesto.IdManifiestoOperacionNacional,
                  manifiesto.IdVehiculo,
                  manifiesto.IdConductor,
                  manifiesto.PlacaVehiculo,
                  manifiesto.CedulaConductor,
                  manifiesto.NombreConductor,
                  ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// modifica un  manifiesto de operacion nacional terrestre
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EditarManifiestoOperacionNacionalTerrestre(ONManifiestoOperacionNalTerrestre manifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paModificarManiOpnNacTerrestre_OPN(manifiesto.IdManifiestoOperacionNacional,
                  manifiesto.IdVehiculo,
                  manifiesto.IdConductor,
                  manifiesto.PlacaVehiculo,
                  manifiesto.CedulaConductor,
                  manifiesto.NombreConductor);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Valida si existe el manifiesto de operacion nacional terrestre
        /// </summary>
        /// <param name="idManifiestoOperNacional"></param>
        public bool ValidarExisteManifiestoOperacionNacionalTerrestre(long idManifiestoOperNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.ManifiestoOperNalTerrestre_OPN.Where(m => m.MOT_IdManifiestoOperacionNacio == idManifiestoOperNacional).Count() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Elimina un  manifiesto de operacion nacional terrestre
        /// </summary>
        /// <param name="manifiesto"></param>
        public void EliminarManifiestoOperacionNacionalTerrestre(long idManifiestoOperNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paEliminarManiOpnNacTerrestre_OPN(idManifiestoOperNacional);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene los consolidados de un manifiesto de la operacion nacional
        /// </summary>
        /// <param name="idManifiestoOperacionNacional">Identificador del manifiesto de la operacion nacional</param>
        /// <returns></returns>
        public IList<ONConsolidado> ObtenerConsolidadosManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional, string idLocalidad)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ConsolidadosManifiestos_VOPN>("MOC_IdManifiestoOperacionNacio", idManifiestoOperacionNacional.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);
                if (!string.IsNullOrEmpty(idLocalidad))
                {
                    LambdaExpression lamda2 = contexto.CrearExpresionLambda<ConsolidadosManifiestos_VOPN>("MOC_IdLocalidadManifestada", idLocalidad.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda2, OperadorLogico.And);
                }

                var consolidado = contexto.ConsultarConsolidadosManifiestos_VOPN(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
                  .ConvertAll<ONConsolidado>(m =>
                    new ONConsolidado()
                    {
                        IdManifiestoOperacionNacional = m.MOC_IdManifiestoOperacionNacio,
                        DescripcionConsolidadoDetalle = m.MOC_DescpConsolidadoDetalle,
                        IdManfiestoConsolidado = m.MOC_IdManfiestoOperaNacioConso,
                        FechaGrabacion = m.MOC_FechaGrabacion,
                        IdGuiaInterna = m.MOC_IdGuiaInterna,
                        IdTipoConsolidado = m.MOC_IdTipoConsolidado,
                        NombreTipoConsolidado = m.TIC_Descripcion,
                        IdTipoConsolidadoDetalle = m.MOC_TipoConsolidadoDetalle,
                        NombreTipoConsolidadoDetalle = m.MOC_DescpConsolidadoDetalle,
                        LocalidadManifestada = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                        {
                            Nombre = m.MOC_NombreLocalidadManifestada,
                            IdLocalidad = m.MOC_IdLocalidadManifestada
                        },
                        NumeroContenedorTula = m.MOC_NumeroContenedorTula,
                        NumeroGuiaInterna = m.MOC_NumeroGuiaInterna,
                        NumeroPrecintoRetorno = m.MOC_NumeroPrecintoRetorno,
                        NumeroPrecintoSalida = m.MOC_NumeroPrecintoSalida,
                        TotalEnviosConsolidado = m.CantidadEnvios.Value,
                        NumControlTransManIda = m.MOC_NumControlTransManIda
                    });
                return consolidado;
            }
        }

        public ONConsolidado ObtenerConsolidadoManifiestoIdConsoMani(ONConsolidado consolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsolidadosManifiestos_VOPN.Where(c => c.MOC_IdManifiestoOperacionNacio == consolidado.IdManifiestoOperacionNacional && c.MOC_IdManfiestoOperaNacioConso == consolidado.IdManfiestoConsolidado).ToList().
                    ConvertAll(c =>
                new ONConsolidado()
                {
                    NumeroContenedorTula = c.MOC_NumeroContenedorTula
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene todas las guias manifestadas, incluye guias sueltas y guias consolidadas
        /// </summary>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerTodasGuiasManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<GuiasManifestadas_VOPN>("IdManifiestoOperacionNacional", idManifiestoOperacionNacional.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                var guias = contexto.ConsultarGuiasManifestadas_VOPN(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente).ToList()
                  .ConvertAll<ONManifiestoGuia>(c =>
                    new ONManifiestoGuia()
                    {
                        IdManifiestoGuia = c.IdManOperNacGuia_ConDetalle,
                        IdManifiestoOperacionNacional = c.IdManifiestoOperacionNacional,
                        IdManifiestoConsolidado = c.IdManfiestoOperaNacioConsolidado,
                        IdAdmisionMensajeria = c.IdAdminisionMensajeria,
                        NumeroGuia = c.NumeroGuia,
                        NombreTipoEnvio = c.NombreTipoEnvio,
                        NombreCiudadDestino = c.NombreCiudadDestino,
                        GuiaSuelta = Convert.ToBoolean(c.GuiaSuelta)
                    });
                return guias;
            }
        }

        /// <summary>
        /// Obtiene todas las guias manifestadas, incluye guias sueltas y guias consolidadas
        /// </summary>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerTodasGuiasManifiesto(long idManifiestoOperacionNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guias = contexto.GuiasManifestadas_VOPN.Where(g => g.IdManifiestoOperacionNacional == idManifiestoOperacionNacional).ToList()
                  .ConvertAll<ONManifiestoGuia>(c =>
                    new ONManifiestoGuia()
                    {
                        IdManifiestoGuia = c.IdManOperNacGuia_ConDetalle,
                        IdManifiestoOperacionNacional = c.IdManifiestoOperacionNacional,
                        IdManifiestoConsolidado = c.IdManfiestoOperaNacioConsolidado,
                        IdAdmisionMensajeria = c.IdAdminisionMensajeria,
                        NumeroGuia = c.NumeroGuia,
                        NombreTipoEnvio = c.NombreTipoEnvio,
                        NombreCiudadDestino = c.NombreCiudadDestino,
                        GuiaSuelta = Convert.ToBoolean(c.GuiaSuelta)
                    });
                return guias;
            }
        }

        /// <summary>
        /// Obtiene todos los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidado> ObtenerTipoConsolidado()
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoConsolidado_OPN.ToList().
                  ConvertAll<ONTipoConsolidado>(t =>
                    new ONTipoConsolidado()
                    {
                        Descripcion = t.TIC_Descripcion,
                        IdTipoConsolidado = t.TIC_IdTipoConsolidado
                    });
            }
        }

        // TODO:ID Obtener Lista Tipos de Novedad Estacion-Ruta
        public IList<ONTipoNovedadEstacionRutaDC> ObtenerTiposNovedadEstacionRuta()
        {
            List<ONTipoNovedadEstacionRutaDC> lista = new List<ONTipoNovedadEstacionRutaDC>();
            DataSet dsRes = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("SELECT TNR_IdTipoNovedadRuta,TNR_NombreNovedad FROM TipoNovedadRuta_OPN WITH(NOLOCK)", sqlConn);
                cmd.CommandType = System.Data.CommandType.Text;

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }

            ONTipoNovedadEstacionRutaDC TipoNov;
            foreach (DataRow item in dsRes.Tables[0].Rows)
            {
                TipoNov = new ONTipoNovedadEstacionRutaDC()
                {
                    IdNovedad = Convert.ToInt16(item["TNR_IdTipoNovedadRuta"]),
                    Descripcion = item["TNR_NombreNovedad"].ToString(),
                };
                lista.Add(TipoNov);
            }

            return lista;
        }


        /// <summary>
        /// Obtiene todos los detalles de los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidadoDetalle> ObtenerTipoConsolidadoDetalle(int idTipoConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoConsolidadoDetalle_OPN.Where(t => t.TCD_IdTipoConsolidado == idTipoConsolidado).ToList().
                  ConvertAll<ONTipoConsolidadoDetalle>(t =>
                    new ONTipoConsolidadoDetalle()
                    {
                        IdTipoConsolidadoDetalle = t.TCD_TipoConsolidadoDetalle,
                        Descripcion = t.TCD_Descripcion,
                        IdTipoConsolidado = t.TCD_IdTipoConsolidado
                    });
            }
        }

        /// <summary>
        /// Inserta un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public ONConsolidado AdicionarConsolidado(ONConsolidado consolidado, long? idGuiaInterna = null)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                lock (this)
                {
                    var Consolidado = contexto.paCrearConsoManiOpnNac_OPN(
                        consolidado.IdManifiestoOperacionNacional,
                        consolidado.LocalidadManifestada.IdLocalidad,
                       consolidado.LocalidadManifestada.Nombre,
                       (short)consolidado.IdTipoConsolidado,
                       (short)consolidado.IdTipoConsolidadoDetalle,
                       consolidado.DescripcionConsolidadoDetalle,
                        consolidado.NumeroContenedorTula,
                        idGuiaInterna,
                        consolidado.NumeroGuiaInterna,
                        consolidado.NumeroPrecintoRetorno,
                         consolidado.NumeroPrecintoSalida,
                         consolidado.NumControlTransManIda,
                         consolidado.NumControlTransManRet,
                         false,
                         ControllerContext.Current.Usuario).FirstOrDefault();

                    if (Consolidado != null && Consolidado.IdConsolidado != null)
                    {
                        consolidado.IdManfiestoConsolidado = Convert.ToInt64(Consolidado.IdConsolidado.Value);
                        return consolidado;
                    }
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_CONSOLIDADO_DETALLE.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_CONSOLIDADO)));
                }
            }
        }

        /// <summary>
        /// Inserta un envio al consolidado
        /// </summary>
        /// <param name="consolidadoDetalle"></param>
        /// <returns></returns>
        public long AdicionarConsolidadoDetalle(ONConsolidadoDetalle consolidadoDetalle)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                try
                {
                    var ConsolidadoDet = contexto.paCrearConsoDetaManiOpnNac_OPN(consolidadoDetalle.IdManfiestoConsolidado, consolidadoDetalle.IdAdminisionMensajeria, consolidadoDetalle.NumeroGuia, (short)consolidadoDetalle.PiezaActual, (short)consolidadoDetalle.TotalPiezas, consolidadoDetalle.EstaDescargada, consolidadoDetalle.PesoEnIngreso, (short)consolidadoDetalle.IdTipoEnvio, consolidadoDetalle.NombreTipoEnvio, consolidadoDetalle.IdCentroServicioOrigen, consolidadoDetalle.NombreCentroServicioOrigen, consolidadoDetalle.IdCentroServicioDestino, consolidadoDetalle.NombreCentroServicioDestino, consolidadoDetalle.IdCiudadDestino, consolidadoDetalle.NombreCiudadDestino, consolidadoDetalle.IdCiudadOrigen, consolidadoDetalle.NombreCiudadOrigen, ControllerContext.Current.Usuario).FirstOrDefault();

                    if (ConsolidadoDet != null && ConsolidadoDet.IdConsolidadoDetalle != null)
                        return (long)ConsolidadoDet.IdConsolidadoDetalle.Value;
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_CONSOLIDADO_DETALLE.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_CONSOLIDADO_DETALLE)));
                    }
                }
                catch (System.Data.EntityCommandExecutionException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        if ((ex.InnerException as SqlException).Number == 2627)
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA)));
                        else
                            throw;
                    }
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Modifica un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public void EditarConsolidado(ONConsolidado consolidado, long? idGuiaInterna = null)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paModificarConsoManiOpnNac_OPN(consolidado.IdManfiestoConsolidado, consolidado.LocalidadManifestada.IdLocalidad
                    , consolidado.LocalidadManifestada.Nombre, (short)consolidado.IdTipoConsolidado, (short)consolidado.IdTipoConsolidadoDetalle, consolidado.DescripcionConsolidadoDetalle,
                 consolidado.NumeroContenedorTula, consolidado.NumeroPrecintoRetorno,
                  consolidado.NumeroPrecintoSalida);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene las guias de un consolidado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public IList<ONConsolidadoDetalle> ObtenerGuiasConsolidado(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string IdCiudadDestino;
                filtro.TryGetValue("IdCiudadDestino", out IdCiudadDestino);

                return contexto.paObtenerGuiasConsolidado_OPN(indicePagina, registrosPorPagina, idConsolidado, IdCiudadDestino).ToList().ConvertAll<ONConsolidadoDetalle>(c =>
                {
                    ONConsolidadoDetalle consoDetalle = new ONConsolidadoDetalle()
                    {
                        EstaDescargada = c.MOD_EstaDescargada,
                        IdAdminisionMensajeria = c.MOD_IdAdminisionMensajeria,
                        PesoEnIngreso = c.MOD_PesoEnIngreso,
                        IdManfiestoConsolidado = c.MOD_IdManfiestoOperaNacioConso,
                        IdManifiestoConsolidadoDetalle = c.MOD_IdManfiestoOpeNaConDetalle,
                        NumeroGuia = c.MOD_NumeroGuia,
                        NombreCiudadDestino = c.MOD_NombreCiudadDestino,
                        TotalPiezas = (int)c.MOD_TotalPiezas,
                        PiezaActual = (int)c.MOD_NumeroPieza
                    };

                    if (c.MOD_TotalPiezas > 0)
                    {
                        consoDetalle.GuiaRotulo = c.MOD_NumeroGuia + "-" + c.MOD_NumeroPieza.ToString() + "/" + c.MOD_TotalPiezas.ToString();
                    }
                    else
                        consoDetalle.GuiaRotulo = c.MOD_NumeroGuia.ToString();

                    return consoDetalle;
                });
            }
        }

        /// <summary>
        /// Inserta una guia suelta a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public long AdicionarGuiaSuelta(ONManifiestoGuia guia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                try
                {
                    var guiaSuelta = contexto.paCrearGuiaSueltaMani_OPN(guia.IdManifiestoOperacionNacional, guia.IdAdmisionMensajeria, guia.NumeroGuia, guia.IdLocalidadManifestada, guia.NombreLocalidadManifestada, guia.EstaDescargada, guia.PesoEnIngreso, (short)guia.IdTipoEnvio, guia.NombreTipoEnvio, guia.IdCentroServicioOrigen, guia.NombreCentroServicioOrigen, guia.IdCentroServicioDestino, guia.NombreCentroServicioDestino, guia.IdCiudadDestino, guia.NombreCiudadDestino, guia.IdCiudadOrigen, guia.NombreCiudadOrigen, ControllerContext.Current.Usuario).FirstOrDefault();

                    if (guiaSuelta != null && guiaSuelta.IdManifiestoGuiaSuelta != null)
                        return (long)guiaSuelta.IdManifiestoGuiaSuelta.Value;
                    else
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_CREACION_GUIA_SUELTA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_CREACION_GUIA_SUELTA)));
                    }
                }
                catch (System.Data.EntityCommandExecutionException ex)
                {
                    if (ex.InnerException != null && ex.InnerException is SqlException)
                    {
                        if ((ex.InnerException as SqlException).Number == 2627)
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA)));
                        else
                            throw;
                    }
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Obtiene todas las guias sueltas de un manifiesto nacional
        /// </summary>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerGuiasSueltas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idManifiestoOpeNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string IdCiudadDestino, NumeroGuia, IdCiudadManifestada;

                filtro.TryGetValue("IdCiudadDestino", out IdCiudadDestino);
                filtro.TryGetValue("NumeroGuia", out NumeroGuia);
                filtro.TryGetValue("IdCiudadManifestada", out IdCiudadManifestada);

                return contexto.paObtenerGuiasSueltasMani_OPN(indicePagina, registrosPorPagina, idManifiestoOpeNacional, IdCiudadDestino, Convert.ToInt64(NumeroGuia), IdCiudadManifestada).ToList().ConvertAll<ONManifiestoGuia>(c =>
                {
                    ONManifiestoGuia guiaSuelta = new ONManifiestoGuia()
                    {
                        EstaDescargada = c.MOG_EstaDescargada,
                        PesoEnIngreso = c.MOG_PesoEnIngreso,
                        NumeroGuia = c.MOG_NumeroGuia,
                        IdAdmisionMensajeria = c.MOG_IdAdminisionMensajeria,
                        IdLocalidadManifestada = c.MOG_IdLocalidadManifestada,
                        IdManifiestoGuia = c.MOG_IdManfiestoOperacionNacGuia,
                        NombreLocalidadManifestada = c.MOG_NombreLocalidadManifestada,
                        IdManifiestoOperacionNacional = c.MOG_IdManifiestoOperacionNacio,
                        NombreCiudadDestino = c.MOG_NombreCiudadDestino,
                        IdTipoEnvio = c.MOG_IdTipoEnvio,
                        NombreTipoEnvio = c.MOG_NombreTipoEnvio
                    };
                    if (c.MOG_TotalPiezas > 0)
                    {
                        guiaSuelta.GuiaRotulo = c.MOG_NumeroGuia + "-" + c.MOG_NumeroPieza.ToString() + "/" + c.MOG_TotalPiezas.ToString();
                    }
                    else
                        guiaSuelta.GuiaRotulo = c.MOG_NumeroGuia.ToString();

                    return guiaSuelta;
                });

            }
        }

        /// <summary>
        /// Obtiene todos los motivos de eliminacion de una guia
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoMotivoElimGuiaMani> ObtenerTodosMotivosEliminacionGuia()
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoMotivoElimancionGuia_OPN.ToList().ConvertAll<ONTipoMotivoElimGuiaMani>(m =>
                  new ONTipoMotivoElimGuiaMani()
                  {
                      IdTipoMotivo = m.TME_IdTipoMotivoElimancionGuia,
                      Descripcion = m.TME_Descricpcion
                  });
            }
        }

        /// <summary>
        /// Elimina guia de un consolidado
        /// </summary>
        /// <param name="IdManifiestoConsolidadoDetalle"></param>
        public void EliminarGuiaConsolidado(long idManifiestoConsolidadoDetalle)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paElimnarGuiaManiConDetalle_OPN(idManifiestoConsolidadoDetalle);
            }
        }

        /// <summary>
        /// Elimina guia suelta de manifiesto
        /// </summary>
        /// <param name="idManifiestoGuia"></param>
        public void EliminarGuiaSueltaManifiesto(long idManifiestoGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paElimnarGuiaSueltaMani_OPN(idManifiestoGuia);
            }
        }

        /// <summary>
        /// inserta el motivo por el cual se elimina una guia suelta o de un consolidado de un manifiesto
        /// </summary>
        /// <param name="motivo"></param>
        public long AdicionarMotivoEliminacionGuia(ONMotivoElimGuiaMani motivo)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var motivoElim = contexto.paAdicionarGuiaManiElimHist_OPN(motivo.IdManifiestoOperacionNacional, motivo.NumeroManifiestoCarga, (short)motivo.IdTipoMotivoEliminacion, motivo.NombreTipoMotivoEliminacion, motivo.NumeroGuia, motivo.Observaciones, ControllerContext.Current.Usuario).FirstOrDefault();
                if (motivoElim != null && motivoElim.IdGuiaManiElimHist != null)
                {
                    return (long)motivoElim.IdGuiaManiElimHist.Value;
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_MOTIVO_ELIMINACION_GUIA_MANIFIESTO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_FALLO_CREACION_MOTIVO_ELIMINACION_GUIA_MANIFIESTO)));
            }
        }

        /// <summary>
        /// Verifica si una guia ya fue agregada a cualquier consolidado de un manifiesto
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        public bool VerificarExisteGuiaConsolidadoManifiesto(long numeroGuia, long idManifiestoOpeNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guiaConso = contexto.paObtenerGuiaConsoMani_OPN(numeroGuia, idManifiestoOpeNacional);
                if (guiaConso.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Verifica si una guia suelta ya fue agregada a un manifiesto
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        public bool VerificarExisteGuiaSueltaManifiesto(long numeroGuia, long idManifiestoOpeNacional)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var guiaSuelta = contexto.paObtenerGuiaSueltaMani_OPN(numeroGuia, idManifiestoOpeNacional);
                if (guiaSuelta.Count() > 0)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Verificar si una ciudad se encuentra dentro de las ciudades de una ruta (estaciones junto con la cobertura de la estacion y la ciudades adicionales y su cobertura)
        /// </summary>
        /// <param name="idLocalidadManifiesta"></param>
        /// <param name="idLocalidadDestinoGuia"></param>
        /// <param name="idRuta"></param>
        /// <returns>0=No esta en ruta, 1 = ok, 2= si ruta, no pertenece a localidadManifiesta ni area de influencia</returns>
        public int VerificarCiudadEnRuta(string idLocalidadManifiesta, string idLocalidadDestinoGuia, int idRuta)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ciudadRuta = contexto.paVerificarCiudadDentroRut_RUT(idLocalidadManifiesta, idLocalidadDestinoGuia, idRuta).FirstOrDefault();
                if (ciudadRuta != null && ciudadRuta.CiudadDentroRuta != null)
                {
                    return ciudadRuta.CiudadDentroRuta.Value;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Cierra un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void CerrarManifiesto(long idManifiesto, DateTime fechaSalida)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoOperacionNaciona_OPN manifiesto = contexto.ManifiestoOperacionNaciona_OPN.Where(m => m.MON_IdManifiestoOperacionNacio == idManifiesto).SingleOrDefault();
                if (manifiesto != null)
                {
                    manifiesto.MON_FechaSalidaSatrack = fechaSalida;
                    manifiesto.MON_EstaManifiestoCerrado = true;
                    if (manifiesto.MON_FechaCierre == manifiesto.MON_FechaGrabacion)
                        manifiesto.MON_FechaCierre = DateTime.Now;

                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Reabre un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void AbrirManifiesto(long idManifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManifiestoOperacionNaciona_OPN manifiesto = contexto.ManifiestoOperacionNaciona_OPN.Where(m => m.MON_IdManifiestoOperacionNacio == idManifiesto).SingleOrDefault();
                if (manifiesto != null)
                {
                    manifiesto.MON_EstaManifiestoCerrado = false;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Consulta las estaciones de una ruta y la cantidad de envíos manifestados en un manifiesto específico x cada estación
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public List<ONCantEnviosManXEstacionDC> ConsultarCantEnviosManXEstacion(int idRuta, long idManifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ONCantEnviosManXEstacionDC> cantEnviosRes = contexto.paObtenerCantEnviosManRuta_OPN(idManifiesto, idRuta).ToList().ConvertAll<ONCantEnviosManXEstacionDC>(
                  c =>
                    new ONCantEnviosManXEstacionDC()
                    {
                        CantidadEnvios = (int)c.Cantidad,
                        IdLocalidad = c.IdEstacion,
                        IdManifiesto = idManifiesto,
                        IdRuta = idRuta,
                        NombreLocalidad = c.NombreEstacion
                    }
                  );

                return cantEnviosRes;
            }
        }

        /// EMRL ajuste para solicitud de horarios de ruta
        /// <summary>
        /// Consulta la programacion de una ruta para los dias enviados
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="dias"></param>
        /// <returns></returns>
        public List<ONHorarioRutaDC> ConsultarHorariosRuta(int idRuta, string dias)
        {
            List<ONHorarioRutaDC> horariosRuta = new List<ONHorarioRutaDC>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("pa_FrecuenciaRuta_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRuta", idRuta);
                cmd.Parameters.AddWithValue("@Dias", dias);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlConn.Open();
                da.Fill(dt);
                sqlConn.Close();

                horariosRuta = dt.AsEnumerable().ToList().ConvertAll<ONHorarioRutaDC>(h =>
                    new ONHorarioRutaDC()
                    {
                        IdFrecuenciaRuta = h.Field<int>("FRR_IdFrecuenciaRuta"),
                        IdDia = h.Field<string>("FRR_IdDia"),
                        HoraSalida = h.Field<TimeSpan>("HoraSalida").ToString(),
                        HoraLlegada = h.Field<TimeSpan>("HoraLLegada").ToString(),
                        DescripcionDia = h.Field<string>("DescripcionDia"),
                        FechaSalida = h.Field<DateTime>("FechaSalida"),
                        FechaActual = CalcularFechaDia(h.Field<DateTime>("FechaSalida"), h.Field<string>("FRR_IdDia"))
                    });
            }

            return horariosRuta;
        }

        #region FechasCalculadas

        private DateTime CalcularFechaDia(DateTime horaSalida, string idDia)
        {

            switch (DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("es-ES")))
            {
                case "lunes":
                    return FechasLunes(idDia, horaSalida);
                case "martes":
                    return FechasMartes(idDia, horaSalida);
                case "miércoles":
                    return FechasMiercoles(idDia, horaSalida);
                case "jueves":
                    return FechasJueves(idDia, horaSalida);
                case "viernes":
                    return FechasViernes(idDia, horaSalida);
                case "sábado":
                    return FechasSabado(idDia, horaSalida);
                case "domingo":
                    return FechasDomingo(idDia, horaSalida);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasDomingo(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "6":
                    return horaSalida.AddDays(-1);
                case "7":
                    return horaSalida;
                case "1":
                    return horaSalida.AddDays(1);
                case "2":
                    return horaSalida.AddDays(2);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasSabado(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "5":
                    return horaSalida.AddDays(-1);
                case "6":
                    return horaSalida;
                case "7":
                    return horaSalida.AddDays(1);
                case "1":
                    return horaSalida.AddDays(2);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasViernes(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "4":
                    return horaSalida.AddDays(-1);
                case "5":
                    return horaSalida;
                case "6":
                    return horaSalida.AddDays(1);
                case "7":
                    return horaSalida.AddDays(2);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasJueves(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "3":
                    return horaSalida.AddDays(-1);
                case "4":
                    return horaSalida;
                case "5":
                    return horaSalida.AddDays(1);
                case "6":
                    return horaSalida.AddDays(2);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasMiercoles(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "2":
                    return horaSalida.AddDays(-1);
                case "3":
                    return horaSalida;
                case "4":
                    return horaSalida.AddDays(1);
                case "5":
                    return horaSalida.AddDays(2);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasMartes(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "1":
                    return horaSalida.AddDays(-1);
                case "2":
                    return horaSalida;
                case "3":
                    return horaSalida.AddDays(1);
                case "4":
                    return horaSalida.AddDays(2);
                default:
                    return horaSalida;
            }
        }

        private DateTime FechasLunes(string idDia, DateTime horaSalida)
        {
            switch (idDia)
            {
                case "1":
                    return horaSalida;
                case "2":
                    return horaSalida.AddDays(1);
                case "3":
                    return horaSalida.AddDays(2);
                case "7":
                    return horaSalida.AddDays(-1);
                default:
                    return horaSalida;
            }
        }

        #endregion

        /// <summary>
        /// Consulta las guías pendientes x manifestar de acuerdo a una localidad origen y a una ruta especifica.
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public List<ADGuia> ConsultarEnviosPendientesXManifestar(string idLocalidad, int idRuta)
        {
            //using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            List<ADGuia> EnviosPendientes = new List<ADGuia>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerPendientesXManifestar_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@localidad", idLocalidad);
                cmd.Parameters.AddWithValue("@IdRuta", idRuta);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlConn.Open();
                da.Fill(dt);
                sqlConn.Close();

                EnviosPendientes = dt.AsEnumerable().ToList().ConvertAll<ADGuia>(c =>
                    new ADGuia()
                    {

                        IdAdmision = c.Field<long>("ADM_IdAdminisionMensajeria"),
                        NumeroGuia = c.Field<long>("ADM_NumeroGuia"),
                        EstadoGuia = c.Field<ADEnumEstadoGuia>("EGT_IdEstadoGuia"),
                        IdCiudadDestino = c.Field<string>("ADM_IdCiudadDestino"),
                        NombreCiudadDestino = c.Field<string>("ADM_NombreCiudadDestino"),
                        FechaAdmision = c.Field<DateTime>("ADM_FechaAdmision"),
                        NombreServicio = c.Field<string>("Servicio"),
                        NombreTipoEnvio = c.Field<string>("ADM_NombreTipoEnvio"),
                        DiceContener = c.Field<string>("ADM_DiceContener"),

                    });

                //return contexto.paObtenerPendientesXManifestar_OPN(idLocalidad, idRuta).ToList().ConvertAll<ADGuia>(
                //  c =>
                //    new ADGuia()
                //    {
                //        IdAdmision = c.ADM_IdAdminisionMensajeria,
                //        NumeroGuia = c.ADM_NumeroGuia,
                //        EstadoGuia = (ADEnumEstadoGuia)c.EGT_IdEstadoGuia,
                //        IdCiudadDestino = c.ADM_IdCiudadDestino,
                //        NombreCiudadDestino = c.ADM_NombreCiudadDestino,
                //        FechaAdmision = c.ADM_FechaAdmision,
                //        NombreServicio = c.Servicio,
                //        NombreTipoEnvio = c.ADM_NombreTipoEnvio,
                //        DiceContener = c.ADM_DiceContener
                //    }
                //);
            }
            return EnviosPendientes;
        }

        /// <summary>
        /// Método para guardar el ingreso de una guía a la agencia
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public long GuardarIngresoGuiaAgencia(ONEnviosDescargueRutaDC guia, long idCentroServiciosIngreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return Convert.ToInt64(contexto.paCrearIngresoOperAgenGuia_OPN(guia.IdAdmisionMensajeria, guia.NumeroGuia, guia.PesoGuiaIngreso, (short)guia.PiezaActualRotulo, (short)guia.TotalPiezasRotulo, idCentroServiciosIngreso, DateTime.Now, ControllerContext.Current.Usuario).FirstOrDefault());
            }
        }

        /// <summary>
        /// Método para guardar la novedad de una guía capturada
        /// </summary>
        /// <param name="nov"></param>
        /// <param name="idIngresoGuia"></param>
        public void GuardarNovedadGuiaIngresada(OUNovedadIngresoDC nov, long idIngresoGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngresoNovAgenGuia_OPN(idIngresoGuia, nov.IdNovedad, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Método para guardar el ingreso de una guía no registrada a la agencia
        /// </summary>
        /// <param name="guia"></param>
        /// <returns></returns>
        public long GuardarIngresoGuiaNoAgencia(ONEnviosDescargueRutaDC guia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return Convert.ToInt64(contexto.paCrearIngresoOperaGuiaNoReg_OPN(guia.NumeroGuia, guia.PesoGuiaIngreso, guia.IdCentroServicioOrigen, ControllerContext.Current.Usuario).FirstOrDefault().Value);
            }
        }

        /// <summary>
        /// Método para guardar la novedad de una guía no capturada
        /// </summary>
        /// <param name="nov"></param>
        /// <param name="idIngresoGuia"></param>
        public void GuardarNovedadGuiaNoIngresada(OUNovedadIngresoDC nov, long idIngresoGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngresoOperaNovGuiaNoReg_OPN(idIngresoGuia, nov.IdNovedad, ControllerContext.Current.Usuario);
            }
        }

        public ONManifiestoGuia ConsultarUltimoManifiestoGuia(long idAdmision)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManfiestoOperaNacioGuia_OPN manifiestoGuiadb = contexto.ManfiestoOperaNacioGuia_OPN.Include("ManifiestoOperacionNaciona_OPN").Where(g => g.MOG_IdAdminisionMensajeria == idAdmision).OrderByDescending(o => o.MOG_FechaGrabacion).FirstOrDefault();

                ONManifiestoGuia manifiestoGuia = null;

                if (manifiestoGuiadb != null)
                {
                    manifiestoGuia = new ONManifiestoGuia()
                    {
                        IdAdmisionMensajeria = idAdmision,
                        IdLocalidadManifestada = manifiestoGuiadb.MOG_IdLocalidadManifestada,
                        NombreLocalidadManifestada = manifiestoGuiadb.MOG_NombreLocalidadManifestada,
                        NumeroGuia = manifiestoGuiadb.MOG_NumeroGuia,
                        IdLocalidadDespacho = manifiestoGuiadb.ManifiestoOperacionNaciona_OPN.MON_IdLocalidadDespacho,
                        NombreLocalidadDespacho = manifiestoGuiadb.ManifiestoOperacionNaciona_OPN.MON_NombreLocalidadDespacho
                    };
                }
                else
                {
                    ManfiestoOpeNacConDetalle_OPN manifiestoGuiaDetalledb = contexto.ManfiestoOpeNacConDetalle_OPN.Include("ManfiestoOperaNacioConsoli_OPN").Include("ManifiestoOperacionNaciona_OPN").Where(g => g.MOD_IdAdminisionMensajeria == idAdmision).OrderByDescending(o => o.MOD_FechaGrabacion).FirstOrDefault();

                    manifiestoGuia = new ONManifiestoGuia()
                    {
                        IdAdmisionMensajeria = idAdmision,
                        IdLocalidadManifestada = manifiestoGuiaDetalledb.ManfiestoOperaNacioConsoli_OPN.MOC_IdLocalidadManifestada,
                        NombreLocalidadManifestada = manifiestoGuiaDetalledb.ManfiestoOperaNacioConsoli_OPN.MOC_NombreLocalidadManifestada,
                        NumeroGuia = manifiestoGuiaDetalledb.MOD_NumeroGuia,
                        IdLocalidadDespacho = manifiestoGuiaDetalledb.ManfiestoOperaNacioConsoli_OPN.ManifiestoOperacionNaciona_OPN.MON_IdLocalidadDespacho,
                        NombreLocalidadDespacho = manifiestoGuiaDetalledb.ManfiestoOperaNacioConsoli_OPN.ManifiestoOperacionNaciona_OPN.MON_NombreLocalidadDespacho
                    };
                }
                return manifiestoGuia;
            }
        }


        // TODO:ID Grabar Novedades Estacion Ruta
        public long AdicionarNovedadEstacionRuta(ONNovedadEstacionRutaDC Novedad_EstRuta)
        {
            long newIdNovedadEstacionRuta = 0;
            //Se hace paso a ADO de como se graba la novedad
            // 1. Graba en la Tabla de Novedades (NovedadEstacionRuta_OPN)
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paInsertarNovedadEstacionRuta_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdManifiestoOperacionNacion", Novedad_EstRuta.IdManifiestoOpeNal);
                cmd.Parameters.AddWithValue("@IdLocalidad", Novedad_EstRuta.IdLocalidadEstacion);
                cmd.Parameters.AddWithValue("@IdTipoNovedadRuta", Novedad_EstRuta.IdTipoNovedad);
                cmd.Parameters.AddWithValue("@FechaNovedad", Novedad_EstRuta.FechaNovedad);
                cmd.Parameters.AddWithValue("@HorasNov", Novedad_EstRuta.HorasNovedad);
                cmd.Parameters.AddWithValue("@MinutosNov", Novedad_EstRuta.MinutosNovedad);
                cmd.Parameters.AddWithValue("@Observaciones", Novedad_EstRuta.Observaciones);
                cmd.Parameters.AddWithValue("@FechaGrabacion", DateTime.Now);
                cmd.Parameters.AddWithValue("@CreadoPor", Novedad_EstRuta.CreadoPor);
                cmd.Parameters.AddWithValue("@ClaseNovedad", Novedad_EstRuta.ClaseNovedad);
                cmd.Parameters.AddWithValue("@TipoAfectacion", Convert.ToInt32(Novedad_EstRuta.TipoAfectacion));
                cmd.Parameters.AddWithValue("@LugarIncidente", Novedad_EstRuta.LugarIncidente);
                cmd.Parameters.AddWithValue("@Retraso", Novedad_EstRuta.Retraso);
                if (Novedad_EstRuta.IdCentroServiciosCOL == 0)
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosCOL", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosCOL", Novedad_EstRuta.IdCentroServiciosCOL);
                }
                //1 Ciudad
                if (string.IsNullOrWhiteSpace(Novedad_EstRuta.IdCiudad))
                {
                    cmd.Parameters.AddWithValue("@IdCiudadAfectada", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCiudadAfectada", Novedad_EstRuta.IdCiudad);
                }
                if (Novedad_EstRuta.IdCentroServiciosPunto == 0)
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosPTO", null);

                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCentroServiciosPTO", Novedad_EstRuta.IdCentroServiciosPunto);
                }
                if (Novedad_EstRuta.IdMensajero == 0)
                {
                    cmd.Parameters.AddWithValue("@IdMensajero", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdMensajero", Novedad_EstRuta.IdMensajero);
                }
                if (Novedad_EstRuta.IdClienteCre == 0)
                {
                    cmd.Parameters.AddWithValue("@IdCliente", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdCliente", Novedad_EstRuta.IdClienteCre);
                }
                if (Novedad_EstRuta.IdContrato == 0)
                {
                    cmd.Parameters.AddWithValue("@IdContratoCliCre", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdContratoCliCre", Novedad_EstRuta.IdContrato);
                }
                if (Novedad_EstRuta.IdSucursalContrato == 0)
                {
                    cmd.Parameters.AddWithValue("@IdSucursalContrato", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IdSucursalContrato", Novedad_EstRuta.IdSucursalContrato);
                }
                newIdNovedadEstacionRuta = Convert.ToInt64(cmd.ExecuteScalar());
                sqlConn.Close();
            }

            /*
            NovedadEstacionRuta_OPN objNew = new NovedadEstacionRuta_OPN();

            objNew.NER_ClaseNovedad = Novedad_EstRuta.ClaseNovedad;
            objNew.NER_TipoAfectacion = (int)Novedad_EstRuta.TipoAfectacion;

            objNew.NER_IdManifiestoOperacionNacio = Novedad_EstRuta.IdManifiestoOpeNal;

            if (Novedad_EstRuta.IdCentroServiciosCOL == 0)
                objNew.NER_IdCentroServiciosCOL = null;
            else
                objNew.NER_IdCentroServiciosCOL = Novedad_EstRuta.IdCentroServiciosCOL;


            //1 Ciudad
            if (string.IsNullOrWhiteSpace(Novedad_EstRuta.IdCiudad))
                objNew.NER_IdCiudadAfectada = null;
            else
                objNew.NER_IdCiudadAfectada = Novedad_EstRuta.IdCiudad;


            //2  Punto
            if (Novedad_EstRuta.IdCentroServiciosPunto == 0)
                objNew.NER_IdCentroServiciosPTO = null;
            else
                objNew.NER_IdCentroServiciosPTO = Novedad_EstRuta.IdCentroServiciosPunto;


            //3 Mensajero
            if (Novedad_EstRuta.IdMensajero == 0)
                objNew.NER_IdMensajero = null;
            else
                objNew.NER_IdMensajero = Novedad_EstRuta.IdMensajero;


            //4 Cliente
            if (Novedad_EstRuta.IdClienteCre == 0)
                objNew.NER_IdCliente = null;
            else
                objNew.NER_IdCliente = Novedad_EstRuta.IdClienteCre;

            //4.2 Contrato
            if (Novedad_EstRuta.IdContrato == 0)
                objNew.NER_IdContratoCliCre = null;
            else
                objNew.NER_IdContratoCliCre = Novedad_EstRuta.IdContrato;

            //4.3 SucursalContrato
            if (Novedad_EstRuta.IdSucursalContrato == 0)
                objNew.NER_IdSucursalContrato = null;
            else
                objNew.NER_IdSucursalContrato = Novedad_EstRuta.IdSucursalContrato;



            objNew.NER_IdLocalidad = Novedad_EstRuta.IdLocalidadEstacion;

            objNew.NER_IdTipoNovedadRuta = Novedad_EstRuta.IdTipoNovedad;
            objNew.NER_FechaNovedad = Novedad_EstRuta.FechaNovedad;
            objNew.NER_HorasNov = Novedad_EstRuta.HorasNovedad;
            objNew.NER_MinutosNov = Novedad_EstRuta.MinutosNovedad;
            objNew.NER_Retraso = Novedad_EstRuta.Retraso;

            objNew.NER_LugarIncidente = Novedad_EstRuta.LugarIncidente;
            objNew.NER_Observaciones = Novedad_EstRuta.Observaciones;

            objNew.NER_FechaGrabacion = DateTime.Now;
            objNew.NER_CreadoPor = Novedad_EstRuta.CreadoPor;

            contexto.NovedadEstacionRuta_OPN.Add(objNew);
            contexto.SaveChanges();

            newIdNovedadEstacionRuta = objNew.NER_IdNovedadEstacionRuta;*/



            //// 2. Se Afectan las correspondientes Guias (NovedadesRuta_Guia_OPN) deacuerdo a la Novedad de Ruta recien creada
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paAfectarGuias_NovedadRuta_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@newIdNovedadEstacionRuta", newIdNovedadEstacionRuta);

                SqlDataReader read = cmd.ExecuteReader();

                if (Novedad_EstRuta.Retraso > 0)
                {
                    if (read.HasRows)
                    {
                        while (read.Read())
                        {
                            EGTipoNovedadGuia.CambiarFechaEntrega(new Servicios.ContratoDatos.Comun.COCambioFechaEntregaDC
                            {
                                Guia = new ADGuia { NumeroGuia = Convert.ToInt64(read["NumeroGuia"]) },
                                TiempoAfectacion = Novedad_EstRuta.Retraso * 24
                            });
                        }

                    }
                }
            }


            return newIdNovedadEstacionRuta;
        }


        public void ObtenerGuiasParaNotificarNovedadesRuta(List<long> LstNov)
        {

            long idNER_First = LstNov.FirstOrDefault();
            long idNER_Last = LstNov.Last();

            DataSet dsRes = new DataSet();
            SqlDataAdapter da;
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGuiasParaNotificacion_OPN", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@newIdNovedadEstacionRuta_Ini", idNER_First));
                cmd.Parameters.Add(new SqlParameter("@newIdNovedadEstacionRuta_Fin", idNER_Last));

                da = new SqlDataAdapter(cmd);
                da.Fill(dsRes);
            }


            if (dsRes.Tables[0].Rows.Count > 0) // Solo Notifica si hay Guias Afectadas por la Novedad
            {

                // 0 ----Datos de la Novedad
                string NombreNovedad = dsRes.Tables[1].Rows[0]["NombreNovedad"].ToString();
                DateTime FechaNovedad = Convert.ToDateTime(dsRes.Tables[1].Rows[0]["FechaNovedad"]);
                string DiasRetraso = dsRes.Tables[1].Rows[0]["DiasRetraso"].ToString();
                string Observaciones = dsRes.Tables[1].Rows[0]["Observaciones"].ToString();


                string rutaLocal = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                rutaLocal = new Uri(rutaLocal).LocalPath;

                string pMensaje;


                // 1 SMS - Clientes Contado/Alcobro
                #region SMS Clientes Contado/Alcobro
                DataTable dtGuiasClientesContado = dsRes.Tables[0].AsEnumerable().Where(g => g.Field<string>("TipoCliente") != "CONVENIO").CopyToDataTable();
                foreach (DataRow iteRow in dtGuiasClientesContado.Rows)
                {
                    string numeroCel = iteRow["NumeroCel"].ToString();
                    if (!MensajesTexto.Instancia.NumeroCelValido(numeroCel)) continue;
                    string NumeroGuia = iteRow["NumeroGuia"].ToString();
                    MensajesTexto.Instancia.EnviarMensajeTexto(numeroCel, "INTERRAPIDISIMO Informacion Guia " + NumeroGuia + " presenta retraso por fuerza mayor.");
                }
                #endregion



                // 2 email CLIENTES-CREDITO
                #region email CLIENTES-CREDITO

                ////////////////////////  
                pMensaje = "Apreciado Cliente: <br>";
                pMensaje += "Cuando usted realiza un despacho por INTER RAPID&Iacute;SIMO espera que sus env&iacute;os lleguen en el menor tiempo posible y nosotros nos esforzamos por eso.";
                pMensaje += " No obstante, en ocasiones se presentan novedades de fuerza mayor que podr&iacute;an impactar el cumplimiento en la entrega oportuna,";
                pMensaje += " como lo ocurrido con los siguientes env&iacute;os: (Archivo adjunto) <br><br>";
                pMensaje += "Novedad: " + NombreNovedad;
                pMensaje += ", Fecha y hora de la novedad: " + FechaNovedad.ToString() + "<br><br>";
                pMensaje += "Cualquier informaci&oacute;n comunicarse con la l&iacute;nea 5605000. <br><br>";
                pMensaje += "Atentamente <br>";
                pMensaje += "LOG&Iacute;STICA INTER RAPID&Iacute;SIMO S.A.";
                ////////////////////////////

                DataTable dtGuiasClientesCredito = dsRes.Tables[0].AsEnumerable().Where(g => g.Field<string>("TipoCliente") == "CONVENIO").CopyToDataTable();
                if (dtGuiasClientesCredito.Rows.Count > 0)
                {

                    // En el Grupo de Guias que afecto la Novedad, pueden existir varios Grupos de Clientes-Credito
                    DataTable dtClientesCredito = dtGuiasClientesCredito.AsEnumerable().GroupBy(g => g.Field<int>("IdCliente")).Select(s => s.First()).CopyToDataTable();
                    DataTable dtGuias;
                    string email_string;
                    foreach (DataRow iteCli in dtClientesCredito.Rows)
                    {

                        // Email - Cliente CREDITO
                        email_string = iteCli["Email"].ToString();

                        if (Email_Valido(email_string))
                        {
                            dtGuias = dtGuiasClientesCredito.AsEnumerable().Where(g => g.Field<int>("IdCliente") == Convert.ToInt32(iteCli["IdCliente"])).CopyToDataTable();
                            /////////////////////////////////////////////////////////////////////////////////////////////
                            //// Crea un Archivo de Texto con la informacion de los envios para el ADJUNTO
                            string RutaArchivo = rutaLocal + @"\DetalleEnvios.txt";
                            if (File.Exists(RutaArchivo)) File.Delete(RutaArchivo);
                            using (StreamWriter outfile = new StreamWriter(RutaArchivo))
                            {
                                outfile.WriteLine("NumeroGuia;CiudadDestino");
                                foreach (DataRow item in dtGuias.Rows)
                                    outfile.WriteLine(item["NumeroGuia"].ToString() + ";" + item["CiudadDestino"].ToString());

                                outfile.Flush();
                            }
                            /////////////////////////////////////////////////////////////////////////////////////////////


                            Enviar_Correo(email_string, RutaArchivo, pMensaje, "Informacion Envios");
                        }
                    }
                }
                #endregion



                // 3 email FUNCIONARIOS-INTERRAPIDISIMO
                #region email FUNCIONARIOS-INTERRAPIDISIMO

                DataTable dtFuncionarios = dsRes.Tables[2];
                if (dtFuncionarios.Rows.Count > 0)
                {
                    DataTable dtGuiasTodas = dsRes.Tables[0];
                    string RutaArchivoFuncionarios = rutaLocal + @"\DetalleEnviosFunc.txt";

                    #region Mensaje a Funcionarios de Interrapidisimo
                    pMensaje = "Se presenta Novedad de fuerza mayor que podr&iacute;a impactar el cumplimiento en la entrega oportuna";
                    pMensaje += " de los siguientes env&iacute;os: (Archivo adjunto) <br><br>";
                    pMensaje += " Datos de la Novedad. <br>";
                    pMensaje += "Novedad: " + NombreNovedad + ", Fecha y hora de la novedad: " + FechaNovedad.ToString() + "<br>";
                    pMensaje += "Dias de Retraso: " + DiasRetraso + "<br>";
                    pMensaje += "Descripci&oacute;n: " + Observaciones + "<br><br>";



                    pMensaje += "Atentamente <br>";
                    pMensaje += "LOG&Iacute;STICA INTER RAPID&Iacute;SIMO S.A.";
                    #endregion

                    // Enviar Correo a Funcionarios Interrapidisimo (con archivo resumen de las Guias afectadas)
                    //////// Crea un Archivo de Texto con la informacion de los envios para el ADJUNTO
                    if (File.Exists(RutaArchivoFuncionarios)) File.Delete(RutaArchivoFuncionarios);
                    using (StreamWriter outfile = new StreamWriter(RutaArchivoFuncionarios))
                    {
                        outfile.WriteLine("NumeroGuia;CiudadDestino");
                        foreach (DataRow item in dtGuiasTodas.Rows)
                            outfile.WriteLine(item["NumeroGuia"].ToString() + ";" + item["CiudadDestino"].ToString());

                        outfile.Flush();
                    }
                    //////////////////////////////////////////////////////////////////////////////////


                    string email_string;
                    foreach (DataRow iteFunc in dtFuncionarios.Rows)
                    {
                        // Email - Clientes CREDITO
                        email_string = iteFunc["Email"].ToString();
                        if (Email_Valido(email_string))
                        {
                            Enviar_Correo(email_string, RutaArchivoFuncionarios, pMensaje, "Informacion Envios");
                        }
                    }
                }
                #endregion



            } // FFIINN Solo Notifica si hay Guias Afectadas por la Novedad

        }




        private void Enviar_Correo(string emailDestino, string RutaArchivo, string pMensaje, string pAsunto)
        {
            CorreoElectronico objCorreos = CorreoElectronico.Instancia;
            SmtpClient clientSMTP = objCorreos.CrearSMTP();
            string emailRemitente = objCorreos.InformacionSMTP.Remitente;


            ////Ivancho
            //NetworkCredential credentials = new NetworkCredential("soporte@braintech.com.co", "brain2013", "");
            //SmtpClient clientSMTP = new SmtpClient("smtp.gmail.com", 587);
            //clientSMTP.EnableSsl = true;
            //clientSMTP.Credentials = credentials;
            //string emailRemitente = "soporte@braintech.com.co";


            MailMessage Mensaje = new MailMessage();
            Mensaje.To.Add(new MailAddress(emailDestino));
            Mensaje.From = new MailAddress(emailRemitente, "Interrapidisimo", System.Text.Encoding.UTF8);
            Mensaje.Subject = pAsunto;

            Attachment archi = new Attachment(RutaArchivo);
            Mensaje.Attachments.Add(archi);

            Mensaje.Body = pMensaje;
            Mensaje.IsBodyHtml = true;


            clientSMTP.Send(Mensaje);
            Mensaje.Dispose();
        }



        private Boolean Email_Valido(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        #region TrayectoCasillero

        /// <summary>
        /// Método para obtener los rangos de peso de los casilleros
        /// </summary>
        /// <returns></returns>
        public IList<ONRangoPesoCasilleroDC> ObtenerRangosPesoCasillero()
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.RangoPesoCasillero_OPN.ToList()
                  .ConvertAll<ONRangoPesoCasilleroDC>(c =>
                    new ONRangoPesoCasilleroDC()
                    {
                        IdRangoPeso = c.RPC_IdRangoPeso,
                        RangoInicial = c.RPC_RangoInicial,
                        RangoFinal = c.RPC_RangoFinal,
                        CreadoPor = c.RPC_CreadoPor,
                        FechaGrabacion = c.RPC_FechaGrabacion
                    });
            }
        }


        /// <summary>
        /// Método para obtener los trayectos de un origen 
        /// </summary>
        /// <returns></returns>
        public IList<ONTrayectoCasilleroPesoDC> ObtenerTrayectosCasilleroDestino(PALocalidadDC localidadOrigen)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

                return contexto.TrayectoCasilleroPeso_VOPN
                  .Where(tr => tr.TRC_IdLocalidadOrigen == localidadOrigen.IdLocalidad)
                  .ToList()
                  .ConvertAll<ONTrayectoCasilleroPesoDC>(c =>
                    new ONTrayectoCasilleroPesoDC()
                    {
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                        CreadoPor = c.TCP_CreadoPor,
                        FechaGrabacion = c.TCP_FechaGrabacion,
                        IdTrayectoCasilleroPeso = c.TCP_IdTrayectoCasilleroPeso,
                        IdCasillero = c.TCP_IdCasillero,
                        IdTrayectoCasillero = new ONTrayectoCasilleroDC
                        {
                            IdTrayectoCasillero = c.TRC_IdTrayectoCasillero,
                            LocalidadOrigen = new PALocalidadDC
                            {
                                IdLocalidad = c.TRC_IdLocalidadOrigen
                            },
                            LocalidadDestino = new PALocalidadDC
                            {
                                IdLocalidad = c.TRC_IdLocalidadDestino,
                                Nombre = c.LOC_Nombre,
                                IdAncestroPGrado = c.LOC_IdAncestroPrimerGrado,
                                IdAncestroSGrado = c.LOC_IdAncestroSegundoGrado,
                                IdAncestroTGrado = c.LOC_IdAncestroTercerGrado,
                                CodigoPostal = c.LOC_CodigoPostal
                            },
                        },
                        IdRangoPeso = new ONRangoPesoCasilleroDC { IdRangoPeso = c.TCP_IdRangoPeso },
                    });
            }
        }


        /// <summary>
        /// Método para adicionar un trayecto casillero
        /// </summary>
        /// <returns></returns>
        public ONTrayectoCasilleroPesoDC AdicionarTrayectoCasillero(ONTrayectoCasilleroPesoDC trayectoCasillero)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return trayectoCasillero;
            }
        }

        /// <summary>
        /// Método para eliminar un trayecto casillero
        /// </summary>
        /// <returns></returns>
        public void EliminarTrayectoCasillero(ONTrayectoCasilleroPesoDC trayectoCasillero)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

            }
        }


        /// <summary>
        /// Método para modificar un trayecto casillero
        /// </summary>
        /// <returns></returns>
        public void ModificarTrayectoCasillero(ONTrayectoCasilleroPesoDC trayectoCasillero)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {

            }
        }


        #endregion

        /// <summary>
        /// para obtener parametros guias en novedades de ruta cuando se requiera asignar novedad a guias especificas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="guiaxNovIndv"></param>
        /// <returns></returns>
        public List<ONParametrosGuiaInvNovedadRuta> ObtenerParametrosGuiaIndvPorNovedadRuta(long numeroGuia, ONEnumGuiaIndvNovedadRuta guiaxNovIndv, string tipoUbicacion)
        {
            List<ONParametrosGuiaInvNovedadRuta> lista = new List<ONParametrosGuiaInvNovedadRuta>();
            ONParametrosGuiaInvNovedadRuta parametrosGuiaNovedad = new ONParametrosGuiaInvNovedadRuta();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd;
                SqlDataReader reader;
                switch (guiaxNovIndv)
                {

                    case ONEnumGuiaIndvNovedadRuta.EstacionRuta:
                        cmd = new SqlCommand("paObtenerEstacionRutaPorGuia_OPN", sqlConn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            parametrosGuiaNovedad = new ONParametrosGuiaInvNovedadRuta()
                            {

                                IdEstacionRuta = Convert.ToInt32(reader["ESR_IdEstacionRuta"]),
                                NombreLocalidadEstacion = Convert.ToString(reader["ESR_NombreLocalidadEstacion"]),
                                IdCiudadDestino = Convert.ToString(reader["MOG_IdCiudadDestino"]),
                                IdCiudadOrigen = Convert.ToString(reader["MOG_IdCiudadOrigen"]),
                                IdCentroServicioDestino = Convert.ToInt64(reader["MOG_IdCentroServicioDestino"]),
                                IdCentroServicioOrigen = Convert.ToInt64(reader["MOG_IdCentroServicioOrigen"]),
                                IdMensajero = Convert.ToInt64(reader["ADM_IdMensajero"])
                            };
                            lista.Add(parametrosGuiaNovedad);
                        }
                        break;

                    case ONEnumGuiaIndvNovedadRuta.Mensajero:
                        cmd = new SqlCommand("paObtenerNovedadParaMensajeroPorGuia_OPN", sqlConn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            parametrosGuiaNovedad = new ONParametrosGuiaInvNovedadRuta()
                            {
                                NombreMensajero = Convert.ToString(reader["NombreMensajero"])
                            };
                            lista.Add(parametrosGuiaNovedad);
                        }
                        break;

                    case ONEnumGuiaIndvNovedadRuta.ClienteCredito:
                        cmd = new SqlCommand("paObtenerNovedadParaCliCredoPorGuia_OPN", sqlConn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tipo", tipoUbicacion);
                        cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            parametrosGuiaNovedad = new ONParametrosGuiaInvNovedadRuta()
                            {
                                NitConvenioRemitente = Convert.ToString(reader["nitConvenio"]),
                                RazonSocialConvenioRemitente = Convert.ToString(reader["RazonSocialConvenio"])
                            };
                            lista.Add(parametrosGuiaNovedad);
                        }
                        break;
                }

                sqlConn.Close();
            }
            return lista;
        }
        ///// <summary>
        ///// Metodo para Obtener informacion de las Novedades de Transportede de  la guia seleccionada
        ///// </summary>
        ///// <param name="OObtenerNovedadesTransporteGuia"></param>
        ///// <returns></returns>
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            List<ONNovedadesTransporteDC> Lista = new List<ONNovedadesTransporteDC>();

            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerNovedadesRutaGuia_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@NumeroGuia", numeroGuia));
                SqlDataReader read = cmd.ExecuteReader();

                while (read.Read())
                {
                    ONNovedadesTransporteDC newObjNovedadesTransporteDC = new ONNovedadesTransporteDC
                    {
                        IdManifiestoOperacionNacio = Convert.ToInt64(read["NER_IdManifiestoOperacionNacio"]),
                        NombreNovedad = read["NombreNovedad"].ToString(),
                        LugarIncidente = read["NombreCiudad"].ToString(),
                        Descripcion = read["NER_Observaciones"].ToString(),
                        FechaNovedad = Convert.ToDateTime(read["NER_FechaNovedad"]),
                        Tiempo = read["Tiempo"].ToString(),
                        FechaEstimadaEntrega = Convert.ToDateTime(read["ADM_FechaEstimadaEntregaNew"]),
                    };
                    Lista.Add(newObjNovedadesTransporteDC);
                }
            }

            return Lista;
        }

        // Se revisa si el Casillero es AEREO (Ciudades de la Costa en la Tabla TrayectoCasilleroServicio_OPN)
        public bool ValidarTrayectoServicioAereo(string idLocalidadOrigen, string idLocalidadDestino, int idServicio)
        {
            bool rta = false;
            try
            {
                using (SqlConnection clientConn = new SqlConnection(CadCnxController))
                {
                    string cmdText = @"SELECT * FROM TrayectoCasilleroServicio_OPN AS tcs 
	                                        INNER JOIN MunicipioCentroLogistico_PUA AS mclOri ON tcs.TCS_IdColOrigen = mclOri.MCL_IdCentroLogistico
	                                        INNER JOIN MunicipioCentroLogistico_PUA AS mclDes WITH(NOLOCK) ON tcs.TCS_IdColDestino = mclDes.MCL_IdCentroLogistico
                                        WHERE mclOri.MCL_IdLocalidad = @IdLocalidadOrigen 
	                                        AND mclDes.MCL_IdLocalidad = @IdLocalidadDestino 
	                                        AND tcs.TCS_IdServicio = @idServicio";
                    clientConn.Open();
                    SqlCommand cmd = new SqlCommand(cmdText, clientConn);
                    cmd.Parameters.AddWithValue("@IdLocalidadOrigen", idLocalidadOrigen);
                    cmd.Parameters.AddWithValue("@IdLocalidadDestino", idLocalidadDestino);
                    cmd.Parameters.AddWithValue("@idServicio", idServicio);
                    var reader = cmd.ExecuteReader();
                    return reader.HasRows;
                }

            }
            catch
            {
                rta = false;
            }

            return rta;

        }

    }

}