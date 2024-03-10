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

        public ObjectViewModel(Model.Object o)
        {
            _object = o;
        }
        public string Name {get{ return _object.Name; } set{
                _object.Name = value; OnPropertyChanged(nameof(Name)); 
            } 
        }
        public SecurityMark SecurityMark {get{ return _object.SecurityMark; } set{
                
                _object.SecurityMark = value;
                OnPropertyChanged(nameof(SecurityMark));
            }
        }
        public string Path { get { return _object.Path; } set { 
                _object.Path = value; OnPropertyChanged(nameof(Path)); 
            } 
        }
    }
}
