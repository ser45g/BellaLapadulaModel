using Microsoft.Win32;
using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Services;
using MultipleUserLoginForm.Stores;
using MultipleUserLoginForm.Utilities;
using MultipleUserLoginForm.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using WPFLocalizeExtension.Engine;
using System.Resources;
using MultipleUserLoginForm.Properties;
using MultipleUserLoginForm.LocalizationHelper;

namespace MultipleUserLoginForm.ViewModel
{
    public class AdminViewModel:ViewModelBase, INotifyDataErrorInfo
    {
        private readonly MatricsStore _matricsStore;
        private OpenFileDialog _openFileDialog=new OpenFileDialog();

        public AdminViewModel(NavigationStore ns, MatricsStore ms) {

            NavigateLoginCommand = new NavigateCommand<LoginViewModel>(
				new NavigationService<LoginViewModel>(ns, () => new LoginViewModel(ns,_matricsStore)));

			_matricsStore= ms;

            subjects = _matricsStore.CurrentMatrics.Subjects;
            objects = _matricsStore.CurrentMatrics.Objects;

            _log = _matricsStore.CurrentMatrics.Log;

            Objects.CollectionChanged += ObjectsChanged ;
            Subjects.CollectionChanged += SubjectsChanged;


            AddObjectCommand = new RelayCommand((obj) => AddObject(obj), (obj) => CanCreateObj); ;
			RemoveObjectCommand=new RelayCommand((obj) => RemoveObject(obj));
			ClearObjectCommand=new RelayCommand((obj) => ClearObjects(obj));
            ChangeObjectCommand = new RelayCommand((obj) => ChangeObject(obj));			
            
			AddSubjectCommand=new RelayCommand((obj) => AddSubject(obj),(obj)=>CanCreate);
			RemoveSubjectCommand=new RelayCommand((obj)=> RemoveSubject(obj));
			ClearSubjectCommand=new RelayCommand((obj) => ClearSubjects(obj));
			ChangeSubjectCommand = new RelayCommand((obj) => ChangeSubject(obj));

            SetSelectedItemCommand = new RelayCommand((obj) => { TreeViewSelectedItem = obj; });

            BrowseObjectPathCommand = new RelayCommand((obj) =>
            {
                if (_openFileDialog.ShowDialog() == true)
                {
                    ObjectPath = _openFileDialog.FileName;
                }
            });

            _errorsViewModel = new ErrorsViewModel();
            _errorsObjectViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_Errorschanged;
            _errorsObjectViewModel.ErrorsChanged += ErrorsObjectViewModel_Errorschanged;
        }

        #region Object Properties: Set of Objects
        private ObjectViewModel _selectedObject;
		public ObjectViewModel SelectedObject
		{
			get { return _selectedObject; }
			set
			{
				_selectedObject = value;
				if(value != null)
				{
					ObjectName= _selectedObject.Name;
					ObjectPath=_selectedObject.Path;
					ObjectSecurityMark= _selectedObject.SecurityMark;
				}
				OnPropertyChanged(nameof(SelectedObject));
			}
		}

		private string _objectName;
		public string ObjectName { get { return _objectName; } set {
                _errorsObjectViewModel.ClearErrors(nameof(ObjectName));
				_objectName = value;
				OnPropertyChanged(nameof(ObjectName));
			} }

        private string _objectPath;
        public string ObjectPath { get { return _objectPath; } set { 
                _errorsObjectViewModel.ClearErrors(nameof(ObjectPath));
                
                _objectPath = value;
				OnPropertyChanged(nameof(ObjectPath));
			} }

		private SecurityMark _objectSecurityMark;
		public SecurityMark ObjectSecurityMark { get { return _objectSecurityMark; } set {
				_objectSecurityMark = value;
				OnPropertyChanged(nameof(ObjectSecurityMark));
			} 
        }

        private void AddObject(object obj)
        {
            ObjectViewModel o = new ObjectViewModel(new Model.Object() { Path = ObjectPath, Name = ObjectName, SecurityMark = ObjectSecurityMark });

            ValidateStringIsNullOrEmpty(o.Name, nameof(ObjectName), _errorsObjectViewModel);
            ValidateStringIsNullOrEmpty(o.Path, nameof(ObjectPath), _errorsObjectViewModel);
            if (CanCreateObj)
            {
                LogObject(CommandAction.Add, o);
                Objects.Add(o);
            }
        }

