using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTMiddleware.Shared.Interfaces
{
    public interface IEntity
    {
        string Id { get; set; }
    }

    public interface IDateAudit
    {
        DateTime DateCreated { get; set; }
        DateTime? DateLastUpdated { get; set; }
    }

    public interface IDateAuditActor
    {
        string CreatedBy { get; set; }
        string LastUpdatedBy { get; set; }
    }

    public interface IRecordArchive
    {
        bool Archived { get; set; }
        DateTime? DateArchived { get; set; }
    }

    public interface IRecordArchiveActor : IRecordArchive
    {
        string ArchivedBy { get; set; }
    }

    public interface IAudit : IDateAudit, IDateAuditActor
    {
    }

    public interface IFullAudit : IAudit, IRecordArchiveActor
    {
    }

    public interface IActiveState
    {
        bool IsActive { get; set; }
    }

    public interface IEntityLifeTime
    {
        DateTime ExpiryDate { get; set; }
    }

    public interface IApproval
    {
        public string ApprovalStatus { get; set; }
        public string ApprovalActionBy { get; set; }
        public DateTime? ApprovalActionDate { get; set; }
        public string ApprovalActionReason { get; set; }
    }
}
