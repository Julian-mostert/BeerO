using System.Collections.Generic;
using System.Text.RegularExpressions;
using BeerOBot.ConsoleApp.SlackPlugin;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Quartz.Util;
using SpotifyAPI.Web.Models;

namespace BeerOBot.ConsoleApp.SlackMiddleWare
{
    public class SpotifyMiddleWare : MiddlewareBase
    {
        private string _setDeviceRegEx = "(?'text'set\\sdevice\\s)(?'DeviceId'.{40})";

        private readonly SpotifyPlugin _spotifyPlugin;
        public SpotifyMiddleWare(IMiddleware next, SpotifyPlugin spotifyPlugin) : base(next)
        {
            this._spotifyPlugin = spotifyPlugin;

            this.HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("who is the captain"),
                    Description = "What spotify account is this",
                    EvaluatorFunc = this.GetSpotifyUserDetails,
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("lets start the party"),
                    Description = "creates a playlist for beer o clock",
                    EvaluatorFunc = this.CreateSpotifyPlaylist,
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = RegexHandle.For("(tune\\sthis\\s)<(spotify:track:.{22})>","(tune\\sthis\\s)(.{22})"),                    
                    Description = "add a track to the playlist",
                    EvaluatorFunc = this.AddTrackToPlaylist,
                    VisibleInHelp = true
                },
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("Get devices"),
                    Description = "Finds devices available for playback",
                    EvaluatorFunc = this.GetAvialableSpotifyDevices,
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = RegexHandle.For(this._setDeviceRegEx),
                    Description = "Set the player device by device ID",
                    EvaluatorFunc = this.SetDevice,
                    VisibleInHelp = false
                },
                new HandlerMapping
                {
                    ValidHandles = ExactMatchHandle.For("start the tunes"),
                    Description = "Starts the music",
                    EvaluatorFunc = this.PlayResumeMusic,
                    VisibleInHelp = false
                },
            };
        }

        public IEnumerable<ResponseMessage> GetSpotifyUserDetails(IncomingMessage message, IValidHandle matchedHandle)
        {
            PrivateProfile spotifyPrivateProfile = this._spotifyPlugin.GetSpotifyUser();

            yield return message.ReplyToChannel(
                                                $"I am {spotifyPrivateProfile.DisplayName} : ID {spotifyPrivateProfile.Id} : Email address {spotifyPrivateProfile.Email}"
                                                , (Attachment) null
                                                );
        }

        public IEnumerable<ResponseMessage> CreateSpotifyPlaylist(IncomingMessage message, IValidHandle matchedHandle)
        {
            string reply;
            FullPlaylist createdPlaylist = this._spotifyPlugin.CreateNewPlaylist();
            if (createdPlaylist != null && createdPlaylist.Error == null)
            {
                reply = $"Aye aye captain party started, id:{createdPlaylist.Id},Name:{createdPlaylist.Name}";
            }
            else
            {
                reply = "we have been keel hauled";
            }
            yield return message.ReplyToChannel(
                                                reply,
                                                (Attachment) null
                                                );
        }

        public IEnumerable<ResponseMessage> AddTrackToPlaylist(IncomingMessage message, IValidHandle matchedHandle)
        {
            string reply;
            if (this._spotifyPlugin.spotifyPlayList == null)
            {
                reply = "the party needs to be started first";
            }
            else
            {
                Regex tuneRegex = new Regex("(tune\\sthis\\s)<(spotify:track:.{22})>");
                Match tuneMatch = tuneRegex.Match(message.FullText);
                string spotifyURI = tuneMatch.Groups[2].Value;
                bool trackAdded = this._spotifyPlugin.AddTrack(spotifyURI);
                reply = trackAdded ? "you added to the party" : "some thing happened";
                
            }

            yield return message.ReplyToChannel(reply,(Attachment) null);
        }

        public IEnumerable<ResponseMessage> GetAvialableSpotifyDevices(IncomingMessage message, IValidHandle matchedHandle)
        {
            string reply;


            AvailabeDevices spotifyDevices = this._spotifyPlugin.GetSpotifyDevices();
            if (spotifyDevices.HasError())
            {
                reply = "No Devices where found";
            }
            else
            {
                reply = "Please choose a device to use : set device deviceID";
                foreach (Device spotifyDevice in spotifyDevices.Devices)
                {
                    yield return message.ReplyToChannel($"Spotify device name : {spotifyDevice.Name} | type : {spotifyDevice.Type} | ID : {spotifyDevice.Id} ",(Attachment) null);
                } 
            } 
            yield return message.ReplyToChannel(reply,(Attachment) null);
        }
        
        public IEnumerable<ResponseMessage> SetDevice(IncomingMessage message, IValidHandle matchedHandle)
        {
            string reply;
            Regex regex = new Regex( this._setDeviceRegEx);
            Match setDeviceMatch = regex.Match(message.RawText);
            if (setDeviceMatch.Success)
            {
                string deviceId = setDeviceMatch.Groups["DeviceId"].Value;
                this._spotifyPlugin.SetPlaybackDevice(deviceId);
                reply = "Device was set";
            }
            else
            {
                reply = $"did not match the regex : {message.RawText}";
            }

            yield return message.ReplyToChannel(reply,(Attachment) null);
        }

        public IEnumerable<ResponseMessage> PlayResumeMusic(IncomingMessage message, IValidHandle matchedHandle)
        {
            string reply;
            if (this._spotifyPlugin.DeviceId.IsNullOrWhiteSpace())
            {
                reply = "No device selected";
            }
            else if(this._spotifyPlugin == null)
            {
                reply = "No playlist created";
            }
            else
            {
                bool resumeplay = this._spotifyPlugin.PlayPlaylist();
                if (resumeplay)
                {
                    reply = "the songs should start soooooon";
                }
                else
                {
                    reply = "eish i dont know";
                }
            }

            yield return message.ReplyToChannel(reply,(Attachment) null);
        }

    }
}