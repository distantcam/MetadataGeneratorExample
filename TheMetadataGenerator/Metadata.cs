using System.Collections;
using System.Collections.Generic;

namespace TheMetadataGenerator
{
    abstract class Metadata : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> fields = new Dictionary<string, string>();

        public string Name { get; }

        public Metadata(string name)
        {
            Name = name;
        }

        public void AddField(string name, string description)
        {
            fields.Add(name, description);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => fields.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
