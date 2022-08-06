using Cache.LRU;
using System.Collections.Generic;

namespace Cache
{
	public sealed class LRUCache : ICache
	{
		private readonly object cacheLock = new object();
		private int capacity;
		private int count;
		Dictionary<int, LRUNode> map;
		LRUDoubleLinkedList doubleLinkedList;
		public LRUCache(int capacity)
		{
			this.capacity = capacity;
			count = 0;
			map = new Dictionary<int, LRUNode>();
			doubleLinkedList = new LRUDoubleLinkedList();
		}

		public object GetCachedValueByKey(int key)
		{
			lock (cacheLock)
			{

				if (!map.ContainsKey(key)) return -1;
				LRUNode node = map[key];
				doubleLinkedList.RemoveNode(node);
				// each time when access the node, we move it to the top
				doubleLinkedList.AddNodeToTop(node);
				return node.Value;

			}
		}

		public void AddValueToCacheByKey(int key, object value, out string? rtrnMessage)
		{
			// If key/value pair already exist..
			if (map.ContainsKey(key))
			{
				object soonToBeOldValue = GetCachedValueByKey(key);

				lock (cacheLock)
                {
					// ..just need to update value and move it to the top
					LRUNode node = map[key];
					doubleLinkedList.RemoveNode(node);
					node.Value = value;
					doubleLinkedList.AddNodeToTop(node);

				}

				rtrnMessage = $"Value {soonToBeOldValue.ToString} has been removed from the cache";

				//return $"Value {soonToBeOldValue.ToString} has been removed from the cache";
			}
			else
			{
				// if cache is full.. 
				if (count == capacity)
				{
					
					lock (cacheLock)
                    {
						// ..then remove the least recently used node
						LRUNode lru = doubleLinkedList.RemoveLRUNode();
						map.Remove(lru.Key);
						count--;

						rtrnMessage = "";

					}
					
				}

                lock (cacheLock)
                {
					// Otherwise, add a new node
					LRUNode node = new LRUNode(key, value);
					doubleLinkedList.AddNodeToTop(node);
					map[key] = node;
					count++;

					rtrnMessage = "";

				}
				
			}

		}

	}
}
