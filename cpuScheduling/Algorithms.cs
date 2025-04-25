public static class Algorithms {
    public static void FCFS(List<Process> processes) {
        processes = processes.OrderBy(p => p.ArrivalTime).ToList();
        int currentTime = 0;

        foreach (var process in processes) {
            if (currentTime < process.ArrivalTime)
                currentTime = process.ArrivalTime;

            process.StartTime = currentTime;
            process.CompletionTime = currentTime + process.BurstTime;
            process.TurnaroundTime = process.CompletionTime - process.ArrivalTime;
            process.WaitingTime = process.TurnaroundTime - process.BurstTime;

            currentTime = process.CompletionTime;
        }

        PrintResults("FCFS", processes);
        
    }

    public static void SJF(List<Process> processes) {
        //sort processes by arrival time
        for (int i = 0; i < processes.Count - 1; i++) {
            for (int j = i + 1; j < processes.Count; j++) {
                if (processes[j].ArrivalTime < processes[i].ArrivalTime) {
                    Process temp = processes[i];
                    processes[i] = processes[j];
                    processes[j] = temp;
                }
            }
        }

        List<Process> remainingProcesses = new List<Process>(processes);
        int currentTime = 0;
        
        while (remainingProcesses.Count > 0) {
            List<Process> arrived = new List<Process>();
            for (int i = 0; i < remainingProcesses.Count; i++)
            {
                if (remainingProcesses[i].ArrivalTime <= currentTime) {
                    arrived.Add(remainingProcesses[i]);
                }
            }
            
            if (arrived.Count == 0) {
                //find the earliest arrival time among remaining processes
                int minArrival = int.MaxValue;
                for (int i = 0; i < remainingProcesses.Count; i++) {
                    if (remainingProcesses[i].ArrivalTime < minArrival) {
                        minArrival = remainingProcesses[i].ArrivalTime;
                    }
                }
                currentTime = minArrival;
                continue;
            }
            
            //find process with shortest burst time
            Process shortest = arrived[0];
            for (int i = 1; i < arrived.Count; i++) {
                if (arrived[i].BurstTime < shortest.BurstTime) {
                    shortest = arrived[i];
                }
            }
            
            shortest.StartTime = currentTime;
            shortest.CompletionTime = currentTime + shortest.BurstTime;
            shortest.TurnaroundTime = shortest.CompletionTime - shortest.ArrivalTime;
            shortest.WaitingTime = shortest.TurnaroundTime - shortest.BurstTime;
            
            currentTime = shortest.CompletionTime;
            remainingProcesses.Remove(shortest);
        }

        PrintResults("SJF", processes);
    }

    public static void RoundRobin(List<Process> processes, int quantum) {
        Queue<Process> queue = new Queue<Process>(processes.OrderBy(p => p.ArrivalTime));
        int currentTime = 0;

        while (queue.Count > 0) {
            Process process = queue.Dequeue();

            if (process.StartTime == -1)
                process.StartTime = currentTime;

            int timeSlice = Math.Min(process.RemainingTime, quantum);
            currentTime += timeSlice;
            process.RemainingTime -= timeSlice;

            if (process.RemainingTime > 0) {
                queue.Enqueue(process);
            }
            else {
                process.CompletionTime = currentTime;
                process.TurnaroundTime = process.CompletionTime - process.ArrivalTime;
                process.WaitingTime = process.TurnaroundTime - process.BurstTime;
            }
        }

        PrintResults("Round Robin", processes);
    }

    
    public static void PriorityScheduling(List<Process> processes) {
        int currentTime = 0;
        int completed = 0;
        int n = processes.Count;
        
        foreach (var p in processes)
            p.RemainingTime = p.BurstTime;

        while (completed < n) {
            Process selectedProcess = null;
            int highestPriority = int.MaxValue;
            int earliestArrival = int.MaxValue;
            
            for (int i = 0; i < n; i++) {
                Process p = processes[i];
                if (p.ArrivalTime <= currentTime && p.RemainingTime > 0) {
                    if (p.Priority < highestPriority || 
                        (p.Priority == highestPriority && p.ArrivalTime < earliestArrival)) {
                        highestPriority = p.Priority;
                        earliestArrival = p.ArrivalTime;
                        selectedProcess = p;
                    }
                }
            }

            if (selectedProcess == null) {
                currentTime++;
                continue;
            }

            //non-preemptive
            selectedProcess.StartTime = currentTime;
            currentTime += selectedProcess.BurstTime;
            selectedProcess.CompletionTime = currentTime;
            selectedProcess.TurnaroundTime = selectedProcess.CompletionTime - selectedProcess.ArrivalTime;
            selectedProcess.WaitingTime = selectedProcess.TurnaroundTime - selectedProcess.BurstTime;
            selectedProcess.RemainingTime = 0;
            completed++;
        }

        PrintResults("Priority Scheduling", processes);
    }


    public static void ShortestRemainingTimeFirst(List<Process> processes) {
        int currentTime = 0;
        int completed = 0;
        int n = processes.Count;

        foreach (var p in processes)
            p.RemainingTime = p.BurstTime;

        while (completed < n) {
            Process shortest = null;
            int minRemaining = int.MaxValue;

            //find process with shortest remaining time
            foreach (var p in processes) {
                if (p.ArrivalTime <= currentTime && 
                    p.RemainingTime > 0 && 
                    p.RemainingTime < minRemaining)
                {
                    shortest = p;
                    minRemaining = p.RemainingTime;
                }
            }

            if (shortest == null) {
                currentTime++;
                continue;
            }

            if (shortest.StartTime == -1)
                shortest.StartTime = currentTime;

            currentTime++;
            shortest.RemainingTime--;

            if (shortest.RemainingTime == 0) {
                completed++;
                shortest.CompletionTime = currentTime;
                shortest.TurnaroundTime = shortest.CompletionTime - shortest.ArrivalTime;
                shortest.WaitingTime = shortest.TurnaroundTime - shortest.BurstTime;
            }
        }

        PrintResults("Shortest Remaining Time First (SRTF)", processes);
    }


    public static void HighestResponseRatioNext(List<Process> processes) {
        int currentTime = 0;
        int completed = 0;
        int n = processes.Count;
        
        while (completed < n) {

            //find all arrived, incomplete processes
            List<Process> candidates = new List<Process>();
            for (int i = 0; i < processes.Count; i++) {
                Process p = processes[i];
                if (p.ArrivalTime <= currentTime && p.RemainingTime > 0) {
                    candidates.Add(p);
                }
            }
            
            if (candidates.Count == 0) {
                currentTime++;
                continue;
            }
            
            //find process with highest response ratio
            Process selected = null;
            double maxRatio = -1;
            
            for (int i = 0; i < candidates.Count; i++) {
                Process p = candidates[i];
                int waiting = currentTime - p.ArrivalTime;
                double ratio = (double)(waiting + p.BurstTime) / p.BurstTime;
                
                if (ratio > maxRatio) {
                    maxRatio = ratio;
                    selected = p;
                }
            }
            
            if (selected.StartTime == -1)
                selected.StartTime = currentTime;
                
            currentTime += selected.BurstTime;
            selected.RemainingTime = 0;
            selected.CompletionTime = currentTime;
            selected.TurnaroundTime = selected.CompletionTime - selected.ArrivalTime;
            selected.WaitingTime = selected.TurnaroundTime - selected.BurstTime;
            completed++;
        }

        PrintResults("Highest Response Ratio Next (HRRN)", processes);
    }

 

    public static void PrintResults(string algorithmName, List<Process> processes) {
        processes = processes.OrderBy(p => p.ID).ToList();

        Console.WriteLine($"\n=== {algorithmName} ===");
        Console.WriteLine("Process | Arrival | Burst | Start | Completion | Turnaround | Waiting");
        Console.WriteLine("------------------------------------------------------------------");

        int totalBurstTime = 0; 
        int lastCompletionTime = 0;
        int totalWaitingTime = 0;
        int totalTurnaroundTime = 0;

        foreach (var process in processes) {
            Console.WriteLine($"{process.ID,-7} | {process.ArrivalTime,-7} | {process.BurstTime,-5} | {process.StartTime,-5} | {process.CompletionTime,-10} | {process.TurnaroundTime,-10} | {process.WaitingTime,-7}");

            totalBurstTime += process.BurstTime;
            totalWaitingTime += process.WaitingTime;
            totalTurnaroundTime += process.TurnaroundTime;
            lastCompletionTime = Math.Max(lastCompletionTime, process.CompletionTime);
        }

        double averageWaitingTime = (double)totalWaitingTime / processes.Count;
        double averageTurnaroundTime = (double)totalTurnaroundTime / processes.Count;

        if (lastCompletionTime == 0) {
            Console.WriteLine("Average Waiting Time: 0.00");
            Console.WriteLine("Average Turnaround Time: 0.00");
            Console.WriteLine("CPU Utilization: 0.00%");
            Console.WriteLine("Throughput: 0.00 processes per second");
            
            return;
        }

        Console.WriteLine($"Average Waiting Time: {averageWaitingTime:F2}");
        Console.WriteLine($"Average Turnaround Time: {averageTurnaroundTime:F2}");

        double cpuUtilization = ((double)totalBurstTime / lastCompletionTime) * 100;
        double throughput = (double)processes.Count / lastCompletionTime;
        
        Console.WriteLine($"CPU Utilization: {cpuUtilization:F2}%");
        Console.WriteLine($"Throughput: {throughput:F2} processes per second");
        Console.WriteLine();

    }



}