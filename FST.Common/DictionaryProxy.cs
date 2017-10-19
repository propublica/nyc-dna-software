// this code is borrowed from the internet. thank you, internet

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace FST.Common
{
    /// <summary>
    /// Proxy class to permit XML Serialization of generic dictionaries
    /// </summary>
    /// <typeparam name="K">The type of the key</typeparam>
    /// <typeparam name="V">The type of the value</typeparam>
    public class DictionaryProxy<K, V>
    {
        #region Construction and Initialization
        public DictionaryProxy(IDictionary<K, V> original)
        {
            Original = original;
        }

        /// <summary>
        /// Default constructor so deserialization works
        /// </summary>
        public DictionaryProxy()
        {
        }

        /// <summary>
        /// Use to set the dictionary if necessary, but don't serialize
        /// </summary>
        [XmlIgnore]
        public IDictionary<K, V> Original { get; set; }
        #endregion

        #region The Proxy List
        /// <summary>
        /// Holds the keys and values
        /// </summary>
        public class KeyAndValue
        {
            public K Key { get; set; }
            public V Value { get; set; }
        }

        // This field will store the deserialized list
        private Collection<KeyAndValue> _list;

        /// <remarks>
        /// XmlElementAttribute is used to prevent extra nesting level. It's
        /// not necessary.
        /// </remarks>
        [XmlElement]
        public Collection<KeyAndValue> KeysAndValues
        {
            get
            {
                if (_list == null)
                {
                    _list = new Collection<KeyAndValue>();
                }

                // On deserialization, Original will be null, just return what we have
                if (Original == null)
                {
                    return _list;
                }

                // If Original was present, add each of its elements to the list
                _list.Clear();
                foreach (var pair in Original)
                {
                    _list.Add(new KeyAndValue { Key = pair.Key, Value = pair.Value });
                }

                return _list;
            }
        }
        #endregion

        /// <summary>
        /// Convenience method to return a dictionary from this proxy instance
        /// </summary>
        /// <returns></returns>
        public Dictionary<K, V> ToDictionary()
        {
            return KeysAndValues.ToDictionary(key => key.Key, value => value.Value);
        }
    }
}
