using Forsvik.Core.Model.External;
using Forsvik.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Forsvik.Core.Database.Repositories
{
    public class SearchRepository
    {
        public readonly string ConnectionString;
        private readonly ArchivingContext Database;

        public SearchRepository(ArchivingContext database)
        {
            ConnectionString = database.Database.Connection.ConnectionString;
            Database = database;
        }

        public List<SearchModel> Find(string searchText, SearchCategory category)
        {
            var items = new List<SearchModel>();
            var parts = GetSearchParts(searchText);

            if (category != SearchCategory.Folders)
            {
                var fileIds = GetFileIds(parts, category);

                items.AddRange(Database
                    .Files
                    .Where(x => fileIds.Contains(x.Id))
                    .Where(x => x.Folder != null)
                    .ToList()
                    .Select(x => new SearchModel
                    {
                        EntityId = x.Id,
                        EntityType = Model.Context.EntityType.File,
                        FolderId = x.Folder.Id,
                        Name = x.Name,
                        Extension = x.Extension,
                        Description = x.Description,
                        Path = x.Folder.Path,
                        Tags = x.Tags
                    }));
            }

            if (category == SearchCategory.All || category == SearchCategory.Folders)
            {
                var folderIds = GetFolderIds(parts, category);

                items.AddRange(Database
                    .Folders
                    .Where(x => folderIds.Contains(x.Id))
                    .ToList()
                    .Select(x => new SearchModel
                    {
                        EntityId = x.Id,
                        EntityType = Model.Context.EntityType.Folder,
                        FolderId = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Path = x.Path,
                        Tags = x.Tags
                    }));
            }

            return items
                .Take(100)
                .OrderBy(x => x.Name)
                .ToList();
        }

        private List<Guid> GetFileIds(List<string> parts, SearchCategory category)
        {
            var list = new List<Guid>();
            var tags = GetTagIds(parts);

            using (var conn = Connection())
            {
                // ..from Name
                var sql = CreateFileQuery(parts, category, tags);

                using (var cmd = new SqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        list.Add(reader.GetGuid(0));
                    }
                }
            }
            return list;
        }

        private List<Guid> GetFolderIds(List<string> parts, SearchCategory category)
        {
            var list = new List<Guid>();
            var tags = GetTagIds(parts);

            using (var conn = Connection())
            {
                // ..from Name
                var sql = CreateFolderQuery(parts, tags);

                using (var cmd = new SqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader.GetGuid(0));
                    }
                }
            }
            return list;
        }

        private List<Guid> GetTagIds(List<string> parts)
        {
            var list = new List<Guid>();

            using (var conn = Connection())
            {
                var sql = "select EntityId from Tags where ";
                var first = true;

                foreach (var p in parts)
                {
                    if (!first)
                        sql += "or ";

                    sql += $"Text = '{p}' ";
                    first = false;
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(reader.GetGuid(0));
                    }
                }
            }
            return list;
        }

        private string CreateFolderQuery(List<string> parts, List<Guid> tags)
        {
            var sql = "select Id from Folders where ";
            var first = true;

            foreach (var p in parts)
            {
                if (!first)
                    sql += "or ";
                sql += $"Name like '%{p}%' ";
                sql += $"or Description like '%{p}%' ";

                first = false;
            }

            if (tags.Any())
            {
                var ts = string.Join(',', tags.Select(t => $"'{t}'"));
                sql += $"or Id in ({ts}) ";
            }
            return sql;
        }

        private string CreateFileQuery(List<string> parts, SearchCategory category, List<Guid> tags)
        {
            var sql = "select Id from Files where (";
            bool first = true;
            
            foreach (var p in parts)
            {
                if (!first)
                    sql += "or ";
                sql += $"Name like '%{p}%' ";
                sql += $"or Description like '%{p}%' ";

                first = false;
            }
            
            if (tags.Any())
            {
                var ts = string.Join(',', tags.Select(t => $"'{t}'"));
                sql += $"or Id in ({ts}) ";
            }

            sql += ") ";

            if (category == SearchCategory.Documents)
            {
                sql += "and Extension in ('doc','docx','xls','xlsx','pdf','ppt','pptx','txt','ods', 'html','odt','zip')";
            }
            if (category == SearchCategory.Images)
            {
                sql += "and Extension in ('jpg','jpeg','png','gif','avi','mov','wav','wmv','ods', 'html','odt','mp3')";
            }
            return sql;
        }

        private List<string> GetSearchParts(string searchText)
        {
            if (searchText.StartsWith('"') && searchText.EndsWith('"'))
                return new List<string> { searchText.Trim('\"') };

            return searchText
                .Split(' ')
                .Select(x => x.Trim())
                .Where(x => x.HasValue())
                .ToList();
        }

        public List<SearchModel> FindImages(string searchText)
        {
            throw new NotImplementedException();
        }

        public List<SearchModel> FindDocuments(string searchText)
        {
            throw new NotImplementedException();
        }

        public List<SearchModel> FindFolders(string searchText)
        {
            throw new NotImplementedException();
        }

        private SqlConnection Connection()
        {            
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }
    }
}
