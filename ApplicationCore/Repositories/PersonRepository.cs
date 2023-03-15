using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Persistence;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ApplicationCore.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<PersonRepository> _logger;

    public PersonRepository(ApplicationDbContext dbContext, ILogger<PersonRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<int>> SavePerson(Person person)
    {
        try
        {
            await _dbContext.Person.AddAsync(person);
            return Result.Ok(person.Id);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "There was an error saving the person");
            return Result.Fail("There was an error saving the person");
        }
    }
}