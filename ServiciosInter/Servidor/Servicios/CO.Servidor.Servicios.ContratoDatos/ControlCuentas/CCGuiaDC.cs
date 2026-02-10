using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CCGuiaDC : DataContractBase
    {
        [DataMember]
        public long NumeroGuia { get; set; }
        [DataMember]
        public int IdEstadoGuia { get; set; }
        [DataMember]
        public bool EsAutomatico { get; set; }
        [DataMember]
        public bool EsNovedad { get; set; }
        [DataMember]
        public List<CCEnumTipoNovedadCtrLiquidacionDC> TipoNovedades { get; set; }
        [DataMember]
        public string DescripcionTipoNovedad { get; set; }
        [DataMember]
        public decimal ValorComercialAdmision { get; set; }
        [DataMember]
        public decimal ValorTotalAdmision { get; set; }
        [DataMember]
        public decimal PesoTotalAdmision { get; set; }
        [DataMember]
        public decimal ValorComercialPruebaEntrega { get; set; }
        [DataMember]
        public decimal ValorTotalPruebaEntrega { get; set; }
        [DataMember]
        public decimal PesoTotalPruebaEntrega { get; set; }
        [DataMember]
        public decimal PesoBasculaAuditoria { get; set; }
        [DataMember]
        public decimal LargoVolumetricoAuditoria { get; set; }
        [DataMember]
        public decimal AnchoVolumetricoAuditoria { get; set; }
        [DataMember]
        public decimal AltoVolumetricoAuditoria { get; set; }
        [DataMember]
        public decimal PesoVolumetricoTotalAuditoria { get; set; }
        [DataMember]
        public decimal PesoTotalAuditoria { get; set; }
        [DataMember]
        public DateTime FechaAuditoria { get; set; }
        [DataMember]
        public string ObservacionesAuditoria { get; set; }
        [DataMember]
        public string CreadoPor { get; set; }
        [DataMember]
        public List<string> ImagenesAuditoria { get; set; }
        [DataMember]
        public int IdFormaPagoInicial { get; set; }
        [DataMember]
        public string DescripcionFormaPagoInicial { get; set; }
        [DataMember]
        public int IdFormaPagoAuditoria { get; set; }
        [DataMember]
        public string DescripcionFormaPagoAuditoria { get; set; }
        [DataMember]
        public int IdServicioInicial { get; set; }
        [DataMember]
        public string DescripcionServicioInicial { get; set; }
        [DataMember]
        public int IdServicioAuditoria { get; set; }
        [DataMember]
        public string DescripcionServicioAuditoria { get; set; }
        [DataMember]
        public int IdCiudadDestinoInicial { get; set; }
        [DataMember]
        public string NombreCiudadDestinoInicial { get; set; }
        [DataMember]
        public int IdCiudadDestinoAuditoria { get; set; }
        [DataMember]
        public string NombreCiudadDestinoAuditoria { get; set; }
        [DataMember]
        public RAEnumOrigenRaps OrigenRaps { get; set; }
        [DataMember]
        public EnumTipoReporteAuditoriaPeso TipoAuditoriaPeso { get; set; }

        [DataMember]
        public long IdCentroServicio { get; set; }
    }
}
