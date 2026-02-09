using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAParametrizacionRapsDC
    {
        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public int IdSistemaFuente { get; set; }

        [DataMember]
        public int IdTipoRap { get; set; }

        [DataMember]
        public string DescripcionRaps { get; set; }

        [DataMember]
        public int IdProceso { get; set; }

        [DataMember]
        public bool UtilizaFormato { get; set; }

        [DataMember]
        public int IdFormato { get; set; }

        [DataMember]
        public int IdTipoCierre { get; set; }

        [DataMember]
        public string IdCargoCierra { get; set; }

        [DataMember]
        public string IdCargoIncumplimiento { get; set; }

        [DataMember]
        public int IdOrigenRaps { get; set; }

        [DataMember]
        public bool Estado { get; set; }

        [DataMember]
        public int IdGrupoUsuario { get; set; }

        [DataMember]
        public int IdSubclasificacion { get; set; }

        [DataMember]
        public int IdTipoPeriodo { get; set; }

        [DataMember]
        public int IdHoraEscalar { get; set; }

        [DataMember]
        public int IdTipoHora { get; set; }

        [DataMember]
        public long IdParametrizacionPadre { get; set; }

        [DataMember]
        public int IdTipoIncumplimiento { get; set; }

        [DataMember]
        public int IdNivelGravedad { get; set; }

        [DataMember]
        public int IdTipoEscalonamiento { get; set; }

    }
}
