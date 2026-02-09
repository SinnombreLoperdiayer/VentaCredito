using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Tarifas
{
  public class TAPrecioTarifaMensajeria
  {    
    public decimal ValorKiloInicial { get; set; }

    public decimal ValorKiloAdicional { get; set; }

    public decimal ValorTotal { get; set; }
    
    public decimal ValorPrimaSeguro { get; set; }

    public int TiempoEntregaHoras { get; set; }
  }
}
