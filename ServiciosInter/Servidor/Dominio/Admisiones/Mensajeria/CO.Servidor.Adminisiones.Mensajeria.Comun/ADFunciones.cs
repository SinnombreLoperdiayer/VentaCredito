using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CO.Servidor.Adminisiones.Mensajeria.Comun
{
   public static  class ADFunciones
    {

        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                     RegexOptions.None);
            }

            catch (Exception )
            {
                return String.Empty;
            }
        }
    }
}
