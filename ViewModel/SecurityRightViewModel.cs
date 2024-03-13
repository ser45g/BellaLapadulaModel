using MultipleUserLoginForm.Commands;
using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Services;
using MultipleUserLoginForm.Stores;
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
    public class SecurityRightViewModel:ViewModelBase
    {
        private ObjectViewModel _object;
        public ObjectViewModel Object { get { return _object; } set { _object = value; OnPropertyChanged(nameof(Object)); } }
        private SubjectViewModel _subject;
        public SubjectViewModel Subject { get { return _subject; } set { _subject = value; OnPropertyChanged(nameof(Subject)); } }
        public Right Right { get; set; }
        public string RightName { get => LocalizedStrings.Instance[Right.ToString()]; }

        public ICommand ReadCommand { get; set; }
        public ICommand WriteCommand { get; set;}
        public ICommand ExecuteCommand { get;set; }

        public bool IsReadable => Right == Right.Read || Right == Right.All;
        public bool IsWritable => Right == Right.Write || Right == Right.All;
        public bool IsExecutable => Right == Right.Execute || Right == Right.All;
        public SecurityRightViewModel() { }

    }
}
