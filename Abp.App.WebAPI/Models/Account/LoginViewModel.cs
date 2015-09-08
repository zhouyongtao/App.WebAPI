using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Abp.App.WebAPI.Models.Account
{
    public class LoginViewModel
    {
        /// <summary>
        /// 账户号
        /// </summary>
        [Required]
        [Display(Name = "账户号", Description = "账户号")]
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [Display(Name = "密码", Description = "密码")]
        public string Pwd { get; set; }
    }
}