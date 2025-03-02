using Bannerlord.LauncherManager.Localization;
using Bannerlord.LauncherManager.Models;
using Bannerlord.LauncherManager.Utils;

using System.Linq;
using System.Threading.Tasks;

namespace Bannerlord.LauncherManager;

partial class LauncherManagerHandler
{
    //private const int SORTING = 1;
    // private const int NOTSORTING = 0;
    // if (Interlocked.CompareExchange(ref IsSorting, SORTING, NOTSORTING) == NOTSORTING)
    public bool IsSorting;

    /// <summary>
    /// External<br/>
    /// </summary>
    public async Task SortAsync()
    {
        IsSorting = true;
        var modules = (await GetModuleViewModelsAsync())?.Select(x => x.ModuleInfoExtended) ?? [];
        var sorted = SortHelper.AutoSort(modules);
        var sortedViewModels = await GetViewModelsFromModulesAsync(sorted);
        await SetModuleViewModelsAsync(sortedViewModels);

        await SendNotificationAsync("sort-finished", NotificationType.Info, new BUTRTextObject("{=J7dh36Dy}Finished sorting!").ToString(), 3000);
        IsSorting = false;
    }
}