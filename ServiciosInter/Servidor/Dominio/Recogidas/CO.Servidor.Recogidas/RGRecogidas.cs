using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.LogisticaInversa.Comun;
using CO.Servidor.OperacionUrbana;
using CO.Servidor.Raps;
using CO.Servidor.Raps.Comun.Integraciones;
using CO.Servidor.RAPS.Reglas.Parametros;
using CO.Servidor.Recogidas.Comun;
using CO.Servidor.Recogidas.Datos;
using CO.Servidor.Recogidas.Datos.Administrador;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using CO.Servidor.Servicios.ContratoDatos.Recogidas.Recogidas;


using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Transactions;

namespace CO.Servidor.Recogidas
{
    /// <summary>
    /// Administrador para la administraciÃ³n de rutas
    /// </summary>
    public class RGRecogidas : ControllerBase
    {
        private static readonly RGRecogidas instancia = (RGRecogidas)FabricaInterceptores.GetProxy(new RGRecogidas(), COConstantesModulos.MODULO_RECOGIDAS);
        private IPUFachadaCentroServicios fachadaCentroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();
        public static RGRecogidas Instancia
        {
            get { return RGRecogidas.instancia; }
        }

        /// <summary>
        /// constructor
        /// </summary>
        private RGRecogidas()
        { }

        /// <summary>
        /// Obtiene las recogidas que no se han asignado
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <param name="idCol"></param>
        /// <param name="idClienteCredito"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasPorAsignar(string idCiudad, string idCol, int? idClienteCredito, long? idCentroServicio, int numeroPagina, int tamanioPagina)
        {
            return RGRepositorio.Instancia.ObtenerRecogidasPorAsignar(idCiudad, idCol, idClienteCredito, idCentroServicio, numeroPagina, tamanioPagina);
        }

        /// <summary>
        /// obtiene la lista de clientes credito
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerClientesCredito(string idCol)
        {
            return RGRepositorio.Instancia.ObtenerClientesCredito(idCol);
        }

        public long ObtenerCantidadFijasPorAsignar(string idCentroServicio)
        {
            return RGRepositorio.Instancia.ObtenerCantidadFijasPorAsignar(idCentroServicio);
        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaAsignarRecogidas(string idLocalidad)
        {
            return RGRepositorio.Instancia.ObtenerEmpleadosParaAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// Obtener Motivos Estado Solicitud Recogida X Actor
        /// </summary>
        /// <param name="idActor"></param>
        /// <returns></returns>
        public List<RGMotivoEstadoSolRecogidaDC> ObtenerMotivoEstadoSolRecogidaXActor(long idActor)
        {
            return RGRepositorio.Instancia.ObtenerMotivoEstadoSolRecogidaXActor(idActor);
        }

        /// <summary>
        /// Gestionar Con Motivo estado Solicitud Recogida
        /// </summary>
        /// <param name="solicitud"></param>
        /// <param name="idMotivo"></param>
        /// <param name="idActor"></param>
        public void CancelarConMotivoSolRecogida(RGAsignarRecogidaDC solicitud, int idMotivo, int idActor)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                var asignacion = new RGAsignacionSolicitudRecogidaDC
                {
                    DocPersonaResponsable = solicitud.DocPersonaResponsable,
                    IdSolicitudRecogida = solicitud.IdSolicitudRecogida,
                    PlacaVehiculo = solicitud.PlacaVehiculo
                };

                RGRepositorio.Instancia.BorrarAsignacionSolicitudRecogida(asignacion);

                RGRepositorio.Instancia.CancelarSolicitudRecogida(asignacion);

                var nuevoEstado = RGRepositorio.Instancia.ObtenerNuevoEstadoMotivosSolicitud(idMotivo, idActor);

                RGRepositorio.Instancia.InsertarEstadoSolRecogidaTraza(solicitud, nuevoEstado);

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Obtiene todos los datos de un empleado por su cedula
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// <returns></returns>
        public RGEmpleadoDC ObtenerDatosDeEmpleadoPorCedula(string idEmpleado)
        {
            return RGRepositorio.Instancia.ObtenerDatosDeEmpleadoPorCedula(idEmpleado);
        }

        /// <summary>
        /// Inserta la programacion de recogidas fijas a un mensajero
        /// </summary>
        public void InsertarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                programacion.listaRecogidas.ForEach(r =>
                {
                    r.Id = r.Id == 0 || r.Id == r.idCliente ? null : r.Id;
                    r.idCliente = r.idCliente == 0 ? null : r.idCliente;
                    r.IdCentroServicio = r.IdCentroServicio == 0 ? null : r.IdCentroServicio;
                    r.IdSucursal = r.IdSucursal == 0 ? null : r.IdSucursal;
                    r.IdProgramacion = RGRepositorio.Instancia.InsertarProgramacionRecogidasFijas(programacion, r);
                    if (r.IdProgramacion > 0)
                    {
                        r.NumeroDocumento = programacion.docPersonaResponsable;
                        RGRepositorio.Instancia.InsertarAsignacionSolicitud(r);
                    }
                    else
                    {
                        throw new FaultException("No fue posible registrar la programaciÃ³n");
                    }

                });
                scope.Complete();
            }

        }

