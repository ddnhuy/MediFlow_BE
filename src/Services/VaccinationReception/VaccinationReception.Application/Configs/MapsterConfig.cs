using CustomerInfo.Grpc.Protos;
using Google.Protobuf.WellKnownTypes;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.DTOs.PatientDTOs;
using VaccinationReception.Application.Patients.Commands.CreatePatient;
using VaccinationReception.Application.Patients.Commands.UpdatePatient;

namespace VaccinationReception.Application.Configs
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<PatientSummaryModel, PatientSummaryDTO>()
              .Map(dest => dest.DOB, src => src.Dob != null ? src.Dob.ToDateTime() : DateTime.MinValue)
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.Code, src => src.Code)
              .Map(dest => dest.Name, src => src.Name)
              .Map(dest => dest.Gender, src => src.Gender)
              .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
              .Map(dest => dest.IdentityCard, src => src.IdentityCard)
              .Map(dest => dest.AddressDetail, src => src.AddressDetail)
              .Map(dest => dest.Province, src => src.Province)
              .Map(dest => dest.District, src => src.District)
              .Map(dest => dest.Ward, src => src.Ward)
              .Map(dest => dest.IsPregnant, src => src.IsPregnant)
              .Map(dest => dest.IsForeigner, src => src.IsForeigner);

            config.NewConfig<PatientDetailModel, PatientDetailDTO>()
            .Map(dest => dest.DOB, src => src.Dob != null ? src.Dob.ToDateTime() : DateTime.MinValue)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt != null ? src.CreatedAt.ToDateTime() : DateTime.MinValue)
            .Map(dest => dest.LastUpdatedAt, src => src.LastUpdatedAt != null ? src.LastUpdatedAt.ToDateTime() : DateTime.MinValue)
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.IdentityCard, src => src.IdentityCard)
            .Map(dest => dest.AddressDetail, src => src.AddressDetail)
            .Map(dest => dest.Province, src => src.Province)
            .Map(dest => dest.District, src => src.District)
            .Map(dest => dest.Ward, src => src.Ward)
            .Map(dest => dest.CreatedBy, src => src.CreatedBy)
            .Map(dest => dest.LastUpdatedBy, src => src.LastUpdatedBy)
            .Map(dest => dest.IsPregnant, src => src.IsPregnant)
            .Map(dest => dest.IsForeigner, src => src.IsForeigner)
            .Map(dest => dest.IsSuspended, src => src.IsSuspended)
            .Map(dest => dest.IsCancelled, src => src.IsCancelled);

            config.NewConfig<CreatePatientCommand, CreatePatientRequest>()
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.IdentityCard, src => src.IdentityCard)
            .Map(dest => dest.Dob, src => Timestamp.FromDateTime(src.Dob.ToUniversalTime()))
            .Map(dest => dest.AddressDetail, src => src.AddressDetail)
            .Map(dest => dest.Province, src => src.Province)
            .Map(dest => dest.District, src => src.District)
            .Map(dest => dest.Ward, src => src.Ward)
            .Map(dest => dest.IsPregnant, src => src.IsPregnant)
            .Map(dest => dest.IsForeigner, src => src.IsForeigner)
            .Map(dest => dest.IsSuspended, src => src.IsSuspended)
            .Map(dest => dest.IsCancelled, src => src.IsCancelled);

            config.NewConfig<UpdatePatientCommand, UpdatePatientRequest>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Gender, src => src.Gender)
            .Map(dest => dest.Dob, src => Timestamp.FromDateTime(src.Dob.ToUniversalTime()))
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.IdentityCard, src => src.IdentityCard)
            .Map(dest => dest.AddressDetail, src => src.AddressDetail)
            .Map(dest => dest.Province, src => src.Province)
            .Map(dest => dest.District, src => src.District)
            .Map(dest => dest.Ward, src => src.Ward)
            .Map(dest => dest.IsPregnant, src => src.IsPregnant)
            .Map(dest => dest.IsForeigner, src => src.IsForeigner)
            .Map(dest => dest.IsSuspended, src => src.IsSuspended)
            .Map(dest => dest.IsCancelled, src => src.IsCancelled);
        }
    }
}