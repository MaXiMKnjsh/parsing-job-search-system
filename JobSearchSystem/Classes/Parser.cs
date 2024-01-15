using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Diagnostics;
using OpenQA.Selenium.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace JobSearchSystem
{
    internal class Parser
    {
        private string searchProperties;
        private Collection collection;
        public Parser(Collection collection, string searchProperties)
        {
            this.collection = collection;
            this.searchProperties = searchProperties;
        }

        public string Start()
        {
            collection.Clear();

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless"); // Включение безголового режима браузера

            IWebDriver driver = new ChromeDriver(options);

            driver.Url = "https://brest.rabota.by/search/vacancy?" + searchProperties;

            //получение количества найденных результатов
            string stringResult = (driver.FindElements(By.ClassName("bloko-header-section-3")).ToList())[0].Text;
            //преобразование строки
            string[] arrayResult;
            int pagesLeftToParse = 0, vacanciesCount = 0;
            try
            {
                arrayResult = stringResult.Split(new char[] { ' ' });
                vacanciesCount = int.Parse(arrayResult[0]);
            }
            catch (Exception) { vacanciesCount = 0; }
            finally
            {
                pagesLeftToParse = (int)Math.Ceiling((double)vacanciesCount / 50);
                string mainUrl = driver.Url;
                int currentPage = 1;

                string nextUrl, name, city, salary, link;

                try
                {
                    while (pagesLeftToParse > 0) // пока страницы с данными не закончатся
                    {
                        // поиск всех блоков с вакансиями на загруженной странице
                        var elements = driver.FindElements(By.ClassName("serp-item")).ToList();

                        // перебор каждого блока и вытаскивание из него нужные данные
                        foreach (var i in elements)
                        {
                            // данные по умолчанию
                            name = "Название вакансии не указано";
                            city = "Нет адреса";
                            salary = "Не указана";
                            link = string.Empty;

                            var buffer = i.FindElements(By.ClassName("serp-item__title")).ToList();
                            if (buffer.Count > 0) name = buffer[0].Text;

                            buffer = i.FindElements(By.ClassName("bloko-header-section-2")).ToList();
                            if (buffer.Count > 0) salary = buffer[0].Text;

                            buffer = i.FindElements(By.ClassName("bloko-link")).ToList();
                            if (buffer.Count > 0) link = buffer[0].GetAttribute("href");

                            buffer = i.FindElements(By.ClassName("bloko-text")).ToList();
                            if (buffer.Count > 0)
                            {
                                city = string.Empty;
                                foreach (var el in buffer)
                                {
                                    if (el.Text != string.Empty)
                                    {
                                        if (city == string.Empty)
                                            city += el.Text;
                                        else city += ", " + el.Text;
                                    }
                                }
                            }

                            // добавление вакансии в коллекцию
                            collection.Add(name, salary, city, link);
                        }

                        pagesLeftToParse--;

                        // создание новой ссылки для перехода на следующую страницу
                        nextUrl = mainUrl + "&page=" + currentPage;

                        // переход по ссылке на след. страницу
                        driver.Navigate().GoToUrl(nextUrl);

                        // счётчик, показывающий номер текущей страницы обхода
                        currentPage++;
                    }

                // запись всех данных коллекции в логи (.txt файл)
                collection.AddToLog();

                }
                catch { MessageBox.Show("Что-то случилось...!", "Ошибочка вышла...", MessageBoxButton.OK, MessageBoxImage.Error); }

            }

            driver.Quit();

            return vacanciesCount.ToString();
        }
    }
}
