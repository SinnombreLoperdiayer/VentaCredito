using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Framework.Servidor.Comun.DataAnnotations
{
  /// <summary>
  /// Clase que permite establecer los campos por lo cuales se pueden hacer filtros
  /// </summary>
  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public class FiltrableAttribute : System.Attribute
  {
    protected string campoBDRelacionado = "";
    protected string labelText = "";
    protected COEnumTipoControlFiltro tipoControl;

    public string FormatoRegex { get; set; }

    public int MaximaLongitud { get; set; }

    public string MensajeError { get; set; }

    /// <summary>
    /// Constructor del atributo de filtrado
    /// </summary>
    /// <param name="CampoBDRelacionado">Campo de la base de datos que se relaciona con la propiedad que posee este atributo</param>
    /// <param name="LabelText">Texto que se le debe establecer al label que acompaña el control</param>
    /// <param name="TipoControl">Tipo de control que se desea crear (No soporta combobox el atributo) Si se requiere un combobox se debe hacer programaticamente</param>
    public FiltrableAttribute(string CampoBDRelacionado, string LabelText, COEnumTipoControlFiltro TipoControl)
    {
      campoBDRelacionado = CampoBDRelacionado;
      labelText = LabelText;
      tipoControl = TipoControl;
    }

    public FiltrableAttribute(string CampoBDRelacionado, Type ResourceType, string ResourceName, COEnumTipoControlFiltro TipoControl)
    {
      ResourceManager resmgr = new ResourceManager(ResourceType);
      campoBDRelacionado = CampoBDRelacionado;

      try
      {
        labelText = resmgr.GetString(ResourceName);
      }
      catch (MissingManifestResourceException)
      {
      }
      tipoControl = TipoControl;
    }

    public FiltrableAttribute(string CampoBDRelacionado, Type ResourceType, string ResourceLabelName, string ResourceErrorName, COEnumTipoControlFiltro TipoControl)
    {
      ResourceManager resmgr = new ResourceManager(ResourceType);
      campoBDRelacionado = CampoBDRelacionado;
      labelText = resmgr.GetString(ResourceLabelName);
      MensajeError = resmgr.GetString(ResourceErrorName);
      tipoControl = TipoControl;
    }

    /// <summary>
    /// Obtiene los valores del atrbibuto asignado a una propiedad especifica
    /// </summary>
    /// <typeparam name="T">Tipo Generico de dato del objeto al que se le quiere extraer los valores del atributo Filtrable</typeparam>
    /// <param name="tipo">Tipo Generico de dato del objeto al que se le quiere extraer los valores del atributo Filtrable</param>
    /// <param name="nombrePropiedad">Nombre de la propiedad de la cual se desea extraer la información</param>
    /// <param name="CampoBDRelacionado">Campo de salida que representa el campo de la base de datos relacionado a esta propiedad</param>
    /// <param name="LabelText">Campo de salida que representa el texto del label que acompaña el control</param>
    /// <param name="TipoControl">Tipo de control que se desea crear en la interfaz gráfica</param>
    public static void ObtenerValores<T>(string nombrePropiedad, out string CampoBDRelacionado, out string LabelText, out COEnumTipoControlFiltro TipoControl, out string formatoRegex, out int longitudMaxima, out string MensajeError)
    {
      CampoBDRelacionado = string.Empty;
      LabelText = string.Empty;
      TipoControl = COEnumTipoControlFiltro.TextBox;
      formatoRegex = string.Empty;
      longitudMaxima = 0;
      MensajeError = string.Empty;

      Type classType = typeof(T);
      PropertyInfo property = classType.GetProperty(nombrePropiedad);

      object[] attributes = property.GetCustomAttributes(true);

      List<string> valorret = new List<string>();
      if (attributes.Length != 0)
      {
        foreach (object attribute in attributes)
        {
          FiltrableAttribute attr = attribute as FiltrableAttribute;
          if (attr != null)
          {
            CampoBDRelacionado = attr.campoBDRelacionado;
            LabelText = attr.labelText;
            TipoControl = attr.tipoControl;
            if (attr.FormatoRegex != null)
              formatoRegex = attr.FormatoRegex;
            longitudMaxima = attr.MaximaLongitud;
            if (attr.MensajeError != null)
              MensajeError = attr.MensajeError;
          }
        }
      }
    }
  }
}