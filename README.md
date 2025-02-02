# PrioQ

**PrioQ** is a modular, extensible, in‑memory priority queue system built on .NET. It supports multiple queue algorithms (e.g., heap, bucket, bitmask) via a **decorator architecture** that seamlessly adds advanced features such as logging, locking, lazy deletion, and analytics. Thanks to its clean, layered design, PrioQ remains both **testable** and **extensible**, letting you easily add new behaviors or algorithms to meet production needs.

---

## Key Features

1. **Multiple Algorithms**  
   - **HeapPriorityQueue** – Traditional binary heap (O(log n) for enqueue/dequeue).  
   - **BucketPriorityQueue** – Bucket-based approach, suitable for limited priority ranges.  
   - **BitmaskPriorityQueue** – Single-level bitmask (up to 64 priorities), offering O(1) operations.  
   - **DoubleLevelBitmaskPriorityQueue** – For larger priority ranges (up to 256), also O(1).

2. **Decorator Architecture**  
   - **Logging** – Logs each enqueue/dequeue.  
   - **Locking** – Provides thread safety via locking.  
   - **Lazy Deletion** – Marks items before discarding them.  
   - **Analytics** – Records analytics data for reporting.

3. **Configurable at Runtime**  
   - **JSON Configuration** – A `config.json` or custom `IConfigProvider` can specify unbounded vs. bounded priorities, chosen algorithm, and decorators to apply.

4. **Clean Architecture**  
   - **Domain** – Core entities (`PriorityQueueItem`, `QueueConfig`) and business logic contracts.  
   - **Application** – Use cases (like `EnqueueCommandUseCase`, `DequeueCommandUseCase`, `InitializeQueueUseCase`) orchestrate interactions between domain and infrastructure.  
   - **Infrastructure** – Implements queue algorithms, repository, analytics collectors, and decorator factories.  
   - **Presentation** – An ASP.NET Core UI (with a Setup Wizard & Dashboard) and optional CLI for administration.

5. **Testability**  
   - **Unit Tests** – Thorough tests for each use case, queue implementation, and decorator (using Moq/fakes).  
   - **Integration Tests** – In-memory server for end-to-end scenarios.  
   - **Stress Tests** – Confirms performance and concurrency safety under heavy load.

---

## Notable Classes for Contributors

The **classes most interesting** to developers extending or customizing queue behavior are located in: /Infrastructure/PriorityQueues/

Here you’ll find the existing queue implementations (`HeapPriorityQueue`, `BucketPriorityQueue`, `BitmaskPriorityQueue`, `DoubleLevelBitmaskPriorityQueue`) as well as base or utility classes for building new algorithms.

---

## Final Notes

**PrioQ** aims to be a **flexible, modular** queueing solution for .NET:

- **Interchangeable algorithms** for different priority range/performance needs.
- **Extensible decorators** for logging, analytics, concurrency, and more.
- **Simple config-driven** approach (`config.json` or custom providers).
- **Clean architecture** that’s easy to understand, test, and maintain.

Thank you for using **PrioQ**! We hope it simplifies your priority queue management and integrates smoothly into your .NET applications. For questions or advanced customization, please check out our [Issues](#) or contact the maintainers.

**Happy coding—and happy queueing!**

