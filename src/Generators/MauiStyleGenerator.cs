using System.Collections.Generic;
using System.Text;

namespace XamlStyleTypes
{
    public class MauiStyleGenerator : XamlStyleGenerator
    {
        public const string Name = nameof(MauiStyleGenerator);

        public const string Description = "Create local types for XAML Styles in a .NET MAUI ResourceDictionary.";

        internal override string GetName() => Name;

        // Rely on implicit namespaces for MAUI
        internal override List<string> GetDefaultNamespaces() => new() { };

        internal override void AddIndividualStyleClass(StringBuilder output, string key, string targetType, string fileIdentifier)
        {
            output.AppendLine();
            output.AppendLine($"    public class {key} : {targetType}");
            output.AppendLine($"    {{");
            output.AppendLine($"        public {key}()");
            output.AppendLine($"        {{");
            output.AppendLine($"            if (App.Current.Resources.TryGetValue(\"{key}\", out object result))");
            output.AppendLine($"            {{");
            output.AppendLine($"                this.Style = result as Style;");
            output.AppendLine($"            }}");
            output.AppendLine($"        }}");
            output.AppendLine($"    }}");
        }
    }
}
