using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YGO_Searcher
{
    /// <summary>
    /// Logique d'interaction pour RequestProgressionWindow.xaml
    /// </summary>
    public partial class RequestProgressionWindow : Window
    {
        public RequestProgressionWindow()
        {
            InitializeComponent();
        }
        
        public void AddStatusText(string StatusText = "")
        {
            Log_ListBox.Items.Add(StatusText);
        }
        public void SetStatusPercent(double Value = 0)
        {
            RequestProgressStatus_Bar.Value = Value;
        }
    }
}
