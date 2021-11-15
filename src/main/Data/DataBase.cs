/*
 * #copyright_placeholder Copyright Ben Strukus
 */

namespace rt.Data
{
    using rt.Utility;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Handles common pretty-print formatting
    /// </summary>
    /// <remarks>
    /// In general, it's not advised to add "Base" as a suffix for abstract classes, but in this case I couldn't help myself.
    /// </remarks>
    public abstract class DataBase
    {
        private int spaceCount;

        private string indentation
        {
            get
            {
                string spaces = new string(' ', this.spaceCount);

                return $"{spaces} - ";
            }
        }

        // Error checking
        public abstract bool IsValid();

        // Pretty-Print
        public virtual void PrintData(int spaceCount)
        {
            this.spaceCount = spaceCount;
            string title = this.GetType().Name.ToUpper();
            this.PrintTitle(title.Substring(0, title.Length - 4));
        }

        public virtual void Print(string label, string value)
        {
            Log.Info($"{this.indentation}{label}: {value}");
        }

        public virtual void Print(string label, int value)
        {
            Print(label, value.ToString());
        }

        public virtual void Print(string label, double value)
        {
            Print(label, value.ToString());
        }

        public virtual void Print(string label, List<double> list)
        {
            Log.Info($"{this.indentation}{label}: [{Format(list)}]");
        }

        public void PrintTitle(string label)
        {
            string spaces = this.spaceCount > 0 ? new string(' ', this.spaceCount - 2) : string.Empty;
            Log.Info($"{spaces}- {label}");
        }

        private static string Format(List<double> vectorDoubles)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < vectorDoubles.Count; ++i)
            {
                sb.Append($"{vectorDoubles[i]}");
                if (i != (vectorDoubles.Count - 1))
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}