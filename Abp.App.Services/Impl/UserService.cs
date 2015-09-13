using Abp.App.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.App.Services.Impl
{
    public class UserService : IUserService
    {
        /// <summary>
        /// 用户仓库服务
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository"></param>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 验证用户名密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public async Task<bool> ValidateUserNameAuthorizationPwd(string userName, string pwd)
        {
            if (userName.IsNullOrEmpty() || pwd.IsNullOrEmpty())
                return false;
            return await _userRepository.ValidateUserNameAuthorizationPwd(userName, pwd);
        }
    }
}
