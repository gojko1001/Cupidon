using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Errors;
using DatingApp.Repository.Interfaces;
using DatingApp.Services.interfaces;
using DatingApp.Utils.Pagination;

namespace DatingApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public Task<MemberDto> GetUser(string username, bool isCurrentUser)
        {
            return _unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser);
        }

        public async Task<PagedList<MemberDto>> GetUsers(UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(userParams.CurrentUsername);
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            return await _unitOfWork.UserRepository.GetMembersAsync(userParams);
        }
        
        public async Task UpdateUser(MemberUpdateDto memberUpdateDto, string username)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _unitOfWork.UserRepository.Update(user);
            if (await _unitOfWork.Complete())
                return;
            throw new InvalidActionException("Failed to update user info");
        }

    }
}
