using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V118.DOM;
using OpenQA.Selenium.DevTools.V118.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSearchSystem
{
    // Collection создал с целью поработать с IEnumerable/Ienumerator
    // Можно было обойтись гораздо проще, создав List<Node> не пхав всё в отдельный класс :)
    public class Collection : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            return new CollectionEnum(myList);
        }
        public class Node
        {
            public string name { get; set; }
            public string salary { get; set; }
            public string city { get; set; }
            public string link { get; set; }
            public Node(string name, string salary, string city, string link)
            {
                this.name = name;
                this.salary = salary;
                this.city = city;
                this.link = link;
            }
        }
        private List<Node> myList;

        private string requestName;
        public string RequestName
        {
            set { requestName = value; }
        }

        public Collection()
        {
            myList = new List<Node>();
        }
        public void Clear()
        {
            myList.Clear();
        }
        public void Add(string name, string salary, string city, string link)
        {
            myList.Add(new Node(name, salary, city, link));
        }
        public void Add(Node node)
        {
            myList.Add(node);
        }
        public void AddToLog()
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            string fileName= $"{Path.GetRandomFileName()}.json";

            // дописываю в metaData.txt название файла, запрос, количество вернувшихся результатов, дату запроса
            using (StreamWriter sw = new StreamWriter("logs\\metaData.txt",true)) 
            {
                sw.WriteLine($"{fileName}\t{requestName}\t{myList.Count}\t{DateTime.Now}");
            }

            using (StreamWriter sw = new StreamWriter($"logs\\{fileName}", false))
            {
                string jsonNode;
                foreach (Node node in myList)
                {
                    // Сериализуем объект в формат JSON
                    jsonNode = JsonConvert.SerializeObject(node, Formatting.Indented);

                    sw.WriteLine(jsonNode);
                }
            }
        }

        public int Count() { return myList.Count; }
        public string GetVacancyLink(int index)
        {
            return myList[index].link;
        }
    }

    class CollectionEnum : IEnumerator
    {
        private List<JobSearchSystem.Collection.Node> myList;
        private int position = -1;
        public CollectionEnum(List<JobSearchSystem.Collection.Node> myList) => this.myList = myList;
        public object Current
        {
            get
            {
                if (position == -1 || position >= myList.Count)
                    throw new ArgumentException();
                return myList[position];
            }
        }

        public bool MoveNext()
        {
            if (position < myList.Count - 1)
            {
                position++;
                return true;
            }
            else
                return false;
        }

        public void Reset() => position = -1;
    }
}
