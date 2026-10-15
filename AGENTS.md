# AGENTS.md

Цей документ містить опис бізнес-контексту, архітектурних патернів, структури проєкту та правил найменування для автоматизованих агентів (AI Agents) та розробників, які працюють із кодовою базою рішення **KPS_Kurs**.

---

## 0. Бізнес-контекст проєкту (Business Context)

### Тема проєкту
**«Розробка вебплатформи для створення та проведення освітніх вікторин із елементами стратегічної гри»**

### Постановка задачі та предметна область
Метою проєкту є розробка вебплатформи (RESTful Web API бекенду) для створення, налаштування та проведення освітніх вікторин із механіками настільних стратегічних ігор. 

**Основні функціональні вимоги до системи:**
- **Користувачі та ролі (`Users` / `Identity`)**: реєстрація та авторизація користувачів із розподілом на ролі (`Player`, `Author`, `Admin`).
- **Конструктор та каталог контенту (`Catalog`)**: інструменти для авторів контенту, що дозволяють створювати навчальні ігри (`Games`), ігрові поля (`Boards`) із різними типами клітинок (`Start`, `Finish`, `Question`, `Bonus`, `Penalty`, `Card`), ігрові картки (`Cards`), а також формувати банк запитань (`Questions`) та варіантів відповідей (`Answers`) у розрізі навчальних дисциплін (`Disciplines`), тем (`Topics`) і рівнів складності.
- **Ігровий процес (`Gameplay`)**: підтримка проведення ігрових сесій (`Sessions`), підключення гравців (`SessionPlayers`) та налаштування ігрових механік (кидок кубика, переміщення гравця ігровим полем, виконання дій на клітинках, отримання бонусів/штрафів, відповіді на запитання та нарахування балів). Всі події в межах сесії логуються у форматі Event Sourcing (`SessionEvents` із `jsonb` payload).
- **Аналітика та прогрес (`Analytics`)**: збереження підсумкових результатів проходження ігор (`GameResults`), формування рейтингів, відображення статистики правильних відповідей і відстеження рівня опанування дисциплін користувачами (`UserProgress`).

---

## 1. Архітектурний огляд (Architecture Overview)

Проєкт побудовано за принципами **Модульного Моноліту (Modular Monolith)** та **Чистої Архітектури (Clean Architecture)** у вигляді **ASP.NET Core Web API** на платформі **.NET 10**.

Примітка для модуля Users: Модуль Users використовує ASP.NET Core Identity. Його контекст бази даних (UsersDbContext) повинен успадковуватися від IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, де ApplicationUser : IdentityUser<Guid>. Усі таблиці Identity (AspNetUsers, AspNetRoles або перейменовані Users, Roles тощо) повинні явно конфігуруватися у схемі identity (builder.HasDefaultSchema("identity")).

### Ключові принципи:
- **Модульність**: Кожна бізнес-домена розміщена в окремому модулі у директорії `src/Modules/`:
  - `Catalog` — управління дисциплінами, темами, питаннями, іграми, дошками та картками.
  - `Users` (схема БД `identity`) — управління користувачами, ролями та автентифікацією.
  - `Gameplay` — керування активними ігровими сесіями, станом гравців на полі та ігровими подіями.
  - `Analytics` — статистика ігор, результати сесій та прогрес навчання.
- **Ізоляція шарів**: Кожен модуль розділений на 5 проєктів:
  1. `Domain` — доменні сутності, перерахування (Enums), бізнес-правила, базові моделі. Не має зовнішніх залежностей.
  2. `Application` — CQRS (команди, запити), DTO, конфігурації маппера (AutoMapper / Mapster), pipeline behaviors, інтерфейси репозиторіїв та Unit of Work, FluentValidation.
  3. `Persistence` — EF Core DbContext, конфігурації сутностей, реалізація Unit of Work та Repository, міграції.
  4. `Presentation` — REST API контролери (`[ApiController]`) конкретного модуля.
  5. `Integration` — відкриті контракти, DTO подій та інтерфейси для міжмодульної взаємодії.
- **Хост-додаток (Web API Host)**: Проєкт `src/API/EntryPoint` (ASP.NET Core Web API) слугує єдиною точкою входу, підключає всі модулі, налаштовує DI-контейнер, Swagger/OpenAPI, аутентифікацію (JWT) та HTTP-пайплайн.
- **Централізоване керування пакетами (Central Package Management — CPM)**:
  - На першому ж кроці в корені рішення обов'язково має створюватися файл `Directory.Packages.props` із глобальним увімкненням `ManagePackageVersionsCentrally="true"`.
  - Це гарантує, що всі 10+ проєктів модульного моноліту використовуватимуть єдині узгоджені версії пакетів (MediatR, FluentValidation, AutoMapper, EF Core тощо), запобігаючи конфліктам збірки.

