using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Transactions;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.Util;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.AdmEstadosConsolidado;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.Comun;

namespace CO.Servidor.OperacionNacional
{
    public partial class ONOperacionNacional : ControllerBase
    {
        private static readonly ONOperacionNacional instancia = (ONOperacionNacional)FabricaInterceptores.GetProxy(new ONOperacionNacional(), COConstantesModulos.MODULO_OPERACION_NACIONAL);

        public static ONOperacionNacional Instancia
        {
            get { return ONOperacionNacional.instancia; }
        }

        private IFachadaRutas rutas;

        /// <summary>
        /// Indica si una guía, dado su número y el id de la agencia, ya ha sido ingresada a centro de acopio pero no había sido creada la guía como tal al momento del ingreso
        /// </summary>
        /// <param name="numeroGuia"></param>.
        /// <param name="idAgencia"></param>
        /// <returns>True si la guía ya fué ingresada</returns>
        public bool GuiaYaFueIngresadaACentroDeAcopio(long numeroGuia, long idAgencia)
        {
            return ONRepositorio.Instancia.ValidarGuiaCentroAcopio(numeroGuia, idAgencia);
        }

        /// <summary>
        /// Indica si una guía, dado su número ya ha sido ingresasda a centro de copio antes de haberla creado en el sistema.
        /// </summary>
        /// <param name="numeroGuia">Retorna el id del centro de servicio que ingresó a centro de acopio la guía</param>
        /// <returns></returns>
        public long GuiaYaFueIngresadaACentroDeAcopioRetornaCentroServicio(long numeroGuia)
        {
            return ONRepositorio.Instancia.GuiaYaFueIngresadaACentroDeAcopioRetornaCentroServicio(numeroGuia);
        }

