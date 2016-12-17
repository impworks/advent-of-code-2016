<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

void Main()
{
    var state = new State
    {
        HashableString = "vkjiggvb",
        Position = new Point(0, 0)
    };
    
    var solver = new Solver(state);
    var length = solver.FindLongestPathLength();
    
    length.Dump();
}

class Solver
{
    public Solver(State initialState)
    {
        _initialState = initialState;

        _targetPosition = new Point(3, 3);
        
        // order of step in this array corresponds to the order of a char in the string
        _steps = new[]
        {
            new Step { Symbol = 'U', Direction = new Point( 0, -1) },
            new Step { Symbol = 'D', Direction = new Point( 0,  1) },
            new Step { Symbol = 'L', Direction = new Point(-1,  0) },
            new Step { Symbol = 'R', Direction = new Point( 1,  0) }
        };
    }
    
    private readonly State _initialState;
    private readonly Point _targetPosition;
    private readonly Step[] _steps;
    
    public int FindLongestPathLength()
    {
        int maxPathLength = 0;
        
        var queue = new Queue<State>();
        queue.Enqueue(_initialState);

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            var childStates = GetChildStates(state);

            foreach (var childState in childStates)
            {
                // check if the state is valid
                if (childState == null)
                    continue;

                // check if the state is a solution
                if (childState.Position == _targetPosition)
                {
                    if(childState.PathLength > maxPathLength)
                        maxPathLength = childState.PathLength;
                    
                    continue;
                }

                queue.Enqueue(childState);
            }
        }

        return maxPathLength;
    }

    private State[] GetChildStates(State state)
    {
        var newStates = new State[4];
        var hash = GetHash(state.HashableString);
    
        for (var i = 0; i < _steps.Length; i++)
        {
            var ch = hash[i];
            var step = _steps[i];
            
            // check if the step is within bounds
            var newPosition = new Point(state.Position.X + step.Direction.X, state.Position.Y + step.Direction.Y);
            if(newPosition.X < 0 || newPosition.Y < 0 || newPosition.X > 3 || newPosition.Y > 3)
                continue;
            
            // check if the door is closed
            if(ch < 'b')
                continue;

            // add a new state
            newStates[i] = new State
            {
                PathLength = state.PathLength + 1,
                HashableString = state.HashableString + step.Symbol,
                Position = newPosition
            };
        }
        
        return newStates;
    }
        
    private string GetHash(string input)
    {
        using (var md5 = MD5.Create())
        {
            var inBytes = Encoding.ASCII.GetBytes(input);
            var outBytes = md5.ComputeHash(inBytes);
            return outBytes[0].ToString("x2") + outBytes[1].ToString("x2");
        }
    }
}

class Step
{
    public char Symbol;
    public Point Direction;
}

class State
{
    public int PathLength;
    public string HashableString;
    public Point Position;
}
