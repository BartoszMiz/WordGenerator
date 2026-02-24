using System.Text;

namespace WordGenerator;

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
