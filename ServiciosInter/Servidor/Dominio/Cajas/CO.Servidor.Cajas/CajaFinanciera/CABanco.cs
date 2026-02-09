using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using CO.Servidor.Cajas.Comun;
using CO.Servidor.Cajas.Datos;
using CO.Servidor.Cajas.Datos.Modelo;
using CO.Servidor.Servicios.ContratoDatos.GestionCajas;
using Framework.Servidor.Comun;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.CajaFinanciera
{
  /// <summary>
  /// Administra operaciones sobre la caja de bancos
  /// </summary>
  internal class CABanco
  {
    /// <summary>
    /// Registrar operación sobre la caja de bancos
    /// </summary>
    /// <param name="idApertura">Identificador de la apertura de caja con la cual se registra la transacción</param>
    /// <param name="operacion">Información de la operación</param>
    /// <returns>Identificador de la operación de banco</returns>
    public long RegistrarOperacionBanco(long idApertura, CACajaBancoDC operacion)
    {
      //registrar egreso en banco

      // cualquier operación sobre la caja de bancos debe afectar una cuenta bancaria
      if (String.IsNullOrEmpty(operacion.NumeroCta))
      {
        throw new FaultException<ControllerException>(
            new ControllerException(COConstantesModulos.CAJA, CAEnumTipoErrorCaja.ERROR_NO_CUENTA_BANCO.ToString(),
             CACajaServerMensajes.CargarMensaje(CAEnumTipoErrorCaja.ERROR_NO_CUENTA_BANCO))
             );
      }

      OperacionCajaBanco_CAJ opBanco = new OperacionCajaBanco_CAJ
        {
          CAB_IdAperturaCaja = idApertura,
          CAB_IdConceptoCaja = operacion.ConceptoCaja.IdConceptoCaja,
          CAB_NombreConceptoCaja = operacion.ConceptoCaja.Nombre,
          CAB_ConceptoEsIngreso = operacion.ConceptoCaja.EsIngreso,
          CAB_DescripcionBanco = operacion.DescripcionBanco,
          CAB_DescripcionTipoDocumento = operacion.DocumentoBancario.DescripcionTipoDocBancario,
          CAB_FechaMovimiento = operacion.FechaMovimiento,
          CAB_IdBanco = operacion.IdBanco,
          CAB_NumeroCuenta = operacion.NumeroCta,
          CAB_Valor = operacion.Valor,
          CAB_IdTipoDocumento = operacion.DocumentoBancario.TipoDocBancario,
          CAB_NumeroDocumento = operacion.DocumentoBancario.NumeroDocBancario ?? String.Empty,
          CAB_Observacion = operacion.Observacion,
          CAB_MovHechoPor = operacion.MovHechoPor,
          CAB_FechaGrabacion = DateTime.Now,
          CAB_CreadoPor = ControllerContext.Current.Usuario,
          CAB_NumeroComprobante=operacion.NumeroComprobante
        };

      return CARepositorioGestionCajas.Instancia.AdicionarOperacionCajaBanco(opBanco);
    }    
  }
}