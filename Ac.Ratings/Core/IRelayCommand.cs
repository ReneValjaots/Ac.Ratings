using System.Windows.Input;

namespace Ac.Ratings.Core {
    public interface IRelayCommand<in T> : IRelayCommand {
        bool CanExecute(T? parameter);
        void Execute(T? parameter);
    }

    public interface IRelayCommand : ICommand {
        void NotifyCanExecuteChanged();
    }
}