using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepository _repository;
        private readonly IMapper _mapper;

        public GrpcPlatformService(IPlatformRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var plaforms = _repository.GetAllPlatforms();

            foreach (var plat in plaforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(plat));
            }

            return Task.FromResult(response);
        }
    }
}