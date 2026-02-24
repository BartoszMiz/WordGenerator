using WordGenerator;

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
