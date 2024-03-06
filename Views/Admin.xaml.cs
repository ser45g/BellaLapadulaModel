


using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using System.Xml;
using System.Xml.Linq;

//using Microsoft.Office.Interop.Word;

namespace MultipleUserLoginForm.Views
{
    /// <summary>
    /// Interaction logic for UserCabinet.xaml
    /// </summary>
    public partial class Admin : UserControl
    {
        XpsDocument xps;
        public Admin()
        {
            InitializeComponent();
            
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = ".docx",
                //Filter = "Word documents (.docx)|*.docx"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                //GemBox.Document.ComponentInfo.SetLicense("DZVK-3AYJ-X30B-X171");
                //var document=GemBox.Document.DocumentModel.Load(openFileDialog.FileName);
                //xps = document.ConvertToXpsDocument(GemBox.Document.XpsSaveOptions.XpsDefault);
                //docViewer.Document=xps.GetFixedDocumentSequence();

                moonReader.OpenFile(openFileDialog.FileName);
                moonReader.ZoomToWidth();
            }
        }


        private void OnOpenFileClicked()
        {
            var openFileDialog = new OpenFileDialog()
            {
                DefaultExt = ".docx",
                Filter = "Word documents (.docx)|*.docx"
            };

            if (openFileDialog.ShowDialog() == true)
                this.ReadDocx(openFileDialog.FileName);
        }

