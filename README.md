# PrioQ

**PrioQ** is a modular, extensible, **in‑memory priority queue system** built on .NET. It supports multiple queue algorithms (heap, bucket, bitmask) via a **decorator architecture** that seamlessly adds advanced features such as logging, **locking** (for concurrent producers/consumers), lazy deletion, and **analytics** (for usage reporting). Thanks to its clean, layered design and powerful concurrency model, PrioQ remains both **testable** and **scalable**, letting you easily integrate new behaviors or algorithms for production needs.

---

## Key Features

1. **Multiple Algorithms**  
   - **HeapPriorityQueue** – Traditional binary heap (O(log n) for enqueue/dequeue).  
   - **BucketPriorityQueue** – Bucket-based approach, ideal for limited priority ranges.  
   - **BitmaskPriorityQueue** – Single-level bitmask (up to 64 priorities), O(1) operations.  
   - **DoubleLevelBitmaskPriorityQueue** – For larger priority ranges (up to 256), also O(1) operations.

2. **Decorator Architecture**  
   - **Logging** – Logs each enqueue/dequeue for auditing.  
   - **Locking** – Ensures thread safety so **multiple concurrent** producers/consumers can reliably send commands to the queue.  
   - **Lazy Deletion** – Marks items before discarding, optimizing certain workloads.  
   - **Analytics** – Records usage data for **generating reports** on average time in queue per priority level, throughput, etc.

3. **Configurable at Runtime**  
   - **JSON Configuration** – A `config.json` or custom `IConfigProvider` can specify unbounded vs. bounded priorities, chosen algorithm, and toggles for decorators (logging, locking, analytics, etc.).

4. **Clean Architecture**  
   - **Domain** – Core entities (`PriorityQueueItem`, `QueueConfig`) and business logic contracts.  
   - **Application** – Use cases (like `EnqueueCommandUseCase`, `DequeueCommandUseCase`, `InitializeQueueUseCase`) orchestrate interactions between domain and infrastructure.  
   - **Infrastructure** – Implements queue algorithms, repository, analytics collectors, decorator factories.  
   - **Presentation** – An ASP.NET Core UI (with a Setup Wizard & Dashboard), **API** endpoints for remote usage, and an optional CLI for local administration.

5. **Testability & Scalability**  
   - **Unit Tests** – Thorough coverage of each queue implementation, decorator, and use case (using Moq/fakes).  
   - **Integration Tests** – Spin up the in‑memory server for end‑to‑end scenarios.  
   - **Stress/Load Tests** – Confirms performance under heavy concurrency, ensuring your queue remains a production-ready solution.

---

## API & Concurrency

- **Public API Endpoints** – Expose `/api/priorityqueue/enqueue`, `/api/priorityqueue/dequeue`, etc., enabling external services to **send commands** concurrently without stepping on each other’s operations.  
- **Locking Decorator** – Ensures thread safety when multiple producers/consumers are accessing the queue simultaneously. This is critical for production where concurrency is high.

---

## Analytics & Reporting

- **Analytics Decorator** – Tracks how long commands of different priorities spend in the queue, total items processed, and other metrics.  
- **Report Generation** – Use cases like `AnalyticsReportUseCase` aggregate and expose this data, helping you optimize priority distribution and track performance over time.

---

## Why C# Instead of Python?

- **High Performance**: C# on .NET is compiled to IL (and JIT-optimized), providing near-native speed. For a **priority queue** that can become a bottleneck under heavy loads, performance is critical.  
- **Robust Concurrency Model**: The .NET runtime offers efficient threads, synchronization primitives, and the Task Parallel Library. This supports PrioQ’s concurrency features more smoothly than the GIL (Global Interpreter Lock) in Python.  
- **Strongly Typed & Compile-Time Safety**: Large-scale systems benefit from compile-time checks and tooling that reduce runtime errors.  
- **Ecosystem & Tooling**: Visual Studio, JetBrains Rider, and the .NET ecosystem provide comprehensive debugging, performance analysis, and testing tools. This speeds up development and ensures reliability.

---

## Notable Classes for Contributors

You’ll find the **most important** queue algorithm classes in: /Infrastructure/PriorityQueues/


This folder includes `HeapPriorityQueue`, `BucketPriorityQueue`, `BitmaskPriorityQueue`, `DoubleLevelBitmaskPriorityQueue`, etc. If you plan to extend or create a new algorithm, start here.

---

## Final Notes

**PrioQ** aims to be a **flexible, production-ready** queueing solution for .NET:

- **Interchangeable algorithms** for different priority/performance requirements.  
- **Decorator-based** approach for logging, concurrency locking, analytics, etc.  
- **Config-driven** (JSON or custom providers) for easy customization.  
- **Clean architecture** that’s easy to understand, test, and maintain.  
- **C#** chosen over Python for better performance, concurrency handling, and strong tooling.

**Thank you for using PrioQ**! We hope it streamlines your priority queue management and integrates smoothly into your .NET applications. For questions or advanced customization, please open an [Issue](#) or contact the maintainers.

**Happy coding—and happy queueing!**
