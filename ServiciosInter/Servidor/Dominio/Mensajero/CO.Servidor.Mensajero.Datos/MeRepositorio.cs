using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Mensajero;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Mensajero.Datos.Mapper;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;

namespace CO.Servidor.Mensajero.Datos
{
    public class MeRepositorio
    {
        private static readonly MeRepositorio instancia = new MeRepositorio();

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;
        public static MeRepositorio Instancia
        {
            get { return MeRepositorio.instancia; }
        }

        private MeRepositorio()
        { }

        public MEMensajero ConsultarMensajero(int idDocumento)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMensajerosNovasoft_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@identificacion", idDocumento);
                SqlDataReader reader;
                conn.Open();
                reader = cmd.ExecuteReader();

                MEMensajero resultado = new MEMensajero();


                while (reader.Read())
                {
                    resultado = new MEMensajero()
                    {
                        Nombre = reader["Nombres"] == DBNull.Value ? string.Empty : reader["Nombres"].ToString(),
                        //PersonaInterna = new OUPersonaInternaDC()
                        //{
                        //    IdCargo = reader["CAR_IdCargo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CAR_IdCargo"]),
                        //    Cargo = reader["CAR_Descripcion"] == DBNull.Value ? string.Empty : reader["CAR_Descripcion"].ToString(),
                        //    Direccion = reader["Direccion"] == DBNull.Value ? string.Empty : reader["Direccion"].ToString(),
                        //    Telefono = reader["Telefono"] == DBNull.Value ? string.Empty : reader["Telefono"].ToString(),
                        //    Email = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"].ToString(),
                        //    Municipio = reader["LOC_Nombre"] == DBNull.Value ? string.Empty : reader["LOC_Nombre"].ToString(),

                        //},
                        Celular = reader["Celular"] == DBNull.Value ? string.Empty : reader["Celular"].ToString(),
                        CiudadResidencia = reader["CiudadResidencia"] == DBNull.Value ? string.Empty : reader["CiudadResidencia"].ToString(),
                        IdAgencia = reader["AGE_IdAgencia"] == DBNull.Value ? string.Empty : reader["AGE_IdAgencia"].ToString(),
                        Agencia = new PUAgenciaDeRacolDC()
                        {
                            IdCentroServicio = reader["CES_IdCentroServicios"] == DBNull.Value ? 0 : Convert.ToInt64(reader["CES_IdCentroServicios"]),
                            NombreCentroServicio = reader["CES_Nombre"] == DBNull.Value ? string.Empty : reader["CES_Nombre"].ToString(),
                        },
                        IdVehiculo = reader["VEH_IdVehiculo"] == DBNull.Value ? 0 : Convert.ToInt32(reader["VEH_IdVehiculo"]),
                        Placa = reader["VEH_Placa"] == DBNull.Value ? string.Empty : reader["VEH_Placa"].ToString(),
                        TipoMensajero = reader["TIM_Descripcion"] == DBNull.Value ? string.Empty : reader["TIM_Descripcion"].ToString(),
                        FechaVenciminetoPase = reader["MEN_FechaVencimientoPase"] == DBNull.Value ? new DateTime() : Convert.ToDateTime(reader["MEN_FechaVencimientoPase"]),
                        EsMensajeroUrbano = reader["MEN_EsMensajeroUrbano"] == DBNull.Value ? false : Convert.ToBoolean(reader["MEN_EsMensajeroUrbano"]),
                        EsContratista = reader["MEN_EsContratista"] == DBNull.Value ? false : Convert.ToBoolean(reader["MEN_EsContratista"]),
                        NumeroPase = reader["MEN_NumeroPase"] == DBNull.Value ? string.Empty : reader["MEN_NumeroPase"].ToString(),
                        TipoVehiculo = reader["TIV_Descripcion"] == DBNull.Value ? string.Empty : reader["TIV_Descripcion"].ToString(),


                    };

                }

                conn.Close();

