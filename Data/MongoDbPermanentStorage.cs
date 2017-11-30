using EPiServer.Forms.Core.Data;
using EPiServer.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using EPiServer.Data.Dynamic;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Core;
using EPiServer.Forms;
using EPiServer.ServiceLocation;
using EPiServer.Data;
using EPiServer.Forms.Core.Data.Internal;

namespace EPiServer.Forms.Demo.Data
{
    /// <summary>
    /// To use MongDB storage you need to modify the Forms.config as below:
    /// 
    /// ...
    /// <storage defaultProvider="MongoDbPermanentStorage">
    ///  <providers>
    ///    <add name = "MongoDbPermanentStorage" type="EPiServer.Forms.Demo.Data.MongoDbPermanentStorage, EPiServer.Forms.Demo" connectionUri="mongodb://{username}:{password}@{host}:{port}" databaseName="{database}" />
    ///    <!--
    ///    Example:
    ///    <add name = "MongoDbPermanentStorage" type="EPiServer.Forms.Demo.Data.MongoDbPermanentStorage, EPiServer.Forms.Demo" connectionUri="mongodb://root:root@127.0.0.1:27017" databaseName="FormsDb" />
    ///    -->
    ///  </providers>
    /// </storage>
    /// ...
    ///
    /// </summary>

    [ServiceConfiguration(ServiceType = typeof(IPermanentStorage))]
    public class MongoDbPermanentStorage: PermanentStorage
    {
        private string _connectionUri = string.Empty;
        private string _databaseName = string.Empty;

        private MongoClient _client = null;
        private IMongoDatabase _database = null;

        private static object _lock = new object();
        private static readonly ILogger _logger = LogManager.GetLogger(typeof(MongoDbPermanentStorage));

        private Injected<IEPiServerFormsCoreConfig> _config;
        private Injected<DdsStructureStorage> _structureStorage;

        public MongoDbPermanentStorage()
        {
            var formsCoreConfig = _config.Service as EPiServerFormsCoreConfig;
            var providerSettings = formsCoreConfig.DefaultStorageProvider;

            _connectionUri = providerSettings.Parameters["connectionUri"];
            _databaseName = providerSettings.Parameters["databaseName"];

            lock (_lock)
            {
                _client = new MongoClient(_connectionUri);
                _database = _client.GetDatabase(_databaseName);
            }
        }

        /// <summary>
        /// Get collection associated with specified Form.
        /// </summary>
        /// <param name="formIden"></param>
        /// <returns></returns>
        protected virtual IMongoCollection<Submission> GetFormCollection(FormIdentity formIden)
        {
            var collectionName = string.Format("{0}{1}", Constants.StoreNamePrefix, formIden.Guid);
            IMongoCollection<Submission> collection = null;

            lock (_lock)
            {
                if (!IsCollectionExisted(_database, collectionName))
                {
                    _logger.Debug("Collection is not existed [{0}]. Create new one.", collectionName);
                    _database.CreateCollection(collectionName);
                    collection = _database.GetCollection<Submission>(collectionName);

                    // create indexes for the form collection
                    collection.Indexes.CreateOne(Builders<Submission>.IndexKeys.Ascending(s => s.Id));
                    collection.Indexes.CreateOne(Builders<Submission>.IndexKeys.Ascending(s => s.Data[Constants.SYSTEMCOLUMN_SubmitTime]));
                }
                else
                {
                    collection = _database.GetCollection<Submission>(collectionName);
                }
            }

            return collection;
        }

        /// <summary>
        /// Check if a collection existed in a database.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        protected virtual bool IsCollectionExisted(IMongoDatabase database, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collectionCursor = database.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collectionCursor != null && collectionCursor.Any();
        }

        /// <summary>
        /// Save new submission into storage.
        /// </summary>
        public override Guid SaveToStorage(FormIdentity formIden, Submission submission)
        {
            var collection = GetFormCollection(formIden);
            var submissionId = Guid.NewGuid();
            submission.Id = submissionId.ToString();
            collection.InsertOne(submission);

            return submissionId;
        }

