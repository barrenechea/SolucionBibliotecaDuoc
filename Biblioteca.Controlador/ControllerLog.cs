using System;
using System.Collections.Generic;
using System.Data;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerLog : ControllerDatabase
    {
        #region Attribute
        public string LogUsername { private get; set; }
        #endregion
        #region Insert Query
        /// <summary>
        /// Generates an entry inside the Log table
        /// </summary>
        /// <param name="activity">Activity that Admin has done</param>
        protected void Log(string activity)
        {
            Execute("INSERT INTO Log(actividad, id_usuario) VALUES (@Activity, @LogUsername);", new[] { "@Activity", "@LogUsername" }, new object[] { activity, LogUsername });
        }
        #endregion
        #region Fetch Query
        /// <summary>
        /// Fetch all data inside Log table
        /// </summary>
        /// <returns>List of Log data</returns>
        public List<Log> FetchAll()
        {
            var logTable = Select("SELECT actividad, id_usuario, fecha FROM Log ORDER BY fecha DESC");

            if (logTable == null) return null;

            var listado = new List<Log>();
            for (var i = 0; i < logTable.Rows.Count; i++)
            {
                listado.Add(new Log(logTable.Rows[i].Field<string>("actividad"), logTable.Rows[i].Field<string>("id_usuario"), logTable.Rows[i].Field<DateTime>("fecha")));
            }

            Log("Visualizó Log");
            return listado;
        }
        #endregion
    }
}