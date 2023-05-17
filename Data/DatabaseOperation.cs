using AkilliSayac.Interfaces;

namespace AkilliSayac.Data
{
    public class DatabaseOperation : IDatabaseOperation
    {
        private IDatabaseOperation database;

        public DatabaseOperation(IDatabaseOperation database)
        {
            this.database = database;
        }

        public byte[] DatabaseBackUp(string userId)
        {
            return database.DatabaseBackUp(userId);
        }
    }
}
