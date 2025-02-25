using GeneratorCore;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace XamlStyleTypes
{
    public abstract class XamlStyleGenerator : BaseCodeGeneratorWithSite
    {
        public override string GetDefaultExtension() => ".cs";

        internal abstract BaseGeneratorLogic GetGeneratorLogic();

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            return GetGeneratorLogic().GenerateCode(inputFileName, inputFileContent, FileNamespace, includeResourceLoading: false);
        }
    }
}
