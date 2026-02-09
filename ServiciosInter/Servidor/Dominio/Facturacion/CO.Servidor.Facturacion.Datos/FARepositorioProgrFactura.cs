using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Facturacion.Comun;
using CO.Servidor.Facturacion.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.Facturacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using System.Data.Entity;
using System.Transactions;

namespace CO.Servidor.Facturacion.Datos
{
    public class FARepositorioProgrFactura
    {
        #region Atributos

        private static readonly FARepositorioProgrFactura instancia = new FARepositorioProgrFactura();
        private const string NombreModelo = "ModeloFacturacion";

        #endregion Atributos

        #region Propiedades

        /// <summary>
        /// Retorna la instancia de la clase FARepositorio
        /// </summary>
        public static FARepositorioProgrFactura Instancia
        {
            get { return FARepositorioProgrFactura.instancia; }
        }

        #endregion Propiedades

        #region Programaciones

        /// <summary>
        /// Consulta las programaciones que existen ya creadas en el sistema según los criterios de búsqueda establecidos
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="campoOrdenamiento"></param>
        /// <param name="indicePagina"></param>
        /// <param name="registrosPorPagina"></param>
        /// <param name="ordenamientoAscendente"></param>
        /// <param name="totalRegistros"></param>
        /// <returns></returns>
        public IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesFiltro(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ConsultarContainsProgramacionFactura_FAC(filtro, "PRF_FechaGrabacion", out totalRegistros, indicePagina, registrosPorPagina, false)
                .ToList().ConvertAll<FAProgramacionFacturaDC>(prog =>
                {
                    FAProgramacionFacturaDC programacion = new FAProgramacionFacturaDC()
                    {
                        CreadoPor = prog.PRF_CreadoPor,
                        DescAgrupamiento = prog.PRF_DescAgrupamiento,
                        DescContrato = prog.PRF_DescContrato,
                        FechaEjecucion = prog.PRF_FechaEjecucion,
                        FechaGrabacion = prog.PRF_FechaGrabacion,
                        FechaProgramacion = prog.PRF_FechaProgramacion,
                        IdAgrupamiento = prog.PRF_IdAgrupamiento,
                        IdCliente = prog.PRF_IdCliente,
                        IdContrato = prog.PRF_IdContrato,
                        DiaCorte = prog.PRF_DiaCorte,
                        IdProgramacion = prog.PRF_IdProgramacion,
                        IdRacol = prog.PRF_IdRacol,
                        NombreRacol = prog.PRF_NombreRacol,
                        Ejecutado = prog.PRF_Ejecutado,
                        RazonSocialCliente = prog.PRF_RazonSocialCliente,
                        Estado = prog.PRF_Tipo,
                        ConceptosProgramados = new ObservableCollection<FAConceptoProgramadoDC>(contexto.ConceptoProgramado_FAC.Where(conc => conc.COP_IdProgramacion == prog.PRF_IdProgramacion).ToList().ConvertAll<FAConceptoProgramadoDC>(conc =>
                      {
                          return new FAConceptoProgramadoDC()
                          {
                              Cantidad = conc.COP_Cantidad,
                              CreadoPor = conc.COP_CreadoPor,
                              DescripcionConcepto = conc.COP_DescripcionConcepto,
                              Sucursal = new Servicios.ContratoDatos.Clientes.CLSucursalDC 
                              { 
                                  IdSucursal = conc.COP_IdSucursal, 
                                  Nombre = conc.COP_DescripcionSucursal
                              },
                              FechaGrabacion = conc.COP_FechaGrabacion,
                              IdConceptoProg = conc.COP_IdConceptoProg,
                              IdProgramacion = conc.COP_IdProgramacion,
                              Servicio = new Servicios.ContratoDatos.Tarifas.TAServicioDC { IdServicio = conc.COP_IdServicio },
                              ValorUnitario = conc.COP_ValorUnitario,
                              Total = conc.COP_Total,
                              TotalImpuestos = conc.COP_TotalImpuestos,
                              TotalNeto = conc.COP_TotalNeto,
                              Impuestos = new ObservableCollection<FAImpConceptoProgramado>(
                              contexto.ImpConceptoProgramado_FAC.Where(imp => imp.ICP_IdConceptoProg == conc.COP_IdConceptoProg).ToList().ConvertAll<FAImpConceptoProgramado>(imp =>
                                {
                                    return new FAImpConceptoProgramado()
                                    {
                                        BaseCalculo = imp.ICP_BaseCalculo,
                                        CreadoPor = imp.ICP_CreadoPor,
                                        Descripcion = imp.ICP_Descripcion,
                                        FechaGrabacion = imp.ICP_FechaGrabacion,
                                        IdConceptoProg = imp.ICP_IdConceptoProg,
                                        IdImpuesto = imp.ICP_IdImpuesto,
                                        Total = imp.ICP_Total,
                                        ValorPorc = imp.ICP_ValorPorc
                                    };
                                }))
                          };
                      })),
                        ExclusionesProgramacion = new ObservableCollection<FAExclusionProgramacionDC>(contexto.ExclusionesProgramacion_FAC.Where(exc => exc.EXP_IdProgramacion == prog.PRF_IdProgramacion).ToList().ConvertAll<FAExclusionProgramacionDC>(exc =>
                          {
                              return new FAExclusionProgramacionDC()
                              {
                                  CreadoPor = exc.EXP_CreadoPor,
                                  FechaGrabacion = exc.EXP_FechaGrabacion,
                                  IdProgramacion = exc.EXP_IdProgramacion,
                                  IdServicio = exc.EXP_IdServicio,
                                  NumeroOperacion = exc.EXP_NumeroOperacion,
                                  NombreServicio = exc.EXP_NombreServicio,
                                  Valor = exc.EXP_Valor,
                                  FechaOperacion = exc.EXP_FechaOperacion
                              };
                          }))
                    };
                    return programacion;
                });
            }
        }

