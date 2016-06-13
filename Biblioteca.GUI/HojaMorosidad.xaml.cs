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

        private void LoadData()
        {
            lblNroFicha.Content = _nroFicha;
        }


        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            SearchHojaMorosidadDialog();
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


    }
}
