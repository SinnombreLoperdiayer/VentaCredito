using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace Framework.Servidor.Comun.Util
{
    public class Serializacion
    {
        /// <summary>
        /// Serializa un objeto
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a serializar</typeparam>
        /// <param name="data">Objeto a serializar</param>
        /// <returns>Cadena con la serialización XML</returns>
        public static string Serialize<T>(T data)
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, data);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(memoryStream);
                string content = reader.ReadToEnd();
                return content;
            }
        }

        /// <summary>
        /// Deserializa un objeto a partir de un XML
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a deserializar</typeparam>
        /// <param name="xml">XML con la información del objeto</param>
        /// <returns>Instancia del objeto</returns>
        public static T Deserialize<T>(string xml)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                var serializer = new DataContractSerializer(typeof(T));
                T theObject = (T)serializer.ReadObject(stream);
                return theObject;
            }
        }

       /// <summary>
       /// Deserializa un objeto NO Datacontract desde un xml
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="toDeserialize"></param>
       /// <returns></returns>
        public static T DeserializeObject<T>(string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }

        /// <summary>
        /// Serializa un objeto NODataContract a xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();

        }

        /// <summary>
        /// SerializarObjeto dataContract
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <returns></returns>
        public static string SerializarObjetoDataContract<T>(T objeto)
        {
            try
            {
                /*  var serializer = new DataContractSerializer(typeof(T));
                  var sb = new StringBuilder();
                  using (var writer = XmlWriter.Create(sb))
                  {
                      serializer.WriteObject(writer, objeto);
                      writer.Flush();
                      return sb.ToString();
                  }
              */
                using (MemoryStream memoryStream = new MemoryStream())
                using (StreamReader reader = new StreamReader(memoryStream))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(memoryStream, objeto);
                    memoryStream.Position = 0;
                    return reader.ReadToEnd();
                }


            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Deserializar objeto DataContract
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objeto"></param>
        /// <returns></returns>
        public static T DeserializarObjetoDataContract<T>(string objSerializado)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(objSerializado);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));

                T Retorno = (T)deserializer.ReadObject(stream);

                return Retorno;
            }

        }

    }
}
