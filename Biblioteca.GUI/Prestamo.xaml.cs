using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Lógica de interacción para Prestamo.xaml
    /// </summary>
    public partial class Prestamo
    {
        public Prestamo()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Close();
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {

            if (App.Prestamo.TestConnection().Status)
            {
                if (!Validation()) return;
                ExecutePrestamo();
            }
            else
            {
                lblStatus.Content = "Se ha perdido la conexión con el servidor";
            }
        }

        /// <summary>
        /// validaciones del formulario segun el modelo de negocio
        /// </summary>
        /// <returns>bool de confirmacion de que están todos los parametros bién </returns>
        private bool Validation()
        {
            if (string.IsNullOrWhiteSpace(txtNroFicha.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNroFicha.Focus();
                return false;
            }
            int i;
            if (!int.TryParse(txtNroFicha.Text, out i))
            {
                lblStatus.Content = "El número de ficha es de solo números";
                txtNroFicha.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCodLibro.Text))
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtCodLibro.Focus();
                return false;
            }
            var codigos = txtCodLibro.Text.ToUpper().Split(',');
            if (codigos.Any(string.IsNullOrWhiteSpace))
            {
                lblStatus.Content = "No debe ingresar espacios en blanco entre las comas";
                txtCodLibro.Focus();
                return false;
            }
            var resultado = App.Prestamo.ExistsNroFicha(txtNroFicha.Text);
            if (resultado.Status)
            {
                var prestados = App.Prestamo.CantLibrosPrestados(txtNroFicha.Text);
                if (App.Users.IsStudent(int.Parse(txtNroFicha.Text)).Status)
                {
                    if (codigos.Length > 1)
                    {
                        lblStatus.Content = "Los estudiantes no pueden solicitar más de un libro";
                        txtCodLibro.Focus();
                        return false;
                    }
                    if (prestados >= 1)
                    {
                        lblStatus.Content = "El estudiante tiene un prestamo pendiente";
                        return false;
                    }
                }
                else
                {
                    if (prestados + codigos.Length > 5)
                    {
                        lblStatus.Content = "Los funcionarios no pueden tener más de cinco libros";
                        txtCodLibro.Focus();
                        return false;
                    }
                }
            }
            else
            {
                lblStatus.Content = resultado.Mensaje;
                txtNroFicha.Focus();
                return false;
            }

            resultado = App.Prestamo.UsuarioActivado(txtNroFicha.Text);
            if (!resultado.Status)
            {
                lblStatus.Content = resultado.Mensaje;
                txtNroFicha.Focus();
                return false;
            }
            foreach (var cod in codigos)
            {
                resultado = App.Prestamo.ExistsLibro(cod.Trim());
                if (!resultado.Status)
                {
                    lblStatus.Content = resultado.Mensaje;
                    txtCodLibro.Focus();
                    return false;
                }
                var librosDisponibles = App.Prestamo.LibroDisponible(cod.Trim());
                if (librosDisponibles < 1 || codigos.Count(t => t.Trim().Equals(cod.Trim())) > librosDisponibles)
                {
                    lblStatus.Content = "No hay suficientes libros";
                    txtCodLibro.Focus();
                    return false;
                }
            }
            lblStatus.Content = string.Empty;
            return true;
        }

        private void ExecutePrestamo()
        {
            var preload = App.Prestamo.PreloadPrestamo(int.Parse(txtNroFicha.Text));
            if (preload.Status)
            {
                var result = App.Prestamo.InsertPrestamo();

                if (result.Status)
                {
                    ExecuteDetallePrestamo();
                }
                else
                    lblStatus.Content = result.Mensaje;
            }
            else
                lblStatus.Content = preload.Mensaje;
        }

        private void ExecuteDetallePrestamo()
        {
            var preload = App.Prestamo.PreloadDetallePrestamo(int.Parse(txtNroFicha.Text), txtCodLibro.Text.Split(','));
            if (preload.Status)
            {
                var result = App.Prestamo.InsertDetallePrestamo();

                if (result.Status)
                {
                    foreach (var libro in txtCodLibro.Text.Split(','))
                    {
                        App.Prestamo.DescuentaLibro(libro.ToUpper().Trim());
                    }
                    new PanelAdmin(result).Show();
                    Close();
                }
                else
                    lblStatus.Content = result.Mensaje;
            }
            else
                lblStatus.Content = preload.Mensaje;
        }
    }
}
