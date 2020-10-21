using AutoMapper;
using AutoMapper.QueryableExtensions.Impl;
using Core.Abstractions.Repositories;
using Core.Abstractions.Services;
using Domain.Commands;
using Domain.DataModels;
using Domain.ViewModel;
using Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public TaskService(IMapper mapper, ITaskRepository taskRepository, IMemberRepository memberRepository)
        {
            _mapper = mapper;
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
        }

        public async Task<CreateTaskCommandResult> CreateTaskCommandHandler(CreateTaskCommand command)
        {
            var task = _mapper.Map<FamilyTask>(command);
            var persistedMember = default(FamilyTask);
            try
            {
                persistedMember = await _taskRepository.CreateRecordAsync(task);
            }
            catch (System.Exception ex)
            {

                throw;
            }

            var result = _mapper.Map<TaskVm>(persistedMember);

            return new CreateTaskCommandResult
            {
                Payload = result
            };
        }

        public async Task<UpdateTaskCommandResult> UpdateTaskCommandHandler(UpdateTaskCommand command)
        {
            var isSucceed = true;
            var member = await _taskRepository.ByIdAsync(command.Id);

            _mapper.Map(command, member);

            var affectedRecordsCount = await _taskRepository.UpdateRecordAsync(member);

            if (affectedRecordsCount < 1)
            {
                isSucceed = false;
            }

            return new UpdateTaskCommandResult()
            {
                Succeeded = isSucceed
            };
        }

        public async Task<GetAllTasksResult> GetAllTasksQueryHandler()
        {
            var result = new List<TaskVm>();

            var tasks = await _taskRepository.Reset().ToListAsync();

            foreach (var item in tasks)
            {
                if (item.AssignedMemberId.HasValue)
                {
                    item.AssignedMember = await _memberRepository.ByIdAsync(item.AssignedMemberId.Value);
                }
            }
            if (tasks.NotNull() && tasks.Any())
            {
                result = _mapper.Map<IEnumerable<TaskVm>>(tasks).ToList();
            }

            return new GetAllTasksResult()
            {
                Payload = result
            };
        }


    }
}
