using System;
using System.Data;
using System.IO;
using Biblioteca.Controlador.Utils;
using Biblioteca.Entidad;
using MySql.Data.MySqlClient;

namespace Biblioteca.Controlador
{
    public abstract class ControllerDatabase
    {
        #region Attributes
        private readonly MySqlConnection _connection;
        private static string _server;
        private static string _port;
        private static string _database;
        private static string _username;
        private static string _password;
        #endregion
        #region Constructor
        /// <summary>
        /// Constructor of this class.
        /// Consider that this is an abstract class, so this is going to be loaded from a child class.
        /// </summary>
        protected ControllerDatabase()
        {
            FetchConfig();

            var cadenaConexion = string.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4}; CHARSET=utf8", _server, _port, _database, _username, _password);
            _connection = new MySqlConnection
            {
                ConnectionString = cadenaConexion
            };
        }
        #endregion
        #region Ini creation and read methods
        /// <summary>
        /// Fetch all parameters from the external config file
        /// </summary>
        private static void FetchConfig()
        {
            IniFix();
            var parser = new ControllerIniParser("config.ini");

            _server = parser.GetSetting("BIBLIOTECA", "SERVER");
            _port = parser.GetSetting("BIBLIOTECA", "PORT");
            _database = parser.GetSetting("BIBLIOTECA", "DATABASE");
            _username = parser.GetSetting("BIBLIOTECA", "USERNAME");
            _password = parser.GetSetting("BIBLIOTECA", "PASSWORD");
        }
        /// <summary>
        /// Checks if the external config file exists or not.
        /// If doesn't exist, it creates the file.
        /// </summary>
        private static void IniFix()
        {
            if (File.Exists("config.ini")) return;

            using (var writer = new StreamWriter("config.ini"))
            {
                writer.WriteLine("[BIBLIOTECA]");
                writer.WriteLine("SERVER=localhost");
                writer.WriteLine("PORT=3306");
                writer.WriteLine("DATABASE=biblioteca");
                writer.WriteLine("USERNAME=root");
                writer.WriteLine("PASSWORD=");
            }
        }
        #endregion
        #region SQL Methods
        /// <summary>
        /// Just a basic connection testing method.
        /// </summary>
        /// <returns>Boolean that indicates if the connection is successful or not</returns>
        public Message TestConnection()
        {
            try
            {
                if (_connection.State != ConnectionState.Closed)
                    return new Message(false, "Por favor espere...");
                
                _connection.Open();
                return new Message(true);
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        return new Message(false, "No se pudo conectar al servidor");
                    case 1042:
                        return new Message(false, "No se pudo conectar al servidor");
                    case 1045:
                        return new Message(false, "Usuario/Contraseña inválidos");
                    default:
                        return new Message(false, e.Message);
                }
            }
            finally
            {
                _connection.Close();
            }
        }
        /// <summary>
        /// Obtains the Server version
        /// </summary>
        /// <returns>Message that indicates if the fetch was successful, and the version itself</returns>
        public Message FetchVersion()
        {
            try
            {
                _connection.Open();
                using (var cmd = new MySqlCommand("SELECT VERSION();", _connection))
                    return new Message(true, Convert.ToString(cmd.ExecuteScalar()));
            }
            catch
            {
                return new Message(false, "Unable to fetch version");
            }
            finally
            {
                _connection.Close();
            }
        }
        /// <summary>
        /// Insert, update or delete something into the database.
        /// This method prepares the statement.
        /// ¡USE THIS TO AVOID SQL INJECTION!
        /// </summary>
        /// <param name="sentence">The sentence to be prepared and executed</param>
        /// <param name="parameters">Array of parameters to be prepared on the SQL sentence</param>
        /// <param name="values">Array of any object type to be prepared on the SQL sentence</param>
        /// <returns>Message that indicates if the execution was successful or not</returns>
        protected Message Execute(string sentence, string[] parameters, object[] values)
        {
            if (parameters.Length != values.Length) throw new ArgumentException("Parameters must be equal to values");
            try
            {
                _connection.Open();
                using (var cmd = new MySqlCommand(sentence, _connection))
                {
                    cmd.Prepare();
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.AddWithValue(parameters[i], values[i]);
                    }
                    cmd.ExecuteNonQuery();
                }
                return new Message(true);
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 1062:
                        return new Message(false, "1");
                    default:
                        return new Message(false, e.Message);
                }
            }
            finally
            {
                _connection.Close();
            }
        }
        /// <summary>
        /// Fetch data from any specified sentence.
        /// Do NOT use this method if you're inserting parameters on your SQL sentence!
        /// </summary>
        /// <param name="sentence">The SQL sentence to run</param>
        /// <returns>DataTable with the requested data</returns>
        protected DataTable Select(string sentence)
        {
            try
            {
                _connection.Open();
                using (var cmd = new MySqlCommand(sentence, _connection))
                {
                    cmd.CommandType = CommandType.Text;
                    using (var dataAdapter = new MySqlDataAdapter(cmd))
                    {
                        var table = new DataTable();
                        dataAdapter.Fill(table);
                        return table;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
        /// <summary>
        /// Fetch data from any specified sentence.
        /// This method prepares the statement.
        /// ¡USE THIS TO AVOID SQL INJECTION!
        /// </summary>
        /// <param name="sentence">The SQL sentence to prepare and run</param>
        /// <param name="parameters">Array of parameters to be prepared on the SQL sentence</param>
        /// <param name="values">Array of any object to be prepared on the SQL sentence</param>
        /// <returns></returns>
        protected DataTable Select(string sentence, string[] parameters, object[] values)
        {
            if(parameters.Length != values.Length) throw new ArgumentException("Parameters must be equal to values");
            try
            {
                _connection.Open();
                using (var cmd = new MySqlCommand(sentence, _connection))
                {
                    cmd.Prepare();
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.AddWithValue(parameters[i], values[i]);
                    }
                    
                    cmd.CommandType = CommandType.Text;
                    using (var dataAdapter = new MySqlDataAdapter(cmd))
                    {
                        var table = new DataTable();
                        dataAdapter.Fill(table);
                        return table;
                    }
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }
        #endregion
    }
}