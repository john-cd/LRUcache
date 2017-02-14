using System;
using System.Collections.Generic;

namespace CacheLib
{
    // LRU cache i.e. a cache that, when low on memory, evicts least recently used items.
    public class Cache<K, V>
    {
        private readonly int _capacity;
        //
        private readonly object _syncRoot = new object(); // coarse synchronization for thread safety 
        internal readonly IDictionary<K, V> _internalStorage; // stores the values
        internal readonly LinkedList<K> _lru; // stores the most recent used keys; first = most recently used

        public Cache(int capacity = 50_000) 
        {
            this._capacity = (capacity > 0)? capacity : throw new ArgumentOutOfRangeException("capacity cannot be less or equal than zero.", nameof(capacity));
            this._internalStorage = new Dictionary<K, V>(this._capacity);
            this._lru = new LinkedList<K>();
        }

        public void Add(K key, V value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            lock (_syncRoot)
            {
                if (!_internalStorage.ContainsKey(key))
                {
                    if (_internalStorage.Count >= _capacity)
                    {
                        // remove least used key
                        _internalStorage.Remove(_lru.Last.Value);
                        _lru.RemoveLast();
                    }
                    _internalStorage.Add(key, value);
                    _lru.Remove(key);
                    _lru.AddFirst(key);
                }
                else
                    throw new ArgumentException("An element with the same key already exists.", nameof(key));
            }
        }

        public bool TryGetValue(K key, out V value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            lock (_syncRoot)
            {
                if (_internalStorage.TryGetValue(key, out value))
                {
                    // key found - move to the top of the list 
                    _lru.Remove(key);
                    _lru.AddFirst(key);
                    return true;
                }
                // else - key not found.
                value = default(V);
                return false;
            }
        }

        // Possible improvements:
        // * LRU are traditionally implemented using a Hashmap (O(1) access to value) and LinkedList (O(1) access to last). 
        // For very large datasets, a probabilistic data structure (bloom filter) is a better choice: https://en.wikipedia.org/wiki/Bloom_filter
        // * Fine-grained concurrency control with read-write locks and/or use of concurent collections.
        // * Storage in a single data structure is possible, if strict LRU is not required; for example, choose a random subset of elements and evict the oldest.

    }
}
