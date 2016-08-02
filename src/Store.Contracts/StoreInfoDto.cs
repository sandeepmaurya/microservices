namespace Store.Contracts
{
    public class StoreInfoDto
    {
        public string StoreId { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
    }
}
