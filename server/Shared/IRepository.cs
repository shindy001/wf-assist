namespace WfAssist.Shared;

public interface IRepository<TAggregate>
{
	Task<TAggregate?> TryFindAsync(Guid key);
	void Add(TAggregate aggregate);
	void Delete(TAggregate aggregate);

	public async Task Delete(Guid key)
	{
		var aggregate = await TryFindAsync(key);
		if (aggregate is not null)
		{
			Delete(aggregate);
		}
	}
}