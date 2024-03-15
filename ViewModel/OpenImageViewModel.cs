using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MultipleUserLoginForm.ViewModel
{
    public class OpenImageViewmodel : ViewModelBase, IDisposable
    {
       
        private bool disposedValue;
        public ObjectViewModel Object { get; }


        private BitmapImage _bmp;

        public BitmapImage ImageSource
        {
            get { return _bmp; }
            set { _bmp = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }


        public OpenImageViewmodel(BitmapImage bmp, ObjectViewModel obj, SubjectViewModel subj, NavigationStore ns,
            MatricsStore ms)
        {
            Object = obj;
            ImageSource = bmp;
            NavigateCommand<UserCabinetViewModel> navCom = new NavigateCommand<UserCabinetViewModel>(new Services.NavigationService<UserCabinetViewModel>(ns,
                () => new UserCabinetViewModel(subj, ns, ms)));
            TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleReadWrite"]} {subj.Login} - " +
                LocalizedStrings.Instance[$"modelType{ms.CurrentMatrics.CurrentModelType.ToString()}"];

            RelayCommand relayCommand = new RelayCommand((o) =>
            {
                navCom.Execute(o);
                Dispose();
            });
            NavigateLoginCommand = relayCommand;


        }

        public ICommand NavigateLoginCommand { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                  
                    


                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ReadWriteObjectViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
