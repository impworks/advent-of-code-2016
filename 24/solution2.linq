<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
    var finder = new Pathfinder();
    var result = finder.FindShortestPath();
    result.Dump();
}

class Pathfinder
{
    public Pathfinder()
    {
        LoadField();

        _directions = new[]
        {
            new Point(1, 0),
            new Point(-1, 0),
            new Point(0, 1),
            new Point(0, -1)
        };
    }

    private Dictionary<int, Point> _points;
    private bool[,] _field;
    private Point[] _directions;
    private Size _size;

    public int FindShortestPath()
    {
        var distances = GetAllDistances();
        var best = int.MaxValue;
        TraversePath(distances, "0", 0, ref best);
        return best;
    }

    private void LoadField()
    {
        var path = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "input.txt");
        var lines = File.ReadAllLines(path);

        _field = new bool[lines.Length, lines[0].Length];
        _points = new Dictionary<int, Point>();
        _size = new Size(lines[0].Length, lines.Length);

        for (var y = 0; y < lines.Length; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var ch = line[x];
                _field[y, x] = ch == '#';

                if (ch != '.' && ch != '#')
                {
                    var num = int.Parse(ch.ToString());
                    _points[num] = new Point(x, y);
                }
            }
        }
    }

    private int GetDistanceBetweenPoints(Point a, Point b)
    {
        var knownPoints = new HashSet<Point>();
        var queue = new Queue<Step>();
        queue.Enqueue(new Step { Point = a });

        // breadth-first search
        while (queue.Any())
        {
            var step = queue.Dequeue();
            foreach (var dir in _directions)
            {
                var x = step.Point.X + dir.X;
                var y = step.Point.Y + dir.Y;

                if (x < 0 || x >= _size.Width || y < 0 || y >= _size.Height || _field[y, x])
                    continue;

                if (x == b.X && y == b.Y)
                    return step.Length + 1;

                var point = new Point(x, y);
                if (!knownPoints.Add(point))
                    continue;

                queue.Enqueue(new Step { Point = point, Length = step.Length + 1 });
            }
        }

        throw new ArgumentException("Path not found");
    }

    private Dictionary<Tuple<Point, Point>, int> GetAllDistances()
    {
        // cache distances between every two points
        var result = new Dictionary<Tuple<Point, Point>, int>();

        for (var idx = 0; idx < _points.Count - 1; idx++)
        {
            for (var idx2 = idx + 1; idx2 < _points.Count; idx2++)
            {
                var pt1 = _points[idx];
                var pt2 = _points[idx2];
                var distance = GetDistanceBetweenPoints(pt1, pt2);

                result[Tuple.Create(pt1, pt2)] = distance;
                result[Tuple.Create(pt2, pt1)] = distance;
            }
        }

        return result;
    }

    private void TraversePath(Dictionary<Tuple<Point, Point>, int> lookup, string visited, int distance, ref int best)
    {
        var lastPoint = _points[visited[visited.Length - 1] - '0'];
        
        // end of path
        if (visited.Length == _points.Count)
        {
            // go back to point 0
            distance += lookup[Tuple.Create(lastPoint, _points[0])];
            
            if (distance < best)
                best = distance;

            return;
        }

        foreach (var point in _points)
        {
            var pointSymbol = (char)('0' + point.Key);
            if (visited.Contains(pointSymbol))
                continue;

            var stepDistance = lookup[Tuple.Create(lastPoint, point.Value)];
            TraversePath(lookup, visited + pointSymbol, distance + stepDistance, ref best);
        }
    }
}

class Step
{
    public Point Point;
    public int Length;
}

// Define other methods and classes here