        private void RemoveObject(object obj)
        { 
            LogObject(CommandAction.Remove, SelectedObject); 
            Objects.Remove(SelectedObject);
        }

        private void ChangeObject(object obj)
        {
            ObjectViewModel o = new ObjectViewModel(new Model.Object() { Path = ObjectPath, Name = ObjectName, SecurityMark = ObjectSecurityMark });

            ValidateStringIsNullOrEmpty(o.Name, nameof(ObjectName), _errorsObjectViewModel);
            ValidateStringIsNullOrEmpty(o.Path, nameof(ObjectPath), _errorsObjectViewModel);
            ObjectViewModel old = Objects.FirstOrDefault((obj) => { return obj.Name == ObjectName; });
            if (CanCreateObj)
            {
                LogObject(CommandAction.Change, o,old);
                Objects.Remove(old);
                Objects.Add(o);
            }
        }

        private void ClearObjects(object obj)
        {
            LogObject(CommandAction.Clear, null);
            Objects.Clear(); 
        }
        #endregion

        #region Subject Properties: Set of Subjects

        private SubjectViewModel _selectedSubject;
        public SubjectViewModel SelectedSubject
        {
            get { return _selectedSubject; }
            set
            {
                _selectedSubject = value;
                if (value != null)
                {
                    SubjectLogin = _selectedSubject.Login;
                    SubjectPassword = _selectedSubject.Password;
                    SubjectSecurityMark = _selectedSubject.SecurityMark;
                    SubjectName = _selectedSubject.Name;
                    SubjectSecondName = _selectedSubject.SecondName;

                }
                OnPropertyChanged(nameof(SelectedSubject));

            }
        }

        private string _subjectLogin;
        public string SubjectLogin
        {
            get { return _subjectLogin; }
            set
            {
                _errorsViewModel.ClearErrors(nameof(SubjectLogin));
                _subjectLogin = value;
                OnPropertyChanged(nameof(SubjectLogin));
            }
        }
        private string _subjectName;
        public string SubjectName
        {
            get { return _subjectName; }
            set
            {
                _subjectName = value;
                OnPropertyChanged(nameof(SubjectName));
            }
        }
        private string _subjectSecondName;
        public string SubjectSecondName
        {
            get { return _subjectSecondName; }
            set
            {
                _subjectSecondName = value;
                OnPropertyChanged(nameof(SubjectSecondName));
            }
        }
        private string _subjectPassword;
        public string SubjectPassword
        {
            get { return _subjectPassword; }
            set
            {
                _errorsViewModel.ClearErrors(nameof(SubjectPassword));
                _subjectPassword = value;
                OnPropertyChanged(nameof(SubjectPassword));
            }
        }
        private SecurityMark _subjectSecurityMark;
        public SecurityMark SubjectSecurityMark
        {
            get { return _subjectSecurityMark; }
            set
            {
                _subjectSecurityMark = value;
                OnPropertyChanged(nameof(SubjectSecurityMark));
            }
        }

        private void AddSubject(object obj)
        {
            SubjectViewModel s = new SubjectViewModel(new Subject()
            {
                Login = SubjectLogin,
                Password = SubjectPassword,
                SecurityMark = SubjectSecurityMark,
                Name = SubjectName,
                SecondName = SubjectSecondName
            });
            ValidateSubjectLogin(s.Login);
            ValidateStringIsNullOrEmpty(s.Password, nameof(SubjectPassword), _errorsViewModel);
            if (CanCreate)
            {
                LogSubject(CommandAction.Add, s);
                Subjects.Add(s);
            }
        }

        private void ChangeSubject(object obj)
        {

            //_errorsViewModel.ClearErrors(nameof(SubjectLogin));
            SubjectViewModel s = new SubjectViewModel(new Subject()
            {
                Login = SubjectLogin,
                Password = SubjectPassword,
                SecurityMark = SubjectSecurityMark,
                Name = SubjectName,
                SecondName = SubjectSecondName }
            );
            ValidateStringIsNullOrEmpty(s.Login, nameof(SubjectLogin), _errorsViewModel);
            ValidateStringIsNullOrEmpty(s.Password, nameof(SubjectPassword), _errorsViewModel);
            SubjectViewModel old = Subjects.FirstOrDefault((obj) => { return obj.Login == SubjectLogin; });
            if (CanCreate)
            {
                LogSubject(CommandAction.Change, s,old);
                Subjects.Remove(old);
                Subjects.Add(s);

            }
        }