---

## 2. Структура папок (Directory Structure)

```text
KPS_Kurs/
├── KPS_Kurs.slnx                                  # Solution-файл проєкту
├── Directory.Packages.props                       # Централізоване керування версіями NuGet-пакетів (CPM)
├── AGENTS.md                                      # Правила та контекст для AI-агентів
├── src/
│   ├── API/
│   │   ├── EntryPoint/                            # Головний Web API Host (ASP.NET Core Web API)
│   │   │   ├── Controllers/                       # Системні/загальні API контролери
│   │   │   ├── Properties/
│   │   │   ├── appsettings.json                   # Конфігурація підключень та налаштувань
│   │   │   └── Program.cs                         # Точка реєстрації DI, модулів та middleware
│   │   └── Shared/                                # Спільні компоненти, базові класи та утиліти для API
│   └── Modules/
│       ├── Catalog/                               # Модуль "Каталог ігор та запитань"
│       │   ├── Catalog.Application/               # CQRS (Commands/Queries), DTOs, Mapping, Interfaces, Behaviors
│       │   ├── Catalog.Domain/                    # Entities (Game, Board, Question тощо), Enums, BaseEntity
│       │   ├── Catalog.Integration/               # Інтеграційні контракти та події між модулями
│       │   ├── Catalog.Persistence/               # DbContext, EF Configurations, Repositories, Migrations
│       │   └── Catalog.Presentation/              # REST API Контролери модуля Catalog
│       ├── Users/                                 # Модуль "Користувачі та Автентифікація" (схема identity)
│       │   ├── Users.Application/
│       │   ├── Users.Domain/
│       │   ├── Users.Integration/
│       │   ├── Users.Persistence/
│       │   └── Users.Presentation/
│       ├── Gameplay/                              # Модуль "Ігровий процес та Сесії" (схема gameplay)
│       └── Analytics/                             # Модуль "Аналітика та Прогрес" (схема analytics)
```

---

## 3. Реалізовані патерни (Design Patterns & Practices)

### 3.1. CQRS, MediatR та Використання Маппера (Vertical Slice inside Application)
Кожна операція (Command або Query) реалізується за допомогою патерну **Vertical Slice / Feature Packaging** у вигляді єдиного статичного класу-контейнера.

**Використання Маппера в Хендлерах:**
Усі перетворення між вхідними DTO, доменними сутностями та вихідними `ResponseDto` всередині `Handler` виконуються за допомогою ін'єктованого сервісу маппінгу (`IMapper`). Ручне перекладання полів у хендлерах не рекомендується, окрім специфічних викликів фабричних доменних методів.

**Структура файлу команди/запиту:**
```csharp
namespace Catalog.Application.Games.Commands;

public static class CreateGame
{
    // 1. DTO / Command Definition
    public sealed record Command(CreateGameDto Dto) : ICommand<Result<GameResponseDto>>;

    // 2. FluentValidation Validator
    public sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Dto.Title)
                .NotEmpty().WithMessage("Game title cannot be empty.")
                .MaximumLength(200);
                
            RuleFor(x => x.Dto.AuthorId)
                .NotEmpty().WithMessage("AuthorId is required.");
        }
    }

    // 3. MediatR Request Handler (з ін'єкцією IUnitOfWork та IMapper)
    public sealed class Handler(IUnitOfWork unitOfWork, IMapper mapper) 
        : IRequestHandler<Command, Result<GameResponseDto>>
    {
        public async Task<Result<GameResponseDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Маппінг DTO у доменну сутність (або створення через доменний конструктор/фабрику)
            var game = mapper.Map<Domain.Entities.Game>(request.Dto);

            await unitOfWork.Games.AddAsync(game, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Маппінг збереженої сутності у вихідний ResponseDto
            var responseDto = mapper.Map<GameResponseDto>(game);

            return Result<GameResponseDto>.Success(responseDto);
        }
    }
}
```

### 3.2. MediatR Pipeline Behaviors
У шарі `Application` реалізовано автоматичні пайплайни для обробки запитів (виконуються у строгому порядку):
1. **`LoggingBehavior<TRequest, TResponse>`**:
   - Логує початок, час виконання та помилки запиту.
