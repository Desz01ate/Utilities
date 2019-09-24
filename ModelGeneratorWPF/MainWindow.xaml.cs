using Microsoft.WindowsAPICodePack.Dialogs;
using ModelGenerator.Services.DesignPattern.Interfaces;
using ModelGeneratorWPF.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModelGeneratorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TargetGeneratorType generatorType { get; set; }
        private TargetLanguage targetLanguage { get; set; }
        private TargetDatabaseConnector targetDatabaseConnector { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            #region initializer
            cb_GeneratorMode.SelectedIndex = 1;
            cb_GeneratorMode.SelectedIndex = 0;
            var supportedDatabase = typeof(TargetDatabaseConnector).GetMembers(BindingFlags.Static | BindingFlags.Public);
            foreach (var database in supportedDatabase)
            {
                if (!(database.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description))
                {
                    cb_TargetDatabase.Items.Add(database.Name);
                }
                else
                {
                    cb_TargetDatabase.Items.Add(description.Description);
                }
            }
            #endregion
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedIndex = cb_GeneratorMode.SelectedIndex;
            if (cb_TargetLang != null)
            {
                cb_TargetLang.Items.Clear();
                var supportedLanguages = typeof(TargetLanguage).GetMembers(BindingFlags.Static | BindingFlags.Public);
                switch (selectedIndex)
                {
                    case 0: //model generator
                        generatorType = TargetGeneratorType.Model;
                        foreach (var langauge in supportedLanguages)
                        {
                            if (!(langauge.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description))
                            {
                                cb_TargetLang.Items.Add(langauge.Name);
                            }
                            else
                            {
                                cb_TargetLang.Items.Add(description.Description);
                            }
                        }
                        break;
                    case 1: //unit of work generator
                        generatorType = TargetGeneratorType.UnitOfWork;
                        foreach (var langauge in supportedLanguages.Where(x => x.Name == "CSharp" || x.Name == "VisualBasic"))
                        {
                            if (!(langauge.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute description))
                            {
                                cb_TargetLang.Items.Add(langauge.Name);
                            }
                            else
                            {
                                cb_TargetLang.Items.Add(description.Description);
                            }
                        }
                        break;
                }
                cb_TargetLang.SelectedIndex = 0;
            }
        }

        private void Cb_TargetLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            targetLanguage = (TargetLanguage)cb_TargetLang.SelectedIndex;
        }

        private void Cb_TargetDatabase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            targetDatabaseConnector = (TargetDatabaseConnector)cb_TargetDatabase.SelectedIndex;
            switch (targetDatabaseConnector)
            {
                case TargetDatabaseConnector.SQLServer:
                    txt_connectionString.Text = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password = myPassword;";
                    break;
                case TargetDatabaseConnector.Oracle:
                    txt_connectionString.Text = "Data Source=MyOracleDB;User Id=myUsername;Password=myPassword;";
                    break;
                case TargetDatabaseConnector.MySQL:
                    txt_connectionString.Text = "Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";
                    break;
                case TargetDatabaseConnector.PostgreSQL:
                    txt_connectionString.Text = "Server=myServerAddress;Port=5432;Database=myDataBase;User Id=myUsername;Password = myPassword;";
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var outputDir = txt_outputDir.Content.ToString();
            if (!Directory.Exists(outputDir))
            {
                MessageBox.Show("You must select an output directory before trying to generate!");
                return;
            }
            btn_Generate.IsEnabled = false;
            btn_Generate.Content = "Generating...";
            try
            {
                switch (generatorType)
                {
                    case TargetGeneratorType.Model:
                        var generator = LangaugesData.GetSpecificGenerator(targetLanguage, targetDatabaseConnector, txt_connectionString.Text, outputDir, txt_namespace.Text);
                        generator.GenerateAllTable();
                        break;
                    case TargetGeneratorType.UnitOfWork:
                        LangaugesData.PerformStrategyGenerate(targetLanguage, targetDatabaseConnector, txt_connectionString.Text, outputDir, txt_namespace.Text);
                        break;
                }
                Process.Start("explorer.exe", outputDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            btn_Generate.Content = "Generate";
            btn_Generate.IsEnabled = true;
        }

        private void Txt_outputDir_MouseDown(object sender, MouseButtonEventArgs e)
        {


        }

        private void Txt_outputDir_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new CommonOpenFileDialog
            {
                InitialDirectory = "C:\\Users",
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txt_outputDir.Content = dialog.FileName;
            }
        }
    }
}
