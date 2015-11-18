namespace Rant.Engine
{
    internal sealed class Limit
    {
	    private readonly int _max;
	    private int _value;

	    public int Maximum => _max;

	    public Limit(int max)
	    {
		    _max = max;
		    _value = 0;
	    }

	    public bool Accumulate(int value)
	    {
		    return _max > 0 && (_value += value) > _max;
	    }
    }
}