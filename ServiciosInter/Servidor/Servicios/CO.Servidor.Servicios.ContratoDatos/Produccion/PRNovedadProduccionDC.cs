using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PRNovedadProduccionDC : IEditableObject, INotifyPropertyChanged, IDataErrorInfo
    {
        private long idNovedadProduccion;

        private short idMotivoNovedad;

        private long idCentroServicios;

        private int idServicio;

        private decimal valor;

        private int cantidad;

        private string observacion;

        private DateTime fechaGrabacion;

        private string creadoPor;

        private bool cargada;

        private int mesVigencia;

        private int anoVigencia;

        private string[] codCentrosServicios;

        public PRNovedadProduccionDC()
        {

        }

        public PRNovedadProduccionDC(string[] codCentrosServicios)
        {
            this.codCentrosServicios = codCentrosServicios;
        }

        [DataMember]
        public long IdNovedadProduccion
        {
            get
            {
                return idNovedadProduccion;
            }
            set
            {
                if (idNovedadProduccion == value) return;
                idNovedadProduccion = value;
                OnPropertyChanged("IdNovedadProduccion");
            }
        }

        [DataMember]
        public short IdMotivoNovedad
        {
            get
            {
                return idMotivoNovedad;
            }
            set
            {
                if (idMotivoNovedad == value) return;
                idMotivoNovedad = value;
                OnPropertyChanged("IdMotivoNovedad");
            }
        }

        [DataMember]
        public string DescMotivo
        {
            get;
            set;
        }

        [DataMember]
        public long IdCentroServicios
        {
            get
            {
                return idCentroServicios;
            }
            set
            {
                if (idCentroServicios == value) return;
                idCentroServicios = value;
                OnPropertyChanged("IdCentroServicios");
            }
        }

        [DataMember]
        public string DescCentroServicios
        {
            get;
            set;
        }

        [DataMember]
        public int IdServicio
        {
            get
            {
                return idServicio;
            }
            set
            {
                if (idServicio == value) return;
                idServicio = value;
                OnPropertyChanged("IdServicio");
            }
        }

        [DataMember]
        public string DescServicio
        {
            get;
            set;
        }

        [DataMember]
        public decimal Valor
        {
            get
            {
                return valor;
            }
            set
            {
                if (valor == value) return;
                valor = value;
                OnPropertyChanged("Valor");
            }
        }

        [DataMember]
        public int Cantidad
        {
            get
            {
                return cantidad;
            }
            set
            {
                if (cantidad == value) return;
                cantidad = value;
                OnPropertyChanged("Cantidad");
            }
        }

        [DataMember]
        public string Observacion
        {
            get
            {
                return observacion;
            }
            set
            {
                if (observacion == value) return;
                observacion = value;
                OnPropertyChanged("Observacion");
            }
        }

        [DataMember]
        public DateTime FechaGrabacion
        {
            get
            {
                return fechaGrabacion;
            }
            set
            {
                if (fechaGrabacion == value) return;
                fechaGrabacion = value;
                OnPropertyChanged("FechaGrabacion");
            }
        }

        [DataMember]
        public string CreadoPor
        {
            get
            {
                return creadoPor;
            }
            set
            {
                if (creadoPor == value) return;
                creadoPor = value;
                OnPropertyChanged("CreadoPor");
            }
        }

        [DataMember]
        public bool Cargada
        {
            get
            {
                return cargada;
            }
            set
            {
                if (cargada == value) return;
                cargada = value;
                OnPropertyChanged("Cargada");
            }
        }

        public bool Modificable { get { return !Cargada; } }

        [DataMember]
        public int MesVigencia
        {
            get
            {
                return mesVigencia;
            }
            set
            {
                if (mesVigencia == value) return;
                mesVigencia = value;
                OnPropertyChanged("MesVigencia");
            }
        }

        [DataMember]
        public int AnoVigencia
        {
            get
            {
                return anoVigencia;
            }
            set
            {
                if (anoVigencia == value) return;
                anoVigencia = value;
                OnPropertyChanged("AnoVigencia");
            }
        }

        public string[] CodCentrosServicios
        {
            get
            {
                return codCentrosServicios;
            }
            set
            {
                if (codCentrosServicios == value) return;
                codCentrosServicios = value;
                OnPropertyChanged("CodCentrosServicios");
            }
        }


        #region IEditableObject

        private PRNovedadProduccionDC backupCopy;
        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit) return;
            inEdit = true;
            backupCopy = this.MemberwiseClone() as PRNovedadProduccionDC;
        }

        public void CancelEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            this.IdNovedadProduccion = backupCopy.idNovedadProduccion;

            this.IdMotivoNovedad = backupCopy.IdMotivoNovedad;

            this.IdCentroServicios = backupCopy.IdCentroServicios;

            this.IdServicio = backupCopy.IdServicio;

            this.Valor = backupCopy.Valor;

            this.Cantidad = backupCopy.Cantidad;

            this.Observacion = backupCopy.Observacion;

            this.FechaGrabacion = backupCopy.FechaGrabacion;

            this.CreadoPor = backupCopy.CreadoPor;

            this.Cargada = backupCopy.Cargada;

            this.MesVigencia = backupCopy.MesVigencia;

            this.AnoVigencia = backupCopy.AnoVigencia;
        }

        public void EndEdit()
        {
            if (!inEdit) return;
            inEdit = false;
            backupCopy = null;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                //this get will be invoked everytime user change the Name or Age filed in Datagird
                //columnName contains the property name which is modified by user.
                string error = string.Empty;
                switch (columnName)
                {


                    case "IdCentroServicios":
                        if (codCentrosServicios != null && !codCentrosServicios.Contains(IdCentroServicios.ToString()))
                            error = "El Id de centro de servicios no existe o no está activo.";
                        break;
                    case "Valor":
                        //if user changes the name field, I check if the new value is empty or not
                        //if it is empty I set the error message accordingly.
                        if (Valor<=0)
                            error = "El valor debe ser mayor a cero (0).";
                        break;

                    case "Cantidad":
                        //if user change the Age, I verify that Age is greater than 20,
                        //if not I set the error message.
                        if (Cantidad <= 0)
                            error = "La cantidad debe ser mayor a cero (0).";
                        break;
                }
                //just return the error or empty string if there is no error
                return error;
            }
        }

        #endregion

    }
}
