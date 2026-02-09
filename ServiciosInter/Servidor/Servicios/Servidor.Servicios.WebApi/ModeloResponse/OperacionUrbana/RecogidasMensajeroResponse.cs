using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.OperacionUrbana
{
    public class RecogidasMensajeroResponse
    {
        List<OURecogidasDC> lstPendientes;

        public List<OURecogidasDC> LstPendientes
        {
            get { return lstPendientes; }
            set { lstPendientes = value; }
        }
        List<OURecogidasDC> lstEfectuadas;

        public List<OURecogidasDC> LstEfectuadas
        {
            get { return lstEfectuadas; }
            set { lstEfectuadas = value; }
        }
        List<OURecogidasDC> lstCanceladasMensajero;

        public List<OURecogidasDC> LstCanceladasMensajero
        {
            get { return lstCanceladasMensajero; }
            set { lstCanceladasMensajero = value; }
        }
        List<OURecogidasDC> lstCanceladasCliente;

        public List<OURecogidasDC> LstCanceladasCliente
        {
            get { return lstCanceladasCliente; }
            set { lstCanceladasCliente = value; }
        }
        List<OURecogidasDC> lstVencidas;

        public List<OURecogidasDC> LstVencidas
        {
            get { return lstVencidas; }
            set { lstVencidas = value; }
        }


    }
}