2. **`ValidationBehavior<TRequest, TResponse>`**:
   - Автоматично знаходить та запускає всі `IValidator<TRequest>` перед виконанням хендлера.
   - У разі виявлення помилок повертає помилку валідації або викидає `ValidationException` (який перетворюється middleware у `400 Bad Request` із `ProblemDetails`).
3. **`TransactionBehavior<TRequest, TResponse>`**:
   - Відстежує запити, що реалізують інтерфейс `IBaseCommand` (`ICommand` / `ICommand<TResponse>`).
   - Автоматично загортає виконання команди в БД-транзакцію через `IExecutionStrategy` з підтримкою `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync` та логуванням.

### 3.3. Unit of Work & Generic Repository
- **`IRepository<TEntity>`**: Універсальний інтерфейс репозиторію (`GetByIdAsync`, `FirstOrDefaultAsync`, `FindAsync`, `AnyAsync`, `Query`, `AddAsync`, `Update`, `Remove`).
- **`IUnitOfWork`**: Надає доступ до репозиторіїв сутностей конкретного модуля (наприклад, у модулі `Catalog`: `Disciplines`, `Topics`, `Questions`, `Answers`, `Games`, `Boards`, `BoardCells`, `Cards`), керує збереженням змін (`SaveChangesAsync`) та транзакціями.
- **Реалізація**: `UnitOfWork` використовує ліниву ініціалізацію (lazy initialization) репозиторіїв та працює з відповідним `DbContext` модуля.

### 3.4. Result Pattern
Для повернення результатів виконання команд та запитів замість викидання виключень для бізнес-логіки використовується уніфікована обгортка `Result<T>`:
- `Result<T>.Success(T value)` — успішне виконання.
- `Result<T>.Failure(string error)` — бізнес-помилка виконання з описом причини.

### 3.5. Encapsulated Domain Model (DDD-Lite)
- Доменні сутності успадковуються від `BaseEntity` (`Guid Id`).
- Властивості сутностей мають модифікатор **`private set`**.
- Зміна стану сутностей здійснюється через доменні методи або конструктори.
- Колекції інкапсульовані: внутрішнє поле `List<T> _items` експонується назовні як `IReadOnlyCollection<T>`.

---

## 4. Правила найменування та кодингу (Naming Conventions)

### 4.1. Проєкти та Простори Назв (Namespaces)
- **Проєкти**: `[ModuleName].[Layer]` (наприклад, `Catalog.Domain`, `Catalog.Application`, `Catalog.Persistence`, `Catalog.Presentation`, `Catalog.Integration`).
- **Хост-проєкт**: `EntryPoint` (у папці `src/API/EntryPoint`).
- **Namespaces**: Повністю відповідають шляху до папки відносно проєкту.

### 4.2. Класи та Файли
- **Фічі CQRS**: Статичний клас-контейнер `[Verb][Entity]` (наприклад, `CreateGame`, `GetQuestionsByTopic`). Вкладені типи: `Command`/`Query`, `Validator`, `Handler`.
- **DTO**: `Create[Entity]Dto`, `Update[Entity]Dto`, `[Entity]ResponseDto`.
- **Профілі Маппера**: `[Entity]MappingProfile.cs` (або конфігурації у папці `Mappings/` шару `Application`).
- **Сутності**: В однині (`Game`, `Board`, `BoardCell`, `Question`, `Session`).
- **Конфігурації EF Core**: `[Entity]Configuration.cs`.

### 4.3. C# та Синтаксис
- Використовувати можливості **C# 12 / .NET 10**: Primary Constructors, File-scoped namespaces, Record-типи для команд та DTO.
- Приватні поля класичного типу позначаються префіксом: `_context`, `_cells`, `_questions`.

### 4.4. Контролери (Web API Presentation Layer)
Оскільки проєкт є **ASP.NET Core Web API** (а не MVC з Views), презентаційний шар кожного модуля (`[ModuleName].Presentation`) будується за стандартами RESTful API:
- Контролери позначаються атрибутами `[ApiController]` та `[Route("api/[controller]")]` (або явним маршрутом, наприклад `[Route("api/catalog/games")]`).
- Контролери успадковують `ControllerBase` (не `Controller`, оскільки підтримка Razor Views не потрібна).
- Відповіді повертаються у форматі JSON через стандартні методи `ActionResult` / `IActionResult` (`Ok()`, `CreatedAtAction()`, `BadRequest()`, `NotFound()`, `NoContent()`).
- Валідація вхідних моделей та команд відбувається автоматично через `[ApiController]` та `ValidationBehavior` у MediatR.
- Якщо `result.IsSuccess == false`, контролер повертає відповідний HTTP-код помилки (наприклад, `BadRequest(new { error = result.Error })` або `NotFound()`).

