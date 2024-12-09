namespace LibraryApp.Services;

public class LibraryService
{
    private readonly SQLiteAsyncConnection _database;
    public LibraryService()
    {
        string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyDatabase.db");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Book>().Wait();

    }
    private List<Book> _bookList = new List<Book>();
    public async Task<List<Book>> GetBooks()
    {
        if (_bookList.Count > 0)
            return _bookList;

        _bookList = await _database.Table<Book>().ToListAsync();
        return _bookList;
    }
}
