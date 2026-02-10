using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas.Novasoft
{
    [DataContract(Namespace = "http://contrologis.com")]
    public class CATipoCuentaDC
    {
        private string codigoCuenta;
        [DataMember]
        public string CodigoCuenta
        {
            get { return codigoCuenta; }
            set { codigoCuenta = value;}
        }

        private string nombreCuenta;
        [DataMember]
        public string NombreCuenta
        {
            get { return nombreCuenta; }
            set { nombreCuenta = value; }
        }

        private string codigoNombreCuenta;
        [DataMember]
        public string CodigoNombreCuenta
        {
            get { return codigoNombreCuenta; }
            set { codigoNombreCuenta = value; }
        }
    }
}
