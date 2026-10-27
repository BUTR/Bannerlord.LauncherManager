using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.Wasm.Adapters;

/// <summary>
/// The callbacks of a LauncherManager instance, implemented by the TS wrapper, which finds the instance by its handler id.
/// Paths use '/', lists and objects are JSON.
/// </summary>
internal static partial class JsCallbacks
{
    private const string Module = "launchermanager";

    [JSImport("setGameParameters", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.Void>>]
    public static partial Task SetGameParameters(int handler, string executable, string gameParameters);

    [JSImport("sendNotification", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.Void>>]
    public static partial Task SendNotification(int handler, string id, string type, string message, double displayMs);

    [JSImport("sendDialog", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> SendDialog(int handler, string type, string title, string message, string filters);

    [JSImport("getInstallPath", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> GetInstallPath(int handler);

    /// <returns>A Uint8Array, null when the file can't be read</returns>
    [JSImport("readFileContent", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.Any>>]
    public static partial Task<object?> ReadFileContent(int handler, string filePath, int offset, int length);

    [JSImport("writeFileContent", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.Void>>]
    public static partial Task WriteFileContent(int handler, string filePath, byte[]? data);

    [JSImport("readDirectoryFileList", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> ReadDirectoryFileList(int handler, string directoryPath);

    [JSImport("readDirectoryList", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> ReadDirectoryList(int handler, string directoryPath);

    [JSImport("getAllModuleViewModels", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> GetAllModuleViewModels(int handler);

    [JSImport("getModuleViewModels", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> GetModuleViewModels(int handler);

    [JSImport("setModuleViewModels", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.Void>>]
    public static partial Task SetModuleViewModels(int handler, string moduleViewModels);

    [JSImport("getOptions", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> GetOptions(int handler);

    [JSImport("getState", Module)]
    [return: JSMarshalAs<JSType.Promise<JSType.String>>]
    public static partial Task<string?> GetState(int handler);
}
