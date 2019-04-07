using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YGO_Searcher
{
    public class About
    {
        public static string NAME = "YGO Searcher";
        public static string CREATOR = "Merwan LARA";
        public static int MAJOR_VERSION = 1;
        public static int MINOR_VERSION = 5;

        public static void ShowAboutWindow()
        {
            string ToShow = String.Format("{0} (Version {1}.{2})\n\nCreated by {3}.",
                About.NAME,
                About.MAJOR_VERSION,
                About.MINOR_VERSION,
                About.CREATOR);

            MessageBox.Show(ToShow, "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
