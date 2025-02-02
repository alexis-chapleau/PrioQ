# PrioQ
PrioQ – A Modular, Extensible Priority Queue System
PrioQ is a powerful, in‑memory priority queue service built on .NET. It supports multiple queue algorithms (e.g., heap, buckets, bitmask) with a decorator architecture for advanced features such as logging, locking, lazy deletion, and analytics. Through clean, layered design, PrioQ remains both testable and extensible—you can easily add new behaviors or algorithms to meet your production needs.

Key Features
Multiple Algorithms:

HeapPriorityQueue – A traditional binary heap-based priority queue with O(log n) enqueue/dequeue.
BucketPriorityQueue – Bucket-based approach, suitable for limited priority ranges.
BitmaskPriorityQueue – Single-level bitmask (up to 64 priorities), offering O(1) operations.
DoubleLevelBitmaskPriorityQueue – For larger priority ranges (up to 256 priorities), also O(1) operations.
Decorator Architecture:
Add cross-cutting features by layering decorators:

Logging – Logs each enqueue/dequeue.
Locking – Provides thread safety via locking.
Lazy Deletion – Marks items for removal before discarding them.
Analytics – Records analytics data for reporting.
Configurable at Runtime:

JSON Configuration – A config.json file or a custom IConfigProvider can specify whether you use unbounded vs. bounded priorities, which algorithm to select, and whether to apply decorators.
Clean Architecture:

Domain – Holds core entities (e.g. PriorityQueueItem, QueueConfig) and business logic contracts.
Application – Contains use cases (e.g. EnqueueCommandUseCase, DequeueCommandUseCase, InitializeQueueUseCase) orchestrating interactions between domain and infrastructure.
Infrastructure – Implements the queue algorithms, repository, analytics collectors, and decorator factories.
Presentation – Offers an ASP.NET Core UI (with a Setup Wizard and a Dashboard) and optional CLI for queue administration.
Testability:

Unit Tests – Thorough tests for each use case, queue implementation, and decorator using Moq/Fakes.
Integration Tests – Spin up the in‑memory server and verify end‑to‑end scenarios.
Stress Tests – Confirm performance and concurrency safety for high loads.

Final Notes
PrioQ aims to be a flexible, modular queueing solution for .NET:

Interchangeable algorithms for different priority range/performance needs.
Extensible decorators for logging, analytics, concurrency, and more.
Simple config-driven approach (JSON or custom providers).
Clean architecture that is easy to understand, unit test, and maintain.
Thank you for using PrioQ! We hope it simplifies your priority queue management and integrates smoothly into your .NET applications. For questions or advanced customization, check out our Issues or contact the maintainers.

Happy coding—and happy queueing!