**Приклад Web API Контролера:**
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Catalog.Application.Games.Commands;
using Catalog.Application.Games.Queries;
using Catalog.Application.Games.Dtos;

namespace Catalog.Presentation.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetGames.Query(), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetGameById.Query(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGameDto dto, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateGame.Command(dto), ct);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }
}
```

---

## 5. Інструкції для AI Агентів при виконанні завдань

1. **Додавання нової фічі (Command/Query)**:
   * Створювати статичний клас у `src/Modules/[ModuleName]/[ModuleName].Application/[DomainGroup]/Commands/` або `/Queries/`.
   * Завжди використовувати типізований результат: `ICommand<Result<TDto>>` або `IQuery<Result<TDto>>`.
   * **Обов'язково ін'єктувати та використовувати `IMapper`** у `Handler` для перетворення між DTO та доменними сутностями, а також додавати відповідні маппінг-профілі в шар `Application`.
2. **Робота з базою даних**:
   * Зміни в схемі описувати у `[ModuleName].Domain` та `[ModuleName].Persistence/Configurations/`.
   * Завжди додавати EF Core Fluent API конфігурації замість Data Annotations.
   * Генерувати та реєструвати міграції суто у проєкті `[ModuleName].Persistence`.
3. **Презентаційний шар (Web API)**:
   * Завжди створювати REST API контролери (`[ApiController]`, спадкування від `ControllerBase`), що приймають та повертають JSON.
   * **Ніколи** не генерувати Razor Views (`.cshtml`) або MVC-специфічні конструкції (`return View()`, `RedirectToAction()`).
4. **Міжмодульна взаємодія**:
   * Використовувати виключно контракти з `[ModuleName].Integration`.
   * Прямі залежності між `Application`, `Domain` або `Persistence` різних модулів **суворо заборонені**.
5. **Дотримання чистоти шарів**:
   * `Domain` не посилається на жодні зовнішні бібліотеки.
   * `Application` посилається лише на `Domain` (та `Integration` за потреби).
   * `Persistence` посилається на `Domain` та `Application`.
   * `EntryPoint` об'єднує всі модулі для запуску Web API.
6. **Керування NuGet-пакетами (CPM)**:
   * На першому ж кроці в корені рішення обов'язково створюється та підтримується файл `Directory.Packages.props` із глобальним увімкненням `ManagePackageVersionsCentrally="true"`.
   * Ніколи не прописувати атрибут `Version` безпосередньо у файлах проєктів (`.csproj`). Версії всіх пакетів фіксуються виключно в `Directory.Packages.props`. У проєктах додаються лише посилання через `<PackageReference Include="..." />`.
7. **Ізоляція міграцій EF Core**:
   * Оскільки всі модулі (`Users`, `Catalog`, `Gameplay`, `Analytics`) використовують одну фізичну базу даних SQL Server, але різні схеми (`identity`, `catalog`, `gameplay`, `analytics`), кожен модуль під час налаштування `AddDbContext` зобов'язаний явно вказувати окрему таблицю історії міграцій у своїй схемі за допомогою `sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "<module_schema>")`.

---

## 6. Database Schema & EF Core Rules

Примітка для модуля Users: Модуль Users використовує ASP.NET Core Identity. Його контекст бази даних (UsersDbContext) повинен успадковуватися від IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, де ApplicationUser : IdentityUser<Guid>. Усі таблиці Identity (AspNetUsers, AspNetRoles або перейменовані Users, Roles тощо) повинні явно конфігуруватися у схемі identity (builder.HasDefaultSchema("identity")).

Цей проєкт — Модульний Моноліт. Кожен модуль має власну ізольовану логічну схему в БД (`identity`, `catalog`, `gameplay`, `analytics`).
**КРИТИЧНЕ ПРАВИЛО:** Прямі навігаційні властивості (Navigation Properties) та Foreign Keys (FK) на рівні EF Core між різними схемами/модулями **СУВОРО ЗАБОРОНЕНІ**.
Замість них використовуються **"Soft FK" (Cross-module IDs)** — звичайні скалярні властивості типу `Guid`. EF Core одного модуля не повинен знати про існування сутностей іншого модуля.

## Ізоляція EF Core міграцій (Окрема таблиця `__EFMigrationsHistory` для кожної схеми в SQL Server)

Оскільки всі модулі (`Users`, `Catalog`, `Gameplay`, `Analytics`) використовують **одну фізичну базу даних SQL Server**, але **різні логічні схеми** (`identity`, `catalog`, `gameplay`, `analytics`), кожен модуль під час налаштування `AddDbContext` **повинен явно вказувати окрему таблицю історій міграцій у своїй схемі**:

```csharp
options.UseSqlServer(connectionString, sqlOptions =>
    sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")); // замість catalog підставляти відповідну схему модуля
```

Це запобігає конфліктам таблиць міграцій між різними ізольованими контекстами бази даних. За замовчуванням EF Core створює одну таблицю `dbo.__EFMigrationsHistory` на всю БД. Якщо кілька контекстів використовуватимуть одну дефолтну таблицю, виникнуть конфлікти метаданих, блокування та помилки застосування міграцій.

### Таблиця відповідності модулів, контекстів та схем міграцій:
| Модуль | DbContext | Схема БД | Виклик `MigrationsHistoryTable` |
|---|---|---|---|
| `Users` | `UsersDbContext` | `identity` | `sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "identity")` |
| `Catalog` | `CatalogDbContext` | `catalog` | `sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "catalog")` |
| `Gameplay` | `GameplayDbContext` | `gameplay` | `sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "gameplay")` |
| `Analytics` | `AnalyticsDbContext` | `analytics` | `sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "analytics")` |

### Приклад конфігурації у шарі Persistence (`DependencyInjection.cs`):
```csharp
public static IServiceCollection AddCatalogPersistence(this IServiceCollection services, IConfiguration configuration)
{
    services.AddDbContext<CatalogDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnectionSqlServer"),
            sqlOptions =>
            {
                // Обов'язкова ізоляція таблиці міграцій для схеми модуля:
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "catalog");
                sqlOptions.MigrationsAssembly(typeof(CatalogDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null
                );
            }
        ));

    services.AddScoped<IUnitOfWork, UnitOfWork>();
    return services;
}
```

## Схема БД (DBML)
Кожна таблиця має мати поле createdAt, updatedAt
```dbml
// ==========================================
// ENUMS
// ==========================================
Enum Role {
  Player
  Author
  Admin
}

