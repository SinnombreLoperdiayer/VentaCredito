using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CO.Servidor.GestionGiros.Comun;
using CO.Servidor.PagosManuales.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.PagosManuales;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.PagosManuales.Datos
{
    public class GIRepositorioPagosManuales
    {
        #region Atributos

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private const string NombreModelo = "ModeloPagosManuales";

        #endregion Atributos

        #region Crear Instancia

        private static readonly GIRepositorioPagosManuales instancia = new GIRepositorioPagosManuales();

        /// <summary>
        /// Retorna la instancia de la clase GIRepositorioPagosManuales
        /// </summary>
        public static GIRepositorioPagosManuales Instancia
        {
            get { return GIRepositorioPagosManuales.instancia; }
        }

        #endregion Crear Instancia

        #region Metodos

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicioDestino, int indicePagina, int registrosPorPagina)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGiroNoTransmitido_GIR(indicePagina, registrosPorPagina, idCentroServicioDestino, GIConstantesSolicitudes.ESTADO_GIRO_ACT.ToString())
                  .ToList().
                ConvertAll<GIAdmisionGirosDC>(
                  adm => new GIAdmisionGirosDC()
                  {
                      IdAdminGiro = adm.ADG_IdAdmisionGiro,
                      CodVerfiGiro = adm.ADG_DigitoVerificacion,
                      IdGiro = adm.ADG_IdGiro,

                      GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                      {
                          ClienteRemitente = new CLClienteContadoDC()
                          {
                              TipoId = adm.ADG_IdTipoIdentificacionRemitente,
                              Identificacion = adm.ADG_IdRemitente,
                              Nombre = adm.ADG_NombreRemitente
                          },
                          ClienteDestinatario = new CLClienteContadoDC()
                          {
                              TipoId = adm.ADG_IdTipoIdentificacionDestinatario,
                              Identificacion = adm.ADG_IdDestinatario,
                              Nombre = adm.ADG_NombreDestinatario
                          }
                      },
                      AgenciaOrigen = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = adm.ADG_IdCentroServicioOrigen,
                          Nombre = adm.ADG_NombreCentroServicioOrigen,
                          CodigoPostal = adm.ADG_CodigoPostalOrigen,
                          NombreMunicipio = adm.ADG_DescCiudadOrigen,
                          NombrePais = adm.ADG_DescPaisOrigen
                      },
                      AgenciaDestino = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = adm.ADG_IdCentroServicioDestino,
                          Nombre = adm.ADG_NombreCentroServicioDestino,
                          CodigoPostal = adm.ADG_CodigoPostalDestino,
                          NombreMunicipio = adm.ADG_DescCiudadDestino,
                          NombrePais = adm.ADG_DescPaisDestino
                      },
                      Precio = new TAPrecioDC
                      {
                          ValorGiro = adm.ADG_ValorGiro,
                          ValorServicio = adm.ADG_ValorPorte,
                          ValorImpuestos = adm.ADG_ValorImpuestos,
                          ValorAdicionales = adm.ADG_ValorAdicionales,
                          ValorTotal = adm.ADG_ValorTotal
                      },
                      FechaGrabacion = adm.ADG_FechaGrabacion,
                      EstadoGiro = adm.UltimoEstadoGiro
                  });
            }
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgenciaPorTrasmitir(long idCentroServicioDestino)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerGiroNoTransmitidoAgencia_GIR(idCentroServicioDestino)
                  .ToList().
                ConvertAll<GIAdmisionGirosDC>(
                  adm => new GIAdmisionGirosDC()
                  {
                      IdAdminGiro = adm.ADG_IdAdmisionGiro,
                      CodVerfiGiro = adm.ADG_DigitoVerificacion,
                      IdGiro = adm.ADG_IdGiro,

                      GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                      {
                          ClienteRemitente = new CLClienteContadoDC()
                          {
                              TipoId = adm.ADG_IdTipoIdentificacionRemitente,
                              Identificacion = adm.ADG_IdRemitente,
                              Nombre = adm.ADG_NombreRemitente
                          },
                          ClienteDestinatario = new CLClienteContadoDC()
                          {
                              TipoId = adm.ADG_IdTipoIdentificacionDestinatario,
                              Identificacion = adm.ADG_IdDestinatario,
                              Nombre = adm.ADG_NombreDestinatario
                          }
                      },
                      AgenciaOrigen = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = adm.ADG_IdCentroServicioOrigen,
                          Nombre = adm.ADG_NombreCentroServicioOrigen,
                          CodigoPostal = adm.ADG_CodigoPostalOrigen,
                          NombreMunicipio = adm.ADG_DescCiudadOrigen,
                          NombrePais = adm.ADG_DescPaisOrigen
                      },
                      AgenciaDestino = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = adm.ADG_IdCentroServicioDestino,
                          Nombre = adm.ADG_NombreCentroServicioDestino,
                          CodigoPostal = adm.ADG_CodigoPostalDestino,
                          NombreMunicipio = adm.ADG_DescCiudadDestino,
                          NombrePais = adm.ADG_DescPaisDestino
                      },
                      Precio = new TAPrecioDC
                      {
                          ValorGiro = adm.ADG_ValorGiro,
                          ValorServicio = adm.ADG_ValorPorte,
                          ValorImpuestos = adm.ADG_ValorImpuestos,
                          ValorAdicionales = adm.ADG_ValorAdicionales,
                          ValorTotal = adm.ADG_ValorTotal
                      },
                      FechaGrabacion = adm.ADG_FechaGrabacion,
                      EstadoGiro = adm.UltimoEstadoGiro
                  });
            }
        }

        /// <summary>
        /// consulta los giros activos para realizar el descarge manual de pagos
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosADescargar(long? idCentroServicioDestino, long? idGiro, int indicePagina, int registrosPorPagina)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var infoGirosDescargaManual = contexto.paObtenerGirosDescargaManu_GIR(indicePagina, registrosPorPagina, idCentroServicioDestino, GIConstantesSolicitudes.ESTADO_GIRO_ACT.ToString(), idGiro).ToList();
                if (infoGirosDescargaManual != null && infoGirosDescargaManual.Any())
                {
                    return infoGirosDescargaManual.
                    ConvertAll<GIAdmisionGirosDC>(
                      adm => new GIAdmisionGirosDC()
                      {
                          IdAdminGiro = adm.ADG_IdAdmisionGiro,
                          CodVerfiGiro = adm.ADG_DigitoVerificacion,

                          IdGiro = adm.ADG_IdGiro,
                          GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                          {
                              ClienteRemitente = new CLClienteContadoDC()
                              {
                                  TipoId = adm.ADG_IdTipoIdentificacionRemitente,
                                  Identificacion = adm.ADG_IdRemitente,
                                  Nombre = adm.ADG_NombreRemitente
                              },
                              ClienteDestinatario = new CLClienteContadoDC()
                              {
                                  TipoId = adm.ADG_IdTipoIdentificacionDestinatario,
                                  Identificacion = adm.ADG_IdDestinatario,
                                  Nombre = adm.ADG_NombreDestinatario
                              }
                          },
                          AgenciaOrigen = new PUCentroServiciosDC()
                          {
                              IdCentroServicio = adm.ADG_IdCentroServicioOrigen,
                              Nombre = adm.ADG_NombreCentroServicioOrigen,
                              CodigoPostal = adm.ADG_CodigoPostalOrigen,
                              NombreMunicipio = adm.ADG_DescCiudadOrigen,
                              NombrePais = adm.ADG_DescPaisOrigen
                          },
                          AgenciaDestino = new PUCentroServiciosDC()
                          {
                              IdCentroServicio = adm.ADG_IdCentroServicioDestino,
                              Nombre = adm.ADG_NombreCentroServicioDestino,
                              CodigoPostal = adm.ADG_CodigoPostalDestino,
                              NombreMunicipio = adm.ADG_DescCiudadDestino,
                              NombrePais = adm.ADG_DescPaisDestino
                          },
                          Precio = new TAPrecioDC
                          {
                              ValorGiro = adm.ADG_ValorGiro,
                              ValorServicio = adm.ADG_ValorPorte,
                              ValorImpuestos = adm.ADG_ValorImpuestos,
                              ValorAdicionales = adm.ADG_ValorAdicionales,
                              ValorTotal = adm.ADG_ValorTotal
                          },
                          FechaGrabacion = adm.ADG_FechaGrabacion,
                          EstadoGiro = adm.ESG_Estado
                      });
                }
                else
                    return new List<GIAdmisionGirosDC>();
            }
        }

        /// <summary>
        /// Obtiene la informacion de los intentos a transmitir de un giro
        /// </summary>
        /// <param name="idAdminGiro"></param>
        /// <returns></returns>
        public List<GIIntentosTransmisionGiroDC> ObtenerIntentosTransmitir(long idAdminGiro)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.IntentosTransmisionGiro_GIR.Where(it => it.ITG_IdAdmisionGiro == idAdminGiro)
                  .ToList().ConvertAll(tran => new GIIntentosTransmisionGiroDC()
                  {
                      IdIntentoTransGiro = tran.ITG_IdIntentoTransGiro,
                      FechaGrabacion = tran.ITG_FechaGrabacion,
                      Observaciones = tran.ITG_Observaciones,
                      IdPlanilla = tran.ITG_NoPlanillaTransmision,
                      TipoTransmision = (GIEnumTipoTransmisionDC)Enum.Parse(typeof(GIEnumTipoTransmisionDC), tran.ITG_Resultado, true),
                      CreadoPor = tran.ITG_CreadoPor,
                      NombreContacto = tran.ITG_IdentificacionContactoTransmision + " - " + tran.ITG_NombreContactoTransmision
                  });
            }
        }

        /// <summary>
        /// Consultar informacion del giro adicionales - impuestos- intentos transmitir
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroTransmisionGiro(long idGiro)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIAdmisionGirosDC admisiongiro;
                GIpaObtenerGiroIdGiro giro = contexto.paObtenerGiroIdGiro_GIR(idGiro).FirstOrDefault();

                admisiongiro = new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.EstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen.Value,
                        Nombre = giro.NombreCentroServicioOrigen
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino.Value,
                        Nombre = giro.NombreCentroServicioDestino
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro.Value
                    },
                    Observaciones = giro.Observaciones,
                    ObservacionesSolicitudes = giro.ObservacionesSolicitudes
                };

                if (giro.TipoGiro == GIConstantesSolicitudes.GIROPEATONAPEATON)
                {
                    admisiongiro.GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                    {
                        ClienteRemitente = new CLClienteContadoDC()
                        {
                            Identificacion = giro.IdRemitente,
                            TipoId = giro.TipoIdRemitente,
                            Apellido1 = giro.Apellido1Remitente,
                            Apellido2 = giro.Apellido2Remitente,
                            Nombre = giro.NombreRemitente,
                            Telefono = giro.TelefonoRemitente,
                            Direccion = giro.DireccionRemitente,
                            Email = giro.EmailRemitente,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionRemitente }
                        },
                        ClienteDestinatario = new CLClienteContadoDC()
                        {
                            Identificacion = giro.IdDestinatario,
                            TipoId = giro.TipoIdDestinatario,
                            Apellido1 = giro.Apellido1Destinatario,
                            Apellido2 = giro.Apellido2Destinatario,
                            Nombre = giro.NombreDestinatario,
                            Telefono = giro.TelefonoDestinatario,
                            Direccion = giro.DireccionDestinatario,
                            Email = giro.EmailDestinatario,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario },
                            TipoIdentificacionReclamoGiro = giro.TipoIdentificacionReclamoGiro
                        }
                    };
                }

                var impuestos = contexto.paObtenerGiroImpuestos_GIR(giro.IdAdmisionGiro);

                if (impuestos != null)
                {
                    admisiongiro.Precio.InfoImpuestos = impuestos.ToList().ConvertAll(
                      imp => new TAImpuestoDelServicio()
                      {
                          DescripcionImpuesto = imp.DescripcionImpuesto,
                          ValorImpuesto = imp.TarifaImpuesto
                      });
                }

                var adicional = contexto.paObtenerGiroAdicionales_GIR(giro.IdAdmisionGiro);

                if (adicional != null)
                {
                    admisiongiro.Precio.ServiciosSolicitados = new ObservableCollection<TAValorAdicional>(adicional.ToList().GroupBy(
                      giroAdd => giroAdd.IdServicioAdicional,
                      (Key, contenido) =>
                        new TAValorAdicional()
                        {
                            Descripcion = contenido.First().DescripcionServicioAdicional,
                            PrecioValorAdicional = contenido.First().ValorAdicional,
                            CamposTipoValorAdicionalDC = contenido.ToList().ConvertAll(
                            campos => new TACampoTipoValorAdicionalDC()
                            {
                                Display = campos.CAMPOADICIONAL,
                                ValorCampo = campos.VALORCAMPOADICIONAL
                            })
                        }));
                }

                admisiongiro.IntentosTransmitir = new ObservableCollection<GIIntentosTransmisionGiroDC>(ObtenerIntentosTransmitir(giro.IdAdmisionGiro.Value));

                return admisiongiro;
            }
        }

        /// <summary>
        /// Obtener el numero de la planilla fecha actual
        /// </summary>
        public long? ObtenerNumeroPlanillaFechaActual(long idAgencia)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaPlanilla = DateTime.Now.Date;

                PlanillaTransmisionGiros_VGIR planilla = contexto.PlanillaTransmisionGiros_VGIR.Where(r => r.FechaPlanilla == fechaPlanilla && r.ADG_IdCentroServicioDestino == idAgencia).FirstOrDefault();
                if (planilla == null)
                    return 0;
                else
                    return planilla.ITG_NoPlanillaTransmision;
            }
        }

        /// <summary>
        /// Insertar los intentos de transmision de un giro
        /// </summary>
        /// <param name="intestosTransmision"></param>
        public void InsertarIntentosTransmisionGiro(GIIntentosTransmisionGiroDC intentosTransmision)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paCrearIntentoTransmiGiro_GIR(intentosTransmision.IdAdminGiro, intentosTransmision.TipoTransmision.ToString(),
                                                        intentosTransmision.Observaciones, DateTime.Now, ControllerContext.Current.Usuario,
                                                        intentosTransmision.NombreContacto, intentosTransmision.IdentificacionContacto,
                                                        intentosTransmision.IdPlanilla, intentosTransmision.AgenciaDestino.IdCentroServicio, (int)intentosTransmision.TipoPlanilla);
            }
        }

        /// <summary>
        /// Actualizar la tabla giros indicando que el giro ya fue transmitido
        /// </summary>
        /// <param name="idAdminGiro"></param>
        public void ActualizarGiroTransmisionGiro(long idAdminGiro)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarGiroTransmiti_GIR(idAdminGiro);
            }
        }

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        public PGComprobantePagoDC PagarGiro(PGPagosGirosDC pagosGiros)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PGComprobantePagoDC comprobante = new PGComprobantePagoDC();

                comprobante.IdComprobantePago = pagosGiros.IdComprobantePago;
                comprobante.FechaGrabacion = DateTime.Now;
                contexto.paInsertarPagoGiro_GIR(pagosGiros.IdAdmisionGiro, pagosGiros.IdComprobantePago,
                                                pagosGiros.PagoAutorizadoPeaton, pagosGiros.PagoAutorizadoEmpresarial,
                                                  pagosGiros.PagoAutomatico, pagosGiros.IdCentroServiciosPagador,
                                                  pagosGiros.NombreCentroServicios, pagosGiros.ValorPagado,
                                                  pagosGiros.ClienteCobrador.Identificacion, pagosGiros.ClienteCobrador.TipoId,
                                                  pagosGiros.ClienteCobrador.Nombre, pagosGiros.ClienteCobrador.Apellido1,
                                                  pagosGiros.ClienteCobrador.Apellido2, pagosGiros.ClienteCobrador.Telefono,
                                                  pagosGiros.ClienteCobrador.Direccion, pagosGiros.ClienteCobrador.Email,
                                                  pagosGiros.ClienteCobrador.Ocupacion.IdOcupacion.ToString(), pagosGiros.Observaciones,
                                                  comprobante.FechaGrabacion,
                                                  ControllerContext.Current.Usuario);

                contexto.paInsertarEstadoGiro_GIR(pagosGiros.IdAdmisionGiro, GIConstantesSolicitudes.ESTADO_GIRO_PAG.ToString(), DateTime.Now, ControllerContext.Current.Usuario);

             

                return comprobante;
            }
        }

        /// <summary>
        /// Obtiene el concepto de cajas para el servicio de pagos de giros
        /// </summary>
        /// <returns></returns>
        public int ObtenerConceptoPagos()
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                int concepto = 0;
                string conceptoPago = contexto.ParametrosGiros_GIR.Where(para => para.PAG_IdParametro == GIConstantesSolicitudes.IDCONCEPTOPAGO).FirstOrDefault().PAG_ValorParametro;
                int.TryParse(conceptoPago, out concepto);
                return concepto;
            }
        }

        /// <summary>
        /// Obtiene las planillas de trasmision para una agencia
        /// </summary>
        /// <returns></returns>
        public List<GIIntentosTransmisionGiroDC> ObtenerPlanillasTrasmisionAgencia(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, long idAgencia)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                filtro.Add("ADG_IdCentroServicioDestino", idAgencia.ToString());
                return contexto.ConsultarContainsPlanillaTransmisionGiros_VGIR(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .OrderByDescending(r => r.ITG_NoPlanillaTransmision)
                  .ToList()
                  .ConvertAll(r => new GIIntentosTransmisionGiroDC()
                  {
                      IdPlanilla = r.ITG_NoPlanillaTransmision,
                      DescripcionTipoPlanilla = r.TPI_Descripcion,
                      FechaPlanilla = r.FechaPlanilla.Value,
                      AgenciaDestino = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = r.ADG_IdCentroServicioDestino,
                          Nombre = r.ADG_NombreCentroServicioDestino
                      },
                  });
            }
        }

        /// <summary>
        /// Método para obtener el correo de el remitente de un giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns></returns>
        public PGPagosGirosDC ObtenerCorreos(PGPagosGirosDC giro)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdmisionGiros_GIR giroRe = contexto.AdmisionGiros_GIR.Where(gir => gir.ADG_IdAdmisionGiro == giro.IdAdmisionGiro).FirstOrDefault();
                if (giroRe != null)
                {
                    giro.EnviaCorreo = giroRe.ADG_NotificarPagoPorEmail != null ? giroRe.ADG_NotificarPagoPorEmail.Value : false;
                    giro.CorreoRemitente = giroRe.ADG_EmailRemitente;
                }
                return giro;
            }
        }

        /// <summary>
        /// Valida si el motivo de la devolucion retorna el flete del giro
        /// </summary>
        /// <param name="numeroGiro"></param>
        /// <returns></returns>
        public bool ValidarMotivoDevRetornaFlete(long numeroGiro)
        {
            using (ModeloPagosManuales contexto = new ModeloPagosManuales(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var sol = contexto.paObtenerSolicitudesDevolucionApro_GIR(numeroGiro).ToList().FirstOrDefault();

                if (sol != null)
                {
                    return sol.MOS_RetornaFlete.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion Metodos
    }
}