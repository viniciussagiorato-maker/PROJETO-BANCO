using _65465456456.Pages;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging.Effects;
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
        public List<Button> MenuButtons = new();
        public List<Canvas> Menus = new();
        public home()
        {
            InitializeComponent();
            AreaConteudo.Content = new PageInicio();

            MenuButtons = new()
        {
            btninicio,
            btnalunos,
            btnprofessores,
            btnturmas,
            btnnotas,
            btnagenda,
            btnfinanceiro,
            btnconfig
        };
            RefreshMenus(btninicio);
        }
        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                RefreshMenus(btn);
            }
        }

        private void RefreshMenus(Button curMenu)
        {
            foreach (var btn in MenuButtons)
            {
                btn.Background = btn == curMenu
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4178FF"))
                : Brushes.Transparent;

            }

            OpeMenu(curMenu);
        }
        private void OpeMenu(Button btn)
        {
            foreach (var menu in Menus)
            {
                menu.Visibility = Visibility.Collapsed;
            }

            switch (btn.Name)
            {
                case "btninicio":
                    AreaConteudo.Content = new PageInicio();
                    break;

                case "btnalunos":
                    AreaConteudo.Content = new Pagealunos();
                    break;

                case "btnprofessores":
                    AreaConteudo.Content = new PageProfessores();
                    break;
                case "btnconfig":
                    AreaConteudo.Content = new Pageconfig();
                    break;


            }
        }

        private void FecharSistema(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }





      
    }
}







