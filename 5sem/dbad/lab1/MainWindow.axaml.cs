using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;

namespace lab1;

public partial class MainWindow : Window
{
    private DataSetService dataSetService;
    private bool isCascade;
    Dictionary<string, string> columnMapping;
    public MainWindow()
    {
        InitializeComponent();
        this.Opened += MainWindow_Opened;
    }

    private async void MainWindow_Opened(object sender, EventArgs e)
    {
        LoadData(true);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void LoadData(bool isInit)
    {
        string jsonPath = "accounting_for_leased_premises.json";
        columnMapping = ReadJsonFile("ruColumns.json");

        if (!File.Exists(jsonPath))
        {
            var msgBox = new MsgBox("Вопрос");
            msgBox.ShowDialog(this);
            await msgBox.WaitForCloseAsync();
            return;
        }
        if (isInit)
        {
            var cascadeMsgBox = new MsgBox("Вопрос", "Использовать каскадное удаление?", true, true);
            cascadeMsgBox.ShowDialog(this);
            await cascadeMsgBox.WaitForCloseAsync();
            isCascade = cascadeMsgBox.getDeleteMode();
        }
        try
        {
            dataSetService = new DataSetService();
            string json = File.ReadAllText(jsonPath);
            DataSetContainer dataSetContainer = JsonConvert.DeserializeObject<DataSetContainer>(json);

            DataSet dataSetFromFile = new DataSet("accounting_for_leased_premises");

            dataSetService.FillTableInfoFromDataSet(dataSetFromFile, dataSetContainer);
            dataSetService.FillDataSetFromDataSetContainer(dataSetFromFile, dataSetContainer);

            dataSetService.dataSet = dataSetFromFile;

            // Mapping between DataGrid and DataTable
            var gridTableMap = new Dictionary<DataGrid, DataTable>
        {
            { this.FindControl<DataGrid>("buildingDataGrid"), dataSetService.dataSet.Tables["Building"] },
            { this.FindControl<DataGrid>("roomDataGrid"), dataSetService.dataSet.Tables["Room"] },
            { this.FindControl<DataGrid>("renterDataGrid"), dataSetService.dataSet.Tables["Renter"] },
            { this.FindControl<DataGrid>("rentDataGrid"), dataSetService.dataSet.Tables["Rent"] }
        };

            // Load data for each DataGrid
            foreach (var entry in gridTableMap)
            {
                DataGrid dataGrid = entry.Key;
                DataTable dataTable = entry.Value;
                string typeName = "lab1." + dataTable.TableName;

                if (dataTable != null)
                {
                    Type dataType = Type.GetType(typeName);
                    dataGrid.ItemsSource = dataSetService.DataTableToList(dataTable, dataType);
                    dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
                }
            }

            dataSetService.dataSet.Tables["Building"].PrimaryKey = new DataColumn[] { dataSetService.dataSet.Tables["Building"].Columns["BuildingID"] };
            dataSetService.dataSet.Tables["Room"].PrimaryKey = new DataColumn[] { dataSetService.dataSet.Tables["Room"].Columns["RoomID"] };
            dataSetService.dataSet.Tables["Renter"].PrimaryKey = new DataColumn[] { dataSetService.dataSet.Tables["Renter"].Columns["RenterID"] };
            dataSetService.dataSet.Tables["Rent"].PrimaryKey = new DataColumn[] { dataSetService.dataSet.Tables["Rent"].Columns["RentID"] };

            DataTable buildingTable = dataSetService.dataSet.Tables["Building"];
            DataTable roomTable = dataSetService.dataSet.Tables["Room"];
            DataTable renterTable = dataSetService.dataSet.Tables["Renter"];
            DataTable rentTable = dataSetService.dataSet.Tables["Rent"];
            dataSetService.SetRelationShips(buildingTable, roomTable, renterTable, rentTable);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (sender is DataGrid dataGrid)
        {
            if (e.PropertyName.EndsWith("ID"))
            {
                e.Cancel = true; // Отменяем создание столбца
            }

            // Заменить английское имя столбца на русское
            if (columnMapping.ContainsKey(e.PropertyName.ToLower()))
            {
                e.Column.Header = columnMapping[e.PropertyName.ToLower()];
            }
        }
    }

    static Dictionary<string, string> ReadJsonFile(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button editButton)
        {
            string windowTitle = "";
            DataGrid dataGrid = null;

            if (editButton.Name == "BuildingEditButton")
            {
                dataGrid = this.FindControl<DataGrid>("buildingDataGrid");
                windowTitle = "Редактирование здания";
            }
            else if (editButton.Name == "RoomEditButton")
            {
                dataGrid = this.FindControl<DataGrid>("roomDataGrid");
                windowTitle = "Редактирование комнаты";
            }
            else if (editButton.Name == "RenterEditButton")
            {
                dataGrid = this.FindControl<DataGrid>("renterDataGrid");
                windowTitle = "Редактирование арендатора";
            }
            else if (editButton.Name == "RentEditButton")
            {
                dataGrid = this.FindControl<DataGrid>("rentDataGrid");
                windowTitle = "Редактирование аренды";
            }

            var selectedItems = dataGrid.SelectedItems.Cast<object>().ToList();
            if (selectedItems.Count() != 0)
            {
                selectedIndex = dataGrid.SelectedIndex;
                Type type = selectedItems[0].GetType();
                var actionForm = new ActionsForm(windowTitle, selectedItems, dataSetService.dataSet, type, true);
                actionForm.Closed += (sender, e) => ActionFormClosed(dataGrid);
                actionForm.ShowDialog(this);
            }
        }
    }
    private int selectedIndex = -1;
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button addButton)
        {
            string windowTitle = "";
            DataGrid dataGrid = null;
            Type type = null;

            if (addButton.Name == "BuildingAddButton")
            {
                dataGrid = this.FindControl<DataGrid>("buildingDataGrid");
                type = typeof(Building);
                windowTitle = "Добавить здание";
            }
            else if (addButton.Name == "RoomAddButton")
            {
                dataGrid = this.FindControl<DataGrid>("roomDataGrid");
                type = typeof(Room);
                windowTitle = "Добавить комнату";
            }
            else if (addButton.Name == "RenterAddButton")
            {
                dataGrid = this.FindControl<DataGrid>("renterDataGrid");
                type = typeof(Renter);
                windowTitle = "Добавить арендатора";
            }
            else if (addButton.Name == "RentAddButton")
            {
                dataGrid = this.FindControl<DataGrid>("rentDataGrid");
                type = typeof(Rent);
                windowTitle = "Добавить аренду";
            }


            if (dataGrid != null)
            {
                selectedIndex = dataGrid.ItemsSource.Cast<object>().Count();
                var actionForm = new ActionsForm(windowTitle, null, dataSetService.dataSet, type, false, true);
                actionForm.Closed += (sender, e) => ActionFormClosed(dataGrid);
                actionForm.ShowDialog(this);
            }
        }
    }

    private void ActionFormClosed(DataGrid dataGrid)
    {
        dataGrid.ItemsSource = null;
        LoadData(false);
        dataGrid.SelectedIndex = selectedIndex;
        selectedIndex = -1;
    }
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var confirmationDialog = new ConfirmationDialog("Вы действительно хотите удалить запись?");
        confirmationDialog.ShowDialog(this);
        await confirmationDialog.WaitForCloseAsync();
        bool DeleteRecord = confirmationDialog.getResult();
        if (DeleteRecord)
        {
            try
            {
                if (isCascade)
                {
                    if (sender is Button deleteButton)
                    {
                        cascadeDeletion(deleteButton);
                    }
                }
                else
                {
                    if (sender is Button deleteButton)
                    {
                        noneCascadeDeletion(deleteButton);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private void cascadeDeletion(Button deleteButton)
    {
        DataRow rowToDel = null;
        DataTable table = null;
        DataGrid dataGrid = null;

        if (deleteButton.Name == "BuildingDeleteButton")
        {
            if (this.FindControl<DataGrid>("buildingDataGrid").SelectedIndex != -1)
            {
                rowToDel = dataSetService.dataSet.Tables["Building"].Rows[this.FindControl<DataGrid>("buildingDataGrid").SelectedIndex];
                table = dataSetService.dataSet.Tables["Building"];
                dataGrid = this.FindControl<DataGrid>("buildingDataGrid");
            }
        }
        else if (deleteButton.Name == "RoomDeleteButton")
        {
            if (this.FindControl<DataGrid>("roomDataGrid").SelectedIndex != -1)
            {
                rowToDel = dataSetService.dataSet.Tables["Room"].Rows[this.FindControl<DataGrid>("roomDataGrid").SelectedIndex];
                table = dataSetService.dataSet.Tables["Room"];
                dataGrid = this.FindControl<DataGrid>("roomDataGrid");
            }
        }
        else if (deleteButton.Name == "RenterDeleteButton")
        {
            if (this.FindControl<DataGrid>("renterDataGrid").SelectedIndex != -1)
            {
                rowToDel = dataSetService.dataSet.Tables["Renter"].Rows[this.FindControl<DataGrid>("renterDataGrid").SelectedIndex];
                table = dataSetService.dataSet.Tables["Renter"];
                dataGrid = this.FindControl<DataGrid>("renterDataGrid");
            }
        }
        else if (deleteButton.Name == "RentDeleteButton")
        {
            if (this.FindControl<DataGrid>("rentDataGrid").SelectedIndex != -1)
            {
                rowToDel = dataSetService.dataSet.Tables["Rent"].Rows[this.FindControl<DataGrid>("rentDataGrid").SelectedIndex];
                table = dataSetService.dataSet.Tables["Rent"];
                dataGrid = this.FindControl<DataGrid>("rentDataGrid");
            }
        }


        if (rowToDel != null && table != null)
        {
            CascadeDelete(table, rowToDel);
        }
        dataGrid.ItemsSource = null;
        LoadData(false);
    }

    private void noneCascadeDeletion(Button deleteButton)
    {
        DataGrid dataGrid = null;
        DataSet newDataSet = new DataSet("accounting_for_leased_premises");

        if (deleteButton.Name == "BuildingDeleteButton")
        {
            if (this.FindControl<DataGrid>("buildingDataGrid").SelectedIndex != -1)
            {
                dataGrid = this.FindControl<DataGrid>("buildingDataGrid");
                List<Building> list = dataGrid.ItemsSource.Cast<Building>().ToList();
                foreach (Building item in dataGrid.SelectedItems)
                {
                    list.Remove(item);
                }
                dataGrid.ItemsSource = list;
            }
        }
        else if (deleteButton.Name == "RoomDeleteButton")
        {
            if (this.FindControl<DataGrid>("roomDataGrid").SelectedIndex != -1)
            {
                dataGrid = this.FindControl<DataGrid>("roomDataGrid");
                List<Room> list = dataGrid.ItemsSource.Cast<Room>().ToList();
                foreach (Room item in dataGrid.SelectedItems)
                {
                    list.Remove(item);
                }
                dataGrid.ItemsSource = list;
            }
        }
        else if (deleteButton.Name == "RenterDeleteButton")
        {
            if (this.FindControl<DataGrid>("renterDataGrid").SelectedIndex != -1)
            {
                dataGrid = this.FindControl<DataGrid>("renterDataGrid");
                List<Renter> list = dataGrid.ItemsSource.Cast<Renter>().ToList();
                foreach (Renter item in dataGrid.SelectedItems)
                {
                    list.Remove(item);
                }
                dataGrid.ItemsSource = list;
            }
        }
        else if (deleteButton.Name == "RentDeleteButton")
        {
            if (this.FindControl<DataGrid>("rentDataGrid").SelectedIndex != -1)
            {
                dataGrid = this.FindControl<DataGrid>("rentDataGrid");
                List<Rent> list = dataGrid.ItemsSource.Cast<Rent>().ToList();
                foreach (Rent item in dataGrid.SelectedItems)
                {
                    list.Remove(item);
                }
                dataGrid.ItemsSource = list;
            }
        }
        if (dataGrid != null)
        {
            newDataSet.Tables.Add(dataSetService.ConvertListToDataTable(this.FindControl<DataGrid>("buildingDataGrid").ItemsSource.Cast<Building>().ToList()));
            newDataSet.Tables.Add(dataSetService.ConvertListToDataTable(this.FindControl<DataGrid>("roomDataGrid").ItemsSource.Cast<Room>().ToList()));
            newDataSet.Tables.Add(dataSetService.ConvertListToDataTable(this.FindControl<DataGrid>("renterDataGrid").ItemsSource.Cast<Renter>().ToList()));
            newDataSet.Tables.Add(dataSetService.ConvertListToDataTable(this.FindControl<DataGrid>("rentDataGrid").ItemsSource.Cast<Rent>().ToList()));


            string jsonPath = "accounting_for_leased_premises.json";
            string jsonToRead = File.ReadAllText(jsonPath);
            DataSetContainer dataSetContainer = JsonConvert.DeserializeObject<DataSetContainer>(jsonToRead);

            var dataToSerialize = new
            {
                DataSet = newDataSet,
                TableInfo = dataSetContainer.TableInfo
            };

            string json = JsonConvert.SerializeObject(dataToSerialize, Formatting.Indented);
            string jsonFilePath = "accounting_for_leased_premises.json";
            File.WriteAllText(jsonFilePath, json);

            dataGrid.ItemsSource = null;
            LoadData(false);
        }
    }

    public void CascadeDelete(DataTable table, DataRow row)
    {
        // Check if the table has any related child tables
        if (dataSetService.dataSet.Relations.Count > 0)
        {
            // Iterate through the child tables
            foreach (DataRelation relation in dataSetService.dataSet.Relations)
            {
                if (relation.ParentTable == table)
                {
                    string childTableName = relation.ChildTable.TableName;
                    string parentColumnName = relation.ParentColumns[0].ColumnName;
                    string childColumnName = relation.ChildColumns[0].ColumnName;

                    // Find related child rows
                    DataRow[] childRows = GetChildRows(row, relation);

                    // Recursively delete child rows
                    foreach (DataRow childRow in childRows)
                    {
                        CascadeDelete(dataSetService.dataSet.Tables[childTableName], childRow);
                    }
                }
            }
        }

        // Delete the current row
        dataSetService.dataSet.Tables[table.TableName].Rows.Remove(row);
        dataSetService.SaveDataSet();
    }

    public DataRow[] GetChildRows(DataRow parentRow, DataRelation relation)
    {
        DataTable childTable = relation.ChildTable;
        DataColumn parentColumn = relation.ParentColumns[0]; // Assuming single-column relation
        int index = GetColumnIndex(childTable.Columns, parentColumn.ToString());
        List<DataRow> childRows = new List<DataRow>();

        foreach (DataRow childRow in childTable.Rows)
        {
            // Check if the foreign key matches the parent row's primary key
            if (childRow[index].ToString() == parentRow[parentColumn].ToString())
            {
                childRows.Add(childRow);
            }
        }

        return childRows.ToArray();
    }

    private int GetColumnIndex(DataColumnCollection colums, string columnName)
    {
        for (int i = 0; i < colums.Count; i++)
        {
            if (colums[i].ColumnName == columnName)
            {
                return i;
            }
        }
        return -1;
    }
    private void CreateDocsButton_Click(object sender, RoutedEventArgs e)
    {
        var createDocs = new CreateDocs();
        createDocs.ShowDialog(this);
    }
}