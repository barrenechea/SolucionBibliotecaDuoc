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
        public List<HojaMorosidad> HojaMorosidadPersistence { get; set; }

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
            var table = Select("SELECT * FROM Hoja_morosidad WHERE nro_ficha = @NroFicha;", new[] {"@NroFicha"},
                new object[] {nroFicha});
            if (table.Rows.Count == 0)
                return new Message(false, "Estudiante no tiene hoja de morosidad");
            HojaMorosidadPersistence = new List<HojaMorosidad>();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                HojaMorosidadPersistence.Add(new HojaMorosidad(table.Rows[i].Field<DateTime>("fecha"), table.Rows[i].Field<string>("sancion"),
                    table.Rows[i].Field<int>("dias_atraso"),table.Rows[i].Field<int>("nro_ficha"),
                    table.Rows[i].Field<string>("cod_libro")));
            }
            return new Message(true);
        }
    }
}