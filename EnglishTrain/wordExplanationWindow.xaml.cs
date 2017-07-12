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

namespace EnglishTrain
{
    /// <summary>
    /// wordExplanationWindow.xaml 的互動邏輯
    /// </summary>
    public partial class wordExplanationWindow : Window
    {
        string word;
        public wordExplanationWindow(string str,string w)
        {
            InitializeComponent();
            word = w;
            LB.Content = str;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            bool success;
            DataBase.addWordData(word,out success);
            if (success)
                MessageBox.Show($"{word}新增成功");
            else
                MessageBox.Show($"{word}新增失敗");
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
