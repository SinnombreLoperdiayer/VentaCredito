using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using CO.Controller.Servidor.Integraciones.CuatroSieteDos;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Admisiones.Giros.Datos.Modelo;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Util;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.LogisticaInversa;
using CO.Servidor.Servicios.ContratoDatos.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Admisiones.Giros.Datos
{
    public class GIRepositorio
    {
        #region Atributos

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private const string NombreModelo = "ModeloVentaGiros";

        /// <summary>
        /// Path almacena imagenes scanneadas
        /// </summary>
        private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];
        private string conexionStringArchivo = ConfigurationManager.ConnectionStrings["ControllerArchivo"].ConnectionString;
        private string conexionStringTransaccional = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;
        #endregion Atributos

        #region Crear Instancia

        private static readonly GIRepositorio instancia = new GIRepositorio();

        /// <summary>
        /// Retorna la instancia de la clase GIRepositorio
        /// </summary>
        public static GIRepositorio Instancia
        {
            get { return GIRepositorio.instancia; }
        }

        #endregion Crear Instancia

        #region Metodos

        /// <summary>
        /// valida si el giro ya fue creado
        /// </summary>
        /// <param name="idGiro">Numero del giro</param>
        /// <returns>True: el giro existe</returns>
        public bool ElGiroExiste(long idGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerGiroIdGiro giro = contexto.paObtenerGiroIdGiro_GIR(idGiro).FirstOrDefault();
                if (giro == null || giro.IdAdmisionGiro == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Obtiene el identificador de admisión giro
        /// </summary>
        /// <param name="idGiro">Identificador giro</param>
        /// <returns>Identificador admisión giro</returns>
        public long? ObtenerIdentificadorAdmisionGiro(long idGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                paObtenerGiroIdGiro consulta = contexto.paObtenerGiroIdGiro_GIR(idGiro).FirstOrDefault();

                if (consulta.IdAdmisionGiro != null)
                    return consulta.IdAdmisionGiro;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Creacion de un giro Peaton peaton
        /// </summary>
        /// <param name="giro"></param>
        public GINumeroGiro CrearGiro(GIAdmisionGirosDC giro, bool esGiroProduccion = false)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string digitoChequeo = string.Empty;

                digitoChequeo = GIDigitoChequeo.Instancia.CrearDigitoChequeo(giro.IdGiro.Value);

                // Insertar en la tabla admision giros y estados giros
                long? idGiro = contexto.paInsertarAdmisionGiros_GIR(
                  giro.IdGiro,
                  giro.PrefijoIdGiro,
                  digitoChequeo,
                  giro.GuidDeChequeo,
                  giro.GiroAutomatico,
                  GIConstantesAdmisionesGiros.GIROPEATONAPEATON,
                  GIEnumEstadosGiro.ACT.ToString(),
                  giro.AgenciaOrigen.IdCentroServicio,
                  giro.AgenciaOrigen.Nombre,
                  giro.AgenciaOrigen.IdPais,
                  giro.AgenciaOrigen.NombrePais,
                  giro.AgenciaOrigen.IdMunicipio,
                  giro.AgenciaOrigen.NombreMunicipio,
                  giro.AgenciaOrigen.CodigoPostal,
                  giro.AgenciaDestino.IdCentroServicio,
                  giro.AgenciaDestino.Nombre,
                  giro.AgenciaDestino.IdPais,
                  giro.AgenciaDestino.NombrePais,
                  giro.AgenciaDestino.IdMunicipio,
                  giro.AgenciaDestino.NombreMunicipio,
                  giro.AgenciaDestino.CodigoPostal,
                  giro.Precio.ValorGiro,
                  giro.Precio.ValorGiro != 0 ? giro.Precio.TarifaPorcPorte : 0,
                  giro.Precio.ValorGiro != 0 ? giro.Precio.TarifaFijaPorte : 0,
                  giro.Precio.ValorServicio,
                  giro.Precio.ValorAdicionales,
                  giro.Precio.ValorImpuestos,
                  giro.Precio.ValorTotal,
                  DateTime.Now,
                  ControllerContext.Current.Usuario,
                  giro.GirosPeatonPeaton.ClienteRemitente.TipoId,
                  giro.GirosPeatonPeaton.ClienteRemitente.Identificacion,
                  giro.GirosPeatonPeaton.ClienteRemitente.Nombre + " " + giro.GirosPeatonPeaton.ClienteRemitente.Apellido1 + " " + giro.GirosPeatonPeaton.ClienteRemitente.Apellido2,
                  giro.GirosPeatonPeaton.ClienteDestinatario.TipoId,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Identificacion,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Nombre + " " + giro.GirosPeatonPeaton.ClienteDestinatario.Apellido1 + " " + giro.GirosPeatonPeaton.ClienteDestinatario.Apellido2,
                  giro.DeclaracionVoluntariaOrigenes,
                  giro.Observaciones,
                  giro.RecibeNotificacionPago,
                  giro.GirosPeatonPeaton.ClienteRemitente.Telefono,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Telefono,
                  giro.GirosPeatonPeaton.ClienteRemitente.Direccion,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Direccion,
                  giro.GirosPeatonPeaton.ClienteRemitente.Email,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Email,
                  esGiroProduccion
                  ).FirstOrDefault();

                // Insertar en la tabla Giro Peaton peaton
                contexto.paInsertarGirosPeatonPeaton_GIR(
                  idGiro.Value,
                  giro.GirosPeatonPeaton.ClienteRemitente.Identificacion,
                  giro.GirosPeatonPeaton.ClienteRemitente.TipoId,
                  giro.GirosPeatonPeaton.ClienteRemitente.Nombre,
                  giro.GirosPeatonPeaton.ClienteRemitente.Apellido1,
                  giro.GirosPeatonPeaton.ClienteRemitente.Apellido2,
                  giro.GirosPeatonPeaton.ClienteRemitente.Telefono,
                  giro.GirosPeatonPeaton.ClienteRemitente.Direccion,
                  giro.GirosPeatonPeaton.ClienteRemitente.Email == null ? string.Empty : giro.GirosPeatonPeaton.ClienteRemitente.Email,
                  giro.GirosPeatonPeaton.ClienteRemitente.Ocupacion.DescripcionOcupacion,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Identificacion,
                  giro.GirosPeatonPeaton.ClienteDestinatario.TipoId,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Nombre,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Apellido1,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Apellido2,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Telefono,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Direccion,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Email == null ? string.Empty : giro.GirosPeatonPeaton.ClienteDestinatario.Email,
                  giro.GirosPeatonPeaton.ClienteDestinatario.Ocupacion.DescripcionOcupacion,
                  giro.GirosPeatonPeaton.ClienteDestinatario.TipoIdentificacionReclamoGiro,
                  DateTime.Now,
                  ControllerContext.Current.Usuario
                  );

                //Insertar en la tabla de Adicionales giros
                if (giro.Precio.ServiciosSolicitados != null)
                {
                    giro.Precio.ServiciosSolicitados.ToList().ForEach(ss =>
                    {
                        contexto.paInsertarAdicionalesGiro_GIR(idGiro, ss.IdTipoValorAdicional, ss.Descripcion, ss.PrecioValorAdicional, DateTime.Now, ControllerContext.Current.Usuario);

                        if (ss.CamposTipoValorAdicionalDC != null)
                        {
                            ss.CamposTipoValorAdicionalDC.ToList().ForEach(campo =>
                              {
                                  contexto.paInsertarDetalleAddGiros_GIR(idGiro, ss.IdTipoValorAdicional, campo.Display, campo.ValorCampo, DateTime.Now, ControllerContext.Current.Usuario);
                              });
                        }
                    });
                }

                // Insertar en la tabla impuestos giros
                if (giro.Precio.InfoImpuestos != null)
                {
                    giro.Precio.InfoImpuestos.ToList().ForEach(imp =>
                    {
                        contexto.paInsertarImpuestosGiro_GIR(idGiro, imp.IdImpuesto, imp.DescripcionImpuesto, imp.ValorImpuesto, giro.Precio.ValorImpuestos, DateTime.Now, ControllerContext.Current.Usuario);
                    });
                }

                IntegrarCuatroSieteDos(giro);

                return new GINumeroGiro() { IdGiro = giro.IdGiro, CodVerfiGiro = digitoChequeo, FechaGrabacion = DateTime.Now, PrefijoIdGiro = giro.PrefijoIdGiro };
            }
        }

        /// <summary>
        /// Creacion de un giro
        /// </summary>
        /// <param name="giro"></param>
        public GINumeroGiro GuardarGiroPeatonConvenio(GIAdmisionGirosDC giro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                string digitoChequeo = string.Empty;

                digitoChequeo = GIDigitoChequeo.Instancia.CrearDigitoChequeo(giro.IdGiro.Value);

                // Insertar en la tabla admision giros y estados giros
                long? idGiro = contexto.paInsertarAdmisionGiros_GIR(
                  giro.IdGiro,
                  giro.PrefijoIdGiro,
                  digitoChequeo,
                  giro.GuidDeChequeo,
                  true,
                  GIConstantesAdmisionesGiros.GIROPEATONACONVENIO,
                  GIEnumEstadosGiro.ACT.ToString(),
                  giro.AgenciaOrigen.IdCentroServicio,
                  giro.AgenciaOrigen.Nombre,
                  giro.AgenciaOrigen.IdPais,
                  giro.AgenciaOrigen.NombreDepto + "/" + giro.AgenciaOrigen.NombrePais,
                  giro.AgenciaOrigen.IdMunicipio,
                  giro.AgenciaOrigen.NombreMunicipio,
                  giro.AgenciaOrigen.CodigoPostal,
                  giro.AgenciaDestino.IdCentroServicio,
                  giro.AgenciaDestino.Nombre,
                  giro.AgenciaDestino.IdPais,
                  giro.AgenciaDestino.NombreDepto + "/" + giro.AgenciaDestino.NombrePais,
                  giro.AgenciaDestino.IdMunicipio,
                  giro.AgenciaDestino.NombreMunicipio,
                  giro.AgenciaDestino.CodigoPostal,
                  giro.Precio.ValorGiro,
                  giro.Precio.TarifaPorcPorte,
                  giro.Precio.TarifaFijaPorte,
                  giro.Precio.ValorServicio,
                  giro.Precio.ValorAdicionales,
                  giro.Precio.ValorImpuestos,
                  giro.Precio.ValorTotal,
                  DateTime.Now,
                  ControllerContext.Current.Usuario,
                  giro.GirosPeatonConvenio.ClienteContado.TipoId,
                  giro.GirosPeatonConvenio.ClienteContado.Identificacion,
                  giro.GirosPeatonConvenio.ClienteContado.Nombre + " " + giro.GirosPeatonConvenio.ClienteContado.Apellido1 + " " + giro.GirosPeatonConvenio.ClienteContado.Apellido2,
                  GIConstantesAdmisionesGiros.TIPOIDCONVENIO,
                  giro.GirosPeatonConvenio.ClienteConvenio.Nit,
                  giro.GirosPeatonConvenio.ClienteConvenio.RazonSocial,
                  giro.DeclaracionVoluntariaOrigenes,
                  giro.Observaciones,
                  giro.RecibeNotificacionPago,
                  giro.GirosPeatonConvenio.ClienteContado.Telefono,
                  giro.GirosPeatonConvenio.ClienteConvenio.Telefono,
                  giro.GirosPeatonConvenio.ClienteContado.Direccion,
                  giro.GirosPeatonConvenio.ClienteConvenio.Direccion,
                  giro.GirosPeatonConvenio.ClienteContado.Email == null ? string.Empty : giro.GirosPeatonConvenio.ClienteContado.Email,
                  string.Empty,
                  false
                  ).FirstOrDefault();

                // Insertar en la tabla Giro Peaton Convenio
                contexto.paInsertarGirosPeatonConvenio_GIR(
                  idGiro.Value,
                  giro.GirosPeatonConvenio.ClienteContado.Identificacion,
                  giro.GirosPeatonConvenio.ClienteContado.TipoId,
                  giro.GirosPeatonConvenio.ClienteContado.Nombre,
                  giro.GirosPeatonConvenio.ClienteContado.Apellido1,
                  giro.GirosPeatonConvenio.ClienteContado.Apellido2,
                  giro.GirosPeatonConvenio.ClienteContado.Telefono,
                  giro.GirosPeatonConvenio.ClienteContado.Direccion,
                  giro.GirosPeatonConvenio.ClienteContado.Email == null ? string.Empty : giro.GirosPeatonConvenio.ClienteContado.Email,
                  giro.GirosPeatonConvenio.ClienteContado.Ocupacion.DescripcionOcupacion,
                  giro.GirosPeatonConvenio.ClienteConvenio.IdCliente,
                  giro.GirosPeatonConvenio.ClienteConvenio.Nit,
                  giro.GirosPeatonConvenio.ClienteConvenio.RazonSocial,
                  giro.GirosPeatonConvenio.ClienteConvenio.Telefono,
                  giro.GirosPeatonConvenio.ClienteConvenio.Direccion,
                  DateTime.Now,
                  ControllerContext.Current.Usuario
                  );

                //Insertar en la tabla de Adicionales giros
                if (giro.Precio.ServiciosSolicitados != null)
                {
                    giro.Precio.ServiciosSolicitados.ToList().ForEach(ss =>
                    {
                        contexto.paInsertarAdicionalesGiro_GIR(idGiro, ss.IdTipoValorAdicional, ss.Descripcion, ss.PrecioValorAdicional, DateTime.Now, ControllerContext.Current.Usuario);

                        if (ss.CamposTipoValorAdicionalDC != null)
                        {
                            ss.CamposTipoValorAdicionalDC.ToList().ForEach(campo =>
                            {
                                contexto.paInsertarDetalleAddGiros_GIR(idGiro, ss.IdTipoValorAdicional, campo.Display, campo.ValorCampo, DateTime.Now, ControllerContext.Current.Usuario);
                            });
                        }
                    });
                }

                // Insertar en la tabla impuestos giros
                if (giro.Precio.InfoImpuestos != null)
                {
                    giro.Precio.InfoImpuestos.ToList().ForEach(imp =>
                    {
                        contexto.paInsertarImpuestosGiro_GIR(idGiro, imp.IdImpuesto, imp.DescripcionImpuesto, imp.ValorImpuesto, giro.Precio.ValorImpuestos, DateTime.Now, ControllerContext.Current.Usuario);
                    });
                }
                IntegrarCuatroSieteDos(giro);

                return new GINumeroGiro() { IdGiro = giro.IdGiro, CodVerfiGiro = digitoChequeo, FechaGrabacion = DateTime.Now, PrefijoIdGiro = giro.PrefijoIdGiro };
            }
        }

        /// <summary>
        /// reporta el movimiento del giro con 472
        /// </summary>
        /// <param name="giro"></param>
        public void IntegrarCuatroSieteDos(GIAdmisionGirosDC giro)
        {
            //Integracion472.Instancia.IntegrarCuatroSieteDosAdmision(giro,"4");
            // TODO: RON Prueba de deshabilitación integración
        }

        /// <summary>
        /// Consulta la informacion de un giro a partir de el guid
        /// </summary>
        /// <param name="GuidDeChequeo"></param>
        /// <returns></returns>
        public GINumeroGiro ConsultarGiroPorGuid(string guidDeChequeo)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIpaObtenerGiroGuidGiro giroConsulta = contexto.paObtenerGiroGuidGiro_GIR(guidDeChequeo).FirstOrDefault();
                if (giroConsulta != null)
                {
                    return new GINumeroGiro() { IdGiro = giroConsulta.ADG_IdGiro, CodVerfiGiro = giroConsulta.ADG_DigitoVerificacion, FechaGrabacion = giroConsulta.ADG_FechaGrabacion };
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton peaton
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonPeatonPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime? fechaInicial = DateTime.Now.Date;
                long? idCentroServicioOrigen = null;
                long? idCentroServicioDestino = null;
                long? idRacol = null;

                // Un RACOL puede consultar y reimprimir los giros del mes
                if (string.Compare(ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                    idRacol = idCentroServicio;
                }
                // Un Usuario de Direccion general  puede consultar y reimprimir los giros sin un limite de fecha
                else if (string.Compare(ConstantesFramework.USUARIO_GESTION, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                }
                else
                {
                    idCentroServicioOrigen = idCentroServicio;
                }

                return contexto.paObtenerGirosFiltro_GIR(indicePagina, registrosPorPagina, idCentroServicioOrigen, idCentroServicioDestino, fechaInicial, DateTime.Now, idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, GIConstantesAdmisionesGiros.GIROPEATONAPEATON, idRacol).ToList().
             ConvertAll<GIAdmisionGirosDC>(
               adm => new GIAdmisionGirosDC()
               {
                   IdAdminGiro = adm.ADG_IdAdmisionGiro,
                   CodVerfiGiro = adm.ADG_DigitoVerificacion,

                   IdGiro = adm.ADG_IdGiro,
                   Observaciones = adm.ADG_Observaciones,
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
                       NombrePais = adm.ADG_DescPaisDestino,
                       Direccion = adm.CES_Direccion,
                       Telefono1 = adm.CES_Telefono1,
                       IdMunicipio = adm.ADG_IdCiudadDestino,
                   },
                   Precio = new TAPrecioDC
                   {
                       ValorGiro = adm.ADG_ValorGiro,
                       ValorServicio = adm.ADG_ValorPorte,
                       ValorImpuestos = adm.ADG_ValorImpuestos,
                       ValorAdicionales = adm.ADG_ValorAdicionales,
                       ValorTotal = adm.ADG_ValorTotal,
                       ValorTotalServicio = adm.ADG_ValorPorte + adm.ADG_ValorImpuestos + adm.ADG_ValorAdicionales,
                   },
                   FechaGrabacion = adm.ADG_FechaGrabacion,
                   EstadoGiro = GIEnumEstadosGiro.ACT.ToString()
               });
            }
        }

        /// <summary>
        /// consulta los giros activos realizados el dia actual peaton Convenio
        /// No retorna todos los valores del giro
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<GIAdmisionGirosDC> ConsultarGirosPeatonConvenioPorAgencia(long idCentroServicio, long idGiro, string tipoIdRemitente, string identificacionRemitente, string tipoIdDestinatario, string identificacionDestinatario, int indicePagina, int registrosPorPagina, string tipoCentroServicio)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                //DateTime fechaInicial = DateTime.Now.Date.Add(new TimeSpan(-1, 23, 59, 59, 59));
                DateTime? fechaInicial = DateTime.Now.Date;
                long? idCentroServicioOrigen = null;
                long? idCentroServicioDestino = null;
                long? idRacol = null;

                idCentroServicioOrigen = idCentroServicio;

                // Un RACOL puede consultar y reimprimir los giros del mes
                if (string.Compare(ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                    idRacol = idCentroServicio;
                }
                // Un Usuario de Direccion general  puede consultar y reimprimir los giros sin un limite de fecha
                else if (string.Compare(ConstantesFramework.USUARIO_GESTION, tipoCentroServicio, true) == 0)
                {
                    fechaInicial = null;
                }
                else
                {
                    idCentroServicioOrigen = idCentroServicio;
                }

                return contexto.paObtenerGirosFiltro_GIR(indicePagina, registrosPorPagina, idCentroServicioOrigen, idCentroServicioDestino, fechaInicial, DateTime.Now.AddDays(1), idGiro, tipoIdRemitente, identificacionRemitente, tipoIdDestinatario, identificacionDestinatario, GIConstantesAdmisionesGiros.GIROPEATONACONVENIO, idRacol).ToList().
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
                       NombrePais = adm.ADG_DescPaisOrigen,
                   },
                   AgenciaDestino = new PUCentroServiciosDC()
                   {
                       IdCentroServicio = adm.ADG_IdCentroServicioDestino,
                       Nombre = adm.ADG_NombreCentroServicioDestino,
                       CodigoPostal = adm.ADG_CodigoPostalDestino,
                       NombreMunicipio = adm.ADG_DescCiudadDestino,
                       NombrePais = adm.ADG_DescPaisDestino,
                       Direccion = adm.CES_Direccion,
                       Telefono1 = adm.CES_Telefono1
                   },
                   Precio = new TAPrecioDC
                   {
                       ValorGiro = adm.ADG_ValorGiro,
                       ValorServicio = adm.ADG_ValorPorte,
                       ValorImpuestos = adm.ADG_ValorImpuestos,
                       ValorAdicionales = adm.ADG_ValorAdicionales,
                       ValorTotal = adm.ADG_ValorTotal
                   },
                   Observaciones = adm.ADG_Observaciones,
                   FechaGrabacion = adm.ADG_FechaGrabacion,
                   EstadoGiro = GIEnumEstadosGiro.ACT.ToString()
               });
            }
        }

        /// <summary>
        /// Consultar la informacion de la tabla peaton peaton
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeaton(long idAdmisionGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIpaObtenerGiroPeatonPeaton peaton = contexto.paObtenerGiroPeatonPeaton_GIR(idAdmisionGiro).FirstOrDefault();

                GIAdmisionGirosDC admision = new GIAdmisionGirosDC()
                {
                    GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                    {
                        ClienteRemitente = new CLClienteContadoDC()
                        {
                            Apellido1 = peaton.GPP_Apellido1Remitente,
                            Apellido2 = peaton.GPP_Apellido2Remitente == null ? string.Empty : peaton.GPP_Apellido2Remitente,
                            Direccion = peaton.GPP_DireccionRemitente,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = peaton.GPP_OcupacionRemitente },
                            Email = peaton.GPP_EmailRemitente,
                            Identificacion = peaton.GPP_IdRemitente,
                            Nombre = peaton.GPP_NombreRemitente,
                            Telefono = peaton.GPP_TelefonoRemitente,
                            TipoId = peaton.GPP_TipoIdRemitente
                        },
                        ClienteDestinatario = new CLClienteContadoDC()
                        {
                            Apellido1 = peaton.GPP_Apellido1Destinatario,
                            Apellido2 = peaton.GPP_Apellido2Destinatario == null ? string.Empty : peaton.GPP_Apellido2Destinatario,
                            Direccion = peaton.GPP_DireccionDestinatario,
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = peaton.GPP_OcupacionDestinatario },
                            Email = peaton.GPP_EmailDestinatario,
                            Identificacion = peaton.GPP_IdDestinatario,
                            Nombre = peaton.GPP_NombreDestinatario,
                            Telefono = peaton.GPP_TelefonoDestinatario,
                            TipoId = peaton.GPP_TipoIdDestinatario,
                        }
                    }
                };
                return admision;
            }
        }

        /// <summary>
        /// Consultar la informacion de la tabla peaton Convenio
        /// </summary>
        /// <param name="idAdmisionGiro"></param>
        public GIAdmisionGirosDC ConsultarInformacionPeatonConvenio(long idAdmisionGiro)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                GIpaObtenerGiroPeatonConvenio convenio = contexto.paObtenerGiroPeatonConvenio_GIR(idAdmisionGiro).FirstOrDefault();

                if (convenio != null)
                {
                    GIAdmisionGirosDC admision = new GIAdmisionGirosDC()
                    {
                        GirosPeatonConvenio = new GIGirosPeatonConvenioDC()
                        {
                            ClienteContado = new CLClienteContadoDC()
                            {
                                Apellido1 = convenio.GPC_Apellido1Remitente,
                                Apellido2 = convenio.GPC_Apellido2Remitente,
                                Direccion = convenio.GPC_DireccionRemitente,
                                Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = convenio.GPC_OcupacionRemitente.ToString() },
                                Email = convenio.GPC_EmailRemitente,
                                Identificacion = convenio.GPC_IdRemitente,
                                Nombre = convenio.GPC_NombreRemitente,
                                Telefono = convenio.GPC_TelefonoRemitente,
                                TipoId = convenio.GPC_TipoIdRemitente
                            },
                            ClienteConvenio = new CLClientesDC()
                            {
                                Nit = convenio.GPC_NitConvenioDestinatario,
                                RazonSocial = convenio.GPC_RazonSocialConvenioDestinatario,
                                Telefono = convenio.GPC_TelefonoConvenioDestinatario,
                                Direccion = convenio.GPC_DireccionConvenioDestinatario
                            }
                        }
                    };
                    return admision;
                }
                else
                    return null;
            }
        }

        /// <summary>
        /// Valida que el remitente no supere el monto de envios diarios de dinero en un giro
        /// </summary>
        /// <param name="giro"></param>
        public bool ValidarDeclaracionFondos(decimal valorAcumulado)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                if (contexto.ParametrosGiros_GIR.FirstOrDefault(par => par.PAG_IdParametro == GIConstantesAdmisionesGiros.VALORMAXIMODECLARACIONFONDOS) == null)
                {
                    return false;
                }

                string valorParametro = contexto.ParametrosGiros_GIR.FirstOrDefault(par => par.PAG_IdParametro == GIConstantesAdmisionesGiros.VALORMAXIMODECLARACIONFONDOS).PAG_ValorParametro;
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

        /// <summary>
        /// Almacenar declaracion voluntaria de fondos
        /// </summary>
        /// <param name="archivo">archivo a almacenar</param>
        public long AlmacenarDeclaracionFondos(string archivo, GIAdmisionGirosDC giro)
        {

            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenArchivoGiro_GIR almacenArchivo = new AlmacenArchivoGiro_GIR()
                {
                    AGI_RutaArchivo = archivo,
                    AGI_NombreAdjunto = GIEnumTipoDocumentoPagoGiro.DOCUMENTOIDENTIDAD.ToString(),
                    AGI_CreadoPor = ControllerContext.Current.Usuario,
                    AGI_FechaGrabacion = DateTime.Now
                };
                contexto.AlmacenArchivoGiro_GIR.Add(almacenArchivo);
                ArchivosPagoGiro_GIR archivoPag = new ArchivosPagoGiro_GIR()
                {
                    APG_IdAdmisionGiro = giro.IdAdminGiro.Value,
                    APG_IdArchivo = almacenArchivo.AGI_IdArchivo,
                    APG_CreadoPor = ControllerContext.Current.Usuario,
                    APG_IdTipoDocumentoPagoGiro = (short)GIEnumTipoDocumentoPagoGiro.DOCUMENTOIDENTIDAD,
                    APG_FechaGrabacion = DateTime.Now
                };
                contexto.ArchivosPagoGiro_GIR.Add(archivoPag);
                contexto.SaveChanges();
                return almacenArchivo.AGI_IdArchivo;
            }

        }

        /// <summary>
        /// Consulta los tipos de identificacion con los que se reclama el giro
        /// </summary>
        /// <returns>Lista de tipos de identificacion</returns>
        public IList<PATipoIdentificacion> ConsultarTiposIdentificacionReclamaGiros()
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoIdentificacionAdmin_GIR.Where(tipo => tipo.TII_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList().ConvertAll(
                  obj =>
                  new PATipoIdentificacion()
                  {
                      DescripcionIdentificacion = obj.TII_Descripcion,
                      IdTipoIdentificacion = obj.TII_IdTipoIdentificacion
                  });
            }
        }

        /// <summary>
        /// Actualiza la Informacion del Giro
        /// por una Solicitud
        /// </summary>
        /// <param name="giroUpdate">info del giro a Actualizar</param>
        public void ActualizarInfoGiro(GIAdmisionGirosDC giroUpdate)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                AlmacenarAuditAdmisionGiro(giroUpdate.IdAdminGiro.Value, EnumEstadoRegistro.MODIFICADO);

                contexto.paActualizarGiroPorSolicitud_GIR(giroUpdate.IdAdminGiro, giroUpdate.AgenciaDestino.CiudadUbicacion.IdLocalidad, giroUpdate.AgenciaDestino.CiudadUbicacion.Nombre,
                                                            giroUpdate.AgenciaDestino.PaisCiudad.IdLocalidad, giroUpdate.AgenciaDestino.PaisCiudad.Nombre, giroUpdate.AgenciaDestino.IdCentroServicio,
                                                            giroUpdate.AgenciaDestino.Nombre, giroUpdate.EsTransmitido, giroUpdate.GirosPeatonPeaton.ClienteDestinatario.TipoId,
                                                            giroUpdate.GirosPeatonPeaton.ClienteDestinatario.Identificacion, giroUpdate.GirosPeatonPeaton.ClienteDestinatario.NombreYApellidos,
                                                            giroUpdate.Observaciones, giroUpdate.Precio.ValorGiro, giroUpdate.Precio.ValorServicio, giroUpdate.Precio.ValorTotal);
                contexto.SaveChanges();
            }
        }


        /// <summary>
        /// Obtiene información inicial destinatario peaton peaton
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        /// <returns></returns>
        public GIAdmisionGirosDC ConsultarInformacionPeatonPeatonInicial(long idAdmisiongiro)
        {
            GIAdmisionGirosDC admision = null;
            using (SqlConnection conn = new SqlConnection(conexionStringTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionGiroPeatonPeaton_GIR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionGiro", idAdmisiongiro);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    admision = new GIAdmisionGirosDC()
                    {
                        GirosPeatonPeaton = new GIGirosPeatonPeatonDC()
                        {
                            ClienteRemitente = new CLClienteContadoDC()
                            {
                                Apellido1 = string.Empty,
                                Apellido2 = string.Empty,
                                Direccion = reader["ADG_DireccionRemitente"].ToString(),
                                Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = reader["GPP_OcupacionRemitente"].ToString() },
                                Email = reader["ADG_EmailRemitente"].ToString(),
                                Identificacion = reader["ADG_IdRemitente"].ToString(),
                                Nombre = reader["ADG_NombreRemitente"].ToString(),
                                Telefono = reader["ADG_TelefonoRemitente"].ToString(),
                                TipoId = reader["GPP_TipoIdRemitente"].ToString()
                            },
                            ClienteDestinatario = new CLClienteContadoDC()
                            {
                                Apellido1 = string.Empty,
                                Apellido2 = string.Empty,
                                Direccion = reader["ADG_DireccionDestinatario"].ToString(),
                                Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = reader["GPP_OcupacionDestinatario"].ToString() },
                                Email = reader["ADG_EmailDestinatario"].ToString(),
                                Identificacion = reader["ADG_IdDestinatario"].ToString(),
                                Nombre = reader["ADG_NombreDestinatario"].ToString(),
                                Telefono = reader["ADG_TelefonoDestinatario"].ToString(),
                                TipoId = reader["GPP_TipoIdDestinatario"].ToString()
                            }
                        }
                    };
                }
            }
            return admision;
        }

        /// <summary>
        /// Obtiene informacion inicial del destinatario peaton convenio
        /// </summary>
        /// <param name="idAdmisiongiro"></param>
        /// <returns></returns>
        public GIAdmisionGirosDC ConsultarInformacionPeatonConvenioInicial(long idAdmisiongiro)
        {
            GIAdmisionGirosDC admision = new GIAdmisionGirosDC();
            using (SqlConnection conn = new SqlConnection(conexionStringTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerInformacionGiroPeatonConvenio_GIR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idAdmisionGiro", idAdmisiongiro);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    admision.GirosPeatonConvenio = new GIGirosPeatonConvenioDC()
                    {
                        ClienteContado = new CLClienteContadoDC()
                        {
                            Apellido1 = string.Empty,
                            Apellido2 = string.Empty,
                            Direccion = reader["ADG_DireccionRemitente"].ToString(),
                            Ocupacion = new PAOcupacionDC() { DescripcionOcupacion = reader["GPP_OcupacionRemitente"].ToString() },
                            Email = reader["ADG_EmailRemitente"].ToString(),
                            Identificacion = reader["ADG_IdRemitente"].ToString(),
                            Nombre = reader["ADG_NombreRemitente"].ToString(),
                            Telefono = reader["ADG_TelefonoRemitente"].ToString(),
                            TipoId = reader["GPP_TipoIdRemitente"].ToString()
                        },
                        ClienteConvenio = new CLClientesDC()
                        {
                            Nit = reader["GPC_NitConvenioDestinatario,"].ToString(),
                            RazonSocial = reader["GPC_RazonSocialConvenioDestinatario,"].ToString(),
                            Telefono = reader["GPC_TelefonoConvenioDestinatario,"].ToString(),
                            Direccion = reader["GPC_DireccionConvenioDestinatario,"].ToString()
                        }
                    };
                }
            }
            return admision;
        }


        /// <summary>
        /// Consulta la tabla Parametros Giros
        /// </summary>
        /// <param name="idParametro"></param>
        /// <returns></returns>
        public GIParametrosGirosDC ConsultarParametrosGiros(string idParametro)
        {
            GIParametrosGirosDC paramGiros = new GIParametrosGirosDC();

            using (SqlConnection conn = new SqlConnection(conexionStringTransaccional))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("paObtenerParametrosGiros_GIR", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdParametro", idParametro);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    paramGiros.IdParametro = reader["PAG_IdParametro"].ToString();
                    paramGiros.ValorParametro = reader["PAG_ValorParametro"].ToString();
                }
            }
            return paramGiros;
        }


        #endregion Metodos

            #region Historico

            /// <summary>
            /// Metodo de Auditoria de Giros
            /// </summary>
            /// <param name="idAdmisionGiro"></param>
            /// <param name="tipoModificacion"></param>
        private void AlmacenarAuditAdmisionGiro(long idAdmisionGiro, EnumEstadoRegistro tipoModificacion)
        {
            using (ModeloVentaGiros contexto = new ModeloVentaGiros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                contexto.paInsertarAdmisionGiroHistorico_GIR(idAdmisionGiro, DateTime.Now, ControllerContext.Current.Usuario,
                                                              tipoModificacion.ToString());
            }
        }

        #endregion Historico
    }
}