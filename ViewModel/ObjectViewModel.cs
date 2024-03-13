using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.Model;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.ViewModel
{
    public class ObjectViewModel:ViewModelBase
    {
        private Model.Object _object;

        public ObjectViewModel() { _object=new Model.Object(); }

        static Dictionary<string, string> _iconsDictionary = new Dictionary<string, string>();

        static ObjectViewModel()
        {
            _iconsDictionary.Add(".txt", "/ObjectIcons/txt.png");
            _iconsDictionary.Add("default", "/ObjectIcons/unknown.png");
            _iconsDictionary.Add(".docx", "/ObjectIcons/doc.png");
            _iconsDictionary.Add(".doc", "/ObjectIcons/doc.png");
            _iconsDictionary.Add(".pdf", "/ObjectIcons/pdf.png");
            _iconsDictionary.Add(".xlsx", "/ObjectIcons/xlsx.png");
            _iconsDictionary.Add(".rtf", "/ObjectIcons/rtf.png");
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
