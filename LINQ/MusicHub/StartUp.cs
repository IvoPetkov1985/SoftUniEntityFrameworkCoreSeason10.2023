namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            int searchedDurationInS = int.Parse(Console.ReadLine());
            Console.WriteLine(ExportSongsAboveDuration(context, searchedDurationInS));
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumsInfo = context.Producers
                .Include(x => x.Albums)
                    .ThenInclude(x => x.Songs)
                    .ThenInclude(x => x.Writer)
                .First(x => x.Id == producerId)
                .Albums
                .Select(a => new
                {
                    AlbumName = a.Name,
                    AlbumReleaseDate = a.ReleaseDate,
                    ProducerName = a.Producer.Name,
                    AlbumSongs = a.Songs
                        .Select(s => new
                        {
                            SongPrice = s.Price,
                            SongName = s.Name,
                            WriterName = s.Writer.Name
                        })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.WriterName)
                        .ToArray(),

                    AlbumPrice = a.Price
                })
                .OrderByDescending(a => a.AlbumPrice)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var album in albumsInfo)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.AlbumReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine("-Songs:");

                int counter = 1;

                foreach (var song in album.AlbumSongs)
                {
                    sb.AppendLine($"---#{counter++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:F2}");
                    sb.AppendLine($"---Writer: {song.WriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        //{
        //    var span = new TimeSpan(0, 0, duration);
        //    var songsAboveDuration = context
        //        .Songs
        //        .Where(s => s.Duration > span)
        //        .Select(s => new
        //        {
        //            SongName = s.Name,
        //            PerformerFullName = s.SongPerformers
        //                .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
        //                .OrderBy(name => name)
        //                .ToList(),
        //            WriterName = s.Writer.Name,
        //            AlbumProducerName = s.Album.Producer.Name,
        //            Duration = s.Duration.ToString("c")
        //        })
        //        .OrderBy(s => s.SongName)
        //        .ThenBy(s => s.WriterName)
        //        .ToList();

        //    StringBuilder sb = new StringBuilder();
        //    int counter = 1;

        //    foreach (var s in songsAboveDuration)
        //    {
        //        sb
        //            .AppendLine($"-Song #{counter++}")
        //            .AppendLine($"---SongName: {s.SongName}")
        //            .AppendLine($"---Writer: {s.WriterName}");

        //        if (s.PerformerFullName.Any())
        //        {
        //            sb.AppendLine(string
        //                .Join(Environment.NewLine, s.PerformerFullName
        //                    .Select(p => $"---Performer: {p}")));
        //        }

        //        sb
        //            .AppendLine($"---AlbumProducer: {s.AlbumProducerName}")
        //            .AppendLine($"---Duration: {s.Duration}");
        //    }

        //    return sb.ToString().TrimEnd();
        //}

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            TimeSpan span = new TimeSpan(0, 0, duration);

            var songs = context.Songs
                .Where(s => s.Duration > span)
                .Select(s => new
                {
                    SongName = s.Name,
                    PerfFullNames = s.SongPerformers
                        .Select(p => p.Performer.FirstName + " " + p.Performer.LastName)
                        .OrderBy(name => name)
                        .AsEnumerable(),
                    WriterName = s.Writer.Name,
                    AlbumProd = s.Album.Producer.Name,
                    SongDuration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .AsEnumerable();

            StringBuilder sb = new StringBuilder();

            int counter = 1;

            foreach (var song in songs)
            {
                sb.AppendLine($"-Song #{counter++}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                if (song.PerfFullNames.Any())
                {
                    sb.AppendLine(string.Join(Environment.NewLine, song.PerfFullNames.Select(p => $"---Performer: {p}")));
                }

                sb.AppendLine($"---AlbumProducer: {song.AlbumProd}");
                sb.AppendLine($"---Duration: {song.SongDuration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
