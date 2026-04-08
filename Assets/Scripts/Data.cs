using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace QuizCinema
{
    [System.Serializable]
    public class Data
    {
        public Question[] Questions = new Question[0];

        public Data() { }

        public static void Write(Data data, string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Data));
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, data);
            }
        }

        public static Data Fetch(string filePath)
        {
            return Fetch(out bool result, filePath);
        }

        public static Data Fetch(out bool result, string filePath)
        {
            if (!File.Exists(filePath))
            {
                result = false;
                return new Data();
            }

            //Debug.Log(filePath);

            XmlSerializer deserializer = new XmlSerializer(typeof(Data));
            using (Stream stream = new FileStream(filePath, FileMode.Open))
            {
                var data = (Data)deserializer.Deserialize(stream);

                result = true;
                return data;
            }
        }

    }
}