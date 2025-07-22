using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Strings.Enums
{
    public enum ExaminationService
    {
        [Display(Name = "Xét nghiệm máu")]
        Blood,

        [Display(Name = "Xét nghiệm kháng thể viêm gan B ")]
        Anti_HBs
    }
}
