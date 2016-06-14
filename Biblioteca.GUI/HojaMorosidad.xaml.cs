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
    /// Interaction logic for HojaMorosidad.xaml
    /// </summary>
    public partial class HojaMorosidad : MetroWindow
    {
        private int _nroFicha;
        private bool _isStudent;
        private bool _vista = false;
        public HojaMorosidad()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens a Search Dialog, in order to get a Numero de Ficha related to an Usuario
        /// </summary>
        private async void SearchHojaMorosidadDialog()
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
                _nroFicha = int.Parse(nroFicha);
                WindowLogic();
            }
            else
            {
                new PanelAdmin(exists).Show();
                Close();
            }
        }

        private void WindowLogic()
        {
            var test = App.Morosidad.TestConnection();
            if (test.Status)
            {
                var fetch = App.Morosidad.FetchHojaMorosidad(_nroFicha);
                if (fetch.Status)
                {
                    fetch = App.Users.IsStudent(_nroFicha);
                    _isStudent = fetch.Status;
                    if (_isStudent)
                    {
                        LoadData();
                        return;
                    }
                }

                new PanelAdmin(fetch).Show();
                //Clear();
                Close();
            }
            else
            {
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
                Close();
            }
        }

        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }

        private void LoadData()
        {
            if (App.Morosidad.TestConnection().Status)
            {
                lblNroFicha.Content = _nroFicha;
                lblNombre.Content = string.Format("{0} {1}", App.Users.PersonaPersistence.Nombre, App.Users.PersonaPersistence.Apellido);
                lblRun.Content = App.Users.PersonaPersistence.Run;
                lblCurso.Content = ((Estudiante)App.Users.PersonaPersistence).Curso;
                lblFonoFijo.Content = App.Users.PersonaPersistence.FonoFijo;
                lblFonoCel.Content = App.Users.PersonaPersistence.FonoCel;
                lblApoderado.Content = string.Format("{0} {1}", App.Users.ApoderadoPersistence.Nombre, App.Users.ApoderadoPersistence.Apellido);
                lblFonoCelAp.Content = App.Users.ApoderadoPersistence.FonoCel;
                lblFonoFijoAp.Content = App.Users.ApoderadoPersistence.FonoFijo;
                lblParentesco.Content = App.Users.Parentesco;
                lstMorosidad.ItemsSource = App.Morosidad.HojaMorosidadPersistence;    
            }
            else
            {
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
                Close();
            }
        }

        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            if (App.Morosidad.TestConnection().Status)
            {
                SearchHojaMorosidadDialog();
            }
            else
            {
                ShowNormalDialog("Error", "Se ha perdido la conexión con el servidor. Intente nuevamente más tarde");
            }
        }
        /// <summary>
        /// Event that loads when user clicks on the Logout button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Users.ClearPersistantData();
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }

        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            App.Users.ClearPersistantData();
            new PanelAdmin().Show();
            Close();
        }

        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            BtnLogic();
        }

        private void BtnLogic()
        {
            if (!_vista)
            {
                grdDetalle.Visibility = Visibility.Visible;
                btnVerDetalle.Content = "Ocultar detalle";
                _vista = true;
            }
            else
            {
                grdDetalle.Visibility = Visibility.Hidden;
                btnVerDetalle.Content = "Ver detalle del estudiante";
                _vista = false;
            }
        }


    }
}
