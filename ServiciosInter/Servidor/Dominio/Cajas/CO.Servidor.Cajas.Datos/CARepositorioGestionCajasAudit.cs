using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CO.Servidor.Cajas.Datos.Modelo;
using Framework.Servidor.Excepciones;

namespace CO.Servidor.Cajas.Datos
{
  internal static class CARepositorioGestionCajasAudit
  {
    internal static void MapeoConceptoCaja(ModeloCajas contexto, string usuario)
    {
      contexto.Audit<ConceptoCaja_CAJ, ConceptoCajaHist_CAJ>((record, action) => new ConceptoCajaHist_CAJ
              {
                COC_CambiadoPor = usuario,
                COC_Descripcion = record.Field<ConceptoCaja_CAJ, string>(f => f.COC_Descripcion),
                COC_EsIngreso = record.Field<ConceptoCaja_CAJ, bool>(f => f.COC_EsIngreso),
                COC_FechaCambio = DateTime.Now,
                COC_CreadoPor = record.Field<ConceptoCaja_CAJ, string>(f => f.COC_CreadoPor),
                COC_EsServicio = record.Field<ConceptoCaja_CAJ, bool>(f => f.COC_EsServicio),
                COC_FechaGrabacion = record.Field<ConceptoCaja_CAJ, DateTime>(f => f.COC_FechaGrabacion),
                COC_IdConceptoCaja = record.Field<ConceptoCaja_CAJ, int>(f => f.COC_IdConceptoCaja),
                COC_IdCuentaExterna = record.Field<ConceptoCaja_CAJ, short?>(f => f.COC_IdCuentaExterna),
                COC_Nombre = record.Field<ConceptoCaja_CAJ, string>(f => f.COC_Nombre),
                COC_ContrapartidaCasaMatriz = record.Field<ConceptoCaja_CAJ, bool>(f => f.COC_ContrapartidaCasaMatriz),
                COC_ContrapartidaCS = record.Field<ConceptoCaja_CAJ, bool>(f => f.COC_ContrapartidaCS),
                COC_RequiereNoDocumento = record.Field<ConceptoCaja_CAJ, bool>(f => f.COC_RequiereNoDocumento),
                COC_TipoCambio = action.ToString()
              }, (ph) => contexto.ConceptoCajaHist_CAJ.Add(ph));
    }
  }
}