        /// <summary>
        /// obtiene todos los empleados que sean mensajeros conductores y auxiliares de zona sin importar que esten activos en la empresa
        /// </summary>
        /// <returns></returns>
        public List<RGEmpleadoDC> ObtenerEmpleadosParaReAsignarRecogidas(string idLocalidad)
        {
            return RGRepositorio.Instancia.ObtenerEmpleadosParaReAsignarRecogidas(idLocalidad);
        }

        /// <summary>
        /// obtiene todos los centros de serivcio que pertenezcan a determinado col y a determinada ciudad
        /// </summary>
        /// <param name="idCol"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerCentrosDeServicio(string idCol, string idCiudad)
        {
            return RGRepositorio.Instancia.ObtenerCentrosDeServicio(idCol, idCiudad);
        }

        /// <summary>
        /// Consulta las territoriales de los centros de servicios
        /// </summary>
        /// <param name="idCentroServicio"></param>        
        /// <returns></returns>
        public List<RGTerritorialDC> ObtenerTerritoriales(string idCentroServicio)
        {
            return RGParametros.Instancia.ObtenerTerritoriales(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los horarios para sucursal o para centro de servicio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string ObtenerHorarioRecogida(RGRecogidasDC datosUbicacion)
        {
            string horario = "";
            List<string> lstHorario = new List<string>();
            if (datosUbicacion.Id == datosUbicacion.idCliente)
            {
                lstHorario = RGRepositorio.Instancia.ObtenerHorarioRecogidaSucursal(datosUbicacion.idCliente);
                lstHorario.ForEach(h =>
                {
                    horario = horario + " " + h;
                });
                return string.IsNullOrEmpty(horario) ? "No Tiene Horario" : horario;
            }
            lstHorario = RGRepositorio.Instancia.ObtenerHorarioRecogidaCentroServicio((int)datosUbicacion.Id);
            lstHorario.ForEach(h =>
            {
                horario = horario + " " + h;
            });
            return string.IsNullOrEmpty(horario) ? "No Tiene Horario" : horario;

        }

        /// <summary>
        /// Obtiene la ultima solicitud registrada
        /// </summary>
        /// <returns></returns>
        public RGRecogidasDC ObtenerUltimaSolicitud(string numeroDocumento)
        {
            return RGRepositorio.Instancia.ObtenerUltimaSolicitud(numeroDocumento == null ? "0" : numeroDocumento);
        }

        public RGEstadisticas ObtenerEstadisticas(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            RGEstadisticas result = new RGEstadisticas();
            /*Balance Recogidas vs Entregas Lineal Chart*/
            //result.graficoLineal

            List<RGDictionary> entregas = RGRepositorio.Instancia.ObtenerComportamientoEntregaHoras(fechaInicial, fechaFinal, idCiudad == null ? string.Empty : idCiudad, idTerritorial == null ? string.Empty : idTerritorial, idRegional == null ? string.Empty : idRegional);

            List<RGDictionary> recogidas = RGRepositorio.Instancia.ObtenerComportamientoRecogidasHoras(fechaInicial, fechaFinal, idCiudad == null ? string.Empty : idCiudad, idTerritorial == null ? string.Empty : idTerritorial, idRegional == null ? string.Empty : idRegional);

            result.graficoLineal.Titulo = "Balance Recogidas vs Entregas Por Horas";
            result.graficoLineal.Descripcion = "";
            result.graficoLineal.Data.Add(recogidas);
            result.graficoLineal.Data.Add(entregas);

            /* Detalle Recogidas Donut Chart*/

            RGGraficaDonnaPie balanceRecogida = RGRepositorio.Instancia.ObtenerEstadisticasPorFecha(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional, false);
            balanceRecogida.Titulo = "Recogidas";

            RGGraficaDonnaPie balanceEntrega = RGRepositorio.Instancia.ObtenerEstadisticasPorFecha(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional, true);
            balanceEntrega.Titulo = "Entregas";

            result.graficosDonnut.Add(balanceRecogida);
            result.graficosDonnut.Add(balanceEntrega);

            /* Tabla Top Balance Ciudades */

            RGBestSellingChart bestRecogidas = RGRepositorio.Instancia.ObtenerValoresTopPorCiudad(fechaInicial, fechaFinal, true);
            bestRecogidas.Titulo = "Ciudades Con Mas Recogidas";
            RGBestSellingChart bestEntregas = RGRepositorio.Instancia.ObtenerValoresTopPorCiudad(fechaInicial, fechaFinal, false);
            bestEntregas.Titulo = "Ciudades Con Mas Entregas";

            result.chartResults.Add(bestRecogidas);
            result.chartResults.Add(bestEntregas);

            return result;
        }



        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerMensajerosActivosAplicacion(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return RGRepositorio.Instancia.ObtenerMensajerosActivosPorAplicacion(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public List<RGDetalleMensajeroBalance.RGDetalleInfoMensajero> ObtenerPosicionesMensajerosActivos(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idCiudad, string idTerritorial, string idRegional)
        {
            return RGRepositorio.Instancia.ObtenerPosicionesMensajerosActivos(fechaInicial, fechaFinal, idCiudad, idTerritorial, idRegional);
        }

        public RGDetalleMensajeroBalance ObtenerDetalleMensajeroPorId(DateTime fechaInicial, Nullable<DateTime> fechaFinal, string idMensajero)
        {
            RGDetalleMensajeroBalance mensajero = new RGDetalleMensajeroBalance()
            {
                infoMensajero = new RGDetalleMensajeroBalance.RGDetalleInfoMensajero()
                ,
                rutaMensajero = new List<RGDetalleMensajeroBalance.RGRutasPorFechaMensajero>()
            };

            List<RGDetalleMensajeroBalance.RGDetalleRutaMensajero> result = new List<RGDetalleMensajeroBalance.RGDetalleRutaMensajero>();

            mensajero.infoMensajero = RGRepositorio.Instancia.ObtenerDetalleInfoMensajero(fechaInicial, fechaFinal, idMensajero);
            result = RGRepositorio.Instancia.ObtenerDetalleRutaMensajero(fechaInicial, fechaFinal, idMensajero);
            var fechas = result.Select(f => f.FechaGrabacion.Date).Distinct();

            foreach (var item in fechas)
            {
                List<RGDetalleMensajeroBalance.RGDetalleRutaMensajero> listGrouped = result.Where(r => r.FechaGrabacion.Date.Equals(item)).ToList();
                mensajero.rutaMensajero.Add(new RGDetalleMensajeroBalance.RGRutasPorFechaMensajero()
                {
                    RutaPorFecha = listGrouped
                });
            }

            return mensajero;
        }

        public List<RGRecogidaEsporadicaDC> ObtenerMisRecogidasClientePeaton(string idUsuario)
        {
            return RGRepositorio.Instancia.ObtenerMisRecogidasClientePeaton(idUsuario);
        }

        /// <summary>
        /// registra el forzado de un recogida esporadica
        /// </summary>
        /// <param name="solicitud"></param>
        public void AsignarRecogida(RGAsignarRecogidaDC solicitud)
        {
            EnumEstadoSolicitudRecogida estadoSolicitud;

            using (TransactionScope transaccion = new TransactionScope())
            {
                estadoSolicitud = RGRepositorio.Instancia.ObtenerEstadoSolicitudRecogida(solicitud);

                /***************************** SI EL ESTADO ES CREADO ***********************************************/
                if (estadoSolicitud == EnumEstadoSolicitudRecogida.Creado)
                {
                    lock (this)
                    {
                        estadoSolicitud = RGRepositorio.Instancia.ObtenerEstadoSolicitudRecogida(solicitud);
                        /***************************** SI ES DIFERENTE DE RESERVADO ***********************************************/
                        if (estadoSolicitud != EnumEstadoSolicitudRecogida.Reservado)
                        {
                            RegistrarAsignacionRecogida(solicitud);
                        }
                        else
                        {
                            throw new FaultException("La recogida ya fue asignada.");
                        }
                    }
                }
                else if (solicitud.EstadoRecogida != EnumEstadoSolicitudRecogida.Reservado)
                {
                    RegistrarAsignacionRecogida(solicitud);
                }
                else
                {
                    throw new FaultException("La recogida no se puede asignar.");
                }
                transaccion.Complete();
            }

        }

        /// <summary>
        /// Inserta una gestion de telemercadeo
        /// </summary>
        /// <param name="telemercadeo"></param>
        public void InsertarTelemercadeo(RGTelemercadeo telemercadeo)
        {
            RGRepositorio.Instancia.InsertaTelemercadeo(telemercadeo);
        }

        public void ModificarCoordenadasRecogidaEsporadica(RGRecogidaEsporadicaDC recogida)
        {
            EnumEstadoSolicitudRecogida estadoSolicitud;

            estadoSolicitud = RGRepositorio.Instancia.ObtenerEstadoSolicitudRecogida(
                    new RGAsignarRecogidaDC { IdSolicitudRecogida = (long)recogida.IdSolRecogida });
            if (estadoSolicitud == EnumEstadoSolicitudRecogida.Creado
                || estadoSolicitud == EnumEstadoSolicitudRecogida.ParaForzar
                || estadoSolicitud == EnumEstadoSolicitudRecogida.Telemercadeo)
            {
                RGRepositorio.Instancia.ModificarCoordenadasRecogidaEsporadica(recogida);
            }
            else
            {
                throw new FaultException("No es posible asignar Coordenadas en el estado " + estadoSolicitud.ToString());
            }
        }

        /// <summary>
        /// metodo para asignar solicitud recogida
        /// </summary>
        /// <param name="solicitud"></param>
        internal void RegistrarAsignacionRecogida(RGAsignarRecogidaDC solicitud)
        {

            var asignacion = new RGAsignacionSolicitudRecogidaDC
            {
                DocPersonaResponsable = solicitud.DocPersonaResponsable,
                IdSolicitudRecogida = solicitud.IdSolicitudRecogida,
                PlacaVehiculo = solicitud.PlacaVehiculo
            };
            RGRepositorio.Instancia.InsertarAsignacionSolicitudRecogida_REC(asignacion);
            RGRepositorio.Instancia.InsertarEstadoSolRecogidaTraza(solicitud, solicitud.EstadoRecogida);
        }

        public void ModificarRecogidaEsporadica(RGRecogidasDC recogida)
        {

            if (recogida.Id != null)
            {
                RGEnumClaseSolicitud claseSolicitud = TipoRecogidaToClaseRecogida(recogida);
                var detalle = ObtenerDetalleSolRecogida((long)recogida.Id, claseSolicitud);
                if (recogida.TipoRecogida == RGEnumTipoRecogidaDC.Esporadica ||
                    (recogida.TipoRecogida == RGEnumTipoRecogidaDC.FijaCliente && recogida.EsEsporadicaCliente))
                {
                    RGRepositorio.Instancia.ModificarRecogidaEsporadica(recogida);
                }
                else
                {
                    throw new FaultException<ControllerException>
                  (new ControllerException(COConstantesModulos.RECOGIDAS,
                  RGEnumTipoErrorRecogidas.EX_RECOGIDA_NO_ES_ESPORADICA.ToString(),
                    RGMensajesRecogidas.Instance.CargarMensaje(RGEnumTipoErrorRecogidas.EX_RECOGIDA_NO_ES_ESPORADICA)));
                }
            }
            else
            {
                throw new FaultException<ControllerException>
                     (new ControllerException(COConstantesModulos.MENSAJERIA,
                     RGEnumTipoErrorRecogidas.EX_SOLICITUD_RECOGIDA_NO_ESPECIFICADO.ToString(),
                       RGMensajesRecogidas.Instance.CargarMensaje(RGEnumTipoErrorRecogidas.EX_SOLICITUD_RECOGIDA_NO_ESPECIFICADO)));

            }
        }

        private RGEnumClaseSolicitud TipoRecogidaToClaseRecogida(RGRecogidasDC recogida)
        {
            RGEnumClaseSolicitud resultado = RGEnumClaseSolicitud.SinSolicitud;

            switch (recogida.TipoRecogida)
            {
                case RGEnumTipoRecogidaDC.NoDefinida:
                    resultado = RGEnumClaseSolicitud.SinSolicitud;
                    break;
                case RGEnumTipoRecogidaDC.FijaCliente:
                    if (recogida.EsEsporadicaCliente)
                    {
                        resultado = RGEnumClaseSolicitud.ExporadicaClienteFijo;
                    }
                    else
                    {
                        resultado = RGEnumClaseSolicitud.FijaCliente;
                    }
                    break;
                case RGEnumTipoRecogidaDC.Esporadica:
                    resultado = RGEnumClaseSolicitud.Exporadica;
                    break;
                case RGEnumTipoRecogidaDC.FijaCentroServicio:
                    resultado = RGEnumClaseSolicitud.FijaCentroServicio;
                    break;
                default:
                    resultado = RGEnumClaseSolicitud.SinSolicitud;
                    break;
            }
            return resultado;

        }

        /// <summary>
        /// Obtiene las recogidas programadas a cierto empleado
        /// </summary>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasPorEmpleado(string idEmpleado)
        {
            return RGRepositorio.Instancia.ObtenerRecogidasPorEmpleado(idEmpleado);
        }

        /// <summary>
        /// Obtiene los registros a mostrar en el tablero de administracion de  solicitud de recogidas y generar los conteos
        /// </summary>
        /// <param name="idCentroservicio"></param>
        /// <param name="fechaConteo"></param>
        /// <returns></returns>
        public RGDetalleyConteoRecogidasDC ConsultaDetConteosRecogidas(string idCentroservicio, DateTime fechaConteo, long documento, string municipio)
        {
            var detalle = RGRepositorioAdministrador.Instancia.ConsultaDetConteosRecogidas(idCentroservicio, fechaConteo, documento, municipio);
            if (detalle != null)
            {
                return new RGDetalleyConteoRecogidasDC
                {
                    DetalleRecogidas = detalle,
                    Conteos = CuentaDetalleRecogidas(detalle),
                };
            }

            return null;
        }

        /// <summary>
        /// Inserta las recogidas esporadicas
        /// </summary>
        /// <param name="recogida"></param>
        /// <returns></returns>
        public long InsertarRecogidaEsporadica(RGRecogidasDC recogida)
        {
            if (TiposValidos(recogida))
            {
                return RGRepositorio.Instancia.InsertarSolicitudRecogida(recogida);
            }
            else
            {
                throw new FaultException("Error en contenido de datos");
            }
        }

        private bool TiposValidos(RGRecogidasDC recogida)
        {

            long lngNumero = 0;
            if (!long.TryParse(recogida.NumeroDocumento, out lngNumero))
            {
                return false;
            }

            Regex patron = new Regex(@"^\d{10}|^\d{7}");
            if (!patron.IsMatch(recogida.NumeroTelefono))
            {
                return false;
            }

            return true;
        }

        public List<RGMensajeroLocalidadDC> ObtenerMensajerosLocalidad(string idLocalidad)
        {
            return RGRepositorioAdministrador.Instancia.ObtenerMensajerosLocalidad(idLocalidad);
        }

        /// <summary>
        /// /// <summary>
        /// Obtine las posiciones de dispositivos cercanos a la coordenada cercana
        /// </summary>
        /// <param name="latitud"></param>
        /// <param name="longitud"></param>
        /// <returns></returns>        
        public List<RGDispositivoMensajeroDC> ObtenerMensajerosCercanos(decimal latitud, decimal longitud)
        {
            return RGRepositorioAdministrador.Instancia.ObtenerMensajerosCercanos(latitud, longitud);
        }

        public List<RGDispositivoMensajeroDC> ObtenerMensajerosForzarRecogida(string ubicaciones)
        {
            return RGRepositorio.Instancia.ObtenerMensajerosForzarRecogida(ubicaciones);
        }

        /// <summary>
        /// Metodo para finalizar recogida (esporadica y fija)
        /// </summary>
        /// <param name="recogida"></param>
        public void EjecutarRecogida(RGAsignarRecogidaDC recogida, int idSistema, int tipoNovedad, Dictionary<string, object> parametros, string idCiudad)
        {
            RGRepositorio.Instancia.InsertarEstadoSolRecogidaTraza(recogida, EnumEstadoSolicitudRecogida.Realizada);

            /*********************************** Se actualiza el numero de piezas de la recogida fija ************************************/
            if (recogida.NumeroPiezas != 0)
            {
                RGRepositorio.Instancia.ActualizarNumeroPiezasRecogidaFija(recogida);
            }

            /********************** Si existen imagenes de la recogida ******************************/
            if (recogida.FotografiasRecogida != null)
            {
                RGRepositorio.Instancia.InsertarFotografiasRecogida(recogida);
            }

            /*****************************Si no tiene cÃ³digo qr se genera la solicitud de rap****************************************/
            if (recogida.TipoRecogida == RGEnumTipoRecogidaDC.FijaCentroServicio || recogida.TipoRecogida == RGEnumTipoRecogidaDC.FijaCliente)
            {
                if (!recogida.TieneCodigoQR)
                {
                    RASolicitudes.Instancia.CrearSolicitudAcumulativa(idSistema, tipoNovedad, parametros, idCiudad);
                }
            }

        }

        /// <summary>
        /// Metodo para finalizar recogida (esporadica y fija)
        /// </summary>
        /// <param name="recogida"></param>
        public void EjecutarRecogidaV7(RGAsignarRecogidaDC recogida, int idSistema, int tipoNovedad, Dictionary<string, object> parametros, string idCiudad)
        {
            RGRepositorio.Instancia.InsertarEstadoSolRecogidaTraza(recogida, EnumEstadoSolicitudRecogida.Realizada);

            /*********************************** Se actualiza el numero de piezas de la recogida fija ************************************/
            if (recogida.NumeroPiezas != 0)
            {
                RGRepositorio.Instancia.ActualizarNumeroPiezasRecogidaFija(recogida);
            }

            /********************** Si existen imagenes de la recogida ******************************/
            if (recogida.FotografiasRecogida != null)
            {
                RGRepositorio.Instancia.InsertarFotografiasRecogida(recogida);
            }

            IntegrarRaps(recogida);
        }


        /// <summary>
        /// metodo para integrar la falla de recogidas segun corresponda
        /// </summary>
        /// <param name="gestion"></param>
        /// <param name="guia"></param>
        private void IntegrarRaps(RGAsignarRecogidaDC recogida)
        {
            PUAgenciaDeRacolDC colResponsable = null;
            OUDatosMensajeroDC datosMensajero = null;
            RAParametrosSolicitudAcumulativaDC parametrosSolicitudAcumulativa = new RAParametrosSolicitudAcumulativaDC();
            List<LIParametrizacionIntegracionRAPSDC> lstParametros = new List<LIParametrizacionIntegracionRAPSDC>();
            RADatosFallaDC datosFalla = null;

            /*****************************Si no tiene cÃ³digo qr se genera la solicitud de rap****************************************/
            if (!recogida.TieneCodigoQR)
            {
                RAFallaMapper ma = new RAFallaMapper();
                datosFalla = ma.MapperDatosFallaAutomaticaRecogidas(recogida, RAEnumSistemaOrigen.CONTROLLER.GetHashCode());
                if (recogida.TipoRecogida == RGEnumTipoRecogidaDC.FijaCentroServicio)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.NO_TIENE_QR_PUNTO.GetHashCode());
                }
                else if (recogida.TipoRecogida == RGEnumTipoRecogidaDC.FijaCliente)
                {
                    parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Puntos, CoEnumTipoNovedadRaps.NO_TIENE_QR_CLIENTE_CRE.GetHashCode());
                }
            }

            ///// Valida si la recogida no es esporadica y si la recogida no tuvo un retraso mayor a 15 minutos
            if ((recogida.TipoRecogida != RGEnumTipoRecogidaDC.Esporadica) && (recogida.FechaEjecucionRecogida > recogida.FechaProgramacionRecogida.AddMinutes(15)))
            {
                colResponsable = fachadaCentroServicio.ObteneColPropietarioBodega(ControllerContext.Current.IdCentroServicio);
                datosMensajero = OUAdministradorOperacionUrbana.Instancia.ObtenerDatosMensajero(ControllerContext.Current.Identificacion);
                datosFalla.IdCiudad = colResponsable.IdCiudadResponsable;
                datosFalla.NombreCompleto = datosMensajero.NombreMensajero;
                parametrosSolicitudAcumulativa = AdministradorReglaEstadoRAPS.Instancia.IntegrarRapFallas(datosFalla, RAEnumOrigenRaps.Mensajero, CoEnumTipoNovedadRaps.NO_REALIZÓ_RECOGIDA_FIJA_MENSAJERO_AUTO.GetHashCode());
            }

            /*****************************************CREA SOLICITUD ACUMULATIVA********************************************************/
            if (!parametrosSolicitudAcumulativa.EstaEnviado)
            {
                if (parametrosSolicitudAcumulativa.TipoNovedad != CoEnumTipoNovedadRaps.Pordefecto && parametrosSolicitudAcumulativa.Parametrosparametrizacion.Count > 0)
                {
                    RAIntegracionesRaps.Instancia.CrearSolicitudAcumulativaRaps((CoEnumTipoNovedadRaps)parametrosSolicitudAcumulativa.TipoNovedad.GetHashCode(), parametrosSolicitudAcumulativa.Parametrosparametrizacion, datosFalla.IdCiudad.Substring(0, 5), ControllerContext.Current == null ? "MotorRaps" : ControllerContext.Current.Usuario, datosFalla.IdSistema);
                }
            }

            else
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_RAPS, Raps.Comun.RAEnumTipoErrorClientes.EX_INSERTAR_SOLICITUD.ToString(), "La falla ya fue registrada para el responsable solicitado"));
            }
        }

        private RGConteoRecogidasDC CuentaDetalleRecogidas(List<RGDetalleConteoAdminRecogidasDC> detalle)
        {
            var resultado = new RGConteoRecogidasDC
            {
                Todas = detalle.Count,
                Nuevas = detalle.Where(d => d.FiltroConteo == "Nueva").Count(),
                Canceladas = detalle.Where(d => d.FiltroConteo == "Cancelada Cliente").Count(),
                Ejecutadas = detalle.Where(d => d.FiltroConteo == "Ejecutadas").Count(),
                FijasAsignar = detalle.Where(d => d.FiltroConteo == "Para Asignar").Count(),
                Forzar = detalle.Where(d => d.FiltroConteo == "Para Forzar").Count(),
                Reservadas = detalle.Where(d => d.FiltroConteo == "Reservada").Count(),
                Telemercadeo = detalle.Where(d => d.FiltroConteo == "Telemercadeo").Count(),
                ForzadaFueraTiempo = detalle.Where(d => d.FiltroConteo == "Forzada Fuera de Tiempo").Count(),

            };
            return resultado;
        }

        /// <summary>
        /// Edita las recogidas asignadas a un empleado con el fin de asignarselas a un nuevo empleado
        /// </summary>
        /// <param name="programacion"></param>
        public void EditarProgramacionRecogidasFijas(RGProgramacionRecogidaFijaDC programacion)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                programacion.listaRecogidas.ForEach(p =>
                {
                    programacion.IdAsignacion = p.IdAsignacion;
                    RGRepositorio.Instancia.EditarProgramacionRecogidasFijas(programacion);
                    RGRepositorio.Instancia.InsertarAuditoriaRecogidasFijas(p);
                    RGRepositorio.Instancia.EditarRecogidasFijasPendientesDeRecoger(programacion, p);

                });

                transaccion.Complete();
            }

        }

        /// <summary>
        /// Consulta agencias de las territoriales de los centros de servicios
        /// </summary>
        /// <returns></returns>
        public List<RGAgenciaDC> ObtenerAgencias()
        {
            return RGParametros.Instancia.ObtenerAgencias();
        }

        /// <summary>
        /// Consulta el detalle de una solicitud de recogida basada en la clase de solicitud
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="claseSolicitud"></param>
        /// <returns></returns>
        public RGDetalleSolicitudRecogidaDC ObtenerDetalleSolRecogida(long idSolicitudRecogida, RGEnumClaseSolicitud claseSolicitud)
        {
            RGDetalleSolicitudRecogidaDC detalle = new RGDetalleSolicitudRecogidaDC();
            detalle = RGRepositorio.Instancia.obtenerDetalleSolRecogida(idSolicitudRecogida, claseSolicitud);
            detalle.ImagenesEvidencia = RGRepositorio.Instancia.ObtenerEvidenciasRecogida(idSolicitudRecogida);
            return detalle;
        }
        /// <summary>
        /// inserta un cambio de estado de una solicitud
        /// </summary>
        /// <param name="solicitud"></param>
        public void InsertarEstadoSolRecogidaTraza(RGAsignarRecogidaDC solicitud, EnumEstadoSolicitudRecogida nuevoEstado)
        {
            RGRepositorio.Instancia.InsertarEstadoSolRecogidaTraza(solicitud, nuevoEstado);
        }

        #region Recogidas Controller App
        /// <summary>
        /// Metodo para obtener las recogidas disponibles por mensajero 
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasDisponibles(string idLocalidad)
        {
            return RGRepositorio.Instancia.ObtenerRecogidasDisponibles(idLocalidad);
        }

        /// <summary>
        /// Metodo para obtener las recogidas reservadas por mensajero
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasReservadasMensajero(string numIdentificacion)
        {
            return RGRepositorio.Instancia.ObtenerRecogidasReservadasMensajero(numIdentificacion);
        }

        /// <summary>
        /// Metodo para obtener las recogidas efectivas del mensajero 
        /// </summary>
        /// <param name="numIdentificacion"></param>
        /// <returns></returns>
        public List<RGRecogidasDC> ObtenerRecogidasEfectivasMensajero(string numIdentificacion)
        {
            return RGRepositorio.Instancia.ObtenerRecogidasEfectivasMensajero(numIdentificacion);
        }
        #endregion

        #region IVR
        public string ObtenerNombreyDireccionCliente(string telefono)
        {
            return RGRepositorio.Instancia.ObtenerNombreyDireccionCliente(telefono);
        }

        #endregion
    }
}