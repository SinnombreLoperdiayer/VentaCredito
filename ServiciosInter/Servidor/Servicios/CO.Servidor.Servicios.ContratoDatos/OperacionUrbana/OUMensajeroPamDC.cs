using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using Framework.Servidor.Servicios.ContratoDatos.Seguridad;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class OUMensajeroPamDC : DataContractBase
    {
        [DataMember]
        public OUPersonaInternaDC PersonaInterna { get; set; }
        [DataMember]
        public ASUsuario Usuario { get; set; }
        [DataMember]
        public string NombreCompleto { get; set; }
    }
}
