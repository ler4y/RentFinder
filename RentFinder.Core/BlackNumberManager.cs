using System.Collections.Generic;
using System.Linq;
using RentFinder.Model;
using System.IO;
using Newtonsoft.Json;

namespace RentFinder.Core
{
    public class BlackNumberManager
    {
        public BlackNumberManager()
        {

        }

        private readonly object _locker = new object();
        private Dictionary<string, List<uint>> _phoneAdDictionary { get; set; } = new Dictionary<string, List<uint>>();

        public void AddAd(PreviewAdModel model)
        {
            foreach (var number in model.PhoneNumbers)
            {
                lock (_locker)
                {
                    if (_phoneAdDictionary.ContainsKey(number))
                    {
                        if (!_phoneAdDictionary[number].Contains(model.AdId))
                            _phoneAdDictionary[number].Add(model.AdId);
                    }
                    else
                        _phoneAdDictionary.Add(number, new List<uint> { model.AdId });
                }
            }
            
        }

        public void BulkAdd(IEnumerable<PreviewAdModel> modelList)
        {
            lock (_locker)
            {
                foreach (var ad in modelList)
                {
                    foreach (var number in ad.PhoneNumbers)
                    {
                        if (_phoneAdDictionary.ContainsKey(number))
                        {
                            if (!_phoneAdDictionary[number].Contains(ad.AdId))
                                _phoneAdDictionary[number].Add(ad.AdId);
                        }
                        else
                            _phoneAdDictionary.Add(number, new List<uint> { ad.AdId });
                    }
                }
            }
        }

        public List<string> GetBlackNumbers(uint maximumAdsForNumber = 2)
        {
            lock (_locker)
            {
                return _phoneAdDictionary.AsParallel().Where(s => s.Value.Count <= maximumAdsForNumber).Select(s => s.Key).ToList();
            }
        }

        public string GetShortReport()
        {
            lock (_locker)
            {
                return _phoneAdDictionary.OrderByDescending(s => s.Value.Count).Select(s => $"{s.Key}: {s.Value.Count}\n").Aggregate((c1, c2) => c1 + c2);
            }
        }

        public string GetLongReport()
        {
            lock (_locker)
            {
                return _phoneAdDictionary.OrderByDescending(s => s.Value.Count)
                    .Select(s => $"{s.Key}:\r\n{s.Value.Select(m=>$"\t{m}\r\n").Aggregate((c1,c2)=> c1+c2)}\r\n")
                    .Aggregate((c1, c2) => c1 + "\r\n" + c2);
            }
        }

        public void SaveToStream(Stream stream)
        {
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(JsonConvert.SerializeObject(_phoneAdDictionary));
            }
        }

        public void LoadFromStream(Stream stream)
        {
            try
            {
                        using (var sr = new StreamReader(stream))
                        {
                            var data = sr.ReadToEnd();
                            _phoneAdDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<uint>>>(data);
                        }
            }
            catch 
            {
                //TODO: action on deserialisation error
            }
        }
    }
}
