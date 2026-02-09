using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using Framework.Servidor.Comun.DataAnnotations;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.OperacionUrbana
{
  [DataContract(Namespace = "http://contrologis.com")]
  public class OUPersonaInternaDC : DataContractBase
  {
    /// <summary>
    /// retorna o asigna la cedula de la persona
    /// </summary>
      private string identificacion;

         [DataMember]
         [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
         [CamposOrdenamiento("PEI_Identificacion")]
         [Filtrable("PEI_Identificacion", "Identificacion: ", COEnumTipoControlFiltro.TextBox)]
         [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Identificacion", Description = "Identificacion")]
      public string Identificacion
      {
          get { return identificacion; }
          set {
              identificacion =  value.Trim(); }
      }

    /// <summary>
    /// retorna o asigna el tipo de identificacion de la persona
    /// </summary>
    [DataMember]
    public string IdTipoIdentificacion { get; set; }

    /// <summary>
    /// retorna o asigna el tipo de identificacion de la persona
    /// </summary>
    [DataMember]
    public string IdMunicipio { get; set; }

    /// <summary>
    /// retorna o asigna el tipo de identificacion de la persona
    /// </summary>
    [DataMember]
    public long IdPersonaInterna { get; set; }

    /// <summary>
    /// Retorna o asigna el id del cargo de la persona
    /// </summary>
    [DataMember]
    public int IdCargo { get; set; }

    /// <summary>
    /// Retorna o asigna el cargo de la persona
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Cargo", Description = "Cargo")]
    //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Cargo { get; set; }

    /// <summary>
    /// retorna o asigna el nombre de la persona
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Nombre", Description = "Nombre")]
    public string Nombre { get; set; }

    /// <summary>
    /// Retorna o Asigna el primer apellido de la persona
    /// </summary>
    [DataMember]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PrimerApellido", Description = "PrimerApellido")]
    public string PrimerApellido { get; set; }

    /// <summary>
    /// Retorna o asigna el segundo apellido de la persona
    /// </summary>
    [DataMember]
    public string SegundoApellido { get; set; }

    /// <summary>
    /// retorna o asigna la direccion de la persona
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Direccion", Description = "Direccion")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Direccion { get; set; }

    /// <summary>
    /// Retorna o asigna el municipio de la persona
    /// </summary>
    [DataMember]
    [CamposOrdenamiento("LOC_Nombre")]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Ciudad", Description = "Ciudad")]
   // [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [Filtrable("LOC_Nombre", "Ciudad: ", COEnumTipoControlFiltro.TextBox)]
    public string Municipio { get; set; }

    /// <summary>
    /// retorna o asigna el telefono de la persona
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Telefono", Description = "Telefono")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string Telefono { get; set; }

    /// <summary>
    /// retorna o asigna el correo de la persona
    /// </summary>
    [DataMember]
    public string Email { get; set; }

    /// <summary>
    /// retorna o asigna el codigo de la regional de  la persona
    /// </summary>
    [DataMember]
    public long Regional { get; set; }

    /// <summary>
    /// retorna o asigna el codigo de la regional de  la persona
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoContrato", Description = "TipoContrato")]
    //[Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public string TipoContrato { get; set; }

    /// <summary>
    /// REtorna o asigana la fecha de inicio de contrato
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaIngreso", Description = "FechaIngreso")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    public DateTime FechaInicioContrato { get; set; }

    /// <summary>
    /// Retorna o asigna la fecha de terminacion del contrato
    /// </summary>
    [DataMember]
    [Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaTerminacionContrato", Description = "FechaTerminacionContrato")]
    [Required(ErrorMessageResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), ErrorMessageResourceName = "CampoRequerido")]
    [DataType(DataType.DateTime)]
    public DateTime FechaTerminacionContrato { get; set; }

    /// <summary>
    /// Retorna o asigna observaciones
    /// </summary>
    [DataMember]
    public string Comentarios { get; set; }
  }
}