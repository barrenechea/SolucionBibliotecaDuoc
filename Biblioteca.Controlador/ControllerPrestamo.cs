using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Entidad;

namespace Biblioteca.Controlador
{
    public class ControllerPrestamo : ControllerDatabase
    {
        private Prestamo PrestamoPersistence {get; set; }
        #region Preload method
        /// <summary>
        /// This method is to preload a Prestamo into the controller before requesting an Insert or Update query.
        /// </summary>
        /// <param name="fechaPrestamo">Fecha en la que se presta el libro</param>
        /// <param name="nroFicha">Número de ficha del solicitante</param>
        /// <param name="codPrestamo">Codigo que identifica al préstamo</param>
        /// <returns>Message, indicates status of the preload, and a message in case of failure</returns>
        public Message PreloadPrestamo(DateTime fechaPrestamo, int nroFicha)
        {
            Message msg;
            try
            {
                PrestamoPersistence = new Prestamo(fechaPrestamo,nroFicha);
                msg = new Message(true);
            }
            catch
            {
                msg = new Message(false, "Verifique los parámetros");
            }
            return msg;
        }
        #endregion


        #region Querys
        public Message Insert()
        {
            if (PrestamoPersistence == null) return new Message(false, "Debe precargar los datos de una persona");

            string sqlSentence;
            string[] arrayParameters;
            object[] arrayObjects;

            

            sqlSentence = "INSERT INTO Prestamo (fec_prestamo, nro_ficha) " +
                          "VALUES (@FecPrestamo, @NroFicha);";
            arrayParameters = new[] { "@FecPrestamo", "@NroFicha" };
            arrayObjects = new object[] { DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), PrestamoPersistence.NroFicha };

            var executePre = Execute(sqlSentence, arrayParameters, arrayObjects);

            #region Insert into Detalle prestamo

            sqlSentence = "INSERT INTO detalle_prestamo(fec_devolucion, libro_devuelto, renovacion, cod_prestamo, cod_libro) " +
                          "VALUES (@FecDevolucion, @LibroDevuelto, @Renovacion, @CodPrestamo, @CodLibro);";

            arrayParameters = new[] { "@FecDevolucion", "@LibroDevuelto", "@Renovacion", "@CodPrestamo", "@CodLibro"};
            arrayObjects = new object[] { DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), false, 0, PrestamoPersistence.CodPrestamo, };
            
            //Execute(sqlSentence, arrayParameters, arrayObjects);
            //arrayObjects = new object[] { PersonaPersistence.Run, "9" + PersonaPersistence.FonoCel };
            //Execute(sqlSentence, arrayParameters, arrayObjects);

            //toDo sumar los días habiles, select al codigo de préstamo y saber de donde chucha saco el codigo del libro que se pide. Pd: te amo seba <3

            #endregion
            return executePre;
        }


        #endregion



        public Message UsuarioActivado(string nroFicha)
        {
            var estado = Select("SELECT estado from Usuario WHERE nro_ficha = @Nro_ficha;", new[] { "@Nro_ficha" }, new object[] { nroFicha });
            return new Message(estado.Rows[0].Field<bool>(0), estado.Rows[0].Field<bool>(0) ? null : "El usuario está desactivado, revise hoja de morosidad.");
        }
    }
}
