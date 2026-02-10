using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using CO.Servidor.Cajas.Apertura;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Dominio.Comun.Area;
using CO.Servidor.Dominio.Comun.CentroServicios;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Dominio.Comun.Suministros;
using CO.Servidor.Dominio.Comun.Tarifas;
using CO.Servidor.Servicios.ContratoDatos.Area;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using CO.Servidor.Servicios.ContratoDatos.Cajas.Transacciones;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas.Impresion;
using CO.Servidor.Servicios.ContratoDatos.Suministros;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;
using Framework.Servidor.ParametrosFW;
using Framework.Servidor.Servicios.ContratoDatos.Parametros.Consecutivos;

namespace CO.Servidor.Cajas.CajaFinanciera
{
  /// <summary>
  /// Clase que realiza los diferentes movimientos de la caja financiera modulo gestion de Cajas cajas entre Banco - RACOL - Casa Matriz
  /// </summary>
  internal class CACajaFinanciera : ControllerBase
  {
    #region Atributos

    private static readonly CACajaFinanciera instancia = (CACajaFinanciera)FabricaInterceptores.GetProxy(new CACajaFinanciera(), COConstantesModulos.CAJA);

    #endregion Atributos

    #region Instancia

    public static CACajaFinanciera Instancia
    {
      get { return CACajaFinanciera.instancia; }
    }

    #endregion Instancia

    #region Transacciones Banco-Casa Matriz-CentroSvc-Operación Nacional

    /// <summary>
    /// Transacciones entre RACOL- Agencia - Banco - Casa Matriz.
    /// </summary>
    /// <param name="infoTransaccion">The info transaccion.</param>
    /// <param name="tipoTransaccion">The tipo transaccion.</param>
    public CAResultadoRegistroTransaccionDC TransaccionRacolBancoCasaMatriz(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      CAResultadoRegistroTransaccionDC resultadoRegistro = null;


      if (infoTransaccion.RegistroCajaBanco!=null && infoTransaccion.RegistroCajaBanco.DocumentoBancario!=null && !string.IsNullOrEmpty(infoTransaccion.RegistroCajaBanco.DocumentoBancario.NumeroDocBancario))                 
      {
          string mensaje= CARepositorioGestionCajas.Instancia.NumConsignacionExistente(infoTransaccion.RegistroCajaBanco.DocumentoBancario.NumeroDocBancario, infoTransaccion.RegistroCajaBanco.IdBanco);
          if (mensaje != null)
              throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.CAJA,"00",mensaje));
      }

      switch (infoTransaccion.TipoTransaccion)
      {
        case CAEnumOperCajaCentroServicosGestion.TransBancoCentroServicio:              
          RegistrarTransaccionBancoCentroSvc(infoTransaccion);
          break;

        case CAEnumOperCajaCentroServicosGestion.TransCentroSvcCentroSvc:
          resultadoRegistro = RegistrarTransaccionRacolAgencia(infoTransaccion);
          break;

        case CAEnumOperCajaCentroServicosGestion.TransCasaMatrizBanco:
          RegistrarTransaccionCasaMatrizBanco(infoTransaccion);
          break;

        case CAEnumOperCajaCentroServicosGestion.TranRacolOperacioNacional:
          resultadoRegistro = RegistrarTransaccionRacolCajaOPN(infoTransaccion);
          break;

        case CAEnumOperCajaCentroServicosGestion.TranCasaMatrizOperacioNacional:
          resultadoRegistro = RegistrarTransaccionCasaMatrizOpn(infoTransaccion);
          break;

        case CAEnumOperCajaCentroServicosGestion.TranBancoOperacioNacional:
          RegistrarTransaccionOpnBanco(infoTransaccion);
          break;

        default:
          break;
      }

