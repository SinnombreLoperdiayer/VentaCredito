using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace CO.Servidor.Raps.Datos
{
    public partial class RARepositorio
    {

        #region MotorEscalonamiento

        /// <summary>
        /// Obtiene los horarios del empleado para el cual se realizara el escalamiento de un rap
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idSucursal"></param>
        public RACargoEscalarDC ObtenerHorariosEmpleadoEscalarPorCargoSucursal(RACargoEscalarDC cargoEscalar)
        {
            using (SqlConnection sqlConn = new SqlConnection(conexionStringNovasoft))
            {
                SqlCommand cmd = new SqlCommand("paObtenerHorarioEmpleadoNovasoft_Raps", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@documentoEmpl", cargoEscalar.DocumentoEmpleado);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlConn.Open();
                da.Fill(dt);
                sqlConn.Close();


                RACargoEscalarDC cargoEmpleado = new RACargoEscalarDC();

                var lstEmpleados = dt.AsEnumerable().ToList();

                if (lstEmpleados.Count > 0)
                {
                    var empleado = lstEmpleados.GroupBy(r => r.Field<string>("Documento")).Select(s => s.First()).ToList().FirstOrDefault();

                    cargoEmpleado.DocumentoEmpleado = empleado.Field<string>("Documento").Trim();
                    cargoEmpleado.NombreEmpleado = empleado.Field<string>("nombre");
                    cargoEmpleado.Sucursal = empleado.Field<string>("IdSucursal");
                    cargoEmpleado.Correo = empleado.Field<string>("Correo");

                    cargoEmpleado.HorarioEmpleado = new List<RAHorarioEmpleadoDC>();
                    lstEmpleados.Where(e => e.Field<string>("Documento").Trim() == cargoEmpleado.DocumentoEmpleado.Trim()).ToList().ForEach(emp =>
                    {

                        cargoEmpleado.HorarioEmpleado.Add(
                            new RAHorarioEmpleadoDC() { IdDia = emp.Field<int>("diaSemana"), HoraEntrada = DateTime.Parse(emp["HoraEntrada"].ToString()), HoraSalida = DateTime.Parse(emp["HoraSalida"].ToString()) }
                            );
                    });
                }
                return cargoEmpleado;

            }
        }

        /// <summary>
        /// Obtiene los datos del correo y id de la persona que esta en ese escalonamiento, con el fin de traer el cargo indicado
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idSucursalEscalar"></param>
        public RACargoEscalarDC ObtenerDatosDeescalonamientoPorCargoYSucursal(string idCargo, string idSucursalEscalar)
        {
            RACargoEscalarDC escalar = new RACargoEscalarDC();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultarEscalonamientoCorrectoPorSucursal_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Idcargo", idCargo);
                cmd.Parameters.AddWithValue("@CodigoSucursal", idSucursalEscalar);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    escalar.Correo = reader["e_mail_alt"].ToString();
                    escalar.DocumentoEmpleado = reader["COD_EMP"].ToString();
                }
                conn.Close();
            }
            return escalar;
        }

        /// <summary>
        /// Obtiene todas las Solicitudes vencidas junto con el escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RASolicitudDC> ObtenerSolicitudesVencidasEscalonamiento()
        {
            List<DataRow> lstSolicitudes = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerSolicitudesVencidasEscalonamientoV3_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlConn.Open();
                da.Fill(dt);
                sqlConn.Close();
                lstSolicitudes = dt.AsEnumerable().ToList();

            }
            List<RASolicitudDC> solicitudesVencidas = new List<RASolicitudDC>();


            if (lstSolicitudes.Count > 0)
            {

                var solicitudes = lstSolicitudes.GroupBy(r => r.Field<long>("IdSolicitud")).Select(s => s.First()).ToList();

                List<Task> lstTareas = new List<Task>();

                solicitudes.ForEach(solic =>
                {
                    lstTareas.Add(Task.Factory.StartNew(() =>
                    {
                        using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
                        {

                            RASolicitudDC soli = new RASolicitudDC()
                            {

                                Descripcion = solic.Field<string>("DescripcionSolicitud"),
                                DocumentoResponsable = solic.Field<string>("DocumentoResponsable"),
                                DocumentoSolicita = solic.Field<string>("DocumentoSolicita"),
                                FechaVencimiento = solic.Field<DateTime>("FechaVencimiento"),
                                IdCargoResponsable = solic.Field<string>("IdCargoResponsable"),
                                IdCargoSolicita = solic.Field<string>("IdCargoSolicita"),
                                IdParametrizacionRap = solic.Field<long>("IdParametrizacionRap"),
                                IdSolicitud = solic.Field<long>("IdSolicitud"),
                                IdSolicitudPadre = solic.Field<long>("IdSolicitudPadre"),

                                Escalonamiento = new List<RAEscalonamientoDC>(),
                                Cargo = new RACargoDC()
                                {
                                    CargoNovasoft = solic.Field<string>("IdCargoResSolicitud"),
                                    CorreoCorporativo = solic.Field<string>("CorreoCorporativoResSolicitud"),
                                    Descripcion = solic.Field<string>("DescripcionCargoResSolicitud"),
                                    IdCargo = solic.Field<string>("IdCargoResSolicitud"),
                                    CodSucursal = solic.Field<string>("CodigoSucursalSolicitud")
                                }
                            };
                            var escalonamiento = lstSolicitudes.Where(s => s.Field<long>("IdSolicitud") == soli.IdSolicitud).OrderBy(s => s.Field<int>("OrdenEscalonamiento")).ToList();


                            //agrupa por cargo y extrae el escalonamiento completo del rap
                            ///verifica si el cargo tiene varios empleados, y toma el que tenga menos solicitudes asignadas abiertas                          
                            if (escalonamiento != null && escalonamiento.Count > 0)
                            {

                                soli.Escalonamiento = new List<RAEscalonamientoDC>();

                                foreach (var esca in escalonamiento)
                                {
                                    Dictionary<string, int> cantidades = new Dictionary<string, int>();
                                    DataTable dtPersonaCargo = new DataTable();
                                    using (SqlConnection sqlConnNova = new SqlConnection(conexionStringNovasoft))
                                    {
                                        //busca las personas asociadas al cargo en la sucursal, si no existe en la sucursal (regional) la busca en casa matriz (201)
                                        SqlCommand cmd = new SqlCommand("paBuscarPersonaCargoSucursalNovasoft_RAP", sqlConnNova);
                                        cmd.CommandTimeout = 60;
                                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@CodSucursal", esca.Field<string>("CodigoSucursalSolicitud").Trim());
                                        cmd.Parameters.AddWithValue("@CodCargo", esca.Field<string>("IdCargoEscalonamiento").Trim());
                                        cmd.Parameters.AddWithValue("@IdProceso", esca["IdProcesoEscalonamiento"] != DBNull.Value ? esca.Field<string>("IdProcesoEscalonamiento").Trim() : "");
                                        cmd.Parameters.AddWithValue("@IdProcedimiento", esca["IdProcedimientoEscalonamiento"] != DBNull.Value ? esca.Field<string>("IdProcedimientoEscalonamiento").Trim() : "");
                                        cmd.Parameters.AddWithValue("@CodCasaMatriz", ObtenerCodigoCasaMatriz());

                                        sqlConnNova.Open();
                                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                                        da.Fill(dtPersonaCargo);
                                        sqlConnNova.Close();
                                    }
                                    if (dtPersonaCargo.AsEnumerable().Count() > 0)
                                    {

                                        //Busca las asignaciones por persona
                                        dtPersonaCargo.AsEnumerable().ToList().ForEach(pCargo =>
                                        {
                                            SqlCommand cmd = new SqlCommand("paVerificarCantidadAsignacionesEmpleado_RAP", sqlConn);
                                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                            cmd.CommandTimeout = 120;
                                            cmd.Parameters.AddWithValue("@documentoEmpleado", pCargo.Field<string>("Cod_emp").Trim());
                                            sqlConn.Open();
                                            int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                                            sqlConn.Close();
                                            cantidades.Add(pCargo.Field<string>("Cod_emp").Trim(), cantidad);
                                        });
                                        string documentoAsignar = "", emailAsignar = "", codSucursalAsignar = "", codPlantaAsignar = "";
                                        documentoAsignar = cantidades.OrderBy(c => c.Value).First().Key;

                                        if (documentoAsignar != soli.DocumentoResponsable)
                                        {

                                            var personaAsig = dtPersonaCargo.AsEnumerable().ToList().Where(p => p.Field<string>("Cod_emp") == documentoAsignar).First();
                                            emailAsignar = personaAsig.Field<string>("e_mail_alt");
                                            codSucursalAsignar = personaAsig.Field<string>("cod_suc");
                                            codPlantaAsignar = personaAsig.Field<string>("codPlantaNova");

                                            var dataEscalonaSeleccionado = lstSolicitudes.Where(s => s.Field<long>("IdSolicitud") == soli.IdSolicitud && s.Field<string>("IdCargoEscalonamiento").Trim() == esca.Field<string>("IdCargoEscalonamiento").Trim()).First();
                                            RAEscalonamientoDC escalona = new RAEscalonamientoDC()
                                            {
                                                CorreoEscalar = emailAsignar,
                                                idCargo = esca.Field<string>("IdCargoEscalonamiento").Trim(),
                                                IdParametrizacionRap = solic.Field<long>("IdParametrizacionRap"),
                                                IdSucursalEscalar = codSucursalAsignar,
                                                IdTipoHora = dataEscalonaSeleccionado.Field<int>("IdTipoHoraEscalonamiento"),
                                                Orden = dataEscalonaSeleccionado.Field<int>("OrdenEscalonamiento"),
                                                HorasEscalar = dataEscalonaSeleccionado.Field<byte>("HorasEscalar"),
                                                CargoEscalar = new RACargoEscalarDC()
                                                {
                                                    Correo = emailAsignar,
                                                    DocumentoEmpleado = documentoAsignar,
                                                    IdCargoController = esca.Field<string>("IdCargoEscalonamiento").Trim(),
                                                    CodPlantaNovasoft = codPlantaAsignar,
                                                    IdCargoNovasoft = esca.Field<string>("IdCargoEscalonamiento").Trim(),
                                                    IdCiudad = personaAsig.Field<string>("cod_ciu"),
                                                    NombreEmpleado = personaAsig.Field<string>("NombreEmpleado"),
                                                    Sucursal = codSucursalAsignar,
                                                    HorarioEmpleado = new List<RAHorarioEmpleadoDC>()
                                                }
                                            };

                                            SqlCommand cmd = new SqlCommand("paObtenerHorarioEmpleadoNovasoft_Raps", sqlConn);
                                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@documentoEmpl", documentoAsignar);
                                            sqlConn.Open();
                                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                                            DataTable dthorarioempleado = new DataTable();
                                            da.Fill(dthorarioempleado);
                                            sqlConn.Close();

                                            dthorarioempleado.AsEnumerable().ToList().ForEach(hora =>
                                            {
                                                escalona.CargoEscalar.HorarioEmpleado.Add(
                                                                                    new RAHorarioEmpleadoDC()
                                                                                    {
                                                                                        IdDia = hora.Field<int>("diaSemana"),
                                                                                        HoraEntrada = DateTime.Parse(hora["HoraEntrada"].ToString()),
                                                                                        HoraSalida = DateTime.Parse(hora["HoraSalida"].ToString())
                                                                                    }
                                                                                     );
                                            });

                                            soli.Escalonamiento.Add(escalona);
                                            break;
                                        }
                                    }
                                }

                            }

                            solicitudesVencidas.Add(soli);

                        }
                    }));
                });
                Task.WaitAll(lstTareas.ToArray());
            }

            return solicitudesVencidas;

        }


        public string ObtenerCodigoCasaMatriz()
        {
            string codigoCasaMatriz = "";

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCodigoCasaMatriz_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                var resultado = cmd.ExecuteScalar();
                conn.Close();
                if (resultado != null)
                {
                    codigoCasaMatriz = resultado.ToString();
                }
            };

            return codigoCasaMatriz;
        }


        /// <summary>
        /// Obtiene todas las gestiones realizadas por un cargo de una solicitud
        /// </summary>
        /// <param name="idCargo"></param>
        /// <returns></returns>
        public List<RAGestionDC> ObtenerGestionesSolicitudPorCargo(long idSolicitud, int idCargoGestiona)
        {

            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGestionesSolicitudCargo_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@IdCargoGestiona", idCargoGestiona);
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sqlConn.Open();
                da.Fill(dt);
                sqlConn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<RAGestionDC>(gestion =>
                {
                    RAGestionDC ges = new RAGestionDC()
                    {
                        IdGestion = gestion.Field<long>("IdGestion"),
                        DocumentoResponsable = gestion.Field<string>("DocumentoResponsable"),
                        IdCargoGestiona = gestion.Field<string>("IdCargoGestiona")
                    };

                    return ges;
                });
            }
        }

        /// <summary>
        /// Obtiene las parametrizaciones de raps automaticos listos para crear solicitudes
        /// </summary>
        public List<RASolicitudDC> ObtenerRapsAutomaticosGenerarSolicitudes()
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrizacionesAutomaticasMotorRaps", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                var lstDt = dt.AsEnumerable().ToList();

                List<RASolicitudDC> lstSolicitudes = lstDt.GroupBy(r => r.Field<long>("IdParametrizacionRap")).Select(s => s.First())
                      .ToList().ConvertAll<RASolicitudDC>(s =>
                      {
                          RASolicitudDC solicitud = new RASolicitudDC()
                          {

                              IdParametrizacionRap = s.Field<long>("IdParametrizacionRap"),
                              Descripcion = s.Field<string>("Nombre")
                          };

                          var escalonamiento = lstDt.Where(l => l.Field<long>("IdParametrizacionRap") == s.Field<long>("IdParametrizacionRap")).ToList();

                          ///verifica si el cargo tiene varios empleados, y toma el que tenga menos solicitudes asignadas abiertas
                          var escalonamientoDuplicados = escalonamiento.GroupBy(e => e.Field<int>("idCargo")).Select(e => e.Select(x => x).ToList()).FirstOrDefault();
                          if (escalonamientoDuplicados != null && escalonamiento.Count > 1)
                          {
                              Dictionary<string, int> cantidades = new Dictionary<string, int>();
                              escalonamientoDuplicados.ForEach(esca =>
                              {

                                  cmd = new SqlCommand("paVerificarCantidadAsignacionesEmpleado_RAP", conn);
                                  cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                  cmd.CommandTimeout = 120;
                                  cmd.Parameters.AddWithValue("@documentoEmpleado", esca.Field<string>("cedulaEmpleadoEscalonamiento").Trim());
                                  conn.Open();
                                  int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                                  conn.Close();

                                  cantidades.Add(esca.Field<string>("cedulaEmpleadoEscalonamiento").Trim(), cantidad);
                              });

                              string documentoAsignar = cantidades.OrderBy(c => c.Value).First().Key;

                              var escalonamietoQuitar = escalonamiento.Where(e => e.Field<int>("idCargo") == escalonamientoDuplicados.Where(esca => esca.Field<string>("cedulaEmpleadoEscalonamiento") == esca.Field<string>("cedulaEmpleadoEscalonamiento")).First().Field<int>("idCargo")).ToList();

                              for (int i = escalonamietoQuitar.Count - 1; i >= 0; i--)
                              {
                                  if (escalonamietoQuitar[i].Field<string>("cedulaEmpleadoEscalonamiento").Trim() != documentoAsignar)
                                  {
                                      escalonamiento.Remove(escalonamietoQuitar[i]);
                                  }
                              }


                          }


                          solicitud.Escalonamiento = escalonamiento

                                               .ToList().ConvertAll<RAEscalonamientoDC>(e =>
                                               {
                                                   RAEscalonamientoDC escalona = new RAEscalonamientoDC()
                                                   {
                                                       CorreoEscalar = e["CorreoCorporativoEscalonamiento"] != DBNull.Value ? e["CorreoCorporativoEscalonamiento"].ToString() : "",
                                                       idCargo = e.Field<string>("idCargo"),
                                                       IdParametrizacionRap = e.Field<long>("IdParametrizacionRap"),
                                                       IdSucursalEscalar = e.Field<string>("CodigoSucursal"),
                                                       IdTipoHora = e.Field<int>("IdTipoHoraEscalona"),
                                                       Orden = e.Field<int>("Orden"),
                                                       HorasEscalar = e.Field<byte>("HorasEscalar")
                                                   };

                                                   return escalona;
                                               });


                          return solicitud;

                      });


                return lstSolicitudes;

            }
        }


        /// <summary>
        /// Obtiene las parametrizaciones de raps automaticos con sistema fuente listos para crear solicitudes
        /// </summary>
        public List<RASolicitudDC> ObtenerRapsAutomaticosConSistemaFuenteGenerarSolicitudes()
        {
            DataTable dt = new DataTable();
            Dictionary<string, DataTable> dicPersonasCargo = new Dictionary<string, DataTable>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrizacionesAutomaticasAcumulativasMotorRapsV3_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 600;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                conn.Open();
                da.Fill(dt);
                conn.Close();
            }
            var lstDt = dt.AsEnumerable().ToList();

            List<Task> lstTareas = new List<Task>();

            List<RASolicitudDC> lstSolicitudes = new List<RASolicitudDC>();

            var lstSolicitudesAgrupadas = lstDt;
            bool error = false;
            lstSolicitudesAgrupadas.ForEach(s =>
            {
                // lstTareas.Add(Task.Factory.StartNew(() =>
                //  {

                try
                {
                    using (SqlConnection conn = new SqlConnection(conexionStringRaps))
                    {
                        conn.Open();
                        RASolicitudDC solicitud = new RASolicitudDC()
                        {

                            IdParametrizacionRap = s.Field<long>("IdParametrizacionRap"),
                            Descripcion = s.Field<string>("SOA_DescripcionSolicitud"),
                            IdSolicitud = s.Field<long>("SOA_IdSolicitudAcumulativa"),
                            IdParametrizacionRapPadre = s.Field<long>("IdParametrizacionPadre"),
                            NombreParametrizacionRapPadre = s["NombreRapPadre"] != DBNull.Value ? s.Field<string>("NombreRapPadre") : "",
                            idSucursal = s.Field<string>("CodigoSucursal").Trim()
                        };


                        SqlCommand cmd = new SqlCommand("paObtenerEscalonamientoParametrizacionRapsSinInfoNova_Raps", conn);
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdParametrizacionRap", s.Field<long>("IdParametrizacionRap"));
                        cmd.Parameters.AddWithValue("@CodSucursal", s.Field<string>("CodigoSucursal").Trim());

                        //  conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dtEscalona = new DataTable();
                        da.Fill(dtEscalona);
                        // conn.Close();

                        var escalonamiento = dtEscalona.AsEnumerable().ToList();

                        int cont = 1;

                        //agrupa por cargo y extrae el escalonamiento completo del rap
                        ///verifica si el cargo tiene varios empleados, y toma el que tenga menos solicitudes asignadas abiertas                          
                        if (escalonamiento != null && escalonamiento.Count >= 1)
                        {

                            solicitud.Escalonamiento = new List<RAEscalonamientoDC>();

                            foreach (var esca in escalonamiento)
                            {
                                Dictionary<string, int> cantidades = new Dictionary<string, int>();
                                DataTable dtPersonaCargo = new DataTable();
                                string llaveDiccionario = solicitud.idSucursal.ToString() + "-" + esca.Field<string>("IdCargo").Trim();

                                lock (this)
                                {
                                    //Cache personas asociadas al cargo. Se verifica si la sucursal con el cargo ya fue consultado para reutilizarlo
                                    if (dicPersonasCargo.ContainsKey(llaveDiccionario))
                                    {
                                        dtPersonaCargo = dicPersonasCargo[llaveDiccionario];
                                    }
                                    else
                                    {
                                        using (SqlConnection connNova = new SqlConnection(conexionStringNovasoft))
                                        {
                                            //busca las personas asociadas al cargo en la sucursal, si no existe en la sucursal (regional) la busca en casa matriz (201)
                                            cmd = new SqlCommand("paBuscarPersonaCargoSucursalNovasoft_RAP", connNova);
                                            cmd.CommandTimeout = 60;
                                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@CodSucursal", solicitud.idSucursal.ToString());
                                            cmd.Parameters.AddWithValue("@CodCargo", esca.Field<string>("IdCargo").Trim());
                                            cmd.Parameters.AddWithValue("@IdProceso", esca["IdProceso"] != DBNull.Value ? esca.Field<string>("IdProceso").Trim() : "");
                                            cmd.Parameters.AddWithValue("@IdProcedimiento", esca["IdProcedimiento"] != DBNull.Value ? esca.Field<string>("IdProcedimiento").Trim() : "");
                                            cmd.Parameters.AddWithValue("@CodCasaMatriz", ObtenerCodigoCasaMatriz());
                                            connNova.Open();
                                            da = new SqlDataAdapter(cmd);
                                            da.Fill(dtPersonaCargo);
                                            connNova.Close();
                                        }
                                        dicPersonasCargo.Add(llaveDiccionario, dtPersonaCargo);
                                    }
                                }

                                if (dtPersonaCargo.AsEnumerable().Count() > 0)
                                {

                                    //Busca las asignaciones por persona
                                    dtPersonaCargo.AsEnumerable().ToList().ForEach(pCargo =>
                                    {
                                        cmd = new SqlCommand("paVerificarCantidadAsignacionesEmpleado_RAP", conn);
                                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                        cmd.CommandTimeout = 600;
                                        cmd.Parameters.AddWithValue("@documentoEmpleado", pCargo.Field<string>("Cod_emp").Trim());
                                        //  conn.Open();
                                        int cantidad = Convert.ToInt32(cmd.ExecuteScalar());
                                        //  conn.Close();
                                        cantidades.Add(pCargo.Field<string>("Cod_emp").Trim(), cantidad);
                                    });
                                    string documentoAsignar = "", emailAsignar = "", codSucursalAsignar = "", codPlantaAsignar = "";
                                    documentoAsignar = cantidades.OrderBy(c => c.Value).First().Key;
                                    var personaAsig = dtPersonaCargo.AsEnumerable().ToList().Where(p => p.Field<string>("Cod_emp") == documentoAsignar).First();
                                    emailAsignar = personaAsig.Field<string>("e_mail_alt");
                                    codSucursalAsignar = personaAsig.Field<string>("cod_suc");
                                    codPlantaAsignar = personaAsig.Field<string>("codPlantaNova");


                                    RAEscalonamientoDC escalona = new RAEscalonamientoDC()
                                    {
                                        CorreoEscalar = emailAsignar,
                                        idCargo = esca.Field<string>("idCargo").Trim(),
                                        IdParametrizacionRap = s.Field<long>("IdParametrizacionRap"),
                                        IdSucursalEscalar = codSucursalAsignar,
                                        IdTipoHora = esca.Field<int>("IdTipoHora"),
                                        Orden = esca.Field<int>("Orden"),
                                        HorasEscalar = esca.Field<byte>("HorasEscalar"),
                                        IdTipoEscalonamiento = esca.Field<int>("IdTipoEscalonamiento"),
                                        CargoEscalar = new RACargoEscalarDC()
                                        {
                                            Correo = emailAsignar,
                                            DocumentoEmpleado = documentoAsignar,
                                            IdCargoController = esca.Field<string>("idCargo").Trim(),
                                            CodPlantaNovasoft = codPlantaAsignar,
                                            IdCargoNovasoft = esca.Field<string>("idCargo").Trim(),
                                            IdCiudad = personaAsig.Field<string>("cod_ciu"),
                                            NombreEmpleado = personaAsig.Field<string>("NombreEmpleado"),
                                            Sucursal = codSucursalAsignar,
                                            HorarioEmpleado = new List<RAHorarioEmpleadoDC>()
                                        }
                                    };

                                    cmd = new SqlCommand("paObtenerHorarioEmpleadoNovasoft_Raps", conn);
                                    cmd.CommandTimeout = 600;
                                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@documentoEmpl", documentoAsignar);
                                    //  conn.Open();
                                    da = new SqlDataAdapter(cmd);
                                    DataTable dthorarioempleado = new DataTable();
                                    da.Fill(dthorarioempleado);
                                    //  conn.Close();
                                    int i = 0;
                                    dthorarioempleado.AsEnumerable().ToList().ForEach(hora =>
                                    {
                                        escalona.CargoEscalar.HorarioEmpleado.Add(
                                                                            new RAHorarioEmpleadoDC()
                                                                            {
                                                                                IdDia = hora.Field<int>("diaSemana"),
                                                                                HoraEntrada = DateTime.Parse(hora["HoraEntrada"].ToString()).AddDays(i),
                                                                                HoraSalida = DateTime.Parse(hora["HoraSalida"].ToString()).AddDays(i)
                                                                            }
                                                                             );
                                        i++;
                                    });
                                    if (cont == 2)
                                    {
                                        break;
                                    }
                                    cont++;

                                    solicitud.Escalonamiento.Add(escalona);
                                    break;
                                }
                            }
                        }

                        //Consulta los paramettros de la solicitudacumulativa
                        cmd = new SqlCommand("paObtenerParametrosSolAcumulativa_Raps", conn);
                        cmd.CommandTimeout = 600;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PSA_IdSolicitudAcumulativa", solicitud.IdSolicitud);
                        // conn.Open();
                        da = new SqlDataAdapter(cmd);
                        DataTable dtParametros = new DataTable();
                        da.Fill(dtParametros);
                        //  conn.Close();

                        var parametros = dtParametros.AsEnumerable().ToList();

                        if (parametros != null)
                        {
                            solicitud.ParametrosSolicitud = parametros.ToList().ConvertAll<RAParametrosParametrizacionDC>(p =>
                            {
                                RAParametrosParametrizacionDC parametro = new RAParametrosParametrizacionDC()
                                {
                                    idParametro = p.Field<int>("PSA_IdParametroParametrizacion"),
                                    descripcionParametro = p.Field<string>("DescripcionParametro"),
                                    Valor = p.Field<string>("PSA_Valor"),
                                    idTipoDato = p.Field<int>("idTipoDato"),
                                    EsAgrupamiento = p.Field<bool>("EsAgrupamiento"),
                                    EsEncabezadoDescripcion = p["EsEncabezadoDescripcion"] != DBNull.Value ? p.Field<bool>("EsEncabezadoDescripcion") : false,
                                    IdSolicitud = solicitud.IdSolicitud,
                                    DescripcionSolicitud = solicitud.Descripcion,
                                    idParametrizacionRap = solicitud.IdParametrizacionRap
                                };

                                return parametro;

                            });
                        }
                        if (solicitud != null)
                        {
                            lstSolicitudes.Add(solicitud);
                        }
                        conn.Close();
                        GC.Collect();
                    }
                }
                catch (Exception ex)
                {
                    error = true;
                    Utilidades.AuditarExcepcion(ex, true);
                }

                // }));
            });

            GC.Collect();
            // Task.WaitAll(lstTareas.ToArray());
            GC.Collect();
            if (error)
                throw new Exception("Ocurrieron errores en el mapeo de la consulta de solicitudesAcumulativas");

            return lstSolicitudes;


        }



        /// <summary>
        /// Inserta la ultima ejecucion para un rap automatico 
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        public void InsertarUltimaEjecucionRapAutomatico(long idParametrizacionRap, string usuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarEjecucionRapsAutomaticos_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ERA_IdParametrizacionRap", idParametrizacionRap);
                cmd.Parameters.AddWithValue("@ERA_CreadoPor", usuario);
                conn.Open();
                cmd.ExecuteNonQuery();

            }

        }

        /// <summary>
        /// Actualiza una solicitud acumulativa con la solicitud creada
        /// </summary>
        /// <param name="idSolicitudAcumulativa"></param>
        /// <param name="idSolicitudCreada"></param>
        public void ActualizarSolicitudAcumulativaConSolicitud(long idSolicitudAcumulativa, long idSolicitudCreada)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paActualizaSolicitudAcumulaticaConSolicitud_RAPS", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SOA_IdSolicitudAcumulativa", idSolicitudAcumulativa);
                cmd.Parameters.AddWithValue("@SOA_IdSolicitudCreada", idSolicitudCreada);
                conn.Open();
                cmd.ExecuteNonQuery();

            }

        }

        /// <summary>
        /// Obtiene los tipos de escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoEscalonamientoDC> ObtenerTiposEscalonamiento()
        {
            List<RATipoEscalonamientoDC> lstEscalonamiento = new List<RATipoEscalonamientoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposEscalonamiento_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RATipoEscalonamientoDC tipoEscalonamiento = new RATipoEscalonamientoDC()
                    {
                        IdTipoEscalonamiento = Convert.ToInt32(reader["TEC_IdTipoEscalonamiento"]),
                        Descripcion = reader["TEC_Descripcion"].ToString()
                    };
                    lstEscalonamiento.Add(tipoEscalonamiento);
                }
                conn.Close();
            }
            return lstEscalonamiento;
        }


        /// <summary>
        /// Obtiene las solicitudes que tienen como fecha de vencimiento hoy y/o mañana
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<RASolicitudDC> ObtenerSolicitudesProximasAVencer(string idUsuario)
        {
            List<RASolicitudDC> lstSolicitudes = new List<RASolicitudDC>();
            using (SqlConnection conn = new SqlConnection())
            {
                SqlCommand cmd = new SqlCommand("paObtenerSolicitudesAVencer_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocumentoResponsable", idUsuario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RASolicitudDC solicitud = new RASolicitudDC
                    {
                        IdSolicitud = Convert.ToInt32(reader["IdSolicitud"]),
                        IdCargoResponsable = reader["IdCargoResponsable"].ToString()
                    };
                    lstSolicitudes.Add(solicitud);
                }
                conn.Close();
            }
            return lstSolicitudes;
        }


        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosFramework(string llave)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrosFramework_PAR", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PAR_IdParametro", llave);
                conn.Open();
                var parametro = cmd.ExecuteScalar();
                conn.Close();

                if (parametro != null)
                {
                    return Convert.ToString(parametro);
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_RAPS,
                                        ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO.ToString(),
                                         MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_NO_EXISTE_PARAMETRO_CONFIGURADO));
                    throw new FaultException<ControllerException>(excepcion);
                }

            }

        }

        /// <summary>
        /// Obtiene el listado de días festivos que hay entre una fecha y otra
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="fechahasta">Fecha hasta la cua se hará la consulta</param>
        /// <returns></returns>
        public List<DateTime> ObtenerFestivos(DateTime fechadesde, DateTime fechahasta, string idPais)
        {
            List<DateTime> Festivos = new List<DateTime>();
            if (Cache.Instancia.ContainsFestivos())
            {
                Festivos = Cache.Instancia.GetFestivos();
            }
            else
            {
                using (SqlConnection cnn = new SqlConnection(conexionStringController))
                {
                    SqlCommand cmd = new SqlCommand("paObtenerCalendarioFestivos", cnn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CAL_IdPais", idPais);
                    cmd.Parameters.AddWithValue("@fechadesde", fechadesde);
                    cmd.Parameters.AddWithValue("@fechahasta", fechahasta);
                    cnn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Festivos.Add(Convert.ToDateTime(reader["CAL_Fecha"]).Date);
                    }

                    cnn.Close();

                    Cache.Instancia.AddFestivos(Festivos);
                }
            }

            return Festivos;
        }

        /// <summary>
        /// Obtiene el comentario de una gestion automatica de una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public string ObtenerComentarioDeGestion(int idSolicitud)
        {
            string comentario = "";
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerComentarioGestionAutomatica_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 60;
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                conn.Open();
                var result = cmd.ExecuteScalar();
                conn.Close();
                if (result != null)
                {
                    comentario = result.ToString();
                }
            }
            return comentario;
        }

        /// <summary>
        /// Obtiene los niveles de falla para los mensajeros
        /// </summary>
        /// <returns></returns>
        public List<RANivelFallaDC> ObtenerNivelesDeFalla()
        {
            List<RANivelFallaDC> lsNiveles = new List<RANivelFallaDC>();

            using (SqlConnection cnn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNivelesFalla_RAP", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RANivelFallaDC nivel = new RANivelFallaDC
                    {
                        IdNivelFalla = Convert.ToInt32(reader["NFM_IdNivel"]),
                        Descripcion = reader["NFM_Descripcion"].ToString()
                    };
                    lsNiveles.Add(nivel);
                }

                cnn.Close();
            }

            return lsNiveles;
        }

        /// <summary>
        /// Obtiene los niveles de falla para los mensajeros
        /// </summary>
        /// <returns></returns>
        public List<RAResponsableTipoNovedadDC> ObtenerTodoTipoNovedadDisponible()
        {
            List<RAResponsableTipoNovedadDC> responsables = new List<RAResponsableTipoNovedadDC>();

            using (SqlConnection cnn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultarTodoTipoNovedadDisponible_RAP", cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RAResponsableTipoNovedadDC responsable = new RAResponsableTipoNovedadDC
                    {
                        Id = reader["RTI_Id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RTI_Id"]),
                        Nombre = reader["TPN_DescripcionTipoNovedad"] == DBNull.Value ? "" : reader["TPN_DescripcionTipoNovedad"].ToString(),
                        IdOrigenRaps = reader["RTI_IdOrigenRaps"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RTI_IdOrigenRaps"]),
                        IdTipoNovedadHijo = reader["TPN_IdTipoNovedad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TPN_IdTipoNovedad"]),
                        EstadoOrigen = reader["EstadoOrigen"] == DBNull.Value ? 0 : Convert.ToInt32(reader["EstadoOrigen"])
                    };
                    responsables.Add(responsable);
                }


                cnn.Close();
            }

            return responsables;
        }

        /// <summary>
        /// Metodo para obtener parametros integracion por tipo novedad 
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        public List<LIParametrizacionIntegracionRAPSDC> ObtenerParametrosIntegracionPorTipoNovedad(int idTipoNovedad)
        {
            List<LIParametrizacionIntegracionRAPSDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosIntegracionRapsporTipoNovedad_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idTipoNovedad", idTipoNovedad));

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    resultado = new List<LIParametrizacionIntegracionRAPSDC>();
                    while (reader.Read())
                    {
                        resultado.Add(new LIParametrizacionIntegracionRAPSDC()
                        {
                            IdParametro = reader["idparametro"] == DBNull.Value ? 0 : Convert.ToInt32(reader["idparametro"]),
                            Longitud = reader["Longitud"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Longitud"]),
                            DescripcionParametro = reader["DescripcionParametro"] == DBNull.Value ? "" : reader["DescripcionParametro"].ToString(),
                            TipoDato = reader["IdTipoDato"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdTipoDato"])
                        });
                    }
                }
            }
            return resultado;
        }
        /// <summary>
        /// Obtener parametros fallas personalizadas
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public List<RAParametrosPersonalizacionRapsDC> ListaParametrosPersonalizacionPorNovedad(int idTipoNovedad)
        {
            List<RAParametrosPersonalizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosNovedadPersonalizada_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoNovedad", idTipoNovedad));

                var reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    resultado = new List<RAParametrosPersonalizacionRapsDC>();
                    while (reader.Read())
                    {
                        resultado.Add(new RAParametrosPersonalizacionRapsDC()
                        {
                            IdTipoNovedad = reader["IdTipoNovedad"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdTipoNovedad"]),
                            IdParametro = reader["IdParametro"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdParametro"]),
                            IdFuncion = reader["IdFuncion"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdFuncion"]),
                        });
                    }
                }
            }
            return resultado;
        }

        #endregion


    }
}
