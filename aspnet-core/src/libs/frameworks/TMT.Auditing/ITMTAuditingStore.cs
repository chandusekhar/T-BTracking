using System.Threading.Tasks;

namespace TMT.Auditing
{
    public interface ITMTAuditingStore
    {
        Task SaveAsync(TMTAuditLogInfo auditInfo);
    }
}