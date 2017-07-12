using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnglishTrain
{
    /* 流程：單字匯入直接貼在TextBox上，以隔行做區隔
     * 題庫內容可查看匯入的單字or句子、可刪減
     */
    //Yahoo爬下來的資料中，一個英文單字有多種詞性，一個詞性有0~N種中文意思，一個中文意思有多個例句
    [Serializable]
    /// <summary>
    /// 單字類別，含詞性、中文意思、權重等。
    /// </summary>
    class Word
    {
        /// <summary>單字權重起始值，數字越大越不熟，0=非常熟，完全不會出現在單字練習。</summary>
        public int weight = 3;
        /// <summary>英文單字</summary>
        public readonly string word;//該英文單字
        /// <summary>音標</summary>
        public readonly string phoneticSymbol;
        /// <summary>給詞性Key獲得中文解釋List</summary>
        public Dictionary<string, List<string>> chineseMeaning = new Dictionary<string, List<string>>();
        /// <summary>直接ToString的解釋</summary>
        private readonly string explanation ;
        public Word(string word,string phonetic,string exp)
        {
            this.word = word;
            this.phoneticSymbol = phonetic;
            explanation = exp;
        }
        public override string ToString()
        {
            return explanation;
        }
    }
    [Serializable]
    class Sentence
    {
        public Sentence(string chi, string eng,string w,string p,int i)
        {
            Chi = chi;
            Eng = eng;
            WordKey = w;
            PartOfSpeech = p;
            ChiMeaningIndex = i;
        }
        public readonly string Chi;
        public readonly string Eng;
        public readonly string WordKey;//屬於哪個單字的例句
        public readonly string PartOfSpeech;//屬於該單字在哪個詞性的例句
        public readonly int ChiMeaningIndex;//該單字在該詞性時屬於哪個中文意思的例句
    }
    
    /*
    public static class Test
    {
        public static readonly string intransitiveVerb = "vi.不及物動詞";
        public static readonly string transitiveVerb = "vt.及物動詞";
        public static readonly string noun = "n.名詞";
        public static readonly string adverb = "ad.副詞";
        public static readonly string preposition = "prep.介係詞";
        public static readonly string conjunction = "conj.連接詞";
        public static readonly string pronoun = "pron.代名詞";
        public static readonly string transitiveVerb2 = "vt.[W]";
    }*/
    
    static class DataBase
    {
        static string debugPath = Directory.GetCurrentDirectory();
        /// <summary>例句資料庫，給key(單字)獲得例句</summary>
        public static Dictionary<string, List<Sentence>> sentanceDB = new Dictionary<string, List<Sentence>>();
        /// <summary>單字資料庫，給key(單字)獲得單字資料</summary>
        public static Dictionary<string, Word> wordDB = new Dictionary<string, Word>();
        /// <summary>新增單字，將單字、單字的例句從Yahoo爬進資料庫</summary>
        public static void addWordData(string word,out bool success)
        {
            success = false;
            word = getVerbRoot(getSingularNoun(word));//獲得動詞原形、單數名詞
            if (!wordDB.Keys.Contains(word))//判斷資料庫內是否已經有該單字
            {
                string htmlstr = getHTML(word);
                string yahooData = getWord(htmlstr);//尚未做單複數、動詞時態轉換
                //yahooData結構:單字#KK音標#詞性$中文意思@例句^例句$中文意思@例句^例句#詞性$中文意思#詞性$中文意思...
                if (!yahooData.Equals("search error"))
                {
                    string[] layer1 = yahooData.Split(new char[] { '#' });//分割:KK音標#詞性...#詞性...#詞性...
                    Word w = new Word(layer1[0], layer1[1].Replace('ˋ', '`'), getWordExplanation(htmlstr));//給word物件單字和音標
                    List<Sentence> sentences = new List<Sentence>();

                    for (int l1 = 2; l1 < layer1.Length; l1++)
                    {
                        List<string> chiMeaning = new List<string>();

                        string[] layer2 = layer1[l1].Split(new char[] { '$' });//分割:詞性$中文意思...$中文意思...$中文意思...

                        for (int l2 = 1; l2 < layer2.Length; l2++)
                        {
                            string[] layer3 = layer2[l2].Split(new char[] { '@' });//分割:中文意思@例句^例句...
                            chiMeaning.Add(layer3[0]);//中文意思
                            if (!layer3[1].Equals("沒例句。"))
                            {
                                string[] layer4 = layer3[1].Split(new char[] { '^' });//分割:例句^例句^例句...

                                foreach (string ss in layer4)
                                {
                                    int index = ss.LastIndexOf(' ');//獲得 例句 與 例句的中文 中間的索引
                                    Sentence s = new Sentence(ss.Substring(index + 1), ss.Substring(0, index), layer1[0], layer2[0], chiMeaning.Count-1);
                                    sentences.Add(s);//例句
                                }
                            }
                        }
                        w.chineseMeaning[layer2[0]] = chiMeaning;//詞性當Key，給key獲得該詞性的所有中文意思
                    }
                    sentanceDB[layer1[0]] = sentences;
                    wordDB[layer1[0]] = w;
                    success = true;
                }
            }
            SaveDatabase();//儲存資料在本地端
        }
        /// <summary>Get HTML Source Code</summary>
        public static string getHTML(string word)
        {
            WebRequest myRequest = WebRequest.Create(@"https://tw.dictionary.search.yahoo.com/search?p=" + word + "&fr2=dict");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream());
            string htmlSourceCode = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return htmlSourceCode;
        }
        private static string getWord(string htmlSourceCode)
        {
            #region useing IronPython
            var engine = Python.CreateEngine();
            #region Load Python Lib From Debug Path
            string dir = System.IO.Path.GetDirectoryName(debugPath + "\\python\\");
            ICollection<string> paths = engine.GetSearchPaths();
            if (!String.IsNullOrWhiteSpace(dir))
                paths.Add(dir);
            else
                paths.Add(Environment.CurrentDirectory);
            engine.SetSearchPaths(paths);
            #endregion

            var scope = engine.CreateScope();
            var pySrc =
            #region python code
@"from bs4 import BeautifulSoup
def getstring(inp):
    result = '' 
    soup = BeautifulSoup(inp, 'html.parser')
    try:
        allBlock = soup.find_all('div', class_='dd algo explain mt-20 lst DictionaryResults')
        meaningBlock = allBlock[0].find_all('ul', class_='compArticleList mb-15 ml-10')
    except:
        result='search error'
    else:
        word=soup.find_all('span',class_=' mr-15')
        result+= word[0].text+'#'
        pronunciation=soup.find_all('span',class_='cite')
        result+=pronunciation[0].text
        parts = allBlock[0].find_all('h3')
        for i in range(len(meaningBlock)):
            result+='#'+parts[i].text
        
            for j in meaningBlock[i].find_all('li'):
                result+='$'+j.h4.text+'@'
                hasES=False
                try:
                    exampleSentence =j.find_all('span')
                    for k in range(1, len(exampleSentence),2):
                        if exampleSentence[k].text!=' ':
                            if k==len(exampleSentence)-2:
                                result+=exampleSentence[k].text
                                hasES=True
                            else:
                                result+=exampleSentence[k].text+'^'
                                hasES=True
                except:
                    result+='沒例句。'
                    pass
                if hasES==False:
                    result+='沒例句。'
    return result";
            #endregion
            engine.Execute(pySrc, scope);
            string result = "";
            try
            {
                var calcAdd = scope.GetVariable("getstring");//使用python內的getstring方法
                result = calcAdd(htmlSourceCode);
            }
            catch (Exception)
            {
                result = "search error";
            }
            #endregion
            return result;
        }
        public static string getWordExplanation(string htmlSourceCode)//獲得Tosting的文字
        {
            #region useing IronPython
            var engine = Python.CreateEngine();
            #region Load Python Lib From Debug Path
            string dir = System.IO.Path.GetDirectoryName(debugPath + "\\python\\");
            ICollection<string> paths = engine.GetSearchPaths();
            if (!String.IsNullOrWhiteSpace(dir))
                paths.Add(dir);
            else
                paths.Add(Environment.CurrentDirectory);
            engine.SetSearchPaths(paths);
            #endregion
            var scope = engine.CreateScope();
            var pySrc =
            #region python code
@"from bs4 import BeautifulSoup
def getstring(inp):
    result = '' 
    soup = BeautifulSoup(inp, 'html.parser')
    try:
        allBlock = soup.find_all('div', class_='dd algo explain mt-20 lst DictionaryResults')
        meaningBlock = allBlock[0].find_all('ul', class_='compArticleList mb-15 ml-10')
    except:
        result='search error'
    else:
        word=soup.find_all('span',class_=' mr-15')
        result+= word[0].text+'\n'
        pronunciation=soup.find_all('span',class_='cite')
        result+=pronunciation[0].text+'\n'
        parts = allBlock[0].find_all('h3')
        for i in range(len(meaningBlock)):
            result+=parts[i].text+'\n'
            for j in meaningBlock[i].find_all('li'):
                result+='　　'+j.h4.text+'\n'
                try:
                    exampleSentence =j.find_all('span')
                    for k in range(1, len(exampleSentence),2):
                        if exampleSentence[k].text!=' ':
                            result+='　　　　'+exampleSentence[k].text+'\n'
                except:
                    pass
    return result";
            #endregion
            engine.Execute(pySrc, scope);
            string result = "";
            try
            {
                var calcAdd = scope.GetVariable("getstring");
                result = calcAdd(htmlSourceCode);
            }
            catch (Exception)
            {
                result = "search error";
            }
            #endregion
            return result;
        }

        private static Dictionary<string, string> verb_lemmas = new Dictionary<string, string>();//動詞型態字典
        private static void getVerbLemmas()//獲得動詞型態字典
        {
            #region getData
            string[] data;
            Dictionary<string, string[]> verb_tenses = new Dictionary<string, string[]>();

            using (StreamReader sr = new StreamReader(debugPath + "\\verb.txt"))
            {
                string line = sr.ReadToEnd();
                data = line.Split(new char[] { '\n' });
            }
            for (int i = 0; i < data.Length; i++)
            {
                string[] a = data[i].Split(new char[] { ',' });
                verb_tenses[a[0]] = a;
            }
            foreach (KeyValuePair<string, string[]> infinitive in verb_tenses)
            {
                foreach (string tense in verb_tenses[infinitive.Key])
                {
                    if (!tense.Equals(""))
                    {
                        verb_lemmas[tense] = infinitive.Key;
                    }
                }
            }
            #endregion
        }
        /// <summary>獲得原形動詞</summary>
        public static string getVerbRoot(string v)
        {
            try
            {
                return verb_lemmas[v];
            }
            catch (Exception)
            {
                return v;
            }
        }
        /// <summary>獲得單數名詞</summary>
        public static string getSingularNoun(string n)
        {
            return System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(System.Globalization.CultureInfo.GetCultureInfo("en-us")).Singularize(n);
        }
        
        /// <summary>移除單字</summary>
        public static void removeWord(string WordKey)
        {
            wordDB.Remove(WordKey);
            sentanceDB.Remove(WordKey);
            SaveDatabase();
        }
        /// <summary>載入資料庫</summary>
        public static void LoadDatabase()
        {
            try
            {
                //將檔案還原成原來的物件
                using (FileStream oFileStream = new FileStream($"{debugPath}\\wordDB.txt", FileMode.Open))
                {
                    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                    wordDB = (Dictionary<string, Word>)myBinaryFormatter.Deserialize(oFileStream);
                }
                
                using (FileStream oFileStream = new FileStream($"{debugPath}\\sentanceDB.txt", FileMode.Open))
                {
                    BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                    sentanceDB = (Dictionary<string, List<Sentence>>)myBinaryFormatter.Deserialize(oFileStream);
                }
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.ToString(), "讀檔");
            }
            
        }
        /// <summary>增加單字權重</summary>
        public static void weightIncrease(string word)
        {
            wordDB[word].weight++;
            SaveDatabase();
        }
        /// <summary>減少單字權重</summary>
        public static void weightDecrease(string word)
        {
            wordDB[word].weight--;
            SaveDatabase();
        }
        /// <summary>儲存資料庫</summary>
        private static void SaveDatabase()
        {
            using (FileStream oFileStream = new FileStream($"{debugPath}\\wordDB.txt", FileMode.Create))
            {
                //建立二進位格式化物件
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                //將物件進行二進位格式序列化，並且將之儲存成檔案
                myBinaryFormatter.Serialize(oFileStream, wordDB);
                oFileStream.Flush();
                oFileStream.Close();
                oFileStream.Dispose();
            }
            using (FileStream oFileStream = new FileStream($"{debugPath}\\sentanceDB.txt", FileMode.Create))
            {
                //建立二進位格式化物件
                BinaryFormatter myBinaryFormatter = new BinaryFormatter();
                //將物件進行二進位格式序列化，並且將之儲存成檔案
                myBinaryFormatter.Serialize(oFileStream, sentanceDB);
                oFileStream.Flush();
                oFileStream.Close();
                oFileStream.Dispose();
            }
        }
    } 
}
