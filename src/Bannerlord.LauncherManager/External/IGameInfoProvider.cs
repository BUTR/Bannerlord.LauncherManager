using System.Threading.Tasks;

namespace Bannerlord.LauncherManager.External;

public interface IGameInfoProvider
{
    Task<string> GetInstallPathAsync();
}