using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Newtonsoft.Json;


namespace lab2;

public partial class ActionsForm : Window
{
    bool isEditForm;
    bool isAddForm;
    Type typeData;
    int ID = -1;
    DataHandler dataHandler;
    public ActionsForm(string title, List<object> rows, Type type, bool isEdit = false, bool isAdd = false)
    {
        isEditForm = isEdit;
        isAddForm = isAdd;
        typeData = type;
        dataHandler = new DataHandler("accounting_for_leased_premises.db");
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
                            ID = GetPropertyValue<int>(rows[0], property.Name);
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
                label.SetValue(Avalonia.Layout.Layoutable.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Bottom);
                label.SetValue(Avalonia.Layout.Layoutable.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);
                totalHeight += labelHeight;

                if (property.PropertyType == typeof(int))
                {
                    var textBox = new TextBox();
                    textBox.Margin = new Thickness(5);
                    textBox.Width = 200;
                    double textBoxHeight = textBox.Height;
                    totalHeight += textBoxHeight;

                    textBox.SetValue(Avalonia.Layout.Layoutable.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Bottom);
                    textBox.SetValue(Avalonia.Layout.Layoutable.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);

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


                        comboBox.SetValue(Avalonia.Layout.Layoutable.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Bottom);
                        comboBox.SetValue(Avalonia.Layout.Layoutable.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);

                        comboBox1.SetValue(Avalonia.Layout.Layoutable.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Bottom);
                        comboBox1.SetValue(Avalonia.Layout.Layoutable.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);


                        switch (typeData.Name)
                        {
                            case "Room":
                                comboBox.ItemsSource = dataHandler.GetColumnValuesFromTable("Building", "Name");
                                break;
                            case "Rent":
                                if (property.Name == "RoomName")
                                    comboBox1.ItemsSource = dataHandler.GetColumnValuesFromTable("Room", "Name");
                                else
                                    comboBox.ItemsSource = dataHandler.GetColumnValuesFromTable("Renter", "CompanyName");
                                break;
                        }
                        stackPanel.Children.Add(label);
                        if (comboBox.Items.Count != 0)
                            stackPanel.Children.Add(comboBox);
                        if (comboBox1.ItemCount != 0)
                            stackPanel.Children.Add(comboBox1);
                    }
                    else
                    {
                        var textBox = new TextBox();
                        textBox.Margin = new Thickness(5);
                        textBox.Width = 200;
                        double textBoxHeight = textBox.Height;
                        totalHeight += textBoxHeight;

                        textBox.SetValue(Avalonia.Layout.Layoutable.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Bottom);
                        textBox.SetValue(Avalonia.Layout.Layoutable.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);


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


                    datePicker.SetValue(Avalonia.Layout.Layoutable.VerticalAlignmentProperty, Avalonia.Layout.VerticalAlignment.Bottom);
                    datePicker.SetValue(Avalonia.Layout.Layoutable.HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);


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

    static T GetPropertyValue<T>(object obj, string propertyName)
    {
        Type type = obj.GetType();
        PropertyInfo property = type.GetProperty(propertyName);

        if (property != null)
        {
            object value = property.GetValue(obj);
            return (T)value;
        }

        throw new ArgumentException($"Property with name '{propertyName}' not found in type '{type.Name}'.");
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

    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

        StackPanel stackPanel = this.FindControl<StackPanel>("stackPanel");
        if (!checkControls(stackPanel))
            return;
        dynamic formData = GetDataFromForm(stackPanel, ID, isEditForm, isAddForm);
        dataHandler.SaveToDb(formData, typeData, dataHandler.GetPrimaryKeyColumn(typeData.Name), isEditForm, isAddForm);
        Close();
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

    private dynamic GetDataFromForm(StackPanel stackPanel, int ID, bool isEditForm = false, bool isAddForm = false)
    {
        dynamic formData = new ExpandoObject();

        var formDataDict = formData as IDictionary<string, object>;

        try
        {
            var properties = typeData.GetProperties();

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

                if (property.Name.EndsWith("ID") && isEditForm && dataHandler.GetPrimaryKeyColumn(typeData.Name) == property.Name)
                {
                    formDataDict[property.Name] = ID;
                    continue;
                }
                else if (property.Name.EndsWith("ID") && isAddForm)
                {
                    switch (typeData.Name)
                    {
                        case "Building":
                            if (dataHandler.TableHasRows(typeData.Name))
                                formDataDict[property.Name] = dataHandler.GetMaxId(typeData.Name, property.Name) + 1;
                            else
                                formDataDict[property.Name] = 1;
                            continue;

                        case "Room":
                            if (property.Name == "RoomID")
                            {
                                if (dataHandler.TableHasRows(typeData.Name))
                                    formDataDict[property.Name] = dataHandler.GetMaxId(typeData.Name, property.Name) + 1;
                                else
                                    formDataDict[property.Name] = 1;
                                continue;
                            }
                            break;
                        case "Renter":
                            if (dataHandler.TableHasRows(typeData.Name))
                                formDataDict[property.Name] = dataHandler.GetMaxId(typeData.Name, property.Name) + 1;
                            else
                                formDataDict[property.Name] = 1;
                            continue;
                        case "Rent":
                            if (property.Name == "RentID")
                            {
                                if (dataHandler.TableHasRows(typeData.Name))
                                    formDataDict[property.Name] = dataHandler.GetMaxId(typeData.Name, property.Name) + 1;
                                else
                                    formDataDict[property.Name] = 1;
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


                if (textBox != null)
                {
                    formDataDict[property.Name] = textBox.Text;
                    textBoxIndex++;
                }

                if (datePicker != null)
                {
                    formDataDict[property.Name] = datePicker.SelectedDate?.ToString("yyyy-MM-dd");
                }

                if (comboBox != null)
                {
                    if (property.Name.EndsWith("ID"))
                    {
                        switch (typeData.Name) // Вместо Building укажите ваш тип данных
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
                            parentID = dataHandler.GetParentId(parentName, comboBox.SelectedItem.ToString());
                        }
                        if (parentID != -1)
                        {
                            switch (parentName)
                            {
                                case "BuildingName":
                                    formDataDict[property.Name] = parentID;
                                    break;
                                case "RoomName":
                                    formDataDict[property.Name] = parentID;
                                    break;
                                case "RenterName":
                                    formDataDict[property.Name] = parentID;
                                    break;
                            }
                        }
                        comboBoxIndex++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return formData;
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

}
