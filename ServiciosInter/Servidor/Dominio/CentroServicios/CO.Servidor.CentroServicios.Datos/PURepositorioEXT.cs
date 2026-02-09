using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using CO.Servidor.CentroServicios.Comun;
using CO.Servidor.CentroServicios.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Data.SqlClient;
using System.Configuration;

namespace CO.Servidor.CentroServicios.Datos
{
    public partial class PURepositorio
    {
        private string CadCnxController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        #region Servicio Giros

        /// <summary>
        /// Consulta el estado del Centro de Servicios
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        /// <returns>Estado del centro de servicios: Activo = 'ACT', Inactivo  = 'INA', En Liquidación = 'LIQ'</returns>
        public EstadoDC ConsultarEstadoAgencia(long idCentroServicios)
        {
            string estado = string.Empty;
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                estado = contexto.CentroServicios_PUA.FirstOrDefault(obj => obj.CES_IdCentroServicios == idCentroServicios).CES_Estado;
            }

            return new EstadoDC() { IdEstado = estado, EstadoDescripcion = estado };
        }

        /// <summary>
        /// Consulta si la agencia tiene configurado el servicio de Giros
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        /// <param name="idServicio">Id servicio de giros</param>
        /// <returns>True: la agencia posee un servicio de giros</returns>
        public bool ConsultarServiciosGirosEnAgencia(long idCentroServicios, int idServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var centroServicio = contexto.CentroSvcLocalidadServici_VPUA.Where(cs => cs.CES_IdCentroServicios == idCentroServicios && cs.CSS_IdServicio == idServicio && cs.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO).FirstOrDefault();
                if (centroServicio == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Consulta si la agencia puede recibir giros
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        /// <returns>Indica si la agencia que tenga asignado el servicio de giros está autorizado para recibir giros</returns>
        public bool ConsultarPuedeRecibirGiros(long idCentroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentroServicios_PUA.FirstOrDefault(cs => cs.CES_IdCentroServicios == idCentroServicios).CES_PuedeRecibirGiros;
            }
        }

        /// <summary>
        /// Consulta si la agencia puede pagar giros
        /// </summary>
        /// <param name="idCentroServicios">Codigo Centro Servicios</param>
        /// <returns>Indica si la agencia que tenga asignado el servicio de giros está autorizado para recibir giros</returns>
        public bool ConsultarPuedePagarGiros(long idCentroServicios, int idServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaActual = DateTime.Now.Date.Add(new TimeSpan(0, 23, 59, 59, 59));
                var puedePagar = contexto.CentroSvcLocalidadServici_VPUA.Where(cs =>
                         cs.CES_Tipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL &&
                         cs.CES_Tipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_COL &&
                         cs.CES_PuedePagarGiros == true &&
                         cs.CES_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                         cs.CES_IdCentroServicios == idCentroServicios &&
                         cs.CSS_IdServicio == idServicio &&
                         cs.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                         cs.CSS_FechaInicioVenta <= fechaActual).FirstOrDefault();

                if (puedePagar != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Obtiene los datos básicos de los centros de servivios de giros
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <returns>Colección centros de servicio</returns>
        public IEnumerable<PUCentroServiciosDC> ObtenerCentrosServicioGiros(int idServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<PUCentroServiciosDC> centrosServicio = contexto.CentroSvcLocalidadServici_VPUA
                  .Where(cs =>
                         cs.CES_Tipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_RACOL &&
                         cs.CES_Tipo != ConstantesFramework.TIPO_CENTRO_SERVICIO_COL &&
                         cs.CES_PuedePagarGiros == true &&
                         cs.CES_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                         cs.CSS_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                         cs.CSS_IdServicio == idServicio &&
                         cs.CSS_FechaInicioVenta < DateTime.Now)
                         .OrderBy(o => o.CES_Nombre)
                          .ToList()
                         .ConvertAll(r => new PUCentroServiciosDC()
                         {
                             IdCentroServicio = r.CES_IdCentroServicios,
                             Tipo = r.CES_Tipo,
                             Nombre = r.CES_Nombre
                         });

                return centrosServicio;
            }
        }

        /// <summary>
        /// Obtiene el numero total de envios en pendientyes por ingr a custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoPendIngrCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConteoGuiasEnCustodiaIngreso_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoMovimiento", idTipoMovimiento);
                cmd.Parameters.AddWithValue("@IdEstadoGuia", idEstadoGuia);
                conn.Open();
                var resultado = cmd.ExecuteScalar();
                conn.Close();
                return (resultado == null ? 0 : Convert.ToInt32(resultado));
            }
        }

