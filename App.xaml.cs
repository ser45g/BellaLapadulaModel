using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Stores;
using MultipleUserLoginForm.Utilities;
using MultipleUserLoginForm.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;
using WPFLocalizeExtension.Engine;

namespace MultipleUserLoginForm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MatricesViewModel ms;
        protected override void OnStartup(StartupEventArgs e)
        {
            //LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            //LocalizeDictionary.Instance.SetCultureCommand.Execute(MultipleUserLoginForm.Properties.Settings.Default.CurrentCulture);
            LocalizedStrings.Instance.SetCulture(MultipleUserLoginForm.Properties.Settings.Default.CurrentCulture);

            NavigationStore store = new NavigationStore();
             ms = new MatricesViewModel();
            if(ms.Admins.Count < 1)
            {

                ms.Admins.Add(new SubjectViewModel(new Subject() { Login = "admin", Password = "admin",SecurityMark= SecurityMark.TopSecret }));
            }
            MatricsStore matricsStore = new MatricsStore() { CurrentMatrics = ms };
            store.CurrentViewModel = new LoginViewModel(store,matricsStore);
            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel(store) };
            MainWindow.Show();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            ms.Dispose();
            MultipleUserLoginForm.Properties.Settings.Default.Save();
            base.OnExit(e);
        }
    }

}
