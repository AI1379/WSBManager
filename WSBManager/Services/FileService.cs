using System.Collections.Generic;
using System.Linq;
using System.Reactive.Joins;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace WSBManager.Services;

public interface IFileService
{
    Task<string?> OpenFileAsync(
        string title = "Open File",
        string extensionsName = "All supported files",
        IEnumerable<string>? extensions = null
    );

    Task<string?> SaveFileAsync(
        string suggestedFileName,
        string title = "Save File",
        string defaultExtension = "*.*"
    );
}

public class FileService : IFileService
{
    private readonly TopLevel _topLevel;

    public FileService(TopLevel topLevel)
    {
        _topLevel = topLevel;
    }

    public async Task<string?> OpenFileAsync(
        string title = "Open File",
        string extensionsName = "All supported files",
        IEnumerable<string>? extensions = null)
    {
        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(extensionsName)
                {
                    Patterns = (extensions ?? ["*.*"]).ToArray()
                }
            ]
        };
        var files = await _topLevel.StorageProvider.OpenFilePickerAsync(options);

        return files.FirstOrDefault()?.Path.LocalPath;
    }

    public async Task<string?> SaveFileAsync(
        string suggestedFileName,
        string title = "Save File",
        string defaultExtension = "*.*"
    )
    {
        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = suggestedFileName,
            DefaultExtension = defaultExtension,
            FileTypeChoices =
            [
                new FilePickerFileType("All supported files")
                {
                    Patterns = [defaultExtension]
                }
            ]
        };

        var file = await _topLevel.StorageProvider.SaveFilePickerAsync(options);

        return file?.Path.LocalPath;
    }
}