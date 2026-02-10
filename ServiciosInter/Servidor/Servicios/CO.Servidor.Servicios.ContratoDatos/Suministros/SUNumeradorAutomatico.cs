using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Suministros
{
  /// <summary>
  /// Clase que contiene la informacion del numerador automatico
  /// </summary>
  public class SUNumeradorAutomatico
  {
    public long ValorActual { get; set; }

    public DateTime FechaInicial { get; set; }

    public DateTime FechaFinal { get; set; }

    public string IdNumerador { get; set; }
  }
}