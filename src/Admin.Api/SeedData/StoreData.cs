using System.Collections.Generic;
using Store.Contracts;

namespace Admin.Api.SeedData
{
    public static class StoreData
    {
        public static IEnumerable<CityInfoDto> Cities
        {
            get
            {
                return new List<CityInfoDto>
                {
                    new CityInfoDto
                    {
                        Id = 10,
                        Name = "Hyderabad"
                    },
                    new CityInfoDto
                    {
                        Id = 11,
                        Name = "Bangalore"
                    },
                    new CityInfoDto
                    {
                        Id = 12,
                        Name = "Pune"
                    },
                    new CityInfoDto
                    {
                        Id = 13,
                        Name = "Mumbai"
                    }
                };
            }
        }
        public static IEnumerable<StoreInfoDto> Stores
        {
            get
            {
                return new List<StoreInfoDto>
                {
                    new StoreInfoDto
                    {
                        CityId = 10,
                        StoreId = "10-001",
                        Name = "Chandanagar",
                        Address = "Shop No. 32, Some Complex, Chandanagar",
                        CityName = "Hyderabad",
                        State = "Telangana",
                        PinCode = "500049"
                    },
                    new StoreInfoDto
                    {
                        CityId = 10,
                        StoreId = "10-002",
                        Name = "Gachibowli",
                        Address = "Shop No. 32, Some Complex, Gachibowli",
                        CityName = "Hyderabad",
                        State = "Telangana",
                        PinCode = "500032"
                    },
                    new StoreInfoDto
                    {
                        CityId = 11,
                        StoreId = "11-001",
                        Name = "Yeshwantpur",
                        Address = "Shop No. 32, Some Complex, Yeshwantpur",
                        CityName = "Bangalore",
                        State = "Karnataka",
                        PinCode = "560022"
                    },
                    new StoreInfoDto
                    {
                        CityId = 11,
                        StoreId = "11-002",
                        Name = "Bellandur",
                        Address = "Shop No. 32, Some Complex, Bellandur",
                        CityName = "Bangalore",
                        State = "Karnataka",
                        PinCode = "560103"
                    },
                    new StoreInfoDto
                    {
                        CityId = 12,
                        StoreId = "12-001",
                        Name = "Rahatani",
                        Address = "Shop No. 32, Some Complex, Rahatani",
                        CityName = "Pune",
                        State = "Maharashtra",
                        PinCode = "411017"
                    },
                    new StoreInfoDto
                    {
                        CityId = 12,
                        StoreId = "12-002",
                        Name = "Pimple Saudagar",
                        Address = "Shop No. 32, Some Complex, Pimple Saudagar",
                        CityName = "Pune",
                        State = "Maharashtra",
                        PinCode = "411027"
                    },
                    new StoreInfoDto
                    {
                        CityId = 13,
                        StoreId = "13-001",
                        Name = "Borivali West",
                        Address = "Shop No. 32, Some Complex, Borivali West",
                        CityName = "Mumbai",
                        State = "Maharashtra",
                        PinCode = "400092"
                    },
                    new StoreInfoDto
                    {
                        CityId = 13,
                        StoreId = "13-002",
                        Name = "Kharghar",
                        Address = "Shop No. 32, Some Complex, Kharghar",
                        CityName = "Navi Mumbai",
                        State = "Maharashtra",
                        PinCode = "410210"
                    }
                };
            }
        }
    }
}
