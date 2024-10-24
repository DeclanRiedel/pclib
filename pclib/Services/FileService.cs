using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Npgsql;
using pclib.Models;

namespace pclib.Services
{
    public class FileService
    {
        private readonly string _connectionString;
        private readonly string _localStoragePath;

        public FileService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _localStoragePath = configuration["LocalStoragePath"];
        }

        public async Task<List<FileMetadata>> GetAllFileMetadataAsync()
        {
            var files = new List<FileMetadata>();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT id, filename, bookname, author, filepath FROM filemetadata", connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                files.Add(new FileMetadata
                {
                    Id = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    BookName = reader.GetString(2),
                    Author = reader.GetString(3),
                    FilePath = reader.GetString(4)
                });
            }

            return files;
        }

        public async Task<byte[]> GetFilePreviewAsync(string filePath)
        {
            using var fileStream = new FileStream(Path.Combine(_localStoragePath, filePath), FileMode.Open, FileAccess.Read);
            var buffer = new byte[1000];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            return buffer;
        }

        public async Task<FileMetadata> GetFileMetadataAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new NpgsqlCommand("SELECT id, filename, bookname, author, filepath FROM filemetadata WHERE id = @id", connection);
            command.Parameters.AddWithValue("id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new FileMetadata
                {
                    Id = reader.GetInt32(0),
                    FileName = reader.GetString(1),
                    BookName = reader.GetString(2),
                    Author = reader.GetString(3),
                    FilePath = reader.GetString(4)
                };
            }

            return null;
        }
    }
}
