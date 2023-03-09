using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private readonly RedisCollection<Person> _people;
    private readonly RedisConnectionProvider _provider;

    public PeopleController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _people = (RedisCollection<Person>)provider.RedisCollection<Person>();
    }

    [HttpPost]
    public async Task<Person> AddPerson([FromBody] Person person)
    {
        await _people.InsertAsync(person);

        return person;
    }

    [HttpGet("filterAge")]
    public Task<IList<Person>> FilterByAge([FromQuery] int minAge, [FromQuery] int maxAge)
        => _people.Where(x => x.Age >= minAge && x.Age <= maxAge).ToListAsync();

    [HttpGet("filterGeo")]
    public Task<IList<Person>> FilterByGeo([FromQuery] double lon, [FromQuery] double lat, [FromQuery] double radius, [FromQuery] string unit)
        => _people.GeoFilter(x => x.Address!.Location, lon, lat, radius, Enum.Parse<GeoLocDistanceUnit>(unit)).ToListAsync();
    
    [HttpGet("filterName")]
    public Task<IList<Person>> FilterByName([FromQuery] string firstName, [FromQuery] string lastName) 
        => _people.Where(x => x.FirstName == firstName && x.LastName == lastName).ToListAsync();

    [HttpGet("postalCode")]
    public Task<IList<Person>> FilterByPostalCode([FromQuery] string postalCode)
        => _people.Where(x => x.Address!.PostalCode == postalCode).ToListAsync();

    [HttpGet("fullText")]
    public Task<IList<Person>> FilterByPersonalStatement([FromQuery] string text)
        => _people.Where(x => x.PersonalStatement == text).ToListAsync();

    [HttpGet("streetName")]
    public Task<IList<Person>> FilterByStreetName([FromQuery] string streetName)
        => _people.Where(x => x.Address!.StreetName == streetName).ToListAsync();

    [HttpGet("skill")]
    public Task<IList<Person>> FilterBySkill([FromQuery] string skill)
        => _people.Where(x => x.Skills.Contains(skill)).ToListAsync();

    [HttpPatch("updateAge/{id}")]
    public async Task<IActionResult> UpdateAge([FromRoute] string id, [FromBody] int newAge)
    {
        foreach (var person in _people.Where(x => x.Id == id))
        {
            person.Age = newAge;
        }

        await _people.SaveAsync();

        return Accepted();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson([FromRoute] string id)
    {
        await _provider.Connection.UnlinkAsync($"Person:{id}");

        return NoContent();
    }
}

