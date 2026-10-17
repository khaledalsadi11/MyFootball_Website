using Football.Models;
using Football.Repositry.IRepositry;

namespace Football.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Club> clubs { get; }
        IRepository<Coach> coaches { get; }
        IRepository<League> leagues { get; }
        IRepository<Player> players { get; }
        IRepository<Nationality> nationalites { get; }
        IRepository<AgeGroup> ageGroups { get; }
        IRepository<Team> teams { get; }
        IRepository<TrainingSession> trainingSessions { get; }
        IRepository<Attendance> attendances { get; }

        int CommitChanges();
    }
}
