namespace WfAssist.Shared;

public interface IUnitOfWork
{
	IRepository<TAggregate> GetRepository<TAggregate>();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}