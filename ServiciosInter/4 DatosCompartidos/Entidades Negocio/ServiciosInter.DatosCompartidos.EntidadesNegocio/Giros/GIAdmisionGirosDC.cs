using ServiciosInter.DatosCompartidos.EntidadesNegocio.CentrosServicio;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using System;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros
{
    public class GIAdmisionGirosDC
    {
        #region VENTA DE GIROS

        public long? IdAdminGiro { get; set; }

        public long? IdGiro { get; set; }

        public string PrefijoIdGiro { get; set; }

        public string CodVerfiGiro { get; set; }

        public string EstadoGiro { get; set; }

        public PUCentroServiciosDC AgenciaOrigen { get; set; }

        public PUCentroServiciosDC AgenciaDestino { get; set; }

        public TAPrecioDC Precio { get; set; }

        public long? DeclaracionVoluntariaOrigenes { get; set; }

        public bool RequiereDeclaracionVoluntaria { get; set; }

        public string ArchivoDeclaracionVoluntariaOrigenes { get; set; }

        public string Observaciones { get; set; }

        public string GuidDeChequeo { get; set; }

        public GIGirosPeatonPeatonDC GirosPeatonPeaton { get; set; }

        public DateTime FechaGrabacion { get; set; }

        public long NumeroGiroAgenciaManual { get; set; }

        public int IdCaja { get; set; }

        public long IdCodigoUsuario { get; set; }

        public string IdTipoGiro { get; set; }

        public bool HabilitarVentaGiros { get; set; }

        public string ReImpresion { get; set; }

        public bool RecibeNotificacionPago { get; set; }

        public string UsuarioCreacionGiro { get; set; }

        public bool GiroAutomatico { get; set; }

        #endregion VENTA DE GIROS

        #region PAGOS DE GIROS

        public DateTime FechaLimitePagoGiro { get; set; }

        public string DocumentoDestinatario { get; set; }

        public string NombreDestinatario { get; set; }

        public string TelefonoDestinatario { get; set; }

        public string EmailDestinatario { get; set; }

        public string DocumentoRemitente { get; set; }

        public string NombreRemitente { get; set; }

        public string TelefonoRemitente { get; set; }

        public string EmailRemitente { get; set; }

        public bool EsTransmitido { get; set; }

        #endregion PAGOS DE GIROS

        #region VALIDACIÓN

        public bool TieneGestion { get; set; }

        public long Caja { get; set; }

        public int Lote { get; set; }

        public int Posicion { get; set; }

        public bool Aprobada { get; set; }

        public bool NoAprobada { get; set; }

        public string ObservacionesSolicitudes { get; set; }

        #endregion VALIDACIÓN

        // TODO:ID Este campo solo se utiliza para la integracion con 472
        public long IdEstadoGiro { get; set; }

        // TODO:ID Este campo solo se utiliza para la integracion con 472
        public DateTime FechaEstadoGiro { get; set; }

        // TODO:ID Este campo solo se utiliza para la integracion con 472
        public bool EstACT_yaTransmitido { get; set; }

        public string NombreServicio { get; set; }

        public ADFacturaDianDC FacturaDian { get; set; }

    }
}
