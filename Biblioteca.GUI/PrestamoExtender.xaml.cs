using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Biblioteca.Entidad;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for PrestamoExtender.xaml
    /// </summary>
    public partial class PrestamoExtender : MetroWindow
    {
        private string _nroFicha;
        public PrestamoExtender()
        {
            InitializeComponent();
        }

        private async void SearchPrestamoDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var nroFicha = await this.ShowInputAsync("Buscar", "Ingrese el número de ficha del usuario", settings);

            if (string.IsNullOrWhiteSpace(nroFicha))
            {
                new PanelAdmin().Show();
                Close();
                return;
            }

            var exists = App.Users.FetchUsuario(nroFicha);
            if (exists.Status)
            {
                if (App.Users.IsStudent(int.Parse(nroFicha)))
                {
                    _nroFicha = nroFicha;
                    LoadData();
                }
                else
                {
                    new PanelAdmin(new Message(false, "Sólo los estudiantes pueden extender un préstamo")).Show();
                    Close();
                }
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }

        private void LoadData()
        {
            //toDo llenar los labels
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Login.Logout();
            new Inicio().Show();
            Close();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Close();
        }
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            SearchPrestamoDialog();
        }

        private void BtnExtender_Click(object sender, RoutedEventArgs e)
        {
            //toDo extender el prestamo según tipo de libro y sumar 1 a la cosa de extender.
        }

    }
}
