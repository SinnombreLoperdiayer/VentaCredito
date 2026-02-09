using System.Runtime.Caching;
using ServiciosInter.DatosCompartidos.Comun;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Parametros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Tarifas;
using ServiciosInter.Infraestructura.AccesoDatos.Repository.Parametros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ServiciosInter.Negocio.Parametros
{
    public class ParametrosNegocio
    {
        private static readonly ParametrosNegocio instancia = new ParametrosNegocio();

        public static ParametrosNegocio Instancia
        {
            get { return ParametrosNegocio.instancia; }
        }

        private ParametrosNegocio()
        {
            lstDiasNoHabiles = new Lazy<List<DateTime>>(() => ParametrosRepository.Instancia.ObtenerDiasNoHabiles());
        }

        private Lazy<List<DateTime>> lstDiasNoHabiles = null;

        public List<DateTime> LstDiasNoHabiles
        {
            get
            {
                return lstDiasNoHabiles.Value;
            }
        }

        /// <summary>
        /// Consulta los parametros generales del Framework
        /// </summary>
        /// <param name="llave">Nombre del parametro</param>
        /// <returns>Valor el parametro</returns>
        public string ConsultarParametrosFramework(string llave)
        {
            return ParametrosRepository.Instancia.ConsultarParametrosFramework(llave);
        }

        /// <summary>
        /// Retorna la lista de localidad que no son países ni departamentos para Colombia
        /// </summary>
        /// <returns></returns>
        public List<PALocalidadDC> ObtenerLocalidadesNoPaisNoDepartamentoColombia()
        {
            return ParametrosRepository.Instancia.ObtenerMunicipiosCorregimientoInspeccionCaserioPais((int)PAEnumTipoLocalidad.PAIS, (int)PAEnumTipoLocalidad.DEPARTAMENTO, "057");
        }

        /// <summary>
        /// Obtener la agencia a partir de la localidad
        /// </summary>
        ///// <param name="localidad"></param>
        public PUCentroServiciosDC ObtenerAgenciaLocalidad(string localidad)
        {
            return ParametrosRepository.Instancia.ObtenerAgenciaLocalidad(localidad);
        }

        /// <summary>
        /// Valida si un municipio permite forma de pago alcobro.
        /// </summary>
        /// <param name="idMunicipio"></param>
        /// <returns> true => Si tiene forma de pago alcobro,    false => No tiene forma de pago alcobro </returns>
        public bool ValidarMunicipioPermiteAlcobro(string idMunicipio)
        {
            return ParametrosRepository.Instancia.ValidarMunicipioPermiteAlcobro(idMunicipio);
        }

        /// <summary>
        /// Retorna la lista del horario de determinado centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<TAHorarioRecogidaCsvDC> ObtenerHorarioRecogidaDeCsv(long idCentroServicio)
        {
            return ParametrosRepository.Instancia.ObtenerHorarioRecogidaDeCsv(idCentroServicio);
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio destino</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestino(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal peso)
        {
            if (servicio.IdServicio != TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                return ParametrosRepository.Instancia.ValidarServicioTrayectoDestino(municipioOrigen, municipioDestino, servicio, peso);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retorna el número de dias del trayecto si existe
        /// </summary>
        /// <param name="municipioOrigen">Municipio destino</param>
        /// <param name="municipioDestino">Municipio de destino</param>
        /// <param name="servicio">Servicio a validar</param>
        /// <param name="idListaPrecios">Lista de precios</param>
        /// <returns></returns>
        public TATiempoDigitalizacionArchivo ValidarServicioTrayectoDestinoExcepcion(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino, TAServicioDC servicio, decimal peso, int idListaPrecios)
        {
            if (servicio.IdServicio != TAConstantesServicios.SERVICIO_INTERNACIONAL)
            {
                return ParametrosRepository.Instancia.ValidarServicioTrayectoDestinoExcepcion(municipioOrigen, municipioDestino, servicio, peso, idListaPrecios);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene
        /// </summary>
        /// <param name="municipioOrigen"></param>
        /// <param name="municipioDestino"></param>
        /// <returns></returns>
        public int ObtenerIdServicioDeMayorTiempoEntrega(PALocalidadDC municipioOrigen, PALocalidadDC municipioDestino)
        {
            return ParametrosRepository.Instancia.ObtenerIdServicioDeMayorTiempoEntrega(municipioOrigen, municipioDestino);
        }

        /// <summary>
        /// Método para obtener los agregar los dias laborales entre dos fechas
        /// </summary>
        /// <param name="fechaOriginal"></param>
        /// <param name="diasLaborables"></param>
        /// <returns></returns>
        public int ConsultarDiasLaborales(DateTime fechaInicial, DateTime fechaFinal)
        {
            return ParametrosRepository.Instancia.ConsultarDiasLaborales(fechaInicial, fechaFinal);
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
            return ParametrosRepository.Instancia.ValidarServicioTrayectoCasilleroAereo(municipioOrigen, municipioDestino, idServicio);
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabilSinSabados(DateTime fechadesde, double numerodias, string idPais, List<DateTime> fechasH = null)
        {
            int numeroDiasNoHabiles = 0;
            DateTime fechaHasta = fechadesde.AddDays(numerodias);

            for (int i = 0; i <= numerodias; i++)
            {
                if (fechadesde.AddDays(i).DayOfWeek == DayOfWeek.Saturday)
                {
                    fechaHasta = fechaHasta.AddDays(1);
                }
            }

            numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date > fechadesde.Date && d.Date <= fechaHasta.Date).Count();

            if (numerodias == 0 && fechadesde.Date == fechaHasta.Date)
            {
                numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date >= fechadesde.Date && d.Date <= fechaHasta.Date).Count();
            }

            if (numeroDiasNoHabiles == 0)
            {
                return fechaHasta;
            }
            else
            {
                return ObtenerFechaFinalHabilSinSabados(fechaHasta, (numeroDiasNoHabiles), idPais, fechasH);
            }
        }

        public TAServicioDC ObtenerNombreServicioPorIdServicio(int idServicio)
        {
            ObjectCache cache = MemoryCache.Default;
            string cacheKey = "Servicio_" + idServicio;

            if (cache.Contains(cacheKey))
            {
                return (TAServicioDC)cache.Get(cacheKey);
            }
            else
            {
                TAServicioDC servicio = ParametrosRepository.Instancia.ObtenerNombreServicioPorIdServicio(idServicio);
                if (servicio != null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddHours(24)
                    };
                    cache.Set(cacheKey, servicio, policy);
                }
                return servicio;
            }
        }

        /// <summary>
        /// Obtiene la fecha hábil más próxima desde una fecha inicial sumando un número de días específicos teniendo en cuenta los sábados como días hábiles
        /// </summary>
        /// <param name="fechadesde">Fecha desde la cual se hará la consulta</param>
        /// <param name="numerodias">Número de días que se quiere sumar a la fecha desde</param>
        public DateTime ObtenerFechaFinalHabil(DateTime fechadesde, double numerodias, string idPais)
        {
            int numeroDiasNoHabiles = 0;
            DateTime fechaHasta = fechadesde.AddDays(numerodias);
            numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date > fechadesde && d.Date <= fechaHasta).Count();
            if (numerodias == 0 && fechadesde.Date == fechaHasta.Date)
            {
                numeroDiasNoHabiles = LstDiasNoHabiles.Where(d => d.Date >= fechadesde.Date && d.Date <= fechaHasta.Date).Count();
            }
            if (numeroDiasNoHabiles == 0)
                return fechaHasta;
            else
                return ObtenerFechaFinalHabil(fechaHasta, (numeroDiasNoHabiles), idPais);
        }

        /// <summary>
        /// Actualiza la informació de validación de dos centros de servicios implicados en un trayecto
        /// </summary>
        /// <param name="localidadDestino">Ciudad o municipio de destino del envío</param>
        /// <param name="validacion">Validación del trayecto</param>
        /// <param name="idCentroServicio">Id del centro de servicios de origen de la transacción</param>
        /// <param name="localidadOrigen">si no se tiene el id centro de servicio origen el metodo lo busca a través de la localidad original</param>
        public void ObtenerInformacionValidacionTrayecto(PALocalidadDC localidadDestino, ADValidacionServicioTrayectoDestino validacion, long idCentroServicio, PALocalidadDC localidadOrigen = null)
        {
            ParametrosRepository.Instancia.ObtenerInformacionValidacionTrayecto(localidadDestino, validacion, idCentroServicio, localidadOrigen);
        }

        /// <summary>
        /// Obtener información de validación del trayecto
        /// </summary>
        /// <param name="localidadOrigen"></param>
        /// <param name="idCentroServicioOrigen"></param>
        public void ObtenerInformacionValidacionTrayectoOrigen(PALocalidadDC localidadOrigen, ADValidacionServicioTrayectoDestino validacion)
        {
            ParametrosRepository.Instancia.ObtenerInformacionValidacionTrayectoOrigen(localidadOrigen, validacion);
        }

        public PAContratoCliente ObtenerContratoPorIdCliente(int idCliente, int idcontrato = 0)
        {
            return ParametrosRepository.Instancia.ObtenerContratoCliente(idCliente, idcontrato);
        }

        /// <summary>
        /// Obtiene los Servicios de la DB
        /// </summary>
        /// <returns>Lista con los servicios de la DB</returns>
        public List<TAServicioDC> ObtenerServicios()
        {
            return ParametrosRepository.Instancia.ObtenerServicios();
        }

        /// <summary>
        /// Obtiene los servicios asociados el contrato de la DB
        /// </summary>
        /// <param name="idContrato">Identificador del contrato</param>
        /// <returns>Lista de servicios</returns>
        public List<TAServicioDC> ObtenerServiciosPorContrato(int idContrato)
        {
            return ParametrosRepository.Instancia.ObtenerServiciosContratoCliente(idContrato);
        }

        /// <summary>
        /// Verifica si por ese contrato tiene el servicio asignado
        /// </summary>
        /// <param name="idContrato"></param>
        /// <param name="idServicio"></param>
        /// <returns>Retorna 1 si el cliente tiene el servicio asignado y 0 si no</returns>
        public bool ValidarServicioPorContrato(int idContrato, int idServicio)
        {
            return ParametrosRepository.Instancia.ValidarServicioContratoCliente(idContrato, idServicio);
        }

        /// <summary>
        /// Obtiene toda la info basica de los centros de servicio
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicio()
        {
            return ParametrosRepository.Instancia.ObtenerInformacionGeneralCentrosServicio();
        }

        /// <summary>
        /// Sin horarios
        /// </summary>
        /// <returns></returns>
        public List<PUCentroServicioInfoGeneral> ObtenerInformacionGeneralCentrosServicioAPP()
        {
            return ParametrosRepository.Instancia.ObtenerInformacionGeneralCentrosServicioAPP();
        }

        /// <summary>
        /// Consultar Horarios
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<string> ObtenerHorariosCentroServicioAppRecogidas(long idCentroServicio)
        {
            return ParametrosRepository.Instancia.ObtenerHorariosCentroServicioAppRecogidas(idCentroServicio);
        }

        /// <summary>
        /// Obtiene los servicios junto con sus horarios de venta de un centro de servicio
        /// </summary>
        /// <param name="idCentroServicio"></param>
        /// <returns></returns>
        public List<PUServicio> ObtenerServiciosCentroServicio(long idCentroServicio)
        {
            return ParametrosRepository.Instancia.ObtenerServiciosCentroServicio(idCentroServicio);
        }

        /// <summary>
        /// Consulta la fecha del servidor
        /// </summary>
        /// <returns></returns>
        public List<PATipoIdentificacion> ConsultarTiposIdentificacion()
        {
            return ParametrosRepository.Instancia.ConsultarTiposIdentificacion();
        }

        public DateTime ObtenerFechaRecogidaCiudad(string idCiudad, string fecha)
        {
            return ParametrosRepository.Instancia.ObtenerFechaRecogidaCiudad(idCiudad, fecha);
        }

        public IList<string> ObtenerFestivosAnio()
        {
            return ParametrosRepository.Instancia.ObtenerFestivosAnio();
        }

        /// <summary>
        /// Servicio que trae el valor comercial minimo y maximo que aplica para contrapago
        /// Hevelin Dayana Diaz Susa - 12/11/2021
        /// </summary>
        public TAPrecioRangoContrapago ValidarValorComercialContrapago(int IdListaPrecio)
        {
            return ParametrosRepository.Instancia.ValidarValorComercialContrapago(IdListaPrecio);
        }

        /// <summary>
        /// Servicio que valida si existe un centro de servicioque aplique contrapago en una determinada localidad 
        /// Hevelin Dayana Diaz - 12/11/2021
        /// </summary>
        /// <param name="IdLocalidadOrigen"></param>
        public PUCentroServiciosDC ObtenerCentroServicioContrapagoLocalidad(string IdLocalidadOrigen)
        {
            return ParametrosRepository.Instancia.ObtenerCentroServicioContrapagoLocalidad(IdLocalidadOrigen);
        }

        /// <summary>
        /// Retorna el Estado (0 = Inactivo / 1 = Activo) de una localidad para saber si es viable o no para que permita crear preenvios
        /// </summary>
        /// <param name="IdLocalidadDestino"></param>
        /// <returns>Estado de validez de la localidad destino</returns>
        public bool ConsultarValidezDestinoGeneracionGuias(string IdLocalidadDestino)
        {
            return ParametrosRepository.Instancia.ConsultarValidezDestinoGeneracionGuias(IdLocalidadDestino);
        }

    }
}