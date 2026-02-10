using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Threading.Tasks;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.Collections.ObjectModel;
using System.Data;
using CO.Servidor.Dominio.Comun.Util;
using System.Configuration;

namespace CO.Servidor.Cajas.Datos
{
    public class CARepositorioCaja
    {
        #region Constructor

        private CARepositorioCaja()
        {
            ListaConceptos = ObtenerConceptos();
        }

        #endregion Constructor

        #region Campos

        private static readonly CARepositorioCaja instancia = new CARepositorioCaja();
        private const string NombreModelo = "ModeloCajas";

        #endregion Campos

        #region Propiedades

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        public static CARepositorioCaja Instancia
        {
            get
            {
                return CARepositorioCaja.instancia;
            }
        }

        public List<CAConceptoCajaDC> ListaConceptos { get; set; }

        #endregion Propiedades

        #region Apertura y Cierre de Caja

        /// <summary>
        /// Validar una apertura de caja para un centro de servicios y un usuario
        /// </summary>
        /// <param name="idCaja">Identificación de la caja sobre la cual se hace la operación</param>
        /// <param name="idCodigoUsuario">Código del usuario que hace la operación</param>
        /// <param name="idCentroServicios">Identificación del centro de servicios sobre el cual se hace la operación</param>
        /// <returns>Identificación de la apertura de caja</returns>
        /// <remarks>Cuando la caja es cero (0), se busca alguna apertura anterior sin validar el usuario, esto para evitar multiples aperturas sobre la caja cero</remarks>
        public long ValidarApeturaCentroServicios(int idCaja, long idCodigoUsuario, long idCentroServicios)
        {
            AperturaCentroServicos_VCAJ apertura = null;

            if (idCaja != 0) // validar una apertura previa para la caja, usuario y centro de servicios
            {
                using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
                {
                    apertura = contexto.AperturaCentroServicos_VCAJ
                    .Where(caj => caj.APC_IdCaja == idCaja &&
                                  caj.APC_EstaAbierta == true &&
                                  caj.APC_IdCodigoUsuario == idCodigoUsuario &&
                                  caj.ACS_IdCentroServicios == idCentroServicios)
                    .FirstOrDefault();
                }
            }
            else //validar una apertura previa para la caja cero y el centro de servicios
            {
                using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
                {
                    apertura = contexto.AperturaCentroServicos_VCAJ
                    .Where(caj => caj.APC_IdCaja == 0 &&
                                  caj.APC_EstaAbierta == true &&
                                  caj.ACS_IdCentroServicios == idCentroServicios)
                    .FirstOrDefault();
                }
            }

            return apertura == null ? 0 : apertura.APC_IdAperturaCaja;
        }


        /// <summary>
        /// Validar una apertura de caja para un centro de servicios y un usuario
        /// </summary>
        /// <param name="idCaja">Identificación de la caja sobre la cual se hace la operación</param>
        /// <param name="idCodigoUsuario">Código del usuario que hace la operación</param>
        /// <param name="idCentroServicios">Identificación del centro de servicios sobre el cual se hace la operación</param>
        /// <returns>Identificación de la apertura de caja</returns>
        /// <remarks>Cuando la caja es cero (0), se busca alguna apertura anterior sin validar el usuario, esto para evitar multiples aperturas sobre la caja cero</remarks>
        public long ValidarApeturaCentroServicios(int idCaja, long idCodigoUsuario, long idCentroServicios, SqlConnection conexion, SqlTransaction transaccion)
        {
            AperturaCentroServicos_VCAJ apertura = null;

            if (idCaja != 0) // validar una apertura previa para la caja, usuario y centro de servicios
            {

                string cmdText = @" SELECT *
                                    FROM AperturaCentroServicos_VCAJ
                                    WHERE APC_IdCaja = @APC_IdCaja
                                          AND APC_EstaAbierta = 1
                                          AND APC_IdCodigoUsuario = @APC_IdCodigoUsuario
                                          AND ACS_IdCentroServicios = @ACS_IdCentroServicios";
                SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
                cmd.Parameters.Add(Utilidades.AddParametro("@APC_IdCaja", idCaja));
                cmd.Parameters.Add(Utilidades.AddParametro("@APC_IdCodigoUsuario", idCodigoUsuario));
                cmd.Parameters.Add(Utilidades.AddParametro("@ACS_IdCentroServicios", idCentroServicios));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var aper = dt.AsEnumerable().FirstOrDefault();

                if (aper != null)
                {
                    apertura = new AperturaCentroServicos_VCAJ()
                    {
                        APC_IdAperturaCaja = aper.Field<long>("APC_IdAperturaCaja")
                    };
                }

            }
            else //validar una apertura previa para la caja cero y el centro de servicios
            {

                string cmdText = @" SELECT *
                                    FROM AperturaCentroServicos_VCAJ
                                    WHERE APC_IdCaja = 0
                                          AND APC_EstaAbierta = 1                                          
                                          AND ACS_IdCentroServicios = @ACS_IdCentroServicios";
                SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
                cmd.Parameters.Add(Utilidades.AddParametro("@APC_IdCaja", idCaja));
                cmd.Parameters.Add(Utilidades.AddParametro("@ACS_IdCentroServicios", idCentroServicios));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                var aper = dt.AsEnumerable().FirstOrDefault();

                if (aper != null)
                {
                    apertura = new AperturaCentroServicos_VCAJ()
                    {
                        APC_IdAperturaCaja = aper.Field<long>("APC_IdAperturaCaja")
                    };
                }
            }

            return apertura == null ? 0 : apertura.APC_IdAperturaCaja;
        }


        /// <summary>
        /// Validar apertura en cajas de gestión: Casa Matriz, Operación Nacional y Banco
        /// </summary>
        /// <param name="idCodigoUsuario">Código del usuario que hace la operación</param>
        /// <param name="idCasaMatriz">Identificador de la casa matriz desde donde se hace la operación</param>
        /// <param name="tipoApertura">Tipo de apertura que se desaea hacer: Casa Matriz, Operación Nacional o Banco</param>
        /// <returns>Identificador de la apertura, 0 si no se encuentra ninguna apertura de caja Abierta</returns>
        public long ValidarApeturaCajaGestion(long idCodigoUsuario, short idCasaMatriz, CAEnumTipoApertura tipoApertura)
        {
            AperturaCasaMatriz_VCAJ apertura = null;
            string tipoAperturaCaja = tipoApertura.ToString();

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                apertura = contexto.AperturaCasaMatriz_VCAJ
                .Where(caj => caj.APC_IdCaja == 0 &&
                              caj.APC_EstaAbierta == true &&
                              caj.APC_IdCodigoUsuario == idCodigoUsuario &&
                              caj.ACM_IdCasaMatriz == idCasaMatriz &&
                              caj.APC_TipoApertura == tipoAperturaCaja)
                .FirstOrDefault();
            }

            return apertura == null ? 0 : apertura.APC_IdAperturaCaja;
        }

        /// <summary>
        /// Adicionar la apertura a caja de centro servicios.
        /// </summary>
        /// <param name="infoCaja">Objeto con información de la caja que se desea abrir.</param>
        /// <param name="idCentroServicio">Id centro servicio.</param>
        /// <param name="nombreCentroServicio">Nombre centro servicio.</param>
        public void AdicionarAperturaCentroServicios(CAAperturaCajaDC infoCaja, long idCentroServicio,
                                                      string nombreCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                AperturaCajaCentroServicios_CAJ AperCajaCentroServicios = new AperturaCajaCentroServicios_CAJ()
                {
                    ACS_IdAperturaCajaCentroServicios = infoCaja.IdAperturaCaja,
                    ACS_IdCentroServicios = idCentroServicio,
                    ACS_NombreCentroServicios = nombreCentroServicio,
                    ACS_FechaGrabacion = DateTime.Now,
                    ACS_CreadoPor = infoCaja.CreadoPor ?? ControllerContext.Current.Usuario,
                };
                contexto.AperturaCajaCentroServicios_CAJ.Add(AperCajaCentroServicios);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adicionar la apertura a caja de centro servicios.
        /// </summary>
        /// <param name="infoCaja">Objeto con información de la caja que se desea abrir.</param>
        /// <param name="idCentroServicio">Id centro servicio.</param>
        /// <param name="nombreCentroServicio">Nombre centro servicio.</param>
        public void AdicionarAperturaCentroServicios(CAAperturaCajaDC infoCaja, long idCentroServicio, string nombreCentroServicio, SqlConnection conexion, SqlTransaction transaccion)
        {

            string cmdText = @"INSERT INTO [AperturaCajaCentroServicios_CAJ]
                                           ([ACS_IdAperturaCajaCentroServicios]
                                           ,[ACS_IdCentroServicios]
                                           ,[ACS_NombreCentroServicios]
                                           ,[ACS_FechaGrabacion]
                                           ,[ACS_CreadoPor])
                                     VALUES
                                           (@ACS_IdAperturaCajaCentroServicios
                                           ,@ACS_IdCentroServicios
                                           ,@ACS_NombreCentroServicios
                                           ,GETDATE()
                                           ,@ACS_CreadoPor)";
            SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);

            cmd.Parameters.Add(Utilidades.AddParametro("@ACS_IdAperturaCajaCentroServicios", infoCaja.IdAperturaCaja));
            cmd.Parameters.Add(Utilidades.AddParametro("@ACS_IdCentroServicios", idCentroServicio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ACS_NombreCentroServicios", nombreCentroServicio));
            cmd.Parameters.Add(Utilidades.AddParametro("@ACS_CreadoPor", infoCaja.CreadoPor ?? ControllerContext.Current.Usuario));

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Adicionar la apertura a la caja casa matriz.
        /// </summary>
        /// <param name="infoCaja">Objeto con información de la caja que se desea abrir.</param>
        /// <param name="idCentroServicio">Id centro servicio.</param>
        public void AdicionarAperturaCajaCasaMatriz(CAAperturaCajaDC infoCaja, short idCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                AperturaCajaCasaMatriz_CAJ AperCajaCasaMatriz = new AperturaCajaCasaMatriz_CAJ()
                {
                    ACM_IdAperturaCaja = infoCaja.IdAperturaCaja,
                    ACM_IdCasaMatriz = idCentroServicio,
                    ACM_FechaGrabacion = DateTime.Now,
                    ACM_CreadoPor = infoCaja.CreadoPor ?? ControllerContext.Current.Usuario
                };
                contexto.AperturaCajaCasaMatriz_CAJ.Add(AperCajaCasaMatriz);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona la Apertura de una caja.
        /// </summary>
        /// <param name="infoCaja">The info caja.</param>
        /// <param name="contexto">The contexto.</param>
        /// <returns></returns>
        public long AdicionarAperturaCaja(CAAperturaCajaDC infoCaja, CAEnumTipoApertura tipoApertura)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                AperturaCaja_CAJ AperturaCaja = new AperturaCaja_CAJ()
                {
                    APC_IdCaja = infoCaja.IdCaja,
                    APC_EstaAbierta = true,

                    //Se valida que el Id de la Caja sea el del principal para asignarle la base
                    //de la Caja del centro de centro de servicio
                    APC_BaseInicialApertura = infoCaja.IdCaja == 0 ? infoCaja.BaseInicialApertura : 0,
                    APC_FechaGrabacion = DateTime.Now,
                    APC_CreadoPor = infoCaja.CreadoPor ?? ControllerContext.Current.Usuario,
                    APC_IdCodigoUsuario = infoCaja.IdCodigoUsuario,
                    APC_TipoApertura = tipoApertura.ToString(),
                };
                contexto.AperturaCaja_CAJ.Add(AperturaCaja);
                contexto.SaveChanges();

                return AperturaCaja.APC_IdAperturaCaja;
            }
        }

        /// <summary>
        /// Adiciona la Apertura de una caja.
        /// </summary>
        /// <param name="infoCaja">The info caja.</param>
        /// <param name="contexto">The contexto.</param>
        /// <returns></returns>
        public long AdicionarAperturaCaja(CAAperturaCajaDC infoCaja, CAEnumTipoApertura tipoApertura, SqlConnection conexion, SqlTransaction transaccion)
        {
            string cmdText = @"INSERT INTO [AperturaCaja_CAJ]
                                           ([APC_IdCaja]
                                           ,[APC_EstaAbierta]
                                           ,[APC_BaseInicialApertura]
                                           ,[APC_FechaGrabacion]
                                           ,[APC_CreadoPor]
                                           ,[APC_IdCodigoUsuario]
                                           ,[APC_TipoApertura])
                                     VALUES
                                           (@APC_IdCaja
                                           ,@APC_EstaAbierta
                                           ,@APC_BaseInicialApertura
                                           ,GETDATE()
                                           ,@APC_CreadoPor
                                           ,@APC_IdCodigoUsuario
                                           ,@APC_TipoApertura)

                                    SELECT SCOPE_IDENTITY()";

            SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(Utilidades.AddParametro("@APC_IdCaja", infoCaja.IdCaja));
            cmd.Parameters.Add(Utilidades.AddParametro("@APC_EstaAbierta", true));
            cmd.Parameters.Add(Utilidades.AddParametro("@APC_BaseInicialApertura", infoCaja.IdCaja == 0 ? infoCaja.BaseInicialApertura : 0));
            cmd.Parameters.Add(Utilidades.AddParametro("@APC_CreadoPor", infoCaja.CreadoPor ?? ControllerContext.Current.Usuario));
            cmd.Parameters.Add(Utilidades.AddParametro("@APC_IdCodigoUsuario", infoCaja.IdCodigoUsuario));
            cmd.Parameters.Add(Utilidades.AddParametro("@APC_TipoApertura", tipoApertura.ToString()));

            return Convert.ToInt64(cmd.ExecuteScalar());
        }


        /// <summary>
        /// Cierra la caja del Auxiliar por el codigo.
        /// </summary>
        /// <param name="idApertura">Es el id de la apertura.</param>
        /// <param name="idPunto">Es el id del punto.</param>
        public void CerrarCajaAux(long idCodigoUsuario, long idApertura, long idPunto, int idCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                AperturaCaja_CAJ updateCaja = contexto.AperturaCaja_CAJ.FirstOrDefault(up => up.APC_IdAperturaCaja == idApertura);

                if (updateCaja != null && updateCaja.APC_EstaAbierta == true)
                {
                    decimal IngresosEfectivo = 0, EgresosEfectivo = 0, IngresosOtrasFormas = 0, EgresosOtrasFormas = 0;
                    decimal ComisionVenta = 0, ComisionResponsable = 0;

                    string User = null;

                    if (idCodigoUsuario == ConstantesFramework.ID_USUARIO_SISTEMA)
                        User = ConstantesFramework.USUARIO_SISTEMA;
                    else
                        User = ControllerContext.Current.Usuario;

                    List<ObtenerInfoCierreCaja_CAJ> TotalIngresosEgresosEfectivo = contexto.paObtenerInfoCierreCaja_CAJ(idApertura, CAConstantesCaja.FORMA_PAGO_EFECTIVO, CAConstantesCaja.OPERADOR_LOGICO_IGUAL).ToList();

                    if (TotalIngresosEgresosEfectivo.Count > 0)
                    {
                        IngresosEfectivo = TotalIngresosEgresosEfectivo.FirstOrDefault(r => r.APC_IdAperturaCaja == idApertura).Ingresos;
                        EgresosEfectivo = TotalIngresosEgresosEfectivo.FirstOrDefault(r => r.APC_IdAperturaCaja == idApertura).Egresos;
                        User = TotalIngresosEgresosEfectivo.FirstOrDefault(r => r.APC_IdAperturaCaja == idApertura).APC_CreadoPor;
                    }

                    List<ObtenerInfoCierreCaja_CAJ> TotalIngresosEgresosOtrasFormas = contexto.paObtenerInfoCierreCaja_CAJ(idApertura, CAConstantesCaja.FORMA_PAGO_EFECTIVO, CAConstantesCaja.OPERADOR_LOGICO_DIFERENCIA).ToList();

                    if (TotalIngresosEgresosOtrasFormas.Count > 0)
                    {
                        IngresosOtrasFormas = TotalIngresosEgresosOtrasFormas.FirstOrDefault(r => r.APC_IdAperturaCaja == idApertura).Ingresos;
                        EgresosOtrasFormas = TotalIngresosEgresosOtrasFormas.FirstOrDefault(r => r.APC_IdAperturaCaja == idApertura).Egresos;
                    }

                    List<ComisionesVentasApertura_CAJ> TotalComisiones = contexto.paObtenerComisionesVentasApertura_CAJ(idApertura)
                      .ToList();

                    if (TotalComisiones.Count > 0)
                    {
                        ComisionVenta = TotalComisiones.FirstOrDefault(r => r.RTC_IdAperturaCaja == idApertura).TotalComisionVenta.Value;
                        ComisionResponsable = TotalComisiones.FirstOrDefault(r => r.RTC_IdAperturaCaja == idApertura).TotalComisionResponsable.Value;
                    }

                    updateCaja.APC_EstaAbierta = false;

                    CierreCaja_CAJ CerrarCaja = new CierreCaja_CAJ()
                    {
                        CIC_IdCierreCaja = idApertura,
                        CIC_TotalIngresosEfectivo = IngresosEfectivo,
                        CIC_TotalEgresosEfectivo = EgresosEfectivo,
                        CIC_TotalIngresosOtrasFormas = IngresosOtrasFormas,
                        CIC_TotalEgresosOtrasFormas = EgresosOtrasFormas,
                        CIC_FechaGrabacion = DateTime.Now,
                        CIC_CreadoPor = User,
                        CIC_IdCierrepuntoAsociado = 0
                    };
                    contexto.CierreCaja_CAJ.Add(CerrarCaja);
                    contexto.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Cierra la Caja del Punto ó Centro de Servicio y actualiza el Id de Cierre de Punto en las Cajas respectivas como reportado.
        /// </summary>
        /// <param name="cierrePuntoCentroServicio"></param>
        /// <param name="esCierreAutomatico">Indica si es cierre automático</param>
        public long CerrarCajaPuntoCentroServcio(long idCentroServicio)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {

                SqlCommand cmd = new SqlCommand(@"paCerrarCentroServicio_CAJ", conn);
                cmd.CommandTimeout = 300;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCentroServicios", idCentroServicio);
                cmd.Parameters.AddWithValue("@Automatico", 0);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@Masivo", 0);
                conn.Open();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Adiciona un registro de cierre centro de servicios
        /// </summary>
        /// <param name="cierrePuntoCentroServicio">Información del cierre de centro de servicios</param>
        /// <returns>Identificador del cierre de centro de servicios recien creado</returns>
        public long AdicionarCierreCentroServcio(CierreCentroServicios_CAJ cierrePuntoCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.CierreCentroServicios_CAJ
                  .Add(cierrePuntoCentroServicio);

                contexto.SaveChanges();

                return cierrePuntoCentroServicio.CCS_IdCierreCentroServicios;
            }
        }

        /// <summary>
        /// Actualizo el cierre Asociado
        /// </summary>
        /// <param name="cierrePuntoCentroServicio"></param>
        /// <param name="contexto"></param>
        /// <param name="NvCierre"></param>
        public void ActualizarCierreAsociado(CACierreCentroServicioDC cierrePuntoCentroServicio, CierreCentroServicios_CAJ NvCierre)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                if (cierrePuntoCentroServicio.CajasPuntoReportadas != null)
                {
                    cierrePuntoCentroServicio.CajasPuntoReportadas
                      .ToList()
                      .ForEach(r =>
                      {
                          //Proceso para el cerrado de la caja Cero
                          AperturaCaja_CAJ CajaCero = contexto.AperturaCaja_CAJ.FirstOrDefault(apertura => apertura.APC_IdAperturaCaja == r.IdCierreCaja);
                          if (CajaCero != null)
                          {
                              if (CajaCero.APC_EstaAbierta == true && CajaCero.APC_IdCaja == 0)
                              {
                                  CerrarCajaAux(CajaCero.APC_IdCodigoUsuario, r.IdCierreCaja, NvCierre.CCS_IdCierreCentroServicios, CajaCero.APC_IdCaja);
                                  ReportarCajaACajeroPrincipal(r.IdCierreCaja);
                              }

                              else
                              {
                                  CierreCaja_CAJ idCierreCaja = contexto.CierreCaja_CAJ.FirstOrDefault(e => e.CIC_IdCierreCaja == r.IdCierreCaja);

                                  idCierreCaja.CIC_IdCierrepuntoAsociado = NvCierre.CCS_IdCierreCentroServicios;

                                  contexto.SaveChanges();
                              }
                          }
                      }
                    );
                }
            }
        }

        public CACierreCajaDC ObtenerInfoCierreCaja(long idCierreCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.AperturaCaja_CAJ.Include("CierreCaja_CAJ").Where(caja => caja.APC_IdAperturaCaja == idCierreCaja)
                                              .ToList()
                                              .ConvertAll<CACierreCajaDC>(caj => new CACierreCajaDC()
                                              {
                                                  IdCierreCaja = caj.APC_IdAperturaCaja,
                                                  TotalIngresoEfectivo = caj.CierreCaja_CAJ.CIC_TotalIngresosEfectivo,
                                                  TotalEgresoEfectivo = caj.CierreCaja_CAJ.CIC_TotalEgresosEfectivo,
                                                  TotalIngresoOtrasFormas = caj.CierreCaja_CAJ.CIC_TotalIngresosOtrasFormas,
                                                  TotalEgresoOtrasFormas = caj.CierreCaja_CAJ.CIC_TotalEgresosOtrasFormas,
                                                  FechaCierre = caj.CierreCaja_CAJ.CIC_FechaGrabacion,
                                                  CreadoPor = caj.CierreCaja_CAJ.CIC_CreadoPor,
                                                  IdCaja = caj.APC_IdCaja,
                                                  idUsuario = caj.APC_IdCodigoUsuario,
                                                  UsuarioCaja = caj.APC_CreadoPor
                                              }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Metodo que obtiene el resumen de las aperturas de caja no cerradas por col de los mensajeros
        /// </summary>
        /// <param name="idCol"></param>
        public List<CACierreCajaDC> ObtenerResumenMensajerosCOL(long idCol)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerResumenMensajerosCOL_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idCol", idCol);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<CACierreCajaDC>(c =>
                {
                    return new CACierreCajaDC
                    {
                        IdCierreCaja = c.Field<long>("APC_IdAperturaCaja"),
                        IdCaja = c.Field<int>("APC_IdCaja"),
                        FechaApertura = c.Field<DateTime>("APC_FechaGrabacion"),
                        TotalIngresoEfectivo = c.Field<short>("RVF_IdFormaPago") == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<int>("Ingresos") : 0,
                        TotalEgresoEfectivo = c.Field<short>("RVF_IdFormaPago") == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<int>("Egresos") : 0,
                        TotalReportar = c.Field<short>("RVF_IdFormaPago") == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<int>("Ingresos") - c.Field<int>("Egresos") : 0,
                        UsuarioCaja = c.Field<string>("nombreCompleto"),
                        IdPuntoAtencion = c.Field<long>("ACS_IdCentroServicios"),
                        FechaCierre = null,
                    };
                });
            }
        }


        public void AsociarCierreCajaConCierreCentroSvc(long idCierreCaja, long idCierreCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CierreCaja_CAJ cierreCaja = contexto.CierreCaja_CAJ
                  .Where(w => w.CIC_IdCierreCaja == idCierreCaja)
                  .FirstOrDefault();

                if (cierreCaja != null)
                {
                    cierreCaja.CIC_EstaReportado = true;
                    cierreCaja.CIC_IdCierrepuntoAsociado = idCierreCentroServicio;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtener el resumen de valores de las transacciones de una recolección de dinero de un punto
        /// </summary>
        /// <param name="idRecoleccionPunto">Identificador de la recolección de dinero del punto</param>
        /// <returns>Resumen de los valores de las transacciones de la recolección de dinero</returns>
        public ResumenRecoleccionDineroPuntos_VCAJ ObtenerResumenRecoleccionPunto(long idRecoleccionPunto)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ResumenRecoleccionDineroPuntos_VCAJ
                  .Where(w => w.RDP_IdRecoleccion == idRecoleccionPunto)
                  .FirstOrDefault();
            }
        }

        #endregion Apertura y Cierre de Caja

        #region Consultas

        #region Consulta Cierre Puntos

        /// <summary>
        /// Obtener el resumen consolidado de un cierre para un centro de servicios
        /// </summary>
        /// <param name="idApertura">Identifación de la apertura para consultar las transacciones de caja</param>
        /// <param name="idPuntoCentroServicio">Identificador del centro de servicios sobre el cual se hace el cierre</param>
        /// <param name="idCierrePuntoAsociado">Identificdor del cierre asociado en el caso que se esté consultando uno</param>
        /// <param name="baseInicialCentroServicio">Base inicial con el cual se abrió el centro de servicios</param>
        /// <returns>Consolidado con la información resumida de las transacciones de una apertura o de un cierre de centro de serivcios </returns>
        public CAConsolidadoCierreDC ObtenerResumenConsolidadoCierre(long idApertura,
          long idPuntoCentroServicio,
          long idCierrePuntoAsociado,
          decimal baseInicialCentroServicio)
        {
            // conseguir el resumen de las transacciones

            List<CAResumenCierreCajaPrincipalDC> cierreAgrupadoConcepto;
            List<CAResumenCierreCajaPrincipalFormaPagoDC> formasPagoAgrupado;
            decimal SaldoFinalAnteriorCentroServicio;
            CAConsolidadoCierreDC resumen = new CAConsolidadoCierreDC();

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                //verifica el tipo de consulta si es de cierre anterior o de un nuevo cierre
                if (idCierrePuntoAsociado == 0)
                {
                    SaldoFinalAnteriorCentroServicio = ObtenerUltimoCierrePunto(idPuntoCentroServicio).SaldoAnteriorEfectivo;
                }
                else
                {
                    SaldoFinalAnteriorCentroServicio = ObtenerCierrePuntoSaldoFinal(idPuntoCentroServicio, idCierrePuntoAsociado).SaldoAnteriorEfectivo;
                }

                // 1. obtener resumen agrupando por concepto incluidas cierres de cajas auxiliares
                cierreAgrupadoConcepto = contexto.paObtenerResumenTransccionesConcepto_CAJ(idApertura, idCierrePuntoAsociado, idPuntoCentroServicio)
                  .ToList()
                  .ConvertAll<CAResumenCierreCajaPrincipalDC>(e => new CAResumenCierreCajaPrincipalDC
                  {
                      IdApertura = idApertura,
                      IdCierreAsociado = idCierrePuntoAsociado,
                      ValorIngreso = e.TotalValorIngresoContado.Value,
                      ValorEgreso = e.TotalValorEgresoContado.Value,
                      Cantidad = e.CantidadTransacciones.Value,
                      IdConceptoCaja = e.IdConceptoCaja,
                      NombreConceptoCaja = e.NombreConcepto
                  });

                // 2. conseguir el resumen de las transacciones por forma de pago
                formasPagoAgrupado = contexto.paObtenerResumenCajaPorFormaPago_CAJ(idApertura, idCierrePuntoAsociado, idPuntoCentroServicio)
                  .ToList()
                  .ConvertAll<CAResumenCierreCajaPrincipalFormaPagoDC>(e => new CAResumenCierreCajaPrincipalFormaPagoDC
                  {
                      IdApertura = idApertura,
                      IdCierreAsociado = idCierrePuntoAsociado,
                      IdFormaPago = e.IdFormaPago,
                      NombreFormaPago = e.DescripcionFormaPago,
                      TotalValorIngreso = e.ValorTotalIngreso.Value,
                      TotalValorEgreso = e.ValorTotalEgreso.Value,
                      Cantidad = e.CantidadRegistros.Value
                  });

                // cargar las cierres de cajas auxiliares que estan cerradas y reportadas a caja principal,
                // pero aún no reportadas para un cierre de centro de servicios
                var cierresAuxiliares = contexto.CierreCaja_CAJ // source
                  .Join(
                    contexto.AperturaCaja_CAJ, // target
                    d => d.CIC_IdCierreCaja,   // PK
                    i => i.APC_IdAperturaCaja, // FK
                    (a, b) => new { Cierre = a, Apertura = b })
                  .Where(w => w.Apertura.APC_EstaAbierta == false && w.Apertura.APC_IdCaja != 0 &&
                        w.Cierre.CIC_EstaReportado == true && w.Cierre.CIC_IdCierrepuntoAsociado == 0 &&
                        w.Apertura.AperturaCajaCentroServicios_CAJ.ACS_IdCentroServicios == idPuntoCentroServicio)
                  .ToList();

                resumen.CierresCajaAuxiliaresReportadas = new List<long>(cierresAuxiliares.Count);

                foreach (var item in cierresAuxiliares)
                {
                    resumen.CierresCajaAuxiliaresReportadas.Add(item.Cierre.CIC_IdCierreCaja);
                }
            }

            resumen.ConceptosAgrupados = cierreAgrupadoConcepto;
            resumen.FormasPagoAgrupadas = formasPagoAgrupado;
            resumen.SaldoAnteriorEfectivo = SaldoFinalAnteriorCentroServicio;
            resumen.SaldoInicial = baseInicialCentroServicio;

            return resumen;
        }

