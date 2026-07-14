using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
    /// Interação lógica para PageTurmas.xam
    /// </summary>
    public partial class PageTurmas : UserControl
    {
        private static readonly string conexaoString =
            "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

        private int? idEmEdicao = null;

        public PageTurmas()
        {
            InitializeComponent();
            carregarProfessores();
            datagridturmas1();
            atualizarIndicadores();
        }

        private void carregarProfessores()
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "SELECT Id_prof, Nome_prof FROM professores ORDER BY Nome_prof";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbProfessor.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar professores: " + ex.Message);
                }
            }
        }

        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = turnome.Text;
            string serie = cmbSerie.SelectionBoxItem?.ToString();
            string turno = cmbTurno.SelectionBoxItem?.ToString();
            string sala = tursala.Text;
            string capacidadeTexto = turcapacidade.Text;
            object idProf = cmbProfessor.SelectedValue;

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(serie) ||
                string.IsNullOrWhiteSpace(turno))
            {
                MessageBox.Show("Preencha ao menos Nome, Série e Turno.");
                return;
            }

            int? capacidade = null;
            if (!string.IsNullOrWhiteSpace(capacidadeTexto))
            {
                if (int.TryParse(capacidadeTexto, out int capacidadeValor))
                {
                    capacidade = capacidadeValor;
                }
                else
                {
                    MessageBox.Show("Capacidade deve ser um número.");
                    return;
                }
            }

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql;

                    if (idEmEdicao == null)
                    {
                        sql = @"
                INSERT INTO turmas
                (Nome_turma, Serie, Turno, Sala, Capacidade, Id_prof)
                VALUES
                (@Nome_turma, @Serie, @Turno, @Sala, @Capacidade, @Id_prof)";
                    }
                    else
                    {
                        sql = @"
                UPDATE turmas SET
                Nome_turma = @Nome_turma,
                Serie = @Serie,
                Turno = @Turno,
                Sala = @Sala,
                Capacidade = @Capacidade,
                Id_prof = @Id_prof
                WHERE Id_turma = @Id_turma";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@Nome_turma", nome);
                    cmd.Parameters.AddWithValue("@Serie", serie);
                    cmd.Parameters.AddWithValue("@Turno", turno);
                    cmd.Parameters.AddWithValue("@Sala", string.IsNullOrWhiteSpace(sala) ? (object)DBNull.Value : sala);
                    cmd.Parameters.AddWithValue("@Capacidade", capacidade.HasValue ? (object)capacidade.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Id_prof", idProf ?? DBNull.Value);

                    if (idEmEdicao != null)
                    {
                        cmd.Parameters.AddWithValue("@Id_turma", idEmEdicao.Value);
                    }

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(idEmEdicao == null
                        ? "Turma cadastrada com sucesso!"
                        : "Turma atualizada com sucesso!");

                    limparFormulario();
                    datagridturmas1();
                    atualizarIndicadores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        public void datagridturmas1()
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = @"
                SELECT t.Id_turma, t.Nome_turma, t.Serie, t.Turno, t.Sala, t.Capacidade,
                       p.Nome_prof AS Professor
                FROM turmas t
                LEFT JOIN professores p ON t.Id_prof = p.Id_prof
                ORDER BY t.Nome_turma";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    datagridturmas.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private void pesquisarturma(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPesquisar.Text))
            {
                string nome = txtPesquisar.Text;

                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();

                        string sql = @"
                SELECT t.Id_turma, t.Nome_turma, t.Serie, t.Turno, t.Sala, t.Capacidade,
                       p.Nome_prof AS Professor
                FROM turmas t
                LEFT JOIN professores p ON t.Id_prof = p.Id_prof
                WHERE t.Nome_turma LIKE @nome
                ORDER BY t.Nome_turma";

                        MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);
                        da.SelectCommand.Parameters.AddWithValue("@nome", "%" + nome + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        datagridturmas.ItemsSource = dt.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }
                }
            }
            else
            {
                datagridturmas1();
            }
        }

        private void atualizarIndicadores()
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    MySqlCommand cmdTotal = new MySqlCommand("SELECT COUNT(*) FROM turmas", conexao);
                    txtTotalTurmas.Text = Convert.ToString(cmdTotal.ExecuteScalar() ?? 0);

                    MySqlCommand cmdAlunos = new MySqlCommand("SELECT COUNT(*) FROM alunos", conexao);
                    txtTotalAlunos.Text = Convert.ToString(cmdAlunos.ExecuteScalar() ?? 0);

                    MySqlCommand cmdManha = new MySqlCommand("SELECT COUNT(*) FROM turmas WHERE Turno = 'Manhã'", conexao);
                    txtTurnoManha.Text = Convert.ToString(cmdManha.ExecuteScalar() ?? 0);

                    MySqlCommand cmdTarde = new MySqlCommand("SELECT COUNT(*) FROM turmas WHERE Turno IN ('Tarde','Noite')", conexao);
                    txtTurnoTarde.Text = Convert.ToString(cmdTarde.ExecuteScalar() ?? 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar indicadores: " + ex.Message);
                }
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView linha)
            {
                idEmEdicao = Convert.ToInt32(linha["Id_turma"]);

                turnome.Text = linha["Nome_turma"]?.ToString();
                tursala.Text = linha["Sala"]?.ToString();
                turcapacidade.Text = linha["Capacidade"]?.ToString();

                foreach (ComboBoxItem item in cmbSerie.Items)
                {
                    if (item.Content?.ToString() == linha["Serie"]?.ToString())
                    {
                        cmbSerie.SelectedItem = item;
                        break;
                    }
                }

                foreach (ComboBoxItem item in cmbTurno.Items)
                {
                    if (item.Content?.ToString() == linha["Turno"]?.ToString())
                    {
                        cmbTurno.SelectedItem = item;
                        break;
                    }
                }

                string professorNome = linha["Professor"]?.ToString();
                if (!string.IsNullOrEmpty(professorNome) && cmbProfessor.Items.Count > 0)
                {
                    foreach (DataRowView profRow in cmbProfessor.Items)
                    {
                        if (profRow["Nome_prof"]?.ToString() == professorNome)
                        {
                            cmbProfessor.SelectedItem = profRow;
                            break;
                        }
                    }
                }

                btnCadastrar.Content = "Salvar Alterações";
            }
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is DataRowView linha)
            {
                int id = Convert.ToInt32(linha["Id_turma"]);

                var resultado = MessageBox.Show(
                    "Deseja realmente excluir esta turma?",
                    "Confirmar exclusão",
                    MessageBoxButton.YesNo);

                if (resultado != MessageBoxResult.Yes)
                    return;

                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();

                        string sql = "DELETE FROM turmas WHERE Id_turma = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conexao);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        datagridturmas1();
                        atualizarIndicadores();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }
                }
            }
        }

        private void limparFormulario()
        {
            turnome.Clear();
            tursala.Clear();
            turcapacidade.Clear();
            cmbSerie.SelectedIndex = -1;
            cmbTurno.SelectedIndex = -1;
            cmbProfessor.SelectedIndex = -1;
            idEmEdicao = null;
            btnCadastrar.Content = "Cadastrar Turma";
        }

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            limparFormulario();
        }
    }
}
