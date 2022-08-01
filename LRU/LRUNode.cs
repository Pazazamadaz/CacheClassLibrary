namespace Cache.LRU
{
	public class LRUNode
	{
		public int Key { get; set; }
		public object Value { get; set; }
		public LRUNode Previous { get; set; }
		public LRUNode Next { get; set; }
		public LRUNode() { }
		public LRUNode(int k, object v)
		{
			this.Key = k;
			this.Value = v;
		}
	}
}
