using Microsoft.Extensions.Caching.Memory;

namespace Cache
{
	public sealed class LRUCache : ICache
	{

		static MemoryCacheOptions cacheOption = new MemoryCacheOptions { };
		static MemoryCacheEntryOptions cacheEntryOption = new MemoryCacheEntryOptions { };
		static MemoryCache cache = new MemoryCache(cacheOption);
		public Dictionary<int, int> _cacheLedger = new Dictionary<int, int>();
		private int _capacity = 0;
		private int counter = 0;
		
		public LRUCache(int capacity, int size)
        {
			cacheOption.SizeLimit = capacity;
			cacheEntryOption.Size = size;
			this._capacity = capacity;
			
		}

		// Associated unit tests need to be able to clear the cache
		public void EmptyTheCache()
        {
			List<int> cacheKeys = _cacheLedger.Select(c => c.Key).ToList();
			foreach (int cacheKey in cacheKeys)
			{
				cache.Remove(cacheKey);
			}
		}

		public object? GetCachedValueByKey(int key)
		{
			int counter = _cacheLedger[key];
			object existingCachedValue;

			// If the key exists in the cache
			if (cache.TryGetValue(key, out existingCachedValue))
            {
				// then update the counter for it in the ledger 
				_cacheLedger[key] = counter++;

				// then return the value
				return existingCachedValue;	
            }

			return null;
		}

		public void AddValueToCacheByKey(int key, object value)
		{
			// If the ledger isn't full
			if (_cacheLedger.Count < _capacity)
            {
				// then insert the new key/value pair into the memory cache, or overwrite it if it already exsits
				cache.Set(key, value, cacheEntryOption);

				// then update the ledger
				if (_cacheLedger.ContainsKey(key))
                {
					// increment the counter if it already exists in the ledger
					_cacheLedger[key] = counter++;
				}
				else
                {
					// or add a new entry for it to the ledger with the increased counter value
					_cacheLedger[key] = counter++;
                }
			}
			else
            {
				// work out which is the least used, which is the one with the lowest counter
				var leastUsed = _cacheLedger.OrderBy(c => c.Value).First().Key;

				// then remove the least used one from the cache
				cache.Remove(leastUsed);

				// and replace it with the new one
				cache.Set(key, value, cacheEntryOption);

				// and then remove it's reference in the ledger
				_cacheLedger.Remove(leastUsed);

				// and replace it with a new one
				_cacheLedger.Add(key, counter++);

			}
		}
	}
}
