using ServiciosInter.DatosCompartidos.EntidadesNegocio.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADGuiaClientePortalRespuesta
    {
        /// <summary>
        /// Asigna o retorna el objeto con los datos del remitente.
        /// </summary>
        public CLClienteContadoClienteRespuesta Remitente { get; set; }

        /// <summary>
        /// Asigna o retorna el objeto con los datos del destinatario.
        /// </summary>
        public CLClienteContadoClienteRespuesta Destinatario { get; set; }

        /// <summary>
        /// Asigna o retorna el numero de guia.
        /// </summary>
        public long NumeroGuia { get; set; }

        /// <summary>
        /// Asigna o retorna el nombre de la ciudad origen.
        /// </summary>
        public string NombreCiudadOrigen { get; set; }

        /// <summary>
        /// Asigna o retorna el nombre de la ciudad destino.
        /// </summary>
        public string NombreCiudadDestino { get; set; }

        /// <summary>
        /// Asigna o retorna la nueva fecha estimada entrega.
        /// </summary>
        public DateTime FechaEstimadaEntregaNew { get; set; }

        /// <summary>
        /// Asigna o retorna la fecha de admision.
        /// </summary>
        public DateTime FechaAdmision { get; set; }

        /// <summary>
        /// Asigna o retorna el nombre tipo envio.
        /// </summary>
        public string NombreTipoEnvio { get; set; }

        /// <summary>
        /// Asigna o retorna el total de piezas.
        /// </summary>
        public short TotalPiezas { get; set; }
        /// <summary>
        /// Asigna o retorna el Numero Pieza.
        /// </summary>
        public short NumeroPieza { get; set; }

        /// <summary>
        /// Asigna o retorna el peso.
        /// </summary>
        public decimal Peso { get; set; }

        /// <summary>
        /// Asigna o retorna el peso liquido volumetico.
        /// </summary>
        public decimal PesoLiqVolumetrico { get; set; }

        /// <summary>
        /// Asigna o retorna el numero bolsa de seguridad.
        /// </summary>
        public string NumeroBolsaSeguridad { get; set; }

        /// <summary>
        /// Asigna o retorna lo que dice contener.s
        /// </summary>
        public string DiceContener { get; set; }

        /// <summary>
        /// Asigna o retorna las observaciones.
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// Asigna o retorna el id de servicio.
        /// </summary>
        public int IdServicio { get; set; }

        /// <summary>
        /// Asigna o retorna el nombre del servicio.
        /// </summary>
        public string NombreServicio { get; set; }

        /// <summary>
        /// Asigna o retorna una lista con las formas de pago.
        /// </summary>
        public List<ADGuiaFormaPagoClienteRespuesta> FormasPago { get; set; }
    }
}
