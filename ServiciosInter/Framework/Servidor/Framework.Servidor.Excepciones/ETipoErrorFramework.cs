namespace Framework.Servidor.Excepciones
{
    /// <summary>
    /// Enumeración para manejo de errores del framework
    /// </summary>
    public enum ETipoErrorFramework : int
    {
        /// <summary>
        /// Mensaje de error {0} no se encuentra configurado
        /// </summary>
        EX_MENSAJE_NO_CONFIGURADO = 0,

        /// <summary>
        /// No se encuentra registrado apropiadamente el equipo
        /// </summary>
        EX_ID_NO_IDENTIFICADO = 1,

        /// <summary>
        /// Usuario bloqueado por multiples intentos de autenticación
        /// </summary>
        EX_USUARIO_BLOQUEADO_INTENTOS_AUTENTICA = 2,

        /// <summary>
        /// La contraseña debe ser diferente a la que utilizo anteriormente.
        /// </summary>
        EX_PASSWORD_DIFERENTE = 3,

        /// <summary>
        /// Usuario o contraseña no validos.
        /// </summary>
        EX_USUARIO_CLAVE_NO_VALIDO = 4,

        /// <summary>
        /// Debe cambiar la clave.
        /// </summary>
        EX_DEBECAMBIAR_CLAVE = 5,

        /// <summary>
        /// El usuario esta bloqueado.
        /// </summary>
        EX_USUARIO_BLOQUEADO = 6,

        /// <summary>
        /// Contraseña invalida
        /// </summary>
        EX_CLAVE_INVALIDA = 7,

        /// <summary>
        /// Tiempo de vida del token de sesión no ha sido configurado
        /// </summary>
        EX_TIEMPO_VIDA_NO_CONFIGURADO = 8,

        /// <summary>
        /// La máquina solicitante se encuentra deshabilitada
        /// </summary>
        EX_IDMAQUINA_NOVALIDO = 9,

        /// <summary>
        /// El campo no existe.
        /// </summary>
        EX_CAMPO_NO_EXISTE = 10,

        /// <summary>
        /// Error desconocido contactese con servicio tecnico.
        /// </summary>
        EX_ERROR_DESCONOCIDO = 11,

        /// <summary>
        /// Error en el formato de la informacion.
        /// </summary>
        EX_ERROR_FORMATO_INFOR = 12,

        /// <summary>
        /// Error division por cero.
        /// </summary>
        EX_ERROR_DIVISION_CERO = 13,

        /// <summary>
        /// Error en operacion aritmetica.
        /// </summary>
        EX_ERROR_OPERACION_ARITMETICA = 14,

        /// <summary>
        /// Error en conversión de datos.
        /// </summary>
        EX_ERROR_CONVERSION_DATOS = 15,

        /// <summary>
        /// Operación invalida.
        /// </summary>
        EX_ERROR_OPERACION_INVALIDA = 16,

        /// <summary>
        /// Directorio no encontrado.
        /// </summary>
        EX_ERROR_DIRECTORIO_NO_ENCONTRADO = 17,

        /// <summary>
        /// Error cargando el archivo.
        /// </summary>
        EX_ERROR_CARGANDO_ARCHIVO = 18,

        /// <summary>
        /// Archivo no encontrado.
        /// </summary>
        EX_ERROR_ARCHIVO_NO_ENCONTRADO = 19,

        /// <summary>
        /// Memoria excedida
        /// </summary>
        EX_ERROR_MEMORIA_EXCEDIDA = 20,

        /// <summary>
        /// Proceso abortado.
        /// </summary>
        EX_ERROR_PROCESO_ABORTADO = 21,

        /// <summary>
        /// Error en interceptor.
        /// </summary>
        EX_ERROR_EN_INTERCEPTOR = 22,

        /// <summary>
        /// No se encontraron las credenciales de wcf desde el cliente.
        /// </summary>
        EX_ERROR_FALTA_CREDENCIALES_WCF = 23,

        /// <summary>
        /// Usuario o password de servicios incorrecto.
        /// </summary>
        EX_ERROR_USER_PASS_WCF_INCORRECTOS = 24,

        /// <summary>
        /// Cargo sin usuario asignado
        /// </summary>
        EX_ERROR_CARGO_SIN_USUARIO_ASIGNADO = 25,

        /// <summary>
        /// La tarea se encuentra configurada de forma incorrecta
        /// </summary>
        EX_ERROR_TAREA_MAL_CONFIGURADA = 26,

        /// <summary>
        /// El parámetro de límite máximo de archivos adjuntos no está configurado apropiadamente
        /// </summary>
        EX_PARAMETROS_ARCHIVOS_ADJUNTOS_NO_CONFIGURADO = 27,

        /// <summary>
        /// La versión no ha sido configurada apropiadamente para el usuario que no requiere Id
        /// </summary>
        EX_VERSION_NO_PARAMETRIZADA = 28,

        /// <summary>
        /// La falla no ha sido configurada apropiadamente
        /// </summary>
        EX_FALLA_MAL_CONFIGURADA = 29,

        /// <summary>
        /// La cadena de conexión al Directorio Activo es nula o vacía
        /// </summary>
        EX_CADENA_CONEXION_LDAP_NULA = 30,

        /// <summary>
        ///El usuario de Directorio Activo es nulo o vacío.
        /// </summary>
        EX_USUARIO_LDAP_NULO = 31,

        /// <summary>
        ///La clave del usuario de Directorio Activo es nula o vacía.
        /// </summary>
        EX_PASSWORD_USUARIO_LDAP_NULO = 32,

        /// <summary>
        ///No se obtuvo el dominio de la cadena de conexión del Directorio Activo
        /// </summary>
        EX_ERROR_OBTENER_DOMINIO_CADENA_CONEXION_LDAP = 33,

        /// <summary>
        /// El usuario No tiene autorización para asignar fallas porque no está asociado a un centro logístico
        /// </summary>
        EX_ERROR_USUARIO_NO_AUTORIZADO_ASIGNACION_TAREAS = 34,

        /// <summary>
        /// La consulta en la Base de Datos no retorno ningún resultado
        /// </summary>
        EX_CONSULTA_DB_NULL = 35,

        /// <summary>
        /// El usuario no existe.
        /// </summary>
        EX_USUARIO_NO_EXISTE = 36,

        /// <summary>
        /// Ya existe un registro con el mismo código.
        /// </summary>
        EX_REGISTRO_YA_EXISTE = 37,

        /// <summary>
        /// Tarea escalada y reasignada
        /// </summary>
        EX_TAREA_ESCALADA = 38,

        /// <summary>
        /// NO se encontraron usuarios activos para realizar el escalamiento
        /// </summary>
        EX_NO_USUARIOS_ACTIVOS_ESCALAMIENTO = 39,

        /// <summary>
        /// No se encontraron cargos superiores para hacer el escalamiento
        /// </summary>
        EX_NO_CARGOS_SUPERIORES = 40,

        /// <summary>
        /// El usuario asignado no existe o no es válido
        /// </summary>
        EX_USUARIO_ASIGNADO_NO_EXISTE = 41,

        /// <summary>
        /// El cargo asignado no existe o no es válido
        /// </summary>
        EX_CARGO_NO_EXISTE = 42,

        /// <summary>
        /// Error desconocido en LDAP.
        /// </summary>
        EX_ERROR_DESCONOCIDO_LDAP = 43,

        /// <summary>
        /// No se puede eliminar por que presenta registros asociados.
        /// </summary>
        EX_ERROR_BORRANDO_FK = 44,

        /// <summary>
        /// El usuario para hacer la asignación de la tarea no está activo en el sistema
        /// </summary>
        EX_USUARIO_NO_ACTIVO_O_NO_VALIDO_ATENDER_TAREA = 45,

        /// <summary>
        /// Fallo proceso de cambio de cambio de contraseña.
        /// </summary>
        EX_ERROR_CAMBIANDO_PASSWORD = 46,

        /// <summary>
        /// Cuando una identificación se encuentra dentro de las listas restrictivas
        /// </summary>
        EX_ALERTA_LISTAS_RESTRICTIVAS = 51,

        /// <summary>
        /// Cuando no se ha agragado a la tabla la zona general (id = -1)
        /// </summary>
        EX_FALTA_ZONA_GENERAL = 52,

        /// <summary>
        /// Cuando el centro de servicio no tiene servicios asignados o no se encuentra habilitado el centro de servicio
        /// </summary>
        EX_CENTRO_SERVICIO_SIN_SERVICIOS = 53,

        /// <summary>
        /// Cuando el usuario no pertenece a la regional a la que pertenece el equpo dónde se está autenticando
        /// </summary>
        EX_USUARIO_NO_PERTENECE_REGIONAL_MAQUINA = 54,

        /// <summary>
        /// Centro de servicios no válido
        /// </summary>
        EX_CENTRO_SERVICIO_NO_VALIDO = 55,

        /// <summary>
        /// Sucursal no válida
        /// </summary>
        EX_SUCURSAL_NO_VALIDA = 56,

        /// <summary>
        /// Versión del cliente no configurada
        /// </summary>
        EX_VERSION_CLIENTE_NO_CONFIGURADA = 57,

        /// <summary>
        /// Indica que la configuración del cliente no es válida porque no tiene registradas sucursales
        /// </summary>
        EX_CONFIGURACION_CLIENTE_INVALIDA = 58,

        /// <summary>
        /// Indica que la sucursal no tiene servicios de mensajeria asignados
        /// </summary>
        EX_SUCURSAL_SIN_SERVICIOS = 59,

        /// <summary>
        /// Indica que la No existe un consecutivo habilitado
        /// </summary>
        EX_NO_SE_PUEDE_ASIGNAR_CONSECUTIVO = 60,

        /// <summary>
        /// Error al adjuntar un archivo
        /// </summary>
        EX_FALLO_ADJUNTAR_ARCHIVO = 61,

        /// <summary>
        /// Error, alerta no configurada
        /// </summary>
        EX_ALERTA_NO_CONFIGURADA = 62,

        /// <summary>
        /// Error, no se pudo crear la planilla
        /// </summary>
        EX_ERROR_PLANILLA = 63,

        /// <summary>
        /// Error validando los campos ingresados.
        /// </summary>
        EX_ERROR_ENTITY_VALITATION = 64,

        /// <summary>
        /// Error en solicitud de Numerador.
        /// </summary>
        EX_ERROR_OBTENER_NUMERADOR = 65,

        /// <summary>
        /// Error de restricción por base de datos, intente nuevamente cargando los datos.
        /// </summary>
        EX_ERROR_EN_CONSTRAINT = 66,

        /// <summary>
        /// Error del punto no tiene una comision asignada
        /// </summary>
        EX_ERROR_NO_TIENE_COMISION_ASIGNADA = 67,

        /// <summary>
        /// Error al recibir el parametro en cero de la base de la comisión
        /// </summary>
        EX_ERROR_VALOR_BASE_COMISION_ES_CERO = 68,

        /// <summary>
        /// Error en el porcentaje de la comisión del responsable
        /// </summary>
        EX_ERROR_PORCENTAJE_COMISION_RESPONSABLE = 69,

        /// <summary>
        /// Error en el valor de la comisión del responsable
        /// </summary>
        EX_ERROR_VALOR_COMISION_RESPONSABLE = 70,

        /// <summary>
        /// Error en el valor del porcentaje de la comisión del Centro de Servicio.
        /// </summary>
        EX_ERROR_PORCENTAJE_COMISION_CENTROSERVICIO = 71,

        /// <summary>
        /// Error en el valor de la comisión del Centro de Servicio.
        /// </summary>
        EX_ERROR_VALOR_COMISION_CENTROSERVICIO = 72,

        /// <summary>
        /// Error lanzado cuando se viola la integridad referencial.
        /// </summary>
        EX_ERROR_VIOLACION_INEGRIDAD_REFERENCIAL = 73,

        /// <summary>
        /// Error lanzado cuando no existe un area interna default.
        /// </summary>
        EX_ERROR_AREA_INTERNA_DEFAULT = 74,

        /// <summary>
        /// Error lanzado cuando no se puede conectar al web service de cálculo del precio del dólar
        /// </summary>
        EX_ERROR_NO_SE_PUDO_CONECTAR_WS_DOLAR = 75,

        /// <summary>
        /// No existe una caja para editar para el tipo de consecutivo.
        /// </summary>
        EX_ERROR_NO_SE_PUDO_EDITAR_CAJA_CONSECUTIVO = 76,

        /// <summary>
        /// No existe un consecutivo para el tipo de consecutivo enviado.
        /// </summary>
        EX_ERROR_NO_EXISTE_TIPO_CONSECUTIVO = 77,

        /// <summary>
        /// Cuando los Parametros para ejecutar la regla llegan nulos.
        /// </summary>
        EX_ERROR_PARAMETROS_NULOS_EJECUCION_REGLA = 78,

        /// <summary>
        /// Mensaje de cuando no esta implemetada la regla.
        /// </summary>
        EX_ERROR_REGLA_NO_IMPLEMENTADA = 79,

        /// <summary>
        /// Usuario intenta acceder a una carpeta a la cual no tiene permisos de Windows.
        /// </summary>
        EX_NO_TIENE_PERMISOS_SOBRE_CARPETA_PRIVADA_WIN = 80,

        /// <summary>
        /// Error de acceso a parametro
        /// </summary>
        EX_NO_EXISTE_PARAMETRO_CONFIGURADO = 81,

        /// <summary>
        /// Mensaje de error cuando una racol no tiene configurada una versión.
        /// </summary>
        EX_NO_EXISTE_VERSION_RACOL = 82,

        /// <summary>
        /// Mensaje de error cuando una gestión no tiene configurada una versión.
        /// </summary>
        EX_NO_EXISTE_VERSION_GESTION = 83,

        /// <summary>
        /// Mensaje informativo informando que debe esperar a que le autoricen la máquina.
        /// </summary>
        IN_MAQUINA_REGISTRADA = 84,

        /// <summary>
        /// Mensaje de error cuando no esta configurado el valor del porcentaje de recargo
        /// de combustible del operador postal
        /// </summary>
        EX_NO_EXISTE_PORCENTAJE_RECARGO_COMBUSTIBLE_OP = 85,

        /// <summary>
        /// Se presentó un error de datos, por favor comunicarse con soporte técnico
        /// </summary>
        EX_ERROR_ENTITY_DETALLE = 86,

        /// <summary>
        /// No se encontraron destinatarios para el correo
        /// </summary>
        EX_NO_EXISTEN_DESTINATARIOS_CONFIGURADOS = 87,

        /// <summary>
        /// El usuario ya existe en la base de datos
        /// </summary>
        EX_USUARIO_YA_EXISTE = 89,

        EX_PERSONA_INTERNA_ASOCIADA = 90,

        /// <summary>
        /// Usuario de la persona interna ya activa
        /// </summary>
        EX_USUARIO_ACTIVO_DE_PERSONA_INTERNA = 91,

        /// <summary>
        /// Usuario de la persona interna ya activa
        /// </summary>
        EX_USUARIO_ACTIVO_PARA_UNA_PERSONA_INTERNA = 92,

        /// <summary>
        ///el usuario y la persona interna estan activas
        /// </summary>
        EX_DOCUMENTO_PERSONA_ACTIVO_USUARIO_ACTIVO = 93,

        /// <summary>
        /// Mensaje de error cuando el usuario esta activo i no asignado
        /// </summary>
        EX_USUARIO_ACTIVO_SIN_ASIGNAR = 94,

        /// <summary>
        /// No se encontró el inventario del consolidado
        /// </summary>
        EX_NO_ENCONTRO_CODIGO_INVENTARIO=95,

        /// <summary>
				/// Mensaje de error cuando se esta realizando la asignación de tulas entre diferentes centros de servicio.
        /// </summary>
        EX_DIFERENTE_CENTRO_SERVICIO=96
    }
}