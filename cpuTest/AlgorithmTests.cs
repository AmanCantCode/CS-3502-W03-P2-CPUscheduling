public class AlgorithmTests : IDisposable
{
    private const string SummaryFile = "TestResults.csv";
    private static bool _csvInitialized = false; // Changed to static

    public void Dispose() {
        // Cleanup if needed
    }

    private void InitializeCsv() {
        if (!_csvInitialized) {
            File.WriteAllText(SummaryFile, "Test,Algorithm,AWT,ATT,CPU Utilization (%),Throughput\n");
            _csvInitialized = true;
        }
    }

    public (double AWT, double ATT, double CpuUtilization, double Throughput) GetMetrics(List<Process> processes) {
        double totalWaitingTime = processes.Sum(p => p.WaitingTime);
        double totalTurnaroundTime = processes.Sum(p => p.TurnaroundTime);
        double totalBurstTime = processes.Sum(p => p.BurstTime);
        double makespan = processes.Max(p => p.CompletionTime);

        double awt = totalWaitingTime / processes.Count;
        double atat = totalTurnaroundTime / processes.Count;
        double cpuUtil = (totalBurstTime / makespan) * 100;
        double throughput = processes.Count / makespan;

        return (awt, atat, cpuUtil, throughput);
    }

    private void WriteResultsToCsv(string testName, List<Process> processes, string algorithm) {
        var metrics = GetMetrics(processes);
        File.AppendAllText(SummaryFile, $"{testName},{algorithm},{metrics.AWT:F2},{metrics.ATT:F2},{metrics.CpuUtilization:F2},{metrics.Throughput:F2}\n");
    }

    private void RunAllAlgorithms(List<Process> processes, string testName) {
        Console.WriteLine($"\n{testName} Test");
        InitializeCsv(); // Only writes header if not already initialized

        var fcfs = Clone(processes);
        Algorithms.FCFS(fcfs);
        WriteResultsToCsv(testName, fcfs, "FCFS");

        var sjf = Clone(processes);
        Algorithms.SJF(sjf);
        WriteResultsToCsv(testName, sjf, "SJF");

        var quantum = testName.Contains("Large") ? 4 : 
                     testName.Contains("Extreme") ? 5 : 2;
        var rr = Clone(processes);
        Algorithms.RoundRobin(rr, quantum);
        WriteResultsToCsv(testName, rr, "Round Robin");

        var priority = Clone(processes);
        Algorithms.PriorityScheduling(priority);
        WriteResultsToCsv(testName, priority, "Priority");

        var srtf = Clone(processes);
        Algorithms.ShortestRemainingTimeFirst(srtf);
        WriteResultsToCsv(testName, srtf, "SRTF");

        var hrrn = Clone(processes);
        Algorithms.HighestResponseRatioNext(hrrn);
        WriteResultsToCsv(testName, hrrn, "HRRN");
    }

    [Fact]
    public void BasicFunctionalityTest() => RunAllAlgorithms(
        new List<Process>
        {
            new Process(1, 0, 5, 2),
            new Process(2, 2, 3, 1),
            new Process(3, 4, 1, 3)
        }, 
        "Basic Functionality");

    [Fact]
    public void LargeScaleRandomTest() => RunAllAlgorithms(
        Enumerable.Range(1, 20).Select(i => 
            new Process(i, 
                new Random().Next(0, 10),
                new Random().Next(1, 20),
                new Random().Next(1, 10))
        ).ToList(),
        "Large Scale Random");

    [Fact]
    public void EdgeCase_IdenticalArrivalAndBurst() => RunAllAlgorithms(
        Enumerable.Range(1, 10).Select(i => 
            new Process(i, 0, 5, i)
        ).ToList(),
        "Identical Arrival Burst");

    [Fact]
    public void EdgeCase_ExtremeBurstMix() => RunAllAlgorithms(
        Enumerable.Range(1, 10).Select(i => 
            new Process(i, i % 3, (i % 2 == 0 ? 100 : 1), i)
        ).ToList(),
        "Extreme Burst Mix");

    [Fact]
    public void EdgeCase_SkewedPriorities() => RunAllAlgorithms(
        Enumerable.Range(1, 10).Select(i => 
            new Process(i, i % 4, new Random().Next(1, 20), i < 5 ? 1 : 10)
        ).ToList(),
        "Skewed Priorities");

    private List<Process> Clone(List<Process> original) =>
        original.Select(p => new Process(p.ID, p.ArrivalTime, p.BurstTime, p.Priority)).ToList();
}
