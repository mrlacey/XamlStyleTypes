using System.Collections.Generic;
using System.Text;

namespace GeneratorCore
{
	// TODO: Move this in to the RX.CG.Maui project
	public class MauiGeneratorLogic : BaseGeneratorLogic
    {
        private readonly string _name;

        public MauiGeneratorLogic(string name)
        {
            _name = name;
        }

        internal override string GetName() => _name;

        // Rely on implicit namespaces for MAUI
        internal override List<string> GetDefaultNamespaces() => [];

        internal override void AddIndividualStyleClass(StringBuilder output, string key, string targetType, string fileIdentifier, bool includeResourceLoading = false)
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

            if (includeResourceLoading)
			{
				output.AppendLine($"        public {key}(ResourceId resourceId) : this()");
				output.AppendLine($"        {{");
				output.AppendLine($"            this.LoadResources(resourceId);");
				output.AppendLine($"        }}");
			}

            output.AppendLine($"    }}");
        }
    }
}
