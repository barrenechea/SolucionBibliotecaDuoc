using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerHojaMorosidad : ControllerLog
    {
        public void Insert(int nroFicha, int diasAtrasados, string codLibro)
        {
            string sancion;
            if (diasAtrasados <= 2)
                sancion = "A";
            else if (diasAtrasados <= 4)
                sancion = "B";
            else
                sancion = "C";

            var sqlSentence = "INSERT INTO Hoja_morosidad(fecha, sancion, dias_atraso, nro_ficha, cod_libro) VALUES (@Fecha, @Sancion, @DiasAtraso, @NroFicha, @CodLibro)";
            var arrayParameters = new[] { "@Fecha", "@Sancion", "@DiasAtraso", "@NroFicha", "@CodLibro" };
            var arrayObjects = new object[] { DateTime.Now, sancion, diasAtrasados, nroFicha, codLibro };
            Execute(sqlSentence, arrayParameters, arrayObjects);

            Execute("UPDATE Usuario set estado = 0 WHERE nro_ficha = @NroFicha", new[] { "@NroFicha" }, new object[] { nroFicha });

            Log(string.Format("Agregó morosidad a ficha {0}", nroFicha));
        }

        public Message FetchHojaMorosidad(int nroFicha)
        {
            var table = Select("SELECT COUNT(*) FROM Hoja_morosidad WHERE nro_ficha = @NroFicha;", new[] {"@NroFicha"},
                new object[] {nroFicha});
            return (int)table.Rows[0].Field<long>(0) == 0 ? new Message(false, "Estudiante no tiene hoja de morosidad") : new Message(true);
        }
    }
}