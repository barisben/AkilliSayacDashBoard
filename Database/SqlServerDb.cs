using AkilliSayac.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web.Mvc;

namespace AkilliSayac.Database
{
    [Authorize(Roles = "SuperAdmin")]
    public class SqlServerDb : IDatabaseOperation
    {
        private readonly ApplicationDbContext _db;

        public SqlServerDb(ApplicationDbContext db) {  _db = db; }

        public byte[] DatabaseBackUp(string userId)
        {
            string databaseName = _db.Database.GetDbConnection().Database;
            string backupFileName = Path.Combine(Path.GetTempPath(), $"{databaseName}_backup_{DateTime.Now:ddMMyyyyHHmmss}.bak");

            _db.Database.ExecuteSqlRaw($"BACKUP DATABASE {_db.Database.GetDbConnection().Database} TO DISK = '{backupFileName}'");

            byte[] fileBytes = File.ReadAllBytes(backupFileName);
            File.Delete(backupFileName);

            Log log = new Log();
            log.LogMessage = "Veritabanı yedeği indirildi";
            log.LogTime = DateTime.Now;
            log.LogTypeId = _db.LogTypes.Where(x => x.LogTypeName == "Database").FirstOrDefault().LogTypeId;
            log.UserId = userId;

            _db.Logs.Add(log);
            _db.SaveChanges();

            return fileBytes;   
        }
    }
}
