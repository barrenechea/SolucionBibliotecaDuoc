using System;
using System.Collections.Generic;
using System.Data;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerPrestamo : ControllerLog
    {
        #region Attributes
        private Prestamo PrestamoPersistence { get; set; }
        public List<DetallePrestamo> DetPrestamoPersistenceList { get; private set; }
        public List<Libro> InfoLibrosPersistence { get; private set; }
        #endregion
        #region Preload methods
        /// <summary>
        /// This method is to preload a Prestamo into the controller before requesting an Insert query.
        /// </summary>
        /// <param name="nroFicha">Número de ficha del solicitante</param>
        /// <returns>Message, indicates status of the preload, and a message in case of failure</returns>
        public Message PreloadPrestamo(int nroFicha)
        {
            Message msg;
            try
            {
                PrestamoPersistence = new Prestamo(DateTime.Now, nroFicha);
                msg = new Message(true);
            }
            catch
            {
                msg = new Message(false, "Verifique los parámetros");
            }
            return msg;
        }
        /// <summary>
        /// This method is to preload a List of Detalle prestamo into the controller before requesting an Insert query.
        /// </summary>
        /// <param name="nroFicha">Número de ficha del solicitante</param>
        /// <param name="codLibros">Códigos de los libros que solicita</param>
        /// <returns></returns>
        public Message PreloadDetallePrestamo(int nroFicha, string[] codLibros)
        {
            Message msg;
            try
            {
                DetPrestamoPersistenceList = new List<DetallePrestamo>();
                var codPrestamo = CodigoPrestamo(nroFicha);
                foreach (var codLibro in codLibros)
                {
                    DetPrestamoPersistenceList.Add(new DetallePrestamo(SumarDias(DateTime.Now, TipoLibro(codLibro.Trim()) == 4 ? 5 : 3), false, 0, codPrestamo, codLibro.Trim()));
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
        #region SQL Methods
        #region Insert Querys
        public Message InsertPrestamo()
        {
            if (PrestamoPersistence == null) return new Message(false, "Debe precargar los datos del préstamo");

            var sqlSentence = "INSERT INTO Prestamo (fec_prestamo, nro_ficha) " +
                                 "VALUES (@FecPrestamo, @NroFicha);";
            var arrayParameters = new[] { "@FecPrestamo", "@NroFicha" };
            var arrayObjects = new object[] { DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), PrestamoPersistence.NroFicha };
            
            ClearPersistantData();

            return Execute(sqlSentence, arrayParameters, arrayObjects);
        }
        public Message InsertDetallePrestamo()
        {
            if (DetPrestamoPersistenceList.Count == 0) return new Message(false, "Debe precargar un detalle de préstamo");

            var sqlSentence = "INSERT INTO Detalle_prestamo(fec_devolucion, libro_devuelto, renovacion, cod_prestamo, cod_libro) " +
                          "VALUES (@FecDevolucion, @LibroDevuelto, @Renovacion, @CodPrestamo, @CodLibro);";
            var arrayParameters = new[] { "@FecDevolucion", "@LibroDevuelto", "@Renovacion", "@CodPrestamo", "@CodLibro" };
            foreach (var dp in DetPrestamoPersistenceList)
            {
                var arrayObjects = new object[] { dp.FecDevolucion, dp.LibroDevuelto, dp.Renovacion, dp.CodPrestamo, dp.CodLibro.Trim() };
                var exec = Execute(sqlSentence, arrayParameters, arrayObjects);
                if (!exec.Status) return exec;
            }

            Log(string.Format("Préstamo generado. ID préstamo: {0}", DetPrestamoPersistenceList[0].CodPrestamo));

            ClearPersistantData();
            return new Message(true, "Prestamo ingresado exitosamente");
        }
        #endregion
        #region Update Querys
        public Message ExtenderPrestamo(int codPrestamo)
        {
            var newFecha = SumarDias(DetPrestamoPersistenceList[0].FecDevolucion, TipoLibro(DetPrestamoPersistenceList[0].CodLibro) == 4 ? 5 : 3);

            var sqlSentence = "UPDATE Detalle_prestamo SET fec_devolucion= @NewFecha, renovacion = renovacion+1 WHERE cod_prestamo = @CodPrestamo;";
            var arrayParameters = new[] { "@NewFecha", "@CodPrestamo" };
            var arrayObjects = new object[] { newFecha, codPrestamo };

            Execute(sqlSentence, arrayParameters, arrayObjects);
            Log(string.Format("Préstamo extendido. ID préstamo: {0}", codPrestamo));
            return new Message(true, string.Format("Nueva fecha de devolución: {0}", newFecha.ToString("dd-MM-yyyy")));
        }
        public Message DevolverLibro(int codPrestamo, string codLibro, bool isStudent)
        {
            var sqlSentence = "UPDATE Detalle_prestamo SET libro_devuelto = 1 WHERE cod_prestamo = @CodPrestamo AND cod_libro = @CodLibro AND libro_devuelto <> 1 LIMIT 1;";
            var arrayParameters = new[] { "@CodPrestamo", "@CodLibro" };
            var arrayObjects = new object[] { codPrestamo, codLibro };
            Execute(sqlSentence, arrayParameters, arrayObjects);

            Log(string.Format("Préstamo devuelto. ID préstamo: {0} [Cod. Libro: {1}]", codPrestamo, codLibro));

            if (!isStudent) return new Message(true, "Devolución realizada con éxito.");
            return (DetPrestamoPersistenceList[0].FecDevolucion < DateTime.Now) ? new Message(false, "El usuario ha quedado registrado en la hoja de morosidad. Revise en el Panel de administración para más información") : new Message(true, "Devolución realizada con éxito.");
        }
        #endregion
        #region Select Querys
        #region Libro
        /// <summary>
        /// Indica el numero del tipo de un libro
        /// </summary>
        /// <param name="codLibro">Código del libro a buscar</param>
        /// <returns>int con el numero del tipo de libro</returns>
        private int TipoLibro(string codLibro)
        {
            var query = Select("SELECT cod_tipo FROM Libro WHERE cod_libro = @CodLibro", new[] { "@CodLibro" },
                new object[] { codLibro });
            return query.Rows[0].Field<int>(0);
        }
        #endregion
        #region Prestamo
        public Message FetchPrestamo(int nroFicha)
        {
            DetPrestamoPersistenceList = new List<DetallePrestamo>();
            InfoLibrosPersistence = new List<Libro>();
            var table = Select("SELECT dp.cod_libro, l.titulo, l.autor, dp.fec_devolucion, dp.cod_libro, dp.renovacion " +
                                    "FROM Prestamo p " +
                                    "JOIN Detalle_prestamo dp " +
                                    "ON p.cod_prestamo = dp.cod_prestamo " +
                                    "JOIN Libro l ON l.cod_libro = dp.cod_libro " +
                                    "WHERE p.nro_ficha = @NroFicha AND dp.libro_devuelto = 0;",
                                    new[] { "@NroFicha" }, new object[] { nroFicha });
            if (table.Rows.Count == 0) return new Message(false, "No tiene préstamos pendientes");
            for (var i = 0; i < table.Rows.Count; i++)
            {
                InfoLibrosPersistence.Add(new Libro(table.Rows[i].Field<string>("cod_libro"),
                    table.Rows[i].Field<string>("titulo"), table.Rows[i].Field<string>("autor")));
                DetPrestamoPersistenceList.Add(new DetallePrestamo(table.Rows[i].Field<DateTime>("fec_devolucion"),
                    table.Rows[i].Field<string>("cod_libro"), table.Rows[i].Field<int>("renovacion")));
            }
            return new Message(true);
        }
        /// <summary>
        /// Indica el codigo del ultimo préstamo que un usuario realizó
        /// </summary>
        /// <param name="numFicha">número de ficha que se buscará</param>
        /// <returns>int con el codigo del préstamo</returns>
        public int CodigoPrestamo(int numFicha)
        {
            var codigo = Select("SELECT MAX(cod_prestamo) FROM prestamo WHERE nro_ficha = @NroFicha;", new[] { "@NroFicha" }, new object[] { numFicha });
            return codigo.Rows[0].Field<int>(0);
        }

        public int CodigoPrestamo(int nroFicha, string codLibro)
        {
            var sqlSentence = "SELECT MAX(dp.cod_prestamo) FROM prestamo p JOIN detalle_prestamo dp ON p.cod_prestamo = dp.cod_prestamo WHERE p.nro_ficha = @NroFicha AND dp.cod_libro = @CodLibro AND dp.libro_devuelto <> 1;";
            var arrayParameters = new[] { "@NroFicha", "@CodLibro" };
            var arrayObjects = new object[] { nroFicha, codLibro };
            var codigo = Select(sqlSentence, arrayParameters, arrayObjects);

            return codigo.Rows[0].Field<int>(0);
        }
        #endregion
        #endregion
        #endregion
        #region Custom methods
        /// <summary>
        /// Suma dias hábiles
        /// </summary>
        /// <param name="fecha">fecha a la que se le van a sumar los dias</param>
        /// <param name="dias">cantidad de días hábiles que se le sumarán</param>
        /// <returns></returns>
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
        /// <summary>
        /// Cuenta los días de retraso en la entrega de un libro.
        /// ¡El libro debe estar dentro de la persistencia!
        /// </summary>
        /// <returns>Integer con los días de atraso</returns>
        public int DiasAtraso()
        {
            if (DetPrestamoPersistenceList.Count == 0) return 0;

            var fecha = DetPrestamoPersistenceList[0].FecDevolucion;
            var contador = 0;
            var auxFecha = int.Parse((fecha - DateTime.Now).ToString("%d"));

            for (var i = 0; i < auxFecha; i++)
            {
                fecha = fecha.AddDays(-1);
                if (fecha.DayOfWeek != DayOfWeek.Sunday && fecha.DayOfWeek != DayOfWeek.Saturday)
                    contador++;
            }
            return contador;
        }
        /// <summary>
        /// Clear all persistant data inside this controller
        /// </summary>
        public void ClearPersistantData()
        {
            PrestamoPersistence = null;
            DetPrestamoPersistenceList = null;
            InfoLibrosPersistence = null;
        }
        #endregion
    }
}