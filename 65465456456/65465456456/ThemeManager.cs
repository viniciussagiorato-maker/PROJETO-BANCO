using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace _65465456456
{
    public enum TemaApp
    {
        Claro,
        Escuro
    }

    /// <summary>
    /// Controla o tema (claro/escuro) do aplicativo inteiro.
    /// Troca o ResourceDictionary de cores no nível da Application,
    /// então qualquer tela que use {DynamicResource ...} é atualizada
    /// automaticamente, mesmo telas já abertas.
    /// </summary>
    public static class ThemeManager
    {
        private const string ArquivoPreferencia = "tema.cfg";

        public static TemaApp TemaAtual { get; private set; } = TemaApp.Claro;

        public static void AplicarTema(TemaApp tema, bool salvarPreferencia = true)
        {
            string caminho = tema == TemaApp.Escuro
                ? "Themes/DarkTheme.xaml"
                : "Themes/LightTheme.xaml";

            var novoDicionario = new ResourceDictionary
            {
                Source = new Uri(caminho, UriKind.Relative)
            };

            var dicionarios = Application.Current.Resources.MergedDictionaries;

            // Remove o dicionário de tema anterior (identificado pelo nome do arquivo)
            var anterior = dicionarios.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.EndsWith("LightTheme.xaml") ||
                 d.Source.OriginalString.EndsWith("DarkTheme.xaml")));

            if (anterior != null)
                dicionarios.Remove(anterior);

            dicionarios.Add(novoDicionario);
            TemaAtual = tema;

            if (salvarPreferencia)
                SalvarPreferencia(tema);
        }

        private static void SalvarPreferencia(TemaApp tema)
        {
            try
            {
                File.WriteAllText(CaminhoArquivoPreferencia(), tema.ToString());
            }
            catch
            {
                // Preferência de tema não é essencial; ignora falha de escrita em disco.
            }
        }

        /// <summary>
        /// Lê a preferência salva (se existir) e aplica na inicialização do app.
        /// </summary>
        public static void CarregarPreferenciaSalva()
        {
            try
            {
                var caminho = CaminhoArquivoPreferencia();
                if (File.Exists(caminho))
                {
                    var texto = File.ReadAllText(caminho).Trim();
                    if (Enum.TryParse<TemaApp>(texto, out var temaSalvo))
                    {
                        AplicarTema(temaSalvo, salvarPreferencia: false);
                        return;
                    }
                }
            }
            catch
            {
                // Se der erro lendo o arquivo, cai no tema padrão abaixo.
            }

            AplicarTema(TemaApp.Claro, salvarPreferencia: false);
        }

        private static string CaminhoArquivoPreferencia()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ArquivoPreferencia);
        }
    }
}
