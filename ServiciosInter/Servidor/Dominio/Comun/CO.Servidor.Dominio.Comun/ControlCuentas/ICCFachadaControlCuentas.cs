using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.ControlCuentas;

namespace CO.Servidor.Dominio.Comun.ControlCuentas
{
  public interface ICCFachadaControlCuentas
  {
    /// <summary>
    /// Obtiene la informacion del almacen
    /// </summary>
    /// <param name="idOperacion">Numero del giro</param>
    /// <returns></returns>
    List<CCAlmacenDC> ObtenerAlmacenControlCuentas(long idOperacion);

    /// <summary>
    /// Obtiene el almacen CtrolCuentas Giros
    /// </summary>
    /// <param name="idOperacion"></param>
    /// <returns>info del Archivo Control Ctas</returns>
    CCAlmacenDC ObtenerAlmacenControlCuentasGiros(long idOperacion);
  }
}