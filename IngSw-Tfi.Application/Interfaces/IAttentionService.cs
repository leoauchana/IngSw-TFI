using IngSw_Tfi.Application.DTOs;

namespace IngSw_Tfi.Application.Interfaces;

public interface IAttentionService
{
    Task<AttentionDto.Response?> AddAttention(string idDoctor, AttentionDto.Request newAttention);
    Task<IncomeDto.Response?> UpdateIncomeStatus(string incomeId, string newStatus);
    Task<List<AttentionDto.Response>?> GetAll();
}
