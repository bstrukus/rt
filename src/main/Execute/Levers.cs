/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
    using System.Collections.Generic;

    internal class Levers
    {
        // #todo Read all of these values in from the config
        private const bool DebuggingEnabled = true;

        public const int ObjectStart = 43;
        public const int ObjectLimit = 2;

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

        private static Levers Instance;

        public static bool GetOption(Option option)
        {
#pragma warning disable 0162
            if (!DebuggingEnabled)
                return false;
#pragma warning restore 0162

            if (Instance == null)
            {
                Instance = new Levers();
            }
            return Instance.options.Contains(option);
        }

        private HashSet<Option> options;

        private Levers()
        {
            this.options = new HashSet<Option>
            {
                // #levers Lighting Calculation
                //Option.BooleanTest,
                //Option.RenderNormals,
                //Option.ViewVectorLighting,
                //Option.DisableShadows,

                // #levers Scene Loading
                //Option.PrintSceneLoading,

                // #levers Object Control
                //Option.LimitObjects,
            };
        }
    }
}