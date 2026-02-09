using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.Cajas;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Comisiones;
using CO.Servidor.Dominio.Comun.ControlCuentas;
using CO.Servidor.Dominio.Comun.ExploradorGiros;
using CO.Servidor.Dominio.Comun.Facturacion;
using CO.Servidor.Dominio.Comun.GestionGiros;
using CO.Servidor.Dominio.Comun.LogisticaInversa;
using CO.Servidor.Dominio.Comun.OperacionNacional;
using CO.Servidor.Dominio.Comun.OperacionUrbana;
using CO.Servidor.Dominio.Comun.ParametrosOperacion;
using CO.Servidor.Dominio.Comun.Produccion;
using CO.Servidor.Dominio.Comun.Rutas;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Dominio.Comun.Telemercadeo;
using CO.Servidor.Dominio.Comun.CentroAcopio;
using CO.Servidor.Dominio.Comun.Raps;

namespace CO.Servidor.Dominio.Comun.Middleware
{
    /// <summary>
    /// Fabrica para interfaces entre módulos de la aplicación
    /// </summary>
    public class COFabricaDominio
    {
        private static readonly COFabricaDominio instancia = new COFabricaDominio();
        private Dictionary<Type, object> instancias = new Dictionary<Type, object>();
        private Dictionary<Type, Implementacion> tiposDeInstancias = new Dictionary<Type, Implementacion>();

