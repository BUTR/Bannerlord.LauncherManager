using System.Collections.Generic;

namespace Bannerlord.LauncherManager.Utils;

public sealed record ChangeModulePositionResult(bool Success, IReadOnlyCollection<string> Issues);