<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	var origin = new Point(1, 1);
	var target = new Point(31, 39);
	var func = GetPathSource(1362);
	var finder = new Pathfinder(origin, target, func, 50);

	finder.FindShortestPath();
	finder.VisitedPointsCount.Dump();
}

int CountBits(int i)
{
	i = i - ((i >> 1) & 0x55555555);
	i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
	return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
}

Func<Point, bool> GetPathSource(int number)
{
	return pt =>
	{
		var hash = pt.X * pt.X + 3 * pt.X + 2 * pt.X * pt.Y + pt.Y + pt.Y * pt.Y + number;
		var bitsCount = CountBits(hash);
		return bitsCount % 2 == 0;
	};
}

class Pathfinder
{
	public Pathfinder(Point origin, Point target, Func<Point, bool> pointSource, int? maxSteps = null)
	{
		_origin = origin;
		_target = target;
		_pointSource = pointSource;
		_maxSteps = maxSteps;

		// do not allow diagonals
		_directions = new[]
		{
			new Point(0, 1),
			new Point(0, -1),
			new Point(1, 0),
			new Point(-1, 0)
		};

		_visitedPoints = new HashSet<long>();
	}

	private readonly Func<Point, bool> _pointSource;
	private readonly Point _origin;
	private readonly Point _target;
	private readonly HashSet<long> _visitedPoints;
	private readonly Point[] _directions;
	private readonly int? _maxSteps;

	public int VisitedPointsCount => _visitedPoints.Count;

	public List<Point> FindShortestPath()
	{
		var start = new Location
		{
			Point = _origin,
			Path = null
		};

		// breadth-first search
		var queue = new Queue<Location>();
		queue.Enqueue(start);

		while (queue.Count > 0)
		{
			var current = queue.Dequeue();

			foreach (var dir in _directions)
			{
				var newLocation = Explore(current, dir);
				if (newLocation == null)
					continue;

				if (newLocation.Point == _target)
					return newLocation.Path.ToList();

				if (_maxSteps == null || newLocation.Path.Length < _maxSteps)
					queue.Enqueue(newLocation);
			}
		}

		return null;
	}

	private Location Explore(Location current, Point direction)
	{
		// check for bounds
		var newPoint = new Point(current.Point.X + direction.X, current.Point.Y + direction.Y);
		if (newPoint.X < 0 || newPoint.Y < 0)
			return null;

		// check for wall
		var state = _pointSource(newPoint);
		if (!state)
			return null;

		// check for visited point
		var pointHash = newPoint.X * 1000L + newPoint.Y;
		if (_visitedPoints.Contains(pointHash))
			return null;

		_visitedPoints.Add(pointHash);

		// looks fine
		return new Location
		{
			Point = newPoint,
			Path = new LinkedItem<Point>(newPoint, current.Path)
		};
	}
}

class LinkedItem<T>
{
	public LinkedItem(T value, LinkedItem<T> next = null)
	{
		Value = value;
		NextItem = next;
		Length = next == null ? 1 : next.Length + 1;
	}

	public int Length;
	public T Value;
	public LinkedItem<T> NextItem;

	public List<T> ToList()
	{
		var result = new List<T>();
		var curr = this;
		while (curr != null)
		{
			result.Add(curr.Value);
			curr = curr.NextItem;
		}
		return result;
	}
}

class Location
{
	public Point Point;
	public LinkedItem<Point> Path;
}
