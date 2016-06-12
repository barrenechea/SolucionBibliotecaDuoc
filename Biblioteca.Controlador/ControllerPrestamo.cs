using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerPrestamo : ControllerDatabase
    {
        private Prestamo PrestamoPersistence { get; set; }
        private List<DetallePrestamo> DetPrestamoPersistenceList { get; set; }
        public DetallePrestamo DetPrestamoPersistence { get; set; }

        #region Preload method

        /// <summary>
        /// This method is to preload a Prestamo into the controller before requesting an Insert or Update query.
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
        /// This method is to preload a Detalle prestamo into the controller before requesting an Insert or Update query.
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
                    if (TipoLibro(codLibro.Trim()) == 4)
                        DetPrestamoPersistenceList.Add(new DetallePrestamo(SumarDias(DateTime.Now, 5), false, 0, codPrestamo, codLibro.Trim()));
                    else
                        DetPrestamoPersistenceList.Add(new DetallePrestamo(SumarDias(DateTime.Now, 3), false, 0, codPrestamo, codLibro.Trim()));
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
        public Message InsertPrestamo()
        {
            if (PrestamoPersistence == null) return new Message(false, "Debe precargar los datos del préstamo");

            var sqlSentence = "INSERT INTO Prestamo (fec_prestamo, nro_ficha) " +
                                 "VALUES (@FecPrestamo, @NroFicha);";
            var arrayParameters = new[] { "@FecPrestamo", "@NroFicha" };
            var arrayObjects = new object[] { DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), PrestamoPersistence.NroFicha };
            
            return Execute(sqlSentence, arrayParameters, arrayObjects);
        }

        public Message InsertDetallePrestamo()
        {
            var sqlSentence = "INSERT INTO Detalle_prestamo(fec_devolucion, libro_devuelto, renovacion, cod_prestamo, cod_libro) " +
                          "VALUES (@FecDevolucion, @LibroDevuelto, @Renovacion, @CodPrestamo, @CodLibro);";
            var arrayParameters = new[] { "@FecDevolucion", "@LibroDevuelto", "@Renovacion", "@CodPrestamo", "@CodLibro" };
            foreach (var dp in DetPrestamoPersistenceList)
            {
                var arrayObjects = new object[] { dp.FecDevolucion, dp.LibroDevuelto, dp.Renovacion, dp.CodPrestamo, dp.CodLibro.Trim() };
                var exec = Execute(sqlSentence, arrayParameters, arrayObjects);
                if (!exec.Status) return exec;
            }
            return new Message(true, "Prestamo ingresado exitosamente");
        }
        #endregion
        #region Utils
        #region Usuario

        /// <summary>
        /// Revisa si un usuario puede pedir libros de a cuerdo a su estado en la base de datos
        /// </summary>
        /// <param name="nroFicha">Numero de ficha del usuario a consultar</param>
        /// <returns>Message con el estado (True o False). En caso de ser false, se retorna mensaje correspondiente</returns>
        public Message UsuarioActivado(string nroFicha)
        {
            var estado = Select("SELECT estado from Usuario WHERE nro_ficha = @Nro_ficha;", new[] { "@Nro_ficha" }, new object[] { nroFicha });
            return new Message(estado.Rows[0].Field<bool>(0), estado.Rows[0].Field<bool>(0) ? null : "Usuario tiene sanción activa.");
        }

        /// <summary>
        /// Revisa si el número de ficha existe en la base de datos
        /// </summary>
        /// <param name="nroFicha"></param>
        /// <returns>Message con el resultado (True o False). En caso de ser false, se retorna mensaje correspondiente</returns>
        public Message ExistsNroFicha(string nroFicha)
        {
            var exists = Select("SELECT * from Usuario WHERE nro_ficha = @NroFicha;", new[] { "@NroFicha" }, new object[] { nroFicha }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No se ha encontrado el número de ficha");
        }
        #endregion
        #region Libro


        /// <summary>
        /// Revisa si el libro que se está solicitando existe en la base de datos
        /// </summary>
        /// <param name="codLibro">Codigo del libro a revisar</param>
        /// <returns>Message con el estado de existencia del libro (True o False). En caso de ser false, se retorna mensaje correspondiente</returns>
        public Message ExistsLibro(string codLibro)
        {
            var exists = Select("SELECT * from Libro WHERE cod_libro = @CodLibro;", new[] { "@CodLibro" }, new object[] { codLibro }).Rows.Count != 0;
            return new Message(exists, exists ? null : "El libro " + codLibro.Trim() + " no existe");
        }

        /// <summary>
        /// Indica la cantidad de libros que hay en el sistema
        /// </summary>
        /// <param name="codLibro">Libro a revisar</param>
        /// <returns>int con la cantidad de libros</returns>
        public int LibroDisponible(string codLibro)
        {
            var exists = Select("SELECT count(*) from Libro WHERE cod_libro = @CodLibro and nro_copias > 0;", new[] { "@CodLibro" }, new object[] { codLibro });
            return (int)exists.Rows[0].Field<long>(0);
        }

        /// <summary>
        /// Indica la cantidad de libros que tienen prestados los usuarios
        /// </summary>
        /// <param name="nroFicha">Numero e ficha del usuario al que se le revisará</param>
        /// <returns>int con la cantidad de libros</returns>
        public int CantLibrosPrestados(string nroFicha)
        {
            var exists = Select("SELECT count(dp.cod_prestamo) " +
                               "FROM detalle_prestamo dp " +
                               "JOIN prestamo p " +
                               "ON dp.cod_prestamo = p.cod_prestamo " +
                               "JOIN usuario u ON p.nro_ficha = u.nro_ficha " +
                               "WHERE u.nro_ficha = @NroFicha AND libro_devuelto=0;"
                               , new[] { "@NroFicha" }, new object[] { nroFicha });
            return (int)exists.Rows[0].Field<long>(0);
        }

        /// <summary>
        /// Descuenta un libro
        /// </summary>
        /// <param name="codLibro">Código del libro que se descontará</param>
        public void DescuentaLibro(string codLibro)
        {
            Execute("UPDATE libro SET nro_copias=nro_copias-1 WHERE cod_libro = @CodLibro;", new[] { "CodLibro" }, new object[] { codLibro });
        }

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

        public int DiasAtraso(DateTime fecha)
        {
            var contador = 0;
            var auxFecha = int.Parse((fecha - DateTime.Now).ToString("%d"));

            for (var i = 0; i < auxFecha; i++)
            {
                fecha = fecha.AddDays(-1);
                if (fecha.DayOfWeek != DayOfWeek.Sunday && fecha.DayOfWeek != DayOfWeek.Saturday)
                    contador ++;
            }
            return contador;
        }

        public Message FetchPrestamo(string nroFicha)
        {
            var table = Select("SELECT dp.fec_devolucion, dp.cod_libro, dp.renovacion " +
                                       "FROM Prestamo p " +
                                       "JOIN Detalle_prestamo dp ON p.cod_prestamo = dp.cod_prestamo " +
                                       "WHERE p.nro_ficha = @NroFicha AND dp.libro_devuelto = 0;", new[] { "@NroFicha" }, new object[] { nroFicha });
            if(table.Rows.Count==0) return new Message(false, "No tiene préstamos pendientes");
            DetPrestamoPersistence = new DetallePrestamo(table.Rows[0].Field<DateTime>("fec_devolucion"), table.Rows[0].Field<string>("cod_libro"),table.Rows[0].Field<int>("renovacion"));
            return CantLibrosPrestados(nroFicha) == 0 ? new Message(false, "El estudiante no tiene préstamo pendiente") : new Message(true);
        }

        public List<Libro> InfoLibrosPrestados(int nroFicha)
        {
            var libroTable = Select("SELECT dp.cod_libro, l.titulo, l.autor " +
                                    "FROM Prestamo p " +
                                    "JOIN Detalle_prestamo dp " +
                                    "ON p.cod_prestamo = dp.cod_prestamo " +
                                    "JOIN Libro l ON l.cod_libro = dp.cod_libro " +
                                    "WHERE p.nro_ficha = @NroFicha AND dp.libro_devuelto = 0;",
                                    new[] { "@NroFicha" }, new object[] { nroFicha });
            var libroList = new List<Libro>();
            for (var i = 0; i < libroTable.Rows.Count; i++)
            {
                libroList.Add(new Libro(libroTable.Rows[i].Field<string>("cod_libro"),
                    libroTable.Rows[i].Field<string>("titulo"),libroTable.Rows[i].Field<string>("autor")));
            }
            return libroList;
        }

        public void ClearPersistantData()
        {
            DetPrestamoPersistence = null;
            DetPrestamoPersistenceList = null;
            PrestamoPersistence = null;
        }

        public void ExtenderPrestamo(string codPrestamo, string codLibro, DateTime fecDevolucion)
        {
            var newFecha = SumarDias(fecDevolucion, TipoLibro(codLibro) == 4 ? 5 : 3);
            var sqlSentence = "UPDATE Detalle_prestamo SET fec_devolucion= @NewFecha, renovacion = renovacion+1 WHERE cod_prestamo = @CodPrestamo;"; 
            var arrayParameters = new[] { "@NewFecha", "@CodPrestamo" };
            var arrayObjects = new object[] { newFecha, codPrestamo };
            Execute(sqlSentence, arrayParameters, arrayObjects);
        }

        public Message DevolverLibro(string codPrestamo, string codLibro, DateTime fecDevolucion)
        {
            var sqlSentence = "UPDATE Detalle_prestamo SET libro_devuelto = 1 WHERE cod_prestamo = @CodPrestamo and cod_libro = @CodLibro;";
            var arrayParameters = new[] { "@CodPrestamo", "@CodLibro" };
            var arrayObjects = new object[] { codPrestamo, codLibro };
            Execute(sqlSentence, arrayParameters, arrayObjects);

            if (fecDevolucion < DateTime.Now)
            {
                return new Message(false, "El usuario ha quedado registrado en la hoja de morosidad. Revise en el Panel de administración para más información");
            }
            return new Message(true);

        }

        public Message HojaDeMorosidad(int nroFicha, int diasAtrasados)
        {
            //if()
            return new Message(true);
        }
        #endregion
        #endregion
    }
}