        /// <summary>
        /// Obtiene el numero total de envios en custodia
        /// </summary>
        /// <param name="idTipoMovimiento"></param>
        /// <param name="idEstadoGuia"></param>
        /// <returns></returns>
        public int ObtenerConteoGuiasCustodia(int idTipoMovimiento, int idEstadoGuia)
        {
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConteoGuiasEnCustodia_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdTipoMovimiento", idTipoMovimiento);
                cmd.Parameters.AddWithValue("@IdEstadoGuia", idEstadoGuia);
                conn.Open();
                var resultado = cmd.ExecuteScalar();
                conn.Close();
                return (resultado == null ? 0 : Convert.ToInt32(resultado));
            }
        }

        /// <summary>
        /// Obtiene los puntosde atencion
        /// de una agencia centro Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Puntos de Una Agencia</returns>
        public List<PUCentroServiciosDC> ObtenerPuntosDeAgenciaActivos(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.PuntosDeAgencia_VPUA.Where(l => l.PUS_IdAgencia == idCentroServicio && l.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll<PUCentroServiciosDC>(Puntos => new PUCentroServiciosDC()
                  {
                      IdCentroServicio = Puntos.CES_IdCentroServicios,
                      Nombre = Puntos.CES_Nombre,
                      Estado = Puntos.CES_Estado,
                  });
            }
        }

        /// <summary>
        /// Obtiene la lista de las agencias que pueden realizar pagos de giros
        /// </summary>
        /// <param name="unidadNegocio">Unidad de negocio giro</param>
        public IList<PUCentroServiciosDC> ObtenerAgenciasPuedenPagarGiros(int idServicio)
        {

            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciasPuedenPagarGiros_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CSS_IdServicio", idServicio);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable ();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                IList<PUCentroServiciosDC> listaPuntos = dt.AsEnumerable().ToList().ConvertAll(centroSer => new PUCentroServiciosDC()
                      {
                          IdCentroServicio = centroSer.Field<long>("CES_IdCentroServicios"),
                          IdCentroCostos = centroSer.Field<string>("CES_IdCentroCostos"),
                          Barrio = centroSer.Field<string>("CES_Barrio"),
                          DigitoVerificacion = centroSer.Field<string>("CES_DigitoVerificacion"),
                          Direccion = centroSer.Field<string>("CES_Direccion"),
                          Email = centroSer.Field<string>("CES_Email"),
                          Estado = centroSer.Field<string>("CES_Estado"),
                          Fax = centroSer.Field<string>("CES_Fax"),
                          Latitud = centroSer.Field<decimal?>("CES_Latitud"),
                          Nombre = centroSer.Field<string>("CES_Nombre") + " - Barrio: " + centroSer.Field<string>("CES_Barrio"),
                          Telefono1 = centroSer.Field<string>("CES_Telefono1"),
                          Telefono2 = centroSer.Field<string>("CES_Telefono2"),
                          Sistematizado = centroSer.Field<bool>("CES_Sistematizada"),
                          IdMunicipio = centroSer.Field<string>("CES_IdMunicipio"),
                          NombreMunicipio = centroSer.Field<string>("LOC_Nombre"),
                          CodigoPostal = centroSer.Field<string>("LOC_CodigoPostal"),
                          IdPais = centroSer.Field<string>("IdPais"),
                          NombrePais = centroSer.Field<string>("NombrePais"),
                          NombreDepto = centroSer.Field<string>("NombreDepartamento"),
                          Tipo = centroSer.Field<string>("CES_Tipo"),
                          IdTipoPropiedad = centroSer.Field<short>("CES_IdTipoPropiedad"),

                      });
                return listaPuntos;
            }

        }

        /// <summary>
        /// Retorna las agencias creadas en el sistemas que se encuentran activas
        /// </summary>
        /// <returns></returns>
        public List<PUAgencia> ObtenerAgenciasActivas()
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Agencia_VPUA.Where(agencia => agencia.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                  .ConvertAll(agencia => new PUAgencia
                  {
                      IdAgencia = agencia.AGE_IdAgencia,
                      IdTipoAgencia = agencia.AGE_IdTipoAgencia,
                      CentroServicios = new PUCentroServiciosDC
                        {
                            IdCentroServicio = agencia.AGE_IdAgencia,
                            Nombre = agencia.CES_Nombre
                        },
                      IdLocalidad = agencia.LOC_IdLocalidad,
                      TienePuntosACargo = contexto.PuntosDeAgencia_VPUA.Count(punto => agencia.AGE_IdAgencia == punto.PUS_IdAgencia) > 0
                  });
            }
        }

        /// <summary>
        /// Obtiene los centros de servicios asociados al racol que puedan pagar giros
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public IList<PUCentroServiciosDC> ObtenerCentroSvcRacolPuedenPagarGiros(long idRacol, int idServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IList<PUCentroServiciosDC> listaPuntos = contexto.CentrosServicosRacol_VPUA.Where(cs =>
                         cs.CES_PuedePagarGiros == true &&
                         cs.CES_Estado == ConstantesFramework.ESTADO_ACTIVO &&
                         cs.REA_IdRegionalAdm == idRacol
                         ).ToList().ConvertAll(centroSer => new PUCentroServiciosDC()
                         {
                             IdCentroServicio = centroSer.CES_IdCentroServicios,
                             IdCentroCostos = centroSer.CES_IdCentroCostos,
                             Direccion = centroSer.CES_Direccion,
                             Estado = centroSer.CES_Estado,
                             Nombre = centroSer.CES_Nombre,
                             Telefono1 = centroSer.CES_Telefono1,
                             Sistematizado = centroSer.CES_Sistematizada,
                             NombreMunicipio = centroSer.LOC_Nombre,
                         });
                return listaPuntos;
            }
        }

        /// <summary>
        /// Valida que el Centro de servicio no supere el valor maximo a enviar de giros y acumula el valor del giro a la agenci
        /// </summary>
        /// <param name="idCentroServicio"></param>
        public void AcumularVentaGirosAgencia(GIAdmisionGirosDC giro)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial = DateTime.Now.Date;
                AcumuladosCentroServicios_PUA acumulado = contexto.AcumuladosCentroServicios_PUA.FirstOrDefault(ac => ac.ACS_IdCentroServicios == giro.AgenciaOrigen.IdCentroServicio);
                if (acumulado != null)
                {
                    if (acumulado.ACS_FechaActualizacion < fechaInicial)
                        acumulado.ACS_AcumuladoVentaGiros = giro.Precio.ValorGiro;
                    else
                        acumulado.ACS_AcumuladoVentaGiros += giro.Precio.ValorGiro;

                    acumulado.ACS_FechaActualizacion = DateTime.Now;
                }
                else
                {
                    acumulado = new AcumuladosCentroServicios_PUA()
                    {
                        ACS_AcumuladoVentaGiros = giro.Precio.ValorGiro,
                        ACS_CreadoPor = ControllerContext.Current.Usuario,
                        ACS_FechaActualizacion = DateTime.Now,
                        ACS_FechaGrabacion = DateTime.Now,
                        ACS_IdCentroServicios = giro.AgenciaOrigen.IdCentroServicio
                    };
                    contexto.AcumuladosCentroServicios_PUA.Add(acumulado);
                }

                decimal topemax = contexto.CentroServicios_PUA.FirstOrDefault(cs => cs.CES_IdCentroServicios == giro.AgenciaOrigen.IdCentroServicio).CES_TopeMaximoPorGiros;

                if (topemax != 0 && topemax < acumulado.ACS_AcumuladoVentaGiros)
                {
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            PAAdministrador.Instancia.EnviarCorreoAgenciaSuperaMaxOpe(giro.AgenciaOrigen.IdCentroServicio, giro.AgenciaOrigen.Nombre);
                        }
                        catch (Exception ex)
                        {
                            AuditoriaTrace.EscribirAuditoria(ETipoAuditoria.Error, ex.ToString(), COConstantesModulos.GIROS);
                        }
                    }, TaskCreationOptions.PreferFairness);

                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_AGENCIA_SUPERO_MAX_VENTA_GIROS.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_AGENCIA_SUPERO_MAX_VENTA_GIROS)));
                }
                else
                {
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Actualiza la informacin de una agencia para el servicio de giros
        /// </summary>
        /// <param name="centroServicio"></param>
        public void ActualizarConfiguracionGiros(PUCentroServiciosDC centroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroServicios_PUA cServicio = contexto.CentroServicios_PUA.Where(cs => cs.CES_IdCentroServicios == centroServicio.IdCentroServicio).FirstOrDefault();
                cServicio.CES_ClasGirosPorIngresos = centroServicio.ClasificacionPorIngresos.IdClasificacion;
                cServicio.CES_PuedePagarGiros = centroServicio.PagaGiros;
                cServicio.CES_PuedeRecibirGiros = centroServicio.RecibeGiros;
                cServicio.CES_Sistematizada = centroServicio.Sistematizado;
                cServicio.CES_TopeMaximoPorGiros = centroServicio.TopeMaximoGiros;
                cServicio.CES_TopeMaximoPorPagos = centroServicio.TopeMaximoPagos;

                if (centroServicio.ObservacionCentroServicio != null)
                {
                    ObservacionCentroServicio_PUA observaciones = new ObservacionCentroServicio_PUA()
                    {
                        OCS_IdCentroServicios = centroServicio.IdCentroServicio,
                        OCS_Motivo = centroServicio.ObservacionCentroServicio.Motivo == null ? string.Empty : centroServicio.ObservacionCentroServicio.Motivo,
                        OCS_Obsevacion = centroServicio.ObservacionCentroServicio.Obsevacion == null ? string.Empty : centroServicio.ObservacionCentroServicio.Obsevacion,
                        OCS_CreadoPor = ControllerContext.Current.Usuario,
                        OCS_FechaGrabacion = DateTime.Now
                    };
                    cServicio.ObservacionCentroServicio_PUA.Add(observaciones);
                }
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtener las observaciones de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public IList<PUObservacionCentroServicioDC> ObtenerObservacionCentroServicio(long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ObservacionCentroServicio_PUA.Where(cs => cs.OCS_IdCentroServicios == idCentroServicio).ToList()
                  .ConvertAll(conv => new PUObservacionCentroServicioDC()
                  {
                      Motivo = conv.OCS_Motivo,
                      Obsevacion = conv.OCS_Obsevacion
                  });
            }
        }

        /// <summary>
        /// Obtiene la informacion de un centro se servicio por el id del centro de servicio
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerInformacionCentroServicioPorId(long idCentroServicios)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentroSvcLocalidadServici_VPUA centroSer = contexto.CentroSvcLocalidadServici_VPUA.FirstOrDefault(cs => cs.CES_IdCentroServicios == idCentroServicios);
                if (centroSer == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CENTRO_SERVICIOS, EnumTipoErrorCentroServicios.EX_AGENCIA_NO_EXISTE_INACTIVA.ToString(), MensajesCentroServicios.CargarMensaje(EnumTipoErrorCentroServicios.EX_AGENCIA_NO_EXISTE_INACTIVA)));
                }
                else
                {
                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = centroSer.CES_IdCentroServicios,
                        IdCentroCostos = centroSer.CES_IdCentroCostos,
                        Barrio = centroSer.CES_Barrio,
                        DigitoVerificacion = centroSer.CES_DigitoVerificacion,
                        Direccion = centroSer.CES_Direccion,
                        Email = centroSer.CES_Email,
                        Estado = centroSer.CES_Estado,
                        Nombre = centroSer.CES_Nombre,
                        Telefono1 = centroSer.CES_Telefono1,
                        Telefono2 = centroSer.CES_Telefono2,
                        Sistematizado = centroSer.CES_Sistematizada,
                        IdMunicipio = centroSer.CES_IdMunicipio,
                        IdPais = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == centroSer.CES_IdMunicipio).Single().LOC_IdAncestroSegundoGrado,
                        NombrePais = contexto.Localidades_VPAR.Where(t => t.LOC_IdLocalidad == centroSer.CES_IdMunicipio).Single().LOC_NombreSegundo,
                        CodigoPostal = centroSer.LOC_CodigoPostal,
                        NombreMunicipio = centroSer.LOC_Nombre,
                        NombreDepto = centroSer.NombreDepartamento,
                        CiudadUbicacion = new PALocalidadDC()
                        {
                            IdLocalidad = centroSer.CES_IdMunicipio,
                            Nombre = centroSer.LOC_Nombre
                        },
                    };
                }
            }
        }

        #endregion Servicio Giros

        /// <summary>
        /// Metodo que consulta todas las agencias y puntos de un RACOL
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        /// 
        //TODO JONATHAN: VALIDAR FUNCIONAMIENTO DE CAMBIO A ADO
        public List<PUCentroServiciosDC> ObtenerAgenciasYPuntosRacolActivos(long idRacol)
        {
            PALocalidadDC localidad = new PALocalidadDC();
            List<PUCentroServiciosDC> lstCentrosServicio = new List<PUCentroServiciosDC>();                 
            using (SqlConnection sqlConn = new SqlConnection(CadCnxController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerAgenciasPuntosPorRacol_PUA", sqlConn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idRegional", idRacol);
                cmd.Parameters.AddWithValue("@tipoAgencia", ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA);
                cmd.Parameters.AddWithValue("@tipoPunto", ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO);
                cmd.Parameters.AddWithValue("@estado", ConstantesFramework.ESTADO_ACTIVO);
                cmd.Parameters.AddWithValue("@aplicaPAM", null);
                DataTable dt = new DataTable();
                sqlConn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                sqlConn.Close();
                var CentrosServicio = dt.AsEnumerable().ToList();

                SqlCommand cmdLocalidad = new SqlCommand("paObtenerNombreCompletoLocalidad_PAR", sqlConn);
                cmdLocalidad.CommandType = CommandType.StoredProcedure;
                cmdLocalidad.Parameters.AddWithValue("@idLocalidad", sqlConn);
                DataTable dtLocalidad = new DataTable();
                sqlConn.Open();
                SqlDataAdapter daLocalidad = new SqlDataAdapter(cmd);
                daLocalidad.Fill(dtLocalidad);
                sqlConn.Close();
                var localidadConsulta = dt.AsEnumerable().FirstOrDefault();
                localidad.NombreCompleto = localidadConsulta.Field<string>("NombreCompleto");
                localidad.IdLocalidad = localidadConsulta.Field<string>("LOC_IdLocalidad");
                localidad.Nombre = localidadConsulta.Field<string>("LOC_Nombre");
                localidad.NombreCorto = localidadConsulta.Field<string>("LOC_NombreCorto");

                if (CentrosServicio != null && CentrosServicio.Any())
                {
                    lstCentrosServicio = CentrosServicio
                        .ConvertAll(r => new PUCentroServiciosDC()
                        {
                            IdCentroServicio = r.Field<long>("CES_IdCentroServicios"),
                            Nombre = r.Field<string>("CES_Nombre"),
                            Tipo = r.Field<string>("CES_Tipo"),
                            IdColRacolApoyo = r.Field<long?>("REA_IdRegionalAdm"),
                            DescripcionRacol = r.Field<string>("REA_Descripcion"),
                            Sistematizado = r.Field<bool>("CES_Sistematizada"),
                            CodigoBodega = r.Field<string>("CES_CodigoBodega"),
                            CiudadUbicacion = localidad,
                            IdCentroCostos = r.Field<string>("CES_IdCentroCostos"),
                            NombreMunicipio = r.Field<string>("LOC_Nombre"),
                            TopeMaximoGiros = r.Field<decimal>("CES_TopeMaximoPorGiros"),
                            TopeMaximoPagos = r.Field<decimal>("CES_TopeMaximoPorPagos"),
                            PagaGiros = r.Field<bool>("CES_PuedePagarGiros"),
                            RecibeGiros = r.Field<bool>("CES_PuedeRecibirGiros"),
                            Telefono1 = r.Field<string>("CES_Telefono1"),
                            Direccion = r.Field<string>("CES_Direccion"),
                            ClasificacionPorIngresos = new PUClasificacionPorIngresosDC()
                            {
                                IdClasificacion = r.Field<string>("CES_ClasGirosPorIngresos")
                            }
                        }
                        );
                }
                return lstCentrosServicio;

            }
        }

        /// <summary>
        /// obtiene el centro de servicio Adscrito a un racol
        /// </summary>
        /// <param name="idRacol"></param>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerCentroServicioAdscritoRacol(long idRacol, long idCentroServicio)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                CentrosServicosRacol_VPUA centroAdscrito = contexto.CentrosServicosRacol_VPUA.FirstOrDefault(cent => cent.REA_IdRegionalAdm == idRacol
                                                           && (cent.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA
                                                           || cent.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_PUNTO)
                                                           && cent.CES_Estado == ConstantesFramework.ESTADO_ACTIVO
                                                           && cent.CES_IdCentroServicios == idCentroServicio);

                PUCentroServiciosDC centro = null;

                if (centroAdscrito != null)
                {
                    centro = new PUCentroServiciosDC()
                   {
                       IdCentroServicio = centroAdscrito.CES_IdCentroServicios,
                       Nombre = centroAdscrito.CES_Nombre,
                       Tipo = centroAdscrito.CES_Tipo,
                       IdColRacolApoyo = centroAdscrito.REA_IdRegionalAdm,
                       DescripcionRacol = centroAdscrito.REA_Descripcion,
                       Sistematizado = centroAdscrito.CES_Sistematizada,
                       CodigoBodega = centroAdscrito.CES_CodigoBodega,
                       CiudadUbicacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                       {
                           IdLocalidad = centroAdscrito.LOC_IdLocalidad,
                           Nombre = centroAdscrito.LOC_Nombre,
                           NombreCorto = centroAdscrito.LOC_NombreCorto,
                           NombreCompleto = contexto.Localidades_VPAR.Where(loc => loc.LOC_IdLocalidad == centroAdscrito.LOC_IdLocalidad).SingleOrDefault().NombreCompleto,
                       },
                       IdCentroCostos = centroAdscrito.CES_IdCentroCostos,
                       NombreMunicipio = centroAdscrito.LOC_Nombre,
                       TopeMaximoGiros = centroAdscrito.CES_TopeMaximoPorGiros,
                       TopeMaximoPagos = centroAdscrito.CES_TopeMaximoPorPagos,
                       PagaGiros = centroAdscrito.CES_PuedePagarGiros,
                       RecibeGiros = centroAdscrito.CES_PuedeRecibirGiros,
                       Telefono1 = centroAdscrito.CES_Telefono1,
                       Direccion = centroAdscrito.CES_Direccion,
                       ClasificacionPorIngresos = new PUClasificacionPorIngresosDC()
                       {
                           IdClasificacion = centroAdscrito.CES_ClasGirosPorIngresos
                       }
                   };
                }
                return centro;
            }
        }

        /// <summary>
        /// Metodo que consulta todas las agencias
        /// </summary>
        /// <param name="idRacol"></param>
        /// <returns></returns>
        public List<PUCentroServiciosDC> ObtenerAgenciasRacolActivos(long idRacol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CentrosServicosRacol_VPUA.Where(cent => cent.REA_IdRegionalAdm == idRacol
                                          && cent.CES_Tipo == ConstantesFramework.TIPO_CENTRO_SERVICIO_AGENCIA
                                          && cent.CES_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .OrderBy(o => o.CES_Nombre)
                  .ToList()
                  .ConvertAll(r =>
                  {
                      PUCentroServiciosDC centro = new PUCentroServiciosDC()
                      {
                          IdCentroServicio = r.CES_IdCentroServicios,
                          Nombre = r.CES_Nombre,
                          Tipo = r.CES_Tipo,
                          IdColRacolApoyo = r.REA_IdRegionalAdm,
                          DescripcionRacol = r.REA_Descripcion,
                          Sistematizado = r.CES_Sistematizada,
                          CodigoBodega = r.CES_CodigoBodega,
                          CiudadUbicacion = new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC()
                          {
                              IdLocalidad = r.LOC_IdLocalidad,
                              Nombre = r.LOC_Nombre,
                              NombreCorto = r.LOC_NombreCorto,
                              NombreCompleto = contexto.Localidades_VPAR.Where(loc => loc.LOC_IdLocalidad == r.LOC_IdLocalidad).SingleOrDefault().NombreCompleto,
                          },
                          IdCentroCostos = r.CES_IdCentroCostos,
                          NombreMunicipio = r.LOC_Nombre,
                          TopeMaximoGiros = r.CES_TopeMaximoPorGiros,
                          TopeMaximoPagos = r.CES_TopeMaximoPorPagos,
                          PagaGiros = r.CES_PuedePagarGiros,
                          RecibeGiros = r.CES_PuedeRecibirGiros,
                          Telefono1 = r.CES_Telefono1,
                          Direccion = r.CES_Direccion,
                          ClasificacionPorIngresos = new PUClasificacionPorIngresosDC()
                          {
                              IdClasificacion = r.CES_ClasGirosPorIngresos
                          }
                      };

                      return centro;
                  });
            }
        }

        /// <summary>
        /// Obtienen todos los municipios de un racol
        /// </summary>
        /// <param name="idRacol">id Racol</param>
        /// <returns>municipios del racol</returns>
        public List<PALocalidadDC> ObtenerMunicipiosDeRacol(long idRacol)
        {
            using (ModeloCentroServicios contexto = new ModeloCentroServicios(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IQueryable<MunicipioRegionalAdm_PUA> consultaDB;
                consultaDB = contexto.MunicipioRegionalAdm_PUA.Include("Localidad_PAR").Where(racol => racol.MRA_IdLocalidadRegionalAdm == idRacol);

                List<PALocalidadDC> localidades = consultaDB.ToList().ConvertAll(
                  municipio =>
                  {
                      PALocalidadDC loc = new PALocalidadDC()
                      {
                          Nombre = municipio.Localidad_PAR.LOC_Nombre,
                          CodigoPostal = municipio.Localidad_PAR.LOC_CodigoPostal,
                          NombreCorto = municipio.Localidad_PAR.LOC_NombreCorto,
                          IdTipoLocalidad = municipio.Localidad_PAR.LOC_IdTipo,
                          IdLocalidad = municipio.Localidad_PAR.LOC_IdLocalidad,
                          NombreCompleto = contexto.Localidades_VPAR.Where(r => r.LOC_IdLocalidad == municipio.MRA_IdMunicipio).FirstOrDefault().NombreCompleto
                      };

                      return loc;
                  });

                return localidades.OrderBy(l => l.Nombre).ToList();
            }
        }

        /// <summary>
        /// obtiene tipo y centro servicio responsable de centro servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ObtenerTipoYResponsableCentroServicio(long idCentroServicio)
        {
            PUCentroServiciosDC centroServicio = null;
            using (SqlConnection conn = new SqlConnection(CadCnxController))
            {

                SqlCommand cmd = new SqlCommand("paObtenerTipoYResponsableCentroServicio_PUA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicios", idCentroServicio);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    if (reader.Read())
                    {
                        centroServicio = new PUCentroServiciosDC()
                        {
                            Tipo = reader["Tipo"].ToString(),
                            IdCentroServicio = Convert.ToInt64(reader["IdCsv"]),
                            infoResponsable = new PUAgenciaDeRacolDC() {
                                IdResponsable = Convert.ToInt64(reader["IdResponsable"].ToString()),
                            },
                            Nombre = reader["Nombre"].ToString()
                        };
                   
                        //centroServicio.Tipo= reader["Tipo"].ToString();
                        //centroServicio.IdCentroServicio= Convert.ToInt64(reader["IdCsv"]);
                        //centroServicio.infoResponsable.IdResponsable = Convert.ToInt64(reader["IdResponsable"].ToString());
                        //centroServicio.Nombre = reader["Nombre"].ToString();
                    }

                }
                conn.Close();
                return centroServicio;
            }
        }
    }
}