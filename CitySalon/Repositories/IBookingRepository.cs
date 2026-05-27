using CitySalon.Models;

namespace CitySalon.Repositories
{
    public interface IBookingRepository
    {
        List<Booking> GetAll();

        Booking GetById(int id);

        void Add(Booking booking);

        void Update(Booking booking);

        void Delete(int id);

        bool IsTimeTaken(int employeeId, DateTime startTime, DateTime endTime);
    }
}
