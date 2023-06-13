namespace AzureAPI.Models
{
    public class CosmosDB
    {
        public string Account { get; set; }
        public string Key { get; set; }
        public int DatabaseName { get; set; }

        public int ContainerName { get; set; }

    }
}
