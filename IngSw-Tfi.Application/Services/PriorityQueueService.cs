using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Application.Services;

public class PriorityQueueService : IPriorityQueueService
{
    private readonly PriorityQueue<Income, EmergencyLevel> _queue
            = new PriorityQueue<Income, EmergencyLevel>();

    private readonly object _lock = new object();

    public void Enqueue(Income income)
    {
        lock (_lock)
        {
            _queue.Enqueue(income, income.EmergencyLevel);
        }
    }

    public Income? Dequeue()
    {
        lock (_lock)
        {
            if (_queue.Count == 0) return null;
            return _queue.Dequeue();
        }
    }

    public IEnumerable<Income> GetAll()
    {
        lock (_lock)
        {
            // DEVOLVER COPIA, no la cola interna
            return _queue.UnorderedItems
                         .Select(x => x.Element)
                         .ToList();
        }
    }
}
