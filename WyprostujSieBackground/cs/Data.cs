using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;

namespace WyprostujSieBackground
{
    public class Data
    {
        private readonly bool toSave;

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

        public const string BadPostureKey = "BadPosture";

        public const string NoPersBKey = "NoPersB";
        [JsonProperty(PropertyName = NoPersBKey)]
        public bool NoPersB = true;
        public const string TMPersBKey = "TMPersB";
        [JsonProperty(PropertyName = TMPersBKey)]
        public bool TMPersB = true;

        readonly public string configFolder;
        const string fileName = "wyprostujsie.json";

        private bool DataReadOk() //TODO
        {
            if (File.Exists(Path.Combine(configFolder, fileName)))
                return true;
            else return false;
        }

        public async Task Save()
        {
            if (toSave)
            {
                string serialize = this.ToString();

                using (var sw = new StreamWriter(Path.Combine(configFolder, fileName)))
                {
                    await sw.WriteAsync(serialize);
                }
            }
        }

        private void ReadData()
        {
            string dataStringJson = File.ReadAllText(Path.Combine(configFolder, fileName));
            Data temp = JsonConvert.DeserializeObject<Data>(dataStringJson);

            this.BokAnB = temp.BokAnB;
            this.BokAnD = temp.BokAnD;
            this.NeckAnB = temp.NeckAnB;
            this.NeckAnD = temp.NeckAnD;
            this.SpineAnB = temp.SpineAnB;
            this.SpineAnD = temp.SpineAnD;
            this.NoPersB = temp.NoPersB;
            this.TMPersB = temp.TMPersB;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public Data(bool read, bool toSave)
        {
            this.toSave = toSave;

            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            configFolder = Path.Combine(commonAppData, "WyprostujSie");

            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            if (read)
            {
                if (DataReadOk())
                    ReadData();
            }
        }
    }
}
