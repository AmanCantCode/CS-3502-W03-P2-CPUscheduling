class Program {
       
        static List<Process> CloneProcesses(List<Process> original) {
            var clone = new List<Process>();
            foreach (var p in original) {
                clone.Add(new Process(p.ID, p.ArrivalTime, p.BurstTime, p.Priority));
            }
            return clone;
        }

        static void Main() {
            List<Process> processes1 = new List<Process> {

                new Process(1, 4, 5),
                new Process(2, 6, 4),
                new Process(3, 0, 3),
                new Process(4, 6, 2),
                new Process(5, 5, 4)
            };

            List<Process> processes2 = new List<Process> {

                new Process(1, 0, 5, 3),
                new Process(2, 1, 3, 2),
                new Process(3, 2, 8, 1),
                new Process(4, 3, 6, 6),
                new Process(5, 5, 4, 2)
            };

            while (true) {
                Console.WriteLine("CPU Scheduling Simulator: Choose an algorithm (1-6)");
                Console.WriteLine("1. First Come First Served (FCFS)");
                Console.WriteLine("2. Shortest Job First (SJF)");
                Console.WriteLine("3. Round Robin (RR)");
                Console.WriteLine("4. Priority Scheduling");
                Console.WriteLine("5. Shortest Remaining Time First (SRTF)");
                Console.WriteLine("6. Highest Response Ratio Next (HRRN)");
                Console.WriteLine("7. Exit");

                int choice = int.Parse(Console.ReadLine());

                switch (choice) {
                    case 1:
                        Algorithms.FCFS(CloneProcesses(processes1));
                        break;
                    case 2:
                        Algorithms.SJF(CloneProcesses(processes1));
                        break;
                    case 3:
                        Console.Write("Enter time quantum: ");
                        int quantum = int.Parse(Console.ReadLine());
                        Algorithms.RoundRobin(CloneProcesses(processes1), quantum);
                        break;
                    case 4:
                        Algorithms.PriorityScheduling(CloneProcesses(processes2));
                        break;
                    case 5:
                        Algorithms.ShortestRemainingTimeFirst(CloneProcesses(processes1));
                        break;
                    case 6:
                        Algorithms.HighestResponseRatioNext(CloneProcesses(processes1));
                        break;
                    case 7:
                        Console.WriteLine("Exiting program...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }
    }
