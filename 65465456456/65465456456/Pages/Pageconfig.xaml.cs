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

namespace _65465456456.Pages
{
    /// <summary>
    /// Interação lógica para Pageconfig.xam
    /// </summary>
    public partial class Pageconfig : UserControl
    {
        public Pageconfig()
        {
            InitializeComponent();

            // Ao abrir a página, marca o radio button de acordo com o tema
            // que está ativo no app no momento (evita "resetar" para Claro
            // toda vez que o usuário reabre a tela de Configurações).
            if (ThemeManager.TemaAtual == TemaApp.Escuro)
                temaescuro.IsChecked = true;
            else
                temaclaro.IsChecked = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {






        }

        // Tema Claro selecionado: mantém as cores originais do app
        private void temaclaro_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.AplicarTema(TemaApp.Claro);
        }

        // Tema Escuro selecionado: aplica uma paleta de cores diferente no app inteiro
        private void temaescuro_Checked(object sender, RoutedEventArgs e)
        {
            ThemeManager.AplicarTema(TemaApp.Escuro);
        }
    }
}
