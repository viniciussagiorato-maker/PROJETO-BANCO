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

namespace _65465456456.Pages
{
    /// <summary>
    /// Interação lógica para PageProfessores.xam
    /// </summary>
    public partial class PageProfessores : UserControl
    {
        public PageProfessores()
        {
            InitializeComponent();
        }

        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {

            string nome = pronome.Text;
            string cpf = procpf.Text;
            string email = proemail.Text;
            string senha = prosenha.Password;



            if (string.IsNullOrWhiteSpace(nome) ||
                 string.IsNullOrWhiteSpace(cpf) ||
                 string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha)
                )
            {
                MessageBox.Show("Preencha todos os campos.");
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
                INSERT INTO professores
                (Nome, CPF, Email, Senha)
                VALUES
                (@Nome, @CPF, @Email, @Senha)";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@CPF", cpf);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);

                    cmd.ExecuteNonQuery();


                }

                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }


            }
        }
    }
}
