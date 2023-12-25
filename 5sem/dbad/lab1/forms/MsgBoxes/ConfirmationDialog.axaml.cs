using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace lab1;

public partial class ConfirmationDialog : Window
{
    bool DialogResult { get; set; }

    private DispatcherTimer timer;
    private TaskCompletionSource<bool> closeCompletionSource;
    public ConfirmationDialog(string question)
    {
        InitializeComponent();
        var textBlock = this.FindControl<TextBlock>("QuestionText");
        textBlock.Text = question;
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(0.1);
        timer.Tick += Timer_Tick;

        closeCompletionSource = new TaskCompletionSource<bool>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.DialogResult = true;
        timer.Start();
    }

    private void Button_Click_1(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.DialogResult = false;
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

    public bool getResult()
    {
        return DialogResult;
    }
}
