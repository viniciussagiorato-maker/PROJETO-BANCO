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
    /// Interação lógica para PageAgenda.xam
    /// </summary>
    public partial class PageAgenda : UserControl
    {
        private static readonly string conexaoString =
            "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

        private int? idEmEdicao = null;

        public PageAgenda()
        {
            InitializeComponent();
            carregarTurmas();
            datagridagenda1();
            atualizarIndicadores();
        }

        private void carregarTurmas()
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = "SELECT Id_turma, Nome_turma FROM turmas ORDER BY Nome_turma";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbTurmaEvento.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar turmas: " + ex.Message);
                }
            }
        }

        private void btnCadastrar_Click(object sender, RoutedEventArgs e)
        {
            string titulo = agetitulo.Text;
            string tipo = cmbTipo.SelectionBoxItem?.ToString();
            string turmaNome = (cmbTurmaEvento.SelectedItem as DataRowView)?["Nome_turma"]?.ToString();
            string hora = agehora.Text;
            string descricao = agedescricao.Text;

            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(tipo) ||
                dtData.SelectedDate == null)
            {
                MessageBox.Show("Preencha ao menos Título, Tipo e Data.");
                return;
            }

            DateTime data = dtData.SelectedDate.Value;

            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql;

                    if (idEmEdicao == null)
                    {
                        sql = @"
                INSERT INTO agenda
                (Titulo, Descricao, Data_evento, Hora_evento, Tipo, Turma)
                VALUES
                (@Titulo, @Descricao, @Data_evento, @Hora_evento, @Tipo, @Turma)";
                    }
                    else
                    {
                        sql = @"
                UPDATE agenda SET
                Titulo = @Titulo,
                Descricao = @Descricao,
                Data_evento = @Data_evento,
                Hora_evento = @Hora_evento,
                Tipo = @Tipo,
                Turma = @Turma
                WHERE Id_evento = @Id_evento";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, conexao);

                    cmd.Parameters.AddWithValue("@Titulo", titulo);
                    cmd.Parameters.AddWithValue("@Descricao", string.IsNullOrWhiteSpace(descricao) ? (object)DBNull.Value : descricao);
                    cmd.Parameters.AddWithValue("@Data_evento", data.Date);
                    cmd.Parameters.AddWithValue("@Hora_evento", string.IsNullOrWhiteSpace(hora) ? (object)DBNull.Value : hora);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                    cmd.Parameters.AddWithValue("@Turma", string.IsNullOrWhiteSpace(turmaNome) ? (object)DBNull.Value : turmaNome);

                    if (idEmEdicao != null)
                    {
                        cmd.Parameters.AddWithValue("@Id_evento", idEmEdicao.Value);
                    }

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(idEmEdicao == null
                        ? "Evento adicionado com sucesso!"
                        : "Evento atualizado com sucesso!");

                    limparFormulario();
                    datagridagenda1();
                    atualizarIndicadores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        public void datagridagenda1()
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    string sql = @"
                SELECT Id_evento, Titulo, Descricao, Data_evento, Hora_evento, Tipo, Turma
                FROM agenda
                ORDER BY Data_evento ASC, Hora_evento ASC";

                    MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    datagridagenda.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private void pesquisarevento(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPesquisar.Text))
            {
                string titulo = txtPesquisar.Text;

                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();

                        string sql = @"
                SELECT Id_evento, Titulo, Descricao, Data_evento, Hora_evento, Tipo, Turma
                FROM agenda
                WHERE Titulo LIKE @titulo
                ORDER BY Data_evento ASC, Hora_evento ASC";

                        MySqlDataAdapter da = new MySqlDataAdapter(sql, conexao);
                        da.SelectCommand.Parameters.AddWithValue("@titulo", "%" + titulo + "%");

                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        datagridagenda.ItemsSource = dt.DefaultView;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.Message);
                    }
                }
            }
            else
            {
                datagridagenda1();
            }
        }

        private void atualizarIndicadores()
        {
            using (MySqlConnection conexao = new MySqlConnection(conexaoString))
            {
                try
                {
                    conexao.Open();

                    MySqlCommand cmdMes = new MySqlCommand(
                        "SELECT COUNT(*) FROM agenda WHERE MONTH(Data_evento) = MONTH(CURDATE()) AND YEAR(Data_evento) = YEAR(CURDATE())",
                        conexao);
                    txtEventosMes.Text = Convert.ToString(cmdMes.ExecuteScalar() ?? 0);

                    MySqlCommand cmdProvas = new MySqlCommand(
                        "SELECT COUNT(*) FROM agenda WHERE Tipo = 'Prova' AND Data_evento >= CURDATE()",
                        conexao);
                    txtProvas.Text = Convert.ToString(cmdProvas.ExecuteScalar() ?? 0);

                    MySqlCommand cmdReunioes = new MySqlCommand(
                        "SELECT COUNT(*) FROM agenda WHERE Tipo = 'Reunião' AND Data_evento >= CURDATE()",
                        conexao);
                    txtReunioes.Text = Convert.ToString(cmdReunioes.ExecuteScalar() ?? 0);

                    MySqlCommand cmdProximo = new MySqlCommand(
                        "SELECT Titulo, Data_evento FROM agenda WHERE Data_evento >= CURDATE() ORDER BY Data_evento ASC, Hora_evento ASC LIMIT 1",
                        conexao);

                    using (var reader = cmdProximo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DateTime data = Convert.ToDateTime(reader["Data_evento"]);
                            txtProximoEvento.Text = $"{reader["Titulo"]} ({data:dd/MM})";
                        }
                        else
                        {
                            txtProximoEvento.Text = "—";
                        }
                    }
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
                idEmEdicao = Convert.ToInt32(linha["Id_evento"]);

                agetitulo.Text = linha["Titulo"]?.ToString();
                agedescricao.Text = linha["Descricao"]?.ToString();
                agehora.Text = linha["Hora_evento"]?.ToString();

                if (linha["Data_evento"] != DBNull.Value)
                {
                    dtData.SelectedDate = Convert.ToDateTime(linha["Data_evento"]);
                }

                foreach (ComboBoxItem item in cmbTipo.Items)
                {
                    if (item.Content?.ToString() == linha["Tipo"]?.ToString())
                    {
                        cmbTipo.SelectedItem = item;
                        break;
                    }
                }

                string turmaNome = linha["Turma"]?.ToString();
                if (!string.IsNullOrEmpty(turmaNome))
                {
                    foreach (DataRowView turmaRow in cmbTurmaEvento.Items)
                    {
                        if (turmaRow["Nome_turma"]?.ToString() == turmaNome)
                        {
                            cmbTurmaEvento.SelectedItem = turmaRow;
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
                int id = Convert.ToInt32(linha["Id_evento"]);

                var resultado = MessageBox.Show(
                    "Deseja realmente excluir este evento?",
                    "Confirmar exclusão",
                    MessageBoxButton.YesNo);

                if (resultado != MessageBoxResult.Yes)
                    return;

                using (MySqlConnection conexao = new MySqlConnection(conexaoString))
                {
                    try
                    {
                        conexao.Open();

                        string sql = "DELETE FROM agenda WHERE Id_evento = @id";
                        MySqlCommand cmd = new MySqlCommand(sql, conexao);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        datagridagenda1();
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
            agetitulo.Clear();
            agedescricao.Clear();
            agehora.Clear();
            dtData.SelectedDate = null;
            cmbTipo.SelectedIndex = -1;
            cmbTurmaEvento.SelectedIndex = -1;
            idEmEdicao = null;
            btnCadastrar.Content = "Adicionar Evento";
        }

        private void btnLimpar_Click(object sender, RoutedEventArgs e)
        {
            limparFormulario();
        }
    }
}
