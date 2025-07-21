using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetAllExaminationOfReceptionQuery(int ReceptionId) : IQuery<GetAllExaminationOfReceptionQueryResponse>;

    public record GetAllExaminationOfReceptionItem(int ExaminationId, string ServiceName);

    public record GetAllExaminationOfReceptionQueryResponse(
        List<GetAllExaminationOfReceptionItem> Examinations
    );
}
