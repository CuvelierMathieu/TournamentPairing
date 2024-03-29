﻿using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TournamentPairing.Core;

namespace TournamentPairing;

public class MainViewModel : BasePropertyChanged
{
    private readonly FtpUploader _ftpUploader;

    public FtpConnectionParameter FtpConnectionParameter { get; set; }
    public ObservableCollection<Tournament>? Tournaments { get; set; }
    public ICommand ModifyLocalPathCommand { get; set; }
    public ICommand UploadCommand { get; set; }
    public ICommand DeleteTournamentCommand { get; set; }
    public ICommand CreateTournamentCommand { get; set; }
    public ICommand SaveConfigCommand { get; set; }
    public ICommand ModifyEmptyFileCommand { get; set; }
    public ICommand ResetAllTournamentsCommand { get; set; }
    public bool IsEnabled { get => Get<bool>(); set => Set(value); }
    public (double Height, double Width)? ConfigDimensions { get; private set; }

    public MainViewModel(IConfigurationRoot config, FtpUploader ftpUploader)
    {
        LoadFromConfig(config);

        ModifyLocalPathCommand = new RelayCommand<Tournament>(ModifyLocalPath);
        UploadCommand = new RelayCommand<Tournament>(Upload, CanUpload);
        DeleteTournamentCommand = new RelayCommand<Tournament>(DeleteTournament);
        CreateTournamentCommand = new RelayCommand(CreateTournament);
        SaveConfigCommand = new RelayCommand(SaveConfig);
        ModifyEmptyFileCommand = new RelayCommand(ModifyEmptyFile);
        ResetAllTournamentsCommand = new RelayCommand(ResetAllTournaments, CanResetAllTournaments);

        _ftpUploader = ftpUploader;
        IsEnabled = true;
    }

    private bool CanResetAllTournaments()
    {
        return File.Exists(FtpConnectionParameter.EmptyFile);
    }

    private void ResetAllTournaments()
    {
        IsEnabled = false;

        List<(Tournament t, Exception e)> errors = new();

        foreach (Tournament tournament in Tournaments)
        {
            try
            {
                _ftpUploader.Upload(new UploadParameter()
                {
                    FtpConnectionParameter = FtpConnectionParameter,
                    LocalFilePath = FtpConnectionParameter.EmptyFile,
                    RemoteFilePath = tournament.RemoteFilePath
                });

            }
            catch (Exception e)
            {
                errors.Add((tournament, e));
            }
        }

        if (errors.Any())
        {
            StringBuilder sb = new("Des erreurs ont eu lieu :\n");

            foreach ((Tournament t, Exception e) in errors)
                sb.AppendLine($"- Tournoi {t.Name} : {e}");

            MessageBox.Show(sb.ToString(), "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
            MessageBox.Show($"Les tournois ont bien été réinitialisés", "Envoi", MessageBoxButton.OK, MessageBoxImage.Information);

        IsEnabled = true;
    }

    private void ModifyEmptyFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Title = $"Choix du fichier vide"
        };

        if (openFileDialog.ShowDialog() == true)
            FtpConnectionParameter.EmptyFile = openFileDialog.FileName;
    }

    private void SaveConfig()
    {
        foreach (Tournament tournament in Tournaments)
            if (tournament.IsNew)
                tournament.IsNew = false;

        var config = new
        {
            Tournaments = Tournaments,
            FtpAddress = FtpConnectionParameter.Address,
            Username = FtpConnectionParameter.Username,
            Password = FtpConnectionParameter.Password,
            EmptyFile = FtpConnectionParameter.EmptyFile,
        };

        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText("config.json", json);
    }

    private void CreateTournament()
    {
        Tournaments.Add(new() { Name = string.Empty, IsNew = true });
    }

    private void DeleteTournament(Tournament tournament)
    {
        MessageBoxResult result = MessageBox.Show(
            $"Êtes-vous sûr de vouloir supprimer le tournoir {tournament.Name} ?",
            "Suppression",
            MessageBoxButton.OKCancel,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Cancel)
            return;

        Tournaments.Remove(tournament);

        MessageBox.Show("Suppression effectuée. Pensez à enregistrer vos modifications.", "Suppression", MessageBoxButton.OK, MessageBoxImage.Information);
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
        try
        {
            IsEnabled = false;

            _ftpUploader.Upload(new UploadParameter()
            {
                FtpConnectionParameter = FtpConnectionParameter,
                LocalFilePath = tournament.LocalFilePath,
                RemoteFilePath = tournament.RemoteFilePath
            });

            MessageBox.Show($"Le fichier a bien été envoyé !", "Envoi", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            MessageBox.Show($"Une erreur s'est produite durant l'envoi : {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsEnabled = true;
        }
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
            Password = config["Password"],
            EmptyFile = config["EmptyFile"]
        };

        ConfigDimensions = double.TryParse(config["Height"].Replace('.', ','), out double height)
            && double.TryParse(config["Width"].Replace('.', ','), out double width)
            ? (height, width)
            : null;
    }
}
