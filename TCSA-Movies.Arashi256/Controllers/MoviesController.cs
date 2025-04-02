using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TCSA_Movies.Arashi256.Models;

namespace TCSA_Movies.Arashi256.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly TCSA_MoviesArashi256Context _context;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(TCSA_MoviesArashi256Context context, ILogger<MoviesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string movieGenre, string searchString)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie' is null.");
            }
            IQueryable<string> genreQuery = from m in _context.Movie orderby m.Genre select m.Genre;
            var movies = from m in _context.Movie select m;
            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(s => s.Title!.ToUpper().Contains(searchString.ToUpper()));
            }
            if (!string.IsNullOrEmpty(movieGenre))
            {
                movies = movies.Where(x => x.Genre == movieGenre);
            }
            var movieGenreVM = new MovieGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Movies = await movies.ToListAsync()
            };
            return View(movieGenreVM);
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                TempData["success"] = $"Movie '{movie.Title}' added successfully";
                _logger.LogInformation("Add new movie: {Movie}", movie.ToString());
                return RedirectToAction(nameof(Index));
            }
            else
            {
                string errorMessage = $"Movie '{movie.Title}' could not be added";
                TempData["failure"] = errorMessage;
                _logger.LogError(errorMessage);
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                string errorMessage = $"Could not find movie details - supplied movie ID '{id}' was null";
                TempData["failure"] = errorMessage;
                _logger.LogError(errorMessage);
                return NotFound();
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                string errorMessage = $"Could not find movie details for supplied movie ID '{id}'";
                TempData["failure"] = errorMessage;
                _logger.LogError(errorMessage);
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price,Rating")] Movie movie)
        {
            if (id != movie.Id)
            {
                string errorMessage = $"Movie ID mismatch for movie edit: '{id}' against {movie.Id} for movie '{movie.Title}'";
                TempData["failure"] = errorMessage;
                _logger.LogError(errorMessage);
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                    string successMessage = $"Movie '{movie.Title}' updated successfully";
                    TempData["success"] = successMessage;
                    _logger.LogInformation(successMessage);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        string errorMessage = $"Movie ID '{movie.Id}' not found";
                        TempData["failure"] = errorMessage;
                        _logger.LogError(errorMessage);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                string errorMessage = "Movie ID is null for delete!";
                TempData["failure"] = errorMessage;
                _logger.LogError(errorMessage);
                return NotFound();
            }
            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                string errorMessage = $"Movie ID '{id}' not found for delete!";
                TempData["failure"] = errorMessage;
                _logger.LogError(errorMessage);
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            await _context.SaveChangesAsync();
            string successMessage = $"Movie '{movie.Title}' deleted successfully";
            TempData["success"] = successMessage;
            _logger.LogInformation(successMessage);
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