        private COFabricaDominio()
        {
            tiposDeInstancias.Add(typeof(IPUFachadaCentroServicios), new Implementacion() { NombreAssembly = "CO.Servidor.CentroServicios.dll", NombreClase = "CO.Servidor.CentroServicios.PUFachadaCentroServicios" });
            tiposDeInstancias.Add(typeof(ICLFachadaClientes), new Implementacion() { NombreAssembly = "CO.Servidor.Clientes.dll", NombreClase = "CO.Servidor.Clientes.CLFachadaClientes" });
            tiposDeInstancias.Add(typeof(ISUFachadaSuministros), new Implementacion() { NombreAssembly = "CO.Servidor.Suministros.dll", NombreClase = "CO.Servidor.Suministros.SUFachadaSuministros" });
            tiposDeInstancias.Add(typeof(IADFachadaAdmisionesMensajeria), new Implementacion() { NombreAssembly = "CO.Servidor.Adminisiones.Mensajeria.dll", NombreClase = "CO.Servidor.Adminisiones.Mensajeria.ADFachadaAdmisionesMensajeria" });
            tiposDeInstancias.Add(typeof(ITAFachadaTarifas), new Implementacion() { NombreAssembly = "CO.Servidor.Tarifas.dll", NombreClase = "CO.Servidor.Tarifas.TAFachadaTarifas" });
            tiposDeInstancias.Add(typeof(IFachadaRutas), new Implementacion() { NombreAssembly = "CO.Servidor.Rutas.dll", NombreClase = "CO.Servidor.Rutas.RUFachadaRutas" });
            tiposDeInstancias.Add(typeof(IGIFachadaGestionGiros), new Implementacion() { NombreAssembly = "CO.Servidor.Solicitudes.Giros.dll", NombreClase = "CO.Servidor.Solicitudes.Giros.GIFachadaGestionGiros" });
            tiposDeInstancias.Add(typeof(ICAFachadaCajas), new Implementacion() { NombreAssembly = "CO.Servidor.Cajas.dll", NombreClase = "CO.Servidor.Cajas.CAFachadaCajas" });
            tiposDeInstancias.Add(typeof(IPOFachadaParametrosOperacion), new Implementacion() { NombreAssembly = "CO.Servidor.ParametrosOperacion.dll", NombreClase = "CO.Servidor.ParametrosOperacion.POFachadaParametrosOperacion" });
            tiposDeInstancias.Add(typeof(IGIFachadaExploradorGiros), new Implementacion() { NombreAssembly = "CO.Servidor.ExploradorGiros.dll", NombreClase = "CO.Servidor.ExploradorGiros.GIFachadaExploradorGiros" });
            tiposDeInstancias.Add(typeof(ICMFachadaComisiones), new Implementacion() { NombreAssembly = "CO.Servidor.Comisiones.dll", NombreClase = "CO.Servidor.Comisiones.CMFachadaComisiones" });
            tiposDeInstancias.Add(typeof(ILIFachadaLogisticaInversa), new Implementacion() { NombreAssembly = "CO.Servidor.LogisticaInversa.dll", NombreClase = "CO.Servidor.LogisticaInversa.LIFachadaLogisticaInversa" });
            tiposDeInstancias.Add(typeof(ILIFachadaLogisticaInversaPruebasEntrega), new Implementacion() { NombreAssembly = "CO.Servidor.LogisticaInversa.PruebasEntrega.dll", NombreClase = "CO.Servidor.LogisticaInversa.PruebasEntrega.LIFachadaPruebasEntrega" });
            tiposDeInstancias.Add(typeof(IOUFachadaOperacionUrbana), new Implementacion() { NombreAssembly = "CO.Servidor.OperacionUrbana.dll", NombreClase = "CO.Servidor.OperacionUrbana.OUFachadaOperacionUrbana" });
            tiposDeInstancias.Add(typeof(IONFachadaOperacionNacional), new Implementacion() { NombreAssembly = "CO.Servidor.OperacionNacional.dll", NombreClase = "CO.Servidor.OperacionNacional.ONFachadaOperacionNacional" });
            tiposDeInstancias.Add(typeof(IPRFachadaProduccion), new Implementacion() { NombreAssembly = "CO.Servidor.Produccion.dll", NombreClase = "CO.Servidor.Produccion.PRFachadaProduccion" });
            tiposDeInstancias.Add(typeof(IADFachadaAdmisionesGiros), new Implementacion() { NombreAssembly = "CO.Servidor.Admisiones.Giros.dll", NombreClase = "CO.Servidor.Admisiones.Giros.ADFachadaAdmisionesGiros" });
            tiposDeInstancias.Add(typeof(IFAFachadaFacturacion), new Implementacion() { NombreAssembly = "CO.Servidor.Facturacion.dll", NombreClase = "CO.Servidor.Facturacion.FAFachadaFacturacion" });
            tiposDeInstancias.Add(typeof(ICCFachadaControlCuentas), new Implementacion() { NombreAssembly = "CO.Servidor.ControlCuentas.dll", NombreClase = "CO.Servidor.ControlCuentas.CCFachadaControlCuentas" });
            tiposDeInstancias.Add(typeof(IARFachadaAreas), new Implementacion() { NombreAssembly = "CO.Servidor.Areas.dll", NombreClase = "CO.Servidor.Areas.ARFachadaAreas" });
            tiposDeInstancias.Add(typeof(IGIFachadaTelemercadeo), new Implementacion() { NombreAssembly = "CO.Servidor.Telemercadeo.Giros.dll", NombreClase = "CO.Servidor.Telemercadeo.Giros.GIFachadaTelemercadeo" });
            tiposDeInstancias.Add(typeof(ICAFachadaCentroAcopio), new Implementacion() { NombreAssembly = "CO.Servidor.CentroAcopio.dll", NombreClase = "CO.Servidor.CentroAcopio.CAFachadaCentroAcopio" });
            tiposDeInstancias.Add(typeof(IRAFachadaRaps), new Implementacion() { NombreAssembly = "CO.Servidor.Raps.dll", NombreClase = "CO.Servidor.Raps.RAFachadaRaps" });
        }

        public static COFabricaDominio Instancia
        {
            get { return instancia; }
        }

        /// <summary>
        /// Crea las instancias según la interface que recibe por parámetro
        /// </summary>
        /// <typeparam name="TModulo">Interface a implementar</typeparam>
        /// <returns>Clase concreta que implementa la interface TModulo</returns>
        public TModulo CrearInstancia<TModulo>()
        {
            object fachada = null;
            Type tipoModulo = typeof(TModulo);
            instancias.TryGetValue(tipoModulo, out fachada);

            if (fachada == null)
            {
                Implementacion implem = null;
                tiposDeInstancias.TryGetValue(tipoModulo, out implem);
                if (implem != null)
                {
                    string path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

                    Assembly ass = Assembly.LoadFrom(Path.Combine(path, implem.NombreAssembly));
                    fachada = ass.CreateInstance(implem.NombreClase);

                    lock (this)
                    {
                        if (!instancias.ContainsKey(tipoModulo))
                            instancias.Add(tipoModulo, fachada);
                    }
                }
            }
            return (TModulo)fachada;
        }
    }

    internal class Implementacion
    {
        public string NombreAssembly { get; set; }

        public string NombreClase { get; set; }
    }
}