using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace Framework.Servidor.Comun
{
  /// <summary>
  /// Descripcion: Fabrica para crear la conexion a base de datos
  /// Autor: Miguel Gonzalez
  /// Fecha: 29/09/2011
  /// Version: 1.0
  /// Modificado por:
  /// Fecha Modificación:
  /// </summary>
  public class FabricaConexionesDb
  {
    /// <summary>
    /// Tipo de motor de base de datos a utilizar
    /// </summary>
    private COEnumMotorBaseDatos eMotorBaseDatos;

    /// <summary>
    /// Lee del config el proveedor de base de datos a utilizar
    /// </summary>
    private FabricaConexionesDb()
    {
      COMetadataConfigurationSectionFramework confiSeccion = WebConfigurationManager.GetSection("efMetadataSection") as COMetadataConfigurationSectionFramework;
      //eMotorBaseDatos = (COEnumMotorBaseDatos)Enum.Parse(typeof(COEnumMotorBaseDatos), confiSeccion.Provider, true);
    }

    /// <summary>
    /// Instancia singleton de la clase
    /// </summary>
    private static readonly FabricaConexionesDb instancia = new FabricaConexionesDb();

    /// <summary>
    /// Retorna el objeto singleton de la clase
    /// </summary>
    public static FabricaConexionesDb Instancia
    {
      get { return instancia; }
    }

    /// <summary>
    /// retorna el Administrador de Conexion segun el proveedor configurado en el archivo .Config
    /// </summary>
    /// <returns></returns>
    public ICOAdministradorDbConexion CrearDbConexion()
    {
      eMotorBaseDatos = COEnumMotorBaseDatos.SQLSERVER;

      switch (eMotorBaseDatos)
      {
        case COEnumMotorBaseDatos.SQLSERVER:
          return COAdministradorConexionesDbFramework.Instancia;
        default:
          return COAdministradorConexionesDbFramework.Instancia;
      }
    }
  }
}