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
    /// Interação lógica para PageInicio.xam
    /// </summary>
    public partial class PageInicio : UserControl
    {
        public PageInicio()
        {
            InitializeComponent();
            menupequeno();
        }



        private void menupequeno() {


            string conexao = "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            try
            {
                using (MySqlConnection conexaoBanco = new MySqlConnection(conexao))
                {
                    conexaoBanco.Open();

                    string sql = "SELECT COUNT(*) FROM alunos";

                    MySqlCommand cmd = new MySqlCommand(sql, conexaoBanco);

                    int numAlunos = Convert.ToInt32(cmd.ExecuteScalar());

                    blockalunos.Text = $"Alunos: {numAlunos}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }





        }
    }
}
