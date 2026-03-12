using System;
using System.Windows.Input;

namespace Rockstar.Admin.WPF.ViewModels.Base
{
    /// <summary>
    /// RelayCommand без параметра
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    /// <summary>
    /// RelayCommand с параметром типа T
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null)
            {
                // Если параметр null — разрешаем только если canExecute не задан
                // или если T — ссылочный тип и мы явно разрешаем null
                return _canExecute == null || !typeof(T).IsValueType;
            }

            if (parameter is T value)
            {
                return _canExecute?.Invoke(value) ?? true;
            }

            return false;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T value)
            {
                _execute(value);
            }
            // Если parameter == null и T — ссылочный тип, можно передать null
            else if (parameter == null && !typeof(T).IsValueType)
            {
                _execute((T)(object)null!);
            }
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}