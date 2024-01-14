using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobSearchSystem
{
    public class LogPageNode
    {
        public LogPageNode(string name, string count, string date, string link) 
        {
            this.link = link;
            this.name = name;
            this.count = count;
            this.date = date;
        }
        public string link { get; set; }
        public string name { get; set; }
        public string count { get; set; }
        public string date { get; set; }
    }
}
