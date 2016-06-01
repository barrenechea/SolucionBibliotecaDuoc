using System;

namespace Biblioteca.Entidad
{
    public class Estudiante : Usuario
    {
        #region Attribute
        private string _curso;
        #endregion
        #region Get and Set
        public string Curso
        {
            get { return _curso; }
            set { _curso = value; }
        }
        #endregion
        #region Constructor
        public Estudiante(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, DateTime fecNacimiento, int nroFicha, bool estado, string curso)
            : base(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel, fecNacimiento, nroFicha, estado)
        {
            Curso = curso;
        }
        #endregion
    }
}