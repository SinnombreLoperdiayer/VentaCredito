using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Tarifas
{
    /// <summary>
    /// Clase para el manejo de trayectos
    /// </summary>
    internal class TATrayecto : ControllerBase
    {
        private static readonly TATrayecto instancia = (TATrayecto)FabricaInterceptores.GetProxy(new TATrayecto(), COConstantesModulos.TARIFAS);

        public static TATrayecto Instancia
        {
            get { return instancia; }
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio destino</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio)
        {
            if (servicio.IdServicio != CO.Servidor.Dominio.Comun.Tarifas.TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                return TARepositorio.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna los tiempos para la digitalizacion y archivo de una guia despues de entregada
        /// </summary>
        /// <param name="idCiudadOrigen"></param>
        /// <param name="idCiudadDestino"></param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ObtenerTiempoDigitalizacionArchivo(string idCiudadOrigen, string idCiudadDestino)
        {
            return TARepositorio.Instancia.ObtenerTiempoDigitalizacionArchivo(idCiudadOrigen,idCiudadDestino);
        }

        /// <summary>
        /// Retorna Validacion si el Servicio-Origen-Destino, debe etiquetarse como AEREO en el campo del casillero de la Guia
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool ValidarServicioTrayectoCasilleroAereo(string municipioOrigen, string municipioDestino, int idServicio)
        {
            return TARepositorio.Instancia.ValidarServicioTrayectoCasilleroAereo(municipioOrigen, municipioDestino, idServicio);
        }

        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            return TARepositorio.Instancia.ObtenerHorarioRecogidaDeCsv(idCentroServicio);
        }


        /// <summary>
        /// Retorna la lista de horario de determinada sucursal para cliente credito
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeSucursal(int idSucursal)
        {
            return TARepositorio.Instancia.ObtenerHorarioRecogidaDeSucursal(idSucursal);
        }

        /// <summary>
        /// Valida trayecto para la sucursal dada y calcula duración en días del trayecto y el valor de la prima de seguro para clientes crédito
        /// </summary>
        /// <param name="municipioOrigen">Municipio de origen del trayecto</param>
        /// <param name="municipioDestino">Municipio de origen del trayecto</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="idListaPrecios">Identificador de la lista de precios</param>
        /// <returns></returns>
        public ADValidacionServicioTrayectoDestino ValidarServicioTrayectoDestinoCliente(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, int idListaPrecios, int idServicio, int idSucursal)
        {
            ADValidacionServicioTrayectoDestino validacion = TARepositorio.Instancia.ValidarServicioTrayectoDestinoCliente(municipioOrigen, municipioDestino, servicio, idListaPrecios, idServicio);

            return validacion;
        }

        /// <summary>
        /// Obtiene los trayectos
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>Listado de Tipos de impuesto</returns>
        public IEnumerable<TATrayectoDC> ObtenerTrayectos(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros)
        {
            return TARepositorio.Instancia.ObtenerTrayectos(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros);
        }

        /// <summary>
        /// Guarda trayectos
        /// </summary>
        /// <param name="consolidadoTrayectos">Colección de trayectos</param>
        public void GuardarTrayectos(ObservableCollection<TATrayectoDC> consolidadoTrayectos)
        {
            consolidadoTrayectos.ToList().ForEach(trayecto =>
              {
                  if (trayecto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                  {
                      if (trayecto.Replica == false)
                      {
                          using (TransactionScope transaccion = new TransactionScope())
                          {
                              TARepositorio.Instancia.AdicionarTrayecto(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == false)
                                  TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayecto);

                              transaccion.Complete();
                          }
                      }
                      else if (trayecto.Replica == true)
                      {
                          using (TransactionScope transaccion = new TransactionScope())
                          {
                              TARepositorio.Instancia.AdicionarTrayecto(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == false)
                                  TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadDestino, trayecto.IdLocalidadOrigen) == false)
                              {
                                  TATrayectoDC trayectoReplica = new TATrayectoDC()
                                  {
                                      Trayecto = trayecto.Trayecto,
                                      IdLocalidadOrigen = trayecto.IdLocalidadDestino,
                                      IdLocalidadDestino = trayecto.IdLocalidadOrigen,
                                      Servicios = trayecto.Servicios
                                  };

                                  TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayectoReplica);
                              }

                              AdicionarReplica(trayecto);

                              transaccion.Complete();
                          }
                      }
                  }
                  else if (trayecto.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
                  {
                      if (trayecto.Replica == false)
                      {
                          using (TransactionScope transaccion = new TransactionScope())
                          {
                              TARepositorio.Instancia.EditarTrayecto(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == false)
                                  TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == true)
                                  TARepositorio.Instancia.EditarTrayectoKiloInicial(trayecto);

                              transaccion.Complete();
                          }
                      }
                      else if (trayecto.Replica == true)
                      {
                          using (TransactionScope transaccion = new TransactionScope())
                          {
                              TARepositorio.Instancia.EditarTrayecto(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == false)
                                  TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadDestino, trayecto.IdLocalidadOrigen) == false)
                              {
                                  TATrayectoDC trayectoReplica = new TATrayectoDC()
                                  {
                                      Trayecto = trayecto.Trayecto,
                                      IdLocalidadOrigen = trayecto.IdLocalidadDestino,
                                      IdLocalidadDestino = trayecto.IdLocalidadOrigen,
                                      Servicios = trayecto.Servicios
                                  };

                                  TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayectoReplica);
                              }

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == true)
                                  TARepositorio.Instancia.EditarTrayectoKiloInicial(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadDestino, trayecto.IdLocalidadOrigen) == true)
                              {
                                  TATrayectoDC trayectoReplica = new TATrayectoDC()
                                  {
                                      Trayecto = trayecto.Trayecto,
                                      IdLocalidadOrigen = trayecto.IdLocalidadDestino,
                                      IdLocalidadDestino = trayecto.IdLocalidadOrigen,
                                      Servicios = trayecto.Servicios
                                  };

                                  TARepositorio.Instancia.EditarTrayectoKiloInicial(trayectoReplica);
                              }

                              AdicionarReplica(trayecto);

                              transaccion.Complete();
                          }
                      }
                  }
                  else if (trayecto.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                      if (trayecto.Replica == false)
                      {
                          using (TransactionScope transaccion = new TransactionScope())
                          {
                              TARepositorio.Instancia.EliminarTrayecto(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == true)
                                  TARepositorio.Instancia.EliminarTrayectoKiloInicial(trayecto);

                              transaccion.Complete();
                          }
                      }
                      else if (trayecto.Replica == true)
                      {
                          using (TransactionScope transaccion = new TransactionScope())
                          {
                              TARepositorio.Instancia.EliminarTrayecto(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == true)
                                  TARepositorio.Instancia.EliminarTrayectoKiloInicial(trayecto);

                              if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadDestino, trayecto.IdLocalidadOrigen) == true)
                              {
                                  TATrayectoDC trayectoReplica = new TATrayectoDC()
                                  {
                                      Trayecto = trayecto.Trayecto,
                                      IdLocalidadOrigen = trayecto.IdLocalidadDestino,
                                      IdLocalidadDestino = trayecto.IdLocalidadOrigen,
                                      Servicios = trayecto.Servicios
                                  };

                                  TARepositorio.Instancia.EliminarTrayectoKiloInicial(trayectoReplica);
                              }

                              EliminarReplica(trayecto);

                              transaccion.Complete();
                          }
                      }
              });
        }

        /// <summary>
        /// Adiciona el recíproco de un trayecto
        /// </summary>
        /// <param name="trayecto">Objeto trayecto</param>
        private void AdicionarReplica(TATrayectoDC trayecto)
        {
            string idLocalidadOrigenReplica = trayecto.IdLocalidadDestino;
            string idLocalidadDestinoReplica = trayecto.IdLocalidadOrigen;

            if (TARepositorio.Instancia.ValidarTrayectoSubTrayecto(idLocalidadOrigenReplica, idLocalidadDestinoReplica) == true && trayecto.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
                throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES_RECIPROCO.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES_RECIPROCO)));

            bool trayectoExiste = TARepositorio.Instancia.ValidarTrayectoSubTrayecto(idLocalidadOrigenReplica, idLocalidadDestinoReplica);

            if (trayectoExiste == false)
            {
                trayecto.IdLocalidadOrigen = idLocalidadOrigenReplica;
                trayecto.IdLocalidadDestino = idLocalidadDestinoReplica;
                //Se comenta para que registre los mismo servicios del
                //trayecto inicial Rafram 22/08/2012
                //trayecto.Servicios.ToList().ForEach(s =>
                //  {
                //    s.Asignado = true;
                //    s.TiempoEntrega = 0;
                //  });

                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (TARepositorio.Instancia.ValidarTrayectoKiloInicial(trayecto.IdLocalidadOrigen, trayecto.IdLocalidadDestino) == false)
                        TARepositorio.Instancia.AdicionarTrayectoKiloInicial(trayecto);

                    TARepositorio.Instancia.AdicionarTrayecto(trayecto);

                    transaccion.Complete();
                }
            }
            else if (trayectoExiste == true)
            {
                trayecto.IdTrayecto = TARepositorio.Instancia.ObtenerIdentificadorTrayecto(idLocalidadOrigenReplica, idLocalidadDestinoReplica);
                TARepositorio.Instancia.EditarTrayecto(trayecto);
            }
        }

        /// <summary>
        /// Elimina el recíproco de un trayecto
        /// </summary>
        /// <param name="trayecto">Objeto trayecto</param>
        private void EliminarReplica(TATrayectoDC trayecto)
        {
            string idLocalidadOrigenReplica = trayecto.IdLocalidadDestino;
            string idLocalidadDestinoReplica = trayecto.IdLocalidadOrigen;

            bool trayectoExiste = TARepositorio.Instancia.ValidarTrayectoSubTrayecto(idLocalidadOrigenReplica, idLocalidadDestinoReplica);

            if (trayectoExiste == true)
            {
                trayecto.IdTrayecto = TARepositorio.Instancia.ObtenerIdentificadorTrayecto(idLocalidadOrigenReplica, idLocalidadDestinoReplica);
                trayecto.IdLocalidadOrigen = idLocalidadOrigenReplica;
                trayecto.IdLocalidadDestino = idLocalidadDestinoReplica;

                TARepositorio.Instancia.EliminarTrayecto(trayecto);
            }
        }

        /// <summary>
        /// Valida si existe un trayecto para una ciudad de origen y un servicio
        /// </summary>
        public void ValidarServicioTrayectoOrigen(string idLocalidadOrigen, int idServicio)
        {
            TARepositorio.Instancia.ValidarServicioTrayectoOrigen(idLocalidadOrigen, idServicio);
        }

        /// <summary>
        /// Obtiene excepciones de precio trayecto subtrayecto
        /// </summary>
        /// <param name="filtro">Filtro</param>
        /// <param name="campoOrdenamiento">Campo de Ordenamiento</param>
        /// <param name="indicePagina">Indice de página</param>
        /// <param name="registrosPorPagina">Registro por página</param>
        /// <param name="ordenamientoAscendente">Ordenamiento</param>
        /// <param name="totalRegistros">Total de Registros</param>
        /// <returns>excepción trayecto subtrayecto</returns>
        public IEnumerable<TATrayectoExcepcionDC> ObtenerExcepcionTrayectoSubTrayecto(IDictionary<string, string> filtro, string campoOrdenamiento, int indicePagina, int registrosPorPagina, bool ordenamientoAscendente, out int totalRegistros, int idListaPrecio)
        {
            return TARepositorio.Instancia.ObtenerExcepcionTrayectoSubTrayecto(filtro, campoOrdenamiento, indicePagina, registrosPorPagina, ordenamientoAscendente, out totalRegistros, idListaPrecio);
        }

        /// <summary>
        /// Metodo para guardar los cambios de una excepción a un trayecto subtrayecto
        /// </summary>
        /// <param name="excepcion"></param>
        public void GuardarCambiosExcepcionTrayectoSubTrayecto(TATrayectoExcepcionDC excepcion)
        {
            if (excepcion.EstadoRegistro == EnumEstadoRegistro.ADICIONADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (TARepositorio.Instancia.ValidarExcepcionTrayectoSubTrayecto(excepcion.IdLocalidadOrigen, excepcion.IdLocalidadDestino, excepcion.IdListaPrecio) == true)
                        throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.TARIFAS, TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES.ToString(), TAMensajesTarifas.CargarMensaje(TAEnumTipoErrorTarifas.EX_TRAYECTO_SUBTRAYECTO_EXISTE_LOCALIDADES)));

                    if (TARepositorio.Instancia.ValidarExcepcionTrayectoKiloInicial(excepcion.IdLocalidadOrigen, excepcion.IdLocalidadDestino, excepcion.IdListaPrecio) == false)
                        TARepositorio.Instancia.AdicionarExcepcionTrayectoKiloInicial(excepcion);

                    excepcion.IdTrayectoSubTrayectoExcepcion = TARepositorio.Instancia.AdicionarExcepcionTrayectoSubTrayecto(excepcion);

                    excepcion.Servicios.ToList().ForEach(g =>
                      {
                          g.IdTrayectoSubTrayectoExcepcion = excepcion.IdTrayectoSubTrayectoExcepcion;
                          TARepositorio.Instancia.AdicionarServicioExcepcion(g);
                      });
                    transaccion.Complete();
                }
            }

            if (excepcion.EstadoRegistro == EnumEstadoRegistro.MODIFICADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    if (TARepositorio.Instancia.ValidarExcepcionTrayectoKiloInicial(excepcion.IdLocalidadOrigen, excepcion.IdLocalidadDestino, excepcion.IdListaPrecio) == false)
                        TARepositorio.Instancia.AdicionarExcepcionTrayectoKiloInicial(excepcion);

                    excepcion.Servicios.ToList().ForEach(g =>
                    {
                        g.IdTrayectoSubTrayectoExcepcion = excepcion.IdTrayectoSubTrayectoExcepcion;
                        if (g.EstadoRegistro == EnumEstadoRegistro.ADICIONADO && g.TiempoEntrega > 0)
                            TARepositorio.Instancia.AdicionarServicioExcepcion(g);
                        if (g.EstadoRegistro == EnumEstadoRegistro.MODIFICADO && g.TiempoEntrega > 0)
                            TARepositorio.Instancia.ModificarServicioExcepcion(g);
                        if (g.EstadoRegistro == EnumEstadoRegistro.BORRADO)
                            TARepositorio.Instancia.EliminarServicioExcepcion(g);
                    });
                    transaccion.Complete();
                }
            }
            if (excepcion.EstadoRegistro == EnumEstadoRegistro.BORRADO)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    TARepositorio.Instancia.EliminarServiciosExcepcion(excepcion);
                    TARepositorio.Instancia.EliminarExcepcionTrayectoSubTrayecto(excepcion);
                    transaccion.Complete();
                }
            }
        }

        /// <summary>
        /// Metodo para obtener los servicios por excepcion
        /// </summary>
        /// <param name="excepcion"></param>
        /// <returns></returns>
        public IEnumerable<TATrayectoExcepcionServicioDC> ObtenerServiciosPorExcepcion(TATrayectoExcepcionDC excepcion)
        {
            return TARepositorio.Instancia.ObtenerServiciosPorExcepcion(excepcion);
        }

        #region Komprech

        /// <summary>
        /// retorna el numero de Dias de entrega entre bogota y la
        /// localidad de destino para Komprech
        /// </summary>
        /// <param name="idLocalidadDestino">id localidad destino</param>
        /// <returns>numero de horas en entregar</returns>
        public int ObtenerTiempoDiasEntregaKomprech(string idLocalidadDestino)
        {
            return TARepositorio.Instancia.ObtenerTiempoDiasEntregaKomprech(idLocalidadDestino);
        }

        #endregion Komprech
    }
}