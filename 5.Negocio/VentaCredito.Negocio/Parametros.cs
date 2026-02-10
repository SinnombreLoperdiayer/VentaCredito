using Servicio.Entidades.Admisiones.Mensajeria;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VentaCredito.Transversal.Entidades.AdmisionPreenvio;

namespace VentaCredito.Negocio
{
    public class Parametros
    {
        private const string MENSAJE_SERVICIO_AGIL_SOLO_ENTREGA_DIRECCION = "Los servicios ágiles solo están disponibles para tipo de entrega en dirección.";
        private static Parametros instancia = new Parametros();

        public static Parametros Instancia
        {
            get
            {
                return instancia;
            }
        }

        public IEnumerable<ADTipoEntrega> ObtenerTiposEntrega()
        {
            return Datos.Repositorio.Parametros.Instancia.ObtenerTiposEntrega();
        }

        public IEnumerable<ServicioAgilFranjaDC> ObtenerHorariosServiciosAgiles(int? idServicio = null)
        {
            var catalogoServicios = Tarifas.Parametros.Instancia
                .ObtenerServicios()
                .ToDictionary(servicio => servicio.IdServicio, servicio => servicio.Nombre);

            IEnumerable<Datos.Repositorio.HorarioServicioAgil> listaServiciosAgiles = idServicio.HasValue
                ? Datos.Repositorio.Parametros.Instancia.ObtenerHorariosServiciosAgiles(idServicio)
                : catalogoServicios.Keys.SelectMany(idServicioCatalogo => Datos.Repositorio.Parametros.Instancia.ObtenerHorariosServiciosAgiles(idServicioCatalogo));

            var serviciosAgiles = listaServiciosAgiles
                .Where(TieneParametria)
                .Select(servicio => MapearServicioAgil(servicio, catalogoServicios))
                .ToList();

            if (idServicio.HasValue && !serviciosAgiles.Any())
            {
                throw CrearErrorControlado(HttpStatusCode.Conflict, "El servicio ágil seleccionado no tiene parametría vigente de nombre/alias/horario.");
            }

            return serviciosAgiles;
        }

        public bool EsServicioAgil(int idServicio)
        {
            return Datos.Repositorio.Parametros.Instancia.ObtenerHorariosServiciosAgiles(idServicio).Any(TieneParametria);
        }

        public void ValidarRestriccionTipoEntregaServicioAgil(int idServicio, string idTipoEntrega)
        {
            if (!EsServicioAgil(idServicio))
            {
                return;
            }

            ADTipoEntrega tipoEntrega = ObtenerTiposEntrega().FirstOrDefault(tipo => tipo.Id == idTipoEntrega);
            bool esEntregaDireccion = tipoEntrega != null && !string.IsNullOrWhiteSpace(tipoEntrega.Descripcion)
                && tipoEntrega.Descripcion.IndexOf("direccion", StringComparison.OrdinalIgnoreCase) >= 0;

            if (!esEntregaDireccion)
            {
                throw CrearErrorControlado(HttpStatusCode.PreconditionFailed, MENSAJE_SERVICIO_AGIL_SOLO_ENTREGA_DIRECCION);
            }
        }

        private static ServicioAgilFranjaDC MapearServicioAgil(Datos.Repositorio.HorarioServicioAgil servicio, IReadOnlyDictionary<int, string> catalogoServicios)
        {
            string nombreServicio = !string.IsNullOrWhiteSpace(servicio.NombreServicio)
                ? servicio.NombreServicio
                : (catalogoServicios.ContainsKey(servicio.IdServicio) ? catalogoServicios[servicio.IdServicio] : string.Empty);

            if (string.IsNullOrWhiteSpace(servicio.Alias) && string.IsNullOrWhiteSpace(nombreServicio))
            {
                throw CrearErrorControlado(HttpStatusCode.Conflict, "No fue posible resolver nombre o alias del servicio ágil parametrizado.");
            }

                        if (!servicio.AplicaTodoDia.GetValueOrDefault() && (!servicio.HoraInicio.HasValue || !servicio.HoraFin.HasValue))
            {
                throw CrearErrorControlado(HttpStatusCode.Conflict, "No fue posible resolver horario del servicio ágil parametrizado.");
            }

            string horaInicio = servicio.HoraInicio.HasValue ? servicio.HoraInicio.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : string.Empty;
            string horaFin = servicio.HoraFin.HasValue ? servicio.HoraFin.Value.ToString("HH:mm", CultureInfo.InvariantCulture) : string.Empty;
            string franja = servicio.AplicaTodoDia.GetValueOrDefault() ? "Durante el día" : string.Format("{0} - {1}", horaInicio, horaFin);

            return new ServicioAgilFranjaDC
            {
                IdHorario = servicio.IdHorario,
                IdServicio = servicio.IdServicio,
                NombreServicio = nombreServicio,
                Alias = string.IsNullOrWhiteSpace(servicio.Alias) ? nombreServicio : servicio.Alias,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                FranjaHoraria = franja,
                AplicaTodoDia = servicio.AplicaTodoDia.GetValueOrDefault()
            };
        }


        private static bool TieneParametria(Datos.Repositorio.HorarioServicioAgil servicio)
        {
            return servicio != null
                && servicio.IdServicio > 0
                && (servicio.AplicaTodoDia.GetValueOrDefault() || (servicio.HoraInicio.HasValue && servicio.HoraFin.HasValue));
        }

        private static HttpResponseException CrearErrorControlado(HttpStatusCode statusCode, string mensaje)
        {
            return new HttpResponseException(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(mensaje)
            });
        }
    }
}
