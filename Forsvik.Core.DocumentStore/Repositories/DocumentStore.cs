using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Forsvik.Core.DocumentStore.Contract;
using Forsvik.Core.DocumentStore.Extensions;
using Forsvik.Core.DocumentStore.Models;
using Forsvik.Core.Model.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime.Serialization.SystemTextJson;


namespace Forsvik.Core.DocumentStore.Repositories
{
    public class DocumentStore : IDocumentStore
    {
        private readonly IDocumentDbContext _database;

        private const string TagKey = "Tag";

        private JsonSerializerOptions _serializerOptions;

        public DocumentStore(IDocumentDbContext database)
        {
            _database = database;
            _serializerOptions = new JsonSerializerOptions { Converters = { NodaConverters.LocalDateConverter } };
        }

        public Task<Guid> Save<T>(T item, params Expression<Func<T, object>>[] indexes) where T : class
        {
            var keyValues = ExtractIndex(item, indexes);

            return Save(item, keyValues.ToArray());
        }

        /// <summary>
        /// Save an item with unique combination of properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public async Task<Guid> Save<T>(T item, params KeyValue[] indexes) where T : class
        {
            var keyValues = indexes.ToList();

            var presentId = await ExistsInternal<T>(keyValues);

            var documentId = await PersistDocument(item, presentId);

            if (presentId == null)
            {
                AddIndex(keyValues, documentId);
                await _database.SaveChangesAsync();
            }

            return documentId;
        }

        /// <summary>
        /// Save an item with non unique tags
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public async Task<Guid> Save<T>(T item, params string[] tags) where T : class
        {
            var id = await Save(item);

            foreach (var tag in tags)
            {
                await AddIndex(id, TagKey, tag);
            }

            return id;
        }

        /// <summary>
        /// Save many items in a batch.
        /// Extract search index from item properties
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public async Task<List<Guid>> SaveMany<T>(IEnumerable<T> items, params Expression<Func<T, object>>[] indexes) where T : class
        {
            var ids = new List<Guid>();

            foreach (var item in items)
            {
                var keyValues = ExtractIndex(item, indexes);

                var presentId = await ExistsInternal<T>(keyValues);

                var documentId = await PersistDocument(item, presentId, false);

                if (presentId == null)
                {
                    AddIndex(keyValues, documentId);
                }
                ids.Add(documentId);
            }
            await _database.SaveChangesAsync();

            return ids;
        }

        /// <summary>
        /// Save many items in a batch.
        /// Attach the same tags to each item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="tags"></param>
        /// <param name="dynamicTag"></param>
        /// <returns></returns>
        public async Task<List<Guid>> SaveMany<T>(IEnumerable<T> items, IEnumerable<string> tags, Func<T, string> dynamicTag = null) where T : class
        {
            var ids = new List<Guid>();

            foreach (var item in items)
            {
                var documentId = await PersistDocument(item, null, false);

                foreach (var tag in tags)
                {
                    await AddIndex(documentId, TagKey, tag, false);
                }

                if (dynamicTag != null)
                {
                    await AddIndex(documentId, TagKey, dynamicTag(item), false);
                }

                ids.Add(documentId);
            }
            await _database.SaveChangesAsync();

            return ids;
        }

        public Task<Guid> Save<T>(T data, Guid? id = null)
        {
            return PersistDocument(data, id);
        }

        //public Task<Guid?> Exists<T>(params Expression<Func<T, bool>>[] indexes)
        //{
        //    var searchKeys = indexes
        //        .Select(IndexFromExpression)
        //        .ToList();

        //    return ExistsInternal<T>(searchKeys);
        //}

        public async Task<bool> ExistsIndex(KeyValue index)
        {
            return await _database.DocumentIndexes.AnyAsync(x => x.Key == index.Key && x.Value == index.Value);
        }

        public Task<Guid?> Exists<T>(params KeyValue[] keyValues)
        {
            return ExistsInternal<T>(keyValues);
        }

        public async Task<T> Get<T>(params Expression<Func<T, bool>>[] indexes)
        {
            var searchKeys = indexes
                .Select(IndexFromExpression)
                .ToList();

            var documentId = await ExistsInternal<T>(searchKeys);

            if (documentId == null) return default;

            var document = await _database.Documents.FindAsync(documentId.Value);

            return (T)ConvertToObject<T>(document);
        }

