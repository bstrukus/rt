/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Execute
{
    using System.Collections.Generic;

    internal class Levers
    {
        private const bool DebuggingEnabled = true;

        public enum Option
        {
            BooleanTest,
            RenderNormals,
            ViewVectorLighting,
            PrintSceneLoading,
        }

        private static Levers Instance;

        public static bool GetOption(Option option)
        {
            if (!DebuggingEnabled)
                return false;

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
                Option.ViewVectorLighting,

                // #levers Scene Loading
                Option.PrintSceneLoading,
            };
        }
    }
}