
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Raps.Comun;
using CO.Servidor.Raps.Datos;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Consultas;
using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Reglas;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.MotorReglas;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using CO.Servidor.RAPS.Reglas.ResponsablesManuales;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos;
using Newtonsoft.Json;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Dominio.Comun.Admisiones;

namespace CO.Servidor.Raps
{
    public class RASolicitudes : ControllerBase
    {
        private static readonly RASolicitudes instancia = (RASolicitudes)FabricaInterceptores.GetProxy(new RASolicitudes(), COConstantesModulos.MODULO_RAPS);
        IADFachadaAdmisionesMensajeria fachadaAdmisionMensajeria = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>();


        private string rutaFolderImagenesRAPS = "";
        #region singleton

        public static RASolicitudes Instancia
        {
            get { return RASolicitudes.instancia; }
        }

        public RASolicitudes()
        {
            rutaFolderImagenesRAPS = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenRAPS");
        }

        #endregion

        #region Fachadas
        private IPUFachadaCentroServicios fachadaCes = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        #endregion

        #region metodos

        /// <summary>
        /// Crea un gestion de una solicitud
        /// </summary>
        /// <param name="gestion"></param>
        /// <returns></returns>
        //public long CrearGestion(RAGestionDC gestion)
        //{
        //    return RARepositorioSolicitudes.Instancia.CrearGestion(gestion);
        //}

        /// <summary>
        /// obtiene las gestiones de una solicitud
        /// </summary>
        /// <param name="IdSolicitud"></param>
        /// <returns></returns>
        public List<RAGestionDC> ListarGestion(long IdSolicitud)
        {
            List<RAGestionDC> result = new List<RAGestionDC>();
            RAGestionDC ultimagestion = new RAGestionDC();
            //Nueva validación
            result = RARepositorioSolicitudes.Instancia.ListarGestion(IdSolicitud);

            if (result != null)
            {
                ultimagestion = RARepositorioSolicitudes.Instancia.ListarGestion(IdSolicitud).OrderByDescending(r => r.IdGestion).ToList().FirstOrDefault();

                if (string.IsNullOrWhiteSpace(ultimagestion.CodigoPlantaResponsable))
                {
                    ultimagestion.MessageError = "No se puede cargar la solicitud, porque el responsable no esta correctamente parametrizado en NOVASOFT.";
                }
            }

            return result;
        }

        /// <summary>
        /// obtiene la informacion  de un item de gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public RAGestionDC ObtenerGestion(long idGestion)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerGestion(idGestion);
        }

