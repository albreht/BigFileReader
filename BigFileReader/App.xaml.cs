using System.IO;
using System.Windows;

namespace BigFileReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Count() == 0)
                throw new ArgumentException("Missing startup argument!");

            var filePath = Path.GetFullPath(e.Args[0]);

            var mainWindow = new MainWindow(filePath);
            mainWindow.Show();
        }
    }
}