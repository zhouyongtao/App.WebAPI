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
        // [Key]
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientType { get; set; }
        public string Scope { get; set; }
        public string UserName { get; set; }
        // [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        public string IpAddress { get; set; }
    }
}
