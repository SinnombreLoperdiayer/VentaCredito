using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Servicio.Entidades.GestionCajas;
using VentaCredito.Cajas.Datos.RepositorioCasaMatriz;
using Servicios.Entidades.Cajas;

namespace VentaCredito.Cajas.CajaFinaciera
{
    public class CACajaCasaMatriz
    {
        private static readonly CACajaCasaMatriz instancia = new CACajaCasaMatriz();

        /// <summary>
        /// Retorna una instancia de Consultas de admision guía interna
        /// /// </summary>
        public static CACajaCasaMatriz Instancia
        {
            get { return CACajaCasaMatriz.instancia; }
        }

        /// <summary>
        /// Registrar operación en la tabla de caja Casa Matriz
        /// </summary>
        /// <param name="idApertura">Identificador de la apertura de caja</param>
        /// <param name="infoTransaccion">Información de la operación</param>
        /// <returns>Identificador con el cual queda guardado el registro de la operación</returns>
        public long RegistrarOperacion(long idApertura, CACajaCasaMatrizDC infoTransaccion)
        {
            CACajaCasaMatrizDC operacion = new CACajaCasaMatrizDC()
            {
                //CAE_ConceptoEsIngreso = infoTransaccion.ConceptoCaja.EsIngreso,
                //CAE_CreadoPor = infoTransaccion.CreadoPor,
                //CAE_Descripcion = infoTransaccion.Descripcion ?? String.Empty,
                //CAE_FechaGrabacion = DateTime.Now,
                //CAE_FechaMovimiento = infoTransaccion.FechaMov,
                //CAE_IdAperturaCaja = idApertura,
                //CAE_IdConceptoCaja = infoTransaccion.ConceptoCaja.IdConceptoCaja,
                //CAE_NombreConceptoCaja = infoTransaccion.ConceptoCaja.Nombre,
                //CAE_MovHechoPor = infoTransaccion.MovHechoPor,
                //CAE_NumeroDocumento = infoTransaccion.NumeroDocumento,
                //CAE_Observacion = infoTransaccion.Observacion ?? String.Empty,
                //CAE_Valor = infoTransaccion.Valor
            };

            //if (operacion.CAE_FechaMovimiento.Equals(DateTime.MinValue))
            //{
            //    operacion.CAE_FechaMovimiento = operacion.CAE_FechaGrabacion;
            //}

            //return OperacionCajaCasaMatriz.Instancia.AdicionarOperacionCajaCasaMatriz(operacion);
            return 0;
        }
    }
}
