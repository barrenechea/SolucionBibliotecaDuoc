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
                PrestamoPersistence = new Prestamo(DateTime.Now, nroFicha);
                var codPrestamo = CodigoLibro(nroFicha);
                foreach (var codLibro  in codLibros)
                {
                    DetPrestamoPersistence.Add(new DetallePrestamo(SumarDias(DateTime.Now, devolucion), false, 0, codPrestamo, codLibro.Trim()));
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
                arrayObjects = new object[] { dp.FecDevolucion, dp.LibroDevuelto, dp.Renovacion, dp.CodPrestamo, dp.CodLibro.Trim() };
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
            return new Message(estado.Rows[0].Field<bool>(0), estado.Rows[0].Field<bool>(0) ? null : "Usuario tiene sanción activa.");
        }

        public Message ExistsNroFicha(string nroFicha)
        {
            var exists = Select("SELECT * from Usuario WHERE nro_ficha = @NroFicha;", new[] { "@NroFicha" }, new object[] { nroFicha }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No se ha encontrado el número de ficha");
        }

        public Message ExistsLibro(string codLibro)
        {
            var exists = Select("SELECT * from Libro WHERE cod_libro = @CodLibro;", new[] { "@CodLibro" }, new object[] { codLibro }).Rows.Count != 0;
            return new Message(exists, exists ? null : "El libro " + codLibro + " no existe");
        }

        public Message LibroDisponible(string codLibro)
        {
            var exists = Select("SELECT * from Libro WHERE cod_libro = @CodLibro and nro_copias > 0;", new[] { "@CodLibro" }, new object[] { codLibro }).Rows.Count != 0;
            return new Message(exists, exists ? null : "El libro "+codLibro+" no se encuentra disponible");
        }

        public int LibrosPrestados(string nroFicha)
        {
            var exists = Select("SELECT count(dp.cod_prestamo) " +
                               "FROM detalle_prestamo dp " +
                               "JOIN prestamo p " +
                               "ON dp.cod_prestamo = p.cod_prestamo " +
                               "JOIN usuario u ON p.nro_ficha = u.nro_ficha " +
                               "WHERE u.nro_ficha = @NroFicha AND libro_devuelto=0;"
                               , new [] { "@NroFicha" } , new object[] { nroFicha });
            return (int)exists.Rows[0].Field<long>(0);
        }

        private int CodigoLibro(int numFicha)
        {
            var codigo = Select("SELECT MAX(cod_prestamo) FROM prestamo WHERE nro_ficha = @NroFicha;", new [] { "@NroFicha" }, new object[] {numFicha});
            return codigo.Rows[0].Field<int>(0);
        }

        private DateTime SumarDias(DateTime fecha, int dias)
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