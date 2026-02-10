using ServiciosInter.DatosCompartidos.EntidadesNegocio.Giros;
using ServiciosInter.DatosCompartidos.EntidadesNegocio.Mensajeria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosInter.DatosCompartidos.EntidadesNegocio.Facturacion
{
    public class FacturacionRequest
    {
        public ADGuia guia;
        public GIAdmisionGirosDC giro;
    }
}
