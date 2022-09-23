using Bannerlord.ModuleManager;

namespace Bannerlord.VortexExtension
{
    public partial class VortexExtensionHandler
    {
        private const int SORTING = 1;
        private const int NOTSORTING = 0;
        public bool IsSorting;
        // if (Interlocked.CompareExchange(ref IsSorting, SORTING, NOTSORTING) == NOTSORTING)

        public void Sort()
        {
            var activeProfile = GetActiveProfile();
            if (string.IsNullOrEmpty(activeProfile.GameId))
            {
                IsSorting = false;
                return;
            }

            var loadOrder = GetLoadOrder();
            var modules = GetFromLoadOrder(loadOrder);
            var sorted = ModuleSorter.Sort(modules);
            SetLoadOrder(GetFromModules(sorted));

            var translated = TranslateString("Finished sorting");
            SendNotification("mnb2-sort-finished", "info", translated, 3000);
        }
    }
}