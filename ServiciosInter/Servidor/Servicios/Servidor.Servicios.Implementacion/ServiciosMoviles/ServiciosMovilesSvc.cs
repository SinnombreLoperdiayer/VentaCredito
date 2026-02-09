using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using CO.Servidor.GestionGiro.ClienteConvenio;
using CO.Servidor.ServicioalCliente;
using CO.Servidor.Servicios.ContratoDatos.ServicioalCliente;
using CO.Servidor.Servicios.Contratos;
using Framework.Servidor.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.OperacionUrbana;
using Framework.Servidor.Excepciones;


namespace CO.Servidor.Servicios.Implementacion.ServiciosMoviles
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ServiciosMovilesSvc : IServiciosMovilesSvc
    {
        #region Operacion Urbana

        public IList<OUMensajeroDC> ObtenerMensajeroCentroLogistico(long centroLogistico) 
        {
            int totalRegistros = 0;
            Dictionary<string, string> filtro = new Dictionary<string, string>(); 
            return  OUAdministradorOperacionUrbana.Instancia.ObtenerMensajeroCentroLogistico(filtro, string.Empty, 0, 1000, true, out totalRegistros, centroLogistico);              
        }

        public List<OUNovedadIngresoDC> ObtenerNovedadesIngreso()
        {
            return OUAdministradorOperacionUrbana.Instancia.ObtenerNovedadesIngreso();
        }

        public OUPlanillaVentaGuiasDC IngresarGuiaCentroAcopio(string numeroGuia, string idCiudadOrigen, string idPuntoOrigen, string idMensajero,string idNovedad , string usuario)
        {
            OUPlanillaVentaGuiasDC guia = new OUPlanillaVentaGuiasDC()
            {
                NumeroGuia = Convert.ToInt64(numeroGuia),
                IdPuntoServicio = Convert.ToInt64(idPuntoOrigen),
                IdCiudadOrigenGuia = idCiudadOrigen,
                Mensajero = new OUMensajeroDC { IdMensajero = Convert.ToInt64(idMensajero) }

            };
            List<OUNovedadIngresoDC> listaNovedades = new List<OUNovedadIngresoDC>();
            if (idNovedad !="8")
                listaNovedades.Add(new OUNovedadIngresoDC { IdNovedad = Convert.ToInt16(idNovedad), Asignado = true });

            OperationContext.Current.Extensions.Add(new ControllerContext() { Usuario = usuario });
            return OUAdministradorOperacionUrbana.Instancia.IngresarGuiaSuelta(guia, listaNovedades);
        }

        #endregion
    }
}
