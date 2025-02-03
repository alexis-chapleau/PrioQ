# PrioQ

**PrioQ** is a modular, extensible, in‑memory priority queue system built on .NET. It supports multiple queue algorithms (heap, bucket, bitmask) via a decorator architecture that seamlessly adds advanced features such as logging, locking (for concurrent producers/consumers), lazy deletion, and analytics (for usage reporting). Thanks to its clean, layered design and powerful concurrency model, PrioQ remains both testable and scalable, letting you easily integrate new behaviors or algorithms for production needs.

---

## Key Features

### Multiple Algorithms

- **HeapPriorityQueue** – Traditional binary heap (O(log n) for enqueue/dequeue).
- **BucketPriorityQueue** – Bucket-based approach, ideal for limited priority ranges.
- **BitmaskPriorityQueue** – Single-level bitmask (up to 64 priorities), O(1) operations.
- **DoubleLevelBitmaskPriorityQueue** – For larger priority ranges (up to 256), also O(1) operations.
- **ConcurrentDoubleLevelBitmaskPriorityQueue** - Uses Concurrent data structures to optimize concurrency
- **ConcurrentBitmaskPriorityQueue** - Uses Concurrent data structures to optimize concurrency
- **ConcurrentBucketPriorityQueue** - Uses Concurrent data structures to optimize concurrency

### Decorator Architecture

- **Logging** – Logs each enqueue/dequeue for auditing.
- **Locking** – Ensures thread safety for concurrent producers/consumers.
- **Lazy Deletion** – Marks items before discarding, optimizing workloads.
- **Analytics** – Tracks usage data to generate reports on queue performance.

### Configurable at Runtime

- **JSON Configuration** – Customize settings via `config.json` or a custom `IConfigProvider`.
- **Toggle Features** – Enable/disable logging, locking, analytics, and more dynamically.

### Clean Architecture

- **Domain** – Core entities (`PriorityQueueItem`, `QueueConfig`) and business logic.
- **Application** – Use cases like `EnqueueCommandUseCase`, `DequeueCommandUseCase`.
- **Infrastructure** – Implements queue algorithms, repository, analytics collectors.
- **Presentation** – ASP.NET Core UI, API endpoints, and an optional CLI.

### Testability & Scalability

- **Unit Tests** – Comprehensive coverage using Moq and fakes.
- **Integration Tests** – Simulates real-world interactions.
- **Load Testing** – Validates performance under heavy concurrency.

---

## Web Application Interface

A new **Web Application Interface** has been added for interactive management and reporting of your priority queue. With this interface, you can:

- **Initialize a Queue**: Create or replace the queue via an intuitive Setup Wizard.
- **Enqueue & Dequeue Commands**: Manage commands through interactive forms.
- **Generate Dynamic Analysis Reports**: View graphical reports of queue performance.

> **Note:** This web interface is for local development and testing only. The queue is a singleton—replacing it will lose current items (item recovery is not implemented yet).

---

## Public API Endpoints

PrioQ exposes public API endpoints for external integrations:

- `POST /api/priorityqueue/initialize` – Initialize or replace the queue.
- `POST /api/priorityqueue/enqueue` – Enqueue a command.
- `POST /api/priorityqueue/dequeue` – Dequeue a command.

These endpoints allow programmatic access to all core queue operations.

---

## Running the Web Application

To run the web interface locally, execute the following command at the project root or in the `/PrioQ/Presentation` folder:

```sh
 dotnet run
```

Then open your browser and navigate to `http://localhost:56155/`.

---

## API & Concurrency

- **Public API Endpoints**: `/api/priorityqueue/enqueue`, `/api/priorityqueue/dequeue`, etc., allow concurrent external access.
- **Locking Decorator**: Ensures thread safety when multiple producers/consumers interact with the queue.

---

## Analytics & Reporting

- **Analytics Decorator**: Tracks how long commands spend in the queue, throughput, and other metrics.
- **Report Generation**: Use cases like `AnalyticsReportUseCase` aggregate and expose data for performance tracking.

---

## Why C# Instead of Python?

### **Performance**
- .NET is JIT-optimized, providing near-native speed.
- Ideal for high-performance queueing.

### **Robust Concurrency**
- .NET's threading model and Task Parallel Library support efficient multi-threading.

### **Strong Typing & Compile-Time Safety**
- Reduces runtime errors and simplifies system maintenance.

### **Ecosystem & Tooling**
- Visual Studio, JetBrains Rider, and the .NET ecosystem offer excellent debugging and performance analysis tools.

---

## Notable Classes for Contributors

Key queue algorithm classes are in `/Infrastructure/PriorityQueues/`:

- `HeapPriorityQueue`
- `BucketPriorityQueue`
- `BitmaskPriorityQueue`
- `DoubleLevelBitmaskPriorityQueue`
- `ConcurrentDoubleLevelBitmaskPriorityQueue`
- `ConcurrentBitmaskPriorityQueue`
- `ConcurrentBucketPriorityQueue`

If you plan to extend or create a new algorithm, start here.

---

## Final Notes

### **PrioQ is a flexible, production-ready queueing solution for .NET**:

✔ **Interchangeable Algorithms** – Tailor the solution to your priority and performance needs.
✔ **Decorator-Based Architecture** – Easily add logging, concurrency locking, and analytics.
✔ **Config-Driven Customization** – Use JSON or custom providers to fine-tune behavior.
✔ **Clean, Testable Architecture** – Simplifies maintenance and extension.
✔ **High Performance & Concurrency** – C# provides superior speed and robust tooling.
✔ **Web Application Interface** – Manage queues interactively and generate dynamic reports.
✔ **Public API** – External systems can integrate with PrioQ seamlessly.

---

## Running Locally

Run the following command to start the web interface for development and testing:

```sh
 dotnet run
```

> **Reminder:** The queue is a singleton—replacing it will lose current items (item recovery is not implemented yet).

---

## Contact & Support

For questions, feature requests, or contributions, please [open an issue](https://www.youtube.com/watch?v=dQw4w9WgXcQ&ab_channel=RickAstley) or contact the maintainers.
