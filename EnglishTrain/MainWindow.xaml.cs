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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnglishTrain
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataBase.LoadDatabase();
            
        }

        private void WordButton_Click(object sender, RoutedEventArgs e)
        {
            WordTestWindow wordTestwindow = new WordTestWindow();
            Close();
            wordTestwindow.Show();
        }

        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            ImportWordWindow imw = new ImportWordWindow();
            Close();
            imw.Show();
        }

        private void DatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseWindow dbw = new DatabaseWindow();
            Close();
            dbw.Show();
        }
    }
}
