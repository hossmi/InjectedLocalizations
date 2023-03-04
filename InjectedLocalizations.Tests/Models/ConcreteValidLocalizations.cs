using System.Globalization;

namespace InjectedLocalizations.Models
{
    public class ConcreteValidLocalizations : IValidLocalizations
    {
        public ConcreteValidLocalizations(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public string Some_sample_property => "Propiedad de ejemplo.";

        public IServiceProvider ServiceProvider { get; }
        public CultureInfo Culture { get; } = new CultureInfo("es-ES");

        public string That_tree_has_0_tasty_apples(int applesCount) => $"Ese árbol tiene {applesCount} sabrosas manzanas.";

        public string The_user_0_with_code_1_is_forbiden(string userName, Guid userId) => $"El usuario {userName} con código {userId} no está permitido.";
    }
}
