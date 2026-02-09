using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using CO.Cliente.Servicios.ContratoDatos;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;

namespace CO.Servidor.Servicios.ContratoDatos.Cajas
{
  /// <summary>
  /// Clase que contiene la informacion de
  /// la recoleccion de dinero de puntos
  /// </summary>
  [DataContract(Namespace = "http://contrologis.com")]
  public class CARecoleccionDinPuntoDC
  {
    ///// <summary>
    ///// Es el id recoleccion.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Id")]
    //public int IdRecoleccion { get; set; }

    ///// <summary>
    ///// Es el id punto servicio.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "IdPuntoCvalorentroServicio")]
    //public long IdPuntoServicio { get; set; }

    ///// <summary>
    ///// El nombre punto.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
    //public string NombrePunto { get; set; }

    // //<summary>
    // //Es la clase de Puntocentro Servicio
    // //</summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "PuntoCentroServicio")]
    //public PUCentroServiciosDC PuntoCentroServicio { get; set; }

    //private string bolsaSeguridad;

    ///// <summary>
    ///// The bolsa seguridad.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "BolsaSeguridad", Description = "TooltipBolsaSeguridad")]
    //public string BolsaSeguridad
    //{
    //  get { return bolsaSeguridad; }
    //  set
    //  {
    //    bolsaSeguridad = value;
    //    if (value != null && OnIngresoBolsaSeguridadEvent != null)
    //    {
    //      OnIngresoBolsaSeguridadEvent();
    //    }
    //  }
    //}

    ///// <summary>
    ///// Es el usuario cierre.
    ///// </summary>
    //[DataMember]
    //public string UsuarioCierre { get; set; }

    ///// <summary>
    ///// Es el mensajero punto.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "NombreMensajero")]
    //public OUNombresMensajeroDC MensajeroPunto { get; set; }

    ///// <summary>
    ///// Es la observacion.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "Observaciones", Description = "TooltipObservaciones")]
    //public string Observacion { get; set; }

    ///// <summary>
    ///// Es el valor total enviado.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TotalReportado", Description = "ToolTipTotalAEmpresa")]
    //public decimal ValorTotalEnviado { get; set; }

    //private decimal valorReal;

    ///// <summary>
    ///// The valor real.
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorReal")]
    //public decimal ValorReal
    //{
    //  get { return valorReal; }
    //  set
    //  {
    //    valorReal = value;
    //    if (ValorTotalEnviado != 0)
    //    {
    //      ValorDiferencia = ValorTotalEnviado - valorReal;
    //    }
    //  }
    //}

    //private decimal valorDiferencia;

    ///// <summary>
    ///// Es el Valor de la diferencia entre el
    ///// Valor TotalAReportar y TotalReportado
    ///// </summary>
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "ValorDeDiferencia")]
    //public decimal ValorDiferencia
    //{
    //  get { return valorDiferencia; }
    //  set
    //  {
    //    valorDiferencia = value;
    //    if (value != 0 && OnDiferenciaEncontradaEvent != null)
    //    {
    //      OnDiferenciaEncontradaEvent();
    //    }
    //  }
    //}

    ///// <summary>
    ///// es el tipo Observacion Punto
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "TipoObsevPunto", Description = "ToolTipTipoObsevPunto")]
    //public CATipoObsPuntoAgenciaDC TipoObservacionPunto { get; set; }

    ///// <summary>
    ///// Es la fecha de Recoleccion
    ///// </summary>
    //[DataMember]
    //[Display(ResourceType = typeof(Framework.Servidor.Servicios.ContratoDatos.Etiquetas), Name = "FechaRecoleccion", Description = "TootipFechaRecoleccion")]
    //public DateTime FechaRecoleccion { get; set; }

    ///// <summary>
    ///// Indica si el registro es manual [regsitro manual].
    ///// </summary>
    ///// <value>
    /////   <c>true</c> if [regsitro manual]; otherwise, <c>false</c>.
    ///// </value>
    //[DataMember]
    //public bool RegistroManual { get; set; }

    //#region Eventos

    ///// <summary>
    ///// Evento para Consultar la bolsa de seguridad
    ///// </summary>
    //public delegate void OnIngresoBolsaSeguridad();

    //public event OnIngresoBolsaSeguridad OnIngresoBolsaSeguridadEvent;

    ///// <summary>
    ///// Evento cuando se genera una direncia entre el vr enviado y el real registrado
    ///// </summary>
    //public delegate void OnDiferenciaEncontrada();

    //public event OnDiferenciaEncontrada OnDiferenciaEncontradaEvent;

    //#endregion Eventos
  }
}