        /// <summary>
        /// Obtiene una plantilla de correo
        /// </summary>
        /// <param name="idPlantilla"></param>
        /// <returns></returns>
        public RAPantillaAccionCorreoDC ObtenerPantillaAccionCorreo(long idPlantilla)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerPantillaAccionCorreo(idPlantilla);
        }

        /// <summary>
        /// Crea una nueva solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        /// <returns></returns>
        public long CrearSolicitud(RASolicitudDC solicitud)
        {
            return RARepositorioSolicitudes.Instancia.CrearSolicitud(solicitud);
        }

        /// <summary>
        /// Listar solicitudes
        /// </summary>
        /// <returns></returns>
        public List<RASolicitudDC> ListarSolicitud()
        {
            return RARepositorioSolicitudes.Instancia.ListarSolicitud();
        }


        /// <summary>
        /// Crear adjunto
        /// </summary>
        /// <param name="adjunto"></param>
        /// <returns></returns>
        public bool CrearAdjunto(RAAdjuntoDC adjunto)
        {
            return RARepositorioSolicitudes.Instancia.CrearAdjunto(adjunto);
        }

        /// <summary>
        /// Lista los adjuntos de una gestion
        /// </summary>
        /// <param name="idGestion"></param>
        /// <returns></returns>
        public List<RAAdjuntoDC> ListarAdjunto(long idGestion)
        {
            return RARepositorioSolicitudes.Instancia.ListarAdjunto(idGestion);
        }

        /// <summary>
        /// Obtener una adjunto
        /// </summary>
        /// <param name="idAdjunto"></param>
        /// <returns></returns>
        public RAAdjuntoDC ObtenerAdjunto(long idAdjunto)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerAdjunto(idAdjunto);
        }


        #endregion

        #region Creacion solicitudes

        ///// <summary>
        ///// Registra una solicitud con su gestion y adjuntos
        ///// </summary>
        ///// <param name="solicitud"></param>
        ///// <param name="adjunto"></param>
        ///// <param name="gestion"></param>
        ///// <returns></returns>
        public long RegistrarSolicitud(RASolicitudDC solicitud, List<RAAdjuntoDC> adjunto, RAGestionDC gestion, Dictionary<string, object> parametrosParametrizacion, string usuario = "")
        {
            var resultado = true;
            List<RATiposDatosParametrizacionDC> lstParametros = new List<RATiposDatosParametrizacionDC>();
            DateTime fecha;
            long idSolicitud = 0;
            bool ingresoAValidacion = false;

            if (gestion.IdUsuario != null && gestion.IdUsuario != "MotorRaps")
            {

                string idciudadCol = fachadaCes.ObtenerCOLResponsable(ControllerContext.Current.IdCentroServicio).IdMunicipio;

                RAEscalonamientoDC escalonamiento = null;

                if (string.IsNullOrWhiteSpace(solicitud.DocumentoResponsable) || solicitud.DocumentoResponsable == "0")
                {
                    escalonamiento = RARepositorioSolicitudes.Instancia.ObtenerPersonaAsignarRap(solicitud.IdParametrizacionRap, idciudadCol, solicitud.idSucursal);
                }
                else
                {
                    escalonamiento = new RAEscalonamientoDC()
                    {
                        DocumentoEmpeladoEscalar = solicitud.DocumentoResponsable,
                    };

                    escalonamiento.CargoEscalar = ObtenerHorariosEmpleadoEscalarPorCargoSucursal(
                        new RACargoEscalarDC()
                        {
                            DocumentoEmpleado = escalonamiento.DocumentoEmpeladoEscalar
                        });
                }


                if (!string.IsNullOrWhiteSpace(escalonamiento.DocumentoEmpeladoEscalar) && escalonamiento.DocumentoEmpeladoEscalar != "0")
                {
                    ingresoAValidacion = true;
                    if (escalonamiento.IdTipoHora > 0 && escalonamiento.HorasEscalar > 0)
                    {
                        DateTime nuevaFechaVencimiento;
                        if (escalonamiento.IdTipoHora == 1)
                        {
                            nuevaFechaVencimiento = RAMotorRaps.Instancia.CalcularFechaVencimientoHorarioLaboral(escalonamiento.CargoEscalar, escalonamiento.HorasEscalar, solicitud.FechaVencimiento);
                        }
                        else
                        {
                            nuevaFechaVencimiento = DateTime.Now;
                            if (solicitud.FechaVencimiento != null && ((DateTime)solicitud.FechaVencimiento) > nuevaFechaVencimiento)
                            {
                                nuevaFechaVencimiento = nuevaFechaVencimiento.Add(((DateTime)solicitud.FechaVencimiento) - nuevaFechaVencimiento);
                            }
                            nuevaFechaVencimiento = nuevaFechaVencimiento.AddHours(escalonamiento.HorasEscalar);
                        }
                        solicitud.FechaVencimiento = nuevaFechaVencimiento;
                    }
                    gestion.FechaVencimiento = solicitud.FechaVencimiento;
                    solicitud.IdEstado = RAEnumEstados.Asignado;
                    solicitud.FechaCreacion = DateTime.Now;
                    solicitud.IdCargoResponsable = string.IsNullOrWhiteSpace(solicitud.IdCargoResponsable) ? escalonamiento.idCargo : solicitud.IdCargoResponsable;
                    solicitud.DocumentoResponsable = string.IsNullOrEmpty(solicitud.DocumentoResponsable) ? escalonamiento.DocumentoEmpeladoEscalar : solicitud.DocumentoResponsable;
                    solicitud.IdSolicitudPadre = 0;
                    solicitud.idSucursal = escalonamiento.CargoEscalar.Sucursal;
                    gestion.IdCargoGestiona = solicitud.IdCargoSolicita;
                    //gestion.CorreoEnvia = correoEnvia;
                    gestion.IdCargoDestino = solicitud.IdCargoResponsable;
                    gestion.CorreoDestino = escalonamiento.CargoEscalar.Correo;
                    //cod_pla
                    gestion.IdResponsable = string.IsNullOrWhiteSpace(solicitud.IdResponsable) ? gestion.IdCargoDestino : solicitud.IdResponsable;
                    gestion.DocumentoSolicita = solicitud.DocumentoSolicita;
                    gestion.DocumentoResponsable = solicitud.DocumentoResponsable;
                    //gestion.IdAccion = RAEnumAccion.Crear;
                    gestion.IdEstado = RAEnumEstados.Asignado;

                }


            }

            if (!ingresoAValidacion)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), "No se pudo insertar la solicitud, ya que no se encontro ningun empleado asociado a los cargos del escalonamiento"));



            using (TransactionScope transaccion = new TransactionScope())
            {

                // registrar la solicitud
                if (solicitud != null && gestion != null)
                {
                    idSolicitud = RARepositorioSolicitudes.Instancia.CrearSolicitud(solicitud);
                    gestion.IdSolicitud = idSolicitud;
                    var idGestion = RARepositorioSolicitudes.Instancia.CrearGestion(gestion);
                    //guardar adjunto
                    if (adjunto != null && adjunto.Any())
                    {
                        adjunto.ForEach(adj =>
                        {
                            adj.IdSolicitud = idSolicitud;
                            adj.IdGestion = idGestion;

                            string carpetaDestino = Path.Combine(string.Concat(rutaFolderImagenesRAPS, "\\", DateTime.Now.ToString("s").Substring(0, 10)));
                            if (!Directory.Exists(carpetaDestino))
                            {
                                Directory.CreateDirectory(carpetaDestino);
                            }
                            byte[] bytebuffer = Convert.FromBase64String(adj.Adjunto);
                            string ruta = string.Concat(carpetaDestino, "\\", adj.NombreArchivo, "_", idSolicitud, ".", adj.Extension);
                            adj.UbicacionNombre = ruta;
                            if (!RARepositorioSolicitudes.Instancia.CrearAdjunto(adj))
                                resultado = false;
                            else
                                File.WriteAllBytes(ruta, bytebuffer);
                        });
                    }
                    //INSERTAR PARAMETROS DE LA SOLICITUD
                    if (parametrosParametrizacion != null)
                    {
                        List<string> listNumber = parametrosParametrizacion.Select(p => p.Key).ToList();
                        lstParametros = ObtenerParamametroPorIdDeParametrizacion(solicitud.IdParametrizacionRap);

                        if (listNumber.Count == lstParametros.Count)
                        {
                            lstParametros.ForEach(parametro =>
                            {
                                if (parametrosParametrizacion.ContainsKey(parametro.IdTipoParametro.ToString()))
                                {
                                    bool correspondeTipoDato = false;
                                    switch (parametro.IdTipoDato)
                                    {
                                        case (int)RAEnumTipoDato.NUMERO:
                                            //parametrosParametrizacion[parametro.IdTipoDato.ToString()] is int
                                            if (parametrosParametrizacion[parametro.IdTipoParametro.ToString()].ToString().All(char.IsDigit))
                                                correspondeTipoDato = true;
                                            break;
                                        case (int)RAEnumTipoDato.CADENA:
                                            if (parametrosParametrizacion[parametro.IdTipoParametro.ToString()] is string)
                                                correspondeTipoDato = true;
                                            break;
                                        case (int)RAEnumTipoDato.FECHA:
                                            if (DateTime.TryParse(parametrosParametrizacion[parametro.IdTipoParametro.ToString()].ToString(), out fecha))
                                                correspondeTipoDato = true;
                                            break;
                                        case (int)RAEnumTipoDato.TIPONOVEDAD:
                                            if (!RARepositorio.Instancia.ConsultarTipoNovedad(Convert.ToInt32(parametrosParametrizacion[parametro.IdTipoParametro.ToString()]), parametro.IdTipoParametro))
                                                correspondeTipoDato = true;
                                            break;
                                        default:
                                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));

                                    }
                                    if (!correspondeTipoDato)
                                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));
                                    else
                                        RARepositorio.Instancia.InsertarParametroSolicitud(idSolicitud, parametro.IdTipoParametro, parametrosParametrizacion[parametro.IdTipoParametro.ToString()].ToString(), usuario);
                                }
                                else
                                {
                                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));
                                }
                            });
                        }
                    }
                    transaccion.Complete();
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD)));
                }

                transaccion.Dispose();
            }

            ///Notifica solo al cliente al que le fue asignada la solicitud
            RAMotorRaps.Instancia.NotificarClienteConectado(idSolicitud, solicitud.DocumentoResponsable, "Nuevo RAP Asignado. Id Solicitud: ", true);

            Task.Factory.StartNew(() =>
            {
                try
                {

                    ///Notifica a todos los cliente con el fin de que se refresquen los contadores
                    //RAMotorRaps.Instancia.NotificarTodosClientesConectados();

                    string correoDestino = RARepositorioSolicitudes.Instancia.ObtenerCorreoSegunIdCargo(gestion.IdCargoDestino);

                    StringBuilder sbRespuesta = ValidarAmbientePruebasEnviarCorreo();
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<b>NUEVO RAP</b>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("Descripción: " + solicitud.Descripcion);
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("Número de la solicitud: " + gestion.IdSolicitud);
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("Solicitante: " + gestion.NombreSolicita);
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("Responsable: " + gestion.NombreResponsable);
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("Fecha Vencimiento: <b>" + solicitud.FechaVencimiento.ToString("dd/MM/yyyy hh:mm tt") + "</b>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("<br>");
                    sbRespuesta.AppendLine("Tipo de solicitud:  <b>" + solicitud.Descripcion + "</b>");
                    CorreoElectronico.Instancia.Enviar(correoDestino, "Solicitud Creada", sbRespuesta.ToString());
                }
                catch (Exception ex)
                {
                    Utilidades.AuditarExcepcion(ex, true);
                }
            });
            return idSolicitud;
        }

        /// <summary>
        /// Obtiene el detalle de los parametros de una solicitud acumulativa
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAListaParametrosAcumulativasDC> ObtenerDetalleParametrosAcumulativas(long idsolicitud, long idParametrizacion)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerDetalleParametrosAcumulativas(idsolicitud, idParametrizacion);
        }

        /// <summary>
        /// Crea una solicitud tarea con su respectiva parametrizacion
        /// </summary>
        /// <param name="solicitud"></param>
        /// <param name="adjunto"></param>
        /// <param name="gestion"></param>
        /// <param name="parametrosParametrizacion"></param>
        /// <param name="parametrizacionRaps"></param>
        /// <param name="lstEscalonamiento"></param>
        /// <param name="lstTiempoEjecucion"></param>
        /// <param name="lstParametros"></param>
        /// <returns></returns>
        public long RegistrarSolicitudTarea(RASolicitudDC solicitud, List<RAAdjuntoDC> adjunto, RAGestionDC gestion, Dictionary<string, object> parametrosParametrizacion, RAParametrizacionRapsDC parametrizacionRaps,
            List<RAEscalonamientoDC> listEscalonamiento, List<RATiempoEjecucionRapsDC> listaTiempoEjecucion, List<RAParametrosParametrizacionDC> listaParametros, List<RAPersonaDC> lstPersonas, string usuario = "")
        {
            List<RATiposDatosParametrizacionDC> lstParametros = new List<RATiposDatosParametrizacionDC>();
            DateTime fecha;
            long idSolicitud = 0;
            long idParametrizacion = 0;
            parametrizacionRaps = new RAParametrizacionRapsDC();
            parametrizacionRaps.IdSistemaFuente = 0;
            parametrizacionRaps.IdProceso = 0; //validar
            parametrizacionRaps.UtilizaFormato = false;
            parametrizacionRaps.IdFormato = 0;
            parametrizacionRaps.IdTipoCierre = 2;
            parametrizacionRaps.IdTipoRap = 3;
            parametrizacionRaps.IdOrigenRaps = 1;
            parametrizacionRaps.Estado = true;
            parametrizacionRaps.IdCargoCierra = solicitud.IdCargoSolicita;
            parametrizacionRaps.IdGrupoUsuario = 0;
            parametrizacionRaps.IdSubclasificacion = 0;
            parametrizacionRaps.IdTipoPeriodo = 10;
            parametrizacionRaps.Nombre = solicitud.Descripcion;
            parametrizacionRaps.DescripcionRaps = solicitud.Descripcion;
            parametrizacionRaps.IdTipoEscalonamiento = solicitud.IdTipoEscalonamiento;

            using (TransactionScope transaccion = new TransactionScope())
            {
                idParametrizacion = RAConfiguracion.Instancia.CrearParametrizacionRaps(parametrizacionRaps, listEscalonamiento, listaTiempoEjecucion, listaParametros);
                solicitud.IdParametrizacionRap = idParametrizacion;
                lstPersonas.ForEach(p =>
                {
                    solicitud.DocumentoResponsable = p.NumeroDocumento.ToString();
                    solicitud.IdCargoResponsable = p.IdCargoNovasoft;
                    solicitud.IdResponsable = p.IdCargo;
                    solicitud.FechaInicio = p.FechaEjecucionTarea;
                    solicitud.FechaVencimiento = p.FechaEjecucionTarea;
                    gestion.Comentario = p.DescripcionTarea;
                    RegistrarSolicitud(solicitud, adjunto, gestion, parametrosParametrizacion);
                });

                transaccion.Complete();
            }
            return idParametrizacion;
        }



        /// <summary>
        /// Crea una solucitud de tipo acumulativa
        /// </summary>
        /// <param name="regSolicitudAcumulativa"></param>
        /// <returns></returns>
        [System.Obsolete()]
        public bool CrearSolicitudAcumulativa(int idSistema, int idTipoNovedad, Dictionary<string, object> parametros, string idCiudad)
        {

            bool resultado = false;
            List<RATiposDatosParametrizacionDC> lstParametros = new List<RATiposDatosParametrizacionDC>();
            DateTime fecha;
            string idSucursal = RARepositorio.Instancia.ObtenerSucursalNovasoft(idCiudad.Substring(0, 5));

            List<RASolicitudDC> listaParametrizaciones = RARepositorio.Instancia.ObtenerParametrizacionesSegunNovedad(idSistema, idTipoNovedad, idCiudad);
            var rutaImagenes = PAParametros.Instancia.ConsultarParametrosFramework("FoldImgFallaAPP");
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    listaParametrizaciones.ForEach(solicitud =>
                    {

                        long idSolicitudAcumulativa = RARepositorioSolicitudes.Instancia.CrearSolicitudAcumulativa(solicitud);
                        //insertar parametros
                        lstParametros = ObtenerParamametroPorIdDeParametrizacion(solicitud.IdParametrizacionRap);
                        lstParametros.ForEach(parametro =>
                        {
                            if (parametros.ContainsKey(parametro.IdTipoParametro.ToString()))
                            {
                                bool correspondeTipoDato = false;
                                switch (parametro.IdTipoDato)
                                {
                                    case (int)RAEnumTipoDato.NUMERO:
                                        if (parametros[parametro.IdTipoParametro.ToString()].ToString().All(char.IsDigit))
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.CADENA:
                                        if (parametros[parametro.IdTipoParametro.ToString()] is string)
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.FECHA:
                                        //parametros[parametro.IdTipoDato.ToString()] is DateTime
                                        if (DateTime.TryParse(parametros[parametro.IdTipoParametro.ToString()].ToString(), out fecha))
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.TIPONOVEDAD:
                                        if (!RARepositorio.Instancia.ConsultarTipoNovedad(Convert.ToInt32(parametros[parametro.IdTipoParametro.ToString()]), parametro.IdTipoParametro))
                                            correspondeTipoDato = true;
                                        break;
                                    case (int)RAEnumTipoDato.ADJUNTO:
                                        if (parametros[parametro.IdTipoParametro.ToString()].ToString() is string)
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(parametros[parametro.IdTipoParametro.ToString()].ToString()))
                                                {
                                                    parametros[parametro.IdTipoParametro.ToString()] = ObtenerRutaImagen(parametros[parametro.IdTipoParametro.ToString()].ToString(), rutaImagenes);
                                                }
                                                correspondeTipoDato = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                correspondeTipoDato = false;
                                            }
                                        }
                                        break;
                                    case (int)RAEnumTipoDato.FOTOGRAFIA:
                                        if (parametros[parametro.IdTipoParametro.ToString()].ToString() is string)
                                        {
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(parametros[parametro.IdTipoParametro.ToString()].ToString()))
                                                {
                                                    parametros[parametro.IdTipoParametro.ToString()] = ObtenerRutaImagen(parametros[parametro.IdTipoParametro.ToString()].ToString(), rutaImagenes);
                                                }
                                                correspondeTipoDato = true;
                                            }
                                            catch (Exception ex)
                                            {
                                                correspondeTipoDato = false;
                                            }
                                        }
                                        break;
                                    default:
                                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));

                                }
                                if (!correspondeTipoDato)
                                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));
                                else
                                {

                                    RARepositorio.Instancia.InsertarParametroSolicitudAcumulativa(idSolicitudAcumulativa, parametro.IdTipoParametro, parametros[parametro.IdTipoParametro.ToString()].ToString());
                                    resultado = true;
                                }
                            }
                            else
                            {
                                resultado = false;
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_PARAMETRO_NO_ES_EL_ESPERADO)));
                            }
                        });
                    });
                    trans.Complete();
                }
                catch (Exception ex)
                {
                   
                    Utilidades.AuditarExcepcion(ex, true);
                    throw ex;
                }
            }
            return resultado;
        }

       
        public string ObtenerRutaImagen(string imagen64, string rutaImagenes)
        {

            var carpetaDestino = Path.Combine(rutaImagenes + "\\" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            //foreach (var item in novedadConsolidado.AdjuntosConsolidado)
            //{
            //var nombreImagen = DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.ToShortTimeString().Replace(":", "_");
            var nombreImagen = DateTime.Now.ToString().Replace(":", "_").Replace(".", "_").Replace("/", "-");

            byte[] bytebuffer = Convert.FromBase64String(imagen64);
            MemoryStream memoryStream = new MemoryStream(bytebuffer);
            var image = Image.FromStream(memoryStream);
            ImageCodecInfo jpgEncoder = UtilidadesFW.GetEncoder(ImageFormat.Jpeg);
            var ruta = carpetaDestino + "\\" + nombreImagen + ".jpg";
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 180L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            image.Save(ruta, jpgEncoder, myEncoderParameters);
            //}

            return ruta;
        }



        /// <summary>
        /// Permite crear una gestion
        /// </summary>
        /// <param name="adjunto"></param>
        /// <param name="gestion"></param>
        /// <returns></returns>
        public bool CrearGestion(List<RAAdjuntoDC> adjunto, RAGestionDC gestion)
        {
            bool resultado = true;
            List<string> lstResponsablesNotifica = new List<string>();
            RAIdentificaEmpleadoDC empSolicita = RARepositorioSolicitudes.Instancia.ObtenerDatosEmpleado(gestion.DocumentoSolicita);
            RAIdentificaEmpleadoDC empResponsable = RARepositorioSolicitudes.Instancia.ObtenerDatosEmpleado(gestion.DocumentoResponsable);
            //RAGestionDC datosAdicionales = RARepositorioSolicitudes.Instancia.ObtenerDatosParaInsertarGestion_RAP(gestion.IdSolicitud, gestion.IdCargoGestiona);
            RASolicitudDC solicitud = RARepositorioSolicitudes.Instancia.ObtenerSolicitudRap(gestion.IdSolicitud);

            if (solicitud.Anchor != gestion.Anchor)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_SOL_CAMBIO_DE_ESTADO.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_SOL_CAMBIO_DE_ESTADO)));
            }

            RAParametrizacionRapsDC parametrizacion = RARepositorio.Instancia.ObtenerParametrizacionRaps(solicitud.IdParametrizacionRap);
            if (gestion.IdEstado == RAEnumEstados.Cerrado && solicitud.IdEstado != RAEnumEstados.Respuesta && solicitud.IdEstado != RAEnumEstados.Escalado && solicitud.IdCargoSolicita != "0")
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_NO_ES_POSIBLE_CERRAR_LA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_NO_ES_POSIBLE_CERRAR_LA_SOLICITUD)));
            }

            StringBuilder sbRespuesta = ValidarAmbientePruebasEnviarCorreo();
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<b>NUEVO RAP</b>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("Descripción: " + solicitud.Descripcion);
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("Número de la solicitud: " + gestion.IdSolicitud);
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("Solicitante: " + gestion.NombreSolicita);
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("Responsable: " + gestion.NombreResponsable);
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("Fecha Vencimiento: <b>" + solicitud.FechaVencimiento.ToString("dd/MM/yyyy hh:mm tt") + "</b>");
            sbRespuesta.AppendLine("<br>");
            sbRespuesta.AppendLine("<br>");
            //sbRespuesta.AppendLine("Tipo de solicitud:  <b>" + solicitud.Descripcion + "</b>");

            if (gestion.IdCargoGestiona == null)
            {
                gestion.IdCargoGestiona = empSolicita.IdCargo;
            }
            gestion.CorreoDestino = empResponsable.email;
            gestion.CorreoEnvia = empSolicita.email;
            gestion.IdResponsable = string.IsNullOrEmpty(empResponsable.CodigoPlanta) ? gestion.IdResponsable : empResponsable.CodigoPlanta;
            gestion.IdCargoDestino = string.IsNullOrWhiteSpace(gestion.IdCargoDestino) ? empResponsable.IdCargo : gestion.IdCargoDestino;
            int addMinutos = 0;
            addMinutos = int.TryParse(RARepositorio.Instancia.ObtnerParametroConfiguracion("TiempoFechaVencimientoGestion"), out addMinutos) ? addMinutos : 120;
            gestion.FechaVencimiento = DateTime.Now.AddMinutes(addMinutos);

            if (gestion.IdEstado == RAEnumEstados.Respuesta && parametrizacion.IdTipoCierre == 1)
            {
                string correoDestino = RARepositorioSolicitudes.Instancia.ObtenerCorreoSegunIdCargo(parametrizacion.IdCargoCierra);
                gestion.CorreoDestino = String.IsNullOrEmpty(correoDestino) ? "" : correoDestino;
                gestion.IdCargoDestino = parametrizacion.IdCargoCierra;
            }

            if (solicitud.IdCargoSolicita == "0")
            {
                gestion.CorreoDestino = "No Aplica por Cierre";
                gestion.CorreoEnvia = "No Aplica por Cierre";
                gestion.IdResponsable = string.IsNullOrEmpty(solicitud.IdResponsable) ? gestion.IdResponsable : solicitud.IdResponsable;
                gestion.IdCargoDestino = solicitud.IdCargoResponsable;
                gestion.DocumentoResponsable = solicitud.DocumentoResponsable;
                gestion.FechaVencimiento = DateTime.Now;
            }

            if (gestion.IdEstado == RAEnumEstados.Respuesta)
            {
                StringBuilder sbCierre = ValidarAmbientePruebasEnviarCorreo();
                sbCierre.AppendLine("<br>");
                sbCierre.AppendLine("<b>Respuesta de RAP creado</b>");
                sbCierre.AppendLine("<br>");
                sbCierre.AppendLine("<br>");
                sbCierre.AppendLine("Respuesta: " + gestion.Comentario);
                sbCierre.AppendLine("<br>");
                sbCierre.AppendLine("<br>");
                sbCierre.AppendLine("Número de la solicitud: " + gestion.IdSolicitud);
                sbCierre.AppendLine("<br>");
                sbCierre.AppendLine("<br>");
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CorreoElectronico.Instancia.Enviar(gestion.CorreoDestino, "Respuesta a RAP Creado", sbCierre.ToString());
                    }
                    catch (Exception ex)
                    {
                        Utilidades.AuditarExcepcion(ex, true);
                    }
                });

            }
            else if (gestion.IdEstado == RAEnumEstados.Rechazado)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CorreoElectronico.Instancia.Enviar(gestion.CorreoDestino, "Rechazo de RAP Creado", sbRespuesta.ToString());
                    }
                    catch (Exception ex)
                    {
                        Utilidades.AuditarExcepcion(ex, true);
                    }
                });
            }

            else if (gestion.IdEstado == RAEnumEstados.Reasignado)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        CorreoElectronico.Instancia.Enviar(gestion.CorreoDestino, "Reasignacion de RAP", sbRespuesta.ToString());
                    }
                    catch (Exception ex)
                    {
                        Utilidades.AuditarExcepcion(ex, true);
                    }
                });
            }

            else if (gestion.IdEstado == RAEnumEstados.Cerrado)
            {
                gestion.CorreoDestino = "No Aplica";
                gestion.CorreoEnvia = "No Aplica";
                //gestion.IdResponsable = "";
                //gestion.IdCargoDestino = "";
                //gestion.DocumentoResponsable = "No Aplica";
                gestion.FechaVencimiento = DateTime.Now;
            }

            lstResponsablesNotifica = RARepositorioSolicitudes.Instancia.ObtenerResponsablesDeSolicitud(gestion.IdSolicitud, gestion.DocumentoResponsable);

            lstResponsablesNotifica.ForEach(r =>
            {
                RAMotorRaps.Instancia.NotificarClienteConectado(solicitud.IdSolicitud, r, "Se ha gestionado la solicitud: ", true);
            });


            //RAMotorRaps.Instancia.NotificarTodosClientesConectados();
            using (TransactionScope trans = new TransactionScope())
            {
                var idGestion = RARepositorioSolicitudes.Instancia.CrearGestion(gestion);
                RARepositorioSolicitudes.Instancia.ActualizarSolicitud(gestion.IdSolicitud, gestion.IdEstado, gestion.DocumentoResponsable);
                //guardar adjunto
                if (adjunto != null && adjunto.Any())
                {
                    adjunto.ForEach(adj =>
                    {
                        adj.IdSolicitud = gestion.IdSolicitud;
                        adj.IdGestion = idGestion;
                        string rutaAdjuntos = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenRAPS");
                        string carpetaDestino = Path.Combine(string.Concat(rutaAdjuntos, "\\", DateTime.Now.ToString("s").Substring(0, 10)));
                        if (!Directory.Exists(carpetaDestino))
                        {
                            Directory.CreateDirectory(carpetaDestino);
                        }
                        byte[] bytebuffer = Convert.FromBase64String(adj.Adjunto);
                        string ruta = string.Concat(carpetaDestino, "\\", adj.NombreArchivo, "_", gestion.IdSolicitud, ".", adj.Extension);
                        adj.UbicacionNombre = ruta;
                        if (!RARepositorioSolicitudes.Instancia.CrearAdjunto(adj))
                            resultado = false;
                        else
                            File.WriteAllBytes(ruta, bytebuffer);
                    });
                }
                trans.Complete();
            }
            return resultado;
        }

      


       


        #endregion

        #region gestion

        public bool ResponderSolicitudesRaps(RAGestionDC gestion)
        {
            //validar informacion de solicitante 
            var solicitante = RARepositorioSolicitudes.Instancia.ConsultaInformacionSolicitante(gestion.IdSolicitud);
            if (solicitante == null)
            {

                //TODO: consumir metodo de escalonamiento para poder registrar respuesta 
                //      recordar actualizar objeto gestion segun sea el caso
                return false; // FaultException("requiere Escalar implementacion pendiente");
            }
            using (TransactionScope transaction = new TransactionScope())
            {

                var resultadoGestion = RARepositorioSolicitudes.Instancia.CrearGestion(gestion) > 0;


                var solicitud = new RASolicitudDC
                {
                    IdSolicitud = gestion.IdSolicitud,
                    IdCargoSolicita = gestion.IdCargoDestino,
                    IdCargoResponsable = gestion.IdResponsable,
                    FechaVencimiento = gestion.FechaVencimiento,
                    IdEstado = gestion.IdEstado,
                };

                var resultadoActualizaSolicitud = RARepositorio.Instancia.ActualizaSolicitud(solicitud);

                transaction.Complete();
            }

            return true;

        }

        public List<RAConteoEstadosSolicitante> ObtenerConteoEstadosSolicitudes(string idDocumentoSolicita)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerConteoEstadosSolicitudes(idDocumentoSolicita);
        }


        #endregion

        #region ObtenerListaSolicitudesRaps
        public List<RAObtenerListaSolicitudesRaps> ObtenerListaSolicitudesRaps(long DocumentoSolicita, int IdEstado)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerListaSolicitudesRaps(DocumentoSolicita, IdEstado);
        }
        #endregion

        #region Escalonamiento

        /// <summary>
        /// Obtiene los horarios del empleado para el cual se realizara el escalamiento de un rap
        /// </summary>
        /// <param name="idCargo"></param>
        /// <param name="idSucursal"></param>
        public RACargoEscalarDC ObtenerHorariosEmpleadoEscalarPorCargoSucursal(RACargoEscalarDC cargoEscalar)
        {
            return RARepositorio.Instancia.ObtenerHorariosEmpleadoEscalarPorCargoSucursal(cargoEscalar);

        }



        #endregion

        #region consultas
        /// <summary>
        /// Metodo para obtener los parametro por una parametrizacion especifica
        /// </summary>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RATiposDatosParametrizacionDC> ObtenerParamametroPorIdDeParametrizacion(long idParametrizacion)
        {
            return RARepositorio.Instancia.ObtenerParamametroPorIdDeParametrizacion(idParametrizacion);
        }


        /// <summary>
        /// Lista los parametros raps activos por tipo raps
        /// </summary>
        /// <param name="idTipoRap"></param>
        /// <returns></returns>
        public IEnumerable<RAParametrizacionRapsDC> ListarParametroRapXTipoRapAct(int idTipoRap)
        {
            return RARepositorioSolicitudes.Instancia.ListarParametroRapXTipoRapAct(idTipoRap);
        }

        /// <summary>
        /// Obtiene los tipod de novedad segun el sistema origen
        /// </summary>
        /// <param name="idSistemaOrigen"></param>
        /// <returns></returns>
        public List<RANovedadDC> ObtenerTiposNovedad(int idSistemaOrigen, int idTipoNovedad)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerTiposNovedad(idSistemaOrigen, idTipoNovedad);
        }

        /// <summary>
        /// Retorna las veces que esta un tipo de novedad en parametrizaciones activas
        /// </summary>
        /// <param name="idTipoNovedad">Id de la novedad</param>
        /// <returns></returns>
        public RANovedadDC ObtenerCantidadTiposNovedad(long idTipoNovedad)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerCantidadTiposNovedad(idTipoNovedad);
        }

        /// <summary>
        /// Lista simplificada de las solicitudes por estado
        /// </summary>
        /// <param name="responsableSolicitud"></param>
        /// <param name="estadoSolicitud"></param>
        /// <returns></returns>
        public List<RASolicitudItemDC> ListarSolicitudes(string responsableSolicitud, RAEnumEstados estadoSolicitud)
        {
            return RARepositorioSolicitudes.Instancia.ListarSolicitudes(responsableSolicitud, estadoSolicitud);
        }

        /// <summary>
        /// Consulta una solicitud
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public RASolicitudConsultaDC ObtenerSolicitud(long idSolicitud)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerSolicitud(idSolicitud);
        }

        /// <summary>
        /// Obtiene las personas asociadas a una sucursal y un grupo especifico
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public List<RAIdentificaEmpleadoDC> ObtenerEmpleadosPorGrupoYSucursal(int IdGrupo, int IdSucursal)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerEmpleadosPorGrupoYSucursal(IdGrupo, IdSucursal);
        }

        /// <summary>
        /// Valida si es un ambiente de pruebas y retorna un StringBuilder con el encabezado de pruebas
        /// </summary>
        /// <returns></returns>
        private static StringBuilder ValidarAmbientePruebasEnviarCorreo()
        {
            StringBuilder sb = new StringBuilder();
            if (Convert.ToBoolean(PAParametros.Instancia.ConsultarParametrosFramework("EsAmbientePruebas")))
            {
                sb.AppendLine("<br>");
                sb.AppendLine("<big><b>------------------------------------------</b></big>");
                sb.AppendLine("<br>");
                sb.AppendLine("<big><b>***************** CORREO DE PRUEBA ! IGNORAR ! *****************</b></big>");
                sb.AppendLine("<br>");
                sb.AppendLine("<big><b>------------------------------------------</b></big>");
                sb.AppendLine("<br>");
            }
            return sb;
        }

        /// <summary>
        /// Obtiene las fallas cometidas por un mensajero en el dia anterior al actual
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RAFallaMensajeroDC> ObtenerReporteFallasPorMensajero(string idMensajero)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerReporteFallasPorMensajero(idMensajero);
        }

        /// <summary>
        /// Obtiene el responsable asignado para responder los raps de las fallas causadas por determinado centro de servicio Modi
        /// </summary>
        /// <param name="identificacionResponsableFalla"></param>
        /// <returns></returns>
        public RACargoEscalarDC ObtenerResponsableCentroServicioParaAsignarRaps(string identificacionResponsableFalla)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerResponsableCentroServicioParaAsignarRaps(identificacionResponsableFalla);
        }

        /// <summary>
        /// Obtiene el horario de un empleado por su numero de identificacion
        /// </summary>
        /// <param name="documentoEmpleado"></param>
        /// <returns></returns>
        public List<RAHorarioEmpleadoDC> ObtenerHorariosEmpleadoPorIdentificacion(string documentoEmpleado)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerHorariosEmpleadoPorIdentificacion(documentoEmpleado);
        }

        #endregion

        #region FallasMensajero

        public List<RASolucitudesAutomaticasDC> ObtenerTodasFallasPorMensajero(string idresponsable, string idestado)
        {
            return RARepositorioSolicitudes.Instancia.ObtenerTodasFallasPorMensajero(idresponsable, idestado);
        }
        public RADetalleFallasMensajeroDC ObtenerDetalleFallasMensajero(string idmensajero, string idresponsable, string idSucursal, int estado)
        {

            RADetalleFallasMensajeroDC result = new RADetalleFallasMensajeroDC();

            List<Task> lstTareas = new List<Task>();

            lstTareas.Add(Task.Factory.StartNew(() =>
            {
                result.Encabezado = RARepositorioSolicitudes.Instancia.ObtenerEstadoActualSolicitud(idmensajero);

            }));


            lstTareas.Add(Task.Factory.StartNew(() =>
            {
                result.FallasMensajero = RARepositorioSolicitudes.Instancia.ObtenerFallasMensajero(idmensajero, idresponsable, estado);

            }));

            Task.WaitAll(lstTareas.ToArray());

            result.ComentarioGestion = RARepositorio.Instancia.ObtenerComentarioDeGestion(result.FallasMensajero.FirstOrDefault().IdSolicitud);
            result.AdjuntosGestion = RARepositorioSolicitudes.Instancia.ListarAdjunto(result.FallasMensajero.FirstOrDefault().IdSolicitud);

            long idParametrizacionPadre = RARepositorioSolicitudes.Instancia.ObtenerIdParametrizacionPadrePorIdHijo(result.FallasMensajero.FirstOrDefault().IdParametrizacion);
            List<RACargoEscalonamientoDC> seguimiento = RARepositorioSolicitudes.Instancia.ObtenerEscalonamiento(idParametrizacionPadre, result.FallasMensajero[0].IdSolicitud);

            RAPersonaDC segundoResponsable = new RAPersonaDC();
            foreach (var item in seguimiento)
            {
                if (!string.IsNullOrEmpty(segundoResponsable.NombreCompleto))
                    break;

                segundoResponsable = RARepositorioSolicitudes.Instancia.ObtenerProcesoPorPersona(item.IdCargo, idSucursal, item.IdProceso, item.IdProcedimiento);
            }
            result.Encabezado.ProcesoResponsable = segundoResponsable.Proceso;
            result.Encabezado.IdentificacionResponsable = segundoResponsable.NumeroDocumento.ToString();
            result.Encabezado.NombreResponsable = segundoResponsable.NombreCompleto;
            result.Encabezado.TelefonoResponsable = segundoResponsable.Telefono;

            return result;
        }

        /// <summary>
        /// Obtiene el detalle de la solicitud acumulativa a partir de la solicitud creada y la parametrizacion
        /// </summary>
        /// <param name="idsolicitud"></param>
        /// <param name="idParametrizacion"></param>
        /// <returns></returns>
        public List<RGDictionary> ObtenerDetalleSolicitudesAcumulativas(long idsolicitud, long idParametrizacion)
        {
            List<RGDictionary> result = new List<RGDictionary>();
            var rs = RARepositorioSolicitudes.Instancia.ObtenerDetalleSolicitudesAcumulativas(idsolicitud, idParametrizacion);
            string indexed = string.Empty;

            foreach (RGDictionary item in rs)
            {
                var currentItem = item.Name;
                if (indexed.IndexOf(item.Name) > -1) continue;
                var currentDetail = "";

                foreach (RGDictionary pivot in rs)
                {
                    if (pivot.Name == currentItem)
                    {
                        currentDetail += pivot.Value + ",";
                    }

                }

                result.Add(new RGDictionary()
                {
                    Name = currentItem,
                    Value = currentDetail.Remove(currentDetail.Length - 1, 1)
                });

                indexed += currentItem + ",";
            }

            return result;
        }

        #endregion




        /// <summary>
        /// obtiene responsable con parametros visibles
        /// </summary>
        /// <param name="idTipoNovedad"></param>
        /// <param name="numeroGuia"></param>
        /// <param name="estadoOrigen"></param>
        /// <returns></returns>
        public bool CrearFallaPersonalizadaRaps(RegistroSolicitudAppDC regSolicitud)
        {
            ADGuia datosGuia = null;
            IDictionary<string, object> parametrosRegla = null;


            if (regSolicitud.NumeroGuia != 0)
            {
                /********************* VALIDA EXISTENCIA GUIA *************************/
                try
                {
                    datosGuia = fachadaAdmisionMensajeria.ObtenerGuiaXNumeroGuia(regSolicitud.NumeroGuia);
                    parametrosRegla = new Dictionary<string, object>();
                    parametrosRegla.Add("guia", datosGuia);

                }
                catch (Exception ex)
                {
                    throw new FaultException<ControllerException>(new ControllerException(Framework.Servidor.Comun.COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_CONSULTA_GUIA_NO_EXISTE)));
                }


                /***********************ASIGNA ESTADO ORIGEN SI  ES DIFERENTE DE CERO********************************/
                if (regSolicitud.EstadoOrigen != 0)
                {
                    datosGuia.EstadoGuia = (ADEnumEstadoGuia)regSolicitud.EstadoOrigen;
                }
                /****************************OBTIENE REGLA DE ACUERDO AL ESTADO DE LA GUIA**********************************/
                RAReglasIngrecionesManualDC regla = RAIntegracionRaps.Instancia.ObtenerReglasIntegracionesManual((int)datosGuia.EstadoGuia);

                if (regla == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_NO_EXISTE_REGLA_PARA_ESTADO)));                    
                }


                /**************************************** EJECUTA REGLA **************************************************/
                RAResponsableDC resultado = new RAResponsableDC();
                resultado = AdministradorReglasRaps.EjecutarRegla(regla.Assembly, regla.NameSpace, regla.Clase, parametrosRegla);

                if (resultado == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, RAEnumTipoErrorClientes.EX_CONSULTA_SOLICITUD.ToString(), RAMensajesRaps.CargarMensaje(RAEnumTipoErrorClientes.EX_NO_EXISTE_REGLA_PARA_ESTADO)));
                }

                regSolicitud.IdResponsable = resultado.IdResponsableFalla;
                regSolicitud.DatosReponsable = new RAResponsableDC()
                {
                    Id = resultado.Id,
                    IdentificacionResponsable = resultado.IdentificacionResponsable,
                    Nombre = resultado.Nombre,
                };
                /****************************************OBTIENE PARAMETROS VISIBLES*********************************************/
                regSolicitud.Guia = datosGuia;
            }
            else
            {
                regSolicitud.IdResponsable = regSolicitud.IdOrigenRaps;
            }


            return RASolicitud.Instancia.CrearSolicitudAcumulativaPersonalizada(regSolicitud);
        }




    }
}
