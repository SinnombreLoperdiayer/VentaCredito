using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Cierres;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Impresion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.ServiceModel;
using System.Data.SqlClient;
using CO.Servidor.Dominio.Comun.Util;
using System.Configuration;

namespace CO.Servidor.Cajas.Datos
{
    /// <summary>
    /// Repositorio para operacaiones sobre caja de casa matriz
    /// </summary>
    public class CARepositorioGestionCajas
    {
        private const string NombreModelo = "ModeloCajas";
        private static CARepositorioGestionCajas instancia = new CARepositorioGestionCajas();

        private string conexionStringController = ConfigurationManager.ConnectionStrings["ControllerTransaccional"].ConnectionString;

        /// <summary>
        /// Retorna instancia del repositorio de gestión cajas
        /// </summary>
        public static CARepositorioGestionCajas Instancia
        {
            get { return CARepositorioGestionCajas.instancia; }
        }

        /// <summary>
        /// Adiciona registro de movimiento entre centros de servicios
        /// </summary>
        /// <param name="movimiento">Información del movimiento</param>
        public void AdiconarMovCentroServiciosACentroServicios(CAMovCentroSvcCentroSvcDC movimiento)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CentroSvcCentroSvcMov_CAJ nuevoMov = contexto.CentroSvcCentroSvcMov_CAJ
                  .Add(new CentroSvcCentroSvcMov_CAJ
                  {
                      CSM_CreadoPor = movimiento.UsuarioRegistra,
                      CSM_FechaGrabacion = DateTime.Now,
                      CSM_IdCentroServiciosDestino = movimiento.IdCentroServicioDestino,
                      CSM_IdCentroServiciosOrigen = movimiento.IdCentroServicioOrigen,
                      CSM_IdRegistroTxDestino = movimiento.IdRegistroTxDestino,
                      CSM_IdRegistroTxOrigen = movimiento.IdRegistroTxOrigen,
                      CSM_NombreCentroSvcDestino = movimiento.NombreCentroServicioDestino,
                      CSM_NombreCentroSvcOrigen = movimiento.NombreCentroServicioOrigen,
                      CSM_NumeroBolsaSeguridad = movimiento.BolsaSeguridad == null ? string.Empty : movimiento.BolsaSeguridad,
                      CSM_NumeroPrecinto = movimiento.NumeroPrecinto == null ? string.Empty : movimiento.NumeroPrecinto
                  });

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona  el Movimiento entre un centro Servicio y la
        /// caja del Banco.
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

