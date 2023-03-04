namespace InjectedLocalizations.Models
{
    public interface IGenericLocalizations<T> : ILocalizations
    {
        string Some_method_witch_has_0_as_parameter(T someParameter);
    }
}
