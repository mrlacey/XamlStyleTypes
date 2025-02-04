using System.Collections.Generic;
using System.Text;

namespace GeneratorCore
{
	// TODO: Move this in to the RX.CG.Wpf project
	public class WpfGeneratorLogic : BaseGeneratorLogic
    {
        private readonly string _name;

        public WpfGeneratorLogic(string name)
        {
            _name = name;
        }

        internal override string GetName() => _name;

        internal override List<string> GetDefaultNamespaces() => [];

        internal override void AddIndividualStyleClass(StringBuilder output, string key, string targetType, string fileIdentifier, bool includeResourceLoading = false)
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
