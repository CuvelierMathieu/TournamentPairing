using MahApps.Metro.Controls;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace TournamentPairing;

public partial class MainWindow : MetroWindow
{
    private const string AbsurdDefaultPassword = "123456790";
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<MainWindow>()
            .AddJsonFile("config.json")
            .AddJsonFile("dimensions.json")
            .Build();

        _viewModel = new(config, new FtpUploader());
        DataContext = _viewModel;

        if (_viewModel.FtpConnectionParameter.Password is not null)
            FtpPasswordBox.Password = AbsurdDefaultPassword;

        if (_viewModel.ConfigDimensions.HasValue)
        {
            Height = _viewModel.ConfigDimensions.Value.Height;
            Width = _viewModel.ConfigDimensions.Value.Width;
        }
    }

    private void FtpPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (FtpPasswordBox.Password == AbsurdDefaultPassword)
            return;

        _viewModel.FtpConnectionParameter.Password = FtpPasswordBox.Password;
    }

    private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        var dimensions = new
        {
            Height = ActualHeight,
            Width = ActualWidth
        };

        string json = JsonConvert.SerializeObject(dimensions);

        File.WriteAllText("dimensions.json", json);
    }
}