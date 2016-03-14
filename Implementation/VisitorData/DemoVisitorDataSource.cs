using EPiServer.Forms.Core.VisitorData;
using EPiServer.Forms.Implementation.VisitorData;
using EPiServer.ServiceLocation;
using System.Collections.Generic;

namespace EPiServer.Forms.Demo.Implementation.VisitorData
{
    /// <summary>
    /// This will add one more VisitorDataSource into the HiddenVisitorProfiling
    /// </summary>
    [ServiceConfiguration(ServiceType = typeof(IVisitorDataSource))]
    public class DemoVisitorDataSource : VisitorDataSourceBase
    {
        /// <inheritdoc />
        public override void SetData(ref object visitorData, string property)
        {
            visitorData = "Demo Visitor Data";
        }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            return new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("demo_visitor_data_item_1", "Demo Visitor Data - Item #1"),
                new KeyValuePair<string, string>("demo_visitor_data_item_2", "Demo Visitor Data - Item #2")
            };
        }
    }
}