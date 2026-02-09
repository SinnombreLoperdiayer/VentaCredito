using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes
{
    [DataContract]
    public class RegistroSolicitudAppDC
    {
        [DataMember]
        public string IdCiudad { get; set; }

        [DataMember]
        public int IdSistema { get; set; }

        [DataMember]
        public int IdTipoNovedad { get; set; }

        [DataMember]
        public long NumeroGuia { get; set; }

        [DataMember]
        public string NombreCompleto { get; set; }

        [DataMember]
        public DateTime FechaRegistro { get; set; }

        [DataMember]
        public int Motivo { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Foto { get; set; }

        [DataMember]
        public long IdCentroServicio { get; set; }

        [DataMember]
        public string Observacion { get; set; }

        [DataMember]
        public string Adjunto { get; set; }

        [DataMember]
        public bool EsPersonalizado { get; set; }

        [DataMember]
        //public Dictionary<string, object> Parametros;
        public List<RAParametrosPersonalizacionRapsDC> Parametros;
        //public List<RAResponsableTipoNovedadDC> Parametros;

        [DataMember]
        public Dictionary<string, object> ValoresParametros;

        //[DataMember]
        //public Dictionary<string, object> Resultado;

        [DataMember]
        public int IdOrigenRaps;

        [DataMember]
        public int EstadoOrigen;

        [DataMember]
        public ADGuia Guia;

        [DataMember]
        public int IdResponsable;

        [DataMember]
        public RAResponsableDC DatosReponsable;



    }
}