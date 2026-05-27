using CitySalon.Models;

namespace CitySalon.Repositories
{
    public interface IContactMessageRepository
    {
        List<ContactMessage> GetAll();

        ContactMessage? GetById(int id);

        void Add(ContactMessage message);

        void Delete(int id);

        void MarkAsRead(int id);
    }
}