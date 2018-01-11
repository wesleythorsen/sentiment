﻿using System;
using System.Collections.Generic;
using Sentiment.Web.Api.Models;

namespace Sentiment.Analyzer
{
    public interface ISentimentAnalyzer
    {
        Dictionary<DateTime, SentimentInfo> GetSentiment(IEnumerable<Tweet> tweets, TimeSpan groupingInterval);
    }
}