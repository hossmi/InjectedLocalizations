namespace InjectedLocalizations.Models
{
    public interface IMethodNotMatchingParametersLocalizations : ILocalizations
    {
        string This_Method_has_0_and_1_parameters_but_it_does_not_have_arguments();
        string This_Method_has_0_parameter_but_it_has_three_arguments(int x, string y, double z);
        string This_Method_has_number_2_used_parameter_number_666_out_of_range_parameter_and_unused_parameters(int x, string y, double z);
    }
}
