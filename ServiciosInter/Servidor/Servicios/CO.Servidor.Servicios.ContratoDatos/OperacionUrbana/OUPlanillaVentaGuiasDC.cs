using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUPlanillaVentaGuiasDC : DataContractBase
    {
        /// <summary>
        /// Retorna o asigna el numero de la planilla
        /// </summary>
        [DataMember]
        public long IdPlanilla { get; set; }

        /// <summary>
        /// retorna o asigna el numero de las guias
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Guia", Description = "Guia")]
        [CamposOrdenamiento("ADM_NumeroGuia")]
        public long NumeroGuia { get; set; }

        /// <summary>
        /// retorna o asigna el numero de la tula o contenedor
        /// </summary>
        [DataMember]
        public string NumeroTulaContenedor { get; set; }

        /// <summary>
        /// retorna o asigna el id de la admision
        /// </summary>
        [DataMember]
        public long IdAdmision { get; set; }

        /// <summary>
        /// Retorna o asigna si el envio es recomendado
        /// </summary>
        [DataMember]
        public bool EsRecomendado { get; set; }

        /// <summary>
        /// Retorna o asigna si Id de la unidad de negocio
        /// </summary>
        [DataMember]
        public string IdUnidadNegocio { get; set; }

        /// <summary>
        /// Retorna o asigna el id de la ciudad origen
        /// </summary>
        [DataMember]
        public string IdCiudadOrigen { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad origen
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadOrigen", Description = "CiudadOrigen")]
        public string NombreCiudadOrigen { get; set; }

        /// <summary>
        /// Retorna o asigna el id de la ciudad origen
        /// </summary>
        [DataMember]
        public string IdCiudadDestino { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad origen
        /// </summary>
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "CiudadOrigen", Description = "CiudadOrigen")]
        public string NombreCiudadDestino { get; set; }

        /// <summary>
        /// Retorna o asigna si la guia fue seleccionada
        /// </summary>
        [DataMember]
        public bool EsSeleccionado { get; set; }

        /// <summary>
        /// Retorna o asigna el id del servicio
        /// </summary>
        [DataMember]
        public int IdServicio { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre del servicio
        /// </summary>
        [DataMember]
        public string NombreServicio { get; set; }

        /// <summary>
        /// Retorna o asigna lo que contienen el envio
        /// </summary>
        [DataMember]
        public string DiceContener { get; set; }

        /// <summary>
        /// Retorna o asigna el peso del envio
        /// </summary>
        [DataMember]
        public decimal Peso { get; set; }

        [DataMember]
        public long IdPuntoServicio { get; set; }

        [DataMember]
        public long? IdAsignacionTula { get; set; }

        [DataMember]
        public long? NumContTransRetorno { get; set; }
        [DataMember]
        public int TotalPiezasRotulo { get; set; }
        [DataMember]
        public int PiezaActualRotulo { get; set; }
        [DataMember]
        public string IdCiudadOrigenGuia { get; set; }

        /// <summary>
        /// Retorna o asigna el nombre de la ciudad origen
        /// </summary>
        [DataMember]
        public string NombreCiudadOrigenGuia { get; set; }

        [DataMember]
        public OUMensajeroDC Mensajero { get; set; }

        [DataMember]
        public long IdIngresoGuia { get; set; }

        [DataMember]
        public string TipoEnvio { get; set; }

        [DataMember]
        public OUEnumTipoEnvioPlanilla TipoEnvioPlanilla { get; set; }

        [DataMember]
        public bool EsEntregada { get; set; }

        [DataMember]
        public string CreadoPor { get; set; }
    }
}