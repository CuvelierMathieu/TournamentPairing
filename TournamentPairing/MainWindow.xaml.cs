using Microsoft.Extensions.Configuration;
using System.Windows;

namespace TournamentPairing;

public partial class MainWindow : Window
{
    private const string AbsurdDefaultPassword = "123456790";
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        IConfigurationRoot config = new ConfigurationBuilder()
            .AddUserSecrets<MainWindow>()
            .AddJsonFile("config.json")
            .Build();

        _viewModel = new(config, new FtpUploader());
        DataContext = _viewModel;

        if (_viewModel.FtpConnectionParameter.Password is not null)
            FtpPasswordBox.Password = AbsurdDefaultPassword;
    }

    private void FtpPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (FtpPasswordBox.Password == AbsurdDefaultPassword)
            return;

        _viewModel.FtpConnectionParameter.Password = FtpPasswordBox.Password;
    }
}