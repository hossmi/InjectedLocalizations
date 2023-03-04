using System;

namespace InjectedLocalizations.Models
{
    public interface INotValidReturnTypeLocalizations : ILocalizations
    {
        int Some_sample_property { get; }
        double The_user_0_with_code_1_is_forbiden(string userName, Guid userId); // not valid return type
    }
}
