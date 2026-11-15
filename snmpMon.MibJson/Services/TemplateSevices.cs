using snmpMon.MibParse.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using snmpMon.MibJson.Models;

namespace snmpMon.MibJson.Services
{
    public class TemplateSevices : ITemplateSevices
    {
        public string ConvertToJson(JsonNode node)
        {
            var jsonStream = JsonConvert.SerializeObject(node);
            if (!string.IsNullOrWhiteSpace(jsonStream))
            {
                var direcctoyPath = GetTemplatesFolderPath();
                string fileName = $"{node.Name}.json";
                var fullPath = Path.Combine(direcctoyPath, fileName);
                CreateNewJsonTemplateFile(jsonStream, fullPath);
                return fullPath;
            }
            return string.Empty;
        }

        public void RemoveJsonFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                
            }
        }
        
        public void ModifyExistsFile(JsonNode node, string path)
        {
            var jsonStream = JsonConvert.SerializeObject(node);
            if(!string.IsNullOrWhiteSpace(jsonStream))
            {
                CreateNewJsonTemplateFile(jsonStream, path);
            }
        }

        private void CreateNewJsonTemplateFile(string jsonStream, string path)
        {
            try
            {
               File.WriteAllText(path, jsonStream);
            }
            catch (Exception ex) { }

        }

        private JsonNode ConvertToObjectNode(string data)
        {
            JsonNode? jsonNode = JsonConvert.DeserializeObject<JsonNode>(data);
            return jsonNode ?? new JsonNode();
        }
        private string GetTemplatesFolderPath()
        {
            string PATH = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "SNMPTEMPLATES");
            CheckForFolderPath(PATH);
            return PATH;
        }

        private void CheckForFolderPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        public List<JsonNode> GetGroupOfJsonNodes(List<string> paths)
        {
            string strFile = string.Empty;
            List<JsonNode> nodes = new();
            foreach (string path in paths)
            {
                strFile = File.ReadAllText(path);
                var node = ConvertToObjectNode(strFile);
                node.TemplatePath = path;
                nodes.Add(node);
            }
            return nodes;
        }
    }
}
