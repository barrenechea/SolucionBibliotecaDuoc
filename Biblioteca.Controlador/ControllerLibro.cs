using System.Collections.Generic;
using System.Data;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerLibro : ControllerDatabase
    {
        #region Attribute
        public Libro LibroPersistence { get; private set; }
        #endregion
        #region Preload method
        /// <summary>
        /// This method is to preload a Libro into the controller before requesting an Insert or Update query.
        /// </summary>
        /// <param name="codLibro">Codigo de libro</param>
        /// <param name="titulo">Titulo de libro</param>
        /// <param name="autor">Autor del libro</param>
        /// <param name="categoria">Categoría del libro</param>
        /// <param name="argumento">Argumento del libro</param>
        /// <param name="ubicacion">Ubicación del libro (estanterías)</param>
        /// <param name="editorial">Editorial del libro</param>
        /// <param name="codTipo">Código del Tipo de Libro</param>
        /// <param name="nroPaginas">Número de páginas que posee el libro</param>
        /// <param name="nroCopias">Numero de copias que la biblioteca posee del libro</param>
        /// <returns>Message, indicates status of the preload, and a message in case of failure</returns>
        public Message PreloadLibro(string codLibro, string titulo, string autor, string categoria, string argumento, string ubicacion, string editorial, int codTipo, int nroPaginas, int nroCopias)
        {
            Message msg;
            try
            {
                LibroPersistence = new Libro(codLibro, titulo, autor, categoria, argumento, ubicacion, editorial,
                    codTipo, nroPaginas, nroCopias);
                msg = new Message(true);
            }
            catch
            {
                msg = new Message(false, "Verifique los parámetros");
            }
            return msg;
        }
        #endregion
        #region Insert, Update querys
        /// <summary>
        /// Insert a Libro into the database.
        /// ¡Preload is required before executing this!
        /// </summary>
        /// <returns>Message that indicates if the Insert was successful or nor</returns>
        public Message Insert()
        {
            if(LibroPersistence == null) return new Message(false, "Debe precargar los datos de un libro");

            const string sqlSentence = "INSERT INTO Libro(cod_libro, titulo, autor, categoria, argumento, ubicacion, editorial, cod_tipo, nro_paginas, nro_copias) " +
                                       "VALUES (@CodLibro, @Titulo, @Autor, @Categoria, @Argumento, @Ubicacion, @Editorial, @CodTipo, @NroPaginas, @NroCopias);";
            
            var arrayParameters = new[]
            {
               "@CodLibro", "@Titulo", "@Autor", "@Categoria", "@Argumento", "@Ubicacion", "@Editorial", "@CodTipo", "@NroPaginas", "@NroCopias"
            };
            
            var arrayObjects = new object[]
            {
                LibroPersistence.CodLibro, LibroPersistence.Titulo, LibroPersistence.Autor, LibroPersistence.Categoria, LibroPersistence.Argumento, LibroPersistence.Ubicacion, LibroPersistence.Editorial, LibroPersistence.CodTipo, LibroPersistence.NroPaginas, LibroPersistence.NroCopias
            };
            
            var executeLib = Execute(sqlSentence, arrayParameters, arrayObjects);

            if (executeLib.Status)
            {
                executeLib.Mensaje = string.Format("Libro {0} agregado exitosamente", LibroPersistence.CodLibro);
                LibroPersistence = null;
                return executeLib;
            }

            if (executeLib.Mensaje.Equals("1")) executeLib.Mensaje = "Código del libro ya existe";
            LibroPersistence = null;
            return executeLib;
        }
        /// <summary>
        /// Update an existing Libro on the database.
        /// ¡Preload is required before executing this!
        /// </summary>
        /// <returns>Message that indicates if the Update was successful or nor</returns>
        public Message Update()
        {
            if (LibroPersistence == null) return new Message(false, "Debe precargar los datos de un libro");

            const string sqlSentence = "UPDATE Libro SET titulo=@Titulo, autor=@Autor, categoria=@Categoria, " +
                                       "argumento=@Argumento, ubicacion=@Ubicacion, editorial=@Editorial, " +
                                       "cod_tipo=@CodTipo, nro_paginas=@NroPaginas, nro_copias=@NroCopias " +
                                       "WHERE cod_libro = @CodLibro;";

            var arrayParameters = new[]
            {
                "@Titulo", "@Autor", "@Categoria", "@Argumento", "@Ubicacion", "@Editorial", "@CodTipo", "@NroPaginas", "@NroCopias", "@CodLibro"
            };

            var arrayObjects = new object[]
            {
                LibroPersistence.Titulo, LibroPersistence.Autor, LibroPersistence.Categoria, LibroPersistence.Argumento, LibroPersistence.Ubicacion, LibroPersistence.Editorial, LibroPersistence.CodTipo, LibroPersistence.NroPaginas, LibroPersistence.NroCopias, LibroPersistence.CodLibro
            };

            var executeUpdate = Execute(sqlSentence, arrayParameters, arrayObjects);
            if (executeUpdate.Status)
                executeUpdate.Mensaje = string.Format("Libro {0} modificado exitosamente", LibroPersistence.CodLibro);
            LibroPersistence = null;
            return executeUpdate;
        }
        #endregion
        #region Select querys
        /// <summary>
        /// Fetch all Libros existing on the database that has at least 1 copia available
        /// </summary>
        /// <returns>List of available Libros</returns>
        public List<Libro> FetchAllAvailable()
        {
            var libroTable = Select("SELECT cod_libro, titulo, autor, categoria, argumento, ubicacion, editorial, cod_tipo, nro_paginas, nro_copias FROM Libro WHERE nro_copias > 0;");

            var libroList = new List<Libro>();
            for (var i = 0; i < libroTable.Rows.Count; i++)
            {
                libroList.Add(new Libro(libroTable.Rows[i].Field<string>("cod_libro"),
                    libroTable.Rows[i].Field<string>("titulo"),
                    libroTable.Rows[i].Field<string>("autor"),
                    libroTable.Rows[i].Field<string>("categoria"),
                    libroTable.Rows[i].Field<string>("argumento"),
                    libroTable.Rows[i].Field<string>("ubicacion"),
                    libroTable.Rows[i].Field<string>("editorial"),
                    libroTable.Rows[i].Field<int>("cod_tipo"),
                    libroTable.Rows[i].Field<int>("nro_paginas"),
                    libroTable.Rows[i].Field<int>("nro_copias")));
            }
            return libroList;
        }
        /// <summary>
        /// Fetch all TipoLibro from the database
        /// </summary>
        /// <returns>List of TipoLibro</returns>
        public List<TipoLibro> FetchTipoLibros()
        {
            var tipoLibroTable = Select("SELECT cod_tipo, nom_tipo FROM Tipo_libro ORDER BY nom_tipo;");

            if (tipoLibroTable == null) return null;

            var listado = new List<TipoLibro>();
            for (var i = 0; i < tipoLibroTable.Rows.Count; i++)
            {
                listado.Add(new TipoLibro(tipoLibroTable.Rows[i].Field<int>("cod_tipo"), tipoLibroTable.Rows[i].Field<string>("nom_tipo")));
            }
            return listado;
        }
        /// <summary>
        /// Fetch an specific Libro from the database and loads it into Persistence
        /// </summary>
        /// <param name="codLibro">Codigo de Libro to search</param>
        /// <returns>Message that indicates if the fetch was successful or not</returns>
        public Message FetchLibro(string codLibro)
        {
            var libroSelected = Select(
                    "SELECT cod_libro, titulo, autor, categoria, argumento, ubicacion, editorial, cod_tipo, nro_paginas, nro_copias FROM Libro WHERE cod_libro = @CodLibro;",
                    new[] {"@CodLibro"}, new object[] {codLibro.ToUpper()});

            if (libroSelected == null || libroSelected.Rows.Count == 0)
                return new Message(false, "Libro no encontrado");

            LibroPersistence = new Libro(libroSelected.Rows[0].Field<string>("cod_libro"),
                libroSelected.Rows[0].Field<string>("titulo"),
                libroSelected.Rows[0].Field<string>("autor"),
                libroSelected.Rows[0].Field<string>("categoria"),
                libroSelected.Rows[0].Field<string>("argumento"),
                libroSelected.Rows[0].Field<string>("ubicacion"),
                libroSelected.Rows[0].Field<string>("editorial"),
                libroSelected.Rows[0].Field<int>("cod_tipo"),
                libroSelected.Rows[0].Field<int>("nro_paginas"),
                libroSelected.Rows[0].Field<int>("nro_copias"));

            return new Message(true);
        }
        #endregion
        #region Custom method
        /// <summary>
        /// Removes all persistant data inside this Controller
        /// </summary>
        public void ClearPersistantData()
        {
            LibroPersistence = null;
        }
        #endregion
    }
}