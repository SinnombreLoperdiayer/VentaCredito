using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Servidor.Servicios.ContratoDatos.Cajas;
using Framework.Servidor.Servicios.ContratoDatos;

namespace CO.Servidor.Servicios.ContratoDatos.GestionCajas
{
  /// <summary>
  /// Clase que contiene las clases de Operaciones
  /// entre Racol-Agencia-CasaMatriz-Banco
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CAOperaRacolBancoEmpresaDC : DataContractBase
  {
    /// <summary>
    /// Es la Info para el registro
    /// en la caja del Centro Servicio.
    /// </summary>
    /// <value>
    /// The registro centro servicio.
    /// </value>
    [DataMember]
    public CARegistroTransacCajaDC RegistroCentroServicio { get; set; }

    /// <summary>
    /// Es la Info para el registro
    /// en la caja del Centro Servicio Dependiente
    /// </summary>
    /// <value>
    /// The registro centr SVC menor.
    /// </value>
    [DataMember]
    public CARegistroTransacCajaDC RegistroCentrSvcMenor { get; set; }

    /// <summary>
    /// Es la Info para el registro
    /// en la caja de la Banco.
    /// </summary>
    /// <value>
    /// The registro caja banco.
    /// </value>
    [DataMember]
    public CACajaBancoDC RegistroCajaBanco { get; set; }

    /// <summary>
    /// Es la Info para el registro
    /// en la caja de la Empresa.
    /// </summary>
    /// <value>
    /// The registro caja empresa.
    /// </value>
    [DataMember]
    public CACajaCasaMatrizDC RegistroCajaEmpresa { get; set; }

    /// <summary>
    /// Retorna o asigna la información de la operación sobre la caja de Operación Nacional
    /// </summary>
    [DataMember]
    public CARegistroOperacionOpnDC RegistroCajaOpn { get; set; }

    /// <summary>
    /// Es la info del Movimiento entre las Cajas de los
    /// Centros de Servicio.
    /// </summary>
    /// <value>
    /// The movimiento centro servicio.
    /// </value>
    [DataMember]
    public CAMovCentroSvcCentroSvcDC RegistroMovCentroSvcCentroSvc { get; set; }

    /// <summary>
    /// Es la info del Movimiento entre las Cajas de los
    /// Centros de Servicio y el Banco.
    /// </summary>
    /// <value>
    /// The registro mov banco centro SVC.
    /// </value>
    [DataMember]
    public CAMovBancoCentroSvcDC RegistroMovBancoCentroSvc { get; set; }

    /// <summary>
    /// Es la info del Movimiento entre las Cajas de
    /// la Empresa y el Banco.
    /// </summary>
    /// <value>
    /// The registro mov empresa banco.
    /// </value>
    [DataMember]
    public CAMovEmpresaBancoDC RegistroMovEmpresaBanco { get; set; }

    /// <summary>
    /// Es la info del Movimiento entre las Cajas de
    /// la Empresa y el Centro Servicio.
    /// </summary>
    /// <value>
    /// The registro mov empresa centro SVC.
    /// </value>
    [DataMember]
    public CAMovEmpresaCentroSvcDC RegistroMovEmpresaCentroSvc { get; set; }

    /// <summary>
    /// Es el tipo de Transaccion entre
    /// Racol-Agencia, Racol-Banco
    /// </summary>
    /// <value>
    /// The tipo transaccion.
    /// </value>
    [DataMember]
    public CAEnumOperCajaCentroServicosGestion TipoTransaccion { get; set; }
  }
}