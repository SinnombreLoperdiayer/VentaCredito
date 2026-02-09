using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.ControlCuentas;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;

namespace CO.Servidor.ControlCuentas
{
  public class CCFachadaControlCuentas : ICCFachadaControlCuentas
  {
    #region SingleToN

    private static readonly CCFachadaControlCuentas instancia = new CCFachadaControlCuentas();

    /// <summary>
    /// Retorna una instancia de la fachada de admisiones
    /// /// </summary>
    public static CCFachadaControlCuentas Instancia
    {
      get { return CCFachadaControlCuentas.instancia; }
    }

    #endregion SingleToN

    #region Metodos

    /// <summary>
    /// Obtiene la informacion del almacen
    /// </summary>
    /// <param name="idOperacion">Numero del giro</param>
    /// <returns></returns>
    public List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion)
    {
      return CCControlCuentas.Instancia.ObtenerAlmacenControlCuentas(idOperacion);
    }

    /// <summary>
    /// Obtiene el almacen CtrolCuentas Giros
    /// </summary>
    /// <param name="idOperacion"></param>
    /// <returns>info del Archivo Control Ctas</returns>
    public CCAlmacenDC ObtenerAlmacenControlCuentasGiros(long idOperacion)
    {
      return CCControlCuentas.Instancia.ObtenerAlmacenControlCuentasGiros(idOperacion);
    }

    #endregion Metodos
  }
}