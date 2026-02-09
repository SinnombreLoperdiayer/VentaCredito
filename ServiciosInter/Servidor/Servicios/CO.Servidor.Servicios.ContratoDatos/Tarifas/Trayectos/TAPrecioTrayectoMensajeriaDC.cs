using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Trayectos
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class TAPrecioTrayectoMensajeriaDC : DataContractBase
    {
        public event EventHandler OnCambioPorcentaje;

        [DataMember]
        public long IdPrecioTrayectoSubTrayecto { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public TATipoSubTrayecto SubTrayecto { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloAdicional")]
        public decimal KiloAdicional { get; set; }

        [DataMember]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloAdicional")]
        public decimal ValorBaseKiloAdicional { get; set; }

        [DataMember]
        public List<TATipoSubTrayecto> SubTrayectosDisponibles { get; set; }

        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        private decimal porcentajeIncremento;

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Incremento")]
        public decimal PorcentajeIncremento
        {
            get { return porcentajeIncremento; }
            set
            {
                porcentajeIncremento = value;
                if (OnCambioPorcentaje != null)
                    OnCambioPorcentaje(null, null);
            }
        }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorPropuestoKiloAdicional")]
        public decimal ValorPropuestoKiloAdicional { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorFijadoKiloAdicional")]
        public decimal ValorFijadoKiloAdicional { get; set; }
    }
}