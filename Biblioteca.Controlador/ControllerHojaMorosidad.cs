using System;
using System.Collections.Generic;
using System.Data;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerHojaMorosidad : ControllerLog
    {
        #region Attribute
        private List<HojaMorosidad> HojaMorosidadPersistence { get; set; }
        #endregion
        #region Insert method
        /// <summary>
        /// Insert data into Hoja_morosidad table
        /// </summary>
        /// <param name="nroFicha">Nro Ficha associated</param>
        /// <param name="diasAtrasados">Days of delay on return Libro</param>
        /// <param name="codLibro">Cod. Libro delayed</param>
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
        #endregion
        #region Fetch method
        /// <summary>
        /// Fetch a list of Hoja de Morosidad from Hoja_morosidad Table and set into persistence
        /// </summary>
        /// <param name="nroFicha">Nro Ficha to search</param>
        /// <returns>Message that indicates if the fetch was successful or not</returns>
        public Message FetchHojaMorosidad(int nroFicha)
        {
            var table = Select("SELECT * FROM Hoja_morosidad WHERE nro_ficha = @NroFicha;", new[] {"@NroFicha"}, new object[] {nroFicha});
            if (table.Rows.Count == 0) return new Message(false, "Estudiante no tiene hoja de morosidad");
            HojaMorosidadPersistence = new List<HojaMorosidad>();
            for (var i = 0; i < table.Rows.Count; i++)
            {
                HojaMorosidadPersistence.Add(new HojaMorosidad(table.Rows[i].Field<DateTime>("fecha"), table.Rows[i].Field<string>("sancion"),
                    table.Rows[i].Field<int>("dias_atraso"),table.Rows[i].Field<int>("nro_ficha"),
                    table.Rows[i].Field<string>("cod_libro")));
            }

            Log(string.Format("Visualizó morosidad. Ficha: {0}", nroFicha));

            return new Message(true);
        }
        #endregion
    }
}