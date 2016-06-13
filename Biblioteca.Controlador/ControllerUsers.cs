using System;
using System.Collections.Generic;
using System.Data;
using Biblioteca.Entidad;
// ReSharper disable JoinDeclarationAndInitializer

namespace Biblioteca.Controlador
{
    public class ControllerUsers : ControllerLog
    {
        #region Attributes
        public Persona PersonaPersistence { get; private set; }
        public string RunApoderado { get; private set; }
        public string Parentesco { get; private set; }
        #endregion
        #region Preload methods
        /// <summary>
        /// Preload an Estudiante into memory. Use this before executing an Insert or Update
        /// </summary>
        /// <param name="run">RUN Estudiante</param>
        /// <param name="nombre">Nombre Estudiante</param>
        /// <param name="apellido">Apellido Estudiante</param>
        /// <param name="direccion">Dirección Estudiante</param>
        /// <param name="codComuna">Codigo de Comuna de Estudiante</param>
        /// <param name="fonoFijo">Fono Fijo de Estudiante</param>
        /// <param name="fonoCel">Fono Celular de Estudiante</param>
        /// <param name="fecNacimiento">Fecha Nacimiento de Estudiante</param>
        /// <param name="estado">Estado (indica si la cuenta está habilitada o no)</param>
        /// <param name="curso">Curso de Estudiante</param>
        /// <param name="runApoderado">RUN Apoderado del Estudiante</param>
        /// <param name="parentesco">Parentesco del Apoderado con respecto al Estudiante</param>
        /// <param name="nroFicha">Nro Ficha del Estudiante (Opcional for Insert, mandatory for Update)</param>
        /// <returns>Message that indicates if the Preload was executed successfully or not</returns>
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
        /// <summary>
        /// Preload a Funcionario into memory. Use this before executing an Insert or Update
        /// </summary>
        /// <param name="run">RUN Funcionario</param>
        /// <param name="nombre">Nombre Funcionario</param>
        /// <param name="apellido">Apellido Funcionario</param>
        /// <param name="direccion">Dirección Funcionario</param>
        /// <param name="codComuna">Codigo de Comuna de Funcionario</param>
        /// <param name="fonoFijo">Fono Fijo de Funcionario</param>
        /// <param name="fonoCel">Fono Celular de Funcionario</param>
        /// <param name="fecNacimiento">Fecha Nacimiento de Funcionario</param>
        /// <param name="estado">Estado (indica si la cuenta está habilitada o no)</param>
        /// <param name="cargo">Cargo de Funcionario</param>
        /// <param name="nroFicha">Nro Ficha del Estudiante (Opcional for Insert, mandatory for Update)</param>
        /// <returns>Message that indicates if the Preload was executed successfully or not</returns>
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
        /// <summary>
        /// Preload an Apoderado into memory. Use this before executing an Insert or Update
        /// </summary>
        /// <param name="run">RUN Apoderado</param>
        /// <param name="nombre">Nombre Apoderado</param>
        /// <param name="apellido">Apellido Apoderado</param>
        /// <param name="direccion">Dirección Apoderado</param>
        /// <param name="codComuna">Codigo de Comuna de Apoderado</param>
        /// <param name="fonoFijo">Fono Fijo de Apoderado</param>
        /// <param name="fonoCel">Fono Celular de Apoderado</param>
        /// <param name="numFicha">Num Ficha de Estudiante asociada a Apoderado</param>
        /// <param name="parentesco">Parentesco del Apoderado con respecto al Estudiante</param>
        /// <returns>Message that indicates if the Preload was executed successfully or not</returns>
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
        /// <summary>
        /// Insert an Estudiante, Funcionario or Apoderado into the database.
        /// ¡Preload is required before executing this!
        /// </summary>
        /// <returns>Message that indicates if the Insert was successful or not</returns>
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
                var ficha = FetchNroFicha(((Usuario)PersonaPersistence).Run);
                if ((Usuario)PersonaPersistence is Funcionario)
                {
                    #region Insert into Funcionario
                    sqlSentence = "INSERT INTO Funcionario (nro_ficha, cargo) VALUES (@NroFicha, @Cargo);";
                    arrayParameters = new[] { "@NroFicha", "@Cargo" };
                    arrayObjects = new object[] { ficha, ((Funcionario)PersonaPersistence).Cargo };

                    var executeFuncionario = Execute(sqlSentence, arrayParameters, arrayObjects);
                    if (executeFuncionario.Status) executeFuncionario.Mensaje = string.Format("Funcionario agregado. N° de Ficha: {0}", ficha);
                    Log(string.Format("Funcionario agregado. Ficha: {0}", ficha));
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
                    Log(string.Format("Estudiante agregado. Ficha: {0}", ficha));
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
                Log(string.Format("Apoderado agregado. RUN: {0} [Ficha asociada: {1}]", PersonaPersistence.Run, ((Apoderado)PersonaPersistence).NroFicha));
                PersonaPersistence = null;
                return executeApoderado;
                #endregion
            }
            return executePer;
        }
        /// <summary>
        /// Insert only an Apoderado onto the Apoderado table at the database.
        /// </summary>
        /// <param name="run">RUN Apoderado</param>
        /// <param name="numFicha">Ficha de Estudiante asociada a Apoderado</param>
        /// <param name="parentesco">Parentesco entre Apoderado y Estudiante</param>
        /// <returns>Message that indicates if the Insert was successful or not</returns>
        public Message InsertApoderadoOnly(string run, int numFicha, string parentesco)
        {
            const string sqlSentence = "INSERT INTO Apoderado (rut, nro_ficha, parentesco) VALUES (@Run, @NroFicha, @Parentesco);";
            var arrayParameters = new[] { "@Run", "@NroFicha", "@Parentesco" };
            var arrayObjects = new object[] { run, numFicha, parentesco };
            Log(string.Format("Apoderado asociado. RUN: {0} [Ficha asociada: {1}]", run, numFicha));
            return Execute(sqlSentence, arrayParameters, arrayObjects);
        }
        /// <summary>
        /// Updates an existing Estudiante or Funcionario at the database.
        /// ¡Preload is required before executing this!
        /// </summary>
        /// <returns>Message that indicates if the Update was successful or not</returns>
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
            if (!executePer.Status) return executePer;
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

