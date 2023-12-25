using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;


namespace lab2;

public partial class MsgBox : Window
{
    private DispatcherTimer timer;
    private TaskCompletionSource<bool> closeCompletionSource;
    private string m_msgText = "";
    private bool m_isVisibleBtns = true;
    private bool isCascade = false;
    public MsgBox(string title, string text = "Базы данных нет на компьютере. Создать?", bool isVisibleBtns = true, bool isCascade = false)
    {
        m_msgText = text;
        m_isVisibleBtns = isVisibleBtns;
        this.Title = title;
        this.isCascade = isCascade;
        InitializeComponent();

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(0.1);
        timer.Tick += Timer_Tick;

        closeCompletionSource = new TaskCompletionSource<bool>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        var messageText = this.FindControl<TextBlock>("MessageText");
        messageText.Text = m_msgText;

        var yesButton = this.FindControl<Button>("YesButton");
        var noButton = this.FindControl<Button>("NoButton");

        yesButton.Click += YesButton_Click;
        noButton.Click += NoButton_Click;

        yesButton.IsVisible = m_isVisibleBtns;
        noButton.IsVisible = m_isVisibleBtns;
    }

    private void YesButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!isCascade)
        {
            DataHandler dataHandler = new DataHandler("accounting_for_leased_premises.db");
            dataHandler.CreateDatabase();
            DataSet dataSet = dataHandler.GetDataSetFromJson("accounting_for_leased_premises.json");
            dataHandler.InsertDataSetIntoSQLite(dataSet);

            var messageText = this.FindControl<TextBlock>("MessageText");
            messageText.Text = "База данных создана";

        }
        timer.Start();
    }

    public bool getDeleteMode()
    {
        return isCascade;
    }
    private void NoButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e){
        isCascade = false;
        timer.Start();
    }

    public async Task WaitForCloseAsync()
    {
        await closeCompletionSource.Task;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        Close();
        timer.Stop();

        closeCompletionSource.SetResult(true); // Устанавливаем результат завершения задачи
    }
}