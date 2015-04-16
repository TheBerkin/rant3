using Rant.Engine.Syntax;

namespace Rant.Engine.Constructs
{
    internal class BlockAttribs
    {
	    private int _reps;

	    public int Repetitons
	    {
		    get { return _reps; }
		    set
		    {
			    _reps = value;
			    RepEach = false;
		    }
	    }
		public bool RepEach { get; set; }
        public RantAction Separator { get; set; }
        public RantAction Before { get; set; }
        public RantAction After { get; set; }
        public Synchronizer Sync { get; set; }
		public int Chance { get; set; }

        public BlockAttribs()
        {
	        SetDefaults();
        }

	    public void SetDefaults()
	    {
			Repetitons = 1;
		    RepEach = false;
			Chance = 100;
			Separator = null;
			Before = null;
			After = null;
			Sync = null;
		}

		/// <summary>
		/// Calculates the index of the next block item to execute.
		/// </summary>
		/// <param name="blockItemCount">The number of items in the block.</param>
		/// <param name="rng">The random number generator to use with index calculation.</param>
		/// <returns></returns>
		public int NextIndex(int blockItemCount, RNG rng)
	    {
			if (Chance < 100 && rng.Next(0, 100) > Chance)
			{
				return -1; // Skip the block
			}

			// Use synchronizer if available
			if (Sync != null)
			{
				return Sync.NextItem(blockItemCount);
			}

			return rng.Next(blockItemCount);
		}
    }
}