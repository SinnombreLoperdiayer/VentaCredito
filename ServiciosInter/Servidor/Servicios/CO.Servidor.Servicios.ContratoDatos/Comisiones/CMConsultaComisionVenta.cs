using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.Comisiones
{
  public class CMConsultaComisionVenta
  {
    public long NumeroOperacion { get; set; }

    public long IdCentroServicios { get; set; }

    public int IdServicio { get; set; }

    public decimal ValorBaseComision { get; set; }

    public CMEnumTipoComision TipoComision { get; set; }
  }
}