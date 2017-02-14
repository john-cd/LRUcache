# LRUcache

A test implementation of a simple LRU cache in C#

## Possible improvements

* LRU are traditionally implemented using a Hashmap (O(1) access to value) and LinkedList (O(1) access to last). 
* For very large datasets, a probabilistic data structure (bloom filter) is a better choice: https://en.wikipedia.org/wiki/Bloom_filter
* Fine-grained concurrency control with read-write locks and/or use of concurent collections.
* Storage in a single data structure is possible, if strict LRU is not required; for example, choose a random subset of elements and evict the oldest.
