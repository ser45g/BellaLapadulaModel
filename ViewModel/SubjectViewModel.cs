using MultipleUserLoginForm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.ViewModel
{
    public class SubjectViewModel:ViewModelBase
    {
        private Model.Subject _subject;
        public SubjectViewModel(Model.Subject subj)
        {
            this._subject = subj;
        }
        public SubjectViewModel() { _subject = new Subject(); }
        public string Login { get {return _subject.Login; }
            set { _subject.Login = value; OnPropertyChanged(nameof(Login)); }
        }
        public string Password
        {
            get { return _subject.Password; }
            set { _subject.Password = value; OnPropertyChanged(nameof(Password)); }
        }
        /// <summary>
        /// not cool, I better improve it
        /// </summary>
        public SecurityMark SecurityMark
        {
            get { return _subject.SecurityMark; }
            set {
                _subject.SecurityMark = value;
                OnPropertyChanged(nameof(SecurityMark));
            }
        }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }
        private string _secondName;
        public string SecondName
        {
            get { return _secondName; }
            set { _secondName = value; OnPropertyChanged(nameof(SecondName)); }
        }
    }
}
