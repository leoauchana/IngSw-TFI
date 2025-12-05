using IngSw_Bdd.Mock;
using IngSw_Tfi.Application.Interfaces;
using IngSw_Tfi.Application.Services;
using IngSw_Tfi.Domain.Interfaces;
using IngSw_Tfi.Domain.Repository;
using IngSw_Tfi.Transversal.Services;
using Reqnroll.BoDi;

namespace IngSw_Bdd.Support;

[Binding]
public class IngSwDependencies
{

    private readonly IObjectContainer _container;

    public IngSwDependencies(IObjectContainer container)
    {
        _container = container;
    }

    [BeforeScenario]
    public void RegisterServices()
    {
        _container.RegisterTypeAs<PatientsRepositoyInMemory, IPatientRepository>();
        _container.RegisterTypeAs<IncomeRepositoryInMemory, IIncomeRepository>();
        _container.RegisterTypeAs<EmployeeRepositoryInMemory, IEmployeeRepository>();
        _container.RegisterTypeAs<SocialWorkRepositoryInMemory, ISocialWorkRepository>();

        _container.RegisterTypeAs<PatientsService, IPatientsService>();
        _container.RegisterTypeAs<IncomesService, IIncomesService>();
        _container.RegisterTypeAs<SocialWorkServiceApi, ISocialWorkServiceApi>();

        _container.RegisterTypeAs<PriorityQueueService, IPriorityQueueService>();
    }
}
