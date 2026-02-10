using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADTrazaGuia : DataContractBase
    {
        public ADTrazaGuia()
        {
            FechaGrabacion = DateTime.Now;
        }

        /// <summary>
        /// retorna o asigna el id de admision mensajeria
        /// </summary>
        [DataMember]
        public long? IdTrazaGuia { get; set; }

        /// <summary>
        /// retorna o asigna el id de admision mensajeria
        /// </summary>
        [DataMember]
        public long? IdAdmision { get; set; }

        /// <summary>
        /// Retorna o asigna el número de guía
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NumeroGuia", Description = "TooltipNumeroGuia")]
        public long? NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        [DataMember]
        public short? IdEstadoGuia { get; set; }

        [DataMember]
        public int NumeroPieza { get; set; }

        [DataMember]
        public int TotalPiezas { get; set; }

        /// <summary>
        /// retorna o asigna la descripción de el estado de la guia
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Estado")]
        public string DescripcionEstadoGuia { get; set; }

        /// <summary>
        /// retorna o asigna el estado de la guia
        /// </summary>
        [DataMember]
        public short? IdNuevoEstadoGuia { get; set; }

        /// <summary>
        /// Retorna o asigna las observaciones de la guia
        /// </summary>
        [DataMember]
        public string Observaciones { get; set; }

        /// <summary>
        /// retorna o asigna el id de la ciudad del centro logistico
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdUbicacionActual", Description = "TooltipIdUbicacionActual")]
        public string IdCiudad { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad del centro logistico
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "UbicacionActual", Description = "TooltipUbicacionActual")]
        public string Ciudad { get; set; }

        /// <summary>
        /// Retorna o asigna el modulo desde el cual se realiza el cambio de estado
        /// </summary>
        [DataMember]
        public string Modulo { get; set; }

        /// <summary>
        /// Fecha de grabación del registro
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Fecha")]
        public DateTime FechaGrabacion { get; set; }

        /// <summary>
        /// retorna o asigna el usuario que produjo el cambio de estado
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// Fecha de grabación del registro
        /// </summary>
        [DataMember]
        public DateTime FechaAdmisionGuia { get; set; }


        [DataMember]
        public long IdCentroServicioEstado { get; set; }

        /// <summary>
        /// IdCentroServicioEstado
        /// </summary>
        [DataMember]
        public long IdEstadoGuiaLog { get; set; }

        /// <summary>
        /// Nombre centro servicio estado 
        /// </summary>

        [DataMember]
        public string NombreCentroServicioEstado { get; set; }

        [DataMember]
        public string Latitud { get; set; }
        [DataMember]
        public string Longitud { get; set; }

        /// <summary>
        /// Fecha de Entrega Registrada
        /// </summary>
        [DataMember]
         public DateTime FechaEntrega { get; set;  }

        /// <summary>
        /// Id Origen Aplicacion
        /// </summary>
        [DataMember]
        public int IdOrigenApliacion { get; set; }

        /// <summary>
        /// Id Origen Aplicacion
        /// </summary>
        [DataMember]
        public bool ReversaEstado { get; set; }

        /// <summary>
        /// Id Origen Aplicacion
        /// </summary>
        [DataMember]
        public bool EstadoHabilitado { get; set; }

        /// <summary>
        /// clase de estado sispostal
        /// </summary>
        [DataMember]
        public string ClaseEstado { get; set; }
        public ADMotivoGuiaDC MotivoGuia { get; set; }

    }
}