using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;


namespace lab1;

public partial class AddForm : Window
{
    private DataSetService dataSetService;
    public AddForm()
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
            List<Building> buildings = new List<Building>();
            if (buildingTable != null)
                buildings = dataSetService.DataTableToList<Building>(buildingTable);
            else
                buildings.Add(new Building());

            m_buildingsDataGrid.ItemsSource = buildings;

            var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
            DataTable roomTable = dataSetService.dataSet.Tables["Room"];
            List<Room> rooms = new List<Room>();
            if (roomTable != null)
                rooms = dataSetService.DataTableToList<Room>(roomTable);
            else
                rooms.Add(new Room());

            m_roomsDataGrid.ItemsSource = rooms;

            var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
            DataTable renterTable = dataSetService.dataSet.Tables["Renter"];
            List<Renter> renters = new List<Renter>();
            if (renterTable != null)
                renters = dataSetService.DataTableToList<Renter>(renterTable);
            else
                renters.Add(new Renter());

            m_rentersDataGrid.ItemsSource = renters;

            var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
            DataTable rentTable = dataSetService.dataSet.Tables["Rent"];
            List<Rent> rents = new List<Rent>();
            if (rentTable != null)
                rents = dataSetService.DataTableToList<Rent>(rentTable);
            else
                rents.Add(new Rent());
            m_rentsDataGrid.ItemsSource = rents;

            dataSetService.SetRelationShips(buildingTable, roomTable, renterTable);
        }
        catch (Exception e)
        {
            MsgBox msgBox = new MsgBox(e.Message, false);
            msgBox.Show();
        }

    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button deleteButton)
        {
            if (deleteButton.Name == "addBuildingButton")
            {
                var m_buildingsDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
                List<Building> buildings = new List<Building>();
                if (m_buildingsDataGrid.ItemsSource.Cast<Building>().ToList() != null)
                    buildings = m_buildingsDataGrid.ItemsSource.Cast<Building>().ToList();

                buildings.Add(new Building());

                m_buildingsDataGrid.ItemsSource = buildings;
            }
            else if (deleteButton.Name == "addRoomButton")
            {
                var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
                List<Room> rooms = new List<Room>();
                if (m_roomsDataGrid.ItemsSource != null)
                    rooms = m_roomsDataGrid.ItemsSource.Cast<Room>().ToList();

                rooms.Add(new Room());

                m_roomsDataGrid.ItemsSource = rooms;
            }
            else if (deleteButton.Name == "addRenterButton")
            {
                var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
                List<Renter> renters = new List<Renter>();
                if (m_rentersDataGrid.ItemsSource != null)
                    renters = m_rentersDataGrid.ItemsSource.Cast<Renter>().ToList();

                renters.Add(new Renter());

                m_rentersDataGrid.ItemsSource = renters;
            }
            else if (deleteButton.Name == "addRentButton")
            {
                var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
                List<Rent> rents = new List<Rent>();
                if (m_rentsDataGrid.ItemsSource != null)
                    rents = m_rentsDataGrid.ItemsSource.Cast<Rent>().ToList();

                rents.Add(new Rent());

                m_rentsDataGrid.ItemsSource = rents;
            }
        }
    }

    private void FormClosing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        DataSetService dataSetService = new DataSetService();

        var m_buildingsDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
        var buildings = m_buildingsDataGrid.ItemsSource.Cast<Building>().Where(b => !IsObjectEmpty(b)).ToList();

        var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
        var rooms = m_roomsDataGrid.ItemsSource.Cast<Room>().Where(r => !IsObjectEmpty(r)).ToList();

        var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
        var renters = m_rentersDataGrid.ItemsSource.Cast<Renter>().Where(r => !IsObjectEmpty(r)).ToList();

        var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
        var rents = m_rentsDataGrid.ItemsSource.Cast<Rent>().Where(r => !IsObjectEmpty(r)).ToList();

        DataSet dataSet = new DataSet();
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Building>(buildings));
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Room>(rooms));
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Renter>(renters));
        dataSet.Tables.Add(dataSetService.ConvertListToDataTable<Rent>(rents));

        string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
        string filePath = "accounting_for_leased_premises.json";
        File.WriteAllText(filePath, json);
    }


    private bool IsObjectEmpty(object obj)
    {
        if (obj == null)
        {
            return true;
        }

        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);

            if (value == null || (value is int intValue && intValue == 0))
            {
                return true;
            }
        }

        return false;
    }

}