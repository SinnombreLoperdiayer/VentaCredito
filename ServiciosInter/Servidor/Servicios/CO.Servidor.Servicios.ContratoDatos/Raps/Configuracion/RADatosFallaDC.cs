using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    public class RADatosFallaDC
    {
        #region guia
        public short IdMotivoGuia { get; set; }
        public string NombreCompleto { get; set; }
        public long IdCentroServicioDestino { get; set; }
        public string Ciudad { get; set; }
        public long IdCentroLogistico { get; set; }
        public DateTime FechaMotivoDevolucion { get; set; }
        public string Observaciones { get; set; }
        public long? NumeroGuia { get; set; }
        public string NombreCentroServicioDestino { get; set; }

        #endregion

        //public OUGuiaIngresadaDC Guia { get; set; }
        #region recogida
        public string IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaProgramacionRecogida { get; set; }
        public string IdCiudad { get; set; }
        public string DocPersonaResponsable { get; set; }
        //public RGAsignarRecogidaDC Recogida { get; set; }
        public string DireccionCliente { get; set; }
        #endregion

        public int IdMotivo { get; set; }



        public string Adjunto { get; set; }

        public DateTime FechaAdmision { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public DateTime FechaDescarga { get; set; }
        public string Foto { get; set; }

        public string TipoObjeto { get; set; }

        public List<RAParametrosPersonalizacionRapsDC> Parametros { get; set; }

        public List<RAParametrosPersonalizacionRapsDC> ResultadoParametros { get; set; }

        public int IdTipoNovedad { get; set; }

        public bool EjecutaFuciones { get; set; }

        public int IdSistema { get; set; }

        public decimal PesoAuditoria { get; set; }
        public decimal ValorAuditoria { get; set; }
        public decimal DiferenciaPeso { get; set; }
        public decimal ValorTotalAuditoria { get; set; }
        public ADEnumEstadoGuia EstadoGuia { get; set; }
        public long IdAdmision { get; set; }
        public string CentroSercicioOrigen { get; set; }
    }
}
