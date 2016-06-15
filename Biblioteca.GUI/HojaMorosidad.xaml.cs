using System.Windows;
using Biblioteca.Entidad;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for HojaMorosidad.xaml
    /// </summary>
    public partial class HojaMorosidad
    {
        #region Attributes
        private int _nroFicha;
        private bool _vista;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new HojaMorosidad Window
        /// </summary>
        public HojaMorosidad()
        {
            InitializeComponent();
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Opens a Search Dialog, in order to get a Numero de Ficha related to an Estudiante
        /// </summary>
        private async void SearchHojaMorosidadDialog()
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Buscar",
                NegativeButtonText = "Cancelar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var nroFicha = await this.ShowInputAsync("Buscar", "Ingrese el número de ficha del estudiante", settings);

            var check = App.Users.CheckEmptySearchString(nroFicha);
            if (!check.Status)
            {
                if (check.Mensaje == null)
                {
                    new PanelAdmin().Show();
                    Close();
                }
                else
                {
                    new PanelAdmin(check).Show();
                    Close();
                }
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
        /// <summary>
        /// Logic that follows after fetching a Numero de Ficha
        /// </summary>
        private void WindowLogic()
        {
            var test = App.Morosidad.TestConnection();
            if (test.Status)
            {
                var isStudent = App.Users.IsStudent(_nroFicha);
                if (!isStudent.Status)
                {
                    new PanelAdmin(isStudent).Show();
                    Close();
                }
                else
                {
                    var fetch = App.Morosidad.FetchHojaMorosidad(_nroFicha);
                    if (fetch.Status)
                    {
                        LoadData();
                    }
                    else
                    {
                        new PanelAdmin(fetch).Show();
                        Close();
                    }
                }
            }
            else
            {
                new PanelAdmin(test).Show();
                Close();
            }
        }
        /// <summary>
        /// Load data inside Window fields
        /// </summary>
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
        /// <summary>
        /// Logic related to Ver Detalle button (on press)
        /// </summary>
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
        /// <summary>
        /// Shows just an alert inside the Window
        /// </summary>
        /// <param name="title">Title of the alert</param>
        /// <param name="message">Message of the alert</param>
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event that triggers automatically when window is initilized
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            var test = App.Morosidad.TestConnection();

            if (test.Status) SearchHojaMorosidadDialog();
            else
            {
                new PanelAdmin(test).Show();
                Close();
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
        /// <summary>
        /// Event that loads when user clicks on the Ver Detalle button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void btnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            BtnLogic();
        }
        #endregion
    }
}