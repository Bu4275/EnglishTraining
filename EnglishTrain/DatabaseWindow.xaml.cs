using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// DatabaseWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DatabaseWindow : Window
    {
        //搜尋自動改變LIST，點選List單字可觀看該單字詳細與例句(先用exp)
        public DatabaseWindow()
        {
            InitializeComponent();

            updataList();
        }

        private void updataList()//更新listbox
        {
            WordListBox.Items.Clear();
            IEnumerable<string> search = DataBase.wordDB.Select(x => x.Value.word).Where(x => x.Contains(SearchTextBox.Text)).OrderByDescending(x => x);
            foreach (string w in search)
            {
                WordListBox.Items.Add(w);
            }
            WordListBox.SelectionChanged += WordListBox_SelectionChanged;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            updataList();
        }

        private void WordListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WordListBox.SelectedValue != null)
            {
                string word = WordListBox.SelectedValue.ToString();
                //explanationTextBlock.Text = DataBase.wordDB[word].ToString();
                dataGrid.Children.Clear();
                dataGrid.RowDefinitions.Clear();
                dataGrid.RowDefinitions.Add(new RowDefinition());
                dataGrid.RowDefinitions.Add(new RowDefinition());
                dataGrid.VerticalAlignment = VerticalAlignment.Top;
                //顏色範例
                //https://msdn.microsoft.com/zh-tw/library/system.windows.media.brushes(v=vs.110).aspx
                #region 單字Label設定
                var wordlabel = new Label();
                wordlabel.Content = word;
                wordlabel.Foreground = Brushes.SkyBlue;
                wordlabel.FontSize = 45;
                Grid.SetRow(wordlabel, 0);
                dataGrid.Children.Add(wordlabel);
                #endregion
                #region 音標Label設定
                var phoneticlabel = new Label();
                phoneticlabel.Content = DataBase.wordDB[word].phoneticSymbol;
                phoneticlabel.Foreground = Brushes.SpringGreen;
                phoneticlabel.FontSize = 22;
                Grid.SetRow(phoneticlabel, 1);
                dataGrid.Children.Add(phoneticlabel);
                #endregion
                int gridRowIndex = 2;
                foreach (KeyValuePair<string, List<string>> m in DataBase.wordDB[word].chineseMeaning)
                {
                    #region 詞性設定
                    dataGrid.RowDefinitions.Add(new RowDefinition());
                    var partOfSpeechlabel = new Label();
                    partOfSpeechlabel.Content = m.Key;
                    partOfSpeechlabel.Foreground = Brushes.Lavender;
                    partOfSpeechlabel.FontSize = 30;
                    Grid.SetRow(partOfSpeechlabel, gridRowIndex);
                    dataGrid.Children.Add(partOfSpeechlabel);
                    gridRowIndex++;
                    #endregion
                    for(int i = 0; i < m.Value.Count; i++)
                    {
                        #region 中文意思設定
                        dataGrid.RowDefinitions.Add(new RowDefinition());
                        var chiMeaninglabel = new Label();
                        chiMeaninglabel.Content = $"　{m.Value[i]}";
                        chiMeaninglabel.Foreground = Brushes.PapayaWhip;
                        chiMeaninglabel.FontSize = 30;
                        Grid.SetRow(chiMeaninglabel, gridRowIndex);
                        dataGrid.Children.Add(chiMeaninglabel);
                        gridRowIndex++;
                        #endregion
                        IEnumerable<Sentence> searchSentence = DataBase.sentanceDB[word].Select(x => x).Where(x => (x.PartOfSpeech == m.Key && x.ChiMeaningIndex == i));
                        foreach(Sentence s in searchSentence)
                        {
                            #region 英文例句設定
                            dataGrid.RowDefinitions.Add(new RowDefinition());
                            var sentenceGrid = new Grid();
                            string[] sentanceWords = s.Eng.Split(new char[] { ' ' });
                            sentenceGrid.HorizontalAlignment = HorizontalAlignment.Left;
                            #   region 加上前面的空白間隔
                            sentenceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            sentenceGrid.ColumnDefinitions[0].Width = GridLength.Auto;
                            var Emptylabel = new Label();
                            Emptylabel.Content = $"　　　";
                            Emptylabel.FontSize = 22;
                            Grid.SetRow(Emptylabel, 0);
                            sentenceGrid.Children.Add(Emptylabel);
                            #endregion
                            for (int j = 0; j < sentanceWords.Length; j++)
                            {
                                sentenceGrid.ColumnDefinitions.Add(new ColumnDefinition());
                                sentenceGrid.ColumnDefinitions[j+1].Width = GridLength.Auto;//給予grid寬度自動，防止按鈕部分遮蓋
                                var button = new Button();
                                button.Content = sentanceWords[j] + " ";
                                button.BorderThickness = new Thickness(0);//按鈕框線粗細，0=看不到框線
                                button.Background = Brushes.Black;
                                button.Foreground = Brushes.Pink;
                                button.FontSize = 22;
                                button.Click += Button_Click;
                                Grid.SetColumn(button, j+1);
                                sentenceGrid.Children.Add(button);
                            }
                            Grid.SetRow(sentenceGrid, gridRowIndex);
                            dataGrid.Children.Add(sentenceGrid);
                            gridRowIndex++;
                            #endregion
                            #region 中文例句設定
                            dataGrid.RowDefinitions.Add(new RowDefinition());
                            var clabel = new Label();
                            clabel.Content = $"　　　 {s.Chi}";
                            clabel.Foreground = Brushes.Gainsboro;
                            clabel.FontSize = 22;
                            Grid.SetRow(clabel, gridRowIndex);
                            dataGrid.Children.Add(clabel);
                            gridRowIndex++;
                            #endregion
                        }
                    }
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string word = b.Content.ToString().Substring(0, b.Content.ToString().Length - 1);//去除空白
            word = Regex.Replace(word, "[.']", "", RegexOptions.IgnoreCase);//去除'和.
            word = DataBase.getVerbRoot(word);//給出動詞字根
            word = DataBase.getSingularNoun(word);//給出單數名詞
            string result = DataBase.getWordExplanation(DataBase.getHTML(word));
            if (result.Equals("search error"))
            {
                MessageBox.Show("找不到該單字！");
            }
            else
            {
                wordExplanationWindow wordWindow = new wordExplanationWindow(result, word);//取得HTML並用python擷取單字解釋
                wordWindow.Show();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Close();
            mw.Show();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataBase.removeWord(WordListBox.SelectedValue.ToString());
                updataList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("錯誤，無選取單字");
            }
            
        }
    }
}
