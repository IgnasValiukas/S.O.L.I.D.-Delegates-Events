using System;
using System.Collections.Generic;

// Defining a Task class to represent a task with properties Id, Description, and IsCompleted
public class Task
{
    public int Id { get; set; } // Property to store task ID
    public string Description { get; set; } // Property to store task description
    public bool IsCompleted { get; set; } // Property to store task completion status
}

// Defining a custom EventArgs class TaskEventArgs to pass task-related information
public class TaskEventArgs : EventArgs
{
    public Task Task { get; } // Property to hold a task object

    // Constructor to initialize a TaskEventArgs object with a task
    public TaskEventArgs(Task task)
    {
        Task = task;
    }
}

// Defining a custom delegate TaskActionDelegate to represent an action to be performed on a task
public delegate void TaskActionDelegate<T>(T task);

// Defining a TaskManager class to manage tasks
public class TaskManager
{
    // Defining events for task creation, update, and deletion
    public event EventHandler<TaskEventArgs> TaskCreated;
    public event EventHandler<TaskEventArgs> TaskUpdated;
    public event EventHandler<TaskEventArgs> TaskDeleted;

    private List<Task> tasks = new List<Task>(); // List to store tasks
    private int nextId = 1; // Variable to generate unique IDs for tasks

    // Method to add a new task with the given description
    public void AddTask(string description)
    {
        Task newTask = new Task { Id = nextId++, Description = description, IsCompleted = false };
        tasks.Add(newTask);
        OnTaskCreated(new TaskEventArgs(newTask)); // Trigger TaskCreated event
    }

    // Method to update an existing task with the given ID, description, and completion status
    public void UpdateTask(int id, string newDescription, bool newIsCompleted)
    {
        Task taskToUpdate = tasks.Find(t => t.Id == id); // Find task by ID
        if (taskToUpdate != null)
        {
            taskToUpdate.Description = newDescription; // Update task description
            taskToUpdate.IsCompleted = newIsCompleted; // Update task completion status
            OnTaskUpdated(new TaskEventArgs(taskToUpdate)); // Trigger TaskUpdated event
        }
    }

    // Method to delete a task with the given ID
    public void DeleteTask(int id)
    {
        Task taskToDelete = tasks.Find(t => t.Id == id); // Find task by ID
        if (taskToDelete != null)
        {
            tasks.Remove(taskToDelete); // Remove task from the list
            OnTaskDeleted(new TaskEventArgs(taskToDelete)); // Trigger TaskDeleted event
        }
    }

    // Method to get all tasks
    public List<Task> GetAllTasks()
    {
        return tasks; // Return the list of tasks
    }

    // Method to perform a given action on each task
    public void PerformActionOnTask(Action<Task> action)
    {
        foreach (var task in tasks)
        {
            action(task); // Perform the action on each task
        }
    }

    // Method to get descriptions of all tasks using a selector function
    public List<string> GetTaskDescriptions(Func<Task, string> selector)
    {
        List<string> descriptions = new List<string>(); // List to store task descriptions
        foreach (var task in tasks)
        {
            descriptions.Add(selector(task)); // Add the description of each task to the list
        }
        return descriptions; // Return the list of task descriptions
    }

    // Method to trigger the TaskCreated event
    protected virtual void OnTaskCreated(TaskEventArgs e)
    {
        TaskCreated?.Invoke(this, e); // Invoke the TaskCreated event
    }

    // Method to trigger the TaskUpdated event
    protected virtual void OnTaskUpdated(TaskEventArgs e)
    {
        TaskUpdated?.Invoke(this, e); // Invoke the TaskUpdated event
    }

    // Method to trigger the TaskDeleted event
    protected virtual void OnTaskDeleted(TaskEventArgs e)
    {
        TaskDeleted?.Invoke(this, e); // Invoke the TaskDeleted event
    }
}

class Program
{
    // Main method, entry point of the program
    static void Main(string[] args)
    {
        TaskManager taskManager = new TaskManager(); // Creating an instance of TaskManager
        taskManager.TaskCreated += TaskCreatedHandler; // Subscribing to TaskCreated event
        taskManager.TaskUpdated += TaskUpdatedHandler; // Subscribing to TaskUpdated event
        taskManager.TaskDeleted += TaskDeletedHandler; // Subscribing to TaskDeleted event

        bool exit = false; // Variable to control program exit
        while (!exit)
        {
            Console.WriteLine("Enter 'A' to add a task, 'U' to update a task, 'D' to delete a task, 'V' to view tasks, or 'E' to exit:");
            string input = Console.ReadLine()?.ToUpper(); // Reading user input and converting to uppercase

            switch (input) // Handling user input
            {
                case "A": // Adding a new task
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Enter task description:");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    string description = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    taskManager.AddTask(description); // Adding a task with the provided description
                    Console.ResetColor();
                    break;
                case "U": // Updating an existing task
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter task ID to update:");
                    Console.ResetColor();
                    int idToUpdate;
                    if (int.TryParse(Console.ReadLine(), out idToUpdate)) // Parsing user input as integer
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Enter new task description:");
                        Console.ResetColor();
                        string newDescription = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Enter new task completion status (true/false):");
                        Console.ResetColor();
                        bool newIsCompleted;
                        if (bool.TryParse(Console.ReadLine(), out newIsCompleted)) // Parsing user input as boolean
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            taskManager.UpdateTask(idToUpdate, newDescription, newIsCompleted); // Updating the task
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid completion status.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid task ID.");
                        Console.ResetColor();
                    }
                    break;
                case "D": // Deleting a task
                    Console.WriteLine("Enter task ID to delete:");
                    int idToDelete;
                    if (int.TryParse(Console.ReadLine(), out idToDelete)) // Parsing user input as integer
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        taskManager.DeleteTask(idToDelete); // Deleting the task
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Invalid task ID.");
                    }
                    break;
                case "V": // Viewing all tasks
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    ViewTasks(taskManager.GetAllTasks()); // Displaying all tasks
                    Console.ResetColor();
                    break;
                case "E": // Exiting the program
                    exit = true; // Setting exit flag to true
                    break;
                default: // Handling invalid input
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }

    // Event handler for TaskCreated event
    static void TaskCreatedHandler(object sender, TaskEventArgs e)
    {
        Console.WriteLine($"Task created: {e.Task.Description}"); // Displaying task creation message
    }

    // Event handler for TaskUpdated event
    static void TaskUpdatedHandler(object sender, TaskEventArgs e)
    {
        Console.WriteLine($"Task updated: {e.Task.Description}, Completed: {e.Task.IsCompleted}"); // Displaying task update message
    }

    // Event handler for TaskDeleted event
    static void TaskDeletedHandler(object sender, TaskEventArgs e)
    {
        Console.WriteLine($"Task deleted: {e.Task.Description}"); // Displaying task deletion message
    }

    // Method to view all tasks
    static void ViewTasks(List<Task> tasks)
    {
        Console.WriteLine("Tasks:"); // Displaying heading for tasks
        foreach (var task in tasks) // Iterating through each task
        {
            Console.WriteLine($"ID: {task.Id}, Description: {task.Description}, Completed: {task.IsCompleted}"); // Displaying task details
        }
    }
}
