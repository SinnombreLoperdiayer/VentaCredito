using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Tarifas.Datos.Repositorio
{
    public class ValorPesoDeclaradoDC
    {
        #region Primitive Properties

        [DataMember]
        public int IdValorMinimoDeclarado
        {
            get { return idValorMinimoDeclarado; }
            set
            {
                idValorMinimoDeclarado = value;
            }
        }
        private int idValorMinimoDeclarado;

        [DataMember]
        public int IdListaPrecios
        {
            get { return idListaPrecios; }
            set
            {
                idListaPrecios = value;
            }
        }
    
    private int idListaPrecios;

    [DataMember]
    public decimal PesoInicial
    {
        get { return pesoInicial; }
        set
        {
                pesoInicial = value;
        }
    }
    private decimal pesoInicial;

    [DataMember]
    public decimal PesoFinal
    {
        get { return pesoFinal; }
        set { pesoFinal = value; }
    }
    private decimal pesoFinal;

    [DataMember]
    public decimal ValorMinimoDeclarado
    {
        get { return valorMinimoDeclarado; }
        set
        {
                valorMinimoDeclarado = value;
        }
    }
    private decimal valorMinimoDeclarado;

    [DataMember]
    public decimal ValorMaximoDeclarado
    {
        get { return valorMaximoDeclarado; }
        set
        {
                valorMaximoDeclarado = value;
        }
    }
    private decimal valorMaximoDeclarado;

    [DataMember]
    public System.DateTime FechaGrabacion
    {
        get { return fechaGrabacion; }
        set
        {   fechaGrabacion = value; }
    }
    private System.DateTime fechaGrabacion;

    [DataMember]
    public string CreadoPor
    {
        get { return creadoPor; }
        set { creadoPor = value;  }
        
    }
    private string creadoPor;

    [DataMember]
    public byte[] anchor
    {
        get { return _anchor; }
        set
        {
            if (_anchor != value)
            {
                _anchor = value;
            }
        }
    }
    private byte[] _anchor;

    #endregion
}
}
