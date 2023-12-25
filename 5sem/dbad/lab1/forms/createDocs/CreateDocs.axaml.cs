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
using System.ComponentModel.DataAnnotations;


namespace lab1;

public partial class CreateDocs : Window
{
    private DataSetService dataSetService;

    private CheckBox m_checkBox1;
    private CheckBox m_checkBox2;
    private CheckBox m_checkBox3;
    private CheckBox m_checkBox4;
    private List<string> tableNames;
    DataTable combinedDataTable;
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
        string path = "accounting_for_leased_premises.json";
        dataSetService = new DataSetService();
        try
        {
            var json = File.ReadAllText(path);
            DataSetContainer dataSetContainer = JsonConvert.DeserializeObject<DataSetContainer>(json);

            DataSet dataSetFromFile = new DataSet("accounting_for_leased_premises");

            dataSetService.FillTableInfoFromDataSet(dataSetFromFile, dataSetContainer);
            dataSetService.FillDataSetFromDataSetContainer(dataSetFromFile, dataSetContainer);

            dataSetService.dataSet = dataSetFromFile;

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
        if (dataTable == null) { return; }
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
        if (dataTable == null) { return; }
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
            CreateCombinedTable(dataSetService.dataSet, startDate, endDate);

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
    private void CreateCombinedTable(DataSet dataSet, DateTime startDate, DateTime endDate)
    {
        DataTable rentTable = dataSet.Tables["Rent"];
        DataTable roomTable = dataSet.Tables["Room"];
        DataTable renterTable = dataSet.Tables["Renter"];
        DataTable buildingTable = dataSet.Tables["Building"];

        var filteredRentRows = rentTable.AsEnumerable()
            .Where(row =>
            {
                DateTime contractDate = DateTime.Parse(row["ContractDate"].ToString());
                return startDate <= contractDate && contractDate <= endDate;
            });

        combinedDataTable = new DataTable();

        foreach (var tableName in tableNames)
        {
            if (tableName == "Rent")
            {
                // Add columns for the Rent table
                combinedDataTable.Columns.Add("ContractNumber", typeof(string));
                combinedDataTable.Columns.Add("ContractDate", typeof(DateTime));
                combinedDataTable.Columns.Add("StartDate", typeof(DateTime));
                combinedDataTable.Columns.Add("EndDate", typeof(DateTime));
            }
            else if (tableName == "Room")
            {
                combinedDataTable.Columns.Add("RoomName", typeof(string));
                combinedDataTable.Columns.Add("Area", typeof(long));
            }
            else if (tableName == "Renter")
            {
                combinedDataTable.Columns.Add("CompanyName", typeof(string));
                combinedDataTable.Columns.Add("LegalAddress", typeof(string));
                combinedDataTable.Columns.Add("CEOFullName", typeof(string));
                combinedDataTable.Columns.Add("ContactPhone", typeof(string));
            }
            else if (tableName == "Building")
            {
                combinedDataTable.Columns.Add("BuildingName", typeof(string));
                combinedDataTable.Columns.Add("BuildingAddress", typeof(string));
            }
        }

        var joinedRows = from rentRow in filteredRentRows
                         join roomRow in roomTable.AsEnumerable()
                         on rentRow.Field<long>("RoomID") equals roomRow.Field<long>("RoomID")
                         join renterRow in renterTable.AsEnumerable()
                         on rentRow.Field<long>("RenterID") equals renterRow.Field<long>("RenterID")
                         join buildingRow in buildingTable.AsEnumerable()
                         on roomRow.Field<long>("BuildingID") equals buildingRow.Field<long>("BuildingID")
                         select new
                         {
                             ContractNumber = rentRow.Field<string>("ContractNumber"),
                             ContractDate = DateTime.Parse(rentRow.Field<string>("ContractDate")),
                             StartDate = DateTime.Parse(rentRow.Field<string>("StartDate")),
                             EndDate = DateTime.Parse(rentRow.Field<string>("EndDate")),
                             RoomName = roomRow.Field<string>("Name"),
                             Area = roomRow.Field<long>("Area"),
                             CompanyName = renterRow.Field<string>("CompanyName"),
                             LegalAddress = renterRow.Field<string>("LegalAddress"),
                             CEOFullName = renterRow.Field<string>("CEOFullName"),
                             ContactPhone = renterRow.Field<string>("ContactPhone"),
                             BuildingName = buildingRow.Field<string>("Name"),
                             BuildingAddress = buildingRow.Field<string>("Address")
                         };

        foreach (var joinedRow in joinedRows)
        {
            DataRow newRow = combinedDataTable.NewRow();

            if (tableNames.Contains("Rent"))
            {
                newRow["ContractNumber"] = joinedRow.ContractNumber;
                newRow["ContractDate"] = joinedRow.ContractDate;
                newRow["StartDate"] = joinedRow.StartDate;
                newRow["EndDate"] = joinedRow.EndDate;
            }
            if (tableNames.Contains("Room"))
            {
                newRow["RoomName"] = joinedRow.RoomName;
                newRow["Area"] = joinedRow.Area;
            }
            if (tableNames.Contains("Renter"))
            {
                newRow["CompanyName"] = joinedRow.CompanyName;
                newRow["LegalAddress"] = joinedRow.LegalAddress;
                newRow["CEOFullName"] = joinedRow.CEOFullName;
                newRow["ContactPhone"] = joinedRow.ContactPhone;
            }
            if (tableNames.Contains("Building"))
            {
                newRow["BuildingName"] = joinedRow.BuildingName;
                newRow["BuildingAddress"] = joinedRow.BuildingAddress;
            }

            combinedDataTable.Rows.Add(newRow);
        }
    }
}