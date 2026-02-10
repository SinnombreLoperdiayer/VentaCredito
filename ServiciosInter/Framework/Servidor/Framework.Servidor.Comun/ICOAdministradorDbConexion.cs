using System;
namespace Framework.Servidor.Comun
{
    /// <summary>
    /// Descripcion: Interface para administrar la conexion a la base de datos 
    ///              de los diferentes proveedores
    /// Autor: Miguel Gonzalez
    /// Fecha: 29/09/2011
    /// Version: 1.0
    /// Modificado por:
    /// Fecha Modificación:
    /// </summary>   
    public interface ICOAdministradorDbConexion
    {
        /// <summary>
        /// Obtener la cadena de conexion
        /// </summary>
        /// <param name="nombreModelo">Nombre del modelo el cual se encuentra configurado
        /// en el archivo .Config en la seccion connectionStrings </param>
        /// <returns>Cadena de conexion</returns>
        string ObtenerConnectionString(string nombreModelo);
    }
}