        private void RemoveSubject(object obj)
        {
            LogSubject(CommandAction.Remove, SelectedSubject); 
            Subjects.Remove(SelectedSubject); 
        }
        
        private void ClearSubjects(object obj)
        { 
            LogSubject(CommandAction.Clear, null);
            Subjects.Clear();
        }
        #endregion



        private ObservableCollection<SubjectViewModel> subjects ;
		public ObservableCollection<SubjectViewModel> Subjects {  get {
				return subjects;
            } }
		private ObservableCollection<ObjectViewModel> objects ;
		public ObservableCollection<ObjectViewModel> Objects {  get { return 
					objects; } }

		public ICommand AddObjectCommand { get; }
		public ICommand ChangeObjectCommand { get; }
		public ICommand RemoveObjectCommand { get; }
		public ICommand ClearObjectCommand { get; }

		public ICommand AddSubjectCommand { get; }
		public ICommand ChangeSubjectCommand { get; }
		public ICommand RemoveSubjectCommand { get; }
		public ICommand ClearSubjectCommand { get; }

		public ICommand NavigateLoginCommand { get; }

        public ICommand BrowseObjectPathCommand {  get; }

      
        #region Log
        private ObservableCollection<string> _log;
        public ObservableCollection<string> Log
        {
            get { return _log; }
            set
            {
                _log = value;
                OnPropertyChanged(nameof(Log));
            }
        }

        private enum CommandAction { Add, Remove,Change,Clear }

        void LogObject(CommandAction comAct, ObjectViewModel obj,ObjectViewModel old=null)
        {
            string str = "";
            switch (comAct) {
                case CommandAction.Add:
                    str=$"{LocalizedStrings.Instance["logTheObjectAdmin"]} " +
                        $"{obj?.Name} {LocalizedStrings.Instance["logWithThePathAdmin"]} {obj?.Path} " +
                        $"{LocalizedStrings.Instance["logWasAddedAdmin"]} ";
                    break;
                case CommandAction.Remove:
                    str=$"{LocalizedStrings.Instance["logTheObjectAdmin"]} " +
                        $"{obj?.Name} {LocalizedStrings.Instance["logWithThePathAdmin"]} {obj?.Path} " +
                        $"{LocalizedStrings.Instance["logWasRemovedAdmin"]}";

                    break;
                case CommandAction.Change:
                    str=$"{LocalizedStrings.Instance["logTheObjectAdmin"]} " +
                        $"{old?.Name} {LocalizedStrings.Instance["logWithThePathAdmin"]} {old?.Path} " +
                        $"{LocalizedStrings.Instance["logWasChangedToAdmin"]} {LocalizedStrings.Instance["logTheObjectAdmin"]} " +
                        $"{obj?.Name} {LocalizedStrings.Instance["logWithThePathAdmin"]} {obj?.Path}";

                    break;
                case CommandAction.Clear:
                    str=LocalizedStrings.Instance["logAllObjectsWerePurgedAdmin"];

                    break;
            }
            LogWithDataAndOrder(str);
        }

        void LogSubject(CommandAction comAct, SubjectViewModel subj,SubjectViewModel old = null)
        {
            string str = "";
            switch (comAct)
            {
                case CommandAction.Add:
                    str=$"{LocalizedStrings.Instance["logTheSubjectAdmin"]} {subj?.Name} {subj?.SecondName} " +
                        $"{LocalizedStrings.Instance["logWithTheLoginAdmin"]} {subj?.Login} " +
                        $"{LocalizedStrings.Instance["logWasAddedAdmin"]}";
                    break;
                case CommandAction.Remove:
                    str=$"{LocalizedStrings.Instance["logTheSubjectAdmin"]} {subj?.Name} {subj?.SecondName} " +
                        $"{LocalizedStrings.Instance["logWithTheLoginAdmin"]} {subj?.Login} " +
                        $"{LocalizedStrings.Instance["logWasRemovedAdmin"]}";

                    break;
                case CommandAction.Change:
                    str=$"{LocalizedStrings.Instance["logTheSubjectAdmin"]} {old?.Name} {old?.SecondName} " +
                        $"{LocalizedStrings.Instance["logWithTheLoginAdmin"]} {old?.Login} " +
                        $"{LocalizedStrings.Instance["logAndPasswordAdmin"]} {old?.Password}" +
                        $"{LocalizedStrings.Instance["logWasChangedToAdmin"]} " +
                        $"{LocalizedStrings.Instance["logTheSubjectAdmin"]} {subj?.Name} {subj?.SecondName} " +
                        $"{LocalizedStrings.Instance["logWithTheLoginAdmin"]} {subj?.Login} " +
                        $"{LocalizedStrings.Instance["logAndPasswordAdmin"]} {subj?.Password}";

                    break;
                case CommandAction.Clear:
                    str= LocalizedStrings.Instance["logAllSubjectsWerePurgedAdmin"];
                    break;
            }
            LogWithDataAndOrder(str);

        }