        public async Task<T> Get<T>(params KeyValue[] indexes)
        {
            var documentId = await ExistsInternal<T>(indexes);

            if (documentId == null) return default;

            var document = await _database.Documents.FindAsync(documentId.Value);

            return (T)ConvertToObject<T>(document);
        }

        public Task AddIndex(Guid documentId, string key, string value)
        {
            return AddIndex(documentId, key, value, true);
        }

        public Task AddTag(Guid documentId, string value, bool saveChanges = true)
        {
            return AddIndex(documentId, TagKey, value, saveChanges);
        }

        public Task SaveChanges()
        {
            return _database.SaveChangesAsync();
        }

        private async Task AddIndex(Guid documentId, string key, string value, bool saveChanges)
        {
            _database.DocumentIndexes.Add(new DocumentIndex
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                DocumentId = documentId,
                Key = key,
                Value = value
            });

            try
            {
                if (saveChanges)
                    await _database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<string> GetIndex(Guid documentId, string key)
        {
            var item = await _database
                .DocumentIndexes
                .Where(x => x.DocumentId == documentId)
                .FirstOrDefaultAsync(x => x.Key == key);

            return item?.Value;
        }

        public async Task Remove(Guid id)
        {
            var document = await _database.Documents.FindAsync(id);
            _database.DocumentIndexes.RemoveRange(_database.DocumentIndexes.Where(x => x.DocumentId == id));
            _database.Documents.Remove(document);
            await _database.SaveChangesAsync();
        }

        public async Task<List<KeyValue>> GetIndexes(Guid documentId)
        {
            var indexes = await _database
                .DocumentIndexes
                .Where(x => x.DocumentId == documentId)
                .ToListAsync();

            return indexes.Select(kv => new KeyValue
            {
                Key = kv.Key,
                Value = kv.Value
            }).ToList();
        }

        public async Task RemoveTagRange(IEnumerable<Guid> documentIds, string tag)
        {
            foreach (var batch in documentIds.Batch(2000))
            {
                var items = await _database
                    .DocumentIndexes
                    .Where(x => batch.Contains(x.DocumentId))
                    .Where(x => x.Key == TagKey && x.Value == tag)
                    .ToListAsync();

                _database.DocumentIndexes.RemoveRange(items);
            }

            await _database.SaveChangesAsync();
        }

        public Task<List<Guid>> SearchDocumentIds(params string[] tags)
        {
            var keyValues = tags.Select(tag => new KeyValue { Key = TagKey, Value = tag });

            return CreateIndexQuery(keyValues).ToListAsync();
        }

        public Task<List<T>> Search<T>(params Expression<Func<T, bool>>[] searchProperties)
        {
            var keyValues = searchProperties
                .Select(IndexFromExpression);

            return Search<T>(keyValues.ToArray());
        }

        public Task<List<T>> Search<T>(params string[] tags)
        {
            var keyValues = tags.Select(tag => new KeyValue { Key = TagKey, Value = tag });

            return Search<T>(keyValues.ToArray());
        }

        public async Task<List<T>> Search<T>(params KeyValue[] keyValues)
        {
            var query = CreateIndexQuery(keyValues);

            var documents = await GetDocumentsInternal<T>(query);

            return DeserializeDocuments<T>(documents)
                .ToList();
        }

        public Task<List<DocumentModel<T>>> SearchModel<T>(params string[] tags)
        {
            var keyValues = tags.Select(tag => new KeyValue { Key = TagKey, Value = tag }).ToArray();

            return SearchModel<T>(keyValues);
        }

        public async Task<List<DocumentModel<T>>> SearchModel<T>(params KeyValue[] keyValues)
        {
            var query = CreateIndexQuery(keyValues);

            var documents = await GetDocumentsInternal<T>(query);

            return DeserializeDocumentModels<T>(documents)
                .ToList();
        }

        public async Task<List<object>> SearchObject(params string[] tags)
        {
            var keyValues = tags.Select(tag => new KeyValue { Key = TagKey, Value = tag }).ToArray();

            var ids = CreateIndexQuery(keyValues);

            var documents = await _database
                .Documents
                .Where(item => ids.Contains(item.Id))
                .ToListAsync();

            var objects = new List<object>();

            foreach (var document in documents)
            {
                var types = Assembly.GetEntryAssembly()?.GetExportedTypes();
                var type = types?.FirstOrDefault(x => x.FullName == document.DataType);
                if (type == null) return new List<object>();
                
                objects.Add(JsonSerializer.Deserialize(Encoding.UTF8.GetString(document.Data), type, _serializerOptions));
            }

            return objects;
        }
        

        public async Task<T> Get<T>(Guid id)
        {
            var document = await _database.Documents.FindAsync(id);

            if (document == null) return default;

            return (T)ConvertToObject<T>(document);
        }

        public static void Configure(EntityTypeBuilder<DocumentIndex> builder, string schemaName)
        {
        }

        #region - private -

        private IQueryable<Guid> CreateIndexQuery(IEnumerable<KeyValue> keyValues)
        {
            IQueryable<Guid> outer = null;
            foreach (var keyValue in keyValues)
            {
                string key = keyValue.Key;
                string value = keyValue.Value;

                var queryable = _database.DocumentIndexes
                    .Where(x => x.Key == key && x.Value == value)
                    .OrderBy(x => x.Created)
                    .Select(x => x.DocumentId);

                if (outer == null)
                    outer = queryable;
                else
                    outer = outer.Join(queryable, (left => left), (right => right), (left, right) => left);
            }

            return outer;
        }

        private KeyValue IndexFromExpression<T>(Expression<Func<T, bool>> expression)
        {
            KeyValue index = new KeyValue();

            var body = (BinaryExpression)expression.Body;
            index.Key = ((MemberExpression)body.Left).Member.Name;
            index.Value = GetExpressionValue<T>(body);

            return index.Key != null ? index : null;
        }

        private Document CreateNewDocument(object data)
        {
            return new Document
            {
                Created = DateTime.Now,
                DataType = data.GetType().ToString()
            };
        }

        private void AddIndex(List<KeyValue> keyValues, Guid documentId)
        {
            foreach (var kv in keyValues)
            {
                _database.DocumentIndexes.Add(new DocumentIndex
                {
                    Id = Guid.NewGuid(),
                    DocumentId = documentId,
                    Key = kv.Key,
                    Value = kv.Value,
                    Created = DateTime.Now
                });
            }
        }

        private List<KeyValue> ExtractIndex<T>(T data, params Expression<Func<T, object>>[] indexes)
        {
            if (indexes == null) return new List<KeyValue>();

            var list = new List<KeyValue>();

            foreach (var index in indexes)
            {
                MemberExpression memberExpr = null;

                if (index.Body.NodeType == ExpressionType.Convert)
                {
                    memberExpr = ((UnaryExpression)index.Body).Operand as MemberExpression;
                }
                else if (index.Body.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = index.Body as MemberExpression;
                }

                if (memberExpr == null) throw new Exception("Expression could not be evaluated");

                list.Add(new KeyValue
                {
                    Key = memberExpr?.Member.Name,
                    Value = data.GetType().GetProperty(memberExpr.Member.Name)?.GetValue(data)?.ToString()
                });
            }
            return list;
        }

        private string GetExpressionValue<T>(BinaryExpression prop)
        {
            string target;
            if (prop.Right is ConstantExpression constantExpression)
                target = constantExpression.Value.ToString();
            else if (prop.Right is MethodCallExpression rightExp)
            {
                target = Expression.Lambda(rightExp).Compile().DynamicInvoke()?.ToString();
            }
            else
            {
                var right = (MemberExpression)prop.Right;
                var expression = right.Expression;
                if (expression is ConstantExpression constantExpression3)
                {
                    var obj = constantExpression3.Value;
                    var member = right.Member as FieldInfo;
                    target = !(member != null)
                        ? ((PropertyInfo)right.Member).GetValue(obj)?.ToString()?.Trim()
                        : member.GetValue(obj)?.ToString()?.Trim();
                }
                else
                {
                    var obj = Evaluate(expression);
                    target = ((MemberExpression)prop.Right).Member is FieldInfo
                        ? ((FieldInfo)((MemberExpression)prop.Right).Member).GetValue(obj)?.ToString()?.Trim()
                        : ((PropertyInfo)((MemberExpression)prop.Right).Member).GetValue(obj)?.ToString()?.Trim();
                }
            }
            return !string.IsNullOrEmpty(target)
                ? target
                : throw new ArgumentNullException($"Search argument {prop.Right} cannot be empty");
        }

        private object Evaluate(Expression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Constant:
                    return (e as ConstantExpression)?.Value;

                case ExpressionType.MemberAccess:
                    var memberExpression = e as MemberExpression;
                    var field = memberExpression?.Member as FieldInfo;
                    var property = memberExpression?.Member as PropertyInfo;
                    var obj = memberExpression?.Expression == null
                        ? null
                        : Evaluate(memberExpression.Expression);
                    if (field != null)
                        return field.GetValue(obj);
                    return property != null ? property.GetValue(obj) : null;

                default:
                    throw new Exception("Please simplify your expression");
            }
        }

