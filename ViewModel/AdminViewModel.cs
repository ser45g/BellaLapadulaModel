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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            //Rights = new ObservableCollection<SecurityRightViewModel>(_matricsStore.CurrentMatrics.Rights);
            Objects.CollectionChanged += ObjectsChanged ;
            Subjects.CollectionChanged += SubjectsChanged;


			AddObjectCommand=new RelayCommand((obj) => {
                ObjectViewModel o = new ObjectViewModel(new Model.Object() { Path = ObjectPath, Name = ObjectName, SecurityMark = ObjectSecurityMark });

                ValidateStringIsNullOrEmpty(o.Name,nameof(ObjectName),_errorsObjectViewModel);
                ValidateStringIsNullOrEmpty(o.Path, nameof(ObjectPath), _errorsObjectViewModel);
                if (CanCreateObj)
                {
                    LogObject(CommandAction.Add, o);
			        Objects.Add(o);
                }
               
            },(obj)=>CanCreateObj);
			RemoveObjectCommand=new RelayCommand((obj) => { LogObject(CommandAction.Remove, SelectedObject); Objects.Remove(SelectedObject); });
			ClearObjectCommand=new RelayCommand((obj) => { LogObject(CommandAction.Clear, null); Objects.Clear(); });
            ChangeObjectCommand = new RelayCommand((obj) => { RemoveObjectCommand.Execute(obj); AddObjectCommand.Execute(obj); });			
            
			//*********************************************************************************
			AddSubjectCommand=new RelayCommand((obj) => {
                SubjectViewModel s = new SubjectViewModel(new Subject()
                {
                    Login = SubjectLogin,
                    Password = SubjectPassword,
                    SecurityMark = SubjectSecurityMark
                })
                {Name=SubjectName,SecondName=SubjectSecondName };
                ValidateSubjectLogin(s.Login);
                ValidateStringIsNullOrEmpty(s.Password, nameof(SubjectPassword),_errorsViewModel);
                if (CanCreate)
                {   
                    LogSubject(CommandAction.Add, s);
                    Subjects.Add(s);
                }
            },(obj)=>CanCreate);
			RemoveSubjectCommand=new RelayCommand((obj)=> { LogSubject(CommandAction.Remove, SelectedSubject); Subjects.Remove(SelectedSubject); });
			ClearSubjectCommand=new RelayCommand((obj) => { LogSubject(CommandAction.Clear, null); Subjects.Clear(); });
			ChangeSubjectCommand = new RelayCommand((obj) => {

                _errorsViewModel.ClearErrors(nameof(SubjectLogin));
                SubjectViewModel s = new SubjectViewModel(new Subject()
                {
                    Login = SubjectLogin,
                    Password = SubjectPassword,
                    SecurityMark = SubjectSecurityMark
                })
                { Name = SubjectName, SecondName = SubjectSecondName };
                ValidateStringIsNullOrEmpty (s.Login, nameof(SubjectLogin),_errorsViewModel);
                ValidateStringIsNullOrEmpty(s.Password, nameof(SubjectPassword),_errorsViewModel);
                SubjectViewModel old = Subjects.FirstOrDefault((obj) => { return obj.Login == SubjectLogin; });
                if (CanCreate)
                {
                    LogSubject(CommandAction.Remove, old);
                    Subjects.Remove(old);
                    LogSubject(CommandAction.Add, s);
                    Subjects.Add(s);

                }
            });

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
			} }
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

        void LogObject(CommandAction comAct, ObjectViewModel obj)
        {
            string str = "";
            switch (comAct) {
                case CommandAction.Add:
                    str=$"The object {obj?.Name} with the path {obj?.Path} was added ";
                    break;
                case CommandAction.Remove:
                    str=$"The object {obj?.Name} with the path {obj?.Path} was removed";

                    break;
                case CommandAction.Change:
                    str=$"The object {obj?.Name} with the path {obj?.Path} was changed";

                    break;
                case CommandAction.Clear:
                    str="All the objects were purged";

                    break;
            }
            LogWithDataAndOrder(str);
        }

        void LogSubject(CommandAction comAct, SubjectViewModel subj)
        {
            string str = "";
            switch (comAct)
            {
                case CommandAction.Add:
                    str=  $"The subject {subj?.Name} {subj?.SecondName} with the login {subj?.Login} was added";
                    break;
                case CommandAction.Remove:
                    str= $"The subject {subj?.Name} {subj?.SecondName} with the login {subj?.Login} was removed";

                    break;
                case CommandAction.Change:
                    str= $"The subject {subj?.Name} {subj?.SecondName} with the login {subj?.Login} was changed";

                    break;
                case CommandAction.Clear:
                    str= "All the subjects were purged";

                    break;
            }
            LogWithDataAndOrder(str);

        }

        void LogWithDataAndOrder(string str)
        {
            if (str != "")
            {
                Log.Insert(0, $"{Log.Count}){str} at {DateTime.Now}");
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
                    LogWithDataAndOrder("The Type of the model was changed to BellaLapadula");
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
                    LogWithDataAndOrder("The Type of the model was changed to Biba");
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
                    LogWithDataAndOrder("The Type of the model was changed to BellaLapadula");
                    OnPropertyChanged(nameof(TreeSource));

                }
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
                    IsObjectOrSubject = "OBJECT";
                }else if(value is SubjectTreeViewModel subj)
                {
                    IsReadable=false;
                    IsWritable=false;
                    IsExecutable=false;

                    ChosenObjectName = subj.Subject.Name;
                    ChosenObjectSecurityMark = subj.Subject.SecurityMark;
                    IsObjectOrSubject = "SUBJECT";
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
                _errorsViewModel.AddError(nameof(SubjectLogin), "This login is not correct/available");
            }
        }



        private void ValidateStringIsNullOrEmpty(string str, string propName, ErrorsViewModel _errors)
        {
            if (string.IsNullOrEmpty(str))
            {
                _errors.AddError(propName, $"The {propName} field can't be empty");
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

    public class SubjectTreeViewModel
    {
        public SubjectViewModel Subject { get; }
        public ObservableCollection<SecurityRightViewModel> Objects { get; }

        public SubjectTreeViewModel(SubjectViewModel subject,MatricesViewModel matrix)
        {
            this.Subject = subject;
            Objects = new ObservableCollection<SecurityRightViewModel>( matrix.GetObjectsForSubj(Subject.Login));
        }
       
    }

}
