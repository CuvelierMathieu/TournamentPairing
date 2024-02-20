using System.Windows.Input;

namespace TournamentPairing
{
    internal class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;


        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action execute) : this(execute, CanAlwaysExecute)
        { }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _execute();

        private static bool CanAlwaysExecute() => true;
    }

    internal class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;


        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<T> execute) : this(execute, CanAlwaysExecute)
        { }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is not T typedParameter)
                return false;

            return _canExecute(typedParameter);
        }

        public void Execute(object? parameter)
        {
            if (parameter is not T typedParameter)
                throw new ArgumentException(null, nameof(parameter));

            _execute((T)parameter);
        }

        private static bool CanAlwaysExecute<T>(T parameter) => true;
    }
}
