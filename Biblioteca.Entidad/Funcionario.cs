using System;

namespace Biblioteca.Entidad
{
    public class Funcionario : Usuario
    {
        #region Attribute
        private string _cargo;
        #endregion
        #region Get and Set
        public string Cargo
        {
            get { return _cargo; }
            set { _cargo = value; }
        }
        #endregion
        #region Constructor
        public Funcionario(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, DateTime fecNacimiento, int nroFicha, bool estado, string cargo)
            : base(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel, fecNacimiento, nroFicha, estado)
        {
            Cargo = cargo;
        }
        #endregion
    }
}
