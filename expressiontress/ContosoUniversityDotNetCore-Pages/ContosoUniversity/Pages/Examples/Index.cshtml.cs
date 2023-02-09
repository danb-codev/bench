using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ContosoUniversity.Pages.Examples;

public class Index : PageModel
{
    private readonly IMediator _mediator;

    public Index(IMediator mediator) => _mediator = mediator;

    public Result Data { get; set; }

    public async Task OnGetAsync(string sortOrder,
        string currentFilter, string searchString, int? pageIndex)
        => Data = await _mediator.Send(new Query { CurrentFilter = currentFilter, Page = pageIndex, SearchString = searchString, SortOrder = sortOrder });

    public record Query : IRequest<Result>
    {
        public string SortOrder { get; init; }
        public string CurrentFilter { get; init; }
        public string SearchString { get; init; }
        public int? Page { get; init; }
    }

    public record Result
    {
        public string CurrentSort { get; init; }
        public string NameSortParm { get; init; }
        public string DateSortParm { get; init; }
        public string CurrentFilter { get; init; }
        public string SearchString { get; init; }
        public PaginatedList<Model> Results { get; init; }
    }

    public record Model
    {
        public int Id { get; init; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; init; }
        public string LastName { get; init; }
        public DateTime EnrollmentDate { get; init; }
        public int EnrollmentsCount { get; init; }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateProjection<Student, Model>();
        }
    }

    public class QueryHandler : IRequestHandler<Query, Result>
    {
        private readonly SchoolContext _db;
        private readonly IConfigurationProvider _configuration;

        public QueryHandler(SchoolContext db, IConfigurationProvider configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<Result> Handle(Query message, CancellationToken token)
        {
            IQueryable<Student> students = _db.Students;
            string searchString = message.SearchString ?? message.CurrentFilter;

            if (!string.IsNullOrEmpty(searchString))
            {
                //students = students.Where(s => s.LastName.Contains(searchString)
                //    || s.FirstMidName.Contains(searchString));
                students = students.FindStudentsByName(searchString);
            }

            //students = message.SortOrder switch
            //{
            //    "name_desc" => students.OrderByDescending(s => s.LastName),
            //    "Date" => students.OrderBy(s => s.EnrollmentDate),
            //    "date_desc" => students.OrderByDescending(s => s.EnrollmentDate),
            //    _ => students.OrderBy(s => s.LastName)
            //};
            students = students.StudentsOrderBy(message.SortOrder);

            int pageSize = 3;
            int pageNumber = (message.SearchString == null ? message.Page : 1) ?? 1;

            var results = await students
                .ProjectTo<Model>(_configuration)
                .PaginatedListAsync(pageNumber, pageSize);

            var model = new Result
            {
                CurrentSort = message.SortOrder,
                NameSortParm = string.IsNullOrEmpty(message.SortOrder) ? "name_desc" : "",
                DateSortParm = message.SortOrder == "Date" ? "date_desc" : "Date",
                CurrentFilter = searchString,
                SearchString = searchString,
                Results = results
            };

            return model;
        }
    }
}

static class FilterExtensions
{
    public static IQueryable<TEntity> FindStudentsByName<TEntity>(this IQueryable<TEntity> @this, string searchString)
    {
        var stringType = typeof(string);
        var entityType = typeof(TEntity);
        // Student s
        var param = Expression.Parameter(entityType, "s");
        var constant = Expression.Constant(searchString, stringType);
        MethodInfo method = stringType.GetMethod("Contains", new[] { stringType });

        // left
        var firstNameProp = Expression.Property(param, "FirstMidName");
        var left = Expression.Call(firstNameProp, method, constant);

        // right
        var lastNameProp = Expression.Property(param, "LastName");
        var right = Expression.Call(lastNameProp, method, constant);

        // left or right
        var orExp = Expression.Or(left, right);

        var findExp = Expression.Lambda<Func<TEntity, bool>>(orExp, new[] { param });

        return @this.Where(findExp);
    }

    private static readonly MethodInfo OrderByMethod =
        typeof(Queryable).GetMethods()
            .Where(method => method.Name == "OrderBy")
            .Where(method => method.GetParameters().Length == 2)
            .Single();
    private static readonly MethodInfo OrderByDescMethod =
        typeof(Queryable).GetMethods()
            .Where(method => method.Name == "OrderByDescending")
            .Where(method => method.GetParameters().Length == 2)
            .Single();

    public static IQueryable<TEntity> StudentsOrderBy<TEntity>(this IQueryable<TEntity> @this, string sortOrder)
    {
        var entityType = typeof(TEntity);
        (string propName, MethodInfo sortMethodInfo) = sortOrder switch
        {
            "name_desc" => ("LastName", OrderByDescMethod),
            "Date" => ("EnrollmentDate",OrderByMethod),
            "date_desc" => ("EnrollmentDate",OrderByDescMethod),
            _ => ("LastName",OrderByMethod)
        };
        var param = Expression.Parameter(entityType, "s");
        var prop = Expression.Property(param, propName);

        LambdaExpression lambda = Expression.Lambda(prop, new[] { param });
        MethodInfo genericMethod = sortMethodInfo.MakeGenericMethod(new[] { entityType, prop.Type });
        object ret = genericMethod.Invoke(null, new object[] { @this, lambda });

        return (IQueryable<TEntity>)ret;
    }
}
