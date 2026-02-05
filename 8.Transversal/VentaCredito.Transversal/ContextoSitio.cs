using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VentaCredito.Transversal
{
    public class ContextoSitio : IExtension<OperationContext>
    {

        //The "current" custom context
        public static ContextoSitio Current
        {
            get
            {
                return OperationContext.Current == null ? null : OperationContext.Current.Extensions.Find<ContextoSitio>();
            }
        }

        #region IExtension<OperationContext> Members

        public void Attach(OperationContext owner)
        {
            //no-op
        }

        public void Detach(OperationContext owner)
        {
            //no-op
        }

        #endregion IExtension<OperationContext> Members

        string usuario;

        long idCliente;

        private string password;

        private string token;

        public string Token
        {
            get { return token; }
            set { token = value; }
        }


        public string Password
        {
            get { return password; }
            set { password = value; }
        }


        public string Usuario
        {
            get { return usuario; }
            set { usuario = value; }
        }

        public long IdCliente
        {
            get { return idCliente; }
            set { idCliente = value; }
        }

        private string metodo;

        public string Metodo
        {
            get { return metodo; }
            set { metodo = value; }
        }

    }
}