        /// <summary>
        /// Update a submission associated with a Guid.
        /// </summary>
        public override Guid UpdateToStorage(Guid formSubmissionId, FormIdentity formIden, Submission submission)
        {
            var collection = GetFormCollection(formIden);

            var updateFilter = Builders<Submission>.Filter.Eq(s => s.Id, formSubmissionId.ToString());
            var update = Builders<Submission>.Update.Set(s => s.Id, formSubmissionId.ToString());
            foreach (var item in submission.Data)
            {
                update = update.Set(s => s.Data[item.Key], item.Value);
            }
            collection.UpdateOne(updateFilter, update);

            return formSubmissionId;
        }

        /// <summary>
        /// Load submissions in a date time range.
        /// </summary>
        public override IEnumerable<PropertyBag> LoadSubmissionFromStorage(FormIdentity formIden, DateTime beginDate, DateTime endDate, bool finalizedOnly = false)
        {
            var collection = GetFormCollection(formIden);
            var filter = Builders<Submission>.Filter.Gte(s => s.Data[Constants.SYSTEMCOLUMN_SubmitTime], beginDate)
                & Builders<Submission>.Filter.Lte(s => s.Data[Constants.SYSTEMCOLUMN_SubmitTime], endDate);
            var submissions = collection.Find(filter).ToEnumerable();
            
            if (finalizedOnly)
            {
                submissions = submissions.Where(s => s.Data.ContainsKey(Constants.SYSTEMCOLUMN_FinalizedSubmission)
                                                     && s.Data[Constants.SYSTEMCOLUMN_FinalizedSubmission].ToString().ToLower() == Constants.TrueAsStringLower);
            }

            if (!string.IsNullOrEmpty(formIden.Language))
            {
                submissions = submissions.Where(s => s.Data[Constants.SYSTEMCOLUMN_Language].ToString() == formIden.Language);
            }

            var allKeys = GetAllFields(formIden);
            return submissions.Select(s =>
            {
                EnsureFormFieldsExistInSubmissionData(ref s, allKeys);

                var bag = new PropertyBag();
                bag.Id = Identity.NewIdentity(Guid.Parse(s.Id));
                bag.Add(s.Data);
                return bag;
            });
        }

        /// <summary>
        /// Return distinct keys of a list submissions.
        /// </summary>
        protected virtual IEnumerable<string> GetAllFields(FormIdentity formIden)
        {
            var formStructure = _structureStorage.Service.GetStructure(formIden.Guid);
            return formStructure != null ? formStructure.AllFields : new string[] { };
        }

        /// <summary>
        /// Ensure submission data contains all existing keys in the collection.
        /// </summary>
        protected virtual void EnsureFormFieldsExistInSubmissionData(ref Submission submission, IEnumerable<string> allKeys)
        {
            var diffKeys = allKeys.Except(submission.Data.Keys);
            foreach (var key in diffKeys)
            {
                submission.Data.Add(key, null);
            }
        }

        /// <summary>
        /// Load list of submissions.
        /// </summary>
        public override IEnumerable<PropertyBag> LoadSubmissionFromStorage(FormIdentity formIden, string[] submissionIds)
        {
            var collection = GetFormCollection(formIden);
            var externalIds = new List<string>();
            foreach (var submissionId in submissionIds)
            {
                Identity parsedId;
                var externalId = submissionId.Contains(':') && Identity.TryParse(submissionId, out parsedId) ? // a full Identity string pattern = [long]:[Guid] ?
                        parsedId.ExternalId.ToString()
                        : submissionId; // or just has External Id
                externalIds.Add(externalId);
            }

            var filter = Builders<Submission>.Filter.In(s => s.Id, externalIds);
            var submissions = collection.Find(filter);
            var allKeys = GetAllFields(formIden);

            return submissions.ToEnumerable().Select(s =>
            {
                EnsureFormFieldsExistInSubmissionData(ref s, allKeys);

                var bag = new PropertyBag();
                bag.Id = Identity.NewIdentity(Guid.Parse(s.Id));
                bag.Add(s.Data);
                return bag;
            });
        }

        /// <summary>
        /// Delete a submission from storage.
        /// </summary>
        public override void Delete(FormIdentity formIden, string submissionId)
        {
            var externalId = Identity.Parse(submissionId).ExternalId;
            var collection = GetFormCollection(formIden);
            var filter = Builders<Submission>.Filter.Eq(s => s.Id, externalId.ToString());
            collection.DeleteOne(filter);
        }
    }
}