using ApplicationCore.Entities;
using FluentResults;

namespace ApplicationCore.Interfaces;

public interface IPersonRepository
{
    Task<Result<int>> SavePerson(Person person);
}