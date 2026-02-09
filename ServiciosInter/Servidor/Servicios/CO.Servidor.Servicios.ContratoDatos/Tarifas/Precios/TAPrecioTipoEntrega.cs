using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using Framework.Servidor.Comun;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas.Precios
{
    /// <summary>
    /// Clase que contiene la información del precio por tipo de entrega
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class TAPrecioTipoEntrega : DataContractBase
    {
        [DataMember]
        public int IdListaPrecio { get; set; }

        [DataMember]
        public int IdServicio { get; set; }

        [DataMember]
        public long IdPrecioTipoEntrega { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Descripcion", Description = "TooltipDescripcionTipoEntrega")]    
        public string DescripcionTipoEntrega { get; set; }
        
        private string idTipoEntrega;
        [DataMember]
        public string IdTipoEntrega
        {
            get { return idTipoEntrega; }
            set { idTipoEntrega = value; OnPropertyChanged("IdTipoEntrega"); }
        }
        
        private ADTipoEntrega tipoEntrega;
        [DataMember]
        public ADTipoEntrega TipoEntrega
        {
            get { return tipoEntrega; }
            set { tipoEntrega = value; OnPropertyChanged("TipoEntrega"); }
        }

        [DataMember]
        public int IdListaPrecioServicio { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloInicial")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal ValorKiloInicial { get; set; }


        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorKiloAdicional")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal ValorKiloAdicional { get; set; }


        private decimal pesoInicial;
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoInicial")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal PesoInicial
        {
            get { return pesoInicial; }
            set
            {

                if (value != 0)
                {
                    if (value < PesoFinal)
                        pesoInicial = value;
                }
                else
                    if (PesoFinal == 0)
                        pesoInicial = value;

                OnPropertyChanged("PesoInicial");
            }
        }


        private decimal pesoFinal;
        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PesoFinal")]
        [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
        public decimal PesoFinal
        {
            get { return pesoFinal; }
            set
            {

                if (value != 0)
                {
                    if (value > PesoInicial)
                        pesoFinal = value;
                }
                else
                    if (PesoInicial == 0)
                        pesoFinal = value;

                OnPropertyChanged("PesoFinal");
            }
        }
          [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }
    }
}
