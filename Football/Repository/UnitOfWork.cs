using Football.Data;
using Football.Models;
using Football.Repository.IRepository;
using Football.Repositry.IRepositry;

namespace Football.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        public UnitOfWork(AppDbContext context) 
        {
            _context = context;
            clubs = new Repository<Club>(_context);
            coaches = new Repository<Coach>(_context);
            leagues = new Repository<League>(_context);
            players = new Repository<Player>(_context);
            nationalites = new Repository<Nationality>(_context);
            ageGroups = new Repository<AgeGroup>(_context);
            teams = new Repository<Team>(_context);
            trainingSessions = new Repository<TrainingSession>(_context);
            attendances = new Repository<Attendance>(_context);
        }

        private readonly AppDbContext _context;

        public IRepository<Club> clubs { get; private set; }
        public IRepository<Coach> coaches { get; private set; }
        public IRepository<League> leagues { get; private set; }
        public IRepository<Player> players { get; private set; }
        public IRepository<Nationality> nationalites { get; private set; }
        public IRepository<AgeGroup> ageGroups { get; private set; }
        public IRepository<Team> teams { get; private set; }
        public IRepository<TrainingSession> trainingSessions { get; private set; }
        public IRepository<Attendance> attendances { get; private set; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int CommitChanges()
        {
           return _context.SaveChanges();
        }
    }
}
