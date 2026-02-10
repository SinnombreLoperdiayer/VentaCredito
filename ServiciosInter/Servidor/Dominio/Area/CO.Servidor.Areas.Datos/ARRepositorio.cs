using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using CO.Servidor.Areas.Comun;
using CO.Servidor.Areas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Area;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Areas.Datos
{
    public class ARRepositorio
    {
        private static readonly ARRepositorio instancia = new ARRepositorio();
        private const string NombreModelo = "ModeloAreas";

        /// <summary>
        /// Retorna la instancia de la clase ARRepositorio
        /// </summary>
        public static ARRepositorio Instancia
        {
            get { return ARRepositorio.instancia; }
        }

        #region ctor

        private ARRepositorio()
        {
        }

        #endregion ctor

        /// <summary>
        /// Retorna todas las casas matriz activas
        /// </summary>
        /// <returns>Colección con todas las casas matrices</returns>
        public IList<ARCasaMatrizDC> ObtenerTodasLasCasaMatriz()
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.CasaMatriz_ARE
                  .Where(c => c.CAM_Estado == ConstantesFramework.ESTADO_ACTIVO)
                  .ToList()
                   .ConvertAll(c =>
                     new ARCasaMatrizDC()
                     {
                         IdCasaMatriz = c.CAM_IdCasaMatriz,
                         CentroCostos = c.CAM_CentroCostos,
                         CodigoMinisterio = c.CAM_CodigoMinisterio,
                         CodigoSucursalERP = c.CAM_CodigoSucursalERP,
                         DigitoVerificacion = c.CAM_DigitoVerificacion,
                         Direccion = c.CAM_Direccion,
                         Estado = c.CAM_Estado,
                         IdLocalidad = c.CAM_IdLocalidad,
                         Nit = c.CAM_Nit,
                         Nombre = c.CAM_Nombre,
                         Sigla = c.CAM_Sigla,
                         Telefono = c.CAM_Telefono
                     }).OrderBy(c => c.Nombre).ToList();
            }
        }

        /// <summary>
        /// Método que retorna la información de una casa matriz a partir de su id
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns></returns>
        public ARCasaMatrizDC ObtenerCasaMatriz(int idCasaMatriz)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                CasaMatriz_VARE casaMatriz = contexto.CasaMatriz_VARE
                     .Where(c => c.CAM_IdCasaMatriz == idCasaMatriz)
                     .FirstOrDefault();

                if (casaMatriz != null)
                {
                    return new ARCasaMatrizDC()
                    {
                        IdCasaMatriz = casaMatriz.CAM_IdCasaMatriz,
                        CentroCostos = casaMatriz.CAM_CentroCostos,
                        CodigoMinisterio = casaMatriz.CAM_CodigoMinisterio,
                        CodigoSucursalERP = casaMatriz.CAM_CodigoSucursalERP,
                        DigitoVerificacion = casaMatriz.CAM_DigitoVerificacion,
                        Direccion = casaMatriz.CAM_DigitoVerificacion,
                        Estado = casaMatriz.CAM_Estado,
                        IdLocalidad = casaMatriz.CAM_IdLocalidad,
                        NombreLocalidad = casaMatriz.NombreCompletoLocalidad,
                        Nit = casaMatriz.CAM_Nit,
                        Nombre = casaMatriz.CAM_Nombre,
                        Sigla = casaMatriz.CAM_Sigla,
                        Telefono = casaMatriz.CAM_Telefono
                    };
                }

                ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_AREAS, AREnumTipoErrorAreas.ERROR_CASA_MATRIZ_NO_CONFIGURADA.ToString(),
                                                 String.Format(ARResolverMensajes.CargarMensaje(AREnumTipoErrorAreas.ERROR_CASA_MATRIZ_NO_CONFIGURADA), idCasaMatriz));
                throw new FaultException<ControllerException>(excepcion);
            }
        }

        /// <summary>
        /// Método para obtener todas las gestiones de todas las casas matrices
        /// </summary>
        /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
        public IList<ARGestionDC> ObtenerGestiones()
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.GestionesCasaMatriz_VARE
                  .ToList()
                   .ConvertAll(c =>
                     new ARGestionDC()
                     {
                         IdCasaMatriz = c.CAM_IdCasaMatriz,
                         CentroCostos = c.GES_CentroCostos,
                         IdGestion = c.GES_CodigoGestion,
                         Descripcion = c.GES_Descripcion,
                         IdGestionExterno = c.GES_IdGestion,
                         IdMacroProceso = c.MAP_IdMacroProceso,
                         CodigoBodegaERP = c.GES_CodigoBodegaERP
                     });
            }
        }

        /// <summary>
        /// Método para obtener todas las gestiones de todas las casas matrices
        /// </summary>
        /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
        public IList<ARGestionDC> ObtenerGestiones(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarEqualsGestionesCasaMatriz_VARE(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                   .ConvertAll(c =>
                     new ARGestionDC()
                     {
                         IdCasaMatriz = c.CAM_IdCasaMatriz,
                         CentroCostos = c.GES_CentroCostos,
                         IdGestion = c.GES_CodigoGestion,
                         Descripcion = c.GES_Descripcion,
                         IdMacroProceso = c.MAP_IdMacroProceso,
                         CodigoBodegaERP = c.GES_CodigoBodegaERP
                     });
            }
        }

        /// <summary>
        /// Obtener los bancos configurados para una casa matriz
        /// </summary>
        /// <param name="idCasaMatriz">Identificador de la casa matriz</param>
        /// <returns>Información de la casa matriz y sus bancos</returns>
        public ARCasaMatrizCuentaBancoDC ObtenerCuentaBancoCasaMatriz(short idCasaMatriz)
        {
            ARCasaMatrizCuentaBancoDC resultado = null;
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<CasaMatrizCuentaBanco_VARE> cuentaBancoCM = contexto.CasaMatrizCuentaBanco_VARE
                  .Where(w => w.CAM_IdCasaMatriz == idCasaMatriz)
                  .ToList();

                if (cuentaBancoCM.Count() > 0)
                {
                    resultado = new ARCasaMatrizCuentaBancoDC
                    {
                        IdCasaMatriz = idCasaMatriz,
                        NombreCasaMatriz = cuentaBancoCM.FirstOrDefault().CAM_Nombre,
                        CuentaBanco = new List<ARCuentaCasaMatrizDC>()
                    };

                    Parallel.ForEach(cuentaBancoCM, item =>
                    {
                        resultado.CuentaBanco.Add(new ARCuentaCasaMatrizDC
                        {
                            Banco = new PABanco
                            {
                                Descripcion = item.BAN_Descripcion,
                                EstadoRegistro = EnumEstadoRegistro.SIN_CAMBIOS,
                                IdBanco = item.BAE_IdBanco
                            },
                            IdCasaMatriz = idCasaMatriz,
                            DescripcionTipoCuenta = item.TCB_Descripcion,
                            NumeroCuenta = item.CBE_NumeroCuenta,
                            TipoCuenta = item.CBE_TipoCuenta
                        });
                    });
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtener los datos de la empresa (casa matriz según parámetros del Framework
        /// </summary>
        /// <returns>Información de la empresa</returns>
        public AREmpresaDC ObtenerDatosEmpresa()
        {
            AREmpresaDC resultado = null;
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                int idEmpresa;

                if (int.TryParse(Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.ID_CASA_MATRIZ_EMPRESA),
                  out idEmpresa))
                {
                    CasaMatriz_ARE empresa = contexto.CasaMatriz_ARE
                   .Where(w => w.CAM_IdCasaMatriz == idEmpresa)
                   .FirstOrDefault();

                    string nombreLocalidad = null;
                    var loc = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerLocalidadPorId(empresa.CAM_IdLocalidad);

                    if (loc != null)
                    {
                        nombreLocalidad = loc.Nombre;
                    }

                    resultado = new AREmpresaDC
                    {
                        CodigoMinisterio = empresa.CAM_CodigoMinisterio,
                        DigitoVerificacion = empresa.CAM_DigitoVerificacion,
                        Direccion = empresa.CAM_Direccion,
                        IdEmpresa = empresa.CAM_IdCasaMatriz,
                        IdentificacionPersonaExterna = null,
                        IdentificacionPersonaInterna = null,
                        IdLocalidad = empresa.CAM_IdLocalidad,
                        Nit = empresa.CAM_Nit,
                        NombreEmpresa = empresa.CAM_Nombre,
                        SiglaEmpresa = empresa.CAM_Sigla,
                        Telefono = empresa.CAM_Telefono,
                        NombreLocalidad = nombreLocalidad
                    };
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene los macroprocesos de una casa matriz
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns></returns>
        public List<ARMacroprocesoDC> ObtenerMacroprocesoCasaMatriz(int idCasaMatriz)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MacroProceso_ARE
                  .Where(m => m.MAP_IdCasaMatriz == idCasaMatriz)
                  .ToList()
                  .ConvertAll<ARMacroprocesoDC>(m =>
                    new ARMacroprocesoDC()
                    {
                        Descripcion = m.MAP_Descripcion,
                        IdCasaMatriz = m.MAP_IdCasaMatriz,
                        IdMacroproceso = m.MAP_IdMacroProceso
                    });
            }
        }

        /// <summary>
        /// Obtiene los macroprocesos
        /// </summary>
        /// <param name="idCasaMatriz"></param>
        /// <returns></returns>
        public List<ARMacroprocesoDC> ObtenerTodosMacroprocesos()
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MacroProceso_ARE
                  .ToList()
                  .ConvertAll<ARMacroprocesoDC>(m =>
                    new ARMacroprocesoDC()
                    {
                        Descripcion = m.MAP_Descripcion,
                        IdCasaMatriz = m.MAP_IdCasaMatriz,
                        IdMacroproceso = m.MAP_IdMacroProceso
                    });
            }
        }

        /// <summary>
        /// Método para obtener las gestiones por macroproceso
        /// </summary>
        /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
        public IList<ARGestionDC> ObtenerGestionesMacroproceso(string idMacroproceso)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Gestion_ARE
                  .Where(g => g.GES_IdMacroProceso == idMacroproceso)
                  .ToList()
                   .ConvertAll(c =>
                     new ARGestionDC()
                     {
                         CentroCostos = c.GES_CentroCostos,
                         IdGestion = c.GES_CodigoGestion,
                         Descripcion = c.GES_Descripcion,
                         IdGestionExterno = c.GES_IdGestion,
                         IdMacroProceso = c.GES_IdMacroProceso,
                         CodigoBodegaERP = c.GES_CodigoBodegaERP
                     });
            }
        }

        /// <summary>
        /// Método para obtener las gestiones por macroproceso
        /// </summary>
        /// <returns>Coleciones con todas las gestiones de todas las casas matrices</returns>
        public IList<ARGestionDC> ObtenerTodasGestionesMacroproceso()
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Gestion_ARE
                  .ToList()
                   .ConvertAll(c =>
                     new ARGestionDC()
                     {
                         CentroCostos = c.GES_CentroCostos,
                         IdGestion = c.GES_CodigoGestion,
                         Descripcion = c.GES_Descripcion,
                         IdGestionExterno = c.GES_IdGestion,
                         IdMacroProceso = c.GES_IdMacroProceso,
                         CodigoBodegaERP = c.GES_CodigoBodegaERP
                     });
            }
        }

        /// <summary>
        /// Retorna los procesos de una gestion
        /// </summary>
        /// <param name="idGestion">Id de la gestión</param>
        /// <returns>Lista de procesos</returns>
        public IList<ARProcesoDC> ObtenerProcesosGestion(long idGestion)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.Proceso_ARE
                  .Where(p => p.PRO_CodigoGestion == idGestion)
                  .ToList()
                  .ConvertAll<ARProcesoDC>(
                  p =>
                  new ARProcesoDC()
                  {
                      CentroCostos = p.PRO_CentroCostos,
                      CodBodegaErp = p.PRO_CodigoBodegaERP,
                      CodGestion = p.PRO_CodigoGestion,
                      Descripcion = p.PRO_Descripcion,
                      IdProceso = p.PRO_IdProceso,
                      IdCodigoProceso = p.PRO_IdCodigoProceso
                  });
            }
        }

        /// <summary>
        /// Retorna todos los procesos
        /// </summary>
        /// <param name="idGestion">Id de la gestión</param>
        /// <returns>Lista de procesos</returns>
        public IList<ARProcesoDC> ObtenerTodosProcesos()
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ProcesosCasaMatriz_VARE
                  .ToList()
                  .ConvertAll<ARProcesoDC>(
                  p =>
                  new ARProcesoDC()
                  {
                      CentroCostos = p.PRO_CentroCostos,
                      CodBodegaErp = p.PRO_CodigoBodegaERP,
                      CodGestion = p.PRO_CodigoGestion,
                      Descripcion = p.CAM_Nombre + "/" + p.MAP_Descripcion + "/" + p.PRO_Descripcion,
                      IdProceso = p.PRO_IdProceso,
                      IdCodigoProceso = p.PRO_IdCodigoProceso
                  });
            }
        }

        /// <summary>
        /// Método para obtener todos los procesos de todas las casas matrices
        /// </summary>
        /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
        public IList<ARProcesoDC> ObtenerProcesos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarEqualsProcesosCasaMatriz_VARE(filtro, campoOrdenamiento, out totalRegistros, indicePagina, registrosPorPagina, ordenamientoAscendente)
                  .ToList()
                   .ConvertAll(p =>
                     new ARProcesoDC()
                     {
                         CentroCostos = p.PRO_CentroCostos,
                         CodBodegaErp = p.PRO_CodigoBodegaERP,
                         CodGestion = p.PRO_CodigoGestion,
                         Descripcion = p.PRO_Descripcion,
                         IdProceso = p.PRO_IdProceso,
                         IdCodigoProceso = p.PRO_IdCodigoProceso
                     });
            }
        }

        /// <summary>
        /// Método para obtener todos los procesos de todas las casas matrices
        /// </summary>
        /// <returns>Coleccion con todos los procesos de todas las casas matrices</returns>
        public IList<ARProcesoDC> ObtenerProcesosCasaMatriz(int casaMatriz)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                return contexto
                  .ProcesosCasaMatriz_VARE.Where(r => r.CAM_IdCasaMatriz == casaMatriz)
                  .ToList()
                   .ConvertAll(p =>
                     new ARProcesoDC()
                     {
                         CentroCostos = p.PRO_CentroCostos,
                         CodBodegaErp = p.PRO_CodigoBodegaERP,
                         CodGestion = p.PRO_CodigoGestion,
                         Descripcion = p.PRO_Descripcion,
                         IdProceso = p.PRO_IdProceso,
                         IdCodigoProceso = p.PRO_IdCodigoProceso
                     });
            }
        }

        /// <summary>
        /// Obtener la casa matriz de un proceso
        /// </summary>
        /// <param name="idProceso">Id del proceso</param>
        /// <returns>Objeto casa matriz</returns>
        public ARCasaMatrizDC ObtenerCasaMatrizProceso(long idProceso)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                ARCasaMatrizDC casa = new ARCasaMatrizDC();
                ProcesosCasaMatriz_VARE proceso = contexto.ProcesosCasaMatriz_VARE.Where(p => p.PRO_IdProceso == idProceso).FirstOrDefault();
                if (proceso != null)
                {
                    casa.IdCasaMatriz = proceso.CAM_IdCasaMatriz;
                    casa.Nombre = proceso.CAM_Nombre;
                    casa.CentroCostos = proceso.CAM_CentroCostos;
                    casa.CodigoMinisterio = proceso.CAM_CodigoMinisterio;
                    casa.CodigoSucursalERP = proceso.CAM_CodigoSucursalERP;
                    casa.DigitoVerificacion = proceso.CAM_DigitoVerificacion;
                    casa.Direccion = proceso.CAM_Direccion;
                    casa.Estado = proceso.CAM_Estado;
                    casa.IdLocalidad = proceso.CAM_IdLocalidad;
                    casa.NombreLocalidad = proceso.NombreCompleto;
                    casa.Nit = proceso.CAM_Nit;
                    casa.Telefono = proceso.CAM_Telefono;
                    casa.Sigla = proceso.CAM_Sigla;
                    casa.DescripcionGestion = proceso.GES_Descripcion;
                    casa.IdGestion = proceso.GES_IdGestion;
                    casa.CodigoGestion = proceso.GES_CodigoGestion;
                }
                return casa;
            }
        }

        #region Configuración Casa Matriz

        public void Guardar(ARCasaMatrizConTodo casaMatrizInfo)
        {
            using (ModeloAreasEntity contexto = new ModeloAreasEntity(FabricaConexionesDb.Instancia.CrearDbConexion().ObtenerConnectionString(NombreModelo)))
            {
                //guardar casa Matriz
                GuardarCambiosCasaMatriz(casaMatrizInfo.CasaMatriz, contexto);

                //guardar macroprocesos
                GuardarCambiosMacroprocesos(casaMatrizInfo.MacroProcesos, contexto);

                //guardar gestiones
                GuardarGestiones(casaMatrizInfo.Gestiones, contexto);

                //guardar procesos
                GuardarProcesos(casaMatrizInfo.Procesos, contexto);

                //guardar todos los cambios
                contexto.SaveChanges();
            }
        }

        private void GuardarCambiosCasaMatriz(ARCasaMatrizDC casaMatriz, ModeloAreasEntity contexto)
        {
            CasaMatriz_ARE casaMatrizCons;

            switch (casaMatriz.EstadoRegistro)
            {
                case EnumEstadoRegistro.SIN_CAMBIOS:
                    break;

                case EnumEstadoRegistro.ADICIONADO:
                    casaMatrizCons = new CasaMatriz_ARE();
                    MapearCasaMatriz(casaMatrizCons, casaMatriz);

                    contexto.CasaMatriz_ARE
                      .Add(casaMatrizCons);
                    break;

                case EnumEstadoRegistro.MODIFICADO:
                    casaMatrizCons = contexto.CasaMatriz_ARE
                    .Where(w => w.CAM_IdCasaMatriz == casaMatriz.IdCasaMatriz)
                    .FirstOrDefault();

                    if (casaMatrizCons != null)
                    {
                        MapearCasaMatriz(casaMatrizCons, casaMatriz);
                    }

                    break;

                case EnumEstadoRegistro.BORRADO:
                    casaMatrizCons = contexto.CasaMatriz_ARE
                     .Where(w => w.CAM_IdCasaMatriz == casaMatriz.IdCasaMatriz)
                     .FirstOrDefault();

                    if (casaMatrizCons != null)
                    {
                        contexto.CasaMatriz_ARE
                          .Remove(casaMatrizCons);
                    }

                    break;

                default:
                    break;
            }
        }

        private void GuardarCambiosMacroprocesos(IList<ARMacroprocesoDC> macroProcesos, ModeloAreasEntity contexto)
        {
            if (macroProcesos == null)
            {
                return;
            }

            MacroProceso_ARE macroProcesoCons;

            foreach (var macroProceso in macroProcesos)
            {
                switch (macroProceso.EstadoRegistro)
                {
                    case EnumEstadoRegistro.SIN_CAMBIOS:
                        break;

                    case EnumEstadoRegistro.ADICIONADO:
                        macroProcesoCons = new MacroProceso_ARE();

                        MaperarMacropProceso(macroProcesoCons, macroProceso);

                        contexto.MacroProceso_ARE
                          .Add(macroProcesoCons);
                        break;

                    case EnumEstadoRegistro.MODIFICADO:
                        macroProcesoCons = contexto.MacroProceso_ARE
                          .Where(w => w.MAP_IdMacroProceso == macroProceso.IdMacroproceso)
                          .FirstOrDefault();

                        if (macroProcesoCons != null)
                        {
                            MaperarMacropProceso(macroProcesoCons, macroProceso);
                        }

                        break;

                    case EnumEstadoRegistro.BORRADO:
                        macroProcesoCons = contexto.MacroProceso_ARE
                         .Where(w => w.MAP_IdMacroProceso == macroProceso.IdMacroproceso)
                         .FirstOrDefault();

                        if (macroProcesoCons != null)
                        {
                            contexto.MacroProceso_ARE.Remove(macroProcesoCons);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void GuardarGestiones(IList<ARGestionDC> gestiones, ModeloAreasEntity contexto)
        {
            if (gestiones == null)
            {
                return;
            }

            Gestion_ARE gestionCons;

            foreach (var gesion in gestiones)
            {
                switch (gesion.EstadoRegistro)
                {
                    case EnumEstadoRegistro.SIN_CAMBIOS:
                        break;

                    case EnumEstadoRegistro.ADICIONADO:
                        gestionCons = new Gestion_ARE();

                        MapearGestion(gestionCons, gesion);

                        contexto.Gestion_ARE
                          .Add(gestionCons);

                        break;

                    case EnumEstadoRegistro.MODIFICADO:

                        gestionCons = contexto.Gestion_ARE
                          .Where(w => w.GES_CodigoGestion == gesion.IdGestion)
                          .FirstOrDefault();

                        if (gestionCons != null)
                        {
                            MapearGestion(gestionCons, gesion);
                        }
                        break;

                    case EnumEstadoRegistro.BORRADO:

                        gestionCons = contexto.Gestion_ARE
                          .Where(w => w.GES_CodigoGestion == gesion.IdGestion)
                          .FirstOrDefault();

                        if (gestionCons != null)
                        {
                            contexto.Gestion_ARE
                              .Remove(gestionCons);
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        private void GuardarProcesos(IList<ARProcesoDC> procesos, ModeloAreasEntity contexto)
        {
            if (procesos == null)
            {
                return;
            }

            Proceso_ARE proceosCons;

            foreach (ARProcesoDC proceso in procesos)
            {
                switch (proceso.EstadoRegistro)
                {
                    case EnumEstadoRegistro.SIN_CAMBIOS:
                        break;

                    case EnumEstadoRegistro.ADICIONADO:
                        proceosCons = new Proceso_ARE();

                        MapearProceso(proceosCons, proceso);

                        contexto.Proceso_ARE
                          .Add(proceosCons);
                        break;

                    case EnumEstadoRegistro.MODIFICADO:
                        proceosCons = contexto.Proceso_ARE
                          .Where(w => w.PRO_IdProceso == proceso.IdProceso)
                          .FirstOrDefault();

                        if (proceosCons != null)
                        {
                            MapearProceso(proceosCons, proceso);
                        }

                        break;

                    case EnumEstadoRegistro.BORRADO:
                        proceosCons = contexto.Proceso_ARE
                         .Where(w => w.PRO_IdProceso == proceso.IdProceso)
                         .FirstOrDefault();

                        if (proceosCons != null)
                        {
                            contexto.Proceso_ARE
                              .Remove(proceosCons);
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        private void MapearCasaMatriz(CasaMatriz_ARE destino, ARCasaMatrizDC casaMatrizObjetoOrigen)
        {
            if (destino == null)
            {
                destino = new CasaMatriz_ARE();
            }

            destino.CAM_CentroCostos = casaMatrizObjetoOrigen.CentroCostos.Trim().ToUpper();
            destino.CAM_CodigoMinisterio = (short)casaMatrizObjetoOrigen.CodigoMinisterio;
            destino.CAM_CodigoSucursalERP = casaMatrizObjetoOrigen.CodigoSucursalERP.Trim().ToUpper();
            destino.CAM_CreadoPor = ControllerContext.Current.Usuario;
            destino.CAM_DigitoVerificacion = casaMatrizObjetoOrigen.DigitoVerificacion ?? String.Empty;
            destino.CAM_Direccion = casaMatrizObjetoOrigen.Direccion.Trim().ToUpper();
            destino.CAM_Estado = casaMatrizObjetoOrigen.Estado ?? "ACT";
            destino.CAM_FechaGrabacion = DateTime.Now;
            destino.CAM_IdCasaMatriz = (short)casaMatrizObjetoOrigen.IdCasaMatriz;
            destino.CAM_IdLocalidad = casaMatrizObjetoOrigen.IdLocalidad;
            destino.CAM_Nit = casaMatrizObjetoOrigen.Nit.Trim();
            destino.CAM_Nombre = casaMatrizObjetoOrigen.Nombre.Trim().ToUpper();
            destino.CAM_Sigla = casaMatrizObjetoOrigen.Sigla ?? "N/A";
            destino.CAM_Telefono = casaMatrizObjetoOrigen.Telefono.Trim();
        }

        private void MaperarMacropProceso(MacroProceso_ARE destino, ARMacroprocesoDC macroProcesoObjetoOrigen)
        {
            if (String.IsNullOrEmpty(macroProcesoObjetoOrigen.IdMacroproceso))
            {
                ControllerException excepcion = new ControllerException(COConstantesModulos.MODULO_AREAS, AREnumTipoErrorAreas.ERROR_MACROPROCESO_SIN_CODIGO.ToString(),
                                               String.Format(ARResolverMensajes.CargarMensaje(AREnumTipoErrorAreas.ERROR_MACROPROCESO_SIN_CODIGO)));
                throw new FaultException<ControllerException>(excepcion);
            }

            if (destino == null)
            {
                destino = new MacroProceso_ARE();
            }

            destino.MAP_CreadoPor = ControllerContext.Current.Usuario;
            destino.MAP_Descripcion = macroProcesoObjetoOrigen.Descripcion.Trim().ToUpper();
            destino.MAP_FechaGrabacion = DateTime.Now;
            destino.MAP_IdCasaMatriz = (short)macroProcesoObjetoOrigen.IdCasaMatriz;
            destino.MAP_IdMacroProceso = macroProcesoObjetoOrigen.IdMacroproceso.Trim().ToUpper();
        }

        private void MapearGestion(Gestion_ARE destino, ARGestionDC gestionObjetoOrigen)
        {
            if (destino == null)
            {
                destino = new Gestion_ARE();
            }

            destino.GES_CentroCostos = gestionObjetoOrigen.CentroCostos.Trim();
            destino.GES_CodigoBodegaERP = gestionObjetoOrigen.CodigoBodegaERP.Trim();
            destino.GES_CodigoGestion = gestionObjetoOrigen.IdGestion;
            destino.GES_CreadoPor = ControllerContext.Current.Usuario;
            destino.GES_Descripcion = gestionObjetoOrigen.Descripcion.Trim().ToUpper();
            destino.GES_FechaGrabacion = DateTime.Now;
            destino.GES_IdGestion = gestionObjetoOrigen.IdGestionExterno.Trim().ToUpper();
            destino.GES_IdMacroProceso = gestionObjetoOrigen.IdMacroProceso;
        }

        private void MapearProceso(Proceso_ARE destino, ARProcesoDC procesoObjetoOrigen)
        {
            if (destino == null)
            {
                destino = new Proceso_ARE();
            }

            destino.PRO_CentroCostos = procesoObjetoOrigen.CentroCostos.Trim();
            destino.PRO_CodigoBodegaERP = procesoObjetoOrigen.CodBodegaErp.Trim();
            destino.PRO_CodigoGestion = procesoObjetoOrigen.CodGestion;
            destino.PRO_CreadoPor = ControllerContext.Current.Usuario;
            destino.PRO_Descripcion = procesoObjetoOrigen.Descripcion.Trim().ToUpper();
            destino.PRO_FechaGrabacion = DateTime.Now;
            destino.PRO_IdCodigoProceso = procesoObjetoOrigen.IdCodigoProceso.Trim().ToUpper();
            destino.PRO_IdProceso = procesoObjetoOrigen.IdProceso;
        }

        #endregion Configuración Casa Matriz
    }
}