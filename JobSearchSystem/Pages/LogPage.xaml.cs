using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

namespace JobSearchSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для LogPage.xaml
    /// </summary>
    public partial class LogPage : Page
    {
        private List<LogPageNode> LogPageList;
        private Collection collection;
        private NavigateClass navigateClass;
        public LogPage(List<LogPageNode> LogPageList, NavigateClass navigateClass)
        {
            InitializeComponent();
            this.LogPageList = LogPageList;
            this.navigateClass = navigateClass;
            collection = new Collection();

            foreach (var item in this.LogPageList)
                List.Items.Add(item);
        }

        private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                string path = LogPageList[listView.SelectedIndex].link;
                
                WriteLogs(path);
            }
        }

        private void WriteLogs(string path)
        {
            string jsonText = File.ReadAllText("logs\\"+path);

            var stringReader = new StringReader(jsonText);

            DeserializeMultiple(stringReader);

            navigateClass.NavigateToPage(new ListPage(collection));
            
        }

        private Collection DeserializeMultiple(TextReader textReader)
        {
            JsonSerializer serializer = new JsonSerializer();
            Collection result = new Collection();

            using (var jsonReader = new JsonTextReader(textReader))
            {
                jsonReader.CloseInput = false;
                jsonReader.SupportMultipleContent = true;

                while (jsonReader.Read())
                {
                    collection.Add(serializer.Deserialize<Collection.Node>(jsonReader));
                }
            }
            return result;
        }

        private void DeleteLog_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List.Items.Clear();
        }
    }
}