Enum CellType {
  Start
  Finish
  Question
  Bonus
  Penalty
  Card
}

Enum SessionStatus {
  Pending
  InProgress
  Completed
}

// ==========================================
// IDENTITY MODULE (Проєкт: Users, Схема: identity)
// ==========================================
Table identity.Users {
  Id uuid [pk]
  Email varchar [unique, not null]
  PasswordHash varchar [not null]
  Role Role [default: 'Player']
  CreatedAt timestamp
}

// ==========================================
// CATALOG MODULE (Проєкт: Catalog, Схема: catalog)
// ==========================================
Table catalog.Disciplines {
  Id uuid [pk]
  Name varchar [not null]
}

Table catalog.Topics {
  Id uuid [pk]
  DisciplineId uuid [ref: > catalog.Disciplines.Id]
  Name varchar [not null]
}

Table catalog.Questions {
  Id uuid [pk]
  TopicId uuid [ref: > catalog.Topics.Id]
  Text text [not null]
  DifficultyLevel int
}

Table catalog.Answers {
  Id uuid [pk]
  QuestionId uuid [ref: > catalog.Questions.Id]
  Text varchar [not null]
  IsCorrect boolean [default: false]
}

Table catalog.Games {
  Id uuid [pk]
  Title varchar [not null]
  Description text
  AuthorId uuid [note: 'Cross-module ID (identity.Users)']
  CreatedAt timestamp
}

Table catalog.Boards {
  Id uuid [pk]
  GameId uuid [ref: > catalog.Games.Id]
  SizeX int
  SizeY int
  BackgroundImage varchar
}

Table catalog.BoardCells {
  Id uuid [pk]
  BoardId uuid [ref: > catalog.Boards.Id]
  PositionX int
  PositionY int
  Type CellType
}

Table catalog.Cards {
  Id uuid [pk]
  GameId uuid [ref: > catalog.Games.Id]
  Title varchar [not null]
  Description text
  EffectType varchar
  EffectValue int
}

