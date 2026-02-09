using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Rutas.Optimizacion;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Servicios;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;

namespace CO.Servidor.Tarifas
{
    public class TAFachadaKomprech
    {
        /// <summary>
        /// Instancia Singleton
        /// </summary>
        private static readonly TAFachadaKomprech instancia = new TAFachadaKomprech();

        /// <summary>
        /// Retorna una instancia de la fabrica de Dominio
        /// </summary>
        public static TAFachadaKomprech Instancia
        {
            get { return TAFachadaKomprech.instancia; }
        }

        /// <summary>
        /// Calcula el precio del envío para komprech, valida el trayecto y retorna el número de horas que tarda el envío
        /// </summary>
        /// <param name="idServicio"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="peso"></param>
        /// <param name="valorDeclarado"></param>
        /// <returns></returns>
        public TAPrecioTarifaMensajeria CalcularTarifaKomprech(int idServicio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado)
        {
            string idListaPrecioStr = TAAdministradorTarifas.Instancia.ObtenerParametro("IdListPrecioKomprech");
            int idListaPrecio = 1;
            if (idListaPrecioStr != null)
            {
                if (!int.TryParse(idListaPrecioStr, out idListaPrecio))
                {
                    idListaPrecio = 1;
                }
            }
            else
            {
                throw new Exception(Comun.TAMensajesTarifas.CargarMensaje(Comun.TAEnumTipoErrorTarifas.EX_LISTA_PRECIOS_KOMPRECH_INVALIDA));
            }

          // Cálculo de la tarifa
            TAPrecioMensajeriaDC precio = null;
            TAPrecioCargaDC precioCarga = null;
            if (valorDeclarado == 0 && peso == 0)
            {
              precio = new TAPrecioMensajeriaDC
              {
                 Valor = 0, ValorPrimaSeguro = 0, ValorKiloAdicional = 0, ValorKiloInicial = 0
              };
            }
            else
            {
              switch (idServicio)
              {
                case TAConstantesServicios.SERVICIO_MENSAJERIA:
                  precio = TAServicioMensajeria.Instancia.CalcularPrecio(idServicio,
                            idListaPrecio,
                            idLocalidadOrigen,
                            idLocalidadDestino,
                            peso,
                            valorDeclarado,
                            true);
                  break;

                case TAConstantesServicios.SERVICIO_CARGA_EXPRESS:
                  precio = TAServicioCargaExpress.Instancia.CalcularPrecio(idServicio,
                            idListaPrecio,
                            idLocalidadOrigen,
                            idLocalidadDestino,
                            peso,
                            valorDeclarado,
                            true);
                  break;

                case TAConstantesServicios.SERVICIO_RAPI_CARGA:
                  precioCarga = TAServicioRapiCarga.Instancia.CalcularPrecio(idServicio,
                            idListaPrecio,
                            idLocalidadOrigen,
                            idLocalidadDestino,
                            peso,
                            valorDeclarado,
                            true);
                  break;

                default:
                  throw new Exception(Comun.TAMensajesTarifas.CargarMensaje(Comun.TAEnumTipoErrorTarifas.EX_SERVICIO_NO_VALIDO));
              }
            }

            TATrayecto.Instancia.ValidarServicioTrayectoDestino(new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC
            {
                IdLocalidad = idLocalidadOrigen
            }, new Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC
            {
                IdLocalidad = idLocalidadDestino
            }, new Servidor.Servicios.ContratoDatos.Tarifas.TAServicioDC
                                {
                                    IdServicio = idServicio
                                });

            int numeroDias = ObtenerTiempoDiasEntregaKomprech(idLocalidadDestino);

            
      //      numeroDias = 0;

            int numeroHoras = 0;

            numeroHoras = numeroDias * 24;
            DateTime horaEntrega = DateTime.Now.AddHours(numeroHoras);
            if (horaEntrega.Hour < 18)
            {
                numeroHoras += 18 - horaEntrega.Hour;
            }
            else
            {
                numeroHoras += 24 - (horaEntrega.Hour - 18);
            }

            return new TAPrecioTarifaMensajeria
            {
                TiempoEntregaHoras = numeroHoras,
                ValorKiloAdicional = precio != null ? precio.ValorKiloAdicional : precioCarga.ValorKiloAdicional,
                ValorKiloInicial = precio != null ? precio.ValorKiloInicial : 0,
                ValorPrimaSeguro = precio != null ? precio.ValorPrimaSeguro : precioCarga.ValorPrimaSeguro,
                ValorTotal = precio != null ? precio.Valor : precioCarga.Valor
            };
        }

        /// <summary>
        /// retorna el tiempo de entrega entre bogota y la
        /// localidad de destino para Komprech
        /// </summary>
        /// <param name="idLocalidadDestino">id localidad destino</param>
        /// <returns>numero de horas en entregar</returns>
        public int ObtenerTiempoDiasEntregaKomprech(string idLocalidadDestino)
        {
          int diasEntrega = 0;
          try
          {
            diasEntrega = TATrayecto.Instancia.ObtenerTiempoDiasEntregaKomprech(idLocalidadDestino);
          }
          catch (FaultException<ControllerException>)
          {
            diasEntrega = 0;
          }
          return diasEntrega;
        }
    }
}