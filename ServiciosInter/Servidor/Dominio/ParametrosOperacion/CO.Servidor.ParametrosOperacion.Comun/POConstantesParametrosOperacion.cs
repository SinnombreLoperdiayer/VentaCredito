using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.ParametrosOperacion.Comun
{
  /// <summary>
  /// Clase que contiene las constantes de los paramentros de operacion
  /// </summary>
  public class POConstantesParametrosOperacion
  {
    /// <summary>
    /// Constante que indica el identificador estado suspendido
    /// </summary>
    public const string EstadoSuspendido = "SUS";
    public const string MODULO_PARAMETROS_OPERACION = "PARAMETROSOPERACION";

    /// <summary>
    /// Tipo de poliza de seguro SOAT
    /// </summary>
    public const string TipoPolizaSeguro_SOAT = "1";

    /// <summary>
    /// Tipo poliza de seguro Todo Riesgo
    /// </summary>
    public const string TipoPolizaSeguro_TodoRiesgo = "2";

    /// <summary>
    /// Tipo de vehiculo moto
    /// </summary>
    public const short TipoVehiculo_Moto = 1;
    /// <summary>
    /// Tipo de vehiculo carro
    /// </summary>
    public const short TipoVehiculo_Carro = 2;

    /// <summary>
    /// Tipo de contrato propio
    /// </summary>
    public const short TipoContrato_Propio = 1;

    /// <summary>
    /// Tipo de contrato Contratista
    /// </summary>
    public const short TipoContrato_Contratista = 2;

    /// <summary>
    /// Nit de interRapidisimo
    /// </summary>
    public const string NitInterrapidisimo = "800251569";
    /// <summary>
    /// Digito de verificacion para interrapidisimo
    /// </summary>
    public const string DigVerificacionInterRapidisimo = "7";
    /// <summary>
    /// Tipo de documento para NIT
    /// </summary>
    public const string TipoDocumento_Nit = "NI";

    /// <summary>
    /// Constante para el estado activo
    /// </summary>
    public const string Estado_Activo = "ACT";

    /// <summary>
    /// Constante para el estado inactivo
    /// </summary>
    public const string Estado_Inactivo = "INA";

    /// <summary>
    /// Constante para el tipo de mensajero auxiliar
    /// </summary>
    public const int ID_TIPO_MENSAJERO_AUXILIAR = 4;
  }
}