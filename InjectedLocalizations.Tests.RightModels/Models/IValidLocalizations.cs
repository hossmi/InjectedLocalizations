using System;

namespace InjectedLocalizations.Models
{
    public interface IValidLocalizations : ILocalizations
    {
        string Some_sample_property { get; }
        string That_tree_has_0_tasty_apples(int applesCount);
        string The_user_0_with_code_1_is_forbiden(string userName, Guid userId);
    }
}
