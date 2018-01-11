using System;
using Sentiment.Web.Api.Models;

namespace Sentiment.Data
{
    public interface ISentimentRepository
    {
        SentimentSeries GetSentimentByKeyword(string keyword, TimeSpan groupingInterval, DateTime startDate, DateTime endDate);
        SentimentSeries GetSentimentByConfigId(long configId, TimeSpan groupingInterval, DateTime startDate, DateTime endDate);
    }
}