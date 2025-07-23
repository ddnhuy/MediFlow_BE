using BuildingBlocks.CQRS;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using VaccinationReception.Application.Helpers;

namespace VaccinationReception.Application.Examinations.Queries
{
    public class GetAllExaminationTechnicianQueryHandler : IQueryHandler<GetAllExaminationTechnicianQuery, GetAllExaminationTechnicianRespone>
    {
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetAllExaminationTechnicianQueryHandler(ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto, IHttpContextAccessor httpContextAccessor)
        {
            _applicationUserProto = applicationUserProto;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetAllExaminationTechnicianRespone> Handle(GetAllExaminationTechnicianQuery request, CancellationToken cancellationToken)
        {
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.TryParse(userIdClaim, out var parsedId) ? parsedId : 1;

            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);

            var technicians = await _applicationUserProto.ListUsersByRoleNameAsync(new ListUsersByRoleNameRequest()
            {
                RoleName = request.RoleName
            }, metadata);

            var response = new GetAllExaminationTechnicianRespone
            (
                ExaminationTechnicians : technicians.Data.Select(u => new ExaminationTechnicianItem(u.Id, u.Name)).ToList()
            );

            return response;
        }
    }
}
