using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using OfficeOpenXml;
using Xceed.Document.NET;
using Xceed.Words.NET;


namespace lab1;

public partial class CreateDocs : Window
{
    private DataSetService dataSetService;

    private CheckBox m_checkBox1;
    private CheckBox m_checkBox2;
    private CheckBox m_checkBox3;
    private CheckBox m_checkBox4;
    private List<string> tableNames;
    public CreateDocs()
    {
        InitializeComponent();
        LoadData();

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void LoadData()
    {
        string path = "accounting_for_leased_premises.json";
        dataSetService = new DataSetService();
        try
        {
            var json = File.ReadAllText(path);
            dataSetService.dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            m_checkBox1 = this.FindControl<CheckBox>("checkBox1");
            m_checkBox2 = this.FindControl<CheckBox>("checkBox2");
            m_checkBox3 = this.FindControl<CheckBox>("checkBox3");
            m_checkBox4 = this.FindControl<CheckBox>("checkBox4");

            m_checkBox1.Content = dataSetService.dataSet.Tables["Building"].TableName;
            m_checkBox2.Content = dataSetService.dataSet.Tables["Room"].TableName;
            m_checkBox3.Content = dataSetService.dataSet.Tables["Renter"].TableName;
            m_checkBox4.Content = dataSetService.dataSet.Tables["Rent"].TableName;



        }
        catch (Exception e)
        {
            MsgBox msgBox = new MsgBox("Ошибка", e.Message, false);
            msgBox.Show();
        }
    }
    private void GenerateWord()
    {
        using (DocX doc = DocX.Create("docs/output.docx"))
        {
            foreach (string tableName in tableNames)
            {
                ExportDataTableToWord(tableName, doc);
            }

            // Сохраняем документ после добавления всех таблиц
            doc.Save();
        }
    }

    private void ExportDataTableToWord(string tableName, DocX doc)
    {
        DataTable dataTable = dataSetService.dataSet.Tables[tableName];

        doc.InsertParagraph(tableName).Bold().FontSize(14d).SpacingAfter(10d);

        Table table = doc.AddTable(dataTable.Rows.Count + 1, dataTable.Columns.Count);

        // Добавляем заголовки столбцов
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            table.Rows[0].Cells[i].Paragraphs[0].Append(dataTable.Columns[i].ColumnName).Bold();
        }

        // Добавляем данные из DataTable
        for (int row = 0; row < dataTable.Rows.Count; row++)
        {
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                table.Rows[row + 1].Cells[col].Paragraphs[0].Append(dataTable.Rows[row][col].ToString());
            }
        }

        // Добавляем таблицу в документ
        doc.InsertTable(table);
    }

    private void GenerateXlsx()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("docs/output.xlsx")))
        {
            foreach (string tableName in tableNames)
            {
                var existingSheet = package.Workbook.Worksheets[tableName];
                if (existingSheet != null)
                {
                    package.Workbook.Worksheets.Delete(existingSheet);
                }
                ExportDataTableToXlsx(tableName, package);
            }

            // Сохраняем файл XLSX после добавления всех листов
            package.Save();
        }
    }

    private void ExportDataTableToXlsx(string tableName, ExcelPackage package)
    {
        DataTable dataTable = dataSetService.dataSet.Tables[tableName];
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(tableName);

        // Добавляем заголовки столбцов
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
        }

        // Добавляем данные из DataTable
        for (int row = 0; row < dataTable.Rows.Count; row++)
        {
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
            }
        }
    }


    private async void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int counter = 0;
        tableNames = new List<string>() { };
        if (m_checkBox1.IsChecked == true)
        {
            counter++;
            tableNames.Add(dataSetService.dataSet.Tables["Building"].TableName);
        }
        if (m_checkBox2.IsChecked == true)
        {
            counter++;
            tableNames.Add(dataSetService.dataSet.Tables["Room"].TableName);
        }
        if (m_checkBox3.IsChecked == true)
        {
            counter++;
            tableNames.Add(dataSetService.dataSet.Tables["Renter"].TableName);
        }
        if (m_checkBox4.IsChecked == true)
        {
            counter++;
            tableNames.Add(dataSetService.dataSet.Tables["Rent"].TableName);
        }

        if (counter < 3)
        {
            MsgBox msgBox = new MsgBox("Ошибка", "Необходимо выбрать хотя бы 3 таблицы", false);
            msgBox.Show();
        }
        else
        {
            Task wordTask = Task.Run(() => GenerateWord());
            Task xlsxTask = Task.Run(() => GenerateXlsx());

            // Дожидаемся завершения обеих задач
            await Task.WhenAll(wordTask, xlsxTask);

            MsgBox msgBox = new MsgBox("Успешно", "Документы созданы", false);
            msgBox.Show();

            Close();
        }
    }

}