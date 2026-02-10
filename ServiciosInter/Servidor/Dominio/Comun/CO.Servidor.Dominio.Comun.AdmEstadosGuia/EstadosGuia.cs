using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionNacional;
using QuickGraph;

namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia
{
    public static class EstadosGuia
    {
        /// <summary>
        /// Contiene el grafo que contiene la información del flujo de los estados
        /// </summary>
        private static AdjacencyGraph<int, Edge<int>> grafo;

        private static void CargarGrafo()
        {
            grafo = new AdjacencyGraph<int, Edge<int>>();
            List<EGEstadoGuia> estadosGuia = EGRepositorio.Instancia.ObtenerEstadosGuia();
            estadosGuia.ForEach(estadoGuia =>
                estadoGuia.Precedesores.ForEach(predecesor =>
                  {
                      if (predecesor.Id > 0)
                          grafo.AddVerticesAndEdge(new Edge<int>(predecesor.Id, estadoGuia.Id));
                  })
              );
        }

        /// <summary>
        /// Valida si se puede cambiar del estado actual de la guía al nuevo estado,
        /// </summary>
        /// <param name="estadoActualGuia">Estado actual de la guía</param>
        /// <param name="nuevoEstadoGuia">Nuevo estado de la guía</param>
        /// <returns>Indica si es válido el cambio de estado</returns>
        public static bool ValidarCambioEstado(ADEnumEstadoGuia estadoActualGuia, ADEnumEstadoGuia nuevoEstadoGuia)
        {
            if (grafo == null)
            {
                lock (typeof(EstadosGuia))
                {
                    if (grafo == null)
                    {
                        CargarGrafo();
                    }
                }
            }
            Edge<int> edge;
            grafo.TryGetEdge((int)estadoActualGuia, (int)nuevoEstadoGuia, out edge);
            return (edge != null);
        }

        /// <summary>
        /// Obtiene los posibles estados siguientes
        /// </summary>
        /// <param name="estadoActualGuia"></param>
        /// <returns></returns>
        public static List<ADEnumEstadoGuia> ObtenerPosiblesEstadosSiguientes(ADEnumEstadoGuia estadoActualGuia)
        {
            if (grafo == null)
            {
                lock (typeof(EstadosGuia))
                {
                    if (grafo == null)
                    {
                        CargarGrafo();
                    }
                }
            }
            IEnumerable<Edge<int>> edges;
            grafo.TryGetOutEdges((int)estadoActualGuia, out edges);

            //grafo.TryGetEdges((int)estadoActualGuia, (int)estadoActualGuia, out edges);
            List<ADEnumEstadoGuia> estadosGuia = new List<ADEnumEstadoGuia>();
            if (edges != null)
            {
                edges.ToList().ForEach(e => estadosGuia.Add((ADEnumEstadoGuia)Enum.Parse(typeof(ADEnumEstadoGuia), e.Target.ToString())));
            }
            return estadosGuia;
        }

        /// <summary>
        /// Obtiene los posibles estados Autorizados para devolver el estado de una Guia
        /// </summary>
        /// <param name="ObtenerEstadosParaDevolver"></param>
        /// <returns></returns>
        public static List<ADEstadoGuia> ObtenerEstadosParaDevolver()
        {
            return EGRepositorio.Instancia.ObtenerEstadosParaDevolver();
        }

        /// <summary>
        /// Retorna el ultimo estado de una guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public static ADEnumEstadoGuia ObtenerUltimoEstado(long idAdmision)
        {
            return EGRepositorio.Instancia.ObtenerUltimoEstado(idAdmision);
        }

        /// <summary>
        /// Retorna el ultimo estado de una guia por el numero
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public static ADEnumEstadoGuia ObtenerUltimoEstadoxNumero(long numeroGuia)
        {
            return EGRepositorio.Instancia.ObtenerUltimoEstadoxNumero(numeroGuia);
        }

        /// <summary>
        /// Retorna el ultimo estado de una guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public static bool EnCentrodeAcopio_Automatico(long idAdmision)
        {
            return EGRepositorio.Instancia.EnCentrodeAcopio_Automatico(idAdmision);
        }


        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        public static List<ADTrazaGuia> ObtenerEstadosGuia(long numeroGuia)
        {
            return EGRepositorio.Instancia.ObtenerEstadosGuia(numeroGuia);
        }



                /// <summary>
        /// Obtiene los Estados y Motivos de la Guia seleccionada
        /// </summary>
        /// <returns></returns>
        public static List<ADEstadoGuiaMotivoDC> ObtenerEstadosMotivosGuia(long numeroGuia)
        {
            return EGRepositorio.Instancia.ObtenerEstadosMotivosGuia(numeroGuia);
        }

        /// <summary>
        /// Obtiene los estados  de una guia en una localidad
        /// </summary>
        /// <returns></returns>
        public static List<ADTrazaGuia> ObtenerEstadosGuiaxIdAdmision(long idAdmision)
        {
            return EGRepositorio.Instancia.ObtenerEstadosGuiaxIdAdmision(idAdmision);
        }
        /// <summary>
        /// Guarda la traza de la guia ingresada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static long InsertaEstadoGuia(ADTrazaGuia guia)
        {
            return EGRepositorio.Instancia.InsertaEstadoGuia(guia);
        }


        /// <summary>
        /// Guarda la traza de la guia ingresada en un centro de servicio especifico
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static long InsertaEstadoGuiaCentroServicio(ADTrazaGuia guia)
        {
            return EGRepositorio.Instancia.InsertaEstadoGuiaCentroServicio(guia);
        }

        /// <summary>
        /// Guarda la traza de la guia ingresada en un centro de servicio especifico
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static long InsertaEstadoGuiaFecha(ADTrazaGuia guia)
        {
            return EGRepositorio.Instancia.InsertaEstadoGuiaFecha(guia);
        }



        
        /// <summary>
        /// Guarda el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static void InsertaEstadoGuiaMotivo(ADEstadoGuiaMotivoDC guia)
        {
            EGRepositorio.Instancia.InsertaEstadoGuiaMotivo(guia);
        }


        /// <summary>
        /// Guarda el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static void InsertaEstadoGuiaMotivoFecha(ADEstadoGuiaMotivoDC guia)
        {
            EGRepositorio.Instancia.InsertaEstadoGuiaMotivoFecha(guia);
        }


        /// <summary>
        /// Actualiza el estado motivo de una guía al realizar un cambio de estado
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static void ActualizarEstadoGuiaMotivo(ADEstadoGuiaMotivoDC guia)
        {
            EGRepositorio.Instancia.ActualizarEstadoGuiaMotivo(guia);
        }


        /// <summary>
        /// Valida y guarda la traza de la guia ingresada
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static long ValidarInsertarEstadoGuia(ADTrazaGuia guia)
        {
            if (grafo == null)
            {
                lock (typeof(EstadosGuia))
                {
                    if (grafo == null)
                    {
                        CargarGrafo();
                    }
                }
            }
            Edge<int> edge;
            grafo.TryGetEdge((int)guia.IdEstadoGuia, (int)guia.IdNuevoEstadoGuia, out edge);
            if (edge != null)
            {
                return EGRepositorio.Instancia.InsertaEstadoGuia(guia);
            }
            else if (guia.NumeroPieza > 0 && guia.TotalPiezas > 0)
            {
                // Indica que es un rótulo, por tanto ignore el cambio de estado
                return -1;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Valida y guarda la traza de la guia ingresada con fecha
        /// </summary>
        /// <param name="guia"></param>
        /// <param name="estadoGuia"></param>
        public static long ValidarInsertarEstadoGuiaFecha(ADTrazaGuia guia)
        {
            if (grafo == null)
            {
                lock (typeof(EstadosGuia))
                {
                    if (grafo == null)
                    {
                        CargarGrafo();
                    }
                }
            }
            Edge<int> edge;
            grafo.TryGetEdge((int)guia.IdEstadoGuia, (int)guia.IdNuevoEstadoGuia, out edge);
            if (edge != null)
            {
                return EGRepositorio.Instancia.InsertaEstadoGuiaFecha(guia);
            }
            else if (guia.NumeroPieza > 0 && guia.TotalPiezas > 0)
            {
                // Indica que es un rótulo, por tanto ignore el cambio de estado
                return -1;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Valida si una guia esta dentro de un centro de acopio
        /// </summary>
        /// <param name="consolidado"></param>
        /// <returns></returns>
        public static bool ValidarGuiaIngresoCentroAcopio(long numeroGuia, string idLocalidadManifiesta)
        {
            bool GuiaConIngreso = false;
            List<ADTrazaGuia> traza = EGRepositorio.Instancia.ObtenerTrazaGuiasLocalidad(numeroGuia, idLocalidadManifiesta).ToList();
            if (traza.Count <= 0)
                return false;

            List<ADTrazaGuia> trazaGuia = new List<ADTrazaGuia>();

            foreach (ADTrazaGuia t in traza)
            {
                if (t.IdEstadoGuia == (short)ADEnumEstadoGuia.CentroAcopio)
                {
                    GuiaConIngreso = true;
                    break;
                }
                trazaGuia.Add(t);
            };
            trazaGuia = trazaGuia.OrderByDescending(t => t.FechaGrabacion).ToList();

            foreach (ADTrazaGuia t in trazaGuia)
            {
                if (t.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoNacional || t.IdEstadoGuia == (short)ADEnumEstadoGuia.TransitoRegional || t.IdEstadoGuia == (short)ADEnumEstadoGuia.ReclameEnOficina || t.IdEstadoGuia == (short)ADEnumEstadoGuia.EnReparto)
                {
                    GuiaConIngreso = false;
                    break;
                }
            };

            return GuiaConIngreso;
        }

     

        /// <summary>
        /// Método para insertar un registro de estado guía relacionado a un impreso
        /// </summary>
        /// <param name="trazaImpreso"></param>
        public static void InsertarEstadoGuiaImpreso(ADTrazaGuiaImpresoDC trazaImpreso)
        {
            EGRepositorio.Instancia.InsertarEstadoGuiaImpreso(trazaImpreso);
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public static ADTrazaGuia ObtenerTrazaUltimoEstadoGuia(long idAdmision)
        {
            return EGRepositorio.Instancia.ObtenerTrazaUltimoEstadoGuia(idAdmision);
        }

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        //public static ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuia(long numeroGuia)
        //{
        //  return EGRepositorio.Instancia.ObtenerTrazaUltimoEstadoXNumGuia(numeroGuia);
        //}


        /// <summary>
        /// Método para validar si una guia paso por un estado especifico
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <param name="estadoGuia"></param>
        /// <returns></returns>
        public static bool ValidarEstadoGuia(long numeroGuia, short estadoGuia)
        {
            return EGRepositorio.Instancia.ValidarEstadoGuia(numeroGuia, estadoGuia);
        }



        public static void CambiarDevolverEstadoGuia(long IdNumeroGuia, long IdEstado, string pObservaciones, string Usuario)
        {
            EGRepositorio.Instancia.CambiarDevolverEstadoGuia(IdNumeroGuia,  IdEstado, pObservaciones, Usuario);
        }

        /// <summary>
        /// Consulta el estado gestion devolucion o entrega y segun el caso adiciona el motivo
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public static ADTrazaGuia ObtenerEstadoGestion(long numeroGuia)
        {
            return EGRepositorio.Instancia.ObtenerEstadoGestion(numeroGuia);
        }
        /// <summary>
        /// onsulta el motivo de devolucion de acuerdo a el log
        /// </summary>
        /// <param name="idTrazaGuia"></param>
        /// <returns></returns>
        public static ADMotivoGuiaDC ObtenerMotivoGestion(long idTrazaGuia)
        {
            return EGRepositorio.Instancia.ObtenerMotivoGestion(idTrazaGuia);
        }


                /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public static ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuia(long numeroGuia)
        {
            return EGRepositorio.Instancia.ObtenerTrazaUltimoEstadoXNumGuia(numeroGuia);
        }

        public static ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuiaSispostal(long idGuia)
        {
            return EGRepositorio.Instancia.ObtenerTrazaUltimoEstadoXNumGuiaSispostal(idGuia);
        }

        #region Sispostal

        /// <summary>
        /// Obtiene la traza del ultimo estado de la guia x numero de guia
        /// </summary>
        /// <param name="idAdmision"></param>
        /// <returns></returns>
        public static ADTrazaGuia ObtenerTrazaUltimoEstadoXNumGuiaMasivos(long numeroGuia)
        {
            return EGRepositorio.Instancia.ObtenerTrazaUltimoEstadoXNumGuiaMasivos(numeroGuia);
        }

        /// <summary>
        /// Entrega Guia Masivos
        /// </summary>
        /// <param name="guia"></param>
        public static void ActualizarEntregadoGuiaMasivos(ADTrazaGuia guia, bool intentoEntrega)
        {
            EGRepositorio.Instancia.ActualizarEntregadoGuiaMasivos(guia, intentoEntrega);
        }

        /// <summary>
        /// Devolucion Guia Masivos
        /// </summary>
        /// <param name="guia"></param>
        public static void DevolucionGuiaMasivos(ADTrazaGuia guia)
        {
            EGRepositorio.Instancia.DevolucionGuiaMasivos(guia);
        }

        #endregion
    }
}