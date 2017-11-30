//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using EPiServer.Forms.EditView.DataTransfer;
//using EPiServer.ServiceLocation;
//using System.Data;
//using System.IO;

//namespace EPiServer.Forms.Demo.Implementation
//{   
//    /// <summary>
//    /// Export EPiForm SubmissionData to TXT
//    /// </summary>
//    [ServiceConfiguration(ServiceType = typeof(DataExporterBase))]
//    public class TxtDataExporter : DataExporterBase
//    {
//        public override string Export(DataTable dataTable)
//        {
//            var writer = new StringWriter();

//            foreach (DataRow row in dataTable.Rows)
//            {
//                foreach (DataColumn col in dataTable.Columns)
//                {
//                    writer.WriteLine("{0}: {1}", col.ColumnName, row[col]);
//                }
//                writer.WriteLine();
//            }

//            return writer.ToString();
//        }

//        /// <inheritdoc />
//        public override string MimeType
//        {
//            get { return "text/plain"; }
//        }
//        public override string ExportFileExtension
//        {
//            get { return "txt"; }
//        }

//        /// <inheritdoc />
//        public override string Name
//        {
//            get { return "TXT"; }
//        }
        
//    }
//}
