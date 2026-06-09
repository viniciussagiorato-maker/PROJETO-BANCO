using MySql.Data.MySqlClient;
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
    /// Interação lógica para Paginaloginsimples.xam
    /// </summary>
    public partial class Paginaloginsimples : Page
    {
        public Paginaloginsimples()
        {
            InitializeComponent();
        }

        private void emailusuario_GotFocus(object sender, RoutedEventArgs e)
        {
            blockusuario.Visibility = Visibility.Hidden;
        }

        private void emailusuario_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(email_usuario.Text))
            {
                blockusuario.Visibility = Visibility.Visible;
            }
        }


        private void Senhadigitar_GotFocus(object sender, RoutedEventArgs e)
        {
            senhablock.Visibility = Visibility.Hidden;
        }

        private void Senhadigitar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(senhadigitar.Password))
            {
                senhablock.Visibility = Visibility.Visible;
            }
        }

        private void Logar(object sender, RoutedEventArgs e)
        {

            string Usu = email_usuario.Text;
            string sen = senhadigitar.Password;
            string nome = "";


            string conexaoString =
                 "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql =
                        "SELECT * FROM alunos " +
                        "WHERE Email = @Email AND Senha = @Senha";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@Email", Usu);
                    cmd.Parameters.AddWithValue("@Senha", sen);
                    cmd.Parameters.AddWithValue("@Nome", nome);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        MessageBox.Show("Login realizado com sucesso!");

                        

                        MessageBox.Show("Bem-vindo " + nome);

                        NavigationService.Navigate(new home());
                    }
                    else
                    {
                        MessageBox.Show("Email ou senha incorretos.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private void CADASTROBTN_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Cadastro());

        }

      
    }
}
