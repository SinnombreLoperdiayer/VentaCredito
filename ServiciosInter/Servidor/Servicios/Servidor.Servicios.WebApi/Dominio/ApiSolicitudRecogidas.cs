using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using CO.Servidor.Servicios.Implementacion.Rutas;
using CO.Servidor.Servicios.WebApi.Comun;
using CO.Servidor.Servicios.WebApi.Controllers;
using CO.Servidor.Servicios.WebApi.ModeloResponse.SolicitudRecogidasApp;
using CO.Servidor.Servicios.WebApi.ModelosRequest.OperacionUrbana;
using CO.Servidor.Servicios.WebApi.ModelosRequest.SolicitudRecogidasApp;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CO.Servidor.Servicios.WebApi.Dominio
{
    public class ApiSolicitudRecogidas : ApiDominioBase
    {
        private static readonly ApiSolicitudRecogidas instancia = (ApiSolicitudRecogidas)FabricaInterceptorApi.GetProxy(new ApiSolicitudRecogidas(), COConstantesModulos.MODULO_OPERACION_URBANA);

        public static ApiSolicitudRecogidas Instancia
        {
            get { return ApiSolicitudRecogidas.instancia; }
        }

        private ApiSolicitudRecogidas()
        {
        }


        /// <summary>
        /// Obtiene la lista de precios plena y vigente
        /// </summary>
        /// <returns></returns>
        public int ObtenerIdListaPreciosPlenaVigente()
        {
            int idLista = 0;
            var lista = FabricaServicios.ServicioTarifas.ObtenerListasPrecio().FirstOrDefault();

            if (lista != null)
                idLista = lista.IdListaPrecio;

            return idLista;
        }


        /// <summary>
        /// Retorna las localidades que no son paises ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        public List<ResponseGenericoApp> ObtenerLocalidadesColombia()
        {
            return FabricaServicios.ServicioParametros.ObtenerLocalidadesNoPaisNoDepartamentoColombia().ToList().
                ConvertAll<ResponseGenericoApp>(c =>
                    new ResponseGenericoApp()
                    {
                        label = c.NombreCompleto,
                        value = c.IdLocalidad
                    });
        }
        /// <summary>
        /// Obtiene los tipos de envio
        /// </summary>
        /// <returns></returns>
        public List<ResponseGenericoApp> ObtenerTipoEnvio()
        {
            return FabricaServicios.ServicioTarifas.ObtenerTipoEnvios().ConvertAll<ResponseGenericoApp>(c =>
                      new ResponseGenericoApp()
                      {
                          label = c.Nombre,
                          value = c.IdTipoEnvio.ToString()
                      });
        }


        /// <summary>
        /// Obtiene los tipos de entrega
        /// </summary>
        /// <returns></returns>
        public List<ResponseGenericoApp> ObtenerTipoEntrega()
        {
            return FabricaServicios.ServicioMensajeria.ObtenerTiposEntrega().ToList().ConvertAll<ResponseGenericoApp>(c =>
                      new ResponseGenericoApp()
                      {
                          label = c.Descripcion,
                          value = c.Id.ToString()
                      });
        }

        /// <summary>
        /// Obtiene todas las agencias y puntos en el sistema
        /// </summary>
        /// <returns></returns>
        public List<CentroServiciosApp> ObtenerPuntosAgenciasActivos()
        {
            return FabricaServicios.ServicioCentroServicios.ObtenerTodosAgenciaColPuntosActivos().ConvertAll<CentroServiciosApp>
                (s => new CentroServiciosApp()
                {
                    CES_Estado = s.Estado,
                    CES_Tipo = s.Tipo,
                    direccion = s.Direccion,
                    ID = s.IdCentroServicio,
                    idMunicipio = s.IdMunicipio,
                    latitud = s.Latitud.Value,
                    longitud = s.Longitud.Value,
                    nombre = s.Nombre,
                    telefono1 = s.Telefono1,
                    telefono2 = s.Telefono2,
                    tipoPropiedad = s.IdTipoPropiedad
                });

        }


        /// <summary>
        /// Obtiene la ubicacion de una guia para la app del cliente
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public UbicacionGuiaResponse ObtenerUbicacionGuia(long numeroGuia)
        {
            var guia = FabricaServicios.ServicioMensajeria.ObtenerUbicacionGuia(numeroGuia);

            return new UbicacionGuiaResponse()
            {
                ADI_NumeroGuiaDHL = guia.NumeroGuiaDHL,
                ADM_FechaAdmision = guia.FechaAdmision,
                ADM_FechaEntrega = guia.FechaEntrega,
                ADM_IdServicio = guia.IdServicio,
                ADM_NombreCiudadDestino = guia.NombreCiudadDestino,
                ADM_NombreCiudadOrigen = guia.NombreCiudadOrigen,
                EGT_DescripcionEstado = guia.DescripcionEstadoGuiaTraza,
                EGT_NombreLocalidad = guia.NombreLocalidadGuiatraza,
                EGT_NumeroGuia = guia.NumeroGuia,
                MOG_Descripcion = guia.DescripcionMotivoGuia,
                ADM_NombreRemitente = guia.NombreRemitente,
                ADM_NombreDestinatario = guia.NombreDestinatario,
                ADM_DireccionRemitente = guia.DireccionRemitente,
                ADM_DireccionDestinatario = guia.DireccionDestinatario,
                ADM_TelefonoRemitente = guia.TelefonoRemitente,
                ADM_TelefonoDestinatario = guia.TelefonoDestinatario,
                ADM_NombreServicio = guia.NombreServicio,
                ADM_NombreTipoEnvio = guia.NombreTipoEnvio,
                ADM_Peso = guia.Peso,
                ADM_ValorAdmision = guia.ValorAdmision,
                ADM_ValorPrimaSeguro = guia.ValorPrimaSeguro,
                ADM_ValorAdicionales = guia.ValorAdicionales,
                ADM_ValorTotal = guia.ValorTotal,
                ADM_DiceContener = guia.DiceContener,
                FOP_Descripcion = guia.DescripcionFormaPago
            };
        }


        /// <summary>
        /// Guarda una solicitud de recogida de peaton
        /// </summary>
        /// <param name="recogida"></param>
        public long GuardarSolicitudRecogidaPeaton(SolicitudRecogidaRequest recogida)
        {
            DateTime fechaRecogida = new DateTime(recogida.AnioRecogida, recogida.MesRecogida, recogida.DiaRecogida, recogida.HoraRecogida, recogida.MinutoRecogida, 0);

            OURecogidasDC recogidaDC = new OURecogidasDC()
            {

                CantidadEnvios = recogida.CantidadEnvios,
                FechaRecogida = fechaRecogida,
                PersonaSolicita = recogida.NombrePersona,
                Contacto = recogida.NombrePersona,
                NombreCliente = recogida.NombrePersona,
                PesoAproximado = recogida.PesoAproximado,
                Observaciones = recogida.Observaciones,
                PersonaRecepcionoRecogida = "WebApi",
                LongitudRecogida = recogida.Longitud,
                LatitudRecogida = recogida.Latitud,
                ComplementoDireccion = recogida.RecogidaPeaton.ComplementoDireccionCliente,
                LocalidadRecogida = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                {
                    IdLocalidad = recogida.IdLocalidad
                },
                Fotografias = recogida.Fotografias,
                Direccion = recogida.RecogidaPeaton.DireccionCliente,
                RecogidaPeaton = new OURecogidaPeatonDC()
                {
                    TipoIdentificacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PATipoIdentificacion()
                    {
                        IdTipoIdentificacion = recogida.RecogidaPeaton.TipoIdentificacion
                    },
                    DocumentoCliente = recogida.RecogidaPeaton.DocumentoCliente,
                    NombreCliente = recogida.RecogidaPeaton.NombreCliente,
                    DireccionCliente = recogida.RecogidaPeaton.DireccionCliente,
                    TelefonoCliente = recogida.RecogidaPeaton.TelefonoCliente,
                    Email = recogida.RecogidaPeaton.Email,
                    Celular = recogida.RecogidaPeaton.Celular,
                    EnviosRecogida = recogida.RecogidaPeaton.TiposEnvio.ConvertAll<OUEnviosRecogidaPeatonDC>(r =>
                        new OUEnviosRecogidaPeatonDC()
                        {
                            MunicipioDestino = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                            {
                                IdLocalidad = recogida.IdLocalidad
                            },
                            TipoEnvio = new ContratoDatos.Tarifas.TATipoEnvio()
                            {
                                IdTipoEnvio = r.IdTipoEnvio,
                                Descripcion = r.Descripcion,
                                Nombre = r.Descripcion
                            },
                            CantidadEnvios = r.CantidadEnvios,
                            PesoAproximado = r.PesoAproximado

                        })
                },
                TipoOrigenRecogida = (OUEnumTipoOrigenRecogida)Enum.Parse(typeof(OUEnumTipoOrigenRecogida), recogida.TipoOrigen)

            };

            using (TransactionScope trans = new TransactionScope())
            {

                long idRecogida = FabricaServicios.ServicioOperacionUrbana.GuardaSolicitudClientePeaton(recogidaDC);

                ///La la recogida es de un cliente APP se debe asociar el dispositivo a la recogida para que se pueda notificar al cliente de los datos del mensajero
                if (recogidaDC.TipoOrigenRecogida == OUEnumTipoOrigenRecogida.APP)
                {
                    SolicitudRecogidaPushMovilRequest solicitud = new SolicitudRecogidaPushMovilRequest()
                    {
                        IdLocalidad = recogida.IdLocalidad,
                        SistemaOperativo = recogida.SistemaOperativo,
                        TokenDispositivo = recogida.TokenDispositivo,
                        IdRecogida = idRecogida
                    };
                    ApiOperacionUrbana.Instancia.RegistrarSolicitudRecogidaMovil(solicitud);
                }

                trans.Complete();
                //Test.ReportarCreacionRecogida();            
                //RecogidasSignalRController.ReportarCreacionRecogida();
                return idRecogida;
            }

        }

        public void ActualizaSolicitudClientePeaton(ActualizarRecogidaRequest recogida)
        {
            OURecogidasDC recogidaCP = new OURecogidasDC()
            {
                IdRecogida = recogida.idRecogida,
                CantidadEnvios = recogida.CantidadEnvios,
                FechaRecogida = recogida.FechaRecogida,
                PesoAproximado = recogida.PesoAproximado,
                Cliente = new ContratoDatos.Clientes.CLClientesDC()
                {
                    Nit = recogida.Documento,
                    NombreRepresentanteLegal = recogida.Nombre,
                    Telefono = recogida.Telefono
                },
                Contacto = recogida.Celular,
                Email = recogida.Email,
                LongitudRecogida = recogida.Longitud,
                LatitudRecogida = recogida.Latitud,
                ComplementoDireccion = recogida.Complemento,
                Direccion = recogida.Direccion,
                EstadoRecogida = new OUEstadosSolicitudRecogidaDC() { IdEstado = recogida.IdEstado }
            };

            FabricaServicios.ServicioOperacionUrbana.ActualizaSolicitudClientePeaton(recogidaCP);
        }

        /// <summary>
        /// Obtiene El valor comercial dependiento del peso
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public ValorComercialResponse ConsultarValorComercialPeso(int peso)
        {

            return new ValorComercialResponse()
            {
                valorComercial = FabricaServicios.ServicioTarifas.ConsultarValorComercialPeso(peso),
                fechaServidor = DateTime.Now
            };

        }

        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<ServicioPesoResponse> ConsultarServiciosPesosMinimoxMaximos()
        {
            return FabricaServicios.ServicioTarifas.ConsultarServiciosPesosMinimoxMaximos().ConvertAll<ServicioPesoResponse>(s =>
                new ServicioPesoResponse()
                {
                    SER_IdServicio = s.IdServicio,
                    SME_PesoMaximo = s.PesoMaximo,
                    SME_PesoMínimo = s.PesoMinimo
                });
        }


        /// <summary>
        /// Obtiene la imagen de una guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        public string ObtenerImagenGuia(long numeroGuia)
        {

            LIArchivoGuiaMensajeriaDC archivo = new LIArchivoGuiaMensajeriaDC()
            {
                ValorDecodificado = numeroGuia.ToString()
            };
            

            string Sftp;
            string SPassword;
            string SUsuario;
            int cont = 0;
            string imagen="";

            if (numeroGuia.ToString().Length >12 && numeroGuia.ToString().StartsWith("8"))
                {
                Sftp = "ftpDigSispostal";
                SPassword = "passFtpDigSispostal";
                SUsuario = "UserFtpDigSispostal";
                    archivo = FabricaServicios.ServicioLogisticaInversa.ObtenerArchivoGuiaSispostal(archivo);
                    imagen = TraerImagenFtp(archivo,numeroGuia,Sftp,SUsuario,SPassword,cont);
            }
                else
                {
                Sftp = "ftpDigitalizacion";
                SPassword = "passFtpDigitalizaci";
                SUsuario = "UserFtpDigitalizaci";

                archivo = FabricaServicios.ServicioLogisticaInversa.ObtenerArchivoGuiaFS(archivo);
                     
                      
                    if (archivo != null)
                    {
                        foreach (var a in archivo.RutaServidor.ToList())
                        {
                            int convertir = 0;
                            if (int.TryParse(a.ToString(), out convertir))
                            {
                                break;
                            }
                            else
                                cont++;
                        }
                    imagen = TraerImagenFtp(archivo, numeroGuia, Sftp, SUsuario, SPassword, cont);
                }
                    
                }

            return imagen;
           
        }

        private string TraerImagenFtp(LIArchivoGuiaMensajeriaDC archivo,long numeroGuia,string SFtp,string SUsuario,string SPassword,int cont )
        {

            string ftp;
            string pass;
            string user;
            Uri Uriftp = null;

            ftp = FabricaServicios.ServicioParametros.ConsultarParametrosFramework(SFtp).ValorParametro;
            pass = FabricaServicios.ServicioParametros.ConsultarParametrosFramework(SPassword).ValorParametro;
            user = FabricaServicios.ServicioParametros.ConsultarParametrosFramework(SUsuario).ValorParametro;

            if (archivo != null)
            {

                if (numeroGuia.ToString().Length >12 && numeroGuia.ToString().StartsWith("8"))
                {


                    Uriftp = new Uri(ftp + "/" + archivo.RutaServidor.Replace(@"\", "/"));

                }
                else
                {

                    Uriftp = new Uri(ftp + "/" + archivo.RutaServidor.Substring(cont, archivo.RutaServidor.Length - cont).Replace(@"\", "/"));

                }

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(Uriftp);
                ftpRequest.Credentials = new NetworkCredential(user, pass);
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();
                Stream stream = ftpResponse.GetResponseStream();

                byte[] imagenArray = Util.ReadToEnd(stream);
                stream.Close();

                string imgString = Convert.ToBase64String(imagenArray);

                return imgString;


            }
            else
                return "";
        }


       

        /// <summary>
        /// Obtiene los tipos de envio para la app cliente de recogidas
        /// </summary>
        /// <returns></returns>
        public List<TipoEnvioApp> ObtenerTiposEnvio()
        {
            return FabricaServicios.ServicioTarifas.ObtenerTipoEnvios().ConvertAll<TipoEnvioApp>(t =>
                new TipoEnvioApp()
                {
                    TEN_Descripcion = t.Descripcion,
                    TEN_IdTipoEnvio = t.IdTipoEnvio,
                    TEN_PesoMaximo = t.PesoMaximo,
                    TEN_PesoMinimo = t.PesoMinimo
                });
        }




        /// <summary>
        /// consulta direcciones historico recogidas por documento
        /// </summary>
        /// <param name="SolicitudRecogidaPeaton"></param>
        public List<SolicitudRecogidaRequest> ObtenerDireccionesPeaton(SolicitudRecogidaPeaton Peaton)
        {
            OURecogidaPeatonDC PeatonSer = new OURecogidaPeatonDC();
            PeatonSer.TipoIdentificacion = new PATipoIdentificacion()
                                            {
                                                IdTipoIdentificacion = Peaton.TipoIdentificacion
                                            };
            PeatonSer.DocumentoCliente = Peaton.DocumentoCliente;
            List<OURecogidasDC> Recogidas = FabricaServicios.ServicioOperacionUrbana.ObtenerDireccionesPeaton(PeatonSer);

            List<SolicitudRecogidaRequest> DireccionRecogidas = new List<SolicitudRecogidaRequest>();
            foreach (var item in Recogidas.ToList())
            {
                DireccionRecogidas.Add(new SolicitudRecogidaRequest
                    {
                        Latitud = item.LatitudRecogida,
                        Longitud = item.LongitudRecogida,
                        RecogidaPeaton = new SolicitudRecogidaPeaton()
                        {
                            DireccionCliente = item.Direccion,
                            ComplementoDireccionCliente = item.ComplementoDireccion
                        }
                    });
            }
            return DireccionRecogidas;

        }

        /// <summary>
        /// Metodo para obtener informacion recogida usuario externo
        /// </summary>
        /// <param name="identificacion"></param>
        /// <returns></returns>
        public SolicitudRecogidaResponse ObtenerInformacionRecogidaUsuarioExterno(string nomUsuario)
        {
            OURecogidasDC recogida = FabricaServicios.ServicioOperacionUrbana.ObtenerInformacionRecogidaUsuarioExterno(nomUsuario);
            if (recogida != null)
            {
                SolicitudRecogidaResponse solicitudRecogida = new SolicitudRecogidaResponse()
                {
                    NombreCompleto = recogida.NombreCliente,
                    NumeroIdentificacion = recogida.RecogidaPeaton.DocumentoCliente,
                    Telefono = recogida.RecogidaPeaton.TelefonoCliente,
                    IdMunicipio = recogida.LocalidadRecogida.IdLocalidad,
                    Direccion = recogida.RecogidaPeaton.DireccionCliente,
                    Email = recogida.RecogidaPeaton.Email,
                    ComplementoDireccion = recogida.ComplementoDireccion,
                    Latitud = recogida.LatitudRecogida,
                    Longitud = recogida.LongitudRecogida,
                    NombreMunicipio = recogida.LocalidadRecogida.Nombre,
                    Celular = recogida.RecogidaPeaton.Celular
                };
                return solicitudRecogida;
            }
            else
            {
                return new SolicitudRecogidaResponse();
            }
        }

        /// <summary>
        /// Metodo para calificar la solicitud de recogida
        /// </summary>
        /// <param name="idSolicitudRecogida"></param>
        /// <param name="calificacion"></param>
        /// <param name="observaciones"></param>
        public void CalificarSolicitudRecogida(long idSolicitudRecogida, int calificacion, string observaciones)
        {
            FabricaServicios.ServicioOperacionUrbana.CalificarSolicitudRecogida(idSolicitudRecogida, calificacion, observaciones);
        }

        //PRUEBA RUTAS
        #region MetodosCWeb


        private static readonly RURutasSvc rutasCWeb = new RURutasSvc();

        /// <summary>1
        /// Obtiene información de la ruta y Coordenadas de centros de servicio de la ruta
        /// </summary>
        /// <returns></returns>

        public List<RURutaICWeb> ObtenerRuta()
        {
            return FabricaServicios.RutasCWeb.ObtenerRuta();
        }
        /// <summary>
        /// obtiene centros de servicios de la ruta indicada
        /// </summary>
        /// <param name="idRuta"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RURutaCWebDetalleCentrosServicios> ObtenerRutaDetalleCentroServiciosRuta(int idRuta, int id)
        {
            return FabricaServicios.RutasCWeb.ObtenerRutaDetalleCentroServiciosRuta(idRuta, id);
        }
        /// <summary>
        /// elimina punto de ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void EliminarPtoRuta(PtoRuta datosPunto)
        {
            FabricaServicios.RutasCWeb.EliminarPtoRuta(datosPunto);
        }
        /// <summary>
        /// agrega un punto a la ruta indicada
        /// </summary>
        /// <param name="datosPunto"></param>
        public void AgregarPtoRuta(PtoRuta datosPunto)
        {
            FabricaServicios.RutasCWeb.AgregarPtoRuta(datosPunto);
        }
        //public RURutaICWebDetalle ObtenerRutaDetalle(int idRuta)
        //{
        //    return FabricaServicios.RutasCWeb.obtenerRutaDetalleCentroServiciosRuta(idRuta);
        //}
        /// <summary>
        /// crear punto
        /// </summary>
        /// <param name="datosPunto"></param>
        public void CrearPunto(PtoRuta datosPunto)
        {
            FabricaServicios.RutasCWeb.CrearPunto(datosPunto);
        }
        #endregion
        /// <summary>
        /// asigna posicion en ruta a punto indicado
        /// </summary>
        /// <param name="datosPunto"></param>
        public void OrganizarPtos(PtoRuta datosPunto)
        {
            FabricaServicios.RutasCWeb.OrganizarPtos(datosPunto);
        }
        /// <summary>
        /// obtiene todos los medios de transporte
        /// </summary>
        /// <returns></returns>
        public List<RUMedioTransporte> ObtenerMediosTransporte()
        {
            return FabricaServicios.RutasCWeb.ObtenerMediosTransporte();
        }
        /// <summary>
        /// obtiene todos lod tipos de vehiculos
        /// </summary>
        /// <returns></returns>
        public List<RUTipoVehiculo> ObtenerTiposVehiculos()
        {
            return FabricaServicios.RutasCWeb.ObtenerTiposVehiculos();
        }
        /// <summary>
        /// obtiene todos los tipos de ruta
        /// </summary>
        /// <returns></returns>
        public List<RUTipoRuta> ObtenerTiposRuta()
        {
            return FabricaServicios.RutasCWeb.ObtenerTiposRuta();
        }
        /// <summary>
        /// crea nueva ruta
        /// </summary>
        /// <param name="ruta"></param>
        /// <returns></returns>
        public int CrearRuta(RURutaICWeb ruta)
        {
            return FabricaServicios.RutasCWeb.CrearRuta(ruta);
        }
    }

}
