using System;
using System.Runtime.Serialization;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas.Impresion
{
  /// <summary>
  /// Clase con los datos para el reporte GIR-R10
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CADatosImpresionGirR10DC
  {
    /// <summary>
    /// Retorna o asigna el nombre de la ciudad donde se imprime el reporte
    /// </summary>
    [DataMember]
    public string NombreCiudad { get; set; }

    /// <summary>
    /// Retorna o asigna la fecha de impresión
    /// </summary>
    [DataMember]
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Retorna o asigna el nombre del responsable de la impresión
    /// </summary>
    [DataMember]
    public string NombreResponsable { get; set; }

    /// <summary>
    /// Valor en letras
    /// </summary>
    [DataMember]
    public string ValorEnLetras { get; set; }

    /// <summary>
    /// Valor de la operación
    /// </summary>
    [DataMember]
    public decimal Valor { get; set; }

    /// <summary>
    /// Observación de la operación
    /// </summary>
    [DataMember]
    public string Obseracion { get; set; }

    [DataMember]
    public string CentoCostos { get; set; }

    /// <summary>
    /// Nombre del centro de servicios donde se hace la impresión
    /// </summary>
    [DataMember]
    public string NombreOficina { get; set; }


    [DataMember]
    public long IdOficina { get; set; }

    /// <summary>
    /// Número de documento GIR-R10
    /// </summary>
    [DataMember]
    public long NumeroDocumento { get; set; }

    [DataMember]
    public long NumeroGuia { get; set; }

    [DataMember]
    public string NumeroPrecinto { get; set; }

    [DataMember]
    public string NumeroBolsaSeguridad { get; set; }








  }
}