namespace GestionProyectos.Data
{
    public class DataProvider : IDataProvider
    {
        public DataDbContext DataDbContext { get; }
        public DataProvider(DataDbContext nodeDbContext)
        {
            this.DataDbContext = nodeDbContext;
        }
    }
}
