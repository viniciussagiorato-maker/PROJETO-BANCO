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
    /// Interação lógica para PageProfessores.xam
    /// </summary>
    public partial class PageProfessores : UserControl
    {
        public PageProfessores()
        {
            InitializeComponent();
            datagridpro1();
        }

        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {

            string nome = pronome.Text;
            string cpf = procpf.Text;
            string email = proemail.Text;
            string senha = prosenha.Password;
            string Forma = proformacao.Text;
            string Disci = cmbDisciplina.SelectionBoxItem.ToString();

         


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
                (Nome_prof, CPF, Email, Senha, Formacao, Disciplina)
                VALUES
                (@Nome_prof, @CPF, @Email, @Senha, @Formacao, @Disciplina)";

                        MySqlCommand cmd = new MySqlCommand(sql, conexao);

                        cmd.Parameters.AddWithValue("@Nome_prof", nome);
                        cmd.Parameters.AddWithValue("@CPF", cpf);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Senha", senha);
                        cmd.Parameters.AddWithValue("@Formacao", Forma);
                        cmd.Parameters.AddWithValue("@Disciplina", Disci);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Professor cadastrado com sucesso!");
                        datagridpro1();
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }


                }
     


        }
        public void datagridpro1()
        {

            string conexaoString =
      "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "SELECT * FROM professores";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    datagridpro.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }



            }
        }


        private void pesquisaraluno(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPesquisar.Text))
            {

                string nome = txtPesquisar.Text;
                string conexaoString =
                    "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";
                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();
                        string sql = "SELECT * FROM professores WHERE Nome_prof LIKE @nome";
                        MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);
                        da.SelectCommand.Parameters.AddWithValue("@nome", "%" + nome + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        datagridpro.ItemsSource = dt.DefaultView;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }
                }

            }
            else
            {

                datagridpro1();

            }

        }

      

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {

            procpf.Clear();
            pronome.Clear();
            proemail.Clear();
            procpf.Clear();
            proformacao.Clear();
            prosenha.Clear();   
            cmbDisciplina.SelectedIndex = -1;


        }


        private bool alterandoCpf = false;


        private void procpf_TextChanged(object sender, TextChangedEventArgs e)
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
        private void exclupro(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataRowView linha = btn.DataContext as DataRowView;

            // Usando o Id_prof para garantir exclusão precisa
            int idProf = Convert.ToInt32(linha["Id_prof"]);

            string conexaoString = "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "DELETE FROM professores WHERE Id_prof = @idProf";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);
                    cmd.Parameters.AddWithValue("@idProf", idProf);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Professor excluído com sucesso!");

                    datagridpro1(); // recarrega o grid
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

    }
}
