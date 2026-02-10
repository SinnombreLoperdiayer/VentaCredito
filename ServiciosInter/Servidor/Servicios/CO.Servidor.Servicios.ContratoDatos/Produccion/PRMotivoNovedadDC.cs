using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CO.Servidor.Servicios.ContratoDatos.Produccion
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class PRMotivoNovedadDC : IEditableObject, INotifyPropertyChanged
    {
        private short idMotivoNovedad;

        private PREnumTipoNovedadDC tipoNovedad;

        private string descripcion;

        private DateTime fechaGrabacion;

        private string creadoPor;


        PRMotivoNovedadDC _OriginalObject;
        bool _Editing;

        [DataMember]
        public short IdMotivoNovedad { get { return idMotivoNovedad; } set { idMotivoNovedad = value; NotifyPropertyChanged("IdMotivoNovedad"); } }

        [DataMember]
        public short IdMotivoNovedadOriginal { get; set; }

        [DataMember]
        public PREnumTipoNovedadDC TipoNovedad { get { return tipoNovedad; } set { tipoNovedad = value; NotifyPropertyChanged("TipoNovedad"); } }

        [DataMember]
        public string Descripcion { get { return descripcion; } set { descripcion = value; NotifyPropertyChanged("Descripcion"); } }

        [DataMember]
        public DateTime FechaGrabacion { get { return fechaGrabacion; } set { fechaGrabacion = value; NotifyPropertyChanged("FechaGrabacion"); } }

        [DataMember]
        public string CreadoPor { get { return creadoPor; } set { creadoPor = value; NotifyPropertyChanged("CreadoPor"); } }

        [DataMember]
        public bool Seleccionado { get; set; }

        public PRMotivoNovedadDC() { }

        public void BeginEdit()
        {
            if (!_Editing)
            {
                _Editing = true;
                _OriginalObject = this.MemberwiseClone() as PRMotivoNovedadDC;
            }
        }

        public void CancelEdit()
        {
            if (_Editing)
            {
                IdMotivoNovedad = _OriginalObject.IdMotivoNovedad;
                TipoNovedad = _OriginalObject.TipoNovedad;
                Descripcion = _OriginalObject.Descripcion;
                FechaGrabacion = _OriginalObject.FechaGrabacion;
                CreadoPor = _OriginalObject.CreadoPor;
                _Editing = false;
            }
        }
        public void EndEdit()
        {
            if (_Editing)
            {
                _Editing = false;
                _OriginalObject = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}