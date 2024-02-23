using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultipleUserLoginForm.ViewModel
{
    public class MatricesViewModel:ViewModelBase,IDisposable
    {
        public enum ModelType
        {
            BellaLapadula, Biba, Combined
        }
        private ModelType _modelType;

        public ModelType CurrentModelType
        {
            get { return _modelType; }
            set { _modelType = value; }
        }

        public ObservableCollection<string> Log { get; set; } = new ObservableCollection<string>();

        public MatricesViewModel()
        {
            objects = new ObservableCollection<ObjectViewModel>();
            subjects = new ObservableCollection<SubjectViewModel>();


            string path = Environment.CurrentDirectory;
            _fileIOAdmins = new FileIO<List<SubjectViewModel>>(path+@"\Admins.json");
            _fileIOObj = new FileIO<List<ObjectViewModel>>(path + @"\Objects.json");
            _fileIOSubj = new FileIO<List<SubjectViewModel>>(path + @"\Subjects.json");
            _fileIOLog= new FileIO<List<string>>(path+@"Log.json");
            Load();
            UpdateRights();
        }

        private void Load()
        {
            try
            {
                objects= new ObservableCollection<ObjectViewModel>(_fileIOObj.LoadData());
                subjects= new ObservableCollection<SubjectViewModel>(_fileIOSubj.LoadData());
                Admins= _fileIOAdmins.LoadData();
                //Log= new ObservableCollection<string>(_fileIOLog.LoadData());
            }catch (Exception ex)
            {
                MessageBox.Show("Cannot load data", ex.Message);
                Application.Current.Shutdown();
            }
        }

        private ObservableCollection<SubjectViewModel> subjects;
        public ObservableCollection<SubjectViewModel> Subjects { get { return subjects; } private set { subjects = value; OnPropertyChanged(nameof(Subjects)); } }
       
        private ObservableCollection<ObjectViewModel>  objects;
        private bool disposedValue;

        public ObservableCollection<ObjectViewModel> Objects { get { return objects; } private set { objects = value; OnPropertyChanged(nameof(Objects)); } }

        public List<SecurityRightViewModel> Rights { get; private set; }


        private FileIO<List<SubjectViewModel>> _fileIOSubj;
        private FileIO<List<SubjectViewModel>> _fileIOAdmins;
        private FileIO<List<ObjectViewModel>> _fileIOObj;
        private FileIO<List<string>> _fileIOLog; 
        public SubjectViewModel GetSubject(string login)
        {
            try
            {
                return Subjects.SingleOrDefault((obj) => obj.Login == login);

            }catch (Exception ex)
            {
                return new SubjectViewModel(new Subject());
            }
        }
        public ObjectViewModel GetObject(string name)
        {
            try
            {
                return Objects.Single(x => x.Name == (name));

            }
            catch (Exception ex)
            {
                return new ObjectViewModel(new Model.Object());
            }
        }
        public List<SecurityRightViewModel> GetObjectsForSubj(string login)
        {


            SubjectViewModel s = GetSubject(login);

            List<SecurityRightViewModel> l = new List<SecurityRightViewModel>();
            for (int j = 0; j < Objects.Count; ++j)
            {
                l.Add(GetRight(s, Objects[j]));
            }
            return l;
        }
        public void UpdateRights()
        {
            List<SecurityRightViewModel> securityRights = new List<SecurityRightViewModel>(Subjects.Count * Objects.Count);
            int k = 0;
            for (int i = 0; i < Subjects.Count; ++i)
            {
                for (int j = 0; j < Objects.Count; ++j)
                {
                    securityRights.Add( GetRight(Subjects[i], Objects[j]));
                }
            }
            Rights = securityRights;

            //return (from o in Objects where o.SecurityMark <= s.SecurityMark 
            //        select new SecurityRight() {Object=o,Subject=s,Right=Right.All }).ToList<SecurityRight>(); 
        }
        public void AddSubject(SubjectViewModel subject)
        {
            Subjects.Add(subject);
            UpdateRights();
        }
        public void RemoveSubject(SubjectViewModel subject)
        {
            Subjects.Remove(subject);
            UpdateRights();
        }
        public void AddObject(ObjectViewModel obj)
        {
            Objects.Add(obj);
            UpdateRights();
        }
        public void RemoveObject(ObjectViewModel obj)
        {
            Objects.Remove(obj);
            UpdateRights();
        }
        SecurityRightViewModel GetRight(SubjectViewModel s, ObjectViewModel o)
        {
            SecurityRightViewModel right = null;
            switch (CurrentModelType)
            {
                case ModelType.BellaLapadula:
                    right = ImplyBellaLapaduleModel(s, o);
                    break;
                case ModelType.Biba: right = ImplyBibaModel(s, o); break;
                case ModelType.Combined: right = ImplyCombinedModel(s, o); break;

            }
            return right;
        }
        SecurityRightViewModel ImplyBellaLapaduleModel(SubjectViewModel s, ObjectViewModel o)
        {
            Right right;
            if (s.SecurityMark > o.SecurityMark)
            {
                right = Right.Read;
            }
            else if (s.SecurityMark == o.SecurityMark)
            {
                right = Right.All;
            }
            else
            {
                right = Right.Write;
            }
            return new SecurityRightViewModel() { Object = o, Subject = s, Right = right };


        }
        SecurityRightViewModel ImplyBibaModel(SubjectViewModel s, ObjectViewModel o)
        {
            Right right;
            if (s.SecurityMark > o.SecurityMark)
            {
                right = Right.Write;
            }
            else if (s.SecurityMark == o.SecurityMark)
            {
                right = Right.All;
            }
            else
            {
                right = Right.Read;
            }
            return new SecurityRightViewModel() { Object = o, Subject = s, Right = right };


        }
        SecurityRightViewModel ImplyCombinedModel(SubjectViewModel s, ObjectViewModel o)
        {
            Right right;
            if (s.SecurityMark == o.SecurityMark)
            {
                right = Right.All;
            }
            else
            {
                right = Right.None;
            }
            return new SecurityRightViewModel(){ Object = o, Subject = s, Right = right };

        }
        public List<SubjectViewModel> Admins { get; set; }=new List<SubjectViewModel>();

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _fileIOAdmins.SaveData(Admins);
                    _fileIOObj.SaveData(objects.ToList());
                    _fileIOSubj.SaveData(subjects.ToList());   
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MatricesViewModel()
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
