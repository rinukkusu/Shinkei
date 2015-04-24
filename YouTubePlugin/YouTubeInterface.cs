using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YouTubePlugin
{
    public class YouTubeInterface
    {
        private readonly YouTubePlugin _plugininstance;
        private YouTubeService _service;

        public YouTubeInterface(YouTubePlugin plugin)
        {
            _plugininstance = plugin;

            if (String.IsNullOrWhiteSpace(_plugininstance.Settings.ApiKey))
            {
                throw new Exception("Please set the ApiKey for YouTube");
            }

            _service = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _plugininstance.Settings.ApiKey,
                ApplicationName = _plugininstance.GetType().ToString()
            });
        }

        public Video GetVideoInfo(string IdOrUrl)
        {
            string Id = IdOrUrl;

            Regex reg = new Regex(@"v=([A-Za-z0-9_-]+)");
            if (reg.IsMatch(IdOrUrl))
            {
                var matches = reg.Matches(IdOrUrl);
                Id = matches[0].Groups[1].Captures[0].Value;
            }

            var videoListRequest = _service.Videos.List("snippet,contentDetails,statistics");
            videoListRequest.Id = Id;
            VideoListResponse response = videoListRequest.Execute();

            if (response.Items != null && response.Items.Count() > 0)
            {
                return response.Items[0];
            }
            else
            {
                return default(Video);
            }
        }

        public List<Video> SearchVideos(string keywords)
        {
            List<Video> returnList = new List<Video>();

            var searchListRequest = _service.Search.List("snippet");
            searchListRequest.Q = keywords;
            searchListRequest.MaxResults = 50;

            SearchListResponse response = searchListRequest.Execute();

            if (response.Items != null && response.Items.Count() > 0)
            {
                foreach (var searchResult in response.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        Video VideoInfo = GetVideoInfo(searchResult.Id.VideoId);
                        if (VideoInfo != null)
                        {
                            returnList.Add(VideoInfo);
                        }
                    }

                    if (returnList.Count() == _plugininstance.Settings.MaxResults)
                    {
                        break;
                    }
                }
            }

            return returnList;
        }

        public string GetCategory(string CategoryId)
        {
            var getCategoryRequest = _service.VideoCategories.List("snippet");
            getCategoryRequest.Id = CategoryId;
            var response = getCategoryRequest.Execute();

            if (response.Items != null && response.Items.Count() > 0)
            {
                return response.Items[0].Snippet.Title;
            }

            return "n/a";
        }

        public string FormatResponse(Video VideoInfo)
        {
            string formattedInfo = _plugininstance.Settings.SearchResultFormat;

            formattedInfo = formattedInfo.Replace("%link%", String.Format(@"https://youtu.be/{0}", VideoInfo.Id));
            formattedInfo = formattedInfo.Replace("%title%", VideoInfo.Snippet.Title);
            formattedInfo = formattedInfo.Replace("%category%", GetCategory(VideoInfo.Snippet.CategoryId));
            formattedInfo = formattedInfo.Replace("%length%", GetReadableTimespan(VideoInfo.ContentDetails.Duration));
            formattedInfo = formattedInfo.Replace("%views%", VideoInfo.Statistics.ViewCount.ToString());
            formattedInfo = formattedInfo.Replace("%rating%", GetRating(VideoInfo.Statistics.LikeCount, VideoInfo.Statistics.DislikeCount));

            return formattedInfo;
        
        
        }

        public string GetReadableTimespan(string timespan)
        {
            string returnString = timespan;

            Regex reg = new Regex(@"PT((\d+)H)?((\d+)M)?((\d+)S)?");
            if (reg.IsMatch(timespan))
            {
                MatchCollection matches = reg.Matches(timespan);
                if (matches.Count > 0)
                {
                    string hours = matches[0].Groups[2].Captures.Count > 0 ? matches[0].Groups[2].Captures[0].Value : "";
                    string minutes = matches[0].Groups[4].Captures.Count > 0 ? matches[0].Groups[4].Captures[0].Value : "";
                    string seconds = matches[0].Groups[6].Captures.Count > 0 ? matches[0].Groups[6].Captures[0].Value : "";

                    if (!String.IsNullOrWhiteSpace(hours))
                    {
                        returnString = String.Format("{0}h {1}m {2}s", hours, minutes, seconds);
                    }
                    else if (!String.IsNullOrWhiteSpace(minutes))
                    {
                        returnString = String.Format("{0}m {1}s", minutes, seconds);
                    }
                    else if (!String.IsNullOrWhiteSpace(seconds))
                    {
                        returnString = String.Format("{1}s", seconds);
                    }
                }
            }

            return returnString;
        }

        public string GetRating(ulong? likeCount, ulong? dislikeCount)
        {
            double localLikeCount = likeCount.HasValue ? likeCount.Value : 0;
            double localDislikeCount = dislikeCount.HasValue ? dislikeCount.Value : 0;

            if ((localLikeCount == 0) && (localDislikeCount == 0))
            {
                return "n/a";
            }

            if (localLikeCount == 0)
            {
                return "0%";
            }

            double rating = ((localLikeCount - localDislikeCount) / localLikeCount);
            return String.Format("{0}%", (int)(rating * 100));
        }
    }
}
