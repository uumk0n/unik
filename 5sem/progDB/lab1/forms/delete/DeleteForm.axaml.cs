using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;


namespace lab1;

public partial class DeleteForm : Window
{
    private DataSetService dataSetService;
    public DeleteForm()
    {
        InitializeComponent();
        LoadData();
        this.Closing += FormClosing;
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

            var m_buildingsDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
            DataTable buildingTable = dataSetService.dataSet.Tables["Building"];
            List<Building> buildings = dataSetService.DataTableToList<Building>(buildingTable);
            m_buildingsDataGrid.ItemsSource = buildings;

            var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
            DataTable roomTable = dataSetService.dataSet.Tables["Room"];
            List<Room> rooms = dataSetService.DataTableToList<Room>(roomTable);
            m_roomsDataGrid.ItemsSource = rooms;

            var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
            DataTable renterTable = dataSetService.dataSet.Tables["Renter"];
            List<Renter> renters = dataSetService.DataTableToList<Renter>(renterTable);
            m_rentersDataGrid.ItemsSource = renters;

            var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
            DataTable rentTable = dataSetService.dataSet.Tables["Rent"];
            List<Rent> rents = dataSetService.DataTableToList<Rent>(rentTable);
            m_rentsDataGrid.ItemsSource = rents;

            dataSetService.SetRelationShips(buildingTable, roomTable, renterTable);
        }
        catch (Exception e)
        {
            MsgBox msgBox = new MsgBox(e.Message, false);
            msgBox.Show();
        }

    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button deleteButton)
        {

            if (deleteButton.Name == "deleteBuildingButton")
            {
                var m_buildingsDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
                var buildings = m_buildingsDataGrid.ItemsSource.Cast<Building>().ToList();
                foreach (var item in m_buildingsDataGrid.SelectedItems)
                {
                    buildings.Remove((Building)item);
                }
                m_buildingsDataGrid.ItemsSource = buildings;
            }
            else if (deleteButton.Name == "deleteRoomButton")
            {
                var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
                var rooms = m_roomsDataGrid.ItemsSource.Cast<Room>().ToList();
                foreach (var item in m_roomsDataGrid.SelectedItems)
                {
                    rooms.Remove((Room)item);
                }
                m_roomsDataGrid.ItemsSource = rooms;
            }
            else if (deleteButton.Name == "deleteRenterButton")
            {
                var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
                var renters = m_rentersDataGrid.ItemsSource.Cast<Renter>().ToList();
                foreach (var item in m_rentersDataGrid.SelectedItems)
                {
                    renters.Remove((Renter)item);
                }
                m_rentersDataGrid.ItemsSource = renters;
            }
            else if (deleteButton.Name == "deleteRentButton")
            {
                var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
                var rents = m_rentsDataGrid.ItemsSource.Cast<Rent>().ToList();
                foreach (var item in m_rentsDataGrid.SelectedItems)
                {
                    rents.Remove((Rent)item);
                }
                m_rentsDataGrid.ItemsSource = rents;
            }
        }
    }

    private void FormClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        DataSetService dataSetService = new DataSetService();

        var m_buildingsDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
        var buildings = m_buildingsDataGrid.ItemsSource.Cast<Building>().ToList();

        var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
        var rooms = m_roomsDataGrid.ItemsSource.Cast<Room>().ToList();

        var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
        var renters = m_rentersDataGrid.ItemsSource.Cast<Renter>().ToList();

        var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
        var rents = m_rentsDataGrid.ItemsSource.Cast<Rent>().ToList();

        DataSet dataSet = new DataSet();
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Building>(buildings));
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Room>(rooms));
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Renter>(renters));
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Rent>(rents));

        // Сериализация в JSON
        string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);

        // Сохранение JSON данных в файл
        string filePath = "accounting_for_leased_premises.json";
        File.WriteAllText(filePath, json);
    }
}