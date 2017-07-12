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
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class WordTestWindow : Window
    {
        /// <summary>單字Yahoo發音</summary>
        WMPLib.WindowsMediaPlayer YahooPlayer;
        /// <summary>單字Google發音</summary>
        WMPLib.WindowsMediaPlayer GooglePlayer;
        /// <summary>例句慢速發音</summary>
        List<WMPLib.WindowsMediaPlayer> SlowPlayer = new List<WMPLib.WindowsMediaPlayer>();
        /// <summary>例句常速發音</summary>
        List<WMPLib.WindowsMediaPlayer> NormalPlayer = new List<WMPLib.WindowsMediaPlayer>();
        /// <summary>目前的狀態</summary>
        UIVisibilityState CurrentState = UIVisibilityState.OnlyVoice;
        /// <summary>UI的狀態種類</summary>
        enum UIVisibilityState
        {
            OnlyVoice, VoiceAndEng, All
        }
        public WordTestWindow()
        {
            InitializeComponent();
            YahooPlayer = new WMPLib.WindowsMediaPlayer();
            GooglePlayer = new WMPLib.WindowsMediaPlayer();
            setTest();
        }
        /// <summary>以權重值獲得隨機的單字</summary>
        private string GetRandomWord()
        {
            Random ran = new Random();
            List<string> randomBox = new List<string>();//產生權重數目的單字
            foreach (KeyValuePair<string, Word> word in DataBase.wordDB)
            {
                for (int i = 0; i < word.Value.weight; i++)
                {
                    randomBox.Add(word.Key);
                }
            }
            return randomBox.Count == 0 ? "沒單字" : randomBox[ran.Next(randomBox.Count)];
        }
        /// <summary>設置題目，包含控制項改動</summary>
        private void setTest()
        {
            NormalPlayer.Clear();
            SlowPlayer.Clear();

            string wordstr = GetRandomWord();
            
            if(wordstr!= "沒單字")
            {
                wordLabel.Content = wordstr;
                Word word = DataBase.wordDB[wordstr];

                //設題目時先載入音檔，避免每次播放都要載入。
                GooglePlayer.URL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={word.word}";
                YahooPlayer.URL = $"https://s.yimg.com/tn/dict/dreye/live/f/{word.word}.mp3";
                if ((bool)GoogleRadioButton.IsChecked)
                    YahooPlayer.controls.stop();
                else
                    GooglePlayer.controls.stop();

                phoneticSymbolLabel.Content = word.phoneticSymbol;//show音標
                foreach(KeyValuePair<string, List<string>> s in word.chineseMeaning)//show詞性、中文意思
                {
                    ChiTextBlock.Text += $"{s.Key}\n";
                    for(int i = 0; i < s.Value.Count; i++)
                        ChiTextBlock.Text += $"　 {s.Value[i]}\n";
                }
                for (int i = 0; i < DataBase.sentanceDB[wordstr].Count; i++)
                {
                    //載入例句音檔
                    NormalPlayer.Add(new WMPLib.WindowsMediaPlayer { URL= $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q={DataBase.sentanceDB[wordstr][i].Eng}" });
                    NormalPlayer[i].controls.stop();
                    SlowPlayer.Add(new WMPLib.WindowsMediaPlayer { URL = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&ttsspeed=0.1&q={DataBase.sentanceDB[wordstr][i].Eng}" });
                    SlowPlayer[i].controls.stop();
                    //新增英文和中文的Row
                    SentenceGrid.RowDefinitions.Add(new RowDefinition());
                    SentenceGrid.RowDefinitions.Add(new RowDefinition());
                    var grid = new Grid();
                    var label = new Label();
                    label.Content = DataBase.sentanceDB[wordstr][i].Chi;
                    label.Visibility = Visibility.Hidden;
                    label.Foreground = Brushes.Gainsboro;
                    label.FontSize = 29;
                    string[] sentanceWords = DataBase.sentanceDB[wordstr][i].Eng.Split(new char[] {' '});
                    grid.HorizontalAlignment = HorizontalAlignment.Left;
                    grid.Visibility = Visibility.Hidden;
                    #region 創建例句內所有單字按紐
                    for (int j = 0; j < sentanceWords.Length; j++)
                    {

                        var column = new ColumnDefinition();
                        grid.ColumnDefinitions.Add(column);
                        grid.ColumnDefinitions[j].Width = GridLength.Auto;//給予grid寬度自動，防止按鈕部分遮蓋
                        var button = new Button();
                        button.Content = sentanceWords[j]+" ";
                        button.HorizontalContentAlignment = HorizontalAlignment.Left;
                        button.BorderThickness = new Thickness(0);//按鈕框線粗細，0=看不到框線
                        button.Background = Brushes.Black;
                        button.Foreground = Brushes.Pink;
                        button.FontSize = 29;
                        button.Click += Button_Click;
                        Grid.SetColumn(button, j);
                        grid.Children.Add(button);
                    }
                    #endregion
                    #region 創建例句播放聲音按紐
                    var column2 = new ColumnDefinition();
                    grid.ColumnDefinitions.Add(column2);
                    var sentanceVoiceButton = new Button();
                    sentanceVoiceButton.Content = $"_{i}Play";
                    sentanceVoiceButton.FontSize = 29;
                    sentanceVoiceButton.Tag = i;
                    sentanceVoiceButton.VerticalAlignment = VerticalAlignment.Center;
                    sentanceVoiceButton.Click += SentanceVoiceButton_Click;
                    Grid.SetColumn(sentanceVoiceButton, sentanceWords.Length);
                    grid.Children.Add(sentanceVoiceButton);
                    #endregion
                    Grid.SetRow(grid, i * 2);
                    Grid.SetRow(label, i * 2 + 1);
                    SentenceGrid.Children.Add(grid);
                    SentenceGrid.Children.Add(label);
                }
            }
            else//沒有不熟單字的情況
            {
                ruleLabel1.Content = "目前沒有不熟的單字";
                ruleLabel2.Content = "請先去新增單字";
                NextButton.IsEnabled = false;
            }
        }
        /// <summary>例句英文單字按紐，點擊查詢單字</summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            string word = b.Content.ToString().Substring(0, b.Content.ToString().Length-1);//去除空白
            word = Regex.Replace(word, "[.']", "", RegexOptions.IgnoreCase);//去除'和.
            word = DataBase.getVerbRoot(word);//給出動詞字根
            word = DataBase.getSingularNoun(word);//給出單數名詞
            string result = DataBase.getWordExplanation(DataBase.getHTML(word));
            if (result.Equals("search error"))
            {
                MessageBox.Show("找不到該單字！");
            }else
            {
                wordExplanationWindow wordWindow = new wordExplanationWindow(result,word);//取得HTML並用python擷取單字解釋
                wordWindow.Show();
            }
        }
        /// <summary>例句聲音播放按紐</summary>
        private void SentanceVoiceButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            foreach (WMPLib.WindowsMediaPlayer p in SlowPlayer)
            {
                p.controls.pause();
            }
            foreach (WMPLib.WindowsMediaPlayer p in NormalPlayer)
            {
                p.controls.pause();
            }
            if ((bool)SlowVoiceCheckBox.IsChecked)
            {
                
                SlowPlayer[(int)b.Tag].controls.currentPosition = 0;
                SlowPlayer[(int)b.Tag].controls.play();
            }else
            {
                NormalPlayer[(int)b.Tag].controls.currentPosition = 0;
                NormalPlayer[(int)b.Tag].controls.play();
            }
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)GoogleRadioButton.IsChecked)
            {
                GooglePlayer.controls.pause();
                YahooPlayer.controls.pause();
                GooglePlayer.controls.currentPosition = 0;
                GooglePlayer.controls.play();
            }
            else//否則Yahoo發音
            {
                GooglePlayer.controls.pause();
                YahooPlayer.controls.pause();
                YahooPlayer.controls.currentPosition = 0;
                YahooPlayer.controls.play();
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            Close();
            mw.Show();
        }

        private void UnfamiliarButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeWeight((Button)sender);
        }

        private void MediumButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeWeight((Button)sender);
        }

        private void FamiliarButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeWeight((Button)sender);
        }
        private void ChangeWeight(Button bt)
        {
            switch (bt.Name)
            {
                case "UnfamiliarButton":
                    DataBase.weightIncrease(wordLabel.Content.ToString());
                    break;
                case "MediumButton":
                    break;
                case "FamiliarButton":
                    DataBase.weightDecrease(wordLabel.Content.ToString());
                    break;
                default:
                    MessageBox.Show("Error in ChangeWeight");
                    break;
            }
            #region UI控制
            //按完後進到下一題
            CurrentState = UIVisibilityState.OnlyVoice;
            UnfamiliarButton.IsEnabled = false;
            MediumButton.IsEnabled = false;
            FamiliarButton.IsEnabled = false;
            NextButton.IsEnabled = true;
            NextButton.Visibility = Visibility.Visible;
            ruleLabel1.Visibility = Visibility.Visible;
            ruleLabel2.Visibility = Visibility.Visible;
            wordLabel.Visibility = Visibility.Hidden;
            phoneticSymbolLabel.Visibility = Visibility.Hidden;
            chiMeanScrollViewer.Visibility = Visibility.Hidden;
            SentenceGrid.RowDefinitions.Clear();
            SentenceGrid.Children.Clear();
            #endregion
            setTest();
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (CurrentState)
            {
                case UIVisibilityState.OnlyVoice:
                    for(int i=0;i< SentenceGrid.Children.Count; i += 2)
                    {
                        SentenceGrid.Children[i].Visibility = Visibility.Visible;
                    }
                    ruleLabel1.Visibility = Visibility.Hidden;
                    ruleLabel2.Visibility = Visibility.Hidden;
                    ruleTextBlock1.Visibility = Visibility.Visible;
                    wordLabel.Visibility = Visibility.Visible;
                    phoneticSymbolLabel.Visibility = Visibility.Visible;
                    CurrentState = UIVisibilityState.VoiceAndEng;

                    break;
                case UIVisibilityState.VoiceAndEng:
                    for (int i = 1; i < SentenceGrid.Children.Count; i += 2)
                    {
                        SentenceGrid.Children[i].Visibility = Visibility.Visible;
                    }
                    ruleTextBlock1.Visibility = Visibility.Hidden;
                    NextButton.IsEnabled = false;
                    NextButton.Visibility = Visibility.Hidden;
                    UnfamiliarButton.IsEnabled = true;
                    MediumButton.IsEnabled = true;
                    FamiliarButton.IsEnabled = true;
                    chiMeanScrollViewer.Visibility = Visibility.Visible;
                    CurrentState = UIVisibilityState.All;
                    break;
                default:
                    MessageBox.Show("Error in CurrentState");
                    break;
            }
        }
        
    }
}