// ==========================================
// GAMEPLAY MODULE (Проєкт: Gameplay, Схема: gameplay)
// ==========================================
Table gameplay.Sessions {
  Id uuid [pk]
  GameId uuid [note: 'Cross-module ID (catalog.Games)']
  Status SessionStatus
  StartTime timestamp
  EndTime timestamp
}

Table gameplay.SessionPlayers {
  Id uuid [pk]
  SessionId uuid [ref: > gameplay.Sessions.Id]
  UserId uuid [note: 'Cross-module ID (identity.Users)']
  CurrentPositionId uuid [note: 'Cross-module ID (catalog.BoardCells)']
  Score int [default: 0]
  IsActiveTurn boolean [default: false]
}

Table gameplay.SessionEvents {
  Id uuid [pk]
  SessionId uuid [ref: > gameplay.Sessions.Id]
  PlayerId uuid [ref: > gameplay.SessionPlayers.Id]
  EventType varchar [not null]
  Payload jsonb [not null, note: 'Event Sourcing data (e.g. dice roll result)']
  CreatedAt timestamp
}

// ==========================================
// ANALYTICS MODULE (Проєкт: Analytics, Схема: analytics)
// ==========================================
Table analytics.GameResults {
  Id uuid [pk]
  SessionId uuid [note: 'Cross-module ID (gameplay.Sessions)']
  UserId uuid [note: 'Cross-module ID (identity.Users)']
  GameId uuid [note: 'Cross-module ID (catalog.Games)']
  TotalScore int
  CorrectAnswers int
  FinalRank int
  CompletedAt timestamp
}

Table analytics.UserProgress {
  Id uuid [pk]
  UserId uuid [note: 'Cross-module ID (identity.Users)']
  DisciplineId uuid [note: 'Cross-module ID (catalog.Disciplines)']
  MasteryLevel int [default: 0]
  TotalQuestionsAnswered int [default: 0]
}

// ==========================================
// CROSS-MODULE VIRTUAL RELATIONS
// ==========================================
// Ці зв'язки додані виключно для візуалізації на діаграмі. 
// У коді EF Core їх не повинно бути (немає навігаційних властивостей між модулями), 
// щоб не порушувати ізоляцію Bounded Contexts.
Ref: catalog.Games.AuthorId > identity.Users.Id
Ref: gameplay.Sessions.GameId > catalog.Games.Id
Ref: gameplay.SessionPlayers.UserId > identity.Users.Id
Ref: gameplay.SessionPlayers.CurrentPositionId > catalog.BoardCells.Id
Ref: analytics.GameResults.SessionId > gameplay.Sessions.Id
Ref: analytics.GameResults.UserId > identity.Users.Id
Ref: analytics.GameResults.GameId > catalog.Games.Id
Ref: analytics.UserProgress.UserId > identity.Users.Id
Ref: analytics.UserProgress.DisciplineId > catalog.Disciplines.Id
```

## Приклад EF Core конфігурації (Fluent API) для Soft FK

Щоб уникнути помилок міжмодульної взаємодії: **Soft FK (Cross-module ID) налаштовується виключно як звичайна скалярна колонка**. 
НІЯКИХ викликів `HasOne()`, `WithMany()` або `HasForeignKey()` для зв'язків між різними схемами/модулями. Водночас внутрішньомодульні зв'язки (наприклад, `Game` -> `Boards` або `Game` -> `Cards` всередині схеми `catalog`) налаштовуються стандартними засобами EF Core.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Catalog.Domain.Entities;

namespace Catalog.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        // Вказати назву таблиці та ізольовану схему модуля
        builder.ToTable("Games", "catalog");
        
        builder.HasKey(x => x.Id);

        // ПРАВИЛЬНО: звичайна конфігурація скалярної колонки для Cross-module Soft FK (identity.Users)
        builder.Property(x => x.AuthorId)
               .IsRequired();
        
        // СУВОРО ЗАБОРОНЕНО (викличе помилку міграції, бо User знаходиться в модулі Users / схемі identity):
        // builder.HasOne(x => x.Author).WithMany().HasForeignKey(x => x.AuthorId);

        builder.Property(x => x.Title)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.Description)
               .HasColumnType("text");

        // Внутрішньомодульні зв'язки (в межах схеми catalog) ДОЗВОЛЕНІ та налаштовуються повноцінно:
        builder.HasMany(x => x.Cards)
               .WithOne()
               .HasForeignKey(x => x.GameId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
```