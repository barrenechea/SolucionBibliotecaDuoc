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
        private readonly bool _isAdd;

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
            App.Login.Logout();
            new Inicio().Show();
            Close();
        }

        private void BtnExecute_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation()) return;
            Execute();
        }

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
            if (App.Users.IsStudent(txtNroFicha.Text))
            {
                if (txtCodLibro.Text.Split(',').Length > 1)
                {
                    lblStatus.Content = "Los estudiantes no pueden solicitar más de un libro";
                    txtCodLibro.Focus();
                    return false;
                }
            }
            else
            {
                if (txtCodLibro.Text.Split(',').Length > 5)
                {
                    lblStatus.Content = "Los funcionarios no pueden solicitar más de cinco libros";
                    txtCodLibro.Focus();
                    return false;
                }
            }
            //toDo quitar espacios en blanco a los codigos de libro.
            lblStatus.Content = string.Empty;
            return true;
        }

        private void Execute()
        {
            if (App.Libros.TestConnection().Status)
            {
                var preload = App.Prestamo.PreloadPrestamo(DateTime.Now, int.Parse(txtNroFicha.Text.ToUpper()));

                if (preload.Status)
                {
                    var result = _isAdd ? App.Prestamo.Insert() : App.Libros.Update();

                    if (result.Status)
                    {
                        new PanelAdmin(result).Show();
                        Close();
                    }
                    else
                        lblStatus.Content = result.Mensaje;
                }
                else
                    lblStatus.Content = preload.Mensaje;
            }
            else
            {
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
            }
        }

        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
    }
}
