using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace EnglishTrain
{
    /// <summary>
    /// ImportWordWindow.xaml 的互動邏輯
    /// </summary>
    public partial class ImportWordWindow : Window
    {
        
        public ImportWordWindow()
        {
            InitializeComponent();
        }
        private void DataProcessing(string wordsString)//處理TextBox文字(將新單字匯入資料庫)
        {
            string[] words = wordsString.Split(new char[] { '\n' }); //TextBox文字以\n劃分各單字
            
            StringBuilder message = new StringBuilder();
            
            for (int i = 0; i < words.Length; i++)
            {
                bool success;
                DataBase.addWordData(words[i],out success);
                double pVlaue = (double)(i + 1) / words.Length * 100;
                //更新進度條
                pbStatus.Dispatcher.Invoke(() => pbStatus.Value = pVlaue, DispatcherPriority.Background);
                if (!success)
                {
                    message.Append($"{words[i]}新增失敗\n");
                }
                
                Thread.Sleep(1000);//防止被誤認為DDOS
            }
            if(!message.ToString().Equals(String.Empty))
                MessageBox.Show(message.ToString(), "新增失敗可能是已含有該單字，或Yahoo找不到該單字");

            importButton.IsEnabled = true;
            BackButton.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            importButton.IsEnabled = false;
            BackButton.IsEnabled = false;
            DataProcessing(Regex.Replace(inputTextBox.Text, "[\r]", "", RegexOptions.IgnoreCase));
            

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Close();
            mw.Show();
        }
    }
}
