using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.ControlCuentas
{
   public class CCGuiaValidacion
   {
      public long NumeroGuia { get; set; }

      public decimal? ValorTransporte { get; set; }

      public decimal? ValorPrima { get; set; }

      public decimal? ValorTotal { get; set; }

      public string IdTipoEntrega { get; set; }

      public int IdServicio { get; set; }

      public string IdCiudadOrigen { get; set; }

      public string IdCiudadDestino { get; set; }

      public decimal Peso { get; set; }

      public decimal ValorDeclarado { get; set; }

      public int? IdListaPreciosPeatonConvenio { get; set; }

      public int? IdListaPreciosConvenioPeaton { get; set; }

      public int? IdListaPreciosConvenioConvenio { get; set; }

      public int? IdListaPrecios
      {
         get
         {
            if (IdListaPreciosConvenioPeaton.HasValue) return IdListaPreciosConvenioPeaton;
            else if (IdListaPreciosConvenioConvenio.HasValue) return IdListaPreciosConvenioConvenio;
            else return IdListaPreciosPeatonConvenio;
         }
      }
   }
}
