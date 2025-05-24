using CustomerInfo.Grpc.Models;
using CustomerInfo.Grpc.Protos;
using Mapster;
using Google.Protobuf.WellKnownTypes;

namespace CustomerInfo.Grpc.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // CreatePatientRequest -> Patient
            config.NewConfig<CreatePatientRequest, Patient>()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.DOB, src => src.Dob.ToDateTime())
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.IdentityCard, src => src.IdentityCard)
                .Map(dest => dest.AddressDetail, src => src.AddressDetail)
                .Map(dest => dest.Province, src => src.Province)
                .Map(dest => dest.District, src => src.District)
                .Map(dest => dest.Ward, src => src.Ward)
                .Map(dest => dest.IsPregnant, src => src.IsPregnant)
                .Map(dest => dest.IsForeigner, src => src.IsForeigner)
                .IgnoreNullValues(true);

            // UpdatePatientRequest -> Patient
            config.NewConfig<UpdatePatientRequest, Patient>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.DOB, src => src.Dob.ToDateTime())
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.IdentityCard, src => src.IdentityCard)
                .Map(dest => dest.AddressDetail, src => src.AddressDetail)
                .Map(dest => dest.Province, src => src.Province)
                .Map(dest => dest.District, src => src.District)
                .Map(dest => dest.Ward, src => src.Ward)
                .Map(dest => dest.IsPregnant, src => src.IsPregnant)
                .Map(dest => dest.IsForeigner, src => src.IsForeigner)
                .IgnoreNullValues(true);

            // Patient -> PatientDetailModel
            config.NewConfig<Patient, PatientDetailModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Dob, src => Timestamp.FromDateTime(src.DOB.ToUniversalTime()))
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.IdentityCard, src => src.IdentityCard)
                .Map(dest => dest.AddressDetail, src => src.AddressDetail)
                .Map(dest => dest.Province, src => src.Province)
                .Map(dest => dest.District, src => src.District)
                .Map(dest => dest.Ward, src => src.Ward)
                .Map(dest => dest.IsPregnant, src => src.IsPregnant)
                .Map(dest => dest.IsForeigner, src => src.IsForeigner)
                .Map(dest => dest.IsSuspended, src => src.IsSuspended)
                .Map(dest => dest.IsCancelled, src => src.IsCancelled)
                .Map(dest => dest.CreatedAt, src => Timestamp.FromDateTime(src.CreatedAt.ToUniversalTime()))
                .Map(dest => dest.CreatedBy, src => src.CreatedBy)
                .Map(dest => dest.LastUpdatedAt, src => Timestamp.FromDateTime(src.LastUpdatedAt.ToUniversalTime()))
                .Map(dest => dest.LastUpdatedBy, src => src.LastUpdatedBy);

            // Patient -> PatientSummaryModel
            config.NewConfig<Patient, PatientSummaryModel>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.Dob, src => Timestamp.FromDateTime(src.DOB.ToUniversalTime()))
                .Map(dest => dest.IdentityCard, src => src.IdentityCard)
                .Map(dest => dest.AddressDetail, src => src.AddressDetail)
                .Map(dest => dest.Province, src => src.Province)
                .Map(dest => dest.District, src => src.District)
                .Map(dest => dest.Ward, src => src.Ward)
                .Map(dest => dest.IsPregnant, src => src.IsPregnant)
                .Map(dest => dest.IsForeigner, src => src.IsForeigner);
        }
    }
}