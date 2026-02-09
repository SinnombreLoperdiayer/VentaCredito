
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.CentroServicios;
using CO.Servidor.Servicios.ContratoDatos.OperacionUrbana;
using CO.Servidor.Servicios.ContratoDatos.Raps.Configuracion;
using CO.Servidor.Servicios.ContratoDatos.Raps.Solicitudes;
using CO.Servidor.Servicios.ContratoDatos.Recogidas;
using Framework.Servidor.Servicios.ContratoDatos.Parametros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CO.Servidor.RAPS.Reglas.Parametros
{
    public class RAFallaMapper
    {
        public RADatosFallaDC MapperDatosFallaAutomaticaMensajeroGuiaLogistica(OUGuiaIngresadaDC datos, int idSistema)
        {
            RADatosFallaDC resultado = new RADatosFallaDC();


            if (!datos.Equals(null))
            {
                resultado = new RADatosFallaDC()
                {
                    IdSistema = idSistema,
                    IdMotivoGuia = datos.Motivo == null ? Convert.ToInt16(0) : datos.Motivo.IdMotivoGuia,
                    NombreCompleto = datos.NombreCompleto == null ? string.Empty : datos.NombreCompleto.Split('-')[1],
                    IdCentroServicioDestino = datos.IdCentroServicioDestino,
                    Ciudad = datos.Ciudad == null ? string.Empty : datos.Ciudad,
                    IdCentroLogistico = datos.IdCentroLogistico,
                    FechaMotivoDevolucion = datos.FechaMotivoDevolucion == null ? new DateTime() : datos.FechaMotivoDevolucion,
                    Observaciones = datos.Observaciones == null ? string.Empty : datos.Observaciones,
                    NumeroGuia = datos.NumeroGuia,
                    NombreCentroServicioDestino = datos.NombreCentroServicioDestino == null ? string.Empty : datos.NombreCentroServicioDestino,
                    IdCiudad = datos.IdCiudad == null ? string.Empty : datos.IdCiudad,
                    DocPersonaResponsable = datos.NombreCompleto == null ? string.Empty : datos.NombreCompleto.Split('-')[0].Trim(),
                    //Adjunto = "",
                };


            }
            return resultado;
        }
        public RADatosFallaDC MapperDatosFallaAutomaticaAgenciaGuiaLogistica(OUGuiaIngresadaDC datos, int idSistema)
        {
            RADatosFallaDC resultado = new RADatosFallaDC();


            if (!datos.Equals(null))
            {
                resultado = new RADatosFallaDC()
                {
                    IdSistema = idSistema,
                    IdMotivoGuia = datos.Motivo == null ? Convert.ToInt16(0) : datos.Motivo.IdMotivoGuia,
                    NombreCompleto = datos.NombreCentroServicioOrigen,
                    DocPersonaResponsable = datos.IdCentroServicioOrigen.ToString(),
                    IdCiudad = datos.IdCiudad == null ? string.Empty : datos.IdCiudad,
                    Ciudad = datos.Ciudad == null ? string.Empty : datos.Ciudad,
                    Observaciones = datos.Observaciones == null ? "" : datos.Observaciones,
                    IdCentroServicioDestino = datos.IdCentroServicioDestino,
                    IdCentroLogistico = datos.IdCentroLogistico,
                    FechaMotivoDevolucion = datos.FechaMotivoDevolucion == null ? new DateTime() : datos.FechaMotivoDevolucion,
                    NumeroGuia = datos.NumeroGuia,
                    NombreCentroServicioDestino = datos.NombreCentroServicioDestino == null ? string.Empty : datos.NombreCentroServicioDestino,

                };


            }
            return resultado;
        }
        public RADatosFallaDC MapperDatosFallaAutomaticaGuia(ADGuia datos, RGEmpleadoDC datosEmpleado, int idSistema)
        {
            RADatosFallaDC resultado = new RADatosFallaDC();


            if (!datos.Equals(null))
            {
                resultado = new RADatosFallaDC()
                {
                    IdSistema = idSistema,
                    NombreCompleto = datosEmpleado.nombreEmpleado,
                    IdCentroServicioDestino = datos.IdCentroServicioDestino,
                    Ciudad = datos.NombreCiudadOrigen,
                    Observaciones = datos.Observaciones,
                    NumeroGuia = datos.NumeroGuia,
                    DocPersonaResponsable = datosEmpleado.idEmpleado,
                    NombreCentroServicioDestino = datos.NombreCentroServicioDestino,
                    IdCiudad = datos.IdCiudadOrigen,

                };


            }
            return resultado;
        }

        public RADatosFallaDC MapperDatosFallaAutomaticaRecogidas(RGAsignarRecogidaDC datos, int idSistema)
        {
            RADatosFallaDC resultado = null;


            if (!datos.Equals(null))
            {
                resultado = new RADatosFallaDC()
                {
                    IdSistema = idSistema,
                    IdCentroServicioDestino = datos.Mensajero == null ? 0 : datos.Mensajero.IdCentroServicios,
                    NombreCompleto = datos.Mensajero == null ? string.Empty : datos.Mensajero.NombreMensajero,
                    IdCliente = datos.IdCliente.ToString(),
                    NombreCliente = datos.NombreCliente.ToString(),
                    Observaciones = datos.Observacion.ToString(),
                    IdCiudad = datos.IdCiudad.ToString(),
                    FechaProgramacionRecogida = datos.FechaProgramacionRecogida,
                    DocPersonaResponsable = datos.DocPersonaResponsable.ToString(),
                    DireccionCliente = datos.DireccionCliente.ToString(),
                };


            }
            return resultado;
        }

        ///TODO: pendiente por mapear el objeto enviado desde novedades
        public RADatosFallaDC MapperDatosFallaCentroAcopio(PUCentroServiciosDC datos, int idSistema, long numeroGuia, long numeroEnvioNN)
        {
            RADatosFallaDC resultado = new RADatosFallaDC();


            if (!datos.Equals(null))
            {
                resultado = new RADatosFallaDC()
                {
                    IdSistema = idSistema,
                    NumeroGuia = numeroGuia,
                    Observaciones = numeroEnvioNN.ToString(),
                    DocPersonaResponsable = datos.IdCentroServicio.ToString(),
                    NombreCompleto = datos.Nombre,
                    IdCiudad = datos.CiudadUbicacion == null ? string.Empty : datos.CiudadUbicacion.IdLocalidad,
                };


            }
            return resultado;
        }

    }
}
