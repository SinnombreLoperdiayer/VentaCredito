using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.GestionGiros.Telemercadeo;

namespace CO.Servidor.Dominio.Comun.Telemercadeo
{
  public interface IGIFachadaTelemercadeo
  {
    /// <summary>
    /// Obtiene la informacion de Telemercadeo de
    /// un giro especifico
    /// </summary>
    /// <param name="idAdmisionGiro"></param>
    /// <returns>la info del telemercadeo de un giro</returns>
    GITelemercadeoGiroDC ObtenerTelemercadeoDeGiro(long idAdmisionGiro);
  }
}