using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;
using CO.Servidor.Admisiones.Giros.Comun;
using CO.Servidor.Admisiones.Giros.Datos;
using CO.Servidor.Admisiones.Giros.TransaccionCaja;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Clientes;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Giros.Venta;
using CO.Servidor.Servicios.ContratoDatos.Clientes;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System.IO;
using System.Configuration;
using System;

namespace CO.Servidor.Admisiones.Giros.Venta
{
  /// <summary>
  /// Clase para manejar la admisión de giros para cliente convenio
  /// </summary>
  internal class GIAdmisionGiroConvenio : ControllerBase
  {
      /// <summary>
      /// Path almacena imagenes scanneadas
      /// </summary>
      private string filePath = ConfigurationManager.AppSettings["Controller.RutaDescargaArchivos"];

    #region CrearInstancia

    private static readonly GIAdmisionGiroConvenio instancia = (GIAdmisionGiroConvenio)FabricaInterceptores.GetProxy(new GIAdmisionGiroConvenio(), COConstantesModulos.GIROS);

    /// <summary>
    /// Retorna una instancia de centro Servicios
    /// /// </summary>
    public static GIAdmisionGiroConvenio Instancia
    {
      get { return GIAdmisionGiroConvenio.instancia; }
    }

    #endregion CrearInstancia

    /// <summary>
    /// Guardar los giros convenio.
    /// </summary>
    public GINumeroGiro GuardarGiroPeatonConvenio(GIAdmisionGirosDC giro)
    {
      SUNumeradorPrefijo numeroSuministro = null;
      bool obligaDeclaracionVoluntariaFondos = false;

      using (TransactionScope transaccion = new TransactionScope())
      {
        // Se obtiene el número del giro
        ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();
        numeroSuministro = fachadaSuministros.ObtenerNumeroPrefijoValor(SUEnumSuministro.FACTURA_VENTA_GIRO_POSTAL_AUTOMATICO);
        transaccion.Complete();
      }

      ValidarAgenciaServicioGiros(giro.AgenciaOrigen.IdCentroServicio);
      using (TransactionScope transaccion = new TransactionScope())
      {
        GINumeroGiro numeroGiro = null;

        COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().AcumularVentaGirosAgencia(giro);
        decimal valorAcumulado = COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().GuardarClienteAcumularValores
          (giro.GirosPeatonConvenio.ClienteContado, giro.Precio.ValorGiro);

        if (giro.ArchivoDeclaracionVoluntariaOrigenes == null)
        {
          //Se cambio la logia para que la validacion no fuera por acumulado sino por valor del giro
          if (GIRepositorio.Instancia.ValidarDeclaracionFondos(giro.Precio.ValorGiro))
            obligaDeclaracionVoluntariaFondos = true;
        }
        else
        {
          giro.ArchivoDeclaracionVoluntariaOrigenes = GuardarImagenCarpeta(giro.ArchivoDeclaracionVoluntariaOrigenes);
          giro.DeclaracionVoluntariaOrigenes = GIRepositorio.Instancia.AlmacenarDeclaracionFondos(giro.ArchivoDeclaracionVoluntariaOrigenes, giro);
        }

        if (giro.NumeroGiroAgenciaManual > 0)
        {
          // el giro proviene de una agencia manual
          giro.IdGiro = giro.NumeroGiroAgenciaManual;
        }
        else
        {
          giro.IdGiro = numeroSuministro.ValorActual;
          giro.PrefijoIdGiro = numeroSuministro.Prefijo;
        }

        // Consulta la informacion de  la agencia destino
        giro.AgenciaDestino = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ObtenerCentroServicio(giro.AgenciaDestino.IdCentroServicio);
        PALocalidadDC localidad = Framework.Servidor.ParametrosFW.PAAdministrador.Instancia.ObtenerInformacionLocalidad(giro.AgenciaDestino.IdMunicipio);
        giro.AgenciaDestino.NombreDepto = localidad.NombreCompleto;
        giro.AgenciaDestino.NombreMunicipio = localidad.Nombre;
        giro.AgenciaDestino.CodigoPostal = localidad.CodigoPostal;

        numeroGiro = GIRepositorio.Instancia.GuardarGiroPeatonConvenio(giro);
        numeroGiro.PrefijoIdGiro = giro.PrefijoIdGiro;
        numeroGiro.ObligaDeclaracionVoluntariaFondos = obligaDeclaracionVoluntariaFondos;
        GITransaccionCaja.EnviarTransaccionCajaVentaGiroConvenio(giro);
        transaccion.Complete();
        return numeroGiro;
      }
    }

    /// <summary>
    /// Valida que una agencia pueda realizar la venta de un giro
    /// </summary>
    /// <param name="idCentroServicios">Codigo Centro Servicios</param>
    private void ValidarAgenciaServicioGiros(long idCentroServicios)
    {
      COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>().ValidarAgenciaServicioGiros(idCentroServicios);
    }

    /// <summary>
    /// Obtiene el valor total,servicio,tarifas de un giro dirigido a un cliente contado a partir de un contrato
    /// </summary>
    /// <returns>Colección de precio rango para una lista de precio servicio</returns>
    public TAPrecioDC ObtenerValorGiroClienteContadoGiro(int idContrato, decimal valor)
    {
      return COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerValorGiroClienteContadoGiro(idContrato, valor);
    }

    /// <summary>
    ///  Obtiene el valor a cobrar a un servicio segun su  valor total.
    /// </summary>
    public TAPrecioDC CalcularValorServicoAPartirValorTotal(int idContrato, decimal valorTotal)
    {
      return COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().CalcularValorServicoAPartirValorTotal(idContrato, valorTotal);
    }

    /// <summary>
    /// Obtener los impuestos de el servicio de giros
    /// </summary>
    /// <returns></returns>
    public TAServicioImpuestosDC ObtenerImpuestosGiros()
    {
      return COFabricaDominio.Instancia.CrearInstancia<ITAFachadaTarifas>().ObtenerImpuestosGiros();
    }

    /// <summary>
    /// Consulta los cliente creditos con sus respectivos contratos, y Si puede realizar giros convenio o dispersion de fondos
    /// </summary>
    /// <returns>Colección clientes y contratos</returns>
    public List<CLClientesDC> ObtenerClientesContratosGiros()
    {
      return COFabricaDominio.Instancia.CrearInstancia<ICLFachadaClientes>().ObtenerClientesContratosGiros();
    }
    /// <summary>
    /// Método para guardar las imagenes en la carpeta compartida
    /// </summary>
    /// <param name="rutaArchivo"></param>
    public string GuardarImagenCarpeta(string rutaArchivo)
    {
        string rutaImagenes = Framework.Servidor.ParametrosFW.PAParametros.Instancia.ConsultarParametrosFramework("FolderImagenGiros");
        string carpetaDestino = Path.Combine(rutaImagenes + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year);
        string rutaDestino = Path.Combine(carpetaDestino, rutaArchivo);
        if (!Directory.Exists(carpetaDestino))
        {
            Directory.CreateDirectory(carpetaDestino);
        }

        lock (this)
        {
            File.Move(Path.Combine(this.filePath, COConstantesModulos.GIROS, rutaArchivo), rutaDestino);
        }
        return rutaDestino;
    }

  }
}