using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Forsvik.Core.DocumentStore.Models;
using Forsvik.Core.Model.Context;

namespace Forsvik.Core.DocumentStore.Contract
{
    public interface IDocumentStore
    {
        Task<Guid> Save<T>(T data, Guid? id = null);
        Task<Guid> Save<T>(T item, params KeyValue[] indexes) where T : class;
        Task<Guid> Save<T>(T item, params Expression<Func<T, object>>[] indexes) where T : class;
        Task<Guid> Save<T>(T item, params string[] tags) where T : class;
        Task<List<Guid>> SaveMany<T>(IEnumerable<T> items, params Expression<Func<T, object>>[] indexes) where T : class;
        Task<List<Guid>> SaveMany<T>(IEnumerable<T> items, IEnumerable<string> tags, Func<T, string> dynamicTag = null) where T : class;
        Task AddIndex(Guid documentId, string key, string value);
        Task AddTag(Guid documentId, string value, bool saveChanges = true);
        Task<string> GetIndex(Guid documentId, string key);
        Task<List<KeyValue>> GetIndexes(Guid documentId);
        Task<T> Get<T>(params Expression<Func<T, bool>>[] indexes);
        Task<T> Get<T>(params KeyValue[] indexes);
        //Task<Guid?> Exists<T>(params Expression<Func<T, bool>>[] indexes);
        Task<Guid?> Exists<T>(params KeyValue[] keyValues);
        Task<bool> ExistsIndex(KeyValue index);
        Task<List<T>> Search<T>(params Expression<Func<T, bool>>[] searchProperties);
        Task<List<T>> Search<T>(params KeyValue[] keyValues);
        Task<List<T>> Search<T>(params string[] tags);
        Task<List<DocumentModel<T>>> SearchModel<T>(params KeyValue[] keyValues);
        Task<List<DocumentModel<T>>> SearchModel<T>(params string[] keyValues);
        Task<List<object>> SearchObject(params string[] tags);
        Task<List<Guid>> SearchDocumentIds(params string[] tags);
        Task<T> Get<T>(Guid id);
        Task Remove(Guid id);
        Task RemoveTagRange(IEnumerable<Guid> documentIds, string tag);
        Task SaveChanges();
    }
}
