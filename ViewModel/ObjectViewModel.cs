using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using MultipleUserLoginForm.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultipleUserLoginForm.ViewModel
{
    public class ObjectViewModel:ViewModelBase
    {
        private Model.Object _object;

        public ObjectViewModel() { _object=new Model.Object(); }

        static Dictionary<string, string> _iconsDictionary = new Dictionary<string, string>();

        static ObjectViewModel()
        {
            string path=Directory.GetCurrentDirectory();

            FileIO<Dictionary<string,string>> _fileIO = new FileIO<Dictionary<string, string>>(path+"/ObjectIcons.json");
            _iconsDictionary=_fileIO.LoadData();
        }

        public ObjectViewModel(Model.Object o)
        {
            _object = o;
        }
        public Model.Object GetObject() { return _object; }
        public string Name {get{ return _object.Name; } set{
                _object.Name = value; OnPropertyChanged(nameof(Name)); 
            } 
        }
        public SecurityMark SecurityMark {get{ return _object.SecurityMark; } set{
                
                _object.SecurityMark = value;
                OnPropertyChanged(nameof(SecurityMark));
            }
        }
        public string SecurityMarkName { get => LocalizedStrings.Instance[$"SecurityMark{SecurityMark.ToString()}"]; }
        public string ObjectIconPath { get {
                string extension = System.IO.Path.GetExtension(Path);
                string iconPath = _iconsDictionary.GetValueOrDefault(extension);
                if(iconPath != null)
                    return iconPath;
                return _iconsDictionary["default"];
                    } }
        public string Path { get { return _object.Path; } set { 
                _object.Path = value; OnPropertyChanged(nameof(Path)); 
            } 
        }

        
    }
}
