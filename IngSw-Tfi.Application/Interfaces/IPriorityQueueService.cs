using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Application.Interfaces;

public interface IPriorityQueueService
{
    void Enqueue(Income income);
    Income? Dequeue();
    IEnumerable<Income> GetAll();
}
