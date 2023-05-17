using AkilliSayac.Data;

namespace AkilliSayac.Interfaces
{
    public interface ILogOperation
    {
        static abstract void GetLogsFromFile(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment);
    }
}
