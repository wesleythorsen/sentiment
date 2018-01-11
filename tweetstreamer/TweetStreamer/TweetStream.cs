using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;
using Stream = Tweetinvi.Stream;

namespace TweetStreamer
{
    class TweetStreamer
    {
        /// <summary>
        /// From TweetInvi library. Provides methods for accessing the twitter API.
        /// </summary>
        private ISampleStream _Stream;

        /// <summary>
        /// EntityFramework context.
        /// </summary>
        private TwitterEntities _Context;

        public delegate void StreamEvent(string text);
        
        // OAuth credentials from twitter, required for using their api
        private string _ConsumerKey = "";
        private string _ConsumerSecret = "";
        private string _AccessToken = "";
        private string _AccessTokenSecret = "";
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public TweetStreamer()
        {
            // Initialize new entityframework context
            _Context = new TwitterEntities();

            // Set OAuth credentials
            Auth.SetUserCredentials(_ConsumerKey, _ConsumerSecret, _AccessToken, _AccessTokenSecret);
        }
        
        /// <summary>
        /// Producer. Streams tweets from twiter sample stream and post for consumer to process.
        /// </summary>
        /// <param name="target"></param>
        public void Produce(ITargetBlock<ITweet> target)
        {
            // Create Twitter sample stream (1% of all public tweets)
            _Stream = Stream.CreateSampleStream();
            
            _Stream.TweetReceived += (object sender, Tweetinvi.Events.TweetReceivedEventArgs e) => {
                target.Post(e.Tweet);
            };
            
            _Stream.AddTweetLanguageFilter(LanguageFilter.English);
            
            // Begin streaming from twitter sample stream
            _Stream.StartStream();
            
            // Set the target to the completed state to signal to the consumer
            // that no more data will be available
            target.Complete();
        }
        
        /// <summary>
        /// Consumer. Reads tweets from producer.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public async Task<int> ConsumeAsync(ISourceBlock<ITweet> source)
        {
            // Read from the source buffer until the source buffer has no 
            // available output data
            while (await source.OutputAvailableAsync())
            {
                ITweet tweet = source.Receive();

                if (tweet.IsRetweet)
                {
                    // If tweet is a retweet, use the orginal tweet
                    tweet = tweet.RetweetedTweet;
                }

                AddTweet(tweet);
            }

            return 0;
        }

        public void Start()
        {
            // Create a BufferBlock<byte[]> object. This object serves as the 
            // target block for the producer and the source block for the consumer
            var buffer = new BufferBlock<ITweet>();

            // Start the consumer. The Consume method runs asynchronously
            var consumer = ConsumeAsync(buffer);

            // Post source data to the dataflow block
            Produce(buffer);

            // Wait for the consumer to process all data
            consumer.Wait();
        }

        /// <summary>
        /// Adds a Tweet to the data context.
        /// </summary>
        /// <param name="tweet"></param>
        /// <returns></returns>
        private void AddTweet(ITweet tweet)
        {
            Tweet existingTweet = null;
            
            // Check to see if tweet already exists in database
            existingTweet = _Context.Tweets.Local.Where(t => t.id == tweet.Id).FirstOrDefault() ??
                            _Context.Tweets.Where(t => t.id == tweet.Id).FirstOrDefault();
            
            if (existingTweet != null)
            {
                // Tweet alread exists, update the Favorite and Retweet count
                existingTweet.favorite_count = tweet.FavoriteCount;
                existingTweet.retweet_count = tweet.RetweetCount;
                existingTweet.updated_at = DateTime.Now;
            }
            else
            {
                // Tweet doesn't exist in database, create new record and add it
                var newTweet = new Tweet();
                newTweet.created_at = tweet.CreatedAt;
                newTweet.favorite_count = tweet.FavoriteCount;
                newTweet.id = tweet.Id;
                newTweet.retweet_count = tweet.RetweetCount;
                newTweet.source = tweet.Source;
                newTweet.text = tweet.Text;
                newTweet.updated_at = DateTime.Now;
                newTweet.user_id = tweet.CreatedBy.Id;
                newTweet.user_name = tweet.CreatedBy.Name;
                
                _Context.Tweets.Add(newTweet);
            }
            
            // Save changes to database
            _Context.SaveChanges();
        }
    }
}
