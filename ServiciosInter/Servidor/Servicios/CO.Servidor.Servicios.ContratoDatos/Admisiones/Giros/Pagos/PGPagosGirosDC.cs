using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using Framework.Servidor.Servicios.ContratoDatos;
using Framework.Servidor.Servicios.ContratoDatos.Agenda;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;

namespace CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Pagos
{
  /// <summary>
  /// Clase con la informacion para pagar un giro
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class PGPagosGirosDC : DataContractBase
  {
    /// <summary>
    /// Id de la caja que realiza el giro
    /// </summary>
    [DataMember]
    public int IdCaja { get; set; }

    /// <summary>
    /// Es el id del usuario. Rafram 14-09-2012
    /// </summary>
    [DataMember]
    public long IdCodigoUsuario { get; set; }

    [DataMember]
    public long IdAdmisionGiro { get; set; }

    /// <summary>
    /// Retorna o asigna el número del giro
    /// </summary>
    [DataMember]
    public long? IdGiro { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ComprobantePago", Description = "ToolTipComprobantePago")]
    public long IdComprobantePago { get; set; }

    [DataMember]
    public bool PagoAutorizadoPeaton { get; set; }

    [DataMember]
    public bool PagoAutorizadoEmpresarial { get; set; }

    [DataMember]
    public bool PagoAutomatico { get; set; }

    [DataMember]
    public long IdCentroServiciosPagador { get; set; }

    [DataMember]
    public PALocalidadDC LocalidadPagador { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "CentroServicio", Description = "TooltipCentroServicio")]
    public string NombreCentroServicios { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "ValorPagado")]
    public decimal ValorPagado { get; set; }

    /// <summary>
    /// Informacion del cliente que cobra el giro
    /// </summary>
    [DataMember]
    public CLClienteContadoDC ClienteCobrador { get; set; }

    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    public string Observaciones { get; set; }

    /// <summary>
    /// Identifica la imagen de la autorizacion del pago
    /// </summary>
    [DataMember]
    public long? AutorizacionPago { get; set; }

    /// <summary>
    /// Contiene el archivo de autorizacion
    /// </summary>
    [DataMember]
    public string ArchivoAutorizacionPago { get; set; }

    /// <summary>
    /// Identifica el archivo imagen de la cedula con la que reclaman el giro
    /// </summary>
    [DataMember]
    public long? CedulaClientePago { get; set; }

    /// <summary>
    /// Contiene el archivo de la cedula del cliente
    /// </summary>
    [DataMember]
    public string ArchivoCedulaClientePago { get; set; }

    /// <summary>
    /// Identificador del archivo imagen del certificado de la empresa
    /// </summary>
    [DataMember]
    public long? CertificadoEmpresa { get; set; }

    /// <summary>
    /// Contiene el archivo imagen del certificado de la empresa para poder reclamar el giro
    /// </summary>
    [DataMember]
    public string ArchivoCertificadoEmpresa { get; set; }

    /// <summary>
    /// Contiene el archivo imágen del comprobante de pago
    /// </summary>
    [DataMember]
    public string ArchivoComprobantePago { get; set; }

    /// <summary>
    /// contiene el archivo imagen de la declaracion voluntara de fondos
    /// </summary>
    [DataMember]
    public string ArchivoDeclaracionVoluntariaOrigenes { get; set; }

    /// <summary>
    /// Precio que vale el servicio
    /// </summary>
    [DataMember]
    public decimal ValorServicio { get; set; }

    /// <summary>
    /// Usuario que realiza el pago
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "UsuarioPago", Description = "ToolTipUsuarioPago")]
    public string UsuarioPago { get; set; }

    /// <summary>
    /// Fecha y hora de pago
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "FechaPago", Description = "ToolTipFechaPago")]
    public DateTime FechaHoraPago { get; set; }

    /// <summary>
    /// Documento del destinatario
    /// </summary>
    [DataMember]
    public string DocumentoDestinatario { get; set; }

    /// <summary>
    /// Nombre de los archivos
    /// </summary>
    [DataMember]
    public List<ASArchivoFramework> Archivos { get; set; }

    /// <summary>
    /// Observaciones cuando el numero de comprobante de pago no corresponde a la agencia destino
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Cliente.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    public string ObservacionesComprobante { get; set; }

    /// <summary>
    /// Es el estado del giro
    /// </summary>
    [DataMember]
    public string EstadoGiro { get; set; }

    /// <summary>
    /// Bandera que indica si se envia un correo al pagar el giro
    /// </summary>
    [DataMember]
    public bool EnviaCorreo { get; set; }


    /// <summary>
    /// Dirección de correo electronico del remitente
    /// </summary>
    [DataMember]
    public string CorreoRemitente { get; set; }


    [DataMember]
    public string TelefonoRemitente { get; set; }
  }
}