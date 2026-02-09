using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Text;
using CO.Servidor.OperacionNacional.Comun;
using CO.Servidor.OperacionNacional.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using System.Data;

namespace CO.Servidor.OperacionNacional.Datos
{
    public partial class ONRepositorio
    {
        #region Ingreso Col ruta

        /// <summary>
        /// Obtiene el ultimo ingreso del vehiculo a la agencia
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <param name="idAgencia">id de la agencia</param>
        /// <returns>false si es salida, true si es ingreso</returns>
        public bool ObtenerTipoIngresoVehiculo(int idVehiculo, long idAgencia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (idVehiculo > 0)
                {
                    var esIngreso = contexto.paObtenerTipoIngresoTransp_OPN(idVehiculo, idAgencia).FirstOrDefault();

                    if (esIngreso == null)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_FALTA_INGRESO_VEHICULO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_FALTA_INGRESO_VEHICULO)));

                    return esIngreso.Value;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Obtiene el ultimo ingreso del vehiculo a la agencia si, el vehiculo no ha ingresado ninguna vez retorna null
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <param name="idAgencia">id de la agencia</param>
        /// <returns>false si es salida, true si es ingreso, null no ha sido ingresado nunca a esa agencia</returns>
        public bool? ObtenerTipoIngresoVehiculoAgencia(int idVehiculo, long idAgencia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                bool? esIngreso = contexto.paObtenerTipoIngresoTransp_OPN(idVehiculo, idAgencia).FirstOrDefault();

                return esIngreso;
            }
        }

        /// <summary>
        /// Valida si la ciudad del usuario esta en la ruta, (destino, origen) o en las estaciones de la ruta
        /// </summary>
        /// <param name="idLocalidad">id de la localidad</param>
        /// <returns>true si la ciudad esta en la ruta, false si la ciudad no esta en la ruta</returns>
        public bool ValidarCiudadEnRutaEstacionesRuta(string idLocalidad, int idVehiculo, long idAgencia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var localidad = contexto.paObtenerDesOriRutaEstaRut_OPN(idLocalidad, idVehiculo, idAgencia);
                if (localidad == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Obtiene los envios consolidados de los manifiestos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        public List<ONManifiestoOperacionNacional> ObtenerEnviosConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, int indicePagina, int registrosPorPagina)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviConsoManiVehi_OPN(idVehiculo, idRuta, indicePagina, registrosPorPagina)
                  .ToList()
                  .ConvertAll(r => new ONManifiestoOperacionNacional()
                  {
                      IdManifiestoOperacionNacional = r.MOG_IdManifiestoOperacionNacio,
                      ConsolidadoManifiesto = new ONConsolidado()
                      {
                          NumeroGuiaInterna = r.GuiaInterna,
                          IdManfiestoConsolidado = r.idConsolidadoManifiesto,
                          DetalleConsolidado = new ONConsolidadoDetalle()
                          {
                              NumeroGuia = r.MOG_NumeroGuia,
                              PesoEnIngreso = r.MOG_PesoEnIngreso,
                              IdTipoEnvio = r.MOG_IdTipoEnvio,
                              NombreTipoEnvio = r.MOG_NombreTipoEnvio,
                              IdCiudadDestino = r.MOG_IdCiudadDestino,
                              NombreCiudadDestino = r.MOG_NombreCiudadDestino
                          }
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene los consolidados de los manifiestos abiertos asociados al vehiculo
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerConsolidadosManifiestoVehiculo(int idVehiculo, int idRuta, bool estaDescargado, int indicePagina, int registrosPorPagina)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerConsoManifAbiVehi_OPN(idVehiculo, idRuta, estaDescargado, indicePagina, registrosPorPagina)
                  .ToList()
                  .ConvertAll(r => new ONConsolidado()
                  {
                      IdTipoConsolidado = r.MOC_TipoConsolidadoDetalle,
                      NombreTipoConsolidado = r.TIC_Descripcion,
                      IdManfiestoConsolidado = r.MOC_IdManfiestoOperaNacioConso,
                      IdManifiestoOperacionNacional = r.MON_IdManifiestoOperacionNacio,
                      NumeroContenedorTula = r.MOC_NumeroContenedorTula,
                      NumeroGuiaInterna = r.MOC_NumeroGuiaInterna,
                      NumeroPrecintoRetorno = r.MOC_NumeroPrecintoRetorno,
                      DescripcionConsolidadoDetalle = r.MOC_DescpConsolidadoDetalle
                  });
            }
        }

        /// <summary>
        /// Obtiene el id del ingreso del operativo del ultimo ingreso del vehiculo a la agencia
        /// </summary>
        public long ObtenerUltimoIngresoOperativoAgenciaRuta(int idVehiculo, long idAgencia, int idRuta)
        {
            long idIngresoOperativo = 0;
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ingresoVehiculo = contexto.paObtenerIngAgenVehiTipOpe_OPN(idVehiculo, idAgencia, OPConstantesOperacionNacional.ID_TIPO_OPERATIVO_RUTA, null).FirstOrDefault();
                if (ingresoVehiculo != null)
                {
                    if (!ingresoVehiculo.IOA_EstaCerradoIngreso && ingresoVehiculo.IOA_IdRuta == idRuta)
                        idIngresoOperativo = ingresoVehiculo.IOA_IdIngresoOperativoAgencia;
                }

                return idIngresoOperativo;
            }
        }

        /// <summary>
        /// Obtiene el id del ingreso del operativo del ultimo ingreso del vehiculo a la agencia
        /// </summary>
        public long ObtenerUltimoIngresoOperativoAgenciaCiudad(int idVehiculo, long idAgencia)
        {
            long idIngresoOperativo = 0;
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ingresoVehiculo = contexto.paObtenerIngAgenVehiTipOpe_OPN(idVehiculo, idAgencia, OPConstantesOperacionNacional.ID_TIPO_OPERATIVO_CIUDAD, null).FirstOrDefault();
                if (ingresoVehiculo != null)
                {
                    if (!ingresoVehiculo.IOA_EstaCerradoIngreso)
                        idIngresoOperativo = ingresoVehiculo.IOA_IdIngresoOperativoAgencia;
                }

                return idIngresoOperativo;
            }
        }

        /// <summary>
        /// Obtiene el id del ingreso del operativo del ultimo ingreso del vehiculo a la agencia
        /// </summary>
        public ONIngresoOperativoDC ObtenerUltimoIngresoOperativoAgenciaManifiesto(int idVehiculo, long idAgencia, long idManifiesto)
        {
            ONIngresoOperativoDC ingresoOperativo = null;
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ingresoVehiculo = contexto.paObtenerIngAgenVehiTipOpe_OPN(idVehiculo, idAgencia, OPConstantesOperacionNacional.ID_TIPO_OPERATIVO_MANIFIESTO, idManifiesto).FirstOrDefault();
                if (ingresoVehiculo != null)
                {
                    ingresoOperativo = new ONIngresoOperativoDC()
                    {
                        IdIngresoOperativo = ingresoVehiculo.IOA_IdIngresoOperativoAgencia,
                        IngresoCerrado = ingresoVehiculo.IOA_EstaCerradoIngreso,
                        ManifiestoIngreso = new ONManifiestoOperacionNacional()
                        {
                            IdManifiestoOperacionNacional = ingresoVehiculo.IOA_IdManifiestoOperacionNacio == null ? 0 : ingresoVehiculo.IOA_IdManifiestoOperacionNacio.Value
                        }
                    };

                    //Codigo original de Johana:  si estaba cerrado retorna un cero el cual indica que se debe crear un nuevo ingreso
                    //if (!ingresoVehiculo.IOA_EstaCerradoIngreso)
                    //  idIngresoOperativo = ingresoVehiculo.IOA_IdIngresoOperativoAgencia;
                }
            }
            return ingresoOperativo;
        }

        /// <summary>
        /// Retorna los envios del consolidado seleccionado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerEnviosConsolidado(long idConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviosConsolidado_OPN(idConsolidado)
                .ToList()
                .ConvertAll(r => new ONConsolidadoDetalle()
                  {
                      NumeroGuia = r.MOD_NumeroGuia,
                      IdAdminisionMensajeria = r.MOD_IdAdminisionMensajeria,
                      IdManfiestoConsolidado = r.MOD_IdManfiestoOperaNacioConso,
                      IdCiudadDestino = r.MOD_IdCiudadDestino,
                      NombreCiudadDestino = r.MOD_NombreCiudadDestino,
                      IdCiudadOrigen = r.MOD_IdCiudadOrigen,
                      NombreCiudadOrigen = r.MOD_NombreCiudadOrigen,
                      PesoEnIngreso = r.MOD_PesoEnIngreso,
                      EstaDescargada = r.IOG_IdIngresoOperaAgenciaGuia == null ? false : true,
                      EstaDescargadaDetalle = r.IOG_IdIngresoOperaAgenciaGuia == null ? OPConstantesOperacionNacional.DETALLE_NO : OPConstantesOperacionNacional.DETALLE_SI
                  });
            }
        }

        /// <summary>
        /// validar si una guia se encuentra dentro de un consolidado
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <returns></returns>
        public bool ValidarEnvioDentroConsolidado(long idConsolidado, long numeroGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviosConsolidado_OPN(idConsolidado).ToList().Where(e => e.MOD_NumeroGuia == numeroGuia).Count() > 0 ? true : false;
            }
        }

        /// <summary>
        /// Obtiene los manifiestos sin descargar de un vehiculo
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosVehiculo(int idVehiculo)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerManifiestVehiculo_OPN(idVehiculo)
                  .ToList()
                  .ConvertAll(r => new ONManifiestoOperacionNacional()
                  {
                      IdManifiestoOperacionNacional = r.MON_IdManifiestoOperacionNacio,
                      LocalidadDespacho = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                      {
                          IdLocalidad = r.MON_IdLocalidadDespacho,
                          Nombre = r.MON_NombreLocalidadDespacho
                      }
                  });
            }
        }

        /// <summary>
        /// Obtiene el ingreso de un envio a la agencia
        /// </summary>
        /// <param name="idOperativo"></param>
        /// <param name="idAdmisionMensajeria"></param>
        public bool ObtenerIngresoAgenciaEnvio(long idOperativo, long idAdmisionMensajeria)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ingresoEnvio = contexto.paObtenerIngresoEnvioAgenc_OPN((int)idOperativo, idAdmisionMensajeria).FirstOrDefault();

                if (ingresoEnvio == null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Consulta el ingreso de un envio para un id de operativo especifico
        /// </summary>
        /// <param name="idOperativo"></param>
        /// <param name="numeroGuia"></param>
        /// <returns> false si no existe ingreso, true si el envio ya fue ingresado para el id de operativo seleccionado</returns>
        public bool ObtenerIngresoAgenciaEnvioNoRegistrado(long idOperativo, long numeroGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var ingresoEnvio = contexto.paObtenerIngrEnvNoRegAgenc_OPN((int)idOperativo, numeroGuia).FirstOrDefault();

                if (ingresoEnvio == null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Obtiene los envios sueltos del manifiesto
        /// </summary>
        /// <param name="idVehiculo">id del vehiculo</param>
        /// <param name="idIngresoOperativo">id del ingreso a la agencia del vehiculo</param>
        /// <param name="estaDescargada">bit para saber si esta descargado o no el manifiesto</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize">tamaño de la pagina</param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerEnviosSueltosManifiestoVehiculo(int idVehiculo, int idRuta, long idIngresoOperativo, bool estaDescargada, int pageIndex, int pageSize)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvSuelManifiVehi_OPN(idVehiculo, idRuta, idIngresoOperativo, estaDescargada, pageIndex, pageSize)
                  .ToList()
                  .ConvertAll(r => new ONConsolidadoDetalle()
                  {
                      NumeroGuia = r.MOG_NumeroGuia,
                      IdAdminisionMensajeria = r.MOG_IdAdminisionMensajeria,
                      IdCiudadDestino = r.MOG_IdCiudadDestino,
                      NombreCiudadDestino = r.MOG_NombreCiudadDestino,
                      PesoEnIngreso = r.MOG_PesoEnIngreso,
                      EstaDescargada = r.IOG_IdIngresoOperaAgenciaGuia == null ? false : true,
                      EstaDescargadaDetalle = r.IOG_IdIngresoOperaAgenciaGuia == null ? OPConstantesOperacionNacional.DETALLE_NO : OPConstantesOperacionNacional.DETALLE_SI
                  });
            }
        }

        /// <summary>
        /// Obtiene el total de los envios manifestados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosManifestadosVehiculoLocalidad(int idVehiculo, string idLocalidadDestino)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerTotalEnviManiVehi_OPN(idVehiculo, idLocalidadDestino).FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// Consulta todos los envios de un consolidado
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idIngresoOperativo"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerTodosEnviosSueltosVehiculo(int idVehiculo, int idRuta)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviosSueltManifi_OPN(idVehiculo, idRuta).ToList().ConvertAll(r => new ONConsolidadoDetalle()
                  {
                      NumeroGuia = r.MOG_NumeroGuia,
                      IdAdminisionMensajeria = r.MOG_IdAdminisionMensajeria,
                      PesoEnIngreso = r.MOG_PesoEnIngreso,
                      IdCiudadDestino = r.MOG_IdCiudadDestino,
                      NombreCiudadDestino = r.MOG_NombreCiudadDestino,
                      IdCiudadOrigen = r.MOG_IdCiudadOrigen,
                      NombreCiudadOrigen = r.MOG_NombreCiudadOrigen
                  });
            }
        }

        /// <summary>
        /// Consulta todos los envios consolidados del manifiesto
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <returns></returns>
        public List<ONConsolidadoDetalle> ObtenerTodosEnviosConsolidadoManifiesto(int idVehiculo, int idRuta)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvManfConsolidad_OPN(idVehiculo, idRuta).ToList()
                  .ConvertAll(r => new ONConsolidadoDetalle()
                  {
                      NumeroGuia = r.MOD_NumeroGuia,
                      IdAdminisionMensajeria = r.MOD_IdAdminisionMensajeria,
                      PesoEnIngreso = r.MOD_PesoEnIngreso,
                      IdCiudadDestino = r.MOD_IdCiudadDestino,
                      NombreCiudadDestino = r.MOD_NombreCiudadDestino,
                      IdCiudadOrigen = r.MOD_IdCiudadOrigen,
                      NombreCiudadOrigen = r.MOD_NombreCiudadOrigen,
                      IdManfiestoConsolidado = r.MOC_IdManfiestoOperaNacioConso
                  });
            }
        }

        /// <summary>
        /// Obtiene el total de los envios sobrantes
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosSobrantesVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerTotEnvManVehSobra_OPN(idOperativo, idLocalidadDestino).FirstOrDefault().Value;
            }
        }

        /// <summary>
        /// Obtiene el total de los envios descargados
        /// </summary>
        /// <param name="idVehiculo"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <returns></returns>
        public int ObtenerTotalEnviosDescargadosVehiculoLocalidad(long idOperativo, string idLocalidadDestino)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerTotEnvManVehDesca_OPN(idOperativo, idLocalidadDestino).FirstOrDefault().Value;
            }
        }

        public ONConsolidado ObtenerConsolidadoEnvioTransito(int idVehiculo, bool estaDescargado, long idGuiaInterna)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ONConsolidado consolidadoTransito = new ONConsolidado();
                var consolidado = contexto.paObtenerConManAbVehGuiInt_OPN(idVehiculo, estaDescargado, idGuiaInterna).FirstOrDefault();

                if (consolidado != null)
                {
                    consolidadoTransito.IdGuiaInterna = consolidado.MOC_IdGuiaInterna;
                    consolidadoTransito.NumeroGuiaInterna = consolidado.MOC_NumeroGuiaInterna;
                    consolidadoTransito.LocalidadManifestada = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                    {
                        IdLocalidad = consolidado.MOC_IdLocalidadManifestada
                    };
                }

                return consolidadoTransito;
            }
        }

        #region Insert

        /// <summary>
        /// Guarda el operativo en la agencia por ruta
        /// </summary>
        /// <param name="ingresoOperativo"></param>
        /// <returns></returns>
        public long GuardarIngresoOperativoAgencia(ONIngresoOperativoDC ingresoOperativo, short tipoOperativo)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                long idIngresoOperativo;
                var idIngreso = contexto.paCrearIngreOperaAgencRuta_OPN(tipoOperativo
                  , ingresoOperativo.Ruta.IdRuta
                  , ingresoOperativo.Ruta.NombreRuta
                  , ingresoOperativo.Vehiculo.IdVehiculo
                  , ingresoOperativo.Vehiculo.Placa
                  , ingresoOperativo.Conductores.IdConductor
                  , ingresoOperativo.Conductores.NombreCompleto
                  , ingresoOperativo.CiudadDescargue.IdLocalidad
                  , ingresoOperativo.CiudadDescargue.Nombre
                  , ingresoOperativo.IdAgencia
                  , false
                  , ControllerContext.Current.Usuario, ingresoOperativo.ManifiestoIngreso.IdManifiestoOperacionNacional).FirstOrDefault();

                if (long.TryParse(idIngreso.ToString(), out idIngresoOperativo))
                    return idIngresoOperativo;
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_INGRESO_OPERATIVO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_INGRESO_OPERATIVO))); ;
            }
        }

        /// <summary>
        /// Guarda la novedad del consolidado
        /// </summary>
        /// <param name="idConsolidado">id del consolidado </param>
        /// <param name="descripcion">Descripcion de la novedad</param>
        /// <param name="numeroPrecintoIngreso">numero del precinto de ingreso</param>
        /// <param name="numeroTulaContenedor">número de tula o contendor de ingreso</param>
        public void GuardarNovedadConsolidado(long idConsolidado, string descripcion, long? numeroPrecintoIngreso, string numeroTulaContenedor)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearNovedadConsolidado_OPN(idConsolidado
                                                        , descripcion
                                                        , numeroPrecintoIngreso
                                                        , numeroTulaContenedor
                                                        , ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Guarda el ingreso de un envio registrado en el sistema
        /// </summary>
        /// <param name="ingreso"></param>
        public void GuardarIngresoEnvioRegistrado(ONIngresoOperativoDC ingreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngreOperaAgenEnvio_OPN(ingreso.IdIngresoOperativo
                  , ingreso.EnvioIngreso.IdAdmisionMensajeria
                  , ingreso.EnvioIngreso.NumeroGuia
                  , ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque
                  , ingreso.EnvioIngreso.PesoGuiaIngreso
                  , ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Guarda el Ingreso de un envio no registrado en el sistema
        /// </summary>
        /// <param name="ingreso"></param>
        public void GuardarIngresoEnvioNoRegistrado(ONIngresoOperativoDC ingreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngOpeAgenEnviNoReg_OPN(ingreso.IdIngresoOperativo
                  , ingreso.EnvioIngreso.NumeroGuia
                  , ingreso.EnvioIngreso.EstadoEmpaque.IdEstadoEmpaque
                  , ingreso.EnvioIngreso.PesoGuiaIngreso
                  , ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Guarda el ingreso de todos los envios asociados a un consolidado, envios transito
        /// </summary>
        /// <param name="idOperativoAgencia"></param>
        /// <param name="idEstadoEmpaque"></param>
        /// <param name="numeroGuiaInterna"></param>
        public void GuardarIngresoEnviosTransito(long idOperativoAgencia, int idEstadoEmpaque, long numeroGuiaInterna)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngOperaAgenEnvTran_OPN(idOperativoAgencia
                  , idEstadoEmpaque
                  , ControllerContext.Current.Usuario
                  , DateTime.Now
                  , numeroGuiaInterna);
            }
        }

        /// <summary>
        /// Guarda las inconsistencias sobrantes del operativo
        /// </summary>
        /// <param name="idOperativo"></param>
        /// <param name="idLocalidad"></param>
        public void GuardarInconsistenciasSobrantesOperativoIngreso(long idOperativo, long idManifiestoIngreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearInconSobraOperatAge_OPN(idOperativo, ControllerContext.Current.Usuario, idManifiestoIngreso);
            }
        }

        /// <summary>
        /// Guarda las inconsistencias faltantes del operativo
        /// </summary>
        /// <param name="idOperativo"></param>
        /// <param name="idLocalidad"></param>
        public void GuardarInconsistenciasFaltantesOperativoIngreso(long idOperativo, long idManifiestoIngreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearInconsiFalOperatAge_OPN(idOperativo, ControllerContext.Current.Usuario, idManifiestoIngreso);
            }
        }

        #endregion Insert

        #region Actualizar

        /// <summary>
        /// Actualiza el descargue de un envio
        /// </summary>
        /// <param name="idConsolidado"></param>
        /// <param name="idAdmision"></param>
        /// <param name="estaDescargada"></param>
        public void ActualizarDescargueEnvioConsolidado(long idConsolidado, long idAdmision, bool estaDescargada)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarDescarEnvConso_OPN(idConsolidado, idAdmision, estaDescargada);
            }
        }

        /// <summary>
        /// Verifica el estado del ingreso del operativo por agencia
        /// </summary>
        /// <param name="idIngresoOperativoAgencia"></param>
        /// <returns></returns>
        public bool VerificarEstadoIngresoOperativoAgencia(long idIngresoOperativoAgencia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                bool? rst = contexto.paVerificarEstadoCierIngreOperat_OPN(idIngresoOperativoAgencia).FirstOrDefault();
                if (rst == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.PARAMETROS_OPERATIVOS, ETipoErrorFramework.EX_CONSULTA_DB_NULL.ToString(), MensajesFramework.CargarMensaje(ETipoErrorFramework.EX_CONSULTA_DB_NULL));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return rst.Value;
            }
        }

        /// <summary>
        /// Cierra el operativo de ingreso para la agencia
        /// </summary>
        /// <param name="idIngresoOperativoAgencia"></param>
        /// <param name="estaCerrado"></param>
        public void ActualizarCierreOperativoAgencia(long idIngresoOperativoAgencia, bool estaCerrado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizaCierIngreOperat_OPN(idIngresoOperativoAgencia, estaCerrado);
            }
        }

        /// <summary>
        /// Cierra el operativo de ingreso para la agencia
        /// </summary>
        /// <param name="idIngresoOperativoAgencia"></param>
        /// <param name="estaCerrado"></param>
        public void ActualizarDescargueManifiestoRuta(int idRuta, int idVehiculo)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizaDescarManiRuta_OPN(idRuta, idVehiculo);
            }
        }

        #endregion Actualizar

        #endregion Ingreso Col ruta

        #region Parametros

        /// <summary>
        /// Obtiene el valor para el parametro especificado
        /// </summary>
        /// <param name="parametroTipoEnvio"></param>
        public string ObtenerParametroOperacionNacional(string idParametro)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var parametro = contexto.ParametrosOperacionNacional_OPN.Where(r => r.PON_IdParametro == idParametro).FirstOrDefault();

                if (parametro == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_PARAMETRO_NO_EXISTE.ToString(), string.Format(MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_PARAMETRO_NO_EXISTE), idParametro)));
                }
                return parametro.PON_ValorParametro;
            }
        }

        #endregion Parametros

        #region Ingreso COL por Manifiesto

        /// <summary>
        /// Obtiene los manifietos donde la ciudad origen, destino o estacion de ruta sea la ciudad
        /// donde se esta haciendo el descargue
        /// </summary>
        /// <param name="idLocalidad"></param>
        /// <returns></returns>
        public List<ONManifiestoOperacionNacional> ObtenerManifiestosXLocalidad(IDictionary<string, string> filtro, string idLocalidad, int indicePagina, int registrosPorPagina)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string idManifiesto;
                string nombreLocalidadDestino;
                string nombreLocalidadOrigen;
                string ruta;

                filtro.TryGetValue("MON_IdManifiestoOperacionNacio", out idManifiesto);
                filtro.TryGetValue("RUT_Nombre", out ruta);
                filtro.TryGetValue("RUT_NombreLocalidadOrigen", out nombreLocalidadOrigen);
                filtro.TryGetValue("RUT_NombreLocalidadDestino", out nombreLocalidadDestino);

                return contexto.paObtenerManifiestosLocali_OPN(idLocalidad,
                          Convert.ToInt64(idManifiesto),
                          ruta,
                          nombreLocalidadOrigen,
                          nombreLocalidadDestino,
                          indicePagina,
                          registrosPorPagina
                          )
                  .ToList()
                  .ConvertAll(r => new ONManifiestoOperacionNacional()
                  {
                      IdManifiestoOperacionNacional = r.MON_IdManifiestoOperacionNacio,
                      LocalidadDespacho = new PALocalidadDC()
                      {
                          IdLocalidad = r.RUT_IdLocalidadOrigen,
                          Nombre = r.RUT_NombreLocalidadOrigen
                      },
                      LocalidadDestino = new PALocalidadDC()
                      {
                          IdLocalidad = r.RUT_IdLocalidadDestino,
                          Nombre = r.RUT_NombreLocalidadDestino
                      },
                      IdRutaDespacho = r.MON_IdRutaDespacho,
                      NombreRuta = r.RUT_Nombre,
                      IdCentroServiciosManifiesta = r.MON_IdCentroServiciosManifiesta,
                      NumeroManifiestoCarga = r.MON_NumeroManifiestoCarga,
                      FechaCreacion = r.MON_FechaGrabacion
                  });
            }
        }

        /// <summary>
        /// Consulta el detalle del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <returns></returns>
        public ONIngresoOperativoDC ObtenerDetalleManifiesto(ONIngresoOperativoDC ingreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var detalleManifiesto = contexto.paObtenerDetalleManifiesto_OPN(ingreso.ManifiestoIngreso.IdManifiestoOperacionNacional).FirstOrDefault();

                if (detalleManifiesto != null)
                {
                    ingreso.ManifiestoIngreso.IdEmpresaTransportadora = detalleManifiesto.MON_IdEmpresaTransportadora;
                    ingreso.ManifiestoIngreso.NombreEmpresaTransportadora = detalleManifiesto.ETR_Nombre;
                    ingreso.ManifiestoIngreso.IdTipoTransporte = detalleManifiesto.ETR_IdTipoTransporte;
                    ingreso.ManifiestoIngreso.NombreTipoTransporte = detalleManifiesto.TIT_Descripcion;
                    ingreso.Vehiculo = new POVehiculo()
                    {
                        IdVehiculo = detalleManifiesto.MOT_IdVehiculo == null ? 0 : detalleManifiesto.MOT_IdVehiculo.Value,
                        Placa = detalleManifiesto.MOT_Placa == null ? string.Empty : detalleManifiesto.MOT_Placa,
                    };
                    ingreso.Conductores = new POConductores()
                    {
                        IdConductor = detalleManifiesto.MOT_IdConductor == null ? 0 : detalleManifiesto.MOT_IdConductor.Value,
                        Identificacion = detalleManifiesto.MOT_IdentificacionConductor == null ? string.Empty : detalleManifiesto.MOT_IdentificacionConductor,
                        NombreCompleto = detalleManifiesto.MOT_NombreConductor == null ? string.Empty : detalleManifiesto.MOT_NombreConductor,
                    };
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_OPERACION_NACIONAL, EnumTipoErrorOperacionNacional.EX_ERROR_DETALLE_MANIFIESTO.ToString(), MensajesOperacionNacional.CargarMensaje(EnumTipoErrorOperacionNacional.EX_ERROR_DETALLE_MANIFIESTO)));

                return ingreso;
            }
        }

        /// <summary>
        /// Obtiene los envios de un manifiesto por numero de manifesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerEnviosXManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnviosManifiesto_OPN(idManifiesto, indicePagina, registrosPorPagina)
                  .ToList()
                  .ConvertAll(r => new ONConsolidado()
                  {
                      DetalleConsolidado = new ONConsolidadoDetalle()
                      {
                          IdAdminisionMensajeria = r.MOD_IdAdminisionMensajeria,
                          NumeroGuia = r.MOD_NumeroGuia,
                          IdTipoEnvio = r.MOD_IdTipoEnvio,
                          NombreTipoEnvio = r.MOD_NombreTipoEnvio,
                          PesoEnIngreso = r.MOD_PesoEnIngreso,
                          IdCiudadDestino = r.MOD_IdCiudadDestino,
                          NombreCiudadDestino = r.MOD_NombreCiudadDestino,
                          IdCiudadOrigen = r.MOD_IdCiudadOrigen,
                          NombreCentroServicioOrigen = r.MOD_NombreCiudadOrigen,
                      },
                      IdManfiestoConsolidado = r.Cosolidado,
                      IdManifiestoOperacionNacional = idManifiesto,
                      NumeroGuiaInterna = r.GuiaConsolidado
                  });
            }
        }

        /// <summary>
        /// Valida si un envio esta manifestado
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public bool ValidarEnvioDentroManifiesto(long idManifiesto, long numeroGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var l = contexto.paObtenerEnviosManifiesto_OPN(idManifiesto, 1, int.MaxValue - 1).ToList();

                return l.Where(e => e.MOD_NumeroGuia == numeroGuia).Count() > 0 ? true : false;
            }
        }

        /// <summary>
        /// Retorna o asigan los consolidados del manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerDetalleConsolidadosManifiesto(long idManifiesto, int indicePagina, int registrosPorPagina)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var a = contexto.paObtenerConsolidadosManif_OPN(idManifiesto, indicePagina, registrosPorPagina)
                  .ToList()
                  .ConvertAll(r => new ONConsolidado()
                  {
                      IdManfiestoConsolidado = r.MOC_IdManfiestoOperaNacioConso,
                      NumeroPrecintoSalida = r.MOC_NumeroPrecintoSalida,
                      IdTipoConsolidado = r.MOC_IdTipoConsolidado,
                      NombreTipoConsolidado = r.TIC_Descripcion,
                      TotalEnviosConsolidado = r.TotalEnvios.Value,
                      NumeroGuiaInterna = r.MOC_NumeroGuiaInterna,
                      NumeroContenedorTula = r.MOC_NumeroContenedorTula,
                      IdTipoConsolidadoDetalle = r.MOC_TipoConsolidadoDetalle,
                      DescripcionConsolidadoDetalle = r.MOC_DescpConsolidadoDetalle,
                      NumeroPrecintoRetorno = r.MOC_NumeroPrecintoRetorno
                  });
                return a;
            }
        }

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosConsolidadoXManifiesto(long idManifiesto)
        {
            List<ONEnviosDescargueRutaDC> lista = new List<ONEnviosDescargueRutaDC>();
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerEnvConsManifiesto_OPN", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@idManifiesto", idManifiesto));
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ONEnviosDescargueRutaDC env = new ONEnviosDescargueRutaDC()
                    {
                        NumeroGuia = Convert.ToInt64(rdr["MOD_NumeroGuia"]),
                        IdAdmisionMensajeria = Convert.ToInt64(rdr["MOD_IdAdminisionMensajeria"]),
                        PesoGuiaSistema = Convert.ToDecimal(rdr["MOD_PesoEnIngreso"]),
                        IdCiudadOrigen = Convert.ToString(rdr["MOD_IdCiudadOrigen"]),
                        NombreCiudadOrigen = Convert.ToString(rdr["MOD_NombreCiudadOrigen"]),
                        IdLocalidadDestino = Convert.ToString(rdr["MOD_IdCiudadDestino"]),
                        NombreCiudadDestino = Convert.ToString(rdr["MOD_NombreCiudadDestino"])
                    };
                    lista.Add(env);
                }
                sqlConn.Close();
            }
            return lista;
        }

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONConsolidado> ObtenerEnviosConsolidadosXManifiesto(long idManifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvConsManifiesto_OPN(idManifiesto)
                  .ToList()
                  .ConvertAll(c =>
                      {
                          ONConsolidado conso = new ONConsolidado()
                          {
                              NumeroContenedorTula = c.MOC_NumeroContenedorTula,
                              NumeroPrecintoSalida = c.MOC_NumeroPrecintoSalida,
                              NombreTipoConsolidado = c.TIC_DescripcionTipoConsolidado
                          };

                          if (c.MOD_TotalPiezas <= 0)
                          {
                              conso.NumeroGuiaRotulo = c.MOD_NumeroGuia.ToString();
                          }
                          else
                          {
                              conso.NumeroGuiaRotulo = c.MOD_NumeroGuia + "-" + c.MOD_NumeroPieza + "/" + c.MOD_TotalPiezas;
                          }
                          return conso;
                      });
            }
        }

        /// <summary>
        /// Obtiene los envios de consolidados de un manifiesto y la ciudad Manifestada
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONConsolidado> ObtenerEnviosConsolidadosXManifiestoCiudaManifestada(long idManifiesto, string idCiudadManifestada)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvConsManifiestoCiudadManifestada_OPN(idManifiesto, idCiudadManifestada)
                  .ToList()
                  .ConvertAll(c =>
                  {
                      ONConsolidado conso = new ONConsolidado()
                      {
                          NumeroContenedorTula = c.MOC_NumeroContenedorTula,
                          NumeroPrecintoSalida = c.MOC_NumeroPrecintoSalida,
                          NombreTipoConsolidado = c.TIC_DescripcionTipoConsolidado,
                          LocalidadManifestada = new PALocalidadDC() { IdLocalidad = idCiudadManifestada },
                          NumControlTransManIda = c.MOC_NumControlTransManIda,
                          NumControlTransManRet = c.MOC_NumControlTransManRet,
                          IdManfiestoConsolidado = c.MOC_IdManfiestoOperaNacioConso
                      };

                      if (c.MOD_TotalPiezas <= 0)
                      {
                          conso.NumeroGuiaRotulo = c.MOD_NumeroGuia.ToString();
                      }
                      else
                      {
                          conso.NumeroGuiaRotulo = c.MOD_NumeroGuia + "-" + c.MOD_NumeroPieza + "/" + c.MOD_TotalPiezas;
                      }
                      return conso;
                  });
            }
        }

        /// <summary>
        /// Obtiene la cantidad de envios de consolidados de un manifiesto y la ciudad Manifestada
        /// </summary>
        /// <param name="idManifiesto"></param>
        public int ObtenerCantidadEnviosConsolidadosXManifiestoCiudaManifestada(long idManifiesto, string idCiudadManifestada)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerCantidadConsManifiestoCiudadManifestada_OPN(idManifiesto, idCiudadManifestada).First().Value;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de envios dentro de un consolidado de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public int ObtenerCantidadEnviosConsolidados(long idConsolidadoManifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerCantidadEnviosConsolidadoManifiesto_OPN(idConsolidadoManifiesto).First().Value;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de consolidados con el mismo numero de contenedor o tula en una manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public int ObtenerCantidadConsolidadosXContenedorTulaYManifiesto(long idManifiesto, string numContenedorTula)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerCantidadConsoNumConTulaManifiesto_OPN(idManifiesto, numContenedorTula).First().Value;
            }
        }



        /// <summary>
        /// Obtiene los envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosSueltosXManifiesto(long idManifiesto)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvSuelManifiesto_OPN(idManifiesto)
                    .ToList()
                  .ConvertAll(r =>
                      {
                          ONEnviosDescargueRutaDC des = new ONEnviosDescargueRutaDC()
                       {
                           NumeroGuia = r.MOG_NumeroGuia,
                           IdAdmisionMensajeria = r.MOG_IdAdminisionMensajeria,
                           PesoGuiaSistema = r.MOG_PesoEnIngreso,
                           IdCiudadOrigen = r.MOG_IdCiudadOrigen,
                           NombreCiudadOrigen = r.MOG_NombreCiudadOrigen,
                           IdLocalidadDestino = r.MOG_IdCiudadDestino,
                           NombreCiudadDestino = r.MOG_NombreCiudadDestino,
                           IdCentroServicioDestino = r.MOG_IdCentroServicioDestino,
                           IdCentroServicioOrigen = r.MOG_IdCentroServicioOrigen,
                           NombreCentroServicioDestino = r.MOG_NombreCentroServicioDestino,
                           NombreCentroServicioOrigen = r.MOG_NombreCentroServicioOrigen
                       };
                          if (r.MOG_TotalPiezas.HasValue)
                              des.TotalPiezasRotulo = r.MOG_TotalPiezas.Value;
                          else
                              des.TotalPiezasRotulo = 0;

                          if (r.MOG_NumeroPieza.HasValue)
                              des.PiezaActualRotulo = r.MOG_NumeroPieza.Value;
                          else
                              des.PiezaActualRotulo = 0;

                          return des;
                      });
            }
        }

        /// <summary>
        /// Obtiene los envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public List<ONEnviosDescargueRutaDC> ObtenerEnviosSueltosXManifiestoCiudadManifestada(long idManifiesto, string CiudadManifestada)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerEnvSuelManifiestoCiudadManifestada_OPN(idManifiesto, CiudadManifestada)
                    .ToList()
                  .ConvertAll(r =>
                  {
                      ONEnviosDescargueRutaDC des = new ONEnviosDescargueRutaDC()
                      {
                          NumeroGuia = r.MOG_NumeroGuia,
                          IdAdmisionMensajeria = r.MOG_IdAdminisionMensajeria,
                          PesoGuiaSistema = r.MOG_PesoEnIngreso,
                          IdCiudadOrigen = r.MOG_IdCiudadOrigen,
                          NombreCiudadOrigen = r.MOG_NombreCiudadOrigen,
                          IdLocalidadDestino = r.MOG_IdCiudadDestino,
                          NombreCiudadDestino = r.MOG_NombreCiudadDestino,
                          IdCentroServicioDestino = r.MOG_IdCentroServicioDestino,
                          IdCentroServicioOrigen = r.MOG_IdCentroServicioOrigen,
                          NombreCentroServicioDestino = r.MOG_NombreCentroServicioDestino,
                          NombreCentroServicioOrigen = r.MOG_NombreCentroServicioOrigen
                      };
                      if (r.MOG_TotalPiezas.HasValue)
                          des.TotalPiezasRotulo = r.MOG_TotalPiezas.Value;
                      else
                          des.TotalPiezasRotulo = 0;

                      if (r.MOG_NumeroPieza.HasValue)
                          des.PiezaActualRotulo = r.MOG_NumeroPieza.Value;
                      else
                          des.PiezaActualRotulo = 0;

                      return des;
                  });
            }
        }

        /// <summary>
        /// Obtiene la cantidad de  envios sueltos de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public int ObtenerCantidadEnviosSueltosXManifiestoCiudadManifestada(long idManifiesto, string CiudadManifestada)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerCantidadEnviosSueltosManifiestoCiudadManifestada_OPN(idManifiesto, CiudadManifestada).First().Value;
            }
        }

        /// <summary>
        /// Obtiene los totales de manifiesto, total sobrantes, total faltantes
        /// </summary>
        /// <param name="idManifiesto"></param>
        /// <param name="idOperativo"></param>
        public ONCierreDescargueManifiestoDC ObtenerTotalCierreManifiesto(long idManifiesto, long idOperativo)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ONCierreDescargueManifiestoDC totalesCierre = new ONCierreDescargueManifiestoDC();
                var totales = contexto.paObtenerTotCieOperatManif_OPN(idManifiesto, idOperativo).FirstOrDefault();

                totalesCierre.TotalEnviosManifestado = totales.TotalManifestados.Value;
                totalesCierre.TotalEnviosPendientes = totales.TotalFaltantes.Value;
                totalesCierre.TotalEnviosSobrantes = totales.TotalSobrantes.Value;

                return totalesCierre;
            }
        }

        #endregion Ingreso COL por Manifiesto

        #region Ingreso a centro de acopio regional o nacional

        /// <summary>
        /// Método para actualizar los consolidados de un manifiesto
        /// </summary>
        /// <param name="NoControl"></param>
        /// <param name="NoPrecinto"></param>
        /// <param name="NoTula"></param>
        /// <returns></returns>
        public List<ONConsolidado> ObtenerManifiestosConsolidados(long controlTrans, long noPrecinto, string noConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ONConsolidado> consolidados = new List<ONConsolidado>();
                IDictionary<string, string> filtro = new Dictionary<string, string>();
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                if (controlTrans != 0)
                {
                    LambdaExpression lamda = contexto.CrearExpresionLambda<ManfiestoOperaNacioConsoli_OPN>("MOC_IdManifiestoOperacionNacio", controlTrans.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda, OperadorLogico.And);
                }
                if (noPrecinto != 0)
                {
                    LambdaExpression lamda1 = contexto.CrearExpresionLambda<ManfiestoOperaNacioConsoli_OPN>("MOC_NumeroPrecintoSalida", noPrecinto.ToString(), OperadorComparacion.Equal);
                    where.Add(lamda1, OperadorLogico.And);
                }
                if (!String.IsNullOrEmpty(noConsolidado))
                {
                    LambdaExpression lamda2 = contexto.CrearExpresionLambda<ManfiestoOperaNacioConsoli_OPN>("MOC_NumeroContenedorTula", noConsolidado, OperadorComparacion.Contains);
                    where.Add(lamda2, OperadorLogico.And);
                }

                int totalRegistros;
                var asignacionesManifiesto = contexto.ConsultarManfiestoOperaNacioConsoli_OPN(filtro, where, string.Empty, out totalRegistros, 0, 100, true)
                       .Where(aS => aS.MOC_EsIngresado == false)
                       .ToList();
                if (asignacionesManifiesto != null && asignacionesManifiesto.Any())
                {
                    asignacionesManifiesto.ForEach(asg =>
                    {
                        consolidados.Add(
                                            new ONConsolidado
                                            {
                                                NumeroContenedorTula = asg.MOC_NumeroContenedorTula,
                                                NumeroPrecintoRetorno = asg.MOC_NumeroPrecintoRetorno,
                                                NumeroPrecintoSalida = asg.MOC_NumeroPrecintoSalida,
                                                IdManfiestoConsolidado = asg.MOC_IdManfiestoOperaNacioConso,
                                                NumControlTransManIda = asg.MOC_NumControlTransManIda,
                                                NumControlTransManRet = asg.MOC_NumControlTransManRet,
                                            });
                    });
                }
                return consolidados;
            }
        }

        /// <summary>
        /// Método para cambiar el estado ingresado de un manifiesto
        /// </summary>
        /// <param name="idManifiesto"></param>
        public void CambiarEstadoConsolidado(long idConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ManfiestoOperaNacioConsoli_OPN man = contexto.ManfiestoOperaNacioConsoli_OPN.FirstOrDefault(m => m.MOC_IdManfiestoOperaNacioConso == idConsolidado);
                if (man != null)
                {
                    man.MOC_EsIngresado = true;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Método para ingresar un consolidado
        /// </summary>
        /// <param name="asignacion"></param>
        /// <param name="idCentroServicio"></param>
        public void IngresoConsolidado(ONConsolidado asignacion, long idCentroServicio)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoOpeAgeConsolidado_OPN ingreso = new IngresoOpeAgeConsolidado_OPN
                {
                    IOC_CreadoPor = ControllerContext.Current.Usuario,
                    IOC_FechaGrabacion = DateTime.Now,
                    IOG_IdCentroServicio = idCentroServicio,
                    IOC_IdManifiestoOperaNacioCo = asignacion.IdManfiestoConsolidado,
                    IOC_NumControlTransManifi = asignacion.NumControlTransManIda,
                    IOC_NumeroContenedorTula = asignacion.NumeroContenedorTula,
                };
                contexto.IngresoOpeAgeConsolidado_OPN.Add(ingreso);
                contexto.SaveChanges();
            }
        }

        #endregion Ingreso a centro de acopio regional o nacional

        #region Descargue Consolidados

        #region Consultas

        /// <summary>
        /// Obtiene los Consolidados de Guias para el descarge Urbano - Regional - Nacional
        /// </summary>
        /// <param name="ingresoConsolidado">info a consultar</param>
        /// <returns>info de las guias descargadas</returns>
        public ONDescargueConsolidadosUrbRegNalDC ObtenerIngresoConsolidado(ONDescargueConsolidadosUrbRegNalDC ingresoConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ONDescargueConsolidadosUrbRegNalDC infoIngresoConsolidado = new ONDescargueConsolidadosUrbRegNalDC();

                // Consulto si existe un consolidado en la tbl IngresoConsolidados_OPN
                paObtenerIngresoConsolidado_OPN infoIngreso = contexto.paObtenerIngresoConsolidado_OPN(ingresoConsolidado.CtrolManifiesto, ingresoConsolidado.NumPrecinto, ingresoConsolidado.NumTula).FirstOrDefault();

                //Valido si exite info
                if (infoIngreso != null)
                {
                    infoIngresoConsolidado = new ONDescargueConsolidadosUrbRegNalDC()
                    {
                        CtrolManifiesto = infoIngreso.CtrlTransMan,
                        NumPrecinto = infoIngreso.NumPrecinto.Value,
                        NumTula = infoIngreso.NumTula,
                        ListNovedadConsolidado = contexto.IngresoConsoNovedad_OPN.Include("NovedadDescargueConso_OPN")
                                                    .Where(ing => ing.ICN_NumControlTransMan == infoIngreso.CtrlTransMan)
                                                    .ToList().ConvertAll<ONNovedadesConsolidadoDC>(lst => new ONNovedadesConsolidadoDC()
                                                    {
                                                        NumCtrolTransMan = infoIngreso.CtrlTransMan,
                                                        IdNovedadConsolidado = lst.ICN_IdNovedadDescargue,
                                                        DescripcionNovedadConsolidado = lst.NovedadDescargueConso_OPN.NDC_Descripcion,
                                                        NovedadSeleccionada = true
                                                    }),

                        ListGuiasIngresadas = contexto.paObtenerIngresoConYSinNumeroGuia_OPN(infoIngreso.CtrlTransMan)
                        .ToList().ConvertAll<OnDescargueEnvioSueltoDC>(ingrso => new OnDescargueEnvioSueltoDC()
                        {
                            IdIngresoGuia = ingrso.IdIngreso,
                            NumeroGuia = ingrso.NumGuia.ToString(),
                            NumeroPiezas = ingrso.NumPieza.Value,
                            TotalPiezas = ingrso.TotalPiezas.Value,
                            IngresoConGuia = ingrso.IngresoConGuia.Value,
                            LstNovedadesGuias = contexto.paObtenerNovedadesIngresosGuia_OPN(ingrso.IdIngreso)
                            .ToList().ConvertAll<ONNovedadesEnvioDC>(novedd => new ONNovedadesEnvioDC()
                            {
                                IdNovedadEnvioSuelto = novedd.IdNovedad,
                                DescripcionNovedad = novedd.DescripcionNovedad,
                                NovedadSeleccionada = true
                            })
                        })
                    };
                }

                //Si no existe consulto en AsignacionTulaPuntoServicio_OPU y ManfiestoOperaNacioConsoli_OPN para crearlo
                else
                {
                    paObtenerConsolidadosUrbRegNal_OPN infoConsUrbRegNal = contexto.paObtenerConsolidadosUrbRegNal_OPN(ingresoConsolidado.CtrolManifiesto, ingresoConsolidado.NumPrecinto, ingresoConsolidado.NumTula).FirstOrDefault();
                    if (infoConsUrbRegNal != null && infoConsUrbRegNal.CtrlTransMan != 0)
                    {
                        infoIngresoConsolidado = new ONDescargueConsolidadosUrbRegNalDC()
                        {
                            CtrolManifiesto = infoConsUrbRegNal.CtrlTransMan.Value,
                            NumPrecinto = infoConsUrbRegNal.NumPrecinto.Value,
                            NumTula = infoConsUrbRegNal.NumTula,
                            ListNovedadConsolidado = new List<ONNovedadesConsolidadoDC>(),
                            ListGuiasIngresadas = new List<OnDescargueEnvioSueltoDC>(),
                            EstadoAsignacion = infoConsUrbRegNal.Estado,
                            EstaDescargadoManifiesto = infoConsUrbRegNal.Descargado.Value
                        };
                    }
                }

                return infoIngresoConsolidado;
            }
        }

        /// <summary>
        /// Obtiene las novedades delos ingresos de la Guia
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesEnvioDC> ObtenerNovedadesIngresosGuia()
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.NovedadesEnviosSueltos_OPU
                                    .ToList()
                                    .ConvertAll<ONNovedadesEnvioDC>(nov => new ONNovedadesEnvioDC()
                                    {
                                        IdNovedadEnvioSuelto = nov.NES_IdNovedad,
                                        DescripcionNovedad = nov.NES_Descripcion
                                    });
            }
        }

        /// <summary>
        ///Obtiene las novedades de Descargue Consolidado
        /// </summary>
        /// <returns></returns>
        public List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueConsolidado()
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<ONNovedadesConsolidadoDC> todasNovedades = contexto.NovedadDescargueConso_OPN
                    .ToList()
                    .ConvertAll<ONNovedadesConsolidadoDC>(novOpe => new ONNovedadesConsolidadoDC()
                    {
                        IdNovedadConsolidado = novOpe.NDC_IdNovedadDescargue,
                        DescripcionNovedadConsolidado = novOpe.NDC_Descripcion
                    });

                return todasNovedades;
            }
        }

        /// <summary>
        ///Obtiene las novedades de Un descarque Especifico
        /// </summary>
        /// <returns>Lista de Novedades Asociadas</returns>
        public List<ONNovedadesConsolidadoDC> ObtenerNovedadesDescargueUnico(long NumCtrolTransMan)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.IngresoConsoNovedad_OPN.Where(nov => nov.ICN_NumControlTransMan == NumCtrolTransMan)
                    .ToList()
                    .ConvertAll<ONNovedadesConsolidadoDC>(novOpe => new ONNovedadesConsolidadoDC()
                    {
                        NumCtrolTransMan = novOpe.ICN_NumControlTransMan,
                        IdNovedadConsolidado = novOpe.ICN_IdNovedadDescargue
                    });
            }
        }

        /// <summary>
        /// Obtiene las Novedades de las Guias Ingresadas independientemente si
        /// estan en admin Mensajeria o no
        /// </summary>
        /// <param name="idAdminGuia">Numero de la Guia Asociada</param>
        /// <returns>Lista de la Novedades Asociadas</returns>
        public List<ONNovedadesEnvioDC> ObtenerNovedadesGuia(long idAdminGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerNovedadesIngresosGuia_OPN(idAdminGuia)
                             .ToList().ConvertAll<ONNovedadesEnvioDC>(novedd => new ONNovedadesEnvioDC()
                             {
                                 IdNovedadEnvioSuelto = novedd.IdNovedad,
                                 DescripcionNovedad = novedd.DescripcionNovedad,
                                 NovedadSeleccionada = true
                             });
            }
        }

        /// <summary>
        /// obtiene la Guia ya Ingresada para su descargue
        /// </summary>
        /// <param name="idGuia"></param>
        /// <returns>info de la Guia Ingresada</returns>
        public OnDescargueEnvioSueltoDC ObtenerGuiaIngresadaDescargada(long idGuia)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGuiaIngresadaConsolidado_OPN(idGuia).ToList().ConvertAll<OnDescargueEnvioSueltoDC>(guia => new OnDescargueEnvioSueltoDC()
                {
                    NumCtrolTransMan = guia.NumCtrlTranMan,
                    IdAdminMensajeria = guia.IdAdminMenj.Value,
                    IdIngresoGuia = guia.IdIngreso
                }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtiene las Novedades de la Guia de Ingreso
        /// </summary>
        /// <param name="idIngreso"></param>
        /// <returns></returns>
        public List<ONNovedadesEnvioDC> ObtenerNovedadesIngresoGuia(long idIngreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerNovedadesIngresosGuia_OPN(idIngreso).ToList().ConvertAll<ONNovedadesEnvioDC>(guia => new ONNovedadesEnvioDC()
                {
                    IdNovedadEnvioSuelto = guia.IdNovedad,
                    DescripcionNovedad = guia.DescripcionNovedad,
                });
            }
        }

        #endregion Consultas

        #region Borrar

        /// <summary>
        ///Borra la Novedade Asociada al Ingreso Consolidado
        /// </summary>
        public void BorrarNovedadIngresoConsolidado(long NumCtrolTransMan, int idNovedadAsociada)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoConsoNovedad_OPN borraNovedad = contexto.IngresoConsoNovedad_OPN.FirstOrDefault(nov => nov.ICN_NumControlTransMan == NumCtrolTransMan && nov.ICN_IdNovedadDescargue == idNovedadAsociada);
                if (borraNovedad != null)
                {
                    contexto.IngresoConsoNovedad_OPN.Remove(borraNovedad);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Borra las novedades de la Guia que esta en admincion Mensajeria
        /// </summary>
        /// <param name="idIngreso"></param>
        public void BorrarNovedadConGuiaConsolidado(long idIngreso, int idNovedad)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoConGuiaNov_OPN borrarNovedad = contexto.IngresoConGuiaNov_OPN.FirstOrDefault(nov => nov.ICG_IdIngreso == idIngreso && nov.ICG_IdNovedad == idNovedad);
                if (borrarNovedad != null)
                {
                    contexto.IngresoConGuiaNov_OPN.Remove(borrarNovedad);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Borra las novedades que esten asociadas a la
        /// guia de y no esten en admi Mensaj
        /// </summary>
        /// <param name="idIngreso"></param>
        public void BorrarNovedadSinGuiaConsolidado(long idIngreso, int idNovedad)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoConNoGuiaNov_OPN borrarNovedad = contexto.IngresoConNoGuiaNov_OPN.FirstOrDefault(nov => nov.ICN_IdIngreso == idIngreso && nov.ICN_IdNovedad == idNovedad);
                if (borrarNovedad != null)
                {
                    contexto.IngresoConNoGuiaNov_OPN.Remove(borrarNovedad);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Borrar

        #region Validar

        /// <summary>
        /// Valida si existe el numero de Control de transporte de Manifiesto
        /// </summary>
        /// <param name="NumCtrolTransMan"></param>
        /// <returns>Verdadero si lo encuentra y falso si no existe</returns>
        public bool ValidarIngresoConsolidado(long NumCtrolTransMan)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.paObtenerIngresoConsolidado_OPN(NumCtrolTransMan, 0, "0").FirstOrDefault() != null)
                    return true;
                else
                    return false;
            }
        }

        #endregion Validar

        #region Adicionar

        /// <summary>
        /// Adiciona el IngresoConsolidado a la Tbla
        /// </summary>
        /// <param name="ingresoConsolidado">info a Ingresar</param>
        public void AdicionarIngresoConsolidado(ONDescargueConsolidadosUrbRegNalDC ingresoConsolidado)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIngresoConsolidado_OPN(ingresoConsolidado.CtrolManifiesto, ingresoConsolidado.NumPrecinto,
                                                        ingresoConsolidado.NumTula, ingresoConsolidado.EsmanifiestoManual,
                                                        ingresoConsolidado.IdCentroServicio, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Adiciona Nva Novedad
        /// </summary>
        /// <param name="nvaNovedad"></param>
        public void AdicionarNovedadIngresoConsolidado(ONNovedadesConsolidadoDC nvaNovedad, long numCtrolTransMan)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IngresoConsoNovedad_OPN nvNovedad = new IngresoConsoNovedad_OPN()
                {
                    ICN_NumControlTransMan = numCtrolTransMan,
                    ICN_IdNovedadDescargue = nvaNovedad.IdNovedadConsolidado,
                    ICN_FechaGrabacion = DateTime.Now,
                    ICN_CreadoPor = ControllerContext.Current.Usuario
                };
                contexto.IngresoConsoNovedad_OPN.Add(nvNovedad);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona una Guia que aparece en Tbl Admin Mensajeria
        /// </summary>
        /// <param name="nvaGuia">info de la guia</param>
        /// <param name="numCtrolTrans">nuemr ctrol</param>
        public long AdicionarIngresoConGuia(ADGuia nvaGuia, long numCtrolTrans)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paCrearIngresoGuiaConAdmiMensaj_OPN(numCtrolTrans, nvaGuia.NumeroGuia, nvaGuia.IdAdmision,
                                                              nvaGuia.NumeroPieza, nvaGuia.TotalPiezas,
                                                              ControllerContext.Current.Usuario).FirstOrDefault().idIngreso.Value;
            }
        }

        /// <summary>
        /// Adiciona una Guia que NO aparece Tbl Admin Mensajeria
        /// </summary>
        /// <param name="nvaGuia">info de la  Guia</param>
        /// <param name="numCtrolTrans">numeor de Ctrl</param>
        public long AdicionarIngresoSinGuia(OnDescargueEnvioSueltoDC nvaGuia, long numCtrolTrans)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paCrearIngresoGuiaNOAdmiMensaj_OPN(numCtrolTrans, Convert.ToInt64(nvaGuia.NumeroGuia), ControllerContext.Current.Usuario).FirstOrDefault().idIngreso.Value;
            }
        }

        /// <summary>
        /// Adiciona una Novedad de Guia que aparece en Tbl Admin Mensajeria
        /// </summary>
        /// <param name="nvaGuia">info de la Novedad de guia</param>
        /// <param name="numCtrolTrans">nuemr ctrol</param>
        public void AdicionarNovedadIngresoConGuia(ONNovedadesEnvioDC nvaNovedadGuia, long idIngreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearNovedadGuiaConAdmiMensaj_OPN(idIngreso, nvaNovedadGuia.IdNovedadEnvioSuelto,
                                                                                    ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        /// Adiciona una Novedad de Guia que NO aparece Tbl Admin Mensajeria
        /// </summary>
        /// <param name="nvaGuia">info de la  Guia</param>
        /// <param name="numCtrolTrans">numeor de Ctrl</param>
        public void AdicionarNovedadIngresoSinGuia(ONNovedadesEnvioDC nvaNovedadGuia, long idIngreso)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearNovedadGuiaNOAdmiMensaj_OPN(idIngreso, nvaNovedadGuia.IdNovedadEnvioSuelto,
                                                                            ControllerContext.Current.Usuario);
            }
        }

        #endregion Adicionar

        #region Actualizar

        /// <summary>
        /// Actualiza el Estado del Manifiesto
        /// </summary>
        /// <param name="numCtrlTranMan"></param>
        public void ActualizarManifiestoOperacionNalConsolidado(long numCtrlTranMan)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarManifiestoOperNal_OPN(numCtrlTranMan);
            }
        }

        /// <summary>
        /// Actualiza el estado de la AsignacionTula
        /// </summary>
        /// <param name="numCtrlTranMan"></param>
        public void ActualizarAsignacionTulaPuntoServicio(long numCtrlTranMan)
        {
            using (ModeloOperacionNacional contexto = new ModeloOperacionNacional(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarAsignacionTulaPtoSvc_OPN(numCtrlTranMan);
            }
        }

        #endregion Actualizar

        #endregion Descargue Consolidados
    }
}