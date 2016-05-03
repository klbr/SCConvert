using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using SCConverter.Helpers;
using SCConverter.Models;

namespace SCConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Arquivos PDF|*.pdf";
            if (openFileDialog.ShowDialog() ?? false)
            {
                fileTextBlock.Text = openFileDialog.FileName;
            }

            try
            {
                ConvertButton.IsEnabled = !string.IsNullOrEmpty(fileTextBlock.Text) && File.Exists(fileTextBlock.Text);
            }
            catch
            {
                ConvertButton.IsEnabled = false;
            }
        }

        private async void ConvertFile_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var converter = new ConverterHelper();
            converter.Progress += Converter_Progress;
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Arquivos CSV|*.csv";
                saveFileDialog.FileName = fileTextBlock.Text.Remove(fileTextBlock.Text.LastIndexOf('.')) + ".csv";
                if (saveFileDialog.ShowDialog() ?? false)
                {
                    var type = insumosRadioButton.IsChecked ?? false ? PdfType.FeedStock : PdfType.Composition;
                    await converter.Convert(fileTextBlock.Text, saveFileDialog.FileName, type);
                    if (MessageBox.Show(string.Format("Arquivo '{0}' convertido com sucesso. \nDeseja Abrí-lo?", Path.GetFileName(saveFileDialog.FileName)), "Conversor", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
                    {
                        Process.Start(saveFileDialog.FileName);
                    }
                    fileTextBlock.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro na Conversão", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            finally
            {
                this.IsEnabled = true;
                converter.Progress -= Converter_Progress;
                converter = null;
                progressTextBlock.Text = "";
                converterProgress.Value = 0;
            }
        }

        private void Converter_Progress(object sender, double value)
        {
            if (sender is string)
            {
                progressTextBlock.Text = sender as string;
            }
            converterProgress.Value = value * 100;
        }
    }
}
