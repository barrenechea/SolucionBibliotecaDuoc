using System;
using System.Collections.Generic;
using System.Data;
using Biblioteca.Entidad;
// ReSharper disable JoinDeclarationAndInitializer

namespace Biblioteca.Controlador
{
    public class ControllerUsers : ControllerDatabase
    {
        #region Attributes
        public Persona PersonaPersistence { get; private set; }
        public string RunApoderado { get; private set; }
        public string Parentesco { get; private set; }
        #endregion
        #region Preload methods
        public Message PreloadPersona(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, DateTime fecNacimiento, bool estado, string curso, string runApoderado, string parentesco, int nroFicha = 0)
        {
            Message msg;
            try
            {
                PersonaPersistence = new Estudiante(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel,
                    fecNacimiento, nroFicha, estado, curso);
                RunApoderado = runApoderado;
                Parentesco = parentesco;
                msg = new Message(true);
            }
            catch
            {
                msg = new Message(false, "Verifique los parámetros");
            }
            return msg;
        }
        public Message PreloadPersona(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, DateTime fecNacimiento, bool estado, string cargo, int nroFicha = 0)
        {
            Message msg;
            try
            {
                PersonaPersistence = new Funcionario(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel,
                    fecNacimiento, nroFicha, estado, cargo);
                msg = new Message(true);
            }
            catch
            {
                msg = new Message(false, "Verifique los parámetros");
            }
            return msg;
        }
        public Message PreloadPersona(string run, string nombre, string apellido, string direccion, int codComuna, string fonoFijo, string fonoCel, int numFicha, string parentesco)
        {
            Message msg;
            try
            {
                PersonaPersistence = new Apoderado(run, nombre, apellido, direccion, codComuna, fonoFijo, fonoCel, numFicha, parentesco);
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
            if (PersonaPersistence == null) return new Message(false, "Debe precargar los datos de una persona");

            string sqlSentence;
            string[] arrayParameters;
            object[] arrayObjects;

            #region Insert into Persona

            sqlSentence = "INSERT INTO Persona (rut, nombre, apellido, direccion, cod_comuna, fec_creacion) " +
                          "VALUES (@Run, @Nombre, @Apellido, @Direccion, @CodComuna, @FechaCreacion);";
            arrayParameters = new[] { "@Run", "@Nombre", "@Apellido", "@Direccion", "@CodComuna", "@FechaCreacion" };
            arrayObjects = new object[] { PersonaPersistence.Run, PersonaPersistence.Nombre, PersonaPersistence.Apellido, PersonaPersistence.Direccion, PersonaPersistence.CodComuna, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") };

            var executePer = Execute(sqlSentence, arrayParameters, arrayObjects);
            if (!executePer.Status)
            {
                if (executePer.Mensaje.Equals("1")) executePer.Mensaje = "RUN ya existe";
                return executePer;
            }
            #endregion
            #region Insert into Telefono

            sqlSentence = "INSERT INTO Telefono (rut, telefono) VALUES (@Run, @Telefono);";
            arrayParameters = new[] { "@Run", "@Telefono" };
            arrayObjects = new object[] { PersonaPersistence.Run, "2" + PersonaPersistence.FonoFijo };
            Execute(sqlSentence, arrayParameters, arrayObjects);
            arrayObjects = new object[] { PersonaPersistence.Run, "9" + PersonaPersistence.FonoCel };
            Execute(sqlSentence, arrayParameters, arrayObjects);
            #endregion

            if (PersonaPersistence is Usuario)
            {
                #region Insert into Usuario
                sqlSentence = "INSERT INTO Usuario (rut, estado, fec_nac) VALUES (@Run, @Estado, @FecNacimiento);";
                arrayParameters = new[] { "@Run", "@Estado", "@FecNacimiento" };
                arrayObjects = new object[] { ((Usuario)PersonaPersistence).Run, ((Usuario)PersonaPersistence).Estado, ((Usuario)PersonaPersistence).FecNacimiento };
                var executeUser = Execute(sqlSentence, arrayParameters, arrayObjects);

                if (!executeUser.Status) return executeUser;
                #endregion
                var ficha = FetchLastNroFicha();
                if ((Usuario)PersonaPersistence is Funcionario)
                {
                    #region Insert into Funcionario
                    sqlSentence = "INSERT INTO Funcionario (nro_ficha, cargo) VALUES (@NroFicha, @Cargo);";
                    arrayParameters = new[] { "@NroFicha", "@Cargo" };
                    arrayObjects = new object[] { ficha, ((Funcionario)PersonaPersistence).Cargo };

                    var executeFuncionario = Execute(sqlSentence, arrayParameters, arrayObjects);
                    if (executeFuncionario.Status) executeFuncionario.Mensaje = string.Format("Funcionario agregado. N° de Ficha: {0}", ficha);
                    PersonaPersistence = null;
                    return executeFuncionario;
                    #endregion
                }
                if ((Usuario)PersonaPersistence is Estudiante)
                {
                    #region Insert into Estudiante
                    sqlSentence = "INSERT INTO Estudiante (nro_ficha, curso) VALUES (@NroFicha, @Curso);";
                    arrayParameters = new[] { "@NroFicha", "@Curso" };
                    arrayObjects = new object[] { ficha, ((Estudiante)PersonaPersistence).Curso };

                    var executeEstudiante = Execute(sqlSentence, arrayParameters, arrayObjects);
                    if (executeEstudiante.Status) executeEstudiante.Mensaje = string.Format("Estudiante agregado. N° de Ficha: {0}", ficha);
                    PersonaPersistence = null;
                    return executeEstudiante;
                    #endregion
                }
            }
            else if (PersonaPersistence is Apoderado)
            {
                #region Insert into Apoderado
                sqlSentence = "INSERT INTO Apoderado (rut, nro_ficha, parentesco) VALUES (@Run, @NroFicha, @Parentesco);";
                arrayParameters = new[] { "@Run", "@NroFicha", "@Parentesco" };
                arrayObjects = new object[] { ((Apoderado)PersonaPersistence).Run, ((Apoderado)PersonaPersistence).NroFicha, ((Apoderado)PersonaPersistence).Parentesco };

                var executeApoderado = Execute(sqlSentence, arrayParameters, arrayObjects);
                if (executeApoderado.Status) executeApoderado.Mensaje = "Apoderado agregado exitosamente";
                PersonaPersistence = null;
                return executeApoderado;
                #endregion
            }
            return executePer;
        }
        public Message InsertApoderadoOnly(string run, int numFicha, string parentesco)
        {
            const string sqlSentence = "INSERT INTO Apoderado (rut, nro_ficha, parentesco) VALUES (@Run, @NroFicha, @Parentesco);";
            var arrayParameters = new[] { "@Run", "@NroFicha", "@Parentesco" };
            var arrayObjects = new object[] { run, numFicha, parentesco };

            return Execute(sqlSentence, arrayParameters, arrayObjects);
        }
        public Message Update()
        {
            if (PersonaPersistence == null) return new Message(false, "Debe precargar los datos de una persona");

            string[] arrayParameters;
            object[] arrayObjects;

            #region Update Persona

            var sqlSentence = "UPDATE Persona SET nombre=@Nombre, apellido=@Apellido, direccion=@Direccion, cod_comuna=@CodComuna WHERE rut=@Run;";

            arrayParameters = new[] { "@Nombre", "@Apellido", "@Direccion", "@CodComuna", "@Run" };
            arrayObjects = new object[] { PersonaPersistence.Nombre, PersonaPersistence.Apellido, PersonaPersistence.Direccion, PersonaPersistence.CodComuna, PersonaPersistence.Run };

            var executePer = Execute(sqlSentence, arrayParameters, arrayObjects);
            if (!executePer.Status)
            {
                return executePer;
            }
            #endregion
            #region Update Telefono
            Execute("DELETE FROM Telefono WHERE rut=@Run;", new[] { "@Run" }, new object[] { PersonaPersistence.Run });

            sqlSentence = "INSERT INTO Telefono (rut, telefono) VALUES (@Run, @Telefono);";
            arrayParameters = new[] { "@Run", "@Telefono" };
            arrayObjects = new object[] { PersonaPersistence.Run, "2" + PersonaPersistence.FonoFijo };
            Execute(sqlSentence, arrayParameters, arrayObjects);
            arrayObjects = new object[] { PersonaPersistence.Run, "9" + PersonaPersistence.FonoCel };
            Execute(sqlSentence, arrayParameters, arrayObjects);
            #endregion
            //ToDo Continue here
            if (PersonaPersistence is Usuario)
            {
                #region Insert into Usuario
                sqlSentence = "INSERT INTO Usuario (rut, estado, fec_nac) VALUES (@Run, @Estado, @FecNacimiento);";
                arrayParameters = new[] { "@Run", "@Estado", "@FecNacimiento" };
                arrayObjects = new object[] { ((Usuario)PersonaPersistence).Run, ((Usuario)PersonaPersistence).Estado, ((Usuario)PersonaPersistence).FecNacimiento };
                var executeUser = Execute(sqlSentence, arrayParameters, arrayObjects);

                if (!executeUser.Status) return executeUser;
                #endregion
                var ficha = FetchLastNroFicha();
                if ((Usuario)PersonaPersistence is Funcionario)
                {
                    #region Insert into Funcionario
                    sqlSentence = "INSERT INTO Funcionario (nro_ficha, cargo) VALUES (@NroFicha, @Cargo);";
                    arrayParameters = new[] { "@NroFicha", "@Cargo" };
                    arrayObjects = new object[] { ficha, ((Funcionario)PersonaPersistence).Cargo };

                    var executeFuncionario = Execute(sqlSentence, arrayParameters, arrayObjects);
                    if (executeFuncionario.Status) executeFuncionario.Mensaje = string.Format("Funcionario agregado. N° de Ficha: {0}", ficha);
                    PersonaPersistence = null;
                    return executeFuncionario;
                    #endregion
                }
                if ((Usuario)PersonaPersistence is Estudiante)
                {
                    #region Insert into Estudiante
                    sqlSentence = "INSERT INTO Estudiante (nro_ficha, curso) VALUES (@NroFicha, @Curso);";
                    arrayParameters = new[] { "@NroFicha", "@Curso" };
                    arrayObjects = new object[] { ficha, ((Estudiante)PersonaPersistence).Curso };

                    var executeEstudiante = Execute(sqlSentence, arrayParameters, arrayObjects);
                    if (executeEstudiante.Status) executeEstudiante.Mensaje = string.Format("Estudiante agregado. N° de Ficha: {0}", ficha);
                    PersonaPersistence = null;
                    return executeEstudiante;
                    #endregion
                }
            }
            else if (PersonaPersistence is Apoderado)
            {
                #region Insert into Apoderado
                sqlSentence = "INSERT INTO Apoderado (rut, nro_ficha, parentesco) VALUES (@Run, @NroFicha, @Parentesco);";
                arrayParameters = new[] { "@Run", "@NroFicha", "@Parentesco" };
                arrayObjects = new object[] { ((Apoderado)PersonaPersistence).Run, ((Apoderado)PersonaPersistence).NroFicha, ((Apoderado)PersonaPersistence).Parentesco };

                var executeApoderado = Execute(sqlSentence, arrayParameters, arrayObjects);
                if (executeApoderado.Status) executeApoderado.Mensaje = "Apoderado agregado exitosamente";
                PersonaPersistence = null;
                return executeApoderado;
                #endregion
            }
            return executePer;
        }
        //ToDo Update method
        #endregion
        #region Select querys
        public List<Comuna> FetchComunas()
        {
            var comunaTable = Select("SELECT cod_comuna, nom_comuna FROM Comuna ORDER BY nom_comuna;");
            var listado = new List<Comuna>();
            if (comunaTable == null) return listado;

            for (var i = 0; i < comunaTable.Rows.Count; i++)
            {
                listado.Add(new Comuna(comunaTable.Rows[i].Field<int>(0), comunaTable.Rows[i].Field<string>(1)));
            }
            return listado;
        }
        public Message FetchUsuario(string ficha)
        {
            int nroFicha;

            if (!int.TryParse(ficha, out nroFicha)) return new Message(false, "Número de ficha inválido");

            var exists = Select("SELECT estado from Usuario WHERE nro_ficha = @NroFicha;", new[] { "@NroFicha" }, new object[] { nroFicha }).Rows.Count != 0;
            if (!exists) return new Message(false, "No se ha encontrado la ficha");

            string sqlSentence;
            DataTable runTable;
            DataTable phoneTable;
            var fonoFijo = string.Empty;
            var fonoCel = string.Empty;

            if (IsStudent(nroFicha))
            {
                sqlSentence =
                    "SELECT Persona.rut, Persona.nombre, Persona.apellido, Persona.direccion, Persona.cod_comuna, " +
                    "Usuario.nro_ficha, Usuario.estado, Usuario.fec_nac, Estudiante.curso FROM Persona " +
                    "JOIN Usuario ON Usuario.rut=Persona.rut " +
                    "JOIN Estudiante ON Estudiante.nro_ficha=Usuario.nro_ficha " +
                    "WHERE Estudiante.nro_ficha=@NroFicha;";
                runTable = Select(sqlSentence, new[] { "@NroFicha" }, new object[] { nroFicha });

                sqlSentence = "SELECT telefono FROM Telefono WHERE rut=@Run;";
                phoneTable = Select(sqlSentence, new[] { "@Run" }, new object[] { runTable.Rows[0].Field<string>("rut") });
                foreach (DataRow row in phoneTable.Rows)
                {
                    if (row.Field<string>("telefono").StartsWith("9"))
                        fonoCel = row.Field<string>("telefono").Substring(1);

                    else
                        fonoFijo = row.Field<string>("telefono").Substring(1);
                }
                PersonaPersistence = new Estudiante(runTable.Rows[0].Field<string>("rut"), runTable.Rows[0].Field<string>("nombre"), runTable.Rows[0].Field<string>("apellido"), runTable.Rows[0].Field<string>("direccion"), runTable.Rows[0].Field<int>("cod_comuna"), fonoFijo, fonoCel, runTable.Rows[0].Field<DateTime>("fec_nac"), nroFicha, runTable.Rows[0].Field<bool>("estado"), runTable.Rows[0].Field<string>("curso"));

                sqlSentence = "SELECT rut, parentesco FROM Apoderado WHERE nro_ficha=@NroFicha";
                runTable = Select(sqlSentence, new[] { "@NroFicha" }, new object[] { nroFicha });
                RunApoderado = runTable.Rows[0].Field<string>("rut");
                Parentesco = runTable.Rows[0].Field<string>("parentesco");
            }
            else
            {
                sqlSentence =
                    "SELECT Persona.rut, Persona.nombre, Persona.apellido, Persona.direccion, Persona.cod_comuna, " +
                    "Usuario.nro_ficha, Usuario.estado, Usuario.fec_nac, Funcionario.cargo FROM Persona " +
                    "JOIN Usuario ON Usuario.rut=Persona.rut " +
                    "JOIN Funcionario ON Funcionario.nro_ficha=Usuario.nro_ficha " +
                    "WHERE Funcionario.nro_ficha=@NroFicha;";
                runTable = Select(sqlSentence, new[] { "@NroFicha" }, new object[] { nroFicha });

                sqlSentence = "SELECT telefono FROM Telefono WHERE rut=@Run;";
                phoneTable = Select(sqlSentence, new[] { "@Run" }, new object[] { runTable.Rows[0].Field<string>("rut") });
                foreach (DataRow row in phoneTable.Rows)
                {
                    if (row.Field<string>("telefono").StartsWith("9"))
                        fonoCel = row.Field<string>("telefono").Substring(1);

                    else
                        fonoFijo = row.Field<string>("telefono").Substring(1);
                }
                PersonaPersistence = new Funcionario(runTable.Rows[0].Field<string>("rut"), runTable.Rows[0].Field<string>("nombre"), runTable.Rows[0].Field<string>("apellido"), runTable.Rows[0].Field<string>("direccion"), runTable.Rows[0].Field<int>("cod_comuna"), fonoFijo, fonoCel, runTable.Rows[0].Field<DateTime>("fec_nac"), nroFicha, runTable.Rows[0].Field<bool>("estado"), runTable.Rows[0].Field<string>("cargo"));
            }
            return new Message(true);
        }
        public int FetchLastNroFicha()
        {
            var nroFicha = Select("SELECT MAX(nro_ficha) FROM Usuario;");
            return nroFicha.Rows[0].Field<int>(0);
        }
        #endregion
        #region Existance Check Querys
        public Message ExistsRun(string run)
        {
            var exists = Select("SELECT * from Persona WHERE rut = @Run;", new[] { "@Run" }, new object[] { run }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No se ha encontrado el RUN");
        }
        #endregion
        #region Is Student Querys
        public bool IsStudent(string run)
        {
            var studentTable = Select(
                "SELECT Estudiante.nro_ficha, Usuario.estado, Persona.rut FROM Estudiante " +
                "JOIN Usuario ON Usuario.nro_ficha=Estudiante.nro_ficha " +
                "JOIN Persona ON Persona.rut=Usuario.rut " +
                "WHERE Persona.rut=@Run;", new[] { "@Run" }, new object[] { run });

            return studentTable.Rows.Count != 0;
        }
        private bool IsStudent(int nroFicha)
        {
            var studentTable = Select("SELECT curso FROM Estudiante WHERE nro_ficha=@NroFicha;", new[] { "@NroFicha" }, new object[] { nroFicha });
            return studentTable.Rows.Count != 0;
        }
        #endregion
        #region Custom Method
        public void ClearPersistantData()
        {
            PersonaPersistence = null;
            RunApoderado = string.Empty;
            Parentesco = string.Empty;
        }
        #endregion
    }
}