<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
</Query>

void Main()
{
    var initialState = new State
    {
        Elevator = 0,
        Items = new int[State.ELEMENTS_COUNT * 2]
    };
    
    initialState.Items[State.THULIUM] = 0;
    initialState.Items[State.THULIUM + State.CHIP] = 0;
    initialState.Items[State.PLUTONIUM] = 0;
    initialState.Items[State.PLUTONIUM + State.CHIP] = 1;
    initialState.Items[State.STRONTIUM] = 0;
    initialState.Items[State.STRONTIUM + State.CHIP] = 1;
    initialState.Items[State.PROMETHIUM] = 2;
    initialState.Items[State.PROMETHIUM + State.CHIP] = 2;
    initialState.Items[State.RUTHENIUM] = 2;
    initialState.Items[State.RUTHENIUM + State.CHIP] = 2;

    var finder = new SolutionFinder(initialState);
    var solution = finder.Solve();
    
    if(solution == null)
        throw new ArgumentException("No solution found");
    
    solution.Count.Dump();
}

public class SolutionFinder
{
	public SolutionFinder(State initial)
	{
		_initialState = initial;
		_knownStates = new HashSet<long>();
	}
	
	private readonly State _initialState;
	private readonly HashSet<long> _knownStates;
	
	public List<State> Solve()
	{
		var queue = new Queue<State>();
		queue.Enqueue(_initialState);

		// breadth-first search
		while (queue.Count > 0)
		{
			var state = queue.Dequeue();
			var nextStates = GetNextStates(state);

			foreach (var nextState in nextStates)
			{
				if(!IsStateValid(nextState))
                    continue;
                    
                if(IsStateComplete(nextState))
                    return FlattenHistory(nextState);
                    
                queue.Enqueue(nextState);
			}
		}
		
		// no solution found
		return null;
	}
	
	private List<State> GetNextStates(State state)
	{
		var result = new List<State>();

        var floorItems = GetItemsOnFloor(state, state.Elevator);
        if(floorItems.Count == 0)
            return result;
            
        var floorItemPairs = GetPairs(floorItems);
        
        // try to move anything up
        if (state.Elevator < State.FLOORS_COUNT - 1)
        {
            // prefer moving two items
            if(floorItemPairs != null)
                foreach (var pair in floorItemPairs)
                    result.Add(GetNextState(state, 1, pair.Item1, pair.Item2));
            
            foreach(var item in floorItems)
                result.Add(GetNextState(state, 1, item));
        }

        // try to move anything down
        if (state.Elevator > 0)
        {
            // prefer moving one item
            foreach (var item in floorItems)
                result.Add(GetNextState(state, -1, item));
                
            if(floorItemPairs != null)
                foreach (var pair in floorItemPairs)
                    result.Add(GetNextState(state, -1, pair.Item1, pair.Item2));
        }

        return result;
	}
	
	private bool IsStateValid(State state)
	{
		// check: state already processed
		var hash = state.GetHash();
		if(_knownStates.Contains(hash))
			return false;
			
		_knownStates.Add(hash);

		// check: no chips are irradiated							  
		for (var floor = 0; floor < State.FLOORS_COUNT; floor++)
		{
            // check if the floor has a generator
            var hasGenerator = false;
            for (var idx = 0; idx < State.ELEMENTS_COUNT; idx++)
            {
                if (state.Items[idx * 2] == floor)
                {
                    hasGenerator = true;
                    break;
                }
            }
            
            if(!hasGenerator)
                continue;

            // check if there is a chip not matched to its generator in the floor
            for (var idx = 0; idx < State.ELEMENTS_COUNT; idx++)
            {
                if(state.Items[idx * 2 + 1] == floor && state.Items[idx * 2] != floor)
                    return false;
            }
		}
		
		return true;
	}
    
    private bool IsStateComplete(State state)
    {
        return state.Items.All(x => x == State.FLOORS_COUNT - 1);        
    }
    
    private List<int> GetItemsOnFloor(State state, int floor)
    {
        // returns the positions of elements that are on current floor.
        var result = new List<int>();
        
        for (var idx = 0; idx < State.ELEMENTS_COUNT * 2; idx++)
        {
            if(state.Items[idx] == floor)
                result.Add(idx);
        }
        
        return result;
    }
    
    private List<Tuple<int, int>> GetPairs(List<int> items)
    {
        if(items.Count < 2)
            return null;
            
        var result = new List<Tuple<int, int>>();
        
        for(var idx1 = 0; idx1 < items.Count - 1; idx1++)
            for(var idx2 = idx1 + 1; idx2 < items.Count; idx2++)
                result.Add(Tuple.Create(items[idx1], items[idx2]));
                
        return result;
    }
    
    private State GetNextState(State state, int moveDirection, int item1, int? item2 = null)
    {
        var nextState = state.Clone();
        
        nextState.Elevator += moveDirection;
        
        nextState.Items[item1] += moveDirection;
        if(item2 != null)
            nextState.Items[item2.Value] += moveDirection;
            
        return nextState;
    }
    
    private List<State> FlattenHistory(State state)
    {
        var result = new List<State>();

        var curr = state;
        while (curr != null)
        {
            result.Add(curr);
            curr = curr.PreviousState;
        }
        
        // exclude initial state
        if(result.Count > 0)
            result.RemoveAt(result.Count - 1);
        
        result.Reverse();
        return result;
    }
}

public class State
{
	// number of chemical elements involved
	// a chip and a generator for each
	public const int ELEMENTS_COUNT = 5;
	public const int FLOORS_COUNT = 4;
    
    public const int THULIUM = 0;
    public const int PLUTONIUM = 2;
    public const int STRONTIUM = 4;
    public const int PROMETHIUM = 6;
    public const int RUTHENIUM = 8;
    public const int CHIP = 1;
	
	// current elevator floor (0-3)
	public int Elevator;	
	public int[] Items;
    
    public State PreviousState;
	
	public State Clone()
	{
		return new State
		{
            PreviousState = this,
			Elevator = Elevator,
			Items = Items.ToArray()
		};
	}

	public long GetHash()
	{
		// all chip+gen combinations are actually interchangable!
		// order pairs by their value, thus greatly decreasing the number of states to check

		var pairs = new int[ELEMENTS_COUNT];

		for (var i = 0; i < ELEMENTS_COUNT; i++)
			pairs[i] = Items[i*2] + Items[i*2 + 1] * 10;
		
		var result = 0;
		
		foreach (var pair in pairs.OrderBy(x => x))
			result = result * 100 + pair;
		
		result = result * 10 + Elevator;
		
		return result;
	}
}