using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class SUPropietarioGuia
  {
    /// <summary>
    /// Identificación o código del propietario del suministro
    /// </summary>
    [DataMember]
    public long Id { get; set; }

    /// <summary>
    /// Nombre del propietario del suministro
    /// </summary>
    [DataMember]
    public string Nombre { get; set; }

    /// <summary>
    /// Aplica solo cuando el propietario es una sucursal
    /// </summary>
    [DataMember]
    public PALocalidadDC PaisSucursal { get; set; }

    /// <summary>
    /// Aplica solo cuando el propietario es una sucursal
    /// </summary>
    [DataMember]
    public PALocalidadDC CiudadSucursal { get; set; }

    /// <summary>
    /// Aplica solo cuando el propietario es una sucursal
    /// </summary>
    [DataMember]
    public CLClientesDC Cliente { get; set; }

    /// <summary>
    /// Contiene el tipo de propietario en terminos del grupo de propietarios de suministros
    /// </summary>
    [DataMember]
    public SUEnumGrupoSuministroDC Propietario { get; set; }

    /// <summary>
    /// Contiene la información del centro de servicios cuando se trata de un centro de servicios
    /// </summary>
    [DataMember]
    public PUCentroServiciosDC CentroServicios { get; set; }

    /// <summary>
    /// Contiene el número de cédula cuando se trata de un mensajero
    /// </summary>
    [DataMember]
    public string CedulaMensajero { get; set; }

    [DataMember]
    public int? IdContrato { get; set; }

    [DataMember]
    public int? IdListaPrecios { get; set; }

    [DataMember]
    public bool? ContratoAplicaValidacionPesoAdm { get; set; }
  }
}