        private Task<List<Document>> GetDocumentsInternal<T>(IEnumerable<Guid> ids)
        {
            var type = typeof(T).ToString();

            return _database
                .Documents
                .Where(item => item.DataType.StartsWith(type)) // To be optimized into DataType == type
                .Where(item => ids.Contains(item.Id))
                .ToListAsync();
        }

        private IEnumerable<T> DeserializeDocuments<T>(IEnumerable<Document> documents)
        {
            var objectList = new List<T>();
            foreach (Document document in documents)
            {
                try
                {
                    var item = (T)ConvertToObject<T>(document);

                    if (item == null)
                        throw new Exception("DataType not available in document");

                    objectList.Add(item);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not deserialize document {document.Id} as type {document.DataType}: " + ex.GetBaseException().Message);
                }
            }
            return objectList;
        }

        private IEnumerable<DocumentModel<T>> DeserializeDocumentModels<T>(IEnumerable<Document> documents)
        {
            var objectList = new List<DocumentModel<T>>();
            foreach (Document document in documents)
            {
                try
                {
                    var model = (T)ConvertToObject<T>(document);

                    if (model == null)
                        throw new Exception("DataType not available in document");

                    objectList.Add(new DocumentModel<T> { DocumentId = document.Id, Model = model });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Could not deserialize document {document.Id} as type {document.DataType}: " + ex.GetBaseException().Message);
                }
            }
            return objectList;
        }

