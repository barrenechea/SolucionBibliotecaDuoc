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
        private DetallePrestamo _detallePrestamo;

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
                    if (App.Prestamo.LibrosPrestados(nroFicha) == 0)
                    {
                        new PanelAdmin(new Message(false, "El estudiante no tiene préstamo pendiente")).Show();
                        Close();
                        return;
                    }
                    _nroFicha = int.Parse(nroFicha);
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
            _detallePrestamo = App.Prestamo.InfoLibroPrestado(_nroFicha);
            lblNomEstudiante.Content = App.Users.PersonaPersistence.Nombre + " " + App.Users.PersonaPersistence.Apellido;
            lblDevolucion.Content = _detallePrestamo.FecDevolucion.ToString("dd-MM-yyyy");
            lblExtendido.Content = _detallePrestamo.Renovacion;
            lblNomLibro.Content = _detallePrestamo.CodLibro.ToUpper();
            if (_detallePrestamo.Renovacion >= 3 )
            {
                btnExtender.IsEnabled = false;
                lblStatus.Content = "No se puede extender un libro más de tres veces";
            }
            if (_detallePrestamo.FecDevolucion >= DateTime.Now) return;
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
            App.Prestamo.ExtenderPrestamo(codPrestamo.ToString(),_detallePrestamo.CodLibro,_detallePrestamo.FecDevolucion);
            LoadData();
        }
    }
}