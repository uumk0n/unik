using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;


namespace lab1;

public partial class ActionsForm : Window
{
    DataSetService dataSetService;
    bool isEditForm;
    bool isAddForm;
    Type typeData;
    int ID = -1;
    public ActionsForm(string title, List<object> rows, DataSet dataSet, Type type, bool isEdit = false, bool isAdd = false)
    {
        isEditForm = isEdit;
        isAddForm = isAdd;
        typeData = type;
        dataSetService = new DataSetService
        {
            dataSet = dataSet
        };
        InitializeComponent();
        InitControls(title, rows, isEdit, isAdd);
        var myButton = this.FindControl<Button>("saveBtn");
        myButton.Click += Button_Click;

    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    static Dictionary<string, string> ReadJsonFile(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
    }
    private void InitControls(string title, List<object> rows, bool isEdit = false, bool isAdd = false)
    {
        Title = title;
        Dictionary<string, string> columnMapping = ReadJsonFile("ruColumns.json");
        double totalHeight = 0;

        if (isEdit || isAdd)
        {
            var stackPanel = this.FindControl<StackPanel>("stackPanel");
            var properties = typeData.GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == "BuildingID" || property.Name == "RoomID" || property.Name == "RenterID" || property.Name == "RentID")
                {
                    if (isEdit)
                        if (ID == -1)
                            ID = (int)property.GetValue(rows[0]);
                    continue;
                }
                if (property.Name.EndsWith("ID"))
                {
                    continue;
                }

                var label = new TextBlock();
                label.Text = columnMapping[property.Name.ToLower()];
                label.Margin = new Thickness(5);
                double labelHeight = label.Height;

                totalHeight += labelHeight;

                if (property.PropertyType == typeof(int))
                {
                    var textBox = new TextBox();
                    textBox.Margin = new Thickness(5);
                    textBox.Width = 200;
                    double textBoxHeight = textBox.Height;
                    totalHeight += textBoxHeight;

                    if (isEdit)
                    {
                        textBox.Text = property.GetValue(rows[0])?.ToString();
                    }
                    else if (isAdd)
                    {
                        textBox.Text = " ";
                    }

                    textBox.LostFocus += LongTextBoxLostFocus;

                    stackPanel.Children.Add(label);
                    stackPanel.Children.Add(textBox);
                }
                else if (property.PropertyType == typeof(string))
                {
                    if (property.Name == "BuildingName" || property.Name == "RoomName" || property.Name == "RenterName")
                    {
                        var comboBox = new ComboBox();
                        comboBox.Margin = new Thickness(5);
                        comboBox.Width = 200;

                        //для таблицы rent
                        var comboBox1 = new ComboBox();
                        comboBox1.Margin = new Thickness(5);
                        comboBox1.Width = 200;

                        switch (typeData.Name)
                        {
                            case "Room":
                                comboBox.ItemsSource = dataSetService.dataSet.Tables["Building"].AsEnumerable().Select(row => row.Field<string>("Name")).ToList();
                                break;
                            case "Rent":
                                comboBox.ItemsSource = dataSetService.dataSet.Tables["Renter"].AsEnumerable().Select(row => row.Field<string>("CompanyName")).ToList();
                                comboBox1.ItemsSource = dataSetService.dataSet.Tables["Room"].AsEnumerable().Select(row => row.Field<string>("Name")).ToList();
                                break;
                        }
                        stackPanel.Children.Add(label);
                        stackPanel.Children.Add(comboBox);
                        if (comboBox1.ItemCount > 0)
                            stackPanel.Children.Add(comboBox1);
                    }
                    else
                    {
                        var textBox = new TextBox();
                        textBox.Margin = new Thickness(5);
                        textBox.Width = 200;
                        double textBoxHeight = textBox.Height;
                        totalHeight += textBoxHeight;

                        if (isEdit)
                        {
                            textBox.Text = property.GetValue(rows[0])?.ToString();
                        }
                        else if (isAdd)
                        {
                            textBox.Text = " ";
                        }

                        textBox.LostFocus += StringTextBoxLostFocus;

                        stackPanel.Children.Add(label);
                        stackPanel.Children.Add(textBox);
                    }
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    var datePicker = new DatePicker();
                    datePicker.Margin = new Thickness(5);
                    datePicker.Width = 300;
                    double datePickerHeight = datePicker.Height;
                    totalHeight += datePickerHeight;

                    if (isEdit)
                    {
                        object dateValue = property.GetValue(rows[0]);
                        if (dateValue is DateTime validDate && validDate.Year >= 0 && validDate.Year <= 10000)
                        {
                            datePicker.SelectedDate = validDate;
                        }
                    }
                    else if (isAdd)
                    {
                        datePicker.SelectedDate = DateTime.Now;
                    }

                    stackPanel.Children.Add(label);
                    stackPanel.Children.Add(datePicker);
                }
            }
        }
        this.Height = totalHeight + 20;
    }

    private bool checkControls(StackPanel stackPanel)
    {
        var children = FindLogicalChildren<Control>(stackPanel);

        // Проверяем, являются ли дети TextBox и не пустыми
        foreach (var child in children)
        {
            if (child is TextBox textBox)
            {
                // Проверяем, не пусто ли текстовое поле
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    return false;
                }
            }
            else if (child is ComboBox comboBox)
            {
                // Проверяем, выбран ли элемент в ComboBox
                if (comboBox.SelectedItem == null)
                {
                    return false;
                }
            }
            else if (child is DatePicker datePicker)
            {
                // Проверяем, установлена ли дата в DatePicker
                if (datePicker.SelectedDate == null)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void LongTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (!long.TryParse(textBox.Text, out _))
            {
                textBox.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
    }

    private void StringTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.BorderBrush = Brushes.Red;
            }
            else
            {
                textBox.ClearValue(TextBox.BorderBrushProperty);
            }
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

        try
        {
            var stackPanel = this.FindControl<StackPanel>("stackPanel");
            if (!checkControls(stackPanel))
                return;
            var properties = typeData.GetProperties();
            DataRow newRow = null;

            int textBoxIndex = 0;
            int datePickerIndex = 0;
            int comboBoxIndex = 0;

            string parentName = "";

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                if (property.Name == "BuildingName" || property.Name == "RoomName" || property.Name == "RenterName")
                {
                    continue;
                }

                if (property.Name.EndsWith("ID") && isEditForm)
                {
                    if (newRow == null)
                    {
                        newRow = dataSetService.dataSet.Tables[typeData.Name].NewRow();
                    }
                    newRow[property.Name] = ID;
                    continue;
                }
                else if (property.Name.EndsWith("ID") && isAddForm)
                {
                    if (newRow == null)
                    {
                        newRow = dataSetService.dataSet.Tables[typeData.Name].NewRow();
                    }
                    switch (typeData.Name)
                    {
                        case "Building":
                            if (dataSetService.dataSet.Tables[typeData.Name].Rows.Count > 0)
                                newRow[property.Name] = GetMaxId(dataSetService.dataSet.Tables[typeData.Name]) + 1;
                            else
                                newRow[property.Name] = 1;
                            continue;

                        case "Room":
                            if (property.Name == "RoomID")
                            {
                                if (dataSetService.dataSet.Tables[typeData.Name].Rows.Count > 0)
                                    newRow[property.Name] = GetMaxId(dataSetService.dataSet.Tables[typeData.Name]) + 1;
                                else
                                    newRow[property.Name] = 1;
                                continue;
                            }
                            break;
                        case "Renter":
                            if (dataSetService.dataSet.Tables[typeData.Name].Rows.Count > 0)
                                newRow[property.Name] = GetMaxId(dataSetService.dataSet.Tables[typeData.Name]) + 1;
                            else
                                newRow[property.Name] = 1;
                            continue;
                        case "Rent":
                            if (property.Name == "RentID")
                            {
                                if (dataSetService.dataSet.Tables[typeData.Name].Rows.Count > 0)
                                    newRow[property.Name] = GetMaxId(dataSetService.dataSet.Tables[typeData.Name]) + 1;
                                else
                                    newRow[property.Name] = 1;
                                continue;
                            }
                            break;
                    }
                }

                // Используйте FindLogicalChildren для поиска TextBox в StackPanel
                var textBoxes = FindLogicalChildren<TextBox>(stackPanel);
                var datePickers = FindLogicalChildren<DatePicker>(stackPanel);
                var comboBoxes = FindLogicalChildren<ComboBox>(stackPanel);

                // Используйте Skip(i) для пропуска TextBox, соответствующих свойствам, уже обработанным выше
                var textBox = textBoxes.Skip(textBoxIndex).FirstOrDefault();
                var datePicker = datePickers.Skip(datePickerIndex).FirstOrDefault();
                var comboBox = comboBoxes.Skip(comboBoxIndex).FirstOrDefault();

                if (comboBox != null)
                {
                    if (property.Name.EndsWith("ID"))
                    {
                        switch (typeData.Name)
                        {
                            case "Room":
                                parentName = "BuildingName";
                                break;
                            case "Rent":
                                parentName = "RenterName";
                                break;
                        }
                        int parentID = -1;
                        if (parentName != "")
                        {
                            parentID = GetParentId(parentName, comboBox.SelectedItem.ToString());
                        }
                        if (parentID != -1)
                        {
                            switch (parentName)
                            {
                                case "BuildingName":
                                    newRow["BuildingID"] = parentID;
                                    break;
                                case "RoomName":
                                    newRow["RoomID"] = parentID;
                                    break;
                                case "RenterName":
                                    newRow["RenterID"] = parentID;
                                    break;
                            }
                        }
                        comboBoxIndex++;
                    }
                }
                if (textBox != null)
                {
                    if (newRow == null)
                    {
                        newRow = dataSetService.dataSet.Tables[typeData.Name].NewRow();
                    }

                    newRow[property.Name] = textBox.Text;
                    textBoxIndex++;
                }

                if (datePicker != null)
                {
                    if (newRow == null)
                    {
                        newRow = dataSetService.dataSet.Tables[typeData.Name].NewRow();
                    }

                    newRow[property.Name] = datePicker.SelectedDate?.ToString("yyyy-MM-dd");
                }
            }

            if (newRow != null && isAddForm)
            {
                dataSetService.dataSet.Tables[typeData.Name].Rows.Add(newRow);
            }
            else if (newRow != null && isEditForm)//функцию использовать тут
            {
                dataSetService.dataSet.Tables[typeData.Name].Rows.Find(ID).ItemArray = newRow.ItemArray;
            }
            dataSetService.SaveDataSet();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Close();
    }

    private int GetParentId(string parentName, string parentValue)
    {
        DataTable parentTable;
        switch (parentName)
        {
            case "BuildingName":
                parentTable = dataSetService.dataSet.Tables["Building"];
                if (parentTable != null && parentTable.Columns.Contains("BuildingID") && parentTable.Columns.Contains("Name"))
                {
                    DataRow[] rows = parentTable.Select($"Name = '{parentValue}'");

                    if (rows.Length > 0)
                    {
                        // Возвращаем BuildingID первой найденной записи
                        return Convert.ToInt32(rows[0]["BuildingID"]);
                    }
                }
                break;
            case "RoomName":
                parentTable = dataSetService.dataSet.Tables["Room"];
                if (parentTable != null && parentTable.Columns.Contains("RoomID") && parentTable.Columns.Contains("Name"))
                {
                    DataRow[] rows = parentTable.Select($"Name = '{parentValue}'");

                    if (rows.Length > 0)
                    {
                        return Convert.ToInt32(rows[0]["RoomID"]);
                    }
                }
                break;
            case "RenterName":
                parentTable = dataSetService.dataSet.Tables["Renter"];
                if (parentTable != null && parentTable.Columns.Contains("RenterID") && parentTable.Columns.Contains("Name"))
                {
                    DataRow[] rows = parentTable.Select($"Name = '{parentValue}'");

                    if (rows.Length > 0)
                    {
                        return Convert.ToInt32(rows[0]["RenterID"]);
                    }
                }
                break;
        }
        return -1;
    }

    public static IEnumerable<T> FindLogicalChildren<T>(ILogical logical) where T : ILogical
    {
        if (logical != null)
        {
            foreach (var child in logical.LogicalChildren)
            {
                if (child is T typedChild)
                {
                    yield return typedChild;
                }

                foreach (var nestedChild in FindLogicalChildren<T>(child))
                {
                    yield return nestedChild;
                }
            }
        }
    }
    private long GetMaxId(DataTable dataTable)
    {
        if (dataTable != null && dataTable.Rows.Count > 0)
        {
            string primaryKeyColumnName = dataTable.Columns.Cast<DataColumn>()
                .FirstOrDefault(column => column.ColumnName.EndsWith("ID"))?.ColumnName;

            if (!string.IsNullOrEmpty(primaryKeyColumnName))
            {
                long maxId = dataTable.AsEnumerable().Max(row => row.Field<long>(primaryKeyColumnName));
                return maxId;
            }
            else
            {
                return -1;
            }
        }

        return 0;
    }

    public void CopyPropertiesToColumns(PropertyInfo[] properties, DataTable table)
    {

        if (table.Columns.Count == 0)
        {
            foreach (PropertyInfo property in properties)
            {
                DataColumn column = new DataColumn(property.Name);

                if (property.PropertyType == typeof(string))
                {
                    column.DataType = typeof(string);
                }
                else if (property.PropertyType == typeof(int))
                {
                    column.DataType = typeof(int);
                }
                else if (property.PropertyType == typeof(double))
                {
                    column.DataType = typeof(double);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    column.DataType = typeof(DateTime);
                }

                table.Columns.Add(column);
            }
        }
    }

}
