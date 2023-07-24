using ASPNetVueTemplate.Helpers;
using ASPNetVueTemplate.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASPNetVueTemplate.Controllers;

/*
 * This is an example of an API controller consuming the ExampleTable database table.
 * 
 * The PUT and POST methods will take a JSON object matching the ExampleTable class parameters in the body of the request.
 * Note that the body of the PUT and POST can be any class (see concept of ViewModels), and does not have to be your database table exactly. See other code examples from Jaclyn.
 * 
 */

[Route("api/[controller]")]
[ApiController]
public class ExampleController : ControllerBase
{
	/*
	 * Controllers use a concept called dependency injection to give you access to only things you need, in this case your database context, and app settings (as an example).
	 * Review this concept so you can inject other things as needed 
	 */
	private readonly ExampleAppDbContext _context;
	private readonly AppSettings _settings;

	public ExampleController(ExampleAppDbContext context, IOptions<AppSettings> settings)
	{
		_context = context;
		_settings = settings.Value;
	}

	/*
	 * Return paginated list of items in your table
	 * URL: /api/example
	 * URL with all optional parameters: /api/example?sort=name&page=1&perpage=20&filter=Jaclyn
	 * 
	 */
	[HttpGet]
	public async Task<ActionResult<dynamic>> GetList(string sort = "", int page = 1, int perPage = 20, string filter = "")
	{
		var query = _context.ExampleTables.AsNoTracking().AsQueryable();

		//Optional filtering of records
		if (!string.IsNullOrEmpty(filter))
		{
			query = query.Where(p => EF.Functions.Like(p.Name, $"{filter}%")
				|| EF.Functions.Like(p.Description, $"{filter}%"));
		}

		//Sort fields will be with first letter lower case. If descending order, add _desc to sort name, e.g., name_desc
		query = query.DynamicOrderBy(sort, "name");

		//Get just the items in the selected page so that we're not querying too much data from the DB at once
		var items = await PaginatedList<dynamic>.CreateAsync(query, page, perPage);

		//Returns the item list and total number of items in the table (not the paginated total); this is useful for displaying the total item count in the interface
		//When consuming this from your Vue.js, it will be in JSON with names starting in lowercase, e.g., items and total instead of Items and Total
		return new
		{
			Items = items,
			Total = items.TotalItems
		};
	}

	/*
	 * Return one record from your table with the given ID
	 * URL: /api/example/1
	 * 
	 */
	[HttpGet("{id}")]
	public async Task<ActionResult<dynamic>> Get(int id)
	{
		var item = await _context.ExampleTables.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
		if (item == null) return NotFound();

		return item;
	}

	/*
	 * Update an existing item in your table with the given ID
	 * URL: /api/example/1
	 * 
	 */
	[HttpPut("{id}")]
	public async Task<IActionResult> Put(int id, ExampleTable model)
	{
		if (id != model.Id)
		{
			return BadRequest();
		}
		
		_context.Entry(model).State = EntityState.Modified;

		try
		{
			await _context.SaveChangesAsync();
		}
		catch (DbUpdateConcurrencyException)
		{
			if (!await _context.ExampleTables.AnyAsync(d => d.Id == id))
			{
				return NotFound();
			}
			else
			{
				throw;
			}
		}

		return NoContent();
	}

	/*
	 * Add a new item to the database and return its ID
	 * URL: /api/example
	 * 
	 */
	[HttpPost]
	public async Task<ActionResult<int>> Post(ExampleTable model)
	{
		_context.ExampleTables.Add(model);
		await _context.SaveChangesAsync();

		return model.Id;
	}
}
