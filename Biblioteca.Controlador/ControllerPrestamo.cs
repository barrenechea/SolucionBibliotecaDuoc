using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerPrestamo : ControllerDatabase
    {
        private Prestamo PrestamoPersistence { get; set; }
        private List<DetallePrestamo> DetPrestamoPersistence { get; set; }

        #region Preload method

        /// <summary>
        /// This method is to preload a Prestamo into the controller before requesting an Insert or Update query.
        /// </summary>
        /// <param name="nroFicha">Número de ficha del solicitante</param>
        /// <param name="codLibros">Codigos de libros que se solicitan</param>
        /// <param name="devolucion">Días en los que se debe devolver el libro</param>
        /// <returns>Message, indicates status of the preload, and a message in case of failure</returns>
        public Message PreloadPrestamo(int nroFicha, string[] codLibros, int devolucion)
        {
            Message msg;
            try
            {
                //toDo Problema: si más de una bibliotecaria agrega un prestamo al mismo tiempo 
                PrestamoPersistence = new Prestamo(DateTime.Now, nroFicha);
                var codPrestamo = CodigoLibro();
                foreach (var codLibro  in codLibros)
                {
                    DetPrestamoPersistence.Add(new DetallePrestamo(SumarDias(DateTime.Now, devolucion), false, 0, codPrestamo, codLibro));
                }
                msg = new Message(true);
            }
            catch
            {
                msg = new Message(false, "Verifique los parámetros");
            }
            return msg;
        }
        #endregion

        #region Querys
        public Message Insert()
        {
            if (PrestamoPersistence == null || DetPrestamoPersistence == null) return new Message(false, "Debe precargar los datos del préstamo");

            string sqlSentence;
            string[] arrayParameters;
            object[] arrayObjects;

            sqlSentence = "INSERT INTO Prestamo (fec_prestamo, nro_ficha) " +
                          "VALUES (@FecPrestamo, @NroFicha);";
            arrayParameters = new[] { "@FecPrestamo", "@NroFicha" };
            arrayObjects = new object[] { DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), PrestamoPersistence.NroFicha };

            var executePre = Execute(sqlSentence, arrayParameters, arrayObjects);


            #region Insert into Detalle prestamo

            foreach (var dp in DetPrestamoPersistence)
            {
                sqlSentence = "INSERT INTO Detalle_prestamo(fec_devolucion, libro_devuelto, renovacion, cod_prestamo, cod_libro) " +
                          "VALUES (@FecDevolucion, @LibroDevuelto, @Renovacion, @CodPrestamo, @CodLibro);";

                arrayParameters = new[] { "@FecDevolucion", "@LibroDevuelto", "@Renovacion", "@CodPrestamo", "@CodLibro" };
                arrayObjects = new object[] { dp.FecDevolucion, dp.LibroDevuelto, dp.Renovacion, dp.CodPrestamo, dp.CodLibro };
                Execute(sqlSentence, arrayParameters, arrayObjects);
            }

            #endregion
            return executePre;
        }
        #endregion

        #region Utils

        public Message UsuarioActivado(string nroFicha)
        {
            var estado = Select("SELECT estado from Usuario WHERE nro_ficha = @Nro_ficha;", new[] { "@Nro_ficha" }, new object[] { nroFicha });
            return new Message(estado.Rows[0].Field<bool>(0), estado.Rows[0].Field<bool>(0) ? null : "El usuario está desactivado, revise hoja de morosidad.");
        }

        public Message ExistsNroFicha(string nroFicha)
        {
            var exists = Select("SELECT * from Usuario WHERE nro_ficha = @NroFicha;", new[] { "@NroFicha" }, new object[] { nroFicha }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No se ha encontrado el número de ficha");
        }

        public Message ExistsLibro(string codLibro)
        {
            var exists = Select("SELECT * from Libro WHERE cod_libro = @CodLibro;", new[] { "@CodLibro" }, new object[] { codLibro }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No se ha encontrado el libro");
        }

        public Message LibroDisponible(string codLibro)
        {
            var exists = Select("SELECT * from Libro WHERE cod_libro = @CodLibro and nro_copias > 0;", new[] { "@CodLibro" }, new object[] { codLibro }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No quedan libros disponibles");
        }

        public int CodigoLibro()
        {
            var codigo = Select("SELECT MAX(cod_prestamo) FROM prestamo;");
            return codigo.Rows[0].Field<int>(0);
        }

        public DateTime SumarDias(DateTime fecha, int dias)
        {
            for (var i = 0; i < dias; i++)
            {
                fecha = fecha.AddDays(1);
                if (fecha.DayOfWeek == DayOfWeek.Sunday || fecha.DayOfWeek == DayOfWeek.Saturday)
                    i--;
            }
            return fecha;
        }
        #endregion
    }
}