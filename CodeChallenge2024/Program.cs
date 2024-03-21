string INPUT_DIRECTORY = "../../../input/";
string[] INPUT_FILES = { "a.txt" };
string[] lines = File.ReadAllLines(INPUT_DIRECTORY + INPUT_FILES[0]);

foreach (string line in lines)
    Console.WriteLine(line);