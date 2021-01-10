using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GoogleCast;
using GoogleCast.Channels;
using GoogleCast.Models.Media;

namespace ChromecastServer.Controllers
{
    public class CastController : Controller
    {
        public async Task<IActionResult> Get(string receiver, string[] mi, bool repeat = false, bool random = false)
        {
            if (!IPAddress.TryParse(receiver, out IPAddress address))
            {
                return BadRequest();
            }
            //TODO: Maybe check that address is private for security.

            Receiver receiverObj = new Receiver() { IPEndPoint = new IPEndPoint(address, 8009) };
            Sender sender = new Sender();
            // Connect to the Chromecast
            await sender.ConnectAsync(receiverObj);
            // Launch the default media receiver application
            IMediaChannel mediaChannel = sender.GetChannel<IMediaChannel>();
            await sender.LaunchAsync(mediaChannel);
            IEnumerable<QueueItem> qi = LoadMediaItems(mi);
                if(random)
                {
                    await mediaChannel.QueueLoadAsync(repeat? RepeatMode.RepeatAllAndShuffle:RepeatMode.RepeatOff, qi.OrderBy((_) => Guid.NewGuid()).ToArray());
                }
                else
                {
                    await mediaChannel.QueueLoadAsync(repeat? RepeatMode.RepeatAll:RepeatMode.RepeatOff, qi.ToArray());
                }
            
            return Ok();
        }
        private static IEnumerable<QueueItem> LoadMediaItems(IEnumerable<string> mediaItems)
        {
            return mediaItems.Select((string s) =>
            {
                MediaInfo info = new MediaInfo(s);
                MovieMetadata mm = new MovieMetadata();
                mm.Title = (info.Title + ", " + info.folderName + " " + info.prefix).Trim();
                MediaInformation mi = new MediaInformation()
                {
                    ContentId = s,
                    Metadata = mm,
                };
                QueueItem toReturn = new QueueItem()
                {
                    Autoplay = true,
                    Media = mi
                };
                return toReturn;
            }).ToArray();
        }
        private struct MediaInfo
        {
            public string fileName;
            public string folderName;
            public string prefix;
            public string Title;
            public MediaInfo(string filePath)
            {
                //Tries to guess media info based on the file name and path.
                //Shouldn't be a big deal if it's wrong, but nice if it's right.
                fileName = Path.GetFileNameWithoutExtension(filePath);
                folderName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(filePath));
                int splitpoint = fileName.IndexOf('-');
                if (splitpoint >= 0)
                {
                    prefix = fileName.Substring(0, splitpoint).Trim();
                    Title = fileName.Substring(splitpoint + 1).Trim();
                }
                else
                {
                    Title = fileName.Trim();
                    prefix = "";
                }

            }
        }
    }
}