      return resultadoRegistro;
    }

    /// <summary>
    /// Registro de transacciones entre Centro de Servicio y Centro de servicio
    /// </summary>
    /// <param name="infoTransaccion">The info transaccion.</param>
    /// <param name="idRegistroTransCentroOrigen">The id registro trans centro origen.</param>
    /// <param name="idRegistroTransCentroDestino">The id registro trans centro destino.</param>
    internal void RegistroTransaccionCentroSvcCentroSvc(CAOperaRacolBancoEmpresaDC infoTransaccion, long idRegistroTransCentroOrigen, out long idRegistroTransCentroDestino)
    {
      using (TransactionScope trans = new TransactionScope())
      {
        // Se debe obtener el concepto dupla
        CAConceptoCajaDC dupla = CACaja.Instancia.ObtenerDuplaConcepto(infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].ConceptoCaja.IdConceptoCaja);
        infoTransaccion.RegistroCentrSvcMenor.RegistrosTransacDetallesCaja[0].ConceptoCaja = dupla;

        //Registro en Centro Servicio Dependiente Agencia-Punto
        idRegistroTransCentroDestino = CAApertura.Instancia.RegistrarVenta(infoTransaccion.RegistroCentrSvcMenor, infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta).IdTransaccionCaja;

        //Se agrega la data para el registro de transaccion
        infoTransaccion.RegistroMovCentroSvcCentroSvc = new CAMovCentroSvcCentroSvcDC()
        {
          IdCentroServicioOrigen = infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta,
          NombreCentroServicioOrigen = infoTransaccion.RegistroCentroServicio.NombreCentroServiciosVenta,
          IdCentroServicioDestino = infoTransaccion.RegistroCentrSvcMenor.IdCentroServiciosVenta,
          NombreCentroServicioDestino = infoTransaccion.RegistroCentrSvcMenor.NombreCentroServiciosVenta,
          IdRegistroTxOrigen = idRegistroTransCentroOrigen,
          IdRegistroTxDestino = idRegistroTransCentroDestino,
          UsuarioRegistra = infoTransaccion.RegistroCentroServicio.Usuario
        };

        //Adiciono la transacción
        CARepositorioCaja.Instancia.AdicionarMovRacolAgencia(infoTransaccion.RegistroMovCentroSvcCentroSvc);

        trans.Complete();
      }
    }

    private CAResultadoRegistroTransaccionDC RegistrarTransaccionRacolCajaOPN(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      CAResultadoRegistroTransaccionDC resultadoTransaccion = new CAResultadoRegistroTransaccionDC();
      long idOperacionOpn;
      long idOperacionRacol;

      using (TransactionScope trans = new TransactionScope())
      {
        if (infoTransaccion.RegistroCajaOpn.IdConceptoCaja == (int)CAEnumConceptosCaja.CONCEPTO_TRASLADO_OPENAL_A_RACOL)
        {
          if (String.IsNullOrEmpty(infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].NumeroComprobante))
          {
            string numComp = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Documento_caja_GRI_R10).ToString();
            infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].NumeroComprobante = numComp;
            resultadoTransaccion.NumeroComprobante = numComp;
          }
        }

        //1. Registrar operación caja RACOL
        idOperacionRacol = CAApertura.Instancia.RegistrarVenta(infoTransaccion.RegistroCentroServicio).IdTransaccionCaja;

        //2. Registrar operación caja OPN
        idOperacionOpn = CAApertura.Instancia.RegistraOperacionCajaOpn(infoTransaccion.RegistroCentroServicio.InfoAperturaCaja, infoTransaccion.RegistroCajaOpn);

        //3. registrar movimimiemto caja RACOL <-> OPN
        CajaCentroSvcOperacionNacionalMov_CAJ mov = new CajaCentroSvcOperacionNacionalMov_CAJ
        {
          CCO_CreadoPor = ControllerContext.Current.Usuario,
          CCO_FechaGrabacion = DateTime.Now,
          CCO_IdCentroServicios = infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta,
          CCO_IdOperacionCajaOperacionNacional = idOperacionOpn,
          CCO_IdRegistroTransaccionCaja = idOperacionRacol,
          CCO_NumeroBolsaSeguridad = infoTransaccion.RegistroCajaOpn.BolsaSeguridad ?? "0",
          CCO_NumeroGuia = infoTransaccion.RegistroCajaOpn.NumeroGuia,
          CCO_NumeroPrecinto = infoTransaccion.RegistroCajaOpn.NumeroPrecinto ?? "0"
        };

        CARepositorioGestionCajas.Instancia.AdicionarMovCentroSvcOpn(mov);

        #region Validar Suministros del formato GIR-R10

        ProcesarSuministrosGirR10(infoTransaccion, infoTransaccion.RegistroCajaOpn.IdCasaMatriz, false);

        #endregion Validar Suministros del formato GIR-R10

        trans.Complete();
      }

      return resultadoTransaccion;
    }

    /// <summary>
    /// Transaccions the empresa banco.
    /// </summary>
    /// <param name="infoTransaccion">The info transaccion.</param>
    /// <param name="idRegistroTransCentroOrigen">The id registro trans centro origen.</param>
    /// <param name="idRegistroTransCentroDestino">The id registro trans centro destino.</param>
    private void RegistrarTransaccionCasaMatrizBanco(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      long idRegistroTransCentroOrigen = 0;
      long idRegistroTransCentroDestino = 0;

      using (TransactionScope trans = new TransactionScope())
      {
        //Registrar en Caja Casa Matriz
        idRegistroTransCentroOrigen = CAApertura.Instancia.RegistrarOperacionCasaMatriz(infoTransaccion.RegistroCajaEmpresa);

        // Registrar en Banco
        idRegistroTransCentroDestino = CAApertura.Instancia.RegistrarOperacionBanco(infoTransaccion.RegistroCajaBanco);

        //Se agrega la data para el registro de transaccion
        infoTransaccion.RegistroMovEmpresaBanco = new CAMovEmpresaBancoDC()
        {
          IdCajaEmpresa = idRegistroTransCentroOrigen,
          IdCajaBanco = idRegistroTransCentroDestino,
          CreadoPor = infoTransaccion.RegistroCajaEmpresa.CreadoPor
        };

        // Adiciona El movimiento Entre la caja de la empresa y la caja del Banco.
        CARepositorioCaja.Instancia.AdicionarMovCasaMatrizBanco(infoTransaccion.RegistroMovEmpresaBanco);

        trans.Complete();
      }
    }

    /// <summary>
    /// Transacciones entre el Racol y las Agencias.
    /// </summary>
    /// <param name="infoTransaccion">The info transaccion.</param>
    /// <param name="idRegistroTransCentroOrigen">The id registro trans centro origen.</param>
    /// <param name="idRegistroTransCentroDestino">The id registro trans centro destino.</param>
    private CAResultadoRegistroTransaccionDC RegistrarTransaccionRacolAgencia(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      CAResultadoRegistroTransaccionDC resultadoTransaccion = new CAResultadoRegistroTransaccionDC();

      long idRegistroTransCentroOrigen;
      long idRegistroTransCentroDestino;
      bool validarGirR10 = true;
      long idPropietarioGirR10 = 0;

      using (TransactionScope trans = new TransactionScope())
      {
        if (infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].ConceptoCaja.IdConceptoCaja ==
         (int)CAEnumConceptosCaja.CONCEPTO_TRASLADO_RACOL_A_CENTRO_SERVICIOS)
        {
          if (!String.IsNullOrEmpty(infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].NumeroComprobante) &&
            infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].NumeroComprobante.Equals("0"))
          {
            string numComp = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Documento_caja_GRI_R10).ToString();
            infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].NumeroComprobante = numComp;
            infoTransaccion.RegistroCentrSvcMenor.RegistrosTransacDetallesCaja[0].NumeroComprobante = numComp;

            resultadoTransaccion.NumeroComprobante = numComp;
            validarGirR10 = false;
            idPropietarioGirR10 = infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta;
          }
        }
        else if (infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].ConceptoCaja.IdConceptoCaja ==
         (int)CAEnumConceptosCaja.CONCEPTO_TRASLADO_AGENCIA_A_RACOL)
        {
          idPropietarioGirR10 = infoTransaccion.RegistroCentrSvcMenor.IdCentroServiciosVenta;
          resultadoTransaccion.NumeroComprobante = infoTransaccion.RegistroCentrSvcMenor.RegistrosTransacDetallesCaja.First().NumeroComprobante;
        }

        //Registro en caja CentroServicio
        idRegistroTransCentroOrigen = CAApertura.Instancia.RegistrarVenta(infoTransaccion.RegistroCentroServicio).IdTransaccionCaja;
        resultadoTransaccion.IdTransaccion = idRegistroTransCentroOrigen;

        //Registro en Centro Servicio Dependiente Agencia-Punto
        idRegistroTransCentroDestino = CAApertura.Instancia.RegistrarVenta(infoTransaccion.RegistroCentrSvcMenor, infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta).IdTransaccionCaja;

        //Se agrega la data para el registro de transaccion
        if (infoTransaccion.RegistroMovCentroSvcCentroSvc == null)
        {
          infoTransaccion.RegistroMovCentroSvcCentroSvc = new CAMovCentroSvcCentroSvcDC();
        }

        infoTransaccion.RegistroMovCentroSvcCentroSvc.IdCentroServicioOrigen = infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta;
        infoTransaccion.RegistroMovCentroSvcCentroSvc.NombreCentroServicioOrigen = infoTransaccion.RegistroCentroServicio.NombreCentroServiciosVenta;
        infoTransaccion.RegistroMovCentroSvcCentroSvc.IdCentroServicioDestino = infoTransaccion.RegistroCentrSvcMenor.IdCentroServiciosVenta;
        infoTransaccion.RegistroMovCentroSvcCentroSvc.NombreCentroServicioDestino = infoTransaccion.RegistroCentrSvcMenor.NombreCentroServiciosVenta;
        infoTransaccion.RegistroMovCentroSvcCentroSvc.IdRegistroTxOrigen = idRegistroTransCentroOrigen;
        infoTransaccion.RegistroMovCentroSvcCentroSvc.IdRegistroTxDestino = idRegistroTransCentroDestino;
        infoTransaccion.RegistroMovCentroSvcCentroSvc.UsuarioRegistra = infoTransaccion.RegistroCentroServicio.Usuario;

        // Adicionar los movimientos genrados entre Racol y Agencia.
        CARepositorioCaja.Instancia.AdicionarMovRacolAgencia(infoTransaccion.RegistroMovCentroSvcCentroSvc);

        #region Validar Suministros para concepto CAEnumConceptosCaja.CONCEPTO_TRASLADO_RACOL_A_CENTRO_SERVICIOS

        if (infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].ConceptoCaja.IdConceptoCaja ==
          (int)CAEnumConceptosCaja.CONCEPTO_TRASLADO_RACOL_A_CENTRO_SERVICIOS ||
          infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja[0].ConceptoCaja.IdConceptoCaja ==
          (int)CAEnumConceptosCaja.CONCEPTO_TRASLADO_AGENCIA_A_RACOL)
        {
          ProcesarSuministrosGirR10(infoTransaccion, idPropietarioGirR10, validarGirR10);
        }

        #endregion Validar Suministros para concepto CAEnumConceptosCaja.CONCEPTO_TRASLADO_RACOL_A_CENTRO_SERVICIOS

        trans.Complete();
      }

      return resultadoTransaccion;
    }

    /// <summary>
    /// Registrar transacción entre el Banco y Los Centros de Servicios.
    /// </summary>
    /// <param name="infoTransaccion">The info transaccion.</param>
    /// <param name="idRegistroTransCentroOrigen">The id registro trans centro origen.</param>
    /// <param name="idRegistroTransCentroDestino">The id registro trans centro destino.</param>
    private void RegistrarTransaccionBancoCentroSvc(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      long idRegistroTransCentroOrigen;
      long idRegistroTransCentroDestino;

      using (TransactionScope trans = new TransactionScope())
      {
        //Registro en caja CentroServicio
        idRegistroTransCentroOrigen = CAApertura.Instancia.RegistrarVenta(infoTransaccion.RegistroCentroServicio).IdTransaccionCaja;

        infoTransaccion.RegistroCajaBanco.IdCentroRegistra = infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta;
        infoTransaccion.RegistroCajaBanco.NombreCentroRegistra = infoTransaccion.RegistroCentroServicio.NombreCentroServiciosVenta;

        //Registro en Caja Banco
        idRegistroTransCentroDestino = CAApertura.Instancia.RegistrarOperacionBanco(infoTransaccion.RegistroCentroServicio.InfoAperturaCaja, infoTransaccion.RegistroCajaBanco);

        //Se agrega la data para el registro de transaccion
        infoTransaccion.RegistroMovBancoCentroSvc = new CAMovBancoCentroSvcDC()
        {
          IdCentroServicio = infoTransaccion.RegistroCentroServicio.IdCentroServiciosVenta,
          NombreCentroServicio = infoTransaccion.RegistroCentroServicio.NombreCentroServiciosVenta,
          IdRegistroTransaccion = idRegistroTransCentroOrigen,
          FechaGrabacion = DateTime.Now,
          CreadoPor = infoTransaccion.RegistroCentroServicio.Usuario,
          IdCajaBanco = idRegistroTransCentroDestino
        };

        // Adicionar  el Movimiento entre un centro Servicio y la  caja del Banco.
        CARepositorioCaja.Instancia.AdicionarMovCentroSrvCajaBanco(infoTransaccion.RegistroMovBancoCentroSvc);
        trans.Complete();
      }
    }

    private CAResultadoRegistroTransaccionDC RegistrarTransaccionCasaMatrizOpn(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      CAResultadoRegistroTransaccionDC resultadoTransaccion = new CAResultadoRegistroTransaccionDC();
      long idRegistroCasaMatriz;
      long idRegistroOpn;

      using (TransactionScope trans = new TransactionScope())
      {
        if (infoTransaccion.RegistroCajaOpn.IdConceptoCaja == (int)CAEnumConceptosCaja.CONCEPTO_TRASLADO_OPENAL_A_CASA_MATRIZ)
        {
          if (String.IsNullOrEmpty(infoTransaccion.RegistroCajaOpn.NumeroDocumento))
          {
            string numComp = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Documento_caja_GRI_R10).ToString();
            infoTransaccion.RegistroCajaOpn.NumeroDocumento = numComp;
            resultadoTransaccion.NumeroComprobante = numComp;
          }
        }

        // registrar operación casa matriz
        idRegistroCasaMatriz = CAApertura.Instancia.RegistrarOperacionCasaMatriz(infoTransaccion.RegistroCajaEmpresa);

        // registrar operación opn
        idRegistroOpn = CAApertura.Instancia.RegistraOperacionCajaOpn(infoTransaccion.RegistroCajaOpn);

        // registrar mov cs <-> opn
        CajaCasaMatrizOperacionNacionalMov_CAJ mov = new CajaCasaMatrizOperacionNacionalMov_CAJ
        {
          CMO_CreadoPor = infoTransaccion.RegistroCajaEmpresa.CreadoPor,
          CMO_FechaGrabacion = DateTime.Now,
          CMO_IdOperacionCajaCasaMatriz = idRegistroCasaMatriz,
          CON_IdOperacionCajaOperacionNacional = idRegistroOpn,
          CMO_NumeroBolsaSeguridad = infoTransaccion.RegistroCajaOpn.BolsaSeguridad ?? "0",
          CMO_NumeroPrecinto = infoTransaccion.RegistroCajaOpn.NumeroPrecinto,
          CMO_NumeroGuia = infoTransaccion.RegistroCajaOpn.NumeroGuia,
        };

        CARepositorioCaja.Instancia.AdicionarMovCasaMatrizOpn(mov);

        #region Validar Suministros del formato GIR-R10

        if (infoTransaccion.RegistroCajaEmpresa.ConceptoCaja.IdConceptoCaja != 31 && infoTransaccion.RegistroCajaEmpresa.ConceptoCaja.IdConceptoCaja != 36)
        {
          ProcesarSuministrosGirR10(infoTransaccion, infoTransaccion.RegistroCajaOpn.IdCasaMatriz, false);
        }

        #endregion Validar Suministros del formato GIR-R10

        trans.Complete();
      }

      return resultadoTransaccion;
    }

    private void RegistrarTransaccionOpnBanco(CAOperaRacolBancoEmpresaDC infoTransaccion)
    {
      long idRegistroBanco;
      long idRegistroOpn;

      using (TransactionScope trans = new TransactionScope())
      {
        // registrar operación opn
        idRegistroOpn = CAApertura.Instancia.RegistraOperacionCajaOpn(infoTransaccion.RegistroCajaOpn);

        //Registro en Caja Banco
        idRegistroBanco = CAApertura.Instancia.RegistrarOperacionBanco(infoTransaccion.RegistroCajaBanco);

        //Registro movimiento
        CajaBancoOperacionNacionalMov_CAJ mov = new CajaBancoOperacionNacionalMov_CAJ
        {
          CBO_CreadoPor = ControllerContext.Current.Usuario,
          CBO_FechaGrabacion = DateTime.Now,
          CBO_IdOperacionCajaBanco = idRegistroBanco,
          CBO_IdOperacionCajaOperacionNacional = idRegistroOpn
        };

        CARepositorioCaja.Instancia.AdicionarMovOpnBanco(mov);

        trans.Complete();
      }
    }

    /// <summary>
    /// Carga información inicial para la impresión de formato GRI-R10
    /// </summary>
    /// <param name="idCentroServiciosOrigen">Identificador centro de servicios origen de la transacción</param>
    /// <param name="idCentroServiciosDestino">Identificador centro de servicios destino de la transacción</param>
    /// <param name="esGestion">True, el origen es una gestion, False es un centro de servicios</param>
    /// <param name="valor">alor de la operacion</param>
    /// <param name="bolsaSeguridad">Número de bolsa de seguridad</param>
    /// <param name="numeroGuia">Número de guía interna con la cual se hace el movimiento</param>
    /// <param name="numeroPrecinto">Número del precinto, este puede ser null</param>
    /// <returns>Obejeto con los datos iniciales del formato</returns>
    /// <remarks>El objeto retornado no se llena completamente ya que hay algunos datos que se encuentran solo en el cliente</remarks>
    internal CADatosImpresionGirR10DC ObtenerDatosFormatoGirR10(long idCentroServiciosOrigen,
      long idCentroServiciosDestino,
      bool esGestion,
      decimal valor,
      string bolsaSeguridad,
      long numeroGuia,
      string numeroPrecinto)
    {
      CADatosImpresionGirR10DC formato = null;
      string centroCostos;
      string nombreCiudad;
      string nombreOficina;
      long idCentroServicios;

      if (esGestion)
      {
        IARFachadaAreas fachadaArea = COFabricaDominio.Instancia.CrearInstancia<IARFachadaAreas>();
        ARCasaMatrizDC casaMatriz = fachadaArea.ObtenerCasaMatriz((int)idCentroServiciosDestino);
        centroCostos = casaMatriz.CentroCostos;
        nombreCiudad = casaMatriz.NombreLocalidad;
        nombreOficina = casaMatriz.Nombre;
        idCentroServicios = casaMatriz.IdCasaMatriz;
      }
      else
      {
        IPUFachadaCentroServicios fachadaCentroServicios = COFabricaDominio.Instancia.CrearInstancia<IPUFachadaCentroServicios>();

        PUCentroServiciosDC centroServicios = fachadaCentroServicios.ObtenerCentroServicio(idCentroServiciosDestino);
        centroCostos = centroServicios.IdCentroCostos;
        nombreCiudad = centroServicios.CiudadUbicacion.Nombre;
        nombreOficina = centroServicios.Nombre;
        idCentroServicios = centroServicios.IdCentroServicio;
      }

      using (TransactionScope transaccion = new TransactionScope())
      {
        formato = new CADatosImpresionGirR10DC
        {
          CentoCostos = centroCostos,
          Fecha = DateTime.Now,
          NombreCiudad = nombreCiudad,
          NombreOficina = nombreOficina,
          IdOficina = idCentroServicios,
          //NumeroDocumento = PAAdministrador.Instancia.ObtenerConsecutivo(PAEnumConsecutivos.Documento_caja_GRI_R10),
          Obseracion = String.Empty,
          Valor = valor,
          ValorEnLetras = String.Empty,
          NombreResponsable = String.Empty,
        };

        transaccion.Complete();
      }

      return formato;
    }

    /// <summary>
    /// Obtiene todas las aperturas
    /// abiertas de Casa Matriz - Operacion nacional - Banco
    /// </summary>
    /// <returns>lista de aperturas activas</returns>
    public IList<CAAperturaCajaCasaMatrizDC> ObtenerAperturasCajaGestion()
    {
      return CARepositorioGestionCajas.Instancia.ObtenerAperturasCajaGestion();
    }

    #endregion Transacciones Banco-Casa Matriz-CentroSvc-Operación Nacional

    #region Suministros

    private SUPropietarioGuia ValidarPropiedadSuministro(string numeroSuministro,
     SUEnumSuministro tipoSuministro,
     ISUFachadaSuministros fachadaSuministros, long idPropietario,
     out long numero)
    {
      numero = 0;

      // sacar el numero numeroSuministro
      if (!long.TryParse(numeroSuministro, out numero))
      {
        StringBuilder num = new StringBuilder();
        foreach (char c in numeroSuministro)
        {
          if (Char.IsNumber(c))
          {
            num.Append(c);
          }
        }

        if (!long.TryParse(num.ToString(), out numero))
        {
          throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_GESTION_CAJAS,
            CAEnumTipoErrorCaja.ERROR_SUMINISTRO_NO_ES_CORRECTO.ToString(),
            String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_SUMINISTRO_NO_ES_CORRECTO), numeroSuministro)));
        }
      }

      //validar suministro
      SUPropietarioGuia duenoSum = fachadaSuministros.ObtenerPropietarioSuministro(numero, tipoSuministro, idPropietario);

      return duenoSum;
    }

    private void ProcesarSuministrosGirR10(CAOperaRacolBancoEmpresaDC infoTransaccion, long idPropietario, bool validarPropietario)
    {
      ISUFachadaSuministros fachadaSuministros = COFabricaDominio.Instancia.CrearInstancia<ISUFachadaSuministros>();

      long numeroSum;
      string numeroGuia;
      string numeroBolsa;
      string numeroPrecinto;
      long girR10;

      if (infoTransaccion.TipoTransaccion == CAEnumOperCajaCentroServicosGestion.TranCasaMatrizOperacioNacional)
      {
        numeroGuia = infoTransaccion.RegistroCajaOpn.NumeroGuia.ToString();
        numeroBolsa = infoTransaccion.RegistroCajaOpn.BolsaSeguridad;
        numeroPrecinto = infoTransaccion.RegistroCajaOpn.NumeroPrecinto;
        long.TryParse(infoTransaccion.RegistroCajaOpn.NumeroDocumento, out girR10);
      }
      else
      {
        numeroGuia = infoTransaccion.RegistroMovCentroSvcCentroSvc.NumeroGuia.ToString();
        numeroBolsa = infoTransaccion.RegistroMovCentroSvcCentroSvc.BolsaSeguridad;
        numeroPrecinto = infoTransaccion.RegistroMovCentroSvcCentroSvc.NumeroPrecinto;
        long.TryParse(infoTransaccion.RegistroCentroServicio.RegistrosTransacDetallesCaja.First().NumeroComprobante, out girR10);
      }

      SUPropietarioGuia propietario = null;
      SUConsumoSuministroDC consumo;

      //validar GIR-R10
      if (girR10 != 0)
      {
        long idProp = idPropietario;

        if (validarPropietario)
        {
          propietario = ValidarPropiedadSuministro(girR10.ToString(),
            SUEnumSuministro.COMPROBANTE_ABONO_EFECTIVO, fachadaSuministros, idPropietario, out numeroSum);

          if (propietario.Id != idPropietario)
          {
            throw new FaultException<ControllerException>(new ControllerException(COConstantesModulos.MODULO_GESTION_CAJAS,
            CAEnumTipoErrorCaja.ERROR_SUMINISTRO_NO_ES_CORRECTO.ToString(),
            String.Format(CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_SUMINISTRO_NO_ES_CORRECTO), girR10)));
          }

          idProp = propietario.Id;
        }

        consumo = new SUConsumoSuministroDC()
        {
          Cantidad = 1,
          EstadoConsumo = SUEnumEstadoConsumo.CON,
          GrupoSuministro = propietario == null ? SUEnumGrupoSuministroDC.AGE : propietario.Propietario,
          IdDuenoSuministro = propietario == null ? idProp : propietario.Id,
          IdServicioAsociado = TAConstantesServicios.SERVICIO_MENSAJERIA,
          NumeroSuministro = girR10,
          Suministro = SUEnumSuministro.COMPROBANTE_ABONO_EFECTIVO
        };

        fachadaSuministros.GuardarConsumoSuministro(consumo);
      }

      //if (numeroGuia != null && !numeroGuia.Equals("0"))
      //{
      //  // validar guia interna
      //  propietario = ValidarPropiedadSuministro(numeroGuia,
      //   SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA, fachadaSuministros, idPropietario, out numeroSum);

      //  // consumir guía
      //  consumo = new SUConsumoSuministroDC()
      //  {
      //    Cantidad = 1,
      //    EstadoConsumo = SUEnumEstadoConsumo.CON,
      //    GrupoSuministro = propietario.Propietario,
      //    IdDuenoSuministro = propietario.Id,
      //    IdServicioAsociado = TAConstantesServicios.SERVICIO_MENSAJERIA,
      //    NumeroSuministro = numeroSum,
      //    Suministro = SUEnumSuministro.GUIA_CORRESPONDENCIA_INTERNA
      //  };

      //  fachadaSuministros.GuardarConsumoSuministro(consumo);
      //}

      if (numeroBolsa != null && !numeroBolsa.Equals("0"))
      {
        // validar y consumir bolsa seguridad
        fachadaSuministros.GuardarConsumoBolsaSeguridad(numeroBolsa,
         TAConstantesServicios.SERVICIO_MENSAJERIA, idPropietario);
      }

      // validar precinto
      if (!String.IsNullOrEmpty(numeroPrecinto))
      {
        propietario = ValidarPropiedadSuministro(numeroPrecinto,
          SUEnumSuministro.PRECINTO_SEGURIDAD, fachadaSuministros, idPropietario,
          out numeroSum);

        // consumir precinto
        consumo = new SUConsumoSuministroDC();
        consumo.IdDuenoSuministro = propietario.Id;
        consumo.IdServicioAsociado = TAConstantesServicios.SERVICIO_MENSAJERIA;
        consumo.NumeroSuministro = numeroSum;
        consumo.Suministro = SUEnumSuministro.PRECINTO_SEGURIDAD;
        fachadaSuministros.GuardarConsumoSuministro(consumo);
      }
    }

    #endregion Suministros

    #region Consultas

    /// <summary>
    /// Metodo para Obtener las Transacciones.
    /// realizadas por la Empresa.
    /// </summary>
    /// <param name="fechaTransaccion">The fecha transaccion.</param>
    /// <returns></returns>
    public IList<CACajaCasaMatrizDC> ObtenerTransaccionesEmpresa(DateTime fechaTransaccion, short idCasaMtriz)
    {
      return CARepositorioCaja.Instancia.ObtenerTransaccionesCasaMatriz(fechaTransaccion, idCasaMtriz);
    }

    /// <summary>
    /// Metodo para obtener las operaciones del RACOL.
    /// </summary>
    /// <param name="fechaTransaccion">The fecha transaccion.</param>
    /// <param name="idCentroServicio">The id centro servicio.</param>
    /// <returns></returns>
    public List<CACajaCasaMatrizDC> ObtenerTransaccionesRACOL(DateTime fechaTransaccion, long idCentroServicio)
    {
      return CARepositorioCaja.Instancia.ObtenerTransaccionesRACOL(fechaTransaccion, idCentroServicio);
    }

    /// <summary>
    /// Obtener las operaciones de caja de Operación Nacional en una fecha
    /// </summary>
    /// <param name="idCasaMatriz">Identificador único de la casa matriz</param>
    /// <param name="fecha">Fecha en la cual se hace la consulta</param>
    /// <returns>Collección con la información de las operaciones</returns>
    public IList<CACajaCasaMatrizDC> ObtenerOperacionesOpn(DateTime fechaTransaccion, short idCasaMatriz)
    {
      return CARepositorioCaja.Instancia.ObtenerOperacionesOpn(idCasaMatriz, fechaTransaccion);
    }

    public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionCasaMatriz()
    {
      return CARepositorioCaja.Instancia.ObtenerConceptosCajaGestionCasaMatriz();
    }

    public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionOpn()
    {
      return CARepositorioCaja.Instancia.ObtenerConceptosCajaGestionOpn();
    }

    public IEnumerable<CAConceptoCajaDC> ObtenerConceptosCajaGestionRacol()
    {
      return CARepositorioCaja.Instancia.ObtenerConceptosCajaGestionRacol();
    }

    #endregion Consultas
  }
}