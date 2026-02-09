using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CAServiciosFormaPagoDC : INotifyPropertyChanged
    {

        /// <summary>
        /// IdServicio
        /// </summary>
        [DataMember]
        public long IdServicio { get; set; }
        
        /// <summary>
        /// Enumeración que indica el estado del objeto dentro de una lista
        /// </summary>
        [DataMember]
        public EnumEstadoRegistro EstadoRegistro { get; set; }

        /// <summary>
        /// Objeto Tipo Servicio : TAServicioDC
        /// </summary>
        private TAServicioDC tipoServicio;
        [DataMember]
        public TAServicioDC TipoServicio
        {
            get { return tipoServicio; }
            set { tipoServicio = value; OnPropertyChanged("TipoServicio"); }
        }
        /// <summary>
        /// Objeto TAFormaPago : TAFormaPago
        /// </summary>
        private TAFormaPago formaPagoNova;
        [DataMember]
        public TAFormaPago FormaPagoNova
        {
            get { return formaPagoNova; }
            set
            {
                if (value != null && value.ServiciosAsociados != null)
                {
                    LstTiposServicio = new ObservableCollection<TAServicioDC>(value.ServiciosAsociados);
                }
                formaPagoNova = value;
                OnPropertyChanged("FormaPagoNova");
            }
        }
        /// <summary>
        /// Lista Formas Pago
        /// </summary>
        private ObservableCollection<TAFormaPago> lstformaPago;
        [DataMember]
        public ObservableCollection<TAFormaPago> LstFormaPago
        {
            get { return lstformaPago; }
            set
            {

                lstformaPago = value;
                OnPropertyChanged("LstFormaPago");

            }
        }
        /// <summary>
        /// Lista Tipo Servicios
        /// </summary>
        private ObservableCollection<TAServicioDC> lstTiposServicio;
        public ObservableCollection<TAServicioDC> LstTiposServicio
        {
            get { return lstTiposServicio; }
            set { lstTiposServicio = value; OnPropertyChanged("LstTiposServicio"); }
        }

        /// <summary>
        /// Tipo Cuenta Ventas Debito
        /// </summary>
        private CATipoCuentaDC tipoCuentaVD;
        [DataMember]
        public CATipoCuentaDC TipoCuentaVD
        {
            get { return tipoCuentaVD; }
            set { tipoCuentaVD = value; OnPropertyChanged("TipoCuentaVD"); }
        }


        /// <summary>
        /// Tipo Cuenta ventas Credito
        /// </summary>
        private CATipoCuentaDC tipoCuentaVC;
        [DataMember]
        public CATipoCuentaDC TipoCuentaVC
        {
            get { return tipoCuentaVC; }
            set { tipoCuentaVC = value; OnPropertyChanged("TipoCuentaVC"); }
        }


        /// <summary>
        /// Tipo Cuenta Efectiva Debito Debito
        /// </summary>
        private CATipoCuentaDC tipoCuentaEDD;
        [DataMember]
        public CATipoCuentaDC TipoCuentaEDD
        {
            get { return tipoCuentaEDD; }
            set { tipoCuentaEDD = value; OnPropertyChanged("TipoCuentaEDD"); }
        }

        /// <summary>
        /// Tipo Cuenta Efectivas Debito Credito
        /// </summary>
        private CATipoCuentaDC tipoCuentaEDC;
        [DataMember]
        public CATipoCuentaDC TipoCuentaEDC
        {
            get { return tipoCuentaEDC; }
            set { tipoCuentaEDC = value; OnPropertyChanged("TipoCuentaEDC"); }
        }

        /// <summary>
        /// Tipo Cuenta Efectivas Credito Credito
        /// </summary>
        private CATipoCuentaDC tipoCuentaECC;
        [DataMember]
        public CATipoCuentaDC TipoCuentaECC
        {
            get { return tipoCuentaECC; }
            set { tipoCuentaECC = value; OnPropertyChanged("TipoCuentaECC"); }
        }
        /// <summary>
        /// Tipo Cuenta Efectivas Credito Debito
        /// </summary>
        private CATipoCuentaDC tipoCuentaECD;
        [DataMember]
        public CATipoCuentaDC TipoCuentaECD
        {
            get { return tipoCuentaECD; }
            set { tipoCuentaECD = value; OnPropertyChanged("TipoCuentaECD"); }
        }
        /// <summary>
        /// Lista Tipos Cuenta Novasoft
        /// </summary>
        private ObservableCollection<CATipoCuentaDC> lstTipoCuentaNova;
        [DataMember]
        public ObservableCollection<CATipoCuentaDC> LstTipoCuentaNova
        {
            get { return lstTipoCuentaNova; }
            set { lstTipoCuentaNova = value; OnPropertyChanged("LstTipoCuentaNova"); }
        }
      

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Se genera en el evento de cambio del valor de una propiedad
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cuyo valor cambia.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
    }
}
