using Microsoft.AspNetCore.Mvc;
using MW.OrderRegistration.ApiDemo.Contracts;
using MW.OrderRegistration.ConsoleDemo.Domain.Entities;
using MW.Persistence.Abstractions.Repositories;
using MW.Persistence.Abstractions.UnitOfWork;

namespace MW.OrderRegistration.ApiDemo.Controllers;

/// <summary>
/// Debug/test endpoints for validating repository and unit-of-work abstractions.
/// Development-only — not exposed in production.
/// </summary>
[ApiController]
[Route("api/test/repository")]
[Produces("application/json")]
public class TestRepositoryController : ControllerBase
{
    private readonly IRepository<TestItem, Guid> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TestRepositoryController> _logger;

    public TestRepositoryController(
        IRepository<TestItem, Guid> repository,
        IUnitOfWork unitOfWork,
        ILogger<TestRepositoryController> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Saves a simple demo entity using repository abstractions and unit of work.
    /// </summary>
    [HttpPost("save")]
    [ProducesResponseType(typeof(TestItemResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Save([FromBody] SaveTestItemRequest request, CancellationToken ct)
    {
        var entity = new TestItem
        {
            Id = Guid.NewGuid(),
            Name = string.IsNullOrWhiteSpace(request.Name) ? $"TestItem-{DateTime.UtcNow:HHmmss}" : request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("[TestRepo] Saved — Id={Id}, Name={Name}", entity.Id, entity.Name);

        return Created($"/api/test/repository/{entity.Id}", MapToResponse(entity));
    }

    /// <summary>
    /// Retrieves a previously saved demo entity by id through repository abstractions.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TestItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound(new { Message = $"TestItem {id} not found." });

        return Ok(MapToResponse(entity));
    }

    /// <summary>
    /// Updates an existing demo entity through repository abstractions and unit of work.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TestItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTestItemRequest request, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound(new { Message = $"TestItem {id} not found." });

        entity.Name = string.IsNullOrWhiteSpace(request.Name) ? entity.Name : request.Name;
        entity.Description = request.Description ?? entity.Description;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("[TestRepo] Updated — Id={Id}, Name={Name}", entity.Id, entity.Name);

        return Ok(MapToResponse(entity));
    }

    /// <summary>
    /// Deletes a demo entity through repository abstractions and unit of work.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return NotFound(new { Message = $"TestItem {id} not found." });

        _repository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("[TestRepo] Deleted — Id={Id}", id);

        return Ok(new { Message = $"TestItem {id} deleted.", Id = id });
    }

    /// <summary>
    /// Returns a list of demo entities using repository abstractions.
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(IReadOnlyList<TestItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var entities = await _repository.GetAllAsync(ct);
        var result = entities.Select(MapToResponse).ToList();
        return Ok(result);
    }

    /// <summary>
    /// Intentionally fails after making changes to observe rollback behavior.
    /// </summary>
    [HttpPost("rollback")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Rollback(CancellationToken ct)
    {
        var entity = new TestItem
        {
            Id = Guid.NewGuid(),
            Name = $"RollbackTest-{DateTime.UtcNow:HHmmss}",
            Description = "This entity should NOT be persisted — intentional rollback test.",
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, ct);

        _logger.LogWarning("[TestRepo] Rollback test — entity added to context but throwing before SaveChanges. Id={Id}", entity.Id);

        // Intentional failure before commit — entity should NOT be saved
        throw new InvalidOperationException(
            $"[Debug/Test] Intentional rollback test failure. " +
            $"Entity Id={entity.Id} was added to the context but SaveChangesAsync was never called. " +
            $"This verifies that uncommitted changes are not persisted.");
    }

    private static TestItemResponse MapToResponse(TestItem entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
