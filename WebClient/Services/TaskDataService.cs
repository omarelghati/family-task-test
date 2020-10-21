using Core.Extensions.ModelConversion;
using Domain.Commands;
using Domain.ViewModel;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebClient.Abstractions;
using WebClient.Shared.Models;

namespace WebClient.Services
{
    public class TaskDataService : ITaskDataService
    {
        public List<TaskVm> Tasks { get; private set; }
        public TaskVm SelectedTask { get; private set; }


        public event EventHandler TasksUpdated;
        public event EventHandler TaskSelected;

        private readonly HttpClient httpClient;
        public TaskDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            Tasks = new List<TaskVm>();
            LoadTasks();
        }

        private async void LoadTasks()
        {
            Tasks = (await GetAllTasks()).Payload.ToList();
            TasksUpdated?.Invoke(this, null);
        }

        public void SelectTask(Guid id)
        {
            SelectedTask = Tasks.SingleOrDefault(t => t.Id == id);
            TasksUpdated?.Invoke(this, null);
        }

        public async void ToggleTask(Guid id)
        {
            var taskModel = Tasks.First(x => x.Id == id);
            taskModel.IsComplete = !taskModel.IsComplete;

            await Update(taskModel.ToUpdateTaskCommand());
            TasksUpdated?.Invoke(this, null);
        }

        private async Task<CreateTaskCommandResult> Create(CreateTaskCommand command)
        {
            return await httpClient.PostJsonAsync<CreateTaskCommandResult>("tasks", command);
        }

        private async Task<UpdateTaskCommandResult> Update(UpdateTaskCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateTaskCommandResult>("tasks", command);
        }

        public async void AssignTask(TaskVm task, Guid memberId)
        {
            await httpClient.PostJsonAsync<CreateTaskCommandResult>($"tasks/assign/{memberId}", task);
            LoadTasks();
        }

        public async void AddTask(TaskVm model)
        {
            var result = await Create(model.ToCreateTaskCommand());
            if (result != null)
            {
                var updatedList = (await GetAllTasks()).Payload.ToList();

                if (updatedList != null)
                {
                    Tasks = updatedList;
                    TasksUpdated?.Invoke(this, null);
                }
            }
        }

        private async Task<GetAllTasksResult> GetAllTasks()
        {
            return await httpClient.GetJsonAsync<GetAllTasksResult>("tasks");
        }
    }
}