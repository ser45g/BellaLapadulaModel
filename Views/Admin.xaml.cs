using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;


//using Microsoft.Office.Interop.Word;

namespace MultipleUserLoginForm.Views
{
    /// <summary>
    /// Interaction logic for UserCabinet.xaml
    /// </summary>
    public partial class Admin : UserControl
    {
        public string CurrentPath { get; set; }
        public Admin()
        {
            InitializeComponent();
            CurrentPath = Directory.GetCurrentDirectory()+"\\info";


        }

        private void rbRussian_Checked(object sender, RoutedEventArgs e)
        {
          
            //SetDocumentViewer(CurrentPath + $"\\info.{Properties.Settings.Default.CurrentCulture}.pdf");
        }

        private void rbEnglish_Checked(object sender, RoutedEventArgs e)
        {
            //webBrowser.Navigate("file:///"+CurrentPath + $"\\info.{Properties.Settings.Default.CurrentCulture}.pdf");
            //SetDocumentViewer(CurrentPath + $"\\info.{Properties.Settings.Default.CurrentCulture}.pdf");
        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Name;
            if(tabItem == "info")
            {
                webBrowser.Navigate("file:///" + CurrentPath + $"\\info.{Properties.Settings.Default.CurrentCulture}.pdf");

            }
        }

       


        //private void SetDocumentViewer(string pathPdf)
        //{   
        //    using(FileStream pdfFile=new FileStream(pathPdf, FileMode.Open, FileAccess.Read))
        //    {
        //        RenderSettings renderSettings=new RenderSettings();
        //        ConvertToWpfOptions convertToWpfOptions=new ConvertToWpfOptions();
        //        Document document = new Document(pdfFile);
        //        FixedDocument fixedDocument=document.ConvertToWpf(renderSettings, convertToWpfOptions);
        //        docViewer.Document = fixedDocument;
        //        //XpsDocument xpsDocument = new XpsDocument(CurrentPath, FileAccess.Read);
        //
        //    }
        //
        //}
    }
}
