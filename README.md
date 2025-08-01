# LondonStockAPI

System Design for MVP – LondonStockAPI
Architecture:
1. Type: Layered (API, Business/Repository, Data)
2. Framework: ASP.NET Core Web API (.NET 8)
3. Database: SQL Server (via Entity Framework Core)
4. Unit Testing: xUnit and Moq

Key Components:

API Layer:
1. Exposes REST endpoints for submitting trades and retrieving stock data.
2. Uses controllers (e.g., TradesController) for request handling.
3. Input/output models use DTOs and AutoMapper for mapping.
   
Business/Repository Layer:
1. ITradeRepo and TradeRepo abstract data access.
2. Handles trade creation and stock queries (single, multiple, all).
   
Data Layer:
1. AppDbContext manages EF Core entities (Trade).
2. Database schema includes a Trades table with indexes for performance. Added non-clustered index on ticker symbol for faster retrieval
   
Caching:
  In-memory caching (IMemoryCache) for stock lookups to reduce DB load.
  
MVP Features:
1. Submit a trade (POST).
2. Get current value for a stock (GET).
3. Get all stocks (GET).
4. Get selected stocks (POST).
5. Basic in-memory caching for stock data.


Improvements
1. Introducing a business layer service ITradeEngine between API and Repository. Currently calculating stock price in repository is tighlty coupled. Having a trade engine for calculation makes it more modular and testable. This will also helps if there is any change in the calculation or addition of new format for calculation.
2. Use model validation attributes and return detailed validation errors. Implement global exception handling middleware for consistent error responses.
3. Secure the end points by adding authentication and add role based access control for sensitive operations like SubmitTrade.
4. Add API versioning, to support multiple API versions to ensure backward compatibility.

Bottlenecks
1. In the production single instance of API cannot handle high concurrent requests. Need to implement horizontal auto-scaling and add a load-balancer to split the load between app servers
2. In current solution I am using InMemory cache. When we scale our API horizontally we need centralized cache. Need to implement IDistributeCache and make use of Redis cache.
3. This application in production will be high write intensive. And data can be lost if API or database fails. We can use asynchronous processing using message queue, it decouples writes from reads and buffers load. In this case our API becomes producer and writes message to queue. We can implement a consumer service as worker service which reads data from queue and writes it to database. Same time we can have another consumer which updates the stock average price in Redis cache. And also we can have separate Write and Read databases.
4. Getting All Stocks can take more time to load as the number of stocks increases. We can implement pagination here.
5. At the beginning of the day we can pre-calculate all the stock average values and pre-populate in Cache.
6. High write load might fail due to timeouts. We can have retry policies, using polly.
7. I am not sure whether we can have rate limiting because I need to know more about business requirement. And as per now I think that each broker can submit many trades in given time.
8. And sometimes we can have load on a single stock. We can consider data partitioning on the ticker symbol.
9. Deployment point of view we can create docker images and deploy in Kubernetes cluster with each servers, database, message queue, redis cache as PODS and assign cluster ip address for them to communicate with each other. 
