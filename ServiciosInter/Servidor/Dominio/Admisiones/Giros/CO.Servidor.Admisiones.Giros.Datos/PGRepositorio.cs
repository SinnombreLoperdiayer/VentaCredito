using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Admisiones.Giros.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Controller.Servidor.Integraciones.CuatroSieteDos;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace CO.Servidor.Admisiones.Giros.Datos
{
    public class PGRepositorio
    {
        

        #region Crear Instancia

        private static readonly PGRepositorio instancia = new PGRepositorio();

        /// <summary>
        /// Retorna la instancia de la clase GIRepositorio
        /// </summary>
        public static PGRepositorio Instancia
        {
            get { return PGRepositorio.instancia; }
        }

        #endregion Crear Instancia

        #region Atributos

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private const string NombreModelo = "ModeloVentaGiros";

        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
     

        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];

        #endregion Atributos

        #region Metodos

        #region Consultas de Giros a Pagar

        /// <summary>
        /// Consultar la cantidad de pagos y la sumatoria total de los mismos
        /// por agencia
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PGTotalPagosDC ConsultarPagosAgencia(long idCentroServicio)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PGTotalPagosDC totalPagos = null;
                paObtenerCantPagosAgencia cantidadPagos = contexto.paObtenerCantPagosAgencia_GIR(idCentroServicio, GIEnumEstadosGiro.ACT.ToString()).FirstOrDefault();
                if (cantidadPagos != null)
                {
                    totalPagos = new PGTotalPagosDC()
                    {
                        CantidadPagos = cantidadPagos.CantidadGiros.Value,
                        SumatoriaPagos = cantidadPagos.TotalGiros
                    };
                }

                return totalPagos;
            }
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiro(long idGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIAdmisionGirosDC admisiongiro = null;
                paObtenerGiroIdGiro giro = contexto.paObtenerGiroIdGiro_GIR(idGiro).FirstOrDefault();

                if (giro.IdAdmisionGiro == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE)));

                if (giro.EstadoGiro == GIEnumEstadosGiro.PAG.ToString())
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO)));

                admisiongiro = new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.EstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen.Value,
                        Nombre = giro.NombreCentroServicioOrigen,

                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = giro.IdCiudadOrigen,
                            Nombre = giro.DescCiudadOrigen
                        },
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino.Value,
                        Nombre = giro.NombreCentroServicioDestino,

                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = giro.IdCiudadDestino,
                            Nombre = giro.DescCiudadDestino
                        },
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro.Value,
                        ValorServicio = giro.ValorPorte.Value
                    },
                    Observaciones = giro.Observaciones
                };
                if (giro.TipoGiro == GIConstantesAdmisionesGiros.GIROPEATONAPEATON)
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
                else if (giro.TipoGiro == GIConstantesAdmisionesGiros.GIROCONVENIOAPEATON)
                {
                    admisiongiro.GirosPeatonConvenio = new GIGirosPeatonConvenioDC()
                    {
                        ClienteContado = new CLClienteContadoDC()
                       {
                           Identificacion = giro.IdDestinatario,
                           TipoId = giro.TipoIdDestinatario,
                           Apellido1 = giro.Apellido1Destinatario,
                           Apellido2 = giro.Apellido2Destinatario,
                           Nombre = giro.NombreDestinatario,
                           Telefono = giro.TelefonoDestinatario,
                           Direccion = giro.DireccionDestinatario,
                           Email = giro.EmailDestinatario,
                           Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario }
                       },
                        ClienteConvenio = new CLClientesDC()
                        {
                            Nit = giro.IdRemitente,
                            RazonSocial = giro.NombreRemitente
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

                return admisiongiro;
            }
        }


        /// <summary>
        /// Consultar el giro por numero de giro y la ciudad de destino
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroNumGiroCiudadDestino(long idGiro, string idCiudadDestino)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIAdmisionGirosDC admisiongiro = null;
                var giro = contexto.paObtenerGiroIdGiroCiudadDestino_GIR(idGiro,idCiudadDestino).FirstOrDefault();

                if (giro.IdAdmisionGiro == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE)));

                if (giro.EstadoGiro == GIEnumEstadosGiro.PAG.ToString())
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO)));

                admisiongiro = new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.EstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen.Value,
                        Nombre = giro.NombreCentroServicioOrigen,

                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = giro.IdCiudadOrigen,
                            Nombre = giro.DescCiudadOrigen
                        },
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino.Value,
                        Nombre = giro.NombreCentroServicioDestino,

                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = giro.IdCiudadDestino,
                            Nombre = giro.DescCiudadDestino
                        },
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro.Value,
                        ValorServicio = giro.ValorPorte.Value
                    },
                    Observaciones = giro.Observaciones
                };
                if (giro.TipoGiro == GIConstantesAdmisionesGiros.GIROPEATONAPEATON)
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
                else if (giro.TipoGiro == GIConstantesAdmisionesGiros.GIROCONVENIOAPEATON)
                {
                    admisiongiro.GirosPeatonConvenio = new GIGirosPeatonConvenioDC()
                    {
                        ClienteContado = new CLClienteContadoDC()
                        {
                            Identificacion = giro.IdDestinatario,
                            TipoId = giro.TipoIdDestinatario,
                            Apellido1 = giro.Apellido1Destinatario,
                            Apellido2 = giro.Apellido2Destinatario,
                            Nombre = giro.NombreDestinatario,
                            Telefono = giro.TelefonoDestinatario,
                            Direccion = giro.DireccionDestinatario,
                            Email = giro.EmailDestinatario,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario }
                        },
                        ClienteConvenio = new CLClientesDC()
                        {
                            Nit = giro.IdRemitente,
                            RazonSocial = giro.NombreRemitente
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

                return admisiongiro;
            }
        }

        /// <summary>
        /// Consultar el giro por numero de giro
        /// y centro Servicio destino
        /// </summary>
        /// <param name="idGiro"></param>
        public GIAdmisionGirosDC ConsultarGiroXNumGiroCentroServicio(long idGiro, long idCentroSvc)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIAdmisionGirosDC admisiongiro = null;
                paObtenerGiroIdGiroCentroServicio_GIR giro = contexto.paObtenerGiroIdGiroCentroServicio_GIR(idGiro, idCentroSvc).FirstOrDefault();

                if (giro.IdAdmisionGiro == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE)));

                if (giro.EstadoGiro == GIEnumEstadosGiro.PAG.ToString())
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO)));

                admisiongiro = new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.EstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen.Value,
                        Nombre = giro.NombreCentroServicioOrigen,

                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = giro.IdCiudadOrigen,
                            Nombre = giro.DescCiudadOrigen
                        },
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino.Value,
                        Nombre = giro.NombreCentroServicioDestino,

                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = giro.IdCiudadDestino,
                            Nombre = giro.DescCiudadDestino
                        },
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro.Value,
                        ValorServicio = giro.ValorPorte.Value
                    },
                    Observaciones = giro.Observaciones
                };
                if (giro.TipoGiro == GIConstantesAdmisionesGiros.GIROPEATONAPEATON)
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
                else if (giro.TipoGiro == GIConstantesAdmisionesGiros.GIROCONVENIOAPEATON)
                {
                    admisiongiro.GirosPeatonConvenio = new GIGirosPeatonConvenioDC()
                    {
                        ClienteContado = new CLClienteContadoDC()
                        {
                            Identificacion = giro.IdDestinatario,
                            TipoId = giro.TipoIdDestinatario,
                            Apellido1 = giro.Apellido1Destinatario,
                            Apellido2 = giro.Apellido2Destinatario,
                            Nombre = giro.NombreDestinatario,
                            Telefono = giro.TelefonoDestinatario,
                            Direccion = giro.DireccionDestinatario,
                            Email = giro.EmailDestinatario,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario }
                        },
                        ClienteConvenio = new CLClientesDC()
                        {
                            Nit = giro.IdRemitente,
                            RazonSocial = giro.NombreRemitente
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

                return admisiongiro;
            }
        }

        public void ObtenerGiroCentroServicioOrigenRacol()
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            { }
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacion(string tipoId, string identificacion)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<PApaObtenerGiroIdentificacio> lstGiros = contexto.paObtenerGiroIdentificacio_GIR(tipoId, identificacion).ToList();

                if (lstGiros == null || lstGiros.Count == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS)));
                }

                return lstGiros.ConvertAll(giro => new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.UltimoEstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen,
                        Nombre = giro.NombreCentroServicioOrigen
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino,
                        Nombre = giro.NombreCentroServicioDestino
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro
                    },
                    Observaciones = giro.Observaciones,
                    GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
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
                          Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario }
                      }
                   }
                });
            }
        }

        /// <summary>
        /// Retorna los giros que tengan un comprobante de pago registrado
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idGiro"></param>
        /// <param name="tipoIdRemitente"></param>
        /// <param name="identificacionRemitente"></param>
        /// <param name="tipoIdDestinatario"></param>
        /// <param name="identificacionDestinatario"></param>
        /// <param name="estadoGiro"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="tipoCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarPagoGirosPeatonConvenioPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //DateTime fechaInicial = DateTime.Now.Date.Add(new TimeSpan(-1, 23, 59, 59, 59));
                DateTime? fechaInicial = DateTime.Now.Date;
                long? idCentroServicioOrigen = null;
                long? idCentroServicioDestino = null;
                long? idRacol = null;

                idCentroServicioDestino = idCentroServicio;

                // Un RACOL puede consultar y reimprimir los giros del mes
                if (string.Compare(ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                    idRacol = idCentroServicio;
                }

                // Un Usuario de Direccion general  puede consultar y reimprimir los giros sin un limite de fecha
                if (string.Compare(ConstantesFramework.USUARIO_GESTION, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                }

                if (string.Compare(ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL, tipoCentroServicio, true) != 0 && string.Compare(ConstantesFramework.USUARIO_GESTION, tipoCentroServicio, true) == 0)
                {
                    idCentroServicioDestino = idCentroServicio;
                }

                return contexto.paObtenerPagoGirosFiltro_GIR(indicePagina, registrosPorPagina, idCentroServicioOrigen, idCentroServicioDestino, fechaInicial, DateTime.Now.AddDays(1), idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, GIConstantesAdmisionesGiros.GIROPEATONACONVENIO, idRacol).ToList().
             ConvertAll<GIAdmisionGirosDC>(
               adm => new GIAdmisionGirosDC()
               {
                   IdAdminGiro = adm.ADG_IdAdmisionGiro,
                   CodVerfiGiro = adm.ADG_DigitoVerificacion,

                   IdGiro = adm.ADG_IdGiro,
                   GirosPeatonConvenio = new GIGirosPeatonConvenioDC()
                   {
                       ClienteContado = new CLClienteContadoDC()
                       {
                           TipoId = adm.ADG_IdTipoIdentificacionRemitente,
                           Identificacion = adm.ADG_IdTipoIdentificacionRemitente,
                           Nombre = adm.ADG_NombreRemitente
                       },
                       ClienteConvenio = new CLClientesDC()
                       {
                           Nit = adm.ADG_IdDestinatario,
                           RazonSocial = adm.ADG_NombreDestinatario
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
                   EstadoGiro = GIEnumEstadosGiro.PAG.ToString()
               });
            }
        }

        public List<GIAdmisionGirosDC> ConsultarPagoGirosPeatonPeatonPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime? fechaInicial = DateTime.Now.Date;
                long? idCentroServicioOrigen = null;
                long? idCentroServicioDestino = null;
                long? idRacol = null;

                // Un RACOL puede consultar y reimprimir los giros sin limite de fecha
                if (string.Compare(ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                    idRacol = idCentroServicio;
                }

                // Un Usuario de Direccion general  puede consultar y reimprimir los giros sin un limite de fecha
                if (string.Compare(ConstantesFramework.USUARIO_GESTION, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                }

                if (string.Compare(ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL, tipoCentroServicio, true) != 0 && string.Compare(ConstantesFramework.USUARIO_GESTION, tipoCentroServicio, true) == 0)
                {
                    idCentroServicioDestino = idCentroServicio;
                }

                return contexto.paObtenerPagoGirosFiltro_GIR(indicePagina, registrosPorPagina, idCentroServicioOrigen, idCentroServicioDestino, fechaInicial, DateTime.Now, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, GIConstantesAdmisionesGiros.GIROPEATONAPEATON, idRacol).ToList().
             ConvertAll<GIAdmisionGirosDC>(
               adm => new GIAdmisionGirosDC()
               {
                   IdAdminGiro = adm.ADG_IdAdmisionGiro,
                   CodVerfiGiro = adm.ADG_DigitoVerificacion,
                   TelefonoDestinatario = adm.ADG_TelefonoDestinatario,
                   TelefonoRemitente = adm.ADG_TelefonoRemitente,
                   IdGiro = adm.ADG_IdGiro,
                   GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                   {
                       ClienteRemitente = new CLClienteContadoDC()
                       {
                           TipoId = adm.ADG_IdTipoIdentificacionRemitente,
                           Identificacion = adm.ADG_IdRemitente,
                           Nombre = adm.ADG_NombreRemitente,
                           Telefono = adm.ADG_TelefonoRemitente
                       },
                       ClienteDestinatario = new CLClienteContadoDC()
                       {
                           TipoId = adm.ADG_IdTipoIdentificacionDestinatario,
                           Identificacion = adm.ADG_IdDestinatario,
                           Nombre = adm.ADG_NombreDestinatario,
                           Telefono = adm.ADG_TelefonoDestinatario
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
                   EstadoGiro = GIEnumEstadosGiro.PAG.ToString()
               });
            }
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCiudad(string tipoId, string identificacion, string idCiudadConsulta)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                List<GiroIdentificacionCiudad_GIR> lstGiros = contexto.paObtenerGiroIdentificacionCiudad_GIR(tipoId, identificacion, idCiudadConsulta).ToList();

                if (lstGiros == null || lstGiros.Count == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS)));
                }

                return lstGiros.ConvertAll(giro => new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.UltimoEstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen,
                        Nombre = giro.NombreCentroServicioOrigen
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino,
                        Nombre = giro.NombreCentroServicioDestino
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro
                    },
                    Observaciones = giro.Observaciones,
                    GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
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
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario }
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Consultar el giro por tipo y numero de identificacion del destinatario
        /// y por la ciudad de donde se realiza la consulta
        /// </summary>
        /// <param name="tipoId"></param>
        /// <param name="identificacion"></param>
        /// <param name="idCiudadConsulta"></param>
        /// <returns>Lista de Giros por la ciudad consultada</returns>
        public List<GIAdmisionGirosDC> ConsultarGiroXIdentificacionCentroServiciosDestino(string tipoId, string identificacion, long idCentroServicio)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var lstGiros = contexto.paObtenerGiroIdentificacionCentroServiciosDestino_GIR(tipoId, identificacion, idCentroServicio).ToList();

                if (lstGiros == null || lstGiros.Count == 0)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_ENCONTRO_GIROS)));
                }

                return lstGiros.ConvertAll(giro => new GIAdmisionGirosDC()
                {
                    IdGiro = giro.IdGiro,
                    IdAdminGiro = giro.IdAdmisionGiro,
                    EstadoGiro = giro.UltimoEstadoGiro,
                    AgenciaOrigen = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioOrigen,
                        Nombre = giro.NombreCentroServicioOrigen
                    },
                    AgenciaDestino = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = giro.IdCentroServicioDestino,
                        Nombre = giro.NombreCentroServicioDestino
                    },
                    Precio = new TAPrecioDC()
                    {
                        ValorGiro = giro.ValorGiro
                    },
                    Observaciones = giro.Observaciones,
                    GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
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
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = giro.OcupacionDestinatario }
                        }
                    }
                });
            }
        }



        /// <summary>
        /// Obtiene el concepto de cajas para el servicio de pagos de giros
        /// </summary>
        /// <returns></returns>
        public int ObtenerConceptoPagos()
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string conceptoPago = contexto.ParametrosGiros_GIR.Where(para => para.PAG_IdParametro == GIConstantesAdmisionesGiros.IDCONCEPTOPAGO).FirstOrDefault().PAG_ValorParametro;
                return int.Parse(conceptoPago);
            }
        }


     

        /// <summary>
        /// Consulta la informacion de un pago
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public PGPagosGirosDC ConsultarInformacionPago(long idAdmisionGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerGiroInfoPagoRS_GIR infoPago = contexto.paObtenerGiroInfoPago_GIR(idAdmisionGiro).FirstOrDefault();

                if (infoPago == null)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_PAGO_NO_EXISTE.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_PAGO_NO_EXISTE)));

                return new PGPagosGirosDC()
                {
                    IdCentroServiciosPagador = infoPago.PAG_IdCentroServiciosPagador,
                    NombreCentroServicios = infoPago.PAG_NombreCentroServicios,
                    UsuarioPago = infoPago.PAG_CreadoPor,
                    FechaHoraPago = infoPago.PAG_FechaGrabacion,
                    IdComprobantePago = infoPago.PAG_IdComprobantePago,
                    PagoAutorizadoPeaton = infoPago.PAG_PagoAutorizadoPeaton,
                    PagoAutorizadoEmpresarial = infoPago.PAG_PagoAutorizadoEmpresarial,
                    PagoAutomatico = infoPago.PAG_PagoAutomatico,
                    Observaciones = infoPago.PAG_Observaciones,
                    ClienteCobrador = new Servicios.ContratoDatos.Clientes.CLClienteContadoDC()
                    {
                        TipoId = infoPago.PAG_TipoIdCobrador,
                        Identificacion = infoPago.PAG_IdCobrador,
                        Nombre = infoPago.PAG_NombreCobrador,
                        Apellido1 = infoPago.PAG_Apellido1Cobrador,
                        Apellido2 = infoPago.PAG_Apellido2Cobrador,
                        Telefono = infoPago.PAG_TelefonoCobrador,
                        Direccion = infoPago.PAG_DireccionCobrador,
                        Email = infoPago.PAG_EmailCobrador,
                        Ocupacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PAOcupacionDC()
                        {
                            DescripcionOcupacion = infoPago.PAG_OcupacionCobrador
                        }
                    },
                    LocalidadPagador = new PALocalidadDC()
                    {
                        Nombre = infoPago.LOC_Nombre,
                        IdLocalidad = infoPago.LOC_IdLocalidad,
                        CodigoPostal = infoPago.LOC_CodigoPostal,
                        NombreAncestroSGrado = infoPago.LOC_NombreSegundo
                    }
                };
            }
        }

        /// <summary>
        /// Obtiene el identificador de pagos giro
        /// </summary>
        /// <param name="idAdmisionGiro">Identificador admisión</param>
        /// <returns>Identificador</returns>
        public long ObtenerIdentificadorPagosGiro(long idAdmisionGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PagoGiros_GIR consulta = contexto.PagoGiros_GIR.Where(w => w.PAG_IdAdmisionGiro == idAdmisionGiro).FirstOrDefault();

                if (consulta == null)
                    return 0;
                else
                    return consulta.PAG_IdComprobantePago;
            }
        }

        /// <summary>
        /// Obtiene la informacion de un giro por el numero de comprobante de pago
        /// </summary>
        /// <param name="idComprobantePago"></param>
        /// <returns></returns>
        public PGPagosGirosDC ObtenerGiroPorComprobantePago(long idComprobantePago)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PGPagosGirosDC pagoGiro = null;
                GIpaObtenerPagoGiroPorCompro pago = contexto.paObtenerPagoGiroPorCompro_GIR(idComprobantePago).FirstOrDefault();
                if (pago != null)
                {
                    pagoGiro = new PGPagosGirosDC()
                             {
                                 IdAdmisionGiro = pago.PAG_IdAdmisionGiro,
                                 IdGiro = pago.ADG_IdGiro,
                                 IdComprobantePago = pago.PAG_IdComprobantePago
                             };
                }

                return pagoGiro;
            }
        }

        /// <summary>
        /// Método para obtener el correo de el remitente de un giro
        /// </summary>
        /// <param name="idGiro"></param>
        /// <returns></returns>
        public PGPagosGirosDC ObtenerCorreos(PGPagosGirosDC giro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AdmisionGiros_GIR giroRe = contexto.AdmisionGiros_GIR.Where(gir => gir.ADG_IdAdmisionGiro == giro.IdAdmisionGiro).FirstOrDefault();
                if (giroRe != null)
                {
                    giro.EnviaCorreo = giroRe.ADG_NotificarPagoPorEmail != null ? giroRe.ADG_NotificarPagoPorEmail.Value : false;
                    giro.CorreoRemitente = giroRe.ADG_EmailRemitente;
                    return giro;
                }
                else
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_EXISTE)));
            }
        }

        #endregion Consultas de Giros a Pagar

        #region Realizar el proceso de pago

        /// <summary>
        /// Valida que el giro este disponible para pagar, no tenga solicitudes
        /// </summary>
        public void ValidacionPago(long idAdmisiongiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PGpaObtenerAdmisionGiro_GIR giro = contexto.paObtenerAdmisionGiro_GIR(idAdmisiongiro).FirstOrDefault();
                if (giro != null && giro.ESG_Estado == GIEnumEstadosGiro.PAG.ToString())
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_GIRO_NO_ACTIVO)));

                //Valida si el giro esta transmitido
                //if (giro != null && giro.ADG_Transmitido == true)
                //   throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_GIRO_TRANSMITIDO.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_GIRO_TRANSMITIDO)));

                paObtenerGiroSinSolicitud solicitudes = contexto.paObtenerGiroSinSolicitud_GIR(giro.ADG_IdGiro).FirstOrDefault();

                if (solicitudes != null && solicitudes.ADG_IdGiro == 0)
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_SOLICITUDES_ACTIVAS.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_NO_SE_PUEDE_PAGAR_SOLICITUDES_ACTIVAS)));
            }
        }

        /// <summary>
        /// Consulta si el pago se realizo exitosamente
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        public void ConsultarPago(long idAdmisiongiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIpaObtenerGiroIdAdminGiro giro = contexto.paObtenerGiroIdAdminGiro_GIR(idAdmisiongiro).FirstOrDefault();
                if (giro == null || giro.ADG_IdGiro == 0 || giro.ESG_Estado != GIEnumEstadosGiro.PAG.ToString())
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.GIROS, EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_SE_PUDO_PAGAR.ToString(), MensajesAdmisionesGiros.CargarMensaje(EnumTipoErrorAdmisionesGiros.EX_GIRO_NO_SE_PUDO_PAGAR)));
            }
        }

        /// <summary>
        /// Realiza el pago del giro
        /// </summary>
        public PGComprobantePagoDC PagarGiro(PGPagosGirosDC pagosGiros, long numeroPago)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                PGComprobantePagoDC comprobante = new PGComprobantePagoDC();

                comprobante.IdComprobantePago = numeroPago;
                comprobante.FechaGrabacion = DateTime.Now;
                contexto.paInsertarPagoGiro_GIR(pagosGiros.IdAdmisionGiro,
                  numeroPago,
                  pagosGiros.PagoAutorizadoPeaton,
                  pagosGiros.PagoAutorizadoEmpresarial,
                  pagosGiros.PagoAutomatico,
                  pagosGiros.IdCentroServiciosPagador,
                  pagosGiros.NombreCentroServicios,
                  pagosGiros.ValorPagado,
                  pagosGiros.ClienteCobrador.Identificacion,
                  pagosGiros.ClienteCobrador.TipoId,
                  pagosGiros.ClienteCobrador.Nombre,
                  pagosGiros.ClienteCobrador.Apellido1,
                  pagosGiros.ClienteCobrador.Apellido2,
                  pagosGiros.ClienteCobrador.Telefono,
                  pagosGiros.ClienteCobrador.Direccion,
                  pagosGiros.ClienteCobrador.Email,
                  pagosGiros.ClienteCobrador.Ocupacion.IdOcupacion.ToString(),
                  pagosGiros.Observaciones,
                  comprobante.FechaGrabacion,
                  ControllerContext.Current.Usuario);

                contexto.paInsertarEstadoGiro_GIR(pagosGiros.IdAdmisionGiro, GIEnumEstadosGiro.PAG.ToString(), DateTime.Now, ControllerContext.Current.Usuario);

                //Integracion472.Instancia.IntegrarCuatroSieteDosPagoDevAnul(pagosGiros.IdAdmisionGiro, "1",DateTime.Now);
              // TODO: RON Descomentariar
                return comprobante;
            }
        }

        
        #region Almacenar Archivos



        /// <summary>
        /// Almacenar el documento de Certificado Empresarial
        /// </summary>
        ///<param name="pagosGiros">informacion del pago</param>
        public long AlmacenarArchivoPagoGiroPago(PGPagosGirosDC pagosGiros , string archivo , GIEnumTipoDocumentoPagoGiro tipoDoc)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenArchivoGiro_GIR almacenArchivo = new AlmacenArchivoGiro_GIR()
                {
                    AGI_RutaArchivo = archivo,
                    AGI_NombreAdjunto = tipoDoc.ToString(),
                    AGI_CreadoPor = ControllerContext.Current.Usuario,
                    AGI_FechaGrabacion = DateTime.Now
                };
                contexto.AlmacenArchivoGiro_GIR.Add(almacenArchivo);
                contexto.SaveChanges();

                ArchivosPagoGiro_GIR archivoPag = new ArchivosPagoGiro_GIR()
                {
                    APG_IdAdmisionGiro = pagosGiros.IdAdmisionGiro,
                    APG_IdArchivo = almacenArchivo.AGI_IdArchivo,
                    APG_CreadoPor = ControllerContext.Current.Usuario,
                    APG_IdTipoDocumentoPagoGiro = (short)tipoDoc,
                    APG_FechaGrabacion = DateTime.Now
                };
                contexto.ArchivosPagoGiro_GIR.Add(archivoPag);
                contexto.SaveChanges();
                return almacenArchivo.AGI_IdArchivo;
            }
        }


        /// <summary>
        /// obtiene el la imagen del documento adjunto al pago de
        /// un giro en el formato de Comprobante de Pago
        /// </summary>
        /// <param name="tipoDocPago">Enumeracion del tipo Doc</param>
        /// <param name="idAdmisionGiro">Id de Admision del Giro</param>
        /// <returns>Imagen</returns>
        public string ObtenerImagenDocumentoDestinatarioPagoGiro(GIEnumTipoDocumentoPagoGiro tipoDocPago, long idAdmisionGiro)
        {
            string rutaArchivo = "";

            string query = @"SELECT * FROM ArchivosPagoGiro_GIR INNER JOIN AlmacenArchivoGiro_GIR ON APG_IdArchivo = AGI_IdArchivo " +
                "WHERE APG_IdAdmisionGiro = " + idAdmisionGiro + " AND APG_IdTipoDocumentoPagoGiro = " + (int)tipoDocPago ;
            using (SqlConnection sqlConn = new SqlConnection(conexionStringController))
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand(query, sqlConn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count != 0)
                    rutaArchivo = Convert.ToString((dt.Rows[0]["AGI_RutaArchivo"]));
                sqlConn.Close();

                if (!string.IsNullOrEmpty(rutaArchivo))
                {
                    Bitmap imagen = new Bitmap(rutaArchivo);
                    MemoryStream memoryStream = new MemoryStream();
                    imagen.Save(memoryStream, ImageFormat.Png);
                    byte[] bitmapBytes = memoryStream.GetBuffer();
                    return Convert.ToBase64String(bitmapBytes);
                }
                else
                    return String.Empty;
            }
        }


 

        #endregion Almacenar Archivos

        /// <summary>
        /// Valida que la sumatoria de los valores recibidos por el destinatario
        /// no superan un monto en un dial
        /// </summary>
        /// <param name="giro">true: obliga a declarar</param>
        public bool ValidarDeclaracionFondos(decimal valorAcumulado)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string declaracion = GIConstantesAdmisionesGiros.VALORMAXIMODECLARACIONFONDOSDESTINATARIO;
                ParametrosGiros_GIR parametros = contexto.ParametrosGiros_GIR.FirstOrDefault(par => par.PAG_IdParametro == declaracion);

                if (parametros == null)
                {
                    return false;
                }

                string valorParametro = parametros.PAG_ValorParametro;
                decimal valorMaximo;
                bool obligaDeclaracion = false;

                if (decimal.TryParse(valorParametro, out valorMaximo))
                    if (valorAcumulado > valorMaximo)
                        obligaDeclaracion = true;
                    else
                        obligaDeclaracion = false;

                return obligaDeclaracion;
            }
        }

        #endregion Realizar el proceso de pago

        #endregion Metodos
    }
}