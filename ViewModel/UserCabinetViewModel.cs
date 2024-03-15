using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Services;
using MultipleUserLoginForm.Stores;
using MultipleUserLoginForm.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

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


        private  void Read(object obj)
        {
            SelectedItem = (SecurityRightViewModel)obj;
            bool isSupported = SupportedExtensions(SelectedItem.Object.Path);
            if (!isSupported)
            {
                //MessageBox.Show(LocalizedStrings.Instance["mesThisTypeOfFileINotSupportedUserCabinet"],
                //    LocalizedStrings.Instance["mesError"],MessageBoxButton.OK,MessageBoxImage.Stop);
                Execute(obj);
                return;
            }
            try
            {
                if (!File.Exists(SelectedItem.Object.Path))
                {
                    MessageBox.Show($"{LocalizedStrings.Instance["mesBoxCannotFindFileUserCabinet"]} {SelectedItem.Object.Path}",
                        LocalizedStrings.Instance["mesBoxTitleErrorUserCabinet"],
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (OpenImage(SelectedItem.Object.Path))
                    return;
                OpenTextFile(SelectedItem.Object.Path);

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
            
            bool isSupported=SupportedExtensions(SelectedItem.Object.Path);
            if (isSupported==false)
            {
                // MessageBox.Show(LocalizedStrings.Instance["mesThisTypeOfFileINotSupportedUserCabinet"],
                //      LocalizedStrings.Instance["msgError"], MessageBoxButton.OK, MessageBoxImage.Stop);
                Execute(obj);
                return;
            }
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
                    if (OpenImage(SelectedItem.Object.Path))
                        return;
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
                    if (OpenImage(SelectedItem.Object.Path))
                        return;
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

        private bool SupportedExtensions(string fullPath)
        {
            string ext=Path.GetExtension(fullPath);
            if (ext == ".pdf" || ext == ".doc"||ext==".docx"||ext==".xlsx"||ext==".rtf"||ext==".mp3"||ext==".mp4a"
                ||ext=="mp4")
                return false;
            return true;
        }

        private bool OpenImage(string fullPath)
        {
            string ext = Path.GetExtension(fullPath);
            if ((ext == ".png" )||( ext == ".jpg") || (ext == ".bmp")|| (ext == ".ico") || (ext == ".jpeg"))
            {
                BitmapImage bmp = new BitmapImage(new Uri(fullPath, UriKind.Absolute));
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                NavigateCommand<OpenImageViewmodel> navigateCommand = new NavigateCommand<OpenImageViewmodel>(
                      new NavigationService<OpenImageViewmodel>(_navigationStore, () => new OpenImageViewmodel(bmp,
                      SelectedItem.Object, SelectedItem.Subject, _navigationStore, _matricsStore)));
                navigateCommand.Execute(this);
                return true;
            }
            return false;
        }

        private bool OpenTextFile(string fullPath)
        {
            try
            {
                using (var reader = File.OpenText(fullPath))
                {
                    //string s = await reader.ReadToEndAsync();
                    string s = reader.ReadToEnd();
                    NavigateCommand<ReadWriteObjectViewModel> navigateCommand = new NavigateCommand<ReadWriteObjectViewModel>(
                        new NavigationService<ReadWriteObjectViewModel>(_navigationStore, () => new ReadWriteObjectViewModel(s, true,
                        SelectedItem.Object, SelectedItem.Subject, _navigationStore, _matricsStore)));
                    navigateCommand.Execute(this);
                }

            }catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public ICollectionView FilteredView { get; set; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText;}
            set { _searchText = value;
                
                FilteredView.Filter = DoescollectionContainName;
                OnPropertyChanged(nameof(SearchText));
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
        private bool DoescollectionContainName(object obj)
        {
            SecurityRightViewModel s = obj as SecurityRightViewModel;
            return s.Object.Name.ToLower().Contains(SearchText.ToLower());
        }
        public UserCabinetViewModel(SubjectViewModel account,NavigationStore ns, MatricsStore ms)
        {
            _matricsStore = ms;
            _navigationStore = ns;
            _account = account;
            NavigateLoginCommand = new NavigateCommand<LoginViewModel>(new NavigationService<LoginViewModel>(ns,()=>new LoginViewModel(ns,ms)));

            TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleUserCabinet"]} - " +
                LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"];

            ReadCommand = new RelayCommand(Read);
            WriteCommand = new RelayCommand(Write);
            ExecuteCommand = new RelayCommand(Execute);

            FilteredView = new CollectionViewSource { Source = AvailableObjects }.View;
        }
    }
}
