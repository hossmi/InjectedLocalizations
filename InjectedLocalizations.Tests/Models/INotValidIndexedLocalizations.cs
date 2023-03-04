namespace InjectedLocalizations.Models
{
    public interface INotValidIndexedLocalizations : ILocalizations
    {
        double this[string someParameter] { get; }
    }
}
