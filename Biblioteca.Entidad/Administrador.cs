using System;
using Biblioteca.Entidad.Enum;

namespace Biblioteca.Entidad
{
    public class Administrador
    {
        #region Attributes
        private string _nombre;
        private string _apellido;
        private string _idUsuario;
        private string _contrasena;
        private bool _estado;
        private TipoUsuario _tipoDeUsuario;
        #endregion
        #region Gets and Sets
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

        public string IdUsuario
        {
            get { return _idUsuario; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar un ID de usuario");
                _idUsuario = value;
            }
        }

        public string Contrasena
        {
            get { return _contrasena; }
            set
            {
                if (value == string.Empty)
                    throw new ArgumentException("Debe ingresar una contraseña");
                _contrasena = value;
            }
        }

        public bool Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }
        public TipoUsuario TipoDeUsuario
        {
            get { return _tipoDeUsuario; }
            set { _tipoDeUsuario = value; }
        }
        public char TipoDeUsuarioChar
        {
            get
            {
                switch (TipoDeUsuario)
                {
                    case TipoUsuario.Director:
                        return 'D';
                    case TipoUsuario.Jefebiblioteca:
                        return 'J';
                    case TipoUsuario.Bibliotecario:
                        return 'B';
                    default:
                        return ' ';
                }
            }
        }
        #endregion
        #region Constructor
        public Administrador(string nombre, string apellido, string idUsuario, string contrasena, bool estado,
            char tipoUsuario)
        {
            Nombre = nombre;
            Apellido = apellido;
            IdUsuario = idUsuario;
            Contrasena = contrasena;
            Estado = estado;
            switch (tipoUsuario)
            {
                case 'B':
                    TipoDeUsuario = TipoUsuario.Bibliotecario;
                    break;
                case 'J':
                    TipoDeUsuario = TipoUsuario.Jefebiblioteca;
                    break;
                case 'D':
                    TipoDeUsuario = TipoUsuario.Director;
                    break;
                default:
                    TipoDeUsuario = TipoUsuario.Sininfo;
                    break;
            }
        }
        #endregion
    }
}
