﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace ContosoUniversity.Pages.Courses;

public class Delete : PageModel
{
    private readonly IMediator _mediator;

    public Delete(IMediator mediator) => _mediator = mediator;

    [BindProperty]
    public Command Data { get; set; }

    public async Task OnGetAsync(Query query) => Data = await _mediator.Send(query);

    public async Task<IActionResult> OnPostAsync()
    {
        await _mediator.Send(Data);

        return this.RedirectToPageJson(nameof(Index));
    }

    public record Query : IRequest<Command>
    {
        public int? Id { get; init; }
    }

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(m => m.Id).NotNull();
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile() => CreateProjection<Course, Command>();
    }

    public class QueryHandler : IRequestHandler<Query, Command>
    {
        private readonly SchoolContext _db;
        private readonly IConfigurationProvider _configuration;

        public QueryHandler(SchoolContext db, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public Task<Command> Handle(Query message, CancellationToken token) =>
            _db.Courses
                //.Where(c => c.Id == message.Id)
                .FindRecord(message.Id)
                .ProjectTo<Command>(_configuration)
                .SingleOrDefaultAsync(token);
    }

    public record Command : IRequest
    {
        [Display(Name = "Number")]
        public int Id { get; init; }
        public string Title { get; init; }
        public int Credits { get; init; }

        [Display(Name = "Department")]
        public string DepartmentName { get; init; }
    }

    public class CommandHandler : IRequestHandler<Command>
    {
        private readonly SchoolContext _db;

        public CommandHandler(SchoolContext db) => _db = db;

        public async Task<Unit> Handle(Command message, CancellationToken token)
        {
            var course = await _db.Courses.FindAsync(message.Id);

            _db.Courses.Remove(course);

            return default;
        }
    }
}

static class FilterExtensions
{
    public static IQueryable<TEntity> FindRecord<TEntity>(this IQueryable<TEntity> @this, int? id)
    {
        var entityType = typeof(TEntity);
        var param = Expression.Parameter(entityType, "x");
        var prop = Expression.PropertyOrField(param, "Id");
        var constant = Expression.Constant(id, typeof(int));
        var findLambda = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(prop, constant),
                new[] { param }
            );

        return @this.Where(findLambda);
    }
}