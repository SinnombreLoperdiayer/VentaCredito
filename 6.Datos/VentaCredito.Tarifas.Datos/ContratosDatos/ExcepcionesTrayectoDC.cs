using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VentaCredito.Tarifas.Datos.Repositorio;

namespace VentaCredito.Tarifas.Datos.ContratosDatos
{
    public class ExcepcionesTrayectoDC
    {
        #region Primitive Properties

        [DataMember]
        public long SET_IdPrecioServicioExcepionTrayecto
        {
            get { return sET_IdPrecioServicioExcepionTrayecto; }
            set
            {
                if (sET_IdPrecioServicioExcepionTrayecto != value)
                {
                    sET_IdPrecioServicioExcepionTrayecto = value;
                }
            }
        }
        private long sET_IdPrecioServicioExcepionTrayecto;

        [DataMember]
        public int SET_IdListaPrecioServicio
        {
            get { return sET_IdListaPrecioServicio; }
            set
            {
                if (sET_IdListaPrecioServicio != value)
                {
                    sET_IdListaPrecioServicio = value;
                }
            }
        }
        private int sET_IdListaPrecioServicio;

        [DataMember]
        public string SET_IdLocalidadOrigen
        {
            get { return sET_IdLocalidadOrigen; }
            set
            {
                if (sET_IdLocalidadOrigen != value)
                {
                    sET_IdLocalidadOrigen = value;
                }
            }
        }
        private string sET_IdLocalidadOrigen;

        [DataMember]
        public string SET_IdLocalidadDestino
        {
            get { return sET_IdLocalidadDestino; }
            set
            {
                if (sET_IdLocalidadDestino != value)
                {
                    
                    sET_IdLocalidadDestino = value;
                }
            }
        }
        private string sET_IdLocalidadDestino;

        [DataMember]
        public System.DateTime SET_FechaGrabacion
        {
            get { return sET_FechaGrabacion; }
            set
            {
                if (sET_FechaGrabacion != value)
                {
                    
                    sET_FechaGrabacion = value;
                }
            }
        }
        private System.DateTime sET_FechaGrabacion;

        [DataMember]
        public string SET_CreadoPor
        {
            get { return sET_CreadoPor; }
            set
            {
                if (sET_CreadoPor != value)
                {
                    
                    sET_CreadoPor = value;
                }
            }
        }
        private string sET_CreadoPor;

        [DataMember]
        public decimal SET_ValorKiloInicial
        {
            get { return sET_ValorKiloInicial; }
            set
            {
                if (sET_ValorKiloInicial != value)
                {
                    
                    sET_ValorKiloInicial = value;
                }
            }
        }
        private decimal sET_ValorKiloInicial;

        [DataMember]
        public decimal SET_ValorKiloAdicional
        {
            get { return sET_ValorKiloAdicional; }
            set
            {
                if (sET_ValorKiloAdicional != value)
                {
                    
                    sET_ValorKiloAdicional = value;
                }
            }
        }
        private decimal sET_ValorKiloAdicional;

        [DataMember]
        public Nullable<bool> SET_EsOrigenTodoElPais
        {
            get { return sET_EsOrigenTodoElPais; }
            set
            {
                if (sET_EsOrigenTodoElPais != value)
                {
                    
                    sET_EsOrigenTodoElPais = value;
                }
            }
        }
        private Nullable<bool> sET_EsOrigenTodoElPais;

        [DataMember]
        public Nullable<bool> SET_EsDestinoTodoElPais
        {
            get { return sET_EsDestinoTodoElPais; }
            set
            {
                if (sET_EsDestinoTodoElPais != value)
                {
                    
                    sET_EsDestinoTodoElPais = value;
                }
            }
        }
        private Nullable<bool> sET_EsDestinoTodoElPais;

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
        private decimal lPS_PrimaSeguros;

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

        #endregion

       
    }
}
