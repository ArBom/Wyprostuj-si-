using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Wyprostuj_sie.cs
{
    class Data
    {
        const string SpineAnBKey = "SpineAnB";
        [JsonProperty(PropertyName = SpineAnBKey)]
        public bool SpineAnB = true;
        const string spineAnDKey = "SpineAnD";
        [JsonProperty(PropertyName = spineAnDKey)]
        public double SpineAnD = 0;

        const string BokAnBKey = "BokAnB";
        [JsonProperty(PropertyName = BokAnBKey)]
        public bool BokAnB = true;
        const string BokAnDKey = "BokAnD";
        [JsonProperty(PropertyName = BokAnDKey)]
        public double BokAnD = 0;

        const string NeckAnBKey = "NeckAnB";
        [JsonProperty(PropertyName = NeckAnBKey)]
        public bool NeckAnB = true;
        const string NeckAnKey = "NeckAn";
        [JsonProperty(PropertyName = NeckAnKey)]
        public double NeckAnD = 0;

        const string file = "wyprostujsie.json";

        private bool DataReadOk()
        {
            return false;
        }

        public void Save()
        { //TODO + utworzenie pliku jeśli nie istnieje
            string serialize = this.ToString();
            var commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //var configFolder = Path.Compbine(commonAppData, "MyAppFolder");
        }

        private void ReadData()
        {
            string dataStringJson = ""; //TODO
            Data temp = JsonConvert.DeserializeObject<Data>(dataStringJson);

            this.BokAnB = temp.BokAnB;
            this.BokAnD = temp.BokAnD;
            this.NeckAnB = temp.NeckAnB;
            this.NeckAnD = temp.NeckAnD;
            this.SpineAnB = temp.SpineAnB;
            this.SpineAnD = temp.SpineAnD;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Data()
        {
            if (DataReadOk())
                ReadData();            
        }
    }
}
