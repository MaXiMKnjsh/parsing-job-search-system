using JobSearchSystem.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JobSearchSystem
{
    public class NavigateClass
    {
        private Frame Myframe;
        public NavigateClass(Frame frame)
        {
            this.Myframe = frame;
            NavigateToPage(new MainPage());
        }
        public void NavigateToPage(Page page)
        {
            Myframe.Navigate(page);
        }
    }
}
