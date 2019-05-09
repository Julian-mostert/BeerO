using System;
using BeerOBot.ConsoleApp.Config;
using BeerOBot.ConsoleApp.Spotify;
using Noobot.Core.Plugins;
using Quartz.Util;
using SpotifyAPI.Web.Models;

namespace BeerOBot.ConsoleApp.SlackPlugin
{
    public class SpotifyPlugin : IPlugin
    {
        private ISpotifyBase _spotifyBase;

        public PrivateProfile spotifyPrivateProfile { get; set; }

        public FullPlaylist spotifyPlayList { get; set; }

        public SpotifyPlugin(ISpotifyBase spotifyBase)
        {
            this._spotifyBase = spotifyBase;
        }

        public string DeviceId { get; set; }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public PrivateProfile GetSpotifyUser()
        {
            this.GetSetPrivateProfile();
            return this.spotifyPrivateProfile;
        }

        public FullPlaylist CreateNewPlaylist()
        {
            if (this.spotifyPlayList == null)
            {
                string playlistName = GetPlaylistName();
                this.GetSetPrivateProfile();
                this.GetPlaylist(playlistName);
                this.CreatePlaylist(playlistName);
            }

            return this.spotifyPlayList;
        }

        private static string GetPlaylistName()
        {
            return$"EntelectHQ-{DateTime.Now.ToShortDateString()}";
        }


        private void GetSetPrivateProfile()
        {
            if (this.spotifyPrivateProfile == null)
            {
                this.CheckOrRefreshToken();
                PrivateProfile privateProfile = this._spotifyBase.SpotifyWebApi.GetPrivateProfile();
                this.spotifyPrivateProfile = privateProfile;
            }
        }

        private void CreatePlaylist(string playlistName)
        {
            if (this.spotifyPlayList == null)
            {
                this.CheckOrRefreshToken();
                FullPlaylist createdPlaylist = this._spotifyBase.SpotifyWebApi.CreatePlaylist(
                                                                                              this.spotifyPrivateProfile
                                                                                                  .Id,
                                                                                              playlistName,
                                                                                              true
                                                                                             );
                this.spotifyPlayList = createdPlaylist;
            }
        }

        private void GetPlaylist(string playlistName)
        {
            if (this.spotifyPlayList == null)
            {
                this.CheckOrRefreshToken();
                Paging<SimplePlaylist> latestUserPlaylist =
                    this._spotifyBase.SpotifyWebApi.GetUserPlaylists(this.spotifyPrivateProfile.Id);
                foreach (SimplePlaylist playlist in latestUserPlaylist.Items)
                {
                    if (playlist.Name == playlistName)
                    {
                        FullPlaylist getPlaylist = this._spotifyBase.SpotifyWebApi.GetPlaylist(
                                                                                               this
                                                                                                   .spotifyPrivateProfile
                                                                                                   .Id,
                                                                                               playlist.Id,
                                                                                               "",
                                                                                               ""
                                                                                              );
                        this.spotifyPlayList = getPlaylist;
                    }
                }
            }
        }

        private void CheckOrRefreshToken()
        {
            if (this._spotifyBase.spotifyToken.IsExpired())
            {
                this._spotifyBase.RefreshToken();
            }
        }

        public bool AddTrack(string trackUrl)
        {
            this.GetSetPrivateProfile();
            this.CreateNewPlaylist();
            this.CheckOrRefreshToken();
            ErrorResponse test = this._spotifyBase.SpotifyWebApi.AddPlaylistTrack(
                                                                                  this.spotifyPlayList.Id,
                                                                                  this.spotifyPlayList.Id,
                                                                                  trackUrl
                                                                                 );
            return!test.HasError();
        }

        public AvailabeDevices GetSpotifyDevices()
        {
            this.CheckOrRefreshToken();
            AvailabeDevices spotifyDevices = this._spotifyBase.SpotifyWebApi.GetDevices();
            return spotifyDevices;
        }

        public void SetPlaybackDevice(string deviceId)
        {
            this.DeviceId = deviceId;
        }

        public bool PlayPlaylist()
        {
            bool startedPlaying = false;
            if (!this.DeviceId.IsNullOrWhiteSpace())
            {
                ErrorResponse playResponse =
                    this._spotifyBase.SpotifyWebApi.ResumePlayback(this.DeviceId, this.spotifyPlayList.Uri, null, "");

                if (!playResponse.HasError())
                {
                    startedPlaying = true;
                }
            }

            return startedPlaying;
        }
    }
}