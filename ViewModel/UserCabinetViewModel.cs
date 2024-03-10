using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Services;
using MultipleUserLoginForm.Stores;
using MultipleUserLoginForm.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MultipleUserLoginForm.ViewModel
{
    public class UserCabinetViewModel : ViewModelBase
    {
        private SubjectViewModel _account;
        private readonly MatricsStore _matricsStore;
        private readonly NavigationStore _navigationStore;

       
        public ICommand ReadCommand { get; }
        public ICommand WriteCommand { get; }
        public ICommand ExecuteCommand { get; }

        

        private SecurityRightViewModel _selectedItem;

        public SecurityRightViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }


        private void Read(object obj)
        {
            SelectedItem = (SecurityRightViewModel)obj;
            try
            {
                if (!File.Exists(SelectedItem.Object.Path))
                {
                    MessageBox.Show($"{LocalizedStrings.Instance["mesBoxCannotFindFileUserCabinet"]} {SelectedItem.Object.Path}",
                        LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                using (var reader = File.OpenText(SelectedItem.Object.Path))
                {
                    string s = reader.ReadToEnd();
                    NavigateCommand<ReadWriteObjectViewModel> navigateCommand = new NavigateCommand<ReadWriteObjectViewModel>(
                        new NavigationService<ReadWriteObjectViewModel>(_navigationStore, () => new ReadWriteObjectViewModel(s, true,
                        SelectedItem.Object, SelectedItem.Subject, _navigationStore, _matricsStore)));
                    navigateCommand.Execute(this);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(LocalizedStrings.Instance["mesBoxCannotReadUserCabinet"], LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Write(object obj)
        {
            SelectedItem = (SecurityRightViewModel)obj;

            if (SelectedItem.Right == Right.All)
            {
                try
                {
                    if (!File.Exists(SelectedItem.Object.Path))
                    {
                        MessageBox.Show($"{LocalizedStrings.Instance["mesBoxCannotFindFileUserCabinet"]} {SelectedItem.Object.Path}",
                            LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var reader = File.OpenText(SelectedItem.Object.Path))
                    {
                        string s = reader.ReadToEnd();
                        NavigateCommand<ReadWriteObjectViewModel> navigateCommand = new NavigateCommand<ReadWriteObjectViewModel>(
                            new NavigationService<ReadWriteObjectViewModel>(_navigationStore, () => new ReadWriteObjectViewModel(s, false, 
                            SelectedItem.Object, SelectedItem.Subject, _navigationStore, _matricsStore)));
                        navigateCommand.Execute(this);
                    }
                    return;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(LocalizedStrings.Instance["mesBoxCannotWriteUserCabinet"], LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"], 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBoxResult res = MessageBox.Show(LocalizedStrings.Instance["mesBoxRewriteUserCabinet"],
                    SelectedItem.Object.Name, MessageBoxButton.YesNo,MessageBoxImage.Question);
                if (res == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    if (!File.Exists(SelectedItem.Object.Path))
                    {
                        MessageBox.Show($"Can't find this file:{SelectedItem.Object.Path}", LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    using (var reader = File.Create(SelectedItem.Object.Path))
                    {

                        NavigateCommand<ReadWriteObjectViewModel> navigateCommand = new NavigateCommand<ReadWriteObjectViewModel>(
                            new NavigationService<ReadWriteObjectViewModel>(_navigationStore, () => new ReadWriteObjectViewModel("", false,
                            SelectedItem.Object, SelectedItem.Subject, _navigationStore, _matricsStore)));
                        navigateCommand.Execute(this);
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show(LocalizedStrings.Instance["mesBoxCannotReadUserCabinet"], LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void Execute(object obj)
        {
            SelectedItem = (SecurityRightViewModel)obj;

            try
            {
                if (!File.Exists(SelectedItem.Object.Path))
                {
                    MessageBox.Show($"{LocalizedStrings.Instance["mesBoxCannotFindFileUserCabinet"]} {SelectedItem.Object.Path}",
                        LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = SelectedItem.Object.Path;
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LocalizedStrings.Instance["mesBoxCannotExecuteUserCabinet"], LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand NavigateLoginCommand { get; }

        public SubjectViewModel Account { get { return _account; } }

        public  List<SecurityRightViewModel> AvailableObjects { get {
                var list = (from el in _matricsStore.CurrentMatrics.GetObjectsForSubj(_account.Login) where (el.IsWritable||
                           el.IsReadable||el.IsExecutable )==true select el).ToList();
                foreach (var s in list) {
                    s.ReadCommand = ReadCommand;
                    s.WriteCommand = WriteCommand;
                    s.ExecuteCommand = ExecuteCommand;
                }
                return list;
            }
                
                
            
            } 

        public UserCabinetViewModel(SubjectViewModel account,NavigationStore ns, MatricsStore ms)
        {
            _matricsStore = ms;
            _navigationStore = ns;
            _account = account;
            NavigateLoginCommand = new NavigateCommand<LoginViewModel>(new NavigationService<LoginViewModel>(ns,()=>new LoginViewModel(ns,ms)));

            ReadCommand = new RelayCommand(Read);
            WriteCommand = new RelayCommand(Write);
            ExecuteCommand = new RelayCommand(Execute);
        }
    }
}
