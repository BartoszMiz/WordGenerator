using System.Text;

var sourcePath = "names.txt";
const int N = 20;

SingleCharMarkovChainWordGenerator.Setup(sourcePath);
DoubleCharMarkovChainWordGenerator.Setup(sourcePath);

Console.WriteLine("========== Single Character Markov Chain words: ==========");
for (int i = 0; i < N; i++)
{
	Console.WriteLine(SingleCharMarkovChainWordGenerator.GenerateWord());
}

Console.WriteLine();
Console.WriteLine("========== Double Character Markov Chain words: ==========");
for (int i = 0; i < N; i++)
{
	Console.WriteLine(DoubleCharMarkovChainWordGenerator.GenerateWord());
}

public static class DoubleCharMarkovChainWordGenerator
{
	public static bool IsInitialized { get; private set; } = false;
	private static int _charactersCount;
	private static char[] _numToChar = null!;
	private static Dictionary<char, int> _charToNum = null!;
	private static double[,,] _probabilities = null!;

	public static void Setup(string filePath)
	{
		var lines = File.ReadAllLines(filePath)
			.Select(x => x.ToLower() + '\n')
			.Where(x => x.Length >= 3)
			.ToArray();
		
		var characters = new HashSet<char>();
		foreach (var line in  lines)
		{
			foreach (var c in line)
			{
				characters.Add(c);
			}
		}

		_charactersCount = characters.Count;
		_numToChar = characters.ToArray();
		_charToNum = new Dictionary<char, int>();

		for (int i = 0; i < _charactersCount; i++)
		{
			_charToNum.Add(_numToChar[i], i);
		}

		_probabilities = new double[_charactersCount, _charactersCount, _charactersCount];

		foreach (var line in lines)
		{
			for (int i = 2; i < line.Length; i++)
			{
				var prevPrev = line[i - 2];
				var prev = line[i - 1];
				var current = line[i];
				_probabilities[_charToNum[prevPrev], _charToNum[prev], _charToNum[current]] += 1;
			}
		}

		for (int i = 0; i < _charactersCount; i++)
		{
			for (int j = 0; j < _charactersCount; j++)
			{
				var total = Enumerable.Range(0, _charactersCount).Select(k => _probabilities[i, j, k]).Sum();
				for (int k = 0; k < _charactersCount; k++)
				{
					_probabilities[i, j, k] /= total;
				}
			}
		}
		
		if (!SingleCharMarkovChainWordGenerator.IsInitialized)
			SingleCharMarkovChainWordGenerator.Setup(filePath);
		
		IsInitialized = true;
	}

	public static string GenerateWord()
	{
		var sb = new StringBuilder();
		
		// TODO: clean up this hack
		var word = "";
		do
		{
			word = SingleCharMarkovChainWordGenerator.GenerateWord();
		} while (word.Length < 2);

		var firstCharIdx = _charToNum[word[0]];
		var secondCharIdx = _charToNum[word[1]];
		
		sb.Append(_numToChar[firstCharIdx]);
		sb.Append(_numToChar[secondCharIdx]);

		while (true)
		{
			var x = Random.Shared.NextDouble();
			var acc = 0.0;
			var i = 0;
			for (; i < _charactersCount; i++)
			{
				var prob = _probabilities[firstCharIdx, secondCharIdx, i];
				if (acc + prob >= x)
					break;

				acc += prob;
			}

			var character = _numToChar[i];
			if (character == '\n')
				return sb.ToString();
			
			sb.Append(character);
			firstCharIdx = secondCharIdx;
			secondCharIdx = i;
		}
	}
}

public static class SingleCharMarkovChainWordGenerator
{
	public static bool IsInitialized { get; private set; } = false;
	
	private static int _charactersCount;
	private static char[] _numToChar = null!;
	private static Dictionary<char, int> _charToNum = null!;
	private static double[,] _probabilities = null!;
	
	public static void Setup(string filePath)
	{
		var lines = File.ReadAllLines(filePath).Select(x => x.ToLower() + '\n').ToArray();
		var characters = new HashSet<char>();
		foreach (var line in lines)
		{
			foreach (var c in line)
			{
				characters.Add(c);
			}
		}

		_charactersCount = characters.Count;
		_numToChar = characters.ToArray();
		_charToNum = new Dictionary<char, int>();

		for (int i = 0; i < _charactersCount; i++)
		{
			_charToNum.Add(_numToChar[i], i);
		}
		
		_probabilities = new double[characters.Count, characters.Count];

		foreach (var line in lines)
		{
			for (int i = 1; i < line.Length; i++)
			{
				var previous = line[i - 1];
				var current = line[i];
				_probabilities[_charToNum[previous], _charToNum[current]] += 1;
			}
		}
		
		for (int i = 0; i < _charactersCount; i++)
		{
			var total = 0.0;
			for (int j = 0; j < _charactersCount; j++)
			{
				total += _probabilities[i, j];
			}

			for (int j = 0; j < _charactersCount; j++)
			{
				_probabilities[i, j] /= total;
			}
		}

		IsInitialized = true;
	}

	public static string GenerateWord()
	{
		var sb = new StringBuilder();
		var charIndex = 0;
		do
		{
			charIndex = Random.Shared.Next(0, _charactersCount);
		}
		while (_numToChar[charIndex] == '\n');

		sb.Append(_numToChar[charIndex]);

		while (true)
		{
			var x = Random.Shared.NextDouble();
			var acc = 0.0;
			var i = 0;
			for (; i < _charactersCount; i++)
			{
				var prob = _probabilities[charIndex, i];
				if (acc + prob >= x)
					break;

				acc += prob;
			}

			var character = _numToChar[i];
			if (character == '\n')
				return sb.ToString();

			sb.Append(character);
			charIndex = i;
		}
	}
}
