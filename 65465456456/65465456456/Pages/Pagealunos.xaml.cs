using _65465456456.Pages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interação lógica para Pagealunos.xam
    /// </summary>
    public partial class Pagealunos : UserControl
    {
        public Pagealunos()
        {
            InitializeComponent();

            datagridalu();
           

        }

        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = alunome.Text;
            string cpf = aluCPF.Text;
            string email = aluemail.Text;
            string senha = alusenha.Password;
            string turma = cmbTurma.SelectionBoxItem.ToString();
            int idade = int.Parse(aluidade.Text);




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
                INSERT INTO alunos
                (Nome, CPF, Email, Senha, Turma, idade)
                VALUES
                (@Nome, @CPF, @Email, @Senha, @Turma, @idade)";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@Nome", nome);
                    cmd.Parameters.AddWithValue("@CPF", cpf);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Senha", senha);
                    cmd.Parameters.AddWithValue("@Turma", turma);
                    cmd.Parameters.AddWithValue("@idade", idade);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Aluno cadastrado com sucesso!");
                    datagridalu();
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }



        public void datagridalu()
        {

            string conexaoString =
      "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "SELECT * FROM alunos";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    datagridalunos.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }





            }

        }
        private void exclualuno(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            DataRowView linha = btn.DataContext as DataRowView;

            int id = Convert.ToInt32(linha["Id_aluno"]);

            string conexaoString =
                "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "DELETE FROM alunos WHERE Id_aluno = @id";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Aluno excluído com sucesso!");

                    datagridalu(); // recarrega o grid
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }


        private void pesquisaraluno(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(alupesquisar.Text))
            {

                string nome = alupesquisar.Text;
                string conexaoString =
                    "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";
                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();
                        string sql = "SELECT * FROM alunos WHERE Nome LIKE @nome";
                        MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);
                        da.SelectCommand.Parameters.AddWithValue("@nome", "%" + nome + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        datagridalunos.ItemsSource = dt.DefaultView;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }





                }

            }
            else {

                datagridalu();



            }
        }

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {

            aluCPF.Clear();
            alunome.Clear();    
            aluemail.Clear();   
            aluidade.Clear();
            alusenha.Clear();
            cmbTurma.SelectedIndex = -1;



        }

        private bool alterandoCpf = false;


        private void alucpf_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (alterandoCpf)
                return;

            alterandoCpf = true;

            TextBox txt = sender as TextBox;

            // Remove tudo que não é número
            string numeros = new string(txt.Text.Where(char.IsDigit).ToArray());

            // Limita em 11 dígitos
            if (numeros.Length > 11)
                numeros = numeros.Substring(0, 11);

            string cpfFormatado = "";

            if (numeros.Length <= 3)
            {
                cpfFormatado = numeros;
            }
            else if (numeros.Length <= 6)
            {
                cpfFormatado = numeros.Insert(3, ".");
            }
            else if (numeros.Length <= 9)
            {
                cpfFormatado = numeros.Insert(3, ".").Insert(7, ".");
            }
            else
            {
                cpfFormatado = numeros.Insert(3, ".").Insert(7, ".").Insert(11, "-");
            }

            txt.Text = cpfFormatado;
            txt.SelectionStart = txt.Text.Length;

            alterandoCpf = false;
        }

    }
}




