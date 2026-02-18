using NEW_FINAL_ERP.Infrastructure;
using NEW_FINAL_ERP.Models;

namespace NEW_FINAL_ERP.Repositories
{
    public interface INumberSequenceRepository
    {
        Task<IEnumerable<NumberSequence>> GetAll();

        Task Insert(UnitOfWork uow, NumberSequence seq);

        Task SoftDelete(UnitOfWork uow, int id);

        
    }
}
