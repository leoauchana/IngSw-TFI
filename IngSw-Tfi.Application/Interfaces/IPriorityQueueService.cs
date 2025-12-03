using IngSw_Tfi.Domain.Entities;
using IngSw_Tfi.Domain.Enums;

namespace IngSw_Tfi.Application.Interfaces;

public interface IPriorityQueueService
{
    void Enqueue(Income income);
    Income? Dequeue();
    IEnumerable<Income> GetAll();
    bool HasActiveIncome(Guid patientId);
}
