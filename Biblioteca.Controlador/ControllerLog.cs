using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}