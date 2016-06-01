using System;

namespace Biblioteca.Entidad
{
    public class Libro
    {
        #region Attributes
        private string _codLibro;
        private string _titulo;
        private string _autor;
        private string _categoria;
        private string _argumento;
        private string _ubicacion;
        private string _editorial;
        private int _codTipo;
        private int _nroPaginas;
        private int _nroCopias;
        #endregion
        #region Gets and sets
        public string CodLibro
        {
            get { return _codLibro; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un cod de libro");
                _codLibro = value;
            }
        }

        public string Titulo
        {
            get { return _titulo; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un titulo");
                _titulo = value;
            }
        }

        public string Autor
        {
            get { return _autor; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un autor");
                _autor = value;
            }
        }

        public string Categoria
        {
            get { return _categoria; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar una categoria");
                _categoria = value;
            }
        }

        public string Argumento
        {
            get { return _argumento; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un argumento");
                _argumento = value;
            }
        }

        public string Ubicacion
        {
            get { return _ubicacion; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar una ubicacion");
                _ubicacion = value;
            }
        }

        public string Editorial
        {
            get { return _editorial; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar una editorial");
                _editorial = value;
            }
        }

        public int CodTipo
        {
            get { return _codTipo; }
            set { _codTipo = value; }
        }

        public int NroPaginas
        {
            get { return _nroPaginas; }
            set
            {
                if (value == 0)
                    throw new ArgumentException("Debe ingresar la cantidad de paginas");
                _nroPaginas = value;
            }
        }

        public int NroCopias
        {
            get { return _nroCopias; }
            set
            {
                if (value == 0)
                    throw new ArgumentException("Debe ingresar la cantidad de copias");
                _nroCopias = value;
            }
        }
        #endregion
        #region Constructor
        public Libro(string codLibro, string titulo, string autor, string categoria, string argumento, string ubicacion, string editorial, int codTipo, int nroPaginas, int nroCopias)
        {
            CodLibro = codLibro;
            Titulo = titulo;
            Autor = autor;
            Categoria = categoria;
            Argumento = argumento;
            Ubicacion = ubicacion;
            Editorial = editorial;
            CodTipo = codTipo;
            NroPaginas = nroPaginas;
            NroCopias = nroCopias;
        }
        #endregion
    }
}