        public void AdicionarMovCentroSrvCajaCasaMatriz(CajaCasaMatrizCentroSvcMov_CAJ operacionCSCasaMatriz)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                operacionCSCasaMatriz.CEM_FechaGrabacion=DateTime.Now;
                operacionCSCasaMatriz.CEM_CreadoPor = ControllerContext.Current.Usuario;
                contexto.CajaCasaMatrizCentroSvcMov_CAJ.Add(operacionCSCasaMatriz);
                contexto.SaveChanges();
            }
        }

        public void AdicionarMovCentroSrvCajaCasaMatriz(CajaCasaMatrizCentroSvcMov_CAJ operacionCSCasaMatriz,SqlConnection conexion, SqlTransaction transaccion)
        {
            string cmdText = @"
                                INSERT INTO [CajaCasaMatrizCentroSvcMov_CAJ]
                                           ([CEM_IdOperacionCajaCasaMatriz]
                                           ,[CEM_IdCentroServiciosMov]
                                           ,[CEM_NombreCentroServiciosMov]
                                           ,[CEM_IdRegistroTranscaccion]
                                           ,[CEM_FechaGrabacion]
                                           ,[CEM_CreadoPor])
                                     VALUES
                                           (@CEM_IdOperacionCajaCasaMatriz
                                           ,@CEM_IdCentroServiciosMov
                                           ,@CEM_NombreCentroServiciosMov
                                           ,@CEM_IdRegistroTranscaccion
                                           ,GETDATE()
                                           ,@CEM_CreadoPor) ";

            SqlCommand cmd = new SqlCommand(cmdText, conexion, transaccion);
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.Add(Utilidades.AddParametro("@CEM_IdOperacionCajaCasaMatriz",operacionCSCasaMatriz.CEM_IdOperacionCajaCasaMatriz));
            cmd.Parameters.Add(Utilidades.AddParametro("@CEM_IdCentroServiciosMov",operacionCSCasaMatriz.CEM_IdCentroServiciosMov));
            cmd.Parameters.Add(Utilidades.AddParametro("@CEM_NombreCentroServiciosMov",operacionCSCasaMatriz.CEM_NombreCentroServiciosMov));
            cmd.Parameters.Add(Utilidades.AddParametro("@CEM_IdRegistroTranscaccion", operacionCSCasaMatriz.CEM_IdRegistroTranscaccion));
            cmd.Parameters.Add(Utilidades.AddParametro("@CEM_CreadoPor",ControllerContext.Current.Usuario));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Adiciona una Operación a la caja del Banco.
        /// </summary>
        public long AdicionarOperacionCajaBanco(OperacionCajaBanco_CAJ operacion)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                OperacionCajaBanco_CAJ opBanco = contexto.OperacionCajaBanco_CAJ
                  .Add(operacion);

                contexto.SaveChanges();

                return opBanco.CAB_IdOperacionCajaBanco;
            }
        }        

        /// <summary>
        /// Adicionar una operación de caja de Operación Nacional (Opn)
        /// </summary>
        /// <param name="operacion">Información de la operación</param>
        /// <returns>Identificador de la operación</returns>
        public long AdicionarOperacionCajaOpn(OperacionCajaOperacionNacional_CAJ operacion)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                operacion.CON_NumeroDocumento = operacion.CON_NumeroDocumento ?? String.Empty;

                OperacionCajaOperacionNacional_CAJ opOpn = contexto.OperacionCajaOperacionNacional_CAJ
                  .Add(operacion);

                contexto.SaveChanges();

                return opOpn.CON_IdOperacionCajaOperacionNacional;
            }
        }

        public void AdicionarMovCentroSvcOpn(CajaCentroSvcOperacionNacionalMov_CAJ mov)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CajaCentroSvcOperacionNacionalMov_CAJ movOpn = contexto.CajaCentroSvcOperacionNacionalMov_CAJ
                  .Add(mov);

                contexto.SaveChanges();
            }
        }

        public long AdicionarOperacionCajaCasaMatriz(OperacionCajaCasaMatriz_CAJ operacion)
        {
            SqlConnection conexion = new SqlConnection(conexionStringController );
            SqlTransaction transaccion = null;
            conexion.Open();
            transaccion = conexion.BeginTransaction();
            long id = AdicionarOperacionCajaCasaMatriz(operacion, conexion, transaccion);
            transaccion.Commit();
            conexion.Close();
            return id;
        }

        public long AdicionarOperacionCajaCasaMatriz(OperacionCajaCasaMatriz_CAJ operacion,SqlConnection conexion, SqlTransaction transaccion)
        {

            if (!string.IsNullOrEmpty(operacion.CAE_Observacion) && operacion.CAE_Observacion.Length > 200)
            {
                operacion.CAE_Observacion =  operacion.CAE_Observacion.Substring(0, 200);
            }

            string text = @"INSERT INTO [OperacionCajaCasaMatriz_CAJ]
                           ([CAE_IdAperturaCaja]
                           ,[CAE_IdConceptoCaja]
                           ,[CAE_Valor]
                           ,[CAE_ConceptoEsIngreso]
                           ,[CAE_NumeroDocumento]
                           ,[CAE_Observacion]
                           ,[CAE_FechaMovimiento]
                           ,[CAE_MovHechoPor]
                           ,[CAE_Descripcion]
                           ,[CAE_FechaGrabacion]
                           ,[CAE_CreadoPor]
                           ,[CAE_NombreConceptoCaja])
                     VALUES
                           (@CAE_IdAperturaCaja
                           ,@CAE_IdConceptoCaja
                           ,@CAE_Valor
                           ,@CAE_ConceptoEsIngreso
                           ,@CAE_NumeroDocumento
                           ,@CAE_Observacion
                           ,@CAE_FechaMovimiento
                           ,@CAE_MovHechoPor
                           ,@CAE_Descripcion
                           ,GETDATE()
                           ,@CAE_CreadoPor
                           ,@CAE_NombreConceptoCaja)
                    SELECT SCOPE_IDENTITY() 
                    ";

            SqlCommand cmd = new SqlCommand(text, conexion, transaccion);
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_IdAperturaCaja",operacion.CAE_IdAperturaCaja));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_IdConceptoCaja",operacion.CAE_IdConceptoCaja));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_Valor",operacion.CAE_Valor));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_ConceptoEsIngreso",operacion.CAE_ConceptoEsIngreso));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_NumeroDocumento",operacion.CAE_NumeroDocumento));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_Observacion",operacion.CAE_Observacion));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_FechaMovimiento",operacion.CAE_FechaMovimiento));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_MovHechoPor",operacion.CAE_MovHechoPor));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_Descripcion",operacion.CAE_Descripcion));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_CreadoPor",operacion.CAE_CreadoPor));
            cmd.Parameters.Add(Utilidades.AddParametro("@CAE_NombreConceptoCaja", operacion.CAE_NombreConceptoCaja));

            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        /// <summary>
        /// Retorna las cuentas externas del sistema
        /// </summary>
        /// <returns></returns>
        public List<CO.Servidor.Servicios.ContratoDatos.Cajas.CACuentaExterna> ObtenerCuentasExternas()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CuentaExterna_PAR
                      .OrderBy(cuenta => cuenta.CEX_Descripcion)
                      .ToList()
                      .ConvertAll(cuenta =>
                          new CO.Servidor.Servicios.ContratoDatos.Cajas.CACuentaExterna
                          {
                              Id = cuenta.CEX_IdCuentaExterna,
                              Descripcion = cuenta.CEX_Descripcion,
                              CodCtaExterna = cuenta.CEX_Codigo
                          });
            }
        }

        /// <summary>
        /// Obtiene las categorias de los conceptos
        /// </summary>
        /// <returns>lista ordenada de Categorias</returns>
        public List<CAConceptoCajaCategoriaDC> ObtenerCategoriaConceptosCaja()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CategoriaConceptoCaja_CAJ.OrderBy(or => or.CCC_Descripcion)
                                                            .ToList()
                                                            .ConvertAll(cat => new CAConceptoCajaCategoriaDC()
                {
                    IdCategoria = cat.CCC_IdCategoria,
                    Descripcion = cat.CCC_Descripcion
                });
            }
        }


        /// <summary>
        /// retorna los conceptos filtrados por categoria
        /// </summary>
        /// <param name="categoria"></param>
        /// <returns></returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCaja(CAEnumCategoriasConceptoCaja categoria)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
               return  contexto.CategConceptoCaja_VCAJ.Where(c => c.CCC_IdCategoria == (int)categoria).ToList().
                    ConvertAll<CAConceptoCajaDC>(c =>
                    new CAConceptoCajaDC
                    {
                        IdConceptoCaja = c.COC_IdConceptoCaja,
                        Descripcion = c.COC_Descripcion,
                        Nombre = c.COC_Nombre,
                        EsIngreso = c.COC_EsIngreso,
                        IdCuentaExterna = c.COC_IdCuentaExterna     ,
                        ContraPartidaCasaMatriz=c.COC_ContrapartidaCasaMatriz,
                        ContraPartidaCS=c.COC_ContrapartidaCS,
                        RequiereNoDocumento=c.COC_RequiereNoDocumento,
                     });

            }
        }

        /// <summary>
        /// Consulta los conceptos de caja filtrados, paginas y ordenados
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public List<CAConceptoCajaDC> ObtenerConceptosCaja(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            totalRegistros = 0;
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {                
                Dictionary<LambdaExpression, OperadorLogico> where = new Dictionary<LambdaExpression, OperadorLogico>();
                LambdaExpression lamda = contexto.CrearExpresionLambda<ConceptoCaja_CAJ>("COC_EsServicio", false.ToString(), OperadorComparacion.Equal);
                where.Add(lamda, OperadorLogico.And);

                return contexto.ConsultarConceptoCaja_CAJ(filtro, where, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                  .ConvertAll(concepto =>
                      {                          
                          CAConceptoCajaDC concep = new CAConceptoCajaDC()
                          {
                              IdConceptoCaja = concepto.COC_IdConceptoCaja,
                              Descripcion = concepto.COC_Descripcion,
                              Nombre = concepto.COC_Nombre,
                              EsIngreso = concepto.COC_EsIngreso,
                              IdCuentaExterna = concepto.COC_IdCuentaExterna,
                              RequiereNoDocumento=concepto.COC_RequiereNoDocumento,
                              GruposCategorias =new System.Collections.ObjectModel.ObservableCollection<CAConceptoCajaCategoriaDC>( contexto.ConceptoCajaCategoria_CAJ.Include("CategoriaConceptoCaja_CAJ").Where(con => con.CCA_IdConceptoCaja == concepto.COC_IdConceptoCaja).ToList().ConvertAll<CAConceptoCajaCategoriaDC>((c) =>
                              {
                                  return new CAConceptoCajaCategoriaDC()
                                  {
                                      IdCategoria=c.CCA_IdCategoria,
                                      Descripcion=c.CategoriaConceptoCaja_CAJ.CCC_Descripcion
                                  };
                              })),                              //GrupoCategoria = new CAConceptoCajaCategoriaDC()
                              //{
                              //    IdCategoria = idcategoria.Value
                              //},
                              IdCategoriaAnterior = 0,
                              ContraPartidaCasaMatriz = concepto.COC_ContrapartidaCasaMatriz,
                              ContraPartidaCS = concepto.COC_ContrapartidaCS
                          };
                          return concep;
                      }
                      );
            }
        }

       


        /// <summary>
        /// Se aplican cambios realizados sobre un concepto de caja
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void ActualizarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ConceptoCaja_CAJ concepto = contexto.ConceptoCaja_CAJ.FirstOrDefault(c => c.COC_IdConceptoCaja == conceptoCaja.IdConceptoCaja);
                short? idCuentaExterna = null;
                if (conceptoCaja.CuentaExterna != null)
                {
                    idCuentaExterna = conceptoCaja.CuentaExterna.Id;
                }
                if (concepto != null)
                {
                    concepto.COC_Nombre = conceptoCaja.Nombre;
                    concepto.COC_Descripcion = conceptoCaja.Descripcion;
                    concepto.COC_IdCuentaExterna = idCuentaExterna;
                    concepto.COC_RequiereNoDocumento = conceptoCaja.RequiereNoDocumento;
                    concepto.COC_EsIngreso = conceptoCaja.EsIngreso;
                    concepto.COC_ContrapartidaCasaMatriz = conceptoCaja.ContraPartidaCasaMatriz;
                    concepto.COC_ContrapartidaCS = conceptoCaja.ContraPartidaCS;
                    CARepositorioGestionCajasAudit.MapeoConceptoCaja(contexto, ControllerContext.Current.Usuario);

                    contexto.SaveChanges();                    
                }
            }
        }

        /// <summary>
        /// Se inserta un concepto de caja nuevo
        /// </summary>
        /// <param name="conceptoCaja"></param>
        public void AdicionarConceptoCaja(CAConceptoCajaDC conceptoCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                short? idCuentaExterna = null;
                if (conceptoCaja.CuentaExterna != null)
                {
                    idCuentaExterna = conceptoCaja.CuentaExterna.Id;
                }
                ConceptoCaja_CAJ concepto = new ConceptoCaja_CAJ()
                {
                    COC_CreadoPor = ControllerContext.Current.Usuario,
                    COC_Descripcion = conceptoCaja.Descripcion,
                    COC_EsIngreso = conceptoCaja.EsIngreso,
                    COC_EsServicio = false,
                    COC_RequiereNoDocumento=conceptoCaja.RequiereNoDocumento,
                    COC_FechaGrabacion = DateTime.Now,
                    COC_IdCuentaExterna = idCuentaExterna,
                    COC_IdConceptoCaja = conceptoCaja.IdConceptoCaja,
                    COC_Nombre = conceptoCaja.Nombre,
                    COC_VisibleAgenciaOPunto = false,
                    COC_VisibleMensajero = false,
                    COC_VisibleRacol = false,
                    COC_ContrapartidaCasaMatriz=conceptoCaja.ContraPartidaCasaMatriz,
                    COC_ContrapartidaCS=conceptoCaja.ContraPartidaCS
                };
                contexto.ConceptoCaja_CAJ.Add(concepto);

                ////Adiciona la relacion entre concepto y categoria cuando se crea el concepto                
                //foreach (CAConceptoCajaCategoriaDC categoria in conceptoCaja.GruposCategorias)
                //{
                //    AdicionarConceptoCategoria(categoria,concepto.COC_IdConceptoCaja);
                //}

                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Adiciona la relacion entre concepto y categoria cuando se crea el concepto
        /// </summary>
        /// <param name="conceptoCategoria">info del concepto</param>
        public void AdicionarConceptoCategoria(CAConceptoCajaCategoriaDC conceptoCategoria, int idConceptoCaja)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ConceptoCajaCategoria_CAJ nuevoConcepto = new ConceptoCajaCategoria_CAJ()
                {
                    CCA_IdCategoria = Convert.ToInt16(conceptoCategoria.IdCategoria),
                    CCA_IdConceptoCaja = idConceptoCaja,
                    CCA_CreadoPor = ControllerContext.Current.Usuario,
                    CCA_FechaGrabacion = DateTime.Now
                };
                contexto.ConceptoCajaCategoria_CAJ.Add(nuevoConcepto);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// actualiza la tabla de relacion del concepto con la
        /// categoria
        /// </summary>
        /// <param name="conceptoCategoria">concepto de caja</param>
        public void RemoverConceptoCategoria(CAConceptoCajaDC conceptoCategoria)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {

                //borro la Relacion y Agrego la Nueva
                contexto.ConceptoCajaCategoria_CAJ.Where(con => con.CCA_IdConceptoCaja == conceptoCategoria.IdConceptoCaja).ToList().ForEach(c =>
                {
                    contexto.ConceptoCajaCategoria_CAJ.Remove(c);
                });                                
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Cerrar las aperturas de caja de Casa Matriz, Operación Naciona, Bancos y Centros de Servicios que el usuario ha hecho
        /// </summary>
        /// <param name="idCasaMatriz">Identificación de la casa matriz sobre la cual se hacen las aperturas</param>
        /// <param name="idCodigoUsuario">Código del usuario que hizo las aperturas</param>
        /// <param name="idRacol">Identificación del RACOL desde donde se hacen las operaciones</param>
        /// <remarks>Las aperturas sobre centros de servicos se hacen sobre la caja 0</remarks>
        public void CerrarCajasGestion(short idCasaMatriz, long idCodigoUsuario, long idRacol)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                long? idRacolSeleccionado = null;
                string usuario = null;
                if (idRacol > 0)
                {
                    idRacolSeleccionado = idRacol;
                }

                if (idCodigoUsuario == -1)
                    usuario = ConstantesFramework.USUARIO_SISTEMA;
                else
                    usuario = ControllerContext.Current.Usuario;

                contexto.paCerrarAperturasCasaMatriz_CAJ(idCasaMatriz, idCodigoUsuario, usuario, idRacolSeleccionado);
            }
        }

        /// <summary>
        /// Obtener la información del cierre de las cajas de gestión
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la casa matriz donde se hace el cierre</param>
        /// <param name="idCodigoUsuario">Identificador del usuairo que hace el cierre</param>
        /// <returns>Colección con la información del cierre</returns>
        public IList<CACierreCajaGestionDC> ObtenerCierreCajasGestion(short idCasaMatriz, long idCodigoUsuario, int paginas = 10)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CierreCajaGestion_VCAJ
                  .Where(w => w.APC_IdCodigoUsuario == idCodigoUsuario)
                  .OrderByDescending(o => o.CCM_FechaGrabacion)
                  .Take(paginas)
                  .ToList()
                  .ConvertAll<CACierreCajaGestionDC>(item => new CACierreCajaGestionDC
                    {
                        DescripcionTipoApetura = item.TipoAperturaDesc,
                        FechaApertura = item.APC_FechaGrabacion,
                        FechaCierre = item.CCM_FechaGrabacion,
                        IdCierre = item.CCM_IdCierreCaja,
                        TipoApetura = item.APC_TipoApertura,
                        TotalEgresos = item.CCM_TotalEgresos,
                        TotalIngresos = item.CCM_TotalIngresos,
                        IdCasaMatriz = idCasaMatriz,
                        IdCodigoUsuario = item.APC_IdCodigoUsuario,
                        Usuario = item.APC_CreadoPor
                    });
            }
        }

        /// <summary>
        /// Obtiene todas las aperturas
        /// abiertas de Casa Matriz - Operacion nacional - Banco
        /// </summary>
        /// <returns>lista de aperturas activas</returns>
        public IList<CAAperturaCajaCasaMatrizDC> ObtenerAperturasCajaGestion()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.AperturaCasaMatriz_VCAJ.Where(cie => cie.APC_EstaAbierta == true)
                  .ToList()
                  .ConvertAll<CAAperturaCajaCasaMatrizDC>(item => new CAAperturaCajaCasaMatrizDC
                  {
                      IdAperturaCajaMatriz = item.APC_IdAperturaCaja,
                      IdCaja = item.APC_IdCaja,
                      BaseInicial = item.APC_BaseInicialApertura,
                      FechaGrabacion = item.APC_FechaGrabacion,
                      UsuarioApertura = item.APC_CreadoPor,
                      IdCodigoUsuario = item.APC_IdCodigoUsuario,
                      TipoApertura = item.APC_TipoApertura,
                      IdCasaMatriz = item.ACM_IdCasaMatriz,
                      NombreEmpresa = item.CAM_Nombre,
                      NitEmpresa = item.CAM_Nit,
                      CentroDeCostos = item.CAM_CentroCostos,
                      Estado = item.CAM_Estado
                  });
            }
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns></returns>
        public CABaseGestionCajasDC ObtenerBaseCajaOperacionNacional(int idCasaMatriz)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                BaseGestionCajas_CAJ baseCajaOpeNal = contexto.BaseGestionCajas_CAJ.FirstOrDefault(id => id.BGC_IdCasaMatriz == idCasaMatriz);
                CABaseGestionCajasDC baseOperacionNal = null;

                if (baseCajaOpeNal != null)
                {
                    baseOperacionNal = new CABaseGestionCajasDC()
                    {
                        BaseOperacionNacional = baseCajaOpeNal.BGC_BaseOperacionNacional,
                        BaseCasaMatriz = baseCajaOpeNal.BGC_BaseCasaMatriz
                    };
                }
                return baseOperacionNal;
            }
        }

        /// <summary>
        /// Obtiene la info la base de caja de Operacion Nacional
        /// de acuerdo con su Casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns>lista de Bases de Caja de las Casas Matriz y Operacion Nal</returns>
        public List<CABaseGestionCajasDC> ObtenerBasesDeCajasOperacionNacional()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.BaseGestionCajas_CAJ.ToList().ConvertAll<CABaseGestionCajasDC>(bCaja => new CABaseGestionCajasDC()
                {
                    CasaMatriz = new Servicios.ContratoDatos.Area.ARCasaMatrizDC()
                    {
                        IdCasaMatriz = bCaja.BGC_IdCasaMatriz,
                    },
                    BaseCasaMatriz = bCaja.BGC_BaseCasaMatriz,
                    BaseOperacionNacional = bCaja.BGC_BaseOperacionNacional
                });
            }
        }

        /// <summary>
        /// Adiciona la Base de la Caja de Operacion
        /// Nacional por Casa Matriz
        /// </summary>
        /// <param name="baseCajaOperacionNacional">info de Ingreso</param>
        public void AdicionarBaseCajaOperacionNacional(CABaseGestionCajasDC nvBaseCajaOpeNal)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                BaseGestionCajas_CAJ nvBaseCaja = new BaseGestionCajas_CAJ()
                {
                    BGC_IdCasaMatriz = (short)nvBaseCajaOpeNal.CasaMatriz.IdCasaMatriz,
                    BGC_BaseOperacionNacional = nvBaseCajaOpeNal.BaseOperacionNacional,
                    BGC_BaseCasaMatriz = nvBaseCajaOpeNal.BaseCasaMatriz,
                    BGC_CreadoPor = ControllerContext.Current.Usuario,
                    BGC_FechaGrabacion = DateTime.Now
                };
                contexto.BaseGestionCajas_CAJ.Add(nvBaseCaja);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza la Base de
        /// operacion Nacional y de
        /// Casa Matriz
        /// </summary>
        /// <param name="updateBaseCajaOpeNal"></param>
        public void ActualizarBaseCajaOperacionNacional(CABaseGestionCajasDC updateBaseCajaOpeNal)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                BaseGestionCajas_CAJ updateBaseCaja = contexto.BaseGestionCajas_CAJ.FirstOrDefault(ap => ap.BGC_IdCasaMatriz == updateBaseCajaOpeNal.CasaMatriz.IdCasaMatriz);

                if (updateBaseCaja != null)
                {
                    updateBaseCaja.BGC_BaseOperacionNacional = updateBaseCajaOpeNal.BaseOperacionNacional;
                    updateBaseCaja.BGC_BaseCasaMatriz = updateBaseCajaOpeNal.BaseCasaMatriz;
                    contexto.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Borra la Base de la operacion
        /// Nacional y de la Casa Matriz
        /// </summary>
        /// <param name="borrarBaseCajaOpeNal"></param>
        public void BorrarBaseCajaOperacionNacional(CABaseGestionCajasDC borrarBaseCajaOpeNal)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                BaseGestionCajas_CAJ borrarBaseCaja = contexto.BaseGestionCajas_CAJ.FirstOrDefault(ap => ap.BGC_IdCasaMatriz == borrarBaseCajaOpeNal.CasaMatriz.IdCasaMatriz);

                if (borrarBaseCaja != null)
                {
                    contexto.BaseGestionCajas_CAJ.Remove(borrarBaseCaja);
                    contexto.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Verifica si un número de consignación específico ya está asociado a un banco en su caja
        /// </summary>
        /// <param name="numConsignacion"></param>
        /// <param name="idBanco"></param>
        /// <returns></returns>
        public string NumConsignacionExistente(string numConsignacion, string idBanco)
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                var operacionBanco = contexto.OperacionCajaBanco_CAJ.Where(con => con.CAB_NumeroDocumento == numConsignacion && con.CAB_IdBanco == idBanco).FirstOrDefault();

                if (operacionBanco != null)
                    return "La consignación No. " + numConsignacion + " ya fué registrada para el banco " + operacionBanco.CAB_DescripcionBanco + " por valor de " + operacionBanco.CAB_Valor.ToString("c") + " por el cajero " + operacionBanco.CAB_CreadoPor + " en la fecha " + operacionBanco.CAB_FechaGrabacion.ToString("dd/MM/yyyy");

                return null;
            }
        }
        

        #region Tipo Observacion Recoleccion Dinero

        /// <summary>
        /// Obtiene los tipos de Observaciones
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Obtiene los tipos de Observaciones</returns>
        ///
        public IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacionFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloCajas contexto = new ModeloCajas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsTipoObsPuntoAgencia_CAJ(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                    .ToList()
                    .ConvertAll(r => new CATipoObsPuntoAgenciaDC()
                    {
                        idTipoObservacion = r.TOP_IdTipoObservacion,
                        Descripcion = r.TOP_Descripcion,
                        EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS
                    });
            }
        }

        /// <summary>
        /// Adiciona un tipo de Observacion
        /// </summary>
        public void AdicionarTipoObservacion(CATipoObsPuntoAgenciaDC tipoObservacion)
        {
            using (ModeloCajas contexto = new ModeloCajas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoObsPuntoAgencia_CAJ tipoentrega = new TipoObsPuntoAgencia_CAJ
                {
                    TOP_CreadoPor = ControllerContext.Current.Usuario,
                    TOP_Descripcion = tipoObservacion.Descripcion,
                    TOP_FechaGrabacion = DateTime.Now
                };
                contexto.TipoObsPuntoAgencia_CAJ.Add(tipoentrega);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        ///  Editar tipo ibservacion
        /// </summary>
        /// <param name="tipoObservacion">clase tipoObservacion</param>
        public void EditarTipoEbservacion(CATipoObsPuntoAgenciaDC tipoObservacion)
        {
            using (ModeloCajas contexto = new ModeloCajas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoObsPuntoAgencia_CAJ tipoObser = contexto.TipoObsPuntoAgencia_CAJ
                    .Where(r => r.TOP_IdTipoObservacion == tipoObservacion.idTipoObservacion)
                    .FirstOrDefault();
                tipoObser.TOP_Descripcion = tipoObservacion.Descripcion;
                contexto.SaveChanges();
            }
        }

        /// <summary>
        ///  eliminar tipo ibservacion
        /// </summary>
        /// <param name="tipoObservacion">clase tipoObservacion</param>
        public void EliminarTipoEbservacion(CATipoObsPuntoAgenciaDC tipoObservacion)
        {
            using (ModeloCajas contexto = new ModeloCajas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                TipoObsPuntoAgencia_CAJ tipoObser = contexto.TipoObsPuntoAgencia_CAJ
                    .Where(r => r.TOP_IdTipoObservacion == tipoObservacion.idTipoObservacion)
                    .FirstOrDefault();
                contexto.TipoObsPuntoAgencia_CAJ.Remove(tipoObser);
                contexto.SaveChanges();
            }
        }

        public IEnumerable<CATipoObsPuntoAgenciaDC> ObtenerTipoObservacion()
        {
            using (ModeloCajas contexto = new ModeloCajas(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TipoObsPuntoAgencia_CAJ.OrderBy(r => r.TOP_Descripcion).ToList().ConvertAll(r => new CATipoObsPuntoAgenciaDC()
                {
                    idTipoObservacion = r.TOP_IdTipoObservacion,
                    Descripcion = r.TOP_Descripcion
                });
            }
        }

        #endregion Tipo Observacion Recoleccion Dinero

        #region Cajas Disponibles

        /// <summary>
        /// Obtiene las Cajas disponibles asignadas a los
        /// centros de Servicio
        /// </summary>
        /// <returns>Numero de Cajas Disponibles</returns>
        public int ObtenerNumeroCajasDisponibles()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CajasDisponibles_PUA.Count();
            }
        }

        /// <summary>
        /// Agrega una caja para que sea disponible a
        /// los puntos o centros de servicio
        /// </summary>
        /// <param name="numeroCajas">numero de caja</param>
        public void AgregarCajaDisponible()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                int numCajaActual = contexto.CajasDisponibles_PUA.Count();

                CajasDisponibles_PUA nwCaja = new CajasDisponibles_PUA()
                {
                    CAD_IdCaja = numCajaActual + 1,
                    CAD_CreadoPor = ControllerContext.Current.Usuario,
                    CAD_FechaGrabacion = DateTime.Now
                };
                contexto.CajasDisponibles_PUA.Add(nwCaja);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Borra el numero de Caja disponible
        /// </summary>
        public void EliminarCajaDisponible()
        {
            using (ModeloCajas contexto = new ModeloCajas(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                int numCajaActual = contexto.CajasDisponibles_PUA.Count();

                CajasDisponibles_PUA delCaja = contexto.CajasDisponibles_PUA.FirstOrDefault(caj => caj.CAD_IdCaja == numCajaActual);

                contexto.CajasDisponibles_PUA.Remove(delCaja);
                contexto.SaveChanges();
            }
        }

        #endregion Cajas Disponibles
    }
}