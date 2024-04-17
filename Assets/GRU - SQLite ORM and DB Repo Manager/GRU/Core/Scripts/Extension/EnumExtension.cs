using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Enums;

namespace SpatiumInteractive.Libraries.Unity.GRU.Extensions
{
    public static class EnumExtension
    {
        public static string GetAssemblyName(this AssemblySelection assemblySelected)
        {
            switch (assemblySelected)
            {
                case AssemblySelection.Everywhere:
                    return "Everywhere";
                case AssemblySelection.OutsideGRU:
                    return "Outside GRU";
                case AssemblySelection.InsideGRU:
                    return "Inside GRU";
                default:
                    return "Everywhere";
            }
        }
    }
}
