using MTMiddleware.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityLibrary.Common;

namespace MTMiddleware.Shared.Abstracts
{
    public abstract class Entity : IEntity
    {
        public Entity()
        {
            Id = Utility.UniqueId().ToString();
        }

        [MaxLength(50)]
        public string Id { get; set; }
    }

    public abstract class Approval : IApproval
    {
        public string? ApprovalStatus { get; set; } = String.Empty;
        public string? ApprovalActionBy { get; set; } = String.Empty;
        public DateTime? ApprovalActionDate { get; set; }
        public string? ApprovalActionReason { get; set; } = String.Empty;
    }

    public abstract class AuditEntity : Entity, IAudit
    {
        public AuditEntity()
        {
            DateCreated = DateTime.UtcNow;
        }

        public DateTime DateCreated { get; set; }
        public DateTime? DateLastUpdated { get; set; }

        public string? CreatedBy { get; set; } = String.Empty;
        public string? LastUpdatedBy { get; set; } = String.Empty;
    }

    public abstract class BaseEntity : AuditEntity, IFullAudit, IActiveState
    {
        public BaseEntity()
        {
            DateTime currentDate = DateTime.UtcNow;

            DateCreated = currentDate;
            IsActive = true;
        }

        public bool IsActive { get; set; }
        public bool Archived { get; set; }
        public DateTime? DateArchived { get; set; }
        public string? ArchivedBy { get; set; } = String.Empty;
    }

    public abstract class BaseApprovalEntity : AuditEntity, IFullAudit, IActiveState, IApproval
    {
        public BaseApprovalEntity()
        {
            DateCreated = DateTime.UtcNow;
            IsActive = true;
            Archived = false;
        }

        public bool IsActive { get; set; }
        public bool Archived { get; set; }
        public DateTime? DateArchived { get; set; }
        public string? ArchivedBy { get; set; } = String.Empty;

        public string? ApprovalStatus { get; set; } = string.Empty;
        public string? ApprovalActionBy { get; set; } = String.Empty;
        public DateTime? ApprovalActionDate { get; set; }
        public string? ApprovalActionReason { get; set; } = String.Empty;
    }
}
