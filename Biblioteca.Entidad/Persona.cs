using System;

namespace Biblioteca.Entidad
{
    public class Persona
    {
        #region Attributes
        private string _run;
        private string _nombre;
        private string _apellido;
        private string _direccion;
        private int _codComuna;
        private string _fonoFijo;
        private string _fonoCel;
        private string _fecCreacion;
        
        #endregion
        #region Gets and Sets
        public string Run
        {
            get { return _run; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un run");
                _run = value.ToUpper();
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un nombre");
                _nombre = value;
            }
        }

        public string Apellido
        {
            get { return _apellido; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un apellido");
                _apellido = value;
            }
        }

        public string Direccion
        {
            get { return _direccion; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar una direccion");
                _direccion = value;
            }
        }

        public int CodComuna
        {
            get { return _codComuna; }
            set { _codComuna = value; }
        }

        public string FecCreacion
        {
            get { return _fecCreacion; }
            set { _fecCreacion = value; }
        }

        public string FonoFijo
        {
            get { return _fonoFijo; }
            set { _fonoFijo = value; }
        }

        public string FonoCel
        {
            get { return _fonoCel; }
            set { _fonoCel = value; }
        }

        #endregion
        #region Constructor
        protected Persona(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel)
        {
            Run = run.ToUpper();
            Nombre = nombre;
            Apellido = apellido;
            Direccion = direccion;
            CodComuna = codComuna;
            FecCreacion = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            FonoFijo = fonoFijo;
            FonoCel = fonoCel;
        }
        #endregion
    }
}
