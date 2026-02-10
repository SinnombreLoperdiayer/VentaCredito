using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Dominio.Comun.Telemercadeo;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;

namespace CO.Servidor.Telemercadeo.Giros
{
  public class GIFachadaTelemercadeo : IGIFachadaTelemercadeo
  {
    /// <summary>
    /// Obtiene la informacion de Telemercadeo de
    /// un giro especifico
    /// </summary>
    /// <param name="idAdmisionGiro"></param>
    /// <returns>la info del telemercadeo de un giro</returns>
    public GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro)
    {
      return GITelemercadeoGiros.Instancia.ObtenerTelemercadeoDeGiro(idAdmisionGiro);
    }
  }
}