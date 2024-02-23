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

		//**********************************************************************

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
                _errorsViewModel.ClearErrors(nameof(ObjectName));
				_objectName = value;
				OnPropertyChanged(nameof(ObjectName));
			} }

        private string _objectPath;
        public string ObjectPath { get { return _objectPath; } set { 
                _errorsViewModel.ClearErrors(nameof(ObjectPath));
                
                _objectPath = value;
				OnPropertyChanged(nameof(ObjectPath));
			} }

		private SecurityMark _objectSecurityMark;
		public SecurityMark ObjectSecurityMark { get { return _objectSecurityMark; } set {
				_objectSecurityMark = value;
				OnPropertyChanged(nameof(ObjectSecurityMark));
			} }

        //*********************************************************************

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

		//*****************************************************************************
        
        public ObservableCollection<SecurityRightViewModel> Rights { get;  }

       
        
		private ObservableCollection<SubjectViewModel> subjects ;
		public ObservableCollection<SubjectViewModel> Subjects {  get {
				return subjects;
            } }
		private ObservableCollection<ObjectViewModel> objects ;
		public ObservableCollection<ObjectViewModel> Objects {  get { return 
					objects; } }

		public ICommand AddObjectCommand { get; }
		public ICommand AddSubjectCommand { get; }
		public ICommand ChangeObjectCommand { get; }
		public ICommand ChangeSubjectCommand { get; }
		public ICommand RemoveObjectCommand { get; }
		public ICommand RemoveSubjectCommand { get; }
		public ICommand ClearSubjectCommand { get; }
		public ICommand ClearObjectCommand { get; }

		public ICommand NavigateLoginCommand { get; }

        public AdminViewModel(NavigationStore ns, MatricsStore ms) {

            NavigateLoginCommand = new NavigateCommand<LoginViewModel>(
				new NavigationService<LoginViewModel>(ns, () => new LoginViewModel(ns,_matricsStore)));

			_matricsStore= ms;

            subjects = _matricsStore.CurrentMatrics.Subjects;
            objects = _matricsStore.CurrentMatrics.Objects;

            _log = _matricsStore.CurrentMatrics.Log;

            Rights = new ObservableCollection<SecurityRightViewModel>(_matricsStore.CurrentMatrics.Rights);
            Objects.CollectionChanged += ObjectsChanged ;
            Subjects.CollectionChanged += SubjectsChanged;


			AddObjectCommand=new RelayCommand((obj) => {
                ObjectViewModel o = new ObjectViewModel(new Model.Object() { Path = ObjectPath, Name = ObjectName, SecurityMark = ObjectSecurityMark });

                ValidateObjectName(o.Name);
                ValidateObjectPath(o.Path);
                if (CanCreateObj)
                {
                    LogObject(CommandAction.Add, o);
			        Objects.Add(o);

                }
               
            },(obj)=>CanCreate);
			RemoveObjectCommand=new RelayCommand((obj) => { LogObject(CommandAction.Remove, SelectedObject); Objects.Remove(SelectedObject); });
			ClearObjectCommand=new RelayCommand((obj) => { LogObject(CommandAction.Clear, null); Objects.Clear(); });
			ChangeObjectCommand = new RelayCommand((obj) => { RemoveObjectCommand.Execute(obj); AddObjectCommand.Execute(obj); });
			
			AddSubjectCommand=new RelayCommand((obj) => {
                SubjectViewModel s = new SubjectViewModel(new Subject()
                {
                    Login = SubjectLogin,
                    Password = SubjectPassword,
                    SecurityMark = SubjectSecurityMark
                })
                {Name=SubjectName,SecondName=SubjectSecondName };
                ValidateSubjectLogin(s.Login);
                ValidateSubjectPassword(s.Password);
                if (CanCreate)
                {   
                    LogSubject(CommandAction.Add, s);
                    Subjects.Add(s);

                }
                
   
            },(obj)=>CanCreate);
			RemoveSubjectCommand=new RelayCommand((obj)=> { LogSubject(CommandAction.Remove, SelectedSubject); Subjects.Remove(SelectedSubject); });
			ClearSubjectCommand=new RelayCommand((obj) => { LogSubject(CommandAction.Clear, null); Subjects.Clear(); });
			ChangeSubjectCommand = new RelayCommand((obj) => { RemoveSubjectCommand?.Execute(obj); AddSubjectCommand?.Execute(obj); });

            SetSelectedItemCommand = new RelayCommand((obj) => { TreeViewSelectedItem = obj; });

            BrowseObjectPathCommand = new RelayCommand((obj) =>
            {
                if (_openFileDialog.ShowDialog() == true)
                {
                    ObjectPath = _openFileDialog.FileName;
                }
            });

            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_Errorschanged;

        }

        private void ValidateSubjectLogin(string login)
        {

            var i = _matricsStore.CurrentMatrics.Subjects.FirstOrDefault(x => x.Login == login);
            if ((string.IsNullOrEmpty(login))|| 
                ( i!= null
               ))
            {
                _errorsViewModel.AddError(nameof(SubjectLogin), "This login is not correct/available");
            }
            else{

            }
        }

        private void ValidateSubjectPassword(string pwd)
        {

            if(string.IsNullOrEmpty(pwd))
            {
                _errorsViewModel.AddError(nameof(SubjectPassword), "The password field can't be empty");
            }
            
        }


        private void ValidateObjectName(string name)
        {

            if (string.IsNullOrEmpty(name))
            {
                _errorsViewModel.AddError(nameof(ObjectName), "The name field can't be empty");
            }

        }
        private void ValidateObjectPath(string path)
        {

            if (string.IsNullOrEmpty(path))
            {
                _errorsViewModel.AddError(nameof(ObjectPath), "The path field can't be empty");
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

        //********************************************************************



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
        //*****************************************************************************
        void LogWithDataAndOrder(string str)
        {
            if (str != "")
            {
                Log.Insert(0, $"{Log.Count}){str} at {DateTime.Now}");
            }
        }
        private readonly ErrorsViewModel _errorsViewModel;

      
        public IEnumerable GetErrors(string? propertyName)
        {
            return
            _errorsViewModel.GetErrors(propertyName);
        }
        private void ErrorsViewModel_Errorschanged(object? sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanCreate));
        }
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


        private ObservableCollection<SubjectTreeViewModel> _treeSource;

        public ObservableCollection<SubjectTreeViewModel> TreeSource
        {
            get
            {
                return new ObservableCollection<SubjectTreeViewModel>(from s in Subjects
                                                                      select
                                                                         new SubjectTreeViewModel(s, _matricsStore.CurrentMatrics));
            }
           
        }
        public ObservableCollection<SubjectViewModel> Rows => _matricsStore.CurrentMatrics.Subjects;
        public ObservableCollection<SecurityRightViewModel> Columns => new ObservableCollection<SecurityRightViewModel>( _matricsStore.CurrentMatrics.Rights);
        public ICommand AccessMatrixGotFocusCommand { get; }

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

        public ICommand BrowseObjectPathCommand {  get; }

        public bool HasErrors => _errorsViewModel.HasErrors;
        public bool CanCreate => !HasErrors;
        public bool CanCreateObj => !HasErrors;

        private OpenFileDialog _openFileDialog=new OpenFileDialog();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
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
