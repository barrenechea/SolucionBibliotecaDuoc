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
            if (txtCodLibro.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtCodLibro.Focus();
                return false;
            }
            if (txtTitulo.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtTitulo.Focus();
                return false;
            }
            if (txtAutor.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtAutor.Focus();
                return false;
            }
            if (txtCategoria.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtCategoria.Focus();
                return false;
            }
            if (txtArgumento.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtArgumento.Focus();
                return false;
            }
            if (txtUbicacion.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtUbicacion.Focus();
                return false;
            }
            if (txtEditorial.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtEditorial.Focus();
                return false;
            }
            if (cmbTipo.SelectedItem == null)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                cmbTipo.Focus();
                return false;
            }
            if (txtNroPag.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNroPag.Focus();
                return false;
            }
            int i;
            if (int.TryParse(txtNroPag.Text, out i))
            {
                if (i <= 0)
                {
                    lblStatus.Content = "El número de paginas debe ser mayor a 0";
                    txtNroPag.Focus();
                    return false;
                }
            }
            else
            {
                lblStatus.Content = "Debe ingresar solo números";
                txtNroPag.Focus();
                return false;
            }
            if (txtNroCopias.Text == string.Empty)
            {
                lblStatus.Content = "Debe llenar todos los campos";
                txtNroCopias.Focus();
                return false;
            }
            if (int.TryParse(txtNroCopias.Text, out i))
            {
                if (_isAdd && i <= 0)
                {
                    lblStatus.Content = "El número de copias debe ser mayor a 0";
                    txtNroCopias.Focus();
                    return false;
                }
            }
            else
            {
                lblStatus.Content = "Debe ingresar solo números";
                txtNroCopias.Focus();
                return false;
            }
            lblStatus.Content = string.Empty;
            return true;
        }
        private void Execute()
        {
            if (App.Libros.TestConnection().Status)
            {
                var preload = App.Libros.PreloadLibro(txtCodLibro.Text.ToUpper(), txtTitulo.Text, txtAutor.Text, txtCategoria.Text, txtArgumento.Text, txtUbicacion.Text, txtEditorial.Text, (int)cmbTipo.SelectedValue, int.Parse(txtNroPag.Text), int.Parse(txtNroCopias.Text));

                if (preload.Status)
                {
                    var result = _isAdd ? App.Libros.Insert() : App.Libros.Update();

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
