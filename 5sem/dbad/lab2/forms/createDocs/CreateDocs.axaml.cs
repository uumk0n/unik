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
using System.Linq;
using System.Threading;


namespace lab2;

public partial class CreateDocs : Window
{
    private CheckBox m_checkBox1;
    private CheckBox m_checkBox2;
    private CheckBox m_checkBox3;
    private CheckBox m_checkBox4;
    private List<string> tableNames;
    DataTable combinedDataTable;
    DataHandler dataHandler;
    private List<string> tableNamesFromDb;
    Dictionary<string, string> columnMapping;
    public CreateDocs()
    {
        InitializeComponent();
        LoadData();
        columnMapping = ReadJsonFile("ruColumns.json");
    }

    static Dictionary<string, string> ReadJsonFile(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void LoadData()
    {
        dataHandler = new DataHandler("accounting_for_leased_premises.db");
        tableNamesFromDb = dataHandler.GetTableNames();
        try
        {

            m_checkBox1 = this.FindControl<CheckBox>("checkBox1");
            m_checkBox2 = this.FindControl<CheckBox>("checkBox2");
            m_checkBox3 = this.FindControl<CheckBox>("checkBox3");
            m_checkBox4 = this.FindControl<CheckBox>("checkBox4");

            m_checkBox1.Content = tableNamesFromDb[0];
            m_checkBox2.Content = tableNamesFromDb[2];
            m_checkBox3.Content = tableNamesFromDb[3];
            m_checkBox4.Content = tableNamesFromDb[4];

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void GenerateWord()
    {
        using (DocX doc = DocX.Create("docs/output.docx"))
        {
            ExportDataTableToWord("Отчёт", combinedDataTable, doc);
            // Сохраняем документ после добавления всех таблиц
            doc.Save();
        }
    }

    private void ExportDataTableToWord(string tableName, DataTable dataTable, DocX doc)
    {
        doc.InsertParagraph(tableName).Bold().FontSize(14d).SpacingAfter(10d);

        Table table = doc.AddTable(dataTable.Rows.Count + 1, dataTable.Columns.Count);

        // Add column headers
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            table.Rows[0].Cells[i].Paragraphs[0].Append(columnMapping[dataTable.Columns[i].ColumnName.ToLower()]).Bold();
        }

        // Add data from DataTable
        for (int row = 0; row < dataTable.Rows.Count; row++)
        {
            for (int col = 0; col < dataTable.Columns.Count; col++)
            {
                table.Rows[row + 1].Cells[col].Paragraphs[0].Append(dataTable.Rows[row][col].ToString());
            }
        }

        // Add the table to the document
        doc.InsertTable(table);
    }


    private void GenerateXlsx()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo("docs/output.xlsx")))
        {

            if (package.Workbook.Worksheets["Отчёт"] != null)
            {
                package.Workbook.Worksheets.Delete(package.Workbook.Worksheets["Отчёт"]);
            }
            ExportDataTableToXlsx("Отчёт", combinedDataTable, package);



            // Сохраняем файл XLSX после добавления всех листов
            package.Save();
        }
    }

    private void ExportDataTableToXlsx(string tableName, DataTable dataTable, ExcelPackage package)
    {
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(tableName);

        // Add headers
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            worksheet.Cells[1, i + 1].Value = columnMapping[dataTable.Columns[i].ColumnName.ToLower()];
        }

        // Add data
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
            tableNames.Add(tableNamesFromDb[0]);
        }
        if (m_checkBox2.IsChecked == true)
        {
            counter++;
            tableNames.Add(tableNamesFromDb[2]);
        }
        if (m_checkBox3.IsChecked == true)
        {
            counter++;
            tableNames.Add(tableNamesFromDb[3]);
        }
        if (m_checkBox4.IsChecked == true)
        {
            counter++;
            tableNames.Add(tableNamesFromDb[4]);
        }

        if (counter < 3)
        {
            MsgBox msgBox = new MsgBox("Ошибка", "Необходимо выбрать хотя бы 3 таблицы", false);
            msgBox.Show();
        }
        else
        {

            var datePickerStart = this.FindControl<DatePicker>("datePickerStart");
            var datePickerEnd = this.FindControl<DatePicker>("datePickerEnd");
            if (datePickerStart.SelectedDate == null || datePickerEnd.SelectedDate == null)
            {
                MsgBox msgBox1 = new MsgBox("Ошибка", "Необходимо выбрать дату", false);
                msgBox1.Show();
                return;
            }

            DateTime startDate = datePickerStart.SelectedDate?.DateTime ?? DateTime.MinValue;
            DateTime endDate = datePickerEnd.SelectedDate?.DateTime ?? DateTime.MaxValue;
            combinedDataTable = dataHandler.CreateCombinedTable(startDate, endDate, tableNames);

            Thread wordThread = new Thread(() => GenerateWord());
            Thread xlsxThread = new Thread(() => GenerateXlsx());

            // Запускаем потоки
            wordThread.Start();
            xlsxThread.Start();

            // Дожидаемся завершения обоих потоков
            wordThread.Join();
            xlsxThread.Join();

            MsgBox msgBox = new MsgBox("Успешно", "Документы созданы", false);
            msgBox.Show();

            Close();
        }
    }
}