        /// <summary>
        /// Obtiene el resumen por concepto de caja de un punto o centro de servicio para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id del punto ó centro de servicio.</param>
        /// <returns>la lista de los conceptos agrupados y totalizados para cerrar el punto</returns>
        public List<CAResumeCierrePuntoDC> ObtenerResumenCierrePunto(decimal baseInicialCentroServicio, long idPuntoCentroServicio,
                                                                      long idCierrePuntoAsociado, bool cierreAutomatico = false)
        {
            List<ResumeCierrePunto_CAJ> Cierres = new List<ResumeCierrePunto_CAJ>();
            decimal SaldoFinalAnteriorCentroServicio;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                //verifica el tipo de consulta si es de cierre anterior o de un nuevo cierre
                if (idCierrePuntoAsociado == 0)
                {
                    SaldoFinalAnteriorCentroServicio = ObtenerUltimoCierrePunto(idPuntoCentroServicio).SaldoAnteriorEfectivo;
                }
                else
                {
                    SaldoFinalAnteriorCentroServicio = ObtenerCierrePuntoSaldoFinal(idPuntoCentroServicio, idCierrePuntoAsociado).SaldoAnteriorEfectivo;
                }

                //consulta la data para el resumen
                Cierres = contexto.paObtenerResumeCierrePunto_CAJ(idPuntoCentroServicio, idCierrePuntoAsociado).ToList();

                return LlenarDataCierrePunto(baseInicialCentroServicio, idCierrePuntoAsociado, SaldoFinalAnteriorCentroServicio, Cierres, contexto, idPuntoCentroServicio);
            }
        }

        /// <summary>
        /// Metodo que llena la data del cierre del centro de servicio
        /// </summary>
        /// <param name="baseInicialCentroServicio"></param>
        /// <param name="idCierrePuntoAsociado"></param>
        /// <param name="Resumen"></param>
        /// <param name="SaldoFinalAnteriorCentroServicio"></param>
        /// <param name="Cierres"></param>
        /// <returns>lista de data para cerrar el punto </returns>
        private static List<CAResumeCierrePuntoDC> LlenarDataCierrePunto(decimal baseInicialCentroServicio, long idCierrePuntoAsociado,
                                                                          decimal SaldoFinalAnteriorCentroServicio, List<ResumeCierrePunto_CAJ> Cierres, ModeloCajas contextoDb, long idPuntoCentroServicio)
        {
            List<CAResumeCierrePuntoDC> Resumen = new List<CAResumeCierrePuntoDC>();

            if (Cierres.Count != 0)
            {
                Cierres.ForEach(ci =>
                {
                    Resumen = Cierres
                     .ConvertAll<CAResumeCierrePuntoDC>(resumeCierre => new CAResumeCierrePuntoDC()
                     {
                         BaseComision = resumeCierre.BaseComision.Value,
                         BaseInicial = baseInicialCentroServicio,
                         Cantidad = resumeCierre.Cantidad.Value,
                         idCierreAsociado = resumeCierre.IdCierrePuntoAsociado,

                         Egreso = resumeCierre.RTD_ConceptoEsIngreso == false ? resumeCierre.VrAdicionales.Value + resumeCierre.VrPrimaSeguros.Value
                                                                               + resumeCierre.VrTercero.Value + resumeCierre.BaseComision.Value +
                                                                               resumeCierre.VrImpuestos.Value - resumeCierre.VrRetenciones.Value : 0,
                         EsIngreso = resumeCierre.RTD_ConceptoEsIngreso,
                         IdCierreCaja = resumeCierre.IdCierreCaja,
                         IdConceptoCaja = resumeCierre.RTD_IdConceptoCaja,
                         IdFormaPago = resumeCierre.RVF_IdFormaPago,
                         IdPuntoAtencion = resumeCierre.IdPunto,
                         Ingreso = resumeCierre.RTD_ConceptoEsIngreso == true ? resumeCierre.VrAdicionales.Value + resumeCierre.VrPrimaSeguros.Value
                                                                               + resumeCierre.VrTercero.Value + resumeCierre.BaseComision.Value +
                                                                               resumeCierre.VrImpuestos.Value - resumeCierre.VrRetenciones.Value : 0,
                         NombreConceptoCaja = resumeCierre.RTD_NombreConcepto,
                         SaldoAnteriorEfectivo = SaldoFinalAnteriorCentroServicio,
                         ValorFormaPago = resumeCierre.VrFormaPago.Value,
                         NombreFormaPago = resumeCierre.RVF_DescripcionFormaPago,
                         InfoGirosNoPagosCentroSvc = new Servicios.ContratoDatos.Admisiones.Giros.Pagos.PGTotalPagosDC(),

                         IdCaja = resumeCierre.IdCaja,
                         EstaReportado = (bool)resumeCierre.EstaReportado
                     });
                });
            }
            else
            {
                Resumen = new List<CAResumeCierrePuntoDC>();
                CAResumeCierrePuntoDC resum =
                  new CAResumeCierrePuntoDC
                  {
                      BaseComision = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      BaseInicial = baseInicialCentroServicio,
                      Cantidad = Convert.ToInt16(CAConstantesCaja.VALOR_CERO),
                      Egreso = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      EsIngreso = true,
                      IdCierreCaja = CAConstantesCaja.VALOR_CERO_LONG,
                      IdConceptoCaja = Convert.ToInt16(CAConstantesCaja.VALOR_CERO),
                      IdFormaPago = TAConstantesServicios.ID_FORMA_PAGO_CONTADO,
                      IdPuntoAtencion = CAConstantesCaja.VALOR_CERO_LONG,
                      Ingreso = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      NombreConceptoCaja = string.Empty,
                      SaldoAnteriorEfectivo = SaldoFinalAnteriorCentroServicio,
                      TotalComisionAgenciaResponsable = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      TotalComisionCentroServicios = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      TotalComisionEmpresa = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      ValorFormaPago = CAConstantesCaja.VALOR_CERO_DECIMAL,
                      NombreFormaPago = string.Empty,
                      InfoGirosNoPagosCentroSvc = new Servicios.ContratoDatos.Admisiones.Giros.Pagos.PGTotalPagosDC()
                  };
                if (idCierrePuntoAsociado == 0)
                {
                    long? idCierre = contextoDb.paObtUltAperCaj0NoCerrada_CAJ(idPuntoCentroServicio).FirstOrDefault();
                    resum.IdCierreCaja = idCierre.HasValue ? idCierre.Value : 0;
                }
                Resumen.Add(resum);
            }
            return Resumen;
        }

        /// <summary>
        /// Obtiene el último cierre del punto.
        /// </summary>
        /// <returns></returns>
        public CACierreCentroServicioDC ObtenerUltimoCierrePunto(long idPuntoCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CierreCentroServicios_CAJ ultimo = contexto.CierreCentroServicios_CAJ
                  .Where(ci => ci.CCS_IdCentroServicios == idPuntoCentroServicio)
                  .OrderByDescending(cierre => cierre.CCS_FechaGrabacion)
                  .FirstOrDefault();

                CACierreCentroServicioDC ultimoCierre;

                if (ultimo != null)
                {
                    ultimoCierre = new CACierreCentroServicioDC
                    {
                        BaseInicial = ultimo.CCS_BaseInicial,
                        IdCentroServicio = ultimo.CCS_IdCentroServicios,
                        SaldoAnteriorEfectivo = ultimo.CCS_SaldoFinalEfectivo.Value,
                        SaldoFinalEfectivo = ultimo.CCS_SaldoFinalEfectivo.Value,
                        TotalEgresosEfectivo = ultimo.CCS_TotalEgresosEfectivo,
                        TotalIngresosEfectivo = ultimo.CCS_TotalIngresosEfectivo,
                        TotalIngresosOtrasFormas = ultimo.CCS_TotalIngresosOtrasFormas,
                        TotalEgresosOtrasFormas = ultimo.CCS_TotalEgresosOtrasFormas,
                        IdCierreCentroServicio = ultimo.CCS_IdCierreCentroServicios,
                        UsuarioCierraPunto = ultimo.CCS_CreadoPor,
                        FechaCierre = ultimo.CCS_FechaGrabacion,
                        InfoGirosNoPagosCentroSvc = new PGTotalPagosDC()
                        {
                            CantidadPagos = ultimo.CCS_CantidadGirosNoPagados,
                            SumatoriaPagos = ultimo.CCS_ValorGirosNoPagados
                        }
                    };
                }
                else
                {
                    ultimoCierre = new CACierreCentroServicioDC()
                    {
                        BaseInicial = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        IdCentroServicio = idPuntoCentroServicio,
                        SaldoAnteriorEfectivo = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        SaldoFinalEfectivo = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        TotalEgresosEfectivo = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        TotalIngresosEfectivo = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        TotalIngresosOtrasFormas = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        TotalEgresosOtrasFormas = CAConstantesCaja.VALOR_CERO_DECIMAL,
                        FechaCierre = ConstantesFramework.MinDateTimeController,
                        UsuarioCierraPunto = ControllerContext.Current != null ? ControllerContext.Current.Usuario : ConstantesFramework.USUARIO_SISTEMA,
                        IdCierreCentroServicio = 0,
                        InfoGirosNoPagosCentroSvc = new PGTotalPagosDC()
                        {
                            CantidadPagos = 0,
                            SumatoriaPagos = 0
                        }
                    };
                }
                return ultimoCierre;
            }
        }

        /// <summary>
        /// Obtiene el cierre consultado
        /// </summary>
        /// <param name="idPuntoCentroServicio">The id punto centro servicio.</param>
        /// <param name="idCierre">The id cierre.</param>
        /// <returns></returns>
        private CACierreCentroServicioDC ObtenerCierrePuntoSaldoFinal(long idPuntoCentroServicio, long idCierre)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CierreCentroServicios_CAJ cierre = contexto.CierreCentroServicios_CAJ
                  .FirstOrDefault(ci => ci.CCS_IdCentroServicios == idPuntoCentroServicio
                                            && ci.CCS_IdCierreCentroServicios == idCierre);

                if (cierre != null)
                {
                    CACierreCentroServicioDC cierresPunto = new CACierreCentroServicioDC()
                    {
                        BaseInicial = cierre.CCS_BaseInicial,
                        IdCentroServicio = cierre.CCS_IdCentroServicios,
                        SaldoAnteriorEfectivo = cierre.CCS_SaldoAnteriorEfectivo,
                        SaldoFinalEfectivo = cierre.CCS_SaldoFinalEfectivo.Value,
                        TotalEgresosEfectivo = cierre.CCS_TotalEgresosEfectivo,
                        TotalIngresosEfectivo = cierre.CCS_TotalIngresosEfectivo,
                        TotalIngresosOtrasFormas = cierre.CCS_TotalIngresosOtrasFormas,
                        TotalEgresosOtrasFormas = cierre.CCS_TotalEgresosOtrasFormas,
                        FechaCierre = cierre.CCS_FechaGrabacion,
                        UsuarioCierraPunto = cierre.CCS_CreadoPor,
                        IdCierreCentroServicio = cierre.CCS_IdCierreCentroServicios
                    };

                    return cierresPunto;
                }
                else
                {
                    CACierreCentroServicioDC cierresPunto = new CACierreCentroServicioDC();
                    return cierresPunto;
                }
            }
        }

        /// <summary>
        /// Obtiene los 10 ultimos cierres de un  punto.
        /// </summary>
        /// <param name="filtro">Es el filtro.</param>
        /// <param name="campoOrdenamiento">El campo de ordenamiento.</param>
        /// <param name="indicePagina">El indice pagina.</param>
        /// <param name="registrosPorPagina">Los registros por pagina.</param>
        /// <param name="ordenamientoAscendente">Sie s verdadero <c>true</c> es ascendente el ordenamiento.</param>
        /// <param name="totalRegistros">El total de Registros.</param>
        /// <param name="idPuntoCentroServicio">Es el Id del Punto de Servicio.</param>
        /// <returns>La lista de los 10 ultimos Cierres del punto</returns>
        public List<CACierreCentroServicioDC> ObtenerCierresPunto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idPuntoCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<CierreCentroServicios_CAJ>("CCS_IdCentroServicios", idPuntoCentroServicio.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarCierreCentroServicios_CAJ(filtro, where, "CCS_FechaGrabacion", out totalRegistros,
                                                      indicePagina, registrosPorPagina, ordenamientoAscendente)
                                                      .ToList()
                                                      .ConvertAll<CACierreCentroServicioDC>(s => new CACierreCentroServicioDC()
                                                      {
                                                          BaseInicial = s.CCS_BaseInicial,
                                                          IdCentroServicio = s.CCS_IdCentroServicios,
                                                          SaldoAnteriorEfectivo = s.CCS_SaldoAnteriorEfectivo,
                                                          SaldoFinalEfectivo = s.CCS_SaldoFinalEfectivo.Value,
                                                          TotalEgresosEfectivo = s.CCS_TotalEgresosEfectivo,
                                                          TotalIngresosEfectivo = s.CCS_TotalIngresosEfectivo,
                                                          TotalIngresosOtrasFormas = s.CCS_TotalIngresosOtrasFormas,
                                                          TotalEgresosOtrasFormas = s.CCS_TotalEgresosOtrasFormas,
                                                          FechaCierre = s.CCS_FechaGrabacion,
                                                          UsuarioCierraPunto = s.CCS_CreadoPor,
                                                          IdCierreCentroServicio = s.CCS_IdCierreCentroServicios
                                                      });
            }
        }

        /// <summary>
        /// Obtiene los cierres de las cajas de un Punto.
        /// </summary>
        /// <param name="idPuntoCentroServicio">The id punto centro servicio.</param>
        /// <returns>Lista de Cierres de cajas realizados en el punto</returns>
        public List<CACierreCentroServicioDC> ObtenerCierresPunto(long idPuntoCentroServicio)
        {
            List<CACierreCentroServicioDC> cierresPunto = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                List<CierreCentroServicios_CAJ> cierres = contexto.CierreCentroServicios_CAJ.Where(cierre => cierre.CCS_IdCentroServicios == idPuntoCentroServicio)
                                                      .ToList();

                if (cierres.Count > 0)
                {
                    cierresPunto = cierres
                      .ConvertAll<CACierreCentroServicioDC>(s => new CACierreCentroServicioDC()
                      {
                          BaseInicial = s.CCS_BaseInicial,
                          IdCentroServicio = s.CCS_IdCentroServicios,
                          SaldoAnteriorEfectivo = s.CCS_SaldoAnteriorEfectivo,
                          SaldoFinalEfectivo = s.CCS_SaldoFinalEfectivo.Value,
                          TotalEgresosEfectivo = s.CCS_TotalEgresosEfectivo,
                          TotalIngresosEfectivo = s.CCS_TotalIngresosEfectivo,
                          TotalIngresosOtrasFormas = s.CCS_TotalIngresosOtrasFormas,
                          TotalEgresosOtrasFormas = s.CCS_TotalEgresosOtrasFormas,
                          FechaCierre = s.CCS_FechaGrabacion,
                          UsuarioCierraPunto = s.CCS_CreadoPor,
                          IdCierreCentroServicio = s.CCS_IdCierreCentroServicios
                      });
                }
            }

            return cierresPunto;
        }

        /// <summary>
        /// Obtiene id Usuario Cierre
        /// </summary>
        /// <param name="idCierreCaja"></param>
        /// <returns></returns>
        public long ObtenerIdUsuarioCierre(long idCierreCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.AperturaCaja_CAJ
                  .FirstOrDefault(idCierre => idCierre.APC_IdAperturaCaja == idCierreCaja).APC_IdCodigoUsuario;
            }
        }

        /// <summary>
        /// Obtener las cajas para cerrar.
        /// </summary>
        /// <param name="idPuntoCentroServicio">Es el id punto centro servicio.</param>
        /// <param name="idFormaPago">Es la id forma pago.</param>
        /// <param name="operador">Es el operador logico.</param>
        /// <returns>Las cajas que se pueden Cerrar</returns>
        public List<CACierreCajaDC> ObtenerCajasParaCerrar(long idPuntoCentroServicio, short idFormaPago, string operador)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerResumenCajas_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@idPuntoAtencion", idPuntoCentroServicio);
                cmd.Parameters.AddWithValue("@idFormaPago", idFormaPago);
                cmd.Parameters.AddWithValue("@operador", operador);
                cmd.Parameters.AddWithValue("@idApertura", 0);
                //cmd.Parameters.AddWithValue("@esPam", esPam);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<CACierreCajaDC>(c =>
                            new CACierreCajaDC
                            {
                                IdCierreCaja = c.Field<long>("APC_IdAperturaCaja"),
                                IdCaja = c.Field<int>("APC_IdCaja"),
                                FechaApertura = c.Field<DateTime>("APC_FechaGrabacion"),
                                TotalIngresoEfectivo = c.Field<short>("RVF_IdFormaPago") == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<decimal>("Ingresos") : 0,
                                TotalEgresoEfectivo = c.Field<short>("RVF_IdFormaPago") == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<decimal>("Egresos") : 0,
                                TotalIngresoOtrasFormas = c.Field<short>("RVF_IdFormaPago") != TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<decimal>("Ingresos") : 0,
                                TotalEgresoOtrasFormas = c.Field<short>("RVF_IdFormaPago") != TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<decimal>("Egresos") : 0,
                                TotalReportar = c.Field<short>("RVF_IdFormaPago") == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? c.Field<decimal>("Ingresos") - c.Field<decimal>("Egresos") : 0,
                                CreadoPor = c.Field<string>("Creador"),
                                UsuarioCaja = c.Field<string>("APC_CreadoPor"),
                                IdPuntoAtencion = idPuntoCentroServicio,
                                FechaCierre = null,
                            }).ToList();
            }

        }

        /// <summary>
        /// Obtiene las Cajas Cerradas y Pendientes para Cerrar
        /// de un punto o centro de Servicio
        /// </summary>
        /// <param name="idCentroSrv"></param>
        /// <param name="idFormaPago"></param>
        /// <param name="operador"></param>
        /// <returns>lista de Cajas abiertas y cerradas</returns>
        public List<CACierreCajaDC> ObtenerCajasAReportarCajeroPrincipal(long idCentroSrv, short idFormaPago, string operador)
        {
            List<CACierreCajaDC> cajasAbiertas = ObtenerCajasParaCerrar(idCentroSrv, idFormaPago, operador);
            List<CACierreCajaDC> cajasCerradas = ObtenerCajasCerradasPorPuntoYFecha(idCentroSrv, DateTime.Now);

            cajasAbiertas.ForEach(Cajas =>
            {
                if (cajasCerradas == null)
                {
                    cajasCerradas = new List<CACierreCajaDC>();
                }
                cajasCerradas.Add(Cajas);
            });

            return cajasCerradas;
        }

        /// <summary>
        /// Obtiene las cajas cerradas por punto Y fecha.
        /// </summary>
        /// <param name="idCentroSvc">The id centro SVC.</param>
        /// <param name="fechaCierre">The fecha cierre.</param>
        /// <returns>Lista de Cajas Cerradas</returns>
        public List<CACierreCajaDC> ObtenerCajasCerradasPorPuntoYFecha(long idCentroSvc, DateTime fechaCierre)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                DateTime fechaInicial, fechaFinal;                
                fechaInicial = fechaCierre.Date;
                fechaFinal = fechaCierre.Date.Add(new TimeSpan(0, 23, 59, 59, 999));

                SqlCommand cmd = new SqlCommand("paObtenerCajasCerradasXCSFecha_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCS", idCentroSvc);
                cmd.Parameters.AddWithValue("@FechaDesde", fechaInicial);
                cmd.Parameters.AddWithValue("@FechaHasta", fechaFinal);
                //cmd.Parameters.AddWithValue("@esPam", esPam);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<CACierreCajaDC>(c =>
                {
                    return new CACierreCajaDC
                    {
                        IdCierreCaja = c.Field<long>("CIC_IdCierreCaja"),
                        IdCaja = c.Field<int>("APC_IdCaja"),
                        TotalIngresoEfectivo = c.Field<decimal>("CIC_TotalIngresosEfectivo"),
                        TotalEgresoEfectivo = c.Field<decimal>("CIC_TotalEgresosEfectivo"),
                        TotalIngresoOtrasFormas = c.Field<decimal>("CIC_TotalIngresosOtrasFormas"),
                        TotalEgresoOtrasFormas = c.Field<decimal>("CIC_TotalEgresosOtrasFormas"),
                        TotalReportar = c.Field<decimal>("CIC_TotalIngresosEfectivo") - c.Field<decimal>("CIC_TotalEgresosEfectivo"),
                        UsuarioCaja = c.Field<string>("Usuario"),
                        IdPuntoAtencion = c.Field<long>("ACS_IdCentroServicios"),
                        FechaCierre = c.Field<DateTime>("CIC_FechaGrabacion"),
                        FechaApertura = c.Field<DateTime>("APC_FechaGrabacion"),
                        CreadoPor = c.Field<string>("Usuario")
                    };
                });
            }

        }

        /// <summary>
        /// Obtiene el Valor a Enviar a la Empresa
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        public decimal ObtenerValorEmpresa(long idCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CACierreCentroServicioDC valorEmpresa = ObtenerUltimoCierrePunto(idCentroServicio);

                RecoleccionDineroPuntos_CAJ idCierreReporteDinero = contexto.RecoleccionDineroPuntos_CAJ
                  .FirstOrDefault(id => id.RDP_IdCierreCentroServicios == valorEmpresa.IdCierreCentroServicio);

                decimal ValorFinalEmpresa = 0;

                // Si existe un cierre reportado no permite realizar el envio de dinero
                if (idCierreReporteDinero == null)
                {
                    if (valorEmpresa != null)
                    {
                        var enviadoNoDescargado = (from Recoleccion in contexto.RecoleccionDineroPuntos_CAJ
                                                   join Reporte in contexto.ReporteDineroPuntoAgencia_CAJ
                                                   on Recoleccion.RDP_IdRecoleccion equals Reporte.RDP_IdRecoleccion into Enviados
                                                   from env in Enviados.DefaultIfEmpty()
                                                   where Recoleccion.RDP_IdPuntoServicio == idCentroServicio && env.RDP_IdRecoleccion == null
                                                   select new { Recoleccion.RDP_ValorTotalEnviado })
                                                   .ToList();

                        if (enviadoNoDescargado.Count > 0)
                        {
                            decimal Enviado = enviadoNoDescargado.Sum(sum => sum.RDP_ValorTotalEnviado);
                            ValorFinalEmpresa = valorEmpresa.SaldoFinalEfectivo - Enviado;
                        }
                        else
                        {
                            ValorFinalEmpresa = valorEmpresa.SaldoFinalEfectivo;
                        }
                        return ValorFinalEmpresa;
                    }

                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                                .ERROR_NO_EXISTE_CIERRE_DEL_PUNTO.ToString(), CACajaServerMensajes
                                                                                .CargarMensaje(CAEnumTipoErrorCaja.ERROR_NO_EXISTE_CIERRE_DEL_PUNTO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }

                //Se lanza el mensaje de Dinero Ya Reportado
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_DINERO_YA_REPORTADO_AGENCIA.ToString(),
                                                    string.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_DINERO_YA_REPORTADO_AGENCIA),
                                                    valorEmpresa.IdCierreCentroServicio, valorEmpresa.FechaCierre));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Valida si un punto tiene pendiente algun descargue de dinero reportado
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public bool ValidarReporteDineroPuntoNoDescargado(long idCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var enviadoNoDescargado = (from Recoleccion in contexto.RecoleccionDineroPuntos_CAJ
                                           join Reporte in contexto.ReporteDineroPuntoAgencia_CAJ
                                           on Recoleccion.RDP_IdRecoleccion equals Reporte.RDP_IdRecoleccion into Enviados
                                           from env in Enviados.DefaultIfEmpty()
                                           where Recoleccion.RDP_IdPuntoServicio == idCentroServicio && env.RDP_IdRecoleccion == null
                                           select new { Recoleccion.RDP_ValorTotalEnviado })
                                      .ToList();

                if (enviadoNoDescargado.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Obtiene el dinero puntos reportados.
        /// </summary>
        /// <param name="idCentroServicio">es el id centro servicio.</param>
        /// <returns>la lista de los puntos con el dinero reportado</returns>
        public List<CARecoleccionDineroPuntoDC> ObtenerDineroReportadoPuntos(IDictionary<string, string> filtro, long idCentroServicio)
        {
            List<CARecoleccionDineroPuntoDC> reporteDineros = new List<CARecoleccionDineroPuntoDC>();

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                string idPuntoServicioFiltro;
                string idMensajeroFiltro;

                filtro.TryGetValue("RDP_IdPuntoServicio", out idPuntoServicioFiltro);
                filtro.TryGetValue("RDP_IdMensajero", out idMensajeroFiltro);

                List<DineroReportPunto_CAJ> dineroPuntos = contexto.paObtenerDineroReportPunto_CAJ(idCentroServicio, idPuntoServicioFiltro,
                                                                                                    idMensajeroFiltro)
                                                                                                    .ToList();

                if (dineroPuntos.Count > 0)
                {
                    dineroPuntos.ForEach(pto =>
                    {
                        if (pto.RDP_ValorTotalEnviado > 0)
                        {
                            CARecoleccionDineroPuntoDC punto = new CARecoleccionDineroPuntoDC()
                            {
                                BolsaSeguridad = pto.RDP_NoBolsaSeguridad == null ? string.Empty : pto.RDP_NoBolsaSeguridad,
                                IdPuntoServicio = pto.RDP_IdPuntoServicio,
                                MensajeroPunto = new OUNombresMensajeroDC()
                                {
                                    NombreApellido = pto.RDP_NombreMensajero == null ? string.Empty : pto.RDP_NombreMensajero,
                                    IdPersonaInterna = pto.RDP_IdMensajero,
                                },
                                NombrePunto = pto.RDP_NombrePuntoServicio,
                                UsuarioCierre = pto.RDP_CreadoPor,
                                ValorTotalEnviado = pto.RDP_ValorTotalEnviado.Value,
                                FechaRecoleccion = pto.RDP_FechaGrabacion,
                                IdRecoleccion = pto.RDP_IdRecoleccion,
                                ValorDiferencia = pto.RDP_ValorTotalEnviado.Value
                            };

                            reporteDineros.Add(punto);
                        }
                    });
                }
            }

            return reporteDineros;
        }

        /// <summary>
        /// Obtiene el ultimo cierre no reportado
        /// </summary>
        /// <param name="idAgencia"></param>
        /// <param name="idPunto"></param>
        /// <returns>info del cierre no reportado </returns>
        public CACierreCentroServicioDC ObtenerUltimoCierreNoReportadoAgencia(long idAgencia, long idPunto)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CACierreCentroServicioDC infoCierreNoReportado = null;

                var CierreNoReportado = (from CierreCtrSvc in contexto.CierreCentroServicios_CAJ
                                         join RecoleccDinero in contexto.RecoleccionDineroPuntos_CAJ
                                         on CierreCtrSvc.CCS_IdCierreCentroServicios equals RecoleccDinero.RDP_IdCierreCentroServicios
                                         into NoReport
                                         from noRep in NoReport.DefaultIfEmpty()
                                         where CierreCtrSvc.CCS_IdCentroServicios == idPunto &&
                                         noRep.RDP_IdRecoleccion == null
                                         select new
                                         {
                                             CierreCtrSvc.CCS_FechaGrabacion,
                                             CierreCtrSvc.CCS_IdCierreCentroServicios,
                                             CierreCtrSvc.CCS_SaldoFinalEfectivo
                                         })
                                          .OrderByDescending(order => order.CCS_FechaGrabacion).FirstOrDefault();

                if (CierreNoReportado != null)
                {
                    infoCierreNoReportado = new CACierreCentroServicioDC()
                    {
                        IdCierreCentroServicio = CierreNoReportado.CCS_IdCierreCentroServicios,
                        SaldoFinalEfectivo = CierreNoReportado.CCS_SaldoFinalEfectivo.Value
                    };
                }
                return infoCierreNoReportado;
            }
        }

        /// <summary>
        /// Obtener los Tipos Observ punto.
        /// </summary>
        /// <returns></returns>
        public List<CATipoObsPuntoAgenciaDC> ObtenerTiposObservPunto()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoObsPuntoAgencia_CAJ
                  .ToList()
                  .ConvertAll<CATipoObsPuntoAgenciaDC>(r => new CATipoObsPuntoAgenciaDC()
                  {
                      idTipoObservacion = r.TOP_IdTipoObservacion,
                      Descripcion = r.TOP_Descripcion
                  });
            }
        }

        /// <summary>
        /// Obtiene la caja por apertura.
        /// </summary>
        /// <param name="idApertura">Identificación de la apertura.</param>
        /// <returns>Objeto con la información de la apertura y la caja</returns>
        public CAAperturaCajaDC ObtenerCajaPorApertura(long idApertura)
        {
            CAAperturaCajaDC caja = null;
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                AperturaCaja_CAJ aperCaja = contexto.AperturaCaja_CAJ
                  .FirstOrDefault(id => id.APC_IdAperturaCaja == idApertura);

                if (aperCaja != null)
                {
                    caja = new CAAperturaCajaDC()
                    {
                        IdAperturaCaja = aperCaja.APC_IdAperturaCaja,
                        IdCaja = aperCaja.APC_IdCaja,
                        IdCodigoUsuario = aperCaja.APC_IdCodigoUsuario,
                    };
                }
            }

            return caja;
        }

        /// <summary>
        ///Valida que ya exista un cierre automatico de sistema antes de permitir al cajero hacer un cierre manual
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public bool ValidarCierreSistema(long idCentroServicios)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaHoy = DateTime.Now.Date;

                CierreCentroServicios_CAJ cierreSistema = contexto.CierreCentroServicios_CAJ.Where(c => c.CCS_IdCentroServicios == idCentroServicios && c.CCS_FechaGrabacion >= fechaHoy && c.CCS_EsCierreSistema == true).FirstOrDefault();

                if (cierreSistema != null)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// obtiene la apertura de caja por
        /// los datos del usuario
        /// </summary>
        /// <param name="idUsuario">id d usuario</param>
        /// <param name="idCentroServicio">id d centro servicio</param>
        /// <param name="idCaja">id caja del usuario</param>
        /// <returns>long numero apertura</returns>
        public long ObtenerAperturaCajaPorUsuario(long idUsuario, long idCentroServicio, int idCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                AperturaCentroServicos_VCAJ idApertura = contexto.AperturaCentroServicos_VCAJ.FirstOrDefault(ver => ver.APC_IdCodigoUsuario == idUsuario
                                                                                && ver.ACS_IdCentroServicios == idCentroServicio
                                                                                && ver.APC_IdCaja == idCaja
                                                                                && ver.APC_EstaAbierta == true);

                if (idApertura == null)
                    return 0;
                else
                    return idApertura.APC_IdAperturaCaja;
            }
        }

        /// <summary>
        /// Obtiene la informacion de los alcobros sin cancelar y de los
        /// que estan en transito de un Centro de Servicio
        /// </summary>
        /// <param name="idCentroSrv">es el Id del Centro de Servicio</param>
        /// <returns>info de Totales</returns>
        public ADAlCobrosSinCancelarDC ObtenerTotalesAlCobrosSinPagar(long idCentroSrv)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                paObtenerAlCobrosSinCancelar_MEN dataAlCobros = contexto.paObtenerAlCobrosSinCancelar_MEN(idCentroSrv).FirstOrDefault();

                if (dataAlCobros != null)
                {
                    ADAlCobrosSinCancelarDC infoAlCobros = new ADAlCobrosSinCancelarDC()
                    {
                        CantAlCobrosEnTransito = dataAlCobros.TotalAlCobroEnTransito.HasValue ? dataAlCobros.TotalAlCobroEnTransito.Value : 0,
                        ValorAlCobrosEnTransito = dataAlCobros.VRAlCobroEnTransito.HasValue ? dataAlCobros.VRAlCobroEnTransito.Value : 0,
                        CantAlCobrosSinCancelar = dataAlCobros.TotalAlCobroSinCancelar.HasValue ? dataAlCobros.TotalAlCobroSinCancelar.Value : 0,
                        ValorAlCobrosSinCancelar = dataAlCobros.TotalAlCobroSinCancelar.HasValue ? dataAlCobros.VRAlCobroSinCancelar.Value : 0
                    };

                    return infoAlCobros;
                }
                else
                {
                    ADAlCobrosSinCancelarDC infoAlCobros = new ADAlCobrosSinCancelarDC()
                    {
                        CantAlCobrosEnTransito = 0,
                        ValorAlCobrosEnTransito = 0,
                        CantAlCobrosSinCancelar = 0,
                        ValorAlCobrosSinCancelar = 0
                    };
                    return infoAlCobros;
                }
            }
        }

        #endregion Consulta Cierre Puntos

        #region Consulta Mensajero

        /// <summary>
        /// Estados de la Cta mensajero.
        /// </summary>
        /// <param name="idMensajero">The id mensajero.</param>
        /// <param name="fechaConsulta">The fecha consulta.</param>
        /// <returns></returns>
        public List<CACuentaMensajeroDC> ObtenerEstadoCtaMensajero(long idMensajero, DateTime fechaConsulta)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial, fechaFinal;

                fechaInicial = fechaConsulta.Date;
                fechaFinal = fechaConsulta.Date.Add(new TimeSpan(0, 23, 59, 59, 999));

                List<paObtenerResumeCtaMensajer_CAJ_Result> obtenCtaMensajero = contexto.paObtenerResumeCtaMensajer_CAJ(idMensajero, fechaInicial, fechaFinal)
                  .ToList();

                List<CACuentaMensajeroDC> ctaMensajero = obtenCtaMensajero
                    .ConvertAll<CACuentaMensajeroDC>(cuenta => new CACuentaMensajeroDC()
                    {
                        IdTransaccion = cuenta.CUM_IdTransaccion,
                        ConceptoCajaMensajero = new CAConceptoCajaDC()
                        {
                            Nombre = cuenta.COC_Nombre
                        },
                        NumeroDocumento = cuenta.CUM_NumeroDocumento.Value,
                        ValorIngreso = cuenta.Ingresos,
                        ValorEgreso = cuenta.Egresos,
                        SaldoAcumulado = cuenta.CUM_SaldoAcumulado,
                        FechaGrabacion = cuenta.CUM_FechaGrabacion,
                        Observaciones = cuenta.CUM_Observaciones
                    });

                return ctaMensajero;
            }
        }

        /// <summary>
        /// Obtener los conceptos de Caja de Gestión de un RACOL
        /// </summary>
        /// <returns>Arreglo con los conceptos de caja de una Caja de Gestión de RACOL</returns>
        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionRacol()
        {
            List<CAConceptoCajaDC> conceptosRacol = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var conceptoCategoria = contexto.ConceptoCaja_CAJ
                  .Join(contexto.ConceptoCajaCategoria_CAJ,
                  k => k.COC_IdConceptoCaja,
                  f => f.CCA_IdConceptoCaja,
                  (a, b) => new { Con = a, CCat = b })
                  .Join(contexto.CategoriaConceptoCaja_CAJ,
                  k2 => k2.CCat.CCA_IdCategoria,
                  f2 => f2.CCC_IdCategoria,
                  (c, d) => new { Cat = c, ConPorCat = d })
                  .Where(w => w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_BANCOS ||
                    w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_RACOL ||
                    w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_RACOL_OPN)
                  .Select(s => new
                  {
                      Concepto = s.Cat.Con,
                      Catetoria = s.ConPorCat
                  })
                  .ToList();

                conceptosRacol = new List<CAConceptoCajaDC>(conceptoCategoria.Count);

                foreach (var item in conceptoCategoria)
                {
                    conceptosRacol.Add(new CAConceptoCajaDC
                    {
                        Descripcion = item.Concepto.COC_Nombre,
                        IdCuentaExterna = item.Concepto.COC_IdCuentaExterna,
                        EsEgreso = item.Concepto.COC_EsIngreso,
                        EsIngreso = !item.Concepto.COC_EsIngreso,
                        IdConceptoCaja = item.Concepto.COC_IdConceptoCaja,
                        Nombre = item.Concepto.COC_Nombre
                    });
                }
            }

            return conceptosRacol
              .OrderBy(o => o.Nombre)
              .ToList();
        }

        /// <summary>
        /// Obtener los conceptos de Caja de Gestión de Operación Nacional
        /// </summary>
        /// <returns>Arreglo con los conceptos de caja de una Caja de Gestión de Operación Nacional</returns>
        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionOpn()
        {
            List<CAConceptoCajaDC> conceptosRacol = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var conceptoCategoria = contexto.ConceptoCaja_CAJ
                  .Join(contexto.ConceptoCajaCategoria_CAJ,
                  k => k.COC_IdConceptoCaja,
                  f => f.CCA_IdConceptoCaja,
                  (a, b) => new { Con = a, CCat = b })
                  .Join(contexto.CategoriaConceptoCaja_CAJ,
                  k2 => k2.CCat.CCA_IdCategoria,
                  f2 => f2.CCC_IdCategoria,
                  (c, d) => new { Cat = c, ConPorCat = d })
                  .Where(w => w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_BANCOS ||
                    w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_RACOL_OPN ||
                     w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_DESDE_CAJA_OPN)
                  .Select(s => new
                  {
                      Concepto = s.Cat.Con,
                      Catetoria = s.ConPorCat
                  })
                  .ToList();

                conceptosRacol = new List<CAConceptoCajaDC>(conceptoCategoria.Count);

                foreach (var item in conceptoCategoria)
                {
                    conceptosRacol.Add(new CAConceptoCajaDC
                    {
                        Descripcion = item.Concepto.COC_Nombre,
                        IdCuentaExterna = item.Concepto.COC_IdCuentaExterna,
                        EsEgreso = item.Concepto.COC_EsIngreso,
                        EsIngreso = !item.Concepto.COC_EsIngreso,
                        IdConceptoCaja = item.Concepto.COC_IdConceptoCaja,
                        Nombre = item.Concepto.COC_Nombre
                    });
                }
            }

            return conceptosRacol
              .OrderBy(o => o.Nombre)
              .ToList();
        }

        /// <summary>
        /// Obtener los conceptos de Caja de Gestión de Casa Matriz
        /// </summary>
        /// <returns>Arreglo con los conceptos de caja de una Caja de Gestión de Casa Matriz</returns>
        public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionCasaMatriz()
        {
            List<CAConceptoCajaDC> conceptosRacol = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var conceptoCategoria = contexto.ConceptoCaja_CAJ
                  .Join(contexto.ConceptoCajaCategoria_CAJ,
                  k => k.COC_IdConceptoCaja,
                  f => f.CCA_IdConceptoCaja,
                  (a, b) => new { Con = a, CCat = b })
                  .Join(contexto.CategoriaConceptoCaja_CAJ,
                  k2 => k2.CCat.CCA_IdCategoria,
                  f2 => f2.CCC_IdCategoria,
                  (c, d) => new { Cat = c, ConPorCat = d })
                  .Where(w => w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_BANCOS ||
                    w.ConPorCat.CCC_IdCategoria == (int)CAEnumCategoriasConceptoCaja.MOVIMIENTO_CASA_MATRIZ)
                  .Select(s => new
                  {
                      Concepto = s.Cat.Con,
                      Catetoria = s.ConPorCat
                  })
                  .ToList();

                conceptosRacol = new List<CAConceptoCajaDC>(conceptoCategoria.Count);

                foreach (var item in conceptoCategoria)
                {
                    conceptosRacol.Add(new CAConceptoCajaDC
                    {
                        Descripcion = item.Concepto.COC_Nombre,
                        IdCuentaExterna = item.Concepto.COC_IdCuentaExterna,
                        EsEgreso = item.Concepto.COC_EsIngreso,
                        EsIngreso = !item.Concepto.COC_EsIngreso,
                        IdConceptoCaja = item.Concepto.COC_IdConceptoCaja,
                        Nombre = item.Concepto.COC_Nombre
                    });
                }
            }

            return conceptosRacol
              .OrderBy(o => o.Nombre)
              .ToList();
        }

        /// <summary>
        /// Obtiene los conceptos de Caja por especificacion de
        /// visibilidad para mensajero - punto/Agencia - Racol.
        /// </summary>
        /// <param name="filtroCampoVisible">The filtro campo visible.</param>
        /// <returns>Lista de Conceptos de Caja por el filtro de Columna</returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCajaPorCategoria(int idCategoria)
        {
            List<CAConceptoCajaDC> conceptoCaja = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var conceptosCajaPorCategoria = (from CategConcepCaja in contexto.CategoriaConceptoCaja_CAJ
                                                 join ConcepCajaCateg in contexto.ConceptoCajaCategoria_CAJ
                                                 on CategConcepCaja.CCC_IdCategoria equals ConcepCajaCateg.CCA_IdCategoria
                                                 join ConcepCaja in contexto.ConceptoCaja_CAJ
                                                 on ConcepCajaCateg.CCA_IdConceptoCaja equals ConcepCaja.COC_IdConceptoCaja
                                                 where CategConcepCaja.CCC_IdCategoria == idCategoria
                                                 select new
                                                 {
                                                     ConcepCaja.COC_IdConceptoCaja,
                                                     ConcepCaja.COC_Nombre,
                                                     ConcepCaja.COC_EsIngreso,
                                                     CategConcepCaja.CCC_IdCategoria,
                                                     CategConcepCaja.CCC_Descripcion,
                                                     ConcepCaja.COC_ContrapartidaCasaMatriz,
                                                     ConcepCaja.COC_ContrapartidaCS
                                                 })
                                                 .ToList();

                if (conceptosCajaPorCategoria.Count > 0)
                {
                    conceptoCaja = conceptosCajaPorCategoria
                      .ConvertAll<CAConceptoCajaDC>(concep => new CAConceptoCajaDC()
                      {
                          IdConceptoCaja = concep.COC_IdConceptoCaja,
                          Nombre = concep.COC_Nombre,
                          EsIngreso = concep.COC_EsIngreso,
                          GruposCategorias = new ObservableCollection<CAConceptoCajaCategoriaDC>(
                              new List<CAConceptoCajaCategoriaDC>(){new CAConceptoCajaCategoriaDC()
                          {
                              IdCategoria = concep.CCC_IdCategoria,
                              Descripcion = concep.CCC_Descripcion
                          }}),
                          ContraPartidaCasaMatriz = concep.COC_ContrapartidaCasaMatriz,
                          ContraPartidaCS = concep.COC_ContrapartidaCS
                      });
                }
                else
                {
                    conceptoCaja = new List<CAConceptoCajaDC>();
                }

                return conceptoCaja;
            }
        }

        /// <summary>
        /// Obtiene el concepto de Caja por id.
        /// </summary>
        /// <param name="idConcepto">Identifcador del concepto de caja.</param>
        /// <returns>Objeto con la información del concepto de caja</returns>
        public CAConceptoCajaDC ObtenerConceptoPorId(int idConcepto)
        {
            CAConceptoCajaDC conceptoCaja = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ConceptoCaja_CAJ concepto = contexto.ConceptoCaja_CAJ
                  .FirstOrDefault(concep => concep.COC_IdConceptoCaja == idConcepto);

                if (concepto == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA,
                      CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA.ToString(),
                      String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA), idConcepto));

                    throw new FaultException<ControllerException>(excepcion);
                }

                if (concepto != null)
                {
                    conceptoCaja = new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = concepto.COC_IdConceptoCaja,
                        Nombre = concepto.COC_Nombre,
                        EsIngreso = concepto.COC_EsIngreso,
                        IdCuentaExterna = concepto.COC_IdCuentaExterna
                    };
                }
            }

            return conceptoCaja;
        }

        /// <summary>
        /// Obtiene el concepto de Caja por id.
        /// </summary>
        /// <param name="idConcepto">Identifcador del concepto de caja.</param>
        /// <returns>Objeto con la información del concepto de caja</returns>
        public CAConceptoCajaDC ObtenerConceptoPorId(int idConcepto, SqlConnection conexion, SqlTransaction transaccion)
        {
            CAConceptoCajaDC conceptoCaja = null;

            string cmdText = @"SELECT * 
                               FROM ConceptoCaja_CAJ
                               WHERE COC_IdConceptoCaja = @idConcepto";
            SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@idConcepto", idConcepto);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var concepto = dt.AsEnumerable().FirstOrDefault();
            if (concepto != null)
            {
                conceptoCaja = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = concepto.Field<int>("COC_IdConceptoCaja"),
                    Nombre = concepto.Field<string>("COC_Nombre"),
                    EsIngreso = concepto.Field<bool>("COC_EsIngreso"),
                    IdCuentaExterna = concepto.Field<short?>("COC_IdCuentaExterna")
                };
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA,
                     CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA.ToString(),
                     String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_NO_CONCEPTO_CAJA), idConcepto));

                throw new FaultException<ControllerException>(excepcion);
            }

            return conceptoCaja;
        }

        /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <returns></returns>
        public List<CAReporteMensajeroCajaDC> ObtenerReportesMensajeros(long idMensajero)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var repor = contexto.paObtenerReportesMensajero_CAJ(idMensajero).ToList().ConvertAll<CAReporteMensajeroCajaDC>(r =>
                     new CAReporteMensajeroCajaDC()
                     {
                         Mensajero = new OUNombresMensajeroDC()
                         {
                             IdMensajero = r.RMC_IdMensajero,
                             NombreApellido = r.RMC_NombreMensajero
                         },
                         UsuarioRegistro = r.RMC_CreadoPor,
                         NumeroComprobanteTransDetCaja = r.RTD_NumeroComprobante
                     });

                return repor;
            }
        }

        /// <summary>
        /// Obtiene todos los reportes de caja de un mensajero por comprobante para imprimir
        /// </summary>
        /// <param name="idMensajero"></param>
        /// <param name="numComprobante"></param>
        /// <returns></returns>
        public CADatosImpCompMensajeroDC ObtenerReportesMensajerosImprimir(long idMensajero, string numComprobante)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CADatosImpCompMensajeroDC imp = new CADatosImpCompMensajeroDC()
                {
                    MovimientosAgencia = new List<CADatosMovimientoDC>(),
                    MovmientosMensajero = new List<CADatosMovimientoDC>(),
                    ListEntregasAlCobroMensajero = new List<OUEnviosPendMensajerosDC>()
                };

                contexto.paObtenerEnviMenjAlCobro_CAJ(idMensajero, long.Parse(numComprobante)).ToList().ForEach(o =>
                {
                    imp.ListEntregasAlCobroMensajero.Add(new OUEnviosPendMensajerosDC()
                    {
                        NumeroGuia = o.ADM_NumeroGuia,
                        ValorTotalGuia = (decimal)o.PAG_ValorTotalGuia,
                        Descargado = true
                    });
                });

                contexto.paObtenerReporteMensajeroImprimir_CAJ(idMensajero, numComprobante).ToList()
                    .ForEach(o =>
                    {
                        if (o.COC_IdConceptoCaja > 0)
                            imp.MovimientosAgencia.Add(new CADatosMovimientoDC()
                            {
                                IdConceptoCaja = o.COC_IdConceptoCaja,
                                NombreConceptoCaja = o.COC_Nombre.ToUpper(),
                                NumeroOperacion = o.RTD_Numero == 0 ? "" : "Aut. Des.:" + o.RTD_Numero.ToString(),
                                ValorOperacion = o.RTD_ValorServicio
                            });
                        else
                            imp.MovimientosAgencia.Add(new CADatosMovimientoDC()
                            {
                                IdConceptoCaja = o.IdConceptoCajaMensajero,
                                NombreConceptoCaja = o.NombConceptoCajaMensajero.ToUpper(),
                                NumeroOperacion = o.RTD_Numero == 0 ? "" : "Aut. Des.:" + o.RTD_Numero.ToString(),
                                ValorOperacion = o.RTD_ValorServicio
                            });

                        imp.MovmientosMensajero.Add(new CADatosMovimientoDC()
                        {
                            IdConceptoCaja = o.IdConceptoCajaMensajero,
                            NombreConceptoCaja = o.NombConceptoCajaMensajero.ToUpper(),
                            NumeroOperacion = o.RTD_Numero == 0 ? "" : "Aut. Des.:" + o.RTD_Numero.ToString(),
                            ValorOperacion = o.RTD_ValorServicio
                        });
                        imp.CedulaMensajero = o.PEI_Identificacion;
                        imp.NombrMensajero = o.CUM_NombreMensajero;
                        imp.FechaActual = o.RTD_FechaGrabacion;
                    });



                return imp;
            }
        }

        #endregion Consulta Mensajero

        #region Consultas Prepago

        /// <summary>
        /// Validar el saldo del Prepago para descontar del
        /// saldo.
        /// </summary>
        /// <param name="idPinPrepago">el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">es el valor de la compra con el pin prepago.</param>
        /// <returns></returns>
        public void ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                PinPrepago_CAJ pinPrepagoResult = contexto.PinPrepago_CAJ.Where(r => r.PIP_Pin == pinPrepago)
                  .FirstOrDefault();

                if (pinPrepagoResult != null)
                {
                    if ((pinPrepagoResult.PIP_Saldo - valorCompraPinPrepago) < 0)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                              .ERROR_VALOR_PINPREPAGO_SUPERA_SALDO.ToString(), CACajaServerMensajes
                                                                              .CargarMensaje(CAEnumTipoErrorCaja.ERROR_VALOR_PINPREPAGO_SUPERA_SALDO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                            .ERROR_PINPREPAGO_NO_ENCONTRADO.ToString(), CACajaServerMensajes
                                                                            .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_NO_ENCONTRADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Validar el saldo del Prepago para descontar del
        /// saldo.
        /// </summary>
        /// <param name="idPinPrepago">el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">es el valor de la compra con el pin prepago.</param>
        /// <returns></returns>
        public void ValidarSaldoPrepago(long pinPrepago, decimal valorCompraPinPrepago, SqlConnection conexion, SqlTransaction transaccion)
        {

            string cmdText = @" SELECT * 
                                FROM PinPrepago_CAJ
                                WHERE PIP_Pin = @pinPrepago";
            SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddWithValue("@pinPrepago", pinPrepago);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var pinPrepagoRes = dt.AsEnumerable().FirstOrDefault();
            if (pinPrepagoRes != null)
            {
                if ((pinPrepagoRes.Field<decimal>("PIP_Saldo") - valorCompraPinPrepago) < 0)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                          .ERROR_VALOR_PINPREPAGO_SUPERA_SALDO.ToString(), CACajaServerMensajes
                                                                          .CargarMensaje(CAEnumTipoErrorCaja.ERROR_VALOR_PINPREPAGO_SUPERA_SALDO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
            else
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                        .ERROR_PINPREPAGO_NO_ENCONTRADO.ToString(), CACajaServerMensajes
                                                                        .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_NO_ENCONTRADO));
                throw new FaultException<ControllerException>(excepcion);
            }

        }

        /// <summary>
        /// Obtiene los Prepagos vendidos por un Centro de Servicio.
        /// </summary>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns>Lista de Prepagos vendidos</returns>
        public List<CAPinPrepagoDC> ObtenerPrepagosCentroServicio(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina,
                                                                  int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros,
                                                                  long idCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                totalRegistros = 0;
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lambda = contexto.CrearExpresionLambda<PinPrepago_CAJ>("PIP_IdCentroServiciosVende", idCentroServicio.ToString(), OperadorComparacion.Equal);
                where.Add(lambda, OperadorLogico.And);

                return contexto.ConsultarPinPrepago_CAJ(filtro, where, "PIP_Pin", out totalRegistros,
                                                      indicePagina, registrosPorPagina, ordenamientoAscendente)
                                                      .ToList()
                                                      .ConvertAll<CAPinPrepagoDC>(venta => new CAPinPrepagoDC()
                                                      {
                                                          Pin = venta.PIP_Pin,
                                                          ValorPin = venta.PIP_Valor,
                                                          SaldoPinPrepago = venta.PIP_Saldo,
                                                          FechaActualizacion = venta.PIP_FechaActualizacion,
                                                          NombreComprador = venta.PIP_NombreComprador,
                                                          TipoId = new PATipoIdentificacion()
                                                          {
                                                              DescripcionIdentificacion = venta.PIP_IdTipoIdentificacion
                                                          },
                                                          Identificacion = venta.PIP_Identificacion,
                                                          Direccion = venta.PIP_Direccion,
                                                          IdLocalidad = venta.PIP_IdLocalidad,
                                                          IdCentroServicioVende = venta.PIP_IdCentroServiciosVende,
                                                          FechaGrabacion = venta.PIP_FechaGrabacion
                                                      });
            }
        }

        /// <summary>
        /// Obtiene el detalle del pin prepago.
        /// </summary>
        /// <param name="pinPrepago">The pin prepago.</param>
        /// <returns>Lista de las compras realizadas con un pin prepago</returns>
        public List<CAPinPrepagoDtllCompraDC> ObtenerDtllCompraPinPrepago(long pinPrepago)
        {
            List<CAPinPrepagoDtllCompraDC> comprasPrepago;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                List<RegVentaPinPrepag_CAJ> dtlleCompra = contexto.paObtenerRegVentaPinPrepag_CAJ(pinPrepago)
                  .ToList();

                if (dtlleCompra.Count > 0)
                {
                    comprasPrepago = dtlleCompra
                      .ConvertAll<CAPinPrepagoDtllCompraDC>(dtll => new CAPinPrepagoDtllCompraDC()
                      {
                          PinPrepago = dtll.PIP_Pin,
                          DescripcionConceptoCaja = dtll.RTD_NombreConcepto,
                          Cantidad = dtll.RTD_Cantidad,
                          ValorServicio = dtll.RVF_Valor,
                          FechaCompra = dtll.RTD_FechaGrabacion,
                          NumeroDocumento = dtll.RTD_Numero
                      });
                }
                else
                {
                    comprasPrepago = new List<CAPinPrepagoDtllCompraDC>();
                }
            }

            return comprasPrepago;
        }

        /// <summary>
        /// Obtiene el valor del parametro configurado en la tabla de parametros caja.
        /// </summary>
        /// <param name="idParametro">The id parametro.</param>
        /// <returns></returns>
        public string ObtenerParametroCajas(string idParametro)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ParametrosCajas_CAJ parametroCaj = contexto.ParametrosCajas_CAJ
                  .FirstOrDefault(idPara => idPara.PAC_IdParametro == idParametro);

                if (parametroCaj == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA,
                      CAEnumTipoErrorCaja.ERROR_PARAMETRO_CAJA_NO_ENCONTRADO.ToString(),
                      String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_PARAMETRO_CAJA_NO_ENCONTRADO), idParametro));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return parametroCaj.PAC_ValorParametro;
            }
        }

        /// <summary>
        /// Obtiene todos los valores de los parametros configurado en la tabla de parametros caja.
        /// </summary>        
        /// <returns></returns>
        public IEnumerable<CAParametroCaja> ObtenerParametrosCajas()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ParametrosCajas_CAJ.ToList().ConvertAll<CAParametroCaja>(p =>
                {
                    return new CAParametroCaja()
                    {
                        ValorParametro = p.PAC_ValorParametro,
                        IdParametro = p.PAC_IdParametro,
                        DescripcionParametro = p.PAC_Descripcion
                    };
                });

            }
        }

        /// <summary>
        /// Actualiza el valor de un parametro de cajas
        /// </summary>
        /// <param name="idParametro"></param>
        /// <param name="valor"></param>
        public void ActualizarParametroCaja(string idParametro, string valor)
        {

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ParametrosCajas_CAJ paramCaja = contexto.ParametrosCajas_CAJ.Where(p => p.PAC_IdParametro == idParametro).FirstOrDefault();
                if (paramCaja != null)
                {
                    paramCaja.PAC_ValorParametro = valor;

                    contexto.SaveChanges();
                }
            }
        }

        #endregion Consultas Prepago

        #region Racol-Casa Matriz

        /// <summary>
        /// Metodo para Obtener las Transacciones realizadas por la Empresa.
        /// </summary>
        /// <param name="fechaTransaccion">The fecha transaccion.</param>
        /// <returns></returns>
        public IList<CACajaCasaMatrizDC> ObtenerTransaccionesCasaMatriz(DateTime fechaTransaccion, short idCasaMatriz)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaInicial, fechaFinal;
                fechaInicial = fechaTransaccion.Date;
                fechaFinal = fechaTransaccion.Date.Add(new TimeSpan(0, 23, 59, 59, 999));

                var operaconesCasaMatriz = contexto.OperacionCajaCasaMatriz_CAJ
                  .Join(contexto.AperturaCaja_CAJ,
                  op => op.CAE_IdAperturaCaja,
                  ap => ap.APC_IdAperturaCaja,
                  (op, ap) => op)
                  .Where(w => w.CAE_FechaMovimiento >= fechaInicial && w.CAE_FechaMovimiento <= fechaFinal &&
                    w.AperturaCaja_CAJ.AperturaCajaCasaMatriz_CAJ.ACM_IdCasaMatriz == idCasaMatriz)
                  .ToList()
                 .ConvertAll<CACajaCasaMatrizDC>(dtll => new CACajaCasaMatrizDC()
                 {
                     ConceptoCaja = new CAConceptoCajaDC()
                     {
                         IdConceptoCaja = dtll.CAE_IdConceptoCaja,
                         Nombre = dtll.CAE_NombreConceptoCaja,
                         EsIngreso = dtll.CAE_ConceptoEsIngreso
                     },
                     Valor = dtll.CAE_Valor,
                     NumeroDocumento = dtll.CAE_NumeroDocumento,
                     Observacion = dtll.CAE_Observacion,
                     Descripcion = dtll.CAE_Descripcion,
                     FechaMov = dtll.CAE_FechaMovimiento,
                     IdTransaccion = dtll.CAE_IdOperacionCajaCasaMatriz
                 });

                return operaconesCasaMatriz;
            }
        }

        /// <summary>
        /// Metodo para obtener las operaciones del RACOL.
        /// </summary>
        /// <param name="fechaTransaccion">The fecha transaccion.</param>
        /// <param name="idCentroServicio">The id centro servicio.</param>
        /// <returns></returns>
        public List<CACajaCasaMatrizDC> ObtenerTransaccionesRACOL(DateTime fechaTransaccion, long idCentroServicio)
        {
            DateTime fechaInicial, fechaFinal;
            fechaInicial = fechaTransaccion.Date;
            fechaFinal = fechaTransaccion.Date.Add(new TimeSpan(0, 23, 59, 59, 999));

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                List<paObtenerRegitrosTranCajaCentroSvc_CAJ_Result> operacionesRACOL = contexto.paObtenerRegitrosTranCajaCentroSvc_CAJ(idCentroServicio, fechaInicial, fechaFinal)
                  .ToList();

                List<CACajaCasaMatrizDC> operaciones = operacionesRACOL
                  .ConvertAll<CACajaCasaMatrizDC>(ope => new CACajaCasaMatrizDC()
                  {
                      Valor = ope.RTD_ValorServicio,
                      ConceptoCaja = new CAConceptoCajaDC()
                      {
                          IdConceptoCaja = ope.RTD_IdConceptoCaja,
                          Nombre = ope.RTD_NombreConcepto,
                      },
                      Descripcion = ope.RTD_Descripcion,
                      FechaGrabacion = ope.RTD_FechaGrabacion,
                      FechaMov = ope.RTD_FechaGrabacion,
                      NombreCentroServicioRegistra = ope.RTC_NombreCentroServiciosVenta,
                      IdTransaccion = ope.RTD_IdRegistroTransDetalleCaja
                  });

                return operaciones;
            }
        }

        /// <summary>
        /// Obtener las operaciones de caja de Operación Nacional en una fecha
        /// </summary>
        /// <param name="idCasaMatriz">Identificador único de la casa matriz</param>
        /// <param name="fecha">Fecha en la cual se hace la consulta</param>
        /// <returns>Collección con la información de las operaciones</returns>
        public IList<CACajaCasaMatrizDC> ObtenerOperacionesOpn(short idCasaMatriz, DateTime fecha)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                DateTime fechaFin = fecha.Date.Add(new TimeSpan(0, 23, 59, 59, 999));

                return contexto.OperacionCajaOperacionNacional_CAJ
                  .Where(w => w.CON_FechaMovimiento >= fecha.Date && w.CON_FechaMovimiento <= fechaFin &&
                    w.AperturaCaja_CAJ.AperturaCajaCasaMatriz_CAJ.ACM_IdCasaMatriz == idCasaMatriz)
                    .OrderByDescending(o => o.CON_FechaGrabacion)
                  .ToList()
                  .ConvertAll<CACajaCasaMatrizDC>((ope) => new CACajaCasaMatrizDC
                  {
                      Valor = ope.CON_Valor,
                      ConceptoCaja = new CAConceptoCajaDC()
                      {
                          IdConceptoCaja = ope.CON_IdConceptoCaja,
                          Nombre = ope.CON_NombreConceptoCaja
                      },
                      Descripcion = ope.CON_Descripcion,
                      FechaGrabacion = ope.CON_FechaGrabacion,
                      FechaMov = ope.CON_FechaMovimiento
                  });
            }
        }

        /// <summary>
        /// Obtiene la dupla del concepto.
        /// </summary>
        /// <param name="idConcepto">The id concepto.</param>
        /// <returns>Dupla del concepto enviado si existe, de lo contrario retorna nulo</returns>
        public CAConceptoCajaDC ObtenerDuplaConcepto(int idConcepto)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var dplaConcepto = (from DuplaConcepto in contexto.DuplaConceptoCaja_CAJ
                                    join ConcepCaja in contexto.ConceptoCaja_CAJ
                                    on DuplaConcepto.DCC_IdConceptoCajaSecundario equals ConcepCaja.COC_IdConceptoCaja
                                    where DuplaConcepto.DCC_IdConceptoCajaPrimario == idConcepto
                                    select new { ConcepCaja.COC_IdConceptoCaja, ConcepCaja.COC_Nombre, ConcepCaja.COC_EsIngreso })
                                    .FirstOrDefault();

                if (dplaConcepto == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                             .ERROR_DUPLA_NO_EXISTE.ToString(), CACajaServerMensajes
                                                             .CargarMensaje(CAEnumTipoErrorCaja.ERROR_DUPLA_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                CAConceptoCajaDC conceptoCaja = new CAConceptoCajaDC()
                {
                    IdConceptoCaja = dplaConcepto.COC_IdConceptoCaja,
                    Nombre = dplaConcepto.COC_Nombre,
                    EsIngreso = dplaConcepto.COC_EsIngreso
                };

                return conceptoCaja;
            }
        }

        #endregion Racol-Casa Matriz

        #region Consultar Cliente Propietario De Operacion

        /// <summary>
        /// Consultar el id del cliente propietario de una operación
        /// </summary>
        /// <param name="idOperacion">Número de la operación de caja que se quiere consultar</param>
        /// <returns>Id del cliente dueño de la operación, si no pertenece a un cliente retorna null</returns>
        public List<ClienteOperacion_CAJ> ConsultarClientePropDeOperacion(long idOperacionDesde, long idOperacionHasta)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerClientePropOper_CAJ(idOperacionDesde, idOperacionHasta).ToList();
            }
        }

        #endregion Consultar Cliente Propietario De Operacion

        #region Caja

        /// <summary>
        /// Consulta el centro de servicios que recibió el pago de un al cobro
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ConsultarCentroDeServiciosPagoAlCobro(long numeroGuia, out decimal valorCargado)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var CSAlCobro = contexto.paObtenerCSRecacudoAlcobro_CAJ(numeroGuia).FirstOrDefault();
                PUCentroServiciosDC centroServicio = null;

                valorCargado = 0;
                if (CSAlCobro != null)
                {
                    centroServicio = new PUCentroServiciosDC()
                    {
                        IdCentroServicio = CSAlCobro.RTC_IdCentroServiciosVenta,
                        Nombre = CSAlCobro.RTC_NombreCentroServiciosVenta,
                        NoComprobante = CSAlCobro.RTD_NumeroComprobante
                    };
                    valorCargado = (decimal)CSAlCobro.ValorEnCaja;
                }
                return centroServicio;
            }
        }

        /// <summary>
        /// Retorna el id del detalle de movimiento de caja que pertenece a un centro de servicio dados el número de guía y el concepto
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="idConceptoCaja"></param>
        /// <param name="idCentroServicios"></param>
        /// <returns>Ide de detalle de la transacción de caja</returns>
        public long ObtenerIdDetalleMovimientoPorCentroServicio(long numeroGuia, int idConceptoCaja, long idCentroServicios)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CARegTransDetallePorCentroServicio transaccion = contexto.paObtenerRegTransDetallePorCentroServicio_CAJ(numeroGuia, idConceptoCaja, idCentroServicios)
                  .FirstOrDefault();

                if (transaccion == null)
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                            .ERROR_MOVIMIENTO_NO_EXISTE.ToString(), CACajaServerMensajes
                                                            .CargarMensaje(CAEnumTipoErrorCaja.ERROR_MOVIMIENTO_NO_EXISTE));
                    throw new FaultException<ControllerException>(excepcion);
                }

                return transaccion.RTD_IdRegistroTransDetalleCaja;
            }
        }

        /// <summary>
        /// Obtiene el saldo acumulado en caja
        /// </summary>
        /// <param name="idCentroServicios"></param>
        /// <returns></returns>
        public decimal ObtenerSaldoActualCaja(long idCentroServicios)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerSaldoActualCaja_CAJ(idCentroServicios)
                  .FirstOrDefault()
                  .Value;
            }
        }

        /// <summary>
        /// Obtener la info de la caja para cierre.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <param name="idFormaPago">The id forma pago.</param>
        /// <param name="operador">The operador.</param>
        /// <returns></returns>
        public List<CACierreCajaDC> ObtenerInfoCierreCaja(long idAperturaCaja, short idFormaPago, string operador)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                List<CACierreCajaDC> infoCierreCaja = contexto.paObtenerInfoCierreCaja_CAJ(idAperturaCaja, idFormaPago, operador)
                                                  .ToList()
                                                  .ConvertAll<CACierreCajaDC>(info => new CACierreCajaDC()
                                                  {
                                                      IdCierreCaja = info.APC_IdAperturaCaja,
                                                      IdCaja = info.APC_IdCaja,
                                                      FechaApertura = info.APC_FechaGrabacion,
                                                      TotalIngresoEfectivo = info.RVF_IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? info.Ingresos : 0,
                                                      TotalEgresoEfectivo = info.RVF_IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? info.Egresos : 0,
                                                      TotalIngresoOtrasFormas = info.RVF_IdFormaPago != TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? info.Ingresos : 0,
                                                      TotalEgresoOtrasFormas = info.RVF_IdFormaPago != TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? info.Egresos : 0,
                                                      TotalReportar = info.RVF_IdFormaPago == TAConstantesServicios.ID_FORMA_PAGO_CONTADO ? info.Ingresos - info.Egresos : 0,
                                                      UsuarioCaja = info.APC_CreadoPor,
                                                  });

                return infoCierreCaja;
            }
        }

        /// <summary>
        /// Obtener Cierres del Cajero.
        /// </summary>
        /// <param name="idCodigoUsuario">The id codigo usuario.</param>
        /// <returns></returns>

        /// <summary>
        /// Obtener Cierres del Cajero.
        /// </summary>
        /// <param name="idCodigoUsuario">Código del usuario que hace los cierres</param>
        /// <param name="fechaCierre">Fecha de la consulta</param>
        /// <returns>Colección con la información de los cierres del cajero</returns>
        /// <remarks>La fecha de cierre DateTime.MinValue hace la consulta sobre todos los cierres del cajero</remarks>
        public List<CACierreCajaDC> ObtenerCierresCajero(long idCodigoUsuario, long idCentroServicio, DateTime fechaCierre, int indicePagina, int tamanoPagina = 10)
        {
            //List<CACierreCajaDC> listCierres = null;

            bool incluirFecha = fechaCierre != DateTime.MinValue;
            DateTime fechaIni = incluirFecha ? fechaCierre.Date : ConstantesFramework.MinDateTimeController;
            DateTime fechaFin = incluirFecha ? fechaCierre.Date.AddDays(1) : ConstantesFramework.MinDateTimeController;

            DataTable dt = new DataTable();

            using (SqlConnection sqlconn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString))
            {
                sqlconn.Open();
                SqlCommand command = new SqlCommand("paObtenerCierreCajaCentroServicios_CAJ", sqlconn);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 500000;
                command.Parameters.AddWithValue("@CodigoUsuario", idCodigoUsuario);
                command.Parameters.AddWithValue("@IdCentroServicio", idCentroServicio);
                command.Parameters.AddWithValue("@FechaInicial", fechaIni);
                command.Parameters.AddWithValue("@FechaFinal", fechaFin);
                command.Parameters.AddWithValue("@IncluirFiltroFecha", incluirFecha);
                command.Parameters.AddWithValue("@PageIndex", indicePagina);
                command.Parameters.AddWithValue("@PageSize", tamanoPagina);
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);

            }

            List<CACierreCajaDC> listCierres = new List<CACierreCajaDC>();



            listCierres = dt.AsEnumerable().Select(r => new CACierreCajaDC()
            {
                IdCierreCaja = r.Field<long>("CIC_IdCierreCaja"),
                FechaCierre = r.Field<DateTime>("CIC_FechaGrabacion"),
                TotalIngresoEfectivo = r.Field<decimal>("CIC_TotalIngresosEfectivo"),
                TotalEgresoEfectivo = r.Field<decimal>("CIC_TotalEgresosEfectivo"),
                TotalIngresoOtrasFormas = r.Field<decimal>("CIC_TotalIngresosOtrasFormas"),
                TotalEgresoOtrasFormas = r.Field<decimal>("CIC_TotalEgresosOtrasFormas"),
                TotalReportar = r.Field<decimal>("CIC_TotalIngresosEfectivo") - r.Field<decimal>("CIC_TotalEgresosEfectivo"),
                UsuarioCaja = r.Field<string>("APC_CreadoPor"),
                NRegistros = r.Field<int>("TOTAL")

            }).ToList();

            return listCierres ?? new List<CACierreCajaDC>();
        }

        /// <summary>
        /// Metodo que consulta todas las Cajas abiertas
        ///  con su respectivo centro de servicio
        /// </summary>
        /// <returns>lista de cajas abiertas</returns>
        public List<CACierreAutomaticoDC> ObtenerCentrosSvcConCajasAbiertas()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var cajasAbiertas = contexto.AperturaCentroServicos_VCAJ.Where(abierta => abierta.APC_EstaAbierta == true)
                                          .ToList();

                List<CACierreAutomaticoDC> cajas = cajasAbiertas.GroupBy(punto => punto.ACS_IdCentroServicios).Select(elemet => new CACierreAutomaticoDC()
                {
                    IdCentroServicios = elemet.First().ACS_IdCentroServicios,
                    NombreCentroServicio = elemet.First().ACS_NombreCentroServicios,
                    TipoCentroSvc = elemet.First().CES_Tipo
                }).ToList();

                return cajas;
            }
        }

        /// <summary>
        /// Crear aperturas en cero para todos los centros de servicio que no tengan movimientos en una fecha especifica
        /// </summary>
        public void CrearAperturasEnCero(DateTime fechaApertura)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                fechaApertura = fechaApertura.Date;
                contexto.paAperturaCajasEnCero(fechaApertura, fechaApertura.AddDays(1));

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene la Ultima apertura activa del usuario por Centro de
        /// servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <param name="idCaja"></param>
        /// <param name="idCodUsuario"></param>
        /// <returns>el id long de la ultima apertura</returns>
        public long ObtenerUltimaAperturaActiva(long idCentroServicio, int idCaja, long idCodUsuario)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                long ultimaApertura = 0;
                AperturaCentroServicos_VCAJ infoApertura;

                if (idCaja == 0) // para las aperturas de caja 0, no se valida el usuario
                {
                    infoApertura = contexto.AperturaCentroServicos_VCAJ
                    .OrderByDescending(id => id.APC_FechaGrabacion)
                    .FirstOrDefault(caj => caj.ACS_IdCentroServicios == idCentroServicio &&
                                    caj.APC_IdCaja == idCaja && caj.APC_EstaAbierta == true);
                }
                else
                {
                    infoApertura = contexto.AperturaCentroServicos_VCAJ
                     .OrderByDescending(id => id.APC_FechaGrabacion)
                     .FirstOrDefault(caj => caj.ACS_IdCentroServicios == idCentroServicio &&
                                     caj.APC_IdCaja == idCaja && caj.APC_EstaAbierta == true &&
                                     caj.APC_IdCodigoUsuario == idCodUsuario);
                }

                if (infoApertura != null)
                {
                    ultimaApertura = infoApertura.APC_IdAperturaCaja;
                }
                return ultimaApertura;
            }
        }

        /// <summary>
        /// Obtiene la transaccion completa dependiendo del
        /// id del concepto de la caja y del numero de la guia
        /// </summary>
        public CARegistroTransacCajaDC ObtenerTransaccionCaja(long numeroGuia, int idConceptoCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CARegistroTransacCajaDC regTransaccion = new CARegistroTransacCajaDC()
                {
                    RegistrosTransacDetallesCaja = new List<CARegistroTransacCajaDetalleDC>(),
                    InfoAperturaCaja = new CAAperturaCajaDC()
                };

                regTransaccion.RegistrosTransacDetallesCaja = contexto.paObtenerRegTransDetalle_CAJ(numeroGuia, idConceptoCaja).ToList().ConvertAll(r =>
                  new CARegistroTransacCajaDetalleDC()
                  {
                      IdRegistroTranscaccion = r.RTD_IdRegistroTranscaccion,
                      Cantidad = r.RTD_Cantidad,

                      ConceptoCaja = new CAConceptoCajaDC()
                      {
                          IdConceptoCaja = r.RTD_IdConceptoCaja,
                          Nombre = r.RTD_NombreConcepto,
                          EsIngreso = r.RTD_ConceptoEsIngreso,
                      },
                      ConceptoEsIngreso = r.RTD_ConceptoEsIngreso,
                      Descripcion = r.RTD_Descripcion,
                      EstadoFacturacion = (CAEnumEstadoFacturacion)Enum.Parse(typeof(CAEnumEstadoFacturacion), r.RTD_EstadoFacturacion),
                      FechaFacturacion = r.RTD_FechaFacturacion,
                      Numero = r.RTD_Numero,
                      NumeroFactura = r.RTD_NumeroFactura,
                      Observacion = r.RTD_Observacion,
                      ValorDeclarado = r.RTD_ValorDeclarado,
                      ValoresAdicionales = r.RTD_ValoresAdicionales,
                      ValorImpuestos = r.RTD_ValorImpuestos,
                      ValorPrimaSeguros = r.RTD_ValorPrimaSeguros,
                      ValorRetenciones = r.RTD_ValorRetenciones,
                      ValorServicio = r.RTD_ValorServicio,
                      ValorTercero = r.RTD_ValorTercero
                  });

                if (regTransaccion.RegistrosTransacDetallesCaja.Count > 0)
                {
                    var regTrans = contexto.paObtenerRegistroTrans_CAJ(regTransaccion.RegistrosTransacDetallesCaja.First().IdRegistroTranscaccion).FirstOrDefault();

                    if (regTransaccion != null)
                    {
                        regTransaccion.InfoAperturaCaja.IdAperturaCaja = regTrans.RTC_IdAperturaCaja;
                        regTransaccion.IdCentroResponsable = regTrans.RTC_IdCentroServiosResponsable;
                        regTransaccion.IdCentroServiciosVenta = regTrans.RTC_IdCentroServiciosVenta;
                        regTransaccion.NombreCentroResponsable = regTrans.RTC_NombreCtroSvcsResponsable;
                        regTransaccion.NombreCentroServiciosVenta = regTrans.RTC_NombreCentroServiciosVenta;
                        regTransaccion.TipoDatosAdicionales = (CAEnumTipoDatosAdicionales)Enum.Parse(typeof(CAEnumTipoDatosAdicionales), regTrans.RTC_TipoDatosAdicionales);
                        regTransaccion.ValorTotal = regTrans.RTC_ValorTotal;
                        regTransaccion.TotalImpuestos = regTrans.RTC_TotalImpuestos;
                        regTransaccion.TotalRetenciones = regTrans.RTC_TotalRetenciones;

                        AperturaCaja_CAJ infoCaja = contexto.AperturaCaja_CAJ.FirstOrDefault(idCaj => idCaj.APC_IdAperturaCaja == regTrans.RTC_IdAperturaCaja);
                        if (infoCaja != null)
                        {
                            regTransaccion.InfoAperturaCaja = new CAAperturaCajaDC()
                            {
                                IdCaja = infoCaja.APC_IdCaja,
                                IdCodigoUsuario = infoCaja.APC_IdCodigoUsuario,
                            };
                        }
                    }

                    var regFormaPago = contexto.paObtenerRegFormaPago_CAJ(regTransaccion.RegistrosTransacDetallesCaja.First().IdRegistroTranscaccion);
                    if (regFormaPago != null)
                    {
                        regTransaccion.RegistroVentaFormaPago = regFormaPago.ToList().ConvertAll(r =>
                          new CARegistroVentaFormaPagoDC()
                          {
                              Campo01 = r.RVF_Campo1,
                              Campo02 = r.RVF_Campo2,
                              Descripcion = r.RVF_DescripcionFormaPago,
                              IdFormaPago = r.RVF_IdFormaPago,
                              NumeroAsociado = r.RVF_NumeroAsociado,
                              Valor = r.RVF_Valor
                          });
                    }
                }

                return regTransaccion;
            }
        }

        /// <summary>
        /// Obtiene la Transaccion detalle de
        /// un giro
        /// </summary>
        /// <param name="numeroGiro">se valida con RTD_numero</param>
        /// <param name="idConceptoCaja">es el concepto deCaja</param>
        /// <returns></returns>
        public CARegistroTransacCajaDetalleDC ObtenerDetalleTransaccion(long numero, int idConceptoCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerRegTransDetalle_CAJ(numero, idConceptoCaja).ToList().ConvertAll(r =>
                  new CARegistroTransacCajaDetalleDC()
                  {
                      IdRegistroTranscaccion = r.RTD_IdRegistroTranscaccion,
                      Cantidad = r.RTD_Cantidad,

                      ConceptoCaja = new CAConceptoCajaDC()
                      {
                          IdConceptoCaja = r.RTD_IdConceptoCaja,
                      },
                      ConceptoEsIngreso = r.RTD_ConceptoEsIngreso,
                      Descripcion = r.RTD_Descripcion,
                      EstadoFacturacion = (CAEnumEstadoFacturacion)Enum.Parse(typeof(CAEnumEstadoFacturacion), r.RTD_EstadoFacturacion),
                      FechaFacturacion = r.RTD_FechaFacturacion,
                      Numero = r.RTD_Numero,
                      NumeroFactura = r.RTD_NumeroFactura,
                      Observacion = r.RTD_Observacion,
                      ValorDeclarado = r.RTD_ValorDeclarado,
                      ValoresAdicionales = r.RTD_ValoresAdicionales,
                      ValorImpuestos = r.RTD_ValorImpuestos,
                      ValorPrimaSeguros = r.RTD_ValorPrimaSeguros,
                      ValorRetenciones = r.RTD_ValorRetenciones,
                      ValorServicio = r.RTD_ValorServicio,
                      ValorTercero = r.RTD_ValorTercero
                  }).FirstOrDefault();
            }
        }

        /// <summary>
        /// Obtener información complementaria para el cierre de una caja
        /// </summary>
        /// <param name="idCierre">Identificacdor del cierre de caja</param>
        /// <returns>Información complementaria del cierre de caja</returns>
        public CAInfoComplementariaCierreCajaDC ObtenerInformacionComplementariaCierreCaja(long idCierre)
        {
            CAInfoComplementariaCierreCajaDC cierre = null;

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CierreCajaUsuarioCierre_VCAJ infoCierre = contexto.CierreCajaUsuarioCierre_VCAJ
                  .Where(c => c.CIC_IdCierreCaja == idCierre)
                  .FirstOrDefault();

                if (infoCierre != null)
                {
                    cierre = new CAInfoComplementariaCierreCajaDC
                    {
                        NombreCompletoUsuario = infoCierre.NombreCompleto,
                        FechaCierre = infoCierre.CIC_FechaGrabacion,
                        IdCierreCaja = infoCierre.CIC_IdCierreCaja,
                        TotalEgresosEfectivo = infoCierre.CIC_TotalEgresosEfectivo,
                        TotalIngresosEfectivo = infoCierre.CIC_TotalIngresosEfectivo,
                        TotalIngresosOtrasFormas = infoCierre.CIC_TotalIngresosOtrasFormas,
                        FechaApertura = infoCierre.FechaApertura,
                        IdCAja = infoCierre.APC_IdCaja
                    };
                }

                return cierre;
            }
        }

        #endregion Caja

        #region Consultar caja afectada al cobro
        /// <summary>
        /// Consulta el Centro de servicio cuya caja se afectó por concepto de pago de al cobro
        /// </summary>
        /// <param name="numeroAlCobro"></param>
        /// <returns></returns>
        public PUCentroServiciosDC ConsultarCajaAfectadaPorPagoDeAlCobro(long numeroAlCobro, out decimal valorCargado)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                valorCargado = 0;
                var CSAlCobroCaja = contexto.paObtenerCajaAfectadaAlCobro_CAJ(numeroAlCobro).FirstOrDefault();

                if (CSAlCobroCaja != null && CSAlCobroCaja.IdCS != null)
                {
                    valorCargado = CSAlCobroCaja.ValorTotal == null ? 0 : CSAlCobroCaja.ValorTotal.Value;

                    return new PUCentroServiciosDC()
                    {
                        IdCentroServicio = CSAlCobroCaja.IdCS.Value,
                        Nombre = CSAlCobroCaja.NombreCS,
                        NoComprobante = CSAlCobroCaja.NoComprobante
                    };
                }
                return null;
            }
        }
        #endregion

        #endregion Consultas

        #region Insertar

        #region Operaciones Puntos

        /// <summary>
        /// Adicionar El movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">es la Propiedad del movimiento caja.</param>
        /// /// <param name="anulacionGuia">indica si se esta anulando una guia</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, bool anulacionGuia = false)
        {
            CAIdTransaccionesCajaDC idRegistros = new CAIdTransaccionesCajaDC();

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                //Valido si hay forma de pago con pin Prepago para Validar saldo y
                //continuar con la operacion
                if (!anulacionGuia)
                    ValidarSaldoPinPrepago(movimientoCaja);
                else
                    RetornarSaldoPinPrepago(movimientoCaja, contexto);

                //Insercion de Registro Transaccion caja y de Dtlle
                long idRegistroCaja = InsertarRegistroTransaccion(movimientoCaja, movimientoCaja.InfoAperturaCaja.IdAperturaCaja, idRegistros, contexto);

                //Registrar Formas de Pago
                InsertarFormasDePago(movimientoCaja, idRegistroCaja, contexto, anulacionGuia);

                //Insercion de Tipos de Datos Adicionales
                InsertarTiposDatosAdicionales(movimientoCaja, idRegistroCaja, contexto);

                // También debe retornar el número de comprobante relacionado
                idRegistros.NumeroConsecutivo = movimientoCaja.RegistrosTransacDetallesCaja.First().NumeroComprobante;

                ///Valida si es anulacion de guia, esta validacion se hace para no alterar el comportamiento original del metodo debido
                ///a que la anulacion de la guia es un comportamieto adicionado al metodo original
                if (anulacionGuia)
                    contexto.SaveChanges();
            }
            return idRegistros;
        }

        /// <summary>
        /// Adicionar El movimiento caja.
        /// </summary>
        /// <param name="movimientoCaja">es la Propiedad del movimiento caja.</param>
        /// /// <param name="anulacionGuia">indica si se esta anulando una guia</param>
        public CAIdTransaccionesCajaDC AdicionarMovimientoCaja(CARegistroTransacCajaDC movimientoCaja, SqlConnection conexion, SqlTransaction transaccion, bool anulacionGuia = false)
        {
            CAIdTransaccionesCajaDC idRegistros = new CAIdTransaccionesCajaDC();

            //Valido si hay forma de pago con pin Prepago para Validar saldo y
            //continuar con la operacion
            if (!anulacionGuia)
                ValidarSaldoPinPrepago(movimientoCaja, conexion, transaccion);
            else
                RetornarSaldoPinPrepago(movimientoCaja, conexion, transaccion);

            //Insercion de Registro Transaccion caja y de Dtlle
            long idRegistroCaja = InsertarRegistroTransaccion(movimientoCaja, movimientoCaja.InfoAperturaCaja.IdAperturaCaja, idRegistros, conexion, transaccion);

            //Registrar Formas de Pago
            InsertarFormasDePago(movimientoCaja, idRegistroCaja, conexion, transaccion, anulacionGuia);

            //Insercion de Tipos de Datos Adicionales
            InsertarTiposDatosAdicionales(movimientoCaja, idRegistroCaja, conexion, transaccion);

            // También debe retornar el número de comprobante relacionado
            idRegistros.NumeroConsecutivo = movimientoCaja.RegistrosTransacDetallesCaja.First().NumeroComprobante;



            return idRegistros;
        }


        public void AdicionarDetalleCaja(CARegistroTransacCajaDetalleDC registroDetalle, SqlConnection conexion, SqlTransaction transaccion, long IdCentroServiciosVenta, string Usuario)
        {
            SqlCommand cmdDestalleCaja = new SqlCommand("paCrearRegTransCajaDetalle_CAJ", conexion, transaccion);

            cmdDestalleCaja.CommandType = CommandType.StoredProcedure;

            cmdDestalleCaja.Parameters.AddWithValue("@idRegistroTranscaccion", registroDetalle.IdRegistroTranscaccion);
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@idConceptoCaja", registroDetalle.ConceptoCaja.IdConceptoCaja));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@nombreConcepto", registroDetalle.ConceptoCaja.Nombre));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@conceptoEsIngreso", registroDetalle.ConceptoEsIngreso));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@cantidad", registroDetalle.Cantidad));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorServicio", registroDetalle.ValorServicio));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valoresAdicionales", registroDetalle.ValoresAdicionales));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorTercero", registroDetalle.ValorTercero));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorImpuestos", registroDetalle.ValorImpuestos));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorRetenciones", registroDetalle.ValorRetenciones));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorPrimaSeguros", registroDetalle.ValorPrimaSeguros));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorDeclarado", registroDetalle.ValorDeclarado));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@numero", registroDetalle.Numero));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@idCuentaExterna", Convert.ToInt16(CAConstantesCaja.VALOR_VACIO)));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@numeroFactura", registroDetalle.NumeroFactura));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@estadoFacturacion", registroDetalle.EstadoFacturacion.ToString()));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@fechaFacturacion", registroDetalle.FechaFacturacion));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@observacion", registroDetalle.Observacion == null ? string.Empty : registroDetalle.Observacion));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@descripcion", registroDetalle.Descripcion == null ? string.Empty : registroDetalle.Descripcion));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@numeroComprobante", registroDetalle.NumeroComprobante == null ? string.Empty : registroDetalle.NumeroComprobante));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@creadoPor", Usuario));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@minDate", ConstantesFramework.MinDateTimeController));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@CentroServicios", IdCentroServiciosVenta));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@ValorEfectivo", registroDetalle.ValorServicio));
            cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@RTC_FechaGrabacion", DateTime.Now));

            long idRegistroDet = Convert.ToInt64(cmdDestalleCaja.ExecuteScalar());


            //TODO:revisar el registroDetalle de impuestos
            //Inserto registro de Impuestos
            CARegistroTransacCajaDC movimientoCaja = new CARegistroTransacCajaDC { Usuario = Usuario };
            InsertarRegistroImpuesto(movimientoCaja, registroDetalle, idRegistroDet, conexion, transaccion);

            //Inserto las Transacciones Adicionales de Operaciones Caja           
            AdicionarDetallesAdicionales(idRegistroDet, registroDetalle.DetalleAdicional, conexion, transaccion);

            ////Agrego los datos de id transaccion detalle transaccion
            //IdRegistros.IdTransaccionCajaDtll = new List<long>();
            //IdRegistros.IdTransaccionCajaDtll.Add(idRegistroDet);

        }

      
        /// <summary>
        /// Insercion de Registro Transaccion caja y de Dtlle
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idAperturaCaja">The id apertura caja.</param>
        /// <param name="IdRegistros">The id registros.</param>
        /// <param name="contexto">The contexto.</param>
        /// <returns></returns>
        public long InsertarRegistroTransaccion(CARegistroTransacCajaDC movimientoCaja, long idAperturaCaja, CAIdTransaccionesCajaDC IdRegistros, ModeloCajas contexto)
        {
            long idRegistroCaja;

            //se hace la insercion en el registro transaccional de la Caja y se toma el Id
            idRegistroCaja = Convert.ToInt64(
              contexto.paCrearRegistroTransCaja_CAJ(
                  movimientoCaja.Usuario, idAperturaCaja, movimientoCaja.IdCentroServiciosVenta, movimientoCaja.NombreCentroServiciosVenta,
                  movimientoCaja.IdCentroResponsable, movimientoCaja.NombreCentroResponsable, movimientoCaja.ValorTotal,
                  movimientoCaja.TotalImpuestos, movimientoCaja.TotalRetenciones, movimientoCaja.TipoDatosAdicionales.ToString(),
                  movimientoCaja.Usuario, DateTime.Now)
              .FirstOrDefault().idRegistroTransaccion);

            //clase para el retorno
            IdRegistros.IdTransaccionCaja = idRegistroCaja;

            movimientoCaja.RegistrosTransacDetallesCaja
              .ToList()
              .ForEach(AddDtll =>
              {
                  //Dependiendo del tipo de Cliente se llenan unos campos
                  if (movimientoCaja.TipoDatosAdicionales == CAEnumTipoDatosAdicionales.CRE)
                  {
                      //AddDtll.NumeroFactura = CAConstantesCaja.VALOR_VACIO;
                      AddDtll.EstadoFacturacion = CAEnumEstadoFacturacion.PED;
                      AddDtll.FechaFacturacion = ConstantesFramework.MinDateTimeController;
                  }
                  else
                  {
                      //Rafram 20121231 se deja el valor que traiga por defecto el campo numero Factura
                      //AddDtll.NumeroFactura = AddDtll.Numero.ToString();
                      AddDtll.EstadoFacturacion = CAEnumEstadoFacturacion.FAC;
                      AddDtll.FechaFacturacion = DateTime.Now;
                  }

                  decimal valorEfectivo = movimientoCaja.RegistroVentaFormaPago.Where(fp => fp.IdFormaPago == CAConstantesCaja.FORMA_PAGO_EFECTIVO).Sum(val => val.Valor);

                  long? idRegistroDet = contexto.paCrearRegTransCajaDetalle_CAJ(idRegistroCaja, AddDtll.ConceptoCaja.IdConceptoCaja, AddDtll.ConceptoCaja.Nombre, AddDtll.ConceptoEsIngreso, AddDtll.Cantidad, AddDtll.ValorServicio, AddDtll.ValoresAdicionales,
                                                                          AddDtll.ValorTercero, AddDtll.ValorImpuestos, AddDtll.ValorRetenciones, AddDtll.ValorPrimaSeguros, AddDtll.ValorDeclarado, AddDtll.Numero, Convert.ToInt16(CAConstantesCaja.VALOR_VACIO),
                                                                          AddDtll.NumeroFactura, AddDtll.EstadoFacturacion.ToString(), AddDtll.FechaFacturacion, AddDtll.Observacion == null ? string.Empty : AddDtll.Observacion, AddDtll.Descripcion == null ? string.Empty : AddDtll.Descripcion,
                                                                          AddDtll.NumeroComprobante == null ? string.Empty : AddDtll.NumeroComprobante, movimientoCaja.Usuario, ConstantesFramework.MinDateTimeController, movimientoCaja.IdCentroServiciosVenta, valorEfectivo, DateTime.Now).FirstOrDefault().IdDetalle;

                  //Inserto registro de Impuestos
                  InsertarRegistroImpuesto(movimientoCaja, contexto, AddDtll, idRegistroDet);

                  //Inserto las Transacciones Adicionales de Operaciones Caja
                  AdicionarDetallesAdicionales(contexto, idRegistroDet.Value, AddDtll.DetalleAdicional);

                  //Agrego los datos de id transaccion detalle transaccion
                  if (idRegistroDet != null)
                  {
                      IdRegistros.IdTransaccionCajaDtll = new List<long>();
                      IdRegistros.IdTransaccionCajaDtll.Add(idRegistroDet.HasValue ? idRegistroDet.Value : 0);
                  }
              });
            return idRegistroCaja;
        }

        /// <summary>
        /// Insercion de Registro Transaccion caja y de Dtlle
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idAperturaCaja">The id apertura caja.</param>
        /// <param name="IdRegistros">The id registros.</param>
        /// <param name="contexto">The contexto.</param>
        /// <returns></returns>
        public long InsertarRegistroTransaccion(CARegistroTransacCajaDC movimientoCaja, long idAperturaCaja, CAIdTransaccionesCajaDC IdRegistros, SqlConnection conexion, SqlTransaction transaccion)
        {
            long idRegistroCaja;

            SqlCommand cmd = new SqlCommand("paCrearRegistroTransCaja_CAJ", conexion, transaccion);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_Usuario", movimientoCaja.Usuario));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_IdAperturaCaja", idAperturaCaja));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_IdCentroServiciosVenta", movimientoCaja.IdCentroServiciosVenta));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_NombreCentroServiciosVenta", movimientoCaja.NombreCentroServiciosVenta));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_IdCentroServiciosResponsable", movimientoCaja.IdCentroResponsable));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_NombreCtroSvcsResponsable", movimientoCaja.NombreCentroResponsable));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_ValorTotal", movimientoCaja.ValorTotal));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_TotalImpuestos", movimientoCaja.TotalImpuestos));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_TotalRetenciones", movimientoCaja.TotalRetenciones));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_TipoDatosAdicionales", movimientoCaja.TipoDatosAdicionales.ToString()));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_CreadoPor", movimientoCaja.Usuario));
            cmd.Parameters.Add(Utilidades.AddParametro("@RTC_FechaGrabacion", DateTime.Now));

            //se hace la insercion en el registro transaccional de la Caja y se toma el Id
            idRegistroCaja = Convert.ToInt64(cmd.ExecuteScalar());



            //clase para el retorno
            IdRegistros.IdTransaccionCaja = idRegistroCaja;

            movimientoCaja.RegistrosTransacDetallesCaja
              .ToList()
              .ForEach(AddDtll =>
              {
                  //Dependiendo del tipo de Cliente se llenan unos campos
                  if (movimientoCaja.TipoDatosAdicionales == CAEnumTipoDatosAdicionales.CRE)
                  {
                      //AddDtll.NumeroFactura = CAConstantesCaja.VALOR_VACIO;
                      AddDtll.EstadoFacturacion = CAEnumEstadoFacturacion.PED;
                      AddDtll.FechaFacturacion = ConstantesFramework.MinDateTimeController;
                  }
                  else
                  {
                      //Rafram 20121231 se deja el valor que traiga por defecto el campo numero Factura
                      //AddDtll.NumeroFactura = AddDtll.Numero.ToString();
                      AddDtll.EstadoFacturacion = CAEnumEstadoFacturacion.FAC;
                      AddDtll.FechaFacturacion = DateTime.Now;
                  }

                  decimal valorEfectivo = movimientoCaja.RegistroVentaFormaPago.Where(fp => fp.IdFormaPago == CAConstantesCaja.FORMA_PAGO_EFECTIVO).Sum(val => val.Valor);

                  SqlCommand cmdDestalleCaja = new SqlCommand("paCrearRegTransCajaDetalle_CAJ", conexion, transaccion);

                  cmdDestalleCaja.CommandType = CommandType.StoredProcedure;

                  cmdDestalleCaja.Parameters.AddWithValue("@idRegistroTranscaccion", idRegistroCaja);
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@idConceptoCaja", AddDtll.ConceptoCaja.IdConceptoCaja));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@nombreConcepto", AddDtll.ConceptoCaja.Nombre));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@conceptoEsIngreso", AddDtll.ConceptoEsIngreso));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@cantidad", AddDtll.Cantidad));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorServicio", AddDtll.ValorServicio));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valoresAdicionales", AddDtll.ValoresAdicionales));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorTercero", AddDtll.ValorTercero));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorImpuestos", AddDtll.ValorImpuestos));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorRetenciones", AddDtll.ValorRetenciones));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorPrimaSeguros", AddDtll.ValorPrimaSeguros));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@valorDeclarado", AddDtll.ValorDeclarado));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@numero", AddDtll.Numero));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@idCuentaExterna", Convert.ToInt16(CAConstantesCaja.VALOR_VACIO)));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@numeroFactura", AddDtll.NumeroFactura));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@estadoFacturacion", AddDtll.EstadoFacturacion.ToString()));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@fechaFacturacion", AddDtll.FechaFacturacion));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@observacion", AddDtll.Observacion == null ? string.Empty : AddDtll.Observacion));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@descripcion", AddDtll.Descripcion == null ? string.Empty : AddDtll.Descripcion));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@numeroComprobante", AddDtll.NumeroComprobante == null ? string.Empty : AddDtll.NumeroComprobante));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@creadoPor", movimientoCaja.Usuario));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@minDate", ConstantesFramework.MinDateTimeController));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@CentroServicios", movimientoCaja.IdCentroServiciosVenta));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@ValorEfectivo", valorEfectivo));
                  cmdDestalleCaja.Parameters.Add(Utilidades.AddParametro("@RTC_FechaGrabacion", DateTime.Now));

                  long idRegistroDet = Convert.ToInt64(cmdDestalleCaja.ExecuteScalar());

                  //Inserto registro de Impuestos
                  InsertarRegistroImpuesto(movimientoCaja, AddDtll, idRegistroDet, conexion, transaccion);

                  //Inserto las Transacciones Adicionales de Operaciones Caja
                  AdicionarDetallesAdicionales(idRegistroDet, AddDtll.DetalleAdicional, conexion, transaccion);

                  //Agrego los datos de id transaccion detalle transaccion

                  IdRegistros.IdTransaccionCajaDtll = new List<long>();
                  IdRegistros.IdTransaccionCajaDtll.Add(idRegistroDet);

              });
            return idRegistroCaja;
        }


        /// <summary>
        ///Se registra la transaccion adicional por una operacion de caja ppal
        /// </summary>
        /// <param name="idDetalle">id de la tranasaccion del detalle</param>
        /// <param name="detalle">detalle de la transaccion</param>
        public void AdicionarDetallesAdicionales(ModeloCajas contexto, long idDetalle, CARegistroTransacCajaDetallAdicionalDC detalle)
        {
            if (detalle != null && detalle.Adicional01 != null)
            {
                contexto.paCrearRegTransCajaDetlleAdicional_CAJ(idDetalle, detalle.IdSucursal.Value, detalle.Adicional01, detalle.Adicional02, detalle.Adicional03, ControllerContext.Current.Usuario);
            }
        }

        /// <summary>
        ///Se registra la transaccion adicional por una operacion de caja ppal
        /// </summary>
        /// <param name="idDetalle">id de la tranasaccion del detalle</param>
        /// <param name="detalle">detalle de la transaccion</param>
        public void AdicionarDetallesAdicionales(long idDetalle, CARegistroTransacCajaDetallAdicionalDC detalle, SqlConnection conexion, SqlTransaction transaccion)
        {

            if (detalle != null && detalle.Adicional01 != null)
            {
                SqlCommand cmd = new SqlCommand("paCrearRegTransCajaDetlleAdicional_CAJ", conexion, transaccion);
                cmd.Parameters.Add(Utilidades.AddParametro("@idRegistroDetalleTranscaccion", idDetalle));
                cmd.Parameters.Add(Utilidades.AddParametro("@idSucursal", detalle.IdSucursal.Value));
                cmd.Parameters.Add(Utilidades.AddParametro("@Adicional1", detalle.Adicional01));
                cmd.Parameters.Add(Utilidades.AddParametro("@Adicional2", detalle.Adicional02));
                cmd.Parameters.Add(Utilidades.AddParametro("@Adicional3", detalle.Adicional03));
                cmd.Parameters.Add(Utilidades.AddParametro("@creadoPor", ControllerContext.Current.Usuario));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserta los registros de los Impuestos.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="contexto">The contexto.</param>
        /// <param name="AddDtll">The add DTLL.</param>
        /// <param name="idRegistroDet">The id registro det.</param>
        public void InsertarRegistroImpuesto(CARegistroTransacCajaDC movimientoCaja, CARegistroTransacCajaDetalleDC AddDtll, long idRegistroDet, SqlConnection conexion, SqlTransaction transaccion)
        {

            if (AddDtll.LtsImpuestos != null)
            {
                AddDtll.LtsImpuestos.ForEach(Imp =>
                {
                    SqlCommand cmd = new SqlCommand("paCrearRegTransImpuestos_CAJ", conexion, transaccion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(Utilidades.AddParametro("@RTI_IdRegTransDtllCaja", idRegistroDet));
                    cmd.Parameters.Add(Utilidades.AddParametro("@RTI_IdImpuesto", Imp.InfoImpuesto.IdImpuesto));
                    cmd.Parameters.Add(Utilidades.AddParametro("@RTI_DescripcionImpuesto", Imp.InfoImpuesto.DescripcionImpuesto));
                    cmd.Parameters.Add(Utilidades.AddParametro("@RTI_ValorConfigurado", Imp.InfoImpuesto.ValorImpuesto));
                    cmd.Parameters.Add(Utilidades.AddParametro("@RTI_IdCuentaExterna", ObtenerConceptoPorId(AddDtll.ConceptoCaja.IdConceptoCaja, conexion, transaccion)));
                    cmd.Parameters.Add(Utilidades.AddParametro("@@RTI_Valor", Imp.ValorImpuestoLiquidado));
                    cmd.Parameters.Add(Utilidades.AddParametro("@@RTI_CreadoPor", movimientoCaja.Usuario));

                    cmd.ExecuteNonQuery();

                });
            }
        }

        /// <summary>
        /// Inserta los registros de los Impuestos.
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="contexto">The contexto.</param>
        /// <param name="AddDtll">The add DTLL.</param>
        /// <param name="idRegistroDet">The id registro det.</param>
        public void InsertarRegistroImpuesto(CARegistroTransacCajaDC movimientoCaja, ModeloCajas contexto, CARegistroTransacCajaDetalleDC AddDtll, long? idRegistroDet)
        {
            if (AddDtll.LtsImpuestos != null)
            {
                AddDtll.LtsImpuestos.ForEach(Imp =>
                {
                    contexto.paCrearRegTransImpuestos_CAJ(idRegistroDet, Imp.InfoImpuesto.IdImpuesto, Imp.InfoImpuesto.DescripcionImpuesto, Imp.InfoImpuesto.ValorImpuesto,
                                                          ObtenerConceptoPorId(AddDtll.ConceptoCaja.IdConceptoCaja).IdCuentaExterna, Imp.ValorImpuestoLiquidado, movimientoCaja.Usuario);
                });
            }
        }

        /// <summary>
        ///Valido si hay forma de pago con pin Prepago para Validar saldo
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public void ValidarSaldoPinPrepago(CARegistroTransacCajaDC movimientoCaja)
        {
            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                //Se valida el Valor de pin Prepago
                if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO && AddFormaPag.Valor != 0)
                {
                    if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                    {
                        ValidarSaldoPrepago(Convert.ToInt64(AddFormaPag.NumeroAsociado), AddFormaPag.Valor);
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                              .ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO.ToString(), CACajaServerMensajes
                                                              .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
            });
        }
        /// <summary>
        ///Valido si hay forma de pago con pin Prepago para Validar saldo
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public void ValidarSaldoPinPrepago(CARegistroTransacCajaDC movimientoCaja, SqlConnection conexion, SqlTransaction transaccion)
        {



            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                //Se valida el Valor de pin Prepago
                if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO && AddFormaPag.Valor != 0)
                {
                    if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                    {
                        ValidarSaldoPrepago(Convert.ToInt64(AddFormaPag.NumeroAsociado), AddFormaPag.Valor, conexion, transaccion);
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                              .ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO.ToString(), CACajaServerMensajes
                                                              .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
            });
        }



        /// <summary>
        /// Retorna el valor de una anulacion a un prin prepago
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public void RetornarSaldoPinPrepago(CARegistroTransacCajaDC movimientoCaja, ModeloCajas contexto)
        {
            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO && AddFormaPag.Valor != 0)
                {
                    if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                    {
                        long numeroPin = Convert.ToInt64(AddFormaPag.NumeroAsociado);
                        PinPrepago_CAJ pinPrepagoResult = contexto.PinPrepago_CAJ.Where(r => r.PIP_Pin == numeroPin)
                    .FirstOrDefault();
                        if (pinPrepagoResult != null)
                        {
                            pinPrepagoResult.PIP_Saldo = pinPrepagoResult.PIP_Saldo + AddFormaPag.Valor;
                        }
                    }
                    else
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                              .ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO.ToString(), CACajaServerMensajes
                                                              .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_SIN_NUMERO_ASOCIADO));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                }
            });
        }
        /// <summary>
        /// Retorna el valor de una anulacion a un prin prepago
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        public void RetornarSaldoPinPrepago(CARegistroTransacCajaDC movimientoCaja, SqlConnection conexion, SqlTransaction transaccion)
        {

            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO && AddFormaPag.Valor != 0)
                {
                    if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                    {
                        long numeroPin = Convert.ToInt64(AddFormaPag.NumeroAsociado);

                        string cmdText = @"UPDATE  PinPrepago_CAJ WITH(ROWLOCK)
                                           SET  PIP_Saldo = PIP_Saldo+@nuevoSaldo                                                   
                                           WHERE PIP_Pin=@pin";
                        SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@pin", numeroPin);
                        cmd.Parameters.AddWithValue("@nuevoSaldo", AddFormaPag.Valor);
                        cmd.ExecuteNonQuery();
                    }
                }
            });
        }

        /// <summary>
        ///Insercion de Tipos de Datos Adicionales
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idRegistroCaja">The id registro caja.</param>
        /// <param name="contexto">The contexto.</param>
        public void InsertarTiposDatosAdicionales(CARegistroTransacCajaDC movimientoCaja, long idRegistroCaja, ModeloCajas contexto)
        {
            if (movimientoCaja.TipoDatosAdicionales == CAEnumTipoDatosAdicionales.CRE)
            {
                if (movimientoCaja.RegistroVentaClienteCredito != null)
                {
                    // se ejecuta el Procedimiento Almacenado de los datos del Cliente Credito
                    contexto.paCrearRegClienteCredito_CAJ(idRegistroCaja, movimientoCaja.RegistroVentaClienteCredito.IdCliente,
                                                          movimientoCaja.RegistroVentaClienteCredito.NombreCliente,
                                                          movimientoCaja.RegistroVentaClienteCredito.NitCliente,
                                                          movimientoCaja.RegistroVentaClienteCredito.IdContrato,
                                                          movimientoCaja.RegistroVentaClienteCredito.NumeroContrato,
                                                          movimientoCaja.RegistroVentaClienteCredito.IdSucursal,
                                                          movimientoCaja.RegistroVentaClienteCredito.NombreSucursal,
                                                          movimientoCaja.Usuario);
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                      .ERROR_CLIENTE_CREDITO_SIN_DATOS.ToString(), CACajaServerMensajes
                                                                      .CargarMensaje(CAEnumTipoErrorCaja.ERROR_CLIENTE_CREDITO_SIN_DATOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }


        /// <summary>
        ///Insercion de Tipos de Datos Adicionales
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idRegistroCaja">The id registro caja.</param>
        /// <param name="contexto">The contexto.</param>
        public void InsertarTiposDatosAdicionales(CARegistroTransacCajaDC movimientoCaja, long idRegistroCaja, SqlConnection conexion, SqlTransaction transaccion)
        {
            if (movimientoCaja.TipoDatosAdicionales == CAEnumTipoDatosAdicionales.CRE)
            {
                if (movimientoCaja.RegistroVentaClienteCredito != null)
                {
                    SqlCommand cmd = new SqlCommand("paCrearRegClienteCredito_CAJ", conexion, transaccion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(Utilidades.AddParametro("@idRegistroVenta", idRegistroCaja));
                    cmd.Parameters.Add(Utilidades.AddParametro("@idCliente", movimientoCaja.RegistroVentaClienteCredito.IdCliente));
                    cmd.Parameters.Add(Utilidades.AddParametro("@nombreCliente", movimientoCaja.RegistroVentaClienteCredito.NombreCliente));
                    cmd.Parameters.Add(Utilidades.AddParametro("@nit", movimientoCaja.RegistroVentaClienteCredito.NitCliente));
                    cmd.Parameters.Add(Utilidades.AddParametro("@idContrato", movimientoCaja.RegistroVentaClienteCredito.IdContrato));
                    cmd.Parameters.Add(Utilidades.AddParametro("@numeroContrato", movimientoCaja.RegistroVentaClienteCredito.NumeroContrato));
                    cmd.Parameters.Add(Utilidades.AddParametro("@idSucursal", movimientoCaja.RegistroVentaClienteCredito.IdSucursal));
                    cmd.Parameters.Add(Utilidades.AddParametro("@nombreSucursal", movimientoCaja.RegistroVentaClienteCredito.NombreSucursal));
                    cmd.Parameters.Add(Utilidades.AddParametro("@creadoPor", movimientoCaja.Usuario));

                    // se ejecuta el Procedimiento Almacenado de los datos del Cliente Credito
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                      .ERROR_CLIENTE_CREDITO_SIN_DATOS.ToString(), CACajaServerMensajes
                                                                      .CargarMensaje(CAEnumTipoErrorCaja.ERROR_CLIENTE_CREDITO_SIN_DATOS));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }


        /// <summary>
        /// Se Registran las Formas de Pago
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idRegistroCaja">The id registro caja.</param>
        /// <param name="contexto">The contexto.</param>
        public void InsertarFormasDePago(CARegistroTransacCajaDC movimientoCaja, long idRegistroCaja, ModeloCajas contexto, bool anulacionGuia = false)
        {
            //se recorren las formas de pago y se insertan las que tienen valor>0
            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                if (AddFormaPag.Valor > 0)
                {
                    if (!anulacionGuia)

                        //Actualiza el Valor de pin Prepago
                        if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO)
                        {
                            if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                            {
                                ActualizarSaldoPinPrepago(Convert.ToInt64(AddFormaPag.NumeroAsociado), AddFormaPag.Valor);
                            }
                        }

                    //Diego informa: que incluso siendo una anulacion de guia, se debe insertar en la tabla de formas de pago
                    //Se ejecuta el Procedimiento Almacenado de las formas de Pago
                    contexto.paCrearRegTransFormasPago_CAJ(idRegistroCaja, AddFormaPag.IdFormaPago, AddFormaPag.Descripcion, AddFormaPag.Valor,
                                                            AddFormaPag.NumeroAsociado, AddFormaPag.Campo01, AddFormaPag.Campo02,
                                                            movimientoCaja.Usuario);
                }
            });
        }

        /// <summary>
        /// Se Registran las Formas de Pago
        /// </summary>
        /// <param name="movimientoCaja">The movimiento caja.</param>
        /// <param name="idRegistroCaja">The id registro caja.</param>
        /// <param name="contexto">The contexto.</param>
        public void InsertarFormasDePago(CARegistroTransacCajaDC movimientoCaja, long idRegistroCaja, SqlConnection conexion, SqlTransaction transaccion, bool anulacionGuia = false)
        {
            //se recorren las formas de pago y se insertan las que tienen valor>0
            movimientoCaja.RegistroVentaFormaPago.ToList().ForEach(AddFormaPag =>
            {
                if (AddFormaPag.Valor > 0)
                {
                    if (!anulacionGuia)
                    {
                        //Actualiza el Valor de pin Prepago
                        if (AddFormaPag.IdFormaPago == CAConstantesCaja.FORMA_PAGO_PREPAGO)
                        {
                            if (!String.IsNullOrWhiteSpace(AddFormaPag.NumeroAsociado))
                            {
                                ActualizarSaldoPinPrepago(Convert.ToInt64(AddFormaPag.NumeroAsociado), AddFormaPag.Valor, conexion, transaccion);
                            }
                        }

                    }
                    //Diego informa: que incluso siendo una anulacion de guia, se debe insertar en la tabla de formas de pago
                    //Se ejecuta el Procedimiento Almacenado de las formas de Pago

                    SqlCommand cmd = new SqlCommand("paCrearRegTransFormasPago_CAJ", conexion, transaccion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(Utilidades.AddParametro("@idRegistroVenta", idRegistroCaja));
                    cmd.Parameters.Add(Utilidades.AddParametro("@idFormaPago", AddFormaPag.IdFormaPago));
                    cmd.Parameters.Add(Utilidades.AddParametro("@descripcionFormaPago", AddFormaPag.Descripcion));
                    cmd.Parameters.Add(Utilidades.AddParametro("@valor", AddFormaPag.Valor));
                    cmd.Parameters.Add(Utilidades.AddParametro("@numeroAsociado", AddFormaPag.NumeroAsociado));
                    cmd.Parameters.Add(Utilidades.AddParametro("@campo1", AddFormaPag.Campo01));
                    cmd.Parameters.Add(Utilidades.AddParametro("@campo2", AddFormaPag.Campo02));
                    cmd.Parameters.Add(Utilidades.AddParametro("@creadoPor", movimientoCaja.Usuario));

                    cmd.ExecuteNonQuery();
                }
            });
        }


        /// <summary>
        /// Marcar y reportar cierres que fueron consolidados en un envío de dinero a agencia
        /// </summary>
        /// <param name="idCierrePuntoAsociado">Identificación del cierre de centro de servicios asociado al cierre de caja</param>
        /// <param name="fechaFiltro">Fecha hasta la cual se consolidaron los cierres de caja</param>
        public void MarcarCierresComoReportados(long idCierrePunto, long idCentroServicio)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                IList<CierreCentroServicios_CAJ> cierres = contexto.CierreCentroServicios_CAJ
                  .Where(ci => ci.CCS_IdCierreCentroServicios <= idCierrePunto &&
                    ci.CCS_EstaReportado == false && ci.CCS_IdCentroServicios == idCentroServicio)
                  .ToList();

                foreach (var cierre in cierres)
                {
                    cierre.CCS_EstaReportado = true;
                }

                if (cierres.Count > 0)
                {
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Inserta el dinero envio empresa.
        /// </summary>
        /// <param name="envioDineroAgencia">El envio dinero agencia.</param>
        public void InsertarDineroEnvioEmpresa(CARecoleccionDineroPuntoDC envioDineroAgencia)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                RecoleccionDineroPuntos_CAJ addNvEnvioDinero = new RecoleccionDineroPuntos_CAJ()
                {
                    RDP_IdPuntoServicio = envioDineroAgencia.IdPuntoServicio,
                    RDP_NombrePuntoServicio = envioDineroAgencia.NombrePunto,
                    RDP_NoBolsaSeguridad = envioDineroAgencia.BolsaSeguridad,
                    RDP_IdMensajero = Convert.ToInt64(envioDineroAgencia.MensajeroPunto.IdPersonaInterna),
                    RDP_NombreMensajero = envioDineroAgencia.MensajeroPunto.NombreApellido,
                    RDP_ValorTotalEnviado = envioDineroAgencia.ValorTotalEnviado,
                    RDP_CreadoPor = envioDineroAgencia.UsuarioCierre,
                    RDP_IdCierreCentroServicios = envioDineroAgencia.IdCierreCentroServicios,
                    RDP_FechaGrabacion = DateTime.Now,
                };
                contexto.RecoleccionDineroPuntos_CAJ.Add(addNvEnvioDinero);
                try
                {
                    contexto.SaveChanges();
                }

                catch (System.Data.Entity.Infrastructure.DbUpdateException dbUPEx)
                {
                    if ((dbUPEx.InnerException.InnerException as SqlException).Number == 2601)
                    {
                        ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                              .ERROR_BOLSA_SEGURIDAD_YA_REGISTRADA.ToString(), CACajaServerMensajes
                                                                              .CargarMensaje(CAEnumTipoErrorCaja.ERROR_BOLSA_SEGURIDAD_YA_REGISTRADA));
                        throw new FaultException<ControllerException>(excepcion);
                    }
                };
            }
        }

        /// <summary>
        /// Adiciona el dinero recibido por la Empresa
        /// de una agencio o punto.
        /// </summary>
        /// <param name="dineroRecibido">The dinero recibido.</param>
        public void AdicionarDineroAgencia(CARecoleccionDineroPuntoDC dineroRecibido)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                if (dineroRecibido.RegistroManual)
                {
                    RecoleccionDineroPuntos_CAJ AddNvEnvioDinero = new RecoleccionDineroPuntos_CAJ()
                    {
                        RDP_IdPuntoServicio = dineroRecibido.PuntoCentroServicio.IdCentroServicio,
                        RDP_NombrePuntoServicio = dineroRecibido.PuntoCentroServicio.Nombre,
                        RDP_NoBolsaSeguridad = dineroRecibido.BolsaSeguridad,
                        RDP_IdMensajero = dineroRecibido.MensajeroPunto.IdMensajero,
                        RDP_NombreMensajero = dineroRecibido.MensajeroPunto.NombreApellido,
                        RDP_ValorTotalEnviado = dineroRecibido.ValorReal,
                        RDP_CreadoPor = dineroRecibido.UsuarioCierre,
                        RDP_FechaGrabacion = DateTime.Now,
                        RDP_IdCierreCentroServicios = dineroRecibido.IdCierreCentroServicios
                    };
                    contexto.RecoleccionDineroPuntos_CAJ.Add(AddNvEnvioDinero);
                    contexto.SaveChanges();

                    ReporteDineroPuntoAgencia_CAJ AddRepoteDineroPunto = new ReporteDineroPuntoAgencia_CAJ()
                    {
                        RDP_IdRecoleccion = AddNvEnvioDinero.RDP_IdRecoleccion,
                        RDP_ValorReportado = dineroRecibido.ValorReal,
                        RDP_TipoObservacion = dineroRecibido.TipoObservacionPunto != null ? dineroRecibido.TipoObservacionPunto.idTipoObservacion : (short)1,
                        RDP_Observaciones = dineroRecibido.Observacion != null ? dineroRecibido.Observacion : string.Empty,
                        RDP_Manual = dineroRecibido.RegistroManual,
                        RDP_CreadoPor = dineroRecibido.UsuarioCierre,
                        RDP_FechaGrabacion = DateTime.Now
                    };
                    contexto.ReporteDineroPuntoAgencia_CAJ.Add(AddRepoteDineroPunto);
                    contexto.SaveChanges();
                }
                else
                {
                    ReporteDineroPuntoAgencia_CAJ AddRepoteDineroPunto = new ReporteDineroPuntoAgencia_CAJ()
                    {
                        RDP_IdRecoleccion = dineroRecibido.IdRecoleccion,
                        RDP_ValorReportado = dineroRecibido.ValorTotalEnviado,
                        RDP_TipoObservacion = dineroRecibido.TipoObservacionPunto.idTipoObservacion,
                        RDP_Observaciones = dineroRecibido.Observacion,
                        RDP_Manual = dineroRecibido.RegistroManual,
                        RDP_CreadoPor = dineroRecibido.UsuarioCierre,
                        RDP_FechaGrabacion = DateTime.Now,
                    };
                    contexto.ReporteDineroPuntoAgencia_CAJ.Add(AddRepoteDineroPunto);
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Adiciona las Transacciones de un Mensajero.
        /// </summary>
        /// <param name="registroMensajero">Clase Cuenta Mensajero.</param>
        public long AdicionarTransaccMensajero(CACuentaMensajeroDC registroMensajero)
        {

            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paValidaInsertarCuentaMensajero_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CUM_IdMensajero", registroMensajero.Mensajero.IdPersonaInterna);
                cmd.Parameters.AddWithValue("@CUM_NombreMensajero", registroMensajero.Mensajero.NombreApellido);
                cmd.Parameters.AddWithValue("@CUM_IdConcepto", registroMensajero.ConceptoCajaMensajero.IdConceptoCaja);
                cmd.Parameters.AddWithValue("@CUM_NumeroDocumento", registroMensajero.NumeroDocumento);
                cmd.Parameters.AddWithValue("@CUM_Valor", registroMensajero.Valor);
                cmd.Parameters.AddWithValue("@CUM_CreadoPor", registroMensajero.UsuarioRegistro);
                cmd.Parameters.AddWithValue("@CUM_ConceptoEsIngreso", registroMensajero.ConceptoEsIngreso);
                cmd.Parameters.AddWithValue("@CUM_Observaciones", string.IsNullOrEmpty(registroMensajero.Observaciones) ? "" : registroMensajero.Observaciones);
                cmd.Parameters.AddWithValue("@CUM_NoPlanillaAlCobros", registroMensajero.NoPlanillaAlCobros);
                cmd.Parameters.AddWithValue("@CUM_NoPlanillaVentas", registroMensajero.NoPlanillaVentas);
                conn.Open();
                return Convert.ToInt64(cmd.ExecuteScalar());
            }

            /*
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CuentaMensajero_CAJ SaldoAnterior = contexto.CuentaMensajero_CAJ.OrderByDescending(orden => orden.CUM_IdTransaccion)
                                                    .FirstOrDefault(registro => registro.CUM_IdMensajero == registroMensajero.Mensajero.IdPersonaInterna);
                CuentaMensajero_CAJ AddRegistro;

                AddRegistro = new CuentaMensajero_CAJ()
                {
                    CUM_IdMensajero = registroMensajero.Mensajero.IdPersonaInterna,
                    CUM_NombreMensajero = registroMensajero.Mensajero.NombreApellido,
                    CUM_IdConcepto = registroMensajero.ConceptoCajaMensajero.IdConceptoCaja,
                    CUM_NumeroDocumento = registroMensajero.NumeroDocumento,
                    CUM_Valor = registroMensajero.Valor,
                    CUM_ConceptoEsIngreso = registroMensajero.ConceptoEsIngreso,
                    CUM_FechaGrabacion = DateTime.Now,
                    CUM_CreadoPor = registroMensajero.UsuarioRegistro,
                    CUM_Observaciones = registroMensajero.Observaciones,
                    CUM_NoPlanillaVentas = registroMensajero.NoPlanillaVentas,
                    CUM_NoPlanillaAlCobros = registroMensajero.NoPlanillaAlCobros
                };
                if (SaldoAnterior == null)
                    AddRegistro.CUM_SaldoAcumulado = registroMensajero.Valor * (registroMensajero.ConceptoEsIngreso ? 1 : -1);
                else
                {
                    decimal ValorSaldoAnterior = SaldoAnterior.CUM_SaldoAcumulado;
                    AddRegistro.CUM_SaldoAcumulado = ValorSaldoAnterior + registroMensajero.Valor * (registroMensajero.ConceptoEsIngreso ? 1 : -1);
                }
                contexto.CuentaMensajero_CAJ.Add(AddRegistro);
                contexto.SaveChanges();
                return AddRegistro.CUM_IdTransaccion;
            }*/
        }

        /// <summary>
        /// Inserta in registro del cliente credito y cambia el estado de la facturacion y el tipo de dato
        /// Este metodo se utiliza para el cambio de forma de pago de AlCobro -Credito
        /// </summary>
        public void AdicionarRegistroTransClienteCredito(ADNovedadGuiaDC novedadGuia)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                RegistroTransacDetalleCaja_CAJ transDetalle = contexto.RegistroTransacDetalleCaja_CAJ.FirstOrDefault(reg => reg.RTD_Numero == novedadGuia.Guia.NumeroGuia);
                if (transDetalle == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_TRANSACCION_DETALLE_CAJA.ToString(), CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_TRANSACCION_DETALLE_CAJA)));
                }

                transDetalle.RTD_EstadoFacturacion = CAConstantesCaja.PENDIENTE_POR_FACTURAR;

                RegistroTransaccionesCaja_CAJ transCaja = contexto.RegistroTransaccionesCaja_CAJ.FirstOrDefault(caja => caja.RTC_IdRegistroTranscaccion == transDetalle.RTD_IdRegistroTranscaccion);
                if (transCaja == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_TRANSACCION_CAJA.ToString(), CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_TRANSACCION_CAJA)));
                }

                transCaja.RTC_TipoDatosAdicionales = CAConstantesCaja.CLIENTE_CREDITO;

                RegistroTrasnClienteCredit_CAJ registroTrans = contexto.RegistroTrasnClienteCredit_CAJ.FirstOrDefault(rtcc => rtcc.RTC_IdRegistroVenta == transDetalle.RTD_IdRegistroTranscaccion);
                if (registroTrans != null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_TRANSACCION_CLIENTE_CREDITO.ToString(), CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_TRANSACCION_CLIENTE_CREDITO)));
                }

                //RegistroTrasnClienteCredit_CAJ transClienteCredito = new RegistroTrasnClienteCredit_CAJ()
                //{
                //  RTC_IdRegistroVenta = transDetalle.RTD_IdRegistroTranscaccion,
                //  RTC_IdCliente = novedadGuia.ClienteCredito.IdCliente,
                //  RTC_IdContrato = novedadGuia.ClienteCredito.IdContrato,
                //  RTC_IdSucursal = novedadGuia.ClienteCredito.IdSucursal,
                //  RTC_Nit = novedadGuia.ClienteCredito.Nit,
                //  RTC_NombreSucursal = novedadGuia.ClienteCredito.NombreSucursal,
                //  RTC_NumeroContrato = novedadGuia.ClienteCredito.NumeroContrato,
                //  RTC_NombreCliente = novedadGuia.ClienteCredito.NombreCliente,
                //  RTC_FechaGrabacion = DateTime.Now,
                //  RTC_CreadoPor = ControllerContext.Current.Usuario
                //};

                //contexto.RegistroTrasnClienteCredit_CAJ.Add(transClienteCredito);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adicionar Reporte del Mensajero.
        /// </summary>
        /// <param name="reportMensajero">Clase report mensajero.</param>
        public void AdicionarReporteMensajero(CAReporteMensajeroCajaDC reportMensajero)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ReporteMensajeroCaja_CAJ AddReporte = new ReporteMensajeroCaja_CAJ()
                {
                    RMC_IdMensajero = reportMensajero.Mensajero.IdPersonaInterna,
                    RMC_NombreMensajero = reportMensajero.Mensajero.NombreApellido,
                    RMC_FechaGrabacion = DateTime.Now,
                    RMC_CreadoPor = ControllerContext.Current.Usuario,
                    RMC_NoComprobante = reportMensajero.NumeroComprobanteTransDetCaja
                };

                if (reportMensajero.IdRegistroTransDetalleCaja == 0)
                    AddReporte.RMC_IdRegistroTransDetalleCaja = null;
                else
                    AddReporte.RMC_IdRegistroTransDetalleCaja = reportMensajero.IdRegistroTransDetalleCaja;

                contexto.ReporteMensajeroCaja_CAJ.Add(AddReporte);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona la venta de un Pin Prepago.
        /// </summary>
        /// <param name="ventaPinPrepago">The venta pin prepago.</param>
        public void AdicionarPinPrepago(CAPinPrepagoDC ventaPinPrepago)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                PinPrepago_CAJ AddPinPrepago = new PinPrepago_CAJ()
                {
                    PIP_Pin = ventaPinPrepago.Pin,
                    PIP_Valor = ventaPinPrepago.ValorPin,
                    PIP_Saldo = ventaPinPrepago.ValorPin,
                    PIP_FechaActualizacion = DateTime.Now,
                    PIP_NombreComprador = ventaPinPrepago.NombreComprador,
                    PIP_IdTipoIdentificacion = ventaPinPrepago.TipoId.IdTipoIdentificacion,
                    PIP_Identificacion = ventaPinPrepago.Identificacion,
                    PIP_Direccion = ventaPinPrepago.Direccion,
                    PIP_IdLocalidad = ventaPinPrepago.IdLocalidad,
                    PIP_IdCentroServiciosVende = ventaPinPrepago.IdCentroServicioVende,
                    PIP_FechaGrabacion = DateTime.Now,
                    PIP_CreadoPor = ventaPinPrepago.Usuario
                };
                contexto.PinPrepago_CAJ.Add(AddPinPrepago);
                contexto.SaveChanges();
            }
        }

        #endregion Operaciones Puntos

        #region Operacion Racol/Casa Matriz

        /// <summary>
        /// Adiciona los movimientos genrados entre Racol y Agencia.
        /// </summary>
        /// <param name="movimiento">The movimiento.</param>
        public void AdicionarMovRacolAgencia(CAMovCentroSvcCentroSvcDC movimiento)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CentroSvcCentroSvcMov_CAJ AddMovimiento = new CentroSvcCentroSvcMov_CAJ()
                {
                    CSM_IdCentroServiciosOrigen = movimiento.IdCentroServicioOrigen,
                    CSM_NombreCentroSvcOrigen = movimiento.NombreCentroServicioOrigen,
                    CSM_IdCentroServiciosDestino = movimiento.IdCentroServicioDestino,
                    CSM_NombreCentroSvcDestino = movimiento.NombreCentroServicioDestino,
                    CSM_IdRegistroTxOrigen = movimiento.IdRegistroTxOrigen,
                    CSM_IdRegistroTxDestino = movimiento.IdRegistroTxDestino,
                    CSM_FechaGrabacion = DateTime.Now,
                    CSM_CreadoPor = movimiento.UsuarioRegistra,
                    CSM_NumeroBolsaSeguridad = movimiento.BolsaSeguridad ?? "0",
                    CSM_NumeroGuia = movimiento.NumeroGuia,
                    CSM_NumeroPrecinto = movimiento.NumeroPrecinto ?? "0"
                };
                contexto.CentroSvcCentroSvcMov_CAJ.Add(AddMovimiento);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona  el Movimiento entre un centro Servicio y la caja del Banco.
        /// </summary>
        /// <param name="movimiento">The movimiento.</param>
        public void AdicionarMovCentroSrvCajaBanco(CAMovBancoCentroSvcDC movimiento)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                BancoCentroServiciosMov_CAJ AddMovimiento = new BancoCentroServiciosMov_CAJ()
                {
                    BCM_IdCajaBanco = movimiento.IdCajaBanco,
                    BCM_IdCentroServicios = movimiento.IdCentroServicio,
                    BCM_IdRegistroTranscaccion = movimiento.IdRegistroTransaccion,
                    BCM_FechaGrabacion = DateTime.Now,
                    BCM_CreadoPor = movimiento.CreadoPor
                };

                contexto.BancoCentroServiciosMov_CAJ.Add(AddMovimiento);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona El movimiento Entre la caja de Casa Matriz y la caja del Banco.
        /// </summary>
        /// <param name="movimiento">Información del movimiento entre las cajas.</param>
        public void AdicionarMovCasaMatrizBanco(CAMovEmpresaBancoDC movimiento)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CajaCasaMatrizBancoMov_CAJ AddMovimiento = new CajaCasaMatrizBancoMov_CAJ()
                {
                    CBM_CreadoPor = movimiento.CreadoPor,
                    CBM_FechaGrabacion = DateTime.Now,
                    CBM_IdOperacionCajaBanco = movimiento.IdCajaBanco,
                    CBM_IdOperacionCajaCasaMatriz = movimiento.IdCajaEmpresa,
                };
                contexto.CajaCasaMatrizBancoMov_CAJ
                  .Add(AddMovimiento);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adicionar movimientos entre la casa matriz y el centro de servicios.
        /// </summary>
        /// <param name="movimiento">The movimiento.</param>
        public void AdicionarMovCasaMatrizOpn(CajaCasaMatrizOperacionNacionalMov_CAJ movimiento)
        {
            if (movimiento.CMO_NumeroPrecinto == null)
            {
                movimiento.CMO_NumeroPrecinto = "0";
            }

            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.CajaCasaMatrizOperacionNacionalMov_CAJ
                  .Add(movimiento);

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adicionar movimiento entre la caja de operación nacional y banco.
        /// </summary>
        /// <param name="movimiento"></param>
        public void AdicionarMovOpnBanco(CajaBancoOperacionNacionalMov_CAJ movimiento)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.CajaBancoOperacionNacionalMov_CAJ
                  .Add(movimiento);

                contexto.SaveChanges();
            }
        }

        #endregion Operacion Racol/Casa Matriz

        #region operacion devolución alcobros

        /// <summary>
        /// metodo que inserta un movimiento para que se descuente un movimiento al cobro por guia
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="observacion"></param>
        /// <param name="descripcion"></param>
        /// <param name="idPersonaInterna"></param>
        /// <param name="idCentroServicios"></param>
        public Boolean InsertarDescuentoAlCobroDevuelto(long numeroGuia, long idCentroServicios, long idAperturaCaja)
        {
            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paInsertarTransaccionesCajaDevolucionAlcobro_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdAperturaCaja", idAperturaCaja);
                cmd.Parameters.AddWithValue("@CreadoPor", ControllerContext.Current.Usuario);
                cmd.Parameters.AddWithValue("@NumeroGuia", numeroGuia);
                conn.Open();
                object result = cmd.ExecuteScalar();
                conn.Close();
                if (result == DBNull.Value || result == null || Convert.ToInt32(result) == 0)
                    return false;
                else
                    return true;
            }

        }

        #endregion

        #endregion Insertar

        #region Actualizar

        /// <summary>
        /// Modifica una transacción de caja agregando o modificando el número de comprobante asociado
        /// </summary>
        /// <param name="IdRegistroTransaccionDetalle"></param>
        /// <param name="numeroComprobante"></param>
        public void ActualizarNumeroComprobanteTransaccionCaja(long IdRegistroTransaccionDetalle, string numeroComprobante)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                contexto.paActualizarNumCompRegTransDet(IdRegistroTransaccionDetalle, numeroComprobante);
            }
        }

        /// <summary>
        /// Actualizar el Saldo del Pin Prepago.
        /// </summary>
        /// <param name="idPinPrepago">Es el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">Es el valor de la compra del pin prepago.</param>
        public void ActualizarSaldoPinPrepago(long idPinPrepago, decimal valorCompraPinPrepago)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                PinPrepago_CAJ actualizaPinPrepago = contexto.PinPrepago_CAJ.Where(pin => pin.PIP_Pin == idPinPrepago).FirstOrDefault();

                if (actualizaPinPrepago != null)
                {
                    actualizaPinPrepago.PIP_Saldo = actualizaPinPrepago.PIP_Saldo - valorCompraPinPrepago;
                    actualizaPinPrepago.PIP_FechaActualizacion = DateTime.Now;
                    contexto.SaveChanges();
                }
                else
                {
                    ControllerException excepcion = new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja
                                                                            .ERROR_PINPREPAGO_NO_ENCONTRADO.ToString(), CACajaServerMensajes
                                                                            .CargarMensaje(CAEnumTipoErrorCaja.ERROR_PINPREPAGO_NO_ENCONTRADO));
                    throw new FaultException<ControllerException>(excepcion);
                }
            }
        }

        /// <summary>
        /// Actualizar el Saldo del Pin Prepago.
        /// </summary>
        /// <param name="idPinPrepago">Es el id del pin prepago.</param>
        /// <param name="valorCompraPinPrepago">Es el valor de la compra del pin prepago.</param>
        public void ActualizarSaldoPinPrepago(long idPinPrepago, decimal valorCompraPinPrepago, SqlConnection conexion, SqlTransaction transaccion)
        {

            string cmdText = @"UPDATE PinPrepago_CAJ WITH(ROWLOCK)
                               SET PIP_Saldo = PIP_Saldo - @valorCompraPin,
                                   PIP_FechaActualizacion = GETDATE()
                               WHERE PIP_Pin = @idPinPrepago";
            SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@idPinPrepago", idPinPrepago);
            cmd.Parameters.AddWithValue("@valorCompraPin", valorCompraPinPrepago);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Actualiza la Observacion de trans.
        /// </summary>
        /// <param name="CuentaMensajero">The cuenta mensajero.</param>
        public void ActualizarObservacionEstadoCta(CACuentaMensajeroDC cuentaMensajero)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CuentaMensajero_CAJ cuenta = contexto.CuentaMensajero_CAJ.Where(idReg => idReg.CUM_IdTransaccion == cuentaMensajero.IdTransaccion).FirstOrDefault();

                if (cuenta != null)
                {
                    cuenta.CUM_Observaciones = cuentaMensajero.Observaciones;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Actualiza el cierre de caja como Caja Reportada al cajero Ppal
        /// </summary>
        /// <param name="idCierreCajaAux">The id cierre caja aux.</param>
        public void ReportarCajaACajeroPrincipal(long idCierreCajaAux)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CierreCaja_CAJ cajaReportada = contexto.CierreCaja_CAJ.FirstOrDefault(idcaja => idcaja.CIC_IdCierreCaja == idCierreCajaAux);

                if (cajaReportada != null)
                {
                    cajaReportada.CIC_EstaReportado = true;
                    contexto.SaveChanges();
                }
            }
        }

        public List<CAConceptoCajaDC> ObtenerConceptos()
        {


            using (SqlConnection conn = new SqlConnection(conexionStringController))
            {
                SqlCommand cmd = new SqlCommand("paObtenerConceptosCaja_CAJ", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                da.Fill(dt);
                conn.Close();

                return dt.AsEnumerable().ToList().ConvertAll<CAConceptoCajaDC>(c =>
                 {
                     return new CAConceptoCajaDC()
                     {
                         IdConceptoCaja = c.Field<int>("COC_IdConceptoCaja"),
                         Descripcion = c.Field<string>("COC_Descripcion"),
                         Nombre = c.Field<string>("COC_Nombre"),
                         EsServicio = c.Field<bool>("COC_EsServicio"),
                         ContraPartidaCasaMatriz = c.Field<bool>("COC_ContrapartidaCasaMatriz"),
                         ContraPartidaCS = c.Field<bool>("COC_ContrapartidaCS")
                     };
                 });

            }

            /*
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConceptoCaja_CAJ.ToList().ConvertAll<CAConceptoCajaDC>(c =>
                {
                    return new CAConceptoCajaDC()
                    {
                        IdConceptoCaja = c.COC_IdConceptoCaja,
                        Descripcion = c.COC_Descripcion,
                        Nombre = c.COC_Nombre,
                        EsServicio = c.COC_EsServicio,
                        ContraPartidaCasaMatriz = c.COC_ContrapartidaCasaMatriz,
                        ContraPartidaCS = c.COC_ContrapartidaCS
                    };
                });
            }*/
        }

        #endregion Actualizar

        #region Operaciones sobre número de operación

        /// <summary>
        /// Obtener el registro de transacción para una númeor de operación
        /// </summary>
        /// <param name="numeroOperacion">Número de operación: Número de guía, Número de giro, etc</param>
        /// <returns>Información del registro detallado de la transacción</returns>
        public CARegistroTransacCajaDetalleDC ObtenerRegistroVentaPorNumeroOperacion(long numeroOperacion)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                RegistroTransacDetalleCaja_CAJ registro = contexto.RegistroTransacDetalleCaja_CAJ
                  .Where(w => w.RTD_Numero == numeroOperacion)
                  .FirstOrDefault();

                if (registro == null)
                {
                    throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA,
                      CAEnumTipoErrorCaja.ERROR_CAJA_NUMERO_NO_ENCONTRADO.ToString(),
                      CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_CAJA_NUMERO_NO_ENCONTRADO)));
                }

                return MapearRegistroDetalleCaja(registro);
            }
        }

        /// <summary>
        /// Mapear una entidad de registro detalle caja a un contrato de datos
        /// </summary>
        /// <param name="entidad">Entidad con la información de la tabla en la base de datos</param>
        /// <returns>Contrato de datos</returns>
        private CARegistroTransacCajaDetalleDC MapearRegistroDetalleCaja(RegistroTransacDetalleCaja_CAJ entidad)
        {
            CARegistroTransacCajaDetalleDC infoTransaccion = new CARegistroTransacCajaDetalleDC
            {
                Cantidad = entidad.RTD_Cantidad,
                ConceptoCaja = new CAConceptoCajaDC
                {
                    IdConceptoCaja = entidad.RTD_IdConceptoCaja,
                },
                ConceptoEsIngreso = entidad.RTD_ConceptoEsIngreso,
                Descripcion = entidad.RTD_Descripcion,
                EstadoFacturacion = (CAEnumEstadoFacturacion)Enum.Parse(typeof(CAEnumEstadoFacturacion), entidad.RTD_EstadoFacturacion),
                FechaFacturacion = entidad.RTD_FechaFacturacion,
                IdRegistroTranscaccion = entidad.RTD_IdRegistroTranscaccion,
                Numero = entidad.RTD_Numero,
                NumeroComprobante = entidad.RTD_NumeroComprobante,
                NumeroFactura = entidad.RTD_NumeroFactura,
                Observacion = entidad.RTD_Observacion,
                ValorDeclarado = entidad.RTD_ValorDeclarado,
                ValoresAdicionales = entidad.RTD_ValoresAdicionales,
                ValorImpuestos = entidad.RTD_ValorImpuestos,
                ValorPrimaSeguros = entidad.RTD_ValorPrimaSeguros,
                ValorRetenciones = entidad.RTD_ValorRetenciones,
                ValorServicio = entidad.RTD_ValorServicio,
                ValorTercero = entidad.RTD_ValorTercero
            };

            return infoTransaccion;
        }

        #endregion Operaciones sobre número de operación
    }
}