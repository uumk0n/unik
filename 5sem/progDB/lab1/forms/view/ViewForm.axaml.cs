using System;
using System.IO;
using System.Data;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace lab1;
public partial class ViewForm : Window
{


    public ViewForm()
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
        DataSetService dataSetService = new DataSetService();
        try
        {
            var json = File.ReadAllText(path);

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(json);

            var m_buildingsDataGrid = this.FindControl<DataGrid>("buildingDataGrid");
            DataTable buildingTable = dataSet.Tables["Building"];
            List<Building> buildings = dataSetService.DataTableToList<Building>(buildingTable);
            m_buildingsDataGrid.ItemsSource = buildings;

            var m_roomsDataGrid = this.FindControl<DataGrid>("roomDataGrid");
            DataTable roomTable = dataSet.Tables["Room"];
            List<Room> rooms = dataSetService.DataTableToList<Room>(roomTable);
            m_roomsDataGrid.ItemsSource = rooms;

            var m_rentersDataGrid = this.FindControl<DataGrid>("renterDataGrid");
            DataTable renterTable = dataSet.Tables["Renter"];
            List<Renter> renters = dataSetService.DataTableToList<Renter>(renterTable);
            m_rentersDataGrid.ItemsSource = renters;

            var m_rentsDataGrid = this.FindControl<DataGrid>("rentDataGrid");
            DataTable rentTable = dataSet.Tables["Rent"];
            List<Rent> rents = dataSetService.DataTableToList<Rent>(rentTable);
            m_rentsDataGrid.ItemsSource = rents;

        }
        catch (Exception e)
        {
            MsgBox msgBox = new MsgBox(e.Message, false);
            msgBox.Show();
        }

    }

}