                return resultado;

            }
        }

        public int InsertarPersonaInterna(MEMensajero mensajero)
        {
            
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarPersonaInterna_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Identificacion", mensajero.PersonaInterna.Identificacion.Trim());
                cmd.Parameters.AddWithValue("@IdTipoIdentificacion", ConstantesFramework.TIPO_DOCUMENTO_CC);
                cmd.Parameters.AddWithValue("@Nombre", mensajero.PersonaInterna.Nombre);
                cmd.Parameters.AddWithValue("@Direccion", mensajero.PersonaInterna.Direccion);
                cmd.Parameters.AddWithValue("@Email", string.Empty);
                cmd.Parameters.AddWithValue("@Municipio", mensajero.LocalidadMensajero.IdLocalidad);
                cmd.Parameters.AddWithValue("@PrimerApellido", mensajero.PersonaInterna.PrimerApellido);
                cmd.Parameters.AddWithValue("@SegundoApellido", mensajero.PersonaInterna.SegundoApellido);
                cmd.Parameters.AddWithValue("@Telefono", mensajero.PersonaInterna.Telefono);
                cmd.Parameters.AddWithValue("@IdCargo", mensajero.CargoMensajero.IdCargo);
                cmd.Parameters.AddWithValue("@IdRegionalAdm", mensajero.PersonaInterna.Regional);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Comentarios", string.Empty);

                conn.Open();
                var resultado = cmd.ExecuteScalar();
                return Convert.ToInt32(resultado);
            }
           
        }

        public bool InsertarMensajeroVehiculo(POVehiculo vehiculo,long idMensajero)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarMensajeroVehiculo_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdVehiculo", vehiculo.IdVehiculo);
                cmd.Parameters.AddWithValue("@IdMensajero", idMensajero);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                resultado = cmd.ExecuteNonQuery();
             
            }
            return resultado > 0 ? true: false;

        }
     
        public int CrearMensajero(MEMensajero mensajero,long id)
        {
            var resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                DateTime fechaVencimiento = mensajero.FechaVenciminetoPase.Year < 2000 ? DateTime.Now : mensajero.FechaVenciminetoPase;
                SqlCommand cmd = new SqlCommand("paInsertarMensajero_CPO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPersonaInterna", id);
                cmd.Parameters.AddWithValue("@IdTipoMensajero", mensajero.TipMensajeros.IdTipoMensajero);
                cmd.Parameters.AddWithValue("@IdAgencia", mensajero.Agencia.IdCentroServicio);
                cmd.Parameters.AddWithValue("@Telefono2", mensajero.Telefono2);
                cmd.Parameters.AddWithValue("@FechaIngreso", mensajero.PersonaInterna.FechaInicioContrato);
                cmd.Parameters.AddWithValue("@FechaTerminacionContrato", mensajero.PersonaInterna.FechaTerminacionContrato);
                cmd.Parameters.AddWithValue("@FechaVencimientoPase", fechaVencimiento);
                cmd.Parameters.AddWithValue("@NumeroPase", mensajero.NumeroPase);
                cmd.Parameters.AddWithValue("@Estado", mensajero.Estado.IdEstado);
                cmd.Parameters.AddWithValue("@EsContratista", mensajero.EsContratista);
                cmd.Parameters.AddWithValue("@TipoContrato", mensajero.TipoContrato.IdTipoContrato);
                cmd.Parameters.AddWithValue("@EsMensajeroUrbano", mensajero.EsMensajeroUrbano);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                conn.Open();
                resultado = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }
            return resultado;
        }

        public List<MEMensajero> ObtenerPersonal(string idTipoUsuario)
        {
            List<MEMensajero> personal = new List<MEMensajero>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPersonalNovasoft_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoUsuario", idTipoUsuario);
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    personal = MERepositorioMapper.ToListMensajero(resultado);
                }
            }
            return personal;
        }

        public MEMensajero ObtenerDetalleEmpleadoNovasoft(string idDocumento,string compania)
        {
            MEMensajero personal = new MEMensajero();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleEmpleadoNovasoft_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroIdentificacion",idDocumento);
                cmd.Parameters.AddWithValue("@Compania", compania);
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    personal = MERepositorioMapper.ToListDetalleEmpleadoNovasoft(resultado);
                }
            }
            return personal;
        }

        public bool ObtenerPersonaInternaXId(string idDocumento)
        {
            
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPersonaInternaXId_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroIdentificacion", idDocumento);
                conn.Open();

                var resultado = cmd.ExecuteScalar();
                return Convert.ToInt32(resultado)>0 ? true : false;
            }

        }


        public MEMensajero ObtenerEmpleadoNovasoft(string idDocumento)
        {
            MEMensajero personal = new MEMensajero();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEmpleadoNovasoft_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroIdentificacion", idDocumento);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    personal = MERepositorioMapper.ToEmpleadoNovasoft(resultado);
                }
            }
            return personal;
        }


        #region Configuración Mensajero
        public List<MECargo> ObtenerCargos()
        {
            List<MECargo> cargos = new List<MECargo>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerCargosNovasoft_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    cargos = MERepositorioMapper.ToListCargo(resultado);
                }
            }
            return cargos;
        }

        public List<METipoContrato> ObtenerTiposContrato()
        {
            List<METipoContrato> tipoContratos = new List<METipoContrato>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTipoContrato_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tipoContratos = MERepositorioMapper.ToListTipoContrato(resultado);
                }

            }
            return tipoContratos;
        }

        public List<METipoMensajero> ObtenerTipoMensajero()
        {
            List<METipoMensajero> tipoMensajero = new List<METipoMensajero>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTipoMensajeroNofasoft_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tipoMensajero = MERepositorioMapper.ToListTipoMensajero(resultado);
                }
            }

            return tipoMensajero;

        }

        public List<MEEstadoMensajero> ObtenerEstadosMensajero()
        {
            List<MEEstadoMensajero> estadosMensajero = new List<MEEstadoMensajero>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerEstadosMensajero_SEG", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    estadosMensajero = MERepositorioMapper.ToListEstadoMensajero(resultado);
                }
            }
            return estadosMensajero;
        }

        public List<MEGruposLiquidacion> ObtenerGruposliquidacion(int pagina,int nRegistros, bool estado)
        {
            List<MEGruposLiquidacion> gruposLiquidacion = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGruposliquidacion_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@NRegistros", nRegistros);
                cmd.Parameters.AddWithValue("@Estado", estado);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    gruposLiquidacion = MERepositorioMapper.ToListGruposLiquidacion(resultado);
                }
            }
            return gruposLiquidacion;
        }

        public List<MEGruposLiquidacion> ObtenerGruposliquidacionUnico()
        {
            List<MEGruposLiquidacion> gruposLiquidacion = null;
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGruposliquidacionUnico_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    gruposLiquidacion = MERepositorioMapper.ToListGruposLiquidacionUnico(resultado);
                }
            }
            return gruposLiquidacion;
        }

        public List<MEGrupoBasico> ObtenerGruposBasicos(int pagina,int nRegistros, bool estado)
        {
            List<MEGrupoBasico> gruposBasicos = null;
            if (gruposBasicos == null)
            {
                gruposBasicos = new List<MEGrupoBasico>();
            }
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGruposBasicos_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@NRegistros", nRegistros);
                cmd.Parameters.AddWithValue("@Estado", estado);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    gruposBasicos = MERepositorioMapper.ToListGruposBasicos(resultado);
                }
            }
            return gruposBasicos;
        }

        public List<MEGrupoRodamiento> ObtenerGruposRodamiento(string idCiudad,int pagina,int nRegistros, bool estado)
        {
            List<MEGrupoRodamiento> gruposRodamiento = new List<MEGrupoRodamiento>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerGruposRodamiento_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                if (idCiudad == "null")
                { cmd.Parameters.AddWithValue("@idCiudad", DBNull.Value); }
                else { cmd.Parameters.AddWithValue("@idCiudad", idCiudad); }
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@NRegistros", nRegistros);
                cmd.Parameters.AddWithValue("@Estado", estado);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    gruposRodamiento = MERepositorioMapper.ToListGruposRodamiento(resultado);
                }
            }
            return gruposRodamiento;
        }

        public List<MEBasicoLiquidacion> ObtenerLiquidacionBasico(int idBasico)
        {
            List<MEBasicoLiquidacion> basicoLiquidacion = new List<MEBasicoLiquidacion>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerLiquidacionBasico_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idGrupoBasico", idBasico);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    basicoLiquidacion = MERepositorioMapper.ToListBasicoLiquidacion(resultado);
                }
            }
            return basicoLiquidacion;
        }

        public List<METipoTransporte> ObtenerTiposTransporte()
        {
            List<METipoTransporte> tiposTransporte = new List<METipoTransporte>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposTransporte_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposTransporte = MERepositorioMapper.ToListTipoTransporte(resultado);
                }
            }
            return tiposTransporte;
        }

        public int InsertarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            // int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarGrupoRodamiento_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoCiudad", GrupoRodamiento.IdCiudad);
                cmd.Parameters.AddWithValue("@IdZona", GrupoRodamiento.IdZona);
                cmd.Parameters.AddWithValue("@FechaInicial", GrupoRodamiento.FechaInicial);
                cmd.Parameters.AddWithValue("@FechaFinal", GrupoRodamiento.FechaFinal);
                cmd.Parameters.AddWithValue("@NombreRodamiento", GrupoRodamiento.RodamientoMensajero);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Estado", GrupoRodamiento.Estado);

                conn.Open();
                var resultado = cmd.ExecuteScalar();
                return Convert.ToInt32(resultado);
            }

        }

        public bool ModificarGrupoRodamiento(MEGrupoRodamiento GrupoRodamiento)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarGrupoRodamiento_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", GrupoRodamiento.Id);
                cmd.Parameters.AddWithValue("@IdCiudad", GrupoRodamiento.IdCiudad);
                cmd.Parameters.AddWithValue("@IdZona", GrupoRodamiento.IdZona);
                cmd.Parameters.AddWithValue("@FechaInicial", GrupoRodamiento.FechaInicial);
                cmd.Parameters.AddWithValue("@FechaFinal", GrupoRodamiento.FechaFinal);
                cmd.Parameters.AddWithValue("@NombreRodamiento", GrupoRodamiento.RodamientoMensajero);
                cmd.Parameters.AddWithValue("@Estado", GrupoRodamiento.Estado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                conn.Open();
                resultado = cmd.ExecuteNonQuery();

            }
            return resultado > 0 ? true : false;
        }

        public bool ModificarTipoRodamiento(METipoRodamiento TipoRodamiento)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarTipoRodamiento_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRodamiento", TipoRodamiento.IdGrupoRodamiento);
                cmd.Parameters.AddWithValue("@IdTipoTransporte", TipoRodamiento.IdTipoTransporte);
                cmd.Parameters.AddWithValue("@Valor", TipoRodamiento.Valor);
                cmd.Parameters.AddWithValue("@EstadoTpoRod", TipoRodamiento.EstadoTpoRod);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@MinimoVital", TipoRodamiento.MinimoVital);

                conn.Open();
                resultado = cmd.ExecuteNonQuery();
            }
            return resultado > 0 ? true : false;
        }


        public bool InsertarTipoRodamiento(METipoRodamiento TipoRodamiento)
        {            

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarTipoRodamiento_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRodamiento", TipoRodamiento.IdGrupoRodamiento);
                cmd.Parameters.AddWithValue("@IdTipoTransporte", TipoRodamiento.IdTipoTransporte);
                cmd.Parameters.AddWithValue("@Valor", TipoRodamiento.Valor);
                cmd.Parameters.AddWithValue("@EstadoTpoRod", TipoRodamiento.EstadoTpoRod);
                cmd.Parameters.AddWithValue("@MinimoVital", TipoRodamiento.MinimoVital);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
          
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            return true;
        }

        public int InsertarBasico(MEGrupoBasico GrupoBasico)
        {
            //int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarGrupoBasico_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GrupoBasico", GrupoBasico.GrupoBasico);
                cmd.Parameters.AddWithValue("@ValorInicial", GrupoBasico.ValorInicial);
                cmd.Parameters.AddWithValue("@ValorFinal", GrupoBasico.ValorFinal);
                cmd.Parameters.AddWithValue("@NumeroCuotas", GrupoBasico.NumeroCuotas);
                cmd.Parameters.AddWithValue("@FechaInicial", GrupoBasico.FechaFinal);
                cmd.Parameters.AddWithValue("@FechaFinal", GrupoBasico.FechaFinal);
                cmd.Parameters.AddWithValue("@Estado", GrupoBasico.Estado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                conn.Open();
                var resultado = cmd.ExecuteScalar();

                return Convert.ToInt32(resultado);
            }
        }

        public bool InsertarBasicoLiquidacion(MEBasicoLiquidacion BasicoLiq)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarBasicoLiquidacion_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoBasico", BasicoLiq.IdGrupoBasico);
                cmd.Parameters.AddWithValue("@Mes", BasicoLiq.Mes);
                cmd.Parameters.AddWithValue("@Valor", BasicoLiq.Valor);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                conn.Open();
                resultado = cmd.ExecuteNonQuery();
            }
            return resultado > 0 ? true : false;
        }



        public bool ModificarBasico(MEGrupoBasico GrupoBasico)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarGrupoBasico_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GrupoBasico", GrupoBasico.GrupoBasico);
                cmd.Parameters.AddWithValue("@ValorInicial", GrupoBasico.ValorInicial);
                cmd.Parameters.AddWithValue("@ValorFinal", GrupoBasico.ValorFinal);
                cmd.Parameters.AddWithValue("@NumeroCuotas", GrupoBasico.NumeroCuotas);
                cmd.Parameters.AddWithValue("@FechaInicial", GrupoBasico.FechaFinal);
                cmd.Parameters.AddWithValue("@FechaFinal", GrupoBasico.FechaFinal);
                cmd.Parameters.AddWithValue("@Estado", GrupoBasico.Estado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdGrupo", GrupoBasico.IdGrupoBasico);

                conn.Open();
                resultado = cmd.ExecuteNonQuery();

            }
            return resultado > 0 ? true : false;
        }

        public bool ModificarBasicoLiquidacion(MEBasicoLiquidacion BasicoLiq)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarBasicoLiquidacion_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoBasico", BasicoLiq.IdGrupoBasico);
                cmd.Parameters.AddWithValue("@Mes", BasicoLiq.Mes);
                cmd.Parameters.AddWithValue("@Valor", BasicoLiq.Valor);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                conn.Open();
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado > 0 ? true : false;
        }
        public bool EliminarBasicoLiquidacion(long IdGrupoBasico, int cuotaMax)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paEliminarBasicoLiquidacion_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoBasico", IdGrupoBasico);
                cmd.Parameters.AddWithValue("@CuotaMax", cuotaMax);

                conn.Open();
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado > 0 ? true : false;
        }

        public List<METipoAccion> ObtenerTiposAccion()
        {
            List<METipoAccion> tiposAccion = new List<METipoAccion>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposAccion_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposAccion = MERepositorioMapper.ToListTipoAccion(resultado);
                }

            }
            return tiposAccion;
        }

        public List<METipoPenalidad> ObtenerTiposPenalidad(int pagina,int nRegistros, bool estado)
        {
            List<METipoPenalidad> tiposPenalidades = new List<METipoPenalidad>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposPenalidades_PEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@NRegistros", nRegistros);
                cmd.Parameters.AddWithValue("@Estado", estado);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposPenalidades = MERepositorioMapper.ToListTipoPenalidad(resultado);
                }
            }
            return tiposPenalidades;
        }

        public List<METipoPenalidad> ObtenerTiposPenalidadesConfig()
        {
            List<METipoPenalidad> tiposPenalidadesRaps = new List<METipoPenalidad>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposPenalidadesRaps_PEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposPenalidadesRaps = MERepositorioMapper.ToListTipoPenalidadesConfig(resultado);
                }
            }
            return tiposPenalidadesRaps;
        }

        public List<METipoPenalidad> ObtenerTiposPenalidadesRaps()
        {
            List<METipoPenalidad> tiposPenalidadesRaps = new List<METipoPenalidad>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposPenalidadesRaps_RAP", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposPenalidadesRaps = MERepositorioMapper.ToListTipoPenalidadesRaps(resultado);
                }
            }
            return tiposPenalidadesRaps;
        }

        public List<METipoUsuario> ObtenerTiposUsuarios()
        {
            List<METipoUsuario> tiposUsuario = new List<METipoUsuario>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposUsuarios_PEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposUsuario = MERepositorioMapper.ToListTipoUsuario(resultado);
                }
            }
            return tiposUsuario;
        }

        public List<METipoRodamiento> ObtenerTiposRodamientoXGrupo(int idGrupoRodamiento)
        {
            List<METipoRodamiento> tiposRodamiento = new List<METipoRodamiento>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerRodamientoTipoXGrupo_ROD", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdRodamiento", idGrupoRodamiento);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposRodamiento = MERepositorioMapper.ToListTipoRodamiento(resultado);
                }
            }
            return tiposRodamiento;
        }

        public List<METiposLiquidacion> ObtenerTiposLiquidacion(int idGrupoLiquidacion, int pagina, int nRegistros)
        {
            List<METiposLiquidacion> tiposLiquidacion = new List<METiposLiquidacion>();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposLiquidacion_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoLiq", idGrupoLiquidacion);
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@NRegistros", nRegistros);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tiposLiquidacion = MERepositorioMapper.ToListTipoLiquidacion(resultado);
                }
            }
            return tiposLiquidacion;
        }

        public bool ConsultarTipoLiquidacion(long idTipoLiq,long idGrupo)
        {
            int resultado = 0;
            using(SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarExisteTipoLiquidacion_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoLiq", idTipoLiq);
                cmd.Parameters.AddWithValue("@IdGrupoLiq", idGrupo);
                conn.Open();

                resultado = (int)cmd.ExecuteScalar();

                
            }
            return resultado>0 ? true :false;
        }

        public int InsertarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarGrupoLiquidacion_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@GrupoLiq", GrupoLiquidacion.GrupoLiq);
                cmd.Parameters.AddWithValue("@FechaInicio", GrupoLiquidacion.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", GrupoLiquidacion.FechaFinal);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Estado", GrupoLiquidacion.Estado);
                conn.Open();


                var resultado = cmd.ExecuteScalar();

                return Convert.ToInt32(resultado);

            }

        }


        public bool InsertarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarTipoLiquidacion_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoLiq", TipoLiquidacion.IdGrupoLiq);
                //cmd.Parameters.AddWithValue("@IdTipoLiq", TipoLiquidacion.IdTipoLiq);
                cmd.Parameters.AddWithValue("@IdUnidad", TipoLiquidacion.IdUnidad);
                cmd.Parameters.AddWithValue("@IdTipoAccion", TipoLiquidacion.IdTipoAccion);
                cmd.Parameters.AddWithValue("@IdFormaPago", TipoLiquidacion.IdFormaPago);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@ValorPorcentual", TipoLiquidacion.ValorPorcentual);
                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }
            return resultado > 0 ? true : false;
        }

        public bool ModificarTipoLiquidacion(METiposLiquidacion TipoLiquidacion)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarTipoLiquidacion_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoLiq", TipoLiquidacion.IdGrupoLiq);
                cmd.Parameters.AddWithValue("@IdTipoLiq", TipoLiquidacion.IdTipoLiq);
                cmd.Parameters.AddWithValue("@IdUnidad", TipoLiquidacion.IdUnidad);
                cmd.Parameters.AddWithValue("@ValorPorcentual", TipoLiquidacion.ValorPorcentual);
                cmd.Parameters.AddWithValue("@IdTipoAccion", TipoLiquidacion.IdTipoAccion);
                cmd.Parameters.AddWithValue("@IdFormaPago", TipoLiquidacion.IdFormaPago);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }
            return resultado > 0 ? true : false;
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocioFormaPago(string idTipoAccion, int pagina, int nRegistros)
        {
            List<MEUnidadNegocioFormaPago> unidadNegocioFormaPago = new List<MEUnidadNegocioFormaPago>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarUnidadesFormaPago_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoAccion", idTipoAccion);
                cmd.Parameters.AddWithValue("@Pagina", pagina);
                cmd.Parameters.AddWithValue("@NRegistros", nRegistros);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    unidadNegocioFormaPago = MERepositorioMapper.ToListUnidadNegocioFormaPago(resultado);
                }

            }
            return unidadNegocioFormaPago;
        }

        public List<MEUnidadNegocioFormaPago> ObtenerUnidadesNegocio()
        {
            List<MEUnidadNegocioFormaPago> unidadNegocio = new List<MEUnidadNegocioFormaPago>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerUnidadNegocio_TAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    unidadNegocio = MERepositorioMapper.ToListUnidadNegocio(resultado);
                }

            }
            return unidadNegocio;
        }
        public List<MEUnidadNegocioFormaPago> ObtenerFormaPago(int IdTipoAccion)
        {
            List<MEUnidadNegocioFormaPago> formaPago = new List<MEUnidadNegocioFormaPago>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerFormasPago_TAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoAccion", IdTipoAccion);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    formaPago = MERepositorioMapper.ToListFormaPago(resultado);
                }

            }
            return formaPago;
        }


        public bool ModificarGrupoLiquidacion(MEGruposLiquidacion GrupoLiquidacion)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarGrupoLiquidacion_PAR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoLiq", GrupoLiquidacion.IdGrupoLiq);
                cmd.Parameters.AddWithValue("@FechaInicio", GrupoLiquidacion.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFinal", GrupoLiquidacion.FechaFinal);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario); 
                cmd.Parameters.AddWithValue("@Estado", GrupoLiquidacion.Estado);
                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }
            return resultado > 0 ? true : false;
        }

        public List<METipoCuenta> ObtenerTipoCuenta()
        {
            List<METipoCuenta> tipoCuenta = new List<METipoCuenta>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposCuenta_PEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tipoCuenta = MERepositorioMapper.ToListTipoCuenta(resultado);
                }

            }
            return tipoCuenta;
        }

        public bool InsertarPenalidad(METipoPenalidad TipoPenalidad)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarTiposPenalidad_PEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Penalidad", TipoPenalidad.Penalidad);
                cmd.Parameters.AddWithValue("@IdTipoUsuario", TipoPenalidad.IdTipoUsuario);
                cmd.Parameters.AddWithValue("@ValorPorcentual", TipoPenalidad.ValorPorcentual);
                cmd.Parameters.AddWithValue("@Porcentaje", TipoPenalidad.Porcentaje);
                cmd.Parameters.AddWithValue("@IdTipoCuenta", TipoPenalidad.TipoCuenta.IdTipoCuenta);
                cmd.Parameters.AddWithValue("@FechaInicio", TipoPenalidad.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", TipoPenalidad.FechaFin);
                cmd.Parameters.AddWithValue("@IdPenalidadRaps", TipoPenalidad.IdPenalidadRaps);
                cmd.Parameters.AddWithValue("@IdParametroRaps", TipoPenalidad.IdParametroRaps);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Estado", TipoPenalidad.Estado);

                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }
            return resultado > 0 ? true : false;
        }

        public bool ModificarPenalidad(METipoPenalidad TipoPenalidad)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarTiposPenalidad_PEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPenalidad", TipoPenalidad.IdPenalidad);
                cmd.Parameters.AddWithValue("@Penalidad", TipoPenalidad.Penalidad);
                cmd.Parameters.AddWithValue("@IdTipoUsuario", TipoPenalidad.IdTipoUsuario);
                cmd.Parameters.AddWithValue("@ValorPorcentual", TipoPenalidad.ValorPorcentual);
                cmd.Parameters.AddWithValue("@Porcentaje", TipoPenalidad.Porcentaje);
                cmd.Parameters.AddWithValue("@IdTipoCuenta", TipoPenalidad.TipoCuenta.IdTipoCuenta);
                cmd.Parameters.AddWithValue("@FechaInicio", TipoPenalidad.FechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", TipoPenalidad.FechaFin);
                cmd.Parameters.AddWithValue("@Estado", TipoPenalidad.Estado);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }

            return resultado > 0 ? true : false;
            #endregion
        }

        public List<POVehiculo> ConsultaVehiculosNovasoft()
        {
            List<POVehiculo> vehiculos = new List<POVehiculo>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerVehiculosNovasoft_VEH", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    vehiculos = MERepositorioMapper.ToListVehiculo(resultado);
                }

            }
            return vehiculos;
        }

        public List<POVehiculo> ConsultaVehiculosNovasoftXDocumento(string idDocumento)
        {
            List<POVehiculo> vehiculos = new List<POVehiculo>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerVehiculosNovasoftXIdDocumento_VEH", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NumeroIdentificacion", idDocumento);
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    vehiculos = MERepositorioMapper.ToListVehiculo(resultado);
                }

            }
            return vehiculos;
        }

        public List<METipoPAM> ObtenerTipoPAM()
        {
            List<METipoPAM> tipoPam = new List<METipoPAM>();

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTipoPam_PAM", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                conn.Open();

                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    tipoPam = MERepositorioMapper.ToListTipoPAM(resultado);
                }

            }
            return tipoPam;
        }

        public bool InsertarModificarLiquidacionPersonaInterna(long idPersona, string valor, string tipo)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarActualizarLiquidacionPersonaInterna_CPO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@IdPersonaInterna", idPersona);

                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }

            return resultado > 0 ? true : false;
        }

        public bool InsertarModificarLiquidacionPersonaExterna(long idPersona, string valor, string tipo)
        {
            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarActualizarLiquidacionPersonaExterna_CPO", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Tipo", tipo);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@IdPersonaInterna", idPersona);

                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }

            return resultado > 0 ? true : false;
        }

        public double ConsultarIPC(DateTime fecha)
        {
            double resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarIPC_NOV", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha",fecha);

                conn.Open();

                var res =  cmd.ExecuteScalar();
                resultado = Convert.ToDouble(res);
            }

            return resultado;
        }

        public double ConsultarActivo(string tabla, string campo,DateTime fechaInicio)
        {
            double resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paConsultarActivo_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tabla",tabla);
                cmd.Parameters.AddWithValue("@campo", campo);
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);

                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }

            return resultado;
        }

        public MEConfigComisiones ConsultarConfiguracionComisionesEmpleado(int idPersona, string tabla, string campo)
        {
            MEConfigComisiones configComisiones = new MEConfigComisiones();
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConfiguracionComisiones_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPersona", idPersona);
                cmd.Parameters.AddWithValue("@tabla", tabla);
                cmd.Parameters.AddWithValue("@campo", campo);
                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();

                if (resultado.HasRows)
                {
                    configComisiones = MERepositorioMapper.ToListConfigComisiones(resultado);
                }
            }
            return configComisiones;
        }


        public bool ModificarComisionesIPC(double IPC)
        {

            int resultado = 0;

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paModificarComisionesIPC_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ipc", IPC);

                conn.Open();

                resultado = cmd.ExecuteNonQuery();

            }

            return resultado > 0 ? true:false;
        }

        public List<MEMensajero> ObtenerPersonasConfig()
        {
            List<MEMensajero> personal = new List<MEMensajero>();

            using(SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerPersonaConfig_MEN", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                conn.Open();
                SqlDataReader resultado = cmd.ExecuteReader();
                if(resultado.HasRows)
                {
                    personal = MERepositorioMapper.ToListDetalleEmpleadoNovasoftConfig(resultado);
                }

            }
            return personal;
        }

    }
}