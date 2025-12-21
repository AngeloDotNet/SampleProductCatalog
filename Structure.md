## Project Structure

```
ProductCatalog/
├── src/
│   ├── ProductCatalog.Domain/              # Entità, Value Objects, Eventi di Dominio
│   ├── ProductCatalog.Application/         # Use Cases, DTOs, Interfaces
│   ├── ProductCatalog.Infrastructure/      # Repository, RabbitMQ, EventStore, EF Core
│   └── ProductCatalog.API/                 # Controllers, Middleware, Configuration
├── docker-compose.yml
└── README.md
```


## Caratteristiche implementate

- ✅ Domain-Driven Design: Aggregati, Value Objects, Domain Events
- ✅ Clean Architecture: Separazione chiara tra layer
- ✅ CQRS: Separazione tra Command e Query
- ✅ Event-Driven: Pubblicazione eventi su RabbitMQ
- ✅ Repository Pattern
- ✅ Unit of Work
- ✅ MediatR custom per gestione comandi/query
- ✅ Entity Framework Core con PostgreSQL
- ✅ Docker Compose per orchestrazione