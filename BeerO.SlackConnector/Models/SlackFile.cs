﻿using System;

namespace BeerO.SlackConnector.Models
{
    public class SlackFile
    {
        public string Id { get; }
        public int Created { get; }
        public int Timestamp { get; }
        public string Name { get; }
        public string Title { get; }
        public string Mimetype { get; }
        public string FileType { get; }
        public string PrettyType { get; }
        public string User { get; }
        public bool Editable { get; }
        public int Size { get; }
        public string Mode { get; }
        public bool IsExternal { get; }
        public string ExternalType { get; }
        public bool IsPublic { get; }
        public bool PublicUrlShared { get; }
        public bool DisplayAsBot { get; }
        public string Username { get; }
        public Uri UrlPrivate { get; }
        public Uri UrlPrivateDownload { get; }
        public int ImageExifRotation { get; }
        public int OriginalWidth { get; }
        public int OriginalHeight { get; }
        public Uri DeanimateGif { get; }
        public Uri Permalink { get; }
        public Uri PermalinkPublic { get; }
        public SlackThumbnail Thumbnail { get; }

        public SlackFile(
            string id,
            int created,
            int timestamp,
            string name,
            string title,
            string mimetype,
            string fileType,
            string prettyType,
            string user,
            bool editable,
            int size,
            string mode,
            bool isExternal,
            string externalType,
            bool isPublic,
            bool publicUrlShared,
            bool displayAsBot,
            string username,
            Uri urlPrivate,
            Uri urlPrivateDownload,
            int imageExifRotation,
            int originalWidth,
            int originalHeight,
            Uri deanimateGif,
            Uri permalink,
            Uri permalinkPublic,
            SlackThumbnail thumbnail)
        {
            this.Id = id;
            this.Created = created;
            this.Timestamp = timestamp;
            this.Name = name;
            this.Title = title;
            this.Mimetype = mimetype;
            this.FileType = fileType;
            this.PrettyType = prettyType;
            this.User = user;
            this.Editable = editable;
            this.Size = size;
            this.Mode = mode;
            this.IsExternal = isExternal;
            this.ExternalType = externalType;
            this.IsPublic = isPublic;
            this.PublicUrlShared = publicUrlShared;
            this.DisplayAsBot = displayAsBot;
            this.Username = username;
            this.UrlPrivate = urlPrivate;
            this.UrlPrivateDownload = urlPrivateDownload;
            this.ImageExifRotation = imageExifRotation;
            this.OriginalWidth = originalWidth;
            this.OriginalHeight = originalHeight;
            this.DeanimateGif = deanimateGif;
            this.Permalink = permalink;
            this.PermalinkPublic = permalinkPublic;
            this.Thumbnail = thumbnail;
        }
    }
}