using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Stores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultipleUserLoginForm.ViewModel
{
    public class ReadWriteObjectViewModel: ViewModelBase,IDisposable
    {
        private string _text;
        private bool disposedValue;
        public ObjectViewModel Object { get; }

        public string Text
        {
            get { return _text; }
            set { 
                
                _text = value;
                OnPropertyChanged(nameof(Text));
                
            }
        }
        public bool IsReadonly { get; }

        public ReadWriteObjectViewModel(string text, bool isReadonly, ObjectViewModel obj,SubjectViewModel  subj,NavigationStore ns,
            MatricsStore ms)
        {
            Object = obj;
            IsReadonly = isReadonly;
            Text = text;
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
                    using (StreamWriter writer = File.CreateText(Object.Path))
                    {
                        writer.Write(Text);
                    }



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
