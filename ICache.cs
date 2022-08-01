namespace Cache
{
    public interface ICache
    {
        // return the value if key exists in cache, or -1 if not. 
        int GetCachedValueByKey(int key);

        // add a new value to the cache
        void AddValueToCacheByKey(int key, int value);
    }
}