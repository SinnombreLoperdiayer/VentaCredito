using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CO.Servidor.Servicios.WebApi.ModeloResponse.OperacionUrbana
{
    public class RecogidasClienteMovilResponse
    {

        List<OURecogidasDC> lstEfectuadas;

        public List<OURecogidasDC> LstEfectuadas
        {
            get { return lstEfectuadas; }
            set { lstEfectuadas = value; }
        }
        List<OURecogidasDC> lstCanceladasCliente;

        public List<OURecogidasDC> LstCanceladasCliente
        {
            get { return lstCanceladasCliente; }
            set { lstCanceladasCliente = value; }
        }
        List<OURecogidasDC> lstProgramadas;

        public List<OURecogidasDC> LstProgramadas
        {
            get { return lstProgramadas; }
            set { lstProgramadas = value; }
        }
        List<OURecogidasDC> lstPendientesPorProgramar;

        public List<OURecogidasDC> LstPendientesPorProgramar
        {
            get { return lstPendientesPorProgramar; }
            set { lstPendientesPorProgramar = value; }
        }

        List<OURecogidasDC> lstPendientesPorReProgramar;

        public List<OURecogidasDC> LstPendientesPorReProgramar
        {
            get { return lstPendientesPorReProgramar; }
            set { lstPendientesPorReProgramar = value; }
        }

    }
}
