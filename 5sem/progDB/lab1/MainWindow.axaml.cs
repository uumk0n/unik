using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Tmds.DBus.Protocol;

namespace lab1;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ViewButton_Click(object sender, RoutedEventArgs e)
    {
        if (File.Exists("accounting_for_leased_premises.json"))
        {
            var viewForm = new ViewForm();
            viewForm.Show();
        }
        else
        {
            var msgBox = new MessageBox();
            msgBox.Show();
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        var editForm = new EditForm();
        editForm.Show();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var addForm = new AddForm();
        addForm.Show();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var deleteForm = new DeleteForm();
        deleteForm.Show();
    }
}