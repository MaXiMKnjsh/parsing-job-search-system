﻿using System;
using System.Collections.Generic;
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
using System.Diagnostics;

namespace JobSearchSystem.Pages
{
    /// <summary>
    /// Логика взаимодействия для SecondPage.xaml
    /// </summary>
    public partial class ListPage : Page
    {
        Collection collection;
        public ListPage(Collection collection)
        {
            InitializeComponent();
            
            this.collection = collection;

            foreach (var item in this.collection)
               List.Items.Add(item);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                Process.Start(collection.GetVacancyLink(listView.SelectedIndex));            
            }
        }
    }
}
