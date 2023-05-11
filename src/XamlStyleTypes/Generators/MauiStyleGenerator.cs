using GeneratorCore;

namespace XamlStyleTypes
{
    public class MauiStyleGenerator : XamlStyleGenerator
    {
        public const string Name = nameof(MauiStyleGenerator);

        public const string Description = "Create local types for XAML Styles in a .NET MAUI ResourceDictionary.";

        internal override BaseGeneratorLogic GetGeneratorLogic()
        {
            return new MauiGeneratorLogic(Name);
        }
    }
}
