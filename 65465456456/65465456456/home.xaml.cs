using _65465456456.Pages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _65465456456
{
    /// <summary>
    /// Interação lógica para home.xam
    /// </summary>
    public partial class home : Page
    {
        public home()
        {
            InitializeComponent();
            AreaConteudo.Content = new PageInicio();
        }

        private void btnalunos_Click(object sender, RoutedEventArgs e)
        {

            AreaConteudo.Content = new Pagealunos();


        }

        private void btninicio_Click(object sender, RoutedEventArgs e)
        {

            AreaConteudo.Content = new PageInicio();


        }
    }
}
