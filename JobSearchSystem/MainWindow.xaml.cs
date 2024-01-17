using System;
using System.Collections.Generic;
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
using JobSearchSystem.Pages;
using Newtonsoft.Json;

namespace JobSearchSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Collection collection = new Collection();

        // характеристики запроса
        private string area, searchPeriod, orderBy;

        private NavigateClass navigateClass;

        private List<LogPageNode> LogPageList;

        public MainWindow()
        {
            InitializeComponent();
        }

        // переход на другую форму
        private void Window_Loaded(object sender, RoutedEventArgs e) => navigateClass = new NavigateClass(MyFrame);

        // обработка клика на "ПОИСК"
        private async void SearchButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SearchTextBox.Text != string.Empty)
            {
                ReadFilters();

                // составление полных характеристик запроса для построения корректной ссылки
                string searchProperties = $"text={SearchTextBox.Text}+&search_period={searchPeriod}+&order_by={orderBy}+{area}";

                // передаю название запроса для записи в файл
                collection.RequestName =SearchTextBox.Text;

                // асинхронно получая количество найденных результатов и сами результаты
                CountBox.Text = "Результатов: " +
                await Task.Run(() =>
                {
                    Parser parser = new Parser(collection, searchProperties);
                    return parser.Start();
                });

                if (collection.Count() > 0) navigateClass.NavigateToPage(new ListPage(collection));
            }
            else MessageBox.Show("В вашем запросе ничего не указано!", "Ошибочка вышла...", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // обработка клика на "ЛОГ", т.е. заполнение таблицы
        private async void LogButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // получает данные для заполнения таблицы всего лога
            bool isExistLogs =  await Task.Run(() =>
            {
                string metaLogPath = "logs\\metaData.txt";

                if (File.Exists(metaLogPath))
                {
                    LogPageList = new List<LogPageNode>();

                    using (StreamReader sr = new StreamReader(metaLogPath))
                    {
                        string line = string.Empty; string[] buff;

                        while (!sr.EndOfStream)
                        {
                            line = sr.ReadLine();
                            buff = line.Split(new char[] { '\t' });

                            LogPageList.Add(new LogPageNode(buff[1], buff[2], buff[3], buff[0]));
                        }
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show("Файл-лог пуст!", "Ошибочка вышла...", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            });

            if (isExistLogs)
            {
                CountBox.Text = "История поиска";
                navigateClass.NavigateToPage(new LogPage(LogPageList,navigateClass));
            }
        }
        
        private void Info_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            navigateClass.NavigateToPage(new InfoPage());
                CountBox.Text = "Информация";
        }

            // чтение фильтров поиска
            private void ReadFilters()
        {
            area = searchPeriod = orderBy = string.Empty;

            if (iSrepublicTime_radioButton.IsChecked == true) orderBy = "publication_time";
            if (iSsalaryDecrease_radioButton.IsChecked == true) orderBy = "salary_desc";
            if (iSrelevance_radioButton.IsChecked == true) orderBy = "relevance";
            if (iSsalaryIncrease_radioButton.IsChecked == true) orderBy = "salary_asc";

            if (isAllTime_radioButton.IsChecked == true) searchPeriod = "0";
            if (isLastDay_radioButton.IsChecked == true) searchPeriod = "1";
            if (isMonth_radioButton.IsChecked == true) searchPeriod = "30";
            if (isWeek_radioButton.IsChecked == true) searchPeriod = "7";
            if (isThreeDays_radioButton.IsChecked == true) searchPeriod = "3";

            if (isBelarus_checkBox.IsChecked == true) area += "&area=16";
            if (isBrest_checkBox.IsChecked == true) area += "&area=1007";
            if (isVitebsk_checkBox.IsChecked == true) area += "&area=1005";
            if (isGomel_checkBox.IsChecked == true) area += "&area=1003";
            if (isGrodno_checkBox.IsChecked == true) area += "&area=1006";
            if (isMinsk_checkBox.IsChecked == true) area += "&area=1002";
            if (isMogilev_checkBox.IsChecked == true) area += "&area=1004";
            // если ни один чекбокс не выбран, то поиск идёт по Беларуси (area=16)
            if (area == string.Empty) area = "&area=16";
        }
        
        // переход по гиперссылке "rabota.by"
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) => Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
    }
}
