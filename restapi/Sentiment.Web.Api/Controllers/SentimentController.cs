using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sentiment.Data;

namespace Sentiment.Web.Api.Controllers
{
    public class SentimentController : ApiController
    {
        private ISentimentRepository _SentimentRepo;

        public SentimentController(ISentimentRepository sentimentRepo)
        {
            _SentimentRepo = sentimentRepo;
        }

        [Route("sentiment/byKeyword/{keyword}/{startDate?}/{endDate?}/{period?}")]
        public IHttpActionResult GetSentiment(string keyword, DateTime? startDate = null, DateTime? endDate = null, string period = "d")
        {
            TimeSpan groupingInterval;
            switch (period)
            {
                case "d":
                    groupingInterval = TimeSpan.FromDays(1);
                    break;
                case "w":
                    groupingInterval = TimeSpan.FromDays(7);
                    break;
                case "m":
                    groupingInterval = TimeSpan.FromDays(30);
                    break;
                case "y":
                    groupingInterval = TimeSpan.FromDays(365);
                    break;
                default:
                    return Ok("Invalid period (must be 'd', 'w', 'm', or 'y')");
            }

            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.Now;

            var sentiment = _SentimentRepo.GetSentimentByKeyword(keyword, groupingInterval, startDate.Value, endDate.Value);

            return Ok(sentiment);
        }

        [Route("sentiment/byCfg/{configId}/{startDate?}/{endDate?}/{period?}")]
        public IHttpActionResult GetSentiment(long configId, DateTime? startDate = null, DateTime? endDate = null, string period = "d")
        {
            TimeSpan groupingInterval;
            switch (period)
            {
                case "d":
                    groupingInterval = TimeSpan.FromDays(1);
                    break;
                case "w":
                    groupingInterval = TimeSpan.FromDays(7);
                    break;
                case "m":
                    groupingInterval = TimeSpan.FromDays(30);
                    break;
                case "y":
                    groupingInterval = TimeSpan.FromDays(365);
                    break;
                default:
                    return Ok("Invalid period (must be 'd', 'w', 'm', or 'y')");
            }

            startDate = startDate ?? DateTime.MinValue;
            endDate = endDate ?? DateTime.Now;

            var sentiment = _SentimentRepo.GetSentimentByConfigId(configId, groupingInterval, startDate.Value, endDate.Value);

            return Ok(sentiment);
        }
    }
}
