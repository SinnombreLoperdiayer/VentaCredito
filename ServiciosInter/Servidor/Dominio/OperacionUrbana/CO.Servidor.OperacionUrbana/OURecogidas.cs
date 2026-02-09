using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.OperacionUrbana.Comun;
using CO.Servidor.OperacionUrbana.Datos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using System.Data.SqlClient;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;

namespace CO.Servidor.OperacionUrbana
{
    internal class OURecogidas : ControllerBase
    {
        private static readonly OURecogidas instancia = (OURecogidas)FabricaInterceptores.GetProxy(new OURecogidas(), COConstantesModulos.MODULO_OPERACION_URBANA);

        /// <summary>
        /// Retorna una instancia de
        /// recogidas
        /// /// </summary>
        public static OURecogidas Instancia
        {
            get { return OURecogidas.instancia; }
        }

        #region Programacion

        /// <summary>
        /// Obtiene los estados de la solicitud de recogida
        /// </summary>
        /// <returns></returns>
        public List<OUEstadosSolicitudRecogidaDC> ObtenerEstadosRecogida()
        {
            return OURepositorio.Instancia.ObtenerEstadosRecogida();
        }

        /// <summary>
        /// Obtiene las recogidas de la agencia
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idAgencia"></param>
        /// <param name="incluyeFechaAsignacion"></param>
        /// <param name="incluyeFechaRecogida"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidas(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idAgencia, bool incluyeFechaAsignacion, bool incluyeFechaRecogida)
        {
            List<OURecogidasDC> lst = OURepositorio.Instancia.ObtenerRecogidas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idAgencia, incluyeFechaAsignacion, incluyeFechaRecogida);
            //lst = lst.GroupBy(r => r.IdRecogida).Select(r => r.First()).OrderByDescending(r => r.IdRecogida).ToList();
            return lst;
        }

        /// <summary>
        /// Guarda la solicitud de recogida del punto de servicios
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardaSolicitudRecogidaPuntoSvc(OURecogidasDC recogida)
        {
            if (recogida.FechaSolicitud.Hour >= int.Parse(OURepositorio.Instancia.ObtenerParametroOperacionUrbana(OUConstantesOperacionUrbana.HORA_LIMITE_PROGRAMACION_RECOGIDAS)))
                if (recogida.FechaRecogida.Date == recogida.FechaSolicitud.Date)
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                        recogida.FechaRecogida = DateTime.Now.AddDays(2);
                    else
                        recogida.FechaRecogida = DateTime.Now.AddDays(1);
                }

