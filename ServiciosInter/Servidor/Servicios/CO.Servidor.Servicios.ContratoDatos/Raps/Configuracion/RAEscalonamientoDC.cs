using CO.Servidor.Servicios.ContratoDatos.Raps.Escalonamiento;
using Framework.Servidor.Servicios.ContratoDatos;
using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class RAEscalonamientoDC
    {

        public RAEscalonamientoDC()
        {

        }

        public RAEscalonamientoDC(RAEscalonamientoDC escalonamiento)
        {
            this.IdParametrizacionRap = escalonamiento.IdParametrizacionRap;
            this.idCargo = escalonamiento.idCargo;
            this.IdProceso = escalonamiento.IdProceso;
            this.NombreProceso = escalonamiento.NombreProceso;
            this.IdProcedimiento = escalonamiento.IdProcedimiento;
            this.NombreProcedimiento = escalonamiento.NombreProcedimiento;
            this.Orden = escalonamiento.Orden;
            this.IdTipoHora = escalonamiento.IdTipoHora;
            this.HorasEscalar = escalonamiento.HorasEscalar;
            this.IdSucursalEscalar = escalonamiento.IdSucursalEscalar;
            this.CorreoEscalar = escalonamiento.CorreoEscalar;
            this.Descripcion = escalonamiento.Descripcion;
            this.DocumentoEmpeladoEscalar = escalonamiento.DocumentoEmpeladoEscalar;
            this.CargoRegional = escalonamiento.CargoRegional;
            this.CargoEnteControl = escalonamiento.CargoEnteControl;
            this.IdSolicitud = escalonamiento.IdSolicitud;
            this.CargoEscalar = escalonamiento.CargoEscalar;
            this.IdCodigoPlanta = escalonamiento.IdCodigoPlanta;
            this.IdTipoEscalonamiento = escalonamiento.IdTipoEscalonamiento;
    }

        [DataMember]
        public long IdParametrizacionRap { get; set; }

        [DataMember]
        public string idCargo { get; set; }

        [DataMember]
        public string IdProceso { get; set; }

        [DataMember]
        public string NombreProceso { get; set; }

        [DataMember]
        public string IdProcedimiento { get; set; }

        [DataMember]
        public string NombreProcedimiento { get; set; }

        [DataMember]
        public int Orden { get; set; }

        [DataMember]
        public int IdTipoHora { get; set; }

        [DataMember]
        public int HorasEscalar { get; set; }

        [DataMember]
        public string IdSucursalEscalar { get; set; }

        [DataMember]
        public string CorreoEscalar { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string DocumentoEmpeladoEscalar { get; set; }

        [DataMember]
        public bool CargoRegional { get; set; }

        [DataMember]
        public bool CargoEnteControl { get; set; }

        [DataMember]
        public long IdSolicitud { get; set; }

        [DataMember]
        public RACargoEscalarDC CargoEscalar { get; set; }

        [DataMember]
        public string IdCodigoPlanta { get; set; }

        [DataMember]
        public int IdTipoEscalonamiento { get; set; }
    } 
}
