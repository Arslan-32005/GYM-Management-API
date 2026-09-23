using Microsoft.EntityFrameworkCore;
namespace GYM_Management_API.Data
{
    public class GymContext: DbContext
    {
        public GymContext(DbContextOptions options) : base(options)
        {
        }
        
    }
}
