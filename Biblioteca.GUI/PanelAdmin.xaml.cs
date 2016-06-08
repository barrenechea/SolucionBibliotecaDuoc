using System.ComponentModel;
using System.Threading;
using System.Windows;
using Biblioteca.Entidad;
using Biblioteca.Entidad.Enum;
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
        public PanelAdmin()
        {
            InitializeComponent();
            RearrangeFixWindow();
            _showMessage = null;
            _checkConnection.DoWork += Conn_DoWork;
            _checkConnection.WorkerReportsProgress = true;
            _checkConnection.ProgressChanged += Conn_ProgressChanged;
            _checkConnection.RunWorkerCompleted += Conn_RunWorkerCompleted;
            _checkConnection.RunWorkerAsync();
        }
        public PanelAdmin(Message msg)
        {
            InitializeComponent();
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
        private void RearrangeFixWindow()
        {
            // Hide unused tabs based on admin privileges
            TabBibliotecario.Visibility = TabJefe.Visibility = TabDirector.Visibility = Visibility.Hidden;
            lblNombre.Content = App.Admins.AdminActive.Nombre;
            lblApellido.Content = App.Admins.AdminActive.Apellido;

            switch (App.Admins.AdminActive.TipoDeUsuario)
            {
                case TipoUsuario.Bibliotecario:
                    TabJefe.IsEnabled = TabDirector.IsEnabled = false;
                    TabBibliotecario.IsSelected = true;
                    break;
                case TipoUsuario.Jefebiblioteca:
                    TabBibliotecario.IsEnabled = TabDirector.IsEnabled = false;
                    TabJefe.IsSelected = true;
                    break;
                case TipoUsuario.Director:
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
        private void Conn_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lblStatus.Content = string.Format("Estado: {0} (Intento {1})", _connectionMessage.Mensaje, e.ProgressPercentage);
        }
        private void Conn_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Content = _connectionMessage.Mensaje;
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
        private async void ShowNormalDialog(string title, string message)
        {
            await this.ShowMessageAsync(title, message);
        }
        #endregion
        #region Event Handlers
        #region Bibliotecarios y Jefes
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

            if (result == MessageDialogResult.Affirmative)
            {
                new Prestamo().Show();
                Close();
                //ToDo Nuevo préstamo
            }
            else if (result == MessageDialogResult.Negative)
            {
                new PrestamoExtender().Show();
                Close();
                //ToDo Extender préstamo
            }
            else
            {
                //ToDo Devolución
            }
        }
        private void BtnVisualizarMorosidad_Click(object sender, RoutedEventArgs e)
        {
            //ToDo Visualizar Hojas de Morosidad
        }
        #endregion
        #region JefeBiblioteca
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
        private void BtnVisualizarLog_Click(object sender, RoutedEventArgs e)
        {
            //ToDo Visualizar Log
        }
        #endregion

        #region General Events
        private void BtnLogout_OnClick(object sender, RoutedEventArgs e)
        {
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
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