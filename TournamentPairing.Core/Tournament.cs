namespace TournamentPairing.Core;

public class Tournament : BasePropertyChanged
{
    public required string Name { get => Get<string>(); set => Set(value); }
    public string? LocalFilePath { get => Get<string?>(); set => Set(value); }
    public string? RemoteFilePath { get => Get<string?>(); set => Set(value); }
    public bool IsNew { get => Get<bool>(); set => Set(value); }
}
