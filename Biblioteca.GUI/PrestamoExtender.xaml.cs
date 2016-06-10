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
    public partial class PrestamoExtender
    {
        private int _nroFicha;
        
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
                exists = App.Users.IsStudent(int.Parse(nroFicha));
                if (exists.Status)
                {
                    exists = App.Prestamo.FetchPrestamo(nroFicha);
                    if (!exists.Status)
                    {
                        new PanelAdmin(exists).Show();
                        Close();
                        return;
                    }
                    _nroFicha = int.Parse(nroFicha);
                    LoadData();
                }
                else
                {
                    new PanelAdmin(exists).Show();
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
            
            lblNomEstudiante.Content = App.Users.PersonaPersistence.Nombre + " " + App.Users.PersonaPersistence.Apellido;
            lblDevolucion.Content = App.Prestamo.DetPrestamoPersistence.FecDevolucion.ToString("dd-MM-yyyy");
            lblExtendido.Content = App.Prestamo.DetPrestamoPersistence.Renovacion;
            lblNomLibro.Content = App.Prestamo.DetPrestamoPersistence.CodLibro.ToUpper();
            
            if (App.Prestamo.DetPrestamoPersistence.Renovacion >= 3 )
            {
                btnExtender.IsEnabled = false;
                lblStatus.Content = "No se puede extender un libro más de tres veces";
            }
            if (App.Prestamo.DetPrestamoPersistence.FecDevolucion >= DateTime.Now) return;
            btnExtender.IsEnabled = false;
            lblStatus.Content = "El alumno tiene morosidad, no puede extender el libro";
            
        }

        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            App.Prestamo.ClearPersistantData();
            App.Users.ClearPersistantData();
            new PanelAdmin().Show();
            Close();
        }

        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            SearchPrestamoDialog();
        }

        private void BtnExtender_Click(object sender, RoutedEventArgs e)
        {
            var codPrestamo = App.Prestamo.CodigoPrestamo(_nroFicha);
            App.Prestamo.ExtenderPrestamo(codPrestamo.ToString(), App.Prestamo.DetPrestamoPersistence.CodLibro, App.Prestamo.DetPrestamoPersistence.FecDevolucion);
            LoadData();
        }
    }
}