using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace snmpMon.MibParse.Models
{
    public class MibNode
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("oid")]
        public string Oid { get; set; }

        [JsonProperty("nodetype")]
        public string NodeType { get; set; } 

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("syntax")]
        public SyntaxInfo Syntax { get; set; } // الكائن المتداخل

        [JsonProperty("description")] // تأكد أن الاسم يطابق الـ JSON تماماً في العقد الأخرى
        public string Desc { get; set; }

        // خاصية ذكية مضافة (شورت كت) لجلب الـ Type مباشرة من كائن الـ Syntax النظيف
        public string ActualType => Syntax?.Type;
    }

    public class SyntaxInfo
    {
        [JsonProperty("type")]
        public string Type { get; set; } // هنا سيقرأ كلمة "INTEGER" بنجاح

        [JsonProperty("class")]
        public string Class { get; set; }
    }
}
