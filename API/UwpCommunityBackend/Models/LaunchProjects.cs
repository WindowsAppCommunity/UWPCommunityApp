using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace UwpCommunityBackend.Models
{
    public class LaunchProjects
    {
        /// <summary>
        /// Returns the number of private projects
        /// </summary>
        [JsonProperty(PropertyName = "privateCount")]
        public int PrivateCount { get; set; }

        [JsonProperty(PropertyName = "projects")]
        public List<Project> Projects { get; set; }
    }
}
