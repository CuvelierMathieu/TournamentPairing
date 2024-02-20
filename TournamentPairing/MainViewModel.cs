using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System.IO;
using System.Windows.Input;
using TournmanetPairing.Core;

namespace TournamentPairing;

public class MainViewModel : BasePropertyChanged
{
    private readonly FtpUploader _ftpUploader;

    public FtpConnectionParameter FtpConnectionParameter { get; set; }
    public List<Tournament>? Tournaments { get; set; }
    public ICommand ModifyLocalPathCommand { get; set; }
    public ICommand UploadCommand { get; set; }
    public bool IsEnabled { get => Get<bool>(); set => Set(value); }

    public MainViewModel(IConfigurationRoot config, FtpUploader ftpUploader)
    {
        LoadFromConfig(config);

        ModifyLocalPathCommand = new RelayCommand<Tournament>(ModifyLocalPath);
        UploadCommand = new RelayCommand<Tournament>(Upload, CanUpload);
        _ftpUploader = ftpUploader;
        IsEnabled = true;
    }

    private void ModifyLocalPath(Tournament tournament)
    {
        OpenFileDialog openFileDialog = new()
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Title = $"Choix du fichier pour le tournoi {tournament.Name}"
        };

        if (openFileDialog.ShowDialog() == true)
            tournament.LocalFilePath = openFileDialog.FileName;
    }

    private bool CanUpload(Tournament tournament)
    {
        return !string.IsNullOrWhiteSpace(tournament.LocalFilePath)
            && !string.IsNullOrWhiteSpace(tournament.RemoteFilePath)
            && File.Exists(tournament.LocalFilePath);
    }

    private void Upload(Tournament tournament)
    {
        IsEnabled = false;

        _ftpUploader.Upload(new UploadParameter()
        {
            FtpConnectionParameter = FtpConnectionParameter,
            LocalFilePath = tournament.LocalFilePath,
            RemoteFilePath = tournament.RemoteFilePath
        });

        IsEnabled = true;
    }

    private void LoadFromConfig(IConfigurationRoot config)
    {
        int i = 0;
        Tournaments = [];

        while (true)
        {
            if (config[$"Tournaments:{i}:Name"] is null)
                break;

            Tournaments.Add(new()
            {
                Name = config[$"Tournaments:{i}:Name"],
                LocalFilePath = config[$"Tournaments:{i}:LocalFilePath"],
                RemoteFilePath = config[$"Tournaments:{i}:RemoteFilePath"]
            });

            i++;
        }

        FtpConnectionParameter = new()
        {
            Address = config["FtpAddress"],
            Username = config["Username"],
            Password = config["Password"]
        };
    }
}
