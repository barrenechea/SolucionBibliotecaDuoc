using System.Data;
using Biblioteca.Controlador.Utils;
using Biblioteca.Entidad;
using Biblioteca.Entidad.Enum;

namespace Biblioteca.Controlador
{
    public class ControllerAdmins : ControllerDatabase
    {
        #region Attributes
        public Administrador AdminActive { get; private set; }
        public Administrador AdminPersistence { get; private set; }
        #endregion
        #region Login Methods
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
        #region Preload Method
        public Message PreloadAdmin(string nombre, string apellido, string usuario, string password, bool status)
        {
            Message msg;
            try
            {
                if (AdminPersistence != null)
                {
                    AdminPersistence.Nombre = nombre;
                    AdminPersistence.Apellido = apellido;
                    if(!string.IsNullOrWhiteSpace(password))
                        AdminPersistence.Contrasena = ControllerHash.Hash(password);
                    AdminPersistence.Estado = status;
                }
                else
                {
                    var tipoCuenta = AdminActive.TipoDeUsuario == TipoUsuario.Director ? 'J' : 'B';
                    AdminPersistence = new Administrador(nombre, apellido, usuario, ControllerHash.Hash(password), true, tipoCuenta);
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
        #region Insert, Update querys
        public Message Insert()
        {
            if (AdminPersistence == null) return new Message(false, "Debe precargar un Administrador");

            const string sqlSentence = "INSERT INTO Administrador (nombre,apellido,id_usuario,contrasena,estado,tipo_usuario) " +
                                       "VALUES (@Nombre, @Apellido, @IdUsuario, @HashedPass, @Estado, @TipoUsuario);";

            var arrayParameters = new[] { "@Nombre", "@Apellido", "@IdUsuario", "@HashedPass", "@Estado", "@TipoUsuario" };

            var arrayObjects = new object[] { AdminPersistence.Nombre, AdminPersistence.Apellido, AdminPersistence.IdUsuario, AdminPersistence.Contrasena, AdminPersistence.Estado, AdminPersistence.TipoDeUsuarioChar };

            var executeAdmin = Execute(sqlSentence, arrayParameters, arrayObjects);

            if (executeAdmin.Status)
            {
                executeAdmin.Mensaje = string.Format("{0} agregado exitosamente", AdminPersistence.TipoDeUsuarioChar == 'J' ? "Jefe Biblioteca" : "Bibliotecario");
                AdminPersistence = null;
                return executeAdmin;
            }

            if (executeAdmin.Mensaje.Equals("1")) executeAdmin.Mensaje = "Nombre de usuario ya existe";
            AdminPersistence = null;
            return executeAdmin;
        }
        public Message Update()
        {
            if (AdminPersistence == null) return new Message(false, "Debe precargar un Administrador");

            const string sqlSentence = "UPDATE Administrador SET nombre = @Nombre, apellido = @Apellido, " +
                                       "contrasena = @HashedPass, estado = @Estado, " +
                                       "tipo_usuario = @TipoUsuario WHERE id_usuario = @IdUsuario;";

            var arrayParameters = new[] { "@Nombre", "@Apellido", "@HashedPass", "@Estado", "@TipoUsuario", "@IdUsuario" };

            var arrayObjects = new object[]
            {
                AdminPersistence.Nombre,
                AdminPersistence.Apellido,
                AdminPersistence.Contrasena,
                AdminPersistence.Estado,
                AdminPersistence.TipoDeUsuarioChar,
                AdminPersistence.IdUsuario
            };

            var executeUpdate = Execute(sqlSentence, arrayParameters, arrayObjects);
            if (executeUpdate.Status)
            {
                executeUpdate.Mensaje = string.Format("{0} modificado exitosamente", AdminPersistence.TipoDeUsuarioChar == 'J' ? "Jefe Biblioteca" : "Bibliotecario");
                AdminPersistence = null;
            }
            return executeUpdate;
        }
        #endregion
        #region Select Query
        public Message FetchUsuario(string usuario)
        {
            if (string.Equals(usuario.ToLower(), AdminActive.IdUsuario.ToLower())) return new Message(false, "No puede modificarse a si mismo");
            var user = Select("select nombre, apellido, id_usuario, contrasena, estado, tipo_usuario from Administrador where id_usuario=@User;", new[] { "@User" }, new object[] { usuario });

            if (user == null || user.Rows.Count == 0) return new Message(false, "Cuenta no encontrada");

            var admin = new Administrador(user.Rows[0].Field<string>(0), user.Rows[0].Field<string>(1), user.Rows[0].Field<string>(2), user.Rows[0].Field<string>(3), user.Rows[0].Field<bool>(4), char.Parse(user.Rows[0].Field<string>(5)));
            
            if (admin.TipoDeUsuarioChar == 'B' && AdminActive.TipoDeUsuarioChar != 'J')
                return new Message(false, "No tiene permisos para modificar esta cuenta");

            if (admin.TipoDeUsuarioChar == 'J' && AdminActive.TipoDeUsuarioChar != 'D')
                return new Message(false, "No tiene permisos para modificar esta cuenta");

            if (admin.TipoDeUsuarioChar == 'D')
                return new Message(false, "No tiene permisos para modificar esta cuenta");

            AdminPersistence = admin;

            return new Message(true);
        }
        #endregion
        #region Existance Check Query
        public bool ExistsUsuario(string usuario)
        {
            return Select("select nombre, apellido, id_usuario, contrasena, estado, tipo_usuario from Administrador where id_usuario=@User;", new[] { "@User" }, new object[] { usuario }).Rows.Count != 0;
        }
        #endregion
    }
}