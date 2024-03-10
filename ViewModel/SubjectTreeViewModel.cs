using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.ViewModel
{
    public class SubjectTreeViewModel
    {
        public SubjectViewModel Subject { get; }
        public ObservableCollection<SecurityRightViewModel> Objects { get; }

        public SubjectTreeViewModel(SubjectViewModel subject, MatricesViewModel matrix)
        {
            this.Subject = subject;
            Objects = new ObservableCollection<SecurityRightViewModel>(matrix.GetObjectsForSubj(Subject.Login));
        }

    }
}
