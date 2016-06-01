using System.Data;
using Biblioteca.Controlador.Utils;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerLogin : ControllerDatabase
    {
        #region Attributes
        public Administrador AdminActive { get; private set; }
        #endregion
        #region Constructor
        public ControllerLogin()
        {
            AdminActive = null;
        }
        #endregion
        #region Methods
        public Message Login(string user, string pass)
        {
            var adminTable = Select("select nombre, apellido, id_usuario, contrasena, estado, tipo_usuario from Administrador where id_usuario=@User;", new[]{"@User"}, new object[]{user});

            if (adminTable == null) return null;

            if(adminTable.Rows.Count == 0) return new Message(false, "No se ha encontrado el usuario");

            if (!ControllerHash.CheckHash(pass, adminTable.Rows[0].Field<string>(3)))
                return new Message(false, "Contraseña incorrecta");

            if (!adminTable.Rows[0].Field<bool>(4))
                return new Message(false, "La cuenta está deshabilitada");

            if (adminTable.Rows[0].Field<string>(5) != "B" && adminTable.Rows[0].Field<string>(5) != "J" && adminTable.Rows[0].Field<string>(5) != "D")
                return new Message(false, "La cuenta no está correctamente configurada");

            AdminActive = new Administrador(adminTable.Rows[0].Field<string>(0), adminTable.Rows[0].Field<string>(1), adminTable.Rows[0].Field<string>(2), adminTable.Rows[0].Field<string>(3), adminTable.Rows[0].Field<bool>(4), char.Parse(adminTable.Rows[0].Field<string>(5)));

            return new Message(true);
        }
        public void Logout()
        {
            AdminActive = null;
        }
        #endregion
    }
}