using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.ViewModel
{
    public class SubjectViewModel : ViewModelBase
    {
        private Model.Subject _subject;

        public SubjectViewModel(Model.Subject subj)
        {
            this._subject = subj;
        }
        public SubjectViewModel() { _subject = new Subject(); }

        public Subject GetSubject() => _subject;

        public string Login { get { return _subject.Login; }
            set { _subject.Login = value; OnPropertyChanged(nameof(Login)); }
        }
        public string Password
        {
            get { return _subject.Password; }
            set { _subject.Password = value; OnPropertyChanged(nameof(Password)); }
        }

        public SecurityMark SecurityMark
        {
            get { return _subject.SecurityMark; }
            set {
                _subject.SecurityMark = value;
                OnPropertyChanged(nameof(SecurityMarkName));
                OnPropertyChanged(nameof(SecurityMark));
            }
        }
        public string SecurityMarkName { get => LocalizedStrings.Instance[$"SecurityMark{SecurityMark.ToString()}"]; }
        public string Name
        {
            get { return _subject.Name; }
            set { _subject.Name = value; 
                OnPropertyChanged(nameof(Name)); }
        }
        public string SecondName
        {
            get { return _subject.SecondName; }
            set { _subject.SecondName = value; 
                OnPropertyChanged(nameof(SecondName)); }
        }
    }
}
