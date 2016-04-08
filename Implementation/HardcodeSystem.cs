using EPiServer.Forms.Core.Internal.Autofill;
using EPiServer.Forms.Core.Internal.ExternalSystem;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Forms.Demo.Implementation
{
    /// <summary>
    /// Always autofill FormElement with "Hanoi"
    /// </summary>
    public class HardcodeSystem : IExternalSystem, IAutofillProvider
    {
        public virtual string Id
        {
            get { return "HardcodeSystem"; }
        }

        public virtual IEnumerable<IDatasource> DataSources
        {
            get
            {
                var ds1 = new Datasource
                {
                    Id = "HardcodeSystemDS",
                    Name = "Hardcode System Datasource",
                    OwnerSystem = this
                };

                return new[] { ds1 };
            }
        }

        /// <summary>
        /// Returns a list of suggested values by field mapping key.        
        /// </summary>
        /// <param name="fieldMappingKeys">List of field mapping keys</param>
        /// <returns>Collection of suggested value</returns>
        public virtual IEnumerable<string> GetSuggestedValues(IDatasource selectedDatasource, IEnumerable<RemoteFieldInfo> remoteFieldInfos)
        {
            if (selectedDatasource == null || remoteFieldInfos == null)
            {
                return Enumerable.Empty<string>();
            }

            if (this.DataSources.Any(ds => ds.Id == selectedDatasource.Id)  // datasource belong to this system
                && remoteFieldInfos.Any(mi => mi.DatasourceId == selectedDatasource.Id))    // and remoteFieldInfos is for our system datasource
            {
                return new[] { "Hanoi", "Episerver", "Forms" };
            }

            return Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public Dictionary<string, string> GetDatasourceColumns(string datasourceId)
        {
            return new Dictionary<string, string> { { "alwaysHardcode", "always Hardcode" } };
        }
    }
}