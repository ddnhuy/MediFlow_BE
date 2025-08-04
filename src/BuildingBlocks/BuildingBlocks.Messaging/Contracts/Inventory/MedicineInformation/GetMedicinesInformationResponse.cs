using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation
{
    public class GetMedicinesInformationResponse
    {
        public List<GetMedicineInformationResponse> Medicines { get; set; } = new();
    }
}
