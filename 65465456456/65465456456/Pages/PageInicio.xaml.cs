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



        private void menupequeno()
        {

            
            string conexao = "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            try
            {
                using (MySqlConnection conexaoBanco = new MySqlConnection(conexao))
                {
                    conexaoBanco.Open();

                    // Alunos
                    MySqlCommand cmdAlunos = new MySqlCommand("SELECT COUNT(*) FROM alunos", conexaoBanco);
                    blockalunos.Text = "Alunos: " + cmdAlunos.ExecuteScalar().ToString();

                    // Professores
                    MySqlCommand cmdProf = new MySqlCommand("SELECT COUNT(*) FROM professores", conexaoBanco);
                    blockprof.Text = "Professores: " + cmdProf.ExecuteScalar().ToString();



                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }





    }
}

