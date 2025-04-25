public class Process {
    public int ID { get; set; }
    public int ArrivalTime { get; set; }
    public int BurstTime { get; set; }
    public int Priority { get; set; } 
    public int RemainingTime { get; set; }
    public int StartTime { get; set; } = -1; 
    public int CompletionTime { get; set; }
    public int WaitingTime { get; set; }
    public int TurnaroundTime { get; set; }


    public Process(int id, int arrivalTime, int burstTime) {
        ID = id;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        RemainingTime = burstTime;
        CompletionTime = 0;
    }
    public Process(int id, int arrivalTime, int burstTime, int priority) {
        ID = id;
        ArrivalTime = arrivalTime;
        BurstTime = burstTime;
        RemainingTime = burstTime;
        Priority = priority;
        CompletionTime = 0;
    }

}
