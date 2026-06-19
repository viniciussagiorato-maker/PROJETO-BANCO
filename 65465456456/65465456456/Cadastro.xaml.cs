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
    /// Interação lógica para Cadastro.xam
    /// </summary>
    public partial class Cadastro : Page
    {
        public Cadastro()
        {
            InitializeComponent();
        }

        private void geral_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is TextBlock block)
            {
                block.Visibility = Visibility.Hidden;
            }
            else if (sender is PasswordBox pwd && pwd.Tag is TextBlock block2)
            {
                block2.Visibility = Visibility.Hidden;
            }
        }

        private void geral_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && txt.Tag is TextBlock block)
            {
                block.Visibility = string.IsNullOrWhiteSpace(txt.Text)
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }
            else if (sender is PasswordBox pwd && pwd.Tag is TextBlock block2)
            {
                block2.Visibility = string.IsNullOrWhiteSpace(pwd.Password)
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }
        }






        private void cadastrar_Click(object sender, RoutedEventArgs e)
        {


         

            string senha = senhadigitar.Password;
            string nome = nomedigitar.Text;
            string cpf = CPF.Text;
            string email = email_usuario.Text;
            string senhaca = senhadigitar_Copiar.Password;

            if (string.IsNullOrWhiteSpace(nome) ||
                 string.IsNullOrWhiteSpace(cpf) ||
                 string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha) ||
                string.IsNullOrWhiteSpace(senhaca))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            // Verifica se as senhas coincidem
            if (senha != senhaca)
            {
                MessageBox.Show("As senhas não coincidem.");
                return;
            }

            string conexaoString =
                "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = @"
                INSERT INTO usuarios
                (Nome_user, CPF, Email, Senha)
                VALUES
                (@Nome, @CPF, @Email, @Senha)";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@CPF", cpf);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    int linhasAfetadas = cmd.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                    {
                        MessageBox.Show("Usuario cadastrado com sucesso!");
                        NavigationService.Navigate(new Paginaloginsimples());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }



        }




    }
    }

