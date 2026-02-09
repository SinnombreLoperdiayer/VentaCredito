using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria
{
    /// <summary>
    /// Contiene información de los parámetros de inicio para admisiones
    /// </summary>
    [DataContract(Namespace = "http://contrologis.com")]
    public class ADParametrosAdmisiones
    {
        [DataMember]
        public string UnidadMedidaPorDefecto { get; set; }

        [DataMember]
        public decimal PorcentajePrimaSeguro { get; set; }

        [DataMember]   
        public decimal TopeMaxValorDeclarado { get; set; }

        [DataMember]
        public decimal TopeMinVlrDeclRapiCarga { get; set; }

        [DataMember]
        public decimal PesoPorDefecto { get; set; }

        [DataMember]
        public string TipoMonedaPorDefecto { get; set; }

        [DataMember]
        public bool TipoMonedaModificable { get; set; }

        [DataMember]
        public int PesoMinimoRotulo { get; set; }

        [DataMember]
        [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PorcentajeRecargo", Description = "ToolTipPorcentajeRecargo")]
        public double PorcentajeRecargo { get; set; }

        [DataMember]
        public int NumeroPiezasAplicaRotulo { get; set; }

        [DataMember]
        public string ImagenPublicidadGuia { get; set; }

        [DataMember]
        public string ValorReimpresionCertificacion { get; set; }
    }
}