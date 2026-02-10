using System.Collections.Generic;
using System.ServiceModel;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios;
using CO.Servidor.Tarifas.Comun;
using CO.Servidor.Tarifas.Datos;
using CO.Servidor.Tarifas.Servicios;
using Framework.Servidor.Comun;
using System.Linq;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft;

namespace CO.Servidor.Tarifas
{
    /// <summary>
    /// Fachada para los servicios de dominio de tarifas
    /// </summary>
    public class TAFachadaTarifas : ITAFachadaTarifas
    {
        /// <summary>
        /// Metodo encargado de devolver el id de la lista de precios vigente
        /// </summary>
        /// <returns>int con el id de la lista de precio</returns>
        public int ObtenerIdListaPrecioVigente()
        {
            return TAListaPrecios.Instancia.ObtenerIdListaPrecioVigente();
        }

        /// <summary>
        /// Metodo utilizado para conocer la lista de precios para determinado cliente credito
        /// </summary>
        /// <param name="IdClienteCredito"></param>
        /// <returns></returns>
        public int ObtenerIdListaPrecioClienteCredito(int IdClienteCredito)
        {
            return TAListaPrecios.Instancia.ObtenerIdListaPrecioClienteCredito(IdClienteCredito);
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio destino</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioOrigen, Framework.Servidor.Servicios.ContratoDatos.Parametros.PALocalidadDC municipioDestino, TAServicioDC servicio)
        {
            return TATrayecto.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio);
        }