        /// <summary>
        /// constructor
        /// </summary>
        private ONOperacionNacional()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(PAAdministrador.Instancia.ConsultarParametrosFramework("Cultura"));
            rutas = COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>();
        }

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
            return ONRepositorio.Instancia.ObtenerManifiestosOperacionNacional(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta, incluyeFecha);
        }

        /// <summary>
        /// Obtiene manifiesto de la guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ONManifiestoOperacionNacional ObtenerResponsableGuiaManifiestoPorNGuia(long numeroGuia)
        {
            return ONRepositorio.Instancia.ObtenerResponsableGuiaManifiestoPorNGuia(numeroGuia);
        }
        /// <summary>
        /// para obtener parametros guias en novedades de ruta cuando se requiera asignar novedad a guias especificas
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="guiaxNovIndv"></param>
        /// <returns></returns>
        public List<ONParametrosGuiaInvNovedadRuta> ObtenerParametrosGuiaIndvPorNovedadRuta(long numeroGuia, ONEnumGuiaIndvNovedadRuta guiaxNovIndv, string tipoUbicacion)
        {
            return ONRepositorio.Instancia.ObtenerParametrosGuiaIndvPorNovedadRuta(numeroGuia, guiaxNovIndv, tipoUbicacion);
        }
        /// <summary>
        /// Obtiene los manifiestos de la operacion nacional con Novedades
        /// </summary>
        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_ConNovRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta)
        {
            return ONRepositorio.Instancia.ObtenerManifiestosOpeNal_ConNovRuta(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta);
        }

        /// <summary>
        /// Obtiene los manifiestos de la operacion nacional Disponibles para asignar Novedades
        /// </summary>
        public IList<ONManifiestoOperacionNacional> ObtenerManifiestosOpeNal_Disponibles(long idCentroServicios, int idRuta, DateTime fechaInicial, DateTime fechaFinal)
        {
            return ONRepositorio.Instancia.ObtenerManifiestosOpeNal_Disponibles(idCentroServicios, idRuta, fechaInicial, fechaFinal);
        }

        /// <summary>
        /// Obtiene las Novedades Ruta [ORI],[DES],[OPE]
        /// </summary>
        public IList<ONNovedadEstacionRutaDC> Obtener_NovedadesEstacionRuta(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idCentroServiciosManifiesta)
        {
            return ONRepositorio.Instancia.Obtener_NovedadesEstacionRuta(filtro, indicePagina, registrosPorPagina, idCentroServiciosManifiesta);
        }

        public ONManifiestoOperacionNacional Obtener_ManifiestoxId(long IdManifiesto)
        {
            return ONRepositorio.Instancia.Obtener_ManifiestoxId(IdManifiesto);
        }


        /// EMRL ajuste para solicitud de horarios de ruta
        /// <summary>
        /// Consulta los horarios de una ruta dada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public List<ONHorarioRutaDC> ConsultarHorariosRuta(int idRuta)
        {
            List<ONHorarioRutaDC> horarios = new List<ONHorarioRutaDC>();
            horarios = ONRepositorio.Instancia.ConsultarHorariosRuta(idRuta, ObtenerDiasValidosConsulta(DateTime.Now));

            if (horarios != null && horarios.Count > 0)
            {
                //Filtrar solo 8 horas antes de la hora actual del dia anterior
                return horarios.Where(d => !d.IdDia.Trim().Equals(ObtenerDiaAnterior(DateTime.Now).Trim())
                        || (d.IdDia.Equals(ObtenerDiaAnterior(DateTime.Now)) && ValidarHorarioDiaAnterior(d.HoraSalida))).ToList();
            }
            else
                return horarios;
        }

        private bool ValidarHorarioDiaAnterior(string hora)
        {
            string[] arreglohora = hora.Split(':');
            if (arreglohora != null && arreglohora.Length == 3)
            {
                DateTime tiempo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt16(arreglohora[0]), Convert.ToInt16(arreglohora[1]), Convert.ToInt16(arreglohora[2]));
                if (DateTime.Now.Subtract(tiempo.AddDays(-1)).Hours > 8)
                    return false;
                else
                    return true;
            }
            else
                return false;
        }


        private string ObtenerDiasValidosConsulta(DateTime fecha)
        {
            switch (fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")))
            {
                case "lunes":
                    return "1,2,3,7";
                case "martes":
                    return "1,2,3,4";
                case "miércoles":
                    return "2,3,4,5";
                case "jueves":
                    return "3,4,5,6";
                case "viernes":
                    return "4,5,6,7";
                case "sábado":
                    return "5,6,7,1";
                case "domingo":
                    return "6,7,1,2";
                default:
                    return "1,2,3,4,5,6,7";
            }
        }

        private string ObtenerDiaAnterior(DateTime fecha)
        {
            switch (fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES")))
            {
                case "lunes":
                    return "7";
                case "martes":
                    return "1";
                case "miércoles":
                    return "2";
                case "jueves":
                    return "3";
                case "viernes":
                    return "4";
                case "sábado":
                    return "5";
                case "domingo":
                    return "6";
                default:
                    return "1";
            }
        }

        /// <summary>
        /// Obtiene una lista de los vehiculos activos que pertenecen a un racol y que tienen ingreso a un col
        /// </summary>
        /// <param name="idRacol">Id del racol al que pertenece el vehiculo</param>
        /// <param name="idCentroServicios">Id del centro de servicios al que ingreso el vehiculo</param>
        /// <param name="EsCol">Indica si el usuario esta logeado en un col</param>
        /// <returns></returns>
        public IList<POVehiculo> ObtenerVehiculosIngresoCentroServicioXRacol(long idRacol, long idCentroServicios, bool esCol)
        {
            DateTime? fechaActual = DateTime.Now;
            fechaActual = new DateTime(fechaActual.Value.Year, fechaActual.Value.Month, fechaActual.Value.Day, 00, 00, 00);

            if (esCol)
                return ONRepositorio.Instancia.ObtenerVehiculosIngresoCentroServicioXRacol(idRacol, idCentroServicios, fechaActual);
            else
                return ONRepositorio.Instancia.ObtenerVehiculosManifestarSinIngresoCentroServicioXRacol(idRacol, fechaActual);
        }

        /// <summary>
        /// Inserta, actualiza un manifiesto nacional
        /// </summary>
        /// <param name="manifiesto"></param>
        public ONManifiestoOperacionNacional ActualizarManifiestoOperacionNacional(ONManifiestoOperacionNacional manifiesto)
        {
            switch (manifiesto.EstadoRegistro)
            {
                case EnumEstadoRegistro.ADICIONADO:
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        //Genera el manifiesto para el ministerio
                        if (manifiesto.RutaGeneraManifiestoMinisterio)
                        {
                            //Valido si el centro autenticado esta en la ciudad origen de la ruta
                            if (manifiesto.IdLocalidadOrigenRuta != manifiesto.LocalidadDespacho.IdLocalidad || manifiesto.IdTipoTransporte == OPConstantesOperacionNacional.ID_TIPO_TRANSPORTE_AFORADO)
                            {
                                manifiesto.NumeroManifiestoCarga = 0;
                            }
                            else
                            {
                                long consecutivo = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Manifiesto_Ministerio);
                                manifiesto.NumeroManifiestoCarga = consecutivo;
                            }
                        }


                        string idManifiesto = manifiesto.IdRutaDespacho.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString().Substring(2) + PAParametros.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Manifiesto_Operacion_Nacional).ToString().PadLeft(7, '0');
                        manifiesto.IdManifiestoOperacionNacional = long.Parse(idManifiesto);
                        ONRepositorio.Instancia.AgregarManifiestoOperacionNacional(manifiesto);

                        //Generacion de Manifiesto Interno
                        if (manifiesto.ManifiestoTerrestre != null && manifiesto.ManifiestoTerrestre.IdVehiculo > 0)
                        {
                            manifiesto.ManifiestoTerrestre.IdManifiestoOperacionNacional = manifiesto.IdManifiestoOperacionNacional;
                            ONRepositorio.Instancia.AgregarManifiestoOperacionNacionalTerrestre(manifiesto.ManifiestoTerrestre);
                        }
                        transaccion.Complete();
                        return manifiesto;
                    }
                case EnumEstadoRegistro.MODIFICADO:
                    using (TransactionScope transaccion = new TransactionScope())
                    {
                        ONRepositorio.Instancia.EditarManifiestoOperacionNacional(manifiesto);

                        if (manifiesto.ManifiestoTerrestre != null && manifiesto.ManifiestoTerrestre.IdVehiculo > 0 && manifiesto.IdTipoTransporte == OPConstantesOperacionNacional.ID_TIPO_TRANSPORTE_PROPIO)
                        {
                            manifiesto.ManifiestoTerrestre.IdManifiestoOperacionNacional = manifiesto.IdManifiestoOperacionNacional;

                            if (ONRepositorio.Instancia.ValidarExisteManifiestoOperacionNacionalTerrestre(manifiesto.IdManifiestoOperacionNacional))
                            {

                                ONRepositorio.Instancia.EditarManifiestoOperacionNacionalTerrestre(manifiesto.ManifiestoTerrestre);
                            }
                            else
                            {
                                ONRepositorio.Instancia.AgregarManifiestoOperacionNacionalTerrestre(manifiesto.ManifiestoTerrestre);
                            }
                        }
                        else
                        {
                            ONRepositorio.Instancia.EliminarManifiestoOperacionNacionalTerrestre(manifiesto.IdManifiestoOperacionNacional);
                        }
                        transaccion.Complete();
                    }
                    return manifiesto;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Obtiene los consolidados de un manifiesto de la operacion nacional
        /// </summary>
        /// <param name="idManifiestoOperacionNacional">Identificador del manifiesto de la operacion nacional</param>
        /// <returns></returns>
        public IList<ONConsolidado> ObtenerConsolidadosManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional, string idLocalidad)
        {
            return ONRepositorio.Instancia.ObtenerConsolidadosManifiesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idManifiestoOperacionNacional, idLocalidad);
        }

        /// <summary>
        /// Consulta las estaciones de una ruta y la cantidad de envíos manifestados en un manifiesto específico x cada estación
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public List<ONCantEnviosManXEstacionDC> ConsultarCantEnviosManXEstacion(int idRuta, long idManifiesto)
        {
            return ONRepositorio.Instancia.ConsultarCantEnviosManXEstacion(idRuta, idManifiesto);
        }

        /// <summary>
        /// Obtiene todos los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidado> ObtenerTipoConsolidado()
        {
            return ONRepositorio.Instancia.ObtenerTipoConsolidado();
        }


        /// <summary>
        /// Obtiene todos los detalles de los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoConsolidadoDetalle> ObtenerTipoConsolidadoDetalle(int idTipoConsolidado)
        {
            return ONRepositorio.Instancia.ObtenerTipoConsolidadoDetalle(idTipoConsolidado);
        }


        // TODO:ID Obtener Lista Tipos de Novedad Estacion-Ruta
        public IList<ONTipoNovedadEstacionRutaDC> ObtenerTiposNovedadEstacionRuta()
        {
            List<COTipoNovedadGuiaDC> listNovedades = EGTipoNovedadGuia.ObtenerTiposNovedadGuia(COEnumTipoNovedad.RUTA);


            return listNovedades.ConvertAll<ONTipoNovedadEstacionRutaDC>(n =>
            {
                ONTipoNovedadEstacionRutaDC obj = new ONTipoNovedadEstacionRutaDC() { IdNovedad = n.IdTipoNovedadGuia, Descripcion = n.Descripcion };
                return obj;
            });

        }

        // TODO:ID 
        public void AdicionarNovedadEstacionRuta(List<ONNovedadEstacionRutaDC> lstNovedadesEstacioRuta)
        {
            long newIdnov;

            List<long> lstIdsNovANotificar = new List<long>();
            using (TransactionScope transaccion = new TransactionScope())
            {
                foreach (ONNovedadEstacionRutaDC item in lstNovedadesEstacioRuta)
                {
                    newIdnov = ONRepositorio.Instancia.AdicionarNovedadEstacionRuta(item);
                    if (item.Retraso > 0)  // Solo se notifica a los clientes cuando la Novedad Genera Retraso
                        lstIdsNovANotificar.Add(newIdnov);
                }
                transaccion.Complete();
            }


            //// 3. Aca Notificamos a los Clientes Mediante email(Clientes Credito) y SMS (Clientes Contado o Alcobro)
            #region Notificacion de la Novedad
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {

                try
                {
                    bool rtaParametroEsPruebas = true;
                    bool.TryParse(PAAdministrador.Instancia.ConsultarParametrosFramework("EsAmbientePruebas"), out rtaParametroEsPruebas);

                    if (!rtaParametroEsPruebas)
                        ONRepositorio.Instancia.ObtenerGuiasParaNotificarNovedadesRuta(lstIdsNovANotificar);

                }
                catch (Exception exc)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MODULO_OPERACION_NACIONAL);
                }

            }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
            #endregion


        }


        public void AdicionarNovedadEstacionRutaPorGuia(List<ONNovedadEstacionRutaxGuiaDC> lstNovedadesEstacioRuta)
        {
            long newIdnov;

            List<long> lstIdsNovANotificar = new List<long>();
            using (TransactionScope transaccion = new TransactionScope())
            {
                foreach (ONNovedadEstacionRutaxGuiaDC item in lstNovedadesEstacioRuta)
                {
                    newIdnov = ONRepositorio.Instancia.AdicionarNovedadEstacionRutaCargueDatos(item);
                    if (item.NovedadRutaEstacion.Retraso > 0)  // Solo se notifica a los clientes cuando la Novedad Genera Retraso
                        lstIdsNovANotificar.Add(newIdnov);
                }
                transaccion.Complete();
            }


            //// 3. Aca Notificamos a los Clientes Mediante email(Clientes Credito) y SMS (Clientes Contado o Alcobro)
            #region Notificacion de la Novedad
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {

                try
                {
                    bool rtaParametroEsPruebas = true;
                    bool.TryParse(PAAdministrador.Instancia.ConsultarParametrosFramework("EsAmbientePruebas"), out rtaParametroEsPruebas);

                    if (!rtaParametroEsPruebas)
                        ONRepositorio.Instancia.ObtenerGuiasParaNotificarNovedadesRuta(lstIdsNovANotificar);

                }
                catch (Exception exc)
                {
                    AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, exc.ToString(), COConstantesModulos.MODULO_OPERACION_NACIONAL);
                }

            }, System.Threading.Tasks.TaskCreationOptions.PreferFairness);
            #endregion

        }



        /// <summary>
        /// Inserta un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public ONConsolidado AdicionarConsolidado(ONConsolidado consolidado, long idCentroServicios)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {

                if (ONRepositorio.Instancia.ObtenerCantidadConsolidadosXContenedorTulaYManifiesto(consolidado.IdManifiestoOperacionNacional, consolidado.NumeroContenedorTula) > 0)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_GUIA_NO_ES_INTERNA.ToString(), string.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CONTENEDORTULA_YA_MANIFESTADO), consolidado.NumeroContenedorTula)));




                ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

                //validacion de tulas  
                /*
               List<SUConsolidadoDC> lstConsolidados =  fachadaSuministros.ObtenerInventarioConsolidado(consolidado.NumeroContenedorTula);
                
               if (lstConsolidados.Count > 0)
               {


                   bool tulaEnRuta = false;

                   lstConsolidados.ForEach(c =>
                   {
                       if (c.IdLocalidad == consolidado.LocalidadManifestada.IdLocalidad)
                       {
                           tulaEnRuta = true;
                       }
                   });

                   if (!tulaEnRuta)
                   {

                       List<RUEstacionRuta> estacionesRuta = COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEstacionesYLocalidadesAdicionalesRuta(consolidado.IdRutaDespacho).ToList();
                       var est = estacionesRuta.Where(e => e.IdLocalidadEstacion == consolidado.LocalidadManifestada.IdLocalidad).ToList();
                       foreach (var es in est )                      
                       {
                           if (lstConsolidados.Where(c => es.CiudadEstacion.IdLocalidad == c.IdLocalidad).Count() > 0)
                           {
                               tulaEnRuta = true;
                           }

                           if (es.CiudadesHijas != null)
                               es.CiudadesHijas.ForEach(ch =>
                                   {
                                       if (lstConsolidados.Where(c => ch.EstacionHija.IdLocalidad == c.IdLocalidad).Count() > 0)
                                       {
                                           tulaEnRuta = true;
                                       }
                                   });

                           if (tulaEnRuta)
                               break;
                       }
                   }

                   if (!tulaEnRuta)
                   {
                       foreach (var c in lstConsolidados)
                       {
                           if (consolidado.LocalidadManifestada.IdLocalidad != c.IdLocalidad)
                           {
                               ValidarOrigenDestinoManifiesto(consolidado.IdLocalidadDespacha, c.IdLocalidad, c.NombreLocalidad, consolidado.LocalidadManifestada.IdLocalidad, consolidado.IdRutaDespacho,true);
                               tulaEnRuta = true;
                               break;
                           }
                       }
                   }

                   if (tulaEnRuta)
                   {

                       //verificar estado del consolidado si no esta creado o descargado
                       EnumEstadosConsolidados estadoTula = ECAdminEstadosConsolidado.ObtenerUltimoEstadoConsolidado(consolidado.NumeroContenedorTula);


                       if (estadoTula != EnumEstadosConsolidados.CRE && estadoTula != EnumEstadosConsolidados.DES)
                       {
                           throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_YA_ESTA_UTILIZADA_OTRO_PROCESO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_YA_ESTA_UTILIZADA_OTRO_PROCESO)));
                       }
                       //Actualiza el estado del consolidado a manifestado
                       ECAdminEstadosConsolidado.GuardarEstadoConsolidado(new ECEstadoConsolidado() { Estado = EnumEstadosConsolidados.MAN, IdCentroServicios = idCentroServicios, NoTula = consolidado.NumeroContenedorTula, Observaciones = "" });
                   }
                   else
                   {
                       throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                   }


                   
               }
               else
               {
                   throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_EXISTE_TULA_CONSOLIDADO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_EXISTE_TULA_CONSOLIDADO)));
               }
                */


                if (consolidado.NumeroPrecintoRetorno != null && consolidado.NumeroPrecintoRetorno > 0)
                {
                    SUPropietarioGuia PropietarioGuia = fachadaSuministros.ObtenerPropietarioSuministro(consolidado.NumeroPrecintoRetorno.Value, SUEnumSuministro.PRECINTO_SEGURIDAD, idCentroServicios);
                    GuardarConsumoPrecintoSeguridad((long)consolidado.NumeroPrecintoRetorno, PropietarioGuia.Propietario, PropietarioGuia.Id);
                }
                if (consolidado.NumeroPrecintoSalida != null && consolidado.NumeroPrecintoSalida > 0)
                {
                    SUPropietarioGuia PropietarioGuia = fachadaSuministros.ObtenerPropietarioSuministro(consolidado.NumeroPrecintoSalida.Value, SUEnumSuministro.PRECINTO_SEGURIDAD, idCentroServicios);
                    GuardarConsumoPrecintoSeguridad((long)consolidado.NumeroPrecintoSalida, PropietarioGuia.Propietario, PropietarioGuia.Id);
                }






                ONConsolidado consolidadoGuardado = null;

                List<RUEstacionRuta> estaciones = COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>().ObtenerEstacionesRuta(consolidado.IdRutaDespacho).ToList();

                RUEstacionRuta estacion = estaciones.Where(e => e.IdLocalidadEstacion == consolidado.LocalidadManifestada.IdLocalidad).FirstOrDefault();
                int orden = 0;
                if (estacion != null)
                {
                    orden = estacion.OrdenEnRuta;
                }

                string numContTransIda = consolidado.IdRutaDespacho.ToString() + orden.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString().Substring(2) + PAParametros.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Control_Transporte_Manifiesto_Nacional_Despacho);
                string numContTransRet = null;

                PUCentroServiciosDC centroServicio = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(consolidado.LocalidadManifestada.IdLocalidad);
                if (centroServicio != null)
                {
                    if (!centroServicio.Sistematizado)
                    {
                        numContTransRet = consolidado.IdRutaDespacho.ToString() + orden.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString().Substring(2) + PAParametros.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Control_Transporte_Manifiesto_Nacional_Despacho);
                    }
                }

                consolidado.NumControlTransManIda = long.Parse(numContTransIda);

                if (!string.IsNullOrEmpty(numContTransRet))
                {
                    consolidado.NumControlTransManRet = long.Parse(numContTransRet);
                }

                if (consolidado.NumeroGuiaInterna != null)
                {
                    ADGuia Guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(consolidado.NumeroGuiaInterna.Value);
                    if (Guia == null)
                    {
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_GUIA_NO_ES_INTERNA.ToString(), string.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_GUIA_NO_ES_INTERNA), consolidado.NumeroGuiaInterna)));
                    }
                    consolidadoGuardado = ONRepositorio.Instancia.AdicionarConsolidado(consolidado, Guia.IdAdmision);
                }
                else
                {
                    consolidadoGuardado = ONRepositorio.Instancia.AdicionarConsolidado(consolidado);
                }

                transaccion.Complete();

                return consolidadoGuardado;
            }
        }

        /// <summary>
        /// Modifica un consolidado
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public void EditarConsolidado(ONConsolidado consolidado, long idCentroServicios)
        {

            ONConsolidado conso = ONRepositorio.Instancia.ObtenerConsolidadoManifiestoIdConsoMani(consolidado);

            if (conso != null && consolidado.NumeroContenedorTula != consolidado.NumeroContenedorTula)
            {
                if (ONRepositorio.Instancia.ObtenerCantidadConsolidadosXContenedorTulaYManifiesto(consolidado.IdManifiestoOperacionNacional, consolidado.NumeroContenedorTula) > 0)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_GUIA_NO_ES_INTERNA.ToString(), string.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CONTENEDORTULA_YA_MANIFESTADO), consolidado.NumeroContenedorTula)));
            }

            ADGuia Guia = null;
            if (consolidado.NumeroGuiaInterna != null)
                Guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerInfoGuiaXNumeroGuia(consolidado.NumeroGuiaInterna.Value);

            using (TransactionScope transaccion = new TransactionScope())
            {
                if (Guia != null && Guia.IdAdmision != 0)
                    ONRepositorio.Instancia.EditarConsolidado(consolidado, Guia.IdAdmision);
                else
                    ONRepositorio.Instancia.EditarConsolidado(consolidado);


                transaccion.Complete();
            }
        }

        /// <summary>
        /// Obtiene las guias de un consolidado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public IList<ONConsolidadoDetalle> ObtenerGuiasConsolidado(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idConsolidado)
        {
            return ONRepositorio.Instancia.ObtenerGuiasConsolidado(filtro, indicePagina, registrosPorPagina, idConsolidado);
        }

        private bool CambiarLocalidadManifestada = false;
        private string idCiudadDestinoRuta = "";

        /// <summary>
        /// Valida que haya almenos una interconexion entre la ciudad destino de la ruta con la ciudad destino de la guia
        /// </summary>
        /// <param name="idRuta">Id de la ruta</param>
        /// <param name="idLocalidadDestinoGuia">Id de la localidad destino de la guia</param>
        /// <returns></returns>
        private bool ValidarInterconexionRutas(int idRuta, string idLocalidadDestinoGuia)
        {
            RURutaDC ruta = rutas.ObtenerRutaIdRuta(idRuta);
            if (ruta != null)
            {
                RURutaOptimaCalculada rutaOptima = rutas.CalcularRutaOptima(ruta.IdLocalidadDestino, idLocalidadDestinoGuia);
                if (rutaOptima.TodosLosCaminos.Count <= 0)
                {
                    return false;
                }
                else
                {
                    CambiarLocalidadManifestada = true;
                    idCiudadDestinoRuta = ruta.IdLocalidadDestino;
                    return true;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// Valida que haya almenos una interconexion entre la ciudad a la que manifiesta con la ciudad destino de la guia
        /// </summary>
        /// <param name="idRuta">Id de la ruta</param>
        /// <param name="idLocalidadDestinoGuia">Id de la localidad destino de la guia</param>
        /// <returns></returns>
        private bool ValidarInterconexionRutas(string idLocalidadManifiesta, string idLocalidadDestinoGuia)
        {
            RURutaOptimaCalculada rutaOptima = rutas.CalcularRutaOptima(idLocalidadManifiesta, idLocalidadDestinoGuia);
            if (rutaOptima != null)
            {
                if (rutaOptima.TodosLosCaminos.Count <= 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
                return false;
        }

        /// <summary>
        /// obtiene una lista de guias en el centro de acopio, que debieron haber sido agregadas al manifiesto
        /// </summary>
        /// <param name="manifiesto"></param>
        /// <returns></returns>
        public List<ADGuia> ObtenerEnviosPendientesPorManifestar(ONManifiestoOperacionNacional manifiesto)
        {
            List<ADGuia> lstGuias = ONRepositorio.Instancia.ConsultarEnviosPendientesXManifestar(manifiesto.LocalidadDespacho.IdLocalidad, manifiesto.IdRutaDespacho);
            return lstGuias;
        }

        /// <summary>
        /// Obtiene todas las guias sueltas de un manifiesto nacional
        /// </summary>
        /// <param name="idManifiestoOpeNacional"></param>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerGuiasSueltas(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina, long idManifiestoOpeNacional)
        {
            return ONRepositorio.Instancia.ObtenerGuiasSueltas(filtro, indicePagina, registrosPorPagina, idManifiestoOpeNacional);
        }

        /// <summary>
        /// Obtiene todos los motivos de eliminacion de una guia
        /// </summary>
        /// <returns></returns>
        public IList<ONTipoMotivoElimGuiaMani> ObtenerTodosMotivosEliminacionGuia()
        {
            return ONRepositorio.Instancia.ObtenerTodosMotivosEliminacionGuia();
        }

        /// <summary>
        /// Elimina una guia suelta de un manifiesto o una guia de un consolidado de un manifiesto
        /// </summary>
        /// <param name="motivo"></param>
        public void EliminarGuiaManifiesto(ONMotivoElimGuiaMani motivo)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                ADGuia Guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(motivo.NumeroGuia);

                //Realizar el cambio de estado de la guia
                ADTrazaGuia trazaGuia = new ADTrazaGuia()
                {
                    IdAdmision = Guia.IdAdmision,
                    NumeroGuia = Guia.NumeroGuia,
                    Observaciones = string.Empty,
                    IdCiudad = motivo.IdLocalidadDespacho,
                    Ciudad = motivo.NombreLocalidadDespacho,
                    Modulo = COConstantesModulos.MODULO_OPERACION_NACIONAL,
                    IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(Guia.NumeroGuia),
                    IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio
                };


                trazaGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);
                if (trazaGuia.IdTrazaGuia == 0)
                {
                    string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                    , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                    , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                    excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                    throw new FaultException<ControllerException>(excepcion);
                }


                if (motivo.GuiaSuelta)
                    ONRepositorio.Instancia.EliminarGuiaSueltaManifiesto(motivo.IdManifiestoGuia);
                else
                    ONRepositorio.Instancia.EliminarGuiaConsolidado(motivo.IdManifiestoConsolidadoDetalle);

                ONRepositorio.Instancia.AdicionarMotivoEliminacionGuia(motivo);

                transaccion.Complete();
            }
        }

        /// <summary>
        /// Cierra un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void CerrarManifiesto(long idManifiesto, DateTime fechaSalidaSatrack)
        {
            ONRepositorio.Instancia.CerrarManifiesto(idManifiesto, fechaSalidaSatrack);
        }

        /// <summary>
        /// Reabre un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void AbrirManifiesto(ONManifiestoOperacionNacional manifiesto)
        {
            TimeSpan hora = DateTime.Now.Subtract(manifiesto.FechaCierre);

            if (hora.Hours <= int.Parse(ONRepositorio.Instancia.ObtenerParametroOperacionNacional("PlazoHorasAbrirMani")))
            {
                ONRepositorio.Instancia.AbrirManifiesto(manifiesto.IdManifiestoOperacionNacional);
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_ABRIR_MANIFIESTO));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Genera el reporte del manifiesto de carga
        /// </summary>
        /// <param name="idManifiesto"></param>
        public ONReporteManifiestoCarga GenerarReporteManifiestoCarga(long idManifiesto, long IdCentroServiciosManifiesta)
        {
            Dictionary<string, string> filtro = new Dictionary<string, string>();
            filtro.Add("MON_IdManifiestoOperacionNacio", idManifiesto.ToString());
            ONManifiestoOperacionNacional manifiesto = ONRepositorio.Instancia.ObtenerManifiestosOperacionNacional(filtro, 0, 10, IdCentroServiciosManifiesta, false).FirstOrDefault();

            if (manifiesto != null)
            {
                if (manifiesto.ManifiestoTerrestre != null)//Valida que existan los datos del vehiculo y del conductor del vehiculo
                {
                    decimal pesoEnvios = ONRepositorio.Instancia.ObtenerPesoEnviosManifiesto(idManifiesto);
                    string nombreAseguradoraGeneral = ONRepositorio.Instancia.ObtenerParametroOperacionNacional(OPConstantesOperacionNacional.NOMBRE_ASEGURADORA_GENERAL);
                    string polizaAseguradoraGeneral = ONRepositorio.Instancia.ObtenerParametroOperacionNacional(OPConstantesOperacionNacional.NUMERO_POLIZA_ASEGURADORA_GENERAL);
                    AREmpresaDC empresa = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>().ObtenerDatosEmpresa();
                    IFachadaRutas FRutas = COFabricaDominio.Instancia.CrearInstancia<IFachadaRutas>();
                    RURutaDC ruta = FRutas.ObtenerRutaIdRuta(manifiesto.IdRutaDespacho);

                    decimal valorAPagarPactado = ruta.CostoMensualRuta / 25;
                    string valorAPagarPactadoLetras = ConvertirNumerosLetras.enletras(valorAPagarPactado.ToString(), true);
                    IPOFachadaParametrosOperacion FParametrosOperacion = COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>();
                    filtro.Clear();
                    filtro.Add("VEH_IdVehiculo", manifiesto.ManifiestoTerrestre.IdVehiculo.ToString());
                    int total = 0;
                    POVehiculo vehiculo = FParametrosOperacion.ObtenerVehiculos(filtro, "", 0, 10, true, out total).FirstOrDefault();
                    if (vehiculo != null)
                    {
                        pesoEnvios = pesoEnvios > vehiculo.Capacidad ? vehiculo.Capacidad : pesoEnvios;

                        filtro.Clear();
                        filtro.Add("Identificador", manifiesto.ManifiestoTerrestre.IdConductor.ToString());
                        filtro.Add("PEI_Identificacion", manifiesto.ManifiestoTerrestre.CedulaConductor.ToString());
                        POMensajero conductor = FParametrosOperacion.ObtenerMensajerosConductores(filtro, "", 0, 10, true, out total).FirstOrDefault();

                        if (conductor != null)
                        {
                            ONReporteManifiestoCarga reporte = new ONReporteManifiestoCarga()
                            {
                                Conductor = conductor,
                                Empresa = empresa,
                                Ruta = ruta,
                                Vehiculo = vehiculo,
                                Manifiesto = manifiesto,
                                PesoEnvios = pesoEnvios,
                                NombreAseguradoraGeneral = nombreAseguradoraGeneral,
                                NumPolizaAseguradoraGeneral = polizaAseguradoraGeneral,
                                FechaGeneracionReporteManifiesto = DateTime.Now,
                                ValorAPagarPactado = valorAPagarPactado,
                                ValorAPagarPactadoLetras = valorAPagarPactadoLetras,
                                FechaPagoSaldo = manifiesto.FechaCierre.AddDays(3)
                            };

                            PUCentroServiciosDC centroOrigen = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(manifiesto.IdCentroServiciosManifiesta);
                            reporte.DireccionCentroServOrigen = centroOrigen.Direccion;
                            PUCentroServiciosDC centroDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgenciaLocalidad(ruta.IdLocalidadDestino);
                            reporte.DireccionCentroServDestino = centroDestino.Direccion;
                            return reporte;
                        }
                        else
                        {
                            ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_OBTENER_DATOS_CONDUCTOR));
                            throw new FaultException<ControllerException>(excepcion);
                        }
                    }
                    else
                    {
                        ONReporteManifiestoCarga reporte = new ONReporteManifiestoCarga()
                        {
                            Conductor = null,
                            Empresa = empresa,
                            Ruta = ruta,
                            Vehiculo = null,
                            Manifiesto = manifiesto,
                            PesoEnvios = pesoEnvios,
                            NombreAseguradoraGeneral = nombreAseguradoraGeneral,
                            NumPolizaAseguradoraGeneral = polizaAseguradoraGeneral,
                            FechaGeneracionReporteManifiesto = DateTime.Now,
                            ValorAPagarPactado = valorAPagarPactado,
                            ValorAPagarPactadoLetras = valorAPagarPactadoLetras,
                            FechaPagoSaldo = manifiesto.FechaCierre.AddDays(3)
                        };
                        return reporte;
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_MANIFIESTO_SIN_VEHICULO_CONFIGURADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            else
            {
                //ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), "Manifiesto no generado");
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Obtiene todas las guias manifestadas, incluye guias sueltas y guias consolidadas
        /// </summary>
        /// <returns></returns>
        public IList<ONManifiestoGuia> ObtenerTodasGuiasManifiesto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idManifiestoOperacionNacional)
        {
            return ONRepositorio.Instancia.ObtenerTodasGuiasManifiesto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idManifiestoOperacionNacional);
        }

        /// <summary>
        /// Inserta un envio al consolidado
        /// </summary>
        /// <param name="consolidadoDetalle"></param>
        /// <returns></returns>
        public ONConsolidadoDetalle AdicionarConsolidadoDetalle(ONConsolidadoDetalle consolidadoDetalle)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                ADTrazaGuia trazaGuia = new ADTrazaGuia();
                CambiarLocalidadManifestada = false;
                ADGuia Guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(consolidadoDetalle.NumeroGuia.Value);

                if (Guia == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_GUIA_NO_EXISTE.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_GUIA_NO_EXISTE)));
                }

                PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgencia(consolidadoDetalle.IdCentroServicioOrigen);

                ADTrazaGuia traza = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(consolidadoDetalle.NumeroGuia.Value);
                //obsolete
                //if (!EstadosGuia.ValidarGuiaIngresoCentroAcopio(consolidadoDetalle.NumeroGuia.Value, consolidadoDetalle.IdLocalidadDespacho))
                //{
                //    //Si el centro de servicios es una agencia se debe dar ingreso automatico al centro de acopio
                //    if (centroServ.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA && centroServ.TipoSubtipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_COL)
                //    {
                //        OUPlanillaVentaGuiasDC guiaIngresada = new OUPlanillaVentaGuiasDC()
                //        {

                //            IdCiudadOrigenGuia = consolidadoDetalle.IdLocalidadDespacho,
                //            NumeroGuia = consolidadoDetalle.NumeroGuia.Value,
                //            PiezaActualRotulo = consolidadoDetalle.PiezaActual,
                //            TotalPiezasRotulo = consolidadoDetalle.TotalPiezas,
                //            IdPuntoServicio = consolidadoDetalle.IdCentroServicioOrigen

                //        };

                //        guiaIngresada = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().IngresarGuiaSueltaCentroAcopio(guiaIngresada, null);

                //    }
                //    else
                //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO)));
                //}


                //Verificar que la guia no haya sido agregada como una guia suelta
                if (ONRepositorio.Instancia.VerificarExisteGuiaSueltaManifiesto(consolidadoDetalle.NumeroGuia.Value, consolidadoDetalle.IdManifiestoOperacionNacional))
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA)));

                ///Verificar que la guía no haya sigo agregada a un consolidado
                if (ONRepositorio.Instancia.VerificarExisteGuiaConsolidadoManifiesto(consolidadoDetalle.NumeroGuia.Value, consolidadoDetalle.IdManifiestoOperacionNacional))
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA)));


                if (traza.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio && traza.IdCiudad != consolidadoDetalle.IdLocalidadDespacho)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO)));

                //obsolete
                //if (consolidadoDetalle.IdCentroServicioOrigen != Guia.IdCentroServicioOrigen)
                //    if (!EstadosGuia.ValidarGuiaIngresoCentroAcopio(consolidadoDetalle.NumeroGuia.Value, consolidadoDetalle.IdLocalidadDespacho))
                //        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO)));

                consolidadoDetalle.IdAdminisionMensajeria = Guia.IdAdmision;
                consolidadoDetalle.PesoEnIngreso = Guia.Peso;
                consolidadoDetalle.EstaDescargada = false;

                consolidadoDetalle.IdTipoEnvio = Guia.IdTipoEnvio;
                consolidadoDetalle.NombreTipoEnvio = Guia.NombreTipoEnvio;
                consolidadoDetalle.IdCentroServicioOrigen = Guia.IdCentroServicioOrigen;
                consolidadoDetalle.NombreCentroServicioOrigen = Guia.NombreCentroServicioOrigen;
                consolidadoDetalle.IdCentroServicioDestino = Guia.IdCentroServicioDestino;
                consolidadoDetalle.NombreCentroServicioDestino = Guia.NombreCentroServicioDestino;
                consolidadoDetalle.IdCiudadDestino = Guia.IdCiudadDestino;
                consolidadoDetalle.NombreCiudadDestino = Guia.NombreCiudadDestino;
                consolidadoDetalle.IdCiudadOrigen = Guia.IdCiudadOrigen;
                consolidadoDetalle.NombreCiudadOrigen = Guia.NombreCiudadOrigen;

                //this.ValidarOrigenDestinoManifiesto(consolidadoDetalle.IdLocalidadDespacho, Guia.IdCiudadDestino, Guia.NombreCiudadDestino, consolidadoDetalle.IdLocalidadManifiesta, consolidadoDetalle.IdRuta);

                //Realizar el cambio de estado de la guia
                trazaGuia = new ADTrazaGuia()
                {
                    IdAdmision = consolidadoDetalle.IdAdminisionMensajeria,
                    NumeroGuia = consolidadoDetalle.NumeroGuia,
                    Observaciones = string.Empty,
                    IdCiudad = consolidadoDetalle.IdLocalidadDespacho,
                    Ciudad = consolidadoDetalle.NombreLocalidadDespacho,
                    Modulo = COConstantesModulos.MODULO_OPERACION_NACIONAL,
                    IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(consolidadoDetalle.NumeroGuia.Value)
                };

                if (consolidadoDetalle.TipoRuta == (int)RUEnumTipoRutaDC.Nacional)
                {
                    trazaGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoNacional;
                    trazaGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);
                    if (trazaGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }

                }
                else if (consolidadoDetalle.TipoRuta == (int)RUEnumTipoRutaDC.Regional)
                {
                    trazaGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoRegional;
                    trazaGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);
                    if (trazaGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }

                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA.ToString(), "No se pudo identificar el tipo de ruta."));
                }

                long idConsolidadoDetalle = ONRepositorio.Instancia.AdicionarConsolidadoDetalle(consolidadoDetalle);
                consolidadoDetalle.IdManifiestoConsolidadoDetalle = idConsolidadoDetalle;

                transaccion.Complete();

                return consolidadoDetalle;
            }
        }

        /// <summary>
        /// Inserta una guia suelta a un manifiesto
        /// </summary>
        /// <param name="guia"></param>
        public ONManifiestoGuia AdicionarGuiaSuelta(ONManifiestoGuia guia)
        {

            using (TransactionScope transaccion = new TransactionScope())
            {
                ADTrazaGuia trazaGuia = new ADTrazaGuia();
                CambiarLocalidadManifestada = false;
                ADGuia Guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerGuiaXNumeroGuia(guia.NumeroGuia);

                PUCentroServiciosDC centroServ = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerAgencia(guia.IdCentroServicioOrigen);

                ADTrazaGuia traza = EstadosGuia.ObtenerTrazaUltimoEstadoXNumGuia(guia.NumeroGuia);


                //Obsolete
                //Si el centro de servicios es una agencia se debe dar ingreso automatico al centro de acopio
                //if (centroServ.Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA && centroServ.TipoSubtipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_COL && traza.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio)
                //{

                //    OUPlanillaVentaGuiasDC guiaIngresada = new OUPlanillaVentaGuiasDC()
                //    {

                //        IdCiudadOrigenGuia = guia.IdLocalidadDespacho,
                //        NumeroGuia = guia.NumeroGuia,
                //        PiezaActualRotulo = guia.PiezaActual,
                //        TotalPiezasRotulo = guia.TotalPiezas,
                //        IdPuntoServicio = guia.IdCentroServicioOrigen

                //    };

                //    guiaIngresada = COFabricaDominio.Instancia.CrearInstancia<IOUFachadaOperacionUrbana>().IngresarGuiaSueltaCentroAcopio(guiaIngresada, null);
                //}
                //else 
                if (traza.IdEstadoGuia != (short)ADEnumEstadoGuia.CentroAcopio && traza.IdCiudad != guia.IdLocalidadDespacho)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_NO_SE_ENCUENTRA_CENTRO_ACOPIO)));



                //Verificar que la guia no haya sido agregada como una guia suelta
                if (ONRepositorio.Instancia.VerificarExisteGuiaSueltaManifiesto(guia.NumeroGuia, guia.IdManifiestoOperacionNacional))
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_MANIFESTADA)));

                ///Verificar que la guía no haya sigo agregada a un consolidado
                if (ONRepositorio.Instancia.VerificarExisteGuiaConsolidadoManifiesto(guia.NumeroGuia, guia.IdManifiestoOperacionNacional))
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA)));

                guia.IdAdmisionMensajeria = Guia.IdAdmision;
                guia.PesoEnIngreso = Guia.Peso;
                guia.EstaDescargada = false;

                guia.IdTipoEnvio = Guia.IdTipoEnvio;
                guia.NombreTipoEnvio = Guia.NombreTipoEnvio;
                guia.IdCentroServicioOrigen = Guia.IdCentroServicioOrigen;
                guia.NombreCentroServicioOrigen = Guia.NombreCentroServicioOrigen;
                guia.IdCentroServicioDestino = Guia.IdCentroServicioDestino;
                guia.NombreCentroServicioDestino = Guia.NombreCentroServicioDestino;
                guia.IdCiudadDestino = Guia.IdCiudadDestino;
                guia.NombreCiudadDestino = Guia.NombreCiudadDestino;
                guia.IdCiudadOrigen = Guia.IdCiudadOrigen;
                guia.NombreCiudadOrigen = Guia.NombreCiudadOrigen;

                //Se valida si la guia puede llegar al destino desde la ciudad a donde la manifiestan
                //this.ValidarOrigenDestinoManifiesto(guia.IdLocalidadDespacho, guia.IdCiudadDestino, guia.NombreCiudadDestino, guia.IdLocalidadManifestada, guia.IdRuta);

                //Realizar el cambio de estado de la guía
                trazaGuia = new ADTrazaGuia()
                {
                    IdAdmision = guia.IdAdmisionMensajeria,
                    NumeroGuia = guia.NumeroGuia,
                    Observaciones = string.Empty,
                    IdCiudad = guia.IdLocalidadDespacho,
                    Ciudad = guia.NombreLocalidadDespacho,
                    Modulo = COConstantesModulos.MODULO_OPERACION_NACIONAL,
                    IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(guia.NumeroGuia)
                };

                if (CambiarLocalidadManifestada)
                {
                    guia.IdLocalidadManifestada = idCiudadDestinoRuta;
                }

                if (guia.TipoRuta == (int)RUEnumTipoRutaDC.Nacional)
                {
                    trazaGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoNacional;
                    trazaGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);
                    if (trazaGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }


                }
                else if (guia.TipoRuta == (int)RUEnumTipoRutaDC.Regional)
                {
                    trazaGuia.IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.TransitoRegional;
                    trazaGuia.IdTrazaGuia = EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);
                    if (trazaGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_GUIA_YA_CONSOLIDADA.ToString(), "No se pudo identificar el tipo de ruta."));
                }
                guia.IdManifiestoGuia = ONRepositorio.Instancia.AdicionarGuiaSuelta(guia);

                transaccion.Complete();
                return guia;
            }
        }

        /// <summary>
        /// Obtiene los consolidados y las guias sueltas para la impresion del manifiesto por ruta
        /// </summary>
        /// <param name="idPlanilla"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public List<ONImpresionManifiestoDC> ObtenerImpresionManifiestoRuta(long idManifiesto, List<string> ciudadesAManifestar)
        {
            List<ONImpresionManifiestoDC> impresion = new List<ONImpresionManifiestoDC>();



            ciudadesAManifestar.ForEach(c =>
                {
                    ONImpresionManifiestoDC imp = new ONImpresionManifiestoDC()
                    {
                        GuiasRotulosSueltos = new List<string>(),
                        Consolidados = new List<ONConsolidado>()
                    };

                    imp.IdLocalidadManifestada = c;

                    ONRepositorio.Instancia.ObtenerEnviosSueltosXManifiestoCiudadManifestada(idManifiesto, c).ForEach(e =>
                        {
                            if (e.TotalPiezasRotulo <= 0)
                                imp.GuiasRotulosSueltos.Add(e.NumeroGuia.ToString());
                            else
                                imp.GuiasRotulosSueltos.Add(e.NumeroGuia + "-" + e.PiezaActualRotulo + "/" + e.TotalPiezasRotulo);
                        });
                    imp.Consolidados = ONRepositorio.Instancia.ObtenerEnviosConsolidadosXManifiestoCiudaManifestada(idManifiesto, c);

                    imp.Consolidados.ForEach(con =>
                        {
                            con.TotalEnviosConsolidado = ONRepositorio.Instancia.ObtenerCantidadEnviosConsolidados(con.IdManfiestoConsolidado);
                        });

                    imp.TotalConsolidados = ONRepositorio.Instancia.ObtenerCantidadEnviosConsolidadosXManifiestoCiudaManifestada(idManifiesto, c);
                    imp.TotalEnvios = ONRepositorio.Instancia.ObtenerCantidadEnviosSueltosXManifiestoCiudadManifestada(idManifiesto, c);
                    imp.IdManifiesto = idManifiesto;
                    impresion.Add(imp);
                });

            return impresion;
        }


        #region Ingreso Salida Transportador

        /// <summary>
        /// Obtiene el ingreso o salida del transportador por los parametros de entrada
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="idRuta"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <param name="identificadorConductor"></param>
        /// <param name="nombreConductor"></param>
        /// <param name="incluyeFecha"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        internal IEnumerable<ONIngresoSalidaTransportadorDC> ObtenerIngresoSalidaTransportador(ONFiltroTransportadorDC filtro, int indicePagina, int registrosPorPagina, bool incluyeFecha)
        {
            return ONRepositorio.Instancia.ObtenerIngresoSalidaTransportador(filtro, indicePagina, registrosPorPagina, incluyeFecha);
        }

        /// <summary>
        /// Obtiene la informacion de ingreso y salida del transportador por ID
        /// </summary>
        /// <param name="idIngrsoSalidaTransportado"></param>
        /// <returns></returns>
        internal ONIngresoSalidaTransportadorDC ObtenerIngresoSalidaTransportadorPorId(long idIngrsoSalidaTransportado)
        {
            return ONRepositorio.Instancia.ObtenerIngresoSalidaTransportadorPorId(idIngrsoSalidaTransportado);
        }

        /// <summary>
        /// Obtiene todas las novedades operativas
        /// </summary>
        internal IList<ONNovedadOperativoDC> ObtenerNovedadOperativo()
        {
            return ONRepositorio.Instancia.ObtenerNovedadOperativo();
        }



        /// <summary>
        /// Consultar la última ruta de un vehículo que ha sido manifestado,
        /// el vehículo se consulta por su placa
        /// si no existe consulta el conductor asociado a la placa
        /// </summary>
        internal ONRutaConductorDC ObtenerUltimaRutaConduPlaca(string placa)
        {
            ONRutaConductorDC rutaConductor = ONRepositorio.Instancia.ObtenerUltimaRutaConduPlaca(placa);
            if (rutaConductor == null)
            {
                rutaConductor = COFabricaDominio.Instancia.CrearInstancia<IPOFachadaParametrosOperacion>().ObtenerConductoresPorVehiculo(placa);
            }

            return rutaConductor;
        }

        /// <summary>
        /// Obtener los consolidados a partir de la guia interna
        /// </summary>
        /// <param name="idGuiaInterna"></param>
        internal ONConsolidado ObtenerConsolidadoPorIdGuia(long idGuiaInterna)
        {
            return ONRepositorio.Instancia.ObtenerConsolidadoPorIdGuia(idGuiaInterna);
        }


        #endregion Ingreso Salida Transportador

        #region Ingreso Operativo Ciudad

        /// <summary>
        /// Guarda el envio ingresado
        /// </summary>
        /// <param name="ingreso"></param>
        public ONIngresoOperativoCiudadDC GuardaIngresoEnvioOperativoCiudad(ONIngresoOperativoCiudadDC ingreso)
        {
            bool estaDescargada = true;

            ///Si el envio no tiene id de mensajeria consulta el id
            if (ingreso.EnvioIngreso.IdAdmisionMensajeria == 0)
            {
                ///Valida que la guia ingresada este capturada en sistema
                ADGuia guia = COFabricaDominio.Instancia.CrearInstancia<IADFachadaAdmisionesMensajeria>().ObtenerInfoGuiaXNumeroGuia(ingreso.EnvioIngreso.NumeroGuia.Value);
                if (guia.IdAdmision == 0)
                    ingreso.EnvioIngreso.IdAdmisionMensajeria = guia.IdAdmision;
                else
                {
                    ingreso.EnvioIngreso.IdAdmisionMensajeria = guia.IdAdmision;
                    ingreso.EnvioIngreso.IdLocalidadDestino = guia.IdCiudadDestino;
                    ingreso.EnvioIngreso.NombreCiudadDestino = guia.NombreCiudadDestino;
                    ingreso.EnvioIngreso.IdCiudadOrigen = guia.IdCiudadOrigen;
                    ingreso.EnvioIngreso.NombreCiudadOrigen = guia.NombreCiudadOrigen;
                }
            }

            ///Si el estado del empaque del envio no tiene bolsa de seguridad envia una falla
            if (ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque == OPConstantesOperacionNacional.ID_ESTADO_EMPAQUE_SIN_BOLSA_SEGURIDAD
              || ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque == OPConstantesOperacionNacional.ID_ESTADO_EMPAQUE_MAL_EMBALADO)
            {
                if (ingreso.EnvioIngreso.IdAdmisionMensajeria == 0)
                {
                    ONManejadorFallas.DespacharFallaPorBolsaDeSeguridad(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
                }
                else
                {
                    ONManejadorFallas.DespacharFallaPorBolsaDeSeguridadOrigen(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
                }

                ONManejadorFallas.DespacharFallaPorBolsaDeSeguridad(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
            }

            ///Si el envio tiene valor de peso de ingreso valida la diferencia teniendo encuenta el desfase del peso
            if (ingreso.EnvioIngreso.PesoGuiaSistema > 0)
            {
                string valor = ONRepositorio.Instancia.ObtenerParametroOperacionNacional(OPConstantesOperacionNacional.ID_PARAMETRO_DESFASE_PESO);
                decimal valorDesfase = Convert.ToDecimal(valor);
                if (ingreso.EnvioIngreso.PesoGuiaIngreso < (ingreso.EnvioIngreso.PesoGuiaSistema - valorDesfase) ||
                  ingreso.EnvioIngreso.PesoGuiaIngreso < (ingreso.EnvioIngreso.PesoGuiaSistema + valorDesfase))
                {
                    ONManejadorFallas.DespacharFallaPorDiferenciaPeso(ingreso.EnvioIngreso, ControllerContext.Current.Usuario);
                }
            }

            ///Valida que la ciudad destino del envio no tenga ruta

            using (TransactionScope scope = new TransactionScope())
            {
                ///La guia no esta capturada en sistema
                if (ingreso.EnvioIngreso.IdAdmisionMensajeria == 0)
                {
                    ///Si ya se ingreso el envio para el id del operativo muestra una excepcion, de lo contrario ingresa el envio
                    if (!ONRepositorio.Instancia.ObtenerIngresoAgenciaEnvioNoRegistrado(ingreso.IdIngresoOperativo, ingreso.EnvioIngreso.NumeroGuia.Value))
                    {
                        ONRepositorio.Instancia.GuardarIngresoEnvioNoRegistrado(ingreso);
                    }
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO)));
                }

                /// La guia esta capturada en sistema
                else
                {
                    ///Valida que la ciudad destino del envio no tenga una ruta configurada
                    ONRepositorio.Instancia.ValidaCiudadDestinoEnvio(ingreso.EnvioIngreso.IdAdmisionMensajeria);

                    ///consulta el envio, si el envio ya fue ingresado genera una excepcion, sino ingresa el envio
                    if (!ONRepositorio.Instancia.ObtenerIngresoAgenciaEnvio(ingreso.IdIngresoOperativo, ingreso.EnvioIngreso.IdAdmisionMensajeria))
                        ONRepositorio.Instancia.GuardarIngresoEnvioRegistrado(ingreso);
                    else
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ENVIO_YA_INGRESO)));


                    ADTrazaGuia trazaGuia = new ADTrazaGuia()
                    {
                        IdAdmision = ingreso.EnvioIngreso.IdAdmisionMensajeria,
                        NumeroGuia = ingreso.EnvioIngreso.NumeroGuia,
                        Observaciones = string.Empty,
                        IdCiudad = ingreso.CiudadIngreso.IdLocalidad,
                        Ciudad = ingreso.CiudadIngreso.Nombre,
                        Modulo = COConstantesModulos.MODULO_OPERACION_NACIONAL,
                        IdEstadoGuia = (short)EstadosGuia.ObtenerUltimoEstadoxNumero(ingreso.EnvioIngreso.NumeroGuia.Value),
                        IdNuevoEstadoGuia = (short)ADEnumEstadoGuia.CentroAcopio
                    };

                    EstadosGuia.ValidarInsertarEstadoGuia(trazaGuia);

                    if (trazaGuia.IdTrazaGuia == 0)
                    {
                        string descripcionEstado = ", Estado actual " + ((ADEnumEstadoGuia)(trazaGuia.IdEstadoGuia)).ToString();
                        ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL
                                        , EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO.ToString()
                                        , MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_CAMBIO_ESTADO_NO_VALIDO));
                        excepcion.Mensaje = excepcion.Mensaje + descripcionEstado;
                        throw new FaultException<ControllerException>(excepcion);
                    }

                    if (ingreso.CiudadIngreso.IdLocalidad == ingreso.EnvioIngreso.IdLocalidadDestino)
                    {
                        estaDescargada = true;
                        ONRepositorio.Instancia.ActualizarDescargueEnvioConsolidado(ingreso.Consolidado.IdManfiestoConsolidado,
                          ingreso.EnvioIngreso.IdAdmisionMensajeria, estaDescargada);
                    }
                }
                scope.Complete();
            }

            return ingreso;
        }

        /// <summary>
        /// Guarda  el ingreso al operativo por ciudad
        /// </summary>
        /// <param name="ingreso"></param>
        /// <returns></returns>
        public long GuardarOperativoAgenciaCiudad(ONIngresoOperativoCiudadDC ingreso)
        {
            long idOperativo = 0;
            if (ingreso.IdIngresoOperativo == 0)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ///Consulta el ultimo ingreso del vehiculo a la agencia, si el ingreso esta abierto
                    ///retorna el id del ingreso, sino realiza el ingreso
                    idOperativo = ONRepositorio.Instancia.ObtenerUltimoIngresoOperativoAgenciaCiudad(ingreso.Vehiculo.IdVehiculo, ingreso.IdAgencia);

                    if (idOperativo == 0)
                        idOperativo = ONRepositorio.Instancia.GuardarIngresoOperativoAgencia(ingreso, OPConstantesOperacionNacional.ID_TIPO_OPERATIVO_CIUDAD);

                    scope.Complete();
                }
            }

            return idOperativo;
        }

        public void TerminarDescargueOperativo(long idIngresoOperativo)
        {
            bool estaCerrado = true;

            ///Cierra el ingreso del operativo
            ONRepositorio.Instancia.ActualizarCierreOperativoAgencia(idIngresoOperativo, estaCerrado);
        }

        /// <summary>
        /// Almaacena el consumo de un precinto de seguridad
        /// </summary>
        /// <param name="numeroPrecinto"></param>
        /// <param name="tipoPropietario"></param>
        /// <param name="idPropietario"></param>
        private void GuardarConsumoPrecintoSeguridad(long numeroPrecinto, SUEnumGrupoSuministroDC tipoPropietario, long idPropietario)
        {
            ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

            SUConsumoSuministroDC consumoPrecinto = new SUConsumoSuministroDC()
            {
                Cantidad = 1,
                EstadoConsumo = SUEnumEstadoConsumo.CON,
                GrupoSuministro = tipoPropietario,
                IdDuenoSuministro = idPropietario,
                IdServicioAsociado = 0,
                NumeroSuministro = numeroPrecinto,
                Suministro = SUEnumSuministro.PRECINTO_SEGURIDAD
            };

            fachadaSuministros.GuardarConsumoSuministro(consumoPrecinto);
        }

        #endregion Ingreso Operativo Ciudad

        #region Descargue Consolidados

        /// <summary>
        /// Obtiene los Consolidados de Guias para el descarge Urbano - Regional - Nacional
        /// </summary>
        /// <param name="ingresoConsolidado">info a consultar</param>
        /// <returns>info de las guias descargadas</returns>
        public ONDescargueConsolidadosUrbRegNalDC ObtenerIngresoConsolidado(ONDescargueConsolidadosUrbRegNalDC ingresoConsolidado)
        {
            return ONRepositorio.Instancia.ObtenerIngresoConsolidado(ingresoConsolidado);
        }

        /// <summary>
        /// Obtiene las novedades delos ingresos de la Guia
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesEnvioDC> ObtenerNovedadesIngresosGuia()
        {
            return ONRepositorio.Instancia.ObtenerNovedadesIngresosGuia();
        }

        /// <summary>
        ///Obtiene las novedades de Descargue Consolidado
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueConsolidado()
        {
            return ONRepositorio.Instancia.ObtenerNovedadesDescargueConsolidado();
        }

        /// <summary>
        /// Obtiene la info inicial de la pantalla de Descargue Consolidados
        /// </summary>
        /// <returns>Listas de info Inicial pantalla</returns>
        public ONDescargueConsolidadosInfoInicialDC ObtenerInfoInicialDescargueConsolidados()
        {
            ONDescargueConsolidadosInfoInicialDC info = new ONDescargueConsolidadosInfoInicialDC();
            info.LstNovedadesConsolidados = ObtenerNovedadesDescargueConsolidado();
            info.LstNovedadesEnvioGuia = ObtenerNovedadesIngresosGuia();

            return info;
        }

        #endregion Descargue Consolidados

        public ONManifiestoGuia ConsultarUltimoManifiestoGuia(long idAdmision)
        {
            return ONRepositorio.Instancia.ConsultarUltimoManifiestoGuia(idAdmision);
        }

        /// <summary>
        /// Prueba de carga para el grafo de rutas
        /// </summary>
        /// <param name="IdCiudadDestino"></param>
        /// <param name="nombreCiudadDestino"></param>
        /// <param name="idCiudadManifiesta"></param>
        /// <param name="nombreciudadManifiesta"></param>
        /// <param name="idRuta"></param>
        /// <returns></returns>
        public Dictionary<string, string> CalcularRutaOptimaOmitiendoCiudadesPruebaCargaGrafo(string IdCiudadDestino, string nombreCiudadDestino, string idCiudadManifiesta, string nombreciudadManifiesta, int idRuta)
        {

            bool permiteEnRuta = true;
            Dictionary<string, string> resultados = new Dictionary<string, string>();

            resultados.Add("IdCiudadDestino", IdCiudadDestino.ToString());
            resultados.Add("IdCiudadManifiesta", idCiudadManifiesta.ToString());
            resultados.Add("NombreCiudadDestino", nombreCiudadDestino.ToString());
            resultados.Add("NombreciudadManifiesta", nombreciudadManifiesta.ToString());

            DateTime horainicio = DateTime.Now;


            int ciudadEnRuta = ONRepositorio.Instancia.VerificarCiudadEnRuta(idCiudadManifiesta, IdCiudadDestino, idRuta);

            int tiempo = (DateTime.Now - horainicio).Milliseconds;
            resultados.Add("TiempoVerificandoCiudadEnRuta", tiempo.ToString());

            RURutaDC ruta = rutas.ObtenerRutaIdRuta(idRuta);
            List<string> ciudadesAOmitir = new List<string> { ruta.IdLocalidadOrigen, ruta.IdLocalidadDestino };

            horainicio = DateTime.Now;
            RURutaOptimaCalculada rutaOptima = rutas.CalcularRutaOptimaOmitiendoCiudades(idCiudadManifiesta, IdCiudadDestino, ciudadesAOmitir);
            tiempo = (DateTime.Now - horainicio).Milliseconds;
            resultados.Add("TiempoCalculoRutaOptima", tiempo.ToString());

            horainicio = DateTime.Now;
            if (rutaOptima != null)
            {

                //si dentro de las rutas previstas el sistema indica que debe volver a pasar por si misma
                if (rutaOptima.CaminoMasCorto.CostosAristas.Where(c => c.Value.Arista.RutaArista.IdRuta == idRuta).Count() > 0)
                {
                    permiteEnRuta = false;
                }
                //Si  dentro de las aristas previstas el sistema indica que debe volver a pasar por el origen  de la ruta  no hay conexion con el destino final
                if (rutaOptima.CaminoMasCorto.CostosAristas.Where(c => c.Value.Arista.VerticeDestino.IdVertice == ruta.IdLocalidadOrigen || c.Value.Arista.VerticeOrigen.IdVertice == ruta.IdLocalidadOrigen).Count() > 0)
                {
                    permiteEnRuta = false;
                }
                //Si  dentro de las aristas previstas el sistema indica que debe volver a pasar por el destino  de la ruta y el destino es diferente a la ciudad manifestada  no hay conexion con el destino final
                if (rutaOptima.CaminoMasCorto.CostosAristas.Where(c => c.Value.Arista.VerticeDestino.IdVertice == ruta.IdLocalidadDestino || c.Value.Arista.VerticeOrigen.IdVertice == ruta.IdLocalidadDestino).Count() > 0
                    && ruta.IdLocalidadDestino != idCiudadManifiesta)
                {
                    permiteEnRuta = false;
                }

                if (rutaOptima.CaminoMasCorto.CostosAristas[0] != null)
                {
                    RURutaDC rutaMasCorta = rutas.ObtenerRutaIdRuta(rutaOptima.CaminoMasCorto.CostosAristas[0].Arista.RutaArista.IdRuta);

                    if (ruta.CiudadDestino.IdLocalidad == rutaMasCorta.CiudadOrigen.IdLocalidad && ruta.CiudadOrigen.IdLocalidad == rutaMasCorta.CiudadDestino.IdLocalidad)
                    {
                        permiteEnRuta = false;
                    }
                }

                int countRutas = rutaOptima.CaminoMasCorto.CostosAristas
                    .GroupBy(c1 => c1.Value.Arista.RutaArista.IdRuta)
                    .Select(r => r.First()).Count();

                int cantMaxima = Convert.ToInt32(ONRepositorio.Instancia.ObtenerParametroOperacionNacional("CantiMaxRutasGrafo"));

                if (countRutas > cantMaxima)
                {
                    permiteEnRuta = false;
                }

            }
            else
            {
                permiteEnRuta = false;
            }

            tiempo = (DateTime.Now - horainicio).Milliseconds;
            resultados.Add("TiempoValidacionesRutaOptima", tiempo.ToString());

            resultados.Add("PermiteEnRuta", permiteEnRuta.ToString());
            resultados.Add("Ruta", idRuta + "-" + ruta.NombreRuta);

            return resultados;
        }



        private void ValidarOrigenDestinoManifiesto(string IdCiudadOrigen, string IdCiudadDestino, string nombreCiudadDestino, string idCiudadManifiesta, int idRuta, bool esTula = false)
        {
            if (IdCiudadOrigen == IdCiudadDestino)
            {
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, "La guia no puede ser manifestada porque el destino es la misma ciudad origen", "La guia no puede ser manifestada porque el destino es la misma ciudad origen"));
            }

            int ciudadEnRuta = ONRepositorio.Instancia.VerificarCiudadEnRuta(idCiudadManifiesta, IdCiudadDestino, idRuta);

            switch (ciudadEnRuta)
            {
                case 0:
                    {
                        if (!esTula)
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR)));
                        else
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                    }
                //significa que hay validar otros caminos para llegar al destino
                case 2:



                    //busco la ruta optima desde la agencia que se esta manifestando hasta el destino de la guia
                    RURutaDC ruta = rutas.ObtenerRutaIdRuta(idRuta);
                    List<string> ciudadesAOmitir = new List<string> { ruta.IdLocalidadOrigen, ruta.IdLocalidadDestino };
                    RURutaOptimaCalculada rutaOptima = rutas.CalcularRutaOptimaOmitiendoCiudades(idCiudadManifiesta, IdCiudadDestino, ciudadesAOmitir);




                    if (rutaOptima != null)
                    {

                        //si dentro de las rutas previstas el sistema indica que debe volver a pasar por si misma
                        if (rutaOptima.CaminoMasCorto.CostosAristas.Where(c => c.Value.Arista.RutaArista.IdRuta == idRuta).Count() > 0)
                        {
                            if (!esTula)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR)));
                            else
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                        }
                        //Si  dentro de las aristas previstas el sistema indica que debe volver a pasar por el origen  de la ruta  no hay conexion con el destino final
                        if (rutaOptima.CaminoMasCorto.CostosAristas.Where(c => c.Value.Arista.VerticeDestino.IdVertice == ruta.IdLocalidadOrigen || c.Value.Arista.VerticeOrigen.IdVertice == ruta.IdLocalidadOrigen).Count() > 0)
                        {
                            if (!esTula)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR)));
                            else
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                        }
                        //Si  dentro de las aristas previstas el sistema indica que debe volver a pasar por el destino  de la ruta y el destino es diferente a la ciudad manifestada  no hay conexion con el destino final
                        if (rutaOptima.CaminoMasCorto.CostosAristas.Where(c => c.Value.Arista.VerticeDestino.IdVertice == ruta.IdLocalidadDestino || c.Value.Arista.VerticeOrigen.IdVertice == ruta.IdLocalidadDestino).Count() > 0
                            && ruta.IdLocalidadDestino != idCiudadManifiesta)
                        {
                            if (!esTula)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR)));
                            else
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                        }

                        /*  if (rutaOptima.CaminoMasCorto.CostosAristas[0] != null)
                          {
                              RURutaDC rutaMasCorta = rutas.ObtenerRutaIdRuta(rutaOptima.CaminoMasCorto.CostosAristas[0].Arista.RutaArista.IdRuta);

                              if (ruta.CiudadDestino.IdLocalidad == rutaMasCorta.CiudadOrigen.IdLocalidad && ruta.CiudadOrigen.IdLocalidad == rutaMasCorta.CiudadDestino.IdLocalidad)
                              {
                                  if (!esTula)
                                      throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR)));
                                  else
                                      throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                              }
                          }*/

                        int countRutas = rutaOptima.CaminoMasCorto.CostosAristas
                            .GroupBy(c1 => c1.Value.Arista.RutaArista.IdRuta)
                            .Select(r => r.First()).Count();

                        int cantMaxima = Convert.ToInt32(ONRepositorio.Instancia.ObtenerParametroOperacionNacional("CantiMaxRutasGrafo"));

                        if (countRutas > cantMaxima)
                        {
                            if (!esTula)
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_NO_SE_PUEDE_LLEGAR_AL_DESTINO_DE_GUIA_DESDE_AGENCIA_A_MANIFESTAR) + " Num rutas" + countRutas.ToString()));
                            else
                                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_TULA_NO_PERTENECE_CIUDAD_O_NO_ACTIVA)));
                        }

                        GC.Collect();

                        break;
                    }
                    else
                    {
                        if (!esTula)
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, "La ciudad " + IdCiudadDestino + "-" + nombreCiudadDestino + " no se encuentra cargada en ninguna ruta.", "La ciudad " + IdCiudadDestino + "-" + nombreCiudadDestino + " no se encuentra cargada en ninguna ruta."));
                        else
                            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, "La ciudad " + IdCiudadDestino + "-" + nombreCiudadDestino + " dueña de la tula/contenedor, no se encuentra cargada en ninguna ruta.", "La ciudad " + IdCiudadDestino + "-" + nombreCiudadDestino + " dueña de la tula/contenedor, no se encuentra cargada en ninguna ruta."));

                    }

            }
        }

        /// <summary>
        /// Obtiene las novedades de transporte de determinada guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ONNovedadesTransporteDC> ObtenerNovedadesTransporteGuia(long numeroGuia)
        {
            return ONRepositorio.Instancia.ObtenerNovedadesTransporteGuia(numeroGuia);
        }

        // Se revisa si el Casillero es AEREO (Ciudades de la Costa en la Tabla TrayectoCasilleroServicio_OPN)
        public bool ValidarTrayectoServicioAereo(string idLocalidadOrigen, string idLocalidadDestino, int idServicio)
        {
            return ONRepositorio.Instancia.ValidarTrayectoServicioAereo(idLocalidadOrigen, idLocalidadDestino, idServicio);
        }
        #region TrayectoCasillero

        /// <summary>
        /// Método para obtener los rangos de peso de los casilleros
        /// </summary>
        /// <returns></returns>
        public IList<ONRangoPesoCasilleroDC> ObtenerRangosPesoCasillero()
        {
            return ONRepositorio.Instancia.ObtenerRangosPesoCasillero();
        }


        /// <summary>
        /// Método para obtener los trayectos de un origen 
        /// </summary>
        /// <returns></returns>
        public IList<ONTrayectoCasilleroPesoDC> ObtenerTrayectosCasilleroDestino(PALocalidadDC localidadOrigen)
        {
            return ONRepositorio.Instancia.ObtenerTrayectosCasilleroDestino(localidadOrigen);
        }


        /// <summary>
        /// Método para guardar los cambios de  los trayectos casillero
        /// </summary>
        /// <returns></returns>
        public IList<ONTrayectoCasilleroPesoDC> GuardarTrayectoCasillero(List<ONTrayectoCasilleroPesoDC> ListaTrayectosCasillero)
        {
            ListaTrayectosCasillero
          .ForEach(tr =>
          {
              switch (tr.EstadoRegistro)
              {
                  case EnumEstadoRegistro.ADICIONADO:
                      ONRepositorio.Instancia.AdicionarTrayectoCasillero(tr);
                      break;
                  case EnumEstadoRegistro.MODIFICADO:
                      ONRepositorio.Instancia.ModificarTrayectoCasillero(tr);
                      break;
                  case EnumEstadoRegistro.BORRADO:
                      ONRepositorio.Instancia.EliminarTrayectoCasillero(tr);
                      break;
              }
          });
            return ListaTrayectosCasillero;
        }


        #endregion
    }
}