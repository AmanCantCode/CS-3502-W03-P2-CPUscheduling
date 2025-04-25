# CS-3502-W03-P2-CPUscheduling

This is an Opertaing Systems project where different CPU Scheduling algorithms are implemented and can be tested to measure their performance.

    In order to run this project, you need:
        1. .NET SDK 9.0
        2. An IDE 
        3. [OPTIONAL] RStudio (with R installed) to see the graphs created by the test program.

    To run this program, open the terminal and follow:
        1. Change the directory to cpuScheduling 
        2. Use the command: dotnet build
        3. Use the command: dotnet run

    To test this program, open the terminal and follow:
        1. Change the directory to cpuTest 
        2. Use the command: dotnet build
        3. Use the command: dotnet test

    To see the graphs with RStudio:
        1. Copy the R program provided below into RStudio
        2. Set the working directory to where the .csv file is located in this project
            (cpuTest/bin/Debug/net9.0)
        3. Highlight the entire program and click run





------------------------------------------------------------------------------------------------------------------------------------

                install.packages("ggplot2")
                install.packages("gridExtra")
                library(ggplot2)
                library(gridExtra)

                #Set the working directory to csv file's location before executing the program

                data = read.csv("TestResults.csv", check.names = TRUE)
                data$Algorithm = factor(data$Algorithm, 
                                        levels = c("FCFS", "SJF", "Round Robin", 
                                                    "Priority", "SRTF", "HRRN"))

                create_ggplot = function(data, metric, ylab, title) {
                ggplot(
                    data, 
                    aes(x = Algorithm, y = .data[[metric]], fill = Test)) +
                    geom_bar(stat = "identity", position = position_dodge()) +
                    labs(title = title, y = ylab, x = "Algorithm") +
                    theme_minimal() +
                    theme(axis.text.x = element_text(angle = 45, hjust = 1)
                        )
                }

                AWT <- create_ggplot(data, "AWT", "Average Waiting Time", "AWT by Algorithm and Test")
                ATT <- create_ggplot(data, "ATT", "Average Turnaround Time", "ATT by Algorithm and Test")
                cpuUtil <- create_ggplot(data, "CPU.Utilization....", "CPU Utilization (%)", "CPU Utilization by Algorithm and Test")
                Throughput <- create_ggplot(data, "Throughput", "Throughput", "Throughput by Algorithm and Test")

                #See each graph
                AWT
                ATT
                cpuUtil
                Throughput

                #See the graphs together
                grid.arrange(AWT, ATT, cpuUtil, Throughput, ncol = 2)



