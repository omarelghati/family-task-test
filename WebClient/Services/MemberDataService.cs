using Domain.Commands;
using Domain.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebClient.Abstractions;
using Microsoft.AspNetCore.Components;
using Domain.ViewModel;
using Core.Extensions.ModelConversion;
using Core.Extensions;

namespace WebClient.Services
{
    public class MemberDataService : IMemberDataService
    {
        private readonly HttpClient httpClient;
        public MemberDataService(IHttpClientFactory clientFactory)
        {
            httpClient = clientFactory.CreateClient("FamilyTaskAPI");
            Members = new List<MemberVm>();
            LoadMembers();
        }
        //private IEnumerable<MemberVm> members;

        public List<MemberVm> Members { get; private set; }

        public MemberVm SelectedMember { get; private set; }

        public event EventHandler MembersChanged;
        public event EventHandler<string> UpdateMemberFailed;
        public event EventHandler<string> CreateMemberFailed;
        public event EventHandler SelectedMemberChanged;

        private async void LoadMembers()
        {
            var temp = (await GetAllMembers()).Payload.ToList();

            if(Members.NotNull() && temp.Count != Members.Count)
            {
                Members = temp;
                MembersChanged?.Invoke(this, null);
            }
        }

        private async Task<CreateMemberCommandResult> Create(CreateMemberCommand command)
        {
            return await httpClient.PostJsonAsync<CreateMemberCommandResult>("members", command);
        }

        private async Task<GetAllMembersQueryResult> GetAllMembers()
        {
            return await httpClient.GetJsonAsync<GetAllMembersQueryResult>("members");
        }

        private async Task<UpdateMemberCommandResult> Update(UpdateMemberCommand command)
        {
            return await httpClient.PutJsonAsync<UpdateMemberCommandResult>($"members/{command.Id}", command);
        }

        public async Task UpdateMember(MemberVm model)
        {
            var result = await Update(model.ToUpdateMemberCommand());

            Console.WriteLine(JsonSerializer.Serialize(result));

            if (result != null)
            {
                var updatedList = (await GetAllMembers()).Payload.ToList();

                if (updatedList != null)
                {
                    Members = updatedList;
                    MembersChanged?.Invoke(this, null);
                    return;
                }
                UpdateMemberFailed?.Invoke(this, "The save was successful, but we can no longer get an updated list of members from the server.");
            }

            UpdateMemberFailed?.Invoke(this, "Unable to save changes.");
        }

        public async Task CreateMember(MemberVm model)
        {
            var result = await Create(model.ToCreateMemberCommand());
            if (result != null)
            {
                var updatedList = (await GetAllMembers()).Payload;

                if (updatedList != null)
                {
                    Members = updatedList.ToList();
                    MembersChanged?.Invoke(this, null);
                    return;
                }
                UpdateMemberFailed?.Invoke(this, "The creation was successful, but we can no longer get an updated list of members from the server.");
            }

            UpdateMemberFailed?.Invoke(this, "Unable to create record.");
        }

        public void SelectMember(Guid id)
        {
            if (Members.All(memberVm => memberVm.Id != id)) return;
            {
                SelectedMember = Members.SingleOrDefault(memberVm => memberVm.Id == id);
                SelectedMemberChanged?.Invoke(this, null);
            }
        }

        public void SelectNullMember()
        {
            SelectedMember = null;
            SelectedMemberChanged?.Invoke(this, null);
        }
    }
}
