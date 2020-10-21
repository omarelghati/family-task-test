using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Domain.ViewModel
{
    public class TaskVm
    {
        public Guid Id { get; set; }

        public string Subject { get; set; }

        public bool IsComplete { get; set; }

        public Guid? AssignedMemberId { get; set; }

        public MemberVm AssignedMember { get; set; }
    }
}
