using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Tarifas.Datos.Repositorio;

namespace VentaCredito.Tarifas.Datos.ContratosDatos
{
    public class PrecioTrayectoDC  
    {
        #region Primitive Properties

        [DataMember]
        public long TRA_IdTrayecto
        {
            get { return tRA_IdTrayecto; }
            set
            {
                if (tRA_IdTrayecto != value)
                {
                    tRA_IdTrayecto = value;
                }
            }
        }
        private long tRA_IdTrayecto;

        [DataMember]
        public string TRA_IdLocalidadOrigen
        {
            get { return tRA_IdLocalidadOrigen; }
            set
            {
                if (tRA_IdLocalidadOrigen != value)
                {
                    tRA_IdLocalidadOrigen = value;
                }
            }
        }
        private string tRA_IdLocalidadOrigen;

        [DataMember]
        public int TRA_IdTrayectoSubTrayecto
        {
            get { return tRA_IdTrayectoSubTrayecto; }
            set
            {
                if (tRA_IdTrayectoSubTrayecto != value)
                {
                    tRA_IdTrayectoSubTrayecto = value;
                }
            }
        }
        private int tRA_IdTrayectoSubTrayecto;

        [DataMember]
        public string TRA_IdLocalidadDestino
        {
            get { return tRA_IdLocalidadDestino; }
            set
            {
                if (tRA_IdLocalidadDestino != value)
                {
                    tRA_IdLocalidadDestino = value;
                }
            }
        }
        private string tRA_IdLocalidadDestino;

        [DataMember]
        public string TRS_IdTipoSubTrayecto
        {
            get { return tRS_IdTipoSubTrayecto; }
            set
            {
                if (tRS_IdTipoSubTrayecto != value)
                {
                    tRS_IdTipoSubTrayecto = value;
                }
            }
        }
        private string tRS_IdTipoSubTrayecto;

        [DataMember]
        public string TRS_IdTipoTrayecto
        {
            get { return tRS_IdTipoTrayecto; }
            set
            {
                if (tRS_IdTipoTrayecto != value)
                {
                    tRS_IdTipoTrayecto = value;
                }
            }
        }
        private string tRS_IdTipoTrayecto;

        [DataMember]
        public long STR_IdTrayecto
        {
            get { return sTR_IdTrayecto; }
            set
            {
                if (sTR_IdTrayecto != value)
                {
                    sTR_IdTrayecto = value;
                }
            }
        }
        private long sTR_IdTrayecto;

        [DataMember]
        public int STR_IdServicio
        {
            get { return sTR_IdServicio; }
            set
            {
                if (sTR_IdServicio != value)
                {
                    sTR_IdServicio = value;
                }
            }
        }
        private int sTR_IdServicio;

        [DataMember]
        public int PTR_IdTrayectoSubTrayecto
        {
            get { return pTR_IdTrayectoSubTrayecto; }
            set
            {
                if (pTR_IdTrayectoSubTrayecto != value)
                {
                    pTR_IdTrayectoSubTrayecto = value;
                }
            }
        }
        private int pTR_IdTrayectoSubTrayecto;

        [DataMember]
        public decimal PTR_ValorFijo
        {
            get { return pTR_ValorFijo; }
            set
            {
                if (pTR_ValorFijo != value)
                {
                    pTR_ValorFijo = value;
                }
            }
        }
        private decimal pTR_ValorFijo;

        [DataMember]
        public int PTR_IdListaPrecioServicio
        {
            get { return pTR_IdListaPrecioServicio; }
            set
            {
                if (pTR_IdListaPrecioServicio != value)
                {
                    pTR_IdListaPrecioServicio = value;
                }
            }
        }
        private int pTR_IdListaPrecioServicio;

        #endregion

       
    }
}
