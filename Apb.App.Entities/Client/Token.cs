using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apb.App.Entities.Client
{
    /// <summary>
    /// https://github.com/StackExchange/dapper-dot-net/tree/master/Dapper.Contrib
    /// </summary>
    [Table("tokens")]
    public class Token
    {
        [Key]
        public int Id { get; set; }
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public string Expires_in { get; set; }

        public string Refresh_token { get; set; }

        public string UserName { get; set; }

        public string ClientId { get; set; }

        public DateTime IssuedUtc { get; set; }

        public DateTime ExpiresUtc { get; set; }
    }
}
