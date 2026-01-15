using Driver.Services.Domain.Abstractions;

namespace Driver.Services.Domain.AggregatesModel.DriverAggregate;

public interface IDriverRepository : IRepository<Driver>
{
    Task<Driver?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Driver?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Driver?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Driver>> GetOnlineDriversAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Driver>> GetPendingVerificationDriversAsync(CancellationToken cancellationToken = default);
    Task<IQueryable<Driver>> GetAllAsync(CancellationToken cancellationToken = default);
    Driver Add(Driver driver);
    void Update(Driver driver);
    void Remove(Driver driver);
}
