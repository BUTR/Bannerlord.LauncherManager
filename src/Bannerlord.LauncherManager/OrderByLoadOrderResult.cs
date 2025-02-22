using Bannerlord.LauncherManager.Models;

using System.Collections.Generic;

namespace Bannerlord.LauncherManager;

public record OrderByLoadOrderResult(bool Result, IReadOnlyList<string>? Issues, IReadOnlyList<IModuleViewModel> OrderedModules);