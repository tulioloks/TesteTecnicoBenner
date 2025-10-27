using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TesteTecnicoBenner.Services
{
    public class JsonService<T> 
    {
        private readonly string filePath;

        public JsonService(string path)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Directory.GetParent(basePath).Parent.Parent.FullName;
            filePath = Path.Combine(projectRoot, path);
        }

        public List<T> Carregar()
        {
            if (!File.Exists(filePath)) return new List<T>();
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        public void Salvar(List<T> dados)
        {
            var json = JsonConvert.SerializeObject(dados, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}
