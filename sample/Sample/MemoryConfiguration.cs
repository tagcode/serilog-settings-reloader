using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Sample
{
    /// <summary>Memory configuration</summary>
    public class MemoryConfiguration : ConfigurationProvider, IEnumerable<KeyValuePair<string, string>>, IConfigurationSource
    {
        /// <summary>Expose inner configuration data</summary>
        public new IDictionary<String, String> Data => base.Data;
        /// <summary>Configuration data</summary>
        public string this[string key] { get => base.Data[key]; set => base.Data[key] = value; }

        /// <summary>Create memory configuration</summary>
        public MemoryConfiguration() : base() { }

        /// <summary>Assign <paramref name="value"/> to <paramref name="key"/></summary>
        public new MemoryConfiguration Set(string key, string value) { base.Data[key] = value; return this; }
        /// <summary>Build configuration provider.</summary>
        public IConfigurationProvider Build(IConfigurationBuilder builder) => this;
        /// <summary>Enumerate</summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Data.GetEnumerator();
        /// <summary>Enumerate</summary>
        IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
    }
}
