using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Controlador
{
    public class ControllerHojaMorosidad : ControllerLog
    {
        public void Insert(int nroFicha, int diasAtrasados, string codLibro)
        {
            string sancion;
            if (diasAtrasados <= 2)
                sancion = "3 días sin pedir libros";
            else if (diasAtrasados <= 4)
                sancion = "5 días sin pedir libros.";
            else
                sancion = "7 días de suspensión más citación al apoderado.";

            var sqlSentence = "INSERT INTO Hoja_morosidad(fecha, sancion, dias_atraso, nro_ficha, cod_libro) VALUES (@Fecha, @Sancion, @DiasAtraso, @NroFicha, @CodLibro)";
            var arrayParameters = new[] { "@Fecha", "@Sancion", "@DiasAtraso", "@NroFicha", "@CodLibro" };
            var arrayObjects = new object[] { DateTime.Now, sancion, diasAtrasados, nroFicha, codLibro };
            Execute(sqlSentence, arrayParameters, arrayObjects);

            Execute("UPDATE Usuario set estado = 0 WHERE nro_ficha = @NroFicha", new[] { "@NroFicha" }, new object[] { nroFicha });

            Log(string.Format("Agregó morosidad a ficha {0}", nroFicha));
        }
    }
}