        void LogWithDataAndOrder(string str)
        {
            if (str != "")
            {
                Log.Insert(0, $"{Log.Count}){str} - {DateTime.Now}");
            }
        }
        #endregion

        #region Type of Model: Settings
        public bool IsBelLapChecked { get { return _matricsStore.
                    CurrentMatrics.CurrentModelType == MatricesViewModel.ModelType.
                    BellaLapadula; }
			set {
                if(value && _matricsStore.CurrentMatrics.CurrentModelType != MatricesViewModel.ModelType.BellaLapadula)
                {
                    _matricsStore.CurrentMatrics.CurrentModelType = MatricesViewModel.ModelType.BellaLapadula;
                    LogWithDataAndOrder(LocalizedStrings.Instance["logTypeOfModelChangedToBella-LapadulaAdmin"]);
                    OnPropertyChanged(nameof(TreeSource));

                }
            } 
        }

		public bool IsBibaChecked { get{ 
                return _matricsStore.CurrentMatrics.CurrentModelType == MatricesViewModel.ModelType.Biba; 
            }
			set {
                if (value && _matricsStore.CurrentMatrics.CurrentModelType != MatricesViewModel.ModelType.Biba)
                {
                    _matricsStore.CurrentMatrics.CurrentModelType = MatricesViewModel.ModelType.Biba;
                    LogWithDataAndOrder(LocalizedStrings.Instance["logTypeOfModelChangedToBibaAdmin"]);
                    OnPropertyChanged(nameof(TreeSource));
                }
            } 
        }

		public bool IsCombinedChecked { get{ 
                return _matricsStore.CurrentMatrics.CurrentModelType == MatricesViewModel.ModelType.Combined; 
            } 
			set {
                if (value && _matricsStore.CurrentMatrics.CurrentModelType != MatricesViewModel.ModelType.Combined)
                {
                    _matricsStore.CurrentMatrics.CurrentModelType = MatricesViewModel.ModelType.Combined;
                    LogWithDataAndOrder(LocalizedStrings.Instance["logTypeOfModelChangedToCombinedAdmin"]);
                    OnPropertyChanged(nameof(TreeSource));
                }
            } 
        }

        public bool IsEnglishChecked
        {
            get { return Settings.Default.CurrentCulture =="en-US"; }
            set {
                if(value)
                {
                    Settings.Default.CurrentCulture = "en-US";
                    LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                    LocalizeDictionary.Instance.SetCultureCommand.Execute("en-US");
                }
                    OnPropertyChanged(nameof(IsEnglishChecked));
            }
        }

        public bool IsRussianChecked
        {
            get { return Settings.Default.CurrentCulture == "ru-RU"; }
            set {
                if (value == true)
                {   
                    Settings.Default.CurrentCulture = "ru-RU";
                    LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
                    LocalizeDictionary.Instance.SetCultureCommand.Execute("ru-RU");

                }
                    OnPropertyChanged(nameof(IsRussianChecked));
                
            }
        }

        #endregion

        #region TreeView: Access Matrix

        private ObservableCollection<SubjectTreeViewModel> _treeSource;
        public ObservableCollection<SubjectTreeViewModel> TreeSource
        {
            get
            {
                return new ObservableCollection<SubjectTreeViewModel>(from s in Subjects select
                         new SubjectTreeViewModel(s, _matricsStore.CurrentMatrics));
            }
           
        }

