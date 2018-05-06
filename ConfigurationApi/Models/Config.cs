using System;

namespace ConfigurationApi.Models
{
    public class Config
    {
        public Guid Id { get; set; }
        public string ApplicationName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; }
    }
}
