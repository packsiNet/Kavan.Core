The solution contains these main layers:
Kavan.Core/
│
├── ApplicationLayer/
├── DomainLayer/
├── InfrastructureLayer/
└── Kavan.Api/

🧩 1. Domain Layer (DomainLayer)

Purpose:
Defines core business entities and domain logic, independent of other layers.

Rules:

Create entities under DomainLayer/Entities/

Common base classes are under DomainLayer/Common/BaseEntities/

Example: BaseEntity, AuditableEntity

Attributes and helpers are under DomainLayer/Common/Attributes/

Do not reference any other layer from Domain
Example:

namespace DomainLayer.Entities;

public class User  : BaseEntityModel, IAuditableEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
}

⚙️ 2. Application Layer (ApplicationLayer)

Purpose:
Implements application logic, orchestrates domain, infrastructure, and exposes commands/queries.

📁 Folder Structure
ApplicationLayer/
 ├── Common/
 │    ├── Behaviors/
 │    ├── Enums/
 │    ├── Extensions/
 │    └── Utilities/
 ├── Dto/
 │    ├── [FeatureName]/
 │    └── BaseDtos/
 ├── Features/
 │    └── [FeatureName]/
 │         ├── Commands/
 │         ├── Query/
 │         ├── Handler/
 │         └── Validations/
 ├── Interfaces/
 │    ├── Repositories/
 │    ├── Services/
 │    └── External/
 ├── Mapping/
 │    └── [FeatureName]/
 └── ...

📘 CQRS Rule

Every API endpoint or feature must be implemented as a CQRS Feature Folder:

Each feature has:

Command class (for mutations)

Query class (for reads)

Handler class (implements IRequestHandler<,>)

Validator class (FluentValidation)

DTOs inside ApplicationLayer/Dto/[FeatureName]

Handlers use IUnitOfWork and repositories from InfrastructureLayer

Example:
ApplicationLayer/Features/User/Commands/CreateUserCommand.cs
ApplicationLayer/Features/User/Handler/CreateUserHandler.cs
ApplicationLayer/Features/User/Validations/CreateUserValidator.cs
ApplicationLayer/Dto/User/UserDto.cs

Example Command:
public record CreateUserCommand(string Username, string Email) : IRequest<UserDto>;

Example Handler:
public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateUserHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<User>(request);
        await _uow.Repository<User>().AddAsync(entity);
        await _uow.SaveChangesAsync();
        return _mapper.Map<UserDto>(entity);
    }
}

🧱 3. Infrastructure Layer (InfrastructureLayer)

Purpose:
Implements interfaces defined in the Application layer (e.g., Repositories, Services, Notifications, Binance APIs, etc.)

📁 Folder Structure:

InfrastructureLayer/
 ├── BusinessLogic/
 │    └── Services/
 │         ├── Binance/
 │         ├── Notifications/
 │         └── ...
 ├── Configuration/
 ├── Context/
 ├── Repository/
 │    ├── UnitOfWork.cs
 │    └── Repository.cs
 ├── Helpers/
 ├── Extensions/
 └── Migrations/


Rules:

Implement all repository and service interfaces here.

Define UnitOfWork under InfrastructureLayer/Repository/UnitOfWork.cs

Use ApplicationLayer.Interfaces for contracts.

Register all dependencies in InfrastructureLayer/Configuration/DependencyInjection.cs

Example:
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IRepository<User> Users => new Repository<User>(_context);

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

🔹 Service Registration Convention

✅ Do NOT manually register services in DependencyInjection.cs

Instead, use attribute-based registration:
[InjectAsScoped]
public class CandleAggregatorService(
    IUnitOfWork _unitOfWork,
    IRepository<Cryptocurrency> _repository,
    IRepository<Candle_1m> _candles_1m,
    IRepository<Candle_5m> _candles_5m,
    IRepository<Candle_1h> _candles_1h,
    IRepository<Candle_4h> _candles_4h,
    IRepository<Candle_1d> _candles_1d
) : ICandleAggregatorService
{
    // Implementation here
}
Rules:

[InjectAsScoped] or [InjectAsSingleton] replaces manual AddScoped/AddSingleton

The service must implement its interface defined in ApplicationLayer.Interfaces

Use IUnitOfWork and IRepository<TEntity> for data access


🌐 4. API Layer (Kavan.Api)

Purpose:
Hosts all API endpoints and acts as the application entry point.

📁 Folder Structure

Kavan.Api/
 ├── Controllers/
 │    └── [FeatureName]Controller.cs
 ├── Middlewares/
 ├── Program.cs
 ├── appsettings.json
 └── ...

Rules:

Each Controller = one Feature

Controllers must only use IMediator to send Commands or Queries

No logic in Controller methods

Example:
namespace PresentationApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SampleController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync()
        => await ResultHelper.GetResultAsync(mediator, new SampleGetQuery());
}

🔄 Dependency Rules

✅ Allowed:

DomainLayer → (no dependencies)
ApplicationLayer → DomainLayer
InfrastructureLayer → ApplicationLayer, DomainLayer
Kavan.Api → ApplicationLayer


❌ Not allowed:
DomainLayer → ApplicationLayer
ApplicationLayer → InfrastructureLayer

🧩 Code Generation Flow (AI must follow)

Whenever creating a new feature or API:

Create a new folder in
ApplicationLayer/Features/[FeatureName]/

Add:

Command

Query

Handler

Validator

Add DTOs under ApplicationLayer/Dto/[FeatureName]/

Add Entity (if needed) in DomainLayer/Entities

Add Service Interface in ApplicationLayer/Interfaces/Services/

Implement Service in InfrastructureLayer/BusinessLogic/Services/[FeatureName]/

Decorate with [InjectAsScoped]

Add Controller under Kavan.Api/Controllers/[FeatureName]Controller.cs

Use IMediator to communicate with ApplicationLayer