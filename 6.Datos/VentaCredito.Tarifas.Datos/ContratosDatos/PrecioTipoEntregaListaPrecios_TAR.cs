using System;
using System.ComponentModel;
using VentaCredito.Tarifas.Datos.ContratosDatos;
using VentaCredito.Tarifas.Datos.Repositorio;

namespace VentaCredito.Tarifas.Datos
{
    public class PrecioTipoEntregaListaPrecios_TAR 
    {
        #region Primitive Properties

        [DataMember]
        public int LPS_IdListaPrecioServicio
        {
            get { return lPS_IdListaPrecioServicio; }
            set
            {
                if (lPS_IdListaPrecioServicio != value)
                {
                    lPS_IdListaPrecioServicio = value;
                }
            }
        }
        private int lPS_IdListaPrecioServicio;

        [DataMember]
        public int LPS_IdServicio
        {
            get { return lPS_IdServicio; }
            set
            {
                if (lPS_IdServicio != value)
                {
                    
                    lPS_IdServicio = value;
                }
            }
        }
        private int lPS_IdServicio;

        [DataMember]
        public int LPS_IdListaPrecios
        {
            get { return lPS_IdListaPrecios; }
            set
            {
                if (lPS_IdListaPrecios != value)
                {
                    
                    lPS_IdListaPrecios = value;
                }
            }
        }
        private int lPS_IdListaPrecios;

        [DataMember]
        public decimal LPS_PrimaSeguros
        {
            get { return lPS_PrimaSeguros; }
            set
            {
                if (lPS_PrimaSeguros != value)
                {
                    
                    lPS_PrimaSeguros = value;
                }
            }
        }
        private  decimal lPS_PrimaSeguros;

        [DataMember]
        public string LPS_Estado
        {
            get { return lPS_Estado; }
            set
            {
                if (lPS_Estado != value)
                {
                    
                    lPS_Estado = value;
                }
            }
        }
        private string lPS_Estado;

        [DataMember]
        public long PTE_IdPrecioTipoEntrega
        {
            get { return pTE_IdPrecioTipoEntrega; }
            set
            {
                if (pTE_IdPrecioTipoEntrega != value)
                {
                    
                    pTE_IdPrecioTipoEntrega = value;
                }
            }
        }
        private long pTE_IdPrecioTipoEntrega;

        [DataMember]
        public string PTE_IdTipoEntrega
        {
            get { return pTE_IdTipoEntrega; }
            set
            {
                if (pTE_IdTipoEntrega != value)
                {
                    
                    pTE_IdTipoEntrega = value;
                }
            }
        }
        private string pTE_IdTipoEntrega;

        [DataMember]
        public int PTE_IdListaPrecioServicio
        {
            get { return pTE_IdListaPrecioServicio; }
            set
            {
                if (pTE_IdListaPrecioServicio != value)
                {
                    
                    pTE_IdListaPrecioServicio = value;
                }
            }
        }
        private int pTE_IdListaPrecioServicio;

        [DataMember]
        public decimal PTE_ValorKiloInicial
        {
            get { return pTE_ValorKiloInicial; }
            set
            {
                if (pTE_ValorKiloInicial != value)
                {
                    
                    pTE_ValorKiloInicial = value;
                }
            }
        }
        private decimal pTE_ValorKiloInicial;

        [DataMember]
        public decimal PTE_ValorKiloAdicional
        {
            get { return pTE_ValorKiloAdicional; }
            set
            {
                if (pTE_ValorKiloAdicional != value)
                {
                    
                    pTE_ValorKiloAdicional = value;
                }
            }
        }
        private decimal pTE_ValorKiloAdicional;

        [DataMember]
        public Nullable<decimal> PTR_Inicial
        {
            get { return pTR_Inicial; }
            set
            {
                if (pTR_Inicial != value)
                {
                    
                    pTR_Inicial = value;
                }
            }
        }
        private Nullable<decimal> pTR_Inicial;

        [DataMember]
        public Nullable<decimal> PTR_Final
        {
            get { return pTR_Final; }
            set
            {
                if (pTR_Final != value)
                {
                    
                    pTR_Final = value;
                }
            }
        }
        private Nullable<decimal> pTR_Final;

        [DataMember]
        public Nullable<long> PTR_IdPrecioTipoEntrega
        {
            get { return pTR_IdPrecioTipoEntrega; }
            set
            {
                if (pTR_IdPrecioTipoEntrega != value)
                {
                    
                    pTR_IdPrecioTipoEntrega = value;
                }
            }
        }
        private Nullable<long> pTR_IdPrecioTipoEntrega;

        #endregion
       
    }
}