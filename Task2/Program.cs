namespace Task2
{
    internal class Program
    {
        static object locker = new object();
        static string filePath = @"C:\Users\User\source\repos\SystemProgramming\homework1204\Task2\task2.txt";
        static void SentenceCount(string path)
        {
            lock (locker)
            {
                if (path != null)
                {
                    string text = File.ReadAllText(path);
                    int count = 0;
                    bool sentenceEnded = false;

                    for (int i = 0; i < text.Length; i++)
                    {
                        char c = text[i];

                        if (c == '.' || c == '!' || c == '?')
                        {
                            sentenceEnded = true;
                        }
                        else if (sentenceEnded && c == ' ')
                        {
                            int j = i + 1;
                            while (j < text.Length && text[j] == ' ')
                            {
                                j++;
                            }

                            if (j < text.Length && Char.IsLetter(text[j]))
                            {
                                count++;
                                sentenceEnded = false;
                            }
                        }
                    }

                    if (sentenceEnded || text.Length > 0)
                    {
                        count++;
                    }

                    Console.WriteLine($"Count of sentences in txt file: {count}");
                }
            }
        }

        static void Replace()
        {
            lock (locker)
            {
                if (filePath != null)
                {
                    string text = File.ReadAllText(filePath);
                    string result = "";

                    foreach (char c in text)
                    {
                        if (c == '!')
                            result += '#';
                        else
                            result += c;
                    }

                    File.WriteAllText(filePath, result);
                    Console.WriteLine("All '!' replaced with '#'");

                }
            }
        }
        
        static void Main(string[] args)
        {
            Thread t1 = new Thread(() => SentenceCount(filePath));
            Thread t2 = new Thread(Replace);

            t1.Start();
            t2.Start();
        }
    }
}
