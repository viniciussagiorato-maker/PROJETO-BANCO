using MySql.Data.MySqlClient;
using Mysqlx.Connection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using static _65465456456.banco;

namespace _65465456456.Pages
{
    public partial class PageNotas : UserControl
    {
        public PageNotas()
        {
            InitializeComponent();
            CarregarAlunos();
            CarregarNotas();

        }

        // Classe auxiliar para Aluno
        public class Aluno
        {
            public int Id_aluno { get; set; }
            public string Nome { get; set; }
        }

        // Classe auxiliar para Nota
        public class Nota
        {
            public int Id_nota { get; set; }
            public int Id_aluno { get; set; }
            public string Nome { get; set; }
            public double Matematica { get; set; }
            public double Portugues { get; set; }
            public double Ciencias { get; set; }
            public double Ingles { get; set; }
            public double Geografia { get; set; }
            public double Historia { get; set; }
            public double Media { get; set; }

            public string status { get; set; } = "";
        }

        // Carrega lista de alunos no ComboBox
        private void CarregarAlunos()
        {
            var listaAlunos = new List<Aluno>();

            using (var conn = MySQLConnection.GetConnection())
            {
                string query = "SELECT Id_aluno, Nome FROM alunos";
                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaAlunos.Add(new Aluno
                    {
                        Id_aluno = reader.GetInt32("Id_aluno"),
                        Nome = reader.GetString("Nome")
                    });
                }
            }

            cmbAlunos.ItemsSource = listaAlunos;
        }

        // Botão Salvar
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    string query = @"INSERT INTO notas 
                                    (Id_aluno, Matematica, Portugues, Ciencias, Ingles, Geografia, Historia)
                                    VALUES (@Id_aluno, @Matematica, @Portugues, @Ciencias, @Ingles, @Geografia, @Historia)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id_aluno", cmbAlunos.SelectedValue);
                    cmd.Parameters.AddWithValue("@Matematica", double.Parse(txtMatematica.Text));
                    cmd.Parameters.AddWithValue("@Portugues", double.Parse(txtPortugues.Text));
                    cmd.Parameters.AddWithValue("@Ciencias", double.Parse(txtCiencias.Text));
                    cmd.Parameters.AddWithValue("@Ingles", double.Parse(txtIngles.Text));
                    cmd.Parameters.AddWithValue("@Geografia", double.Parse(txtGeografia.Text));
                    cmd.Parameters.AddWithValue("@Historia", double.Parse(txtHistoria.Text));

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Nota salva com sucesso!");
                    CarregarNotas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }

        // Carrega notas no DataGrid
        private void CarregarNotas()
        {
            var listaNotas = new List<Nota>();

            using (var conn = MySQLConnection.GetConnection())
            {
                string query = @"SELECT n.Id_nota, a.Nome, n.Id_aluno, n.Matematica, n.Portugues, n.Ciencias, 
                                 n.Ingles, n.Geografia, n.Historia, n.Media
                                 FROM notas n
                                 INNER JOIN alunos a ON n.Id_aluno = a.Id_aluno";

                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    listaNotas.Add(new Nota
                    {
                        Id_nota = reader.GetInt32("Id_nota"),
                        Id_aluno = reader.GetInt32("Id_aluno"),
                        Nome = reader.GetString("Nome"),
                        Matematica = reader.GetDouble("Matematica"),
                        Portugues = reader.GetDouble("Portugues"),
                        Ciencias = reader.GetDouble("Ciencias"),
                        Ingles = reader.GetDouble("Ingles"),
                        Geografia = reader.GetDouble("Geografia"),
                        Historia = reader.GetDouble("Historia"),
                        Media = reader.GetDouble("Media"), // já vem calculada do banco
                        status = reader.GetDouble("Media") >= 7 ? "Aprovado" : "Reprovado"

                    });
                }
            }

            DataGridnotas.ItemsSource = listaNotas;
        }



        private void pesquisaraluno(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Buscanota.Text))
            {
                string nome = Buscanota.Text;
                string conexaoString = "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();
                        string sql = @"SELECT a.Nome,
                                      n.Matematica,
                                      n.Portugues,
                                      n.Ciencias,
                                      n.Ingles,
                                      n.Geografia,
                                      n.Historia,
                                      n.Media
                               FROM alunos a
                               INNER JOIN notas n ON a.Id_aluno = n.Id_aluno
                               WHERE a.Nome LIKE @nome";





                        MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);
                        da.SelectCommand.Parameters.AddWithValue("@nome", "%" + nome + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        DataGridnotas.ItemsSource = dt.DefaultView;
                        conexao.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }
                }
            }
            else
            {
                CarregarNotas();
            }
        }


        private void exclunota(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataRowView linha = btn.DataContext as DataRowView;

            int idAluno = Convert.ToInt32(linha["Id_aluno"]);

            string conexaoString = "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "DELETE FROM notas WHERE Id_aluno = @idAluno";

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);
                    cmd.Parameters.AddWithValue("@idAluno", idAluno);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Notas do aluno excluídas com sucesso!");

                    CarregarNotas(); // recarrega o grid
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }


    }


}




