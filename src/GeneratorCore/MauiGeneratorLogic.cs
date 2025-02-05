using System.Collections.Generic;
using System.Text;

namespace GeneratorCore
{
	// TODO: Move this in to the RX.CG.Maui project
	public class MauiGeneratorLogic : BaseGeneratorLogic
    {
        private readonly string _name;
		private readonly string _version;

		public MauiGeneratorLogic(string name, string version)
        {
            _name = name;
			_version = version;
		}

        internal override string GetName() => _name;

        // Rely on implicit namespaces for MAUI
        internal override List<string> GetDefaultNamespaces() => [];

        internal override void AddIndividualStyleClass(StringBuilder output, string key, string targetType, string fileIdentifier, bool includeResourceLoading = false)
        {
            output.AppendLine();

			// See: https://learn.microsoft.com/en-gb/archive/blogs/codeanalysis/correct-usage-of-the-compilergeneratedattribute-and-the-generatedcodeattribute
			output.AppendLine($"    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(\"{GetName()}\", \"{_version}\")]");
			output.AppendLine($"    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
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
