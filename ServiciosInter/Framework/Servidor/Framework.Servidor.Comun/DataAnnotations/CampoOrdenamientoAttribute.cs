using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework.Servidor.Comun.DataAnnotations
{
  [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
  public class CamposOrdenamientoAttribute : System.Attribute
  {
    protected string campo1;
    protected string campo2;
    protected string campo3;

    public CamposOrdenamientoAttribute(string Campo1, string Campo2, string Campo3)
    {
      campo1 = Campo1;
      campo2 = Campo2;
      campo3 = Campo3;
    }

    public CamposOrdenamientoAttribute(string Campo1, string Campo2)
    {
      campo1 = Campo1;
      campo2 = Campo2;
    }

    public CamposOrdenamientoAttribute(string Campo1)
    {
      campo1 = Campo1;
    }

    public static string ObtenerExpresion<T>(string nombrePropiedad)
    {
      string expresionOrdenamiento = string.Empty;

      Type classType = typeof(T);
      PropertyInfo property = classType.GetProperty(nombrePropiedad);

      string[] propiedades = nombrePropiedad.Split('.');
      if (propiedades.Count() > 1)
      {
        for (int i = 0; i <= propiedades.Count() - 1; i++)
        {
          if (propiedades.Count() > i + 1)
          {
            PropertyInfo property2 = classType.GetProperty(propiedades[i]);
            if (property2 != null)
            {
              PropertyInfo property3 = property2.PropertyType.GetProperty(propiedades[i + 1]);
              if (property3 != null)
                property = property3;
            }
          }
        }
      }

      if (property != null)
      {
        object[] attributes = property.GetCustomAttributes(true);

        List<string> valorret = new List<string>();
        if (attributes.Length != 0)
        {
          foreach (object attribute in attributes)
          {
            CamposOrdenamientoAttribute attr = attribute as CamposOrdenamientoAttribute;
            if (attr != null)
            {
              if (attr.campo1 != null)
                expresionOrdenamiento = attr.campo1;
              if (attr.campo2 != null)
                expresionOrdenamiento = expresionOrdenamiento + ", " + attr.campo2;
              if (attr.campo3 != null)
                expresionOrdenamiento = expresionOrdenamiento + ", " + attr.campo3;
              return expresionOrdenamiento;
            }
          }
        }
      }
      return expresionOrdenamiento;
    }
  }
}