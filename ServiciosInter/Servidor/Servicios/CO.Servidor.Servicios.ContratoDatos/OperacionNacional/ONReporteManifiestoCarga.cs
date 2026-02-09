using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.ParametrosOperacion;
using CO.Servidor.Servicios.ContratoDatos.Rutas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionNacional
{
  /// <summary>
  /// Contiene la informacion del reporte del manifiesto de carga
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class ONReporteManifiestoCarga
  {
    [DataMember]
    public AREmpresaDC Empresa { get; set; }

    [DataMember] 
    public RURutaDC Ruta { get; set; }

    [DataMember]
    public POVehiculo Vehiculo { get; set; }

    [DataMember]
    public POMensajero Conductor { get; set; }

    [DataMember]
    public ONManifiestoOperacionNacional Manifiesto { get; set; }

    [DataMember]
    public decimal PesoEnvios { get; set; }

    [DataMember]
    public string NombreAseguradoraGeneral { get; set; }

    [DataMember]
    public string NumPolizaAseguradoraGeneral { get; set; }

    [DataMember]
    public DateTime FechaGeneracionReporteManifiesto { get; set; }

    [DataMember]
    public decimal ValorAPagarPactado { get; set; }

    [DataMember]
    public string ValorAPagarPactadoLetras { get; set; }

    [DataMember]
    public DateTime FechaPagoSaldo { get; set; }
    
    [DataMember]
    public string DireccionCentroServOrigen { get; set; }

    [DataMember]
    public string DireccionCentroServDestino { get; set; }
  }
}