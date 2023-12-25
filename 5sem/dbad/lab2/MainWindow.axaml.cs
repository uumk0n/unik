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
using lab1;
using Newtonsoft.Json;

namespace lab2;

public partial class MainWindow : Window
{
    private bool isCascade;
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

        columnMapping = ReadJsonFile("ruColumns.json");
        DataHandler dataHandler = new DataHandler("accounting_for_leased_premises.db");
        dataHandler.CreateDatabase();
        if (dataHandler.IsDatabaseEmpty())
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

        List<Building> buildingRecords = dataHandler.GetRecordsFromTable("Building", reader =>
        {
            return new Building
            {
                BuildingID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Address = reader.GetString(2)
            };
        });

        List<Room> roomRecords = dataHandler.GetRecordsFromTable("Room", reader =>
        {
            return new Room
            {
                RoomID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Area = reader.GetInt32(2),
                BuildingID = reader.GetInt32(3)
            };
        });

        List<Renter> renterRecords = dataHandler.GetRecordsFromTable("Renter", reader =>
        {
            return new Renter
            {
                RenterID = reader.GetInt32(0),
                CompanyName = reader.GetString(1),
                LegalAddress = reader.GetString(2),
                CEOFullName = reader.GetString(3),
                ContactPhone = reader.GetString(4)
            };
        });

        List<Rent> rentRecords = dataHandler.GetRecordsFromTable("Rent", reader =>
        {
            return new Rent
            {
                RentID = reader.GetInt32(0),
                RoomID = reader.GetInt32(1),
                RenterID = reader.GetInt32(2),
                ContractNumber = reader.GetString(3),
                ContractDate = reader.GetDateTime(4),
                StartDate = reader.GetDateTime(5),
                EndDate = reader.GetDateTime(6)
            };
        });

        var buildingDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
        var roomDataGrid = this.FindControl<DataGrid>("roomDataGrid");
        var renterDataGrid = this.FindControl<DataGrid>("renterDataGrid");
        var rentDataGrid = this.FindControl<DataGrid>("rentDataGrid");

        buildingDataGrid.ItemsSource = buildingRecords;
        roomDataGrid.ItemsSource = roomRecords;
        renterDataGrid.ItemsSource = renterRecords;
        rentDataGrid.ItemsSource = rentRecords;

        foreach (var dataGrid in new[] { buildingDataGrid, roomDataGrid, renterDataGrid, rentDataGrid })
        {
            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
        }
    }

    static Dictionary<string, string> ReadJsonFile(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
    }

    Dictionary<string, string> columnMapping;
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
            List<object> selectedItems = null;
            if (dataGrid.SelectedItems != null)
                selectedItems = dataGrid.SelectedItems.Cast<object>().ToList();
            if (selectedItems.Count() != 0)
            {
                Type type = selectedItems[0].GetType();
                selectedIndex = dataGrid.SelectedIndex;
                var actionForm = new ActionsForm(windowTitle, selectedItems, type, true);
                actionForm.Closed += (sender, e) => ActionFormClosed(dataGrid);
                actionForm.ShowDialog(this);
            }
        }
    }

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

            List<object> selectedItems = null;
            if (dataGrid.SelectedItems != null)
                selectedItems = dataGrid.SelectedItems.Cast<object>().ToList();
            if (selectedItems.Count() != 0)
            {
                selectedIndex = dataGrid.ItemsSource.Cast<object>().Count();
                var actionForm = new ActionsForm(windowTitle, null, type, false, true);
                actionForm.Closed += (sender, e) => ActionFormClosed(dataGrid);
                actionForm.ShowDialog(this);
            }
        }
    }
    private int selectedIndex = -1;
    private void ActionFormClosed(DataGrid dataGrid)
    {
        dataGrid.ItemsSource = null;
        LoadData(false);
        dataGrid.SelectedIndex = selectedIndex;
        selectedIndex = -1;
    }
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button deleteButton)
        {

            String tableName = null;
            DataGrid dataGrid = null;
            int recordID = -1;

            if (deleteButton.Name == "BuildingDeleteButton")
            {
                if (this.FindControl<DataGrid>("buildingDataGrid").SelectedIndex != -1)
                {
                    tableName = "Building";
                    dataGrid = this.FindControl<DataGrid>("buildingDataGrid");
                    recordID = dataGrid.ItemsSource.Cast<Building>().ToList()
                    [dataGrid.SelectedIndex].BuildingID;
                }
            }
            else if (deleteButton.Name == "RoomDeleteButton")
            {
                if (this.FindControl<DataGrid>("roomDataGrid").SelectedIndex != -1)
                {
                    tableName = "Room";
                    dataGrid = this.FindControl<DataGrid>("roomDataGrid");
                    recordID = dataGrid.ItemsSource.Cast<Room>().ToList()
                    [dataGrid.SelectedIndex].RoomID;
                }
            }
            else if (deleteButton.Name == "RenterDeleteButton")
            {
                if (this.FindControl<DataGrid>("renterDataGrid").SelectedIndex != -1)
                {
                    tableName = "Renter";
                    dataGrid = this.FindControl<DataGrid>("renterDataGrid");
                    recordID = dataGrid.ItemsSource.Cast<Renter>().ToList()
                    [dataGrid.SelectedIndex].RenterID;
                }
            }
            else if (deleteButton.Name == "RentDeleteButton")
            {
                if (this.FindControl<DataGrid>("rentDataGrid").SelectedIndex != -1)
                {
                    tableName = "Rent";
                    dataGrid = this.FindControl<DataGrid>("rentDataGrid");
                    recordID = dataGrid.ItemsSource.Cast<Rent>().ToList()
                    [dataGrid.SelectedIndex].RentID;
                }
            }

            List<object> selectedItems = null;
            if (dataGrid != null)
                if (dataGrid.SelectedItems != null)
                    selectedItems = dataGrid.SelectedItems.Cast<object>().ToList();
            if (selectedItems != null && selectedItems.Count() != 0)
            {
                var confirmationDialog = new ConfirmationDialog("Вы действительно хотите удалить запись?");
                confirmationDialog.ShowDialog(this);
                await confirmationDialog.WaitForCloseAsync();
                bool DeleteRecord = confirmationDialog.getResult();

                if (DeleteRecord)
                {
                    if (isCascade && tableName != null && recordID != -1)
                    {
                        DataHandler dataHandler = new DataHandler("accounting_for_leased_premises.db");
                        dataHandler.DeleteCascadeRecord(tableName, recordID);
                    }
                    else
                    {
                        DataHandler dataHandler = new DataHandler("accounting_for_leased_premises.db");
                        dataHandler.DeleteRecord(tableName, recordID);
                    }
                    if (dataGrid != null)
                        dataGrid.ItemsSource = null;
                    LoadData(false);
                }
            }

        }
    }

    private void CreateDocsButton_Click(object sender, RoutedEventArgs e)
    {
        var createDocs = new CreateDocs();
        createDocs.ShowDialog(this);
    }
}