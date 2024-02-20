namespace TournamentPairing.Core;

public class FtpConnectionParameter : BasePropertyChanged
{
    public required string Address { get => Get<string>(); set => Set(value); }
    public required string Username { get => Get<string>(); set => Set(value); }
    public required string Password { get => Get<string>(); set => Set(value); }
}
