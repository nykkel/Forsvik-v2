using Forsvik.Core.Model.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Forsvik.Core.Database.Repositories
{
    public class DocumentRepository
    {
        private readonly ArchivingContext Database;

        public DocumentRepository(ArchivingContext database)
        {
            Database = database;
        }

        public Guid Save<T>(T data, params Expression<Func<T, object>>[] indexes)
        {
            var keyValues = ExtractIndex(data, indexes);

            var index = FindIndex(keyValues);

            var document = index != null
                ? Database.Documents.Find(index.DocumentId)
                : CreateNewDocument(data);

            document.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            Database.Documents.AddOrUpdate(document);

            if (index == null)
            {
                AddIndex(keyValues, document.Id);
            }

            Database.SaveChanges();

            return document.Id;
        }

        public byte[] GetBinary(Guid id)
        {
            return Database.Documents.Find(id).Data;
        }

        public Guid Save<T>(T data, Guid? id = null)
        {
            var document = id.HasValue
                ? Database.Documents.Find(id.Value)
                : CreateNewDocument(data);

            document.Data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

            Database.Documents.AddOrUpdate(document);
            Database.SaveChanges();

            return document.Id;
        }

        public T Find<T>(params Expression<Func<T, bool>>[] indexes)
        {
            var searchKeys = indexes
                .Select(i => IndexFromExpression(i))
                .ToList();

            var index = FindIndex(searchKeys);

            if (index != null)
            {
                var document = Database.Documents.Find(index.DocumentId);
                var type = Type.GetType(document.DataType);
                var json = Encoding.UTF8.GetString(document.Data);
                return JsonConvert.DeserializeObject<T>(json);
            }

            return default;
        }

        public Guid SaveBinary(byte[] data, Guid? id = null)
        {
            var document = id.HasValue
                ? Database.Documents.Find(id.Value)
                : CreateNewDocument(data);

            document.Data = data;
            Database.Documents.AddOrUpdate(document);
            Database.SaveChanges();

            return document.Id;
        }

        public void AddIndex(Guid documentId, string key, string value)
        {
            Database.DocumentsIndexes.Add(new DocumentIndex
            {
                Id = Guid.NewGuid(),
                Created = DateTime.Now,
                DocumentId = documentId,
                Key = key,
                Value = value
            });
            Database.SaveChanges();
        }

        public string GetIndex(Guid documentId, string key)
        {
            var item = Database
                .DocumentsIndexes
                .Where(x => x.DocumentId == documentId)
                .Where(x => x.Key == key)
                .FirstOrDefault();

            return item != null ? item.Value : null;
        }

        internal void Remove(Guid id)
        {
            Database.DocumentsIndexes.RemoveRange(Database.DocumentsIndexes.Where(x => x.DocumentId == id));
            Database.Documents.Remove(Database.Documents.Find(id));
            Database.SaveChanges();
        }

        public List<KeyValue> GetIndexes(Guid documentId)
        {
            return Database
                .DocumentsIndexes
                .Where(x => x.DocumentId == documentId)
                .ToList()
                .Select(kv => new KeyValue
                {
                    Key = kv.Key,
                    Value = kv.Value
                })
                .ToList();
        }

        public List<T> Search<T>(params Expression<Func<T, bool>>[] indexes)
        {
            // TODO...
            return new List<T>();
        }

        public T Get<T>(Guid id)
        {
            var document = Database.Documents.Find(id);
            var json = Encoding.UTF8.GetString(document.Data);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string GetAsJson(Guid id)
        {
            var document = Database.Documents.Find(id);
            return Encoding.UTF8.GetString(document.Data);
        }

        private KeyValue IndexFromExpression<T>(Expression<Func<T, bool>> expression)
        {
            KeyValue index = new KeyValue();

            if (expression.Body is BinaryExpression be)
            {
                index.Key = (be.Left as MemberExpression).Member.Name;
                if (be.Right is ConstantExpression v)
                {
                    index.Value = v.Value.ToString();
                }
                if (be.Right is MemberExpression memberExpr)
                {
                    if (memberExpr.Member as PropertyInfo != null)
                    {
                        var exp = (MemberExpression)memberExpr.Expression;
                        var constant = (ConstantExpression)exp.Expression;
                        var fieldInfoValue = ((FieldInfo)exp.Member).GetValue(constant.Value);
                        index.Value = ((PropertyInfo)memberExpr.Member).GetValue(fieldInfoValue, null)?.ToString();
                    }
                    else if (memberExpr.Member as FieldInfo != null)
                    {
                        var fieldInfo = memberExpr.Member as FieldInfo;
                        var constantExpression = memberExpr.Expression as ConstantExpression;
                        if (fieldInfo != null & constantExpression != null)
                        {
                            index.Value = fieldInfo.GetValue(constantExpression.Value)?.ToString();
                        }
                    }
                }
            }

            return index.Key != null ? index : null;
        }

        private Document CreateNewDocument(object data)
        {
            return new Document
            {
                Created = DateTime.Now,
                Id = Guid.NewGuid(),
                DataType = data.GetType().AssemblyQualifiedName
            };
        }

        private void AddIndex(List<KeyValue> keyValues, Guid documentId)
        {
            foreach (var kv in keyValues)
            {
                Database.DocumentsIndexes.Add(new DocumentIndex
                {
                    Id = Guid.NewGuid(),
                    DocumentId = documentId,
                    Key = kv.Key,
                    Value = kv.Value,
                    Created = DateTime.Now
                });
            }
        }

        private DocumentIndex FindIndex(List<KeyValue> keyValues)
        {
            var query = Database.DocumentsIndexes.AsQueryable();

            foreach (var kv in keyValues)
            {
                query = query.Where(x => x.Key == kv.Key && x.Value == kv.Value);
            }

            var result = query.GroupBy(x => x.DocumentId).ToList();

            var docGroup = result.FirstOrDefault(x => x.Count() == keyValues.Count);

            if (docGroup != null)
            {
                return docGroup.First();
            }
            return null;
        }

        private List<KeyValue> ExtractIndex<T>(T data, params Expression<Func<T, object>>[] indexes)
        {
            var list = new List<KeyValue>();

            foreach (var lambda in indexes)
            {
                MemberExpression memberExpr = null;

                if (lambda.Body.NodeType == ExpressionType.Convert)
                {
                    memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                }
                else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
                {
                    memberExpr = lambda.Body as MemberExpression;
                }

                list.Add(new KeyValue
                {
                    Key = memberExpr.Member.Name,
                    Value = data.GetType().GetProperty(memberExpr.Member.Name).GetValue(data)?.ToString()
                });
            }
            return list;
        }
    }
}
