using CO.Servidor.Dominio.Comun.AdmEstadosGuia.Datos;
using CO.Servidor.Dominio.Comun.Admisiones;
using CO.Servidor.Dominio.Comun.Middleware;
using CO.Servidor.Servicios.ContratoDatos.Admisiones.Mensajeria;
using CO.Servidor.Servicios.ContratoDatos.Comun;
using CO.Servidor.Servicios.ContratoDatos.Tarifas;
using Framework.Servidor.Comun;
using Framework.Servidor.ParametrosFW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CO.Servidor.Dominio.Comun.AdmEstadosGuia
{
    public static class EGTipoNovedadGuia
    {

        /// <summary>
        /// Obtiene los tipos de novedad de una guia de acuerdo al tipo
        /// </summary>
        /// <returns></returns>
        public static List<COTipoNovedadGuiaDC> ObtenerTiposNovedadGuia(COEnumTipoNovedad tipoNovedad)
        {
            return EGRepositorio.Instancia.ObtenerTiposNovedadGuia(tipoNovedad);
        }

        /// <summary>
        /// Método para afectar la fecha estimada de entrega
        /// </summary>
        /// <param name="cambioFecha"></param>
        public static void CambiarFechaEntrega(COCambioFechaEntregaDC cambioFecha)
        {
            string archivo = @"c:\logExcepciones\Debug.txt";
            System.IO.FileInfo f = new System.IO.FileInfo(archivo);
            System.IO.StreamWriter writer;
            if (!f.Exists) // si no existe entonces lo crea, si ya existe NO lo crea
            {
                writer = f.CreateText();
                writer.Close();
            }


            DateTime fechaDigitalizacion;
            DateTime fechaEntregaNew;
            DateTime fechaArchivo;
            CODatosCambioFechasDC tiemposGuia = new CODatosCambioFechasDC();



            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 1 ");
                writer.Close();
            }

            tiemposGuia = EGRepositorio.Instancia.ObtenerDatosParaCambiosDeFecha(cambioFecha.Guia.NumeroGuia);

            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 2 ");
                writer.Close();
            }
            fechaEntregaNew = PAAdministrador.Instancia.ObtenerFechaFinalHabilSinSabados(tiemposGuia.FechaEstimadaEntregaNew, (cambioFecha.TiempoAfectacion / 24), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 3 ");
                writer.Close();
            }
            fechaEntregaNew = new DateTime(fechaEntregaNew.Year, fechaEntregaNew.Month, fechaEntregaNew.Day, 18, 0, 0);
            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 4 ");
                writer.Close();
            }
            fechaDigitalizacion = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaEntregaNew, (tiemposGuia.Tiempos.numeroDiasDigitalizacion), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 5 ");
                writer.Close();
            }
            fechaArchivo = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaDigitalizacion, tiemposGuia.Tiempos.numeroDiasArchivo, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 6 ");
                writer.Close();
            }
            cambioFecha.FechaEntrega = tiemposGuia.FechaEstimadaEntregaNew;
            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 7 ");
                writer.Close();
            }
            cambioFecha.FechaNuevaEntrega = fechaEntregaNew;
            cambioFecha.Guia.FechaEstimadaDigitalizacion = fechaDigitalizacion;
            cambioFecha.Guia.FechaEstimadaArchivo = fechaArchivo;

            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 8 ");
                writer.Close();
            }
            EGRepositorio.Instancia.CambiarFechaEntrega(cambioFecha);

            if (Framework.Servidor.Excepciones.ControllerContext.Current.Usuario == "gerencia.cm2")
            {
                writer = f.AppendText();
                writer.WriteLine("CambiarFechaEntrega => 9 ");
                writer.Close();
            }
        }

        /// <summary>
        /// Método para afectar la fecha estimada de entrega calendario recibe tiempo en dias
        /// </summary>
        /// <param name="cambioFecha"></param>
        public static void CambiarFechaEntregaCalendario(COCambioFechaEntregaDC cambioFecha)
        {
            DateTime fechaDigitalizacion;
            DateTime fechaEntregaNew;
            DateTime fechaArchivo;
            CODatosCambioFechasDC tiemposGuia = new CODatosCambioFechasDC();
            tiemposGuia = EGRepositorio.Instancia.ObtenerDatosParaCambiosDeFecha(cambioFecha.Guia.NumeroGuia);
            fechaEntregaNew = DateTime.Now.Date.AddDays(cambioFecha.TiempoAfectacion);
            fechaEntregaNew = new DateTime(fechaEntregaNew.Year, fechaEntregaNew.Month, fechaEntregaNew.Day, 18, 0, 0);
            fechaDigitalizacion = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaEntregaNew, (tiemposGuia.Tiempos.numeroDiasDigitalizacion), PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            fechaArchivo = PAAdministrador.Instancia.ObtenerFechaFinalHabil(fechaDigitalizacion, tiemposGuia.Tiempos.numeroDiasArchivo, PAAdministrador.Instancia.ConsultarParametrosFramework(ConstantesFramework.PARA_PAIS_DEFAULT));
            cambioFecha.FechaEntrega = tiemposGuia.FechaEstimadaEntregaNew;
            cambioFecha.FechaNuevaEntrega = fechaEntregaNew;
            cambioFecha.Guia.FechaEstimadaDigitalizacion = fechaDigitalizacion;
            cambioFecha.Guia.FechaEstimadaArchivo = fechaArchivo;
            EGRepositorio.Instancia.CambiarFechaEntrega(cambioFecha);
        }

       
        /// <summary>
        /// Obtiene los sabados entre una fecha y otra
        /// </summary>
        /// <returns></returns>
        public static int ContadorSabados(DateTime fechaInicio, DateTime fechaFin)
        {
            int cuentaSabados = 0;
            fechaInicio = new DateTime(fechaInicio.Year, fechaInicio.Month, fechaInicio.Day);
            fechaFin = new DateTime(fechaFin.Year, fechaFin.Month, fechaFin.Day);
            while (fechaInicio <= fechaFin)
            {
                if (fechaInicio.DayOfWeek == 0)
                {
                    cuentaSabados++;
                }
                fechaInicio = fechaInicio.AddDays(1);
            }
            return cuentaSabados;
        }

        /// <summary>
        /// Metodo para afectar la fecha estimada de entrega para el dia anterior.
        /// </summary>
        /// <param name="cambioFecha"></param>
        public static void CambiarFechaEntregaDiaAnterior(COCambioFechaEntregaDC cambioFecha)
        {
            EGRepositorio.Instancia.CambiarFechaEntregaDiaAnterior(cambioFecha);

        }
    }
}
