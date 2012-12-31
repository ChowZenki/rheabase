using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Configuration
{
    public class PropertyTree
    {
        private readonly Dictionary<string, PropertyTree> _Tree;
        private readonly StringComparer _StringComparison;
        private string _Value;

        public PropertyTree()
            : this(StringComparer.InvariantCultureIgnoreCase)
        {

        }

        public PropertyTree(StringComparer comparison)
        {
            _StringComparison = comparison;
            _Tree = new Dictionary<string, PropertyTree>(comparison);
        }

        public T Value<T>()
        {
            return (T)Convert.ChangeType(_Value, typeof(T));
        }

        public void SetValue<T>(T value)
        {
            _Value = value.ToString();
        }

        public T Get<T>(string path, T defaultValue = default(T))
        {
            PropertyTree tree = this;

            return path.Split('.').Any(part => !tree._Tree.TryGetValue(part, out tree) || tree == null) ? defaultValue : tree.Value<T>();
        }

        public void Set<T>(string path, T value)
        {
            string[] parts = path.Split('.');
            PropertyTree tree = this;

            foreach (string part in parts)
            {
                PropertyTree tree2;

                if (!tree._Tree.TryGetValue(part, out tree2) || tree2 == null)
                {
                    tree2 = new PropertyTree(_StringComparison);
                    tree._Tree.Add(part, tree2);
                }

                tree = tree2;
            }

            tree.SetValue(value);
        }

        public void FromConfigFile(string file)
        {
            FromConfigFile(new StreamReader(file));
        }

        public void FromConfigFile(TextReader reader)
        {
            string text = reader.ReadToEnd();
            string[] lines = text.Replace("\r", "\n").Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (line == "")
                    continue;

                if (line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                {
                    Set(parts[0].Trim(), parts[1].Trim());
                }
            }
        }
    }
}
