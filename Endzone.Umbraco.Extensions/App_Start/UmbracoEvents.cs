using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Endzone.Umbraco.Extensions.App_Start
{
    public class UmbracoEvents : ApplicationEventHandler
    {
        private const string deletedMediaPrefix = "deleted---";

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            MediaService.Trashed += ObfuscateDeletedMediaFiles;
            MediaService.Moving += DeobfuscateRestoredMediaFiles;
        }

        private void ObfuscateDeletedMediaFiles(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            foreach (var item in e.MoveInfoCollection)
            {
                UpdateMediaFile(item.Entity, true);
            }
        }

        private void DeobfuscateRestoredMediaFiles(IMediaService sender, MoveEventArgs<IMedia> e)
        {
            foreach (var item in e.MoveInfoCollection)
            {
                if (!item.Entity.Trashed || item.NewParentId == Constants.System.RecycleBinMedia)
                {
                    // only check media items that are moved from the recycling bin
                    continue;
                }

                UpdateMediaFile(item.Entity, false);
            }
        }

        private void UpdateMediaFile(IMedia media, bool obfuscate)
        {
            var file = media.Properties[Constants.Conventions.Media.File];

            var fileSrc = file.Value?.ToString();

            if (DetectIsJson(fileSrc))
            {
                fileSrc = JsonConvert.DeserializeObject<JObject>(fileSrc).GetValue("src")?.ToString();
            }

            var fileDirectory = Path.GetDirectoryName(fileSrc);
            var fileName = Path.GetFileName(fileSrc);

            if (obfuscate && fileName.StartsWith(deletedMediaPrefix))
            {
                // already renamed, nothing to do here
                return;
            }
            else if (!obfuscate && !fileName.StartsWith(deletedMediaPrefix))
            {
                // doesn't have the prefix, nothing to do here
                return;
            }

            var oldFilePath = Path.Combine(fileDirectory, fileName);
            var newFilePath = Path.Combine(fileDirectory, (obfuscate ? deletedMediaPrefix + fileName : fileName.TrimStart(deletedMediaPrefix)));

            var mediaFileSystem = FileSystemProviderManager.Current.GetFileSystemProvider<MediaFileSystem>();

            mediaFileSystem.CopyFile(oldFilePath, newFilePath);
            mediaFileSystem.DeleteFile(oldFilePath);

            file.Value = newFilePath;

            ApplicationContext.Current.Services.MediaService.Save(media, raiseEvents: false);
        }

        private bool DetectIsJson(string input)
        {
            if (input.IsNullOrWhiteSpace())
            {
                return false;
            }

            input = input.Trim();

            return (input.StartsWith("{") && input.EndsWith("}"))
                   || (input.StartsWith("[") && input.EndsWith("]"));
        }
    }
}
