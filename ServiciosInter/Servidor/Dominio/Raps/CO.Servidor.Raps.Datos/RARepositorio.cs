using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
//using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CO.Servidor.Raps.Datos
{
    public partial class RARepositorio : ControllerBase
    {
        private string conexionStringRaps = ConfigurationManager.ConnectionStrings["rapsTransaccional"].ConnectionString;
        private string conexionStringNovasoft = ConfigurationManager.ConnectionStrings["novasoftTransaccional"].ConnectionString;
        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        private static readonly RARepositorio instancia = new RARepositorio();

        #region Singleton

        public static RARepositorio Instancia
        {
            get { return RARepositorio.instancia; }
        }

        private RARepositorio() { }

        #endregion

        #region metodos

        /// <summary>
        /// Lista las acciones
        /// </summary>
        /// <returns></returns>
        public List<RAAccionDC> ListarAccion()
        {
            List<RAAccionDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(this.conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarAccionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaAccion(resultReader);
                }

            }
            return resultado;
        }

        public RAAccionDC ObtenerAccion(short idAccion)
        {
            RAAccionDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAccionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdAccion", idAccion));
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAAccion(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea una accion
        /// </summary>
        /// <param name="accion"></param>
        /// <returns>True si funciono</returns>
        public bool CrearAccion(RAAccionDC accion)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarAccionRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAccion", accion.IdAccion);
                cmd.Parameters.AddWithValue("@Descripcion", accion.Descripcion);
                cmd.Parameters.AddWithValue("@estado", accion.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Crear accion plantilla parametrizacion raps
        /// </summary>
        /// <param name="accionPlantillaParametrizacionRaps"></param>
        /// <returns></returns>
        public bool CrearAccionPlantillaParametrizacionRaps(RAAccionPlantillaParametrizacionRapsDC accionPlantillaParametrizacionRaps)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarAccionPlantillaParametrizacionRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAccionPlantilla", accionPlantillaParametrizacionRaps.IdAccionPlantilla);
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", accionPlantillaParametrizacionRaps.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@IdAccion", accionPlantillaParametrizacionRaps.IdAccion);
                cmd.Parameters.AddWithValue("@IdPlantilla", accionPlantillaParametrizacionRaps.IdPlantilla);
                cmd.Parameters.AddWithValue("@Estado", accionPlantillaParametrizacionRaps.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// listar acciones plantilla parametrizacion raps
        /// </summary>
        /// <returns></returns>
        public List<RAAccionPlantillaParametrizacionRapsDC> ListarAccionPlantillaParametrizacionRaps()
        {


            List<RAAccionPlantillaParametrizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarAccionPlantillaParametrizacionRapsRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaAccionPlantillaParametrizacionRaps(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Obtiene una plantilla de parametrizacionRaps para una accion
        /// </summary>
        /// <param name="idAccionPlantilla"></param>
        /// <returns></returns>
        public RAAccionPlantillaParametrizacionRapsDC ObtenerAccionPlantillaParametrizacionRaps(long idAccionPlantilla)
        {
            RAAccionPlantillaParametrizacionRapsDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerAccionPlantillaParametrizacionRapsRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdAccionPlantilla", idAccionPlantilla));

                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAAccionPlantillaParametrizacionRaps(resultReader);
                }

            }
            return resultado;
        }


        /// <summary>
        /// Crea un nuevo cargo
        /// </summary>
        /// <param name="Cargo"></param>
        /// <returns></returns>
        public bool CrearCargo(RACargoDC Cargo)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarCargoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCargo", Cargo.IdCargo);
                cmd.Parameters.AddWithValue("@Descripcion", Cargo.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", Cargo.Estado);
                cmd.Parameters.AddWithValue("@IdProcedimiento", Cargo.Procedimiento);
                cmd.Parameters.AddWithValue("@CargoNovasoft", Cargo.CargoNovasoft);
                cmd.Parameters.AddWithValue("@EnteControl", Cargo.EnteControl);
                cmd.Parameters.AddWithValue("@Regional", Cargo.Regional);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }


        public List<RACargoDC> ListarCargos()
        {
            List<RACargoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaListarCargoRap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperACargo(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Listar los cargos
        /// </summary>
        /// <param name="idCargo"></param>
        /// <returns></returns>
        public List<RACargoDC> ObtenerCargos(int idCargo)
        {
            List<RACargoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCargoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdCargo", idCargo));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperACargo(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Crea una nueva clasificacion
        /// </summary>
        /// <param name="clasificacion"></param>
        /// <returns>Verdadero al grabar </returns>
        public bool CrearClasificacion(RAClasificacionDC clasificacion)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarClasificacionRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@IdClasificacion", clasificacion.IdClasificacion);
                cmd.Parameters.AddWithValue("@Descripcion", clasificacion.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", clasificacion.Estado);


                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista las clasificaciones
        /// </summary>
        /// <returns>Lista la clasificacion</returns>
        public List<RAClasificacionDC> ListarClasificacion()
        {

            List<RAClasificacionDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarClasificacionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaClasificacion(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Obtiene una clasificacion
        /// </summary>
        /// <param name="idClasificacion"></param>
        /// <returns>Clasificacion</returns>
        public RAClasificacionDC ObtenerClasificacion(int idClasificacion)
        {
            RAClasificacionDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerClasificacionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdClasificacion", idClasificacion));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAClasificacion(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un registro de escalonamiento
        /// </summary>
        /// <param name="escalonamiento"></param>
        /// <returns></returns>
        public bool CrearEscalonamiento(RAEscalonamientoDC escalonamiento)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarEscalonamientoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@IdEscalonamiento", escalonamiento.IdEscalonamiento);
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", escalonamiento.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@idCargo", escalonamiento.idCargo);
                cmd.Parameters.AddWithValue("@IdProceso", escalonamiento.IdProceso);
                cmd.Parameters.AddWithValue("@IdProcedimiento", escalonamiento.IdProcedimiento);
                cmd.Parameters.AddWithValue("@orden", escalonamiento.Orden);
                cmd.Parameters.AddWithValue("@IdTipoHora", escalonamiento.IdTipoHora);
                cmd.Parameters.AddWithValue("@HorasEscalar", escalonamiento.HorasEscalar);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los registros de escalonamiento para una parametrizacion
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns>Lista de escalonamiento</returns>
        public List<RAEscalonamientoDC> ListarEscalonamiento(long idParametrizacionRap)
        {
            List<RAEscalonamientoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarEscalonamientoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdParametrizacionRap", idParametrizacionRap));


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaEscalonamiento(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// consulta un item de escalonamiento
        /// </summary>
        /// <param name="idEscalonamiento"></param>
        /// <returns>Escalonamiento</returns>
        //public RAEscalonamientoDC ObtenerEscalonamiento(long idEscalonamiento)
        //public List<RAEscalonamientoDC> ObtenerEscalonamiento(long idParametrizacionRap)
        //{
        //    RAEscalonamientoDC resultado = null;
        //    using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
        //    {
        //        sqlConn.Open();
        //        SqlCommand cmd = new SqlCommand("paObtenerEscalonamientoRAP", sqlConn);
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        cmd.Parameters.Add(new SqlParameter("@idParametrizacionRap", idParametrizacionRap));


        //        var resultReader = cmd.ExecuteReader();
        //        if (resultReader.HasRows)
        //        {
        //            resultado = RARepositorioMapper.MapperAEscalonamiento(resultReader);
        //        }

        //    }
        //    return resultado;
        //}

        /// <summary>
        /// Crear estado
        /// </summary>
        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CrearEstado(RAEstadosDC estado)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarEstadosRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@IdEstado", estado.IdEstado);
                cmd.Parameters.AddWithValue("@Descripcion", estado.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", estado.Estado);


                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los estados existentes
        /// </summary>
        /// <returns>Lista estado</returns>
        public List<RAEstadosDC> ListarEstados(bool esResponsable, int idEstadoActual)
        {
            List<RAEstadosDC> resultado = new List<RAEstadosDC>();
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarEstadosRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EsResponsable", esResponsable);
                cmd.Parameters.AddWithValue("@IdEstadoActual", idEstadoActual);
                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaEstados(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta un estado
        /// </summary>
        /// <param name="IdEstado"></param>
        /// <returns>objeto estado</returns>
        public RAEstadosDC ObtenerEstado(int IdEstado)
        {
            RAEstadosDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEstadosRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdEstado", IdEstado));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAEstados(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un nuevo item en la tabla Flujo Accion Estado
        /// </summary>
        /// <param name="flujoAccionEstado"></param>
        /// <returns>Verdaro si todo correcto</returns>
        public bool CrearFlujoAccionEstado(RAFlujoAccionEstadoDC flujoAccionEstado)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarFlujoAccionEstadoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFlujo", flujoAccionEstado.IdFlujo);
                cmd.Parameters.AddWithValue("@IdAccion", flujoAccionEstado.IdAccion);
                cmd.Parameters.AddWithValue("@idEstado", flujoAccionEstado.IdEstado);
                cmd.Parameters.AddWithValue("@IdCargo", flujoAccionEstado.IdCargo);
                cmd.Parameters.AddWithValue("@IdEstadoFinal", flujoAccionEstado.IdEstadoFinal);
                cmd.Parameters.AddWithValue("@IdCargoFinal", flujoAccionEstado.IdCargoFinal);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// lista los item de un flujo
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public List<RAFlujoAccionEstadoDC> ListarFlujoAccionEstado(int idFlujo)
        {
            List<RAFlujoAccionEstadoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarFlujoAccionEstadoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFlujo", idFlujo));


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaFlujoAccionEstado(resultReader);
                }

            }

            return resultado;
        }

        /// <summary>
        /// Obtiene un registro de flujo Accion Estado
        /// </summary>
        /// <param name="idFlujo"></param>
        /// <returns></returns>
        public RAFlujoAccionEstadoDC ObtenerFlujoAccionEstado(int idFlujo, byte idAccion, int idEstado, int idCargo)
        {
            RAFlujoAccionEstadoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerFlujoAccionEstadoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFlujo", idFlujo));
                cmd.Parameters.Add(new SqlParameter("@IdAccion", idAccion));
                cmd.Parameters.Add(new SqlParameter("@IdEstado", idEstado));
                cmd.Parameters.Add(new SqlParameter("@IdCargo", idCargo));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAFlujoAccionEstado(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Crea un formato
        /// </summary>
        /// <param name="formato"></param>
        /// <returns></returns>
        public bool CrearFormato(RAFormatoDC formato)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarFormatoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdFormato", formato.IdFormato);
                cmd.Parameters.AddWithValue("@Descripcion", formato.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", formato.Estado);
                cmd.Parameters.AddWithValue("@IdSistemaFormato", formato.IdSistemaFormato);

                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// lista los formatos
        /// </summary>
        /// <returns></returns>
        public List<RAFormatoDC> ListarFormato()
        {
            List<RAFormatoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarFormatoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaFormato(resultReader);
                }

            }
            return resultado;

        }

        public List<RAFormatoDC> ListaFormatosSitema(int IdSistemaFormato)
        {
            List<RAFormatoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarFormatosSistemaRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaFormato(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// obitiene un registro de formato
        /// </summary>
        /// <param name="idFormato"></param>
        /// <returns></returns>
        public RAFormatoDC ObtenerFormato(int idFormato)
        {
            RAFormatoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerFormatoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdFormato", idFormato));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAFormato(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un nuevo grupo usuario
        /// </summary>
        /// <param name="GrupoUsuario"></param>
        /// <returns></returns>
        public bool CrearGrupoUsuario(RAGrupoUsuarioDC GrupoUsuario)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarGrupoUsuarioRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupoUsuario", GrupoUsuario.IdGrupoUsuario);
                cmd.Parameters.AddWithValue("@Descripcion", GrupoUsuario.Descripcion);
                cmd.Parameters.AddWithValue("@estado", GrupoUsuario.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los grupos de usuario
        /// </summary>
        /// <returns></returns>
        public List<RAGrupoUsuarioDC> ListarGrupoUsuario()
        {
            List<RAGrupoUsuarioDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarGrupoUsuarioRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaGrupoUsuario(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Obtiene un registro de grupo usuario
        /// </summary>
        /// <param name="idGrupoUsuario"></param>
        /// <returns></returns>
        public RAGrupoUsuarioDC ObtenerGrupoUsuario(int idGrupoUsuario)
        {
            RAGrupoUsuarioDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerGrupoUsuarioRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdGrupoUsuario", idGrupoUsuario));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAGrupoUsuario(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea una nueva planilla de correo para una accion
        /// </summary>
        /// <param name="plantillaAccionCorreo"></param>
        /// <returns></returns>
        public bool CrearPantillaAccionCorreo(RAPantillaAccionCorreoDC plantillaAccionCorreo)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarPantillaAccionCorreoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdPlantilla", plantillaAccionCorreo.IdPlantilla);
                cmd.Parameters.AddWithValue("@IdAccion", plantillaAccionCorreo.IdAccion);
                cmd.Parameters.AddWithValue("@Asunto", plantillaAccionCorreo.Asunto);
                cmd.Parameters.AddWithValue("@Cuerpo", plantillaAccionCorreo.Cuerpo);
                cmd.Parameters.AddWithValue("@EsPredeterminada", plantillaAccionCorreo.EsPredeterminada);
                cmd.Parameters.AddWithValue("@Estado", plantillaAccionCorreo.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista las plantillas para una accion
        /// </summary>
        /// <returns></returns>
        public List<RAPantillaAccionCorreoDC> ListarPantillaAccionCorreo(byte idAccion)
        {
            List<RAPantillaAccionCorreoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarPantillaAccionCorreoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAccion", idAccion);


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaPantillaAccionCorreo(resultReader);
                }

            }

            return resultado;
        }

        /// <summary>
        /// Crea una nueva parametrizacion 
        /// </summary>
        /// <param name="ParametrizacionRaps"></param>
        /// <returns></returns>
        public long CrearParametrizacionRaps(RAParametrizacionRapsDC ParametrizacionRaps)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarParametrizacionRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", ParametrizacionRaps.Nombre);
                cmd.Parameters.AddWithValue("@IdSistemaFuente", ParametrizacionRaps.IdSistemaFuente);
                cmd.Parameters.AddWithValue("@IdTipoRap", ParametrizacionRaps.IdTipoRap);
                cmd.Parameters.AddWithValue("@DescripcionRaps", ParametrizacionRaps.DescripcionRaps);
                cmd.Parameters.AddWithValue("@IdProceso", ParametrizacionRaps.IdProceso);
                cmd.Parameters.AddWithValue("@UtilizaFormato", ParametrizacionRaps.UtilizaFormato);
                cmd.Parameters.AddWithValue("@IdFormato", ParametrizacionRaps.IdFormato);
                cmd.Parameters.AddWithValue("@IdTipoCierre", ParametrizacionRaps.IdTipoCierre);
                cmd.Parameters.AddWithValue("@IdCargoCierra", ParametrizacionRaps.IdCargoCierra);
                cmd.Parameters.AddWithValue("@IdCargoIncumplimiento", ParametrizacionRaps.IdCargoIncumplimiento);
                cmd.Parameters.AddWithValue("@IdOrigenRaps", ParametrizacionRaps.IdOrigenRaps);
                cmd.Parameters.AddWithValue("@Estado", ParametrizacionRaps.Estado);
                cmd.Parameters.AddWithValue("@IdGrupoUsuario", ParametrizacionRaps.IdGrupoUsuario);
                cmd.Parameters.AddWithValue("@IdSubclasificacion", ParametrizacionRaps.IdSubclasificacion);
                cmd.Parameters.AddWithValue("@IdTipoPeriodo", ParametrizacionRaps.IdTipoPeriodo);
                cmd.Parameters.AddWithValue("@IdHoraEscalar", ParametrizacionRaps.IdHoraEscalar);
                cmd.Parameters.AddWithValue("@IdTipohora", ParametrizacionRaps.IdTipoHora);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdTipoIncumplimiento", ParametrizacionRaps.IdTipoIncumplimiento);
                cmd.Parameters.AddWithValue("@IdParametrizacionPadre", ParametrizacionRaps.IdParametrizacionPadre == null ? 0 : ParametrizacionRaps.IdParametrizacionPadre);
                cmd.Parameters.AddWithValue("@IdNivelGravedad", ParametrizacionRaps.IdNivelGravedad);
                cmd.Parameters.AddWithValue("@TipoEscalonamiento", ParametrizacionRaps.IdTipoEscalonamiento);
                resultado = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return resultado;
        }

        /// <summary>
        /// Modificar Parametrizacion, Escalonamiento y Tiempos de Ejecucion Raps
        /// </summary>
        /// <param name="parametrizacionRaps"></param>
        /// <returns></returns>

        public bool ModificarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {

                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paActualizarParametrizacionRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", parametrizacionRaps.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@Nombre", parametrizacionRaps.Nombre);
                cmd.Parameters.AddWithValue("@IdSistemaFuente", parametrizacionRaps.IdSistemaFuente);
                cmd.Parameters.AddWithValue("@IdTipoRap", parametrizacionRaps.IdTipoRap);
                cmd.Parameters.AddWithValue("@DescripcionRaps", parametrizacionRaps.DescripcionRaps);
                cmd.Parameters.AddWithValue("@IdProceso", parametrizacionRaps.IdProceso);
                cmd.Parameters.AddWithValue("@UtilizaFormato", parametrizacionRaps.UtilizaFormato);
                cmd.Parameters.AddWithValue("@IdFormato", parametrizacionRaps.IdFormato);
                cmd.Parameters.AddWithValue("@IdTipoCierre", parametrizacionRaps.IdTipoCierre);
                cmd.Parameters.AddWithValue("@IdCargoCierra", parametrizacionRaps.IdCargoCierra);
                cmd.Parameters.AddWithValue("@IdCargoIncumplimiento", parametrizacionRaps.IdCargoIncumplimiento);
                cmd.Parameters.AddWithValue("@IdOrigenRaps", parametrizacionRaps.IdOrigenRaps);
                cmd.Parameters.AddWithValue("@IdGrupoUsuario", parametrizacionRaps.IdGrupoUsuario);
                cmd.Parameters.AddWithValue("@IdSubclasificacion", parametrizacionRaps.IdSubclasificacion);
                cmd.Parameters.AddWithValue("@IdTipoPeriodo", parametrizacionRaps.IdTipoPeriodo);
                cmd.Parameters.AddWithValue("@IdHoraEscalar", parametrizacionRaps.IdHoraEscalar);
                cmd.Parameters.AddWithValue("@IdTipohora", parametrizacionRaps.IdTipoHora);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista de Parametrizaciones de solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRaps()
        {
            List<RAParametrizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarParametrizacionRapsRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaParametrizacionRaps(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Lista de Parametrizaciones activas tipo manuales
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsManuales()
        {
            List<RAParametrizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarParametrizacionManualRaps_Rap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaParametrizacionRaps(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Lista de Parametrizaciones activas tipo tarea
        /// </summary>
        /// <returns></returns>
        public List<RAParametrizacionRapsDC> ListarParametrizacionRapsTareas()
        {
            List<RAParametrizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarParametrizacionTareaRaps_Rap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaParametrizacionRaps(resultReader);
                }

            }
            return resultado;

        }



        /// <summary>
        /// consulta la informacion de una parametrizacion de solicitudes 
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public RAParametrizacionRapsDC ObtenerParametrizacionRaps(long idParametrizacionRap)
        {
            RAParametrizacionRapsDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrizacionRapsRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdParametrizacionRap", idParametrizacionRap));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAParametrizacionRaps(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un registro de proceso
        /// </summary>
        /// <param name="proceso"></param>
        /// <returns></returns>
        public bool CrearProceso(RAProcesoDC proceso)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarProcesoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProceso", proceso.IdProceso);
                cmd.Parameters.AddWithValue("@Descripcion", proceso.Descripcion);
                cmd.Parameters.AddWithValue("@IdMacroProceso", proceso.IdMacroProceso);
                cmd.Parameters.AddWithValue("@estado", proceso.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los procesos
        /// </summary>
        /// <returns></returns>
        public List<RAProcesoDC> ListarProceso()
        {


            List<RAProcesoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarProcesoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaProceso(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta un proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public RAProcesoDC ObtenerProceso(int idProceso)
        {
            RAProcesoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerProcesoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdProceso", idProceso));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAProceso(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un registro de quien cierra
        /// </summary>
        /// <param name="TipoCierre"></param>
        /// <returns></returns>
        public bool CrearTipoCierre(RATipoCierreDC TipoCierre)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarTipoCierreRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@IdTipoCierre", TipoCierre.IdTipoCierre);
                cmd.Parameters.AddWithValue("@Descripcion", TipoCierre.Descripcion);
                cmd.Parameters.AddWithValue("@estado", TipoCierre.Estado);


                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista registros quien cierra
        /// </summary>
        /// <returns></returns>
        public List<RATipoCierreDC> ListarTipoCierre()
        {
            List<RATipoCierreDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarTipoCierreRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaTipoCierre(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// consulta la informacion un registro Quien Cierra
        /// </summary>
        /// <param name="idTipoCierre"></param>
        /// <returns></returns>
        public RATipoCierreDC ObtenerTipoCierre(int idTipoCierre)
        {
            RATipoCierreDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoCierreRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdQuienCierra", idTipoCierre));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAtipoCierre(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// crea un nuevo registro de sistema formato
        /// </summary>
        /// <param name="sistemaFormato"></param>
        /// <returns></returns>
        public bool CrearSistemaFormato(RASistemaFormatoDC sistemaFormato)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarSistemaFormatoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSistemaFormato", sistemaFormato.IdSistemaFormato);
                cmd.Parameters.AddWithValue("@Descripcion", sistemaFormato.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", sistemaFormato.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }


        /// <summary>
        /// lista los registros de sistema formato
        /// </summary>
        /// <returns></returns>
        public List<RASistemaFormatoDC> ListarSistemaFormato()
        {
            List<RASistemaFormatoDC> resultado = null;

            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarSistemaFormatoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaSistemaFormato(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta un registro de sistema formato
        /// </summary>
        /// <param name="idSistemaFormato"></param>
        /// <returns></returns>
        public RASistemaFormatoDC ObtenerSistemaFormato(int idSistemaFormato)
        {
            RASistemaFormatoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSistemaFormatoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSistemaFormato", idSistemaFormato));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperASistemaFormato(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea una nueva subclasificacion
        /// </summary>
        /// <param name="subClasificacion"></param>
        /// <returns></returns>
        public bool CrearSubClasificacion(RASubClasificacionDC subClasificacion)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarSubClasificacionRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSubclasificacion", subClasificacion.IdSubclasificacion);
                cmd.Parameters.AddWithValue("@Descripcion", subClasificacion.Descripcion);
                cmd.Parameters.AddWithValue("@IdClasificacion", subClasificacion.IdClasificacion);
                cmd.Parameters.AddWithValue("@Estado", subClasificacion.Estado);


                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista las sub clasificaciones de una clasificacion
        /// </summary>
        /// <returns></returns>
        public List<RASubClasificacionDC> ListarSubClasificacion(int idClasificacion)
        {


            List<RASubClasificacionDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarSubClasificacionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdClasificacion", idClasificacion));

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaSubClasificacion(resultReader);
                }

            }
            return resultado;
        }



        /// <summary>
        /// Consulta una subclasificacion
        /// </summary>
        /// <param name="IdSubclasificacion"></param>
        /// <returns></returns>
        public RASubClasificacionDC ObtenerSubClasificacion(int IdSubclasificacion)
        {
            RASubClasificacionDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerSubClasificacionRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSubclasificacion", IdSubclasificacion));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperASubClasificacion(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un nuevo tiempo de ejecucion
        /// </summary>
        /// <param name="TiempoEjecucionRaps"></param>
        /// <returns></returns>
        public bool CrearTiempoEjecucionRaps(RATiempoEjecucionRapsDC TiempoEjecucionRaps)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarTiempoEjecucionRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@NumeroEjecucion", TiempoEjecucionRaps.NumeroEjecucion);
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", TiempoEjecucionRaps.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@idTipoPeriodo", TiempoEjecucionRaps.idTipoPeriodo);
                cmd.Parameters.AddWithValue("@DiaPeriodo", TiempoEjecucionRaps.DiaPeriodo);
                cmd.Parameters.AddWithValue("@Hora", TiempoEjecucionRaps.Hora);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);

                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los tiempo de ejecucion para una parametrizacion de Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <returns></returns>
        public List<RATiempoEjecucionRapsDC> ListarTiempoEjecucionRaps(long idParametrizacionRap)
        {
            List<RATiempoEjecucionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarTiempoEjecucionRapsRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdParametrizacionRap", idParametrizacionRap));

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaTiempoEjecucionRaps(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta registro tiempo de ejecucion Raps
        /// </summary>
        /// <param name="idEjecucion"></param>
        /// <returns></returns>
        public RATiempoEjecucionRapsDC ObtenerTiempoEjecucionRaps(long idEjecucion)
        {
            RATiempoEjecucionRapsDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTiempoEjecucionRapsRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdEjecucion", idEjecucion));

                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperATiempoEjecucionRaps(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un tipo de hora
        /// </summary>
        /// <param name="tipoHora"></param>
        /// <returns></returns>
        public bool CrearTipoHora(RATipoHoraDC tipoHora)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarTipoHoraRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoHora", tipoHora.IdTipoHora);
                cmd.Parameters.AddWithValue("@Descripcion", tipoHora.Descripcion);
                cmd.Parameters.AddWithValue("@estado", tipoHora.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los tipos de hora
        /// </summary>
        /// <returns></returns>
        public List<RATipoHoraDC> ListarTipoHora()
        {
            List<RATipoHoraDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarTipoHoraRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaTipoHora(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta un tipo de hora
        /// </summary>
        /// <param name="idTipoHora"></param>
        /// <returns></returns>
        public RATipoHoraDC ObtenerTipoHora(int idTipoHora)
        {
            RATipoHoraDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoHoraRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoHora", idTipoHora));
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperATipoHora(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Crea un nuevo tipo de periodo
        /// </summary>
        /// <param name="tipoPeriodo"></param>
        /// <returns></returns>
        public bool CrearTipoPeriodo(RATipoPeriodoDC tipoPeriodo)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarTipoPeriodoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idTipoPeriodo", tipoPeriodo.IdTipoPeriodo);
                cmd.Parameters.AddWithValue("@Descripcion", tipoPeriodo.Descripcion);
                cmd.Parameters.AddWithValue("@Periodos", tipoPeriodo.Periodos);
                cmd.Parameters.AddWithValue("@Estado", tipoPeriodo.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los tipos de periodos
        /// </summary>
        /// <returns></returns>
        public List<RATipoPeriodoDC> ListarTipoPeriodo()
        {
            List<RATipoPeriodoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarTipoPeriodoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaTipoPeriodo(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta un tipo de periodo
        /// </summary>
        /// <param name="idTipoPeriodo"></param>
        /// <returns></returns>
        public RATipoPeriodoDC ObtenerTipoPeriodo(int idTipoPeriodo)
        {
            RATipoPeriodoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoPeriodoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoPeriodo", idTipoPeriodo));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperATipoPeriodo(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea un nuevo tipo de Rap
        /// </summary>
        /// <param name="tipoRap"></param>
        /// <returns></returns>
        public bool CrearTipoRap(RATipoRapDC tipoRap)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarTipoRapRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoRap", tipoRap.IdTipoRap);
                cmd.Parameters.AddWithValue("@Descripcion", tipoRap.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", tipoRap.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los tipos de Raps
        /// </summary>
        /// <returns></returns>
        public List<RATipoRapDC> ListarTipoRap()
        {
            List<RATipoRapDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarTipoRapRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaTipoRap(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// Consulta un tipo de rap
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public RATipoRapDC ObtenerTipoRap(int idTipoRap)
        {
            RATipoRapDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerTipoRapRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoRap", idTipoRap));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperATipoRap(resultReader);
                }

            }
            return resultado;
        }



        /// <summary>
        /// Crea un nuevo Origen de Rap
        /// </summary>
        /// <param name="OrigenRaps"></param>
        /// <returns></returns>
        public bool CrearOrigenRaps(RAOrigenRapsDC origenRaps)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarOrigenRaps", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdOrigenRaps", origenRaps.IdOrigenRaps);
                cmd.Parameters.AddWithValue("@Descripcion", origenRaps.descripcion);
                cmd.Parameters.AddWithValue("@Estado", origenRaps.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Crea un macroproceso
        /// </summary>
        /// <param name="macroproceso"></param>
        /// <returns></returns>
        public bool CrearMacroproceso(RAMacroprocesoDC macroproceso)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarMacroprocesoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idMacriProceso", macroproceso.IdMacroProceso);
                cmd.Parameters.AddWithValue("@Descripcion", macroproceso.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", macroproceso.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// listar los macroprocesos
        /// </summary>
        /// <returns></returns>
        public List<RAMacroprocesoDC> ListarMacroproceso()
        {
            List<RAMacroprocesoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarMacroprocesoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaMacroproceso(resultReader);
                }

            }
            return resultado;

        }

        /// <summary>
        /// obtener un macroproceso
        /// </summary>
        /// <param name="idMacroProceso"></param>
        /// <returns></returns>
        public RAMacroprocesoDC ObtenerMacroproceso(int idMacroProceso)
        {
            RAMacroprocesoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerMacroprocesoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdMacroProceso", idMacroProceso));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAMacroproceso(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Crea procedimiento
        /// </summary>
        /// <param name="Procedimiento"></param>
        /// <returns></returns>
        public bool CrearProcedimiento(RAProcedimientoDC Procedimiento)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarProcedimientoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idProcedimiento", Procedimiento.IdProcedimiento);
                cmd.Parameters.AddWithValue("@Descripcion", Procedimiento.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", Procedimiento.Estado);
                cmd.Parameters.AddWithValue("@IdProceso", Procedimiento.IdProceso);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Lista los procedimiento de un proceso
        /// </summary>
        /// <returns></returns>
        public List<RAProcedimientoDC> ListarProcedimiento(int idProceso)
        {
            List<RAProcedimientoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarProcedimientoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdProceso", idProceso);


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaProcedimiento(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtener el procedimiento del parametro
        /// </summary>
        /// <param name="idProcedimiento"></param>
        /// <returns></returns>
        public RAProcedimientoDC ObtenerProcedimiento(int idProcedimiento)
        {
            RAProcedimientoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerProcedimientoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdProcedimiento", idProcedimiento));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAProcedimiento(resultReader);
                }
            }

            return resultado;
        }

        #endregion

        #region UsuarioDeGrupo
        public bool CrearUsuarioDeGrupo(RAUsuarioDeGrupoDC UsuarioDeGrupo)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarUsuarioDeGrupoRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCargo", UsuarioDeGrupo.IdCargo);
                cmd.Parameters.AddWithValue("@idUsuarioGrupo", UsuarioDeGrupo.idUsuarioGrupo);

                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        public List<RAUsuarioDeGrupoDC> ListarUsuariosDeGrupo(int idUsuarioGrupo)
        {

            List<RAUsuarioDeGrupoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarUsuarioDeGrupoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuarioGrupo", idUsuarioGrupo));
                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaUsuarioDeGrupo(resultReader);
                }

            }
            return resultado;

        }

        public RAUsuarioDeGrupoDC ObtenerUsuarioDeGrupo(int idCargo, int idUsuarioGrupo)
        {
            RAUsuarioDeGrupoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerUsuarioDeGrupoRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdUsuarioGrupo", idUsuarioGrupo));
                cmd.Parameters.Add(new SqlParameter("@IdCargo", idCargo));
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAUsuarioDeGrupo(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>
        /// Listar origenes de Raps
        /// </summary>
        /// <returns></returns>
        public List<RAOrigenRapsDC> ListarOrigenRaps()
        {
            List<RAOrigenRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaListarOrigenRapsRap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaOrigenRaps(resultReader);

                }
            }

            return (resultado == null ? null : resultado.FindAll(x => x.Estado.Equals(true)));
        }

        public List<RAFormatoDC> ListarFormatoAct(int idSistemaFormato)
        {
            List<RAFormatoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarFormatosSistemaRAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdSistemaFormato", idSistemaFormato));

                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAListarFormato(resultReader);
                }

            }
            return resultado;
        }

        /// <summary>

        /// Cambia el estado de una parametrizacion Raps
        /// </summary>
        /// <param name="idParametrizacionRap"></param>
        /// <param name="estado"></param>
        /// <returns></returns>
        public bool CambiaEstadoParametrizacionRaps(long idParametrizacionRap, bool estado)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paCambiaEstadoParametrizacionRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", idParametrizacionRap);
                cmd.Parameters.AddWithValue("@Estado", estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        #endregion

        #region Insertar

        /// <summary>
        /// Crear Tipo Incumplimiento
        /// </summary>
        /// <param name="tipoIncumplimiento"></param>
        /// <returns></returns>
        public bool CrearTipoIncumplimiento(RATipoIncumplimientoDC tipoIncumplimiento)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("PaInsertarTipoIncumplimiento", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoIncumplimiento", tipoIncumplimiento.IdTipoIncumplimiento);
                cmd.Parameters.AddWithValue("@Descripcion", tipoIncumplimiento.Descripcion);
                cmd.Parameters.AddWithValue("@Estado", tipoIncumplimiento.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        /// <summary>
        /// Crear Hora Escalar
        /// </summary>
        /// <param name="horaEscalar"></param>
        /// <returns></returns>
        public bool CrearHoraEscalar(RAHoraEscalarDC horaEscalar)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarHoraEscalarRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idHoraEscalar", horaEscalar.IdHoraEscalar);
                cmd.Parameters.AddWithValue("@HoraEscalar", horaEscalar.HoraEscalar);
                cmd.Parameters.AddWithValue("@Estado", horaEscalar.Estado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        public void InsertarParametroSolicitud(long idSolicitud, int idParametro, string valor, string usuario = "")
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarParametrosSolicitud_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", idSolicitud);
                cmd.Parameters.AddWithValue("@IdParametro", idParametro);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current != null ? ControllerContext.Current.Usuario : usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Insertar nueva solicitud
        /// </summary>
        /// <param name="InsertarSolicitud"></param>
        /// <returns></returns>
        public long InsertarSolicitud(RASolicitudDC solicitudRaps)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarSolicitudRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParametrizacionRap", solicitudRaps.IdParametrizacionRap);
                cmd.Parameters.AddWithValue("@IdCargoSolicita", solicitudRaps.IdCargoSolicita);
                cmd.Parameters.AddWithValue("@IdCargoResponsable", solicitudRaps.IdCargoResponsable);
                cmd.Parameters.AddWithValue("@FechaCreacion", solicitudRaps.FechaCreacion);
                cmd.Parameters.AddWithValue("@FechaInicio", solicitudRaps.FechaCreacion);
                cmd.Parameters.AddWithValue("@FechaVencimiento", solicitudRaps.FechaVencimiento);
                cmd.Parameters.AddWithValue("@IdEstado", solicitudRaps.IdEstado);
                cmd.Parameters.AddWithValue("@Descripcion", solicitudRaps.Descripcion);
                cmd.Parameters.AddWithValue("@IdSolicitudPadre", solicitudRaps.IdSolicitudPadre);
                cmd.Parameters.AddWithValue("@DocumentoSolicita", solicitudRaps.DocumentoSolicita);
                cmd.Parameters.AddWithValue("@DocumentoResponsable", solicitudRaps.DocumentoResponsable);
                cmd.Parameters.AddWithValue("@CodigoSucursal", solicitudRaps.idSucursal.ToString());
                resultado = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return resultado;
        }

        //public long InsertarGestion(RAGestionDC gestion)
        //{
        //    int resultado = 0;

        //    using (var clientConn = new SqlConnection(conexionStringRaps))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        clientConn.Open();
        //        cmd = new SqlCommand("paInsertarGestionRAP", clientConn);
        //        cmd.CommandType = System.Data.CommandType.StoredProcedure;

        //        cmd.Parameters.AddWithValue("@IdGestion", gestion.IdGestion);
        //        cmd.Parameters.AddWithValue("@Fecha", gestion.Fecha);
        //        cmd.Parameters.AddWithValue("@Comentario", gestion.Comentario);
        //        cmd.Parameters.AddWithValue("@IdCargoGestiona", gestion.IdCargoGestiona);
        //        cmd.Parameters.AddWithValue("@CorreoEnvia", gestion.CorreoEnvia);
        //        cmd.Parameters.AddWithValue("@IdAccion", gestion.IdAccion);
        //        cmd.Parameters.AddWithValue("@IdCargoDestino", gestion.IdCargoDestino);
        //        cmd.Parameters.AddWithValue("@CorreoDestino", gestion.CorreoDestino);
        //        cmd.Parameters.AddWithValue("@IdResponsable", gestion.IdResponsable);
        //        cmd.Parameters.AddWithValue("@IdEstado", gestion.IdEstado);
        //        cmd.Parameters.AddWithValue("@IdUsuario", gestion.IdUsuario);
        //        cmd.Parameters.AddWithValue("@FechaVencimiento", gestion.FechaVencimiento);
        //        cmd.Parameters.AddWithValue("@DocumentoSolicita", gestion.DocumentoSolicita);
        //        cmd.Parameters.AddWithValue("@DocumentoResponsable", gestion.DocumentoResponsable);
        //        resultado = Convert.ToInt32(cmd.ExecuteScalar());
        //    }

        //    return resultado;
        //}       

        public void InsertarAdjuntos(RAAdjuntoDC adjunto)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paInsertarAdjuntosRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdAdjunto", adjunto.IdAdjunto);
                cmd.Parameters.AddWithValue("@IdGestion", adjunto.IdGestion);
                cmd.Parameters.AddWithValue("@Tamaño", adjunto.Tamaño);
                cmd.Parameters.AddWithValue("@Extension", adjunto.Extension);
                cmd.Parameters.AddWithValue("@UbicacionNombre", adjunto.UbicacionNombre);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertarParametrosParametrizacion(RAParametrosParametrizacionDC parametros)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarParametrosParametrizacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", parametros.idParametrizacionRap);
                cmd.Parameters.AddWithValue("@DescripcionParametro", parametros.descripcionParametro);
                cmd.Parameters.AddWithValue("@IdTipoDato", parametros.idTipoDato);
                cmd.Parameters.AddWithValue("@Longitud", parametros.longitud);
                cmd.Parameters.AddWithValue("@EsAgrupamiento", parametros.EsAgrupamiento);
                cmd.Parameters.AddWithValue("@EsEncabezadoDescripcion",parametros.EsEncabezadoDescripcion);
                cmd.Parameters.AddWithValue("@DescripcionReporte", parametros.DescripcionReporte);
                cmd.Parameters.AddWithValue("@idTipoNovedad", parametros.idTipoNovedad == 0 ? null : parametros.idTipoNovedad);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        public int InsertarTipoNovedad(string descripcion)
        {
            int resultado = 0;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarTipoNovedad_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@descripcion", descripcion);
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                resultado = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            }
            return resultado;
        }

        public void InsertarParametroSolicitudAcumulativa(long idSolicitudAcumulativa, int idParametro, string valor)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarParametrosSolicitudAcumulativa_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitudAcumulativa", idSolicitudAcumulativa);
                cmd.Parameters.AddWithValue("@IdParametroParametrizacion", idParametro);
                cmd.Parameters.AddWithValue("@Valor", valor);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                //cmd.Parameters.AddWithValue("@EsAgrupamiento", esAgrupamiento);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Inserta las notificaciones que no pudieron ser enviadas al usuario
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public void InsertarNotificacionPendiente(string mensaje, long idReceptor, long idSolicitud)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarNotificacionesPendientes_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@mensaje", mensaje);
                cmd.Parameters.AddWithValue("@idReceptor", idReceptor.ToString());
                cmd.Parameters.AddWithValue("@creadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@idSolicitud", idSolicitud);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Crea el grupo
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool CrearGrupo(RACargoGrupoDC grupo)
        {
            var resultado = 0;
            string IdCargos = string.Empty;
            foreach (string Id in grupo.lstCargo.Select(g => g.IdCargo))
            {
                if (string.IsNullOrEmpty(IdCargos))
                {
                    IdCargos = Id;
                }
                else
                {
                    IdCargos = IdCargos + "," + Id;
                }
            }

            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Descripcion", grupo.DescripcionGrupo);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@IdCargos", IdCargos);
                conn.Open();
                resultado = cmd.ExecuteNonQuery();
                conn.Close();
            }
            return resultado > 0 ? true : false;
        }

        /// <summary>
        /// Inserta los cargos que coresponden a un grupo
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idGrupo"></param>
        public void InsertarGrupoCargo(string idCargo, int idGrupo)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paInsertarCargoGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCargo", idCargo);
                cmd.Parameters.AddWithValue("@IdGrupo", idGrupo);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        #endregion

        #region Actualizar

        public bool ActualizaSolicitud(RASolicitudDC solicitud)
        {
            int resultado = 0;

            using (var clientConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clientConn.Open();
                cmd = new SqlCommand("paActualizaSolicitudRapsRAP", clientConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdSolicitud", solicitud.IdSolicitud);
                cmd.Parameters.AddWithValue("@IdCargoSolicita", solicitud.IdCargoSolicita);
                cmd.Parameters.AddWithValue("@IdCargoResponsable", solicitud.IdCargoResponsable);
                cmd.Parameters.AddWithValue("@FechaVencimiento", solicitud.FechaVencimiento);
                cmd.Parameters.AddWithValue("@IdEstado", solicitud.IdEstado);
                resultado = cmd.ExecuteNonQuery();
            }

            return resultado == 1 ? true : false;
        }

        public long ActualizarParametrizacionRaps(RAParametrizacionRapsDC parametrizacionRaps)
        {
            throw new NotImplementedException();
        }

        public void ModificarParametroRaps(RAParametrosParametrizacionDC parametro)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paModificarParametrosParametrizacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametrizacionRap", parametro.idParametrizacionRap);
                cmd.Parameters.AddWithValue("@DescripcionParametro", parametro.descripcionParametro);
                cmd.Parameters.AddWithValue("@IdTipoDato", parametro.idTipoDato);
                cmd.Parameters.AddWithValue("@Longitud", parametro.longitud);
                cmd.Parameters.AddWithValue("@ModificadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@idParametro", parametro.idParametro);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Elimina un grupo por su id
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>        
        public bool EliminarGrupo(int idGrupo)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdGrupo", idGrupo);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            return true;
        }

        /// <summary>
        /// Edita la informacion de un grupo
        /// </summary>
        /// <param name="infoGrupo"></param>        
        public void EditarGrupo(RACargoGrupoDC infoGrupo)
        {
            string IdCargos = string.Empty;
            foreach (string Id in infoGrupo.lstCargo.Select(g => g.IdCargo))
            {
                if (string.IsNullOrEmpty(IdCargos))
                {
                    IdCargos = Id;
                }
                else
                {
                    IdCargos = IdCargos + "," + Id;
                }
            }
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paActualizarInfoGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Descripcion", infoGrupo.DescripcionGrupo);
                cmd.Parameters.AddWithValue("@IdGrupo", infoGrupo.IdGrupo);
                cmd.Parameters.AddWithValue("@IdCargos", IdCargos);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Cambia de estato 0 a 1 las notificaciones vistas por un usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        public void ActualizarEstadoNotificacion(string idUsuario)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paModificarEstadoNotificacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdNotificacion", idUsuario);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        #endregion

        #region Eliminar

        public void EliminarParametros(string idsParametrizacion)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarParametrosParametrizacion_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@lstGuiasNoEliminar", idsParametrizacion);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>
        /// Borrar Escalonamiento de acuerdo a un IdParametrizacion
        /// </summary>
        /// <param name="IdParametrizacionRap"></param>
        public void borrarEscalonamiento(long IdParametrizacionRap)
        {
            using (var clienteConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clienteConn.Open();
                cmd = new SqlCommand("paBorrarEscalonamientoRAP", clienteConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParametrizacionRap", IdParametrizacionRap);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Borrar Tiempo de Ejecucion
        /// </summary>
        /// <param name="IdParametrizacionRap"></param>
        public void borrarTiempoEjecucion(long IdParametrizacionRap)
        {
            using (var clienteConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand();
                clienteConn.Open();
                cmd = new SqlCommand("paBorrarTiempoEjecucionRAP", clienteConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdParametrizacionRap", IdParametrizacionRap);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina todos los cargosque pertenezcan a determinado grupo
        /// </summary>
        /// <param name="p"></param>
        public void EliminarCargosDeGrupo(int idGrupo)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paEliminarCargosPorGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idGrupo", idGrupo);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
        #endregion

        #region Consultar

        public List<RATipoDatoDC> ObtenerTiposDatos()
        {
            List<RATipoDatoDC> lstTiposDatos = new List<RATipoDatoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerTiposDatos_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RATipoDatoDC tipoDato = new RATipoDatoDC();
                    tipoDato.idTipoDato = Convert.ToInt32(reader["IdTipoDato"]);
                    tipoDato.descripcion = reader["DescripcionTipoDato"].ToString();
                    lstTiposDatos.Add(tipoDato);
                }
                conn.Close();
            }
            return lstTiposDatos;
        }

        public List<RATiposDatosParametrizacionDC> ObtenerParamametroPorIdDeParametrizacion(long idParametrizacion)
        {
            List<RATiposDatosParametrizacionDC> lstTiposDatos = new List<RATiposDatosParametrizacionDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultarTiposDatosParametros_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idParametrizacion", idParametrizacion);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RATiposDatosParametrizacionDC tipo = new RATiposDatosParametrizacionDC();
                    tipo.IdTipoDato = Convert.ToInt32(reader["IdTipoDato"]);
                    tipo.IdTipoParametro = Convert.ToInt32(reader["IdParametro"]);
                    tipo.descripcionParametro = reader["DescripcionParametro"].ToString();
                    tipo.descripcionTipoDato = reader["DescripcionTipoDato"].ToString();
                    lstTiposDatos.Add(tipo);
                }
                conn.Close();
            }
            return lstTiposDatos;
        }

        public List<RAParametrosParametrizacionDC> ListarParametros(long idParametrizacion)
        {
            List<RAParametrosParametrizacionDC> lstParametros = new List<RAParametrosParametrizacionDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultarParametros_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idParametrizacion", idParametrizacion);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAParametrosParametrizacionDC parametro = new RAParametrosParametrizacionDC();
                    parametro.idParametro = Convert.ToInt32(reader["IdParametro"]);
                    parametro.descripcionParametro = reader["DescripcionParametro"].ToString();
                    parametro.idTipoDato = Convert.ToInt32(reader["IdTipoDato"]);
                    parametro.idTipoNovedad = reader["IdTipoNovedad"].ToString() == ""?0:Convert.ToInt32(reader["IdTipoNovedad"]);
                    parametro.longitud = Convert.ToInt32(reader["Longitud"]);
                    parametro.EsAgrupamiento = Convert.ToBoolean(reader["esAgrupamiento"]);
                    parametro.EsEncabezadoDescripcion = reader["EsEncabezadoDescripcion"].ToString() != "" ? Convert.ToBoolean(reader["EsEncabezadoDescripcion"]) : false;
                    parametro.DescripcionReporte = reader["DescripcionReporte"].ToString() != "" ? Convert.ToBoolean(reader["DescripcionReporte"]) : false;
                    lstParametros.Add(parametro);
                }
                conn.Close();
                return lstParametros;
            }
        }

        public RAOrigenRapsDC ObtenerOrigenRaps(int idOrigenRaps)
        {

            RAOrigenRapsDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaObtenerOrigenRaps", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdOrigenRaps", idOrigenRaps));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAOrigenRaps(resultReader);
                }
            }

            return resultado;
        }

        public bool ConsultarTipoNovedad(int idTipoNovedad, int idParametro)
        {
            bool existeTipo = false;
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultarTipoNovedad_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idTipoNovedad", idTipoNovedad);
                cmd.Parameters.AddWithValue("@idParametro", idParametro);
                existeTipo = Convert.ToInt32(cmd.ExecuteScalar()) >= 1 ? true : false;
            }
            return existeTipo;
        }

        public List<RAHoraEscalarDC> ListarHoraEscalar()
        {
            List<RAHoraEscalarDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarHoraEscalarRap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListarHoraEscalar(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtener Hora Escalar
        /// </summary>
        /// <param name="idhoraEscalar"></param>
        /// <returns></returns>
        public RAHoraEscalarDC ObtenerHoraEscalar(int idhoraEscalar)
        {
            RAHoraEscalarDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaObtenerHoraEscalar", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdHoraEscalar", idhoraEscalar));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperObtenerHoraEscalar(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtener Tipo Incumplimiento
        /// </summary>
        /// <param name="idTipoIncumplimiento"></param>
        /// <returns></returns>
        public RATipoIncumplimientoDC ObtenerTipoIncumplimiento(int idTipoIncumplimiento)
        {
            RATipoIncumplimientoDC resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("PaObtenerTipoIncumplimiento", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdTipoIncumplimiento", idTipoIncumplimiento));


                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperATipoIncumplimiento(resultReader);
                }
            }

            return resultado;
        }

        public List<RAPaginaParametrizacionRapsDC> ListarParametrizacionRapsPaginada(int pagina, int registrosXPagina, int tipoRap, string ordenaPor)
        {
            List<RAPaginaParametrizacionRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarParametrizacionRapsPaginada", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@pagina", pagina);
                cmd.Parameters.AddWithValue("@registrosXPagina", registrosXPagina);
                cmd.Parameters.AddWithValue("@TipoRap", tipoRap);
                cmd.Parameters.AddWithValue("@ordenaPor", ordenaPor);

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperAPaginaParametrizacionRaps(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Listar Personas con cargo de Novasoft
        /// </summary>
        /// <returns></returns>
        public List<RACargoPersonaNovaRapDC> ListarCargoPersonaNova_Rap(RAFiltroCargoPersonaNovaRapDC filtro)
        {
            List<RACargoPersonaNovaRapDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringNovasoft))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCargoPersonaNOVA_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cargos", filtro.CodigoCargos);
                cmd.Parameters.AddWithValue("@territoriales", filtro.IdTerritoriales);
                cmd.Parameters.AddWithValue("@regionales", filtro.IdRegionales);
                cmd.Parameters.AddWithValue("@procesos", filtro.IdProcesos);
                cmd.Parameters.AddWithValue("@procedimientos", filtro.IdProcedimientos);
                cmd.Parameters.AddWithValue("@porPersona", filtro.PorPersona);

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListarCargoPersonaNova_Rap(resultReader, filtro.PorPersona);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Listar Cargo Escalonamiento Parametrizacionn de una Raps
        /// </summary>
        /// <returns></returns>
        public List<RAListarCargoEscalonamientoRapsDC> ListarCargoEscalonamientoRaps()
        {
            List<RAListarCargoEscalonamientoRapsDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringNovasoft))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerCargosNOVA_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListarCargoEscalonamientoRaps(resultReader);
                }
            }

            return resultado;

            //List<RAListarCargoEscalonamientoRapsDC> resultado = null;
            //using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            //{
            //    sqlConn.Open();
            //    SqlCommand cmd = new SqlCommand("paListarCargoEscalonamientoRaps", sqlConn);
            //    cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //    var resultReader = cmd.ExecuteReader();

            //    if (resultReader.HasRows)
            //    {
            //        resultado = RARepositorioMapper.MapperListarCargoEscalonamientoRaps(resultReader);
            //    }
            //}

            //return resultado;
        }

        /// <summary>
        /// ListarCargoEscalonamientoParametrizacionRaps
        /// </summary>
        /// <returns></returns>
        public List<RAEscalonamientoDC> ListarCargoEscalonamientoParametrizacionRaps(int idParametrizacion)
        {
            List<RAEscalonamientoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarCargoEscalonamientoParametrizacionRaps", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idParametrizacionRaps", idParametrizacion);

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListarCargoEscalonamientoParametrizacionRaps(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// listar los Tipos de Incumplimiento
        /// </summary>
        /// <returns></returns>
        public List<RATipoIncumplimientoDC> ListarTipoIncumplimiento()
        {
            List<RATipoIncumplimientoDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paListarTipoIncumplimiento", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaTipoIncumplimiento(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Listar sucursales y cargos para definir escalonamiento
        /// </summary>
        /// <returns></returns>
        public List<RARegionalSuculsalDC> ListarRegionalCargo()
        {
            List<RARegionalSuculsalDC> resultado = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paConsultaRegionalCargoRap", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperListaRegionalSucursal(resultReader);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Metodo para Obtener las territoriales
        /// </summary>
        /// <returns></returns>
        public List<RATerritorialDC> ObtenerTerritoriales()
        {
            List<RATerritorialDC> resultado = new List<RATerritorialDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringNovasoft))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"paObtenerTerritorialesNova_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Dispose();
                if (reader.HasRows)
                {
                    resultado = RARepositorioMapper.MapperObtenerTerritoriales(reader);
                }
            }
            return resultado;
        }

        /// <summary>
        /// Metodo para Obtener las regionales
        /// </summary>
        /// <returns></returns>
        public List<RARegionalSuculsalDC> ObtenerRegionales()
        {
            List<RARegionalSuculsalDC> lstRegionales = new List<RARegionalSuculsalDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringNovasoft))
            {
                conn.Open();
                //SqlCommand cmd = new SqlCommand(@"paObtenerRegionales_RAP", conn);
                SqlCommand cmd = new SqlCommand(@"paObtenerRegionalesNova_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Dispose();
                if (reader.HasRows)
                {
                    lstRegionales = RARepositorioMapper.MapperObtenerSucursales(reader);
                }
            }
            return lstRegionales;
        }

        /// <summary>
        /// Metodo para obtener todos los empleados con su respetiva informacion de novasoft
        /// </summary>
        /// <returns></returns>
        public List<RAPersonaDC> ObtenerEmpleadosNovasoft(string cargos, int idParametrizacion, int idSucursal)
        {
            List<RAPersonaDC> lstPersona = new List<RAPersonaDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"paObtenerEmpleadosNovasoft_RAP", conn);
                cmd.Parameters.Add(new SqlParameter("@IdCargo", cargos));
                cmd.Parameters.Add(new SqlParameter("@Idparametrizacion", idParametrizacion));
                cmd.Parameters.Add(new SqlParameter("@IdSucursal", idSucursal));
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.Dispose();
                if (reader.HasRows)
                {
                    lstPersona = RARepositorioMapper.MapperObtenerPersonalNovasoft(reader);
                }
            }

            return lstPersona;
        }

        /// <summary>
        /// consulta la sucursal por id de ciudad
        /// </summary>
        /// <param name="p"></param>
        public string ObtenerSucursalNovasoft(string idCiudad)
        {
            string idSucursal = "";
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerSucursalNovaSoft_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCiudad", idCiudad);
                conn.Open();
                var idSuc = cmd.ExecuteScalar();
                conn.Close();
                if (idSuc != null)
                    idSucursal = idSuc.ToString();
            }
            return idSucursal;
        }

        public List<RASolicitudDC> ObtenerParametrizacionesSegunNovedad(int idSistema, int idTipoNovedad, string idCiudad)
        {
            List<RASolicitudDC> listaParametrizacion = new List<RASolicitudDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerParametrizacionParaSolicitudesAcumulativa_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.AddWithValue("@idSistemaFuente", idSistema);
                cmd.Parameters.AddWithValue("@idTipoNovedad", idTipoNovedad);
                cmd.Parameters.AddWithValue("@idCiudad", idCiudad);


                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();
                listaParametrizacion = dt.AsEnumerable().ToList().ConvertAll<RASolicitudDC>(r =>
                    {
                        RASolicitudDC parametrizacion = new RASolicitudDC()
                        {
                            Descripcion = r.Field<string>("TPN_DescripcionTipoNovedad"),
                            IdParametrizacionRap = r.Field<long>("IdParametrizacionRap"),
                            idSucursal = r["codigoSuc"].ToString()
                        };
                        return parametrizacion;
                    });
            }
            return listaParametrizacion;
        }

        /// <summary>
        /// Obtiene todos los menus para raps de la tabla menuconfiguracion
        /// </summary>
        /// <returns></returns>
        public List<RAMenusPermitidosDC> ObtenerMenusRaps(string identificacion)
        {
            List<RAMenusPermitidosDC> lstMenus = new List<RAMenusPermitidosDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerMenusRaps_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                cmd.Parameters.Add(new SqlParameter("@DocumentoSolicita", identificacion));
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAMenusPermitidosDC menu = new RAMenusPermitidosDC
                    {
                        Nombre = reader["MCF_Nombre"].ToString(),
                        Icon = DBNull.Value.Equals(reader["MCF_Icono"]) ? "" : reader["MCF_Icono"].ToString(),
                        Accion = DBNull.Value.Equals(reader["MCF_Accion"]) ? "" : reader["MCF_Accion"].ToString(),
                        Activo = Convert.ToBoolean(reader["MCF_Habilitado"]),
                        IdMenu = Convert.ToInt32(reader["MCF_IdMenu"]),
                        MenuPadre = DBNull.Value.Equals(reader["MCF_MenuPadre"]) ? 0 : Convert.ToInt32(reader["MCF_MenuPadre"]),
                        NumeroSolicitudes = Convert.ToInt32(reader["MCF_Numero"])
                    };
                    lstMenus.Add(menu);
                }
                conn.Close();
            }
            return lstMenus;
        }

        /// <summary>
        /// obtiene los procesos
        /// </summary>
        public List<RAProcesoDC> ObtenerProcesos()
        {
            List<RAProcesoDC> lstProcesos = new List<RAProcesoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringNovasoft))
            {
                SqlCommand cmd = new SqlCommand("paObtenerProcesosNOVA_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAProcesoDC proceso = new RAProcesoDC
                    {
                        IdProceso = reader["IdProceso"].ToString(),
                        Descripcion = reader["Descripcion"].ToString()
                    };
                    lstProcesos.Add(proceso);
                }
                conn.Close();
            }
            return lstProcesos;
            //List<RAProcesoDC> lstProcesos = new List<RAProcesoDC>();
            //using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            //{
            //    SqlCommand cmd = new SqlCommand("paObtenerProcesos_RAP", conn);
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    conn.Open();
            //    SqlDataReader reader = cmd.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        RAProcesoDC proceso = new RAProcesoDC
            //        {
            //            IdProceso = Convert.ToInt32(reader["IdProceso"]),
            //            Descripcion = reader["Descripcion"].ToString()
            //        };
            //        lstProcesos.Add(proceso);
            //    }
            //    conn.Close();
            //}
            //return lstProcesos;
        }

        /// <summary>
        /// Obtiene los procedimientos de determinado proceso
        /// </summary>
        /// <param name="idProceso"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientoPorproceso(int idProceso)
        {
            List<RAProcedimientoDC> lstProcedimiento = new List<RAProcedimientoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerProcedimientoPorProceso_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idProceso", idProceso);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RAProcedimientoDC procedimiento = new RAProcedimientoDC
                    {
                        IdProcedimiento = reader["IdProcedimiento"].ToString(),
                        Descripcion = reader["Descripcion"].ToString()
                    };
                    lstProcedimiento.Add(procedimiento);
                }
                conn.Close();
            }
            return lstProcedimiento;
        }

        /// <summary>
        /// Obtiene los procedimientos de determinados procesos
        /// </summary>
        /// <param name="procesos"></param>
        /// <returns></returns>
        public List<RAProcedimientoDC> ObtenerProcedimientosPorprocesos(string procesos)
        {
            List<RAProcedimientoDC> lstProcedimiento = new List<RAProcedimientoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringNovasoft))
            {
                SqlCommand cmd = new SqlCommand("paObtenerProcedimientosNOVA_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@procesos", procesos);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RAProcedimientoDC procedimiento = new RAProcedimientoDC
                    {
                        IdProcedimiento = reader["IdProcedimiento"].ToString(),
                        Descripcion = reader["Descripcion"].ToString()
                    };
                    lstProcedimiento.Add(procedimiento);
                }

                conn.Close();
            }
            return lstProcedimiento;
        }


        /// <summary>
        /// obtiene las notificaciones pendientes por usuario
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public List<RANotificacionDC> ListarNotificacionesPendientes(string idUsuario)
        {
            List<RANotificacionDC> lstNotificacion = new List<RANotificacionDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerNotificacionesPendientesPorUsuario_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RANotificacionDC notificacion = new RANotificacionDC
                    {
                        idNotificacion = Convert.ToInt32(reader["NPT_IdNotificacion"]),
                        Descripcion = reader["NPT_Mensaje"].ToString(),
                        fechaGrabacion = Convert.ToDateTime(reader["NPT_FechaGrabacion"].ToString()),
                        idSolicitud = Convert.ToInt64(reader["NPT_IdSolicitud"])
                    };
                    lstNotificacion.Add(notificacion);
                }
                conn.Close();
            }
            return lstNotificacion;
        }

        /// <summary>
        /// Lista todos los grupos creados
        /// </summary>
        /// <returns></returns>
        public List<RACargoGrupoDC> ListarGrupos()
        {
            List<RACargoGrupoDC> lstGrupos = new List<RACargoGrupoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paListarGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    RACargoGrupoDC grupo = new RACargoGrupoDC
                    {
                        IdGrupo = Convert.ToInt32(reader["GRP_IdGrupo"]),
                        DescripcionGrupo = reader["GRP_Descripcion"].ToString()
                    };
                    lstGrupos.Add(grupo);
                }
                conn.Close();
            }
            return lstGrupos;
        }

        /// <summary>
        /// Obtiene el cargo con sus respectivos cargos
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public RACargoGrupoDC ObtenerDetalleGrupo(int idGrupo, bool porPersona)
        {
            RACargoGrupoDC cargoDetalle = new RACargoGrupoDC();
            cargoDetalle.lstCargo = new List<RACargoDC>();
            using (SqlConnection conn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paObtenerDetalleGrupo_RAP", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idGrupo", idGrupo);
                cmd.Parameters.AddWithValue("@porPersona", porPersona);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    cargoDetalle.IdGrupo = idGrupo;
                    while (reader.Read())
                    {
                        cargoDetalle.DescripcionGrupo = reader["GRP_Descripcion"].ToString();
                        RACargoDC cargo = new RACargoDC
                        {
                            IdCargo = porPersona?reader["IdCargo"].ToString():"0",
                            CodigoCargo = reader["CodigoCargo"].ToString(),
                            Descripcion = reader["NombreCargo"].ToString(),
                            Identificacion = porPersona ? reader["Identificacion"].ToString() : "0",
                            NombrePersona = porPersona ? reader["NombrePersona"].ToString() : "0",
                            IdTerritorial = reader["IdTerritorial"].ToString() != "" ? Convert.ToInt16(reader["IdTerritorial"]):0,
                            IdRegional =  reader["IdRegional"].ToString(),
                            IdProceso =  reader["IdProceso"].ToString(),
                            NombreProceso = reader["NombreProceso"].ToString(),
                            Procedimiento =  reader["IdProcedimiento"].ToString(),
                            NombreProcedimiento = reader["NombreProcedimiento"].ToString()
                        };
                        if (!string.IsNullOrEmpty(cargo.IdCargo) || !porPersona)
                        {
                            cargoDetalle.lstCargo.Add(cargo);
                        }
                    }
                }
                conn.Close();
            }
            return cargoDetalle;
        }

        /// <summary>
        /// Obtiene el Parametro segun la key
        /// </summary>
        /// <returns>Lista estado</returns>
        public string ObtnerParametroConfiguracion(string key)
        {
            string valor = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosConfiguracion_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@llave", key);
                var resultReader = cmd.ExecuteReader();

                if (resultReader.HasRows)
                {
                    while (resultReader.Read())
                    {
                        valor = resultReader["Valor"].ToString();
                    }
                }

            }
            return valor;

        }
        #endregion


        #region Region Fallas / Web


        /// <summary>
        /// Metodo para obtener tipos de novedad segun responsable 
        /// </summary>
        /// <param name="idClaseResponsable"></param>
        /// <param name="idSistemaFuente"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTipoNovedadSegunResponsable(int idClaseResponsable, int idSistemaFuente)
        {
            List<RANovedadDC> tiposNovedad = null;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringRaps))
            {
                SqlCommand cmd = new SqlCommand("paConsultarTipoNovedadSegunClaseResponsableSistemaFuente_RAP", sqlConn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@IdClaseResponsable", idClaseResponsable));
                cmd.Parameters.Add(new SqlParameter("@IdSistemaOrigen", idSistemaFuente));

                sqlConn.Open();
                var resultReader = cmd.ExecuteReader();
                if (resultReader.HasRows)
                {
                    tiposNovedad = RARepositorioMapper.MapperTipoNovedad(resultReader);
                }
            }

            return tiposNovedad;
        }
        #endregion
    }
}