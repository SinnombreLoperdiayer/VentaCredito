using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.Clientes.Comun
{
  /// <summary>
  /// Clase para manejo de constantes propias de clientes
  /// </summary>
  public class ConstantesClientes
  {
    /// <summary>
    ///  Tipo de archivo adiconal (valor = 1)
    /// </summary>
    public const int TIPO_ARCHIVO_ADICIONAL = 1;

    /// <summary>
    ///  Archivo adicional (valor = "ADICIONAL")
    /// </summary>
    public const string ARCHIVO_ADICIONAL = "ADICIONAL";

    /// <summary>
    /// Tipo localidad 1 (valor = "1")
    /// </summary>
    public const string TIPO_LOCALIDAD_1 = "1";

    /// <summary>
    /// Tipo localidad 2 (valor = "2")
    /// </summary>
    public const string TIPO_LOCALIDAD_2 = "2";

    /// <summary>
    /// Tipo localidad 3 (valor = "3")
    /// </summary>
    public const string TIPO_LOCALIDAD_3 = "3";

    /// <summary>
    ///  Tipo de archivo adiconal (valor = 1)
    /// </summary>
    public const bool TIPO_ORIGEN_ABIERTO = true;

    /// <summary>
    ///  Tipo de archivo adiconal (valor = 1)
    /// </summary>
    public const bool TIPO_ORIGEN_CERRADO = false;

    /// <summary>
    /// Tipo localidad 3 (valor = "3")
    /// </summary>
    public const string DESC_ORIGEN_ABIERTO = "ABIERTO";

    /// <summary>
    /// Tipo localidad 3 (valor = "3")
    /// </summary>
    public const string DESC_ORIGEN_CERRADO = "CERRADO";

    /// <summary>
    /// Constante para identificar el tipo de sector cliente privado
    /// </summary>
    public const int TIPO_SECTOR_CLIENTE_PRIVADO = 1;
    /// <summary>
    /// Constante para identificar el tipo de sector cliente publico
    /// </summary>
    public const int TIPO_SECTOR_CLIENTE_PUBLICO = 2;
    /// <summary>
    /// Constante para identificar el tipo de sector cliente economia mixta
    /// </summary>
    public const int TIPO_SECTOR_CLIENTE_MIXTA = 3;

    /// <summary>
    /// Constante para identificar el tipo de archivo de contrato  registro de disponibilidad presupuestal
    /// </summary>
    public const int TIPO_DOCUMENTO_CONTRATO_REG_DISPO_PRESUPUESTAL = 5;

    /// <summary>
    /// valor vacio de ciudad
    /// </summary>
    public const string VALOR_VACIO = "     ";

    /// <summary>
    /// valor CERO de ciudad
    /// </summary>
    public const string VALOR_CERO = "0";
  }
}