            if (recogida.FechaRecogida.DayOfWeek == DayOfWeek.Sunday)
                recogida.FechaRecogida = recogida.FechaRecogida.AddDays(1);
            recogida.NombreCliente = recogida.PuntoServicio.Nombre;
            OURepositorio.Instancia.GuardaSolicitudRecogidaPuntoSvc(recogida);
        }

        /// <summary>
        /// Guarda la solicitud de la recogida por cliente convenio
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarSolicitudClienteConvenio(OURecogidasDC recogida)
        {
            if (recogida.FechaSolicitud.Hour >= int.Parse(OURepositorio.Instancia.ObtenerParametroOperacionUrbana(OUConstantesOperacionUrbana.HORA_LIMITE_PROGRAMACION_RECOGIDAS)))
                if (recogida.FechaRecogida.Date == recogida.FechaSolicitud.Date)
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                        recogida.FechaRecogida = DateTime.Now.AddDays(2);
                    else
                        recogida.FechaRecogida = DateTime.Now.AddDays(1);
                }

            if (recogida.FechaRecogida.DayOfWeek == DayOfWeek.Sunday)
                recogida.FechaRecogida = recogida.FechaRecogida.AddDays(1);
            recogida.NombreCliente = recogida.Cliente.RazonSocial;
            OURepositorio.Instancia.GuardarSolicitudClienteConvenio(recogida);
        }

        //public string GuardarSolicitudRecogidaASMX(string tipoDocumento, string documento, string nombre, string direccion, string telefono, string idciudad, string ciudad, string fecharecogida, string peso, string cantpiezas, string tipoenvio, string descripenvio, string creadopor)
        //{
        //    int cantidadpiezas = Convert.ToInt32(cantpiezas);

        //    if (ControllerContext.Current == null)
        //        ControllerContext.Current = new ControllerContext();


        //    ControllerContext.Current.Usuario = creadopor;

        //    OURecogidasDC recogida = new OURecogidasDC
        //    {
        //            FechaRecogida = Convert.ToDateTime(fecharecogida),
        //            PersonaSolicita = nombre,
        //            Contacto = nombre,
        //            Observaciones = null,
        //            NombreCliente = nombre,
        //            PersonaRecepcionoRecogida = nombre,

        //            RecogidaPeaton = new OURecogidaPeatonDC
        //        {
        //            TipoIdentificacion = new PATipoIdentificacion 
        //            {
        //                IdTipoIdentificacion = tipoDocumento
        //            },
        //            DocumentoCliente = documento,
        //            TelefonoCliente = telefono,
        //            DireccionCliente = direccion,
        //        },


        //    };

        //    recogida.RecogidaPeaton.EnviosRecogida = new List<OUEnviosRecogidaPeatonDC>();

        //    for (int i = 1; i <= cantidadpiezas; i++)
        //    {
        //        recogida.RecogidaPeaton.EnviosRecogida.Add(new OUEnviosRecogidaPeatonDC
        //        {
        //            CantidadEnvios = Convert.ToInt16(i),
        //            MunicipioDestino = new PALocalidadDC { IdLocalidad = idciudad, Nombre = ciudad },
        //            PesoAproximado = Convert.ToDecimal(peso),
        //            TipoEnvio = new Servicios.ContratoDatos.Tarifas.TATipoEnvio
        //            {
        //                IdTipoEnvio = Convert.ToInt16(tipoenvio),
        //                Descripcion = descripenvio,
        //            },


        //        });

        //    }

        //    return GuardaSolicitudClientePeaton(recogida).ToString();
        //}

        /// <summary>
        /// Guarda la solicitud de recogida del cliente peaton
        /// </summary>
        /// <param name="recogida"></param>
        public long GuardaSolicitudClientePeaton(OURecogidasDC recogida)
        {


            if (recogida.TipoOrigenRecogida == OUEnumTipoOrigenRecogida.CON)
                recogida.EstadoRecogida = new OUEstadosSolicitudRecogidaDC { IdEstado = (short)OUEnumEstadoSolicitudRecogidas.IN_PENDIENTE_COORDENADAS };
            else
            {

                if ((string.IsNullOrEmpty(recogida.LatitudRecogida) || (string.IsNullOrEmpty(recogida.LongitudRecogida))))
                    recogida.EstadoRecogida = new OUEstadosSolicitudRecogidaDC { IdEstado = (short)OUEnumEstadoSolicitudRecogidas.IN_PENDIENTE_COORDENADAS };
                else
                    recogida.EstadoRecogida = new OUEstadosSolicitudRecogidaDC { IdEstado = (short)OUEnumEstadoSolicitudRecogidas.IN_DISPONIBLE };
            }


            if (recogida.FechaSolicitud.Hour >= int.Parse(OURepositorio.Instancia.ObtenerParametroOperacionUrbana(OUConstantesOperacionUrbana.HORA_LIMITE_PROGRAMACION_RECOGIDAS)))
                if (recogida.FechaRecogida.Date == recogida.FechaSolicitud.Date)
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                        recogida.FechaRecogida = DateTime.Now.AddDays(2);
                    else
                        recogida.FechaRecogida = DateTime.Now.AddDays(1);
                }

            if (recogida.FechaRecogida.DayOfWeek == DayOfWeek.Sunday)
                recogida.FechaRecogida = recogida.FechaRecogida.AddDays(1);

            /*  recogida.PesoAproximado = 0;
              recogida.CantidadEnvios = 0;
              foreach (OUEnviosRecogidaPeatonDC envios in recogida.RecogidaPeaton.EnviosRecogida)
              {
                  recogida.CantidadEnvios += envios.CantidadEnvios;
                  recogida.PesoAproximado += envios.PesoAproximado;
                  if (envios.MunicipioDestino == null)
                  {
                      envios.MunicipioDestino = new PALocalidadDC();
                      envios.MunicipioDestino.IdLocalidad = null;
                  }
              }*/

            recogida.NombreCliente = recogida.PersonaSolicita;
            PUCentroServiciosDC centro = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(recogida.LocalidadRecogida.IdLocalidad);
            if (centro != null)
            {
                recogida.IdAgenciaResponsable = centro.IdCentroServicio;
            }

            long idSolicitudRecogida = OURepositorio.Instancia.GuardaSolicitudClientePeaton(recogida);
            if (recogida.Fotografias != null)
            {
                OURepositorio.Instancia.GuardarFotografiasSolicitudRecogidaPeaton(recogida.Fotografias, idSolicitudRecogida);
            }
            return idSolicitudRecogida;
        }

        /// <summary>
        /// Actualiza la informacion de una recogida de peaton
        /// </summary>
        /// <param name="recogida"></param>
        public void ActualizaSolicitudClientePeaton(OURecogidasDC recogida)
        {
            OURepositorio.Instancia.ActualizaSolicitudClientePeaton(recogida);

            if (recogida.EstadoRecogida.IdEstado == (int)OUEnumEstadoSolicitudRecogidas.IN_PENDIENTE_COORDENADAS && !string.IsNullOrWhiteSpace(recogida.LongitudRecogida))
                OURepositorio.Instancia.ActualizarEstadoSolicitudRecogida(recogida.IdRecogida.Value, OUEnumEstadoSolicitudRecogidas.IN_DISPONIBLE);
        }

        /// <summary>
        /// consulta direcciones historico recogidas por documento
        /// </summary>
        /// <param name="SolicitudRecogidaPeaton"></param>
        public List<OURecogidasDC> ObtenerDireccionesPeaton(OURecogidaPeatonDC Peaton)
        {
            return OURepositorio.Instancia.ObtenerDireccionesPeaton(Peaton);
        }

        /// <summary>
        /// Metodo para obtener ultima informacion del usuario externo registrada
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionRecogidaUsuarioExterno(string nomUsuario)
        {
            return OURepositorio.Instancia.ObtenerInformacionRecogidaUsuarioExterno(nomUsuario);
        }

        /// <summary>
        /// Consulta la solicitud de recogida para un cliente peaton especifico
        /// </summary>
        /// <returns></returns>
        public OURecogidasDC ObtenerInformacionClientePeaton(OURecogidasDC infoRecogida)
        {
            return OURepositorio.Instancia.ObtenerInformacionClientePeaton(infoRecogida);
        }

        #endregion Programacion

        #region Programacion Recogidas

        #region Consultas

        /// <summary>
        /// Retorna los motivos de la Reprogramacion
        /// </summary>
        /// <returns></returns>
        public List<OUMotivosReprogramacionDC> ObtenerMotivosReprogramacion()
        {
            return OURepositorio.Instancia.ObtenerMotivosReprogramacion();
        }

        /// <summary>
        /// Retorna la recogida por punto de servicio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPuntoServicio(long idSolicitud)
        {
            return OURepositorio.Instancia.ObtenerRecogidaPuntoServicio(idSolicitud);
        }

        /// <summary>
        /// Retorna la recogida del cliente convenio
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaConvenio(long idSolicitud)
        {
            return OURepositorio.Instancia.ObtenerRecogidaConvenio(idSolicitud);
        }



        /// <summary>
        /// Retorna la recogida peaton
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public OURecogidasDC ObtenerRecogidaPeaton(long idSolicitud)
        {
            return OURepositorio.Instancia.ObtenerRecogidaPeaton(idSolicitud);
        }



        /// <summary>
        /// Retorna el historico de la programacion de la recogida
        /// </summary>
        /// <param name="idSolicitud"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerProgramacionRecogidas(long idSolicitud)
        {
            List<OUProgramacionSolicitudRecogidaDC> lst = OURepositorio.Instancia.ObtenerProgramacionRecogidas(idSolicitud);

            return lst;
        }

        /// <summary>
        /// Obtiene las planillas de recogidas creadas para el tipo de mensajero, zona y fecha de recogidas
        /// </summary>
        /// <param name="idZona">id de la zona</param>
        /// <param name="idTipoMensajero">id del tipo de mensajero</param>
        /// <param name="fechaRecogida">fecha de recogida</param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidaZonaTipoMenFecha(string idZona, int idTipoMensajero, DateTime fechaRecogida, long idCol)
        {
            return OURepositorio.Instancia.ObtenerPlanillasRecogidaZonaTipoMenFecha(idZona, idTipoMensajero, fechaRecogida, idCol);
        }

        #endregion Consultas

        #region Insercion

        /// <summary>
        /// Actualiza el reporte de la recogida al mensajero
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaReporteMensajero(OUProgramacionSolicitudRecogidaDC programacion)
        {
            OURepositorio.Instancia.ActualizaReporteMensajero(programacion);
        }

        /// <summary>
        /// Actualiza la solicitud de la recogida y la planilla
        /// </summary>
        /// <param name="programacion"></param>
        public void ActualizaSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {

            using (TransactionScope scope = new TransactionScope())
            {
                if (programacion.IdPlanillaRecogida == null || programacion.IdPlanillaRecogida <= 0)
                    programacion.Estado = OUConstantesOperacionUrbana.ESTADO_NO_PLANILLADA;
                else
                    programacion.Estado = OUConstantesOperacionUrbana.ESTADO_PLANILLADA;

                ///si la solicitud tiene programacion hace la reprogramación
                if (programacion.Recogida.EstaProgramada)
                {
                    if (programacion.MotivoReprogramacion == null)
                    {
                        programacion.MotivoReprogramacion = new OUMotivosReprogramacionDC();
                        programacion.MotivoReprogramacion.IdMotivo = null;
                        programacion.MotivoReprogramacion.DescripcionMotivo = string.Empty;
                    }

                    programacion.EstaDescargada = true;
                    ///Actualiza la actual programacion con los motivo de reprogramacion
                    OURepositorio.Instancia.ActualizaProgramacionSolicitudRecogida(programacion);
                    OURepositorio.Instancia.EliminarSolicitudPlanillaRecogida(programacion.IdProgramacionSolicitudRecogida);
                    programacion.EstaDescargada = false;
                    programacion.MotivoReprogramacion = new OUMotivosReprogramacionDC();
                    programacion.MotivoReprogramacion.IdMotivo = null;
                    programacion.MotivoReprogramacion.DescripcionMotivo = string.Empty;

                    ///Guarda la nueva programacion
                    OURepositorio.Instancia.GuardaProgramacionSolicitudRecogida(programacion);

                    ///Guarda la solicitud en la planilla seleccionada
                    if (programacion.IdPlanillaRecogida > 0)
                    {
                        programacion.Recogida.EstaReportada = false;
                        long idPlanilla = OURepositorio.Instancia.ObtenerPlanillaRecogidaSolicitud(programacion.Recogida.IdRecogida.Value);
                        if (idPlanilla != 0)
                            OURepositorio.Instancia.ActualizarSolicitudPlanillaRecogida(programacion);
                        else
                            OURepositorio.Instancia.GuardarSolicitudPlanillaRecogida(programacion);

                        ///Actualiza el estado de la solicitud
                        OURepositorio.Instancia.ActualizaEstadoSolicitudRecogida((short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA, programacion.Recogida.IdRecogida.Value);
                        OURepositorio.Instancia.ActualizaEstadoProgramacionSolicitudRecogida(true, OUConstantesOperacionUrbana.ESTADO_PLANILLADA, programacion.IdProgramacionSolicitudRecogida);
                    }
                    else
                    {
                        ///Elimina la programacion existente y la descarga de la planilla
                        OURepositorio.Instancia.EliminarSolicitudPlanillaRecogida(programacion.IdProgramacionSolicitudRecogida);
                        ///Actualiza el estado de la solicitud
                        OURepositorio.Instancia.ActualizaEstadoSolicitudRecogida((short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA_SIN_PLANILLAR, programacion.Recogida.IdRecogida.Value);
                        OURepositorio.Instancia.ActualizaEstadoProgramacionSolicitudRecogida(false, OUConstantesOperacionUrbana.ESTADO_NO_PLANILLADA, programacion.IdProgramacionSolicitudRecogida);
                    }
                }
                ///si la solicutud no tiene programacion hace la programacion
                else
                {
                    if (programacion.MotivoReprogramacion == null)
                        programacion.MotivoReprogramacion = new OUMotivosReprogramacionDC();
                    ///Guarda la programacion de la recogida
                    OURepositorio.Instancia.GuardaProgramacionSolicitudRecogida(programacion);

                    ///si la programacion tiene asignacion a una planilla hace la asignacion
                    if (programacion.IdPlanillaRecogida > 0)
                    {
                        programacion.ReportadoMensajero = false;
                        OURepositorio.Instancia.GuardarSolicitudPlanillaRecogida(programacion);
                        ///Actualiza el estado de la solicitud
                        OURepositorio.Instancia.ActualizaEstadoSolicitudRecogida((short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA, programacion.Recogida.IdRecogida.Value);

                        OURepositorio.Instancia.ActualizaEstadoProgramacionSolicitudRecogida(true, OUConstantesOperacionUrbana.ESTADO_PLANILLADA, programacion.IdProgramacionSolicitudRecogida);

                    }
                    else
                    {
                        ///Actualiza el estado de la solicitud como planillada no programada
                        OURepositorio.Instancia.ActualizaEstadoSolicitudRecogida((short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA_SIN_PLANILLAR, programacion.Recogida.IdRecogida.Value);

                        // OURepositorio.Instancia.ActualizaEstadoProgramacionSolicitudRecogida(true, OUConstantesOperacionUrbana.ESTADO_PLANILLADA, programacion.IdProgramacionSolicitudRecogida);
                        OURepositorio.Instancia.ActualizaEstadoProgramacionSolicitudRecogida(false, OUConstantesOperacionUrbana.ESTADO_NO_PLANILLADA, programacion.IdProgramacionSolicitudRecogida);

                    }
                }

                scope.Complete();
            }
        }


        /// <summary>
        /// Agrega una programacion y una planilla a una solicitud de recogida
        /// </summary>
        /// <param name="programacion"></param>
        public void AgregarProgramacionSolicitudRecogida(OUProgramacionSolicitudRecogidaDC programacion)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                programacion.IdPlanillaRecogida = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Planilla_prog_de_recogida);

                trans.Complete();
            }
            OURepositorio.Instancia.AgregarProgramacionSolicitudRecogida(programacion);

        }


        /// <summary>
        /// Asigna una solicitud de recogida a un mensajero
        /// </summary>
        /// <returns></returns>
        public long AsignarRecogidaMensajero(OUAsignacionRecogidaMensajeroDC asignacion)
        {
            return OURepositorio.Instancia.AsignarRecogidaMensajero(asignacion);
        }

        /// <summary>
        /// Actualiza la georeferenciacion de una recogida
        /// </summary>
        /// <param name="longitud"></param>
        /// <param name="latitud"></param>
        /// <param name="idRecogida"></param>
        public void ActualizarGeoreferenciacionRecogida(string longitud, string latitud, long idRecogida)
        {
            OURepositorio.Instancia.ActualizarGeoreferenciacionRecogida(longitud, latitud, idRecogida);
        }


        #endregion Insercion

        #region Planilla Recogidas

        /// <summary>
        /// Obtiene todas las recogidas asignadas a un mensajero en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasMensajerosDia(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerRecogidasMensajerosDia(idMensajero);
        }

        /// <summary>
        /// Obtiene todas las recogidas creadas por un cliente movil en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasClienteMovilDia(string tokenDispositivo)
        {
            return OURepositorio.Instancia.ObtenerRecogidasClienteMovilDia(tokenDispositivo);
        }

        /// <summary>
        /// Selecciona todas las recogidas vencidas que fueron asignadas a los usuarioMensajero (usuarios PAM ) en un dia
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasVencidasMensajerosPAMDia()
        {
            return OURepositorio.Instancia.ObtenerRecogidasVencidasMensajerosPAMDia();
        }

        /// <summary>
        /// Obtiene las planillas de recogidas por centro logistico
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="idCol"></param>
        /// <param name="incluyeFecha"></param>
        /// <returns></returns>
        public List<OUProgramacionSolicitudRecogidaDC> ObtenerPlanillasRecogidas(Dictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, long idCol, bool incluyeFecha)
        {
            return OURepositorio.Instancia.ObtenerPlanillasRecogidas(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, idCol, incluyeFecha);
        }

        /// <summary>
        /// Obtiene la programacion de la recogida esporadica sin planillar
        /// </summary>
        /// <param name="idZona"></param>
        /// <param name="idTipoMensajero"></param>
        /// <param name="idCol"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerProgramacionRecogidasSinPlanillar(string idZona, short idTipoMensajero, long idCol, DateTime fechaRecogidas)
        {
            List<OURecogidasDC> lst = OURepositorio.Instancia.ObtenerProgramacionRecogidasSinPlanillar(idZona, idTipoMensajero, idCol, fechaRecogidas);
            return lst;
        }




        /// <summary>
        /// Guarda la planilla de recogidas con las recogidas seleccionadas
        /// </summary>
        /// <param name="programacion"></param>
        public void GuardaPlanillaRecogidas(OUProgramacionSolicitudRecogidaDC programacion)
        {
            //using (TransactionScope scope = new TransactionScope())
            //{
            if (programacion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope trans = new TransactionScope())
                {
                    programacion.IdPlanillaRecogida = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Planilla_prog_de_recogida);
                    trans.Complete();
                }
            }

            foreach (OURecogidasDC recogidas in programacion.RecogidasPlanilla)
            {
                if (recogidas.EstaSeleccionada && recogidas.RegistroHabilitado)
                {
                    programacion.Recogida = recogidas;
                    programacion.Recogida.EstaReportada = false;
                    programacion.IdProgramacionSolicitudRecogida = recogidas.IdProgramacionSolicitudRecogida;
                    OURepositorio.Instancia.GuardarSolicitudPlanillaRecogida(programacion);

                    if (programacion.MotivoReprogramacion == null)
                    {
                        programacion.MotivoReprogramacion = new OUMotivosReprogramacionDC();
                        programacion.MotivoReprogramacion.IdMotivo = null;
                        programacion.MotivoReprogramacion.DescripcionMotivo = string.Empty;
                    }

                    OUProgramacionSolicitudRecogidaDC prog = OURepositorio.Instancia.ObtenerProgramacionSolicitudRecogida(recogidas.IdProgramacionSolicitudRecogida);

                    OURepositorio.Instancia.ObtenerProgramacionRecogidas(recogidas.IdRecogida.Value);

                    OURepositorio.Instancia.GuardarHistoricoProgramacionRecogidas(new OUProgramacionSolicitudRecogidaDC()
                    {
                        CreadoPor = ControllerContext.Current.Usuario,
                        Estado = OUConstantesOperacionUrbana.ESTADO_PLANILLADA,
                        EstadoRegistro = EnumEstadoRegistro.ADICIONADO,
                        FechaCreacion = DateTime.Now,
                        FechaProgramacion = prog.FechaProgramacion,
                        FechaReporteMensajero = programacion.FechaReporteMensajero,
                        IdPlanillaRecogida = programacion.IdPlanillaRecogida,
                        MensajeroPlanilla = programacion.MensajeroPlanilla,
                        MotivoReprogramacion = programacion.MotivoReprogramacion,
                        Recogida = recogidas,
                        ReportadoMensajero = programacion.ReportadoMensajero,
                        TipoMensajero = programacion.TipoMensajero,
                        Zona = programacion.Zona
                    });
                    OURepositorio.Instancia.ActualizaEstadoSolicitudRecogida((short)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA, programacion.Recogida.IdRecogida.Value);
                    OURepositorio.Instancia.ActualizaEstadoProgramacionSolicitudRecogida(true, OUConstantesOperacionUrbana.ESTADO_PLANILLADA, programacion.IdProgramacionSolicitudRecogida);

                }
            }
            //    scope.Complete();
            //}
        }

        /// <summary>
        /// Obtiene la planilla de programacion de recogidas
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public OUProgramacionSolicitudRecogidaDC ObtenerPlanillaRecogida(long idPlanilla)
        {
            return OURepositorio.Instancia.ObtenerPlanillaRecogida(idPlanilla);
        }

        #endregion Planilla Recogidas

        #endregion Programacion Recogidas

        #region Descargue Recogidas

        /// <summary>
        /// Retorna los motivos de descargue de recogidas
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoDescargueRecogidasDC> ObtenerMotivosDescargueRecogidas()
        {
            return OURepositorio.Instancia.ObtenerMotivosDescargueRecogidas();
        }

        /// <summary>
        /// Retorna los motivos de descargue de recogidas firltrado por idmotivo
        /// </summary>
        /// <returns></returns>
        public OUMotivoDescargueRecogidasDC ObtenerMotivosDescargueRecogidasIdMotivo(int idMotivo)
        {
            return OURepositorio.Instancia.ObtenerMotivosDescargueRecogidasIdMotivo(idMotivo);
        }

        /// <summary>
        /// Obtiene las recogidas de la planilla
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="tipoRecogida"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPlanilla(long idPlanilla)
        {
            return OURepositorio.Instancia.ObtenerRecogidasPlanilla(idPlanilla);
        }

        /// <summary>
        /// Guarda el descargue de una recogida
        /// </summary>
        /// <param name="AdminPlanilla"></param>
        public void GuardarDescargueRecogida(OUProgramacionSolicitudRecogidaDC AdminPlanilla)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ///Valida que la recogida no se encuentre descargada
                //OURecogidasDC recogidaInfo = new OURecogidasDC();
                // recogidaInfo = OURepositorio.Instancia.ObtenerDescargueRecogidaPorSolicitud(AdminPlanilla.Recogida.IdRecogida.Value);

                //if (recogidaInfo.IdRecogida == AdminPlanilla.Recogida.IdRecogida)
                //    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_URBANA, OUEnumTipoErrorOU.EX_RECOGIDA_DESCARGADA.ToString(), string.Format(OUMensajesOperacionUrbana.CargarMensaje(OUEnumTipoErrorOU.EX_RECOGIDA_DESCARGADA), AdminPlanilla.Recogida.IdRecogida)));




                ///Guarda el descargue de la solicitud
                OURepositorio.Instancia.GuardarDescargueRecogida(AdminPlanilla.Recogida);
                ///Actualiza la solicitud con el estado y si esta programada
                if (AdminPlanilla.Recogida.MotivoDescargue.PermiteReprogramar)
                    OURepositorio.Instancia.ActualizarDescargueRecogida(AdminPlanilla.Recogida.IdRecogida.Value, AdminPlanilla.Recogida.IdProgramacionSolicitudRecogida, (int)OUEnumEstadoSolicitudRecogidas.IN_DESCARGADA_POR_REPROGRAMAR, true, false);
                else
                    OURepositorio.Instancia.ActualizarDescargueRecogida(AdminPlanilla.Recogida.IdRecogida.Value, AdminPlanilla.Recogida.IdProgramacionSolicitudRecogida, (int)OUEnumEstadoSolicitudRecogidas.IN_REALIZADA, true, true);


                scope.Complete();
            }
        }


        /// <summary>
        /// Guarda el descargue de la recogida peaton
        /// </summary>
        /// <param name="recogida"></param>
        public void GuardarDescargueRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                OURepositorio.Instancia.GuardarDescargueRecogidaPeaton(descargue);

                if (descargue.MotivoDescargue.PermiteReprogramar)
                {
                    OURepositorio.Instancia.ActualizarEstadoSolicitudRecogida(descargue.IdRecogida, OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA_NO_RECOGIDA);
                    OURepositorio.Instancia.ActualizarEstadoSolicitudRecogida(descargue.IdRecogida, OUEnumEstadoSolicitudRecogidas.IN_REPROGRAMAR);
                }
                else
                {
                    OURepositorio.Instancia.ActualizarEstadoSolicitudRecogida(descargue.IdRecogida, OUEnumEstadoSolicitudRecogidas.IN_REALIZADA);
                }

                trans.Complete();
            }
        }

        /// <summary>
        /// cancela una recogida de peaton
        /// </summary>
        /// <param name="descargue"></param>
        public void CancelarRecogidaPeaton(OUDescargueRecogidaMensajeroDC descargue)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                ///Si la recogida tiene alguna asignación a mensajero entonces se descarga
                if (descargue.IdAsignacion > 0)
                {
                    OURepositorio.Instancia.GuardarDescargueRecogidaPeaton(descargue);
                }
                OURepositorio.Instancia.ActualizarEstadoSolicitudRecogida(descargue.IdRecogida, OUEnumEstadoSolicitudRecogidas.IN_CANCELADA);
                trans.Complete();
            }
        }


        /// <summary>
        /// Actualiza el estado de la solicitud de uan recogida e inserta el estado traza
        /// </summary>
        public void ActualizarEstadoSolicitudRecogida(long idRecogida, OUEnumEstadoSolicitudRecogidas estado)
        {
            OURepositorio.Instancia.ActualizarEstadoSolicitudRecogida(idRecogida, estado);
        }

        /// <summary>
        /// Abre una recogida
        /// </summary>
        /// <param name="recogida"></param>
        public void AbrirRecogida(OUAperturaRecogidaDC recogida)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                ///Guarda la apertura de la recogida
                OURepositorio.Instancia.AbrirRecogida(recogida.IdRecogida, recogida.Observaciones, recogida.MotivoApertura.IdMotivo);
                ///Actualiza la solicitud de la recogida
                bool estaDescargada = false;
                OURepositorio.Instancia.ActualizarDescargueRecogida(recogida.IdRecogida, recogida.IdProgramacionSolicitudRecogida, (int)OUEnumEstadoSolicitudRecogidas.IN_PROGRAMADA, estaDescargada, false);

                ///Envia la falla por la apertura de la recogida
                OUManejadorFallas.DespacharFallaPorReabrirRecogida(recogida.IdRecogida, recogida.MotivoApertura.DescripcionMotivo, ControllerContext.Current.Usuario);

                scope.Complete();
            }
        }

        /// <summary>
        /// Obtiene los motivos de apertura de una recogida
        /// </summary>
        /// <returns></returns>
        public List<OUMotivoAperturaDC> ObtenerMotivosAperturaRecogida()
        {
            return OURepositorio.Instancia.ObtenerMotivosAperturaRecogida();
        }

        #endregion Descargue Recogidas

        /// <summary>
        /// Retorna el número de la última planilla y el mensajero asociado dado el número de guía
        /// </summary>
        public OUPlanillaAsignacionMensajero ObtenerUltimaPlanillaMensajeroGuia(long numeroGuia)
        {
            return OURepositorio.Instancia.ObtenerUltimaPlanillaMensajeroGuia(numeroGuia);
        }


        /// <summary>
        /// Inserta la relacion entre un dispositivo movil y una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        /// <param name="idDispositivoMovil"></param>
        public void RegistrarSolicitudRecogidaMovil(long idRecogida, long idDispositivoMovil)
        {
            OURepositorio.Instancia.RegistrarSolicitudRecogidaMovil(idRecogida, idDispositivoMovil);
        }
        /// <summary>
        /// retorna el token del dispositivo movil con el que se hizo una recogida
        /// </summary>
        /// <param name="idRecogida"></param>
        public PADispositivoMovil ObtenerdispositivoMovilClienteRecogida(long idRecogida)
        {
            return OURepositorio.Instancia.ObtenerdispositivoMovilClienteRecogida(idRecogida);
        }



        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesDia()
        {
            return OURepositorio.Instancia.ObtenerRecogidasPeatonPendientesDia();
        }

        /// <summary>
        /// Obtiene todas las recogidas pendientes de peaton del dia actual, filtradas por un rango de tiempo para la hora de recogida y ciudad de recogida
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasPeatonPendientesPorProgramarDia(string idLocalidad)
        {
            return OURepositorio.Instancia.ObtenerRecogidasPeatonPendientesPorProgramarDia(idLocalidad);
        }

        /// <summary>
        /// Obtiene todas las solicitudes de recogida disponibles por localidad.
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerRecogidasDisponiblesPeatonDia(string idLocalidad)
        {
            List<OURecogidasDC> retorno = OURepositorio.Instancia.ObtenerRecogidasDisponiblesPeatonDia(idLocalidad);


            retorno.Where(r => r.Fotografias != null && r.Fotografias.Count > 0).ToList();

            return retorno;
        }

        /// <summary>
        /// obtiene 
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerDetalleGuiasAuditadas(long idAuditoria)
        {
            return OURepositorio.Instancia.ObtenerDetalleGuiasAuditadas(idAuditoria);
        }

        /// <summary>
        /// Obtiene todas las recogidas de peaton sin programacion
        /// </summary>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerTodasRecogidasPeatonPendientesPorProgramar()
        {
            return OURepositorio.Instancia.ObtenerTodasRecogidasPeatonPendientesPorProgramar();
        }

        /// <summary>
        /// Guarda las notificaciones enviadas por cada recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        public void GuardarNotificacionRecogida(long idSolicitudRecogida)
        {
            OURepositorio.Instancia.GuardarNotificacionRecogida(idSolicitudRecogida);
        }

        /// <summary>
        /// Obtiene los datos del usuario que está solicitando 
        /// la recogida si ya se ha registrado antes.
        /// </summary>
        /// <param name="tipoid"></param>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public PAPersonaInternaDC ObtenerInfoUsuarioRecogida(string tipoid, string identificacion)
        {
            return OURepositorio.Instancia.ObtenerInfoUsuarioRecogida(tipoid, identificacion);
        }

        /// <summary>
        /// Obtiene las recogidas realizadas segun el token del dispositivo del cliente peaton
        /// </summary>
        /// <param name="tokenDispositivo"></param>
        /// <returns></returns>
        public List<OURecogidasDC> ObtenerMisRecogidasClientePeaton(string tokenDispositivo)
        {
            return OURepositorio.Instancia.ObtenerMisRecogidasClientePeaton(tokenDispositivo);
        }

        /// <summary>
        /// Obtiene las imagenes capturadas de la solicitud recogida.
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <returns></returns>
        public List<string> ObtenerImagenesSolicitudRecogida(long idSolicitudRecogida)
        {
            return OURepositorio.Instancia.ObtenerImagenesSolicitudRecogida(idSolicitudRecogida);
        }

        /// <summary>
        /// Metodo para calificar la solicitud de recogida.
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="calificacion"></param>
        /// <param name="observaciones"></param>
        public void CalificarSolicitudRecogida(long idSolicitudRecogida, int calificacion, string observaciones)
        {
            OURepositorio.Instancia.CalificarSolicitudRecogida(idSolicitudRecogida, calificacion, observaciones);
        }

        /// <summary>
        /// obtiene las guias de determinada auditoria
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerGuiasPorAuditoria(long idAuditoria)
        {
            return OURepositorio.Instancia.ObtenerGuiasPorAuditoria(idAuditoria);
        }

        /// <summary>
        /// Obtiene las auditorias realizadas a determinado mensajero ennun rango de fecha
        /// </summary>
        /// <param name="IdAuditoria"></param>
        /// <param name="fechaIni"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<OUGuiaIngresadaDC> ObtenerAuditoriasPorMensajero(long IdAuditoria, DateTime fechaIni, DateTime fechaFin)
        {
            return OURepositorio.Instancia.ObtenerAuditoriasPorMensajero(IdAuditoria, fechaIni, fechaFin);
        }

        /// <summary>
        /// Aprovisiona guía catalogada como "fantasma", es decir una numeración que debería ser automática
        /// </summary>
        /// <param name="numGUia"></param>
        /// <param name="idCs"></param>
        public bool AprovisionGuiaFantasma(long numGUia, long idCs)
        {
            return OURepositorio.Instancia.AprovisionGuiaFantasma(numGUia, idCs);
        }

        /// <summary>
        /// Retorna el número de auditoria
        /// </summary>
        /// <param name="idCs"></param>
        /// <returns></returns>
        public long CrearAuditoriaAsignacionMensajero(long idCs, long idMensajero)
        {
            return OURepositorio.Instancia.CrearAuditoriaAsignacionMensajero(idCs, idMensajero);
        }

        /// <summary>
        /// Crear auditoria asignacion mensajero guia
        /// </summary>
        /// <param name="idAuditoria"></param>
        /// <param name="esSobrante"></param>
        /// <param name="idMensajero"></param>
        /// <param name="fecha"></param>
        public void CrearAuditoriaAsignacionMensajeroGuia(long idAuditoria, int esSobrante, long idMensajero, DateTime fecha)
        {
            OURepositorio.Instancia.CrearAuditoriaAsignacionMensajeroGuia(idAuditoria, esSobrante, idMensajero, fecha);
        }
        /// <summary>
        /// metodo para obtener motivo y fecha intento de entrega por numero de guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idMotivoGuia"></param>
        /// <returns></returns>
        public LIMotivoEvidenciaGuiaDC ObtenerFechaIntentoYMotivoGuia(long numeroGuia, long idPlanilla)
        {
            return OURepositorio.Instancia.ObtenerFechaIntentoYMotivoGuia(numeroGuia , idPlanilla);
        }
        /// <summary>
        /// Metodo que consulta las guias planilladas para un auditor
        /// </summary>
        /// <param name="idAuditor"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasAuditor(long idAuditor)
        {
            return OURepositorio.Instancia.ObtenerGuiasAuditor(idAuditor);
        }

        /// <summary>
        /// Metodo para consultar las guias planilladas por mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public IList<OUGuiaIngresadaDC> ObtenerGuiasMensajero(long idMensajero)
        {
            return OURepositorio.Instancia.ObtenerGuiasMensajero(idMensajero);
        }
    }
}