using System.Collections.ObjectModel;
using System.Data;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;


namespace lab1;

public partial class ViewForm : Window
{
    public ViewForm()
    {
        InitializeComponent();

        DataSetService dataSetService = new DataSetService();

        string json = System.IO.File.ReadAllText("accounting_for_leased_premises.json");

        var data = JsonConvert.DeserializeObject<DataModel>(json);

        DataTable tableBuildings = dataSetService.convertToDataTable(data.Buildings);

        DataGrid? dataGrid = this.FindControl<DataGrid>("Buildings");
        var buildings = dataGrid;
        if(tableBuildings!=null)
        {
            buildings.ItemsSource = tableBuildings.Rows;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}