using System.Text;

var sourcePath = "names.txt";
var lines = File.ReadAllLines(sourcePath).Select(x => x.ToLower() + '\n').ToArray();

Console.Write("Finding unique characters... ");
var characters = new HashSet<char>();
foreach (var line in lines)
{
	foreach (var c in line)
	{
		characters.Add(c);
	}
}

characters.Add('\n');

var numToChar = characters.ToArray();
var charToNum = new Dictionary<char, int>();

for (int i = 0; i < numToChar.Length; i++)
{
	charToNum.Add(numToChar[i], i);
}

Console.WriteLine("done!");
Console.Write("Counting characters... ");

var probabilities = new double[characters.Count, characters.Count];

foreach (var line in lines)
{
	for (int i = 1; i < line.Length; i++)
	{
		var previous = line[i - 1];
		var current = line[i];
		probabilities[charToNum[previous], charToNum[current]] += 1;
	}
}

Console.WriteLine("done!");
Console.Write("Normalizing the probabilities matrix... ");

for (int i = 0; i < characters.Count; i++)
{
	var total = 0.0;
	for (int j = 0; j < characters.Count; j++)
	{
		total += probabilities[i, j];
	}

	for (int j = 0; j < characters.Count; j++)
	{
		probabilities[i, j] /= total;
	}
}
Console.WriteLine("done!");
Console.Write("Generating... ");

var sb = new StringBuilder();
var charIndex = 0;
do
{
	charIndex = Random.Shared.Next(0, characters.Count);
}
while (numToChar[charIndex] == '\n');

sb.Append(numToChar[charIndex]);

while (true)
{
	var x = Random.Shared.NextDouble();
	var acc = 0.0;
	var i = 0;
	for (; i < characters.Count; i++)
	{
		var prob = probabilities[charIndex, i];
		if (acc + prob >= x)
			break;

		acc += prob;
	}

	var character = numToChar[i];
	if (character == '\n')
		break;

	sb.Append(character);
	charIndex = i;
}

Console.WriteLine("done!");
Console.WriteLine($"Generated word: {sb.ToString()}");

// Console.Write("  ");
// for (int i = 0; i < characters.Count; i++)
// {
// 	Console.Write($"{numToChar[i]} ");
// }
// Console.WriteLine();
//
// for (int i = 0; i < characters.Count; i++)
// {
// 	Console.Write($"{numToChar[i]} ");
// 	for (int j = 0; j < characters.Count; j++)
// 	{
// 		Console.Write($"{probabilities[i, j]} ");
// 	}
// 	Console.WriteLine();
// }

