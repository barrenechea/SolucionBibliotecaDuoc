using System.Collections.Generic;
using System.Windows;
using Biblioteca.Entidad;

namespace Biblioteca.GUI
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView
    {
        #region Attribute
        private readonly List<Log> _listLog;
        #endregion
        #region Constructor
        /// <summary>
        /// Generates a new instance of LogView
        /// </summary>
        public LogView()
        {
            InitializeComponent();
            _listLog = new List<Log>();
            grdLog.ItemsSource = _listLog;
        }
        #endregion
        #region Event Handlers
        /// <summary>
        /// Event that's triggered automatically when the Window has successfully loaded
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void WindowHasLoaded(object sender, RoutedEventArgs e)
        {
            grdLog.Columns[1].Header = "ADMINISTRADOR";
            var test = App.Log.TestConnection();

            if (test.Status)
            {
                var list = App.Log.FetchAll();
                foreach (var log in list)
                {
                    _listLog.Add(log);
                }
                grdLog.Items.Refresh();
            }
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
            App.Admins.Logout();
            new Inicio().Show();
            Close();
        }
        /// <summary>
        /// Event that loads when user clicks on the Back button
        /// </summary>
        /// <param name="sender">The object that triggered this event</param>
        /// <param name="e">Parameters (optional)</param>
        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            new PanelAdmin().Show();
            Close();
        }
        #endregion
    }
}