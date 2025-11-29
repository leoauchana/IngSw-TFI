using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;
using IngSw_Tfi.Domain.ValueObjects;

namespace IngSw_Tfi.Application.Services;

public class PriorityQueueService : IPriorityQueueService
{
    private readonly PriorityQueue<Income, IncomePriority> _queue
            = new PriorityQueue<Income, IncomePriority>(
                Comparer<IncomePriority>.Create((a, b) => a.CompareTo(b)));

    private readonly object _lock = new object();

    public void Enqueue(Income income)  
    {
        lock (_lock)
        {
            var priority = new IncomePriority(
                income.EmergencyLevel ?? EmergencyLevel.WITHOUT_URGENCY,
                income.IncomeDate ?? DateTime.Now
                );

            _queue.Enqueue(income, priority);
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
            return _queue.UnorderedItems
                        .OrderBy(x => x.Priority,
                              Comparer<IncomePriority>.Create((a, b) => a.CompareTo(b)))
                         .Select(x => x.Element)
                         .ToList();
        }
    }
}
