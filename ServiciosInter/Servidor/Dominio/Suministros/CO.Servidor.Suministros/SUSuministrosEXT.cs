using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Suministros.Datos;

namespace CO.Servidor.Suministros
{
  public partial class SUSuministros
  {
    /// <summary>
    /// Obtiene los tipos de  suministros existentes
    /// </summary>
    /// <returns></returns>
    public IEnumerable<SUSuministro> ObtenerTiposSuministros()
    {
      return SURepositorio.Instancia.ObtenerTiposSuministros();
    }

    /// <summary>
    /// Consulta el suministro asociado a un prefijo especifico
    /// </summary>
    /// <param name="prefijo"></param>
    /// <returns></returns>
    public SUSuministro ConsultarSuministroxPrefijo(string prefijo)
    {
      return SURepositorio.Instancia.ConsultarSuministroxPrefijo(prefijo);
    }
  }
}