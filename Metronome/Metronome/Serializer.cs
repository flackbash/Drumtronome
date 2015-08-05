// Thanks to Louis for contributing this class.

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Metronome
{
    sealed class Serializer
    {
        public void SerializeObject(string filename, SaveData objectToSerialize)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            Stream stream = File.Open(filename, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, objectToSerialize);
            stream.Close();
        }

        public SaveData DeSerializeObject(string filename)
        {
            if (File.Exists(filename) == false)
            {
                return null;
            }
            Stream stream = File.Open(filename, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            SaveData objectToSerialize = (SaveData)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;
        }
    }
}
