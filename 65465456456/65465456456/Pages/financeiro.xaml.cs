using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static _65465456456.banco;

namespace _65465456456.Pages
{
    /// <summary>
    /// Interação lógica para financeiro.xam
    /// </summary>
    public partial class financeiro : UserControl
    {
        public financeiro()
        {
            InitializeComponent();

            dpData.SelectedDate = DateTime.Today;
            cmbTipo.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;

            CarregarDados();
        }

        // Classe auxiliar para cada lançamento financeiro
        public class Lancamento
        {
            public int Id_lanc { get; set; }
            public string Descricao { get; set; }
            public string Categoria { get; set; }
            public string Tipo { get; set; }
            public decimal Valor { get; set; }
            public string Status { get; set; }
            public DateTime Data_lancamento { get; set; }
        }

        // Carrega a grid de movimentações e recalcula todos os indicadores
        private void CarregarDados()
        {
            var lista = new List<Lancamento>();

            using (var conn = MySQLConnection.GetConnection())
            {
                string query = @"SELECT Id_lanc, Descricao, Categoria, Tipo, Valor, Status, Data_lancamento
                                  FROM financeiro
                                  ORDER BY Data_lancamento DESC, Id_lanc DESC";

                var cmd = new MySqlCommand(query, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Lancamento
                    {
                        Id_lanc = reader.GetInt32("Id_lanc"),
                        Descricao = reader.GetString("Descricao"),
                        Categoria = reader.IsDBNull(reader.GetOrdinal("Categoria")) ? "" : reader.GetString("Categoria"),
                        Tipo = reader.GetString("Tipo"),
                        Valor = reader.GetDecimal("Valor"),
                        Status = reader.GetString("Status"),
                        Data_lancamento = reader.GetDateTime("Data_lancamento")
                    });
                }
            }

            dgFinanceiro.ItemsSource = lista;
            AtualizarIndicadores(lista);
        }

        // Recalcula cards, barras de progresso e resumo do mês a partir da lista carregada
        private void AtualizarIndicadores(List<Lancamento> lista)
        {
            decimal receitas = lista.Where(l => l.Tipo == "Receita" && l.Status == "Pago").Sum(l => l.Valor);
            decimal despesas = lista.Where(l => l.Tipo == "Despesa" && l.Status == "Pago").Sum(l => l.Valor);
            decimal saldo = receitas - despesas;
            int pendentes = lista.Count(l => l.Status == "Pendente");

            int mensalidadesPagas = lista.Count(l => l.Categoria == "Mensalidade" && l.Status == "Pago");
            int mensalidadesPendentes = lista.Count(l => l.Categoria == "Mensalidade" && l.Status == "Pendente");
            decimal previsaoRecebimento = lista.Where(l => l.Categoria == "Mensalidade" && l.Status == "Pendente").Sum(l => l.Valor);

            txtReceitas.Text = receitas.ToString("C2", CultureInfo.CurrentCulture);
            txtDespesas.Text = despesas.ToString("C2", CultureInfo.CurrentCulture);
            txtSaldo.Text = saldo.ToString("C2", CultureInfo.CurrentCulture);
            txtPendentes.Text = pendentes.ToString();

            txtPagas.Text = mensalidadesPagas.ToString();
            txtPendentesMes.Text = mensalidadesPendentes.ToString();
            txtPrevisao.Text = previsaoRecebimento.ToString("C2", CultureInfo.CurrentCulture);

            decimal totalMovimentado = receitas + despesas;
            pbReceitas.Value = totalMovimentado > 0 ? (double)(receitas / totalMovimentado * 100) : 0;
            pbDespesas.Value = totalMovimentado > 0 ? (double)(despesas / totalMovimentado * 100) : 0;

            int totalMensalidades = mensalidadesPagas + mensalidadesPendentes;
            pbMensalidades.Value = totalMensalidades > 0 ? (double)mensalidadesPagas / totalMensalidades * 100 : 0;

            decimal metaTotal = receitas + previsaoRecebimento;
            pbMeta.Value = metaTotal > 0 ? (double)(receitas / metaTotal * 100) : 0;
        }

        // Botão Salvar Lançamento
        private void btnSalvarLancamento_Click(object sender, RoutedEventArgs e)
        {
            string descricao = txtDescricao.Text.Trim();
            string categoria = cmbCategoria.Text.Trim();
            string tipo = (cmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(descricao) ||
                string.IsNullOrWhiteSpace(tipo) ||
                string.IsNullOrWhiteSpace(status) ||
                dpData.SelectedDate == null ||
                string.IsNullOrWhiteSpace(txtValor.Text))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios.");
                return;
            }

            if (!decimal.TryParse(txtValor.Text.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valor) || valor <= 0)
            {
                MessageBox.Show("Informe um valor numérico válido para o lançamento.");
                return;
            }

            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    string query = @"INSERT INTO financeiro
                                    (Descricao, Categoria, Tipo, Valor, Status, Data_lancamento)
                                    VALUES (@Descricao, @Categoria, @Tipo, @Valor, @Status, @Data_lancamento)";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Descricao", descricao);
                    cmd.Parameters.AddWithValue("@Categoria", categoria);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                    cmd.Parameters.AddWithValue("@Valor", valor);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Data_lancamento", dpData.SelectedDate.Value.Date);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Lançamento salvo com sucesso!");
                    LimparCampos();
                    CarregarDados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar: " + ex.Message);
            }
        }

        // Botão Limpar Tudo
        private void btnLimparLancamento_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void LimparCampos()
        {
            txtDescricao.Clear();
            cmbCategoria.SelectedIndex = -1;
            cmbCategoria.Text = "";
            cmbTipo.SelectedIndex = 0;
            txtValor.Clear();
            dpData.SelectedDate = DateTime.Today;
            cmbStatus.SelectedIndex = 0;
        }

        // Exclui o lançamento da linha correspondente
        private void excluirLancamento(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Lancamento item = btn?.DataContext as Lancamento;

            if (item == null)
                return;

            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    string query = "DELETE FROM financeiro WHERE Id_lanc = @id";
                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", item.Id_lanc);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Lançamento excluído com sucesso!");
                CarregarDados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir: " + ex.Message);
            }
        }

        // Pesquisa por descrição ou categoria
        private void txtPesquisar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPesquisar.Text))
            {
                CarregarDados();
                return;
            }

            string termo = txtPesquisar.Text;
            var lista = new List<Lancamento>();

            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    string query = @"SELECT Id_lanc, Descricao, Categoria, Tipo, Valor, Status, Data_lancamento
                                      FROM financeiro
                                      WHERE Descricao LIKE @termo OR Categoria LIKE @termo
                                      ORDER BY Data_lancamento DESC, Id_lanc DESC";

                    var cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@termo", "%" + termo + "%");
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Lancamento
                        {
                            Id_lanc = reader.GetInt32("Id_lanc"),
                            Descricao = reader.GetString("Descricao"),
                            Categoria = reader.IsDBNull(reader.GetOrdinal("Categoria")) ? "" : reader.GetString("Categoria"),
                            Tipo = reader.GetString("Tipo"),
                            Valor = reader.GetDecimal("Valor"),
                            Status = reader.GetString("Status"),
                            Data_lancamento = reader.GetDateTime("Data_lancamento")
                        });
                    }
                }

                dgFinanceiro.ItemsSource = lista;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na pesquisa: " + ex.Message);
            }
        }
    }
}
