using Core.Abstractions.Repositories;
using Domain.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Repositories
{
    public class TaskRepository : BaseRepository<Guid, FamilyTask, TaskRepository>, ITaskRepository
    {
        public TaskRepository(FamilyTaskContext context) : base(context)
        {

        }

        ITaskRepository IBaseRepository<Guid, FamilyTask, ITaskRepository>.NoTrack()
        {
            return base.NoTrack();
        }

        ITaskRepository IBaseRepository<Guid, FamilyTask, ITaskRepository>.Reset()
        {
            return base.Reset();
        }
    }
}
