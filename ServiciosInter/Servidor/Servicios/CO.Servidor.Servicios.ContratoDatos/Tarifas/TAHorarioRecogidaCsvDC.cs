using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;
using System;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
    /// <summary>
    /// Contiene la informacion del horario de recogida de un centro de servicio
    /// </summary>
    /// 

    [DataContract (Namespace="http://contrologis.com")]
    public class TAHorarioRecogidaCsvDC
    {
        [DataMember]
        public int DiaDeLaSemana { get; set; }

        [DataMember]
        public DateTime HoraRecogida { get; set; }
    }
}
