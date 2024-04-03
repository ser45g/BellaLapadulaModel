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
using MultipleUserLoginForm.Data;
using System.Windows.Data;
using System.Reflection;

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
            TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleAdmin"]} - " + 
                LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"] ;

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
            TreeSourceFiltered = new CollectionViewSource { Source = TreeSource }.View;

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

            _securityMarkComboSource = new ObservableCollection<string>();
            _categoryComboSource = new ObservableCollection<string>();
            SetLocalizedComboSource();
           
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
                    ObjectSecurityCategory= _selectedObject.SecurityCategory;
				}
				OnPropertyChanged(nameof(SelectedObject));
                OnPropertyChanged(nameof(ObjectMandatoryMark));

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
                OnPropertyChanged(nameof(ObjectSecurityMarkDisplay));
                OnPropertyChanged(nameof(ObjectMandatoryMark));
			} 
        }
        private Category _objectSecurityCategory;
        public Category ObjectSecurityCategory
        {
            get { return _objectSecurityCategory; }
            set
            {
                _objectSecurityCategory = value;
                OnPropertyChanged(nameof(ObjectSecurityCategory));
                OnPropertyChanged(nameof(ObjectSecurityCategoryDisplay));
                OnPropertyChanged(nameof(ObjectMandatoryMark));
            }
        }
        // private string _objectSecurityMarkDisplay;

        public string ObjectSecurityMarkDisplay
        {
            get { string s= LocalizedStrings.Instance["SecurityMark" + ObjectSecurityMark.ToString()]; return s; }
            set { 
                if (value != null)
                {

                    ObjectSecurityMark = _KeyLocalizedValueSecurityMarkDictionary[value];
                    OnPropertyChanged(nameof(ObjectSecurityMarkDisplay));
                   
                }
            }
        }
        public string ObjectSecurityCategoryDisplay
        {
            get { return LocalizedStrings.Instance["category" + ObjectSecurityCategory.ToString()]; }
            set {
                if(value != null)
                {
                    ObjectSecurityCategory = _KeyLocalizedValueCategoryDictionary[value];
                    OnPropertyChanged(nameof(ObjectSecurityCategoryDisplay));
                    
                }
                
               
            }
        }
        public string ObjectMandatoryMark
        {
            get { return ObjectViewModel.GetMandatoryMarkAsString(ObjectSecurityMark, ObjectSecurityCategory); }

        }



        bool _objectBeingAdded = false;
        private void AddObject(object obj)
        {
            if (_objectBeingAdded == true)
                return;
            _objectBeingAdded = true;
            ObjectViewModel o = new ObjectViewModel(new Model.Object() { Path = ObjectPath, Name = ObjectName, SecurityMark = ObjectSecurityMark,
            SecurityCategory=ObjectSecurityCategory});

            ValidateStringIsNullOrEmpty(o.Name, nameof(ObjectName), _errorsObjectViewModel);
            ValidateStringIsNullOrEmpty(o.Path, nameof(ObjectPath), _errorsObjectViewModel);
            if (CanCreateObj)
            {
                LogObject(CommandAction.Add, o);
                ModelData.AddObjectToDb(o);
                Objects.Add(o);
            }
            _objectBeingAdded = false;
        }

        private void RemoveObject(object obj)
        { 
            if(SelectedObject != null)
            {   
                LogObject(CommandAction.Remove, SelectedObject); 
                ModelData.RemoveObjectToDb(SelectedObject);
                Objects.Remove(SelectedObject);

            }
            else
            {
                MessageBox.Show(LocalizedStrings.Instance["mesBoxToRemoveObjectNeedToSelectAdmin"], 
                    LocalizedStrings.Instance["mesBoxWarningAdmin"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
           
        }

        private void ChangeObject(object obj)
        {
            ObjectViewModel o = new ObjectViewModel(new Model.Object() { Path = ObjectPath, Name = ObjectName, SecurityMark = ObjectSecurityMark,
            SecurityCategory=ObjectSecurityCategory});

            ValidateStringIsNullOrEmpty(o.Name, nameof(ObjectName), _errorsObjectViewModel);
            ValidateStringIsNullOrEmpty(o.Path, nameof(ObjectPath), _errorsObjectViewModel);
            ObjectViewModel old = SelectedObject;
            if (old == null) {
                MessageBox.Show(LocalizedStrings.Instance["msgToChangeObjectNeedSelectAdmin"], LocalizedStrings.Instance["mesBoxWarningAdmin"],
                    MessageBoxButton.OK,MessageBoxImage.Warning);
                return; 
            }
            if (CanCreateObj)
            {
                LogObject(CommandAction.Change, o,old);
                ModelData.ChangeObjectToDb(o,old);
                Objects.Remove(old);
                Objects.Add(o);

            }
        }

        private void ClearObjects(object obj)
        {
            bool clear = MessageBox.Show(LocalizedStrings.Instance["mesBoxAreYouSureDeleteAllObjectsAdmin"],
                LocalizedStrings.Instance["mesBoxWarningAdmin"], MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK;
            if (!clear) { return; }
            LogObject(CommandAction.Clear, null);
            ModelData.ClearObjectsDb();
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
                    SubjectSecurityCategory = _selectedSubject.SecurityCategory;
                    SubjectName = _selectedSubject.Name;
                    SubjectSecondName = _selectedSubject.SecondName;

                }
                OnPropertyChanged(nameof(SelectedSubject));
                OnPropertyChanged(nameof(SubjectMandatoryMark));

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
                OnPropertyChanged(nameof(SubjectSecurityMarkDisplay));
            }
        }
        private string _subjectSecurityMarkDisplay;
        public string SubjectSecurityMarkDisplay
        {
            get {  return LocalizedStrings.Instance["SecurityMark" + SubjectSecurityMark.ToString()];  }
            set
            {
                if (value == null)
                    return;
                 SubjectSecurityMark = _KeyLocalizedValueSecurityMarkDictionary[value];
                 //OnPropertyChanged(nameof(SubjectSecurityMarkDisplay));
                
            }
        }
        public string SubjectMandatoryMark
        {
            get { return SubjectViewModel.GetMandatoryMarkAsString(SubjectSecurityMark, SubjectSecurityCategory);  }
            
        }

        private Category _subjectSecurityCategory;
        public Category SubjectSecurityCategory
        {
            get { return _subjectSecurityCategory; }
            set
            {
                
                _subjectSecurityCategory = value;
                OnPropertyChanged(nameof(SubjectSecurityCategory));
                OnPropertyChanged(nameof(SubjectSecurityCategoryDisplay));
            }
        }
        public string SubjectSecurityCategoryDisplay
        {
            get { return LocalizedStrings.Instance["category" + SubjectSecurityCategory.ToString()]; }
            set
            {
                if (value != null)
                {
                    SubjectSecurityCategory = _KeyLocalizedValueCategoryDictionary[value];
                }
                else
                {
                    SubjectSecurityCategory = Category.First;
                }
                    OnPropertyChanged(nameof(SubjectSecurityCategoryDisplay));
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
                SecondName = SubjectSecondName,
                SecurityCategory = SubjectSecurityCategory
            });
            ValidateSubjectLogin(s.Login);
            ValidateStringIsNullOrEmpty(s.Password, nameof(SubjectPassword), _errorsViewModel);
            if (CanCreate)
            {
                LogSubject(CommandAction.Add, s);
                Subjects.Add(s);
                ModelData.AddSubjectToDb(s);
            }
        }

        private void ChangeSubject(object obj)
        {

            SubjectViewModel s = new SubjectViewModel(new Subject()
            {
                Login = SubjectLogin,
                Password = SubjectPassword,
                SecurityMark = SubjectSecurityMark,
                Name = SubjectName,
                SecondName = SubjectSecondName,
            SecurityCategory=SubjectSecurityCategory}
            );
            ValidateStringIsNullOrEmpty(s.Login, nameof(SubjectLogin), _errorsViewModel);
            ValidateStringIsNullOrEmpty(s.Password, nameof(SubjectPassword), _errorsViewModel);
            SubjectViewModel old = SelectedSubject;
            if (old == null)
            {
                MessageBox.Show(LocalizedStrings.Instance["msgToChangeSubjectNeedSelectAdmin"], LocalizedStrings.Instance["mesBoxWarningAdmin"],
                   MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CanCreate)
            {
                LogSubject(CommandAction.Change, s,old);
                ModelData.ChangeSubjectToDb(s,old);
                Subjects.Remove(old);
                Subjects.Add(s);

            }
        }

        private void RemoveSubject(object obj)
        {
            if(SelectedSubject != null)
            {   
                LogSubject(CommandAction.Remove, SelectedSubject); 
                ModelData.RemoveSubjectToDb(SelectedSubject);
                Subjects.Remove(SelectedSubject);

            }
            else
            {
                MessageBox.Show(LocalizedStrings.Instance["mesBoxToRemoveSubjectNeedToSelectAdmin"],
                    LocalizedStrings.Instance["mesBoxWarningAdmin"], MessageBoxButton.OK, MessageBoxImage.Warning);

            }

        }
        
        private void ClearSubjects(object obj)
        { 
            bool clear= MessageBox.Show(LocalizedStrings.Instance["mesBoxAreYouSureDeleteAllSubjectsAdmin"],
                 LocalizedStrings.Instance["mesBoxWarningAdmin"],
                 MessageBoxButton.OKCancel, MessageBoxImage.Information)==MessageBoxResult.OK;
            if (!clear) { return; }
            LogSubject(CommandAction.Clear, null);
            ModelData.ClearSubjectsDb();
            Subjects.Clear();
        }
        #endregion



        private ObservableCollection<SubjectViewModel> subjects ;
		public ObservableCollection<SubjectViewModel> Subjects {  get {
				return (subjects);
            }
            set {
                subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }
		private ObservableCollection<ObjectViewModel> objects ;
		public ObservableCollection<ObjectViewModel> Objects {  get {
                return (objects); 
            }
            set { objects = value;
                OnPropertyChanged(nameof(Objects)); 
            }
        }

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

        private ObservableCollection<string> _securityMarkComboSource;
        public ObservableCollection< string> SecurityMarkComboSource { 
            get { return _securityMarkComboSource; } 
            set {
                _securityMarkComboSource = value;
                OnPropertyChanged(nameof(SecurityMarkComboSource));
            } }
        private ObservableCollection<string> _categoryComboSource;
        public ObservableCollection< string> CategoryComboSource { 
            get { return _categoryComboSource; } set {
                _categoryComboSource = value;
                OnPropertyChanged(nameof(CategoryComboSource));
            } }

        private Dictionary<string, SecurityMark> _KeyLocalizedValueSecurityMarkDictionary=new Dictionary<string, SecurityMark>();
        private Dictionary<string, Category> _KeyLocalizedValueCategoryDictionary=new Dictionary<string, Category>();

        private void SetLocalizedComboSource() {
            if (SecurityMarkComboSource == null||CategoryComboSource==null) return;


            _KeyLocalizedValueSecurityMarkDictionary.Clear();
            _KeyLocalizedValueCategoryDictionary.Clear();
 
            _KeyLocalizedValueSecurityMarkDictionary.Add(LocalizedStrings.Instance["SecurityMarkUnclassified"],SecurityMark.Unclassified);
            _KeyLocalizedValueSecurityMarkDictionary.Add(LocalizedStrings.Instance["SecurityMarkConfidential"],SecurityMark.Confidential);
            _KeyLocalizedValueSecurityMarkDictionary.Add(LocalizedStrings.Instance["SecurityMarkSecret"],SecurityMark.Secret);
            _KeyLocalizedValueSecurityMarkDictionary.Add(LocalizedStrings.Instance["SecurityMarkTopSecret"],SecurityMark.TopSecret);

            _KeyLocalizedValueCategoryDictionary.Add(LocalizedStrings.Instance["categoryFirst"], Category.First);
            _KeyLocalizedValueCategoryDictionary.Add(LocalizedStrings.Instance["categorySecond"], Category.Second);
            _KeyLocalizedValueCategoryDictionary.Add(LocalizedStrings.Instance["categoryThird"], Category.Third);
            _KeyLocalizedValueCategoryDictionary.Add(LocalizedStrings.Instance["categoryFourth"], Category.Fourth);
            _KeyLocalizedValueCategoryDictionary.Add(LocalizedStrings.Instance["categoryFifth"], Category.Fifth);

            var c = new ObservableCollection<string>();
            var s = new ObservableCollection<string>();
           
            s.Add(LocalizedStrings.Instance["SecurityMarkUnclassified"]);
            s.Add(LocalizedStrings.Instance["SecurityMarkConfidential"]);
            s.Add(LocalizedStrings.Instance["SecurityMarkSecret"]);
            s.Add(LocalizedStrings.Instance["SecurityMarkTopSecret"]);
           
            c.Add(LocalizedStrings.Instance["categoryFirst"]);
            c.Add(LocalizedStrings.Instance["categorySecond"]);
            c.Add(LocalizedStrings.Instance["categoryThird"]);
            c.Add(LocalizedStrings.Instance["categoryFourth"]);
            c.Add(LocalizedStrings.Instance["categoryFifth"]);

            SecurityMarkComboSource = s;
            CategoryComboSource = c;
          
            //List<string> a = new List<string>();
            //foreach(var i in SecurityMarkComboSource) { 
            //    i
            //}
            OnPropertyChanged(nameof(SecurityMarkComboSource));
            OnPropertyChanged(nameof(CategoryComboSource));

           //OnPropertyChanged(nameof(ObjectSecurityCategoryDisplay));
           //OnPropertyChanged(nameof(ObjectSecurityMarkDisplay));
            //string s = LocalizedStrings.Instance["SecurityMark" + ObjectSecurityMark.ToString()];
           // ObjectSecurityMarkDisplay = s;
            //OnPropertyChanged(nameof(SubjectSecurityCategoryDisplay));
            OnPropertyChanged(nameof(SubjectSecurityMarkDisplay));

        }
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
                    Settings.Default.CurrentTypeOfModel = _matricsStore.CurrentMatrics.CurrentModelType.ToString();

                    LogWithDataAndOrder(LocalizedStrings.Instance["logTypeOfModelChangedToBella-LapadulaAdmin"]);
                    TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleAdmin"]} - " +
               LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"];
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
                    Settings.Default.CurrentTypeOfModel = _matricsStore.CurrentMatrics.CurrentModelType.ToString();

                    LogWithDataAndOrder(LocalizedStrings.Instance["logTypeOfModelChangedToBibaAdmin"]);
                    TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleAdmin"]} - " +
                LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"];
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
                    Settings.Default.CurrentTypeOfModel=_matricsStore.CurrentMatrics.CurrentModelType.ToString();

                    LogWithDataAndOrder(LocalizedStrings.Instance["logTypeOfModelChangedToCombinedAdmin"]);
                    TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleAdmin"]} - " +
                LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"];
                    OnPropertyChanged(nameof(TreeSource));
                }
            } 
        }

        public bool IsEnglishChecked
        {
            get { return LocalizedStrings.Instance.GetCurrentCultureCode() =="en-US"; }
            set {
                if(value==true)
                {
                    Settings.Default.CurrentCulture = "en-US";
                    LocalizedStrings.Instance.SetCulture("en-US");

                    Objects = new ObservableCollection<ObjectViewModel>(Objects);
                    Subjects = new ObservableCollection<SubjectViewModel>(Subjects);
                    OnPropertyChanged(nameof(IsEnglishChecked));
                    OnPropertyChanged(nameof(TreeSource));

                    SetLocalizedComboSource();
                    TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleAdmin"]} - " +
                LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"];
                }
            }
        }

        public bool IsRussianChecked
        {
            get { return LocalizedStrings.Instance.GetCurrentCultureCode() == "ru-RU"; }
            set {
                if (value == true)
                {   
                    Settings.Default.CurrentCulture = "ru-RU";
                    LocalizedStrings.Instance.SetCulture("ru-RU");

                    Objects = new ObservableCollection<ObjectViewModel>(Objects);
                    Subjects = new ObservableCollection<SubjectViewModel>(Subjects);
                    OnPropertyChanged(nameof(IsRussianChecked));
                    OnPropertyChanged(nameof(TreeSource));

                    SetLocalizedComboSource();
                    TitleStore.Instance.Title = $"{LocalizedStrings.Instance["titleAdmin"]} - " +
                LocalizedStrings.Instance[$"modelType{_matricsStore.CurrentMatrics.CurrentModelType.ToString()}"];
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
                _treeSource = new ObservableCollection<SubjectTreeViewModel>(from s in Subjects
                            where DoesSubjectContain(s) select
                         new SubjectTreeViewModel(s, _matricsStore.CurrentMatrics));

                return _treeSource;
            }
           
           
        }
        private ICollectionView _treeSourceFiltered;
        public ICollectionView TreeSourceFiltered { get { return _treeSourceFiltered; }
            set
            {
                _treeSourceFiltered=value;
                OnPropertyChanged(nameof(TreeSourceFiltered));
            }
        }

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { _searchText = value;
                OnPropertyChanged(nameof(TreeSource));
                OnPropertyChanged(nameof(SearchText));
            }
        }
        //private bool DoesTreeSubjectContain(object subject)
        //{
        //    SubjectTreeViewModel subjectTree = (SubjectTreeViewModel)subject;
        //    return subjectTree.Subject.Login.ToLower().Contains(SearchText.ToLower());
        //}
        private bool DoesSubjectContain(SubjectViewModel subject)
        {
            if (SearchText == null)
                return true;
            return subject.Login.ToLower().Contains(SearchText.ToLower());
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
                    ChosenObjectSecurityMark = s.Object.SecurityMarkName;
                    ChosenObjectSecurityCategory = s.Object.SecurityCategoryName;
                    IsObjectOrSubject = LocalizedStrings.Instance["textBlockOBJECTAccessMatrix"];
                }
                else if(value is SubjectTreeViewModel subj)
                {
                     
                    IsReadable=false;
                    IsWritable=false;
                    IsExecutable=false;

                    ChosenObjectName = subj.Subject.Login;
                    ChosenObjectSecurityMark = subj.Subject.SecurityMarkName;
                    ChosenObjectSecurityCategory = subj.Subject.SecurityCategoryName;
                    subj.Subject.SecurityCategory.ToString();
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
        private string _chosenObjectSecurityMark;
        public string ChosenObjectSecurityMark
        {
            get { return _chosenObjectSecurityMark; }
            set { _chosenObjectSecurityMark = value;
                OnPropertyChanged(nameof(ChosenObjectSecurityMark));
            }
        }
        private string _chosenObjectSecurityCategory;
        public string ChosenObjectSecurityCategory
        {
            get { return _chosenObjectSecurityCategory; }
            set {
                _chosenObjectSecurityCategory = value;
                OnPropertyChanged(nameof(ChosenObjectSecurityCategory));
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
            var i = Subjects.FirstOrDefault(x => x.Login == login);
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
