using AutoMapper;
using krist_server.DTO.AuthDTOs;
using krist_server.Interfaces;
using krist_server.Models;
using Newtonsoft.Json;
using Supabase;

namespace krist_server.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly Client _client;
        private readonly IMapper _mapper;

        public UserRepository(Client client, IMapper mapper)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var response = await _client.From<User>().Where(x => x.UserId == id).Single();
            return response;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var response = await _client.From<User>().Where(x => x.Email == email).Single();
            return response;
        }

        public async Task<User?> CreateUserAsync(User user, string UserId)
        {
            user.UId = UserId;
            var response = await _client.From<User>().Insert(user);
            return response.Model;
        }
    }
}