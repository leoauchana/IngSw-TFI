namespace IngSw_Tfi.Application.DTOs;

public class PatientDto
{
    public record Request(string cuilPatient, string namePatient, string lastNamePatient, string email, DateTime birthDate, string? phone,
        string streetDomicilie, int numberDomicilie, string localityDomicilie, string? nameSocialWork, string? affiliateNumber);
    public record Response(Guid id, string cuilPatient, string namePatient, string lastNamePatient, string email, DateTime birthDate, string? phone,
        string streetDomicilie, int numberDomicilie, string localityDomicilie);
}
