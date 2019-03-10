namespace Hackland.AccessControl
{

    public class ApplicationConfigurationModel
    {
        public bool EnableCreateDefaultUserRoute { get; set; }
        public string CreateDefaultUserUsername { get; set; }
        public string CreateDefaultUserPassword { get; set; }
    }
}
