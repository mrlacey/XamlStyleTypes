using System.Collections.Generic;
using System.Text;

namespace GeneratorCore
{
    public class WpfGeneratorLogic : BaseGeneratorLogic
    {
        private readonly string _name;

        public WpfGeneratorLogic(string name)
        {
            _name = name;
        }

        internal override string GetName() => _name;

        internal override List<string> GetDefaultNamespaces() => new() { };

        internal override void AddIndividualStyleClass(StringBuilder output, string key, string targetType, string fileIdentifier)
        {
            output.AppendLine();
            output.AppendLine($"    public class {key} : {targetType}");
            output.AppendLine($"    {{");
            output.AppendLine($"        public {key}()");
            output.AppendLine($"        {{");
            output.AppendLine($"            this.Style = (Style)App.Current.Resources[\"{key}\"];");
            output.AppendLine($"        }}");
            output.AppendLine($"    }}");
        }
    }
}
