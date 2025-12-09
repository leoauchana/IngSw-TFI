using IngSw_Tfi.Domain.Entities;

namespace IngSw_Tfi.Domain.Repository;

public interface ISocialWorkRepository
{
    Task<List<SocialWork>?> GetAll();
<<<<<<< HEAD
    Task<bool> ValidateInsuranceAndMember(string nameSocial, int memerNumber);
    Task<bool> ValidateInsuranceAndMember(string nameSocial, int memerNumber);
=======
    bool ValidateInsuranceAndMember(string nameSocial, int memerNumber);

>>>>>>> lastFixTests
    bool ValidateInsuranceAndMember(string nameSocial, int memerNumber);

>>>>>>> lastFixTests
    bool ValidateInsuranceAndMember(string nameSocial, int memerNumber);

>>>>>>> lastFixTests
    bool ValidateInsuranceAndMember(string nameSocial, int memerNumber);

>>>>>>> lastFixTests
=======
    bool ValidateInsuranceAndMember(string nameSocial, int memerNumber);

>>>>>>> lastFixTests
    Task<SocialWork?> GetById(string idSocialWork);
}
