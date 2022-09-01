using CommandsService.Models;

namespace CommandsService.SyncDataServices.Grps
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}