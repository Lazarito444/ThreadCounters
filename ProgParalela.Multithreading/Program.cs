namespace ProgParalela.Multithreading;

public static class Program
{
    private static readonly Dictionary<int, Counter> Counters = new Dictionary<int, Counter>();
    private static int _nextCounterId = 1;
    private static bool _running = true;

    public static void Main(string[] args)
    {
        Console.WriteLine("Aplicación de Contadores Multihilo");

        while (_running)
        {
            ShowMenu();
            Console.Write("Seleccione una opción: ");
            string option = Console.ReadLine() ?? "4";
                
            switch (option)
            {
                case "1":
                    StartCounter();
                    break;
                case "2":
                    StopCounter();
                    break;
                case "3":
                    ShowCountersStatus();
                    break;
                case "4":
                    ExitProgram();
                    break;
                default:
                    Console.WriteLine("Opción no válida.");
                    break;
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("\nMenú:");
        Console.WriteLine("1. Iniciar un nuevo contador");
        Console.WriteLine("2. Detener un contador");
        Console.WriteLine("3. Mostrar estado de los contadores");
        Console.WriteLine("4. Salir del programa");
    }

    private static void StartCounter()
    {
        Console.Write("Ingrese el intervalo en milisegundos para el contador: ");
        if (int.TryParse(Console.ReadLine(), out int interval))
        {
            int counterId = _nextCounterId++;
            Counter newCounter = new Counter(counterId, interval);
            Counters[counterId] = newCounter;

            Thread counterThread = new Thread(newCounter.Start)
            {
                IsBackground = true
            };
            newCounter.Thread = counterThread;
            counterThread.Start();

            Console.WriteLine($"Contador {counterId} iniciado con un intervalo de {interval} ms.");
        }
        else
        {
            Console.WriteLine("Entrada no válida. Debe ingresar un número entero.");
        }
    }

    static void StopCounter()
    {
        Console.Write("Ingrese el ID del contador a detener: ");
        if (int.TryParse(Console.ReadLine(), out int counterId) && Counters.ContainsKey(counterId))
        {
            Counters[counterId].Stop();
            Counters.Remove(counterId);
            Console.WriteLine($"Contador {counterId} detenido.");
        }
        else
        {
            Console.WriteLine("ID de contador no válido o no encontrado.");
        }
    }

    static void ShowCountersStatus()
    {
        Console.WriteLine("\nEstado actual de los contadores:");
        foreach (var counter in Counters.Values)
        {
            Console.WriteLine($"Contador {counter.Id} - Valor: {counter.Value} (Intervalo: {counter.Interval} ms)");
        }
    }

    static void ExitProgram()
    {
        Console.WriteLine("Saliendo del programa. Deteniendo todos los contadores...");
        _running = false;

        foreach (var counter in Counters.Values)
        {
            counter.Stop();
        }

        Console.WriteLine("Todos los contadores detenidos. Programa finalizado.");
    }
}

class Counter
{
    public int Id { get; private set; }
    public int Interval { get; private set; }
    public int Value { get; private set; } = 0;
    public Thread Thread { get; set; }
    private bool _active = true;

    public Counter(int id, int interval)
    {
        Id = id;
        Interval = interval;
    }

    public void Start()
    {
        while (_active)
        {
            Value++;
            Thread.Sleep(Interval);
        }
    }

    public void Stop()
    {
        _active = false;
    }
}