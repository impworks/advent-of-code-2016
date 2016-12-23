<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>JsonFormatting = Newtonsoft.Json.Formatting</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	var solver = new Solver();
	solver.Solve().Dump();
}

private class Solver
{
	public Solver()
	{
		_knownStates = new HashSet<long>();
		_directions = new[]
		{
			new Point(-1, 0),
			new Point(1, 0),
			new Point(0, -1),
			new Point(0, 1)
		};
		
		LoadField();
	}
	
	public const int FIELD_WIDTH = 30;
	public const int FIELD_HEIGHT = 35;
	
	private readonly HashSet<long> _knownStates;
	private State _initialState;
	private char[,] _field;
	private readonly Point[] _directions;

	public int Solve()
	{
		var queue = new Queue<State>();
		queue.Enqueue(_initialState);

		// breadth-first search
		while (queue.Count != 0)
		{
			var state = queue.Dequeue();
			var childStates = GetChildStates(state);

			foreach (var childState in childStates)
			{
				// visited state
				var hash = childState.GetHashCode();
				if(_knownStates.Contains(hash))
					continue;
					
				_knownStates.Add(hash);
				
				// solution found
				if(childState.TargetX == 0 && childState.TargetY == 0)
					return childState.Steps;
					
				queue.Enqueue(childState);
			}
		}
		
		throw new Exception("Solution not found");
	}
	
	private List<State> GetChildStates(State state)
	{
		var childStates = new List<State>(4);

		// only move the empty node around
		foreach (var dir in _directions)
		{
			var x = state.HoleX;
			var y = state.HoleY;
			var toX = (short)(state.HoleX + dir.X);
			var toY = (short)(state.HoleY + dir.Y);

			// out of bounds
			if(toX < 0 || toY < 0 || toX >= FIELD_WIDTH || toY >= FIELD_HEIGHT)
				continue;
			
			// heavy node, unmovable
			if(_field[toY, toX] == '#')
				continue;
										
			var newState = Swap(state, x, y, toX, toY);
			
			// prefer moving the target
			if(toX == state.TargetX && toY == state.TargetY)
				childStates.Insert(0, newState);
			else
				childStates.Add(newState);
		}
		
		return childStates;
	}
	
	private State Swap(State original, short x, short y, short x2, short y2)
	{
		var isMovingTarget = x2 == original.TargetX && y2 == original.TargetY;
		
		var newState = new State
		{
			Steps = original.Steps + 1,
			TargetX = isMovingTarget ? x : original.TargetX,
			TargetY = isMovingTarget ? y : original.TargetY,
			HoleX = x2,
			HoleY = y2
		};
		
		return newState;
	}
	
	private void LoadField()
	{
		var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
		var lines = File.ReadAllLines(path).Skip(2);

		_field = new char[FIELD_HEIGHT, FIELD_WIDTH];
		_initialState = new State { TargetX = FIELD_WIDTH - 1 };

		foreach (var line in lines)
		{
			var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var nameParts = parts[0].Split('-');

			var x = short.Parse(nameParts[1].Substring(1));
			var y = short.Parse(nameParts[2].Substring(1));
			var capacity = short.Parse(parts[1].Replace("T", ""));
			var used = short.Parse(parts[2].Replace("T", ""));

			// mega node
			if(capacity > 100)
				_field[y, x] = '#';

			// empty node
			else if (used == 0)
			{
				_field[y, x] = 'e';
				_initialState.HoleX = x;
				_initialState.HoleY = y;
			}
				
			else
				_field[y, x] = '.';
		}
	}
}

class State
{
	public short TargetX;
	public short TargetY;
	public short HoleX;
	public short HoleY;
	public int Steps;
	
	public override int GetHashCode()
	{
		return $"{HoleX}/{HoleY}/{TargetX}/{TargetY}".GetHashCode();
	}
	
	public override string ToString()
	{
		return $"Target: [{TargetX},{TargetY}] Hole: [{HoleX},{HoleY}]";
	}
}