        /// <summary>
        /// Consulta las programaciones no ejecutadas de un cliente y un rango de fechas
        /// </summary>        
        /// <returns></returns>
        public IEnumerable<FAProgramacionFacturaDC> ConsultarProgramacionesNoEjecutadas(DateTime fechaDesde, DateTime fechaHasta, int idCliente)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                IEnumerable<ProgramacionFactura_FAC> programaciones;
                if (idCliente > 0)
                {
                    programaciones = contexto.ProgramacionFactura_FAC.Where(p => p.PRF_IdCliente == idCliente && p.PRF_FechaProgramacion >= fechaDesde && p.PRF_FechaProgramacion <= fechaHasta && p.PRF_Ejecutado==false).ToList();                      
                }
                else
                    programaciones = contexto.ProgramacionFactura_FAC.Where(p => p.PRF_FechaProgramacion >= fechaDesde && p.PRF_FechaProgramacion <= fechaHasta && p.PRF_Ejecutado == false).ToList();                      

                return programaciones.ToList().ConvertAll<FAProgramacionFacturaDC>(prog =>
                    {
                        FAProgramacionFacturaDC programacion = new FAProgramacionFacturaDC()
                        {
                            IdProgramacion = prog.PRF_IdProgramacion,
                            RazonSocialCliente = prog.PRF_RazonSocialCliente,
                            IdCliente = prog.PRF_IdCliente,
                            DescAgrupamiento = prog.PRF_DescAgrupamiento,
                            FechaProgramacion = prog.PRF_FechaProgramacion,
                            IncluirEnFacturacion = true,
                            ResultadoEjecucion = "Sin Ejecutar"
                        };
                        return programacion;
                    });
            }
        }

        #endregion Programaciones

        #region Exclusión de Movimientos

        /// <summary>
        /// Inserta una nueva exclusión asociada a una programación
        /// </summary>
        /// <param name="exclusion"></param>
        public void ExcluirMovimientoDeProgramacion(FAExclusionProgramacionDC exclusion)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProgramacionFactura_FAC proFac = contexto.ProgramacionFactura_FAC.Where(pr => pr.PRF_IdProgramacion == exclusion.IdProgramacion).FirstOrDefault();

                if (proFac != null)
                    if (proFac.PRF_DiaCorte < exclusion.FechaOperacion.Day)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_FACTURACION, FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_SUPERAFECHA.ToString(), FAMensajesFacturacion.CargarMensaje(FAEnumTipoErrorFacturacion.EX_MOVIMIENTO_SUPERAFECHA)));

                ExclusionesProgramacion_FAC exclusionProgramada = new ExclusionesProgramacion_FAC()
                {
                    EXP_CreadoPor = ControllerContext.Current.Usuario,
                    EXP_FechaGrabacion = exclusion.FechaGrabacion,
                    EXP_IdProgramacion = exclusion.IdProgramacion,
                    EXP_IdServicio = exclusion.IdServicio,
                    EXP_NumeroOperacion = exclusion.NumeroOperacion,
                    EXP_NombreServicio = exclusion.NombreServicio,
                    EXP_Valor = exclusion.Valor,
                    EXP_FechaOperacion = exclusion.FechaOperacion
                };

                contexto.ExclusionesProgramacion_FAC.Add(exclusionProgramada);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Permite deshacer la exclusión de un movimiento ya excluido previamente
        /// </summary>
        /// <param name="exclusion">Exclusión que se desea deshacer</param>
        public void DeshacerExclusionDeMovimiento(FAExclusionProgramacionDC exclusion)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ExclusionesProgramacion_FAC excProg = contexto.ExclusionesProgramacion_FAC.Where(exc => exc.EXP_NumeroOperacion == exclusion.NumeroOperacion && exc.EXP_IdServicio == exclusion.IdServicio).FirstOrDefault();

                if (excProg != null)
                {
                    contexto.ExclusionesProgramacion_FAC.Remove(excProg);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Exclusión de Movimientos

        #region Generar Facturas

        public void GenerarFacturaProgramada(FAProgramacionFacturaDC programacion, long numeroFactura)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew,TimeSpan.FromHours(5)))
            {
                using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
                {                    
                    contexto.paFacturarCliente_FAC(programacion.IdProgramacion, ControllerContext.Current.Usuario, numeroFactura);
                    if (contexto.DetalleXOperFactura_FAC.Where(d => d.DOF_NumeroFactura == numeroFactura).Count() == 0)
                    {
                        scope.Complete();
                        //if(resultadoFac==null || resultadoFac.REF_ValorTotal==0)
                        //if (resumenFac == null || resumenFac.REF_ValorTotal == 0)
                        throw new FaultException<ControllerException>(new ControllerException("FAC", "0", "la factura arrojo 0 movimientos y no se generó asegurese de que el contrato del cliente tenga los agrupamientos de facturación adecuados."));
                    }
                    else
                        scope.Complete();

                }
            }
        }

        #endregion Generar Facturas

        #region Otros Conceptos

        /// <summary>
        /// Agrega un concepto nuevo a una programación de una factura.
        /// </summary>
        /// <param name="concepto"></param>
        public int GuardaConceptoProgramado(FAConceptoProgramadoDC concepto)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConceptoProgramado_FAC conceptoProgramado = new ConceptoProgramado_FAC()
                {
                    COP_Cantidad = concepto.Cantidad,
                    COP_CreadoPor = ControllerContext.Current.Usuario,
                    COP_DescripcionConcepto = concepto.DescripcionConcepto,
                    COP_DescripcionSucursal = concepto.Sucursal.Nombre,
                    COP_FechaGrabacion = DateTime.Now,
                    COP_ValorUnitario = concepto.ValorUnitario,
                    COP_IdProgramacion = concepto.IdProgramacion,
                    COP_IdServicio = concepto.Servicio.IdServicio,
                    COP_IdSucursal = concepto.Sucursal.IdSucursal,
                    COP_Total = concepto.Total,
                    COP_TotalImpuestos = concepto.TotalImpuestos,
                    COP_TotalNeto = concepto.TotalNeto
                };

                concepto.Impuestos.ToList().ForEach(imp => conceptoProgramado.ImpConceptoProgramado_FAC.Add(
                  new ImpConceptoProgramado_FAC()
                  {
                      ICP_BaseCalculo = imp.BaseCalculo,
                      ICP_CreadoPor = ControllerContext.Current.Usuario,
                      ICP_Descripcion = imp.Descripcion,
                      ICP_FechaGrabacion = DateTime.Now,
                      ICP_IdImpuesto = imp.IdImpuesto,
                      ICP_Total = imp.Total,
                      ICP_ValorPorc = imp.ValorPorc
                  }
                  ));

                contexto.ConceptoProgramado_FAC.Add(conceptoProgramado);

                contexto.SaveChanges();

                return conceptoProgramado.COP_IdConceptoProg;
            }
        }

        /// <summary>
        /// Elimina un concepto de una programación
        /// </summary>
        /// <param name="concepto"></param>
        public void EliminarConceptoProgramacion(FAConceptoProgramadoDC concepto)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ConceptoProgramado_FAC conceptoprog = contexto.ConceptoProgramado_FAC.Where(con => con.COP_IdConceptoProg == concepto.IdConceptoProg).FirstOrDefault();

                if (conceptoprog != null)
                {
                    List<ImpConceptoProgramado_FAC> impuestosBorrar = contexto.ImpConceptoProgramado_FAC.Where(imp => imp.ICP_IdConceptoProg == concepto.IdConceptoProg).ToList();
                    foreach (ImpConceptoProgramado_FAC impuestoBorrar in impuestosBorrar)
                    {
                        contexto.ImpConceptoProgramado_FAC.Remove(impuestoBorrar);
                    }

                    contexto.ConceptoProgramado_FAC.Remove(conceptoprog);
                    contexto.SaveChanges();
                }
            }
        }

        #endregion Otros Conceptos

        #region Cambiar Programacion De Factura

        /// <summary>
        /// Cambia la programación de una factura a una nueva fecha
        /// </summary>
        /// <param name="nuevaProgramacion"></param>
        public void CambiarProgramacionFactura(FAProgramacionFacturaDC nuevaProgramacion, DateTime nuevaFecha)
        {
            nuevaProgramacion.FechaProgramacion = nuevaFecha;
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                ProgramacionFactura_FAC programacionFactura = contexto.ProgramacionFactura_FAC.Where(pro => pro.PRF_IdProgramacion == nuevaProgramacion.IdProgramacion).FirstOrDefault();
                if (programacionFactura != null)
                {
                    programacionFactura.PRF_FechaProgramacion = nuevaProgramacion.FechaProgramacion;
                    FAFacturacionAudit.MapearAuditModificarProgramacion(contexto);
                }
                contexto.SaveChanges();
            }
        }

        #endregion Cambiar Programacion De Factura

        #region Consultas

        /// <summary>
        /// Indica si un movimiento de un cliente crédito específico ya se encuentra facturado
        /// </summary>
        /// <param name="idMovimiento">Identificador del movimiento</param>
        /// <param name="idServicio">Servicio al cual se encuentra asociado el movimiento</param>
        /// <returns></returns>
        public bool MovimientoYaFacturado(long idMovimiento, int idServicio)
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                DetalleXOperFactura_FAC detOperFac = contexto.DetalleXOperFactura_FAC.Where(mov => mov.DOF_IdServicio == idServicio && mov.DOF_NumeroOperacion == idMovimiento).FirstOrDefault();
                if (detOperFac != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Consulta el último número facturado en la tabla de resumen factura
        /// </summary>
        /// <returns></returns>
        public long ConsultarNoUltimaFactura()
        {
            using (FacturacionEntities contexto = new FacturacionEntities(COAdministradorConexionesDbFramework.Instancia.ObtenerConnectionString(NombreModelo)))
            {
                return contexto.ResumenFactura_FAC.Max(f => f.REF_NumeroFactura);
            }
        }
        #endregion Consultas
    }
}