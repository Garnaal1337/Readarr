﻿using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Tv;
using NzbDrone.Core.Download;
using NzbDrone.Core.Configuration;

namespace NzbDrone.Core.Test.Download.DownloadClientTests
{
    public abstract class DownloadClientFixtureBase<TSubject> : CoreTest<TSubject>
        where TSubject : class, IDownloadClient
    {
        protected readonly string _title = "Droned.S01E01.Pilot.1080p.WEB-DL-DRONE";
        protected readonly string _downloadUrl = "http://somewhere.com/Droned.S01E01.Pilot.1080p.WEB-DL-DRONE.ext";

        [SetUp]
        public void SetupBase()
        {
            Mocker.GetMock<IConfigService>()
                .SetupGet(s => s.DownloadClientHistoryLimit)
                .Returns(30);

            Mocker.GetMock<IParsingService>()
                .Setup(s => s.Map(It.IsAny<ParsedEpisodeInfo>(), It.IsAny<int>(), null))
                .Returns(CreateRemoteEpisode());
        }

        protected virtual RemoteEpisode CreateRemoteEpisode()
        {
            var remoteEpisode = new RemoteEpisode();
            remoteEpisode.Release = new ReleaseInfo();
            remoteEpisode.Release.Title = _title;
            remoteEpisode.Release.DownloadUrl = _downloadUrl;
            remoteEpisode.Release.DownloadProtocol = Subject.Protocol;

            remoteEpisode.ParsedEpisodeInfo = new ParsedEpisodeInfo();
            remoteEpisode.ParsedEpisodeInfo.FullSeason = false;

            remoteEpisode.Episodes = new List<Episode>();

            remoteEpisode.Series = new Series();

            return remoteEpisode;
        }

        protected void VerifyIdentifiable(DownloadClientItem downloadClientItem)
        {
            downloadClientItem.DownloadClient.Should().Be(Subject.Definition.Name);
            downloadClientItem.DownloadClientId.Should().NotBeNullOrEmpty();

            downloadClientItem.Title.Should().NotBeNullOrEmpty();

            downloadClientItem.RemoteEpisode.Should().NotBeNull();

        }

        protected void VerifyQueued(DownloadClientItem downloadClientItem)
        {
            VerifyIdentifiable(downloadClientItem);
            downloadClientItem.RemainingSize.Should().NotBe(0);
            //downloadClientItem.RemainingTime.Should().NotBe(TimeSpan.Zero);
            //downloadClientItem.OutputPath.Should().NotBeNullOrEmpty();
            downloadClientItem.Status.Should().Be(DownloadItemStatus.Queued);
        }

        protected void VerifyPaused(DownloadClientItem downloadClientItem)
        {
            VerifyIdentifiable(downloadClientItem);

            downloadClientItem.RemainingSize.Should().NotBe(0);
            //downloadClientItem.RemainingTime.Should().NotBe(TimeSpan.Zero);
            //downloadClientItem.OutputPath.Should().NotBeNullOrEmpty();
            downloadClientItem.Status.Should().Be(DownloadItemStatus.Paused);
        }

        protected void VerifyDownloading(DownloadClientItem downloadClientItem)
        {
            VerifyIdentifiable(downloadClientItem);

            downloadClientItem.RemainingSize.Should().NotBe(0);
            //downloadClientItem.RemainingTime.Should().NotBe(TimeSpan.Zero);
            //downloadClientItem.OutputPath.Should().NotBeNullOrEmpty();
            downloadClientItem.Status.Should().Be(DownloadItemStatus.Downloading);
        }

        protected void VerifyCompleted(DownloadClientItem downloadClientItem)
        {
            VerifyIdentifiable(downloadClientItem);

            downloadClientItem.Title.Should().NotBeNullOrEmpty();
            downloadClientItem.RemainingSize.Should().Be(0);
            downloadClientItem.RemainingTime.Should().Be(TimeSpan.Zero);
            //downloadClientItem.OutputPath.Should().NotBeNullOrEmpty();
            downloadClientItem.Status.Should().Be(DownloadItemStatus.Completed);
        }

        protected void VerifyFailed(DownloadClientItem downloadClientItem)
        {
            VerifyIdentifiable(downloadClientItem);

            downloadClientItem.Status.Should().Be(DownloadItemStatus.Failed);
        }
    }
}