        /// <summary>
        /// Retorna los tiempos para la digitalizacion y archivo de una guia despues de entregada
        /// </summary>
        /// <param name="idCiudadOrigen"></param>
        /// <param name="idCiudadDestino"></param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ObtenerTiempoDigitalizacionArchivo(string idCiudadOrigen, string idCiudadDestino)
        {
            return TATrayecto.Instancia.ObtenerTiempoDigitalizacionArchivo(idCiudadOrigen,idCiudadDestino);
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
            return TATrayecto.Instancia.ValidarServicioTrayectoCasilleroAereo(municipioOrigen, municipioDestino, idServicio);
        }


        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            return TATrayecto.Instancia.ObtenerHorarioRecogidaDeCsv(idCentroServicio);
        }

        /// <summary>
        /// Retorna la lista de horario de determinada sucursal para cliente credito
        /// </summary>
        /// <param name="idSucursal"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeSucursal(int idSucursal)
        {
            return TATrayecto.Instancia.ObtenerHorarioRecogidaDeSucursal(idSucursal);
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
            return TATrayecto.Instancia.ValidarServicioTrayectoDestinoCliente(municipioOrigen, municipioDestino, servicio, idListaPrecios, idServicio, idSucursal);
        }

        /// <summary>
        /// Retorna el precio del servicio
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Colección con precios</returns>
        public TAPrecioCentroCorrespondenciaDC CalcularPrecioCentroCorrespondencia(int idListaPrecios)
        {
            return TAServicioCentroCorrespondencia.Instancia.CalcularPrecio(idListaPrecios);
        }

        /// <summary>
        /// Calcular el precio para una tarifa internacional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valor Precio</returns>
        public TAPrecioServicioDC CalcularPrecioInternacional(int idListaPrecios, int tipoEmpaque, string idLocalidadDestino, decimal peso, string idZona, decimal valorDeclarado)
        {
            return TAServicioInternacional.Instancia.CalcularPrecio(idListaPrecios, tipoEmpaque, idLocalidadDestino, peso, idZona, valorDeclarado);
        }

        /// <summary>
        /// Obtiene el precio del servicio trámites
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioTramiteDC CalcularPrecioTramites(int idListaPrecios, int idTramite)
        {
            return TAServicioTramites.Instancia.CalcularPrecio(idListaPrecios, idTramite);
        }


        /// <summary>
        /// Obtiene los pesos mínimo y máximo de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador Servicio</param>
        /// <returns>Objeto con los pesos</returns>
        public TAServicioPesoDC ObtenerServicioPeso(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerServicioPeso(idServicio);
        }


        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeria(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioMensajeria.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioMensajeriaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAAdministradorTarifasCredito.Instancia.ObtenerPrecioMensajeriaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio mensajeria
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioCargaCredito(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAAdministradorTarifasCredito.Instancia.ObtenerPrecioCargaCredito(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiTulas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiTulas.Instancia.CalcularPrecio(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresMsj(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiValoresMsj.Instancia.CalcularPrecio(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValoresCarga(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiValoresCarga.Instancia.CalcularPrecio(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiCargaConsolidado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiCargaConsolidado.Instancia.CalcularPrecio(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        public TAPrecioMensajeriaDC CalcularPrecioRapiValijas(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiValijas.Instancia.CalcularPrecio(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi promocional
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioServicioDC CalcularPrecioRapiPromocional(int idListaPrecios, decimal cantidad, string idTipoEntrega = "-1")
        {
            return TAServicioRapiPromocional.Instancia.CalcularPrecio(idListaPrecios, cantidad, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi personalizado
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiPersonalizado(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiPersonalizado.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi hoy
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiHoy(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiHoy.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi envíos contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiEnvioContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiEnviosContraPago.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga contra pago
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCargaContraPago(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorContraPago, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiCargaContraPago.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorContraPago, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi carga
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioCargaDC CalcularPrecioRapiCarga(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiCarga.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi AM
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiAm(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiAm.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Indica si un trayecto determinado tiene o no una excepción de precios
        /// </summary>
        /// <param name="idListaPrecios"></param>
        /// <param name="idLocalidadOrigen"></param>
        /// <param name="idLocalidadDestino"></param>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public bool TrayectoTieneExcepcion(int idListaPrecios, string idLocalidadOrigen, string idLocalidadDestino, int idServicio)
        {
            return TARepositorio.Instancia.TrayectoTieneExcepcion(idListaPrecios, idLocalidadOrigen, idLocalidadDestino, idServicio);
        }

        /// <summary>
        /// Obtiene el precio del servicio notificaciones
        /// </summary>
        /// <param name="idListaPrecios">Identificador lista de precios</param>
        /// <returns>Valores de precios</returns>
        public TAPrecioMensajeriaDC CalcularPrecioNotificaciones(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioNotificaciones.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Obtiene el precio del servicio rapi radicado
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <param name="idListaPrecio">Identificador de la lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador localidad de origen</param>
        /// <param name="idLocalidadDestino">Identificador localidad de destino</param>
        /// <param name="peso">Peso</param>
        /// <param name="valorDeclarado">Valor declarado</param>
        /// <returns>Precio</returns>
        public TAPrecioMensajeriaDC CalcularPrecioRapiRadicado(int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioRapiRadicado.Instancia.CalcularPrecio(idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

        /// <summary>
        /// Calcula precio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <param name="idListaPrecio">Identificador id lista de precio</param>
        /// <param name="idLocalidadOrigen">Identificador ciudad de origen</param>
        /// <param name="idLocalidadDestino">Identificador ciudad de destino</param>
        /// <param name="peso">Peso</param>
        /// <returns>Objeto valor</returns>
        public TAPrecioMensajeriaDC CalcularPrecioCargaExpress(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
        {
            return TAServicioCargaExpress.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
        }

      public TAPrecioMensajeriaDC CalcularPrecioCargaAerea(int idServicio, int idListaPrecio, string idLocalidadOrigen, string idLocalidadDestino, decimal peso, decimal valorDeclarado, bool esPrimeraAdmision = true, string idTipoEntrega = "-1")
    {
      return TAServicioCargaExpressAerea.Instancia.CalcularPrecio(idServicio, idListaPrecio, idLocalidadOrigen, idLocalidadDestino, peso, valorDeclarado, esPrimeraAdmision, idTipoEntrega);
    }

        /// <summary>
        /// Obtiene los valores adicionales de un servicio
        /// </summary>
        /// <param name="idServicio">Identificador servicio</param>
        /// <returns>Colección de valores adicionales</returns>
        public IEnumerable<TAValorAdicional> ObtenerValorValoresAdicionalesServicio(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerValorValoresAdicionalesServicio(idServicio);
        }

        /// <summary>
        /// Obtener concepto de caja a partir del numero del servicio
        /// </summary>
        /// <param name="idServicio"></param>
        /// <returns></returns>
        public int ObtenerConceptoCaja(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerConceptoCaja(idServicio);
        }

        /// <summary>
        /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor)
        {
            return TAAdministradorTarifas.Instancia.ObtenerValorGiroClienteContadoGiro(idContrato, valor);
        }

        /// <summary>
        ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
        /// </summary>
        public TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal)
        {
            return TAAdministradorTarifas.Instancia.CalcularValorServicoAPartirValorTotal(idContrato, valorTotal);
        }

        /// <summary>
        /// Obtener los impuestos de el servicio de giros
        /// </summary>
        /// <returns></returns>
        public TAServicioImpuestosDC ObtenerImpuestosGiros()
        {
            return TAAdministradorTarifas.Instancia.ObtenerImpuestosGiros();
        }

        /// <summary>
        /// Obtiene los datos básicos del servicio
        /// </summary>
        /// <param name="idServicio">Identificador del servicio</param>
        /// <returns>Objeto Servicio</returns>
        public TAServicioDC ObtenerDatosServicio(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerDatosServicio(idServicio);
        }

        /// <summary>
        /// Obtiene de precio rango para el servicio de giros
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public TAPrecioRangoImpuestosDC ObtenerPrecioRangoImpuestosGiro()
        {
            return TAAdministradorTarifas.Instancia.ObtenerPrecioRangoImpuestosGiro();
        }

        /// <summary>
        /// Obtiene de precio rango
        /// </summary>
        /// <returns>Colección de precio rango para una lista de precio servicio</returns>
        public IEnumerable<TAPrecioRangoDC> ObtenerPrecioRango(int idServicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerPrecioRango(idServicio);
        }

        /// Obtiene la lista de valor adicional de la DB
        /// </summary>
        /// <returns></returns>
        public List<TAServicioDC> ObtenerServicios()
        {
            return TARepositorio.Instancia.ObtenerServicios().ToList();
        }

        /// <summary>
        /// Obtiene los servicios parametrizados según forma de pago y tipo cuenta Novasoft
        /// </summary>
        /// <returns></returns>
        public List<CAServiciosFormaPagoDC> ObtenerServicioFormaPagoNovasoft()
        {
            return TAAdministradorTarifas.Instancia.ObtenerServicioFormaPago();
        }
        /// <summary>
        /// Actualizacion(Adicion, Eliminacion, Modificacion) Servicios Formas Pago Novasoft
        /// </summary>
        /// <param name="obj"></param>
        public void ActualizacionRegistrosParametrizacionServicioFormaPagoNova(CAServiciosFormaPagoDC obj)
        {
            TAAdministradorTarifas.Instancia.ActualizacionRegistrosParametrizacionServicioFormaPagoNova(obj);
        }

        /// <summary>
        /// Obtiene formas de pago por cada servicio
        /// </summary>
        /// <param name="lstServicios"></param>
        /// <returns></returns>
        public TAFormaPagoServicio ObtenerFormasPagoPorServicios(int servicio)
        {
            return TAAdministradorTarifas.Instancia.ObtenerListaFormasPagoPorServicio(servicio);
        }

        /// <summary>
        /// Consulta los servicios con los pesos minimos y maximos 
        /// </summary>
        /// <param name="numeroGuia"></param>
        /// <returns></returns>
        public List<TAServicioPesoDC> ConsultarServiciosPesosMinimoxMaximos()
        {
            return TAAdministradorTarifas.Instancia.ConsultarServiciosPesosMinimoxMaximos();
        }

        /// <summary>
        /// Obtiene los servicios de rapicarga, Rapi Carga Terrestre y mensajeria por municipio origen y destino 
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public List<int> ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(string municipioOrigen, string municipioDestino)
        {
            return TAAdministradorTarifas.Instancia.ObtenerListaServiciosParaMensajeriaExpresaMayorPeso(municipioOrigen,municipioDestino);
        }
    }
}