using EPiServer.Forms.Core.Internal.Autofill;
using EPiServer.Forms.Core.Internal.ExternalSystem;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Forms.Demo.Implementation
{
    /// <summary>
    /// Always autofill FormElement with static suggestion list
    /// </summary>
    public class StaticAutofillSystem : IExternalSystem, IAutofillProvider
    {
        public virtual string Id
        {
            get { return "StaticAutofillSystem"; }
        }

        /// <summary>
        /// This system does not have datasource, because it is static
        /// </summary>
        public virtual IEnumerable<IDatasource> Datasources
        {
            get
            {
                var ds1 = new Datasource()
                {
                    Id = "StaticAutofillDatasource1",
                    Name = "Static Autofill Datasource 1",
                    OwnerSystem = this,
                    Columns = new Dictionary<string, string> { 
                        { "staticds1email", "static ds1 email" },
                        { "staticds1firstname", "static ds1 first name" }
                    }
                };

                var ds2 = new Datasource()
                {
                    Id = "StaticAutofillDatasource2",
                    Name = "Static Autofill Datasource 2",
                    OwnerSystem = this,
                    Columns = new Dictionary<string, string> { 
                        { "staticds2avatar", "static ds2 avatar" },
                        { "staticds2name", "static ds2 name" },
                        { "staticds2bio", "static ds2 Bio" }
                    }
                };

                return new[] { ds1, ds2 };
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

            if (!this.Datasources.Any(ds => ds.Id == selectedDatasource.Id)  // datasource belong to this system
                || !remoteFieldInfos.Any(mi => mi.DatasourceId == selectedDatasource.Id))    // and remoteFieldInfos is for our system datasource
            {
                return Enumerable.Empty<string>();
            }

            var activeRemoteFieldInfo = remoteFieldInfos.FirstOrDefault(mi => mi.DatasourceId == selectedDatasource.Id);
            switch (activeRemoteFieldInfo.ColumnId)
            {
                case "staticds1email":
                    return new List<string> {
                            "tuan@mail.com",
                            "phu@mail.com",
                            "hung@mail.com",
                            "thach@mail.com"
                        };

                case "staticds1firstname":
                    return new List<string> {
                            "Hung",
                            "Phu",
                            "Tuan",
                            "Thach"
                        };

                case "staticds2avatar":
                    return new List<string> {
                            "tuan.png",
                            "thach.jpg"
                        };

                case "staticds2name":
                    return new List<string>{
                        "Tuan Do",
                        "Thach Nguyen",
                        "Hung Phan",
                        "Phu Nguyen"
                        };

                case "staticds2bio":
                    return new List<string> {
                            "I am from Vietnam",
                            "I am from Sweden"
                        };
                default:
                    return Enumerable.Empty<string>();
            }
        }

    }
}