            if (!(PersonaPersistence is Usuario)) return executePer;

            #region Update Usuario
            sqlSentence = "UPDATE Usuario SET estado = @Estado, fec_nac = @FecNac WHERE nro_ficha = @NroFicha;";
            arrayParameters = new[] { "@Estado", "@FecNac", "@NroFicha" };
            arrayObjects = new object[] { ((Usuario)PersonaPersistence).Estado, ((Usuario)PersonaPersistence).FecNacimiento, ((Usuario)PersonaPersistence).NroFicha };
            var executeUser = Execute(sqlSentence, arrayParameters, arrayObjects);

            if (!executeUser.Status) return executeUser;
            #endregion
            Message execute;
            if ((Usuario)PersonaPersistence is Funcionario)
            {
                #region Update Funcionario
                sqlSentence = "UPDATE Funcionario SET cargo = @Cargo WHERE nro_ficha = @NroFicha;";
                arrayParameters = new[] { "@Cargo", "@NroFicha" };
                arrayObjects = new object[] { ((Funcionario)PersonaPersistence).Cargo, ((Usuario)PersonaPersistence).NroFicha };

                execute = Execute(sqlSentence, arrayParameters, arrayObjects);
                if (!execute.Status) return execute;
                
                Log(string.Format("Funcionario actualizado. Ficha: {0}", ((Usuario)PersonaPersistence).NroFicha));
                #endregion
            }
            else
            {
                #region Update Estudiante
                sqlSentence = "UPDATE Estudiante SET curso = @Curso WHERE nro_ficha = @NroFicha;";
                arrayParameters = new[] { "@Curso", "@NroFicha" };
                arrayObjects = new object[] { ((Estudiante)PersonaPersistence).Curso, ((Usuario)PersonaPersistence).NroFicha };

                execute = Execute(sqlSentence, arrayParameters, arrayObjects);
                if (!execute.Status) return execute;

                Log(string.Format("Estudiante actualizado. Ficha: {0}", ((Usuario)PersonaPersistence).NroFicha));
                #endregion
                #region Update Parentesco
                sqlSentence = "UPDATE Apoderado SET parentesco = @Parentesco WHERE rut = @RunApoderado AND nro_ficha = @NroFicha;";
                arrayParameters = new[] { "@Parentesco", "@RunApoderado", "@NroFicha" };
                arrayObjects = new object[] { Parentesco, RunApoderado, ((Usuario)PersonaPersistence).NroFicha };

                execute = Execute(sqlSentence, arrayParameters, arrayObjects);
                #endregion
            }
            if (execute.Status) execute.Mensaje = string.Format("Ficha N° {0} actualizada", ((Usuario)PersonaPersistence).NroFicha);
            PersonaPersistence = null;
            return execute;
        }
        #endregion
        #region Fetch querys
        /// <summary>
        /// Fetch all Comunas at the Database
        /// </summary>
        /// <returns>List of Comunas</returns>
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
        /// <summary>
        /// Fetch an specific Usuario from the database and loads it onto local memory
        /// </summary>
        /// <param name="ficha">Numero de Ficha from the Usuario</param>
        /// <returns>Message that indicates if the fetch was successful or not</returns>
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

            if (IsStudent(nroFicha).Status)
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
        /// <summary>
        /// Fetch the Numero de Ficha from an specific RUN
        /// </summary>
        /// <param name="run">RUN from the Usuario</param>
        /// <returns>Numero de Ficha associated to the specified RUN</returns>
        public int FetchNroFicha(string run)
        {
            var nroFicha = Select("SELECT nro_ficha FROM Usuario WHERE rut = @Run;", new[] { "@Run" }, new object[] { run });
            return nroFicha.Rows[0].Field<int>(0);
        }
        #endregion
        #region Validation Querys
        /// <summary>
        /// Checks if an specific RUN exists on the Database
        /// </summary>
        /// <param name="run">RUN to be searched</param>
        /// <returns>Message that indicates if the RUN exists or not</returns>
        public Message ExistsRun(string run)
        {
            var exists = Select("SELECT * from Persona WHERE rut = @Run;", new[] { "@Run" }, new object[] { run }).Rows.Count != 0;
            return new Message(exists, exists ? null : "No se ha encontrado el RUN");
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
        /// <summary>
        /// Checks if an specific RUN is an Estudiante
        /// </summary>
        /// <param name="run">RUN to be searched</param>
        /// <returns>Boolean that indicates if the specified RUN is an Estudiante or not</returns>
        public bool IsStudent(string run)
        {
            var studentTable = Select(
                "SELECT Estudiante.nro_ficha, Usuario.estado, Persona.rut FROM Estudiante " +
                "JOIN Usuario ON Usuario.nro_ficha=Estudiante.nro_ficha " +
                "JOIN Persona ON Persona.rut=Usuario.rut " +
                "WHERE Persona.rut=@Run;", new[] { "@Run" }, new object[] { run });

            return studentTable.Rows.Count != 0;
        }
        /// <summary>
        /// Checks if an specific Numero de Ficha is an Estudiante
        /// </summary>
        /// <param name="nroFicha">Numero de Ficha to be searched</param>
        /// <returns>Message that indicates if the specified Numero de Ficha is an Estudiante or not</returns>
        public Message IsStudent(int nroFicha)
        {
            var studentTable = Select("SELECT curso FROM Estudiante WHERE nro_ficha=@NroFicha;", new[] { "@NroFicha" }, new object[] { nroFicha });
            return studentTable.Rows.Count != 0 ? new Message(true) : new Message(false, "El usuario no es un estudiante");
        }
        /// <summary>
        /// Revisa si un usuario puede pedir libros de a cuerdo a su estado en la base de datos
        /// </summary>
        /// <param name="nroFicha">Numero de ficha del usuario a consultar</param>
        /// <returns>Message con el estado (True o False). En caso de ser false, se retorna mensaje correspondiente</returns>
        public Message IsEnabled(string nroFicha)
        {
            var estado = Select("SELECT estado from Usuario WHERE nro_ficha = @Nro_ficha;", new[] { "@Nro_ficha" }, new object[] { nroFicha });
            return new Message(estado.Rows[0].Field<bool>(0), estado.Rows[0].Field<bool>(0) ? null : "Usuario tiene sanción activa.");
        }
        #endregion
        #region Custom Method
        /// <summary>
        /// Removes all persistant data inside this controller
        /// </summary>
        public void ClearPersistantData()
        {
            PersonaPersistence = null;
            RunApoderado = string.Empty;
            Parentesco = string.Empty;
        }
        #endregion
    }
}