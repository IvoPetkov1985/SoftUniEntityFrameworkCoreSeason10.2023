namespace BookShop
{
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();

            // DbInitializer.ResetDatabase(db);
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var books = context.Books
                .AsNoTracking()
                .AsEnumerable()
                .Select(b => new
                {
                    Title = b.Title,
                    AgeRestriction = b.AgeRestriction.ToString()
                })
                .Where(b => b.AgeRestriction.ToLower() == command.ToLower())
                .OrderBy(b => b.Title)
                .AsEnumerable();

            StringBuilder builder = new StringBuilder();

            foreach (var book in books)
            {
                builder.AppendLine(book.Title);
            }

            return builder.ToString().Trim();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .AsNoTracking()
                .AsEnumerable()
                .Where(b => b.Copies < 5000 && b.EditionType.ToString() == "Gold")
                .Select(b => new
                {
                    Id = b.BookId,
                    Title = b.Title
                })
                .OrderBy(b => b.Id)
                .AsEnumerable();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(b => b.Price)
                .AsEnumerable();

            StringBuilder builder = new StringBuilder();

            foreach (var book in books)
            {
                builder.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return builder.ToString().Trim();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    Id = b.BookId,
                    Title = b.Title
                })
                .OrderBy(b => b.Id)
                .AsEnumerable();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            IEnumerable<string> categories = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower());

            var books = context.BooksCategories
                .Where(bc => categories.Contains(bc.Category.Name.ToLower()))
                .Select(b => b.Book.Title)
                .OrderBy(b => b)
                .AsEnumerable();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    Title = b.Title,
                    Edition = b.EditionType.ToString(),
                    Price = b.Price.ToString("F2")
                })
                .AsEnumerable();

            StringBuilder builder = new StringBuilder();

            foreach (var book in books)
            {
                builder.AppendLine($"{book.Title} - {book.Edition} - ${book.Price}");
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => a.FirstName + " " + a.LastName)
                .OrderBy(a => a)
                .AsEnumerable();

            return string.Join(Environment.NewLine, authors);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string searchedSubstring = input.ToLower();

            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(searchedSubstring))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .AsEnumerable();

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            string searchedSubstring = input.ToLower();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(searchedSubstring))
                .Select(b => new
                {
                    Id = b.BookId,
                    BookTitle = b.Title,
                    AuthorFullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .OrderBy(b => b.Id)
                .AsEnumerable();

            StringBuilder builder = new StringBuilder();

            foreach (var book in books)
            {
                builder.AppendLine($"{book.BookTitle} ({book.AuthorFullName})");
            }

            return builder.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .AsEnumerable();

            return books.Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .AsNoTracking()
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    BooksCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.BooksCount)
                .AsEnumerable();

            return string.Join(Environment.NewLine, authors.Select(a => $"{a.AuthorName} - {a.BooksCount}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    CategoryName = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(b => b.Book.Copies * b.Book.Price)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.CategoryName)
                .AsEnumerable();

            StringBuilder builder = new StringBuilder();

            foreach (var category in categories)
            {
                builder.AppendLine($"{category.CategoryName} ${category.TotalProfit:F2}");
            }

            return builder.ToString().TrimEnd();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    CategoryName = c.Name,
                    RecentBooks = c.CategoryBooks
                    .OrderByDescending(b => b.Book.ReleaseDate)
                    .Take(3)
                    .Select(b => new
                    {
                        BookTitle = b.Book.Title,
                        BookYear = b.Book.ReleaseDate.HasValue ? b.Book.ReleaseDate.Value.Year : 0
                    })
                    .AsEnumerable()
                })
                .OrderBy(c => c.CategoryName)
                .AsEnumerable();

            StringBuilder builder = new StringBuilder();

            foreach (var category in categories)
            {
                builder.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.RecentBooks)
                {
                    builder.AppendLine($"{book.BookTitle} ({book.BookYear})");
                }
            }

            return builder.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var selectedBooks = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010)
                .AsEnumerable();

            foreach (var book in selectedBooks)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();

            return booksToRemove.Length;
        }
    }
}
