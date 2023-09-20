using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;


namespace lab1;

public partial class MessageBox : Window
{
    private DispatcherTimer timer;
    public MessageBox()
    {
        InitializeComponent();

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(2);
        timer.Tick += Timer_Tick;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        var messageText = this.FindControl<TextBlock>("MessageText");
        messageText.Text = "Базы данных нет на компьютере. Создать?";
        
        var yesButton = this.FindControl<Button>("YesButton");
        var noButton = this.FindControl<Button>("NoButton");

        yesButton.Click += YesButton_Click;
        noButton.Click += NoButton_Click;
    }

    private void YesButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DataSetService dataSetService = new DataSetService();
        dataSetService.CreateDataSet();

        var messageText = this.FindControl<TextBlock>("MessageText");
        messageText.Text = "База данных создана";

        timer.Start();
    }
    private void NoButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => this.Hide();

    private void Timer_Tick(object sender, EventArgs e)
    {
        Hide();
        timer.Stop();
    }
}