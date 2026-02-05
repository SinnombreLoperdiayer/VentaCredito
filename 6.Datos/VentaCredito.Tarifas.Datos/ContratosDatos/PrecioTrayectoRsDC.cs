using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Tarifas.Datos.ContratosDatos
{
    public class PrecioTrayectoRsDC
    {

        #region Primitive Properties

        public long TRA_IdTrayecto { get; set; }
                
        public string TRA_IdLocalidadOrigen
        {
            get; set;
        }
        

        
        public int TRA_IdTrayectoSubTrayecto
        {   get; set;
        }
        
        public string TRA_IdLocalidadDestino
        {
            get;    set;
        }
        
        public string TRS_IdTipoSubTrayecto
        {
            get;
            set;
        }

        public string TRS_IdTipoTrayecto
        {
            get;
            set;
        }

        public long STR_IdTrayecto
        {
            get;
            set;
        }

        public int STR_IdServicio
        {
            get ;
            set;
        }

        public int PTR_IdTrayectoSubTrayecto
        {
            get;
            set;
        }

        public decimal PTR_ValorFijo
        {
            get;
            set;
        }

        public int PTR_IdListaPrecioServicio
        {
            get;
            set;
        }
        private int _pTR_IdListaPrecioServicio;

        #endregion

    }
}