        private byte[] ToBinary(object data)
        {
            var type = data.GetType();

            if (type == typeof(string))
            {
                return Encoding.UTF8.GetBytes((string)data);
            }

            if (type == typeof(byte[]))
            {
                return (byte[])data;
            }

            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, _serializerOptions));
        }

        private object ConvertToObject<T>(Document document)
        {
            if (document == null || document.Data == null)
                return default;

            var type = typeof(T);

            if (type == typeof(string))
            {
                return Encoding.UTF8.GetString(document.Data);
            }

            if (type == typeof(byte[]))
            {
                return document.Data;
            }

            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(document.Data), _serializerOptions);
        }

        private async Task<Guid> PersistDocument<T>(T item, Guid? id, bool saveChanges = true)
        {
            var document = id.HasValue && id != Guid.Empty
                ? await _database.Documents.FindAsync(id.Value) ?? CreateNewDocument(item)
                : CreateNewDocument(item);

            document.Data = ToBinary(item);

            if (document.Id == Guid.Empty)
            {
                document.Id = id ?? RT.Comb.Provider.Sql.Create();
                _database.Documents.Add(document);
            }
            else
                _database.Documents.Update(document);

            if (saveChanges)
            {
                await _database.SaveChangesAsync();
            }

            return document.Id;
        }

        private async Task<Guid?> ExistsInternal<T>(IEnumerable<KeyValue> searchProperties)
        {
            IQueryable<Guid> outer = null;
            var entityType = typeof(T).ToString();

            var query = from di in _database.DocumentIndexes
                        join doc in _database.Documents on di.DocumentId equals doc.Id
                        where doc.DataType == entityType
                        select di;

            foreach (var searchProperty in searchProperties)
            {
                var queryable = query
                    .Where(x => x.Key == searchProperty.Key && x.Value == searchProperty.Value)
                    .Select(x => x.DocumentId);

                if (outer == null)
                    outer = queryable;
                else
                    outer = outer.Join(queryable, (left => left), (right => right), (left, right) => left);
            }

            if (outer != null)
            {
                var count = await outer.CountAsync();
                if (count == 1)
                    return await outer.FirstOrDefaultAsync();

                if (count > 1)
                {
                    throw new AmbiguousMatchException("There are more than one match for " +
                                                      string.Join(", ", searchProperties.Select(x => x.ToString())));
                }
            }

            return null;
        }

        #endregion - private -
    }
}