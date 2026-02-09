using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria
{
    public class ADTrazaGuia
    {
        /// <summary>
        /// retorna o asigna el id de admision mensajeria
        /// </summary>
        public long? IdTrazaGuia { get; set; }

        /// <summary>
        /// retorna o asigna el id de admision mensajeria
        /// </summary>
        public long? IdAdmision { get; set; }

        /// <summary>
        /// Retorna o asigna el número de guía
        /// </summary>
        public long? NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        public short? IdEstadoGuia { get; set; }

        public int NumeroPieza { get; set; }

        public int TotalPiezas { get; set; }

        /// <summary>
        /// retorna o asigna la descripción de el estado de la guia
        /// </summary>
        public string DescripcionEstadoGuia { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        public short? IdNuevoEstadoGuia { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        public string Observaciones { get; set; }

        /// <summary>
        /// retorna o asigna el id de la ciudad del centro logistico
        /// </summary>
        public string IdCiudad { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad del centro logistico
        /// </summary>
        public string Ciudad { get; set; }

        /// <summary>
        /// Retorna o asigna el modulo desde el cual se realiza el cambio de estado
        /// </summary>
        public string Modulo { get; set; }

        /// <summary>
        /// Fecha de grabación del registro
        /// </summary>
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// retorna o asigna el usuario que produjo el cambio de estado
        /// </summary>
        public string Usuario { get; set; }

        /// <summary>
        /// Fecha de grabación del registro
        /// </summary>
        public DateTime FechaAdmisionGuia { get; set; }

        public long IdCentroServicioEstado { get; set; }

        /// <summary>
        /// IdCentroServicioEstado
        /// </summary>
        public long IdEstadoGuiaLog { get; set; }

        /// <summary>
        /// Nombre centro servicio estado
        /// </summary>
        public string NombreCentroServicioEstado { get; set; }

        public string Latitud { get; set; }

        public string Longitud { get; set; }

        /// <summary>
        /// Fecha de Entrega Registrada
        /// </summary>
        public DateTime FechaEntrega { get; set; }

        /// <summary>
        /// Id Origen Aplicacion
        /// </summary>
        public int IdOrigenApliacion { get; set; }

        /// <summary>
        /// Id Origen Aplicacion
        /// </summary>
        public bool ReversaEstado { get; set; }

        /// <summary>
        /// Id Origen Aplicacion
        /// </summary>
        public bool EstadoHabilitado { get; set; }

        /// <summary>
        /// clase de estado sispostal
        /// </summary>
        public string ClaseEstado { get; set; }

        public ADMotivoGuiaDC MotivoGuia { get; set; }
    }
}