        private object _treeViewSelectedItem;
        public object TreeViewSelectedItem
        {
            get { return _treeViewSelectedItem; }
            set { _treeViewSelectedItem = value;
                if(value is SecurityRightViewModel s)
                {
                    IsReadable=s.IsReadable;
                    IsWritable=s.IsWritable;
                    IsExecutable=s.IsExecutable;

                    ChosenObjectName = s.Object.Name;
                    ChosenObjectSecurityMark = s.Object.SecurityMark;
                    IsObjectOrSubject = LocalizedStrings.Instance["textBlockOBJECTAccessMatrix"];
                }
                else if(value is SubjectTreeViewModel subj)
                {
                    IsReadable=false;
                    IsWritable=false;
                    IsExecutable=false;

                    ChosenObjectName = subj.Subject.Name;
                    ChosenObjectSecurityMark = subj.Subject.SecurityMark;
                    IsObjectOrSubject = LocalizedStrings.Instance["textBlockSUBJECTAccessMatrix"];
                }
                OnPropertyChanged(nameof(TreeViewSelectedItem));
            }
        } 
        public ICommand SetSelectedItemCommand { get; }

        public bool _isReadable;
        public bool IsReadable { get { return _isReadable; } set { _isReadable = value;
                OnPropertyChanged(nameof(IsReadable));
            } }
        public bool _isWritable;
        public bool IsWritable {
            get { return _isWritable; }
            set
            {
                _isWritable = value;
                OnPropertyChanged(nameof(IsWritable));
            }
        }
        public bool _isExecutable;
        public bool IsExecutable {
            get { return _isExecutable; }
            set
            {
                _isExecutable = value;
                OnPropertyChanged(nameof(IsExecutable));
            }
        }
        private string _chosenObjectName;
        public string ChosenObjectName
        {
            get { return _chosenObjectName; }
            set { _chosenObjectName = value;
                OnPropertyChanged(nameof(ChosenObjectName));
            }
        }
        private SecurityMark _chosenObjectSecurityMark;
        public SecurityMark ChosenObjectSecurityMark
        {
            get { return _chosenObjectSecurityMark; }
            set { _chosenObjectSecurityMark = value;
                OnPropertyChanged(nameof(ChosenObjectSecurityMark));
            }
        }
        private string _isObjectOrSubject;
        public string IsObjectOrSubject
        {
            get { return _isObjectOrSubject; }
            set
            {
                _isObjectOrSubject = value;
                OnPropertyChanged(nameof(IsObjectOrSubject));
            }
        }

        private void SubjectsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TreeSource));
        }

        private void ObjectsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TreeSource));
        }
        #endregion
      
        #region Validation

        private void ValidateSubjectLogin(string login)
        {
            var i = _matricsStore.CurrentMatrics.Subjects.FirstOrDefault(x => x.Login == login);
            if ((string.IsNullOrEmpty(login)) || (i != null))
            {
                _errorsViewModel.AddError(nameof(SubjectLogin), LocalizedStrings.Instance["errorInvalidLoginAdmin"]);
            }
        }



        private void ValidateStringIsNullOrEmpty(string str, string propName, ErrorsViewModel _errors)
        {
            if (string.IsNullOrEmpty(str))
            {
                _errors.AddError(propName, $"{propName} {LocalizedStrings.Instance["errorFieldIsEmptyAdmin"]}");
            }

        }



        private readonly ErrorsViewModel _errorsViewModel;
        private readonly ErrorsViewModel _errorsObjectViewModel;


        public IEnumerable GetErrors(string? propertyName)
        {

            var subjErrors = _errorsViewModel.GetErrors(propertyName);
            var objErrors = _errorsObjectViewModel.GetErrors(propertyName);
            List<object> erros = new List<object>();
            if (subjErrors != null)
            {
                foreach (var error in subjErrors) { erros.Add(error); }

            }
            if (objErrors != null)
            {
                foreach (var error in objErrors) { erros.Add(error); }
            }
            return erros;
        }

        private void ErrorsViewModel_Errorschanged(object? sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanCreate));
        }
        private void ErrorsObjectViewModel_Errorschanged(object? sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanCreateObj));
        }
        public bool HasErrors => _errorsViewModel.HasErrors||_errorsObjectViewModel.HasErrors;

        public bool CanCreate => !_errorsViewModel.HasErrors;
        public bool CanCreateObj => !_errorsObjectViewModel.HasErrors;


        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        #endregion

    }
}
