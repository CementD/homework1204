namespace Task1
{
    internal class Program
    {
        static int currentMinute = 270;
        static int waitingPeople = 0;
        static int busNumber = 1;
        static int day = 1;
        static bool simulationRunning = true;
        static object locker = new object();
        static Random rand = new Random();
        static string currentEvent = "";
        static int lastCheckedMinute = -1;

        static string FormatTime()
        {
            int h = currentMinute / 60;
            int m = currentMinute % 60;
            return $"{h:D2}:{m:D2}";
        }

        static void Main(string[] args)
        {
            Thread uiThread = new (() =>
            {
                while (simulationRunning)
                {
                    lock (locker)
                    {
                        Console.Clear();
                        Console.WriteLine("Bus simulator. Press 'E' to exit...");
                        Console.WriteLine($"Day: {day}  Current time: {FormatTime()}");
                        Console.WriteLine("------------------------------------------");

                        if (!string.IsNullOrEmpty(currentEvent))
                            Console.WriteLine(currentEvent);
                    }

                    Thread.Sleep(1000);
                }
            });

            Thread timeThread = new (() =>
            {
                while (simulationRunning)
                {
                    Thread.Sleep(1000);
                    lock (locker)
                    {
                        currentMinute++;

                        if (currentMinute >= 1440)
                        {
                            if (waitingPeople > 25)
                                currentEvent = $"{FormatTime()} There were {waitingPeople - 25} people more at the bus stop. They went home by foot.";

                            Thread.Sleep(5000);

                            currentMinute = 270;
                            waitingPeople = 0;
                            busNumber = 1;
                            day++;
                            currentEvent = $"{FormatTime()} New day starts now.";
                        }
                    }
                }
            });

            Thread passengerThread = new (() =>
            {
                while (simulationRunning)
                {
                    Thread.Sleep(2000);
                    if (currentMinute >= 270 && currentMinute < 1440)
                    {
                        int newPeople = rand.Next(0, 4);
                        lock (locker)
                        {
                            if (newPeople > 0)
                            {
                                waitingPeople += newPeople;
                                currentEvent = $"{FormatTime()} {newPeople} people came. Total: {waitingPeople}";
                            }
                        }
                    }
                }
            });

            Thread busThread = new (() =>
            {
                while (simulationRunning)
                {
                    Thread.Sleep(500);
                    lock (locker)
                    {
                        if (currentMinute >= 300 && currentMinute < 1440 &&
                            currentMinute != lastCheckedMinute &&
                            (currentMinute - 300) % 30 == 0)
                        {
                            int taking = 0;
                            if (waitingPeople <= 25)
                                taking = waitingPeople;
                            else
                                taking = 25;
                            waitingPeople -= taking;
                            currentEvent = $"{FormatTime()} Bus Nr.{busNumber} arrived. Took {taking} passangers. Left: {waitingPeople}";
                            busNumber++;
                            if (busNumber > 10) busNumber = 1;

                            lastCheckedMinute = currentMinute;
                        }
                    }
                }
            });
            Thread inputThread = new (() =>
            {
                while (simulationRunning)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.E)
                    {
                        lock (locker)
                        {
                            Console.Clear();
                            Console.WriteLine("Exit from simulation...");
                            simulationRunning = false;
                            Console.ReadKey();
                        }
                    }
                }
            });

            uiThread.Start();
            timeThread.Start();
            passengerThread.Start();
            busThread.Start();
            inputThread.Start();
        }
    }
}
