using System.ComponentModel;
using System.Threading;
using System.Windows;
using Biblioteca.Entidad;
using Biblioteca.Entidad.Enum;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for PanelAdmin.xaml
    /// </summary>
    public partial class PanelAdmin
    {
        #region Attributes
        private readonly BackgroundWorker _checkConnection = new BackgroundWorker();
        private readonly Message _showMessage;
        private Message _connectionMessage;
        #endregion
        #region Constructors
        /// <summary>
        /// Generates a new instance of PanelAdmin
        /// </summary>
        public PanelAdmin()
        {
            InitializeComponent();
            SetLog();
            RearrangeFixWindow();
            _showMessage = null;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        /// <summary>
        /// Generates a new instance of PanelAdmin
        /// </summary>
        /// <param name="msg">Message to be shown when the Window has loaded</param>
        public PanelAdmin(Message msg)
        {
            InitializeComponent();
            SetLog();
            RearrangeFixWindow();
            _showMessage = msg;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        #endregion
        #region Custom Methods
        /// <summary>
        /// Fix the Window content, based on the TipoCuenta of the current logged Administrador
        /// </summary>
        private void RearrangeFixWindow()
        {
            // Hide unused tabs based on admin privileges
            TabBibliotecario.Visibility = TabJefe.Visibility = TabDirector.Visibility = Visibility.Hidden;
            lblNombre.Content = App.Admins.AdminActive.Nombre;
            lblApellido.Content = App.Admins.AdminActive.Apellido;

            switch (App.Admins.AdminActive.TipoDeUsuario)
            {
                case TipoUsuario.Bibliotecario:
                    SetAccent("Indigo");
                    TabJefe.IsEnabled = TabDirector.IsEnabled = false;
                    TabBibliotecario.IsSelected = true;
                    break;
                case TipoUsuario.Jefebiblioteca:
                    SetAccent("Emerald");
                    TabBibliotecario.IsEnabled = TabDirector.IsEnabled = false;
                    TabJefe.IsSelected = true;
                    break;
                case TipoUsuario.Director:
                    SetAccent("Red");
                    TabBibliotecario.IsEnabled = TabJefe.IsEnabled = false;
                    TabDirector.IsSelected = true;
                    break;
                default:
                    App.Admins.Logout();
                    new Inicio().Show();
                    Close();
                    break;
            }
        }
        /// <summary>
        /// Set a determinated accent to the window
        /// </summary>
        /// <param name="accent">Name of the accent (based on MahApps Accents)</param>
        private static void SetAccent(string accent)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var acc = ThemeManager.GetAccent(accent);
            ThemeManager.ChangeAppStyle(Application.Current, acc, theme.Item1);
        }
        /// <summary>
        /// What the BackgroundWorker does while it's running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Conn_DoWork(object sender, DoWorkEventArgs e)
        {
            var count = 0;
            while (true)
            {
                if (_connectionMessage != null)
                {
                    if (_connectionMessage.Status) break;
                    Thread.Sleep(5000);
                }
                _connectionMessage = App.Admins.TestConnection();
                count++;
                if (_connectionMessage.Status)
                {
                    _connectionMessage.Mensaje = string.Format("Estado: Conectado al servidor. Versión {0}",
                        App.Admins.FetchVersion());
                    break;
                }

                ((BackgroundWorker)sender).ReportProgress(count);
            }
        }
        /// <summary>
        /// What the BackgroundWorker updates on the Window when progress has changed
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Conn_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Content = string.Format("Estado: {0} (Intento {1})", _connectionMessage.Mensaje, e.ProgressPercentage);
        }
        /// <summary>
        /// What the BackgroundWorker does when has finished running the task
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void Conn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Content = _connectionMessage.Mensaje;

            BtnLogout.IsEnabled = true;

            if (App.Admins.AdminActive.TipoDeUsuario == TipoUsuario.Bibliotecario)
                TileBibliAdminUsers.IsEnabled =
                    TileBibliAdminLibros.IsEnabled =
                        TileBibliAdminPrestamos.IsEnabled = 
                            TileBibliVisualizarMorosidad.IsEnabled = true;
            else if (App.Admins.AdminActive.TipoDeUsuario == TipoUsuario.Jefebiblioteca)
                TileJefeAdminUsers.IsEnabled =
                    TileJefeAdminLibros.IsEnabled =
                        TileJefeAdminPrestamos.IsEnabled =
                            TileJefeVisualizarMorosidad.IsEnabled = 
                                TileJefeAdminAdministradores.IsEnabled = true;
            else
                TileDirectorAdminAdministradores.IsEnabled = TileDirectorLog.IsEnabled = true;

        }
        /// <summary>
        /// Set the current logged Username to all Controllers, in order to achieve Logging
        /// activities onto database
        /// </summary>
        private void SetLog()
        {
            App.Users.LogUsername = App.Admins.AdminActive.IdUsuario;
            App.Libros.LogUsername = App.Admins.AdminActive.IdUsuario;
            App.Morosidad.LogUsername = App.Admins.AdminActive.IdUsuario;
            App.Prestamo.LogUsername = App.Admins.AdminActive.IdUsuario;
            App.Log.LogUsername = App.Admins.AdminActive.IdUsuario;
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
        #region Bibliotecarios y Jefes
        /// <summary>
        /// Event that loads when user clicks on the Administrar Usuarios button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void BtnAdminUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Agregar",
                NegativeButtonText = "Buscar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowMessageAsync("¿Qué desea hacer?", "Seleccione \"Agregar\" para ingresar un nuevo usuario. Si desea modificar un usuario existente seleccione \"Buscar\"",
            MessageDialogStyle.AffirmativeAndNegative, settings);

            new UserManager(result == MessageDialogResult.Affirmative).Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Administrar Libros button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void BtnAdminLibros_Click(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Agregar",
                NegativeButtonText = "Buscar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowMessageAsync("¿Qué desea hacer?", "Seleccione \"Agregar\" para ingresar un nuevo libro. Si desea modificar un libro existente seleccione \"Buscar\"",
            MessageDialogStyle.AffirmativeAndNegative, settings);

            new LibroManager(result == MessageDialogResult.Affirmative).Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Administrar Prestamos button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void BtnAdminPrestamos_Click(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Nuevo préstamo",
                NegativeButtonText = "Extender préstamo",
                FirstAuxiliaryButtonText = "Devolución",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowMessageAsync("¿Qué desea hacer?", "Seleccione una de las opciones a continuación para continuar",
            MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);

            if (result == MessageDialogResult.Affirmative) new PrestamoAdd().Show();
            else new PrestamoManager(result == MessageDialogResult.Negative).Show();

            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Visualizar Hoja de Morosidad button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVisualizarMorosidad_Click(object sender, RoutedEventArgs e)
        {
            new HojaMorosidad().Show();
            Close();
        }
        #endregion
        #region JefeBiblioteca
        /// <summary>
        /// Event that loads when user clicks on the Administrar Bibliotecarios button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void BtnAdminBibliotecarios_Click(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Agregar",
                NegativeButtonText = "Buscar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowMessageAsync("¿Qué desea hacer?", "Seleccione \"Agregar\" para ingresar un nuevo bibliotecario. Si desea modificar un bibliotecario existente seleccione \"Buscar\"",
            MessageDialogStyle.AffirmativeAndNegative, settings);

            new AdminManager(result == MessageDialogResult.Affirmative).Show();
            Close();
        }
        #endregion
        #region Director
        /// <summary>
        /// Event that loads when user clicks on the Administrar Jefes de Biblioteca button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private async void BtnAdminJefes_Click(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
            {
                AffirmativeButtonText = "Agregar",
                NegativeButtonText = "Buscar",
                ColorScheme = MetroDialogOptions.ColorScheme
            };

            var result = await this.ShowMessageAsync("¿Qué desea hacer?", "Seleccione \"Agregar\" para ingresar un nuevo Jefe de Biblioteca. Si desea modificar un Jefe de Biblioteca existente seleccione \"Buscar\"",
            MessageDialogStyle.AffirmativeAndNegative, settings);

            new AdminManager(result == MessageDialogResult.Affirmative).Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Visualizar Log button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVisualizarLog_Click(object sender, RoutedEventArgs e)
        {
            new LogView().Show();
            Close();
        }
        #endregion

        #region General Events
        /// <summary>
        /// Event that loads when user clicks on the Logout button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
        /// <summary>
        /// Event that loads itself when the Window was loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void PanelAdmin_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_showMessage == null) return;

            if (_showMessage.Status)
            {
                lblMessage.Content = _showMessage.Mensaje;
                FlyoutMensaje.IsOpen = true;
            }
            else ShowNormalDialog("Información", _showMessage.Mensaje);
        }
        #endregion
        #endregion
    }
}