using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace OctcordBot.ConfigState {
    public class WritableJsonConfigurationSource : JsonConfigurationSource {
        public override IConfigurationProvider Build(IConfigurationBuilder builder) {
            this.EnsureDefaults(builder);
            return new WritableJsonConfigurationProvider(this);
        }
    }
}
