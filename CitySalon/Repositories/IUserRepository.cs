using CitySalon.Models;

namespace CitySalon.Repositories
{
    public interface IUserRepository
    {


        User Login(string email, string password);

        List<User> GetAll();

        User GetById(int id);

        void Add(User user);

        void Update(User user);

        void Delete(int id);



    }
}