        private void ReadDocx(string path)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var flowDocumentConverter = new DocxToFlowDocumentConverter(stream);
                flowDocumentConverter.Read();
                //richTextBox.Document = flowDocumentConverter.Document;
                //this.Title = Path.GetFileName(path);
            }
        }

        //string ReadWord(string fileName)
        //{
        //    object file = fileName;
        //    object readOnly = true;
        //    object addToRecentFiles = false;
        //    Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
        //    Microsoft.Office.Interop.Word.Document doc=app.Documents.Open(ref file,ReadOnly: readOnly,AddToRecentFiles:addToRecentFiles);
        //    richTextBox.Document = doc
        //    return "";
        //}
        //void SetAccessMatrics()
        //{
        //    IEnumerable<string> source = (IEnumerable<string>)datgrdAccessMatrice.ItemsSource;
        //    foreach (string item in source)
        //    {
        //        DataGridTextColumn column = new DataGridTextColumn() { Header="jj"};
        //    }
        //}

    }

    public class DocxReader : IDisposable
    {
        protected const string

            MainDocumentRelationshipType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument",

            // XML namespaces
            WordprocessingMLNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main",
            RelationshipsNamespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships",

            // Miscellaneous elements
            DocumentElement = "document",
            BodyElement = "body",

            // Block-Level elements
            ParagraphElement = "p",
            TableElement = "tbl",

            // Inline-Level elements
            SimpleFieldElement = "fldSimple",
            HyperlinkElement = "hyperlink",
            RunElement = "r",

            // Run content elements
            BreakElement = "br",
            TabCharacterElement = "tab",
            TextElement = "t",

            // Table elements
            TableRowElement = "tr",
            TableCellElement = "tc",

            // Properties elements
            ParagraphPropertiesElement = "pPr",
            RunPropertiesElement = "rPr";
        // Note: new members should also be added to nameTable in CreateNameTable method.

        protected virtual XmlNameTable CreateNameTable()
        {
            var nameTable = new NameTable();

            nameTable.Add(WordprocessingMLNamespace);
            nameTable.Add(RelationshipsNamespace);
            nameTable.Add(DocumentElement);
            nameTable.Add(BodyElement);
            nameTable.Add(ParagraphElement);
            nameTable.Add(TableElement);
            nameTable.Add(ParagraphPropertiesElement);
            nameTable.Add(SimpleFieldElement);
            nameTable.Add(HyperlinkElement);
            nameTable.Add(RunElement);
            nameTable.Add(BreakElement);
            nameTable.Add(TabCharacterElement);
            nameTable.Add(TextElement);
            nameTable.Add(RunPropertiesElement);
            nameTable.Add(TableRowElement);
            nameTable.Add(TableCellElement);

            return nameTable;
        }

        private readonly Package package;
        private readonly PackagePart mainDocumentPart;

        protected PackagePart MainDocumentPart
        {
            get { return this.mainDocumentPart; }
        }

        public DocxReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            this.package = Package.Open(stream, FileMode.Open, FileAccess.Read);

            foreach (var relationship in this.package.GetRelationshipsByType(MainDocumentRelationshipType))
            {
                this.mainDocumentPart = package.GetPart(PackUriHelper.CreatePartUri(relationship.TargetUri));
                break;
            }
        }

        public void Read()
        {
            using (var mainDocumentStream = this.mainDocumentPart.GetStream(FileMode.Open, FileAccess.Read))
            using (var reader = XmlReader.Create(mainDocumentStream, new XmlReaderSettings()
            {
                NameTable = this.CreateNameTable(),
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true
            }))
                this.ReadMainDocument(reader);
        }

        private static void ReadXmlSubtree(XmlReader reader, Action<XmlReader> action)
        {
            using (var subtreeReader = reader.ReadSubtree())
            {
                // Position on the first node.
                subtreeReader.Read();

                if (action != null)
                    action(subtreeReader);
            }
        }

        private void ReadMainDocument(XmlReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace && reader.LocalName == DocumentElement)
                {
                    ReadXmlSubtree(reader, this.ReadDocument);
                    break;
                }
        }

        protected virtual void ReadDocument(XmlReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace && reader.LocalName == BodyElement)
                {
                    ReadXmlSubtree(reader, this.ReadBody);
                    break;
                }
        }

        private void ReadBody(XmlReader reader)
        {
            while (reader.Read())
                this.ReadBlockLevelElement(reader);
        }

        private void ReadBlockLevelElement(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                Action<XmlReader> action = null;

                if (reader.NamespaceURI == WordprocessingMLNamespace)
                    switch (reader.LocalName)
                    {
                        case ParagraphElement:
                            action = this.ReadParagraph;
                            break;

                        case TableElement:
                            action = this.ReadTable;
                            break;
                    }

                ReadXmlSubtree(reader, action);
            }
        }

        protected virtual void ReadParagraph(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace && reader.LocalName == ParagraphPropertiesElement)
                    ReadXmlSubtree(reader, this.ReadParagraphProperties);
                else
                    this.ReadInlineLevelElement(reader);
            }
        }

        protected virtual void ReadParagraphProperties(XmlReader reader)
        {

        }

        private void ReadInlineLevelElement(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                Action<XmlReader> action = null;

                if (reader.NamespaceURI == WordprocessingMLNamespace)
                    switch (reader.LocalName)
                    {
                        case SimpleFieldElement:
                            action = this.ReadSimpleField;
                            break;

                        case HyperlinkElement:
                            action = this.ReadHyperlink;
                            break;

                        case RunElement:
                            action = this.ReadRun;
                            break;
                    }

                ReadXmlSubtree(reader, action);
            }
        }

        private void ReadSimpleField(XmlReader reader)
        {
            while (reader.Read())
                this.ReadInlineLevelElement(reader);
        }

        protected virtual void ReadHyperlink(XmlReader reader)
        {
            while (reader.Read())
                this.ReadInlineLevelElement(reader);
        }

        protected virtual void ReadRun(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace && reader.LocalName == RunPropertiesElement)
                    ReadXmlSubtree(reader, this.ReadRunProperties);
                else
                    this.ReadRunContentElement(reader);
            }
        }

        protected virtual void ReadRunProperties(XmlReader reader)
        {

        }

        private void ReadRunContentElement(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                Action<XmlReader> action = null;

                if (reader.NamespaceURI == WordprocessingMLNamespace)
                    switch (reader.LocalName)
                    {
                        case BreakElement:
                            action = this.ReadBreak;
                            break;

                        case TabCharacterElement:
                            action = this.ReadTabCharacter;
                            break;

                        case TextElement:
                            action = this.ReadText;
                            break;
                    }

                ReadXmlSubtree(reader, action);
            }
        }

        protected virtual void ReadBreak(XmlReader reader)
        {

        }

        protected virtual void ReadTabCharacter(XmlReader reader)
        {

        }

        protected virtual void ReadText(XmlReader reader)
        {

        }

        protected virtual void ReadTable(XmlReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace && reader.LocalName == TableRowElement)
                    ReadXmlSubtree(reader, this.ReadTableRow);
        }

        protected virtual void ReadTableRow(XmlReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace && reader.LocalName == TableCellElement)
                    ReadXmlSubtree(reader, this.ReadTableCell);
        }

        protected virtual void ReadTableCell(XmlReader reader)
        {
            while (reader.Read())
                this.ReadBlockLevelElement(reader);
        }

        public void Dispose()
        {
            this.package.Close();
        }
    }
    public class DocxToFlowDocumentConverter : DocxReader
    {
        private const string

            // Run properties elements
            BoldElement = "b",
            ItalicElement = "i",
            UnderlineElement = "u",
            StrikeElement = "strike",
            DoubleStrikeElement = "dstrike",
            VerticalAlignmentElement = "vertAlign",
            ColorElement = "color",
            HighlightElement = "highlight",
            FontElement = "rFonts",
            FontSizeElement = "sz",
            RightToLeftTextElement = "rtl",

            // Paragraph properties elements
            AlignmentElement = "jc",
            PageBreakBeforeElement = "pageBreakBefore",
            SpacingElement = "spacing",
            IndentationElement = "ind",
            ShadingElement = "shd",

            // Attributes
            IdAttribute = "id",
            ValueAttribute = "val",
            ColorAttribute = "color",
            AsciiFontFamily = "ascii",
            SpacingAfterAttribute = "after",
            SpacingBeforeAttribute = "before",
            LeftIndentationAttribute = "left",
            RightIndentationAttribute = "right",
            HangingIndentationAttribute = "hanging",
            FirstLineIndentationAttribute = "firstLine",
            FillAttribute = "fill";
        // Note: new members should also be added to nameTable in CreateNameTable method.

        private FlowDocument document;
        private TextElement current;
        private bool hasAnyHyperlink;

        public FlowDocument Document
        {
            get { return this.document; }
        }

        public DocxToFlowDocumentConverter(Stream stream)
            : base(stream)
        {
        }

        protected override XmlNameTable CreateNameTable()
        {
            var nameTable = base.CreateNameTable();

            nameTable.Add(BoldElement);
            nameTable.Add(ItalicElement);
            nameTable.Add(UnderlineElement);
            nameTable.Add(StrikeElement);
            nameTable.Add(DoubleStrikeElement);
            nameTable.Add(VerticalAlignmentElement);
            nameTable.Add(ColorElement);
            nameTable.Add(HighlightElement);
            nameTable.Add(FontElement);
            nameTable.Add(FontSizeElement);
            nameTable.Add(RightToLeftTextElement);
            nameTable.Add(AlignmentElement);
            nameTable.Add(PageBreakBeforeElement);
            nameTable.Add(SpacingElement);
            nameTable.Add(IndentationElement);
            nameTable.Add(ShadingElement);
            nameTable.Add(IdAttribute);
            nameTable.Add(ValueAttribute);
            nameTable.Add(ColorAttribute);
            nameTable.Add(AsciiFontFamily);
            nameTable.Add(SpacingAfterAttribute);
            nameTable.Add(SpacingBeforeAttribute);
            nameTable.Add(LeftIndentationAttribute);
            nameTable.Add(RightIndentationAttribute);
            nameTable.Add(HangingIndentationAttribute);
            nameTable.Add(FirstLineIndentationAttribute);
            nameTable.Add(FillAttribute);

            return nameTable;
        }

        protected override void ReadDocument(XmlReader reader)
        {
            this.document = new FlowDocument();
            this.document.BeginInit();
            this.document.ColumnWidth = double.NaN;

            base.ReadDocument(reader);

            if (this.hasAnyHyperlink)
                this.document.AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler((sender, e) => Process.Start(e.Uri.ToString())));

            this.document.EndInit();
        }

        protected override void ReadParagraph(XmlReader reader)
        {
            using (this.SetCurrent(new Paragraph()))
                base.ReadParagraph(reader);
        }

        protected override void ReadTable(XmlReader reader)
        {
            // Skip table for now.
        }

        protected override void ReadParagraphProperties(XmlReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace)
                {
                    var paragraph = (Paragraph)this.current;
                    switch (reader.LocalName)
                    {
                        case AlignmentElement:
                            var textAlignment = ConvertTextAlignment(GetValueAttribute(reader));
                            if (textAlignment.HasValue)
                                paragraph.TextAlignment = textAlignment.Value;
                            break;
                        case PageBreakBeforeElement:
                            paragraph.BreakPageBefore = GetOnOffValueAttribute(reader);
                            break;
                        case SpacingElement:
                            paragraph.Margin = GetSpacing(reader, paragraph.Margin);
                            break;
                        case IndentationElement:
                            SetParagraphIndent(reader, paragraph);
                            break;
                        case ShadingElement:
                            var background = GetShading(reader);
                            if (background != null)
                                paragraph.Background = background;
                            break;
                    }
                }
        }

        protected override void ReadHyperlink(XmlReader reader)
        {
            var id = reader[IdAttribute, RelationshipsNamespace];
            if (!string.IsNullOrEmpty(id))
            {
                var relationship = this.MainDocumentPart.GetRelationship(id);
                if (relationship.TargetMode == TargetMode.External)
                {
                    this.hasAnyHyperlink = true;

                    var hyperlink = new Hyperlink()
                    {
                        NavigateUri = relationship.TargetUri
                    };

                    using (this.SetCurrent(hyperlink))
                        base.ReadHyperlink(reader);
                    return;
                }
            }

            base.ReadHyperlink(reader);
        }

        protected override void ReadRun(XmlReader reader)
        {
            using (this.SetCurrent(new Span()))
                base.ReadRun(reader);
        }

        protected override void ReadRunProperties(XmlReader reader)
        {
            while (reader.Read())
                if (reader.NodeType == XmlNodeType.Element && reader.NamespaceURI == WordprocessingMLNamespace)
                {
                    var inline = (Inline)this.current;
                    switch (reader.LocalName)
                    {
                        case BoldElement:
                            inline.FontWeight = GetOnOffValueAttribute(reader) ? FontWeights.Bold : FontWeights.Normal;
                            break;
                        case ItalicElement:
                            inline.FontStyle = GetOnOffValueAttribute(reader) ? FontStyles.Italic : FontStyles.Normal;
                            break;
                        case UnderlineElement:
                            var underlineTextDecorations = GetUnderlineTextDecorations(reader, inline);
                            if (underlineTextDecorations != null)
                                inline.TextDecorations.Add(underlineTextDecorations);
                            break;
                        case StrikeElement:
                            if (GetOnOffValueAttribute(reader))
                                inline.TextDecorations.Add(TextDecorations.Strikethrough);
                            break;
                        case DoubleStrikeElement:
                            if (GetOnOffValueAttribute(reader))
                            {
                                inline.TextDecorations.Add(new TextDecoration() { Location = TextDecorationLocation.Strikethrough, PenOffset = this.current.FontSize * 0.015 });
                                inline.TextDecorations.Add(new TextDecoration() { Location = TextDecorationLocation.Strikethrough, PenOffset = this.current.FontSize * -0.015 });
                            }
                            break;
                        case VerticalAlignmentElement:
                            var baselineAlignment = GetBaselineAlignment(GetValueAttribute(reader));
                            if (baselineAlignment.HasValue)
                            {
                                inline.BaselineAlignment = baselineAlignment.Value;
                                if (baselineAlignment.Value == BaselineAlignment.Subscript || baselineAlignment.Value == BaselineAlignment.Superscript)
                                    inline.FontSize *= 0.65; //MS Word 2002 size: 65% http://en.wikipedia.org/wiki/Subscript_and_superscript
                            }
                            break;
                        case ColorElement:
                            var color = GetColor(GetValueAttribute(reader));
                            if (color.HasValue)
                                inline.Foreground = new SolidColorBrush(color.Value);
                            break;
                        case HighlightElement:
                            var highlight = GetHighlightColor(GetValueAttribute(reader));
                            if (highlight.HasValue)
                                inline.Background = new SolidColorBrush(highlight.Value);
                            break;
                        case FontElement:
                            var fontFamily = reader[AsciiFontFamily, WordprocessingMLNamespace];
                            if (!string.IsNullOrEmpty(fontFamily))
                                inline.FontFamily = (FontFamily)new FontFamilyConverter().ConvertFromString(fontFamily);
                            break;
                        case FontSizeElement:
                            var fontSize = reader[ValueAttribute, WordprocessingMLNamespace];
                            if (!string.IsNullOrEmpty(fontSize))
                                // Attribute Value / 2 = Points
                                // Points * (96 / 72) = Pixels
                                inline.FontSize = uint.Parse(fontSize) * 0.6666666666666667;
                            break;
                        case RightToLeftTextElement:
                            inline.FlowDirection = (GetOnOffValueAttribute(reader)) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
                            break;
                    }
                }
        }

        protected override void ReadBreak(XmlReader reader)
        {
            this.AddChild(new LineBreak());
        }

        protected override void ReadTabCharacter(XmlReader reader)
        {
            this.AddChild(new Run("\t"));
        }

        protected override void ReadText(XmlReader reader)
        {
            this.AddChild(new Run(reader.ReadString()));
        }

        private void AddChild(TextElement textElement)
        {
            ((IAddChild)this.current ?? this.document).AddChild(textElement);
        }

        private static bool GetOnOffValueAttribute(XmlReader reader)
        {
            var value = GetValueAttribute(reader);

            switch (value)
            {
                case null:
                case "1":
                case "on":
                case "true":
                    return true;
                default:
                    return false;
            }
        }

        private static string GetValueAttribute(XmlReader reader)
        {
            return reader[ValueAttribute, WordprocessingMLNamespace];
        }

        private static Color? GetColor(string colorString)
        {
            if (string.IsNullOrEmpty(colorString) || colorString == "auto")
                return null;

            return (Color)ColorConverter.ConvertFromString('#' + colorString);
        }

        private static Color? GetHighlightColor(string highlightString)
        {
            if (string.IsNullOrEmpty(highlightString) || highlightString == "auto")
                return null;

            return (Color)ColorConverter.ConvertFromString(highlightString);
        }

        private static BaselineAlignment? GetBaselineAlignment(string verticalAlignmentString)
        {
            switch (verticalAlignmentString)
            {
                case "baseline":
                    return BaselineAlignment.Baseline;
                case "subscript":
                    return BaselineAlignment.Subscript;
                case "superscript":
                    return BaselineAlignment.Superscript;
                default:
                    return null;
            }
        }

        private static double? ConvertTwipsToPixels(string twips)
        {
            if (string.IsNullOrEmpty(twips))
                return null;
            else
                return ConvertTwipsToPixels(double.Parse(twips, CultureInfo.InvariantCulture));
        }

        private static double ConvertTwipsToPixels(double twips)
        {
            return 96d / (72 * 20) * twips;
        }

        private static TextAlignment? ConvertTextAlignment(string value)
        {
            switch (value)
            {
                case "both":
                    return TextAlignment.Justify;
                case "left":
                    return TextAlignment.Left;
                case "right":
                    return TextAlignment.Right;
                case "center":
                    return TextAlignment.Center;
                default:
                    return null;
            }
        }

        private static Thickness GetSpacing(XmlReader reader, Thickness margin)
        {
            var after = ConvertTwipsToPixels(reader[SpacingAfterAttribute, WordprocessingMLNamespace]);
            if (after.HasValue)
                margin.Bottom = after.Value;

            var before = ConvertTwipsToPixels(reader[SpacingBeforeAttribute, WordprocessingMLNamespace]);
            if (before.HasValue)
                margin.Top = before.Value;

            return margin;
        }

        private static void SetParagraphIndent(XmlReader reader, Paragraph paragraph)
        {
            var margin = paragraph.Margin;

            var left = ConvertTwipsToPixels(reader[LeftIndentationAttribute, WordprocessingMLNamespace]);
            if (left.HasValue)
                margin.Left = left.Value;

            var right = ConvertTwipsToPixels(reader[RightIndentationAttribute, WordprocessingMLNamespace]);
            if (right.HasValue)
                margin.Right = right.Value;

            paragraph.Margin = margin;

            var firstLine = ConvertTwipsToPixels(reader[FirstLineIndentationAttribute, WordprocessingMLNamespace]);
            if (firstLine.HasValue)
                paragraph.TextIndent = firstLine.Value;

            var hanging = ConvertTwipsToPixels(reader[HangingIndentationAttribute, WordprocessingMLNamespace]);
            if (hanging.HasValue)
                paragraph.TextIndent -= hanging.Value;
        }

        private static Brush GetShading(XmlReader reader)
        {
            var color = GetColor(reader[FillAttribute, WordprocessingMLNamespace]);
            return color.HasValue ? new SolidColorBrush(color.Value) : null;
        }

        private static TextDecorationCollection GetUnderlineTextDecorations(XmlReader reader, Inline inline)
        {
            TextDecoration textDecoration;
            Brush brush;
            var color = GetColor(reader[ColorAttribute, WordprocessingMLNamespace]);

            if (color.HasValue)
                brush = new SolidColorBrush(color.Value);
            else
                brush = inline.Foreground;

            var textDecorations = new TextDecorationCollection()
            {
                (textDecoration = new TextDecoration()
                {
                    Location = TextDecorationLocation.Underline,
                    Pen = new Pen()
                    {
                        Brush = brush
                    }
                })
            };

            switch (GetValueAttribute(reader))
            {
                case "single":
                    break;
                case "double":
                    textDecoration.PenOffset = inline.FontSize * 0.05;
                    textDecoration = textDecoration.Clone();
                    textDecoration.PenOffset = inline.FontSize * -0.05;
                    textDecorations.Add(textDecoration);
                    break;
                case "dotted":
                    textDecoration.Pen.DashStyle = DashStyles.Dot;
                    break;
                case "dash":
                    textDecoration.Pen.DashStyle = DashStyles.Dash;
                    break;
                case "dotDash":
                    textDecoration.Pen.DashStyle = DashStyles.DashDot;
                    break;
                case "dotDotDash":
                    textDecoration.Pen.DashStyle = DashStyles.DashDotDot;
                    break;
                case "none":
                default:
                    // If underline type is none or unsupported then it will be ignored.
                    return null;
            }

            return textDecorations;
        }

        private IDisposable SetCurrent(TextElement current)
        {
            return new CurrentHandle(this, current);
        }

        private struct CurrentHandle : IDisposable
        {
            private readonly DocxToFlowDocumentConverter converter;
            private readonly TextElement previous;

            public CurrentHandle(DocxToFlowDocumentConverter converter, TextElement current)
            {
                this.converter = converter;
                this.converter.AddChild(current);
                this.previous = this.converter.current;
                this.converter.current = current;
            }

            public void Dispose()
            {
                this.converter.current = this.previous;
            }
        }
    }
}
