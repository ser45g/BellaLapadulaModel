using MultipleUserLoginForm.LocalizationHelper;
using MultipleUserLoginForm.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleUserLoginForm.Stores
{
    public class TitleStore:ViewModelBase
    {
        public static TitleStore Instance { get; } = new TitleStore();

		private string _title;

		public string Title
		{
			get { return _title; }
			set { _title = value;
				OnPropertyChanged(nameof(Title));
			}
		}

	}
}
