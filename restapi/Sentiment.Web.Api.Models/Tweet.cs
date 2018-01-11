using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Web.Api.Models
{
    public class Tweet
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int RetweetCount { get; set; }
        public int FavoriteCount { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public long SourceId { get; set; }
    }
}
