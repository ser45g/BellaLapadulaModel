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
    public partial class Admin : UserControl,IDisposable
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
            if(tabItem == "info") {
                string modelType = "";
                if (rbBelLap.IsChecked==true)
                {
                    modelType = "Bella-Lapadula";
                }
                if (rbBiba.IsChecked==true)
                {
                    modelType = "Biba";
                }
                if (rbCombined.IsChecked==true)
                {
                    modelType = "Combined";
                }
                string path = @$"{CurrentPath}" + @$"\info{modelType}.{Properties.Settings.Default.CurrentCulture}.pdf";
                //webBrowser = new Microsoft.Web.WebView2.Wpf.WebView2();
                
                webBrowser.Source = new Uri(path);
                //@"C:\Users\HP\Desktop\6 sem\Курсач 2024\MINE\BellLapadule\bin\Release\net6.0-windows7.0\info\infoBiba.en-US.pdf"
            }
        }

        public void Dispose()
        {
            webBrowser.Dispose();
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
