using System;
using System.Collections.Generic;
using System.Linq;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Comun;
using CO.Servidor.Suministros.Datos.Modelo;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.ServiceModel;

namespace CO.Servidor.Suministros.Datos
{
    /// <summary>
    /// Contiene acceso a datos relacionado con contenedores y tulas
    /// </summary>
    public class SURepositorioConsolidados
    {
        private const string NombreModelo = "ModeloSuministros";

        #region Singleton

        private static readonly SURepositorioConsolidados instancia = new SURepositorioConsolidados();

        /// <summary>
        /// Retorna la instancia de la clase TARepositorio
        /// </summary>
        public static SURepositorioConsolidados Instancia
        {
            get { return SURepositorioConsolidados.instancia; }
        }

        #endregion Singleton

         #region Consultas

        /// <summary>
        /// Indica si un consolidado dado está activo o inactivo
        /// </summary>
        /// <returns></returns>
        /// <param name="codigo">Código del consolidado</param>
        public string ObtenerEstadoActivoConsolidado(string codigo)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                InventarioConsolidado_SUM contenedor = contexto.InventarioConsolidado_SUM.FirstOrDefault(inv => inv.INT_Codigo == codigo);
                if (contenedor != null)
                {
                    return contenedor.INT_Estado;
                }
                else
                {
                    throw new FaultException<ControllerException>
                       (new ControllerException
                           (COConstantesModulos.MODULO_SUMINISTROS,
                           EnumTipoErrorSuministros.EX_ERROR_CODIGO_CONSOLIDADO_NO_EXISTE.ToString(),
                           MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_CODIGO_CONSOLIDADO_NO_EXISTE)));
                }
            }
        }
        /// <summary>
        /// Retorna los tipos de consolidado
        /// </summary>
        /// <returns></returns>
        public List<OUTipoConsolidadoDC> ObtenerTiposConsolidado()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                var lista =  contexto.TiposConsolidado_VOPN.GroupBy(tipo => tipo.TIC_IdTipoConsolidado).ToList();

                List<OUTipoConsolidadoDC> retorno = null;

                if (lista != null && lista.Any())
                {
                    retorno = lista.ConvertAll<OUTipoConsolidadoDC>(tipo => new OUTipoConsolidadoDC
                       {
                           Descripcion = tipo.FirstOrDefault().TIC_Descripcion,
                           IdTipoConsolidado = tipo.FirstOrDefault().TIC_IdTipoConsolidado,
                           Detalles = tipo.GroupBy(det => det.TCD_TipoConsolidadoDetalle).ToList().ConvertAll<OUTipoConsolidadoDetalleDC>(det => new OUTipoConsolidadoDetalleDC
                           {
                               Id = det.FirstOrDefault().TCD_TipoConsolidadoDetalle,
                               Descripcion = det.FirstOrDefault().TCD_Descripcion,
                               Colores = det.ToList().ConvertAll(color => new OUColorTipoConsolidadoDetalleDC
                               {
                                   Id = color.TCC_IdTipoConsolDetColor.HasValue ? color.TCC_IdTipoConsolDetColor.Value : (short)0,
                                   Color = color.TCC_Color
                               })
                           })
                       });
                }

                return retorno;
            }
        }

        /// <summary>
        /// Retorna los tamaños de la tula
        /// </summary>
        /// <returns></returns>
        public List<SUTamanoTulaDC> ObtenerTamanosTula()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.TamanoTula_SUM.ToList().ConvertAll(tula => new SUTamanoTulaDC
                {
                    Id = tula.TAT_IdTamanoTula,
                    Descripcion = tula.TAT_Descripcion
                });
            }
        }

        /// <summary>
        /// Retorna los motivos de cambios de un contenedor
        /// </summary>
        /// <returns></returns>
        public List<SUMotivoCambioDC> ObtenerMotivosCambioContenedor()
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.MotivoCambioContenedor_SUM.OrderBy(motivo => motivo.MCC_Descripcion)
                  .ToList()
                  .ConvertAll(motivo => new SUMotivoCambioDC
                  {
                      Id = motivo.MCC_IdMotivoCambio,
                      Descripcion = motivo.MCC_Descripcion
                  });
            }
        }


        // TODO:ID Lista para traer el Inventario de Consolidados (Tulas-Contenedores)
        public IList<SUConsolidadoDC> ObtenerListaConsolidados(IDictionary<string, string> filtro, int indicePagina, int registrosPorPagina)
        {
            string pCodigo;
            filtro.TryGetValue("INT_Codigo", out pCodigo);

            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerListaConsolidados_SUM(indicePagina, registrosPorPagina, pCodigo).ToList()
                  .ConvertAll(obj => new SUConsolidadoDC
                  {
                      Codigo = obj.INT_Codigo,
                      IdCentroServicios = obj.INT_IdCentroServicios,
                      NombreCentroServicios = obj.CES_Nombre,

                      NombreLocalidad = obj.CES_Nombre,
                      Trayecto = obj.INT_Trayecto,

                      TipoConsolidado = new OUTipoConsolidadoDC()
                      {
                          IdTipoConsolidado = obj.TIC_IdTipoConsolidado,
                          Descripcion = obj.TIC_Descripcion
                      }

                  });
            }
        }


        #endregion

        #region Inserciones
        /// <summary>
        /// Registra un nuevo contenedor en la base de datos
        /// </summary>
        /// <param name="contenedor"></param>
        public void RegistrarNuevoContenedor(SUConsolidadoDC contenedor)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                InventarioConsolidado_SUM _contenedor = new InventarioConsolidado_SUM
                {
                    INT_Codigo = contenedor.Codigo,
                    INT_CreadoPor = ControllerContext.Current.Usuario,
                    INT_Estado = ConstantesFramework.ESTADO_ACTIVO,
                    INT_FechaGrabacion = DateTime.Now,
                    INT_IdCentroServicios = contenedor.IdCentroServicios,
                    INT_IdTipoConsolidado = contenedor.TipoConsolidado.IdTipoConsolidado,
                    INT_TipoConsolidadoDetalle = contenedor.TipoConsolidadoDetalle.Id,
                    INT_Trayecto = contenedor.Trayecto,
                    INT_IdLocalidad = contenedor.IdLocalidad
                };

                if (contenedor.TipoConsolidado.IdTipoConsolidado == SUConstantesSuministros.ID_TIPO_CONSOLIDADO_TULA)
                {
                    InventarioConsolidadoTula_SUM contenedorTula = new InventarioConsolidadoTula_SUM
                    {
                        ITT_Color = contenedor.Color.Id,
                        ITT_CreadoPor = ControllerContext.Current.Usuario,
                        ITT_FechaGrabacion = DateTime.Now,
                        ITT_Tamano = contenedor.Tamano.Id
                    };
                    contexto.InventarioConsolidadoTula_SUM.Add(contenedorTula);
                }

                contexto.InventarioConsolidado_SUM.Add(_contenedor);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Registra una modificación de un contendor
        /// </summary>
        /// <param name="consolidado"></param>
        public void RegistrarModificacionContenedor(SUModificacionConsolidadoDC consolidado)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                InventarioConsolidado_SUM contenedor = contexto.InventarioConsolidado_SUM.FirstOrDefault(inv => inv.INT_Codigo == consolidado.CodigoConsolidado);
                if (contenedor != null)
                {
                    InventarioConsolidadoHist_SUM _consolidado = new InventarioConsolidadoHist_SUM
                    {
                        ITH_CreadoPor = ControllerContext.Current.Usuario,
                        ITH_FechaGrabacion = DateTime.Now,
                        ITH_IdInventarioContenedor = contenedor.INT_IdInventarioConsolidado,
                        ITH_IdMotivoCambio = consolidado.MotivoCambio.Id,
                        ITH_Observaciones = consolidado.Observaciones,
                    };
                    // Si el motivo del cambio de contenedor es "Deshabilitar" entonces se debe inactivar el contenedor
                    if (consolidado.MotivoCambio.Id == SUConstantesSuministros.ID_MOTIVO_CAMBIO_CONTENEDOR_DESHABILITAR)
                    {
                        contenedor.INT_Estado = ConstantesFramework.ESTADO_INACTIVO;
                    }
                    else if (consolidado.MotivoCambio.Id == SUConstantesSuministros.ID_MOTIVO_CAMBIO_CONTENEDOR_HABILITAR)
                    {
                        contenedor.INT_Estado = ConstantesFramework.ESTADO_ACTIVO;
                    }
                    contexto.InventarioConsolidadoHist_SUM.Add(_consolidado);
                    contexto.SaveChanges();
                }
                else
                {
                    throw new FaultException<ControllerException>
                           (new ControllerException
                               (COConstantesModulos.MODULO_SUMINISTROS,
                               EnumTipoErrorSuministros.EX_ERROR_CODIGO_CONSOLIDADO_NO_EXISTE.ToString(),
                               MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_CODIGO_CONSOLIDADO_NO_EXISTE)));
                }
            }
        }
        #endregion

        #region Validaciones

        /// <summary>
        /// Método para validar el dueño de un contenedor
        /// </summary>
        /// <param name="asignacion"></param>
        public void ValidarContenedor(OUAsignacionDC asignacion)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                InventarioConsolidado_SUM contenedor =
                      contexto
                      .InventarioConsolidado_SUM
                      .FirstOrDefault
                      (inv => inv.INT_IdTipoConsolidado == asignacion.TipoAsignacion.IdTipoAsignacion
                                          && inv.INT_Codigo == asignacion.NoTula
                      && inv.INT_Estado == ConstantesFramework.ESTADO_ACTIVO);
                if (contenedor == null)
                {
                    throw new FaultException<ControllerException>
                        (new ControllerException
                            (COConstantesModulos.MODULO_SUMINISTROS,
                            EnumTipoErrorSuministros.EX_ERROR_CONTENEDOR_NO_EXISTE.ToString(),
                            MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_CONTENEDOR_NO_EXISTE)));
                }
                else if (contenedor.INT_IdCentroServicios != asignacion.CentroServicioDestino.IdCentroServicio)
                {
                    throw new FaultException<ControllerException>
                           (new ControllerException
                               (COConstantesModulos.MODULO_SUMINISTROS,
                               EnumTipoErrorSuministros.EX_ERROR_CONTENEDOR_CENTROSERVICIOORIGEN.ToString(),
                               MensajesSuministros.CargarMensaje(EnumTipoErrorSuministros.EX_ERROR_CONTENEDOR_CENTROSERVICIOORIGEN)));
                }
            }
        }

        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public bool ValidarConsolidadoActivoCiudadAsignacion(string codigoConsolidado, string idCiudad)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerInventarioActPorCodigoInventarioCiudadAsigna_SUM(codigoConsolidado, idCiudad).Count() > 0 ? true : false;

            }
        }

        /// <summary>
        /// Valida que un consolidado activo y que pertenezca a una ciudad a una ciudad
        /// </summary>
        /// <param name="codigoConsolidado"></param>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public List<SUConsolidadoDC> ObtenerInventarioConsolidado(string codigoConsolidado)
        {
            using (ModeloSuministros contexto = new ModeloSuministros(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.paObtenerInventarioActPorCodigoInventario_SUM(codigoConsolidado).
                    ToList().ConvertAll<SUConsolidadoDC>(s =>
                        new SUConsolidadoDC()
                        {
                            Codigo = s.INT_Codigo,
                            IdCentroServicios = s.INT_IdCentroServicios,
                            IdLocalidad = s.INT_IdLocalidad,
                            NombreLocalidad = s.LOC_Nombre
                        });

            }
        }


        #endregion
    }
}
