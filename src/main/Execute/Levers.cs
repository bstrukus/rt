/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
    using System.Collections.Generic;

    internal static class Levers
    {
        public enum Option
        {
            // Lighting Calculation
            BooleanTest,
            RenderNormals,
            ViewVectorLighting,
            DisableShadows,

            // Scene Loading
            PrintSceneLoading,

            // Object Control
            LimitObjects,
        }

        public static int ObjectStart { get; private set; }
        public static int ObjectLimit { get; private set; }

        private static HashSet<Option> activeOptions = new HashSet<Option>();

        public static void Initialize(IEnumerable<Option> options, int objectStart = 0, int objectLimit = -1)
        {
            activeOptions = new HashSet<Option>(options);
            ObjectStart = objectStart;
            ObjectLimit = objectLimit;
        }

        public static bool GetOption(Option option)
        {
            return activeOptions.Contains(option);
        }
    }
}
